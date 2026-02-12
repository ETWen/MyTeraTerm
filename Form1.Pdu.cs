using PDUControlLib;
using RJControlslib;

namespace MyTeraTerm
{
    public partial class Form1
    {
        #region Pdu Fields
        private PduController? pduController;
        private const int LIST_VIEW_ROW_HEIGHT = 28;
        #endregion

        #region Pdu Initialization
        
        /// <summary>
        /// 初始化 PDU 控制相關邏輯（事件處理器等）
        /// 此方法由 Form1.Designer.cs 的 InitializePduControl 調用
        /// </summary>
        public void InitializePduControlLogic()
        {
            SetPduButtonsEnabled(false);
        }

        /// <summary>
        /// 初始化 PDU DataGridView（已在 Designer.cs 實作）
        /// 這個方法保留以便未來擴展
        /// </summary>
        private void InitializePduDataGridView()
        {
            // DataGridView 的初始化已在 Designer.cs 中完成
            // 這裡只做額外的設定（如果需要）
            
            AppLogger.LogDebug("[PDU] DataGridView initialized");
        }

        #endregion

        #region Pdu DataGridView Handlers
        
        /// <summary>
        /// 更新 PDU DataGridView 中指定 Port 的資訊
        /// </summary>
        private void UpdatePduDataGridView(int portNumber, bool? state, int? current, int? power)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdatePduDataGridView(portNumber, state, current, power)));
                return;
            }
            
            // 檢查 dgvPdu 是否已初始化
            if (dgvPdu == null)
            {
                AppLogger.LogWarning("[PDU] DataGridView not initialized yet");
                return;
            }
            
            int rowIndex = portNumber - 1;
            if (rowIndex >= 0 && rowIndex < dgvPdu.Rows.Count)
            {
                try
                {
                    var row = dgvPdu.Rows[rowIndex];
                    
                    if (state.HasValue)
                    {
                        row.Cells["Status"].Value = state.Value ? "On" : "Off";
                        row.DefaultCellStyle.BackColor = state.Value ? Color.LightGreen : Color.LightGray;
                    }
                    
                    if (current.HasValue)
                        row.Cells["Current"].Value = $"{current.Value}";
                    
                    if (power.HasValue)
                    {
                        double watts = power.Value / 10.0;
                        row.Cells["Power"].Value = $"{watts:F2}";
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogError($"[PDU] Error updating DataGridView for port {portNumber}", ex);
                }
            }
            else
            {
                AppLogger.LogWarning($"[PDU] Invalid row index: {rowIndex} (Port {portNumber})");
            }
        }

        /// <summary>
        /// 定時更新 PDU 狀態
        /// </summary>
        private void TimerPduUpdate_Tick(object sender, EventArgs e)
        {
            if (pduController != null)
            {
                UpdateAllPduPortStates();
            }
        }

        #endregion

        #region Pdu Controller

        /// <summary>
        /// PDU 連接/斷開按鈕事件
        /// </summary>
        private void btnPduConnect_Click(object? sender, EventArgs e)
        {
            string pduIp = tbPduIpAddress.Text.Trim();
            
            // Check if IP address is provided
            if (string.IsNullOrWhiteSpace(pduIp))
            {
                AppLogger.LogInfo("[PDU] No IP address entered");
                MessageBox.Show("Please enter PDU IP address.", 
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 如果已經連線，先斷開
                if (pduController != null)
                {
                    pduController.Dispose();
                    pduController = null!;
                    btnPduConnect.Text = "Connect";
                    AppLogger.LogInfo("[PDU] Disconnected from PDU");
                    btnPduConnect.BackColor = System.Drawing.Color.LightCoral;
                    
                    // 停止定時器
                    timerPduUpdate?.Stop();
                    
                    // Disable all PDU buttons
                    SetPduButtonsEnabled(false);
                    return;
                }

                // Create PDU Controller
                pduController = new PduController(pduIp, "iPoMan II");
                AppLogger.LogInfo($"[PDU] Connecting to {pduIp}...");

                // Check connection
                if (pduController.CheckConnection())
                {
                    btnPduConnect.Text = "Disconnect";
                    btnPduConnect.BackColor = System.Drawing.Color.LightGreen;
                    
                    AppLogger.LogInfo($"[PDU] Connected successfully to {pduIp}");
                    MessageBox.Show($"PDU connected successfully!\n\nIP: {pduIp}\nModel: iPoMan II", 
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Enable all PDU buttons
                    SetPduButtonsEnabled(true);
                    
                    // Read and update all Port states
                    UpdateAllPduPortStates();
                    
                    // 啟動定時器
                    if (timerPduUpdate == null || !timerPduUpdate.Enabled)
                    {
                        timerPduUpdate?.Start();
                    }
                    AppLogger.LogDebug("[PDU] Auto-update timer started");
                }
                else
                {
                    pduController.Dispose();
                    pduController = null!;
                    
                    AppLogger.LogWarning($"[PDU] Connection failed to {pduIp}");
                    MessageBox.Show($"Failed to connect to PDU at {pduIp}\n\nPlease check:\n- IP address is correct\n- PDU is powered on\n- Network connection is working", 
                                "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                pduController?.Dispose();
                pduController = null!;
                
                AppLogger.LogError("[PDU] Connection error", ex);
                MessageBox.Show($"Error connecting to PDU:\n{ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 啟用/停用所有 PDU 按鈕
        /// </summary>
        private void SetPduButtonsEnabled(bool enabled)
        {
            btnPduPort1.Enabled = enabled;
            btnPduPort2.Enabled = enabled;
            btnPduPort3.Enabled = enabled;
            btnPduPort4.Enabled = enabled;
            btnPduPort5.Enabled = enabled;
            btnPduPort6.Enabled = enabled;
            btnPduPort7.Enabled = enabled;
            btnPduPort8.Enabled = enabled;
            btnPduPort9.Enabled = enabled;
            btnPduPort10.Enabled = enabled;
            btnPduPort11.Enabled = enabled;
            btnPduPort12.Enabled = enabled;
        }

        /// <summary>
        /// 更新所有 PDU Port 的狀態
        /// </summary>
        private void UpdateAllPduPortStates()
        {
            if (pduController == null)
                return;

            // 檢查 dgvPdu 是否已初始化
            if (dgvPdu == null)
            {
                AppLogger.LogWarning("[PDU] DataGridView not initialized, skipping update");
                return;
            }

            try
            {
                RJToggleButton[] buttons = {
                    btnPduPort1, btnPduPort2, btnPduPort3, btnPduPort4,
                    btnPduPort5, btnPduPort6, btnPduPort7, btnPduPort8,
                    btnPduPort9, btnPduPort10, btnPduPort11, btnPduPort12
                };

                for (int i = 0; i < buttons.Length; i++)
                {
                    int portNumber = i + 1;
                    bool? state = pduController.GetPduPortState(portNumber);
                    int? current = pduController.GetPduPortCurrent(portNumber);
                    int? power = pduController.GetPduPortPower(portNumber);
                    
                    
                    if (state.HasValue)
                    {
                        // 暫時取消訂閱事件，避免觸發控制命令
                        buttons[i].CheckedChanged -= BtnPduPort_CheckedChanged;
                        buttons[i].Checked = state.Value;
                        buttons[i].CheckedChanged += BtnPduPort_CheckedChanged;
                        
                    }
                    UpdatePduDataGridView(portNumber, state, current, power);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError("[PDU] Error updating all port states", ex);
            }
        }

        /// <summary>
        /// PDU Toggle 按鈕狀態改變事件
        /// </summary>
        private void BtnPduPort_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RJToggleButton button)
            {
                int portNumber = button.Tag != null ? (int)button.Tag : 0;
                if (portNumber == 0)
                    return;
                    
                bool isOn = button.Checked;
                
                AppLogger.LogInfo($"[PDU] User toggled Port {portNumber} to {(isOn ? "ON" : "OFF")}");
                
                // 控制 PDU
                if (pduController != null)
                {
                    try
                    {
                        bool success;
                        if (isOn)
                            success = pduController.SetPduPortOn(portNumber);
                        else
                            success = pduController.SetPduPortOff(portNumber);

                        if (success)
                        {
                            AppLogger.LogPduControl(pduController.GetDevice(), portNumber, isOn);
                            
                            // 等待一下再讀取狀態確認
                            System.Threading.Thread.Sleep(500);
                            bool? actualState = pduController.GetPduPortState(portNumber);
                            
                            if (actualState.HasValue && actualState.Value != isOn)
                            {
                                // 實際狀態與設定不符，更新按鈕
                                button.CheckedChanged -= BtnPduPort_CheckedChanged;
                                button.Checked = actualState.Value;
                                button.CheckedChanged += BtnPduPort_CheckedChanged;
                                
                                AppLogger.LogWarning($"[PDU] Port {portNumber} state mismatch after control command. Expected: {(isOn ? "ON" : "OFF")}, Actual: {(actualState.Value ? "ON" : "OFF")}");
                                MessageBox.Show($"Warning: Port {portNumber} state mismatch!\nExpected: {(isOn ? "ON" : "OFF")}\nActual: {(actualState.Value ? "ON" : "OFF")}", 
                                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            button.CheckedChanged -= BtnPduPort_CheckedChanged;
                            button.Checked = !isOn;
                            button.CheckedChanged += BtnPduPort_CheckedChanged;
                            AppLogger.LogWarning($"[PDU] Failed to turn {(isOn ? "ON" : "OFF")} Port {portNumber}");
                            MessageBox.Show($"Failed to control PDU Port {portNumber}", 
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        button.CheckedChanged -= BtnPduPort_CheckedChanged;
                        button.Checked = !isOn;
                        button.CheckedChanged += BtnPduPort_CheckedChanged;
                        AppLogger.LogError($"[PDU] Error controlling Port {portNumber}", ex);
                        MessageBox.Show($"Error controlling PDU Port {portNumber}:\n{ex.Message}", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // PDU 未連線，還原按鈕狀態
                    button.CheckedChanged -= BtnPduPort_CheckedChanged;
                    button.Checked = !isOn;
                    button.CheckedChanged += BtnPduPort_CheckedChanged;
                    
                    AppLogger.LogInfo("[PDU] Toggle attempted but PDU is not connected");
                    MessageBox.Show("PDU is not connected. Please connect first.", 
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        #endregion

        #region Pdu TTL Event Handlers

        /// <summary>
        /// TTL Script 請求連接 PDU 事件處理
        /// </summary>
        private void Interpreter_PduConnectRequested(int device, string ip)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Interpreter_PduConnectRequested(device, ip)));
                return;
            }

            try
            {
                if (device == 2)
                {
                    // 連接到 iPoMan II
                    tbPduIpAddress.Text = ip;
                    
                    // 直接建立連線
                    try
                    {
                        // 如果已經連線，先斷開
                        if (pduController != null)
                        {
                            AppLogger.LogInfo("[PDU] Disconnecting existing connection...");
                            pduController.Dispose();
                            pduController = null!;
                            btnPduConnect.Text = "Connect";
                            btnPduConnect.BackColor = System.Drawing.Color.LightCoral;
                            timerPduUpdate?.Stop();
                            SetPduButtonsEnabled(false);
                        }

                        // Create PDU Controller
                        pduController = new PduController(ip, "iPoMan II");
                        AppLogger.LogInfo($"[PDU] Connecting to {ip}...");

                        // Check connection
                        if (pduController.CheckConnection())
                        {
                            btnPduConnect.Text = "Disconnect";
                            btnPduConnect.BackColor = System.Drawing.Color.LightGreen;
                            
                            AppLogger.LogInfo("[PDU] Connected successfully");
                            
                            // Enable all PDU buttons
                            SetPduButtonsEnabled(true);
                            
                            // Read and update all Port states
                            UpdateAllPduPortStates();
                            
                            // 啟動定時器
                            if (timerPduUpdate == null || !timerPduUpdate.Enabled)
                            {
                                timerPduUpdate?.Start();
                            }
                            AppLogger.LogInfo("[PDU] Auto-update timer started");
                        }
                        else
                        {
                            pduController.Dispose();
                            pduController = null!;
                            
                            AppLogger.LogInfo("[PDU] Connection failed");
                            MessageBox.Show($"Failed to connect to PDU at {ip}\n\nPlease check:\n- IP address is correct\n- PDU is powered on\n- Network connection is working", 
                                        "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        pduController?.Dispose();
                        pduController = null!;
                        
                        MessageBox.Show($"Error connecting to PDU:\n{ex.Message}", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Console.WriteLine($"[PDU Error] {ex.Message}");
                    }
                }
                else if (device == 1)
                {
                    // 連接到 iPoMan I (如果未來有支援)
                    Console.WriteLine("[Form] iPoMan I not implemented yet");
                    AppLogger.LogInfo("[Form] iPoMan I support is not implemented yet");
                    MessageBox.Show("iPoMan I support is not implemented yet.", 
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Form] Error handling PDU connect: {ex.Message}");
                AppLogger.LogError("[Form] Error handling PDU connect request", ex);
                MessageBox.Show($"Error handling PDU connect request:\n{ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// TTL Script 請求控制 PDU 事件處理
        /// </summary>
        private void OnTtlPduControlRequested(int device, int port, int action)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnTtlPduControlRequested(device, port, action)));
                return;
            }
            
            AppLogger.LogInfo($"[Form] TTL PDU Control: Device={device}, Port={port}, Action={action}");
            
            // 檢查 PDU 是否連線
            if (pduController == null)
            {
                AppLogger.LogInfo("[Form] PDU not connected, ignoring command");
                MessageBox.Show("PDU is not connected. Please connect first.", 
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 檢查 device 是否符合當前的 PDU
            string currentModel = pduController.Model;
            string requestedModel = device == 1 ? "iPoMan I" : "iPoMan II";
            
            if (currentModel != requestedModel)
            {
                AppLogger.LogInfo($"[Form] PDU model mismatch. Connected: {currentModel}, Requested: {requestedModel}");
                MessageBox.Show($"PDU model mismatch!\nConnected: {currentModel}\nRequested: {requestedModel}", 
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                bool success;
                
                if (action == 1)
                {
                    success = pduController.SetPduPortOn(port);
                    AppLogger.LogInfo($"[Form] PDU Port {port} turned ON: {success}");
                }
                else
                {
                    success = pduController.SetPduPortOff(port);
                    AppLogger.LogInfo($"[Form] PDU Port {port} turned OFF: {success}");
                }
                
                if (success)
                {
                    // 等待一下再讀取狀態
                    System.Threading.Thread.Sleep(500);
                    
                    // 更新按鈕狀態
                    RJToggleButton[] buttons = {
                        btnPduPort1, btnPduPort2, btnPduPort3, btnPduPort4,
                        btnPduPort5, btnPduPort6, btnPduPort7, btnPduPort8,
                        btnPduPort9, btnPduPort10, btnPduPort11, btnPduPort12
                    };
                    
                    if (port >= 1 && port <= 12)
                    {
                        RJToggleButton btn = buttons[port - 1];
                        bool newState = action == 1;
                        
                        // 暫時取消訂閱，避免觸發事件
                        btn.CheckedChanged -= BtnPduPort_CheckedChanged;
                        btn.Checked = newState;
                        btn.CheckedChanged += BtnPduPort_CheckedChanged;
                        
                        // 更新 DataGridView
                        bool? state = pduController.GetPduPortState(port);
                        int? current = pduController.GetPduPortCurrent(port);
                        int? power = pduController.GetPduPortPower(port);
                        UpdatePduDataGridView(port, state, current, power);
                        
                        AppLogger.LogInfo($"[Form] UI updated for Port {port}");
                    }
                }
                else
                {
                    AppLogger.LogInfo($"[Form] Failed to control PDU Port {port}");
                    MessageBox.Show($"Failed to control PDU Port {port}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogInfo($"[Form] Error controlling PDU: {ex.Message}");
                MessageBox.Show($"Error controlling PDU:\n{ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Pdu Cleanup

        /// <summary>
        /// 清理 PDU 相關資源 (在 Form 關閉時呼叫)
        /// </summary>
        private void CleanupPduResources()
        {
            // Stop PDU update timer
            timerPduUpdate?.Stop();
            timerPduUpdate?.Dispose();
            
            // Close PDU Controller
            pduController?.Dispose();
            
            AppLogger.LogInfo("[PDU] Resources cleaned up");
        }

        #endregion
    }
}
