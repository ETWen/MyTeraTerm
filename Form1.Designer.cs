using System.IO.Ports;
using RJControlslib;

namespace MyTeraTerm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private const int Form1_Size_X = 1200;
        private const int Form1_Size_Y = 600;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem5 = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem6 = new ToolStripMenuItem();
            toolStripMenuItem8 = new ToolStripMenuItem();
            toolStripMenuItem9 = new ToolStripMenuItem();
            toolStripMenuItem10 = new ToolStripMenuItem();
            toolStripMenuItem11 = new ToolStripMenuItem();
            toolStripMenuItem7 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            dbgToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            lblPduIpAddress = new Label();
            tbPduIpAddress = new TextBox();
            btnPduConnect = new Button();
            btnPduPort1 = new RJToggleButton();
            btnPduPort2 = new RJToggleButton();
            btnPduPort3 = new RJToggleButton();
            btnPduPort4 = new RJToggleButton();
            btnPduPort5 = new RJToggleButton();
            btnPduPort6 = new RJToggleButton();
            btnPduPort7 = new RJToggleButton();
            btnPduPort8 = new RJToggleButton();
            btnPduPort9 = new RJToggleButton();
            btnPduPort10 = new RJToggleButton();
            btnPduPort11 = new RJToggleButton();
            btnPduPort12 = new RJToggleButton();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            timerLocalTime = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(Form1_Size_X, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem5, toolStripSeparator2, exitToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(38, 20);
            toolStripMenuItem1.Text = "File";
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new Size(162, 22);
            toolStripMenuItem5.Text = "Log";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(159, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(162, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem6, toolStripMenuItem7 });
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(52, 20);
            toolStripMenuItem2.Text = "Setup";
            // 
            // toolStripMenuItem6
            // 
            toolStripMenuItem6.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem8, toolStripMenuItem9, toolStripMenuItem10, toolStripMenuItem11 });
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            toolStripMenuItem6.Size = new Size(155, 22);
            toolStripMenuItem6.Text = "Multi Terminal";
            // 
            // toolStripMenuItem8
            // 
            toolStripMenuItem8.Checked = true;
            toolStripMenuItem8.CheckOnClick = true;
            toolStripMenuItem8.CheckState = CheckState.Checked;
            toolStripMenuItem8.Name = "toolStripMenuItem8";
            toolStripMenuItem8.Size = new Size(81, 22);
            toolStripMenuItem8.Text = "1";
            toolStripMenuItem8.Click += StripMenu_MultiTerminal_Click;
            // 
            // toolStripMenuItem9
            // 
            toolStripMenuItem9.CheckOnClick = true;
            toolStripMenuItem9.Name = "toolStripMenuItem9";
            toolStripMenuItem9.Size = new Size(81, 22);
            toolStripMenuItem9.Text = "2";
            toolStripMenuItem9.Click += StripMenu_MultiTerminal_Click;
            // 
            // toolStripMenuItem10
            // 
            toolStripMenuItem10.CheckOnClick = true;
            toolStripMenuItem10.Name = "toolStripMenuItem10";
            toolStripMenuItem10.Size = new Size(81, 22);
            toolStripMenuItem10.Text = "4";
            toolStripMenuItem10.Click += StripMenu_MultiTerminal_Click;
            // 
            // toolStripMenuItem11
            // 
            toolStripMenuItem11.CheckOnClick = true;
            toolStripMenuItem11.Name = "toolStripMenuItem11";
            toolStripMenuItem11.Size = new Size(81, 22);
            toolStripMenuItem11.Text = "6";
            toolStripMenuItem11.Click += StripMenu_MultiTerminal_Click;
            // 
            // toolStripMenuItem7
            // 
            toolStripMenuItem7.Name = "toolStripMenuItem7";
            toolStripMenuItem7.Size = new Size(155, 22);
            toolStripMenuItem7.Text = "Release Resource";
            toolStripMenuItem7.Click += stripMenu_Release_Resources_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dbgToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(46, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // dbgToolStripMenuItem
            // 
            dbgToolStripMenuItem.Name = "dbgToolStripMenuItem";
            dbgToolStripMenuItem.Size = new Size(109, 22);
            dbgToolStripMenuItem.Text = "Debug";
            dbgToolStripMenuItem.Click += dbgToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(109, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 24);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(Form1_Size_X, Form1_Size_Y - 46);
            tabControl1.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 100);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(200, 100);
            panel2.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(200, 100);
            panel3.TabIndex = 0;
            // 
            // panel4
            // 
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(200, 100);
            panel4.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(200, 100);
            panel5.TabIndex = 0;
            // 
            // panel6
            // 
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(200, 100);
            panel6.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.Gray;
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(Form1_Size_X - 8, Form1_Size_Y - 74);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Serial Port";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.DarkGray;
            tabPage2.Controls.Add(lblPduIpAddress);
            tabPage2.Controls.Add(tbPduIpAddress);
            tabPage2.Controls.Add(btnPduConnect);
            tabPage2.Controls.Add(btnPduPort1);
            tabPage2.Controls.Add(btnPduPort2);
            tabPage2.Controls.Add(btnPduPort3);
            tabPage2.Controls.Add(btnPduPort4);
            tabPage2.Controls.Add(btnPduPort5);
            tabPage2.Controls.Add(btnPduPort6);
            tabPage2.Controls.Add(btnPduPort7);
            tabPage2.Controls.Add(btnPduPort8);
            tabPage2.Controls.Add(btnPduPort9);
            tabPage2.Controls.Add(btnPduPort10);
            tabPage2.Controls.Add(btnPduPort11);
            tabPage2.Controls.Add(btnPduPort12);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(Form1_Size_X - 8, Form1_Size_Y - 74);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "PDU Control";
            // 
            // lblPduIpAddress
            // 
            lblPduIpAddress.Location = new Point(0, 0);
            lblPduIpAddress.Name = "lblPduIpAddress";
            lblPduIpAddress.Size = new Size(100, 23);
            lblPduIpAddress.TabIndex = 0;
            // 
            // tbPduIpAddress
            // 
            tbPduIpAddress.Location = new Point(0, 0);
            tbPduIpAddress.Name = "tbPduIpAddress";
            tbPduIpAddress.Size = new Size(100, 23);
            tbPduIpAddress.TabIndex = 1;
            // 
            // btnPduConnect
            // 
            btnPduConnect.Location = new Point(0, 0);
            btnPduConnect.Name = "btnPduConnect";
            btnPduConnect.Size = new Size(75, 23);
            btnPduConnect.TabIndex = 2;
            // 
            // btnPduPort1
            // 
            btnPduPort1.Location = new Point(0, 0);
            btnPduPort1.MinimumSize = new Size(45, 22);
            btnPduPort1.Name = "btnPduPort1";
            btnPduPort1.OffBackColor = Color.Gray;
            btnPduPort1.OffToggleColor = Color.Gainsboro;
            btnPduPort1.OnBackColor = Color.MediumSlateBlue;
            btnPduPort1.OnToggleColor = Color.WhiteSmoke;
            btnPduPort1.Size = new Size(104, 24);
            btnPduPort1.TabIndex = 18;
            // 
            // btnPduPort2
            // 
            btnPduPort2.Location = new Point(0, 0);
            btnPduPort2.MinimumSize = new Size(45, 22);
            btnPduPort2.Name = "btnPduPort2";
            btnPduPort2.OffBackColor = Color.Gray;
            btnPduPort2.OffToggleColor = Color.Gainsboro;
            btnPduPort2.OnBackColor = Color.MediumSlateBlue;
            btnPduPort2.OnToggleColor = Color.WhiteSmoke;
            btnPduPort2.Size = new Size(104, 24);
            btnPduPort2.TabIndex = 19;
            // 
            // btnPduPort3
            // 
            btnPduPort3.Location = new Point(0, 0);
            btnPduPort3.MinimumSize = new Size(45, 22);
            btnPduPort3.Name = "btnPduPort3";
            btnPduPort3.OffBackColor = Color.Gray;
            btnPduPort3.OffToggleColor = Color.Gainsboro;
            btnPduPort3.OnBackColor = Color.MediumSlateBlue;
            btnPduPort3.OnToggleColor = Color.WhiteSmoke;
            btnPduPort3.Size = new Size(104, 24);
            btnPduPort3.TabIndex = 20;
            // 
            // btnPduPort4
            // 
            btnPduPort4.Location = new Point(0, 0);
            btnPduPort4.MinimumSize = new Size(45, 22);
            btnPduPort4.Name = "btnPduPort4";
            btnPduPort4.OffBackColor = Color.Gray;
            btnPduPort4.OffToggleColor = Color.Gainsboro;
            btnPduPort4.OnBackColor = Color.MediumSlateBlue;
            btnPduPort4.OnToggleColor = Color.WhiteSmoke;
            btnPduPort4.Size = new Size(104, 24);
            btnPduPort4.TabIndex = 21;
            // 
            // btnPduPort5
            // 
            btnPduPort5.Location = new Point(0, 0);
            btnPduPort5.MinimumSize = new Size(45, 22);
            btnPduPort5.Name = "btnPduPort5";
            btnPduPort5.OffBackColor = Color.Gray;
            btnPduPort5.OffToggleColor = Color.Gainsboro;
            btnPduPort5.OnBackColor = Color.MediumSlateBlue;
            btnPduPort5.OnToggleColor = Color.WhiteSmoke;
            btnPduPort5.Size = new Size(104, 24);
            btnPduPort5.TabIndex = 22;
            // 
            // btnPduPort6
            // 
            btnPduPort6.Location = new Point(0, 0);
            btnPduPort6.MinimumSize = new Size(45, 22);
            btnPduPort6.Name = "btnPduPort6";
            btnPduPort6.OffBackColor = Color.Gray;
            btnPduPort6.OffToggleColor = Color.Gainsboro;
            btnPduPort6.OnBackColor = Color.MediumSlateBlue;
            btnPduPort6.OnToggleColor = Color.WhiteSmoke;
            btnPduPort6.Size = new Size(104, 24);
            btnPduPort6.TabIndex = 23;
            // 
            // btnPduPort7
            // 
            btnPduPort7.Location = new Point(0, 0);
            btnPduPort7.MinimumSize = new Size(45, 22);
            btnPduPort7.Name = "btnPduPort7";
            btnPduPort7.OffBackColor = Color.Gray;
            btnPduPort7.OffToggleColor = Color.Gainsboro;
            btnPduPort7.OnBackColor = Color.MediumSlateBlue;
            btnPduPort7.OnToggleColor = Color.WhiteSmoke;
            btnPduPort7.Size = new Size(104, 24);
            btnPduPort7.TabIndex = 24;
            // 
            // btnPduPort8
            // 
            btnPduPort8.Location = new Point(0, 0);
            btnPduPort8.MinimumSize = new Size(45, 22);
            btnPduPort8.Name = "btnPduPort8";
            btnPduPort8.OffBackColor = Color.Gray;
            btnPduPort8.OffToggleColor = Color.Gainsboro;
            btnPduPort8.OnBackColor = Color.MediumSlateBlue;
            btnPduPort8.OnToggleColor = Color.WhiteSmoke;
            btnPduPort8.Size = new Size(104, 24);
            btnPduPort8.TabIndex = 25;
            // 
            // btnPduPort9
            // 
            btnPduPort9.Location = new Point(0, 0);
            btnPduPort9.MinimumSize = new Size(45, 22);
            btnPduPort9.Name = "btnPduPort9";
            btnPduPort9.OffBackColor = Color.Gray;
            btnPduPort9.OffToggleColor = Color.Gainsboro;
            btnPduPort9.OnBackColor = Color.MediumSlateBlue;
            btnPduPort9.OnToggleColor = Color.WhiteSmoke;
            btnPduPort9.Size = new Size(104, 24);
            btnPduPort9.TabIndex = 26;
            // 
            // btnPduPort10
            // 
            btnPduPort10.Location = new Point(0, 0);
            btnPduPort10.MinimumSize = new Size(45, 22);
            btnPduPort10.Name = "btnPduPort10";
            btnPduPort10.OffBackColor = Color.Gray;
            btnPduPort10.OffToggleColor = Color.Gainsboro;
            btnPduPort10.OnBackColor = Color.MediumSlateBlue;
            btnPduPort10.OnToggleColor = Color.WhiteSmoke;
            btnPduPort10.Size = new Size(104, 24);
            btnPduPort10.TabIndex = 27;
            // 
            // btnPduPort11
            // 
            btnPduPort11.Location = new Point(0, 0);
            btnPduPort11.MinimumSize = new Size(45, 22);
            btnPduPort11.Name = "btnPduPort11";
            btnPduPort11.OffBackColor = Color.Gray;
            btnPduPort11.OffToggleColor = Color.Gainsboro;
            btnPduPort11.OnBackColor = Color.MediumSlateBlue;
            btnPduPort11.OnToggleColor = Color.WhiteSmoke;
            btnPduPort11.Size = new Size(104, 24);
            btnPduPort11.TabIndex = 28;
            // 
            // btnPduPort12
            // 
            btnPduPort12.Location = new Point(0, 0);
            btnPduPort12.MinimumSize = new Size(45, 22);
            btnPduPort12.Name = "btnPduPort12";
            btnPduPort12.OffBackColor = Color.Gray;
            btnPduPort12.OffToggleColor = Color.Gainsboro;
            btnPduPort12.OnBackColor = Color.MediumSlateBlue;
            btnPduPort12.OnToggleColor = Color.WhiteSmoke;
            btnPduPort12.Size = new Size(104, 24);
            btnPduPort12.TabIndex = 29;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 478);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(Form1_Size_X, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(155, 17);
            toolStripStatusLabel1.Text = "Welcome to MyTeraTerm!";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(515, 17);
            toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(200, 17);
            toolStripStatusLabel3.Text = "Local time: yyyy-MM-dd hh:mm:ss";
            toolStripStatusLabel3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // timerLocalTime
            // 
            timerLocalTime.Enabled = true;
            timerLocalTime.Interval = 1000;
            timerLocalTime.Tick += timerLocalTime_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(Form1_Size_X, Form1_Size_Y);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            MinimizeBox = false;
            MinimumSize = new Size(Form1_Size_X, Form1_Size_Y);
            Text = "MyTeraTerm";
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("MyTeraTerm.resource.Choco_256x256.ico"))
                {
                    if (stream != null)
                    {
                        Icon = new Icon(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Failed to load icon: {ex.Message}");
            }
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"MyTeraTerm V{version?.Major}.{version?.Minor}.{version?.Build}";
            Resize += EmbedExeForm_Resize;
            
            // Initialize PDU Control
            InitializePduControl();
            InitializePduUpdateTimer();
            
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Initial Components - Serial Port
        private void InitializeSerialPortComponents()
        {
            // Initialize ComboBoxes
            comboBox1ComPort = new ComboBox();
            comboBox1BaudRate = new ComboBox();
            comboBox2ComPort = new ComboBox();
            comboBox2BaudRate = new ComboBox();
            comboBox3ComPort = new ComboBox();
            comboBox3BaudRate = new ComboBox();
            comboBox4ComPort = new ComboBox();
            comboBox4BaudRate = new ComboBox();
            comboBox5ComPort = new ComboBox();
            comboBox5BaudRate = new ComboBox();
            comboBox6ComPort = new ComboBox();
            comboBox6BaudRate = new ComboBox();

            // Initialize Buttons
            button1Connect = new Button();
            button2Connect = new Button();
            button3Connect = new Button();
            button4Connect = new Button();
            button5Connect = new Button();
            button6Connect = new Button();
            button1RunScript = new Button();
            button2RunScript = new Button();
            button3RunScript = new Button();
            button4RunScript = new Button();
            button5RunScript = new Button();
            button6RunScript = new Button();
            button1EndScript = new Button();
            button2EndScript = new Button();
            button3EndScript = new Button();
            button4EndScript = new Button();
            button5EndScript = new Button();
            button6EndScript = new Button();

            //Initialize Checkboxes
            checkBox1LogEnable = new CheckBox();
            checkBox2LogEnable = new CheckBox();
            checkBox3LogEnable = new CheckBox();
            checkBox4LogEnable = new CheckBox();
            checkBox5LogEnable = new CheckBox();
            checkBox6LogEnable = new CheckBox();
            checkBox1TimestampEnable = new CheckBox();
            checkBox2TimestampEnable = new CheckBox();
            checkBox3TimestampEnable = new CheckBox();
            checkBox4TimestampEnable = new CheckBox();
            checkBox5TimestampEnable = new CheckBox();
            checkBox6TimestampEnable = new CheckBox();

            // Initialize Labels
            label1ScriptStatus = new Label();
            label2ScriptStatus = new Label();
            label3ScriptStatus = new Label();
            label4ScriptStatus = new Label();
            label5ScriptStatus = new Label();
            label6ScriptStatus = new Label();
            lblStatusSerialPort1 = new Label();
            lblStatusSerialPort2 = new Label();
            lblStatusSerialPort3 = new Label();
            lblStatusSerialPort4 = new Label();
            lblStatusSerialPort5 = new Label();
            lblStatusSerialPort6 = new Label();
            lblStatusTX1 = new Label();
            lblStatusRX1 = new Label();
            lblStatusTX2 = new Label();
            lblStatusRX2 = new Label();
            lblStatusTX3 = new Label();
            lblStatusRX3 = new Label();
            lblStatusTX4 = new Label();
            lblStatusRX4 = new Label();
            lblStatusTX5 = new Label();
            lblStatusRX5 = new Label();
            lblStatusTX6 = new Label();
            lblStatusRX6 = new Label();

            // Initialize Panels
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();

            // ======= Panel 1 Controls Components =======
            comboBox1ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1ComPort.FormattingEnabled = true;
            comboBox1ComPort.Location = new Point(0, 0);
            comboBox1ComPort.Name = "comboBox1ComPort";
            comboBox1ComPort.Size = new Size(100, 23);
            comboBox1ComPort.TabIndex = 0;

            comboBox1BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1BaudRate.FormattingEnabled = true;
            comboBox1BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox1BaudRate.Location = new Point(101, 0);
            comboBox1BaudRate.Name = "comboBox1BaudRate";
            comboBox1BaudRate.SelectedIndex = 4; // Default to 115200
            comboBox1BaudRate.Size = new Size(100, 23);
            comboBox1BaudRate.TabIndex = 1;

            button1Connect.Location = new Point(234, 1);
            button1Connect.Name = "button1Connect";
            button1Connect.Size = new Size(84, 23);
            button1Connect.Location = new Point(202, 0);
            button1Connect.TabIndex = 4;
            button1Connect.Text = "Connect";
            button1Connect.UseVisualStyleBackColor = true;
            button1Connect.Click += button1Connect_Click;

            button1RunScript.Location = new Point(303, 0);
            button1RunScript.Name = "button1RunScript";
            button1RunScript.Size = new Size(84, 23);
            button1RunScript.TabIndex = 4;
            button1RunScript.Text = "SCRIPT";
            button1RunScript.UseVisualStyleBackColor = true;
            button1RunScript.Click += buttonRunScript1_Click;

            button1EndScript.Location = new Point(403, 0);
            button1EndScript.Name = "button1EndScript";
            button1EndScript.Size = new Size(84, 23);
            button1EndScript.TabIndex = 4;
            button1EndScript.Text = "END SCRIPT";
            button1EndScript.UseVisualStyleBackColor = true;
            button1EndScript.Visible = false;
            button1EndScript.Click += buttonEndScript1_Click;

            checkBox1LogEnable.AutoSize = true;
            checkBox1LogEnable.Checked = true;
            checkBox1LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox1LogEnable.Location = new Point(0, 50);
            checkBox1LogEnable.Name = "checkBox1LogEnable";
            checkBox1LogEnable.Size = new Size(86, 19);
            checkBox1LogEnable.TabIndex = 5;
            checkBox1LogEnable.Text = "LOG";
            checkBox1LogEnable.UseVisualStyleBackColor = true;

            checkBox1TimestampEnable.AutoSize = true;
            checkBox1TimestampEnable.Checked = true;
            checkBox1TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox1TimestampEnable.Location = new Point(75, 50);
            checkBox1TimestampEnable.Name = "checkBox1TimestampEnable";
            checkBox1TimestampEnable.Size = new Size(86, 19);
            checkBox1TimestampEnable.TabIndex = 6;
            checkBox1TimestampEnable.Text = "TIMESTAMP";
            checkBox1TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort1.AutoSize = true;
            lblStatusSerialPort1.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort1.ForeColor = Color.Black;
            lblStatusSerialPort1.Location = new Point(0, 25);
            lblStatusSerialPort1.Name = "lblStatusSerialPort1";
            lblStatusSerialPort1.Size = new Size(29, 24);
            lblStatusSerialPort1.TabIndex = 3;
            lblStatusSerialPort1.Text = "TX/RX:";

            lblStatusTX1.AutoSize = true;
            lblStatusTX1.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX1.ForeColor = Color.DarkGray;
            lblStatusTX1.Location = new Point(70, 25);
            lblStatusTX1.Name = "lblStatusTX1";
            lblStatusTX1.Size = new Size(29, 24);
            lblStatusTX1.TabIndex = 3;
            lblStatusTX1.Text = "●";

            lblStatusRX1.AutoSize = true;
            lblStatusRX1.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX1.ForeColor = Color.DarkGray;
            lblStatusRX1.Location = new Point(95, 25);
            lblStatusRX1.Name = "lblStatusRX1";
            lblStatusRX1.Size = new Size(29, 24);
            lblStatusRX1.TabIndex = 3;
            lblStatusRX1.Text = "●";

            label1ScriptStatus.AutoSize = true;
            label1ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label1ScriptStatus.Location = new Point(130, 25);
            label1ScriptStatus.Name = "label1ScriptStatus";
            label1ScriptStatus.Size = new Size(110, 24);
            label1ScriptStatus.Text = "SCRIPT - NA";

            // ======= Panel 2 Controls Components =======
            comboBox2ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2ComPort.FormattingEnabled = true;
            comboBox2ComPort.Location = new Point(0, 0);
            comboBox2ComPort.Name = "comboBox2ComPort";
            comboBox2ComPort.Size = new Size(100, 23);
            comboBox2ComPort.TabIndex = 0;

            comboBox2BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2BaudRate.FormattingEnabled = true;
            comboBox2BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox2BaudRate.Location = new Point(101, 0);
            comboBox2BaudRate.Name = "comboBox2BaudRate";
            comboBox2BaudRate.SelectedIndex = 4;
            comboBox2BaudRate.Size = new Size(100, 23);
            comboBox2BaudRate.TabIndex = 1;

            button2Connect.Location = new Point(202, 0);
            button2Connect.Name = "button2Connect";
            button2Connect.Size = new Size(84, 23);
            button2Connect.TabIndex = 4;
            button2Connect.Text = "Connect";
            button2Connect.UseVisualStyleBackColor = true;
            button2Connect.Click += button2Connect_Click;

            button2RunScript.Location = new Point(303, 0);
            button2RunScript.Name = "button2RunScript";
            button2RunScript.Size = new Size(84, 23);
            button2RunScript.TabIndex = 4;
            button2RunScript.Text = "SCRIPT";
            button2RunScript.UseVisualStyleBackColor = true;
            button2RunScript.Click += buttonRunScript2_Click;

            button2EndScript.Location = new Point(403, 0);
            button2EndScript.Name = "button2EndScript";
            button2EndScript.Size = new Size(84, 23);
            button2EndScript.TabIndex = 4;
            button2EndScript.Text = "END SCRIPT";
            button2EndScript.UseVisualStyleBackColor = true;
            button2EndScript.Visible = false;
            button2EndScript.Click += buttonEndScript2_Click;

            checkBox2LogEnable.AutoSize = true;
            checkBox2LogEnable.Checked = true;
            checkBox2LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox2LogEnable.Location = new Point(0, 50);
            checkBox2LogEnable.Name = "checkBox2LogEnable";
            checkBox2LogEnable.Size = new Size(86, 19);
            checkBox2LogEnable.TabIndex = 5;
            checkBox2LogEnable.Text = "LOG";
            checkBox2LogEnable.UseVisualStyleBackColor = true;

            checkBox2TimestampEnable.AutoSize = true;
            checkBox2TimestampEnable.Checked = true;
            checkBox2TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox2TimestampEnable.Location = new Point(75, 50);
            checkBox2TimestampEnable.Name = "checkBox2TimestampEnable";
            checkBox2TimestampEnable.Size = new Size(86, 19);
            checkBox2TimestampEnable.TabIndex = 6;
            checkBox2TimestampEnable.Text = "TIMESTAMP";
            checkBox2TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort2.AutoSize = true;
            lblStatusSerialPort2.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort2.ForeColor = Color.Black;
            lblStatusSerialPort2.Location = new Point(0, 25);
            lblStatusSerialPort2.Name = "lblStatusSerialPort2";
            lblStatusSerialPort2.Size = new Size(29, 24);
            lblStatusSerialPort2.TabIndex = 3;
            lblStatusSerialPort2.Text = "TX/RX:";

            lblStatusTX2.AutoSize = true;
            lblStatusTX2.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX2.ForeColor = Color.DarkGray;
            lblStatusTX2.Location = new Point(70, 25);
            lblStatusTX2.Name = "lblStatusTX2";
            lblStatusTX2.Size = new Size(29, 24);
            lblStatusTX2.TabIndex = 3;
            lblStatusTX2.Text = "●";

            lblStatusRX2.AutoSize = true;
            lblStatusRX2.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX2.ForeColor = Color.DarkGray;
            lblStatusRX2.Location = new Point(95, 25);
            lblStatusRX2.Name = "lblStatusRX2";
            lblStatusRX2.Size = new Size(29, 24);
            lblStatusRX2.TabIndex = 3;
            lblStatusRX2.Text = "●";

            label2ScriptStatus.AutoSize = true;
            label2ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label2ScriptStatus.Location = new Point(130, 25);
            label2ScriptStatus.Name = "label2ScriptStatus";
            label2ScriptStatus.Size = new Size(110, 24);
            label2ScriptStatus.Text = "Script - NA";

            // ======= Panel 3 Controls Components =======
            comboBox3ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3ComPort.FormattingEnabled = true;
            comboBox3ComPort.Location = new Point(0, 0);
            comboBox3ComPort.Name = "comboBox3ComPort";
            comboBox3ComPort.Size = new Size(100, 23);
            comboBox3ComPort.TabIndex = 0;

            comboBox3BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3BaudRate.FormattingEnabled = true;
            comboBox3BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox3BaudRate.Location = new Point(101, 0);
            comboBox3BaudRate.Name = "comboBox3BaudRate";
            comboBox3BaudRate.SelectedIndex = 4;
            comboBox3BaudRate.Size = new Size(100, 23);
            comboBox3BaudRate.TabIndex = 1;

            button3Connect.Location = new Point(202, 0);
            button3Connect.Name = "button3Connect";
            button3Connect.Size = new Size(84, 23);
            button3Connect.TabIndex = 4;
            button3Connect.Text = "Connect";
            button3Connect.UseVisualStyleBackColor = true;
            button3Connect.Click += button3Connect_Click;

            button3RunScript.Location = new Point(303, 0);
            button3RunScript.Name = "button3RunScript";
            button3RunScript.Size = new Size(84, 23);
            button3RunScript.TabIndex = 4;
            button3RunScript.Text = "SCRIPT";
            button3RunScript.UseVisualStyleBackColor = true;
            button3RunScript.Click += buttonRunScript3_Click;

            button3EndScript.Location = new Point(403, 0);
            button3EndScript.Name = "button3EndScript";
            button3EndScript.Size = new Size(84, 23);
            button3EndScript.TabIndex = 4;
            button3EndScript.Text = "END SCRIPT";
            button3EndScript.UseVisualStyleBackColor = true;
            button3EndScript.Visible = false;
            button3EndScript.Click += buttonEndScript3_Click;

            checkBox3LogEnable.AutoSize = true;
            checkBox3LogEnable.Checked = true;
            checkBox3LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox3LogEnable.Location = new Point(0, 50);
            checkBox3LogEnable.Name = "checkBox3LogEnable";
            checkBox3LogEnable.Size = new Size(86, 19);
            checkBox3LogEnable.TabIndex = 5;
            checkBox3LogEnable.Text = "LOG";
            checkBox3LogEnable.UseVisualStyleBackColor = true;

            checkBox3TimestampEnable.AutoSize = true;
            checkBox3TimestampEnable.Checked = true;
            checkBox3TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox3TimestampEnable.Location = new Point(75, 50);
            checkBox3TimestampEnable.Name = "checkBox3TimestampEnable";
            checkBox3TimestampEnable.Size = new Size(86, 19);
            checkBox3TimestampEnable.TabIndex = 6;
            checkBox3TimestampEnable.Text = "TIMESTAMP";
            checkBox3TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort3.AutoSize = true;
            lblStatusSerialPort3.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort3.ForeColor = Color.Black;
            lblStatusSerialPort3.Location = new Point(0, 25);
            lblStatusSerialPort3.Name = "lblStatusSerialPort3";
            lblStatusSerialPort3.Size = new Size(29, 24);
            lblStatusSerialPort3.TabIndex = 3;
            lblStatusSerialPort3.Text = "TX/RX:";

            lblStatusTX3.AutoSize = true;
            lblStatusTX3.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX3.ForeColor = Color.DarkGray;
            lblStatusTX3.Location = new Point(70, 25);
            lblStatusTX3.Name = "lblStatusTX3";
            lblStatusTX3.Size = new Size(29, 24);
            lblStatusTX3.TabIndex = 3;
            lblStatusTX3.Text = "●";

            lblStatusRX3.AutoSize = true;
            lblStatusRX3.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX3.ForeColor = Color.DarkGray;
            lblStatusRX3.Location = new Point(95, 25);
            lblStatusRX3.Name = "lblStatusRX3";
            lblStatusRX3.Size = new Size(29, 24);
            lblStatusRX3.TabIndex = 3;
            lblStatusRX3.Text = "●";

            label3ScriptStatus.AutoSize = true;
            label3ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label3ScriptStatus.Location = new Point(130, 25);
            label3ScriptStatus.Name = "label3ScriptStatus";
            label3ScriptStatus.Size = new Size(110, 24);
            label3ScriptStatus.Text = "Script - NA";

            // ======= Panel 4 Controls Components =======
            comboBox4ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4ComPort.FormattingEnabled = true;
            comboBox4ComPort.Location = new Point(0, 0);
            comboBox4ComPort.Name = "comboBox4ComPort";
            comboBox4ComPort.Size = new Size(100, 23);
            comboBox4ComPort.TabIndex = 0;

            comboBox4BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4BaudRate.FormattingEnabled = true;
            comboBox4BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox4BaudRate.Location = new Point(101, 0);
            comboBox4BaudRate.Name = "comboBox4BaudRate";
            comboBox4BaudRate.SelectedIndex = 4;
            comboBox4BaudRate.Size = new Size(100, 23);
            comboBox4BaudRate.TabIndex = 1;

            button4Connect.Location = new Point(202, 0);
            button4Connect.Name = "button4Connect";
            button4Connect.Size = new Size(84, 23);
            button4Connect.TabIndex = 4;
            button4Connect.Text = "Connect";
            button4Connect.UseVisualStyleBackColor = true;
            button4Connect.Click += button4Connect_Click;

            button4RunScript.Location = new Point(303, 0);
            button4RunScript.Name = "button4RunScript";
            button4RunScript.Size = new Size(84, 23);
            button4RunScript.TabIndex = 4;
            button4RunScript.Text = "SCRIPT";
            button4RunScript.UseVisualStyleBackColor = true;
            button4RunScript.Click += buttonRunScript4_Click;

            button4EndScript.Location = new Point(403, 0);
            button4EndScript.Name = "button4EndScript";
            button4EndScript.Size = new Size(84, 23);
            button4EndScript.TabIndex = 4;
            button4EndScript.Text = "END SCRIPT";
            button4EndScript.UseVisualStyleBackColor = true;
            button4EndScript.Visible = false;
            button4EndScript.Click += buttonEndScript4_Click;

            checkBox4LogEnable.AutoSize = true;
            checkBox4LogEnable.Checked = true;
            checkBox4LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox4LogEnable.Location = new Point(0, 50);
            checkBox4LogEnable.Name = "checkBox4LogEnable";
            checkBox4LogEnable.Size = new Size(86, 19);
            checkBox4LogEnable.TabIndex = 5;
            checkBox4LogEnable.Text = "LOG";
            checkBox4LogEnable.UseVisualStyleBackColor = true;

            checkBox4TimestampEnable.AutoSize = true;
            checkBox4TimestampEnable.Checked = true;
            checkBox4TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox4TimestampEnable.Location = new Point(75, 50);
            checkBox4TimestampEnable.Name = "checkBox4TimestampEnable";
            checkBox4TimestampEnable.Size = new Size(86, 19);
            checkBox4TimestampEnable.TabIndex = 6;
            checkBox4TimestampEnable.Text = "TIMESTAMP";
            checkBox4TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort4.AutoSize = true;
            lblStatusSerialPort4.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort4.ForeColor = Color.Black;
            lblStatusSerialPort4.Location = new Point(0, 25);
            lblStatusSerialPort4.Name = "lblStatusSerialPort4";
            lblStatusSerialPort4.Size = new Size(29, 24);
            lblStatusSerialPort4.TabIndex = 3;
            lblStatusSerialPort4.Text = "TX/RX:";

            lblStatusTX4.AutoSize = true;
            lblStatusTX4.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX4.ForeColor = Color.DarkGray;
            lblStatusTX4.Location = new Point(70, 25);
            lblStatusTX4.Name = "lblStatusTX4";
            lblStatusTX4.Size = new Size(29, 24);
            lblStatusTX4.TabIndex = 3;
            lblStatusTX4.Text = "●";

            lblStatusRX4.AutoSize = true;
            lblStatusRX4.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX4.ForeColor = Color.DarkGray;
            lblStatusRX4.Location = new Point(95, 25);
            lblStatusRX4.Name = "lblStatusRX4";
            lblStatusRX4.Size = new Size(29, 24);
            lblStatusRX4.TabIndex = 3;
            lblStatusRX4.Text = "●";

            label4ScriptStatus.AutoSize = true;
            label4ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label4ScriptStatus.Location = new Point(130, 25);
            label4ScriptStatus.Name = "label4ScriptStatus";
            label4ScriptStatus.Size = new Size(110, 24);
            label4ScriptStatus.Text = "Script - NA";

            // ======= Panel 5 Controls Components =======
            comboBox5ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5ComPort.FormattingEnabled = true;
            comboBox5ComPort.Location = new Point(0, 0);
            comboBox5ComPort.Name = "comboBox5ComPort";
            comboBox5ComPort.Size = new Size(100, 23);
            comboBox5ComPort.TabIndex = 0;

            comboBox5BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5BaudRate.FormattingEnabled = true;
            comboBox5BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox5BaudRate.Location = new Point(101, 0);
            comboBox5BaudRate.Name = "comboBox5BaudRate";
            comboBox5BaudRate.SelectedIndex = 4;
            comboBox5BaudRate.Size = new Size(100, 23);
            comboBox5BaudRate.TabIndex = 1;

            button5Connect.Location = new Point(202, 0);
            button5Connect.Name = "button5Connect";
            button5Connect.Size = new Size(84, 23);
            button5Connect.TabIndex = 4;
            button5Connect.Text = "Connect";
            button5Connect.UseVisualStyleBackColor = true;
            button5Connect.Click += button5Connect_Click;

            button5RunScript.Location = new Point(303, 0);
            button5RunScript.Name = "button5RunScript";
            button5RunScript.Size = new Size(84, 23);
            button5RunScript.TabIndex = 4;
            button5RunScript.Text = "SCRIPT";
            button5RunScript.UseVisualStyleBackColor = true;
            button5RunScript.Click += buttonRunScript5_Click;

            button5EndScript.Location = new Point(403, 0);
            button5EndScript.Name = "button5EndScript";
            button5EndScript.Size = new Size(84, 23);
            button5EndScript.TabIndex = 4;
            button5EndScript.Text = "END SCRIPT";
            button5EndScript.UseVisualStyleBackColor = true;
            button5EndScript.Visible = false;
            button5EndScript.Click += buttonEndScript5_Click;

            checkBox5LogEnable.AutoSize = true;
            checkBox5LogEnable.Checked = true;
            checkBox5LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox5LogEnable.Location = new Point(0, 50);
            checkBox5LogEnable.Name = "checkBox5LogEnable";
            checkBox5LogEnable.Size = new Size(86, 19);
            checkBox5LogEnable.TabIndex = 5;
            checkBox5LogEnable.Text = "LOG";
            checkBox5LogEnable.UseVisualStyleBackColor = true;

            checkBox5TimestampEnable.AutoSize = true;
            checkBox5TimestampEnable.Checked = true;
            checkBox5TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox5TimestampEnable.Location = new Point(75, 50);
            checkBox5TimestampEnable.Name = "checkBox5TimestampEnable";
            checkBox5TimestampEnable.Size = new Size(86, 19);
            checkBox5TimestampEnable.TabIndex = 6;
            checkBox5TimestampEnable.Text = "TIMESTAMP";
            checkBox5TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort5.AutoSize = true;
            lblStatusSerialPort5.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort5.ForeColor = Color.Black;
            lblStatusSerialPort5.Location = new Point(0, 25);
            lblStatusSerialPort5.Name = "lblStatusSerialPort5";
            lblStatusSerialPort5.Size = new Size(29, 24);
            lblStatusSerialPort5.TabIndex = 3;
            lblStatusSerialPort5.Text = "TX/RX:";

            lblStatusTX5.AutoSize = true;
            lblStatusTX5.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX5.ForeColor = Color.DarkGray;
            lblStatusTX5.Location = new Point(70, 25);
            lblStatusTX5.Name = "lblStatusTX5";
            lblStatusTX5.Size = new Size(29, 24);
            lblStatusTX5.TabIndex = 3;
            lblStatusTX5.Text = "●";

            lblStatusRX5.AutoSize = true;
            lblStatusRX5.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX5.ForeColor = Color.DarkGray;
            lblStatusRX5.Location = new Point(95, 25);
            lblStatusRX5.Name = "lblStatusRX5";
            lblStatusRX5.Size = new Size(29, 24);
            lblStatusRX5.TabIndex = 3;
            lblStatusRX5.Text = "●";

            label5ScriptStatus.AutoSize = true;
            label5ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label5ScriptStatus.Location = new Point(130, 25);
            label5ScriptStatus.Name = "label5ScriptStatus";
            label5ScriptStatus.Size = new Size(110, 24);
            label5ScriptStatus.Text = "Script - NA";

            // ======= Panel 6 Controls Components =======
            comboBox6ComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox6ComPort.FormattingEnabled = true;
            comboBox6ComPort.Location = new Point(0, 0);
            comboBox6ComPort.Name = "comboBox6ComPort";
            comboBox6ComPort.Size = new Size(100, 23);
            comboBox6ComPort.TabIndex = 0;

            comboBox6BaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox6BaudRate.FormattingEnabled = true;
            comboBox6BaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBox6BaudRate.Location = new Point(101, 0);
            comboBox6BaudRate.Name = "comboBox6BaudRate";
            comboBox6BaudRate.SelectedIndex = 4;
            comboBox6BaudRate.Size = new Size(100, 23);
            comboBox6BaudRate.TabIndex = 1;

            button6Connect.Location = new Point(202, 0);
            button6Connect.Name = "button6Connect";
            button6Connect.Size = new Size(84, 23);
            button6Connect.TabIndex = 4;
            button6Connect.Text = "Connect";
            button6Connect.UseVisualStyleBackColor = true;
            button6Connect.Click += button6Connect_Click;

            button6RunScript.Location = new Point(303, 0);
            button6RunScript.Name = "button6RunScript";
            button6RunScript.Size = new Size(84, 23);
            button6RunScript.TabIndex = 4;
            button6RunScript.Text = "SCRIPT";
            button6RunScript.UseVisualStyleBackColor = true;
            button6RunScript.Click += buttonRunScript6_Click;

            button6EndScript.Location = new Point(403, 0);
            button6EndScript.Name = "button6EndScript";
            button6EndScript.Size = new Size(84, 23);
            button6EndScript.TabIndex = 4;
            button6EndScript.Text = "END SCRIPT";
            button6EndScript.UseVisualStyleBackColor = true;
            button6EndScript.Visible = false;
            button6EndScript.Click += buttonEndScript6_Click;

            checkBox6LogEnable.AutoSize = true;
            checkBox6LogEnable.Checked = true;
            checkBox6LogEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox6LogEnable.Location = new Point(0, 50);
            checkBox6LogEnable.Name = "checkBox6LogEnable";
            checkBox6LogEnable.Size = new Size(86, 19);
            checkBox6LogEnable.TabIndex = 5;
            checkBox6LogEnable.Text = "LOG";
            checkBox6LogEnable.UseVisualStyleBackColor = true;

            checkBox6TimestampEnable.AutoSize = true;
            checkBox6TimestampEnable.Checked = true;
            checkBox6TimestampEnable.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            checkBox6TimestampEnable.Location = new Point(75, 50);
            checkBox6TimestampEnable.Name = "checkBox6TimestampEnable";
            checkBox6TimestampEnable.Size = new Size(86, 19);
            checkBox6TimestampEnable.TabIndex = 6;
            checkBox6TimestampEnable.Text = "TIMESTAMP";
            checkBox6TimestampEnable.UseVisualStyleBackColor = true;

            lblStatusSerialPort6.AutoSize = true;
            lblStatusSerialPort6.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusSerialPort6.ForeColor = Color.Black;
            lblStatusSerialPort6.Location = new Point(0, 25);
            lblStatusSerialPort6.Name = "lblStatusSerialPort6";
            lblStatusSerialPort6.Size = new Size(29, 24);
            lblStatusSerialPort6.TabIndex = 3;
            lblStatusSerialPort6.Text = "TX/RX:";

            lblStatusTX6.AutoSize = true;
            lblStatusTX6.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusTX6.ForeColor = Color.DarkGray;
            lblStatusTX6.Location = new Point(70, 25);
            lblStatusTX6.Name = "lblStatusTX6";
            lblStatusTX6.Size = new Size(29, 24);
            lblStatusTX6.TabIndex = 3;
            lblStatusTX6.Text = "●";

            lblStatusRX6.AutoSize = true;
            lblStatusRX6.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblStatusRX6.ForeColor = Color.DarkGray;
            lblStatusRX6.Location = new Point(95, 25);
            lblStatusRX6.Name = "lblStatusRX6";
            lblStatusRX6.Size = new Size(29, 24);
            lblStatusRX6.TabIndex = 3;
            lblStatusRX6.Text = "●";

            label6ScriptStatus.AutoSize = true;
            label6ScriptStatus.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label6ScriptStatus.Location = new Point(130, 25);
            label6ScriptStatus.Name = "label6ScriptStatus";
            label6ScriptStatus.Size = new Size(110, 24);
            label6ScriptStatus.Text = "Script - NA";

        }
        #endregion

        #region PDU Control Layout
        private void InitializePduControl()
        {
            dgvPdu = new DataGridView();
            RJControlslib.RJToggleButton[] pduControlButtons = {
                btnPduPort1, btnPduPort2, btnPduPort3, btnPduPort4,
                btnPduPort5, btnPduPort6, btnPduPort7, btnPduPort8,
                btnPduPort9, btnPduPort10, btnPduPort11, btnPduPort12
            };
            for (int i = 0; i < pduControlButtons.Length; i++)
            {
                pduControlButtons[i].CheckedChanged += BtnPduPort_CheckedChanged; //+=: Event Handler
                pduControlButtons[i].Tag = i + 1;  // Using Tag property to store index
            }
            //假設您有 12 個按鈕，Tag 屬性會分別儲存按鈕的索引值，從 1 到 12。
            //當某個按鈕的狀態改變時，可以使用 Tag 屬性來知道是哪個按鈕觸發了事件，然後更新 listViewPdu 中對應的行。

            //
            // dgvPdu
            //
            dgvPdu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPdu.BackgroundColor = Color.DarkGray;
            dgvPdu.BorderStyle = BorderStyle.Fixed3D;
            dgvPdu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPdu.ColumnHeadersHeight = 25;
            dgvPdu.RowHeadersVisible = false;
            dgvPdu.AllowUserToAddRows = false;
            dgvPdu.AllowUserToDeleteRows = false;
            dgvPdu.AllowUserToResizeRows = false;
            dgvPdu.AllowUserToResizeColumns = false;
            dgvPdu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPdu.MultiSelect = false;
            dgvPdu.ReadOnly = true;
            dgvPdu.Location = new Point(65, 44);
            dgvPdu.Size = new Size(404, 358);
            dgvPdu.RowTemplate.Height = LIST_VIEW_ROW_HEIGHT;
            dgvPdu.ScrollBars = ScrollBars.None;
            dgvPdu.Name = "dgvPdu";
            dgvPdu.TabIndex = 20;

            // 加入欄位
            dgvPdu.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Port", 
                HeaderText = "Port", 
                Width = 100,
                ReadOnly = true 
            });
            
            dgvPdu.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Status", 
                HeaderText = "Status", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            
            dgvPdu.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Current", 
                HeaderText = "Current (mA)", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvPdu.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Power", 
                HeaderText = "Power (W)", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            // 加入 12 行資料
            for (int i = 1; i <= 12; i++)
            {
                dgvPdu.Rows.Add($"Port {i}", "Off", "0.000", "00.00");
            }
            
            // 設定行的背景色
            dgvPdu.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == 1) // Status 欄位
                {
                    if (e.Value?.ToString() == "On")
                        dgvPdu.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    else
                        dgvPdu.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
            };
            tabPage2.Controls.Add(dgvPdu);

            // 
            // lblPduIpAddress
            // 
            lblPduIpAddress.AutoSize = true;
            lblPduIpAddress.Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            lblPduIpAddress.Location = new Point(6, 6);
            lblPduIpAddress.Name = "lblPduIpAddress";
            lblPduIpAddress.Size = new Size(84, 19);
            lblPduIpAddress.TabIndex = 5;
            lblPduIpAddress.Text = "IP Address";
            // 
            // tbPduIpAddress
            // 
            tbPduIpAddress.Location = new Point(93, 1);
            tbPduIpAddress.Name = "tbPduIpAddress";
            tbPduIpAddress.Size = new Size(119, 23);
            tbPduIpAddress.TabIndex = 6;
            tbPduIpAddress.Text = "192.168.1.21";
            // 
            // btnPduConnect
            // 
            btnPduConnect.Location = new Point(234, 1);
            btnPduConnect.Name = "btnPduConnect";
            btnPduConnect.Size = new Size(84, 23);
            btnPduConnect.TabIndex = 4;
            btnPduConnect.Text = "Connect";
            btnPduConnect.UseVisualStyleBackColor = true;
            btnPduConnect.Click += btnPduConnect_Click;

            int startY = 44 + 25;
            int buttonSpacing = LIST_VIEW_ROW_HEIGHT;
            // 
            // btnPduPort1
            // 
            btnPduPort1.AutoSize = true;
            btnPduPort1.Location = new Point(14, startY);
            btnPduPort1.MinimumSize = new Size(45, 22);
            btnPduPort1.Name = "btnPduPort1";
            btnPduPort1.OffBackColor = Color.Gray;
            btnPduPort1.OffToggleColor = Color.Gainsboro;
            btnPduPort1.OnBackColor = Color.Lime;
            btnPduPort1.OnToggleColor = Color.WhiteSmoke;
            btnPduPort1.Size = new Size(45, 22);
            btnPduPort1.TabIndex = 16;
            btnPduPort1.UseVisualStyleBackColor = true;
            // 
            // btnPduPort2
            // 
            btnPduPort2.AutoSize = true;
            btnPduPort2.Location = new Point(14, startY + buttonSpacing * 1);
            btnPduPort2.MinimumSize = new Size(45, 22);
            btnPduPort2.Name = "btnPduPort2";
            btnPduPort2.OffBackColor = Color.Gray;
            btnPduPort2.OffToggleColor = Color.Gainsboro;
            btnPduPort2.OnBackColor = Color.Lime;
            btnPduPort2.OnToggleColor = Color.WhiteSmoke;
            btnPduPort2.Size = new Size(45, 22);
            btnPduPort2.TabIndex = 16;
            btnPduPort2.UseVisualStyleBackColor = true;
            // 
            // btnPduPort3
            // 
            btnPduPort3.AutoSize = true;
            btnPduPort3.Location = new Point(14, startY + buttonSpacing * 2);
            btnPduPort3.MinimumSize = new Size(45, 22);
            btnPduPort3.Name = "btnPduPort3";
            btnPduPort3.OffBackColor = Color.Gray;
            btnPduPort3.OffToggleColor = Color.Gainsboro;
            btnPduPort3.OnBackColor = Color.Lime;
            btnPduPort3.OnToggleColor = Color.WhiteSmoke;
            btnPduPort3.Size = new Size(45, 22);
            btnPduPort3.TabIndex = 16;
            btnPduPort3.UseVisualStyleBackColor = true;
            // 
            // btnPduPort4
            // 
            btnPduPort4.AutoSize = true;
            btnPduPort4.Location = new Point(14, startY + buttonSpacing * 3);
            btnPduPort4.MinimumSize = new Size(45, 22);
            btnPduPort4.Name = "btnPduPort4";
            btnPduPort4.OffBackColor = Color.Gray;
            btnPduPort4.OffToggleColor = Color.Gainsboro;
            btnPduPort4.OnBackColor = Color.Lime;
            btnPduPort4.OnToggleColor = Color.WhiteSmoke;
            btnPduPort4.Size = new Size(45, 22);
            btnPduPort4.TabIndex = 16;
            btnPduPort4.UseVisualStyleBackColor = true;
            // 
            // btnPduPort5
            // 
            btnPduPort5.AutoSize = true;
            btnPduPort5.Location = new Point(14, startY + buttonSpacing * 4);
            btnPduPort5.MinimumSize = new Size(45, 22);
            btnPduPort5.Name = "btnPduPort5";
            btnPduPort5.OffBackColor = Color.Gray;
            btnPduPort5.OffToggleColor = Color.Gainsboro;
            btnPduPort5.OnBackColor = Color.Lime;
            btnPduPort5.OnToggleColor = Color.WhiteSmoke;
            btnPduPort5.Size = new Size(45, 22);
            btnPduPort5.TabIndex = 16;
            btnPduPort5.UseVisualStyleBackColor = true;
            // 
            // btnPduPort6
            // 
            btnPduPort6.AutoSize = true;
            btnPduPort6.Location = new Point(14, startY + buttonSpacing * 5);
            btnPduPort6.MinimumSize = new Size(45, 22);
            btnPduPort6.Name = "btnPduPort6";
            btnPduPort6.OffBackColor = Color.Gray;
            btnPduPort6.OffToggleColor = Color.Gainsboro;
            btnPduPort6.OnBackColor = Color.Lime;
            btnPduPort6.OnToggleColor = Color.WhiteSmoke;
            btnPduPort6.Size = new Size(45, 22);
            btnPduPort6.TabIndex = 16;
            btnPduPort6.UseVisualStyleBackColor = true;
            // 
            // btnPduPort7
            // 
            btnPduPort7.AutoSize = true;
            btnPduPort7.Location = new Point(14, startY + buttonSpacing * 6);
            btnPduPort7.MinimumSize = new Size(45, 22);
            btnPduPort7.Name = "btnPduPort7";
            btnPduPort7.OffBackColor = Color.Gray;
            btnPduPort7.OffToggleColor = Color.Gainsboro;
            btnPduPort7.OnBackColor = Color.Lime;
            btnPduPort7.OnToggleColor = Color.WhiteSmoke;
            btnPduPort7.Size = new Size(45, 22);
            btnPduPort7.TabIndex = 16;
            btnPduPort7.UseVisualStyleBackColor = true;
            // 
            // btnPduPort8
            // 
            btnPduPort8.AutoSize = true;
            btnPduPort8.Location = new Point(14, startY + buttonSpacing * 7);
            btnPduPort8.MinimumSize = new Size(45, 22);
            btnPduPort8.Name = "btnPduPort8";
            btnPduPort8.OffBackColor = Color.Gray;
            btnPduPort8.OffToggleColor = Color.Gainsboro;
            btnPduPort8.OnBackColor = Color.Lime;
            btnPduPort8.OnToggleColor = Color.WhiteSmoke;
            btnPduPort8.Size = new Size(45, 22);
            btnPduPort8.TabIndex = 16;
            btnPduPort8.UseVisualStyleBackColor = true;
            // 
            // btnPduPort9
            // 
            btnPduPort9.AutoSize = true;
            btnPduPort9.Location = new Point(14, startY + buttonSpacing * 8);
            btnPduPort9.MinimumSize = new Size(45, 22);
            btnPduPort9.Name = "btnPduPort9";
            btnPduPort9.OffBackColor = Color.Gray;
            btnPduPort9.OffToggleColor = Color.Gainsboro;
            btnPduPort9.OnBackColor = Color.Lime;
            btnPduPort9.OnToggleColor = Color.WhiteSmoke;
            btnPduPort9.Size = new Size(45, 22);
            btnPduPort9.TabIndex = 16;
            btnPduPort9.UseVisualStyleBackColor = true;
            // 
            // btnPduPort10
            // 
            btnPduPort10.AutoSize = true;
            btnPduPort10.Location = new Point(14, startY + buttonSpacing * 9);
            btnPduPort10.MinimumSize = new Size(45, 22);
            btnPduPort10.Name = "btnPduPort10";
            btnPduPort10.OffBackColor = Color.Gray;
            btnPduPort10.OffToggleColor = Color.Gainsboro;
            btnPduPort10.OnBackColor = Color.Lime;
            btnPduPort10.OnToggleColor = Color.WhiteSmoke;
            btnPduPort10.Size = new Size(45, 22);
            btnPduPort10.TabIndex = 16;
            btnPduPort10.UseVisualStyleBackColor = true;
            // 
            // btnPduPort11
            // 
            btnPduPort11.AutoSize = true;
            btnPduPort11.Location = new Point(14, startY + buttonSpacing * 10);
            btnPduPort11.MinimumSize = new Size(45, 22);
            btnPduPort11.Name = "btnPduPort11";
            btnPduPort11.OffBackColor = Color.Gray;
            btnPduPort11.OffToggleColor = Color.Gainsboro;
            btnPduPort11.OnBackColor = Color.Lime;
            btnPduPort11.OnToggleColor = Color.WhiteSmoke;
            btnPduPort11.Size = new Size(45, 22);
            btnPduPort11.TabIndex = 16;
            btnPduPort11.UseVisualStyleBackColor = true;
            // 
            // btnPduPort12
            // 
            btnPduPort12.AutoSize = true;
            btnPduPort12.Location = new Point(14, startY + buttonSpacing * 11);
            btnPduPort12.MinimumSize = new Size(45, 22);
            btnPduPort12.Name = "btnPduPort12";
            btnPduPort12.OffBackColor = Color.Gray;
            btnPduPort12.OffToggleColor = Color.Gainsboro;
            btnPduPort12.OnBackColor = Color.Lime;
            btnPduPort12.OnToggleColor = Color.WhiteSmoke;
            btnPduPort12.Size = new Size(45, 22);
            btnPduPort12.TabIndex = 16;
            btnPduPort12.UseVisualStyleBackColor = true;
        }
        
        private void InitializePduUpdateTimer()
        {
            timerPduUpdate = new System.Windows.Forms.Timer();
            timerPduUpdate.Interval = 1000; // 每 5 秒更新一次
            timerPduUpdate.Tick += TimerPduUpdate_Tick;
        }
        #endregion


        private MenuStrip menuStrip1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem dbgToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem toolStripMenuItem10;
        private ToolStripMenuItem toolStripMenuItem11;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ComboBox comboBox1ComPort;
        private ComboBox comboBox1BaudRate;
        private ComboBox comboBox2ComPort;
        private ComboBox comboBox2BaudRate;
        private ComboBox comboBox3ComPort;
        private ComboBox comboBox3BaudRate;
        private ComboBox comboBox4ComPort;
        private ComboBox comboBox4BaudRate;
        private ComboBox comboBox5ComPort;
        private ComboBox comboBox5BaudRate;
        private ComboBox comboBox6ComPort;
        private ComboBox comboBox6BaudRate;
        private Button button1Connect;
        private Button button2Connect;
        private Button button3Connect;
        private Button button4Connect;
        private Button button5Connect;
        private Button button6Connect;
        private Button button1RunScript;
        private Button button2RunScript;
        private Button button3RunScript;
        private Button button4RunScript;
        private Button button5RunScript;
        private Button button6RunScript;
        private Button button1EndScript;
        private Button button2EndScript;
        private Button button3EndScript;
        private Button button4EndScript;
        private Button button5EndScript;
        private Button button6EndScript;
        private RJToggleButton btnPduPort1;
        private RJToggleButton btnPduPort2;
        private RJToggleButton btnPduPort3;
        private RJToggleButton btnPduPort4;
        private RJToggleButton btnPduPort5;
        private RJToggleButton btnPduPort6;
        private RJToggleButton btnPduPort7;
        private RJToggleButton btnPduPort8;
        private RJToggleButton btnPduPort9;
        private RJToggleButton btnPduPort10;
        private RJToggleButton btnPduPort11;
        private RJToggleButton btnPduPort12;
        private CheckBox checkBox1LogEnable;
        private CheckBox checkBox2LogEnable;
        private CheckBox checkBox3LogEnable;
        private CheckBox checkBox4LogEnable;
        private CheckBox checkBox5LogEnable;
        private CheckBox checkBox6LogEnable;
        private CheckBox checkBox1TimestampEnable;
        private CheckBox checkBox2TimestampEnable;
        private CheckBox checkBox3TimestampEnable;
        private CheckBox checkBox4TimestampEnable;
        private CheckBox checkBox5TimestampEnable;
        private CheckBox checkBox6TimestampEnable;
        private Label lblPduIpAddress;
        private TextBox tbPduIpAddress;
        private Button btnPduConnect;
        private Label lblStatusSerialPort1;
        private Label lblStatusSerialPort2;
        private Label lblStatusSerialPort3;
        private Label lblStatusSerialPort4;
        private Label lblStatusSerialPort5;
        private Label lblStatusSerialPort6;
        private Label lblStatusTX1;
        private Label lblStatusRX1;
        private Label lblStatusTX2;
        private Label lblStatusRX2;
        private Label lblStatusTX3;
        private Label lblStatusRX3;
        private Label lblStatusTX4;
        private Label lblStatusRX4;
        private Label lblStatusTX5;
        private Label lblStatusRX5;
        private Label lblStatusTX6;
        private Label lblStatusRX6;
        
        private Label label1ScriptStatus;
        private Label label2ScriptStatus;
        private Label label3ScriptStatus;
        private Label label4ScriptStatus;
        private Label label5ScriptStatus;
        private Label label6ScriptStatus;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Timer timerLocalTime;
        private System.Windows.Forms.Timer timerPduUpdate;
        private DataGridView dgvPdu;
    }
}
