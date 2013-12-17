; Setup_Npgsql.nsi

; For build servers
; - Get NSIS 2.46 and install it. http://nsis.sourceforge.net/Download
; - Get experienceui-1.3.1.exe and install it. http://nsis.sourceforge.net/ExperienceUI

; For editors
; - Get nisedit2.0.3.exe and install it. http://hmne.sourceforge.net/
; - Associate .nsi with "C:\Program Files (x86)\HMSoft\NIS Edit\nisedit.exe"

;--------------------------------

; !define VER "2.2.1.0"
; !define NET40
;   !define MONO_SECURITY
;   !define COMMON_LOGGING
; !define NET45
;   !define MONO_SECURITY
;   !define COMMON_LOGGING
; !define DDEX2010
; !define DDEX2012
; !define DDEX2013

!define APP "Npgsql"
!define COM "The Npgsql Development Team"

!ifndef VER
  !define VER "version-unknown"
!endif

!define TTL "Npgsql ${VER} - .Net Data Provider for Postgresql"

; The name of the installer
Name "${APP} ${VER}"

; The file to write
!ifdef OutFile
  OutFile ${OutFile}
!else
  OutFile "installer.exe" ; "Setup_${APP}-${VER}.exe"
!endif

; The default installation directory
InstallDir "$PROGRAMFILES\${COM}\${APP}"

