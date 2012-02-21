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
using Microsoft.Win32;

namespace SensePost.Wikto
{
    public partial class Wizard : Form
    {
        private frm_Wikto frm_wikto;

        public Wizard(frm_Wikto themainform)
        {
            this.frm_wikto = themainform;
            InitializeComponent();
        }

        private void Wizard_Load(object sender, EventArgs e)
        {
            this.cbo_useAI.SelectedIndex = 0;
        }

        private void CancelTheWizard()
        {
            this.Dispose();
        }

        private void btn_cancel1_Click(object sender, EventArgs e)
        {
            CancelTheWizard();
        }

        private void btn_next1_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 1;
        }

        private void btn_back2_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 0;
        }

        private void btn_cancel2_Click(object sender, EventArgs e)
        {
            CancelTheWizard();
        }

        private void btn_next2_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 2;
        }

        private void btn_back3_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 1;
        }

        private void btn_cancel3_Click(object sender, EventArgs e)
        {
            CancelTheWizard();
        }

        private void btn_next3_Click(object sender, EventArgs e)
        {
            lbl_target.Text = cbo_protocol.Text.ToLower() + "://" + txt_host.Text + ":" + txt_port.Text;
            lbl_internet.Text = cbo_UseGoogle.Text;
            if (cbo_proxy.Text == "No") lbl_proxy.Text = "None";
            else lbl_proxy.Text = txt_proxy.Text;
            lbl_ai.Text = cbo_useAI.Text;
            tab_wizard.SelectedIndex = 3;
        }

        private void cbo_proxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_proxy.Text == "Yes")
                txt_proxy.Enabled = true;
            else
                txt_proxy.Enabled = false;
        }

        private void btn_Back4_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 2;
        }

        private void btn_next4_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 4;
        }

        private void btn_cancel4_Click(object sender, EventArgs e)
        {
            CancelTheWizard();
        }

        private void btn_Back5_Click(object sender, EventArgs e)
        {
            tab_wizard.SelectedIndex = 3;
        }

        private void btn_cancel5_Click(object sender, EventArgs e)
        {
            CancelTheWizard();
        }

        private void UpdateTheScreens(object sender, EventArgs e)
        {
            lbl_target.Text = cbo_protocol.Text.ToLower() + "://" + txt_host.Text + ":" + txt_port.Text;
            lbl_internet.Text = cbo_UseGoogle.Text;
            if (cbo_proxy.Text == "No") lbl_proxy.Text = "None";
            else lbl_proxy.Text = txt_proxy.Text;
            lbl_ai.Text = cbo_useAI.Text;
        }

        private void cbo_protocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_protocol.Text == "HTTP")
            {
                txt_port.Text = "80";
            }
            else
            {
                txt_port.Text = "443";
            }
        }

        private void btn_Aura_Click(object sender, EventArgs e)
        {
            frm_wikto.btn_Spud_Click(null, null);
        }

        private void btn_finish_Click(object sender, EventArgs e)
        {
            frm_wikto.SetWizScanData(txt_host.Text, txt_port.Text, cbo_protocol.Text, cbo_UseGoogle.Text, cbo_proxy.Text, txt_proxy.Text, cbo_useAI.Text);
            CancelTheWizard();
        }

    }
}