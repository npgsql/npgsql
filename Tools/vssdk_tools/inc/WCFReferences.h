

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for WCFReferences.idl:
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


#ifndef __WCFReferences_h__
#define __WCFReferences_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsAddWebReferenceDlg3_FWD_DEFINED__
#define __IVsAddWebReferenceDlg3_FWD_DEFINED__
typedef interface IVsAddWebReferenceDlg3 IVsAddWebReferenceDlg3;
#endif 	/* __IVsAddWebReferenceDlg3_FWD_DEFINED__ */


#ifndef __SVsAddWebReferenceDlg3_FWD_DEFINED__
#define __SVsAddWebReferenceDlg3_FWD_DEFINED__
typedef interface SVsAddWebReferenceDlg3 SVsAddWebReferenceDlg3;
#endif 	/* __SVsAddWebReferenceDlg3_FWD_DEFINED__ */


#ifndef __IVsAddWebReferenceResult_FWD_DEFINED__
#define __IVsAddWebReferenceResult_FWD_DEFINED__
typedef interface IVsAddWebReferenceResult IVsAddWebReferenceResult;
#endif 	/* __IVsAddWebReferenceResult_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceManagerFactory_FWD_DEFINED__
#define __IVsWCFReferenceManagerFactory_FWD_DEFINED__
typedef interface IVsWCFReferenceManagerFactory IVsWCFReferenceManagerFactory;
#endif 	/* __IVsWCFReferenceManagerFactory_FWD_DEFINED__ */


#ifndef __SVsWCFReferenceManagerFactory_FWD_DEFINED__
#define __SVsWCFReferenceManagerFactory_FWD_DEFINED__
typedef interface SVsWCFReferenceManagerFactory SVsWCFReferenceManagerFactory;
#endif 	/* __SVsWCFReferenceManagerFactory_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceManager_FWD_DEFINED__
#define __IVsWCFReferenceManager_FWD_DEFINED__
typedef interface IVsWCFReferenceManager IVsWCFReferenceManager;
#endif 	/* __IVsWCFReferenceManager_FWD_DEFINED__ */


#ifndef __IVsWCFObject_FWD_DEFINED__
#define __IVsWCFObject_FWD_DEFINED__
typedef interface IVsWCFObject IVsWCFObject;
#endif 	/* __IVsWCFObject_FWD_DEFINED__ */


#ifndef __IVsWCFAsyncResult_FWD_DEFINED__
#define __IVsWCFAsyncResult_FWD_DEFINED__
typedef interface IVsWCFAsyncResult IVsWCFAsyncResult;
#endif 	/* __IVsWCFAsyncResult_FWD_DEFINED__ */


#ifndef __IVsWCFCompletionCallback_FWD_DEFINED__
#define __IVsWCFCompletionCallback_FWD_DEFINED__
typedef interface IVsWCFCompletionCallback IVsWCFCompletionCallback;
#endif 	/* __IVsWCFCompletionCallback_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceEvents_FWD_DEFINED__
#define __IVsWCFReferenceEvents_FWD_DEFINED__
typedef interface IVsWCFReferenceEvents IVsWCFReferenceEvents;
#endif 	/* __IVsWCFReferenceEvents_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceGroupCollection_FWD_DEFINED__
#define __IVsWCFReferenceGroupCollection_FWD_DEFINED__
typedef interface IVsWCFReferenceGroupCollection IVsWCFReferenceGroupCollection;
#endif 	/* __IVsWCFReferenceGroupCollection_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceGroup_FWD_DEFINED__
#define __IVsWCFReferenceGroup_FWD_DEFINED__
typedef interface IVsWCFReferenceGroup IVsWCFReferenceGroup;
#endif 	/* __IVsWCFReferenceGroup_FWD_DEFINED__ */


#ifndef __IEnumWCFReferenceGroupMetadataItems_FWD_DEFINED__
#define __IEnumWCFReferenceGroupMetadataItems_FWD_DEFINED__
typedef interface IEnumWCFReferenceGroupMetadataItems IEnumWCFReferenceGroupMetadataItems;
#endif 	/* __IEnumWCFReferenceGroupMetadataItems_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceGroupMetadataItem_FWD_DEFINED__
#define __IVsWCFReferenceGroupMetadataItem_FWD_DEFINED__
typedef interface IVsWCFReferenceGroupMetadataItem IVsWCFReferenceGroupMetadataItem;
#endif 	/* __IVsWCFReferenceGroupMetadataItem_FWD_DEFINED__ */


#ifndef __IEnumWCFReferenceContracts_FWD_DEFINED__
#define __IEnumWCFReferenceContracts_FWD_DEFINED__
typedef interface IEnumWCFReferenceContracts IEnumWCFReferenceContracts;
#endif 	/* __IEnumWCFReferenceContracts_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceContract_FWD_DEFINED__
#define __IVsWCFReferenceContract_FWD_DEFINED__
typedef interface IVsWCFReferenceContract IVsWCFReferenceContract;
#endif 	/* __IVsWCFReferenceContract_FWD_DEFINED__ */


#ifndef __IEnumWCFReferenceEndpoints_FWD_DEFINED__
#define __IEnumWCFReferenceEndpoints_FWD_DEFINED__
typedef interface IEnumWCFReferenceEndpoints IEnumWCFReferenceEndpoints;
#endif 	/* __IEnumWCFReferenceEndpoints_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceEndpoint_FWD_DEFINED__
#define __IVsWCFReferenceEndpoint_FWD_DEFINED__
typedef interface IVsWCFReferenceEndpoint IVsWCFReferenceEndpoint;
#endif 	/* __IVsWCFReferenceEndpoint_FWD_DEFINED__ */


#ifndef __IVsWCFReferenceGroupOptions_FWD_DEFINED__
#define __IVsWCFReferenceGroupOptions_FWD_DEFINED__
typedef interface IVsWCFReferenceGroupOptions IVsWCFReferenceGroupOptions;
#endif 	/* __IVsWCFReferenceGroupOptions_FWD_DEFINED__ */


#ifndef __IVsWCFMetadataStorageProvider_FWD_DEFINED__
#define __IVsWCFMetadataStorageProvider_FWD_DEFINED__
typedef interface IVsWCFMetadataStorageProvider IVsWCFMetadataStorageProvider;
#endif 	/* __IVsWCFMetadataStorageProvider_FWD_DEFINED__ */


#ifndef __IVsEnumWCFMetadataStorages_FWD_DEFINED__
#define __IVsEnumWCFMetadataStorages_FWD_DEFINED__
typedef interface IVsEnumWCFMetadataStorages IVsEnumWCFMetadataStorages;
#endif 	/* __IVsEnumWCFMetadataStorages_FWD_DEFINED__ */


#ifndef __IVsWCFMetadataStorageProviderEvents_FWD_DEFINED__
#define __IVsWCFMetadataStorageProviderEvents_FWD_DEFINED__
typedef interface IVsWCFMetadataStorageProviderEvents IVsWCFMetadataStorageProviderEvents;
#endif 	/* __IVsWCFMetadataStorageProviderEvents_FWD_DEFINED__ */


#ifndef __IVsWCFMetadataStorage_FWD_DEFINED__
#define __IVsWCFMetadataStorage_FWD_DEFINED__
typedef interface IVsWCFMetadataStorage IVsWCFMetadataStorage;
#endif 	/* __IVsWCFMetadataStorage_FWD_DEFINED__ */


#ifndef __IVsWCFMetadataStorageEvents_FWD_DEFINED__
#define __IVsWCFMetadataStorageEvents_FWD_DEFINED__
typedef interface IVsWCFMetadataStorageEvents IVsWCFMetadataStorageEvents;
#endif 	/* __IVsWCFMetadataStorageEvents_FWD_DEFINED__ */


#ifndef __IVsASMXMetadataStorageProvider_FWD_DEFINED__
#define __IVsASMXMetadataStorageProvider_FWD_DEFINED__
typedef interface IVsASMXMetadataStorageProvider IVsASMXMetadataStorageProvider;
#endif 	/* __IVsASMXMetadataStorageProvider_FWD_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageProvider_FWD_DEFINED__
#define __IVsServiceReferenceMetadataStorageProvider_FWD_DEFINED__
typedef interface IVsServiceReferenceMetadataStorageProvider IVsServiceReferenceMetadataStorageProvider;
#endif 	/* __IVsServiceReferenceMetadataStorageProvider_FWD_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageProviderEvents_FWD_DEFINED__
#define __IVsServiceReferenceMetadataStorageProviderEvents_FWD_DEFINED__
typedef interface IVsServiceReferenceMetadataStorageProviderEvents IVsServiceReferenceMetadataStorageProviderEvents;
#endif 	/* __IVsServiceReferenceMetadataStorageProviderEvents_FWD_DEFINED__ */


#ifndef __IVsEnumServiceReferenceMetadataStorages_FWD_DEFINED__
#define __IVsEnumServiceReferenceMetadataStorages_FWD_DEFINED__
typedef interface IVsEnumServiceReferenceMetadataStorages IVsEnumServiceReferenceMetadataStorages;
#endif 	/* __IVsEnumServiceReferenceMetadataStorages_FWD_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorage_FWD_DEFINED__
#define __IVsServiceReferenceMetadataStorage_FWD_DEFINED__
typedef interface IVsServiceReferenceMetadataStorage IVsServiceReferenceMetadataStorage;
#endif 	/* __IVsServiceReferenceMetadataStorage_FWD_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageEvents_FWD_DEFINED__
#define __IVsServiceReferenceMetadataStorageEvents_FWD_DEFINED__
typedef interface IVsServiceReferenceMetadataStorageEvents IVsServiceReferenceMetadataStorageEvents;
#endif 	/* __IVsServiceReferenceMetadataStorageEvents_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "oleipc.h"
#include "vsshell.h"
#include "discoveryservice90.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_WCFReferences_0000_0000 */
/* [local] */ 
































extern RPC_IF_HANDLE __MIDL_itf_WCFReferences_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WCFReferences_0000_0000_v0_0_s_ifspec;


#ifndef __WCFReferencesLib_LIBRARY_DEFINED__
#define __WCFReferencesLib_LIBRARY_DEFINED__

/* library WCFReferencesLib */
/* [helpstring][version][uuid] */ 


enum ServiceReferenceType
    {	SRT_ASMXReference	= 0x1,
	SRT_WCFReference	= 0x2
    } ;
typedef enum ServiceReferenceType ServiceReferenceType;

#define SID_SVsAddWebReferenceDlg3 IID_SVsAddWebReferenceDlg3
#define SID_SVsWCFReferenceManagerFactory IID_SVsWCFReferenceManagerFactory
typedef 
enum ProxySerializerType
    {	PST_Auto	= 0,
	PST_DataContractSerializer	= 1,
	PST_XmlSerializer	= 2
    } 	ProxySerializerType;

typedef 
enum CollectionCategory
    {	CC_Unknown	= 0,
	CC_List	= 1,
	CC_Dictionary	= 2
    } 	CollectionCategory;


enum StorageNameValidationState
    {	SNVS_NewNamespace	= 0,
	SNVS_InvalidNamespace	= 0x1,
	SNVS_UnsupportedNamespace	= 0x2,
	SNVS_ExistingNamespace	= 0x3,
	SNVS_InvalidReferenceName	= 0x4,
	SNVS_ReferenceNameConflicts	= 0x5
    } ;
typedef enum StorageNameValidationState StorageNameValidationState;


EXTERN_C const IID LIBID_WCFReferencesLib;

#ifndef __IVsAddWebReferenceDlg3_INTERFACE_DEFINED__
#define __IVsAddWebReferenceDlg3_INTERFACE_DEFINED__

