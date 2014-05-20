

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

#ifndef __vsempweb_h__
#define __vsempweb_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsEmpWeb_FWD_DEFINED__
#define __IVsEmpWeb_FWD_DEFINED__
typedef interface IVsEmpWeb IVsEmpWeb;

#endif 	/* __IVsEmpWeb_FWD_DEFINED__ */


#ifndef __IVsEmpWebResource_FWD_DEFINED__
#define __IVsEmpWebResource_FWD_DEFINED__
typedef interface IVsEmpWebResource IVsEmpWebResource;

#endif 	/* __IVsEmpWebResource_FWD_DEFINED__ */


#ifndef __IEnumVsEmpWebResourceCheckouts_FWD_DEFINED__
#define __IEnumVsEmpWebResourceCheckouts_FWD_DEFINED__
typedef interface IEnumVsEmpWebResourceCheckouts IEnumVsEmpWebResourceCheckouts;

#endif 	/* __IEnumVsEmpWebResourceCheckouts_FWD_DEFINED__ */


#ifndef __IEnumVsEmpWebResource_FWD_DEFINED__
#define __IEnumVsEmpWebResource_FWD_DEFINED__
typedef interface IEnumVsEmpWebResource IEnumVsEmpWebResource;

#endif 	/* __IEnumVsEmpWebResource_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsempweb_0000_0000 */
/* [local] */ 





typedef /* [v1_enum] */ 
enum TagVsEmpWebResourceType
    {
        krtUnknown	= 0,
        krtFolder	= ( krtUnknown + 1 ) ,
        krtFile	= ( krtFolder + 1 ) 
    } 	eVsEmpWebResourceType;



extern RPC_IF_HANDLE __MIDL_itf_vsempweb_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsempweb_0000_0000_v0_0_s_ifspec;

#ifndef __IVsEmpWeb_INTERFACE_DEFINED__
#define __IVsEmpWeb_INTERFACE_DEFINED__

/* interface IVsEmpWeb */
/* [unique][uuid][object] */ 

typedef /* [v1_enum] */ 
enum TagVsEmpWebInfo
    {
        fIsolated	= 0x1
    } 	eVsEmpWebInfo;

typedef /* [v1_enum] */ 
enum TagVsEmpVersionType
    {
        kvtExplicit	= 0,
        kvtLocal	= ( kvtExplicit + 1 ) ,
        kvtTip	= ( kvtLocal + 1 ) 
    } 	eTagVsEmpVersionType;


