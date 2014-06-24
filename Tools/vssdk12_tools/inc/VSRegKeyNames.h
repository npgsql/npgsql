//
// Macros to deal with wide char string-constants using defined symbols
//
#pragma once

#include "RegistryRoots.h"

#ifndef __TOL
#define _TOL(x)  L##x
#endif
#ifndef __L
#define __L(x)   _TOL(x)
#endif

//
// Registry key/value names used by the Environment
//
// All uses of these macros should use __L(x) so that the strings are UNICODE.
//




#define RGS_VAR_PROGID_APPIDNAME      L"VisualStudio"
#define RGS_VAR_PROGID_VERSION        L"12.0"
#define RGS_VAR_DDE_NAME              L"VisualStudio.12.0"
#define RGS_VAR_EXE_NAME              L"devenv.exe" //used for OpenWithList
#define REGKEY_VISUALSTUDIOROOT_NOVERSION    "Software\\Microsoft\\VisualStudio"
#define LREGKEY_VISUALSTUDIOROOT_NOVERSION   L"Software\\Microsoft\\VisualStudio"
#define REGKEY_VISUALSTUDIOROOT      REGKEY_VISUALSTUDIOROOT_CURRENT
#define LREGKEY_VISUALSTUDIOROOT     LREGKEY_VISUALSTUDIOROOT_CURRENT
#define RGS_VISUALSTUDIOROOT_BEGIN   L"NoRemove Software \n{\n NoRemove Microsoft \n{\n NoRemove VisualStudio \n{\n NoRemove 12.0 \n{\n"
#define RGS_VISUALSTUDIOROOT_END     L"\n}\n \n}\n \n}\n \n}\n"
#define REGKEY_VSEROOT               "Software\\Microsoft\\MSE\\12.0"
#define REGKEY_PACKAGES              "Packages"
#define REGVALUE_INPROCSERVER32      "InprocServer32"
#define LREGVALUE_INPROCSERVER32     L"InProcServer32"
#define REGKEY_SATELLITEDLL          "SatelliteDll"
#define REGVALUE_SATELLITEDLLNAME    "DllName"
#define REGVALUE_SATELLITEDLLPATH    "Path"
#define REGKEY_AUTOMATION            "Automation"
#define REGKEY_AUTOMATIONEVENTS      "AutomationEvents"
#define REGKEY_TOOLBOX               "Toolbox"
#define REGVALUE_DEFAULTITEMS        "Default Items"
#define REGVALUE_FORMATS             "Formats"
#define REGVALUE_INSTALLED           "Installed"
#define REGVALUE_DEFTOOLBOXTAB       "DefaultToolboxTab"
#define REGKEY_PROJECTS              "Projects"
#define REGVALUE_DISPLAYNAME         "DisplayName"
#define REGVALUE_CLSIDPACKAGE        "Package"
#define REGVALUE_CLSIDPROJECTFACTORYPACKAGE "ProjectFactoryPackage"
#define REGVALUE_PROJTEMPLATES       "ProjectTemplatesDir"
#define REGVALUE_PROJFILTER          "DisplayProjectFileExtensions"
#define REGVALUE_LANGUAGE            "Language(VsTemplate)"
#define REGVALUE_DISPLAYPROJECTTYPE  "DisplayProjectType(VsTemplate)"
#define REGVALUE_TEMPLATEIDS  "TemplateIDs(VsTemplate)"
#define REGVALUE_TEMPLATEGROUPIDS  "TemplateGroupIDs(VsTemplate)"
#define REGVALUE_SHOWONLYSPECIFIEDTEMPLATES  "ShowOnlySpecifiedTemplates(VsTemplate)"
#define REGVALUE_NEWPROJECTREQUIRESNEWFOLDER "NewProjectRequiresNewFolder(VsTemplate)"
#define REGVALUE_SUPPORTSDEFERREDSAVE "SupportsDeferredSave(VsTemplate)"
#define REGVALUE_DISABLEPROJECTSECURITYCHECK "DisableProjectSecurityCheck"
#define REGVALUE_UPGRADECHECKSCHEDULEDTIMESTAMP "UpgradeCheckScheduledTimestamp"

#define REGVALUE_PROJECTSUBTYPE      "ProjectSubType(VsTemplate)"  
#define REGVALUE_LOCATIONFIELDMRUPREFIX  "LocationFieldMRUPrefix(VsTemplate)"  

