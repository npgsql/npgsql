; Setup_Npgsql.nsi

; For build servers
; - Get NSIS 2.46 and install it. http://nsis.sourceforge.net/Download
; - Get experienceui-1.3.1.exe and install it. http://nsis.sourceforge.net/ExperienceUI

; For editors
; - Get nisedit2.0.3.exe and install it. http://hmne.sourceforge.net/
; - Associate .nsi with "C:\Program Files (x86)\HMSoft\NIS Edit\nisedit.exe"

;--------------------------------

; !define VER "3.0.3.0"
; !define OutFile "installer.exe"
; !define DDEX2012
; !define DDEX2013
; !define DDEX2015

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

; The stuff to install
Section ""
  SetOutPath "$INSTDIR"

  File /x "*.vshost.*" "tools\ModifyDbProviderFactories.*"

SectionEnd

!ifdef DDEX2015 | DDEX2013 | DDEX2012

Section "Uninstall NpgsqlDdexProvider at first" SecUninstDdex
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"

  ${If} 1 = 1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}
  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  
  ${If} 1 = 1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}
  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  
  ${If} 1 = 1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
  ${EndIf}
  ${If} ${FileExists} $1
    ExecWait '"$1" /u:b803e32d-de1d-4e62-953b-1ed2013e8738 /v:11.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:b803e32d-de1d-4e62-953b-1ed2013e8738 /v:11.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:b803e32d-de1d-4e62-953b-1ed2013e8738 /v:11.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:b803e32d-de1d-4e62-953b-1ed2013e8738 /v:11.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"

    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

SectionEnd

!endif

!ifdef DDEX2015

Section "NpgsqlDdexProvider(Vs2015)" SecDdex2015
  SetOutPath "$INSTDIR\Npgsql-${VER}-net452-vs2015"
  File                  "bin\Release-net452-vs2015\NpgsqlDdexProvider.vsix" ;Vs2015

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"

  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
    StrCpy $2 "$0devenv.exe.config"
    StrCpy $3 "$0Extensions\Microsoft\Entity Framework Tools\DBGen"
  ${EndIf}

  ${If} ${FileExists} $1
    DetailPrint "Install NpgsqlDdexProvider(Vs2015) by VSIXInstaller."
    ExecWait '"$1" "$OUTDIR\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    StrCpy $0 '"$INSTDIR\ModifyDbProviderFactories.exe"'
    StrCpy $0 '$0 "/add-or-replace"'
    StrCpy $0 '$0 "$2"'
    StrCpy $0 '$0 "Npgsql Data Provider"'
    StrCpy $0 '$0 "Npgsql"'
    StrCpy $0 '$0 ".Net Data Provider for PostgreSQL"'
    StrCpy $0 '$0 "Npgsql.NpgsqlFactory, Npgsql"'
    StrCpy $0 '$0 "support"'
    StrCpy $0 '$0 "FF"'

    DetailPrint "Modify devenv.exe.config."
    ExecWait '$0' $1
    DetailPrint "RetCode: $1"
  ${EndIf}

  ${If} ${FileExists} $3
    File "/oname=$3\SSDLToPgSQL.tt" "SSDLToPgSQL.tt"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2013

Section "NpgsqlDdexProvider(Vs2013)" SecDdex2013
  SetOutPath "$INSTDIR\Npgsql-${VER}-net452-vs2013"
  File                  "bin\Release-net452-vs2013\NpgsqlDdexProvider.vsix" ;Vs2013

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"

  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
    StrCpy $2 "$0devenv.exe.config"
    StrCpy $3 "$0Extensions\Microsoft\Entity Framework Tools\DBGen"
  ${EndIf}

  ${If} ${FileExists} $1
    DetailPrint "Install NpgsqlDdexProvider(Vs2013) by VSIXInstaller."
    ExecWait '"$1" "$OUTDIR\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    StrCpy $0 '"$INSTDIR\ModifyDbProviderFactories.exe"'
    StrCpy $0 '$0 "/add-or-replace"'
    StrCpy $0 '$0 "$2"'
    StrCpy $0 '$0 "Npgsql Data Provider"'
    StrCpy $0 '$0 "Npgsql"'
    StrCpy $0 '$0 ".Net Data Provider for PostgreSQL"'
    StrCpy $0 '$0 "Npgsql.NpgsqlFactory, Npgsql"'
    StrCpy $0 '$0 "support"'
    StrCpy $0 '$0 "FF"'

    DetailPrint "Modify devenv.exe.config."
    ExecWait '$0' $1
    DetailPrint "RetCode: $1"
  ${EndIf}

  ${If} ${FileExists} $3
    File "/oname=$3\SSDLToPgSQL.tt" "SSDLToPgSQL.tt"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2012