; Registry key to check for directory (so if you install again, it will
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\${COM}\${APP}" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

!include "LogicLib.nsh"
!include "WordFunc.nsh"
!include "FileFunc.nsh"

; ExperienceUI
; http://nsis.sourceforge.net/ExperienceUI
; Download [experienceui-1.3.1.exe] (934 KB) and install it.
!include "XPUI.nsh"

Icon "${NSISDIR}\ExperienceUI\Contrib\Graphics\Icons\XPUI-install.ico"
Icon "${NSISDIR}\ExperienceUI\Contrib\Graphics\Icons\XPUI-uninstall.ico"

;--------------------------------

; Pages

  ${LicensePage} "License.rtf"
  ${Page} Components
  ${Page} Directory
  ${Page} InstFiles

  !insertmacro XPUI_PAGEMODE_UNINST
  !insertmacro XPUI_PAGE_UNINSTCONFIRM_NSIS
  !insertmacro XPUI_PAGE_COMPONENTS
  !insertmacro XPUI_PAGE_INSTFILES

;--------------------------------
;Languages

  !insertmacro XPUI_LANGUAGE "English"

;--------------------------------

!macro GetAssemblyName FilePath ; out $0
  nsExec::ExecToStack '"$INSTDIR\PrintAssemblyName.exe" "${FilePath}"'
  Pop $0
  ${If} $0 = 0
    Pop $0
  ${Else}
    Pop $0
    StrCpy $0 ""
  ${EndIf}
!macroend

Var FAC

; The stuff to install
Section ""
  SetOutPath "$INSTDIR"
  
  File /x "*.vshost.*" "tools\GACInstall.*"
  File /x "*.vshost.*" "tools\GACRemove.*"
  File /x "*.vshost.*" "tools\ModifyDbProviderFactories.*"
  File /x "*.vshost.*" "tools\PrintAssemblyName.*"

SectionEnd

!ifdef DDEX2013
Section ""
  SetOutPath "$INSTDIR\Npgsql-${VER}-net45-vs2013"
  File                "..\NpgsqlDdexProvider\bin\Release-net45-vs2013\NpgsqlDdexProvider.vsix" ;Vs2013
SectionEnd
!endif

!ifdef DDEX2012
Section ""
  SetOutPath "$INSTDIR\Npgsql-${VER}-net45-vs2012"
  File                "..\NpgsqlDdexProvider\bin\Release-net45-vs2012\NpgsqlDdexProvider.vsix" ;Vs2012
SectionEnd
!endif

!ifdef DDEX2010
Section ""
  SetOutPath "$INSTDIR\Npgsql-${VER}-net40-vs2010"
  File                "..\NpgsqlDdexProvider\bin\Release-net40-vs2010\NpgsqlDdexProvider.vsix" ;Vs2010
SectionEnd
!endif

!ifdef MONO_SECURITY
Section
  SetOutPath "$INSTDIR\lib"
  File /r             "..\lib\Mono.Security.dll"
SectionEnd
!endif

!ifdef COMMON_LOGGING
Section
  SetOutPath "$INSTDIR\packages"
  File /r             "..\packages\Common.Logging.dll"
  File /r             "..\packages\Common.Logging.Core.dll"
SectionEnd
!endif

Function LocateR0RetR1
  ${WordFind} $R9 $R0 "*" $0
  
  ${If} $0 > 0
    StrCpy $R1 $R9
    StrCpy $0 "StopLocate"
  ${EndIf}
  Push $0
FunctionEnd

!ifdef NET45

Section ""
  SetOutPath "$INSTDIR\Npgsql-${VER}-net45"
  File /r             "..\Npgsql\bin\Release-net45\Npgsql.dll"
  File                "..\Npgsql.EntityFramework\bin\Release-net45\Npgsql.EntityFramework.dll"
  File                "..\Npgsql.EntityFramework\bin\Release-net45\Npgsql.EntityFramework.dll.config"
  File                "..\Npgsql.EntityFramework\bin\Legacy-Release-net45\Npgsql.EntityFrameworkLegacy.dll"
SectionEnd

Section "Npgsql.dll (.NET4.5) to GAC" SecNpgsql ; net45
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net45\Npgsql.dll"' $0
  DetailPrint "RetCode: $0"
  
  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net45\Npgsql.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Npgsql.dll" "$0"
SectionEnd

Section "Npgsql.EntityFrameworkLegacy.dll (.NET4.5) to GAC" SecNpgsqlEFL ; net45
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net45\Npgsql.EntityFrameworkLegacy.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net45\Npgsql.EntityFrameworkLegacy.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Npgsql.EntityFrameworkLegacy.dll" "$0"
SectionEnd

!ifdef MONO_SECURITY
Section "Mono.Security.dll to GAC" SecMonoSecurity ; net45
  SetOutPath "$INSTDIR"

  StrCpy $R0 "4.0"
  StrCpy $R1 ""
  ${Locate} "$INSTDIR\lib" "/L=F /M=Mono.Security.dll /B=0" LocateR0RetR1

  ${If} ${FileExists} $R1
    ExecWait '"$INSTDIR\GACInstall.exe" "$R1"' $0
    DetailPrint "RetCode: $0"

    !insertmacro GetAssemblyName        "$R1"
    WriteRegStr HKLM "SOFTWARE\${COM}\${APP}" "Mono.Security.dll" "$0"
  ${EndIf}
SectionEnd
!endif

!ifdef COMMON_LOGGING
Section "Common.Logging to GAC" SecCommonLogging ; net45
  SetOutPath "$INSTDIR"

  StrCpy $R0 "net40"
  StrCpy $R1 ""
  ${Locate} "$INSTDIR\packages" "/L=F /M=Common.Logging.Core.dll /B=1" LocateR0RetR1
  ${If} ${FileExists} $R1
    ExecWait '"$INSTDIR\GACInstall.exe" "$R1"' $0
    DetailPrint "RetCode: $0"

    !insertmacro GetAssemblyName        "$R1"
    WriteRegStr HKLM "SOFTWARE\${COM}\${APP}" "Common.Logging.Core.dll" "$0"
  ${EndIf}

  StrCpy $R0 "net40"
  StrCpy $R1 ""
  ${Locate} "$INSTDIR\packages" "/L=F /M=Common.Logging.dll /B=1" LocateR0RetR1
  ${If} ${FileExists} $R1
    ExecWait '"$INSTDIR\GACInstall.exe" "$R1"' $0
    DetailPrint "RetCode: $0"

    !insertmacro GetAssemblyName        "$R1"
    WriteRegStr HKLM "SOFTWARE\${COM}\${APP}" "Common.Logging.dll" "$0"
  ${EndIf}
SectionEnd
!endif

!else ifdef NET40

Section ""
  SetOutPath "$INSTDIR\Npgsql-${VER}-net40"
  File /r             "..\Npgsql\bin\Release-net40\Npgsql.dll"
  File                "..\Npgsql.EntityFramework\bin\Release-net40\Npgsql.EntityFramework.dll"
  File                "..\Npgsql.EntityFramework\bin\Release-net40\Npgsql.EntityFramework.dll.config"
  File                "..\Npgsql.EntityFramework\bin\Legacy-Release-net40\Npgsql.EntityFrameworkLegacy.dll"
SectionEnd

Section "Npgsql.dll (.NET4.0) to GAC" SecNpgsql ; net40
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net40\Npgsql.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net40\Npgsql.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Npgsql.dll" "$0"
SectionEnd

Section "Npgsql.EntityFrameworkLegacy.dll (.NET4.0) to GAC" SecNpgsqlEFL ; net40
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net40\Npgsql.EntityFrameworkLegacy.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net40\Npgsql.EntityFrameworkLegacy.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Npgsql.EntityFrameworkLegacy.dll" "$0"
SectionEnd


!ifdef MONO_SECURITY
Section "Mono.Security.dll to GAC" SecMonoSecurity ; net40
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net40\Mono.Security.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net40\Mono.Security.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Mono.Security.dll" "$0"
SectionEnd
!endif

!ifdef COMMON_LOGGING
Section "Common.Logging to GAC" SecCommonLogging ; net40
  SetOutPath "$INSTDIR"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net40\Common.Logging.Core.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net40\Common.Logging.Core.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Common.Logging.Core.dll" "$0"

  ExecWait '"$INSTDIR\GACInstall.exe" "$INSTDIR\Npgsql-${VER}-net40\Common.Logging.dll"' $0
  DetailPrint "RetCode: $0"

  !insertmacro GetAssemblyName        "$INSTDIR\Npgsql-${VER}-net40\Common.Logging.dll"
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}"                        "Common.Logging.dll" "$0"
SectionEnd
!endif

