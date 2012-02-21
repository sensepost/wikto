using System;
using System.Collections;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace SensePost.Wikto
{
	/// <summary>
	/// The main class for the spider. This spider can be used with the 
	/// SpiderForm form that has been provided. The spider is completely 
	/// selfcontained. If you would like to use the spider with your own
	/// application just remove the references to m_spiderForm from this file.
	/// 
	/// The files needed for the spider are:
	/// 
	/// Attribute.cs - Used by the HTML parser
	/// AttributeList.cs - Used by the HTML parser
	/// DocumentWorker - Used to "thread" the spider
	/// Done.cs - Allows the spider to know when it is done (see comment below)
	/// Parse.cs - Used by the HTML parser
	/// ParseHTML.cs - The HTML parser
	/// Spider.cs - This file
	/// SpiderForm.cs - Demo of how to use the spider
	/// 
	/// This spider is copyright 2003 by Jeff Heaton. However, it is
	/// released under a Limited GNU Public License (LGPL). You may 
	/// use it freely in your own programs. For the latest version visit
	/// http://www.jeffheaton.com.
    /// ****************************************************************
    /// Changes by SensePost (Pty) Ltd. - Ian de Villiers
    /// http://www.sensepost.com
    /// 
    /// Fixed the somewhat broken monitor methods causing thread issues
    /// on VS2005 .Net where threads would not terminate.
    /// 
    /// Removed the Done (Monitor) Class.  Not entirely neccessary and 
    /// actually causes more problems than it assists with in VS 2005.
	/// </summary>
	public class Spider

	{
        private Object thisLock = new Object();
        private int m_totalthreads;
        private DocumentWorker[] worker;
        private Thread[] mythreads;
        private Boolean excludeindexes;
		private Hashtable m_already;
		private Queue m_workload;
		private Uri m_base;
		private string m_outputPath;
        private SensePost.Wikto.frm_Wikto m_spiderForm;
		private bool m_quit;
        private string[] m_excludefilters;
        private string[] m_includetypes;
        private int m_contentlength;
        private string[] m_excludedirs;
        private string[] m_excludeextensions;
        private volatile bool m_AmIBusy;
        private volatile int n_ThreadCount;

        enum Status { STATUS_FAILED, STATUS_SUCCESS, STATUS_QUEUED };
		public Spider(string in_filter, string in_extension, string in_types, int in_length, string in_excdirs, Boolean excludeidx)
		{
            if (in_filter.Length > 0)
                this.m_excludefilters = in_filter.Split(',');
            if (in_extension.Length > 0)
                this.m_excludeextensions = in_extension.Split(',');
            if (in_types.Length > 0)
                this.m_includetypes = in_types.Split(',');
            if (in_excdirs.Length > 0)
                this.m_excludedirs = in_excdirs.Split(',');
            this.m_contentlength = in_length;
            this.excludeindexes = excludeidx;
			reset();
		}

        public bool AmIBusy
        {
            get { return m_AmIBusy; }
            set { m_AmIBusy = value; }
        }

        public void reset()
		{
			m_already = new Hashtable();
			m_workload = new Queue();
			m_quit = false;
		}

        public int FoundResults
        {
            get { return m_already.Count; }
        }

        public int TotalResults
        {
            get { return m_workload.Count + m_already.Count; }
        }

        public void addURI(Uri uri)
		{
            if (!this.m_quit)
            lock (thisLock)
            {
                bool excludeflag = false;
                foreach (string exclude in m_excludefilters)
                {
                    if (uri.ToString().ToLower().IndexOf(exclude.ToLower()) >= 0)
                    {
                        excludeflag = true;
                        break;
                    }
                }
                bool extensionsflag = false;
                foreach (string badext in m_excludeextensions)
                {
                    if (uri.ToString().ToLower().EndsWith(badext))
                    {
                        extensionsflag = true;
                        break;
                    }
                }
                bool directoriesflag = false;
                foreach (string baddir in m_excludedirs)
                {
                    if (uri.ToString().ToLower().Contains(baddir.ToLower()))
                    {
                        directoriesflag = true;
                        break;
                    }
                }
                if ((m_already.Contains(uri)) || (excludeflag || extensionsflag || directoriesflag))
                {
                }
                else
                {
                    try
                    {
                        m_already.Add(uri, Status.STATUS_QUEUED);
                    }
                    catch { }
                    m_workload.Enqueue(uri);
                }
            }
		}

        public Uri BaseURI 
		{
			get
			{
				return m_base;
			}

			set
			{
				m_base = value;
			}
		}

        public string OutputPath
		{
			get
			{
				return m_outputPath;
			}

			set
			{
				m_outputPath = value;
			}
		}

		public SensePost.Wikto.frm_Wikto ReportTo
		{
			get
			{
				return m_spiderForm;
			}

			set
			{
				m_spiderForm = value;
			}
		}

        public int ThreadCount
        {
            get { return n_ThreadCount; }
            set { n_ThreadCount = value; }
        }

		public bool Quit
		{
			get
			{
				return m_quit;
			}

			set
			{
				m_quit = value;
			}
		}

		public Uri ObtainWork()
		{
            //
            Uri next;
            if (m_workload.Count > 0)
            {
                next = (Uri)m_workload.Dequeue();
                lock(thisLock)
                {
                    foreach (String tmpuri in this.m_excludefilters)
                        if (tmpuri.ToLower().Contains(next.ToString().ToLower()))
                        {
                            next = null;
                        }
                }
            }
            else
            {
                next = null;
            }
			return next;
		}

		public void Start(Uri baseURI,int threads)
		{
            n_ThreadCount = 1;
            this.AmIBusy = false;
            worker = new DocumentWorker[threads];
            //Thread[] mythreads = new Thread[threads];
            this.mythreads = new Thread[threads];
			m_totalthreads = threads;
			m_quit = false;
			m_base = baseURI;
			addURI(m_base);
            worker[0] = new DocumentWorker(this, this.m_includetypes, this.m_contentlength, this.m_excludedirs, this.excludeindexes);
            worker[0].ThisSpider = this;
            worker[0].Number = 0;
            worker[0].ProcessFirstPage();

            for (int i = 0; i < threads; i++)
            {
                worker[i] = new DocumentWorker(this, this.m_includetypes, this.m_contentlength, this.m_excludedirs, this.excludeindexes);
                worker[i].ThisSpider = this;
                worker[i].Number = i;
                mythreads[i] = new Thread(new ThreadStart(worker[i].Process));
                mythreads[i].Priority = ThreadPriority.BelowNormal;
                mythreads[i].Start();
                n_ThreadCount++;
            }

            //bool AmIRunning = true;
            while (!this.m_quit)
            {
                // This is a 'orrible 'ack to reduce the CPU overload.
                Thread.Sleep(1000);
                // We have a few options here...
                // 1 - If the number of threads are zero
                // 1.1 - There are items on the work queue
                //     - We restart the thread block...
                // 1.2 - There are no items on the work queue
                //     - We end the process...
                // 2 - If the number of threads is = TotalThreads Allowed.
                //   - We do nothing
                // 3 - If the number of threads is > 0 and < Total threads allowed.
                // 3.1 - There are items on the work queue
                //     - We restart all the threads...
                // 3.2 - There are no items on the work queue
                //     - We do nothing.  It'll happen on the next loop around...
                if (n_ThreadCount <= 1)
                {
                    if (m_workload.Count == 0)
                        this.m_quit = true;
                    else
                    {
                        for (int i = 0; i < threads; i++)
                        {
                            try
                            {
                                if (!mythreads[i].IsAlive)
                                {
                                    worker[i] = new DocumentWorker(this, this.m_includetypes, this.m_contentlength, this.m_excludedirs, this.excludeindexes);
                                    worker[i].ThisSpider = this;
                                    worker[i].Number = i;
                                    mythreads[i] = new Thread(new ThreadStart(worker[i].Process));
                                    mythreads[i].Priority = ThreadPriority.BelowNormal;
                                    mythreads[i].Start();
                                    n_ThreadCount++;
                                }
                            }
                            catch { }
                        }
                    }
                }
                else if ((n_ThreadCount > 1) && (n_ThreadCount < threads))
                {
                    if (m_workload.Count > 0)
                    {
                        for (int i = 0; i < threads; i++)
                        {
                            try
                            {
                                if (!mythreads[i].IsAlive)
                                {
                                    worker[i] = new DocumentWorker(this, this.m_includetypes, this.m_contentlength, this.m_excludedirs, this.excludeindexes);
                                    worker[i].ThisSpider = this;
                                    worker[i].Number = i;
                                    mythreads[i] = new Thread(new ThreadStart(worker[i].Process));
                                    mythreads[i].Priority = ThreadPriority.BelowNormal;
                                    mythreads[i].Start();
                                    n_ThreadCount++;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
		}

        public void End()
        {
            try
            {
                while (m_workload.Count > 0)
                {
                    Uri tmp = (Uri)m_workload.Dequeue();
                }
            }
            catch { }
            this.m_quit = true;
            for (int i = 0; i < m_totalthreads; i++)
            {
                if (worker[i] != null)
                {
                    worker[i].CtlRunning = false;
                    try
                    {
                        worker[i].m_thread.Abort();
                    }
                    catch { }
                }
            }
        }

        public string StripTags(string source)
		{
			try
			{

				string result;

				// Remove HTML Development formatting
				result = source.Replace("\r", " ");												// Replace line breaks with space because browsers inserts space
				result = result.Replace("\n", " ");												// Replace line breaks with space because browsers inserts space
				result = result.Replace("\t", string .Empty);									// Remove step-formatting
				
				result = Regex.Replace(result, @"&nbsp;+", " ", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*br([^>])*>", " ", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"( )+", " ");    // Remove repeating spaces becuase browsers ignore them

				// Remove the header (prepare first by clearing attributes)
				result = Regex.Replace(result, @"<( )*head([^>])*>", "<head>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*head( )*>)", "</head>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, "(<head>).*(</head>)", "#", RegexOptions.IgnoreCase);

				// remove all scripts (prepare first by clearing attributes)
				result = Regex.Replace(result, @"<( )*script([^>])*>", "<script>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*script( )*>)", "</script>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<script>)([^(<script>\.</script>)])*(</script>)", "#", RegexOptions.IgnoreCase);

				result = Regex.Replace(result, @"(<script>)", "\n<script>\n", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(</script>)", "\n</script>\n", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(\n<script>\n).*(\n</script>\n)", "#", RegexOptions.IgnoreCase);

				// remove all styles (prepare first by clearing attributes)
				result = Regex.Replace(result, @"<( )*style([^>])*>", "<style>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*style( )*>)", "</style>", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, "(<style>).*(</style>)", "#", RegexOptions.IgnoreCase);

				// remove all <..> lines we don't need
				result = Regex.Replace(result, @"<( )*table([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*form([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*input([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*html([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*!DOCTYPE([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*body([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*img([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*tr([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*!--([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*div([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*p([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*h[1-9]([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*b([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*nobr([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*font([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*noscript([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*hr([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*option([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*center([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*select([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*ul([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*ilayer([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*iframe([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*blockquote([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*ol([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*object([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*embed([^>])*>", "#", RegexOptions.IgnoreCase);

				// remove leftovers from the previous ones lines
				result = Regex.Replace(result, @"(<( )*(/)( )*td( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*tr( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*html( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*table( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*body( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*div( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*p( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*h[1-9]( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*b( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*nobr( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*font( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*form( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*noscript( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*option( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*center( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*select( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*ul( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*ilayer( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*iframe( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*blockquote( )*>)", "#", RegexOptions.IgnoreCase);	
				result = Regex.Replace(result, @"(<( )*(/)( )*ol( )*>)", "#", RegexOptions.IgnoreCase);	
				result = Regex.Replace(result, @"(<( )*(/)( )*object( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*embed( )*>)", "#", RegexOptions.IgnoreCase);	
				result = Regex.Replace(result, @"(-->)", "#", RegexOptions.IgnoreCase);

				// separate text with # tags
				result = Regex.Replace(result, @"<( )*td([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*span([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*span( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*a([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*a( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*li([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*li( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"<( )*strong([^>])*>", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*strong( )*>)", "#", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"(<( )*(/)( )*br( )*>)", "#", RegexOptions.IgnoreCase);

				// close the gaps between #'s and merge them
				result = Regex.Replace(result, @"^\s*#", "#");
				result = Regex.Replace(result, @"(#( )+#)", "##");
				result = Regex.Replace(result, @"#\s{1,}#", "##");
				result = Regex.Replace(result, @"#+", "#");

				result = Regex.Replace(result, @"&bull;", "*", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&lsaquo;", "<", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&rsaquo;", ">", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&trade;", "(tm)", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&frasl;", "/", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&lt;", "<", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&gt;", ">", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&copy;", "(c)", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&reg;", "(r)", RegexOptions.IgnoreCase);
				result = Regex.Replace(result, @"&amp;", "&", RegexOptions.IgnoreCase);

				//replace single quote so the sql doesn't break
				result = result.Replace("'", string .Empty);
				result = result.Replace("--", "-");

				return result;

			}
			catch
			{
				return source;
			}
		}

    }
}
