@echo off
set "PUBLISH_DIR=%USERPROFILE%\Desktop\ERPClinica\publish"

dotnet publish "%USERPROFILE%\ERPClinica" -c Release -o "%PUBLISH_DIR%"

echo Published to: %PUBLISH_DIR%
pause
