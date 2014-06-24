

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

#ifndef __compsvcspkg110_h__
#define __compsvcspkg110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsAppContainerProjectDeployResult_FWD_DEFINED__
#define __IVsAppContainerProjectDeployResult_FWD_DEFINED__
typedef interface IVsAppContainerProjectDeployResult IVsAppContainerProjectDeployResult;

#endif 	/* __IVsAppContainerProjectDeployResult_FWD_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployOperation_FWD_DEFINED__
#define __IVsAppContainerProjectDeployOperation_FWD_DEFINED__
typedef interface IVsAppContainerProjectDeployOperation IVsAppContainerProjectDeployOperation;

#endif 	/* __IVsAppContainerProjectDeployOperation_FWD_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployCallback_FWD_DEFINED__
#define __IVsAppContainerProjectDeployCallback_FWD_DEFINED__
typedef interface IVsAppContainerProjectDeployCallback IVsAppContainerProjectDeployCallback;

#endif 	/* __IVsAppContainerProjectDeployCallback_FWD_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployEvents_FWD_DEFINED__
#define __IVsAppContainerProjectDeployEvents_FWD_DEFINED__
typedef interface IVsAppContainerProjectDeployEvents IVsAppContainerProjectDeployEvents;

#endif 	/* __IVsAppContainerProjectDeployEvents_FWD_DEFINED__ */


#ifndef __IVsAppContainerProjectDeploy_FWD_DEFINED__
#define __IVsAppContainerProjectDeploy_FWD_DEFINED__
typedef interface IVsAppContainerProjectDeploy IVsAppContainerProjectDeploy;

#endif 	/* __IVsAppContainerProjectDeploy_FWD_DEFINED__ */


#ifndef __SVsAppContainerProjectDeploy_FWD_DEFINED__
#define __SVsAppContainerProjectDeploy_FWD_DEFINED__
typedef interface SVsAppContainerProjectDeploy SVsAppContainerProjectDeploy;

#endif 	/* __SVsAppContainerProjectDeploy_FWD_DEFINED__ */


#ifndef __IVsAppContainerDeveloperLicensing_FWD_DEFINED__
#define __IVsAppContainerDeveloperLicensing_FWD_DEFINED__
typedef interface IVsAppContainerDeveloperLicensing IVsAppContainerDeveloperLicensing;

#endif 	/* __IVsAppContainerDeveloperLicensing_FWD_DEFINED__ */


#ifndef __SVsAppContainerDeveloperLicensing_FWD_DEFINED__
#define __SVsAppContainerDeveloperLicensing_FWD_DEFINED__
typedef interface SVsAppContainerDeveloperLicensing SVsAppContainerDeveloperLicensing;

#endif 	/* __SVsAppContainerDeveloperLicensing_FWD_DEFINED__ */


#ifndef __IVsFrameworkMultiTargeting2_FWD_DEFINED__
#define __IVsFrameworkMultiTargeting2_FWD_DEFINED__
typedef interface IVsFrameworkMultiTargeting2 IVsFrameworkMultiTargeting2;

#endif 	/* __IVsFrameworkMultiTargeting2_FWD_DEFINED__ */


#ifndef __IAppxBaseExtension_FWD_DEFINED__
#define __IAppxBaseExtension_FWD_DEFINED__
typedef interface IAppxBaseExtension IAppxBaseExtension;

#endif 	/* __IAppxBaseExtension_FWD_DEFINED__ */


#ifndef __IAppxFileOpenPickerExtension_FWD_DEFINED__
#define __IAppxFileOpenPickerExtension_FWD_DEFINED__
typedef interface IAppxFileOpenPickerExtension IAppxFileOpenPickerExtension;

#endif 	/* __IAppxFileOpenPickerExtension_FWD_DEFINED__ */


#ifndef __IAppxShareExtension_FWD_DEFINED__
#define __IAppxShareExtension_FWD_DEFINED__
typedef interface IAppxShareExtension IAppxShareExtension;

#endif 	/* __IAppxShareExtension_FWD_DEFINED__ */


#ifndef __IAppxManifestDocument_FWD_DEFINED__
#define __IAppxManifestDocument_FWD_DEFINED__
typedef interface IAppxManifestDocument IAppxManifestDocument;

#endif 	/* __IAppxManifestDocument_FWD_DEFINED__ */


#ifndef __IAppxManifestDesignerService_FWD_DEFINED__
#define __IAppxManifestDesignerService_FWD_DEFINED__
typedef interface IAppxManifestDesignerService IAppxManifestDesignerService;

#endif 	/* __IAppxManifestDesignerService_FWD_DEFINED__ */


#ifndef __SAppxManifestDesignerService_FWD_DEFINED__
#define __SAppxManifestDesignerService_FWD_DEFINED__
typedef interface SAppxManifestDesignerService SAppxManifestDesignerService;

#endif 	/* __SAppxManifestDesignerService_FWD_DEFINED__ */


#ifndef __IVsDebugTargetSelectionService_FWD_DEFINED__
#define __IVsDebugTargetSelectionService_FWD_DEFINED__
typedef interface IVsDebugTargetSelectionService IVsDebugTargetSelectionService;

#endif 	/* __IVsDebugTargetSelectionService_FWD_DEFINED__ */


#ifndef __IVsProjectCfgDebugTargetSelection_FWD_DEFINED__
#define __IVsProjectCfgDebugTargetSelection_FWD_DEFINED__
typedef interface IVsProjectCfgDebugTargetSelection IVsProjectCfgDebugTargetSelection;

#endif 	/* __IVsProjectCfgDebugTargetSelection_FWD_DEFINED__ */


#ifndef __SVsDebugTargetSelectionService_FWD_DEFINED__
#define __SVsDebugTargetSelectionService_FWD_DEFINED__
typedef interface SVsDebugTargetSelectionService SVsDebugTargetSelectionService;

#endif 	/* __SVsDebugTargetSelectionService_FWD_DEFINED__ */


#ifndef __IVsProjectCfgDebugTypeSelection_FWD_DEFINED__
#define __IVsProjectCfgDebugTypeSelection_FWD_DEFINED__
typedef interface IVsProjectCfgDebugTypeSelection IVsProjectCfgDebugTypeSelection;

#endif 	/* __IVsProjectCfgDebugTypeSelection_FWD_DEFINED__ */


#ifndef __IVsStrongNameKeys2_FWD_DEFINED__
#define __IVsStrongNameKeys2_FWD_DEFINED__
typedef interface IVsStrongNameKeys2 IVsStrongNameKeys2;

#endif 	/* __IVsStrongNameKeys2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell90.h"
#include "vsshell100.h"
#include "compsvcspkg80.h"
#include "compsvcspkg90.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_compsvcspkg110_0000_0000 */
/* [local] */ 

#pragma once

enum _AppContainerDeployOptions
    {
        ACDO_ForceRegistration	= 0x1,
        ACDO_ForceCleanLayout	= 0x2,
        ACDO_UseUniqueDeployPackageIdentity	= 0x4,
        ACDO_RefreshLayoutOnly	= 0x8,
        ACDO_SetNetworkLoopback	= 0x10,
        ACDO_NetworkLoopbackEnable	= 0x20
    } ;
typedef DWORD AppContainerDeployOptions;


enum _DevLicenseCheckOptions
    {
        DLCO_SilenCheckOnly	= 0x1
    } ;
typedef DWORD DevLicenseCheckOptions;

typedef 
enum _DevLicenseStatus
    {
        DLS_NoDevLicense	= 1,
        DLS_DevLicenseValid	= 2,
        DLS_DevLicenseExpired	= 3
    } 	DevLicenseStatus;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0000_v0_0_s_ifspec;

