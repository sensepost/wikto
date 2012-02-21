namespace SensePost.Wikto
{
    partial class News
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(News));
            this.pnl_Main1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chk_ShowStart = new DotNetSkin.SkinControls.SkinCheckBox();
            this.btn_NewsClose = new DotNetSkin.SkinControls.SkinButtonRed();
            this.chk_StartWiz = new DotNetSkin.SkinControls.SkinCheckBox();
            this.ocx_Browser = new System.Windows.Forms.WebBrowser();
            this.pnl_Main1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_Main1
            // 
            this.pnl_Main1.ColumnCount = 1;
            this.pnl_Main1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnl_Main1.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.pnl_Main1.Controls.Add(this.ocx_Browser, 0, 0);
            this.pnl_Main1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Main1.Location = new System.Drawing.Point(0, 0);
            this.pnl_Main1.Name = "pnl_Main1";
            this.pnl_Main1.RowCount = 2;
            this.pnl_Main1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnl_Main1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.pnl_Main1.Size = new System.Drawing.Size(786, 572);
            this.pnl_Main1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Controls.Add(this.chk_ShowStart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_NewsClose, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_StartWiz, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 543);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(780, 26);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chk_ShowStart
            // 
            this.chk_ShowStart.AutoSize = true;
            this.chk_ShowStart.BackColor = System.Drawing.Color.Transparent;
            this.chk_ShowStart.Checked = true;
            this.chk_ShowStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ShowStart.Dock = System.Windows.Forms.DockStyle.Left;
            this.chk_ShowStart.Location = new System.Drawing.Point(3, 3);
            this.chk_ShowStart.Name = "chk_ShowStart";
            this.chk_ShowStart.Size = new System.Drawing.Size(135, 20);
            this.chk_ShowStart.TabIndex = 0;
            this.chk_ShowStart.Text = "Show News on Startup";
            this.chk_ShowStart.UseVisualStyleBackColor = true;
            this.chk_ShowStart.CheckedChanged += new System.EventHandler(this.chk_ShowStart_CheckedChanged);
            // 
            // btn_NewsClose
            // 
            this.btn_NewsClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_NewsClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_NewsClose.ForeColor = System.Drawing.Color.Brown;
            this.btn_NewsClose.Location = new System.Drawing.Point(692, 3);
            this.btn_NewsClose.Name = "btn_NewsClose";
            this.btn_NewsClose.Size = new System.Drawing.Size(85, 20);
            this.btn_NewsClose.TabIndex = 3;
            this.btn_NewsClose.Text = "Close";
            this.btn_NewsClose.UseVisualStyleBackColor = true;
            this.btn_NewsClose.Click += new System.EventHandler(this.btn_NewsClose_Click);
            // 
            // chk_StartWiz
            // 
            this.chk_StartWiz.AutoSize = true;
            this.chk_StartWiz.BackColor = System.Drawing.Color.Transparent;
            this.chk_StartWiz.Checked = true;
            this.chk_StartWiz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_StartWiz.Dock = System.Windows.Forms.DockStyle.Left;
            this.chk_StartWiz.Location = new System.Drawing.Point(262, 3);
            this.chk_StartWiz.Name = "chk_StartWiz";
            this.chk_StartWiz.Size = new System.Drawing.Size(145, 20);
            this.chk_StartWiz.TabIndex = 4;
            this.chk_StartWiz.Text = "Always Start With Wizard";
            this.chk_StartWiz.UseVisualStyleBackColor = true;
            this.chk_StartWiz.CheckedChanged += new System.EventHandler(this.chk_StartWiz_CheckedChanged);
            // 
            // ocx_Browser
            // 
            this.ocx_Browser.AllowWebBrowserDrop = false;
            this.ocx_Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocx_Browser.IsWebBrowserContextMenuEnabled = false;
            this.ocx_Browser.Location = new System.Drawing.Point(3, 3);
            this.ocx_Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.ocx_Browser.Name = "ocx_Browser";
            this.ocx_Browser.ScriptErrorsSuppressed = true;
            this.ocx_Browser.ScrollBarsEnabled = false;
            this.ocx_Browser.Size = new System.Drawing.Size(780, 534);
            this.ocx_Browser.TabIndex = 1;
            this.ocx_Browser.Url = new System.Uri("http://www.sensepost.com/wiktonews.htm", System.UriKind.Absolute);
            // 
            // News
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 572);
            this.ControlBox = false;
            this.Controls.Add(this.pnl_Main1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "News";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SensePost News";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.News_Load);
            this.pnl_Main1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnl_Main1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DotNetSkin.SkinControls.SkinCheckBox chk_ShowStart;
        private System.Windows.Forms.WebBrowser ocx_Browser;
        private DotNetSkin.SkinControls.SkinButtonRed btn_NewsClose;
        private DotNetSkin.SkinControls.SkinCheckBox chk_StartWiz;


    }
}