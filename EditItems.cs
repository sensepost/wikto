/*
Copyright (C) 2004 SensePost Research

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SensePost.Wikto
{
    public partial class EditItems : Form
    {
        private frm_Wikto the_parent;
        private ListBox the_target;
        public EditItems(frm_Wikto frm, ListBox lst)
        {
            this.the_parent = frm;
            this.the_target = lst;
            InitializeComponent();
            String TheItems = "";
            for (int i = 0; i < the_target.Items.Count; i++)
            {
                String tmp = the_target.Items[i].ToString();
                tmp = tmp.Replace("\r", "");
                if (tmp.Length > 0)
                {
                    if (TheItems.Length == 0)
                        TheItems = tmp;
                    else
                        TheItems = TheItems + "\r\n" + tmp;
                }
            }
            this.txt_Items.Text = TheItems;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            the_parent.Focus();
            this.Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            String[] AllItems = this.txt_Items.Text.Split('\n');
            this.the_target.Items.Clear();
            String tmp = "";
            foreach (String itm in AllItems)
            {
                tmp = itm.Replace("\r", "");
                if (tmp.Length > 0)
                    this.the_target.Items.Add(tmp);
            }
            the_parent.Focus();
            this.Dispose();
        }
    }
}