#define REGVALUE_PROJECTEXT          "PossibleProjectExtensions"
#define REGVALUE_PROJECTDEFEXT       "DefaultProjectExtension"
#define REGVALUE_DIRPROJECTS         "DirBasedProjects"
#define REGVALUE_DEVSTUDIOPKGID      "DevStudioPackageID"
#define REGKEY_PRJPROPERTYPAGES      "CommonPropertyPages"
#define REGKEY_PRJCTCFGPROPERTYPAGES "ConfigPropertyPages"
#define REGKEY_PRJCTPRIORITYPROPERTYPAGES "PriorityPropertyPages"
#define REGVALUE_PRJPROPPAGESORDER   "PageOrder"
#define REGKEY_ITEMFILTERS           "Filters"
#define REGVALUE_COMMONOPENFILTER    "CommonOpenFilesFilter"
#define REGVALUE_COMMONFINDFILTER    "CommonFindFilesFilter"
#define REGVALUE_FINDINFILESFILTER   "FindInFilesFilter"
#define REGVALUE_NOTOPENFILEFILTER   "NotOpenFileFilter"
#define REGVALUE_NOTADDEXISTFILTER   "NotAddExistingItemFilter"
#define REGKEY_ADDITEMTEMPLATES      "AddItemTemplates"
#define REGKEY_TEMPLATEDIRS          "TemplateDirs"
#define REGVALUE_TEMPLATESDIR        "TemplatesDir"
#define REGVALUE_ITEMTEMPLATES       "ItemTemplatesDir"
#define REGVALUE_LOCALIZEDSUBDIR     "TemplatesLocalizedSubDir"
#define REGVALUE_SORTPRIORITY        "SortPriority"
#define REGVALUE_NEWPROJDLGONLY      "NewProjectDialogOnly"
#define REGVALUE_NEWPROJDLGEXONLY    "NewProjectDialogExOnly"
#define REGVALUE_FOLDERGUID          "Folder"
#define REGKEY_PSEUDOFOLDERS         "PseudoFolders"
#define REGKEY_VSTEMPLATE           "VsTemplate"
#define REGKEY_PROJECT              "VsTemplate\\Project"
#define REGKEY_ITEM                 "VsTemplate\\Item"
#define REGVALUE_USERFOLDER         "UserFolder"
#define REGVALUE_CACHEFOLDER        "CacheFolder"

#define REGKEY_EDITORS               "Editors"
#define REGKEY_LOGICALVIEWS          "LogicalViews"
#define REGKEY_OPENWITHENTRIES       "OpenWithEntries"
#define REGKEY_EXTENSIONS            "Extensions"
#define REGVALUE_EXCLUDETEXTEDIT     "ExcludeDefTextEditor"
#define REGVALUE_OPENWITHS           "OpenWithEntries"
#define REGVALUE_BINARYOK            "AcceptBinaryFiles"
#define REGVALUE_EDITORTRUSTLEVEL    "EditorTrustLevel"
#define REGVALUE_EDITORCHOOSER       "EditorChooser"
#define REGVALUE_EDITORTEMPLATESDIR  "EditorTemplatesDir"
#define REGVALUE_COMMONPHYSICALVIEWATTRIBUTES   "CommonPhysicalViewAttributes"
#define REGKEY_UNTRUSTEDLOGICALVIEWS "UntrustedLogicalViews"
#define REGKEY_PHYSICALVIEWATTRIBUTES "PhysicalViewAttributes"

#define REGKEY_SERVICES              "Services"
#define REGVALUE_NAME                "Name"

#define REGKEY_NEWPROJTEMPLATES      "NewProjectTemplates"
#define REGKEY_NEWPROJUSERTEMPLATES  "NewProjectUserTemplates"
#define REGVALUE_DEFEXPANDEDFOLDER   "DefaultExpandedFolder"
#define REGVALUE_DEFSELECTEDITEM     "DefaultSelectedItem"

#define REGKEY_INSTALLEDPRODUCTS     "InstalledProducts"
#define REGVALUE_RESOURCEID          "ResourceID"
#define REGVALUE_RESOURCETYPE        "ResourceType"
#define REGVALUE_PRODUCTDETAILS      "ProductDetails"
#define REGVALUE_LOGOID              "LogoID"
#define REGVALUE_USEINTERFACE        "UseInterface"
#define REGVALUE_USEREGNAMEASSPLASHNAME     "UseRegNameAsSplashName"
#define REGKEY_SLNPERSISTENCE        "SolutionPersistence"

#define REGKEY_MENUS                 "Menus"

#define REGKEY_CLSID                 "CLSID"

#define REGKEY_KEYBINDTABLES         "KeyBindingTables"
#define REGVALUE_ALLOWNAVKEYBIND     "AllowNavKeyBinding"

#define REGKEY_LANGUAGES             "Languages"
#define REGKEY_FILEEXTENSIONS        "File Extensions"
#define REGKEY_LANGSERVICES          "Language Services"
#define REGVALUE_LANGRESID           "LangResID"
#define REGVALUE_SHOWCOMPLETION      "ShowCompletion"
#define REGVALUE_SHOWSMARTINDENT     "ShowSmartIndent"
#define REGKEY_EDITORTOOLSOPTIONS    "EditorToolsOptions"
#define REGVALUE_NOTALANGUAGE        "NotALanguage"

#define REGKEY_AUTOMATIONPROPS       "AutomationProperties"

#define REGKEY_TOOLSOPTIONSPAGES     "ToolsOptionsPages"
#define REGKEY_VISIBILITYCMDUICONTEXTS L"VisibilityCmdUIContexts"
#define REGVALUE_CLSIDPAGE           "Page"
#define REGVALUE_PAGESORT            "Sort"
#define REGVALUE_PAGEADDTOMRU        "AddToMRU"
#define REGVALUE_COMPONENTTYPE       "ComponentType"

#define REGKEY_TOOLBOXPAGES          "ToolboxPages"
#define REGVALUE_TBXPAGESORT         "DefaultTbx"

#define REGKEY_TOOLWINDOWS           "ToolWindows"
#define REGVALUE_STYLE               "Style"
#define REGVALUE_ORIENTATION         "Orientation"
#define REGVALUE_FLOAT               "Float"
#define REGVALUE_WINDOW              "Window"

#define REGKEY_COMPONENTPICKERPAGES  "ComponentPickerPages"

#define REGKEY_OUTPUTWINDOW          "OutputWindow"
#define REGVALUE_PANEINITINVISIBLE   "InitiallyInvisible"
#define REGVALUE_PANECLEAREDWITHSLN  "ClearWithSolution"