EXTERN_C const IID IID_IVsEmpWeb;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3675ACF5-0C81-11d3-85C9-00A0C9CFCC16")
    IVsEmpWeb : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUrl( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSourceControlContext( 
            /* [in] */ __RPC__in LPCOLESTR szType,
            /* [in] */ __RPC__in LPCOLESTR szProvider,
            /* [in] */ __RPC__in LPCOLESTR szServer,
            /* [in] */ __RPC__in LPCOLESTR szPath,
            /* [in] */ __RPC__in LPCOLESTR szWorkarea,
            /* [in] */ boolean fAddExistingResources) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceControlContext( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrType,
            /* [out] */ __RPC__deref_out_opt BSTR *szProvider,
            /* [out] */ __RPC__deref_out_opt BSTR *szServer,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWorkarea) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearSourceControlContext( 
            /* [in] */ boolean fRemoveExistingResources) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResource( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumFolder( 
            /* [in] */ __RPC__in LPCOLESTR szUrlFolder,
            /* [in] */ boolean fRecurse,
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResource **ppIEnumVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PutResources( 
            /* [in] */ ULONG cResource,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszLocalPath,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszUrl,
            /* [out] */ __RPC__out ULONG *pcResourceActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetResources( 
            /* [in] */ ULONG cResource,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszUrl,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszLocalPath,
            /* [out] */ __RPC__out ULONG *pcResourceActual) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__out DWORD *pgrfWebInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOffline( 
            /* [in] */ boolean fOffline) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOffline( 
            /* [out] */ __RPC__out boolean *pfOffline) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetResourceVersioned( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ boolean fVersioned,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckoutResource( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ eTagVsEmpVersionType vt,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [in] */ boolean fExclusive,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UncheckoutResource( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckinResource( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ __RPC__in LPCOLESTR szComment,
            /* [in] */ boolean fKeepCheckedOut,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SyncResource( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ eTagVsEmpVersionType vt,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MergeAdvise( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseMerge( 
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownMergeResult,
            /* [in] */ __RPC__in LPCOLESTR szUrlReconciledContent,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResourceIsChanged( 
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__out boolean *pfLocallyChanged,
            /* [out] */ __RPC__out boolean *pfServerChanged) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEmpWebVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEmpWeb * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEmpWeb * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUrl )( 
            __RPC__in IVsEmpWeb * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *SetSourceControlContext )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szType,
            /* [in] */ __RPC__in LPCOLESTR szProvider,
            /* [in] */ __RPC__in LPCOLESTR szServer,
            /* [in] */ __RPC__in LPCOLESTR szPath,
            /* [in] */ __RPC__in LPCOLESTR szWorkarea,
            /* [in] */ boolean fAddExistingResources);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceControlContext )( 
            __RPC__in IVsEmpWeb * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrType,
            /* [out] */ __RPC__deref_out_opt BSTR *szProvider,
            /* [out] */ __RPC__deref_out_opt BSTR *szServer,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrPath,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWorkarea);
        
        HRESULT ( STDMETHODCALLTYPE *ClearSourceControlContext )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ boolean fRemoveExistingResources);
        
        HRESULT ( STDMETHODCALLTYPE *GetResource )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *EnumFolder )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrlFolder,
            /* [in] */ boolean fRecurse,
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResource **ppIEnumVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *PutResources )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ ULONG cResource,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszLocalPath,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszUrl,
            /* [out] */ __RPC__out ULONG *pcResourceActual);
        
        HRESULT ( STDMETHODCALLTYPE *GetResources )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ ULONG cResource,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszUrl,
            /* [size_is][in] */ __RPC__in_ecount_full(cResource) LPCOLESTR *rgszLocalPath,
            /* [out] */ __RPC__out ULONG *pcResourceActual);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            __RPC__in IVsEmpWeb * This,
            /* [out] */ __RPC__out DWORD *pgrfWebInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetOffline )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ boolean fOffline);
        
        HRESULT ( STDMETHODCALLTYPE *GetOffline )( 
            __RPC__in IVsEmpWeb * This,
            /* [out] */ __RPC__out boolean *pfOffline);
        
        HRESULT ( STDMETHODCALLTYPE *SetResourceVersioned )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ boolean fVersioned,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *CheckoutResource )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ eTagVsEmpVersionType vt,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [in] */ boolean fExclusive,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult);
        
        HRESULT ( STDMETHODCALLTYPE *UncheckoutResource )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *CheckinResource )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ __RPC__in LPCOLESTR szComment,
            /* [in] */ boolean fKeepCheckedOut,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult);
        
        HRESULT ( STDMETHODCALLTYPE *SyncResource )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ eTagVsEmpVersionType vt,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [in] */ __RPC__deref_in_opt LPCOLESTR *rgszMergeType,
            /* [in] */ UINT cMergeType,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnknownMergeResult);
        
        HRESULT ( STDMETHODCALLTYPE *MergeAdvise )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [in] */ __RPC__in LPCOLESTR szVersion,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseMerge )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in_opt IUnknown *pIUnknownMergeResult,
            /* [in] */ __RPC__in LPCOLESTR szUrlReconciledContent,
            /* [out] */ __RPC__deref_out_opt IVsEmpWebResource **ppIVsEmpWebResource);
        
        HRESULT ( STDMETHODCALLTYPE *ResourceIsChanged )( 
            __RPC__in IVsEmpWeb * This,
            /* [in] */ __RPC__in LPCOLESTR szUrl,
            /* [out] */ __RPC__out boolean *pfLocallyChanged,
            /* [out] */ __RPC__out boolean *pfServerChanged);
        
        END_INTERFACE
    } IVsEmpWebVtbl;

    interface IVsEmpWeb
    {
        CONST_VTBL struct IVsEmpWebVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEmpWeb_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEmpWeb_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEmpWeb_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEmpWeb_GetUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetUrl(This,pbstrUrl) ) 

