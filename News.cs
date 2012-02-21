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

//using System.Drawing;
/*using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SensePost.Wikto.com.google.api;
using System.Xml;
using System.Text.RegularExpressions;*/
using Microsoft.Win32;
//using Org.Mentalis.Security.Ssl;
//using Org.Mentalis.Security.Certificates;

namespace SensePost.Wikto
{
    public partial class News : Form
    {
        private frm_Wikto g_form;
        
        public News(frm_Wikto theform)
        {
            InitializeComponent();
            this.g_form = theform;
            if (g_form.bl_ShowStart)
                chk_ShowStart.Checked = true;
            else
                chk_ShowStart.Checked = false;
            if (g_form.bl_ShowStartWiz)
                chk_StartWiz.Checked = true;
            else
                chk_StartWiz.Checked = false;


        }

        private void btn_NewsClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void setshowstartreg(bool ShowTheForm)
        {
            //now we have to write the filename location to the registry
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey("SOFTWARE", true);
            OurKey.CreateSubKey("SensePost");
            OurKey.CreateSubKey(@"SensePost\Wikto");

            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                if (ShowTheForm)
                {
                    NewKey.SetValue("ShowStart", "1");
                }
                else
                {
                    NewKey.SetValue("ShowStart", "0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems writing to registry:\n\n" + ex.ToString());
            }
            NewKey.Close();
        }

        public void setwizstartreg(bool ShowTheForm)
        {
            //now we have to write the filename location to the registry
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey("SOFTWARE", true);
            OurKey.CreateSubKey("SensePost");
            OurKey.CreateSubKey(@"SensePost\Wikto");

            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                if (ShowTheForm)
                {
                    NewKey.SetValue("ShowWizard", "1");
                }
                else
                {
                    NewKey.SetValue("ShowWizard", "0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems writing to registry:\n\n" + ex.ToString());
            }
            NewKey.Close();
        }
        

        private void chk_ShowStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_ShowStart.Checked)
            {
                setshowstartreg(true);
            }
            else
            {
                setshowstartreg(false);
            }
        }

        private void News_Load(object sender, EventArgs e)
        {

        }

        private void chk_StartWiz_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_StartWiz.Checked)
            {
                setwizstartreg(true);
            }
            else
            {
                setwizstartreg(false);
            }
        }
    }
}