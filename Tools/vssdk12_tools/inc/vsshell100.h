

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

#ifndef __vsshell100_h__
#define __vsshell100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsDesignTimeAssemblyResolution_FWD_DEFINED__
#define __IVsDesignTimeAssemblyResolution_FWD_DEFINED__
typedef interface IVsDesignTimeAssemblyResolution IVsDesignTimeAssemblyResolution;

#endif 	/* __IVsDesignTimeAssemblyResolution_FWD_DEFINED__ */


#ifndef __SVsDesignTimeAssemblyResolution_FWD_DEFINED__
#define __SVsDesignTimeAssemblyResolution_FWD_DEFINED__
typedef interface SVsDesignTimeAssemblyResolution SVsDesignTimeAssemblyResolution;

#endif 	/* __SVsDesignTimeAssemblyResolution_FWD_DEFINED__ */


#ifndef __IVsEditorFactoryChooser_FWD_DEFINED__
#define __IVsEditorFactoryChooser_FWD_DEFINED__
typedef interface IVsEditorFactoryChooser IVsEditorFactoryChooser;

#endif 	/* __IVsEditorFactoryChooser_FWD_DEFINED__ */


#ifndef __IVsShell4_FWD_DEFINED__
#define __IVsShell4_FWD_DEFINED__
typedef interface IVsShell4 IVsShell4;

#endif 	/* __IVsShell4_FWD_DEFINED__ */


#ifndef __IVsToolbarTrayHost_FWD_DEFINED__
#define __IVsToolbarTrayHost_FWD_DEFINED__
typedef interface IVsToolbarTrayHost IVsToolbarTrayHost;

#endif 	/* __IVsToolbarTrayHost_FWD_DEFINED__ */


#ifndef __IVsUIShell4_FWD_DEFINED__
#define __IVsUIShell4_FWD_DEFINED__
typedef interface IVsUIShell4 IVsUIShell4;

#endif 	/* __IVsUIShell4_FWD_DEFINED__ */


#ifndef __IVsToolWindowToolbarHost2_FWD_DEFINED__
#define __IVsToolWindowToolbarHost2_FWD_DEFINED__
typedef interface IVsToolWindowToolbarHost2 IVsToolWindowToolbarHost2;

#endif 	/* __IVsToolWindowToolbarHost2_FWD_DEFINED__ */


#ifndef __IVsUIElementPane_FWD_DEFINED__
#define __IVsUIElementPane_FWD_DEFINED__
typedef interface IVsUIElementPane IVsUIElementPane;

#endif 	/* __IVsUIElementPane_FWD_DEFINED__ */


#ifndef __IVsWindowFrame3_FWD_DEFINED__
#define __IVsWindowFrame3_FWD_DEFINED__
typedef interface IVsWindowFrame3 IVsWindowFrame3;

#endif 	/* __IVsWindowFrame3_FWD_DEFINED__ */


#ifndef __IVsWindowFrameSwitcher_FWD_DEFINED__
#define __IVsWindowFrameSwitcher_FWD_DEFINED__
typedef interface IVsWindowFrameSwitcher IVsWindowFrameSwitcher;

#endif 	/* __IVsWindowFrameSwitcher_FWD_DEFINED__ */


#ifndef __IVsConfigureToolboxItem_FWD_DEFINED__
#define __IVsConfigureToolboxItem_FWD_DEFINED__
typedef interface IVsConfigureToolboxItem IVsConfigureToolboxItem;

#endif 	/* __IVsConfigureToolboxItem_FWD_DEFINED__ */


#ifndef __IVsDebugger3_FWD_DEFINED__
#define __IVsDebugger3_FWD_DEFINED__
typedef interface IVsDebugger3 IVsDebugger3;

#endif 	/* __IVsDebugger3_FWD_DEFINED__ */


#ifndef __IVsDebugLaunchHook_FWD_DEFINED__
#define __IVsDebugLaunchHook_FWD_DEFINED__
typedef interface IVsDebugLaunchHook IVsDebugLaunchHook;

#endif 	/* __IVsDebugLaunchHook_FWD_DEFINED__ */


#ifndef __SVsSettingsManager_FWD_DEFINED__
#define __SVsSettingsManager_FWD_DEFINED__
typedef interface SVsSettingsManager SVsSettingsManager;

#endif 	/* __SVsSettingsManager_FWD_DEFINED__ */


#ifndef __IVsSettingsStore_FWD_DEFINED__
#define __IVsSettingsStore_FWD_DEFINED__
typedef interface IVsSettingsStore IVsSettingsStore;

#endif 	/* __IVsSettingsStore_FWD_DEFINED__ */


#ifndef __IVsWritableSettingsStore_FWD_DEFINED__
#define __IVsWritableSettingsStore_FWD_DEFINED__
typedef interface IVsWritableSettingsStore IVsWritableSettingsStore;

#endif 	/* __IVsWritableSettingsStore_FWD_DEFINED__ */


#ifndef __IVsSettingsManager_FWD_DEFINED__
#define __IVsSettingsManager_FWD_DEFINED__
typedef interface IVsSettingsManager IVsSettingsManager;

#endif 	/* __IVsSettingsManager_FWD_DEFINED__ */


#ifndef __IVsStringMap_FWD_DEFINED__
#define __IVsStringMap_FWD_DEFINED__
typedef interface IVsStringMap IVsStringMap;

#endif 	/* __IVsStringMap_FWD_DEFINED__ */


#ifndef __IVsDataObjectStringMapEvents_FWD_DEFINED__
#define __IVsDataObjectStringMapEvents_FWD_DEFINED__
typedef interface IVsDataObjectStringMapEvents IVsDataObjectStringMapEvents;

#endif 	/* __IVsDataObjectStringMapEvents_FWD_DEFINED__ */


#ifndef __IVsDataObjectStringMapManager_FWD_DEFINED__
#define __IVsDataObjectStringMapManager_FWD_DEFINED__
typedef interface IVsDataObjectStringMapManager IVsDataObjectStringMapManager;

#endif 	/* __IVsDataObjectStringMapManager_FWD_DEFINED__ */


#ifndef __SVsDataObjectStringMapManager_FWD_DEFINED__
#define __SVsDataObjectStringMapManager_FWD_DEFINED__
typedef interface SVsDataObjectStringMapManager SVsDataObjectStringMapManager;

#endif 	/* __SVsDataObjectStringMapManager_FWD_DEFINED__ */


#ifndef __IVsAddToolboxItems_FWD_DEFINED__
#define __IVsAddToolboxItems_FWD_DEFINED__
typedef interface IVsAddToolboxItems IVsAddToolboxItems;

#endif 	/* __IVsAddToolboxItems_FWD_DEFINED__ */


#ifndef __IVsProvideTargetedToolboxItems_FWD_DEFINED__
#define __IVsProvideTargetedToolboxItems_FWD_DEFINED__
typedef interface IVsProvideTargetedToolboxItems IVsProvideTargetedToolboxItems;

#endif 	/* __IVsProvideTargetedToolboxItems_FWD_DEFINED__ */


#ifndef __IVsDesignerInfo_FWD_DEFINED__
#define __IVsDesignerInfo_FWD_DEFINED__
typedef interface IVsDesignerInfo IVsDesignerInfo;

#endif 	/* __IVsDesignerInfo_FWD_DEFINED__ */


#ifndef __IVsToolboxItemProvider_FWD_DEFINED__
#define __IVsToolboxItemProvider_FWD_DEFINED__
typedef interface IVsToolboxItemProvider IVsToolboxItemProvider;

#endif 	/* __IVsToolboxItemProvider_FWD_DEFINED__ */


#ifndef __IVsComponentSelectorDlg4_FWD_DEFINED__
#define __IVsComponentSelectorDlg4_FWD_DEFINED__
typedef interface IVsComponentSelectorDlg4 IVsComponentSelectorDlg4;

#endif 	/* __IVsComponentSelectorDlg4_FWD_DEFINED__ */


#ifndef __SVsComponentModelHost_FWD_DEFINED__
#define __SVsComponentModelHost_FWD_DEFINED__
typedef interface SVsComponentModelHost SVsComponentModelHost;

#endif 	/* __SVsComponentModelHost_FWD_DEFINED__ */


#ifndef __IVsComponentModelHost_FWD_DEFINED__
#define __IVsComponentModelHost_FWD_DEFINED__
typedef interface IVsComponentModelHost IVsComponentModelHost;

#endif 	/* __IVsComponentModelHost_FWD_DEFINED__ */


#ifndef __IVsBuildPropertyStorage2_FWD_DEFINED__
#define __IVsBuildPropertyStorage2_FWD_DEFINED__
typedef interface IVsBuildPropertyStorage2 IVsBuildPropertyStorage2;

#endif 	/* __IVsBuildPropertyStorage2_FWD_DEFINED__ */


#ifndef __IVsLanguageServiceBuildErrorReporter_FWD_DEFINED__
#define __IVsLanguageServiceBuildErrorReporter_FWD_DEFINED__
typedef interface IVsLanguageServiceBuildErrorReporter IVsLanguageServiceBuildErrorReporter;

#endif 	/* __IVsLanguageServiceBuildErrorReporter_FWD_DEFINED__ */


#ifndef __ILocalRegistry5_FWD_DEFINED__
#define __ILocalRegistry5_FWD_DEFINED__
typedef interface ILocalRegistry5 ILocalRegistry5;

#endif 	/* __ILocalRegistry5_FWD_DEFINED__ */


#ifndef __IVsErrorItem2_FWD_DEFINED__
#define __IVsErrorItem2_FWD_DEFINED__
typedef interface IVsErrorItem2 IVsErrorItem2;

#endif 	/* __IVsErrorItem2_FWD_DEFINED__ */


#ifndef __IVsOutputWindow3_FWD_DEFINED__
#define __IVsOutputWindow3_FWD_DEFINED__
typedef interface IVsOutputWindow3 IVsOutputWindow3;

#endif 	/* __IVsOutputWindow3_FWD_DEFINED__ */


#ifndef __IVsSolution4_FWD_DEFINED__
#define __IVsSolution4_FWD_DEFINED__
typedef interface IVsSolution4 IVsSolution4;

#endif 	/* __IVsSolution4_FWD_DEFINED__ */


#ifndef __IVsSolutionLoadManagerSupport_FWD_DEFINED__
#define __IVsSolutionLoadManagerSupport_FWD_DEFINED__
typedef interface IVsSolutionLoadManagerSupport IVsSolutionLoadManagerSupport;

#endif 	/* __IVsSolutionLoadManagerSupport_FWD_DEFINED__ */


#ifndef __IVsSolutionLoadManager_FWD_DEFINED__
#define __IVsSolutionLoadManager_FWD_DEFINED__
typedef interface IVsSolutionLoadManager IVsSolutionLoadManager;

#endif 	/* __IVsSolutionLoadManager_FWD_DEFINED__ */


#ifndef __IVsSolutionLoadEvents_FWD_DEFINED__
#define __IVsSolutionLoadEvents_FWD_DEFINED__
typedef interface IVsSolutionLoadEvents IVsSolutionLoadEvents;

#endif 	/* __IVsSolutionLoadEvents_FWD_DEFINED__ */


#ifndef __IVsThreadedWaitDialogFactory_FWD_DEFINED__
#define __IVsThreadedWaitDialogFactory_FWD_DEFINED__
typedef interface IVsThreadedWaitDialogFactory IVsThreadedWaitDialogFactory;

#endif 	/* __IVsThreadedWaitDialogFactory_FWD_DEFINED__ */


#ifndef __SVsThreadedWaitDialogFactory_FWD_DEFINED__
#define __SVsThreadedWaitDialogFactory_FWD_DEFINED__
typedef interface SVsThreadedWaitDialogFactory SVsThreadedWaitDialogFactory;

#endif 	/* __SVsThreadedWaitDialogFactory_FWD_DEFINED__ */


#ifndef __IVsThreadedWaitDialog2_FWD_DEFINED__
#define __IVsThreadedWaitDialog2_FWD_DEFINED__
typedef interface IVsThreadedWaitDialog2 IVsThreadedWaitDialog2;

#endif 	/* __IVsThreadedWaitDialog2_FWD_DEFINED__ */


#ifndef __IVsToolboxPageChooser_FWD_DEFINED__
#define __IVsToolboxPageChooser_FWD_DEFINED__
typedef interface IVsToolboxPageChooser IVsToolboxPageChooser;

#endif 	/* __IVsToolboxPageChooser_FWD_DEFINED__ */


#ifndef __IVsResourceManager2_FWD_DEFINED__
#define __IVsResourceManager2_FWD_DEFINED__
typedef interface IVsResourceManager2 IVsResourceManager2;

#endif 	/* __IVsResourceManager2_FWD_DEFINED__ */


#ifndef __IVsProject4_FWD_DEFINED__
#define __IVsProject4_FWD_DEFINED__
typedef interface IVsProject4 IVsProject4;

#endif 	/* __IVsProject4_FWD_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory3_FWD_DEFINED__
#define __IVsProjectUpgradeViaFactory3_FWD_DEFINED__
typedef interface IVsProjectUpgradeViaFactory3 IVsProjectUpgradeViaFactory3;

#endif 	/* __IVsProjectUpgradeViaFactory3_FWD_DEFINED__ */


#ifndef __SVsBuildManagerAccessor_FWD_DEFINED__
#define __SVsBuildManagerAccessor_FWD_DEFINED__
typedef interface SVsBuildManagerAccessor SVsBuildManagerAccessor;

#endif 	/* __SVsBuildManagerAccessor_FWD_DEFINED__ */


#ifndef __IVsBuildManagerAccessor_FWD_DEFINED__
#define __IVsBuildManagerAccessor_FWD_DEFINED__
typedef interface IVsBuildManagerAccessor IVsBuildManagerAccessor;

#endif 	/* __IVsBuildManagerAccessor_FWD_DEFINED__ */


#ifndef __IVsSolutionBuildManager4_FWD_DEFINED__
#define __IVsSolutionBuildManager4_FWD_DEFINED__
typedef interface IVsSolutionBuildManager4 IVsSolutionBuildManager4;

#endif 	/* __IVsSolutionBuildManager4_FWD_DEFINED__ */


#ifndef __IVsSolutionLogger_FWD_DEFINED__
#define __IVsSolutionLogger_FWD_DEFINED__
typedef interface IVsSolutionLogger IVsSolutionLogger;

#endif 	/* __IVsSolutionLogger_FWD_DEFINED__ */


#ifndef __IVsToolbox5_FWD_DEFINED__
#define __IVsToolbox5_FWD_DEFINED__
typedef interface IVsToolbox5 IVsToolbox5;

#endif 	/* __IVsToolbox5_FWD_DEFINED__ */


#ifndef __IVsProfferCommands4_FWD_DEFINED__
#define __IVsProfferCommands4_FWD_DEFINED__
typedef interface IVsProfferCommands4 IVsProfferCommands4;

#endif 	/* __IVsProfferCommands4_FWD_DEFINED__ */


#ifndef __SVsProfferCommands_FWD_DEFINED__
#define __SVsProfferCommands_FWD_DEFINED__
typedef interface SVsProfferCommands SVsProfferCommands;

#endif 	/* __SVsProfferCommands_FWD_DEFINED__ */


#ifndef __IVsHelpProvider_FWD_DEFINED__
#define __IVsHelpProvider_FWD_DEFINED__
typedef interface IVsHelpProvider IVsHelpProvider;

#endif 	/* __IVsHelpProvider_FWD_DEFINED__ */


#ifndef __IVsProjectFlavorReferences2_FWD_DEFINED__
#define __IVsProjectFlavorReferences2_FWD_DEFINED__
typedef interface IVsProjectFlavorReferences2 IVsProjectFlavorReferences2;

#endif 	/* __IVsProjectFlavorReferences2_FWD_DEFINED__ */


#ifndef __SVsCommonMessagePumpFactory_FWD_DEFINED__
#define __SVsCommonMessagePumpFactory_FWD_DEFINED__
typedef interface SVsCommonMessagePumpFactory SVsCommonMessagePumpFactory;

#endif 	/* __SVsCommonMessagePumpFactory_FWD_DEFINED__ */


#ifndef __IVsCommonMessagePumpFactory_FWD_DEFINED__
#define __IVsCommonMessagePumpFactory_FWD_DEFINED__
typedef interface IVsCommonMessagePumpFactory IVsCommonMessagePumpFactory;

#endif 	/* __IVsCommonMessagePumpFactory_FWD_DEFINED__ */


#ifndef __IVsCommonMessagePump_FWD_DEFINED__
#define __IVsCommonMessagePump_FWD_DEFINED__
typedef interface IVsCommonMessagePump IVsCommonMessagePump;

#endif 	/* __IVsCommonMessagePump_FWD_DEFINED__ */


#ifndef __IVsCommonMessagePumpClientEvents_FWD_DEFINED__
#define __IVsCommonMessagePumpClientEvents_FWD_DEFINED__
typedef interface IVsCommonMessagePumpClientEvents IVsCommonMessagePumpClientEvents;

#endif 	/* __IVsCommonMessagePumpClientEvents_FWD_DEFINED__ */


#ifndef __IVsHandleInComingCallDynamicInProc_FWD_DEFINED__
#define __IVsHandleInComingCallDynamicInProc_FWD_DEFINED__
typedef interface IVsHandleInComingCallDynamicInProc IVsHandleInComingCallDynamicInProc;

#endif 	/* __IVsHandleInComingCallDynamicInProc_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"
#include "vsshell2.h"
#include "vsshell80.h"
#include "vsshell90.h"
#include "VsPlatformUI.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell100_0000_0000 */
/* [local] */ 

#pragma once

enum __VSPROPID4
    {
        VSPROPID_NoFrameworkDialogState	= -8028,
        VSPROPID_IsInBackgroundIdleLoadProjectBatch	= -8029,
        VSPROPID_IsInSyncDemandLoadProjectBatch	= -8030,
        VSPROPID_IsSolutionFullyLoaded	= -8031,
        VSPROPID_BaseSolutionExplorerCaption	= -8032,
        VSPROPID_SolutionExplorerCaptionSuffix	= -8033,
        VSPROPID_SolutionExplorerCaption	= -8034,
        VSPROPID_AddNewProjectAsSibling	= -8035,
        VSPROPID_ActiveSolutionLoadManager	= -8036,
        VSPROPID_FIRST4	= -8036
    } ;
typedef /* [public] */ DWORD VSPROPID4;


enum _NoFrameworkDialogState
    {
        NOFXDS_AlwaysShowDialog	= 0,
        NOFXDS_HideDialog_KeepUnloaded	= 1,
        NOFXDS_HideDialog_RetargetToFramework40	= 2
    } ;
typedef DWORD NoFrameworkDialogState;

typedef struct _VsResolvedAssemblyPath
    {
    BSTR bstrOrigAssemblySpec;
    BSTR bstrResolvedAssemblyPath;
    } 	VsResolvedAssemblyPath;

typedef VsResolvedAssemblyPath *PVsResolvedAssemblyPath;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0000_v0_0_s_ifspec;

#ifndef __IVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__
#define __IVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__

