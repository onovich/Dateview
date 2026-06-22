# Dateview

Dateview is a lightweight Windows tray calendar for viewing the current month, Chinese public holidays, adjusted workdays, and weekends without replacing or injecting into the Windows taskbar.<br/>**Dateview 是一个轻量 Windows 托盘日历，用于查看当前月份、中国法定节假日、调休工作日和周末；它不会替换任务栏，也不会注入 Windows 任务栏进程。**

## Features

- Runs as a normal user-mode WPF tray app with `System.Windows.Forms.NotifyIcon`.<br/>**作为普通用户权限的 WPF 托盘应用运行，托盘集成使用 `System.Windows.Forms.NotifyIcon`。**
- Left-click the tray icon to show or hide the compact calendar popup near the taskbar edge.<br/>**左键点击托盘图标，可在任务栏边缘附近显示或隐藏紧凑日历弹窗。**
- Right-click the tray icon for Today, Settings, Start with Windows, and Exit entries; settings currently hosts the editable startup toggle.<br/>**右键点击托盘图标可看到“今天”“设置”“开机启动”“退出”等入口；当前开机启动开关在设置窗口中编辑。**
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
