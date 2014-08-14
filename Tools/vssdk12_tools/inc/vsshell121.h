

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

#ifndef __vsshell121_h__
#define __vsshell121_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __SharedMSBuildFilesManagerHierarchy_FWD_DEFINED__
#define __SharedMSBuildFilesManagerHierarchy_FWD_DEFINED__
typedef interface SharedMSBuildFilesManagerHierarchy SharedMSBuildFilesManagerHierarchy;

#endif 	/* __SharedMSBuildFilesManagerHierarchy_FWD_DEFINED__ */


#ifndef __IVsSolution6_FWD_DEFINED__
#define __IVsSolution6_FWD_DEFINED__
typedef interface IVsSolution6 IVsSolution6;

#endif 	/* __IVsSolution6_FWD_DEFINED__ */


#ifndef __IVsSolutionEvents6_FWD_DEFINED__
#define __IVsSolutionEvents6_FWD_DEFINED__
typedef interface IVsSolutionEvents6 IVsSolutionEvents6;

#endif 	/* __IVsSolutionEvents6_FWD_DEFINED__ */


#ifndef __IVsProjectFileReloadManagerEvents_FWD_DEFINED__
#define __IVsProjectFileReloadManagerEvents_FWD_DEFINED__
typedef interface IVsProjectFileReloadManagerEvents IVsProjectFileReloadManagerEvents;

#endif 	/* __IVsProjectFileReloadManagerEvents_FWD_DEFINED__ */


#ifndef __IVsEnumHierarchies_FWD_DEFINED__
#define __IVsEnumHierarchies_FWD_DEFINED__
typedef interface IVsEnumHierarchies IVsEnumHierarchies;

#endif 	/* __IVsEnumHierarchies_FWD_DEFINED__ */


#ifndef __IVsSharedAssetsProjectEvents_FWD_DEFINED__
#define __IVsSharedAssetsProjectEvents_FWD_DEFINED__
typedef interface IVsSharedAssetsProjectEvents IVsSharedAssetsProjectEvents;

#endif 	/* __IVsSharedAssetsProjectEvents_FWD_DEFINED__ */


#ifndef __IVsSharedAssetsProject_FWD_DEFINED__
#define __IVsSharedAssetsProject_FWD_DEFINED__
typedef interface IVsSharedAssetsProject IVsSharedAssetsProject;

#endif 	/* __IVsSharedAssetsProject_FWD_DEFINED__ */


#ifndef __IVsBuildPropertyStorageEvents_FWD_DEFINED__
#define __IVsBuildPropertyStorageEvents_FWD_DEFINED__
typedef interface IVsBuildPropertyStorageEvents IVsBuildPropertyStorageEvents;

#endif 	/* __IVsBuildPropertyStorageEvents_FWD_DEFINED__ */


#ifndef __IVsBuildPropertyStorage3_FWD_DEFINED__
#define __IVsBuildPropertyStorage3_FWD_DEFINED__
typedef interface IVsBuildPropertyStorage3 IVsBuildPropertyStorage3;

#endif 	/* __IVsBuildPropertyStorage3_FWD_DEFINED__ */


#ifndef __IVsProject5_FWD_DEFINED__
#define __IVsProject5_FWD_DEFINED__
typedef interface IVsProject5 IVsProject5;

#endif 	/* __IVsProject5_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "vsshell.h"
#include "vsshell2.h"
#include "vsshell80.h"
#include "vsshell90.h"
#include "vsshell100.h"
#include "vsshell110.h"
#include "vsshell120.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsshell121_0000_0000 */
/* [local] */ 

#pragma once

enum _VSQuickCheckAnswer
    {
        QCA_Always	= 2,
        QCA_Yes	= 1,
        QCA_QuickAnswerNA	= 0,
        QCA_No	= -1,
        QCA_Never	= -2
    } ;
typedef LONG VSQuickCheckAnswer;


