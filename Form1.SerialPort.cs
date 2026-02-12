using System.IO.Ports;

namespace MyTeraTerm
{
    public partial class Form1
    {
        #region Serial Port - Panel Initialization
        
        /// <summary>
        /// 取得實際可用的 COM Port（排除虛擬 COM101-120 和使用中的 Port）
        /// </summary>
        private string[] GetRealComPorts()
        {
            string[] allPorts = System.IO.Ports.SerialPort.GetPortNames();
            var availablePorts = new List<string>();
            
            foreach (var port in allPorts)
            {
                // 排除虛擬 COM Port (COM101-112)
                var match = System.Text.RegularExpressions.Regex.Match(port, @"COM(\d+)");
                if (match.Success)
                {
                    int portNum = int.Parse(match.Groups[1].Value);
                    if (portNum >= 101 && portNum <= 112)
                    {
                        continue; // 跳過虛擬 Port
                    }
                }
                
                // 驗證 COM Port 是否可用（嘗試開啟）
                try
                {
                    using (var testPort = new SerialPort(port))
                    {
                        testPort.Open();
                        testPort.Close();
                    }
                    // 只有成功開啟並關閉的 Port 才加入列表
                    availablePorts.Add(port);
                    AppLogger.LogDebug($"[COM Port] Available: {port}");
                }
                catch (Exception ex)
                {
                    // Port 被占用或不存在，不加入列表
                    AppLogger.LogDebug($"[COM Port] Excluded: {port}, Reason: {ex.GetType().Name}");
                }
            }
            
            return availablePorts.ToArray();
        }
        
        /// <summary>
        /// 初始化所有 Panels 並加入控制項
        /// </summary>
        private void InitializePanels()
        {
            // 設定所有 ComboBox 的 DropDown 事件（點擊時重新載入 COM Port）
            comboBox1ComPort.DropDown += RefreshComPortList;
            comboBox2ComPort.DropDown += RefreshComPortList;
            comboBox3ComPort.DropDown += RefreshComPortList;
            comboBox4ComPort.DropDown += RefreshComPortList;
            comboBox5ComPort.DropDown += RefreshComPortList;
            comboBox6ComPort.DropDown += RefreshComPortList;

            // 初始載入 COM Port 列表
            RefreshAllComPortLists();

            // 建立並設定 Panel 1
            CreateAndConfigurePanel(panel1, comboBox1ComPort, comboBox1BaudRate, 
                            button1Connect, button1RunScript, button1EndScript, label1ScriptStatus,
                            lblStatusSerialPort1, lblStatusTX1, lblStatusRX1,
                            checkBox1LogEnable, checkBox1TimestampEnable);

            // 建立並設定 Panel 2
            CreateAndConfigurePanel(panel2, comboBox2ComPort, comboBox2BaudRate, 
                            button2Connect, button2RunScript, button2EndScript, label2ScriptStatus,
                            lblStatusSerialPort2, lblStatusTX2, lblStatusRX2,
                            checkBox2LogEnable, checkBox2TimestampEnable);

            // 建立並設定 Panel 3
            CreateAndConfigurePanel(panel3, comboBox3ComPort, comboBox3BaudRate, 
                            button3Connect, button3RunScript, button3EndScript, label3ScriptStatus,
                            lblStatusSerialPort3, lblStatusTX3, lblStatusRX3,
                            checkBox3LogEnable, checkBox3TimestampEnable);

            // 建立並設定 Panel 4
            CreateAndConfigurePanel(panel4, comboBox4ComPort, comboBox4BaudRate, 
                            button4Connect, button4RunScript, button4EndScript, label4ScriptStatus,
                            lblStatusSerialPort4, lblStatusTX4, lblStatusRX4,
                            checkBox4LogEnable, checkBox4TimestampEnable);

            // 建立並設定 Panel 5
            CreateAndConfigurePanel(panel5, comboBox5ComPort, comboBox5BaudRate, 
                            button5Connect, button5RunScript, button5EndScript, label5ScriptStatus,
                            lblStatusSerialPort5, lblStatusTX5, lblStatusRX5,
                            checkBox5LogEnable, checkBox5TimestampEnable);

            // 建立並設定 Panel 6
            CreateAndConfigurePanel(panel6, comboBox6ComPort, comboBox6BaudRate, 
                            button6Connect, button6RunScript, button6EndScript, label6ScriptStatus,
                            lblStatusSerialPort6, lblStatusTX6, lblStatusRX6,
                            checkBox6LogEnable, checkBox6TimestampEnable);

            // 將所有 Panel 加入到 tabPage1
            tabPage1.SuspendLayout();
            tabPage1.Controls.Add(panel1);
            tabPage1.Controls.Add(panel2);
            tabPage1.Controls.Add(panel3);
            tabPage1.Controls.Add(panel4);
            tabPage1.Controls.Add(panel5);
            tabPage1.Controls.Add(panel6);
            tabPage1.ResumeLayout(false);
        }

