using System;

namespace SensePost.Wikto
{
	/// <summary>
	/// Attribute holds one attribute, as is normally stored in
	/// an HTML or XML file. This includes a name, value and delimiter.
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
	public class Attribute: ICloneable
	{
		private string m_name;
		private string m_value;
		private char m_delim;

		public Attribute(string name,string value,char delim)
		{
			m_name = name;
			m_value = value;
			m_delim = delim;
		}

		public Attribute():this("","",(char)0)
		{
		}

		public Attribute(String name,String value):this(name,value,(char)0)
		{
		}

		public char Delim
		{
			get { return m_delim; }
			set { m_delim = value; }
		}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		#region ICloneable Members
		public virtual object Clone()
		{
			return new Attribute(m_name, m_value, m_delim);		
		}
		#endregion
	}

}
