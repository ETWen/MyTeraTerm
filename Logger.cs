using System;
using System.IO;
using System.Text;

namespace MyTeraTerm
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class Logger
    {
        private string logFilePath;
        private readonly object lockObj = new object();
        private bool isInitialized = false;
        private string loggerName;
        private LogLevel minimumLevel;

        /// <summary>
        /// 建立 Logger 實例
        /// </summary>
        /// <param name="folderName">Log 資料夾名稱</param>
        /// <param name="filePrefix">檔案前綴</param>
        /// <param name="minLevel">最低記錄等級</param>
        public Logger(string folderName, string filePrefix, LogLevel minLevel = LogLevel.Info)
        {
            loggerName = filePrefix;
            minimumLevel = minLevel;
            Initialize(folderName, filePrefix);
        }

        private void Initialize(string folderName, string filePrefix)
        {
            if (isInitialized)
                return;

            try
            {
                // 建立 Log 資料夾
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string logsFolder = Path.Combine(appPath, folderName);
                
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                // 建立 log 檔案名稱（包含日期時間）
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                logFilePath = Path.Combine(logsFolder, $"{filePrefix}_{timestamp}.log");

                // 寫入初始訊息
                WriteToFile("=".PadRight(80, '='));
                WriteToFile($"{filePrefix} Log Started");
                WriteToFile($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                WriteToFile($"User: {Environment.UserName}");
                WriteToFile($"Machine: {Environment.MachineName}");
                WriteToFile($"OS: {Environment.OSVersion}");
                WriteToFile($"Version: {GetApplicationVersion()}");
                WriteToFile("=".PadRight(80, '='));
                WriteToFile("");

                isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{loggerName} Error] Failed to initialize: {ex.Message}");
            }
        }

        /// <summary>
        /// 寫入 log 訊息
        /// </summary>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!isInitialized || level < minimumLevel)
                return;

            string levelStr = level.ToString().ToUpper().PadRight(7);
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{levelStr}] {message}";
            
            // 寫入檔案
            WriteToFile(logEntry);
        }

        /// <summary>
        /// 寫入 Debug 訊息
        /// </summary>
        public void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        /// <summary>
        /// 寫入 Info 訊息
        /// </summary>
        public void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// 寫入 Warning 訊息
        /// </summary>
        public void Warning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        /// <summary>
        /// 寫入 Error 訊息
        /// </summary>
        public void Error(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $"\nException: {ex.GetType().Name}: {ex.Message}\nStackTrace: {ex.StackTrace}";
            }
            Log(errorMessage, LogLevel.Error);
        }

        /// <summary>
        /// 實際寫入檔案的方法（執行緒安全）
        /// </summary>
        private void WriteToFile(string message)
        {
            if (string.IsNullOrEmpty(logFilePath))
                return;

            lock (lockObj)
            {
                try
                {
                    File.AppendAllText(logFilePath, message + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // 寫入失敗，靜默處理
                }
            }
        }

        /// <summary>
        /// 取得目前 log 檔案路徑
        /// </summary>
        public string GetLogFilePath()
        {
            return logFilePath ?? "Not initialized";
        }

        /// <summary>
        /// 清理舊的 log 檔案（保留最近 N 天）
        /// </summary>
        public void CleanupOldLogs(string folderName, string filePattern, int daysToKeep = 30)
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string logsFolder = Path.Combine(appPath, folderName);

                if (!Directory.Exists(logsFolder))
                    return;

                var files = Directory.GetFiles(logsFolder, filePattern);
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        try
                        {
                            File.Delete(file);
                            Info($"Deleted old log file: {fileInfo.Name}");
                        }
                        catch (Exception ex)
                        {
                            Error($"Failed to delete old log: {fileInfo.Name}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Failed to cleanup old logs", ex);
            }
        }

        private string GetApplicationVersion()
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
    }
}