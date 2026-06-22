# Release Notes

## 0.1.0-preview - 2026-06-22

Dateview `0.1.0-preview` is the first trial distribution package for Windows tray-calendar validation.<br/>**Dateview `0.1.0-preview` 是用于 Windows 托盘日历验证的首个试用分发包。**

### Distribution

- Provides a portable `win-x64` folder/zip bundle. No installer is required for this preview.<br/>**提供便携式 `win-x64` 文件夹和 zip 包；此预览版不需要安装器。**
- Runs as a normal current-user desktop app from the extracted folder.<br/>**应用从解压目录以普通当前用户桌面程序运行。**
- Includes bundled offline holiday data for China 2025 and 2026.<br/>**内置中国 2025 和 2026 年离线节假日数据。**
- Generates `release-manifest.json`, `.release.json`, and `.sha256.txt` metadata for the portable bundle.<br/>**为便携包生成 `release-manifest.json`、`.release.json` 和 `.sha256.txt` 元数据。**

### User Workflow

- Left-click the tray icon to show or hide the calendar popup.<br/>**左键点击托盘图标可显示或隐藏日历弹窗。**
- Right-click the tray icon to open Today, Settings, Start with Windows, and Exit actions.<br/>**右键托盘图标可打开“今天”“设置”“开机启动”“退出”等操作。**
- Start with Windows uses only the current user's HKCU Run key.<br/>**“开机启动”只使用当前用户的 HKCU Run 项。**
- Uninstall by exiting the app, disabling Start with Windows if it was enabled, deleting the extracted folder, and optionally deleting `%APPDATA%\ChinaTrayCalendar\settings.json`.<br/>**卸载时先退出应用；如已启用开机启动则先关闭；然后删除解压目录，并可按需删除 `%APPDATA%\ChinaTrayCalendar\settings.json`。**

### Integrity And Limits

- SHA256 files are integrity checks for release zips, not code signatures.<br/>**SHA256 文件只用于 zip 完整性校验，不是代码签名。**
- This preview does not include an installer, auto-update, online holiday sync, calendar-account sync, telemetry, Explorer injection, taskbar hooks, HKLM writes, or administrator requirements.<br/>**此预览版不包含安装器、自动更新、在线节假日同步、日历账户同步、遥测、Explorer 注入、任务栏钩子、HKLM 写入或管理员权限要求。**
- Windows may place new tray icons in the hidden notification area; users may need to open the tray overflow and pin Dateview manually.<br/>**Windows 可能会把新的托盘图标放入隐藏通知区域；用户可能需要打开托盘溢出区并手动固定 Dateview。**
