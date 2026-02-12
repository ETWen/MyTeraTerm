@echo off
cd /d "%~dp0com0com\x64"
setupc.exe install PortName=COM101 PortName=COM102
setupc.exe install PortName=COM103 PortName=COM104
setupc.exe install PortName=COM105 PortName=COM106
setupc.exe install PortName=COM107 PortName=COM108
setupc.exe install PortName=COM109 PortName=COM110
setupc.exe install PortName=COM111 PortName=COM112
echo.
echo Virtual COM ports setup completed.
pause
