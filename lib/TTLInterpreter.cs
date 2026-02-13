using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using COMPortBrdigeLib;

namespace TTLInterpreterLib
{
    public class TTLInterpreter : IDisposable
    {
        private ComPortBridge bridge;
        private StreamWriter logWriter;
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private int timeout = 30000;
        private string lastReceivedText = "";
        private int result = 0;
        
        private string[] scriptLines;
        private int currentLine = 0;
        private StringBuilder receiveBuffer = new StringBuilder();
        private bool disposed = false;
        private string currentScriptFile = "";
        private volatile bool isCancelled = false;

        // 新增：狀態更新事件
        public event Action<string, int, string> StatusChanged;
        
        // 新增：全體終端操作事件
        public event Action<string, bool> SendAllRequested;
        public event Func<string, bool> WaitAllRequested;
        
        // 新增：計數事件
        public event Action<string> CountKeysRequested;
        
        // 用於 Waitall 同步
        private ManualResetEvent waitAllEvent = new ManualResetEvent(false);
        private bool waitAllResult = false;

        public TTLInterpreter(ComPortBridge comPortBridge)
        {
            if (comPortBridge == null || !comPortBridge.IsRunning)
                throw new ArgumentException("ComPortBridge must be running");

            bridge = comPortBridge;
            
            // Subscribe to bridge data events
            bridge.DataReceived += Bridge_DataReceived;
            Console.WriteLine("[TTL] Subscribed to bridge DataReceived event");
        }

        private void Bridge_DataReceived(string data)
        {
            lock (receiveBuffer)
            {
                receiveBuffer.Append(data);
                lastReceivedText = receiveBuffer.ToString();
                Console.WriteLine($"[TTL DataRX] Received {data.Length} chars, buffer now: {receiveBuffer.Length} chars");
            }
        }

        #region Script Execution

        public void ExecuteScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Script file not found: {scriptPath}");

            currentScriptFile = Path.GetFileName(scriptPath);
            string content = File.ReadAllText(scriptPath);
            ExecuteScriptContent(content);
        }

        public void ExecuteScriptContent(string scriptContent, string fileName = "")
        {
            currentScriptFile = string.IsNullOrEmpty(fileName) ? "Inline Script" : fileName;
            isCancelled = false;
            
            // 清空接收緩衝區
            receiveBuffer.Clear();
            Console.WriteLine("[TTL] Receive buffer cleared before script execution");
            
            scriptLines = PreprocessScript(scriptContent);
            currentLine = 0;

            while (currentLine < scriptLines.Length)
            {
                // 檢查是否被取消
                if (isCancelled)
                {
                    Console.WriteLine("[TTL] Script execution cancelled");
                    throw new OperationCanceledException("Script execution was cancelled by user");
                }
                
                // 更新狀態：檔案名稱, 行數, 當前指令
                string currentCommand = scriptLines[currentLine];
                StatusChanged?.Invoke(currentScriptFile, currentLine + 1, currentCommand);
                
                ExecuteLine(scriptLines[currentLine]);
                currentLine++;
            }

            // 完成時更新狀態
            StatusChanged?.Invoke(currentScriptFile, scriptLines.Length, "Completed");
            
            Cleanup();
        }

        private string[] PreprocessScript(string content)
        {
            var lines = new List<string>();

            foreach (string line in content.Split('\n'))
            {
                string trimmed = line.Trim();

                int commentIndex = trimmed.IndexOf(';');
                if (commentIndex >= 0)
                    trimmed = trimmed.Substring(0, commentIndex).Trim();

                if (!string.IsNullOrWhiteSpace(trimmed))
                    lines.Add(trimmed);
            }

            return lines.ToArray();
        }

        #endregion

        #region Line Execution

