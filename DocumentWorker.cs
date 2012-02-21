using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SensePost.Wikto
{
	/// <summary>
	/// Perform all of the work of a single thread for the spider.
	/// This involves waiting for a URL to become available, download
	/// and then processing the page.
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
    public class DocumentWorker
	{
        private volatile bool ContinueRunning = true;
        private Boolean excludeindexes;
		private Uri m_uri;
		private Spider m_spider;
		public Thread m_thread;
		private int m_number;
		//public const string IndexFile = "index.html";

        private string[] m_content;
        private int m_contentlength;
        private string[] m_excdirs;

        public DocumentWorker() { }
		public DocumentWorker(Spider spider, string[] contenttypes, int contentsize, string[] excdirs, Boolean exclude)
		{
			m_spider = spider;
            m_content = contenttypes;
            m_contentlength = contentsize;
            m_excdirs = excdirs;
            excludeindexes = exclude;
		}

        public bool CtlRunning
        {
            get { return ContinueRunning; }
            set { ContinueRunning = value; }
        }

        public Spider ThisSpider
        {
            get
            {
                return m_spider;
            }
            set
            {
                m_spider = value;
            }
        }

		/// <summary>
		/// This method will take a URI name, such ash /images/blank.gif
		/// and convert it into the name of a file for local storage.
		/// If the directory structure to hold this file does not exist, it
		/// will be created by this method.
		/// </summary>
		/// <param name="uri">The URI of the file about to be stored</param>
		/// <returns></returns>
		/*private string convertFilename(Uri uri)
		{
			string result = m_spider.OutputPath;
			int index1;
			int index2;			

			// add ending slash if needed
			if( result[result.Length-1]!='\\' )
				result = result+"\\";

			// strip the query if needed

			String path = uri.PathAndQuery;
			int queryIndex = path.IndexOf("?");
			if( queryIndex!=-1 )
				path = path.Substring(0,queryIndex);

			// see if an ending / is missing from a directory only	
			int lastSlash = path.LastIndexOf('/');
			int lastDot = path.LastIndexOf('.');

			if( path[path.Length-1]!='/' )
			{
				if(lastSlash>lastDot)
					path+="/"+IndexFile;
			}

			// determine actual filename		
			lastSlash = path.LastIndexOf('/');

			string filename = "";
			if(lastSlash!=-1)
			{
				filename=path.Substring(1+lastSlash);
				path = path.Substring(0,1+lastSlash);
				if(filename.Equals("") )
					filename=IndexFile;
			}


			// create the directory structure, if needed
			index1 = 1;
			do
			{
				index2 = path.IndexOf('/',index1);
				if(index2!=-1)
				{
					String dirpart = path.Substring(index1,index2-index1);
					result+=dirpart;
					result+="\\";
					Directory.CreateDirectory(result);
					index1 = index2+1;					
				}
			} while(index2!=-1);			
			// attach name			
			result+=filename;

			return result;
		}*/

		/// <summary>
		/// Save a binary file to disk.
		/// </summary>
		/// <param name="response">The response used to save the file</param>
		/*private void SaveBinaryFile(WebResponse response)
		{
			byte []buffer = new byte[1024];

			if( m_spider.OutputPath==null )
				return;

			string filename = convertFilename( response.ResponseUri );
			Stream outStream = File.Create( filename );
			Stream inStream = response.GetResponseStream();	
			
			int l;
			do
			{
				l = inStream.Read(buffer,0,buffer.Length);
				if(l>0)
					outStream.Write(buffer,0,l);
			}
			while(l>0);
			
			outStream.Close();
			inStream.Close();

		}*/

		/// <summary>
		/// Save a text file.
		/// </summary>
		/// <param name="buffer">The text to save</param>
		/*private void SaveTextFile(string buffer)
		{
			if( m_spider.OutputPath==null )
				return;

			string filename = convertFilename( m_uri );
			StreamWriter outStream = new StreamWriter( filename );
			outStream.Write(buffer);
			outStream.Close();
		}*/

		/// <summary>
		/// Download a page
		/// </summary>
		/// <returns>The data downloaded from the page</returns>
		private string GetPage()
		{
            bool shouldigo = true;
			WebResponse response = null;
			Stream stream = null;
			StreamReader reader = null;
            foreach (String tmpitm in this.m_excdirs)
            {
                if (m_uri.ToString().ToLower().Contains(tmpitm.ToLower())) return null;
            }
            // We create two new variables - One for using a proxy, and one containing the proxy settings...
            bool UseProxy = false;
            WebProxy proxyObject;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_uri);
                request.Timeout = 10000; 
                UseProxy = this.m_spider.ReportTo.GetProxyEnabled();
                if (UseProxy)
                {
                    string ProxySettings = "http://" + this.m_spider.ReportTo.GetProxySettings();
                    proxyObject = new WebProxy(ProxySettings);
                    request.Proxy = proxyObject;
                }
                ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){return true;};
				response = request.GetResponse();
				stream = response.GetResponseStream();	
                foreach (string c_type in m_content)
                {
                    string s_type = c_type.Replace("*", "");
                    if (!response.ContentType.ToLower().StartsWith(s_type.ToLower()))
                    {
                        shouldigo = false;
                    }
                }
                if (response.ContentLength > m_contentlength * 1024) shouldigo = false;
                if (!shouldigo)
                {
                    request.Abort();
                    response.Close();
                    stream.Close();
                    return null;
                }
				string buffer = "",line;
				reader = new StreamReader(stream);
				while( (line = reader.ReadLine())!=null )
				{
					buffer+=line+"\r\n";
				}
				return buffer;
			}
            catch
            {
                return null;
            }
			finally
			{
				if( reader!=null )
					reader.Close();

				if( stream!=null )
					stream.Close();

				if( response!=null )
					response.Close();
			}
		}

		private void ProcessLink(string link)
		{
			Uri url;
			try
			{
				url = new Uri(m_uri,link);
			}
			catch
			{
				return;
			}
			if(!url.Scheme.ToLower().Equals("http") &&
				!url.Scheme.ToLower().Equals("https") )
				return;
            if (!url.Host.ToLower().Equals(m_uri.Host.ToLower()))
            {
                m_spider.ReportTo.SetLastUrl(url.ToString());
                return;
            }
            // The following line we add 'cos we want to show external links as well as spidered links...
            else
            {
                m_spider.ReportTo.WriteExcluded(url.ToString());
            }
			m_spider.addURI( url );
		}

		private void ProcessPage(string page)
		{
			ParseHTML parse = new ParseHTML();
			parse.Source = page;

            if ((excludeindexes == true) && ((page.ToLower().Contains("to parent directory")) || (page.ToLower().Contains("index of")) || (page.ToLower().Contains("directory listing of"))))
            {
                // indexable items popped here...
            }
            else
            {
                while (!parse.Eof())
                {
                    char ch = parse.Parse();
                    if (ch == 0)
                    {
                        Attribute a = parse.GetTag()["HREF"];
                        if (a != null)
                            ProcessLink(a.Value);

                        a = parse.GetTag()["SRC"];
                        if (a != null)
                            ProcessLink(a.Value);
                        a = null;
                    }
                }
                parse = null;
                System.GC.Collect();
            }
		}

		public void Process()
		{
            while (this.ContinueRunning)
            {
                m_uri = m_spider.ObtainWork();
                if (m_uri == null)
                {
                    this.ContinueRunning = false;
                }
                else
                {
                    string page = GetPage();
                    if (page != null)
                    {
                        ProcessPage(page);
                    }
                    string strTags = m_spider.StripTags(page);
                    //if (strTags != null) m_spider.WriteTags(strTags, m_uri.ToString());
                }
            }
            m_spider.ThreadCount--;
		}


        public void ProcessFirstPage()
        {
            m_uri = m_spider.ObtainWork();
            if (m_uri == null)
            {
                this.ContinueRunning = false;
            }
            else
            {
                 string page = GetPage();
                 if (page != null)
                 {
                     ProcessPage(page);
                 }
                 string strTags = m_spider.StripTags(page);
                 //if (strTags != null) m_spider.WriteTags(strTags, m_uri.ToString());
            }
        }

		/// <summary>
		/// Start the thread.
		/// </summary>
		public void start()
		{
			ThreadStart ts = new ThreadStart( this.Process );
			m_thread = new Thread(ts);
            m_thread.Name = "THREADWORK: " + this.m_number.ToString();
            m_thread.Priority = ThreadPriority.BelowNormal;
			m_thread.Start();
		}

		/// <summary>
		/// The thread number. Used only to identify this thread.
		/// </summary>
		public int Number 
		{
			get
			{
				return m_number;
			}
			set
			{
				m_number = value;
			}
		}
	}
}
