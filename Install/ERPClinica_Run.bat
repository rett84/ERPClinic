@echo off
cd /d "%USERPROFILE%\Desktop\ERPClinica"
dotnet ERPClinic.dll --urls "http://localhost:7040"

REM Wait a moment to let the server start
timeout /t 3 /nobreak >nul

REM Open the default browser at the specified URL
start "" "http://localhost:7040"

pause