        private void ExecuteLine(string line)
        {
            string originalLine = line;
            bool isAssignment = originalLine.Contains("=") && 
                            !originalLine.Contains("==") &&
                            !originalLine.Contains("!=") &&
                            !originalLine.Contains(">=") &&
                            !originalLine.Contains("<=") &&
                            !originalLine.StartsWith("if ") &&
                            !originalLine.StartsWith("elseif ");

            string command = GetCommand(line).ToLower();
            if (!isAssignment && command != "pductrl" && command != "while" && command != "if")
            {
                line = ReplaceVariables(line);
            }

            if (line.StartsWith(":"))
                return;

            string args = GetArgs(command == "pductrl" ? originalLine : line);

            switch (command.ToLower())
            {
                case "send":
                    Send(args, false);
                    break;
                case "sendln":
                    Send(args, true);
                    break;
                case "sendall":
                    Sendall(args, false);
                    break;
                case "pause":
                    Pause(args);
                    break;
                case "wait":
                    Wait(args);
                    break;
                case "timeout":
                    SetTimeout(args);
                    break;
                case "flushrecv":
                    FlushReceive();
                    break;
                case "messagebox":
                    MessageBox(args);
                    break;
                case "while":
                    ExecuteWhile(originalLine);
                    break;
                case "endwhile":
                    // endwhile 由 ExecuteWhile 處理，這裡不做任何事
                    break;
                case "if":
                    ExecuteIf(originalLine);
                    break;
                case "elseif":
                case "else":
                case "endif":
                    // 這些由 ExecuteIf 處理，單獨出現視為錯誤
                    break;
                case "":
                    if (isAssignment)
                        ExecuteAssignment(originalLine);
                    break;
                // Below command not support Original TeraTerm
                case "waitall":
                    Waitall(args);
                    break;
                case "sendlnall":
                    Sendall(args, true);
                    break;
                case "pductrl":
                    ExecutePduCtrl(args);
                    break;
                case "pduconnect":
                    ExecutePduConnect(args);
                    break;
                case "count":
                    ExecuteCount(args);
                    break;
                case "countkeys":
                    ExecuteCountKeys(args);
                    break;
                default:
                    if (isAssignment)
                        ExecuteAssignment(originalLine);
                    else
                        Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }
        #endregion

        #region Device Control - PduCtrl
        private void ExecutePduConnect(string args)
        {
            try
            {
                // 替換變數
                args = ReplaceVariables(args);

                Console.WriteLine($"[TTL PduConnect] Raw args: '{args}'");

                // 解析參數：支援 "pduconnect 2 192.168.1.21" 或 "pduconnect 2 '192.168.1.21'"
                var match = Regex.Match(args, @"^\s*(\d+)\s+['""]?([0-9.]+)['""]?\s*$");
                
                if (!match.Success)
                {
                    // 嘗試空格分隔的格式
                    var parts = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (parts.Length != 2)
                    {
                        Console.WriteLine($"[TTL PduConnect] Error: Invalid syntax. Expected: pduconnect [device] [ip]");
                        Console.WriteLine($"[TTL PduConnect] Example: pduconnect 2 192.168.1.21");
                        Console.WriteLine($"[TTL PduConnect] Received args: '{args}'");
                        result = 0;
                        return;
                    }

                    // Parse device
                    int device = ParseIntDirect(parts[0].Trim());
                    if (device != 1 && device != 2)
                    {
                        Console.WriteLine($"[TTL PduConnect] Error: Invalid device '{parts[0]}' -> {device}. Must be 1 (iPoMan I) or 2 (iPoMan II)");
                        result = 0;
                        return;
                    }

                    // Parse IP
                    string ip = parts[1].Trim().Trim('\'', '"');
                    
                    // 驗證 IP 格式
                    if (!IsValidIPAddress(ip))
                    {
                        Console.WriteLine($"[TTL PduConnect] Error: Invalid IP address '{ip}'");
                        result = 0;
                        return;
                    }

                    string deviceName = device == 1 ? "iPoMan I" : "iPoMan II";
                    Console.WriteLine($"[TTL PduConnect] Connecting to Device {device} ({deviceName}) at {ip}");
                    
                    // 觸發 PDU 連線事件
                    OnPduConnectRequested(device, ip);

                    // 等待連線建立
                    Thread.Sleep(1000);

                    result = 1; // 成功
                    Console.WriteLine($"[TTL PduConnect] Connection request sent successfully");
                    
                    if (logWriter != null)
                        logWriter.WriteLine($"[PduConnect] Device={deviceName}, IP={ip}");
                }
                else
                {
                    // 使用正則表達式解析
                    int device = int.Parse(match.Groups[1].Value);
                    string ip = match.Groups[2].Value;

                    if (device != 1 && device != 2)
                    {
                        Console.WriteLine($"[TTL PduConnect] Error: Invalid device {device}. Must be 1 or 2");
                        result = 0;
                        return;
                    }

                    if (!IsValidIPAddress(ip))
                    {
                        Console.WriteLine($"[TTL PduConnect] Error: Invalid IP address '{ip}'");
                        result = 0;
                        return;
                    }

                    string deviceName = device == 1 ? "iPoMan I" : "iPoMan II";
                    Console.WriteLine($"[TTL PduConnect] Connecting to Device {device} ({deviceName}) at {ip}");
                    
                    OnPduConnectRequested(device, ip);
                    Thread.Sleep(1000);

                    result = 1;
                    Console.WriteLine($"[TTL PduConnect] Connection request sent successfully");
                    
                    if (logWriter != null)
                        logWriter.WriteLine($"[PduConnect] Device={deviceName}, IP={ip}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTL PduConnect] Error: {ex.Message}");
                Console.WriteLine($"[TTL PduConnect] Stack trace: {ex.StackTrace}");
                result = 0;
                
                if (logWriter != null)
                    logWriter.WriteLine($"[PduConnect] Error: {ex.Message}");
            }
        }
        private bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            var parts = ip.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num))
                    return false;
                