!endif

!ifdef NET40 || NET45

Section "Npgsql DbProviderFactory to machine.config" SecMachineConfig
  SetOutPath "$INSTDIR"

  ReadRegStr $0 HKLM "SOFTWARE\${COM}\${APP}" "Npgsql.dll"
  StrCpy $FAC "Npgsql.NpgsqlFactory, $0"
  WriteRegStr   HKLM "SOFTWARE\${COM}\${APP}" "Npgsql.NpgsqlFactory" "$FAC"

  ${If} ${FileExists} "$WINDIR\Microsoft.NET\Framework\v4.0.30319\Config\machine.config"
    StrCpy $0 '"$INSTDIR\ModifyDbProviderFactories.exe"'
    StrCpy $0 '$0 "/add-or-replace"'
    StrCpy $0 '$0 "$WINDIR\Microsoft.NET\Framework\v4.0.30319\Config\machine.config"'
    StrCpy $0 '$0 "Npgsql Data Provider"'
    StrCpy $0 '$0 "Npgsql"'
    StrCpy $0 '$0 ".Net Data Provider for PostgreSQL"'
    StrCpy $0 '$0 "$FAC"'
    StrCpy $0 '$0 "support"'
    StrCpy $0 '$0 "FF"'

    ExecWait '$0' $1
    DetailPrint "RetCode: $1"
  ${EndIf}

  ${If} ${FileExists} "$WINDIR\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config"
    StrCpy $0 '"$INSTDIR\ModifyDbProviderFactories.exe"'
    StrCpy $0 '$0 "/add-or-replace"'
    StrCpy $0 '$0 "$WINDIR\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config"'
    StrCpy $0 '$0 "Npgsql Data Provider"'
    StrCpy $0 '$0 "Npgsql"'
    StrCpy $0 '$0 ".Net Data Provider for PostgreSQL"'
    StrCpy $0 '$0 "$FAC"'
    StrCpy $0 '$0 "support"'
    StrCpy $0 '$0 "FF"'

    ExecWait '$0' $1
    DetailPrint "RetCode: $1"
  ${EndIF}
SectionEnd

!endif

!ifdef DDEX2013

Section "NpgsqlDdexProvider(Vs2013)" SecDdex2013
  SetOutPath "$INSTDIR"

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" "$INSTDIR\Npgsql-${VER}-net45-vs2013\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2012

