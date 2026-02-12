# MyTeraTerm

> Multi-terminal serial communication tool with embedded TeraTerm windows, COM port bridging, TTL scripting, and PDU control support

[![Version](https://img.shields.io/badge/version-1.0.0-blue)](https://github.com/ETWen/MyTeraTerm/releases)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-Framework%204.7.2-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## 📖 Table of Contents

- [Features](#-features)
- [Screenshots](#-screenshots)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Usage Guide](#-usage-guide)
- [TTL Scripting](#-ttl-scripting)
- [PDU Control](#-pdu-control)
- [RX Data Logging](#-rx-data-logging)
- [Troubleshooting](#-troubleshooting)
- [Building from Source](#-building-from-source)
- [Project Structure](#-project-structure)
- [Contributing](#-contributing)
- [Version History](#-version-history)
- [License](#-license)
- [Acknowledgments](#-acknowledgments)
- [Contact](#-contact)

---

## ✨ Features

### Core Functionality

- 🖥️ **Multi-Terminal Support**
  - Manage up to 6 simultaneous serial terminal connections
  - Independent configuration for each terminal (COM port, baud rate)
  - Real-time TX/RX LED indicators for each terminal

- 📺 **Embedded TeraTerm Windows**
  - Native TeraTerm windows embedded directly in the application
  - No need to manage separate TeraTerm windows
  - Automatic window resizing and layout management

- 🔌 **Virtual COM Port Bridging**
  - Automatic creation of virtual COM port pairs (COM101-112) via com0com
  - Transparent bridging between physical and virtual COM ports
  - Real-time data forwarding in both directions

- 🤖 **TTL Script Automation**
  - Execute TeraTerm Language (TTL) scripts for test automation
  - Real-time script status display (current line and command)
  - Support for wait, send, waitln, sendln commands and more
  - Graceful script cancellation

- 📊 **Real-time RX Data Logging**
  - Optional RX data logging to file for each terminal
  - Configurable timestamp prefixing
  - Organized log structure with session-based folders
  - Automatic log file management

- ⚡ **PDU Power Control Integration**
  - Control PDU (Power Distribution Unit) outlets
  - Power cycle remote devices during testing
  - Integration with TTL scripts for automated power management

### User Interface

- 🎨 Clean, intuitive tabbed interface
- 📊 Real-time connection status indicators (LEDs)
- 🔄 Flexible layout: 1/2/4/6 terminal views
- 🎯 Status bar with version info and local time
- 🔐 Password-protected debug console access

---

## 📸 Screenshots

### Single Terminal View
```
┌─────────────────────────────────────────────────────────┐
│ [Terminal 1 Configuration Panel]                        │
│ COM Port: [COM3▼]  Baud Rate: [115200▼]  [Connect]     │
│ RX: ● TX: ●  Log: ☑ Timestamp: ☑  [Run Script]        │
│ ┌─────────────────────────────────────────────────────┐ │
│ │                                                       │ │
│ │        [Embedded TeraTerm Window]                    │ │
│ │                                                       │ │
│ └─────────────────────────────────────────────────────┘ │
│ Script Status: Idle                                      │
└─────────────────────────────────────────────────────────┘
```

### Multi-Terminal View (2x2)
```
┌──────────────────────┬──────────────────────┐
│   Terminal 1         │   Terminal 2         │
│   [Config] [Status]  │   [Config] [Status]  │
│   ┌────────────────┐ │   ┌────────────────┐ │
│   │   TeraTerm     │ │   │   TeraTerm     │ │
│   └────────────────┘ │   └────────────────┘ │
├──────────────────────┼──────────────────────┤
│   Terminal 3         │   Terminal 4         │
│   [Config] [Status]  │   [Config] [Status]  │
│   ┌────────────────┐ │   ┌────────────────┐ │
│   │   TeraTerm     │ │   │   TeraTerm     │ │
│   └────────────────┘ │   └────────────────┘ │
└──────────────────────┴──────────────────────┘
```

---

## 💻 System Requirements

### Minimum Requirements

- **Operating System**: Windows 10 (build 1809) or later / Windows 11
- **Framework**: .NET Framework 4.7.2 or higher
- **RAM**: 512 MB (1 GB recommended)
- **Disk Space**: 100 MB free space
- **Display**: 1024x768 resolution (1920x1080 recommended)

### Required Dependencies

1. **TeraTerm** (4.106 or later)
   - Download: https://ttssh2.osdn.jp/index.html.en
   - Should be installed in `Application/TeraTerm/` folder

2. **com0com** (3.0.0.0 or later)
   - Download: https://sourceforge.net/projects/com0com/
   - Virtual COM port driver for Windows

### Optional Dependencies

- **PDU Control Library** (for PDU functionality)
- **Serial Port Hardware** (USB-to-Serial adapters, etc.)

---

## 📥 Installation

### Option 1: Binary Release (Recommended)

1. **Download** the latest release from [Releases](https://github.com/ETWen/MyTeraTerm/releases)
   ```
   MyTeraTerm-v1.0.0-win-x64.zip
   ```

2. **Extract** to your preferred location
   ```
   C:\Program Files\MyTeraTerm\
   ```

3. **Install Dependencies**
   - Install TeraTerm: Place `ttermpro.exe` in `Application/TeraTerm/`
   - Install com0com driver (run as Administrator)

4. **First Launch**
   - Run `MyTeraTerm.exe`
   - The application will automatically check and create virtual COM ports (COM101-112)
   - Grant Administrator privileges when prompted (required for first-time setup)

### Option 2: Portable Version

1. Extract the release archive to a USB drive or any folder
2. Ensure TeraTerm and com0com are installed on the target system
3. Run `MyTeraTerm.exe` directly (no installation required)

---

## 🚀 Quick Start

### Basic Connection

1. **Launch MyTeraTerm**
   ```
   MyTeraTerm.exe
   ```

2. **Select Terminal View**
   - Menu → Terminal → Select "1" for single terminal

3. **Configure Terminal 1**
   - COM Port: Select your physical COM port (e.g., COM3)
   - Baud Rate: Select baud rate (e.g., 115200)

4. **Connect**
   - Click "Connect" button
   - TeraTerm window will embed automatically
   - TX/RX LEDs will indicate communication status

5. **Disconnect**
   - Click "Disconnect" button
   - Or press `Alt+Q` in TeraTerm window

### Running a TTL Script

1. **Connect Terminal** (as above)

2. **Click "Run Script"**

3. **Select TTL File**
   - Navigate to `Scripts/` folder
   - Select a `.ttl` file

4. **Monitor Execution**
   - Script status shows current line and command
   - TX/RX LEDs flash during communication

5. **Stop Script** (if needed)
   - Click "End Script" button

---

## 📚 Usage Guide

### Multi-Terminal Setup

#### 2 Terminals (Side-by-Side)
```
Menu → Terminal → 2
```
- Terminal 1: Left panel
- Terminal 2: Right panel

#### 4 Terminals (2x2 Grid)
```
Menu → Terminal → 4
```
- Terminal 1: Top-left
- Terminal 2: Top-right
- Terminal 3: Bottom-left
- Terminal 4: Bottom-right

#### 6 Terminals (2x3 Grid)
```
Menu → Terminal → 6
```
- Terminals 1-3: Top row
- Terminals 4-6: Bottom row

### Terminal Configuration

Each terminal has independent settings:

| Setting | Options | Description |
|---------|---------|-------------|
| **COM Port** | COM1-COM255 | Physical or virtual COM port |
| **Baud Rate** | 300-921600 | Serial communication speed |
| **Log Enable** | ☑/☐ | Enable RX data logging |
| **Timestamp** | ☑/☐ | Add timestamp to each log line |

### LED Indicators

Each terminal has TX/RX status LEDs:

| LED | State | Meaning |
|-----|-------|---------|
| 🟢 TX Green | Flash | Transmitting data |
| 🔴 RX Red | Flash | Receiving data |

---

## 🤖 TTL Scripting

### Supported Commands

```ttl
; Basic Commands
send 'Hello World'         ; Send text
sendln 'Hello'             ; Send text with newline
wait 1000                  ; Wait 1000 ms
wait '$ '                  ; Wait for prompt

; Flow Control
if result = 1 then
    sendln 'success'
endif

mpause 500                 ; Pause 500 ms

; PDU Control (requires PDU hardware)
pduconnect '192.168.1.100' 'admin' 'password'
pduon 1                    ; Turn on outlet 1
wait 2000
pduoff 1                   ; Turn off outlet 1
```

### Example Scripts

#### 1. Simple Login Script
```ttl
; filepath: Scripts/login.ttl
; Simple login script

wait '$ '
sendln 'admin'
wait 'Password: '
sendln 'password123'
wait '# '
sendln 'ls -la'
```

#### 2. Device Power Cycle with PDU
```ttl
; filepath: Scripts/power_cycle.ttl
; Power cycle device via PDU

; Connect to PDU
pduconnect '192.168.1.100' 'admin' 'admin'

; Power off
pduoff 1
mpause 3000

; Power on
pduon 1
mpause 5000

; Wait for device prompt
wait 'login: '
sendln 'root'
wait 'Password: '
sendln 'toor'
```

#### 3. Automated Test Sequence
```ttl
; filepath: Scripts/automated_test.ttl
; Automated device testing

; Wait for boot
wait 'login: '
sendln 'admin'
wait 'Password: '
sendln 'admin123'
wait '# '

; Run tests
sendln 'test_network'
wait 'PASS'

sendln 'test_memory'
wait 'PASS'

sendln 'test_storage'
wait 'PASS'

sendln 'exit'
```

### Creating TTL Scripts

1. **Create a new file** in `Scripts/` folder
   ```
   Scripts/my_script.ttl
   ```

2. **Write commands** using TTL syntax
   ```ttl
   ; My custom script
   wait '$ '
   sendln 'hello world'
   ```

3. **Run from MyTeraTerm**
   - Click "Run Script"
   - Select your `.ttl` file

---

## ⚡ PDU Control

### PDU Connection

PDU control allows you to remotely power cycle devices during testing.

#### In TTL Scripts
```ttl
; Connect to PDU
pduconnect 'PDU_IP' 'username' 'password'

; Control outlets
pduon 1      ; Turn on outlet 1
pduoff 1     ; Turn off outlet 1
```

#### Manual Control (via Menu)
```
Menu → PDU → Connect
- Enter PDU IP address
- Enter username and password
- Click Connect
```

### Supported PDU Commands

| Command | Syntax | Description |
|---------|--------|-------------|
| Connect | `pduconnect 'IP' 'user' 'pass'` | Connect to PDU |
| Power On/Off | `pductrl 'device' 'port' 'on/off'` | Turn on specific outlet |

---

## 📝 RX Data Logging

### Log File Structure

```
MyTeraTerm.exe
└── Logs/
    └── [Form Start Time]/
        ├── [Connect Time]_Terminal1.log
        ├── [Connect Time]_Terminal2.log
        └── ...
```

Example:
```
Logs/
└── 20260212_143025/
    ├── 20260212_143030_Terminal1.log
    ├── 20260212_143045_Terminal2.log
    └── 20260212_143100_Terminal3.log
```

### Log File Format

#### With Timestamp Enabled
```
=== Terminal 1 RX Log ===
Connect Time: 2026-02-12 14:30:30
Timestamp Enabled: True
========================================

[2026-02-12 14:30:35.123] login: admin
[2026-02-12 14:30:35.456] Password: 
[2026-02-12 14:30:36.789] # ls -la
[2026-02-12 14:30:36.890] total 24
[2026-02-12 14:30:36.891] drwxr-xr-x 2 root root 4096 Feb 12 14:30 .
```

#### Without Timestamp
```
=== Terminal 1 RX Log ===
Connect Time: 2026-02-12 14:30:30
Timestamp Enabled: False
========================================

login: admin
Password: 
# ls -la
total 24
drwxr-xr-x 2 root root 4096 Feb 12 14:30 .
```

### Enabling Logging

1. **Before connecting**, check the "Log" checkbox
2. **(Optional)** Check "Timestamp" for timestamped logs
3. Click "Connect"
4. All received data will be logged to file

**Note**: Logging settings are locked after connection to maintain log consistency.

---

## 🔧 Troubleshooting

### Common Issues

#### 1. "Virtual COM ports (COM101-112) not found"

**Cause**: com0com driver not installed or ports not created

**Solution**:
```
1. Click "Yes" when prompted to create virtual ports
2. Grant Administrator privileges
3. Restart MyTeraTerm
```

Or manually:
```cmd
cd Application\com0com\x64
setupc.exe install PortName=COM101 PortName=COM102
setupc.exe install PortName=COM103 PortName=COM104
setupc.exe install PortName=COM105 PortName=COM106
setupc.exe install PortName=COM107 PortName=COM108
setupc.exe install PortName=COM109 PortName=COM110
setupc.exe install PortName=COM111 PortName=COM112
```

#### 2. "Connection Failed - COM Port in use"

**Cause**: Another application is using the COM port

**Solutions**:
- Close other serial terminal applications
- Check Device Manager for COM port conflicts
- Restart computer

#### 3. "TeraTerm not found"

**Cause**: TeraTerm not installed in correct location

**Solution**:
```
1. Ensure ttermpro.exe exists at:
   Application\TeraTerm\ttermpro.exe

2. Or download TeraTerm from:
   https://ttssh2.osdn.jp/

3. Extract to Application\TeraTerm\ folder
```

#### 4. Script Hangs on "wait" Command

**Cause**: Expected text not received

**Solutions**:
- Check serial connection is working
- Verify baud rate matches device
- Check script wait pattern is correct
- Click "End Script" to stop

#### 5. RX Log Not Created

**Cause**: Log checkbox not enabled before connection

**Solution**:
```
1. Disconnect terminal
2. Enable "Log" checkbox
3. Re-connect terminal
```

### Debug Console

For detailed troubleshooting:

1. **Enable Debug Console**
   ```
   Menu → Tools → Debug Console
   ```

2. **Enter password** (default: admin)

3. **View real-time logs**
   - All COM port operations
   - Bridge status
   - TTL script execution
   - Error details

4. **Log file location**
   ```
   Logs\Debug\[date]_debug.log
   ```

---

## 🔨 Building from Source

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.7.2 SDK
- Git

### Clone Repository

```bash
git clone https://github.com/ETWen/MyTeraTerm.git
cd MyTeraTerm
```

### Build Steps

#### Using Visual Studio

1. Open `MyTeraTerm.sln`
2. Build → Build Solution (`Ctrl+Shift+B`)
3. Output: `bin\Release\MyTeraTerm.exe`

#### Using MSBuild (Command Line)

```cmd
msbuild MyTeraTerm.sln /p:Configuration=Release
```

### Dependencies

External libraries are in `lib/` folder:
- `EmbeddedWindowController.cs` - Window embedding
- `TTLInterpreter.cs` - TTL script execution
- `ComPortBridge.cs` - COM port bridging
- `PduController.cs` - PDU control
- `RJToggleButton.cs` - Custom UI controls

---

## 📁 Project Structure

```
MyTeraTerm/
├── Application/              # Runtime dependencies
│   ├── TeraTerm/            # TeraTerm executable
│   │   └── ttermpro.exe
│   └── com0com/             # com0com tools
│       └── x64/
│           └── setupc.exe
├── Scripts/                  # TTL script files
│   ├── login.ttl
│   ├── power_cycle.ttl
│   └── automated_test.ttl
├── Logs/                     # RX log files (auto-created)
│   └── [timestamp]/
│       └── Terminal*.log
├── lib/                      # External libraries
│   ├── ComPortBridge.cs
│   ├── TTLInterpreter.cs
│   ├── EmbeddedWindowController.cs
│   ├── PduController.cs
│   └── RJToggleButton.cs
├── Properties/               # Project properties
├── resource/                 # Resources (icons, images)
├── Form1.cs                  # Main form
├── Form1.Designer.cs         # Form designer
├── Form1.SerialPort.cs       # Serial port logic (partial)
├── Form1.Pdu.cs             # PDU control logic (partial)
├── Program.cs                # Application entry point
├── AppLogger.cs              # Logging system
├── PasswordDialog.cs         # Password dialog
├── MyTeraTerm.csproj        # Project file
├── VERSION.md                # Version history
├── README.md                 # This file
└── ref/                      # Reference documentation
    ├── gitcommit.md         # Git commit guide
    └── TAG_TEMPLATE.md      # Release tag template
```

---

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### Contribution Process

1. **Fork** the repository
2. **Create** a feature branch
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Commit** using [conventional commits](ref/gitcommit.md)
   ```bash
   git commit -m "feat(serial): add auto-reconnect feature"
   ```
4. **Push** to your fork
   ```bash
   git push origin feature/your-feature-name
   ```
5. **Create** a Pull Request

### Coding Standards

- Follow C# naming conventions
- Add XML documentation comments for public methods
- Include error handling with `AppLogger`
- Write unit tests for new features (if applicable)

### Commit Message Format

See [gitcommit.md](ref/gitcommit.md) for detailed commit guidelines.

```
<type>(<scope>): <subject>

<body>

<footer>
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`

---

## 📜 Version History

See [VERSION.md](VERSION.md) for complete version history.

### v1.0.0 (2026-02-12)

#### 🎉 Initial Release

- Multi-terminal support (up to 6 terminals)
- COM port bridging via com0com
- Embedded TeraTerm windows
- TTL script interpreter
- RX data logging with timestamps
- PDU control integration

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024-2026 MyTeraTerm Project

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🙏 Acknowledgments

This project wouldn't be possible without:

- **[TeraTerm Project](https://ttssh2.osdn.jp/)** - Excellent terminal emulator
- **[com0com](https://sourceforge.net/projects/com0com/)** - Virtual COM port driver
- **Microsoft** - .NET Framework and Visual Studio
- All contributors and users who provided feedback

### Third-Party Libraries

- EmbeddedWindowController - Window embedding functionality
- TTLInterpreter - TTL script parsing and execution
- ComPortBridge - Serial port bridging
- PDU Controller - Power distribution unit control
- RJToggleButton - Custom UI toggle button control

---

## 📧 Contact

- **Author**: ETWen
- **Email**: eric441151893@gmail.com
- **GitHub**: https://github.com/ETWen/MyTeraTerm
- **Issues**: https://github.com/ETWen/MyTeraTerm/issues

### Support

- 📖 **Documentation**: See Wiki (coming soon)
- 🐛 **Bug Reports**: [GitHub Issues](https://github.com/ETWen/MyTeraTerm/issues)
- 💡 **Feature Requests**: [GitHub Issues](https://github.com/ETWen/MyTeraTerm/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/ETWen/MyTeraTerm/discussions)

---

## 🌟 Star History

If you find this project useful, please consider giving it a star! ⭐

[![Star History Chart](https://api.star-history.com/svg?repos=ETWen/MyTeraTerm&type=Date)](https://star-history.com/#ETWen/MyTeraTerm&Date)

---

**Made with by 👽ETWen**

Last Updated: 2026-02-12