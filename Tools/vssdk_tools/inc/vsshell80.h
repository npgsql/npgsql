

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for vsshell80.idl:
    Oicf, W0, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

/* verify that the <rpcsal.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCSAL_H_VERSION__
#define __REQUIRED_RPCSAL_H_VERSION__ 100
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __vsshell80_h__
#define __vsshell80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsProjectDebugTargetProvider_FWD_DEFINED__
#define __IVsProjectDebugTargetProvider_FWD_DEFINED__
typedef interface IVsProjectDebugTargetProvider IVsProjectDebugTargetProvider;
#endif 	/* __IVsProjectDebugTargetProvider_FWD_DEFINED__ */


#ifndef __IVsRegisterProjectDebugTargetProvider_FWD_DEFINED__
#define __IVsRegisterProjectDebugTargetProvider_FWD_DEFINED__
typedef interface IVsRegisterProjectDebugTargetProvider IVsRegisterProjectDebugTargetProvider;
#endif 	/* __IVsRegisterProjectDebugTargetProvider_FWD_DEFINED__ */


#ifndef __SVsRegisterDebugTargetProvider_FWD_DEFINED__
#define __SVsRegisterDebugTargetProvider_FWD_DEFINED__
typedef interface SVsRegisterDebugTargetProvider SVsRegisterDebugTargetProvider;
#endif 	/* __SVsRegisterDebugTargetProvider_FWD_DEFINED__ */


#ifndef __IVsBrowseObject_FWD_DEFINED__
#define __IVsBrowseObject_FWD_DEFINED__
typedef interface IVsBrowseObject IVsBrowseObject;
#endif 	/* __IVsBrowseObject_FWD_DEFINED__ */


#ifndef __IVsCfgBrowseObject_FWD_DEFINED__
#define __IVsCfgBrowseObject_FWD_DEFINED__
typedef interface IVsCfgBrowseObject IVsCfgBrowseObject;
#endif 	/* __IVsCfgBrowseObject_FWD_DEFINED__ */


#ifndef __IVsDetermineWizardTrust_FWD_DEFINED__
#define __IVsDetermineWizardTrust_FWD_DEFINED__
typedef interface IVsDetermineWizardTrust IVsDetermineWizardTrust;
#endif 	/* __IVsDetermineWizardTrust_FWD_DEFINED__ */


#ifndef __SVsDetermineWizardTrust_FWD_DEFINED__
#define __SVsDetermineWizardTrust_FWD_DEFINED__
typedef interface SVsDetermineWizardTrust SVsDetermineWizardTrust;
#endif 	/* __SVsDetermineWizardTrust_FWD_DEFINED__ */


#ifndef __IVsSolutionEvents4_FWD_DEFINED__
#define __IVsSolutionEvents4_FWD_DEFINED__
typedef interface IVsSolutionEvents4 IVsSolutionEvents4;
#endif 	/* __IVsSolutionEvents4_FWD_DEFINED__ */


#ifndef __IVsFireSolutionEvents2_FWD_DEFINED__
#define __IVsFireSolutionEvents2_FWD_DEFINED__
typedef interface IVsFireSolutionEvents2 IVsFireSolutionEvents2;
#endif 	/* __IVsFireSolutionEvents2_FWD_DEFINED__ */


#ifndef __IVsPrioritizedSolutionEvents_FWD_DEFINED__
#define __IVsPrioritizedSolutionEvents_FWD_DEFINED__
typedef interface IVsPrioritizedSolutionEvents IVsPrioritizedSolutionEvents;
#endif 	/* __IVsPrioritizedSolutionEvents_FWD_DEFINED__ */


#ifndef __IVsPersistSolutionProps2_FWD_DEFINED__
#define __IVsPersistSolutionProps2_FWD_DEFINED__
typedef interface IVsPersistSolutionProps2 IVsPersistSolutionProps2;
#endif 	/* __IVsPersistSolutionProps2_FWD_DEFINED__ */


#ifndef __IVsEnhancedDataTip_FWD_DEFINED__
#define __IVsEnhancedDataTip_FWD_DEFINED__
typedef interface IVsEnhancedDataTip IVsEnhancedDataTip;
#endif 	/* __IVsEnhancedDataTip_FWD_DEFINED__ */


#ifndef __IVsDebugger2_FWD_DEFINED__
#define __IVsDebugger2_FWD_DEFINED__
typedef interface IVsDebugger2 IVsDebugger2;
#endif 	/* __IVsDebugger2_FWD_DEFINED__ */


#ifndef __IVsDebugProcessNotify_FWD_DEFINED__
#define __IVsDebugProcessNotify_FWD_DEFINED__
typedef interface IVsDebugProcessNotify IVsDebugProcessNotify;
#endif 	/* __IVsDebugProcessNotify_FWD_DEFINED__ */


#ifndef __IVsQueryDebuggableProjectCfg_FWD_DEFINED__
#define __IVsQueryDebuggableProjectCfg_FWD_DEFINED__
typedef interface IVsQueryDebuggableProjectCfg IVsQueryDebuggableProjectCfg;
#endif 	/* __IVsQueryDebuggableProjectCfg_FWD_DEFINED__ */


#ifndef __IVsCommandWindow2_FWD_DEFINED__
#define __IVsCommandWindow2_FWD_DEFINED__
typedef interface IVsCommandWindow2 IVsCommandWindow2;
#endif 	/* __IVsCommandWindow2_FWD_DEFINED__ */


#ifndef __IVsDeferredDocView_FWD_DEFINED__
#define __IVsDeferredDocView_FWD_DEFINED__
typedef interface IVsDeferredDocView IVsDeferredDocView;
#endif 	/* __IVsDeferredDocView_FWD_DEFINED__ */


#ifndef __IVsBuildableProjectCfg2_FWD_DEFINED__
#define __IVsBuildableProjectCfg2_FWD_DEFINED__
typedef interface IVsBuildableProjectCfg2 IVsBuildableProjectCfg2;
#endif 	/* __IVsBuildableProjectCfg2_FWD_DEFINED__ */


#ifndef __IVsPublishableProjectStatusCallback_FWD_DEFINED__
#define __IVsPublishableProjectStatusCallback_FWD_DEFINED__
typedef interface IVsPublishableProjectStatusCallback IVsPublishableProjectStatusCallback;
#endif 	/* __IVsPublishableProjectStatusCallback_FWD_DEFINED__ */


#ifndef __IVsPublishableProjectCfg_FWD_DEFINED__
#define __IVsPublishableProjectCfg_FWD_DEFINED__
typedef interface IVsPublishableProjectCfg IVsPublishableProjectCfg;
#endif 	/* __IVsPublishableProjectCfg_FWD_DEFINED__ */


#ifndef __IVsParseCommandLine2_FWD_DEFINED__
#define __IVsParseCommandLine2_FWD_DEFINED__
typedef interface IVsParseCommandLine2 IVsParseCommandLine2;
#endif 	/* __IVsParseCommandLine2_FWD_DEFINED__ */


#ifndef __IVsCommandWindowsCollection_FWD_DEFINED__
#define __IVsCommandWindowsCollection_FWD_DEFINED__
typedef interface IVsCommandWindowsCollection IVsCommandWindowsCollection;
#endif 	/* __IVsCommandWindowsCollection_FWD_DEFINED__ */


#ifndef __SVsCommandWindowsCollection_FWD_DEFINED__
#define __SVsCommandWindowsCollection_FWD_DEFINED__
typedef interface SVsCommandWindowsCollection SVsCommandWindowsCollection;
#endif 	/* __SVsCommandWindowsCollection_FWD_DEFINED__ */


#ifndef __IVsThreadPool_FWD_DEFINED__
#define __IVsThreadPool_FWD_DEFINED__
typedef interface IVsThreadPool IVsThreadPool;
#endif 	/* __IVsThreadPool_FWD_DEFINED__ */


#ifndef __SVsThreadPool_FWD_DEFINED__
#define __SVsThreadPool_FWD_DEFINED__
typedef interface SVsThreadPool SVsThreadPool;
#endif 	/* __SVsThreadPool_FWD_DEFINED__ */


#ifndef __IVsShell2_FWD_DEFINED__
#define __IVsShell2_FWD_DEFINED__
typedef interface IVsShell2 IVsShell2;
#endif 	/* __IVsShell2_FWD_DEFINED__ */


#ifndef __IVsGradient_FWD_DEFINED__
#define __IVsGradient_FWD_DEFINED__
typedef interface IVsGradient IVsGradient;
#endif 	/* __IVsGradient_FWD_DEFINED__ */


#ifndef __IVsImageButton_FWD_DEFINED__
#define __IVsImageButton_FWD_DEFINED__
typedef interface IVsImageButton IVsImageButton;
#endif 	/* __IVsImageButton_FWD_DEFINED__ */


#ifndef __IVsUIShell2_FWD_DEFINED__
#define __IVsUIShell2_FWD_DEFINED__
typedef interface IVsUIShell2 IVsUIShell2;
#endif 	/* __IVsUIShell2_FWD_DEFINED__ */


#ifndef __SVsMainWindowDropTarget_FWD_DEFINED__
#define __SVsMainWindowDropTarget_FWD_DEFINED__
typedef interface SVsMainWindowDropTarget SVsMainWindowDropTarget;
#endif 	/* __SVsMainWindowDropTarget_FWD_DEFINED__ */


#ifndef __IVsSupportItemHandoff2_FWD_DEFINED__
#define __IVsSupportItemHandoff2_FWD_DEFINED__
typedef interface IVsSupportItemHandoff2 IVsSupportItemHandoff2;
#endif 	/* __IVsSupportItemHandoff2_FWD_DEFINED__ */


#ifndef __IVsLaunchPadOutputParser_FWD_DEFINED__
#define __IVsLaunchPadOutputParser_FWD_DEFINED__
typedef interface IVsLaunchPadOutputParser IVsLaunchPadOutputParser;
#endif 	/* __IVsLaunchPadOutputParser_FWD_DEFINED__ */


#ifndef __IVsLaunchPad2_FWD_DEFINED__
#define __IVsLaunchPad2_FWD_DEFINED__
typedef interface IVsLaunchPad2 IVsLaunchPad2;
#endif 	/* __IVsLaunchPad2_FWD_DEFINED__ */


#ifndef __IVsOpenProjectOrSolutionDlg_FWD_DEFINED__
#define __IVsOpenProjectOrSolutionDlg_FWD_DEFINED__
typedef interface IVsOpenProjectOrSolutionDlg IVsOpenProjectOrSolutionDlg;
#endif 	/* __IVsOpenProjectOrSolutionDlg_FWD_DEFINED__ */


#ifndef __SVsOpenProjectOrSolutionDlg_FWD_DEFINED__
#define __SVsOpenProjectOrSolutionDlg_FWD_DEFINED__
typedef interface SVsOpenProjectOrSolutionDlg SVsOpenProjectOrSolutionDlg;
#endif 	/* __SVsOpenProjectOrSolutionDlg_FWD_DEFINED__ */


#ifndef __IVsCreateAggregateProject_FWD_DEFINED__
#define __IVsCreateAggregateProject_FWD_DEFINED__
typedef interface IVsCreateAggregateProject IVsCreateAggregateProject;
#endif 	/* __IVsCreateAggregateProject_FWD_DEFINED__ */


#ifndef __SVsCreateAggregateProject_FWD_DEFINED__
#define __SVsCreateAggregateProject_FWD_DEFINED__
typedef interface SVsCreateAggregateProject SVsCreateAggregateProject;
#endif 	/* __SVsCreateAggregateProject_FWD_DEFINED__ */


#ifndef __IVsAggregatableProject_FWD_DEFINED__
#define __IVsAggregatableProject_FWD_DEFINED__
typedef interface IVsAggregatableProject IVsAggregatableProject;
#endif 	/* __IVsAggregatableProject_FWD_DEFINED__ */


#ifndef __IVsAggregatableProjectFactory_FWD_DEFINED__
#define __IVsAggregatableProjectFactory_FWD_DEFINED__
typedef interface IVsAggregatableProjectFactory IVsAggregatableProjectFactory;
#endif 	/* __IVsAggregatableProjectFactory_FWD_DEFINED__ */


#ifndef __IVsParentProject2_FWD_DEFINED__
#define __IVsParentProject2_FWD_DEFINED__
typedef interface IVsParentProject2 IVsParentProject2;
#endif 	/* __IVsParentProject2_FWD_DEFINED__ */


#ifndef __IVsBuildPropertyStorage_FWD_DEFINED__
#define __IVsBuildPropertyStorage_FWD_DEFINED__
typedef interface IVsBuildPropertyStorage IVsBuildPropertyStorage;
#endif 	/* __IVsBuildPropertyStorage_FWD_DEFINED__ */


#ifndef __IVsProjectBuildSystem_FWD_DEFINED__
#define __IVsProjectBuildSystem_FWD_DEFINED__
typedef interface IVsProjectBuildSystem IVsProjectBuildSystem;
#endif 	/* __IVsProjectBuildSystem_FWD_DEFINED__ */


#ifndef __IPersistXMLFragment_FWD_DEFINED__
#define __IPersistXMLFragment_FWD_DEFINED__
typedef interface IPersistXMLFragment IPersistXMLFragment;
#endif 	/* __IPersistXMLFragment_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorCfg_FWD_DEFINED__
#define __IVsProjectFlavorCfg_FWD_DEFINED__
typedef interface IVsProjectFlavorCfg IVsProjectFlavorCfg;
#endif 	/* __IVsProjectFlavorCfg_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorCfgOutputGroups_FWD_DEFINED__
#define __IVsProjectFlavorCfgOutputGroups_FWD_DEFINED__
typedef interface IVsProjectFlavorCfgOutputGroups IVsProjectFlavorCfgOutputGroups;
#endif 	/* __IVsProjectFlavorCfgOutputGroups_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorCfgProvider_FWD_DEFINED__
#define __IVsProjectFlavorCfgProvider_FWD_DEFINED__
typedef interface IVsProjectFlavorCfgProvider IVsProjectFlavorCfgProvider;
#endif 	/* __IVsProjectFlavorCfgProvider_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorReferences_FWD_DEFINED__
#define __IVsProjectFlavorReferences_FWD_DEFINED__
typedef interface IVsProjectFlavorReferences IVsProjectFlavorReferences;
#endif 	/* __IVsProjectFlavorReferences_FWD_DEFINED__ */


#ifndef __IVsFilterKeys2_FWD_DEFINED__
#define __IVsFilterKeys2_FWD_DEFINED__
typedef interface IVsFilterKeys2 IVsFilterKeys2;
#endif 	/* __IVsFilterKeys2_FWD_DEFINED__ */


#ifndef __IVsSettingsReader_FWD_DEFINED__
#define __IVsSettingsReader_FWD_DEFINED__
typedef interface IVsSettingsReader IVsSettingsReader;
#endif 	/* __IVsSettingsReader_FWD_DEFINED__ */


#ifndef __IVsSettingsWriter_FWD_DEFINED__
#define __IVsSettingsWriter_FWD_DEFINED__
typedef interface IVsSettingsWriter IVsSettingsWriter;
#endif 	/* __IVsSettingsWriter_FWD_DEFINED__ */


#ifndef __IVsUserSettings_FWD_DEFINED__
#define __IVsUserSettings_FWD_DEFINED__
typedef interface IVsUserSettings IVsUserSettings;
#endif 	/* __IVsUserSettings_FWD_DEFINED__ */


#ifndef __SVsSettingsReader_FWD_DEFINED__
#define __SVsSettingsReader_FWD_DEFINED__
typedef interface SVsSettingsReader SVsSettingsReader;
#endif 	/* __SVsSettingsReader_FWD_DEFINED__ */


#ifndef __IVsUserSettingsQuery_FWD_DEFINED__
#define __IVsUserSettingsQuery_FWD_DEFINED__
typedef interface IVsUserSettingsQuery IVsUserSettingsQuery;
#endif 	/* __IVsUserSettingsQuery_FWD_DEFINED__ */


#ifndef __IVsProfileSettingsFileInfo_FWD_DEFINED__
#define __IVsProfileSettingsFileInfo_FWD_DEFINED__
typedef interface IVsProfileSettingsFileInfo IVsProfileSettingsFileInfo;
#endif 	/* __IVsProfileSettingsFileInfo_FWD_DEFINED__ */


#ifndef __IVsProfileSettingsFileCollection_FWD_DEFINED__
#define __IVsProfileSettingsFileCollection_FWD_DEFINED__
typedef interface IVsProfileSettingsFileCollection IVsProfileSettingsFileCollection;
#endif 	/* __IVsProfileSettingsFileCollection_FWD_DEFINED__ */


#ifndef __IVsProfileSettingsTree_FWD_DEFINED__
#define __IVsProfileSettingsTree_FWD_DEFINED__
typedef interface IVsProfileSettingsTree IVsProfileSettingsTree;
#endif 	/* __IVsProfileSettingsTree_FWD_DEFINED__ */


#ifndef __IVsSettingsErrorInformation_FWD_DEFINED__
#define __IVsSettingsErrorInformation_FWD_DEFINED__
typedef interface IVsSettingsErrorInformation IVsSettingsErrorInformation;
#endif 	/* __IVsSettingsErrorInformation_FWD_DEFINED__ */


#ifndef __IVsProfileDataManager_FWD_DEFINED__
#define __IVsProfileDataManager_FWD_DEFINED__
typedef interface IVsProfileDataManager IVsProfileDataManager;
#endif 	/* __IVsProfileDataManager_FWD_DEFINED__ */


#ifndef __SVsProfileDataManager_FWD_DEFINED__
#define __SVsProfileDataManager_FWD_DEFINED__
typedef interface SVsProfileDataManager SVsProfileDataManager;
#endif 	/* __SVsProfileDataManager_FWD_DEFINED__ */


#ifndef __IVsDeferredSaveProject_FWD_DEFINED__
#define __IVsDeferredSaveProject_FWD_DEFINED__
typedef interface IVsDeferredSaveProject IVsDeferredSaveProject;
#endif 	/* __IVsDeferredSaveProject_FWD_DEFINED__ */


#ifndef __IVsBrowseProjectLocation_FWD_DEFINED__
#define __IVsBrowseProjectLocation_FWD_DEFINED__
typedef interface IVsBrowseProjectLocation IVsBrowseProjectLocation;
#endif 	/* __IVsBrowseProjectLocation_FWD_DEFINED__ */


#ifndef __IVsSolution3_FWD_DEFINED__
#define __IVsSolution3_FWD_DEFINED__
typedef interface IVsSolution3 IVsSolution3;
#endif 	/* __IVsSolution3_FWD_DEFINED__ */


#ifndef __IVsConfigurationManagerDlg_FWD_DEFINED__
#define __IVsConfigurationManagerDlg_FWD_DEFINED__
typedef interface IVsConfigurationManagerDlg IVsConfigurationManagerDlg;
#endif 	/* __IVsConfigurationManagerDlg_FWD_DEFINED__ */


#ifndef __SVsConfigurationManagerDlg_FWD_DEFINED__
#define __SVsConfigurationManagerDlg_FWD_DEFINED__
typedef interface SVsConfigurationManagerDlg SVsConfigurationManagerDlg;
#endif 	/* __SVsConfigurationManagerDlg_FWD_DEFINED__ */


#ifndef __IVsUpdateSolutionEvents3_FWD_DEFINED__
#define __IVsUpdateSolutionEvents3_FWD_DEFINED__
typedef interface IVsUpdateSolutionEvents3 IVsUpdateSolutionEvents3;
#endif 	/* __IVsUpdateSolutionEvents3_FWD_DEFINED__ */


#ifndef __IVsSolutionBuildManager3_FWD_DEFINED__
#define __IVsSolutionBuildManager3_FWD_DEFINED__
typedef interface IVsSolutionBuildManager3 IVsSolutionBuildManager3;
#endif 	/* __IVsSolutionBuildManager3_FWD_DEFINED__ */


#ifndef __IVsSingleFileGeneratorFactory_FWD_DEFINED__
#define __IVsSingleFileGeneratorFactory_FWD_DEFINED__
typedef interface IVsSingleFileGeneratorFactory IVsSingleFileGeneratorFactory;
#endif 	/* __IVsSingleFileGeneratorFactory_FWD_DEFINED__ */


#ifndef __IVsStartPageDownload_FWD_DEFINED__
#define __IVsStartPageDownload_FWD_DEFINED__
typedef interface IVsStartPageDownload IVsStartPageDownload;
#endif 	/* __IVsStartPageDownload_FWD_DEFINED__ */


#ifndef __SVsStartPageDownload_FWD_DEFINED__
#define __SVsStartPageDownload_FWD_DEFINED__
typedef interface SVsStartPageDownload SVsStartPageDownload;
#endif 	/* __SVsStartPageDownload_FWD_DEFINED__ */


#ifndef __IVsMenuEditorTransactionEvents_FWD_DEFINED__
#define __IVsMenuEditorTransactionEvents_FWD_DEFINED__
typedef interface IVsMenuEditorTransactionEvents IVsMenuEditorTransactionEvents;
#endif 	/* __IVsMenuEditorTransactionEvents_FWD_DEFINED__ */


#ifndef __IVsThreadedWaitDialog_FWD_DEFINED__
#define __IVsThreadedWaitDialog_FWD_DEFINED__
typedef interface IVsThreadedWaitDialog IVsThreadedWaitDialog;
#endif 	/* __IVsThreadedWaitDialog_FWD_DEFINED__ */


#ifndef __SVsThreadedWaitDialog_FWD_DEFINED__
#define __SVsThreadedWaitDialog_FWD_DEFINED__
typedef interface SVsThreadedWaitDialog SVsThreadedWaitDialog;
#endif 	/* __SVsThreadedWaitDialog_FWD_DEFINED__ */


#ifndef __IVsProfilesManagerUI_FWD_DEFINED__
#define __IVsProfilesManagerUI_FWD_DEFINED__
typedef interface IVsProfilesManagerUI IVsProfilesManagerUI;
#endif 	/* __IVsProfilesManagerUI_FWD_DEFINED__ */


#ifndef __SVsProfilesManagerUI_FWD_DEFINED__
#define __SVsProfilesManagerUI_FWD_DEFINED__
typedef interface SVsProfilesManagerUI SVsProfilesManagerUI;
#endif 	/* __SVsProfilesManagerUI_FWD_DEFINED__ */


#ifndef __IVsXMLMemberDataCallBack_FWD_DEFINED__
#define __IVsXMLMemberDataCallBack_FWD_DEFINED__
typedef interface IVsXMLMemberDataCallBack IVsXMLMemberDataCallBack;
#endif 	/* __IVsXMLMemberDataCallBack_FWD_DEFINED__ */


#ifndef __IVsXMLMemberDataRegisterCallBack_FWD_DEFINED__
#define __IVsXMLMemberDataRegisterCallBack_FWD_DEFINED__
typedef interface IVsXMLMemberDataRegisterCallBack IVsXMLMemberDataRegisterCallBack;
#endif 	/* __IVsXMLMemberDataRegisterCallBack_FWD_DEFINED__ */


#ifndef __IVsXMLMemberData3_FWD_DEFINED__
#define __IVsXMLMemberData3_FWD_DEFINED__
typedef interface IVsXMLMemberData3 IVsXMLMemberData3;
#endif 	/* __IVsXMLMemberData3_FWD_DEFINED__ */


#ifndef __IVsNavInfoNode_FWD_DEFINED__
#define __IVsNavInfoNode_FWD_DEFINED__
typedef interface IVsNavInfoNode IVsNavInfoNode;
#endif 	/* __IVsNavInfoNode_FWD_DEFINED__ */


#ifndef __IVsEnumNavInfoNodes_FWD_DEFINED__
#define __IVsEnumNavInfoNodes_FWD_DEFINED__
typedef interface IVsEnumNavInfoNodes IVsEnumNavInfoNodes;
#endif 	/* __IVsEnumNavInfoNodes_FWD_DEFINED__ */


#ifndef __IVsNavInfo_FWD_DEFINED__
#define __IVsNavInfo_FWD_DEFINED__
typedef interface IVsNavInfo IVsNavInfo;
#endif 	/* __IVsNavInfo_FWD_DEFINED__ */


#ifndef __IVsObjectBrowserDescription3_FWD_DEFINED__
#define __IVsObjectBrowserDescription3_FWD_DEFINED__
typedef interface IVsObjectBrowserDescription3 IVsObjectBrowserDescription3;
#endif 	/* __IVsObjectBrowserDescription3_FWD_DEFINED__ */


#ifndef __IVsObjectList2_FWD_DEFINED__
#define __IVsObjectList2_FWD_DEFINED__
typedef interface IVsObjectList2 IVsObjectList2;
#endif 	/* __IVsObjectList2_FWD_DEFINED__ */


#ifndef __IVsSimpleObjectList2_FWD_DEFINED__
#define __IVsSimpleObjectList2_FWD_DEFINED__
typedef interface IVsSimpleObjectList2 IVsSimpleObjectList2;
#endif 	/* __IVsSimpleObjectList2_FWD_DEFINED__ */


#ifndef __IVsBrowseContainersList_FWD_DEFINED__
#define __IVsBrowseContainersList_FWD_DEFINED__
typedef interface IVsBrowseContainersList IVsBrowseContainersList;
#endif 	/* __IVsBrowseContainersList_FWD_DEFINED__ */


#ifndef __IVsLibrary2_FWD_DEFINED__
#define __IVsLibrary2_FWD_DEFINED__
typedef interface IVsLibrary2 IVsLibrary2;
#endif 	/* __IVsLibrary2_FWD_DEFINED__ */


#ifndef __IVsSimpleLibrary2_FWD_DEFINED__
#define __IVsSimpleLibrary2_FWD_DEFINED__
typedef interface IVsSimpleLibrary2 IVsSimpleLibrary2;
#endif 	/* __IVsSimpleLibrary2_FWD_DEFINED__ */


#ifndef __IVsLibrary2Ex_FWD_DEFINED__
#define __IVsLibrary2Ex_FWD_DEFINED__
typedef interface IVsLibrary2Ex IVsLibrary2Ex;
#endif 	/* __IVsLibrary2Ex_FWD_DEFINED__ */


#ifndef __IVsEnumLibraries2_FWD_DEFINED__
#define __IVsEnumLibraries2_FWD_DEFINED__
typedef interface IVsEnumLibraries2 IVsEnumLibraries2;
#endif 	/* __IVsEnumLibraries2_FWD_DEFINED__ */


#ifndef __IVsObjectManager2_FWD_DEFINED__
#define __IVsObjectManager2_FWD_DEFINED__
typedef interface IVsObjectManager2 IVsObjectManager2;
#endif 	/* __IVsObjectManager2_FWD_DEFINED__ */


#ifndef __IVsBrowseComponentSet_FWD_DEFINED__
#define __IVsBrowseComponentSet_FWD_DEFINED__
typedef interface IVsBrowseComponentSet IVsBrowseComponentSet;
#endif 	/* __IVsBrowseComponentSet_FWD_DEFINED__ */


#ifndef __IVsSimpleBrowseComponentSet_FWD_DEFINED__
#define __IVsSimpleBrowseComponentSet_FWD_DEFINED__
typedef interface IVsSimpleBrowseComponentSet IVsSimpleBrowseComponentSet;
#endif 	/* __IVsSimpleBrowseComponentSet_FWD_DEFINED__ */


#ifndef __IVsCombinedBrowseComponentSet_FWD_DEFINED__
#define __IVsCombinedBrowseComponentSet_FWD_DEFINED__
typedef interface IVsCombinedBrowseComponentSet IVsCombinedBrowseComponentSet;
#endif 	/* __IVsCombinedBrowseComponentSet_FWD_DEFINED__ */


#ifndef __IVsSelectedSymbol_FWD_DEFINED__
#define __IVsSelectedSymbol_FWD_DEFINED__
typedef interface IVsSelectedSymbol IVsSelectedSymbol;
#endif 	/* __IVsSelectedSymbol_FWD_DEFINED__ */


#ifndef __IVsEnumSelectedSymbols_FWD_DEFINED__
#define __IVsEnumSelectedSymbols_FWD_DEFINED__
typedef interface IVsEnumSelectedSymbols IVsEnumSelectedSymbols;
#endif 	/* __IVsEnumSelectedSymbols_FWD_DEFINED__ */


#ifndef __IVsSelectedSymbols_FWD_DEFINED__
#define __IVsSelectedSymbols_FWD_DEFINED__
typedef interface IVsSelectedSymbols IVsSelectedSymbols;
#endif 	/* __IVsSelectedSymbols_FWD_DEFINED__ */


#ifndef __IVsNavigationTool_FWD_DEFINED__
#define __IVsNavigationTool_FWD_DEFINED__
typedef interface IVsNavigationTool IVsNavigationTool;
#endif 	/* __IVsNavigationTool_FWD_DEFINED__ */


#ifndef __IVsFindSymbol_FWD_DEFINED__
#define __IVsFindSymbol_FWD_DEFINED__
typedef interface IVsFindSymbol IVsFindSymbol;
#endif 	/* __IVsFindSymbol_FWD_DEFINED__ */


#ifndef __IVsFindSymbolEvents_FWD_DEFINED__
#define __IVsFindSymbolEvents_FWD_DEFINED__
typedef interface IVsFindSymbolEvents IVsFindSymbolEvents;
#endif 	/* __IVsFindSymbolEvents_FWD_DEFINED__ */


#ifndef __IVsCallBrowser_FWD_DEFINED__
#define __IVsCallBrowser_FWD_DEFINED__
typedef interface IVsCallBrowser IVsCallBrowser;
#endif 	/* __IVsCallBrowser_FWD_DEFINED__ */


#ifndef __SVsCallBrowser_FWD_DEFINED__
#define __SVsCallBrowser_FWD_DEFINED__
typedef interface SVsCallBrowser SVsCallBrowser;
#endif 	/* __SVsCallBrowser_FWD_DEFINED__ */


#ifndef __IVsComponentSelectorDlg2_FWD_DEFINED__
#define __IVsComponentSelectorDlg2_FWD_DEFINED__
typedef interface IVsComponentSelectorDlg2 IVsComponentSelectorDlg2;
#endif 	/* __IVsComponentSelectorDlg2_FWD_DEFINED__ */


#ifndef __SVsComponentSelectorDlg2_FWD_DEFINED__
#define __SVsComponentSelectorDlg2_FWD_DEFINED__
typedef interface SVsComponentSelectorDlg2 SVsComponentSelectorDlg2;
#endif 	/* __SVsComponentSelectorDlg2_FWD_DEFINED__ */


#ifndef __IVsBuildMacroInfo_FWD_DEFINED__
#define __IVsBuildMacroInfo_FWD_DEFINED__
typedef interface IVsBuildMacroInfo IVsBuildMacroInfo;
#endif 	/* __IVsBuildMacroInfo_FWD_DEFINED__ */


#ifndef __IVsPreviewChangesList_FWD_DEFINED__
#define __IVsPreviewChangesList_FWD_DEFINED__
typedef interface IVsPreviewChangesList IVsPreviewChangesList;
#endif 	/* __IVsPreviewChangesList_FWD_DEFINED__ */


#ifndef __IVsSimplePreviewChangesList_FWD_DEFINED__
#define __IVsSimplePreviewChangesList_FWD_DEFINED__
typedef interface IVsSimplePreviewChangesList IVsSimplePreviewChangesList;
#endif 	/* __IVsSimplePreviewChangesList_FWD_DEFINED__ */


#ifndef __IVsPreviewChangesEngine_FWD_DEFINED__
#define __IVsPreviewChangesEngine_FWD_DEFINED__
typedef interface IVsPreviewChangesEngine IVsPreviewChangesEngine;
#endif 	/* __IVsPreviewChangesEngine_FWD_DEFINED__ */


#ifndef __IVsPreviewChangesService_FWD_DEFINED__
#define __IVsPreviewChangesService_FWD_DEFINED__
typedef interface IVsPreviewChangesService IVsPreviewChangesService;
#endif 	/* __IVsPreviewChangesService_FWD_DEFINED__ */


#ifndef __SVsPreviewChangesService_FWD_DEFINED__
#define __SVsPreviewChangesService_FWD_DEFINED__
typedef interface SVsPreviewChangesService SVsPreviewChangesService;
#endif 	/* __SVsPreviewChangesService_FWD_DEFINED__ */


#ifndef __IVsCodeDefViewContext_FWD_DEFINED__
#define __IVsCodeDefViewContext_FWD_DEFINED__
typedef interface IVsCodeDefViewContext IVsCodeDefViewContext;
#endif 	/* __IVsCodeDefViewContext_FWD_DEFINED__ */


#ifndef __IVsCodeDefView_FWD_DEFINED__
#define __IVsCodeDefView_FWD_DEFINED__
typedef interface IVsCodeDefView IVsCodeDefView;
#endif 	/* __IVsCodeDefView_FWD_DEFINED__ */


#ifndef __IVsSupportCodeDefView_FWD_DEFINED__
#define __IVsSupportCodeDefView_FWD_DEFINED__
typedef interface IVsSupportCodeDefView IVsSupportCodeDefView;
#endif 	/* __IVsSupportCodeDefView_FWD_DEFINED__ */


#ifndef __SVsCodeDefView_FWD_DEFINED__
#define __SVsCodeDefView_FWD_DEFINED__
typedef interface SVsCodeDefView SVsCodeDefView;
#endif 	/* __SVsCodeDefView_FWD_DEFINED__ */


#ifndef __IVsCoTaskMemFreeMyStrings_FWD_DEFINED__
#define __IVsCoTaskMemFreeMyStrings_FWD_DEFINED__
typedef interface IVsCoTaskMemFreeMyStrings IVsCoTaskMemFreeMyStrings;
#endif 	/* __IVsCoTaskMemFreeMyStrings_FWD_DEFINED__ */


#ifndef __IVsRunningDocumentTable2_FWD_DEFINED__
#define __IVsRunningDocumentTable2_FWD_DEFINED__
typedef interface IVsRunningDocumentTable2 IVsRunningDocumentTable2;
#endif 	/* __IVsRunningDocumentTable2_FWD_DEFINED__ */


#ifndef __IVsRunningDocTableEvents4_FWD_DEFINED__
#define __IVsRunningDocTableEvents4_FWD_DEFINED__
typedef interface IVsRunningDocTableEvents4 IVsRunningDocTableEvents4;
#endif 	/* __IVsRunningDocTableEvents4_FWD_DEFINED__ */


#ifndef __IVsToolboxDataProviderRegistry_FWD_DEFINED__
#define __IVsToolboxDataProviderRegistry_FWD_DEFINED__
typedef interface IVsToolboxDataProviderRegistry IVsToolboxDataProviderRegistry;
#endif 	/* __IVsToolboxDataProviderRegistry_FWD_DEFINED__ */


#ifndef __SVsToolboxDataProviderRegistry_FWD_DEFINED__
#define __SVsToolboxDataProviderRegistry_FWD_DEFINED__
typedef interface SVsToolboxDataProviderRegistry SVsToolboxDataProviderRegistry;
#endif 	/* __SVsToolboxDataProviderRegistry_FWD_DEFINED__ */


#ifndef __IVsFontAndColorCacheManager_FWD_DEFINED__
#define __IVsFontAndColorCacheManager_FWD_DEFINED__
typedef interface IVsFontAndColorCacheManager IVsFontAndColorCacheManager;
#endif 	/* __IVsFontAndColorCacheManager_FWD_DEFINED__ */


#ifndef __SVsFontAndColorCacheManager_FWD_DEFINED__
#define __SVsFontAndColorCacheManager_FWD_DEFINED__
typedef interface SVsFontAndColorCacheManager SVsFontAndColorCacheManager;
#endif 	/* __SVsFontAndColorCacheManager_FWD_DEFINED__ */


#ifndef __IVsUpgradeLogger_FWD_DEFINED__
#define __IVsUpgradeLogger_FWD_DEFINED__
typedef interface IVsUpgradeLogger IVsUpgradeLogger;
#endif 	/* __IVsUpgradeLogger_FWD_DEFINED__ */


#ifndef __SVsUpgradeLogger_FWD_DEFINED__
#define __SVsUpgradeLogger_FWD_DEFINED__
typedef interface SVsUpgradeLogger SVsUpgradeLogger;
#endif 	/* __SVsUpgradeLogger_FWD_DEFINED__ */


#ifndef __IVsFileUpgrade_FWD_DEFINED__
#define __IVsFileUpgrade_FWD_DEFINED__
typedef interface IVsFileUpgrade IVsFileUpgrade;
#endif 	/* __IVsFileUpgrade_FWD_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory_FWD_DEFINED__
#define __IVsProjectUpgradeViaFactory_FWD_DEFINED__
typedef interface IVsProjectUpgradeViaFactory IVsProjectUpgradeViaFactory;
#endif 	/* __IVsProjectUpgradeViaFactory_FWD_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory2_FWD_DEFINED__
#define __IVsProjectUpgradeViaFactory2_FWD_DEFINED__
typedef interface IVsProjectUpgradeViaFactory2 IVsProjectUpgradeViaFactory2;
#endif 	/* __IVsProjectUpgradeViaFactory2_FWD_DEFINED__ */


#ifndef __IVsSolutionEventsProjectUpgrade_FWD_DEFINED__
#define __IVsSolutionEventsProjectUpgrade_FWD_DEFINED__
typedef interface IVsSolutionEventsProjectUpgrade IVsSolutionEventsProjectUpgrade;
#endif 	/* __IVsSolutionEventsProjectUpgrade_FWD_DEFINED__ */


#ifndef __IVsActivityLog_FWD_DEFINED__
#define __IVsActivityLog_FWD_DEFINED__
typedef interface IVsActivityLog IVsActivityLog;
#endif 	/* __IVsActivityLog_FWD_DEFINED__ */


#ifndef __SVsActivityLog_FWD_DEFINED__
#define __SVsActivityLog_FWD_DEFINED__
typedef interface SVsActivityLog SVsActivityLog;
#endif 	/* __SVsActivityLog_FWD_DEFINED__ */


#ifndef __IVsPersistDocData3_FWD_DEFINED__
#define __IVsPersistDocData3_FWD_DEFINED__
typedef interface IVsPersistDocData3 IVsPersistDocData3;
#endif 	/* __IVsPersistDocData3_FWD_DEFINED__ */


#ifndef __IVsWindowFrame2_FWD_DEFINED__
#define __IVsWindowFrame2_FWD_DEFINED__
typedef interface IVsWindowFrame2 IVsWindowFrame2;
#endif 	/* __IVsWindowFrame2_FWD_DEFINED__ */


#ifndef __IVsPropertyPage2_FWD_DEFINED__
#define __IVsPropertyPage2_FWD_DEFINED__
typedef interface IVsPropertyPage2 IVsPropertyPage2;
#endif 	/* __IVsPropertyPage2_FWD_DEFINED__ */


#ifndef __IVsWindowFrameNotify3_FWD_DEFINED__
#define __IVsWindowFrameNotify3_FWD_DEFINED__
typedef interface IVsWindowFrameNotify3 IVsWindowFrameNotify3;
#endif 	/* __IVsWindowFrameNotify3_FWD_DEFINED__ */


#ifndef __IVsPackageDynamicToolOwnerEx_FWD_DEFINED__
#define __IVsPackageDynamicToolOwnerEx_FWD_DEFINED__
typedef interface IVsPackageDynamicToolOwnerEx IVsPackageDynamicToolOwnerEx;
#endif 	/* __IVsPackageDynamicToolOwnerEx_FWD_DEFINED__ */


#ifndef __IVsContextualIntellisenseFilter_FWD_DEFINED__
#define __IVsContextualIntellisenseFilter_FWD_DEFINED__
typedef interface IVsContextualIntellisenseFilter IVsContextualIntellisenseFilter;
#endif 	/* __IVsContextualIntellisenseFilter_FWD_DEFINED__ */


#ifndef __IVsContextualIntellisenseFilterProvider_FWD_DEFINED__
#define __IVsContextualIntellisenseFilterProvider_FWD_DEFINED__
typedef interface IVsContextualIntellisenseFilterProvider IVsContextualIntellisenseFilterProvider;
#endif 	/* __IVsContextualIntellisenseFilterProvider_FWD_DEFINED__ */


#ifndef __IVsToolboxActiveUserHook_FWD_DEFINED__
#define __IVsToolboxActiveUserHook_FWD_DEFINED__
typedef interface IVsToolboxActiveUserHook IVsToolboxActiveUserHook;
#endif 	/* __IVsToolboxActiveUserHook_FWD_DEFINED__ */


#ifndef __IVsDefaultToolboxTabState_FWD_DEFINED__
#define __IVsDefaultToolboxTabState_FWD_DEFINED__
typedef interface IVsDefaultToolboxTabState IVsDefaultToolboxTabState;
#endif 	/* __IVsDefaultToolboxTabState_FWD_DEFINED__ */


#ifndef __IVsPathVariableResolver_FWD_DEFINED__
#define __IVsPathVariableResolver_FWD_DEFINED__
typedef interface IVsPathVariableResolver IVsPathVariableResolver;
#endif 	/* __IVsPathVariableResolver_FWD_DEFINED__ */


#ifndef __SVsPathVariableResolver_FWD_DEFINED__
#define __SVsPathVariableResolver_FWD_DEFINED__
typedef interface SVsPathVariableResolver SVsPathVariableResolver;
#endif 	/* __SVsPathVariableResolver_FWD_DEFINED__ */


#ifndef __IVsProjectFactory2_FWD_DEFINED__
#define __IVsProjectFactory2_FWD_DEFINED__
typedef interface IVsProjectFactory2 IVsProjectFactory2;
#endif 	/* __IVsProjectFactory2_FWD_DEFINED__ */


#ifndef __IVsAsynchOpenFromSccProjectEvents_FWD_DEFINED__
#define __IVsAsynchOpenFromSccProjectEvents_FWD_DEFINED__
typedef interface IVsAsynchOpenFromSccProjectEvents IVsAsynchOpenFromSccProjectEvents;
#endif 	/* __IVsAsynchOpenFromSccProjectEvents_FWD_DEFINED__ */


#ifndef __IVsAsynchOpenFromScc_FWD_DEFINED__
#define __IVsAsynchOpenFromScc_FWD_DEFINED__
typedef interface IVsAsynchOpenFromScc IVsAsynchOpenFromScc;
#endif 	/* __IVsAsynchOpenFromScc_FWD_DEFINED__ */


#ifndef __IVsHierarchyDeleteHandler2_FWD_DEFINED__
#define __IVsHierarchyDeleteHandler2_FWD_DEFINED__
typedef interface IVsHierarchyDeleteHandler2 IVsHierarchyDeleteHandler2;
#endif 	/* __IVsHierarchyDeleteHandler2_FWD_DEFINED__ */


#ifndef __IVsToolbox3_FWD_DEFINED__
#define __IVsToolbox3_FWD_DEFINED__
typedef interface IVsToolbox3 IVsToolbox3;
#endif 	/* __IVsToolbox3_FWD_DEFINED__ */


#ifndef __IVsToolboxDataProvider2_FWD_DEFINED__
#define __IVsToolboxDataProvider2_FWD_DEFINED__
typedef interface IVsToolboxDataProvider2 IVsToolboxDataProvider2;
#endif 	/* __IVsToolboxDataProvider2_FWD_DEFINED__ */


#ifndef __IVsResourceManager_FWD_DEFINED__
#define __IVsResourceManager_FWD_DEFINED__
typedef interface IVsResourceManager IVsResourceManager;
#endif 	/* __IVsResourceManager_FWD_DEFINED__ */


#ifndef __SVsResourceManager_FWD_DEFINED__
#define __SVsResourceManager_FWD_DEFINED__
typedef interface SVsResourceManager SVsResourceManager;
#endif 	/* __SVsResourceManager_FWD_DEFINED__ */


#ifndef __IVsAddNewWebProjectItemDlg_FWD_DEFINED__
#define __IVsAddNewWebProjectItemDlg_FWD_DEFINED__
typedef interface IVsAddNewWebProjectItemDlg IVsAddNewWebProjectItemDlg;
#endif 	/* __IVsAddNewWebProjectItemDlg_FWD_DEFINED__ */


#ifndef __IVsWebProject_FWD_DEFINED__
#define __IVsWebProject_FWD_DEFINED__
typedef interface IVsWebProject IVsWebProject;
#endif 	/* __IVsWebProject_FWD_DEFINED__ */


#ifndef __IVsUIHierarchyWindow2_FWD_DEFINED__
#define __IVsUIHierarchyWindow2_FWD_DEFINED__
typedef interface IVsUIHierarchyWindow2 IVsUIHierarchyWindow2;
#endif 	/* __IVsUIHierarchyWindow2_FWD_DEFINED__ */


#ifndef __IVsProjectDataConnection_FWD_DEFINED__
#define __IVsProjectDataConnection_FWD_DEFINED__
typedef interface IVsProjectDataConnection IVsProjectDataConnection;
#endif 	/* __IVsProjectDataConnection_FWD_DEFINED__ */


#ifndef __IVsTaskList2_FWD_DEFINED__
#define __IVsTaskList2_FWD_DEFINED__
typedef interface IVsTaskList2 IVsTaskList2;
#endif 	/* __IVsTaskList2_FWD_DEFINED__ */


#ifndef __IVsTaskProvider3_FWD_DEFINED__
#define __IVsTaskProvider3_FWD_DEFINED__
typedef interface IVsTaskProvider3 IVsTaskProvider3;
#endif 	/* __IVsTaskProvider3_FWD_DEFINED__ */


#ifndef __IVsTaskItem3_FWD_DEFINED__
#define __IVsTaskItem3_FWD_DEFINED__
typedef interface IVsTaskItem3 IVsTaskItem3;
#endif 	/* __IVsTaskItem3_FWD_DEFINED__ */


#ifndef __IVsErrorList_FWD_DEFINED__
#define __IVsErrorList_FWD_DEFINED__
typedef interface IVsErrorList IVsErrorList;
#endif 	/* __IVsErrorList_FWD_DEFINED__ */


#ifndef __SVsErrorList_FWD_DEFINED__
#define __SVsErrorList_FWD_DEFINED__
typedef interface SVsErrorList SVsErrorList;
#endif 	/* __SVsErrorList_FWD_DEFINED__ */


#ifndef __IVsErrorItem_FWD_DEFINED__
#define __IVsErrorItem_FWD_DEFINED__
typedef interface IVsErrorItem IVsErrorItem;
#endif 	/* __IVsErrorItem_FWD_DEFINED__ */


#ifndef __IVsWindowPaneCommitFilter_FWD_DEFINED__
#define __IVsWindowPaneCommitFilter_FWD_DEFINED__
typedef interface IVsWindowPaneCommitFilter IVsWindowPaneCommitFilter;
#endif 	/* __IVsWindowPaneCommitFilter_FWD_DEFINED__ */


#ifndef __IPreferPropertyPagesWithTreeControl_FWD_DEFINED__
#define __IPreferPropertyPagesWithTreeControl_FWD_DEFINED__
typedef interface IPreferPropertyPagesWithTreeControl IPreferPropertyPagesWithTreeControl;
#endif 	/* __IPreferPropertyPagesWithTreeControl_FWD_DEFINED__ */


#ifndef __IVsSpecifyProjectDesignerPages_FWD_DEFINED__
#define __IVsSpecifyProjectDesignerPages_FWD_DEFINED__
typedef interface IVsSpecifyProjectDesignerPages IVsSpecifyProjectDesignerPages;
#endif 	/* __IVsSpecifyProjectDesignerPages_FWD_DEFINED__ */


#ifndef __IVsDeployDependency2_FWD_DEFINED__
#define __IVsDeployDependency2_FWD_DEFINED__
typedef interface IVsDeployDependency2 IVsDeployDependency2;
#endif 	/* __IVsDeployDependency2_FWD_DEFINED__ */


#ifndef __IVsOutputGroup2_FWD_DEFINED__
#define __IVsOutputGroup2_FWD_DEFINED__
typedef interface IVsOutputGroup2 IVsOutputGroup2;
#endif 	/* __IVsOutputGroup2_FWD_DEFINED__ */


#ifndef __IVsFontAndColorUtilities_FWD_DEFINED__
#define __IVsFontAndColorUtilities_FWD_DEFINED__
typedef interface IVsFontAndColorUtilities IVsFontAndColorUtilities;
#endif 	/* __IVsFontAndColorUtilities_FWD_DEFINED__ */


#ifndef __IVsOutputWindow2_FWD_DEFINED__
#define __IVsOutputWindow2_FWD_DEFINED__
typedef interface IVsOutputWindow2 IVsOutputWindow2;
#endif 	/* __IVsOutputWindow2_FWD_DEFINED__ */


#ifndef __IVsDebuggableProjectCfg2_FWD_DEFINED__
#define __IVsDebuggableProjectCfg2_FWD_DEFINED__
typedef interface IVsDebuggableProjectCfg2 IVsDebuggableProjectCfg2;
#endif 	/* __IVsDebuggableProjectCfg2_FWD_DEFINED__ */


#ifndef __IVsProvideUserContext2_FWD_DEFINED__
#define __IVsProvideUserContext2_FWD_DEFINED__
typedef interface IVsProvideUserContext2 IVsProvideUserContext2;
#endif 	/* __IVsProvideUserContext2_FWD_DEFINED__ */


#ifndef __IVsExtensibility3_FWD_DEFINED__
#define __IVsExtensibility3_FWD_DEFINED__
typedef interface IVsExtensibility3 IVsExtensibility3;
#endif 	/* __IVsExtensibility3_FWD_DEFINED__ */


#ifndef __IVsGlobalsCallback2_FWD_DEFINED__
#define __IVsGlobalsCallback2_FWD_DEFINED__
typedef interface IVsGlobalsCallback2 IVsGlobalsCallback2;
#endif 	/* __IVsGlobalsCallback2_FWD_DEFINED__ */


#ifndef __IVsGlobals2_FWD_DEFINED__
#define __IVsGlobals2_FWD_DEFINED__
typedef interface IVsGlobals2 IVsGlobals2;
#endif 	/* __IVsGlobals2_FWD_DEFINED__ */


#ifndef __IVsProfferCommands3_FWD_DEFINED__
#define __IVsProfferCommands3_FWD_DEFINED__
typedef interface IVsProfferCommands3 IVsProfferCommands3;
#endif 	/* __IVsProfferCommands3_FWD_DEFINED__ */


#ifndef __IVsHierarchyRefactorNotify_FWD_DEFINED__
#define __IVsHierarchyRefactorNotify_FWD_DEFINED__
typedef interface IVsHierarchyRefactorNotify IVsHierarchyRefactorNotify;
#endif 	/* __IVsHierarchyRefactorNotify_FWD_DEFINED__ */


#ifndef __IVsRefactorNotify_FWD_DEFINED__
#define __IVsRefactorNotify_FWD_DEFINED__
typedef interface IVsRefactorNotify IVsRefactorNotify;
#endif 	/* __IVsRefactorNotify_FWD_DEFINED__ */


#ifndef __IVsMonitorSelection2_FWD_DEFINED__
#define __IVsMonitorSelection2_FWD_DEFINED__
typedef interface IVsMonitorSelection2 IVsMonitorSelection2;
#endif 	/* __IVsMonitorSelection2_FWD_DEFINED__ */


#ifndef __IVsToolsOptions_FWD_DEFINED__
#define __IVsToolsOptions_FWD_DEFINED__
typedef interface IVsToolsOptions IVsToolsOptions;
#endif 	/* __IVsToolsOptions_FWD_DEFINED__ */


#ifndef __SVsToolsOptions_FWD_DEFINED__
#define __SVsToolsOptions_FWD_DEFINED__
typedef interface SVsToolsOptions SVsToolsOptions;
#endif 	/* __SVsToolsOptions_FWD_DEFINED__ */


#ifndef __IVsDeployableProjectCfg2_FWD_DEFINED__
#define __IVsDeployableProjectCfg2_FWD_DEFINED__
typedef interface IVsDeployableProjectCfg2 IVsDeployableProjectCfg2;
#endif 	/* __IVsDeployableProjectCfg2_FWD_DEFINED__ */


#ifndef __IVsFontAndColorStorage2_FWD_DEFINED__
#define __IVsFontAndColorStorage2_FWD_DEFINED__
typedef interface IVsFontAndColorStorage2 IVsFontAndColorStorage2;
#endif 	/* __IVsFontAndColorStorage2_FWD_DEFINED__ */


#ifndef __IVsDocOutlineProvider2_FWD_DEFINED__
#define __IVsDocOutlineProvider2_FWD_DEFINED__
typedef interface IVsDocOutlineProvider2 IVsDocOutlineProvider2;
#endif 	/* __IVsDocOutlineProvider2_FWD_DEFINED__ */


#ifndef __IVSMDTypeResolutionService_FWD_DEFINED__
#define __IVSMDTypeResolutionService_FWD_DEFINED__
typedef interface IVSMDTypeResolutionService IVSMDTypeResolutionService;
#endif 	/* __IVSMDTypeResolutionService_FWD_DEFINED__ */


#ifndef __IVsUIShellOpenDocument2_FWD_DEFINED__
#define __IVsUIShellOpenDocument2_FWD_DEFINED__
typedef interface IVsUIShellOpenDocument2 IVsUIShellOpenDocument2;
#endif 	/* __IVsUIShellOpenDocument2_FWD_DEFINED__ */


#ifndef __IVsFilterNewProjectDlg_FWD_DEFINED__
#define __IVsFilterNewProjectDlg_FWD_DEFINED__
typedef interface IVsFilterNewProjectDlg IVsFilterNewProjectDlg;
#endif 	/* __IVsFilterNewProjectDlg_FWD_DEFINED__ */


#ifndef __IVsRegisterNewDialogFilters_FWD_DEFINED__
#define __IVsRegisterNewDialogFilters_FWD_DEFINED__
typedef interface IVsRegisterNewDialogFilters IVsRegisterNewDialogFilters;
#endif 	/* __IVsRegisterNewDialogFilters_FWD_DEFINED__ */


#ifndef __SVsRegisterNewDialogFilters_FWD_DEFINED__
#define __SVsRegisterNewDialogFilters_FWD_DEFINED__
typedef interface SVsRegisterNewDialogFilters SVsRegisterNewDialogFilters;
#endif 	/* __SVsRegisterNewDialogFilters_FWD_DEFINED__ */


#ifndef __IVsWebBrowserUser2_FWD_DEFINED__
#define __IVsWebBrowserUser2_FWD_DEFINED__
typedef interface IVsWebBrowserUser2 IVsWebBrowserUser2;
#endif 	/* __IVsWebBrowserUser2_FWD_DEFINED__ */


#ifndef __IVsHasRelatedSaveItems_FWD_DEFINED__
#define __IVsHasRelatedSaveItems_FWD_DEFINED__
typedef interface IVsHasRelatedSaveItems IVsHasRelatedSaveItems;
#endif 	/* __IVsHasRelatedSaveItems_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"
#include "vsshell2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell80_0000_0000 */
/* [local] */ 

#if 0
typedef DWORD_PTR DLGPROC;

typedef DWORD_PTR LPFNPSPCALLBACKA;

typedef DWORD_PTR HINSTANCE;

#endif
#include <prsht.h>
#pragma once
#pragma once
#pragma once
#pragma once

enum VSErrorCodes80
    {	VS_E_MIGRATIONREQUIRESRELOAD	= ( HRESULT  )(( ( ( ( unsigned long  )1 << 31 )  | ( ( unsigned long  )4 << 16 )  )  | ( unsigned long  )0x1fe6 ) ),
	VS_E_SYNCHRONOUSOPENREQUIRED	= ( HRESULT  )(( ( ( ( unsigned long  )1 << 31 )  | ( ( unsigned long  )4 << 16 )  )  | ( unsigned long  )0x1fe7 ) ),
	VS_E_VSSETTINGS_INVALIDVERSION	= ( HRESULT  )(( ( ( ( unsigned long  )1 << 31 )  | ( ( unsigned long  )4 << 16 )  )  | ( unsigned long  )0x1fe8 ) ),
	VS_E_DOCUMENTOPENNOTTRUSTED	= ( HRESULT  )(( ( ( ( unsigned long  )1 << 31 )  | ( ( unsigned long  )4 << 16 )  )  | ( unsigned long  )0x1fe9 ) )
    } ;

enum __VSFORMATINDEX
    {	VSFORMATINDEX_UTF8	= 0,
	VSFORMATINDEX_MBCS	= 1,
	VSFORMATINDEX_UNICODE	= 2
    } ;
typedef ULONG VSFORMATINDEX;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0000_v0_0_s_ifspec;

#ifndef __IVsProjectDebugTargetProvider_INTERFACE_DEFINED__
#define __IVsProjectDebugTargetProvider_INTERFACE_DEFINED__

/* interface IVsProjectDebugTargetProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectDebugTargetProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C5F0CEB-5AC9-4ea4-85E2-72E088EA75A8")
    IVsProjectDebugTargetProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SupplyDebugTarget( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTarget,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectDebugTargetProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectDebugTargetProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectDebugTargetProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectDebugTargetProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *SupplyDebugTarget )( 
            IVsProjectDebugTargetProvider * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTarget,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine);
        
        END_INTERFACE
    } IVsProjectDebugTargetProviderVtbl;

    interface IVsProjectDebugTargetProvider
    {
        CONST_VTBL struct IVsProjectDebugTargetProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectDebugTargetProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectDebugTargetProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectDebugTargetProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectDebugTargetProvider_SupplyDebugTarget(This,pbstrTarget,pbstrCommandLine)	\
    ( (This)->lpVtbl -> SupplyDebugTarget(This,pbstrTarget,pbstrCommandLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectDebugTargetProvider_INTERFACE_DEFINED__ */


#ifndef __IVsRegisterProjectDebugTargetProvider_INTERFACE_DEFINED__
#define __IVsRegisterProjectDebugTargetProvider_INTERFACE_DEFINED__

/* interface IVsRegisterProjectDebugTargetProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterProjectDebugTargetProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C5F0CEA-5AC9-4ea4-85E2-72E088EA75A8")
    IVsRegisterProjectDebugTargetProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddDebugTargetProvider( 
            /* [in] */ __RPC__in_opt IVsProjectDebugTargetProvider *pNewDbgTrgtProvider,
            /* [out] */ __RPC__deref_out_opt IVsProjectDebugTargetProvider **ppNextDbgTrgtProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveDebugTargetProvider( 
            /* [in] */ __RPC__in_opt IVsProjectDebugTargetProvider *pDbgTrgtProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterProjectDebugTargetProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterProjectDebugTargetProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterProjectDebugTargetProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterProjectDebugTargetProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddDebugTargetProvider )( 
            IVsRegisterProjectDebugTargetProvider * This,
            /* [in] */ __RPC__in_opt IVsProjectDebugTargetProvider *pNewDbgTrgtProvider,
            /* [out] */ __RPC__deref_out_opt IVsProjectDebugTargetProvider **ppNextDbgTrgtProvider);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveDebugTargetProvider )( 
            IVsRegisterProjectDebugTargetProvider * This,
            /* [in] */ __RPC__in_opt IVsProjectDebugTargetProvider *pDbgTrgtProvider);
        
        END_INTERFACE
    } IVsRegisterProjectDebugTargetProviderVtbl;

    interface IVsRegisterProjectDebugTargetProvider
    {
        CONST_VTBL struct IVsRegisterProjectDebugTargetProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterProjectDebugTargetProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterProjectDebugTargetProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterProjectDebugTargetProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterProjectDebugTargetProvider_AddDebugTargetProvider(This,pNewDbgTrgtProvider,ppNextDbgTrgtProvider)	\
    ( (This)->lpVtbl -> AddDebugTargetProvider(This,pNewDbgTrgtProvider,ppNextDbgTrgtProvider) ) 

#define IVsRegisterProjectDebugTargetProvider_RemoveDebugTargetProvider(This,pDbgTrgtProvider)	\
    ( (This)->lpVtbl -> RemoveDebugTargetProvider(This,pDbgTrgtProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterProjectDebugTargetProvider_INTERFACE_DEFINED__ */


#ifndef __SVsRegisterDebugTargetProvider_INTERFACE_DEFINED__
#define __SVsRegisterDebugTargetProvider_INTERFACE_DEFINED__

/* interface SVsRegisterDebugTargetProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsRegisterDebugTargetProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B25ED7D-E462-4bba-B181-81D9F73FCD72")
    SVsRegisterDebugTargetProvider : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsRegisterDebugTargetProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsRegisterDebugTargetProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsRegisterDebugTargetProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsRegisterDebugTargetProvider * This);
        
        END_INTERFACE
    } SVsRegisterDebugTargetProviderVtbl;

    interface SVsRegisterDebugTargetProvider
    {
        CONST_VTBL struct SVsRegisterDebugTargetProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsRegisterDebugTargetProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsRegisterDebugTargetProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsRegisterDebugTargetProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsRegisterDebugTargetProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0003 */
/* [local] */ 

#define SID_SVsRegisterDebugTargetProvider IID_SVsRegisterDebugTargetProvider
extern const __declspec(selectany) GUID GUID_BookmarkWindow =     { 0xa0c5197d, 0xac7, 0x4b63, { 0x97, 0xcd, 0x88, 0x72, 0xa7, 0x89, 0xd2, 0x33 } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0003_v0_0_s_ifspec;

#ifndef __IVsBrowseObject_INTERFACE_DEFINED__
#define __IVsBrowseObject_INTERFACE_DEFINED__

/* interface IVsBrowseObject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBrowseObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("bc5b644e-7fd7-4a75-98cc-0c2c98aa96f6")
    IVsBrowseObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProjectItem( 
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pHier,
            /* [out] */ __RPC__out VSITEMID *pItemid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBrowseObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBrowseObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBrowseObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBrowseObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectItem )( 
            IVsBrowseObject * This,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pHier,
            /* [out] */ __RPC__out VSITEMID *pItemid);
        
        END_INTERFACE
    } IVsBrowseObjectVtbl;

    interface IVsBrowseObject
    {
        CONST_VTBL struct IVsBrowseObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBrowseObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBrowseObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBrowseObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBrowseObject_GetProjectItem(This,pHier,pItemid)	\
    ( (This)->lpVtbl -> GetProjectItem(This,pHier,pItemid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBrowseObject_INTERFACE_DEFINED__ */


#ifndef __IVsCfgBrowseObject_INTERFACE_DEFINED__
#define __IVsCfgBrowseObject_INTERFACE_DEFINED__

/* interface IVsCfgBrowseObject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCfgBrowseObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a630cff5-eb22-40b7-9464-5f8d4b98b1cb")
    IVsCfgBrowseObject : public IVsBrowseObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCfg( 
            /* [out] */ __RPC__deref_out_opt IVsCfg **ppCfg) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCfgBrowseObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCfgBrowseObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCfgBrowseObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCfgBrowseObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectItem )( 
            IVsCfgBrowseObject * This,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pHier,
            /* [out] */ __RPC__out VSITEMID *pItemid);
        
        HRESULT ( STDMETHODCALLTYPE *GetCfg )( 
            IVsCfgBrowseObject * This,
            /* [out] */ __RPC__deref_out_opt IVsCfg **ppCfg);
        
        END_INTERFACE
    } IVsCfgBrowseObjectVtbl;

    interface IVsCfgBrowseObject
    {
        CONST_VTBL struct IVsCfgBrowseObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCfgBrowseObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCfgBrowseObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCfgBrowseObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCfgBrowseObject_GetProjectItem(This,pHier,pItemid)	\
    ( (This)->lpVtbl -> GetProjectItem(This,pHier,pItemid) ) 


#define IVsCfgBrowseObject_GetCfg(This,ppCfg)	\
    ( (This)->lpVtbl -> GetCfg(This,ppCfg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCfgBrowseObject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0005 */
/* [local] */ 

extern const __declspec(selectany) CLSID CLSID_SolutionFolderProject = { 0x2150e333, 0x8fdc, 0x42a3, { 0x94, 0x74, 0x1a, 0x39, 0x56, 0xd4, 0x6d, 0xe8 } };

enum __VSHPROPID2
    {	VSHPROPID_PropertyPagesCLSIDList	= -2065,
	VSHPROPID_CfgPropertyPagesCLSIDList	= -2066,
	VSHPROPID_ExtObjectCATID	= -2067,
	VSHPROPID_BrowseObjectCATID	= -2068,
	VSHPROPID_CfgBrowseObjectCATID	= -2069,
	VSHPROPID_AddItemTemplatesGuid	= -2070,
	VSHPROPID_ChildrenEnumerated	= -2071,
	VSHPROPID_StatusBarClientText	= -2072,
	VSHPROPID_DebuggeeProcessId	= -2073,
	VSHPROPID_IsLinkFile	= -2074,
	VSHPROPID_KeepAliveDocument	= -2075,
	VSHPROPID_SupportsProjectDesigner	= -2076,
	VSHPROPID_IntellisenseUnknown	= -2077,
	VSHPROPID_IsUpgradeRequired	= -2078,
	VSHPROPID_DesignerHiddenCodeGeneration	= -2079,
	VSHPROPID_SuppressOutOfDateMessageOnBuild	= -2080,
	VSHPROPID_Container	= -2081,
	VSHPROPID_UseInnerHierarchyIconList	= -2082,
	VSHPROPID_EnableDataSourceWindow	= -2083,
	VSHPROPID_AppTitleBarTopHierarchyName	= -2084,
	VSHPROPID_DebuggerSourcePaths	= -2085,
	VSHPROPID_CategoryGuid	= -2086,
	VSHPROPID_DisableApplicationSettings	= -2087,
	VSHPROPID_ProjectDesignerEditor	= -2088,
	VSHPROPID_PriorityPropertyPagesCLSIDList	= -2089,
	VSHPROPID_NoDefaultNestedHierSorting	= -2090,
	VSHPROPID_ExcludeFromExportItemTemplate	= -2091,
	VSHPROPID_SupportedMyApplicationTypes	= -2092,
	VSHPROPID_FIRST2	= -2092
    } ;
typedef /* [public] */ DWORD VSHPROPID2;


enum __VSDESIGNER_HIDDENCODEGENERATION
    {	VSDHCG_Declarations	= 1,
	VSDHCG_InitMethods	= 2,
	VSDHCG_EventMethods	= 4
    } ;

enum _ProjectLoadSecurityDialogState
    {	PLSDS_ShowAgain	= 1,
	PLSDS_DontShowAgainBrowse	= 2,
	PLSDS_DontShowAgainFullLoad	= 3,
	PLSDS_DontShowAgainUnload	= 4
    } ;
typedef DWORD ProjectLoadSecurityDialogState;


enum __VSPROPID2
    {	VSPROPID_IsSolutionNodeHidden	= -8017,
	VSPROPID_DeferredSaveSolution	= -8018,
	VSPROPID_SimplifiedConfigurations	= -8019,
	VSPROPID_IsSolutionClosing	= -8020,
	VSPROPID_IsAProjectClosing	= -8021,
	VSPROPID_IsSolutionOpeningDocs	= -8022,
	VSPROPID_IsOpenNotificationPending	= -8023,
	VSPROPID_ProjectLoadSecurityDialogState	= -8024,
	VSPROPID_SolutionUserFileCreatedOnThisComputer	= -8025,
	VSPROPID_NewProjectDlgPreferredLanguage	= -8026,
	VSPROPID_FIRST2	= -8026
    } ;
typedef /* [public] */ DWORD VSPROPID2;


enum __VSCFGPROPID2
    {	VSCFGPROPID_HideConfigurations	= -16009,
	VSCFGPROPID_FIRST2	= -16009
    } ;
typedef LONG VSCFGPROPID2;


enum __VSCREATEPROJFLAGS2
    {	CPF_DEFERREDSAVE	= 0x80,
	CPF_OPEN_ASYNCHRONOUSLY	= 0x100,
	CPF_OPEN_STANDALONE	= 0x200
    } ;
typedef DWORD VSCREATEPROJFLAGS2;


enum __VSCREATESOLUTIONFLAGS2
    {	CSF_HIDESOLUTIONNODEALWAYS	= 0x10,
	CSF_DEFERREDSAVESOLUTION	= 0x20
    } ;
typedef /* [public] */ DWORD VSCREATESOLUTIONFLAGS2;


enum __VSSLNOPENOPTIONS2
    {	SLNOPENOPT_LoadingAsync	= 0x8
    } ;
typedef DWORD VSSLNOPENOPTIONS2;


enum __VSADDITEMFLAGS2
    {	VSADDITEM_NoUserTemplateFeatures	= 0x1000,
	VSADDITEM_ShowOpenButtonDropDown	= 0x40000
    } ;
typedef DWORD VSADDITEMFLAGS2;


enum __VSCREATEEDITORFLAGS2
    {	CEF_OPENSPECIFIC	= 0x10
    } ;
typedef DWORD VSCREATEEDITORFLAGS2;


enum __VSOSEFLAGS2
    {	OSE_CheckTrustLevelOfWizard	= 0x100000
    } ;
typedef DWORD VSOSEFLAGS2;


enum __VSOSPEFLAGS2
    {	OSPE_CheckTrustLevelOfWizard	= 0x100000
    } ;
typedef DWORD VSOSPEFLAGS2;


enum __VSWIZARDTRUSTLEVEL
    {	WTL_Trusted	= 1,
	WTL_Untrusted	= 2,
	WTL_Unspecified	= 3
    } ;
typedef DWORD VSWIZARDTRUSTLEVEL;


enum __VSEDITORTRUSTLEVEL
    {	ETL_NeverTrusted	= 0,
	ETL_AlwaysTrusted	= 1,
	ETL_HasUntrustedLogicalViews	= 2
    } ;
typedef DWORD VSEDITORTRUSTLEVEL;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0005_v0_0_s_ifspec;

#ifndef __IVsDetermineWizardTrust_INTERFACE_DEFINED__
#define __IVsDetermineWizardTrust_INTERFACE_DEFINED__

/* interface IVsDetermineWizardTrust */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDetermineWizardTrust;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("42085C99-3F5B-4b61-9737-592479718CEC")
    IVsDetermineWizardTrust : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnWizardInitiated( 
            /* [in] */ __RPC__in LPCOLESTR pszTemplateFilename,
            /* [in] */ __RPC__in REFGUID guidProjectType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnWizardCompleted( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsWizardRunning( 
            /* [out] */ __RPC__out BOOL *pfWizardRunning) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRunningWizardTemplateName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRunningTemplate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWizardTrustLevel( 
            /* [out] */ __RPC__out VSWIZARDTRUSTLEVEL *pdwWizardTrustLevel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetWizardTrustLevel( 
            /* [in] */ VSWIZARDTRUSTLEVEL dwWizardTrustLevel) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDetermineWizardTrustVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDetermineWizardTrust * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDetermineWizardTrust * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDetermineWizardTrust * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnWizardInitiated )( 
            IVsDetermineWizardTrust * This,
            /* [in] */ __RPC__in LPCOLESTR pszTemplateFilename,
            /* [in] */ __RPC__in REFGUID guidProjectType);
        
        HRESULT ( STDMETHODCALLTYPE *OnWizardCompleted )( 
            IVsDetermineWizardTrust * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsWizardRunning )( 
            IVsDetermineWizardTrust * This,
            /* [out] */ __RPC__out BOOL *pfWizardRunning);
        
        HRESULT ( STDMETHODCALLTYPE *GetRunningWizardTemplateName )( 
            IVsDetermineWizardTrust * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRunningTemplate);
        
        HRESULT ( STDMETHODCALLTYPE *GetWizardTrustLevel )( 
            IVsDetermineWizardTrust * This,
            /* [out] */ __RPC__out VSWIZARDTRUSTLEVEL *pdwWizardTrustLevel);
        
        HRESULT ( STDMETHODCALLTYPE *SetWizardTrustLevel )( 
            IVsDetermineWizardTrust * This,
            /* [in] */ VSWIZARDTRUSTLEVEL dwWizardTrustLevel);
        
        END_INTERFACE
    } IVsDetermineWizardTrustVtbl;

    interface IVsDetermineWizardTrust
    {
        CONST_VTBL struct IVsDetermineWizardTrustVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDetermineWizardTrust_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDetermineWizardTrust_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDetermineWizardTrust_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDetermineWizardTrust_OnWizardInitiated(This,pszTemplateFilename,guidProjectType)	\
    ( (This)->lpVtbl -> OnWizardInitiated(This,pszTemplateFilename,guidProjectType) ) 

#define IVsDetermineWizardTrust_OnWizardCompleted(This)	\
    ( (This)->lpVtbl -> OnWizardCompleted(This) ) 

#define IVsDetermineWizardTrust_IsWizardRunning(This,pfWizardRunning)	\
    ( (This)->lpVtbl -> IsWizardRunning(This,pfWizardRunning) ) 

#define IVsDetermineWizardTrust_GetRunningWizardTemplateName(This,pbstrRunningTemplate)	\
    ( (This)->lpVtbl -> GetRunningWizardTemplateName(This,pbstrRunningTemplate) ) 

#define IVsDetermineWizardTrust_GetWizardTrustLevel(This,pdwWizardTrustLevel)	\
    ( (This)->lpVtbl -> GetWizardTrustLevel(This,pdwWizardTrustLevel) ) 

#define IVsDetermineWizardTrust_SetWizardTrustLevel(This,dwWizardTrustLevel)	\
    ( (This)->lpVtbl -> SetWizardTrustLevel(This,dwWizardTrustLevel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDetermineWizardTrust_INTERFACE_DEFINED__ */


#ifndef __SVsDetermineWizardTrust_INTERFACE_DEFINED__
#define __SVsDetermineWizardTrust_INTERFACE_DEFINED__

/* interface SVsDetermineWizardTrust */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDetermineWizardTrust;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9A3B199D-2294-4da6-9B43-7A4EAFE31FA0")
    SVsDetermineWizardTrust : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsDetermineWizardTrustVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsDetermineWizardTrust * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsDetermineWizardTrust * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsDetermineWizardTrust * This);
        
        END_INTERFACE
    } SVsDetermineWizardTrustVtbl;

    interface SVsDetermineWizardTrust
    {
        CONST_VTBL struct SVsDetermineWizardTrustVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDetermineWizardTrust_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDetermineWizardTrust_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDetermineWizardTrust_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDetermineWizardTrust_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0007 */
/* [local] */ 

#define SID_SVsDetermineWizardTrust IID_SVsDetermineWizardTrust


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0007_v0_0_s_ifspec;

#ifndef __IVsSolutionEvents4_INTERFACE_DEFINED__
#define __IVsSolutionEvents4_INTERFACE_DEFINED__

/* interface IVsSolutionEvents4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionEvents4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("23EC4D20-54A9-4365-82C8-ABDFBA686ECF")
    IVsSolutionEvents4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryChangeProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pNewParentHier,
            /* [out][in] */ __RPC__inout BOOL *pfCancel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterChangeProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAsynchOpenProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSolutionEvents4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSolutionEvents4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSolutionEvents4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSolutionEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameProject )( 
            IVsSolutionEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryChangeProjectParent )( 
            IVsSolutionEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pNewParentHier,
            /* [out][in] */ __RPC__inout BOOL *pfCancel);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterChangeProjectParent )( 
            IVsSolutionEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAsynchOpenProject )( 
            IVsSolutionEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded);
        
        END_INTERFACE
    } IVsSolutionEvents4Vtbl;

    interface IVsSolutionEvents4
    {
        CONST_VTBL struct IVsSolutionEvents4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionEvents4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionEvents4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionEvents4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionEvents4_OnAfterRenameProject(This,pHierarchy)	\
    ( (This)->lpVtbl -> OnAfterRenameProject(This,pHierarchy) ) 

#define IVsSolutionEvents4_OnQueryChangeProjectParent(This,pHierarchy,pNewParentHier,pfCancel)	\
    ( (This)->lpVtbl -> OnQueryChangeProjectParent(This,pHierarchy,pNewParentHier,pfCancel) ) 

#define IVsSolutionEvents4_OnAfterChangeProjectParent(This,pHierarchy)	\
    ( (This)->lpVtbl -> OnAfterChangeProjectParent(This,pHierarchy) ) 

#define IVsSolutionEvents4_OnAfterAsynchOpenProject(This,pHierarchy,fAdded)	\
    ( (This)->lpVtbl -> OnAfterAsynchOpenProject(This,pHierarchy,fAdded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionEvents4_INTERFACE_DEFINED__ */


#ifndef __IVsFireSolutionEvents2_INTERFACE_DEFINED__
#define __IVsFireSolutionEvents2_INTERFACE_DEFINED__

/* interface IVsFireSolutionEvents2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFireSolutionEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ED6AAB26-108F-4b4f-A57B-14D20982713D")
    IVsFireSolutionEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE FireOnAfterRenameProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FireOnQueryChangeProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pNewParentHier) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FireOnAfterChangeProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FireOnAfterAsynchOpenProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFireSolutionEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFireSolutionEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFireSolutionEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFireSolutionEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnAfterRenameProject )( 
            IVsFireSolutionEvents2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnQueryChangeProjectParent )( 
            IVsFireSolutionEvents2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pNewParentHier);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnAfterChangeProjectParent )( 
            IVsFireSolutionEvents2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnAfterAsynchOpenProject )( 
            IVsFireSolutionEvents2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded);
        
        END_INTERFACE
    } IVsFireSolutionEvents2Vtbl;

    interface IVsFireSolutionEvents2
    {
        CONST_VTBL struct IVsFireSolutionEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFireSolutionEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFireSolutionEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFireSolutionEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFireSolutionEvents2_FireOnAfterRenameProject(This,pHierarchy)	\
    ( (This)->lpVtbl -> FireOnAfterRenameProject(This,pHierarchy) ) 

#define IVsFireSolutionEvents2_FireOnQueryChangeProjectParent(This,pHierarchy,pNewParentHier)	\
    ( (This)->lpVtbl -> FireOnQueryChangeProjectParent(This,pHierarchy,pNewParentHier) ) 

#define IVsFireSolutionEvents2_FireOnAfterChangeProjectParent(This,pHierarchy)	\
    ( (This)->lpVtbl -> FireOnAfterChangeProjectParent(This,pHierarchy) ) 

#define IVsFireSolutionEvents2_FireOnAfterAsynchOpenProject(This,pHierarchy,fAdded)	\
    ( (This)->lpVtbl -> FireOnAfterAsynchOpenProject(This,pHierarchy,fAdded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFireSolutionEvents2_INTERFACE_DEFINED__ */


#ifndef __IVsPrioritizedSolutionEvents_INTERFACE_DEFINED__
#define __IVsPrioritizedSolutionEvents_INTERFACE_DEFINED__

/* interface IVsPrioritizedSolutionEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPrioritizedSolutionEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("925E8559-17DF-494c-87DA-BBEE251702DE")
    IVsPrioritizedSolutionEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterOpenProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnBeforeCloseProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fRemoved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterLoadProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pStubHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pRealHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnBeforeUnloadProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pRealHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pStubHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterOpenSolution( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved,
            /* [in] */ BOOL fNewSolution) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnBeforeCloseSolution( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterCloseSolution( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterMergeSolution( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnBeforeOpeningChildren( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterOpeningChildren( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnBeforeClosingChildren( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterClosingChildren( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterRenameProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterChangeProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PrioritizedOnAfterAsynchOpenProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPrioritizedSolutionEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPrioritizedSolutionEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPrioritizedSolutionEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterOpenProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnBeforeCloseProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fRemoved);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterLoadProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pStubHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pRealHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnBeforeUnloadProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pRealHierarchy,
            /* [in] */ __RPC__in_opt IVsHierarchy *pStubHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterOpenSolution )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved,
            /* [in] */ BOOL fNewSolution);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnBeforeCloseSolution )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterCloseSolution )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterMergeSolution )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkReserved);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnBeforeOpeningChildren )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterOpeningChildren )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnBeforeClosingChildren )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterClosingChildren )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterRenameProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterChangeProjectParent )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *PrioritizedOnAfterAsynchOpenProject )( 
            IVsPrioritizedSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ BOOL fAdded);
        
        END_INTERFACE
    } IVsPrioritizedSolutionEventsVtbl;

    interface IVsPrioritizedSolutionEvents
    {
        CONST_VTBL struct IVsPrioritizedSolutionEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPrioritizedSolutionEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPrioritizedSolutionEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPrioritizedSolutionEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterOpenProject(This,pHierarchy,fAdded)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterOpenProject(This,pHierarchy,fAdded) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnBeforeCloseProject(This,pHierarchy,fRemoved)	\
    ( (This)->lpVtbl -> PrioritizedOnBeforeCloseProject(This,pHierarchy,fRemoved) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterLoadProject(This,pStubHierarchy,pRealHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterLoadProject(This,pStubHierarchy,pRealHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnBeforeUnloadProject(This,pRealHierarchy,pStubHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnBeforeUnloadProject(This,pRealHierarchy,pStubHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterOpenSolution(This,pUnkReserved,fNewSolution)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterOpenSolution(This,pUnkReserved,fNewSolution) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnBeforeCloseSolution(This,pUnkReserved)	\
    ( (This)->lpVtbl -> PrioritizedOnBeforeCloseSolution(This,pUnkReserved) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterCloseSolution(This,pUnkReserved)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterCloseSolution(This,pUnkReserved) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterMergeSolution(This,pUnkReserved)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterMergeSolution(This,pUnkReserved) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnBeforeOpeningChildren(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnBeforeOpeningChildren(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterOpeningChildren(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterOpeningChildren(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnBeforeClosingChildren(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnBeforeClosingChildren(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterClosingChildren(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterClosingChildren(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterRenameProject(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterRenameProject(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterChangeProjectParent(This,pHierarchy)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterChangeProjectParent(This,pHierarchy) ) 

#define IVsPrioritizedSolutionEvents_PrioritizedOnAfterAsynchOpenProject(This,pHierarchy,fAdded)	\
    ( (This)->lpVtbl -> PrioritizedOnAfterAsynchOpenProject(This,pHierarchy,fAdded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPrioritizedSolutionEvents_INTERFACE_DEFINED__ */


#ifndef __IVsPersistSolutionProps2_INTERFACE_DEFINED__
#define __IVsPersistSolutionProps2_INTERFACE_DEFINED__

/* interface IVsPersistSolutionProps2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPersistSolutionProps2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8D2EC486-8098-4afa-AB94-D270A5EF08CE")
    IVsPersistSolutionProps2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnSolutionLoadFailure( 
            /* [in] */ __RPC__in LPCOLESTR pszKey) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPersistSolutionProps2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPersistSolutionProps2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPersistSolutionProps2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPersistSolutionProps2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnSolutionLoadFailure )( 
            IVsPersistSolutionProps2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszKey);
        
        END_INTERFACE
    } IVsPersistSolutionProps2Vtbl;

    interface IVsPersistSolutionProps2
    {
        CONST_VTBL struct IVsPersistSolutionProps2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPersistSolutionProps2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPersistSolutionProps2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPersistSolutionProps2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPersistSolutionProps2_OnSolutionLoadFailure(This,pszKey)	\
    ( (This)->lpVtbl -> OnSolutionLoadFailure(This,pszKey) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPersistSolutionProps2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0011 */
/* [local] */ 


enum __VSDIRFLAGS2
    {	VSDIRFLAG_RequiresNewFolder	= 0x10,
	VSDIRFLAG_SolutionTemplate	= 0x40,
	VSDIRFLAG_DeferredSaveProject	= 0x80,
	VSDIRFLAG_DontShowNameLocInfo	= 0x100,
	VSDIRFLAG_EnableMasterPage	= 0x200,
	VSDIRFLAG_EnableCodeSeparation	= 0x400,
	VSDIRFLAG_EnableLangDropdown	= 0x800
    } ;
typedef DWORD VSDIRFLAGS2;

typedef 
enum __VSFRAMEMODE2
    {	VSFM_AutoHide	= 4
    } 	VSFRAMEMODE2;


enum __VSFPROPID2
    {	VSFPROPID_OverrideDirtyState	= -4014,
	VSFPROPID_OLEDocObjectDocument	= -4015,
	VSFPROPID_ParentHwnd	= -4016,
	VSFPROPID_ParentFrame	= -4017,
	VSFPROPID_ToolWindowDocCookie	= -4018
    } ;
typedef LONG VSFPROPID2;

extern const __declspec(selectany) GUID GUID_ImmediateWindow =   { 0xecb7191a, 0x597b, 0x41f5, { 0x98, 0x43, 0x03, 0xa4, 0xcf, 0x27, 0x5d, 0xde } };

enum __VSSPROPID2
    {	VSSPROPID_SccProviderChanged	= -9032,
	VSSPROPID_MainWindowSize	= -9033,
	VSSPROPID_MainWindowPos	= -9034,
	VSSPROPID_IsAcademic	= -9035,
	VSSPROPID_IsAppThemed	= -9036,
	VSSPROPID_VisualStudioDir	= -9037,
	VSSPROPID_VsTemplateUserZipProjectFolder	= -9038,
	VSSPROPID_VsTemplateUserZipItemFolder	= -9039,
	VSSPROPID_InstallRootDir	= -9041,
	VSSPROPID_SolutionExplorerSortingEnabled	= -9042,
	VSSPROPID_BuildOutOfDateProjects	= -9043,
	VSSPROPID_RunAfterBuildErrors	= -9044,
	VSSPROPID_MainWindowVisibility	= -9045,
	VSSPROPID_SKUEdition	= -9046,
	VSSPROPID_SubSKUEdition	= -9047,
	VSSPROPID_WaitingForSecondKeyChord	= -9048,
	VSSPROPID_SqmRegistryRoot	= -9049,
	VSSPROPID_AutohideToolFrame	= -9050,
	VSFPROPID_ToolWindowUsesDocSelection	= -9051,
	VSSPROPID_FIRST2	= -9051
    } ;
typedef LONG VSSPROPID2;


enum __BUILDOUTOFDATEPROJECTS
    {	BUILDOUTOFDATEPROJECTS_YES	= 0,
	BUILDOUTOFDATEPROJECTS_NO	= 1,
	BUILDOUTOFDATEPROJECTS_PROMPT	= 2,
	BUILDOUTOFDATEPROJECTS_MAX	= 2
    } ;

enum __RUNAFTERBUILDERRORS
    {	RUNAFTERBUILDERRORS_YES	= 0,
	RUNAFTERBUILDERRORS_NO	= 1,
	RUNAFTERBUILDERRORS_PROMPT	= 2,
	RUNAFTERBUILDERRORS_MAX	= 2
    } ;

enum __VSDBGLAUNCHFLAGS2
    {	DBGLAUNCH_MergeEnv	= 0x80,
	DBGLAUNCH_DesignTimeExprEval	= 0x100,
	DBGLAUNCH_StopAtEntryPoint	= 0x200,
	DBGLAUNCH_CannotDebugAlone	= 0x400
    } ;
typedef DWORD VSDBGLAUNCHFLAGS2;


enum _DEBUG_LAUNCH_OPERATION2
    {	DLO_AttachToHostingProcess	= 4,
	DLO_StartDebuggingHostingProcess	= 5
    } ;
typedef DWORD DEBUG_LAUNCH_OPERATION2;


enum __PSFFILEID2
    {	PSFFILEID_WebSettings	= -1002,
	PSFFILEID_AppManifest	= -1003,
	PSFFILEID_AppDesigner	= -1004,
	PSFFILEID_AppSettings	= -1005,
	PSFFILEID_AssemblyResource	= -1006,
	PSFFILEID_AssemblyInfo	= -1007,
	PSFFILEID_FIRST2	= -1007
    } ;
typedef LONG PSFFILEID2;


enum __PSFFLAGS2
    {	PSFF_CheckoutIfExists	= 0x4
    } ;
typedef DWORD PSFFLAGS2;


enum __VSEDT_STYLE
    {	VSEDT_Reserved1	= 0x1
    } ;
typedef DWORD VSEDT_STYLE;


enum __STOP_DEBUGGING_PROCESS_REASON
    {	SDPR_DETACH	= 0,
	SDPR_TERMINATE	= 1
    } ;
typedef DWORD STOP_DEBUGGING_PROCESS_REASON;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0011_v0_0_s_ifspec;

#ifndef __IVsEnhancedDataTip_INTERFACE_DEFINED__
#define __IVsEnhancedDataTip_INTERFACE_DEFINED__

/* interface IVsEnhancedDataTip */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnhancedDataTip;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B8092238-A091-42f1-A945-080B381FBCFC")
    IVsEnhancedDataTip : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Show( 
            /* [in] */ __RPC__in HWND hwndOwner,
            /* [in] */ __RPC__in POINT *pptTopLeft,
            /* [in] */ __RPC__in RECT *pHotRect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExpression( 
            /* [in] */ __RPC__in BSTR bstrExpression) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseWindowHandle( 
            /* [out] */ __RPC__deref_out_opt HWND *phwnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsErrorTip( 
            /* [out] */ __RPC__out BOOL *pbIsError) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsOpen( 
            /* [out] */ __RPC__out BOOL *pbIsOpen) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnhancedDataTipVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnhancedDataTip * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnhancedDataTip * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnhancedDataTip * This);
        
        HRESULT ( STDMETHODCALLTYPE *Show )( 
            IVsEnhancedDataTip * This,
            /* [in] */ __RPC__in HWND hwndOwner,
            /* [in] */ __RPC__in POINT *pptTopLeft,
            /* [in] */ __RPC__in RECT *pHotRect);
        
        HRESULT ( STDMETHODCALLTYPE *SetExpression )( 
            IVsEnhancedDataTip * This,
            /* [in] */ __RPC__in BSTR bstrExpression);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseWindowHandle )( 
            IVsEnhancedDataTip * This,
            /* [out] */ __RPC__deref_out_opt HWND *phwnd);
        
        HRESULT ( STDMETHODCALLTYPE *IsErrorTip )( 
            IVsEnhancedDataTip * This,
            /* [out] */ __RPC__out BOOL *pbIsError);
        
        HRESULT ( STDMETHODCALLTYPE *IsOpen )( 
            IVsEnhancedDataTip * This,
            /* [out] */ __RPC__out BOOL *pbIsOpen);
        
        END_INTERFACE
    } IVsEnhancedDataTipVtbl;

    interface IVsEnhancedDataTip
    {
        CONST_VTBL struct IVsEnhancedDataTipVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnhancedDataTip_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnhancedDataTip_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnhancedDataTip_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnhancedDataTip_Show(This,hwndOwner,pptTopLeft,pHotRect)	\
    ( (This)->lpVtbl -> Show(This,hwndOwner,pptTopLeft,pHotRect) ) 

#define IVsEnhancedDataTip_SetExpression(This,bstrExpression)	\
    ( (This)->lpVtbl -> SetExpression(This,bstrExpression) ) 

#define IVsEnhancedDataTip_GetBaseWindowHandle(This,phwnd)	\
    ( (This)->lpVtbl -> GetBaseWindowHandle(This,phwnd) ) 

#define IVsEnhancedDataTip_IsErrorTip(This,pbIsError)	\
    ( (This)->lpVtbl -> IsErrorTip(This,pbIsError) ) 

#define IVsEnhancedDataTip_IsOpen(This,pbIsOpen)	\
    ( (This)->lpVtbl -> IsOpen(This,pbIsOpen) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnhancedDataTip_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0012 */
/* [local] */ 

typedef struct _VsDebugTargetInfo2
    {
    DWORD cbSize;
    DEBUG_LAUNCH_OPERATION2 dlo;
    VSDBGLAUNCHFLAGS2 LaunchFlags;
    BSTR bstrRemoteMachine;
    BSTR bstrExe;
    BSTR bstrArg;
    BSTR bstrCurDir;
    BSTR bstrEnv;
    GUID guidLaunchDebugEngine;
    DWORD dwDebugEngineCount;
    GUID *pDebugEngines;
    GUID guidPortSupplier;
    BSTR bstrPortName;
    BSTR bstrOptions;
    DWORD_PTR hStdInput;
    DWORD_PTR hStdOutput;
    DWORD_PTR hStdError;
    BOOL fSendToOutputWindow;
    DWORD dwProcessId;
    IUnknown *pUnknown;
    GUID guidProcessLanguage;
    DWORD dwReserved;
    } 	VsDebugTargetInfo2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0012_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0012_v0_0_s_ifspec;

#ifndef __IVsDebugger2_INTERFACE_DEFINED__
#define __IVsDebugger2_INTERFACE_DEFINED__

/* interface IVsDebugger2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugger2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B33300FB-FEFE-4E00-A74A-17A5EED1B1ED")
    IVsDebugger2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchDebugTargets2( 
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(DebugTargetCount) VsDebugTargetInfo2 *pDebugTargets) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ConfirmStopDebugging( 
            /* [in] */ __RPC__in LPCOLESTR pszMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumDebugEngines( 
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEngineName( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsEngineCompatible( 
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ ULONG EngineCount,
            /* [size_is][in] */ __RPC__in_ecount_full(EngineCount) GUID *pEngineGUIDs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetConsoleHandlesForProcess( 
            /* [in] */ DWORD dwPid,
            /* [out] */ __RPC__out ULONG64 *pdwStdInput,
            /* [out] */ __RPC__out ULONG64 *pdwStdOutput,
            /* [out] */ __RPC__out ULONG64 *pdwStdError) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowSource( 
            /* [in] */ __RPC__in_opt IUnknown *pUnkDebugDocContext,
            /* [in] */ BOOL fMakeActive,
            /* [in] */ BOOL fAlwaysMoveCaret,
            /* [in] */ BOOL fPromptToFindSource,
            /* [in] */ BOOL fIgnoreIfNotFound,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppTextView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateDataTip( 
            /* [in] */ __RPC__in BSTR bstrExpression,
            /* [in] */ VSEDT_STYLE dwStyle,
            /* [out] */ __RPC__deref_out_opt IVsEnhancedDataTip **ppDataTip) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSymbolPath( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolCachePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOutputHandleForProcess( 
            /* [in] */ DWORD dwPid,
            /* [out] */ __RPC__out ULONG64 *pOutputHandle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InsertBreakpointByName( 
            /* [in] */ __RPC__in REFGUID guidLanguage,
            /* [in] */ __RPC__in LPCOLESTR pszCodeLocationText,
            /* [in] */ BOOL bUseIntellisense) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToggleUseQuickConsoleOption( 
            /* [in] */ BOOL fOnOff) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUseQuickConsoleOptionSetting( 
            /* [out] */ __RPC__out BOOL *pfValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInternalDebugMode( 
            /* [out] */ __RPC__out DBGMODE *pdbgmode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDebugger2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDebugger2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDebugger2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchDebugTargets2 )( 
            IVsDebugger2 * This,
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(DebugTargetCount) VsDebugTargetInfo2 *pDebugTargets);
        
        HRESULT ( STDMETHODCALLTYPE *ConfirmStopDebugging )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMessage);
        
        HRESULT ( STDMETHODCALLTYPE *EnumDebugEngines )( 
            IVsDebugger2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumGUID **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetEngineName )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *IsEngineCompatible )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in REFGUID guidEngine,
            /* [in] */ ULONG EngineCount,
            /* [size_is][in] */ __RPC__in_ecount_full(EngineCount) GUID *pEngineGUIDs);
        
        HRESULT ( STDMETHODCALLTYPE *GetConsoleHandlesForProcess )( 
            IVsDebugger2 * This,
            /* [in] */ DWORD dwPid,
            /* [out] */ __RPC__out ULONG64 *pdwStdInput,
            /* [out] */ __RPC__out ULONG64 *pdwStdOutput,
            /* [out] */ __RPC__out ULONG64 *pdwStdError);
        
        HRESULT ( STDMETHODCALLTYPE *ShowSource )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnkDebugDocContext,
            /* [in] */ BOOL fMakeActive,
            /* [in] */ BOOL fAlwaysMoveCaret,
            /* [in] */ BOOL fPromptToFindSource,
            /* [in] */ BOOL fIgnoreIfNotFound,
            /* [out] */ __RPC__deref_out_opt IVsTextView **ppTextView);
        
        HRESULT ( STDMETHODCALLTYPE *CreateDataTip )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in BSTR bstrExpression,
            /* [in] */ VSEDT_STYLE dwStyle,
            /* [out] */ __RPC__deref_out_opt IVsEnhancedDataTip **ppDataTip);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolPath )( 
            IVsDebugger2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolCachePath);
        
        HRESULT ( STDMETHODCALLTYPE *GetOutputHandleForProcess )( 
            IVsDebugger2 * This,
            /* [in] */ DWORD dwPid,
            /* [out] */ __RPC__out ULONG64 *pOutputHandle);
        
        HRESULT ( STDMETHODCALLTYPE *InsertBreakpointByName )( 
            IVsDebugger2 * This,
            /* [in] */ __RPC__in REFGUID guidLanguage,
            /* [in] */ __RPC__in LPCOLESTR pszCodeLocationText,
            /* [in] */ BOOL bUseIntellisense);
        
        HRESULT ( STDMETHODCALLTYPE *ToggleUseQuickConsoleOption )( 
            IVsDebugger2 * This,
            /* [in] */ BOOL fOnOff);
        
        HRESULT ( STDMETHODCALLTYPE *GetUseQuickConsoleOptionSetting )( 
            IVsDebugger2 * This,
            /* [out] */ __RPC__out BOOL *pfValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetInternalDebugMode )( 
            IVsDebugger2 * This,
            /* [out] */ __RPC__out DBGMODE *pdbgmode);
        
        END_INTERFACE
    } IVsDebugger2Vtbl;

    interface IVsDebugger2
    {
        CONST_VTBL struct IVsDebugger2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugger2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugger2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugger2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugger2_LaunchDebugTargets2(This,DebugTargetCount,pDebugTargets)	\
    ( (This)->lpVtbl -> LaunchDebugTargets2(This,DebugTargetCount,pDebugTargets) ) 

#define IVsDebugger2_ConfirmStopDebugging(This,pszMessage)	\
    ( (This)->lpVtbl -> ConfirmStopDebugging(This,pszMessage) ) 

#define IVsDebugger2_EnumDebugEngines(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumDebugEngines(This,ppEnum) ) 

#define IVsDebugger2_GetEngineName(This,guidEngine,pbstrName)	\
    ( (This)->lpVtbl -> GetEngineName(This,guidEngine,pbstrName) ) 

#define IVsDebugger2_IsEngineCompatible(This,guidEngine,EngineCount,pEngineGUIDs)	\
    ( (This)->lpVtbl -> IsEngineCompatible(This,guidEngine,EngineCount,pEngineGUIDs) ) 

#define IVsDebugger2_GetConsoleHandlesForProcess(This,dwPid,pdwStdInput,pdwStdOutput,pdwStdError)	\
    ( (This)->lpVtbl -> GetConsoleHandlesForProcess(This,dwPid,pdwStdInput,pdwStdOutput,pdwStdError) ) 

#define IVsDebugger2_ShowSource(This,pUnkDebugDocContext,fMakeActive,fAlwaysMoveCaret,fPromptToFindSource,fIgnoreIfNotFound,ppTextView)	\
    ( (This)->lpVtbl -> ShowSource(This,pUnkDebugDocContext,fMakeActive,fAlwaysMoveCaret,fPromptToFindSource,fIgnoreIfNotFound,ppTextView) ) 

#define IVsDebugger2_CreateDataTip(This,bstrExpression,dwStyle,ppDataTip)	\
    ( (This)->lpVtbl -> CreateDataTip(This,bstrExpression,dwStyle,ppDataTip) ) 

#define IVsDebugger2_GetSymbolPath(This,pbstrSymbolPath,pbstrSymbolCachePath)	\
    ( (This)->lpVtbl -> GetSymbolPath(This,pbstrSymbolPath,pbstrSymbolCachePath) ) 

#define IVsDebugger2_GetOutputHandleForProcess(This,dwPid,pOutputHandle)	\
    ( (This)->lpVtbl -> GetOutputHandleForProcess(This,dwPid,pOutputHandle) ) 

#define IVsDebugger2_InsertBreakpointByName(This,guidLanguage,pszCodeLocationText,bUseIntellisense)	\
    ( (This)->lpVtbl -> InsertBreakpointByName(This,guidLanguage,pszCodeLocationText,bUseIntellisense) ) 

#define IVsDebugger2_ToggleUseQuickConsoleOption(This,fOnOff)	\
    ( (This)->lpVtbl -> ToggleUseQuickConsoleOption(This,fOnOff) ) 

#define IVsDebugger2_GetUseQuickConsoleOptionSetting(This,pfValue)	\
    ( (This)->lpVtbl -> GetUseQuickConsoleOptionSetting(This,pfValue) ) 

#define IVsDebugger2_GetInternalDebugMode(This,pdbgmode)	\
    ( (This)->lpVtbl -> GetInternalDebugMode(This,pdbgmode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugger2_INTERFACE_DEFINED__ */


#ifndef __IVsDebugProcessNotify_INTERFACE_DEFINED__
#define __IVsDebugProcessNotify_INTERFACE_DEFINED__

/* interface IVsDebugProcessNotify */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugProcessNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("23320EFC-7C7A-4C3D-AD85-93A4E620FDD0")
    IVsDebugProcessNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeforeStopDebuggingProcess( 
            /* [in] */ STOP_DEBUGGING_PROCESS_REASON Reason) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDebugProcessNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDebugProcessNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDebugProcessNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDebugProcessNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeforeStopDebuggingProcess )( 
            IVsDebugProcessNotify * This,
            /* [in] */ STOP_DEBUGGING_PROCESS_REASON Reason);
        
        END_INTERFACE
    } IVsDebugProcessNotifyVtbl;

    interface IVsDebugProcessNotify
    {
        CONST_VTBL struct IVsDebugProcessNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugProcessNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugProcessNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugProcessNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugProcessNotify_BeforeStopDebuggingProcess(This,Reason)	\
    ( (This)->lpVtbl -> BeforeStopDebuggingProcess(This,Reason) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugProcessNotify_INTERFACE_DEFINED__ */


#ifndef __IVsQueryDebuggableProjectCfg_INTERFACE_DEFINED__
#define __IVsQueryDebuggableProjectCfg_INTERFACE_DEFINED__

/* interface IVsQueryDebuggableProjectCfg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsQueryDebuggableProjectCfg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CE25FCEE-9E4D-4ec8-856E-38E2BDB2E13B")
    IVsQueryDebuggableProjectCfg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryDebugTargets( 
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch,
            /* [in] */ ULONG cTargets,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cTargets) VsDebugTargetInfo2 rgDebugTargetInfo[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsQueryDebuggableProjectCfgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsQueryDebuggableProjectCfg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsQueryDebuggableProjectCfg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsQueryDebuggableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDebugTargets )( 
            IVsQueryDebuggableProjectCfg * This,
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch,
            /* [in] */ ULONG cTargets,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cTargets) VsDebugTargetInfo2 rgDebugTargetInfo[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsQueryDebuggableProjectCfgVtbl;

    interface IVsQueryDebuggableProjectCfg
    {
        CONST_VTBL struct IVsQueryDebuggableProjectCfgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryDebuggableProjectCfg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryDebuggableProjectCfg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryDebuggableProjectCfg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryDebuggableProjectCfg_QueryDebugTargets(This,grfLaunch,cTargets,rgDebugTargetInfo,pcActual)	\
    ( (This)->lpVtbl -> QueryDebugTargets(This,grfLaunch,cTargets,rgDebugTargetInfo,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryDebuggableProjectCfg_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0015 */
/* [local] */ 


enum __VSOVERLAYICON2
    {	OVERLAYICON_EXCLUDED	= 5,
	OVERLAYICON_NOTONDISK	= 6,
	OVERLAYICON_MAXINDEX2	= 6
    } ;
typedef DWORD VSOVERLAYICON2;


enum __VSMEPROPID2
    {	VSMEPROPID_ICON	= -1012,
	VSMEPROPID_LAST2	= -1012
    } ;
typedef LONG VSMEPROPID2;


enum __COMMANDWINDOWMODE2
    {	CWM_DEFAULT	= -1
    } ;
typedef DWORD COMMANDWINDOWMODE2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0015_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0015_v0_0_s_ifspec;

#ifndef __IVsCommandWindow2_INTERFACE_DEFINED__
#define __IVsCommandWindow2_INTERFACE_DEFINED__

/* interface IVsCommandWindow2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCommandWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E22363B8-E7D3-49b5-B094-7395BB35CE13")
    IVsCommandWindow2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetMode( 
            /* [in] */ COMMANDWINDOWMODE2 mode) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCommandWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCommandWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCommandWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCommandWindow2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetMode )( 
            IVsCommandWindow2 * This,
            /* [in] */ COMMANDWINDOWMODE2 mode);
        
        END_INTERFACE
    } IVsCommandWindow2Vtbl;

    interface IVsCommandWindow2
    {
        CONST_VTBL struct IVsCommandWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommandWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommandWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommandWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommandWindow2_SetMode(This,mode)	\
    ( (This)->lpVtbl -> SetMode(This,mode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommandWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsDeferredDocView_INTERFACE_DEFINED__
#define __IVsDeferredDocView_INTERFACE_DEFINED__

/* interface IVsDeferredDocView */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDeferredDocView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F22A0AD5-8F51-4f66-A644-EA64770CF8B7")
    IVsDeferredDocView : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_DocView( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkDocView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_CmdUIGuid( 
            /* [out] */ __RPC__out GUID *pGuidCmdId) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDeferredDocViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDeferredDocView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDeferredDocView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDeferredDocView * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_DocView )( 
            IVsDeferredDocView * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkDocView);
        
        HRESULT ( STDMETHODCALLTYPE *get_CmdUIGuid )( 
            IVsDeferredDocView * This,
            /* [out] */ __RPC__out GUID *pGuidCmdId);
        
        END_INTERFACE
    } IVsDeferredDocViewVtbl;

    interface IVsDeferredDocView
    {
        CONST_VTBL struct IVsDeferredDocViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDeferredDocView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDeferredDocView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDeferredDocView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDeferredDocView_get_DocView(This,ppUnkDocView)	\
    ( (This)->lpVtbl -> get_DocView(This,ppUnkDocView) ) 

#define IVsDeferredDocView_get_CmdUIGuid(This,pGuidCmdId)	\
    ( (This)->lpVtbl -> get_CmdUIGuid(This,pGuidCmdId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDeferredDocView_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0017 */
/* [local] */ 


enum __VSBLDCFGPROPID
    {	VSBLDCFGPROPID_LAST	= -16000,
	VSBLDCFGPROPID_SupportsMTBuild	= -16000,
	VSBLDCFGPROPID_FIRST	= -16000
    } ;
typedef LONG VSBLDCFGPROPID;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0017_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0017_v0_0_s_ifspec;

#ifndef __IVsBuildableProjectCfg2_INTERFACE_DEFINED__
#define __IVsBuildableProjectCfg2_INTERFACE_DEFINED__

/* interface IVsBuildableProjectCfg2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildableProjectCfg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("09857e8e-74cc-43a7-993d-3ec774dca298")
    IVsBuildableProjectCfg2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBuildCfgProperty( 
            /* [in] */ VSBLDCFGPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartBuildEx( 
            /* [in] */ DWORD dwBuildId,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBuildableProjectCfg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBuildableProjectCfg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBuildableProjectCfg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBuildableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuildCfgProperty )( 
            IVsBuildableProjectCfg2 * This,
            /* [in] */ VSBLDCFGPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *StartBuildEx )( 
            IVsBuildableProjectCfg2 * This,
            /* [in] */ DWORD dwBuildId,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions);
        
        END_INTERFACE
    } IVsBuildableProjectCfg2Vtbl;

    interface IVsBuildableProjectCfg2
    {
        CONST_VTBL struct IVsBuildableProjectCfg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildableProjectCfg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildableProjectCfg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildableProjectCfg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildableProjectCfg2_GetBuildCfgProperty(This,propid,pvar)	\
    ( (This)->lpVtbl -> GetBuildCfgProperty(This,propid,pvar) ) 

#define IVsBuildableProjectCfg2_StartBuildEx(This,dwBuildId,pIVsOutputWindowPane,dwOptions)	\
    ( (This)->lpVtbl -> StartBuildEx(This,dwBuildId,pIVsOutputWindowPane,dwOptions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildableProjectCfg2_INTERFACE_DEFINED__ */


#ifndef __IVsPublishableProjectStatusCallback_INTERFACE_DEFINED__
#define __IVsPublishableProjectStatusCallback_INTERFACE_DEFINED__

/* interface IVsPublishableProjectStatusCallback */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPublishableProjectStatusCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("279398E7-6FC1-40a2-9FB3-C321DB469E9B")
    IVsPublishableProjectStatusCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PublishBegin( 
            /* [out][in] */ __RPC__inout BOOL *pfContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PublishEnd( 
            /* [in] */ BOOL fSuccess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Tick( 
            /* [out][in] */ __RPC__inout BOOL *pfContinue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPublishableProjectStatusCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPublishableProjectStatusCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPublishableProjectStatusCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPublishableProjectStatusCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *PublishBegin )( 
            IVsPublishableProjectStatusCallback * This,
            /* [out][in] */ __RPC__inout BOOL *pfContinue);
        
        HRESULT ( STDMETHODCALLTYPE *PublishEnd )( 
            IVsPublishableProjectStatusCallback * This,
            /* [in] */ BOOL fSuccess);
        
        HRESULT ( STDMETHODCALLTYPE *Tick )( 
            IVsPublishableProjectStatusCallback * This,
            /* [out][in] */ __RPC__inout BOOL *pfContinue);
        
        END_INTERFACE
    } IVsPublishableProjectStatusCallbackVtbl;

    interface IVsPublishableProjectStatusCallback
    {
        CONST_VTBL struct IVsPublishableProjectStatusCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPublishableProjectStatusCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPublishableProjectStatusCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPublishableProjectStatusCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPublishableProjectStatusCallback_PublishBegin(This,pfContinue)	\
    ( (This)->lpVtbl -> PublishBegin(This,pfContinue) ) 

#define IVsPublishableProjectStatusCallback_PublishEnd(This,fSuccess)	\
    ( (This)->lpVtbl -> PublishEnd(This,fSuccess) ) 

#define IVsPublishableProjectStatusCallback_Tick(This,pfContinue)	\
    ( (This)->lpVtbl -> Tick(This,pfContinue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPublishableProjectStatusCallback_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0019 */
/* [local] */ 


enum __VSPUBLISHOPTS
    {	PUBOPT_PUBLISHCONTEXT	= 0x1
    } ;
typedef DWORD VSPUBLISHOPTS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0019_v0_0_s_ifspec;

#ifndef __IVsPublishableProjectCfg_INTERFACE_DEFINED__
#define __IVsPublishableProjectCfg_INTERFACE_DEFINED__

/* interface IVsPublishableProjectCfg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPublishableProjectCfg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("816B2FBE-5C62-439e-8F67-8F0D5D82BC67")
    IVsPublishableProjectCfg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdvisePublishStatusCallback( 
            /* [in] */ __RPC__in_opt IVsPublishableProjectStatusCallback *pIVsPublishStatusCallback,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadvisePublishStatusCallback( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartPublish( 
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryStatusPublish( 
            /* [out] */ __RPC__out BOOL *pfPublishDone) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopPublish( 
            /* [in] */ BOOL fSync) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowPublishPrompt( 
            /* [out] */ __RPC__out BOOL *pfContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryStartPublish( 
            /* [in] */ DWORD dwOptions,
            /* [optional][out] */ __RPC__out BOOL *pfSupported,
            /* [optional][out] */ __RPC__out BOOL *pfReady) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPublishProperty( 
            /* [in] */ VSPUBLISHOPTS propid,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPublishableProjectCfgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPublishableProjectCfg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPublishableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdvisePublishStatusCallback )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ __RPC__in_opt IVsPublishableProjectStatusCallback *pIVsPublishStatusCallback,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadvisePublishStatusCallback )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *StartPublish )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *QueryStatusPublish )( 
            IVsPublishableProjectCfg * This,
            /* [out] */ __RPC__out BOOL *pfPublishDone);
        
        HRESULT ( STDMETHODCALLTYPE *StopPublish )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ BOOL fSync);
        
        HRESULT ( STDMETHODCALLTYPE *ShowPublishPrompt )( 
            IVsPublishableProjectCfg * This,
            /* [out] */ __RPC__out BOOL *pfContinue);
        
        HRESULT ( STDMETHODCALLTYPE *QueryStartPublish )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ DWORD dwOptions,
            /* [optional][out] */ __RPC__out BOOL *pfSupported,
            /* [optional][out] */ __RPC__out BOOL *pfReady);
        
        HRESULT ( STDMETHODCALLTYPE *GetPublishProperty )( 
            IVsPublishableProjectCfg * This,
            /* [in] */ VSPUBLISHOPTS propid,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        END_INTERFACE
    } IVsPublishableProjectCfgVtbl;

    interface IVsPublishableProjectCfg
    {
        CONST_VTBL struct IVsPublishableProjectCfgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPublishableProjectCfg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPublishableProjectCfg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPublishableProjectCfg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPublishableProjectCfg_AdvisePublishStatusCallback(This,pIVsPublishStatusCallback,pdwCookie)	\
    ( (This)->lpVtbl -> AdvisePublishStatusCallback(This,pIVsPublishStatusCallback,pdwCookie) ) 

#define IVsPublishableProjectCfg_UnadvisePublishStatusCallback(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadvisePublishStatusCallback(This,dwCookie) ) 

#define IVsPublishableProjectCfg_StartPublish(This,pIVsOutputWindowPane,dwOptions)	\
    ( (This)->lpVtbl -> StartPublish(This,pIVsOutputWindowPane,dwOptions) ) 

#define IVsPublishableProjectCfg_QueryStatusPublish(This,pfPublishDone)	\
    ( (This)->lpVtbl -> QueryStatusPublish(This,pfPublishDone) ) 

#define IVsPublishableProjectCfg_StopPublish(This,fSync)	\
    ( (This)->lpVtbl -> StopPublish(This,fSync) ) 

#define IVsPublishableProjectCfg_ShowPublishPrompt(This,pfContinue)	\
    ( (This)->lpVtbl -> ShowPublishPrompt(This,pfContinue) ) 

#define IVsPublishableProjectCfg_QueryStartPublish(This,dwOptions,pfSupported,pfReady)	\
    ( (This)->lpVtbl -> QueryStartPublish(This,dwOptions,pfSupported,pfReady) ) 

#define IVsPublishableProjectCfg_GetPublishProperty(This,propid,pvar)	\
    ( (This)->lpVtbl -> GetPublishProperty(This,propid,pvar) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPublishableProjectCfg_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0020 */
/* [local] */ 

typedef 
enum __VSSOLNBUILDUPDATEFLAGS2
    {	SBF_OPERATION_PUBLISHUI	= 0x800000,
	SBF_OPERATION_PUBLISH	= 0x1000000
    } 	VSSOLNBUILDUPDATEFLAGS2;

#define PCL_AUTO_SWITCHES       -2
#define PCL_AUTO_SWITCHVALUES   -3


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0020_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0020_v0_0_s_ifspec;

#ifndef __IVsParseCommandLine2_INTERFACE_DEFINED__
#define __IVsParseCommandLine2_INTERFACE_DEFINED__

/* interface IVsParseCommandLine2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsParseCommandLine2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7837DF15-0604-4ea1-8515-CD5A30972482")
    IVsParseCommandLine2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetACParamOrSwitch( 
            /* [out] */ __RPC__out int *piACIndex,
            /* [out] */ __RPC__out int *piACStart,
            /* [out] */ __RPC__out int *pcchACLength,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurSwitch,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrACParam) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsParseCommandLine2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsParseCommandLine2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsParseCommandLine2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsParseCommandLine2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetACParamOrSwitch )( 
            IVsParseCommandLine2 * This,
            /* [out] */ __RPC__out int *piACIndex,
            /* [out] */ __RPC__out int *piACStart,
            /* [out] */ __RPC__out int *pcchACLength,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurSwitch,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrACParam);
        
        END_INTERFACE
    } IVsParseCommandLine2Vtbl;

    interface IVsParseCommandLine2
    {
        CONST_VTBL struct IVsParseCommandLine2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsParseCommandLine2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsParseCommandLine2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsParseCommandLine2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsParseCommandLine2_GetACParamOrSwitch(This,piACIndex,piACStart,pcchACLength,pbstrCurSwitch,pbstrACParam)	\
    ( (This)->lpVtbl -> GetACParamOrSwitch(This,piACIndex,piACStart,pcchACLength,pbstrCurSwitch,pbstrACParam) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsParseCommandLine2_INTERFACE_DEFINED__ */


#ifndef __IVsCommandWindowsCollection_INTERFACE_DEFINED__
#define __IVsCommandWindowsCollection_INTERFACE_DEFINED__

/* interface IVsCommandWindowsCollection */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCommandWindowsCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("615FF029-FEFA-492c-8CD2-C3F66644C3F9")
    IVsCommandWindowsCollection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Create( 
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [in] */ DWORD dwToolWindowId,
            /* [in] */ BOOL fShow,
            /* [out] */ __RPC__out UINT *puCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenExistingOrCreateNewCommandWindow( 
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [in] */ BOOL fShow,
            /* [out] */ __RPC__out UINT *puCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCommandWindowFromCookie( 
            /* [in] */ UINT uCookie,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkCmdWindow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCommandWindowFromMode( 
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkCmdWindow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRunningCommandWindowCommand( 
            /* [in] */ UINT uCookie,
            /* [in] */ BOOL fCmdWin) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsOutputWaiting( 
            /* [in] */ UINT uCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( 
            /* [in] */ UINT uCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseAllCommandWindows( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCommandWindowsCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCommandWindowsCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCommandWindowsCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *Create )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [in] */ DWORD dwToolWindowId,
            /* [in] */ BOOL fShow,
            /* [out] */ __RPC__out UINT *puCookie);
        
        HRESULT ( STDMETHODCALLTYPE *OpenExistingOrCreateNewCommandWindow )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [in] */ BOOL fShow,
            /* [out] */ __RPC__out UINT *puCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetCommandWindowFromCookie )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ UINT uCookie,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkCmdWindow);
        
        HRESULT ( STDMETHODCALLTYPE *GetCommandWindowFromMode )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ COMMANDWINDOWMODE2 mode,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkCmdWindow);
        
        HRESULT ( STDMETHODCALLTYPE *SetRunningCommandWindowCommand )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ UINT uCookie,
            /* [in] */ BOOL fCmdWin);
        
        HRESULT ( STDMETHODCALLTYPE *IsOutputWaiting )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ UINT uCookie);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsCommandWindowsCollection * This,
            /* [in] */ UINT uCookie);
        
        HRESULT ( STDMETHODCALLTYPE *CloseAllCommandWindows )( 
            IVsCommandWindowsCollection * This);
        
        END_INTERFACE
    } IVsCommandWindowsCollectionVtbl;

    interface IVsCommandWindowsCollection
    {
        CONST_VTBL struct IVsCommandWindowsCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommandWindowsCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommandWindowsCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommandWindowsCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommandWindowsCollection_Create(This,mode,dwToolWindowId,fShow,puCookie)	\
    ( (This)->lpVtbl -> Create(This,mode,dwToolWindowId,fShow,puCookie) ) 

#define IVsCommandWindowsCollection_OpenExistingOrCreateNewCommandWindow(This,mode,fShow,puCookie)	\
    ( (This)->lpVtbl -> OpenExistingOrCreateNewCommandWindow(This,mode,fShow,puCookie) ) 

#define IVsCommandWindowsCollection_GetCommandWindowFromCookie(This,uCookie,ppunkCmdWindow)	\
    ( (This)->lpVtbl -> GetCommandWindowFromCookie(This,uCookie,ppunkCmdWindow) ) 

#define IVsCommandWindowsCollection_GetCommandWindowFromMode(This,mode,ppunkCmdWindow)	\
    ( (This)->lpVtbl -> GetCommandWindowFromMode(This,mode,ppunkCmdWindow) ) 

#define IVsCommandWindowsCollection_SetRunningCommandWindowCommand(This,uCookie,fCmdWin)	\
    ( (This)->lpVtbl -> SetRunningCommandWindowCommand(This,uCookie,fCmdWin) ) 

#define IVsCommandWindowsCollection_IsOutputWaiting(This,uCookie)	\
    ( (This)->lpVtbl -> IsOutputWaiting(This,uCookie) ) 

#define IVsCommandWindowsCollection_Close(This,uCookie)	\
    ( (This)->lpVtbl -> Close(This,uCookie) ) 

#define IVsCommandWindowsCollection_CloseAllCommandWindows(This)	\
    ( (This)->lpVtbl -> CloseAllCommandWindows(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommandWindowsCollection_INTERFACE_DEFINED__ */


#ifndef __SVsCommandWindowsCollection_INTERFACE_DEFINED__
#define __SVsCommandWindowsCollection_INTERFACE_DEFINED__

/* interface SVsCommandWindowsCollection */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsCommandWindowsCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E337F382-3CAB-44a5-AF0E-3DACA541C89A")
    SVsCommandWindowsCollection : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsCommandWindowsCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsCommandWindowsCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsCommandWindowsCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsCommandWindowsCollection * This);
        
        END_INTERFACE
    } SVsCommandWindowsCollectionVtbl;

    interface SVsCommandWindowsCollection
    {
        CONST_VTBL struct SVsCommandWindowsCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsCommandWindowsCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsCommandWindowsCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsCommandWindowsCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsCommandWindowsCollection_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0023 */
/* [local] */ 

#define SID_SVsCommandWindowsCollection IID_SVsCommandWindowsCollection
typedef void (__cdecl * PVsBackgroundTask_Proc)(DWORD_PTR pvParam, HANDLE hThreadTerminationEvent);
typedef PVsBackgroundTask_Proc PVsBackgroundTask_Function_Pointer;

enum __VSBACKGROUNDTASKPRIORITY
    {	VSBACKGROUNDTASKPRIORITY_STANDARD	= 0,
	VSBACKGROUNDTASKPRIORITY_IMMEDIATE	= 1,
	VSBACKGROUNDTASKPRIORITY_OTHER	= 2
    } ;
typedef DWORD VSBACKGROUNDTASKPRIORITY;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0023_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0023_v0_0_s_ifspec;

#ifndef __IVsThreadPool_INTERFACE_DEFINED__
#define __IVsThreadPool_INTERFACE_DEFINED__

/* interface IVsThreadPool */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadPool;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("615FF0FB-A19B-4bc8-B9AF-372EA191BA46")
    IVsThreadPool : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ScheduleTask( 
            /* [in] */ DWORD_PTR pTaskProc,
            /* [in] */ DWORD_PTR pvParam,
            /* [in] */ VSBACKGROUNDTASKPRIORITY priority) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ScheduleWaitableTask( 
            /* [in] */ DWORD_PTR hWait,
            /* [in] */ DWORD_PTR pTaskProc,
            /* [in] */ DWORD_PTR pvParam) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnscheduleWaitableTask( 
            /* [in] */ DWORD_PTR hWait) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsThreadPoolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsThreadPool * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsThreadPool * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsThreadPool * This);
        
        HRESULT ( STDMETHODCALLTYPE *ScheduleTask )( 
            IVsThreadPool * This,
            /* [in] */ DWORD_PTR pTaskProc,
            /* [in] */ DWORD_PTR pvParam,
            /* [in] */ VSBACKGROUNDTASKPRIORITY priority);
        
        HRESULT ( STDMETHODCALLTYPE *ScheduleWaitableTask )( 
            IVsThreadPool * This,
            /* [in] */ DWORD_PTR hWait,
            /* [in] */ DWORD_PTR pTaskProc,
            /* [in] */ DWORD_PTR pvParam);
        
        HRESULT ( STDMETHODCALLTYPE *UnscheduleWaitableTask )( 
            IVsThreadPool * This,
            /* [in] */ DWORD_PTR hWait);
        
        END_INTERFACE
    } IVsThreadPoolVtbl;

    interface IVsThreadPool
    {
        CONST_VTBL struct IVsThreadPoolVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadPool_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadPool_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadPool_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadPool_ScheduleTask(This,pTaskProc,pvParam,priority)	\
    ( (This)->lpVtbl -> ScheduleTask(This,pTaskProc,pvParam,priority) ) 

#define IVsThreadPool_ScheduleWaitableTask(This,hWait,pTaskProc,pvParam)	\
    ( (This)->lpVtbl -> ScheduleWaitableTask(This,hWait,pTaskProc,pvParam) ) 

#define IVsThreadPool_UnscheduleWaitableTask(This,hWait)	\
    ( (This)->lpVtbl -> UnscheduleWaitableTask(This,hWait) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadPool_INTERFACE_DEFINED__ */


#ifndef __SVsThreadPool_INTERFACE_DEFINED__
#define __SVsThreadPool_INTERFACE_DEFINED__

/* interface SVsThreadPool */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsThreadPool;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7482E83D-842A-447c-8DDB-687F7052F7C7")
    SVsThreadPool : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsThreadPoolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsThreadPool * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsThreadPool * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsThreadPool * This);
        
        END_INTERFACE
    } SVsThreadPoolVtbl;

    interface SVsThreadPool
    {
        CONST_VTBL struct SVsThreadPoolVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsThreadPool_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsThreadPool_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsThreadPool_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsThreadPool_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0025 */
/* [local] */ 

#define SID_SVsThreadPool IID_SVsThreadPool
typedef struct _VSNSEBROWSEINFOW
    {
    DWORD lStructSize;
    LPCOLESTR pszNamespaceGUID;
    LPCOLESTR pszTrayDisplayName;
    LPCOLESTR pszProtocolPrefix;
    BOOL fOnlyShowNSEInTray;
    } 	VSNSEBROWSEINFOW;

typedef struct _VSSAVETREEITEM
    {
    VSRDTSAVEOPTIONS grfSave;
    VSCOOKIE docCookie;
    IVsHierarchy *pHier;
    VSITEMID itemid;
    } 	VSSAVETREEITEM;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0025_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0025_v0_0_s_ifspec;

#ifndef __IVsShell2_INTERFACE_DEFINED__
#define __IVsShell2_INTERFACE_DEFINED__

/* interface IVsShell2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsShell2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F3519E2D-D5D2-4455-B9F4-5F61F993BD66")
    IVsShell2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadPackageStringWithLCID( 
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ ULONG resid,
            /* [in] */ LCID lcid,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsShell2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsShell2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsShell2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsShell2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadPackageStringWithLCID )( 
            IVsShell2 * This,
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ ULONG resid,
            /* [in] */ LCID lcid,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOut);
        
        END_INTERFACE
    } IVsShell2Vtbl;

    interface IVsShell2
    {
        CONST_VTBL struct IVsShell2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsShell2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsShell2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsShell2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsShell2_LoadPackageStringWithLCID(This,guidPackage,resid,lcid,pbstrOut)	\
    ( (This)->lpVtbl -> LoadPackageStringWithLCID(This,guidPackage,resid,lcid,pbstrOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsShell2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0026 */
/* [local] */ 

typedef 
enum __tagVSSYSCOLOREX
    {	VSCOLOR_ACCENT_BORDER	= -5,
	VSCOLOR_ACCENT_DARK	= -6,
	VSCOLOR_ACCENT_LIGHT	= -7,
	VSCOLOR_ACCENT_MEDIUM	= -8,
	VSCOLOR_ACCENT_PALE	= -9,
	VSCOLOR_COMMANDBAR_BORDER	= -10,
	VSCOLOR_COMMANDBAR_DRAGHANDLE	= -11,
	VSCOLOR_COMMANDBAR_DRAGHANDLE_SHADOW	= -12,
	VSCOLOR_COMMANDBAR_GRADIENT_BEGIN	= -13,
	VSCOLOR_COMMANDBAR_GRADIENT_END	= -14,
	VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE	= -15,
	VSCOLOR_COMMANDBAR_HOVER	= -16,
	VSCOLOR_COMMANDBAR_HOVEROVERSELECTED	= -17,
	VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON	= -18,
	VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON_BORDER	= -19,
	VSCOLOR_COMMANDBAR_SELECTED	= -20,
	VSCOLOR_COMMANDBAR_SHADOW	= -21,
	VSCOLOR_COMMANDBAR_TEXT_ACTIVE	= -22,
	VSCOLOR_COMMANDBAR_TEXT_HOVER	= -23,
	VSCOLOR_COMMANDBAR_TEXT_INACTIVE	= -24,
	VSCOLOR_COMMANDBAR_TEXT_SELECTED	= -25,
	VSCOLOR_CONTROL_EDIT_HINTTEXT	= -26,
	VSCOLOR_CONTROL_EDIT_REQUIRED_BACKGROUND	= -27,
	VSCOLOR_CONTROL_EDIT_REQUIRED_HINTTEXT	= -28,
	VSCOLOR_CONTROL_LINK_TEXT	= -29,
	VSCOLOR_CONTROL_LINK_TEXT_HOVER	= -30,
	VSCOLOR_CONTROL_LINK_TEXT_PRESSED	= -31,
	VSCOLOR_CONTROL_OUTLINE	= -32,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_BACKGROUND	= -33,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_BORDER	= -34,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_HIGHLIGHT	= -35,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_HIGHLIGHTTEXT	= -36,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_SEPARATOR	= -37,
	VSCOLOR_DEBUGGER_DATATIP_ACTIVE_TEXT	= -38,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_BACKGROUND	= -39,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_BORDER	= -40,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_HIGHLIGHT	= -41,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_HIGHLIGHTTEXT	= -42,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_SEPARATOR	= -43,
	VSCOLOR_DEBUGGER_DATATIP_INACTIVE_TEXT	= -44,
	VSCOLOR_DESIGNER_BACKGROUND	= -45,
	VSCOLOR_DESIGNER_SELECTIONDOTS	= -46,
	VSCOLOR_DESIGNER_TRAY	= -47,
	VSCOLOR_DESIGNER_WATERMARK	= -48,
	VSCOLOR_EDITOR_EXPANSION_BORDER	= -49,
	VSCOLOR_EDITOR_EXPANSION_FILL	= -50,
	VSCOLOR_EDITOR_EXPANSION_LINK	= -51,
	VSCOLOR_EDITOR_EXPANSION_TEXT	= -52,
	VSCOLOR_ENVIRONMENT_BACKGROUND	= -53,
	VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTBEGIN	= -54,
	VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTEND	= -55,
	VSCOLOR_FILETAB_BORDER	= -56,
	VSCOLOR_FILETAB_CHANNELBACKGROUND	= -57,
	VSCOLOR_FILETAB_GRADIENTDARK	= -58,
	VSCOLOR_FILETAB_GRADIENTLIGHT	= -59,
	VSCOLOR_FILETAB_SELECTEDBACKGROUND	= -60,
	VSCOLOR_FILETAB_SELECTEDBORDER	= -61,
	VSCOLOR_FILETAB_SELECTEDTEXT	= -62,
	VSCOLOR_FILETAB_TEXT	= -63,
	VSCOLOR_FORMSMARTTAG_ACTIONTAG_BORDER	= -64,
	VSCOLOR_FORMSMARTTAG_ACTIONTAG_FILL	= -65,
	VSCOLOR_FORMSMARTTAG_OBJECTTAG_BORDER	= -66,
	VSCOLOR_FORMSMARTTAG_OBJECTTAG_FILL	= -67,
	VSCOLOR_GRID_HEADING_BACKGROUND	= -68,
	VSCOLOR_GRID_HEADING_TEXT	= -69,
	VSCOLOR_GRID_LINE	= -70,
	VSCOLOR_HELP_HOWDOI_PANE_BACKGROUND	= -71,
	VSCOLOR_HELP_HOWDOI_PANE_LINK	= -72,
	VSCOLOR_HELP_HOWDOI_PANE_TEXT	= -73,
	VSCOLOR_HELP_HOWDOI_TASK_BACKGROUND	= -74,
	VSCOLOR_HELP_HOWDOI_TASK_LINK	= -75,
	VSCOLOR_HELP_HOWDOI_TASK_TEXT	= -76,
	VSCOLOR_HELP_SEARCH_FRAME_BACKGROUND	= -77,
	VSCOLOR_HELP_SEARCH_FRAME_TEXT	= -78,
	VSCOLOR_HELP_SEARCH_BORDER	= -79,
	VSCOLOR_HELP_SEARCH_FITLER_TEXT	= -80,
	VSCOLOR_HELP_SEARCH_FITLER_BACKGROUND	= -81,
	VSCOLOR_HELP_SEARCH_FITLER_BORDER	= -82,
	VSCOLOR_HELP_SEARCH_PROVIDER_UNSELECTED_BACKGROUND	= -83,
	VSCOLOR_HELP_SEARCH_PROVIDER_UNSELECTED_TEXT	= -84,
	VSCOLOR_HELP_SEARCH_PROVIDER_SELECTED_BACKGROUND	= -85,
	VSCOLOR_HELP_SEARCH_PROVIDER_SELECTED_TEXT	= -86,
	VSCOLOR_HELP_SEARCH_PROVIDER_ICON	= -87,
	VSCOLOR_HELP_SEARCH_RESULT_LINK_SELECTED	= -88,
	VSCOLOR_HELP_SEARCH_RESULT_LINK_UNSELECTED	= -89,
	VSCOLOR_HELP_SEARCH_RESULT_SELECTED_BACKGROUND	= -90,
	VSCOLOR_HELP_SEARCH_RESULT_SELECTED_TEXT	= -91,
	VSCOLOR_HELP_SEARCH_BACKGROUND	= -92,
	VSCOLOR_HELP_SEARCH_TEXT	= -93,
	VSCOLOR_HELP_SEARCH_PANEL_RULES	= -94,
	VSCOLOR_MDICLIENT_BORDER	= -95,
	VSCOLOR_PANEL_BORDER	= -96,
	VSCOLOR_PANEL_GRADIENTDARK	= -97,
	VSCOLOR_PANEL_GRADIENTLIGHT	= -98,
	VSCOLOR_PANEL_HOVEROVERCLOSE_BORDER	= -99,
	VSCOLOR_PANEL_HOVEROVERCLOSE_FILL	= -100,
	VSCOLOR_PANEL_HYPERLINK	= -101,
	VSCOLOR_PANEL_HYPERLINK_HOVER	= -102,
	VSCOLOR_PANEL_HYPERLINK_PRESSED	= -103,
	VSCOLOR_PANEL_SEPARATOR	= -104,
	VSCOLOR_PANEL_SUBGROUPSEPARATOR	= -105,
	VSCOLOR_PANEL_TEXT	= -106,
	VSCOLOR_PANEL_TITLEBAR	= -107,
	VSCOLOR_PANEL_TITLEBAR_TEXT	= -108,
	VSCOLOR_PANEL_TITLEBAR_UNSELECTED	= -109,
	VSCOLOR_PROJECTDESIGNER_BACKGROUND_GRADIENTBEGIN	= -110,
	VSCOLOR_PROJECTDESIGNER_BACKGROUND_GRADIENTEND	= -111,
	VSCOLOR_PROJECTDESIGNER_BORDER_OUTSIDE	= -112,
	VSCOLOR_PROJECTDESIGNER_BORDER_INSIDE	= -113,
	VSCOLOR_PROJECTDESIGNER_CONTENTS_BACKGROUND	= -114,
	VSCOLOR_PROJECTDESIGNER_TAB_BACKGROUND_GRADIENTBEGIN	= -115,
	VSCOLOR_PROJECTDESIGNER_TAB_BACKGROUND_GRADIENTEND	= -116,
	VSCOLOR_PROJECTDESIGNER_TAB_SELECTED_INSIDEBORDER	= -117,
	VSCOLOR_PROJECTDESIGNER_TAB_SELECTED_BORDER	= -118,
	VSCOLOR_PROJECTDESIGNER_TAB_SELECTED_HIGHLIGHT1	= -119,
	VSCOLOR_PROJECTDESIGNER_TAB_SELECTED_HIGHLIGHT2	= -120,
	VSCOLOR_PROJECTDESIGNER_TAB_SELECTED_BACKGROUND	= -121,
	VSCOLOR_PROJECTDESIGNER_TAB_SEP_BOTTOM_GRADIENTBEGIN	= -122,
	VSCOLOR_PROJECTDESIGNER_TAB_SEP_BOTTOM_GRADIENTEND	= -123,
	VSCOLOR_PROJECTDESIGNER_TAB_SEP_TOP_GRADIENTBEGIN	= -124,
	VSCOLOR_PROJECTDESIGNER_TAB_SEP_TOP_GRADIENTEND	= -125,
	VSCOLOR_SCREENTIP_BORDER	= -126,
	VSCOLOR_SCREENTIP_BACKGROUND	= -127,
	VSCOLOR_SCREENTIP_TEXT	= -128,
	VSCOLOR_SIDEBAR_BACKGROUND	= -129,
	VSCOLOR_SIDEBAR_GRADIENTDARK	= -130,
	VSCOLOR_SIDEBAR_GRADIENTLIGHT	= -131,
	VSCOLOR_SIDEBAR_TEXT	= -132,
	VSCOLOR_SMARTTAG_BORDER	= -133,
	VSCOLOR_SMARTTAG_FILL	= -134,
	VSCOLOR_SMARTTAG_HOVER_BORDER	= -135,
	VSCOLOR_SMARTTAG_HOVER_FILL	= -136,
	VSCOLOR_SMARTTAG_HOVER_TEXT	= -137,
	VSCOLOR_SMARTTAG_TEXT	= -138,
	VSCOLOR_SNAPLINES	= -139,
	VSCOLOR_SNAPLINES_PADDING	= -140,
	VSCOLOR_SNAPLINES_TEXTBASELINE	= -141,
	VSCOLOR_SORT_BACKGROUND	= -142,
	VSCOLOR_SORT_TEXT	= -143,
	VSCOLOR_TASKLIST_GRIDLINES	= -144,
	VSCOLOR_TITLEBAR_ACTIVE	= -145,
	VSCOLOR_TITLEBAR_ACTIVE_GRADIENTBEGIN	= -146,
	VSCOLOR_TITLEBAR_ACTIVE_GRADIENTEND	= -147,
	VSCOLOR_TITLEBAR_ACTIVE_TEXT	= -148,
	VSCOLOR_TITLEBAR_INACTIVE	= -149,
	VSCOLOR_TITLEBAR_INACTIVE_GRADIENTBEGIN	= -150,
	VSCOLOR_TITLEBAR_INACTIVE_GRADIENTEND	= -151,
	VSCOLOR_TITLEBAR_INACTIVE_TEXT	= -152,
	VSCOLOR_TOOLBOX_BACKGROUND	= -153,
	VSCOLOR_TOOLBOX_DIVIDER	= -154,
	VSCOLOR_TOOLBOX_GRADIENTDARK	= -155,
	VSCOLOR_TOOLBOX_GRADIENTLIGHT	= -156,
	VSCOLOR_TOOLBOX_HEADINGACCENT	= -157,
	VSCOLOR_TOOLBOX_HEADINGBEGIN	= -158,
	VSCOLOR_TOOLBOX_HEADINGEND	= -159,
	VSCOLOR_TOOLBOX_ICON_HIGHLIGHT	= -160,
	VSCOLOR_TOOLBOX_ICON_SHADOW	= -161,
	VSCOLOR_TOOLWINDOW_BACKGROUND	= -162,
	VSCOLOR_TOOLWINDOW_BORDER	= -163,
	VSCOLOR_TOOLWINDOW_BUTTON_DOWN	= -164,
	VSCOLOR_TOOLWINDOW_BUTTON_DOWN_BORDER	= -165,
	VSCOLOR_TOOLWINDOW_BUTTON_HOVER_ACTIVE	= -166,
	VSCOLOR_TOOLWINDOW_BUTTON_HOVER_ACTIVE_BORDER	= -167,
	VSCOLOR_TOOLWINDOW_BUTTON_HOVER_INACTIVE	= -168,
	VSCOLOR_TOOLWINDOW_BUTTON_HOVER_INACTIVE_BORDER	= -169,
	VSCOLOR_TOOLWINDOW_TEXT	= -170,
	VSCOLOR_TOOLWINDOW_TAB_SELECTEDTAB	= -171,
	VSCOLOR_TOOLWINDOW_TAB_BORDER	= -172,
	VSCOLOR_TOOLWINDOW_TAB_GRADIENTBEGIN	= -173,
	VSCOLOR_TOOLWINDOW_TAB_GRADIENTEND	= -174,
	VSCOLOR_TOOLWINDOW_TAB_TEXT	= -175,
	VSCOLOR_TOOLWINDOW_TAB_SELECTEDTEXT	= -176,
	VSCOLOR_WIZARD_ORIENTATIONPANEL_BACKGROUND	= -177,
	VSCOLOR_WIZARD_ORIENTATIONPANEL_TEXT	= -178,
	VSCOLOR_LASTEX	= -178
    } 	__VSSYSCOLOREX;

typedef int VSSYSCOLOREX;

typedef 
enum __tagGRADIENTTYPE
    {	VSGRADIENT_FILETAB	= 1,
	VSGRADIENT_PANEL_BACKGROUND	= 2,
	VSGRADIENT_SHELLBACKGROUND	= 3,
	VSGRADIENT_TOOLBOX_HEADING	= 4,
	VSGRADIENT_TOOLTAB	= 5,
	VSGRADIENT_TOOLWIN_ACTIVE_TITLE_BAR	= 6,
	VSGRADIENT_TOOLWIN_INACTIVE_TITLE_BAR	= 7,
	VSGRADIENT_TOOLWIN_BACKGROUND	= 8
    } 	__GRADIENTTYPE;

typedef DWORD GRADIENTTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0026_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0026_v0_0_s_ifspec;

#ifndef __IVsGradient_INTERFACE_DEFINED__
#define __IVsGradient_INTERFACE_DEFINED__

/* interface IVsGradient */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsGradient;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fd3f680a-d5c1-437a-8a21-8084310bf037")
    IVsGradient : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DrawGradient( 
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *gradientRect,
            /* [in] */ __RPC__in RECT *sliceRect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGradientVector( 
            /* [in] */ int cVector,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cVector) COLORREF *rgVector) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsGradientVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsGradient * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsGradient * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsGradient * This);
        
        HRESULT ( STDMETHODCALLTYPE *DrawGradient )( 
            IVsGradient * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ __RPC__in HDC hdc,
            /* [in] */ __RPC__in RECT *gradientRect,
            /* [in] */ __RPC__in RECT *sliceRect);
        
        HRESULT ( STDMETHODCALLTYPE *GetGradientVector )( 
            IVsGradient * This,
            /* [in] */ int cVector,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cVector) COLORREF *rgVector);
        
        END_INTERFACE
    } IVsGradientVtbl;

    interface IVsGradient
    {
        CONST_VTBL struct IVsGradientVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGradient_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGradient_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGradient_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGradient_DrawGradient(This,hwnd,hdc,gradientRect,sliceRect)	\
    ( (This)->lpVtbl -> DrawGradient(This,hwnd,hdc,gradientRect,sliceRect) ) 

#define IVsGradient_GetGradientVector(This,cVector,rgVector)	\
    ( (This)->lpVtbl -> GetGradientVector(This,cVector,rgVector) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGradient_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0027 */
/* [local] */ 


enum __VSCURSORTYPE
    {	VSCURSOR_APPSTARTING	= 1,
	VSCURSOR_COLUMNSPLIT_EW	= 2,
	VSCURSOR_COLUMNSPLIT_NS	= 3,
	VSCURSOR_CONTROL_COPY	= 4,
	VSCURSOR_CONTROL_DELETE	= 5,
	VSCURSOR_CONTROL_MOVE	= 6,
	VSCURSOR_CROSS	= 7,
	VSCURSOR_DRAGDOCUMENT_MOVE	= 8,
	VSCURSOR_DRAGDOCUMENT_NOEFFECT	= 9,
	VSCURSOR_DRAGSCRAP_COPY	= 10,
	VSCURSOR_DRAGSCRAP_MOVE	= 11,
	VSCURSOR_DRAGSCRAP_SCROLL	= 12,
	VSCURSOR_HAND	= 13,
	VSCURSOR_IBEAM	= 14,
	VSCURSOR_ISEARCH	= 15,
	VSCURSOR_ISEARCH_UP	= 16,
	VSCURSOR_MACRO_RECORD_NO	= 17,
	VSCURSOR_NO	= 18,
	VSCURSOR_NOMOVE_2D	= 19,
	VSCURSOR_NOMOVE_HORIZ	= 20,
	VSCURSOR_NOMOVE_VERT	= 21,
	VSCURSOR_PAN_EAST	= 22,
	VSCURSOR_PAN_NE	= 23,
	VSCURSOR_PAN_NORTH	= 24,
	VSCURSOR_PAN_NW	= 25,
	VSCURSOR_PAN_SE	= 26,
	VSCURSOR_PAN_SOUTH	= 27,
	VSCURSOR_PAN_SW	= 28,
	VSCURSOR_PAN_WEST	= 29,
	VSCURSOR_POINTER	= 30,
	VSCURSOR_POINTER_REVERSE	= 31,
	VSCURSOR_SIZE_NS	= 32,
	VSCURSOR_SIZE_EW	= 33,
	VSCURSOR_SIZE_NWSE	= 34,
	VSCURSOR_SIZE_NESW	= 35,
	VSCURSOR_SIZE_ALL	= 36,
	VSCURSOR_SPLIT_EW	= 37,
	VSCURSOR_SPLIT_NS	= 38,
	VSCURSOR_UPARROW	= 39,
	VSCURSOR_WAIT	= 40
    } ;
typedef DWORD VSCURSORTYPE;

#define OFN_NOADDALLFILESFILTER  0x40000000
typedef 
enum tagBWIImagePos
    {	BWI_IMAGE_POS_LEFT	= 0,
	BWI_IMAGE_POS_RIGHT	= 0x1,
	BWI_IMAGE_ONLY	= 0x2
    } 	__BWI_IMAGE_POS;

typedef DWORD BWI_IMAGE_POS;

typedef struct tagVSDRAWITEMSTRUCT
    {
    UINT CtlType;
    UINT CtlID;
    UINT itemID;
    UINT itemAction;
    UINT itemState;
    HWND hwndItem;
    HDC hDC;
    RECT rcItem;
    ULONG_PTR itemData;
    } 	VSDRAWITEMSTRUCT;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0027_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0027_v0_0_s_ifspec;

#ifndef __IVsImageButton_INTERFACE_DEFINED__
#define __IVsImageButton_INTERFACE_DEFINED__

/* interface IVsImageButton */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsImageButton;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("61DF9CCE-E88E-4fe2-9976-77A4F478E24B")
    IVsImageButton : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Draw( 
            /* [in] */ __RPC__in VSDRAWITEMSTRUCT *pDrawItemStruct,
            /* [in] */ BOOL fHot) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsImageButtonVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsImageButton * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsImageButton * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsImageButton * This);
        
        HRESULT ( STDMETHODCALLTYPE *Draw )( 
            IVsImageButton * This,
            /* [in] */ __RPC__in VSDRAWITEMSTRUCT *pDrawItemStruct,
            /* [in] */ BOOL fHot);
        
        END_INTERFACE
    } IVsImageButtonVtbl;

    interface IVsImageButton
    {
        CONST_VTBL struct IVsImageButtonVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsImageButton_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsImageButton_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsImageButton_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsImageButton_Draw(This,pDrawItemStruct,fHot)	\
    ( (This)->lpVtbl -> Draw(This,pDrawItemStruct,fHot) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsImageButton_INTERFACE_DEFINED__ */


#ifndef __IVsUIShell2_INTERFACE_DEFINED__
#define __IVsUIShell2_INTERFACE_DEFINED__

/* interface IVsUIShell2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsUIShell2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4E6B6EF9-8E3D-4756-99E9-1192BAAD5496")
    IVsUIShell2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetOpenFileNameViaDlgEx( 
            /* [out][in] */ __RPC__inout VSOPENFILENAMEW *pOpenFileName,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSaveFileNameViaDlgEx( 
            /* [out][in] */ __RPC__inout VSSAVEFILENAMEW *pSaveFileName,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDirectoryViaBrowseDlgEx( 
            /* [out][in] */ __RPC__inout VSBROWSEINFOW *pBrowse,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic,
            /* [in] */ __RPC__in LPCOLESTR pszOpenButtonLabel,
            /* [in] */ __RPC__in LPCOLESTR pszCeilingDir,
            /* [in] */ __RPC__in VSNSEBROWSEINFOW *pNSEBrowseInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveItemsViaDlg( 
            /* [in] */ UINT cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSSAVETREEITEM rgSaveItems[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVSSysColorEx( 
            /* [in] */ VSSYSCOLOREX dwSysColIndex,
            /* [out] */ __RPC__out DWORD *pdwRGBval) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateGradient( 
            /* [in] */ GRADIENTTYPE gradientType,
            /* [out] */ __RPC__deref_out_opt IVsGradient **pGradient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVSCursor( 
            /* [in] */ VSCURSORTYPE cursor,
            /* [out] */ __RPC__deref_out_opt HCURSOR *phIcon) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsAutoRecoverSavingCheckpoints( 
            /* [out] */ __RPC__out BOOL *pfARSaving) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE VsDialogBoxParam( 
            /* [in] */ HINSTANCE hinst,
            /* [in] */ DWORD dwId,
            /* [in] */ DLGPROC lpDialogFunc,
            /* [in] */ LPARAM lp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateIconImageButton( 
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ __RPC__in HICON hicon,
            /* [in] */ BWI_IMAGE_POS bwiPos,
            /* [out] */ __RPC__deref_out_opt IVsImageButton **ppImageButton) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateGlyphImageButton( 
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ WCHAR chGlyph,
            /* [in] */ int xShift,
            /* [in] */ int yShift,
            /* [in] */ BWI_IMAGE_POS bwiPos,
            /* [out] */ __RPC__deref_out_opt IVsImageButton **ppImageButton) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIShell2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIShell2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIShell2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIShell2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetOpenFileNameViaDlgEx )( 
            IVsUIShell2 * This,
            /* [out][in] */ __RPC__inout VSOPENFILENAMEW *pOpenFileName,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic);
        
        HRESULT ( STDMETHODCALLTYPE *GetSaveFileNameViaDlgEx )( 
            IVsUIShell2 * This,
            /* [out][in] */ __RPC__inout VSSAVEFILENAMEW *pSaveFileName,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic);
        
        HRESULT ( STDMETHODCALLTYPE *GetDirectoryViaBrowseDlgEx )( 
            IVsUIShell2 * This,
            /* [out][in] */ __RPC__inout VSBROWSEINFOW *pBrowse,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic,
            /* [in] */ __RPC__in LPCOLESTR pszOpenButtonLabel,
            /* [in] */ __RPC__in LPCOLESTR pszCeilingDir,
            /* [in] */ __RPC__in VSNSEBROWSEINFOW *pNSEBrowseInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SaveItemsViaDlg )( 
            IVsUIShell2 * This,
            /* [in] */ UINT cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSSAVETREEITEM rgSaveItems[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetVSSysColorEx )( 
            IVsUIShell2 * This,
            /* [in] */ VSSYSCOLOREX dwSysColIndex,
            /* [out] */ __RPC__out DWORD *pdwRGBval);
        
        HRESULT ( STDMETHODCALLTYPE *CreateGradient )( 
            IVsUIShell2 * This,
            /* [in] */ GRADIENTTYPE gradientType,
            /* [out] */ __RPC__deref_out_opt IVsGradient **pGradient);
        
        HRESULT ( STDMETHODCALLTYPE *GetVSCursor )( 
            IVsUIShell2 * This,
            /* [in] */ VSCURSORTYPE cursor,
            /* [out] */ __RPC__deref_out_opt HCURSOR *phIcon);
        
        HRESULT ( STDMETHODCALLTYPE *IsAutoRecoverSavingCheckpoints )( 
            IVsUIShell2 * This,
            /* [out] */ __RPC__out BOOL *pfARSaving);
        
        HRESULT ( STDMETHODCALLTYPE *VsDialogBoxParam )( 
            IVsUIShell2 * This,
            /* [in] */ HINSTANCE hinst,
            /* [in] */ DWORD dwId,
            /* [in] */ DLGPROC lpDialogFunc,
            /* [in] */ LPARAM lp);
        
        HRESULT ( STDMETHODCALLTYPE *CreateIconImageButton )( 
            IVsUIShell2 * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ __RPC__in HICON hicon,
            /* [in] */ BWI_IMAGE_POS bwiPos,
            /* [out] */ __RPC__deref_out_opt IVsImageButton **ppImageButton);
        
        HRESULT ( STDMETHODCALLTYPE *CreateGlyphImageButton )( 
            IVsUIShell2 * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ WCHAR chGlyph,
            /* [in] */ int xShift,
            /* [in] */ int yShift,
            /* [in] */ BWI_IMAGE_POS bwiPos,
            /* [out] */ __RPC__deref_out_opt IVsImageButton **ppImageButton);
        
        END_INTERFACE
    } IVsUIShell2Vtbl;

    interface IVsUIShell2
    {
        CONST_VTBL struct IVsUIShell2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShell2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShell2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShell2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShell2_GetOpenFileNameViaDlgEx(This,pOpenFileName,pszHelpTopic)	\
    ( (This)->lpVtbl -> GetOpenFileNameViaDlgEx(This,pOpenFileName,pszHelpTopic) ) 

#define IVsUIShell2_GetSaveFileNameViaDlgEx(This,pSaveFileName,pszHelpTopic)	\
    ( (This)->lpVtbl -> GetSaveFileNameViaDlgEx(This,pSaveFileName,pszHelpTopic) ) 

#define IVsUIShell2_GetDirectoryViaBrowseDlgEx(This,pBrowse,pszHelpTopic,pszOpenButtonLabel,pszCeilingDir,pNSEBrowseInfo)	\
    ( (This)->lpVtbl -> GetDirectoryViaBrowseDlgEx(This,pBrowse,pszHelpTopic,pszOpenButtonLabel,pszCeilingDir,pNSEBrowseInfo) ) 

#define IVsUIShell2_SaveItemsViaDlg(This,cItems,rgSaveItems)	\
    ( (This)->lpVtbl -> SaveItemsViaDlg(This,cItems,rgSaveItems) ) 

#define IVsUIShell2_GetVSSysColorEx(This,dwSysColIndex,pdwRGBval)	\
    ( (This)->lpVtbl -> GetVSSysColorEx(This,dwSysColIndex,pdwRGBval) ) 

#define IVsUIShell2_CreateGradient(This,gradientType,pGradient)	\
    ( (This)->lpVtbl -> CreateGradient(This,gradientType,pGradient) ) 

#define IVsUIShell2_GetVSCursor(This,cursor,phIcon)	\
    ( (This)->lpVtbl -> GetVSCursor(This,cursor,phIcon) ) 

#define IVsUIShell2_IsAutoRecoverSavingCheckpoints(This,pfARSaving)	\
    ( (This)->lpVtbl -> IsAutoRecoverSavingCheckpoints(This,pfARSaving) ) 

#define IVsUIShell2_VsDialogBoxParam(This,hinst,dwId,lpDialogFunc,lp)	\
    ( (This)->lpVtbl -> VsDialogBoxParam(This,hinst,dwId,lpDialogFunc,lp) ) 

#define IVsUIShell2_CreateIconImageButton(This,hwnd,hicon,bwiPos,ppImageButton)	\
    ( (This)->lpVtbl -> CreateIconImageButton(This,hwnd,hicon,bwiPos,ppImageButton) ) 

#define IVsUIShell2_CreateGlyphImageButton(This,hwnd,chGlyph,xShift,yShift,bwiPos,ppImageButton)	\
    ( (This)->lpVtbl -> CreateGlyphImageButton(This,hwnd,chGlyph,xShift,yShift,bwiPos,ppImageButton) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShell2_INTERFACE_DEFINED__ */


#ifndef __SVsMainWindowDropTarget_INTERFACE_DEFINED__
#define __SVsMainWindowDropTarget_INTERFACE_DEFINED__

/* interface SVsMainWindowDropTarget */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsMainWindowDropTarget;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8ABE01DB-4CB1-4be7-961F-D30B2EF6AEB1")
    SVsMainWindowDropTarget : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsMainWindowDropTargetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsMainWindowDropTarget * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsMainWindowDropTarget * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsMainWindowDropTarget * This);
        
        END_INTERFACE
    } SVsMainWindowDropTargetVtbl;

    interface SVsMainWindowDropTarget
    {
        CONST_VTBL struct SVsMainWindowDropTargetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsMainWindowDropTarget_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsMainWindowDropTarget_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsMainWindowDropTarget_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsMainWindowDropTarget_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0030 */
/* [local] */ 

#define SID_SVsMainWindowDropTarget IID_SVsMainWindowDropTarget


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0030_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0030_v0_0_s_ifspec;

#ifndef __IVsSupportItemHandoff2_INTERFACE_DEFINED__
#define __IVsSupportItemHandoff2_INTERFACE_DEFINED__

/* interface IVsSupportItemHandoff2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSupportItemHandoff2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2AFA4F74-7A1A-4dee-8F99-46E74E5A3C0F")
    IVsSupportItemHandoff2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeHandoffItem( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IVsProject3 *pProjDest) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSupportItemHandoff2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSupportItemHandoff2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSupportItemHandoff2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSupportItemHandoff2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeHandoffItem )( 
            IVsSupportItemHandoff2 * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IVsProject3 *pProjDest);
        
        END_INTERFACE
    } IVsSupportItemHandoff2Vtbl;

    interface IVsSupportItemHandoff2
    {
        CONST_VTBL struct IVsSupportItemHandoff2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSupportItemHandoff2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSupportItemHandoff2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSupportItemHandoff2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSupportItemHandoff2_OnBeforeHandoffItem(This,itemid,pProjDest)	\
    ( (This)->lpVtbl -> OnBeforeHandoffItem(This,itemid,pProjDest) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSupportItemHandoff2_INTERFACE_DEFINED__ */


#ifndef __IVsLaunchPadOutputParser_INTERFACE_DEFINED__
#define __IVsLaunchPadOutputParser_INTERFACE_DEFINED__

/* interface IVsLaunchPadOutputParser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLaunchPadOutputParser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A9832932-5F3B-487d-A80D-95115EADDAC3")
    IVsLaunchPadOutputParser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ParseOutputStringForInfo( 
            /* [in] */ __RPC__in LPCOLESTR pszOutputString,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrFilename,
            /* [optional][out] */ __RPC__out ULONG *pnLineNum,
            /* [optional][out] */ __RPC__out ULONG *pnPriority,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrTaskItemText,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrHelpKeyword) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLaunchPadOutputParserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLaunchPadOutputParser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLaunchPadOutputParser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLaunchPadOutputParser * This);
        
        HRESULT ( STDMETHODCALLTYPE *ParseOutputStringForInfo )( 
            IVsLaunchPadOutputParser * This,
            /* [in] */ __RPC__in LPCOLESTR pszOutputString,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrFilename,
            /* [optional][out] */ __RPC__out ULONG *pnLineNum,
            /* [optional][out] */ __RPC__out ULONG *pnPriority,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrTaskItemText,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrHelpKeyword);
        
        END_INTERFACE
    } IVsLaunchPadOutputParserVtbl;

    interface IVsLaunchPadOutputParser
    {
        CONST_VTBL struct IVsLaunchPadOutputParserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLaunchPadOutputParser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLaunchPadOutputParser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLaunchPadOutputParser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLaunchPadOutputParser_ParseOutputStringForInfo(This,pszOutputString,pbstrFilename,pnLineNum,pnPriority,pbstrTaskItemText,pbstrHelpKeyword)	\
    ( (This)->lpVtbl -> ParseOutputStringForInfo(This,pszOutputString,pbstrFilename,pnLineNum,pnPriority,pbstrTaskItemText,pbstrHelpKeyword) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLaunchPadOutputParser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0032 */
/* [local] */ 

typedef /* [public] */ 
enum __MIDL___MIDL_itf_vsshell80_0000_0032_0001
    {	LPF_TreatOutputAsUnicode	= 0x10
    } 	_LAUNCHPAD_FLAGS2;

typedef DWORD LAUNCHPAD_FLAGS2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0032_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0032_v0_0_s_ifspec;

#ifndef __IVsLaunchPad2_INTERFACE_DEFINED__
#define __IVsLaunchPad2_INTERFACE_DEFINED__

/* interface IVsLaunchPad2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLaunchPad2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0DBD685A-0A10-4e25-B88E-02E58E60785E")
    IVsLaunchPad2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExecCommandEx( 
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir,
            /* [in] */ LAUNCHPAD_FLAGS2 lpf,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pOutputWindowPane,
            /* [in] */ ULONG nTaskItemCategory,
            /* [in] */ ULONG nTaskItemBitmap,
            /* [in] */ __RPC__in LPCOLESTR pszTaskListSubcategory,
            /* [in] */ __RPC__in_opt IVsLaunchPadEvents *pVsLaunchPadEvents,
            /* [in] */ __RPC__in_opt IVsLaunchPadOutputParser *pOutputParser,
            /* [optional][out] */ __RPC__out DWORD *pdwProcessExitCode,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrOutput) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLaunchPad2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLaunchPad2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLaunchPad2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLaunchPad2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExecCommandEx )( 
            IVsLaunchPad2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir,
            /* [in] */ LAUNCHPAD_FLAGS2 lpf,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pOutputWindowPane,
            /* [in] */ ULONG nTaskItemCategory,
            /* [in] */ ULONG nTaskItemBitmap,
            /* [in] */ __RPC__in LPCOLESTR pszTaskListSubcategory,
            /* [in] */ __RPC__in_opt IVsLaunchPadEvents *pVsLaunchPadEvents,
            /* [in] */ __RPC__in_opt IVsLaunchPadOutputParser *pOutputParser,
            /* [optional][out] */ __RPC__out DWORD *pdwProcessExitCode,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrOutput);
        
        END_INTERFACE
    } IVsLaunchPad2Vtbl;

    interface IVsLaunchPad2
    {
        CONST_VTBL struct IVsLaunchPad2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLaunchPad2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLaunchPad2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLaunchPad2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLaunchPad2_ExecCommandEx(This,pszApplicationName,pszCommandLine,pszWorkingDir,lpf,pOutputWindowPane,nTaskItemCategory,nTaskItemBitmap,pszTaskListSubcategory,pVsLaunchPadEvents,pOutputParser,pdwProcessExitCode,pbstrOutput)	\
    ( (This)->lpVtbl -> ExecCommandEx(This,pszApplicationName,pszCommandLine,pszWorkingDir,lpf,pOutputWindowPane,nTaskItemCategory,nTaskItemBitmap,pszTaskListSubcategory,pVsLaunchPadEvents,pOutputParser,pdwProcessExitCode,pbstrOutput) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLaunchPad2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0033 */
/* [local] */ 


enum __VSPROJSLNDLGFLAGS
    {	PSDF_OpenSolutionDialog	= 0x1,
	PSDF_OpenProjectDialog	= 0x2,
	PSDF_AddExistingProjectDialog	= 0x4,
	PSDF_DefaultToAllProjectsFilter	= 0x8,
	PSDF_DirectoryPicker	= 0x10
    } ;
typedef DWORD VSPROJSLNDLGFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0033_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0033_v0_0_s_ifspec;

#ifndef __IVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__
#define __IVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__

/* interface IVsOpenProjectOrSolutionDlg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsOpenProjectOrSolutionDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("09b17094-f50c-40e0-8ab5-57c22a786596")
    IVsOpenProjectOrSolutionDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenProjectOrSolutionViaDlg( 
            /* [in] */ VSPROJSLNDLGFLAGS grfProjSlnDlgFlags,
            /* [in] */ __RPC__in LPCOLESTR pwzStartDirectory,
            /* [in] */ __RPC__in LPCOLESTR pwzDialogTitle,
            /* [in] */ __RPC__in REFGUID rguidProjectType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOpenProjectOrSolutionDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOpenProjectOrSolutionDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOpenProjectOrSolutionDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOpenProjectOrSolutionDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenProjectOrSolutionViaDlg )( 
            IVsOpenProjectOrSolutionDlg * This,
            /* [in] */ VSPROJSLNDLGFLAGS grfProjSlnDlgFlags,
            /* [in] */ __RPC__in LPCOLESTR pwzStartDirectory,
            /* [in] */ __RPC__in LPCOLESTR pwzDialogTitle,
            /* [in] */ __RPC__in REFGUID rguidProjectType);
        
        END_INTERFACE
    } IVsOpenProjectOrSolutionDlgVtbl;

    interface IVsOpenProjectOrSolutionDlg
    {
        CONST_VTBL struct IVsOpenProjectOrSolutionDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOpenProjectOrSolutionDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOpenProjectOrSolutionDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOpenProjectOrSolutionDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOpenProjectOrSolutionDlg_OpenProjectOrSolutionViaDlg(This,grfProjSlnDlgFlags,pwzStartDirectory,pwzDialogTitle,rguidProjectType)	\
    ( (This)->lpVtbl -> OpenProjectOrSolutionViaDlg(This,grfProjSlnDlgFlags,pwzStartDirectory,pwzDialogTitle,rguidProjectType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__ */


#ifndef __SVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__
#define __SVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__

/* interface SVsOpenProjectOrSolutionDlg */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsOpenProjectOrSolutionDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A3761D3F-7C1E-4526-A8D5-1575631F09FC")
    SVsOpenProjectOrSolutionDlg : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsOpenProjectOrSolutionDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsOpenProjectOrSolutionDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsOpenProjectOrSolutionDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsOpenProjectOrSolutionDlg * This);
        
        END_INTERFACE
    } SVsOpenProjectOrSolutionDlgVtbl;

    interface SVsOpenProjectOrSolutionDlg
    {
        CONST_VTBL struct SVsOpenProjectOrSolutionDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsOpenProjectOrSolutionDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsOpenProjectOrSolutionDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsOpenProjectOrSolutionDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsOpenProjectOrSolutionDlg_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0035 */
/* [local] */ 

#define SID_SVsOpenProjectOrSolutionDlg IID_SVsOpenProjectOrSolutionDlg


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0035_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0035_v0_0_s_ifspec;

#ifndef __IVsCreateAggregateProject_INTERFACE_DEFINED__
#define __IVsCreateAggregateProject_INTERFACE_DEFINED__

/* interface IVsCreateAggregateProject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCreateAggregateProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("84f41718-d169-4567-a0cd-b3cbcf58ff71")
    IVsCreateAggregateProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateAggregateProject( 
            /* [in] */ __RPC__in LPCOLESTR pszProjectTypeGuids,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in LPCOLESTR pszLocation,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvProject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCreateAggregateProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCreateAggregateProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCreateAggregateProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCreateAggregateProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateAggregateProject )( 
            IVsCreateAggregateProject * This,
            /* [in] */ __RPC__in LPCOLESTR pszProjectTypeGuids,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in LPCOLESTR pszLocation,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvProject);
        
        END_INTERFACE
    } IVsCreateAggregateProjectVtbl;

    interface IVsCreateAggregateProject
    {
        CONST_VTBL struct IVsCreateAggregateProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCreateAggregateProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCreateAggregateProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCreateAggregateProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCreateAggregateProject_CreateAggregateProject(This,pszProjectTypeGuids,pszFilename,pszLocation,pszName,grfCreateFlags,iidProject,ppvProject)	\
    ( (This)->lpVtbl -> CreateAggregateProject(This,pszProjectTypeGuids,pszFilename,pszLocation,pszName,grfCreateFlags,iidProject,ppvProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCreateAggregateProject_INTERFACE_DEFINED__ */


#ifndef __SVsCreateAggregateProject_INTERFACE_DEFINED__
#define __SVsCreateAggregateProject_INTERFACE_DEFINED__

/* interface SVsCreateAggregateProject */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsCreateAggregateProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("50B729AD-AC3B-451f-BE03-9EA167F5D637")
    SVsCreateAggregateProject : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsCreateAggregateProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsCreateAggregateProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsCreateAggregateProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsCreateAggregateProject * This);
        
        END_INTERFACE
    } SVsCreateAggregateProjectVtbl;

    interface SVsCreateAggregateProject
    {
        CONST_VTBL struct SVsCreateAggregateProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsCreateAggregateProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsCreateAggregateProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsCreateAggregateProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsCreateAggregateProject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0037 */
/* [local] */ 

#define SID_SVsCreateAggregateProject IID_SVsCreateAggregateProject


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0037_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0037_v0_0_s_ifspec;

#ifndef __IVsAggregatableProject_INTERFACE_DEFINED__
#define __IVsAggregatableProject_INTERFACE_DEFINED__

/* interface IVsAggregatableProject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAggregatableProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ffb2e715-7312-4b93-83d7-d37bcc561c90")
    IVsAggregatableProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetInnerProject( 
            /* [in] */ __RPC__in_opt IUnknown *punkInner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeForOuter( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in LPCOLESTR pszLocation,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvProject,
            /* [out] */ __RPC__out BOOL *pfCanceled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAggregationComplete( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAggregateProjectTypeGuids( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjTypeGuids) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetAggregateProjectTypeGuids( 
            /* [in] */ __RPC__in LPCOLESTR lpstrProjTypeGuids) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAggregatableProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAggregatableProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAggregatableProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAggregatableProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetInnerProject )( 
            IVsAggregatableProject * This,
            /* [in] */ __RPC__in_opt IUnknown *punkInner);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeForOuter )( 
            IVsAggregatableProject * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in LPCOLESTR pszLocation,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvProject,
            /* [out] */ __RPC__out BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *OnAggregationComplete )( 
            IVsAggregatableProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAggregateProjectTypeGuids )( 
            IVsAggregatableProject * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjTypeGuids);
        
        HRESULT ( STDMETHODCALLTYPE *SetAggregateProjectTypeGuids )( 
            IVsAggregatableProject * This,
            /* [in] */ __RPC__in LPCOLESTR lpstrProjTypeGuids);
        
        END_INTERFACE
    } IVsAggregatableProjectVtbl;

    interface IVsAggregatableProject
    {
        CONST_VTBL struct IVsAggregatableProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAggregatableProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAggregatableProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAggregatableProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAggregatableProject_SetInnerProject(This,punkInner)	\
    ( (This)->lpVtbl -> SetInnerProject(This,punkInner) ) 

#define IVsAggregatableProject_InitializeForOuter(This,pszFilename,pszLocation,pszName,grfCreateFlags,iidProject,ppvProject,pfCanceled)	\
    ( (This)->lpVtbl -> InitializeForOuter(This,pszFilename,pszLocation,pszName,grfCreateFlags,iidProject,ppvProject,pfCanceled) ) 

#define IVsAggregatableProject_OnAggregationComplete(This)	\
    ( (This)->lpVtbl -> OnAggregationComplete(This) ) 

#define IVsAggregatableProject_GetAggregateProjectTypeGuids(This,pbstrProjTypeGuids)	\
    ( (This)->lpVtbl -> GetAggregateProjectTypeGuids(This,pbstrProjTypeGuids) ) 

#define IVsAggregatableProject_SetAggregateProjectTypeGuids(This,lpstrProjTypeGuids)	\
    ( (This)->lpVtbl -> SetAggregateProjectTypeGuids(This,lpstrProjTypeGuids) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAggregatableProject_INTERFACE_DEFINED__ */


#ifndef __IVsAggregatableProjectFactory_INTERFACE_DEFINED__
#define __IVsAggregatableProjectFactory_INTERFACE_DEFINED__

/* interface IVsAggregatableProjectFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAggregatableProjectFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44569501-2ad0-4966-9bac-12b799a1ced6")
    IVsAggregatableProjectFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAggregateProjectType( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjTypeGuid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PreCreateForOuter( 
            /* [in] */ __RPC__in_opt IUnknown *punkOuter,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkProject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAggregatableProjectFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAggregatableProjectFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAggregatableProjectFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAggregatableProjectFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAggregateProjectType )( 
            IVsAggregatableProjectFactory * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjTypeGuid);
        
        HRESULT ( STDMETHODCALLTYPE *PreCreateForOuter )( 
            IVsAggregatableProjectFactory * This,
            /* [in] */ __RPC__in_opt IUnknown *punkOuter,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkProject);
        
        END_INTERFACE
    } IVsAggregatableProjectFactoryVtbl;

    interface IVsAggregatableProjectFactory
    {
        CONST_VTBL struct IVsAggregatableProjectFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAggregatableProjectFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAggregatableProjectFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAggregatableProjectFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAggregatableProjectFactory_GetAggregateProjectType(This,pszFilename,pbstrProjTypeGuid)	\
    ( (This)->lpVtbl -> GetAggregateProjectType(This,pszFilename,pbstrProjTypeGuid) ) 

#define IVsAggregatableProjectFactory_PreCreateForOuter(This,punkOuter,ppunkProject)	\
    ( (This)->lpVtbl -> PreCreateForOuter(This,punkOuter,ppunkProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAggregatableProjectFactory_INTERFACE_DEFINED__ */


#ifndef __IVsParentProject2_INTERFACE_DEFINED__
#define __IVsParentProject2_INTERFACE_DEFINED__

/* interface IVsParentProject2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsParentProject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D63BB7D7-D7A0-4c02-AA85-7E9233797CDB")
    IVsParentProject2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateNestedProject( 
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ __RPC__in REFGUID rguidProjectType,
            /* [in] */ __RPC__in LPCOLESTR lpszMoniker,
            /* [in] */ __RPC__in LPCOLESTR lpszLocation,
            /* [in] */ __RPC__in LPCOLESTR lpszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddNestedSolution( 
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSSLNOPENOPTIONS grfOpenOpts,
            /* [in] */ __RPC__in LPCOLESTR pszFilename) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsParentProject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsParentProject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsParentProject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsParentProject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNestedProject )( 
            IVsParentProject2 * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ __RPC__in REFGUID rguidProjectType,
            /* [in] */ __RPC__in LPCOLESTR lpszMoniker,
            /* [in] */ __RPC__in LPCOLESTR lpszLocation,
            /* [in] */ __RPC__in LPCOLESTR lpszName,
            /* [in] */ VSCREATEPROJFLAGS grfCreateFlags,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in REFIID iidProject,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppProject);
        
        HRESULT ( STDMETHODCALLTYPE *AddNestedSolution )( 
            IVsParentProject2 * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSSLNOPENOPTIONS grfOpenOpts,
            /* [in] */ __RPC__in LPCOLESTR pszFilename);
        
        END_INTERFACE
    } IVsParentProject2Vtbl;

    interface IVsParentProject2
    {
        CONST_VTBL struct IVsParentProject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsParentProject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsParentProject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsParentProject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsParentProject2_CreateNestedProject(This,itemidLoc,rguidProjectType,lpszMoniker,lpszLocation,lpszName,grfCreateFlags,rguidProjectID,iidProject,ppProject)	\
    ( (This)->lpVtbl -> CreateNestedProject(This,itemidLoc,rguidProjectType,lpszMoniker,lpszLocation,lpszName,grfCreateFlags,rguidProjectID,iidProject,ppProject) ) 

#define IVsParentProject2_AddNestedSolution(This,itemidLoc,grfOpenOpts,pszFilename)	\
    ( (This)->lpVtbl -> AddNestedSolution(This,itemidLoc,grfOpenOpts,pszFilename) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsParentProject2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0040 */
/* [local] */ 


enum _PersistStorageType
    {	PST_PROJECT_FILE	= 1,
	PST_USER_FILE	= 2
    } ;
typedef DWORD PersistStorageType;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0040_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0040_v0_0_s_ifspec;

#ifndef __IVsBuildPropertyStorage_INTERFACE_DEFINED__
#define __IVsBuildPropertyStorage_INTERFACE_DEFINED__

/* interface IVsBuildPropertyStorage */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildPropertyStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E7355FDF-A118-48f5-9655-7EFD9D2DC352")
    IVsBuildPropertyStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPropertyValue( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPropertyValue( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveProperty( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemAttribute( 
            /* [in] */ VSITEMID item,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttributeValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetItemAttribute( 
            /* [in] */ VSITEMID item,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBuildPropertyStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBuildPropertyStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBuildPropertyStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyValue )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPropValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetPropertyValue )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveProperty )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszConfigName,
            /* [in] */ PersistStorageType storage);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemAttribute )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ VSITEMID item,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAttributeValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetItemAttribute )( 
            IVsBuildPropertyStorage * This,
            /* [in] */ VSITEMID item,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeValue);
        
        END_INTERFACE
    } IVsBuildPropertyStorageVtbl;

    interface IVsBuildPropertyStorage
    {
        CONST_VTBL struct IVsBuildPropertyStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildPropertyStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildPropertyStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildPropertyStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildPropertyStorage_GetPropertyValue(This,pszPropName,pszConfigName,storage,pbstrPropValue)	\
    ( (This)->lpVtbl -> GetPropertyValue(This,pszPropName,pszConfigName,storage,pbstrPropValue) ) 

#define IVsBuildPropertyStorage_SetPropertyValue(This,pszPropName,pszConfigName,storage,pszPropValue)	\
    ( (This)->lpVtbl -> SetPropertyValue(This,pszPropName,pszConfigName,storage,pszPropValue) ) 

#define IVsBuildPropertyStorage_RemoveProperty(This,pszPropName,pszConfigName,storage)	\
    ( (This)->lpVtbl -> RemoveProperty(This,pszPropName,pszConfigName,storage) ) 

#define IVsBuildPropertyStorage_GetItemAttribute(This,item,pszAttributeName,pbstrAttributeValue)	\
    ( (This)->lpVtbl -> GetItemAttribute(This,item,pszAttributeName,pbstrAttributeValue) ) 

#define IVsBuildPropertyStorage_SetItemAttribute(This,item,pszAttributeName,pszAttributeValue)	\
    ( (This)->lpVtbl -> SetItemAttribute(This,item,pszAttributeName,pszAttributeValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildPropertyStorage_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0041 */
/* [local] */ 


enum _BuildSystemKindFlags
    {	BSK_MSBUILD	= 1
    } ;
typedef DWORD BuildSystemKindFlags;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0041_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0041_v0_0_s_ifspec;

#ifndef __IVsProjectBuildSystem_INTERFACE_DEFINED__
#define __IVsProjectBuildSystem_INTERFACE_DEFINED__

/* interface IVsProjectBuildSystem */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectBuildSystem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("eb0718c0-e050-4657-872b-e845cd4f617b")
    IVsProjectBuildSystem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetHostObject( 
            /* [in] */ __RPC__in LPCOLESTR pszTargetName,
            /* [in] */ __RPC__in LPCOLESTR pszTaskName,
            /* [in] */ __RPC__in_opt IUnknown *punkHostObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartBatchEdit( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBatchEdit( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelBatchEdit( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BuildTarget( 
            /* [in] */ __RPC__in LPCOLESTR pszTargetName,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSuccess) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBuildSystemKind( 
            /* [retval][out] */ __RPC__out BuildSystemKindFlags *pBuildSystemKind) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectBuildSystemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectBuildSystem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectBuildSystem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectBuildSystem * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetHostObject )( 
            IVsProjectBuildSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszTargetName,
            /* [in] */ __RPC__in LPCOLESTR pszTaskName,
            /* [in] */ __RPC__in_opt IUnknown *punkHostObject);
        
        HRESULT ( STDMETHODCALLTYPE *StartBatchEdit )( 
            IVsProjectBuildSystem * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBatchEdit )( 
            IVsProjectBuildSystem * This);
        
        HRESULT ( STDMETHODCALLTYPE *CancelBatchEdit )( 
            IVsProjectBuildSystem * This);
        
        HRESULT ( STDMETHODCALLTYPE *BuildTarget )( 
            IVsProjectBuildSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszTargetName,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbSuccess);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuildSystemKind )( 
            IVsProjectBuildSystem * This,
            /* [retval][out] */ __RPC__out BuildSystemKindFlags *pBuildSystemKind);
        
        END_INTERFACE
    } IVsProjectBuildSystemVtbl;

    interface IVsProjectBuildSystem
    {
        CONST_VTBL struct IVsProjectBuildSystemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectBuildSystem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectBuildSystem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectBuildSystem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectBuildSystem_SetHostObject(This,pszTargetName,pszTaskName,punkHostObject)	\
    ( (This)->lpVtbl -> SetHostObject(This,pszTargetName,pszTaskName,punkHostObject) ) 

#define IVsProjectBuildSystem_StartBatchEdit(This)	\
    ( (This)->lpVtbl -> StartBatchEdit(This) ) 

#define IVsProjectBuildSystem_EndBatchEdit(This)	\
    ( (This)->lpVtbl -> EndBatchEdit(This) ) 

#define IVsProjectBuildSystem_CancelBatchEdit(This)	\
    ( (This)->lpVtbl -> CancelBatchEdit(This) ) 

#define IVsProjectBuildSystem_BuildTarget(This,pszTargetName,pbSuccess)	\
    ( (This)->lpVtbl -> BuildTarget(This,pszTargetName,pbSuccess) ) 

#define IVsProjectBuildSystem_GetBuildSystemKind(This,pBuildSystemKind)	\
    ( (This)->lpVtbl -> GetBuildSystemKind(This,pBuildSystemKind) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectBuildSystem_INTERFACE_DEFINED__ */


#ifndef __IPersistXMLFragment_INTERFACE_DEFINED__
#define __IPersistXMLFragment_INTERFACE_DEFINED__

/* interface IPersistXMLFragment */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IPersistXMLFragment;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B0C8632-6F01-4e54-9645-FFE82A2F4FE9")
    IPersistXMLFragment : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InitNew( 
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsFragmentDirty( 
            /* [in] */ PersistStorageType storage,
            /* [out] */ __RPC__out BOOL *pfDirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszXMLFragment) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( 
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrXMLFragment,
            /* [in] */ BOOL fClearDirty) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPersistXMLFragmentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPersistXMLFragment * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPersistXMLFragment * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPersistXMLFragment * This);
        
        HRESULT ( STDMETHODCALLTYPE *InitNew )( 
            IPersistXMLFragment * This,
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage);
        
        HRESULT ( STDMETHODCALLTYPE *IsFragmentDirty )( 
            IPersistXMLFragment * This,
            /* [in] */ PersistStorageType storage,
            /* [out] */ __RPC__out BOOL *pfDirty);
        
        HRESULT ( STDMETHODCALLTYPE *Load )( 
            IPersistXMLFragment * This,
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszXMLFragment);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IPersistXMLFragment * This,
            /* [in] */ __RPC__in REFGUID guidFlavor,
            /* [in] */ PersistStorageType storage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrXMLFragment,
            /* [in] */ BOOL fClearDirty);
        
        END_INTERFACE
    } IPersistXMLFragmentVtbl;

    interface IPersistXMLFragment
    {
        CONST_VTBL struct IPersistXMLFragmentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPersistXMLFragment_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPersistXMLFragment_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPersistXMLFragment_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPersistXMLFragment_InitNew(This,guidFlavor,storage)	\
    ( (This)->lpVtbl -> InitNew(This,guidFlavor,storage) ) 

#define IPersistXMLFragment_IsFragmentDirty(This,storage,pfDirty)	\
    ( (This)->lpVtbl -> IsFragmentDirty(This,storage,pfDirty) ) 

#define IPersistXMLFragment_Load(This,guidFlavor,storage,pszXMLFragment)	\
    ( (This)->lpVtbl -> Load(This,guidFlavor,storage,pszXMLFragment) ) 

#define IPersistXMLFragment_Save(This,guidFlavor,storage,pbstrXMLFragment,fClearDirty)	\
    ( (This)->lpVtbl -> Save(This,guidFlavor,storage,pbstrXMLFragment,fClearDirty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPersistXMLFragment_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorCfg_INTERFACE_DEFINED__
#define __IVsProjectFlavorCfg_INTERFACE_DEFINED__

/* interface IVsProjectFlavorCfg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorCfg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3bffc423-6c82-46c0-af2a-79a3ed3eda93")
    IVsProjectFlavorCfg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_CfgType( 
            /* [in] */ __RPC__in REFIID iidCfg,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppCfg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorCfgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFlavorCfg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFlavorCfg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFlavorCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_CfgType )( 
            IVsProjectFlavorCfg * This,
            /* [in] */ __RPC__in REFIID iidCfg,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppCfg);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsProjectFlavorCfg * This);
        
        END_INTERFACE
    } IVsProjectFlavorCfgVtbl;

    interface IVsProjectFlavorCfg
    {
        CONST_VTBL struct IVsProjectFlavorCfgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorCfg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorCfg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorCfg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorCfg_get_CfgType(This,iidCfg,ppCfg)	\
    ( (This)->lpVtbl -> get_CfgType(This,iidCfg,ppCfg) ) 

#define IVsProjectFlavorCfg_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorCfg_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorCfgOutputGroups_INTERFACE_DEFINED__
#define __IVsProjectFlavorCfgOutputGroups_INTERFACE_DEFINED__

/* interface IVsProjectFlavorCfgOutputGroups */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorCfgOutputGroups;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("52F50FAC-B245-4a81-9A02-DBF8F115389B")
    IVsProjectFlavorCfgOutputGroups : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CustomizeOutputGroup( 
            /* [in] */ __RPC__in_opt IVsOutputGroup *pIn,
            /* [out] */ __RPC__deref_out_opt IVsOutputGroup **pOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorCfgOutputGroupsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFlavorCfgOutputGroups * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFlavorCfgOutputGroups * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFlavorCfgOutputGroups * This);
        
        HRESULT ( STDMETHODCALLTYPE *CustomizeOutputGroup )( 
            IVsProjectFlavorCfgOutputGroups * This,
            /* [in] */ __RPC__in_opt IVsOutputGroup *pIn,
            /* [out] */ __RPC__deref_out_opt IVsOutputGroup **pOut);
        
        END_INTERFACE
    } IVsProjectFlavorCfgOutputGroupsVtbl;

    interface IVsProjectFlavorCfgOutputGroups
    {
        CONST_VTBL struct IVsProjectFlavorCfgOutputGroupsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorCfgOutputGroups_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorCfgOutputGroups_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorCfgOutputGroups_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorCfgOutputGroups_CustomizeOutputGroup(This,pIn,pOut)	\
    ( (This)->lpVtbl -> CustomizeOutputGroup(This,pIn,pOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorCfgOutputGroups_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorCfgProvider_INTERFACE_DEFINED__
#define __IVsProjectFlavorCfgProvider_INTERFACE_DEFINED__

/* interface IVsProjectFlavorCfgProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorCfgProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2b81d5f8-f8bd-4a65-8f51-f3bfcd51a924")
    IVsProjectFlavorCfgProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateProjectFlavorCfg( 
            /* [in] */ __RPC__in_opt IVsCfg *pBaseProjectCfg,
            /* [out] */ __RPC__deref_out_opt IVsProjectFlavorCfg **ppFlavorCfg) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorCfgProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFlavorCfgProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFlavorCfgProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFlavorCfgProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateProjectFlavorCfg )( 
            IVsProjectFlavorCfgProvider * This,
            /* [in] */ __RPC__in_opt IVsCfg *pBaseProjectCfg,
            /* [out] */ __RPC__deref_out_opt IVsProjectFlavorCfg **ppFlavorCfg);
        
        END_INTERFACE
    } IVsProjectFlavorCfgProviderVtbl;

    interface IVsProjectFlavorCfgProvider
    {
        CONST_VTBL struct IVsProjectFlavorCfgProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorCfgProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorCfgProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorCfgProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorCfgProvider_CreateProjectFlavorCfg(This,pBaseProjectCfg,ppFlavorCfg)	\
    ( (This)->lpVtbl -> CreateProjectFlavorCfg(This,pBaseProjectCfg,ppFlavorCfg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorCfgProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0046 */
/* [local] */ 


enum __UPDATE_REFERENCE_REASON
    {	URR_PROJECT_OPEN	= 0,
	URR_BUILD	= ( URR_PROJECT_OPEN + 1 ) ,
	URR_START_DEBUG	= ( URR_BUILD + 1 ) ,
	URR_REFERENCEPATH_CHANGED	= ( URR_START_DEBUG + 1 ) ,
	URR_REFERENCE_ADDED	= ( URR_REFERENCEPATH_CHANGED + 1 ) ,
	URR_REFERENCE_REMOVED	= ( URR_REFERENCE_ADDED + 1 ) ,
	URR_EXPLICIT_USER_ACTION	= ( URR_REFERENCE_REMOVED + 1 ) 
    } ;
typedef DWORD UPDATE_REFERENCE_REASON;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0046_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0046_v0_0_s_ifspec;

#ifndef __IVsProjectFlavorReferences_INTERFACE_DEFINED__
#define __IVsProjectFlavorReferences_INTERFACE_DEFINED__

/* interface IVsProjectFlavorReferences */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorReferences;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("66DB9803-1019-48ca-99F2-DAD69E49532C")
    IVsProjectFlavorReferences : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryAddProjectReference( 
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [retval][out] */ __RPC__out BOOL *pbCanAdd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryCanBeReferenced( 
            /* [in] */ __RPC__in_opt IUnknown *pReferencingProject,
            /* [retval][out] */ __RPC__out BOOL *pbAllowReferenced) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryRefreshReferences( 
            /* [in] */ UPDATE_REFERENCE_REASON reason,
            /* [retval][out] */ __RPC__out BOOL *pbUpdate) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFlavorReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFlavorReferences * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFlavorReferences * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryAddProjectReference )( 
            IVsProjectFlavorReferences * This,
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [retval][out] */ __RPC__out BOOL *pbCanAdd);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCanBeReferenced )( 
            IVsProjectFlavorReferences * This,
            /* [in] */ __RPC__in_opt IUnknown *pReferencingProject,
            /* [retval][out] */ __RPC__out BOOL *pbAllowReferenced);
        
        HRESULT ( STDMETHODCALLTYPE *QueryRefreshReferences )( 
            IVsProjectFlavorReferences * This,
            /* [in] */ UPDATE_REFERENCE_REASON reason,
            /* [retval][out] */ __RPC__out BOOL *pbUpdate);
        
        END_INTERFACE
    } IVsProjectFlavorReferencesVtbl;

    interface IVsProjectFlavorReferences
    {
        CONST_VTBL struct IVsProjectFlavorReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorReferences_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorReferences_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorReferences_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorReferences_QueryAddProjectReference(This,pReferencedProject,pbCanAdd)	\
    ( (This)->lpVtbl -> QueryAddProjectReference(This,pReferencedProject,pbCanAdd) ) 

#define IVsProjectFlavorReferences_QueryCanBeReferenced(This,pReferencingProject,pbAllowReferenced)	\
    ( (This)->lpVtbl -> QueryCanBeReferenced(This,pReferencingProject,pbAllowReferenced) ) 

#define IVsProjectFlavorReferences_QueryRefreshReferences(This,reason,pbUpdate)	\
    ( (This)->lpVtbl -> QueryRefreshReferences(This,reason,pbUpdate) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorReferences_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0047 */
/* [local] */ 


enum __VSTRANSACCELEXFLAGS
    {	VSTAEXF_Default	= 0,
	VSTAEXF_NoFireCommand	= 0x1,
	VSTAEXF_IgnoreActiveKBScopes	= 0x2,
	VSTAEXF_UseTextEditorKBScope	= 0x4,
	VSTAEXF_UseGlobalKBScope	= 0x8,
	VSTAEXF_AllowModalState	= 0x10
    } ;
typedef DWORD VSTRANSACCELEXFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0047_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0047_v0_0_s_ifspec;

#ifndef __IVsFilterKeys2_INTERFACE_DEFINED__
#define __IVsFilterKeys2_INTERFACE_DEFINED__

/* interface IVsFilterKeys2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFilterKeys2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A1FF0D7C-1F51-4a95-B107-EC6FFE2C5794")
    IVsFilterKeys2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE TranslateAcceleratorEx( 
            /* [in] */ __RPC__in LPMSG pMsg,
            /* [in] */ VSTRANSACCELEXFLAGS dwFlags,
            /* [in] */ DWORD cKeyBindingScopes,
            /* [size_is][in] */ __RPC__in_ecount_full(cKeyBindingScopes) const GUID rgguidKeyBindingScopes[  ],
            /* [out] */ __RPC__out GUID *pguidCmd,
            /* [out] */ __RPC__out DWORD *pdwCmd,
            /* [out] */ __RPC__out BOOL *fCmdTranslated,
            /* [out] */ __RPC__out BOOL *pfKeyComboStartsChord) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFilterKeys2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFilterKeys2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFilterKeys2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFilterKeys2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateAcceleratorEx )( 
            IVsFilterKeys2 * This,
            /* [in] */ __RPC__in LPMSG pMsg,
            /* [in] */ VSTRANSACCELEXFLAGS dwFlags,
            /* [in] */ DWORD cKeyBindingScopes,
            /* [size_is][in] */ __RPC__in_ecount_full(cKeyBindingScopes) const GUID rgguidKeyBindingScopes[  ],
            /* [out] */ __RPC__out GUID *pguidCmd,
            /* [out] */ __RPC__out DWORD *pdwCmd,
            /* [out] */ __RPC__out BOOL *fCmdTranslated,
            /* [out] */ __RPC__out BOOL *pfKeyComboStartsChord);
        
        END_INTERFACE
    } IVsFilterKeys2Vtbl;

    interface IVsFilterKeys2
    {
        CONST_VTBL struct IVsFilterKeys2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFilterKeys2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFilterKeys2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFilterKeys2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFilterKeys2_TranslateAcceleratorEx(This,pMsg,dwFlags,cKeyBindingScopes,rgguidKeyBindingScopes,pguidCmd,pdwCmd,fCmdTranslated,pfKeyComboStartsChord)	\
    ( (This)->lpVtbl -> TranslateAcceleratorEx(This,pMsg,dwFlags,cKeyBindingScopes,rgguidKeyBindingScopes,pguidCmd,pdwCmd,fCmdTranslated,pfKeyComboStartsChord) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFilterKeys2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0048 */
/* [local] */ 


enum __UserSettingsFlags
    {	USF_None	= 0,
	USF_ResetOnImport	= 0x1,
	USF_DisableOptimizations	= 0x2
    } ;
typedef DWORD UserSettingsFlags;


enum __VSSETTINGSERRORTYPES
    {	vsSettingsErrorTypeSuccess	= 0,
	vsSettingsErrorTypeError	= 0x1,
	vsSettingsErrorTypeWarning	= 0x2,
	vsSettingsErrorTypeRestart	= 0x4,
	vsSettingsErrorTypeNotInstalled	= 0x8,
	vsSettingsErrorTypeMask	= 0xf
    } ;
typedef DWORD VSSETTINGSERRORTYPES;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0048_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0048_v0_0_s_ifspec;

#ifndef __IVsSettingsReader_INTERFACE_DEFINED__
#define __IVsSettingsReader_INTERFACE_DEFINED__

/* interface IVsSettingsReader */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSettingsReader;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("38C38501-1428-4abb-8B27-2F0E1E6DD757")
    IVsSettingsReader : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReadSettingString( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingLong( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__out long *plSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingBoolean( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__out BOOL *pfSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingBytes( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [out][in] */ __RPC__inout BYTE *pSettingValue,
            /* [out] */ __RPC__out long *plDataLength,
            /* [in] */ long lDataMax) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingAttribute( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingXml( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppIXMLDOMNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadSettingXmlAsString( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXML) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadCategoryVersion( 
            /* [out] */ __RPC__out int *pnMajor,
            /* [out] */ __RPC__out int *pnMinor,
            /* [out] */ __RPC__out int *pnBuild,
            /* [out] */ __RPC__out int *pnRevision) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadFileVersion( 
            /* [out] */ __RPC__out int *pnMajor,
            /* [out] */ __RPC__out int *pnMinor,
            /* [out] */ __RPC__out int *pnBuild,
            /* [out] */ __RPC__out int *pnRevision) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReportError( 
            /* [in] */ __RPC__in LPCOLESTR pszError,
            VSSETTINGSERRORTYPES dwErrorType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSettingsReaderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSettingsReader * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSettingsReader * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingString )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingLong )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__out long *plSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingBoolean )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__out BOOL *pfSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingBytes )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [out][in] */ __RPC__inout BYTE *pSettingValue,
            /* [out] */ __RPC__out long *plDataLength,
            /* [in] */ long lDataMax);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingAttribute )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingXml )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppIXMLDOMNode);
        
        HRESULT ( STDMETHODCALLTYPE *ReadSettingXmlAsString )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXML);
        
        HRESULT ( STDMETHODCALLTYPE *ReadCategoryVersion )( 
            IVsSettingsReader * This,
            /* [out] */ __RPC__out int *pnMajor,
            /* [out] */ __RPC__out int *pnMinor,
            /* [out] */ __RPC__out int *pnBuild,
            /* [out] */ __RPC__out int *pnRevision);
        
        HRESULT ( STDMETHODCALLTYPE *ReadFileVersion )( 
            IVsSettingsReader * This,
            /* [out] */ __RPC__out int *pnMajor,
            /* [out] */ __RPC__out int *pnMinor,
            /* [out] */ __RPC__out int *pnBuild,
            /* [out] */ __RPC__out int *pnRevision);
        
        HRESULT ( STDMETHODCALLTYPE *ReportError )( 
            IVsSettingsReader * This,
            /* [in] */ __RPC__in LPCOLESTR pszError,
            VSSETTINGSERRORTYPES dwErrorType);
        
        END_INTERFACE
    } IVsSettingsReaderVtbl;

    interface IVsSettingsReader
    {
        CONST_VTBL struct IVsSettingsReaderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSettingsReader_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSettingsReader_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSettingsReader_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSettingsReader_ReadSettingString(This,pszSettingName,pbstrSettingValue)	\
    ( (This)->lpVtbl -> ReadSettingString(This,pszSettingName,pbstrSettingValue) ) 

#define IVsSettingsReader_ReadSettingLong(This,pszSettingName,plSettingValue)	\
    ( (This)->lpVtbl -> ReadSettingLong(This,pszSettingName,plSettingValue) ) 

#define IVsSettingsReader_ReadSettingBoolean(This,pszSettingName,pfSettingValue)	\
    ( (This)->lpVtbl -> ReadSettingBoolean(This,pszSettingName,pfSettingValue) ) 

#define IVsSettingsReader_ReadSettingBytes(This,pszSettingName,pSettingValue,plDataLength,lDataMax)	\
    ( (This)->lpVtbl -> ReadSettingBytes(This,pszSettingName,pSettingValue,plDataLength,lDataMax) ) 

#define IVsSettingsReader_ReadSettingAttribute(This,pszSettingName,pszAttributeName,pbstrSettingValue)	\
    ( (This)->lpVtbl -> ReadSettingAttribute(This,pszSettingName,pszAttributeName,pbstrSettingValue) ) 

#define IVsSettingsReader_ReadSettingXml(This,pszSettingName,ppIXMLDOMNode)	\
    ( (This)->lpVtbl -> ReadSettingXml(This,pszSettingName,ppIXMLDOMNode) ) 

#define IVsSettingsReader_ReadSettingXmlAsString(This,pszSettingName,pbstrXML)	\
    ( (This)->lpVtbl -> ReadSettingXmlAsString(This,pszSettingName,pbstrXML) ) 

#define IVsSettingsReader_ReadCategoryVersion(This,pnMajor,pnMinor,pnBuild,pnRevision)	\
    ( (This)->lpVtbl -> ReadCategoryVersion(This,pnMajor,pnMinor,pnBuild,pnRevision) ) 

#define IVsSettingsReader_ReadFileVersion(This,pnMajor,pnMinor,pnBuild,pnRevision)	\
    ( (This)->lpVtbl -> ReadFileVersion(This,pnMajor,pnMinor,pnBuild,pnRevision) ) 

#define IVsSettingsReader_ReportError(This,pszError,dwErrorType)	\
    ( (This)->lpVtbl -> ReportError(This,pszError,dwErrorType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSettingsReader_INTERFACE_DEFINED__ */


#ifndef __IVsSettingsWriter_INTERFACE_DEFINED__
#define __IVsSettingsWriter_INTERFACE_DEFINED__

/* interface IVsSettingsWriter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSettingsWriter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F1CF980-AFC6-406e-958D-7F24287E3916")
    IVsSettingsWriter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WriteSettingString( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingLong( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ long lSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingBoolean( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ BOOL fSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingBytes( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [size_is][in] */ __RPC__in_ecount_full(lDataLength) BYTE *pSettingValue,
            long lDataLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingAttribute( 
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [in] */ __RPC__in LPCOLESTR pszSettingValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingXml( 
            /* [in] */ __RPC__in_opt IUnknown *pIXMLDOMNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteSettingXmlFromString( 
            /* [in] */ __RPC__in LPCOLESTR szXML) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteCategoryVersion( 
            /* [in] */ int nMajor,
            /* [in] */ int nMinor,
            /* [in] */ int nBuild,
            /* [in] */ int nRevision) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReportError( 
            /* [in] */ __RPC__in LPCOLESTR pszError,
            /* [in] */ VSSETTINGSERRORTYPES dwErrorType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSettingsWriterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSettingsWriter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSettingsWriter * This);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingString )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingLong )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ long lSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingBoolean )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ BOOL fSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingBytes )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [size_is][in] */ __RPC__in_ecount_full(lDataLength) BYTE *pSettingValue,
            long lDataLength);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingAttribute )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszSettingName,
            /* [in] */ __RPC__in LPCOLESTR pszAttributeName,
            /* [in] */ __RPC__in LPCOLESTR pszSettingValue);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingXml )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in_opt IUnknown *pIXMLDOMNode);
        
        HRESULT ( STDMETHODCALLTYPE *WriteSettingXmlFromString )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR szXML);
        
        HRESULT ( STDMETHODCALLTYPE *WriteCategoryVersion )( 
            IVsSettingsWriter * This,
            /* [in] */ int nMajor,
            /* [in] */ int nMinor,
            /* [in] */ int nBuild,
            /* [in] */ int nRevision);
        
        HRESULT ( STDMETHODCALLTYPE *ReportError )( 
            IVsSettingsWriter * This,
            /* [in] */ __RPC__in LPCOLESTR pszError,
            /* [in] */ VSSETTINGSERRORTYPES dwErrorType);
        
        END_INTERFACE
    } IVsSettingsWriterVtbl;

    interface IVsSettingsWriter
    {
        CONST_VTBL struct IVsSettingsWriterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSettingsWriter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSettingsWriter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSettingsWriter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSettingsWriter_WriteSettingString(This,pszSettingName,pszSettingValue)	\
    ( (This)->lpVtbl -> WriteSettingString(This,pszSettingName,pszSettingValue) ) 

#define IVsSettingsWriter_WriteSettingLong(This,pszSettingName,lSettingValue)	\
    ( (This)->lpVtbl -> WriteSettingLong(This,pszSettingName,lSettingValue) ) 

#define IVsSettingsWriter_WriteSettingBoolean(This,pszSettingName,fSettingValue)	\
    ( (This)->lpVtbl -> WriteSettingBoolean(This,pszSettingName,fSettingValue) ) 

#define IVsSettingsWriter_WriteSettingBytes(This,pszSettingName,pSettingValue,lDataLength)	\
    ( (This)->lpVtbl -> WriteSettingBytes(This,pszSettingName,pSettingValue,lDataLength) ) 

#define IVsSettingsWriter_WriteSettingAttribute(This,pszSettingName,pszAttributeName,pszSettingValue)	\
    ( (This)->lpVtbl -> WriteSettingAttribute(This,pszSettingName,pszAttributeName,pszSettingValue) ) 

#define IVsSettingsWriter_WriteSettingXml(This,pIXMLDOMNode)	\
    ( (This)->lpVtbl -> WriteSettingXml(This,pIXMLDOMNode) ) 

#define IVsSettingsWriter_WriteSettingXmlFromString(This,szXML)	\
    ( (This)->lpVtbl -> WriteSettingXmlFromString(This,szXML) ) 

#define IVsSettingsWriter_WriteCategoryVersion(This,nMajor,nMinor,nBuild,nRevision)	\
    ( (This)->lpVtbl -> WriteCategoryVersion(This,nMajor,nMinor,nBuild,nRevision) ) 

#define IVsSettingsWriter_ReportError(This,pszError,dwErrorType)	\
    ( (This)->lpVtbl -> ReportError(This,pszError,dwErrorType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSettingsWriter_INTERFACE_DEFINED__ */


#ifndef __IVsUserSettings_INTERFACE_DEFINED__
#define __IVsUserSettings_INTERFACE_DEFINED__

/* interface IVsUserSettings */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUserSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("770E285D-3B7D-4342-B3C4-42BD9F53A300")
    IVsUserSettings : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExportSettings( 
            /* [in] */ __RPC__in LPCOLESTR pszCategoryGUID,
            /* [in] */ __RPC__in_opt IVsSettingsWriter *pSettings) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ImportSettings( 
            /* [in] */ __RPC__in LPCOLESTR pszCategoryGUID,
            /* [in] */ __RPC__in_opt IVsSettingsReader *pSettings,
            /* [in] */ UserSettingsFlags flags,
            /* [out][in] */ __RPC__inout BOOL *pfRestartRequired) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserSettings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserSettings * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExportSettings )( 
            IVsUserSettings * This,
            /* [in] */ __RPC__in LPCOLESTR pszCategoryGUID,
            /* [in] */ __RPC__in_opt IVsSettingsWriter *pSettings);
        
        HRESULT ( STDMETHODCALLTYPE *ImportSettings )( 
            IVsUserSettings * This,
            /* [in] */ __RPC__in LPCOLESTR pszCategoryGUID,
            /* [in] */ __RPC__in_opt IVsSettingsReader *pSettings,
            /* [in] */ UserSettingsFlags flags,
            /* [out][in] */ __RPC__inout BOOL *pfRestartRequired);
        
        END_INTERFACE
    } IVsUserSettingsVtbl;

    interface IVsUserSettings
    {
        CONST_VTBL struct IVsUserSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserSettings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserSettings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserSettings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserSettings_ExportSettings(This,pszCategoryGUID,pSettings)	\
    ( (This)->lpVtbl -> ExportSettings(This,pszCategoryGUID,pSettings) ) 

#define IVsUserSettings_ImportSettings(This,pszCategoryGUID,pSettings,flags,pfRestartRequired)	\
    ( (This)->lpVtbl -> ImportSettings(This,pszCategoryGUID,pSettings,flags,pfRestartRequired) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserSettings_INTERFACE_DEFINED__ */


#ifndef __SVsSettingsReader_INTERFACE_DEFINED__
#define __SVsSettingsReader_INTERFACE_DEFINED__

/* interface SVsSettingsReader */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSettingsReader;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7C1BD0D6-2086-46ab-8F07-B9335D0FE7D8")
    SVsSettingsReader : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSettingsReaderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSettingsReader * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSettingsReader * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSettingsReader * This);
        
        END_INTERFACE
    } SVsSettingsReaderVtbl;

    interface SVsSettingsReader
    {
        CONST_VTBL struct SVsSettingsReaderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSettingsReader_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSettingsReader_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSettingsReader_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSettingsReader_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0052 */
/* [local] */ 

#define SID_SVsSettingsReader IID_SVsSettingsReader


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0052_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0052_v0_0_s_ifspec;

#ifndef __IVsUserSettingsQuery_INTERFACE_DEFINED__
#define __IVsUserSettingsQuery_INTERFACE_DEFINED__

/* interface IVsUserSettingsQuery */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUserSettingsQuery;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("334E1F15-7D97-4231-81B0-998E4A960E69")
    IVsUserSettingsQuery : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE NeedExport( 
            /* [in] */ __RPC__in LPCOLESTR szCategoryGUID,
            /* [out] */ __RPC__out BOOL *pfNeedExport) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUserSettingsQueryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUserSettingsQuery * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUserSettingsQuery * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUserSettingsQuery * This);
        
        HRESULT ( STDMETHODCALLTYPE *NeedExport )( 
            IVsUserSettingsQuery * This,
            /* [in] */ __RPC__in LPCOLESTR szCategoryGUID,
            /* [out] */ __RPC__out BOOL *pfNeedExport);
        
        END_INTERFACE
    } IVsUserSettingsQueryVtbl;

    interface IVsUserSettingsQuery
    {
        CONST_VTBL struct IVsUserSettingsQueryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUserSettingsQuery_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUserSettingsQuery_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUserSettingsQuery_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUserSettingsQuery_NeedExport(This,szCategoryGUID,pfNeedExport)	\
    ( (This)->lpVtbl -> NeedExport(This,szCategoryGUID,pfNeedExport) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUserSettingsQuery_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0053 */
/* [local] */ 







enum __VSPROFILELOCATIONS
    {	PFL_LocationNone	= 0,
	PFL_InstallDir	= 0x1,
	PFL_SettingsDir	= 0x2,
	PFL_Other	= 0x4,
	PFL_AutoSave	= 0x8,
	PFL_All	= 0xffffffff
    } ;
typedef DWORD VSPROFILELOCATIONS;


enum __VSPROFILETEAMSETTINGSFLAGS
    {	PTSF_CHECKFORUPDATE	= 0,
	PTSF_UPDATEALWAYS	= 0x1
    } ;
typedef DWORD VSPROFILETEAMSETTINGSFLAGS;


enum __VSPROFILETEAMSETTINGSCHANGEDFLAGS
    {	PTSCF_TEAMFILE_NOCHANGE	= 0,
	PTSCF_TEAMFILE_CHANGED	= 0x1,
	PTSCF_TEAMFILE_SAME	= 0x2
    } ;
typedef DWORD VSPROFILETEAMSETTINGSCHANGEDFLAGS;


enum __VSPROFILECATEGORYSECURITY
    {	PCSEC_SAFE	= 0,
	PCSEC_THREAT_VS	= 0x1,
	PCSEC_THREAT_MACHINE	= 0x2
    } ;
typedef DWORD VSPROFILECATEGORYSECURITY;


enum __VSPROFILECATEGORYSENSITIVITY
    {	PCSEN_SAFE	= 0,
	PCSEN_PRIVACY	= 0x1
    } ;
typedef DWORD VSPROFILECATEGORYSENSITIVITY;


enum __VSPROFILEGETFILENAME
    {	PGFN_EXPORT	= 0x1,
	PGFN_SAVECURRENT	= 0x2,
	PGFN_AUTOSAVE	= 0x4
    } ;
typedef DWORD VSPROFILEGETFILENAME;


enum __VSSETTINGSCOMPLETIONSTATUS
    {	vsSettingsCompletionStatusNotStarted	= 0,
	vsSettingsCompletionStatusIncomplete	= 0x1,
	vsSettingsCompletionStatusComplete	= 0x2,
	vsSettingsCompletionStatusStateMask	= 0xf,
	vsSettingsCompletionStatusSuccess	= 0,
	vsSettingsCompletionStatusWarnings	= 0x10,
	vsSettingsCompletionStatusErrors	= 0x20,
	vsSettingsCompletionStatusSuccessMask	= 0xf0
    } ;
typedef DWORD VSSETTINGSCOMPLETIONSTATUS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0053_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0053_v0_0_s_ifspec;

#ifndef __IVsProfileSettingsFileInfo_INTERFACE_DEFINED__
#define __IVsProfileSettingsFileInfo_INTERFACE_DEFINED__

/* interface IVsProfileSettingsFileInfo */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfileSettingsFileInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8E8E55A9-4111-4808-A0D0-7F067FB3A62F")
    IVsProfileSettingsFileInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFilePath( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileLocation( 
            /* [out] */ __RPC__out VSPROFILELOCATIONS *pfileLocation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFriendlyName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFriendlyName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSettingsForImport( 
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppSettingsTree) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfileSettingsFileInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfileSettingsFileInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfileSettingsFileInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfileSettingsFileInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilePath )( 
            IVsProfileSettingsFileInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileLocation )( 
            IVsProfileSettingsFileInfo * This,
            /* [out] */ __RPC__out VSPROFILELOCATIONS *pfileLocation);
        
        HRESULT ( STDMETHODCALLTYPE *GetFriendlyName )( 
            IVsProfileSettingsFileInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFriendlyName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescription )( 
            IVsProfileSettingsFileInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetSettingsForImport )( 
            IVsProfileSettingsFileInfo * This,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppSettingsTree);
        
        END_INTERFACE
    } IVsProfileSettingsFileInfoVtbl;

    interface IVsProfileSettingsFileInfo
    {
        CONST_VTBL struct IVsProfileSettingsFileInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfileSettingsFileInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfileSettingsFileInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfileSettingsFileInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfileSettingsFileInfo_GetFilePath(This,pbstrFilePath)	\
    ( (This)->lpVtbl -> GetFilePath(This,pbstrFilePath) ) 

#define IVsProfileSettingsFileInfo_GetFileLocation(This,pfileLocation)	\
    ( (This)->lpVtbl -> GetFileLocation(This,pfileLocation) ) 

#define IVsProfileSettingsFileInfo_GetFriendlyName(This,pbstrFriendlyName)	\
    ( (This)->lpVtbl -> GetFriendlyName(This,pbstrFriendlyName) ) 

#define IVsProfileSettingsFileInfo_GetDescription(This,pbstrDescription)	\
    ( (This)->lpVtbl -> GetDescription(This,pbstrDescription) ) 

#define IVsProfileSettingsFileInfo_GetSettingsForImport(This,ppSettingsTree)	\
    ( (This)->lpVtbl -> GetSettingsForImport(This,ppSettingsTree) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfileSettingsFileInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfileSettingsFileCollection_INTERFACE_DEFINED__
#define __IVsProfileSettingsFileCollection_INTERFACE_DEFINED__

/* interface IVsProfileSettingsFileCollection */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfileSettingsFileCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0FAF274A-3898-445a-822F-7D42927EFEF9")
    IVsProfileSettingsFileCollection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSettingsFile( 
            /* [in] */ int index,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileInfo **ppFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddBrowseFile( 
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileInfo **ppFileInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfileSettingsFileCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfileSettingsFileCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfileSettingsFileCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfileSettingsFileCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsProfileSettingsFileCollection * This,
            /* [out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSettingsFile )( 
            IVsProfileSettingsFileCollection * This,
            /* [in] */ int index,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileInfo **ppFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *AddBrowseFile )( 
            IVsProfileSettingsFileCollection * This,
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileInfo **ppFileInfo);
        
        END_INTERFACE
    } IVsProfileSettingsFileCollectionVtbl;

    interface IVsProfileSettingsFileCollection
    {
        CONST_VTBL struct IVsProfileSettingsFileCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfileSettingsFileCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfileSettingsFileCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfileSettingsFileCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfileSettingsFileCollection_GetCount(This,pCount)	\
    ( (This)->lpVtbl -> GetCount(This,pCount) ) 

#define IVsProfileSettingsFileCollection_GetSettingsFile(This,index,ppFileInfo)	\
    ( (This)->lpVtbl -> GetSettingsFile(This,index,ppFileInfo) ) 

#define IVsProfileSettingsFileCollection_AddBrowseFile(This,bstrFilePath,ppFileInfo)	\
    ( (This)->lpVtbl -> AddBrowseFile(This,bstrFilePath,ppFileInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfileSettingsFileCollection_INTERFACE_DEFINED__ */


#ifndef __IVsProfileSettingsTree_INTERFACE_DEFINED__
#define __IVsProfileSettingsTree_INTERFACE_DEFINED__

/* interface IVsProfileSettingsTree */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfileSettingsTree;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("23B6FED1-C3CB-4006-BAD0-64A7EB61DF39")
    IVsProfileSettingsTree : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetChildCount( 
            /* [out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetChild( 
            /* [in] */ int index,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppChildTree) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnabledChildCount( 
            /* [out] */ __RPC__out int *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCategory( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCategory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRegisteredName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRegisteredName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNameForID( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNameForID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFullPath( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFullPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPackage( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPackage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIsAutomationPropBased( 
            /* [out] */ __RPC__out BOOL *pfAutoProp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnabled( 
            /* [out] */ __RPC__out BOOL *pfEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetEnabled( 
            /* [in] */ BOOL fEnabled,
            /* [in] */ BOOL fIncludeChildren) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVisible( 
            /* [out] */ __RPC__out BOOL *pfVisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAlternatePath( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAlternatePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIsPlaceholder( 
            /* [out] */ __RPC__out BOOL *pfPlaceholder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRepresentedNode( 
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppRepresentedNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSecurityLevel( 
            /* [out] */ __RPC__out VSPROFILECATEGORYSECURITY *pSecurityLevel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSensitivityLevel( 
            /* [out] */ __RPC__out VSPROFILECATEGORYSENSITIVITY *pSensitivityLevel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindChildTree( 
            /* [in] */ __RPC__in BSTR bstrNameSearch,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppChildTree) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddChildTree( 
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pChildTree) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RevisePlacements( 
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pTreeRoot,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pTreeRootBasis,
            /* [in] */ __RPC__in BSTR bstrCurrentParent) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfileSettingsTreeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfileSettingsTree * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfileSettingsTree * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfileSettingsTree * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetChildCount )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetChild )( 
            IVsProfileSettingsTree * This,
            /* [in] */ int index,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppChildTree);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnabledChildCount )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out int *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescription )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetCategory )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCategory);
        
        HRESULT ( STDMETHODCALLTYPE *GetRegisteredName )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRegisteredName);
        
        HRESULT ( STDMETHODCALLTYPE *GetNameForID )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrNameForID);
        
        HRESULT ( STDMETHODCALLTYPE *GetFullPath )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFullPath);
        
        HRESULT ( STDMETHODCALLTYPE *GetPackage )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPackage);
        
        HRESULT ( STDMETHODCALLTYPE *GetIsAutomationPropBased )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out BOOL *pfAutoProp);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnabled )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out BOOL *pfEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *SetEnabled )( 
            IVsProfileSettingsTree * This,
            /* [in] */ BOOL fEnabled,
            /* [in] */ BOOL fIncludeChildren);
        
        HRESULT ( STDMETHODCALLTYPE *GetVisible )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out BOOL *pfVisible);
        
        HRESULT ( STDMETHODCALLTYPE *GetAlternatePath )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAlternatePath);
        
        HRESULT ( STDMETHODCALLTYPE *GetIsPlaceholder )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out BOOL *pfPlaceholder);
        
        HRESULT ( STDMETHODCALLTYPE *GetRepresentedNode )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppRepresentedNode);
        
        HRESULT ( STDMETHODCALLTYPE *GetSecurityLevel )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out VSPROFILECATEGORYSECURITY *pSecurityLevel);
        
        HRESULT ( STDMETHODCALLTYPE *GetSensitivityLevel )( 
            IVsProfileSettingsTree * This,
            /* [out] */ __RPC__out VSPROFILECATEGORYSENSITIVITY *pSensitivityLevel);
        
        HRESULT ( STDMETHODCALLTYPE *FindChildTree )( 
            IVsProfileSettingsTree * This,
            /* [in] */ __RPC__in BSTR bstrNameSearch,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppChildTree);
        
        HRESULT ( STDMETHODCALLTYPE *AddChildTree )( 
            IVsProfileSettingsTree * This,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pChildTree);
        
        HRESULT ( STDMETHODCALLTYPE *RevisePlacements )( 
            IVsProfileSettingsTree * This,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pTreeRoot,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pTreeRootBasis,
            /* [in] */ __RPC__in BSTR bstrCurrentParent);
        
        END_INTERFACE
    } IVsProfileSettingsTreeVtbl;

    interface IVsProfileSettingsTree
    {
        CONST_VTBL struct IVsProfileSettingsTreeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfileSettingsTree_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfileSettingsTree_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfileSettingsTree_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfileSettingsTree_GetChildCount(This,pCount)	\
    ( (This)->lpVtbl -> GetChildCount(This,pCount) ) 

#define IVsProfileSettingsTree_GetChild(This,index,ppChildTree)	\
    ( (This)->lpVtbl -> GetChild(This,index,ppChildTree) ) 

#define IVsProfileSettingsTree_GetEnabledChildCount(This,pCount)	\
    ( (This)->lpVtbl -> GetEnabledChildCount(This,pCount) ) 

#define IVsProfileSettingsTree_GetDisplayName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pbstrName) ) 

#define IVsProfileSettingsTree_GetDescription(This,pbstrDescription)	\
    ( (This)->lpVtbl -> GetDescription(This,pbstrDescription) ) 

#define IVsProfileSettingsTree_GetCategory(This,pbstrCategory)	\
    ( (This)->lpVtbl -> GetCategory(This,pbstrCategory) ) 

#define IVsProfileSettingsTree_GetRegisteredName(This,pbstrRegisteredName)	\
    ( (This)->lpVtbl -> GetRegisteredName(This,pbstrRegisteredName) ) 

#define IVsProfileSettingsTree_GetNameForID(This,pbstrNameForID)	\
    ( (This)->lpVtbl -> GetNameForID(This,pbstrNameForID) ) 

#define IVsProfileSettingsTree_GetFullPath(This,pbstrFullPath)	\
    ( (This)->lpVtbl -> GetFullPath(This,pbstrFullPath) ) 

#define IVsProfileSettingsTree_GetPackage(This,pbstrPackage)	\
    ( (This)->lpVtbl -> GetPackage(This,pbstrPackage) ) 

#define IVsProfileSettingsTree_GetIsAutomationPropBased(This,pfAutoProp)	\
    ( (This)->lpVtbl -> GetIsAutomationPropBased(This,pfAutoProp) ) 

#define IVsProfileSettingsTree_GetEnabled(This,pfEnabled)	\
    ( (This)->lpVtbl -> GetEnabled(This,pfEnabled) ) 

#define IVsProfileSettingsTree_SetEnabled(This,fEnabled,fIncludeChildren)	\
    ( (This)->lpVtbl -> SetEnabled(This,fEnabled,fIncludeChildren) ) 

#define IVsProfileSettingsTree_GetVisible(This,pfVisible)	\
    ( (This)->lpVtbl -> GetVisible(This,pfVisible) ) 

#define IVsProfileSettingsTree_GetAlternatePath(This,pbstrAlternatePath)	\
    ( (This)->lpVtbl -> GetAlternatePath(This,pbstrAlternatePath) ) 

#define IVsProfileSettingsTree_GetIsPlaceholder(This,pfPlaceholder)	\
    ( (This)->lpVtbl -> GetIsPlaceholder(This,pfPlaceholder) ) 

#define IVsProfileSettingsTree_GetRepresentedNode(This,ppRepresentedNode)	\
    ( (This)->lpVtbl -> GetRepresentedNode(This,ppRepresentedNode) ) 

#define IVsProfileSettingsTree_GetSecurityLevel(This,pSecurityLevel)	\
    ( (This)->lpVtbl -> GetSecurityLevel(This,pSecurityLevel) ) 

#define IVsProfileSettingsTree_GetSensitivityLevel(This,pSensitivityLevel)	\
    ( (This)->lpVtbl -> GetSensitivityLevel(This,pSensitivityLevel) ) 

#define IVsProfileSettingsTree_FindChildTree(This,bstrNameSearch,ppChildTree)	\
    ( (This)->lpVtbl -> FindChildTree(This,bstrNameSearch,ppChildTree) ) 

#define IVsProfileSettingsTree_AddChildTree(This,pChildTree)	\
    ( (This)->lpVtbl -> AddChildTree(This,pChildTree) ) 

#define IVsProfileSettingsTree_RevisePlacements(This,pTreeRoot,pTreeRootBasis,bstrCurrentParent)	\
    ( (This)->lpVtbl -> RevisePlacements(This,pTreeRoot,pTreeRootBasis,bstrCurrentParent) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfileSettingsTree_INTERFACE_DEFINED__ */


#ifndef __IVsSettingsErrorInformation_INTERFACE_DEFINED__
#define __IVsSettingsErrorInformation_INTERFACE_DEFINED__

/* interface IVsSettingsErrorInformation */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSettingsErrorInformation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33D90D1C-2665-4eec-9194-A79AFD63275F")
    IVsSettingsErrorInformation : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCompletionStatus( 
            /* [retval][out] */ __RPC__out VSSETTINGSCOMPLETIONSTATUS *pdwCompletionStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetErrorCount( 
            /* [retval][out] */ __RPC__out int *pnErrors) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetErrorInfo( 
            /* [in] */ int nErrorIndex,
            /* [out] */ __RPC__out VSSETTINGSERRORTYPES *pdwErrorType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSettingsErrorInformationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSettingsErrorInformation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSettingsErrorInformation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSettingsErrorInformation * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompletionStatus )( 
            IVsSettingsErrorInformation * This,
            /* [retval][out] */ __RPC__out VSSETTINGSCOMPLETIONSTATUS *pdwCompletionStatus);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorCount )( 
            IVsSettingsErrorInformation * This,
            /* [retval][out] */ __RPC__out int *pnErrors);
        
        HRESULT ( STDMETHODCALLTYPE *GetErrorInfo )( 
            IVsSettingsErrorInformation * This,
            /* [in] */ int nErrorIndex,
            /* [out] */ __RPC__out VSSETTINGSERRORTYPES *pdwErrorType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrError);
        
        END_INTERFACE
    } IVsSettingsErrorInformationVtbl;

    interface IVsSettingsErrorInformation
    {
        CONST_VTBL struct IVsSettingsErrorInformationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSettingsErrorInformation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSettingsErrorInformation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSettingsErrorInformation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSettingsErrorInformation_GetCompletionStatus(This,pdwCompletionStatus)	\
    ( (This)->lpVtbl -> GetCompletionStatus(This,pdwCompletionStatus) ) 

#define IVsSettingsErrorInformation_GetErrorCount(This,pnErrors)	\
    ( (This)->lpVtbl -> GetErrorCount(This,pnErrors) ) 

#define IVsSettingsErrorInformation_GetErrorInfo(This,nErrorIndex,pdwErrorType,pbstrError)	\
    ( (This)->lpVtbl -> GetErrorInfo(This,nErrorIndex,pdwErrorType,pbstrError) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSettingsErrorInformation_INTERFACE_DEFINED__ */


#ifndef __IVsProfileDataManager_INTERFACE_DEFINED__
#define __IVsProfileDataManager_INTERFACE_DEFINED__

/* interface IVsProfileDataManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfileDataManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("466EFAF6-F832-4079-83CD-4BBB02719C1D")
    IVsProfileDataManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LastResetPoint( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrResetFilename) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSettingsFiles( 
            /* [in] */ VSPROFILELOCATIONS fileLocations,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileCollection **ppCollection) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultSettingsLocation( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSettingsLocation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUniqueExportFileName( 
            /* [in] */ VSPROFILEGETFILENAME flags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExportFile) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSettingsFileExtension( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSettingsFileExtension) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSettingsForExport( 
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppSettingsTree) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExportSettings( 
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pSettingsTree,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ImportSettings( 
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pSettingsTree,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetSettings( 
            /* [in] */ __RPC__in_opt IVsProfileSettingsFileInfo *pFileInfo,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExportAllSettings( 
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AutoSaveAllSettings( 
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckUpdateTeamSettings( 
            /* [in] */ VSPROFILETEAMSETTINGSFLAGS dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReportTeamSettingsChanged( 
            /* [in] */ VSPROFILETEAMSETTINGSCHANGEDFLAGS dwFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowProfilesUI( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfileDataManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfileDataManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfileDataManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfileDataManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *LastResetPoint )( 
            IVsProfileDataManager * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrResetFilename);
        
        HRESULT ( STDMETHODCALLTYPE *GetSettingsFiles )( 
            IVsProfileDataManager * This,
            /* [in] */ VSPROFILELOCATIONS fileLocations,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsFileCollection **ppCollection);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultSettingsLocation )( 
            IVsProfileDataManager * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSettingsLocation);
        
        HRESULT ( STDMETHODCALLTYPE *GetUniqueExportFileName )( 
            IVsProfileDataManager * This,
            /* [in] */ VSPROFILEGETFILENAME flags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrExportFile);
        
        HRESULT ( STDMETHODCALLTYPE *GetSettingsFileExtension )( 
            IVsProfileDataManager * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSettingsFileExtension);
        
        HRESULT ( STDMETHODCALLTYPE *GetSettingsForExport )( 
            IVsProfileDataManager * This,
            /* [out] */ __RPC__deref_out_opt IVsProfileSettingsTree **ppSettingsTree);
        
        HRESULT ( STDMETHODCALLTYPE *ExportSettings )( 
            IVsProfileDataManager * This,
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pSettingsTree,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation);
        
        HRESULT ( STDMETHODCALLTYPE *ImportSettings )( 
            IVsProfileDataManager * This,
            /* [in] */ __RPC__in_opt IVsProfileSettingsTree *pSettingsTree,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation);
        
        HRESULT ( STDMETHODCALLTYPE *ResetSettings )( 
            IVsProfileDataManager * This,
            /* [in] */ __RPC__in_opt IVsProfileSettingsFileInfo *pFileInfo,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation);
        
        HRESULT ( STDMETHODCALLTYPE *ExportAllSettings )( 
            IVsProfileDataManager * This,
            /* [in] */ __RPC__in BSTR bstrFilePath,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation);
        
        HRESULT ( STDMETHODCALLTYPE *AutoSaveAllSettings )( 
            IVsProfileDataManager * This,
            /* [out] */ __RPC__deref_out_opt IVsSettingsErrorInformation **ppsettingsErrorInformation);
        
        HRESULT ( STDMETHODCALLTYPE *CheckUpdateTeamSettings )( 
            IVsProfileDataManager * This,
            /* [in] */ VSPROFILETEAMSETTINGSFLAGS dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *ReportTeamSettingsChanged )( 
            IVsProfileDataManager * This,
            /* [in] */ VSPROFILETEAMSETTINGSCHANGEDFLAGS dwFlags);
        
        HRESULT ( STDMETHODCALLTYPE *ShowProfilesUI )( 
            IVsProfileDataManager * This);
        
        END_INTERFACE
    } IVsProfileDataManagerVtbl;

    interface IVsProfileDataManager
    {
        CONST_VTBL struct IVsProfileDataManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfileDataManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfileDataManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfileDataManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfileDataManager_LastResetPoint(This,pbstrResetFilename)	\
    ( (This)->lpVtbl -> LastResetPoint(This,pbstrResetFilename) ) 

#define IVsProfileDataManager_GetSettingsFiles(This,fileLocations,ppCollection)	\
    ( (This)->lpVtbl -> GetSettingsFiles(This,fileLocations,ppCollection) ) 

#define IVsProfileDataManager_GetDefaultSettingsLocation(This,pbstrSettingsLocation)	\
    ( (This)->lpVtbl -> GetDefaultSettingsLocation(This,pbstrSettingsLocation) ) 

#define IVsProfileDataManager_GetUniqueExportFileName(This,flags,pbstrExportFile)	\
    ( (This)->lpVtbl -> GetUniqueExportFileName(This,flags,pbstrExportFile) ) 

#define IVsProfileDataManager_GetSettingsFileExtension(This,pbstrSettingsFileExtension)	\
    ( (This)->lpVtbl -> GetSettingsFileExtension(This,pbstrSettingsFileExtension) ) 

#define IVsProfileDataManager_GetSettingsForExport(This,ppSettingsTree)	\
    ( (This)->lpVtbl -> GetSettingsForExport(This,ppSettingsTree) ) 

#define IVsProfileDataManager_ExportSettings(This,bstrFilePath,pSettingsTree,ppsettingsErrorInformation)	\
    ( (This)->lpVtbl -> ExportSettings(This,bstrFilePath,pSettingsTree,ppsettingsErrorInformation) ) 

#define IVsProfileDataManager_ImportSettings(This,pSettingsTree,ppsettingsErrorInformation)	\
    ( (This)->lpVtbl -> ImportSettings(This,pSettingsTree,ppsettingsErrorInformation) ) 

#define IVsProfileDataManager_ResetSettings(This,pFileInfo,ppsettingsErrorInformation)	\
    ( (This)->lpVtbl -> ResetSettings(This,pFileInfo,ppsettingsErrorInformation) ) 

#define IVsProfileDataManager_ExportAllSettings(This,bstrFilePath,ppsettingsErrorInformation)	\
    ( (This)->lpVtbl -> ExportAllSettings(This,bstrFilePath,ppsettingsErrorInformation) ) 

#define IVsProfileDataManager_AutoSaveAllSettings(This,ppsettingsErrorInformation)	\
    ( (This)->lpVtbl -> AutoSaveAllSettings(This,ppsettingsErrorInformation) ) 

#define IVsProfileDataManager_CheckUpdateTeamSettings(This,dwFlags)	\
    ( (This)->lpVtbl -> CheckUpdateTeamSettings(This,dwFlags) ) 

#define IVsProfileDataManager_ReportTeamSettingsChanged(This,dwFlags)	\
    ( (This)->lpVtbl -> ReportTeamSettingsChanged(This,dwFlags) ) 

#define IVsProfileDataManager_ShowProfilesUI(This)	\
    ( (This)->lpVtbl -> ShowProfilesUI(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfileDataManager_INTERFACE_DEFINED__ */


#ifndef __SVsProfileDataManager_INTERFACE_DEFINED__
#define __SVsProfileDataManager_INTERFACE_DEFINED__

/* interface SVsProfileDataManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsProfileDataManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("20945017-0113-4636-BBFC-0716071B5B84")
    SVsProfileDataManager : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsProfileDataManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsProfileDataManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsProfileDataManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsProfileDataManager * This);
        
        END_INTERFACE
    } SVsProfileDataManagerVtbl;

    interface SVsProfileDataManager
    {
        CONST_VTBL struct SVsProfileDataManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsProfileDataManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsProfileDataManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsProfileDataManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsProfileDataManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0059 */
/* [local] */ 

#define SID_SVsProfileDataManager IID_SVsProfileDataManager


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0059_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0059_v0_0_s_ifspec;

#ifndef __IVsDeferredSaveProject_INTERFACE_DEFINED__
#define __IVsDeferredSaveProject_INTERFACE_DEFINED__

/* interface IVsDeferredSaveProject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDeferredSaveProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("83B2961F-AC2B-409b-89BD-DCF698E3C402")
    IVsDeferredSaveProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SaveProjectToLocation( 
            /* [in] */ __RPC__in LPCOLESTR pszProjectFilename) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDeferredSaveProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDeferredSaveProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDeferredSaveProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDeferredSaveProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *SaveProjectToLocation )( 
            IVsDeferredSaveProject * This,
            /* [in] */ __RPC__in LPCOLESTR pszProjectFilename);
        
        END_INTERFACE
    } IVsDeferredSaveProjectVtbl;

    interface IVsDeferredSaveProject
    {
        CONST_VTBL struct IVsDeferredSaveProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDeferredSaveProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDeferredSaveProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDeferredSaveProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDeferredSaveProject_SaveProjectToLocation(This,pszProjectFilename)	\
    ( (This)->lpVtbl -> SaveProjectToLocation(This,pszProjectFilename) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDeferredSaveProject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0060 */
/* [local] */ 


enum __VSCREATENEWPROJVIADLGEXFLAGS
    {	VNPVDE_ALWAYSNEWSOLUTION	= 0x1,
	VNPVDE_OVERRIDEBROWSEBUTTON	= 0x2,
	VNPVDE_ALWAYSADDTOSOLUTION	= 0x4,
	VNPVDE_ADDNESTEDTOSELECTION	= 0x8,
	VNPVDE_USENEWWEBSITEDLG	= 0x10
    } ;
typedef DWORD VSCREATENEWPROJVIADLGEXFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0060_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0060_v0_0_s_ifspec;

#ifndef __IVsBrowseProjectLocation_INTERFACE_DEFINED__
#define __IVsBrowseProjectLocation_INTERFACE_DEFINED__

/* interface IVsBrowseProjectLocation */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBrowseProjectLocation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("368FC032-AE91-44a2-BE6B-093A8A9E63CC")
    IVsBrowseProjectLocation : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BrowseProjectLocation( 
            /* [in] */ __RPC__in LPCOLESTR pszStartDirectory,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectLocation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBrowseProjectLocationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBrowseProjectLocation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBrowseProjectLocation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBrowseProjectLocation * This);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseProjectLocation )( 
            IVsBrowseProjectLocation * This,
            /* [in] */ __RPC__in LPCOLESTR pszStartDirectory,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectLocation);
        
        END_INTERFACE
    } IVsBrowseProjectLocationVtbl;

    interface IVsBrowseProjectLocation
    {
        CONST_VTBL struct IVsBrowseProjectLocationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBrowseProjectLocation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBrowseProjectLocation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBrowseProjectLocation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBrowseProjectLocation_BrowseProjectLocation(This,pszStartDirectory,pbstrProjectLocation)	\
    ( (This)->lpVtbl -> BrowseProjectLocation(This,pszStartDirectory,pbstrProjectLocation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBrowseProjectLocation_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0061 */
/* [local] */ 


enum __VSSAVEDEFERREDSAVEFLAGS
    {	VSDSF_HIDEADDTOSOURCECONTROL	= 0x1
    } ;
typedef DWORD VSSAVEDEFERREDSAVEFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0061_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0061_v0_0_s_ifspec;

#ifndef __IVsSolution3_INTERFACE_DEFINED__
#define __IVsSolution3_INTERFACE_DEFINED__

/* interface IVsSolution3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolution3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("58DCF7BF-F14E-43ec-A7B2-9F78EDD06418")
    IVsSolution3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateNewProjectViaDlgEx( 
            /* [in] */ __RPC__in LPCOLESTR pszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR pszTemplateDir,
            /* [in] */ __RPC__in LPCOLESTR pszExpand,
            /* [in] */ __RPC__in LPCOLESTR pszSelect,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic,
            /* [in] */ VSCREATENEWPROJVIADLGEXFLAGS cnpvdeFlags,
            /* [in] */ __RPC__in_opt IVsBrowseProjectLocation *pBrowse) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUniqueUINameOfProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUniqueName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckForAndSaveDeferredSaveSolution( 
            /* [in] */ BOOL fCloseSolution,
            /* [in] */ __RPC__in LPCOLESTR pszMessage,
            /* [in] */ __RPC__in LPCOLESTR pszTitle,
            /* [in] */ VSSAVEDEFERREDSAVEFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateProjectFileLocationForUpgrade( 
            /* [in] */ __RPC__in LPCOLESTR pszCurrentLocation,
            /* [in] */ __RPC__in LPCOLESTR pszUpgradedLocation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSolution3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSolution3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSolution3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSolution3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewProjectViaDlgEx )( 
            IVsSolution3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR pszTemplateDir,
            /* [in] */ __RPC__in LPCOLESTR pszExpand,
            /* [in] */ __RPC__in LPCOLESTR pszSelect,
            /* [in] */ __RPC__in LPCOLESTR pszHelpTopic,
            /* [in] */ VSCREATENEWPROJVIADLGEXFLAGS cnpvdeFlags,
            /* [in] */ __RPC__in_opt IVsBrowseProjectLocation *pBrowse);
        
        HRESULT ( STDMETHODCALLTYPE *GetUniqueUINameOfProject )( 
            IVsSolution3 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUniqueName);
        
        HRESULT ( STDMETHODCALLTYPE *CheckForAndSaveDeferredSaveSolution )( 
            IVsSolution3 * This,
            /* [in] */ BOOL fCloseSolution,
            /* [in] */ __RPC__in LPCOLESTR pszMessage,
            /* [in] */ __RPC__in LPCOLESTR pszTitle,
            /* [in] */ VSSAVEDEFERREDSAVEFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateProjectFileLocationForUpgrade )( 
            IVsSolution3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszCurrentLocation,
            /* [in] */ __RPC__in LPCOLESTR pszUpgradedLocation);
        
        END_INTERFACE
    } IVsSolution3Vtbl;

    interface IVsSolution3
    {
        CONST_VTBL struct IVsSolution3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolution3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolution3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolution3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolution3_CreateNewProjectViaDlgEx(This,pszDlgTitle,pszTemplateDir,pszExpand,pszSelect,pszHelpTopic,cnpvdeFlags,pBrowse)	\
    ( (This)->lpVtbl -> CreateNewProjectViaDlgEx(This,pszDlgTitle,pszTemplateDir,pszExpand,pszSelect,pszHelpTopic,cnpvdeFlags,pBrowse) ) 

#define IVsSolution3_GetUniqueUINameOfProject(This,pHierarchy,pbstrUniqueName)	\
    ( (This)->lpVtbl -> GetUniqueUINameOfProject(This,pHierarchy,pbstrUniqueName) ) 

#define IVsSolution3_CheckForAndSaveDeferredSaveSolution(This,fCloseSolution,pszMessage,pszTitle,grfFlags)	\
    ( (This)->lpVtbl -> CheckForAndSaveDeferredSaveSolution(This,fCloseSolution,pszMessage,pszTitle,grfFlags) ) 

#define IVsSolution3_UpdateProjectFileLocationForUpgrade(This,pszCurrentLocation,pszUpgradedLocation)	\
    ( (This)->lpVtbl -> UpdateProjectFileLocationForUpgrade(This,pszCurrentLocation,pszUpgradedLocation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolution3_INTERFACE_DEFINED__ */


#ifndef __IVsConfigurationManagerDlg_INTERFACE_DEFINED__
#define __IVsConfigurationManagerDlg_INTERFACE_DEFINED__

/* interface IVsConfigurationManagerDlg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsConfigurationManagerDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57F5E77F-75C9-42A3-8DAF-579C3556A0DD")
    IVsConfigurationManagerDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowConfigurationManagerDlg( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsConfigurationManagerDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsConfigurationManagerDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsConfigurationManagerDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsConfigurationManagerDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowConfigurationManagerDlg )( 
            IVsConfigurationManagerDlg * This);
        
        END_INTERFACE
    } IVsConfigurationManagerDlgVtbl;

    interface IVsConfigurationManagerDlg
    {
        CONST_VTBL struct IVsConfigurationManagerDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsConfigurationManagerDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsConfigurationManagerDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsConfigurationManagerDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsConfigurationManagerDlg_ShowConfigurationManagerDlg(This)	\
    ( (This)->lpVtbl -> ShowConfigurationManagerDlg(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsConfigurationManagerDlg_INTERFACE_DEFINED__ */


#ifndef __SVsConfigurationManagerDlg_INTERFACE_DEFINED__
#define __SVsConfigurationManagerDlg_INTERFACE_DEFINED__

/* interface SVsConfigurationManagerDlg */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsConfigurationManagerDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8F435BAD-95A1-4032-8CD0-DC74F67CB106")
    SVsConfigurationManagerDlg : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsConfigurationManagerDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsConfigurationManagerDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsConfigurationManagerDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsConfigurationManagerDlg * This);
        
        END_INTERFACE
    } SVsConfigurationManagerDlgVtbl;

    interface SVsConfigurationManagerDlg
    {
        CONST_VTBL struct SVsConfigurationManagerDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsConfigurationManagerDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsConfigurationManagerDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsConfigurationManagerDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsConfigurationManagerDlg_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0064 */
/* [local] */ 

#define SID_SVsConfigurationManagerDlg IID_SVsConfigurationManagerDlg


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0064_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0064_v0_0_s_ifspec;

#ifndef __IVsUpdateSolutionEvents3_INTERFACE_DEFINED__
#define __IVsUpdateSolutionEvents3_INTERFACE_DEFINED__

/* interface IVsUpdateSolutionEvents3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpdateSolutionEvents3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("40025C28-3303-42CA-BED8-0F3BD856BD6D")
    IVsUpdateSolutionEvents3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeActiveSolutionCfgChange( 
            /* [in] */ __RPC__in_opt IVsCfg *pOldActiveSlnCfg,
            /* [in] */ __RPC__in_opt IVsCfg *pNewActiveSlnCfg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterActiveSolutionCfgChange( 
            /* [in] */ __RPC__in_opt IVsCfg *pOldActiveSlnCfg,
            /* [in] */ __RPC__in_opt IVsCfg *pNewActiveSlnCfg) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUpdateSolutionEvents3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUpdateSolutionEvents3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUpdateSolutionEvents3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUpdateSolutionEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeActiveSolutionCfgChange )( 
            IVsUpdateSolutionEvents3 * This,
            /* [in] */ __RPC__in_opt IVsCfg *pOldActiveSlnCfg,
            /* [in] */ __RPC__in_opt IVsCfg *pNewActiveSlnCfg);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterActiveSolutionCfgChange )( 
            IVsUpdateSolutionEvents3 * This,
            /* [in] */ __RPC__in_opt IVsCfg *pOldActiveSlnCfg,
            /* [in] */ __RPC__in_opt IVsCfg *pNewActiveSlnCfg);
        
        END_INTERFACE
    } IVsUpdateSolutionEvents3Vtbl;

    interface IVsUpdateSolutionEvents3
    {
        CONST_VTBL struct IVsUpdateSolutionEvents3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpdateSolutionEvents3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpdateSolutionEvents3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpdateSolutionEvents3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpdateSolutionEvents3_OnBeforeActiveSolutionCfgChange(This,pOldActiveSlnCfg,pNewActiveSlnCfg)	\
    ( (This)->lpVtbl -> OnBeforeActiveSolutionCfgChange(This,pOldActiveSlnCfg,pNewActiveSlnCfg) ) 

#define IVsUpdateSolutionEvents3_OnAfterActiveSolutionCfgChange(This,pOldActiveSlnCfg,pNewActiveSlnCfg)	\
    ( (This)->lpVtbl -> OnAfterActiveSolutionCfgChange(This,pOldActiveSlnCfg,pNewActiveSlnCfg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpdateSolutionEvents3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0065 */
/* [local] */ 

typedef 
enum _vsuptodatecheckflags
    {	VSUTDCF_DTEEONLY	= 0x1
    } 	VsUpToDateCheckFlags;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0065_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0065_v0_0_s_ifspec;

#ifndef __IVsSolutionBuildManager3_INTERFACE_DEFINED__
#define __IVsSolutionBuildManager3_INTERFACE_DEFINED__

/* interface IVsSolutionBuildManager3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionBuildManager3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B6EA87ED-C498-4484-81AC-0BED187E28E6")
    IVsSolutionBuildManager3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseUpdateSolutionEvents3( 
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEvents3 *pIVsUpdateSolutionEvents3,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseUpdateSolutionEvents3( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AreProjectsUpToDate( 
            DWORD dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HasHierarchyChangedSinceLastDTEE( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryBuildManagerBusyEx( 
            /* [out] */ __RPC__out DWORD *pdwBuildManagerOperation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSolutionBuildManager3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSolutionBuildManager3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSolutionBuildManager3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSolutionBuildManager3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseUpdateSolutionEvents3 )( 
            IVsSolutionBuildManager3 * This,
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEvents3 *pIVsUpdateSolutionEvents3,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseUpdateSolutionEvents3 )( 
            IVsSolutionBuildManager3 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *AreProjectsUpToDate )( 
            IVsSolutionBuildManager3 * This,
            DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *HasHierarchyChangedSinceLastDTEE )( 
            IVsSolutionBuildManager3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryBuildManagerBusyEx )( 
            IVsSolutionBuildManager3 * This,
            /* [out] */ __RPC__out DWORD *pdwBuildManagerOperation);
        
        END_INTERFACE
    } IVsSolutionBuildManager3Vtbl;

    interface IVsSolutionBuildManager3
    {
        CONST_VTBL struct IVsSolutionBuildManager3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionBuildManager3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionBuildManager3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionBuildManager3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionBuildManager3_AdviseUpdateSolutionEvents3(This,pIVsUpdateSolutionEvents3,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseUpdateSolutionEvents3(This,pIVsUpdateSolutionEvents3,pdwCookie) ) 

#define IVsSolutionBuildManager3_UnadviseUpdateSolutionEvents3(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseUpdateSolutionEvents3(This,dwCookie) ) 

#define IVsSolutionBuildManager3_AreProjectsUpToDate(This,dwOptions)	\
    ( (This)->lpVtbl -> AreProjectsUpToDate(This,dwOptions) ) 

#define IVsSolutionBuildManager3_HasHierarchyChangedSinceLastDTEE(This)	\
    ( (This)->lpVtbl -> HasHierarchyChangedSinceLastDTEE(This) ) 

#define IVsSolutionBuildManager3_QueryBuildManagerBusyEx(This,pdwBuildManagerOperation)	\
    ( (This)->lpVtbl -> QueryBuildManagerBusyEx(This,pdwBuildManagerOperation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionBuildManager3_INTERFACE_DEFINED__ */


#ifndef __IVsSingleFileGeneratorFactory_INTERFACE_DEFINED__
#define __IVsSingleFileGeneratorFactory_INTERFACE_DEFINED__

/* interface IVsSingleFileGeneratorFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSingleFileGeneratorFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3ADA7A5D-591E-4c9e-8B87-5E33F8E64AA8")
    IVsSingleFileGeneratorFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultGenerator( 
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrGenProgID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateGeneratorInstance( 
            /* [in] */ __RPC__in LPCOLESTR wszProgId,
            /* [out] */ __RPC__out BOOL *pbGeneratesDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbGeneratesSharedDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbUseTempPEFlag,
            /* [out] */ __RPC__deref_out_opt IVsSingleFileGenerator **ppGenerate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGeneratorInformation( 
            /* [in] */ __RPC__in LPCWSTR wszProgID,
            /* [out] */ __RPC__out BOOL *pbGeneratesDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbGeneratesSharedDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbUseTempPEFlag,
            /* [out] */ __RPC__out GUID *pguidGenerator) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSingleFileGeneratorFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSingleFileGeneratorFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSingleFileGeneratorFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSingleFileGeneratorFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultGenerator )( 
            IVsSingleFileGeneratorFactory * This,
            /* [in] */ __RPC__in LPCOLESTR wszFilename,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrGenProgID);
        
        HRESULT ( STDMETHODCALLTYPE *CreateGeneratorInstance )( 
            IVsSingleFileGeneratorFactory * This,
            /* [in] */ __RPC__in LPCOLESTR wszProgId,
            /* [out] */ __RPC__out BOOL *pbGeneratesDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbGeneratesSharedDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbUseTempPEFlag,
            /* [out] */ __RPC__deref_out_opt IVsSingleFileGenerator **ppGenerate);
        
        HRESULT ( STDMETHODCALLTYPE *GetGeneratorInformation )( 
            IVsSingleFileGeneratorFactory * This,
            /* [in] */ __RPC__in LPCWSTR wszProgID,
            /* [out] */ __RPC__out BOOL *pbGeneratesDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbGeneratesSharedDesignTimeSource,
            /* [out] */ __RPC__out BOOL *pbUseTempPEFlag,
            /* [out] */ __RPC__out GUID *pguidGenerator);
        
        END_INTERFACE
    } IVsSingleFileGeneratorFactoryVtbl;

    interface IVsSingleFileGeneratorFactory
    {
        CONST_VTBL struct IVsSingleFileGeneratorFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSingleFileGeneratorFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSingleFileGeneratorFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSingleFileGeneratorFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSingleFileGeneratorFactory_GetDefaultGenerator(This,wszFilename,pbstrGenProgID)	\
    ( (This)->lpVtbl -> GetDefaultGenerator(This,wszFilename,pbstrGenProgID) ) 

#define IVsSingleFileGeneratorFactory_CreateGeneratorInstance(This,wszProgId,pbGeneratesDesignTimeSource,pbGeneratesSharedDesignTimeSource,pbUseTempPEFlag,ppGenerate)	\
    ( (This)->lpVtbl -> CreateGeneratorInstance(This,wszProgId,pbGeneratesDesignTimeSource,pbGeneratesSharedDesignTimeSource,pbUseTempPEFlag,ppGenerate) ) 

#define IVsSingleFileGeneratorFactory_GetGeneratorInformation(This,wszProgID,pbGeneratesDesignTimeSource,pbGeneratesSharedDesignTimeSource,pbUseTempPEFlag,pguidGenerator)	\
    ( (This)->lpVtbl -> GetGeneratorInformation(This,wszProgID,pbGeneratesDesignTimeSource,pbGeneratesSharedDesignTimeSource,pbUseTempPEFlag,pguidGenerator) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSingleFileGeneratorFactory_INTERFACE_DEFINED__ */


#ifndef __IVsStartPageDownload_INTERFACE_DEFINED__
#define __IVsStartPageDownload_INTERFACE_DEFINED__

/* interface IVsStartPageDownload */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsStartPageDownload;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F5B2C031-7093-447f-8486-514FB2CCAD4F")
    IVsStartPageDownload : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartDownloadService( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopDownloadService( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsStartPageDownloadVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsStartPageDownload * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsStartPageDownload * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsStartPageDownload * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartDownloadService )( 
            IVsStartPageDownload * This);
        
        HRESULT ( STDMETHODCALLTYPE *StopDownloadService )( 
            IVsStartPageDownload * This);
        
        END_INTERFACE
    } IVsStartPageDownloadVtbl;

    interface IVsStartPageDownload
    {
        CONST_VTBL struct IVsStartPageDownloadVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsStartPageDownload_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsStartPageDownload_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsStartPageDownload_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsStartPageDownload_StartDownloadService(This)	\
    ( (This)->lpVtbl -> StartDownloadService(This) ) 

#define IVsStartPageDownload_StopDownloadService(This)	\
    ( (This)->lpVtbl -> StopDownloadService(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsStartPageDownload_INTERFACE_DEFINED__ */


#ifndef __SVsStartPageDownload_INTERFACE_DEFINED__
#define __SVsStartPageDownload_INTERFACE_DEFINED__

/* interface SVsStartPageDownload */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsStartPageDownload;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A60FCE08-2F9C-4676-86F0-BCD4973FC702")
    SVsStartPageDownload : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsStartPageDownloadVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsStartPageDownload * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsStartPageDownload * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsStartPageDownload * This);
        
        END_INTERFACE
    } SVsStartPageDownloadVtbl;

    interface SVsStartPageDownload
    {
        CONST_VTBL struct SVsStartPageDownloadVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsStartPageDownload_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsStartPageDownload_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsStartPageDownload_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsStartPageDownload_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0069 */
/* [local] */ 

#define SID_SVsStartPageDownload IID_SVsStartPageDownload

enum __VSMEINIT2
    {	MD_ITEMICONSUPPORT	= 0x10000
    } ;
typedef DWORD VSMEINIT2;


enum __MENUEDITOR_TRANSACTION
    {	MENUEDITOR_TRANSACTION_DISCARD	= -1,
	MENUEDITOR_TRANSACTION_ALL	= ( MENUEDITOR_TRANSACTION_DISCARD + 1 ) ,
	MENUEDITOR_TRANSACTION_CUT	= ( MENUEDITOR_TRANSACTION_ALL + 1 ) ,
	MENUEDITOR_TRANSACTION_PASTE	= ( MENUEDITOR_TRANSACTION_CUT + 1 ) ,
	MENUEDITOR_TRANSACTION_COPY	= ( MENUEDITOR_TRANSACTION_PASTE + 1 ) ,
	MENUEDITOR_TRANSACTION_DELETE	= ( MENUEDITOR_TRANSACTION_COPY + 1 ) 
    } ;
typedef DWORD MENUEDITOR_TRANSACTION;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0069_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0069_v0_0_s_ifspec;

#ifndef __IVsMenuEditorTransactionEvents_INTERFACE_DEFINED__
#define __IVsMenuEditorTransactionEvents_INTERFACE_DEFINED__

/* interface IVsMenuEditorTransactionEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsMenuEditorTransactionEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A0E39F2D-1333-4e71-B3AC-FC7BBFD92D9E")
    IVsMenuEditorTransactionEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeginTransaction( 
            /* [in] */ MENUEDITOR_TRANSACTION trans) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndTransaction( 
            /* [in] */ MENUEDITOR_TRANSACTION trans) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMenuEditorTransactionEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMenuEditorTransactionEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMenuEditorTransactionEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMenuEditorTransactionEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginTransaction )( 
            IVsMenuEditorTransactionEvents * This,
            /* [in] */ MENUEDITOR_TRANSACTION trans);
        
        HRESULT ( STDMETHODCALLTYPE *EndTransaction )( 
            IVsMenuEditorTransactionEvents * This,
            /* [in] */ MENUEDITOR_TRANSACTION trans);
        
        END_INTERFACE
    } IVsMenuEditorTransactionEventsVtbl;

    interface IVsMenuEditorTransactionEvents
    {
        CONST_VTBL struct IVsMenuEditorTransactionEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMenuEditorTransactionEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMenuEditorTransactionEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMenuEditorTransactionEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMenuEditorTransactionEvents_BeginTransaction(This,trans)	\
    ( (This)->lpVtbl -> BeginTransaction(This,trans) ) 

#define IVsMenuEditorTransactionEvents_EndTransaction(This,trans)	\
    ( (This)->lpVtbl -> EndTransaction(This,trans) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMenuEditorTransactionEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0070 */
/* [local] */ 


enum __VSTWDFLAGS
    {	VSTWDFLAGS_NOFLAGS	= 0,
	VSTWDFLAGS_CANCELLABLE	= 0x1,
	VSTWDFLAGS_TOPMOST	= 0x2
    } ;
typedef DWORD VSTWDFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0070_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0070_v0_0_s_ifspec;

#ifndef __IVsThreadedWaitDialog_INTERFACE_DEFINED__
#define __IVsThreadedWaitDialog_INTERFACE_DEFINED__

/* interface IVsThreadedWaitDialog */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadedWaitDialog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E051C7B7-7648-473c-8A7D-2B9554A31F9D")
    IVsThreadedWaitDialog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartWaitDialog( 
            /* [in] */ __RPC__in BSTR bstrWaitCaption,
            /* [in] */ __RPC__in BSTR bstrWaitMessage,
            /* [in] */ __RPC__in BSTR bstrIfTruncateAppend,
            /* [in] */ VSTWDFLAGS dwFlags,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in BSTR bstrStatusBarText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndWaitDialog( 
            __RPC__in BOOL *pfCancelled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GiveTimeSlice( 
            /* [in] */ __RPC__in BSTR bstrUpdatedWaitMessage,
            /* [in] */ __RPC__in BSTR bstrIfTruncateAppend,
            /* [in] */ BOOL fDisableCancel,
            /* [out] */ __RPC__out BOOL *pfCancelled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsThreadedWaitDialogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsThreadedWaitDialog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsThreadedWaitDialog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsThreadedWaitDialog * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialog )( 
            IVsThreadedWaitDialog * This,
            /* [in] */ __RPC__in BSTR bstrWaitCaption,
            /* [in] */ __RPC__in BSTR bstrWaitMessage,
            /* [in] */ __RPC__in BSTR bstrIfTruncateAppend,
            /* [in] */ VSTWDFLAGS dwFlags,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in BSTR bstrStatusBarText);
        
        HRESULT ( STDMETHODCALLTYPE *EndWaitDialog )( 
            IVsThreadedWaitDialog * This,
            __RPC__in BOOL *pfCancelled);
        
        HRESULT ( STDMETHODCALLTYPE *GiveTimeSlice )( 
            IVsThreadedWaitDialog * This,
            /* [in] */ __RPC__in BSTR bstrUpdatedWaitMessage,
            /* [in] */ __RPC__in BSTR bstrIfTruncateAppend,
            /* [in] */ BOOL fDisableCancel,
            /* [out] */ __RPC__out BOOL *pfCancelled);
        
        END_INTERFACE
    } IVsThreadedWaitDialogVtbl;

    interface IVsThreadedWaitDialog
    {
        CONST_VTBL struct IVsThreadedWaitDialogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadedWaitDialog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadedWaitDialog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadedWaitDialog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadedWaitDialog_StartWaitDialog(This,bstrWaitCaption,bstrWaitMessage,bstrIfTruncateAppend,dwFlags,varStatusBmpAnim,bstrStatusBarText)	\
    ( (This)->lpVtbl -> StartWaitDialog(This,bstrWaitCaption,bstrWaitMessage,bstrIfTruncateAppend,dwFlags,varStatusBmpAnim,bstrStatusBarText) ) 

#define IVsThreadedWaitDialog_EndWaitDialog(This,pfCancelled)	\
    ( (This)->lpVtbl -> EndWaitDialog(This,pfCancelled) ) 

#define IVsThreadedWaitDialog_GiveTimeSlice(This,bstrUpdatedWaitMessage,bstrIfTruncateAppend,fDisableCancel,pfCancelled)	\
    ( (This)->lpVtbl -> GiveTimeSlice(This,bstrUpdatedWaitMessage,bstrIfTruncateAppend,fDisableCancel,pfCancelled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadedWaitDialog_INTERFACE_DEFINED__ */


#ifndef __SVsThreadedWaitDialog_INTERFACE_DEFINED__
#define __SVsThreadedWaitDialog_INTERFACE_DEFINED__

/* interface SVsThreadedWaitDialog */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsThreadedWaitDialog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1AC64571-85CF-4234-AA00-B57E907E326A")
    SVsThreadedWaitDialog : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsThreadedWaitDialogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsThreadedWaitDialog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsThreadedWaitDialog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsThreadedWaitDialog * This);
        
        END_INTERFACE
    } SVsThreadedWaitDialogVtbl;

    interface SVsThreadedWaitDialog
    {
        CONST_VTBL struct SVsThreadedWaitDialogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsThreadedWaitDialog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsThreadedWaitDialog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsThreadedWaitDialog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsThreadedWaitDialog_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0072 */
/* [local] */ 

#define SID_SVsThreadedWaitDialog IID_SVsThreadedWaitDialog


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0072_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0072_v0_0_s_ifspec;

#ifndef __IVsProfilesManagerUI_INTERFACE_DEFINED__
#define __IVsProfilesManagerUI_INTERFACE_DEFINED__

/* interface IVsProfilesManagerUI */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfilesManagerUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D06B7887-893C-439c-A231-8BF3E5335E30")
    IVsProfilesManagerUI : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowProfilesUI( 
            /* [in] */ __RPC__in_opt IVsProfileDataManager *pDataManager) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfilesManagerUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfilesManagerUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfilesManagerUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfilesManagerUI * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowProfilesUI )( 
            IVsProfilesManagerUI * This,
            /* [in] */ __RPC__in_opt IVsProfileDataManager *pDataManager);
        
        END_INTERFACE
    } IVsProfilesManagerUIVtbl;

    interface IVsProfilesManagerUI
    {
        CONST_VTBL struct IVsProfilesManagerUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilesManagerUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilesManagerUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilesManagerUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilesManagerUI_ShowProfilesUI(This,pDataManager)	\
    ( (This)->lpVtbl -> ShowProfilesUI(This,pDataManager) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilesManagerUI_INTERFACE_DEFINED__ */


#ifndef __SVsProfilesManagerUI_INTERFACE_DEFINED__
#define __SVsProfilesManagerUI_INTERFACE_DEFINED__

/* interface SVsProfilesManagerUI */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsProfilesManagerUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3B0749FF-31E8-42d8-9CD2-F612148D7BDC")
    SVsProfilesManagerUI : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsProfilesManagerUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsProfilesManagerUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsProfilesManagerUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsProfilesManagerUI * This);
        
        END_INTERFACE
    } SVsProfilesManagerUIVtbl;

    interface SVsProfilesManagerUI
    {
        CONST_VTBL struct SVsProfilesManagerUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsProfilesManagerUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsProfilesManagerUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsProfilesManagerUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsProfilesManagerUI_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0074 */
/* [local] */ 

#define SID_SVsProfilesManagerUI IID_SVsProfilesManagerUI

enum __XMLMEMBERDATA_TAGTYPE
    {	XMLMEMBERDATA_TAGTYPE_CREF	= 0
    } ;
typedef DWORD XMLMEMBERDATA_TAGTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0074_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0074_v0_0_s_ifspec;

#ifndef __IVsXMLMemberDataCallBack_INTERFACE_DEFINED__
#define __IVsXMLMemberDataCallBack_INTERFACE_DEFINED__

/* interface IVsXMLMemberDataCallBack */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsXMLMemberDataCallBack;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C7AE54AB-ABF2-494a-BC8C-C577ABB874C9")
    IVsXMLMemberDataCallBack : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDisplayNameForTag( 
            /* [in] */ XMLMEMBERDATA_TAGTYPE nTagType,
            /* [in] */ __RPC__in LPCOLESTR wszBufferIn,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBufferOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsXMLMemberDataCallBackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsXMLMemberDataCallBack * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsXMLMemberDataCallBack * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsXMLMemberDataCallBack * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayNameForTag )( 
            IVsXMLMemberDataCallBack * This,
            /* [in] */ XMLMEMBERDATA_TAGTYPE nTagType,
            /* [in] */ __RPC__in LPCOLESTR wszBufferIn,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBufferOut);
        
        END_INTERFACE
    } IVsXMLMemberDataCallBackVtbl;

    interface IVsXMLMemberDataCallBack
    {
        CONST_VTBL struct IVsXMLMemberDataCallBackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsXMLMemberDataCallBack_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsXMLMemberDataCallBack_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsXMLMemberDataCallBack_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsXMLMemberDataCallBack_GetDisplayNameForTag(This,nTagType,wszBufferIn,pbstrBufferOut)	\
    ( (This)->lpVtbl -> GetDisplayNameForTag(This,nTagType,wszBufferIn,pbstrBufferOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsXMLMemberDataCallBack_INTERFACE_DEFINED__ */


#ifndef __IVsXMLMemberDataRegisterCallBack_INTERFACE_DEFINED__
#define __IVsXMLMemberDataRegisterCallBack_INTERFACE_DEFINED__

/* interface IVsXMLMemberDataRegisterCallBack */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsXMLMemberDataRegisterCallBack;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9B17D7E-AAB8-43cb-AB40-B4E26E0B6D48")
    IVsXMLMemberDataRegisterCallBack : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterCallBack( 
            /* [in] */ __RPC__in_opt IVsXMLMemberDataCallBack *pIVsXMLMemberDataCallBack) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterCallBack( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsXMLMemberDataRegisterCallBackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsXMLMemberDataRegisterCallBack * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsXMLMemberDataRegisterCallBack * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsXMLMemberDataRegisterCallBack * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterCallBack )( 
            IVsXMLMemberDataRegisterCallBack * This,
            /* [in] */ __RPC__in_opt IVsXMLMemberDataCallBack *pIVsXMLMemberDataCallBack);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterCallBack )( 
            IVsXMLMemberDataRegisterCallBack * This);
        
        END_INTERFACE
    } IVsXMLMemberDataRegisterCallBackVtbl;

    interface IVsXMLMemberDataRegisterCallBack
    {
        CONST_VTBL struct IVsXMLMemberDataRegisterCallBackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsXMLMemberDataRegisterCallBack_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsXMLMemberDataRegisterCallBack_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsXMLMemberDataRegisterCallBack_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsXMLMemberDataRegisterCallBack_RegisterCallBack(This,pIVsXMLMemberDataCallBack)	\
    ( (This)->lpVtbl -> RegisterCallBack(This,pIVsXMLMemberDataCallBack) ) 

#define IVsXMLMemberDataRegisterCallBack_UnregisterCallBack(This)	\
    ( (This)->lpVtbl -> UnregisterCallBack(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsXMLMemberDataRegisterCallBack_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0076 */
/* [local] */ 


enum __XMLMEMBERDATA_OPTIONS
    {	XMLMEMBERDATA_OPTIONS_NONE	= 0,
	XMLMEMBERDATA_OPTIONS_PRESERVE_NEWLINES	= 0x1
    } ;
typedef DWORD XMLMEMBERDATA_OPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0076_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0076_v0_0_s_ifspec;

#ifndef __IVsXMLMemberData3_INTERFACE_DEFINED__
#define __IVsXMLMemberData3_INTERFACE_DEFINED__

/* interface IVsXMLMemberData3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsXMLMemberData3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C04165C2-3CAC-4508-B651-DD24906DBD4D")
    IVsXMLMemberData3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetOptions( 
            /* [in] */ XMLMEMBERDATA_OPTIONS options) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSummaryText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSummary) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParamCount( 
            /* [out] */ __RPC__out long *piParams) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParamTextAt( 
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReturnsText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrReturns) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemarksText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRemarks) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionCount( 
            /* [out] */ __RPC__out long *piExceptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExceptionTextAt( 
            /* [in] */ long iException,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFilterPriority( 
            /* [out] */ __RPC__out long *piFilterPriority) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCompletionListText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCompletionListTextAt( 
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPermissionSet( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPermissionSetXML) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeParamCount( 
            /* [out] */ __RPC__out long *piTypeParams) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeParamTextAt( 
            /* [in] */ long iTypeParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsXMLMemberData3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsXMLMemberData3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsXMLMemberData3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsXMLMemberData3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetOptions )( 
            IVsXMLMemberData3 * This,
            /* [in] */ XMLMEMBERDATA_OPTIONS options);
        
        HRESULT ( STDMETHODCALLTYPE *GetSummaryText )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSummary);
        
        HRESULT ( STDMETHODCALLTYPE *GetParamCount )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__out long *piParams);
        
        HRESULT ( STDMETHODCALLTYPE *GetParamTextAt )( 
            IVsXMLMemberData3 * This,
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetReturnsText )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrReturns);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemarksText )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRemarks);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionCount )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__out long *piExceptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionTextAt )( 
            IVsXMLMemberData3 * This,
            /* [in] */ long iException,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilterPriority )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__out long *piFilterPriority);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompletionListText )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompletionListTextAt )( 
            IVsXMLMemberData3 * This,
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList);
        
        HRESULT ( STDMETHODCALLTYPE *GetPermissionSet )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPermissionSetXML);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeParamCount )( 
            IVsXMLMemberData3 * This,
            /* [out] */ __RPC__out long *piTypeParams);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeParamTextAt )( 
            IVsXMLMemberData3 * This,
            /* [in] */ long iTypeParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        END_INTERFACE
    } IVsXMLMemberData3Vtbl;

    interface IVsXMLMemberData3
    {
        CONST_VTBL struct IVsXMLMemberData3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsXMLMemberData3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsXMLMemberData3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsXMLMemberData3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsXMLMemberData3_SetOptions(This,options)	\
    ( (This)->lpVtbl -> SetOptions(This,options) ) 

#define IVsXMLMemberData3_GetSummaryText(This,pbstrSummary)	\
    ( (This)->lpVtbl -> GetSummaryText(This,pbstrSummary) ) 

#define IVsXMLMemberData3_GetParamCount(This,piParams)	\
    ( (This)->lpVtbl -> GetParamCount(This,piParams) ) 

#define IVsXMLMemberData3_GetParamTextAt(This,iParam,pbstrName,pbstrText)	\
    ( (This)->lpVtbl -> GetParamTextAt(This,iParam,pbstrName,pbstrText) ) 

#define IVsXMLMemberData3_GetReturnsText(This,pbstrReturns)	\
    ( (This)->lpVtbl -> GetReturnsText(This,pbstrReturns) ) 

#define IVsXMLMemberData3_GetRemarksText(This,pbstrRemarks)	\
    ( (This)->lpVtbl -> GetRemarksText(This,pbstrRemarks) ) 

#define IVsXMLMemberData3_GetExceptionCount(This,piExceptions)	\
    ( (This)->lpVtbl -> GetExceptionCount(This,piExceptions) ) 

#define IVsXMLMemberData3_GetExceptionTextAt(This,iException,pbstrType,pbstrText)	\
    ( (This)->lpVtbl -> GetExceptionTextAt(This,iException,pbstrType,pbstrText) ) 

#define IVsXMLMemberData3_GetFilterPriority(This,piFilterPriority)	\
    ( (This)->lpVtbl -> GetFilterPriority(This,piFilterPriority) ) 

#define IVsXMLMemberData3_GetCompletionListText(This,pbstrCompletionList)	\
    ( (This)->lpVtbl -> GetCompletionListText(This,pbstrCompletionList) ) 

#define IVsXMLMemberData3_GetCompletionListTextAt(This,iParam,pbstrCompletionList)	\
    ( (This)->lpVtbl -> GetCompletionListTextAt(This,iParam,pbstrCompletionList) ) 

#define IVsXMLMemberData3_GetPermissionSet(This,pbstrPermissionSetXML)	\
    ( (This)->lpVtbl -> GetPermissionSet(This,pbstrPermissionSetXML) ) 

#define IVsXMLMemberData3_GetTypeParamCount(This,piTypeParams)	\
    ( (This)->lpVtbl -> GetTypeParamCount(This,piTypeParams) ) 

#define IVsXMLMemberData3_GetTypeParamTextAt(This,iTypeParam,pbstrName,pbstrText)	\
    ( (This)->lpVtbl -> GetTypeParamTextAt(This,iTypeParam,pbstrName,pbstrText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsXMLMemberData3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0077 */
/* [local] */ 


enum _LIB_LISTTYPE2
    {	LLT_NIL	= 0,
	LLT_MEMBERHIERARCHY	= 0x20
    } ;
typedef DWORD LIB_LISTTYPE2;

typedef LIB_LISTTYPE2 LIBCAT_LISTTYPE2;

#define COUNT_LIBCAT_LISTTYPE 6

enum _LIB_FLAGS2
    {	LF_SUPPORTSPROJECTREFERENCES	= 0x10,
	LF_SUPPORTSFILTERING	= 0x20,
	LF_SUPPORTSFILTERINGWITHEXPANSION	= 0x40,
	LF_SUPPORTSCALLBROWSER	= 0x80,
	LF_SUPPORTSLISTREFERENCES	= 0x100,
	LF_SUPPORTSALWAYSUPDATE	= 0x400,
	LF_SUPPORTSBASETYPES	= 0x800,
	LF_SUPPORTSDERIVEDTYPES	= 0x1000,
	LF_SUPPORTSINHERITEDMEMBERS	= 0x2000,
	LF_SUPPORTSPRIVATEMEMBERS	= 0x4000,
	LF_SUPPORTSCLASSDESIGNER	= 0x8000,
	LF_SHOWFULLNAMESINFINDSYMBOLRESULTS	= 0x10000
    } ;
typedef DWORD LIB_FLAGS2;


enum _LIB_LISTCAPABILITIES2
    {	LLC_ALLOWELEMENTSEARCH	= 0x100
    } ;
typedef DWORD LIB_LISTCAPABILITIES2;


enum _VSOBSEARCHOPTIONS2
    {	VSOBSO_FILTERING	= 0x100,
	VSOBSO_EXPANDCHILDREN	= 0x200,
	VSOBSO_CALLSTO	= 0x400,
	VSOBSO_CALLSFROM	= 0x800,
	VSOBSO_LISTREFERENCES	= 0x1000
    } ;
typedef DWORD VSOBSEARCHOPTIONS2;


typedef struct _VSOBSEARCHCRITERIA2
    {
    LPCOLESTR szName;
    VSOBSEARCHTYPE eSrchType;
    VSOBSEARCHOPTIONS2 grfOptions;
    DWORD dwCustom;
    IVsNavInfo *pIVsNavInfo;
    } 	VSOBSEARCHCRITERIA2;

typedef struct _VSOBNAVNAMEINFONODE2
    {
    WCHAR *pszName;
    LIB_LISTTYPE2 lltName;
    struct _VSOBNAVNAMEINFONODE2 *pNext;
    } 	VSOBNAVNAMEINFONODE2;

typedef struct _VSOBNAVIGATIONINFO3
    {
    GUID *pguidLib;
    WCHAR *pszLibName;
    VSOBNAVNAMEINFONODE2 *pName;
    DWORD dwCustom;
    } 	VSOBNAVIGATIONINFO3;


enum _LIB_CATEGORY2
    {	LC_NIL	= 0,
	LC_PHYSICALCONTAINERTYPE	= 10,
	LC_HIERARCHYTYPE	= 11,
	LC_MEMBERINHERITANCE	= 12,
	LC_SEARCHMATCHTYPE	= 13,
	LC_Last2	= LC_SEARCHMATCHTYPE
    } ;
typedef LONG LIB_CATEGORY2;

#define COUNT_LIB_CATEGORY2 13

enum _LIBCAT_CLASSTYPE2
    {	LCCT_TEMPLATE	= 0x4000,
	LCCT_GENERIC	= 0x8000,
	LCCT_ITERATOR	= 0x10000
    } ;
typedef DWORD LIBCAT_CLASSTYPE2;

#define COUNT_LIBCAT_CLASSTYPE2 17

enum _LIBCAT_MEMBERTYPE2
    {	LCMT_TEMPLATE	= 0x4000,
	LCMT_GENERIC	= 0x8000
    } ;
typedef DWORD LIBCAT_MEMBERTYPE2;

#define COUNT_LIBCAT_MEMBERTYPE2 16

enum _LIBCAT_PHYSICALCONTAINERTYPE
    {	LCPT_GLOBAL	= 0x1,
	LCPT_PROJECTREFERENCE	= 0x2,
	LCPT_PROJECT	= 0x4
    } ;
typedef DWORD LIBCAT_PHYSICALCONTAINERTYPE;

#define COUNT_LIBCAT_PHYSICALCONTAINERTYPE 3

enum _LIBCAT_HIERARCHYTYPE
    {	LCHT_UNKNOWN	= 0x1,
	LCHT_FOLDER	= 0x2,
	LCHT_BASESANDINTERFACES	= 0x4,
	LCHT_PROJECTREFERENCES	= 0x8,
	LCHT_DERIVEDTYPES	= 0x10,
	LCHT_INFO	= 0x20
    } ;
typedef DWORD LIBCAT_HIERARCHYTYPE;

#define COUNT_LIBCAT_HIERARCHYTYPE 5

enum _LIBCAT_MEMBERINHERITANCE
    {	LCMI_IMMEDIATE	= 0x1,
	LCMI_OVERRIDABLE	= 0x2,
	LCMI_OVERRIDEREQUIRED	= 0x4,
	LCMI_OVERRIDDEN	= 0x8,
	LCMI_NOTOVERRIDABLE	= 0x10,
	LCMI_INHERITED	= 0x20
    } ;
typedef DWORD LIBCAT_MEMBERINHERITANCE;

#define COUNT_LIBCAT_MEMBERINHERITANCE 6

enum _LIBCAT_SEARCHMATCHTYPE
    {	LSMT_WHOLEWORD	= 0x1,
	LSMT_WHOLEWORD_NO_CASE	= 0x2,
	LSMT_LEAF_WHOLEWORD	= 0x4,
	LSMT_LEAF_WHOLEWORD_NO_CASE	= 0x8,
	LSMT_PART_WHOLEWORD	= 0x10,
	LSMT_PART_WHOLEWORD_NO_CASE	= 0x20,
	LSMT_PRESTRING	= 0x40,
	LSMT_PRESTRING_NO_CASE	= 0x80,
	LSMT_LEAF_PRESTRING	= 0x100,
	LSMT_LEAF_PRESTRING_NO_CASE	= 0x200,
	LSMT_PART_PRESTRING	= 0x400,
	LSMT_PART_PRESTRING_NO_CASE	= 0x800,
	LSMT_SUBSTRING	= 0x1000,
	LSMT_SUBSTRING_NO_CASE	= 0x2000,
	LSMT_NO_MATCH	= 0x4000
    } ;
typedef DWORD LIBCAT_SEARCHMATCHTYPE;

#define COUNT_LIBCAT_SEARCHMATCHTYPE 15

enum _VSOBJLISTELEMPROPID
    {	VSOBJLISTELEMPROPID_FIRST	= 1,
	VSOBJLISTELEMPROPID_HELPKEYWORD	= 1,
	VSOBJLISTELEMPROPID_COMPONENTPATH	= 2,
	VSOBJLISTELEMPROPID_CODEDEFVIEWCONTEXT	= 3,
	VSOBJLISTELEMPROPID_SUPPORTSCALLSTO	= 4,
	VSOBJLISTELEMPROPID_SUPPORTSCALLSFROM	= 5,
	VSOBJLISTELEMPROPID_FULLNAME	= 6,
	VSOBJLISTELEMPROPID_LEAFNAME	= 7,
	VSOBJLISTELEMPROPID_NAME_FOR_RENAME	= 8,
	VSOBJLISTELEMPROPID_LAST	= 8
    } ;
typedef LONG VSOBJLISTELEMPROPID;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0077_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0077_v0_0_s_ifspec;

#ifndef __IVsNavInfoNode_INTERFACE_DEFINED__
#define __IVsNavInfoNode_INTERFACE_DEFINED__

/* interface IVsNavInfoNode */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsNavInfoNode;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1BD1DD01-1246-4694-9DA7-8ADCD9CA3275")
    IVsNavInfoNode : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Type( 
            /* [out] */ __RPC__out DWORD *pllt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Name( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsNavInfoNodeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsNavInfoNode * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsNavInfoNode * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsNavInfoNode * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IVsNavInfoNode * This,
            /* [out] */ __RPC__out DWORD *pllt);
        
        HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            IVsNavInfoNode * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        END_INTERFACE
    } IVsNavInfoNodeVtbl;

    interface IVsNavInfoNode
    {
        CONST_VTBL struct IVsNavInfoNodeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNavInfoNode_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNavInfoNode_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNavInfoNode_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNavInfoNode_get_Type(This,pllt)	\
    ( (This)->lpVtbl -> get_Type(This,pllt) ) 

#define IVsNavInfoNode_get_Name(This,pbstrName)	\
    ( (This)->lpVtbl -> get_Name(This,pbstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNavInfoNode_INTERFACE_DEFINED__ */


#ifndef __IVsEnumNavInfoNodes_INTERFACE_DEFINED__
#define __IVsEnumNavInfoNodes_INTERFACE_DEFINED__

/* interface IVsEnumNavInfoNodes */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumNavInfoNodes;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D2042A65-5E86-4cfa-AD68-F2D6886628D7")
    IVsEnumNavInfoNodes : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsNavInfoNode **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumNavInfoNodesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumNavInfoNodes * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumNavInfoNodes * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumNavInfoNodes * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumNavInfoNodes * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsNavInfoNode **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumNavInfoNodes * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumNavInfoNodes * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumNavInfoNodes * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppenum);
        
        END_INTERFACE
    } IVsEnumNavInfoNodesVtbl;

    interface IVsEnumNavInfoNodes
    {
        CONST_VTBL struct IVsEnumNavInfoNodesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumNavInfoNodes_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumNavInfoNodes_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumNavInfoNodes_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumNavInfoNodes_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumNavInfoNodes_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumNavInfoNodes_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumNavInfoNodes_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumNavInfoNodes_INTERFACE_DEFINED__ */


#ifndef __IVsNavInfo_INTERFACE_DEFINED__
#define __IVsNavInfo_INTERFACE_DEFINED__

/* interface IVsNavInfo */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsNavInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("61772DB8-2D5B-49cb-9CE8-891459921F7F")
    IVsNavInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLibGuid( 
            /* [out] */ __RPC__out GUID *pGuid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSymbolType( 
            /* [out] */ __RPC__out DWORD *pdwType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumPresentationNodes( 
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCanonicalNodes( 
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsNavInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsNavInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsNavInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsNavInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLibGuid )( 
            IVsNavInfo * This,
            /* [out] */ __RPC__out GUID *pGuid);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolType )( 
            IVsNavInfo * This,
            /* [out] */ __RPC__out DWORD *pdwType);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPresentationNodes )( 
            IVsNavInfo * This,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCanonicalNodes )( 
            IVsNavInfo * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum);
        
        END_INTERFACE
    } IVsNavInfoVtbl;

    interface IVsNavInfo
    {
        CONST_VTBL struct IVsNavInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNavInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNavInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNavInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNavInfo_GetLibGuid(This,pGuid)	\
    ( (This)->lpVtbl -> GetLibGuid(This,pGuid) ) 

#define IVsNavInfo_GetSymbolType(This,pdwType)	\
    ( (This)->lpVtbl -> GetSymbolType(This,pdwType) ) 

#define IVsNavInfo_EnumPresentationNodes(This,dwFlags,ppEnum)	\
    ( (This)->lpVtbl -> EnumPresentationNodes(This,dwFlags,ppEnum) ) 

#define IVsNavInfo_EnumCanonicalNodes(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumCanonicalNodes(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNavInfo_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0080 */
/* [local] */ 

typedef struct tagSymbolDescriptionNode
    {
    DWORD dwType;
    LPCOLESTR pszName;
    } 	SYMBOL_DESCRIPTION_NODE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0080_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0080_v0_0_s_ifspec;

#ifndef __IVsObjectBrowserDescription3_INTERFACE_DEFINED__
#define __IVsObjectBrowserDescription3_INTERFACE_DEFINED__

/* interface IVsObjectBrowserDescription3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsObjectBrowserDescription3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2BCD7A6A-D251-4286-9A61-BDEDDE91114F")
    IVsObjectBrowserDescription3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddDescriptionText3( 
            /* [in] */ __RPC__in LPCWSTR pText,
            /* [in] */ VSOBDESCRIPTIONSECTION obdSect,
            /* [in] */ __RPC__in_opt IVsNavInfo *pHyperJump) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearDescriptionText( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsObjectBrowserDescription3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsObjectBrowserDescription3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsObjectBrowserDescription3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsObjectBrowserDescription3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddDescriptionText3 )( 
            IVsObjectBrowserDescription3 * This,
            /* [in] */ __RPC__in LPCWSTR pText,
            /* [in] */ VSOBDESCRIPTIONSECTION obdSect,
            /* [in] */ __RPC__in_opt IVsNavInfo *pHyperJump);
        
        HRESULT ( STDMETHODCALLTYPE *ClearDescriptionText )( 
            IVsObjectBrowserDescription3 * This);
        
        END_INTERFACE
    } IVsObjectBrowserDescription3Vtbl;

    interface IVsObjectBrowserDescription3
    {
        CONST_VTBL struct IVsObjectBrowserDescription3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsObjectBrowserDescription3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsObjectBrowserDescription3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsObjectBrowserDescription3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsObjectBrowserDescription3_AddDescriptionText3(This,pText,obdSect,pHyperJump)	\
    ( (This)->lpVtbl -> AddDescriptionText3(This,pText,obdSect,pHyperJump) ) 

#define IVsObjectBrowserDescription3_ClearDescriptionText(This)	\
    ( (This)->lpVtbl -> ClearDescriptionText(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsObjectBrowserDescription3_INTERFACE_DEFINED__ */


#ifndef __IVsObjectList2_INTERFACE_DEFINED__
#define __IVsObjectList2_INTERFACE_DEFINED__

/* interface IVsObjectList2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsObjectList2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E37F46C4-C627-4d88-A091-2992EE33B51D")
    IVsObjectList2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFlags( 
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemCount( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandedList( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsLiteTreeList **pptlNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateExpandedList( 
            /* [in] */ __RPC__in_opt IVsLiteTreeList *ExpandedList,
            /* [out] */ __RPC__out ULONG *iIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnClose( 
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandable( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetDisplayData( 
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate,
            /* [out] */ __RPC__out VSTREEITEMCHANGESMASK *pgrfChanges) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetListChanges( 
            /* [out][in] */ __RPC__inout ULONG *pcChanges,
            /* [size_is][in] */ __RPC__in_ecount_full(*pcChanges) VSTREELISTITEMCHANGE *prgListChanges) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToggleState( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCapabilities2( 
            /* [out] */ __RPC__out LIB_LISTCAPABILITIES2 *pgrfCapabilities) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetList2( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCategoryField2( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pfCatField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandable3( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListTypeExcluded,
            /* [out] */ __RPC__out BOOL *pfExpandable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavigationInfo2( 
            /* [in] */ ULONG Index,
            /* [out][in] */ __RPC__inout VSOBNAVIGATIONINFO3 *pobNav) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateNavigationInfo2( 
            /* [in] */ __RPC__in VSOBNAVIGATIONINFO3 *pobNav,
            /* [in] */ __RPC__in VSOBNAVNAMEINFONODE2 *pobName,
            /* [in] */ BOOL fDontUpdate,
            /* [out] */ __RPC__out BOOL *pfMatchedName,
            /* [out] */ __RPC__out ULONG *pIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBrowseObject( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispBrowseObj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserContext( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkUserCtx) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowHelp( 
            /* [in] */ ULONG Index) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceContext( 
            /* [in] */ ULONG Index,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **pszFileName,
            /* [out] */ __RPC__out ULONG *pulLineNum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountSourceItems( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHier,
            /* [out] */ __RPC__out VSITEMID *pitemid,
            /* [retval][out] */ __RPC__out ULONG *pcItems) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMultipleSourceItems( 
            /* [in] */ ULONG Index,
            /* [in] */ VSGSIFLAGS grfGSI,
            /* [in] */ ULONG cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) VSITEMSELECTION rgItemSel[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanGoToSource( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GoToSource( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextMenu( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out CLSID *pclsidActive,
            /* [out] */ __RPC__out LONG *pnMenuId,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppCmdTrgtActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryDragDrop( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoDragDrop( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanRename( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoRename( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [in] */ VSOBJOPFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDelete( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoDelete( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJOPFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FillDescription( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription2 *pobDesc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FillDescription2( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription3 *pobDesc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumClipboardFormats( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSOBJCLIPFORMAT rgcfFormats[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClipboardFormat( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in FORMATETC *pFormatetc,
            /* [in] */ __RPC__in STGMEDIUM *pMedium) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtendedClipboardVariant( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in const VSOBJCLIPFORMAT *pcfFormat,
            /* [out] */ __RPC__out VARIANT *pvarFormat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJLISTELEMPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavInfo( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavInfoNode( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfoNode **ppNavInfoNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateNavInfoNode( 
            /* [in] */ __RPC__in_opt IVsNavInfoNode *pNavInfoNode,
            /* [out] */ __RPC__out ULONG *pulIndex) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsObjectList2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsObjectList2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsObjectList2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsObjectList2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFlags )( 
            IVsObjectList2 * This,
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemCount )( 
            IVsObjectList2 * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandedList )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsLiteTreeList **pptlNode);
        
        HRESULT ( STDMETHODCALLTYPE *LocateExpandedList )( 
            IVsObjectList2 * This,
            /* [in] */ __RPC__in_opt IVsLiteTreeList *ExpandedList,
            /* [out] */ __RPC__out ULONG *iIndex);
        
        HRESULT ( STDMETHODCALLTYPE *OnClose )( 
            IVsObjectList2 * This,
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandable )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetDisplayData )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsObjectList2 * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate,
            /* [out] */ __RPC__out VSTREEITEMCHANGESMASK *pgrfChanges);
        
        HRESULT ( STDMETHODCALLTYPE *GetListChanges )( 
            IVsObjectList2 * This,
            /* [out][in] */ __RPC__inout ULONG *pcChanges,
            /* [size_is][in] */ __RPC__in_ecount_full(*pcChanges) VSTREELISTITEMCHANGE *prgListChanges);
        
        HRESULT ( STDMETHODCALLTYPE *ToggleState )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr);
        
        HRESULT ( STDMETHODCALLTYPE *GetCapabilities2 )( 
            IVsObjectList2 * This,
            /* [out] */ __RPC__out LIB_LISTCAPABILITIES2 *pgrfCapabilities);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetCategoryField2 )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandable3 )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListTypeExcluded,
            /* [out] */ __RPC__out BOOL *pfExpandable);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavigationInfo2 )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out][in] */ __RPC__inout VSOBNAVIGATIONINFO3 *pobNav);
        
        HRESULT ( STDMETHODCALLTYPE *LocateNavigationInfo2 )( 
            IVsObjectList2 * This,
            /* [in] */ __RPC__in VSOBNAVIGATIONINFO3 *pobNav,
            /* [in] */ __RPC__in VSOBNAVNAMEINFONODE2 *pobName,
            /* [in] */ BOOL fDontUpdate,
            /* [out] */ __RPC__out BOOL *pfMatchedName,
            /* [out] */ __RPC__out ULONG *pIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetBrowseObject )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispBrowseObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserContext )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkUserCtx);
        
        HRESULT ( STDMETHODCALLTYPE *ShowHelp )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceContext )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **pszFileName,
            /* [out] */ __RPC__out ULONG *pulLineNum);
        
        HRESULT ( STDMETHODCALLTYPE *CountSourceItems )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHier,
            /* [out] */ __RPC__out VSITEMID *pitemid,
            /* [retval][out] */ __RPC__out ULONG *pcItems);
        
        HRESULT ( STDMETHODCALLTYPE *GetMultipleSourceItems )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSGSIFLAGS grfGSI,
            /* [in] */ ULONG cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) VSITEMSELECTION rgItemSel[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *CanGoToSource )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *GoToSource )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextMenu )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out CLSID *pclsidActive,
            /* [out] */ __RPC__out LONG *pnMenuId,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppCmdTrgtActive);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDragDrop )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *DoDragDrop )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *CanRename )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *DoRename )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [in] */ VSOBJOPFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *CanDelete )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *DoDelete )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJOPFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *FillDescription )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription2 *pobDesc);
        
        HRESULT ( STDMETHODCALLTYPE *FillDescription2 )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription3 *pobDesc);
        
        HRESULT ( STDMETHODCALLTYPE *EnumClipboardFormats )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSOBJCLIPFORMAT rgcfFormats[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *GetClipboardFormat )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in FORMATETC *pFormatetc,
            /* [in] */ __RPC__in STGMEDIUM *pMedium);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedClipboardVariant )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in const VSOBJCLIPFORMAT *pcfFormat,
            /* [out] */ __RPC__out VARIANT *pvarFormat);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJLISTELEMPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfo )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfoNode )( 
            IVsObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfoNode **ppNavInfoNode);
        
        HRESULT ( STDMETHODCALLTYPE *LocateNavInfoNode )( 
            IVsObjectList2 * This,
            /* [in] */ __RPC__in_opt IVsNavInfoNode *pNavInfoNode,
            /* [out] */ __RPC__out ULONG *pulIndex);
        
        END_INTERFACE
    } IVsObjectList2Vtbl;

    interface IVsObjectList2
    {
        CONST_VTBL struct IVsObjectList2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsObjectList2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsObjectList2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsObjectList2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsObjectList2_GetFlags(This,pFlags)	\
    ( (This)->lpVtbl -> GetFlags(This,pFlags) ) 

#define IVsObjectList2_GetItemCount(This,pCount)	\
    ( (This)->lpVtbl -> GetItemCount(This,pCount) ) 

#define IVsObjectList2_GetExpandedList(This,Index,pfCanRecurse,pptlNode)	\
    ( (This)->lpVtbl -> GetExpandedList(This,Index,pfCanRecurse,pptlNode) ) 

#define IVsObjectList2_LocateExpandedList(This,ExpandedList,iIndex)	\
    ( (This)->lpVtbl -> LocateExpandedList(This,ExpandedList,iIndex) ) 

#define IVsObjectList2_OnClose(This,ptca)	\
    ( (This)->lpVtbl -> OnClose(This,ptca) ) 

#define IVsObjectList2_GetText(This,Index,tto,ppszText)	\
    ( (This)->lpVtbl -> GetText(This,Index,tto,ppszText) ) 

#define IVsObjectList2_GetTipText(This,Index,eTipType,ppszText)	\
    ( (This)->lpVtbl -> GetTipText(This,Index,eTipType,ppszText) ) 

#define IVsObjectList2_GetExpandable(This,Index,pfExpandable)	\
    ( (This)->lpVtbl -> GetExpandable(This,Index,pfExpandable) ) 

#define IVsObjectList2_GetDisplayData(This,Index,pData)	\
    ( (This)->lpVtbl -> GetDisplayData(This,Index,pData) ) 

#define IVsObjectList2_UpdateCounter(This,pCurUpdate,pgrfChanges)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate,pgrfChanges) ) 

#define IVsObjectList2_GetListChanges(This,pcChanges,prgListChanges)	\
    ( (This)->lpVtbl -> GetListChanges(This,pcChanges,prgListChanges) ) 

#define IVsObjectList2_ToggleState(This,Index,ptscr)	\
    ( (This)->lpVtbl -> ToggleState(This,Index,ptscr) ) 

#define IVsObjectList2_GetCapabilities2(This,pgrfCapabilities)	\
    ( (This)->lpVtbl -> GetCapabilities2(This,pgrfCapabilities) ) 

#define IVsObjectList2_GetList2(This,Index,ListType,Flags,pobSrch,ppIVsObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,Index,ListType,Flags,pobSrch,ppIVsObjectList2) ) 

#define IVsObjectList2_GetCategoryField2(This,Index,Category,pfCatField)	\
    ( (This)->lpVtbl -> GetCategoryField2(This,Index,Category,pfCatField) ) 

#define IVsObjectList2_GetExpandable3(This,Index,ListTypeExcluded,pfExpandable)	\
    ( (This)->lpVtbl -> GetExpandable3(This,Index,ListTypeExcluded,pfExpandable) ) 

#define IVsObjectList2_GetNavigationInfo2(This,Index,pobNav)	\
    ( (This)->lpVtbl -> GetNavigationInfo2(This,Index,pobNav) ) 

#define IVsObjectList2_LocateNavigationInfo2(This,pobNav,pobName,fDontUpdate,pfMatchedName,pIndex)	\
    ( (This)->lpVtbl -> LocateNavigationInfo2(This,pobNav,pobName,fDontUpdate,pfMatchedName,pIndex) ) 

#define IVsObjectList2_GetBrowseObject(This,Index,ppdispBrowseObj)	\
    ( (This)->lpVtbl -> GetBrowseObject(This,Index,ppdispBrowseObj) ) 

#define IVsObjectList2_GetUserContext(This,Index,ppunkUserCtx)	\
    ( (This)->lpVtbl -> GetUserContext(This,Index,ppunkUserCtx) ) 

#define IVsObjectList2_ShowHelp(This,Index)	\
    ( (This)->lpVtbl -> ShowHelp(This,Index) ) 

#define IVsObjectList2_GetSourceContext(This,Index,pszFileName,pulLineNum)	\
    ( (This)->lpVtbl -> GetSourceContext(This,Index,pszFileName,pulLineNum) ) 

#define IVsObjectList2_CountSourceItems(This,Index,ppHier,pitemid,pcItems)	\
    ( (This)->lpVtbl -> CountSourceItems(This,Index,ppHier,pitemid,pcItems) ) 

#define IVsObjectList2_GetMultipleSourceItems(This,Index,grfGSI,cItems,rgItemSel)	\
    ( (This)->lpVtbl -> GetMultipleSourceItems(This,Index,grfGSI,cItems,rgItemSel) ) 

#define IVsObjectList2_CanGoToSource(This,Index,SrcType,pfOK)	\
    ( (This)->lpVtbl -> CanGoToSource(This,Index,SrcType,pfOK) ) 

#define IVsObjectList2_GoToSource(This,Index,SrcType)	\
    ( (This)->lpVtbl -> GoToSource(This,Index,SrcType) ) 

#define IVsObjectList2_GetContextMenu(This,Index,pclsidActive,pnMenuId,ppCmdTrgtActive)	\
    ( (This)->lpVtbl -> GetContextMenu(This,Index,pclsidActive,pnMenuId,ppCmdTrgtActive) ) 

#define IVsObjectList2_QueryDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect)	\
    ( (This)->lpVtbl -> QueryDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect) ) 

#define IVsObjectList2_DoDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect)	\
    ( (This)->lpVtbl -> DoDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect) ) 

#define IVsObjectList2_CanRename(This,Index,pszNewName,pfOK)	\
    ( (This)->lpVtbl -> CanRename(This,Index,pszNewName,pfOK) ) 

#define IVsObjectList2_DoRename(This,Index,pszNewName,grfFlags)	\
    ( (This)->lpVtbl -> DoRename(This,Index,pszNewName,grfFlags) ) 

#define IVsObjectList2_CanDelete(This,Index,pfOK)	\
    ( (This)->lpVtbl -> CanDelete(This,Index,pfOK) ) 

#define IVsObjectList2_DoDelete(This,Index,grfFlags)	\
    ( (This)->lpVtbl -> DoDelete(This,Index,grfFlags) ) 

#define IVsObjectList2_FillDescription(This,Index,grfOptions,pobDesc)	\
    ( (This)->lpVtbl -> FillDescription(This,Index,grfOptions,pobDesc) ) 

#define IVsObjectList2_FillDescription2(This,Index,grfOptions,pobDesc)	\
    ( (This)->lpVtbl -> FillDescription2(This,Index,grfOptions,pobDesc) ) 

#define IVsObjectList2_EnumClipboardFormats(This,Index,grfFlags,celt,rgcfFormats,pcActual)	\
    ( (This)->lpVtbl -> EnumClipboardFormats(This,Index,grfFlags,celt,rgcfFormats,pcActual) ) 

#define IVsObjectList2_GetClipboardFormat(This,Index,grfFlags,pFormatetc,pMedium)	\
    ( (This)->lpVtbl -> GetClipboardFormat(This,Index,grfFlags,pFormatetc,pMedium) ) 

#define IVsObjectList2_GetExtendedClipboardVariant(This,Index,grfFlags,pcfFormat,pvarFormat)	\
    ( (This)->lpVtbl -> GetExtendedClipboardVariant(This,Index,grfFlags,pcfFormat,pvarFormat) ) 

#define IVsObjectList2_GetProperty(This,Index,propid,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,Index,propid,pvar) ) 

#define IVsObjectList2_GetNavInfo(This,Index,ppNavInfo)	\
    ( (This)->lpVtbl -> GetNavInfo(This,Index,ppNavInfo) ) 

#define IVsObjectList2_GetNavInfoNode(This,Index,ppNavInfoNode)	\
    ( (This)->lpVtbl -> GetNavInfoNode(This,Index,ppNavInfoNode) ) 

#define IVsObjectList2_LocateNavInfoNode(This,pNavInfoNode,pulIndex)	\
    ( (This)->lpVtbl -> LocateNavInfoNode(This,pNavInfoNode,pulIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsObjectList2_INTERFACE_DEFINED__ */


#ifndef __IVsSimpleObjectList2_INTERFACE_DEFINED__
#define __IVsSimpleObjectList2_INTERFACE_DEFINED__

/* interface IVsSimpleObjectList2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSimpleObjectList2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A0C6D693-8226-4ca6-AB03-557AA5A33F75")
    IVsSimpleObjectList2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFlags( 
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCapabilities2( 
            /* [out] */ __RPC__out LIB_LISTCAPABILITIES2 *pgrfCapabilities) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemCount( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetDisplayData( 
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextWithOwnership( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipTextWithOwnership( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCategoryField2( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pfCatField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBrowseObject( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispBrowseObj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserContext( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkUserCtx) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowHelp( 
            /* [in] */ ULONG Index) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceContextWithOwnership( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out ULONG *pulLineNum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CountSourceItems( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHier,
            /* [out] */ __RPC__out VSITEMID *pitemid,
            /* [retval][out] */ __RPC__out ULONG *pcItems) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMultipleSourceItems( 
            /* [in] */ ULONG Index,
            /* [in] */ VSGSIFLAGS grfGSI,
            /* [in] */ ULONG cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) VSITEMSELECTION rgItemSel[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanGoToSource( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GoToSource( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextMenu( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out CLSID *pclsidActive,
            /* [out] */ __RPC__out LONG *pnMenuId,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppCmdTrgtActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryDragDrop( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoDragDrop( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanRename( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoRename( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [in] */ VSOBJOPFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanDelete( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoDelete( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJOPFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FillDescription2( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription3 *pobDesc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumClipboardFormats( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSOBJCLIPFORMAT rgcfFormats[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetClipboardFormat( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in FORMATETC *pFormatetc,
            /* [in] */ __RPC__in STGMEDIUM *pMedium) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtendedClipboardVariant( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in const VSOBJCLIPFORMAT *pcfFormat,
            /* [out] */ __RPC__out VARIANT *pvarFormat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJLISTELEMPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavInfo( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavInfoNode( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfoNode **ppNavInfoNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateNavInfoNode( 
            /* [in] */ __RPC__in_opt IVsNavInfoNode *pNavInfoNode,
            /* [out] */ __RPC__out ULONG *pulIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandable3( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListTypeExcluded,
            /* [out] */ __RPC__out BOOL *pfExpandable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetList2( 
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleObjectList2 **ppIVsSimpleObjectList2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnClose( 
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSimpleObjectList2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSimpleObjectList2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSimpleObjectList2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFlags )( 
            IVsSimpleObjectList2 * This,
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetCapabilities2 )( 
            IVsSimpleObjectList2 * This,
            /* [out] */ __RPC__out LIB_LISTCAPABILITIES2 *pgrfCapabilities);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsSimpleObjectList2 * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemCount )( 
            IVsSimpleObjectList2 * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetDisplayData )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextWithOwnership )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipTextWithOwnership )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetCategoryField2 )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *GetBrowseObject )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispBrowseObj);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserContext )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkUserCtx);
        
        HRESULT ( STDMETHODCALLTYPE *ShowHelp )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceContextWithOwnership )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName,
            /* [out] */ __RPC__out ULONG *pulLineNum);
        
        HRESULT ( STDMETHODCALLTYPE *CountSourceItems )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHier,
            /* [out] */ __RPC__out VSITEMID *pitemid,
            /* [retval][out] */ __RPC__out ULONG *pcItems);
        
        HRESULT ( STDMETHODCALLTYPE *GetMultipleSourceItems )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSGSIFLAGS grfGSI,
            /* [in] */ ULONG cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) VSITEMSELECTION rgItemSel[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *CanGoToSource )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *GoToSource )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJGOTOSRCTYPE SrcType);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextMenu )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out CLSID *pclsidActive,
            /* [out] */ __RPC__out LONG *pnMenuId,
            /* [out] */ __RPC__deref_out_opt IOleCommandTarget **ppCmdTrgtActive);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDragDrop )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *DoDragDrop )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *CanRename )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *DoRename )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [in] */ VSOBJOPFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *CanDelete )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *DoDelete )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJOPFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *FillDescription2 )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJDESCOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsObjectBrowserDescription3 *pobDesc);
        
        HRESULT ( STDMETHODCALLTYPE *EnumClipboardFormats )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSOBJCLIPFORMAT rgcfFormats[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *GetClipboardFormat )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in FORMATETC *pFormatetc,
            /* [in] */ __RPC__in STGMEDIUM *pMedium);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtendedClipboardVariant )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJCFFLAGS grfFlags,
            /* [in] */ __RPC__in const VSOBJCLIPFORMAT *pcfFormat,
            /* [out] */ __RPC__out VARIANT *pvarFormat);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSOBJLISTELEMPROPID propid,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfo )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfoNode )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__deref_out_opt IVsNavInfoNode **ppNavInfoNode);
        
        HRESULT ( STDMETHODCALLTYPE *LocateNavInfoNode )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ __RPC__in_opt IVsNavInfoNode *pNavInfoNode,
            /* [out] */ __RPC__out ULONG *pulIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandable3 )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListTypeExcluded,
            /* [out] */ __RPC__out BOOL *pfExpandable);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsSimpleObjectList2 * This,
            /* [in] */ ULONG Index,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleObjectList2 **ppIVsSimpleObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *OnClose )( 
            IVsSimpleObjectList2 * This,
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca);
        
        END_INTERFACE
    } IVsSimpleObjectList2Vtbl;

    interface IVsSimpleObjectList2
    {
        CONST_VTBL struct IVsSimpleObjectList2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSimpleObjectList2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSimpleObjectList2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSimpleObjectList2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSimpleObjectList2_GetFlags(This,pFlags)	\
    ( (This)->lpVtbl -> GetFlags(This,pFlags) ) 

#define IVsSimpleObjectList2_GetCapabilities2(This,pgrfCapabilities)	\
    ( (This)->lpVtbl -> GetCapabilities2(This,pgrfCapabilities) ) 

#define IVsSimpleObjectList2_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 

#define IVsSimpleObjectList2_GetItemCount(This,pCount)	\
    ( (This)->lpVtbl -> GetItemCount(This,pCount) ) 

#define IVsSimpleObjectList2_GetDisplayData(This,Index,pData)	\
    ( (This)->lpVtbl -> GetDisplayData(This,Index,pData) ) 

#define IVsSimpleObjectList2_GetTextWithOwnership(This,Index,tto,pbstrText)	\
    ( (This)->lpVtbl -> GetTextWithOwnership(This,Index,tto,pbstrText) ) 

#define IVsSimpleObjectList2_GetTipTextWithOwnership(This,Index,eTipType,pbstrText)	\
    ( (This)->lpVtbl -> GetTipTextWithOwnership(This,Index,eTipType,pbstrText) ) 

#define IVsSimpleObjectList2_GetCategoryField2(This,Index,Category,pfCatField)	\
    ( (This)->lpVtbl -> GetCategoryField2(This,Index,Category,pfCatField) ) 

#define IVsSimpleObjectList2_GetBrowseObject(This,Index,ppdispBrowseObj)	\
    ( (This)->lpVtbl -> GetBrowseObject(This,Index,ppdispBrowseObj) ) 

#define IVsSimpleObjectList2_GetUserContext(This,Index,ppunkUserCtx)	\
    ( (This)->lpVtbl -> GetUserContext(This,Index,ppunkUserCtx) ) 

#define IVsSimpleObjectList2_ShowHelp(This,Index)	\
    ( (This)->lpVtbl -> ShowHelp(This,Index) ) 

#define IVsSimpleObjectList2_GetSourceContextWithOwnership(This,Index,pbstrFileName,pulLineNum)	\
    ( (This)->lpVtbl -> GetSourceContextWithOwnership(This,Index,pbstrFileName,pulLineNum) ) 

#define IVsSimpleObjectList2_CountSourceItems(This,Index,ppHier,pitemid,pcItems)	\
    ( (This)->lpVtbl -> CountSourceItems(This,Index,ppHier,pitemid,pcItems) ) 

#define IVsSimpleObjectList2_GetMultipleSourceItems(This,Index,grfGSI,cItems,rgItemSel)	\
    ( (This)->lpVtbl -> GetMultipleSourceItems(This,Index,grfGSI,cItems,rgItemSel) ) 

#define IVsSimpleObjectList2_CanGoToSource(This,Index,SrcType,pfOK)	\
    ( (This)->lpVtbl -> CanGoToSource(This,Index,SrcType,pfOK) ) 

#define IVsSimpleObjectList2_GoToSource(This,Index,SrcType)	\
    ( (This)->lpVtbl -> GoToSource(This,Index,SrcType) ) 

#define IVsSimpleObjectList2_GetContextMenu(This,Index,pclsidActive,pnMenuId,ppCmdTrgtActive)	\
    ( (This)->lpVtbl -> GetContextMenu(This,Index,pclsidActive,pnMenuId,ppCmdTrgtActive) ) 

#define IVsSimpleObjectList2_QueryDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect)	\
    ( (This)->lpVtbl -> QueryDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect) ) 

#define IVsSimpleObjectList2_DoDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect)	\
    ( (This)->lpVtbl -> DoDragDrop(This,Index,pDataObject,grfKeyState,pdwEffect) ) 

#define IVsSimpleObjectList2_CanRename(This,Index,pszNewName,pfOK)	\
    ( (This)->lpVtbl -> CanRename(This,Index,pszNewName,pfOK) ) 

#define IVsSimpleObjectList2_DoRename(This,Index,pszNewName,grfFlags)	\
    ( (This)->lpVtbl -> DoRename(This,Index,pszNewName,grfFlags) ) 

#define IVsSimpleObjectList2_CanDelete(This,Index,pfOK)	\
    ( (This)->lpVtbl -> CanDelete(This,Index,pfOK) ) 

#define IVsSimpleObjectList2_DoDelete(This,Index,grfFlags)	\
    ( (This)->lpVtbl -> DoDelete(This,Index,grfFlags) ) 

#define IVsSimpleObjectList2_FillDescription2(This,Index,grfOptions,pobDesc)	\
    ( (This)->lpVtbl -> FillDescription2(This,Index,grfOptions,pobDesc) ) 

#define IVsSimpleObjectList2_EnumClipboardFormats(This,Index,grfFlags,celt,rgcfFormats,pcActual)	\
    ( (This)->lpVtbl -> EnumClipboardFormats(This,Index,grfFlags,celt,rgcfFormats,pcActual) ) 

#define IVsSimpleObjectList2_GetClipboardFormat(This,Index,grfFlags,pFormatetc,pMedium)	\
    ( (This)->lpVtbl -> GetClipboardFormat(This,Index,grfFlags,pFormatetc,pMedium) ) 

#define IVsSimpleObjectList2_GetExtendedClipboardVariant(This,Index,grfFlags,pcfFormat,pvarFormat)	\
    ( (This)->lpVtbl -> GetExtendedClipboardVariant(This,Index,grfFlags,pcfFormat,pvarFormat) ) 

#define IVsSimpleObjectList2_GetProperty(This,Index,propid,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,Index,propid,pvar) ) 

#define IVsSimpleObjectList2_GetNavInfo(This,Index,ppNavInfo)	\
    ( (This)->lpVtbl -> GetNavInfo(This,Index,ppNavInfo) ) 

#define IVsSimpleObjectList2_GetNavInfoNode(This,Index,ppNavInfoNode)	\
    ( (This)->lpVtbl -> GetNavInfoNode(This,Index,ppNavInfoNode) ) 

#define IVsSimpleObjectList2_LocateNavInfoNode(This,pNavInfoNode,pulIndex)	\
    ( (This)->lpVtbl -> LocateNavInfoNode(This,pNavInfoNode,pulIndex) ) 

#define IVsSimpleObjectList2_GetExpandable3(This,Index,ListTypeExcluded,pfExpandable)	\
    ( (This)->lpVtbl -> GetExpandable3(This,Index,ListTypeExcluded,pfExpandable) ) 

#define IVsSimpleObjectList2_GetList2(This,Index,ListType,Flags,pobSrch,ppIVsSimpleObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,Index,ListType,Flags,pobSrch,ppIVsSimpleObjectList2) ) 

#define IVsSimpleObjectList2_OnClose(This,ptca)	\
    ( (This)->lpVtbl -> OnClose(This,ptca) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSimpleObjectList2_INTERFACE_DEFINED__ */


#ifndef __IVsBrowseContainersList_INTERFACE_DEFINED__
#define __IVsBrowseContainersList_INTERFACE_DEFINED__

/* interface IVsBrowseContainersList */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBrowseContainersList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("288F2A0C-B2E5-4799-9B9C-24E6EFCEFBF4")
    IVsBrowseContainersList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetContainerData( 
            /* [in] */ ULONG ulIndex,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindContainer( 
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pData,
            /* [out] */ __RPC__out ULONG *pulIndex) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBrowseContainersListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBrowseContainersList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBrowseContainersList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBrowseContainersList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetContainerData )( 
            IVsBrowseContainersList * This,
            /* [in] */ ULONG ulIndex,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pData);
        
        HRESULT ( STDMETHODCALLTYPE *FindContainer )( 
            IVsBrowseContainersList * This,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pData,
            /* [out] */ __RPC__out ULONG *pulIndex);
        
        END_INTERFACE
    } IVsBrowseContainersListVtbl;

    interface IVsBrowseContainersList
    {
        CONST_VTBL struct IVsBrowseContainersListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBrowseContainersList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBrowseContainersList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBrowseContainersList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBrowseContainersList_GetContainerData(This,ulIndex,pData)	\
    ( (This)->lpVtbl -> GetContainerData(This,ulIndex,pData) ) 

#define IVsBrowseContainersList_FindContainer(This,pData,pulIndex)	\
    ( (This)->lpVtbl -> FindContainer(This,pData,pulIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBrowseContainersList_INTERFACE_DEFINED__ */


#ifndef __IVsLibrary2_INTERFACE_DEFINED__
#define __IVsLibrary2_INTERFACE_DEFINED__

/* interface IVsLibrary2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLibrary2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EDD9F8A9-3FFE-4c4c-94F8-610B88E19160")
    IVsLibrary2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSupportedCategoryFields2( 
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetList2( 
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLibList( 
            /* [in] */ LIB_PERSISTTYPE lptType,
            /* [retval][out] */ __RPC__deref_out_opt IVsLiteTreeList **ppList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLibFlags2( 
            /* [retval][out] */ __RPC__out LIB_FLAGS2 *pgrfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGuid( 
            /* [out] */ __RPC__deref_out_opt const GUID **ppguidLib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSeparatorString( 
            /* [string][out] */ __RPC__deref_out_opt_string LPCWSTR *pszSeparator) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadState( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveState( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBrowseContainersForHierarchy( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSBROWSECONTAINER rgBrowseContainers[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddBrowseContainer( 
            /* [in] */ __RPC__in PVSCOMPONENTSELECTORDATA pcdComponent,
            /* [out][in] */ __RPC__inout LIB_ADDREMOVEOPTIONS *pgrfOptions,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrComponentAdded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveBrowseContainer( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCWSTR pszLibName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNavInfo( 
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLibrary2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLibrary2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLibrary2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLibrary2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedCategoryFields2 )( 
            IVsLibrary2 * This,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsLibrary2 * This,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetLibList )( 
            IVsLibrary2 * This,
            /* [in] */ LIB_PERSISTTYPE lptType,
            /* [retval][out] */ __RPC__deref_out_opt IVsLiteTreeList **ppList);
        
        HRESULT ( STDMETHODCALLTYPE *GetLibFlags2 )( 
            IVsLibrary2 * This,
            /* [retval][out] */ __RPC__out LIB_FLAGS2 *pgrfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsLibrary2 * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *GetGuid )( 
            IVsLibrary2 * This,
            /* [out] */ __RPC__deref_out_opt const GUID **ppguidLib);
        
        HRESULT ( STDMETHODCALLTYPE *GetSeparatorString )( 
            IVsLibrary2 * This,
            /* [string][out] */ __RPC__deref_out_opt_string LPCWSTR *pszSeparator);
        
        HRESULT ( STDMETHODCALLTYPE *LoadState )( 
            IVsLibrary2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType);
        
        HRESULT ( STDMETHODCALLTYPE *SaveState )( 
            IVsLibrary2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType);
        
        HRESULT ( STDMETHODCALLTYPE *GetBrowseContainersForHierarchy )( 
            IVsLibrary2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSBROWSECONTAINER rgBrowseContainers[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *AddBrowseContainer )( 
            IVsLibrary2 * This,
            /* [in] */ __RPC__in PVSCOMPONENTSELECTORDATA pcdComponent,
            /* [out][in] */ __RPC__inout LIB_ADDREMOVEOPTIONS *pgrfOptions,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrComponentAdded);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveBrowseContainer )( 
            IVsLibrary2 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCWSTR pszLibName);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo )( 
            IVsLibrary2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        END_INTERFACE
    } IVsLibrary2Vtbl;

    interface IVsLibrary2
    {
        CONST_VTBL struct IVsLibrary2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLibrary2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLibrary2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLibrary2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLibrary2_GetSupportedCategoryFields2(This,Category,pgrfCatField)	\
    ( (This)->lpVtbl -> GetSupportedCategoryFields2(This,Category,pgrfCatField) ) 

#define IVsLibrary2_GetList2(This,ListType,Flags,pobSrch,ppIVsObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,ListType,Flags,pobSrch,ppIVsObjectList2) ) 

#define IVsLibrary2_GetLibList(This,lptType,ppList)	\
    ( (This)->lpVtbl -> GetLibList(This,lptType,ppList) ) 

#define IVsLibrary2_GetLibFlags2(This,pgrfFlags)	\
    ( (This)->lpVtbl -> GetLibFlags2(This,pgrfFlags) ) 

#define IVsLibrary2_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 

#define IVsLibrary2_GetGuid(This,ppguidLib)	\
    ( (This)->lpVtbl -> GetGuid(This,ppguidLib) ) 

#define IVsLibrary2_GetSeparatorString(This,pszSeparator)	\
    ( (This)->lpVtbl -> GetSeparatorString(This,pszSeparator) ) 

#define IVsLibrary2_LoadState(This,pIStream,lptType)	\
    ( (This)->lpVtbl -> LoadState(This,pIStream,lptType) ) 

#define IVsLibrary2_SaveState(This,pIStream,lptType)	\
    ( (This)->lpVtbl -> SaveState(This,pIStream,lptType) ) 

#define IVsLibrary2_GetBrowseContainersForHierarchy(This,pHierarchy,celt,rgBrowseContainers,pcActual)	\
    ( (This)->lpVtbl -> GetBrowseContainersForHierarchy(This,pHierarchy,celt,rgBrowseContainers,pcActual) ) 

#define IVsLibrary2_AddBrowseContainer(This,pcdComponent,pgrfOptions,pbstrComponentAdded)	\
    ( (This)->lpVtbl -> AddBrowseContainer(This,pcdComponent,pgrfOptions,pbstrComponentAdded) ) 

#define IVsLibrary2_RemoveBrowseContainer(This,dwReserved,pszLibName)	\
    ( (This)->lpVtbl -> RemoveBrowseContainer(This,dwReserved,pszLibName) ) 

#define IVsLibrary2_CreateNavInfo(This,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo(This,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLibrary2_INTERFACE_DEFINED__ */


#ifndef __IVsSimpleLibrary2_INTERFACE_DEFINED__
#define __IVsSimpleLibrary2_INTERFACE_DEFINED__

/* interface IVsSimpleLibrary2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSimpleLibrary2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2F328444-6E74-48b4-8B95-08015F9D65D9")
    IVsSimpleLibrary2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSupportedCategoryFields2( 
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetList2( 
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleObjectList2 **ppIVsSimpleObjectList2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLibFlags2( 
            /* [retval][out] */ __RPC__out LIB_FLAGS2 *pgrfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGuid( 
            /* [out] */ __RPC__out GUID *pguidLib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSeparatorStringWithOwnership( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSeparator) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadState( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveState( 
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBrowseContainersForHierarchy( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSBROWSECONTAINER rgBrowseContainers[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddBrowseContainer( 
            /* [in] */ __RPC__in PVSCOMPONENTSELECTORDATA pcdComponent,
            /* [out][in] */ __RPC__inout LIB_ADDREMOVEOPTIONS *pgrfOptions,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrComponentAdded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveBrowseContainer( 
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCWSTR pszLibName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNavInfo( 
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSimpleLibrary2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSimpleLibrary2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSimpleLibrary2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedCategoryFields2 )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleObjectList2 **ppIVsSimpleObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetLibFlags2 )( 
            IVsSimpleLibrary2 * This,
            /* [retval][out] */ __RPC__out LIB_FLAGS2 *pgrfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsSimpleLibrary2 * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *GetGuid )( 
            IVsSimpleLibrary2 * This,
            /* [out] */ __RPC__out GUID *pguidLib);
        
        HRESULT ( STDMETHODCALLTYPE *GetSeparatorStringWithOwnership )( 
            IVsSimpleLibrary2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSeparator);
        
        HRESULT ( STDMETHODCALLTYPE *LoadState )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType);
        
        HRESULT ( STDMETHODCALLTYPE *SaveState )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ __RPC__in_opt IStream *pIStream,
            /* [in] */ LIB_PERSISTTYPE lptType);
        
        HRESULT ( STDMETHODCALLTYPE *GetBrowseContainersForHierarchy )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSBROWSECONTAINER rgBrowseContainers[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *AddBrowseContainer )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ __RPC__in PVSCOMPONENTSELECTORDATA pcdComponent,
            /* [out][in] */ __RPC__inout LIB_ADDREMOVEOPTIONS *pgrfOptions,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrComponentAdded);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveBrowseContainer )( 
            IVsSimpleLibrary2 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ __RPC__in LPCWSTR pszLibName);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo )( 
            IVsSimpleLibrary2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        END_INTERFACE
    } IVsSimpleLibrary2Vtbl;

    interface IVsSimpleLibrary2
    {
        CONST_VTBL struct IVsSimpleLibrary2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSimpleLibrary2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSimpleLibrary2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSimpleLibrary2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSimpleLibrary2_GetSupportedCategoryFields2(This,Category,pgrfCatField)	\
    ( (This)->lpVtbl -> GetSupportedCategoryFields2(This,Category,pgrfCatField) ) 

#define IVsSimpleLibrary2_GetList2(This,ListType,Flags,pobSrch,ppIVsSimpleObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,ListType,Flags,pobSrch,ppIVsSimpleObjectList2) ) 

#define IVsSimpleLibrary2_GetLibFlags2(This,pgrfFlags)	\
    ( (This)->lpVtbl -> GetLibFlags2(This,pgrfFlags) ) 

#define IVsSimpleLibrary2_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 

#define IVsSimpleLibrary2_GetGuid(This,pguidLib)	\
    ( (This)->lpVtbl -> GetGuid(This,pguidLib) ) 

#define IVsSimpleLibrary2_GetSeparatorStringWithOwnership(This,pbstrSeparator)	\
    ( (This)->lpVtbl -> GetSeparatorStringWithOwnership(This,pbstrSeparator) ) 

#define IVsSimpleLibrary2_LoadState(This,pIStream,lptType)	\
    ( (This)->lpVtbl -> LoadState(This,pIStream,lptType) ) 

#define IVsSimpleLibrary2_SaveState(This,pIStream,lptType)	\
    ( (This)->lpVtbl -> SaveState(This,pIStream,lptType) ) 

#define IVsSimpleLibrary2_GetBrowseContainersForHierarchy(This,pHierarchy,celt,rgBrowseContainers,pcActual)	\
    ( (This)->lpVtbl -> GetBrowseContainersForHierarchy(This,pHierarchy,celt,rgBrowseContainers,pcActual) ) 

#define IVsSimpleLibrary2_AddBrowseContainer(This,pcdComponent,pgrfOptions,pbstrComponentAdded)	\
    ( (This)->lpVtbl -> AddBrowseContainer(This,pcdComponent,pgrfOptions,pbstrComponentAdded) ) 

#define IVsSimpleLibrary2_RemoveBrowseContainer(This,dwReserved,pszLibName)	\
    ( (This)->lpVtbl -> RemoveBrowseContainer(This,dwReserved,pszLibName) ) 

#define IVsSimpleLibrary2_CreateNavInfo(This,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo(This,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSimpleLibrary2_INTERFACE_DEFINED__ */


#ifndef __IVsLibrary2Ex_INTERFACE_DEFINED__
#define __IVsLibrary2Ex_INTERFACE_DEFINED__

/* interface IVsLibrary2Ex */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLibrary2Ex;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D9C7D24D-7ED2-4a9d-93D1-450426CB27DF")
    IVsLibrary2Ex : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ProfileSettingsChanged( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavInfoContainerData( 
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdComponent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoIdle( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetContainerAsUnchanging( 
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [in] */ BOOL fUnchanging) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsLibrary2ExVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsLibrary2Ex * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsLibrary2Ex * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsLibrary2Ex * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProfileSettingsChanged )( 
            IVsLibrary2Ex * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfoContainerData )( 
            IVsLibrary2Ex * This,
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdComponent);
        
        HRESULT ( STDMETHODCALLTYPE *DoIdle )( 
            IVsLibrary2Ex * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetContainerAsUnchanging )( 
            IVsLibrary2Ex * This,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [in] */ BOOL fUnchanging);
        
        END_INTERFACE
    } IVsLibrary2ExVtbl;

    interface IVsLibrary2Ex
    {
        CONST_VTBL struct IVsLibrary2ExVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLibrary2Ex_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLibrary2Ex_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLibrary2Ex_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLibrary2Ex_ProfileSettingsChanged(This)	\
    ( (This)->lpVtbl -> ProfileSettingsChanged(This) ) 

#define IVsLibrary2Ex_GetNavInfoContainerData(This,pNavInfo,pcsdComponent)	\
    ( (This)->lpVtbl -> GetNavInfoContainerData(This,pNavInfo,pcsdComponent) ) 

#define IVsLibrary2Ex_DoIdle(This)	\
    ( (This)->lpVtbl -> DoIdle(This) ) 

#define IVsLibrary2Ex_SetContainerAsUnchanging(This,pcsdComponent,fUnchanging)	\
    ( (This)->lpVtbl -> SetContainerAsUnchanging(This,pcsdComponent,fUnchanging) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLibrary2Ex_INTERFACE_DEFINED__ */


#ifndef __IVsEnumLibraries2_INTERFACE_DEFINED__
#define __IVsEnumLibraries2_INTERFACE_DEFINED__

/* interface IVsEnumLibraries2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumLibraries2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FE2BB01D-6EBA-4796-ADF9-59EC7D100AC2")
    IVsEnumLibraries2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsLibrary2 **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumLibraries2 **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumLibraries2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumLibraries2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumLibraries2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumLibraries2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumLibraries2 * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsLibrary2 **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumLibraries2 * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumLibraries2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumLibraries2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumLibraries2 **ppenum);
        
        END_INTERFACE
    } IVsEnumLibraries2Vtbl;

    interface IVsEnumLibraries2
    {
        CONST_VTBL struct IVsEnumLibraries2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumLibraries2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumLibraries2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumLibraries2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumLibraries2_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumLibraries2_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumLibraries2_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumLibraries2_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumLibraries2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0088 */
/* [local] */ 






enum _BROWSE_COMPONENT_SET_TYPE
    {	BCST_EXCLUDE_LIBRARIES	= 0,
	BCST_INCLUDE_LIBRARIES	= 0x1
    } ;
typedef DWORD BROWSE_COMPONENT_SET_TYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0088_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0088_v0_0_s_ifspec;

#ifndef __IVsObjectManager2_INTERFACE_DEFINED__
#define __IVsObjectManager2_INTERFACE_DEFINED__

/* interface IVsObjectManager2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsObjectManager2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6A0392E4-68E8-4fbc-AFCF-85155533E48E")
    IVsObjectManager2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterLibrary( 
            /* [in] */ __RPC__in_opt IVsLibrary2 *pLib,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterSimpleLibrary( 
            /* [in] */ __RPC__in_opt IVsSimpleLibrary2 *pLib,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterLibrary( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumLibraries( 
            /* [out] */ __RPC__deref_out_opt IVsEnumLibraries2 **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindLibrary( 
            /* [in] */ __RPC__in REFGUID guidLibrary,
            /* [out] */ __RPC__deref_out_opt IVsLibrary2 **ppLib) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetListAndIndex( 
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsObjectList2 **ppList,
            /* [out] */ __RPC__out ULONG *pIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ParseDataObject( 
            /* [in] */ __RPC__in_opt IDataObject *pIDataObject,
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbols **ppSymbols) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateSimpleBrowseComponentSet( 
            /* [in] */ BROWSE_COMPONENT_SET_TYPE Type,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcLibs) const GUID rgguidLibs[  ],
            /* [in] */ ULONG ulcLibs,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateProjectReferenceSet( 
            /* [in] */ __RPC__in_opt IUnknown *pProject,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateCombinedBrowseComponentSet( 
            /* [retval][out] */ __RPC__deref_out_opt IVsCombinedBrowseComponentSet **ppCombinedSet) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsObjectManager2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsObjectManager2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsObjectManager2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterLibrary )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in_opt IVsLibrary2 *pLib,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSimpleLibrary )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in_opt IVsSimpleLibrary2 *pLib,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterLibrary )( 
            IVsObjectManager2 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *EnumLibraries )( 
            IVsObjectManager2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumLibraries2 **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *FindLibrary )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in REFGUID guidLibrary,
            /* [out] */ __RPC__deref_out_opt IVsLibrary2 **ppLib);
        
        HRESULT ( STDMETHODCALLTYPE *GetListAndIndex )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsObjectList2 **ppList,
            /* [out] */ __RPC__out ULONG *pIndex);
        
        HRESULT ( STDMETHODCALLTYPE *ParseDataObject )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pIDataObject,
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbols **ppSymbols);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSimpleBrowseComponentSet )( 
            IVsObjectManager2 * This,
            /* [in] */ BROWSE_COMPONENT_SET_TYPE Type,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcLibs) const GUID rgguidLibs[  ],
            /* [in] */ ULONG ulcLibs,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet);
        
        HRESULT ( STDMETHODCALLTYPE *CreateProjectReferenceSet )( 
            IVsObjectManager2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pProject,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet);
        
        HRESULT ( STDMETHODCALLTYPE *CreateCombinedBrowseComponentSet )( 
            IVsObjectManager2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsCombinedBrowseComponentSet **ppCombinedSet);
        
        END_INTERFACE
    } IVsObjectManager2Vtbl;

    interface IVsObjectManager2
    {
        CONST_VTBL struct IVsObjectManager2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsObjectManager2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsObjectManager2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsObjectManager2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsObjectManager2_RegisterLibrary(This,pLib,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterLibrary(This,pLib,pdwCookie) ) 

#define IVsObjectManager2_RegisterSimpleLibrary(This,pLib,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterSimpleLibrary(This,pLib,pdwCookie) ) 

#define IVsObjectManager2_UnregisterLibrary(This,dwCookie)	\
    ( (This)->lpVtbl -> UnregisterLibrary(This,dwCookie) ) 

#define IVsObjectManager2_EnumLibraries(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumLibraries(This,ppEnum) ) 

#define IVsObjectManager2_FindLibrary(This,guidLibrary,ppLib)	\
    ( (This)->lpVtbl -> FindLibrary(This,guidLibrary,ppLib) ) 

#define IVsObjectManager2_GetListAndIndex(This,pNavInfo,dwFlags,ppList,pIndex)	\
    ( (This)->lpVtbl -> GetListAndIndex(This,pNavInfo,dwFlags,ppList,pIndex) ) 

#define IVsObjectManager2_ParseDataObject(This,pIDataObject,ppSymbols)	\
    ( (This)->lpVtbl -> ParseDataObject(This,pIDataObject,ppSymbols) ) 

#define IVsObjectManager2_CreateSimpleBrowseComponentSet(This,Type,rgguidLibs,ulcLibs,ppSet)	\
    ( (This)->lpVtbl -> CreateSimpleBrowseComponentSet(This,Type,rgguidLibs,ulcLibs,ppSet) ) 

#define IVsObjectManager2_CreateProjectReferenceSet(This,pProject,ppSet)	\
    ( (This)->lpVtbl -> CreateProjectReferenceSet(This,pProject,ppSet) ) 

#define IVsObjectManager2_CreateCombinedBrowseComponentSet(This,ppCombinedSet)	\
    ( (This)->lpVtbl -> CreateCombinedBrowseComponentSet(This,ppCombinedSet) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsObjectManager2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0089 */
/* [local] */ 

DEFINE_GUID(GUID_VB_BrowseLibrary, 0x414AC972, 0x9829, 0x4B6A, 0xA8, 0xD7, 0xA0, 0x81, 0x52, 0xFE, 0xB8, 0xAA);
DEFINE_GUID(GUID_CSharp_BrowseLibrary, 0x58f1bad0, 0x2288, 0x45b9, 0xac, 0x3a, 0xd5, 0x63, 0x98, 0xf7, 0x78, 0x1d);
DEFINE_GUID(GUID_VJSharp_BrowseLibrary, 0x7B1DC85B, 0xE430, 0x4187, 0x81, 0x77, 0xEF, 0x97, 0xDD, 0x39, 0x0D, 0x9A);
DEFINE_GUID(GUID_VC_BrowseLibrary, 0x6c1ac90e, 0x09fc, 0x4f23, 0x90, 0xff, 0x87, 0xf8, 0xcf, 0xc2, 0xa4, 0x45);
DEFINE_GUID(GUID_BSC_BrowseLibrary, 0x26e73a17, 0x0d6c, 0x4a33, 0xb8, 0x33, 0x22, 0xc7, 0x6c, 0x50, 0x94, 0x9f);
DEFINE_GUID(GUID_Assembly_BrowseLibrary, 0x1ec72fd7, 0xc820, 0x4273, 0x9a, 0x21, 0x77, 0x7a, 0x5c, 0x52, 0x2e, 0x03);
DEFINE_GUID(GUID_TypeLib_BrowseLibrary, 0x18e32c04, 0x58ba, 0x4a1e, 0x80, 0xde, 0x1c, 0x29, 0x16, 0x34, 0x16, 0x6a);
DEFINE_GUID(GUID_Folder_BrowseLibrary, 0xdc534e0e, 0xefbe, 0x4d0c, 0x8a, 0x25, 0x98, 0xbf, 0x2, 0x9f, 0x15, 0xf8);
DEFINE_GUID(GUID_ResourceView_BrowseLibrary, 0xD22514E7, 0x23AF, 0x4723, 0xB6, 0xE5, 0xE1, 0x7D, 0x27, 0x62, 0x6D, 0x34);

enum _BROWSE_COMPONENT_SET_OPTIONS
    {	BCSO_NO_REMOVE	= 0x1,
	BCSO_NO_RENAME	= 0x2,
	BCSO_NO_DRAG_DROP	= 0x4,
	BCSO_PROJECT_REFERENCES	= 0x8
    } ;
#define BCSO_NO_EDIT  (BCSO_NO_REMOVE | BCSO_NO_RENAME | BCSO_NO_DRAG_DROP)
typedef DWORD BROWSE_COMPONENT_SET_OPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0089_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0089_v0_0_s_ifspec;

#ifndef __IVsBrowseComponentSet_INTERFACE_DEFINED__
#define __IVsBrowseComponentSet_INTERFACE_DEFINED__

/* interface IVsBrowseComponentSet */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBrowseComponentSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("804DCBDE-3A63-4c3c-9316-296C4C7E9140")
    IVsBrowseComponentSet : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE put_ComponentsListOptions( 
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_ComponentsListOptions( 
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE put_ChildListOptions( 
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_ChildListOptions( 
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetList2( 
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [in] */ __RPC__in_opt IVsObjectList2 *pExtraListToCombineWith,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSupportedCategoryFields2( 
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNavInfo( 
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBrowseComponentSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBrowseComponentSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBrowseComponentSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBrowseComponentSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *put_ComponentsListOptions )( 
            IVsBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ComponentsListOptions )( 
            IVsBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *put_ChildListOptions )( 
            IVsBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ChildListOptions )( 
            IVsBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsBrowseComponentSet * This,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [in] */ __RPC__in_opt IVsObjectList2 *pExtraListToCombineWith,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedCategoryFields2 )( 
            IVsBrowseComponentSet * This,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo )( 
            IVsBrowseComponentSet * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsBrowseComponentSet * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        END_INTERFACE
    } IVsBrowseComponentSetVtbl;

    interface IVsBrowseComponentSet
    {
        CONST_VTBL struct IVsBrowseComponentSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBrowseComponentSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBrowseComponentSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBrowseComponentSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBrowseComponentSet_put_ComponentsListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ComponentsListOptions(This,dwOptions) ) 

#define IVsBrowseComponentSet_get_ComponentsListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ComponentsListOptions(This,pdwOptions) ) 

#define IVsBrowseComponentSet_put_ChildListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ChildListOptions(This,dwOptions) ) 

#define IVsBrowseComponentSet_get_ChildListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ChildListOptions(This,pdwOptions) ) 

#define IVsBrowseComponentSet_GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2) ) 

#define IVsBrowseComponentSet_GetSupportedCategoryFields2(This,Category,pgrfCatField)	\
    ( (This)->lpVtbl -> GetSupportedCategoryFields2(This,Category,pgrfCatField) ) 

#define IVsBrowseComponentSet_CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#define IVsBrowseComponentSet_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBrowseComponentSet_INTERFACE_DEFINED__ */


#ifndef __IVsSimpleBrowseComponentSet_INTERFACE_DEFINED__
#define __IVsSimpleBrowseComponentSet_INTERFACE_DEFINED__

/* interface IVsSimpleBrowseComponentSet */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSimpleBrowseComponentSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B027F23C-E6B9-415c-ACF0-3D7CFAFCC662")
    IVsSimpleBrowseComponentSet : public IVsBrowseComponentSet
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE put_RootNavInfo( 
            /* [in] */ __RPC__in_opt IVsNavInfo *pRootNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_RootNavInfo( 
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **pRootNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE put_Owner( 
            /* [in] */ __RPC__in_opt IUnknown *pOwner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Owner( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppOwner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindComponent( 
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppRealLibNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdExistingComponent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddComponent( 
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppRealLibNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdAddedComponent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveComponent( 
            /* [in] */ __RPC__in_opt IVsNavInfo *pRealLibNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAllComponents( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSimpleBrowseComponentSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSimpleBrowseComponentSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSimpleBrowseComponentSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *put_ComponentsListOptions )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ComponentsListOptions )( 
            IVsSimpleBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *put_ChildListOptions )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ChildListOptions )( 
            IVsSimpleBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [in] */ __RPC__in_opt IVsObjectList2 *pExtraListToCombineWith,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedCategoryFields2 )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsSimpleBrowseComponentSet * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *put_RootNavInfo )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in_opt IVsNavInfo *pRootNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *get_RootNavInfo )( 
            IVsSimpleBrowseComponentSet * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **pRootNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *put_Owner )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in_opt IUnknown *pOwner);
        
        HRESULT ( STDMETHODCALLTYPE *get_Owner )( 
            IVsSimpleBrowseComponentSet * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppOwner);
        
        HRESULT ( STDMETHODCALLTYPE *FindComponent )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppRealLibNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdExistingComponent);
        
        HRESULT ( STDMETHODCALLTYPE *AddComponent )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [in] */ __RPC__in VSCOMPONENTSELECTORDATA *pcsdComponent,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppRealLibNavInfo,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pcsdAddedComponent);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveComponent )( 
            IVsSimpleBrowseComponentSet * This,
            /* [in] */ __RPC__in_opt IVsNavInfo *pRealLibNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllComponents )( 
            IVsSimpleBrowseComponentSet * This);
        
        END_INTERFACE
    } IVsSimpleBrowseComponentSetVtbl;

    interface IVsSimpleBrowseComponentSet
    {
        CONST_VTBL struct IVsSimpleBrowseComponentSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSimpleBrowseComponentSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSimpleBrowseComponentSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSimpleBrowseComponentSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSimpleBrowseComponentSet_put_ComponentsListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ComponentsListOptions(This,dwOptions) ) 

#define IVsSimpleBrowseComponentSet_get_ComponentsListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ComponentsListOptions(This,pdwOptions) ) 

#define IVsSimpleBrowseComponentSet_put_ChildListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ChildListOptions(This,dwOptions) ) 

#define IVsSimpleBrowseComponentSet_get_ChildListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ChildListOptions(This,pdwOptions) ) 

#define IVsSimpleBrowseComponentSet_GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2) ) 

#define IVsSimpleBrowseComponentSet_GetSupportedCategoryFields2(This,Category,pgrfCatField)	\
    ( (This)->lpVtbl -> GetSupportedCategoryFields2(This,Category,pgrfCatField) ) 

#define IVsSimpleBrowseComponentSet_CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#define IVsSimpleBrowseComponentSet_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 


#define IVsSimpleBrowseComponentSet_put_RootNavInfo(This,pRootNavInfo)	\
    ( (This)->lpVtbl -> put_RootNavInfo(This,pRootNavInfo) ) 

#define IVsSimpleBrowseComponentSet_get_RootNavInfo(This,pRootNavInfo)	\
    ( (This)->lpVtbl -> get_RootNavInfo(This,pRootNavInfo) ) 

#define IVsSimpleBrowseComponentSet_put_Owner(This,pOwner)	\
    ( (This)->lpVtbl -> put_Owner(This,pOwner) ) 

#define IVsSimpleBrowseComponentSet_get_Owner(This,ppOwner)	\
    ( (This)->lpVtbl -> get_Owner(This,ppOwner) ) 

#define IVsSimpleBrowseComponentSet_FindComponent(This,guidLib,pcsdComponent,ppRealLibNavInfo,pcsdExistingComponent)	\
    ( (This)->lpVtbl -> FindComponent(This,guidLib,pcsdComponent,ppRealLibNavInfo,pcsdExistingComponent) ) 

#define IVsSimpleBrowseComponentSet_AddComponent(This,guidLib,pcsdComponent,ppRealLibNavInfo,pcsdAddedComponent)	\
    ( (This)->lpVtbl -> AddComponent(This,guidLib,pcsdComponent,ppRealLibNavInfo,pcsdAddedComponent) ) 

#define IVsSimpleBrowseComponentSet_RemoveComponent(This,pRealLibNavInfo)	\
    ( (This)->lpVtbl -> RemoveComponent(This,pRealLibNavInfo) ) 

#define IVsSimpleBrowseComponentSet_RemoveAllComponents(This)	\
    ( (This)->lpVtbl -> RemoveAllComponents(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSimpleBrowseComponentSet_INTERFACE_DEFINED__ */


#ifndef __IVsCombinedBrowseComponentSet_INTERFACE_DEFINED__
#define __IVsCombinedBrowseComponentSet_INTERFACE_DEFINED__

/* interface IVsCombinedBrowseComponentSet */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCombinedBrowseComponentSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("64CBD015-9D4B-4daf-8801-68EDA90B98C5")
    IVsCombinedBrowseComponentSet : public IVsBrowseComponentSet
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddSet( 
            /* [in] */ __RPC__in_opt IVsSimpleBrowseComponentSet *pSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSetCount( 
            /* [in] */ __RPC__in ULONG *pulCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSetAt( 
            /* [in] */ ULONG ulIndex,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSetAt( 
            /* [in] */ ULONG ulIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAllSets( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveOwnerSets( 
            /* [in] */ __RPC__in_opt IUnknown *pOwner) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCombinedBrowseComponentSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCombinedBrowseComponentSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCombinedBrowseComponentSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *put_ComponentsListOptions )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ComponentsListOptions )( 
            IVsCombinedBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *put_ChildListOptions )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ BROWSE_COMPONENT_SET_OPTIONS dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *get_ChildListOptions )( 
            IVsCombinedBrowseComponentSet * This,
            /* [retval][out] */ __RPC__out BROWSE_COMPONENT_SET_OPTIONS *pdwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetList2 )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ LIB_LISTTYPE2 ListType,
            /* [in] */ LIB_LISTFLAGS Flags,
            /* [in] */ __RPC__in VSOBSEARCHCRITERIA2 *pobSrch,
            /* [in] */ __RPC__in_opt IVsObjectList2 *pExtraListToCombineWith,
            /* [retval][out] */ __RPC__deref_out_opt IVsObjectList2 **ppIVsObjectList2);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedCategoryFields2 )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ LIB_CATEGORY2 Category,
            /* [retval][out] */ __RPC__out DWORD *pgrfCatField);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [retval][out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsCombinedBrowseComponentSet * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate);
        
        HRESULT ( STDMETHODCALLTYPE *AddSet )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ __RPC__in_opt IVsSimpleBrowseComponentSet *pSet);
        
        HRESULT ( STDMETHODCALLTYPE *GetSetCount )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ __RPC__in ULONG *pulCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetSetAt )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ ULONG ulIndex,
            /* [retval][out] */ __RPC__deref_out_opt IVsSimpleBrowseComponentSet **ppSet);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSetAt )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ ULONG ulIndex);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAllSets )( 
            IVsCombinedBrowseComponentSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveOwnerSets )( 
            IVsCombinedBrowseComponentSet * This,
            /* [in] */ __RPC__in_opt IUnknown *pOwner);
        
        END_INTERFACE
    } IVsCombinedBrowseComponentSetVtbl;

    interface IVsCombinedBrowseComponentSet
    {
        CONST_VTBL struct IVsCombinedBrowseComponentSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCombinedBrowseComponentSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCombinedBrowseComponentSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCombinedBrowseComponentSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCombinedBrowseComponentSet_put_ComponentsListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ComponentsListOptions(This,dwOptions) ) 

#define IVsCombinedBrowseComponentSet_get_ComponentsListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ComponentsListOptions(This,pdwOptions) ) 

#define IVsCombinedBrowseComponentSet_put_ChildListOptions(This,dwOptions)	\
    ( (This)->lpVtbl -> put_ChildListOptions(This,dwOptions) ) 

#define IVsCombinedBrowseComponentSet_get_ChildListOptions(This,pdwOptions)	\
    ( (This)->lpVtbl -> get_ChildListOptions(This,pdwOptions) ) 

#define IVsCombinedBrowseComponentSet_GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2)	\
    ( (This)->lpVtbl -> GetList2(This,ListType,Flags,pobSrch,pExtraListToCombineWith,ppIVsObjectList2) ) 

#define IVsCombinedBrowseComponentSet_GetSupportedCategoryFields2(This,Category,pgrfCatField)	\
    ( (This)->lpVtbl -> GetSupportedCategoryFields2(This,Category,pgrfCatField) ) 

#define IVsCombinedBrowseComponentSet_CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo(This,guidLib,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#define IVsCombinedBrowseComponentSet_UpdateCounter(This,pCurUpdate)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate) ) 


#define IVsCombinedBrowseComponentSet_AddSet(This,pSet)	\
    ( (This)->lpVtbl -> AddSet(This,pSet) ) 

#define IVsCombinedBrowseComponentSet_GetSetCount(This,pulCount)	\
    ( (This)->lpVtbl -> GetSetCount(This,pulCount) ) 

#define IVsCombinedBrowseComponentSet_GetSetAt(This,ulIndex,ppSet)	\
    ( (This)->lpVtbl -> GetSetAt(This,ulIndex,ppSet) ) 

#define IVsCombinedBrowseComponentSet_RemoveSetAt(This,ulIndex)	\
    ( (This)->lpVtbl -> RemoveSetAt(This,ulIndex) ) 

#define IVsCombinedBrowseComponentSet_RemoveAllSets(This)	\
    ( (This)->lpVtbl -> RemoveAllSets(This) ) 

#define IVsCombinedBrowseComponentSet_RemoveOwnerSets(This,pOwner)	\
    ( (This)->lpVtbl -> RemoveOwnerSets(This,pOwner) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCombinedBrowseComponentSet_INTERFACE_DEFINED__ */


#ifndef __IVsSelectedSymbol_INTERFACE_DEFINED__
#define __IVsSelectedSymbol_INTERFACE_DEFINED__

/* interface IVsSelectedSymbol */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSelectedSymbol;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9D86535E-5FE7-4aaf-8846-F1FB5556109A")
    IVsSelectedSymbol : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNavInfo( 
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSelectedSymbolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSelectedSymbol * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSelectedSymbol * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSelectedSymbol * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavInfo )( 
            IVsSelectedSymbol * This,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo **ppNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsSelectedSymbol * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        END_INTERFACE
    } IVsSelectedSymbolVtbl;

    interface IVsSelectedSymbol
    {
        CONST_VTBL struct IVsSelectedSymbolVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSelectedSymbol_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSelectedSymbol_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSelectedSymbol_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSelectedSymbol_GetNavInfo(This,ppNavInfo)	\
    ( (This)->lpVtbl -> GetNavInfo(This,ppNavInfo) ) 

#define IVsSelectedSymbol_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSelectedSymbol_INTERFACE_DEFINED__ */


#ifndef __IVsEnumSelectedSymbols_INTERFACE_DEFINED__
#define __IVsEnumSelectedSymbols_INTERFACE_DEFINED__

/* interface IVsEnumSelectedSymbols */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumSelectedSymbols;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CAC67C59-301A-415d-B269-790F7D4731BC")
    IVsEnumSelectedSymbols : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsSelectedSymbol **rgpIVsSelectedSymbol,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumSelectedSymbols **ppIVsEnumSelectedSymbols) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumSelectedSymbolsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumSelectedSymbols * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumSelectedSymbols * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumSelectedSymbols * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumSelectedSymbols * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsSelectedSymbol **rgpIVsSelectedSymbol,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumSelectedSymbols * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumSelectedSymbols * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumSelectedSymbols * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumSelectedSymbols **ppIVsEnumSelectedSymbols);
        
        END_INTERFACE
    } IVsEnumSelectedSymbolsVtbl;

    interface IVsEnumSelectedSymbols
    {
        CONST_VTBL struct IVsEnumSelectedSymbolsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumSelectedSymbols_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumSelectedSymbols_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumSelectedSymbols_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumSelectedSymbols_Next(This,celt,rgpIVsSelectedSymbol,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgpIVsSelectedSymbol,pceltFetched) ) 

#define IVsEnumSelectedSymbols_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumSelectedSymbols_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumSelectedSymbols_Clone(This,ppIVsEnumSelectedSymbols)	\
    ( (This)->lpVtbl -> Clone(This,ppIVsEnumSelectedSymbols) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumSelectedSymbols_INTERFACE_DEFINED__ */


#ifndef __IVsSelectedSymbols_INTERFACE_DEFINED__
#define __IVsSelectedSymbols_INTERFACE_DEFINED__

/* interface IVsSelectedSymbols */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSelectedSymbols;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8A8921BE-42C7-4984-82E9-C68B12C2B22E")
    IVsSelectedSymbols : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcItems) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItem( 
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbol **ppIVsSelectedSymbol) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumSelectedSymbols( 
            /* [out] */ __RPC__deref_out_opt IVsEnumSelectedSymbols **ppIVsEnumSelectedSymbols) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemTypes( 
            /* [out] */ __RPC__out DWORD *pgrfTypes) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSelectedSymbolsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSelectedSymbols * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSelectedSymbols * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSelectedSymbols * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsSelectedSymbols * This,
            /* [out] */ __RPC__out ULONG *pcItems);
        
        HRESULT ( STDMETHODCALLTYPE *GetItem )( 
            IVsSelectedSymbols * This,
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbol **ppIVsSelectedSymbol);
        
        HRESULT ( STDMETHODCALLTYPE *EnumSelectedSymbols )( 
            IVsSelectedSymbols * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumSelectedSymbols **ppIVsEnumSelectedSymbols);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemTypes )( 
            IVsSelectedSymbols * This,
            /* [out] */ __RPC__out DWORD *pgrfTypes);
        
        END_INTERFACE
    } IVsSelectedSymbolsVtbl;

    interface IVsSelectedSymbols
    {
        CONST_VTBL struct IVsSelectedSymbolsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSelectedSymbols_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSelectedSymbols_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSelectedSymbols_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSelectedSymbols_GetCount(This,pcItems)	\
    ( (This)->lpVtbl -> GetCount(This,pcItems) ) 

#define IVsSelectedSymbols_GetItem(This,iItem,ppIVsSelectedSymbol)	\
    ( (This)->lpVtbl -> GetItem(This,iItem,ppIVsSelectedSymbol) ) 

#define IVsSelectedSymbols_EnumSelectedSymbols(This,ppIVsEnumSelectedSymbols)	\
    ( (This)->lpVtbl -> EnumSelectedSymbols(This,ppIVsEnumSelectedSymbols) ) 

#define IVsSelectedSymbols_GetItemTypes(This,pgrfTypes)	\
    ( (This)->lpVtbl -> GetItemTypes(This,pgrfTypes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSelectedSymbols_INTERFACE_DEFINED__ */


#ifndef __IVsNavigationTool_INTERFACE_DEFINED__
#define __IVsNavigationTool_INTERFACE_DEFINED__

/* interface IVsNavigationTool */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsNavigationTool;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FDAEB434-F941-4370-9B89-325249EFEC48")
    IVsNavigationTool : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE NavigateToSymbol( 
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateToNavInfo( 
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSelectedSymbols( 
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbols **ppIVsSelectedSymbols) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsNavigationToolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsNavigationTool * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsNavigationTool * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsNavigationTool * This);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateToSymbol )( 
            IVsNavigationTool * This,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateToNavInfo )( 
            IVsNavigationTool * This,
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectedSymbols )( 
            IVsNavigationTool * This,
            /* [out] */ __RPC__deref_out_opt IVsSelectedSymbols **ppIVsSelectedSymbols);
        
        END_INTERFACE
    } IVsNavigationToolVtbl;

    interface IVsNavigationTool
    {
        CONST_VTBL struct IVsNavigationToolVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNavigationTool_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNavigationTool_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNavigationTool_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNavigationTool_NavigateToSymbol(This,guidLib,rgSymbolNodes,ulcNodes)	\
    ( (This)->lpVtbl -> NavigateToSymbol(This,guidLib,rgSymbolNodes,ulcNodes) ) 

#define IVsNavigationTool_NavigateToNavInfo(This,pNavInfo)	\
    ( (This)->lpVtbl -> NavigateToNavInfo(This,pNavInfo) ) 

#define IVsNavigationTool_GetSelectedSymbols(This,ppIVsSelectedSymbols)	\
    ( (This)->lpVtbl -> GetSelectedSymbols(This,ppIVsSelectedSymbols) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNavigationTool_INTERFACE_DEFINED__ */


#ifndef __IVsFindSymbol_INTERFACE_DEFINED__
#define __IVsFindSymbol_INTERFACE_DEFINED__

/* interface IVsFindSymbol */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindSymbol;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7FF85070-4667-4532-B149-63A7B205060B")
    IVsFindSymbol : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUserOptions( 
            /* [out] */ __RPC__out GUID *pguidScope,
            /* [out] */ __RPC__out VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUserOptions( 
            /* [in] */ __RPC__in REFGUID guidScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DoSearch( 
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindSymbolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindSymbol * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindSymbol * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindSymbol * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserOptions )( 
            IVsFindSymbol * This,
            /* [out] */ __RPC__out GUID *pguidScope,
            /* [out] */ __RPC__out VSOBSEARCHCRITERIA2 *pobSrch);
        
        HRESULT ( STDMETHODCALLTYPE *SetUserOptions )( 
            IVsFindSymbol * This,
            /* [in] */ __RPC__in REFGUID guidScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        HRESULT ( STDMETHODCALLTYPE *DoSearch )( 
            IVsFindSymbol * This,
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        END_INTERFACE
    } IVsFindSymbolVtbl;

    interface IVsFindSymbol
    {
        CONST_VTBL struct IVsFindSymbolVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindSymbol_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindSymbol_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindSymbol_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindSymbol_GetUserOptions(This,pguidScope,pobSrch)	\
    ( (This)->lpVtbl -> GetUserOptions(This,pguidScope,pobSrch) ) 

#define IVsFindSymbol_SetUserOptions(This,guidScope,pobSrch)	\
    ( (This)->lpVtbl -> SetUserOptions(This,guidScope,pobSrch) ) 

#define IVsFindSymbol_DoSearch(This,guidSymbolScope,pobSrch)	\
    ( (This)->lpVtbl -> DoSearch(This,guidSymbolScope,pobSrch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindSymbol_INTERFACE_DEFINED__ */


#ifndef __IVsFindSymbolEvents_INTERFACE_DEFINED__
#define __IVsFindSymbolEvents_INTERFACE_DEFINED__

/* interface IVsFindSymbolEvents */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsFindSymbolEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("18220DBE-1AEB-44ea-A924-F3571D202EF4")
    IVsFindSymbolEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnUserOptionsChanged( 
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindSymbolEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindSymbolEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindSymbolEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindSymbolEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnUserOptionsChanged )( 
            IVsFindSymbolEvents * This,
            /* [in] */ __RPC__in REFGUID guidSymbolScope,
            /* [in] */ __RPC__in const VSOBSEARCHCRITERIA2 *pobSrch);
        
        END_INTERFACE
    } IVsFindSymbolEventsVtbl;

    interface IVsFindSymbolEvents
    {
        CONST_VTBL struct IVsFindSymbolEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindSymbolEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindSymbolEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindSymbolEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindSymbolEvents_OnUserOptionsChanged(This,guidSymbolScope,pobSrch)	\
    ( (This)->lpVtbl -> OnUserOptionsChanged(This,guidSymbolScope,pobSrch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindSymbolEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0098 */
/* [local] */ 

DEFINE_GUID(GUID_VsSymbolScope_All, 0xa5a527ea, 0xcf0a, 0x4abf, 0xb5, 0x1, 0xea, 0xfe, 0x6b, 0x3b, 0xa5, 0xc6);
DEFINE_GUID(GUID_VsSymbolScope_OBSelectedComponents, 0x41fd0b24, 0x8d2b, 0x48c1, 0xb1, 0xda, 0xaa, 0xcf, 0x13, 0xa5, 0x57, 0xf);
DEFINE_GUID(GUID_VsSymbolScope_FSSelectedComponents, 0xc2146638, 0xc2fe, 0x4c1e, 0xa4, 0x9d, 0x64, 0xae, 0x97, 0x1e, 0xef, 0x39);
DEFINE_GUID(GUID_VsSymbolScope_Frameworks, 0x3168518c, 0xb7c9, 0x4e0c, 0xbd, 0x51, 0xe3, 0x32, 0x1c, 0xa7, 0xb4, 0xd8);
DEFINE_GUID(GUID_VsSymbolScope_Solution, 0xb1ba9461, 0xfc54, 0x45b3, 0xa4, 0x84, 0xcb, 0x6d, 0xd0, 0xb9, 0x5c, 0x94);

enum __VSCALLBROWSERMODE
    {	CBM_CALLSTO	= 0,
	CBM_CALLSTO_NEWWINDOW	= 1,
	CBM_CALLSFROM	= 2,
	CBM_CALLSFROM_NEWWINDOW	= 3
    } ;
typedef LONG CALLBROWSERMODE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0098_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0098_v0_0_s_ifspec;

#ifndef __IVsCallBrowser_INTERFACE_DEFINED__
#define __IVsCallBrowser_INTERFACE_DEFINED__

/* interface IVsCallBrowser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCallBrowser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6ED4E844-A0AF-4d6f-B108-8E655785BC88")
    IVsCallBrowser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetRootAtSymbol( 
            /* [in] */ CALLBROWSERMODE cbMode,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRootAtNavInfo( 
            /* [in] */ CALLBROWSERMODE cbMode,
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanCreateNewInstance( 
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCallBrowserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCallBrowser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCallBrowser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCallBrowser * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetRootAtSymbol )( 
            IVsCallBrowser * This,
            /* [in] */ CALLBROWSERMODE cbMode,
            /* [in] */ __RPC__in REFGUID guidLib,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes);
        
        HRESULT ( STDMETHODCALLTYPE *SetRootAtNavInfo )( 
            IVsCallBrowser * This,
            /* [in] */ CALLBROWSERMODE cbMode,
            /* [in] */ __RPC__in_opt IVsNavInfo *pNavInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CanCreateNewInstance )( 
            IVsCallBrowser * This,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        END_INTERFACE
    } IVsCallBrowserVtbl;

    interface IVsCallBrowser
    {
        CONST_VTBL struct IVsCallBrowserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCallBrowser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCallBrowser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCallBrowser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCallBrowser_SetRootAtSymbol(This,cbMode,guidLib,rgSymbolNodes,ulcNodes)	\
    ( (This)->lpVtbl -> SetRootAtSymbol(This,cbMode,guidLib,rgSymbolNodes,ulcNodes) ) 

#define IVsCallBrowser_SetRootAtNavInfo(This,cbMode,pNavInfo)	\
    ( (This)->lpVtbl -> SetRootAtNavInfo(This,cbMode,pNavInfo) ) 

#define IVsCallBrowser_CanCreateNewInstance(This,pfOK)	\
    ( (This)->lpVtbl -> CanCreateNewInstance(This,pfOK) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCallBrowser_INTERFACE_DEFINED__ */


#ifndef __SVsCallBrowser_INTERFACE_DEFINED__
#define __SVsCallBrowser_INTERFACE_DEFINED__

/* interface SVsCallBrowser */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsCallBrowser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F9D0F5FF-C093-4e80-A886-57610D58A728")
    SVsCallBrowser : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsCallBrowserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsCallBrowser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsCallBrowser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsCallBrowser * This);
        
        END_INTERFACE
    } SVsCallBrowserVtbl;

    interface SVsCallBrowser
    {
        CONST_VTBL struct SVsCallBrowserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsCallBrowser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsCallBrowser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsCallBrowser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsCallBrowser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0100 */
/* [local] */ 

#define SID_SVsCallBrowser IID_SVsCallBrowser
extern const __declspec(selectany) GUID GUID_CallBrowser = { 0x5415ea3a, 0xd813, 0x4948, { 0xb5, 0x1e, 0x56, 0x20, 0x82, 0xce, 0x8, 0x87 } };
extern const __declspec(selectany) GUID GUID_CallBrowserSecondary = { 0xf78bcc56, 0x71f7, 0x4e7d, { 0x82, 0x15, 0xf6, 0x90, 0xca, 0xe4, 0xf4, 0x52 } };

enum __VSCOMPSELFLAGS2
    {	VSCOMSEL2_MultiSelectMode	= 0x1,
	VSCOMSEL2_ShowSelectedList	= 0x80,
	VSCOMSEL2_ShowAllPagesOfSpecifiedTypes	= 0x100
    } ;
typedef DWORD VSCOMPSELFLAGS2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0100_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0100_v0_0_s_ifspec;

#ifndef __IVsComponentSelectorDlg2_INTERFACE_DEFINED__
#define __IVsComponentSelectorDlg2_INTERFACE_DEFINED__

/* interface IVsComponentSelectorDlg2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsComponentSelectorDlg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2D7CDC3D-FA79-4df3-9CD2-AACF65A2846E")
    IVsComponentSelectorDlg2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ComponentSelectorDlg2( 
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) PVSCOMPONENTSELECTORDATA rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentSelectorDlg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentSelectorDlg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentSelectorDlg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentSelectorDlg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ComponentSelectorDlg2 )( 
            IVsComponentSelectorDlg2 * This,
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) PVSCOMPONENTSELECTORDATA rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation);
        
        END_INTERFACE
    } IVsComponentSelectorDlg2Vtbl;

    interface IVsComponentSelectorDlg2
    {
        CONST_VTBL struct IVsComponentSelectorDlg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentSelectorDlg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentSelectorDlg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentSelectorDlg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentSelectorDlg2_ComponentSelectorDlg2(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation)	\
    ( (This)->lpVtbl -> ComponentSelectorDlg2(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentSelectorDlg2_INTERFACE_DEFINED__ */


#ifndef __SVsComponentSelectorDlg2_INTERFACE_DEFINED__
#define __SVsComponentSelectorDlg2_INTERFACE_DEFINED__

/* interface SVsComponentSelectorDlg2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsComponentSelectorDlg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("525D42EF-D7F6-412a-9D64-79E787E70E45")
    SVsComponentSelectorDlg2 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsComponentSelectorDlg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsComponentSelectorDlg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsComponentSelectorDlg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsComponentSelectorDlg2 * This);
        
        END_INTERFACE
    } SVsComponentSelectorDlg2Vtbl;

    interface SVsComponentSelectorDlg2
    {
        CONST_VTBL struct SVsComponentSelectorDlg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsComponentSelectorDlg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsComponentSelectorDlg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsComponentSelectorDlg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsComponentSelectorDlg2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0102 */
/* [local] */ 

#define SID_SVsComponentSelectorDlg2 IID_SVsComponentSelectorDlg2
DEFINE_GUID(GUID_BrowseFilesPage, 0x2483f435, 0x673d, 0x4fa3, 0x8a, 0xdd, 0xb5, 0x14, 0x42, 0xf6, 0x53, 0x49);
DEFINE_GUID(GUID_MRUPage, 0x19b97f03, 0x9594, 0x4c1c, 0xbe, 0x28, 0x25, 0xff, 0x3, 0x1, 0x13, 0xb3);


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0102_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0102_v0_0_s_ifspec;

#ifndef __IVsBuildMacroInfo_INTERFACE_DEFINED__
#define __IVsBuildMacroInfo_INTERFACE_DEFINED__

/* interface IVsBuildMacroInfo */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildMacroInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9015B3AB-9FE2-430d-B79F-E0E581BD5B8B")
    IVsBuildMacroInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBuildMacroValue( 
            /* [in] */ __RPC__in BSTR bstrBuildMacroName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuildMacroValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsBuildMacroInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBuildMacroInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBuildMacroInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBuildMacroInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuildMacroValue )( 
            IVsBuildMacroInfo * This,
            /* [in] */ __RPC__in BSTR bstrBuildMacroName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBuildMacroValue);
        
        END_INTERFACE
    } IVsBuildMacroInfoVtbl;

    interface IVsBuildMacroInfo
    {
        CONST_VTBL struct IVsBuildMacroInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildMacroInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildMacroInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildMacroInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildMacroInfo_GetBuildMacroValue(This,bstrBuildMacroName,pbstrBuildMacroValue)	\
    ( (This)->lpVtbl -> GetBuildMacroValue(This,bstrBuildMacroName,pbstrBuildMacroValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildMacroInfo_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0103 */
/* [local] */ 


enum __PREVIEWCHANGESITEMCHECKSTATE
    {	PCCS_None	= 0,
	PCCS_Unchecked	= 1,
	PCCS_PartiallyChecked	= 2,
	PCCS_Checked	= 3
    } ;
typedef LONG PREVIEWCHANGESITEMCHECKSTATE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0103_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0103_v0_0_s_ifspec;

#ifndef __IVsPreviewChangesList_INTERFACE_DEFINED__
#define __IVsPreviewChangesList_INTERFACE_DEFINED__

/* interface IVsPreviewChangesList */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPreviewChangesList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B334F714-993B-4902-89E0-792213B538DB")
    IVsPreviewChangesList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFlags( 
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemCount( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandedList( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsLiteTreeList **pptlNode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateExpandedList( 
            /* [in] */ __RPC__in_opt IVsLiteTreeList *ExpandedList,
            /* [out] */ __RPC__out ULONG *iIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnClose( 
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandable( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetDisplayData( 
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateCounter( 
            /* [out] */ __RPC__out ULONG *pCurUpdate,
            /* [out] */ __RPC__out VSTREEITEMCHANGESMASK *pgrfChanges) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetListChanges( 
            /* [out][in] */ __RPC__inout ULONG *pcChanges,
            /* [size_is][in] */ __RPC__in_ecount_full(*pcChanges) VSTREELISTITEMCHANGE *prgListChanges) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToggleState( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRequestSource( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownTextView) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPreviewChangesListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPreviewChangesList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPreviewChangesList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPreviewChangesList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFlags )( 
            IVsPreviewChangesList * This,
            /* [out] */ __RPC__out VSTREEFLAGS *pFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemCount )( 
            IVsPreviewChangesList * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandedList )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsLiteTreeList **pptlNode);
        
        HRESULT ( STDMETHODCALLTYPE *LocateExpandedList )( 
            IVsPreviewChangesList * This,
            /* [in] */ __RPC__in_opt IVsLiteTreeList *ExpandedList,
            /* [out] */ __RPC__out ULONG *iIndex);
        
        HRESULT ( STDMETHODCALLTYPE *OnClose )( 
            IVsPreviewChangesList * This,
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [string][out] */ __RPC__deref_out_opt_string const WCHAR **ppszText);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandable )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetDisplayData )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateCounter )( 
            IVsPreviewChangesList * This,
            /* [out] */ __RPC__out ULONG *pCurUpdate,
            /* [out] */ __RPC__out VSTREEITEMCHANGESMASK *pgrfChanges);
        
        HRESULT ( STDMETHODCALLTYPE *GetListChanges )( 
            IVsPreviewChangesList * This,
            /* [out][in] */ __RPC__inout ULONG *pcChanges,
            /* [size_is][in] */ __RPC__in_ecount_full(*pcChanges) VSTREELISTITEMCHANGE *prgListChanges);
        
        HRESULT ( STDMETHODCALLTYPE *ToggleState )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr);
        
        HRESULT ( STDMETHODCALLTYPE *OnRequestSource )( 
            IVsPreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownTextView);
        
        END_INTERFACE
    } IVsPreviewChangesListVtbl;

    interface IVsPreviewChangesList
    {
        CONST_VTBL struct IVsPreviewChangesListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPreviewChangesList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPreviewChangesList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPreviewChangesList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPreviewChangesList_GetFlags(This,pFlags)	\
    ( (This)->lpVtbl -> GetFlags(This,pFlags) ) 

#define IVsPreviewChangesList_GetItemCount(This,pCount)	\
    ( (This)->lpVtbl -> GetItemCount(This,pCount) ) 

#define IVsPreviewChangesList_GetExpandedList(This,Index,pfCanRecurse,pptlNode)	\
    ( (This)->lpVtbl -> GetExpandedList(This,Index,pfCanRecurse,pptlNode) ) 

#define IVsPreviewChangesList_LocateExpandedList(This,ExpandedList,iIndex)	\
    ( (This)->lpVtbl -> LocateExpandedList(This,ExpandedList,iIndex) ) 

#define IVsPreviewChangesList_OnClose(This,ptca)	\
    ( (This)->lpVtbl -> OnClose(This,ptca) ) 

#define IVsPreviewChangesList_GetText(This,Index,tto,ppszText)	\
    ( (This)->lpVtbl -> GetText(This,Index,tto,ppszText) ) 

#define IVsPreviewChangesList_GetTipText(This,Index,eTipType,ppszText)	\
    ( (This)->lpVtbl -> GetTipText(This,Index,eTipType,ppszText) ) 

#define IVsPreviewChangesList_GetExpandable(This,Index,pfExpandable)	\
    ( (This)->lpVtbl -> GetExpandable(This,Index,pfExpandable) ) 

#define IVsPreviewChangesList_GetDisplayData(This,Index,pData)	\
    ( (This)->lpVtbl -> GetDisplayData(This,Index,pData) ) 

#define IVsPreviewChangesList_UpdateCounter(This,pCurUpdate,pgrfChanges)	\
    ( (This)->lpVtbl -> UpdateCounter(This,pCurUpdate,pgrfChanges) ) 

#define IVsPreviewChangesList_GetListChanges(This,pcChanges,prgListChanges)	\
    ( (This)->lpVtbl -> GetListChanges(This,pcChanges,prgListChanges) ) 

#define IVsPreviewChangesList_ToggleState(This,Index,ptscr)	\
    ( (This)->lpVtbl -> ToggleState(This,Index,ptscr) ) 

#define IVsPreviewChangesList_OnRequestSource(This,Index,pIUnknownTextView)	\
    ( (This)->lpVtbl -> OnRequestSource(This,Index,pIUnknownTextView) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPreviewChangesList_INTERFACE_DEFINED__ */


#ifndef __IVsSimplePreviewChangesList_INTERFACE_DEFINED__
#define __IVsSimplePreviewChangesList_INTERFACE_DEFINED__

/* interface IVsSimplePreviewChangesList */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSimplePreviewChangesList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C42D228E-B275-4FE6-8469-F3184663B883")
    IVsSimplePreviewChangesList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetItemCount( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetDisplayData( 
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextWithOwnership( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipTextWithOwnership( 
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandable( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExpandedList( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsSimplePreviewChangesList **ppIVsSimplePreviewChangesList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LocateExpandedList( 
            /* [in] */ __RPC__in_opt IVsSimplePreviewChangesList *pIVsSimplePreviewChangesListChild,
            /* [out] */ __RPC__out ULONG *piIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToggleState( 
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRequestSource( 
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownTextView) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnClose( 
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSimplePreviewChangesListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSimplePreviewChangesList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSimplePreviewChangesList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemCount )( 
            IVsSimplePreviewChangesList * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetDisplayData )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ VSTREEDISPLAYDATA *pData);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextWithOwnership )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETEXTOPTIONS tto,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipTextWithOwnership )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ VSTREETOOLTIPTYPE eTipType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandable )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfExpandable);
        
        HRESULT ( STDMETHODCALLTYPE *GetExpandedList )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out BOOL *pfCanRecurse,
            /* [out] */ __RPC__deref_out_opt IVsSimplePreviewChangesList **ppIVsSimplePreviewChangesList);
        
        HRESULT ( STDMETHODCALLTYPE *LocateExpandedList )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ __RPC__in_opt IVsSimplePreviewChangesList *pIVsSimplePreviewChangesListChild,
            /* [out] */ __RPC__out ULONG *piIndex);
        
        HRESULT ( STDMETHODCALLTYPE *ToggleState )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [out] */ __RPC__out VSTREESTATECHANGEREFRESH *ptscr);
        
        HRESULT ( STDMETHODCALLTYPE *OnRequestSource )( 
            IVsSimplePreviewChangesList * This,
            /* [in] */ ULONG Index,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownTextView);
        
        HRESULT ( STDMETHODCALLTYPE *OnClose )( 
            IVsSimplePreviewChangesList * This,
            /* [out] */ __RPC__out VSTREECLOSEACTIONS *ptca);
        
        END_INTERFACE
    } IVsSimplePreviewChangesListVtbl;

    interface IVsSimplePreviewChangesList
    {
        CONST_VTBL struct IVsSimplePreviewChangesListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSimplePreviewChangesList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSimplePreviewChangesList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSimplePreviewChangesList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSimplePreviewChangesList_GetItemCount(This,pCount)	\
    ( (This)->lpVtbl -> GetItemCount(This,pCount) ) 

#define IVsSimplePreviewChangesList_GetDisplayData(This,Index,pData)	\
    ( (This)->lpVtbl -> GetDisplayData(This,Index,pData) ) 

#define IVsSimplePreviewChangesList_GetTextWithOwnership(This,Index,tto,pbstrText)	\
    ( (This)->lpVtbl -> GetTextWithOwnership(This,Index,tto,pbstrText) ) 

#define IVsSimplePreviewChangesList_GetTipTextWithOwnership(This,Index,eTipType,pbstrText)	\
    ( (This)->lpVtbl -> GetTipTextWithOwnership(This,Index,eTipType,pbstrText) ) 

#define IVsSimplePreviewChangesList_GetExpandable(This,Index,pfExpandable)	\
    ( (This)->lpVtbl -> GetExpandable(This,Index,pfExpandable) ) 

#define IVsSimplePreviewChangesList_GetExpandedList(This,Index,pfCanRecurse,ppIVsSimplePreviewChangesList)	\
    ( (This)->lpVtbl -> GetExpandedList(This,Index,pfCanRecurse,ppIVsSimplePreviewChangesList) ) 

#define IVsSimplePreviewChangesList_LocateExpandedList(This,pIVsSimplePreviewChangesListChild,piIndex)	\
    ( (This)->lpVtbl -> LocateExpandedList(This,pIVsSimplePreviewChangesListChild,piIndex) ) 

#define IVsSimplePreviewChangesList_ToggleState(This,Index,ptscr)	\
    ( (This)->lpVtbl -> ToggleState(This,Index,ptscr) ) 

#define IVsSimplePreviewChangesList_OnRequestSource(This,Index,pIUnknownTextView)	\
    ( (This)->lpVtbl -> OnRequestSource(This,Index,pIUnknownTextView) ) 

#define IVsSimplePreviewChangesList_OnClose(This,ptca)	\
    ( (This)->lpVtbl -> OnClose(This,ptca) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSimplePreviewChangesList_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0105 */
/* [local] */ 


enum __PREVIEWCHANGESWARNINGLEVEL
    {	PCWL_None	= 0,
	PCWL_Information	= 1,
	PCWL_Warning	= 2,
	PCWL_Error	= 3
    } ;
typedef LONG PREVIEWCHANGESWARNINGLEVEL;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0105_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0105_v0_0_s_ifspec;

#ifndef __IVsPreviewChangesEngine_INTERFACE_DEFINED__
#define __IVsPreviewChangesEngine_INTERFACE_DEFINED__

/* interface IVsPreviewChangesEngine */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPreviewChangesEngine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6DE3E607-1E2C-4f56-B4A6-BCAF63A0BB9F")
    IVsPreviewChangesEngine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTitle( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTitle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextViewDescription( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTextViewDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWarning( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWarning,
            /* [out] */ __RPC__out PREVIEWCHANGESWARNINGLEVEL *ppcwlWarningLevel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHelpContext( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetConfirmation( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrConfirmation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRootChangesList( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownPreviewChangesList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ApplyChanges( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPreviewChangesEngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPreviewChangesEngine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPreviewChangesEngine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPreviewChangesEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTitle )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTitle);
        
        HRESULT ( STDMETHODCALLTYPE *GetDescription )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextViewDescription )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTextViewDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetWarning )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWarning,
            /* [out] */ __RPC__out PREVIEWCHANGESWARNINGLEVEL *ppcwlWarningLevel);
        
        HRESULT ( STDMETHODCALLTYPE *GetHelpContext )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetConfirmation )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrConfirmation);
        
        HRESULT ( STDMETHODCALLTYPE *GetRootChangesList )( 
            IVsPreviewChangesEngine * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownPreviewChangesList);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyChanges )( 
            IVsPreviewChangesEngine * This);
        
        END_INTERFACE
    } IVsPreviewChangesEngineVtbl;

    interface IVsPreviewChangesEngine
    {
        CONST_VTBL struct IVsPreviewChangesEngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPreviewChangesEngine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPreviewChangesEngine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPreviewChangesEngine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPreviewChangesEngine_GetTitle(This,pbstrTitle)	\
    ( (This)->lpVtbl -> GetTitle(This,pbstrTitle) ) 

#define IVsPreviewChangesEngine_GetDescription(This,pbstrDescription)	\
    ( (This)->lpVtbl -> GetDescription(This,pbstrDescription) ) 

#define IVsPreviewChangesEngine_GetTextViewDescription(This,pbstrTextViewDescription)	\
    ( (This)->lpVtbl -> GetTextViewDescription(This,pbstrTextViewDescription) ) 

#define IVsPreviewChangesEngine_GetWarning(This,pbstrWarning,ppcwlWarningLevel)	\
    ( (This)->lpVtbl -> GetWarning(This,pbstrWarning,ppcwlWarningLevel) ) 

#define IVsPreviewChangesEngine_GetHelpContext(This,pbstrHelpContext)	\
    ( (This)->lpVtbl -> GetHelpContext(This,pbstrHelpContext) ) 

#define IVsPreviewChangesEngine_GetConfirmation(This,pbstrConfirmation)	\
    ( (This)->lpVtbl -> GetConfirmation(This,pbstrConfirmation) ) 

#define IVsPreviewChangesEngine_GetRootChangesList(This,ppIUnknownPreviewChangesList)	\
    ( (This)->lpVtbl -> GetRootChangesList(This,ppIUnknownPreviewChangesList) ) 

#define IVsPreviewChangesEngine_ApplyChanges(This)	\
    ( (This)->lpVtbl -> ApplyChanges(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPreviewChangesEngine_INTERFACE_DEFINED__ */


#ifndef __IVsPreviewChangesService_INTERFACE_DEFINED__
#define __IVsPreviewChangesService_INTERFACE_DEFINED__

/* interface IVsPreviewChangesService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPreviewChangesService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AF142B19-FB37-40c0-9C28-6617ADBFFAC7")
    IVsPreviewChangesService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE PreviewChanges( 
            /* [in] */ __RPC__in_opt IVsPreviewChangesEngine *pIVsPreviewChangesEngine) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPreviewChangesServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPreviewChangesService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPreviewChangesService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPreviewChangesService * This);
        
        HRESULT ( STDMETHODCALLTYPE *PreviewChanges )( 
            IVsPreviewChangesService * This,
            /* [in] */ __RPC__in_opt IVsPreviewChangesEngine *pIVsPreviewChangesEngine);
        
        END_INTERFACE
    } IVsPreviewChangesServiceVtbl;

    interface IVsPreviewChangesService
    {
        CONST_VTBL struct IVsPreviewChangesServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPreviewChangesService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPreviewChangesService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPreviewChangesService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPreviewChangesService_PreviewChanges(This,pIVsPreviewChangesEngine)	\
    ( (This)->lpVtbl -> PreviewChanges(This,pIVsPreviewChangesEngine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPreviewChangesService_INTERFACE_DEFINED__ */


#ifndef __SVsPreviewChangesService_INTERFACE_DEFINED__
#define __SVsPreviewChangesService_INTERFACE_DEFINED__

/* interface SVsPreviewChangesService */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsPreviewChangesService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12143874-DA05-4d05-9F57-2D339C142220")
    SVsPreviewChangesService : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsPreviewChangesServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsPreviewChangesService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsPreviewChangesService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsPreviewChangesService * This);
        
        END_INTERFACE
    } SVsPreviewChangesServiceVtbl;

    interface SVsPreviewChangesService
    {
        CONST_VTBL struct SVsPreviewChangesServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsPreviewChangesService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsPreviewChangesService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsPreviewChangesService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsPreviewChangesService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0108 */
/* [local] */ 

#define SID_SVsPreviewChangesService IID_SVsPreviewChangesService


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0108_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0108_v0_0_s_ifspec;

#ifndef __IVsCodeDefViewContext_INTERFACE_DEFINED__
#define __IVsCodeDefViewContext_INTERFACE_DEFINED__

/* interface IVsCodeDefViewContext */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCodeDefViewContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E7070F9A-502F-4454-B4A2-FE261C568C37")
    IVsCodeDefViewContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [out] */ __RPC__out ULONG *pcItems) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSymbolName( 
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileName( 
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLine( 
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__out ULONG *piLine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCol( 
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__out ULONG *piCol) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCodeDefViewContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCodeDefViewContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCodeDefViewContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCodeDefViewContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsCodeDefViewContext * This,
            /* [out] */ __RPC__out ULONG *pcItems);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolName )( 
            IVsCodeDefViewContext * This,
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSymbolName);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IVsCodeDefViewContext * This,
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetLine )( 
            IVsCodeDefViewContext * This,
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__out ULONG *piLine);
        
        HRESULT ( STDMETHODCALLTYPE *GetCol )( 
            IVsCodeDefViewContext * This,
            /* [in] */ ULONG iItem,
            /* [out] */ __RPC__out ULONG *piCol);
        
        END_INTERFACE
    } IVsCodeDefViewContextVtbl;

    interface IVsCodeDefViewContext
    {
        CONST_VTBL struct IVsCodeDefViewContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeDefViewContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeDefViewContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeDefViewContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeDefViewContext_GetCount(This,pcItems)	\
    ( (This)->lpVtbl -> GetCount(This,pcItems) ) 

#define IVsCodeDefViewContext_GetSymbolName(This,iItem,pbstrSymbolName)	\
    ( (This)->lpVtbl -> GetSymbolName(This,iItem,pbstrSymbolName) ) 

#define IVsCodeDefViewContext_GetFileName(This,iItem,pbstrFileName)	\
    ( (This)->lpVtbl -> GetFileName(This,iItem,pbstrFileName) ) 

#define IVsCodeDefViewContext_GetLine(This,iItem,piLine)	\
    ( (This)->lpVtbl -> GetLine(This,iItem,piLine) ) 

#define IVsCodeDefViewContext_GetCol(This,iItem,piCol)	\
    ( (This)->lpVtbl -> GetCol(This,iItem,piCol) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeDefViewContext_INTERFACE_DEFINED__ */


#ifndef __IVsCodeDefView_INTERFACE_DEFINED__
#define __IVsCodeDefView_INTERFACE_DEFINED__

/* interface IVsCodeDefView */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCodeDefView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("588470CC-84F8-4a57-9AC4-86BCA0625FF4")
    IVsCodeDefView : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowWindow( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HideWindow( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsVisible( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetContext( 
            /* [in] */ __RPC__in_opt IVsCodeDefViewContext *pIVsCodeDefViewContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRefreshDelay( 
            /* [out] */ __RPC__out ULONG *pcMilliseconds) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ForceIdleProcessing( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsCodeDefView( 
            /* [in] */ __RPC__in_opt IVsTextView *pIVsTextView,
            /* [out] */ __RPC__out BOOL *pfIsCodeDefView) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCodeDefViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCodeDefView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCodeDefView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCodeDefView * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowWindow )( 
            IVsCodeDefView * This);
        
        HRESULT ( STDMETHODCALLTYPE *HideWindow )( 
            IVsCodeDefView * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsVisible )( 
            IVsCodeDefView * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetContext )( 
            IVsCodeDefView * This,
            /* [in] */ __RPC__in_opt IVsCodeDefViewContext *pIVsCodeDefViewContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetRefreshDelay )( 
            IVsCodeDefView * This,
            /* [out] */ __RPC__out ULONG *pcMilliseconds);
        
        HRESULT ( STDMETHODCALLTYPE *ForceIdleProcessing )( 
            IVsCodeDefView * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsCodeDefView )( 
            IVsCodeDefView * This,
            /* [in] */ __RPC__in_opt IVsTextView *pIVsTextView,
            /* [out] */ __RPC__out BOOL *pfIsCodeDefView);
        
        END_INTERFACE
    } IVsCodeDefViewVtbl;

    interface IVsCodeDefView
    {
        CONST_VTBL struct IVsCodeDefViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCodeDefView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCodeDefView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCodeDefView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCodeDefView_ShowWindow(This)	\
    ( (This)->lpVtbl -> ShowWindow(This) ) 

#define IVsCodeDefView_HideWindow(This)	\
    ( (This)->lpVtbl -> HideWindow(This) ) 

#define IVsCodeDefView_IsVisible(This)	\
    ( (This)->lpVtbl -> IsVisible(This) ) 

#define IVsCodeDefView_SetContext(This,pIVsCodeDefViewContext)	\
    ( (This)->lpVtbl -> SetContext(This,pIVsCodeDefViewContext) ) 

#define IVsCodeDefView_GetRefreshDelay(This,pcMilliseconds)	\
    ( (This)->lpVtbl -> GetRefreshDelay(This,pcMilliseconds) ) 

#define IVsCodeDefView_ForceIdleProcessing(This)	\
    ( (This)->lpVtbl -> ForceIdleProcessing(This) ) 

#define IVsCodeDefView_IsCodeDefView(This,pIVsTextView,pfIsCodeDefView)	\
    ( (This)->lpVtbl -> IsCodeDefView(This,pIVsTextView,pfIsCodeDefView) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCodeDefView_INTERFACE_DEFINED__ */


#ifndef __IVsSupportCodeDefView_INTERFACE_DEFINED__
#define __IVsSupportCodeDefView_INTERFACE_DEFINED__

/* interface IVsSupportCodeDefView */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSupportCodeDefView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D34E5A63-E990-472A-9852-45FB36AA67AB")
    IVsSupportCodeDefView : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IVsSupportCodeDefViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSupportCodeDefView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSupportCodeDefView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSupportCodeDefView * This);
        
        END_INTERFACE
    } IVsSupportCodeDefViewVtbl;

    interface IVsSupportCodeDefView
    {
        CONST_VTBL struct IVsSupportCodeDefViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSupportCodeDefView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSupportCodeDefView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSupportCodeDefView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSupportCodeDefView_INTERFACE_DEFINED__ */


#ifndef __SVsCodeDefView_INTERFACE_DEFINED__
#define __SVsCodeDefView_INTERFACE_DEFINED__

/* interface SVsCodeDefView */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsCodeDefView;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("66230816-AC33-49b5-97DB-0F6DF2A83624")
    SVsCodeDefView : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsCodeDefViewVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsCodeDefView * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsCodeDefView * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsCodeDefView * This);
        
        END_INTERFACE
    } SVsCodeDefViewVtbl;

    interface SVsCodeDefView
    {
        CONST_VTBL struct SVsCodeDefViewVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsCodeDefView_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsCodeDefView_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsCodeDefView_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsCodeDefView_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0112 */
/* [local] */ 

#define SID_SVsCodeDefView IID_SVsCodeDefView


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0112_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0112_v0_0_s_ifspec;

#ifndef __IVsCoTaskMemFreeMyStrings_INTERFACE_DEFINED__
#define __IVsCoTaskMemFreeMyStrings_INTERFACE_DEFINED__

/* interface IVsCoTaskMemFreeMyStrings */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsCoTaskMemFreeMyStrings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("47811DA4-330F-4eb5-9D14-BBC82773DA66")
    IVsCoTaskMemFreeMyStrings : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IVsCoTaskMemFreeMyStringsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCoTaskMemFreeMyStrings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCoTaskMemFreeMyStrings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCoTaskMemFreeMyStrings * This);
        
        END_INTERFACE
    } IVsCoTaskMemFreeMyStringsVtbl;

    interface IVsCoTaskMemFreeMyStrings
    {
        CONST_VTBL struct IVsCoTaskMemFreeMyStringsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCoTaskMemFreeMyStrings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCoTaskMemFreeMyStrings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCoTaskMemFreeMyStrings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCoTaskMemFreeMyStrings_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0113 */
/* [local] */ 

typedef 
enum __VSRDTFLAGS2
    {	RDT_Lock_WeakEditLock	= 0x800,
	RDT_LOCKUNLOCKMASK	= 0xf00
    } 	_VSRDTFLAGS2;

typedef DWORD VSRDTFLAGS2;


enum __VSRDTSAVEOPTIONS2
    {	RDTSAVEOPT_SkipNewUnsaved	= 0x20,
	RDTSAVEOPT_SaveAllButThis	= 0x40,
	RDTSAVEOPT_FSaveAs	= 0x20000000
    } ;
typedef DWORD VSRDTSAVEOPTIONS2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0113_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0113_v0_0_s_ifspec;

#ifndef __IVsRunningDocumentTable2_INTERFACE_DEFINED__
#define __IVsRunningDocumentTable2_INTERFACE_DEFINED__

/* interface IVsRunningDocumentTable2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRunningDocumentTable2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CD68D3CF-7124-4d3a-AFED-3E305C2B7D0B")
    IVsRunningDocumentTable2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CloseDocuments( 
            /* [in] */ FRAMECLOSE grfSaveOptions,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ VSCOOKIE docCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryCloseRunningDocument( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pfFoundAndClosed) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindAndLockDocumentEx( 
            /* [in] */ VSRDTFLAGS grfRDTLockType,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierPreferred,
            /* [in] */ VSITEMID itemidPreferred,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierActual,
            /* [out] */ __RPC__out VSITEMID *pitemidActual,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkDocDataActual,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindOrRegisterAndLockDocument( 
            /* [in] */ VSRDTFLAGS grfRDTLockType,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierPreferred,
            /* [in] */ VSITEMID itemidPreferred,
            /* [in] */ __RPC__in_opt IUnknown *punkDocData,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierActual,
            /* [out] */ __RPC__out VSITEMID *pitemidActual,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkDocDataActual,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRunningDocumentTable2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRunningDocumentTable2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRunningDocumentTable2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRunningDocumentTable2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CloseDocuments )( 
            IVsRunningDocumentTable2 * This,
            /* [in] */ FRAMECLOSE grfSaveOptions,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ VSCOOKIE docCookie);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCloseRunningDocument )( 
            IVsRunningDocumentTable2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pfFoundAndClosed);
        
        HRESULT ( STDMETHODCALLTYPE *FindAndLockDocumentEx )( 
            IVsRunningDocumentTable2 * This,
            /* [in] */ VSRDTFLAGS grfRDTLockType,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierPreferred,
            /* [in] */ VSITEMID itemidPreferred,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierActual,
            /* [out] */ __RPC__out VSITEMID *pitemidActual,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkDocDataActual,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *FindOrRegisterAndLockDocument )( 
            IVsRunningDocumentTable2 * This,
            /* [in] */ VSRDTFLAGS grfRDTLockType,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierPreferred,
            /* [in] */ VSITEMID itemidPreferred,
            /* [in] */ __RPC__in_opt IUnknown *punkDocData,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHierActual,
            /* [out] */ __RPC__out VSITEMID *pitemidActual,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkDocDataActual,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        END_INTERFACE
    } IVsRunningDocumentTable2Vtbl;

    interface IVsRunningDocumentTable2
    {
        CONST_VTBL struct IVsRunningDocumentTable2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRunningDocumentTable2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRunningDocumentTable2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRunningDocumentTable2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRunningDocumentTable2_CloseDocuments(This,grfSaveOptions,pHierarchy,docCookie)	\
    ( (This)->lpVtbl -> CloseDocuments(This,grfSaveOptions,pHierarchy,docCookie) ) 

#define IVsRunningDocumentTable2_QueryCloseRunningDocument(This,pszMkDocument,pfFoundAndClosed)	\
    ( (This)->lpVtbl -> QueryCloseRunningDocument(This,pszMkDocument,pfFoundAndClosed) ) 

#define IVsRunningDocumentTable2_FindAndLockDocumentEx(This,grfRDTLockType,pszMkDocument,pHierPreferred,itemidPreferred,ppHierActual,pitemidActual,ppunkDocDataActual,pdwCookie)	\
    ( (This)->lpVtbl -> FindAndLockDocumentEx(This,grfRDTLockType,pszMkDocument,pHierPreferred,itemidPreferred,ppHierActual,pitemidActual,ppunkDocDataActual,pdwCookie) ) 

#define IVsRunningDocumentTable2_FindOrRegisterAndLockDocument(This,grfRDTLockType,pszMkDocument,pHierPreferred,itemidPreferred,punkDocData,ppHierActual,pitemidActual,ppunkDocDataActual,pdwCookie)	\
    ( (This)->lpVtbl -> FindOrRegisterAndLockDocument(This,grfRDTLockType,pszMkDocument,pHierPreferred,itemidPreferred,punkDocData,ppHierActual,pitemidActual,ppunkDocDataActual,pdwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRunningDocumentTable2_INTERFACE_DEFINED__ */


#ifndef __IVsRunningDocTableEvents4_INTERFACE_DEFINED__
#define __IVsRunningDocTableEvents4_INTERFACE_DEFINED__

/* interface IVsRunningDocTableEvents4 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsRunningDocTableEvents4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("79A342F3-D637-4d54-83DC-DDD511743A49")
    IVsRunningDocTableEvents4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeFirstDocumentLock( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSaveAll( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterLastDocumentUnlock( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ BOOL fClosedWithoutSaving) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRunningDocTableEvents4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRunningDocTableEvents4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRunningDocTableEvents4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRunningDocTableEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeFirstDocumentLock )( 
            IVsRunningDocTableEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSaveAll )( 
            IVsRunningDocTableEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterLastDocumentUnlock )( 
            IVsRunningDocTableEvents4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ BOOL fClosedWithoutSaving);
        
        END_INTERFACE
    } IVsRunningDocTableEvents4Vtbl;

    interface IVsRunningDocTableEvents4
    {
        CONST_VTBL struct IVsRunningDocTableEvents4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRunningDocTableEvents4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRunningDocTableEvents4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRunningDocTableEvents4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRunningDocTableEvents4_OnBeforeFirstDocumentLock(This,pHier,itemid,pszMkDocument)	\
    ( (This)->lpVtbl -> OnBeforeFirstDocumentLock(This,pHier,itemid,pszMkDocument) ) 

#define IVsRunningDocTableEvents4_OnAfterSaveAll(This)	\
    ( (This)->lpVtbl -> OnAfterSaveAll(This) ) 

#define IVsRunningDocTableEvents4_OnAfterLastDocumentUnlock(This,pHier,itemid,pszMkDocument,fClosedWithoutSaving)	\
    ( (This)->lpVtbl -> OnAfterLastDocumentUnlock(This,pHier,itemid,pszMkDocument,fClosedWithoutSaving) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRunningDocTableEvents4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0115 */
/* [local] */ 

extern const __declspec(selectany) GUID UICONTEXT_NotBuildingAndNotDebugging =  { 0x48ea4a80, 0xf14e, 0x4107, { 0x88, 0xfa, 0x8d, 0x0, 0x16, 0xf3, 0xb, 0x9c } };
extern const __declspec(selectany) GUID UICONTEXT_SolutionOrProjectUpgrading =  { 0xef4f870b, 0x7b85, 0x4f29, { 0x9d, 0x15, 0xce, 0x1a, 0xbf, 0xbe, 0x73, 0x3b } };
extern const __declspec(selectany) GUID UICONTEXT_DataSourceWindowSupported =   { 0x95c314c4, 0x660b, 0x4627, { 0x9f, 0x82, 0x1b, 0xaf, 0x1c, 0x76, 0x4b, 0xbf } };
extern const __declspec(selectany) GUID UICONTEXT_DataSourceWindowAutoVisible = { 0x2e78870d, 0xac7c, 0x4460, { 0xa4, 0xa1, 0x3f, 0xe3, 0x7d, 0x00, 0xef, 0x81 } };
extern const __declspec(selectany) GUID UICONTEXT_ToolboxInitialized =          { 0xdc5db425, 0xf0fd, 0x4403, { 0x96, 0xa1, 0xf4, 0x75, 0xcd, 0xba, 0x9e, 0xe0 } };
extern const __declspec(selectany) GUID UICONTEXT_SolutionExistsAndNotBuildingAndNotDebugging =  { 0xd0e4deec, 0x1b53, 0x4cda, { 0x85, 0x59, 0xd4, 0x54, 0x58, 0x3a, 0xd2, 0x3b } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0115_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0115_v0_0_s_ifspec;

#ifndef __IVsToolboxDataProviderRegistry_INTERFACE_DEFINED__
#define __IVsToolboxDataProviderRegistry_INTERFACE_DEFINED__

/* interface IVsToolboxDataProviderRegistry */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsToolboxDataProviderRegistry;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("653BE2DA-BA98-41b7-8ABC-7A38B0E1C01A")
    IVsToolboxDataProviderRegistry : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterDataProvider( 
            /* [in] */ __RPC__in_opt IVsToolboxDataProvider *pDP,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterDataProvider( 
            /* [in] */ VSCOOKIE dwProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolboxDataProviderRegistryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolboxDataProviderRegistry * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolboxDataProviderRegistry * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolboxDataProviderRegistry * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterDataProvider )( 
            IVsToolboxDataProviderRegistry * This,
            /* [in] */ __RPC__in_opt IVsToolboxDataProvider *pDP,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwProvider);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterDataProvider )( 
            IVsToolboxDataProviderRegistry * This,
            /* [in] */ VSCOOKIE dwProvider);
        
        END_INTERFACE
    } IVsToolboxDataProviderRegistryVtbl;

    interface IVsToolboxDataProviderRegistry
    {
        CONST_VTBL struct IVsToolboxDataProviderRegistryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolboxDataProviderRegistry_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolboxDataProviderRegistry_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolboxDataProviderRegistry_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolboxDataProviderRegistry_RegisterDataProvider(This,pDP,pdwProvider)	\
    ( (This)->lpVtbl -> RegisterDataProvider(This,pDP,pdwProvider) ) 

#define IVsToolboxDataProviderRegistry_UnregisterDataProvider(This,dwProvider)	\
    ( (This)->lpVtbl -> UnregisterDataProvider(This,dwProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolboxDataProviderRegistry_INTERFACE_DEFINED__ */


#ifndef __SVsToolboxDataProviderRegistry_INTERFACE_DEFINED__
#define __SVsToolboxDataProviderRegistry_INTERFACE_DEFINED__

/* interface SVsToolboxDataProviderRegistry */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsToolboxDataProviderRegistry;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3AF9E499-91B8-4b54-BD1D-81F414A587C9")
    SVsToolboxDataProviderRegistry : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsToolboxDataProviderRegistryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsToolboxDataProviderRegistry * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsToolboxDataProviderRegistry * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsToolboxDataProviderRegistry * This);
        
        END_INTERFACE
    } SVsToolboxDataProviderRegistryVtbl;

    interface SVsToolboxDataProviderRegistry
    {
        CONST_VTBL struct SVsToolboxDataProviderRegistryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsToolboxDataProviderRegistry_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsToolboxDataProviderRegistry_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsToolboxDataProviderRegistry_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsToolboxDataProviderRegistry_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0117 */
/* [local] */ 

#define SID_SVsToolboxDataProviderRegistry IID_SVsToolboxDataProviderRegistry


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0117_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0117_v0_0_s_ifspec;

#ifndef __IVsFontAndColorCacheManager_INTERFACE_DEFINED__
#define __IVsFontAndColorCacheManager_INTERFACE_DEFINED__

/* interface IVsFontAndColorCacheManager */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsFontAndColorCacheManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("55D3D8C8-F08C-4b31-B70D-FCC52468A5B2")
    IVsFontAndColorCacheManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CheckCache( 
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out BOOL *pfHasData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearCache( 
            /* [in] */ __RPC__in REFGUID rguidCategory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshCache( 
            /* [in] */ __RPC__in REFGUID rguidCategory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckCacheable( 
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out BOOL *pfCacheable) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearAllCaches( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFontAndColorCacheManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFontAndColorCacheManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFontAndColorCacheManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckCache )( 
            IVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out BOOL *pfHasData);
        
        HRESULT ( STDMETHODCALLTYPE *ClearCache )( 
            IVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFGUID rguidCategory);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshCache )( 
            IVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFGUID rguidCategory);
        
        HRESULT ( STDMETHODCALLTYPE *CheckCacheable )( 
            IVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out BOOL *pfCacheable);
        
        HRESULT ( STDMETHODCALLTYPE *ClearAllCaches )( 
            IVsFontAndColorCacheManager * This);
        
        END_INTERFACE
    } IVsFontAndColorCacheManagerVtbl;

    interface IVsFontAndColorCacheManager
    {
        CONST_VTBL struct IVsFontAndColorCacheManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFontAndColorCacheManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFontAndColorCacheManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFontAndColorCacheManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFontAndColorCacheManager_CheckCache(This,rguidCategory,pfHasData)	\
    ( (This)->lpVtbl -> CheckCache(This,rguidCategory,pfHasData) ) 

#define IVsFontAndColorCacheManager_ClearCache(This,rguidCategory)	\
    ( (This)->lpVtbl -> ClearCache(This,rguidCategory) ) 

#define IVsFontAndColorCacheManager_RefreshCache(This,rguidCategory)	\
    ( (This)->lpVtbl -> RefreshCache(This,rguidCategory) ) 

#define IVsFontAndColorCacheManager_CheckCacheable(This,rguidCategory,pfCacheable)	\
    ( (This)->lpVtbl -> CheckCacheable(This,rguidCategory,pfCacheable) ) 

#define IVsFontAndColorCacheManager_ClearAllCaches(This)	\
    ( (This)->lpVtbl -> ClearAllCaches(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFontAndColorCacheManager_INTERFACE_DEFINED__ */


#ifndef __SVsFontAndColorCacheManager_INTERFACE_DEFINED__
#define __SVsFontAndColorCacheManager_INTERFACE_DEFINED__

/* interface SVsFontAndColorCacheManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsFontAndColorCacheManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FB5F088F-1C86-4648-B01C-0A1C40840C51")
    SVsFontAndColorCacheManager : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsFontAndColorCacheManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsFontAndColorCacheManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsFontAndColorCacheManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsFontAndColorCacheManager * This);
        
        END_INTERFACE
    } SVsFontAndColorCacheManagerVtbl;

    interface SVsFontAndColorCacheManager
    {
        CONST_VTBL struct SVsFontAndColorCacheManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsFontAndColorCacheManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsFontAndColorCacheManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsFontAndColorCacheManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsFontAndColorCacheManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0119 */
/* [local] */ 

#define SID_SVsFontAndColorCacheManager IID_SVsFontAndColorCacheManager

enum __VSUL_ERRORLEVEL
    {	VSUL_INFORMATIONAL	= 0,
	VSUL_WARNING	= 0x1,
	VSUL_ERROR	= 0x2,
	VSUL_STATUSMSG	= 0x3,
	VSUL_PROJECT_HYPERLINK	= 0x4
    } ;
typedef DWORD VSUL_ERRORLEVEL;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0119_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0119_v0_0_s_ifspec;

#ifndef __IVsUpgradeLogger_INTERFACE_DEFINED__
#define __IVsUpgradeLogger_INTERFACE_DEFINED__

/* interface IVsUpgradeLogger */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpgradeLogger;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AE88C42E-B3D6-4fec-9C63-C9F1B28233EA")
    IVsUpgradeLogger : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LogMessage( 
            /* [in] */ VSUL_ERRORLEVEL ErrorLevel,
            /* [in] */ __RPC__in BSTR bstrProject,
            /* [in] */ __RPC__in BSTR bstrSource,
            /* [in] */ __RPC__in BSTR bstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Flush( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUpgradeLoggerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUpgradeLogger * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUpgradeLogger * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUpgradeLogger * This);
        
        HRESULT ( STDMETHODCALLTYPE *LogMessage )( 
            IVsUpgradeLogger * This,
            /* [in] */ VSUL_ERRORLEVEL ErrorLevel,
            /* [in] */ __RPC__in BSTR bstrProject,
            /* [in] */ __RPC__in BSTR bstrSource,
            /* [in] */ __RPC__in BSTR bstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *Flush )( 
            IVsUpgradeLogger * This);
        
        END_INTERFACE
    } IVsUpgradeLoggerVtbl;

    interface IVsUpgradeLogger
    {
        CONST_VTBL struct IVsUpgradeLoggerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpgradeLogger_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpgradeLogger_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpgradeLogger_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpgradeLogger_LogMessage(This,ErrorLevel,bstrProject,bstrSource,bstrDescription)	\
    ( (This)->lpVtbl -> LogMessage(This,ErrorLevel,bstrProject,bstrSource,bstrDescription) ) 

#define IVsUpgradeLogger_Flush(This)	\
    ( (This)->lpVtbl -> Flush(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpgradeLogger_INTERFACE_DEFINED__ */


#ifndef __SVsUpgradeLogger_INTERFACE_DEFINED__
#define __SVsUpgradeLogger_INTERFACE_DEFINED__

/* interface SVsUpgradeLogger */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsUpgradeLogger;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DFB0136F-202E-40b5-872E-AE8289A45B59")
    SVsUpgradeLogger : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsUpgradeLoggerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsUpgradeLogger * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsUpgradeLogger * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsUpgradeLogger * This);
        
        END_INTERFACE
    } SVsUpgradeLoggerVtbl;

    interface SVsUpgradeLogger
    {
        CONST_VTBL struct SVsUpgradeLoggerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsUpgradeLogger_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsUpgradeLogger_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsUpgradeLogger_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsUpgradeLogger_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0121 */
/* [local] */ 

#define SID_SVsUpgradeLogger IID_SVsUpgradeLogger
#define SID_IVsUpgradeLogger IID_SVsUpgradeLogger


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0121_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0121_v0_0_s_ifspec;

#ifndef __IVsFileUpgrade_INTERFACE_DEFINED__
#define __IVsFileUpgrade_INTERFACE_DEFINED__

/* interface IVsFileUpgrade */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFileUpgrade;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5D2D55F2-E545-4301-9C22-52BC694CA76C")
    IVsFileUpgrade : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeFile( 
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpgradeFile_CheckOnly( 
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFileUpgradeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFileUpgrade * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFileUpgrade * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFileUpgrade * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeFile )( 
            IVsFileUpgrade * This,
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeFile_CheckOnly )( 
            IVsFileUpgrade * This,
            /* [in] */ __RPC__in BSTR bstrProjectName,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ BOOL bNoBackup,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired);
        
        END_INTERFACE
    } IVsFileUpgradeVtbl;

    interface IVsFileUpgrade
    {
        CONST_VTBL struct IVsFileUpgradeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileUpgrade_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileUpgrade_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileUpgrade_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileUpgrade_UpgradeFile(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,pUpgradeRequired)	\
    ( (This)->lpVtbl -> UpgradeFile(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,pUpgradeRequired) ) 

#define IVsFileUpgrade_UpgradeFile_CheckOnly(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,pUpgradeRequired)	\
    ( (This)->lpVtbl -> UpgradeFile_CheckOnly(This,bstrProjectName,bstrFileName,bNoBackup,pLogger,pUpgradeRequired) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileUpgrade_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0122 */
/* [local] */ 


enum __VSPPROJECTUPGRADEVIAFACTORYFLAGS
    {	PUVFF_SXSBACKUP	= 0x20,
	PUVFF_COPYBACKUP	= 0x40,
	PUVFF_BACKUPSUPPORTED	= 0x80,
	PUVFF_USE_ALT_BACKUP_LOCATION	= 0x100
    } ;
typedef DWORD VSPUVF_FLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0122_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0122_v0_0_s_ifspec;

#ifndef __IVsProjectUpgradeViaFactory_INTERFACE_DEFINED__
#define __IVsProjectUpgradeViaFactory_INTERFACE_DEFINED__

/* interface IVsProjectUpgradeViaFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectUpgradeViaFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0DBA1379-5D67-4a6c-8C06-A5795AF7364B")
    IVsProjectUpgradeViaFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeProject( 
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ VSPUVF_FLAGS fUpgradeFlag,
            /* [in] */ __RPC__in BSTR bstrCopyLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedFullyQualifiedFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpgradeProject_CheckOnly( 
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory,
            /* [out] */ __RPC__out VSPUVF_FLAGS *pUpgradeProjectCapabilityFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccInfo( 
            /* [in] */ __RPC__in BSTR bstrProjectFileName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccProjectName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccAuxPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccLocalPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectUpgradeViaFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectUpgradeViaFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectUpgradeViaFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectUpgradeViaFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProject )( 
            IVsProjectUpgradeViaFactory * This,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ VSPUVF_FLAGS fUpgradeFlag,
            /* [in] */ __RPC__in BSTR bstrCopyLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedFullyQualifiedFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProject_CheckOnly )( 
            IVsProjectUpgradeViaFactory * This,
            /* [in] */ __RPC__in BSTR bstrFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out BOOL *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory,
            /* [out] */ __RPC__out VSPUVF_FLAGS *pUpgradeProjectCapabilityFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccInfo )( 
            IVsProjectUpgradeViaFactory * This,
            /* [in] */ __RPC__in BSTR bstrProjectFileName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccProjectName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccAuxPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSccLocalPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProvider);
        
        END_INTERFACE
    } IVsProjectUpgradeViaFactoryVtbl;

    interface IVsProjectUpgradeViaFactory
    {
        CONST_VTBL struct IVsProjectUpgradeViaFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectUpgradeViaFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectUpgradeViaFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectUpgradeViaFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectUpgradeViaFactory_UpgradeProject(This,bstrFileName,fUpgradeFlag,bstrCopyLocation,pbstrUpgradedFullyQualifiedFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory)	\
    ( (This)->lpVtbl -> UpgradeProject(This,bstrFileName,fUpgradeFlag,bstrCopyLocation,pbstrUpgradedFullyQualifiedFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory) ) 

#define IVsProjectUpgradeViaFactory_UpgradeProject_CheckOnly(This,bstrFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory,pUpgradeProjectCapabilityFlags)	\
    ( (This)->lpVtbl -> UpgradeProject_CheckOnly(This,bstrFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory,pUpgradeProjectCapabilityFlags) ) 

#define IVsProjectUpgradeViaFactory_GetSccInfo(This,bstrProjectFileName,pbstrSccProjectName,pbstrSccAuxPath,pbstrSccLocalPath,pbstrProvider)	\
    ( (This)->lpVtbl -> GetSccInfo(This,bstrProjectFileName,pbstrSccProjectName,pbstrSccAuxPath,pbstrSccLocalPath,pbstrProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectUpgradeViaFactory_INTERFACE_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory2_INTERFACE_DEFINED__
#define __IVsProjectUpgradeViaFactory2_INTERFACE_DEFINED__

/* interface IVsProjectUpgradeViaFactory2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectUpgradeViaFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E5F6CFF6-C3E1-45bc-9559-C03F94FDF15B")
    IVsProjectUpgradeViaFactory2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnUpgradeProjectCancelled( 
            /* [in] */ __RPC__in BSTR bstrFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectUpgradeViaFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectUpgradeViaFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectUpgradeViaFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectUpgradeViaFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnUpgradeProjectCancelled )( 
            IVsProjectUpgradeViaFactory2 * This,
            /* [in] */ __RPC__in BSTR bstrFileName);
        
        END_INTERFACE
    } IVsProjectUpgradeViaFactory2Vtbl;

    interface IVsProjectUpgradeViaFactory2
    {
        CONST_VTBL struct IVsProjectUpgradeViaFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectUpgradeViaFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectUpgradeViaFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectUpgradeViaFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectUpgradeViaFactory2_OnUpgradeProjectCancelled(This,bstrFileName)	\
    ( (This)->lpVtbl -> OnUpgradeProjectCancelled(This,bstrFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectUpgradeViaFactory2_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionEventsProjectUpgrade_INTERFACE_DEFINED__
#define __IVsSolutionEventsProjectUpgrade_INTERFACE_DEFINED__

/* interface IVsSolutionEventsProjectUpgrade */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionEventsProjectUpgrade;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7B1D55C6-4F6A-4865-B9B3-1A696E233065")
    IVsSolutionEventsProjectUpgrade : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterUpgradeProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ VSPUVF_FLAGS fUpgradeFlag,
            /* [in] */ __RPC__in BSTR bstrCopyLocation,
            /* [in] */ SYSTEMTIME stUpgradeTime,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSolutionEventsProjectUpgradeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSolutionEventsProjectUpgrade * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSolutionEventsProjectUpgrade * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSolutionEventsProjectUpgrade * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterUpgradeProject )( 
            IVsSolutionEventsProjectUpgrade * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [in] */ VSPUVF_FLAGS fUpgradeFlag,
            /* [in] */ __RPC__in BSTR bstrCopyLocation,
            /* [in] */ SYSTEMTIME stUpgradeTime,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger);
        
        END_INTERFACE
    } IVsSolutionEventsProjectUpgradeVtbl;

    interface IVsSolutionEventsProjectUpgrade
    {
        CONST_VTBL struct IVsSolutionEventsProjectUpgradeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionEventsProjectUpgrade_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionEventsProjectUpgrade_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionEventsProjectUpgrade_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionEventsProjectUpgrade_OnAfterUpgradeProject(This,pHierarchy,fUpgradeFlag,bstrCopyLocation,stUpgradeTime,pLogger)	\
    ( (This)->lpVtbl -> OnAfterUpgradeProject(This,pHierarchy,fUpgradeFlag,bstrCopyLocation,stUpgradeTime,pLogger) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionEventsProjectUpgrade_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0125 */
/* [local] */ 

typedef 
enum __tagACTIVITYLOG_ENTRYTYPE
    {	ALE_ERROR	= 1,
	ALE_WARNING	= 2,
	ALE_INFORMATION	= 3
    } 	__ACTIVITYLOG_ENTRYTYPE;

typedef DWORD ACTIVITYLOG_ENTRYTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0125_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0125_v0_0_s_ifspec;

#ifndef __IVsActivityLog_INTERFACE_DEFINED__
#define __IVsActivityLog_INTERFACE_DEFINED__

/* interface IVsActivityLog */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsActivityLog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("76AF73F9-A322-42b0-A515-D4D7553508FE")
    IVsActivityLog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LogEntry( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryGuid( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryHr( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ HRESULT hr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryGuidHr( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ HRESULT hr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryPath( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ __RPC__in LPCOLESTR pszPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryGuidPath( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ __RPC__in LPCOLESTR pszPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryHrPath( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ HRESULT hr,
            /* [in] */ __RPC__in LPCOLESTR pszPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogEntryGuidHrPath( 
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ HRESULT hr,
            /* [in] */ __RPC__in LPCOLESTR pszPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsActivityLogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsActivityLog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsActivityLog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsActivityLog * This);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntry )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryGuid )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryHr )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ HRESULT hr);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryGuidHr )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ HRESULT hr);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryPath )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ __RPC__in LPCOLESTR pszPath);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryGuidPath )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ __RPC__in LPCOLESTR pszPath);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryHrPath )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ HRESULT hr,
            /* [in] */ __RPC__in LPCOLESTR pszPath);
        
        HRESULT ( STDMETHODCALLTYPE *LogEntryGuidHrPath )( 
            IVsActivityLog * This,
            /* [in] */ ACTIVITYLOG_ENTRYTYPE actType,
            /* [in] */ __RPC__in LPCOLESTR pszSource,
            /* [in] */ __RPC__in LPCOLESTR pszDescription,
            /* [in] */ GUID guid,
            /* [in] */ HRESULT hr,
            /* [in] */ __RPC__in LPCOLESTR pszPath);
        
        END_INTERFACE
    } IVsActivityLogVtbl;

    interface IVsActivityLog
    {
        CONST_VTBL struct IVsActivityLogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsActivityLog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsActivityLog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsActivityLog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsActivityLog_LogEntry(This,actType,pszSource,pszDescription)	\
    ( (This)->lpVtbl -> LogEntry(This,actType,pszSource,pszDescription) ) 

#define IVsActivityLog_LogEntryGuid(This,actType,pszSource,pszDescription,guid)	\
    ( (This)->lpVtbl -> LogEntryGuid(This,actType,pszSource,pszDescription,guid) ) 

#define IVsActivityLog_LogEntryHr(This,actType,pszSource,pszDescription,hr)	\
    ( (This)->lpVtbl -> LogEntryHr(This,actType,pszSource,pszDescription,hr) ) 

#define IVsActivityLog_LogEntryGuidHr(This,actType,pszSource,pszDescription,guid,hr)	\
    ( (This)->lpVtbl -> LogEntryGuidHr(This,actType,pszSource,pszDescription,guid,hr) ) 

#define IVsActivityLog_LogEntryPath(This,actType,pszSource,pszDescription,pszPath)	\
    ( (This)->lpVtbl -> LogEntryPath(This,actType,pszSource,pszDescription,pszPath) ) 

#define IVsActivityLog_LogEntryGuidPath(This,actType,pszSource,pszDescription,guid,pszPath)	\
    ( (This)->lpVtbl -> LogEntryGuidPath(This,actType,pszSource,pszDescription,guid,pszPath) ) 

#define IVsActivityLog_LogEntryHrPath(This,actType,pszSource,pszDescription,hr,pszPath)	\
    ( (This)->lpVtbl -> LogEntryHrPath(This,actType,pszSource,pszDescription,hr,pszPath) ) 

#define IVsActivityLog_LogEntryGuidHrPath(This,actType,pszSource,pszDescription,guid,hr,pszPath)	\
    ( (This)->lpVtbl -> LogEntryGuidHrPath(This,actType,pszSource,pszDescription,guid,hr,pszPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsActivityLog_INTERFACE_DEFINED__ */


#ifndef __SVsActivityLog_INTERFACE_DEFINED__
#define __SVsActivityLog_INTERFACE_DEFINED__

/* interface SVsActivityLog */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsActivityLog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("24367A32-EF56-405b-A395-5CF2BCCB2283")
    SVsActivityLog : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsActivityLogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsActivityLog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsActivityLog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsActivityLog * This);
        
        END_INTERFACE
    } SVsActivityLogVtbl;

    interface SVsActivityLog
    {
        CONST_VTBL struct SVsActivityLogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsActivityLog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsActivityLog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsActivityLog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsActivityLog_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0127 */
/* [local] */ 

#define SID_SVsActivityLog IID_SVsActivityLog


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0127_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0127_v0_0_s_ifspec;

#ifndef __IVsPersistDocData3_INTERFACE_DEFINED__
#define __IVsPersistDocData3_INTERFACE_DEFINED__

/* interface IVsPersistDocData3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPersistDocData3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DEC057F4-46D1-4BD3-9D63-21E5E3F19368")
    IVsPersistDocData3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HandsOffDocDataStorage( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HandsOnDocDataStorage( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPersistDocData3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPersistDocData3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPersistDocData3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPersistDocData3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOffDocDataStorage )( 
            IVsPersistDocData3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOnDocDataStorage )( 
            IVsPersistDocData3 * This);
        
        END_INTERFACE
    } IVsPersistDocData3Vtbl;

    interface IVsPersistDocData3
    {
        CONST_VTBL struct IVsPersistDocData3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPersistDocData3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPersistDocData3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPersistDocData3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPersistDocData3_HandsOffDocDataStorage(This)	\
    ( (This)->lpVtbl -> HandsOffDocDataStorage(This) ) 

#define IVsPersistDocData3_HandsOnDocDataStorage(This)	\
    ( (This)->lpVtbl -> HandsOnDocDataStorage(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPersistDocData3_INTERFACE_DEFINED__ */


#ifndef __IVsWindowFrame2_INTERFACE_DEFINED__
#define __IVsWindowFrame2_INTERFACE_DEFINED__

/* interface IVsWindowFrame2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWindowFrame2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("801885A0-9DC6-4e34-B064-1C3228F66794")
    IVsWindowFrame2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Advise( 
            /* [in] */ __RPC__in_opt IVsWindowFrameNotify *pNotify,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Unadvise( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ActivateOwnerDockedWindow( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWindowFrame2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWindowFrame2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWindowFrame2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWindowFrame2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Advise )( 
            IVsWindowFrame2 * This,
            /* [in] */ __RPC__in_opt IVsWindowFrameNotify *pNotify,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *Unadvise )( 
            IVsWindowFrame2 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateOwnerDockedWindow )( 
            IVsWindowFrame2 * This);
        
        END_INTERFACE
    } IVsWindowFrame2Vtbl;

    interface IVsWindowFrame2
    {
        CONST_VTBL struct IVsWindowFrame2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowFrame2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowFrame2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowFrame2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowFrame2_Advise(This,pNotify,pdwCookie)	\
    ( (This)->lpVtbl -> Advise(This,pNotify,pdwCookie) ) 

#define IVsWindowFrame2_Unadvise(This,dwCookie)	\
    ( (This)->lpVtbl -> Unadvise(This,dwCookie) ) 

#define IVsWindowFrame2_ActivateOwnerDockedWindow(This)	\
    ( (This)->lpVtbl -> ActivateOwnerDockedWindow(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowFrame2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0129 */
/* [local] */ 


enum __VSPPPID
    {	VSPPPID_FIRST	= 1,
	VSPPPID_PAGENAME	= 1,
	VSPPPID_LAST	= 1
    } ;
typedef DWORD VSPPPID;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0129_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0129_v0_0_s_ifspec;

#ifndef __IVsPropertyPage2_INTERFACE_DEFINED__
#define __IVsPropertyPage2_INTERFACE_DEFINED__

/* interface IVsPropertyPage2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPropertyPage2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6FC6A958-B2E7-441b-823C-10EA30B24EEC")
    IVsPropertyPage2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ VSPPPID propid,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProperty( 
            /* [in] */ VSPPPID propid,
            /* [in] */ VARIANT var) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPropertyPage2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPropertyPage2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPropertyPage2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPropertyPage2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsPropertyPage2 * This,
            /* [in] */ VSPPPID propid,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *SetProperty )( 
            IVsPropertyPage2 * This,
            /* [in] */ VSPPPID propid,
            /* [in] */ VARIANT var);
        
        END_INTERFACE
    } IVsPropertyPage2Vtbl;

    interface IVsPropertyPage2
    {
        CONST_VTBL struct IVsPropertyPage2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPropertyPage2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPropertyPage2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPropertyPage2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPropertyPage2_GetProperty(This,propid,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,propid,pvar) ) 

#define IVsPropertyPage2_SetProperty(This,propid,var)	\
    ( (This)->lpVtbl -> SetProperty(This,propid,var) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPropertyPage2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0130 */
/* [local] */ 


enum __FRAMESHOW2
    {	FRAMESHOW_BeforeWinHidden	= 10,
	FRAMESHOW_AutoHideSlideEnd	= 11
    } ;
typedef BOOL FRAMESHOW2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0130_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0130_v0_0_s_ifspec;

#ifndef __IVsWindowFrameNotify3_INTERFACE_DEFINED__
#define __IVsWindowFrameNotify3_INTERFACE_DEFINED__

/* interface IVsWindowFrameNotify3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWindowFrameNotify3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8C213AC2-FF13-4361-9FC5-39D368D26CD3")
    IVsWindowFrameNotify3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnShow( 
            /* [in] */ FRAMESHOW2 fShow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMove( 
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnSize( 
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnDockableChange( 
            /* [in] */ BOOL fDockable,
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnClose( 
            /* [out][in] */ __RPC__inout FRAMECLOSE *pgrfSaveOptions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWindowFrameNotify3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWindowFrameNotify3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWindowFrameNotify3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWindowFrameNotify3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnShow )( 
            IVsWindowFrameNotify3 * This,
            /* [in] */ FRAMESHOW2 fShow);
        
        HRESULT ( STDMETHODCALLTYPE *OnMove )( 
            IVsWindowFrameNotify3 * This,
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h);
        
        HRESULT ( STDMETHODCALLTYPE *OnSize )( 
            IVsWindowFrameNotify3 * This,
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h);
        
        HRESULT ( STDMETHODCALLTYPE *OnDockableChange )( 
            IVsWindowFrameNotify3 * This,
            /* [in] */ BOOL fDockable,
            /* [in] */ int x,
            /* [in] */ int y,
            /* [in] */ int w,
            /* [in] */ int h);
        
        HRESULT ( STDMETHODCALLTYPE *OnClose )( 
            IVsWindowFrameNotify3 * This,
            /* [out][in] */ __RPC__inout FRAMECLOSE *pgrfSaveOptions);
        
        END_INTERFACE
    } IVsWindowFrameNotify3Vtbl;

    interface IVsWindowFrameNotify3
    {
        CONST_VTBL struct IVsWindowFrameNotify3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowFrameNotify3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowFrameNotify3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowFrameNotify3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowFrameNotify3_OnShow(This,fShow)	\
    ( (This)->lpVtbl -> OnShow(This,fShow) ) 

#define IVsWindowFrameNotify3_OnMove(This,x,y,w,h)	\
    ( (This)->lpVtbl -> OnMove(This,x,y,w,h) ) 

#define IVsWindowFrameNotify3_OnSize(This,x,y,w,h)	\
    ( (This)->lpVtbl -> OnSize(This,x,y,w,h) ) 

#define IVsWindowFrameNotify3_OnDockableChange(This,fDockable,x,y,w,h)	\
    ( (This)->lpVtbl -> OnDockableChange(This,fDockable,x,y,w,h) ) 

#define IVsWindowFrameNotify3_OnClose(This,pgrfSaveOptions)	\
    ( (This)->lpVtbl -> OnClose(This,pgrfSaveOptions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowFrameNotify3_INTERFACE_DEFINED__ */


#ifndef __IVsPackageDynamicToolOwnerEx_INTERFACE_DEFINED__
#define __IVsPackageDynamicToolOwnerEx_INTERFACE_DEFINED__

/* interface IVsPackageDynamicToolOwnerEx */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPackageDynamicToolOwnerEx;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("91C30F81-E72A-4997-9B07-A0AECB8C9169")
    IVsPackageDynamicToolOwnerEx : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryShowTool( 
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__out BOOL *pfShowTool) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPackageDynamicToolOwnerExVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPackageDynamicToolOwnerEx * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPackageDynamicToolOwnerEx * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPackageDynamicToolOwnerEx * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryShowTool )( 
            IVsPackageDynamicToolOwnerEx * This,
            /* [in] */ __RPC__in REFGUID rguidPersistenceSlot,
            /* [in] */ DWORD dwID,
            /* [out] */ __RPC__out BOOL *pfShowTool);
        
        END_INTERFACE
    } IVsPackageDynamicToolOwnerExVtbl;

    interface IVsPackageDynamicToolOwnerEx
    {
        CONST_VTBL struct IVsPackageDynamicToolOwnerExVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPackageDynamicToolOwnerEx_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPackageDynamicToolOwnerEx_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPackageDynamicToolOwnerEx_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPackageDynamicToolOwnerEx_QueryShowTool(This,rguidPersistenceSlot,dwID,pfShowTool)	\
    ( (This)->lpVtbl -> QueryShowTool(This,rguidPersistenceSlot,dwID,pfShowTool) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPackageDynamicToolOwnerEx_INTERFACE_DEFINED__ */


#ifndef __IVsContextualIntellisenseFilter_INTERFACE_DEFINED__
#define __IVsContextualIntellisenseFilter_INTERFACE_DEFINED__

/* interface IVsContextualIntellisenseFilter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsContextualIntellisenseFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C0C5974A-288D-4719-AB48-9CB812B5E895")
    IVsContextualIntellisenseFilter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTypeVisible( 
            /* [in] */ __RPC__in LPCOLESTR szTypeName,
            /* [out] */ __RPC__out BOOL *pfVisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsMemberVisible( 
            /* [in] */ __RPC__in LPCOLESTR szMemberSignature,
            /* [out] */ __RPC__out BOOL *pfVisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsContextualIntellisenseFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsContextualIntellisenseFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsContextualIntellisenseFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsContextualIntellisenseFilter * This);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IVsContextualIntellisenseFilter * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *IsTypeVisible )( 
            IVsContextualIntellisenseFilter * This,
            /* [in] */ __RPC__in LPCOLESTR szTypeName,
            /* [out] */ __RPC__out BOOL *pfVisible);
        
        HRESULT ( STDMETHODCALLTYPE *IsMemberVisible )( 
            IVsContextualIntellisenseFilter * This,
            /* [in] */ __RPC__in LPCOLESTR szMemberSignature,
            /* [out] */ __RPC__out BOOL *pfVisible);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsContextualIntellisenseFilter * This);
        
        END_INTERFACE
    } IVsContextualIntellisenseFilterVtbl;

    interface IVsContextualIntellisenseFilter
    {
        CONST_VTBL struct IVsContextualIntellisenseFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsContextualIntellisenseFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsContextualIntellisenseFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsContextualIntellisenseFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsContextualIntellisenseFilter_Initialize(This,pHierarchy)	\
    ( (This)->lpVtbl -> Initialize(This,pHierarchy) ) 

#define IVsContextualIntellisenseFilter_IsTypeVisible(This,szTypeName,pfVisible)	\
    ( (This)->lpVtbl -> IsTypeVisible(This,szTypeName,pfVisible) ) 

#define IVsContextualIntellisenseFilter_IsMemberVisible(This,szMemberSignature,pfVisible)	\
    ( (This)->lpVtbl -> IsMemberVisible(This,szMemberSignature,pfVisible) ) 

#define IVsContextualIntellisenseFilter_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsContextualIntellisenseFilter_INTERFACE_DEFINED__ */


#ifndef __IVsContextualIntellisenseFilterProvider_INTERFACE_DEFINED__
#define __IVsContextualIntellisenseFilterProvider_INTERFACE_DEFINED__

/* interface IVsContextualIntellisenseFilterProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsContextualIntellisenseFilterProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F7827DCE-0B39-474d-8D48-FE8100C9044C")
    IVsContextualIntellisenseFilterProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFilter( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__deref_out_opt IVsContextualIntellisenseFilter **ppFilter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsContextualIntellisenseFilterProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsContextualIntellisenseFilterProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsContextualIntellisenseFilterProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsContextualIntellisenseFilterProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilter )( 
            IVsContextualIntellisenseFilterProvider * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__deref_out_opt IVsContextualIntellisenseFilter **ppFilter);
        
        END_INTERFACE
    } IVsContextualIntellisenseFilterProviderVtbl;

    interface IVsContextualIntellisenseFilterProvider
    {
        CONST_VTBL struct IVsContextualIntellisenseFilterProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsContextualIntellisenseFilterProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsContextualIntellisenseFilterProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsContextualIntellisenseFilterProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsContextualIntellisenseFilterProvider_GetFilter(This,pHierarchy,ppFilter)	\
    ( (This)->lpVtbl -> GetFilter(This,pHierarchy,ppFilter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsContextualIntellisenseFilterProvider_INTERFACE_DEFINED__ */


#ifndef __IVsToolboxActiveUserHook_INTERFACE_DEFINED__
#define __IVsToolboxActiveUserHook_INTERFACE_DEFINED__

/* interface IVsToolboxActiveUserHook */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolboxActiveUserHook;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A00C298A-6520-4822-ABD8-C5CD03846599")
    IVsToolboxActiveUserHook : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InterceptDataObject( 
            /* [in] */ __RPC__in_opt IDataObject *pIn,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToolboxSelectionChanged( 
            /* [in] */ __RPC__in_opt IDataObject *pSelected) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolboxActiveUserHookVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolboxActiveUserHook * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolboxActiveUserHook * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolboxActiveUserHook * This);
        
        HRESULT ( STDMETHODCALLTYPE *InterceptDataObject )( 
            IVsToolboxActiveUserHook * This,
            /* [in] */ __RPC__in_opt IDataObject *pIn,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppOut);
        
        HRESULT ( STDMETHODCALLTYPE *ToolboxSelectionChanged )( 
            IVsToolboxActiveUserHook * This,
            /* [in] */ __RPC__in_opt IDataObject *pSelected);
        
        END_INTERFACE
    } IVsToolboxActiveUserHookVtbl;

    interface IVsToolboxActiveUserHook
    {
        CONST_VTBL struct IVsToolboxActiveUserHookVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolboxActiveUserHook_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolboxActiveUserHook_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolboxActiveUserHook_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolboxActiveUserHook_InterceptDataObject(This,pIn,ppOut)	\
    ( (This)->lpVtbl -> InterceptDataObject(This,pIn,ppOut) ) 

#define IVsToolboxActiveUserHook_ToolboxSelectionChanged(This,pSelected)	\
    ( (This)->lpVtbl -> ToolboxSelectionChanged(This,pSelected) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolboxActiveUserHook_INTERFACE_DEFINED__ */


#ifndef __IVsDefaultToolboxTabState_INTERFACE_DEFINED__
#define __IVsDefaultToolboxTabState_INTERFACE_DEFINED__

/* interface IVsDefaultToolboxTabState */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDefaultToolboxTabState;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1749A33B-2099-4a22-B32A-E96ADD887142")
    IVsDefaultToolboxTabState : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultTabExpansion( 
            /* [in] */ __RPC__in LPCOLESTR pszTabID,
            /* [out] */ __RPC__out BOOL *pfExpanded) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDefaultToolboxTabStateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDefaultToolboxTabState * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDefaultToolboxTabState * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDefaultToolboxTabState * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultTabExpansion )( 
            IVsDefaultToolboxTabState * This,
            /* [in] */ __RPC__in LPCOLESTR pszTabID,
            /* [out] */ __RPC__out BOOL *pfExpanded);
        
        END_INTERFACE
    } IVsDefaultToolboxTabStateVtbl;

    interface IVsDefaultToolboxTabState
    {
        CONST_VTBL struct IVsDefaultToolboxTabStateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDefaultToolboxTabState_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDefaultToolboxTabState_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDefaultToolboxTabState_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDefaultToolboxTabState_GetDefaultTabExpansion(This,pszTabID,pfExpanded)	\
    ( (This)->lpVtbl -> GetDefaultTabExpansion(This,pszTabID,pfExpanded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDefaultToolboxTabState_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0136 */
/* [local] */ 


enum __VSPROFILEPATHRESOLVERFLAGS
    {	VSPPR_None	= 0,
	VSPPR_OnlyForProfiles	= 0x1
    } ;
typedef DWORD VSPROFILEPATHRESOLVERFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0136_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0136_v0_0_s_ifspec;

#ifndef __IVsPathVariableResolver_INTERFACE_DEFINED__
#define __IVsPathVariableResolver_INTERFACE_DEFINED__

/* interface IVsPathVariableResolver */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPathVariableResolver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("17A4EF87-4472-47f6-B066-FE96036678D0")
    IVsPathVariableResolver : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolvePath( 
            /* [in] */ __RPC__in LPCOLESTR strEncodedPath,
            /* [in] */ VSPROFILEPATHRESOLVERFLAGS dwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodePath( 
            /* [in] */ __RPC__in LPCOLESTR strPath,
            /* [in] */ VSPROFILEPATHRESOLVERFLAGS dwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEncodedPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsPathVariableResolverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsPathVariableResolver * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsPathVariableResolver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsPathVariableResolver * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolvePath )( 
            IVsPathVariableResolver * This,
            /* [in] */ __RPC__in LPCOLESTR strEncodedPath,
            /* [in] */ VSPROFILEPATHRESOLVERFLAGS dwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        HRESULT ( STDMETHODCALLTYPE *EncodePath )( 
            IVsPathVariableResolver * This,
            /* [in] */ __RPC__in LPCOLESTR strPath,
            /* [in] */ VSPROFILEPATHRESOLVERFLAGS dwFlags,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrEncodedPath);
        
        END_INTERFACE
    } IVsPathVariableResolverVtbl;

    interface IVsPathVariableResolver
    {
        CONST_VTBL struct IVsPathVariableResolverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPathVariableResolver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPathVariableResolver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPathVariableResolver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPathVariableResolver_ResolvePath(This,strEncodedPath,dwFlags,pbstrPath)	\
    ( (This)->lpVtbl -> ResolvePath(This,strEncodedPath,dwFlags,pbstrPath) ) 

#define IVsPathVariableResolver_EncodePath(This,strPath,dwFlags,pbstrEncodedPath)	\
    ( (This)->lpVtbl -> EncodePath(This,strPath,dwFlags,pbstrEncodedPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPathVariableResolver_INTERFACE_DEFINED__ */


#ifndef __SVsPathVariableResolver_INTERFACE_DEFINED__
#define __SVsPathVariableResolver_INTERFACE_DEFINED__

/* interface SVsPathVariableResolver */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsPathVariableResolver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("89DB7429-F4A4-4f26-B832-2EB963A19AAA")
    SVsPathVariableResolver : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsPathVariableResolverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsPathVariableResolver * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsPathVariableResolver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsPathVariableResolver * This);
        
        END_INTERFACE
    } SVsPathVariableResolverVtbl;

    interface SVsPathVariableResolver
    {
        CONST_VTBL struct SVsPathVariableResolverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsPathVariableResolver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsPathVariableResolver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsPathVariableResolver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsPathVariableResolver_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0138 */
/* [local] */ 

#define SID_SVsPathVariableResolver IID_SVsPathVariableResolver

enum __VSASYNCHOPENPROJECTTYPE
    {	AOPT_SYNCHRONOUS	= 0,
	AOPT_ASYNCHRONOUS	= 0x1
    } ;
typedef DWORD VSASYNCHOPENPROJECTTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0138_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0138_v0_0_s_ifspec;

#ifndef __IVsProjectFactory2_INTERFACE_DEFINED__
#define __IVsProjectFactory2_INTERFACE_DEFINED__

/* interface IVsProjectFactory2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("55E1A1E1-ECAC-46e0-BDE3-1E35BC68C31C")
    IVsProjectFactory2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAsynchOpenProjectType( 
            /* [retval][out] */ __RPC__out VSASYNCHOPENPROJECTTYPE *pType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAsynchOpenProjectType )( 
            IVsProjectFactory2 * This,
            /* [retval][out] */ __RPC__out VSASYNCHOPENPROJECTTYPE *pType);
        
        END_INTERFACE
    } IVsProjectFactory2Vtbl;

    interface IVsProjectFactory2
    {
        CONST_VTBL struct IVsProjectFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFactory2_GetAsynchOpenProjectType(This,pType)	\
    ( (This)->lpVtbl -> GetAsynchOpenProjectType(This,pType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFactory2_INTERFACE_DEFINED__ */


#ifndef __IVsAsynchOpenFromSccProjectEvents_INTERFACE_DEFINED__
#define __IVsAsynchOpenFromSccProjectEvents_INTERFACE_DEFINED__

/* interface IVsAsynchOpenFromSccProjectEvents */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsAsynchOpenFromSccProjectEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C31C30EF-3B22-4f02-93BB-BCDA5FA192AA")
    IVsAsynchOpenFromSccProjectEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnFilesDownloaded( 
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullPaths[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnLoadComplete( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnLoadFailed( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAsynchOpenFromSccProjectEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAsynchOpenFromSccProjectEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAsynchOpenFromSccProjectEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAsynchOpenFromSccProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnFilesDownloaded )( 
            IVsAsynchOpenFromSccProjectEvents * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullPaths[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnLoadComplete )( 
            IVsAsynchOpenFromSccProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnLoadFailed )( 
            IVsAsynchOpenFromSccProjectEvents * This);
        
        END_INTERFACE
    } IVsAsynchOpenFromSccProjectEventsVtbl;

    interface IVsAsynchOpenFromSccProjectEvents
    {
        CONST_VTBL struct IVsAsynchOpenFromSccProjectEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAsynchOpenFromSccProjectEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAsynchOpenFromSccProjectEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAsynchOpenFromSccProjectEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAsynchOpenFromSccProjectEvents_OnFilesDownloaded(This,cFiles,rgpszFullPaths)	\
    ( (This)->lpVtbl -> OnFilesDownloaded(This,cFiles,rgpszFullPaths) ) 

#define IVsAsynchOpenFromSccProjectEvents_OnLoadComplete(This)	\
    ( (This)->lpVtbl -> OnLoadComplete(This) ) 

#define IVsAsynchOpenFromSccProjectEvents_OnLoadFailed(This)	\
    ( (This)->lpVtbl -> OnLoadFailed(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAsynchOpenFromSccProjectEvents_INTERFACE_DEFINED__ */


#ifndef __IVsAsynchOpenFromScc_INTERFACE_DEFINED__
#define __IVsAsynchOpenFromScc_INTERFACE_DEFINED__

/* interface IVsAsynchOpenFromScc */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsAsynchOpenFromScc;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("99871A31-DB02-4da9-98FB-89D8EDC700CE")
    IVsAsynchOpenFromScc : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadProjectAsynchronously( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__out BOOL *pReturnValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadProject( 
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsLoadingContent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__out BOOL *pfIsLoading) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAsynchOpenFromSccVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAsynchOpenFromScc * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAsynchOpenFromScc * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAsynchOpenFromScc * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadProjectAsynchronously )( 
            IVsAsynchOpenFromScc * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath,
            /* [out] */ __RPC__out BOOL *pReturnValue);
        
        HRESULT ( STDMETHODCALLTYPE *LoadProject )( 
            IVsAsynchOpenFromScc * This,
            /* [in] */ __RPC__in LPCOLESTR lpszProjectPath);
        
        HRESULT ( STDMETHODCALLTYPE *IsLoadingContent )( 
            IVsAsynchOpenFromScc * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierarchy,
            /* [out] */ __RPC__out BOOL *pfIsLoading);
        
        END_INTERFACE
    } IVsAsynchOpenFromSccVtbl;

    interface IVsAsynchOpenFromScc
    {
        CONST_VTBL struct IVsAsynchOpenFromSccVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAsynchOpenFromScc_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAsynchOpenFromScc_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAsynchOpenFromScc_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAsynchOpenFromScc_LoadProjectAsynchronously(This,lpszProjectPath,pReturnValue)	\
    ( (This)->lpVtbl -> LoadProjectAsynchronously(This,lpszProjectPath,pReturnValue) ) 

#define IVsAsynchOpenFromScc_LoadProject(This,lpszProjectPath)	\
    ( (This)->lpVtbl -> LoadProject(This,lpszProjectPath) ) 

#define IVsAsynchOpenFromScc_IsLoadingContent(This,pHierarchy,pfIsLoading)	\
    ( (This)->lpVtbl -> IsLoadingContent(This,pHierarchy,pfIsLoading) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAsynchOpenFromScc_INTERFACE_DEFINED__ */


#ifndef __IVsHierarchyDeleteHandler2_INTERFACE_DEFINED__
#define __IVsHierarchyDeleteHandler2_INTERFACE_DEFINED__

/* interface IVsHierarchyDeleteHandler2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyDeleteHandler2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("78FD1CBD-387B-4262-BD7B-65C34F86356E")
    IVsHierarchyDeleteHandler2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowSpecificDeleteRemoveMessage( 
            /* [in] */ DWORD dwDelItemOps,
            /* [in] */ ULONG cDelItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cDelItems) VSITEMID rgDelItems[  ],
            /* [out] */ __RPC__out BOOL *pfShowStandardMessage,
            /* [out] */ __RPC__out VSDELETEITEMOPERATION *pdwDelItemOp) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowMultiSelDeleteOrRemoveMessage( 
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [in] */ ULONG cDelItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cDelItems) VSITEMID rgDelItems[  ],
            /* [out] */ __RPC__out BOOL *pfCancelOperation) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHierarchyDeleteHandler2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHierarchyDeleteHandler2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHierarchyDeleteHandler2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHierarchyDeleteHandler2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowSpecificDeleteRemoveMessage )( 
            IVsHierarchyDeleteHandler2 * This,
            /* [in] */ DWORD dwDelItemOps,
            /* [in] */ ULONG cDelItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cDelItems) VSITEMID rgDelItems[  ],
            /* [out] */ __RPC__out BOOL *pfShowStandardMessage,
            /* [out] */ __RPC__out VSDELETEITEMOPERATION *pdwDelItemOp);
        
        HRESULT ( STDMETHODCALLTYPE *ShowMultiSelDeleteOrRemoveMessage )( 
            IVsHierarchyDeleteHandler2 * This,
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [in] */ ULONG cDelItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cDelItems) VSITEMID rgDelItems[  ],
            /* [out] */ __RPC__out BOOL *pfCancelOperation);
        
        END_INTERFACE
    } IVsHierarchyDeleteHandler2Vtbl;

    interface IVsHierarchyDeleteHandler2
    {
        CONST_VTBL struct IVsHierarchyDeleteHandler2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyDeleteHandler2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyDeleteHandler2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyDeleteHandler2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyDeleteHandler2_ShowSpecificDeleteRemoveMessage(This,dwDelItemOps,cDelItems,rgDelItems,pfShowStandardMessage,pdwDelItemOp)	\
    ( (This)->lpVtbl -> ShowSpecificDeleteRemoveMessage(This,dwDelItemOps,cDelItems,rgDelItems,pfShowStandardMessage,pdwDelItemOp) ) 

#define IVsHierarchyDeleteHandler2_ShowMultiSelDeleteOrRemoveMessage(This,dwDelItemOp,cDelItems,rgDelItems,pfCancelOperation)	\
    ( (This)->lpVtbl -> ShowMultiSelDeleteOrRemoveMessage(This,dwDelItemOp,cDelItems,rgDelItems,pfCancelOperation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyDeleteHandler2_INTERFACE_DEFINED__ */


#ifndef __IVsToolbox3_INTERFACE_DEFINED__
#define __IVsToolbox3_INTERFACE_DEFINED__

/* interface IVsToolbox3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolbox3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5C67B771-43AD-4bcf-9342-E82CF8E4CBFD")
    IVsToolbox3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetIDOfTab( 
            /* [in] */ __RPC__in LPCOLESTR lpszTabName,
            /* [in] */ __RPC__in LPCOLESTR lpszTabID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIDOfTab( 
            /* [in] */ __RPC__in LPCOLESTR lpszTabName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTabOfID( 
            /* [in] */ __RPC__in LPCOLESTR lpszTabID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGeneralTabID( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemID( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemDisplayName( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastModifiedTime( 
            /* [out] */ __RPC__out SYSTEMTIME *pst) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolbox3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolbox3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolbox3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetIDOfTab )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszTabName,
            /* [in] */ __RPC__in LPCOLESTR lpszTabID);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDOfTab )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszTabName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabID);
        
        HRESULT ( STDMETHODCALLTYPE *GetTabOfID )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszTabID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabName);
        
        HRESULT ( STDMETHODCALLTYPE *GetGeneralTabID )( 
            IVsToolbox3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTabID);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemID )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrID);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemDisplayName )( 
            IVsToolbox3 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastModifiedTime )( 
            IVsToolbox3 * This,
            /* [out] */ __RPC__out SYSTEMTIME *pst);
        
        END_INTERFACE
    } IVsToolbox3Vtbl;

    interface IVsToolbox3
    {
        CONST_VTBL struct IVsToolbox3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolbox3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolbox3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolbox3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolbox3_SetIDOfTab(This,lpszTabName,lpszTabID)	\
    ( (This)->lpVtbl -> SetIDOfTab(This,lpszTabName,lpszTabID) ) 

#define IVsToolbox3_GetIDOfTab(This,lpszTabName,pbstrTabID)	\
    ( (This)->lpVtbl -> GetIDOfTab(This,lpszTabName,pbstrTabID) ) 

#define IVsToolbox3_GetTabOfID(This,lpszTabID,pbstrTabName)	\
    ( (This)->lpVtbl -> GetTabOfID(This,lpszTabID,pbstrTabName) ) 

#define IVsToolbox3_GetGeneralTabID(This,pbstrTabID)	\
    ( (This)->lpVtbl -> GetGeneralTabID(This,pbstrTabID) ) 

#define IVsToolbox3_GetItemID(This,pDO,pbstrID)	\
    ( (This)->lpVtbl -> GetItemID(This,pDO,pbstrID) ) 

#define IVsToolbox3_GetItemDisplayName(This,pDO,pbstrName)	\
    ( (This)->lpVtbl -> GetItemDisplayName(This,pDO,pbstrName) ) 

#define IVsToolbox3_GetLastModifiedTime(This,pst)	\
    ( (This)->lpVtbl -> GetLastModifiedTime(This,pst) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolbox3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0143 */
/* [local] */ 

#define	lpszToolboxTipFieldName	( L"Name" )

#define	lpszToolboxTipFieldVersion	( L"Version" )

#define	lpszToolboxTipFieldCompany	( L"Company" )

#define	lpszToolboxTipFieldComponentType	( L"ComponentType" )

#define	lpszToolboxTipFieldDescription	( L"Description" )



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0143_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0143_v0_0_s_ifspec;

#ifndef __IVsToolboxDataProvider2_INTERFACE_DEFINED__
#define __IVsToolboxDataProvider2_INTERFACE_DEFINED__

/* interface IVsToolboxDataProvider2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolboxDataProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1CD73232-A3C7-48fa-8B0A-2E35804097BF")
    IVsToolboxDataProvider2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPackageGUID( 
            /* [out] */ __RPC__out GUID *pguidPkg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUniqueID( 
            /* [out] */ __RPC__out GUID *pguidID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemTipInfo( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [in] */ __RPC__in LPCOLESTR lpszCurrentName,
            /* [in] */ __RPC__in_opt IPropertyBag *pStrings) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProfileData( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemID( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReconstituteItem( 
            /* [in] */ __RPC__in LPCOLESTR lpszCurrentName,
            /* [in] */ __RPC__in LPCOLESTR lpszID,
            /* [in] */ __RPC__in LPCOLESTR lpszData,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppDO,
            /* [out] */ __RPC__out TBXITEMINFO *ptif) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolboxDataProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolboxDataProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolboxDataProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolboxDataProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPackageGUID )( 
            IVsToolboxDataProvider2 * This,
            /* [out] */ __RPC__out GUID *pguidPkg);
        
        HRESULT ( STDMETHODCALLTYPE *GetUniqueID )( 
            IVsToolboxDataProvider2 * This,
            /* [out] */ __RPC__out GUID *pguidID);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVsToolboxDataProvider2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemTipInfo )( 
            IVsToolboxDataProvider2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [in] */ __RPC__in LPCOLESTR lpszCurrentName,
            /* [in] */ __RPC__in_opt IPropertyBag *pStrings);
        
        HRESULT ( STDMETHODCALLTYPE *GetProfileData )( 
            IVsToolboxDataProvider2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrData);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemID )( 
            IVsToolboxDataProvider2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrID);
        
        HRESULT ( STDMETHODCALLTYPE *ReconstituteItem )( 
            IVsToolboxDataProvider2 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszCurrentName,
            /* [in] */ __RPC__in LPCOLESTR lpszID,
            /* [in] */ __RPC__in LPCOLESTR lpszData,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppDO,
            /* [out] */ __RPC__out TBXITEMINFO *ptif);
        
        END_INTERFACE
    } IVsToolboxDataProvider2Vtbl;

    interface IVsToolboxDataProvider2
    {
        CONST_VTBL struct IVsToolboxDataProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolboxDataProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolboxDataProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolboxDataProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolboxDataProvider2_GetPackageGUID(This,pguidPkg)	\
    ( (This)->lpVtbl -> GetPackageGUID(This,pguidPkg) ) 

#define IVsToolboxDataProvider2_GetUniqueID(This,pguidID)	\
    ( (This)->lpVtbl -> GetUniqueID(This,pguidID) ) 

#define IVsToolboxDataProvider2_GetDisplayName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pbstrName) ) 

#define IVsToolboxDataProvider2_GetItemTipInfo(This,pDO,lpszCurrentName,pStrings)	\
    ( (This)->lpVtbl -> GetItemTipInfo(This,pDO,lpszCurrentName,pStrings) ) 

#define IVsToolboxDataProvider2_GetProfileData(This,pDO,pbstrData)	\
    ( (This)->lpVtbl -> GetProfileData(This,pDO,pbstrData) ) 

#define IVsToolboxDataProvider2_GetItemID(This,pDO,pbstrID)	\
    ( (This)->lpVtbl -> GetItemID(This,pDO,pbstrID) ) 

#define IVsToolboxDataProvider2_ReconstituteItem(This,lpszCurrentName,lpszID,lpszData,ppDO,ptif)	\
    ( (This)->lpVtbl -> ReconstituteItem(This,lpszCurrentName,lpszID,lpszData,ppDO,ptif) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolboxDataProvider2_INTERFACE_DEFINED__ */


#ifndef __IVsResourceManager_INTERFACE_DEFINED__
#define __IVsResourceManager_INTERFACE_DEFINED__

/* interface IVsResourceManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsResourceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F0D7A6F0-C722-4AB6-A5D9-C7FF13027410")
    IVsResourceManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadResourceString( 
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE LoadResourceBitmap( 
            /* [in] */ REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [retval][out] */ HBITMAP *hbmpValue) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE LoadResourceIcon( 
            /* [in] */ REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [in] */ int cx,
            /* [in] */ int cy,
            /* [retval][out] */ HICON *hicoValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadResourceBlob( 
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [out] */ __RPC__deref_out_opt BYTE **pBytes,
            /* [out] */ __RPC__out long *lAllocated) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadResourceString2( 
            /* [string][in] */ __RPC__in LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE LoadResourceBitmap2( 
            /* [string][in] */ LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR szResourceName,
            /* [retval][out] */ HBITMAP *hbmpValue) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE LoadResourceIcon2( 
            /* [string][in] */ LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [in] */ int cx,
            /* [in] */ int cy,
            /* [retval][out] */ HICON *hicoValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadResourceBlob2( 
            /* [string][in] */ __RPC__in LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [out] */ __RPC__deref_out_opt BYTE **pBytes,
            /* [out] */ __RPC__out long *lAllocated) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSatelliteAssemblyPath( 
            /* [string][in] */ __RPC__in LPCOLESTR assemblyPath,
            /* [in] */ int lcid,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsResourceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsResourceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsResourceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsResourceManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadResourceString )( 
            IVsResourceManager * This,
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *LoadResourceBitmap )( 
            IVsResourceManager * This,
            /* [in] */ REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [retval][out] */ HBITMAP *hbmpValue);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *LoadResourceIcon )( 
            IVsResourceManager * This,
            /* [in] */ REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [in] */ int cx,
            /* [in] */ int cy,
            /* [retval][out] */ HICON *hicoValue);
        
        HRESULT ( STDMETHODCALLTYPE *LoadResourceBlob )( 
            IVsResourceManager * This,
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [out] */ __RPC__deref_out_opt BYTE **pBytes,
            /* [out] */ __RPC__out long *lAllocated);
        
        HRESULT ( STDMETHODCALLTYPE *LoadResourceString2 )( 
            IVsResourceManager * This,
            /* [string][in] */ __RPC__in LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *LoadResourceBitmap2 )( 
            IVsResourceManager * This,
            /* [string][in] */ LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR szResourceName,
            /* [retval][out] */ HBITMAP *hbmpValue);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *LoadResourceIcon2 )( 
            IVsResourceManager * This,
            /* [string][in] */ LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ LPCOLESTR pszResourceName,
            /* [in] */ int cx,
            /* [in] */ int cy,
            /* [retval][out] */ HICON *hicoValue);
        
        HRESULT ( STDMETHODCALLTYPE *LoadResourceBlob2 )( 
            IVsResourceManager * This,
            /* [string][in] */ __RPC__in LPCOLESTR pszAssemblyPath,
            /* [in] */ int culture,
            /* [string][in] */ __RPC__in LPCOLESTR pszResourceName,
            /* [out] */ __RPC__deref_out_opt BYTE **pBytes,
            /* [out] */ __RPC__out long *lAllocated);
        
        HRESULT ( STDMETHODCALLTYPE *GetSatelliteAssemblyPath )( 
            IVsResourceManager * This,
            /* [string][in] */ __RPC__in LPCOLESTR assemblyPath,
            /* [in] */ int lcid,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        END_INTERFACE
    } IVsResourceManagerVtbl;

    interface IVsResourceManager
    {
        CONST_VTBL struct IVsResourceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsResourceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsResourceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsResourceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsResourceManager_LoadResourceString(This,guidPackage,culture,pszResourceName,pbstrValue)	\
    ( (This)->lpVtbl -> LoadResourceString(This,guidPackage,culture,pszResourceName,pbstrValue) ) 

#define IVsResourceManager_LoadResourceBitmap(This,guidPackage,culture,pszResourceName,hbmpValue)	\
    ( (This)->lpVtbl -> LoadResourceBitmap(This,guidPackage,culture,pszResourceName,hbmpValue) ) 

#define IVsResourceManager_LoadResourceIcon(This,guidPackage,culture,pszResourceName,cx,cy,hicoValue)	\
    ( (This)->lpVtbl -> LoadResourceIcon(This,guidPackage,culture,pszResourceName,cx,cy,hicoValue) ) 

#define IVsResourceManager_LoadResourceBlob(This,guidPackage,culture,pszResourceName,pBytes,lAllocated)	\
    ( (This)->lpVtbl -> LoadResourceBlob(This,guidPackage,culture,pszResourceName,pBytes,lAllocated) ) 

#define IVsResourceManager_LoadResourceString2(This,pszAssemblyPath,culture,pszResourceName,pbstrValue)	\
    ( (This)->lpVtbl -> LoadResourceString2(This,pszAssemblyPath,culture,pszResourceName,pbstrValue) ) 

#define IVsResourceManager_LoadResourceBitmap2(This,pszAssemblyPath,culture,szResourceName,hbmpValue)	\
    ( (This)->lpVtbl -> LoadResourceBitmap2(This,pszAssemblyPath,culture,szResourceName,hbmpValue) ) 

#define IVsResourceManager_LoadResourceIcon2(This,pszAssemblyPath,culture,pszResourceName,cx,cy,hicoValue)	\
    ( (This)->lpVtbl -> LoadResourceIcon2(This,pszAssemblyPath,culture,pszResourceName,cx,cy,hicoValue) ) 

#define IVsResourceManager_LoadResourceBlob2(This,pszAssemblyPath,culture,pszResourceName,pBytes,lAllocated)	\
    ( (This)->lpVtbl -> LoadResourceBlob2(This,pszAssemblyPath,culture,pszResourceName,pBytes,lAllocated) ) 

#define IVsResourceManager_GetSatelliteAssemblyPath(This,assemblyPath,lcid,pbstrPath)	\
    ( (This)->lpVtbl -> GetSatelliteAssemblyPath(This,assemblyPath,lcid,pbstrPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsResourceManager_INTERFACE_DEFINED__ */


#ifndef __SVsResourceManager_INTERFACE_DEFINED__
#define __SVsResourceManager_INTERFACE_DEFINED__

/* interface SVsResourceManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsResourceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E9706EF6-4F76-4c6e-8BF0-6E448550470B")
    SVsResourceManager : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsResourceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsResourceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsResourceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsResourceManager * This);
        
        END_INTERFACE
    } SVsResourceManagerVtbl;

    interface SVsResourceManager
    {
        CONST_VTBL struct SVsResourceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsResourceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsResourceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsResourceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsResourceManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0146 */
/* [local] */ 

#define SID_SVsResourceManager IID_SVsResourceManager

enum __VSADDNEWWEBITEMOPTIONS
    {	VSADDWEBITEM_SelectMaster	= 0x1,
	VSADDWEBITEM_SeparateCodeFile	= 0x2,
	VSADDWEBITEM_SelectMasterIsValid	= 0x80000000,
	VSADDWEBITEM_SeparateCodeFileIsValid	= 0x40000000,
	VSADDWEBITEM_IsValidMask	= 0xf0000000
    } ;
typedef DWORD VSADDNEWWEBITEMOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0146_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0146_v0_0_s_ifspec;

#ifndef __IVsAddNewWebProjectItemDlg_INTERFACE_DEFINED__
#define __IVsAddNewWebProjectItemDlg_INTERFACE_DEFINED__

/* interface IVsAddNewWebProjectItemDlg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAddNewWebProjectItemDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("41F92AB8-98B0-4cf4-907B-C5CE4403A570")
    IVsAddNewWebProjectItemDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNewWebProjectItemDlg( 
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ __RPC__in REFGUID rguidProject,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [in] */ __RPC__in LPCOLESTR lpszLanguage,
            /* [in] */ __RPC__in LPCOLESTR lpszSelect,
            /* [in] */ VSADDNEWWEBITEMOPTIONS options) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAddNewWebProjectItemDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAddNewWebProjectItemDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAddNewWebProjectItemDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAddNewWebProjectItemDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewWebProjectItemDlg )( 
            IVsAddNewWebProjectItemDlg * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ __RPC__in REFGUID rguidProject,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [in] */ __RPC__in LPCOLESTR lpszLanguage,
            /* [in] */ __RPC__in LPCOLESTR lpszSelect,
            /* [in] */ VSADDNEWWEBITEMOPTIONS options);
        
        END_INTERFACE
    } IVsAddNewWebProjectItemDlgVtbl;

    interface IVsAddNewWebProjectItemDlg
    {
        CONST_VTBL struct IVsAddNewWebProjectItemDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddNewWebProjectItemDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddNewWebProjectItemDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddNewWebProjectItemDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddNewWebProjectItemDlg_AddNewWebProjectItemDlg(This,itemidLoc,rguidProject,pProject,pszDlgTitle,lpszHelpTopic,lpszLanguage,lpszSelect,options)	\
    ( (This)->lpVtbl -> AddNewWebProjectItemDlg(This,itemidLoc,rguidProject,pProject,pszDlgTitle,lpszHelpTopic,lpszLanguage,lpszSelect,options) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddNewWebProjectItemDlg_INTERFACE_DEFINED__ */


#ifndef __IVsWebProject_INTERFACE_DEFINED__
#define __IVsWebProject_INTERFACE_DEFINED__

/* interface IVsWebProject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0BD8000A-F537-4889-9FBC-C5F63B313956")
    IVsWebProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNewWebItem( 
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSADDITEMOPERATION dwAddItemOperation,
            /* [in] */ __RPC__in LPCOLESTR pszItemName,
            /* [in] */ __RPC__in LPCOLESTR pszFileTemplate,
            /* [in] */ VSADDNEWWEBITEMOPTIONS options,
            /* [in] */ __RPC__in LPCOLESTR pszSelectedLanguage,
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewWebItem )( 
            IVsWebProject * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSADDITEMOPERATION dwAddItemOperation,
            /* [in] */ __RPC__in LPCOLESTR pszItemName,
            /* [in] */ __RPC__in LPCOLESTR pszFileTemplate,
            /* [in] */ VSADDNEWWEBITEMOPTIONS options,
            /* [in] */ __RPC__in LPCOLESTR pszSelectedLanguage,
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult);
        
        END_INTERFACE
    } IVsWebProjectVtbl;

    interface IVsWebProject
    {
        CONST_VTBL struct IVsWebProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebProject_AddNewWebItem(This,itemidLoc,dwAddItemOperation,pszItemName,pszFileTemplate,options,pszSelectedLanguage,hwndDlgOwner,pResult)	\
    ( (This)->lpVtbl -> AddNewWebItem(This,itemidLoc,dwAddItemOperation,pszItemName,pszFileTemplate,options,pszSelectedLanguage,hwndDlgOwner,pResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebProject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0148 */
/* [local] */ 


enum __VSHIERITEMATTRIBUTE
    {	VSHIERITEMATTRIBUTE_Bold	= 1
    } ;
typedef DWORD VSHIERITEMATTRIBUTE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0148_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0148_v0_0_s_ifspec;

#ifndef __IVsUIHierarchyWindow2_INTERFACE_DEFINED__
#define __IVsUIHierarchyWindow2_INTERFACE_DEFINED__

/* interface IVsUIHierarchyWindow2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIHierarchyWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5B8C06A0-4379-4218-A046-B1DC466E5818")
    IVsUIHierarchyWindow2 : public IVsUIHierarchyWindow
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetItemAttribute( 
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH,
            /* [in] */ VSITEMID itemid,
            /* [in] */ VSHIERITEMATTRIBUTE attribute,
            /* [in] */ VARIANT value) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIHierarchyWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIHierarchyWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIHierarchyWindow2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Init )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH,
            /* [in] */ UIHWINFLAGS grfUIHWF,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkOut);
        
        HRESULT ( STDMETHODCALLTYPE *ExpandItem )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH,
            /* [in] */ VSITEMID itemid,
            /* [in] */ EXPANDFLAGS expf);
        
        HRESULT ( STDMETHODCALLTYPE *AddUIHierarchy )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH,
            /* [in] */ VSADDHIEROPTIONS grfAddOptions);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveUIHierarchy )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH);
        
        HRESULT ( STDMETHODCALLTYPE *SetWindowHelpTopic )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpFile,
            /* [in] */ DWORD dwContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemState )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ VSHIERARCHYITEMSTATE dwStateMask,
            /* [retval][out] */ __RPC__out VSHIERARCHYITEMSTATE *pdwState);
        
        HRESULT ( STDMETHODCALLTYPE *FindCommonSelectedHierarchy )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ VSCOMHIEROPTIONS grfOpt,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIHierarchy **lppCommonUIH);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *SetCursor )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ HCURSOR hNewCursor,
            /* [retval][out] */ HCURSOR *phOldCursor);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSelection )( 
            IVsUIHierarchyWindow2 * This,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppHier,
            /* [out] */ __RPC__out VSITEMID *pitemid,
            /* [out] */ __RPC__deref_out_opt IVsMultiItemSelect **ppMIS);
        
        HRESULT ( STDMETHODCALLTYPE *SetItemAttribute )( 
            IVsUIHierarchyWindow2 * This,
            /* [in] */ __RPC__in_opt IVsUIHierarchy *pUIH,
            /* [in] */ VSITEMID itemid,
            /* [in] */ VSHIERITEMATTRIBUTE attribute,
            /* [in] */ VARIANT value);
        
        END_INTERFACE
    } IVsUIHierarchyWindow2Vtbl;

    interface IVsUIHierarchyWindow2
    {
        CONST_VTBL struct IVsUIHierarchyWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIHierarchyWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIHierarchyWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIHierarchyWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIHierarchyWindow2_Init(This,pUIH,grfUIHWF,ppunkOut)	\
    ( (This)->lpVtbl -> Init(This,pUIH,grfUIHWF,ppunkOut) ) 

#define IVsUIHierarchyWindow2_ExpandItem(This,pUIH,itemid,expf)	\
    ( (This)->lpVtbl -> ExpandItem(This,pUIH,itemid,expf) ) 

#define IVsUIHierarchyWindow2_AddUIHierarchy(This,pUIH,grfAddOptions)	\
    ( (This)->lpVtbl -> AddUIHierarchy(This,pUIH,grfAddOptions) ) 

#define IVsUIHierarchyWindow2_RemoveUIHierarchy(This,pUIH)	\
    ( (This)->lpVtbl -> RemoveUIHierarchy(This,pUIH) ) 

#define IVsUIHierarchyWindow2_SetWindowHelpTopic(This,lpszHelpFile,dwContext)	\
    ( (This)->lpVtbl -> SetWindowHelpTopic(This,lpszHelpFile,dwContext) ) 

#define IVsUIHierarchyWindow2_GetItemState(This,pHier,itemid,dwStateMask,pdwState)	\
    ( (This)->lpVtbl -> GetItemState(This,pHier,itemid,dwStateMask,pdwState) ) 

#define IVsUIHierarchyWindow2_FindCommonSelectedHierarchy(This,grfOpt,lppCommonUIH)	\
    ( (This)->lpVtbl -> FindCommonSelectedHierarchy(This,grfOpt,lppCommonUIH) ) 

#define IVsUIHierarchyWindow2_SetCursor(This,hNewCursor,phOldCursor)	\
    ( (This)->lpVtbl -> SetCursor(This,hNewCursor,phOldCursor) ) 

#define IVsUIHierarchyWindow2_GetCurrentSelection(This,ppHier,pitemid,ppMIS)	\
    ( (This)->lpVtbl -> GetCurrentSelection(This,ppHier,pitemid,ppMIS) ) 


#define IVsUIHierarchyWindow2_SetItemAttribute(This,pUIH,itemid,attribute,value)	\
    ( (This)->lpVtbl -> SetItemAttribute(This,pUIH,itemid,attribute,value) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIHierarchyWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsProjectDataConnection_INTERFACE_DEFINED__
#define __IVsProjectDataConnection_INTERFACE_DEFINED__

/* interface IVsProjectDataConnection */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectDataConnection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8E40D748-F682-4951-B465-16D0C252A69D")
    IVsProjectDataConnection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProjectSqlConnection( 
            /* [out] */ __RPC__deref_out_opt IUnknown **pConnection) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProjectDataConnectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProjectDataConnection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProjectDataConnection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProjectDataConnection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectSqlConnection )( 
            IVsProjectDataConnection * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **pConnection);
        
        END_INTERFACE
    } IVsProjectDataConnectionVtbl;

    interface IVsProjectDataConnection
    {
        CONST_VTBL struct IVsProjectDataConnectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectDataConnection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectDataConnection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectDataConnection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectDataConnection_GetProjectSqlConnection(This,pConnection)	\
    ( (This)->lpVtbl -> GetProjectSqlConnection(This,pConnection) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectDataConnection_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0150 */
/* [local] */ 



typedef 
enum __tagVSTASKLISTSELECTIONSCROLLPOS
    {	TSSP_NOSCROLL	= 0,
	TSSP_CENTERCARET	= 1,
	TSSP_CARETATTOP	= 2,
	TSSP_CARETATBOTTOM	= 3,
	TSSP_MINSCROLL	= 4,
	TSSP_SHOWALL	= 5
    } 	__VSTASKLISTSELECTIONSCROLLPOS;

typedef DWORD VSTASKLISTSELECTIONSCROLLPOS;

typedef 
enum __tagVSTASKLISTSELECTIONTYPE
    {	TST_REPLACESEL	= 0,
	TST_EXTENDSEL	= 1,
	TST_ADDTOSEL	= 2
    } 	__VSTASKLISTSELECTIONTYPE;

typedef DWORD VSTASKLISTSELECTIONTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0150_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0150_v0_0_s_ifspec;

#ifndef __IVsTaskList2_INTERFACE_DEFINED__
#define __IVsTaskList2_INTERFACE_DEFINED__

/* interface IVsTaskList2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskList2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("62357211-57FD-4425-A9E5-9A6E0D3B731C")
    IVsTaskList2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSelectionCount( 
            /* [out] */ __RPC__out int *pnItems) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCaretPos( 
            /* [out] */ __RPC__deref_out_opt IVsTaskItem **ppItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumSelectedItems( 
            /* [out] */ __RPC__deref_out_opt IVsEnumTaskItems **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SelectItems( 
            /* [in] */ int nItems,
            /* [size_is][in] */ __RPC__in_ecount_full(nItems) IVsTaskItem *pItems[  ],
            /* [in] */ VSTASKLISTSELECTIONTYPE tsfSelType,
            /* [in] */ VSTASKLISTSELECTIONSCROLLPOS tsspScrollPos) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginTaskEdit( 
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem,
            /* [in] */ int iFocusField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetActiveProvider( 
            /* [out] */ __RPC__deref_out_opt IVsTaskProvider **ppProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetActiveProvider( 
            /* [in] */ __RPC__in REFGUID rguidProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshOrAddTasks( 
            /* [in] */ VSCOOKIE vsProviderCookie,
            /* [in] */ int nTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(nTasks) IVsTaskItem *prgTasks[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveTasks( 
            /* [in] */ VSCOOKIE vsProviderCookie,
            /* [in] */ int nTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(nTasks) IVsTaskItem *prgTasks[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshAllProviders( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTaskList2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTaskList2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTaskList2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTaskList2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSelectionCount )( 
            IVsTaskList2 * This,
            /* [out] */ __RPC__out int *pnItems);
        
        HRESULT ( STDMETHODCALLTYPE *GetCaretPos )( 
            IVsTaskList2 * This,
            /* [out] */ __RPC__deref_out_opt IVsTaskItem **ppItem);
        
        HRESULT ( STDMETHODCALLTYPE *EnumSelectedItems )( 
            IVsTaskList2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumTaskItems **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *SelectItems )( 
            IVsTaskList2 * This,
            /* [in] */ int nItems,
            /* [size_is][in] */ __RPC__in_ecount_full(nItems) IVsTaskItem *pItems[  ],
            /* [in] */ VSTASKLISTSELECTIONTYPE tsfSelType,
            /* [in] */ VSTASKLISTSELECTIONSCROLLPOS tsspScrollPos);
        
        HRESULT ( STDMETHODCALLTYPE *BeginTaskEdit )( 
            IVsTaskList2 * This,
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem,
            /* [in] */ int iFocusField);
        
        HRESULT ( STDMETHODCALLTYPE *GetActiveProvider )( 
            IVsTaskList2 * This,
            /* [out] */ __RPC__deref_out_opt IVsTaskProvider **ppProvider);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveProvider )( 
            IVsTaskList2 * This,
            /* [in] */ __RPC__in REFGUID rguidProvider);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshOrAddTasks )( 
            IVsTaskList2 * This,
            /* [in] */ VSCOOKIE vsProviderCookie,
            /* [in] */ int nTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(nTasks) IVsTaskItem *prgTasks[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveTasks )( 
            IVsTaskList2 * This,
            /* [in] */ VSCOOKIE vsProviderCookie,
            /* [in] */ int nTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(nTasks) IVsTaskItem *prgTasks[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshAllProviders )( 
            IVsTaskList2 * This);
        
        END_INTERFACE
    } IVsTaskList2Vtbl;

    interface IVsTaskList2
    {
        CONST_VTBL struct IVsTaskList2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskList2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskList2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskList2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskList2_GetSelectionCount(This,pnItems)	\
    ( (This)->lpVtbl -> GetSelectionCount(This,pnItems) ) 

#define IVsTaskList2_GetCaretPos(This,ppItem)	\
    ( (This)->lpVtbl -> GetCaretPos(This,ppItem) ) 

#define IVsTaskList2_EnumSelectedItems(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumSelectedItems(This,ppEnum) ) 

#define IVsTaskList2_SelectItems(This,nItems,pItems,tsfSelType,tsspScrollPos)	\
    ( (This)->lpVtbl -> SelectItems(This,nItems,pItems,tsfSelType,tsspScrollPos) ) 

#define IVsTaskList2_BeginTaskEdit(This,pItem,iFocusField)	\
    ( (This)->lpVtbl -> BeginTaskEdit(This,pItem,iFocusField) ) 

#define IVsTaskList2_GetActiveProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> GetActiveProvider(This,ppProvider) ) 

#define IVsTaskList2_SetActiveProvider(This,rguidProvider)	\
    ( (This)->lpVtbl -> SetActiveProvider(This,rguidProvider) ) 

#define IVsTaskList2_RefreshOrAddTasks(This,vsProviderCookie,nTasks,prgTasks)	\
    ( (This)->lpVtbl -> RefreshOrAddTasks(This,vsProviderCookie,nTasks,prgTasks) ) 

#define IVsTaskList2_RemoveTasks(This,vsProviderCookie,nTasks,prgTasks)	\
    ( (This)->lpVtbl -> RemoveTasks(This,vsProviderCookie,nTasks,prgTasks) ) 

#define IVsTaskList2_RefreshAllProviders(This)	\
    ( (This)->lpVtbl -> RefreshAllProviders(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskList2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0151 */
/* [local] */ 

typedef struct _VSTASKCOLUMN
    {
    int iField;
    BSTR bstrHeading;
    int iImage;
    BOOL fShowSortArrow;
    BOOL fAllowUserSort;
    BOOL fVisibleByDefault;
    BOOL fAllowHide;
    BOOL fSizeable;
    BOOL fMoveable;
    int iDefaultSortPriority;
    BOOL fDescendingSort;
    int cxMinWidth;
    int cxDefaultWidth;
    BOOL fDynamicSize;
    BSTR bstrCanonicalName;
    BSTR bstrLocalizedName;
    BSTR bstrTip;
    BOOL fFitContent;
    } 	VSTASKCOLUMN;


enum __VSTASKPROVIDERFLAGS
    {	TPF_ALWAYSVISIBLE	= 0x1,
	TPF_NOAUTOROUTING	= 0x2
    } ;
typedef DWORD VSTASKPROVIDERFLAGS;

DEFINE_GUID(GUID_Comment_TaskProvider, 0x5a2d2729, 0xadff, 0x4a2e, 0xa4, 0x4f, 0x55, 0xeb, 0xbf, 0x5d, 0xf6, 0x4b);


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0151_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0151_v0_0_s_ifspec;

#ifndef __IVsTaskProvider3_INTERFACE_DEFINED__
#define __IVsTaskProvider3_INTERFACE_DEFINED__

/* interface IVsTaskProvider3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskProvider3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AFA6B21D-D599-43f9-A3AB-0840359F11C3")
    IVsTaskProvider3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProviderFlags( 
            /* [out] */ __RPC__out VSTASKPROVIDERFLAGS *tpfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderGuid( 
            /* [out] */ __RPC__out GUID *pguidProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSurrogateProviderGuid( 
            /* [out] */ __RPC__out GUID *pguidProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderToolbar( 
            /* [out] */ __RPC__out GUID *pguidGroup,
            /* [out] */ __RPC__out DWORD *pdwID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColumnCount( 
            /* [out] */ __RPC__out int *pnColumns) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColumn( 
            /* [in] */ int iColumn,
            /* [out] */ __RPC__out VSTASKCOLUMN *pColumn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeginTaskEdit( 
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnEndTaskEdit( 
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem,
            /* [in] */ BOOL fCommitChanges,
            /* [out] */ __RPC__out BOOL *pfAllowChanges) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTaskProvider3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTaskProvider3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTaskProvider3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTaskProvider3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderFlags )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__out VSTASKPROVIDERFLAGS *tpfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderName )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderGuid )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__out GUID *pguidProvider);
        
        HRESULT ( STDMETHODCALLTYPE *GetSurrogateProviderGuid )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__out GUID *pguidProvider);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderToolbar )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__out GUID *pguidGroup,
            /* [out] */ __RPC__out DWORD *pdwID);
        
        HRESULT ( STDMETHODCALLTYPE *GetColumnCount )( 
            IVsTaskProvider3 * This,
            /* [out] */ __RPC__out int *pnColumns);
        
        HRESULT ( STDMETHODCALLTYPE *GetColumn )( 
            IVsTaskProvider3 * This,
            /* [in] */ int iColumn,
            /* [out] */ __RPC__out VSTASKCOLUMN *pColumn);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeginTaskEdit )( 
            IVsTaskProvider3 * This,
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem);
        
        HRESULT ( STDMETHODCALLTYPE *OnEndTaskEdit )( 
            IVsTaskProvider3 * This,
            /* [in] */ __RPC__in_opt IVsTaskItem *pItem,
            /* [in] */ BOOL fCommitChanges,
            /* [out] */ __RPC__out BOOL *pfAllowChanges);
        
        END_INTERFACE
    } IVsTaskProvider3Vtbl;

    interface IVsTaskProvider3
    {
        CONST_VTBL struct IVsTaskProvider3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskProvider3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskProvider3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskProvider3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskProvider3_GetProviderFlags(This,tpfFlags)	\
    ( (This)->lpVtbl -> GetProviderFlags(This,tpfFlags) ) 

#define IVsTaskProvider3_GetProviderName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetProviderName(This,pbstrName) ) 

#define IVsTaskProvider3_GetProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> GetProviderGuid(This,pguidProvider) ) 

#define IVsTaskProvider3_GetSurrogateProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> GetSurrogateProviderGuid(This,pguidProvider) ) 

#define IVsTaskProvider3_GetProviderToolbar(This,pguidGroup,pdwID)	\
    ( (This)->lpVtbl -> GetProviderToolbar(This,pguidGroup,pdwID) ) 

#define IVsTaskProvider3_GetColumnCount(This,pnColumns)	\
    ( (This)->lpVtbl -> GetColumnCount(This,pnColumns) ) 

#define IVsTaskProvider3_GetColumn(This,iColumn,pColumn)	\
    ( (This)->lpVtbl -> GetColumn(This,iColumn,pColumn) ) 

#define IVsTaskProvider3_OnBeginTaskEdit(This,pItem)	\
    ( (This)->lpVtbl -> OnBeginTaskEdit(This,pItem) ) 

#define IVsTaskProvider3_OnEndTaskEdit(This,pItem,fCommitChanges,pfAllowChanges)	\
    ( (This)->lpVtbl -> OnEndTaskEdit(This,pItem,fCommitChanges,pfAllowChanges) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskProvider3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0152 */
/* [local] */ 

typedef 
enum __tagVSTASKVALUETYPE
    {	TVT_NULL	= 0,
	TVT_TEXT	= 1,
	TVT_LINKTEXT	= 2,
	TVT_BASE10	= 3,
	TVT_IMAGE	= 4
    } 	__VSTASKVALUETYPE;

typedef DWORD VSTASKVALUETYPE;


enum __VSTASKVALUEFLAGS
    {	TVF_EDITABLE	= 0x1,
	TVF_ENUM	= 0x2,
	TVF_BINARY_STATE	= 0x4,
	TVF_HORZ_RIGHT	= 0x8,
	TVF_HORZ_CENTER	= 0x10,
	TVF_STRIKETHROUGH	= 0x20,
	TVF_FILENAME	= 0x40
    } ;
typedef DWORD VSTASKVALUEFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0152_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0152_v0_0_s_ifspec;

#ifndef __IVsTaskItem3_INTERFACE_DEFINED__
#define __IVsTaskItem3_INTERFACE_DEFINED__

/* interface IVsTaskItem3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskItem3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F353950E-C381-4ba6-BCAA-6BA64E53D252")
    IVsTaskItem3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTaskProvider( 
            /* [out] */ __RPC__deref_out_opt IVsTaskProvider3 **ppProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTaskName( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColumnValue( 
            /* [in] */ int iField,
            /* [out] */ __RPC__out VSTASKVALUETYPE *ptvtType,
            /* [out] */ __RPC__out VSTASKVALUEFLAGS *ptvfFlags,
            /* [out] */ __RPC__out VARIANT *pvarValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAccessibilityName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTipText( 
            /* [in] */ int iField,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTipText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetColumnValue( 
            /* [in] */ int iField,
            /* [in] */ __RPC__in VARIANT *pvarValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsDirty( 
            /* [out] */ __RPC__out BOOL *pfDirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumCount( 
            /* [in] */ int iField,
            /* [out] */ __RPC__out int *pnValues) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumValue( 
            /* [in] */ int iField,
            /* [in] */ int iValue,
            /* [out] */ __RPC__out VARIANT *pvarValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAccessibilityName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnLinkClicked( 
            /* [in] */ int iField,
            /* [in] */ int iLinkIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNavigationStatusText( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultEditField( 
            /* [out] */ __RPC__out int *piField) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSurrogateProviderGuid( 
            /* [out] */ __RPC__out GUID *pguidProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTaskItem3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTaskItem3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTaskItem3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTaskItem3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTaskProvider )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__deref_out_opt IVsTaskProvider3 **ppProvider);
        
        HRESULT ( STDMETHODCALLTYPE *GetTaskName )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetColumnValue )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [out] */ __RPC__out VSTASKVALUETYPE *ptvtType,
            /* [out] */ __RPC__out VSTASKVALUEFLAGS *ptvfFlags,
            /* [out] */ __RPC__out VARIANT *pvarValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAccessibilityName);
        
        HRESULT ( STDMETHODCALLTYPE *GetTipText )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTipText);
        
        HRESULT ( STDMETHODCALLTYPE *SetColumnValue )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [in] */ __RPC__in VARIANT *pvarValue);
        
        HRESULT ( STDMETHODCALLTYPE *IsDirty )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__out BOOL *pfDirty);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumCount )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [out] */ __RPC__out int *pnValues);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumValue )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [in] */ int iValue,
            /* [out] */ __RPC__out VARIANT *pvarValue,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAccessibilityName);
        
        HRESULT ( STDMETHODCALLTYPE *OnLinkClicked )( 
            IVsTaskItem3 * This,
            /* [in] */ int iField,
            /* [in] */ int iLinkIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetNavigationStatusText )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultEditField )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__out int *piField);
        
        HRESULT ( STDMETHODCALLTYPE *GetSurrogateProviderGuid )( 
            IVsTaskItem3 * This,
            /* [out] */ __RPC__out GUID *pguidProvider);
        
        END_INTERFACE
    } IVsTaskItem3Vtbl;

    interface IVsTaskItem3
    {
        CONST_VTBL struct IVsTaskItem3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskItem3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskItem3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskItem3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskItem3_GetTaskProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> GetTaskProvider(This,ppProvider) ) 

#define IVsTaskItem3_GetTaskName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetTaskName(This,pbstrName) ) 

#define IVsTaskItem3_GetColumnValue(This,iField,ptvtType,ptvfFlags,pvarValue,pbstrAccessibilityName)	\
    ( (This)->lpVtbl -> GetColumnValue(This,iField,ptvtType,ptvfFlags,pvarValue,pbstrAccessibilityName) ) 

#define IVsTaskItem3_GetTipText(This,iField,pbstrTipText)	\
    ( (This)->lpVtbl -> GetTipText(This,iField,pbstrTipText) ) 

#define IVsTaskItem3_SetColumnValue(This,iField,pvarValue)	\
    ( (This)->lpVtbl -> SetColumnValue(This,iField,pvarValue) ) 

#define IVsTaskItem3_IsDirty(This,pfDirty)	\
    ( (This)->lpVtbl -> IsDirty(This,pfDirty) ) 

#define IVsTaskItem3_GetEnumCount(This,iField,pnValues)	\
    ( (This)->lpVtbl -> GetEnumCount(This,iField,pnValues) ) 

#define IVsTaskItem3_GetEnumValue(This,iField,iValue,pvarValue,pbstrAccessibilityName)	\
    ( (This)->lpVtbl -> GetEnumValue(This,iField,iValue,pvarValue,pbstrAccessibilityName) ) 

#define IVsTaskItem3_OnLinkClicked(This,iField,iLinkIndex)	\
    ( (This)->lpVtbl -> OnLinkClicked(This,iField,iLinkIndex) ) 

#define IVsTaskItem3_GetNavigationStatusText(This,pbstrText)	\
    ( (This)->lpVtbl -> GetNavigationStatusText(This,pbstrText) ) 

#define IVsTaskItem3_GetDefaultEditField(This,piField)	\
    ( (This)->lpVtbl -> GetDefaultEditField(This,piField) ) 

#define IVsTaskItem3_GetSurrogateProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> GetSurrogateProviderGuid(This,pguidProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskItem3_INTERFACE_DEFINED__ */


#ifndef __IVsErrorList_INTERFACE_DEFINED__
#define __IVsErrorList_INTERFACE_DEFINED__

/* interface IVsErrorList */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsErrorList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D824A797-62D2-4f60-98F8-4423624BC1BF")
    IVsErrorList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BringToFront( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ForceShowErrors( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsErrorListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsErrorList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsErrorList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsErrorList * This);
        
        HRESULT ( STDMETHODCALLTYPE *BringToFront )( 
            IVsErrorList * This);
        
        HRESULT ( STDMETHODCALLTYPE *ForceShowErrors )( 
            IVsErrorList * This);
        
        END_INTERFACE
    } IVsErrorListVtbl;

    interface IVsErrorList
    {
        CONST_VTBL struct IVsErrorListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsErrorList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsErrorList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsErrorList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsErrorList_BringToFront(This)	\
    ( (This)->lpVtbl -> BringToFront(This) ) 

#define IVsErrorList_ForceShowErrors(This)	\
    ( (This)->lpVtbl -> ForceShowErrors(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsErrorList_INTERFACE_DEFINED__ */


#ifndef __SVsErrorList_INTERFACE_DEFINED__
#define __SVsErrorList_INTERFACE_DEFINED__

/* interface SVsErrorList */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsErrorList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("599BCD9E-CAFA-43DF-AA03-129C0004C2BD")
    SVsErrorList : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsErrorListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsErrorList * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsErrorList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsErrorList * This);
        
        END_INTERFACE
    } SVsErrorListVtbl;

    interface SVsErrorList
    {
        CONST_VTBL struct SVsErrorListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsErrorList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsErrorList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsErrorList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsErrorList_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0155 */
/* [local] */ 

#define SID_SVsErrorList IID_SVsErrorList
extern const __declspec(selectany) GUID GUID_ErrorList = { 0xd78612c7, 0x9962, 0x4b83, { 0x95, 0xd9, 0x26, 0x80, 0x46, 0xda, 0xd2, 0x3a } };
extern const __declspec(selectany) GUID GUID_PropertySheetManager = { 0x6B8E94B5, 0x0949, 0x4d9c, { 0xA8, 0x1F, 0xC1, 0xB9, 0xB7, 0x44, 0x18, 0x5C} };
extern const __declspec(selectany) GUID GUID_CodeDefWin =  { 0x588470cc, 0x84f8, 0x4a57, { 0x9a, 0xc4, 0x86, 0xbc, 0xa0, 0x62, 0x5f, 0xf4 } };
typedef 
enum __tagVSERRORCATEGORY
    {	EC_ERROR	= 0,
	EC_WARNING	= 1,
	EC_MESSAGE	= 2
    } 	__VSERRORCATEGORY;

typedef DWORD VSERRORCATEGORY;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0155_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0155_v0_0_s_ifspec;

#ifndef __IVsErrorItem_INTERFACE_DEFINED__
#define __IVsErrorItem_INTERFACE_DEFINED__

/* interface IVsErrorItem */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsErrorItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CE3DF110-7630-4d6c-81D5-5CFA12ADDAE6")
    IVsErrorItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHierarchy( 
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCategory( 
            /* [out] */ __RPC__out VSERRORCATEGORY *pCategory) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsErrorItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsErrorItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsErrorItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsErrorItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHierarchy )( 
            IVsErrorItem * This,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppProject);
        
        HRESULT ( STDMETHODCALLTYPE *GetCategory )( 
            IVsErrorItem * This,
            /* [out] */ __RPC__out VSERRORCATEGORY *pCategory);
        
        END_INTERFACE
    } IVsErrorItemVtbl;

    interface IVsErrorItem
    {
        CONST_VTBL struct IVsErrorItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsErrorItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsErrorItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsErrorItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsErrorItem_GetHierarchy(This,ppProject)	\
    ( (This)->lpVtbl -> GetHierarchy(This,ppProject) ) 

#define IVsErrorItem_GetCategory(This,pCategory)	\
    ( (This)->lpVtbl -> GetCategory(This,pCategory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsErrorItem_INTERFACE_DEFINED__ */


#ifndef __IVsWindowPaneCommitFilter_INTERFACE_DEFINED__
#define __IVsWindowPaneCommitFilter_INTERFACE_DEFINED__

/* interface IVsWindowPaneCommitFilter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWindowPaneCommitFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("072B9701-2F81-4468-8EB6-074206504B62")
    IVsWindowPaneCommitFilter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsCommitCommand( 
            /* [in] */ __RPC__in REFGUID pguidCmdGroup,
            /* [in] */ DWORD dwCmdID,
            /* [out] */ __RPC__out BOOL *pfCommitCommand) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWindowPaneCommitFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWindowPaneCommitFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWindowPaneCommitFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWindowPaneCommitFilter * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsCommitCommand )( 
            IVsWindowPaneCommitFilter * This,
            /* [in] */ __RPC__in REFGUID pguidCmdGroup,
            /* [in] */ DWORD dwCmdID,
            /* [out] */ __RPC__out BOOL *pfCommitCommand);
        
        END_INTERFACE
    } IVsWindowPaneCommitFilterVtbl;

    interface IVsWindowPaneCommitFilter
    {
        CONST_VTBL struct IVsWindowPaneCommitFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowPaneCommitFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowPaneCommitFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowPaneCommitFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowPaneCommitFilter_IsCommitCommand(This,pguidCmdGroup,dwCmdID,pfCommitCommand)	\
    ( (This)->lpVtbl -> IsCommitCommand(This,pguidCmdGroup,dwCmdID,pfCommitCommand) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowPaneCommitFilter_INTERFACE_DEFINED__ */


#ifndef __IPreferPropertyPagesWithTreeControl_INTERFACE_DEFINED__
#define __IPreferPropertyPagesWithTreeControl_INTERFACE_DEFINED__

/* interface IPreferPropertyPagesWithTreeControl */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IPreferPropertyPagesWithTreeControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("03AEFCEA-62A9-4606-8E17-7CB0FC13D5E0")
    IPreferPropertyPagesWithTreeControl : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IPreferPropertyPagesWithTreeControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPreferPropertyPagesWithTreeControl * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPreferPropertyPagesWithTreeControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPreferPropertyPagesWithTreeControl * This);
        
        END_INTERFACE
    } IPreferPropertyPagesWithTreeControlVtbl;

    interface IPreferPropertyPagesWithTreeControl
    {
        CONST_VTBL struct IPreferPropertyPagesWithTreeControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPreferPropertyPagesWithTreeControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPreferPropertyPagesWithTreeControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPreferPropertyPagesWithTreeControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPreferPropertyPagesWithTreeControl_INTERFACE_DEFINED__ */


#ifndef __IVsSpecifyProjectDesignerPages_INTERFACE_DEFINED__
#define __IVsSpecifyProjectDesignerPages_INTERFACE_DEFINED__

/* interface IVsSpecifyProjectDesignerPages */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSpecifyProjectDesignerPages;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E7E36A24-6435-48fb-8E5B-D2589FC18D72")
    IVsSpecifyProjectDesignerPages : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProjectDesignerPages( 
            /* [out] */ __RPC__out CAUUID *pPages) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSpecifyProjectDesignerPagesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSpecifyProjectDesignerPages * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSpecifyProjectDesignerPages * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSpecifyProjectDesignerPages * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectDesignerPages )( 
            IVsSpecifyProjectDesignerPages * This,
            /* [out] */ __RPC__out CAUUID *pPages);
        
        END_INTERFACE
    } IVsSpecifyProjectDesignerPagesVtbl;

    interface IVsSpecifyProjectDesignerPages
    {
        CONST_VTBL struct IVsSpecifyProjectDesignerPagesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSpecifyProjectDesignerPages_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSpecifyProjectDesignerPages_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSpecifyProjectDesignerPages_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSpecifyProjectDesignerPages_GetProjectDesignerPages(This,pPages)	\
    ( (This)->lpVtbl -> GetProjectDesignerPages(This,pPages) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSpecifyProjectDesignerPages_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0159 */
/* [local] */ 

#define VS_OUTPUTGROUP_CNAME_SGenFiles         L"XmlSerializer"


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0159_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0159_v0_0_s_ifspec;

#ifndef __IVsDeployDependency2_INTERFACE_DEFINED__
#define __IVsDeployDependency2_INTERFACE_DEFINED__

/* interface IVsDeployDependency2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDeployDependency2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("77F54647-95E6-4033-91A8-51CCD014D945")
    IVsDeployDependency2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Property( 
            /* [in] */ __RPC__in LPCOLESTR szProperty,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDeployDependency2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDeployDependency2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDeployDependency2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDeployDependency2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Property )( 
            IVsDeployDependency2 * This,
            /* [in] */ __RPC__in LPCOLESTR szProperty,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        END_INTERFACE
    } IVsDeployDependency2Vtbl;

    interface IVsDeployDependency2
    {
        CONST_VTBL struct IVsDeployDependency2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDeployDependency2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDeployDependency2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDeployDependency2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDeployDependency2_get_Property(This,szProperty,pvar)	\
    ( (This)->lpVtbl -> get_Property(This,szProperty,pvar) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDeployDependency2_INTERFACE_DEFINED__ */


#ifndef __IVsOutputGroup2_INTERFACE_DEFINED__
#define __IVsOutputGroup2_INTERFACE_DEFINED__

/* interface IVsOutputGroup2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsOutputGroup2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("06A3B841-FBEA-46cb-81EA-36DB4D005545")
    IVsOutputGroup2 : public IVsOutputGroup
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_KeyOutputObject( 
            /* [out] */ __RPC__deref_out_opt IVsOutput2 **ppKeyOutput) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Property( 
            /* [in] */ __RPC__in LPCOLESTR pszProperty,
            /* [out] */ __RPC__out VARIANT *pvar) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOutputGroup2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOutputGroup2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOutputGroup2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOutputGroup2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_CanonicalName )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCanonicalName);
        
        HRESULT ( STDMETHODCALLTYPE *get_DisplayName )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *get_KeyOutput )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCanonicalName);
        
        HRESULT ( STDMETHODCALLTYPE *get_ProjectCfg )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt IVsProjectCfg2 **ppIVsProjectCfg2);
        
        HRESULT ( STDMETHODCALLTYPE *get_Outputs )( 
            IVsOutputGroup2 * This,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsOutput2 *rgpcfg[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *get_DeployDependencies )( 
            IVsOutputGroup2 * This,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) IVsDeployDependency *rgpdpd[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *get_KeyOutputObject )( 
            IVsOutputGroup2 * This,
            /* [out] */ __RPC__deref_out_opt IVsOutput2 **ppKeyOutput);
        
        HRESULT ( STDMETHODCALLTYPE *get_Property )( 
            IVsOutputGroup2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszProperty,
            /* [out] */ __RPC__out VARIANT *pvar);
        
        END_INTERFACE
    } IVsOutputGroup2Vtbl;

    interface IVsOutputGroup2
    {
        CONST_VTBL struct IVsOutputGroup2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutputGroup2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutputGroup2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutputGroup2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutputGroup2_get_CanonicalName(This,pbstrCanonicalName)	\
    ( (This)->lpVtbl -> get_CanonicalName(This,pbstrCanonicalName) ) 

#define IVsOutputGroup2_get_DisplayName(This,pbstrDisplayName)	\
    ( (This)->lpVtbl -> get_DisplayName(This,pbstrDisplayName) ) 

#define IVsOutputGroup2_get_KeyOutput(This,pbstrCanonicalName)	\
    ( (This)->lpVtbl -> get_KeyOutput(This,pbstrCanonicalName) ) 

#define IVsOutputGroup2_get_ProjectCfg(This,ppIVsProjectCfg2)	\
    ( (This)->lpVtbl -> get_ProjectCfg(This,ppIVsProjectCfg2) ) 

#define IVsOutputGroup2_get_Outputs(This,celt,rgpcfg,pcActual)	\
    ( (This)->lpVtbl -> get_Outputs(This,celt,rgpcfg,pcActual) ) 

#define IVsOutputGroup2_get_DeployDependencies(This,celt,rgpdpd,pcActual)	\
    ( (This)->lpVtbl -> get_DeployDependencies(This,celt,rgpdpd,pcActual) ) 

#define IVsOutputGroup2_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 


#define IVsOutputGroup2_get_KeyOutputObject(This,ppKeyOutput)	\
    ( (This)->lpVtbl -> get_KeyOutputObject(This,ppKeyOutput) ) 

#define IVsOutputGroup2_get_Property(This,pszProperty,pvar)	\
    ( (This)->lpVtbl -> get_Property(This,pszProperty,pvar) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutputGroup2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0161 */
/* [local] */ 


enum __VSCOLORTYPE
    {	CT_INVALID	= 0,
	CT_RAW	= 1,
	CT_COLORINDEX	= 2,
	CT_SYSCOLOR	= 3,
	CT_VSCOLOR	= 4,
	CT_AUTOMATIC	= 5,
	CT_TRACK_FOREGROUND	= 6,
	CT_TRACK_BACKGROUND	= 7
    } ;
typedef LONG VSCOLORTYPE;


enum __VSCOLORASPECT
    {	CA_FOREGROUND	= 0,
	CA_BACKGROUND	= 1
    } ;
typedef LONG VSCOLORASPECT;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0161_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0161_v0_0_s_ifspec;

#ifndef __IVsFontAndColorUtilities_INTERFACE_DEFINED__
#define __IVsFontAndColorUtilities_INTERFACE_DEFINED__

/* interface IVsFontAndColorUtilities */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFontAndColorUtilities;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A356A017-07EE-4d06-ACDE-FEFDBB49EB50")
    IVsFontAndColorUtilities : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EncodeIndexedColor( 
            /* [in] */ COLORINDEX idx,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodeSysColor( 
            /* [in] */ int iSysColor,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodeVSColor( 
            /* [in] */ VSSYSCOLOREX vsColor,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodeTrackedItem( 
            /* [in] */ int iItemToTrack,
            /* [in] */ VSCOLORASPECT aspect,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodeInvalidColor( 
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EncodeAutomaticColor( 
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetColorType( 
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSCOLORTYPE *pctType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEncodedIndex( 
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out COLORINDEX *pIdx) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEncodedSysColor( 
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out int *piSysColor) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEncodedVSColor( 
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSSYSCOLOREX *pVSColor) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTrackedItemIndex( 
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSCOLORASPECT *pAspect,
            /* [out] */ __RPC__out int *piItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRGBOfIndex( 
            /* [in] */ COLORINDEX idx,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRGBOfItem( 
            /* [in] */ __RPC__in AllColorableItemInfo *pInfo,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out COLORREF *pcrForeground,
            /* [out] */ __RPC__out COLORREF *pcrBackground) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRGBOfEncodedColor( 
            /* [in] */ COLORREF crSource,
            /* [in] */ COLORREF crAutoColor,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out COLORREF *pcrResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitFontInfo( 
            /* [out][in] */ __RPC__inout FontInfo *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FreeFontInfo( 
            /* [out][in] */ __RPC__inout FontInfo *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CopyFontInfo( 
            /* [out][in] */ __RPC__inout FontInfo *pDest,
            /* [in] */ __RPC__in const FontInfo *pSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitItemInfo( 
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FreeItemInfo( 
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CopyItemInfo( 
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pDest,
            /* [in] */ __RPC__in const AllColorableItemInfo *pSource) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFontAndColorUtilitiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFontAndColorUtilities * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFontAndColorUtilities * This);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeIndexedColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORINDEX idx,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeSysColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ int iSysColor,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeVSColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ VSSYSCOLOREX vsColor,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeTrackedItem )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ int iItemToTrack,
            /* [in] */ VSCOLORASPECT aspect,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeInvalidColor )( 
            IVsFontAndColorUtilities * This,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *EncodeAutomaticColor )( 
            IVsFontAndColorUtilities * This,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorType )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSCOLORTYPE *pctType);
        
        HRESULT ( STDMETHODCALLTYPE *GetEncodedIndex )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out COLORINDEX *pIdx);
        
        HRESULT ( STDMETHODCALLTYPE *GetEncodedSysColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out int *piSysColor);
        
        HRESULT ( STDMETHODCALLTYPE *GetEncodedVSColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSSYSCOLOREX *pVSColor);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrackedItemIndex )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [out] */ __RPC__out VSCOLORASPECT *pAspect,
            /* [out] */ __RPC__out int *piItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetRGBOfIndex )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORINDEX idx,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetRGBOfItem )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ __RPC__in AllColorableItemInfo *pInfo,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out COLORREF *pcrForeground,
            /* [out] */ __RPC__out COLORREF *pcrBackground);
        
        HRESULT ( STDMETHODCALLTYPE *GetRGBOfEncodedColor )( 
            IVsFontAndColorUtilities * This,
            /* [in] */ COLORREF crSource,
            /* [in] */ COLORREF crAutoColor,
            /* [in] */ __RPC__in REFGUID rguidCategory,
            /* [out] */ __RPC__out COLORREF *pcrResult);
        
        HRESULT ( STDMETHODCALLTYPE *InitFontInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout FontInfo *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *FreeFontInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout FontInfo *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CopyFontInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout FontInfo *pDest,
            /* [in] */ __RPC__in const FontInfo *pSource);
        
        HRESULT ( STDMETHODCALLTYPE *InitItemInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *FreeItemInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pInfo);
        
        HRESULT ( STDMETHODCALLTYPE *CopyItemInfo )( 
            IVsFontAndColorUtilities * This,
            /* [out][in] */ __RPC__inout AllColorableItemInfo *pDest,
            /* [in] */ __RPC__in const AllColorableItemInfo *pSource);
        
        END_INTERFACE
    } IVsFontAndColorUtilitiesVtbl;

    interface IVsFontAndColorUtilities
    {
        CONST_VTBL struct IVsFontAndColorUtilitiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFontAndColorUtilities_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFontAndColorUtilities_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFontAndColorUtilities_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFontAndColorUtilities_EncodeIndexedColor(This,idx,pcrResult)	\
    ( (This)->lpVtbl -> EncodeIndexedColor(This,idx,pcrResult) ) 

#define IVsFontAndColorUtilities_EncodeSysColor(This,iSysColor,pcrResult)	\
    ( (This)->lpVtbl -> EncodeSysColor(This,iSysColor,pcrResult) ) 

#define IVsFontAndColorUtilities_EncodeVSColor(This,vsColor,pcrResult)	\
    ( (This)->lpVtbl -> EncodeVSColor(This,vsColor,pcrResult) ) 

#define IVsFontAndColorUtilities_EncodeTrackedItem(This,iItemToTrack,aspect,pcrResult)	\
    ( (This)->lpVtbl -> EncodeTrackedItem(This,iItemToTrack,aspect,pcrResult) ) 

#define IVsFontAndColorUtilities_EncodeInvalidColor(This,pcrResult)	\
    ( (This)->lpVtbl -> EncodeInvalidColor(This,pcrResult) ) 

#define IVsFontAndColorUtilities_EncodeAutomaticColor(This,pcrResult)	\
    ( (This)->lpVtbl -> EncodeAutomaticColor(This,pcrResult) ) 

#define IVsFontAndColorUtilities_GetColorType(This,crSource,pctType)	\
    ( (This)->lpVtbl -> GetColorType(This,crSource,pctType) ) 

#define IVsFontAndColorUtilities_GetEncodedIndex(This,crSource,pIdx)	\
    ( (This)->lpVtbl -> GetEncodedIndex(This,crSource,pIdx) ) 

#define IVsFontAndColorUtilities_GetEncodedSysColor(This,crSource,piSysColor)	\
    ( (This)->lpVtbl -> GetEncodedSysColor(This,crSource,piSysColor) ) 

#define IVsFontAndColorUtilities_GetEncodedVSColor(This,crSource,pVSColor)	\
    ( (This)->lpVtbl -> GetEncodedVSColor(This,crSource,pVSColor) ) 

#define IVsFontAndColorUtilities_GetTrackedItemIndex(This,crSource,pAspect,piItem)	\
    ( (This)->lpVtbl -> GetTrackedItemIndex(This,crSource,pAspect,piItem) ) 

#define IVsFontAndColorUtilities_GetRGBOfIndex(This,idx,pcrResult)	\
    ( (This)->lpVtbl -> GetRGBOfIndex(This,idx,pcrResult) ) 

#define IVsFontAndColorUtilities_GetRGBOfItem(This,pInfo,rguidCategory,pcrForeground,pcrBackground)	\
    ( (This)->lpVtbl -> GetRGBOfItem(This,pInfo,rguidCategory,pcrForeground,pcrBackground) ) 

#define IVsFontAndColorUtilities_GetRGBOfEncodedColor(This,crSource,crAutoColor,rguidCategory,pcrResult)	\
    ( (This)->lpVtbl -> GetRGBOfEncodedColor(This,crSource,crAutoColor,rguidCategory,pcrResult) ) 

#define IVsFontAndColorUtilities_InitFontInfo(This,pInfo)	\
    ( (This)->lpVtbl -> InitFontInfo(This,pInfo) ) 

#define IVsFontAndColorUtilities_FreeFontInfo(This,pInfo)	\
    ( (This)->lpVtbl -> FreeFontInfo(This,pInfo) ) 

#define IVsFontAndColorUtilities_CopyFontInfo(This,pDest,pSource)	\
    ( (This)->lpVtbl -> CopyFontInfo(This,pDest,pSource) ) 

#define IVsFontAndColorUtilities_InitItemInfo(This,pInfo)	\
    ( (This)->lpVtbl -> InitItemInfo(This,pInfo) ) 

#define IVsFontAndColorUtilities_FreeItemInfo(This,pInfo)	\
    ( (This)->lpVtbl -> FreeItemInfo(This,pInfo) ) 

#define IVsFontAndColorUtilities_CopyItemInfo(This,pDest,pSource)	\
    ( (This)->lpVtbl -> CopyItemInfo(This,pDest,pSource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFontAndColorUtilities_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0162 */
/* [local] */ 

extern const __declspec(selectany) GUID GUID_DialogsAndToolWindowsFC = { 0x1f987c00, 0xe7c4, 0x4869, { 0x8a, 0x17, 0x23, 0xfd, 0x60, 0x22, 0x68, 0xb0 } };
extern const __declspec(selectany) GUID GUID_TextEditorFC = { 0xa27b4e24, 0xa735, 0x4d1d, { 0xb8, 0xe7,  0x97,  0x16,  0xe1,  0xe3,  0xd8,  0xe0 } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0162_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0162_v0_0_s_ifspec;

#ifndef __IVsOutputWindow2_INTERFACE_DEFINED__
#define __IVsOutputWindow2_INTERFACE_DEFINED__

/* interface IVsOutputWindow2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsOutputWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F4DEB52C-53A3-42fd-A039-3493F09E53FC")
    IVsOutputWindow2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetActivePaneGUID( 
            /* [out] */ __RPC__out GUID *pguidPane) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsOutputWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsOutputWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsOutputWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsOutputWindow2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetActivePaneGUID )( 
            IVsOutputWindow2 * This,
            /* [out] */ __RPC__out GUID *pguidPane);
        
        END_INTERFACE
    } IVsOutputWindow2Vtbl;

    interface IVsOutputWindow2
    {
        CONST_VTBL struct IVsOutputWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutputWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutputWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutputWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutputWindow2_GetActivePaneGUID(This,pguidPane)	\
    ( (This)->lpVtbl -> GetActivePaneGUID(This,pguidPane) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutputWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsDebuggableProjectCfg2_INTERFACE_DEFINED__
#define __IVsDebuggableProjectCfg2_INTERFACE_DEFINED__

/* interface IVsDebuggableProjectCfg2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebuggableProjectCfg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2559D053-417E-4276-A905-193191B5816A")
    IVsDebuggableProjectCfg2 : public IVsDebuggableProjectCfg
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeDebugLaunch( 
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDebuggableProjectCfg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDebuggableProjectCfg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDebuggableProjectCfg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDebuggableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_DisplayName )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *get_IsDebugOnly )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out BOOL *pfIsDebugOnly);
        
        HRESULT ( STDMETHODCALLTYPE *get_IsReleaseOnly )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out BOOL *pfIsReleaseOnly);
        
        HRESULT ( STDMETHODCALLTYPE *EnumOutputs )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumOutputs **ppIVsEnumOutputs);
        
        HRESULT ( STDMETHODCALLTYPE *OpenOutput )( 
            IVsDebuggableProjectCfg2 * This,
            /* [in] */ __RPC__in LPCOLESTR szOutputCanonicalName,
            /* [out] */ __RPC__deref_out_opt IVsOutput **ppIVsOutput);
        
        HRESULT ( STDMETHODCALLTYPE *get_ProjectCfgProvider )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt IVsProjectCfgProvider **ppIVsProjectCfgProvider);
        
        HRESULT ( STDMETHODCALLTYPE *get_BuildableProjectCfg )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt IVsBuildableProjectCfg **ppIVsBuildableProjectCfg);
        
        HRESULT ( STDMETHODCALLTYPE *get_CanonicalName )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCanonicalName);
        
        HRESULT ( STDMETHODCALLTYPE *get_Platform )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out GUID *pguidPlatform);
        
        HRESULT ( STDMETHODCALLTYPE *get_IsPackaged )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out BOOL *pfIsPackaged);
        
        HRESULT ( STDMETHODCALLTYPE *get_IsSpecifyingOutputSupported )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out BOOL *pfIsSpecifyingOutputSupported);
        
        HRESULT ( STDMETHODCALLTYPE *get_TargetCodePage )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out UINT *puiTargetCodePage);
        
        HRESULT ( STDMETHODCALLTYPE *get_UpdateSequenceNumber )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__out ULARGE_INTEGER *puliUSN);
        
        HRESULT ( STDMETHODCALLTYPE *get_RootURL )( 
            IVsDebuggableProjectCfg2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRootURL);
        
        HRESULT ( STDMETHODCALLTYPE *DebugLaunch )( 
            IVsDebuggableProjectCfg2 * This,
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDebugLaunch )( 
            IVsDebuggableProjectCfg2 * This,
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch,
            /* [out] */ __RPC__out BOOL *pfCanLaunch);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeDebugLaunch )( 
            IVsDebuggableProjectCfg2 * This,
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch);
        
        END_INTERFACE
    } IVsDebuggableProjectCfg2Vtbl;

    interface IVsDebuggableProjectCfg2
    {
        CONST_VTBL struct IVsDebuggableProjectCfg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebuggableProjectCfg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebuggableProjectCfg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebuggableProjectCfg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebuggableProjectCfg2_get_DisplayName(This,pbstrDisplayName)	\
    ( (This)->lpVtbl -> get_DisplayName(This,pbstrDisplayName) ) 

#define IVsDebuggableProjectCfg2_get_IsDebugOnly(This,pfIsDebugOnly)	\
    ( (This)->lpVtbl -> get_IsDebugOnly(This,pfIsDebugOnly) ) 

#define IVsDebuggableProjectCfg2_get_IsReleaseOnly(This,pfIsReleaseOnly)	\
    ( (This)->lpVtbl -> get_IsReleaseOnly(This,pfIsReleaseOnly) ) 


#define IVsDebuggableProjectCfg2_EnumOutputs(This,ppIVsEnumOutputs)	\
    ( (This)->lpVtbl -> EnumOutputs(This,ppIVsEnumOutputs) ) 

#define IVsDebuggableProjectCfg2_OpenOutput(This,szOutputCanonicalName,ppIVsOutput)	\
    ( (This)->lpVtbl -> OpenOutput(This,szOutputCanonicalName,ppIVsOutput) ) 

#define IVsDebuggableProjectCfg2_get_ProjectCfgProvider(This,ppIVsProjectCfgProvider)	\
    ( (This)->lpVtbl -> get_ProjectCfgProvider(This,ppIVsProjectCfgProvider) ) 

#define IVsDebuggableProjectCfg2_get_BuildableProjectCfg(This,ppIVsBuildableProjectCfg)	\
    ( (This)->lpVtbl -> get_BuildableProjectCfg(This,ppIVsBuildableProjectCfg) ) 

#define IVsDebuggableProjectCfg2_get_CanonicalName(This,pbstrCanonicalName)	\
    ( (This)->lpVtbl -> get_CanonicalName(This,pbstrCanonicalName) ) 

#define IVsDebuggableProjectCfg2_get_Platform(This,pguidPlatform)	\
    ( (This)->lpVtbl -> get_Platform(This,pguidPlatform) ) 

#define IVsDebuggableProjectCfg2_get_IsPackaged(This,pfIsPackaged)	\
    ( (This)->lpVtbl -> get_IsPackaged(This,pfIsPackaged) ) 

#define IVsDebuggableProjectCfg2_get_IsSpecifyingOutputSupported(This,pfIsSpecifyingOutputSupported)	\
    ( (This)->lpVtbl -> get_IsSpecifyingOutputSupported(This,pfIsSpecifyingOutputSupported) ) 

#define IVsDebuggableProjectCfg2_get_TargetCodePage(This,puiTargetCodePage)	\
    ( (This)->lpVtbl -> get_TargetCodePage(This,puiTargetCodePage) ) 

#define IVsDebuggableProjectCfg2_get_UpdateSequenceNumber(This,puliUSN)	\
    ( (This)->lpVtbl -> get_UpdateSequenceNumber(This,puliUSN) ) 

#define IVsDebuggableProjectCfg2_get_RootURL(This,pbstrRootURL)	\
    ( (This)->lpVtbl -> get_RootURL(This,pbstrRootURL) ) 


#define IVsDebuggableProjectCfg2_DebugLaunch(This,grfLaunch)	\
    ( (This)->lpVtbl -> DebugLaunch(This,grfLaunch) ) 

#define IVsDebuggableProjectCfg2_QueryDebugLaunch(This,grfLaunch,pfCanLaunch)	\
    ( (This)->lpVtbl -> QueryDebugLaunch(This,grfLaunch,pfCanLaunch) ) 


#define IVsDebuggableProjectCfg2_OnBeforeDebugLaunch(This,grfLaunch)	\
    ( (This)->lpVtbl -> OnBeforeDebugLaunch(This,grfLaunch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebuggableProjectCfg2_INTERFACE_DEFINED__ */


#ifndef __IVsProvideUserContext2_INTERFACE_DEFINED__
#define __IVsProvideUserContext2_INTERFACE_DEFINED__

/* interface IVsProvideUserContext2 */
/* [version][object][uuid] */ 


EXTERN_C const IID IID_IVsProvideUserContext2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3931DEF1-8200-481f-A6C2-A4740DE84658")
    IVsProvideUserContext2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUserContextEx( 
            /* [out] */ __RPC__deref_out_opt IVsUserContext **ppctx,
            /* [out] */ __RPC__out int *iPriority) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideUserContext2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideUserContext2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideUserContext2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideUserContext2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserContextEx )( 
            IVsProvideUserContext2 * This,
            /* [out] */ __RPC__deref_out_opt IVsUserContext **ppctx,
            /* [out] */ __RPC__out int *iPriority);
        
        END_INTERFACE
    } IVsProvideUserContext2Vtbl;

    interface IVsProvideUserContext2
    {
        CONST_VTBL struct IVsProvideUserContext2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideUserContext2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideUserContext2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideUserContext2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideUserContext2_GetUserContextEx(This,ppctx,iPriority)	\
    ( (This)->lpVtbl -> GetUserContextEx(This,ppctx,iPriority) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideUserContext2_INTERFACE_DEFINED__ */


#ifndef __IVsExtensibility3_INTERFACE_DEFINED__
#define __IVsExtensibility3_INTERFACE_DEFINED__

/* interface IVsExtensibility3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExtensibility3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a17be28d-6cdc-4c1e-be3e-f0ed067ea3e2")
    IVsExtensibility3 : public IUnknown
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetProperties( 
            /* [in] */ __RPC__in_opt IUnknown *pParent,
            /* [in] */ __RPC__in_opt IDispatch *pdispPropObj,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppProperties) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RunWizardFile( 
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [retval][out] */ __RPC__out long *pResult) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE EnterAutomationFunction( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE ExitAutomationFunction( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE IsInAutomationFunction( 
            /* [retval][out] */ __RPC__out BOOL *pfInAutoFunc) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetUserControl( 
            /* [out] */ __RPC__out VARIANT_BOOL *fUserControl) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetUserControl( 
            /* [in] */ VARIANT_BOOL fUserControl) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetUserControlUnlatched( 
            /* [in] */ VARIANT_BOOL fUserControl) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE LockServer( 
            /* [in] */ VARIANT_BOOL __MIDL__IVsExtensibility30000) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetLockCount( 
            /* [retval][out] */ __RPC__out long *pCount) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE TestForShutdown( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fShutdown) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetGlobalsObject( 
            /* [in] */ VARIANT ExtractFrom,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppGlobals) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetConfigMgr( 
            /* [in] */ __RPC__in_opt IUnknown *pIVsProject,
            /* [in] */ DWORD_PTR itemid,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCfgMgr) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireMacroReset( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetDocumentFromDocCookie( 
            /* [in] */ LONG_PTR lDocCookie,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppDoc) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE IsMethodDisabled( 
            /* [in] */ __RPC__in const GUID *pGUID,
            /* [in] */ long dispid) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetSuppressUI( 
            /* [in] */ VARIANT_BOOL In) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetSuppressUI( 
            /* [out][in] */ __RPC__inout VARIANT_BOOL *pOut) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemAdded( 
            /* [in] */ __RPC__in_opt IUnknown *Project) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemRemoved( 
            /* [in] */ __RPC__in_opt IUnknown *Project) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectsEvent_ItemRenamed( 
            /* [in] */ __RPC__in_opt IUnknown *Project,
            /* [in] */ __RPC__in BSTR OldName) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemAdded( 
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemRemoved( 
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireProjectItemsEvent_ItemRenamed( 
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem,
            /* [in] */ __RPC__in BSTR OldName) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE IsFireCodeModelEventNeeded( 
            /* [out][in] */ __RPC__inout VARIANT_BOOL *vbNeeded) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RunWizardFileEx( 
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vCustomParams,
            /* [retval][out] */ __RPC__out long *pResult) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE FireCodeModelEvent3( 
            /* [in] */ DISPID dispid,
            /* [in] */ __RPC__in_opt IDispatch *pParent,
            /* [in] */ __RPC__in_opt IUnknown *pElement,
            /* [in] */ long changeKind) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsExtensibility3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsExtensibility3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsExtensibility3 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetProperties )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *pParent,
            /* [in] */ __RPC__in_opt IDispatch *pdispPropObj,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppProperties);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RunWizardFile )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [retval][out] */ __RPC__out long *pResult);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *EnterAutomationFunction )( 
            IVsExtensibility3 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *ExitAutomationFunction )( 
            IVsExtensibility3 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *IsInAutomationFunction )( 
            IVsExtensibility3 * This,
            /* [retval][out] */ __RPC__out BOOL *pfInAutoFunc);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetUserControl )( 
            IVsExtensibility3 * This,
            /* [out] */ __RPC__out VARIANT_BOOL *fUserControl);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetUserControl )( 
            IVsExtensibility3 * This,
            /* [in] */ VARIANT_BOOL fUserControl);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetUserControlUnlatched )( 
            IVsExtensibility3 * This,
            /* [in] */ VARIANT_BOOL fUserControl);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *LockServer )( 
            IVsExtensibility3 * This,
            /* [in] */ VARIANT_BOOL __MIDL__IVsExtensibility30000);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetLockCount )( 
            IVsExtensibility3 * This,
            /* [retval][out] */ __RPC__out long *pCount);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *TestForShutdown )( 
            IVsExtensibility3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fShutdown);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetGlobalsObject )( 
            IVsExtensibility3 * This,
            /* [in] */ VARIANT ExtractFrom,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppGlobals);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetConfigMgr )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *pIVsProject,
            /* [in] */ DWORD_PTR itemid,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCfgMgr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireMacroReset )( 
            IVsExtensibility3 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetDocumentFromDocCookie )( 
            IVsExtensibility3 * This,
            /* [in] */ LONG_PTR lDocCookie,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppDoc);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *IsMethodDisabled )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in const GUID *pGUID,
            /* [in] */ long dispid);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetSuppressUI )( 
            IVsExtensibility3 * This,
            /* [in] */ VARIANT_BOOL In);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetSuppressUI )( 
            IVsExtensibility3 * This,
            /* [out][in] */ __RPC__inout VARIANT_BOOL *pOut);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemAdded )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *Project);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemRemoved )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *Project);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectsEvent_ItemRenamed )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *Project,
            /* [in] */ __RPC__in BSTR OldName);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemAdded )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemRemoved )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireProjectItemsEvent_ItemRenamed )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in_opt IUnknown *ProjectItem,
            /* [in] */ __RPC__in BSTR OldName);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *IsFireCodeModelEventNeeded )( 
            IVsExtensibility3 * This,
            /* [out][in] */ __RPC__inout VARIANT_BOOL *vbNeeded);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RunWizardFileEx )( 
            IVsExtensibility3 * This,
            /* [in] */ __RPC__in BSTR bstrWizFilename,
            /* [in] */ long hwndOwner,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vContextParams,
            /* [in] */ __RPC__deref_in_opt SAFEARRAY * *vCustomParams,
            /* [retval][out] */ __RPC__out long *pResult);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *FireCodeModelEvent3 )( 
            IVsExtensibility3 * This,
            /* [in] */ DISPID dispid,
            /* [in] */ __RPC__in_opt IDispatch *pParent,
            /* [in] */ __RPC__in_opt IUnknown *pElement,
            /* [in] */ long changeKind);
        
        END_INTERFACE
    } IVsExtensibility3Vtbl;

    interface IVsExtensibility3
    {
        CONST_VTBL struct IVsExtensibility3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExtensibility3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExtensibility3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExtensibility3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExtensibility3_GetProperties(This,pParent,pdispPropObj,ppProperties)	\
    ( (This)->lpVtbl -> GetProperties(This,pParent,pdispPropObj,ppProperties) ) 

#define IVsExtensibility3_RunWizardFile(This,bstrWizFilename,hwndOwner,vContextParams,pResult)	\
    ( (This)->lpVtbl -> RunWizardFile(This,bstrWizFilename,hwndOwner,vContextParams,pResult) ) 

#define IVsExtensibility3_EnterAutomationFunction(This)	\
    ( (This)->lpVtbl -> EnterAutomationFunction(This) ) 

#define IVsExtensibility3_ExitAutomationFunction(This)	\
    ( (This)->lpVtbl -> ExitAutomationFunction(This) ) 

#define IVsExtensibility3_IsInAutomationFunction(This,pfInAutoFunc)	\
    ( (This)->lpVtbl -> IsInAutomationFunction(This,pfInAutoFunc) ) 

#define IVsExtensibility3_GetUserControl(This,fUserControl)	\
    ( (This)->lpVtbl -> GetUserControl(This,fUserControl) ) 

#define IVsExtensibility3_SetUserControl(This,fUserControl)	\
    ( (This)->lpVtbl -> SetUserControl(This,fUserControl) ) 

#define IVsExtensibility3_SetUserControlUnlatched(This,fUserControl)	\
    ( (This)->lpVtbl -> SetUserControlUnlatched(This,fUserControl) ) 

#define IVsExtensibility3_LockServer(This,__MIDL__IVsExtensibility30000)	\
    ( (This)->lpVtbl -> LockServer(This,__MIDL__IVsExtensibility30000) ) 

#define IVsExtensibility3_GetLockCount(This,pCount)	\
    ( (This)->lpVtbl -> GetLockCount(This,pCount) ) 

#define IVsExtensibility3_TestForShutdown(This,fShutdown)	\
    ( (This)->lpVtbl -> TestForShutdown(This,fShutdown) ) 

#define IVsExtensibility3_GetGlobalsObject(This,ExtractFrom,ppGlobals)	\
    ( (This)->lpVtbl -> GetGlobalsObject(This,ExtractFrom,ppGlobals) ) 

#define IVsExtensibility3_GetConfigMgr(This,pIVsProject,itemid,ppCfgMgr)	\
    ( (This)->lpVtbl -> GetConfigMgr(This,pIVsProject,itemid,ppCfgMgr) ) 

#define IVsExtensibility3_FireMacroReset(This)	\
    ( (This)->lpVtbl -> FireMacroReset(This) ) 

#define IVsExtensibility3_GetDocumentFromDocCookie(This,lDocCookie,ppDoc)	\
    ( (This)->lpVtbl -> GetDocumentFromDocCookie(This,lDocCookie,ppDoc) ) 

#define IVsExtensibility3_IsMethodDisabled(This,pGUID,dispid)	\
    ( (This)->lpVtbl -> IsMethodDisabled(This,pGUID,dispid) ) 

#define IVsExtensibility3_SetSuppressUI(This,In)	\
    ( (This)->lpVtbl -> SetSuppressUI(This,In) ) 

#define IVsExtensibility3_GetSuppressUI(This,pOut)	\
    ( (This)->lpVtbl -> GetSuppressUI(This,pOut) ) 

#define IVsExtensibility3_FireProjectsEvent_ItemAdded(This,Project)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemAdded(This,Project) ) 

#define IVsExtensibility3_FireProjectsEvent_ItemRemoved(This,Project)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemRemoved(This,Project) ) 

#define IVsExtensibility3_FireProjectsEvent_ItemRenamed(This,Project,OldName)	\
    ( (This)->lpVtbl -> FireProjectsEvent_ItemRenamed(This,Project,OldName) ) 

#define IVsExtensibility3_FireProjectItemsEvent_ItemAdded(This,ProjectItem)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemAdded(This,ProjectItem) ) 

#define IVsExtensibility3_FireProjectItemsEvent_ItemRemoved(This,ProjectItem)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemRemoved(This,ProjectItem) ) 

#define IVsExtensibility3_FireProjectItemsEvent_ItemRenamed(This,ProjectItem,OldName)	\
    ( (This)->lpVtbl -> FireProjectItemsEvent_ItemRenamed(This,ProjectItem,OldName) ) 

#define IVsExtensibility3_IsFireCodeModelEventNeeded(This,vbNeeded)	\
    ( (This)->lpVtbl -> IsFireCodeModelEventNeeded(This,vbNeeded) ) 

#define IVsExtensibility3_RunWizardFileEx(This,bstrWizFilename,hwndOwner,vContextParams,vCustomParams,pResult)	\
    ( (This)->lpVtbl -> RunWizardFileEx(This,bstrWizFilename,hwndOwner,vContextParams,vCustomParams,pResult) ) 

#define IVsExtensibility3_FireCodeModelEvent3(This,dispid,pParent,pElement,changeKind)	\
    ( (This)->lpVtbl -> FireCodeModelEvent3(This,dispid,pParent,pElement,changeKind) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExtensibility3_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalsCallback2_INTERFACE_DEFINED__
#define __IVsGlobalsCallback2_INTERFACE_DEFINED__

/* interface IVsGlobalsCallback2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsGlobalsCallback2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5447ce90-561e-4c8e-be70-9fd5f89bcfa7")
    IVsGlobalsCallback2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WriteVariablesToData( 
            /* [in] */ __RPC__in LPCOLESTR pVariableName,
            /* [in] */ __RPC__in VARIANT *varData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadData( 
            /* [in] */ __RPC__in_opt IUnknown *pGlobals) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearVariables( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE VariableChanged( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanModifySource( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetParent( 
            __RPC__deref_in_opt IDispatch **ppOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsGlobalsCallback2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsGlobalsCallback2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsGlobalsCallback2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsGlobalsCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *WriteVariablesToData )( 
            IVsGlobalsCallback2 * This,
            /* [in] */ __RPC__in LPCOLESTR pVariableName,
            /* [in] */ __RPC__in VARIANT *varData);
        
        HRESULT ( STDMETHODCALLTYPE *ReadData )( 
            IVsGlobalsCallback2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pGlobals);
        
        HRESULT ( STDMETHODCALLTYPE *ClearVariables )( 
            IVsGlobalsCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *VariableChanged )( 
            IVsGlobalsCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanModifySource )( 
            IVsGlobalsCallback2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetParent )( 
            IVsGlobalsCallback2 * This,
            __RPC__deref_in_opt IDispatch **ppOut);
        
        END_INTERFACE
    } IVsGlobalsCallback2Vtbl;

    interface IVsGlobalsCallback2
    {
        CONST_VTBL struct IVsGlobalsCallback2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalsCallback2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalsCallback2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalsCallback2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalsCallback2_WriteVariablesToData(This,pVariableName,varData)	\
    ( (This)->lpVtbl -> WriteVariablesToData(This,pVariableName,varData) ) 

#define IVsGlobalsCallback2_ReadData(This,pGlobals)	\
    ( (This)->lpVtbl -> ReadData(This,pGlobals) ) 

#define IVsGlobalsCallback2_ClearVariables(This)	\
    ( (This)->lpVtbl -> ClearVariables(This) ) 

#define IVsGlobalsCallback2_VariableChanged(This)	\
    ( (This)->lpVtbl -> VariableChanged(This) ) 

#define IVsGlobalsCallback2_CanModifySource(This)	\
    ( (This)->lpVtbl -> CanModifySource(This) ) 

#define IVsGlobalsCallback2_GetParent(This,ppOut)	\
    ( (This)->lpVtbl -> GetParent(This,ppOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalsCallback2_INTERFACE_DEFINED__ */


#ifndef __IVsGlobals2_INTERFACE_DEFINED__
#define __IVsGlobals2_INTERFACE_DEFINED__

/* interface IVsGlobals2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsGlobals2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ac735863-23ef-46ed-a3ec-87b58b3df5da")
    IVsGlobals2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Load( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Empty( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsGlobals2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsGlobals2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsGlobals2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsGlobals2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Load )( 
            IVsGlobals2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IVsGlobals2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Empty )( 
            IVsGlobals2 * This);
        
        END_INTERFACE
    } IVsGlobals2Vtbl;

    interface IVsGlobals2
    {
        CONST_VTBL struct IVsGlobals2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobals2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobals2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobals2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobals2_Load(This)	\
    ( (This)->lpVtbl -> Load(This) ) 

#define IVsGlobals2_Save(This)	\
    ( (This)->lpVtbl -> Save(This) ) 

#define IVsGlobals2_Empty(This)	\
    ( (This)->lpVtbl -> Empty(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobals2_INTERFACE_DEFINED__ */


#ifndef __IVsProfferCommands3_INTERFACE_DEFINED__
#define __IVsProfferCommands3_INTERFACE_DEFINED__

/* interface IVsProfferCommands3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfferCommands3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3a83904d-4540-4c51-95a7-618b32a9a9c0")
    IVsProfferCommands3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNamedCommand( 
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveNamedCommand( 
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RenameNamedCommand( 
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonicalNew,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalizedNew) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddCommandBarControl( 
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [in] */ DWORD dwCmdType,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBarCtrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveCommandBarControl( 
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarCtrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddCommandBar( 
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdBarName,
            /* [in] */ DWORD dwType,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveCommandBar( 
            /* [in] */ __RPC__in_opt IDispatch *pCmdBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindCommandBar( 
            /* [in] */ __RPC__in_opt IUnknown *pToolbarSet,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [in] */ DWORD dwMenuId,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispCmdBar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddNamedCommand2( 
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProfferCommands3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProfferCommands3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProfferCommands3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveNamedCommand )( 
            IVsProfferCommands3 * This,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical);
        
        HRESULT ( STDMETHODCALLTYPE *RenameNamedCommand )( 
            IVsProfferCommands3 * This,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonicalNew,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalizedNew);
        
        HRESULT ( STDMETHODCALLTYPE *AddCommandBarControl )( 
            IVsProfferCommands3 * This,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [in] */ DWORD dwCmdType,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBarCtrl);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCommandBarControl )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarCtrl);
        
        HRESULT ( STDMETHODCALLTYPE *AddCommandBar )( 
            IVsProfferCommands3 * This,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdBarName,
            /* [in] */ DWORD dwType,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCommandBar )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *FindCommandBar )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in_opt IUnknown *pToolbarSet,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [in] */ DWORD dwMenuId,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand2 )( 
            IVsProfferCommands3 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType);
        
        END_INTERFACE
    } IVsProfferCommands3Vtbl;

    interface IVsProfferCommands3
    {
        CONST_VTBL struct IVsProfferCommands3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfferCommands3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfferCommands3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfferCommands3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfferCommands3_AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts)	\
    ( (This)->lpVtbl -> AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts) ) 

#define IVsProfferCommands3_RemoveNamedCommand(This,pszCmdNameCanonical)	\
    ( (This)->lpVtbl -> RemoveNamedCommand(This,pszCmdNameCanonical) ) 

#define IVsProfferCommands3_RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew)	\
    ( (This)->lpVtbl -> RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew) ) 

#define IVsProfferCommands3_AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl)	\
    ( (This)->lpVtbl -> AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl) ) 

#define IVsProfferCommands3_RemoveCommandBarControl(This,pCmdBarCtrl)	\
    ( (This)->lpVtbl -> RemoveCommandBarControl(This,pCmdBarCtrl) ) 

#define IVsProfferCommands3_AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar)	\
    ( (This)->lpVtbl -> AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar) ) 

#define IVsProfferCommands3_RemoveCommandBar(This,pCmdBar)	\
    ( (This)->lpVtbl -> RemoveCommandBar(This,pCmdBar) ) 

#define IVsProfferCommands3_FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar)	\
    ( (This)->lpVtbl -> FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar) ) 

#define IVsProfferCommands3_AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType)	\
    ( (This)->lpVtbl -> AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfferCommands3_INTERFACE_DEFINED__ */


#ifndef __IVsHierarchyRefactorNotify_INTERFACE_DEFINED__
#define __IVsHierarchyRefactorNotify_INTERFACE_DEFINED__

/* interface IVsHierarchyRefactorNotify */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyRefactorNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FE5FEE31-C302-4961-876F-F3C8F853E4F8")
    IVsHierarchyRefactorNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeGlobalSymbolRenamed( 
            /* [in] */ ULONG cItemsAffected,
            /* [size_is][in] */ __RPC__in_ecount_full(cItemsAffected) VSITEMID rgItemsAffected[  ],
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName,
            /* [in] */ BOOL promptContinueOnFail) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnGlobalSymbolRenamed( 
            /* [in] */ ULONG cItemsAffected,
            /* [size_is][in] */ __RPC__in_ecount_full(cItemsAffected) VSITEMID rgItemsAffected[  ],
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeReorderParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [in] */ BOOL promptContinueOnFail) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReorderParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeRemoveParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [in] */ BOOL promptContinueOnFail) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoveParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeAddParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ],
            /* [in] */ BOOL promptContinueOnFail) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAddParams( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHierarchyRefactorNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHierarchyRefactorNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHierarchyRefactorNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeGlobalSymbolRenamed )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ ULONG cItemsAffected,
            /* [size_is][in] */ __RPC__in_ecount_full(cItemsAffected) VSITEMID rgItemsAffected[  ],
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName,
            /* [in] */ BOOL promptContinueOnFail);
        
        HRESULT ( STDMETHODCALLTYPE *OnGlobalSymbolRenamed )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ ULONG cItemsAffected,
            /* [size_is][in] */ __RPC__in_ecount_full(cItemsAffected) VSITEMID rgItemsAffected[  ],
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeReorderParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [in] */ BOOL promptContinueOnFail);
        
        HRESULT ( STDMETHODCALLTYPE *OnReorderParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeRemoveParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [in] */ BOOL promptContinueOnFail);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoveParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeAddParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ],
            /* [in] */ BOOL promptContinueOnFail);
        
        HRESULT ( STDMETHODCALLTYPE *OnAddParams )( 
            IVsHierarchyRefactorNotify * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ]);
        
        END_INTERFACE
    } IVsHierarchyRefactorNotifyVtbl;

    interface IVsHierarchyRefactorNotify
    {
        CONST_VTBL struct IVsHierarchyRefactorNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyRefactorNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyRefactorNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyRefactorNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyRefactorNotify_OnBeforeGlobalSymbolRenamed(This,cItemsAffected,rgItemsAffected,cRQNames,rglpszRQName,lpszNewName,promptContinueOnFail)	\
    ( (This)->lpVtbl -> OnBeforeGlobalSymbolRenamed(This,cItemsAffected,rgItemsAffected,cRQNames,rglpszRQName,lpszNewName,promptContinueOnFail) ) 

#define IVsHierarchyRefactorNotify_OnGlobalSymbolRenamed(This,cItemsAffected,rgItemsAffected,cRQNames,rglpszRQName,lpszNewName)	\
    ( (This)->lpVtbl -> OnGlobalSymbolRenamed(This,cItemsAffected,rgItemsAffected,cRQNames,rglpszRQName,lpszNewName) ) 

#define IVsHierarchyRefactorNotify_OnBeforeReorderParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes,promptContinueOnFail)	\
    ( (This)->lpVtbl -> OnBeforeReorderParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes,promptContinueOnFail) ) 

#define IVsHierarchyRefactorNotify_OnReorderParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes)	\
    ( (This)->lpVtbl -> OnReorderParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes) ) 

#define IVsHierarchyRefactorNotify_OnBeforeRemoveParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes,promptContinueOnFail)	\
    ( (This)->lpVtbl -> OnBeforeRemoveParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes,promptContinueOnFail) ) 

#define IVsHierarchyRefactorNotify_OnRemoveParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes)	\
    ( (This)->lpVtbl -> OnRemoveParams(This,itemid,lpszRQName,cParamIndexes,rgParamIndexes) ) 

#define IVsHierarchyRefactorNotify_OnBeforeAddParams(This,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames,promptContinueOnFail)	\
    ( (This)->lpVtbl -> OnBeforeAddParams(This,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames,promptContinueOnFail) ) 

#define IVsHierarchyRefactorNotify_OnAddParams(This,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames)	\
    ( (This)->lpVtbl -> OnAddParams(This,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyRefactorNotify_INTERFACE_DEFINED__ */


#ifndef __IVsRefactorNotify_INTERFACE_DEFINED__
#define __IVsRefactorNotify_INTERFACE_DEFINED__

/* interface IVsRefactorNotify */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRefactorNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("130497E3-5CDB-4f29-9804-A2AF805016D7")
    IVsRefactorNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeGlobalSymbolRenamed( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnGlobalSymbolRenamed( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeReorderParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReorderParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeRemoveParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoveParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeAddParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAddParams( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRefactorNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRefactorNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRefactorNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeGlobalSymbolRenamed )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs);
        
        HRESULT ( STDMETHODCALLTYPE *OnGlobalSymbolRenamed )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ ULONG cRQNames,
            /* [size_is][in] */ __RPC__in_ecount_full(cRQNames) LPCOLESTR rglpszRQName[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszNewName);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeReorderParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs);
        
        HRESULT ( STDMETHODCALLTYPE *OnReorderParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeRemoveParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoveParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParamIndexes,
            /* [size_is][in] */ __RPC__in_ecount_full(cParamIndexes) ULONG rgParamIndexes[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeAddParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgAdditionalCheckoutVSITEMIDs);
        
        HRESULT ( STDMETHODCALLTYPE *OnAddParams )( 
            IVsRefactorNotify * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR lpszRQName,
            /* [in] */ ULONG cParams,
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) ULONG rgszParamIndexes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszRQTypeNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cParams) LPCOLESTR rgszParamNames[  ]);
        
        END_INTERFACE
    } IVsRefactorNotifyVtbl;

    interface IVsRefactorNotify
    {
        CONST_VTBL struct IVsRefactorNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRefactorNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRefactorNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRefactorNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRefactorNotify_OnBeforeGlobalSymbolRenamed(This,pHier,itemid,cRQNames,rglpszRQName,lpszNewName,prgAdditionalCheckoutVSITEMIDs)	\
    ( (This)->lpVtbl -> OnBeforeGlobalSymbolRenamed(This,pHier,itemid,cRQNames,rglpszRQName,lpszNewName,prgAdditionalCheckoutVSITEMIDs) ) 

#define IVsRefactorNotify_OnGlobalSymbolRenamed(This,pHier,itemid,cRQNames,rglpszRQName,lpszNewName)	\
    ( (This)->lpVtbl -> OnGlobalSymbolRenamed(This,pHier,itemid,cRQNames,rglpszRQName,lpszNewName) ) 

#define IVsRefactorNotify_OnBeforeReorderParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes,prgAdditionalCheckoutVSITEMIDs)	\
    ( (This)->lpVtbl -> OnBeforeReorderParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes,prgAdditionalCheckoutVSITEMIDs) ) 

#define IVsRefactorNotify_OnReorderParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes)	\
    ( (This)->lpVtbl -> OnReorderParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes) ) 

#define IVsRefactorNotify_OnBeforeRemoveParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes,prgAdditionalCheckoutVSITEMIDs)	\
    ( (This)->lpVtbl -> OnBeforeRemoveParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes,prgAdditionalCheckoutVSITEMIDs) ) 

#define IVsRefactorNotify_OnRemoveParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes)	\
    ( (This)->lpVtbl -> OnRemoveParams(This,pHier,itemid,lpszRQName,cParamIndexes,rgParamIndexes) ) 

#define IVsRefactorNotify_OnBeforeAddParams(This,pHier,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames,prgAdditionalCheckoutVSITEMIDs)	\
    ( (This)->lpVtbl -> OnBeforeAddParams(This,pHier,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames,prgAdditionalCheckoutVSITEMIDs) ) 

#define IVsRefactorNotify_OnAddParams(This,pHier,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames)	\
    ( (This)->lpVtbl -> OnAddParams(This,pHier,itemid,lpszRQName,cParams,rgszParamIndexes,rgszRQTypeNames,rgszParamNames) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRefactorNotify_INTERFACE_DEFINED__ */


#ifndef __IVsMonitorSelection2_INTERFACE_DEFINED__
#define __IVsMonitorSelection2_INTERFACE_DEFINED__

/* interface IVsMonitorSelection2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsMonitorSelection2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("692e4f21-1ef2-41b1-9116-eed8daa79e7f")
    IVsMonitorSelection2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetElementID( 
            /* [in] */ __RPC__in REFGUID rguidElement,
            /* [out] */ __RPC__out VSSELELEMID *pElementId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEmptySelectionContext( 
            /* [out] */ __RPC__deref_out_opt IVsTrackSelectionEx **ppEmptySelCtxt) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsMonitorSelection2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsMonitorSelection2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsMonitorSelection2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsMonitorSelection2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetElementID )( 
            IVsMonitorSelection2 * This,
            /* [in] */ __RPC__in REFGUID rguidElement,
            /* [out] */ __RPC__out VSSELELEMID *pElementId);
        
        HRESULT ( STDMETHODCALLTYPE *GetEmptySelectionContext )( 
            IVsMonitorSelection2 * This,
            /* [out] */ __RPC__deref_out_opt IVsTrackSelectionEx **ppEmptySelCtxt);
        
        END_INTERFACE
    } IVsMonitorSelection2Vtbl;

    interface IVsMonitorSelection2
    {
        CONST_VTBL struct IVsMonitorSelection2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMonitorSelection2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMonitorSelection2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMonitorSelection2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMonitorSelection2_GetElementID(This,rguidElement,pElementId)	\
    ( (This)->lpVtbl -> GetElementID(This,rguidElement,pElementId) ) 

#define IVsMonitorSelection2_GetEmptySelectionContext(This,ppEmptySelCtxt)	\
    ( (This)->lpVtbl -> GetEmptySelectionContext(This,ppEmptySelCtxt) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMonitorSelection2_INTERFACE_DEFINED__ */


#ifndef __IVsToolsOptions_INTERFACE_DEFINED__
#define __IVsToolsOptions_INTERFACE_DEFINED__

/* interface IVsToolsOptions */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolsOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AE31D40E-CD7A-45cb-8DEF-5EA0E44C688A")
    IVsToolsOptions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsToolsOptionsOpen( 
            /* [out] */ __RPC__out BOOL *pfOpen) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshPageVisibility( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsToolsOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsToolsOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsToolsOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsToolsOptions * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsToolsOptionsOpen )( 
            IVsToolsOptions * This,
            /* [out] */ __RPC__out BOOL *pfOpen);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshPageVisibility )( 
            IVsToolsOptions * This);
        
        END_INTERFACE
    } IVsToolsOptionsVtbl;

    interface IVsToolsOptions
    {
        CONST_VTBL struct IVsToolsOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolsOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolsOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolsOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolsOptions_IsToolsOptionsOpen(This,pfOpen)	\
    ( (This)->lpVtbl -> IsToolsOptionsOpen(This,pfOpen) ) 

#define IVsToolsOptions_RefreshPageVisibility(This)	\
    ( (This)->lpVtbl -> RefreshPageVisibility(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolsOptions_INTERFACE_DEFINED__ */


#ifndef __SVsToolsOptions_INTERFACE_DEFINED__
#define __SVsToolsOptions_INTERFACE_DEFINED__

/* interface SVsToolsOptions */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsToolsOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("376208E4-2679-4c9e-B3D5-929EB0F1A1F7")
    SVsToolsOptions : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsToolsOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsToolsOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsToolsOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsToolsOptions * This);
        
        END_INTERFACE
    } SVsToolsOptionsVtbl;

    interface SVsToolsOptions
    {
        CONST_VTBL struct SVsToolsOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsToolsOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsToolsOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsToolsOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsToolsOptions_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0174 */
/* [local] */ 

#define SID_SVsToolsOptions IID_SVsToolsOptions


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0174_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0174_v0_0_s_ifspec;

#ifndef __IVsDeployableProjectCfg2_INTERFACE_DEFINED__
#define __IVsDeployableProjectCfg2_INTERFACE_DEFINED__

/* interface IVsDeployableProjectCfg2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDeployableProjectCfg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A981529F-4D0D-46ee-A758-AC26E50E099D")
    IVsDeployableProjectCfg2 : public IVsDeployableProjectCfg
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartCleanDeploy( 
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDeployableProjectCfg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDeployableProjectCfg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDeployableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseDeployStatusCallback )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ __RPC__in_opt IVsDeployStatusCallback *pIVsDeployStatusCallback,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseDeployStatusCallback )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *StartDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions);
        
        HRESULT ( STDMETHODCALLTYPE *QueryStatusDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [out] */ __RPC__out BOOL *pfDeployDone);
        
        HRESULT ( STDMETHODCALLTYPE *StopDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ BOOL fSync);
        
        HRESULT ( STDMETHODCALLTYPE *WaitDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ DWORD dwMilliseconds,
            /* [in] */ BOOL fTickWhenMessageQNotEmpty);
        
        HRESULT ( STDMETHODCALLTYPE *QueryStartDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ DWORD dwOptions,
            /* [optional][out] */ __RPC__out BOOL *pfSupported,
            /* [optional][out] */ __RPC__out BOOL *pfReady);
        
        HRESULT ( STDMETHODCALLTYPE *Commit )( 
            IVsDeployableProjectCfg2 * This,
            DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *Rollback )( 
            IVsDeployableProjectCfg2 * This,
            DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *StartCleanDeploy )( 
            IVsDeployableProjectCfg2 * This,
            /* [in] */ __RPC__in_opt IVsOutputWindowPane *pIVsOutputWindowPane,
            /* [in] */ DWORD dwOptions);
        
        END_INTERFACE
    } IVsDeployableProjectCfg2Vtbl;

    interface IVsDeployableProjectCfg2
    {
        CONST_VTBL struct IVsDeployableProjectCfg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDeployableProjectCfg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDeployableProjectCfg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDeployableProjectCfg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDeployableProjectCfg2_AdviseDeployStatusCallback(This,pIVsDeployStatusCallback,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseDeployStatusCallback(This,pIVsDeployStatusCallback,pdwCookie) ) 

#define IVsDeployableProjectCfg2_UnadviseDeployStatusCallback(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseDeployStatusCallback(This,dwCookie) ) 

#define IVsDeployableProjectCfg2_StartDeploy(This,pIVsOutputWindowPane,dwOptions)	\
    ( (This)->lpVtbl -> StartDeploy(This,pIVsOutputWindowPane,dwOptions) ) 

#define IVsDeployableProjectCfg2_QueryStatusDeploy(This,pfDeployDone)	\
    ( (This)->lpVtbl -> QueryStatusDeploy(This,pfDeployDone) ) 

#define IVsDeployableProjectCfg2_StopDeploy(This,fSync)	\
    ( (This)->lpVtbl -> StopDeploy(This,fSync) ) 

#define IVsDeployableProjectCfg2_WaitDeploy(This,dwMilliseconds,fTickWhenMessageQNotEmpty)	\
    ( (This)->lpVtbl -> WaitDeploy(This,dwMilliseconds,fTickWhenMessageQNotEmpty) ) 

#define IVsDeployableProjectCfg2_QueryStartDeploy(This,dwOptions,pfSupported,pfReady)	\
    ( (This)->lpVtbl -> QueryStartDeploy(This,dwOptions,pfSupported,pfReady) ) 

#define IVsDeployableProjectCfg2_Commit(This,dwReserved)	\
    ( (This)->lpVtbl -> Commit(This,dwReserved) ) 

#define IVsDeployableProjectCfg2_Rollback(This,dwReserved)	\
    ( (This)->lpVtbl -> Rollback(This,dwReserved) ) 


#define IVsDeployableProjectCfg2_StartCleanDeploy(This,pIVsOutputWindowPane,dwOptions)	\
    ( (This)->lpVtbl -> StartCleanDeploy(This,pIVsOutputWindowPane,dwOptions) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDeployableProjectCfg2_INTERFACE_DEFINED__ */


#ifndef __IVsFontAndColorStorage2_INTERFACE_DEFINED__
#define __IVsFontAndColorStorage2_INTERFACE_DEFINED__

/* interface IVsFontAndColorStorage2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFontAndColorStorage2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1EE6C79A-B763-42e6-AC95-FD0CC00DE315")
    IVsFontAndColorStorage2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RevertFontToDefault( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RevertItemToDefault( 
            /* [in] */ __RPC__in LPCOLESTR szName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RevertAllItemsToDefault( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFontAndColorStorage2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFontAndColorStorage2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFontAndColorStorage2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFontAndColorStorage2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RevertFontToDefault )( 
            IVsFontAndColorStorage2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RevertItemToDefault )( 
            IVsFontAndColorStorage2 * This,
            /* [in] */ __RPC__in LPCOLESTR szName);
        
        HRESULT ( STDMETHODCALLTYPE *RevertAllItemsToDefault )( 
            IVsFontAndColorStorage2 * This);
        
        END_INTERFACE
    } IVsFontAndColorStorage2Vtbl;

    interface IVsFontAndColorStorage2
    {
        CONST_VTBL struct IVsFontAndColorStorage2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFontAndColorStorage2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFontAndColorStorage2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFontAndColorStorage2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFontAndColorStorage2_RevertFontToDefault(This)	\
    ( (This)->lpVtbl -> RevertFontToDefault(This) ) 

#define IVsFontAndColorStorage2_RevertItemToDefault(This,szName)	\
    ( (This)->lpVtbl -> RevertItemToDefault(This,szName) ) 

#define IVsFontAndColorStorage2_RevertAllItemsToDefault(This)	\
    ( (This)->lpVtbl -> RevertAllItemsToDefault(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFontAndColorStorage2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0176 */
/* [local] */ 


enum __VSSHOWCONTEXTMENUOPTS
    {	VSCTXMENU_SELECTFIRSTITEM	= 0x10000,
	VSCTXMENU_SHOWUNDERLINES	= 0x20000,
	VSCTXMENU_SUPPORTSTYPEAHEAD	= 0x40000
    } ;
typedef DWORD VSSHOWCONTEXTMENUOPTS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0176_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0176_v0_0_s_ifspec;

#ifndef __IVsDocOutlineProvider2_INTERFACE_DEFINED__
#define __IVsDocOutlineProvider2_INTERFACE_DEFINED__

/* interface IVsDocOutlineProvider2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDocOutlineProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9EB7079F-3445-4c43-99D8-46EA8CA1D659")
    IVsDocOutlineProvider2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE TranslateAccelerator( 
            /* [in] */ __RPC__in LPMSG lpMsg) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDocOutlineProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDocOutlineProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDocOutlineProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDocOutlineProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateAccelerator )( 
            IVsDocOutlineProvider2 * This,
            /* [in] */ __RPC__in LPMSG lpMsg);
        
        END_INTERFACE
    } IVsDocOutlineProvider2Vtbl;

    interface IVsDocOutlineProvider2
    {
        CONST_VTBL struct IVsDocOutlineProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDocOutlineProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDocOutlineProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDocOutlineProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDocOutlineProvider2_TranslateAccelerator(This,lpMsg)	\
    ( (This)->lpVtbl -> TranslateAccelerator(This,lpMsg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDocOutlineProvider2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0177 */
/* [local] */ 


enum __VSCREATEWEBBROWSER2
    {	VSCWB_NoHistoryThisPage	= 0x100000,
	VSCWB_NavOptionMask2	= 0x1f0000
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0177_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0177_v0_0_s_ifspec;

#ifndef __IVSMDTypeResolutionService_INTERFACE_DEFINED__
#define __IVSMDTypeResolutionService_INTERFACE_DEFINED__

/* interface IVSMDTypeResolutionService */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVSMDTypeResolutionService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3411DD99-2445-43c8-918E-99BFBFAF8292")
    IVSMDTypeResolutionService : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TypeResolutionService( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppTrs) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVSMDTypeResolutionServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVSMDTypeResolutionService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVSMDTypeResolutionService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVSMDTypeResolutionService * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TypeResolutionService )( 
            IVSMDTypeResolutionService * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppTrs);
        
        END_INTERFACE
    } IVSMDTypeResolutionServiceVtbl;

    interface IVSMDTypeResolutionService
    {
        CONST_VTBL struct IVSMDTypeResolutionServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVSMDTypeResolutionService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVSMDTypeResolutionService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVSMDTypeResolutionService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVSMDTypeResolutionService_get_TypeResolutionService(This,ppTrs)	\
    ( (This)->lpVtbl -> get_TypeResolutionService(This,ppTrs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVSMDTypeResolutionService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0178 */
/* [local] */ 

#define SID_SVSMDTypeResolutionService IID_IVSMDTypeResolutionService
typedef struct _VSDEFAULTPREVIEWER
    {
    BSTR bstrDefBrowserPath;
    BSTR bstrDefBrowserDisplayName;
    BOOL fIsInternalBrowser;
    BOOL fIsSystemBrowser;
    VSPREVIEWRESOLUTION defRes;
    } 	VSDEFAULTPREVIEWER;



extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0178_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0178_v0_0_s_ifspec;

#ifndef __IVsUIShellOpenDocument2_INTERFACE_DEFINED__
#define __IVsUIShellOpenDocument2_INTERFACE_DEFINED__

/* interface IVsUIShellOpenDocument2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIShellOpenDocument2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0649BDA0-0978-4ca0-AB0B-0F619199BCCA")
    IVsUIShellOpenDocument2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDefaultPreviewers( 
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSDEFAULTPREVIEWER rgDefaultPreviewers[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIShellOpenDocument2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIShellOpenDocument2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIShellOpenDocument2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIShellOpenDocument2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultPreviewers )( 
            IVsUIShellOpenDocument2 * This,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSDEFAULTPREVIEWER rgDefaultPreviewers[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsUIShellOpenDocument2Vtbl;

    interface IVsUIShellOpenDocument2
    {
        CONST_VTBL struct IVsUIShellOpenDocument2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShellOpenDocument2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShellOpenDocument2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShellOpenDocument2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShellOpenDocument2_GetDefaultPreviewers(This,celt,rgDefaultPreviewers,pcActual)	\
    ( (This)->lpVtbl -> GetDefaultPreviewers(This,celt,rgDefaultPreviewers,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShellOpenDocument2_INTERFACE_DEFINED__ */


#ifndef __IVsFilterNewProjectDlg_INTERFACE_DEFINED__
#define __IVsFilterNewProjectDlg_INTERFACE_DEFINED__

/* interface IVsFilterNewProjectDlg */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFilterNewProjectDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B10EC465-CEC8-41cd-A132-6C1A58F565FB")
    IVsFilterNewProjectDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE FilterTreeItemByLocalizedName( 
            /* [in] */ __RPC__in LPCOLESTR pszLocalizedName,
            /* [out] */ __RPC__out BOOL *pfFilter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FilterTreeItemByTemplateDir( 
            /* [in] */ __RPC__in LPCOLESTR pszTemplateDir,
            /* [out] */ __RPC__out BOOL *pfFilter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FilterListItemByLocalizedName( 
            /* [in] */ __RPC__in LPCOLESTR pszLocalizedName,
            /* [out] */ __RPC__out BOOL *pfFilter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FilterListItemByTemplateFile( 
            /* [in] */ __RPC__in LPCOLESTR pszTemplateFile,
            /* [out] */ __RPC__out BOOL *pfFilter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFilterNewProjectDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFilterNewProjectDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFilterNewProjectDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFilterNewProjectDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *FilterTreeItemByLocalizedName )( 
            IVsFilterNewProjectDlg * This,
            /* [in] */ __RPC__in LPCOLESTR pszLocalizedName,
            /* [out] */ __RPC__out BOOL *pfFilter);
        
        HRESULT ( STDMETHODCALLTYPE *FilterTreeItemByTemplateDir )( 
            IVsFilterNewProjectDlg * This,
            /* [in] */ __RPC__in LPCOLESTR pszTemplateDir,
            /* [out] */ __RPC__out BOOL *pfFilter);
        
        HRESULT ( STDMETHODCALLTYPE *FilterListItemByLocalizedName )( 
            IVsFilterNewProjectDlg * This,
            /* [in] */ __RPC__in LPCOLESTR pszLocalizedName,
            /* [out] */ __RPC__out BOOL *pfFilter);
        
        HRESULT ( STDMETHODCALLTYPE *FilterListItemByTemplateFile )( 
            IVsFilterNewProjectDlg * This,
            /* [in] */ __RPC__in LPCOLESTR pszTemplateFile,
            /* [out] */ __RPC__out BOOL *pfFilter);
        
        END_INTERFACE
    } IVsFilterNewProjectDlgVtbl;

    interface IVsFilterNewProjectDlg
    {
        CONST_VTBL struct IVsFilterNewProjectDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFilterNewProjectDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFilterNewProjectDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFilterNewProjectDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFilterNewProjectDlg_FilterTreeItemByLocalizedName(This,pszLocalizedName,pfFilter)	\
    ( (This)->lpVtbl -> FilterTreeItemByLocalizedName(This,pszLocalizedName,pfFilter) ) 

#define IVsFilterNewProjectDlg_FilterTreeItemByTemplateDir(This,pszTemplateDir,pfFilter)	\
    ( (This)->lpVtbl -> FilterTreeItemByTemplateDir(This,pszTemplateDir,pfFilter) ) 

#define IVsFilterNewProjectDlg_FilterListItemByLocalizedName(This,pszLocalizedName,pfFilter)	\
    ( (This)->lpVtbl -> FilterListItemByLocalizedName(This,pszLocalizedName,pfFilter) ) 

#define IVsFilterNewProjectDlg_FilterListItemByTemplateFile(This,pszTemplateFile,pfFilter)	\
    ( (This)->lpVtbl -> FilterListItemByTemplateFile(This,pszTemplateFile,pfFilter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFilterNewProjectDlg_INTERFACE_DEFINED__ */


#ifndef __IVsRegisterNewDialogFilters_INTERFACE_DEFINED__
#define __IVsRegisterNewDialogFilters_INTERFACE_DEFINED__

/* interface IVsRegisterNewDialogFilters */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterNewDialogFilters;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9455BDB5-2A5A-45f1-A558-72B88A78E6E3")
    IVsRegisterNewDialogFilters : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterNewProjectDialogFilter( 
            /* [in] */ __RPC__in_opt IVsFilterNewProjectDlg *pFilter,
            /* [out] */ __RPC__out VSCOOKIE *pdwFilterCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterNewProjectDialogFilter( 
            /* [in] */ VSCOOKIE dwFilterCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterAddNewItemDialogFilter( 
            /* [in] */ __RPC__in_opt IVsFilterAddProjectItemDlg *pFilter,
            /* [out] */ __RPC__out VSCOOKIE *pdwFilterCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterAddNewItemDialogFilter( 
            /* [in] */ VSCOOKIE dwFilterCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterNewDialogFiltersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterNewDialogFilters * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterNewDialogFilters * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterNewDialogFilters * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterNewProjectDialogFilter )( 
            IVsRegisterNewDialogFilters * This,
            /* [in] */ __RPC__in_opt IVsFilterNewProjectDlg *pFilter,
            /* [out] */ __RPC__out VSCOOKIE *pdwFilterCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterNewProjectDialogFilter )( 
            IVsRegisterNewDialogFilters * This,
            /* [in] */ VSCOOKIE dwFilterCookie);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterAddNewItemDialogFilter )( 
            IVsRegisterNewDialogFilters * This,
            /* [in] */ __RPC__in_opt IVsFilterAddProjectItemDlg *pFilter,
            /* [out] */ __RPC__out VSCOOKIE *pdwFilterCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterAddNewItemDialogFilter )( 
            IVsRegisterNewDialogFilters * This,
            /* [in] */ VSCOOKIE dwFilterCookie);
        
        END_INTERFACE
    } IVsRegisterNewDialogFiltersVtbl;

    interface IVsRegisterNewDialogFilters
    {
        CONST_VTBL struct IVsRegisterNewDialogFiltersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterNewDialogFilters_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterNewDialogFilters_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterNewDialogFilters_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterNewDialogFilters_RegisterNewProjectDialogFilter(This,pFilter,pdwFilterCookie)	\
    ( (This)->lpVtbl -> RegisterNewProjectDialogFilter(This,pFilter,pdwFilterCookie) ) 

#define IVsRegisterNewDialogFilters_UnregisterNewProjectDialogFilter(This,dwFilterCookie)	\
    ( (This)->lpVtbl -> UnregisterNewProjectDialogFilter(This,dwFilterCookie) ) 

#define IVsRegisterNewDialogFilters_RegisterAddNewItemDialogFilter(This,pFilter,pdwFilterCookie)	\
    ( (This)->lpVtbl -> RegisterAddNewItemDialogFilter(This,pFilter,pdwFilterCookie) ) 

#define IVsRegisterNewDialogFilters_UnregisterAddNewItemDialogFilter(This,dwFilterCookie)	\
    ( (This)->lpVtbl -> UnregisterAddNewItemDialogFilter(This,dwFilterCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterNewDialogFilters_INTERFACE_DEFINED__ */


#ifndef __SVsRegisterNewDialogFilters_INTERFACE_DEFINED__
#define __SVsRegisterNewDialogFilters_INTERFACE_DEFINED__

/* interface SVsRegisterNewDialogFilters */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsRegisterNewDialogFilters;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CB7C5B29-6782-47b7-AA13-21D07026D5E1")
    SVsRegisterNewDialogFilters : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsRegisterNewDialogFiltersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsRegisterNewDialogFilters * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsRegisterNewDialogFilters * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsRegisterNewDialogFilters * This);
        
        END_INTERFACE
    } SVsRegisterNewDialogFiltersVtbl;

    interface SVsRegisterNewDialogFilters
    {
        CONST_VTBL struct SVsRegisterNewDialogFiltersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsRegisterNewDialogFilters_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsRegisterNewDialogFilters_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsRegisterNewDialogFilters_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsRegisterNewDialogFilters_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell80_0000_0182 */
/* [local] */ 

#define SID_SVsRegisterNewDialogFilters IID_SVsRegisterNewDialogFilters


extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0182_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell80_0000_0182_v0_0_s_ifspec;

#ifndef __IVsWebBrowserUser2_INTERFACE_DEFINED__
#define __IVsWebBrowserUser2_INTERFACE_DEFINED__

/* interface IVsWebBrowserUser2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWebBrowserUser2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("821ABD48-96DC-4315-A2C4-82A7239B8166")
    IVsWebBrowserUser2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetWebBrowserContext( 
            /* [out] */ __RPC__deref_out_opt IServiceProvider **ppServiceProvider) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWebBrowserUser2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWebBrowserUser2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWebBrowserUser2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWebBrowserUser2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetWebBrowserContext )( 
            IVsWebBrowserUser2 * This,
            /* [out] */ __RPC__deref_out_opt IServiceProvider **ppServiceProvider);
        
        END_INTERFACE
    } IVsWebBrowserUser2Vtbl;

    interface IVsWebBrowserUser2
    {
        CONST_VTBL struct IVsWebBrowserUser2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWebBrowserUser2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWebBrowserUser2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWebBrowserUser2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWebBrowserUser2_GetWebBrowserContext(This,ppServiceProvider)	\
    ( (This)->lpVtbl -> GetWebBrowserContext(This,ppServiceProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWebBrowserUser2_INTERFACE_DEFINED__ */


#ifndef __IVsHasRelatedSaveItems_INTERFACE_DEFINED__
#define __IVsHasRelatedSaveItems_INTERFACE_DEFINED__

/* interface IVsHasRelatedSaveItems */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHasRelatedSaveItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D82269C8-C3DB-4bd9-AF32-AB140BCFDAE3")
    IVsHasRelatedSaveItems : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRelatedSaveTreeItems( 
            /* [in] */ VSSAVETREEITEM saveItem,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSSAVETREEITEM rgSaveTreeItems[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHasRelatedSaveItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHasRelatedSaveItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHasRelatedSaveItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHasRelatedSaveItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRelatedSaveTreeItems )( 
            IVsHasRelatedSaveItems * This,
            /* [in] */ VSSAVETREEITEM saveItem,
            /* [in] */ ULONG celt,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(celt) VSSAVETREEITEM rgSaveTreeItems[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsHasRelatedSaveItemsVtbl;

    interface IVsHasRelatedSaveItems
    {
        CONST_VTBL struct IVsHasRelatedSaveItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHasRelatedSaveItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHasRelatedSaveItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHasRelatedSaveItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHasRelatedSaveItems_GetRelatedSaveTreeItems(This,saveItem,celt,rgSaveTreeItems,pcActual)	\
    ( (This)->lpVtbl -> GetRelatedSaveTreeItems(This,saveItem,celt,rgSaveTreeItems,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHasRelatedSaveItems_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  CLIPFORMAT_UserSize(     unsigned long *, unsigned long            , CLIPFORMAT * ); 
unsigned char * __RPC_USER  CLIPFORMAT_UserMarshal(  unsigned long *, unsigned char *, CLIPFORMAT * ); 
unsigned char * __RPC_USER  CLIPFORMAT_UserUnmarshal(unsigned long *, unsigned char *, CLIPFORMAT * ); 
void                      __RPC_USER  CLIPFORMAT_UserFree(     unsigned long *, CLIPFORMAT * ); 

unsigned long             __RPC_USER  HBITMAP_UserSize(     unsigned long *, unsigned long            , HBITMAP * ); 
unsigned char * __RPC_USER  HBITMAP_UserMarshal(  unsigned long *, unsigned char *, HBITMAP * ); 
unsigned char * __RPC_USER  HBITMAP_UserUnmarshal(unsigned long *, unsigned char *, HBITMAP * ); 
void                      __RPC_USER  HBITMAP_UserFree(     unsigned long *, HBITMAP * ); 

unsigned long             __RPC_USER  HDC_UserSize(     unsigned long *, unsigned long            , HDC * ); 
unsigned char * __RPC_USER  HDC_UserMarshal(  unsigned long *, unsigned char *, HDC * ); 
unsigned char * __RPC_USER  HDC_UserUnmarshal(unsigned long *, unsigned char *, HDC * ); 
void                      __RPC_USER  HDC_UserFree(     unsigned long *, HDC * ); 

unsigned long             __RPC_USER  HICON_UserSize(     unsigned long *, unsigned long            , HICON * ); 
unsigned char * __RPC_USER  HICON_UserMarshal(  unsigned long *, unsigned char *, HICON * ); 
unsigned char * __RPC_USER  HICON_UserUnmarshal(unsigned long *, unsigned char *, HICON * ); 
void                      __RPC_USER  HICON_UserFree(     unsigned long *, HICON * ); 

unsigned long             __RPC_USER  HWND_UserSize(     unsigned long *, unsigned long            , HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  unsigned long *, unsigned char *, HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * ); 
void                      __RPC_USER  HWND_UserFree(     unsigned long *, HWND * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

unsigned long             __RPC_USER  STGMEDIUM_UserSize(     unsigned long *, unsigned long            , STGMEDIUM * ); 
unsigned char * __RPC_USER  STGMEDIUM_UserMarshal(  unsigned long *, unsigned char *, STGMEDIUM * ); 
unsigned char * __RPC_USER  STGMEDIUM_UserUnmarshal(unsigned long *, unsigned char *, STGMEDIUM * ); 
void                      __RPC_USER  STGMEDIUM_UserFree(     unsigned long *, STGMEDIUM * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