/* interface IVsAddWebReferenceDlg3 */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsAddWebReferenceDlg3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("46e27779-a3e6-484b-a0ee-15795b173ae6")
    IVsAddWebReferenceDlg3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ShowAddWebReferenceDialog( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pProject,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ ServiceReferenceType referenceTypesAllowed,
            /* [in] */ __RPC__in LPCOLESTR pszDialogName,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pExistingReferenceGroup,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceConfigContents,
            /* [out] */ __RPC__deref_out_opt IVsAddWebReferenceResult **ppReferenceResult,
            /* [out] */ __RPC__out BOOL *pfCancelled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowDiscoveredServicesInCurrentDialog( 
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR ServiceUrls[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR ServiceDisplayNames[  ],
            /* [in] */ __RPC__in LPCOLESTR pszStatusText,
            /* [in] */ __RPC__in LPCOLESTR pszErrorText) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAddWebReferenceDlg3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAddWebReferenceDlg3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAddWebReferenceDlg3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAddWebReferenceDlg3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *ShowAddWebReferenceDialog )( 
            IVsAddWebReferenceDlg3 * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pProject,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ ServiceReferenceType referenceTypesAllowed,
            /* [in] */ __RPC__in LPCOLESTR pszDialogName,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pExistingReferenceGroup,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceConfigContents,
            /* [out] */ __RPC__deref_out_opt IVsAddWebReferenceResult **ppReferenceResult,
            /* [out] */ __RPC__out BOOL *pfCancelled);
        
        HRESULT ( STDMETHODCALLTYPE *ShowDiscoveredServicesInCurrentDialog )( 
            IVsAddWebReferenceDlg3 * This,
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR ServiceUrls[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR ServiceDisplayNames[  ],
            /* [in] */ __RPC__in LPCOLESTR pszStatusText,
            /* [in] */ __RPC__in LPCOLESTR pszErrorText);
        
        END_INTERFACE
    } IVsAddWebReferenceDlg3Vtbl;

    interface IVsAddWebReferenceDlg3
    {
        CONST_VTBL struct IVsAddWebReferenceDlg3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddWebReferenceDlg3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddWebReferenceDlg3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddWebReferenceDlg3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddWebReferenceDlg3_ShowAddWebReferenceDialog(This,pProject,pDiscoverySession,referenceTypesAllowed,pszDialogName,pExistingReferenceGroup,pszReferenceConfigContents,ppReferenceResult,pfCancelled)	\
    ( (This)->lpVtbl -> ShowAddWebReferenceDialog(This,pProject,pDiscoverySession,referenceTypesAllowed,pszDialogName,pExistingReferenceGroup,pszReferenceConfigContents,ppReferenceResult,pfCancelled) ) 

#define IVsAddWebReferenceDlg3_ShowDiscoveredServicesInCurrentDialog(This,cItems,ServiceUrls,ServiceDisplayNames,pszStatusText,pszErrorText)	\
    ( (This)->lpVtbl -> ShowDiscoveredServicesInCurrentDialog(This,cItems,ServiceUrls,ServiceDisplayNames,pszStatusText,pszErrorText) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddWebReferenceDlg3_INTERFACE_DEFINED__ */


#ifndef __SVsAddWebReferenceDlg3_INTERFACE_DEFINED__
#define __SVsAddWebReferenceDlg3_INTERFACE_DEFINED__

/* interface SVsAddWebReferenceDlg3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsAddWebReferenceDlg3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("32d47224-21a6-4d8f-9223-c91f0d69c501")
    SVsAddWebReferenceDlg3 : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsAddWebReferenceDlg3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsAddWebReferenceDlg3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsAddWebReferenceDlg3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsAddWebReferenceDlg3 * This);
        
        END_INTERFACE
    } SVsAddWebReferenceDlg3Vtbl;

    interface SVsAddWebReferenceDlg3
    {
        CONST_VTBL struct SVsAddWebReferenceDlg3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsAddWebReferenceDlg3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsAddWebReferenceDlg3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsAddWebReferenceDlg3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsAddWebReferenceDlg3_INTERFACE_DEFINED__ */


#ifndef __IVsAddWebReferenceResult_INTERFACE_DEFINED__
#define __IVsAddWebReferenceResult_INTERFACE_DEFINED__

/* interface IVsAddWebReferenceResult */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsAddWebReferenceResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4b85c5ef-9089-4e4c-a310-325bf87baf23")
    IVsAddWebReferenceResult : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferenceUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceType( 
            /* [retval][out] */ __RPC__out ServiceReferenceType *pType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDetail( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppWCFReferenceGroupDetails) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAddWebReferenceResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAddWebReferenceResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAddWebReferenceResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAddWebReferenceResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceUrl )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceNamespace )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceName )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceName);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceType )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__out ServiceReferenceType *pType);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *GetDetail )( 
            IVsAddWebReferenceResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppWCFReferenceGroupDetails);
        
        END_INTERFACE
    } IVsAddWebReferenceResultVtbl;

    interface IVsAddWebReferenceResult
    {
        CONST_VTBL struct IVsAddWebReferenceResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddWebReferenceResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddWebReferenceResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddWebReferenceResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddWebReferenceResult_GetReferenceUrl(This,pbstrWebReferenceUrl)	\
    ( (This)->lpVtbl -> GetReferenceUrl(This,pbstrWebReferenceUrl) ) 

#define IVsAddWebReferenceResult_GetReferenceNamespace(This,pbstrReferenceNamespace)	\
    ( (This)->lpVtbl -> GetReferenceNamespace(This,pbstrReferenceNamespace) ) 

#define IVsAddWebReferenceResult_GetReferenceName(This,pbstrReferenceName)	\
    ( (This)->lpVtbl -> GetReferenceName(This,pbstrReferenceName) ) 

#define IVsAddWebReferenceResult_GetReferenceType(This,pType)	\
    ( (This)->lpVtbl -> GetReferenceType(This,pType) ) 

#define IVsAddWebReferenceResult_Save(This,ppReferenceGroup)	\
    ( (This)->lpVtbl -> Save(This,ppReferenceGroup) ) 

#define IVsAddWebReferenceResult_GetDetail(This,ppWCFReferenceGroupDetails)	\
    ( (This)->lpVtbl -> GetDetail(This,ppWCFReferenceGroupDetails) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddWebReferenceResult_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceManagerFactory_INTERFACE_DEFINED__
#define __IVsWCFReferenceManagerFactory_INTERFACE_DEFINED__

/* interface IVsWCFReferenceManagerFactory */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceManagerFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2fe19f72-edd1-4fa4-9f36-a90a52ed166a")
    IVsWCFReferenceManagerFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferenceManager( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierProject,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceManager **pIVsWCFReferenceManager) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReferenceManagerSupported( 
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierProject,
            /* [retval][out] */ __RPC__out BOOL *pIsSupported) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceManagerFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceManagerFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceManagerFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceManagerFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceManager )( 
            IVsWCFReferenceManagerFactory * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierProject,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceManager **pIVsWCFReferenceManager);
        
        HRESULT ( STDMETHODCALLTYPE *IsReferenceManagerSupported )( 
            IVsWCFReferenceManagerFactory * This,
            /* [in] */ __RPC__in_opt IVsHierarchy *pHierProject,
            /* [retval][out] */ __RPC__out BOOL *pIsSupported);
        
        END_INTERFACE
    } IVsWCFReferenceManagerFactoryVtbl;

    interface IVsWCFReferenceManagerFactory
    {
        CONST_VTBL struct IVsWCFReferenceManagerFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceManagerFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceManagerFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceManagerFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceManagerFactory_GetReferenceManager(This,pHierProject,pIVsWCFReferenceManager)	\
    ( (This)->lpVtbl -> GetReferenceManager(This,pHierProject,pIVsWCFReferenceManager) ) 

#define IVsWCFReferenceManagerFactory_IsReferenceManagerSupported(This,pHierProject,pIsSupported)	\
    ( (This)->lpVtbl -> IsReferenceManagerSupported(This,pHierProject,pIsSupported) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceManagerFactory_INTERFACE_DEFINED__ */


#ifndef __SVsWCFReferenceManagerFactory_INTERFACE_DEFINED__
#define __SVsWCFReferenceManagerFactory_INTERFACE_DEFINED__

/* interface SVsWCFReferenceManagerFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsWCFReferenceManagerFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DDD04E59-6B86-4a73-8BC7-3FF5D7B1111C")
    SVsWCFReferenceManagerFactory : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsWCFReferenceManagerFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsWCFReferenceManagerFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsWCFReferenceManagerFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsWCFReferenceManagerFactory * This);
        
        END_INTERFACE
    } SVsWCFReferenceManagerFactoryVtbl;

    interface SVsWCFReferenceManagerFactory
    {
        CONST_VTBL struct SVsWCFReferenceManagerFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsWCFReferenceManagerFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsWCFReferenceManagerFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsWCFReferenceManagerFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsWCFReferenceManagerFactory_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceManager_INTERFACE_DEFINED__
#define __IVsWCFReferenceManager_INTERFACE_DEFINED__

/* interface IVsWCFReferenceManager */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2349F2F8-D2D5-4268-898C-35F1F013426D")
    IVsWCFReferenceManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferenceGroupCollection( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupCollection **pWCFReferenceGroupCollection) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceEndpointEnumerator( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseWCFReferenceEvents( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseWCFReferenceEvents( 
            /* [in] */ VSCOOKIE cookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceGroupCollection )( 
            IVsWCFReferenceManager * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupCollection **pWCFReferenceGroupCollection);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceEndpointEnumerator )( 
            IVsWCFReferenceManager * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseWCFReferenceEvents )( 
            IVsWCFReferenceManager * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseWCFReferenceEvents )( 
            IVsWCFReferenceManager * This,
            /* [in] */ VSCOOKIE cookie);
        
        END_INTERFACE
    } IVsWCFReferenceManagerVtbl;

    interface IVsWCFReferenceManager
    {
        CONST_VTBL struct IVsWCFReferenceManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceManager_GetReferenceGroupCollection(This,pWCFReferenceGroupCollection)	\
    ( (This)->lpVtbl -> GetReferenceGroupCollection(This,pWCFReferenceGroupCollection) ) 

#define IVsWCFReferenceManager_GetReferenceEndpointEnumerator(This,ppEnum)	\
    ( (This)->lpVtbl -> GetReferenceEndpointEnumerator(This,ppEnum) ) 

#define IVsWCFReferenceManager_AdviseWCFReferenceEvents(This,pSink,pCookie)	\
    ( (This)->lpVtbl -> AdviseWCFReferenceEvents(This,pSink,pCookie) ) 

#define IVsWCFReferenceManager_UnadviseWCFReferenceEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadviseWCFReferenceEvents(This,cookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceManager_INTERFACE_DEFINED__ */


#ifndef __IVsWCFObject_INTERFACE_DEFINED__
#define __IVsWCFObject_INTERFACE_DEFINED__

/* interface IVsWCFObject */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5E1F02CC-7A5F-4db0-8D55-A05CC0A4AA56")
    IVsWCFObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsValid( 
            /* [retval][out] */ __RPC__out BOOL *pbIsValid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reload( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsValid )( 
            IVsWCFObject * This,
            /* [retval][out] */ __RPC__out BOOL *pbIsValid);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsWCFObject * This);
        
        END_INTERFACE
    } IVsWCFObjectVtbl;

    interface IVsWCFObject
    {
        CONST_VTBL struct IVsWCFObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFObject_IsValid(This,pbIsValid)	\
    ( (This)->lpVtbl -> IsValid(This,pbIsValid) ) 

#define IVsWCFObject_Reload(This)	\
    ( (This)->lpVtbl -> Reload(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFObject_INTERFACE_DEFINED__ */


#ifndef __IVsWCFAsyncResult_INTERFACE_DEFINED__
#define __IVsWCFAsyncResult_INTERFACE_DEFINED__

/* interface IVsWCFAsyncResult */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFAsyncResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("70D6D8A3-1D8D-44e8-8CD8-64B047E9F309")
    IVsWCFAsyncResult : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsCompleted( 
            /* [retval][out] */ __RPC__out BOOL *pbIsCompleted) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsCancelled( 
            /* [retval][out] */ __RPC__out BOOL *pIsCancelled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomState( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunknownCustomState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMethodResult( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Cancel( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFAsyncResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFAsyncResult * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFAsyncResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFAsyncResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsCompleted )( 
            IVsWCFAsyncResult * This,
            /* [retval][out] */ __RPC__out BOOL *pbIsCompleted);
        
        HRESULT ( STDMETHODCALLTYPE *IsCancelled )( 
            IVsWCFAsyncResult * This,
            /* [retval][out] */ __RPC__out BOOL *pIsCancelled);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomState )( 
            IVsWCFAsyncResult * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunknownCustomState);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodResult )( 
            IVsWCFAsyncResult * This);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVsWCFAsyncResult * This);
        
        END_INTERFACE
    } IVsWCFAsyncResultVtbl;

    interface IVsWCFAsyncResult
    {
        CONST_VTBL struct IVsWCFAsyncResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFAsyncResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFAsyncResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFAsyncResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFAsyncResult_IsCompleted(This,pbIsCompleted)	\
    ( (This)->lpVtbl -> IsCompleted(This,pbIsCompleted) ) 

#define IVsWCFAsyncResult_IsCancelled(This,pIsCancelled)	\
    ( (This)->lpVtbl -> IsCancelled(This,pIsCancelled) ) 

#define IVsWCFAsyncResult_GetCustomState(This,ppunknownCustomState)	\
    ( (This)->lpVtbl -> GetCustomState(This,ppunknownCustomState) ) 

#define IVsWCFAsyncResult_GetMethodResult(This)	\
    ( (This)->lpVtbl -> GetMethodResult(This) ) 

#define IVsWCFAsyncResult_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFAsyncResult_INTERFACE_DEFINED__ */


#ifndef __IVsWCFCompletionCallback_INTERFACE_DEFINED__
#define __IVsWCFCompletionCallback_INTERFACE_DEFINED__

/* interface IVsWCFCompletionCallback */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFCompletionCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("183F0821-1F02-4e07-901A-F4F19C162935")
    IVsWCFCompletionCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnFinished( 
            /* [in] */ __RPC__in_opt IVsWCFAsyncResult *pResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFCompletionCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFCompletionCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFCompletionCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFCompletionCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnFinished )( 
            IVsWCFCompletionCallback * This,
            /* [in] */ __RPC__in_opt IVsWCFAsyncResult *pResult);
        
        END_INTERFACE
    } IVsWCFCompletionCallbackVtbl;

    interface IVsWCFCompletionCallback
    {
        CONST_VTBL struct IVsWCFCompletionCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFCompletionCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFCompletionCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFCompletionCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFCompletionCallback_OnFinished(This,pResult)	\
    ( (This)->lpVtbl -> OnFinished(This,pResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFCompletionCallback_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceEvents_INTERFACE_DEFINED__
#define __IVsWCFReferenceEvents_INTERFACE_DEFINED__

/* interface IVsWCFReferenceEvents */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("729D5091-E77F-4d0b-B03A-2310AD58DDC2")
    IVsWCFReferenceEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnReferenceGroupCollectionChanging( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReferenceGroupCollectionChanged( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataChanging( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataChanged( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReferenceGroupPropertiesChanging( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReferenceGroupPropertiesChanged( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnConfigurationChanged( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnReferenceGroupCollectionChanging )( 
            IVsWCFReferenceEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnReferenceGroupCollectionChanged )( 
            IVsWCFReferenceEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataChanging )( 
            IVsWCFReferenceEvents * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataChanged )( 
            IVsWCFReferenceEvents * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *OnReferenceGroupPropertiesChanging )( 
            IVsWCFReferenceEvents * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *OnReferenceGroupPropertiesChanged )( 
            IVsWCFReferenceEvents * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *OnConfigurationChanged )( 
            IVsWCFReferenceEvents * This);
        
        END_INTERFACE
    } IVsWCFReferenceEventsVtbl;

    interface IVsWCFReferenceEvents
    {
        CONST_VTBL struct IVsWCFReferenceEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceEvents_OnReferenceGroupCollectionChanging(This)	\
    ( (This)->lpVtbl -> OnReferenceGroupCollectionChanging(This) ) 

#define IVsWCFReferenceEvents_OnReferenceGroupCollectionChanged(This)	\
    ( (This)->lpVtbl -> OnReferenceGroupCollectionChanged(This) ) 

#define IVsWCFReferenceEvents_OnMetadataChanging(This,pReferenceGroup)	\
    ( (This)->lpVtbl -> OnMetadataChanging(This,pReferenceGroup) ) 

#define IVsWCFReferenceEvents_OnMetadataChanged(This,pReferenceGroup)	\
    ( (This)->lpVtbl -> OnMetadataChanged(This,pReferenceGroup) ) 

#define IVsWCFReferenceEvents_OnReferenceGroupPropertiesChanging(This,pReferenceGroup)	\
    ( (This)->lpVtbl -> OnReferenceGroupPropertiesChanging(This,pReferenceGroup) ) 

#define IVsWCFReferenceEvents_OnReferenceGroupPropertiesChanged(This,pReferenceGroup)	\
    ( (This)->lpVtbl -> OnReferenceGroupPropertiesChanged(This,pReferenceGroup) ) 

#define IVsWCFReferenceEvents_OnConfigurationChanged(This)	\
    ( (This)->lpVtbl -> OnConfigurationChanged(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceEvents_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceGroupCollection_INTERFACE_DEFINED__
#define __IVsWCFReferenceGroupCollection_INTERFACE_DEFINED__

/* interface IVsWCFReferenceGroupCollection */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceGroupCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("343173D2-F910-4c03-930D-16AB1568431B")
    IVsWCFReferenceGroupCollection : public IVsWCFObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ContainingProject( 
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **ppHierProject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateAll( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Count( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pWCFReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAt( 
            /* [in] */ long index) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceGroupByName( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceGroupFromMapFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceGroupCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceGroupCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceGroupCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsValid )( 
            IVsWCFReferenceGroupCollection * This,
            /* [retval][out] */ __RPC__out BOOL *pbIsValid);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsWCFReferenceGroupCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *ContainingProject )( 
            IVsWCFReferenceGroupCollection * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsHierarchy **ppHierProject);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateAll )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession);
        
        HRESULT ( STDMETHODCALLTYPE *Count )( 
            IVsWCFReferenceGroupCollection * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        HRESULT ( STDMETHODCALLTYPE *Item )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroup *pWCFReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAt )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ long index);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceGroupByName )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceGroupFromMapFile )( 
            IVsWCFReferenceGroupCollection * This,
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppWCFReferenceGroup);
        
        END_INTERFACE
    } IVsWCFReferenceGroupCollectionVtbl;

    interface IVsWCFReferenceGroupCollection
    {
        CONST_VTBL struct IVsWCFReferenceGroupCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceGroupCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceGroupCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceGroupCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceGroupCollection_IsValid(This,pbIsValid)	\
    ( (This)->lpVtbl -> IsValid(This,pbIsValid) ) 

#define IVsWCFReferenceGroupCollection_Reload(This)	\
    ( (This)->lpVtbl -> Reload(This) ) 


#define IVsWCFReferenceGroupCollection_ContainingProject(This,ppHierProject)	\
    ( (This)->lpVtbl -> ContainingProject(This,ppHierProject) ) 

#define IVsWCFReferenceGroupCollection_Add(This,pszNamespace,pszName,ppWCFReferenceGroup)	\
    ( (This)->lpVtbl -> Add(This,pszNamespace,pszName,ppWCFReferenceGroup) ) 

#define IVsWCFReferenceGroupCollection_UpdateAll(This,pDiscoverySession)	\
    ( (This)->lpVtbl -> UpdateAll(This,pDiscoverySession) ) 

#define IVsWCFReferenceGroupCollection_Count(This,plCount)	\
    ( (This)->lpVtbl -> Count(This,plCount) ) 

#define IVsWCFReferenceGroupCollection_Item(This,index,ppWCFReferenceGroup)	\
    ( (This)->lpVtbl -> Item(This,index,ppWCFReferenceGroup) ) 

#define IVsWCFReferenceGroupCollection_Remove(This,pWCFReferenceGroup)	\
    ( (This)->lpVtbl -> Remove(This,pWCFReferenceGroup) ) 

#define IVsWCFReferenceGroupCollection_RemoveAt(This,index)	\
    ( (This)->lpVtbl -> RemoveAt(This,index) ) 

#define IVsWCFReferenceGroupCollection_GetReferenceGroupByName(This,pszNamespace,pszName,ppWCFReferenceGroup)	\
    ( (This)->lpVtbl -> GetReferenceGroupByName(This,pszNamespace,pszName,ppWCFReferenceGroup) ) 

#define IVsWCFReferenceGroupCollection_GetReferenceGroupFromMapFile(This,pszMapFilePath,ppWCFReferenceGroup)	\
    ( (This)->lpVtbl -> GetReferenceGroupFromMapFile(This,pszMapFilePath,ppWCFReferenceGroup) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceGroupCollection_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceGroup_INTERFACE_DEFINED__
#define __IVsWCFReferenceGroup_INTERFACE_DEFINED__

/* interface IVsWCFReferenceGroup */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceGroup;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("806cfc9a-2476-4d28-a5b2-c9ebc617b24e")
    IVsWCFReferenceGroup : public IVsWCFObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrGUID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOptions( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupOptions **ppOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOptions( 
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroupOptions *pOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCollection( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupCollection **ppCollection) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddReference( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in LPCOLESTR pszUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AsyncAddReference( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in LPCOLESTR pszUrl,
            /* [in] */ __RPC__in_opt IVsWCFCompletionCallback *pCallback,
            /* [in] */ __RPC__in_opt IUnknown *punknownCustomState,
            /* [out] */ __RPC__deref_out_opt IVsWCFAsyncResult **ppResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Update( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AsyncUpdate( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in_opt IVsWCFCompletionCallback *pCallback,
            /* [in] */ __RPC__in_opt IUnknown *punknownCustomState,
            /* [out] */ __RPC__deref_out_opt IVsWCFAsyncResult **ppResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowOptionsDialog( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetName( 
            /* [in] */ __RPC__in LPCOLESTR pszName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNamespace( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProxyNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProxyNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMapFileProjectItem( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **pProjectItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileCodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetadataItemsEnumerator( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceGroupMetadataItems **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtensionData( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExtensionData( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ __RPC__in SAFEARRAY * content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContractsEnumerator( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceContracts **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceCount( 
            /* [retval][out] */ __RPC__out long *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceUrl( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetReferenceUrl( 
            /* [in] */ long index,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteReference( 
            /* [in] */ long index) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateConfiguration( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBatch( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceGroupVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceGroup * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceGroup * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsValid )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__out BOOL *pbIsValid);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVsWCFReferenceGroup * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetID )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrGUID);
        
        HRESULT ( STDMETHODCALLTYPE *GetOptions )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupOptions **ppOptions);
        
        HRESULT ( STDMETHODCALLTYPE *SetOptions )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in_opt IVsWCFReferenceGroupOptions *pOptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetCollection )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroupCollection **ppCollection);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in LPCOLESTR pszUrl);
        
        HRESULT ( STDMETHODCALLTYPE *AsyncAddReference )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in LPCOLESTR pszUrl,
            /* [in] */ __RPC__in_opt IVsWCFCompletionCallback *pCallback,
            /* [in] */ __RPC__in_opt IUnknown *punknownCustomState,
            /* [out] */ __RPC__deref_out_opt IVsWCFAsyncResult **ppResult);
        
        HRESULT ( STDMETHODCALLTYPE *Update )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession);
        
        HRESULT ( STDMETHODCALLTYPE *AsyncUpdate )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [in] */ __RPC__in_opt IVsWCFCompletionCallback *pCallback,
            /* [in] */ __RPC__in_opt IUnknown *punknownCustomState,
            /* [out] */ __RPC__deref_out_opt IVsWCFAsyncResult **ppResult);
        
        HRESULT ( STDMETHODCALLTYPE *ShowOptionsDialog )( 
            IVsWCFReferenceGroup * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetName )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *GetNamespace )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *SetNamespace )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *GetProxyNamespace )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProxyNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *GetMapFileProjectItem )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **pProjectItem);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileCodeModel )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataItemsEnumerator )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceGroupMetadataItems **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtensionData )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *content);
        
        HRESULT ( STDMETHODCALLTYPE *SetExtensionData )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [in] */ __RPC__in SAFEARRAY * content);
        
        HRESULT ( STDMETHODCALLTYPE *GetContractsEnumerator )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceContracts **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceCount )( 
            IVsWCFReferenceGroup * This,
            /* [retval][out] */ __RPC__out long *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceUrl )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReferenceUrl);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceUrl )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ long index,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceUrl);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteReference )( 
            IVsWCFReferenceGroup * This,
            /* [in] */ long index);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateConfiguration )( 
            IVsWCFReferenceGroup * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginBatch )( 
            IVsWCFReferenceGroup * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBatch )( 
            IVsWCFReferenceGroup * This);
        
        END_INTERFACE
    } IVsWCFReferenceGroupVtbl;

    interface IVsWCFReferenceGroup
    {
        CONST_VTBL struct IVsWCFReferenceGroupVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceGroup_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceGroup_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceGroup_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceGroup_IsValid(This,pbIsValid)	\
    ( (This)->lpVtbl -> IsValid(This,pbIsValid) ) 

#define IVsWCFReferenceGroup_Reload(This)	\
    ( (This)->lpVtbl -> Reload(This) ) 


#define IVsWCFReferenceGroup_GetID(This,pbstrGUID)	\
    ( (This)->lpVtbl -> GetID(This,pbstrGUID) ) 

#define IVsWCFReferenceGroup_GetOptions(This,ppOptions)	\
    ( (This)->lpVtbl -> GetOptions(This,ppOptions) ) 

#define IVsWCFReferenceGroup_SetOptions(This,pOptions)	\
    ( (This)->lpVtbl -> SetOptions(This,pOptions) ) 

#define IVsWCFReferenceGroup_GetCollection(This,ppCollection)	\
    ( (This)->lpVtbl -> GetCollection(This,ppCollection) ) 

#define IVsWCFReferenceGroup_AddReference(This,pDiscoverySession,pszUrl)	\
    ( (This)->lpVtbl -> AddReference(This,pDiscoverySession,pszUrl) ) 

#define IVsWCFReferenceGroup_AsyncAddReference(This,pDiscoverySession,pszUrl,pCallback,punknownCustomState,ppResult)	\
    ( (This)->lpVtbl -> AsyncAddReference(This,pDiscoverySession,pszUrl,pCallback,punknownCustomState,ppResult) ) 

#define IVsWCFReferenceGroup_Update(This,pDiscoverySession)	\
    ( (This)->lpVtbl -> Update(This,pDiscoverySession) ) 

#define IVsWCFReferenceGroup_AsyncUpdate(This,pDiscoverySession,pCallback,punknownCustomState,ppResult)	\
    ( (This)->lpVtbl -> AsyncUpdate(This,pDiscoverySession,pCallback,punknownCustomState,ppResult) ) 

#define IVsWCFReferenceGroup_ShowOptionsDialog(This)	\
    ( (This)->lpVtbl -> ShowOptionsDialog(This) ) 

#define IVsWCFReferenceGroup_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsWCFReferenceGroup_SetName(This,pszName)	\
    ( (This)->lpVtbl -> SetName(This,pszName) ) 

#define IVsWCFReferenceGroup_GetNamespace(This,pbstrNamespace)	\
    ( (This)->lpVtbl -> GetNamespace(This,pbstrNamespace) ) 

#define IVsWCFReferenceGroup_SetNamespace(This,pszNamespace)	\
    ( (This)->lpVtbl -> SetNamespace(This,pszNamespace) ) 

#define IVsWCFReferenceGroup_GetProxyNamespace(This,pbstrProxyNamespace)	\
    ( (This)->lpVtbl -> GetProxyNamespace(This,pbstrProxyNamespace) ) 

#define IVsWCFReferenceGroup_GetMapFileProjectItem(This,pProjectItem)	\
    ( (This)->lpVtbl -> GetMapFileProjectItem(This,pProjectItem) ) 

#define IVsWCFReferenceGroup_GetFileCodeModel(This,ppFileCodeModel)	\
    ( (This)->lpVtbl -> GetFileCodeModel(This,ppFileCodeModel) ) 

#define IVsWCFReferenceGroup_GetMetadataItemsEnumerator(This,ppenum)	\
    ( (This)->lpVtbl -> GetMetadataItemsEnumerator(This,ppenum) ) 

#define IVsWCFReferenceGroup_GetExtensionData(This,name,content)	\
    ( (This)->lpVtbl -> GetExtensionData(This,name,content) ) 

#define IVsWCFReferenceGroup_SetExtensionData(This,name,content)	\
    ( (This)->lpVtbl -> SetExtensionData(This,name,content) ) 

#define IVsWCFReferenceGroup_GetContractsEnumerator(This,ppenum)	\
    ( (This)->lpVtbl -> GetContractsEnumerator(This,ppenum) ) 

#define IVsWCFReferenceGroup_GetReferenceCount(This,pCount)	\
    ( (This)->lpVtbl -> GetReferenceCount(This,pCount) ) 

#define IVsWCFReferenceGroup_GetReferenceUrl(This,index,pbstrReferenceUrl)	\
    ( (This)->lpVtbl -> GetReferenceUrl(This,index,pbstrReferenceUrl) ) 

#define IVsWCFReferenceGroup_SetReferenceUrl(This,index,pszReferenceUrl)	\
    ( (This)->lpVtbl -> SetReferenceUrl(This,index,pszReferenceUrl) ) 

#define IVsWCFReferenceGroup_DeleteReference(This,index)	\
    ( (This)->lpVtbl -> DeleteReference(This,index) ) 

#define IVsWCFReferenceGroup_UpdateConfiguration(This)	\
    ( (This)->lpVtbl -> UpdateConfiguration(This) ) 

#define IVsWCFReferenceGroup_BeginBatch(This)	\
    ( (This)->lpVtbl -> BeginBatch(This) ) 

#define IVsWCFReferenceGroup_EndBatch(This)	\
    ( (This)->lpVtbl -> EndBatch(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceGroup_INTERFACE_DEFINED__ */


#ifndef __IEnumWCFReferenceGroupMetadataItems_INTERFACE_DEFINED__
#define __IEnumWCFReferenceGroupMetadataItems_INTERFACE_DEFINED__

/* interface IEnumWCFReferenceGroupMetadataItems */
/* [object][custom][uuid][unique][version] */ 


EXTERN_C const IID IID_IEnumWCFReferenceGroupMetadataItems;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("603A2AEA-C925-4a8c-A273-65B3B81CD43A")
    IEnumWCFReferenceGroupMetadataItems : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceGroupMetadataItem **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceGroupMetadataItems **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumWCFReferenceGroupMetadataItemsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumWCFReferenceGroupMetadataItems * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumWCFReferenceGroupMetadataItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceGroupMetadataItem **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumWCFReferenceGroupMetadataItems * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumWCFReferenceGroupMetadataItems * This,
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceGroupMetadataItems **ppenum);
        
        END_INTERFACE
    } IEnumWCFReferenceGroupMetadataItemsVtbl;

    interface IEnumWCFReferenceGroupMetadataItems
    {
        CONST_VTBL struct IEnumWCFReferenceGroupMetadataItemsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumWCFReferenceGroupMetadataItems_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumWCFReferenceGroupMetadataItems_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumWCFReferenceGroupMetadataItems_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumWCFReferenceGroupMetadataItems_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEnumWCFReferenceGroupMetadataItems_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEnumWCFReferenceGroupMetadataItems_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEnumWCFReferenceGroupMetadataItems_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEnumWCFReferenceGroupMetadataItems_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumWCFReferenceGroupMetadataItems_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumWCFReferenceGroupMetadataItems_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumWCFReferenceGroupMetadataItems_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumWCFReferenceGroupMetadataItems_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceGroupMetadataItem_INTERFACE_DEFINED__
#define __IVsWCFReferenceGroupMetadataItem_INTERFACE_DEFINED__

/* interface IVsWCFReferenceGroupMetadataItem */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceGroupMetadataItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8167CA28-D8BC-4537-9FF8-D237DD447391")
    IVsWCFReferenceGroupMetadataItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSourceUrl( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSourceUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContent( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrXmlContent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNodeType( 
            /* [retval][out] */ __RPC__out DiscoveryNodeType *pType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceGroupMetadataItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceGroupMetadataItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceGroupMetadataItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceUrl )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrSourceUrl);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetNamespace )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *GetContent )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pbstrXmlContent);
        
        HRESULT ( STDMETHODCALLTYPE *GetNodeType )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [retval][out] */ __RPC__out DiscoveryNodeType *pType);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileName )( 
            IVsWCFReferenceGroupMetadataItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstFileName);
        
        END_INTERFACE
    } IVsWCFReferenceGroupMetadataItemVtbl;

    interface IVsWCFReferenceGroupMetadataItem
    {
        CONST_VTBL struct IVsWCFReferenceGroupMetadataItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceGroupMetadataItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceGroupMetadataItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceGroupMetadataItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceGroupMetadataItem_GetSourceUrl(This,pbstrSourceUrl)	\
    ( (This)->lpVtbl -> GetSourceUrl(This,pbstrSourceUrl) ) 

#define IVsWCFReferenceGroupMetadataItem_GetTargetNamespace(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetTargetNamespace(This,pbstrUrl) ) 

#define IVsWCFReferenceGroupMetadataItem_GetContent(This,pbstrXmlContent)	\
    ( (This)->lpVtbl -> GetContent(This,pbstrXmlContent) ) 

#define IVsWCFReferenceGroupMetadataItem_GetNodeType(This,pType)	\
    ( (This)->lpVtbl -> GetNodeType(This,pType) ) 

#define IVsWCFReferenceGroupMetadataItem_GetFileName(This,pbstFileName)	\
    ( (This)->lpVtbl -> GetFileName(This,pbstFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceGroupMetadataItem_INTERFACE_DEFINED__ */


#ifndef __IEnumWCFReferenceContracts_INTERFACE_DEFINED__
#define __IEnumWCFReferenceContracts_INTERFACE_DEFINED__

/* interface IEnumWCFReferenceContracts */
/* [object][custom][uuid][unique][version] */ 


EXTERN_C const IID IID_IEnumWCFReferenceContracts;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A8F120C5-E7DF-465a-A7FB-711805281A3B")
    IEnumWCFReferenceContracts : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceContract **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceContracts **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumWCFReferenceContractsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumWCFReferenceContracts * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumWCFReferenceContracts * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEnumWCFReferenceContracts * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceContract **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumWCFReferenceContracts * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumWCFReferenceContracts * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumWCFReferenceContracts * This,
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceContracts **ppenum);
        
        END_INTERFACE
    } IEnumWCFReferenceContractsVtbl;

    interface IEnumWCFReferenceContracts
    {
        CONST_VTBL struct IEnumWCFReferenceContractsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumWCFReferenceContracts_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumWCFReferenceContracts_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumWCFReferenceContracts_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumWCFReferenceContracts_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEnumWCFReferenceContracts_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEnumWCFReferenceContracts_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEnumWCFReferenceContracts_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEnumWCFReferenceContracts_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumWCFReferenceContracts_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumWCFReferenceContracts_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumWCFReferenceContracts_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumWCFReferenceContracts_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceContract_INTERFACE_DEFINED__
#define __IVsWCFReferenceContract_INTERFACE_DEFINED__

/* interface IVsWCFReferenceContract */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceContract;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0ED7423C-615C-47eb-931A-8E7D3F45DDCD")
    IVsWCFReferenceContract : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReferenceGroup( 
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppReferenceGroup) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTypeName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPortTypeName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPortTypeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceEndpointEnumerator( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceContractVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceContract * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceContract * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceContract * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceGroup )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFReferenceGroup **ppReferenceGroup);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeName )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrType);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetNamespace )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrTargetNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *GetPortTypeName )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPortTypeName);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceEndpointEnumerator )( 
            IVsWCFReferenceContract * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppEnum);
        
        END_INTERFACE
    } IVsWCFReferenceContractVtbl;

    interface IVsWCFReferenceContract
    {
        CONST_VTBL struct IVsWCFReferenceContractVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceContract_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceContract_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceContract_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceContract_GetReferenceGroup(This,ppReferenceGroup)	\
    ( (This)->lpVtbl -> GetReferenceGroup(This,ppReferenceGroup) ) 

#define IVsWCFReferenceContract_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsWCFReferenceContract_GetTypeName(This,pbstrType)	\
    ( (This)->lpVtbl -> GetTypeName(This,pbstrType) ) 

#define IVsWCFReferenceContract_GetTargetNamespace(This,pbstrTargetNamespace)	\
    ( (This)->lpVtbl -> GetTargetNamespace(This,pbstrTargetNamespace) ) 

#define IVsWCFReferenceContract_GetPortTypeName(This,pbstrPortTypeName)	\
    ( (This)->lpVtbl -> GetPortTypeName(This,pbstrPortTypeName) ) 

#define IVsWCFReferenceContract_GetReferenceEndpointEnumerator(This,ppEnum)	\
    ( (This)->lpVtbl -> GetReferenceEndpointEnumerator(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceContract_INTERFACE_DEFINED__ */


#ifndef __IEnumWCFReferenceEndpoints_INTERFACE_DEFINED__
#define __IEnumWCFReferenceEndpoints_INTERFACE_DEFINED__

/* interface IEnumWCFReferenceEndpoints */
/* [object][custom][uuid][unique][version] */ 


EXTERN_C const IID IID_IEnumWCFReferenceEndpoints;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0d66f264-c498-44c6-b08a-1a9ef57ddd63")
    IEnumWCFReferenceEndpoints : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceEndpoint **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumWCFReferenceEndpointsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumWCFReferenceEndpoints * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumWCFReferenceEndpoints * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEnumWCFReferenceEndpoints * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFReferenceEndpoint **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumWCFReferenceEndpoints * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumWCFReferenceEndpoints * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumWCFReferenceEndpoints * This,
            /* [out] */ __RPC__deref_out_opt IEnumWCFReferenceEndpoints **ppenum);
        
        END_INTERFACE
    } IEnumWCFReferenceEndpointsVtbl;

    interface IEnumWCFReferenceEndpoints
    {
        CONST_VTBL struct IEnumWCFReferenceEndpointsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumWCFReferenceEndpoints_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumWCFReferenceEndpoints_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumWCFReferenceEndpoints_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumWCFReferenceEndpoints_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEnumWCFReferenceEndpoints_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEnumWCFReferenceEndpoints_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEnumWCFReferenceEndpoints_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEnumWCFReferenceEndpoints_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumWCFReferenceEndpoints_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumWCFReferenceEndpoints_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumWCFReferenceEndpoints_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumWCFReferenceEndpoints_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceEndpoint_INTERFACE_DEFINED__
#define __IVsWCFReferenceEndpoint_INTERFACE_DEFINED__

/* interface IVsWCFReferenceEndpoint */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceEndpoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EFD57B55-A7DA-4c65-A6DF-90B3B656D749")
    IVsWCFReferenceEndpoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContract( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrContract) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAddress( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBinding( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBinding) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBindingConfiguration( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBindingConfiguration) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBehaviorConfiguration( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBehaviorConfiguration) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceEndpointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceEndpoint * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceEndpoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceEndpoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *GetContract )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrContract);
        
        HRESULT ( STDMETHODCALLTYPE *GetAddress )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrUrl);
        
        HRESULT ( STDMETHODCALLTYPE *GetBinding )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBinding);
        
        HRESULT ( STDMETHODCALLTYPE *GetBindingConfiguration )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBindingConfiguration);
        
        HRESULT ( STDMETHODCALLTYPE *GetBehaviorConfiguration )( 
            IVsWCFReferenceEndpoint * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrBehaviorConfiguration);
        
        END_INTERFACE
    } IVsWCFReferenceEndpointVtbl;

    interface IVsWCFReferenceEndpoint
    {
        CONST_VTBL struct IVsWCFReferenceEndpointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceEndpoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceEndpoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceEndpoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceEndpoint_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsWCFReferenceEndpoint_GetContract(This,pbstrContract)	\
    ( (This)->lpVtbl -> GetContract(This,pbstrContract) ) 

#define IVsWCFReferenceEndpoint_GetAddress(This,pbstrUrl)	\
    ( (This)->lpVtbl -> GetAddress(This,pbstrUrl) ) 

#define IVsWCFReferenceEndpoint_GetBinding(This,pbstrBinding)	\
    ( (This)->lpVtbl -> GetBinding(This,pbstrBinding) ) 

#define IVsWCFReferenceEndpoint_GetBindingConfiguration(This,pbstrBindingConfiguration)	\
    ( (This)->lpVtbl -> GetBindingConfiguration(This,pbstrBindingConfiguration) ) 

#define IVsWCFReferenceEndpoint_GetBehaviorConfiguration(This,pbstrBehaviorConfiguration)	\
    ( (This)->lpVtbl -> GetBehaviorConfiguration(This,pbstrBehaviorConfiguration) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceEndpoint_INTERFACE_DEFINED__ */


#ifndef __IVsWCFReferenceGroupOptions_INTERFACE_DEFINED__
#define __IVsWCFReferenceGroupOptions_INTERFACE_DEFINED__

/* interface IVsWCFReferenceGroupOptions */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFReferenceGroupOptions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CFCB0C9-5A1C-451a-9CCF-CAE41C3A5344")
    IVsWCFReferenceGroupOptions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetGenerateInternalTypes( 
            /* [retval][out] */ __RPC__out BOOL *pfGenerateInternalTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetGenerateInternalTypes( 
            /* [in] */ BOOL fGenerateInternalTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGenerateAsynchronousMethods( 
            /* [retval][out] */ __RPC__out BOOL *pbGenerateAsynchronousMethods) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetGenerateAsynchronousMethods( 
            /* [in] */ BOOL bGenerateAsynchronousMethods) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGenerateSerializableTypes( 
            /* [retval][out] */ __RPC__out BOOL *pbGenerateSerializableTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetGenerateSerializableTypes( 
            /* [in] */ BOOL bGenerateSerializableTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSerializer( 
            /* [retval][out] */ __RPC__out ProxySerializerType *pProxySerializerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSerializer( 
            /* [in] */ ProxySerializerType proxySerializerType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetImportXmlTypes( 
            /* [retval][out] */ __RPC__out BOOL *pbImportXmlTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetImportXmlTypes( 
            /* [in] */ BOOL bImportXmlTypes) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnableDataBinding( 
            /* [retval][out] */ __RPC__out BOOL *pbEnableDataBinding) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetEnableDataBinding( 
            /* [in] */ BOOL bEnableDataBinding) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetGenerateMessageContracts( 
            /* [retval][out] */ __RPC__out BOOL *pbGenerateMessageContracts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetGenerateMessageContracts( 
            /* [in] */ BOOL bGenerateMessageContracts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferenceAllAssemblies( 
            /* [retval][out] */ __RPC__out BOOL *pbReferenceAllAssemblies) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetReferenceAllAssemblies( 
            /* [in] */ BOOL bReferenceAllAssemblies) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferencedAssembliesCount( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReferencedAssemblies( 
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrReferencedAssemblies[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetReferencedAssemblies( 
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrReferencedAssemblies[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCollectionMappingsCount( 
            /* [in] */ CollectionCategory category,
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCollectionMappings( 
            /* [in] */ CollectionCategory category,
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrCollectionMappings[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCollectionMappings( 
            /* [in] */ CollectionCategory category,
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrCollectionMappings[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExcludedTypesCount( 
            /* [retval][out] */ __RPC__out long *plCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExcludedTypes( 
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrExcludedTypes[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExcludedTypes( 
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrExcludedTypes[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFReferenceGroupOptionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFReferenceGroupOptions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFReferenceGroupOptions * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetGenerateInternalTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pfGenerateInternalTypes);
        
        HRESULT ( STDMETHODCALLTYPE *SetGenerateInternalTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL fGenerateInternalTypes);
        
        HRESULT ( STDMETHODCALLTYPE *GetGenerateAsynchronousMethods )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbGenerateAsynchronousMethods);
        
        HRESULT ( STDMETHODCALLTYPE *SetGenerateAsynchronousMethods )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bGenerateAsynchronousMethods);
        
        HRESULT ( STDMETHODCALLTYPE *GetGenerateSerializableTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbGenerateSerializableTypes);
        
        HRESULT ( STDMETHODCALLTYPE *SetGenerateSerializableTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bGenerateSerializableTypes);
        
        HRESULT ( STDMETHODCALLTYPE *GetSerializer )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out ProxySerializerType *pProxySerializerType);
        
        HRESULT ( STDMETHODCALLTYPE *SetSerializer )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ ProxySerializerType proxySerializerType);
        
        HRESULT ( STDMETHODCALLTYPE *GetImportXmlTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbImportXmlTypes);
        
        HRESULT ( STDMETHODCALLTYPE *SetImportXmlTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bImportXmlTypes);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnableDataBinding )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbEnableDataBinding);
        
        HRESULT ( STDMETHODCALLTYPE *SetEnableDataBinding )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bEnableDataBinding);
        
        HRESULT ( STDMETHODCALLTYPE *GetGenerateMessageContracts )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbGenerateMessageContracts);
        
        HRESULT ( STDMETHODCALLTYPE *SetGenerateMessageContracts )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bGenerateMessageContracts);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferenceAllAssemblies )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out BOOL *pbReferenceAllAssemblies);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferenceAllAssemblies )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ BOOL bReferenceAllAssemblies);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferencedAssembliesCount )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetReferencedAssemblies )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrReferencedAssemblies[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetReferencedAssemblies )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrReferencedAssemblies[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetCollectionMappingsCount )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ CollectionCategory category,
            /* [retval][out] */ __RPC__out long *plCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetCollectionMappings )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ CollectionCategory category,
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrCollectionMappings[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetCollectionMappings )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ CollectionCategory category,
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrCollectionMappings[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetExcludedTypesCount )( 
            IVsWCFReferenceGroupOptions * This,
            /* [retval][out] */ __RPC__out long *plCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetExcludedTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ long cItems,
            /* [size_is][out] */ __RPC__out_ecount_full(cItems) BSTR rgbstrExcludedTypes[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetExcludedTypes )( 
            IVsWCFReferenceGroupOptions * This,
            /* [in] */ long cItems,
            /* [size_is][in] */ __RPC__in_ecount_full(cItems) LPCOLESTR lpstrExcludedTypes[  ]);
        
        END_INTERFACE
    } IVsWCFReferenceGroupOptionsVtbl;

    interface IVsWCFReferenceGroupOptions
    {
        CONST_VTBL struct IVsWCFReferenceGroupOptionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFReferenceGroupOptions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFReferenceGroupOptions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFReferenceGroupOptions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFReferenceGroupOptions_GetGenerateInternalTypes(This,pfGenerateInternalTypes)	\
    ( (This)->lpVtbl -> GetGenerateInternalTypes(This,pfGenerateInternalTypes) ) 

#define IVsWCFReferenceGroupOptions_SetGenerateInternalTypes(This,fGenerateInternalTypes)	\
    ( (This)->lpVtbl -> SetGenerateInternalTypes(This,fGenerateInternalTypes) ) 

#define IVsWCFReferenceGroupOptions_GetGenerateAsynchronousMethods(This,pbGenerateAsynchronousMethods)	\
    ( (This)->lpVtbl -> GetGenerateAsynchronousMethods(This,pbGenerateAsynchronousMethods) ) 

#define IVsWCFReferenceGroupOptions_SetGenerateAsynchronousMethods(This,bGenerateAsynchronousMethods)	\
    ( (This)->lpVtbl -> SetGenerateAsynchronousMethods(This,bGenerateAsynchronousMethods) ) 

#define IVsWCFReferenceGroupOptions_GetGenerateSerializableTypes(This,pbGenerateSerializableTypes)	\
    ( (This)->lpVtbl -> GetGenerateSerializableTypes(This,pbGenerateSerializableTypes) ) 

#define IVsWCFReferenceGroupOptions_SetGenerateSerializableTypes(This,bGenerateSerializableTypes)	\
    ( (This)->lpVtbl -> SetGenerateSerializableTypes(This,bGenerateSerializableTypes) ) 

#define IVsWCFReferenceGroupOptions_GetSerializer(This,pProxySerializerType)	\
    ( (This)->lpVtbl -> GetSerializer(This,pProxySerializerType) ) 

#define IVsWCFReferenceGroupOptions_SetSerializer(This,proxySerializerType)	\
    ( (This)->lpVtbl -> SetSerializer(This,proxySerializerType) ) 

#define IVsWCFReferenceGroupOptions_GetImportXmlTypes(This,pbImportXmlTypes)	\
    ( (This)->lpVtbl -> GetImportXmlTypes(This,pbImportXmlTypes) ) 

#define IVsWCFReferenceGroupOptions_SetImportXmlTypes(This,bImportXmlTypes)	\
    ( (This)->lpVtbl -> SetImportXmlTypes(This,bImportXmlTypes) ) 

#define IVsWCFReferenceGroupOptions_GetEnableDataBinding(This,pbEnableDataBinding)	\
    ( (This)->lpVtbl -> GetEnableDataBinding(This,pbEnableDataBinding) ) 

#define IVsWCFReferenceGroupOptions_SetEnableDataBinding(This,bEnableDataBinding)	\
    ( (This)->lpVtbl -> SetEnableDataBinding(This,bEnableDataBinding) ) 

#define IVsWCFReferenceGroupOptions_GetGenerateMessageContracts(This,pbGenerateMessageContracts)	\
    ( (This)->lpVtbl -> GetGenerateMessageContracts(This,pbGenerateMessageContracts) ) 

#define IVsWCFReferenceGroupOptions_SetGenerateMessageContracts(This,bGenerateMessageContracts)	\
    ( (This)->lpVtbl -> SetGenerateMessageContracts(This,bGenerateMessageContracts) ) 

#define IVsWCFReferenceGroupOptions_GetReferenceAllAssemblies(This,pbReferenceAllAssemblies)	\
    ( (This)->lpVtbl -> GetReferenceAllAssemblies(This,pbReferenceAllAssemblies) ) 

#define IVsWCFReferenceGroupOptions_SetReferenceAllAssemblies(This,bReferenceAllAssemblies)	\
    ( (This)->lpVtbl -> SetReferenceAllAssemblies(This,bReferenceAllAssemblies) ) 

#define IVsWCFReferenceGroupOptions_GetReferencedAssembliesCount(This,plCount)	\
    ( (This)->lpVtbl -> GetReferencedAssembliesCount(This,plCount) ) 

#define IVsWCFReferenceGroupOptions_GetReferencedAssemblies(This,cItems,rgbstrReferencedAssemblies)	\
    ( (This)->lpVtbl -> GetReferencedAssemblies(This,cItems,rgbstrReferencedAssemblies) ) 

#define IVsWCFReferenceGroupOptions_SetReferencedAssemblies(This,cItems,lpstrReferencedAssemblies)	\
    ( (This)->lpVtbl -> SetReferencedAssemblies(This,cItems,lpstrReferencedAssemblies) ) 

#define IVsWCFReferenceGroupOptions_GetCollectionMappingsCount(This,category,plCount)	\
    ( (This)->lpVtbl -> GetCollectionMappingsCount(This,category,plCount) ) 

#define IVsWCFReferenceGroupOptions_GetCollectionMappings(This,category,cItems,rgbstrCollectionMappings)	\
    ( (This)->lpVtbl -> GetCollectionMappings(This,category,cItems,rgbstrCollectionMappings) ) 

#define IVsWCFReferenceGroupOptions_SetCollectionMappings(This,category,cItems,lpstrCollectionMappings)	\
    ( (This)->lpVtbl -> SetCollectionMappings(This,category,cItems,lpstrCollectionMappings) ) 

#define IVsWCFReferenceGroupOptions_GetExcludedTypesCount(This,plCount)	\
    ( (This)->lpVtbl -> GetExcludedTypesCount(This,plCount) ) 

#define IVsWCFReferenceGroupOptions_GetExcludedTypes(This,cItems,rgbstrExcludedTypes)	\
    ( (This)->lpVtbl -> GetExcludedTypes(This,cItems,rgbstrExcludedTypes) ) 

#define IVsWCFReferenceGroupOptions_SetExcludedTypes(This,cItems,lpstrExcludedTypes)	\
    ( (This)->lpVtbl -> SetExcludedTypes(This,cItems,lpstrExcludedTypes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFReferenceGroupOptions_INTERFACE_DEFINED__ */


#ifndef __IVsWCFMetadataStorageProvider_INTERFACE_DEFINED__
#define __IVsWCFMetadataStorageProvider_INTERFACE_DEFINED__

/* interface IVsWCFMetadataStorageProvider */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFMetadataStorageProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f71d2b05-680f-423b-b00f-52a2944ac45c")
    IVsWCFMetadataStorageProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStorages( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWCFMetadataStorages **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateStorage( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt IVsWCFMetadataStorage **pWCFMetadataStorage,
            /* [out] */ __RPC__out StorageNameValidationState *pNameValidationState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStorageFromMapFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFMetadataStorage **pWCFMetadataStorage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseWCFMetadataStorageProviderEvents( 
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorageProviderEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseWCFMetadataStorageProviderEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValidNewReferenceName( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__out BOOLEAN *pbValid,
            /* [out] */ __RPC__out StorageNameValidationState *pValidationState,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MakeValidReferenceName( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedNamespace,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedReferenceName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFMetadataStorageProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFMetadataStorageProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFMetadataStorageProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStorages )( 
            IVsWCFMetadataStorageProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumWCFMetadataStorages **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *CreateStorage )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt IVsWCFMetadataStorage **pWCFMetadataStorage,
            /* [out] */ __RPC__out StorageNameValidationState *pNameValidationState);
        
        HRESULT ( STDMETHODCALLTYPE *GetStorageFromMapFile )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsWCFMetadataStorage **pWCFMetadataStorage);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseWCFMetadataStorageProviderEvents )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorageProviderEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseWCFMetadataStorageProviderEvents )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *IsValidNewReferenceName )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__out BOOLEAN *pbValid,
            /* [out] */ __RPC__out StorageNameValidationState *pValidationState,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage);
        
        HRESULT ( STDMETHODCALLTYPE *MakeValidReferenceName )( 
            IVsWCFMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedNamespace,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedReferenceName);
        
        END_INTERFACE
    } IVsWCFMetadataStorageProviderVtbl;

    interface IVsWCFMetadataStorageProvider
    {
        CONST_VTBL struct IVsWCFMetadataStorageProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFMetadataStorageProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFMetadataStorageProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFMetadataStorageProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFMetadataStorageProvider_GetStorages(This,ppenum)	\
    ( (This)->lpVtbl -> GetStorages(This,ppenum) ) 

#define IVsWCFMetadataStorageProvider_CreateStorage(This,pszNamespace,pszReferenceName,pWCFMetadataStorage,pNameValidationState)	\
    ( (This)->lpVtbl -> CreateStorage(This,pszNamespace,pszReferenceName,pWCFMetadataStorage,pNameValidationState) ) 

#define IVsWCFMetadataStorageProvider_GetStorageFromMapFile(This,pszMapFilePath,pWCFMetadataStorage)	\
    ( (This)->lpVtbl -> GetStorageFromMapFile(This,pszMapFilePath,pWCFMetadataStorage) ) 

#define IVsWCFMetadataStorageProvider_AdviseWCFMetadataStorageProviderEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseWCFMetadataStorageProviderEvents(This,pSink,pdwCookie) ) 

#define IVsWCFMetadataStorageProvider_UnadviseWCFMetadataStorageProviderEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseWCFMetadataStorageProviderEvents(This,dwCookie) ) 

#define IVsWCFMetadataStorageProvider_IsValidNewReferenceName(This,pszNamespace,pszReferenceName,pbValid,pValidationState,pbstrMessage)	\
    ( (This)->lpVtbl -> IsValidNewReferenceName(This,pszNamespace,pszReferenceName,pbValid,pValidationState,pbstrMessage) ) 

#define IVsWCFMetadataStorageProvider_MakeValidReferenceName(This,pszNamespace,pszReferenceName,pbstrSuggestedNamespace,pbstrSuggestedReferenceName)	\
    ( (This)->lpVtbl -> MakeValidReferenceName(This,pszNamespace,pszReferenceName,pbstrSuggestedNamespace,pbstrSuggestedReferenceName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFMetadataStorageProvider_INTERFACE_DEFINED__ */


#ifndef __IVsEnumWCFMetadataStorages_INTERFACE_DEFINED__
#define __IVsEnumWCFMetadataStorages_INTERFACE_DEFINED__

/* interface IVsEnumWCFMetadataStorages */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsEnumWCFMetadataStorages;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e9532448-d152-4ad4-be45-8a8a9cbfe14d")
    IVsEnumWCFMetadataStorages : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFMetadataStorage **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumWCFMetadataStorages **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumWCFMetadataStoragesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumWCFMetadataStorages * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumWCFMetadataStorages * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumWCFMetadataStorages * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumWCFMetadataStorages * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsWCFMetadataStorage **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumWCFMetadataStorages * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumWCFMetadataStorages * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumWCFMetadataStorages * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumWCFMetadataStorages **ppenum);
        
        END_INTERFACE
    } IVsEnumWCFMetadataStoragesVtbl;

    interface IVsEnumWCFMetadataStorages
    {
        CONST_VTBL struct IVsEnumWCFMetadataStoragesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumWCFMetadataStorages_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumWCFMetadataStorages_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumWCFMetadataStorages_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumWCFMetadataStorages_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumWCFMetadataStorages_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumWCFMetadataStorages_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumWCFMetadataStorages_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumWCFMetadataStorages_INTERFACE_DEFINED__ */


#ifndef __IVsWCFMetadataStorageProviderEvents_INTERFACE_DEFINED__
#define __IVsWCFMetadataStorageProviderEvents_INTERFACE_DEFINED__

/* interface IVsWCFMetadataStorageProviderEvents */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFMetadataStorageProviderEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("610dfeb0-5c2b-46c2-bb44-1de7dc42d409")
    IVsWCFMetadataStorageProviderEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAdded( 
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorage *pIVsWCFMetadataStorage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoving( 
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoved( 
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRenamed( 
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMoved( 
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFMetadataStorageProviderEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFMetadataStorageProviderEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFMetadataStorageProviderEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAdded )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorage *pIVsWCFMetadataStorage);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoving )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoved )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnRenamed )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnMoved )( 
            IVsWCFMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath);
        
        END_INTERFACE
    } IVsWCFMetadataStorageProviderEventsVtbl;

    interface IVsWCFMetadataStorageProviderEvents
    {
        CONST_VTBL struct IVsWCFMetadataStorageProviderEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFMetadataStorageProviderEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFMetadataStorageProviderEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFMetadataStorageProviderEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFMetadataStorageProviderEvents_OnAdded(This,pIVsWCFMetadataStorage)	\
    ( (This)->lpVtbl -> OnAdded(This,pIVsWCFMetadataStorage) ) 

#define IVsWCFMetadataStorageProviderEvents_OnRemoving(This,pszOldSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRemoving(This,pszOldSvcmapFilePath) ) 

#define IVsWCFMetadataStorageProviderEvents_OnRemoved(This,pszOldSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRemoved(This,pszOldSvcmapFilePath) ) 

#define IVsWCFMetadataStorageProviderEvents_OnRenamed(This,pszOldSvcmapFilePath,pszNewSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRenamed(This,pszOldSvcmapFilePath,pszNewSvcmapFilePath) ) 

#define IVsWCFMetadataStorageProviderEvents_OnMoved(This,pszOldSvcmapFilePath,pszNewSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnMoved(This,pszOldSvcmapFilePath,pszNewSvcmapFilePath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFMetadataStorageProviderEvents_INTERFACE_DEFINED__ */


#ifndef __IVsWCFMetadataStorage_INTERFACE_DEFINED__
#define __IVsWCFMetadataStorage_INTERFACE_DEFINED__

/* interface IVsWCFMetadataStorage */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFMetadataStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8d095035-ab0c-4363-891a-8c79f5cda259")
    IVsWCFMetadataStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetName( 
            /* [in] */ __RPC__in LPCOLESTR pszName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNamespace( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginUpdate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndUpdate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RunProxyGenerator( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadMapFileData( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveMapFileData( 
            /* [in] */ __RPC__in SAFEARRAY * content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMapFilePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [in] */ __RPC__in SAFEARRAY * content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataSuggestedName,
            /* [in] */ __RPC__in SAFEARRAY * content,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseWCFMetadataStorageEvents( 
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseWCFMetadataStorageEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProjectItemId( 
            /* [retval][out] */ __RPC__out VSITEMID *pProjectItemId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FileCodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFMetadataStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFMetadataStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetName )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *GetNamespace )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *SetNamespace )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *BeginUpdate )( 
            IVsWCFMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndUpdate )( 
            IVsWCFMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *RunProxyGenerator )( 
            IVsWCFMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadMapFileData )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent);
        
        HRESULT ( STDMETHODCALLTYPE *SaveMapFileData )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in SAFEARRAY * content);
        
        HRESULT ( STDMETHODCALLTYPE *GetMapFilePath )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *LoadMetadataFile )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent);
        
        HRESULT ( STDMETHODCALLTYPE *SaveMetadataFile )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [in] */ __RPC__in SAFEARRAY * content);
        
        HRESULT ( STDMETHODCALLTYPE *CreateMetadataFile )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataSuggestedName,
            /* [in] */ __RPC__in SAFEARRAY * content,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteMetadataFile )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseWCFMetadataStorageEvents )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ __RPC__in_opt IVsWCFMetadataStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseWCFMetadataStorageEvents )( 
            IVsWCFMetadataStorage * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IVsWCFMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProjectItemId )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__out VSITEMID *pProjectItemId);
        
        HRESULT ( STDMETHODCALLTYPE *FileCodeModel )( 
            IVsWCFMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel);
        
        END_INTERFACE
    } IVsWCFMetadataStorageVtbl;

    interface IVsWCFMetadataStorage
    {
        CONST_VTBL struct IVsWCFMetadataStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFMetadataStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFMetadataStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFMetadataStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFMetadataStorage_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsWCFMetadataStorage_SetName(This,pszName)	\
    ( (This)->lpVtbl -> SetName(This,pszName) ) 

#define IVsWCFMetadataStorage_GetNamespace(This,pbstrNamespace)	\
    ( (This)->lpVtbl -> GetNamespace(This,pbstrNamespace) ) 

#define IVsWCFMetadataStorage_SetNamespace(This,pszNamespace)	\
    ( (This)->lpVtbl -> SetNamespace(This,pszNamespace) ) 

#define IVsWCFMetadataStorage_BeginUpdate(This)	\
    ( (This)->lpVtbl -> BeginUpdate(This) ) 

#define IVsWCFMetadataStorage_EndUpdate(This)	\
    ( (This)->lpVtbl -> EndUpdate(This) ) 

#define IVsWCFMetadataStorage_RunProxyGenerator(This)	\
    ( (This)->lpVtbl -> RunProxyGenerator(This) ) 

#define IVsWCFMetadataStorage_LoadMapFileData(This,pContent)	\
    ( (This)->lpVtbl -> LoadMapFileData(This,pContent) ) 

#define IVsWCFMetadataStorage_SaveMapFileData(This,content)	\
    ( (This)->lpVtbl -> SaveMapFileData(This,content) ) 

#define IVsWCFMetadataStorage_GetMapFilePath(This,pbstrMapFilePath)	\
    ( (This)->lpVtbl -> GetMapFilePath(This,pbstrMapFilePath) ) 

#define IVsWCFMetadataStorage_LoadMetadataFile(This,pszMetadataFileName,pContent)	\
    ( (This)->lpVtbl -> LoadMetadataFile(This,pszMetadataFileName,pContent) ) 

#define IVsWCFMetadataStorage_SaveMetadataFile(This,pszMetadataFileName,content)	\
    ( (This)->lpVtbl -> SaveMetadataFile(This,pszMetadataFileName,content) ) 

#define IVsWCFMetadataStorage_CreateMetadataFile(This,pszMetadataSuggestedName,content,pbstrRealName)	\
    ( (This)->lpVtbl -> CreateMetadataFile(This,pszMetadataSuggestedName,content,pbstrRealName) ) 

#define IVsWCFMetadataStorage_DeleteMetadataFile(This,pszMetadataFileName)	\
    ( (This)->lpVtbl -> DeleteMetadataFile(This,pszMetadataFileName) ) 

#define IVsWCFMetadataStorage_AdviseWCFMetadataStorageEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseWCFMetadataStorageEvents(This,pSink,pdwCookie) ) 

#define IVsWCFMetadataStorage_UnadviseWCFMetadataStorageEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseWCFMetadataStorageEvents(This,dwCookie) ) 

#define IVsWCFMetadataStorage_Remove(This)	\
    ( (This)->lpVtbl -> Remove(This) ) 

#define IVsWCFMetadataStorage_ProjectItemId(This,pProjectItemId)	\
    ( (This)->lpVtbl -> ProjectItemId(This,pProjectItemId) ) 

#define IVsWCFMetadataStorage_FileCodeModel(This,ppFileCodeModel)	\
    ( (This)->lpVtbl -> FileCodeModel(This,ppFileCodeModel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFMetadataStorage_INTERFACE_DEFINED__ */


#ifndef __IVsWCFMetadataStorageEvents_INTERFACE_DEFINED__
#define __IVsWCFMetadataStorageEvents_INTERFACE_DEFINED__

/* interface IVsWCFMetadataStorageEvents */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsWCFMetadataStorageEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("77fded51-e1ee-42b4-9e6c-bc892487d0ab")
    IVsWCFMetadataStorageEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnMapFileUpdated( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileAdded( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileDeleted( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileRenamed( 
            /* [in] */ __RPC__in LPOLESTR pszFileNameOld,
            /* [in] */ __RPC__in LPOLESTR pszFileNameNew) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileUpdated( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsWCFMetadataStorageEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsWCFMetadataStorageEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsWCFMetadataStorageEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsWCFMetadataStorageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnMapFileUpdated )( 
            IVsWCFMetadataStorageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileAdded )( 
            IVsWCFMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileDeleted )( 
            IVsWCFMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileRenamed )( 
            IVsWCFMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileNameOld,
            /* [in] */ __RPC__in LPOLESTR pszFileNameNew);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileUpdated )( 
            IVsWCFMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        END_INTERFACE
    } IVsWCFMetadataStorageEventsVtbl;

    interface IVsWCFMetadataStorageEvents
    {
        CONST_VTBL struct IVsWCFMetadataStorageEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsWCFMetadataStorageEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsWCFMetadataStorageEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsWCFMetadataStorageEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsWCFMetadataStorageEvents_OnMapFileUpdated(This)	\
    ( (This)->lpVtbl -> OnMapFileUpdated(This) ) 

#define IVsWCFMetadataStorageEvents_OnMetadataFileAdded(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileAdded(This,pszFileName) ) 

#define IVsWCFMetadataStorageEvents_OnMetadataFileDeleted(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileDeleted(This,pszFileName) ) 

#define IVsWCFMetadataStorageEvents_OnMetadataFileRenamed(This,pszFileNameOld,pszFileNameNew)	\
    ( (This)->lpVtbl -> OnMetadataFileRenamed(This,pszFileNameOld,pszFileNameNew) ) 

#define IVsWCFMetadataStorageEvents_OnMetadataFileUpdated(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileUpdated(This,pszFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsWCFMetadataStorageEvents_INTERFACE_DEFINED__ */


#ifndef __IVsASMXMetadataStorageProvider_INTERFACE_DEFINED__
#define __IVsASMXMetadataStorageProvider_INTERFACE_DEFINED__

/* interface IVsASMXMetadataStorageProvider */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsASMXMetadataStorageProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c5e5042e-f39a-4c27-ae06-1b1715ea7223")
    IVsASMXMetadataStorageProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddWebReference( 
            /* [in] */ __RPC__in LPCOLESTR pszURL,
            /* [in] */ __RPC__in LPCOLESTR pszSuggestName,
            /* [in] */ __RPC__in_opt IDiscoveryResult *pDiscoveryResult,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValidNewASMXReferenceName( 
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [retval][out] */ __RPC__out BOOL *pbValid) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsASMXMetadataStorageProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsASMXMetadataStorageProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsASMXMetadataStorageProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsASMXMetadataStorageProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebReference )( 
            IVsASMXMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszURL,
            /* [in] */ __RPC__in LPCOLESTR pszSuggestName,
            /* [in] */ __RPC__in_opt IDiscoveryResult *pDiscoveryResult,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName);
        
        HRESULT ( STDMETHODCALLTYPE *IsValidNewASMXReferenceName )( 
            IVsASMXMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszNewName,
            /* [retval][out] */ __RPC__out BOOL *pbValid);
        
        END_INTERFACE
    } IVsASMXMetadataStorageProviderVtbl;

    interface IVsASMXMetadataStorageProvider
    {
        CONST_VTBL struct IVsASMXMetadataStorageProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsASMXMetadataStorageProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsASMXMetadataStorageProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsASMXMetadataStorageProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsASMXMetadataStorageProvider_AddWebReference(This,pszURL,pszSuggestName,pDiscoveryResult,pbstrRealName)	\
    ( (This)->lpVtbl -> AddWebReference(This,pszURL,pszSuggestName,pDiscoveryResult,pbstrRealName) ) 

#define IVsASMXMetadataStorageProvider_IsValidNewASMXReferenceName(This,pszNewName,pbValid)	\
    ( (This)->lpVtbl -> IsValidNewASMXReferenceName(This,pszNewName,pbValid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsASMXMetadataStorageProvider_INTERFACE_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageProvider_INTERFACE_DEFINED__
#define __IVsServiceReferenceMetadataStorageProvider_INTERFACE_DEFINED__

/* interface IVsServiceReferenceMetadataStorageProvider */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsServiceReferenceMetadataStorageProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6D076165-1AFF-4d68-9BD1-FA09ADF57D34")
    IVsServiceReferenceMetadataStorageProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsStorageTypeSupported( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [retval][out] */ __RPC__out BOOL *pbSupported) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetadataStorages( 
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumServiceReferenceMetadataStorages **ppenum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateMetadataStorage( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt IVsServiceReferenceMetadataStorage **pServiceReferenceMetadataStorage,
            /* [out] */ __RPC__out StorageNameValidationState *pNameValidationState) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMetadataStorageFromMapFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsServiceReferenceMetadataStorage **pServiceReferenceMetadataStorage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseMetadataStorageProviderEvents( 
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorageProviderEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseMetadataStorageProviderEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsValidNewServiceReferenceName( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__out BOOLEAN *pbValid,
            /* [out] */ __RPC__out StorageNameValidationState *pValidationState,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MakeValidServiceReferenceName( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedNamespace,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedReferenceName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsServiceReferenceMetadataStorageProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsServiceReferenceMetadataStorageProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsServiceReferenceMetadataStorageProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsStorageTypeSupported )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [retval][out] */ __RPC__out BOOL *pbSupported);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataStorages )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsEnumServiceReferenceMetadataStorages **ppenum);
        
        HRESULT ( STDMETHODCALLTYPE *CreateMetadataStorage )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt IVsServiceReferenceMetadataStorage **pServiceReferenceMetadataStorage,
            /* [out] */ __RPC__out StorageNameValidationState *pNameValidationState);
        
        HRESULT ( STDMETHODCALLTYPE *GetMetadataStorageFromMapFile )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszMapFilePath,
            /* [retval][out] */ __RPC__deref_out_opt IVsServiceReferenceMetadataStorage **pServiceReferenceMetadataStorage);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseMetadataStorageProviderEvents )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorageProviderEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseMetadataStorageProviderEvents )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *IsValidNewServiceReferenceName )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__out BOOLEAN *pbValid,
            /* [out] */ __RPC__out StorageNameValidationState *pValidationState,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrMessage);
        
        HRESULT ( STDMETHODCALLTYPE *MakeValidServiceReferenceName )( 
            IVsServiceReferenceMetadataStorageProvider * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace,
            /* [in] */ __RPC__in LPCOLESTR pszReferenceName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedNamespace,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrSuggestedReferenceName);
        
        END_INTERFACE
    } IVsServiceReferenceMetadataStorageProviderVtbl;

    interface IVsServiceReferenceMetadataStorageProvider
    {
        CONST_VTBL struct IVsServiceReferenceMetadataStorageProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsServiceReferenceMetadataStorageProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsServiceReferenceMetadataStorageProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsServiceReferenceMetadataStorageProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsServiceReferenceMetadataStorageProvider_IsStorageTypeSupported(This,pszStorageType,pbSupported)	\
    ( (This)->lpVtbl -> IsStorageTypeSupported(This,pszStorageType,pbSupported) ) 

#define IVsServiceReferenceMetadataStorageProvider_GetMetadataStorages(This,ppenum)	\
    ( (This)->lpVtbl -> GetMetadataStorages(This,ppenum) ) 

#define IVsServiceReferenceMetadataStorageProvider_CreateMetadataStorage(This,pszStorageType,pszNamespace,pszReferenceName,pServiceReferenceMetadataStorage,pNameValidationState)	\
    ( (This)->lpVtbl -> CreateMetadataStorage(This,pszStorageType,pszNamespace,pszReferenceName,pServiceReferenceMetadataStorage,pNameValidationState) ) 

#define IVsServiceReferenceMetadataStorageProvider_GetMetadataStorageFromMapFile(This,pszMapFilePath,pServiceReferenceMetadataStorage)	\
    ( (This)->lpVtbl -> GetMetadataStorageFromMapFile(This,pszMapFilePath,pServiceReferenceMetadataStorage) ) 

#define IVsServiceReferenceMetadataStorageProvider_AdviseMetadataStorageProviderEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseMetadataStorageProviderEvents(This,pSink,pdwCookie) ) 

#define IVsServiceReferenceMetadataStorageProvider_UnadviseMetadataStorageProviderEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseMetadataStorageProviderEvents(This,dwCookie) ) 

#define IVsServiceReferenceMetadataStorageProvider_IsValidNewServiceReferenceName(This,pszStorageType,pszNamespace,pszReferenceName,pbValid,pValidationState,pbstrMessage)	\
    ( (This)->lpVtbl -> IsValidNewServiceReferenceName(This,pszStorageType,pszNamespace,pszReferenceName,pbValid,pValidationState,pbstrMessage) ) 

#define IVsServiceReferenceMetadataStorageProvider_MakeValidServiceReferenceName(This,pszStorageType,pszNamespace,pszReferenceName,pbstrSuggestedNamespace,pbstrSuggestedReferenceName)	\
    ( (This)->lpVtbl -> MakeValidServiceReferenceName(This,pszStorageType,pszNamespace,pszReferenceName,pbstrSuggestedNamespace,pbstrSuggestedReferenceName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsServiceReferenceMetadataStorageProvider_INTERFACE_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageProviderEvents_INTERFACE_DEFINED__
#define __IVsServiceReferenceMetadataStorageProviderEvents_INTERFACE_DEFINED__

/* interface IVsServiceReferenceMetadataStorageProviderEvents */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsServiceReferenceMetadataStorageProviderEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DBFECCE0-41A5-41a6-AD3B-67286C457A4F")
    IVsServiceReferenceMetadataStorageProviderEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAdded( 
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorage *pIVsServiceReferenceMetadataStorage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoving( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRemoved( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnRenamed( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMoved( 
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsServiceReferenceMetadataStorageProviderEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAdded )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorage *pIVsServiceReferenceMetadataStorage);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoving )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnRemoved )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnRenamed )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *OnMoved )( 
            IVsServiceReferenceMetadataStorageProviderEvents * This,
            /* [in] */ __RPC__in LPCOLESTR pszStorageType,
            /* [in] */ __RPC__in LPCOLESTR pszOldSvcmapFilePath,
            /* [in] */ __RPC__in LPCOLESTR pszNewSvcmapFilePath);
        
        END_INTERFACE
    } IVsServiceReferenceMetadataStorageProviderEventsVtbl;

    interface IVsServiceReferenceMetadataStorageProviderEvents
    {
        CONST_VTBL struct IVsServiceReferenceMetadataStorageProviderEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsServiceReferenceMetadataStorageProviderEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsServiceReferenceMetadataStorageProviderEvents_OnAdded(This,pIVsServiceReferenceMetadataStorage)	\
    ( (This)->lpVtbl -> OnAdded(This,pIVsServiceReferenceMetadataStorage) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_OnRemoving(This,pszStorageType,pszOldSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRemoving(This,pszStorageType,pszOldSvcmapFilePath) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_OnRemoved(This,pszStorageType,pszOldSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRemoved(This,pszStorageType,pszOldSvcmapFilePath) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_OnRenamed(This,pszStorageType,pszOldSvcmapFilePath,pszNewSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnRenamed(This,pszStorageType,pszOldSvcmapFilePath,pszNewSvcmapFilePath) ) 

#define IVsServiceReferenceMetadataStorageProviderEvents_OnMoved(This,pszStorageType,pszOldSvcmapFilePath,pszNewSvcmapFilePath)	\
    ( (This)->lpVtbl -> OnMoved(This,pszStorageType,pszOldSvcmapFilePath,pszNewSvcmapFilePath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsServiceReferenceMetadataStorageProviderEvents_INTERFACE_DEFINED__ */


#ifndef __IVsEnumServiceReferenceMetadataStorages_INTERFACE_DEFINED__
#define __IVsEnumServiceReferenceMetadataStorages_INTERFACE_DEFINED__

/* interface IVsEnumServiceReferenceMetadataStorages */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsEnumServiceReferenceMetadataStorages;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C5EE29B-8BE6-4a9b-910B-F57EF6A36D6E")
    IVsEnumServiceReferenceMetadataStorages : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsServiceReferenceMetadataStorage **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumServiceReferenceMetadataStorages **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumServiceReferenceMetadataStoragesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumServiceReferenceMetadataStorages * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumServiceReferenceMetadataStorages * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumServiceReferenceMetadataStorages * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumServiceReferenceMetadataStorages * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) IVsServiceReferenceMetadataStorage **rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsEnumServiceReferenceMetadataStorages * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumServiceReferenceMetadataStorages * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsEnumServiceReferenceMetadataStorages * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumServiceReferenceMetadataStorages **ppenum);
        
        END_INTERFACE
    } IVsEnumServiceReferenceMetadataStoragesVtbl;

    interface IVsEnumServiceReferenceMetadataStorages
    {
        CONST_VTBL struct IVsEnumServiceReferenceMetadataStoragesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumServiceReferenceMetadataStorages_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumServiceReferenceMetadataStorages_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumServiceReferenceMetadataStorages_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumServiceReferenceMetadataStorages_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsEnumServiceReferenceMetadataStorages_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsEnumServiceReferenceMetadataStorages_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumServiceReferenceMetadataStorages_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumServiceReferenceMetadataStorages_INTERFACE_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorage_INTERFACE_DEFINED__
#define __IVsServiceReferenceMetadataStorage_INTERFACE_DEFINED__

/* interface IVsServiceReferenceMetadataStorage */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsServiceReferenceMetadataStorage;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0743635E-4F48-457c-9323-0AF36982FA28")
    IVsServiceReferenceMetadataStorage : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStorageType( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStorageType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetName( 
            /* [in] */ __RPC__in LPCOLESTR pszName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNamespace( 
            /* [in] */ __RPC__in LPCOLESTR pszNamespace) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginUpdate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndUpdate( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RunProxyGenerator( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadMapFileData( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveMapFileData( 
            /* [in] */ __RPC__in SAFEARRAY * content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMapFilePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMapFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [in] */ __RPC__in SAFEARRAY * content) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataSuggestedName,
            /* [in] */ __RPC__in SAFEARRAY * content,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteMetadataFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseServiceReferenceMetadataStorageEvents( 
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseServiceReferenceMetadataStorageEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Remove( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ProjectItemId( 
            /* [retval][out] */ __RPC__out VSITEMID *pProjectItemId) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FileCodeModel( 
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsServiceReferenceMetadataStorageVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsServiceReferenceMetadataStorage * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsServiceReferenceMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStorageType )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrStorageType);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        HRESULT ( STDMETHODCALLTYPE *SetName )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszName);
        
        HRESULT ( STDMETHODCALLTYPE *GetNamespace )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *SetNamespace )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszNamespace);
        
        HRESULT ( STDMETHODCALLTYPE *BeginUpdate )( 
            IVsServiceReferenceMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndUpdate )( 
            IVsServiceReferenceMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *RunProxyGenerator )( 
            IVsServiceReferenceMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadMapFileData )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent);
        
        HRESULT ( STDMETHODCALLTYPE *SaveMapFileData )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in SAFEARRAY * content);
        
        HRESULT ( STDMETHODCALLTYPE *GetMapFilePath )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrMapFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *LoadMetadataFile )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pContent);
        
        HRESULT ( STDMETHODCALLTYPE *SaveMetadataFile )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName,
            /* [in] */ __RPC__in SAFEARRAY * content);
        
        HRESULT ( STDMETHODCALLTYPE *CreateMetadataFile )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataSuggestedName,
            /* [in] */ __RPC__in SAFEARRAY * content,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrRealName);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteMetadataFile )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in LPCOLESTR pszMetadataFileName);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseServiceReferenceMetadataStorageEvents )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ __RPC__in_opt IVsServiceReferenceMetadataStorageEvents *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseServiceReferenceMetadataStorageEvents )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IVsServiceReferenceMetadataStorage * This);
        
        HRESULT ( STDMETHODCALLTYPE *ProjectItemId )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__out VSITEMID *pProjectItemId);
        
        HRESULT ( STDMETHODCALLTYPE *FileCodeModel )( 
            IVsServiceReferenceMetadataStorage * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **ppFileCodeModel);
        
        END_INTERFACE
    } IVsServiceReferenceMetadataStorageVtbl;

    interface IVsServiceReferenceMetadataStorage
    {
        CONST_VTBL struct IVsServiceReferenceMetadataStorageVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsServiceReferenceMetadataStorage_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsServiceReferenceMetadataStorage_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsServiceReferenceMetadataStorage_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsServiceReferenceMetadataStorage_GetStorageType(This,pbstrStorageType)	\
    ( (This)->lpVtbl -> GetStorageType(This,pbstrStorageType) ) 

#define IVsServiceReferenceMetadataStorage_GetName(This,pbstrName)	\
    ( (This)->lpVtbl -> GetName(This,pbstrName) ) 

#define IVsServiceReferenceMetadataStorage_SetName(This,pszName)	\
    ( (This)->lpVtbl -> SetName(This,pszName) ) 

#define IVsServiceReferenceMetadataStorage_GetNamespace(This,pbstrNamespace)	\
    ( (This)->lpVtbl -> GetNamespace(This,pbstrNamespace) ) 

#define IVsServiceReferenceMetadataStorage_SetNamespace(This,pszNamespace)	\
    ( (This)->lpVtbl -> SetNamespace(This,pszNamespace) ) 

#define IVsServiceReferenceMetadataStorage_BeginUpdate(This)	\
    ( (This)->lpVtbl -> BeginUpdate(This) ) 

#define IVsServiceReferenceMetadataStorage_EndUpdate(This)	\
    ( (This)->lpVtbl -> EndUpdate(This) ) 

#define IVsServiceReferenceMetadataStorage_RunProxyGenerator(This)	\
    ( (This)->lpVtbl -> RunProxyGenerator(This) ) 

#define IVsServiceReferenceMetadataStorage_LoadMapFileData(This,pContent)	\
    ( (This)->lpVtbl -> LoadMapFileData(This,pContent) ) 

#define IVsServiceReferenceMetadataStorage_SaveMapFileData(This,content)	\
    ( (This)->lpVtbl -> SaveMapFileData(This,content) ) 

#define IVsServiceReferenceMetadataStorage_GetMapFilePath(This,pbstrMapFilePath)	\
    ( (This)->lpVtbl -> GetMapFilePath(This,pbstrMapFilePath) ) 

#define IVsServiceReferenceMetadataStorage_LoadMetadataFile(This,pszMetadataFileName,pContent)	\
    ( (This)->lpVtbl -> LoadMetadataFile(This,pszMetadataFileName,pContent) ) 

#define IVsServiceReferenceMetadataStorage_SaveMetadataFile(This,pszMetadataFileName,content)	\
    ( (This)->lpVtbl -> SaveMetadataFile(This,pszMetadataFileName,content) ) 

#define IVsServiceReferenceMetadataStorage_CreateMetadataFile(This,pszMetadataSuggestedName,content,pbstrRealName)	\
    ( (This)->lpVtbl -> CreateMetadataFile(This,pszMetadataSuggestedName,content,pbstrRealName) ) 

#define IVsServiceReferenceMetadataStorage_DeleteMetadataFile(This,pszMetadataFileName)	\
    ( (This)->lpVtbl -> DeleteMetadataFile(This,pszMetadataFileName) ) 

#define IVsServiceReferenceMetadataStorage_AdviseServiceReferenceMetadataStorageEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseServiceReferenceMetadataStorageEvents(This,pSink,pdwCookie) ) 

#define IVsServiceReferenceMetadataStorage_UnadviseServiceReferenceMetadataStorageEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseServiceReferenceMetadataStorageEvents(This,dwCookie) ) 

#define IVsServiceReferenceMetadataStorage_Remove(This)	\
    ( (This)->lpVtbl -> Remove(This) ) 

#define IVsServiceReferenceMetadataStorage_ProjectItemId(This,pProjectItemId)	\
    ( (This)->lpVtbl -> ProjectItemId(This,pProjectItemId) ) 

#define IVsServiceReferenceMetadataStorage_FileCodeModel(This,ppFileCodeModel)	\
    ( (This)->lpVtbl -> FileCodeModel(This,ppFileCodeModel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsServiceReferenceMetadataStorage_INTERFACE_DEFINED__ */


#ifndef __IVsServiceReferenceMetadataStorageEvents_INTERFACE_DEFINED__
#define __IVsServiceReferenceMetadataStorageEvents_INTERFACE_DEFINED__

/* interface IVsServiceReferenceMetadataStorageEvents */
/* [object][uuid][unique][version] */ 


EXTERN_C const IID IID_IVsServiceReferenceMetadataStorageEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C6DF0D84-83E9-4e07-BA41-903A03763C5B")
    IVsServiceReferenceMetadataStorageEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnMapFileUpdated( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileAdded( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileDeleted( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileRenamed( 
            /* [in] */ __RPC__in LPOLESTR pszFileNameOld,
            /* [in] */ __RPC__in LPOLESTR pszFileNameNew) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnMetadataFileUpdated( 
            /* [in] */ __RPC__in LPOLESTR pszFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsServiceReferenceMetadataStorageEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsServiceReferenceMetadataStorageEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsServiceReferenceMetadataStorageEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsServiceReferenceMetadataStorageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnMapFileUpdated )( 
            IVsServiceReferenceMetadataStorageEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileAdded )( 
            IVsServiceReferenceMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileDeleted )( 
            IVsServiceReferenceMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileRenamed )( 
            IVsServiceReferenceMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileNameOld,
            /* [in] */ __RPC__in LPOLESTR pszFileNameNew);
        
        HRESULT ( STDMETHODCALLTYPE *OnMetadataFileUpdated )( 
            IVsServiceReferenceMetadataStorageEvents * This,
            /* [in] */ __RPC__in LPOLESTR pszFileName);
        
        END_INTERFACE
    } IVsServiceReferenceMetadataStorageEventsVtbl;

    interface IVsServiceReferenceMetadataStorageEvents
    {
        CONST_VTBL struct IVsServiceReferenceMetadataStorageEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsServiceReferenceMetadataStorageEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsServiceReferenceMetadataStorageEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsServiceReferenceMetadataStorageEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsServiceReferenceMetadataStorageEvents_OnMapFileUpdated(This)	\
    ( (This)->lpVtbl -> OnMapFileUpdated(This) ) 

#define IVsServiceReferenceMetadataStorageEvents_OnMetadataFileAdded(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileAdded(This,pszFileName) ) 

#define IVsServiceReferenceMetadataStorageEvents_OnMetadataFileDeleted(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileDeleted(This,pszFileName) ) 

#define IVsServiceReferenceMetadataStorageEvents_OnMetadataFileRenamed(This,pszFileNameOld,pszFileNameNew)	\
    ( (This)->lpVtbl -> OnMetadataFileRenamed(This,pszFileNameOld,pszFileNameNew) ) 

#define IVsServiceReferenceMetadataStorageEvents_OnMetadataFileUpdated(This,pszFileName)	\
    ( (This)->lpVtbl -> OnMetadataFileUpdated(This,pszFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsServiceReferenceMetadataStorageEvents_INTERFACE_DEFINED__ */

#endif /* __WCFReferencesLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


