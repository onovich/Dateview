@echo off
setlocal

pushd "%~dp0" >nul
powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\package-release.ps1"
set "exitCode=%ERRORLEVEL%"
popd >nul

if not "%exitCode%"=="0" (
  echo.
  echo BuildLatest failed with exit code %exitCode%.
  exit /b %exitCode%
)

echo.
echo BuildLatest completed. Release files are under artifacts\release.
exit /b 0
