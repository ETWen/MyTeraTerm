using System;

namespace MyTeraTerm
{
    /// <summary>
    /// AppLogger 使用範例 - 展示各種功能的 logging 方式
    /// </summary>
    public class AppLogger_Usage_Example
    {
        public void ApplicationLifecycle()
        {
            // ===== 應用程式生命週期 =====
            
            // 1. 應用程式啟動（在 Form1 建構函式中）
            AppLogger.Initialize();
            AppLogger.LogInfo("Application starting...");
            
            // 2. 初始化完成
            AppLogger.LogInfo("Form1 initialization completed");
            
            // 3. 應用程式關閉
            AppLogger.LogInfo("Application closing...");
            AppLogger.LogInfo("Application closed successfully");
        }
        
        public void DebugConsole()
        {
            // ===== Debug Console（需要密碼） =====
            
            // 1. 開啟 Debug Console
            AppLogger.AllocateDebugConsole();
            AppLogger.LogInfo("Debug Console allocated");
            
            // 2. 密碼驗證失敗
            AppLogger.LogWarning("Debug console access denied - incorrect password");
        }
        
        public void TerminalOperations(string comPort, string baudRate, string teraTermPath)
        {
            // ===== 終端機連線操作 =====
            
            // 1. 連接終端機
            AppLogger.LogTerminalConnect(comPort, baudRate, teraTermPath);
            // Log 輸出: [INFO] [Terminal] Connecting to COM6 @ 115200 bps
            // Log 輸出: [DEBUG] [Terminal] TeraTerm Path: C:\Docu\...\ttermpro.exe
            
            // 2. 終端機連線成功後的詳細資訊
            AppLogger.LogDebug($"TeraTerm Process ID: 12345");
            AppLogger.LogDebug($"Virtual COM Port: COM101");
            
            // 3. 終端機斷線
            AppLogger.LogTerminalDisconnect(comPort);
            // Log 輸出: [INFO] [Terminal] Disconnected from COM6
            
            // 4. 終端機連線錯誤
            AppLogger.LogError("Failed to launch Tera Term", new Exception("File not found"));
            // Log 輸出: [ERROR] Failed to launch Tera Term
            //           Exception: System.Exception: File not found
            //           StackTrace: ...
        }
        
        public void SerialPortBridge(string data)
        {
            // ===== COM Port Bridge 資料傳輸 =====
            
            // 1. Bridge 啟動
            AppLogger.LogInfo("[Bridge] Starting COM Bridge: COM6 <-> COM101");
            AppLogger.LogDebug("[Bridge] Baud Rate: 115200, Buffer Size: 8192");
            
            // 2. RX 資料接收（詳細版本）
            AppLogger.LogBridgeData("RX", "COM6", 256, data);
            // Log 輸出: [INFO] [Bridge] RX: 256 bytes from COM6
            // Log 輸出: [DEBUG] [Bridge Detail] RX Data: AT+GMR\r\n...
            
            // 3. TX 資料傳送（簡單版本）
            AppLogger.LogTX(1, 128);
            // Log 輸出: [DEBUG] [TX] Port 1: 128 bytes transmitted
            
            // 4. RX 資料接收（簡單版本）
            AppLogger.LogRX(1, 256);
            // Log 輸出: [DEBUG] [RX] Port 1: 256 bytes received
            
            // 5. Bridge 錯誤
            AppLogger.LogError("[Bridge] Failed to open port", new Exception("Access denied"));
            
            // 6. Bridge 停止
            AppLogger.LogInfo("[Bridge] Bridge stopped");
        }
        
        public void TTLScriptExecution()
        {
            // ===== TTL Script 執行 =====
            
            // 1. 開始執行 Script
            AppLogger.LogInfo("[TTL] Starting script: test.ttl");
            
            // 2. 執行每行指令
            AppLogger.LogScriptExecution("test.ttl", "send 'AT+GMR\\r\\n'", 15);
            // Log 輸出: [INFO] [TTL] Script: test.ttl, Line 15
            // Log 輸出: [DEBUG] [TTL] Command: send 'AT+GMR\r\n'
            
            // 3. 等待指令
            AppLogger.LogScriptExecution("test.ttl", "wait 'OK'", 16);
            
            // 4. Script 錯誤
            AppLogger.LogScriptError("test.ttl", 20, "Timeout waiting for response");
            // Log 輸出: [ERROR] [TTL] Error in test.ttl at line 20: Timeout waiting for response
            
            // 5. Script 完成
            AppLogger.LogInfo("[TTL] Script completed successfully");
        }
        
