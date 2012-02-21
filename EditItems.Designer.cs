namespace SensePost.Wikto
{
    partial class EditItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditItems));
            this.txt_Items = new System.Windows.Forms.TextBox();
            this.btn_OK = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btn_Cancel = new DotNetSkin.SkinControls.SkinButtonRed();
            this.SuspendLayout();
            // 
            // txt_Items
            // 
            this.txt_Items.Dock = System.Windows.Forms.DockStyle.Left;
            this.txt_Items.Location = new System.Drawing.Point(0, 0);
            this.txt_Items.Multiline = true;
            this.txt_Items.Name = "txt_Items";
            this.txt_Items.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_Items.Size = new System.Drawing.Size(442, 516);
            this.txt_Items.TabIndex = 0;
            // 
            // btn_OK
            // 
            this.btn_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_OK.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_OK.Location = new System.Drawing.Point(448, 12);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(86, 28);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_Cancel.ForeColor = System.Drawing.Color.Brown;
            this.btn_Cancel.Location = new System.Drawing.Point(448, 46);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(86, 28);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // EditItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(540, 516);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.txt_Items);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EditItems";
            this.ShowInTaskbar = false;
            this.Text = "Edit Items";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_Items;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_OK;
        private DotNetSkin.SkinControls.SkinButtonRed btn_Cancel;
    }
}