# AppLogger 遷移指南

## 快速替換規則

### 1. 將 Console.WriteLine 替換為 AppLogger

#### 一般操作記錄
```csharp
// 舊的
Console.WriteLine($"[PDU] Connected successfully");

// 新的
AppLogger.LogInfo("[PDU] Connected successfully");
```

#### 除錯資訊
```csharp
// 舊的
Console.WriteLine($"[Bridge] Buffer size: 8192");

// 新的
AppLogger.LogDebug("[Bridge] Buffer size: 8192");
```

#### 錯誤訊息
```csharp
// 舊的
Console.WriteLine($"[Error] Failed to connect: {ex.Message}");

// 新的
AppLogger.LogError("Failed to connect", ex);
```

#### 警告訊息
```csharp
// 舊的
Console.WriteLine($"[Warning] Port unavailable");

// 新的
AppLogger.LogWarning("Port unavailable");
```

---

## 2. 特定功能的快速替換

### Terminal 連線
```csharp
// 舊的
Console.WriteLine($"[TeraTerm] Launched from: {teraTermPath}");
Console.WriteLine($"[TeraTerm] Arguments: /C={comPortNumber} /SPEED={baudRate}");

// 新的
AppLogger.LogTerminalConnect(comPort, baudRate, teraTermPath);
```

### Bridge 資料傳輸
```csharp
// 舊的
Console.WriteLine($"[RX] {bytesRead} bytes from {port1.PortName}");

// 新的 (簡單版本)
AppLogger.LogRX(1, bytesRead);

// 新的 (詳細版本)
AppLogger.LogBridgeData("RX", port1.PortName, bytesRead, data);
```

### TTL Script
```csharp
// 舊的
Console.WriteLine($"[TTL] Executing line {lineNumber}: {command}");

// 新的
AppLogger.LogScriptExecution(scriptFile, command, lineNumber);
```

### PDU 控制
```csharp
// 舊的
Console.WriteLine($"[PDU] Port {portNumber} turned {(isOn ? "ON" : "OFF")}");

// 新的
AppLogger.LogPduControl(ipAddress, portNumber, isOn);
```

---

## 3. Form1.cs 修改重點

### 建構函式
```csharp
public Form1()
{
    // 1. 先初始化 Logger（最優先）
    AppLogger.Initialize();
    AppLogger.LogInfo("Application starting...");
    
    // 2. 其他初始化
    InitializeComponent();
    // ...
    
    // 3. 初始化完成
    AppLogger.LogInfo("Form1 initialization completed");
}
```

### Debug Console
```csharp
private void dbgToolStripMenuItem_Click(object sender, EventArgs e)
{
    using (PasswordDialog passwordDialog = new PasswordDialog())
    {
        if (passwordDialog.ShowDialog(this) == DialogResult.OK && passwordDialog.IsAuthenticated)
        {
            try
            {
                // 開啟 Debug Console（自動處理所有輸出）
                AppLogger.AllocateDebugConsole();
                
                MessageBox.Show($"Debug Console activated!\n\nLog file: {AppLogger.GetDebugLogPath()}",
                              "Debug Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Failed to allocate debug console", ex);
            }
        }
        else
        {
            AppLogger.LogWarning("Debug console access denied - incorrect password");
        }
    }
}
```

### TX/RX 事件
```csharp
if (bridge1 != null)
{
    bridge1.DataTransmitted += (data) => 
    {
        // 舊的
        // Console.WriteLine($"[Event] TX triggered for Port 1");
        
        // 新的
        AppLogger.LogTX(1, data?.Length ?? 0);
        FlashTxLed(lblStatusTX1, ledTimer1TX);
    };
    
    bridge1.DataReceived += (data) => 
    {
        // 舊的
        // Console.WriteLine($"[Event] RX triggered for Port 1");
        
        // 新的
        AppLogger.LogRX(1, data?.Length ?? 0);
        FlashRxLed(lblStatusRX1, ledTimer1RX);
    };
}
```

