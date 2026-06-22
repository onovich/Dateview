# Dateview

Dateview is a lightweight Windows tray calendar for viewing the current month, Chinese public holidays, adjusted workdays, and weekends without replacing or injecting into the Windows taskbar.<br/>**Dateview 是一个轻量 Windows 托盘日历，用于查看当前月份、中国法定节假日、调休工作日和周末；它不会替换任务栏，也不会注入 Windows 任务栏进程。**

## Features

- Runs as a normal user-mode WPF tray app with `System.Windows.Forms.NotifyIcon`.<br/>**作为普通用户权限的 WPF 托盘应用运行，托盘集成使用 `System.Windows.Forms.NotifyIcon`。**
- Left-click the tray icon to show or hide the compact calendar popup near the taskbar edge.<br/>**左键点击托盘图标，可在任务栏边缘附近显示或隐藏紧凑日历弹窗。**
- Right-click the tray icon for Today, Settings, Start with Windows, and Exit entries; Start with Windows toggles the current-user startup registration.<br/>**右键点击托盘图标可看到“今天”“设置”“开机启动”“退出”等入口；“开机启动”会切换当前用户的自启动注册。**
- Uses bundled offline holiday JSON files for China holiday and adjusted-workday data.<br/>**使用随应用打包的离线中国节假日 JSON 文件，不依赖启动时联网。**
- Stores settings under the current user's roaming profile and writes startup registration only to HKCU.<br/>**设置保存到当前用户漫游配置目录，自启动注册只写入 HKCU。**
- Does not use Explorer injection, global hooks, administrator privileges, online holiday APIs, sync, or telemetry.<br/>**不使用 Explorer 注入、全局钩子、管理员权限、在线节假日 API、日历同步或遥测。**

## Requirements

- Windows desktop environment.<br/>**Windows 桌面环境。**
- .NET 10 SDK for development, testing, and publishing.<br/>**开发、测试和发布需要 .NET 10 SDK。**

## Build And Test

```powershell
dotnet restore Dateview.slnx
dotnet build Dateview.slnx --configuration Release --no-restore
dotnet test Dateview.slnx --configuration Release --no-build
dotnet format Dateview.slnx --verify-no-changes
```

The repository validation wrapper runs the same gate sequence.<br/>**仓库验证包装器会运行同一组质量门禁。**

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

## Run From Source

```powershell
dotnet run --project src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj --configuration Release
```

The app starts in the tray and does not show the calendar popup until the tray icon is clicked.<br/>**应用启动后驻留托盘，不会在点击托盘图标前主动显示日历弹窗。**

## Publish

```powershell
dotnet publish src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj -p:PublishProfile=win-x64-folder
```

The configured output folder is `src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\`.<br/>**已配置的发布输出目录是 `src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\`。**

## Portable Release Bundle

Create the trial release bundle with the repository script:<br/>**使用仓库脚本创建试用版发布包：**

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
```

The script publishes the app, stages a portable folder, creates a zip, and writes SHA256 and JSON metadata under `artifacts\release\`.<br/>**脚本会发布应用、整理便携目录、创建 zip，并在 `artifacts\release\` 下写入 SHA256 和 JSON 元数据。**

Generated local artifacts include:<br/>**生成的本地产物包括：**

- `artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\`
- `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
- `artifacts\release\Dateview-0.1.0-preview-win-x64.sha256.txt`
- `artifacts\release\Dateview-0.1.0-preview-win-x64.release.json`

These generated artifacts are ignored by git and should not be committed.<br/>**这些生成产物已被 git 忽略，不应提交到仓库。**

## Use A Portable Bundle

1. Download or copy `Dateview-0.1.0-preview-win-x64.zip` and the matching `.sha256.txt` file if it is provided.<br/>**下载或复制 `Dateview-0.1.0-preview-win-x64.zip`；如果同时提供 `.sha256.txt`，也一起保存。**
2. Optional integrity check:<br/>**可选完整性检查：**

```powershell
Get-FileHash -Algorithm SHA256 .\Dateview-0.1.0-preview-win-x64.zip
Get-Content .\Dateview-0.1.0-preview-win-x64.sha256.txt
```

