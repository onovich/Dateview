# Troubleshooting

## Tray Icon

- Windows may place new tray icons in the hidden notification area; open the tray overflow and pin Dateview if needed.<br/>**Windows 可能会把新托盘图标放进隐藏通知区域；如有需要，请展开托盘溢出区并固定 Dateview。**
- If the app is already running, a second launch exits immediately by design.<br/>**如果应用已经运行，第二次启动会按设计立即退出。**

## Popup

- Left-click the tray icon to toggle the popup; Escape and outside activation hide it without closing the app.<br/>**左键点击托盘图标可切换弹窗；Escape 和外部激活会隐藏弹窗，但不会退出应用。**
- The popup is borderless and `ShowInTaskbar=False`, so it should not appear as a normal taskbar button.<br/>**弹窗是无边框窗口并设置了 `ShowInTaskbar=False`，因此不应显示为普通任务栏按钮。**

## DPI And Multi-Monitor

- The Desktop project uses `ApplicationHighDpiMode=PerMonitorV2` and places the popup near the clicked tray point within the current monitor working area.<br/>**Desktop 项目使用 `ApplicationHighDpiMode=PerMonitorV2`，并根据点击的托盘位置把弹窗放在当前显示器工作区内。**
- If placement looks wrong after changing display scale, restart the app so the process can re-read monitor DPI state cleanly.<br/>**如果调整显示缩放后弹窗位置异常，请重启应用，让进程重新读取显示器 DPI 状态。**

## Settings

- Settings are stored at `%APPDATA%\ChinaTrayCalendar\settings.json`.<br/>**设置保存在 `%APPDATA%\ChinaTrayCalendar\settings.json`。**
- Corrupt JSON is surfaced as a settings load failure and should be fixed or removed; a missing file falls back to defaults.<br/>**损坏的 JSON 会显示为设置加载失败，需要修复或删除；缺失文件会回退到默认设置。**

## Startup

- Start with Windows writes only the current user's Run key: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.<br/>**“开机自动启动”只写入当前用户的 Run 注册表项：`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`。**
- The app does not require administrator rights and does not write HKLM.<br/>**应用不需要管理员权限，也不会写入 HKLM。**

## Portable Bundle And Cleanup

- The portable zip should be extracted before running; do not run the executable directly from inside the zip viewer.<br/>**便携版 zip 应先解压再运行；不要直接从 zip 查看器中启动 exe。**
- Keep the `Dateview\assets\holidays\cn\` folder beside the executable. Missing holiday JSON files can prevent offline holiday data from loading.<br/>**请保留 exe 旁边的 `Dateview\assets\holidays\cn\` 目录。缺少节假日 JSON 文件会导致离线节假日数据无法加载。**
- Prefer a normal user-writable folder such as `%LOCALAPPDATA%\Programs\Dateview`; protected locations such as `C:\Program Files` may require administrator rights for copying or cleanup.<br/>**建议解压到普通用户可写目录，例如 `%LOCALAPPDATA%\Programs\Dateview`；`C:\Program Files` 等受保护位置可能在复制或清理时要求管理员权限。**
- If deleting the portable folder fails because files are in use, right-click the tray icon and choose Exit, then confirm no `ChinaTrayCalendar.Desktop.exe` process remains.<br/>**如果删除便携目录时提示文件占用，请先右键托盘图标选择“退出”，再确认没有残留的 `ChinaTrayCalendar.Desktop.exe` 进程。**
- If the app was moved after Start with Windows was enabled, turn the startup toggle off and on again so the HKCU Run value points to the new executable path.<br/>**如果启用“开机启动”后移动了应用目录，请关闭再重新打开该开关，让 HKCU Run 值指向新的 exe 路径。**
- The `.sha256.txt` file is an integrity check for the zip, not a code signature. Use it only to confirm the downloaded zip matches the expected hash.<br/>**`.sha256.txt` 是 zip 完整性校验，不是代码签名；它只用于确认下载到的 zip 与预期哈希一致。**

## Holiday Data

- Holiday data is bundled under `assets\holidays\cn\{year}.json`; update it from official yearly notices only.<br/>**节假日数据随应用打包在 `assets\holidays\cn\{year}.json`；只能根据官方年度通知更新。**
- Do not infer adjusted workdays from formulas; add the official dates explicitly and keep source metadata.<br/>**不要用公式推断调休工作日；应显式写入官方日期并保留来源元数据。**
