using System;

namespace SensePost.Wikto
{
	/// Base class for parseing tag based files, such as HTML, HTTP headers
	/// or XML.
	/// 
	/// 
	/// This spider is copyright 2003 by Jeff Heaton. However, it is
	/// released under a Limited GNU Public License (LGPL). You may 
	/// use it freely in your own programs. For the latest version visit
	/// http://www.jeffheaton.com.
    ///	****************************************************************
    /// Changes by SensePost (Pty) Ltd. - Ian de Villiers
    /// http://www.sensepost.com
    /// 
    /// Fixed the somewhat broken monitor methods causing thread issues
    /// on VS2005 .Net where threads would not terminate.
    /// 
    /// Removed the Done (Monitor) Class.  Not entirely neccessary and 
    /// actually causes more problems than it assists with in VS 2005.
	public class Parse:AttributeList 
	{
		private string m_source;
		private int m_idx;
		private char m_parseDelim;
		private string m_parseName;
		private string m_parseValue;
		public string m_tag;

        public static bool IsWhiteSpace(char ch)
		{
			return( "\t\n\r ".IndexOf(ch) != -1 );
		}

		public void EatWhiteSpace()
		{
			while ( !Eof() ) 
			{
				if ( !IsWhiteSpace(GetCurrentChar()) ) return;
				m_idx++;
			}
		}

		public bool Eof()
		{
			return(m_idx>=m_source.Length );
		}

		public void ParseAttributeName()
		{
			EatWhiteSpace();
			while ( !Eof() ) 
			{
				if ( IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar()=='=') || (GetCurrentChar()=='>') )
					break;
				m_parseName+=GetCurrentChar();
				m_idx++;
			}
			EatWhiteSpace();
		}

		public void ParseAttributeValue()
		{
			if ( m_parseDelim!=0 ) return;
			if ( GetCurrentChar()=='=' ) 
			{
				m_idx++;
				EatWhiteSpace();
				if ( (GetCurrentChar()=='\'') || (GetCurrentChar()=='\"') ) 
				{
					m_parseDelim = GetCurrentChar();
					m_idx++;
					while ( GetCurrentChar()!=m_parseDelim ) 
					{
						m_parseValue+=GetCurrentChar();
						m_idx++;
					}
					m_idx++;
				} 
				else 
				{
					while ( !Eof() && !IsWhiteSpace(GetCurrentChar()) && (GetCurrentChar()!='>') ) 
					{
						m_parseValue+=GetCurrentChar();
						m_idx++;
					}
				}
				EatWhiteSpace();
			}
		}

		public void AddAttribute()
		{
			Attribute a = new Attribute(m_parseName, m_parseValue,m_parseDelim);
			Add(a);
		}

		public char GetCurrentChar()
		{
			return GetCurrentChar(0);
		}

		public char GetCurrentChar(int peek)
		{
			if( (m_idx+peek)<m_source.Length ) return m_source[m_idx+peek];
			else return (char)0;
		}

		public char AdvanceCurrentChar()
		{
			return m_source[m_idx++];
		}

		public void Advance()
		{
			m_idx++;
		}

		public string ParseName
		{
			get { return m_parseName; }
			set { m_parseName = value; }
		}

		public string ParseValue
		{
			get { return m_parseValue; }
			set { m_parseValue = value; }
		}

		public char ParseDelim
		{
			get { return m_parseDelim; }
			set { m_parseDelim = value; }
		}

		public string Source
		{
			get { return m_source; }
			set { m_source = value; }
		}
	}
}
