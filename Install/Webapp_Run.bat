@echo off

REM Go to project folder
cd /d "%USERPROFILE%\Desktop\Webapp"

start "" "%USERPROFILE%\Desktop\Webapp\ERPClinic.exe"

echo Aguarde o WebBrowser abrir...

REM Wait a few seconds to let the server start, then open the browser automatically
timeout /t 10 /nobreak >nul
start "" http://localhost:5000

pause