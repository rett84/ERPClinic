@echo off

REM Go to project folder
cd /d "%USERPROFILE%\Desktop\ERPClinica"

REM Set environment to Development (like VS)
set ASPNETCORE_ENVIRONMENT=Development

REM Force 32-bit process for Jet OLEDB
set DOTNET_RUNNING_IN_32BIT=1

REM Run the Blazor Server app on HTTP and HTTPS
start "" "C:\Program Files (x86)\dotnet\dotnet.exe" run --urls "http://localhost:5000;https://localhost:5001"

REM Wait a few seconds to let the server start, then open the browser automatically
timeout /t 5 /nobreak >nul
start "" http://localhost:5000

pause