#ifndef __IVsAppContainerProjectDeployResult_INTERFACE_DEFINED__
#define __IVsAppContainerProjectDeployResult_INTERFACE_DEFINED__

/* interface IVsAppContainerProjectDeployResult */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerProjectDeployResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96785CEA-89F6-4FE6-AB54-F86E877DDFCD")
    IVsAppContainerProjectDeployResult : public IDispatch
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DeploySuccess( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fSuccess) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_PackageFullName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrPackageFullName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_FirstAppUserModelID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFirstAppUserModelID) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_LayoutFolder( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrLayoutFolder) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerProjectDeployResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerProjectDeployResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerProjectDeployResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsAppContainerProjectDeployResult * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DeploySuccess )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *fSuccess);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_PackageFullName )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrPackageFullName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_FirstAppUserModelID )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrFirstAppUserModelID);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_LayoutFolder )( 
            __RPC__in IVsAppContainerProjectDeployResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *bstrLayoutFolder);
        
        END_INTERFACE
    } IVsAppContainerProjectDeployResultVtbl;

    interface IVsAppContainerProjectDeployResult
    {
        CONST_VTBL struct IVsAppContainerProjectDeployResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerProjectDeployResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerProjectDeployResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerProjectDeployResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerProjectDeployResult_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVsAppContainerProjectDeployResult_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVsAppContainerProjectDeployResult_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVsAppContainerProjectDeployResult_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVsAppContainerProjectDeployResult_get_DeploySuccess(This,fSuccess)	\
    ( (This)->lpVtbl -> get_DeploySuccess(This,fSuccess) ) 

#define IVsAppContainerProjectDeployResult_get_PackageFullName(This,bstrPackageFullName)	\
    ( (This)->lpVtbl -> get_PackageFullName(This,bstrPackageFullName) ) 

#define IVsAppContainerProjectDeployResult_get_FirstAppUserModelID(This,bstrFirstAppUserModelID)	\
    ( (This)->lpVtbl -> get_FirstAppUserModelID(This,bstrFirstAppUserModelID) ) 

#define IVsAppContainerProjectDeployResult_get_LayoutFolder(This,bstrLayoutFolder)	\
    ( (This)->lpVtbl -> get_LayoutFolder(This,bstrLayoutFolder) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerProjectDeployResult_INTERFACE_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployOperation_INTERFACE_DEFINED__
#define __IVsAppContainerProjectDeployOperation_INTERFACE_DEFINED__

/* interface IVsAppContainerProjectDeployOperation */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerProjectDeployOperation;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7773BD9E-1F28-4787-986B-1C42C000E31C")
    IVsAppContainerProjectDeployOperation : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StopDeploy( 
            /* [in] */ VARIANT_BOOL fSync) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDeployResult( 
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployResult **result) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerProjectDeployOperationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerProjectDeployOperation * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerProjectDeployOperation * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerProjectDeployOperation * This);
        
        HRESULT ( STDMETHODCALLTYPE *StopDeploy )( 
            __RPC__in IVsAppContainerProjectDeployOperation * This,
            /* [in] */ VARIANT_BOOL fSync);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeployResult )( 
            __RPC__in IVsAppContainerProjectDeployOperation * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployResult **result);
        
        END_INTERFACE
    } IVsAppContainerProjectDeployOperationVtbl;

    interface IVsAppContainerProjectDeployOperation
    {
        CONST_VTBL struct IVsAppContainerProjectDeployOperationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerProjectDeployOperation_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerProjectDeployOperation_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerProjectDeployOperation_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerProjectDeployOperation_StopDeploy(This,fSync)	\
    ( (This)->lpVtbl -> StopDeploy(This,fSync) ) 

#define IVsAppContainerProjectDeployOperation_GetDeployResult(This,result)	\
    ( (This)->lpVtbl -> GetDeployResult(This,result) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerProjectDeployOperation_INTERFACE_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployCallback_INTERFACE_DEFINED__
#define __IVsAppContainerProjectDeployCallback_INTERFACE_DEFINED__

/* interface IVsAppContainerProjectDeployCallback */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerProjectDeployCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D1F433DF-B126-49A7-A1FD-F4099EFC1A05")
    IVsAppContainerProjectDeployCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnEndDeploy( 
            /* [in] */ VARIANT_BOOL successful,
            /* [in] */ __RPC__in LPCOLESTR deployedPackageMoniker,
            /* [in] */ __RPC__in LPCOLESTR deployedAppUserModelID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OutputMessage( 
            /* [in] */ __RPC__in LPCOLESTR message) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerProjectDeployCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerProjectDeployCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerProjectDeployCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerProjectDeployCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnEndDeploy )( 
            __RPC__in IVsAppContainerProjectDeployCallback * This,
            /* [in] */ VARIANT_BOOL successful,
            /* [in] */ __RPC__in LPCOLESTR deployedPackageMoniker,
            /* [in] */ __RPC__in LPCOLESTR deployedAppUserModelID);
        
        HRESULT ( STDMETHODCALLTYPE *OutputMessage )( 
            __RPC__in IVsAppContainerProjectDeployCallback * This,
            /* [in] */ __RPC__in LPCOLESTR message);
        
        END_INTERFACE
    } IVsAppContainerProjectDeployCallbackVtbl;

    interface IVsAppContainerProjectDeployCallback
    {
        CONST_VTBL struct IVsAppContainerProjectDeployCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerProjectDeployCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerProjectDeployCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerProjectDeployCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerProjectDeployCallback_OnEndDeploy(This,successful,deployedPackageMoniker,deployedAppUserModelID)	\
    ( (This)->lpVtbl -> OnEndDeploy(This,successful,deployedPackageMoniker,deployedAppUserModelID) ) 

#define IVsAppContainerProjectDeployCallback_OutputMessage(This,message)	\
    ( (This)->lpVtbl -> OutputMessage(This,message) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerProjectDeployCallback_INTERFACE_DEFINED__ */


#ifndef __IVsAppContainerProjectDeployEvents_INTERFACE_DEFINED__
#define __IVsAppContainerProjectDeployEvents_INTERFACE_DEFINED__

/* interface IVsAppContainerProjectDeployEvents */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerProjectDeployEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("35CC3FD8-5435-4B9E-95CD-7E30C378FBDF")
    IVsAppContainerProjectDeployEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryDeployStart( 
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [out] */ __RPC__out VARIANT_BOOL *fForceLocalDeployment,
            /* [out] */ __RPC__out VARIANT_BOOL *fCancel,
            /* [out] */ __RPC__deref_out_opt BSTR *cancelReason) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnDeployStart( 
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnDeployEnd( 
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployResult *result) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerProjectDeployEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryDeployStart )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [out] */ __RPC__out VARIANT_BOOL *fForceLocalDeployment,
            /* [out] */ __RPC__out VARIANT_BOOL *fCancel,
            /* [out] */ __RPC__deref_out_opt BSTR *cancelReason);
        
        HRESULT ( STDMETHODCALLTYPE *OnDeployStart )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName);
        
        HRESULT ( STDMETHODCALLTYPE *OnDeployEnd )( 
            __RPC__in IVsAppContainerProjectDeployEvents * This,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployResult *result);
        
        END_INTERFACE
    } IVsAppContainerProjectDeployEventsVtbl;

    interface IVsAppContainerProjectDeployEvents
    {
        CONST_VTBL struct IVsAppContainerProjectDeployEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerProjectDeployEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerProjectDeployEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerProjectDeployEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerProjectDeployEvents_QueryDeployStart(This,projectUniqueName,fForceLocalDeployment,fCancel,cancelReason)	\
    ( (This)->lpVtbl -> QueryDeployStart(This,projectUniqueName,fForceLocalDeployment,fCancel,cancelReason) ) 

#define IVsAppContainerProjectDeployEvents_OnDeployStart(This,projectUniqueName)	\
    ( (This)->lpVtbl -> OnDeployStart(This,projectUniqueName) ) 

#define IVsAppContainerProjectDeployEvents_OnDeployEnd(This,projectUniqueName,result)	\
    ( (This)->lpVtbl -> OnDeployEnd(This,projectUniqueName,result) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerProjectDeployEvents_INTERFACE_DEFINED__ */


#ifndef __IVsAppContainerProjectDeploy_INTERFACE_DEFINED__
#define __IVsAppContainerProjectDeploy_INTERFACE_DEFINED__

/* interface IVsAppContainerProjectDeploy */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerProjectDeploy;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A830AC2A-4D69-45A3-A157-6574756034D5")
    IVsAppContainerProjectDeploy : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseAppContainerDeployEvents( 
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployEvents *sink,
            /* [retval][out] */ __RPC__out VSCOOKIE *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseAppContainerDeployEvents( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartDeployAsync( 
            /* [in] */ AppContainerDeployOptions deployFlags,
            /* [in] */ __RPC__in LPCOLESTR packageContentsRecipe,
            /* [in] */ __RPC__in LPCOLESTR layoutLocation,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployCallback *deployCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployOperation **deployOperation) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartRemoteDeployAsync( 
            /* [in] */ AppContainerDeployOptions deployFlags,
            /* [in] */ __RPC__in_opt IUnknown *deployConnection,
            /* [in] */ __RPC__in LPCOLESTR packageContentsRecipe,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployCallback *deployCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployOperation **deployOperation) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerProjectDeployVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerProjectDeploy * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerProjectDeploy * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerProjectDeploy * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseAppContainerDeployEvents )( 
            __RPC__in IVsAppContainerProjectDeploy * This,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployEvents *sink,
            /* [retval][out] */ __RPC__out VSCOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseAppContainerDeployEvents )( 
            __RPC__in IVsAppContainerProjectDeploy * This,
            /* [in] */ VSCOOKIE cookie);
        
        HRESULT ( STDMETHODCALLTYPE *StartDeployAsync )( 
            __RPC__in IVsAppContainerProjectDeploy * This,
            /* [in] */ AppContainerDeployOptions deployFlags,
            /* [in] */ __RPC__in LPCOLESTR packageContentsRecipe,
            /* [in] */ __RPC__in LPCOLESTR layoutLocation,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployCallback *deployCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployOperation **deployOperation);
        
        HRESULT ( STDMETHODCALLTYPE *StartRemoteDeployAsync )( 
            __RPC__in IVsAppContainerProjectDeploy * This,
            /* [in] */ AppContainerDeployOptions deployFlags,
            /* [in] */ __RPC__in_opt IUnknown *deployConnection,
            /* [in] */ __RPC__in LPCOLESTR packageContentsRecipe,
            /* [in] */ __RPC__in LPCOLESTR projectUniqueName,
            /* [in] */ __RPC__in_opt IVsAppContainerProjectDeployCallback *deployCallback,
            /* [retval][out] */ __RPC__deref_out_opt IVsAppContainerProjectDeployOperation **deployOperation);
        
        END_INTERFACE
    } IVsAppContainerProjectDeployVtbl;

    interface IVsAppContainerProjectDeploy
    {
        CONST_VTBL struct IVsAppContainerProjectDeployVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerProjectDeploy_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerProjectDeploy_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerProjectDeploy_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerProjectDeploy_AdviseAppContainerDeployEvents(This,sink,pCookie)	\
    ( (This)->lpVtbl -> AdviseAppContainerDeployEvents(This,sink,pCookie) ) 

#define IVsAppContainerProjectDeploy_UnadviseAppContainerDeployEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadviseAppContainerDeployEvents(This,cookie) ) 

#define IVsAppContainerProjectDeploy_StartDeployAsync(This,deployFlags,packageContentsRecipe,layoutLocation,projectUniqueName,deployCallback,deployOperation)	\
    ( (This)->lpVtbl -> StartDeployAsync(This,deployFlags,packageContentsRecipe,layoutLocation,projectUniqueName,deployCallback,deployOperation) ) 

#define IVsAppContainerProjectDeploy_StartRemoteDeployAsync(This,deployFlags,deployConnection,packageContentsRecipe,projectUniqueName,deployCallback,deployOperation)	\
    ( (This)->lpVtbl -> StartRemoteDeployAsync(This,deployFlags,deployConnection,packageContentsRecipe,projectUniqueName,deployCallback,deployOperation) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerProjectDeploy_INTERFACE_DEFINED__ */


#ifndef __SVsAppContainerProjectDeploy_INTERFACE_DEFINED__
#define __SVsAppContainerProjectDeploy_INTERFACE_DEFINED__

/* interface SVsAppContainerProjectDeploy */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsAppContainerProjectDeploy;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E97079C-BB7C-4003-8DFB-730CD0B88250")
    SVsAppContainerProjectDeploy : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsAppContainerProjectDeployVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsAppContainerProjectDeploy * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsAppContainerProjectDeploy * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsAppContainerProjectDeploy * This);
        
        END_INTERFACE
    } SVsAppContainerProjectDeployVtbl;

    interface SVsAppContainerProjectDeploy
    {
        CONST_VTBL struct SVsAppContainerProjectDeployVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsAppContainerProjectDeploy_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsAppContainerProjectDeploy_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsAppContainerProjectDeploy_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsAppContainerProjectDeploy_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg110_0000_0006 */
/* [local] */ 

#define SID_SVsAppContainerProjectDeploy IID_SVsAppContainerProjectDeploy


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0006_v0_0_s_ifspec;

#ifndef __IVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__
#define __IVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__

/* interface IVsAppContainerDeveloperLicensing */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsAppContainerDeveloperLicensing;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AB6D6E32-671E-444F-8B52-EA446698B038")
    IVsAppContainerDeveloperLicensing : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CheckDeveloperLicense( 
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine,
            /* [retval][out] */ __RPC__out DATE *pExpiration) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AcquireDeveloperLicense( 
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine,
            /* [retval][out] */ __RPC__out DATE *pExpiration) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveDeveloperLicense( 
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsAppContainerDeveloperLicensingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckDeveloperLicense )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This,
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine,
            /* [retval][out] */ __RPC__out DATE *pExpiration);
        
        HRESULT ( STDMETHODCALLTYPE *AcquireDeveloperLicense )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This,
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine,
            /* [retval][out] */ __RPC__out DATE *pExpiration);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveDeveloperLicense )( 
            __RPC__in IVsAppContainerDeveloperLicensing * This,
            /* [in] */ __RPC__deref_in_opt BSTR *pbstrMachine);
        
        END_INTERFACE
    } IVsAppContainerDeveloperLicensingVtbl;

    interface IVsAppContainerDeveloperLicensing
    {
        CONST_VTBL struct IVsAppContainerDeveloperLicensingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAppContainerDeveloperLicensing_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAppContainerDeveloperLicensing_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAppContainerDeveloperLicensing_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAppContainerDeveloperLicensing_CheckDeveloperLicense(This,pbstrMachine,pExpiration)	\
    ( (This)->lpVtbl -> CheckDeveloperLicense(This,pbstrMachine,pExpiration) ) 

#define IVsAppContainerDeveloperLicensing_AcquireDeveloperLicense(This,pbstrMachine,pExpiration)	\
    ( (This)->lpVtbl -> AcquireDeveloperLicense(This,pbstrMachine,pExpiration) ) 

#define IVsAppContainerDeveloperLicensing_RemoveDeveloperLicense(This,pbstrMachine)	\
    ( (This)->lpVtbl -> RemoveDeveloperLicense(This,pbstrMachine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__ */


#ifndef __SVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__
#define __SVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__

/* interface SVsAppContainerDeveloperLicensing */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsAppContainerDeveloperLicensing;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("46B0C039-DE36-4DAE-A038-9F1635F5F962")
    SVsAppContainerDeveloperLicensing : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsAppContainerDeveloperLicensingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsAppContainerDeveloperLicensing * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsAppContainerDeveloperLicensing * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsAppContainerDeveloperLicensing * This);
        
        END_INTERFACE
    } SVsAppContainerDeveloperLicensingVtbl;

    interface SVsAppContainerDeveloperLicensing
    {
        CONST_VTBL struct SVsAppContainerDeveloperLicensingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsAppContainerDeveloperLicensing_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsAppContainerDeveloperLicensing_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsAppContainerDeveloperLicensing_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsAppContainerDeveloperLicensing_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg110_0000_0008 */
/* [local] */ 

#define SID_SVsAppContainerDeveloperLicensing IID_SVsAppContainerDeveloperLicensing
extern const __declspec(selectany) GUID guidDeveloperLicensingTypesCmdSet = { 0x92C03CE4, 0x4B7C, 0x4144, { 0xAC, 0x32, 0x1A, 0x24, 0xAA, 0xF6, 0xFF, 0x5E } };

enum __VSDEVELOPERLICENSINGCOMMANDS
    {
        CMDID_AcquireDeveloperLicense	= 0x100
    } ;
typedef DWORD VSDEVELOPERLICENSINGCOMMANDS;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0008_v0_0_s_ifspec;

#ifndef __IVsFrameworkMultiTargeting2_INTERFACE_DEFINED__
#define __IVsFrameworkMultiTargeting2_INTERFACE_DEFINED__

/* interface IVsFrameworkMultiTargeting2 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsFrameworkMultiTargeting2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0F0CC2B9-8293-4756-9516-ECB1DB326487")
    IVsFrameworkMultiTargeting2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSDKRootFolders( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *sdkRootFolders) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSDKDisplayName( 
            /* [in] */ __RPC__in LPCWSTR pwszSDKRootDirectory,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSDKReferences( 
            /* [in] */ __RPC__in LPCWSTR pwszSDKRootDirectory,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *sdkReferences) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPathsInTargetFx2( 
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) SAFEARRAY * prgAssemblySpecs,
            /* [in] */ ULONG cAssembliesToResolve,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResolveAssemblyPath2( 
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrResolvedAssemblyPath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsFrameworkMultiTargeting2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSDKRootFolders )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *sdkRootFolders);
        
        HRESULT ( STDMETHODCALLTYPE *GetSDKDisplayName )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [in] */ __RPC__in LPCWSTR pwszSDKRootDirectory,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrDisplayName);
        
        HRESULT ( STDMETHODCALLTYPE *GetSDKReferences )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [in] */ __RPC__in LPCWSTR pwszSDKRootDirectory,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *sdkReferences);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPathsInTargetFx2 )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssembliesToResolve) SAFEARRAY * prgAssemblySpecs,
            /* [in] */ ULONG cAssembliesToResolve,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cAssembliesToResolve) PVsResolvedAssemblyPath prgResolvedAssemblyPaths,
            /* [out] */ __RPC__out ULONG *pcResolvedAssemblyPaths);
        
        HRESULT ( STDMETHODCALLTYPE *ResolveAssemblyPath2 )( 
            __RPC__in IVsFrameworkMultiTargeting2 * This,
            /* [in] */ __RPC__in LPCWSTR pwszAssemblySpec,
            /* [in] */ __RPC__in LPCWSTR pwszTargetFrameworkMoniker,
            /* [in] */ VARIANT_BOOL ignoreVersionForFrameworkReferences,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrResolvedAssemblyPath);
        
        END_INTERFACE
    } IVsFrameworkMultiTargeting2Vtbl;

    interface IVsFrameworkMultiTargeting2
    {
        CONST_VTBL struct IVsFrameworkMultiTargeting2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFrameworkMultiTargeting2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFrameworkMultiTargeting2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFrameworkMultiTargeting2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFrameworkMultiTargeting2_GetSDKRootFolders(This,sdkRootFolders)	\
    ( (This)->lpVtbl -> GetSDKRootFolders(This,sdkRootFolders) ) 

#define IVsFrameworkMultiTargeting2_GetSDKDisplayName(This,pwszSDKRootDirectory,pbstrDisplayName)	\
    ( (This)->lpVtbl -> GetSDKDisplayName(This,pwszSDKRootDirectory,pbstrDisplayName) ) 

#define IVsFrameworkMultiTargeting2_GetSDKReferences(This,pwszSDKRootDirectory,sdkReferences)	\
    ( (This)->lpVtbl -> GetSDKReferences(This,pwszSDKRootDirectory,sdkReferences) ) 

#define IVsFrameworkMultiTargeting2_ResolveAssemblyPathsInTargetFx2(This,pwszTargetFrameworkMoniker,prgAssemblySpecs,cAssembliesToResolve,ignoreVersionForFrameworkReferences,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths)	\
    ( (This)->lpVtbl -> ResolveAssemblyPathsInTargetFx2(This,pwszTargetFrameworkMoniker,prgAssemblySpecs,cAssembliesToResolve,ignoreVersionForFrameworkReferences,prgResolvedAssemblyPaths,pcResolvedAssemblyPaths) ) 

#define IVsFrameworkMultiTargeting2_ResolveAssemblyPath2(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,ignoreVersionForFrameworkReferences,pbstrResolvedAssemblyPath)	\
    ( (This)->lpVtbl -> ResolveAssemblyPath2(This,pwszAssemblySpec,pwszTargetFrameworkMoniker,ignoreVersionForFrameworkReferences,pbstrResolvedAssemblyPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFrameworkMultiTargeting2_INTERFACE_DEFINED__ */


#ifndef __IAppxBaseExtension_INTERFACE_DEFINED__
#define __IAppxBaseExtension_INTERFACE_DEFINED__

/* interface IAppxBaseExtension */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IAppxBaseExtension;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("893AC9E5-23A5-4561-9500-C27EF5A058AC")
    IAppxBaseExtension : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SupportsAnyFileType( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *supportsAnyFileType) = 0;
        
        virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_SupportsAnyFileType( 
            /* [in] */ VARIANT_BOOL supportsAnyFileType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HasSupportedFileType( 
            /* [in] */ __RPC__in LPCOLESTR supportedFileType,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSupportedFileType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddSupportedFileType( 
            /* [in] */ __RPC__in LPCOLESTR supportedFileType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSupportedFileType( 
            /* [in] */ __RPC__in LPCOLESTR supportedFileType) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SupportedFileTypes( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *supportedFileTypes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAppxBaseExtensionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppxBaseExtension * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppxBaseExtension * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppxBaseExtension * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportsAnyFileType )( 
            __RPC__in IAppxBaseExtension * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *supportsAnyFileType);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SupportsAnyFileType )( 
            __RPC__in IAppxBaseExtension * This,
            /* [in] */ VARIANT_BOOL supportsAnyFileType);
        
        HRESULT ( STDMETHODCALLTYPE *HasSupportedFileType )( 
            __RPC__in IAppxBaseExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSupportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *AddSupportedFileType )( 
            __RPC__in IAppxBaseExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSupportedFileType )( 
            __RPC__in IAppxBaseExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportedFileTypes )( 
            __RPC__in IAppxBaseExtension * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *supportedFileTypes);
        
        END_INTERFACE
    } IAppxBaseExtensionVtbl;

    interface IAppxBaseExtension
    {
        CONST_VTBL struct IAppxBaseExtensionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppxBaseExtension_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppxBaseExtension_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppxBaseExtension_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppxBaseExtension_get_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> get_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxBaseExtension_put_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> put_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxBaseExtension_HasSupportedFileType(This,supportedFileType,hasSupportedFileType)	\
    ( (This)->lpVtbl -> HasSupportedFileType(This,supportedFileType,hasSupportedFileType) ) 

#define IAppxBaseExtension_AddSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> AddSupportedFileType(This,supportedFileType) ) 

#define IAppxBaseExtension_RemoveSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> RemoveSupportedFileType(This,supportedFileType) ) 

#define IAppxBaseExtension_get_SupportedFileTypes(This,supportedFileTypes)	\
    ( (This)->lpVtbl -> get_SupportedFileTypes(This,supportedFileTypes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppxBaseExtension_INTERFACE_DEFINED__ */


#ifndef __IAppxFileOpenPickerExtension_INTERFACE_DEFINED__
#define __IAppxFileOpenPickerExtension_INTERFACE_DEFINED__

/* interface IAppxFileOpenPickerExtension */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IAppxFileOpenPickerExtension;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("01B7617F-B00A-4290-870E-D329B5A43033")
    IAppxFileOpenPickerExtension : public IAppxBaseExtension
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IAppxFileOpenPickerExtensionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppxFileOpenPickerExtension * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppxFileOpenPickerExtension * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportsAnyFileType )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *supportsAnyFileType);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SupportsAnyFileType )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [in] */ VARIANT_BOOL supportsAnyFileType);
        
        HRESULT ( STDMETHODCALLTYPE *HasSupportedFileType )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSupportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *AddSupportedFileType )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSupportedFileType )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportedFileTypes )( 
            __RPC__in IAppxFileOpenPickerExtension * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *supportedFileTypes);
        
        END_INTERFACE
    } IAppxFileOpenPickerExtensionVtbl;

    interface IAppxFileOpenPickerExtension
    {
        CONST_VTBL struct IAppxFileOpenPickerExtensionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppxFileOpenPickerExtension_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppxFileOpenPickerExtension_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppxFileOpenPickerExtension_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppxFileOpenPickerExtension_get_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> get_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxFileOpenPickerExtension_put_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> put_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxFileOpenPickerExtension_HasSupportedFileType(This,supportedFileType,hasSupportedFileType)	\
    ( (This)->lpVtbl -> HasSupportedFileType(This,supportedFileType,hasSupportedFileType) ) 

#define IAppxFileOpenPickerExtension_AddSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> AddSupportedFileType(This,supportedFileType) ) 

#define IAppxFileOpenPickerExtension_RemoveSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> RemoveSupportedFileType(This,supportedFileType) ) 

#define IAppxFileOpenPickerExtension_get_SupportedFileTypes(This,supportedFileTypes)	\
    ( (This)->lpVtbl -> get_SupportedFileTypes(This,supportedFileTypes) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppxFileOpenPickerExtension_INTERFACE_DEFINED__ */


#ifndef __IAppxShareExtension_INTERFACE_DEFINED__
#define __IAppxShareExtension_INTERFACE_DEFINED__

/* interface IAppxShareExtension */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IAppxShareExtension;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1323D2E8-A6E1-498D-93B9-16CA249C9306")
    IAppxShareExtension : public IAppxBaseExtension
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasDataFormat( 
            /* [in] */ __RPC__in LPCOLESTR dataFormat,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasDataFormat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddDataFormat( 
            /* [in] */ __RPC__in LPCOLESTR dataFormat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveDataFormat( 
            /* [in] */ __RPC__in LPCOLESTR dataFormat) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DataFormats( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *dataFormats) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAppxShareExtensionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppxShareExtension * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppxShareExtension * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportsAnyFileType )( 
            __RPC__in IAppxShareExtension * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *supportsAnyFileType);
        
        /* [propput] */ HRESULT ( STDMETHODCALLTYPE *put_SupportsAnyFileType )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ VARIANT_BOOL supportsAnyFileType);
        
        HRESULT ( STDMETHODCALLTYPE *HasSupportedFileType )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSupportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *AddSupportedFileType )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSupportedFileType )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR supportedFileType);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_SupportedFileTypes )( 
            __RPC__in IAppxShareExtension * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *supportedFileTypes);
        
        HRESULT ( STDMETHODCALLTYPE *HasDataFormat )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR dataFormat,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasDataFormat);
        
        HRESULT ( STDMETHODCALLTYPE *AddDataFormat )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR dataFormat);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveDataFormat )( 
            __RPC__in IAppxShareExtension * This,
            /* [in] */ __RPC__in LPCOLESTR dataFormat);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataFormats )( 
            __RPC__in IAppxShareExtension * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *dataFormats);
        
        END_INTERFACE
    } IAppxShareExtensionVtbl;

    interface IAppxShareExtension
    {
        CONST_VTBL struct IAppxShareExtensionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppxShareExtension_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppxShareExtension_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppxShareExtension_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppxShareExtension_get_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> get_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxShareExtension_put_SupportsAnyFileType(This,supportsAnyFileType)	\
    ( (This)->lpVtbl -> put_SupportsAnyFileType(This,supportsAnyFileType) ) 

#define IAppxShareExtension_HasSupportedFileType(This,supportedFileType,hasSupportedFileType)	\
    ( (This)->lpVtbl -> HasSupportedFileType(This,supportedFileType,hasSupportedFileType) ) 

#define IAppxShareExtension_AddSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> AddSupportedFileType(This,supportedFileType) ) 

#define IAppxShareExtension_RemoveSupportedFileType(This,supportedFileType)	\
    ( (This)->lpVtbl -> RemoveSupportedFileType(This,supportedFileType) ) 

#define IAppxShareExtension_get_SupportedFileTypes(This,supportedFileTypes)	\
    ( (This)->lpVtbl -> get_SupportedFileTypes(This,supportedFileTypes) ) 


#define IAppxShareExtension_HasDataFormat(This,dataFormat,hasDataFormat)	\
    ( (This)->lpVtbl -> HasDataFormat(This,dataFormat,hasDataFormat) ) 

#define IAppxShareExtension_AddDataFormat(This,dataFormat)	\
    ( (This)->lpVtbl -> AddDataFormat(This,dataFormat) ) 

#define IAppxShareExtension_RemoveDataFormat(This,dataFormat)	\
    ( (This)->lpVtbl -> RemoveDataFormat(This,dataFormat) ) 

#define IAppxShareExtension_get_DataFormats(This,dataFormats)	\
    ( (This)->lpVtbl -> get_DataFormats(This,dataFormats) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppxShareExtension_INTERFACE_DEFINED__ */


#ifndef __IAppxManifestDocument_INTERFACE_DEFINED__
#define __IAppxManifestDocument_INTERFACE_DEFINED__

/* interface IAppxManifestDocument */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IAppxManifestDocument;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("80CAD0DE-8ECC-48FC-B81E-D11B13AB9E8A")
    IAppxManifestDocument : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasCapability( 
            /* [in] */ __RPC__in LPCOLESTR capabilityId,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasCapability) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddCapability( 
            /* [in] */ __RPC__in LPCOLESTR capabilityId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveCapability( 
            /* [in] */ __RPC__in LPCOLESTR capabilityId) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Capabilities( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *capabilities) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_StandardCapabilities( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *standardCapabilities) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapCapabilitySid( 
            /* [in] */ __RPC__in LPCOLESTR sid,
            /* [out] */ __RPC__deref_out_opt BSTR *capabilityId,
            /* [out] */ __RPC__deref_out_opt BSTR *localizedCapabilityName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSearchExtension( 
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSearchExtension) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSearchExtension( 
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveSearchExtension( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetShareExtension( 
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [out] */ __RPC__deref_out_opt IAppxShareExtension **shareExtensionData,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasShareExtension) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetShareExtension( 
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage,
            /* [retval][out] */ __RPC__deref_out_opt IAppxShareExtension **shareExtensionData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveShareExtension( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileOpenPickerExtension( 
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [out] */ __RPC__deref_out_opt IAppxFileOpenPickerExtension **fileOpenPickerExtensionData,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasFileOpenPickerExtension) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFileOpenPickerExtension( 
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage,
            /* [retval][out] */ __RPC__deref_out_opt IAppxFileOpenPickerExtension **fileOpenPickerExtensionData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveFileOpenPickerExtension( void) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ApplicationStartPage( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *startPage) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAppxManifestDocumentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppxManifestDocument * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppxManifestDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in IAppxManifestDocument * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ __RPC__in_range(0,16384) UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IAppxManifestDocument * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *HasCapability )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in LPCOLESTR capabilityId,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasCapability);
        
        HRESULT ( STDMETHODCALLTYPE *AddCapability )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in LPCOLESTR capabilityId);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveCapability )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in LPCOLESTR capabilityId);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Capabilities )( 
            __RPC__in IAppxManifestDocument * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *capabilities);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_StandardCapabilities )( 
            __RPC__in IAppxManifestDocument * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *standardCapabilities);
        
        HRESULT ( STDMETHODCALLTYPE *MapCapabilitySid )( 
            __RPC__in IAppxManifestDocument * This,
            /* [in] */ __RPC__in LPCOLESTR sid,
            /* [out] */ __RPC__deref_out_opt BSTR *capabilityId,
            /* [out] */ __RPC__deref_out_opt BSTR *localizedCapabilityName);
        
        HRESULT ( STDMETHODCALLTYPE *GetSearchExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasSearchExtension);
        
        HRESULT ( STDMETHODCALLTYPE *SetSearchExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveSearchExtension )( 
            __RPC__in IAppxManifestDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetShareExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [out] */ __RPC__deref_out_opt IAppxShareExtension **shareExtensionData,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasShareExtension);
        
        HRESULT ( STDMETHODCALLTYPE *SetShareExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage,
            /* [retval][out] */ __RPC__deref_out_opt IAppxShareExtension **shareExtensionData);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveShareExtension )( 
            __RPC__in IAppxManifestDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileOpenPickerExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [out] */ __RPC__deref_out_opt BSTR *executable,
            /* [out] */ __RPC__deref_out_opt BSTR *entryPoint,
            /* [out] */ __RPC__deref_out_opt BSTR *runtimeType,
            /* [out] */ __RPC__deref_out_opt BSTR *startPage,
            /* [out] */ __RPC__deref_out_opt IAppxFileOpenPickerExtension **fileOpenPickerExtensionData,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *hasFileOpenPickerExtension);
        
        HRESULT ( STDMETHODCALLTYPE *SetFileOpenPickerExtension )( 
            __RPC__in IAppxManifestDocument * This,
            /* [optional][in] */ __RPC__in LPOLESTR executable,
            /* [optional][in] */ __RPC__in LPOLESTR entryPoint,
            /* [optional][in] */ __RPC__in LPOLESTR runtimeType,
            /* [optional][in] */ __RPC__in LPOLESTR startPage,
            /* [retval][out] */ __RPC__deref_out_opt IAppxFileOpenPickerExtension **fileOpenPickerExtensionData);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveFileOpenPickerExtension )( 
            __RPC__in IAppxManifestDocument * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationStartPage )( 
            __RPC__in IAppxManifestDocument * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *startPage);
        
        END_INTERFACE
    } IAppxManifestDocumentVtbl;

    interface IAppxManifestDocument
    {
        CONST_VTBL struct IAppxManifestDocumentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppxManifestDocument_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppxManifestDocument_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppxManifestDocument_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppxManifestDocument_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IAppxManifestDocument_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IAppxManifestDocument_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IAppxManifestDocument_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IAppxManifestDocument_HasCapability(This,capabilityId,hasCapability)	\
    ( (This)->lpVtbl -> HasCapability(This,capabilityId,hasCapability) ) 

#define IAppxManifestDocument_AddCapability(This,capabilityId)	\
    ( (This)->lpVtbl -> AddCapability(This,capabilityId) ) 

#define IAppxManifestDocument_RemoveCapability(This,capabilityId)	\
    ( (This)->lpVtbl -> RemoveCapability(This,capabilityId) ) 

#define IAppxManifestDocument_get_Capabilities(This,capabilities)	\
    ( (This)->lpVtbl -> get_Capabilities(This,capabilities) ) 

#define IAppxManifestDocument_get_StandardCapabilities(This,standardCapabilities)	\
    ( (This)->lpVtbl -> get_StandardCapabilities(This,standardCapabilities) ) 

#define IAppxManifestDocument_MapCapabilitySid(This,sid,capabilityId,localizedCapabilityName)	\
    ( (This)->lpVtbl -> MapCapabilitySid(This,sid,capabilityId,localizedCapabilityName) ) 

#define IAppxManifestDocument_GetSearchExtension(This,executable,entryPoint,runtimeType,startPage,hasSearchExtension)	\
    ( (This)->lpVtbl -> GetSearchExtension(This,executable,entryPoint,runtimeType,startPage,hasSearchExtension) ) 

#define IAppxManifestDocument_SetSearchExtension(This,executable,entryPoint,runtimeType,startPage)	\
    ( (This)->lpVtbl -> SetSearchExtension(This,executable,entryPoint,runtimeType,startPage) ) 

#define IAppxManifestDocument_RemoveSearchExtension(This)	\
    ( (This)->lpVtbl -> RemoveSearchExtension(This) ) 

#define IAppxManifestDocument_GetShareExtension(This,executable,entryPoint,runtimeType,startPage,shareExtensionData,hasShareExtension)	\
    ( (This)->lpVtbl -> GetShareExtension(This,executable,entryPoint,runtimeType,startPage,shareExtensionData,hasShareExtension) ) 

#define IAppxManifestDocument_SetShareExtension(This,executable,entryPoint,runtimeType,startPage,shareExtensionData)	\
    ( (This)->lpVtbl -> SetShareExtension(This,executable,entryPoint,runtimeType,startPage,shareExtensionData) ) 

#define IAppxManifestDocument_RemoveShareExtension(This)	\
    ( (This)->lpVtbl -> RemoveShareExtension(This) ) 

#define IAppxManifestDocument_GetFileOpenPickerExtension(This,executable,entryPoint,runtimeType,startPage,fileOpenPickerExtensionData,hasFileOpenPickerExtension)	\
    ( (This)->lpVtbl -> GetFileOpenPickerExtension(This,executable,entryPoint,runtimeType,startPage,fileOpenPickerExtensionData,hasFileOpenPickerExtension) ) 

#define IAppxManifestDocument_SetFileOpenPickerExtension(This,executable,entryPoint,runtimeType,startPage,fileOpenPickerExtensionData)	\
    ( (This)->lpVtbl -> SetFileOpenPickerExtension(This,executable,entryPoint,runtimeType,startPage,fileOpenPickerExtensionData) ) 

#define IAppxManifestDocument_RemoveFileOpenPickerExtension(This)	\
    ( (This)->lpVtbl -> RemoveFileOpenPickerExtension(This) ) 

#define IAppxManifestDocument_get_ApplicationStartPage(This,startPage)	\
    ( (This)->lpVtbl -> get_ApplicationStartPage(This,startPage) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppxManifestDocument_INTERFACE_DEFINED__ */


#ifndef __IAppxManifestDesignerService_INTERFACE_DEFINED__
#define __IAppxManifestDesignerService_INTERFACE_DEFINED__

/* interface IAppxManifestDesignerService */
/* [object][custom][unique][helpstring][uuid] */ 

typedef 
enum _AppxManifestDesignerTab
    {
        AppxManifestDesignerTab_Current	= 0,
        AppxManifestDesignerTab_Application	= 1,
        AppxManifestDesignerTab_Capabilities	= 2,
        AppxManifestDesignerTab_Declarations	= 3,
        AppxManifestDesignerTab_ContentURIs	= 4,
        AppxManifestDesignerTab_Packaging	= 5
    } 	AppxManifestDesignerTab;


EXTERN_C const IID IID_IAppxManifestDesignerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("11D31BA1-480F-435F-B711-8F192A1C226E")
    IAppxManifestDesignerService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenAppxManifestDocument( 
            /* [in] */ __RPC__in_opt IUnknown *project,
            /* [out] */ __RPC__deref_out_opt IVsDocumentLockHolder **documentHandle,
            /* [out] */ __RPC__deref_out_opt IAppxManifestDocument **appxManifestDocument) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenAppxManifestDesigner( 
            /* [in] */ __RPC__in_opt IVsDocumentLockHolder *documentHandle,
            /* [optional][in] */ AppxManifestDesignerTab tab) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAppxManifestDesignerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IAppxManifestDesignerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IAppxManifestDesignerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IAppxManifestDesignerService * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenAppxManifestDocument )( 
            __RPC__in IAppxManifestDesignerService * This,
            /* [in] */ __RPC__in_opt IUnknown *project,
            /* [out] */ __RPC__deref_out_opt IVsDocumentLockHolder **documentHandle,
            /* [out] */ __RPC__deref_out_opt IAppxManifestDocument **appxManifestDocument);
        
        HRESULT ( STDMETHODCALLTYPE *OpenAppxManifestDesigner )( 
            __RPC__in IAppxManifestDesignerService * This,
            /* [in] */ __RPC__in_opt IVsDocumentLockHolder *documentHandle,
            /* [optional][in] */ AppxManifestDesignerTab tab);
        
        END_INTERFACE
    } IAppxManifestDesignerServiceVtbl;

    interface IAppxManifestDesignerService
    {
        CONST_VTBL struct IAppxManifestDesignerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAppxManifestDesignerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAppxManifestDesignerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAppxManifestDesignerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAppxManifestDesignerService_OpenAppxManifestDocument(This,project,documentHandle,appxManifestDocument)	\
    ( (This)->lpVtbl -> OpenAppxManifestDocument(This,project,documentHandle,appxManifestDocument) ) 

#define IAppxManifestDesignerService_OpenAppxManifestDesigner(This,documentHandle,tab)	\
    ( (This)->lpVtbl -> OpenAppxManifestDesigner(This,documentHandle,tab) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAppxManifestDesignerService_INTERFACE_DEFINED__ */


#ifndef __SAppxManifestDesignerService_INTERFACE_DEFINED__
#define __SAppxManifestDesignerService_INTERFACE_DEFINED__

/* interface SAppxManifestDesignerService */
/* [object][uuid] */ 


EXTERN_C const IID IID_SAppxManifestDesignerService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5692CB97-BF4F-49EE-B62E-718A6C8F9CB2")
    SAppxManifestDesignerService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SAppxManifestDesignerServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SAppxManifestDesignerService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SAppxManifestDesignerService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SAppxManifestDesignerService * This);
        
        END_INTERFACE
    } SAppxManifestDesignerServiceVtbl;

    interface SAppxManifestDesignerService
    {
        CONST_VTBL struct SAppxManifestDesignerServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SAppxManifestDesignerService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SAppxManifestDesignerService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SAppxManifestDesignerService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SAppxManifestDesignerService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg110_0000_0015 */
/* [local] */ 

#define SID_SAppxManifestDesignerService IID_SAppxManifestDesignerService


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0015_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0015_v0_0_s_ifspec;

#ifndef __IVsDebugTargetSelectionService_INTERFACE_DEFINED__
#define __IVsDebugTargetSelectionService_INTERFACE_DEFINED__

/* interface IVsDebugTargetSelectionService */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsDebugTargetSelectionService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("310466BA-2B59-4CCE-9592-95E195096939")
    IVsDebugTargetSelectionService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateDebugTargets( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDebugTargetSelectionServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDebugTargetSelectionService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDebugTargetSelectionService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDebugTargetSelectionService * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateDebugTargets )( 
            __RPC__in IVsDebugTargetSelectionService * This);
        
        END_INTERFACE
    } IVsDebugTargetSelectionServiceVtbl;

    interface IVsDebugTargetSelectionService
    {
        CONST_VTBL struct IVsDebugTargetSelectionServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDebugTargetSelectionService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDebugTargetSelectionService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDebugTargetSelectionService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDebugTargetSelectionService_UpdateDebugTargets(This)	\
    ( (This)->lpVtbl -> UpdateDebugTargets(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDebugTargetSelectionService_INTERFACE_DEFINED__ */


#ifndef __IVsProjectCfgDebugTargetSelection_INTERFACE_DEFINED__
#define __IVsProjectCfgDebugTargetSelection_INTERFACE_DEFINED__

/* interface IVsProjectCfgDebugTargetSelection */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProjectCfgDebugTargetSelection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("255B9803-BA83-421B-924E-CDE7FAAA86A3")
    IVsProjectCfgDebugTargetSelection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasDebugTargets( 
            __RPC__in_opt IVsDebugTargetSelectionService *pDebugTargetSelectionService,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrSupportedTargetCommandIDs,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pHasDebugTarget) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugTargetListOfType( 
            /* [in] */ GUID guidDebugTargetType,
            /* [in] */ DWORD debugTargetTypeId,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrDebugTargetListArray) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentDebugTarget( 
            /* [out] */ __RPC__out GUID *pguidDebugTargetType,
            /* [out] */ __RPC__out DWORD *pDebugTargetTypeId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDebugTarget) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentDebugTarget( 
            /* [in] */ GUID guidDebugTargetType,
            /* [in] */ DWORD debugTargetTypeId,
            /* [in] */ __RPC__in BSTR bstrCurrentDebugTarget) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectCfgDebugTargetSelectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This);
        
        HRESULT ( STDMETHODCALLTYPE *HasDebugTargets )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This,
            __RPC__in_opt IVsDebugTargetSelectionService *pDebugTargetSelectionService,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrSupportedTargetCommandIDs,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pHasDebugTarget);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugTargetListOfType )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This,
            /* [in] */ GUID guidDebugTargetType,
            /* [in] */ DWORD debugTargetTypeId,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrDebugTargetListArray);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentDebugTarget )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This,
            /* [out] */ __RPC__out GUID *pguidDebugTargetType,
            /* [out] */ __RPC__out DWORD *pDebugTargetTypeId,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCurrentDebugTarget);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentDebugTarget )( 
            __RPC__in IVsProjectCfgDebugTargetSelection * This,
            /* [in] */ GUID guidDebugTargetType,
            /* [in] */ DWORD debugTargetTypeId,
            /* [in] */ __RPC__in BSTR bstrCurrentDebugTarget);
        
        END_INTERFACE
    } IVsProjectCfgDebugTargetSelectionVtbl;

    interface IVsProjectCfgDebugTargetSelection
    {
        CONST_VTBL struct IVsProjectCfgDebugTargetSelectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectCfgDebugTargetSelection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectCfgDebugTargetSelection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectCfgDebugTargetSelection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectCfgDebugTargetSelection_HasDebugTargets(This,pDebugTargetSelectionService,pbstrSupportedTargetCommandIDs,pHasDebugTarget)	\
    ( (This)->lpVtbl -> HasDebugTargets(This,pDebugTargetSelectionService,pbstrSupportedTargetCommandIDs,pHasDebugTarget) ) 

#define IVsProjectCfgDebugTargetSelection_GetDebugTargetListOfType(This,guidDebugTargetType,debugTargetTypeId,pbstrDebugTargetListArray)	\
    ( (This)->lpVtbl -> GetDebugTargetListOfType(This,guidDebugTargetType,debugTargetTypeId,pbstrDebugTargetListArray) ) 

#define IVsProjectCfgDebugTargetSelection_GetCurrentDebugTarget(This,pguidDebugTargetType,pDebugTargetTypeId,pbstrCurrentDebugTarget)	\
    ( (This)->lpVtbl -> GetCurrentDebugTarget(This,pguidDebugTargetType,pDebugTargetTypeId,pbstrCurrentDebugTarget) ) 

#define IVsProjectCfgDebugTargetSelection_SetCurrentDebugTarget(This,guidDebugTargetType,debugTargetTypeId,bstrCurrentDebugTarget)	\
    ( (This)->lpVtbl -> SetCurrentDebugTarget(This,guidDebugTargetType,debugTargetTypeId,bstrCurrentDebugTarget) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectCfgDebugTargetSelection_INTERFACE_DEFINED__ */


#ifndef __SVsDebugTargetSelectionService_INTERFACE_DEFINED__
#define __SVsDebugTargetSelectionService_INTERFACE_DEFINED__

/* interface SVsDebugTargetSelectionService */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDebugTargetSelectionService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("89291C82-99BC-4FA3-98C0-802EEB2FBD70")
    SVsDebugTargetSelectionService : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsDebugTargetSelectionServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsDebugTargetSelectionService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsDebugTargetSelectionService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsDebugTargetSelectionService * This);
        
        END_INTERFACE
    } SVsDebugTargetSelectionServiceVtbl;

    interface SVsDebugTargetSelectionService
    {
        CONST_VTBL struct SVsDebugTargetSelectionServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDebugTargetSelectionService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDebugTargetSelectionService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDebugTargetSelectionService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDebugTargetSelectionService_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg110_0000_0018 */
/* [local] */ 

#define SID_SVsDebugTargetSelectionService IID_SVsDebugTargetSelectionService


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0018_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg110_0000_0018_v0_0_s_ifspec;

#ifndef __IVsProjectCfgDebugTypeSelection_INTERFACE_DEFINED__
#define __IVsProjectCfgDebugTypeSelection_INTERFACE_DEFINED__

/* interface IVsProjectCfgDebugTypeSelection */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProjectCfgDebugTypeSelection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1021A0EE-5E4E-4A9B-ACDA-B607FEF3AB65")
    IVsProjectCfgDebugTypeSelection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDebugTypes( 
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrDebugTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDebugTypeName( 
            /* [in] */ __RPC__in BSTR bstrDebugType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugTypeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentDebugType( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentDebugType( 
            /* [in] */ __RPC__in BSTR bstrDebugType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectCfgDebugTypeSelectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugTypes )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This,
            /* [out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrDebugTypes);
        
        HRESULT ( STDMETHODCALLTYPE *GetDebugTypeName )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This,
            /* [in] */ __RPC__in BSTR bstrDebugType,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugTypeName);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentDebugType )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDebugType);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentDebugType )( 
            __RPC__in IVsProjectCfgDebugTypeSelection * This,
            /* [in] */ __RPC__in BSTR bstrDebugType);
        
        END_INTERFACE
    } IVsProjectCfgDebugTypeSelectionVtbl;

    interface IVsProjectCfgDebugTypeSelection
    {
        CONST_VTBL struct IVsProjectCfgDebugTypeSelectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectCfgDebugTypeSelection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectCfgDebugTypeSelection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectCfgDebugTypeSelection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectCfgDebugTypeSelection_GetDebugTypes(This,pbstrDebugTypes)	\
    ( (This)->lpVtbl -> GetDebugTypes(This,pbstrDebugTypes) ) 

#define IVsProjectCfgDebugTypeSelection_GetDebugTypeName(This,bstrDebugType,pbstrDebugTypeName)	\
    ( (This)->lpVtbl -> GetDebugTypeName(This,bstrDebugType,pbstrDebugTypeName) ) 

#define IVsProjectCfgDebugTypeSelection_GetCurrentDebugType(This,pbstrDebugType)	\
    ( (This)->lpVtbl -> GetCurrentDebugType(This,pbstrDebugType) ) 

#define IVsProjectCfgDebugTypeSelection_SetCurrentDebugType(This,bstrDebugType)	\
    ( (This)->lpVtbl -> SetCurrentDebugType(This,bstrDebugType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectCfgDebugTypeSelection_INTERFACE_DEFINED__ */


#ifndef __IVsStrongNameKeys2_INTERFACE_DEFINED__
#define __IVsStrongNameKeys2_INTERFACE_DEFINED__

/* interface IVsStrongNameKeys2 */
/* [object][custom][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsStrongNameKeys2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ea4f4fec-d6ba-40d3-b536-823518822c9d")
    IVsStrongNameKeys2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateNewKeyWithNameAndSpecifiedSignatureAlgorithm( 
            /* [in] */ __RPC__in LPCSTR pszAlgorithmID,
            /* [in] */ DWORD dwKeyLength,
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword,
            /* [in] */ __RPC__in LPCOLESTR szSubjectName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNewKey2( 
            /* [in] */ __RPC__in LPCOLESTR szFileLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsStrongNameKeys2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsStrongNameKeys2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsStrongNameKeys2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsStrongNameKeys2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewKeyWithNameAndSpecifiedSignatureAlgorithm )( 
            __RPC__in IVsStrongNameKeys2 * This,
            /* [in] */ __RPC__in LPCSTR pszAlgorithmID,
            /* [in] */ DWORD dwKeyLength,
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword,
            /* [in] */ __RPC__in LPCOLESTR szSubjectName);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewKey2 )( 
            __RPC__in IVsStrongNameKeys2 * This,
            /* [in] */ __RPC__in LPCOLESTR szFileLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        END_INTERFACE
    } IVsStrongNameKeys2Vtbl;

    interface IVsStrongNameKeys2
    {
        CONST_VTBL struct IVsStrongNameKeys2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsStrongNameKeys2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsStrongNameKeys2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsStrongNameKeys2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsStrongNameKeys2_CreateNewKeyWithNameAndSpecifiedSignatureAlgorithm(This,pszAlgorithmID,dwKeyLength,szFile,szPassword,szSubjectName)	\
    ( (This)->lpVtbl -> CreateNewKeyWithNameAndSpecifiedSignatureAlgorithm(This,pszAlgorithmID,dwKeyLength,szFile,szPassword,szSubjectName) ) 

#define IVsStrongNameKeys2_CreateNewKey2(This,szFileLocation,pbstrFileName)	\
    ( (This)->lpVtbl -> CreateNewKey2(This,szFileLocation,pbstrFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsStrongNameKeys2_INTERFACE_DEFINED__ */


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


