using System;

namespace SensePost.Wikto
{
	/// <summary>
	/// Summary description for ParseHTML.
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

	public class ParseHTML:Parse 
	{
		public AttributeList GetTag()
		{
			AttributeList tag = new AttributeList();
			tag.Name = m_tag;
			foreach(Attribute x in List) tag.Add((Attribute)x.Clone());
			return tag;
		}

		public String BuildTag()
		{
			String buffer="<";
			buffer+=m_tag;
			int i=0;
			while ( this[i]!=null ) 
			{
				buffer+=" ";
				if ( this[i].Value == null ) 
				{
					if ( this[i].Delim!=0 ) buffer+=this[i].Delim;
					buffer+=this[i].Name;
					if ( this[i].Delim!=0 ) buffer+=this[i].Delim;
				} 
				else 
				{
					buffer+=this[i].Name;
					if ( this[i].Value!=null ) 
					{
						buffer+="=";
						if ( this[i].Delim!=0 ) buffer+=this[i].Delim;
						buffer+=this[i].Value;
						if ( this[i].Delim!=0 ) buffer+=this[i].Delim;
					}
				}
				i++;
			}
			buffer+=">";
			return buffer;
		}

		protected void ParseTag()
		{
			m_tag="";
			Clear();
			if ( (GetCurrentChar()=='!') && (GetCurrentChar(1)=='-')&& (GetCurrentChar(2)=='-') ) 
			{
				while ( !Eof() ) 
				{
					if ( (GetCurrentChar()=='-') && (GetCurrentChar(1)=='-')&& (GetCurrentChar(2)=='>') )
						break;
					if ( GetCurrentChar()!='\r' ) m_tag+=GetCurrentChar();
					Advance();
				}
				m_tag+="--";
				Advance();
				Advance();
				Advance();
				ParseDelim = (char)0;
				return;
			}
			while ( !Eof() ) 
			{
				if ( IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar()=='>') ) break;
				m_tag+=GetCurrentChar();
				Advance();
			}
			EatWhiteSpace();
			while ( GetCurrentChar()!='>' ) 
			{
				ParseName = "";
				ParseValue = "";
				ParseDelim = (char)0;
				ParseAttributeName();
				if ( GetCurrentChar()=='>' ) 
				{
					AddAttribute();
					break;
				}
				ParseAttributeValue();
				AddAttribute();
			}
			Advance();
		}

		public char Parse()
		{
			if( GetCurrentChar()=='<' ) 
			{
				Advance();
				char ch=char.ToUpper(GetCurrentChar());
				if ( (ch>='A') && (ch<='Z') || (ch=='!') || (ch=='/') ) 
				{
					ParseTag();
					return (char)0;
				} 
				else return(AdvanceCurrentChar());
			} 
			else return(AdvanceCurrentChar());
		}
	}
}