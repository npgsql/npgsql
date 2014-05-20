

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* @@MIDL_FILE_HEADING(  ) */

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

#ifndef __vsshell110_h__
#define __vsshell110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsPropertyBag_FWD_DEFINED__
#define __IVsPropertyBag_FWD_DEFINED__
typedef interface IVsPropertyBag IVsPropertyBag;

#endif 	/* __IVsPropertyBag_FWD_DEFINED__ */


#ifndef __IVsProjectFaultResolver_FWD_DEFINED__
#define __IVsProjectFaultResolver_FWD_DEFINED__
typedef interface IVsProjectFaultResolver IVsProjectFaultResolver;

#endif 	/* __IVsProjectFaultResolver_FWD_DEFINED__ */


#ifndef __IVsPackageExtensionProvider_FWD_DEFINED__
#define __IVsPackageExtensionProvider_FWD_DEFINED__
typedef interface IVsPackageExtensionProvider IVsPackageExtensionProvider;

#endif 	/* __IVsPackageExtensionProvider_FWD_DEFINED__ */


#ifndef __IVsShell5_FWD_DEFINED__
#define __IVsShell5_FWD_DEFINED__
typedef interface IVsShell5 IVsShell5;

#endif 	/* __IVsShell5_FWD_DEFINED__ */


#ifndef __IVsDiagnosticsItem_FWD_DEFINED__
#define __IVsDiagnosticsItem_FWD_DEFINED__
typedef interface IVsDiagnosticsItem IVsDiagnosticsItem;

#endif 	/* __IVsDiagnosticsItem_FWD_DEFINED__ */


#ifndef __IVsDiagnosticsProvider_FWD_DEFINED__
#define __IVsDiagnosticsProvider_FWD_DEFINED__
typedef interface IVsDiagnosticsProvider IVsDiagnosticsProvider;

#endif 	/* __IVsDiagnosticsProvider_FWD_DEFINED__ */


#ifndef __IVsDataObjectStringMapManager2_FWD_DEFINED__
#define __IVsDataObjectStringMapManager2_FWD_DEFINED__
typedef interface IVsDataObjectStringMapManager2 IVsDataObjectStringMapManager2;

#endif 	/* __IVsDataObjectStringMapManager2_FWD_DEFINED__ */


#ifndef __IVsToolbox6_FWD_DEFINED__
#define __IVsToolbox6_FWD_DEFINED__
typedef interface IVsToolbox6 IVsToolbox6;

#endif 	/* __IVsToolbox6_FWD_DEFINED__ */


#ifndef __IVsToolWindowToolbarHost3_FWD_DEFINED__
#define __IVsToolWindowToolbarHost3_FWD_DEFINED__
typedef interface IVsToolWindowToolbarHost3 IVsToolWindowToolbarHost3;

#endif 	/* __IVsToolWindowToolbarHost3_FWD_DEFINED__ */


#ifndef __IVsSearchQueryParser_FWD_DEFINED__
#define __IVsSearchQueryParser_FWD_DEFINED__
typedef interface IVsSearchQueryParser IVsSearchQueryParser;

#endif 	/* __IVsSearchQueryParser_FWD_DEFINED__ */


#ifndef __IVsSearchQuery_FWD_DEFINED__
#define __IVsSearchQuery_FWD_DEFINED__
typedef interface IVsSearchQuery IVsSearchQuery;

#endif 	/* __IVsSearchQuery_FWD_DEFINED__ */


#ifndef __IVsSearchToken_FWD_DEFINED__
#define __IVsSearchToken_FWD_DEFINED__
typedef interface IVsSearchToken IVsSearchToken;

#endif 	/* __IVsSearchToken_FWD_DEFINED__ */


#ifndef __IVsSearchFilterToken_FWD_DEFINED__
#define __IVsSearchFilterToken_FWD_DEFINED__
typedef interface IVsSearchFilterToken IVsSearchFilterToken;

#endif 	/* __IVsSearchFilterToken_FWD_DEFINED__ */


#ifndef __IVsWindowSearchHostFactory_FWD_DEFINED__
#define __IVsWindowSearchHostFactory_FWD_DEFINED__
typedef interface IVsWindowSearchHostFactory IVsWindowSearchHostFactory;

#endif 	/* __IVsWindowSearchHostFactory_FWD_DEFINED__ */


#ifndef __SVsWindowSearchHostFactory_FWD_DEFINED__
#define __SVsWindowSearchHostFactory_FWD_DEFINED__
typedef interface SVsWindowSearchHostFactory SVsWindowSearchHostFactory;

#endif 	/* __SVsWindowSearchHostFactory_FWD_DEFINED__ */


#ifndef __IVsWindowSearchHost_FWD_DEFINED__
#define __IVsWindowSearchHost_FWD_DEFINED__
typedef interface IVsWindowSearchHost IVsWindowSearchHost;

#endif 	/* __IVsWindowSearchHost_FWD_DEFINED__ */


#ifndef __IVsWindowSearch_FWD_DEFINED__
#define __IVsWindowSearch_FWD_DEFINED__
typedef interface IVsWindowSearch IVsWindowSearch;

#endif 	/* __IVsWindowSearch_FWD_DEFINED__ */


#ifndef __IVsWindowSearchEvents_FWD_DEFINED__
#define __IVsWindowSearchEvents_FWD_DEFINED__
typedef interface IVsWindowSearchEvents IVsWindowSearchEvents;

#endif 	/* __IVsWindowSearchEvents_FWD_DEFINED__ */


#ifndef __IVsSearchTask_FWD_DEFINED__
#define __IVsSearchTask_FWD_DEFINED__
typedef interface IVsSearchTask IVsSearchTask;

#endif 	/* __IVsSearchTask_FWD_DEFINED__ */


#ifndef __IVsSearchCallback_FWD_DEFINED__
#define __IVsSearchCallback_FWD_DEFINED__
typedef interface IVsSearchCallback IVsSearchCallback;

#endif 	/* __IVsSearchCallback_FWD_DEFINED__ */


#ifndef __IVsWindowSearchFilter_FWD_DEFINED__
#define __IVsWindowSearchFilter_FWD_DEFINED__
typedef interface IVsWindowSearchFilter IVsWindowSearchFilter;

#endif 	/* __IVsWindowSearchFilter_FWD_DEFINED__ */


#ifndef __IVsWindowSearchSimpleFilter_FWD_DEFINED__
#define __IVsWindowSearchSimpleFilter_FWD_DEFINED__
typedef interface IVsWindowSearchSimpleFilter IVsWindowSearchSimpleFilter;

#endif 	/* __IVsWindowSearchSimpleFilter_FWD_DEFINED__ */


#ifndef __IVsWindowSearchCustomFilter_FWD_DEFINED__
#define __IVsWindowSearchCustomFilter_FWD_DEFINED__
typedef interface IVsWindowSearchCustomFilter IVsWindowSearchCustomFilter;

#endif 	/* __IVsWindowSearchCustomFilter_FWD_DEFINED__ */


#ifndef __IVsEnumWindowSearchFilters_FWD_DEFINED__
#define __IVsEnumWindowSearchFilters_FWD_DEFINED__
typedef interface IVsEnumWindowSearchFilters IVsEnumWindowSearchFilters;

#endif 	/* __IVsEnumWindowSearchFilters_FWD_DEFINED__ */


#ifndef __IVsWindowSearchOption_FWD_DEFINED__
#define __IVsWindowSearchOption_FWD_DEFINED__
typedef interface IVsWindowSearchOption IVsWindowSearchOption;

#endif 	/* __IVsWindowSearchOption_FWD_DEFINED__ */


#ifndef __IVsEnumWindowSearchOptions_FWD_DEFINED__
#define __IVsEnumWindowSearchOptions_FWD_DEFINED__
typedef interface IVsEnumWindowSearchOptions IVsEnumWindowSearchOptions;

#endif 	/* __IVsEnumWindowSearchOptions_FWD_DEFINED__ */


#ifndef __IVsWindowSearchBooleanOption_FWD_DEFINED__
#define __IVsWindowSearchBooleanOption_FWD_DEFINED__
typedef interface IVsWindowSearchBooleanOption IVsWindowSearchBooleanOption;

#endif 	/* __IVsWindowSearchBooleanOption_FWD_DEFINED__ */


#ifndef __IVsWindowSearchCommandOption_FWD_DEFINED__
#define __IVsWindowSearchCommandOption_FWD_DEFINED__
typedef interface IVsWindowSearchCommandOption IVsWindowSearchCommandOption;

#endif 	/* __IVsWindowSearchCommandOption_FWD_DEFINED__ */


#ifndef __IVsEnumSearchProviders_FWD_DEFINED__
#define __IVsEnumSearchProviders_FWD_DEFINED__
typedef interface IVsEnumSearchProviders IVsEnumSearchProviders;

#endif 	/* __IVsEnumSearchProviders_FWD_DEFINED__ */


#ifndef __IVsSearchProvider_FWD_DEFINED__
#define __IVsSearchProvider_FWD_DEFINED__
typedef interface IVsSearchProvider IVsSearchProvider;

#endif 	/* __IVsSearchProvider_FWD_DEFINED__ */


#ifndef __IVsMRESearchProvider_FWD_DEFINED__
#define __IVsMRESearchProvider_FWD_DEFINED__
typedef interface IVsMRESearchProvider IVsMRESearchProvider;

#endif 	/* __IVsMRESearchProvider_FWD_DEFINED__ */


#ifndef __IVsSearchProviderCallback_FWD_DEFINED__
#define __IVsSearchProviderCallback_FWD_DEFINED__
typedef interface IVsSearchProviderCallback IVsSearchProviderCallback;

#endif 	/* __IVsSearchProviderCallback_FWD_DEFINED__ */


#ifndef __IVsSearchItemResult_FWD_DEFINED__
#define __IVsSearchItemResult_FWD_DEFINED__
typedef interface IVsSearchItemResult IVsSearchItemResult;

#endif 	/* __IVsSearchItemResult_FWD_DEFINED__ */


#ifndef __IVsSearchItemDynamicResult_FWD_DEFINED__
#define __IVsSearchItemDynamicResult_FWD_DEFINED__
typedef interface IVsSearchItemDynamicResult IVsSearchItemDynamicResult;

#endif 	/* __IVsSearchItemDynamicResult_FWD_DEFINED__ */


#ifndef __IVsGlobalSearchTask_FWD_DEFINED__
#define __IVsGlobalSearchTask_FWD_DEFINED__
typedef interface IVsGlobalSearchTask IVsGlobalSearchTask;

#endif 	/* __IVsGlobalSearchTask_FWD_DEFINED__ */


#ifndef __IVsGlobalSearch_FWD_DEFINED__
#define __IVsGlobalSearch_FWD_DEFINED__
typedef interface IVsGlobalSearch IVsGlobalSearch;

#endif 	/* __IVsGlobalSearch_FWD_DEFINED__ */


#ifndef __IVsGlobalSearchCallback_FWD_DEFINED__
#define __IVsGlobalSearchCallback_FWD_DEFINED__
typedef interface IVsGlobalSearchCallback IVsGlobalSearchCallback;

#endif 	/* __IVsGlobalSearchCallback_FWD_DEFINED__ */


#ifndef __IVsGlobalSearchUIResultsCategory_FWD_DEFINED__
#define __IVsGlobalSearchUIResultsCategory_FWD_DEFINED__
typedef interface IVsGlobalSearchUIResultsCategory IVsGlobalSearchUIResultsCategory;

#endif 	/* __IVsGlobalSearchUIResultsCategory_FWD_DEFINED__ */


#ifndef __IVsGlobalSearchUI_FWD_DEFINED__
#define __IVsGlobalSearchUI_FWD_DEFINED__
typedef interface IVsGlobalSearchUI IVsGlobalSearchUI;

#endif 	/* __IVsGlobalSearchUI_FWD_DEFINED__ */


#ifndef __SVsGlobalSearch_FWD_DEFINED__
#define __SVsGlobalSearch_FWD_DEFINED__
typedef interface SVsGlobalSearch SVsGlobalSearch;

#endif 	/* __SVsGlobalSearch_FWD_DEFINED__ */


#ifndef __IVsProfilerTargetInfo_FWD_DEFINED__
#define __IVsProfilerTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerTargetInfo IVsProfilerTargetInfo;

#endif 	/* __IVsProfilerTargetInfo_FWD_DEFINED__ */


#ifndef __IVsProfilerLaunchTargetInfo_FWD_DEFINED__
#define __IVsProfilerLaunchTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerLaunchTargetInfo IVsProfilerLaunchTargetInfo;

#endif 	/* __IVsProfilerLaunchTargetInfo_FWD_DEFINED__ */


#ifndef __IVsProfilerLaunchExeTargetInfo_FWD_DEFINED__
#define __IVsProfilerLaunchExeTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerLaunchExeTargetInfo IVsProfilerLaunchExeTargetInfo;

#endif 	/* __IVsProfilerLaunchExeTargetInfo_FWD_DEFINED__ */


#ifndef __IVsProfilerLaunchBrowserTargetInfo_FWD_DEFINED__
#define __IVsProfilerLaunchBrowserTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerLaunchBrowserTargetInfo IVsProfilerLaunchBrowserTargetInfo;

#endif 	/* __IVsProfilerLaunchBrowserTargetInfo_FWD_DEFINED__ */


#ifndef __IVsProfilerLaunchWebServerTargetInfo_FWD_DEFINED__
#define __IVsProfilerLaunchWebServerTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerLaunchWebServerTargetInfo IVsProfilerLaunchWebServerTargetInfo;

#endif 	/* __IVsProfilerLaunchWebServerTargetInfo_FWD_DEFINED__ */


#ifndef __IVsProfilerAttachTargetInfo_FWD_DEFINED__
#define __IVsProfilerAttachTargetInfo_FWD_DEFINED__
typedef interface IVsProfilerAttachTargetInfo IVsProfilerAttachTargetInfo;

#endif 	/* __IVsProfilerAttachTargetInfo_FWD_DEFINED__ */


#ifndef __IEnumVsProfilerTargetInfos_FWD_DEFINED__
#define __IEnumVsProfilerTargetInfos_FWD_DEFINED__
typedef interface IEnumVsProfilerTargetInfos IEnumVsProfilerTargetInfos;

#endif 	/* __IEnumVsProfilerTargetInfos_FWD_DEFINED__ */


#ifndef __IVsProfilableProjectCfg_FWD_DEFINED__
#define __IVsProfilableProjectCfg_FWD_DEFINED__
typedef interface IVsProfilableProjectCfg IVsProfilableProjectCfg;

#endif 	/* __IVsProfilableProjectCfg_FWD_DEFINED__ */


#ifndef __IVsProfilerLauncher_FWD_DEFINED__
#define __IVsProfilerLauncher_FWD_DEFINED__
typedef interface IVsProfilerLauncher IVsProfilerLauncher;

#endif 	/* __IVsProfilerLauncher_FWD_DEFINED__ */


#ifndef __SVsProfilerLauncher_FWD_DEFINED__
#define __SVsProfilerLauncher_FWD_DEFINED__
typedef interface SVsProfilerLauncher SVsProfilerLauncher;

#endif 	/* __SVsProfilerLauncher_FWD_DEFINED__ */


#ifndef __IVsMRUItemsStore_FWD_DEFINED__
#define __IVsMRUItemsStore_FWD_DEFINED__
typedef interface IVsMRUItemsStore IVsMRUItemsStore;

#endif 	/* __IVsMRUItemsStore_FWD_DEFINED__ */


#ifndef __SVsMRUItemsStore_FWD_DEFINED__
#define __SVsMRUItemsStore_FWD_DEFINED__
typedef interface SVsMRUItemsStore SVsMRUItemsStore;

#endif 	/* __SVsMRUItemsStore_FWD_DEFINED__ */


#ifndef __IVsHierarchyDirectionalDropDataTarget_FWD_DEFINED__
#define __IVsHierarchyDirectionalDropDataTarget_FWD_DEFINED__
typedef interface IVsHierarchyDirectionalDropDataTarget IVsHierarchyDirectionalDropDataTarget;

#endif 	/* __IVsHierarchyDirectionalDropDataTarget_FWD_DEFINED__ */


#ifndef __IVsDocumentPreviewer_FWD_DEFINED__
#define __IVsDocumentPreviewer_FWD_DEFINED__
typedef interface IVsDocumentPreviewer IVsDocumentPreviewer;

#endif 	/* __IVsDocumentPreviewer_FWD_DEFINED__ */


#ifndef __IVsEnumDocumentPreviewers_FWD_DEFINED__
#define __IVsEnumDocumentPreviewers_FWD_DEFINED__
typedef interface IVsEnumDocumentPreviewers IVsEnumDocumentPreviewers;

#endif 	/* __IVsEnumDocumentPreviewers_FWD_DEFINED__ */


#ifndef __IVsUIShellOpenDocument3_FWD_DEFINED__
#define __IVsUIShellOpenDocument3_FWD_DEFINED__
typedef interface IVsUIShellOpenDocument3 IVsUIShellOpenDocument3;

#endif 	/* __IVsUIShellOpenDocument3_FWD_DEFINED__ */


#ifndef __IVsNewDocumentStateContext_FWD_DEFINED__
#define __IVsNewDocumentStateContext_FWD_DEFINED__
typedef interface IVsNewDocumentStateContext IVsNewDocumentStateContext;

#endif 	/* __IVsNewDocumentStateContext_FWD_DEFINED__ */


#ifndef __IVsLanguageServiceBuildErrorReporter2_FWD_DEFINED__
#define __IVsLanguageServiceBuildErrorReporter2_FWD_DEFINED__
typedef interface IVsLanguageServiceBuildErrorReporter2 IVsLanguageServiceBuildErrorReporter2;

#endif 	/* __IVsLanguageServiceBuildErrorReporter2_FWD_DEFINED__ */


#ifndef __IVsRunningDocumentTable3_FWD_DEFINED__
#define __IVsRunningDocumentTable3_FWD_DEFINED__
typedef interface IVsRunningDocumentTable3 IVsRunningDocumentTable3;

#endif 	/* __IVsRunningDocumentTable3_FWD_DEFINED__ */


#ifndef __IVsRunningDocTableEvents5_FWD_DEFINED__
#define __IVsRunningDocTableEvents5_FWD_DEFINED__
typedef interface IVsRunningDocTableEvents5 IVsRunningDocTableEvents5;

#endif 	/* __IVsRunningDocTableEvents5_FWD_DEFINED__ */


#ifndef __SVsReferenceManager_FWD_DEFINED__
#define __SVsReferenceManager_FWD_DEFINED__
typedef interface SVsReferenceManager SVsReferenceManager;

#endif 	/* __SVsReferenceManager_FWD_DEFINED__ */


#ifndef __IVsReference_FWD_DEFINED__
#define __IVsReference_FWD_DEFINED__
typedef interface IVsReference IVsReference;

#endif 	/* __IVsReference_FWD_DEFINED__ */


#ifndef __IVsReferenceProviderContext_FWD_DEFINED__
#define __IVsReferenceProviderContext_FWD_DEFINED__
typedef interface IVsReferenceProviderContext IVsReferenceProviderContext;

#endif 	/* __IVsReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsAssemblyReference_FWD_DEFINED__
#define __IVsAssemblyReference_FWD_DEFINED__
typedef interface IVsAssemblyReference IVsAssemblyReference;

#endif 	/* __IVsAssemblyReference_FWD_DEFINED__ */


#ifndef __IVsAssemblyReferenceProviderContext_FWD_DEFINED__
#define __IVsAssemblyReferenceProviderContext_FWD_DEFINED__
typedef interface IVsAssemblyReferenceProviderContext IVsAssemblyReferenceProviderContext;

#endif 	/* __IVsAssemblyReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsProjectReference_FWD_DEFINED__
#define __IVsProjectReference_FWD_DEFINED__
typedef interface IVsProjectReference IVsProjectReference;

#endif 	/* __IVsProjectReference_FWD_DEFINED__ */


#ifndef __IVsProjectReferenceProviderContext_FWD_DEFINED__
#define __IVsProjectReferenceProviderContext_FWD_DEFINED__
typedef interface IVsProjectReferenceProviderContext IVsProjectReferenceProviderContext;

#endif 	/* __IVsProjectReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsComReference_FWD_DEFINED__
#define __IVsComReference_FWD_DEFINED__
typedef interface IVsComReference IVsComReference;

#endif 	/* __IVsComReference_FWD_DEFINED__ */


#ifndef __IVsComReferenceProviderContext_FWD_DEFINED__
#define __IVsComReferenceProviderContext_FWD_DEFINED__
typedef interface IVsComReferenceProviderContext IVsComReferenceProviderContext;

#endif 	/* __IVsComReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsPlatformReference_FWD_DEFINED__
#define __IVsPlatformReference_FWD_DEFINED__
typedef interface IVsPlatformReference IVsPlatformReference;

#endif 	/* __IVsPlatformReference_FWD_DEFINED__ */


#ifndef __IVsPlatformReferenceProviderContext_FWD_DEFINED__
#define __IVsPlatformReferenceProviderContext_FWD_DEFINED__
typedef interface IVsPlatformReferenceProviderContext IVsPlatformReferenceProviderContext;

#endif 	/* __IVsPlatformReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsFileReference_FWD_DEFINED__
#define __IVsFileReference_FWD_DEFINED__
typedef interface IVsFileReference IVsFileReference;

#endif 	/* __IVsFileReference_FWD_DEFINED__ */


#ifndef __IVsFileReferenceProviderContext_FWD_DEFINED__
#define __IVsFileReferenceProviderContext_FWD_DEFINED__
typedef interface IVsFileReferenceProviderContext IVsFileReferenceProviderContext;

#endif 	/* __IVsFileReferenceProviderContext_FWD_DEFINED__ */


#ifndef __IVsReferenceManagerUser_FWD_DEFINED__
#define __IVsReferenceManagerUser_FWD_DEFINED__
typedef interface IVsReferenceManagerUser IVsReferenceManagerUser;

#endif 	/* __IVsReferenceManagerUser_FWD_DEFINED__ */


#ifndef __IVsReferenceManager_FWD_DEFINED__
#define __IVsReferenceManager_FWD_DEFINED__
typedef interface IVsReferenceManager IVsReferenceManager;

#endif 	/* __IVsReferenceManager_FWD_DEFINED__ */


#ifndef __IVsReferenceManager2_FWD_DEFINED__
#define __IVsReferenceManager2_FWD_DEFINED__
typedef interface IVsReferenceManager2 IVsReferenceManager2;

#endif 	/* __IVsReferenceManager2_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorReferenceManager_FWD_DEFINED__
#define __IVsProjectFlavorReferenceManager_FWD_DEFINED__
typedef interface IVsProjectFlavorReferenceManager IVsProjectFlavorReferenceManager;

#endif 	/* __IVsProjectFlavorReferenceManager_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorReferences3_FWD_DEFINED__
#define __IVsProjectFlavorReferences3_FWD_DEFINED__
typedef interface IVsProjectFlavorReferences3 IVsProjectFlavorReferences3;

#endif 	/* __IVsProjectFlavorReferences3_FWD_DEFINED__ */


#ifndef __IVsAppCompat_FWD_DEFINED__
#define __IVsAppCompat_FWD_DEFINED__
typedef interface IVsAppCompat IVsAppCompat;

#endif 	/* __IVsAppCompat_FWD_DEFINED__ */


#ifndef __IVsDesignTimeAssemblyResolution2_FWD_DEFINED__
#define __IVsDesignTimeAssemblyResolution2_FWD_DEFINED__
typedef interface IVsDesignTimeAssemblyResolution2 IVsDesignTimeAssemblyResolution2;

#endif 	/* __IVsDesignTimeAssemblyResolution2_FWD_DEFINED__ */


#ifndef __IVsBuildManagerAccessor2_FWD_DEFINED__
#define __IVsBuildManagerAccessor2_FWD_DEFINED__
typedef interface IVsBuildManagerAccessor2 IVsBuildManagerAccessor2;

#endif 	/* __IVsBuildManagerAccessor2_FWD_DEFINED__ */


#ifndef __IVsBackForwardNavigation2_FWD_DEFINED__
#define __IVsBackForwardNavigation2_FWD_DEFINED__
typedef interface IVsBackForwardNavigation2 IVsBackForwardNavigation2;

#endif 	/* __IVsBackForwardNavigation2_FWD_DEFINED__ */


#ifndef __IVsSerializeNavigationItem_FWD_DEFINED__
#define __IVsSerializeNavigationItem_FWD_DEFINED__
typedef interface IVsSerializeNavigationItem IVsSerializeNavigationItem;

#endif 	/* __IVsSerializeNavigationItem_FWD_DEFINED__ */


#ifndef __IVsDynamicNavigationItem_FWD_DEFINED__
#define __IVsDynamicNavigationItem_FWD_DEFINED__
typedef interface IVsDynamicNavigationItem IVsDynamicNavigationItem;

#endif 	/* __IVsDynamicNavigationItem_FWD_DEFINED__ */


#ifndef __IVsProvisionalItem_FWD_DEFINED__
#define __IVsProvisionalItem_FWD_DEFINED__
typedef interface IVsProvisionalItem IVsProvisionalItem;

#endif 	/* __IVsProvisionalItem_FWD_DEFINED__ */


#ifndef __IVsStatusbarUser2_FWD_DEFINED__
#define __IVsStatusbarUser2_FWD_DEFINED__
typedef interface IVsStatusbarUser2 IVsStatusbarUser2;

#endif 	/* __IVsStatusbarUser2_FWD_DEFINED__ */


#ifndef __IVsDebugger4_FWD_DEFINED__
#define __IVsDebugger4_FWD_DEFINED__
typedef interface IVsDebugger4 IVsDebugger4;

#endif 	/* __IVsDebugger4_FWD_DEFINED__ */


#ifndef __IVsQueryDebuggableProjectCfg2_FWD_DEFINED__
#define __IVsQueryDebuggableProjectCfg2_FWD_DEFINED__
typedef interface IVsQueryDebuggableProjectCfg2 IVsQueryDebuggableProjectCfg2;

#endif 	/* __IVsQueryDebuggableProjectCfg2_FWD_DEFINED__ */


#ifndef __IVsXMLMemberDataCapability_FWD_DEFINED__
#define __IVsXMLMemberDataCapability_FWD_DEFINED__
typedef interface IVsXMLMemberDataCapability IVsXMLMemberDataCapability;

#endif 	/* __IVsXMLMemberDataCapability_FWD_DEFINED__ */


#ifndef __IVsXMLMemberData4_FWD_DEFINED__
#define __IVsXMLMemberData4_FWD_DEFINED__
typedef interface IVsXMLMemberData4 IVsXMLMemberData4;

#endif 	/* __IVsXMLMemberData4_FWD_DEFINED__ */


#ifndef __IVsHierarchyDeleteHandler3_FWD_DEFINED__
#define __IVsHierarchyDeleteHandler3_FWD_DEFINED__
typedef interface IVsHierarchyDeleteHandler3 IVsHierarchyDeleteHandler3;

#endif 	/* __IVsHierarchyDeleteHandler3_FWD_DEFINED__ */


#ifndef __IVsBooleanSymbolExpressionEvaluator_FWD_DEFINED__
#define __IVsBooleanSymbolExpressionEvaluator_FWD_DEFINED__
typedef interface IVsBooleanSymbolExpressionEvaluator IVsBooleanSymbolExpressionEvaluator;

#endif 	/* __IVsBooleanSymbolExpressionEvaluator_FWD_DEFINED__ */


#ifndef __VsProjectCapabilityExpressionMatcher_FWD_DEFINED__
#define __VsProjectCapabilityExpressionMatcher_FWD_DEFINED__
typedef interface VsProjectCapabilityExpressionMatcher VsProjectCapabilityExpressionMatcher;

#endif 	/* __VsProjectCapabilityExpressionMatcher_FWD_DEFINED__ */


#ifndef __IVsSccProjectEvents_FWD_DEFINED__
#define __IVsSccProjectEvents_FWD_DEFINED__
typedef interface IVsSccProjectEvents IVsSccProjectEvents;

#endif 	/* __IVsSccProjectEvents_FWD_DEFINED__ */


#ifndef __IVsSccManager3_FWD_DEFINED__
#define __IVsSccManager3_FWD_DEFINED__
typedef interface IVsSccManager3 IVsSccManager3;

#endif 	/* __IVsSccManager3_FWD_DEFINED__ */


#ifndef __IVsSccTrackProjectEvents_FWD_DEFINED__
#define __IVsSccTrackProjectEvents_FWD_DEFINED__
typedef interface IVsSccTrackProjectEvents IVsSccTrackProjectEvents;

#endif 	/* __IVsSccTrackProjectEvents_FWD_DEFINED__ */


#ifndef __IVsProjectBuildMessageEvents_FWD_DEFINED__
#define __IVsProjectBuildMessageEvents_FWD_DEFINED__
typedef interface IVsProjectBuildMessageEvents IVsProjectBuildMessageEvents;

#endif 	/* __IVsProjectBuildMessageEvents_FWD_DEFINED__ */


#ifndef __IVsProjectBuildMessageReporter_FWD_DEFINED__
#define __IVsProjectBuildMessageReporter_FWD_DEFINED__
typedef interface IVsProjectBuildMessageReporter IVsProjectBuildMessageReporter;

#endif 	/* __IVsProjectBuildMessageReporter_FWD_DEFINED__ */


#ifndef __IVsNavInfo2_FWD_DEFINED__
#define __IVsNavInfo2_FWD_DEFINED__
typedef interface IVsNavInfo2 IVsNavInfo2;

#endif 	/* __IVsNavInfo2_FWD_DEFINED__ */


#ifndef __IVsLibrary3_FWD_DEFINED__
#define __IVsLibrary3_FWD_DEFINED__
typedef interface IVsLibrary3 IVsLibrary3;

#endif 	/* __IVsLibrary3_FWD_DEFINED__ */


#ifndef __SVsDifferenceService_FWD_DEFINED__
#define __SVsDifferenceService_FWD_DEFINED__
typedef interface SVsDifferenceService SVsDifferenceService;

#endif 	/* __SVsDifferenceService_FWD_DEFINED__ */


#ifndef __IVsDifferenceService_FWD_DEFINED__
#define __IVsDifferenceService_FWD_DEFINED__
typedef interface IVsDifferenceService IVsDifferenceService;

#endif 	/* __IVsDifferenceService_FWD_DEFINED__ */


#ifndef __SVsFileMergeService_FWD_DEFINED__
#define __SVsFileMergeService_FWD_DEFINED__
typedef interface SVsFileMergeService SVsFileMergeService;

#endif 	/* __SVsFileMergeService_FWD_DEFINED__ */


#ifndef __IVsFileMergeService_FWD_DEFINED__
#define __IVsFileMergeService_FWD_DEFINED__
typedef interface IVsFileMergeService IVsFileMergeService;

#endif 	/* __IVsFileMergeService_FWD_DEFINED__ */


#ifndef __IVsUpdateSolutionEvents4_FWD_DEFINED__
#define __IVsUpdateSolutionEvents4_FWD_DEFINED__
typedef interface IVsUpdateSolutionEvents4 IVsUpdateSolutionEvents4;

#endif 	/* __IVsUpdateSolutionEvents4_FWD_DEFINED__ */


#ifndef __IVsFireUpdateSolutionEvents_FWD_DEFINED__
#define __IVsFireUpdateSolutionEvents_FWD_DEFINED__
typedef interface IVsFireUpdateSolutionEvents IVsFireUpdateSolutionEvents;

#endif 	/* __IVsFireUpdateSolutionEvents_FWD_DEFINED__ */


#ifndef __IVsUpdateSolutionEventsAsyncCallback_FWD_DEFINED__
#define __IVsUpdateSolutionEventsAsyncCallback_FWD_DEFINED__
typedef interface IVsUpdateSolutionEventsAsyncCallback IVsUpdateSolutionEventsAsyncCallback;

#endif 	/* __IVsUpdateSolutionEventsAsyncCallback_FWD_DEFINED__ */


#ifndef __IVsUpdateSolutionEventsAsync_FWD_DEFINED__
#define __IVsUpdateSolutionEventsAsync_FWD_DEFINED__
typedef interface IVsUpdateSolutionEventsAsync IVsUpdateSolutionEventsAsync;

#endif 	/* __IVsUpdateSolutionEventsAsync_FWD_DEFINED__ */


#ifndef __IVsSolutionBuildManager5_FWD_DEFINED__
#define __IVsSolutionBuildManager5_FWD_DEFINED__
typedef interface IVsSolutionBuildManager5 IVsSolutionBuildManager5;

#endif 	/* __IVsSolutionBuildManager5_FWD_DEFINED__ */


#ifndef __IVsLaunchPad4_FWD_DEFINED__
#define __IVsLaunchPad4_FWD_DEFINED__
typedef interface IVsLaunchPad4 IVsLaunchPad4;

#endif 	/* __IVsLaunchPad4_FWD_DEFINED__ */


#ifndef __IVsEnumGuids_FWD_DEFINED__
#define __IVsEnumGuids_FWD_DEFINED__
typedef interface IVsEnumGuids IVsEnumGuids;

#endif 	/* __IVsEnumGuids_FWD_DEFINED__ */


#ifndef __IVsUIShell5_FWD_DEFINED__
#define __IVsUIShell5_FWD_DEFINED__
typedef interface IVsUIShell5 IVsUIShell5;

#endif 	/* __IVsUIShell5_FWD_DEFINED__ */


#ifndef __IVsDebugRemoteDiscoveryUI_FWD_DEFINED__
#define __IVsDebugRemoteDiscoveryUI_FWD_DEFINED__
typedef interface IVsDebugRemoteDiscoveryUI IVsDebugRemoteDiscoveryUI;

#endif 	/* __IVsDebugRemoteDiscoveryUI_FWD_DEFINED__ */


#ifndef __SVsDebugRemoteDiscoveryUI_FWD_DEFINED__
#define __SVsDebugRemoteDiscoveryUI_FWD_DEFINED__
typedef interface SVsDebugRemoteDiscoveryUI SVsDebugRemoteDiscoveryUI;

#endif 	/* __SVsDebugRemoteDiscoveryUI_FWD_DEFINED__ */


#ifndef __IVsTaskBody_FWD_DEFINED__
#define __IVsTaskBody_FWD_DEFINED__
typedef interface IVsTaskBody IVsTaskBody;

#endif 	/* __IVsTaskBody_FWD_DEFINED__ */


#ifndef __IVsTask_FWD_DEFINED__
#define __IVsTask_FWD_DEFINED__
typedef interface IVsTask IVsTask;

#endif 	/* __IVsTask_FWD_DEFINED__ */


#ifndef __IVsTaskCompletionSource_FWD_DEFINED__
#define __IVsTaskCompletionSource_FWD_DEFINED__
typedef interface IVsTaskCompletionSource IVsTaskCompletionSource;

#endif 	/* __IVsTaskCompletionSource_FWD_DEFINED__ */


#ifndef __IVsTaskSchedulerService_FWD_DEFINED__
#define __IVsTaskSchedulerService_FWD_DEFINED__
typedef interface IVsTaskSchedulerService IVsTaskSchedulerService;

#endif 	/* __IVsTaskSchedulerService_FWD_DEFINED__ */


#ifndef __SVsTaskSchedulerService_FWD_DEFINED__
#define __SVsTaskSchedulerService_FWD_DEFINED__
typedef interface SVsTaskSchedulerService SVsTaskSchedulerService;

#endif 	/* __SVsTaskSchedulerService_FWD_DEFINED__ */


#ifndef __IVsImageService_FWD_DEFINED__
#define __IVsImageService_FWD_DEFINED__
typedef interface IVsImageService IVsImageService;

#endif 	/* __IVsImageService_FWD_DEFINED__ */


#ifndef __SVsImageService_FWD_DEFINED__
#define __SVsImageService_FWD_DEFINED__
typedef interface SVsImageService SVsImageService;

#endif 	/* __SVsImageService_FWD_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory4_FWD_DEFINED__
#define __IVsProjectUpgradeViaFactory4_FWD_DEFINED__
typedef interface IVsProjectUpgradeViaFactory4 IVsProjectUpgradeViaFactory4;

#endif 	/* __IVsProjectUpgradeViaFactory4_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorUpgradeViaFactory2_FWD_DEFINED__
#define __IVsProjectFlavorUpgradeViaFactory2_FWD_DEFINED__
typedef interface IVsProjectFlavorUpgradeViaFactory2 IVsProjectFlavorUpgradeViaFactory2;

#endif 	/* __IVsProjectFlavorUpgradeViaFactory2_FWD_DEFINED__ */


#ifndef __IVsUpgradeLogger2_FWD_DEFINED__
#define __IVsUpgradeLogger2_FWD_DEFINED__
typedef interface IVsUpgradeLogger2 IVsUpgradeLogger2;

#endif 	/* __IVsUpgradeLogger2_FWD_DEFINED__ */


#ifndef __IVsSolution5_FWD_DEFINED__
#define __IVsSolution5_FWD_DEFINED__
typedef interface IVsSolution5 IVsSolution5;

#endif 	/* __IVsSolution5_FWD_DEFINED__ */


#ifndef __IVsTaskList3_FWD_DEFINED__
#define __IVsTaskList3_FWD_DEFINED__
typedef interface IVsTaskList3 IVsTaskList3;

#endif 	/* __IVsTaskList3_FWD_DEFINED__ */


#ifndef __IVsPersistSolutionOpts2_FWD_DEFINED__
#define __IVsPersistSolutionOpts2_FWD_DEFINED__
typedef interface IVsPersistSolutionOpts2 IVsPersistSolutionOpts2;

#endif 	/* __IVsPersistSolutionOpts2_FWD_DEFINED__ */


#ifndef __IVsAsynchronousProjectCreate_FWD_DEFINED__
#define __IVsAsynchronousProjectCreate_FWD_DEFINED__
typedef interface IVsAsynchronousProjectCreate IVsAsynchronousProjectCreate;

#endif 	/* __IVsAsynchronousProjectCreate_FWD_DEFINED__ */


#ifndef __IVsSolutionEvents5_FWD_DEFINED__
#define __IVsSolutionEvents5_FWD_DEFINED__
typedef interface IVsSolutionEvents5 IVsSolutionEvents5;

#endif 	/* __IVsSolutionEvents5_FWD_DEFINED__ */


#ifndef __IVsHierarchyEvents2_FWD_DEFINED__
#define __IVsHierarchyEvents2_FWD_DEFINED__
typedef interface IVsHierarchyEvents2 IVsHierarchyEvents2;

#endif 	/* __IVsHierarchyEvents2_FWD_DEFINED__ */


#ifndef __IVsAsynchronousProjectCreateUI_FWD_DEFINED__
#define __IVsAsynchronousProjectCreateUI_FWD_DEFINED__
typedef interface IVsAsynchronousProjectCreateUI IVsAsynchronousProjectCreateUI;

#endif 	/* __IVsAsynchronousProjectCreateUI_FWD_DEFINED__ */


#ifndef __IVsSolutionUIEvents_FWD_DEFINED__
#define __IVsSolutionUIEvents_FWD_DEFINED__
typedef interface IVsSolutionUIEvents IVsSolutionUIEvents;

#endif 	/* __IVsSolutionUIEvents_FWD_DEFINED__ */


#ifndef __IVsSolutionUIHierarchyWindow_FWD_DEFINED__
#define __IVsSolutionUIHierarchyWindow_FWD_DEFINED__
typedef interface IVsSolutionUIHierarchyWindow IVsSolutionUIHierarchyWindow;

#endif 	/* __IVsSolutionUIHierarchyWindow_FWD_DEFINED__ */


#ifndef __IVsPrioritizedSolutionEventsSink_FWD_DEFINED__
#define __IVsPrioritizedSolutionEventsSink_FWD_DEFINED__
typedef interface IVsPrioritizedSolutionEventsSink IVsPrioritizedSolutionEventsSink;

#endif 	/* __IVsPrioritizedSolutionEventsSink_FWD_DEFINED__ */


#ifndef __IVsManifestReferenceResolver_FWD_DEFINED__
#define __IVsManifestReferenceResolver_FWD_DEFINED__
typedef interface IVsManifestReferenceResolver IVsManifestReferenceResolver;

#endif 	/* __IVsManifestReferenceResolver_FWD_DEFINED__ */


#ifndef __IVsThreadedWaitDialogCallback_FWD_DEFINED__
#define __IVsThreadedWaitDialogCallback_FWD_DEFINED__
typedef interface IVsThreadedWaitDialogCallback IVsThreadedWaitDialogCallback;

#endif 	/* __IVsThreadedWaitDialogCallback_FWD_DEFINED__ */


#ifndef __IVsThreadedWaitDialog3_FWD_DEFINED__
#define __IVsThreadedWaitDialog3_FWD_DEFINED__
typedef interface IVsThreadedWaitDialog3 IVsThreadedWaitDialog3;

#endif 	/* __IVsThreadedWaitDialog3_FWD_DEFINED__ */


#ifndef __SVsHierarchyManipulation_FWD_DEFINED__
#define __SVsHierarchyManipulation_FWD_DEFINED__
typedef interface SVsHierarchyManipulation SVsHierarchyManipulation;

#endif 	/* __SVsHierarchyManipulation_FWD_DEFINED__ */


#ifndef __IVsHierarchyManipulation_FWD_DEFINED__
#define __IVsHierarchyManipulation_FWD_DEFINED__
typedef interface IVsHierarchyManipulation IVsHierarchyManipulation;

#endif 	/* __IVsHierarchyManipulation_FWD_DEFINED__ */


#ifndef __IVsHierarchyManipulationStateContext_FWD_DEFINED__
#define __IVsHierarchyManipulationStateContext_FWD_DEFINED__
typedef interface IVsHierarchyManipulationStateContext IVsHierarchyManipulationStateContext;

#endif 	/* __IVsHierarchyManipulationStateContext_FWD_DEFINED__ */


#ifndef __IVsFontAndColorStorage3_FWD_DEFINED__
#define __IVsFontAndColorStorage3_FWD_DEFINED__
typedef interface IVsFontAndColorStorage3 IVsFontAndColorStorage3;

#endif 	/* __IVsFontAndColorStorage3_FWD_DEFINED__ */


#ifndef __IVsTaskProvider4_FWD_DEFINED__
#define __IVsTaskProvider4_FWD_DEFINED__
typedef interface IVsTaskProvider4 IVsTaskProvider4;

#endif 	/* __IVsTaskProvider4_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"
#include "vsshell2.h"
#include "vsshell80.h"
#include "vsshell90.h"
#include "vsshell100.h"
#include "IVsSccManager2.h"
#include "textfind.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell110_0000_0000 */
/* [local] */ 

#pragma once





enum __VSPROPID5
    {
        VSPROPID_SolutionFileExt	= -8037,
        VSPROPID_UserOptsFileExt	= -8038,
        VSPROPID_FaultedProjectCount	= -8039,
        VSPROPID_ProjectFaultResolutionContext	= -8040,
        VSPROPID_SolutionViewModel	= -8041,
        VSPROPID_IsOpeningProjectUserInitiated	= -8042,
        VSPROPID_FIRST5	= -8042
    } ;
typedef /* [public] */ DWORD VSPROPID5;


enum __VSSPROPID5
    {
        VSSPROPID_LastActiveInputTick	= -9066,
        VSSPROPID_ProvisionalDelayMilliseconds	= -9067,
        VSSPROPID_ReleaseVersion	= -9068,
        VSSPROPID_ReleaseDescription	= -9069,
        VSSPROPID_EnablePreviewTab	= -9070,
        VSSPROPID_AppBrandName	= -9071,
        VSSPROPID_AppShortBrandName	= -9072,
        VSSPROPID_SKUInfo	= -9073,
        VSSPROPID_NativeScrollbarThemeModePropName	= -9074,
        VSSPROPID_PreviewFileSizeLimit	= -9075,
        VSSPROPID_FIRST5	= -9075
    } ;
typedef LONG VSSPROPID5;


enum __VSFPROPID5
    {
        VSFPROPID_SearchHost	= -5017,
        VSFPROPID_IsSearchEnabled	= -5018,
        VSFPROPID_SearchPlacement	= -5019,
        VSFPROPID_IsProvisional	= -5020,
        VSFPROPID_IsPinned	= -5021,
        VSFPROPID_DontAutoOpen	= -5022,
        VSFPROPID_OverrideCaption	= -5023,
        VSFPROPID_OverrideToolTip	= -5024,
        VSFPROPID_ReplaceDocumentToolbars	= -5025,
        VSFPROPID_NativeScrollbarThemeMode	= -5026,
        VSFPROPID5_FIRST	= -5026
    } ;
typedef LONG VSFPROPID5;


enum __VSNativeScrollbarThemeMode
    {
        NSTM_Undefined	= 0,
        NSTM_All	= 1,
        NSTM_None	= 2,
        NSTM_OnlyTopLevel	= 3
    } ;
typedef DWORD VSNativeScrollbarThemeMode;


enum __HierarchyDropArea
    {
        DROPAREA_On	= 0x1,
        DROPAREA_Above	= 0x2,
        DROPAREA_Below	= 0x4
    } ;
typedef DWORD HierarchyDropArea;


enum __PSFFILEID5
    {
        PSFFILEID_AppxManifest	= -1010,
        PSFFILEID_FIRST5	= -1010
    } ;
typedef LONG PSFFILEID5;

#define	VS_BUILDABLEPROJECTCFGOPTS_PACKAGE	( 8 )


enum __VSHPROPID5
    {
        VSHPROPID_MinimumDesignTimeCompatVersion	= -2110,
        VSHPROPID_ProvisionalViewingStatus	= -2112,
        VSHPROPID_SupportedOutputTypes	= -2113,
        VSHPROPID_TargetPlatformIdentifier	= -2114,
        VSHPROPID_TargetPlatformVersion	= -2115,
        VSHPROPID_TargetRuntime	= -2116,
        VSHPROPID_AppContainer	= -2117,
        VSHPROPID_OutputType	= -2118,
        VSHPROPID_ReferenceManagerUser	= -2119,
        VSHPROPID_ProjectUnloadStatus	= -2120,
        VSHPROPID_DemandLoadDependencies	= -2121,
        VSHPROPID_IsFaulted	= -2122,
        VSHPROPID_FaultMessage	= -2123,
        VSHPROPID_ProjectCapabilities	= -2124,
        VSHPROPID_RequiresReloadForExternalFileChange	= -2125,
        VSHPROPID_ForceFrameworkRetarget	= -2126,
        VSHPROPID_IsProjectProvisioned	= -2127,
        VSHPROPID_SupportsCrossRuntimeReferences	= -2128,
        VSHPROPID_WinMDAssembly	= -2129,
        VSHPROPID_MonikerSameAsPersistFile	= -2130,
        VSHPROPID_IsPackagingProject	= -2131,
        VSHPROPID_ProjectPropertiesDebugPageArg	= -2132,
        VSHPROPID_FIRST5	= -2132
    } ;
typedef /* [public] */ DWORD VSHPROPID5;

DEFINE_GUID(GUID_ProjectDesignerEditorFactory, 0x04b8ab82, 0xa572, 0x4fef, 0x95, 0xce, 0x52, 0x22, 0x44, 0x4b, 0x6b, 0x64);

enum __VSPROJOUTPUTTYPE
    {
        VSPROJ_OUTPUTTYPE_WINEXE	= 0,
        VSPROJ_OUTPUTTYPE_EXE	= 1,
        VSPROJ_OUTPUTTYPE_LIBRARY	= 2,
        VSPROJ_OUTPUTTYPE_WINMDOBJ	= 3,
        VSPROJ_OUTPUTTYPE_APPCONTAINEREXE	= 4,
        VSPROJ_OUTPUTTYPE_NONE	= 100
    } ;
typedef /* [public] */ DWORD VSPROJOUTPUTTYPE;


enum __VSPROJTARGETRUNTIME
    {
        VSPROJ_TARGETRUNTIME_MANAGED	= 0,
        VSPROJ_TARGETRUNTIME_NATIVE	= 1,
        VSPROJ_TARGETRUNTIME_APPHOST	= 2
    } ;
typedef /* [public] */ DWORD VSPROJTARGETRUNTIME;


enum __VSENUMPROJFLAGS2
    {
        EPF_FAULTED	= 0x20,
        EPF_NOTFAULTED	= 0x40,
        EPF_MATCHUNLOADEDTYPE	= 0x80,
        EPF_PROVISIONED	= 0x100
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0000_v0_0_s_ifspec;

#ifndef __IVsPropertyBag_INTERFACE_DEFINED__
#define __IVsPropertyBag_INTERFACE_DEFINED__

/* interface IVsPropertyBag */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_IVsPropertyBag;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("aaeeac4c-3bf3-492c-927d-84ab7d93d6df")
    IVsPropertyBag : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [retval][out] */ __RPC__out VARIANT *pVarValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in VARIANT *pVarValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPropertyBagVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPropertyBag * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPropertyBag * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPropertyBag * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsPropertyBag * This,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [retval][out] */ __RPC__out VARIANT *pVarValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsPropertyBag * This,
            /* [in] */ __RPC__in LPCOLESTR szName,
            /* [in] */ __RPC__in VARIANT *pVarValue);
        
        END_INTERFACE
    } IVsPropertyBagVtbl;

    interface IVsPropertyBag
    {
        CONST_VTBL struct IVsPropertyBagVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPropertyBag_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPropertyBag_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPropertyBag_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPropertyBag_GetValue(This,szName,pVarValue)	\
    ( (This)->lpVtbl -> GetValue(This,szName,pVarValue) ) 

#define IVsPropertyBag_SetValue(This,szName,pVarValue)	\
    ( (This)->lpVtbl -> SetValue(This,szName,pVarValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPropertyBag_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFaultResolver_INTERFACE_DEFINED__
#define __IVsProjectFaultResolver_INTERFACE_DEFINED__

/* interface IVsProjectFaultResolver */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_IVsProjectFaultResolver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c94fb6dd-91aa-495f-b399-51f1428723fc")
    IVsProjectFaultResolver : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolveFault( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfShouldReload) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFaultResolverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFaultResolver * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFaultResolver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFaultResolver * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveFault )( 
            __RPC__in IVsProjectFaultResolver * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfShouldReload);
        
        END_INTERFACE
    } IVsProjectFaultResolverVtbl;

    interface IVsProjectFaultResolver
    {
        CONST_VTBL struct IVsProjectFaultResolverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFaultResolver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFaultResolver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFaultResolver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFaultResolver_ResolveFault(This,pfShouldReload)	\
    ( (This)->lpVtbl -> ResolveFault(This,pfShouldReload) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFaultResolver_INTERFACE_DEFINED__ */


#ifndef __IVsPackageExtensionProvider_INTERFACE_DEFINED__
#define __IVsPackageExtensionProvider_INTERFACE_DEFINED__

/* interface IVsPackageExtensionProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPackageExtensionProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1FC6AF83-7F43-467E-B2C4-28E2B1B376AB")
    IVsPackageExtensionProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateExtensionInstance( 
            /* [in] */ __RPC__in REFCLSID extensionPoint,
            /* [in] */ __RPC__in REFGUID instance,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPackageExtensionProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPackageExtensionProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPackageExtensionProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPackageExtensionProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExtensionInstance )( 
            __RPC__in IVsPackageExtensionProvider * This,
            /* [in] */ __RPC__in REFCLSID extensionPoint,
            /* [in] */ __RPC__in REFGUID instance,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk);
        
        END_INTERFACE
    } IVsPackageExtensionProviderVtbl;

    interface IVsPackageExtensionProvider
    {
        CONST_VTBL struct IVsPackageExtensionProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPackageExtensionProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPackageExtensionProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPackageExtensionProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPackageExtensionProvider_CreateExtensionInstance(This,extensionPoint,instance,ppunk)	\
    ( (This)->lpVtbl -> CreateExtensionInstance(This,extensionPoint,instance,ppunk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPackageExtensionProvider_INTERFACE_DEFINED__ */


#ifndef __IVsShell5_INTERFACE_DEFINED__
#define __IVsShell5_INTERFACE_DEFINED__

/* interface IVsShell5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsShell5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0963C9C9-1083-4BAF-B3F0-55139F708821")
    IVsShell5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadPackageWithContext( 
            /* [in] */ __RPC__in REFGUID packageGuid,
            /* [in] */ int reason,
            /* [in] */ __RPC__in REFGUID context,
            /* [retval][out] */ __RPC__deref_out_opt IVsPackage **package) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreatePackageExtension( 
            /* [in] */ __RPC__in REFGUID package,
            /* [in] */ __RPC__in REFCLSID extensionPoint,
            /* [in] */ __RPC__in REFGUID instance,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsShell5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsShell5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsShell5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsShell5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadPackageWithContext )( 
            __RPC__in IVsShell5 * This,
            /* [in] */ __RPC__in REFGUID packageGuid,
            /* [in] */ int reason,
            /* [in] */ __RPC__in REFGUID context,
            /* [retval][out] */ __RPC__deref_out_opt IVsPackage **package);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePackageExtension )( 
            __RPC__in IVsShell5 * This,
            /* [in] */ __RPC__in REFGUID package,
            /* [in] */ __RPC__in REFCLSID extensionPoint,
            /* [in] */ __RPC__in REFGUID instance,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk);
        
        END_INTERFACE
    } IVsShell5Vtbl;

    interface IVsShell5
    {
        CONST_VTBL struct IVsShell5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsShell5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsShell5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsShell5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsShell5_LoadPackageWithContext(This,packageGuid,reason,context,package)	\
    ( (This)->lpVtbl -> LoadPackageWithContext(This,packageGuid,reason,context,package) ) 

#define IVsShell5_CreatePackageExtension(This,package,extensionPoint,instance,ppunk)	\
    ( (This)->lpVtbl -> CreatePackageExtension(This,package,extensionPoint,instance,ppunk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsShell5_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0004 */
/* [local] */ 

typedef /* [public] */ struct __MIDL___MIDL_itf_vsshell110_0000_0004_0001
    {
    GUID Factory;
    DWORD ElementID;
    } 	VsUIElementDescriptor;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0004_v0_0_s_ifspec;

#ifndef __IVsDiagnosticsItem_INTERFACE_DEFINED__
#define __IVsDiagnosticsItem_INTERFACE_DEFINED__

/* interface IVsDiagnosticsItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDiagnosticsItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a20b98d4-f56c-4b53-819b-3b0e7147b961")
    IVsDiagnosticsItem : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DiagnosticsName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDiagnosticsItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDiagnosticsItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDiagnosticsItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDiagnosticsItem * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DiagnosticsName )( 
            __RPC__in IVsDiagnosticsItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrName);
        
        END_INTERFACE
    } IVsDiagnosticsItemVtbl;

    interface IVsDiagnosticsItem
    {
        CONST_VTBL struct IVsDiagnosticsItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDiagnosticsItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDiagnosticsItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDiagnosticsItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDiagnosticsItem_get_DiagnosticsName(This,pstrName)	\
    ( (This)->lpVtbl -> get_DiagnosticsName(This,pstrName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDiagnosticsItem_INTERFACE_DEFINED__ */


#ifndef __IVsDiagnosticsProvider_INTERFACE_DEFINED__
#define __IVsDiagnosticsProvider_INTERFACE_DEFINED__

/* interface IVsDiagnosticsProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDiagnosticsProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("79f357cd-d78e-44f7-8442-376ff2bff0c4")
    IVsDiagnosticsProvider : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Version( 
            /* [retval][out] */ __RPC__out DWORD *pdwVersion) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DataModel( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppDataModel) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDiagnosticsProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDiagnosticsProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDiagnosticsProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDiagnosticsProvider * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            __RPC__in IVsDiagnosticsProvider * This,
            /* [retval][out] */ __RPC__out DWORD *pdwVersion);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataModel )( 
            __RPC__in IVsDiagnosticsProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppDataModel);
        
        END_INTERFACE
    } IVsDiagnosticsProviderVtbl;

    interface IVsDiagnosticsProvider
    {
        CONST_VTBL struct IVsDiagnosticsProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDiagnosticsProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDiagnosticsProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDiagnosticsProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDiagnosticsProvider_get_Version(This,pdwVersion)	\
    ( (This)->lpVtbl -> get_Version(This,pdwVersion) ) 

#define IVsDiagnosticsProvider_get_DataModel(This,ppDataModel)	\
    ( (This)->lpVtbl -> get_DataModel(This,ppDataModel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDiagnosticsProvider_INTERFACE_DEFINED__ */


#ifndef __IVsDataObjectStringMapManager2_INTERFACE_DEFINED__
#define __IVsDataObjectStringMapManager2_INTERFACE_DEFINED__

/* interface IVsDataObjectStringMapManager2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDataObjectStringMapManager2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("65CA3EC7-633E-4846-9946-17A80FF4C34C")
    IVsDataObjectStringMapManager2 : public IVsDataObjectStringMapManager
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE ClearMapCache( 
            /* [in] */ __RPC__in LPCWSTR szStringMapName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDataObjectStringMapManager2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDataObjectStringMapManager2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDataObjectStringMapManager2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReadStringMap )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *WriteStringMap )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [in] */ BOOL fOverwriteExisting,
            /* [in] */ __RPC__in_opt IVsStringMap *pStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStringMap )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanges )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in_opt IVsDataObjectStringMapEvents *pStringMapEvents,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanges )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *ClearMapCache )( 
            __RPC__in IVsDataObjectStringMapManager2 * This,
            /* [in] */ __RPC__in LPCWSTR szStringMapName);
        
        END_INTERFACE
    } IVsDataObjectStringMapManager2Vtbl;

    interface IVsDataObjectStringMapManager2
    {
        CONST_VTBL struct IVsDataObjectStringMapManager2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDataObjectStringMapManager2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDataObjectStringMapManager2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDataObjectStringMapManager2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDataObjectStringMapManager2_ReadStringMap(This,pObject,szStringMapName,ppStringMap)	\
    ( (This)->lpVtbl -> ReadStringMap(This,pObject,szStringMapName,ppStringMap) ) 

#define IVsDataObjectStringMapManager2_WriteStringMap(This,pObject,szStringMapName,fOverwriteExisting,pStringMap)	\
    ( (This)->lpVtbl -> WriteStringMap(This,pObject,szStringMapName,fOverwriteExisting,pStringMap) ) 

#define IVsDataObjectStringMapManager2_CreateStringMap(This,ppStringMap)	\
    ( (This)->lpVtbl -> CreateStringMap(This,ppStringMap) ) 

#define IVsDataObjectStringMapManager2_AdviseChanges(This,pObject,pStringMapEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseChanges(This,pObject,pStringMapEvents,pdwCookie) ) 

#define IVsDataObjectStringMapManager2_UnadviseChanges(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseChanges(This,dwCookie) ) 


#define IVsDataObjectStringMapManager2_ClearMapCache(This,szStringMapName)	\
    ( (This)->lpVtbl -> ClearMapCache(This,szStringMapName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDataObjectStringMapManager2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0007 */
/* [local] */ 


enum __VSASYNCTOOLBOXSTATE
    {
        ATS_NONE	= 0,
        ATS_INITIALIZING	= 0x1,
        ATS_FILTERING	= 0x2
    } ;
typedef DWORD VSASYNCTOOLBOXSTATE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0007_v0_0_s_ifspec;

#ifndef __IVsToolbox6_INTERFACE_DEFINED__
#define __IVsToolbox6_INTERFACE_DEFINED__

/* interface IVsToolbox6 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsToolbox6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A403B9B7-34C3-4EEF-B40E-5AC270277D84")
    IVsToolbox6 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddItemToDesigner( 
            /* [in] */ __RPC__in_opt IDataObject *pItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CopyToClipboard( 
            /* [in] */ __RPC__in_opt IDataObject *pItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumTabIDs( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumToolboxTabs **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBitmapBackground( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__out DWORD *pRgbColor) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnresolvedItemName( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnresolvedTabName( 
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsFiltered( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pFiltered) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsFiltered( 
            /* [in] */ VARIANT_BOOL filtered) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsTabVisible( 
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ VARIANT_BOOL fRefresh,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsVisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveItem( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in_opt IDataObject *pInsertionPoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveItemToTab( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in LPCWSTR szTabID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveTab( 
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ __RPC__in LPCWSTR szInsertionPoint) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PasteFromClipboard( 
            /* [in] */ __RPC__in LPCWSTR szTabID,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RenameItem( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in LPCWSTR szName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RenameTab( 
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ __RPC__in LPCWSTR szName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetToolbox( 
            /* [in] */ VARIANT_BOOL promptUser,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemovePackageContent( 
            /* [in] */ __RPC__in REFGUID package) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsItemFilteredInvisible( 
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pInvisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAsyncState( 
            /* [retval][out] */ __RPC__out VSASYNCTOOLBOXSTATE *pState) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolbox6Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolbox6 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolbox6 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddItemToDesigner )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem);
        
        HRESULT ( STDMETHODCALLTYPE *CopyToClipboard )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem);
        
        HRESULT ( STDMETHODCALLTYPE *EnumTabIDs )( 
            __RPC__in IVsToolbox6 * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumToolboxTabs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetBitmapBackground )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__out DWORD *pRgbColor);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnresolvedItemName )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnresolvedTabName )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsFiltered )( 
            __RPC__in IVsToolbox6 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pFiltered);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsFiltered )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ VARIANT_BOOL filtered);
        
        HRESULT ( STDMETHODCALLTYPE *IsTabVisible )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ VARIANT_BOOL fRefresh,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsVisible);
        
        HRESULT ( STDMETHODCALLTYPE *MoveItem )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in_opt IDataObject *pInsertionPoint);
        
        HRESULT ( STDMETHODCALLTYPE *MoveItemToTab )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in LPCWSTR szTabID);
        
        HRESULT ( STDMETHODCALLTYPE *MoveTab )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ __RPC__in LPCWSTR szInsertionPoint);
        
        HRESULT ( STDMETHODCALLTYPE *PasteFromClipboard )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in LPCWSTR szTabID,
            /* [out] */ __RPC__deref_out_opt IDataObject **ppItem);
        
        HRESULT ( STDMETHODCALLTYPE *RenameItem )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [in] */ __RPC__in LPCWSTR szName);
        
        HRESULT ( STDMETHODCALLTYPE *RenameTab )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in LPCWSTR szID,
            /* [in] */ __RPC__in LPCWSTR szName);
        
        HRESULT ( STDMETHODCALLTYPE *ResetToolbox )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ VARIANT_BOOL promptUser,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *RemovePackageContent )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in REFGUID package);
        
        HRESULT ( STDMETHODCALLTYPE *IsItemFilteredInvisible )( 
            __RPC__in IVsToolbox6 * This,
            /* [in] */ __RPC__in_opt IDataObject *pItem,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pInvisible);
        
        HRESULT ( STDMETHODCALLTYPE *GetAsyncState )( 
            __RPC__in IVsToolbox6 * This,
            /* [retval][out] */ __RPC__out VSASYNCTOOLBOXSTATE *pState);
        
        END_INTERFACE
    } IVsToolbox6Vtbl;

    interface IVsToolbox6
    {
        CONST_VTBL struct IVsToolbox6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolbox6_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolbox6_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolbox6_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolbox6_AddItemToDesigner(This,pItem)	\
    ( (This)->lpVtbl -> AddItemToDesigner(This,pItem) ) 

#define IVsToolbox6_CopyToClipboard(This,pItem)	\
    ( (This)->lpVtbl -> CopyToClipboard(This,pItem) ) 

#define IVsToolbox6_EnumTabIDs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumTabIDs(This,ppEnum) ) 

#define IVsToolbox6_GetBitmapBackground(This,pItem,pRgbColor)	\
    ( (This)->lpVtbl -> GetBitmapBackground(This,pItem,pRgbColor) ) 

#define IVsToolbox6_GetUnresolvedItemName(This,pItem,pName)	\
    ( (This)->lpVtbl -> GetUnresolvedItemName(This,pItem,pName) ) 

#define IVsToolbox6_GetUnresolvedTabName(This,szID,pName)	\
    ( (This)->lpVtbl -> GetUnresolvedTabName(This,szID,pName) ) 

#define IVsToolbox6_get_IsFiltered(This,pFiltered)	\
    ( (This)->lpVtbl -> get_IsFiltered(This,pFiltered) ) 

#define IVsToolbox6_put_IsFiltered(This,filtered)	\
    ( (This)->lpVtbl -> put_IsFiltered(This,filtered) ) 

#define IVsToolbox6_IsTabVisible(This,szID,fRefresh,pIsVisible)	\
    ( (This)->lpVtbl -> IsTabVisible(This,szID,fRefresh,pIsVisible) ) 

#define IVsToolbox6_MoveItem(This,pItem,pInsertionPoint)	\
    ( (This)->lpVtbl -> MoveItem(This,pItem,pInsertionPoint) ) 

#define IVsToolbox6_MoveItemToTab(This,pItem,szTabID)	\
    ( (This)->lpVtbl -> MoveItemToTab(This,pItem,szTabID) ) 

#define IVsToolbox6_MoveTab(This,szID,szInsertionPoint)	\
    ( (This)->lpVtbl -> MoveTab(This,szID,szInsertionPoint) ) 

#define IVsToolbox6_PasteFromClipboard(This,szTabID,ppItem)	\
    ( (This)->lpVtbl -> PasteFromClipboard(This,szTabID,ppItem) ) 

#define IVsToolbox6_RenameItem(This,pItem,szName)	\
    ( (This)->lpVtbl -> RenameItem(This,pItem,szName) ) 

#define IVsToolbox6_RenameTab(This,szID,szName)	\
    ( (This)->lpVtbl -> RenameTab(This,szID,szName) ) 

#define IVsToolbox6_ResetToolbox(This,promptUser,ppTask)	\
    ( (This)->lpVtbl -> ResetToolbox(This,promptUser,ppTask) ) 

#define IVsToolbox6_RemovePackageContent(This,package)	\
    ( (This)->lpVtbl -> RemovePackageContent(This,package) ) 

#define IVsToolbox6_IsItemFilteredInvisible(This,pItem,pInvisible)	\
    ( (This)->lpVtbl -> IsItemFilteredInvisible(This,pItem,pInvisible) ) 

#define IVsToolbox6_GetAsyncState(This,pState)	\
    ( (This)->lpVtbl -> GetAsyncState(This,pState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolbox6_INTERFACE_DEFINED__ */


#ifndef __IVsToolWindowToolbarHost3_INTERFACE_DEFINED__
#define __IVsToolWindowToolbarHost3_INTERFACE_DEFINED__

/* interface IVsToolWindowToolbarHost3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolWindowToolbarHost3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8EE99928-6A35-470B-8AFF-96B0319F6B9A")
    IVsToolWindowToolbarHost3 : public IVsToolWindowToolbarHost
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddToolbar3( 
            /* [in] */ VSTWT_LOCATION dwLoc,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId,
            /* [optional][in] */ __RPC__in_opt IDropTarget *pDropTarget,
            /* [optional][in] */ __RPC__in_opt IOleCommandTarget *pCommandTarget) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolWindowToolbarHost3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolWindowToolbarHost3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolWindowToolbarHost3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddToolbar )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ VSTWT_LOCATION dwLoc,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId);
        
        HRESULT ( STDMETHODCALLTYPE *BorderChanged )( 
            __RPC__in IVsToolWindowToolbarHost3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowHideToolbar )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId,
            /* [in] */ BOOL fShow);
        
        HRESULT ( STDMETHODCALLTYPE *ProcessMouseActivation )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT msg,
            /* [in] */ WPARAM wp,
            /* [in] */ LPARAM lp);
        
        HRESULT ( STDMETHODCALLTYPE *ForceUpdateUI )( 
            __RPC__in IVsToolWindowToolbarHost3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProcessMouseActivationModal )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ __RPC__in HWND hwnd,
            /* [in] */ UINT msg,
            /* [in] */ WPARAM wp,
            /* [in] */ LPARAM lp,
            /* [out] */ __RPC__out LRESULT *plResult);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *Show )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *Hide )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *AddToolbar3 )( 
            __RPC__in IVsToolWindowToolbarHost3 * This,
            /* [in] */ VSTWT_LOCATION dwLoc,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId,
            /* [optional][in] */ __RPC__in_opt IDropTarget *pDropTarget,
            /* [optional][in] */ __RPC__in_opt IOleCommandTarget *pCommandTarget);
        
        END_INTERFACE
    } IVsToolWindowToolbarHost3Vtbl;

    interface IVsToolWindowToolbarHost3
    {
        CONST_VTBL struct IVsToolWindowToolbarHost3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolWindowToolbarHost3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolWindowToolbarHost3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolWindowToolbarHost3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolWindowToolbarHost3_AddToolbar(This,dwLoc,pguid,dwId)	\
    ( (This)->lpVtbl -> AddToolbar(This,dwLoc,pguid,dwId) ) 

#define IVsToolWindowToolbarHost3_BorderChanged(This)	\
    ( (This)->lpVtbl -> BorderChanged(This) ) 

#define IVsToolWindowToolbarHost3_ShowHideToolbar(This,pguid,dwId,fShow)	\
    ( (This)->lpVtbl -> ShowHideToolbar(This,pguid,dwId,fShow) ) 

#define IVsToolWindowToolbarHost3_ProcessMouseActivation(This,hwnd,msg,wp,lp)	\
    ( (This)->lpVtbl -> ProcessMouseActivation(This,hwnd,msg,wp,lp) ) 

#define IVsToolWindowToolbarHost3_ForceUpdateUI(This)	\
    ( (This)->lpVtbl -> ForceUpdateUI(This) ) 

#define IVsToolWindowToolbarHost3_ProcessMouseActivationModal(This,hwnd,msg,wp,lp,plResult)	\
    ( (This)->lpVtbl -> ProcessMouseActivationModal(This,hwnd,msg,wp,lp,plResult) ) 

#define IVsToolWindowToolbarHost3_Close(This,dwReserved)	\
    ( (This)->lpVtbl -> Close(This,dwReserved) ) 

#define IVsToolWindowToolbarHost3_Show(This,dwReserved)	\
    ( (This)->lpVtbl -> Show(This,dwReserved) ) 

#define IVsToolWindowToolbarHost3_Hide(This,dwReserved)	\
    ( (This)->lpVtbl -> Hide(This,dwReserved) ) 


#define IVsToolWindowToolbarHost3_AddToolbar3(This,dwLoc,pguid,dwId,pDropTarget,pCommandTarget)	\
    ( (This)->lpVtbl -> AddToolbar3(This,dwLoc,pguid,dwId,pDropTarget,pCommandTarget) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolWindowToolbarHost3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0009 */
/* [local] */ 




















enum __VSSEARCHPLACEMENT
    {
        SP_NONE	= 0,
        SP_DYNAMIC	= 1,
        SP_STRETCH	= 2,
        SP_CUSTOM	= 3
    } ;
typedef DWORD VSSEARCHPLACEMENT;


enum __VSSEARCHPARSEERROR
    {
        SPE_NONE	= 0,
        SPE_UNMATCHEDQUOTES	= 0x1,
        SPE_INVALIDESCAPE	= 0x2,
        SPE_EMPTYFILTERFIELD	= 0x4,
        SPE_EMPTYFILTERVALUE	= 0x8
    } ;
typedef DWORD VSSEARCHPARSEERROR;


enum __VSSEARCHFILTERTOKENTYPE
    {
        SFTT_DEFAULT	= 0,
        SFTT_EXCLUDE	= 0x1,
        SFTT_EXACTMATCH	= 0x2
    } ;
typedef DWORD VSSEARCHFILTERTOKENTYPE;

extern const __declspec(selectany) CLSID CLSID_VsSearchQueryParser = { 0xB71B3DF9, 0x7A4A, 0x4D70, { 0x82, 0x93,  0x38, 0x74, 0xDB, 0x09, 0x8F, 0xDD } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0009_v0_0_s_ifspec;

#ifndef __IVsSearchQueryParser_INTERFACE_DEFINED__
#define __IVsSearchQueryParser_INTERFACE_DEFINED__

/* interface IVsSearchQueryParser */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchQueryParser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E6C0144-256B-46B5-B4A7-0005C86CF85F")
    IVsSearchQueryParser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Parse( 
            /* [in] */ __RPC__in LPCOLESTR pszSearchString,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BuildSearchString( 
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BuildSearchStringFromTokens( 
            /* [in] */ DWORD dwTokens,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTokens) IVsSearchToken *pSearchTokens[  ],
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSearchToken( 
            /* [in] */ __RPC__in LPCOLESTR pszTokenText,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchToken **ppSearchToken) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSearchFilterToken( 
            /* [in] */ __RPC__in LPCOLESTR pszFilterField,
            /* [in] */ __RPC__in LPCOLESTR pszFilterValue,
            /* [in] */ VSSEARCHFILTERTOKENTYPE dwFilterTokenType,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchFilterToken **ppSearchFilterToken) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchQueryParserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchQueryParser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchQueryParser * This);
        
        HRESULT ( STDMETHODCALLTYPE *Parse )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ __RPC__in LPCOLESTR pszSearchString,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery);
        
        HRESULT ( STDMETHODCALLTYPE *BuildSearchString )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString);
        
        HRESULT ( STDMETHODCALLTYPE *BuildSearchStringFromTokens )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ DWORD dwTokens,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTokens) IVsSearchToken *pSearchTokens[  ],
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString);
        
        HRESULT ( STDMETHODCALLTYPE *GetSearchToken )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ __RPC__in LPCOLESTR pszTokenText,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchToken **ppSearchToken);
        
        HRESULT ( STDMETHODCALLTYPE *GetSearchFilterToken )( 
            __RPC__in IVsSearchQueryParser * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilterField,
            /* [in] */ __RPC__in LPCOLESTR pszFilterValue,
            /* [in] */ VSSEARCHFILTERTOKENTYPE dwFilterTokenType,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchFilterToken **ppSearchFilterToken);
        
        END_INTERFACE
    } IVsSearchQueryParserVtbl;

    interface IVsSearchQueryParser
    {
        CONST_VTBL struct IVsSearchQueryParserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchQueryParser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchQueryParser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchQueryParser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchQueryParser_Parse(This,pszSearchString,ppSearchQuery)	\
    ( (This)->lpVtbl -> Parse(This,pszSearchString,ppSearchQuery) ) 

#define IVsSearchQueryParser_BuildSearchString(This,pSearchQuery,pbstrSearchString)	\
    ( (This)->lpVtbl -> BuildSearchString(This,pSearchQuery,pbstrSearchString) ) 

#define IVsSearchQueryParser_BuildSearchStringFromTokens(This,dwTokens,pSearchTokens,pbstrSearchString)	\
    ( (This)->lpVtbl -> BuildSearchStringFromTokens(This,dwTokens,pSearchTokens,pbstrSearchString) ) 

#define IVsSearchQueryParser_GetSearchToken(This,pszTokenText,ppSearchToken)	\
    ( (This)->lpVtbl -> GetSearchToken(This,pszTokenText,ppSearchToken) ) 

#define IVsSearchQueryParser_GetSearchFilterToken(This,pszFilterField,pszFilterValue,dwFilterTokenType,ppSearchFilterToken)	\
    ( (This)->lpVtbl -> GetSearchFilterToken(This,pszFilterField,pszFilterValue,dwFilterTokenType,ppSearchFilterToken) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchQueryParser_INTERFACE_DEFINED__ */


#ifndef __IVsSearchQuery_INTERFACE_DEFINED__
#define __IVsSearchQuery_INTERFACE_DEFINED__

/* interface IVsSearchQuery */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchQuery;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6223B428-B465-4B2B-864A-D0FFBC4741FD")
    IVsSearchQuery : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchString( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ParseError( 
            /* [retval][out] */ __RPC__out VSSEARCHPARSEERROR *pdwParseError) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTokens( 
            /* [in] */ DWORD dwMaxTokens,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxTokens, *pdwTokensReturned) IVsSearchToken *rgpSearchTokens[  ],
            /* [retval][out] */ __RPC__out DWORD *pdwTokensReturned) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchQueryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchQuery * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchQuery * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchQuery * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchString )( 
            __RPC__in IVsSearchQuery * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSearchString);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ParseError )( 
            __RPC__in IVsSearchQuery * This,
            /* [retval][out] */ __RPC__out VSSEARCHPARSEERROR *pdwParseError);
        
        HRESULT ( STDMETHODCALLTYPE *GetTokens )( 
            __RPC__in IVsSearchQuery * This,
            /* [in] */ DWORD dwMaxTokens,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxTokens, *pdwTokensReturned) IVsSearchToken *rgpSearchTokens[  ],
            /* [retval][out] */ __RPC__out DWORD *pdwTokensReturned);
        
        END_INTERFACE
    } IVsSearchQueryVtbl;

    interface IVsSearchQuery
    {
        CONST_VTBL struct IVsSearchQueryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchQuery_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchQuery_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchQuery_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchQuery_get_SearchString(This,pbstrSearchString)	\
    ( (This)->lpVtbl -> get_SearchString(This,pbstrSearchString) ) 

#define IVsSearchQuery_get_ParseError(This,pdwParseError)	\
    ( (This)->lpVtbl -> get_ParseError(This,pdwParseError) ) 

#define IVsSearchQuery_GetTokens(This,dwMaxTokens,rgpSearchTokens,pdwTokensReturned)	\
    ( (This)->lpVtbl -> GetTokens(This,dwMaxTokens,rgpSearchTokens,pdwTokensReturned) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchQuery_INTERFACE_DEFINED__ */


#ifndef __IVsSearchToken_INTERFACE_DEFINED__
#define __IVsSearchToken_INTERFACE_DEFINED__

/* interface IVsSearchToken */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchToken;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DB478CC1-3E66-467A-A893-EDE208E18CA1")
    IVsSearchToken : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_OriginalTokenText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOriginalTokenText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TokenStartPosition( 
            /* [retval][out] */ __RPC__out DWORD *pdwTokenStartPosition) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ParsedTokenText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrParsedTokenText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ParseError( 
            /* [retval][out] */ __RPC__out VSSEARCHPARSEERROR *pdwParseError) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchTokenVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchToken * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchToken * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchToken * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_OriginalTokenText )( 
            __RPC__in IVsSearchToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOriginalTokenText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TokenStartPosition )( 
            __RPC__in IVsSearchToken * This,
            /* [retval][out] */ __RPC__out DWORD *pdwTokenStartPosition);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ParsedTokenText )( 
            __RPC__in IVsSearchToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrParsedTokenText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ParseError )( 
            __RPC__in IVsSearchToken * This,
            /* [retval][out] */ __RPC__out VSSEARCHPARSEERROR *pdwParseError);
        
        END_INTERFACE
    } IVsSearchTokenVtbl;

    interface IVsSearchToken
    {
        CONST_VTBL struct IVsSearchTokenVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchToken_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchToken_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchToken_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchToken_get_OriginalTokenText(This,pbstrOriginalTokenText)	\
    ( (This)->lpVtbl -> get_OriginalTokenText(This,pbstrOriginalTokenText) ) 

#define IVsSearchToken_get_TokenStartPosition(This,pdwTokenStartPosition)	\
    ( (This)->lpVtbl -> get_TokenStartPosition(This,pdwTokenStartPosition) ) 

#define IVsSearchToken_get_ParsedTokenText(This,pbstrParsedTokenText)	\
    ( (This)->lpVtbl -> get_ParsedTokenText(This,pbstrParsedTokenText) ) 

#define IVsSearchToken_get_ParseError(This,pdwParseError)	\
    ( (This)->lpVtbl -> get_ParseError(This,pdwParseError) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchToken_INTERFACE_DEFINED__ */


#ifndef __IVsSearchFilterToken_INTERFACE_DEFINED__
#define __IVsSearchFilterToken_INTERFACE_DEFINED__

/* interface IVsSearchFilterToken */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchFilterToken;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4ABDC811-9CC0-4E07-B34A-442D6CC2C29A")
    IVsSearchFilterToken : public IVsSearchToken
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FilterField( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterField) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FilterValue( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterValue) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FilterTokenType( 
            /* [retval][out] */ __RPC__out VSSEARCHFILTERTOKENTYPE *pdwFilterTokenType) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FilterSeparatorPosition( 
            /* [retval][out] */ __RPC__out DWORD *pdwFilterSeparatorPosition) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchFilterTokenVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchFilterToken * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchFilterToken * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_OriginalTokenText )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOriginalTokenText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TokenStartPosition )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__out DWORD *pdwTokenStartPosition);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ParsedTokenText )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrParsedTokenText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ParseError )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__out VSSEARCHPARSEERROR *pdwParseError);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterField )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterField);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterValue )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterValue);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterTokenType )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__out VSSEARCHFILTERTOKENTYPE *pdwFilterTokenType);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterSeparatorPosition )( 
            __RPC__in IVsSearchFilterToken * This,
            /* [retval][out] */ __RPC__out DWORD *pdwFilterSeparatorPosition);
        
        END_INTERFACE
    } IVsSearchFilterTokenVtbl;

    interface IVsSearchFilterToken
    {
        CONST_VTBL struct IVsSearchFilterTokenVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchFilterToken_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchFilterToken_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchFilterToken_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchFilterToken_get_OriginalTokenText(This,pbstrOriginalTokenText)	\
    ( (This)->lpVtbl -> get_OriginalTokenText(This,pbstrOriginalTokenText) ) 

#define IVsSearchFilterToken_get_TokenStartPosition(This,pdwTokenStartPosition)	\
    ( (This)->lpVtbl -> get_TokenStartPosition(This,pdwTokenStartPosition) ) 

#define IVsSearchFilterToken_get_ParsedTokenText(This,pbstrParsedTokenText)	\
    ( (This)->lpVtbl -> get_ParsedTokenText(This,pbstrParsedTokenText) ) 

#define IVsSearchFilterToken_get_ParseError(This,pdwParseError)	\
    ( (This)->lpVtbl -> get_ParseError(This,pdwParseError) ) 


#define IVsSearchFilterToken_get_FilterField(This,pbstrFilterField)	\
    ( (This)->lpVtbl -> get_FilterField(This,pbstrFilterField) ) 

#define IVsSearchFilterToken_get_FilterValue(This,pbstrFilterValue)	\
    ( (This)->lpVtbl -> get_FilterValue(This,pbstrFilterValue) ) 

#define IVsSearchFilterToken_get_FilterTokenType(This,pdwFilterTokenType)	\
    ( (This)->lpVtbl -> get_FilterTokenType(This,pdwFilterTokenType) ) 

#define IVsSearchFilterToken_get_FilterSeparatorPosition(This,pdwFilterSeparatorPosition)	\
    ( (This)->lpVtbl -> get_FilterSeparatorPosition(This,pdwFilterSeparatorPosition) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchFilterToken_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchHostFactory_INTERFACE_DEFINED__
#define __IVsWindowSearchHostFactory_INTERFACE_DEFINED__

/* interface IVsWindowSearchHostFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchHostFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7525C3AF-CE45-46B3-88D3-4327B50FD8B9")
    IVsWindowSearchHostFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateWindowSearchHost( 
            /* [in] */ __RPC__in_opt IUnknown *pParentControl,
            /* [optional][in] */ __RPC__in_opt IDropTarget *pDropTarget,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearchHost **ppSearchHost) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchHostFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchHostFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchHostFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchHostFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateWindowSearchHost )( 
            __RPC__in IVsWindowSearchHostFactory * This,
            /* [in] */ __RPC__in_opt IUnknown *pParentControl,
            /* [optional][in] */ __RPC__in_opt IDropTarget *pDropTarget,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearchHost **ppSearchHost);
        
        END_INTERFACE
    } IVsWindowSearchHostFactoryVtbl;

    interface IVsWindowSearchHostFactory
    {
        CONST_VTBL struct IVsWindowSearchHostFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchHostFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchHostFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchHostFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchHostFactory_CreateWindowSearchHost(This,pParentControl,pDropTarget,ppSearchHost)	\
    ( (This)->lpVtbl -> CreateWindowSearchHost(This,pParentControl,pDropTarget,ppSearchHost) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchHostFactory_INTERFACE_DEFINED__ */


#ifndef __SVsWindowSearchHostFactory_INTERFACE_DEFINED__
#define __SVsWindowSearchHostFactory_INTERFACE_DEFINED__

/* interface SVsWindowSearchHostFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsWindowSearchHostFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A4E62C4F-C462-43BF-A46B-7A6E9DB2E767")
    SVsWindowSearchHostFactory : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsWindowSearchHostFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsWindowSearchHostFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsWindowSearchHostFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsWindowSearchHostFactory * This);
        
        END_INTERFACE
    } SVsWindowSearchHostFactoryVtbl;

    interface SVsWindowSearchHostFactory
    {
        CONST_VTBL struct SVsWindowSearchHostFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsWindowSearchHostFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsWindowSearchHostFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsWindowSearchHostFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsWindowSearchHostFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0015 */
/* [local] */ 

#define SID_SVsWindowSearchHostFactory IID_SVsWindowSearchHostFactory


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0015_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0015_v0_0_s_ifspec;

#ifndef __IVsWindowSearchHost_INTERFACE_DEFINED__
#define __IVsWindowSearchHost_INTERFACE_DEFINED__

/* interface IVsWindowSearchHost */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("576DEAF2-B527-4345-9A4B-BDAB6426FE03")
    IVsWindowSearchHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetupSearch( 
            /* [in] */ __RPC__in_opt IVsWindowSearch *pWindowSearch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TerminateSearch( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchObject( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearch **ppSearchObject) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchEvents( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearchEvents **ppSearchEvents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SearchAsync( 
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchQueryParser( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQueryParser **ppSearchQueryParser) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchQuery( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchTask( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppTask) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsVisible( 
            /* [in] */ VARIANT_BOOL fVisible) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsVisible( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsEnabled( 
            /* [in] */ VARIANT_BOOL fEnabled) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsPopupVisible( 
            /* [in] */ VARIANT_BOOL fVisible) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsPopupVisible( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Activate( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_HelpTopic( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrHelpTopic) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_HelpTopic( 
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetupSearch )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ __RPC__in_opt IVsWindowSearch *pWindowSearch);
        
        HRESULT ( STDMETHODCALLTYPE *TerminateSearch )( 
            __RPC__in IVsWindowSearchHost * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchObject )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearch **ppSearchObject);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchEvents )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowSearchEvents **ppSearchEvents);
        
        HRESULT ( STDMETHODCALLTYPE *SearchAsync )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchQueryParser )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQueryParser **ppSearchQueryParser);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchQuery )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchTask )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppTask);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsVisible )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ VARIANT_BOOL fVisible);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsVisible )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsEnabled )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ VARIANT_BOOL fEnabled);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsEnabled )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsPopupVisible )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ VARIANT_BOOL fVisible);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsPopupVisible )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible);
        
        HRESULT ( STDMETHODCALLTYPE *Activate )( 
            __RPC__in IVsWindowSearchHost * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_HelpTopic )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrHelpTopic);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_HelpTopic )( 
            __RPC__in IVsWindowSearchHost * This,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic);
        
        END_INTERFACE
    } IVsWindowSearchHostVtbl;

    interface IVsWindowSearchHost
    {
        CONST_VTBL struct IVsWindowSearchHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchHost_SetupSearch(This,pWindowSearch)	\
    ( (This)->lpVtbl -> SetupSearch(This,pWindowSearch) ) 

#define IVsWindowSearchHost_TerminateSearch(This)	\
    ( (This)->lpVtbl -> TerminateSearch(This) ) 

#define IVsWindowSearchHost_get_SearchObject(This,ppSearchObject)	\
    ( (This)->lpVtbl -> get_SearchObject(This,ppSearchObject) ) 

#define IVsWindowSearchHost_get_SearchEvents(This,ppSearchEvents)	\
    ( (This)->lpVtbl -> get_SearchEvents(This,ppSearchEvents) ) 

#define IVsWindowSearchHost_SearchAsync(This,pSearchQuery)	\
    ( (This)->lpVtbl -> SearchAsync(This,pSearchQuery) ) 

#define IVsWindowSearchHost_get_SearchQueryParser(This,ppSearchQueryParser)	\
    ( (This)->lpVtbl -> get_SearchQueryParser(This,ppSearchQueryParser) ) 

#define IVsWindowSearchHost_get_SearchQuery(This,ppSearchQuery)	\
    ( (This)->lpVtbl -> get_SearchQuery(This,ppSearchQuery) ) 

#define IVsWindowSearchHost_get_SearchTask(This,ppTask)	\
    ( (This)->lpVtbl -> get_SearchTask(This,ppTask) ) 

#define IVsWindowSearchHost_put_IsVisible(This,fVisible)	\
    ( (This)->lpVtbl -> put_IsVisible(This,fVisible) ) 

#define IVsWindowSearchHost_get_IsVisible(This,pfVisible)	\
    ( (This)->lpVtbl -> get_IsVisible(This,pfVisible) ) 

#define IVsWindowSearchHost_put_IsEnabled(This,fEnabled)	\
    ( (This)->lpVtbl -> put_IsEnabled(This,fEnabled) ) 

#define IVsWindowSearchHost_get_IsEnabled(This,pfEnabled)	\
    ( (This)->lpVtbl -> get_IsEnabled(This,pfEnabled) ) 

#define IVsWindowSearchHost_put_IsPopupVisible(This,fVisible)	\
    ( (This)->lpVtbl -> put_IsPopupVisible(This,fVisible) ) 

#define IVsWindowSearchHost_get_IsPopupVisible(This,pfVisible)	\
    ( (This)->lpVtbl -> get_IsPopupVisible(This,pfVisible) ) 

#define IVsWindowSearchHost_Activate(This)	\
    ( (This)->lpVtbl -> Activate(This) ) 

#define IVsWindowSearchHost_get_HelpTopic(This,pbstrHelpTopic)	\
    ( (This)->lpVtbl -> get_HelpTopic(This,pbstrHelpTopic) ) 

#define IVsWindowSearchHost_put_HelpTopic(This,lpszHelpTopic)	\
    ( (This)->lpVtbl -> put_HelpTopic(This,lpszHelpTopic) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchHost_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0016 */
/* [local] */ 

typedef 
enum __VSSEARCHSTARTTYPE
    {
        SST_INSTANT	= 0,
        SST_DELAYED	= 1,
        SST_ONDEMAND	= 2
    } 	VSSEARCHSTARTTYPE;

typedef 
enum __VSSEARCHPROGRESSTYPE
    {
        SPT_NONE	= 0,
        SPT_INDETERMINATE	= 1,
        SPT_DETERMINATE	= 2
    } 	VSSEARCHPROGRESSTYPE;


enum __VSSEARCHNAVIGATIONKEY
    {
        SNK_ENTER	= 0,
        SNK_DOWN	= 0x1,
        SNK_UP	= 0x2,
        SNK_PAGEDOWN	= 0x3,
        SNK_PAGEUP	= 0x4,
        SNK_HOME	= 0x5,
        SNK_END	= 0x6
    } ;
typedef DWORD VSSEARCHNAVIGATIONKEY;

#define szWSS_SEARCH_USE_MRU L"SearchUseMRU"
#define szWSS_MAXIMUM_MRU_ITEMS L"MaximumMRUItems"
#define szWSS_SEARCH_START_TYPE L"SearchStartType"
#define szWSS_SEARCH_START_DELAY L"SearchStartDelay"
#define szWSS_SEARCH_START_MINCHARS L"SearchStartMinChars"
#define szWSS_RESTART_SEARCH_IF_UNCHANGED L"RestartSearchIfUnchanged"
#define szWSS_SEARCH_TRIMS_WHITESPACES L"SearchTrimsWhitespaces"
#define szWSS_SEARCH_PROGRESS_TYPE L"SearchProgressType"
#define szWSS_SEARCH_PROGRESS_SHOW_DELAY L"SearchProgressShowDelay"
#define szWSS_SEARCH_POPUP_AUTO_DROPDOWN L"SearchPopupAutoDropdown"
#define szWSS_SEARCH_POPUP_CLOSE_DELAY L"SearchPopupCloseDelay"
#define szWSS_SEARCH_WATERMARK L"SearchWatermark"
#define szWSS_SEARCH_TOOLTIP L"SearchTooltip"
#define szWSS_SEARCH_START_TOOLTIP L"SearchStartTooltip"
#define szWSS_SEARCH_STOP_TOOLTIP L"SearchStopTooltip"
#define szWSS_SEARCH_CLEAR_TOOLTIP L"SearchClearTooltip"
#define szWSS_CONTROL_MIN_WIDTH L"ControlMinWidth"
#define szWSS_CONTROL_MAX_WIDTH L"ControlMaxWidth"
#define szWSS_CONTROL_MIN_POPUP_WIDTH L"ControlMinPopupWidth"
#define szWSS_SEARCH_BUTTON_VISIBLE L"SearchButtonVisible"
#define szWSS_FORWARD_ENTER_ON_SEARCH_START L"ForwardEnterKeyOnSearchStart"
#define szWSS_USE_DEFAULT_THEME_COLORS L"UseDefaultThemeColors"


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0016_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0016_v0_0_s_ifspec;

#ifndef __IVsWindowSearch_INTERFACE_DEFINED__
#define __IVsWindowSearch_INTERFACE_DEFINED__

/* interface IVsWindowSearch */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8640A5BB-A6F8-4E4C-B4D7-C7041CFF3C71")
    IVsWindowSearch : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Category( 
            /* [retval][out] */ __RPC__out GUID *pguidCategoryId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateSearch( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchCallback *pSearchCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppSearchTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearSearch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProvideSearchSettings( 
            /* [in] */ __RPC__in_opt IVsUIDataSource *pSearchSettings) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchFiltersEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWindowSearchFilters **ppEnum) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchOptionsEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWindowSearchOptions **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnNavigationKeyDown( 
            /* [in] */ VSSEARCHNAVIGATIONKEY dwNavigationKey,
            /* [in] */ VSUIACCELMODIFIERS dwModifiers,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfHandled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearch * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchEnabled )( 
            __RPC__in IVsWindowSearch * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Category )( 
            __RPC__in IVsWindowSearch * This,
            /* [retval][out] */ __RPC__out GUID *pguidCategoryId);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSearch )( 
            __RPC__in IVsWindowSearch * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchCallback *pSearchCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppSearchTask);
        
        HRESULT ( STDMETHODCALLTYPE *ClearSearch )( 
            __RPC__in IVsWindowSearch * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProvideSearchSettings )( 
            __RPC__in IVsWindowSearch * This,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pSearchSettings);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchFiltersEnum )( 
            __RPC__in IVsWindowSearch * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWindowSearchFilters **ppEnum);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchOptionsEnum )( 
            __RPC__in IVsWindowSearch * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWindowSearchOptions **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *OnNavigationKeyDown )( 
            __RPC__in IVsWindowSearch * This,
            /* [in] */ VSSEARCHNAVIGATIONKEY dwNavigationKey,
            /* [in] */ VSUIACCELMODIFIERS dwModifiers,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfHandled);
        
        END_INTERFACE
    } IVsWindowSearchVtbl;

    interface IVsWindowSearch
    {
        CONST_VTBL struct IVsWindowSearchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearch_get_SearchEnabled(This,pfEnabled)	\
    ( (This)->lpVtbl -> get_SearchEnabled(This,pfEnabled) ) 

#define IVsWindowSearch_get_Category(This,pguidCategoryId)	\
    ( (This)->lpVtbl -> get_Category(This,pguidCategoryId) ) 

#define IVsWindowSearch_CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask)	\
    ( (This)->lpVtbl -> CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask) ) 

#define IVsWindowSearch_ClearSearch(This)	\
    ( (This)->lpVtbl -> ClearSearch(This) ) 

#define IVsWindowSearch_ProvideSearchSettings(This,pSearchSettings)	\
    ( (This)->lpVtbl -> ProvideSearchSettings(This,pSearchSettings) ) 

#define IVsWindowSearch_get_SearchFiltersEnum(This,ppEnum)	\
    ( (This)->lpVtbl -> get_SearchFiltersEnum(This,ppEnum) ) 

#define IVsWindowSearch_get_SearchOptionsEnum(This,ppEnum)	\
    ( (This)->lpVtbl -> get_SearchOptionsEnum(This,ppEnum) ) 

#define IVsWindowSearch_OnNavigationKeyDown(This,dwNavigationKey,dwModifiers,pfHandled)	\
    ( (This)->lpVtbl -> OnNavigationKeyDown(This,dwNavigationKey,dwModifiers,pfHandled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearch_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchEvents_INTERFACE_DEFINED__
#define __IVsWindowSearchEvents_INTERFACE_DEFINED__

/* interface IVsWindowSearchEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A1702207-BF76-4A77-AD7D-0F41C558FDD3")
    IVsWindowSearchEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SearchFiltersChanged( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SearchOptionsChanged( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SearchOptionValueChanged( 
            /* [in] */ __RPC__in_opt IVsWindowSearchBooleanOption *pOption) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *SearchFiltersChanged )( 
            __RPC__in IVsWindowSearchEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *SearchOptionsChanged )( 
            __RPC__in IVsWindowSearchEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *SearchOptionValueChanged )( 
            __RPC__in IVsWindowSearchEvents * This,
            /* [in] */ __RPC__in_opt IVsWindowSearchBooleanOption *pOption);
        
        END_INTERFACE
    } IVsWindowSearchEventsVtbl;

    interface IVsWindowSearchEvents
    {
        CONST_VTBL struct IVsWindowSearchEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchEvents_SearchFiltersChanged(This)	\
    ( (This)->lpVtbl -> SearchFiltersChanged(This) ) 

#define IVsWindowSearchEvents_SearchOptionsChanged(This)	\
    ( (This)->lpVtbl -> SearchOptionsChanged(This) ) 

#define IVsWindowSearchEvents_SearchOptionValueChanged(This,pOption)	\
    ( (This)->lpVtbl -> SearchOptionValueChanged(This,pOption) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0018 */
/* [local] */ 


enum __VSSEARCHTASKSTATUS
    {
        STS_CREATED	= 0,
        STS_STARTED	= 1,
        STS_COMPLETED	= 2,
        STS_STOPPED	= 3,
        STS_ERROR	= 4
    } ;
typedef DWORD VSSEARCHTASKSTATUS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0018_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0018_v0_0_s_ifspec;

#ifndef __IVsSearchTask_INTERFACE_DEFINED__
#define __IVsSearchTask_INTERFACE_DEFINED__

/* interface IVsSearchTask */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchTask;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D709F307-68DB-4600-9EE2-D9F11BA1E005")
    IVsSearchTask : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Start( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Stop( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Id( 
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchQuery( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Status( 
            /* [retval][out] */ __RPC__out VSSEARCHTASKSTATUS *pTaskStatus) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ErrorCode( 
            /* [retval][out] */ __RPC__out HRESULT *errorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchTaskVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchTask * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchTask * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Start )( 
            __RPC__in IVsSearchTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Stop )( 
            __RPC__in IVsSearchTask * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Id )( 
            __RPC__in IVsSearchTask * This,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchQuery )( 
            __RPC__in IVsSearchTask * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Status )( 
            __RPC__in IVsSearchTask * This,
            /* [retval][out] */ __RPC__out VSSEARCHTASKSTATUS *pTaskStatus);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorCode )( 
            __RPC__in IVsSearchTask * This,
            /* [retval][out] */ __RPC__out HRESULT *errorCode);
        
        END_INTERFACE
    } IVsSearchTaskVtbl;

    interface IVsSearchTask
    {
        CONST_VTBL struct IVsSearchTaskVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchTask_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchTask_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchTask_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchTask_Start(This)	\
    ( (This)->lpVtbl -> Start(This) ) 

#define IVsSearchTask_Stop(This)	\
    ( (This)->lpVtbl -> Stop(This) ) 

#define IVsSearchTask_get_Id(This,pdwCookie)	\
    ( (This)->lpVtbl -> get_Id(This,pdwCookie) ) 

#define IVsSearchTask_get_SearchQuery(This,ppSearchQuery)	\
    ( (This)->lpVtbl -> get_SearchQuery(This,ppSearchQuery) ) 

#define IVsSearchTask_get_Status(This,pTaskStatus)	\
    ( (This)->lpVtbl -> get_Status(This,pTaskStatus) ) 

#define IVsSearchTask_get_ErrorCode(This,errorCode)	\
    ( (This)->lpVtbl -> get_ErrorCode(This,errorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchTask_INTERFACE_DEFINED__ */


#ifndef __IVsSearchCallback_INTERFACE_DEFINED__
#define __IVsSearchCallback_INTERFACE_DEFINED__

/* interface IVsSearchCallback */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FACE369A-F6AB-4EA8-8FA5-E50E326A7CFB")
    IVsSearchCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReportProgress( 
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReportComplete( 
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResultsFound) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportProgress )( 
            __RPC__in IVsSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress);
        
        HRESULT ( STDMETHODCALLTYPE *ReportComplete )( 
            __RPC__in IVsSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResultsFound);
        
        END_INTERFACE
    } IVsSearchCallbackVtbl;

    interface IVsSearchCallback
    {
        CONST_VTBL struct IVsSearchCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchCallback_ReportProgress(This,pTask,dwProgress,dwMaxProgress)	\
    ( (This)->lpVtbl -> ReportProgress(This,pTask,dwProgress,dwMaxProgress) ) 

#define IVsSearchCallback_ReportComplete(This,pTask,dwResultsFound)	\
    ( (This)->lpVtbl -> ReportComplete(This,pTask,dwResultsFound) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchCallback_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchFilter_INTERFACE_DEFINED__
#define __IVsWindowSearchFilter_INTERFACE_DEFINED__

/* interface IVsWindowSearchFilter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E807AAEC-6DDF-41A0-8B5B-8A0A630A7648")
    IVsWindowSearchFilter : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tooltip( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchFilter * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        END_INTERFACE
    } IVsWindowSearchFilterVtbl;

    interface IVsWindowSearchFilter
    {
        CONST_VTBL struct IVsWindowSearchFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchFilter_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchFilter_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchFilter_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchSimpleFilter_INTERFACE_DEFINED__
#define __IVsWindowSearchSimpleFilter_INTERFACE_DEFINED__

/* interface IVsWindowSearchSimpleFilter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchSimpleFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("236D9124-37CA-4EDC-AF07-2D8DFA416F42")
    IVsWindowSearchSimpleFilter : public IVsWindowSearchFilter
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FilterField( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterField) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DefaultFilterValue( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultFilterValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchSimpleFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchSimpleFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchSimpleFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchSimpleFilter * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchSimpleFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchSimpleFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FilterField )( 
            __RPC__in IVsWindowSearchSimpleFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFilterField);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultFilterValue )( 
            __RPC__in IVsWindowSearchSimpleFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultFilterValue);
        
        END_INTERFACE
    } IVsWindowSearchSimpleFilterVtbl;

    interface IVsWindowSearchSimpleFilter
    {
        CONST_VTBL struct IVsWindowSearchSimpleFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchSimpleFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchSimpleFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchSimpleFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchSimpleFilter_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchSimpleFilter_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 


#define IVsWindowSearchSimpleFilter_get_FilterField(This,pbstrFilterField)	\
    ( (This)->lpVtbl -> get_FilterField(This,pbstrFilterField) ) 

#define IVsWindowSearchSimpleFilter_get_DefaultFilterValue(This,pbstrDefaultFilterValue)	\
    ( (This)->lpVtbl -> get_DefaultFilterValue(This,pbstrDefaultFilterValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchSimpleFilter_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchCustomFilter_INTERFACE_DEFINED__
#define __IVsWindowSearchCustomFilter_INTERFACE_DEFINED__

/* interface IVsWindowSearchCustomFilter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchCustomFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BD1E4938-41CF-4F9C-ABB3-2FA4F114197D")
    IVsWindowSearchCustomFilter : public IVsWindowSearchFilter
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ApplyFilter( 
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrSearchString,
            /* [out][in] */ __RPC__inout int *piSelectionStart,
            /* [out][in] */ __RPC__inout int *piSelectionEnd) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchCustomFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchCustomFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchCustomFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchCustomFilter * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchCustomFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchCustomFilter * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        HRESULT ( STDMETHODCALLTYPE *ApplyFilter )( 
            __RPC__in IVsWindowSearchCustomFilter * This,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrSearchString,
            /* [out][in] */ __RPC__inout int *piSelectionStart,
            /* [out][in] */ __RPC__inout int *piSelectionEnd);
        
        END_INTERFACE
    } IVsWindowSearchCustomFilterVtbl;

    interface IVsWindowSearchCustomFilter
    {
        CONST_VTBL struct IVsWindowSearchCustomFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchCustomFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchCustomFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchCustomFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchCustomFilter_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchCustomFilter_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 


#define IVsWindowSearchCustomFilter_ApplyFilter(This,pbstrSearchString,piSelectionStart,piSelectionEnd)	\
    ( (This)->lpVtbl -> ApplyFilter(This,pbstrSearchString,piSelectionStart,piSelectionEnd) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchCustomFilter_INTERFACE_DEFINED__ */


#ifndef __IVsEnumWindowSearchFilters_INTERFACE_DEFINED__
#define __IVsEnumWindowSearchFilters_INTERFACE_DEFINED__

/* interface IVsEnumWindowSearchFilters */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumWindowSearchFilters;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7FD10A54-08D9-4F5B-8142-239F6EF688E4")
    IVsEnumWindowSearchFilters : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWindowSearchFilter *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumWindowSearchFilters **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumWindowSearchFiltersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumWindowSearchFilters * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumWindowSearchFilters * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumWindowSearchFilters * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumWindowSearchFilters * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWindowSearchFilter *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumWindowSearchFilters * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumWindowSearchFilters * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumWindowSearchFilters * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumWindowSearchFilters **ppenum);
        
        END_INTERFACE
    } IVsEnumWindowSearchFiltersVtbl;

    interface IVsEnumWindowSearchFilters
    {
        CONST_VTBL struct IVsEnumWindowSearchFiltersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumWindowSearchFilters_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumWindowSearchFilters_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumWindowSearchFilters_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumWindowSearchFilters_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumWindowSearchFilters_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumWindowSearchFilters_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumWindowSearchFilters_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumWindowSearchFilters_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchOption_INTERFACE_DEFINED__
#define __IVsWindowSearchOption_INTERFACE_DEFINED__

/* interface IVsWindowSearchOption */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchOption;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("77D79CE1-9A26-4DC2-9182-3C5E1D1DFB4C")
    IVsWindowSearchOption : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tooltip( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchOptionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchOption * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchOption * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchOption * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        END_INTERFACE
    } IVsWindowSearchOptionVtbl;

    interface IVsWindowSearchOption
    {
        CONST_VTBL struct IVsWindowSearchOptionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchOption_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchOption_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchOption_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchOption_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchOption_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchOption_INTERFACE_DEFINED__ */


#ifndef __IVsEnumWindowSearchOptions_INTERFACE_DEFINED__
#define __IVsEnumWindowSearchOptions_INTERFACE_DEFINED__

/* interface IVsEnumWindowSearchOptions */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumWindowSearchOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ECD23B8C-20EB-4D95-AD05-6A8C87413D10")
    IVsEnumWindowSearchOptions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWindowSearchOption *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumWindowSearchOptions **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumWindowSearchOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumWindowSearchOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumWindowSearchOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumWindowSearchOptions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumWindowSearchOptions * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWindowSearchOption *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumWindowSearchOptions * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumWindowSearchOptions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumWindowSearchOptions * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumWindowSearchOptions **ppenum);
        
        END_INTERFACE
    } IVsEnumWindowSearchOptionsVtbl;

    interface IVsEnumWindowSearchOptions
    {
        CONST_VTBL struct IVsEnumWindowSearchOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumWindowSearchOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumWindowSearchOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumWindowSearchOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumWindowSearchOptions_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumWindowSearchOptions_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumWindowSearchOptions_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumWindowSearchOptions_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumWindowSearchOptions_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchBooleanOption_INTERFACE_DEFINED__
#define __IVsWindowSearchBooleanOption_INTERFACE_DEFINED__

/* interface IVsWindowSearchBooleanOption */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchBooleanOption;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C1335086-78C1-4F9A-823F-5A7166BC051C")
    IVsWindowSearchBooleanOption : public IVsWindowSearchOption
    {
    public:
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Value( 
            /* [in] */ VARIANT_BOOL fValue) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Value( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchBooleanOptionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchBooleanOption * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchBooleanOption * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchBooleanOption * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchBooleanOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchBooleanOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Value )( 
            __RPC__in IVsWindowSearchBooleanOption * This,
            /* [in] */ VARIANT_BOOL fValue);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Value )( 
            __RPC__in IVsWindowSearchBooleanOption * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfValue);
        
        END_INTERFACE
    } IVsWindowSearchBooleanOptionVtbl;

    interface IVsWindowSearchBooleanOption
    {
        CONST_VTBL struct IVsWindowSearchBooleanOptionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchBooleanOption_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchBooleanOption_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchBooleanOption_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchBooleanOption_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchBooleanOption_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 


#define IVsWindowSearchBooleanOption_put_Value(This,fValue)	\
    ( (This)->lpVtbl -> put_Value(This,fValue) ) 

#define IVsWindowSearchBooleanOption_get_Value(This,pfValue)	\
    ( (This)->lpVtbl -> get_Value(This,pfValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchBooleanOption_INTERFACE_DEFINED__ */


#ifndef __IVsWindowSearchCommandOption_INTERFACE_DEFINED__
#define __IVsWindowSearchCommandOption_INTERFACE_DEFINED__

/* interface IVsWindowSearchCommandOption */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsWindowSearchCommandOption;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F2BE6603-D990-4810-8C0B-8CD36C5B51E9")
    IVsWindowSearchCommandOption : public IVsWindowSearchOption
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Invoke( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowSearchCommandOptionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowSearchCommandOption * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowSearchCommandOption * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowSearchCommandOption * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsWindowSearchCommandOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsWindowSearchCommandOption * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTooltip);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in IVsWindowSearchCommandOption * This);
        
        END_INTERFACE
    } IVsWindowSearchCommandOptionVtbl;

    interface IVsWindowSearchCommandOption
    {
        CONST_VTBL struct IVsWindowSearchCommandOptionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowSearchCommandOption_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowSearchCommandOption_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowSearchCommandOption_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowSearchCommandOption_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsWindowSearchCommandOption_get_Tooltip(This,pbstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrTooltip) ) 


#define IVsWindowSearchCommandOption_Invoke(This)	\
    ( (This)->lpVtbl -> Invoke(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowSearchCommandOption_INTERFACE_DEFINED__ */


#ifndef __IVsEnumSearchProviders_INTERFACE_DEFINED__
#define __IVsEnumSearchProviders_INTERFACE_DEFINED__

/* interface IVsEnumSearchProviders */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumSearchProviders;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6AA5C352-FCBE-45BE-AF99-FE1F8D2935EC")
    IVsEnumSearchProviders : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsSearchProvider *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumSearchProviders **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumSearchProvidersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumSearchProviders * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumSearchProviders * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumSearchProviders * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumSearchProviders * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsSearchProvider *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumSearchProviders * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumSearchProviders * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumSearchProviders * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumSearchProviders **ppenum);
        
        END_INTERFACE
    } IVsEnumSearchProvidersVtbl;

    interface IVsEnumSearchProviders
    {
        CONST_VTBL struct IVsEnumSearchProvidersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumSearchProviders_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumSearchProviders_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumSearchProviders_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumSearchProviders_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumSearchProviders_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumSearchProviders_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumSearchProviders_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumSearchProviders_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0029 */
/* [local] */ 

#define szSPS_SEARCH_PROGRESS_TYPE L"SearchProgressType"
#define szSPS_SEARCH_RESULTS_CACHEABLE L"SearchResultsCacheable"


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0029_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0029_v0_0_s_ifspec;

#ifndef __IVsSearchProvider_INTERFACE_DEFINED__
#define __IVsSearchProvider_INTERFACE_DEFINED__

/* interface IVsSearchProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E85FDFC2-A874-4871-88A3-A2E904183A05")
    IVsSearchProvider : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tooltip( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Category( 
            /* [retval][out] */ __RPC__out GUID *pguidCategoryId) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Shortcut( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCategoryShortcut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateSearch( 
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchProviderCallback *pSearchCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppSearchTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProvideSearchSettings( 
            /* [in] */ __RPC__in_opt IVsUIDataSource *pSearchOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateItemResult( 
            /* [in] */ __RPC__in LPCOLESTR lpszPersistenceData,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchItemResult **ppResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchProvider * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsSearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsSearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsSearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Category )( 
            __RPC__in IVsSearchProvider * This,
            /* [retval][out] */ __RPC__out GUID *pguidCategoryId);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Shortcut )( 
            __RPC__in IVsSearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCategoryShortcut);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSearch )( 
            __RPC__in IVsSearchProvider * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchProviderCallback *pSearchCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppSearchTask);
        
        HRESULT ( STDMETHODCALLTYPE *ProvideSearchSettings )( 
            __RPC__in IVsSearchProvider * This,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pSearchOptions);
        
        HRESULT ( STDMETHODCALLTYPE *CreateItemResult )( 
            __RPC__in IVsSearchProvider * This,
            /* [in] */ __RPC__in LPCOLESTR lpszPersistenceData,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchItemResult **ppResult);
        
        END_INTERFACE
    } IVsSearchProviderVtbl;

    interface IVsSearchProvider
    {
        CONST_VTBL struct IVsSearchProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchProvider_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsSearchProvider_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsSearchProvider_get_Tooltip(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrDescription) ) 

#define IVsSearchProvider_get_Category(This,pguidCategoryId)	\
    ( (This)->lpVtbl -> get_Category(This,pguidCategoryId) ) 

#define IVsSearchProvider_get_Shortcut(This,pbstrCategoryShortcut)	\
    ( (This)->lpVtbl -> get_Shortcut(This,pbstrCategoryShortcut) ) 

#define IVsSearchProvider_CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask)	\
    ( (This)->lpVtbl -> CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask) ) 

#define IVsSearchProvider_ProvideSearchSettings(This,pSearchOptions)	\
    ( (This)->lpVtbl -> ProvideSearchSettings(This,pSearchOptions) ) 

#define IVsSearchProvider_CreateItemResult(This,lpszPersistenceData,ppResult)	\
    ( (This)->lpVtbl -> CreateItemResult(This,lpszPersistenceData,ppResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchProvider_INTERFACE_DEFINED__ */


#ifndef __IVsMRESearchProvider_INTERFACE_DEFINED__
#define __IVsMRESearchProvider_INTERFACE_DEFINED__

/* interface IVsMRESearchProvider */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMRESearchProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3EF528C5-C45A-47E0-B9EE-A212A32A99EC")
    IVsMRESearchProvider : public IVsSearchProvider
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetMostRecentlyExecuted( 
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pResult,
            /* [in] */ DWORD dwMaxResults,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwMaxResults, *pdwActualResults) IVsSearchItemResult *pSearchItemResults[  ],
            /* [out][in] */ __RPC__inout DWORD *pdwActualResults) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSite( 
            /* [in] */ __RPC__in_opt IServiceProvider *pSP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsMRESearchProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsMRESearchProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsMRESearchProvider * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Category )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [retval][out] */ __RPC__out GUID *pguidCategoryId);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Shortcut )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCategoryShortcut);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSearch )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ VSCOOKIE dwCookie,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchProviderCallback *pSearchCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchTask **ppSearchTask);
        
        HRESULT ( STDMETHODCALLTYPE *ProvideSearchSettings )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pSearchOptions);
        
        HRESULT ( STDMETHODCALLTYPE *CreateItemResult )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ __RPC__in LPCOLESTR lpszPersistenceData,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchItemResult **ppResult);
        
        HRESULT ( STDMETHODCALLTYPE *SetMostRecentlyExecuted )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pResult,
            /* [in] */ DWORD dwMaxResults,
            /* [length_is][size_is][out][in] */ __RPC__inout_ecount_part(dwMaxResults, *pdwActualResults) IVsSearchItemResult *pSearchItemResults[  ],
            /* [out][in] */ __RPC__inout DWORD *pdwActualResults);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            __RPC__in IVsMRESearchProvider * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pSP);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            __RPC__in IVsMRESearchProvider * This);
        
        END_INTERFACE
    } IVsMRESearchProviderVtbl;

    interface IVsMRESearchProvider
    {
        CONST_VTBL struct IVsMRESearchProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMRESearchProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMRESearchProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMRESearchProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMRESearchProvider_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsMRESearchProvider_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsMRESearchProvider_get_Tooltip(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pbstrDescription) ) 

#define IVsMRESearchProvider_get_Category(This,pguidCategoryId)	\
    ( (This)->lpVtbl -> get_Category(This,pguidCategoryId) ) 

#define IVsMRESearchProvider_get_Shortcut(This,pbstrCategoryShortcut)	\
    ( (This)->lpVtbl -> get_Shortcut(This,pbstrCategoryShortcut) ) 

#define IVsMRESearchProvider_CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask)	\
    ( (This)->lpVtbl -> CreateSearch(This,dwCookie,pSearchQuery,pSearchCallback,ppSearchTask) ) 

#define IVsMRESearchProvider_ProvideSearchSettings(This,pSearchOptions)	\
    ( (This)->lpVtbl -> ProvideSearchSettings(This,pSearchOptions) ) 

#define IVsMRESearchProvider_CreateItemResult(This,lpszPersistenceData,ppResult)	\
    ( (This)->lpVtbl -> CreateItemResult(This,lpszPersistenceData,ppResult) ) 


#define IVsMRESearchProvider_SetMostRecentlyExecuted(This,pSearchQuery,pResult,dwMaxResults,pSearchItemResults,pdwActualResults)	\
    ( (This)->lpVtbl -> SetMostRecentlyExecuted(This,pSearchQuery,pResult,dwMaxResults,pSearchItemResults,pdwActualResults) ) 

#define IVsMRESearchProvider_SetSite(This,pSP)	\
    ( (This)->lpVtbl -> SetSite(This,pSP) ) 

#define IVsMRESearchProvider_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMRESearchProvider_INTERFACE_DEFINED__ */


#ifndef __IVsSearchProviderCallback_INTERFACE_DEFINED__
#define __IVsSearchProviderCallback_INTERFACE_DEFINED__

/* interface IVsSearchProviderCallback */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchProviderCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8DC9CF50-CE22-40D4-A7E2-C5073B506A03")
    IVsSearchProviderCallback : public IVsSearchCallback
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReportResult( 
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pSearchItemResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReportResults( 
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResults,
            /* [size_is][in] */ __RPC__in_ecount_full(dwResults) IVsSearchItemResult *pSearchItemResults[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchProviderCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchProviderCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchProviderCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchProviderCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportProgress )( 
            __RPC__in IVsSearchProviderCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress);
        
        HRESULT ( STDMETHODCALLTYPE *ReportComplete )( 
            __RPC__in IVsSearchProviderCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResultsFound);
        
        HRESULT ( STDMETHODCALLTYPE *ReportResult )( 
            __RPC__in IVsSearchProviderCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pSearchItemResult);
        
        HRESULT ( STDMETHODCALLTYPE *ReportResults )( 
            __RPC__in IVsSearchProviderCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResults,
            /* [size_is][in] */ __RPC__in_ecount_full(dwResults) IVsSearchItemResult *pSearchItemResults[  ]);
        
        END_INTERFACE
    } IVsSearchProviderCallbackVtbl;

    interface IVsSearchProviderCallback
    {
        CONST_VTBL struct IVsSearchProviderCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchProviderCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchProviderCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchProviderCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchProviderCallback_ReportProgress(This,pTask,dwProgress,dwMaxProgress)	\
    ( (This)->lpVtbl -> ReportProgress(This,pTask,dwProgress,dwMaxProgress) ) 

#define IVsSearchProviderCallback_ReportComplete(This,pTask,dwResultsFound)	\
    ( (This)->lpVtbl -> ReportComplete(This,pTask,dwResultsFound) ) 


#define IVsSearchProviderCallback_ReportResult(This,pTask,pSearchItemResult)	\
    ( (This)->lpVtbl -> ReportResult(This,pTask,pSearchItemResult) ) 

#define IVsSearchProviderCallback_ReportResults(This,pTask,dwResults,pSearchItemResults)	\
    ( (This)->lpVtbl -> ReportResults(This,pTask,dwResults,pSearchItemResults) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchProviderCallback_INTERFACE_DEFINED__ */


#ifndef __IVsSearchItemResult_INTERFACE_DEFINED__
#define __IVsSearchItemResult_INTERFACE_DEFINED__

/* interface IVsSearchItemResult */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchItemResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B03B8A31-6BF6-46B1-AC58-0C973F05FFC8")
    IVsSearchItemResult : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tooltip( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrTooltip) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Icon( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InvokeAction( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchProvider( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_PersistenceData( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPersistenceData) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchItemResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchItemResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchItemResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchItemResult * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrTooltip);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Icon )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject);
        
        HRESULT ( STDMETHODCALLTYPE *InvokeAction )( 
            __RPC__in IVsSearchItemResult * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchProvider )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_PersistenceData )( 
            __RPC__in IVsSearchItemResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPersistenceData);
        
        END_INTERFACE
    } IVsSearchItemResultVtbl;

    interface IVsSearchItemResult
    {
        CONST_VTBL struct IVsSearchItemResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchItemResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchItemResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchItemResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchItemResult_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsSearchItemResult_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsSearchItemResult_get_Tooltip(This,pstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pstrTooltip) ) 

#define IVsSearchItemResult_get_Icon(This,ppIconObject)	\
    ( (This)->lpVtbl -> get_Icon(This,ppIconObject) ) 

#define IVsSearchItemResult_InvokeAction(This)	\
    ( (This)->lpVtbl -> InvokeAction(This) ) 

#define IVsSearchItemResult_get_SearchProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> get_SearchProvider(This,ppProvider) ) 

#define IVsSearchItemResult_get_PersistenceData(This,pbstrPersistenceData)	\
    ( (This)->lpVtbl -> get_PersistenceData(This,pbstrPersistenceData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchItemResult_INTERFACE_DEFINED__ */


#ifndef __IVsSearchItemDynamicResult_INTERFACE_DEFINED__
#define __IVsSearchItemDynamicResult_INTERFACE_DEFINED__

/* interface IVsSearchItemDynamicResult */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSearchItemDynamicResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("412A7EBB-C4BD-46CD-B17D-97E337B6F514")
    IVsSearchItemDynamicResult : public IVsSearchItemResult
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Update( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *isStillValid) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSearchItemDynamicResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSearchItemDynamicResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSearchItemDynamicResult * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayText);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tooltip )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrTooltip);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Icon )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject);
        
        HRESULT ( STDMETHODCALLTYPE *InvokeAction )( 
            __RPC__in IVsSearchItemDynamicResult * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchProvider )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_PersistenceData )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPersistenceData);
        
        HRESULT ( STDMETHODCALLTYPE *Update )( 
            __RPC__in IVsSearchItemDynamicResult * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *isStillValid);
        
        END_INTERFACE
    } IVsSearchItemDynamicResultVtbl;

    interface IVsSearchItemDynamicResult
    {
        CONST_VTBL struct IVsSearchItemDynamicResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSearchItemDynamicResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSearchItemDynamicResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSearchItemDynamicResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSearchItemDynamicResult_get_DisplayText(This,pbstrDisplayText)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pbstrDisplayText) ) 

#define IVsSearchItemDynamicResult_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsSearchItemDynamicResult_get_Tooltip(This,pstrTooltip)	\
    ( (This)->lpVtbl -> get_Tooltip(This,pstrTooltip) ) 

#define IVsSearchItemDynamicResult_get_Icon(This,ppIconObject)	\
    ( (This)->lpVtbl -> get_Icon(This,ppIconObject) ) 

#define IVsSearchItemDynamicResult_InvokeAction(This)	\
    ( (This)->lpVtbl -> InvokeAction(This) ) 

#define IVsSearchItemDynamicResult_get_SearchProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> get_SearchProvider(This,ppProvider) ) 

#define IVsSearchItemDynamicResult_get_PersistenceData(This,pbstrPersistenceData)	\
    ( (This)->lpVtbl -> get_PersistenceData(This,pbstrPersistenceData) ) 


#define IVsSearchItemDynamicResult_Update(This,isStillValid)	\
    ( (This)->lpVtbl -> Update(This,isStillValid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSearchItemDynamicResult_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalSearchTask_INTERFACE_DEFINED__
#define __IVsGlobalSearchTask_INTERFACE_DEFINED__

/* interface IVsGlobalSearchTask */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsGlobalSearchTask;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("88FA9F00-B004-4949-83AD-968B419B6CED")
    IVsGlobalSearchTask : public IVsSearchTask
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Providers( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumSearchProviders **pProviders) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGlobalSearchTaskVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGlobalSearchTask * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGlobalSearchTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Start )( 
            __RPC__in IVsGlobalSearchTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Stop )( 
            __RPC__in IVsGlobalSearchTask * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Id )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchQuery )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchQuery **ppSearchQuery);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Status )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [retval][out] */ __RPC__out VSSEARCHTASKSTATUS *pTaskStatus);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorCode )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [retval][out] */ __RPC__out HRESULT *errorCode);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Providers )( 
            __RPC__in IVsGlobalSearchTask * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumSearchProviders **pProviders);
        
        END_INTERFACE
    } IVsGlobalSearchTaskVtbl;

    interface IVsGlobalSearchTask
    {
        CONST_VTBL struct IVsGlobalSearchTaskVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalSearchTask_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalSearchTask_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalSearchTask_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalSearchTask_Start(This)	\
    ( (This)->lpVtbl -> Start(This) ) 

#define IVsGlobalSearchTask_Stop(This)	\
    ( (This)->lpVtbl -> Stop(This) ) 

#define IVsGlobalSearchTask_get_Id(This,pdwCookie)	\
    ( (This)->lpVtbl -> get_Id(This,pdwCookie) ) 

#define IVsGlobalSearchTask_get_SearchQuery(This,ppSearchQuery)	\
    ( (This)->lpVtbl -> get_SearchQuery(This,ppSearchQuery) ) 

#define IVsGlobalSearchTask_get_Status(This,pTaskStatus)	\
    ( (This)->lpVtbl -> get_Status(This,pTaskStatus) ) 

#define IVsGlobalSearchTask_get_ErrorCode(This,errorCode)	\
    ( (This)->lpVtbl -> get_ErrorCode(This,errorCode) ) 


#define IVsGlobalSearchTask_get_Providers(This,pProviders)	\
    ( (This)->lpVtbl -> get_Providers(This,pProviders) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalSearchTask_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalSearch_INTERFACE_DEFINED__
#define __IVsGlobalSearch_INTERFACE_DEFINED__

/* interface IVsGlobalSearch */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsGlobalSearch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("49429DF4-080D-4218-B4CF-9B7AB81116E9")
    IVsGlobalSearch : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateSearch( 
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsGlobalSearchCallback *pSearchCallback,
            /* [optional][in] */ GUID guidCategory,
            /* [retval][out] */ __RPC__deref_out_opt IVsGlobalSearchTask **ppSearchTask) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Providers( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumSearchProviders **pProviders) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProvider( 
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsProviderEnabled( 
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProviderEnabled( 
            /* [in] */ GUID guidCategoryId,
            /* [in] */ VARIANT_BOOL fEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterProvider( 
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterProvider( 
            /* [in] */ GUID guidCategoryId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProviderSettings( 
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSource **ppProviderSettings) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGlobalSearchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGlobalSearch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGlobalSearch * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateSearch )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ __RPC__in_opt IVsSearchQuery *pSearchQuery,
            /* [in] */ __RPC__in_opt IVsGlobalSearchCallback *pSearchCallback,
            /* [optional][in] */ GUID guidCategory,
            /* [retval][out] */ __RPC__deref_out_opt IVsGlobalSearchTask **ppSearchTask);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Providers )( 
            __RPC__in IVsGlobalSearch * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumSearchProviders **pProviders);
        
        HRESULT ( STDMETHODCALLTYPE *GetProvider )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider);
        
        HRESULT ( STDMETHODCALLTYPE *IsProviderEnabled )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *SetProviderEnabled )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ GUID guidCategoryId,
            /* [in] */ VARIANT_BOOL fEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterProvider )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterProvider )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ GUID guidCategoryId);
        
        HRESULT ( STDMETHODCALLTYPE *GetProviderSettings )( 
            __RPC__in IVsGlobalSearch * This,
            /* [in] */ GUID guidCategoryId,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSource **ppProviderSettings);
        
        END_INTERFACE
    } IVsGlobalSearchVtbl;

    interface IVsGlobalSearch
    {
        CONST_VTBL struct IVsGlobalSearchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalSearch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalSearch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalSearch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalSearch_CreateSearch(This,pSearchQuery,pSearchCallback,guidCategory,ppSearchTask)	\
    ( (This)->lpVtbl -> CreateSearch(This,pSearchQuery,pSearchCallback,guidCategory,ppSearchTask) ) 

#define IVsGlobalSearch_get_Providers(This,pProviders)	\
    ( (This)->lpVtbl -> get_Providers(This,pProviders) ) 

#define IVsGlobalSearch_GetProvider(This,guidCategoryId,ppProvider)	\
    ( (This)->lpVtbl -> GetProvider(This,guidCategoryId,ppProvider) ) 

#define IVsGlobalSearch_IsProviderEnabled(This,guidCategoryId,pfEnabled)	\
    ( (This)->lpVtbl -> IsProviderEnabled(This,guidCategoryId,pfEnabled) ) 

#define IVsGlobalSearch_SetProviderEnabled(This,guidCategoryId,fEnabled)	\
    ( (This)->lpVtbl -> SetProviderEnabled(This,guidCategoryId,fEnabled) ) 

#define IVsGlobalSearch_RegisterProvider(This,pProvider)	\
    ( (This)->lpVtbl -> RegisterProvider(This,pProvider) ) 

#define IVsGlobalSearch_UnregisterProvider(This,guidCategoryId)	\
    ( (This)->lpVtbl -> UnregisterProvider(This,guidCategoryId) ) 

#define IVsGlobalSearch_GetProviderSettings(This,guidCategoryId,ppProviderSettings)	\
    ( (This)->lpVtbl -> GetProviderSettings(This,guidCategoryId,ppProviderSettings) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalSearch_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalSearchCallback_INTERFACE_DEFINED__
#define __IVsGlobalSearchCallback_INTERFACE_DEFINED__

/* interface IVsGlobalSearchCallback */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsGlobalSearchCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C9AF73D3-767D-44C8-BFD9-B4F95AFE215F")
    IVsGlobalSearchCallback : public IVsSearchCallback
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResultReported( 
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pItemResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResultsReported( 
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ DWORD dwResults,
            /* [size_is][in] */ __RPC__in_ecount_full(dwResults) IVsSearchItemResult *pSearchItemResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProgressReported( 
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProviderSearchStarted( 
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchTask *pProviderTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProviderSearchCompleted( 
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchTask *pProviderTask) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGlobalSearchCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGlobalSearchCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGlobalSearchCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportProgress )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress);
        
        HRESULT ( STDMETHODCALLTYPE *ReportComplete )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsSearchTask *pTask,
            /* [in] */ DWORD dwResultsFound);
        
        HRESULT ( STDMETHODCALLTYPE *ResultReported )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchItemResult *pItemResult);
        
        HRESULT ( STDMETHODCALLTYPE *ResultsReported )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ DWORD dwResults,
            /* [size_is][in] */ __RPC__in_ecount_full(dwResults) IVsSearchItemResult *pSearchItemResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *ProgressReported )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ DWORD dwProgress,
            /* [in] */ DWORD dwMaxProgress);
        
        HRESULT ( STDMETHODCALLTYPE *ProviderSearchStarted )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchTask *pProviderTask);
        
        HRESULT ( STDMETHODCALLTYPE *ProviderSearchCompleted )( 
            __RPC__in IVsGlobalSearchCallback * This,
            /* [in] */ __RPC__in_opt IVsGlobalSearchTask *pTask,
            /* [in] */ __RPC__in_opt IVsSearchProvider *pProvider,
            /* [in] */ __RPC__in_opt IVsSearchTask *pProviderTask);
        
        END_INTERFACE
    } IVsGlobalSearchCallbackVtbl;

    interface IVsGlobalSearchCallback
    {
        CONST_VTBL struct IVsGlobalSearchCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalSearchCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalSearchCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalSearchCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalSearchCallback_ReportProgress(This,pTask,dwProgress,dwMaxProgress)	\
    ( (This)->lpVtbl -> ReportProgress(This,pTask,dwProgress,dwMaxProgress) ) 

#define IVsGlobalSearchCallback_ReportComplete(This,pTask,dwResultsFound)	\
    ( (This)->lpVtbl -> ReportComplete(This,pTask,dwResultsFound) ) 


#define IVsGlobalSearchCallback_ResultReported(This,pTask,pProvider,pItemResult)	\
    ( (This)->lpVtbl -> ResultReported(This,pTask,pProvider,pItemResult) ) 

#define IVsGlobalSearchCallback_ResultsReported(This,pTask,pProvider,dwResults,pSearchItemResults)	\
    ( (This)->lpVtbl -> ResultsReported(This,pTask,pProvider,dwResults,pSearchItemResults) ) 

#define IVsGlobalSearchCallback_ProgressReported(This,pTask,pProvider,dwProgress,dwMaxProgress)	\
    ( (This)->lpVtbl -> ProgressReported(This,pTask,pProvider,dwProgress,dwMaxProgress) ) 

#define IVsGlobalSearchCallback_ProviderSearchStarted(This,pTask,pProvider,pProviderTask)	\
    ( (This)->lpVtbl -> ProviderSearchStarted(This,pTask,pProvider,pProviderTask) ) 

#define IVsGlobalSearchCallback_ProviderSearchCompleted(This,pTask,pProvider,pProviderTask)	\
    ( (This)->lpVtbl -> ProviderSearchCompleted(This,pTask,pProvider,pProviderTask) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalSearchCallback_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalSearchUIResultsCategory_INTERFACE_DEFINED__
#define __IVsGlobalSearchUIResultsCategory_INTERFACE_DEFINED__

/* interface IVsGlobalSearchUIResultsCategory */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsGlobalSearchUIResultsCategory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B8DC5B9-04CC-49A6-8011-1FF2A6749B2A")
    IVsGlobalSearchUIResultsCategory : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayText( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CategoryProvider( 
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TotalResultsCount( 
            /* [retval][out] */ __RPC__out DWORD *pdwResults) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayedResultsCount( 
            /* [retval][out] */ __RPC__out DWORD *pdwResults) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResult( 
            /* [in] */ DWORD dwIndex,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchItemResult **ppSearchItemResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisplayAllResults( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsDisplayingAllResults( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfDisplayingAllResults) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGlobalSearchUIResultsCategoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayText )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CategoryProvider )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchProvider **ppProvider);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TotalResultsCount )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [retval][out] */ __RPC__out DWORD *pdwResults);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayedResultsCount )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [retval][out] */ __RPC__out DWORD *pdwResults);
        
        HRESULT ( STDMETHODCALLTYPE *GetResult )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [in] */ DWORD dwIndex,
            /* [retval][out] */ __RPC__deref_out_opt IVsSearchItemResult **ppSearchItemResult);
        
        HRESULT ( STDMETHODCALLTYPE *DisplayAllResults )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsDisplayingAllResults )( 
            __RPC__in IVsGlobalSearchUIResultsCategory * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfDisplayingAllResults);
        
        END_INTERFACE
    } IVsGlobalSearchUIResultsCategoryVtbl;

    interface IVsGlobalSearchUIResultsCategory
    {
        CONST_VTBL struct IVsGlobalSearchUIResultsCategoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalSearchUIResultsCategory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalSearchUIResultsCategory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalSearchUIResultsCategory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalSearchUIResultsCategory_get_DisplayText(This,pRetVal)	\
    ( (This)->lpVtbl -> get_DisplayText(This,pRetVal) ) 

#define IVsGlobalSearchUIResultsCategory_get_CategoryProvider(This,ppProvider)	\
    ( (This)->lpVtbl -> get_CategoryProvider(This,ppProvider) ) 

#define IVsGlobalSearchUIResultsCategory_get_TotalResultsCount(This,pdwResults)	\
    ( (This)->lpVtbl -> get_TotalResultsCount(This,pdwResults) ) 

#define IVsGlobalSearchUIResultsCategory_get_DisplayedResultsCount(This,pdwResults)	\
    ( (This)->lpVtbl -> get_DisplayedResultsCount(This,pdwResults) ) 

#define IVsGlobalSearchUIResultsCategory_GetResult(This,dwIndex,ppSearchItemResult)	\
    ( (This)->lpVtbl -> GetResult(This,dwIndex,ppSearchItemResult) ) 

#define IVsGlobalSearchUIResultsCategory_DisplayAllResults(This)	\
    ( (This)->lpVtbl -> DisplayAllResults(This) ) 

#define IVsGlobalSearchUIResultsCategory_get_IsDisplayingAllResults(This,pfDisplayingAllResults)	\
    ( (This)->lpVtbl -> get_IsDisplayingAllResults(This,pfDisplayingAllResults) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalSearchUIResultsCategory_INTERFACE_DEFINED__ */


#ifndef __IVsGlobalSearchUI_INTERFACE_DEFINED__
#define __IVsGlobalSearchUI_INTERFACE_DEFINED__

/* interface IVsGlobalSearchUI */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsGlobalSearchUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8ED03960-F01B-4A63-A492-7E55AE118D3D")
    IVsGlobalSearchUI : public IUnknown
    {
    public:
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsEnabled( 
            /* [in] */ VARIANT_BOOL fEnabled) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_PreserveResults( 
            /* [in] */ VARIANT_BOOL pfPreserve) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_PreserveResults( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfPreserve) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SearchHost( 
            /* [retval][out][retval][out] */ __RPC__deref_out_opt IVsWindowSearchHost **ppSearchHost) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResultsCategories( 
            /* [in] */ DWORD dwMaxCategories,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxCategories, *pdwCategoriesReturned) IVsGlobalSearchUIResultsCategory **rgpResultsCategories,
            /* [retval][out] */ __RPC__out DWORD *pdwCategoriesReturned) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ActiveResultsCategory( 
            /* [retval][out] */ __RPC__out int *piCategoryIndex) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_ActiveResultsCategory( 
            /* [in] */ int iCategoryIndex) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsResultsListVisible( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsResultsListVisible( 
            /* [in] */ VARIANT_BOOL fVisible) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsActive( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfActive) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Activate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ActivateBack( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGlobalSearchUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGlobalSearchUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGlobalSearchUI * This);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsEnabled )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ VARIANT_BOOL fEnabled);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsEnabled )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfEnabled);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_PreserveResults )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ VARIANT_BOOL pfPreserve);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_PreserveResults )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfPreserve);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SearchHost )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out][retval][out] */ __RPC__deref_out_opt IVsWindowSearchHost **ppSearchHost);
        
        HRESULT ( STDMETHODCALLTYPE *GetResultsCategories )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ DWORD dwMaxCategories,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxCategories, *pdwCategoriesReturned) IVsGlobalSearchUIResultsCategory **rgpResultsCategories,
            /* [retval][out] */ __RPC__out DWORD *pdwCategoriesReturned);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveResultsCategory )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out] */ __RPC__out int *piCategoryIndex);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ActiveResultsCategory )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ int iCategoryIndex);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsResultsListVisible )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfVisible);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsResultsListVisible )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [in] */ VARIANT_BOOL fVisible);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsActive )( 
            __RPC__in IVsGlobalSearchUI * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfActive);
        
        HRESULT ( STDMETHODCALLTYPE *Activate )( 
            __RPC__in IVsGlobalSearchUI * This);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateBack )( 
            __RPC__in IVsGlobalSearchUI * This);
        
        END_INTERFACE
    } IVsGlobalSearchUIVtbl;

    interface IVsGlobalSearchUI
    {
        CONST_VTBL struct IVsGlobalSearchUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGlobalSearchUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGlobalSearchUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGlobalSearchUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGlobalSearchUI_put_IsEnabled(This,fEnabled)	\
    ( (This)->lpVtbl -> put_IsEnabled(This,fEnabled) ) 

#define IVsGlobalSearchUI_get_IsEnabled(This,pfEnabled)	\
    ( (This)->lpVtbl -> get_IsEnabled(This,pfEnabled) ) 

#define IVsGlobalSearchUI_put_PreserveResults(This,pfPreserve)	\
    ( (This)->lpVtbl -> put_PreserveResults(This,pfPreserve) ) 

#define IVsGlobalSearchUI_get_PreserveResults(This,pfPreserve)	\
    ( (This)->lpVtbl -> get_PreserveResults(This,pfPreserve) ) 

#define IVsGlobalSearchUI_get_SearchHost(This,ppSearchHost)	\
    ( (This)->lpVtbl -> get_SearchHost(This,ppSearchHost) ) 

#define IVsGlobalSearchUI_GetResultsCategories(This,dwMaxCategories,rgpResultsCategories,pdwCategoriesReturned)	\
    ( (This)->lpVtbl -> GetResultsCategories(This,dwMaxCategories,rgpResultsCategories,pdwCategoriesReturned) ) 

#define IVsGlobalSearchUI_get_ActiveResultsCategory(This,piCategoryIndex)	\
    ( (This)->lpVtbl -> get_ActiveResultsCategory(This,piCategoryIndex) ) 

#define IVsGlobalSearchUI_put_ActiveResultsCategory(This,iCategoryIndex)	\
    ( (This)->lpVtbl -> put_ActiveResultsCategory(This,iCategoryIndex) ) 

#define IVsGlobalSearchUI_get_IsResultsListVisible(This,pfVisible)	\
    ( (This)->lpVtbl -> get_IsResultsListVisible(This,pfVisible) ) 

#define IVsGlobalSearchUI_put_IsResultsListVisible(This,fVisible)	\
    ( (This)->lpVtbl -> put_IsResultsListVisible(This,fVisible) ) 

#define IVsGlobalSearchUI_get_IsActive(This,pfActive)	\
    ( (This)->lpVtbl -> get_IsActive(This,pfActive) ) 

#define IVsGlobalSearchUI_Activate(This)	\
    ( (This)->lpVtbl -> Activate(This) ) 

#define IVsGlobalSearchUI_ActivateBack(This)	\
    ( (This)->lpVtbl -> ActivateBack(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGlobalSearchUI_INTERFACE_DEFINED__ */


#ifndef __SVsGlobalSearch_INTERFACE_DEFINED__
#define __SVsGlobalSearch_INTERFACE_DEFINED__

/* interface SVsGlobalSearch */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsGlobalSearch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("37470C6F-F752-4E39-8292-0D2B435E0D6F")
    SVsGlobalSearch : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsGlobalSearchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsGlobalSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsGlobalSearch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsGlobalSearch * This);
        
        END_INTERFACE
    } SVsGlobalSearchVtbl;

    interface SVsGlobalSearch
    {
        CONST_VTBL struct SVsGlobalSearchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsGlobalSearch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsGlobalSearch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsGlobalSearch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsGlobalSearch_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0040 */
/* [local] */ 

#define SID_SVsGlobalSearch IID_SVsGlobalSearch
typedef 
enum __VSPROFILERPROCESSARCHTYPE
    {
        ARCH_UNKNOWN	= 0,
        ARCH_X86	= 1,
        ARCH_X64	= 2
    } 	VSPROFILERPROCESSARCHTYPE;


enum __VSPROFILERLAUNCHOPTS
    {
        VSPLO_NOPROFILE	= 0x1
    } ;
typedef DWORD VSPROFILERLAUNCHOPTS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0040_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0040_v0_0_s_ifspec;

#ifndef __IVsProfilerTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("11F356E3-2BDC-4AB4-ACD2-48DB043951AA")
    IVsProfilerTargetInfo : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ProcessArchitecture( 
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        END_INTERFACE
    } IVsProfilerTargetInfoVtbl;

    interface IVsProfilerTargetInfo
    {
        CONST_VTBL struct IVsProfilerTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerLaunchTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerLaunchTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerLaunchTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerLaunchTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5E580D53-B85D-41AB-A8F8-B4CD5517EC49")
    IVsProfilerLaunchTargetInfo : public IVsProfilerTargetInfo
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_References( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgbstr) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_EnvironmentSettings( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstr) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_LaunchProfilerFlags( 
            /* [retval][out] */ __RPC__out VSPROFILERLAUNCHOPTS *opts) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerLaunchTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnvironmentSettings )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LaunchProfilerFlags )( 
            __RPC__in IVsProfilerLaunchTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERLAUNCHOPTS *opts);
        
        END_INTERFACE
    } IVsProfilerLaunchTargetInfoVtbl;

    interface IVsProfilerLaunchTargetInfo
    {
        CONST_VTBL struct IVsProfilerLaunchTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerLaunchTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerLaunchTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerLaunchTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerLaunchTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 


#define IVsProfilerLaunchTargetInfo_get_References(This,rgbstr)	\
    ( (This)->lpVtbl -> get_References(This,rgbstr) ) 

#define IVsProfilerLaunchTargetInfo_get_EnvironmentSettings(This,pbstr)	\
    ( (This)->lpVtbl -> get_EnvironmentSettings(This,pbstr) ) 

#define IVsProfilerLaunchTargetInfo_get_LaunchProfilerFlags(This,opts)	\
    ( (This)->lpVtbl -> get_LaunchProfilerFlags(This,opts) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerLaunchTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerLaunchExeTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerLaunchExeTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerLaunchExeTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerLaunchExeTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9DF0CDE8-5971-408A-B76F-993A8A78EEE0")
    IVsProfilerLaunchExeTargetInfo : public IVsProfilerLaunchTargetInfo
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ExecutableArguments( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ExecutablePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_WorkingDirectory( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerLaunchExeTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnvironmentSettings )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LaunchProfilerFlags )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERLAUNCHOPTS *opts);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExecutableArguments )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExecutablePath )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_WorkingDirectory )( 
            __RPC__in IVsProfilerLaunchExeTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        END_INTERFACE
    } IVsProfilerLaunchExeTargetInfoVtbl;

    interface IVsProfilerLaunchExeTargetInfo
    {
        CONST_VTBL struct IVsProfilerLaunchExeTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerLaunchExeTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerLaunchExeTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerLaunchExeTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerLaunchExeTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 


#define IVsProfilerLaunchExeTargetInfo_get_References(This,rgbstr)	\
    ( (This)->lpVtbl -> get_References(This,rgbstr) ) 

#define IVsProfilerLaunchExeTargetInfo_get_EnvironmentSettings(This,pbstr)	\
    ( (This)->lpVtbl -> get_EnvironmentSettings(This,pbstr) ) 

#define IVsProfilerLaunchExeTargetInfo_get_LaunchProfilerFlags(This,opts)	\
    ( (This)->lpVtbl -> get_LaunchProfilerFlags(This,opts) ) 


#define IVsProfilerLaunchExeTargetInfo_get_ExecutableArguments(This,pbstr)	\
    ( (This)->lpVtbl -> get_ExecutableArguments(This,pbstr) ) 

#define IVsProfilerLaunchExeTargetInfo_get_ExecutablePath(This,pbstr)	\
    ( (This)->lpVtbl -> get_ExecutablePath(This,pbstr) ) 

#define IVsProfilerLaunchExeTargetInfo_get_WorkingDirectory(This,pbstr)	\
    ( (This)->lpVtbl -> get_WorkingDirectory(This,pbstr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerLaunchExeTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerLaunchBrowserTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerLaunchBrowserTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerLaunchBrowserTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerLaunchBrowserTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8DFD2DFC-D11E-4430-B311-1D4C732C45C3")
    IVsProfilerLaunchBrowserTargetInfo : public IVsProfilerLaunchTargetInfo
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Url( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerLaunchBrowserTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnvironmentSettings )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LaunchProfilerFlags )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERLAUNCHOPTS *opts);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Url )( 
            __RPC__in IVsProfilerLaunchBrowserTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        END_INTERFACE
    } IVsProfilerLaunchBrowserTargetInfoVtbl;

    interface IVsProfilerLaunchBrowserTargetInfo
    {
        CONST_VTBL struct IVsProfilerLaunchBrowserTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerLaunchBrowserTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerLaunchBrowserTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerLaunchBrowserTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerLaunchBrowserTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 


#define IVsProfilerLaunchBrowserTargetInfo_get_References(This,rgbstr)	\
    ( (This)->lpVtbl -> get_References(This,rgbstr) ) 

#define IVsProfilerLaunchBrowserTargetInfo_get_EnvironmentSettings(This,pbstr)	\
    ( (This)->lpVtbl -> get_EnvironmentSettings(This,pbstr) ) 

#define IVsProfilerLaunchBrowserTargetInfo_get_LaunchProfilerFlags(This,opts)	\
    ( (This)->lpVtbl -> get_LaunchProfilerFlags(This,opts) ) 


#define IVsProfilerLaunchBrowserTargetInfo_get_Url(This,pbstr)	\
    ( (This)->lpVtbl -> get_Url(This,pbstr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerLaunchBrowserTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerLaunchWebServerTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerLaunchWebServerTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerLaunchWebServerTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerLaunchWebServerTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0D9FAB5B-0B37-44D5-B6C2-A66EF7FE7F6C")
    IVsProfilerLaunchWebServerTargetInfo : public IVsProfilerLaunchTargetInfo
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Url( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerLaunchWebServerTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnvironmentSettings )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LaunchProfilerFlags )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERLAUNCHOPTS *opts);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Url )( 
            __RPC__in IVsProfilerLaunchWebServerTargetInfo * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        END_INTERFACE
    } IVsProfilerLaunchWebServerTargetInfoVtbl;

    interface IVsProfilerLaunchWebServerTargetInfo
    {
        CONST_VTBL struct IVsProfilerLaunchWebServerTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerLaunchWebServerTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerLaunchWebServerTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerLaunchWebServerTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerLaunchWebServerTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 


#define IVsProfilerLaunchWebServerTargetInfo_get_References(This,rgbstr)	\
    ( (This)->lpVtbl -> get_References(This,rgbstr) ) 

#define IVsProfilerLaunchWebServerTargetInfo_get_EnvironmentSettings(This,pbstr)	\
    ( (This)->lpVtbl -> get_EnvironmentSettings(This,pbstr) ) 

#define IVsProfilerLaunchWebServerTargetInfo_get_LaunchProfilerFlags(This,opts)	\
    ( (This)->lpVtbl -> get_LaunchProfilerFlags(This,opts) ) 


#define IVsProfilerLaunchWebServerTargetInfo_get_Url(This,pbstr)	\
    ( (This)->lpVtbl -> get_Url(This,pbstr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerLaunchWebServerTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerAttachTargetInfo_INTERFACE_DEFINED__
#define __IVsProfilerAttachTargetInfo_INTERFACE_DEFINED__

/* interface IVsProfilerAttachTargetInfo */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilerAttachTargetInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("85DE5AFD-0624-45ED-B0B7-53666BEBDFDB")
    IVsProfilerAttachTargetInfo : public IVsProfilerTargetInfo
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ProcessId( 
            /* [retval][out] */ __RPC__out DWORD *processId) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerAttachTargetInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerAttachTargetInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerAttachTargetInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerAttachTargetInfo * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessArchitecture )( 
            __RPC__in IVsProfilerAttachTargetInfo * This,
            /* [retval][out] */ __RPC__out VSPROFILERPROCESSARCHTYPE *arch);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessId )( 
            __RPC__in IVsProfilerAttachTargetInfo * This,
            /* [retval][out] */ __RPC__out DWORD *processId);
        
        END_INTERFACE
    } IVsProfilerAttachTargetInfoVtbl;

    interface IVsProfilerAttachTargetInfo
    {
        CONST_VTBL struct IVsProfilerAttachTargetInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerAttachTargetInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerAttachTargetInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerAttachTargetInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerAttachTargetInfo_get_ProcessArchitecture(This,arch)	\
    ( (This)->lpVtbl -> get_ProcessArchitecture(This,arch) ) 


#define IVsProfilerAttachTargetInfo_get_ProcessId(This,processId)	\
    ( (This)->lpVtbl -> get_ProcessId(This,processId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerAttachTargetInfo_INTERFACE_DEFINED__ */


#ifndef __IEnumVsProfilerTargetInfos_INTERFACE_DEFINED__
#define __IEnumVsProfilerTargetInfos_INTERFACE_DEFINED__

/* interface IEnumVsProfilerTargetInfos */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IEnumVsProfilerTargetInfos;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CF9F5EA4-CE28-4DC6-B058-EE910B9171AE")
    IEnumVsProfilerTargetInfos : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsProfilerTargetInfo **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumVsProfilerTargetInfos **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumVsProfilerTargetInfosVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumVsProfilerTargetInfos * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumVsProfilerTargetInfos * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumVsProfilerTargetInfos * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumVsProfilerTargetInfos * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsProfilerTargetInfo **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumVsProfilerTargetInfos * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumVsProfilerTargetInfos * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumVsProfilerTargetInfos * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsProfilerTargetInfos **ppenum);
        
        END_INTERFACE
    } IEnumVsProfilerTargetInfosVtbl;

    interface IEnumVsProfilerTargetInfos
    {
        CONST_VTBL struct IEnumVsProfilerTargetInfosVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumVsProfilerTargetInfos_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumVsProfilerTargetInfos_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumVsProfilerTargetInfos_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumVsProfilerTargetInfos_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumVsProfilerTargetInfos_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumVsProfilerTargetInfos_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumVsProfilerTargetInfos_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumVsProfilerTargetInfos_INTERFACE_DEFINED__ */


#ifndef __IVsProfilableProjectCfg_INTERFACE_DEFINED__
#define __IVsProfilableProjectCfg_INTERFACE_DEFINED__

/* interface IVsProfilableProjectCfg */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProfilableProjectCfg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C486A0C9-C170-4CAD-A2A2-292EE54969B5")
    IVsProfilableProjectCfg : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SuppressSignedAssemblyWarnings( 
            /* [out][retval] */ __RPC__out VARIANT_BOOL *suppress) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_LegacyWebSupportRequired( 
            /* [out][retval] */ __RPC__out VARIANT_BOOL *required) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSupportedProfilingTasks( 
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *tasks) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeforeLaunch( 
            /* [in] */ __RPC__in BSTR profilingTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeforeTargetsLaunched( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LaunchProfiler( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryProfilerTargetInfoEnum( 
            /* [out] */ __RPC__deref_out_opt IEnumVsProfilerTargetInfos **targetsEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AllBrowserTargetsFinished( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProfilerAnalysisFinished( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilableProjectCfgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SuppressSignedAssemblyWarnings )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [out][retval] */ __RPC__out VARIANT_BOOL *suppress);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LegacyWebSupportRequired )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [out][retval] */ __RPC__out VARIANT_BOOL *required);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedProfilingTasks )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *tasks);
        
        HRESULT ( STDMETHODCALLTYPE *BeforeLaunch )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [in] */ __RPC__in BSTR profilingTask);
        
        HRESULT ( STDMETHODCALLTYPE *BeforeTargetsLaunched )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchProfiler )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryProfilerTargetInfoEnum )( 
            __RPC__in IVsProfilableProjectCfg * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsProfilerTargetInfos **targetsEnum);
        
        HRESULT ( STDMETHODCALLTYPE *AllBrowserTargetsFinished )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProfilerAnalysisFinished )( 
            __RPC__in IVsProfilableProjectCfg * This);
        
        END_INTERFACE
    } IVsProfilableProjectCfgVtbl;

    interface IVsProfilableProjectCfg
    {
        CONST_VTBL struct IVsProfilableProjectCfgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilableProjectCfg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilableProjectCfg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilableProjectCfg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilableProjectCfg_get_SuppressSignedAssemblyWarnings(This,suppress)	\
    ( (This)->lpVtbl -> get_SuppressSignedAssemblyWarnings(This,suppress) ) 

#define IVsProfilableProjectCfg_get_LegacyWebSupportRequired(This,required)	\
    ( (This)->lpVtbl -> get_LegacyWebSupportRequired(This,required) ) 

#define IVsProfilableProjectCfg_GetSupportedProfilingTasks(This,tasks)	\
    ( (This)->lpVtbl -> GetSupportedProfilingTasks(This,tasks) ) 

#define IVsProfilableProjectCfg_BeforeLaunch(This,profilingTask)	\
    ( (This)->lpVtbl -> BeforeLaunch(This,profilingTask) ) 

#define IVsProfilableProjectCfg_BeforeTargetsLaunched(This)	\
    ( (This)->lpVtbl -> BeforeTargetsLaunched(This) ) 

#define IVsProfilableProjectCfg_LaunchProfiler(This)	\
    ( (This)->lpVtbl -> LaunchProfiler(This) ) 

#define IVsProfilableProjectCfg_QueryProfilerTargetInfoEnum(This,targetsEnum)	\
    ( (This)->lpVtbl -> QueryProfilerTargetInfoEnum(This,targetsEnum) ) 

#define IVsProfilableProjectCfg_AllBrowserTargetsFinished(This)	\
    ( (This)->lpVtbl -> AllBrowserTargetsFinished(This) ) 

#define IVsProfilableProjectCfg_ProfilerAnalysisFinished(This)	\
    ( (This)->lpVtbl -> ProfilerAnalysisFinished(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilableProjectCfg_INTERFACE_DEFINED__ */


#ifndef __IVsProfilerLauncher_INTERFACE_DEFINED__
#define __IVsProfilerLauncher_INTERFACE_DEFINED__

/* interface IVsProfilerLauncher */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsProfilerLauncher;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D7DC8C01-AFFF-45FE-B338-426E8072F6B7")
    IVsProfilerLauncher : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchProfiler( 
            /* [in] */ __RPC__in_opt IEnumVsProfilerTargetInfos *targetsEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryProfilingEnvironment( 
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *environment) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfilerLauncherVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfilerLauncher * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfilerLauncher * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfilerLauncher * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchProfiler )( 
            __RPC__in IVsProfilerLauncher * This,
            /* [in] */ __RPC__in_opt IEnumVsProfilerTargetInfos *targetsEnum);
        
        HRESULT ( STDMETHODCALLTYPE *QueryProfilingEnvironment )( 
            __RPC__in IVsProfilerLauncher * This,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *environment);
        
        END_INTERFACE
    } IVsProfilerLauncherVtbl;

    interface IVsProfilerLauncher
    {
        CONST_VTBL struct IVsProfilerLauncherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfilerLauncher_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfilerLauncher_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfilerLauncher_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfilerLauncher_LaunchProfiler(This,targetsEnum)	\
    ( (This)->lpVtbl -> LaunchProfiler(This,targetsEnum) ) 

#define IVsProfilerLauncher_QueryProfilingEnvironment(This,environment)	\
    ( (This)->lpVtbl -> QueryProfilingEnvironment(This,environment) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfilerLauncher_INTERFACE_DEFINED__ */


#ifndef __SVsProfilerLauncher_INTERFACE_DEFINED__
#define __SVsProfilerLauncher_INTERFACE_DEFINED__

/* interface SVsProfilerLauncher */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsProfilerLauncher;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EBEDC435-D4B7-4428-83EF-D59EAA18C7DF")
    SVsProfilerLauncher : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsProfilerLauncherVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsProfilerLauncher * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsProfilerLauncher * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsProfilerLauncher * This);
        
        END_INTERFACE
    } SVsProfilerLauncherVtbl;

    interface SVsProfilerLauncher
    {
        CONST_VTBL struct SVsProfilerLauncherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsProfilerLauncher_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsProfilerLauncher_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsProfilerLauncher_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsProfilerLauncher_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0050 */
/* [local] */ 

#define SID_SVsProfilerLauncher IID_SVsProfilerLauncher


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0050_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0050_v0_0_s_ifspec;

#ifndef __IVsMRUItemsStore_INTERFACE_DEFINED__
#define __IVsMRUItemsStore_INTERFACE_DEFINED__

/* interface IVsMRUItemsStore */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMRUItemsStore;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3DB65A1A-15B6-4449-B8E6-BE31F632DC51")
    IVsMRUItemsStore : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMRUItems( 
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszPrefix,
            /* [in] */ DWORD dwMaxResults,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxResults, *pdwResultsFetched) BSTR *rgbstrItems,
            /* [retval][out] */ __RPC__out DWORD *pdwResultsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddMRUItem( 
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMRUItem( 
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteMRUItem( 
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteMRUItems( 
            /* [in] */ __RPC__in REFGUID guidCategory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMaxMRUItems( 
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ DWORD dwMaxItems) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsMRUItemsStoreVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsMRUItemsStore * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsMRUItemsStore * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMRUItems )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszPrefix,
            /* [in] */ DWORD dwMaxResults,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(dwMaxResults, *pdwResultsFetched) BSTR *rgbstrItems,
            /* [retval][out] */ __RPC__out DWORD *pdwResultsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *AddMRUItem )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem);
        
        HRESULT ( STDMETHODCALLTYPE *SetMRUItem )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteMRUItem )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ __RPC__in LPCOLESTR lpszItem);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteMRUItems )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory);
        
        HRESULT ( STDMETHODCALLTYPE *SetMaxMRUItems )( 
            __RPC__in IVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFGUID guidCategory,
            /* [in] */ DWORD dwMaxItems);
        
        END_INTERFACE
    } IVsMRUItemsStoreVtbl;

    interface IVsMRUItemsStore
    {
        CONST_VTBL struct IVsMRUItemsStoreVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMRUItemsStore_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMRUItemsStore_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMRUItemsStore_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMRUItemsStore_GetMRUItems(This,guidCategory,lpszPrefix,dwMaxResults,rgbstrItems,pdwResultsFetched)	\
    ( (This)->lpVtbl -> GetMRUItems(This,guidCategory,lpszPrefix,dwMaxResults,rgbstrItems,pdwResultsFetched) ) 

#define IVsMRUItemsStore_AddMRUItem(This,guidCategory,lpszItem)	\
    ( (This)->lpVtbl -> AddMRUItem(This,guidCategory,lpszItem) ) 

#define IVsMRUItemsStore_SetMRUItem(This,guidCategory,lpszItem)	\
    ( (This)->lpVtbl -> SetMRUItem(This,guidCategory,lpszItem) ) 

#define IVsMRUItemsStore_DeleteMRUItem(This,guidCategory,lpszItem)	\
    ( (This)->lpVtbl -> DeleteMRUItem(This,guidCategory,lpszItem) ) 

#define IVsMRUItemsStore_DeleteMRUItems(This,guidCategory)	\
    ( (This)->lpVtbl -> DeleteMRUItems(This,guidCategory) ) 

#define IVsMRUItemsStore_SetMaxMRUItems(This,guidCategory,dwMaxItems)	\
    ( (This)->lpVtbl -> SetMaxMRUItems(This,guidCategory,dwMaxItems) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMRUItemsStore_INTERFACE_DEFINED__ */


#ifndef __SVsMRUItemsStore_INTERFACE_DEFINED__
#define __SVsMRUItemsStore_INTERFACE_DEFINED__

/* interface SVsMRUItemsStore */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsMRUItemsStore;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BAAF2BB3-60EF-4439-A8C3-C09B5FAD7DBE")
    SVsMRUItemsStore : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsMRUItemsStoreVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsMRUItemsStore * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsMRUItemsStore * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsMRUItemsStore * This);
        
        END_INTERFACE
    } SVsMRUItemsStoreVtbl;

    interface SVsMRUItemsStore
    {
        CONST_VTBL struct SVsMRUItemsStoreVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsMRUItemsStore_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsMRUItemsStore_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsMRUItemsStore_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsMRUItemsStore_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0052 */
/* [local] */ 

#define SID_SVsMRUItemsStore IID_SVsMRUItemsStore


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0052_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0052_v0_0_s_ifspec;

#ifndef __IVsHierarchyDirectionalDropDataTarget_INTERFACE_DEFINED__
#define __IVsHierarchyDirectionalDropDataTarget_INTERFACE_DEFINED__

/* interface IVsHierarchyDirectionalDropDataTarget */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyDirectionalDropDataTarget;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EDF56D0F-44E4-4E24-ABC1-3851B568EF26")
    IVsHierarchyDirectionalDropDataTarget : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSupportedAreas( 
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__out HierarchyDropArea *pSupportedAreas) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DragEnterArea( 
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DragOverArea( 
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DragLeaveArea( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DropArea( 
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHierarchyDirectionalDropDataTargetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedAreas )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This,
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__out HierarchyDropArea *pSupportedAreas);
        
        HRESULT ( STDMETHODCALLTYPE *DragEnterArea )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *DragOverArea )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This,
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        HRESULT ( STDMETHODCALLTYPE *DragLeaveArea )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This);
        
        HRESULT ( STDMETHODCALLTYPE *DropArea )( 
            __RPC__in IVsHierarchyDirectionalDropDataTarget * This,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ DWORD grfKeyState,
            /* [in] */ VSITEMID itemid,
            /* [in] */ HierarchyDropArea area,
            /* [out][in] */ __RPC__inout DWORD *pdwEffect);
        
        END_INTERFACE
    } IVsHierarchyDirectionalDropDataTargetVtbl;

    interface IVsHierarchyDirectionalDropDataTarget
    {
        CONST_VTBL struct IVsHierarchyDirectionalDropDataTargetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyDirectionalDropDataTarget_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyDirectionalDropDataTarget_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyDirectionalDropDataTarget_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyDirectionalDropDataTarget_GetSupportedAreas(This,itemid,pSupportedAreas)	\
    ( (This)->lpVtbl -> GetSupportedAreas(This,itemid,pSupportedAreas) ) 

#define IVsHierarchyDirectionalDropDataTarget_DragEnterArea(This,pDataObject,grfKeyState,itemid,area,pdwEffect)	\
    ( (This)->lpVtbl -> DragEnterArea(This,pDataObject,grfKeyState,itemid,area,pdwEffect) ) 

#define IVsHierarchyDirectionalDropDataTarget_DragOverArea(This,grfKeyState,itemid,area,pdwEffect)	\
    ( (This)->lpVtbl -> DragOverArea(This,grfKeyState,itemid,area,pdwEffect) ) 

#define IVsHierarchyDirectionalDropDataTarget_DragLeaveArea(This)	\
    ( (This)->lpVtbl -> DragLeaveArea(This) ) 

#define IVsHierarchyDirectionalDropDataTarget_DropArea(This,pDataObject,grfKeyState,itemid,area,pdwEffect)	\
    ( (This)->lpVtbl -> DropArea(This,pDataObject,grfKeyState,itemid,area,pdwEffect) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyDirectionalDropDataTarget_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0053 */
/* [local] */ 


enum __DOCUMENTPREVIEWERTYPE
    {
        DP_InternalBrowser	= 0,
        DP_SystemBrowser	= 1,
        DP_InstalledBrowser	= 2,
        DP_PackageRegistered	= 3,
        DP_UserDefined	= 4
    } ;
typedef DWORD DOCUMENTPREVIEWERTYPE;

typedef DWORD DOCUMENTPREVIEWERRESOLUTION;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0053_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0053_v0_0_s_ifspec;

#ifndef __IVsDocumentPreviewer_INTERFACE_DEFINED__
#define __IVsDocumentPreviewer_INTERFACE_DEFINED__

/* interface IVsDocumentPreviewer */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDocumentPreviewer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1A19A9DB-766E-40B4-90FE-B6EF0BE6299B")
    IVsDocumentPreviewer : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DisplayName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Path( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Arguments( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrArguments) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsDefault( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *isDefault) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ __RPC__out DOCUMENTPREVIEWERTYPE *type) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Resolution( 
            /* [retval][out] */ __RPC__out DOCUMENTPREVIEWERRESOLUTION *resolution) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDocumentPreviewerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDocumentPreviewer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDocumentPreviewer * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayName )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Arguments )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrArguments);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsDefault )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *isDefault);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__out DOCUMENTPREVIEWERTYPE *type);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Resolution )( 
            __RPC__in IVsDocumentPreviewer * This,
            /* [retval][out] */ __RPC__out DOCUMENTPREVIEWERRESOLUTION *resolution);
        
        END_INTERFACE
    } IVsDocumentPreviewerVtbl;

    interface IVsDocumentPreviewer
    {
        CONST_VTBL struct IVsDocumentPreviewerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDocumentPreviewer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDocumentPreviewer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDocumentPreviewer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDocumentPreviewer_get_DisplayName(This,pbstrDisplayName)	\
    ( (This)->lpVtbl -> get_DisplayName(This,pbstrDisplayName) ) 

#define IVsDocumentPreviewer_get_Path(This,pbstrPath)	\
    ( (This)->lpVtbl -> get_Path(This,pbstrPath) ) 

#define IVsDocumentPreviewer_get_Arguments(This,pbstrArguments)	\
    ( (This)->lpVtbl -> get_Arguments(This,pbstrArguments) ) 

#define IVsDocumentPreviewer_get_IsDefault(This,isDefault)	\
    ( (This)->lpVtbl -> get_IsDefault(This,isDefault) ) 

#define IVsDocumentPreviewer_get_Type(This,type)	\
    ( (This)->lpVtbl -> get_Type(This,type) ) 

#define IVsDocumentPreviewer_get_Resolution(This,resolution)	\
    ( (This)->lpVtbl -> get_Resolution(This,resolution) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDocumentPreviewer_INTERFACE_DEFINED__ */


#ifndef __IVsEnumDocumentPreviewers_INTERFACE_DEFINED__
#define __IVsEnumDocumentPreviewers_INTERFACE_DEFINED__

/* interface IVsEnumDocumentPreviewers */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsEnumDocumentPreviewers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B2CA3152-8051-4141-BF11-14A77A4F254E")
    IVsEnumDocumentPreviewers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsDocumentPreviewer *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumDocumentPreviewers **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumDocumentPreviewersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumDocumentPreviewers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumDocumentPreviewers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumDocumentPreviewers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumDocumentPreviewers * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsDocumentPreviewer *rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumDocumentPreviewers * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumDocumentPreviewers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumDocumentPreviewers * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumDocumentPreviewers **ppenum);
        
        END_INTERFACE
    } IVsEnumDocumentPreviewersVtbl;

    interface IVsEnumDocumentPreviewers
    {
        CONST_VTBL struct IVsEnumDocumentPreviewersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumDocumentPreviewers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumDocumentPreviewers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumDocumentPreviewers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumDocumentPreviewers_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumDocumentPreviewers_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumDocumentPreviewers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumDocumentPreviewers_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumDocumentPreviewers_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0055 */
/* [local] */ 

extern const __declspec(selectany) GUID UICONTEXT_StandardPreviewerConfigurationChanging = { 0x6d3cad8e, 0x9129, 0x4ec0, { 0x92, 0x9e, 0x69, 0xb6, 0xf5, 0xd4, 0x40, 0xd } };
extern const __declspec(selectany) GUID UICONTEXT_CurrentSelectionSupportsBlend = { 0x95b9707, 0xce75, 0x4464, { 0xa2, 0x9a, 0xbb, 0xed, 0x6a, 0x67, 0x8, 0x75 } };

enum __VSNEWDOCUMENTSTATE
    {
        NDS_Unspecified	= 0,
        NDS_Provisional	= 0x1,
        NDS_Permanent	= 0x2,
        NDS_StateMask	= 0xff,
        NDS_OnlyFastViews	= 0x40000000,
        NDS_NoActivate	= 0x80000000
    } ;
typedef DWORD VSNEWDOCUMENTSTATE;


enum __VSPROVISIONALVIEWINGSTATUS
    {
        PVS_Disabled	= 0,
        PVS_EnabledSlow	= 1,
        PVS_EnabledLarge	= 2,
        PVS_Enabled	= 3
    } ;
typedef DWORD VSPROVISIONALVIEWINGSTATUS;


enum __VSPHYSICALVIEWATTRIBUTES
    {
        PVA_None	= 0,
        PVA_OpensSlowly	= 0x1,
        PVA_SupportsPreview	= 0x2
    } ;
typedef DWORD VSPHYSICALVIEWATTRIBUTES;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0055_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0055_v0_0_s_ifspec;

#ifndef __IVsUIShellOpenDocument3_INTERFACE_DEFINED__
#define __IVsUIShellOpenDocument3_INTERFACE_DEFINED__

/* interface IVsUIShellOpenDocument3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsUIShellOpenDocument3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e6a63a28-154c-42cb-a6e7-9252c2e6d943")
    IVsUIShellOpenDocument3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetNewDocumentState( 
            /* [in] */ VSNEWDOCUMENTSTATE state,
            /* [in] */ __RPC__in REFGUID reason,
            /* [retval][out] */ __RPC__deref_out_opt IVsNewDocumentStateContext **ppRestoreOnRelease) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_NewDocumentState( 
            /* [retval][out] */ __RPC__out VSNEWDOCUMENTSTATE *state) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProvisionalViewingStatusForFile( 
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in_opt IVsHierarchy *hierarchy,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in REFGUID logicalView,
            /* [retval][out] */ __RPC__out VSPROVISIONALVIEWINGSTATUS *status) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProvisionalViewingStatusForEditor( 
            /* [in] */ __RPC__in REFGUID editor,
            /* [in] */ __RPC__in REFGUID logicalView,
            /* [retval][out] */ __RPC__out VSPROVISIONALVIEWINGSTATUS *status) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DocumentPreviewersEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumDocumentPreviewers **ppEnum) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FirstDefaultPreviewer( 
            /* [retval][out] */ __RPC__deref_out_opt IVsDocumentPreviewer **ppPreviewer) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDefaultPreviewer( 
            /* [in] */ __RPC__in_opt IVsDocumentPreviewer *pPreviewer,
            /* [in] */ DOCUMENTPREVIEWERRESOLUTION resolution,
            /* [in] */ VARIANT_BOOL fExclusive) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIShellOpenDocument3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIShellOpenDocument3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIShellOpenDocument3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetNewDocumentState )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [in] */ VSNEWDOCUMENTSTATE state,
            /* [in] */ __RPC__in REFGUID reason,
            /* [retval][out] */ __RPC__deref_out_opt IVsNewDocumentStateContext **ppRestoreOnRelease);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_NewDocumentState )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [retval][out] */ __RPC__out VSNEWDOCUMENTSTATE *state);
        
        HRESULT ( STDMETHODCALLTYPE *GetProvisionalViewingStatusForFile )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in_opt IVsHierarchy *hierarchy,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in REFGUID logicalView,
            /* [retval][out] */ __RPC__out VSPROVISIONALVIEWINGSTATUS *status);
        
        HRESULT ( STDMETHODCALLTYPE *GetProvisionalViewingStatusForEditor )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [in] */ __RPC__in REFGUID editor,
            /* [in] */ __RPC__in REFGUID logicalView,
            /* [retval][out] */ __RPC__out VSPROVISIONALVIEWINGSTATUS *status);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentPreviewersEnum )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumDocumentPreviewers **ppEnum);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FirstDefaultPreviewer )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsDocumentPreviewer **ppPreviewer);
        
        HRESULT ( STDMETHODCALLTYPE *SetDefaultPreviewer )( 
            __RPC__in IVsUIShellOpenDocument3 * This,
            /* [in] */ __RPC__in_opt IVsDocumentPreviewer *pPreviewer,
            /* [in] */ DOCUMENTPREVIEWERRESOLUTION resolution,
            /* [in] */ VARIANT_BOOL fExclusive);
        
        END_INTERFACE
    } IVsUIShellOpenDocument3Vtbl;

    interface IVsUIShellOpenDocument3
    {
        CONST_VTBL struct IVsUIShellOpenDocument3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShellOpenDocument3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShellOpenDocument3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShellOpenDocument3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShellOpenDocument3_SetNewDocumentState(This,state,reason,ppRestoreOnRelease)	\
    ( (This)->lpVtbl -> SetNewDocumentState(This,state,reason,ppRestoreOnRelease) ) 

#define IVsUIShellOpenDocument3_get_NewDocumentState(This,state)	\
    ( (This)->lpVtbl -> get_NewDocumentState(This,state) ) 

#define IVsUIShellOpenDocument3_GetProvisionalViewingStatusForFile(This,filename,hierarchy,itemid,logicalView,status)	\
    ( (This)->lpVtbl -> GetProvisionalViewingStatusForFile(This,filename,hierarchy,itemid,logicalView,status) ) 

#define IVsUIShellOpenDocument3_GetProvisionalViewingStatusForEditor(This,editor,logicalView,status)	\
    ( (This)->lpVtbl -> GetProvisionalViewingStatusForEditor(This,editor,logicalView,status) ) 

#define IVsUIShellOpenDocument3_get_DocumentPreviewersEnum(This,ppEnum)	\
    ( (This)->lpVtbl -> get_DocumentPreviewersEnum(This,ppEnum) ) 

#define IVsUIShellOpenDocument3_get_FirstDefaultPreviewer(This,ppPreviewer)	\
    ( (This)->lpVtbl -> get_FirstDefaultPreviewer(This,ppPreviewer) ) 

#define IVsUIShellOpenDocument3_SetDefaultPreviewer(This,pPreviewer,resolution,fExclusive)	\
    ( (This)->lpVtbl -> SetDefaultPreviewer(This,pPreviewer,resolution,fExclusive) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShellOpenDocument3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0056 */
/* [local] */ 

#define GUID_FindResultsReason           GUID_FindResults1
#define GUID_FindSymbolResultsReason     GUID_ObjectSearchResultsWindow
#define GUID_SolutionExplorerReason      GUID_SolutionExplorer
extern const __declspec(selectany) GUID GUID_NavigationReason   = { 0x8d57e022, 0x9e44, 0x4efd, { 0x8e, 0x4e, 0x23, 0x02, 0x84, 0xf8, 0x63, 0x76 } };
extern const __declspec(selectany) GUID GUID_TeamExplorerReason = { 0x7aa20502, 0x9463, 0x47b7, { 0xbf, 0x43, 0x34, 0x1b, 0xaf, 0x51, 0x15, 0x7c } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0056_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0056_v0_0_s_ifspec;

#ifndef __IVsNewDocumentStateContext_INTERFACE_DEFINED__
#define __IVsNewDocumentStateContext_INTERFACE_DEFINED__

/* interface IVsNewDocumentStateContext */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsNewDocumentStateContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44dd5120-6dfb-4590-a5cb-1066114996ca")
    IVsNewDocumentStateContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Restore( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsNewDocumentStateContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsNewDocumentStateContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsNewDocumentStateContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsNewDocumentStateContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *Restore )( 
            __RPC__in IVsNewDocumentStateContext * This);
        
        END_INTERFACE
    } IVsNewDocumentStateContextVtbl;

    interface IVsNewDocumentStateContext
    {
        CONST_VTBL struct IVsNewDocumentStateContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNewDocumentStateContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNewDocumentStateContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNewDocumentStateContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNewDocumentStateContext_Restore(This)	\
    ( (This)->lpVtbl -> Restore(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNewDocumentStateContext_INTERFACE_DEFINED__ */


#ifndef __IVsLanguageServiceBuildErrorReporter2_INTERFACE_DEFINED__
#define __IVsLanguageServiceBuildErrorReporter2_INTERFACE_DEFINED__

/* interface IVsLanguageServiceBuildErrorReporter2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLanguageServiceBuildErrorReporter2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("832AACF8-0848-4F87-B037-EDE1B9F11C90")
    IVsLanguageServiceBuildErrorReporter2 : public IVsLanguageServiceBuildErrorReporter
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE ReportError2( 
            /* [in] */ __RPC__in BSTR bstrErrorMessage,
            /* [in] */ __RPC__in BSTR bstrErrorId,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ long iStartLine,
            /* [in] */ long iStartColumn,
            /* [in] */ long iEndLine,
            /* [in] */ long iEndColumn,
            /* [in] */ __RPC__in BSTR bstrFileName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsLanguageServiceBuildErrorReporter2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportError )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This,
            /* [in] */ __RPC__in BSTR bstrErrorMessage,
            /* [in] */ __RPC__in BSTR bstrErrorId,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ long iLine,
            /* [in] */ long iColumn,
            /* [in] */ __RPC__in BSTR bstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *ClearErrors )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *ReportError2 )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter2 * This,
            /* [in] */ __RPC__in BSTR bstrErrorMessage,
            /* [in] */ __RPC__in BSTR bstrErrorId,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ long iStartLine,
            /* [in] */ long iStartColumn,
            /* [in] */ long iEndLine,
            /* [in] */ long iEndColumn,
            /* [in] */ __RPC__in BSTR bstrFileName);
        
        END_INTERFACE
    } IVsLanguageServiceBuildErrorReporter2Vtbl;

    interface IVsLanguageServiceBuildErrorReporter2
    {
        CONST_VTBL struct IVsLanguageServiceBuildErrorReporter2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageServiceBuildErrorReporter2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageServiceBuildErrorReporter2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageServiceBuildErrorReporter2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageServiceBuildErrorReporter2_ReportError(This,bstrErrorMessage,bstrErrorId,nPriority,iLine,iColumn,bstrFileName)	\
    ( (This)->lpVtbl -> ReportError(This,bstrErrorMessage,bstrErrorId,nPriority,iLine,iColumn,bstrFileName) ) 

#define IVsLanguageServiceBuildErrorReporter2_ClearErrors(This)	\
    ( (This)->lpVtbl -> ClearErrors(This) ) 


#define IVsLanguageServiceBuildErrorReporter2_ReportError2(This,bstrErrorMessage,bstrErrorId,nPriority,iStartLine,iStartColumn,iEndLine,iEndColumn,bstrFileName)	\
    ( (This)->lpVtbl -> ReportError2(This,bstrErrorMessage,bstrErrorId,nPriority,iStartLine,iStartColumn,iEndLine,iEndColumn,bstrFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageServiceBuildErrorReporter2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0058 */
/* [local] */ 


enum __VSRDTATTRIB2
    {
        RDTA_DocDataIsReadOnly	= 0x40000,
        RDTA_DocDataIsNotReadOnly	= 0x80000,
        RDTA_NOTIFYDOCCHANGEDEXMASK	= 0xffff0018
    } ;
typedef DWORD VSRDTATTRIB2;


enum _VSRDTFLAGS3
    {
        RDT_DontPollForState	= 0x20000
    } ;
typedef DWORD VSRDTFLAGS3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0058_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0058_v0_0_s_ifspec;

#ifndef __IVsRunningDocumentTable3_INTERFACE_DEFINED__
#define __IVsRunningDocumentTable3_INTERFACE_DEFINED__

/* interface IVsRunningDocumentTable3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsRunningDocumentTable3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30525828-bd80-4bdf-9255-d1e0e1c0f34f")
    IVsRunningDocumentTable3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRelatedSaveTreeItems( 
            /* [in] */ VSCOOKIE cookie,
            /* [in] */ VSRDTSAVEOPTIONS grfSave,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSSAVETREEITEM rgSaveTreeItems[  ],
            /* [retval][out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyDocumentChangedEx( 
            /* [in] */ VSCOOKIE cookie,
            /* [in] */ VSRDTATTRIB2 attributes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsDocumentDirty( 
            /* [in] */ VSCOOKIE cookie,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *dirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsDocumentReadOnly( 
            /* [in] */ VSCOOKIE cookie,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *readOnly) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateDirtyState( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateReadOnlyState( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsRunningDocumentTable3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsRunningDocumentTable3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsRunningDocumentTable3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRelatedSaveTreeItems )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie,
            /* [in] */ VSRDTSAVEOPTIONS grfSave,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSSAVETREEITEM rgSaveTreeItems[  ],
            /* [retval][out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyDocumentChangedEx )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie,
            /* [in] */ VSRDTATTRIB2 attributes);
        
        HRESULT ( STDMETHODCALLTYPE *IsDocumentDirty )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *dirty);
        
        HRESULT ( STDMETHODCALLTYPE *IsDocumentReadOnly )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *readOnly);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateDirtyState )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateReadOnlyState )( 
            __RPC__in IVsRunningDocumentTable3 * This,
            /* [in] */ VSCOOKIE cookie);
        
        END_INTERFACE
    } IVsRunningDocumentTable3Vtbl;

    interface IVsRunningDocumentTable3
    {
        CONST_VTBL struct IVsRunningDocumentTable3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRunningDocumentTable3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRunningDocumentTable3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRunningDocumentTable3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRunningDocumentTable3_GetRelatedSaveTreeItems(This,cookie,grfSave,celt,rgSaveTreeItems,pcActual)	\
    ( (This)->lpVtbl -> GetRelatedSaveTreeItems(This,cookie,grfSave,celt,rgSaveTreeItems,pcActual) ) 

#define IVsRunningDocumentTable3_NotifyDocumentChangedEx(This,cookie,attributes)	\
    ( (This)->lpVtbl -> NotifyDocumentChangedEx(This,cookie,attributes) ) 

#define IVsRunningDocumentTable3_IsDocumentDirty(This,cookie,dirty)	\
    ( (This)->lpVtbl -> IsDocumentDirty(This,cookie,dirty) ) 

#define IVsRunningDocumentTable3_IsDocumentReadOnly(This,cookie,readOnly)	\
    ( (This)->lpVtbl -> IsDocumentReadOnly(This,cookie,readOnly) ) 

#define IVsRunningDocumentTable3_UpdateDirtyState(This,cookie)	\
    ( (This)->lpVtbl -> UpdateDirtyState(This,cookie) ) 

#define IVsRunningDocumentTable3_UpdateReadOnlyState(This,cookie)	\
    ( (This)->lpVtbl -> UpdateReadOnlyState(This,cookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRunningDocumentTable3_INTERFACE_DEFINED__ */


#ifndef __IVsRunningDocTableEvents5_INTERFACE_DEFINED__
#define __IVsRunningDocTableEvents5_INTERFACE_DEFINED__

/* interface IVsRunningDocTableEvents5 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsRunningDocTableEvents5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ee37a122-55a0-4d1d-970a-78d5eecad16e")
    IVsRunningDocTableEvents5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterDocumentLockCountChanged( 
            /* [in] */ VSCOOKIE docCookie,
            /* [in] */ VSRDTFLAGS dwRDTLockType,
            /* [in] */ DWORD dwOldLockCount,
            /* [in] */ DWORD dwNewLockCount) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsRunningDocTableEvents5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsRunningDocTableEvents5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsRunningDocTableEvents5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsRunningDocTableEvents5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterDocumentLockCountChanged )( 
            __RPC__in IVsRunningDocTableEvents5 * This,
            /* [in] */ VSCOOKIE docCookie,
            /* [in] */ VSRDTFLAGS dwRDTLockType,
            /* [in] */ DWORD dwOldLockCount,
            /* [in] */ DWORD dwNewLockCount);
        
        END_INTERFACE
    } IVsRunningDocTableEvents5Vtbl;

    interface IVsRunningDocTableEvents5
    {
        CONST_VTBL struct IVsRunningDocTableEvents5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRunningDocTableEvents5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRunningDocTableEvents5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRunningDocTableEvents5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRunningDocTableEvents5_OnAfterDocumentLockCountChanged(This,docCookie,dwRDTLockType,dwOldLockCount,dwNewLockCount)	\
    ( (This)->lpVtbl -> OnAfterDocumentLockCountChanged(This,docCookie,dwRDTLockType,dwOldLockCount,dwNewLockCount) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRunningDocTableEvents5_INTERFACE_DEFINED__ */


#ifndef __SVsReferenceManager_INTERFACE_DEFINED__
#define __SVsReferenceManager_INTERFACE_DEFINED__

/* interface SVsReferenceManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsReferenceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B9CE963C-C8F4-48F9-8302-0C2D75E48D1E")
    SVsReferenceManager : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsReferenceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsReferenceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsReferenceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsReferenceManager * This);
        
        END_INTERFACE
    } SVsReferenceManagerVtbl;

    interface SVsReferenceManager
    {
        CONST_VTBL struct SVsReferenceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsReferenceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsReferenceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsReferenceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsReferenceManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0061 */
/* [local] */ 

#define SID_SVsReferenceManager IID_SVsReferenceManager


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0061_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0061_v0_0_s_ifspec;

#ifndef __IVsReference_INTERFACE_DEFINED__
#define __IVsReference_INTERFACE_DEFINED__

/* interface IVsReference */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EF0757D6-678E-45DB-8251-8AA39CC8320C")
    IVsReference : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Name( 
            /* [in] */ __RPC__in LPCOLESTR strName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FullPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_FullPath( 
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_AlreadyReferenced( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_AlreadyReferenced( 
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        END_INTERFACE
    } IVsReferenceVtbl;

    interface IVsReference
    {
        CONST_VTBL struct IVsReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0062 */
/* [local] */ 


enum __VSREFERENCECHANGEOPERATION
    {
        VSREFERENCECHANGEOPERATION_ADD	= 1,
        VSREFERENCECHANGEOPERATION_REMOVE	= 2
    } ;
typedef DWORD VSREFERENCECHANGEOPERATION;


enum __VSREFERENCECHANGEOPERATIONRESULT
    {
        VSREFERENCECHANGEOPERATIONRESULT_ALLOW	= 1,
        VSREFERENCECHANGEOPERATIONRESULT_DENY	= 2
    } ;
typedef DWORD VSREFERENCECHANGEOPERATIONRESULT;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0062_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0062_v0_0_s_ifspec;

#ifndef __IVsReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5B99FA62-EEAB-4048-ACCB-7A8EB569CBF5")
    IVsReferenceProviderContext : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ProviderGuid( 
            /* [retval][out] */ __RPC__out GUID *pguidProvider) = 0;
        
        virtual /* [propget][local] */ HRESULT STDMETHODCALLTYPE get_References( 
            /* [retval][out] */ SAFEARRAY * *pReferences) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddReference( 
            /* [in] */ __RPC__in_opt IVsReference *pReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateReference( 
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ReferenceFilterPaths( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_ReferenceFilterPaths( 
            /* [in] */ __RPC__in SAFEARRAY * filterPaths) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        END_INTERFACE
    } IVsReferenceProviderContextVtbl;

    interface IVsReferenceProviderContext
    {
        CONST_VTBL struct IVsReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsAssemblyReference_INTERFACE_DEFINED__
#define __IVsAssemblyReference_INTERFACE_DEFINED__

/* interface IVsAssemblyReference */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsAssemblyReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B62A68C9-EA0A-4AD1-B7E5-80280798A41E")
    IVsAssemblyReference : public IVsReference
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAssemblyReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAssemblyReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAssemblyReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAssemblyReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsAssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsAssemblyReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsAssemblyReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsAssemblyReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsAssemblyReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsAssemblyReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        END_INTERFACE
    } IVsAssemblyReferenceVtbl;

    interface IVsAssemblyReference
    {
        CONST_VTBL struct IVsAssemblyReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAssemblyReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAssemblyReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAssemblyReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAssemblyReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsAssemblyReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsAssemblyReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsAssemblyReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsAssemblyReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsAssemblyReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAssemblyReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0064 */
/* [local] */ 


enum __VSASSEMBLYPROVIDERTAB
    {
        TAB_ASSEMBLY_FRAMEWORK	= 0x1,
        TAB_ASSEMBLY_EXTENSIONS	= 0x2,
        TAB_ASSEMBLY_ALL	= ( TAB_ASSEMBLY_FRAMEWORK | TAB_ASSEMBLY_EXTENSIONS ) 
    } ;
typedef DWORD VSASSEMBLYPROVIDERTAB;

DEFINE_GUID(GUID_AssemblyReferenceProvider,  0x9A341D95, 0x5A64, 0x11d3, 0xBF, 0xF9, 0x00, 0xC0, 0x4F, 0x99, 0x02, 0x35);


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0064_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0064_v0_0_s_ifspec;

#ifndef __IVsAssemblyReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsAssemblyReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsAssemblyReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsAssemblyReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("15FE7E6B-8CAE-446D-8D54-4F73C08ED8C0")
    IVsAssemblyReferenceProviderContext : public IVsReferenceProviderContext
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_AssemblySearchPaths( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_AssemblySearchPaths( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TargetFrameworkMoniker( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TargetFrameworkMoniker( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tabs( 
            /* [retval][out] */ __RPC__out VSASSEMBLYPROVIDERTAB *peTabs) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Tabs( 
            /* [in] */ VSASSEMBLYPROVIDERTAB eTabs) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SupportsRetargeting( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfSupportsRetargeting) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SupportsRetargeting( 
            /* [in] */ VARIANT_BOOL varfSupportsRetargeting) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTabTitle( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrTabTitle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTabTitle( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR szTabTitle) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsImplicitlyReferenced( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfIsImplicitlyReferenced) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsImplicitlyReferenced( 
            /* [in] */ VARIANT_BOOL pvarfIsImplicitlyReferenced) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_RetargetingMessage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_RetargetingMessage( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNoItemsMessageForTab( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrNoItemsMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNoItemsMessageForTab( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR bstrNoItemsMessage) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAssemblyReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblySearchPaths )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblySearchPaths )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tabs )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VSASSEMBLYPROVIDERTAB *peTabs);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Tabs )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB eTabs);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportsRetargeting )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfSupportsRetargeting);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SupportsRetargeting )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VARIANT_BOOL varfSupportsRetargeting);
        
        HRESULT ( STDMETHODCALLTYPE *GetTabTitle )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrTabTitle);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabTitle )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR szTabTitle);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsImplicitlyReferenced )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfIsImplicitlyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsImplicitlyReferenced )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VARIANT_BOOL pvarfIsImplicitlyReferenced);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_RetargetingMessage )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_RetargetingMessage )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNoItemsMessageForTab )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrNoItemsMessage);
        
        HRESULT ( STDMETHODCALLTYPE *SetNoItemsMessageForTab )( 
            __RPC__in IVsAssemblyReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR bstrNoItemsMessage);
        
        END_INTERFACE
    } IVsAssemblyReferenceProviderContextVtbl;

    interface IVsAssemblyReferenceProviderContext
    {
        CONST_VTBL struct IVsAssemblyReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAssemblyReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAssemblyReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAssemblyReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAssemblyReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsAssemblyReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsAssemblyReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsAssemblyReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsAssemblyReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 


#define IVsAssemblyReferenceProviderContext_get_AssemblySearchPaths(This,pRetVal)	\
    ( (This)->lpVtbl -> get_AssemblySearchPaths(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_put_AssemblySearchPaths(This,pRetVal)	\
    ( (This)->lpVtbl -> put_AssemblySearchPaths(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_get_TargetFrameworkMoniker(This,pRetVal)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_put_TargetFrameworkMoniker(This,pRetVal)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_get_Tabs(This,peTabs)	\
    ( (This)->lpVtbl -> get_Tabs(This,peTabs) ) 

#define IVsAssemblyReferenceProviderContext_put_Tabs(This,eTabs)	\
    ( (This)->lpVtbl -> put_Tabs(This,eTabs) ) 

#define IVsAssemblyReferenceProviderContext_get_SupportsRetargeting(This,pvarfSupportsRetargeting)	\
    ( (This)->lpVtbl -> get_SupportsRetargeting(This,pvarfSupportsRetargeting) ) 

#define IVsAssemblyReferenceProviderContext_put_SupportsRetargeting(This,varfSupportsRetargeting)	\
    ( (This)->lpVtbl -> put_SupportsRetargeting(This,varfSupportsRetargeting) ) 

#define IVsAssemblyReferenceProviderContext_GetTabTitle(This,etabId,bstrTabTitle)	\
    ( (This)->lpVtbl -> GetTabTitle(This,etabId,bstrTabTitle) ) 

#define IVsAssemblyReferenceProviderContext_SetTabTitle(This,etabId,szTabTitle)	\
    ( (This)->lpVtbl -> SetTabTitle(This,etabId,szTabTitle) ) 

#define IVsAssemblyReferenceProviderContext_get_IsImplicitlyReferenced(This,pvarfIsImplicitlyReferenced)	\
    ( (This)->lpVtbl -> get_IsImplicitlyReferenced(This,pvarfIsImplicitlyReferenced) ) 

#define IVsAssemblyReferenceProviderContext_put_IsImplicitlyReferenced(This,pvarfIsImplicitlyReferenced)	\
    ( (This)->lpVtbl -> put_IsImplicitlyReferenced(This,pvarfIsImplicitlyReferenced) ) 

#define IVsAssemblyReferenceProviderContext_get_RetargetingMessage(This,pRetVal)	\
    ( (This)->lpVtbl -> get_RetargetingMessage(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_put_RetargetingMessage(This,pRetVal)	\
    ( (This)->lpVtbl -> put_RetargetingMessage(This,pRetVal) ) 

#define IVsAssemblyReferenceProviderContext_GetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage)	\
    ( (This)->lpVtbl -> GetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage) ) 

#define IVsAssemblyReferenceProviderContext_SetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage)	\
    ( (This)->lpVtbl -> SetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAssemblyReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsProjectReference_INTERFACE_DEFINED__
#define __IVsProjectReference_INTERFACE_DEFINED__

/* interface IVsProjectReference */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("49C80755-EA7F-4CCE-9A9E-41E3C9EFA753")
    IVsProjectReference : public IVsReference
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Identity( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Identity( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ReferenceSpecification( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_ReferenceSpecification( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsProjectReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsProjectReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsProjectReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            __RPC__in IVsProjectReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Identity )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceSpecification )( 
            __RPC__in IVsProjectReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceSpecification )( 
            __RPC__in IVsProjectReference * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        END_INTERFACE
    } IVsProjectReferenceVtbl;

    interface IVsProjectReference
    {
        CONST_VTBL struct IVsProjectReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsProjectReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsProjectReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsProjectReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsProjectReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsProjectReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 


#define IVsProjectReference_get_Identity(This,pRetVal)	\
    ( (This)->lpVtbl -> get_Identity(This,pRetVal) ) 

#define IVsProjectReference_put_Identity(This,pRetVal)	\
    ( (This)->lpVtbl -> put_Identity(This,pRetVal) ) 

#define IVsProjectReference_get_ReferenceSpecification(This,pRetVal)	\
    ( (This)->lpVtbl -> get_ReferenceSpecification(This,pRetVal) ) 

#define IVsProjectReference_put_ReferenceSpecification(This,pRetVal)	\
    ( (This)->lpVtbl -> put_ReferenceSpecification(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0066 */
/* [local] */ 

DEFINE_GUID(GUID_ProjectReferenceProvider,  0x51ECA6BD, 0x5AE4, 0x43F0, 0xAA, 0x76, 0xDD, 0x0A, 0x7B, 0x08, 0xF4, 0x0C);


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0066_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0066_v0_0_s_ifspec;

#ifndef __IVsProjectReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsProjectReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsProjectReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3A77035E-CB2A-4582-B60E-1123207F2029")
    IVsProjectReferenceProviderContext : public IVsReferenceProviderContext
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentProject( 
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **pRetVal) = 0;
        
        virtual /* [propputref] */ HRESULT STDMETHODCALLTYPE putref_CurrentProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pRetVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsProjectReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentProject )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **pRetVal);
        
        /* [propputref] */ HRESULT ( STDMETHODCALLTYPE *putref_CurrentProject )( 
            __RPC__in IVsProjectReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pRetVal);
        
        END_INTERFACE
    } IVsProjectReferenceProviderContextVtbl;

    interface IVsProjectReferenceProviderContext
    {
        CONST_VTBL struct IVsProjectReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsProjectReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsProjectReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsProjectReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsProjectReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsProjectReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 


#define IVsProjectReferenceProviderContext_get_CurrentProject(This,pRetVal)	\
    ( (This)->lpVtbl -> get_CurrentProject(This,pRetVal) ) 

#define IVsProjectReferenceProviderContext_putref_CurrentProject(This,pRetVal)	\
    ( (This)->lpVtbl -> putref_CurrentProject(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsComReference_INTERFACE_DEFINED__
#define __IVsComReference_INTERFACE_DEFINED__

/* interface IVsComReference */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsComReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3B1D9F20-1152-44D2-851C-12A88DA14337")
    IVsComReference : public IVsReference
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Identity( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Identity( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Guid( 
            /* [retval][out] */ __RPC__out GUID *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Guid( 
            /* [in] */ GUID pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_MajorVersion( 
            /* [retval][out] */ __RPC__out WORD *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_MajorVersion( 
            /* [in] */ WORD pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_MinorVersion( 
            /* [retval][out] */ __RPC__out WORD *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_MinorVersion( 
            /* [in] */ WORD pRetVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsComReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsComReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsComReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsComReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsComReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsComReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsComReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Identity )( 
            __RPC__in IVsComReference * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Guid )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__out GUID *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Guid )( 
            __RPC__in IVsComReference * This,
            /* [in] */ GUID pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_MajorVersion )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__out WORD *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_MajorVersion )( 
            __RPC__in IVsComReference * This,
            /* [in] */ WORD pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_MinorVersion )( 
            __RPC__in IVsComReference * This,
            /* [retval][out] */ __RPC__out WORD *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_MinorVersion )( 
            __RPC__in IVsComReference * This,
            /* [in] */ WORD pRetVal);
        
        END_INTERFACE
    } IVsComReferenceVtbl;

    interface IVsComReference
    {
        CONST_VTBL struct IVsComReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsComReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsComReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsComReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsComReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsComReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 


#define IVsComReference_get_Identity(This,pRetVal)	\
    ( (This)->lpVtbl -> get_Identity(This,pRetVal) ) 

#define IVsComReference_put_Identity(This,pRetVal)	\
    ( (This)->lpVtbl -> put_Identity(This,pRetVal) ) 

#define IVsComReference_get_Guid(This,pRetVal)	\
    ( (This)->lpVtbl -> get_Guid(This,pRetVal) ) 

#define IVsComReference_put_Guid(This,pRetVal)	\
    ( (This)->lpVtbl -> put_Guid(This,pRetVal) ) 

#define IVsComReference_get_MajorVersion(This,pRetVal)	\
    ( (This)->lpVtbl -> get_MajorVersion(This,pRetVal) ) 

#define IVsComReference_put_MajorVersion(This,pRetVal)	\
    ( (This)->lpVtbl -> put_MajorVersion(This,pRetVal) ) 

#define IVsComReference_get_MinorVersion(This,pRetVal)	\
    ( (This)->lpVtbl -> get_MinorVersion(This,pRetVal) ) 

#define IVsComReference_put_MinorVersion(This,pRetVal)	\
    ( (This)->lpVtbl -> put_MinorVersion(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0068 */
/* [local] */ 

DEFINE_GUID(GUID_ComReferenceProvider,  0x4560BE15, 0x8871, 0x482A, 0x80, 0x1D, 0x76, 0xAA, 0x47, 0xF1, 0x76, 0x3A);


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0068_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0068_v0_0_s_ifspec;

#ifndef __IVsComReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsComReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsComReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsComReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8F310644-1D6A-4982-9D3D-C328A1384B43")
    IVsComReferenceProviderContext : public IVsReferenceProviderContext
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IVsComReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsComReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsComReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsComReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsComReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        END_INTERFACE
    } IVsComReferenceProviderContextVtbl;

    interface IVsComReferenceProviderContext
    {
        CONST_VTBL struct IVsComReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsComReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsComReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsComReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsComReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsComReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsPlatformReference_INTERFACE_DEFINED__
#define __IVsPlatformReference_INTERFACE_DEFINED__

/* interface IVsPlatformReference */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsPlatformReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6756F947-41FE-46C7-88F1-5CBE4606A5A0")
    IVsPlatformReference : public IVsReference
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SDKIdentity( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SDKIdentity( 
            /* [in] */ __RPC__in LPCOLESTR pRetVal) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsSDK( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pRetVal) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsSDK( 
            /* [in] */ VARIANT_BOOL pRetVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPlatformReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPlatformReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPlatformReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsPlatformReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsPlatformReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsPlatformReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SDKIdentity )( 
            __RPC__in IVsPlatformReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SDKIdentity )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ __RPC__in LPCOLESTR pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsSDK )( 
            __RPC__in IVsPlatformReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pRetVal);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsSDK )( 
            __RPC__in IVsPlatformReference * This,
            /* [in] */ VARIANT_BOOL pRetVal);
        
        END_INTERFACE
    } IVsPlatformReferenceVtbl;

    interface IVsPlatformReference
    {
        CONST_VTBL struct IVsPlatformReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPlatformReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPlatformReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPlatformReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPlatformReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsPlatformReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsPlatformReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsPlatformReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsPlatformReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsPlatformReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 


#define IVsPlatformReference_get_SDKIdentity(This,pRetVal)	\
    ( (This)->lpVtbl -> get_SDKIdentity(This,pRetVal) ) 

#define IVsPlatformReference_put_SDKIdentity(This,pRetVal)	\
    ( (This)->lpVtbl -> put_SDKIdentity(This,pRetVal) ) 

#define IVsPlatformReference_get_IsSDK(This,pRetVal)	\
    ( (This)->lpVtbl -> get_IsSDK(This,pRetVal) ) 

#define IVsPlatformReference_put_IsSDK(This,pRetVal)	\
    ( (This)->lpVtbl -> put_IsSDK(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPlatformReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0070 */
/* [local] */ 


enum __VSSDKPROVIDERTAB
    {
        TAB_SDK_PLATFORM	= 0x1,
        TAB_SDK_EXTENSIONS	= 0x2,
        TAB_SDK_ALL	= ( TAB_SDK_PLATFORM | TAB_SDK_EXTENSIONS ) 
    } ;
typedef DWORD VSSDKPROVIDERTAB;

DEFINE_GUID(GUID_PlatformReferenceProvider, 0x97324595, 0xE3F9, 0x4AA8, 0x85, 0xB7, 0xDC, 0x94, 0x1E, 0x81, 0x21, 0x52);


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0070_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0070_v0_0_s_ifspec;

#ifndef __IVsPlatformReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsPlatformReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsPlatformReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsPlatformReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E19BDA1E-41E3-47BA-8308-82D18F399D73")
    IVsPlatformReferenceProviderContext : public IVsReferenceProviderContext
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TargetPlatformIdentifier( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlatformIdentifier) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TargetPlatformIdentifier( 
            /* [in] */ __RPC__in LPCOLESTR strTargetPlatformIdentifier) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TargetPlatformVersion( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlaformVersion) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TargetPlatformVersion( 
            /* [in] */ __RPC__in LPCOLESTR strTargetPlaformVersion) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TargetFrameworkMoniker( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlatformMoniker) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TargetFrameworkMoniker( 
            /* [in] */ __RPC__in LPCOLESTR strTargetPlatformMoniker) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_AssemblySearchPaths( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblySearchPaths) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_AssemblySearchPaths( 
            /* [in] */ __RPC__in LPCOLESTR strAssemblySearchPaths) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TargetPlatformReferencesLocation( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformWinmdLocation) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TargetPlatformReferencesLocation( 
            /* [in] */ __RPC__in LPCOLESTR strPlatformWinmdLocation) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SDKRegistryRoot( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSdkRegistryRoot) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SDKRegistryRoot( 
            /* [in] */ __RPC__in LPCOLESTR strSdkRegistryRoot) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SDKDirectoryRoot( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSdkDirectoryRoot) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SDKDirectoryRoot( 
            /* [in] */ __RPC__in LPCOLESTR strSdkDirectoryRoot) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SDKFilterKeywords( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSDKFilterKeywords) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SDKFilterKeywords( 
            /* [in] */ __RPC__in LPCOLESTR strSDKFilterKeywords) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_VisualStudioVersion( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrVisualStudioVersion) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_VisualStudioVersion( 
            /* [in] */ __RPC__in LPCOLESTR strVisualStudioVersion) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ExpandSDKContents( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarBoolExpandSDKContents) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_ExpandSDKContents( 
            /* [in] */ VARIANT_BOOL varBoolExpandSDKContents) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Tabs( 
            /* [retval][out] */ __RPC__out VSSDKPROVIDERTAB *peTabs) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Tabs( 
            /* [in] */ VSSDKPROVIDERTAB eTabs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTabTitle( 
            /* [in] */ VSSDKPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTabTitle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTabTitle( 
            /* [in] */ VSSDKPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR szTabTitle) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsImplicitlyReferenced( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfIsImplicitlyExpanded) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_IsImplicitlyReferenced( 
            /* [in] */ VARIANT_BOOL pvarfIsImplicitlyExpanded) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNoItemsMessageForTab( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrNoItemsMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNoItemsMessageForTab( 
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR bstrNoItemsMessage) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPlatformReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPlatformReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPlatformReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetPlatformIdentifier )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlatformIdentifier);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetPlatformIdentifier )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strTargetPlatformIdentifier);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetPlatformVersion )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlaformVersion);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetPlatformVersion )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strTargetPlaformVersion);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetPlatformMoniker);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strTargetPlatformMoniker);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblySearchPaths )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrAssemblySearchPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblySearchPaths )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strAssemblySearchPaths);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetPlatformReferencesLocation )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPlatformWinmdLocation);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetPlatformReferencesLocation )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strPlatformWinmdLocation);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SDKRegistryRoot )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSdkRegistryRoot);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SDKRegistryRoot )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strSdkRegistryRoot);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SDKDirectoryRoot )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSdkDirectoryRoot);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SDKDirectoryRoot )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strSdkDirectoryRoot);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SDKFilterKeywords )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSDKFilterKeywords);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SDKFilterKeywords )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strSDKFilterKeywords);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_VisualStudioVersion )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrVisualStudioVersion);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_VisualStudioVersion )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR strVisualStudioVersion);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExpandSDKContents )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarBoolExpandSDKContents);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ExpandSDKContents )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VARIANT_BOOL varBoolExpandSDKContents);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Tabs )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VSSDKPROVIDERTAB *peTabs);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Tabs )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VSSDKPROVIDERTAB eTabs);
        
        HRESULT ( STDMETHODCALLTYPE *GetTabTitle )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VSSDKPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTabTitle);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabTitle )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VSSDKPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR szTabTitle);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsImplicitlyReferenced )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pvarfIsImplicitlyExpanded);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_IsImplicitlyReferenced )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VARIANT_BOOL pvarfIsImplicitlyExpanded);
        
        HRESULT ( STDMETHODCALLTYPE *GetNoItemsMessageForTab )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrNoItemsMessage);
        
        HRESULT ( STDMETHODCALLTYPE *SetNoItemsMessageForTab )( 
            __RPC__in IVsPlatformReferenceProviderContext * This,
            /* [in] */ VSASSEMBLYPROVIDERTAB etabId,
            /* [in] */ __RPC__in LPCOLESTR bstrNoItemsMessage);
        
        END_INTERFACE
    } IVsPlatformReferenceProviderContextVtbl;

    interface IVsPlatformReferenceProviderContext
    {
        CONST_VTBL struct IVsPlatformReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPlatformReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPlatformReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPlatformReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPlatformReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsPlatformReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsPlatformReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsPlatformReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsPlatformReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsPlatformReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 


#define IVsPlatformReferenceProviderContext_get_TargetPlatformIdentifier(This,pbstrTargetPlatformIdentifier)	\
    ( (This)->lpVtbl -> get_TargetPlatformIdentifier(This,pbstrTargetPlatformIdentifier) ) 

#define IVsPlatformReferenceProviderContext_put_TargetPlatformIdentifier(This,strTargetPlatformIdentifier)	\
    ( (This)->lpVtbl -> put_TargetPlatformIdentifier(This,strTargetPlatformIdentifier) ) 

#define IVsPlatformReferenceProviderContext_get_TargetPlatformVersion(This,pbstrTargetPlaformVersion)	\
    ( (This)->lpVtbl -> get_TargetPlatformVersion(This,pbstrTargetPlaformVersion) ) 

#define IVsPlatformReferenceProviderContext_put_TargetPlatformVersion(This,strTargetPlaformVersion)	\
    ( (This)->lpVtbl -> put_TargetPlatformVersion(This,strTargetPlaformVersion) ) 

#define IVsPlatformReferenceProviderContext_get_TargetFrameworkMoniker(This,pbstrTargetPlatformMoniker)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,pbstrTargetPlatformMoniker) ) 

#define IVsPlatformReferenceProviderContext_put_TargetFrameworkMoniker(This,strTargetPlatformMoniker)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,strTargetPlatformMoniker) ) 

#define IVsPlatformReferenceProviderContext_get_AssemblySearchPaths(This,pbstrAssemblySearchPaths)	\
    ( (This)->lpVtbl -> get_AssemblySearchPaths(This,pbstrAssemblySearchPaths) ) 

#define IVsPlatformReferenceProviderContext_put_AssemblySearchPaths(This,strAssemblySearchPaths)	\
    ( (This)->lpVtbl -> put_AssemblySearchPaths(This,strAssemblySearchPaths) ) 

#define IVsPlatformReferenceProviderContext_get_TargetPlatformReferencesLocation(This,pbstrPlatformWinmdLocation)	\
    ( (This)->lpVtbl -> get_TargetPlatformReferencesLocation(This,pbstrPlatformWinmdLocation) ) 

#define IVsPlatformReferenceProviderContext_put_TargetPlatformReferencesLocation(This,strPlatformWinmdLocation)	\
    ( (This)->lpVtbl -> put_TargetPlatformReferencesLocation(This,strPlatformWinmdLocation) ) 

#define IVsPlatformReferenceProviderContext_get_SDKRegistryRoot(This,pbstrSdkRegistryRoot)	\
    ( (This)->lpVtbl -> get_SDKRegistryRoot(This,pbstrSdkRegistryRoot) ) 

#define IVsPlatformReferenceProviderContext_put_SDKRegistryRoot(This,strSdkRegistryRoot)	\
    ( (This)->lpVtbl -> put_SDKRegistryRoot(This,strSdkRegistryRoot) ) 

#define IVsPlatformReferenceProviderContext_get_SDKDirectoryRoot(This,pbstrSdkDirectoryRoot)	\
    ( (This)->lpVtbl -> get_SDKDirectoryRoot(This,pbstrSdkDirectoryRoot) ) 

#define IVsPlatformReferenceProviderContext_put_SDKDirectoryRoot(This,strSdkDirectoryRoot)	\
    ( (This)->lpVtbl -> put_SDKDirectoryRoot(This,strSdkDirectoryRoot) ) 

#define IVsPlatformReferenceProviderContext_get_SDKFilterKeywords(This,pbstrSDKFilterKeywords)	\
    ( (This)->lpVtbl -> get_SDKFilterKeywords(This,pbstrSDKFilterKeywords) ) 

#define IVsPlatformReferenceProviderContext_put_SDKFilterKeywords(This,strSDKFilterKeywords)	\
    ( (This)->lpVtbl -> put_SDKFilterKeywords(This,strSDKFilterKeywords) ) 

#define IVsPlatformReferenceProviderContext_get_VisualStudioVersion(This,pbstrVisualStudioVersion)	\
    ( (This)->lpVtbl -> get_VisualStudioVersion(This,pbstrVisualStudioVersion) ) 

#define IVsPlatformReferenceProviderContext_put_VisualStudioVersion(This,strVisualStudioVersion)	\
    ( (This)->lpVtbl -> put_VisualStudioVersion(This,strVisualStudioVersion) ) 

#define IVsPlatformReferenceProviderContext_get_ExpandSDKContents(This,pvarBoolExpandSDKContents)	\
    ( (This)->lpVtbl -> get_ExpandSDKContents(This,pvarBoolExpandSDKContents) ) 

#define IVsPlatformReferenceProviderContext_put_ExpandSDKContents(This,varBoolExpandSDKContents)	\
    ( (This)->lpVtbl -> put_ExpandSDKContents(This,varBoolExpandSDKContents) ) 

#define IVsPlatformReferenceProviderContext_get_Tabs(This,peTabs)	\
    ( (This)->lpVtbl -> get_Tabs(This,peTabs) ) 

#define IVsPlatformReferenceProviderContext_put_Tabs(This,eTabs)	\
    ( (This)->lpVtbl -> put_Tabs(This,eTabs) ) 

#define IVsPlatformReferenceProviderContext_GetTabTitle(This,etabId,pbstrTabTitle)	\
    ( (This)->lpVtbl -> GetTabTitle(This,etabId,pbstrTabTitle) ) 

#define IVsPlatformReferenceProviderContext_SetTabTitle(This,etabId,szTabTitle)	\
    ( (This)->lpVtbl -> SetTabTitle(This,etabId,szTabTitle) ) 

#define IVsPlatformReferenceProviderContext_get_IsImplicitlyReferenced(This,pvarfIsImplicitlyExpanded)	\
    ( (This)->lpVtbl -> get_IsImplicitlyReferenced(This,pvarfIsImplicitlyExpanded) ) 

#define IVsPlatformReferenceProviderContext_put_IsImplicitlyReferenced(This,pvarfIsImplicitlyExpanded)	\
    ( (This)->lpVtbl -> put_IsImplicitlyReferenced(This,pvarfIsImplicitlyExpanded) ) 

#define IVsPlatformReferenceProviderContext_GetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage)	\
    ( (This)->lpVtbl -> GetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage) ) 

#define IVsPlatformReferenceProviderContext_SetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage)	\
    ( (This)->lpVtbl -> SetNoItemsMessageForTab(This,etabId,bstrNoItemsMessage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPlatformReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsFileReference_INTERFACE_DEFINED__
#define __IVsFileReference_INTERFACE_DEFINED__

/* interface IVsFileReference */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsFileReference;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DEB39E27-EF55-4626-A586-24FBDD918792")
    IVsFileReference : public IVsReference
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFileReferenceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFileReference * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFileReference * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFileReference * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsFileReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrName);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            __RPC__in IVsFileReference * This,
            /* [in] */ __RPC__in LPCOLESTR strName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in IVsFileReference * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFullPath);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_FullPath )( 
            __RPC__in IVsFileReference * This,
            /* [in] */ __RPC__in LPCOLESTR bstrFullPath);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AlreadyReferenced )( 
            __RPC__in IVsFileReference * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolAlreadyReferenced);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_AlreadyReferenced )( 
            __RPC__in IVsFileReference * This,
            /* [in] */ VARIANT_BOOL boolAlreadyReferenced);
        
        END_INTERFACE
    } IVsFileReferenceVtbl;

    interface IVsFileReference
    {
        CONST_VTBL struct IVsFileReferenceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileReference_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileReference_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileReference_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileReference_get_Name(This,bstrName)	\
    ( (This)->lpVtbl -> get_Name(This,bstrName) ) 

#define IVsFileReference_put_Name(This,strName)	\
    ( (This)->lpVtbl -> put_Name(This,strName) ) 

#define IVsFileReference_get_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> get_FullPath(This,bstrFullPath) ) 

#define IVsFileReference_put_FullPath(This,bstrFullPath)	\
    ( (This)->lpVtbl -> put_FullPath(This,bstrFullPath) ) 

#define IVsFileReference_get_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> get_AlreadyReferenced(This,boolAlreadyReferenced) ) 

#define IVsFileReference_put_AlreadyReferenced(This,boolAlreadyReferenced)	\
    ( (This)->lpVtbl -> put_AlreadyReferenced(This,boolAlreadyReferenced) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileReference_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0072 */
/* [local] */ 

DEFINE_GUID(GUID_FileReferenceProvider, 0x7B069159, 0xFF02, 0x4752, 0x93, 0xE8, 0x96, 0xB3, 0xCA, 0xDF, 0x44, 0x1A);


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0072_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0072_v0_0_s_ifspec;

#ifndef __IVsFileReferenceProviderContext_INTERFACE_DEFINED__
#define __IVsFileReferenceProviderContext_INTERFACE_DEFINED__

/* interface IVsFileReferenceProviderContext */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsFileReferenceProviderContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1D594BAA-05BC-4882-B118-4CA95AD56E41")
    IVsFileReferenceProviderContext : public IVsReferenceProviderContext
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_BrowseFilter( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBrowseFilter) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_BrowseFilter( 
            /* [in] */ __RPC__in LPCOLESTR bstrBrowseFilter) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DefaultBrowseLocation( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultBrowseLocation) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_DefaultBrowseLocation( 
            /* [in] */ __RPC__in LPCOLESTR bstrDefaultBrowseLocation) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFileReferenceProviderContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFileReferenceProviderContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFileReferenceProviderContext * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ProviderGuid )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [retval][out] */ __RPC__out GUID *pguidProvider);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_References )( 
            IVsFileReferenceProviderContext * This,
            /* [retval][out] */ SAFEARRAY * *pReferences);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [in] */ __RPC__in_opt IVsReference *pReference);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReference )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReference **pRetVal);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ReferenceFilterPaths )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pFilterPaths);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_ReferenceFilterPaths )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [in] */ __RPC__in SAFEARRAY * filterPaths);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseFilter )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBrowseFilter);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseFilter )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR bstrBrowseFilter);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultBrowseLocation )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDefaultBrowseLocation);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultBrowseLocation )( 
            __RPC__in IVsFileReferenceProviderContext * This,
            /* [in] */ __RPC__in LPCOLESTR bstrDefaultBrowseLocation);
        
        END_INTERFACE
    } IVsFileReferenceProviderContextVtbl;

    interface IVsFileReferenceProviderContext
    {
        CONST_VTBL struct IVsFileReferenceProviderContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileReferenceProviderContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileReferenceProviderContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileReferenceProviderContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileReferenceProviderContext_get_ProviderGuid(This,pguidProvider)	\
    ( (This)->lpVtbl -> get_ProviderGuid(This,pguidProvider) ) 

#define IVsFileReferenceProviderContext_get_References(This,pReferences)	\
    ( (This)->lpVtbl -> get_References(This,pReferences) ) 

#define IVsFileReferenceProviderContext_AddReference(This,pReference)	\
    ( (This)->lpVtbl -> AddReference(This,pReference) ) 

#define IVsFileReferenceProviderContext_CreateReference(This,pRetVal)	\
    ( (This)->lpVtbl -> CreateReference(This,pRetVal) ) 

#define IVsFileReferenceProviderContext_get_ReferenceFilterPaths(This,pFilterPaths)	\
    ( (This)->lpVtbl -> get_ReferenceFilterPaths(This,pFilterPaths) ) 

#define IVsFileReferenceProviderContext_put_ReferenceFilterPaths(This,filterPaths)	\
    ( (This)->lpVtbl -> put_ReferenceFilterPaths(This,filterPaths) ) 


#define IVsFileReferenceProviderContext_get_BrowseFilter(This,pbstrBrowseFilter)	\
    ( (This)->lpVtbl -> get_BrowseFilter(This,pbstrBrowseFilter) ) 

#define IVsFileReferenceProviderContext_put_BrowseFilter(This,bstrBrowseFilter)	\
    ( (This)->lpVtbl -> put_BrowseFilter(This,bstrBrowseFilter) ) 

#define IVsFileReferenceProviderContext_get_DefaultBrowseLocation(This,pbstrDefaultBrowseLocation)	\
    ( (This)->lpVtbl -> get_DefaultBrowseLocation(This,pbstrDefaultBrowseLocation) ) 

#define IVsFileReferenceProviderContext_put_DefaultBrowseLocation(This,bstrDefaultBrowseLocation)	\
    ( (This)->lpVtbl -> put_DefaultBrowseLocation(This,bstrDefaultBrowseLocation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileReferenceProviderContext_INTERFACE_DEFINED__ */


#ifndef __IVsReferenceManagerUser_INTERFACE_DEFINED__
#define __IVsReferenceManagerUser_INTERFACE_DEFINED__

/* interface IVsReferenceManagerUser */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsReferenceManagerUser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3AD23841-9A64-4582-B226-E4CFE3942446")
    IVsReferenceManagerUser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ChangeReferences( 
            /* [in] */ VSREFERENCECHANGEOPERATION operation,
            /* [in] */ __RPC__in_opt IVsReferenceProviderContext *changedContext) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetProviderContexts( 
            /* [retval][out] */ SAFEARRAY * *rgProviderContexts) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsReferenceManagerUserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsReferenceManagerUser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsReferenceManagerUser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsReferenceManagerUser * This);
        
        HRESULT ( STDMETHODCALLTYPE *ChangeReferences )( 
            __RPC__in IVsReferenceManagerUser * This,
            /* [in] */ VSREFERENCECHANGEOPERATION operation,
            /* [in] */ __RPC__in_opt IVsReferenceProviderContext *changedContext);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetProviderContexts )( 
            IVsReferenceManagerUser * This,
            /* [retval][out] */ SAFEARRAY * *rgProviderContexts);
        
        END_INTERFACE
    } IVsReferenceManagerUserVtbl;

    interface IVsReferenceManagerUser
    {
        CONST_VTBL struct IVsReferenceManagerUserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsReferenceManagerUser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsReferenceManagerUser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsReferenceManagerUser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsReferenceManagerUser_ChangeReferences(This,operation,changedContext)	\
    ( (This)->lpVtbl -> ChangeReferences(This,operation,changedContext) ) 

#define IVsReferenceManagerUser_GetProviderContexts(This,rgProviderContexts)	\
    ( (This)->lpVtbl -> GetProviderContexts(This,rgProviderContexts) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsReferenceManagerUser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0074 */
/* [local] */ 


enum __VSREFERENCEQUERYRESULT
    {
        REFERENCE_ALLOW	= 0,
        REFERENCE_DENY	= ( REFERENCE_ALLOW + 1 ) ,
        REFERENCE_UNKNOWN	= ( REFERENCE_DENY + 1 ) 
    } ;
typedef DWORD VSREFERENCEQUERYRESULT;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0074_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0074_v0_0_s_ifspec;

#ifndef __IVsReferenceManager_INTERFACE_DEFINED__
#define __IVsReferenceManager_INTERFACE_DEFINED__

/* interface IVsReferenceManager */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsReferenceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("88E00A52-3B22-45C4-8FA0-321319DDA03F")
    IVsReferenceManager : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE ShowReferenceManager( 
            /* [in] */ IVsReferenceManagerUser *pRefMgrUser,
            /* [in] */ LPCWSTR lpszDlgTitle,
            /* [in] */ LPCWSTR lpszHelpTopic,
            /* [in] */ GUID guidDefaultProviderContext,
            /* [in] */ VARIANT_BOOL fForceShowDefaultProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateProviderContext( 
            /* [in] */ GUID guidProvider,
            /* [retval][out] */ __RPC__deref_out_opt IVsReferenceProviderContext **pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryCanReferenceProject( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pReferencing,
            /* [in] */ __RPC__in_opt IVsHierarchy *pReferenced,
            /* [retval][out] */ __RPC__out VSREFERENCEQUERYRESULT *pQueryResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsReferenceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsReferenceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsReferenceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsReferenceManager * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *ShowReferenceManager )( 
            IVsReferenceManager * This,
            /* [in] */ IVsReferenceManagerUser *pRefMgrUser,
            /* [in] */ LPCWSTR lpszDlgTitle,
            /* [in] */ LPCWSTR lpszHelpTopic,
            /* [in] */ GUID guidDefaultProviderContext,
            /* [in] */ VARIANT_BOOL fForceShowDefaultProvider);
        
        HRESULT ( STDMETHODCALLTYPE *CreateProviderContext )( 
            __RPC__in IVsReferenceManager * This,
            /* [in] */ GUID guidProvider,
            /* [retval][out] */ __RPC__deref_out_opt IVsReferenceProviderContext **pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCanReferenceProject )( 
            __RPC__in IVsReferenceManager * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pReferencing,
            /* [in] */ __RPC__in_opt IVsHierarchy *pReferenced,
            /* [retval][out] */ __RPC__out VSREFERENCEQUERYRESULT *pQueryResult);
        
        END_INTERFACE
    } IVsReferenceManagerVtbl;

    interface IVsReferenceManager
    {
        CONST_VTBL struct IVsReferenceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsReferenceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsReferenceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsReferenceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsReferenceManager_ShowReferenceManager(This,pRefMgrUser,lpszDlgTitle,lpszHelpTopic,guidDefaultProviderContext,fForceShowDefaultProvider)	\
    ( (This)->lpVtbl -> ShowReferenceManager(This,pRefMgrUser,lpszDlgTitle,lpszHelpTopic,guidDefaultProviderContext,fForceShowDefaultProvider) ) 

#define IVsReferenceManager_CreateProviderContext(This,guidProvider,pRetVal)	\
    ( (This)->lpVtbl -> CreateProviderContext(This,guidProvider,pRetVal) ) 

#define IVsReferenceManager_QueryCanReferenceProject(This,pReferencing,pReferenced,pQueryResult)	\
    ( (This)->lpVtbl -> QueryCanReferenceProject(This,pReferencing,pReferenced,pQueryResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsReferenceManager_INTERFACE_DEFINED__ */


#ifndef __IVsReferenceManager2_INTERFACE_DEFINED__
#define __IVsReferenceManager2_INTERFACE_DEFINED__

/* interface IVsReferenceManager2 */
/* [object][oleautomation][version][uuid] */ 


EXTERN_C const IID IID_IVsReferenceManager2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B7821F84-D9C8-48F9-93C4-3C4C98A477A1")
    IVsReferenceManager2 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetSDKReferenceDependencies( 
            /* [in] */ LPCWSTR SDKIdentifier,
            /* [in] */ IVsPlatformReferenceProviderContext *pContext,
            /* [retval][out] */ SAFEARRAY * *pDependencies) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsReferenceManager2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsReferenceManager2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsReferenceManager2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsReferenceManager2 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetSDKReferenceDependencies )( 
            IVsReferenceManager2 * This,
            /* [in] */ LPCWSTR SDKIdentifier,
            /* [in] */ IVsPlatformReferenceProviderContext *pContext,
            /* [retval][out] */ SAFEARRAY * *pDependencies);
        
        END_INTERFACE
    } IVsReferenceManager2Vtbl;

    interface IVsReferenceManager2
    {
        CONST_VTBL struct IVsReferenceManager2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsReferenceManager2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsReferenceManager2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsReferenceManager2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsReferenceManager2_GetSDKReferenceDependencies(This,SDKIdentifier,pContext,pDependencies)	\
    ( (This)->lpVtbl -> GetSDKReferenceDependencies(This,SDKIdentifier,pContext,pDependencies) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsReferenceManager2_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorReferenceManager_INTERFACE_DEFINED__
#define __IVsProjectFlavorReferenceManager_INTERFACE_DEFINED__

/* interface IVsProjectFlavorReferenceManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorReferenceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B42565FE-1FE7-418E-B3F0-CF56C5450E80")
    IVsProjectFlavorReferenceManager : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE ShowReferenceManager( 
            /* [in] */ IVsReferenceManager *pRefMgr,
            /* [in] */ IVsReferenceManagerUser *pRefMgrUser,
            /* [in] */ LPCWSTR lpszDlgTitle,
            /* [in] */ LPCWSTR lpszHelpTopic,
            /* [in] */ GUID guidDefaultProviderContext,
            /* [in] */ VARIANT_BOOL fForceShowDefaultProvider) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorReferenceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFlavorReferenceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFlavorReferenceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFlavorReferenceManager * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *ShowReferenceManager )( 
            IVsProjectFlavorReferenceManager * This,
            /* [in] */ IVsReferenceManager *pRefMgr,
            /* [in] */ IVsReferenceManagerUser *pRefMgrUser,
            /* [in] */ LPCWSTR lpszDlgTitle,
            /* [in] */ LPCWSTR lpszHelpTopic,
            /* [in] */ GUID guidDefaultProviderContext,
            /* [in] */ VARIANT_BOOL fForceShowDefaultProvider);
        
        END_INTERFACE
    } IVsProjectFlavorReferenceManagerVtbl;

    interface IVsProjectFlavorReferenceManager
    {
        CONST_VTBL struct IVsProjectFlavorReferenceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorReferenceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorReferenceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorReferenceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorReferenceManager_ShowReferenceManager(This,pRefMgr,pRefMgrUser,lpszDlgTitle,lpszHelpTopic,guidDefaultProviderContext,fForceShowDefaultProvider)	\
    ( (This)->lpVtbl -> ShowReferenceManager(This,pRefMgr,pRefMgrUser,lpszDlgTitle,lpszHelpTopic,guidDefaultProviderContext,fForceShowDefaultProvider) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorReferenceManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0077 */
/* [local] */ 


enum __VSQUERYFLAVORREFERENCESCONTEXT
    {
        VSQUERYFLAVORREFERENCESCONTEXT_AddReference	= 0,
        VSQUERYFLAVORREFERENCESCONTEXT_LoadReference	= 1,
        VSQUERYFLAVORREFERENCESCONTEXT_RefreshReference	= 2
    } ;
typedef LONG VSQUERYFLAVORREFERENCESCONTEXT;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0077_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0077_v0_0_s_ifspec;

#ifndef __IVsProjectFlavorReferences3_INTERFACE_DEFINED__
#define __IVsProjectFlavorReferences3_INTERFACE_DEFINED__

/* interface IVsProjectFlavorReferences3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorReferences3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("005431fc-cf9c-4154-9b28-ec975d88948d")
    IVsProjectFlavorReferences3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryAddProjectReferenceEx( 
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [in] */ VSQUERYFLAVORREFERENCESCONTEXT queryContext,
            /* [out] */ __RPC__out VSREFERENCEQUERYRESULT *pResult,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstreReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryCanBeReferencedEx( 
            /* [in] */ __RPC__in_opt IUnknown *pReferencingProject,
            /* [in] */ VSQUERYFLAVORREFERENCESCONTEXT queryContext,
            /* [out] */ __RPC__out VSREFERENCEQUERYRESULT *pResult,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstreReason) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorReferences3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFlavorReferences3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFlavorReferences3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFlavorReferences3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryAddProjectReferenceEx )( 
            __RPC__in IVsProjectFlavorReferences3 * This,
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [in] */ VSQUERYFLAVORREFERENCESCONTEXT queryContext,
            /* [out] */ __RPC__out VSREFERENCEQUERYRESULT *pResult,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstreReason);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCanBeReferencedEx )( 
            __RPC__in IVsProjectFlavorReferences3 * This,
            /* [in] */ __RPC__in_opt IUnknown *pReferencingProject,
            /* [in] */ VSQUERYFLAVORREFERENCESCONTEXT queryContext,
            /* [out] */ __RPC__out VSREFERENCEQUERYRESULT *pResult,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstreReason);
        
        END_INTERFACE
    } IVsProjectFlavorReferences3Vtbl;

    interface IVsProjectFlavorReferences3
    {
        CONST_VTBL struct IVsProjectFlavorReferences3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorReferences3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorReferences3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorReferences3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorReferences3_QueryAddProjectReferenceEx(This,pReferencedProject,queryContext,pResult,pbstreReason)	\
    ( (This)->lpVtbl -> QueryAddProjectReferenceEx(This,pReferencedProject,queryContext,pResult,pbstreReason) ) 

#define IVsProjectFlavorReferences3_QueryCanBeReferencedEx(This,pReferencingProject,queryContext,pResult,pbstreReason)	\
    ( (This)->lpVtbl -> QueryCanBeReferencedEx(This,pReferencingProject,queryContext,pResult,pbstreReason) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorReferences3_INTERFACE_DEFINED__ */


#ifndef __IVsAppCompat_INTERFACE_DEFINED__
#define __IVsAppCompat_INTERFACE_DEFINED__

/* interface IVsAppCompat */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppCompat;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F9810E7-36BA-4986-938B-F7E14EE02F9A")
    IVsAppCompat : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE AskForUserConsentToBreakAssetCompat( 
            /* [in] */ SAFEARRAY * sarrProjectHierarchies) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BreakAssetCompatibility( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pProjHier,
            /* [in] */ __RPC__in LPCOLESTR lpszMinimumVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentDesignTimeCompatVersion( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDesignTimeCompatVersion) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppCompatVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppCompat * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppCompat * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppCompat * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *AskForUserConsentToBreakAssetCompat )( 
            IVsAppCompat * This,
            /* [in] */ SAFEARRAY * sarrProjectHierarchies);
        
        HRESULT ( STDMETHODCALLTYPE *BreakAssetCompatibility )( 
            __RPC__in IVsAppCompat * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pProjHier,
            /* [in] */ __RPC__in LPCOLESTR lpszMinimumVersion);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentDesignTimeCompatVersion )( 
            __RPC__in IVsAppCompat * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDesignTimeCompatVersion);
        
        END_INTERFACE
    } IVsAppCompatVtbl;

    interface IVsAppCompat
    {
        CONST_VTBL struct IVsAppCompatVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppCompat_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppCompat_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppCompat_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppCompat_AskForUserConsentToBreakAssetCompat(This,sarrProjectHierarchies)	\
    ( (This)->lpVtbl -> AskForUserConsentToBreakAssetCompat(This,sarrProjectHierarchies) ) 

#define IVsAppCompat_BreakAssetCompatibility(This,pProjHier,lpszMinimumVersion)	\
    ( (This)->lpVtbl -> BreakAssetCompatibility(This,pProjHier,lpszMinimumVersion) ) 

#define IVsAppCompat_GetCurrentDesignTimeCompatVersion(This,pbstrCurrentDesignTimeCompatVersion)	\
    ( (This)->lpVtbl -> GetCurrentDesignTimeCompatVersion(This,pbstrCurrentDesignTimeCompatVersion) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppCompat_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0079 */
/* [local] */ 

typedef VsResolvedAssemblyPath *PVsResolvedAssemblyPath;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0079_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0079_v0_0_s_ifspec;

#ifndef __IVsDesignTimeAssemblyResolution2_INTERFACE_DEFINED__
#define __IVsDesignTimeAssemblyResolution2_INTERFACE_DEFINED__

/* interface IVsDesignTimeAssemblyResolution2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDesignTimeAssemblyResolution2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C0031543-0B20-4FA7-B627-8ED28F4B5F64")
    IVsDesignTimeAssemblyResolution2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPathInTargetFx2( 
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) LPCWSTR prgAssemblySpecs[  ],
            /* [in] */ ULONG cAssembliesToResolve,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDesignTimeAssemblyResolution2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDesignTimeAssemblyResolution2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDesignTimeAssemblyResolution2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDesignTimeAssemblyResolution2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPathInTargetFx2 )( 
            __RPC__in IVsDesignTimeAssemblyResolution2 * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) LPCWSTR prgAssemblySpecs[  ],
            /* [in] */ ULONG cAssembliesToResolve,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths);
        
        END_INTERFACE
    } IVsDesignTimeAssemblyResolution2Vtbl;

    interface IVsDesignTimeAssemblyResolution2
    {
        CONST_VTBL struct IVsDesignTimeAssemblyResolution2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDesignTimeAssemblyResolution2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDesignTimeAssemblyResolution2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDesignTimeAssemblyResolution2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDesignTimeAssemblyResolution2_ResolveAssemblyPathInTargetFx2(This,prgAssemblySpecs,cAssembliesToResolve,ignoreVersionForFrameworkReferences,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths)	\
    ( (This)->lpVtbl -> ResolveAssemblyPathInTargetFx2(This,prgAssemblySpecs,cAssembliesToResolve,ignoreVersionForFrameworkReferences,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDesignTimeAssemblyResolution2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0080 */
/* [local] */ 

typedef /* [public][public] */ 
enum __MIDL___MIDL_itf_vsshell110_0000_0080_0001
    {
        VSBUILDMANAGERRESOURCE_DESIGNTIME	= 0x1,
        VSBUILDMANAGERRESOURCE_UITHREAD	= 0x2
    } 	VSBUILDMANAGERRESOURCE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0080_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0080_v0_0_s_ifspec;

#ifndef __IVsBuildManagerAccessor2_INTERFACE_DEFINED__
#define __IVsBuildManagerAccessor2_INTERFACE_DEFINED__

/* interface IVsBuildManagerAccessor2 */
/* [object][custom][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildManagerAccessor2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BC89279F-B8AE-45DC-A171-52B8B8BA7945")
    IVsBuildManagerAccessor2 : public IVsBuildManagerAccessor
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DesignTimeBuildAvailable( 
            /* [retval][out] */ HANDLE *phWaitHandle) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_UIThreadIsAvailableForBuild( 
            /* [retval][out] */ HANDLE *phWaitHandle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AcquireBuildResources( 
            /* [in] */ VSBUILDMANAGERRESOURCE fResources,
            /* [out] */ VSCOOKIE *phCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseBuildResources( 
            /* [in] */ VSCOOKIE hCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBuildManagerAccessor2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsBuildManagerAccessor2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsBuildManagerAccessor2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterLogger )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ LONG submissionId,
            /* [in] */ IUnknown *punkLogger);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterLoggers )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ LONG submissionId);
        
        HRESULT ( STDMETHODCALLTYPE *ClaimUIThreadForBuild )( 
            IVsBuildManagerAccessor2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseUIThreadForBuild )( 
            IVsBuildManagerAccessor2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginDesignTimeBuild )( 
            IVsBuildManagerAccessor2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndDesignTimeBuild )( 
            IVsBuildManagerAccessor2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentBatchBuildId )( 
            IVsBuildManagerAccessor2 * This,
            /* [out] */ ULONG *pBatchId);
        
        HRESULT ( STDMETHODCALLTYPE *GetSolutionConfiguration )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ IUnknown *punkRootProject,
            /* [retval][out] */ BSTR *pbstrXmlFragment);
        
        HRESULT ( STDMETHODCALLTYPE *Escape )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ LPCOLESTR pwszUnescapedValue,
            /* [retval][out] */ BSTR *pbstrEscapedValue);
        
        HRESULT ( STDMETHODCALLTYPE *Unescape )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ LPCOLESTR pwszEscapedValue,
            /* [retval][out] */ BSTR *pbstrUnescapedValue);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DesignTimeBuildAvailable )( 
            IVsBuildManagerAccessor2 * This,
            /* [retval][out] */ HANDLE *phWaitHandle);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_UIThreadIsAvailableForBuild )( 
            IVsBuildManagerAccessor2 * This,
            /* [retval][out] */ HANDLE *phWaitHandle);
        
        HRESULT ( STDMETHODCALLTYPE *AcquireBuildResources )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ VSBUILDMANAGERRESOURCE fResources,
            /* [out] */ VSCOOKIE *phCookie);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseBuildResources )( 
            IVsBuildManagerAccessor2 * This,
            /* [in] */ VSCOOKIE hCookie);
        
        END_INTERFACE
    } IVsBuildManagerAccessor2Vtbl;

    interface IVsBuildManagerAccessor2
    {
        CONST_VTBL struct IVsBuildManagerAccessor2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildManagerAccessor2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildManagerAccessor2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildManagerAccessor2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildManagerAccessor2_RegisterLogger(This,submissionId,punkLogger)	\
    ( (This)->lpVtbl -> RegisterLogger(This,submissionId,punkLogger) ) 

#define IVsBuildManagerAccessor2_UnregisterLoggers(This,submissionId)	\
    ( (This)->lpVtbl -> UnregisterLoggers(This,submissionId) ) 

#define IVsBuildManagerAccessor2_ClaimUIThreadForBuild(This)	\
    ( (This)->lpVtbl -> ClaimUIThreadForBuild(This) ) 

#define IVsBuildManagerAccessor2_ReleaseUIThreadForBuild(This)	\
    ( (This)->lpVtbl -> ReleaseUIThreadForBuild(This) ) 

#define IVsBuildManagerAccessor2_BeginDesignTimeBuild(This)	\
    ( (This)->lpVtbl -> BeginDesignTimeBuild(This) ) 

#define IVsBuildManagerAccessor2_EndDesignTimeBuild(This)	\
    ( (This)->lpVtbl -> EndDesignTimeBuild(This) ) 

#define IVsBuildManagerAccessor2_GetCurrentBatchBuildId(This,pBatchId)	\
    ( (This)->lpVtbl -> GetCurrentBatchBuildId(This,pBatchId) ) 

#define IVsBuildManagerAccessor2_GetSolutionConfiguration(This,punkRootProject,pbstrXmlFragment)	\
    ( (This)->lpVtbl -> GetSolutionConfiguration(This,punkRootProject,pbstrXmlFragment) ) 

#define IVsBuildManagerAccessor2_Escape(This,pwszUnescapedValue,pbstrEscapedValue)	\
    ( (This)->lpVtbl -> Escape(This,pwszUnescapedValue,pbstrEscapedValue) ) 

#define IVsBuildManagerAccessor2_Unescape(This,pwszEscapedValue,pbstrUnescapedValue)	\
    ( (This)->lpVtbl -> Unescape(This,pwszEscapedValue,pbstrUnescapedValue) ) 


#define IVsBuildManagerAccessor2_get_DesignTimeBuildAvailable(This,phWaitHandle)	\
    ( (This)->lpVtbl -> get_DesignTimeBuildAvailable(This,phWaitHandle) ) 

#define IVsBuildManagerAccessor2_get_UIThreadIsAvailableForBuild(This,phWaitHandle)	\
    ( (This)->lpVtbl -> get_UIThreadIsAvailableForBuild(This,phWaitHandle) ) 

#define IVsBuildManagerAccessor2_AcquireBuildResources(This,fResources,phCookie)	\
    ( (This)->lpVtbl -> AcquireBuildResources(This,fResources,phCookie) ) 

#define IVsBuildManagerAccessor2_ReleaseBuildResources(This,hCookie)	\
    ( (This)->lpVtbl -> ReleaseBuildResources(This,hCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildManagerAccessor2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0081 */
/* [local] */ 


enum __VSDBGLAUNCHFLAGS6
    {
        DBGLAUNCH_BlockCredentialsDialog	= 0x10000,
        DBGLAUNCH_BlockWWSDialog	= 0x20000,
        DBGLAUNCH_StartInSimulator	= 0x40000
    } ;
typedef DWORD VSDBGLAUNCHFLAGS6;


enum _DEBUG_LAUNCH_OPERATION4
    {
        DLO_AppPackageDebug	= 7,
        DLO_AttachToSuspendedLaunchProcess	= 8
    } ;
typedef DWORD DEBUG_LAUNCH_OPERATION4;


enum VsAppPackagePlatform
    {
        APPPLAT_WindowsAppx	= 0,
        APPPLAT_WindowsPhoneXAP	= ( APPPLAT_WindowsAppx + 1 ) 
    } ;
typedef struct _VsAppPackageLaunchInfo
    {
    BSTR PackageMoniker;
    BSTR AppUserModelID;
    enum VsAppPackagePlatform AppPlatform;
    } 	VsAppPackageLaunchInfo;

typedef struct _VsDebugTargetInfo4
    {
    DEBUG_LAUNCH_OPERATION4 dlo;
    VSDBGLAUNCHFLAGS6 LaunchFlags;
    BSTR bstrRemoteMachine;
    BSTR bstrExe;
    BSTR bstrArg;
    BSTR bstrCurDir;
    BSTR bstrEnv;
    DWORD dwProcessId;
    VsDebugStartupInfo *pStartupInfo;
    GUID guidLaunchDebugEngine;
    DWORD dwDebugEngineCount;
    /* [size_is] */ GUID *pDebugEngines;
    GUID guidPortSupplier;
    BSTR bstrPortName;
    BSTR bstrOptions;
    BOOL fSendToOutputWindow;
    IUnknown *pUnknown;
    GUID guidProcessLanguage;
    VsAppPackageLaunchInfo AppPackageLaunchInfo;
    IVsHierarchy *project;
    } 	VsDebugTargetInfo4;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0081_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0081_v0_0_s_ifspec;

#ifndef __IVsBackForwardNavigation2_INTERFACE_DEFINED__
#define __IVsBackForwardNavigation2_INTERFACE_DEFINED__

/* interface IVsBackForwardNavigation2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsBackForwardNavigation2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6366f292-8597-4e66-b332-c9d00d4367b8")
    IVsBackForwardNavigation2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RequestAddNavigationItem( 
            /* [in] */ __RPC__in_opt IVsWindowFrame *frame,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *added) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBackForwardNavigation2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBackForwardNavigation2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBackForwardNavigation2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBackForwardNavigation2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RequestAddNavigationItem )( 
            __RPC__in IVsBackForwardNavigation2 * This,
            /* [in] */ __RPC__in_opt IVsWindowFrame *frame,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *added);
        
        END_INTERFACE
    } IVsBackForwardNavigation2Vtbl;

    interface IVsBackForwardNavigation2
    {
        CONST_VTBL struct IVsBackForwardNavigation2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBackForwardNavigation2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBackForwardNavigation2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBackForwardNavigation2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBackForwardNavigation2_RequestAddNavigationItem(This,frame,added)	\
    ( (This)->lpVtbl -> RequestAddNavigationItem(This,frame,added) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBackForwardNavigation2_INTERFACE_DEFINED__ */


#ifndef __IVsSerializeNavigationItem_INTERFACE_DEFINED__
#define __IVsSerializeNavigationItem_INTERFACE_DEFINED__

/* interface IVsSerializeNavigationItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSerializeNavigationItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00757da7-aae5-4fe4-9fdf-2e85364e228b")
    IVsSerializeNavigationItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Serialize( 
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [in] */ __RPC__in_opt IStream *pStream,
            /* [in] */ __RPC__in_opt IUnknown *punkObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Deserialize( 
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [in] */ __RPC__in_opt IStream *pStream,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunkObject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSerializeNavigationItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSerializeNavigationItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSerializeNavigationItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSerializeNavigationItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *Serialize )( 
            __RPC__in IVsSerializeNavigationItem * This,
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [in] */ __RPC__in_opt IStream *pStream,
            /* [in] */ __RPC__in_opt IUnknown *punkObject);
        
        HRESULT ( STDMETHODCALLTYPE *Deserialize )( 
            __RPC__in IVsSerializeNavigationItem * This,
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [in] */ __RPC__in_opt IStream *pStream,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunkObject);
        
        END_INTERFACE
    } IVsSerializeNavigationItemVtbl;

    interface IVsSerializeNavigationItem
    {
        CONST_VTBL struct IVsSerializeNavigationItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSerializeNavigationItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSerializeNavigationItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSerializeNavigationItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSerializeNavigationItem_Serialize(This,pFrame,pStream,punkObject)	\
    ( (This)->lpVtbl -> Serialize(This,pFrame,pStream,punkObject) ) 

#define IVsSerializeNavigationItem_Deserialize(This,pFrame,pStream,ppunkObject)	\
    ( (This)->lpVtbl -> Deserialize(This,pFrame,pStream,ppunkObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSerializeNavigationItem_INTERFACE_DEFINED__ */


#ifndef __IVsDynamicNavigationItem_INTERFACE_DEFINED__
#define __IVsDynamicNavigationItem_INTERFACE_DEFINED__

/* interface IVsDynamicNavigationItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsDynamicNavigationItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3304fdef-9284-4340-b6b7-37ce11167c8c")
    IVsDynamicNavigationItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDynamicNavigationItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDynamicNavigationItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDynamicNavigationItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDynamicNavigationItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            __RPC__in IVsDynamicNavigationItem * This,
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        END_INTERFACE
    } IVsDynamicNavigationItemVtbl;

    interface IVsDynamicNavigationItem
    {
        CONST_VTBL struct IVsDynamicNavigationItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDynamicNavigationItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDynamicNavigationItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDynamicNavigationItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDynamicNavigationItem_GetText(This,pFrame,pbstrText)	\
    ( (This)->lpVtbl -> GetText(This,pFrame,pbstrText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDynamicNavigationItem_INTERFACE_DEFINED__ */


#ifndef __IVsProvisionalItem_INTERFACE_DEFINED__
#define __IVsProvisionalItem_INTERFACE_DEFINED__

/* interface IVsProvisionalItem */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProvisionalItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9977da6b-0c38-4565-a38a-71cf8cd1ef53")
    IVsProvisionalItem : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsProvisonalViewingEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *enabled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProvisionalItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProvisionalItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProvisionalItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProvisionalItem * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsProvisonalViewingEnabled )( 
            __RPC__in IVsProvisionalItem * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *enabled);
        
        END_INTERFACE
    } IVsProvisionalItemVtbl;

    interface IVsProvisionalItem
    {
        CONST_VTBL struct IVsProvisionalItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvisionalItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvisionalItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvisionalItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvisionalItem_get_IsProvisonalViewingEnabled(This,enabled)	\
    ( (This)->lpVtbl -> get_IsProvisonalViewingEnabled(This,enabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvisionalItem_INTERFACE_DEFINED__ */


#ifndef __IVsStatusbarUser2_INTERFACE_DEFINED__
#define __IVsStatusbarUser2_INTERFACE_DEFINED__

/* interface IVsStatusbarUser2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsStatusbarUser2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d3e477a-4b61-444c-860d-f725e5560b40")
    IVsStatusbarUser2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ClearInfo( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsStatusbarUser2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsStatusbarUser2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsStatusbarUser2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsStatusbarUser2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ClearInfo )( 
            __RPC__in IVsStatusbarUser2 * This);
        
        END_INTERFACE
    } IVsStatusbarUser2Vtbl;

    interface IVsStatusbarUser2
    {
        CONST_VTBL struct IVsStatusbarUser2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsStatusbarUser2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsStatusbarUser2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsStatusbarUser2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsStatusbarUser2_ClearInfo(This)	\
    ( (This)->lpVtbl -> ClearInfo(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsStatusbarUser2_INTERFACE_DEFINED__ */


#ifndef __IVsDebugger4_INTERFACE_DEFINED__
#define __IVsDebugger4_INTERFACE_DEFINED__

/* interface IVsDebugger4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugger4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8548668A-F63A-46BB-A3BD-5D053229820A")
    IVsDebugger4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchDebugTargets4( 
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo4 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCurrentlyDebuggingProjects( 
            /* [out] */ __RPC__deref_out_opt IEnumHierarchies **projects) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugger4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugger4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugger4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugger4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchDebugTargets4 )( 
            __RPC__in IVsDebugger4 * This,
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo4 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCurrentlyDebuggingProjects )( 
            __RPC__in IVsDebugger4 * This,
            /* [out] */ __RPC__deref_out_opt IEnumHierarchies **projects);
        
        END_INTERFACE
    } IVsDebugger4Vtbl;

    interface IVsDebugger4
    {
        CONST_VTBL struct IVsDebugger4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugger4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugger4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugger4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugger4_LaunchDebugTargets4(This,DebugTargetCount,pDebugTargets,pLaunchResults)	\
    ( (This)->lpVtbl -> LaunchDebugTargets4(This,DebugTargetCount,pDebugTargets,pLaunchResults) ) 

#define IVsDebugger4_EnumCurrentlyDebuggingProjects(This,projects)	\
    ( (This)->lpVtbl -> EnumCurrentlyDebuggingProjects(This,projects) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugger4_INTERFACE_DEFINED__ */


#ifndef __IVsQueryDebuggableProjectCfg2_INTERFACE_DEFINED__
#define __IVsQueryDebuggableProjectCfg2_INTERFACE_DEFINED__

/* interface IVsQueryDebuggableProjectCfg2 */
/* [object][unique][version][uuid] */ 




EXTERN_C const IID IID_IVsQueryDebuggableProjectCfg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7666C707-8B4B-461C-B555-348C94CB79DF")
    IVsQueryDebuggableProjectCfg2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryDebugTargets( 
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch,
            /* [in] */ ULONG cTargets,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cTargets) VsDebugTargetInfo4 rgDebugTargetInfo[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsQueryDebuggableProjectCfg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsQueryDebuggableProjectCfg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsQueryDebuggableProjectCfg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsQueryDebuggableProjectCfg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDebugTargets )( 
            __RPC__in IVsQueryDebuggableProjectCfg2 * This,
            /* [in] */ VSDBGLAUNCHFLAGS grfLaunch,
            /* [in] */ ULONG cTargets,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cTargets) VsDebugTargetInfo4 rgDebugTargetInfo[  ],
            /* [optional][out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsQueryDebuggableProjectCfg2Vtbl;

    interface IVsQueryDebuggableProjectCfg2
    {
        CONST_VTBL struct IVsQueryDebuggableProjectCfg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryDebuggableProjectCfg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryDebuggableProjectCfg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryDebuggableProjectCfg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryDebuggableProjectCfg2_QueryDebugTargets(This,grfLaunch,cTargets,rgDebugTargetInfo,pcActual)	\
    ( (This)->lpVtbl -> QueryDebugTargets(This,grfLaunch,cTargets,rgDebugTargetInfo,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryDebuggableProjectCfg2_INTERFACE_DEFINED__ */


#ifndef __IVsXMLMemberDataCapability_INTERFACE_DEFINED__
#define __IVsXMLMemberDataCapability_INTERFACE_DEFINED__

/* interface IVsXMLMemberDataCapability */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsXMLMemberDataCapability;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("98F1A862-A153-4369-86B8-7C4E3D0D5E37")
    IVsXMLMemberDataCapability : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrType) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsXMLMemberDataCapabilityVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsXMLMemberDataCapability * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsXMLMemberDataCapability * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsXMLMemberDataCapability * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsXMLMemberDataCapability * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrType);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsXMLMemberDataCapability * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        END_INTERFACE
    } IVsXMLMemberDataCapabilityVtbl;

    interface IVsXMLMemberDataCapability
    {
        CONST_VTBL struct IVsXMLMemberDataCapabilityVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsXMLMemberDataCapability_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsXMLMemberDataCapability_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsXMLMemberDataCapability_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsXMLMemberDataCapability_get_Type(This,pbstrType)	\
    ( (This)->lpVtbl -> get_Type(This,pbstrType) ) 

#define IVsXMLMemberDataCapability_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsXMLMemberDataCapability_INTERFACE_DEFINED__ */


#ifndef __IVsXMLMemberData4_INTERFACE_DEFINED__
#define __IVsXMLMemberData4_INTERFACE_DEFINED__

/* interface IVsXMLMemberData4 */
/* [object][custom][unique][version][uuid] */ 




EXTERN_C const IID IID_IVsXMLMemberData4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("60721047-d8b3-4ff2-b963-d4d1f3102f77")
    IVsXMLMemberData4 : public IVsXMLMemberData3
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE GetAssociatedCapabilities( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgCapabilities) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsXMLMemberData4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsXMLMemberData4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsXMLMemberData4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetOptions )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ XMLMEMBERDATA_OPTIONS options);
        
        HRESULT ( STDMETHODCALLTYPE *GetSummaryText )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSummary);
        
        HRESULT ( STDMETHODCALLTYPE *GetParamCount )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__out long *piParams);
        
        HRESULT ( STDMETHODCALLTYPE *GetParamTextAt )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetReturnsText )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrReturns);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemarksText )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrRemarks);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionCount )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__out long *piExceptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetExceptionTextAt )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ long iException,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilterPriority )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__out long *piFilterPriority);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompletionListText )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompletionListTextAt )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ long iParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCompletionList);
        
        HRESULT ( STDMETHODCALLTYPE *GetPermissionSet )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPermissionSetXML);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeParamCount )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [out] */ __RPC__out long *piTypeParams);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeParamTextAt )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [in] */ long iTypeParam,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrText);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *GetAssociatedCapabilities )( 
            __RPC__in IVsXMLMemberData4 * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *prgCapabilities);
        
        END_INTERFACE
    } IVsXMLMemberData4Vtbl;

    interface IVsXMLMemberData4
    {
        CONST_VTBL struct IVsXMLMemberData4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsXMLMemberData4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsXMLMemberData4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsXMLMemberData4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsXMLMemberData4_SetOptions(This,options)	\
    ( (This)->lpVtbl -> SetOptions(This,options) ) 

#define IVsXMLMemberData4_GetSummaryText(This,pbstrSummary)	\
    ( (This)->lpVtbl -> GetSummaryText(This,pbstrSummary) ) 

#define IVsXMLMemberData4_GetParamCount(This,piParams)	\
    ( (This)->lpVtbl -> GetParamCount(This,piParams) ) 

#define IVsXMLMemberData4_GetParamTextAt(This,iParam,pbstrName,pbstrText)	\
    ( (This)->lpVtbl -> GetParamTextAt(This,iParam,pbstrName,pbstrText) ) 

#define IVsXMLMemberData4_GetReturnsText(This,pbstrReturns)	\
    ( (This)->lpVtbl -> GetReturnsText(This,pbstrReturns) ) 

#define IVsXMLMemberData4_GetRemarksText(This,pbstrRemarks)	\
    ( (This)->lpVtbl -> GetRemarksText(This,pbstrRemarks) ) 

#define IVsXMLMemberData4_GetExceptionCount(This,piExceptions)	\
    ( (This)->lpVtbl -> GetExceptionCount(This,piExceptions) ) 

#define IVsXMLMemberData4_GetExceptionTextAt(This,iException,pbstrType,pbstrText)	\
    ( (This)->lpVtbl -> GetExceptionTextAt(This,iException,pbstrType,pbstrText) ) 

#define IVsXMLMemberData4_GetFilterPriority(This,piFilterPriority)	\
    ( (This)->lpVtbl -> GetFilterPriority(This,piFilterPriority) ) 

#define IVsXMLMemberData4_GetCompletionListText(This,pbstrCompletionList)	\
    ( (This)->lpVtbl -> GetCompletionListText(This,pbstrCompletionList) ) 

#define IVsXMLMemberData4_GetCompletionListTextAt(This,iParam,pbstrCompletionList)	\
    ( (This)->lpVtbl -> GetCompletionListTextAt(This,iParam,pbstrCompletionList) ) 

#define IVsXMLMemberData4_GetPermissionSet(This,pbstrPermissionSetXML)	\
    ( (This)->lpVtbl -> GetPermissionSet(This,pbstrPermissionSetXML) ) 

#define IVsXMLMemberData4_GetTypeParamCount(This,piTypeParams)	\
    ( (This)->lpVtbl -> GetTypeParamCount(This,piTypeParams) ) 

#define IVsXMLMemberData4_GetTypeParamTextAt(This,iTypeParam,pbstrName,pbstrText)	\
    ( (This)->lpVtbl -> GetTypeParamTextAt(This,iTypeParam,pbstrName,pbstrText) ) 


#define IVsXMLMemberData4_GetAssociatedCapabilities(This,prgCapabilities)	\
    ( (This)->lpVtbl -> GetAssociatedCapabilities(This,prgCapabilities) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsXMLMemberData4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0090 */
/* [local] */ 


enum __VSDELETEHANDLEROPTIONS
    {
        DHO_NONE	= 0,
        DHO_SUPPRESS_UI	= 0x1
    } ;
typedef DWORD VSDELETEHANDLEROPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0090_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0090_v0_0_s_ifspec;

#ifndef __IVsHierarchyDeleteHandler3_INTERFACE_DEFINED__
#define __IVsHierarchyDeleteHandler3_INTERFACE_DEFINED__

/* interface IVsHierarchyDeleteHandler3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyDeleteHandler3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CCB143A-FB8D-455C-8413-9E051B98E557")
    IVsHierarchyDeleteHandler3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryDeleteItems( 
            /* [in] */ ULONG cItems,
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSITEMID itemid[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cItems) VARIANT_BOOL pfCanDelete[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteItems( 
            /* [in] */ ULONG cItems,
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSITEMID itemid[  ],
            /* [in] */ VSDELETEHANDLEROPTIONS dwFlags) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHierarchyDeleteHandler3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHierarchyDeleteHandler3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHierarchyDeleteHandler3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHierarchyDeleteHandler3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDeleteItems )( 
            __RPC__in IVsHierarchyDeleteHandler3 * This,
            /* [in] */ ULONG cItems,
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSITEMID itemid[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cItems) VARIANT_BOOL pfCanDelete[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteItems )( 
            __RPC__in IVsHierarchyDeleteHandler3 * This,
            /* [in] */ ULONG cItems,
            /* [in] */ VSDELETEITEMOPERATION dwDelItemOp,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) VSITEMID itemid[  ],
            /* [in] */ VSDELETEHANDLEROPTIONS dwFlags);
        
        END_INTERFACE
    } IVsHierarchyDeleteHandler3Vtbl;

    interface IVsHierarchyDeleteHandler3
    {
        CONST_VTBL struct IVsHierarchyDeleteHandler3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyDeleteHandler3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyDeleteHandler3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyDeleteHandler3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyDeleteHandler3_QueryDeleteItems(This,cItems,dwDelItemOp,itemid,pfCanDelete)	\
    ( (This)->lpVtbl -> QueryDeleteItems(This,cItems,dwDelItemOp,itemid,pfCanDelete) ) 

#define IVsHierarchyDeleteHandler3_DeleteItems(This,cItems,dwDelItemOp,itemid,dwFlags)	\
    ( (This)->lpVtbl -> DeleteItems(This,cItems,dwDelItemOp,itemid,dwFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyDeleteHandler3_INTERFACE_DEFINED__ */


#ifndef __IVsBooleanSymbolExpressionEvaluator_INTERFACE_DEFINED__
#define __IVsBooleanSymbolExpressionEvaluator_INTERFACE_DEFINED__

/* interface IVsBooleanSymbolExpressionEvaluator */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsBooleanSymbolExpressionEvaluator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("59252755-82AC-4A88-A489-453FEEBC694D")
    IVsBooleanSymbolExpressionEvaluator : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EvaluateExpression( 
            /* [unique][in] */ __RPC__in_opt LPCWSTR wszExpression,
            /* [unique][in] */ __RPC__in_opt LPCWSTR wszSymbols,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBooleanSymbolExpressionEvaluatorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBooleanSymbolExpressionEvaluator * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBooleanSymbolExpressionEvaluator * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBooleanSymbolExpressionEvaluator * This);
        
        HRESULT ( STDMETHODCALLTYPE *EvaluateExpression )( 
            __RPC__in IVsBooleanSymbolExpressionEvaluator * This,
            /* [unique][in] */ __RPC__in_opt LPCWSTR wszExpression,
            /* [unique][in] */ __RPC__in_opt LPCWSTR wszSymbols,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfResult);
        
        END_INTERFACE
    } IVsBooleanSymbolExpressionEvaluatorVtbl;

    interface IVsBooleanSymbolExpressionEvaluator
    {
        CONST_VTBL struct IVsBooleanSymbolExpressionEvaluatorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBooleanSymbolExpressionEvaluator_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBooleanSymbolExpressionEvaluator_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBooleanSymbolExpressionEvaluator_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBooleanSymbolExpressionEvaluator_EvaluateExpression(This,wszExpression,wszSymbols,pfResult)	\
    ( (This)->lpVtbl -> EvaluateExpression(This,wszExpression,wszSymbols,pfResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBooleanSymbolExpressionEvaluator_INTERFACE_DEFINED__ */


#ifndef __VsProjectCapabilityExpressionMatcher_INTERFACE_DEFINED__
#define __VsProjectCapabilityExpressionMatcher_INTERFACE_DEFINED__

/* interface VsProjectCapabilityExpressionMatcher */
/* [object][uuid] */ 


EXTERN_C const IID IID_VsProjectCapabilityExpressionMatcher;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("943A3169-D328-4E42-8AF6-7200E5E8C2E4")
    VsProjectCapabilityExpressionMatcher : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct VsProjectCapabilityExpressionMatcherVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VsProjectCapabilityExpressionMatcher * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VsProjectCapabilityExpressionMatcher * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VsProjectCapabilityExpressionMatcher * This);
        
        END_INTERFACE
    } VsProjectCapabilityExpressionMatcherVtbl;

    interface VsProjectCapabilityExpressionMatcher
    {
        CONST_VTBL struct VsProjectCapabilityExpressionMatcherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VsProjectCapabilityExpressionMatcher_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define VsProjectCapabilityExpressionMatcher_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define VsProjectCapabilityExpressionMatcher_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VsProjectCapabilityExpressionMatcher_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0093 */
/* [local] */ 

#define CLSID_VsProjectCapabilityExpressionMatcher IID_VsProjectCapabilityExpressionMatcher


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0093_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0093_v0_0_s_ifspec;

#ifndef __IVsSccProjectEvents_INTERFACE_DEFINED__
#define __IVsSccProjectEvents_INTERFACE_DEFINED__

/* interface IVsSccProjectEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccProjectEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C3F73B6-9939-4A4B-8D25-326A8ABED50F")
    IVsSccProjectEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnProjectRegisteredForSccChange( 
            /* [in] */ __RPC__in REFGUID guidProject,
            /* [in] */ VARIANT_BOOL fIsRegistered) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccProjectEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccProjectEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccProjectEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnProjectRegisteredForSccChange )( 
            __RPC__in IVsSccProjectEvents * This,
            /* [in] */ __RPC__in REFGUID guidProject,
            /* [in] */ VARIANT_BOOL fIsRegistered);
        
        END_INTERFACE
    } IVsSccProjectEventsVtbl;

    interface IVsSccProjectEvents
    {
        CONST_VTBL struct IVsSccProjectEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProjectEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProjectEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProjectEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProjectEvents_OnProjectRegisteredForSccChange(This,guidProject,fIsRegistered)	\
    ( (This)->lpVtbl -> OnProjectRegisteredForSccChange(This,guidProject,fIsRegistered) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProjectEvents_INTERFACE_DEFINED__ */


#ifndef __IVsSccManager3_INTERFACE_DEFINED__
#define __IVsSccManager3_INTERFACE_DEFINED__

/* interface IVsSccManager3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccManager3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("224209ED-E56C-4C8D-A7FF-31CF4686798D")
    IVsSccManager3 : public IVsSccManager2
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE IsBSLSupported( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfIsBSLSupported) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccManager3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccManager3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccManager3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccManager3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSccProject )( 
            __RPC__in IVsSccManager3 * This,
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszProvider);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterSccProject )( 
            __RPC__in IVsSccManager3 * This,
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccGlyph )( 
            __RPC__in IVsSccManager3 * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullPaths[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VsStateIcon rgsiGlyphs[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgdwSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccGlyphFromStatus )( 
            __RPC__in IVsSccManager3 * This,
            /* [in] */ DWORD dwSccStatus,
            /* [retval][out] */ __RPC__out VsStateIcon *psiGlyph);
        
        HRESULT ( STDMETHODCALLTYPE *IsInstalled )( 
            __RPC__in IVsSccManager3 * This,
            /* [retval][out] */ __RPC__out BOOL *pbInstalled);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseForProject )( 
            __RPC__in IVsSccManager3 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDirectory,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *CancelAfterBrowseForProject )( 
            __RPC__in IVsSccManager3 * This);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *IsBSLSupported )( 
            __RPC__in IVsSccManager3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfIsBSLSupported);
        
        END_INTERFACE
    } IVsSccManager3Vtbl;

    interface IVsSccManager3
    {
        CONST_VTBL struct IVsSccManager3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccManager3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccManager3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccManager3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccManager3_RegisterSccProject(This,pscp2Project,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider)	\
    ( (This)->lpVtbl -> RegisterSccProject(This,pscp2Project,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider) ) 

#define IVsSccManager3_UnregisterSccProject(This,pscp2Project)	\
    ( (This)->lpVtbl -> UnregisterSccProject(This,pscp2Project) ) 

#define IVsSccManager3_GetSccGlyph(This,cFiles,rgpszFullPaths,rgsiGlyphs,rgdwSccStatus)	\
    ( (This)->lpVtbl -> GetSccGlyph(This,cFiles,rgpszFullPaths,rgsiGlyphs,rgdwSccStatus) ) 

#define IVsSccManager3_GetSccGlyphFromStatus(This,dwSccStatus,psiGlyph)	\
    ( (This)->lpVtbl -> GetSccGlyphFromStatus(This,dwSccStatus,psiGlyph) ) 

#define IVsSccManager3_IsInstalled(This,pbInstalled)	\
    ( (This)->lpVtbl -> IsInstalled(This,pbInstalled) ) 

#define IVsSccManager3_BrowseForProject(This,pbstrDirectory,pfOK)	\
    ( (This)->lpVtbl -> BrowseForProject(This,pbstrDirectory,pfOK) ) 

#define IVsSccManager3_CancelAfterBrowseForProject(This)	\
    ( (This)->lpVtbl -> CancelAfterBrowseForProject(This) ) 


#define IVsSccManager3_IsBSLSupported(This,pfIsBSLSupported)	\
    ( (This)->lpVtbl -> IsBSLSupported(This,pfIsBSLSupported) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccManager3_INTERFACE_DEFINED__ */


#ifndef __IVsSccTrackProjectEvents_INTERFACE_DEFINED__
#define __IVsSccTrackProjectEvents_INTERFACE_DEFINED__

/* interface IVsSccTrackProjectEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSccTrackProjectEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4871BDEF-3B4A-49CE-80CF-51502FA9A464")
    IVsSccTrackProjectEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseSccProjectEvents( 
            /* [in] */ __RPC__in_opt IVsSccProjectEvents *pEventSink,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnAdviseSccProjectEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccTrackProjectEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccTrackProjectEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccTrackProjectEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccTrackProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseSccProjectEvents )( 
            __RPC__in IVsSccTrackProjectEvents * This,
            /* [in] */ __RPC__in_opt IVsSccProjectEvents *pEventSink,
            /* [retval][out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnAdviseSccProjectEvents )( 
            __RPC__in IVsSccTrackProjectEvents * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsSccTrackProjectEventsVtbl;

    interface IVsSccTrackProjectEvents
    {
        CONST_VTBL struct IVsSccTrackProjectEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccTrackProjectEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccTrackProjectEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccTrackProjectEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccTrackProjectEvents_AdviseSccProjectEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseSccProjectEvents(This,pEventSink,pdwCookie) ) 

#define IVsSccTrackProjectEvents_UnAdviseSccProjectEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnAdviseSccProjectEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccTrackProjectEvents_INTERFACE_DEFINED__ */


#ifndef __IVsProjectBuildMessageEvents_INTERFACE_DEFINED__
#define __IVsProjectBuildMessageEvents_INTERFACE_DEFINED__

/* interface IVsProjectBuildMessageEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProjectBuildMessageEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7fbea20b-68a9-48ec-9032-114268286b24")
    IVsProjectBuildMessageEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBuildMessage( 
            /* [in] */ VSERRORCATEGORY category,
            /* [in] */ __RPC__in LPCWSTR szMessage,
            /* [in] */ __RPC__in LPCWSTR szErrorCode,
            /* [in] */ __RPC__in LPCWSTR szHelpKeyword,
            /* [in] */ long line,
            /* [in] */ long column,
            /* [in] */ long endingLine,
            /* [in] */ long endingColumn,
            /* [in] */ __RPC__in LPCWSTR szFile,
            /* [optional][in] */ __RPC__in_opt IUnknown *pAdditionalInfo,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfStopProcessing) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectBuildMessageEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectBuildMessageEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectBuildMessageEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectBuildMessageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBuildMessage )( 
            __RPC__in IVsProjectBuildMessageEvents * This,
            /* [in] */ VSERRORCATEGORY category,
            /* [in] */ __RPC__in LPCWSTR szMessage,
            /* [in] */ __RPC__in LPCWSTR szErrorCode,
            /* [in] */ __RPC__in LPCWSTR szHelpKeyword,
            /* [in] */ long line,
            /* [in] */ long column,
            /* [in] */ long endingLine,
            /* [in] */ long endingColumn,
            /* [in] */ __RPC__in LPCWSTR szFile,
            /* [optional][in] */ __RPC__in_opt IUnknown *pAdditionalInfo,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfStopProcessing);
        
        END_INTERFACE
    } IVsProjectBuildMessageEventsVtbl;

    interface IVsProjectBuildMessageEvents
    {
        CONST_VTBL struct IVsProjectBuildMessageEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectBuildMessageEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectBuildMessageEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectBuildMessageEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectBuildMessageEvents_OnBuildMessage(This,category,szMessage,szErrorCode,szHelpKeyword,line,column,endingLine,endingColumn,szFile,pAdditionalInfo,pfStopProcessing)	\
    ( (This)->lpVtbl -> OnBuildMessage(This,category,szMessage,szErrorCode,szHelpKeyword,line,column,endingLine,endingColumn,szFile,pAdditionalInfo,pfStopProcessing) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectBuildMessageEvents_INTERFACE_DEFINED__ */


#ifndef __IVsProjectBuildMessageReporter_INTERFACE_DEFINED__
#define __IVsProjectBuildMessageReporter_INTERFACE_DEFINED__

/* interface IVsProjectBuildMessageReporter */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsProjectBuildMessageReporter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("359bf057-da83-455c-9b72-ec00cb478c85")
    IVsProjectBuildMessageReporter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseProjectBuildMessageEvents( 
            /* [in] */ __RPC__in_opt IVsProjectBuildMessageEvents *pEvents,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseProjectBuildMessageEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectBuildMessageReporterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectBuildMessageReporter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectBuildMessageReporter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectBuildMessageReporter * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseProjectBuildMessageEvents )( 
            __RPC__in IVsProjectBuildMessageReporter * This,
            /* [in] */ __RPC__in_opt IVsProjectBuildMessageEvents *pEvents,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseProjectBuildMessageEvents )( 
            __RPC__in IVsProjectBuildMessageReporter * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsProjectBuildMessageReporterVtbl;

    interface IVsProjectBuildMessageReporter
    {
        CONST_VTBL struct IVsProjectBuildMessageReporterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectBuildMessageReporter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectBuildMessageReporter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectBuildMessageReporter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectBuildMessageReporter_AdviseProjectBuildMessageEvents(This,pEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseProjectBuildMessageEvents(This,pEvents,pdwCookie) ) 

#define IVsProjectBuildMessageReporter_UnadviseProjectBuildMessageEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseProjectBuildMessageEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectBuildMessageReporter_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0098 */
/* [local] */ 


enum __SymbolToolLanguage
    {
        SymbolToolLanguage_None	= 0,
        SymbolToolLanguage_CSharp	= 1,
        SymbolToolLanguage_VB	= 2,
        SymbolToolLanguage_VC_CLI	= 3,
        SymbolToolLanguage_VC_ZW	= 4
    } ;
typedef DWORD SymbolToolLanguage;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0098_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0098_v0_0_s_ifspec;

#ifndef __IVsNavInfo2_INTERFACE_DEFINED__
#define __IVsNavInfo2_INTERFACE_DEFINED__

/* interface IVsNavInfo2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsNavInfo2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("916BA1DD-4C84-431e-9B06-FC80F9496FCF")
    IVsNavInfo2 : public IVsNavInfo
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE GetPreferredLanguage( 
            /* [out] */ __RPC__out SymbolToolLanguage *pLanguage) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsNavInfo2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsNavInfo2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsNavInfo2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsNavInfo2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLibGuid )( 
            __RPC__in IVsNavInfo2 * This,
            /* [out] */ __RPC__out GUID *pGuid);
        
        HRESULT ( STDMETHODCALLTYPE *GetSymbolType )( 
            __RPC__in IVsNavInfo2 * This,
            /* [out] */ __RPC__out DWORD *pdwType);
        
        HRESULT ( STDMETHODCALLTYPE *EnumPresentationNodes )( 
            __RPC__in IVsNavInfo2 * This,
            /* [in] */ DWORD dwFlags,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCanonicalNodes )( 
            __RPC__in IVsNavInfo2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumNavInfoNodes **ppEnum);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *GetPreferredLanguage )( 
            __RPC__in IVsNavInfo2 * This,
            /* [out] */ __RPC__out SymbolToolLanguage *pLanguage);
        
        END_INTERFACE
    } IVsNavInfo2Vtbl;

    interface IVsNavInfo2
    {
        CONST_VTBL struct IVsNavInfo2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsNavInfo2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsNavInfo2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsNavInfo2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsNavInfo2_GetLibGuid(This,pGuid)	\
    ( (This)->lpVtbl -> GetLibGuid(This,pGuid) ) 

#define IVsNavInfo2_GetSymbolType(This,pdwType)	\
    ( (This)->lpVtbl -> GetSymbolType(This,pdwType) ) 

#define IVsNavInfo2_EnumPresentationNodes(This,dwFlags,ppEnum)	\
    ( (This)->lpVtbl -> EnumPresentationNodes(This,dwFlags,ppEnum) ) 

#define IVsNavInfo2_EnumCanonicalNodes(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumCanonicalNodes(This,ppEnum) ) 


#define IVsNavInfo2_GetPreferredLanguage(This,pLanguage)	\
    ( (This)->lpVtbl -> GetPreferredLanguage(This,pLanguage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsNavInfo2_INTERFACE_DEFINED__ */


#ifndef __IVsLibrary3_INTERFACE_DEFINED__
#define __IVsLibrary3_INTERFACE_DEFINED__

/* interface IVsLibrary3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLibrary3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4B78A0FC-CFCF-46CB-9363-C13E55165DE3")
    IVsLibrary3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateNavInfo2( 
            /* [in] */ SymbolToolLanguage language,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo2 **ppNavInfo) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsLibrary3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsLibrary3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsLibrary3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsLibrary3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNavInfo2 )( 
            __RPC__in IVsLibrary3 * This,
            /* [in] */ SymbolToolLanguage language,
            /* [size_is][in] */ __RPC__in_ecount_full(ulcNodes) SYMBOL_DESCRIPTION_NODE rgSymbolNodes[  ],
            /* [in] */ ULONG ulcNodes,
            /* [out] */ __RPC__deref_out_opt IVsNavInfo2 **ppNavInfo);
        
        END_INTERFACE
    } IVsLibrary3Vtbl;

    interface IVsLibrary3
    {
        CONST_VTBL struct IVsLibrary3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLibrary3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLibrary3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLibrary3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLibrary3_CreateNavInfo2(This,language,rgSymbolNodes,ulcNodes,ppNavInfo)	\
    ( (This)->lpVtbl -> CreateNavInfo2(This,language,rgSymbolNodes,ulcNodes,ppNavInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLibrary3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0100 */
/* [local] */ 

#define VS_E_INCOMPATIBLEPROJECT    MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2003)
#define VS_E_INCOMPATIBLECLASSICPROJECT    MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2004)
#define VS_E_INCOMPATIBLEPROJECT_UNSUPPORTED_OS    MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2005)
#define VS_S_PROJECT_SAFEREPAIRREQUIRED      MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1FF2)
#define VS_S_PROJECT_UNSAFEREPAIRREQUIRED      MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1FF3)
#define VS_S_PROJECT_ONEWAYUPGRADEREQUIRED      MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1FF4)
#define VS_S_INCOMPATIBLEPROJECT      MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_ITF, 0x1FF5)
#define VS_E_PROMPTREQUIRED MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2006)
#define VS_E_CIRCULARTASKDEPENDENCY MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2007)
#define VS_E_TASKSCHEDULERFAIL MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2008)

enum __VSRDTSAVEOPTIONS3
    {
        RDTSAVEOPT_SilentSave	= 0x80
    } ;
typedef DWORD VSRDTSAVEOPTIONS3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0100_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0100_v0_0_s_ifspec;

#ifndef __SVsDifferenceService_INTERFACE_DEFINED__
#define __SVsDifferenceService_INTERFACE_DEFINED__

/* interface SVsDifferenceService */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDifferenceService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("77115E75-EF9E-4F30-92F2-3FE78BCAF6CF")
    SVsDifferenceService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsDifferenceServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsDifferenceService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsDifferenceService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsDifferenceService * This);
        
        END_INTERFACE
    } SVsDifferenceServiceVtbl;

    interface SVsDifferenceService
    {
        CONST_VTBL struct SVsDifferenceServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDifferenceService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDifferenceService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDifferenceService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDifferenceService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0101 */
/* [local] */ 

#define SID_SVsDifferenceService IID_SVsDifferenceService

enum __VSDIFFSERVICEOPTIONS
    {
        VSDIFFOPT_DoNotShow	= 0x1,
        VSDIFFOPT_DetectBinaryFiles	= 0x2,
        VSDIFFOPT_PromptForEncodingForLeft	= 0x4,
        VSDIFFOPT_PromptForEncodingForRight	= 0x8,
        VSDIFFOPT_LeftFileIsTemporary	= 0x10,
        VSDIFFOPT_RightFileIsTemporary	= 0x20
    } ;
typedef DWORD VSDIFFSERVICEOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0101_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0101_v0_0_s_ifspec;

#ifndef __IVsDifferenceService_INTERFACE_DEFINED__
#define __IVsDifferenceService_INTERFACE_DEFINED__

/* interface IVsDifferenceService */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsDifferenceService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E20E53BE-8B7A-408F-AEA7-C0AAD6D1B946")
    IVsDifferenceService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenComparisonWindow( 
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenComparisonWindowFromCommandLineArguments( 
            /* [in] */ __RPC__in LPCOLESTR arguments,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenComparisonWindow2( 
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR caption,
            /* [in] */ __RPC__in LPCOLESTR tooltip,
            /* [in] */ __RPC__in LPCOLESTR leftLabel,
            /* [in] */ __RPC__in LPCOLESTR rightLabel,
            /* [in] */ __RPC__in LPCOLESTR inlineLabel,
            /* [in] */ __RPC__in LPCOLESTR roles,
            /* [in] */ VSDIFFSERVICEOPTIONS grfDiffOptions,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDifferenceServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDifferenceService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDifferenceService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDifferenceService * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenComparisonWindow )( 
            __RPC__in IVsDifferenceService * This,
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow);
        
        HRESULT ( STDMETHODCALLTYPE *OpenComparisonWindowFromCommandLineArguments )( 
            __RPC__in IVsDifferenceService * This,
            /* [in] */ __RPC__in LPCOLESTR arguments,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow);
        
        HRESULT ( STDMETHODCALLTYPE *OpenComparisonWindow2 )( 
            __RPC__in IVsDifferenceService * This,
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR caption,
            /* [in] */ __RPC__in LPCOLESTR tooltip,
            /* [in] */ __RPC__in LPCOLESTR leftLabel,
            /* [in] */ __RPC__in LPCOLESTR rightLabel,
            /* [in] */ __RPC__in LPCOLESTR inlineLabel,
            /* [in] */ __RPC__in LPCOLESTR roles,
            /* [in] */ VSDIFFSERVICEOPTIONS grfDiffOptions,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pDiffWindow);
        
        END_INTERFACE
    } IVsDifferenceServiceVtbl;

    interface IVsDifferenceService
    {
        CONST_VTBL struct IVsDifferenceServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDifferenceService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDifferenceService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDifferenceService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDifferenceService_OpenComparisonWindow(This,leftFileMoniker,rightFileMoniker,pDiffWindow)	\
    ( (This)->lpVtbl -> OpenComparisonWindow(This,leftFileMoniker,rightFileMoniker,pDiffWindow) ) 

#define IVsDifferenceService_OpenComparisonWindowFromCommandLineArguments(This,arguments,pDiffWindow)	\
    ( (This)->lpVtbl -> OpenComparisonWindowFromCommandLineArguments(This,arguments,pDiffWindow) ) 

#define IVsDifferenceService_OpenComparisonWindow2(This,leftFileMoniker,rightFileMoniker,caption,tooltip,leftLabel,rightLabel,inlineLabel,roles,grfDiffOptions,pDiffWindow)	\
    ( (This)->lpVtbl -> OpenComparisonWindow2(This,leftFileMoniker,rightFileMoniker,caption,tooltip,leftLabel,rightLabel,inlineLabel,roles,grfDiffOptions,pDiffWindow) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDifferenceService_INTERFACE_DEFINED__ */


#ifndef __SVsFileMergeService_INTERFACE_DEFINED__
#define __SVsFileMergeService_INTERFACE_DEFINED__

/* interface SVsFileMergeService */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsFileMergeService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("34D4713E-EB24-4746-938B-BE5640D03210")
    SVsFileMergeService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsFileMergeServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsFileMergeService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsFileMergeService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsFileMergeService * This);
        
        END_INTERFACE
    } SVsFileMergeServiceVtbl;

    interface SVsFileMergeService
    {
        CONST_VTBL struct SVsFileMergeServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsFileMergeService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsFileMergeService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsFileMergeService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsFileMergeService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0103 */
/* [local] */ 

#define SID_SVsFileMergeService IID_SVsFileMergeService


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0103_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0103_v0_0_s_ifspec;

#ifndef __IVsFileMergeService_INTERFACE_DEFINED__
#define __IVsFileMergeService_INTERFACE_DEFINED__

/* interface IVsFileMergeService */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsFileMergeService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9003D4ED-2B8E-46B8-9838-78F5AE7B656D")
    IVsFileMergeService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenAndRegisterMergeWindow( 
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR baseFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR resultFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR leftFileTag,
            /* [in] */ __RPC__in LPCOLESTR rightFileTag,
            /* [in] */ __RPC__in LPCOLESTR baseFileTag,
            /* [in] */ __RPC__in LPCOLESTR resultFileTag,
            /* [in] */ __RPC__in LPCOLESTR leftFileLabel,
            /* [in] */ __RPC__in LPCOLESTR rightFileLabel,
            /* [in] */ __RPC__in LPCOLESTR baseFileLabel,
            /* [in] */ __RPC__in LPCOLESTR resultFileLabel,
            /* [in] */ __RPC__in LPCOLESTR serverGuid,
            /* [in] */ __RPC__in LPCOLESTR leftFileSpec,
            /* [in] */ __RPC__in LPCOLESTR rightFileSpec,
            /* [out] */ __RPC__out int *cookie,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pMergeWindow) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterMergeWindow( 
            /* [in] */ int cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryMergeWindowState( 
            /* [in] */ int cookie,
            /* [out] */ __RPC__out int *pfState,
            /* [out] */ __RPC__deref_out_opt BSTR *errorAndWarningMsg) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFileMergeServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFileMergeService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFileMergeService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFileMergeService * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenAndRegisterMergeWindow )( 
            __RPC__in IVsFileMergeService * This,
            /* [in] */ __RPC__in LPCOLESTR leftFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR rightFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR baseFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR resultFileMoniker,
            /* [in] */ __RPC__in LPCOLESTR leftFileTag,
            /* [in] */ __RPC__in LPCOLESTR rightFileTag,
            /* [in] */ __RPC__in LPCOLESTR baseFileTag,
            /* [in] */ __RPC__in LPCOLESTR resultFileTag,
            /* [in] */ __RPC__in LPCOLESTR leftFileLabel,
            /* [in] */ __RPC__in LPCOLESTR rightFileLabel,
            /* [in] */ __RPC__in LPCOLESTR baseFileLabel,
            /* [in] */ __RPC__in LPCOLESTR resultFileLabel,
            /* [in] */ __RPC__in LPCOLESTR serverGuid,
            /* [in] */ __RPC__in LPCOLESTR leftFileSpec,
            /* [in] */ __RPC__in LPCOLESTR rightFileSpec,
            /* [out] */ __RPC__out int *cookie,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **pMergeWindow);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterMergeWindow )( 
            __RPC__in IVsFileMergeService * This,
            /* [in] */ int cookie);
        
        HRESULT ( STDMETHODCALLTYPE *QueryMergeWindowState )( 
            __RPC__in IVsFileMergeService * This,
            /* [in] */ int cookie,
            /* [out] */ __RPC__out int *pfState,
            /* [out] */ __RPC__deref_out_opt BSTR *errorAndWarningMsg);
        
        END_INTERFACE
    } IVsFileMergeServiceVtbl;

    interface IVsFileMergeService
    {
        CONST_VTBL struct IVsFileMergeServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFileMergeService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFileMergeService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFileMergeService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFileMergeService_OpenAndRegisterMergeWindow(This,leftFileMoniker,rightFileMoniker,baseFileMoniker,resultFileMoniker,leftFileTag,rightFileTag,baseFileTag,resultFileTag,leftFileLabel,rightFileLabel,baseFileLabel,resultFileLabel,serverGuid,leftFileSpec,rightFileSpec,cookie,pMergeWindow)	\
    ( (This)->lpVtbl -> OpenAndRegisterMergeWindow(This,leftFileMoniker,rightFileMoniker,baseFileMoniker,resultFileMoniker,leftFileTag,rightFileTag,baseFileTag,resultFileTag,leftFileLabel,rightFileLabel,baseFileLabel,resultFileLabel,serverGuid,leftFileSpec,rightFileSpec,cookie,pMergeWindow) ) 

#define IVsFileMergeService_UnregisterMergeWindow(This,cookie)	\
    ( (This)->lpVtbl -> UnregisterMergeWindow(This,cookie) ) 

#define IVsFileMergeService_QueryMergeWindowState(This,cookie,pfState,errorAndWarningMsg)	\
    ( (This)->lpVtbl -> QueryMergeWindowState(This,cookie,pfState,errorAndWarningMsg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFileMergeService_INTERFACE_DEFINED__ */


#ifndef __IVsUpdateSolutionEvents4_INTERFACE_DEFINED__
#define __IVsUpdateSolutionEvents4_INTERFACE_DEFINED__

/* interface IVsUpdateSolutionEvents4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpdateSolutionEvents4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("84CA83EE-EE80-42C1-99CE-1DE83F2FCA3A")
    IVsUpdateSolutionEvents4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_QueryDelayFirstUpdateAction( 
            /* [out] */ __RPC__out int *pfDelay) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_BeginFirstUpdateAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_EndLastUpdateAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_BeginUpdateAction( 
            /* [in] */ DWORD dwAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_EndUpdateAction( 
            /* [in] */ DWORD dwAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnActiveProjectCfgChangeBatchBegin( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnActiveProjectCfgChangeBatchEnd( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUpdateSolutionEvents4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUpdateSolutionEvents4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_QueryDelayFirstUpdateAction )( 
            __RPC__in IVsUpdateSolutionEvents4 * This,
            /* [out] */ __RPC__out int *pfDelay);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_BeginFirstUpdateAction )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_EndLastUpdateAction )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_BeginUpdateAction )( 
            __RPC__in IVsUpdateSolutionEvents4 * This,
            /* [in] */ DWORD dwAction);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_EndUpdateAction )( 
            __RPC__in IVsUpdateSolutionEvents4 * This,
            /* [in] */ DWORD dwAction);
        
        HRESULT ( STDMETHODCALLTYPE *OnActiveProjectCfgChangeBatchBegin )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnActiveProjectCfgChangeBatchEnd )( 
            __RPC__in IVsUpdateSolutionEvents4 * This);
        
        END_INTERFACE
    } IVsUpdateSolutionEvents4Vtbl;

    interface IVsUpdateSolutionEvents4
    {
        CONST_VTBL struct IVsUpdateSolutionEvents4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpdateSolutionEvents4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpdateSolutionEvents4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpdateSolutionEvents4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpdateSolutionEvents4_UpdateSolution_QueryDelayFirstUpdateAction(This,pfDelay)	\
    ( (This)->lpVtbl -> UpdateSolution_QueryDelayFirstUpdateAction(This,pfDelay) ) 

#define IVsUpdateSolutionEvents4_UpdateSolution_BeginFirstUpdateAction(This)	\
    ( (This)->lpVtbl -> UpdateSolution_BeginFirstUpdateAction(This) ) 

#define IVsUpdateSolutionEvents4_UpdateSolution_EndLastUpdateAction(This)	\
    ( (This)->lpVtbl -> UpdateSolution_EndLastUpdateAction(This) ) 

#define IVsUpdateSolutionEvents4_UpdateSolution_BeginUpdateAction(This,dwAction)	\
    ( (This)->lpVtbl -> UpdateSolution_BeginUpdateAction(This,dwAction) ) 

#define IVsUpdateSolutionEvents4_UpdateSolution_EndUpdateAction(This,dwAction)	\
    ( (This)->lpVtbl -> UpdateSolution_EndUpdateAction(This,dwAction) ) 

#define IVsUpdateSolutionEvents4_OnActiveProjectCfgChangeBatchBegin(This)	\
    ( (This)->lpVtbl -> OnActiveProjectCfgChangeBatchBegin(This) ) 

#define IVsUpdateSolutionEvents4_OnActiveProjectCfgChangeBatchEnd(This)	\
    ( (This)->lpVtbl -> OnActiveProjectCfgChangeBatchEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpdateSolutionEvents4_INTERFACE_DEFINED__ */


#ifndef __IVsFireUpdateSolutionEvents_INTERFACE_DEFINED__
#define __IVsFireUpdateSolutionEvents_INTERFACE_DEFINED__

/* interface IVsFireUpdateSolutionEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFireUpdateSolutionEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D8C0B590-65EC-42F7-903F-8ED7C1B3D629")
    IVsFireUpdateSolutionEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE FireOnActiveProjectCfgChange( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pIVsHierarchy) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FireOnActiveProjectCfgChangeBatchBegin( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FireOnActiveProjectCfgChangeBatchEnd( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFireUpdateSolutionEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFireUpdateSolutionEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFireUpdateSolutionEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFireUpdateSolutionEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnActiveProjectCfgChange )( 
            __RPC__in IVsFireUpdateSolutionEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pIVsHierarchy);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnActiveProjectCfgChangeBatchBegin )( 
            __RPC__in IVsFireUpdateSolutionEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *FireOnActiveProjectCfgChangeBatchEnd )( 
            __RPC__in IVsFireUpdateSolutionEvents * This);
        
        END_INTERFACE
    } IVsFireUpdateSolutionEventsVtbl;

    interface IVsFireUpdateSolutionEvents
    {
        CONST_VTBL struct IVsFireUpdateSolutionEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFireUpdateSolutionEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFireUpdateSolutionEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFireUpdateSolutionEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFireUpdateSolutionEvents_FireOnActiveProjectCfgChange(This,pIVsHierarchy)	\
    ( (This)->lpVtbl -> FireOnActiveProjectCfgChange(This,pIVsHierarchy) ) 

#define IVsFireUpdateSolutionEvents_FireOnActiveProjectCfgChangeBatchBegin(This)	\
    ( (This)->lpVtbl -> FireOnActiveProjectCfgChangeBatchBegin(This) ) 

#define IVsFireUpdateSolutionEvents_FireOnActiveProjectCfgChangeBatchEnd(This)	\
    ( (This)->lpVtbl -> FireOnActiveProjectCfgChangeBatchEnd(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFireUpdateSolutionEvents_INTERFACE_DEFINED__ */


#ifndef __IVsUpdateSolutionEventsAsyncCallback_INTERFACE_DEFINED__
#define __IVsUpdateSolutionEventsAsyncCallback_INTERFACE_DEFINED__

/* interface IVsUpdateSolutionEventsAsyncCallback */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpdateSolutionEventsAsyncCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("02D0878C-53F5-4CE9-B55C-3577DAE64761")
    IVsUpdateSolutionEventsAsyncCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CompleteLastUpdateAction( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUpdateSolutionEventsAsyncCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUpdateSolutionEventsAsyncCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUpdateSolutionEventsAsyncCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUpdateSolutionEventsAsyncCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *CompleteLastUpdateAction )( 
            __RPC__in IVsUpdateSolutionEventsAsyncCallback * This);
        
        END_INTERFACE
    } IVsUpdateSolutionEventsAsyncCallbackVtbl;

    interface IVsUpdateSolutionEventsAsyncCallback
    {
        CONST_VTBL struct IVsUpdateSolutionEventsAsyncCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpdateSolutionEventsAsyncCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpdateSolutionEventsAsyncCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpdateSolutionEventsAsyncCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpdateSolutionEventsAsyncCallback_CompleteLastUpdateAction(This)	\
    ( (This)->lpVtbl -> CompleteLastUpdateAction(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpdateSolutionEventsAsyncCallback_INTERFACE_DEFINED__ */


#ifndef __IVsUpdateSolutionEventsAsync_INTERFACE_DEFINED__
#define __IVsUpdateSolutionEventsAsync_INTERFACE_DEFINED__

/* interface IVsUpdateSolutionEventsAsync */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUpdateSolutionEventsAsync;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("703ECC2C-7631-46A9-AD1E-19D9592C7A6B")
    IVsUpdateSolutionEventsAsync : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateSolution_EndLastUpdateActionAsync( 
            __RPC__in_opt IVsUpdateSolutionEventsAsyncCallback *pCallback) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUpdateSolutionEventsAsyncVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUpdateSolutionEventsAsync * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUpdateSolutionEventsAsync * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUpdateSolutionEventsAsync * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateSolution_EndLastUpdateActionAsync )( 
            __RPC__in IVsUpdateSolutionEventsAsync * This,
            __RPC__in_opt IVsUpdateSolutionEventsAsyncCallback *pCallback);
        
        END_INTERFACE
    } IVsUpdateSolutionEventsAsyncVtbl;

    interface IVsUpdateSolutionEventsAsync
    {
        CONST_VTBL struct IVsUpdateSolutionEventsAsyncVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpdateSolutionEventsAsync_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpdateSolutionEventsAsync_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpdateSolutionEventsAsync_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpdateSolutionEventsAsync_UpdateSolution_EndLastUpdateActionAsync(This,pCallback)	\
    ( (This)->lpVtbl -> UpdateSolution_EndLastUpdateActionAsync(This,pCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpdateSolutionEventsAsync_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionBuildManager5_INTERFACE_DEFINED__
#define __IVsSolutionBuildManager5_INTERFACE_DEFINED__

/* interface IVsSolutionBuildManager5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionBuildManager5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("75D64352-C2F9-4BF8-9C89-57CFE548BF75")
    IVsSolutionBuildManager5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseUpdateSolutionEvents4( 
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEvents4 *pIVsUpdateSolutionEvents4,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseUpdateSolutionEvents4( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseUpdateSolutionEventsAsync( 
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEventsAsync *pIVsUpdateSolutionEventsAsync,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseUpdateSolutionEventsAsync( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE FindActiveProjectCfgName( 
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectCfgCanonicalName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionBuildManager5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionBuildManager5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionBuildManager5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseUpdateSolutionEvents4 )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEvents4 *pIVsUpdateSolutionEvents4,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseUpdateSolutionEvents4 )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseUpdateSolutionEventsAsync )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ __RPC__in_opt IVsUpdateSolutionEventsAsync *pIVsUpdateSolutionEventsAsync,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseUpdateSolutionEventsAsync )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *FindActiveProjectCfgName )( 
            __RPC__in IVsSolutionBuildManager5 * This,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectCfgCanonicalName);
        
        END_INTERFACE
    } IVsSolutionBuildManager5Vtbl;

    interface IVsSolutionBuildManager5
    {
        CONST_VTBL struct IVsSolutionBuildManager5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionBuildManager5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionBuildManager5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionBuildManager5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionBuildManager5_AdviseUpdateSolutionEvents4(This,pIVsUpdateSolutionEvents4,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseUpdateSolutionEvents4(This,pIVsUpdateSolutionEvents4,pdwCookie) ) 

#define IVsSolutionBuildManager5_UnadviseUpdateSolutionEvents4(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseUpdateSolutionEvents4(This,dwCookie) ) 

#define IVsSolutionBuildManager5_AdviseUpdateSolutionEventsAsync(This,pIVsUpdateSolutionEventsAsync,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseUpdateSolutionEventsAsync(This,pIVsUpdateSolutionEventsAsync,pdwCookie) ) 

#define IVsSolutionBuildManager5_UnadviseUpdateSolutionEventsAsync(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseUpdateSolutionEventsAsync(This,dwCookie) ) 

#define IVsSolutionBuildManager5_FindActiveProjectCfgName(This,rguidProjectID,pbstrProjectCfgCanonicalName)	\
    ( (This)->lpVtbl -> FindActiveProjectCfgName(This,rguidProjectID,pbstrProjectCfgCanonicalName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionBuildManager5_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0109 */
/* [local] */ 

extern const __declspec(selectany) GUID UICONTEXT_SolutionHasAppContainerProject = { 0x7CAC4AE1, 0x2E6B, 0x4B02, { 0xA9, 0x1C, 0x71, 0x61, 0x1E, 0x86, 0xF2, 0x73 } };
typedef 
enum _vsuptodatecheckflags2
    {
        VSUTDCF_REBUILD	= 0x2,
        VSUTDCF_PACKAGE	= 0x4,
        VSUTDCF_PRIVATE	= 0xffff0000
    } 	VsUpToDateCheckFlags2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0109_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0109_v0_0_s_ifspec;

#ifndef __IVsLaunchPad4_INTERFACE_DEFINED__
#define __IVsLaunchPad4_INTERFACE_DEFINED__

/* interface IVsLaunchPad4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLaunchPad4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("70A675A3-71B7-462B-B7FF-02C6956F2715")
    IVsLaunchPad4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ExecCommandWithElevation( 
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir,
            /* [retval][out] */ __RPC__out DWORD *pdwProcessExitCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsLaunchPad4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsLaunchPad4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsLaunchPad4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsLaunchPad4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExecCommandWithElevation )( 
            __RPC__in IVsLaunchPad4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszApplicationName,
            /* [in] */ __RPC__in LPCOLESTR pszCommandLine,
            /* [in] */ __RPC__in LPCOLESTR pszWorkingDir,
            /* [retval][out] */ __RPC__out DWORD *pdwProcessExitCode);
        
        END_INTERFACE
    } IVsLaunchPad4Vtbl;

    interface IVsLaunchPad4
    {
        CONST_VTBL struct IVsLaunchPad4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLaunchPad4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLaunchPad4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLaunchPad4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLaunchPad4_ExecCommandWithElevation(This,pszApplicationName,pszCommandLine,pszWorkingDir,pdwProcessExitCode)	\
    ( (This)->lpVtbl -> ExecCommandWithElevation(This,pszApplicationName,pszCommandLine,pszWorkingDir,pdwProcessExitCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLaunchPad4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0110 */
/* [local] */ 

extern const __declspec(selectany) GUID UICONTEXT_FirstLaunchSetup = { 0xe7b2b2db, 0x973b, 0x4ce9, { 0xa8, 0xd7, 0x84, 0x98, 0x89, 0x5d, 0xea, 0x73 } };
extern const __declspec(selectany) GUID UICONTEXT_OsWindows8OrHigher = { 0x67CFF80C, 0x0863, 0x4202, { 0xA4, 0xE4, 0xCE, 0x80, 0xFD, 0xF8, 0x50, 0x6E } };
extern const __declspec(selectany) GUID UICONTEXT_ToolboxVisible = { 0x643905ee, 0xdae9, 0x4f52, { 0xa3, 0x43, 0x6a, 0x5a, 0x73, 0x49, 0xd5, 0x2c } };
extern const __declspec(selectany) GUID UICONTEXT_BackgroundProjectLoad = { 0xdc769521, 0x31a2, 0x41a5, {0x9b, 0xbb, 0x21, 0x0b, 0x5d, 0x63, 0x56, 0x8d } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0110_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0110_v0_0_s_ifspec;

#ifndef __IVsEnumGuids_INTERFACE_DEFINED__
#define __IVsEnumGuids_INTERFACE_DEFINED__

/* interface IVsEnumGuids */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumGuids;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BEC804F7-F5DE-4F3E-8EBB-DAB26649F33F")
    IVsEnumGuids : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) GUID rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumGuids **ppenum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumGuidsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumGuids * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumGuids * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumGuids * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumGuids * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) GUID rgelt[  ],
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumGuids * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumGuids * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumGuids * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumGuids **ppenum);
        
        END_INTERFACE
    } IVsEnumGuidsVtbl;

    interface IVsEnumGuids
    {
        CONST_VTBL struct IVsEnumGuidsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumGuids_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumGuids_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumGuids_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumGuids_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumGuids_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumGuids_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumGuids_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumGuids_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0111 */
/* [local] */ 


enum __THEMEDCOLORTYPE
    {
        TCT_Background	= 0,
        TCT_Foreground	= ( TCT_Background + 1 ) 
    } ;
typedef DWORD THEMEDCOLORTYPE;

typedef DWORD VS_RGBA;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0111_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0111_v0_0_s_ifspec;

#ifndef __IVsUIShell5_INTERFACE_DEFINED__
#define __IVsUIShell5_INTERFACE_DEFINED__

/* interface IVsUIShell5 */
/* [object][unique][version][uuid] */ 




EXTERN_C const IID IID_IVsUIShell5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2B70EA30-51F2-48BB-ABA8-051946A37283")
    IVsUIShell5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetOpenFileNameViaDlgEx2( 
            /* [out][in] */ __RPC__inout VSOPENFILENAMEW *openFileName,
            /* [in] */ __RPC__in LPCOLESTR helpTopic,
            /* [in] */ __RPC__in LPCOLESTR openButtonLabel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetThemedColor( 
            /* [in] */ __RPC__in REFGUID colorCategory,
            /* [in] */ __RPC__in LPCOLESTR colorName,
            /* [in] */ THEMEDCOLORTYPE colorType,
            /* [retval][out] */ __RPC__out VS_RGBA *colorRgba) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetKeyBindingScope( 
            /* [in] */ __RPC__in REFGUID keyBindingScope,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumKeyBindingScopes( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumGuids **ppEnum) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE ThemeWindow( 
            /* [in] */ HWND hwnd,
            /* [retval][optional][out] */ VARIANT_BOOL *pfThemeApplied) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE CreateThemedImageList( 
            /* [in] */ HANDLE hImageList,
            /* [in] */ COLORREF crBackground,
            /* [retval][out] */ HANDLE *phThemedImageList) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ThemeDIBits( 
            /* [in] */ DWORD dwBitmapLength,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(dwBitmapLength) BYTE pBitmap[  ],
            /* [in] */ DWORD dwPixelWidth,
            /* [in] */ DWORD dwPixelHeight,
            /* [in] */ VARIANT_BOOL fIsTopDownBitmap,
            /* [in] */ COLORREF crBackground) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIShell5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIShell5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIShell5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIShell5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetOpenFileNameViaDlgEx2 )( 
            __RPC__in IVsUIShell5 * This,
            /* [out][in] */ __RPC__inout VSOPENFILENAMEW *openFileName,
            /* [in] */ __RPC__in LPCOLESTR helpTopic,
            /* [in] */ __RPC__in LPCOLESTR openButtonLabel);
        
        HRESULT ( STDMETHODCALLTYPE *GetThemedColor )( 
            __RPC__in IVsUIShell5 * This,
            /* [in] */ __RPC__in REFGUID colorCategory,
            /* [in] */ __RPC__in LPCOLESTR colorName,
            /* [in] */ THEMEDCOLORTYPE colorType,
            /* [retval][out] */ __RPC__out VS_RGBA *colorRgba);
        
        HRESULT ( STDMETHODCALLTYPE *GetKeyBindingScope )( 
            __RPC__in IVsUIShell5 * This,
            /* [in] */ __RPC__in REFGUID keyBindingScope,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *EnumKeyBindingScopes )( 
            __RPC__in IVsUIShell5 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumGuids **ppEnum);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *ThemeWindow )( 
            IVsUIShell5 * This,
            /* [in] */ HWND hwnd,
            /* [retval][optional][out] */ VARIANT_BOOL *pfThemeApplied);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *CreateThemedImageList )( 
            IVsUIShell5 * This,
            /* [in] */ HANDLE hImageList,
            /* [in] */ COLORREF crBackground,
            /* [retval][out] */ HANDLE *phThemedImageList);
        
        HRESULT ( STDMETHODCALLTYPE *ThemeDIBits )( 
            __RPC__in IVsUIShell5 * This,
            /* [in] */ DWORD dwBitmapLength,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(dwBitmapLength) BYTE pBitmap[  ],
            /* [in] */ DWORD dwPixelWidth,
            /* [in] */ DWORD dwPixelHeight,
            /* [in] */ VARIANT_BOOL fIsTopDownBitmap,
            /* [in] */ COLORREF crBackground);
        
        END_INTERFACE
    } IVsUIShell5Vtbl;

    interface IVsUIShell5
    {
        CONST_VTBL struct IVsUIShell5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShell5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShell5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShell5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShell5_GetOpenFileNameViaDlgEx2(This,openFileName,helpTopic,openButtonLabel)	\
    ( (This)->lpVtbl -> GetOpenFileNameViaDlgEx2(This,openFileName,helpTopic,openButtonLabel) ) 

#define IVsUIShell5_GetThemedColor(This,colorCategory,colorName,colorType,colorRgba)	\
    ( (This)->lpVtbl -> GetThemedColor(This,colorCategory,colorName,colorType,colorRgba) ) 

#define IVsUIShell5_GetKeyBindingScope(This,keyBindingScope,pbstrName)	\
    ( (This)->lpVtbl -> GetKeyBindingScope(This,keyBindingScope,pbstrName) ) 

#define IVsUIShell5_EnumKeyBindingScopes(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumKeyBindingScopes(This,ppEnum) ) 

#define IVsUIShell5_ThemeWindow(This,hwnd,pfThemeApplied)	\
    ( (This)->lpVtbl -> ThemeWindow(This,hwnd,pfThemeApplied) ) 

#define IVsUIShell5_CreateThemedImageList(This,hImageList,crBackground,phThemedImageList)	\
    ( (This)->lpVtbl -> CreateThemedImageList(This,hImageList,crBackground,phThemedImageList) ) 

#define IVsUIShell5_ThemeDIBits(This,dwBitmapLength,pBitmap,dwPixelWidth,dwPixelHeight,fIsTopDownBitmap,crBackground)	\
    ( (This)->lpVtbl -> ThemeDIBits(This,dwBitmapLength,pBitmap,dwPixelWidth,dwPixelHeight,fIsTopDownBitmap,crBackground) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShell5_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0112 */
/* [local] */ 

extern const __declspec(selectany) GUID GUID_LightColorTheme             = { 0xde3dbbcd, 0xf642, 0x433c, { 0x83, 0x53, 0x8f, 0x1d, 0xf4, 0x37, 0x0a, 0xba} };
extern const __declspec(selectany) GUID GUID_DarkColorTheme              = { 0x1ded0138, 0x47ce, 0x435e, { 0x84, 0xef, 0x9e, 0xc1, 0xf4, 0x39, 0xb7, 0x49} };
extern const __declspec(selectany) GUID GUID_EnvironmentColorsCategory   = { 0x624ed9c3, 0xbdfd, 0x41fa, { 0x96, 0xc3, 0x7c, 0x82, 0x4e, 0xa3, 0x2e, 0x3d } };
extern const __declspec(selectany) GUID GUID_TreeViewColorsCatetory      = { 0x92ecf08e, 0x8b13, 0x4cf4, { 0x99, 0xe9, 0xae, 0x26, 0x92, 0x38, 0x21, 0x85 } };
extern const __declspec(selectany) GUID GUID_HeaderColorsCatetory        = { 0x4997f547, 0x1379, 0x456e, { 0xb9, 0x85, 0x2f, 0x41, 0x3c, 0xdf, 0xa5, 0x36 } };
extern const __declspec(selectany) GUID GUID_SearchControlColorsCatetory = { 0xf1095fad, 0x881f, 0x45f1, { 0x85, 0x80, 0x58, 0x9e, 0x10, 0x32, 0x5e, 0xb8 } };

enum DEBUG_REMOTE_DISCOVERY_FLAGS
    {
        DRD_NONE	= 0,
        DRD_SHOW_MANUAL	= 0x1
    } ;


extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0112_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0112_v0_0_s_ifspec;

#ifndef __IVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__
#define __IVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__

/* interface IVsDebugRemoteDiscoveryUI */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugRemoteDiscoveryUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("81FB5379-B64E-4d8f-8628-E5305306F12A")
    IVsDebugRemoteDiscoveryUI : public IUnknown
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE SelectRemoteInstanceViaDlg( 
            /* [in] */ __RPC__in BSTR currentTransportQualifier,
            /* [in] */ GUID currentPortSupplier,
            /* [in] */ DWORD flags,
            /* [out] */ __RPC__deref_out_opt BSTR *transportQualifier,
            /* [out] */ __RPC__out GUID *guidPortSupplier) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugRemoteDiscoveryUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugRemoteDiscoveryUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugRemoteDiscoveryUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugRemoteDiscoveryUI * This);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *SelectRemoteInstanceViaDlg )( 
            __RPC__in IVsDebugRemoteDiscoveryUI * This,
            /* [in] */ __RPC__in BSTR currentTransportQualifier,
            /* [in] */ GUID currentPortSupplier,
            /* [in] */ DWORD flags,
            /* [out] */ __RPC__deref_out_opt BSTR *transportQualifier,
            /* [out] */ __RPC__out GUID *guidPortSupplier);
        
        END_INTERFACE
    } IVsDebugRemoteDiscoveryUIVtbl;

    interface IVsDebugRemoteDiscoveryUI
    {
        CONST_VTBL struct IVsDebugRemoteDiscoveryUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugRemoteDiscoveryUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugRemoteDiscoveryUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugRemoteDiscoveryUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugRemoteDiscoveryUI_SelectRemoteInstanceViaDlg(This,currentTransportQualifier,currentPortSupplier,flags,transportQualifier,guidPortSupplier)	\
    ( (This)->lpVtbl -> SelectRemoteInstanceViaDlg(This,currentTransportQualifier,currentPortSupplier,flags,transportQualifier,guidPortSupplier) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__ */


#ifndef __SVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__
#define __SVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__

/* interface SVsDebugRemoteDiscoveryUI */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDebugRemoteDiscoveryUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C1207A4-2B59-426b-8F31-980D0E7A427C")
    SVsDebugRemoteDiscoveryUI : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsDebugRemoteDiscoveryUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsDebugRemoteDiscoveryUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsDebugRemoteDiscoveryUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsDebugRemoteDiscoveryUI * This);
        
        END_INTERFACE
    } SVsDebugRemoteDiscoveryUIVtbl;

    interface SVsDebugRemoteDiscoveryUI
    {
        CONST_VTBL struct SVsDebugRemoteDiscoveryUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDebugRemoteDiscoveryUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDebugRemoteDiscoveryUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDebugRemoteDiscoveryUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDebugRemoteDiscoveryUI_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0114 */
/* [local] */ 

#define SID_SVsDebugRemoteDiscoveryUI IID_SVsDebugRemoteDiscoveryUI

enum __VSTASKRUNCONTEXT
    {
        VSTC_BACKGROUNDTHREAD	= 0,
        VSTC_UITHREAD_SEND	= 1,
        VSTC_UITHREAD_BACKGROUND_PRIORITY	= 2,
        VSTC_UITHREAD_IDLE_PRIORITY	= 3,
        VSTC_CURRENTCONTEXT	= 4,
        VSTC_BACKGROUNDTHREAD_LOW_IO_PRIORITY	= 5,
        VSTC_UITHREAD_NORMAL_PRIORITY	= 6
    } ;
typedef DWORD VSTASKRUNCONTEXT;


enum __VSTASKCONTINUATIONOPTIONS
    {
        VSTCO_None	= 0,
        VSTCO_PreferFairness	= 1,
        VSTCO_LongRunning	= 2,
        VSTCO_AttachedToParent	= 4,
        VSTCO_DenyChildAttach	= 8,
        VSTCO_LazyCancelation	= 32,
        VSTCO_NotOnRanToCompletion	= 0x10000,
        VSTCO_NotOnFaulted	= 0x20000,
        VSTCO_OnlyOnCanceled	= 0x30000,
        VSTCO_NotOnCanceled	= 0x40000,
        VSTCO_OnlyOnFaulted	= 0x50000,
        VSTCO_OnlyOnRanToCompletion	= 0x60000,
        VSTCO_ExecuteSynchronously	= 0x80000,
        VSTCO_IndependentlyCanceled	= 0x40000000,
        VSTCO_NotCancelable	= 0x80000000,
        VSTCO_Default	= VSTCO_NotOnFaulted
    } ;
typedef DWORD VSTASKCONTINUATIONOPTIONS;


enum __VSTASKCREATIONOPTIONS
    {
        VSTCRO_None	= VSTCO_None,
        VSTCRO_PreferFairness	= VSTCO_PreferFairness,
        VSTCRO_LongRunning	= VSTCO_LongRunning,
        VSTCRO_AttachedToParent	= VSTCO_AttachedToParent,
        VSTCRO_DenyChildAttach	= VSTCO_DenyChildAttach,
        VSTCRO_NotCancelable	= VSTCO_NotCancelable
    } ;
typedef DWORD VSTASKCREATIONOPTIONS;


enum __VSTASKWAITOPTIONS
    {
        VSTWO_None	= 0,
        VSTWO_AbortOnTaskCancellation	= 0x1
    } ;
typedef DWORD VSTASKWAITOPTIONS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0114_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0114_v0_0_s_ifspec;

#ifndef __IVsTaskBody_INTERFACE_DEFINED__
#define __IVsTaskBody_INTERFACE_DEFINED__

/* interface IVsTaskBody */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskBody;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("05a07459-551f-4cdf-b38a-16089d083110")
    IVsTaskBody : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DoWork( 
            /* [in] */ __RPC__in_opt IVsTask *pTask,
            /* [in] */ DWORD dwCount,
            /* [size_is][in] */ __RPC__in_ecount_full(dwCount) IVsTask *pParentTasks[  ],
            /* [out] */ __RPC__out VARIANT *pResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskBodyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTaskBody * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTaskBody * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTaskBody * This);
        
        HRESULT ( STDMETHODCALLTYPE *DoWork )( 
            __RPC__in IVsTaskBody * This,
            /* [in] */ __RPC__in_opt IVsTask *pTask,
            /* [in] */ DWORD dwCount,
            /* [size_is][in] */ __RPC__in_ecount_full(dwCount) IVsTask *pParentTasks[  ],
            /* [out] */ __RPC__out VARIANT *pResult);
        
        END_INTERFACE
    } IVsTaskBodyVtbl;

    interface IVsTaskBody
    {
        CONST_VTBL struct IVsTaskBodyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskBody_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskBody_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskBody_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskBody_DoWork(This,pTask,dwCount,pParentTasks,pResult)	\
    ( (This)->lpVtbl -> DoWork(This,pTask,dwCount,pParentTasks,pResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskBody_INTERFACE_DEFINED__ */


#ifndef __IVsTask_INTERFACE_DEFINED__
#define __IVsTask_INTERFACE_DEFINED__

/* interface IVsTask */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTask;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0b98eab8-00bb-45d0-ae2f-3de35cd68235")
    IVsTask : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ContinueWith( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContinueWithEx( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ VSTASKCONTINUATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Start( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Cancel( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResult( 
            /* [retval][out] */ __RPC__out VARIANT *pResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AbortIfCanceled( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Wait( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitEx( 
            /* [in] */ int millisecondsTimeout,
            /* [in] */ VSTASKWAITOPTIONS options,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pTaskCompleted) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsFaulted( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsCompleted( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_IsCanceled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_AsyncState( 
            /* [retval][out] */ __RPC__out VARIANT *pAsyncState) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ppDescriptionText) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_Description( 
            /* [in] */ __RPC__in LPCOLESTR pDescriptionText) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTask * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTask * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueWith )( 
            __RPC__in IVsTask * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueWithEx )( 
            __RPC__in IVsTask * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ VSTASKCONTINUATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *Start )( 
            __RPC__in IVsTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            __RPC__in IVsTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetResult )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__out VARIANT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *AbortIfCanceled )( 
            __RPC__in IVsTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *Wait )( 
            __RPC__in IVsTask * This);
        
        HRESULT ( STDMETHODCALLTYPE *WaitEx )( 
            __RPC__in IVsTask * This,
            /* [in] */ int millisecondsTimeout,
            /* [in] */ VSTASKWAITOPTIONS options,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pTaskCompleted);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsFaulted )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsCompleted )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsCanceled )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pResult);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_AsyncState )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__out VARIANT *pAsyncState);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsTask * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *ppDescriptionText);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            __RPC__in IVsTask * This,
            /* [in] */ __RPC__in LPCOLESTR pDescriptionText);
        
        END_INTERFACE
    } IVsTaskVtbl;

    interface IVsTask
    {
        CONST_VTBL struct IVsTaskVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTask_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTask_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTask_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTask_ContinueWith(This,context,pTaskBody,ppTask)	\
    ( (This)->lpVtbl -> ContinueWith(This,context,pTaskBody,ppTask) ) 

#define IVsTask_ContinueWithEx(This,context,options,pTaskBody,pAsyncState,ppTask)	\
    ( (This)->lpVtbl -> ContinueWithEx(This,context,options,pTaskBody,pAsyncState,ppTask) ) 

#define IVsTask_Start(This)	\
    ( (This)->lpVtbl -> Start(This) ) 

#define IVsTask_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#define IVsTask_GetResult(This,pResult)	\
    ( (This)->lpVtbl -> GetResult(This,pResult) ) 

#define IVsTask_AbortIfCanceled(This)	\
    ( (This)->lpVtbl -> AbortIfCanceled(This) ) 

#define IVsTask_Wait(This)	\
    ( (This)->lpVtbl -> Wait(This) ) 

#define IVsTask_WaitEx(This,millisecondsTimeout,options,pTaskCompleted)	\
    ( (This)->lpVtbl -> WaitEx(This,millisecondsTimeout,options,pTaskCompleted) ) 

#define IVsTask_get_IsFaulted(This,pResult)	\
    ( (This)->lpVtbl -> get_IsFaulted(This,pResult) ) 

#define IVsTask_get_IsCompleted(This,pResult)	\
    ( (This)->lpVtbl -> get_IsCompleted(This,pResult) ) 

#define IVsTask_get_IsCanceled(This,pResult)	\
    ( (This)->lpVtbl -> get_IsCanceled(This,pResult) ) 

#define IVsTask_get_AsyncState(This,pAsyncState)	\
    ( (This)->lpVtbl -> get_AsyncState(This,pAsyncState) ) 

#define IVsTask_get_Description(This,ppDescriptionText)	\
    ( (This)->lpVtbl -> get_Description(This,ppDescriptionText) ) 

#define IVsTask_put_Description(This,pDescriptionText)	\
    ( (This)->lpVtbl -> put_Description(This,pDescriptionText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTask_INTERFACE_DEFINED__ */


#ifndef __IVsTaskCompletionSource_INTERFACE_DEFINED__
#define __IVsTaskCompletionSource_INTERFACE_DEFINED__

/* interface IVsTaskCompletionSource */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskCompletionSource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ce465203-16bc-4ebd-b4d1-9b4416b80931")
    IVsTaskCompletionSource : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Task( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetResult( 
            /* [in] */ VARIANT result) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCanceled( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFaulted( 
            /* [in] */ HRESULT hr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddDependentTask( 
            /* [in] */ __RPC__in_opt IVsTask *pTask) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskCompletionSourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTaskCompletionSource * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTaskCompletionSource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTaskCompletionSource * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Task )( 
            __RPC__in IVsTaskCompletionSource * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *SetResult )( 
            __RPC__in IVsTaskCompletionSource * This,
            /* [in] */ VARIANT result);
        
        HRESULT ( STDMETHODCALLTYPE *SetCanceled )( 
            __RPC__in IVsTaskCompletionSource * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetFaulted )( 
            __RPC__in IVsTaskCompletionSource * This,
            /* [in] */ HRESULT hr);
        
        HRESULT ( STDMETHODCALLTYPE *AddDependentTask )( 
            __RPC__in IVsTaskCompletionSource * This,
            /* [in] */ __RPC__in_opt IVsTask *pTask);
        
        END_INTERFACE
    } IVsTaskCompletionSourceVtbl;

    interface IVsTaskCompletionSource
    {
        CONST_VTBL struct IVsTaskCompletionSourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskCompletionSource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskCompletionSource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskCompletionSource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskCompletionSource_get_Task(This,ppTask)	\
    ( (This)->lpVtbl -> get_Task(This,ppTask) ) 

#define IVsTaskCompletionSource_SetResult(This,result)	\
    ( (This)->lpVtbl -> SetResult(This,result) ) 

#define IVsTaskCompletionSource_SetCanceled(This)	\
    ( (This)->lpVtbl -> SetCanceled(This) ) 

#define IVsTaskCompletionSource_SetFaulted(This,hr)	\
    ( (This)->lpVtbl -> SetFaulted(This,hr) ) 

#define IVsTaskCompletionSource_AddDependentTask(This,pTask)	\
    ( (This)->lpVtbl -> AddDependentTask(This,pTask) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskCompletionSource_INTERFACE_DEFINED__ */


#ifndef __IVsTaskSchedulerService_INTERFACE_DEFINED__
#define __IVsTaskSchedulerService_INTERFACE_DEFINED__

/* interface IVsTaskSchedulerService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskSchedulerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("83cfbaaf-0df9-403d-ae42-e738f0ac9735")
    IVsTaskSchedulerService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateTask( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTaskEx( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ VSTASKCREATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContinueWhenAllCompleted( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ DWORD dwTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTasks) IVsTask *pDependentTasks[  ],
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContinueWhenAllCompletedEx( 
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ DWORD dwTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTasks) IVsTask *pDependentTasks[  ],
            /* [in] */ VSTASKCONTINUATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTaskCompletionSource( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTaskCompletionSource **ppTaskSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTaskCompletionSourceEx( 
            /* [in] */ VSTASKCREATIONOPTIONS options,
            /* [in] */ VARIANT asyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTaskCompletionSource **ppTaskSource) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskSchedulerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTaskSchedulerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTaskSchedulerService * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTask )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTaskEx )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ VSTASKCREATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueWhenAllCompleted )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ DWORD dwTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTasks) IVsTask *pDependentTasks[  ],
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *ContinueWhenAllCompletedEx )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ VSTASKRUNCONTEXT context,
            /* [in] */ DWORD dwTasks,
            /* [size_is][in] */ __RPC__in_ecount_full(dwTasks) IVsTask *pDependentTasks[  ],
            /* [in] */ VSTASKCONTINUATIONOPTIONS options,
            /* [in] */ __RPC__in_opt IVsTaskBody *pTaskBody,
            /* [in] */ VARIANT pAsyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **ppTask);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTaskCompletionSource )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTaskCompletionSource **ppTaskSource);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTaskCompletionSourceEx )( 
            __RPC__in IVsTaskSchedulerService * This,
            /* [in] */ VSTASKCREATIONOPTIONS options,
            /* [in] */ VARIANT asyncState,
            /* [retval][out] */ __RPC__deref_out_opt IVsTaskCompletionSource **ppTaskSource);
        
        END_INTERFACE
    } IVsTaskSchedulerServiceVtbl;

    interface IVsTaskSchedulerService
    {
        CONST_VTBL struct IVsTaskSchedulerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskSchedulerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskSchedulerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskSchedulerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskSchedulerService_CreateTask(This,context,pTaskBody,ppTask)	\
    ( (This)->lpVtbl -> CreateTask(This,context,pTaskBody,ppTask) ) 

#define IVsTaskSchedulerService_CreateTaskEx(This,context,options,pTaskBody,pAsyncState,ppTask)	\
    ( (This)->lpVtbl -> CreateTaskEx(This,context,options,pTaskBody,pAsyncState,ppTask) ) 

#define IVsTaskSchedulerService_ContinueWhenAllCompleted(This,context,dwTasks,pDependentTasks,pTaskBody,ppTask)	\
    ( (This)->lpVtbl -> ContinueWhenAllCompleted(This,context,dwTasks,pDependentTasks,pTaskBody,ppTask) ) 

#define IVsTaskSchedulerService_ContinueWhenAllCompletedEx(This,context,dwTasks,pDependentTasks,options,pTaskBody,pAsyncState,ppTask)	\
    ( (This)->lpVtbl -> ContinueWhenAllCompletedEx(This,context,dwTasks,pDependentTasks,options,pTaskBody,pAsyncState,ppTask) ) 

#define IVsTaskSchedulerService_CreateTaskCompletionSource(This,ppTaskSource)	\
    ( (This)->lpVtbl -> CreateTaskCompletionSource(This,ppTaskSource) ) 

#define IVsTaskSchedulerService_CreateTaskCompletionSourceEx(This,options,asyncState,ppTaskSource)	\
    ( (This)->lpVtbl -> CreateTaskCompletionSourceEx(This,options,asyncState,ppTaskSource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskSchedulerService_INTERFACE_DEFINED__ */


#ifndef __SVsTaskSchedulerService_INTERFACE_DEFINED__
#define __SVsTaskSchedulerService_INTERFACE_DEFINED__

/* interface SVsTaskSchedulerService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_SVsTaskSchedulerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ab244925-40a8-4c5c-b0a5-717bb5d615b6")
    SVsTaskSchedulerService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsTaskSchedulerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsTaskSchedulerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsTaskSchedulerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsTaskSchedulerService * This);
        
        END_INTERFACE
    } SVsTaskSchedulerServiceVtbl;

    interface SVsTaskSchedulerService
    {
        CONST_VTBL struct SVsTaskSchedulerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsTaskSchedulerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsTaskSchedulerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsTaskSchedulerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsTaskSchedulerService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0119 */
/* [local] */ 


enum __VSIconSource
    {
        IS_Unknown	= 0,
        IS_VisualStudio	= 1,
        IS_OperatingSystem	= 2
    } ;
typedef DWORD VSIconSource;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0119_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0119_v0_0_s_ifspec;

#ifndef __IVsImageService_INTERFACE_DEFINED__
#define __IVsImageService_INTERFACE_DEFINED__

/* interface IVsImageService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsImageService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("59A26C3B-4E73-4E57-B1C2-F8A44BCA3CA1")
    IVsImageService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ __RPC__in_opt IVsUIObject *pIconObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Get( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIconForFile( 
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __VSUIDATAFORMAT desiredFormat,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIconForFileEx( 
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __VSUIDATAFORMAT desiredFormat,
            /* [out] */ __RPC__out VSIconSource *iconSource,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsImageServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsImageService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsImageService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsImageService * This);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            __RPC__in IVsImageService * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ __RPC__in_opt IVsUIObject *pIconObject);
        
        HRESULT ( STDMETHODCALLTYPE *Get )( 
            __RPC__in IVsImageService * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetIconForFile )( 
            __RPC__in IVsImageService * This,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __VSUIDATAFORMAT desiredFormat,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject);
        
        HRESULT ( STDMETHODCALLTYPE *GetIconForFileEx )( 
            __RPC__in IVsImageService * This,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __VSUIDATAFORMAT desiredFormat,
            /* [out] */ __RPC__out VSIconSource *iconSource,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppIconObject);
        
        END_INTERFACE
    } IVsImageServiceVtbl;

    interface IVsImageService
    {
        CONST_VTBL struct IVsImageServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsImageService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsImageService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsImageService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsImageService_Add(This,name,pIconObject)	\
    ( (This)->lpVtbl -> Add(This,name,pIconObject) ) 

#define IVsImageService_Get(This,name,ppIconObject)	\
    ( (This)->lpVtbl -> Get(This,name,ppIconObject) ) 

#define IVsImageService_GetIconForFile(This,filename,desiredFormat,ppIconObject)	\
    ( (This)->lpVtbl -> GetIconForFile(This,filename,desiredFormat,ppIconObject) ) 

#define IVsImageService_GetIconForFileEx(This,filename,desiredFormat,iconSource,ppIconObject)	\
    ( (This)->lpVtbl -> GetIconForFileEx(This,filename,desiredFormat,iconSource,ppIconObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsImageService_INTERFACE_DEFINED__ */


#ifndef __SVsImageService_INTERFACE_DEFINED__
#define __SVsImageService_INTERFACE_DEFINED__

/* interface SVsImageService */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_SVsImageService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ACC9EB93-CAD8-41DE-80DA-BD35CC5112AE")
    SVsImageService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsImageServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsImageService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsImageService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsImageService * This);
        
        END_INTERFACE
    } SVsImageServiceVtbl;

    interface SVsImageService
    {
        CONST_VTBL struct SVsImageServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsImageService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsImageService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsImageService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsImageService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0121 */
/* [local] */ 

#define SID_SVsImageService IID_SVsImageService

enum __VSPPROJECTUPGRADEVIAFACTORYREPAIRFLAGS
    {
        VSPUVF_PROJECT_NOREPAIR	= 0,
        VSPUVF_PROJECT_SAFEREPAIR	= 0x10,
        VSPUVF_PROJECT_UNSAFEREPAIR	= 0x20,
        VSPUVF_PROJECT_ONEWAYUPGRADE	= 0x40,
        VSPUVF_PROJECT_INCOMPATIBLE	= 0x80,
        VSPUVF_PROJECT_DEPRECATED	= 0x100
    } ;
typedef DWORD VSPUVF_REPAIRFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0121_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0121_v0_0_s_ifspec;

#ifndef __IVsProjectUpgradeViaFactory4_INTERFACE_DEFINED__
#define __IVsProjectUpgradeViaFactory4_INTERFACE_DEFINED__

/* interface IVsProjectUpgradeViaFactory4 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProjectUpgradeViaFactory4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9BE1F919-4C3C-45B7-9816-E04053F8ECC6")
    IVsProjectUpgradeViaFactory4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeProject_CheckOnly( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out VSPUVF_REPAIRFLAGS *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory,
            /* [out] */ __RPC__out VSPUVF_FLAGS *pUpgradeProjectCapabilityFlags) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectUpgradeViaFactory4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectUpgradeViaFactory4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectUpgradeViaFactory4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectUpgradeViaFactory4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProject_CheckOnly )( 
            __RPC__in IVsProjectUpgradeViaFactory4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out VSPUVF_REPAIRFLAGS *pUpgradeRequired,
            /* [out] */ __RPC__out GUID *pguidNewProjectFactory,
            /* [out] */ __RPC__out VSPUVF_FLAGS *pUpgradeProjectCapabilityFlags);
        
        END_INTERFACE
    } IVsProjectUpgradeViaFactory4Vtbl;

    interface IVsProjectUpgradeViaFactory4
    {
        CONST_VTBL struct IVsProjectUpgradeViaFactory4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectUpgradeViaFactory4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectUpgradeViaFactory4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectUpgradeViaFactory4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectUpgradeViaFactory4_UpgradeProject_CheckOnly(This,pszFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory,pUpgradeProjectCapabilityFlags)	\
    ( (This)->lpVtbl -> UpgradeProject_CheckOnly(This,pszFileName,pLogger,pUpgradeRequired,pguidNewProjectFactory,pUpgradeProjectCapabilityFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectUpgradeViaFactory4_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFlavorUpgradeViaFactory2_INTERFACE_DEFINED__
#define __IVsProjectFlavorUpgradeViaFactory2_INTERFACE_DEFINED__

/* interface IVsProjectFlavorUpgradeViaFactory2 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorUpgradeViaFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9B790CC4-A83D-4D55-B89D-C3DED06E8D31")
    IVsProjectFlavorUpgradeViaFactory2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpgradeProjectFlavor_CheckOnly( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IUnknown *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out VSPUVF_REPAIRFLAGS *pUpgradeRequired,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorUpgradeViaFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFlavorUpgradeViaFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFlavorUpgradeViaFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFlavorUpgradeViaFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpgradeProjectFlavor_CheckOnly )( 
            __RPC__in IVsProjectFlavorUpgradeViaFactory2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IUnknown *pUpgradeBuildPropStg,
            /* [in] */ __RPC__in LPCOLESTR pszProjFileXMLFragment,
            /* [in] */ __RPC__in LPCOLESTR pszUserFileXMLFragment,
            /* [in] */ __RPC__in_opt IVsUpgradeLogger *pLogger,
            /* [out] */ __RPC__out VSPUVF_REPAIRFLAGS *pUpgradeRequired,
            /* [optional][out] */ __RPC__out GUID *pguidNewProjectFactory);
        
        END_INTERFACE
    } IVsProjectFlavorUpgradeViaFactory2Vtbl;

    interface IVsProjectFlavorUpgradeViaFactory2
    {
        CONST_VTBL struct IVsProjectFlavorUpgradeViaFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorUpgradeViaFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorUpgradeViaFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorUpgradeViaFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorUpgradeViaFactory2_UpgradeProjectFlavor_CheckOnly(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pguidNewProjectFactory)	\
    ( (This)->lpVtbl -> UpgradeProjectFlavor_CheckOnly(This,pszFileName,pUpgradeBuildPropStg,pszProjFileXMLFragment,pszUserFileXMLFragment,pLogger,pUpgradeRequired,pguidNewProjectFactory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorUpgradeViaFactory2_INTERFACE_DEFINED__ */


#ifndef __IVsUpgradeLogger2_INTERFACE_DEFINED__
#define __IVsUpgradeLogger2_INTERFACE_DEFINED__

/* interface IVsUpgradeLogger2 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsUpgradeLogger2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DCF3E6DE-2DC0-421E-9DBB-375BB5255A80")
    IVsUpgradeLogger2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ChangeUpgradeLogPath( 
            /* [in] */ __RPC__in LPCOLESTR lpszNewLogFileName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUpgradeLogger2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUpgradeLogger2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUpgradeLogger2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUpgradeLogger2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ChangeUpgradeLogPath )( 
            __RPC__in IVsUpgradeLogger2 * This,
            /* [in] */ __RPC__in LPCOLESTR lpszNewLogFileName);
        
        END_INTERFACE
    } IVsUpgradeLogger2Vtbl;

    interface IVsUpgradeLogger2
    {
        CONST_VTBL struct IVsUpgradeLogger2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUpgradeLogger2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUpgradeLogger2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUpgradeLogger2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUpgradeLogger2_ChangeUpgradeLogPath(This,lpszNewLogFileName)	\
    ( (This)->lpVtbl -> ChangeUpgradeLogPath(This,lpszNewLogFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUpgradeLogger2_INTERFACE_DEFINED__ */


#ifndef __IVsSolution5_INTERFACE_DEFINED__
#define __IVsSolution5_INTERFACE_DEFINED__

/* interface IVsSolution5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolution5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("90570d49-7b10-4dcd-b9ac-530d91f4ebb5")
    IVsSolution5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolveFaultedProjects( 
            /* [in] */ DWORD cHierarchies,
            /* [size_is][in] */ __RPC__in_ecount_full(cHierarchies) IVsHierarchy *rgHierarchies[  ],
            /* [unique][in] */ __RPC__in_opt IVsPropertyBag *pProjectFaultResolutionContext,
            /* [out] */ __RPC__out DWORD *pcResolved,
            /* [out] */ __RPC__out DWORD *pcFailed) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGuidOfProjectFile( 
            /* [in] */ __RPC__in LPCOLESTR pszProjectFile,
            /* [retval][out] */ __RPC__out GUID *pProjectGuid) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolution5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolution5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolution5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolution5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveFaultedProjects )( 
            __RPC__in IVsSolution5 * This,
            /* [in] */ DWORD cHierarchies,
            /* [size_is][in] */ __RPC__in_ecount_full(cHierarchies) IVsHierarchy *rgHierarchies[  ],
            /* [unique][in] */ __RPC__in_opt IVsPropertyBag *pProjectFaultResolutionContext,
            /* [out] */ __RPC__out DWORD *pcResolved,
            /* [out] */ __RPC__out DWORD *pcFailed);
        
        HRESULT ( STDMETHODCALLTYPE *GetGuidOfProjectFile )( 
            __RPC__in IVsSolution5 * This,
            /* [in] */ __RPC__in LPCOLESTR pszProjectFile,
            /* [retval][out] */ __RPC__out GUID *pProjectGuid);
        
        END_INTERFACE
    } IVsSolution5Vtbl;

    interface IVsSolution5
    {
        CONST_VTBL struct IVsSolution5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolution5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolution5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolution5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolution5_ResolveFaultedProjects(This,cHierarchies,rgHierarchies,pProjectFaultResolutionContext,pcResolved,pcFailed)	\
    ( (This)->lpVtbl -> ResolveFaultedProjects(This,cHierarchies,rgHierarchies,pProjectFaultResolutionContext,pcResolved,pcFailed) ) 

#define IVsSolution5_GetGuidOfProjectFile(This,pszProjectFile,pProjectGuid)	\
    ( (This)->lpVtbl -> GetGuidOfProjectFile(This,pszProjectFile,pProjectGuid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolution5_INTERFACE_DEFINED__ */


#ifndef __IVsTaskList3_INTERFACE_DEFINED__
#define __IVsTaskList3_INTERFACE_DEFINED__

/* interface IVsTaskList3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskList3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6028FB96-E279-4707-8945-7A503AEC636E")
    IVsTaskList3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RefreshTasksAsync( 
            /* [in] */ VSCOOKIE providerCookie,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshOrAddTasksAsync( 
            /* [in] */ VSCOOKIE providerCookie,
            /* [in] */ int taskItemCount,
            /* [size_is][in] */ __RPC__in_ecount_full(taskItemCount) IVsTaskItem *taskItems[  ],
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveTasksAsync( 
            /* [in] */ VSCOOKIE providerCookie,
            /* [in] */ int taskItemCount,
            /* [size_is][in] */ __RPC__in_ecount_full(taskItemCount) IVsTaskItem *taskItems[  ],
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshAllProvidersAsync( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskList3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTaskList3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTaskList3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTaskList3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshTasksAsync )( 
            __RPC__in IVsTaskList3 * This,
            /* [in] */ VSCOOKIE providerCookie,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshOrAddTasksAsync )( 
            __RPC__in IVsTaskList3 * This,
            /* [in] */ VSCOOKIE providerCookie,
            /* [in] */ int taskItemCount,
            /* [size_is][in] */ __RPC__in_ecount_full(taskItemCount) IVsTaskItem *taskItems[  ],
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveTasksAsync )( 
            __RPC__in IVsTaskList3 * This,
            /* [in] */ VSCOOKIE providerCookie,
            /* [in] */ int taskItemCount,
            /* [size_is][in] */ __RPC__in_ecount_full(taskItemCount) IVsTaskItem *taskItems[  ],
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshAllProvidersAsync )( 
            __RPC__in IVsTaskList3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        END_INTERFACE
    } IVsTaskList3Vtbl;

    interface IVsTaskList3
    {
        CONST_VTBL struct IVsTaskList3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskList3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskList3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskList3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskList3_RefreshTasksAsync(This,providerCookie,task)	\
    ( (This)->lpVtbl -> RefreshTasksAsync(This,providerCookie,task) ) 

#define IVsTaskList3_RefreshOrAddTasksAsync(This,providerCookie,taskItemCount,taskItems,task)	\
    ( (This)->lpVtbl -> RefreshOrAddTasksAsync(This,providerCookie,taskItemCount,taskItems,task) ) 

#define IVsTaskList3_RemoveTasksAsync(This,providerCookie,taskItemCount,taskItems,task)	\
    ( (This)->lpVtbl -> RemoveTasksAsync(This,providerCookie,taskItemCount,taskItems,task) ) 

#define IVsTaskList3_RefreshAllProvidersAsync(This,task)	\
    ( (This)->lpVtbl -> RefreshAllProvidersAsync(This,task) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskList3_INTERFACE_DEFINED__ */


#ifndef __IVsPersistSolutionOpts2_INTERFACE_DEFINED__
#define __IVsPersistSolutionOpts2_INTERFACE_DEFINED__

/* interface IVsPersistSolutionOpts2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPersistSolutionOpts2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d79ca884-27fc-43f4-a51b-d0b068b312c9")
    IVsPersistSolutionOpts2 : public IVsPersistSolutionOpts
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE LoadUserOptionsEx( 
            /* [in] */ BOOL fPreLoad,
            /* [in] */ __RPC__in_opt IVsSolutionPersistence *pPersistence,
            /* [in] */ VSLOADUSEROPTS grfLoadOpts) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPersistSolutionOpts2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPersistSolutionOpts2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPersistSolutionOpts2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SaveUserOptions )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ __RPC__in_opt IVsSolutionPersistence *pPersistence);
        
        HRESULT ( STDMETHODCALLTYPE *LoadUserOptions )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ __RPC__in_opt IVsSolutionPersistence *pPersistence,
            /* [in] */ VSLOADUSEROPTS grfLoadOpts);
        
        HRESULT ( STDMETHODCALLTYPE *WriteUserOptions )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ __RPC__in_opt IStream *pOptionsStream,
            /* [in] */ __RPC__in LPCOLESTR pszKey);
        
        HRESULT ( STDMETHODCALLTYPE *ReadUserOptions )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ __RPC__in_opt IStream *pOptionsStream,
            /* [in] */ __RPC__in LPCOLESTR pszKey);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *LoadUserOptionsEx )( 
            __RPC__in IVsPersistSolutionOpts2 * This,
            /* [in] */ BOOL fPreLoad,
            /* [in] */ __RPC__in_opt IVsSolutionPersistence *pPersistence,
            /* [in] */ VSLOADUSEROPTS grfLoadOpts);
        
        END_INTERFACE
    } IVsPersistSolutionOpts2Vtbl;

    interface IVsPersistSolutionOpts2
    {
        CONST_VTBL struct IVsPersistSolutionOpts2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPersistSolutionOpts2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPersistSolutionOpts2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPersistSolutionOpts2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPersistSolutionOpts2_SaveUserOptions(This,pPersistence)	\
    ( (This)->lpVtbl -> SaveUserOptions(This,pPersistence) ) 

#define IVsPersistSolutionOpts2_LoadUserOptions(This,pPersistence,grfLoadOpts)	\
    ( (This)->lpVtbl -> LoadUserOptions(This,pPersistence,grfLoadOpts) ) 

#define IVsPersistSolutionOpts2_WriteUserOptions(This,pOptionsStream,pszKey)	\
    ( (This)->lpVtbl -> WriteUserOptions(This,pOptionsStream,pszKey) ) 

#define IVsPersistSolutionOpts2_ReadUserOptions(This,pOptionsStream,pszKey)	\
    ( (This)->lpVtbl -> ReadUserOptions(This,pOptionsStream,pszKey) ) 


#define IVsPersistSolutionOpts2_LoadUserOptionsEx(This,fPreLoad,pPersistence,grfLoadOpts)	\
    ( (This)->lpVtbl -> LoadUserOptionsEx(This,fPreLoad,pPersistence,grfLoadOpts) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPersistSolutionOpts2_INTERFACE_DEFINED__ */


#ifndef __IVsAsynchronousProjectCreate_INTERFACE_DEFINED__
#define __IVsAsynchronousProjectCreate_INTERFACE_DEFINED__

/* interface IVsAsynchronousProjectCreate */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAsynchronousProjectCreate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D1BB7312-01F8-49F7-8E9A-087434BD154A")
    IVsAsynchronousProjectCreate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CanCreateProjectAsynchronously( 
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ VSCREATEPROJFLAGS flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *canOpenAsync) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeCreateProjectAsync( 
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in LPCOLESTR location,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateProjectAsync( 
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in LPCOLESTR location,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS flags,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAsynchronousProjectCreateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAsynchronousProjectCreate * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAsynchronousProjectCreate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAsynchronousProjectCreate * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanCreateProjectAsynchronously )( 
            __RPC__in IVsAsynchronousProjectCreate * This,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ VSCREATEPROJFLAGS flags,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *canOpenAsync);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeCreateProjectAsync )( 
            __RPC__in IVsAsynchronousProjectCreate * This,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in LPCOLESTR location,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS flags);
        
        HRESULT ( STDMETHODCALLTYPE *CreateProjectAsync )( 
            __RPC__in IVsAsynchronousProjectCreate * This,
            /* [in] */ __RPC__in REFGUID rguidProjectID,
            /* [in] */ __RPC__in LPCOLESTR filename,
            /* [in] */ __RPC__in LPCOLESTR location,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSCREATEPROJFLAGS flags,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        END_INTERFACE
    } IVsAsynchronousProjectCreateVtbl;

    interface IVsAsynchronousProjectCreate
    {
        CONST_VTBL struct IVsAsynchronousProjectCreateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAsynchronousProjectCreate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAsynchronousProjectCreate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAsynchronousProjectCreate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAsynchronousProjectCreate_CanCreateProjectAsynchronously(This,rguidProjectID,filename,flags,canOpenAsync)	\
    ( (This)->lpVtbl -> CanCreateProjectAsynchronously(This,rguidProjectID,filename,flags,canOpenAsync) ) 

#define IVsAsynchronousProjectCreate_OnBeforeCreateProjectAsync(This,rguidProjectID,filename,location,pszName,flags)	\
    ( (This)->lpVtbl -> OnBeforeCreateProjectAsync(This,rguidProjectID,filename,location,pszName,flags) ) 

#define IVsAsynchronousProjectCreate_CreateProjectAsync(This,rguidProjectID,filename,location,pszName,flags,task)	\
    ( (This)->lpVtbl -> CreateProjectAsync(This,rguidProjectID,filename,location,pszName,flags,task) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAsynchronousProjectCreate_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionEvents5_INTERFACE_DEFINED__
#define __IVsSolutionEvents5_INTERFACE_DEFINED__

/* interface IVsSolutionEvents5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionEvents5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("af530689-9987-48be-af20-d9392a9c67ff")
    IVsSolutionEvents5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeOpenProject( 
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ __RPC__in REFGUID guidProjectType,
            /* [in] */ __RPC__in LPCOLESTR pszFileName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionEvents5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionEvents5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionEvents5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionEvents5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeOpenProject )( 
            __RPC__in IVsSolutionEvents5 * This,
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ __RPC__in REFGUID guidProjectType,
            /* [in] */ __RPC__in LPCOLESTR pszFileName);
        
        END_INTERFACE
    } IVsSolutionEvents5Vtbl;

    interface IVsSolutionEvents5
    {
        CONST_VTBL struct IVsSolutionEvents5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionEvents5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionEvents5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionEvents5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionEvents5_OnBeforeOpenProject(This,guidProjectID,guidProjectType,pszFileName)	\
    ( (This)->lpVtbl -> OnBeforeOpenProject(This,guidProjectID,guidProjectType,pszFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionEvents5_INTERFACE_DEFINED__ */


#ifndef __IVsHierarchyEvents2_INTERFACE_DEFINED__
#define __IVsHierarchyEvents2_INTERFACE_DEFINED__

/* interface IVsHierarchyEvents2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyEvents2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0D707B06-F2E8-4556-A98A-D015B67AE3EF")
    IVsHierarchyEvents2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnItemAdded( 
            /* [in] */ VSITEMID itemidParent,
            /* [in] */ VSITEMID itemidSiblingPrev,
            /* [in] */ VSITEMID itemidAdded,
            /* [in] */ VARIANT_BOOL ensureVisible) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHierarchyEvents2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHierarchyEvents2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHierarchyEvents2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHierarchyEvents2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnItemAdded )( 
            __RPC__in IVsHierarchyEvents2 * This,
            /* [in] */ VSITEMID itemidParent,
            /* [in] */ VSITEMID itemidSiblingPrev,
            /* [in] */ VSITEMID itemidAdded,
            /* [in] */ VARIANT_BOOL ensureVisible);
        
        END_INTERFACE
    } IVsHierarchyEvents2Vtbl;

    interface IVsHierarchyEvents2
    {
        CONST_VTBL struct IVsHierarchyEvents2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyEvents2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyEvents2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyEvents2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyEvents2_OnItemAdded(This,itemidParent,itemidSiblingPrev,itemidAdded,ensureVisible)	\
    ( (This)->lpVtbl -> OnItemAdded(This,itemidParent,itemidSiblingPrev,itemidAdded,ensureVisible) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyEvents2_INTERFACE_DEFINED__ */


#ifndef __IVsAsynchronousProjectCreateUI_INTERFACE_DEFINED__
#define __IVsAsynchronousProjectCreateUI_INTERFACE_DEFINED__

/* interface IVsAsynchronousProjectCreateUI */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAsynchronousProjectCreateUI;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("808D838C-5821-4E4D-9399-0AB656A64E48")
    IVsAsynchronousProjectCreateUI : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterProjectProvisioned( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAsynchronousProjectCreateUIVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAsynchronousProjectCreateUI * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAsynchronousProjectCreateUI * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAsynchronousProjectCreateUI * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterProjectProvisioned )( 
            __RPC__in IVsAsynchronousProjectCreateUI * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier);
        
        END_INTERFACE
    } IVsAsynchronousProjectCreateUIVtbl;

    interface IVsAsynchronousProjectCreateUI
    {
        CONST_VTBL struct IVsAsynchronousProjectCreateUIVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAsynchronousProjectCreateUI_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAsynchronousProjectCreateUI_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAsynchronousProjectCreateUI_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAsynchronousProjectCreateUI_OnAfterProjectProvisioned(This,pHier)	\
    ( (This)->lpVtbl -> OnAfterProjectProvisioned(This,pHier) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAsynchronousProjectCreateUI_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionUIEvents_INTERFACE_DEFINED__
#define __IVsSolutionUIEvents_INTERFACE_DEFINED__

/* interface IVsSolutionUIEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionUIEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("56870613-AB44-40B6-8125-F0D82D566C26")
    IVsSolutionUIEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnFilterChanged( 
            /* [in] */ __RPC__in REFGUID pguidOldFilterGoup,
            /* [in] */ UINT nOldFilterID,
            /* [in] */ __RPC__in REFGUID pguidNewFilterGroup,
            /* [in] */ UINT nNewFilterID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnFilterAsyncLoadStarted( 
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnFilterAsyncLoadCompleted( 
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionUIEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionUIEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionUIEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionUIEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnFilterChanged )( 
            __RPC__in IVsSolutionUIEvents * This,
            /* [in] */ __RPC__in REFGUID pguidOldFilterGoup,
            /* [in] */ UINT nOldFilterID,
            /* [in] */ __RPC__in REFGUID pguidNewFilterGroup,
            /* [in] */ UINT nNewFilterID);
        
        HRESULT ( STDMETHODCALLTYPE *OnFilterAsyncLoadStarted )( 
            __RPC__in IVsSolutionUIEvents * This,
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID);
        
        HRESULT ( STDMETHODCALLTYPE *OnFilterAsyncLoadCompleted )( 
            __RPC__in IVsSolutionUIEvents * This,
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID);
        
        END_INTERFACE
    } IVsSolutionUIEventsVtbl;

    interface IVsSolutionUIEvents
    {
        CONST_VTBL struct IVsSolutionUIEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionUIEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionUIEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionUIEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionUIEvents_OnFilterChanged(This,pguidOldFilterGoup,nOldFilterID,pguidNewFilterGroup,nNewFilterID)	\
    ( (This)->lpVtbl -> OnFilterChanged(This,pguidOldFilterGoup,nOldFilterID,pguidNewFilterGroup,nNewFilterID) ) 

#define IVsSolutionUIEvents_OnFilterAsyncLoadStarted(This,pguidFilterGroup,nFilterID)	\
    ( (This)->lpVtbl -> OnFilterAsyncLoadStarted(This,pguidFilterGroup,nFilterID) ) 

#define IVsSolutionUIEvents_OnFilterAsyncLoadCompleted(This,pguidFilterGroup,nFilterID)	\
    ( (This)->lpVtbl -> OnFilterAsyncLoadCompleted(This,pguidFilterGroup,nFilterID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionUIEvents_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionUIHierarchyWindow_INTERFACE_DEFINED__
#define __IVsSolutionUIHierarchyWindow_INTERFACE_DEFINED__

/* interface IVsSolutionUIHierarchyWindow */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionUIHierarchyWindow;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D286024E-6940-4D08-986D-CE82E732BAAB")
    IVsSolutionUIHierarchyWindow : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnableFilter( 
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pSuccessful) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisableFilter( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsFilterEnabled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentFilter( 
            /* [out] */ __RPC__out GUID *pguidFilterGroup,
            /* [out] */ __RPC__out UINT *nFilterID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseSolutionUIEvents( 
            /* [in] */ __RPC__in_opt IVsSolutionUIEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseSolutionUIEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionUIHierarchyWindowVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnableFilter )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [in] */ __RPC__in REFGUID pguidFilterGroup,
            /* [in] */ UINT nFilterID,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pSuccessful);
        
        HRESULT ( STDMETHODCALLTYPE *DisableFilter )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsFilterEnabled )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pIsEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentFilter )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [out] */ __RPC__out GUID *pguidFilterGroup,
            /* [out] */ __RPC__out UINT *nFilterID);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseSolutionUIEvents )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [in] */ __RPC__in_opt IVsSolutionUIEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseSolutionUIEvents )( 
            __RPC__in IVsSolutionUIHierarchyWindow * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsSolutionUIHierarchyWindowVtbl;

    interface IVsSolutionUIHierarchyWindow
    {
        CONST_VTBL struct IVsSolutionUIHierarchyWindowVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionUIHierarchyWindow_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionUIHierarchyWindow_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionUIHierarchyWindow_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionUIHierarchyWindow_EnableFilter(This,pguidFilterGroup,nFilterID,pSuccessful)	\
    ( (This)->lpVtbl -> EnableFilter(This,pguidFilterGroup,nFilterID,pSuccessful) ) 

#define IVsSolutionUIHierarchyWindow_DisableFilter(This)	\
    ( (This)->lpVtbl -> DisableFilter(This) ) 

#define IVsSolutionUIHierarchyWindow_IsFilterEnabled(This,pIsEnabled)	\
    ( (This)->lpVtbl -> IsFilterEnabled(This,pIsEnabled) ) 

#define IVsSolutionUIHierarchyWindow_GetCurrentFilter(This,pguidFilterGroup,nFilterID)	\
    ( (This)->lpVtbl -> GetCurrentFilter(This,pguidFilterGroup,nFilterID) ) 

#define IVsSolutionUIHierarchyWindow_AdviseSolutionUIEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseSolutionUIEvents(This,pEventSink,pdwCookie) ) 

#define IVsSolutionUIHierarchyWindow_UnadviseSolutionUIEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseSolutionUIEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionUIHierarchyWindow_INTERFACE_DEFINED__ */


#ifndef __IVsPrioritizedSolutionEventsSink_INTERFACE_DEFINED__
#define __IVsPrioritizedSolutionEventsSink_INTERFACE_DEFINED__

/* interface IVsPrioritizedSolutionEventsSink */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPrioritizedSolutionEventsSink;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6E8674B2-EFA9-4CD6-9743-CC7A549BB0EB")
    IVsPrioritizedSolutionEventsSink : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPrioritizedSolutionEventsSinkVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPrioritizedSolutionEventsSink * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPrioritizedSolutionEventsSink * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPrioritizedSolutionEventsSink * This);
        
        END_INTERFACE
    } IVsPrioritizedSolutionEventsSinkVtbl;

    interface IVsPrioritizedSolutionEventsSink
    {
        CONST_VTBL struct IVsPrioritizedSolutionEventsSinkVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPrioritizedSolutionEventsSink_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPrioritizedSolutionEventsSink_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPrioritizedSolutionEventsSink_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPrioritizedSolutionEventsSink_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0134 */
/* [local] */ 


enum __VSOVERLAYICON3
    {
        OVERLAYICON_FAULTED	= 7,
        OVERLAYICON_MAXINDEX3	= 7
    } ;
typedef DWORD VSOVERLAYICON3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0134_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0134_v0_0_s_ifspec;

#ifndef __IVsManifestReferenceResolver_INTERFACE_DEFINED__
#define __IVsManifestReferenceResolver_INTERFACE_DEFINED__

/* interface IVsManifestReferenceResolver */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsManifestReferenceResolver;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DA5C54DC-7F35-4149-8E1C-948DEBA1CA92")
    IVsManifestReferenceResolver : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolveReferenceAsync( 
            /* [in] */ __RPC__in LPCOLESTR reference,
            /* [in] */ __RPC__in LPCOLESTR relativeToFile,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsManifestReferenceResolverVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsManifestReferenceResolver * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsManifestReferenceResolver * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsManifestReferenceResolver * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveReferenceAsync )( 
            __RPC__in IVsManifestReferenceResolver * This,
            /* [in] */ __RPC__in LPCOLESTR reference,
            /* [in] */ __RPC__in LPCOLESTR relativeToFile,
            /* [retval][out] */ __RPC__deref_out_opt IVsTask **task);
        
        END_INTERFACE
    } IVsManifestReferenceResolverVtbl;

    interface IVsManifestReferenceResolver
    {
        CONST_VTBL struct IVsManifestReferenceResolverVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsManifestReferenceResolver_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsManifestReferenceResolver_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsManifestReferenceResolver_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsManifestReferenceResolver_ResolveReferenceAsync(This,reference,relativeToFile,task)	\
    ( (This)->lpVtbl -> ResolveReferenceAsync(This,reference,relativeToFile,task) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsManifestReferenceResolver_INTERFACE_DEFINED__ */


#ifndef __IVsThreadedWaitDialogCallback_INTERFACE_DEFINED__
#define __IVsThreadedWaitDialogCallback_INTERFACE_DEFINED__

/* interface IVsThreadedWaitDialogCallback */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadedWaitDialogCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f5caf6e8-ad40-4d9d-8366-a919f03ca969")
    IVsThreadedWaitDialogCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnCanceled( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsThreadedWaitDialogCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsThreadedWaitDialogCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsThreadedWaitDialogCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsThreadedWaitDialogCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnCanceled )( 
            __RPC__in IVsThreadedWaitDialogCallback * This);
        
        END_INTERFACE
    } IVsThreadedWaitDialogCallbackVtbl;

    interface IVsThreadedWaitDialogCallback
    {
        CONST_VTBL struct IVsThreadedWaitDialogCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadedWaitDialogCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadedWaitDialogCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadedWaitDialogCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadedWaitDialogCallback_OnCanceled(This)	\
    ( (This)->lpVtbl -> OnCanceled(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadedWaitDialogCallback_INTERFACE_DEFINED__ */


#ifndef __IVsThreadedWaitDialog3_INTERFACE_DEFINED__
#define __IVsThreadedWaitDialog3_INTERFACE_DEFINED__

/* interface IVsThreadedWaitDialog3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadedWaitDialog3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e92e3159-2381-4179-a500-9676dee38896")
    IVsThreadedWaitDialog3 : public IVsThreadedWaitDialog2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartWaitDialogWithCallback( 
            /* [unique][in] */ __RPC__in_opt LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szStatusBarText,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ VARIANT_BOOL fShowProgress,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ LONG iCurrentStep,
            /* [in] */ __RPC__in_opt IVsThreadedWaitDialogCallback *pCallback) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsThreadedWaitDialog3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsThreadedWaitDialog3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsThreadedWaitDialog3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialog )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [in] */ __RPC__in LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ VARIANT_BOOL fShowMarqueeProgress);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialogWithPercentageProgress )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [in] */ __RPC__in LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ LONG iCurrentStep);
        
        HRESULT ( STDMETHODCALLTYPE *EndWaitDialog )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [out] */ __RPC__out BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateProgress )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [in] */ __RPC__in LPCWSTR szUpdatedWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iCurrentStep,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ VARIANT_BOOL fDisableCancel,
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *HasCanceled )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialogWithCallback )( 
            __RPC__in IVsThreadedWaitDialog3 * This,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szStatusBarText,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ VARIANT_BOOL fShowProgress,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ LONG iCurrentStep,
            /* [in] */ __RPC__in_opt IVsThreadedWaitDialogCallback *pCallback);
        
        END_INTERFACE
    } IVsThreadedWaitDialog3Vtbl;

    interface IVsThreadedWaitDialog3
    {
        CONST_VTBL struct IVsThreadedWaitDialog3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadedWaitDialog3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadedWaitDialog3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadedWaitDialog3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadedWaitDialog3_StartWaitDialog(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,iDelayToShowDialog,fIsCancelable,fShowMarqueeProgress)	\
    ( (This)->lpVtbl -> StartWaitDialog(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,iDelayToShowDialog,fIsCancelable,fShowMarqueeProgress) ) 

#define IVsThreadedWaitDialog3_StartWaitDialogWithPercentageProgress(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,iTotalSteps,iCurrentStep)	\
    ( (This)->lpVtbl -> StartWaitDialogWithPercentageProgress(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,iTotalSteps,iCurrentStep) ) 

#define IVsThreadedWaitDialog3_EndWaitDialog(This,pfCanceled)	\
    ( (This)->lpVtbl -> EndWaitDialog(This,pfCanceled) ) 

#define IVsThreadedWaitDialog3_UpdateProgress(This,szUpdatedWaitMessage,szProgressText,szStatusBarText,iCurrentStep,iTotalSteps,fDisableCancel,pfCanceled)	\
    ( (This)->lpVtbl -> UpdateProgress(This,szUpdatedWaitMessage,szProgressText,szStatusBarText,iCurrentStep,iTotalSteps,fDisableCancel,pfCanceled) ) 

#define IVsThreadedWaitDialog3_HasCanceled(This,pfCanceled)	\
    ( (This)->lpVtbl -> HasCanceled(This,pfCanceled) ) 


#define IVsThreadedWaitDialog3_StartWaitDialogWithCallback(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,fShowProgress,iTotalSteps,iCurrentStep,pCallback)	\
    ( (This)->lpVtbl -> StartWaitDialogWithCallback(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,fShowProgress,iTotalSteps,iCurrentStep,pCallback) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadedWaitDialog3_INTERFACE_DEFINED__ */


#ifndef __SVsHierarchyManipulation_INTERFACE_DEFINED__
#define __SVsHierarchyManipulation_INTERFACE_DEFINED__

/* interface SVsHierarchyManipulation */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_SVsHierarchyManipulation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1C917A11-5B6E-4752-9DEC-94B041A05745")
    SVsHierarchyManipulation : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsHierarchyManipulationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsHierarchyManipulation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsHierarchyManipulation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsHierarchyManipulation * This);
        
        END_INTERFACE
    } SVsHierarchyManipulationVtbl;

    interface SVsHierarchyManipulation
    {
        CONST_VTBL struct SVsHierarchyManipulationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsHierarchyManipulation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsHierarchyManipulation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsHierarchyManipulation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsHierarchyManipulation_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell110_0000_0138 */
/* [local] */ 

#define SID_SVsHierarchyManipulation __uuidof(SVsHierarchyManipulation)

enum __VSHIERARCHYMANIPULATIONSTATE
    {
        HMS_Unspecified	= 0,
        HMS_System	= 0x1
    } ;
typedef DWORD VSHIERARCHYMANIPULATIONSTATE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0138_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell110_0000_0138_v0_0_s_ifspec;

#ifndef __IVsHierarchyManipulation_INTERFACE_DEFINED__
#define __IVsHierarchyManipulation_INTERFACE_DEFINED__

/* interface IVsHierarchyManipulation */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyManipulation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F3581DB8-1DCE-4267-8145-B6906AC4D028")
    IVsHierarchyManipulation : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetManipulationState( 
            /* [in] */ VSHIERARCHYMANIPULATIONSTATE state,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchyManipulationStateContext **ppRestoreOnRelease) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHierarchyManipulationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHierarchyManipulation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHierarchyManipulation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHierarchyManipulation * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetManipulationState )( 
            __RPC__in IVsHierarchyManipulation * This,
            /* [in] */ VSHIERARCHYMANIPULATIONSTATE state,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchyManipulationStateContext **ppRestoreOnRelease);
        
        END_INTERFACE
    } IVsHierarchyManipulationVtbl;

    interface IVsHierarchyManipulation
    {
        CONST_VTBL struct IVsHierarchyManipulationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyManipulation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyManipulation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyManipulation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyManipulation_SetManipulationState(This,state,ppRestoreOnRelease)	\
    ( (This)->lpVtbl -> SetManipulationState(This,state,ppRestoreOnRelease) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyManipulation_INTERFACE_DEFINED__ */


#ifndef __IVsHierarchyManipulationStateContext_INTERFACE_DEFINED__
#define __IVsHierarchyManipulationStateContext_INTERFACE_DEFINED__

/* interface IVsHierarchyManipulationStateContext */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHierarchyManipulationStateContext;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E9274A0A-CC2F-4ECB-9FD0-F7BB59C7A47F")
    IVsHierarchyManipulationStateContext : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Restore( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHierarchyManipulationStateContextVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHierarchyManipulationStateContext * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHierarchyManipulationStateContext * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHierarchyManipulationStateContext * This);
        
        HRESULT ( STDMETHODCALLTYPE *Restore )( 
            __RPC__in IVsHierarchyManipulationStateContext * This);
        
        END_INTERFACE
    } IVsHierarchyManipulationStateContextVtbl;

    interface IVsHierarchyManipulationStateContext
    {
        CONST_VTBL struct IVsHierarchyManipulationStateContextVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHierarchyManipulationStateContext_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHierarchyManipulationStateContext_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHierarchyManipulationStateContext_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHierarchyManipulationStateContext_Restore(This)	\
    ( (This)->lpVtbl -> Restore(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHierarchyManipulationStateContext_INTERFACE_DEFINED__ */


#ifndef __IVsFontAndColorStorage3_INTERFACE_DEFINED__
#define __IVsFontAndColorStorage3_INTERFACE_DEFINED__

/* interface IVsFontAndColorStorage3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFontAndColorStorage3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BAA340BB-FA34-4CCB-8C81-436566368517")
    IVsFontAndColorStorage3 : public IVsFontAndColorStorage2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RevertAllCategoriesToDefault( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFontAndColorStorage3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFontAndColorStorage3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFontAndColorStorage3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFontAndColorStorage3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RevertFontToDefault )( 
            __RPC__in IVsFontAndColorStorage3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RevertItemToDefault )( 
            __RPC__in IVsFontAndColorStorage3 * This,
            /* [in] */ __RPC__in LPCOLESTR szName);
        
        HRESULT ( STDMETHODCALLTYPE *RevertAllItemsToDefault )( 
            __RPC__in IVsFontAndColorStorage3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RevertAllCategoriesToDefault )( 
            __RPC__in IVsFontAndColorStorage3 * This);
        
        END_INTERFACE
    } IVsFontAndColorStorage3Vtbl;

    interface IVsFontAndColorStorage3
    {
        CONST_VTBL struct IVsFontAndColorStorage3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFontAndColorStorage3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFontAndColorStorage3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFontAndColorStorage3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFontAndColorStorage3_RevertFontToDefault(This)	\
    ( (This)->lpVtbl -> RevertFontToDefault(This) ) 

#define IVsFontAndColorStorage3_RevertItemToDefault(This,szName)	\
    ( (This)->lpVtbl -> RevertItemToDefault(This,szName) ) 

#define IVsFontAndColorStorage3_RevertAllItemsToDefault(This)	\
    ( (This)->lpVtbl -> RevertAllItemsToDefault(This) ) 


#define IVsFontAndColorStorage3_RevertAllCategoriesToDefault(This)	\
    ( (This)->lpVtbl -> RevertAllCategoriesToDefault(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFontAndColorStorage3_INTERFACE_DEFINED__ */


#ifndef __IVsTaskProvider4_INTERFACE_DEFINED__
#define __IVsTaskProvider4_INTERFACE_DEFINED__

/* interface IVsTaskProvider4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTaskProvider4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6F6E00CC-2261-49E2-8FD9-356B6637A2D9")
    IVsTaskProvider4 : public IUnknown
    {
    public:
        virtual /* [propget][local] */ HRESULT STDMETHODCALLTYPE get_ThemedImageList( 
            /* [retval][out] */ HANDLE *phImageList) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTaskProvider4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTaskProvider4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTaskProvider4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTaskProvider4 * This);
        
        /* [propget][local] */ HRESULT ( STDMETHODCALLTYPE *get_ThemedImageList )( 
            IVsTaskProvider4 * This,
            /* [retval][out] */ HANDLE *phImageList);
        
        END_INTERFACE
    } IVsTaskProvider4Vtbl;

    interface IVsTaskProvider4
    {
        CONST_VTBL struct IVsTaskProvider4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTaskProvider4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTaskProvider4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTaskProvider4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTaskProvider4_get_ThemedImageList(This,phImageList)	\
    ( (This)->lpVtbl -> get_ThemedImageList(This,phImageList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTaskProvider4_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  HWND_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out HWND * ); 
void                      __RPC_USER  HWND_UserFree(     __RPC__in unsigned long *, __RPC__in HWND * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     __RPC__in unsigned long *, __RPC__in LPSAFEARRAY * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     __RPC__in unsigned long *, __RPC__in VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


