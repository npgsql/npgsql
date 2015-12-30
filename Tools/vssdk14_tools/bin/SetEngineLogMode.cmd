@echo off
setlocal

if /i "%~1"=="" goto Help
if /i "%~1"=="-?" goto Help
if /i "%~1"=="/?" goto Help

:: Change this for SxS
set VSVersion=14.0

set MainRegRoot=HKLM\SOFTWARE\Microsoft\VisualStudio\%VSVersion%
set OptionalRegRoots=HKCU\SOFTWARE\Microsoft\VisualStudio\%VSVersion%_Config HKCU\SOFTWARE\Microsoft\VisualStudio\%VSVersion%Exp_Config HKLM\SOFTWARE\Microsoft\VSWinExpress\%VSVersion% HKCU\SOFTWARE\Microsoft\VSWinExpress\%VSVersion%_Config HKLM\SOFTWARE\Microsoft\Glass\%VSVersion%

set reg_exe=reg.exe

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" set reg_exe=%windir%\syswow64\reg.exe

if /i "%~1"=="default" set EngineDiagEnableState=0& goto Set
if /i "%~1"=="error" set EngineDiagEnableState=1& goto Set
if /i "%~1"=="method" set EngineDiagEnableState=2& goto Set
if /i "%~1"=="dump" goto Dump

echo ERROR: Unknown argument '%1'.
exit /b -1

:Set
call :SetRegRoot %MainRegRoot%
for %%r in (%OptionalRegRoots%) do call :SetForRegRootIfExists %%r
exit /b 0

:SetForRegRootIfExists
REM check if the registry root exists
call %reg_exe% query %1 /ve 1>NUL 2>NUL
if NOT %ERRORLEVEL%==0 goto eof

:SetRegRoot
call %reg_exe% add %1\Debugger /v EngineDiagEnableState /t REG_DWORD /d %EngineDiagEnableState% /f
if not %ERRORLEVEL%==0 echo ERROR: failed to write to the registry (%1) & exit /b -1
goto eof

:Dump
echo.
echo Listing current settings (0=default, 1=error, 2=method)...
echo ----------------------------------------------------------
for %%r in (%MainRegRoot% %OptionalRegRoots%) do call call %reg_exe% query %%r\Debugger /v EngineDiagEnableState 2>NUL
exit /b 0

:Help
echo SetEngineLogMode.cmd ^<default ^| error ^| method ^| dump^>
echo.
echo SetEngineLogMode.cmd is used to configure the logging mode used by 
echo vsdebugeng.dll. Three modes of operation are available:
echo.
echo 'default' mode - significant errors are set to the debug output when a 
echo debugger is attached to Visual Studio (or whatever process has loaded 
echo vsdebugeng). Default mode is the behavior in a clean install of Visual 
echo Studio.
echo.
echo 'error' mode - significant errors are set to %%tmp%%\vsdebugeng.dll.log if no
echo debugger is attached to visual studio. If a debugger is attached to Visual 
echo Studio, default mode and error mode are the same.
echo.
echo 'method' mode - In addition to the tracing in 'error' mode, method entry/exit
echo tracing is provided. Four different types of output are sent -- 'CALL' lines 
echo are used when an implementation was successfully loaded and is about to be 
echo called. 'RETURN' lines are used when a called component returns successfully.
echo If the operation fails, a 'RETURN ERROR' line will appear instead. If no 
echo implementation could be found or if the implementation fails to load a 'CALL 
echo ERROR' line will appear. In addition to this, after a CALL/CALL ERROR line, 
echo there are 'Skipped' lines explaining why components are not called.
echo.
echo 'dump' mode displays the current log mode.
echo.
echo Example method logging output:
echo CALL: IDkmRuntimeMonitorBreakpointHandler.EnableRuntimeBreakpoint (ThreadId=6080 Class=Win32BDM.CBaseDebugMonitor IP=0x577D3B00 Object=0x10BD4F90 TickCount=217496250 ComponentId={F50FC269-4428-4680-8A45-462B8C5D37CD})
echo   Skipped: ManagedDM::CCommonEntryPoint {38A59583-E6B1-4EE4-A53C-133BE0F45E55} for object visibility
echo   Skipped: MinidumpBDM.CBaseDebugMonitor {016426E1-A85F-4CAB-941A-7C5EB5C82DC4} for 'BaseDebugMonitorId' filter
echo RETURN: IDkmRuntimeMonitorBreakpointHandler.EnableRuntimeBreakpoint (hr = 0x0, ThreadId=6080, TickCount=217496250)
echo.
:eof