enum __VSHPROPID7
    {
        VSHPROPID_IsSharedItem	= -2142,
        VSHPROPID_SharedItemContextHierarchy	= -2143,
        VSHPROPID_ShortSubcaption	= -2144,
        VSHPROPID_SharedItemsImportFullPaths	= -2145,
        VSHPROPID_ProjectTreeCapabilities	= -2146,
        VSHPROPID_DeploymentRelativePath	= -2147,
        VSHPROPID_IsSharedFolder	= -2148,
        VSHPROPID_OneAppCapabilities	= -2149,
        VSHPROPID_MSBuildImportsStorage	= -2150,
        VSHPROPID_SharedProjectHierarchy	= -2152,
        VSHPROPID_SharedAssetsProject	= -2153,
        VSHPROPID_IsSharedItemsImportFile	= -2154,
        VSHPROPID_ExcludeFromMoveFileToProjectUI	= -2155,
        VSHPROPID_CanBuildQuickCheck	= -2156,
        VSHPROPID_CanDebugLaunchQuickCheck	= -2157,
        VSHPROPID_CanDeployQuickCheck	= -2158,
        VSHPROPID_FIRST7	= -2158
    } ;
typedef /* [public] */ DWORD VSHPROPID7;



extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0000_v0_0_s_ifspec;

#ifndef __SharedMSBuildFilesManagerHierarchy_INTERFACE_DEFINED__
#define __SharedMSBuildFilesManagerHierarchy_INTERFACE_DEFINED__

/* interface SharedMSBuildFilesManagerHierarchy */
/* [object][uuid] */ 


