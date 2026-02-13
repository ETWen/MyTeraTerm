using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// References to other namespaces in lib folder
using EmbedForm;    //EmbeddedWindowController.cs
using TTLInterpreterLib; //TTLInterpreter.cs
using COMPortBrdigeLib; //ComPortBridge.cs
using PDUControlLib; //PduController.cs
using RJControlslib; //RJToggleButton.cs




namespace MyTeraTerm
{
    public partial class Form1 : Form
    {
        #region WinAPI Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;

        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_MINIMIZEBOX = 0x00020000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_SYSMENU = 0x00080000;

        private const int WS_EX_DLGMODALFRAME = 0x00000001;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;
        #endregion

        # region EmbeddedWnd Process/Controllers
        private Process externalProcess;
        private Process externalProcess2;
        private Process externalProcess3;
        private Process externalProcess4;
        private Process externalProcess5;
        private Process externalProcess6;
        private EmbeddedWindowController windowController1;
        private EmbeddedWindowController windowController2;
        private EmbeddedWindowController windowController3;
        private EmbeddedWindowController windowController4;
        private EmbeddedWindowController windowController5;
        private EmbeddedWindowController windowController6;
        #endregion

        #region Form Initialization
        public Form1()
        {
            // 初始化 Logger（最優先）
            AppLogger.Initialize();
            AppLogger.LogInfo("Application starting...");
            
            // 初始化 RX Log 鎖和行首標記
            for (int i = 0; i < rxLogLocks.Length; i++)
            {
                rxLogLocks[i] = new object();
                isLineStart[i] = true;
            }
            
            InitializeComponent();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            toolStripStatusLabel1.Text = $"MyTeraTerm v{version.Major}.{version.Minor}.{version.Build}";

            // 檢查並建立虛擬 COM 端口
            CheckAndCreateVirtualComPorts();

            InitializeSerialPortComponents();
            InitializePanels();
            InitializeLedTimers();

            InitializePduControlLogic();
            UpdatePanelLayout(1);   //Default to single terminal view
            
            // 初始化 PDU Power Cycle 狀態顯示
            UpdatePduPowerCycleStatus();
            
            // Enable console for debugging
            // AllocConsole();
        }