/* interface IVsDesignTimeAssemblyResolution */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDesignTimeAssemblyResolution;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EDA26258-95DF-44a0-A244-D545E6C1196C")
    IVsDesignTimeAssemblyResolution : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPathInTargetFx( 
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) LPCWSTR prgAssemblySpecs[  ],
            /* [in] */ ULONG cAssembliesToResolve,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetFramework( 
            /* [out] */ __RPC__deref_out_opt BSTR *ppTargetFramework) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDesignTimeAssemblyResolutionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDesignTimeAssemblyResolution * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDesignTimeAssemblyResolution * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDesignTimeAssemblyResolution * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPathInTargetFx )( 
            __RPC__in IVsDesignTimeAssemblyResolution * This,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) LPCWSTR prgAssemblySpecs[  ],
            /* [in] */ ULONG cAssembliesToResolve,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetFramework )( 
            __RPC__in IVsDesignTimeAssemblyResolution * This,
            /* [out] */ __RPC__deref_out_opt BSTR *ppTargetFramework);
        
        END_INTERFACE
    } IVsDesignTimeAssemblyResolutionVtbl;

    interface IVsDesignTimeAssemblyResolution
    {
        CONST_VTBL struct IVsDesignTimeAssemblyResolutionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDesignTimeAssemblyResolution_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDesignTimeAssemblyResolution_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDesignTimeAssemblyResolution_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDesignTimeAssemblyResolution_ResolveAssemblyPathInTargetFx(This,prgAssemblySpecs,cAssembliesToResolve,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths)	\
    ( (This)->lpVtbl -> ResolveAssemblyPathInTargetFx(This,prgAssemblySpecs,cAssembliesToResolve,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths) ) 

#define IVsDesignTimeAssemblyResolution_GetTargetFramework(This,ppTargetFramework)	\
    ( (This)->lpVtbl -> GetTargetFramework(This,ppTargetFramework) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__ */


#ifndef __SVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__
#define __SVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__

/* interface SVsDesignTimeAssemblyResolution */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDesignTimeAssemblyResolution;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1B31CCFD-1B75-4ad8-9581-51B27B150C27")
    SVsDesignTimeAssemblyResolution : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsDesignTimeAssemblyResolutionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsDesignTimeAssemblyResolution * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsDesignTimeAssemblyResolution * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsDesignTimeAssemblyResolution * This);
        
        END_INTERFACE
    } SVsDesignTimeAssemblyResolutionVtbl;

    interface SVsDesignTimeAssemblyResolution
    {
        CONST_VTBL struct SVsDesignTimeAssemblyResolutionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDesignTimeAssemblyResolution_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDesignTimeAssemblyResolution_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDesignTimeAssemblyResolution_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDesignTimeAssemblyResolution_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0002 */
/* [local] */ 

#define SID_SVsDesignTimeAssemblyResolution IID_SVsDesignTimeAssemblyResolution
typedef /* [public] */ BSTR TargetFrameworkMoniker;


enum __VSHPROPID4
    {
        VSHPROPID_TargetFrameworkMoniker	= -2102,
        VSHPROPID_ExternalItem	= -2103,
        VSHPROPID_SupportsAspNetIntegration	= -2104,
        VSHPROPID_DesignTimeDependencies	= -2105,
        VSHPROPID_BuildDependencies	= -2106,
        VSHPROPID_BuildAction	= -2107,
        VSHPROPID_DescriptiveName	= -2108,
        VSHPROPID_AlwaysBuildOnDebugLaunch	= -2109,
        VSHPROPID_FIRST4	= -2109
    } ;
typedef /* [public] */ DWORD VSHPROPID4;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0002_v0_0_s_ifspec;

#ifndef __IVsEditorFactoryChooser_INTERFACE_DEFINED__
#define __IVsEditorFactoryChooser_INTERFACE_DEFINED__

/* interface IVsEditorFactoryChooser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEditorFactoryChooser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00462323-0C58-4b10-BC63-95ED7427744C")
    IVsEditorFactoryChooser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ChooseEditorFactory( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IUnknown *punkDocDataExisting,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [out] */ __RPC__out GUID *pguidEditorTypeActual,
            /* [out] */ __RPC__out GUID *pguidLogicalViewActual) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEditorFactoryChooserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEditorFactoryChooser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEditorFactoryChooser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEditorFactoryChooser * This);
        
        HRESULT ( STDMETHODCALLTYPE *ChooseEditorFactory )( 
            __RPC__in IVsEditorFactoryChooser * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in_opt IUnknown *punkDocDataExisting,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [out] */ __RPC__out GUID *pguidEditorTypeActual,
            /* [out] */ __RPC__out GUID *pguidLogicalViewActual);
        
        END_INTERFACE
    } IVsEditorFactoryChooserVtbl;

    interface IVsEditorFactoryChooser
    {
        CONST_VTBL struct IVsEditorFactoryChooserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEditorFactoryChooser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEditorFactoryChooser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEditorFactoryChooser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEditorFactoryChooser_ChooseEditorFactory(This,pszMkDocument,pHier,itemid,punkDocDataExisting,rguidLogicalView,pguidEditorTypeActual,pguidLogicalViewActual)	\
    ( (This)->lpVtbl -> ChooseEditorFactory(This,pszMkDocument,pHier,itemid,punkDocDataExisting,rguidLogicalView,pguidEditorTypeActual,pguidLogicalViewActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEditorFactoryChooser_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0003 */
/* [local] */ 


enum __VSRESTARTTYPE
    {
        RESTART_Normal	= 0,
        RESTART_Elevated	= 1
    } ;
typedef DWORD VSRESTARTTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0003_v0_0_s_ifspec;

#ifndef __IVsShell4_INTERFACE_DEFINED__
#define __IVsShell4_INTERFACE_DEFINED__

/* interface IVsShell4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsShell4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7AB9748C-7F33-4996-8B00-091B562950A5")
    IVsShell4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Restart( 
            /* [in] */ VSRESTARTTYPE rtRestartMode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsShell4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsShell4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsShell4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsShell4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Restart )( 
            __RPC__in IVsShell4 * This,
            /* [in] */ VSRESTARTTYPE rtRestartMode);
        
        END_INTERFACE
    } IVsShell4Vtbl;

    interface IVsShell4
    {
        CONST_VTBL struct IVsShell4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsShell4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsShell4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsShell4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsShell4_Restart(This,rtRestartMode)	\
    ( (This)->lpVtbl -> Restart(This,rtRestartMode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsShell4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0004 */
/* [local] */ 


enum __WindowFrameTypeFlags
    {
        WINDOWFRAMETYPE_Document	= 0x1,
        WINDOWFRAMETYPE_Tool	= 0x2,
        WINDOWFRAMETYPE_All	= 0xffffff,
        WINDOWFRAMETYPE_Uninitialized	= 0x80000000,
        WINDOWFRAMETYPE_AllStates	= 0xff000000
    } ;
typedef DWORD WINDOWFRAMETYPEFLAGS;




extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0004_v0_0_s_ifspec;

#ifndef __IVsToolbarTrayHost_INTERFACE_DEFINED__
#define __IVsToolbarTrayHost_INTERFACE_DEFINED__

/* interface IVsToolbarTrayHost */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsToolbarTrayHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2B3321EE-693F-4b46-9536-E44DAD8C6E60")
    IVsToolbarTrayHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddToolbar( 
            /* [in] */ __RPC__in REFGUID pGuid,
            /* [in] */ DWORD dwId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetToolbarTray( 
            /* [out] */ __RPC__deref_out_opt IVsUIElement **ppToolbarTray) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolbarTrayHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolbarTrayHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolbarTrayHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolbarTrayHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddToolbar )( 
            __RPC__in IVsToolbarTrayHost * This,
            /* [in] */ __RPC__in REFGUID pGuid,
            /* [in] */ DWORD dwId);
        
        HRESULT ( STDMETHODCALLTYPE *GetToolbarTray )( 
            __RPC__in IVsToolbarTrayHost * This,
            /* [out] */ __RPC__deref_out_opt IVsUIElement **ppToolbarTray);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            __RPC__in IVsToolbarTrayHost * This);
        
        END_INTERFACE
    } IVsToolbarTrayHostVtbl;

    interface IVsToolbarTrayHost
    {
        CONST_VTBL struct IVsToolbarTrayHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolbarTrayHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolbarTrayHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolbarTrayHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolbarTrayHost_AddToolbar(This,pGuid,dwId)	\
    ( (This)->lpVtbl -> AddToolbar(This,pGuid,dwId) ) 

#define IVsToolbarTrayHost_GetToolbarTray(This,ppToolbarTray)	\
    ( (This)->lpVtbl -> GetToolbarTray(This,ppToolbarTray) ) 

#define IVsToolbarTrayHost_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolbarTrayHost_INTERFACE_DEFINED__ */


#ifndef __IVsUIShell4_INTERFACE_DEFINED__
#define __IVsUIShell4_INTERFACE_DEFINED__

/* interface IVsUIShell4 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsUIShell4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c59cda92-d99d-42da-b221-8e36b8dc478e")
    IVsUIShell4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetWindowEnum( 
            WINDOWFRAMETYPEFLAGS type,
            /* [out] */ __RPC__deref_out_opt IEnumWindowFrames **ppEnum) = 0;
        
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE SetupToolbar2( 
            /* [in] */ HWND hwnd,
            /* [in] */ IVsToolWindowToolbar *ptwt,
            /* [in] */ IOleCommandTarget *pCmdTarget,
            /* [out] */ IVsToolWindowToolbarHost **pptwth) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetupToolbar3( 
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [out] */ __RPC__deref_out_opt IVsToolWindowToolbarHost **pptwth) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateToolbarTray( 
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTarget,
            /* [out] */ __RPC__deref_out_opt IVsToolbarTrayHost **ppToolbarTrayHost) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIShell4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIShell4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIShell4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIShell4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetWindowEnum )( 
            __RPC__in IVsUIShell4 * This,
            WINDOWFRAMETYPEFLAGS type,
            /* [out] */ __RPC__deref_out_opt IEnumWindowFrames **ppEnum);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *SetupToolbar2 )( 
            IVsUIShell4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ IVsToolWindowToolbar *ptwt,
            /* [in] */ IOleCommandTarget *pCmdTarget,
            /* [out] */ IVsToolWindowToolbarHost **pptwth);
        
        HRESULT ( STDMETHODCALLTYPE *SetupToolbar3 )( 
            __RPC__in IVsUIShell4 * This,
            /* [in] */ __RPC__in_opt IVsWindowFrame *pFrame,
            /* [out] */ __RPC__deref_out_opt IVsToolWindowToolbarHost **pptwth);
        
        HRESULT ( STDMETHODCALLTYPE *CreateToolbarTray )( 
            __RPC__in IVsUIShell4 * This,
            /* [in] */ __RPC__in_opt IOleCommandTarget *pCmdTarget,
            /* [out] */ __RPC__deref_out_opt IVsToolbarTrayHost **ppToolbarTrayHost);
        
        END_INTERFACE
    } IVsUIShell4Vtbl;

    interface IVsUIShell4
    {
        CONST_VTBL struct IVsUIShell4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIShell4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIShell4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIShell4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIShell4_GetWindowEnum(This,type,ppEnum)	\
    ( (This)->lpVtbl -> GetWindowEnum(This,type,ppEnum) ) 

#define IVsUIShell4_SetupToolbar2(This,hwnd,ptwt,pCmdTarget,pptwth)	\
    ( (This)->lpVtbl -> SetupToolbar2(This,hwnd,ptwt,pCmdTarget,pptwth) ) 

#define IVsUIShell4_SetupToolbar3(This,pFrame,pptwth)	\
    ( (This)->lpVtbl -> SetupToolbar3(This,pFrame,pptwth) ) 

#define IVsUIShell4_CreateToolbarTray(This,pCmdTarget,ppToolbarTrayHost)	\
    ( (This)->lpVtbl -> CreateToolbarTray(This,pCmdTarget,ppToolbarTrayHost) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIShell4_INTERFACE_DEFINED__ */


#ifndef __IVsToolWindowToolbarHost2_INTERFACE_DEFINED__
#define __IVsToolWindowToolbarHost2_INTERFACE_DEFINED__

/* interface IVsToolWindowToolbarHost2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsToolWindowToolbarHost2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2EFC69A8-5E06-436D-88D5-F099353356DA")
    IVsToolWindowToolbarHost2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddToolbar2( 
            /* [in] */ VSTWT_LOCATION dwLoc,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId,
            /* [in] */ __RPC__in_opt IDropTarget *pDropTarget) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolWindowToolbarHost2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolWindowToolbarHost2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolWindowToolbarHost2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolWindowToolbarHost2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddToolbar2 )( 
            __RPC__in IVsToolWindowToolbarHost2 * This,
            /* [in] */ VSTWT_LOCATION dwLoc,
            /* [in] */ __RPC__in const GUID *pguid,
            /* [in] */ DWORD dwId,
            /* [in] */ __RPC__in_opt IDropTarget *pDropTarget);
        
        END_INTERFACE
    } IVsToolWindowToolbarHost2Vtbl;

    interface IVsToolWindowToolbarHost2
    {
        CONST_VTBL struct IVsToolWindowToolbarHost2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolWindowToolbarHost2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolWindowToolbarHost2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolWindowToolbarHost2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolWindowToolbarHost2_AddToolbar2(This,dwLoc,pguid,dwId,pDropTarget)	\
    ( (This)->lpVtbl -> AddToolbar2(This,dwLoc,pguid,dwId,pDropTarget) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolWindowToolbarHost2_INTERFACE_DEFINED__ */


#ifndef __IVsUIElementPane_INTERFACE_DEFINED__
#define __IVsUIElementPane_INTERFACE_DEFINED__

/* interface IVsUIElementPane */
/* [object][version][uuid][local] */ 


EXTERN_C const IID IID_IVsUIElementPane;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D5083078-66C7-4047-B101-62A5F7997EC5")
    IVsUIElementPane : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetUIElementSite( 
            /* [in] */ IServiceProvider *pSP) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateUIElementPane( 
            /* [out] */ IUnknown **punkUIElement) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultUIElementSize( 
            /* [out] */ SIZE *psize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseUIElementPane( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadUIElementState( 
            /* [in] */ IStream *pstream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveUIElementState( 
            /* [in] */ IStream *pstream) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateUIElementAccelerator( 
            MSG *lpmsg) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIElementPaneVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIElementPane * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIElementPane * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIElementPane * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetUIElementSite )( 
            IVsUIElementPane * This,
            /* [in] */ IServiceProvider *pSP);
        
        HRESULT ( STDMETHODCALLTYPE *CreateUIElementPane )( 
            IVsUIElementPane * This,
            /* [out] */ IUnknown **punkUIElement);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultUIElementSize )( 
            IVsUIElementPane * This,
            /* [out] */ SIZE *psize);
        
        HRESULT ( STDMETHODCALLTYPE *CloseUIElementPane )( 
            IVsUIElementPane * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadUIElementState )( 
            IVsUIElementPane * This,
            /* [in] */ IStream *pstream);
        
        HRESULT ( STDMETHODCALLTYPE *SaveUIElementState )( 
            IVsUIElementPane * This,
            /* [in] */ IStream *pstream);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateUIElementAccelerator )( 
            IVsUIElementPane * This,
            MSG *lpmsg);
        
        END_INTERFACE
    } IVsUIElementPaneVtbl;

    interface IVsUIElementPane
    {
        CONST_VTBL struct IVsUIElementPaneVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIElementPane_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIElementPane_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIElementPane_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIElementPane_SetUIElementSite(This,pSP)	\
    ( (This)->lpVtbl -> SetUIElementSite(This,pSP) ) 

#define IVsUIElementPane_CreateUIElementPane(This,punkUIElement)	\
    ( (This)->lpVtbl -> CreateUIElementPane(This,punkUIElement) ) 

#define IVsUIElementPane_GetDefaultUIElementSize(This,psize)	\
    ( (This)->lpVtbl -> GetDefaultUIElementSize(This,psize) ) 

#define IVsUIElementPane_CloseUIElementPane(This)	\
    ( (This)->lpVtbl -> CloseUIElementPane(This) ) 

#define IVsUIElementPane_LoadUIElementState(This,pstream)	\
    ( (This)->lpVtbl -> LoadUIElementState(This,pstream) ) 

#define IVsUIElementPane_SaveUIElementState(This,pstream)	\
    ( (This)->lpVtbl -> SaveUIElementState(This,pstream) ) 

#define IVsUIElementPane_TranslateUIElementAccelerator(This,lpmsg)	\
    ( (This)->lpVtbl -> TranslateUIElementAccelerator(This,lpmsg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIElementPane_INTERFACE_DEFINED__ */


#ifndef __IVsWindowFrame3_INTERFACE_DEFINED__
#define __IVsWindowFrame3_INTERFACE_DEFINED__

/* interface IVsWindowFrame3 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsWindowFrame3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b2c3d311-2599-4ce1-9967-f224d99d374b")
    IVsWindowFrame3 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE CreateThumbnail( 
            int width,
            int height,
            /* [out] */ HBITMAP *thumbnail) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowFrame3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowFrame3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowFrame3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowFrame3 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *CreateThumbnail )( 
            IVsWindowFrame3 * This,
            int width,
            int height,
            /* [out] */ HBITMAP *thumbnail);
        
        END_INTERFACE
    } IVsWindowFrame3Vtbl;

    interface IVsWindowFrame3
    {
        CONST_VTBL struct IVsWindowFrame3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowFrame3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowFrame3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowFrame3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowFrame3_CreateThumbnail(This,width,height,thumbnail)	\
    ( (This)->lpVtbl -> CreateThumbnail(This,width,height,thumbnail) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowFrame3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0009 */
/* [local] */ 


enum __VSCREATETOOLWIN2
    {
        CTW_fDocumentLikeTool	= 0x800000
    } ;
typedef DWORD VSCREATETOOLWIN2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0009_v0_0_s_ifspec;

#ifndef __IVsWindowFrameSwitcher_INTERFACE_DEFINED__
#define __IVsWindowFrameSwitcher_INTERFACE_DEFINED__

/* interface IVsWindowFrameSwitcher */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsWindowFrameSwitcher;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C40CF83B-A231-46b3-AA07-8CC30D5E1A04")
    IVsWindowFrameSwitcher : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddTool( 
            /* [in] */ __RPC__in REFGUID guidTool,
            /* [in] */ __RPC__in LPWSTR switchContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveTool( 
            /* [in] */ __RPC__in REFGUID guidTool,
            /* [in] */ __RPC__in LPWSTR switchContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetActiveFrame( 
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **pFrame) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeSwitcher( 
            /* [in] */ UINT selelem) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWindowFrameSwitcherVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWindowFrameSwitcher * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWindowFrameSwitcher * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWindowFrameSwitcher * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddTool )( 
            __RPC__in IVsWindowFrameSwitcher * This,
            /* [in] */ __RPC__in REFGUID guidTool,
            /* [in] */ __RPC__in LPWSTR switchContext);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveTool )( 
            __RPC__in IVsWindowFrameSwitcher * This,
            /* [in] */ __RPC__in REFGUID guidTool,
            /* [in] */ __RPC__in LPWSTR switchContext);
        
        HRESULT ( STDMETHODCALLTYPE *GetActiveFrame )( 
            __RPC__in IVsWindowFrameSwitcher * This,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **pFrame);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeSwitcher )( 
            __RPC__in IVsWindowFrameSwitcher * This,
            /* [in] */ UINT selelem);
        
        END_INTERFACE
    } IVsWindowFrameSwitcherVtbl;

    interface IVsWindowFrameSwitcher
    {
        CONST_VTBL struct IVsWindowFrameSwitcherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWindowFrameSwitcher_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWindowFrameSwitcher_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWindowFrameSwitcher_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWindowFrameSwitcher_AddTool(This,guidTool,switchContext)	\
    ( (This)->lpVtbl -> AddTool(This,guidTool,switchContext) ) 

#define IVsWindowFrameSwitcher_RemoveTool(This,guidTool,switchContext)	\
    ( (This)->lpVtbl -> RemoveTool(This,guidTool,switchContext) ) 

#define IVsWindowFrameSwitcher_GetActiveFrame(This,pFrame)	\
    ( (This)->lpVtbl -> GetActiveFrame(This,pFrame) ) 

#define IVsWindowFrameSwitcher_InitializeSwitcher(This,selelem)	\
    ( (This)->lpVtbl -> InitializeSwitcher(This,selelem) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWindowFrameSwitcher_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0010 */
/* [local] */ 


enum __VSFPROPID4
    {
        VSFPROPID_Icon	= -5012,
        VSFPROPID_TabImage	= -5013,
        VSFPROPID_Thumbnail	= -5014,
        VSFPROPID_NavigationInterface	= -5015,
        VSFPROPID_NextCloneID	= -5016,
        VSFPROPID4_FIRST	= -5016
    } ;
typedef LONG VSFPROPID4;


enum __VSTABBEDMODE
    {
        VSTABBEDMODE_NotTabbed	= 0,
        VSTABBEDMODE_SelectedTab	= 1,
        VSTABBEDMODE_Tabbed	= 2
    } ;
typedef LONG VSTABBEDMODE;


enum __VISUALEFFECTS
    {
        VISUALEFFECTS_None	= 0,
        VISUALEFFECTS_Animations	= 0x1,
        VISUALEFFECTS_Gradients	= 0x2,
        VISUALEFFECTS_AllRichEffects	= 0xffff,
        VISUALEFFECTS_UseHardwareAcceleration	= 0x10000
    } ;
typedef LONG VISUALEFFECTS;


enum __VSSPROPID4
    {
        VSSPROPID_ShellInitialized	= -9053,
        VSSPROPID_ZeroConfigProjectDir	= -9054,
        VSSPROPID_LocalAppDataDir	= -9055,
        VSSPROPID_LayoutIsRightToLeft	= -9056,
        VSSPROPID_OpenFileStartDir	= -9057,
        VSSPROPID_IsModal	= -9058,
        VSSPROPID_MainWindowDataSource	= -9059,
        VSSPROPID_VisualEffectsOption	= -9060,
        VSSPROPID_VisualEffectsAllowed	= -9061,
        VSSPROPID_IsExecutingCommand	= -9062,
        VSSPROPID_ReuseSavedActiveDocWindow	= -9063,
        VSSPROPID_ActivityLogPath	= -9064,
        VSSPROPID_ConfigurationTimestampUtc	= -9065,
        VSSPROPID_FIRST4	= -9065
    } ;
typedef LONG VSSPROPID4;

typedef 
enum __tagVSSYSCOLOREX3
    {
        VSCOLOR_ACTIVEBORDER	= -192,
        VSCOLOR_ACTIVECAPTION	= -193,
        VSCOLOR_APPWORKSPACE	= -194,
        VSCOLOR_BACKGROUND	= -195,
        VSCOLOR_BUTTONFACE	= -196,
        VSCOLOR_BUTTONHIGHLIGHT	= -197,
        VSCOLOR_BUTTONSHADOW	= -198,
        VSCOLOR_BUTTONTEXT	= -199,
        VSCOLOR_CAPTIONTEXT	= -200,
        VSCOLOR_GRAYTEXT	= -201,
        VSCOLOR_HIGHLIGHT	= -202,
        VSCOLOR_HIGHLIGHTTEXT	= -203,
        VSCOLOR_INACTIVEBORDER	= -204,
        VSCOLOR_INACTIVECAPTION	= -205,
        VSCOLOR_INACTIVECAPTIONTEXT	= -206,
        VSCOLOR_INFOBACKGROUND	= -207,
        VSCOLOR_INFOTEXT	= -208,
        VSCOLOR_MENU	= -209,
        VSCOLOR_MENUTEXT	= -210,
        VSCOLOR_SCROLLBAR	= -211,
        VSCOLOR_THREEDDARKSHADOW	= -212,
        VSCOLOR_THREEDFACE	= -213,
        VSCOLOR_THREEDHIGHLIGHT	= -214,
        VSCOLOR_THREEDLIGHTSHADOW	= -215,
        VSCOLOR_THREEDSHADOW	= -216,
        VSCOLOR_WINDOW	= -217,
        VSCOLOR_WINDOWFRAME	= -218,
        VSCOLOR_WINDOWTEXT	= -219,
        VSCOLOR_AUTOHIDE_TAB_TEXT	= -220,
        VSCOLOR_AUTOHIDE_TAB_BACKGROUND_BEGIN	= -221,
        VSCOLOR_AUTOHIDE_TAB_BACKGROUND_END	= -222,
        VSCOLOR_AUTOHIDE_TAB_BORDER	= -223,
        VSCOLOR_AUTOHIDE_TAB_MOUSEOVER_TEXT	= -224,
        VSCOLOR_AUTOHIDE_TAB_MOUSEOVER_BACKGROUND_BEGIN	= -225,
        VSCOLOR_AUTOHIDE_TAB_MOUSEOVER_BACKGROUND_END	= -226,
        VSCOLOR_AUTOHIDE_TAB_MOUSEOVER_BORDER	= -227,
        VSCOLOR_AUTOHIDE_RESIZEGRIP	= -228,
        VSCOLOR_CLASSDESIGNER_CLASSCOMPARTMENT	= -229,
        VSCOLOR_CLASSDESIGNER_CLASSHEADERBACKGROUND	= -230,
        VSCOLOR_CLASSDESIGNER_COMMENTBORDER	= -231,
        VSCOLOR_CLASSDESIGNER_COMMENTSHAPEBACKGROUND	= -232,
        VSCOLOR_CLASSDESIGNER_COMMENTTEXT	= -233,
        VSCOLOR_CLASSDESIGNER_COMPARTMENTSEPARATOR	= -234,
        VSCOLOR_CLASSDESIGNER_CONNECTIONROUTEBORDER	= -235,
        VSCOLOR_CLASSDESIGNER_DEFAULTCONNECTION	= -236,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPETITLEBACKGROUND	= -237,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPEBACKGROUND	= -238,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPEBORDER	= -239,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPESUBTITLE	= -240,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPETEXT	= -241,
        VSCOLOR_CLASSDESIGNER_DEFAULTSHAPETITLE	= -242,
        VSCOLOR_CLASSDESIGNER_DELEGATECOMPARTMENT	= -243,
        VSCOLOR_CLASSDESIGNER_DELEGATEHEADER	= -244,
        VSCOLOR_CLASSDESIGNER_DIAGRAMBACKGROUND	= -245,
        VSCOLOR_CLASSDESIGNER_EMPHASISBORDER	= -246,
        VSCOLOR_CLASSDESIGNER_ENUMHEADER	= -247,
        VSCOLOR_CLASSDESIGNER_FIELDASSOCIATION	= -248,
        VSCOLOR_CLASSDESIGNER_GRADIENTEND	= -249,
        VSCOLOR_CLASSDESIGNER_INHERITANCE	= -250,
        VSCOLOR_CLASSDESIGNER_INTERFACEHEADER	= -251,
        VSCOLOR_CLASSDESIGNER_INTERFACECOMPARTMENT	= -252,
        VSCOLOR_CLASSDESIGNER_LASSO	= -253,
        VSCOLOR_CLASSDESIGNER_LOLLIPOP	= -254,
        VSCOLOR_CLASSDESIGNER_PROPERTYASSOCIATION	= -255,
        VSCOLOR_CLASSDESIGNER_REFERENCEDASSEMBLYBORDER	= -256,
        VSCOLOR_CLASSDESIGNER_RESIZINGSHAPEBORDER	= -257,
        VSCOLOR_CLASSDESIGNER_SHAPEBORDER	= -258,
        VSCOLOR_CLASSDESIGNER_SHAPESHADOW	= -259,
        VSCOLOR_CLASSDESIGNER_TEMPCONNECTION	= -260,
        VSCOLOR_CLASSDESIGNER_TYPEDEF	= -261,
        VSCOLOR_CLASSDESIGNER_TYPEDEFHEADER	= -262,
        VSCOLOR_CLASSDESIGNER_UNRESOLVEDTEXT	= -263,
        VSCOLOR_CLASSDESIGNER_VBMODULECOMPARTMENT	= -264,
        VSCOLOR_CLASSDESIGNER_VBMODULEHEADER	= -265,
        VSCOLOR_COMBOBOX_BACKGROUND	= -266,
        VSCOLOR_COMBOBOX_BORDER	= -267,
        VSCOLOR_COMBOBOX_DISABLED_BACKGROUND	= -268,
        VSCOLOR_COMBOBOX_DISABLED_BORDER	= -269,
        VSCOLOR_COMBOBOX_DISABLED_GLYPH	= -270,
        VSCOLOR_COMBOBOX_GLYPH	= -271,
        VSCOLOR_COMBOBOX_MOUSEDOWN_BACKGROUND	= -272,
        VSCOLOR_COMBOBOX_MOUSEDOWN_BORDER	= -273,
        VSCOLOR_COMBOBOX_MOUSEOVER_BACKGROUND_BEGIN	= -274,
        VSCOLOR_COMBOBOX_MOUSEOVER_BACKGROUND_MIDDLE1	= -275,
        VSCOLOR_COMBOBOX_MOUSEOVER_BACKGROUND_MIDDLE2	= -276,
        VSCOLOR_COMBOBOX_MOUSEOVER_BACKGROUND_END	= -277,
        VSCOLOR_COMBOBOX_MOUSEOVER_BORDER	= -278,
        VSCOLOR_COMBOBOX_MOUSEOVER_GLYPH	= -279,
        VSCOLOR_COMBOBOX_POPUP_BACKGROUND_BEGIN	= -280,
        VSCOLOR_COMBOBOX_POPUP_BACKGROUND_END	= -281,
        VSCOLOR_COMBOBOX_POPUP_BORDER	= -282,
        VSCOLOR_COMMANDBAR_CHECKBOX	= -283,
        VSCOLOR_COMMANDBAR_MENU_BACKGROUND_GRADIENTBEGIN	= -284,
        VSCOLOR_COMMANDBAR_MENU_BACKGROUND_GRADIENTEND	= -285,
        VSCOLOR_COMMANDBAR_MENU_BORDER	= -286,
        VSCOLOR_COMMANDBAR_MENU_ICONBACKGROUND	= -287,
        VSCOLOR_COMMANDBAR_MENU_MOUSEOVER_SUBMENU_GLYPH	= -288,
        VSCOLOR_COMMANDBAR_MENU_SEPARATOR	= -289,
        VSCOLOR_COMMANDBAR_MENU_SUBMENU_GLYPH	= -290,
        VSCOLOR_COMMANDBAR_MOUSEDOWN_BACKGROUND_BEGIN	= -291,
        VSCOLOR_COMMANDBAR_MOUSEDOWN_BACKGROUND_MIDDLE	= -292,
        VSCOLOR_COMMANDBAR_MOUSEDOWN_BACKGROUND_END	= -293,
        VSCOLOR_COMMANDBAR_MOUSEDOWN_BORDER	= -294,
        VSCOLOR_COMMANDBAR_MOUSEOVER_BACKGROUND_BEGIN	= -295,
        VSCOLOR_COMMANDBAR_MOUSEOVER_BACKGROUND_MIDDLE1	= -296,
        VSCOLOR_COMMANDBAR_MOUSEOVER_BACKGROUND_MIDDLE2	= -297,
        VSCOLOR_COMMANDBAR_MOUSEOVER_BACKGROUND_END	= -298,
        VSCOLOR_COMMANDBAR_OPTIONS_BACKGROUND	= -299,
        VSCOLOR_COMMANDBAR_OPTIONS_GLYPH	= -300,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEDOWN_BACKGROUND_BEGIN	= -301,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEDOWN_BACKGROUND_MIDDLE	= -302,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEDOWN_BACKGROUND_END	= -303,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEOVER_BACKGROUND_BEGIN	= -304,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEOVER_BACKGROUND_MIDDLE1	= -305,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEOVER_BACKGROUND_MIDDLE2	= -306,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEOVER_BACKGROUND_END	= -307,
        VSCOLOR_COMMANDBAR_OPTIONS_MOUSEOVER_GLYPH	= -308,
        VSCOLOR_COMMANDBAR_SELECTED_BORDER	= -309,
        VSCOLOR_COMMANDBAR_TOOLBAR_BORDER	= -310,
        VSCOLOR_COMMANDBAR_TOOLBAR_SEPARATOR	= -311,
        VSCOLOR_COMMANDSHELF_BACKGROUND_GRADIENTBEGIN	= -312,
        VSCOLOR_COMMANDSHELF_BACKGROUND_GRADIENTMIDDLE	= -313,
        VSCOLOR_COMMANDSHELF_BACKGROUND_GRADIENTEND	= -314,
        VSCOLOR_COMMANDSHELF_HIGHLIGHT_GRADIENTBEGIN	= -315,
        VSCOLOR_COMMANDSHELF_HIGHLIGHT_GRADIENTMIDDLE	= -316,
        VSCOLOR_COMMANDSHELF_HIGHLIGHT_GRADIENTEND	= -317,
        VSCOLOR_DIAGREPORT_BACKGROUND	= -318,
        VSCOLOR_DIAGREPORT_SECONDARYPAGE_HEADER	= -319,
        VSCOLOR_DIAGREPORT_SECONDARYPAGE_SUBTITLE	= -320,
        VSCOLOR_DIAGREPORT_SECONDARYPAGE_TITLE	= -321,
        VSCOLOR_DIAGREPORT_SUMMARYPAGE_HEADER	= -322,
        VSCOLOR_DIAGREPORT_SUMMARYPAGE_SUBTITLE	= -323,
        VSCOLOR_DIAGREPORT_SUMMARYPAGE_TITLE	= -324,
        VSCOLOR_DIAGREPORT_TEXT	= -325,
        VSCOLOR_DOCKTARGET_BACKGROUND	= -326,
        VSCOLOR_DOCKTARGET_BORDER	= -327,
        VSCOLOR_DOCKTARGET_BUTTON_BACKGROUND_BEGIN	= -328,
        VSCOLOR_DOCKTARGET_BUTTON_BACKGROUND_END	= -329,
        VSCOLOR_DOCKTARGET_BUTTON_BORDER	= -330,
        VSCOLOR_DOCKTARGET_GLYPH_BACKGROUND_BEGIN	= -331,
        VSCOLOR_DOCKTARGET_GLYPH_BACKGROUND_END	= -332,
        VSCOLOR_DOCKTARGET_GLYPH_ARROW	= -333,
        VSCOLOR_DOCKTARGET_GLYPH_BORDER	= -334,
        VSCOLOR_DROPDOWN_BACKGROUND	= -335,
        VSCOLOR_DROPDOWN_BORDER	= -336,
        VSCOLOR_DROPDOWN_DISABLED_BACKGROUND	= -337,
        VSCOLOR_DROPDOWN_DISABLED_BORDER	= -338,
        VSCOLOR_DROPDOWN_DISABLED_GLYPH	= -339,
        VSCOLOR_DROPDOWN_GLYPH	= -340,
        VSCOLOR_DROPDOWN_MOUSEDOWN_BACKGROUND	= -341,
        VSCOLOR_DROPDOWN_MOUSEDOWN_BORDER	= -342,
        VSCOLOR_DROPDOWN_MOUSEOVER_BACKGROUND_BEGIN	= -343,
        VSCOLOR_DROPDOWN_MOUSEOVER_BACKGROUND_MIDDLE1	= -344,
        VSCOLOR_DROPDOWN_MOUSEOVER_BACKGROUND_MIDDLE2	= -345,
        VSCOLOR_DROPDOWN_MOUSEOVER_BACKGROUND_END	= -346,
        VSCOLOR_DROPDOWN_MOUSEOVER_BORDER	= -347,
        VSCOLOR_DROPDOWN_MOUSEOVER_GLYPH	= -348,
        VSCOLOR_DROPDOWN_POPUP_BACKGROUND_BEGIN	= -349,
        VSCOLOR_DROPDOWN_POPUP_BACKGROUND_END	= -350,
        VSCOLOR_DROPDOWN_POPUP_BORDER	= -351,
        VSCOLOR_DROPSHADOW_BACKGROUND	= -352,
        VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTMIDDLE1	= -353,
        VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTMIDDLE2	= -354,
        VSCOLOR_ENVIRONMENT_BACKGROUND_TEXTURE1	= -355,
        VSCOLOR_ENVIRONMENT_BACKGROUND_TEXTURE2	= -356,
        VSCOLOR_EXTENSIONMANAGER_STAR_HIGHLIGHT1	= -357,
        VSCOLOR_EXTENSIONMANAGER_STAR_HIGHLIGHT2	= -358,
        VSCOLOR_EXTENSIONMANAGER_STAR_INACTIVE1	= -359,
        VSCOLOR_EXTENSIONMANAGER_STAR_INACTIVE2	= -360,
        VSCOLOR_FILETAB_HOT_BORDER	= -361,
        VSCOLOR_FILETAB_HOT_TEXT	= -362,
        VSCOLOR_FILETAB_HOT_GLYPH	= -363,
        VSCOLOR_FILETAB_INACTIVE_GRADIENTTOP	= -364,
        VSCOLOR_FILETAB_INACTIVE_GRADIENTBOTTOM	= -365,
        VSCOLOR_FILETAB_INACTIVE_DOCUMENTBORDER_BACKGROUND	= -366,
        VSCOLOR_FILETAB_INACTIVE_DOCUMENTBORDER_EDGE	= -367,
        VSCOLOR_FILETAB_INACTIVE_TEXT	= -368,
        VSCOLOR_FILETAB_LASTACTIVE_GRADIENTTOP	= -369,
        VSCOLOR_FILETAB_LASTACTIVE_GRADIENTMIDDLE1	= -370,
        VSCOLOR_FILETAB_LASTACTIVE_GRADIENTMIDDLE2	= -371,
        VSCOLOR_FILETAB_LASTACTIVE_GRADIENTBOTTOM	= -372,
        VSCOLOR_FILETAB_LASTACTIVE_DOCUMENTBORDER_BACKGROUND	= -373,
        VSCOLOR_FILETAB_LASTACTIVE_DOCUMENTBORDER_EDGE	= -374,
        VSCOLOR_FILETAB_LASTACTIVE_TEXT	= -375,
        VSCOLOR_FILETAB_LASTACTIVE_GLYPH	= -376,
        VSCOLOR_FILETAB_SELECTED_GRADIENTMIDDLE1	= -377,
        VSCOLOR_FILETAB_SELECTED_GRADIENTMIDDLE2	= -378,
        VSCOLOR_NEWPROJECT_BACKGROUND	= -379,
        VSCOLOR_NEWPROJECT_PROVIDER_HOVER_FOREGROUND	= -380,
        VSCOLOR_NEWPROJECT_PROVIDER_HOVER_BEGIN	= -381,
        VSCOLOR_NEWPROJECT_PROVIDER_HOVER_MIDDLE1	= -382,
        VSCOLOR_NEWPROJECT_PROVIDER_HOVER_MIDDLE2	= -383,
        VSCOLOR_NEWPROJECT_PROVIDER_HOVER_END	= -384,
        VSCOLOR_NEWPROJECT_PROVIDER_INACTIVE_FOREGROUND	= -385,
        VSCOLOR_NEWPROJECT_PROVIDER_INACTIVE_BEGIN	= -386,
        VSCOLOR_NEWPROJECT_PROVIDER_INACTIVE_END	= -387,
        VSCOLOR_NEWPROJECT_ITEM_SELECTED_BORDER	= -388,
        VSCOLOR_NEWPROJECT_ITEM_SELECTED	= -389,
        VSCOLOR_NEWPROJECT_ITEM_INACTIVE_BEGIN	= -390,
        VSCOLOR_NEWPROJECT_ITEM_INACTIVE_END	= -391,
        VSCOLOR_NEWPROJECT_ITEM_INACTIVE_BORDER	= -392,
        VSCOLOR_PAGE_CONTENTEXPANDER_CHEVRON	= -393,
        VSCOLOR_PAGE_CONTENTEXPANDER_SEPARATOR	= -394,
        VSCOLOR_PAGE_SIDEBAREXPANDER_BODY	= -395,
        VSCOLOR_PAGE_SIDEBAREXPANDER_CHEVRON	= -396,
        VSCOLOR_PAGE_SIDEBAREXPANDER_HEADER	= -397,
        VSCOLOR_PAGE_SIDEBAREXPANDER_HEADER_HOVER	= -398,
        VSCOLOR_PAGE_SIDEBAREXPANDER_HEADER_PRESSED	= -399,
        VSCOLOR_PAGE_SIDEBAREXPANDER_SEPARATOR	= -400,
        VSCOLOR_PAGE_SIDEBAREXPANDER_TEXT	= -401,
        VSCOLOR_SCROLLBAR_ARROW_BACKGROUND	= -402,
        VSCOLOR_SCROLLBAR_ARROW_DISABLED_BACKGROUND	= -403,
        VSCOLOR_SCROLLBAR_ARROW_MOUSEOVER_BACKGROUND	= -404,
        VSCOLOR_SCROLLBAR_ARROW_PRESSED_BACKGROUND	= -405,
        VSCOLOR_SCROLLBAR_BACKGROUND	= -406,
        VSCOLOR_SCROLLBAR_DISABLED_BACKGROUND	= -407,
        VSCOLOR_SCROLLBAR_THUMB_BACKGROUND	= -408,
        VSCOLOR_SCROLLBAR_THUMB_BORDER	= -409,
        VSCOLOR_SCROLLBAR_THUMB_GLYPH	= -410,
        VSCOLOR_SCROLLBAR_THUMB_MOUSEOVER_BACKGROUND	= -411,
        VSCOLOR_SCROLLBAR_THUMB_PRESSED_BACKGROUND	= -412,
        VSCOLOR_SEARCHBOX_BACKGROUND	= -413,
        VSCOLOR_SEARCHBOX_BORDER	= -414,
        VSCOLOR_SEARCHBOX_MOUSEOVER_BACKGROUND_BEGIN	= -415,
        VSCOLOR_SEARCHBOX_MOUSEOVER_BACKGROUND_MIDDLE1	= -416,
        VSCOLOR_SEARCHBOX_MOUSEOVER_BACKGROUND_MIDDLE2	= -417,
        VSCOLOR_SEARCHBOX_MOUSEOVER_BACKGROUND_END	= -418,
        VSCOLOR_SEARCHBOX_MOUSEOVER_BORDER	= -419,
        VSCOLOR_SEARCHBOX_PRESSED_BACKGROUND	= -420,
        VSCOLOR_SEARCHBOX_PRESSED_BORDER	= -421,
        VSCOLOR_STARTPAGE_BACKGROUND_GRADIENTBEGIN	= -422,
        VSCOLOR_STARTPAGE_BACKGROUND_GRADIENTEND	= -423,
        VSCOLOR_STARTPAGE_BUTTON_BORDER	= -424,
        VSCOLOR_STARTPAGE_BUTTON_MOUSEOVER_BACKGROUND_BEGIN	= -425,
        VSCOLOR_STARTPAGE_BUTTON_MOUSEOVER_BACKGROUND_END	= -426,
        VSCOLOR_STARTPAGE_BUTTON_MOUSEOVER_BACKGROUND_MIDDLE1	= -427,
        VSCOLOR_STARTPAGE_BUTTON_MOUSEOVER_BACKGROUND_MIDDLE2	= -428,
        VSCOLOR_STARTPAGE_BUTTON_PINNED	= -429,
        VSCOLOR_STARTPAGE_BUTTON_PIN_DOWN	= -430,
        VSCOLOR_STARTPAGE_BUTTON_PIN_HOVER	= -431,
        VSCOLOR_STARTPAGE_BUTTON_UNPINNED	= -432,
        VSCOLOR_STARTPAGE_BUTTONTEXT	= -433,
        VSCOLOR_STARTPAGE_BUTTONTEXT_HOVER	= -434,
        VSCOLOR_STARTPAGE_SELECTEDITEM_BACKGROUND	= -435,
        VSCOLOR_STARTPAGE_SELECTEDITEM_STROKE	= -436,
        VSCOLOR_STARTPAGE_SEPARATOR	= -437,
        VSCOLOR_STARTPAGE_TAB_BACKGROUND_BEGIN	= -438,
        VSCOLOR_STARTPAGE_TAB_BACKGROUND_END	= -439,
        VSCOLOR_STARTPAGE_TAB_MOUSEOVER_BACKGROUND_BEGIN	= -440,
        VSCOLOR_STARTPAGE_TAB_MOUSEOVER_BACKGROUND_END	= -441,
        VSCOLOR_STARTPAGE_TEXT_BODY	= -442,
        VSCOLOR_STARTPAGE_TEXT_BODY_SELECTED	= -443,
        VSCOLOR_STARTPAGE_TEXT_BODY_UNSELECTED	= -444,
        VSCOLOR_STARTPAGE_TEXT_CONTROL_LINK_SELECTED	= -445,
        VSCOLOR_STARTPAGE_TEXT_CONTROL_LINK_SELECTED_HOVER	= -446,
        VSCOLOR_STARTPAGE_TEXT_DATE	= -447,
        VSCOLOR_STARTPAGE_TEXT_HEADING	= -448,
        VSCOLOR_STARTPAGE_TEXT_HEADING_MOUSEOVER	= -449,
        VSCOLOR_STARTPAGE_TEXT_HEADING_SELECTED	= -450,
        VSCOLOR_STARTPAGE_TEXT_SUBHEADING	= -451,
        VSCOLOR_STARTPAGE_TEXT_SUBHEADING_MOUSEOVER	= -452,
        VSCOLOR_STARTPAGE_TEXT_SUBHEADING_SELECTED	= -453,
        VSCOLOR_STARTPAGE_UNSELECTEDITEM_BACKGROUND_BEGIN	= -454,
        VSCOLOR_STARTPAGE_UNSELECTEDITEM_BACKGROUND_END	= -455,
        VSCOLOR_STARTPAGE_UNSELECTEDITEM_STROKE	= -456,
        VSCOLOR_STATUSBAR_TEXT	= -457,
        VSCOLOR_TITLEBAR_ACTIVE_GRADIENTMIDDLE1	= -458,
        VSCOLOR_TITLEBAR_ACTIVE_GRADIENTMIDDLE2	= -459,
        VSCOLOR_TOOLBOX_SELECTED_HEADING_BEGIN	= -460,
        VSCOLOR_TOOLBOX_SELECTED_HEADING_MIDDLE1	= -461,
        VSCOLOR_TOOLBOX_SELECTED_HEADING_MIDDLE2	= -462,
        VSCOLOR_TOOLBOX_SELECTED_HEADING_END	= -463,
        VSCOLOR_TOOLWINDOW_BUTTON_INACTIVE_GLYPH	= -464,
        VSCOLOR_TOOLWINDOW_BUTTON_INACTIVE	= -465,
        VSCOLOR_TOOLWINDOW_BUTTON_INACTIVE_BORDER	= -466,
        VSCOLOR_TOOLWINDOW_BUTTON_HOVER_INACTIVE_GLYPH	= -467,
        VSCOLOR_TOOLWINDOW_BUTTON_DOWN_INACTIVE_GLYPH	= -468,
        VSCOLOR_TOOLWINDOW_BUTTON_ACTIVE_GLYPH	= -469,
        VSCOLOR_TOOLWINDOW_BUTTON_HOVER_ACTIVE_GLYPH	= -470,
        VSCOLOR_TOOLWINDOW_BUTTON_DOWN_ACTIVE_GLYPH	= -471,
        VSCOLOR_TOOLWINDOW_CONTENTTAB_GRADIENTBEGIN	= -472,
        VSCOLOR_TOOLWINDOW_CONTENTTAB_GRADIENTEND	= -473,
        VSCOLOR_TOOLWINDOW_FLOATING_FRAME	= -474,
        VSCOLOR_TOOLWINDOW_TAB_MOUSEOVER_BACKGROUND_BEGIN	= -475,
        VSCOLOR_TOOLWINDOW_TAB_MOUSEOVER_BACKGROUND_END	= -476,
        VSCOLOR_TOOLWINDOW_TAB_MOUSEOVER_BORDER	= -477,
        VSCOLOR_TOOLWINDOW_TAB_MOUSEOVER_TEXT	= -478,
        VSCOLOR_VIZSURFACE_BROWN_DARK	= -479,
        VSCOLOR_VIZSURFACE_BROWN_LIGHT	= -480,
        VSCOLOR_VIZSURFACE_BROWN_MEDIUM	= -481,
        VSCOLOR_VIZSURFACE_DARKGOLD_DARK	= -482,
        VSCOLOR_VIZSURFACE_DARKGOLD_LIGHT	= -483,
        VSCOLOR_VIZSURFACE_DARKGOLD_MEDIUM	= -484,
        VSCOLOR_VIZSURFACE_GOLD_DARK	= -485,
        VSCOLOR_VIZSURFACE_GOLD_LIGHT	= -486,
        VSCOLOR_VIZSURFACE_GOLD_MEDIUM	= -487,
        VSCOLOR_VIZSURFACE_GREEN_DARK	= -488,
        VSCOLOR_VIZSURFACE_GREEN_LIGHT	= -489,
        VSCOLOR_VIZSURFACE_GREEN_MEDIUM	= -490,
        VSCOLOR_VIZSURFACE_PLUM_DARK	= -491,
        VSCOLOR_VIZSURFACE_PLUM_LIGHT	= -492,
        VSCOLOR_VIZSURFACE_PLUM_MEDIUM	= -493,
        VSCOLOR_VIZSURFACE_RED_DARK	= -494,
        VSCOLOR_VIZSURFACE_RED_LIGHT	= -495,
        VSCOLOR_VIZSURFACE_RED_MEDIUM	= -496,
        VSCOLOR_VIZSURFACE_SOFTBLUE_DARK	= -497,
        VSCOLOR_VIZSURFACE_SOFTBLUE_LIGHT	= -498,
        VSCOLOR_VIZSURFACE_SOFTBLUE_MEDIUM	= -499,
        VSCOLOR_VIZSURFACE_STEELBLUE_DARK	= -500,
        VSCOLOR_VIZSURFACE_STEELBLUE_LIGHT	= -501,
        VSCOLOR_VIZSURFACE_STEELBLUE_MEDIUM	= -502,
        VSCOLOR_VIZSURFACE_STRONGBLUE_DARK	= -503,
        VSCOLOR_VIZSURFACE_STRONGBLUE_LIGHT	= -504,
        VSCOLOR_VIZSURFACE_STRONGBLUE_MEDIUM	= -505,
        VSCOLOR_LASTEX3	= -505
    } 	__VSSYSCOLOREX3;

typedef 
enum __tagGRADIENTTYPE3
    {
        VSGRADIENT_TOOLBOX_SELECTED_HEADING_TOP	= 11,
        VSGRADIENT_TOOLBOX_SELECTED_HEADING_BOTTOM	= 12
    } 	__GRADIENTTYPE3;

typedef DWORD GRADIENTTYPE3;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0010_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0010_v0_0_s_ifspec;

#ifndef __IVsConfigureToolboxItem_INTERFACE_DEFINED__
#define __IVsConfigureToolboxItem_INTERFACE_DEFINED__

/* interface IVsConfigureToolboxItem */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsConfigureToolboxItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6c3e27f9-775a-456b-8d37-2d7057abb8f1")
    IVsConfigureToolboxItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ConfigureToolboxItem( 
            /* [in] */ __RPC__in_opt IUnknown *item) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsConfigureToolboxItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsConfigureToolboxItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsConfigureToolboxItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsConfigureToolboxItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *ConfigureToolboxItem )( 
            __RPC__in IVsConfigureToolboxItem * This,
            /* [in] */ __RPC__in_opt IUnknown *item);
        
        END_INTERFACE
    } IVsConfigureToolboxItemVtbl;

    interface IVsConfigureToolboxItem
    {
        CONST_VTBL struct IVsConfigureToolboxItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsConfigureToolboxItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsConfigureToolboxItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsConfigureToolboxItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsConfigureToolboxItem_ConfigureToolboxItem(This,item)	\
    ( (This)->lpVtbl -> ConfigureToolboxItem(This,item) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsConfigureToolboxItem_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0011 */
/* [local] */ 


enum __DSI_FLAGS
    {
        DSI_USESHOWWINDOW	= 0x1,
        DSI_USESIZE	= 0x2,
        DSI_USEPOSITION	= 0x4,
        DSI_USECOUNTCHARS	= 0x8,
        DSI_USEFILLATTRIBUTE	= 0x10,
        DSI_RUNFULLSCREEN	= 0x20,
        DSI_FORCEONFEEDBACK	= 0x40,
        DSI_FORCEOFFFEEDBACK	= 0x80,
        DSI_USESTDHANDLES	= 0x100,
        DSI_USECREATIONFLAGS	= 0x40000000,
        DSI_INHERITHANDLES	= 0x80000000
    } ;
typedef DWORD DSI_FLAGS;

typedef struct _VsDebugStartupInfo
    {
    LPCWSTR lpReserved;
    LPCWSTR lpDesktop;
    LPCWSTR lpTitle;
    DWORD dwCreationFlags;
    DWORD dwX;
    DWORD dwY;
    DWORD dwXSize;
    DWORD dwYSize;
    DWORD dwXCountChars;
    DWORD dwYCountChars;
    DWORD dwFillAttribute;
    DSI_FLAGS flags;
    WORD wShowWindow;
    WORD cbReserved2;
    BYTE *lpReserved2;
    DWORD_PTR hStdInput;
    DWORD_PTR hStdOutput;
    DWORD_PTR hStdError;
    } 	VsDebugStartupInfo;

typedef DEBUG_LAUNCH_OPERATION2 DEBUG_LAUNCH_OPERATION3;


enum __VSDBGLAUNCHFLAGS5
    {
        DBGLAUNCH_PrepForDebug	= 0x2000,
        DBGLAUNCH_TerminateOnStop	= 0x4000,
        DBGLAUNCH_BreakOneProcess	= 0x8000
    } ;
typedef DWORD VSDBGLAUNCHFLAGS5;

typedef struct _VsDebugTargetInfo3
    {
    DEBUG_LAUNCH_OPERATION3 dlo;
    VSDBGLAUNCHFLAGS5 LaunchFlags;
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
    } 	VsDebugTargetInfo3;

typedef struct _VsDebugTargetProcessInfo
    {
    DWORD dwProcessId;
    FILETIME creationTime;
    } 	VsDebugTargetProcessInfo;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0011_v0_0_s_ifspec;

#ifndef __IVsDebugger3_INTERFACE_DEFINED__
#define __IVsDebugger3_INTERFACE_DEFINED__

/* interface IVsDebugger3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugger3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F6A3B3E7-5A0E-420B-98D0-AE6963CAD9F3")
    IVsDebugger3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LaunchDebugTargets3( 
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo3 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsProcessRecycleRequired( 
            /* [in] */ __RPC__in VsDebugTargetProcessInfo *pProcessInfo) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugger3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugger3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugger3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugger3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *LaunchDebugTargets3 )( 
            __RPC__in IVsDebugger3 * This,
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo3 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults);
        
        HRESULT ( STDMETHODCALLTYPE *IsProcessRecycleRequired )( 
            __RPC__in IVsDebugger3 * This,
            /* [in] */ __RPC__in VsDebugTargetProcessInfo *pProcessInfo);
        
        END_INTERFACE
    } IVsDebugger3Vtbl;

    interface IVsDebugger3
    {
        CONST_VTBL struct IVsDebugger3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugger3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugger3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugger3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugger3_LaunchDebugTargets3(This,DebugTargetCount,pDebugTargets,pLaunchResults)	\
    ( (This)->lpVtbl -> LaunchDebugTargets3(This,DebugTargetCount,pDebugTargets,pLaunchResults) ) 

#define IVsDebugger3_IsProcessRecycleRequired(This,pProcessInfo)	\
    ( (This)->lpVtbl -> IsProcessRecycleRequired(This,pProcessInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugger3_INTERFACE_DEFINED__ */


#ifndef __IVsDebugLaunchHook_INTERFACE_DEFINED__
#define __IVsDebugLaunchHook_INTERFACE_DEFINED__

/* interface IVsDebugLaunchHook */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDebugLaunchHook;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9040E456-FF48-41FF-AEDD-D5EB36A0DDFB")
    IVsDebugLaunchHook : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetNextHook( 
            /* [in] */ __RPC__in_opt IVsDebugLaunchHook *pNextHook) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsProcessRecycleRequired( 
            /* [in] */ __RPC__in VsDebugTargetProcessInfo *pProcessInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnLaunchDebugTargets( 
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo3 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugLaunchHookVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugLaunchHook * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugLaunchHook * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugLaunchHook * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetNextHook )( 
            __RPC__in IVsDebugLaunchHook * This,
            /* [in] */ __RPC__in_opt IVsDebugLaunchHook *pNextHook);
        
        HRESULT ( STDMETHODCALLTYPE *IsProcessRecycleRequired )( 
            __RPC__in IVsDebugLaunchHook * This,
            /* [in] */ __RPC__in VsDebugTargetProcessInfo *pProcessInfo);
        
        HRESULT ( STDMETHODCALLTYPE *OnLaunchDebugTargets )( 
            __RPC__in IVsDebugLaunchHook * This,
            /* [in] */ ULONG DebugTargetCount,
            /* [size_is][in] */ __RPC__in_ecount_full(DebugTargetCount) VsDebugTargetInfo3 *pDebugTargets,
            /* [size_is][out] */ __RPC__out_ecount_full(DebugTargetCount) VsDebugTargetProcessInfo *pLaunchResults);
        
        END_INTERFACE
    } IVsDebugLaunchHookVtbl;

    interface IVsDebugLaunchHook
    {
        CONST_VTBL struct IVsDebugLaunchHookVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugLaunchHook_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugLaunchHook_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugLaunchHook_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugLaunchHook_SetNextHook(This,pNextHook)	\
    ( (This)->lpVtbl -> SetNextHook(This,pNextHook) ) 

#define IVsDebugLaunchHook_IsProcessRecycleRequired(This,pProcessInfo)	\
    ( (This)->lpVtbl -> IsProcessRecycleRequired(This,pProcessInfo) ) 

#define IVsDebugLaunchHook_OnLaunchDebugTargets(This,DebugTargetCount,pDebugTargets,pLaunchResults)	\
    ( (This)->lpVtbl -> OnLaunchDebugTargets(This,DebugTargetCount,pDebugTargets,pLaunchResults) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugLaunchHook_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0013 */
/* [local] */ 


enum __VsSettingsScope
    {
        SettingsScope_Configuration	= 0x1,
        SettingsScope_UserSettings	= 0x2
    } ;
typedef DWORD VSSETTINGSSCOPE;


enum __VsEnclosingScopes
    {
        EnclosingScopes_None	= 0,
        EnclosingScopes_Configuration	= SettingsScope_Configuration,
        EnclosingScopes_UserSettings	= SettingsScope_UserSettings
    } ;
typedef DWORD VSENCLOSINGSCOPES;


enum __VsSettingsType
    {
        SettingsType_Invalid	= 0,
        SettingsType_Int	= 1,
        SettingsType_Int64	= 2,
        SettingsType_String	= 3,
        SettingsType_Binary	= 4
    } ;
typedef DWORD VSSETTINGSTYPE;


enum __VsApplicationDataFolder
    {
        ApplicationDataFolder_LocalSettings	= 0,
        ApplicationDataFolder_RoamingSettings	= 1,
        ApplicationDataFolder_Configuration	= 2,
        ApplicationDataFolder_Documents	= 3,
        ApplicationDataFolder_UserExtensions	= 4,
        ApplicationDataFolder_ApplicationExtensions	= 5
    } ;
typedef DWORD VSAPPLICATIONDATAFOLDER;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0013_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0013_v0_0_s_ifspec;

#ifndef __SVsSettingsManager_INTERFACE_DEFINED__
#define __SVsSettingsManager_INTERFACE_DEFINED__

/* interface SVsSettingsManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSettingsManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C4DEEBEE-F4C1-4a09-B47C-EF17E83D269D")
    SVsSettingsManager : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsSettingsManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsSettingsManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsSettingsManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsSettingsManager * This);
        
        END_INTERFACE
    } SVsSettingsManagerVtbl;

    interface SVsSettingsManager
    {
        CONST_VTBL struct SVsSettingsManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSettingsManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSettingsManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSettingsManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSettingsManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0014 */
/* [local] */ 

#define SID_SVsSettingsManager IID_SVsSettingsManager


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0014_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0014_v0_0_s_ifspec;

#ifndef __IVsSettingsStore_INTERFACE_DEFINED__
#define __IVsSettingsStore_INTERFACE_DEFINED__

/* interface IVsSettingsStore */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSettingsStore;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6B43326C-AB7C-4263-A7EF-354B9DCBF3D8")
    IVsSettingsStore : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBool( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInt( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out int *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnsignedInt( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out DWORD *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInt64( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out INT64 *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnsignedInt64( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out UINT64 *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetString( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__deref_out_opt BSTR *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBinary( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ ULONG byteLength,
            /* [size_is][optional][out] */ __RPC__out_ecount_full(byteLength) BYTE *pBytes,
            /* [optional][out] */ __RPC__out ULONG *actualByteLength) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBoolOrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ BOOL defaultValue,
            /* [out] */ __RPC__out BOOL *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIntOrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ int defaultValue,
            /* [out] */ __RPC__out int *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnsignedIntOrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ DWORD defaultValue,
            /* [out] */ __RPC__out DWORD *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInt64OrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ INT64 defaultValue,
            /* [out] */ __RPC__out INT64 *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnsignedInt64OrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ UINT64 defaultValue,
            /* [out] */ __RPC__out UINT64 *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStringOrDefault( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ __RPC__in LPCOLESTR defaultValue,
            /* [out] */ __RPC__deref_out_opt BSTR *value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPropertyType( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out VSSETTINGSTYPE *type) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PropertyExists( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *pfExists) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CollectionExists( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out BOOL *pfExists) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSubCollectionCount( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *subCollectionCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPropertyCount( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *propertyCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastWriteTime( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out SYSTEMTIME *lastWriteTime) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSubCollectionName( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *subCollectionName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPropertyName( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *propertyName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSettingsStoreVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSettingsStore * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSettingsStore * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBool )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out int *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out DWORD *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt64 )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out INT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt64 )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out UINT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetString )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__deref_out_opt BSTR *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetBinary )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ ULONG byteLength,
            /* [size_is][optional][out] */ __RPC__out_ecount_full(byteLength) BYTE *pBytes,
            /* [optional][out] */ __RPC__out ULONG *actualByteLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetBoolOrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ BOOL defaultValue,
            /* [out] */ __RPC__out BOOL *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetIntOrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ int defaultValue,
            /* [out] */ __RPC__out int *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedIntOrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ DWORD defaultValue,
            /* [out] */ __RPC__out DWORD *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt64OrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ INT64 defaultValue,
            /* [out] */ __RPC__out INT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt64OrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ UINT64 defaultValue,
            /* [out] */ __RPC__out UINT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringOrDefault )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ __RPC__in LPCOLESTR defaultValue,
            /* [out] */ __RPC__deref_out_opt BSTR *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyType )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out VSSETTINGSTYPE *type);
        
        HRESULT ( STDMETHODCALLTYPE *PropertyExists )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *pfExists);
        
        HRESULT ( STDMETHODCALLTYPE *CollectionExists )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out BOOL *pfExists);
        
        HRESULT ( STDMETHODCALLTYPE *GetSubCollectionCount )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *subCollectionCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyCount )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *propertyCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastWriteTime )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out SYSTEMTIME *lastWriteTime);
        
        HRESULT ( STDMETHODCALLTYPE *GetSubCollectionName )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *subCollectionName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyName )( 
            __RPC__in IVsSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *propertyName);
        
        END_INTERFACE
    } IVsSettingsStoreVtbl;

    interface IVsSettingsStore
    {
        CONST_VTBL struct IVsSettingsStoreVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSettingsStore_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSettingsStore_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSettingsStore_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSettingsStore_GetBool(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetBool(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetInt(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetUnsignedInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetInt64(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetUnsignedInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt64(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetString(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetString(This,collectionPath,propertyName,value) ) 

#define IVsSettingsStore_GetBinary(This,collectionPath,propertyName,byteLength,pBytes,actualByteLength)	\
    ( (This)->lpVtbl -> GetBinary(This,collectionPath,propertyName,byteLength,pBytes,actualByteLength) ) 

#define IVsSettingsStore_GetBoolOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetBoolOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetIntOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetIntOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetUnsignedIntOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetUnsignedIntOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetInt64OrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetInt64OrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetUnsignedInt64OrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt64OrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetStringOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetStringOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsSettingsStore_GetPropertyType(This,collectionPath,propertyName,type)	\
    ( (This)->lpVtbl -> GetPropertyType(This,collectionPath,propertyName,type) ) 

#define IVsSettingsStore_PropertyExists(This,collectionPath,propertyName,pfExists)	\
    ( (This)->lpVtbl -> PropertyExists(This,collectionPath,propertyName,pfExists) ) 

#define IVsSettingsStore_CollectionExists(This,collectionPath,pfExists)	\
    ( (This)->lpVtbl -> CollectionExists(This,collectionPath,pfExists) ) 

#define IVsSettingsStore_GetSubCollectionCount(This,collectionPath,subCollectionCount)	\
    ( (This)->lpVtbl -> GetSubCollectionCount(This,collectionPath,subCollectionCount) ) 

#define IVsSettingsStore_GetPropertyCount(This,collectionPath,propertyCount)	\
    ( (This)->lpVtbl -> GetPropertyCount(This,collectionPath,propertyCount) ) 

#define IVsSettingsStore_GetLastWriteTime(This,collectionPath,lastWriteTime)	\
    ( (This)->lpVtbl -> GetLastWriteTime(This,collectionPath,lastWriteTime) ) 

#define IVsSettingsStore_GetSubCollectionName(This,collectionPath,index,subCollectionName)	\
    ( (This)->lpVtbl -> GetSubCollectionName(This,collectionPath,index,subCollectionName) ) 

#define IVsSettingsStore_GetPropertyName(This,collectionPath,index,propertyName)	\
    ( (This)->lpVtbl -> GetPropertyName(This,collectionPath,index,propertyName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSettingsStore_INTERFACE_DEFINED__ */


#ifndef __IVsWritableSettingsStore_INTERFACE_DEFINED__
#define __IVsWritableSettingsStore_INTERFACE_DEFINED__

/* interface IVsWritableSettingsStore */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsWritableSettingsStore;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("16FA7461-9E7C-4f28-B28F-AABBF73C0193")
    IVsWritableSettingsStore : public IVsSettingsStore
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetBool( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ BOOL value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetInt( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ int value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUnsignedInt( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ DWORD value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetInt64( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ INT64 value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUnsignedInt64( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ UINT64 value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetString( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ __RPC__in LPCOLESTR value) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetBinary( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ ULONG byteLength,
            /* [size_is][in] */ __RPC__in_ecount_full(byteLength) BYTE *pBytes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteProperty( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateCollection( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteCollection( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsWritableSettingsStoreVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsWritableSettingsStore * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsWritableSettingsStore * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBool )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out int *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out DWORD *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt64 )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out INT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt64 )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out UINT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetString )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__deref_out_opt BSTR *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetBinary )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ ULONG byteLength,
            /* [size_is][optional][out] */ __RPC__out_ecount_full(byteLength) BYTE *pBytes,
            /* [optional][out] */ __RPC__out ULONG *actualByteLength);
        
        HRESULT ( STDMETHODCALLTYPE *GetBoolOrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ BOOL defaultValue,
            /* [out] */ __RPC__out BOOL *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetIntOrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ int defaultValue,
            /* [out] */ __RPC__out int *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedIntOrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ DWORD defaultValue,
            /* [out] */ __RPC__out DWORD *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetInt64OrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ INT64 defaultValue,
            /* [out] */ __RPC__out INT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnsignedInt64OrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ UINT64 defaultValue,
            /* [out] */ __RPC__out UINT64 *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringOrDefault )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ __RPC__in LPCOLESTR defaultValue,
            /* [out] */ __RPC__deref_out_opt BSTR *value);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyType )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out VSSETTINGSTYPE *type);
        
        HRESULT ( STDMETHODCALLTYPE *PropertyExists )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out BOOL *pfExists);
        
        HRESULT ( STDMETHODCALLTYPE *CollectionExists )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out BOOL *pfExists);
        
        HRESULT ( STDMETHODCALLTYPE *GetSubCollectionCount )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *subCollectionCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyCount )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out DWORD *propertyCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastWriteTime )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out SYSTEMTIME *lastWriteTime);
        
        HRESULT ( STDMETHODCALLTYPE *GetSubCollectionName )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *subCollectionName);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyName )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ DWORD index,
            /* [out] */ __RPC__deref_out_opt BSTR *propertyName);
        
        HRESULT ( STDMETHODCALLTYPE *SetBool )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ BOOL value);
        
        HRESULT ( STDMETHODCALLTYPE *SetInt )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ int value);
        
        HRESULT ( STDMETHODCALLTYPE *SetUnsignedInt )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ DWORD value);
        
        HRESULT ( STDMETHODCALLTYPE *SetInt64 )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ INT64 value);
        
        HRESULT ( STDMETHODCALLTYPE *SetUnsignedInt64 )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ UINT64 value);
        
        HRESULT ( STDMETHODCALLTYPE *SetString )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ __RPC__in LPCOLESTR value);
        
        HRESULT ( STDMETHODCALLTYPE *SetBinary )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [in] */ ULONG byteLength,
            /* [size_is][in] */ __RPC__in_ecount_full(byteLength) BYTE *pBytes);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteProperty )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName);
        
        HRESULT ( STDMETHODCALLTYPE *CreateCollection )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteCollection )( 
            __RPC__in IVsWritableSettingsStore * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath);
        
        END_INTERFACE
    } IVsWritableSettingsStoreVtbl;

    interface IVsWritableSettingsStore
    {
        CONST_VTBL struct IVsWritableSettingsStoreVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWritableSettingsStore_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWritableSettingsStore_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWritableSettingsStore_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWritableSettingsStore_GetBool(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetBool(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetInt(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetUnsignedInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetInt64(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetUnsignedInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt64(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetString(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> GetString(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_GetBinary(This,collectionPath,propertyName,byteLength,pBytes,actualByteLength)	\
    ( (This)->lpVtbl -> GetBinary(This,collectionPath,propertyName,byteLength,pBytes,actualByteLength) ) 

#define IVsWritableSettingsStore_GetBoolOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetBoolOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetIntOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetIntOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetUnsignedIntOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetUnsignedIntOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetInt64OrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetInt64OrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetUnsignedInt64OrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetUnsignedInt64OrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetStringOrDefault(This,collectionPath,propertyName,defaultValue,value)	\
    ( (This)->lpVtbl -> GetStringOrDefault(This,collectionPath,propertyName,defaultValue,value) ) 

#define IVsWritableSettingsStore_GetPropertyType(This,collectionPath,propertyName,type)	\
    ( (This)->lpVtbl -> GetPropertyType(This,collectionPath,propertyName,type) ) 

#define IVsWritableSettingsStore_PropertyExists(This,collectionPath,propertyName,pfExists)	\
    ( (This)->lpVtbl -> PropertyExists(This,collectionPath,propertyName,pfExists) ) 

#define IVsWritableSettingsStore_CollectionExists(This,collectionPath,pfExists)	\
    ( (This)->lpVtbl -> CollectionExists(This,collectionPath,pfExists) ) 

#define IVsWritableSettingsStore_GetSubCollectionCount(This,collectionPath,subCollectionCount)	\
    ( (This)->lpVtbl -> GetSubCollectionCount(This,collectionPath,subCollectionCount) ) 

#define IVsWritableSettingsStore_GetPropertyCount(This,collectionPath,propertyCount)	\
    ( (This)->lpVtbl -> GetPropertyCount(This,collectionPath,propertyCount) ) 

#define IVsWritableSettingsStore_GetLastWriteTime(This,collectionPath,lastWriteTime)	\
    ( (This)->lpVtbl -> GetLastWriteTime(This,collectionPath,lastWriteTime) ) 

#define IVsWritableSettingsStore_GetSubCollectionName(This,collectionPath,index,subCollectionName)	\
    ( (This)->lpVtbl -> GetSubCollectionName(This,collectionPath,index,subCollectionName) ) 

#define IVsWritableSettingsStore_GetPropertyName(This,collectionPath,index,propertyName)	\
    ( (This)->lpVtbl -> GetPropertyName(This,collectionPath,index,propertyName) ) 


#define IVsWritableSettingsStore_SetBool(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetBool(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetInt(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetUnsignedInt(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetUnsignedInt(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetInt64(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetUnsignedInt64(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetUnsignedInt64(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetString(This,collectionPath,propertyName,value)	\
    ( (This)->lpVtbl -> SetString(This,collectionPath,propertyName,value) ) 

#define IVsWritableSettingsStore_SetBinary(This,collectionPath,propertyName,byteLength,pBytes)	\
    ( (This)->lpVtbl -> SetBinary(This,collectionPath,propertyName,byteLength,pBytes) ) 

#define IVsWritableSettingsStore_DeleteProperty(This,collectionPath,propertyName)	\
    ( (This)->lpVtbl -> DeleteProperty(This,collectionPath,propertyName) ) 

#define IVsWritableSettingsStore_CreateCollection(This,collectionPath)	\
    ( (This)->lpVtbl -> CreateCollection(This,collectionPath) ) 

#define IVsWritableSettingsStore_DeleteCollection(This,collectionPath)	\
    ( (This)->lpVtbl -> DeleteCollection(This,collectionPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWritableSettingsStore_INTERFACE_DEFINED__ */


#ifndef __IVsSettingsManager_INTERFACE_DEFINED__
#define __IVsSettingsManager_INTERFACE_DEFINED__

/* interface IVsSettingsManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSettingsManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("94D59A1D-A3A8-46ab-B1DE-B7F034018137")
    IVsSettingsManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCollectionScopes( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out VSENCLOSINGSCOPES *scopes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPropertyScopes( 
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out VSENCLOSINGSCOPES *scopes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReadOnlySettingsStore( 
            /* [in] */ VSSETTINGSSCOPE scope,
            /* [out] */ __RPC__deref_out_opt IVsSettingsStore **store) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetWritableSettingsStore( 
            /* [in] */ VSSETTINGSSCOPE scope,
            /* [out] */ __RPC__deref_out_opt IVsWritableSettingsStore **writableStore) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetApplicationDataFolder( 
            /* [in] */ VSAPPLICATIONDATAFOLDER folder,
            /* [out] */ __RPC__deref_out_opt BSTR *folderPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCommonExtensionsSearchPaths( 
            /* [in] */ ULONG paths,
            /* [size_is][out] */ __RPC__out_ecount_full(paths) BSTR *commonExtensionsPaths,
            /* [out] */ __RPC__out ULONG *actualPaths) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSettingsManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSettingsManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSettingsManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCollectionScopes )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [out] */ __RPC__out VSENCLOSINGSCOPES *scopes);
        
        HRESULT ( STDMETHODCALLTYPE *GetPropertyScopes )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ __RPC__in LPCOLESTR collectionPath,
            /* [in] */ __RPC__in LPCOLESTR propertyName,
            /* [out] */ __RPC__out VSENCLOSINGSCOPES *scopes);
        
        HRESULT ( STDMETHODCALLTYPE *GetReadOnlySettingsStore )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ VSSETTINGSSCOPE scope,
            /* [out] */ __RPC__deref_out_opt IVsSettingsStore **store);
        
        HRESULT ( STDMETHODCALLTYPE *GetWritableSettingsStore )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ VSSETTINGSSCOPE scope,
            /* [out] */ __RPC__deref_out_opt IVsWritableSettingsStore **writableStore);
        
        HRESULT ( STDMETHODCALLTYPE *GetApplicationDataFolder )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ VSAPPLICATIONDATAFOLDER folder,
            /* [out] */ __RPC__deref_out_opt BSTR *folderPath);
        
        HRESULT ( STDMETHODCALLTYPE *GetCommonExtensionsSearchPaths )( 
            __RPC__in IVsSettingsManager * This,
            /* [in] */ ULONG paths,
            /* [size_is][out] */ __RPC__out_ecount_full(paths) BSTR *commonExtensionsPaths,
            /* [out] */ __RPC__out ULONG *actualPaths);
        
        END_INTERFACE
    } IVsSettingsManagerVtbl;

    interface IVsSettingsManager
    {
        CONST_VTBL struct IVsSettingsManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSettingsManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSettingsManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSettingsManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSettingsManager_GetCollectionScopes(This,collectionPath,scopes)	\
    ( (This)->lpVtbl -> GetCollectionScopes(This,collectionPath,scopes) ) 

#define IVsSettingsManager_GetPropertyScopes(This,collectionPath,propertyName,scopes)	\
    ( (This)->lpVtbl -> GetPropertyScopes(This,collectionPath,propertyName,scopes) ) 

#define IVsSettingsManager_GetReadOnlySettingsStore(This,scope,store)	\
    ( (This)->lpVtbl -> GetReadOnlySettingsStore(This,scope,store) ) 

#define IVsSettingsManager_GetWritableSettingsStore(This,scope,writableStore)	\
    ( (This)->lpVtbl -> GetWritableSettingsStore(This,scope,writableStore) ) 

#define IVsSettingsManager_GetApplicationDataFolder(This,folder,folderPath)	\
    ( (This)->lpVtbl -> GetApplicationDataFolder(This,folder,folderPath) ) 

#define IVsSettingsManager_GetCommonExtensionsSearchPaths(This,paths,commonExtensionsPaths,actualPaths)	\
    ( (This)->lpVtbl -> GetCommonExtensionsSearchPaths(This,paths,commonExtensionsPaths,actualPaths) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSettingsManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0017 */
/* [local] */ 

#define E_VS_KEYMISSING MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2000)


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0017_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0017_v0_0_s_ifspec;

#ifndef __IVsStringMap_INTERFACE_DEFINED__
#define __IVsStringMap_INTERFACE_DEFINED__

/* interface IVsStringMap */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsStringMap;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4743CC21-8943-414d-84C2-E5DE2438D02F")
    IVsStringMap : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in LPCWSTR szKey,
            /* [in] */ __RPC__in LPCWSTR szValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in LPCWSTR szKey,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveValue( 
            /* [in] */ __RPC__in LPCWSTR szKey) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumKeys( 
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clear( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsStringMapVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsStringMap * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsStringMap * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsStringMap * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsStringMap * This,
            /* [in] */ __RPC__in LPCWSTR szKey,
            /* [in] */ __RPC__in LPCWSTR szValue);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsStringMap * This,
            /* [in] */ __RPC__in LPCWSTR szKey,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrValue);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveValue )( 
            __RPC__in IVsStringMap * This,
            /* [in] */ __RPC__in LPCWSTR szKey);
        
        HRESULT ( STDMETHODCALLTYPE *EnumKeys )( 
            __RPC__in IVsStringMap * This,
            /* [out] */ __RPC__deref_out_opt IEnumString **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Clear )( 
            __RPC__in IVsStringMap * This);
        
        END_INTERFACE
    } IVsStringMapVtbl;

    interface IVsStringMap
    {
        CONST_VTBL struct IVsStringMapVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsStringMap_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsStringMap_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsStringMap_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsStringMap_SetValue(This,szKey,szValue)	\
    ( (This)->lpVtbl -> SetValue(This,szKey,szValue) ) 

#define IVsStringMap_GetValue(This,szKey,pbstrValue)	\
    ( (This)->lpVtbl -> GetValue(This,szKey,pbstrValue) ) 

#define IVsStringMap_RemoveValue(This,szKey)	\
    ( (This)->lpVtbl -> RemoveValue(This,szKey) ) 

#define IVsStringMap_EnumKeys(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumKeys(This,ppEnum) ) 

#define IVsStringMap_Clear(This)	\
    ( (This)->lpVtbl -> Clear(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsStringMap_INTERFACE_DEFINED__ */


#ifndef __IVsDataObjectStringMapEvents_INTERFACE_DEFINED__
#define __IVsDataObjectStringMapEvents_INTERFACE_DEFINED__

/* interface IVsDataObjectStringMapEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDataObjectStringMapEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DFEC8E49-B682-43E0-841A-C25C5E2EA1E9")
    IVsDataObjectStringMapEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnStringMapChanged( 
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDataObjectStringMapEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDataObjectStringMapEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDataObjectStringMapEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDataObjectStringMapEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnStringMapChanged )( 
            __RPC__in IVsDataObjectStringMapEvents * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName);
        
        END_INTERFACE
    } IVsDataObjectStringMapEventsVtbl;

    interface IVsDataObjectStringMapEvents
    {
        CONST_VTBL struct IVsDataObjectStringMapEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDataObjectStringMapEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDataObjectStringMapEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDataObjectStringMapEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDataObjectStringMapEvents_OnStringMapChanged(This,pObject,szStringMapName)	\
    ( (This)->lpVtbl -> OnStringMapChanged(This,pObject,szStringMapName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDataObjectStringMapEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0019 */
/* [local] */ 

#define E_VS_MAPMISSING MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2001)


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0019_v0_0_s_ifspec;

#ifndef __IVsDataObjectStringMapManager_INTERFACE_DEFINED__
#define __IVsDataObjectStringMapManager_INTERFACE_DEFINED__

/* interface IVsDataObjectStringMapManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDataObjectStringMapManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("662AFCEC-AD0C-4f59-97E2-F61E0C673514")
    IVsDataObjectStringMapManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReadStringMap( 
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteStringMap( 
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [in] */ BOOL fOverwriteExisting,
            /* [in] */ __RPC__in_opt IVsStringMap *pStringMap) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateStringMap( 
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseChanges( 
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in_opt IVsDataObjectStringMapEvents *pStringMapEvents,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseChanges( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDataObjectStringMapManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDataObjectStringMapManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDataObjectStringMapManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReadStringMap )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *WriteStringMap )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in LPCWSTR szStringMapName,
            /* [in] */ BOOL fOverwriteExisting,
            /* [in] */ __RPC__in_opt IVsStringMap *pStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStringMap )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [out] */ __RPC__deref_out_opt IVsStringMap **ppStringMap);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanges )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [in] */ __RPC__in_opt IDataObject *pObject,
            /* [in] */ __RPC__in_opt IVsDataObjectStringMapEvents *pStringMapEvents,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanges )( 
            __RPC__in IVsDataObjectStringMapManager * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsDataObjectStringMapManagerVtbl;

    interface IVsDataObjectStringMapManager
    {
        CONST_VTBL struct IVsDataObjectStringMapManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDataObjectStringMapManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDataObjectStringMapManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDataObjectStringMapManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDataObjectStringMapManager_ReadStringMap(This,pObject,szStringMapName,ppStringMap)	\
    ( (This)->lpVtbl -> ReadStringMap(This,pObject,szStringMapName,ppStringMap) ) 

#define IVsDataObjectStringMapManager_WriteStringMap(This,pObject,szStringMapName,fOverwriteExisting,pStringMap)	\
    ( (This)->lpVtbl -> WriteStringMap(This,pObject,szStringMapName,fOverwriteExisting,pStringMap) ) 

#define IVsDataObjectStringMapManager_CreateStringMap(This,ppStringMap)	\
    ( (This)->lpVtbl -> CreateStringMap(This,ppStringMap) ) 

#define IVsDataObjectStringMapManager_AdviseChanges(This,pObject,pStringMapEvents,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseChanges(This,pObject,pStringMapEvents,pdwCookie) ) 

#define IVsDataObjectStringMapManager_UnadviseChanges(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseChanges(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDataObjectStringMapManager_INTERFACE_DEFINED__ */


#ifndef __SVsDataObjectStringMapManager_INTERFACE_DEFINED__
#define __SVsDataObjectStringMapManager_INTERFACE_DEFINED__

/* interface SVsDataObjectStringMapManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDataObjectStringMapManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8016834A-90E4-4e01-B24F-958DAA30B7D5")
    SVsDataObjectStringMapManager : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsDataObjectStringMapManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsDataObjectStringMapManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsDataObjectStringMapManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsDataObjectStringMapManager * This);
        
        END_INTERFACE
    } SVsDataObjectStringMapManagerVtbl;

    interface SVsDataObjectStringMapManager
    {
        CONST_VTBL struct SVsDataObjectStringMapManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDataObjectStringMapManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDataObjectStringMapManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDataObjectStringMapManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDataObjectStringMapManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0021 */
/* [local] */ 

#define SID_SVsDataObjectStringMapManager IID_SVsDataObjectStringMapManager
#define szTOOLBOX_MULTITARGETING_STRINGMAP L"MultiTargeting:{FBB22D27-7B21-42ac-88C8-595F94BDBCA5}"
#define szTOOLBOX_MULTITARGETING_TYPENAME L"TypeName"
#define szTOOLBOX_MULTITARGETING_ASSEMBLYNAME L"AssemblyName"
#define szTOOLBOX_MULTITARGETING_FRAMEWORKS L"Frameworks"
#define szTOOLBOX_MULTITARGETING_ITEMPROVIDER L"ItemProvider"
#define szTOOLBOX_MULTITARGETING_USEPTF_TOOLTIP L"UseProjectTargetFrameworkVersionInTooltip"


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0021_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0021_v0_0_s_ifspec;

#ifndef __IVsAddToolboxItems_INTERFACE_DEFINED__
#define __IVsAddToolboxItems_INTERFACE_DEFINED__

/* interface IVsAddToolboxItems */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsAddToolboxItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C1A29AC2-8FEC-407a-8650-41E05F5E87F7")
    IVsAddToolboxItems : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddItem( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [unique][in] */ __RPC__in_opt TBXITEMINFO *ptif,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szItemID,
            /* [unique][in] */ __RPC__in_opt LPCWSTR lpszTab) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddItemWithOwningPackage( 
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [unique][in] */ __RPC__in_opt TBXITEMINFO *ptif,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szItemID,
            /* [unique][in] */ __RPC__in_opt LPCWSTR lpszTab,
            /* [in] */ __RPC__in REFGUID guidPkg) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAddToolboxItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAddToolboxItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAddToolboxItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAddToolboxItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddItem )( 
            __RPC__in IVsAddToolboxItems * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [unique][in] */ __RPC__in_opt TBXITEMINFO *ptif,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szItemID,
            /* [unique][in] */ __RPC__in_opt LPCWSTR lpszTab);
        
        HRESULT ( STDMETHODCALLTYPE *AddItemWithOwningPackage )( 
            __RPC__in IVsAddToolboxItems * This,
            /* [in] */ __RPC__in_opt IDataObject *pDO,
            /* [unique][in] */ __RPC__in_opt TBXITEMINFO *ptif,
            /* [unique][in] */ __RPC__in_opt LPCWSTR szItemID,
            /* [unique][in] */ __RPC__in_opt LPCWSTR lpszTab,
            /* [in] */ __RPC__in REFGUID guidPkg);
        
        END_INTERFACE
    } IVsAddToolboxItemsVtbl;

    interface IVsAddToolboxItems
    {
        CONST_VTBL struct IVsAddToolboxItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddToolboxItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddToolboxItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddToolboxItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddToolboxItems_AddItem(This,pDO,ptif,szItemID,lpszTab)	\
    ( (This)->lpVtbl -> AddItem(This,pDO,ptif,szItemID,lpszTab) ) 

#define IVsAddToolboxItems_AddItemWithOwningPackage(This,pDO,ptif,szItemID,lpszTab,guidPkg)	\
    ( (This)->lpVtbl -> AddItemWithOwningPackage(This,pDO,ptif,szItemID,lpszTab,guidPkg) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddToolboxItems_INTERFACE_DEFINED__ */


#ifndef __IVsProvideTargetedToolboxItems_INTERFACE_DEFINED__
#define __IVsProvideTargetedToolboxItems_INTERFACE_DEFINED__

/* interface IVsProvideTargetedToolboxItems */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProvideTargetedToolboxItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CDF20EE2-B038-42ea-AA42-E1CAAFFCDCA5")
    IVsProvideTargetedToolboxItems : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetInstanceOfExistingTypeForNewFramework( 
            /* [in] */ __RPC__in_opt IDataObject *pExistingItem,
            /* [in] */ __RPC__in LPCWSTR szNewTFM,
            /* [in] */ __RPC__in_opt IVsAddToolboxItems *pAdder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddNewTypesForNewFramework( 
            /* [in] */ __RPC__in LPCWSTR szHighestExistingTFMWithSameID,
            /* [in] */ __RPC__in LPCWSTR szNewTFM,
            /* [in] */ __RPC__in_opt IVsAddToolboxItems *pAdder) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProvideTargetedToolboxItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProvideTargetedToolboxItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProvideTargetedToolboxItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProvideTargetedToolboxItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetInstanceOfExistingTypeForNewFramework )( 
            __RPC__in IVsProvideTargetedToolboxItems * This,
            /* [in] */ __RPC__in_opt IDataObject *pExistingItem,
            /* [in] */ __RPC__in LPCWSTR szNewTFM,
            /* [in] */ __RPC__in_opt IVsAddToolboxItems *pAdder);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewTypesForNewFramework )( 
            __RPC__in IVsProvideTargetedToolboxItems * This,
            /* [in] */ __RPC__in LPCWSTR szHighestExistingTFMWithSameID,
            /* [in] */ __RPC__in LPCWSTR szNewTFM,
            /* [in] */ __RPC__in_opt IVsAddToolboxItems *pAdder);
        
        END_INTERFACE
    } IVsProvideTargetedToolboxItemsVtbl;

    interface IVsProvideTargetedToolboxItems
    {
        CONST_VTBL struct IVsProvideTargetedToolboxItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideTargetedToolboxItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideTargetedToolboxItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideTargetedToolboxItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideTargetedToolboxItems_GetInstanceOfExistingTypeForNewFramework(This,pExistingItem,szNewTFM,pAdder)	\
    ( (This)->lpVtbl -> GetInstanceOfExistingTypeForNewFramework(This,pExistingItem,szNewTFM,pAdder) ) 

#define IVsProvideTargetedToolboxItems_AddNewTypesForNewFramework(This,szHighestExistingTFMWithSameID,szNewTFM,pAdder)	\
    ( (This)->lpVtbl -> AddNewTypesForNewFramework(This,szHighestExistingTFMWithSameID,szNewTFM,pAdder) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideTargetedToolboxItems_INTERFACE_DEFINED__ */


#ifndef __IVsDesignerInfo_INTERFACE_DEFINED__
#define __IVsDesignerInfo_INTERFACE_DEFINED__

/* interface IVsDesignerInfo */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDesignerInfo;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("59518321-3CCA-4bc9-BC1B-ADBCEEF21F14")
    IVsDesignerInfo : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_DesignerTechnology( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTechnology) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDesignerInfoVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDesignerInfo * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDesignerInfo * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDesignerInfo * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_DesignerTechnology )( 
            __RPC__in IVsDesignerInfo * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrTechnology);
        
        END_INTERFACE
    } IVsDesignerInfoVtbl;

    interface IVsDesignerInfo
    {
        CONST_VTBL struct IVsDesignerInfoVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDesignerInfo_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDesignerInfo_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDesignerInfo_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDesignerInfo_get_DesignerTechnology(This,pbstrTechnology)	\
    ( (This)->lpVtbl -> get_DesignerTechnology(This,pbstrTechnology) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDesignerInfo_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0024 */
/* [local] */ 


enum _VSTREEFLAGS2
    {
        TF_AVOIDPRECACHE	= 0x200
    } ;
typedef DWORD VSTREEFLAGS2;

typedef 
enum _VSTREETEXTOPTIONS2
    {
        TTO_SORTTEXT2	= 0x80
    } 	VSTREETEXTOPTIONS2;


enum __VSLITETREEOPTS2
    {
        LT_ENABLEEXPLORERTHEME	= 0x4
    } ;

enum __PSFFILEID4
    {
        PSFFILEID_WcfServiceReferencesConfig	= -1009,
        PSFFILEID_FIRST4	= -1009
    } ;
typedef LONG PSFFILEID4;

extern const __declspec(selectany) CLSID CLSID_UnloadedProject = { 0x76E22BD3, 0xC2EC, 0x47F1, { 0x80, 0x2B, 0x53, 0x19, 0x77, 0x56, 0xDA, 0xE8 } };
extern const __declspec(selectany) GUID UICONTEXT_SolutionExistsAndFullyLoaded = { 0x10534154, 0x102d, 0x46e2, { 0xab, 0xa8, 0xa6, 0xbf, 0xa2, 0x5b, 0xa0, 0xbe } };
extern const __declspec(selectany) GUID UICONTEXT_SolutionOpening = { 0xd2567162, 0xf94f, 0x4091, { 0x87, 0x98, 0xa0, 0x96, 0xe6, 0x1b, 0x8b, 0x50 } };
extern const __declspec(selectany) GUID UICONTEXT_ProjectRetargeting = { 0xde039a0e, 0xc18f, 0x490c, { 0x94, 0x4a, 0x88, 0x8b, 0x8e, 0x86, 0xda, 0x4b } };
extern const __declspec(selectany) GUID UICONTEXT_HistoricalDebugging = { 0xd1b1e38f, 0x1a7e, 0x4236, { 0xaf, 0x55, 0x6f, 0xa8, 0xf5, 0xfa, 0x76, 0xe6 } };
extern const __declspec(selectany) GUID UICONTEXT_DataSourceWizardSuppressed = { 0x5705ad15, 0x40ee, 0x4426, { 0xad, 0x3e, 0xba, 0x75, 0x06, 0x10, 0xD5, 0x99 } };


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0024_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0024_v0_0_s_ifspec;

#ifndef __IVsToolboxItemProvider_INTERFACE_DEFINED__
#define __IVsToolboxItemProvider_INTERFACE_DEFINED__

/* interface IVsToolboxItemProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolboxItemProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F2F94425-E001-4c4d-816C-70202E9A594C")
    IVsToolboxItemProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetItemContent( 
            /* [in] */ __RPC__in LPCWSTR szItemID,
            /* [in] */ CLIPFORMAT format,
            /* [out] */ __RPC__deref_out_opt HGLOBAL *pGlobal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolboxItemProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolboxItemProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolboxItemProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolboxItemProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemContent )( 
            __RPC__in IVsToolboxItemProvider * This,
            /* [in] */ __RPC__in LPCWSTR szItemID,
            /* [in] */ CLIPFORMAT format,
            /* [out] */ __RPC__deref_out_opt HGLOBAL *pGlobal);
        
        END_INTERFACE
    } IVsToolboxItemProviderVtbl;

    interface IVsToolboxItemProvider
    {
        CONST_VTBL struct IVsToolboxItemProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolboxItemProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolboxItemProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolboxItemProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolboxItemProvider_GetItemContent(This,szItemID,format,pGlobal)	\
    ( (This)->lpVtbl -> GetItemContent(This,szItemID,format,pGlobal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolboxItemProvider_INTERFACE_DEFINED__ */


#ifndef __IVsComponentSelectorDlg4_INTERFACE_DEFINED__
#define __IVsComponentSelectorDlg4_INTERFACE_DEFINED__

/* interface IVsComponentSelectorDlg4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsComponentSelectorDlg4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C43F5129-896F-4653-98E8-B0A16BFE0FC1")
    IVsComponentSelectorDlg4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ComponentSelectorDlg5( 
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) VSCOMPONENTSELECTORDATA *rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ __RPC__in LPCWSTR targetFrameworkMoniker) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsComponentSelectorDlg4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsComponentSelectorDlg4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsComponentSelectorDlg4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsComponentSelectorDlg4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ComponentSelectorDlg5 )( 
            __RPC__in IVsComponentSelectorDlg4 * This,
            /* [in] */ VSCOMPSELFLAGS2 grfFlags,
            /* [in] */ __RPC__in_opt IVsComponentUser *pUser,
            /* [in] */ ULONG cComponents,
            /* [size_is][in] */ __RPC__in_ecount_full(cComponents) VSCOMPONENTSELECTORDATA *rgpcsdComponents[  ],
            /* [in] */ __RPC__in LPCOLESTR lpszDlgTitle,
            /* [in] */ __RPC__in LPCOLESTR lpszHelpTopic,
            /* [out][in] */ __RPC__inout ULONG *pxDlgSize,
            /* [out][in] */ __RPC__inout ULONG *pyDlgSize,
            /* [in] */ ULONG cTabInitializers,
            /* [size_is][in] */ __RPC__in_ecount_full(cTabInitializers) VSCOMPONENTSELECTORTABINIT rgcstiTabInitializers[  ],
            /* [out][in] */ __RPC__inout GUID *pguidStartOnThisTab,
            /* [in] */ __RPC__in LPCOLESTR pszBrowseFilters,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrBrowseLocation,
            /* [in] */ __RPC__in LPCWSTR targetFrameworkMoniker);
        
        END_INTERFACE
    } IVsComponentSelectorDlg4Vtbl;

    interface IVsComponentSelectorDlg4
    {
        CONST_VTBL struct IVsComponentSelectorDlg4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentSelectorDlg4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentSelectorDlg4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentSelectorDlg4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentSelectorDlg4_ComponentSelectorDlg5(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation,targetFrameworkMoniker)	\
    ( (This)->lpVtbl -> ComponentSelectorDlg5(This,grfFlags,pUser,cComponents,rgpcsdComponents,lpszDlgTitle,lpszHelpTopic,pxDlgSize,pyDlgSize,cTabInitializers,rgcstiTabInitializers,pguidStartOnThisTab,pszBrowseFilters,pbstrBrowseLocation,targetFrameworkMoniker) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentSelectorDlg4_INTERFACE_DEFINED__ */


#ifndef __SVsComponentModelHost_INTERFACE_DEFINED__
#define __SVsComponentModelHost_INTERFACE_DEFINED__

/* interface SVsComponentModelHost */
/* [object][local][uuid] */ 


EXTERN_C const IID IID_SVsComponentModelHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DEBF6A60-A2CB-4059-8677-0067F046C4BC")
    SVsComponentModelHost : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsComponentModelHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsComponentModelHost * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsComponentModelHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsComponentModelHost * This);
        
        END_INTERFACE
    } SVsComponentModelHostVtbl;

    interface SVsComponentModelHost
    {
        CONST_VTBL struct SVsComponentModelHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsComponentModelHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsComponentModelHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsComponentModelHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsComponentModelHost_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0027 */
/* [local] */ 

#define SID_SVsComponentModelHost IID_SVsComponentModelHost


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0027_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0027_v0_0_s_ifspec;

#ifndef __IVsComponentModelHost_INTERFACE_DEFINED__
#define __IVsComponentModelHost_INTERFACE_DEFINED__

/* interface IVsComponentModelHost */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsComponentModelHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C442A6B0-3F06-4003-912D-9B8CB750C5E7")
    IVsComponentModelHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComponentAssemblies( 
            /* [in] */ ULONG cAssemblies,
            /* [size_is][out] */ __RPC__out_ecount_full(cAssemblies) BSTR rgbstrAssemblyPaths[  ],
            /* [out] */ __RPC__out ULONG *pcActualAssemblies) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCatalogCacheFolder( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFolderPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryLoadComponentAssemblies( 
            /* [in] */ ULONG cAssemblies,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssemblies) LPCWSTR prgAssemblyPaths[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cAssemblies) VARIANT_BOOL rgCanLoad[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsComponentModelHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsComponentModelHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsComponentModelHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsComponentModelHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComponentAssemblies )( 
            __RPC__in IVsComponentModelHost * This,
            /* [in] */ ULONG cAssemblies,
            /* [size_is][out] */ __RPC__out_ecount_full(cAssemblies) BSTR rgbstrAssemblyPaths[  ],
            /* [out] */ __RPC__out ULONG *pcActualAssemblies);
        
        HRESULT ( STDMETHODCALLTYPE *GetCatalogCacheFolder )( 
            __RPC__in IVsComponentModelHost * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFolderPath);
        
        HRESULT ( STDMETHODCALLTYPE *QueryLoadComponentAssemblies )( 
            __RPC__in IVsComponentModelHost * This,
            /* [in] */ ULONG cAssemblies,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssemblies) LPCWSTR prgAssemblyPaths[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cAssemblies) VARIANT_BOOL rgCanLoad[  ]);
        
        END_INTERFACE
    } IVsComponentModelHostVtbl;

    interface IVsComponentModelHost
    {
        CONST_VTBL struct IVsComponentModelHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentModelHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentModelHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentModelHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentModelHost_GetComponentAssemblies(This,cAssemblies,rgbstrAssemblyPaths,pcActualAssemblies)	\
    ( (This)->lpVtbl -> GetComponentAssemblies(This,cAssemblies,rgbstrAssemblyPaths,pcActualAssemblies) ) 

#define IVsComponentModelHost_GetCatalogCacheFolder(This,pbstrFolderPath)	\
    ( (This)->lpVtbl -> GetCatalogCacheFolder(This,pbstrFolderPath) ) 

#define IVsComponentModelHost_QueryLoadComponentAssemblies(This,cAssemblies,prgAssemblyPaths,rgCanLoad)	\
    ( (This)->lpVtbl -> QueryLoadComponentAssemblies(This,cAssemblies,prgAssemblyPaths,rgCanLoad) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentModelHost_INTERFACE_DEFINED__ */


#ifndef __IVsBuildPropertyStorage2_INTERFACE_DEFINED__
#define __IVsBuildPropertyStorage2_INTERFACE_DEFINED__

/* interface IVsBuildPropertyStorage2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildPropertyStorage2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3B175AC0-F7E2-4187-80A0-A73C39313C49")
    IVsBuildPropertyStorage2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetPropertyValueEx( 
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszPropertyGroupCondition,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBuildPropertyStorage2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBuildPropertyStorage2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBuildPropertyStorage2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBuildPropertyStorage2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetPropertyValueEx )( 
            __RPC__in IVsBuildPropertyStorage2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszPropName,
            /* [in] */ __RPC__in LPCOLESTR pszPropertyGroupCondition,
            /* [in] */ PersistStorageType storage,
            /* [in] */ __RPC__in LPCOLESTR pszPropValue);
        
        END_INTERFACE
    } IVsBuildPropertyStorage2Vtbl;

    interface IVsBuildPropertyStorage2
    {
        CONST_VTBL struct IVsBuildPropertyStorage2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildPropertyStorage2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildPropertyStorage2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildPropertyStorage2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildPropertyStorage2_SetPropertyValueEx(This,pszPropName,pszPropertyGroupCondition,storage,pszPropValue)	\
    ( (This)->lpVtbl -> SetPropertyValueEx(This,pszPropName,pszPropertyGroupCondition,storage,pszPropValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildPropertyStorage2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0029 */
/* [local] */ 


enum _BuildSystemKindFlags2
    {
        BSK_MSBUILD_VS9	= 1,
        BSK_MSBUILD_VS10	= 2
    } ;
typedef DWORD BuildSystemKindFlags2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0029_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0029_v0_0_s_ifspec;

#ifndef __IVsLanguageServiceBuildErrorReporter_INTERFACE_DEFINED__
#define __IVsLanguageServiceBuildErrorReporter_INTERFACE_DEFINED__

/* interface IVsLanguageServiceBuildErrorReporter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsLanguageServiceBuildErrorReporter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A7C1346C-6FD6-4ad5-A6FA-AE732AA42040")
    IVsLanguageServiceBuildErrorReporter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReportError( 
            /* [in] */ __RPC__in BSTR bstrErrorMessage,
            /* [in] */ __RPC__in BSTR bstrErrorId,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ long iLine,
            /* [in] */ long iColumn,
            /* [in] */ __RPC__in BSTR bstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearErrors( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsLanguageServiceBuildErrorReporterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReportError )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter * This,
            /* [in] */ __RPC__in BSTR bstrErrorMessage,
            /* [in] */ __RPC__in BSTR bstrErrorId,
            /* [in] */ VSTASKPRIORITY nPriority,
            /* [in] */ long iLine,
            /* [in] */ long iColumn,
            /* [in] */ __RPC__in BSTR bstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *ClearErrors )( 
            __RPC__in IVsLanguageServiceBuildErrorReporter * This);
        
        END_INTERFACE
    } IVsLanguageServiceBuildErrorReporterVtbl;

    interface IVsLanguageServiceBuildErrorReporter
    {
        CONST_VTBL struct IVsLanguageServiceBuildErrorReporterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsLanguageServiceBuildErrorReporter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsLanguageServiceBuildErrorReporter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsLanguageServiceBuildErrorReporter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsLanguageServiceBuildErrorReporter_ReportError(This,bstrErrorMessage,bstrErrorId,nPriority,iLine,iColumn,bstrFileName)	\
    ( (This)->lpVtbl -> ReportError(This,bstrErrorMessage,bstrErrorId,nPriority,iLine,iColumn,bstrFileName) ) 

#define IVsLanguageServiceBuildErrorReporter_ClearErrors(This)	\
    ( (This)->lpVtbl -> ClearErrors(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsLanguageServiceBuildErrorReporter_INTERFACE_DEFINED__ */


#ifndef __ILocalRegistry5_INTERFACE_DEFINED__
#define __ILocalRegistry5_INTERFACE_DEFINED__

/* interface ILocalRegistry5 */
/* [unique][uuid][local][object] */ 


EXTERN_C const IID IID_ILocalRegistry5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D5F528B9-E492-43a7-AFC0-F98A8FB0516D")
    ILocalRegistry5 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateAggregatedManagedInstance( 
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ LPVOID pUnkOuter,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ILocalRegistry5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILocalRegistry5 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILocalRegistry5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILocalRegistry5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateAggregatedManagedInstance )( 
            ILocalRegistry5 * This,
            /* [in] */ LPCWSTR codeBase,
            /* [in] */ LPCWSTR assemblyName,
            /* [in] */ LPCWSTR typeName,
            /* [in] */ LPVOID pUnkOuter,
            /* [in] */ REFIID riid,
            /* [out] */ void **ppvObj);
        
        END_INTERFACE
    } ILocalRegistry5Vtbl;

    interface ILocalRegistry5
    {
        CONST_VTBL struct ILocalRegistry5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILocalRegistry5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILocalRegistry5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILocalRegistry5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILocalRegistry5_CreateAggregatedManagedInstance(This,codeBase,assemblyName,typeName,pUnkOuter,riid,ppvObj)	\
    ( (This)->lpVtbl -> CreateAggregatedManagedInstance(This,codeBase,assemblyName,typeName,pUnkOuter,riid,ppvObj) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILocalRegistry5_INTERFACE_DEFINED__ */


#ifndef __IVsErrorItem2_INTERFACE_DEFINED__
#define __IVsErrorItem2_INTERFACE_DEFINED__

/* interface IVsErrorItem2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsErrorItem2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96C14EA1-1435-4DA9-8DDD-72DFBC2194E9")
    IVsErrorItem2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCustomIconIndex( 
            /* [out] */ __RPC__out long *index) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsErrorItem2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsErrorItem2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsErrorItem2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsErrorItem2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomIconIndex )( 
            __RPC__in IVsErrorItem2 * This,
            /* [out] */ __RPC__out long *index);
        
        END_INTERFACE
    } IVsErrorItem2Vtbl;

    interface IVsErrorItem2
    {
        CONST_VTBL struct IVsErrorItem2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsErrorItem2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsErrorItem2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsErrorItem2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsErrorItem2_GetCustomIconIndex(This,index)	\
    ( (This)->lpVtbl -> GetCustomIconIndex(This,index) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsErrorItem2_INTERFACE_DEFINED__ */


#ifndef __IVsOutputWindow3_INTERFACE_DEFINED__
#define __IVsOutputWindow3_INTERFACE_DEFINED__

/* interface IVsOutputWindow3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsOutputWindow3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35DBFD79-2B63-4355-A828-8E3D6D440687")
    IVsOutputWindow3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreatePane2( 
            /* [in] */ __RPC__in REFGUID rguidPane,
            /* [in] */ __RPC__in LPCOLESTR pszPaneName,
            /* [in] */ BOOL fInitVisible,
            /* [in] */ BOOL fClearWithSolution,
            /* [in] */ __RPC__in LPCOLESTR lpszContentType,
            /* [in] */ __RPC__in LPCOLESTR lpszTextViewRoles) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsOutputWindow3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsOutputWindow3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsOutputWindow3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsOutputWindow3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreatePane2 )( 
            __RPC__in IVsOutputWindow3 * This,
            /* [in] */ __RPC__in REFGUID rguidPane,
            /* [in] */ __RPC__in LPCOLESTR pszPaneName,
            /* [in] */ BOOL fInitVisible,
            /* [in] */ BOOL fClearWithSolution,
            /* [in] */ __RPC__in LPCOLESTR lpszContentType,
            /* [in] */ __RPC__in LPCOLESTR lpszTextViewRoles);
        
        END_INTERFACE
    } IVsOutputWindow3Vtbl;

    interface IVsOutputWindow3
    {
        CONST_VTBL struct IVsOutputWindow3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsOutputWindow3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsOutputWindow3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsOutputWindow3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsOutputWindow3_CreatePane2(This,rguidPane,pszPaneName,fInitVisible,fClearWithSolution,lpszContentType,lpszTextViewRoles)	\
    ( (This)->lpVtbl -> CreatePane2(This,rguidPane,pszPaneName,fInitVisible,fClearWithSolution,lpszContentType,lpszTextViewRoles) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsOutputWindow3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0033 */
/* [local] */ 


enum __VSBSLFLAGS
    {
        VSBSLFLAGS_None	= 0,
        VSBSLFLAGS_NotCancelable	= 0x1,
        VSBSLFLAGS_LoadBuildDependencies	= 0x2,
        VSBSLFLAGS_ExpandProjectOnLoad	= 0x4,
        VSBSLFLAGS_SelectProjectOnLoad	= 0x8,
        VSBSLFLAGS_LoadAllPendingProjects	= 0x10
    } ;
typedef DWORD VSBSLFLAGS;


enum _VSProjectLoadPriority
    {
        PLP_DemandLoad	= 0,
        PLP_BackgroundLoad	= 1,
        PLP_LoadIfNeeded	= 2,
        PLP_ExplicitLoadOnly	= 3
    } ;
typedef DWORD VSProjectLoadPriority;


enum _VSProjectUnloadStatus
    {
        UNLOADSTATUS_UnloadedByUser	= 0,
        UNLOADSTATUS_LoadPendingIfNeeded	= 1,
        UNLOADSTATUS_StorageNotLoadable	= 2,
        UNLOADSTATUS_StorageNotAvailable	= 3,
        UNLOADSTATUS_UpgradeFailed	= 4
    } ;
typedef DWORD VSProjectUnloadStatus;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0033_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0033_v0_0_s_ifspec;

#ifndef __IVsSolution4_INTERFACE_DEFINED__
#define __IVsSolution4_INTERFACE_DEFINED__

/* interface IVsSolution4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolution4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D2FB5B25-EAF0-4BE9-8E9B-F2C662AB9826")
    IVsSolution4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WriteUserOptsFile( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsBackgroundSolutionLoadEnabled( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfIsEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnsureProjectsAreLoaded( 
            /* [in] */ DWORD cProjects,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) GUID guidProjects[  ],
            /* [in] */ VSBSLFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnsureProjectIsLoaded( 
            /* [in] */ __RPC__in REFGUID guidProject,
            /* [in] */ VSBSLFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnsureSolutionIsLoaded( 
            /* [in] */ VSBSLFLAGS grfFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReloadProject( 
            /* [in] */ __RPC__in REFGUID guidProjectID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnloadProject( 
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ VSProjectUnloadStatus dwUnloadStatus) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolution4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolution4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolution4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *WriteUserOptsFile )( 
            __RPC__in IVsSolution4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsBackgroundSolutionLoadEnabled )( 
            __RPC__in IVsSolution4 * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfIsEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureProjectsAreLoaded )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ DWORD cProjects,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) GUID guidProjects[  ],
            /* [in] */ VSBSLFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureProjectIsLoaded )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ __RPC__in REFGUID guidProject,
            /* [in] */ VSBSLFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *EnsureSolutionIsLoaded )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ VSBSLFLAGS grfFlags);
        
        HRESULT ( STDMETHODCALLTYPE *ReloadProject )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ __RPC__in REFGUID guidProjectID);
        
        HRESULT ( STDMETHODCALLTYPE *UnloadProject )( 
            __RPC__in IVsSolution4 * This,
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ VSProjectUnloadStatus dwUnloadStatus);
        
        END_INTERFACE
    } IVsSolution4Vtbl;

    interface IVsSolution4
    {
        CONST_VTBL struct IVsSolution4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolution4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolution4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolution4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolution4_WriteUserOptsFile(This)	\
    ( (This)->lpVtbl -> WriteUserOptsFile(This) ) 

#define IVsSolution4_IsBackgroundSolutionLoadEnabled(This,pfIsEnabled)	\
    ( (This)->lpVtbl -> IsBackgroundSolutionLoadEnabled(This,pfIsEnabled) ) 

#define IVsSolution4_EnsureProjectsAreLoaded(This,cProjects,guidProjects,grfFlags)	\
    ( (This)->lpVtbl -> EnsureProjectsAreLoaded(This,cProjects,guidProjects,grfFlags) ) 

#define IVsSolution4_EnsureProjectIsLoaded(This,guidProject,grfFlags)	\
    ( (This)->lpVtbl -> EnsureProjectIsLoaded(This,guidProject,grfFlags) ) 

#define IVsSolution4_EnsureSolutionIsLoaded(This,grfFlags)	\
    ( (This)->lpVtbl -> EnsureSolutionIsLoaded(This,grfFlags) ) 

#define IVsSolution4_ReloadProject(This,guidProjectID)	\
    ( (This)->lpVtbl -> ReloadProject(This,guidProjectID) ) 

#define IVsSolution4_UnloadProject(This,guidProjectID,dwUnloadStatus)	\
    ( (This)->lpVtbl -> UnloadProject(This,guidProjectID,dwUnloadStatus) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolution4_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionLoadManagerSupport_INTERFACE_DEFINED__
#define __IVsSolutionLoadManagerSupport_INTERFACE_DEFINED__

/* interface IVsSolutionLoadManagerSupport */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionLoadManagerSupport;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D48DB33C-0F89-47AD-AB42-D6683608BD60")
    IVsSolutionLoadManagerSupport : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetProjectLoadPriority( 
            /* [in] */ __RPC__in REFGUID refguidProject,
            /* [in] */ VSProjectLoadPriority loadState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProjectLoadPriority( 
            /* [in] */ __RPC__in REFGUID refguidProject,
            /* [out] */ __RPC__out VSProjectLoadPriority *pLoadState) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionLoadManagerSupportVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionLoadManagerSupport * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionLoadManagerSupport * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionLoadManagerSupport * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetProjectLoadPriority )( 
            __RPC__in IVsSolutionLoadManagerSupport * This,
            /* [in] */ __RPC__in REFGUID refguidProject,
            /* [in] */ VSProjectLoadPriority loadState);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectLoadPriority )( 
            __RPC__in IVsSolutionLoadManagerSupport * This,
            /* [in] */ __RPC__in REFGUID refguidProject,
            /* [out] */ __RPC__out VSProjectLoadPriority *pLoadState);
        
        END_INTERFACE
    } IVsSolutionLoadManagerSupportVtbl;

    interface IVsSolutionLoadManagerSupport
    {
        CONST_VTBL struct IVsSolutionLoadManagerSupportVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionLoadManagerSupport_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionLoadManagerSupport_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionLoadManagerSupport_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionLoadManagerSupport_SetProjectLoadPriority(This,refguidProject,loadState)	\
    ( (This)->lpVtbl -> SetProjectLoadPriority(This,refguidProject,loadState) ) 

#define IVsSolutionLoadManagerSupport_GetProjectLoadPriority(This,refguidProject,pLoadState)	\
    ( (This)->lpVtbl -> GetProjectLoadPriority(This,refguidProject,pLoadState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionLoadManagerSupport_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionLoadManager_INTERFACE_DEFINED__
#define __IVsSolutionLoadManager_INTERFACE_DEFINED__

/* interface IVsSolutionLoadManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionLoadManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DCF13D02-C7A2-427F-9F03-B3360257B301")
    IVsSolutionLoadManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnDisconnect( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeOpenProject( 
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ __RPC__in REFGUID guidProjectType,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsSolutionLoadManagerSupport *pSLMgrSupport) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionLoadManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionLoadManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionLoadManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionLoadManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnDisconnect )( 
            __RPC__in IVsSolutionLoadManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeOpenProject )( 
            __RPC__in IVsSolutionLoadManager * This,
            /* [in] */ __RPC__in REFGUID guidProjectID,
            /* [in] */ __RPC__in REFGUID guidProjectType,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [in] */ __RPC__in_opt IVsSolutionLoadManagerSupport *pSLMgrSupport);
        
        END_INTERFACE
    } IVsSolutionLoadManagerVtbl;

    interface IVsSolutionLoadManager
    {
        CONST_VTBL struct IVsSolutionLoadManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionLoadManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionLoadManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionLoadManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionLoadManager_OnDisconnect(This)	\
    ( (This)->lpVtbl -> OnDisconnect(This) ) 

#define IVsSolutionLoadManager_OnBeforeOpenProject(This,guidProjectID,guidProjectType,pszFileName,pSLMgrSupport)	\
    ( (This)->lpVtbl -> OnBeforeOpenProject(This,guidProjectID,guidProjectType,pszFileName,pSLMgrSupport) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionLoadManager_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionLoadEvents_INTERFACE_DEFINED__
#define __IVsSolutionLoadEvents_INTERFACE_DEFINED__

/* interface IVsSolutionLoadEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionLoadEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6ACFF38A-0D6C-4792-B9D2-9469D60A2AD7")
    IVsSolutionLoadEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeOpenSolution( 
            /* [in] */ __RPC__in LPCOLESTR pszSolutionFilename) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeBackgroundSolutionLoadBegins( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryBackgroundLoadProjectBatch( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfShouldDelayLoadToNextIdle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnBeforeLoadProjectBatch( 
            /* [in] */ VARIANT_BOOL fIsBackgroundIdleBatch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterLoadProjectBatch( 
            /* [in] */ VARIANT_BOOL fIsBackgroundIdleBatch) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterBackgroundSolutionLoadComplete( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionLoadEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionLoadEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionLoadEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionLoadEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeOpenSolution )( 
            __RPC__in IVsSolutionLoadEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszSolutionFilename);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeBackgroundSolutionLoadBegins )( 
            __RPC__in IVsSolutionLoadEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryBackgroundLoadProjectBatch )( 
            __RPC__in IVsSolutionLoadEvents * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfShouldDelayLoadToNextIdle);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeLoadProjectBatch )( 
            __RPC__in IVsSolutionLoadEvents * This,
            /* [in] */ VARIANT_BOOL fIsBackgroundIdleBatch);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterLoadProjectBatch )( 
            __RPC__in IVsSolutionLoadEvents * This,
            /* [in] */ VARIANT_BOOL fIsBackgroundIdleBatch);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterBackgroundSolutionLoadComplete )( 
            __RPC__in IVsSolutionLoadEvents * This);
        
        END_INTERFACE
    } IVsSolutionLoadEventsVtbl;

    interface IVsSolutionLoadEvents
    {
        CONST_VTBL struct IVsSolutionLoadEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionLoadEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionLoadEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionLoadEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionLoadEvents_OnBeforeOpenSolution(This,pszSolutionFilename)	\
    ( (This)->lpVtbl -> OnBeforeOpenSolution(This,pszSolutionFilename) ) 

#define IVsSolutionLoadEvents_OnBeforeBackgroundSolutionLoadBegins(This)	\
    ( (This)->lpVtbl -> OnBeforeBackgroundSolutionLoadBegins(This) ) 

#define IVsSolutionLoadEvents_OnQueryBackgroundLoadProjectBatch(This,pfShouldDelayLoadToNextIdle)	\
    ( (This)->lpVtbl -> OnQueryBackgroundLoadProjectBatch(This,pfShouldDelayLoadToNextIdle) ) 

#define IVsSolutionLoadEvents_OnBeforeLoadProjectBatch(This,fIsBackgroundIdleBatch)	\
    ( (This)->lpVtbl -> OnBeforeLoadProjectBatch(This,fIsBackgroundIdleBatch) ) 

#define IVsSolutionLoadEvents_OnAfterLoadProjectBatch(This,fIsBackgroundIdleBatch)	\
    ( (This)->lpVtbl -> OnAfterLoadProjectBatch(This,fIsBackgroundIdleBatch) ) 

#define IVsSolutionLoadEvents_OnAfterBackgroundSolutionLoadComplete(This)	\
    ( (This)->lpVtbl -> OnAfterBackgroundSolutionLoadComplete(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionLoadEvents_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0037 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0037_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0037_v0_0_s_ifspec;

#ifndef __IVsThreadedWaitDialogFactory_INTERFACE_DEFINED__
#define __IVsThreadedWaitDialogFactory_INTERFACE_DEFINED__

/* interface IVsThreadedWaitDialogFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadedWaitDialogFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D10D92B6-D073-456F-8A26-B63811202B21")
    IVsThreadedWaitDialogFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateInstance( 
            /* [out] */ __RPC__deref_out_opt IVsThreadedWaitDialog2 **ppIVsThreadedWaitDialog) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsThreadedWaitDialogFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsThreadedWaitDialogFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsThreadedWaitDialogFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsThreadedWaitDialogFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstance )( 
            __RPC__in IVsThreadedWaitDialogFactory * This,
            /* [out] */ __RPC__deref_out_opt IVsThreadedWaitDialog2 **ppIVsThreadedWaitDialog);
        
        END_INTERFACE
    } IVsThreadedWaitDialogFactoryVtbl;

    interface IVsThreadedWaitDialogFactory
    {
        CONST_VTBL struct IVsThreadedWaitDialogFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadedWaitDialogFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadedWaitDialogFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadedWaitDialogFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadedWaitDialogFactory_CreateInstance(This,ppIVsThreadedWaitDialog)	\
    ( (This)->lpVtbl -> CreateInstance(This,ppIVsThreadedWaitDialog) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadedWaitDialogFactory_INTERFACE_DEFINED__ */


#ifndef __SVsThreadedWaitDialogFactory_INTERFACE_DEFINED__
#define __SVsThreadedWaitDialogFactory_INTERFACE_DEFINED__

/* interface SVsThreadedWaitDialogFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsThreadedWaitDialogFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D2DDF9C8-FE3B-4DD9-9499-78D8EA277210")
    SVsThreadedWaitDialogFactory : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsThreadedWaitDialogFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsThreadedWaitDialogFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsThreadedWaitDialogFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsThreadedWaitDialogFactory * This);
        
        END_INTERFACE
    } SVsThreadedWaitDialogFactoryVtbl;

    interface SVsThreadedWaitDialogFactory
    {
        CONST_VTBL struct SVsThreadedWaitDialogFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsThreadedWaitDialogFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsThreadedWaitDialogFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsThreadedWaitDialogFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsThreadedWaitDialogFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0039 */
/* [local] */ 

#define SID_SVsThreadedWaitDialogFactory IID_SVsThreadedWaitDialogFactory


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0039_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0039_v0_0_s_ifspec;

#ifndef __IVsThreadedWaitDialog2_INTERFACE_DEFINED__
#define __IVsThreadedWaitDialog2_INTERFACE_DEFINED__

/* interface IVsThreadedWaitDialog2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsThreadedWaitDialog2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("88194D8B-88DA-4C33-A2C6-15140626E222")
    IVsThreadedWaitDialog2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StartWaitDialog( 
            /* [in] */ __RPC__in LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ VARIANT_BOOL fShowMarqueeProgress) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartWaitDialogWithPercentageProgress( 
            /* [in] */ __RPC__in LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ LONG iCurrentStep) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndWaitDialog( 
            /* [out] */ __RPC__out BOOL *pfCanceled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateProgress( 
            /* [in] */ __RPC__in LPCWSTR szUpdatedWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iCurrentStep,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ VARIANT_BOOL fDisableCancel,
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HasCanceled( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsThreadedWaitDialog2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsThreadedWaitDialog2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsThreadedWaitDialog2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsThreadedWaitDialog2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialog )( 
            __RPC__in IVsThreadedWaitDialog2 * This,
            /* [in] */ __RPC__in LPCWSTR szWaitCaption,
            /* [in] */ __RPC__in LPCWSTR szWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ VARIANT varStatusBmpAnim,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iDelayToShowDialog,
            /* [in] */ VARIANT_BOOL fIsCancelable,
            /* [in] */ VARIANT_BOOL fShowMarqueeProgress);
        
        HRESULT ( STDMETHODCALLTYPE *StartWaitDialogWithPercentageProgress )( 
            __RPC__in IVsThreadedWaitDialog2 * This,
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
            __RPC__in IVsThreadedWaitDialog2 * This,
            /* [out] */ __RPC__out BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateProgress )( 
            __RPC__in IVsThreadedWaitDialog2 * This,
            /* [in] */ __RPC__in LPCWSTR szUpdatedWaitMessage,
            /* [in] */ __RPC__in LPCWSTR szProgressText,
            /* [in] */ __RPC__in LPCWSTR szStatusBarText,
            /* [in] */ LONG iCurrentStep,
            /* [in] */ LONG iTotalSteps,
            /* [in] */ VARIANT_BOOL fDisableCancel,
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled);
        
        HRESULT ( STDMETHODCALLTYPE *HasCanceled )( 
            __RPC__in IVsThreadedWaitDialog2 * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfCanceled);
        
        END_INTERFACE
    } IVsThreadedWaitDialog2Vtbl;

    interface IVsThreadedWaitDialog2
    {
        CONST_VTBL struct IVsThreadedWaitDialog2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsThreadedWaitDialog2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsThreadedWaitDialog2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsThreadedWaitDialog2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsThreadedWaitDialog2_StartWaitDialog(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,iDelayToShowDialog,fIsCancelable,fShowMarqueeProgress)	\
    ( (This)->lpVtbl -> StartWaitDialog(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,iDelayToShowDialog,fIsCancelable,fShowMarqueeProgress) ) 

#define IVsThreadedWaitDialog2_StartWaitDialogWithPercentageProgress(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,iTotalSteps,iCurrentStep)	\
    ( (This)->lpVtbl -> StartWaitDialogWithPercentageProgress(This,szWaitCaption,szWaitMessage,szProgressText,varStatusBmpAnim,szStatusBarText,fIsCancelable,iDelayToShowDialog,iTotalSteps,iCurrentStep) ) 

#define IVsThreadedWaitDialog2_EndWaitDialog(This,pfCanceled)	\
    ( (This)->lpVtbl -> EndWaitDialog(This,pfCanceled) ) 

#define IVsThreadedWaitDialog2_UpdateProgress(This,szUpdatedWaitMessage,szProgressText,szStatusBarText,iCurrentStep,iTotalSteps,fDisableCancel,pfCanceled)	\
    ( (This)->lpVtbl -> UpdateProgress(This,szUpdatedWaitMessage,szProgressText,szStatusBarText,iCurrentStep,iTotalSteps,fDisableCancel,pfCanceled) ) 

#define IVsThreadedWaitDialog2_HasCanceled(This,pfCanceled)	\
    ( (This)->lpVtbl -> HasCanceled(This,pfCanceled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsThreadedWaitDialog2_INTERFACE_DEFINED__ */


#ifndef __IVsToolboxPageChooser_INTERFACE_DEFINED__
#define __IVsToolboxPageChooser_INTERFACE_DEFINED__

/* interface IVsToolboxPageChooser */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolboxPageChooser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("652C0D0F-217E-46E8-9B85-0EC52279DA8F")
    IVsToolboxPageChooser : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPreferredToolboxPage( 
            /* [out] */ __RPC__out GUID *pguidPage) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolboxPageChooserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolboxPageChooser * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolboxPageChooser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolboxPageChooser * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPreferredToolboxPage )( 
            __RPC__in IVsToolboxPageChooser * This,
            /* [out] */ __RPC__out GUID *pguidPage);
        
        END_INTERFACE
    } IVsToolboxPageChooserVtbl;

    interface IVsToolboxPageChooser
    {
        CONST_VTBL struct IVsToolboxPageChooserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolboxPageChooser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolboxPageChooser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolboxPageChooser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolboxPageChooser_GetPreferredToolboxPage(This,pguidPage)	\
    ( (This)->lpVtbl -> GetPreferredToolboxPage(This,pguidPage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolboxPageChooser_INTERFACE_DEFINED__ */


#ifndef __IVsResourceManager2_INTERFACE_DEFINED__
#define __IVsResourceManager2_INTERFACE_DEFINED__

/* interface IVsResourceManager2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsResourceManager2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2597DABC-BBA7-4758-9A33-8894157AD74A")
    IVsResourceManager2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ParseResourceID( 
            /* [in] */ __RPC__in LPCWSTR szId,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUnadornedId,
            /* [out] */ __RPC__out GUID *pguidPackage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDllPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AnnotateIDIfNecessary( 
            /* [in] */ __RPC__in LPCWSTR szId,
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ __RPC__in LPCWSTR szDllPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAnnotatedId) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsResourceManager2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsResourceManager2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsResourceManager2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsResourceManager2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ParseResourceID )( 
            __RPC__in IVsResourceManager2 * This,
            /* [in] */ __RPC__in LPCWSTR szId,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUnadornedId,
            /* [out] */ __RPC__out GUID *pguidPackage,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDllPath);
        
        HRESULT ( STDMETHODCALLTYPE *AnnotateIDIfNecessary )( 
            __RPC__in IVsResourceManager2 * This,
            /* [in] */ __RPC__in LPCWSTR szId,
            /* [in] */ __RPC__in REFGUID guidPackage,
            /* [in] */ __RPC__in LPCWSTR szDllPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrAnnotatedId);
        
        END_INTERFACE
    } IVsResourceManager2Vtbl;

    interface IVsResourceManager2
    {
        CONST_VTBL struct IVsResourceManager2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsResourceManager2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsResourceManager2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsResourceManager2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsResourceManager2_ParseResourceID(This,szId,lcid,pbstrUnadornedId,pguidPackage,pbstrDllPath)	\
    ( (This)->lpVtbl -> ParseResourceID(This,szId,lcid,pbstrUnadornedId,pguidPackage,pbstrDllPath) ) 

#define IVsResourceManager2_AnnotateIDIfNecessary(This,szId,guidPackage,szDllPath,pbstrAnnotatedId)	\
    ( (This)->lpVtbl -> AnnotateIDIfNecessary(This,szId,guidPackage,szDllPath,pbstrAnnotatedId) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsResourceManager2_INTERFACE_DEFINED__ */


#ifndef __IVsProject4_INTERFACE_DEFINED__
#define __IVsProject4_INTERFACE_DEFINED__

/* interface IVsProject4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProject4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("12A0D88D-D8FE-4637-8350-214B5C29DE31")
    IVsProject4 : public IVsProject3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ContainsFileEndingWith( 
            /* [in] */ __RPC__in LPCOLESTR pszEndingWith,
            /* [retval][out] */ __RPC__out BOOL *pfDoesContain) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContainsFileWithItemType( 
            /* [in] */ __RPC__in LPCOLESTR pszItemType,
            /* [retval][out] */ __RPC__out BOOL *pfDoesContain) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFilesEndingWith( 
            /* [in] */ __RPC__in LPCOLESTR pszEndingWith,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSITEMID rgItemids[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFilesWithItemType( 
            /* [in] */ __RPC__in LPCOLESTR pszItemType,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSITEMID rgItemids[  ],
            /* [out] */ __RPC__out ULONG *pcActual) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProject4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProject4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProject4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsDocumentInProject )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pfFound,
            /* [out] */ __RPC__out VSDOCUMENTPRIORITY *pdwPriority,
            /* [out] */ __RPC__out VSITEMID *pitemid);
        
        HRESULT ( STDMETHODCALLTYPE *GetMkDocument )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMkDocument);
        
        HRESULT ( STDMETHODCALLTYPE *OpenItem )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [in] */ __RPC__in_opt IUnknown *punkDocDataExisting,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppWindowFrame);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemContext )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__deref_out_opt IServiceProvider **ppSP);
        
        HRESULT ( STDMETHODCALLTYPE *GenerateUniqueItemName )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ __RPC__in LPCOLESTR pszExt,
            /* [in] */ __RPC__in LPCOLESTR pszSuggestedRoot,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrItemName);
        
        HRESULT ( STDMETHODCALLTYPE *AddItem )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSADDITEMOPERATION dwAddItemOperation,
            /* [in] */ __RPC__in LPCOLESTR pszItemName,
            /* [in] */ ULONG cFilesToOpen,
            /* [size_is][in] */ __RPC__in_ecount_full(cFilesToOpen) LPCOLESTR rgpszFilesToOpen[  ],
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveItem )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ DWORD dwReserved,
            /* [in] */ VSITEMID itemid,
            /* [retval][out] */ __RPC__out BOOL *pfResult);
        
        HRESULT ( STDMETHODCALLTYPE *ReopenItem )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in REFGUID rguidEditorType,
            /* [in] */ __RPC__in LPCOLESTR pszPhysicalView,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [in] */ __RPC__in_opt IUnknown *punkDocDataExisting,
            /* [retval][out] */ __RPC__deref_out_opt IVsWindowFrame **ppWindowFrame);
        
        HRESULT ( STDMETHODCALLTYPE *AddItemWithSpecific )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemidLoc,
            /* [in] */ VSADDITEMOPERATION dwAddItemOperation,
            /* [in] */ __RPC__in LPCOLESTR pszItemName,
            /* [in] */ ULONG cFilesToOpen,
            /* [size_is][in] */ __RPC__in_ecount_full(cFilesToOpen) LPCOLESTR rgpszFilesToOpen[  ],
            /* [in] */ __RPC__in HWND hwndDlgOwner,
            /* [in] */ VSSPECIFICEDITORFLAGS grfEditorFlags,
            /* [in] */ __RPC__in REFGUID rguidEditorType,
            /* [in] */ __RPC__in LPCOLESTR pszPhysicalView,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [retval][out] */ __RPC__out VSADDRESULT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *OpenItemWithSpecific )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ VSSPECIFICEDITORFLAGS grfEditorFlags,
            /* [in] */ __RPC__in REFGUID rguidEditorType,
            /* [in] */ __RPC__in LPCOLESTR pszPhysicalView,
            /* [in] */ __RPC__in REFGUID rguidLogicalView,
            /* [in] */ __RPC__in_opt IUnknown *punkDocDataExisting,
            /* [out] */ __RPC__deref_out_opt IVsWindowFrame **ppWindowFrame);
        
        HRESULT ( STDMETHODCALLTYPE *TransferItem )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocumentOld,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocumentNew,
            /* [in] */ __RPC__in_opt IVsWindowFrame *punkWindowFrame);
        
        HRESULT ( STDMETHODCALLTYPE *ContainsFileEndingWith )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszEndingWith,
            /* [retval][out] */ __RPC__out BOOL *pfDoesContain);
        
        HRESULT ( STDMETHODCALLTYPE *ContainsFileWithItemType )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszItemType,
            /* [retval][out] */ __RPC__out BOOL *pfDoesContain);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilesEndingWith )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszEndingWith,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSITEMID rgItemids[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilesWithItemType )( 
            __RPC__in IVsProject4 * This,
            /* [in] */ __RPC__in LPCOLESTR pszItemType,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pcActual) VSITEMID rgItemids[  ],
            /* [out] */ __RPC__out ULONG *pcActual);
        
        END_INTERFACE
    } IVsProject4Vtbl;

    interface IVsProject4
    {
        CONST_VTBL struct IVsProject4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProject4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProject4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProject4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProject4_IsDocumentInProject(This,pszMkDocument,pfFound,pdwPriority,pitemid)	\
    ( (This)->lpVtbl -> IsDocumentInProject(This,pszMkDocument,pfFound,pdwPriority,pitemid) ) 

#define IVsProject4_GetMkDocument(This,itemid,pbstrMkDocument)	\
    ( (This)->lpVtbl -> GetMkDocument(This,itemid,pbstrMkDocument) ) 

#define IVsProject4_OpenItem(This,itemid,rguidLogicalView,punkDocDataExisting,ppWindowFrame)	\
    ( (This)->lpVtbl -> OpenItem(This,itemid,rguidLogicalView,punkDocDataExisting,ppWindowFrame) ) 

#define IVsProject4_GetItemContext(This,itemid,ppSP)	\
    ( (This)->lpVtbl -> GetItemContext(This,itemid,ppSP) ) 

#define IVsProject4_GenerateUniqueItemName(This,itemidLoc,pszExt,pszSuggestedRoot,pbstrItemName)	\
    ( (This)->lpVtbl -> GenerateUniqueItemName(This,itemidLoc,pszExt,pszSuggestedRoot,pbstrItemName) ) 

#define IVsProject4_AddItem(This,itemidLoc,dwAddItemOperation,pszItemName,cFilesToOpen,rgpszFilesToOpen,hwndDlgOwner,pResult)	\
    ( (This)->lpVtbl -> AddItem(This,itemidLoc,dwAddItemOperation,pszItemName,cFilesToOpen,rgpszFilesToOpen,hwndDlgOwner,pResult) ) 


#define IVsProject4_RemoveItem(This,dwReserved,itemid,pfResult)	\
    ( (This)->lpVtbl -> RemoveItem(This,dwReserved,itemid,pfResult) ) 

#define IVsProject4_ReopenItem(This,itemid,rguidEditorType,pszPhysicalView,rguidLogicalView,punkDocDataExisting,ppWindowFrame)	\
    ( (This)->lpVtbl -> ReopenItem(This,itemid,rguidEditorType,pszPhysicalView,rguidLogicalView,punkDocDataExisting,ppWindowFrame) ) 


#define IVsProject4_AddItemWithSpecific(This,itemidLoc,dwAddItemOperation,pszItemName,cFilesToOpen,rgpszFilesToOpen,hwndDlgOwner,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,pResult)	\
    ( (This)->lpVtbl -> AddItemWithSpecific(This,itemidLoc,dwAddItemOperation,pszItemName,cFilesToOpen,rgpszFilesToOpen,hwndDlgOwner,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,pResult) ) 

#define IVsProject4_OpenItemWithSpecific(This,itemid,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,punkDocDataExisting,ppWindowFrame)	\
    ( (This)->lpVtbl -> OpenItemWithSpecific(This,itemid,grfEditorFlags,rguidEditorType,pszPhysicalView,rguidLogicalView,punkDocDataExisting,ppWindowFrame) ) 

#define IVsProject4_TransferItem(This,pszMkDocumentOld,pszMkDocumentNew,punkWindowFrame)	\
    ( (This)->lpVtbl -> TransferItem(This,pszMkDocumentOld,pszMkDocumentNew,punkWindowFrame) ) 


#define IVsProject4_ContainsFileEndingWith(This,pszEndingWith,pfDoesContain)	\
    ( (This)->lpVtbl -> ContainsFileEndingWith(This,pszEndingWith,pfDoesContain) ) 

#define IVsProject4_ContainsFileWithItemType(This,pszItemType,pfDoesContain)	\
    ( (This)->lpVtbl -> ContainsFileWithItemType(This,pszItemType,pfDoesContain) ) 

#define IVsProject4_GetFilesEndingWith(This,pszEndingWith,celt,rgItemids,pcActual)	\
    ( (This)->lpVtbl -> GetFilesEndingWith(This,pszEndingWith,celt,rgItemids,pcActual) ) 

#define IVsProject4_GetFilesWithItemType(This,pszItemType,celt,rgItemids,pcActual)	\
    ( (This)->lpVtbl -> GetFilesWithItemType(This,pszItemType,celt,rgItemids,pcActual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProject4_INTERFACE_DEFINED__ */


#ifndef __IVsProjectUpgradeViaFactory3_INTERFACE_DEFINED__
#define __IVsProjectUpgradeViaFactory3_INTERFACE_DEFINED__

/* interface IVsProjectUpgradeViaFactory3 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectUpgradeViaFactory3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("943CE488-176F-457B-8C88-3502C775501C")
    IVsProjectUpgradeViaFactory3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CheckProjectUpgraded( 
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [out] */ __RPC__out VARIANT_BOOL *pUpgradeComplete,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedProjectFileName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectUpgradeViaFactory3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectUpgradeViaFactory3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectUpgradeViaFactory3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectUpgradeViaFactory3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckProjectUpgraded )( 
            __RPC__in IVsProjectUpgradeViaFactory3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszFileName,
            /* [out] */ __RPC__out VARIANT_BOOL *pUpgradeComplete,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pbstrUpgradedProjectFileName);
        
        END_INTERFACE
    } IVsProjectUpgradeViaFactory3Vtbl;

    interface IVsProjectUpgradeViaFactory3
    {
        CONST_VTBL struct IVsProjectUpgradeViaFactory3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectUpgradeViaFactory3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectUpgradeViaFactory3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectUpgradeViaFactory3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectUpgradeViaFactory3_CheckProjectUpgraded(This,pszFileName,pUpgradeComplete,pbstrUpgradedProjectFileName)	\
    ( (This)->lpVtbl -> CheckProjectUpgraded(This,pszFileName,pUpgradeComplete,pbstrUpgradedProjectFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectUpgradeViaFactory3_INTERFACE_DEFINED__ */


#ifndef __SVsBuildManagerAccessor_INTERFACE_DEFINED__
#define __SVsBuildManagerAccessor_INTERFACE_DEFINED__

/* interface SVsBuildManagerAccessor */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsBuildManagerAccessor;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9A9569D2-2F7E-4af6-9E40-630F86D3F3F9")
    SVsBuildManagerAccessor : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsBuildManagerAccessorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsBuildManagerAccessor * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsBuildManagerAccessor * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsBuildManagerAccessor * This);
        
        END_INTERFACE
    } SVsBuildManagerAccessorVtbl;

    interface SVsBuildManagerAccessor
    {
        CONST_VTBL struct SVsBuildManagerAccessorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsBuildManagerAccessor_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsBuildManagerAccessor_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsBuildManagerAccessor_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsBuildManagerAccessor_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0045 */
/* [local] */ 

#define SID_SVsBuildManagerAccessor IID_SVsBuildManagerAccessor


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0045_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0045_v0_0_s_ifspec;

#ifndef __IVsBuildManagerAccessor_INTERFACE_DEFINED__
#define __IVsBuildManagerAccessor_INTERFACE_DEFINED__

/* interface IVsBuildManagerAccessor */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildManagerAccessor;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C580A81B-9FFC-41eb-B8D7-5DCAD29601B7")
    IVsBuildManagerAccessor : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterLogger( 
            /* [in] */ LONG submissionId,
            /* [in] */ __RPC__in_opt IUnknown *punkLogger) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterLoggers( 
            /* [in] */ LONG submissionId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClaimUIThreadForBuild( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReleaseUIThreadForBuild( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginDesignTimeBuild( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndDesignTimeBuild( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentBatchBuildId( 
            /* [out] */ __RPC__out ULONG *pBatchId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSolutionConfiguration( 
            /* [in] */ __RPC__in_opt IUnknown *punkRootProject,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXmlFragment) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Escape( 
            /* [in] */ __RPC__in LPCOLESTR pwszUnescapedValue,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrEscapedValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Unescape( 
            /* [in] */ __RPC__in LPCOLESTR pwszEscapedValue,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUnescapedValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBuildManagerAccessorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterLogger )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ LONG submissionId,
            /* [in] */ __RPC__in_opt IUnknown *punkLogger);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterLoggers )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ LONG submissionId);
        
        HRESULT ( STDMETHODCALLTYPE *ClaimUIThreadForBuild )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReleaseUIThreadForBuild )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginDesignTimeBuild )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndDesignTimeBuild )( 
            __RPC__in IVsBuildManagerAccessor * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentBatchBuildId )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [out] */ __RPC__out ULONG *pBatchId);
        
        HRESULT ( STDMETHODCALLTYPE *GetSolutionConfiguration )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ __RPC__in_opt IUnknown *punkRootProject,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrXmlFragment);
        
        HRESULT ( STDMETHODCALLTYPE *Escape )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ __RPC__in LPCOLESTR pwszUnescapedValue,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrEscapedValue);
        
        HRESULT ( STDMETHODCALLTYPE *Unescape )( 
            __RPC__in IVsBuildManagerAccessor * This,
            /* [in] */ __RPC__in LPCOLESTR pwszEscapedValue,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUnescapedValue);
        
        END_INTERFACE
    } IVsBuildManagerAccessorVtbl;

    interface IVsBuildManagerAccessor
    {
        CONST_VTBL struct IVsBuildManagerAccessorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildManagerAccessor_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildManagerAccessor_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildManagerAccessor_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildManagerAccessor_RegisterLogger(This,submissionId,punkLogger)	\
    ( (This)->lpVtbl -> RegisterLogger(This,submissionId,punkLogger) ) 

#define IVsBuildManagerAccessor_UnregisterLoggers(This,submissionId)	\
    ( (This)->lpVtbl -> UnregisterLoggers(This,submissionId) ) 

#define IVsBuildManagerAccessor_ClaimUIThreadForBuild(This)	\
    ( (This)->lpVtbl -> ClaimUIThreadForBuild(This) ) 

#define IVsBuildManagerAccessor_ReleaseUIThreadForBuild(This)	\
    ( (This)->lpVtbl -> ReleaseUIThreadForBuild(This) ) 

#define IVsBuildManagerAccessor_BeginDesignTimeBuild(This)	\
    ( (This)->lpVtbl -> BeginDesignTimeBuild(This) ) 

#define IVsBuildManagerAccessor_EndDesignTimeBuild(This)	\
    ( (This)->lpVtbl -> EndDesignTimeBuild(This) ) 

#define IVsBuildManagerAccessor_GetCurrentBatchBuildId(This,pBatchId)	\
    ( (This)->lpVtbl -> GetCurrentBatchBuildId(This,pBatchId) ) 

#define IVsBuildManagerAccessor_GetSolutionConfiguration(This,punkRootProject,pbstrXmlFragment)	\
    ( (This)->lpVtbl -> GetSolutionConfiguration(This,punkRootProject,pbstrXmlFragment) ) 

#define IVsBuildManagerAccessor_Escape(This,pwszUnescapedValue,pbstrEscapedValue)	\
    ( (This)->lpVtbl -> Escape(This,pwszUnescapedValue,pbstrEscapedValue) ) 

#define IVsBuildManagerAccessor_Unescape(This,pwszEscapedValue,pbstrUnescapedValue)	\
    ( (This)->lpVtbl -> Unescape(This,pwszEscapedValue,pbstrUnescapedValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildManagerAccessor_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionBuildManager4_INTERFACE_DEFINED__
#define __IVsSolutionBuildManager4_INTERFACE_DEFINED__

/* interface IVsSolutionBuildManager4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionBuildManager4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C07342B-BA98-4235-983C-8638391A420A")
    IVsSolutionBuildManager4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateProjectDependencies( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionBuildManager4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionBuildManager4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionBuildManager4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionBuildManager4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateProjectDependencies )( 
            __RPC__in IVsSolutionBuildManager4 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHier);
        
        END_INTERFACE
    } IVsSolutionBuildManager4Vtbl;

    interface IVsSolutionBuildManager4
    {
        CONST_VTBL struct IVsSolutionBuildManager4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionBuildManager4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionBuildManager4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionBuildManager4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionBuildManager4_UpdateProjectDependencies(This,pHier)	\
    ( (This)->lpVtbl -> UpdateProjectDependencies(This,pHier) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionBuildManager4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0047 */
/* [local] */ 


enum __VsErrorType
    {
        VsErrorType_Error	= 1,
        VsErrorType_Warning	= 2
    } ;
typedef DWORD VSERRORTYPE;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0047_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0047_v0_0_s_ifspec;

#ifndef __IVsSolutionLogger_INTERFACE_DEFINED__
#define __IVsSolutionLogger_INTERFACE_DEFINED__

/* interface IVsSolutionLogger */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionLogger;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4CCB7A7A-BCEB-427C-84C5-FF0BD655311E")
    IVsSolutionLogger : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LogMessage( 
            __RPC__in LPCOLESTR pszMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogError( 
            VSERRORTYPE ErrorType,
            __RPC__in LPCOLESTR pszMessage,
            __RPC__in LPCOLESTR pszFile,
            long nLine,
            long nColumn,
            __RPC__in LPCOLESTR pszErrorCode,
            __RPC__in LPCOLESTR pszTaskListMessage,
            __RPC__in LPCOLESTR pszUniqueProjectName,
            __RPC__in LPCOLESTR pszHelpKeyword) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionLoggerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionLogger * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionLogger * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionLogger * This);
        
        HRESULT ( STDMETHODCALLTYPE *LogMessage )( 
            __RPC__in IVsSolutionLogger * This,
            __RPC__in LPCOLESTR pszMessage);
        
        HRESULT ( STDMETHODCALLTYPE *LogError )( 
            __RPC__in IVsSolutionLogger * This,
            VSERRORTYPE ErrorType,
            __RPC__in LPCOLESTR pszMessage,
            __RPC__in LPCOLESTR pszFile,
            long nLine,
            long nColumn,
            __RPC__in LPCOLESTR pszErrorCode,
            __RPC__in LPCOLESTR pszTaskListMessage,
            __RPC__in LPCOLESTR pszUniqueProjectName,
            __RPC__in LPCOLESTR pszHelpKeyword);
        
        END_INTERFACE
    } IVsSolutionLoggerVtbl;

    interface IVsSolutionLogger
    {
        CONST_VTBL struct IVsSolutionLoggerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionLogger_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionLogger_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionLogger_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionLogger_LogMessage(This,pszMessage)	\
    ( (This)->lpVtbl -> LogMessage(This,pszMessage) ) 

#define IVsSolutionLogger_LogError(This,ErrorType,pszMessage,pszFile,nLine,nColumn,pszErrorCode,pszTaskListMessage,pszUniqueProjectName,pszHelpKeyword)	\
    ( (This)->lpVtbl -> LogError(This,ErrorType,pszMessage,pszFile,nLine,nColumn,pszErrorCode,pszTaskListMessage,pszUniqueProjectName,pszHelpKeyword) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionLogger_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0048 */
/* [local] */ 

typedef DWORD VSCREATENEWPROJVIADLGEXFLAGS2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0048_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0048_v0_0_s_ifspec;

#ifndef __IVsToolbox5_INTERFACE_DEFINED__
#define __IVsToolbox5_INTERFACE_DEFINED__

/* interface IVsToolbox5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsToolbox5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("724B9AAF-F1D1-4AE8-923B-0F2469B1502A")
    IVsToolbox5 : public IUnknown
    {
    public:
        virtual /* [local] */ HRESULT STDMETHODCALLTYPE GetItemBitmap( 
            /* [in] */ IDataObject *pDataObject,
            /* [retval][out] */ HBITMAP *pBitmap) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItemFlags( 
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [retval][out] */ __RPC__out TBXITEMINFOFLAGS *pFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsItemEnabled( 
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ VARIANT_BOOL fForceEvaluation,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastRefreshTime( 
            /* [retval][out] */ __RPC__out DATE *pTime) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsToolbox5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsToolbox5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsToolbox5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsToolbox5 * This);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *GetItemBitmap )( 
            IVsToolbox5 * This,
            /* [in] */ IDataObject *pDataObject,
            /* [retval][out] */ HBITMAP *pBitmap);
        
        HRESULT ( STDMETHODCALLTYPE *GetItemFlags )( 
            __RPC__in IVsToolbox5 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [retval][out] */ __RPC__out TBXITEMINFOFLAGS *pFlags);
        
        HRESULT ( STDMETHODCALLTYPE *IsItemEnabled )( 
            __RPC__in IVsToolbox5 * This,
            /* [in] */ __RPC__in_opt IDataObject *pDataObject,
            /* [in] */ VARIANT_BOOL fForceEvaluation,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastRefreshTime )( 
            __RPC__in IVsToolbox5 * This,
            /* [retval][out] */ __RPC__out DATE *pTime);
        
        END_INTERFACE
    } IVsToolbox5Vtbl;

    interface IVsToolbox5
    {
        CONST_VTBL struct IVsToolbox5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsToolbox5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsToolbox5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsToolbox5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsToolbox5_GetItemBitmap(This,pDataObject,pBitmap)	\
    ( (This)->lpVtbl -> GetItemBitmap(This,pDataObject,pBitmap) ) 

#define IVsToolbox5_GetItemFlags(This,pDataObject,pFlags)	\
    ( (This)->lpVtbl -> GetItemFlags(This,pDataObject,pFlags) ) 

#define IVsToolbox5_IsItemEnabled(This,pDataObject,fForceEvaluation,pEnabled)	\
    ( (This)->lpVtbl -> IsItemEnabled(This,pDataObject,fForceEvaluation,pEnabled) ) 

#define IVsToolbox5_GetLastRefreshTime(This,pTime)	\
    ( (This)->lpVtbl -> GetLastRefreshTime(This,pTime) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsToolbox5_INTERFACE_DEFINED__ */


#ifndef __IVsProfferCommands4_INTERFACE_DEFINED__
#define __IVsProfferCommands4_INTERFACE_DEFINED__

/* interface IVsProfferCommands4 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProfferCommands4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3edbd520-6748-43a4-8be4-cadab7472cf6")
    IVsProfferCommands4 : public IVsProfferCommands3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddNamedCommand3( 
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdTooltip,
            /* [in] */ __RPC__in_opt IUnknown *punkImage,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProfferCommands4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProfferCommands4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProfferCommands4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveNamedCommand )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonical);
        
        HRESULT ( STDMETHODCALLTYPE *RenameNamedCommand )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonical,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonicalNew,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameLocalizedNew);
        
        HRESULT ( STDMETHODCALLTYPE *AddCommandBarControl )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonical,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [in] */ DWORD dwCmdType,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBarCtrl);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCommandBarControl )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarCtrl);
        
        HRESULT ( STDMETHODCALLTYPE *AddCommandBar )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdBarName,
            /* [in] */ DWORD dwType,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBarParent,
            /* [in] */ DWORD dwIndex,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCommandBar )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in_opt IDispatch *pCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *FindCommandBar )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in_opt IUnknown *pToolbarSet,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [in] */ DWORD dwMenuId,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppdispCmdBar);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand2 )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszCmdTooltip,
            /* [string][in] */ __RPC__in_string const LPCOLESTR pszSatelliteDLL,
            /* [in] */ DWORD dwBitmapResourceId,
            /* [in] */ DWORD dwBitmapImageIndex,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType);
        
        HRESULT ( STDMETHODCALLTYPE *AddNamedCommand3 )( 
            __RPC__in IVsProfferCommands4 * This,
            /* [in] */ __RPC__in const GUID *pguidPackage,
            /* [in] */ __RPC__in const GUID *pguidCmdGroup,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdNameCanonical,
            /* [out] */ __RPC__out DWORD *pdwCmdId,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdNameLocalized,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszBtnText,
            /* [string][in] */ __RPC__in_string LPCOLESTR pszCmdTooltip,
            /* [in] */ __RPC__in_opt IUnknown *punkImage,
            /* [in] */ DWORD dwCmdFlagsDefault,
            /* [in] */ DWORD cUIContexts,
            /* [size_is][in] */ __RPC__in_ecount_full(cUIContexts) const GUID *rgguidUIContexts,
            /* [in] */ DWORD dwUIElementType);
        
        END_INTERFACE
    } IVsProfferCommands4Vtbl;

    interface IVsProfferCommands4
    {
        CONST_VTBL struct IVsProfferCommands4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProfferCommands4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProfferCommands4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProfferCommands4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProfferCommands4_AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts)	\
    ( (This)->lpVtbl -> AddNamedCommand(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts) ) 

