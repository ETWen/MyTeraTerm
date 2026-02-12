using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyTeraTerm
{
    /// <summary>
    /// 應用程式 Logger 管理類別
    /// - DebugLog: 永遠記錄所有操作（使用者可提供此 log 協助除錯）
    /// - Console: 需要密碼才能開啟即時 Console 視窗
    /// </summary>
    public static class AppLogger
    {
        // Debug Logger（永遠啟用，記錄所有操作）
        private static Logger debugLogger;
        
        // Console 是否已開啟
        private static bool isConsoleAllocated = false;
        
        private static readonly object lockObj = new object();

        /// <summary>
        /// 初始化 Logger（應用程式啟動時呼叫）
        /// </summary>
        public static void Initialize()
        {
            lock (lockObj)
            {
                try
                {
                    // 建立 Debug Logger（永遠啟用，記錄所有等級）
                    debugLogger = new Logger("DebugLogs", "Debug", LogLevel.Debug);
                    
                    debugLogger.Info("=".PadRight(80, '='));
                    debugLogger.Info("=== Application Started ===");
                    debugLogger.Info($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    debugLogger.Info($"Version: {GetApplicationVersion()}");
                    debugLogger.Info($"User: {Environment.UserName}");
                    debugLogger.Info($"Machine: {Environment.MachineName}");
                    debugLogger.Info($"OS: {Environment.OSVersion}");
                    debugLogger.Info($"Executable: {Application.ExecutablePath}");
                    debugLogger.Info($"Working Dir: {Environment.CurrentDirectory}");
                    debugLogger.Info($".NET Version: {Environment.Version}");
                    debugLogger.Info("=".PadRight(80, '='));
                    debugLogger.Info("");
                    
                    // 清理 30 天前的 debug log
                    debugLogger.CleanupOldLogs("DebugLogs", "Debug_*.log", 30);
                }
                catch (Exception ex)
                {
                    // Logger 初始化失敗，嘗試輸出到 Console（如果有）
                    try
                    {
                        Console.WriteLine($"[AppLogger Error] Failed to initialize: {ex.Message}");
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 開啟 Debug Console（需要密碼驗證）
        /// 這只是開啟即時顯示視窗，不影響 log 記錄
        /// </summary>
        public static void AllocateDebugConsole()
        {
            lock (lockObj)
            {
                if (isConsoleAllocated)
                {
                    LogWarning("Debug Console already allocated");
                    return;
                }

                try
                {
                    // 分配 Console 視窗
                    AllocConsole();
                    isConsoleAllocated = true;
                    
                    LogInfo("=== Debug Console Allocated ===");
                    LogInfo($"Console window opened for real-time debugging");
                    LogInfo($"Log file: {GetDebugLogPath()}");
                    
                    // 在 Console 顯示歡迎訊息
                    Console.WriteLine("=".PadRight(80, '='));
                    Console.WriteLine("=== Debug Console Activated ===");
                    Console.WriteLine("=".PadRight(80, '='));
                    Console.WriteLine($"Time:      {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"User:      {Environment.UserName}");
                    Console.WriteLine($"Machine:   {Environment.MachineName}");
                    Console.WriteLine($"Version:   {GetApplicationVersion()}");
                    Console.WriteLine($"Log File:  {GetDebugLogPath()}");
                    Console.WriteLine("=".PadRight(80, '='));
                    Console.WriteLine();
                    Console.WriteLine("All operations are being logged to the file above.");
                    Console.WriteLine("This console shows real-time debug messages.");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    LogError("Failed to allocate debug console", ex);
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// Debug 訊息（詳細的除錯資訊）
        /// </summary>
        public static void LogDebug(string message)
        {
            debugLogger?.Debug(message);
            
            // 如果 Console 已開啟，也即時顯示
            if (isConsoleAllocated)
            {
                try
                {
                    Console.WriteLine($"[DEBUG] {message}");
                }
                catch { }
            }
        }

        /// <summary>
        /// 資訊訊息（一般操作記錄）
        /// </summary>
        public static void LogInfo(string message)
        {
            debugLogger?.Info(message);
            
            if (isConsoleAllocated)
            {
                try
                {
                    Console.WriteLine($"[INFO] {message}");
                }
                catch { }
            }
        }

        /// <summary>
        /// 警告訊息
        /// </summary>
        public static void LogWarning(string message)
        {
            debugLogger?.Warning(message);
            
            if (isConsoleAllocated)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] {message}");
                    Console.ResetColor();
                }
                catch { }
            }
        }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            debugLogger?.Error(message, ex);
            
            if (isConsoleAllocated)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] {message}");
                    if (ex != null)
                    {
                        Console.WriteLine($"  Exception: {ex.Message}");
                    }
                    Console.ResetColor();
                }
                catch { }
            }
        }

        /// <summary>
        /// 取得 Debug Log 路徑
        /// </summary>
        public static string GetDebugLogPath()
        {
            return debugLogger?.GetLogFilePath() ?? "Not initialized";
        }

        /// <summary>
        /// 檢查 Console 是否已開啟
        /// </summary>
        public static bool IsConsoleAllocated()
        {
            return isConsoleAllocated;
        }

        /// <summary>
        /// 開啟 DebugLogs 資料夾
        /// </summary>
        public static void OpenDebugLogsFolder()
        {
            try
            {
                string logsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugLogs");
                
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }
                
                Process.Start("explorer.exe", logsFolder);
                LogInfo("Opened DebugLogs folder");
            }
            catch (Exception ex)
            {
                LogError("Failed to open DebugLogs folder", ex);
            }
        }

        /// <summary>
        /// 取得應用程式版本
        /// </summary>
        private static string GetApplicationVersion()
        {
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version?.Major}.{version?.Minor}.{version?.Build}";
            }
            catch
            {
                return "Unknown";
            }
        }

        #region 便利方法 - 記錄特定事件

        /// <summary>
        /// 記錄 COM Port Bridge 的詳細資訊
        /// </summary>
        public static void LogBridgeData(string direction, string portName, int bytesCount, string data)
        {
            // 簡單記錄
            LogInfo($"[Bridge] {direction}: {bytesCount} bytes from {portName}");
            
            // 詳細記錄（包含資料內容）
            string displayData = data.Replace("\r", "\\r").Replace("\n", "\\n");
            if (displayData.Length > 100)
                displayData = displayData.Substring(0, 97) + "...";
            
            LogDebug($"[Bridge Detail] {direction} Data: {displayData}");
        }

        /// <summary>
        /// 記錄 TX 事件
        /// </summary>
        public static void LogTX(int portNumber, int bytesCount)
        {
            LogDebug($"[TX] Port {portNumber}: {bytesCount} bytes transmitted");
        }

        /// <summary>
        /// 記錄 RX 事件
        /// </summary>
        public static void LogRX(int portNumber, int bytesCount)
        {
            LogDebug($"[RX] Port {portNumber}: {bytesCount} bytes received");
        }

        /// <summary>
        /// 記錄終端連線事件
        /// </summary>
        public static void LogTerminalConnect(string comPort, string baudRate, string teraTermPath)
        {
            LogInfo($"[Terminal] Connecting to {comPort} @ {baudRate} bps");
            LogDebug($"[Terminal] TeraTerm Path: {teraTermPath}");
        }

        /// <summary>
        /// 記錄終端中斷連線事件
        /// </summary>
        public static void LogTerminalDisconnect(string comPort)
        {
            LogInfo($"[Terminal] Disconnected from {comPort}");
        }

        /// <summary>
        /// 記錄 TTL Script 執行
        /// </summary>
        public static void LogScriptExecution(string scriptFile, string command, int lineNumber)
        {
            LogInfo($"[TTL] Script: {scriptFile}, Line {lineNumber}");
            LogDebug($"[TTL] Command: {command}");
        }

        /// <summary>
        /// 記錄 TTL Script 錯誤
        /// </summary>
        public static void LogScriptError(string scriptFile, int lineNumber, string error)
        {
            LogError($"[TTL] Error in {scriptFile} at line {lineNumber}: {error}");
        }

        /// <summary>
        /// 記錄 PDU 控制事件
        /// </summary>
        public static void LogPduControl(string ipAddress, int port, bool turnOn)
        {
            string action = turnOn ? "ON" : "OFF";
            LogInfo($"[PDU] Port {port} turned {action} (IP: {ipAddress})");
        }

        /// <summary>
        /// 記錄應用程式關閉
        /// </summary>
        public static void LogApplicationClose()
        {
            LogInfo("=== Application Closing ===");
            LogDebug("Cleanup process started");
            debugLogger?.Info("=".PadRight(80, '='));
            debugLogger?.Info("");
        }

        #endregion
    }
}