using System;
using System.Collections;

namespace SensePost.Wikto
{
    /// <summary>
    /// SP_UrlLib is a Url Parsing Class for SensePost's Spider.
    /// </summary>
    class SP_UrlLib
    {
        #region Class Private Variables
        Uri m_Uri;
        #endregion

        #region Class Instantiation and Overrides
        public SP_UrlLib(string url)
        {
            this.m_Uri = new Uri(url);
        }
        #endregion

        #region Public Getters / Setters
        // This will provide the absolute path to the resource being requested.
        // Ex: http://www.sensepost.com/dir/page.moo = /dir/page.moo
        public string AbsolutePath
        {
            get { return this.m_Uri.AbsolutePath.ToString(); }
        }

        // This will provide the absolute uri for the resource being requested.
        // Ex: http://www.sensepost.com/dir/page.moo = http://www.sensepost.com/dir/page.moo
        public string AbsoluteUri
        {
            get { return this.m_Uri.AbsoluteUri.ToString(); }
        }

        // This will provide the authority for the resource being requested.
        // Ex: http://www.sensepost.com/dir/page.moo = www.sensepost.com
        public string Authority
        {
            get { return this.m_Uri.Authority.ToString(); }
        }

        // This will provide the fragments (ie: The bits after the anchor character...
        // Ex: http://www.sensepost.com/dir/page.moo#Where = #Where
        public string Fragment
        {
            get { return this.m_Uri.Fragment.ToString(); }
        }

        // This will provide the hostname used in thne uri
        // Ex: http://www.sensepost.com/dir/page.moo = www.sensepost.com
        public string Host
        {
            get { return this.m_Uri.Host.ToString(); }
        }

        // This will provide a concatenated path and query...
        // Ex: http://www.sensepost.com/dir/page.moo?param=baah = /dir/page.moo?param=baah
        public string PathAndQuery
        {
            get { return this.m_Uri.PathAndQuery.ToString(); }
        }

        // This will provide the port number used in the uri
        // Ex: http://www.sensepost.com/dir/page.moo?param=baah = 80
        public string Port
        {
            get { return this.m_Uri.Port.ToString(); }
        }

        // This will provide the query string section of the url
        // Ex: http://www.sensepost.com/dir/page.moo?param=baah = ?param=baah
        public string Query
        {
            get { return this.m_Uri.Query.ToString(); }
        }

        // This will provide the protocol schema of the url.
        // Ex: http://www.sensepost.com = http
        public string Scheme
        {
            get { return this.m_Uri.Scheme.ToString(); }
        }

        // This sill provide the directory order of the uri.
        // Ex: http://www.sensepost.com/dir1/dir2/file.txt = [/, dir1/, dir2/]
        public string[] Segments
        {
            get { return this.m_Uri.Segments; }
        }
        #endregion

        #region Public Methods
        // When passed with a specific string, this will check to ensure that the url is a child of the specified location.
        // The reason for this is that we want to mirror sites using specific start directories, and this is not going to work
        // if the directories dont stay within the damned things...
        public bool IsChildOf(string url)
        {
            Uri l_Uri = new Uri(url);
            /*if (l_Uri.Host != this.m_Uri.Host) return false;
            if (l_Uri.Port != this.m_Uri.Port) return false;
            if (l_Uri.Scheme != this.m_Uri.Scheme) return false;*/
            if (this.m_Uri.Segments.Length < l_Uri.Segments.Length) return false;
            for (int i = 0; i < l_Uri.Segments.Length; i++) if (l_Uri.Segments[i].Replace("/", "") != this.m_Uri.Segments[i].Replace("/", "")) return false;
            return true;
        }

        // When passed with a specific string, this will check to ensure that the url is on the same web server as the original
        public bool IsSameSite(string url)
        {
            Uri l_Uri = new Uri(url);
            if (l_Uri.Host != this.m_Uri.Host) return false;
            if (l_Uri.Port != this.m_Uri.Port) return false;
            if (l_Uri.Scheme != this.m_Uri.Scheme) return false;
            return true;
        }

        // This one checks that the level of the current resource is within the limit of the number of directories
        // we should drill down into...
        public bool IsWithinTheLevel(string url, int level)
        {
            Uri l_Uri = new Uri(url);
            if (this.m_Uri.Segments[this.m_Uri.Segments.Length - 1].EndsWith("/"))
            {
                if ((this.m_Uri.Segments.Length - l_Uri.Segments.Length) >= level) return false;
                else return true;
            }
            else
            {
                if ((this.m_Uri.Segments.Length - l_Uri.Segments.Length) > level) return false;
                else return true;
            }
        }
        #endregion
    }
}