3. Extract the zip to a normal user folder, such as `%LOCALAPPDATA%\Programs\Dateview` or a tools folder under the current user's profile.<br/>**将 zip 解压到普通用户目录，例如 `%LOCALAPPDATA%\Programs\Dateview`，或当前用户配置文件下的工具目录。**
4. Run `Dateview\ChinaTrayCalendar.Desktop.exe` from the extracted folder.<br/>**从解压后的目录运行 `Dateview\ChinaTrayCalendar.Desktop.exe`。**
5. To exit, right-click the tray icon and choose Exit.<br/>**退出应用时，右键托盘图标并选择“退出”。**
6. To start with Windows, right-click the tray icon and toggle Start with Windows. This writes only the current user's HKCU Run entry.<br/>**如需开机启动，右键托盘图标并切换“开机启动”。该操作只写入当前用户的 HKCU Run 项。**
7. To uninstall the portable build, exit the app, turn off Start with Windows if enabled, delete the extracted folder, and optionally remove `%APPDATA%\ChinaTrayCalendar\settings.json` if you do not want to keep preferences.<br/>**卸载便携版时，先退出应用；如果启用了开机启动，先关闭它；然后删除解压目录。如不想保留偏好设置，可再删除 `%APPDATA%\ChinaTrayCalendar\settings.json`。**

P9 does not provide an installer, auto-update, code signing, online holiday sync, or telemetry.<br/>**P9 不提供安装器、自动更新、代码签名、在线节假日同步或遥测。**

## Windows Trust Notes

Dateview `0.1.0-preview` is an unsigned portable preview. Windows may show an unknown-publisher, SmartScreen, or Microsoft Defender warning for an unsigned executable or a downloaded zip.<br/>**Dateview `0.1.0-preview` 是未签名的便携预览版。对于未签名 exe 或下载得到的 zip，Windows 可能显示未知发布者、SmartScreen 或 Microsoft Defender 提示。**

- Use builds only from a trusted project or distributor location.<br/>**只使用来自可信项目位置或可信分发方的构建。**
- Compare the zip SHA256 with the matching `.sha256.txt` before running it.<br/>**运行前请将 zip 的 SHA256 与配套 `.sha256.txt` 对比。**
- Do not disable Windows Security, Microsoft Defender, or SmartScreen to run Dateview.<br/>**不要为了运行 Dateview 而关闭 Windows 安全中心、Microsoft Defender 或 SmartScreen。**
- If the source or hash does not match what you expected, do not run the app.<br/>**如果来源或哈希与预期不一致，请不要运行该应用。**

## Settings And Data

- Settings file: `%APPDATA%\ChinaTrayCalendar\settings.json`.<br/>**设置文件：`%APPDATA%\ChinaTrayCalendar\settings.json`。**
- Startup registration: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`, value name `ChinaTrayCalendar`.<br/>**自启动注册位置：`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`，值名为 `ChinaTrayCalendar`。**
- Holiday data: `assets\holidays\cn\{year}.json`, copied into the published output.<br/>**节假日数据：`assets\holidays\cn\{year}.json`，发布时会复制到输出目录。**
- Current bundled holiday coverage is 2025 and 2026, sourced from official State Council notices listed in `docs\HOLIDAY_DATA.md`.<br/>**当前内置节假日覆盖 2025 和 2026 年，来源是 `docs\HOLIDAY_DATA.md` 中列出的国务院官方通知。**

## Architecture

The project keeps strict layer direction: Desktop -> Infrastructure -> Application -> Domain, and Desktop -> Application -> Domain.<br/>**项目保持严格层级方向：Desktop -> Infrastructure -> Application -> Domain，以及 Desktop -> Application -> Domain。**

- `Domain`: pure calendar and holiday classification logic.<br/>**`Domain`：纯日历与节假日分类逻辑。**
- `Application`: use cases and ports such as holiday repository, settings store, clock, and autostart service.<br/>**`Application`：用例和端口，例如节假日仓储、设置存储、时钟和自启动服务。**
- `Infrastructure`: JSON holiday/settings storage and HKCU startup registration adapters.<br/>**`Infrastructure`：JSON 节假日/设置存储，以及 HKCU 自启动注册适配器。**
- `Desktop`: WPF windows, tray lifecycle, view models, placement, and composition root.<br/>**`Desktop`：WPF 窗口、托盘生命周期、视图模型、定位和组合根。**

## Troubleshooting

See `docs\TROUBLESHOOTING.md` for tray visibility, DPI, multi-monitor, settings path, startup, and holiday-data notes.<br/>**托盘可见性、DPI、多屏、设置路径、自启动和节假日数据问题见 `docs\TROUBLESHOOTING.md`。**
