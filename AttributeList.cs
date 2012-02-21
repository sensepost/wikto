using System;
using System.Collections;

namespace SensePost.Wikto
{
	/// <summary>
	/// The AttributeList class is used to store list of
	/// Attribute classes.
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
	/// 
	public class AttributeList:Attribute
	{

		protected ArrayList m_list;
		public override Object Clone()
		{
			AttributeList rtn = new AttributeList();			
			for ( int i=0;i<m_list.Count;i++ ) rtn.Add( (Attribute)this[i].Clone() );
			return rtn;
		}

		public AttributeList():base("","")
		{
			m_list = new ArrayList();
		}

		public void Add(Attribute a)
		{
            try
            {
                m_list.Add(a);
            }
            catch { }
		}

		public void Clear()
		{
			m_list.Clear();
		}

		public bool IsEmpty()
		{
			return( m_list.Count<=0);
		}

		public void Set(string name,string value)
		{
			if ( name==null ) return;
			if ( value==null ) value="";
			Attribute a = this[name];
			if ( a==null ) 
			{
				a = new Attribute(name,value);
				Add(a);
			} 
			else a.Value = value;
		}

		public int Count
		{
			get { return m_list.Count; }
		}

		public ArrayList List
		{
			get { return m_list; }
		}

		public Attribute this[int index]
		{
			get
            { 
                if ( index<m_list.Count ) return(Attribute)m_list[index];
				else return null;
			}
		}

		public Attribute this[string index]
		{
			get 
			{
				int i=0;
				while ( this[i]!=null ) 
				{
					if ( this[i].Name.ToLower().Equals( (index.ToLower()) )) return this[i];
					i++;
				}
				return null;

			}
		}

	}

}