        public void PDUControl()
        {
            // ===== PDU 電源控制 =====
            
            // 1. PDU 連線
            AppLogger.LogInfo("[PDU] Connecting to 192.168.1.100...");
            AppLogger.LogInfo("[PDU] Connected successfully to 192.168.1.100");
            
            // 2. PDU 斷線
            AppLogger.LogInfo("[PDU] Disconnected from PDU");
            
            // 3. 控制 PDU Port（開啟）
            AppLogger.LogPduControl("192.168.1.100", 1, true);
            // Log 輸出: [INFO] [PDU] Port 1 turned ON (IP: 192.168.1.100)
            
            // 4. 控制 PDU Port（關閉）
            AppLogger.LogPduControl("192.168.1.100", 2, false);
            // Log 輸出: [INFO] [PDU] Port 2 turned OFF (IP: 192.168.1.100)
            
            // 5. PDU 狀態更新
            AppLogger.LogDebug("[PDU] Auto-update timer started");
            AppLogger.LogDebug("[PDU] UI updated for Port 3");
            
            // 6. PDU 錯誤
            AppLogger.LogWarning("[PDU] Connection failed to 192.168.1.100");
            AppLogger.LogError("[PDU] Error controlling Port 5", new Exception("SNMP timeout"));
        }
        
        public void ErrorHandling()
        {
            // ===== 錯誤處理範例 =====
            
            try
            {
                // 某些操作
                throw new InvalidOperationException("COM Port already in use");
            }
            catch (Exception ex)
            {
                // 記錄錯誤（包含完整 Exception 資訊）
                AppLogger.LogError("Failed to open COM port", ex);
                // Log 輸出: [ERROR] Failed to open COM port
                //           Exception: System.InvalidOperationException: COM Port already in use
                //           StackTrace: at ...
            }
            
            // 警告訊息（不是錯誤，但需要注意）
            AppLogger.LogWarning("Baud rate mismatch detected - using default 115200");
            
            // 資訊訊息（一般操作記錄）
            AppLogger.LogInfo("COM Port configuration saved");
            
            // Debug 訊息（只有開啟 Debug Console 才會記錄）
            AppLogger.LogDebug("Buffer content: [FF 00 AA 55 ...]");
        }
        
        public void UserActions()
        {
            // ===== 使用者操作記錄 =====
            
            // 1. 按鈕點擊
            AppLogger.LogInfo("User clicked Connect button");
            AppLogger.LogDebug($"Selected COM Port: COM6, Baud Rate: 115200");
            
            // 2. 設定變更
            AppLogger.LogInfo("User changed terminal layout to 2x2");
            
            // 3. 功能切換
            AppLogger.LogInfo("User toggled PDU Port 3");
            
            // 4. 檔案操作
            AppLogger.LogInfo("User selected script file: C:\\Scripts\\test.ttl");
            
            // 5. 批次操作
            AppLogger.LogInfo("Killed 3 Tera Term processes");
        }
        
        public void CompleteExample()
        {
            // ===== 完整的使用情境範例 =====
            
            // 步驟 1: 應用程式啟動
            AppLogger.Initialize();
            AppLogger.LogInfo("MyTeraTerm Application Starting...");
            
            // 步驟 2: 使用者選擇 COM Port 並連線
            string comPort = "COM6";
            string baudRate = "115200";
            AppLogger.LogInfo($"User selected COM Port: {comPort}, Baud Rate: {baudRate}");
            
            try
            {
                // 步驟 3: 建立 Bridge
                AppLogger.LogInfo($"[Bridge] Starting COM Bridge: {comPort} <-> COM101");
                AppLogger.LogDebug($"[Bridge] Baud Rate: {baudRate}, Buffer Size: 8192");
                
                // 步驟 4: 啟動 Tera Term
                string teraTermPath = "C:\\MyTeraTerm\\Application\\TeraTerm\\ttermpro.exe";
                AppLogger.LogTerminalConnect("COM102", baudRate, teraTermPath);
                
                // 步驟 5: 資料傳輸
                AppLogger.LogTX(1, 15);
                AppLogger.LogRX(1, 42);
                
                // 步驟 6: 執行 TTL Script
                AppLogger.LogInfo("[TTL] Starting script: test.ttl");
                AppLogger.LogScriptExecution("test.ttl", "send 'AT\\r\\n'", 1);
                AppLogger.LogScriptExecution("test.ttl", "wait 'OK'", 2);
                
                // 步驟 7: 控制 PDU
                AppLogger.LogPduControl("192.168.1.100", 1, true);
                
                AppLogger.LogInfo("All operations completed successfully");
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Operation failed", ex);
            }
            
            // 步驟 8: 應用程式關閉
            AppLogger.LogInfo("Application closing...");
        }
        