#define IVsProfferCommands4_RemoveNamedCommand(This,pszCmdNameCanonical)	\
    ( (This)->lpVtbl -> RemoveNamedCommand(This,pszCmdNameCanonical) ) 

#define IVsProfferCommands4_RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew)	\
    ( (This)->lpVtbl -> RenameNamedCommand(This,pszCmdNameCanonical,pszCmdNameCanonicalNew,pszCmdNameLocalizedNew) ) 

#define IVsProfferCommands4_AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl)	\
    ( (This)->lpVtbl -> AddCommandBarControl(This,pszCmdNameCanonical,pCmdBarParent,dwIndex,dwCmdType,ppCmdBarCtrl) ) 

#define IVsProfferCommands4_RemoveCommandBarControl(This,pCmdBarCtrl)	\
    ( (This)->lpVtbl -> RemoveCommandBarControl(This,pCmdBarCtrl) ) 

#define IVsProfferCommands4_AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar)	\
    ( (This)->lpVtbl -> AddCommandBar(This,pszCmdBarName,dwType,pCmdBarParent,dwIndex,ppCmdBar) ) 

#define IVsProfferCommands4_RemoveCommandBar(This,pCmdBar)	\
    ( (This)->lpVtbl -> RemoveCommandBar(This,pCmdBar) ) 