### 應用程式關閉
```csharp
protected override void OnFormClosing(FormClosingEventArgs e)
{
    AppLogger.LogInfo("Application closing...");
    
    // Cleanup
    CleanupPduResources();
    // ...
    
    AppLogger.LogInfo("Application closed successfully");
    
    base.OnFormClosing(e);
}
```

---

## 4. ComPortBridge.cs 修改重點

### 建構函式
```csharp
public ComPortBridge(string portName1, string portName2, string baudRate)
{
    AppLogger.LogDebug($"[Bridge] Initializing: {portName1} <-> {portName2} @ {baudRate}");
    
    // ... 初始化代碼
    
    AppLogger.LogInfo("[Bridge] Initialized successfully");
}
```

### Start() 方法
```csharp
public void Start()
{
    if (isRunning) return;

    AppLogger.LogInfo($"[Bridge] Starting: {port1.PortName} <-> {port2.PortName}");
    AppLogger.LogDebug($"[Bridge] Baud Rate: {port1.BaudRate}, Buffer Size: {BufferSize}");
    
    try
    {
        // ... 啟動代碼
        
        AppLogger.LogInfo("[Bridge] Started successfully");
    }
    catch (Exception ex)
    {
        AppLogger.LogError("[Bridge] Failed to start", ex);
        throw;
    }
}
```

### BridgeData() 方法  
```csharp
private void BridgeData()
{
    while (isRunning)
    {
        try
        {
            // RX
            if (port1.IsOpen && port1.BytesToRead > 0)
            {
                int bytesRead = port1.Read(buffer1, 0, Math.Min(buffer1.Length, port1.BytesToRead));
                if (bytesRead > 0)
                {
                    port2.Write(buffer1, 0, bytesRead);
                    string data = Encoding.ASCII.GetString(buffer1, 0, bytesRead);
                    
                    // 簡單記錄
                    AppLogger.LogInfo($"[Bridge] RX: {bytesRead} bytes from {port1.PortName}");
                    
                    // 詳細記錄（含資料內容）
                    AppLogger.LogBridgeData("RX", port1.PortName, bytesRead, data);
                    
                    DataReceived?.Invoke(data);
                }
            }
            
            // TX
            if (port2.IsOpen && port2.BytesToRead > 0)
            {
                // ... 類似處理
                AppLogger.LogBridgeData("TX", port1.PortName, bytesRead, data);
            }
        }
        catch (Exception ex)
        {
            if (isRunning)
            {
                AppLogger.LogError("[Bridge] Critical error", ex);
            }
        }
    }
}
```

---

## 5. TTLInterpreter.cs 修改重點

### Script 執行
```csharp
public void ExecuteScriptContent(string scriptContent, string fileName = "")
{
    currentScriptFile = string.IsNullOrEmpty(fileName) ? "Inline Script" : fileName;
    
    // 舊的
    // Console.WriteLine("[TTL] Receive buffer cleared before script execution");
    
    // 新的
    AppLogger.LogDebug("[TTL] Receive buffer cleared before script execution");
    
    scriptLines = PreprocessScript(scriptContent);
    currentLine = 0;

    AppLogger.LogInfo($"[TTL] Starting script: {currentScriptFile}");

    while (currentLine < scriptLines.Length)
    {
        string currentCommand = scriptLines[currentLine];
        
        // 記錄每行執行
        AppLogger.LogScriptExecution(currentScriptFile, currentCommand, currentLine + 1);
        
        try
        {
            ExecuteLine(currentCommand);
        }
        catch (Exception ex)
        {
            AppLogger.LogScriptError(currentScriptFile, currentLine + 1, ex.Message);
            throw;
        }
        
        currentLine++;
    }
    
    AppLogger.LogInfo($"[TTL] Script completed: {currentScriptFile}");
}
```

---

## 6. Form1.Pdu.cs 修改重點

