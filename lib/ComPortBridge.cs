using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using MyTeraTerm;

namespace COMPortBrdigeLib
{
    public class ComPortBridge : IDisposable
    {
        private SerialPort port1;
        private SerialPort port2;
        private Thread bridgeThread;
        private bool isRunning;

        // 新增：重試參數
        private const int RetryCount = 3;
        private const int RetryDelayMs = 1000;
        private const int BufferSize = 8192;

        // 新增：資料接收事件
        public event Action<string> DataTransmitted;
        public event Action<string> DataReceived;

        public ComPortBridge(string portName1, string portName2, string baudRate)
        {
            TryClosePort(portName1);
            TryClosePort(portName2);
            
            // ✅ 等待系統釋放資源
            Thread.Sleep(200);

            int baud = int.Parse(baudRate);

            port1 = new SerialPort(portName1, baud, Parity.None, 8, StopBits.One);
            port2 = new SerialPort(portName2, baud, Parity.None, 8, StopBits.One);
            
            // ✅ 性能優化：設置緩衝區大小
            port1.ReadBufferSize = BufferSize;
            port1.WriteBufferSize = BufferSize;
            port2.ReadBufferSize = BufferSize;
            port2.WriteBufferSize = BufferSize;
            
            port1.Encoding = Encoding.ASCII;
            port2.Encoding = Encoding.ASCII;
            
            // ✅ 增加 timeout 以處理高負載
            port1.ReadTimeout = 500;
            port1.WriteTimeout = 500;
            port2.ReadTimeout = 500;
            port2.WriteTimeout = 500;
            
            // ✅ 清除緩衝區
            try
            {
                port1.DiscardInBuffer();
                port1.DiscardOutBuffer();
                port2.DiscardInBuffer();
                port2.DiscardOutBuffer();
            }
            catch { /* Port may not be open yet */ }
        }

        private static void TryClosePort(string portName)
        {
            try
            {
                // 檢查 Port 是否存在
                if (!SerialPort.GetPortNames().Contains(portName))
                {
                    Console.WriteLine($"[Bridge] Port {portName} does not exist");
                    return;
                }
                
                // 嘗試開啟並立即關閉，釋放資源
                using (SerialPort tempPort = new SerialPort(portName))
                {
                    if (tempPort.IsOpen)
                    {
                        tempPort.Close();
                        Console.WriteLine($"[Bridge] Closed residual port: {portName}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"[Bridge] Port {portName} is in use by another process");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bridge] Error checking port {portName}: {ex.Message}");
            }
        }
        private bool OpenPortWithRetry(SerialPort port, string portName)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    if (!port.IsOpen)
                    {
                        AppLogger.LogDebug($"[Bridge] Attempting to open {portName} (Attempt {i + 1}/{RetryCount})");
                        port.Open();
                        AppLogger.LogInfo($"[Bridge] Successfully opened {portName}");
                        return true;
                    }
                    return true;
                }
                catch (UnauthorizedAccessException ex)
                {
                    AppLogger.LogWarning($"[Bridge] {portName} is in use (Attempt {i + 1}/{RetryCount})");
                    
                    if (i < RetryCount - 1)
                    {
                        // 嘗試釋放
                        TryClosePort(portName);
                        
                        // 等待後重試
                        AppLogger.LogDebug($"[Bridge] Waiting {RetryDelayMs}ms before retry...");
                        Thread.Sleep(RetryDelayMs);
                    }
                    else
                    {
                        // 最後一次失敗，記錄完整錯誤
                        AppLogger.LogError($"[Bridge] Failed to open {portName} - Access denied");
                    }
                }
                catch (IOException ex)
                {
                    AppLogger.LogWarning($"[Bridge] Port {portName} IO error: {ex.Message}");
                    
                    if (i < RetryCount - 1)
                    {
                        AppLogger.LogDebug($"[Bridge] Waiting {RetryDelayMs}ms before retry...");
                        Thread.Sleep(RetryDelayMs);
                    }
                    else
                    {
                        AppLogger.LogError($"[Bridge] Failed to open {portName}");
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogError($"[Bridge] Unexpected error opening {portName}");
                    return false;
                }
            }
            
            return false;
        }

        public void Start()
        {
            if (isRunning) return;

            AppLogger.LogInfo($"[Bridge] Starting: {port1.PortName} <-> {port2.PortName}");
            AppLogger.LogDebug($"[Bridge] Baud Rate: {port1.BaudRate}, Buffer Size: {BufferSize}");

            try
            {
                // 修改：先開虛擬 Port，再開實體 Port（避免資料遺失）
                bool port2Opened = OpenPortWithRetry(port2, port2.PortName);
                bool port1Opened = OpenPortWithRetry(port1, port1.PortName);
                
                if (!port1Opened || !port2Opened)
                {
                    string errorMsg = $"Failed to open ports after {RetryCount} attempts.\n" +
                                     $"Port1 ({port1.PortName}): {(port1Opened ? "OK" : "FAILED")}\n" +
                                     $"Port2 ({port2.PortName}): {(port2Opened ? "OK" : "FAILED")}\n\n" +
                                     "Possible solutions:\n" +
                                     "1. Close any other programs using these COM ports\n" +
                                     "2. Restart com0com service\n" +
                                     "3. Reboot your computer";
                    
                    AppLogger.LogError("[Bridge] Failed to open ports");
                    //throw new InvalidOperationException(errorMsg);
                }
                
                // 清除緩衝區，確保沒有舊資料
                port1.DiscardInBuffer();
                port1.DiscardOutBuffer();
                port2.DiscardInBuffer();
                port2.DiscardOutBuffer();

                isRunning = true;

                bridgeThread = new Thread(BridgeData);
                bridgeThread.IsBackground = true;
                bridgeThread.Priority = ThreadPriority.AboveNormal;  // 提高線程優先級
                bridgeThread.Name = $"ComBridge_{port1.PortName}_{port2.PortName}";
                bridgeThread.Start();

                AppLogger.LogInfo($"[Bridge] Started successfully: {port1.PortName} <-> {port2.PortName}");
            }
            catch (Exception ex)
            {
                AppLogger.LogError("[Bridge] Failed to start");
                Stop();
                throw;
            }
        }