                if (num < 0 || num > 255)
                    return false;
            }

            return true;
        }
        public event Action<int, string> PduConnectRequested;
        protected virtual void OnPduConnectRequested(int device, string ip)
        {
            PduConnectRequested?.Invoke(device, ip);
        }
        private void ExecutePduCtrl(string args)
        {
            try
            {
                Console.WriteLine($"[TTL PduCtrl] Original args: '{args}'");
                
                // 替換變數
                args = ReplaceVariables(args);
                
                Console.WriteLine($"[TTL PduCtrl] After variable replacement: '{args}'");

                // Parse arguments
                var parts = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                Console.WriteLine($"[TTL PduCtrl] Parsed parts: [{string.Join(", ", parts)}]");
                
                if (parts.Length !=3)
                {
                    Console.WriteLine($"[TTL PduCtrl] Error: Invalid syntax. Expected: pductrl [device] [port] [action]");
                    Console.WriteLine($"[TTL PduCtrl] Received {parts.Length} arguments: '{args}'");
                    result = 0;
                    return;
                }

                // Parse device
                int device = ParseIntDirect(parts[0].Trim());
                Console.WriteLine($"[TTL PduCtrl] Device: '{parts[0]}' -> {device}");
                
                if (device != 1 && device != 2)
                {
                    Console.WriteLine($"[TTL PduCtrl] Error: Invalid device '{parts[0]}' -> {device}. Must be 1 (iPoMan I) or 2 (iPoMan II)");
                    result = 0;
                    return;
                }

                // Parse port
                int port = ParseIntDirect(parts[1].Trim());
                Console.WriteLine($"[TTL PduCtrl] Port: '{parts[1]}' -> {port}");
                
                if (port < 1 || port > 12)
                {
                    Console.WriteLine($"[TTL PduCtrl] Error: Invalid port '{parts[1]}' -> {port}. Must be 1-12");
                    result = 0;
                    return;
                }

                // Parse Action
                int action = ParseIntDirect(parts[2].Trim());
                Console.WriteLine($"[TTL PduCtrl] Action: '{parts[2]}' -> {action}");
                
                if (action != 0 && action != 1)
                {
                    Console.WriteLine($"[TTL PduCtrl] Error: Invalid action value: {action}");
                    result = 0;
                    return;
                }

                string deviceName = device == 1 ? "Device1" : "Device2";
                string actionName = action == 1 ? "ON" : "OFF";
                Console.WriteLine($"[TTL PduCtrl] Command: Device={deviceName}, Port={port}, Action={actionName}");
        
                // Trigger event needs to be handled by the Form1
                Thread.Sleep(500);
                OnPduControlRequested(device, port, action);

                // 等待一下讓 PDU 執行命令
                Thread.Sleep(500);

                result = 1; // 成功
                Console.WriteLine($"[TTL PduCtrl] Command sent successfully");
                if (logWriter != null)
                    logWriter.WriteLine($"[PduCtrl] Device={deviceName}, Port={port}, Action={actionName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTL PduCtrl] Error: {ex.Message}");
                Console.WriteLine($"[TTL PduCtrl] Stack trace: {ex.StackTrace}");
                result = 0;
                
                if (logWriter != null)
                    logWriter.WriteLine($"[PduCtrl] Error: {ex.Message}");
            }
        }
        public event Action<int, int, int> PduControlRequested;
        protected virtual void OnPduControlRequested(int device, int port, int action)
        {
            PduControlRequested?.Invoke(device, port, action);
        }
        
        /// <summary>
        /// Execute countkeys command - increment counter for user-specified keyword
        /// </summary>
        /// <summary>
        /// Execute count command - immediately increment counter
        /// </summary>
        private void ExecuteCount(string args)
        {
            try
            {
                // Remove quotes and whitespace
                string keyword = args.Trim().Trim('"', '\'');
                
                // Replace variables
                keyword = ReplaceVariables(keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Console.WriteLine($"[TTL Count] Error: Keyword cannot be empty");
                    result = 0;
                    return;
                }
                
                Console.WriteLine($"[TTL Count] Immediately counting keyword: '{keyword}'");
                
                // Trigger count event
                CountKeysRequested?.Invoke(keyword);
                
                result = 1; // Success
                
                if (logWriter != null)
                    logWriter.WriteLine($"[Count] Keyword: '{keyword}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTL Count] Error: {ex.Message}");
                result = 0;
                
                if (logWriter != null)
                    logWriter.WriteLine($"[Count] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute countkeys command - wait for keyword to appear, then increment counter
        /// </summary>
        private void ExecuteCountKeys(string args)
        {
            try
            {
                // Remove quotes and whitespace
                string keyword = args.Trim().Trim('"', '\'');
                
                // Replace variables
                keyword = ReplaceVariables(keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Console.WriteLine($"[TTL CountKeys] Error: Keyword cannot be empty");
                    result = 0;
                    return;
                }
                
                Console.WriteLine($"[TTL CountKeys] Waiting for keyword: '{keyword}'");
                
                if (logWriter != null)
                    logWriter.WriteLine($"[CountKeys] Waiting for keyword: '{keyword}'");
                
                int checkInterval = 100; // Check every 100ms
                int elapsedTime = 0;
                
                while (true)
                {
                    // Check if cancelled
                    if (isCancelled)
                    {
                        Console.WriteLine("[TTL CountKeys] Cancelled");
                        throw new OperationCanceledException("CountKeys operation was cancelled by user");
                    }
                    
                    // Check if keyword exists in buffer
                    lock (receiveBuffer)
                    {
                        string bufferContent = receiveBuffer.ToString();
                        if (bufferContent.Contains(keyword))
                        {
                            Console.WriteLine($"[TTL CountKeys] Keyword found in buffer after {elapsedTime}ms: '{keyword}'");
                            
                            // Trigger count event
                            CountKeysRequested?.Invoke(keyword);
                            
                            result = 1; // Success
                            
                            if (logWriter != null)
                                logWriter.WriteLine($"[CountKeys] Keyword found after {elapsedTime}ms");
                            
                            return;
                        }
                    }
                    
                    // Wait before next check
                    Thread.Sleep(checkInterval);
                    elapsedTime += checkInterval;
                    
                    // Report progress every 10 seconds
                    if (elapsedTime % 10000 == 0)
                    {
                        Console.WriteLine($"[TTL CountKeys] Still waiting for '{keyword}'... ({elapsedTime / 1000}s elapsed)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTL CountKeys] Error: {ex.Message}");
                result = 0;
                
                if (logWriter != null)
                    logWriter.WriteLine($"[CountKeys] Error: {ex.Message}");
            }
        }
        #endregion


        #region Control Flow - If Statement

        /// <summary>
        /// Execute if/elseif/else/endif block
        /// </summary>
        private void ExecuteIf(string line)
        {
            // 解析 if 條件
            var match = Regex.Match(line, @"if\s+(.+?)\s+then", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                // 沒有 then 關鍵字，嘗試不帶 then 的格式
                match = Regex.Match(line, @"if\s+(.+)", RegexOptions.IgnoreCase);
                if (!match.Success)
                    throw new Exception("Invalid if statement");
            }

            string condition = match.Groups[1].Value.Trim();
            
            // 找到對應的 endif
            int ifStartLine = currentLine;
            int endifLine = FindEndIf(ifStartLine);
            
            if (endifLine < 0)
                throw new Exception("if statement without matching endif");

            Console.WriteLine($"[TTL If] Evaluating condition: {condition}");

            // 找出所有的 elseif 和 else
            List<int> elseifLines = new List<int>();
            int elseLine = -1;
            
            for (int i = ifStartLine + 1; i < endifLine; i++)
            {
                string testLine = scriptLines[i].Trim().ToLower();
                if (testLine.StartsWith("elseif"))
                    elseifLines.Add(i);
                else if (testLine.StartsWith("else") && !testLine.StartsWith("elseif"))
                {
                    elseLine = i;
                    break; // else 之後不會有 elseif
                }
            }

            // 評估條件並執行對應的區塊
            bool conditionMet = false;
            int executeStartLine = -1;
            int executeEndLine = -1;

            // 檢查主 if 條件
            if (EvaluateCondition(condition))
            {
                conditionMet = true;
                executeStartLine = ifStartLine + 1;
                executeEndLine = elseifLines.Count > 0 ? elseifLines[0] : (elseLine >= 0 ? elseLine : endifLine);
                Console.WriteLine($"[TTL If] Main condition is TRUE");
            }
            else
            {
                Console.WriteLine($"[TTL If] Main condition is FALSE");
                
                // 檢查 elseif 條件
                for (int i = 0; i < elseifLines.Count && !conditionMet; i++)
                {
                    int elseifLineIndex = elseifLines[i];
                    string elseifLine = scriptLines[elseifLineIndex];
                    
                    var elseifMatch = Regex.Match(elseifLine, @"elseif\s+(.+?)\s+then", RegexOptions.IgnoreCase);
                    if (!elseifMatch.Success)
                    {
                        elseifMatch = Regex.Match(elseifLine, @"elseif\s+(.+)", RegexOptions.IgnoreCase);
                    }
                    
                    if (elseifMatch.Success)
                    {
                        string elseifCondition = elseifMatch.Groups[1].Value.Trim();
                        Console.WriteLine($"[TTL If] Evaluating elseif condition: {elseifCondition}");
                        
                        if (EvaluateCondition(elseifCondition))
                        {
                            conditionMet = true;
                            executeStartLine = elseifLineIndex + 1;
                            executeEndLine = (i + 1 < elseifLines.Count) ? elseifLines[i + 1] : (elseLine >= 0 ? elseLine : endifLine);
                            Console.WriteLine($"[TTL If] Elseif condition is TRUE");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"[TTL If] Elseif condition is FALSE");
                        }
                    }
                }
                
                // 如果所有條件都不滿足，執行 else 區塊
                if (!conditionMet && elseLine >= 0)
                {
                    conditionMet = true;
                    executeStartLine = elseLine + 1;
                    executeEndLine = endifLine;
                    Console.WriteLine($"[TTL If] Executing else block");
                }
            }

            // 執行對應的區塊
            if (conditionMet && executeStartLine >= 0 && executeEndLine > executeStartLine)
            {
                int savedLine = currentLine;
                currentLine = executeStartLine;
                
                while (currentLine < executeEndLine)
                {
                    // 更新狀態
                    string currentCommand = scriptLines[currentLine];
                    StatusChanged?.Invoke(currentScriptFile, currentLine + 1, currentCommand);
                    
                    ExecuteLine(scriptLines[currentLine]);
                    currentLine++;
                }
            }
            else
            {
                Console.WriteLine($"[TTL If] No condition met, skipping all blocks");
            }
            
            // 跳到 endif 之後
            currentLine = endifLine;
        }

        /// <summary>
        /// Find matching endif for current if
        /// </summary>
        private int FindEndIf(int ifLineIndex)
        {
            int depth = 0;
            
            for (int i = ifLineIndex; i < scriptLines.Length; i++)
            {
                string line = scriptLines[i].Trim().ToLower();
                
                if (line.StartsWith("if "))
                    depth++;
                else if (line.StartsWith("endif"))
                {
                    depth--;
                    if (depth == 0)
                        return i;
                }
            }
            
            return -1; // endif not found
        }

        #endregion


        #region Control Flow - While Loop

        /// <summary>
        /// Execute while loop
        /// </summary>
        private void ExecuteWhile(string line)
        {
            // 解析 while 條件
            var match = Regex.Match(line, @"while\s+(.+)", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new Exception("Invalid while statement");

            // Modified: Store original condition for re-evaluation
            string originalCondition = match.Groups[1].Value.Trim();
            
            // 找到對應的 endwhile
            int whileStartLine = currentLine;
            int endwhileLine = FindEndWhile(whileStartLine);
            
            if (endwhileLine < 0)
                throw new Exception("while statement without matching endwhile");

            Console.WriteLine($"[TTL While] Starting loop at line {whileStartLine + 1}");
            Console.WriteLine($"[TTL While] Original condition: {originalCondition}");  // ✅ 加入除錯

            CleanupInvalidVariables();
            
            // Modified: Re-evaluate condition each iteration
            int loopCount = 0;
            while (true)
            {
                // 檢查是否被取消
                if (isCancelled)
                {
                    Console.WriteLine("[TTL] While loop cancelled");
                    throw new OperationCanceledException("While loop was cancelled by user");
                }
                
                // ✅ 加入除錯：顯示當前變數值
                if (variables.ContainsKey("idx"))
                {
                    Console.WriteLine($"[TTL While] Current idx value: {variables["idx"]}");
                }
                
                // Every iteration, replace variables in the original condition
                string evaluatedCondition = ReplaceVariables(originalCondition);
                
                // ✅ 加入除錯：確認替換結果
                Console.WriteLine($"[TTL While] After ReplaceVariables: '{originalCondition}' -> '{evaluatedCondition}'");
                
                if (!EvaluateCondition(evaluatedCondition))
                    break;
                
                loopCount++;
                Console.WriteLine($"[TTL While] Loop iteration {loopCount}");
                
                // Execute loop body
                int savedLine = currentLine;
                currentLine = whileStartLine + 1; // 從 while 的下一行開始
                
                while (currentLine < endwhileLine)
                {
                    // 更新狀態
                    string currentCommand = scriptLines[currentLine];
                    StatusChanged?.Invoke(currentScriptFile, currentLine + 1, currentCommand);
                    
                    ExecuteLine(scriptLines[currentLine]);
                    currentLine++;
                }
                
                // Modified: Update status display before re-evaluating condition
                StatusChanged?.Invoke(currentScriptFile, whileStartLine + 1, $"while {originalCondition} (checking)");
            }
            
            Console.WriteLine($"[TTL While] Loop ended after {loopCount} iterations");
            
            // To ensure we skip the endwhile line after finishing the loop
            currentLine = endwhileLine;
        }
        private void CleanupInvalidVariables()
        {
            var invalidKeys = new List<string>();
            
            foreach (var key in variables.Keys)
            {
                if (int.TryParse(key, out _))
                {
                    invalidKeys.Add(key);
                }
            }
            
            foreach (var key in invalidKeys)
            {
                Console.WriteLine($"[TTL] Removing invalid variable: '{key}' = {variables[key]}");
                variables.Remove(key);
            }
        }

        /// <summary>
        /// Find matching endwhile for current while
        /// </summary>
        private int FindEndWhile(int whileLineIndex)
        {
            int depth = 0;
            
            for (int i = whileLineIndex; i < scriptLines.Length; i++)
            {
                string line = scriptLines[i].Trim().ToLower();
                
                if (line.StartsWith("while"))
                    depth++;
                else if (line.StartsWith("endwhile"))
                {
                    depth--;
                    if (depth == 0)
                        return i;
                }
            }
            
            return -1; // endwhile not found
        }

        #endregion

        #region Control Flow - Condition Evaluation

        /// <summary>
        /// Evaluate a condition expression
        /// </summary>
        private bool EvaluateCondition(string condition)
        {
            condition = condition.Trim();
            Console.WriteLine($"[TTL Eval] Evaluating: {condition}");
            
            // 處理比較運算子 (按照優先順序)
            if (condition.Contains(">="))
            {
                var parts = condition.Split(new[] { ">=" }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0]);
                    int right = ParseIntDirect(parts[1]);
                    bool result = left >= right;
                    Console.WriteLine($"[TTL Eval] {left} >= {right} = {result}");
                    return result;
                }
            }
            else if (condition.Contains("<="))
            {
                var parts = condition.Split(new[] { "<=" }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0]);
                    int right = ParseIntDirect(parts[1]);
                    bool result = left <= right;
                    Console.WriteLine($"[TTL Eval] {left} <= {right} = {result}");
                    return result;
                }
            }
            else if (condition.Contains("=="))
            {
                var parts = condition.Split(new[] { "==" }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    string left = parts[0].Trim().Trim('\'', '"');
                    string right = parts[1].Trim().Trim('\'', '"');
                    bool result = left == right;
                    Console.WriteLine($"[TTL Eval] '{left}' == '{right}' = {result}");
                    return result;
                }
            }
            else if (condition.Contains("!="))
            {
                var parts = condition.Split(new[] { "!=" }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    string left = parts[0].Trim().Trim('\'', '"');
                    string right = parts[1].Trim().Trim('\'', '"');
                    bool result = left != right;
                    Console.WriteLine($"[TTL Eval] '{left}' != '{right}' = {result}");
                    return result;
                }
            }
            else if (condition.Contains(">"))
            {
                var parts = condition.Split('>');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0]);
                    int right = ParseIntDirect(parts[1]);
                    bool result = left > right;
                    Console.WriteLine($"[TTL Eval] {left} > {right} = {result}");
                    return result;
                }
            }
            else if (condition.Contains("<"))
            {
                var parts = condition.Split('<');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0]);
                    int right = ParseIntDirect(parts[1]);
                    bool result = left < right;
                    Console.WriteLine($"[TTL Eval] {left} < {right} = {result}");
                    return result;
                }
            }
            else if (condition.Contains("=") && !condition.Contains("=="))
            {
                var parts = condition.Split('=');
                if (parts.Length == 2)
                {
                    // 先嘗試數字比較
                    if (int.TryParse(parts[0].Trim(), out int leftInt) && int.TryParse(parts[1].Trim(), out int rightInt))
                    {
                        bool result = leftInt == rightInt;
                        Console.WriteLine($"[TTL Eval] {leftInt} = {rightInt} = {result}");
                        return result;
                    }
                    // 字串比較
                    else
                    {
                        string left = parts[0].Trim().Trim('\'', '"');
                        string right = parts[1].Trim().Trim('\'', '"');
                        bool result = left == right;
                        Console.WriteLine($"[TTL Eval] '{left}' = '{right}' = {result}");
                        return result;
                    }
                }
            }
            
            // 如果沒有運算子，檢查是否為非零值
            int value = ParseIntDirect(condition);
            bool boolResult = value != 0;
            Console.WriteLine($"[TTL Eval] {value} != 0 = {boolResult}");
            return boolResult;
        }

        #endregion

        #region Variable Assignment
        private void ExecuteAssignment(string line)
        {
            int equalIndex = line.IndexOf('=');
            if (equalIndex < 0)
                return;

            string varName = line.Substring(0, equalIndex).Trim();
            string valueExpr = line.Substring(equalIndex + 1).Trim();
            
            Console.WriteLine($"[TTL Assignment] Processing: varName='{varName}', valueExpr='{valueExpr}'");

            // Added: Check for invalid variable names
            if (int.TryParse(varName, out _))
            {
                Console.WriteLine($"[TTL] Error: Invalid variable name '{varName}' (cannot be a number)");
                return;
            }

            // Added: Check for variable name format (only letters, numbers, underscores, and cannot start with a number)
            if (!Regex.IsMatch(varName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                Console.WriteLine($"[TTL] Error: Invalid variable name '{varName}'");
                return;
            }

            // Replace variables in value expression
            valueExpr = ReplaceVariables(valueExpr);

            // 計算表達式
            object value = EvaluateExpression(valueExpr);

            variables[varName] = value;
            
            Console.WriteLine($"[TTL Assignment] Stored: variables['{varName}'] = {value}");
            Console.WriteLine($"[TTL Assignment] Current variables: {string.Join(", ", variables.Select(kv => $"{kv.Key}={kv.Value}"))}");
            
            // Added: Improved log output
            if (valueExpr.Contains("+") || valueExpr.Contains("-") || valueExpr.Contains("*") || valueExpr.Contains("/"))
            {
                Console.WriteLine($"[TTL] Variable '{varName}' = {value} (from: {valueExpr})");
            }
            else
            {
                Console.WriteLine($"[TTL] Variable '{varName}' = {value}");
            }
        }
        private object EvaluateExpression(string expr)
        {
            expr = expr.Trim();

            // Added: Improved operator handling
            // 處理加法
            if (expr.Contains("+"))
            {
                var parts = expr.Split('+');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0].Trim());
                    int right = ParseIntDirect(parts[1].Trim());
                    return left + right;
                }
            }

            // 處理減法
            if (expr.Contains("-") && !expr.StartsWith("-")) // 排除負數
            {
                var parts = expr.Split('-');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0].Trim());
                    int right = ParseIntDirect(parts[1].Trim());
                    return left - right;
                }
            }

            // 處理乘法
            if (expr.Contains("*"))
            {
                var parts = expr.Split('*');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0].Trim());
                    int right = ParseIntDirect(parts[1].Trim());
                    return left * right;
                }
            }

            // 處理除法
            if (expr.Contains("/"))
            {
                var parts = expr.Split('/');
                if (parts.Length == 2)
                {
                    int left = ParseIntDirect(parts[0].Trim());
                    int right = ParseIntDirect(parts[1].Trim());
                    if (right != 0)
                        return left / right;
                }
            }

            // 處理字串
            if (expr.StartsWith("\"") && expr.EndsWith("\""))
            {
                return expr.Substring(1, expr.Length - 2);
            }

            // 處理數字
            if (int.TryParse(expr, out int intValue))
            {
                return intValue;
            }

            // 處理變數（在 ReplaceVariables 後應該已經是值了）
            return expr;
        }
        #endregion

        #region Basic Commands

        /// <summary>
        /// Send text through bridge
        /// </summary>
        private void Send(string args, bool newline)
        {
            string text = args.Trim('\'', '"');
            string dataToSend = text + (newline ? "\r\n" : "");
            
            bridge.SendData(dataToSend);
            Console.WriteLine($"[TTL Send] {text}");

            if (logWriter != null)
                logWriter.WriteLine($">> {text}");
        }
        
        /// <summary>
        /// Send text to all connected terminals
        /// </summary>
        private void Sendall(string args, bool newline)
        {
            string text = args.Trim('\'', '"');
            
            if (SendAllRequested == null)
            {
                Console.WriteLine($"[TTL SendAll] Error: No handler for SendAllRequested event");
                result = 0;
                return;
            }
            
            Console.WriteLine($"[TTL SendAll] Sending to all terminals: {text}");
            SendAllRequested?.Invoke(text, newline);
            
            if (logWriter != null)
                logWriter.WriteLine($">> [All] {text}");
            
            result = 1;
        }

        /// <summary>
        /// Wait for expected text in all connected terminals
        /// </summary>
        private void Waitall(string text)
        {
            // Remove quotes if present
            text = text.Trim().Trim('"', '\'');
            
            // Replace variables
            text = ReplaceVariables(text);
            
            if (WaitAllRequested == null)
            {
                Console.WriteLine($"[TTL WaitAll] Error: No handler for WaitAllRequested event");
                result = 0;
                return;
            }
            
            Console.WriteLine($"[TTL WaitAll] Waiting for text in all terminals: '{text}'");
            
            if (logWriter != null)
                logWriter.WriteLine($"[WaitAll] Waiting for: '{text}'");
            
            int checkInterval = 100; // Check every 100ms
            int elapsedTime = 0;
            
            while (true)
            {
                // 檢查是否被取消
                if (isCancelled)
                {
                    Console.WriteLine("[TTL] WaitAll cancelled");
                    throw new OperationCanceledException("WaitAll operation was cancelled by user");
                }
                
                // 詢問 Form1 是否所有終端都收到了
                bool allReceived = WaitAllRequested?.Invoke(text) ?? false;
                
                Console.WriteLine($"[TTL WaitAll] Check result: allReceived={allReceived}, elapsed={elapsedTime}ms");
                
                if (allReceived)
                {
                    Console.WriteLine($"[TTL WaitAll] Text found in all terminals after {elapsedTime}ms: '{text}'");
                    
                    if (logWriter != null)
                        logWriter.WriteLine($"[WaitAll] Text found in all terminals after {elapsedTime}ms");
                    
                    result = 1; // Success
                    return;
                }
                
                // Wait before next check
                Thread.Sleep(checkInterval);
                elapsedTime += checkInterval;
                
                // Report progress every 10 seconds
                if (elapsedTime % 10000 == 0)
                {
                    Console.WriteLine($"[TTL WaitAll] Still waiting for '{text}' in all terminals... ({elapsedTime / 1000}s elapsed)");
                }
            }
        }
        
        /// <summary>
        /// Wait for expected text in receive buffer
        /// </summary>
        private void Wait(string text)
        {
            // Remove quotes if present
            text = text.Trim().Trim('"', '\'');
            
            // Replace variables
            text = ReplaceVariables(text);
            
            Console.WriteLine($"[TTL] Waiting for text: '{text}' (no timeout)");
            
            if (logWriter != null)
                logWriter.WriteLine($"[Wait] Waiting for: '{text}'");

            int checkInterval = 100; // Check every 100ms
            int elapsedTime = 0;

            while (true)
            {
                // 檢查是否被取消
                if (isCancelled)
                {
                    Console.WriteLine("[TTL] Wait cancelled");
                    throw new OperationCanceledException("Wait operation was cancelled by user");
                }
                
                // Using lock to ensure thread safety when accessing receiveBuffer
                lock (receiveBuffer)
                {
                    // Check if text is in receive buffer
                    string currentBuffer = receiveBuffer.ToString();
                    
                    if (currentBuffer.Contains(text))
                    {
                        Console.WriteLine($"[TTL] Text found after {elapsedTime}ms: '{text}'");
                        
                        if (logWriter != null)
                            logWriter.WriteLine($"[Wait] Text found after {elapsedTime}ms");
                        
                        result = 1; // Success
                        lastReceivedText = currentBuffer;
                        
                        // Using lock to ensure thread safety when accessing receiveBuffer
                        int foundIndex = currentBuffer.IndexOf(text);
                        if (foundIndex >= 0)
                        {
                            int removeLength = foundIndex + text.Length;
                            
                            // Using lock to ensure thread safety when accessing receiveBuffer
                            if (removeLength <= receiveBuffer.Length)
                            {
                                string removedText = currentBuffer.Substring(0, removeLength);
                                string remainingText = currentBuffer.Substring(removeLength);
                                
                                receiveBuffer.Clear();
                                receiveBuffer.Append(remainingText);
                                
                                Console.WriteLine($"[TTL] Removed {removeLength} chars from buffer");
                                Console.WriteLine($"[TTL] Remaining in buffer: {remainingText.Length} chars");
                            }
                            else
                            {
                                // This should not happen, but just in case, clear the buffer to avoid infinite loop
                                receiveBuffer.Clear();
                                Console.WriteLine($"[TTL] Buffer cleared (length mismatch)");
                            }
                        }
                        
                        return;
                    }
                }

                // Wait before next check
                Thread.Sleep(checkInterval);
                elapsedTime += checkInterval;

                // Report progress every 10 seconds
                if (elapsedTime % 10000 == 0)
                {
                    Console.WriteLine($"[TTL] Still waiting for '{text}'... ({elapsedTime / 1000}s elapsed)");
                }
            }
        }

        /// <summary>
        /// Clear receive buffer
        /// </summary>
        private void FlushReceive()
        {
            lock (receiveBuffer)
            {
                receiveBuffer.Clear();
                Console.WriteLine("[TTL] Receive buffer flushed");
            }
        }
        
        /// <summary>
        /// Check if receive buffer contains specified text
        /// </summary>
        public bool ContainsText(string text)
        {
            lock (receiveBuffer)
            {
                string bufferContent = receiveBuffer.ToString();
                bool contains = bufferContent.Contains(text);
                Console.WriteLine($"[TTL ContainsText] Looking for: '{text}', Buffer length: {bufferContent.Length}, Contains: {contains}");
                if (bufferContent.Length > 0 && bufferContent.Length <= 200)
                {
                    Console.WriteLine($"[TTL ContainsText] Buffer content: '{bufferContent}'");
                }
                else if (bufferContent.Length > 200)
                {
                    Console.WriteLine($"[TTL ContainsText] Buffer content (first 200): '{bufferContent.Substring(0, 200)}'");
                }
                return contains;
            }
        }
        
        /// <summary>
        /// Remove text from receive buffer (used by WaitAll)
        /// </summary>
        public void RemoveTextFromBuffer(string text)
        {
            lock (receiveBuffer)
            {
                string currentBuffer = receiveBuffer.ToString();
                int foundIndex = currentBuffer.IndexOf(text);
                
                if (foundIndex >= 0)
                {
                    int removeLength = foundIndex + text.Length;
                    
                    if (removeLength <= receiveBuffer.Length)
                    {
                        string remainingText = currentBuffer.Substring(removeLength);
                        receiveBuffer.Clear();
                        receiveBuffer.Append(remainingText);
                        
                        Console.WriteLine($"[TTL] Removed '{text}' from buffer (WaitAll)");
                    }
                }
            }
        }

        private void Pause(string args)
        {
            int seconds = ParseInt(args);
            Console.WriteLine($"[TTL] Pausing for {seconds} seconds...");
            
            // 使用小間隔的 Sleep 來允許取消
            int elapsed = 0;
            int interval = 100; // 每 100ms 檢查一次
            int totalMs = seconds * 1000;
            
            while (elapsed < totalMs)
            {
                if (isCancelled)
                {
                    Console.WriteLine("[TTL] Pause cancelled");
                    throw new OperationCanceledException("Pause operation was cancelled by user");
                }
                
                int sleepTime = Math.Min(interval, totalMs - elapsed);
                Thread.Sleep(sleepTime);
                elapsed += sleepTime;
            }
        }

        private void SetTimeout(string args)
        {
            var match = Regex.Match(args, @"=\s*(\d+)");
            if (match.Success)
                timeout = ParseInt(match.Groups[1].Value) * 1000;
            else
                timeout = ParseInt(args) * 1000;

            Console.WriteLine($"[TTL] Timeout set to {timeout}ms");
        }

        private void MessageBox(string args)
        {
            string message = args.Trim('\'', '"');
            Console.WriteLine($"[TTL MessageBox] {message}");
            System.Windows.Forms.MessageBox.Show(message, "TTL Script", 
                System.Windows.Forms.MessageBoxButtons.OK, 
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        #endregion

        #region Helper Methods

        private string GetCommand(string line)
        {
            int spaceIndex = line.IndexOf(' ');
            if (spaceIndex > 0)
                return line.Substring(0, spaceIndex).Trim();
            return line.Trim();
        }

        private string GetArgs(string line)
        {
            int spaceIndex = line.IndexOf(' ');
            if (spaceIndex > 0)
                return line.Substring(spaceIndex + 1).Trim();
            return "";
        }

        private string ReplaceVariables(string text)
        {
            string originalText = text;
            
            // Modified: Use regex for whole word replacement
            foreach (var kvp in variables)
            {
                // Skip numeric variable names
                if (int.TryParse(kvp.Key, out _))
                {
                    Console.WriteLine($"[TTL] Warning: Skipping numeric variable name '{kvp.Key}' in replacement");
                    continue;
                }
                
                // Use regex to ensure whole word match
                string pattern = @"\b" + Regex.Escape(kvp.Key) + @"\b";
                string oldText = text;
                text = Regex.Replace(text, pattern, kvp.Value?.ToString() ?? "");
                
                // 調試：顯示替換結果
                if (oldText != text)
                {
                    Console.WriteLine($"[TTL ReplaceVar] Replaced '{kvp.Key}' with '{kvp.Value}': '{oldText}' -> '{text}'");
                }
            }

            // Replace built-in variable result
            text = Regex.Replace(text, @"\bresult\b", result.ToString());
            
            if (originalText != text)
            {
                Console.WriteLine($"[TTL ReplaceVar] Final: '{originalText}' -> '{text}'");
            }
            
            return text;
        }

        private int ParseInt(string value)
        {
            value = ReplaceVariables(value.Trim());

            if (int.TryParse(value, out int result))
                return result;

            return 0;
        }
        private int ParseIntDirect(string value)
        {
            value = value.Trim();

            if (int.TryParse(value, out int result))
                return result;

            return 0;
        }

        private void Cleanup()
        {
            
        }

        #endregion

        public void Cancel()
        {
            isCancelled = true;
            Console.WriteLine("[TTL] Script cancellation requested");
        }

        public void Dispose()
        {
            isCancelled = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose ManualResetEvent
                    waitAllEvent?.Dispose();
                    
                    // Unsubscribe from bridge events
                    if (bridge != null)
                    {
                        bridge.DataReceived -= Bridge_DataReceived;
                        Console.WriteLine("[TTL] Unsubscribed from bridge DataReceived event");
                    }
                }
                
                disposed = true;
            }
        }

        ~TTLInterpreter()
        {
            Dispose(false);
        }
    }
}