        private void CheckAndCreateVirtualComPorts()
        {
            try
            {
                AppLogger.LogInfo("Checking virtual COM ports (COM101-112)...");
                
                // 檢查 COM101-112 是否存在
                bool allPortsExist = true;
                List<string> missingPorts = new List<string>();
                
                for (int i = 101; i <= 112; i++)
                {
                    string portName = $"COM{i}";
                    if (!IsComPortAvailable(portName))
                    {
                        allPortsExist = false;
                        missingPorts.Add(portName);
                    }
                }
                
                if (allPortsExist)
                {
                    AppLogger.LogInfo("All virtual COM ports (COM101-112) are available.");
                    return;
                }
                
                // 顯示缺少的端口
                AppLogger.LogWarning($"Missing COM ports: {string.Join(", ", missingPorts)}");
                
                // 詢問用戶是否要建立虛擬端口
                var result = MessageBox.Show(
                    $"Virtual COM ports are required for this application.\n\n" +
                    $"Missing ports: {string.Join(", ", missingPorts.Take(5))}{(missingPorts.Count > 5 ? "..." : "")}\n\n" +
                    $"Do you want to create them now?\n" +
                    $"(This requires administrator privileges)",
                    "Virtual COM Ports Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    CreateVirtualComPorts();
                }
                else
                {
                    AppLogger.LogWarning("User declined to create virtual COM ports.");
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error checking virtual COM ports: {ex.Message}");
                MessageBox.Show($"Error checking virtual COM ports: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsComPortAvailable(string portName)
        {
            try
            {
                // 取得系統所有可用的 COM 端口
                string[] availablePorts = SerialPort.GetPortNames();
                return availablePorts.Contains(portName);
            }
            catch
            {
                return false;
            }
        }

        private void CreateVirtualComPorts()
        {
            try
            {
                // 找到 setupc.exe 的路徑
                string setupcPath = Path.Combine(Application.StartupPath, "Application", "com0com", "x64", "setupc.exe");
                
                if (!File.Exists(setupcPath))
                {
                    AppLogger.LogError($"setupc.exe not found at: {setupcPath}");
                    MessageBox.Show($"setupc.exe not found at:\n{setupcPath}\n\nPlease ensure the com0com tools are installed.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                AppLogger.LogInfo($"Creating virtual COM ports using: {setupcPath}");
                
                // 建立 6 對虛擬端口 (COM101-102, COM103-104, COM105-106, COM107-108, COM109-110, COM111-112)
                StringBuilder commands = new StringBuilder();
                for (int i = 101; i <= 111; i += 2)
                {
                    // 使用 setupc.exe install 命令
                    // 格式: install PortName=COM101 PortName=COM102
                    commands.AppendLine($"install PortName=COM{i} PortName=COM{i + 1}");
                }
                
                // 將命令寫入批次檔（放在 Application 目錄）
                string batchFilePath = Path.Combine(Application.StartupPath, "Application", "setup_com_ports.bat");
                AppLogger.LogInfo($"Batch file will be saved at: {batchFilePath}");
                File.WriteAllText(batchFilePath, "@echo off\ncd /d \"%~dp0com0com\\x64\"\n");
                
                foreach (string cmd in commands.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    File.AppendAllText(batchFilePath, $"setupc.exe {cmd}\n");
                }
                
                File.AppendAllText(batchFilePath, "echo.\necho Virtual COM ports setup completed.\npause\n");
                
                // 使用管理員權限執行批次檔
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = batchFilePath,
                    Verb = "runas", // 要求管理員權限
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                
                try
                {
                    Process? process = Process.Start(psi);
                    
                    if (process != null)
                    {
                        AppLogger.LogInfo("Waiting for setupc.exe to complete...");
                        process.WaitForExit();
                        
                        // 保留批次檔供使用者參考（不刪除）
                        AppLogger.LogInfo($"Batch file saved at: {batchFilePath}");
                        
                        if (process.ExitCode == 0)
                        {
                            AppLogger.LogInfo("Virtual COM ports created successfully.");
                            MessageBox.Show("Virtual COM ports have been created successfully.\n\nPlease restart the application.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            AppLogger.LogWarning($"setupc.exe exited with code: {process.ExitCode}");
                        }
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // 使用者取消了 UAC 提示
                    AppLogger.LogWarning("User cancelled administrator privileges request.");
                    MessageBox.Show("Administrator privileges are required to create virtual COM ports.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error creating virtual COM ports: {ex.Message}");
                MessageBox.Show($"Error creating virtual COM ports:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Menu Event Handlers
        private void StripMenu_MultiTerminal_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;

            // Cancel all selections
            toolStripMenuItem8.Checked = false;
            toolStripMenuItem9.Checked = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem11.Checked = false;

            // Set the currently clicked item as checked
            clickedItem.Checked = true;

            // Update layout based on selection
            int terminalCount = int.Parse(clickedItem.Text);
            UpdatePanelLayout(terminalCount);
        }
        private void stripMenu_Release_Resources_Click(object sender, EventArgs e)
        {
            KillAllTeraTermProcesses();
            
            // 清理當前的 controller 引用
            windowController1 = null;
            windowController2 = null;
            windowController3 = null;
            windowController4 = null;
            windowController5 = null;
            windowController6 = null;
            
            // 清理 process 引用
            externalProcess = null;
            externalProcess2 = null;
            externalProcess3 = null;
            externalProcess4 = null;
            externalProcess5 = null;
            externalProcess6 = null;
        }
        private void dbgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PasswordDialog passwordDialog = new PasswordDialog())
            {
                if (passwordDialog.ShowDialog(this) == DialogResult.OK && passwordDialog.IsAuthenticated)
                {
                    try
                    {
                        // 開啟 Debug Console
                        AppLogger.AllocateDebugConsole();
                        
                        MessageBox.Show($"Debug Console has been activated!\n\n" +
                                      $"Log file location:\n{AppLogger.GetDebugLogPath()}\n\n" +
                                      "You can now see real-time debug messages in the console window.\n" +
                                      "All messages are also saved to the log file.",
                                      "Debug Console Enabled",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError("Failed to allocate debug console", ex);
                        MessageBox.Show($"Failed to allocate debug console:\n{ex.Message}",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string aboutMessage = $"MyTeraTerm\n" +
                         $"Version {version?.Major}.{version?.Minor}.{version?.Build} (Build 1001)\n" +
                         $"(C) 2024-2026 MyTeraTerm Project\n\n" +
                         $"Includes:\n" +
                         $"  Serial Communication Library\n" +
                         $"  Copyright (C) 2024 Developer Name\n\n" +
                         $"Built using Microsoft Visual C# .NET\n" +
                         $"Build time: {DateTime.Now:MMMM dd, yyyy HH:mm:ss}\n" +
                         $"Author: eric441151893@gmail.com";

            MessageBox.Show(aboutMessage, "About MyTeraTerm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void KillAllTeraTermProcesses()
        {
            try
            {
                // Find all processes with the name "ttermpro" (Tera Term)
                Process[] teraTermProcesses = Process.GetProcessesByName("ttermpro");
                
                int killedCount = 0;
                foreach (Process process in teraTermProcesses)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(2000);
                        killedCount++;
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogWarning($"Failed to kill Tera Term process {process.Id}: {ex.Message}");
                    }
                }
                
                AppLogger.LogInfo($"Killed {killedCount} Tera Term processes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error while killing Tera Term processes:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Terminal Connection Methods
        private ComPortBridge bridge1;
        private ComPortBridge bridge2;
        private ComPortBridge bridge3;
        private ComPortBridge bridge4;
        private ComPortBridge bridge5;
        private ComPortBridge bridge6;
        
        // RX Logging
        private readonly DateTime formStartTime = DateTime.Now;
        private DateTime[] terminalConnectTime = new DateTime[6];
        private StreamWriter?[] rxLogWriters = new StreamWriter?[6];
        private readonly object[] rxLogLocks = new object[6];
        private bool[] isLineStart = new bool[6];
        
        // PDU Power On Statistics
        private int[] pduPowerOnCount = new int[6];

        private void button1Connect_Click(object sender, EventArgs e)
        {
            if (comboBox1ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button1Connect.Text == "Connect")
            {
                string realPort = comboBox1ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox1BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM101";
                string virtualPort2 = "COM102";
                
                try
                {
                    GenerateBridge(ref bridge1, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge1 != null)
                    {
                        bridge1.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(1, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX1, ledTimer1TX);
                        };
                        
                        bridge1.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(1, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX1, ledTimer1RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(0, data, checkBox1TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(0, checkBox1LogEnable, checkBox1TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess, ref windowController1, panel1, ref bridge1);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(0, ref bridge1, ref ttlInterpreter1);
                
                button1Connect.Text = "Disconnect";
                button1Connect.BackColor = Color.LightGreen;
                comboBox1ComPort.Enabled = false;
                comboBox1BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 1] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge1 != null)
                    {
                        bridge1.Stop();
                        bridge1.Dispose();
                        bridge1 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(0, checkBox1LogEnable, checkBox1TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter1?.Dispose();
                ttlInterpreter1 = null;
                
                DisconnectTerminal(ref externalProcess, ref windowController1, ref bridge1);
                button1Connect.Text = "Connect";
                button1Connect.BackColor = Color.LightCoral;
                comboBox1ComPort.Enabled = true;
                comboBox1BaudRate.Enabled = true;
            }
        }
        private void button2Connect_Click(object sender, EventArgs e)
        {
            if (comboBox2ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox2BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button2Connect.Text == "Connect")
            {
                string realPort = comboBox2ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox2BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM103";
                string virtualPort2 = "COM104";
                
                try
                {
                    GenerateBridge(ref bridge2, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge2 != null)
                    {
                        bridge2.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(2, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX2, ledTimer2TX);
                        };
                        
                        bridge2.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(2, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX2, ledTimer2RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(1, data, checkBox2TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(1, checkBox2LogEnable, checkBox2TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess2, ref windowController2, panel2, ref bridge2);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(1, ref bridge2, ref ttlInterpreter2);
                
                button2Connect.Text = "Disconnect";
                button2Connect.BackColor = Color.LightGreen;
                comboBox2ComPort.Enabled = false;
                comboBox2BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 2] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge2 != null)
                    {
                        bridge2.Stop();
                        bridge2.Dispose();
                        bridge2 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(1, checkBox2LogEnable, checkBox2TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter2?.Dispose();
                ttlInterpreter2 = null;
                
                DisconnectTerminal(ref externalProcess2, ref windowController2, ref bridge2);
                button2Connect.Text = "Connect";
                button2Connect.BackColor = Color.LightCoral;
                comboBox2ComPort.Enabled = true;
                comboBox2BaudRate.Enabled = true;
            }
        }
        private void button3Connect_Click(object sender, EventArgs e)
        {
            if (comboBox3ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox3BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button3Connect.Text == "Connect")
            {
                string realPort = comboBox3ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox3BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM105";
                string virtualPort2 = "COM106";
                
                try
                {
                    GenerateBridge(ref bridge3, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge3 != null)
                    {
                        bridge3.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(3, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX3, ledTimer3TX);
                        };
                        
                        bridge3.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(3, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX3, ledTimer3RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(2, data, checkBox3TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(2, checkBox3LogEnable, checkBox3TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess3, ref windowController3, panel3, ref bridge3);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(2, ref bridge3, ref ttlInterpreter3);
                
                button3Connect.Text = "Disconnect";
                button3Connect.BackColor = Color.LightGreen;
                comboBox3ComPort.Enabled = false;
                comboBox3BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 3] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge3 != null)
                    {
                        bridge3.Stop();
                        bridge3.Dispose();
                        bridge3 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(2, checkBox3LogEnable, checkBox3TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter3?.Dispose();
                ttlInterpreter3 = null;
                
                DisconnectTerminal(ref externalProcess3, ref windowController3, ref bridge3);
                button3Connect.Text = "Connect";
                button3Connect.BackColor = Color.LightCoral;
                comboBox3ComPort.Enabled = true;
                comboBox3BaudRate.Enabled = true;
            }
        }
        private void button4Connect_Click(object sender, EventArgs e)
        {
            if (comboBox4ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox4BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button4Connect.Text == "Connect")
            {
                string realPort = comboBox4ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox4BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM107";
                string virtualPort2 = "COM108";
                
                try
                {
                    GenerateBridge(ref bridge4, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge4 != null)
                    {
                        bridge4.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(4, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX4, ledTimer4TX);
                        };
                        
                        bridge4.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(4, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX4, ledTimer4RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(3, data, checkBox4TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(3, checkBox4LogEnable, checkBox4TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess4, ref windowController4, panel4, ref bridge4);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(3, ref bridge4, ref ttlInterpreter4);
                
                button4Connect.Text = "Disconnect";
                button4Connect.BackColor = Color.LightGreen;
                comboBox4ComPort.Enabled = false;
                comboBox4BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 4] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge4 != null)
                    {
                        bridge4.Stop();
                        bridge4.Dispose();
                        bridge4 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(3, checkBox4LogEnable, checkBox4TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter4?.Dispose();
                ttlInterpreter4 = null;
                
                DisconnectTerminal(ref externalProcess4, ref windowController4, ref bridge4);
                button4Connect.Text = "Connect";
                button4Connect.BackColor = Color.LightCoral;
                comboBox4ComPort.Enabled = true;
                comboBox4BaudRate.Enabled = true;
            }
        }
        private void button5Connect_Click(object sender, EventArgs e)
        {
            if (comboBox5ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox5BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button5Connect.Text == "Connect")
            {
                string realPort = comboBox5ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox5BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM109";
                string virtualPort2 = "COM110";
                
                try
                {
                    GenerateBridge(ref bridge5, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge5 != null)
                    {
                        bridge5.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(5, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX5, ledTimer5TX);
                        };
                        
                        bridge5.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(5, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX5, ledTimer5RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(4, data, checkBox5TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(4, checkBox5LogEnable, checkBox5TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess5, ref windowController5, panel5, ref bridge5);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(4, ref bridge5, ref ttlInterpreter5);
                
                button5Connect.Text = "Disconnect";
                button5Connect.BackColor = Color.LightGreen;
                comboBox5ComPort.Enabled = false;
                comboBox5BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 5] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge5 != null)
                    {
                        bridge5.Stop();
                        bridge5.Dispose();
                        bridge5 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(4, checkBox5LogEnable, checkBox5TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter5?.Dispose();
                ttlInterpreter5 = null;
                
                DisconnectTerminal(ref externalProcess5, ref windowController5, ref bridge5);
                button5Connect.Text = "Connect";
                button5Connect.BackColor = Color.LightCoral;
                comboBox5ComPort.Enabled = true;
                comboBox5BaudRate.Enabled = true;
            }
        }
        private void button6Connect_Click(object sender, EventArgs e)
        {
            if (comboBox6ComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM Port.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox6BaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a Baud Rate.", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (button6Connect.Text == "Connect")
            {
                string realPort = comboBox6ComPort.SelectedItem?.ToString() ?? "";
                string baudRate = comboBox6BaudRate.SelectedItem?.ToString() ?? "";
                string virtualPort = "COM111";
                string virtualPort2 = "COM112";
                
                try
                {
                    GenerateBridge(ref bridge6, realPort, virtualPort, baudRate);
                    
                    // 訂閱事件
                    if (bridge6 != null)
                    {
                        bridge6.DataTransmitted += (data) => 
                        {
                            //AppLogger.LogTX(6, data?.Length ?? 0);
                            FlashTxLed(lblStatusTX6, ledTimer6TX);
                        };
                        
                        bridge6.DataReceived += (data) => 
                        {
                            //AppLogger.LogRX(6, data?.Length ?? 0);
                            FlashRxLed(lblStatusRX6, ledTimer6RX);
                            // RX 日誌記錄
                            if (data != null)
                                WriteRxLog(5, data, checkBox6TimestampEnable.Checked);
                        };
                    }
                    
                    // 創建 RX 日誌檔案
                    CreateRxLogFile(5, checkBox6LogEnable, checkBox6TimestampEnable);
                    
                    ConnectTerminal(virtualPort2, baudRate, ref externalProcess6, ref windowController6, panel6, ref bridge6);
                    
                    // 為此終端創建 TTLInterpreter 以支持 waitall
                    CreateInterpreterForTerminal(5, ref bridge6, ref ttlInterpreter6);
                
                button6Connect.Text = "Disconnect";
                button6Connect.BackColor = Color.LightGreen;
                comboBox6ComPort.Enabled = false;
                comboBox6BaudRate.Enabled = false;
                }
                catch (InvalidOperationException ex)
                {
                    AppLogger.LogError($"[Terminal 6] Connection failed: {realPort}");
                    
                    MessageBox.Show(
                        $"Connection Failed\n\n" +
                        $"COM Port: {realPort}\n" +
                        $"Baud Rate: {baudRate}\n\n" +
                        $"Possible Causes:\n" +
                        $"1. COM Port is in use by another program\n" +
                        $"2. com0com service is not running\n" +
                        $"3. Insufficient permissions for COM Port\n\n" +
                        $"Suggested Solutions:\n" +
                        $"1. Close other programs using this COM Port\n" +
                        $"2. Restart com0com service\n" +
                        $"3. Restart your computer\n\n" +
                        $"For detailed error information, check Debug Log.",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // 確保清理資源
                    if (bridge6 != null)
                    {
                        bridge6.Stop();
                        bridge6.Dispose();
                        bridge6 = null;
                    }
                }
            }
            else
            {
                CloseRxLogFile(5, checkBox6LogEnable, checkBox6TimestampEnable);
                
                // 釋放 TTLInterpreter
                ttlInterpreter6?.Dispose();
                ttlInterpreter6 = null;
                
                DisconnectTerminal(ref externalProcess6, ref windowController6, ref bridge6);
                button6Connect.Text = "Connect";
                button6Connect.BackColor = Color.LightCoral;
                comboBox6ComPort.Enabled = true;
                comboBox6BaudRate.Enabled = true;
            }
        }

        private void GenerateBridge(ref ComPortBridge bridge, string realPort, string virtualPort, string baudRate)
        {
            if (bridge == null || !bridge.IsRunning)
            {
                try
                {
                    AppLogger.LogInfo($"[Bridge] Creating bridge: {realPort} -> {virtualPort} @ {baudRate} bps");
                    bridge = new ComPortBridge(realPort, virtualPort, baudRate);
                    bridge.Start();
                    AppLogger.LogInfo($"[Bridge] Bridge started: {realPort} <-> {virtualPort}");
                }
                catch (InvalidOperationException ex)
                {
                    // COM Port 開啟失敗，清理資源
                    AppLogger.LogError($"[Bridge] Failed to create bridge: {realPort} <-> {virtualPort}");
                    
                    if (bridge != null)
                    {
                        bridge.Stop();
                        bridge.Dispose();
                        bridge = null;
                    }
                    
                    throw; // 重新拋出讓上層處理
                }
                catch (Exception ex)
                {
                    // 其他未預期的錯誤
                    AppLogger.LogError($"[Bridge] Unexpected error creating bridge: {realPort} <-> {virtualPort}", ex);
                    
                    if (bridge != null)
                    {
                        bridge.Stop();
                        bridge.Dispose();
                        bridge = null;
                    }
                    
                    throw new InvalidOperationException($"無法建立 COM Port 橋接: {ex.Message}", ex);
                }
            }
            else
            {
                AppLogger.LogInfo($"[Bridge] Stopping bridge: {realPort} <-> {virtualPort}");
                bridge.Stop();
                bridge.Dispose();
                bridge = null;
            }
        }
        private void ConnectTerminal(string comPort, string baudRate, 
                                    ref Process process, ref EmbeddedWindowController controller, 
                                    Panel targetPanel, ref ComPortBridge bridge)
        {
            // Check if already connected
            if (process != null && !process.HasExited)
            {
                MessageBox.Show("Terminal is already connected. Please press Alt+Q close first.", 
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Validate COM port selection
            if (string.IsNullOrWhiteSpace(comPort))
            {
                MessageBox.Show("Please select a COM Port.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate Baud Rate selection
            if (string.IsNullOrWhiteSpace(baudRate))
            {
                MessageBox.Show("Please select a Baud Rate.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extract COM port number
            string comPortNumber = System.Text.RegularExpressions.Regex.Match(comPort, @"\d+").Value;
            if (string.IsNullOrEmpty(comPortNumber))
            {
                MessageBox.Show("Invalid COM Port format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string appPath = Application.StartupPath;
                string teraTermPath = Path.Combine(appPath, "Application", "TeraTerm", "ttermpro.exe");
                if (!File.Exists(teraTermPath))
                {
                    MessageBox.Show($"Tera Term executable not found!\n\nExpected path:\n{teraTermPath}\n\n" +
                                "Please ensure ttermpro.exe is in the 'Application/TeraTerm' folder.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLogger.LogError($"TeraTerm not found at: {teraTermPath}");
                    return;
                }
                
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = teraTermPath;
                psi.Arguments = $"/C={comPortNumber} /SPEED={baudRate}";
                psi.WindowStyle = ProcessWindowStyle.Minimized;
                psi.CreateNoWindow = false;
                
                process = Process.Start(psi);
                
                // Enable raising events to capture process exit
                process.EnableRaisingEvents = true;
                
                // Capture bridge reference in closure
                ComPortBridge capturedBridge = bridge;
                EmbeddedWindowController capturedController = controller;
                
                // Add event handler for process exit
                process.Exited += (s, e) =>
                {
                    // Use Invoke to ensure we're on the UI thread
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            HandleProcessExited(capturedBridge, ref capturedController);
                        }));
                    }
                    else
                    {
                        HandleProcessExited(capturedBridge, ref capturedController);
                    }
                };
                
                EmbedExternalWindow(process, ref controller, targetPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error launching Tera Term: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisconnectTerminal(ref Process process, ref EmbeddedWindowController controller, ref ComPortBridge bridge)
        {
            try
            {
                // Stop and dispose Bridge (stop data transmission first)
                if (bridge != null && bridge.IsRunning)
                {
                    bridge.Stop();
                    bridge.Dispose();
                    bridge = null;
                }
                
                // Release embedded window controller
                if (controller != null)
                {
                    controller.ReleaseHandle();
                    controller = null;
                }
                
                // Close embedded Tera Term process (asynchronous cleanup, do not block UI)
                if (process != null && !process.HasExited)
                {
                    Process processToKill = process;
                    process = null; // Immediately clear reference to avoid duplicate operations
                    
                    try
                    {
                        processToKill.Kill();
                        // Asynchronously wait for process exit, do not block UI thread
                        Task.Run(() =>
                        {
                            try
                            {
                                if (processToKill.WaitForExit(5000))
                                {
                                    processToKill.Dispose();
                                }
                                else
                                {
                                    AppLogger.LogWarning("Process did not exit within timeout");
                                    processToKill.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                AppLogger.LogError("Error disposing process", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError("Error killing process", ex);
                    }
                }
                
                AppLogger.LogInfo("Terminal disconnected successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnect:\n{ex.Message}", 
                                "Disconnect Error", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
                AppLogger.LogError($"Error during disconnect: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Create RX log file
        /// </summary>
        private void CreateRxLogFile(int terminalIndex, CheckBox logEnableCheckBox, CheckBox timestampEnableCheckBox)
        {
            try
            {
                if (!logEnableCheckBox.Checked)
                    return;
                
                // Lock CheckBoxes
                logEnableCheckBox.Enabled = false;
                timestampEnableCheckBox.Enabled = false;
                
                // Log connect time
                terminalConnectTime[terminalIndex] = DateTime.Now;
                
                // Create log directory: Logs/[Form start time]
                string formStartTimeStr = formStartTime.ToString("yyyyMMdd_HHmmss");
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", formStartTimeStr);
                Directory.CreateDirectory(logDir);
                
                // Log file name: [Connect time]_Terminal[1-6].log
                string connectTimeStr = terminalConnectTime[terminalIndex].ToString("yyyyMMdd_HHmmss");
                string logFileName = $"{connectTimeStr}_Terminal{terminalIndex + 1}.log";
                string logFilePath = Path.Combine(logDir, logFileName);
                
                // Create StreamWriter
                rxLogWriters[terminalIndex] = new StreamWriter(logFilePath, append: true)
                {
                    AutoFlush = true
                };
                
                // Set to line start state
                isLineStart[terminalIndex] = true;
                
                // Write header information
                rxLogWriters[terminalIndex].WriteLine($"=== Terminal {terminalIndex + 1} RX Log ===");
                rxLogWriters[terminalIndex].WriteLine($"Connect Time: {terminalConnectTime[terminalIndex]:yyyy-MM-dd HH:mm:ss}");
                rxLogWriters[terminalIndex].WriteLine($"Timestamp Enabled: {timestampEnableCheckBox.Checked}");
                rxLogWriters[terminalIndex].WriteLine($"========================================");
                rxLogWriters[terminalIndex].WriteLine();
                
                AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] RX Log file created: {logFilePath}");
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[Terminal {terminalIndex + 1}] Failed to create RX log file", ex);
                MessageBox.Show($"Failed to create log file for Terminal {terminalIndex + 1}:\\n{ex.Message}", 
                    "Log Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        /// <summary>
        /// Close RX log file
        /// </summary>
        private void CloseRxLogFile(int terminalIndex, CheckBox logEnableCheckBox, CheckBox timestampEnableCheckBox)
        {
            try
            {
                if (rxLogWriters[terminalIndex] != null)
                {
                    lock (rxLogLocks[terminalIndex])
                    {
                        rxLogWriters[terminalIndex].WriteLine();
                        rxLogWriters[terminalIndex].WriteLine($"=== Disconnected at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                        rxLogWriters[terminalIndex].Close();
                        rxLogWriters[terminalIndex].Dispose();
                        rxLogWriters[terminalIndex] = null;
                        
                        AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] RX Log file closed");
                    }
                }
                
                // Unlock CheckBoxes
                logEnableCheckBox.Enabled = true;
                timestampEnableCheckBox.Enabled = true;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[Terminal {terminalIndex + 1}] Failed to close RX log file", ex);
            }
        }
        
        /// <summary>
        /// Write RX data to log file
        /// </summary>
        private void WriteRxLog(int terminalIndex, string data, bool timestampEnabled)
        {
            if (rxLogWriters[terminalIndex] == null || string.IsNullOrEmpty(data))
                return;
            
            try
            {
                lock (rxLogLocks[terminalIndex])
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        char c = data[i];
                        
                        // Add timestamp at the start of the line
                        if (isLineStart[terminalIndex] && timestampEnabled)
                        {
                            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            rxLogWriters[terminalIndex].Write($"[{timestamp}] ");
                            isLineStart[terminalIndex] = false;
                        }
                        
                        // Write character
                        rxLogWriters[terminalIndex].Write(c);
                        
                        // Check if character is a newline
                        if (c == '\n')
                        {
                            isLineStart[terminalIndex] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[Terminal {terminalIndex + 1}] Failed to write RX log", ex);
            }
        }

        private void HandleProcessExited(ComPortBridge bridge, ref EmbeddedWindowController controller)
        {
            // Stop and dispose bridge
            if (bridge != null && bridge.IsRunning)
            {
                try
                {
                    bridge.Stop();
                    bridge.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping bridge: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
            // Release window controller
            if (controller != null)
            {
                try
                {
                    controller.ReleaseHandle();
                    controller = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error releasing controller: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Run Script Button Handler
        private TTLInterpreter ttlInterpreter1;
        private TTLInterpreter ttlInterpreter2;
        private TTLInterpreter ttlInterpreter3;
        private TTLInterpreter ttlInterpreter4;
        private TTLInterpreter ttlInterpreter5;
        private TTLInterpreter ttlInterpreter6;
        private CancellationTokenSource scriptCts1;
        private CancellationTokenSource scriptCts2;
        private CancellationTokenSource scriptCts3;
        private CancellationTokenSource scriptCts4;
        private CancellationTokenSource scriptCts5;
        private CancellationTokenSource scriptCts6;
        private void buttonRunScript1_Click(object sender, EventArgs e)
        {
            RunTTLScript(0, ref bridge1, ref ttlInterpreter1, ref scriptCts1, label1ScriptStatus, button1RunScript, button1EndScript);
        }
        private void buttonRunScript2_Click(object sender, EventArgs e)
        {
            RunTTLScript(1, ref bridge2, ref ttlInterpreter2, ref scriptCts2, label2ScriptStatus, button2RunScript, button2EndScript);
        }
        private void buttonRunScript3_Click(object sender, EventArgs e)
        {
            RunTTLScript(2, ref bridge3, ref ttlInterpreter3, ref scriptCts3, label3ScriptStatus, button3RunScript, button3EndScript);
        }
        private void buttonRunScript4_Click(object sender, EventArgs e)
        {
            RunTTLScript(3, ref bridge4, ref ttlInterpreter4, ref scriptCts4, label4ScriptStatus, button4RunScript, button4EndScript);
        }
        private void buttonRunScript5_Click(object sender, EventArgs e)
        {
            RunTTLScript(4, ref bridge5, ref ttlInterpreter5, ref scriptCts5, label5ScriptStatus, button5RunScript, button5EndScript);
        }
        private void buttonRunScript6_Click(object sender, EventArgs e)
        {
            RunTTLScript(5, ref bridge6, ref ttlInterpreter6, ref scriptCts6, label6ScriptStatus, button6RunScript, button6EndScript);
        }

        private void buttonEndScript1_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter1, ref scriptCts1, label1ScriptStatus, button1RunScript, button1EndScript);
        }
        private void buttonEndScript2_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter2, ref scriptCts2, label2ScriptStatus, button2RunScript, button2EndScript);
        }
        private void buttonEndScript3_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter3, ref scriptCts3, label3ScriptStatus, button3RunScript, button3EndScript);
        }
        private void buttonEndScript4_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter4, ref scriptCts4, label4ScriptStatus, button4RunScript, button4EndScript);
        }
        private void buttonEndScript5_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter5, ref scriptCts5, label5ScriptStatus, button5RunScript, button5EndScript);
        }
        private void buttonEndScript6_Click(object sender, EventArgs e)
        {
            EndTTLScript(ref ttlInterpreter6, ref scriptCts6, label6ScriptStatus, button6RunScript, button6EndScript);
        }

        private void RunTTLScript(int terminalIndex, ref ComPortBridge bridge, ref TTLInterpreter interpreter, ref CancellationTokenSource cts, Label statusLabel, Button runButton, Button endButton)
        {
            if (bridge == null || !bridge.IsRunning)
            {
                MessageBox.Show("Please connect terminal first.", 
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Reset counter for this terminal
            pduPowerOnCount[terminalIndex] = 0;
            UpdatePduPowerCycleStatus();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select TTL Script File";
                openFileDialog.Filter = "TTL Script Files (*.ttl)|*.ttl|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                string scriptsPath = Path.Combine(Application.StartupPath, "Scripts");
                if (Directory.Exists(scriptsPath))
                {
                    openFileDialog.InitialDirectory = scriptsPath;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(scriptsPath);
                        openFileDialog.InitialDirectory = scriptsPath;
                        AppLogger.LogInfo($"Created Scripts directory: {scriptsPath}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogWarning($"Failed to create Scripts directory: {ex.Message}");
                        openFileDialog.InitialDirectory = Application.StartupPath;
                    }
                }

                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string scriptFilePath = openFileDialog.FileName;
                string scriptFileName = Path.GetFileName(scriptFilePath);
                
                string script;
                try
                {
                    script = File.ReadAllText(scriptFilePath);
                    AppLogger.LogInfo($"[TTL] Loaded script from: {scriptFilePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading script file:\n{ex.Message}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppLogger.LogError($"Error reading script file: {ex.Message}");
                    return;
                }

                // 鎖定 Run Script 按鈕，顯示 End Script 按鈕
                runButton.Enabled = false;
                endButton.Visible = true;

                // 創建新的 CancellationTokenSource
                cts?.Cancel();
                cts?.Dispose();
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                // Create or reuse TTL Interpreter
                if (interpreter == null)
                {
                    interpreter = new TTLInterpreter(bridge);
                    AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] Interpreter created for script execution");
                    
                    // 訂閱 WaitAll 和 SendAll 事件（只在创建时订阅一次）
                    interpreter.WaitAllRequested += (text) =>
                    {
                        AppLogger.LogInfo($"[WaitAll Event] Called from Terminal {terminalIndex + 1}, checking for: '{text}'");
                        bool result = CheckAllTerminalsReceived(text);
                        AppLogger.LogInfo($"[WaitAll Event] Returning: {result}");
                        return result;
                    };
                    
                    interpreter.SendAllRequested += (text, newline) =>
                    {
                        AppLogger.LogInfo($"[SendAll Event] Called from Terminal {terminalIndex + 1}");
                        SendToAllTerminals(text, newline);
                    };
                }
                else
                {
                    AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] Reusing existing interpreter for script execution");
                }

                TTLInterpreter currentInterpreter = interpreter;
                
                // 訂閱狀態更新事件（每次运行都需要订阅，因为状态标签可能不同）
                currentInterpreter.StatusChanged += (fileName, lineNumber, command) =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            UpdateScriptStatus(statusLabel, fileName, lineNumber, command);
                        }));
                    }
                    else
                    {
                        UpdateScriptStatus(statusLabel, fileName, lineNumber, command);
                    }
                };
                
                // 訂閱 PDU 連線事件
                currentInterpreter.PduConnectRequested += Interpreter_PduConnectRequested;

                // 訂閱 PDU 控制事件 (with terminal index)
                currentInterpreter.PduControlRequested += (device, port, action) =>
                {
                    OnTtlPduControlRequested(terminalIndex, device, port, action);
                };
                
                // 訂閱 CountKeys 事件
                currentInterpreter.CountKeysRequested += (keyword) =>
                {
                    OnCountKeysRequested(terminalIndex, keyword);
                };
                
                // REMOVED: 不要在这里重复订阅 SendAll 和 WaitAll 事件
                // 已经在上面创建 interpreter 时或在 CreateInterpreterForTerminal 中订阅过了

                // Execute script (run in background to avoid blocking UI)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        AppLogger.LogInfo("=== Starting TTL Script ===");
                        AppLogger.LogInfo($"Script file: {scriptFilePath}");
                        
                        // 使用現有的方法（不帶 CancellationToken）
                        currentInterpreter.ExecuteScriptContent(script, scriptFileName);
                        
                        // 檢查是否被取消
                        if (token.IsCancellationRequested)
                        {
                            AppLogger.LogInfo("=== TTL Script Cancelled ===");
                            
                            Invoke(new Action(() =>
                            {
                                statusLabel.Text = $"Script - {scriptFileName} : Stopped by user";
                                statusLabel.ForeColor = System.Drawing.Color.Orange;
                                
                                runButton.Enabled = true;
                                endButton.Visible = false;
                            }));
                            return;
                        }
                        
                        AppLogger.LogInfo("=== TTL Script Completed ===");
                        
                        Invoke(new Action(() =>
                        {
                            statusLabel.Text = $"Script - {scriptFileName} : Completed";
                            statusLabel.ForeColor = System.Drawing.Color.Green;
                            
                            runButton.Enabled = true;
                            endButton.Visible = false;
                        }));
                    }
                    catch (OperationCanceledException)
                    {
                        AppLogger.LogInfo("=== TTL Script Cancelled ===");
                        
                        Invoke(new Action(() =>
                        {
                            statusLabel.Text = $"Script - {scriptFileName} : Stopped by user";
                            statusLabel.ForeColor = System.Drawing.Color.Orange;
                            
                            runButton.Enabled = true;
                            endButton.Visible = false;
                        }));
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Script error in '{scriptFileName}':\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            
                            statusLabel.Text = $"Script - {scriptFileName} : Error";
                            statusLabel.ForeColor = System.Drawing.Color.Red;
                            
                            runButton.Enabled = true;
                            endButton.Visible = false;
                        }));
                        AppLogger.LogError($"[TTL Error] {ex.Message}");
                    }
                }, token);
                
                statusLabel.Text = $"Script - {scriptFileName} : Starting...";
                statusLabel.ForeColor = System.Drawing.Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.LogError($"Error: {ex.Message}");
                
                runButton.Enabled = true;
                endButton.Visible = false;
            }
        }

        private void EndTTLScript(ref TTLInterpreter interpreter, ref CancellationTokenSource cts, Label statusLabel, Button runButton, Button endButton)
        {
            try
            {
                // 1. 先呼叫 interpreter 的 Cancel() 方法來設定取消旗標
                if (interpreter != null)
                {
                    AppLogger.LogInfo("[TTL] Calling Cancel() on interpreter");
                    interpreter.Cancel();
                }
                
                // 2. 發送取消請求給 CancellationTokenSource
                if (cts != null && !cts.IsCancellationRequested)
                {
                    AppLogger.LogInfo("[TTL] Sending cancellation request to CTS");
                    cts.Cancel();
                }
                
                // 3. 等待一小段時間讓腳本有機會優雅地停止
                System.Threading.Thread.Sleep(200);
                
                // 4. 不要釋放 interpreter，保留它以便 waitall 繼續工作和下次腳本重用
                // interpreter 會在斷開終端連線時被釋放
                
                // 更新狀態
                statusLabel.Text = "Script - Stopped by user";
                statusLabel.ForeColor = System.Drawing.Color.Orange;
                
                AppLogger.LogInfo("[TTL] Script stopped by user");
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[TTL] Error stopping script: {ex.Message}");
            }
            
            // 恢復按鈕狀態
            runButton.Enabled = true;
            endButton.Visible = false;
        }
        
        /// <summary>
        /// Send text to all connected terminals
        /// </summary>
        private void SendToAllTerminals(string text, bool newline)
        {
            string dataToSend = text + (newline ? "\r\n" : "");
            
            ComPortBridge[] bridges = { bridge1, bridge2, bridge3, bridge4, bridge5, bridge6 };
            int sentCount = 0;
            
            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i] != null && bridges[i].IsRunning)
                {
                    try
                    {
                        bridges[i].SendData(dataToSend);
                        sentCount++;
                        //AppLogger.LogInfo($"[SendAll] Sent to Terminal {i + 1}: {text}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError($"[SendAll] Failed to send to Terminal {i + 1}: {ex.Message}");
                    }
                }
            }
            
            //AppLogger.LogInfo($"[SendAll] Sent to {sentCount} terminal(s)");
        }
        
        /// <summary>
        /// Check if all connected terminals have received the specified text
        /// </summary>
        private bool CheckAllTerminalsReceived(string text)
        {
            TTLInterpreter[] interpreters = { ttlInterpreter1, ttlInterpreter2, ttlInterpreter3, 
                                            ttlInterpreter4, ttlInterpreter5, ttlInterpreter6 };
            ComPortBridge[] bridges = { bridge1, bridge2, bridge3, bridge4, bridge5, bridge6 };
            
            bool anyConnected = false;
            bool allReceived = true;
            
            //AppLogger.LogInfo($"[WaitAll Check] Looking for text: '{text}'");
            
            for (int i = 0; i < bridges.Length; i++)
            {
                // 只檢查已連線的終端
                if (bridges[i] != null && bridges[i].IsRunning)
                {
                    anyConnected = true;
                    //AppLogger.LogInfo($"[WaitAll Check] Terminal {i + 1} is connected (Bridge: {bridges[i].Port1Name} <-> {bridges[i].Port2Name})");
                    
                    if (interpreters[i] == null)
                    {
                        //AppLogger.LogWarning($"[WaitAll Check] Terminal {i + 1} interpreter is null!");
                        allReceived = false;
                        break;
                    }
                    
                    // 获取解释器缓冲区的实际内容用于调试
                    string bufferPreview = GetInterpreterBufferPreview(interpreters[i]);
                    //AppLogger.LogInfo($"[WaitAll Check] Terminal {i + 1} buffer: {bufferPreview}");
                    
                    bool hasText = interpreters[i].ContainsText(text);
                    //AppLogger.LogInfo($"[WaitAll Check] Terminal {i + 1} has text '{text}': {hasText}");
                    
                    if (!hasText)
                    {
                        allReceived = false;
                        break;
                    }
                }
                else
                {
                    if (bridges[i] != null)
                    {
                        //AppLogger.LogInfo($"[WaitAll Check] Terminal {i + 1} bridge not running (IsRunning: {bridges[i].IsRunning})");
                    }
                    else
                    {
                        //AppLogger.LogInfo($"[WaitAll Check] Terminal {i + 1} bridge is null");
                    }
                }
            }
            
            // 如果沒有任何連線的終端，返回 false
            if (!anyConnected)
            {
                //AppLogger.LogWarning("[WaitAll Check] No terminals connected!");
                return false;
            }
            
            // 如果所有已連線的終端都收到了，清除各終端的緩衝區
            if (allReceived)
            {
                //AppLogger.LogInfo($"[WaitAll Check] All terminals received the text!");
                for (int i = 0; i < bridges.Length; i++)
                {
                    if (bridges[i] != null && bridges[i].IsRunning && interpreters[i] != null)
                    {
                        interpreters[i].RemoveTextFromBuffer(text);
                        //AppLogger.LogInfo($"[WaitAll] Terminal {i + 1} buffer cleared");
                    }
                }
            }
            else
            {
                //AppLogger.LogInfo($"[WaitAll Check] Not all terminals have received the text yet");
            }
            
            return allReceived;
        }
        
        /// <summary>
        /// Get a preview of interpreter buffer content for debugging
        /// </summary>
        private string GetInterpreterBufferPreview(TTLInterpreter interpreter)
        {
            if (interpreter == null)
                return "[null interpreter]";
            
            try
            {
                // 使用反射访问私有的 receiveBuffer 字段
                var bufferField = typeof(TTLInterpreter).GetField("receiveBuffer", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (bufferField != null)
                {
                    var buffer = bufferField.GetValue(interpreter) as System.Text.StringBuilder;
                    if (buffer != null)
                    {
                        string content = buffer.ToString();
                        if (content.Length == 0)
                            return "[empty]";
                        else if (content.Length <= 100)
                            return $"'{content}'";
                        else
                            return $"'{content.Substring(0, 100)}...' (length: {content.Length})";
                    }
                }
                return "[cannot access buffer]";
            }
            catch (Exception ex)
            {
                return $"[error: {ex.Message}]";
            }
        }
        
        /// <summary>
        /// Create TTLInterpreter for a terminal (used for waitall support)
        /// </summary>
        private void CreateInterpreterForTerminal(int terminalIndex, ref ComPortBridge bridge, ref TTLInterpreter interpreter)
        {
            try
            {
                if (bridge == null || !bridge.IsRunning)
                {
                    AppLogger.LogWarning($"[Terminal {terminalIndex + 1}] Cannot create interpreter: bridge not running");
                    return;
                }
                
                // Dispose existing interpreter if any
                interpreter?.Dispose();
                
                // Create new interpreter
                interpreter = new TTLInterpreter(bridge);
                AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] Interpreter created for waitall support");
                
                // Event handlers for waitall and sendall (only subscribe once when creating interpreter)
                interpreter.WaitAllRequested += (text) =>
                {
                    //AppLogger.LogInfo($"[WaitAll Event] Called from Terminal {terminalIndex + 1}, checking for: '{text}'");
                    bool result = CheckAllTerminalsReceived(text);
                    //AppLogger.LogInfo($"[WaitAll Event] Returning: {result}");
                    return result;
                };
                
                interpreter.SendAllRequested += (text, newline) =>
                {
                    //AppLogger.LogInfo($"[SendAll Event] Called from Terminal {terminalIndex + 1}");
                    SendToAllTerminals(text, newline);
                };
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[Terminal {terminalIndex + 1}] Failed to create interpreter: {ex.Message}");
            }
        }

        private void UpdateScriptStatus(Label statusLabel, string fileName, int lineNumber, string command)
        {
            // 截斷過長的指令
            string displayCommand = $"{fileName} : {lineNumber} : {command}";

            displayCommand = displayCommand.Length > 100 ? displayCommand.Substring(0, 97) + "..." : displayCommand;
            
            // 更新 Label 文字
            statusLabel.Text = displayCommand;
            
            // 根據不同指令類型設定顏色
            if (command.ToLower().StartsWith("wait"))
            {
                statusLabel.ForeColor = System.Drawing.Color.Orange;
            }
            else if (command.ToLower().StartsWith("send"))
            {
                statusLabel.ForeColor = System.Drawing.Color.Blue;
            }
            else if (command == "Completed")
            {
                statusLabel.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                statusLabel.ForeColor = System.Drawing.Color.Black;
            }
        }
        
        /// <summary>
        /// Handle countkeys command - increment counter and log
        /// </summary>
        private void OnCountKeysRequested(int terminalIndex, string keyword)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnCountKeysRequested(terminalIndex, keyword)));
                return;
            }
            
            try
            {
                // Increment counter
                pduPowerOnCount[terminalIndex]++;
                
                // Log to Debug log
                AppLogger.LogInfo($"[Terminal {terminalIndex + 1}] CountKeys '{keyword}': {pduPowerOnCount[terminalIndex]}");
                
                // Log to RX log
                if (rxLogWriters[terminalIndex] != null)
                {
                    WriteRxLog(terminalIndex, $"\n[CountKeys] '{keyword}' Count: {pduPowerOnCount[terminalIndex]}\n", false);
                }
                
                // Update status bar
                UpdatePduPowerCycleStatus();
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"[Terminal {terminalIndex + 1}] CountKeys error: {ex.Message}");
            }
        }
        #endregion

        #region EmbedWindow Method
        private void EmbedExternalWindow(Process process, ref EmbeddedWindowController controller, Panel targetPanel)
        {
            if (process == null)
                return;

            process.WaitForInputIdle();
            System.Threading.Thread.Sleep(500);
            
            IntPtr exeHandle = process.MainWindowHandle;
            if (exeHandle != IntPtr.Zero)
            {
                // Remove menu bar
                SetMenu(exeHandle, IntPtr.Zero);

                // Remove window decorations
                int style = GetWindowLong(exeHandle, GWL_STYLE);
                style = style & ~WS_CAPTION & ~WS_BORDER & ~WS_THICKFRAME
                        & ~WS_MINIMIZEBOX & ~WS_MAXIMIZEBOX & ~WS_SYSMENU;
                SetWindowLong(exeHandle, GWL_STYLE, style);

                int exStyle = GetWindowLong(exeHandle, GWL_EXSTYLE);
                exStyle = exStyle & ~WS_EX_DLGMODALFRAME;
                SetWindowLong(exeHandle, GWL_EXSTYLE, exStyle);

                SetWindowPos(exeHandle, IntPtr.Zero, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

                // Embed window into target panel
                SetParent(exeHandle, targetPanel.Handle);
                int embeddedX = 0;
                int embeddedY = 75;
                int embeddedWidth = targetPanel.Width;
                int embeddedHeight = targetPanel.Height - 75;
                //MoveWindow(exeHandle, 0, 50, targetPanel.Width, targetPanel.Height - 50, true);
                MoveWindow(exeHandle, embeddedX, embeddedY, embeddedWidth, embeddedHeight, true);

                // Create controller for the embedded window
                controller = new EmbeddedWindowController(exeHandle, targetPanel);
            }
        }
        private void EmbedExeForm_Resize(object sender, EventArgs e)
        {
            windowController1?.UpdateSize();
            windowController2?.UpdateSize();
            windowController3?.UpdateSize();
            windowController4?.UpdateSize();
            windowController5?.UpdateSize();
            windowController6?.UpdateSize();
        }
        #endregion
    
        #region Status Strip Handlers
        private void timerLocalTime_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel3.Text = $"Local time: {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
        }
        #endregion
    
        #region Form Closing Handler
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Close all RX log files
            for (int i = 0; i < rxLogWriters.Length; i++)
            {
                if (rxLogWriters[i] != null)
                {
                    try
                    {
                        lock (rxLogLocks[i])
                        {
                            rxLogWriters[i].WriteLine();
                            rxLogWriters[i].WriteLine($"=== Application closed at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                            rxLogWriters[i].Close();
                            rxLogWriters[i].Dispose();
                            rxLogWriters[i] = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError($"Failed to close RX log {i + 1}", ex);
                    }
                }
            }
            
            // Cleanup PDU resources
            CleanupPduResources();
            
            // Close all TTL Interpreters
            ttlInterpreter1?.Dispose();
            ttlInterpreter2?.Dispose();
            ttlInterpreter3?.Dispose();
            ttlInterpreter4?.Dispose();
            ttlInterpreter5?.Dispose();
            ttlInterpreter6?.Dispose();
            
            // Stop all bridges
            bridge1?.Stop();
            bridge1?.Dispose();
            bridge2?.Stop();
            bridge2?.Dispose();
            bridge3?.Stop();
            bridge3?.Dispose();
            bridge4?.Stop();
            bridge4?.Dispose();
            bridge5?.Stop();
            bridge5?.Dispose();
            bridge6?.Stop();
            bridge6?.Dispose();
            
            base.OnFormClosing(e);
        }
        #endregion
    }
}