        public void LogLevels()
        {
            // ===== Log 等級說明 =====
            
            // DEBUG - 詳細的除錯資訊（只有開啟 Debug Console 才記錄）
            AppLogger.LogDebug("Detailed debugging information");
            AppLogger.LogDebug("Function XYZ called with parameters: ...");
            AppLogger.LogDebug("Buffer content: [00 FF AA 55]");
            
            // INFO - 一般資訊記錄（永遠記錄）
            AppLogger.LogInfo("User connected to COM6");
            AppLogger.LogInfo("Terminal launched successfully");
            AppLogger.LogInfo("Script execution started");
            
            // WARNING - 警告訊息（不是錯誤，但值得注意）
            AppLogger.LogWarning("COM Port not available, using fallback");
            AppLogger.LogWarning("Connection timeout - retrying...");
            AppLogger.LogWarning("PDU Port state mismatch detected");
            
            // ERROR - 錯誤訊息（包含完整 Exception）
            AppLogger.LogError("Failed to open file", new FileNotFoundException());
            AppLogger.LogError("Network error", new System.Net.WebException());
            AppLogger.LogError("Serial port error", new Exception("Access denied"));
        }
        
        public void RealWorldLogOutput()
        {
            /*
             * ===== 實際的 Log 檔案範例 =====
             * 
             * 檔案路徑: DebugLogs/Debug_20260209_143022.log
             * 
             * ================================================================================
             * Debug Log Started
             * Time: 2026-02-09 14:30:22
             * User: YourName
             * Machine: DESKTOP-ABC123
             * OS: Microsoft Windows NT 10.0.22631.0
             * Version: 1.0.0
             * ================================================================================
             * 
             * [2026-02-09 14:30:22.123] [INFO   ] Application starting...
             * [2026-02-09 14:30:22.456] [INFO   ] Form1 initialization completed
             * [2026-02-09 14:30:25.789] [INFO   ] User selected COM Port: COM6, Baud Rate: 115200
             * [2026-02-09 14:30:26.012] [INFO   ] [Bridge] Starting COM Bridge: COM6 <-> COM101
             * [2026-02-09 14:30:26.034] [DEBUG  ] [Bridge] Baud Rate: 115200, Buffer Size: 8192
             * [2026-02-09 14:30:26.567] [INFO   ] [Terminal] Connecting to COM102 @ 115200 bps
             * [2026-02-09 14:30:26.568] [DEBUG  ] [Terminal] TeraTerm Path: C:\MyTeraTerm\Application\TeraTerm\ttermpro.exe
             * [2026-02-09 14:30:28.123] [DEBUG  ] [TX] Port 1: 15 bytes transmitted
             * [2026-02-09 14:30:28.234] [DEBUG  ] [RX] Port 1: 42 bytes received
             * [2026-02-09 14:30:30.456] [INFO   ] [TTL] Starting script: test.ttl
             * [2026-02-09 14:30:30.457] [INFO   ] [TTL] Script: test.ttl, Line 1
             * [2026-02-09 14:30:30.458] [DEBUG  ] [TTL] Command: send 'AT\r\n'
             * [2026-02-09 14:30:31.123] [INFO   ] [TTL] Script: test.ttl, Line 2
             * [2026-02-09 14:30:31.124] [DEBUG  ] [TTL] Command: wait 'OK'
             * [2026-02-09 14:30:35.678] [INFO   ] [PDU] Connecting to 192.168.1.100...
             * [2026-02-09 14:30:36.234] [INFO   ] [PDU] Connected successfully to 192.168.1.100
             * [2026-02-09 14:30:36.789] [INFO   ] [PDU] Port 1 turned ON (IP: 192.168.1.100)
             * [2026-02-09 14:30:40.123] [WARNING] [PDU] Connection timeout - retrying...
             * [2026-02-09 14:30:45.456] [ERROR  ] Failed to read PDU status
             * Exception: System.Net.WebException: The operation has timed out
             * StackTrace:    at System.Net.HttpWebRequest.GetResponse()
             *    at PDUControlLib.PduController.GetStatus()
             *    at MyTeraTerm.Form1.UpdatePduStatus()
             * [2026-02-09 14:35:22.789] [INFO   ] Application closing...
             * [2026-02-09 14:35:23.012] [INFO   ] Application closed successfully
             * 
             * ================================================================================
             */
        }
    }
}
