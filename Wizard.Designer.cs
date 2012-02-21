namespace SensePost.Wikto
{
    partial class Wizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.tab_wizard = new System.Windows.Forms.TabControl();
            this.tab_wiz1 = new System.Windows.Forms.TabPage();
            this.btn_cancel1 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_back1 = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_next1 = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.lbl_wel2 = new System.Windows.Forms.Label();
            this.lbl_wel1 = new System.Windows.Forms.Label();
            this.tab_targets = new System.Windows.Forms.TabPage();
            this.cbo_UseGoogle = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_cancel2 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_back2 = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_next2 = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.cbo_protocol = new System.Windows.Forms.ComboBox();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.txt_host = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tab_configuration = new System.Windows.Forms.TabPage();
            this.btn_Aura = new DotNetSkin.SkinControls.SkinButton();
            this.cbo_useAI = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_cancel3 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_back3 = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_next3 = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.txt_proxy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbo_proxy = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tab_confirm = new System.Windows.Forms.TabPage();
            this.btn_cancel4 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_Back4 = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_next4 = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.label18 = new System.Windows.Forms.Label();
            this.lbl_ai = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lbl_proxy = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbl_internet = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbl_target = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tab_overview = new System.Windows.Forms.TabPage();
            this.txt_overview = new System.Windows.Forms.TextBox();
            this.btn_cancel5 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_Back5 = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_finish = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.tab_wizard.SuspendLayout();
            this.tab_wiz1.SuspendLayout();
            this.tab_targets.SuspendLayout();
            this.tab_configuration.SuspendLayout();
            this.tab_confirm.SuspendLayout();
            this.tab_overview.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_wizard
            // 
            this.tab_wizard.Controls.Add(this.tab_wiz1);
            this.tab_wizard.Controls.Add(this.tab_targets);
            this.tab_wizard.Controls.Add(this.tab_configuration);
            this.tab_wizard.Controls.Add(this.tab_confirm);
            this.tab_wizard.Controls.Add(this.tab_overview);
            this.tab_wizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tab_wizard.Location = new System.Drawing.Point(0, 0);
            this.tab_wizard.Name = "tab_wizard";
            this.tab_wizard.SelectedIndex = 0;
            this.tab_wizard.Size = new System.Drawing.Size(499, 257);
            this.tab_wizard.TabIndex = 0;
            this.tab_wizard.SelectedIndexChanged += new System.EventHandler(this.UpdateTheScreens);
            // 
            // tab_wiz1
            // 
            this.tab_wiz1.BackColor = System.Drawing.Color.Transparent;
            this.tab_wiz1.Controls.Add(this.btn_cancel1);
            this.tab_wiz1.Controls.Add(this.btn_back1);
            this.tab_wiz1.Controls.Add(this.btn_next1);
            this.tab_wiz1.Controls.Add(this.lbl_wel2);
            this.tab_wiz1.Controls.Add(this.lbl_wel1);
            this.tab_wiz1.Location = new System.Drawing.Point(4, 22);
            this.tab_wiz1.Name = "tab_wiz1";
            this.tab_wiz1.Padding = new System.Windows.Forms.Padding(3);
            this.tab_wiz1.Size = new System.Drawing.Size(491, 231);
            this.tab_wiz1.TabIndex = 0;
            this.tab_wiz1.Text = "Welcome";
            this.tab_wiz1.UseVisualStyleBackColor = true;
            // 
            // btn_cancel1
            // 
            this.btn_cancel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_cancel1.ForeColor = System.Drawing.Color.Brown;
            this.btn_cancel1.Location = new System.Drawing.Point(325, 202);
            this.btn_cancel1.Name = "btn_cancel1";
            this.btn_cancel1.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel1.TabIndex = 2;
            this.btn_cancel1.Text = "Cancel";
            this.btn_cancel1.UseVisualStyleBackColor = true;
            this.btn_cancel1.Click += new System.EventHandler(this.btn_cancel1_Click);
            // 
            // btn_back1
            // 
            this.btn_back1.Enabled = false;
            this.btn_back1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_back1.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_back1.Location = new System.Drawing.Point(244, 202);
            this.btn_back1.Name = "btn_back1";
            this.btn_back1.Size = new System.Drawing.Size(75, 23);
            this.btn_back1.TabIndex = 3;
            this.btn_back1.Text = "<< Back";
            this.btn_back1.UseVisualStyleBackColor = true;
            // 
            // btn_next1
            // 
            this.btn_next1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_next1.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_next1.Location = new System.Drawing.Point(406, 202);
            this.btn_next1.Name = "btn_next1";
            this.btn_next1.Size = new System.Drawing.Size(75, 23);
            this.btn_next1.TabIndex = 1;
            this.btn_next1.Text = "Next  >>";
            this.btn_next1.UseVisualStyleBackColor = true;
            this.btn_next1.Click += new System.EventHandler(this.btn_next1_Click);
            // 
            // lbl_wel2
            // 
            this.lbl_wel2.AutoSize = true;
            this.lbl_wel2.Location = new System.Drawing.Point(8, 62);
            this.lbl_wel2.Name = "lbl_wel2";
            this.lbl_wel2.Size = new System.Drawing.Size(194, 13);
            this.lbl_wel2.TabIndex = 1;
            this.lbl_wel2.Text = "Click Next to continue or Cancel to quit.";
            // 
            // lbl_wel1
            // 
            this.lbl_wel1.AutoSize = true;
            this.lbl_wel1.Location = new System.Drawing.Point(8, 14);
            this.lbl_wel1.Name = "lbl_wel1";
            this.lbl_wel1.Size = new System.Drawing.Size(481, 13);
            this.lbl_wel1.TabIndex = 0;
            this.lbl_wel1.Text = "This wizard will help you initiate a scan against a specific target by asking you" +
                " a number of questions.";
            // 
            // tab_targets
            // 
            this.tab_targets.BackColor = System.Drawing.Color.Transparent;
            this.tab_targets.Controls.Add(this.cbo_UseGoogle);
            this.tab_targets.Controls.Add(this.label1);
            this.tab_targets.Controls.Add(this.btn_cancel2);
            this.tab_targets.Controls.Add(this.btn_back2);
            this.tab_targets.Controls.Add(this.btn_next2);
            this.tab_targets.Controls.Add(this.cbo_protocol);
            this.tab_targets.Controls.Add(this.txt_port);
            this.tab_targets.Controls.Add(this.txt_host);
            this.tab_targets.Controls.Add(this.label5);
            this.tab_targets.Controls.Add(this.label4);
            this.tab_targets.Controls.Add(this.label3);
            this.tab_targets.Location = new System.Drawing.Point(4, 22);
            this.tab_targets.Name = "tab_targets";
            this.tab_targets.Padding = new System.Windows.Forms.Padding(3);
            this.tab_targets.Size = new System.Drawing.Size(491, 231);
            this.tab_targets.TabIndex = 1;
            this.tab_targets.Text = "Target Selection";
            this.tab_targets.UseVisualStyleBackColor = true;
            // 
            // cbo_UseGoogle
            // 
            this.cbo_UseGoogle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbo_UseGoogle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_UseGoogle.FormattingEnabled = true;
            this.cbo_UseGoogle.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbo_UseGoogle.Location = new System.Drawing.Point(11, 175);
            this.cbo_UseGoogle.Name = "cbo_UseGoogle";
            this.cbo_UseGoogle.Size = new System.Drawing.Size(470, 21);
            this.cbo_UseGoogle.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Is the host Internet-facing ?";
            // 
            // btn_cancel2
            // 
            this.btn_cancel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_cancel2.ForeColor = System.Drawing.Color.Brown;
            this.btn_cancel2.Location = new System.Drawing.Point(325, 202);
            this.btn_cancel2.Name = "btn_cancel2";
            this.btn_cancel2.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel2.TabIndex = 7;
            this.btn_cancel2.Text = "Cancel";
            this.btn_cancel2.UseVisualStyleBackColor = true;
            this.btn_cancel2.Click += new System.EventHandler(this.btn_cancel2_Click);
            // 
            // btn_back2
            // 
            this.btn_back2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_back2.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_back2.Location = new System.Drawing.Point(244, 202);
            this.btn_back2.Name = "btn_back2";
            this.btn_back2.Size = new System.Drawing.Size(75, 23);
            this.btn_back2.TabIndex = 6;
            this.btn_back2.Text = "<< Back";
            this.btn_back2.UseVisualStyleBackColor = true;
            this.btn_back2.Click += new System.EventHandler(this.btn_back2_Click);
            // 
            // btn_next2
            // 
            this.btn_next2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_next2.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_next2.Location = new System.Drawing.Point(406, 202);
            this.btn_next2.Name = "btn_next2";
            this.btn_next2.Size = new System.Drawing.Size(75, 23);
            this.btn_next2.TabIndex = 5;
            this.btn_next2.Text = "Next  >>";
            this.btn_next2.UseVisualStyleBackColor = true;
            this.btn_next2.Click += new System.EventHandler(this.btn_next2_Click);
            // 
            // cbo_protocol
            // 
            this.cbo_protocol.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbo_protocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_protocol.FormattingEnabled = true;
            this.cbo_protocol.Items.AddRange(new object[] {
            "HTTP",
            "HTTPS"});
            this.cbo_protocol.Location = new System.Drawing.Point(11, 78);
            this.cbo_protocol.Name = "cbo_protocol";
            this.cbo_protocol.Size = new System.Drawing.Size(470, 21);
            this.cbo_protocol.TabIndex = 2;
            this.cbo_protocol.SelectedIndexChanged += new System.EventHandler(this.cbo_protocol_SelectedIndexChanged);
            // 
            // txt_port
            // 
            this.txt_port.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txt_port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_port.Location = new System.Drawing.Point(11, 126);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(470, 20);
            this.txt_port.TabIndex = 3;
            // 
            // txt_host
            // 
            this.txt_host.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txt_host.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_host.Location = new System.Drawing.Point(11, 30);
            this.txt_host.Name = "txt_host";
            this.txt_host.Size = new System.Drawing.Size(470, 20);
            this.txt_host.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(283, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Does the web server use SSL (HTTPS) or straight HTTP ?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(204, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "What port does the web server listen on ?";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(341, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "What is the hostname or IP address of the machine you want to scan ?";
            // 
            // tab_configuration
            // 
            this.tab_configuration.Controls.Add(this.btn_Aura);
            this.tab_configuration.Controls.Add(this.cbo_useAI);
            this.tab_configuration.Controls.Add(this.label8);
            this.tab_configuration.Controls.Add(this.btn_cancel3);
            this.tab_configuration.Controls.Add(this.btn_back3);
            this.tab_configuration.Controls.Add(this.btn_next3);
            this.tab_configuration.Controls.Add(this.txt_proxy);
            this.tab_configuration.Controls.Add(this.label6);
            this.tab_configuration.Controls.Add(this.cbo_proxy);
            this.tab_configuration.Controls.Add(this.label2);
            this.tab_configuration.Location = new System.Drawing.Point(4, 22);
            this.tab_configuration.Name = "tab_configuration";
            this.tab_configuration.Size = new System.Drawing.Size(491, 231);
            this.tab_configuration.TabIndex = 2;
            this.tab_configuration.Text = "Configuration";
            this.tab_configuration.UseVisualStyleBackColor = true;
            // 
            // btn_Aura
            // 
            this.btn_Aura.Location = new System.Drawing.Point(11, 119);
            this.btn_Aura.Name = "btn_Aura";
            this.btn_Aura.Size = new System.Drawing.Size(470, 26);
            this.btn_Aura.TabIndex = 4;
            this.btn_Aura.Text = "Start SPUD";
            this.btn_Aura.UseVisualStyleBackColor = true;
            this.btn_Aura.Click += new System.EventHandler(this.btn_Aura_Click);
            // 
            // cbo_useAI
            // 
            this.cbo_useAI.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbo_useAI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_useAI.FormattingEnabled = true;
            this.cbo_useAI.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbo_useAI.Location = new System.Drawing.Point(11, 175);
            this.cbo_useAI.Name = "cbo_useAI";
            this.cbo_useAI.Size = new System.Drawing.Size(470, 21);
            this.cbo_useAI.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 159);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(343, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Do you want to make use of our scanning AI to reduce false positives ?";
            // 
            // btn_cancel3
            // 
            this.btn_cancel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_cancel3.ForeColor = System.Drawing.Color.Brown;
            this.btn_cancel3.Location = new System.Drawing.Point(325, 202);
            this.btn_cancel3.Name = "btn_cancel3";
            this.btn_cancel3.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel3.TabIndex = 8;
            this.btn_cancel3.Text = "Cancel";
            this.btn_cancel3.UseVisualStyleBackColor = true;
            this.btn_cancel3.Click += new System.EventHandler(this.btn_cancel3_Click);
            // 
            // btn_back3
            // 
            this.btn_back3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_back3.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_back3.Location = new System.Drawing.Point(244, 202);
            this.btn_back3.Name = "btn_back3";
            this.btn_back3.Size = new System.Drawing.Size(75, 23);
            this.btn_back3.TabIndex = 7;
            this.btn_back3.Text = "<< Back";
            this.btn_back3.UseVisualStyleBackColor = true;
            this.btn_back3.Click += new System.EventHandler(this.btn_back3_Click);
            // 
            // btn_next3
            // 
            this.btn_next3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_next3.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_next3.Location = new System.Drawing.Point(406, 202);
            this.btn_next3.Name = "btn_next3";
            this.btn_next3.Size = new System.Drawing.Size(75, 23);
            this.btn_next3.TabIndex = 6;
            this.btn_next3.Text = "Next  >>";
            this.btn_next3.UseVisualStyleBackColor = true;
            this.btn_next3.Click += new System.EventHandler(this.btn_next3_Click);
            // 
            // txt_proxy
            // 
            this.txt_proxy.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txt_proxy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_proxy.Enabled = false;
            this.txt_proxy.Location = new System.Drawing.Point(11, 79);
            this.txt_proxy.Name = "txt_proxy";
            this.txt_proxy.Size = new System.Drawing.Size(470, 20);
            this.txt_proxy.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(350, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Please enter the proxy server\'s IP Address and Port (eg: 127.0.0.1:3128):";
            // 
            // cbo_proxy
            // 
            this.cbo_proxy.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbo_proxy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_proxy.FormattingEnabled = true;
            this.cbo_proxy.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbo_proxy.Location = new System.Drawing.Point(11, 30);
            this.cbo_proxy.Name = "cbo_proxy";
            this.cbo_proxy.Size = new System.Drawing.Size(470, 21);
            this.cbo_proxy.TabIndex = 1;
            this.cbo_proxy.SelectedIndexChanged += new System.EventHandler(this.cbo_proxy_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Do you use a proxy server to access this host ?";
            // 
            // tab_confirm
            // 
            this.tab_confirm.Controls.Add(this.btn_cancel4);
            this.tab_confirm.Controls.Add(this.btn_Back4);
            this.tab_confirm.Controls.Add(this.btn_next4);
            this.tab_confirm.Controls.Add(this.label18);
            this.tab_confirm.Controls.Add(this.lbl_ai);
            this.tab_confirm.Controls.Add(this.label16);
            this.tab_confirm.Controls.Add(this.lbl_proxy);
            this.tab_confirm.Controls.Add(this.label14);
            this.tab_confirm.Controls.Add(this.lbl_internet);
            this.tab_confirm.Controls.Add(this.label12);
            this.tab_confirm.Controls.Add(this.lbl_target);
            this.tab_confirm.Controls.Add(this.label10);
            this.tab_confirm.Controls.Add(this.label9);
            this.tab_confirm.Location = new System.Drawing.Point(4, 22);
            this.tab_confirm.Name = "tab_confirm";
            this.tab_confirm.Size = new System.Drawing.Size(491, 231);
            this.tab_confirm.TabIndex = 3;
            this.tab_confirm.Text = "Confirm Settings";
            this.tab_confirm.UseVisualStyleBackColor = true;
            // 
            // btn_cancel4
            // 
            this.btn_cancel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_cancel4.ForeColor = System.Drawing.Color.Brown;
            this.btn_cancel4.Location = new System.Drawing.Point(325, 202);
            this.btn_cancel4.Name = "btn_cancel4";
            this.btn_cancel4.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel4.TabIndex = 3;
            this.btn_cancel4.Text = "Cancel";
            this.btn_cancel4.UseVisualStyleBackColor = true;
            this.btn_cancel4.Click += new System.EventHandler(this.btn_cancel4_Click);
            // 
            // btn_Back4
            // 
            this.btn_Back4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_Back4.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_Back4.Location = new System.Drawing.Point(244, 202);
            this.btn_Back4.Name = "btn_Back4";
            this.btn_Back4.Size = new System.Drawing.Size(75, 23);
            this.btn_Back4.TabIndex = 2;
            this.btn_Back4.Text = "<< Back";
            this.btn_Back4.UseVisualStyleBackColor = true;
            this.btn_Back4.Click += new System.EventHandler(this.btn_Back4_Click);
            // 
            // btn_next4
            // 
            this.btn_next4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_next4.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_next4.Location = new System.Drawing.Point(406, 202);
            this.btn_next4.Name = "btn_next4";
            this.btn_next4.Size = new System.Drawing.Size(75, 23);
            this.btn_next4.TabIndex = 1;
            this.btn_next4.Text = "Next  >>";
            this.btn_next4.UseVisualStyleBackColor = true;
            this.btn_next4.Click += new System.EventHandler(this.btn_next4_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(8, 124);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(221, 13);
            this.label18.TabIndex = 9;
            this.label18.Text = "Press Next to get details on what to do next...";
            // 
            // lbl_ai
            // 
            this.lbl_ai.AutoSize = true;
            this.lbl_ai.Location = new System.Drawing.Point(135, 102);
            this.lbl_ai.Name = "lbl_ai";
            this.lbl_ai.Size = new System.Drawing.Size(0, 13);
            this.lbl_ai.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 102);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(90, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Use AI Scanning:";
            // 
            // lbl_proxy
            // 
            this.lbl_proxy.AutoSize = true;
            this.lbl_proxy.Location = new System.Drawing.Point(135, 80);
            this.lbl_proxy.Name = "lbl_proxy";
            this.lbl_proxy.Size = new System.Drawing.Size(0, 13);
            this.lbl_proxy.TabIndex = 6;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 80);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(100, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Using Proxy Server:";
            // 
            // lbl_internet
            // 
            this.lbl_internet.AutoSize = true;
            this.lbl_internet.Location = new System.Drawing.Point(135, 58);
            this.lbl_internet.Name = "lbl_internet";
            this.lbl_internet.Size = new System.Drawing.Size(0, 13);
            this.lbl_internet.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 58);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Internet Facing:";
            // 
            // lbl_target
            // 
            this.lbl_target.AutoSize = true;
            this.lbl_target.Location = new System.Drawing.Point(135, 36);
            this.lbl_target.Name = "lbl_target";
            this.lbl_target.Size = new System.Drawing.Size(0, 13);
            this.lbl_target.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Scan Target:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(202, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Right - I\'ll setup Wikto to do the following:";
            // 
            // tab_overview
            // 
            this.tab_overview.Controls.Add(this.txt_overview);
            this.tab_overview.Controls.Add(this.btn_cancel5);
            this.tab_overview.Controls.Add(this.btn_Back5);
            this.tab_overview.Controls.Add(this.btn_finish);
            this.tab_overview.Location = new System.Drawing.Point(4, 22);
            this.tab_overview.Name = "tab_overview";
            this.tab_overview.Size = new System.Drawing.Size(491, 231);
            this.tab_overview.TabIndex = 4;
            this.tab_overview.Text = "Overview";
            this.tab_overview.UseVisualStyleBackColor = true;
            // 
            // txt_overview
            // 
            this.txt_overview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_overview.Location = new System.Drawing.Point(3, 3);
            this.txt_overview.Multiline = true;
            this.txt_overview.Name = "txt_overview";
            this.txt_overview.ReadOnly = true;
            this.txt_overview.Size = new System.Drawing.Size(485, 193);
            this.txt_overview.TabIndex = 18;
            this.txt_overview.Text = resources.GetString("txt_overview.Text");
            // 
            // btn_cancel5
            // 
            this.btn_cancel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_cancel5.ForeColor = System.Drawing.Color.Brown;
            this.btn_cancel5.Location = new System.Drawing.Point(325, 202);
            this.btn_cancel5.Name = "btn_cancel5";
            this.btn_cancel5.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel5.TabIndex = 3;
            this.btn_cancel5.Text = "Cancel";
            this.btn_cancel5.UseVisualStyleBackColor = true;
            this.btn_cancel5.Click += new System.EventHandler(this.btn_cancel5_Click);
            // 
            // btn_Back5
            // 
            this.btn_Back5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_Back5.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_Back5.Location = new System.Drawing.Point(244, 202);
            this.btn_Back5.Name = "btn_Back5";
            this.btn_Back5.Size = new System.Drawing.Size(75, 23);
            this.btn_Back5.TabIndex = 2;
            this.btn_Back5.Text = "<< Back";
            this.btn_Back5.UseVisualStyleBackColor = true;
            this.btn_Back5.Click += new System.EventHandler(this.btn_Back5_Click);
            // 
            // btn_finish
            // 
            this.btn_finish.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_finish.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_finish.Location = new System.Drawing.Point(406, 202);
            this.btn_finish.Name = "btn_finish";
            this.btn_finish.Size = new System.Drawing.Size(75, 23);
            this.btn_finish.TabIndex = 1;
            this.btn_finish.Text = "Finish";
            this.btn_finish.UseVisualStyleBackColor = true;
            this.btn_finish.Click += new System.EventHandler(this.btn_finish_Click);
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 257);
            this.ControlBox = false;
            this.Controls.Add(this.tab_wizard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wikto Scan Wizard";
            this.Load += new System.EventHandler(this.Wizard_Load);
            this.tab_wizard.ResumeLayout(false);
            this.tab_wiz1.ResumeLayout(false);
            this.tab_wiz1.PerformLayout();
            this.tab_targets.ResumeLayout(false);
            this.tab_targets.PerformLayout();
            this.tab_configuration.ResumeLayout(false);
            this.tab_configuration.PerformLayout();
            this.tab_confirm.ResumeLayout(false);
            this.tab_confirm.PerformLayout();
            this.tab_overview.ResumeLayout(false);
            this.tab_overview.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab_wizard;
        private System.Windows.Forms.TabPage tab_wiz1;
        private System.Windows.Forms.Label lbl_wel1;
        private System.Windows.Forms.TabPage tab_targets;
        private System.Windows.Forms.Label lbl_wel2;
        private System.Windows.Forms.TextBox txt_host;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbo_protocol;
        private System.Windows.Forms.TextBox txt_port;
        private DotNetSkin.SkinControls.SkinButtonRed btn_cancel1;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_back1;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_next1;
        private System.Windows.Forms.ComboBox cbo_UseGoogle;
        private System.Windows.Forms.Label label1;
        private DotNetSkin.SkinControls.SkinButtonRed btn_cancel2;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_back2;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_next2;
        private System.Windows.Forms.TabPage tab_configuration;
        private System.Windows.Forms.TextBox txt_proxy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbo_proxy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tab_confirm;
        private System.Windows.Forms.ComboBox cbo_useAI;
        private System.Windows.Forms.Label label8;
        private DotNetSkin.SkinControls.SkinButtonRed btn_cancel3;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_back3;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_next3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbl_internet;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbl_target;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lbl_ai;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbl_proxy;
        private DotNetSkin.SkinControls.SkinButtonRed btn_cancel4;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_Back4;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_next4;
        private System.Windows.Forms.TabPage tab_overview;
        private DotNetSkin.SkinControls.SkinButtonRed btn_cancel5;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_Back5;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_finish;
        private System.Windows.Forms.TextBox txt_overview;
        private DotNetSkin.SkinControls.SkinButton btn_Aura;
    }
}