#define IVsProfferCommands4_FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar)	\
    ( (This)->lpVtbl -> FindCommandBar(This,pToolbarSet,pguidCmdGroup,dwMenuId,ppdispCmdBar) ) 

#define IVsProfferCommands4_AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType)	\
    ( (This)->lpVtbl -> AddNamedCommand2(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,pszSatelliteDLL,dwBitmapResourceId,dwBitmapImageIndex,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType) ) 


#define IVsProfferCommands4_AddNamedCommand3(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,punkImage,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType)	\
    ( (This)->lpVtbl -> AddNamedCommand3(This,pguidPackage,pguidCmdGroup,pszCmdNameCanonical,pdwCmdId,pszCmdNameLocalized,pszBtnText,pszCmdTooltip,punkImage,dwCmdFlagsDefault,cUIContexts,rgguidUIContexts,dwUIElementType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProfferCommands4_INTERFACE_DEFINED__ */


#ifndef __SVsProfferCommands_INTERFACE_DEFINED__
#define __SVsProfferCommands_INTERFACE_DEFINED__

/* interface SVsProfferCommands */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsProfferCommands;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8cc0cde1-c16a-4749-99af-6f7523c34a57")
    SVsProfferCommands : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsProfferCommandsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsProfferCommands * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsProfferCommands * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsProfferCommands * This);
        
        END_INTERFACE
    } SVsProfferCommandsVtbl;

    interface SVsProfferCommands
    {
        CONST_VTBL struct SVsProfferCommandsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsProfferCommands_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsProfferCommands_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsProfferCommands_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsProfferCommands_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0051 */
/* [local] */ 


enum __VSADDITEMFLAGS3
    {
        VSADDITEM_NoOnlineTemplates	= 0x1000000
    } ;
typedef DWORD VSADDITEMFLAGS3;


enum __VSCREATENEWPROJVIADLGEXFLAGS2
    {
        VNPVDE_NOONLINETEMPLATES	= 0x20,
        VNPVDE_NORECENTTEMPLATES	= 0x40,
        VNPVDE_NOFRAMEWORKSELECTION	= 0x80,
        VNPVDE_HIDESOLUTIONNAME	= 0x100
    } ;

enum __FILTERKEYSMESSAGES
    {
        FilterKeysMessage_GotFocus	= 0x7,
        FilterKeysMessage_LostFocus	= 0x8,
        FilterKeysMessage_SysKeyDown	= 0x104,
        FilterKeysMessage_KeyDown	= 0x100,
        FilterKeysMessage_Character	= 0x102,
        FilterKeysMessage_DragDrop	= 0x70c,
        FilterKeysMessage_TextChanged	= 0x5
    } ;
typedef DWORD FilterKeysMessages;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0051_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0051_v0_0_s_ifspec;

#ifndef __IVsHelpProvider_INTERFACE_DEFINED__
#define __IVsHelpProvider_INTERFACE_DEFINED__

/* interface IVsHelpProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHelpProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A0B29E84-6CFD-490d-9A17-B0BF64D58C55")
    IVsHelpProvider : public IUnknown
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE DisplayTopic( 
            /* [in] */ __RPC__in SAFEARRAY * Keywords,
            /* [in] */ __RPC__in SAFEARRAY * Attributes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHelpProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHelpProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHelpProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHelpProvider * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopic )( 
            __RPC__in IVsHelpProvider * This,
            /* [in] */ __RPC__in SAFEARRAY * Keywords,
            /* [in] */ __RPC__in SAFEARRAY * Attributes);
        
        END_INTERFACE
    } IVsHelpProviderVtbl;

    interface IVsHelpProvider
    {
        CONST_VTBL struct IVsHelpProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpProvider_DisplayTopic(This,Keywords,Attributes)	\
    ( (This)->lpVtbl -> DisplayTopic(This,Keywords,Attributes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0052 */
/* [local] */ 


enum __ProjectReferenceOutputValidity
    {
        PROV_DoDefaultCheck	= 0,
        PROV_OutputValid	= 1,
        PROV_OutputInvalid	= 2
    } ;
typedef DWORD ProjectReferenceOutputValidity;



extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0052_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0052_v0_0_s_ifspec;

#ifndef __IVsProjectFlavorReferences2_INTERFACE_DEFINED__
#define __IVsProjectFlavorReferences2_INTERFACE_DEFINED__

/* interface IVsProjectFlavorReferences2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFlavorReferences2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("54BE5D82-5886-4EF1-97C0-81716586FB8A")
    IVsProjectFlavorReferences2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryCheckIsProjectReferenceOutputValid( 
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [in] */ __RPC__in LPCOLESTR pszReferencedProjectOutput,
            /* [retval][out] */ __RPC__out ProjectReferenceOutputValidity *pOutputValidity) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFlavorReferences2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFlavorReferences2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFlavorReferences2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFlavorReferences2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryCheckIsProjectReferenceOutputValid )( 
            __RPC__in IVsProjectFlavorReferences2 * This,
            /* [in] */ __RPC__in_opt IUnknown *pReferencedProject,
            /* [in] */ __RPC__in LPCOLESTR pszReferencedProjectOutput,
            /* [retval][out] */ __RPC__out ProjectReferenceOutputValidity *pOutputValidity);
        
        END_INTERFACE
    } IVsProjectFlavorReferences2Vtbl;

    interface IVsProjectFlavorReferences2
    {
        CONST_VTBL struct IVsProjectFlavorReferences2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFlavorReferences2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFlavorReferences2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFlavorReferences2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFlavorReferences2_QueryCheckIsProjectReferenceOutputValid(This,pReferencedProject,pszReferencedProjectOutput,pOutputValidity)	\
    ( (This)->lpVtbl -> QueryCheckIsProjectReferenceOutputValid(This,pReferencedProject,pszReferencedProjectOutput,pOutputValidity) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFlavorReferences2_INTERFACE_DEFINED__ */


#ifndef __SVsCommonMessagePumpFactory_INTERFACE_DEFINED__
#define __SVsCommonMessagePumpFactory_INTERFACE_DEFINED__

/* interface SVsCommonMessagePumpFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsCommonMessagePumpFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E269CBA1-C9FB-45a8-91CA-EC8D3474FDD2")
    SVsCommonMessagePumpFactory : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsCommonMessagePumpFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsCommonMessagePumpFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsCommonMessagePumpFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsCommonMessagePumpFactory * This);
        
        END_INTERFACE
    } SVsCommonMessagePumpFactoryVtbl;

    interface SVsCommonMessagePumpFactory
    {
        CONST_VTBL struct SVsCommonMessagePumpFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsCommonMessagePumpFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsCommonMessagePumpFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsCommonMessagePumpFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsCommonMessagePumpFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0054 */
/* [local] */ 

#define SID_SVsCommonMessagePumpFactory IID_SVsCommonMessagePumpFactory




extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0054_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0054_v0_0_s_ifspec;

#ifndef __IVsCommonMessagePumpFactory_INTERFACE_DEFINED__
#define __IVsCommonMessagePumpFactory_INTERFACE_DEFINED__

/* interface IVsCommonMessagePumpFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCommonMessagePumpFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E8E8300A-C63F-423F-B596-29308CA0AB4C")
    IVsCommonMessagePumpFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateInstance( 
            /* [out] */ __RPC__deref_out_opt IVsCommonMessagePump **ppIVsCommonMessagePump) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCommonMessagePumpFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsCommonMessagePumpFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsCommonMessagePumpFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsCommonMessagePumpFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateInstance )( 
            __RPC__in IVsCommonMessagePumpFactory * This,
            /* [out] */ __RPC__deref_out_opt IVsCommonMessagePump **ppIVsCommonMessagePump);
        
        END_INTERFACE
    } IVsCommonMessagePumpFactoryVtbl;

    interface IVsCommonMessagePumpFactory
    {
        CONST_VTBL struct IVsCommonMessagePumpFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommonMessagePumpFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommonMessagePumpFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommonMessagePumpFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommonMessagePumpFactory_CreateInstance(This,ppIVsCommonMessagePump)	\
    ( (This)->lpVtbl -> CreateInstance(This,ppIVsCommonMessagePump) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommonMessagePumpFactory_INTERFACE_DEFINED__ */


#ifndef __IVsCommonMessagePump_INTERFACE_DEFINED__
#define __IVsCommonMessagePump_INTERFACE_DEFINED__

/* interface IVsCommonMessagePump */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCommonMessagePump;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FB3B20F4-9C8E-454a-984B-B1334F790541")
    IVsCommonMessagePump : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ModalWaitForObjects( 
            /* [size_is][in] */ HANDLE rgHandles[  ],
            /* [in] */ UINT cHandles,
            /* [out] */ DWORD *pdwWaitResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ModalWaitForObjectsWithClient( 
            /* [size_is][in] */ HANDLE rgHandles[  ],
            /* [in] */ UINT cHandles,
            /* [in] */ IVsCommonMessagePumpClientEvents *pClient) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTimeout( 
            /* [in] */ DWORD dwTimeoutInMilliseconds) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetAllowCancel( 
            /* [in] */ VARIANT_BOOL fAllowCancel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetWaitText( 
            /* [in] */ LPCOLESTR pszWaitText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetWaitTitle( 
            /* [in] */ LPCOLESTR pszWaitTitle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStatusBarText( 
            /* [in] */ LPCOLESTR pszStatusBarText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnableRealProgress( 
            /* [in] */ VARIANT_BOOL fEnableRealProgress) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProgressInfo( 
            /* [in] */ long iTotalSteps,
            /* [in] */ long iCurrentStep,
            /* [in] */ LPCOLESTR pszProgressText) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCommonMessagePumpVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCommonMessagePump * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCommonMessagePump * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCommonMessagePump * This);
        
        HRESULT ( STDMETHODCALLTYPE *ModalWaitForObjects )( 
            IVsCommonMessagePump * This,
            /* [size_is][in] */ HANDLE rgHandles[  ],
            /* [in] */ UINT cHandles,
            /* [out] */ DWORD *pdwWaitResult);
        
        HRESULT ( STDMETHODCALLTYPE *ModalWaitForObjectsWithClient )( 
            IVsCommonMessagePump * This,
            /* [size_is][in] */ HANDLE rgHandles[  ],
            /* [in] */ UINT cHandles,
            /* [in] */ IVsCommonMessagePumpClientEvents *pClient);
        
        HRESULT ( STDMETHODCALLTYPE *SetTimeout )( 
            IVsCommonMessagePump * This,
            /* [in] */ DWORD dwTimeoutInMilliseconds);
        
        HRESULT ( STDMETHODCALLTYPE *SetAllowCancel )( 
            IVsCommonMessagePump * This,
            /* [in] */ VARIANT_BOOL fAllowCancel);
        
        HRESULT ( STDMETHODCALLTYPE *SetWaitText )( 
            IVsCommonMessagePump * This,
            /* [in] */ LPCOLESTR pszWaitText);
        
        HRESULT ( STDMETHODCALLTYPE *SetWaitTitle )( 
            IVsCommonMessagePump * This,
            /* [in] */ LPCOLESTR pszWaitTitle);
        
        HRESULT ( STDMETHODCALLTYPE *SetStatusBarText )( 
            IVsCommonMessagePump * This,
            /* [in] */ LPCOLESTR pszStatusBarText);
        
        HRESULT ( STDMETHODCALLTYPE *EnableRealProgress )( 
            IVsCommonMessagePump * This,
            /* [in] */ VARIANT_BOOL fEnableRealProgress);
        
        HRESULT ( STDMETHODCALLTYPE *SetProgressInfo )( 
            IVsCommonMessagePump * This,
            /* [in] */ long iTotalSteps,
            /* [in] */ long iCurrentStep,
            /* [in] */ LPCOLESTR pszProgressText);
        
        END_INTERFACE
    } IVsCommonMessagePumpVtbl;

    interface IVsCommonMessagePump
    {
        CONST_VTBL struct IVsCommonMessagePumpVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommonMessagePump_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommonMessagePump_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommonMessagePump_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommonMessagePump_ModalWaitForObjects(This,rgHandles,cHandles,pdwWaitResult)	\
    ( (This)->lpVtbl -> ModalWaitForObjects(This,rgHandles,cHandles,pdwWaitResult) ) 

#define IVsCommonMessagePump_ModalWaitForObjectsWithClient(This,rgHandles,cHandles,pClient)	\
    ( (This)->lpVtbl -> ModalWaitForObjectsWithClient(This,rgHandles,cHandles,pClient) ) 

#define IVsCommonMessagePump_SetTimeout(This,dwTimeoutInMilliseconds)	\
    ( (This)->lpVtbl -> SetTimeout(This,dwTimeoutInMilliseconds) ) 

#define IVsCommonMessagePump_SetAllowCancel(This,fAllowCancel)	\
    ( (This)->lpVtbl -> SetAllowCancel(This,fAllowCancel) ) 

#define IVsCommonMessagePump_SetWaitText(This,pszWaitText)	\
    ( (This)->lpVtbl -> SetWaitText(This,pszWaitText) ) 

#define IVsCommonMessagePump_SetWaitTitle(This,pszWaitTitle)	\
    ( (This)->lpVtbl -> SetWaitTitle(This,pszWaitTitle) ) 

#define IVsCommonMessagePump_SetStatusBarText(This,pszStatusBarText)	\
    ( (This)->lpVtbl -> SetStatusBarText(This,pszStatusBarText) ) 

#define IVsCommonMessagePump_EnableRealProgress(This,fEnableRealProgress)	\
    ( (This)->lpVtbl -> EnableRealProgress(This,fEnableRealProgress) ) 

#define IVsCommonMessagePump_SetProgressInfo(This,iTotalSteps,iCurrentStep,pszProgressText)	\
    ( (This)->lpVtbl -> SetProgressInfo(This,iTotalSteps,iCurrentStep,pszProgressText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommonMessagePump_INTERFACE_DEFINED__ */


#ifndef __IVsCommonMessagePumpClientEvents_INTERFACE_DEFINED__
#define __IVsCommonMessagePumpClientEvents_INTERFACE_DEFINED__

/* interface IVsCommonMessagePumpClientEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCommonMessagePumpClientEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9C6D9104-7DB9-4abd-841D-F0CFD24DE3D0")
    IVsCommonMessagePumpClientEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnHandleSignaled( 
            /* [in] */ UINT nHandle,
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnTimeout( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterMessageProcessed( 
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsCommonMessagePumpClientEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnHandleSignaled )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This,
            /* [in] */ UINT nHandle,
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnTimeout )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterMessageProcessed )( 
            __RPC__in IVsCommonMessagePumpClientEvents * This,
            /* [out] */ __RPC__out VARIANT_BOOL *pfContinue);
        
        END_INTERFACE
    } IVsCommonMessagePumpClientEventsVtbl;

    interface IVsCommonMessagePumpClientEvents
    {
        CONST_VTBL struct IVsCommonMessagePumpClientEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCommonMessagePumpClientEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCommonMessagePumpClientEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCommonMessagePumpClientEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCommonMessagePumpClientEvents_OnHandleSignaled(This,nHandle,pfContinue)	\
    ( (This)->lpVtbl -> OnHandleSignaled(This,nHandle,pfContinue) ) 

#define IVsCommonMessagePumpClientEvents_OnTimeout(This,pfContinue)	\
    ( (This)->lpVtbl -> OnTimeout(This,pfContinue) ) 

#define IVsCommonMessagePumpClientEvents_OnAfterMessageProcessed(This,pfContinue)	\
    ( (This)->lpVtbl -> OnAfterMessageProcessed(This,pfContinue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCommonMessagePumpClientEvents_INTERFACE_DEFINED__ */


#ifndef __IVsHandleInComingCallDynamicInProc_INTERFACE_DEFINED__
#define __IVsHandleInComingCallDynamicInProc_INTERFACE_DEFINED__

/* interface IVsHandleInComingCallDynamicInProc */
/* [object][unique][local][version][uuid] */ 


EXTERN_C const IID IID_IVsHandleInComingCallDynamicInProc;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ADB942C5-1637-4D76-8635-7A9376F20699")
    IVsHandleInComingCallDynamicInProc : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AllowIncomingCall( 
            /* [in] */ DWORD dwCallType,
            /* [in] */ HTASK htaskCaller,
            /* [in] */ DWORD dwTickCount,
            /* [in] */ REFIID iid,
            /* [in] */ WORD wMethod,
            /* [retval][out] */ VARIANT_BOOL *pfAllow) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHandleInComingCallDynamicInProcVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHandleInComingCallDynamicInProc * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHandleInComingCallDynamicInProc * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHandleInComingCallDynamicInProc * This);
        
        HRESULT ( STDMETHODCALLTYPE *AllowIncomingCall )( 
            IVsHandleInComingCallDynamicInProc * This,
            /* [in] */ DWORD dwCallType,
            /* [in] */ HTASK htaskCaller,
            /* [in] */ DWORD dwTickCount,
            /* [in] */ REFIID iid,
            /* [in] */ WORD wMethod,
            /* [retval][out] */ VARIANT_BOOL *pfAllow);
        
        END_INTERFACE
    } IVsHandleInComingCallDynamicInProcVtbl;

    interface IVsHandleInComingCallDynamicInProc
    {
        CONST_VTBL struct IVsHandleInComingCallDynamicInProcVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHandleInComingCallDynamicInProc_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHandleInComingCallDynamicInProc_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHandleInComingCallDynamicInProc_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHandleInComingCallDynamicInProc_AllowIncomingCall(This,dwCallType,htaskCaller,dwTickCount,iid,wMethod,pfAllow)	\
    ( (This)->lpVtbl -> AllowIncomingCall(This,dwCallType,htaskCaller,dwTickCount,iid,wMethod,pfAllow) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHandleInComingCallDynamicInProc_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell100_0000_0058 */
/* [local] */ 

#define VS_E_INCOMPATIBLEPROJECT    MAKE_HRESULT(SEVERITY_ERROR, FACILITY_ITF, 0x2003)


extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0058_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell100_0000_0058_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  CLIPFORMAT_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in CLIPFORMAT * ); 
unsigned char * __RPC_USER  CLIPFORMAT_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in CLIPFORMAT * ); 
unsigned char * __RPC_USER  CLIPFORMAT_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out CLIPFORMAT * ); 
void                      __RPC_USER  CLIPFORMAT_UserFree(     __RPC__in unsigned long *, __RPC__in CLIPFORMAT * ); 

unsigned long             __RPC_USER  HBITMAP_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in HBITMAP * ); 
unsigned char * __RPC_USER  HBITMAP_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in HBITMAP * ); 
unsigned char * __RPC_USER  HBITMAP_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out HBITMAP * ); 
void                      __RPC_USER  HBITMAP_UserFree(     __RPC__in unsigned long *, __RPC__in HBITMAP * ); 

unsigned long             __RPC_USER  HGLOBAL_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in HGLOBAL * ); 
unsigned char * __RPC_USER  HGLOBAL_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in HGLOBAL * ); 
unsigned char * __RPC_USER  HGLOBAL_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out HGLOBAL * ); 
void                      __RPC_USER  HGLOBAL_UserFree(     __RPC__in unsigned long *, __RPC__in HGLOBAL * ); 

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


