@echo off
setlocal

set "REPO_ROOT=%~dp0"
set "APP_EXE="

for /f "usebackq delims=" %%F in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$root = Resolve-Path -LiteralPath '%REPO_ROOT%'; $exe = Get-ChildItem -LiteralPath (Join-Path $root 'artifacts\release') -Filter 'ChinaTrayCalendar.Desktop.exe' -Recurse -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1; if ($exe) { $exe.FullName }"`) do set "APP_EXE=%%F"

if not defined APP_EXE (
  echo No built preview app found. Building latest preview first...
  call "%REPO_ROOT%BuildLatest.cmd"
  if errorlevel 1 exit /b %ERRORLEVEL%

  for /f "usebackq delims=" %%F in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$root = Resolve-Path -LiteralPath '%REPO_ROOT%'; $exe = Get-ChildItem -LiteralPath (Join-Path $root 'artifacts\release') -Filter 'ChinaTrayCalendar.Desktop.exe' -Recurse -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1; if ($exe) { $exe.FullName }"`) do set "APP_EXE=%%F"
)

if not defined APP_EXE (
  echo Failed to locate ChinaTrayCalendar.Desktop.exe under artifacts\release.
  exit /b 1
)

for %%F in ("%APP_EXE%") do set "APP_DIR=%%~dpF"

echo Starting Dateview preview:
echo %APP_EXE%
start "Dateview Preview" /D "%APP_DIR%" "%APP_EXE%"
exit /b 0
