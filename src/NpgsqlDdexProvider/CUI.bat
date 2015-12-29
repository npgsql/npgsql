(
msbuild Build-nsis-installer.csproj /p:Configuration=Release 
) && (
echo Uninst
"H:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\VSIXInstaller.exe" /q /u:958b9481-2712-4670-9a62-8fe65e5beea7
) && (
echo Inst
"H:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\VSIXInstaller.exe" /q bin\Release-net452-vs2015\NpgsqlDdexProvider.vsix
) && (
echo Done
)