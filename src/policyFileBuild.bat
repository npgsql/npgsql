@if "%WindowsSdkDir_35%"=="" goto NO_WIN_SDK

@setlocal

@set net20out=bin\policies\net20
@set net40out=bin\policies\net40

@if not exist %net20out% mkdir %net20out%
@if not exist %net40out% mkdir %net40out%

@"%WindowsSdkDir_35%\al.exe" /nologo /link:policy.2.0.Npgsql.config /out:%net20out%\policy.2.0.Npgsql.dll /keyfile:Npgsql\Npgsql.snk
@"%WindowsSdkDir_35%\NETFX 4.0 Tools\al.exe" /nologo /link:policy.2.0.Npgsql.config /out:%net40out%\policy.2.0.Npgsql.dll /keyfile:Npgsql\Npgsql.snk
@goto :EOF

:NO_WIN_SDK
@echo ==========================================================
@echo ERROR:
@echo Please make sure that an environment variable WindowsSdkDir_35 is set.
@echo This variable should point to the SDK dir which contains al.exe for clr 2.0.
@echo You can set this variable with launching command shell via Windows SDK Comamnd 
@echo Prompt or Visual Studio Command Prompt.
@echo ==========================================================
@pause