        private void BridgeData()
        {
            // ✅ 增加緩衝區大小
            byte[] buffer1 = new byte[BufferSize];
            byte[] buffer2 = new byte[BufferSize];
            bool hadData = false;  // ✅ 追蹤是否有資料傳輸

            while (isRunning)
            {
                hadData = false;
                
                try
                {
                    // Port1 (實體) -> Port2 (虛擬): 接收資料 (RX)
                    if (port1.IsOpen && port1.BytesToRead > 0)
                    {
                        try
                        {
                            int bytesRead = port1.Read(buffer1, 0, Math.Min(buffer1.Length, port1.BytesToRead));
                            if (bytesRead > 0)
                            {
                                port2.Write(buffer1, 0, bytesRead);
                                hadData = true;
                                
                                // 觸發 RX 事件
                                string data = System.Text.Encoding.ASCII.GetString(buffer1, 0, bytesRead);
                                DataReceived?.Invoke(data);
                                Console.WriteLine($"[RX] {bytesRead} bytes from {port1.PortName}");
                            }
                        }
                        catch (TimeoutException)
                        {
                            // RX timeout 正常，繼續處理 TX
                        }
                        catch (Exception ex)
                        {
                            if (isRunning)
                            {
                                Console.WriteLine($"[RX Error] {ex.Message}");
                            }
                        }
                    }

                    // Port2 (虛擬) -> Port1 (實體): 傳送資料 (TX)
                    if (port2.IsOpen && port2.BytesToRead > 0)
                    {
                        try
                        {
                            int bytesRead = port2.Read(buffer2, 0, Math.Min(buffer2.Length, port2.BytesToRead));
                            if (bytesRead > 0)
                            {
                                port1.Write(buffer2, 0, bytesRead);
                                hadData = true;
                                
                                // 觸發 TX 事件
                                string data = System.Text.Encoding.ASCII.GetString(buffer2, 0, bytesRead);
                                DataTransmitted?.Invoke(data);
                                Console.WriteLine($"[TX] {bytesRead} bytes to {port1.PortName}");
                            }
                        }
                        catch (TimeoutException)
                        {
                            // TX timeout 正常，繼續
                        }
                        catch (Exception ex)
                        {
                            if (isRunning)
                            {
                                Console.WriteLine($"[TX Error] {ex.Message}");
                            }
                        }
                    }

                    // ✅ 動態休眠：有資料時不休眠，沒資料時休眠減少 CPU
                    if (!hadData)
                    {
                        Thread.Sleep(5);
                    }
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        Console.WriteLine($"[Bridge Critical Error] {ex.Message}");
                        Thread.Sleep(100);  // 嚴重錯誤時暫停一下
                    }
                }
            }
        }

        /// <summary>
        /// 發送資料到實體設備（透過 Port1）
        /// </summary>
        public void SendData(string data)
        {
            if (!isRunning || !port1.IsOpen)
                throw new InvalidOperationException("Bridge is not running or port1 is not open");

            try
            {
                port1.Write(data);
                Console.WriteLine($"[Bridge->Device] {data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 發送資料到 Tera Term（透過 Port2）
        /// </summary>
        public void SendToTerminal(string data)
        {
            if (!isRunning || !port2.IsOpen)
                throw new InvalidOperationException("Bridge is not running or port2 is not open");

            try
            {
                port2.Write(data);
                Console.WriteLine($"[Bridge->Terminal] {data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to terminal: {ex.Message}");
                throw;
            }
        }

        private void ClosePortSafely(SerialPort port)
        {
            if (port == null)
                return;
                
            try
            {
                if (port.IsOpen)
                {
                    // 丟棄緩衝區資料
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    
                    // 關閉 Port
                    port.Close();
                    
                    Console.WriteLine($"[Bridge] Closed {port.PortName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bridge] Error closing {port.PortName}: {ex.Message}");
            }
            
            // 等待一下確保系統釋放資源
            Thread.Sleep(100);
        }

        public void Stop()
        {
            isRunning = false;

            // ✅ 修改：加入 timeout 機制
            if (bridgeThread != null && bridgeThread.IsAlive)
            {
                if (!bridgeThread.Join(2000)) // 2 seconds timeout
                {
                    try
                    {
                        bridgeThread.Abort();
                        Console.WriteLine("[Bridge] Thread aborted due to timeout");
                    }
                    catch { }
                }
            }

            // ✅ 修改：使用安全關閉方法
            ClosePortSafely(port1);
            ClosePortSafely(port2);

            Console.WriteLine("Bridge stopped");
        }

        private bool disposed = false;
        public void Dispose()
        {
            // ✅ 修改：避免重複釋放
            if (disposed)
                return;

            Stop();
            
            try
            {
                port1?.Dispose();
                port2?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bridge] Error disposing: {ex.Message}");
            }

            disposed = true;
            Console.WriteLine("[Bridge] Disposed");
        }

        public bool IsRunning => isRunning;
        public string Port1Name => port1?.PortName ?? "N/A";
        public string Port2Name => port2?.PortName ?? "N/A";

        ~ComPortBridge()
        {
            Dispose();
        }
    }
}