#define IVsEmpWeb_SetSourceControlContext(This,szType,szProvider,szServer,szPath,szWorkarea,fAddExistingResources)	\
    ( (This)->lpVtbl -> SetSourceControlContext(This,szType,szProvider,szServer,szPath,szWorkarea,fAddExistingResources) ) 

#define IVsEmpWeb_GetSourceControlContext(This,pbstrType,szProvider,szServer,pbstrPath,pbstrWorkarea)	\
    ( (This)->lpVtbl -> GetSourceControlContext(This,pbstrType,szProvider,szServer,pbstrPath,pbstrWorkarea) ) 

#define IVsEmpWeb_ClearSourceControlContext(This,fRemoveExistingResources)	\
    ( (This)->lpVtbl -> ClearSourceControlContext(This,fRemoveExistingResources) ) 

#define IVsEmpWeb_GetResource(This,szUrl,ppIVsEmpWebResource)	\
    ( (This)->lpVtbl -> GetResource(This,szUrl,ppIVsEmpWebResource) ) 

#define IVsEmpWeb_EnumFolder(This,szUrlFolder,fRecurse,ppIEnumVsEmpWebResource)	\
    ( (This)->lpVtbl -> EnumFolder(This,szUrlFolder,fRecurse,ppIEnumVsEmpWebResource) ) 

#define IVsEmpWeb_PutResources(This,cResource,rgszLocalPath,rgszUrl,pcResourceActual)	\
    ( (This)->lpVtbl -> PutResources(This,cResource,rgszLocalPath,rgszUrl,pcResourceActual) ) 

#define IVsEmpWeb_GetResources(This,cResource,rgszUrl,rgszLocalPath,pcResourceActual)	\
    ( (This)->lpVtbl -> GetResources(This,cResource,rgszUrl,rgszLocalPath,pcResourceActual) ) 

#define IVsEmpWeb_GetInfo(This,pgrfWebInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,pgrfWebInfo) ) 

#define IVsEmpWeb_SetOffline(This,fOffline)	\
    ( (This)->lpVtbl -> SetOffline(This,fOffline) ) 

#define IVsEmpWeb_GetOffline(This,pfOffline)	\
    ( (This)->lpVtbl -> GetOffline(This,pfOffline) ) 

#define IVsEmpWeb_SetResourceVersioned(This,szUrl,fVersioned,ppIVsEmpWebResource)	\
    ( (This)->lpVtbl -> SetResourceVersioned(This,szUrl,fVersioned,ppIVsEmpWebResource) ) 

#define IVsEmpWeb_CheckoutResource(This,szUrl,vt,szVersion,fExclusive,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult)	\
    ( (This)->lpVtbl -> CheckoutResource(This,szUrl,vt,szVersion,fExclusive,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult) ) 

#define IVsEmpWeb_UncheckoutResource(This,szUrl,ppIVsEmpWebResource)	\
    ( (This)->lpVtbl -> UncheckoutResource(This,szUrl,ppIVsEmpWebResource) ) 

#define IVsEmpWeb_CheckinResource(This,szUrl,szComment,fKeepCheckedOut,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult)	\
    ( (This)->lpVtbl -> CheckinResource(This,szUrl,szComment,fKeepCheckedOut,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult) ) 

#define IVsEmpWeb_SyncResource(This,szUrl,vt,szVersion,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult)	\
    ( (This)->lpVtbl -> SyncResource(This,szUrl,vt,szVersion,rgszMergeType,cMergeType,ppIVsEmpWebResource,ppIUnknownMergeResult) ) 

#define IVsEmpWeb_MergeAdvise(This,szUrl,szVersion,ppIVsEmpWebResource)	\
    ( (This)->lpVtbl -> MergeAdvise(This,szUrl,szVersion,ppIVsEmpWebResource) ) 

#define IVsEmpWeb_AdviseMerge(This,pIUnknownMergeResult,szUrlReconciledContent,ppIVsEmpWebResource)	\
    ( (This)->lpVtbl -> AdviseMerge(This,pIUnknownMergeResult,szUrlReconciledContent,ppIVsEmpWebResource) ) 