EXTERN_C const IID IID_SharedMSBuildFilesManagerHierarchy;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("22912BB2-3FF9-4D55-B4DB-D210B6035D4C")
    SharedMSBuildFilesManagerHierarchy : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SharedMSBuildFilesManagerHierarchyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SharedMSBuildFilesManagerHierarchy * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SharedMSBuildFilesManagerHierarchy * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SharedMSBuildFilesManagerHierarchy * This);
        
        END_INTERFACE
    } SharedMSBuildFilesManagerHierarchyVtbl;

    interface SharedMSBuildFilesManagerHierarchy
    {
        CONST_VTBL struct SharedMSBuildFilesManagerHierarchyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SharedMSBuildFilesManagerHierarchy_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SharedMSBuildFilesManagerHierarchy_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SharedMSBuildFilesManagerHierarchy_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SharedMSBuildFilesManagerHierarchy_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell121_0000_0001 */
/* [local] */ 

#define PROJECTID_SharedMSBuildFilesManagerHierarchy IID_SharedMSBuildFilesManagerHierarchy
#define UICONTEXT_SharedMSBuildFilesManagerHierarchyLoaded PROJECTID_SharedMSBuildFilesManagerHierarchy

enum __VSADDVPFLAGS2
    {
        ADDVP_ReloadOnProjectFileChanged	= 0x80
    } ;
typedef DWORD VSADDVPFLAGS2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0001_v0_0_s_ifspec;

#ifndef __IVsSolution6_INTERFACE_DEFINED__
#define __IVsSolution6_INTERFACE_DEFINED__

/* interface IVsSolution6 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolution6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96CB263F-EB15-4F70-B735-AD5AD7F6D363")
    IVsSolution6 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetProjectParent( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pProject,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddNewProjectFromTemplate( 
            /* [in] */ __RPC__in LPCOLESTR szTemplatePath,
            /* [in] */ __RPC__in SAFEARRAY * rgCustomParams,
            /* [in] */ __RPC__in LPCOLESTR szTargetFramework,
            /* [in] */ __RPC__in LPCOLESTR szDestinationFolder,
            /* [in] */ __RPC__in LPCOLESTR szProjectName,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppNewProj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddExistingProject( 
            /* [in] */ __RPC__in LPCOLESTR szFullPath,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppNewProj) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseForExistingProject( 
            /* [in] */ __RPC__in LPCOLESTR szDialogTitle,
            /* [in] */ __RPC__in LPCOLESTR szStartupLocation,
            /* [in] */ GUID preferedProjectType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSelected) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolution6Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolution6 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolution6 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolution6 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetProjectParent )( 
            __RPC__in IVsSolution6 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pProject,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent);
        
        HRESULT ( STDMETHODCALLTYPE *AddNewProjectFromTemplate )( 
            __RPC__in IVsSolution6 * This,
            /* [in] */ __RPC__in LPCOLESTR szTemplatePath,
            /* [in] */ __RPC__in SAFEARRAY * rgCustomParams,
            /* [in] */ __RPC__in LPCOLESTR szTargetFramework,
            /* [in] */ __RPC__in LPCOLESTR szDestinationFolder,
            /* [in] */ __RPC__in LPCOLESTR szProjectName,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppNewProj);
        
        HRESULT ( STDMETHODCALLTYPE *AddExistingProject )( 
            __RPC__in IVsSolution6 * This,
            /* [in] */ __RPC__in LPCOLESTR szFullPath,
            /* [in] */ __RPC__in_opt IVsHierarchy *pParent,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **ppNewProj);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseForExistingProject )( 
            __RPC__in IVsSolution6 * This,
            /* [in] */ __RPC__in LPCOLESTR szDialogTitle,
            /* [in] */ __RPC__in LPCOLESTR szStartupLocation,
            /* [in] */ GUID preferedProjectType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSelected);
        
        END_INTERFACE
    } IVsSolution6Vtbl;

    interface IVsSolution6
    {
        CONST_VTBL struct IVsSolution6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolution6_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolution6_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolution6_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolution6_SetProjectParent(This,pProject,pParent)	\
    ( (This)->lpVtbl -> SetProjectParent(This,pProject,pParent) ) 

#define IVsSolution6_AddNewProjectFromTemplate(This,szTemplatePath,rgCustomParams,szTargetFramework,szDestinationFolder,szProjectName,pParent,ppNewProj)	\
    ( (This)->lpVtbl -> AddNewProjectFromTemplate(This,szTemplatePath,rgCustomParams,szTargetFramework,szDestinationFolder,szProjectName,pParent,ppNewProj) ) 

#define IVsSolution6_AddExistingProject(This,szFullPath,pParent,ppNewProj)	\
    ( (This)->lpVtbl -> AddExistingProject(This,szFullPath,pParent,ppNewProj) ) 

#define IVsSolution6_BrowseForExistingProject(This,szDialogTitle,szStartupLocation,preferedProjectType,pbstrSelected)	\
    ( (This)->lpVtbl -> BrowseForExistingProject(This,szDialogTitle,szStartupLocation,preferedProjectType,pbstrSelected) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolution6_INTERFACE_DEFINED__ */


#ifndef __IVsSolutionEvents6_INTERFACE_DEFINED__
#define __IVsSolutionEvents6_INTERFACE_DEFINED__

/* interface IVsSolutionEvents6 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionEvents6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9AD84AB1-5C4E-4084-B161-21B6696237CB")
    IVsSolutionEvents6 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeforeProjectRegisteredInRunningDocumentTable( 
            /* [in] */ GUID projectID,
            /* [in] */ __RPC__in LPCOLESTR projectFullPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterProjectRegisteredInRunningDocumentTable( 
            /* [in] */ GUID projectID,
            /* [in] */ __RPC__in LPCOLESTR projectFullPath,
            /* [in] */ VSCOOKIE docCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionEvents6Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionEvents6 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionEvents6 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionEvents6 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeforeProjectRegisteredInRunningDocumentTable )( 
            __RPC__in IVsSolutionEvents6 * This,
            /* [in] */ GUID projectID,
            /* [in] */ __RPC__in LPCOLESTR projectFullPath);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterProjectRegisteredInRunningDocumentTable )( 
            __RPC__in IVsSolutionEvents6 * This,
            /* [in] */ GUID projectID,
            /* [in] */ __RPC__in LPCOLESTR projectFullPath,
            /* [in] */ VSCOOKIE docCookie);
        
        END_INTERFACE
    } IVsSolutionEvents6Vtbl;

    interface IVsSolutionEvents6
    {
        CONST_VTBL struct IVsSolutionEvents6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionEvents6_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionEvents6_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionEvents6_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionEvents6_OnBeforeProjectRegisteredInRunningDocumentTable(This,projectID,projectFullPath)	\
    ( (This)->lpVtbl -> OnBeforeProjectRegisteredInRunningDocumentTable(This,projectID,projectFullPath) ) 

#define IVsSolutionEvents6_OnAfterProjectRegisteredInRunningDocumentTable(This,projectID,projectFullPath,docCookie)	\
    ( (This)->lpVtbl -> OnAfterProjectRegisteredInRunningDocumentTable(This,projectID,projectFullPath,docCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionEvents6_INTERFACE_DEFINED__ */


#ifndef __IVsProjectFileReloadManagerEvents_INTERFACE_DEFINED__
#define __IVsProjectFileReloadManagerEvents_INTERFACE_DEFINED__

/* interface IVsProjectFileReloadManagerEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFileReloadManagerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2AE1E600-A58A-4A31-A534-AFCB7200542C")
    IVsProjectFileReloadManagerEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnQueryAdditionalFilesToBeClosedBeforeProjectsReloaded( 
            /* [in] */ int cProjectsToBeReloaded,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjectsToBeReloaded) GUID rgProjectsToBeReloaded[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgsaAdditionalFilesToBeClosed) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFileReloadManagerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFileReloadManagerEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFileReloadManagerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFileReloadManagerEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAdditionalFilesToBeClosedBeforeProjectsReloaded )( 
            __RPC__in IVsProjectFileReloadManagerEvents * This,
            /* [in] */ int cProjectsToBeReloaded,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjectsToBeReloaded) GUID rgProjectsToBeReloaded[  ],
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *rgsaAdditionalFilesToBeClosed);
        
        END_INTERFACE
    } IVsProjectFileReloadManagerEventsVtbl;

    interface IVsProjectFileReloadManagerEvents
    {
        CONST_VTBL struct IVsProjectFileReloadManagerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFileReloadManagerEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFileReloadManagerEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFileReloadManagerEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFileReloadManagerEvents_OnQueryAdditionalFilesToBeClosedBeforeProjectsReloaded(This,cProjectsToBeReloaded,rgProjectsToBeReloaded,rgsaAdditionalFilesToBeClosed)	\
    ( (This)->lpVtbl -> OnQueryAdditionalFilesToBeClosedBeforeProjectsReloaded(This,cProjectsToBeReloaded,rgProjectsToBeReloaded,rgsaAdditionalFilesToBeClosed) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFileReloadManagerEvents_INTERFACE_DEFINED__ */


#ifndef __IVsEnumHierarchies_INTERFACE_DEFINED__
#define __IVsEnumHierarchies_INTERFACE_DEFINED__

/* interface IVsEnumHierarchies */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumHierarchies;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("97A31B3B-B37F-43A5-92F4-71E6E63F80F6")
    IVsEnumHierarchies : public IUnknown
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Item( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **lppcReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *lplReturn) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumHierarchiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumHierarchies * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumHierarchies * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumHierarchies * This);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            __RPC__in IVsEnumHierarchies * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Item )( 
            __RPC__in IVsEnumHierarchies * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **lppcReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in IVsEnumHierarchies * This,
            /* [retval][out] */ __RPC__out long *lplReturn);
        
        END_INTERFACE
    } IVsEnumHierarchiesVtbl;

    interface IVsEnumHierarchies
    {
        CONST_VTBL struct IVsEnumHierarchiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumHierarchies_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumHierarchies_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumHierarchies_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumHierarchies_get__NewEnum(This,ppUnk)	\
    ( (This)->lpVtbl -> get__NewEnum(This,ppUnk) ) 

#define IVsEnumHierarchies_get_Item(This,index,lppcReturn)	\
    ( (This)->lpVtbl -> get_Item(This,index,lppcReturn) ) 

#define IVsEnumHierarchies_get_Count(This,lplReturn)	\
    ( (This)->lpVtbl -> get_Count(This,lplReturn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumHierarchies_INTERFACE_DEFINED__ */


#ifndef __IVsSharedAssetsProjectEvents_INTERFACE_DEFINED__
#define __IVsSharedAssetsProjectEvents_INTERFACE_DEFINED__

/* interface IVsSharedAssetsProjectEvents */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSharedAssetsProjectEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("389526A8-4344-4A8B-ACDA-1F180058E57F")
    IVsSharedAssetsProjectEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnImportingProjectLoaded( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pLoadedProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnImportingProjectRemovedOrUnloaded( 
            /* [in] */ GUID removedOrUnloadedProjectGuid) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSharedAssetsProjectEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSharedAssetsProjectEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSharedAssetsProjectEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSharedAssetsProjectEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnImportingProjectLoaded )( 
            __RPC__in IVsSharedAssetsProjectEvents * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pLoadedProject);
        
        HRESULT ( STDMETHODCALLTYPE *OnImportingProjectRemovedOrUnloaded )( 
            __RPC__in IVsSharedAssetsProjectEvents * This,
            /* [in] */ GUID removedOrUnloadedProjectGuid);
        
        END_INTERFACE
    } IVsSharedAssetsProjectEventsVtbl;

    interface IVsSharedAssetsProjectEvents
    {
        CONST_VTBL struct IVsSharedAssetsProjectEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSharedAssetsProjectEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSharedAssetsProjectEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSharedAssetsProjectEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSharedAssetsProjectEvents_OnImportingProjectLoaded(This,pLoadedProject)	\
    ( (This)->lpVtbl -> OnImportingProjectLoaded(This,pLoadedProject) ) 

#define IVsSharedAssetsProjectEvents_OnImportingProjectRemovedOrUnloaded(This,removedOrUnloadedProjectGuid)	\
    ( (This)->lpVtbl -> OnImportingProjectRemovedOrUnloaded(This,removedOrUnloadedProjectGuid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSharedAssetsProjectEvents_INTERFACE_DEFINED__ */


#ifndef __IVsSharedAssetsProject_INTERFACE_DEFINED__
#define __IVsSharedAssetsProject_INTERFACE_DEFINED__

/* interface IVsSharedAssetsProject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSharedAssetsProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ECC3CAB9-FFD8-4ABA-B0D3-25A505CD3B19")
    IVsSharedAssetsProject : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SharedItemsImportFullPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *fullPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumImportingProjects( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumHierarchies **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseEvents( 
            /* [in] */ __RPC__in_opt IVsSharedAssetsProjectEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseEvents( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSharedAssetsProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSharedAssetsProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSharedAssetsProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSharedAssetsProject * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SharedItemsImportFullPath )( 
            __RPC__in IVsSharedAssetsProject * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *fullPath);
        
        HRESULT ( STDMETHODCALLTYPE *EnumImportingProjects )( 
            __RPC__in IVsSharedAssetsProject * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumHierarchies **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseEvents )( 
            __RPC__in IVsSharedAssetsProject * This,
            /* [in] */ __RPC__in_opt IVsSharedAssetsProjectEvents *pEvents,
            /* [retval][out] */ __RPC__out VSCOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseEvents )( 
            __RPC__in IVsSharedAssetsProject * This,
            /* [in] */ VSCOOKIE cookie);
        
        END_INTERFACE
    } IVsSharedAssetsProjectVtbl;

    interface IVsSharedAssetsProject
    {
        CONST_VTBL struct IVsSharedAssetsProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSharedAssetsProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSharedAssetsProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSharedAssetsProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSharedAssetsProject_get_SharedItemsImportFullPath(This,fullPath)	\
    ( (This)->lpVtbl -> get_SharedItemsImportFullPath(This,fullPath) ) 

#define IVsSharedAssetsProject_EnumImportingProjects(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumImportingProjects(This,ppEnum) ) 

#define IVsSharedAssetsProject_AdviseEvents(This,pEvents,pCookie)	\
    ( (This)->lpVtbl -> AdviseEvents(This,pEvents,pCookie) ) 

#define IVsSharedAssetsProject_UnadviseEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadviseEvents(This,cookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSharedAssetsProject_INTERFACE_DEFINED__ */


#ifndef __IVsBuildPropertyStorageEvents_INTERFACE_DEFINED__
#define __IVsBuildPropertyStorageEvents_INTERFACE_DEFINED__

/* interface IVsBuildPropertyStorageEvents */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildPropertyStorageEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C6C93FD-C88F-45AC-AC2B-39E91176F894")
    IVsBuildPropertyStorageEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterItemsChanged( 
            /* [in] */ int cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) const LPCOLESTR rgpszItemFullPaths[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBuildPropertyStorageEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBuildPropertyStorageEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBuildPropertyStorageEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBuildPropertyStorageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterItemsChanged )( 
            __RPC__in IVsBuildPropertyStorageEvents * This,
            /* [in] */ int cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) const LPCOLESTR rgpszItemFullPaths[  ]);
        
        END_INTERFACE
    } IVsBuildPropertyStorageEventsVtbl;

    interface IVsBuildPropertyStorageEvents
    {
        CONST_VTBL struct IVsBuildPropertyStorageEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildPropertyStorageEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildPropertyStorageEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildPropertyStorageEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildPropertyStorageEvents_OnAfterItemsChanged(This,cItems,rgpszItemFullPaths)	\
    ( (This)->lpVtbl -> OnAfterItemsChanged(This,cItems,rgpszItemFullPaths) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildPropertyStorageEvents_INTERFACE_DEFINED__ */


#ifndef __IVsBuildPropertyStorage3_INTERFACE_DEFINED__
#define __IVsBuildPropertyStorage3_INTERFACE_DEFINED__

/* interface IVsBuildPropertyStorage3 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsBuildPropertyStorage3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9669894B-8698-4E4A-BF06-AECA45559C36")
    IVsBuildPropertyStorage3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseEvents( 
            /* [in] */ __RPC__in_opt IVsBuildPropertyStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsBuildPropertyStorage3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsBuildPropertyStorage3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsBuildPropertyStorage3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsBuildPropertyStorage3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseEvents )( 
            __RPC__in IVsBuildPropertyStorage3 * This,
            /* [in] */ __RPC__in_opt IVsBuildPropertyStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseEvents )( 
            __RPC__in IVsBuildPropertyStorage3 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsBuildPropertyStorage3Vtbl;

    interface IVsBuildPropertyStorage3
    {
        CONST_VTBL struct IVsBuildPropertyStorage3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsBuildPropertyStorage3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsBuildPropertyStorage3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsBuildPropertyStorage3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsBuildPropertyStorage3_AdviseEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseEvents(This,pSink,pdwCookie) ) 

#define IVsBuildPropertyStorage3_UnadviseEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsBuildPropertyStorage3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vsshell121_0000_0009 */
/* [local] */ 


enum __VSDOCUMENTPRIORITY2
    {
        DP2_Intrinsic	= 60,
        DP2_Standard	= 50,
        DP2_IndirectMember	= 46,
        DP2_NonMember	= 40,
        DP2_CanAddAsNonMember	= 30,
        DP2_External	= 20,
        DP2_CanAddAsExternal	= 10,
        DP2_Unsupported	= 0
    } ;
typedef LONG VSDOCUMENTPRIORITY2;



extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsshell121_0000_0009_v0_0_s_ifspec;

#ifndef __IVsProject5_INTERFACE_DEFINED__
#define __IVsProject5_INTERFACE_DEFINED__

/* interface IVsProject5 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProject5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B04F747B-48EE-4EC7-8A2E-E9417F6214C3")
    IVsProject5 : public IUnknown
    {
    public:
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE IsDocumentInProject2( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pfFound,
            /* [out] */ __RPC__out VSDOCUMENTPRIORITY2 *pdwPriority2,
            /* [out] */ __RPC__out VSITEMID *pitemid) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProject5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProject5 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProject5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProject5 * This);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *IsDocumentInProject2 )( 
            __RPC__in IVsProject5 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pfFound,
            /* [out] */ __RPC__out VSDOCUMENTPRIORITY2 *pdwPriority2,
            /* [out] */ __RPC__out VSITEMID *pitemid);
        
        END_INTERFACE
    } IVsProject5Vtbl;

    interface IVsProject5
    {
        CONST_VTBL struct IVsProject5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProject5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProject5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProject5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProject5_IsDocumentInProject2(This,pszMkDocument,pfFound,pdwPriority2,pitemid)	\
    ( (This)->lpVtbl -> IsDocumentInProject2(This,pszMkDocument,pfFound,pdwPriority2,pitemid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProject5_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     __RPC__in unsigned long *, __RPC__in LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