Section "NpgsqlDdexProvider(Vs2012)" SecDdex2012
  SetOutPath "$INSTDIR\Npgsql-${VER}-net452-vs2012"
  File                  "bin\Release-net452-vs2012\NpgsqlDdexProvider.vsix" ;Vs2012

  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"

  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
    StrCpy $2 "$0devenv.exe.config"
    StrCpy $3 "$0Extensions\Microsoft\Entity Framework Tools\DBGen"
  ${EndIf}

  ${If} ${FileExists} $1
    DetailPrint "Install NpgsqlDdexProvider(Vs2012) by VSIXInstaller."
    ExecWait '"$1" "$OUTDIR\NpgsqlDdexProvider.vsix"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    StrCpy $0 '"$INSTDIR\ModifyDbProviderFactories.exe"'
    StrCpy $0 '$0 "/add-or-replace"'
    StrCpy $0 '$0 "$2"'
    StrCpy $0 '$0 "Npgsql Data Provider"'
    StrCpy $0 '$0 "Npgsql"'
    StrCpy $0 '$0 ".Net Data Provider for PostgreSQL"'
    StrCpy $0 '$0 "Npgsql.NpgsqlFactory, Npgsql"'
    StrCpy $0 '$0 "support"'
    StrCpy $0 '$0 "FF"'

    DetailPrint "Modify devenv.exe.config."
    ExecWait '$0' $1
    DetailPrint "RetCode: $1"
  ${EndIf}

  ${If} ${FileExists} $3
    File "/oname=$3\SSDLToPgSQL.tt" "SSDLToPgSQL.tt"
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

Function .onInit
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0" "InstallDir"
  ${IfNot} ${FileExists} "$0\*"
    SectionSetFlags ${SecDdex2015} 0 ; uncheck item
  ${EndIf}

  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
  ${IfNot} ${FileExists} "$0\*"
    SectionSetFlags ${SecDdex2013} 0 ; uncheck item
  ${EndIf}

  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
  ${IfNot} ${FileExists} "$0\*"
    SectionSetFlags ${SecDdex2012} 0 ; uncheck item
  ${EndIf}
FunctionEnd

;--------------------------------
;Uninstaller Section

!ifdef DDEX2015

Section "un.Remove NpgsqlDdexProvider(Vs2015)" UnDdex2015
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
    StrCpy $2 "$0devenv.exe.config"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:14.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    ExecWait '"$INSTDIR\ModifyDbProviderFactories.exe" "/remove" "$2" "Npgsql"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

!ifdef DDEX2013

Section "un.Remove NpgsqlDdexProvider(Vs2013)" UnDdex2013
  StrCpy $1 "$INSTDIR\VSIXInstaller.exe"
  ${IfNot} ${FileExists} $1
    StrCpy $0 ""
    ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
    StrCpy $1 "$0VSIXInstaller.exe"
    StrCpy $2 "$0devenv.exe.config"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:12.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    ExecWait '"$INSTDIR\ModifyDbProviderFactories.exe" "/remove" "$2" "Npgsql"' $0
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
    StrCpy $2 "$0devenv.exe.config"
  ${EndIf}

  ${If} ${FileExists} $1
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Pro /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:IntegratedShell /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Premium /q' $0
    DetailPrint "RetCode: $0"
    ExecWait '"$1" /u:958b9481-2712-4670-9a62-8fe65e5beea7 /v:11.0 /s:Ultimate /q' $0
    DetailPrint "RetCode: $0"
  ${EndIf}

  ${If} ${FileExists} $2
    ExecWait '"$INSTDIR\ModifyDbProviderFactories.exe" "/remove" "$2" "Npgsql"' $0
    DetailPrint "RetCode: $0"
  ${EndIf}
SectionEnd

!endif

Section "un."
  ; Remove files
  Delete "$INSTDIR\ModifyDbProviderFactories.*"

  Delete "$INSTDIR\Npgsql-${VER}-net452-vs2015\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net452-vs2015"

  Delete "$INSTDIR\Npgsql-${VER}-net452-vs2013\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net452-vs2013"

  Delete "$INSTDIR\Npgsql-${VER}-net452-vs2012\NpgsqlDdexProvider.vsix"
  RMDir  "$INSTDIR\Npgsql-${VER}-net452-vs2012"

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
  LangString DESC_SecDdex2015      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2015"
  LangString DESC_SecDdex2013      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2013"
  LangString DESC_SecDdex2012      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2012"
  LangString DESC_SecDdex2010      ${LANG_ENGLISH} "Install Npgsql DDEX provider for Visual Studio 2010"
  LangString DESC_SecUninstDdex    ${LANG_ENGLISH} "unInstall Npgsql DDEX provider at first, so that it can install same version."

  LangString DESC_UnDdex2015      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2015"
  LangString DESC_UnDdex2013      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2013"
  LangString DESC_UnDdex2012      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2012"
  LangString DESC_UnDdex2010      ${LANG_ENGLISH} "Uninstall Npgsql DDEX provider for Visual Studio 2010"

  ;Assign language strings to sections
  !insertmacro XPUI_FUNCTION_DESCRIPTION_BEGIN
  !ifdef DDEX2015 | DDEX2013 | DDEX2012
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecUninstDdex}    $(DESC_SecUninstDdex)
  !endif

  !ifdef DDEX2015
    !insertmacro XPUI_DESCRIPTION_TEXT ${SecDdex2015}      $(DESC_SecDdex2015)
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
