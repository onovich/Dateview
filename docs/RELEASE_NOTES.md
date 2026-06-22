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
- Verify a bundle by comparing `Get-FileHash -Algorithm SHA256 .\Dateview-0.1.0-preview-win-x64.zip` with the matching `.sha256.txt` file. The exact validated artifact hash is recorded in `docs\P9_RELEASE_DISTRIBUTION_VALIDATION.md`.<br/>**校验发布包时，请将 `Get-FileHash -Algorithm SHA256 .\Dateview-0.1.0-preview-win-x64.zip` 的结果与配套 `.sha256.txt` 文件对比。精确的已验证产物哈希记录在 `docs\P9_RELEASE_DISTRIBUTION_VALIDATION.md`。**
- Because this preview is unsigned, Windows may show unknown-publisher, SmartScreen, or Microsoft Defender prompts. Keep Windows security features enabled; run only builds from a trusted source whose hash matches the expected value.<br/>**由于此预览版未签名，Windows 可能显示未知发布者、SmartScreen 或 Microsoft Defender 提示。请保持 Windows 安全功能开启；只运行来自可信来源且哈希匹配预期值的构建。**
- This preview does not include an installer, auto-update, online holiday sync, calendar-account sync, telemetry, Explorer injection, taskbar hooks, HKLM writes, or administrator requirements.<br/>**此预览版不包含安装器、自动更新、在线节假日同步、日历账户同步、遥测、Explorer 注入、任务栏钩子、HKLM 写入或管理员权限要求。**
- Windows may place new tray icons in the hidden notification area; users may need to open the tray overflow and pin Dateview manually.<br/>**Windows 可能会把新的托盘图标放入隐藏通知区域；用户可能需要打开托盘溢出区并手动固定 Dateview。**
- Holiday data coverage in this preview is limited to the bundled 2025 and 2026 China JSON files.<br/>**此预览版的节假日数据范围仅限内置的中国 2025 和 2026 年 JSON 文件。**

### Validation Status

- Release validation passed for build/test/format, repeatable bundle generation, SHA256 generation, temporary unzip/run smoke, second-instance exit, relative holiday-data loading, and settings/startup cleanup.<br/>**发布验证已覆盖并通过：构建/测试/格式检查、可重复打包、SHA256 生成、临时目录解压运行、第二实例退出、相对路径节假日数据加载，以及设置/开机启动清理。**
- Physical multi-monitor and non-100% DPI spot checks remain validation coverage risks from P8 because this machine exposed only one display at 96 DPI during the QA run. Geometry tests and single-monitor live smoke passed; repeat those spot checks on hardware with multiple monitors or alternate scaling before a broader public release.<br/>**物理多显示器和非 100% DPI 抽查是 P8 留下的验证覆盖风险：本机 QA 时只有一个 96 DPI 显示器。几何测试和单显示器真实 smoke 已通过；在更广泛公开发布前，应在多显示器或不同缩放硬件上重复抽查。**