### PDU 連線
```csharp
private void btnPduConnect_Click(object sender, EventArgs e)
{
    string pduIp = tbPduIpAddress.Text.Trim();
    
    if (pduController != null)
    {
        pduController.Dispose();
        pduController = null;
        AppLogger.LogInfo("[PDU] Disconnected from PDU");
        return;
    }
    
    try
    {
        pduController = new PduController(pduIp, "iPoMan II");
        AppLogger.LogInfo($"[PDU] Connecting to {pduIp}...");
        
        if (pduController.CheckConnection())
        {
            AppLogger.LogInfo($"[PDU] Connected successfully to {pduIp}");
            // ... UI 更新
        }
        else
        {
            AppLogger.LogWarning($"[PDU] Connection failed to {pduIp}");
        }
    }
    catch (Exception ex)
    {
        AppLogger.LogError("[PDU] Connection error", ex);
    }
}
```

### PDU 控制
```csharp
private void BtnPduPort_CheckedChanged(object sender, EventArgs e)
{
    if (sender is RJToggleButton button)
    {
        int portNumber = (int)button.Tag;
        bool isOn = button.Checked;
        
        AppLogger.LogInfo($"[PDU] User toggled Port {portNumber} to {(isOn ? "ON" : "OFF")}");
        
        try
        {
            bool success = isOn ? pduController.SetPduPortOn(portNumber) 
                                : pduController.SetPduPortOff(portNumber);
            
            if (success)
            {
                AppLogger.LogPduControl(pduController.GetDevice(), portNumber, isOn);
            }
            else
            {
                AppLogger.LogWarning($"[PDU] Failed to control Port {portNumber}");
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"[PDU] Error controlling Port {portNumber}", ex);
        }
    }
}
```

---

## 7. 批量替換建議（使用 VS Code 搜尋/取代）

### 搜尋（正則表達式）
```
Console\.WriteLine\("(.+?)"\);
Console\.WriteLine\(\$"(.+?)"\);
```

### 手動判斷後替換為：
- `AppLogger.LogInfo($1);`  - 一般操作
- `AppLogger.LogDebug($1);` - 詳細除錯
- `AppLogger.LogWarning($1);` - 警告
- `AppLogger.LogError($1);` - 錯誤（需要加 Exception 參數）

---

## 8. 測試檢查清單

完成遷移後，請確認：

- [ ] Form1 建構函式中調用了 `AppLogger.Initialize()`
- [ ] Debug Menu 使用 `AppLogger.AllocateDebugConsole()`
- [ ] 所有 `Console.WriteLine` 已替換為 `AppLogger` 方法
- [ ] 錯誤處理都使用 `AppLogger.LogError(message, ex)`
- [ ] 關鍵操作都有記錄（連線、斷線、資料傳輸、Script 執行、PDU 控制）
- [ ] 編譯無錯誤
- [ ] 執行應用程式，檢查 `DebugLogs` 資料夾是否產生 log 檔案
- [ ] 使用密碼開啟 Debug Console，確認即時訊息顯示正常
- [ ] 執行各種操作，確認 log 記錄完整

---

## 9. Log 檔案位置

### 發佈後的資料夾結構
```
MyTeraTerm V1.0.0/
├── MyTeraTerm.exe
├── Application/
│   └── TeraTerm/
├── Scripts/
├── DebugLogs/              ← Log 檔案存放位置
│   ├── Debug_20260209_143022.log
│   ├── Debug_20260209_150315.log
│   └── (自動保留 30 天)
└── README.txt
```

### 使用者回報問題時
請使用者提供 `DebugLogs` 資料夾中最新的 log 檔案即可。

---

## 10. 常見問題

### Q: 一般使用者會產生 log 嗎？
A: 不會！只有開發者透過密碼開啟 Debug Console 後才會產生 log。

### Q: Log 檔案何時建立？
A: 第一次調用 `AppLogger.Initialize()` 或 `AppLogger.AllocateDebugConsole()` 時。

### Q: Log 檔案會自動清理嗎？
A: 會，保留最近 30 天的 log，舊的會自動刪除。

### Q: 如何修改 log 保留天數？
A: 修改 `Logger.cs` 中的 `CleanupOldLogs("DebugLogs", "Debug_*.log", 30)` 最後的數字。

### Q: Debug Console 和 Log 檔案的關係？
A: 兩者獨立。Log 永遠會記錄到檔案，Debug Console 只是即時顯示視窗。