        /// <summary>
        /// 設定單一 Panel 的屬性並加入控制項
        /// </summary>
        private void CreateAndConfigurePanel(Panel panel, ComboBox comPort, ComboBox baudRate, 
                                            Button connect, Button runScript, Button endScript,
                                            Label status, Label lblStatusTxRx, Label lblTx, Label lblRx,
                                            CheckBox logEnable, CheckBox timestampEnable)
        {
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.Gray;
            panel.Controls.Add(comPort);
            panel.Controls.Add(baudRate);
            panel.Controls.Add(connect);
            panel.Controls.Add(runScript);
            panel.Controls.Add(endScript);
            panel.Controls.Add(status);
            panel.Controls.Add(lblStatusTxRx);
            panel.Controls.Add(lblTx);
            panel.Controls.Add(lblRx);
            panel.Controls.Add(logEnable);
            panel.Controls.Add(timestampEnable);
            panel.Resize += EmbedExeForm_Resize;
        }
        
        /// <summary>
        /// 刷新所有 ComboBox 的 COM Port 列表
        /// </summary>
        private void RefreshAllComPortLists()
        {
            string[] realPorts = GetRealComPorts();
            
            RefreshSingleComPortList(comboBox1ComPort, realPorts);
            RefreshSingleComPortList(comboBox2ComPort, realPorts);
            RefreshSingleComPortList(comboBox3ComPort, realPorts);
            RefreshSingleComPortList(comboBox4ComPort, realPorts);
            RefreshSingleComPortList(comboBox5ComPort, realPorts);
            RefreshSingleComPortList(comboBox6ComPort, realPorts);
        }
        
        /// <summary>
        /// 刷新單一 ComboBox 的 COM Port 列表（保留目前選擇）
        /// </summary>
        private void RefreshSingleComPortList(ComboBox comboBox, string[] ports)
        {
            string? currentSelection = comboBox.SelectedItem?.ToString();
            
            comboBox.Items.Clear();
            comboBox.Items.AddRange(ports);
            
            // 如果之前有選擇且仍然存在，則恢復選擇
            if (!string.IsNullOrEmpty(currentSelection) && ports.Contains(currentSelection))
            {
                comboBox.SelectedItem = currentSelection;
            }
        }
        
        /// <summary>
        /// 當使用者點擊 ComboBox 時刷新 COM Port 列表
        /// </summary>
        private void RefreshComPortList(object? sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string[] realPorts = GetRealComPorts();
                RefreshSingleComPortList(comboBox, realPorts);
                
                AppLogger.LogDebug($"[COM Port] Refreshed port list: {string.Join(", ", realPorts)}");
            }
        }
        
        #endregion

        #region Serial Port - Layout Management
        