#define IVsEmpWeb_ResourceIsChanged(This,szUrl,pfLocallyChanged,pfServerChanged)	\
    ( (This)->lpVtbl -> ResourceIsChanged(This,szUrl,pfLocallyChanged,pfServerChanged) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEmpWeb_INTERFACE_DEFINED__ */


#ifndef __IVsEmpWebResource_INTERFACE_DEFINED__
#define __IVsEmpWebResource_INTERFACE_DEFINED__

/* interface IVsEmpWebResource */
/* [unique][uuid][object] */ 

typedef /* [v1_enum] */ 
enum TagVsEmpWebResourceInfo2
    {
        fVsEmpWebResourceCheckedOut	= 0x1,
        fVsEmpWebResourceCheckedOutByMe	= 0x2,
        fVsEmpWebResourceCheckedOutExclusive	= 0x4,
        fVsEmpWebResourceVersioned	= 0x8,
        fVsEmpWebResourceDeletedLocally	= 0x10,
        fVsEmpWebResourceDeletedOnServer	= 0x20,
        fVsEmpWebResourceOutOfDate	= 0x40,
        fVsEmpWebResourceExcluded	= 0x80,
        fVsEmpWebResourceGhosted	= 0x100,
        fVsEmpWebResourceExists	= 0x200,
        fVsEmpWebResourceIsFolder	= 0x400,
        fVsEmpWebResourceReadOnly	= 0x800
    } 	eVsEmpWebResourceInfo2;


EXTERN_C const IID IID_IVsEmpWebResource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3675ACF6-0C81-11d3-85C9-00A0C9CFCC16")
    IVsEmpWebResource : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetType( 
            /* [out] */ __RPC__out eVsEmpWebResourceType *pert) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUrl( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumCheckouts( 
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResourceCheckouts **ppIEnumVsEmpWebResourceCheckouts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInfo( 
            /* [out] */ __RPC__out DWORD *pgrfResourceInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLocalVersion( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocalVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDefaultVersion( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLastModifiedDate( 
            /* [out] */ __RPC__out FILETIME *pftLastModified) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileSize( 
            /* [out] */ __RPC__out ULARGE_INTEGER *puliFileSize) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEmpWebResourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEmpWebResource * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEmpWebResource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEmpWebResource * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__out eVsEmpWebResourceType *pert);
        
        HRESULT ( STDMETHODCALLTYPE *GetUrl )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *EnumCheckouts )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResourceCheckouts **ppIEnumVsEmpWebResourceCheckouts);
        
        HRESULT ( STDMETHODCALLTYPE *GetInfo )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__out DWORD *pgrfResourceInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetLocalVersion )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrLocalVersion);
        
        HRESULT ( STDMETHODCALLTYPE *GetDefaultVersion )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDefaultVersion);
        
        HRESULT ( STDMETHODCALLTYPE *GetLastModifiedDate )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__out FILETIME *pftLastModified);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileSize )( 
            __RPC__in IVsEmpWebResource * This,
            /* [out] */ __RPC__out ULARGE_INTEGER *puliFileSize);
        
        END_INTERFACE
    } IVsEmpWebResourceVtbl;

    interface IVsEmpWebResource
    {
        CONST_VTBL struct IVsEmpWebResourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEmpWebResource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEmpWebResource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEmpWebResource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEmpWebResource_GetType(This,pert)	\
    ( (This)->lpVtbl -> GetType(This,pert) ) 

#define IVsEmpWebResource_GetUrl(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetUrl(This,pbstrUrl) ) 

#define IVsEmpWebResource_EnumCheckouts(This,ppIEnumVsEmpWebResourceCheckouts)	\
    ( (This)->lpVtbl -> EnumCheckouts(This,ppIEnumVsEmpWebResourceCheckouts) ) 

#define IVsEmpWebResource_GetInfo(This,pgrfResourceInfo)	\
    ( (This)->lpVtbl -> GetInfo(This,pgrfResourceInfo) ) 

#define IVsEmpWebResource_GetLocalVersion(This,pbstrLocalVersion)	\
    ( (This)->lpVtbl -> GetLocalVersion(This,pbstrLocalVersion) ) 

#define IVsEmpWebResource_GetDefaultVersion(This,pbstrDefaultVersion)	\
    ( (This)->lpVtbl -> GetDefaultVersion(This,pbstrDefaultVersion) ) 

#define IVsEmpWebResource_GetLastModifiedDate(This,pftLastModified)	\
    ( (This)->lpVtbl -> GetLastModifiedDate(This,pftLastModified) ) 

#define IVsEmpWebResource_GetFileSize(This,puliFileSize)	\
    ( (This)->lpVtbl -> GetFileSize(This,puliFileSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEmpWebResource_INTERFACE_DEFINED__ */


#ifndef __IEnumVsEmpWebResourceCheckouts_INTERFACE_DEFINED__
#define __IEnumVsEmpWebResourceCheckouts_INTERFACE_DEFINED__

/* interface IEnumVsEmpWebResourceCheckouts */
/* [unique][uuid][object] */ 

typedef struct tagCHECKOUTDATA
    {
    BSTR bstrCheckoutId;
    BSTR bstrOwner;
    FILETIME ftDate;
    BSTR bstrVersion;
    } 	CHECKOUTDATA;


EXTERN_C const IID IID_IEnumVsEmpWebResourceCheckouts;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ABA4462-0C9F-11d3-85C9-00A0C9CFCC16")
    IEnumVsEmpWebResourceCheckouts : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) CHECKOUTDATA *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResourceCheckouts **ppIEnumVsEmpWebResourceCheckouts) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumVsEmpWebResourceCheckoutsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) CHECKOUTDATA *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumVsEmpWebResourceCheckouts * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResourceCheckouts **ppIEnumVsEmpWebResourceCheckouts);
        
        END_INTERFACE
    } IEnumVsEmpWebResourceCheckoutsVtbl;

    interface IEnumVsEmpWebResourceCheckouts
    {
        CONST_VTBL struct IEnumVsEmpWebResourceCheckoutsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumVsEmpWebResourceCheckouts_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumVsEmpWebResourceCheckouts_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumVsEmpWebResourceCheckouts_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumVsEmpWebResourceCheckouts_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumVsEmpWebResourceCheckouts_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumVsEmpWebResourceCheckouts_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumVsEmpWebResourceCheckouts_Clone(This,ppIEnumVsEmpWebResourceCheckouts)	\
    ( (This)->lpVtbl -> Clone(This,ppIEnumVsEmpWebResourceCheckouts) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumVsEmpWebResourceCheckouts_INTERFACE_DEFINED__ */


#ifndef __IEnumVsEmpWebResource_INTERFACE_DEFINED__
#define __IEnumVsEmpWebResource_INTERFACE_DEFINED__

/* interface IEnumVsEmpWebResource */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IEnumVsEmpWebResource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ABA4461-0C9F-11d3-85C9-00A0C9CFCC16")
    IEnumVsEmpWebResource : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsEmpWebResource **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResource **ppIEnumVsEmpWebResource) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEnumVsEmpWebResourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IEnumVsEmpWebResource * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IEnumVsEmpWebResource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IEnumVsEmpWebResource * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IEnumVsEmpWebResource * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsEmpWebResource **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IEnumVsEmpWebResource * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IEnumVsEmpWebResource * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IEnumVsEmpWebResource * This,
            /* [out] */ __RPC__deref_out_opt IEnumVsEmpWebResource **ppIEnumVsEmpWebResource);
        
        END_INTERFACE
    } IEnumVsEmpWebResourceVtbl;

    interface IEnumVsEmpWebResource
    {
        CONST_VTBL struct IEnumVsEmpWebResourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumVsEmpWebResource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumVsEmpWebResource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumVsEmpWebResource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumVsEmpWebResource_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumVsEmpWebResource_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumVsEmpWebResource_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumVsEmpWebResource_Clone(This,ppIEnumVsEmpWebResource)	\
    ( (This)->lpVtbl -> Clone(This,ppIEnumVsEmpWebResource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumVsEmpWebResource_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