Section "NpgsqlDdexProvider(Vs2012)" SecDdex2012
  SetOutPath "$INSTDIR"

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" "$INSTDIR\Npgsql-${VER}-net45-vs2012\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2010

Section "NpgsqlDdexProvider(Vs2010)" SecDdex2010
  SetOutPath "$INSTDIR"

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" "$INSTDIR\Npgsql-${VER}-net40-vs2010\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

Section ""
  SetOutPath "$INSTDIR"

  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\${COM}\${APP}" "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "DisplayIcon" "$INSTDIR\uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "DisplayName" "${TTL}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "DisplayVersion" "${VER}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "Publisher" "${COM}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

;--------------------------------
;Uninstaller Section

!ifdef DDEX2013

Section "un.Remove NpgsqlDdexProvider(Vs2013)" UnDdex2013
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2012

Section "un.Remove NpgsqlDdexProvider(Vs2012)" UnDdex2012
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2010

Section "un.Remove NpgsqlDdexProvider(Vs2010)" UnDdex2010
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef NET40 || NET45

Section "un.Remove Npgsql.dll from GAC" UnNpgsql
  ; Npgsql.dll
  ReadRegStr $2 HKLM "SOFTWARE\${COM}\${APP}" "Npgsql.dll"
  
  ${If} $2 != ""
    ExecWait '"$INSTDIR\GACRemove.exe" "$2"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

Section "un.Remove Npgsql.EntityFrameworkLegacy.dll from GAC" UnNpgsqlEFL
  ; Npgsql.dll
  ReadRegStr $2 HKLM "SOFTWARE\${COM}\${APP}" "Npgsql.EntityFrameworkLegacy.dll"

  ${If} $2 != ""
    ExecWait '"$INSTDIR\GACRemove.exe" "$2"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!ifdef COMMON_LOGGING
Section "un.Remove Common.Logging from GAC" UnLoggingCommon
  ; Common.Logging.dll
  StrCpy $2 ""
  ReadRegStr $2 HKLM "SOFTWARE\${COM}\${APP}" "Common.Logging.dll"

  ${If} $2 != ""
    ExecWait '"$INSTDIR\GACRemove.exe" "$2"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ; Common.Logging.Core.dll
  StrCpy $2 ""
  ReadRegStr $2 HKLM "SOFTWARE\${COM}\${APP}" "Common.Logging.Core.dll"

  ${If} $2 != ""
    ExecWait '"$INSTDIR\GACRemove.exe" "$2"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd
!endif

!ifdef MONO_SECURITY
Section "un.Remove Mono.Security.dll from GAC" UnMonoSecurity
  ; Mono.Security
  StrCpy $2 ""
  ReadRegStr $2 HKLM "SOFTWARE\${COM}\${APP}" "Mono.Security.dll"

  ${If} $2 != ""
    ExecWait '"$INSTDIR\GACRemove.exe" "$2"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd
!endif

Section "un.Remove Npgsql DbProviderFactory from machine.config" UnMachineConfig
  ${If} ${FileExists}                                            "$WINDIR\Microsoft.NET\Framework\v4.0.30319\Config\machine.config"
    ExecWait '"$INSTDIR\ModifyDbProviderFactories.exe" "/remove" "$WINDIR\Microsoft.NET\Framework\v4.0.30319\Config\machine.config" "Npgsql"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists}                                            "$WINDIR\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config"
    ExecWait '"$INSTDIR\ModifyDbProviderFactories.exe" "/remove" "$WINDIR\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config" "Npgsql"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

SectionEnd

!endif

Section "un."
  ; Remove files
  Delete "$INSTDIR\GACInstall.*"
  Delete "$INSTDIR\GACRemove.*"
  Delete "$INSTDIR\ModifyDbProviderFactories.*"
  Delete "$INSTDIR\PrintAssemblyName.*"

  Delete "$INSTDIR\Npgsql-${VER}-net45\*.dll"
  RMDir  "$INSTDIR\Npgsql-${VER}-net45"

  Delete "$INSTDIR\Npgsql-${VER}-net45-vs2013\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net45-vs2013"
  
  Delete "$INSTDIR\Npgsql-${VER}-net45-vs2012\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net45-vs2012"

  Delete "$INSTDIR\Npgsql-${VER}-net40-vs2010\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net40-vs2010"

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP}"
  DeleteRegKey HKLM "SOFTWARE\${COM}\${APP}"

  ; Remove uninstaller
  Delete "$INSTDIR\uninstall.exe"

  ; Remove directories used
  RMDir "$INSTDIR"