        private void UpdatePanelLayout(int terminalCount)
        {
            // Hide all panels
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;

            int availableWidth = tabPage1.ClientSize.Width;
            int availableHeight = tabPage1.ClientSize.Height;
            int gap = 5;

            switch (terminalCount)
            {
                case 1:
                    panel1.SetBounds(0, 0, availableWidth, availableHeight);
                    panel1.Visible = true;
                    break;

                case 2:
                    int halfWidth = (availableWidth - gap) / 2;
                    panel1.SetBounds(0, 0, halfWidth, availableHeight);
                    panel2.SetBounds(halfWidth + gap, 0, halfWidth, availableHeight);
                    panel1.Visible = true;
                    panel2.Visible = true;
                    break;

                case 4:
                    int halfWidth4 = (availableWidth - gap) / 2;
                    int halfHeight4 = (availableHeight - gap) / 2;
                    panel1.SetBounds(0, 0, halfWidth4, halfHeight4);
                    panel2.SetBounds(halfWidth4 + gap, 0, halfWidth4, halfHeight4);
                    panel3.SetBounds(0, halfHeight4 + gap, halfWidth4, halfHeight4);
                    panel4.SetBounds(halfWidth4 + gap, halfHeight4 + gap, halfWidth4, halfHeight4);
                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    break;

                case 6:
                    int colWidth = (availableWidth - gap) / 2;
                    int rowHeight = (availableHeight - gap * 2) / 3;
                    panel1.SetBounds(0, 0, colWidth, rowHeight);
                    panel2.SetBounds(colWidth + gap, 0, colWidth, rowHeight);
                    panel3.SetBounds(0, rowHeight + gap, colWidth, rowHeight);
                    panel4.SetBounds(colWidth + gap, rowHeight + gap, colWidth, rowHeight);
                    panel5.SetBounds(0, (rowHeight + gap) * 2, colWidth, rowHeight);
                    panel6.SetBounds(colWidth + gap, (rowHeight + gap) * 2, colWidth, rowHeight);
                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    panel5.Visible = true;
                    panel6.Visible = true;
                    break;
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // 根據選單選項決定顯示幾個 terminal
            if (toolStripMenuItem8.Checked)
                UpdatePanelLayout(1);
            else if (toolStripMenuItem9.Checked)
                UpdatePanelLayout(2);
            else if (toolStripMenuItem10.Checked)
                UpdatePanelLayout(4);
            else if (toolStripMenuItem11.Checked)
                UpdatePanelLayout(6);
        }
        #endregion
        
        #region TX/RX LED Control
        
        private System.Windows.Forms.Timer? ledTimer1TX;
        private System.Windows.Forms.Timer? ledTimer1RX;
        private System.Windows.Forms.Timer? ledTimer2TX;
        private System.Windows.Forms.Timer? ledTimer2RX;
        private System.Windows.Forms.Timer? ledTimer3TX;
        private System.Windows.Forms.Timer? ledTimer3RX;
        private System.Windows.Forms.Timer? ledTimer4TX;
        private System.Windows.Forms.Timer? ledTimer4RX;
        private System.Windows.Forms.Timer? ledTimer5TX;
        private System.Windows.Forms.Timer? ledTimer5RX;
        private System.Windows.Forms.Timer? ledTimer6TX;
        private System.Windows.Forms.Timer? ledTimer6RX;

        private void InitializeLedTimers()
        {
            // 為每個 COM Port 建立 TX/RX LED Timer
            ledTimer1TX = CreateLedTimer(lblStatusTX1);
            ledTimer1RX = CreateLedTimer(lblStatusRX1);
            ledTimer2TX = CreateLedTimer(lblStatusTX2);
            ledTimer2RX = CreateLedTimer(lblStatusRX2);
            ledTimer3TX = CreateLedTimer(lblStatusTX3);
            ledTimer3RX = CreateLedTimer(lblStatusRX3);
            ledTimer4TX = CreateLedTimer(lblStatusTX4);
            ledTimer4RX = CreateLedTimer(lblStatusRX4);
            ledTimer5TX = CreateLedTimer(lblStatusTX5);
            ledTimer5RX = CreateLedTimer(lblStatusRX5);
            ledTimer6TX = CreateLedTimer(lblStatusTX6);
            ledTimer6RX = CreateLedTimer(lblStatusRX6);
        }

        private System.Windows.Forms.Timer CreateLedTimer(Label ledLabel)
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // 100ms 後熄滅
            timer.Tick += (s, e) =>
            {
                ledLabel.ForeColor = Color.DarkGray; // 恢復為暗灰色
                timer.Stop();
            };
            return timer;
        }

        private void FlashTxLed(Label txLabel, System.Windows.Forms.Timer? timer)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => FlashTxLed(txLabel, timer)));
                return;
            }

            txLabel.ForeColor = Color.LightGreen;
            timer?.Stop();
            timer?.Start();
        }

        private void FlashRxLed(Label rxLabel, System.Windows.Forms.Timer? timer)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => FlashRxLed(rxLabel, timer)));
                return;
            }

            rxLabel.ForeColor = Color.LightGreen;
            timer?.Stop();
            timer?.Start();
        }

        #endregion

    }
}