SectionEnd


;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecNpgsql        ${LANG_ENGLISH} "Install Npgsql.dll into your GAC. $\nIt is useful for example: $\n- Npgsql DDEX provider for Visual Studio $\n- Microsoft Power Query for Excel"
  LangString DESC_SecNpgsqlEFL     ${LANG_ENGLISH} "Install Npgsql.EntityFrameworkLegacy.dll into your GAC. $\nIt is useful for example: $\n- Npgsql DDEX provider for Visual Studio"
  LangString DESC_SecMonoSecurity  ${LANG_ENGLISH} "Install Mono.Security.dll (required by Npgsql.dll) into your GAC. "
  LangString DESC_SecCommonLogging ${LANG_ENGLISH} "Install Common.Logging (required by Npgsql.dll) into your GAC. "
  LangString DESC_SecMachineConfig ${LANG_ENGLISH} "Install Npgsql DbProviderFactory to machine.config. $\nIt is useful for example: $\n- Npgsql DDEX provider for Visual Studio $\n- Microsoft Power Query for Excel"
  LangString DESC_SecDdex2013      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2013"
  LangString DESC_SecDdex2012      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2012"
  LangString DESC_SecDdex2010      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2010"

  LangString DESC_UnNpgsql        ${LANG_ENGLISH} "Uninstall Npgsql.dll from your GAC."
  LangString DESC_UnMonoSecurity  ${LANG_ENGLISH} "Uninstall Mono.Security.dll from your GAC."
  LangString DESC_UnCommonLogging ${LANG_ENGLISH} "Uninstall Common.Logging from your GAC."
  LangString DESC_UnMachineConfig ${LANG_ENGLISH} "Uninstall Npgsql DbProviderFactory from machine.config."
  LangString DESC_UnDdex2013      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2013"
  LangString DESC_UnDdex2012      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2012"
  LangString DESC_UnDdex2010      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2010"

  ;Assign language strings to sections
  !insertmacro XPUI_FUNCTION_DESCRIPTION_BEGIN
  !ifdef NET40 || NET45
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecNpgsql}        $(DESC_SecNpgsql)
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecNpgsqlEFL}     $(DESC_SecNpgsqlEFL)
    !ifdef MONO_SECURITY
      !insertmacro XPUI_DESCRIPTION_TEXT ${SecMonoSecurity}  $(DESC_SecMonoSecurity)
    !endif
    !ifdef COMMON_LOGGING
      !insertmacro XPUI_DESCRIPTION_TEXT ${SecCommonLogging} $(DESC_SecCommonLogging)
    !endif
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecMachineConfig} $(DESC_SecMachineConfig)
  !endif
  !ifdef DDEX2013
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecDdex2013}      $(DESC_SecDdex2013)
  !endif
  !ifdef DDEX2012
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecDdex2012}      $(DESC_SecDdex2012)
  !endif
  !ifdef DDEX2010
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecDdex2010}      $(DESC_SecDdex2010)
  !endif
  !insertmacro XPUI_FUNCTION_DESCRIPTION_END

    ;!insertmacro XPUI_DESCRIPTION_TEXT ${UnNpgsql}        $(DESC_UnNpgsql)
    ;!insertmacro XPUI_DESCRIPTION_TEXT ${UnMonoSecurity}  $(DESC_UnMonoSecurity)
    ;!insertmacro XPUI_DESCRIPTION_TEXT ${UnMachineConfig} $(DESC_UnMachineConfig)
    ;!insertmacro XPUI_DESCRIPTION_TEXT ${UnDdex2013}      $(DESC_UnDdex2013)
    ;!insertmacro XPUI_DESCRIPTION_TEXT ${UnDdex2010}      $(DESC_UnDdex2010)
