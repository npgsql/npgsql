

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

#ifndef __vssolutn_h__
#define __vssolutn_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsExposedObjectProvider_FWD_DEFINED__
#define __IVsExposedObjectProvider_FWD_DEFINED__
typedef interface IVsExposedObjectProvider IVsExposedObjectProvider;

#endif 	/* __IVsExposedObjectProvider_FWD_DEFINED__ */


#ifndef __IVsExposedObjectProvider2_FWD_DEFINED__
#define __IVsExposedObjectProvider2_FWD_DEFINED__
typedef interface IVsExposedObjectProvider2 IVsExposedObjectProvider2;

#endif 	/* __IVsExposedObjectProvider2_FWD_DEFINED__ */


#ifndef __IVsEnumExposedObjects_FWD_DEFINED__
#define __IVsEnumExposedObjects_FWD_DEFINED__
typedef interface IVsEnumExposedObjects IVsEnumExposedObjects;

#endif 	/* __IVsEnumExposedObjects_FWD_DEFINED__ */


#ifndef __IVsExposedObject_FWD_DEFINED__
#define __IVsExposedObject_FWD_DEFINED__
typedef interface IVsExposedObject IVsExposedObject;

#endif 	/* __IVsExposedObject_FWD_DEFINED__ */


#ifndef __IVsExposedCOMServer_FWD_DEFINED__
#define __IVsExposedCOMServer_FWD_DEFINED__
typedef interface IVsExposedCOMServer IVsExposedCOMServer;

#endif 	/* __IVsExposedCOMServer_FWD_DEFINED__ */


#ifndef __IVsExposedCOMServer2_FWD_DEFINED__
#define __IVsExposedCOMServer2_FWD_DEFINED__
typedef interface IVsExposedCOMServer2 IVsExposedCOMServer2;

#endif 	/* __IVsExposedCOMServer2_FWD_DEFINED__ */


#ifndef __IVsExposedMainExe_FWD_DEFINED__
#define __IVsExposedMainExe_FWD_DEFINED__
typedef interface IVsExposedMainExe IVsExposedMainExe;

#endif 	/* __IVsExposedMainExe_FWD_DEFINED__ */


#ifndef __IVsPersistPropertyStream_FWD_DEFINED__
#define __IVsPersistPropertyStream_FWD_DEFINED__
typedef interface IVsPersistPropertyStream IVsPersistPropertyStream;

#endif 	/* __IVsPersistPropertyStream_FWD_DEFINED__ */


#ifndef __IVsSolutionDebuggingAssistant_FWD_DEFINED__
#define __IVsSolutionDebuggingAssistant_FWD_DEFINED__
typedef interface IVsSolutionDebuggingAssistant IVsSolutionDebuggingAssistant;

#endif 	/* __IVsSolutionDebuggingAssistant_FWD_DEFINED__ */


#ifndef __IVsSolutionBuilder_FWD_DEFINED__
#define __IVsSolutionBuilder_FWD_DEFINED__
typedef interface IVsSolutionBuilder IVsSolutionBuilder;

#endif 	/* __IVsSolutionBuilder_FWD_DEFINED__ */


#ifndef __IVsGroupSelectionDialog_FWD_DEFINED__
#define __IVsGroupSelectionDialog_FWD_DEFINED__
typedef interface IVsGroupSelectionDialog IVsGroupSelectionDialog;

#endif 	/* __IVsGroupSelectionDialog_FWD_DEFINED__ */


#ifndef __IVsProjectFilter_FWD_DEFINED__
#define __IVsProjectFilter_FWD_DEFINED__
typedef interface IVsProjectFilter IVsProjectFilter;

#endif 	/* __IVsProjectFilter_FWD_DEFINED__ */


#ifndef __IVsPropertyInterfaceBroker_FWD_DEFINED__
#define __IVsPropertyInterfaceBroker_FWD_DEFINED__
typedef interface IVsPropertyInterfaceBroker IVsPropertyInterfaceBroker;

#endif 	/* __IVsPropertyInterfaceBroker_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vssolutn_0000_0000 */
/* [local] */ 




#define CP_VSDPL CP_UTF8
#define	VS_MAX_PROJECT_CFG_CANONICAL_NAME	( 255 )

#define SBF_SUPPRESS_NOTEXISTS_QUERY SBF_SUPPRESS_NONE


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0000_v0_0_s_ifspec;

#ifndef __IVsExposedObjectProvider_INTERFACE_DEFINED__
#define __IVsExposedObjectProvider_INTERFACE_DEFINED__

/* interface IVsExposedObjectProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedObjectProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027ca-8c1f-11d0-8a34-00a0c91e2acd")
    IVsExposedObjectProvider : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumExposedObjects( 
            /* [out] */ __RPC__deref_out_opt IVsEnumExposedObjects **ppIVsEnumExposedObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenExposedObject( 
            /* [in] */ __RPC__in LPCOLESTR szExposedObjectCanonicalName,
            /* [out] */ __RPC__deref_out_opt IVsExposedObject **ppIVsExposedObject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedObjectProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedObjectProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedObjectProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedObjectProvider * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumExposedObjects )( 
            __RPC__in IVsExposedObjectProvider * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumExposedObjects **ppIVsEnumExposedObject);
        
        HRESULT ( STDMETHODCALLTYPE *OpenExposedObject )( 
            __RPC__in IVsExposedObjectProvider * This,
            /* [in] */ __RPC__in LPCOLESTR szExposedObjectCanonicalName,
            /* [out] */ __RPC__deref_out_opt IVsExposedObject **ppIVsExposedObject);
        
        END_INTERFACE
    } IVsExposedObjectProviderVtbl;

    interface IVsExposedObjectProvider
    {
        CONST_VTBL struct IVsExposedObjectProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedObjectProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedObjectProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedObjectProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedObjectProvider_EnumExposedObjects(This,ppIVsEnumExposedObject)	\
    ( (This)->lpVtbl -> EnumExposedObjects(This,ppIVsEnumExposedObject) ) 

#define IVsExposedObjectProvider_OpenExposedObject(This,szExposedObjectCanonicalName,ppIVsExposedObject)	\
    ( (This)->lpVtbl -> OpenExposedObject(This,szExposedObjectCanonicalName,ppIVsExposedObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedObjectProvider_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0001 */
/* [local] */ 

#define VS_EOSUMMARY_GENERAL_OBJECT_PRESENT			0x00000001


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0001_v0_0_s_ifspec;

#ifndef __IVsExposedObjectProvider2_INTERFACE_DEFINED__
#define __IVsExposedObjectProvider2_INTERFACE_DEFINED__

/* interface IVsExposedObjectProvider2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedObjectProvider2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2bc4e9c5-66b3-11d1-b194-00a0c91e2acd")
    IVsExposedObjectProvider2 : public IVsExposedObjectProvider
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_ContentsSummary( 
            /* [in] */ __RPC__in REFGUID guidType,
            /* [out] */ __RPC__out ULONG *pdwSummary) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedObjectProvider2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedObjectProvider2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedObjectProvider2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedObjectProvider2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumExposedObjects )( 
            __RPC__in IVsExposedObjectProvider2 * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumExposedObjects **ppIVsEnumExposedObject);
        
        HRESULT ( STDMETHODCALLTYPE *OpenExposedObject )( 
            __RPC__in IVsExposedObjectProvider2 * This,
            /* [in] */ __RPC__in LPCOLESTR szExposedObjectCanonicalName,
            /* [out] */ __RPC__deref_out_opt IVsExposedObject **ppIVsExposedObject);
        
        HRESULT ( STDMETHODCALLTYPE *get_ContentsSummary )( 
            __RPC__in IVsExposedObjectProvider2 * This,
            /* [in] */ __RPC__in REFGUID guidType,
            /* [out] */ __RPC__out ULONG *pdwSummary);
        
        END_INTERFACE
    } IVsExposedObjectProvider2Vtbl;

    interface IVsExposedObjectProvider2
    {
        CONST_VTBL struct IVsExposedObjectProvider2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedObjectProvider2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedObjectProvider2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedObjectProvider2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedObjectProvider2_EnumExposedObjects(This,ppIVsEnumExposedObject)	\
    ( (This)->lpVtbl -> EnumExposedObjects(This,ppIVsEnumExposedObject) ) 

#define IVsExposedObjectProvider2_OpenExposedObject(This,szExposedObjectCanonicalName,ppIVsExposedObject)	\
    ( (This)->lpVtbl -> OpenExposedObject(This,szExposedObjectCanonicalName,ppIVsExposedObject) ) 


#define IVsExposedObjectProvider2_get_ContentsSummary(This,guidType,pdwSummary)	\
    ( (This)->lpVtbl -> get_ContentsSummary(This,guidType,pdwSummary) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedObjectProvider2_INTERFACE_DEFINED__ */


#ifndef __IVsEnumExposedObjects_INTERFACE_DEFINED__
#define __IVsEnumExposedObjects_INTERFACE_DEFINED__

/* interface IVsEnumExposedObjects */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumExposedObjects;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027bc-8c1f-11d0-8a34-00a0c91e2acd")
    IVsEnumExposedObjects : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG cElements,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cElements) IVsExposedObject *rgpIVsExposedObject[  ],
            /* [out] */ __RPC__out ULONG *pcElementsFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG cElements) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsEnumExposedObjects **ppIVsEnumExposedObjects) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsEnumExposedObjectsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsEnumExposedObjects * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsEnumExposedObjects * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsEnumExposedObjects * This);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsEnumExposedObjects * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            __RPC__in IVsEnumExposedObjects * This,
            /* [in] */ ULONG cElements,
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cElements) IVsExposedObject *rgpIVsExposedObject[  ],
            /* [out] */ __RPC__out ULONG *pcElementsFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            __RPC__in IVsEnumExposedObjects * This,
            /* [in] */ ULONG cElements);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            __RPC__in IVsEnumExposedObjects * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumExposedObjects **ppIVsEnumExposedObjects);
        
        END_INTERFACE
    } IVsEnumExposedObjectsVtbl;

    interface IVsEnumExposedObjects
    {
        CONST_VTBL struct IVsEnumExposedObjectsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumExposedObjects_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumExposedObjects_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumExposedObjects_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumExposedObjects_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsEnumExposedObjects_Next(This,cElements,rgpIVsExposedObject,pcElementsFetched)	\
    ( (This)->lpVtbl -> Next(This,cElements,rgpIVsExposedObject,pcElementsFetched) ) 

#define IVsEnumExposedObjects_Skip(This,cElements)	\
    ( (This)->lpVtbl -> Skip(This,cElements) ) 

#define IVsEnumExposedObjects_Clone(This,ppIVsEnumExposedObjects)	\
    ( (This)->lpVtbl -> Clone(This,ppIVsEnumExposedObjects) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumExposedObjects_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0003 */
/* [local] */ 

extern const __declspec(selectany) GUID GUID_VS_EOTYPE_COM_CLASS = { /* 707d11aa-91ca-11d0-8a3e-00a0c91e2acd */
    0x707d11aa,
    0x91ca,
    0x11d0,
    {0x8a, 0x3e, 0x00, 0xa0, 0xc9, 0x1e, 0x2a, 0xcd}
  };
extern const __declspec(selectany) GUID GUID_VS_EOTYPE_MAIN_EXE = { /* 707d11b8-91ca-11d0-8a3e-00a0c91e2acd */
    0x707d11b8,
    0x91ca,
    0x11d0,
    {0x8a, 0x3e, 0x00, 0xa0, 0xc9, 0x1e, 0x2a, 0xcd}
  };


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0003_v0_0_s_ifspec;

#ifndef __IVsExposedObject_INTERFACE_DEFINED__
#define __IVsExposedObject_INTERFACE_DEFINED__

/* interface IVsExposedObject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027bd-8c1f-11d0-8a34-00a0c91e2acd")
    IVsExposedObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Description( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Type( 
            /* [out] */ __RPC__out GUID *pguidType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_HelpContext( 
            /* [out] */ __RPC__out DWORD *pdwHelpContext) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_HelpFile( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFile) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsExposedObject * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsExposedObject * This,
            /* [out] */ __RPC__out GUID *pguidType);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpContext )( 
            __RPC__in IVsExposedObject * This,
            /* [out] */ __RPC__out DWORD *pdwHelpContext);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpFile )( 
            __RPC__in IVsExposedObject * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFile);
        
        END_INTERFACE
    } IVsExposedObjectVtbl;

    interface IVsExposedObject
    {
        CONST_VTBL struct IVsExposedObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedObject_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsExposedObject_get_Type(This,pguidType)	\
    ( (This)->lpVtbl -> get_Type(This,pguidType) ) 

#define IVsExposedObject_get_HelpContext(This,pdwHelpContext)	\
    ( (This)->lpVtbl -> get_HelpContext(This,pdwHelpContext) ) 

#define IVsExposedObject_get_HelpFile(This,pbstrHelpFile)	\
    ( (This)->lpVtbl -> get_HelpFile(This,pbstrHelpFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedObject_INTERFACE_DEFINED__ */


#ifndef __IVsExposedCOMServer_INTERFACE_DEFINED__
#define __IVsExposedCOMServer_INTERFACE_DEFINED__

/* interface IVsExposedCOMServer */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedCOMServer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027be-8c1f-11d0-8a34-00a0c91e2acd")
    IVsExposedCOMServer : public IVsExposedObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_CLSID( 
            /* [out] */ __RPC__out CLSID *pclsid) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedCOMServerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedCOMServer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedCOMServer * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [out] */ __RPC__out GUID *pguidType);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpContext )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [out] */ __RPC__out DWORD *pdwHelpContext);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpFile )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFile);
        
        HRESULT ( STDMETHODCALLTYPE *get_CLSID )( 
            __RPC__in IVsExposedCOMServer * This,
            /* [out] */ __RPC__out CLSID *pclsid);
        
        END_INTERFACE
    } IVsExposedCOMServerVtbl;

    interface IVsExposedCOMServer
    {
        CONST_VTBL struct IVsExposedCOMServerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedCOMServer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedCOMServer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedCOMServer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedCOMServer_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsExposedCOMServer_get_Type(This,pguidType)	\
    ( (This)->lpVtbl -> get_Type(This,pguidType) ) 

#define IVsExposedCOMServer_get_HelpContext(This,pdwHelpContext)	\
    ( (This)->lpVtbl -> get_HelpContext(This,pdwHelpContext) ) 

#define IVsExposedCOMServer_get_HelpFile(This,pbstrHelpFile)	\
    ( (This)->lpVtbl -> get_HelpFile(This,pbstrHelpFile) ) 


#define IVsExposedCOMServer_get_CLSID(This,pclsid)	\
    ( (This)->lpVtbl -> get_CLSID(This,pclsid) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedCOMServer_INTERFACE_DEFINED__ */


#ifndef __IVsExposedCOMServer2_INTERFACE_DEFINED__
#define __IVsExposedCOMServer2_INTERFACE_DEFINED__

/* interface IVsExposedCOMServer2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedCOMServer2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2bc4e9c6-66b3-11d1-b194-00a0c91e2acd")
    IVsExposedCOMServer2 : public IVsExposedCOMServer
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_TypeLibId( 
            /* [out] */ __RPC__out CLSID *pclsid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_ProgId( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProgid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Version( 
            /* [out] */ __RPC__out ULONG *pulVersion) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedCOMServer2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedCOMServer2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedCOMServer2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__out GUID *pguidType);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpContext )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__out DWORD *pdwHelpContext);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpFile )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFile);
        
        HRESULT ( STDMETHODCALLTYPE *get_CLSID )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__out CLSID *pclsid);
        
        HRESULT ( STDMETHODCALLTYPE *get_TypeLibId )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__out CLSID *pclsid);
        
        HRESULT ( STDMETHODCALLTYPE *get_ProgId )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProgid);
        
        HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            __RPC__in IVsExposedCOMServer2 * This,
            /* [out] */ __RPC__out ULONG *pulVersion);
        
        END_INTERFACE
    } IVsExposedCOMServer2Vtbl;

    interface IVsExposedCOMServer2
    {
        CONST_VTBL struct IVsExposedCOMServer2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedCOMServer2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedCOMServer2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedCOMServer2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedCOMServer2_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsExposedCOMServer2_get_Type(This,pguidType)	\
    ( (This)->lpVtbl -> get_Type(This,pguidType) ) 

#define IVsExposedCOMServer2_get_HelpContext(This,pdwHelpContext)	\
    ( (This)->lpVtbl -> get_HelpContext(This,pdwHelpContext) ) 

#define IVsExposedCOMServer2_get_HelpFile(This,pbstrHelpFile)	\
    ( (This)->lpVtbl -> get_HelpFile(This,pbstrHelpFile) ) 


#define IVsExposedCOMServer2_get_CLSID(This,pclsid)	\
    ( (This)->lpVtbl -> get_CLSID(This,pclsid) ) 


#define IVsExposedCOMServer2_get_TypeLibId(This,pclsid)	\
    ( (This)->lpVtbl -> get_TypeLibId(This,pclsid) ) 

#define IVsExposedCOMServer2_get_ProgId(This,pbstrProgid)	\
    ( (This)->lpVtbl -> get_ProgId(This,pbstrProgid) ) 

#define IVsExposedCOMServer2_get_Version(This,pulVersion)	\
    ( (This)->lpVtbl -> get_Version(This,pulVersion) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedCOMServer2_INTERFACE_DEFINED__ */


#ifndef __IVsExposedMainExe_INTERFACE_DEFINED__
#define __IVsExposedMainExe_INTERFACE_DEFINED__

/* interface IVsExposedMainExe */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsExposedMainExe;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027de-8c1f-11d0-8a34-00a0c91e2acd")
    IVsExposedMainExe : public IVsExposedObject
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_CommandLine( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsExposedMainExeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsExposedMainExe * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsExposedMainExe * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsExposedMainExe * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in IVsExposedMainExe * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDescription);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsExposedMainExe * This,
            /* [out] */ __RPC__out GUID *pguidType);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpContext )( 
            __RPC__in IVsExposedMainExe * This,
            /* [out] */ __RPC__out DWORD *pdwHelpContext);
        
        HRESULT ( STDMETHODCALLTYPE *get_HelpFile )( 
            __RPC__in IVsExposedMainExe * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrHelpFile);
        
        HRESULT ( STDMETHODCALLTYPE *get_CommandLine )( 
            __RPC__in IVsExposedMainExe * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrCommandLine);
        
        END_INTERFACE
    } IVsExposedMainExeVtbl;

    interface IVsExposedMainExe
    {
        CONST_VTBL struct IVsExposedMainExeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsExposedMainExe_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsExposedMainExe_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsExposedMainExe_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsExposedMainExe_get_Description(This,pbstrDescription)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrDescription) ) 

#define IVsExposedMainExe_get_Type(This,pguidType)	\
    ( (This)->lpVtbl -> get_Type(This,pguidType) ) 

#define IVsExposedMainExe_get_HelpContext(This,pdwHelpContext)	\
    ( (This)->lpVtbl -> get_HelpContext(This,pdwHelpContext) ) 

#define IVsExposedMainExe_get_HelpFile(This,pbstrHelpFile)	\
    ( (This)->lpVtbl -> get_HelpFile(This,pbstrHelpFile) ) 


#define IVsExposedMainExe_get_CommandLine(This,pbstrCommandLine)	\
    ( (This)->lpVtbl -> get_CommandLine(This,pbstrCommandLine) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsExposedMainExe_INTERFACE_DEFINED__ */


#ifndef __IVsPersistPropertyStream_INTERFACE_DEFINED__
#define __IVsPersistPropertyStream_INTERFACE_DEFINED__

/* interface IVsPersistPropertyStream */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPersistPropertyStream;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027cd-8c1f-11d0-8a34-00a0c91e2acd")
    IVsPersistPropertyStream : public IPersist
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InitNew( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ __RPC__in_opt IVsPropertyStreamIn *pIVsPersistPropertyStreamIn,
            /* [in] */ __RPC__in_opt IErrorLog *pIErrorLog) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( 
            /* [in] */ __RPC__in_opt IVsPropertyStreamOut *pIVsPersistPropertyStreamOut,
            /* [in] */ BOOL fClearDirty) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPersistPropertyStreamVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPersistPropertyStream * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPersistPropertyStream * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPersistPropertyStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassID )( 
            __RPC__in IVsPersistPropertyStream * This,
            /* [out] */ __RPC__out CLSID *pClassID);
        
        HRESULT ( STDMETHODCALLTYPE *InitNew )( 
            __RPC__in IVsPersistPropertyStream * This);
        
        HRESULT ( STDMETHODCALLTYPE *Load )( 
            __RPC__in IVsPersistPropertyStream * This,
            /* [in] */ __RPC__in_opt IVsPropertyStreamIn *pIVsPersistPropertyStreamIn,
            /* [in] */ __RPC__in_opt IErrorLog *pIErrorLog);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            __RPC__in IVsPersistPropertyStream * This,
            /* [in] */ __RPC__in_opt IVsPropertyStreamOut *pIVsPersistPropertyStreamOut,
            /* [in] */ BOOL fClearDirty);
        
        END_INTERFACE
    } IVsPersistPropertyStreamVtbl;

    interface IVsPersistPropertyStream
    {
        CONST_VTBL struct IVsPersistPropertyStreamVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPersistPropertyStream_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPersistPropertyStream_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPersistPropertyStream_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPersistPropertyStream_GetClassID(This,pClassID)	\
    ( (This)->lpVtbl -> GetClassID(This,pClassID) ) 


#define IVsPersistPropertyStream_InitNew(This)	\
    ( (This)->lpVtbl -> InitNew(This) ) 

#define IVsPersistPropertyStream_Load(This,pIVsPersistPropertyStreamIn,pIErrorLog)	\
    ( (This)->lpVtbl -> Load(This,pIVsPersistPropertyStreamIn,pIErrorLog) ) 

#define IVsPersistPropertyStream_Save(This,pIVsPersistPropertyStreamOut,fClearDirty)	\
    ( (This)->lpVtbl -> Save(This,pIVsPersistPropertyStreamOut,fClearDirty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPersistPropertyStream_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0008 */
/* [local] */ 

#define SID_SVsSolutionDebuggingAssistant IID_IVsSolutionDebuggingAssistant


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0008_v0_0_s_ifspec;

#ifndef __IVsSolutionDebuggingAssistant_INTERFACE_DEFINED__
#define __IVsSolutionDebuggingAssistant_INTERFACE_DEFINED__

/* interface IVsSolutionDebuggingAssistant */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionDebuggingAssistant;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("910E8282-F867-11d0-AB36-00A0C90F2713")
    IVsSolutionDebuggingAssistant : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MapDeployedURLToProjectItem( 
            /* [in] */ __RPC__in LPCOLESTR pszDUrl,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pphier,
            /* [out] */ __RPC__out VSITEMID *pitemid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapOutputToDeployedURL( 
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszOutputCanonicalName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDUrl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapProjectRelUrlToDeployedURL( 
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszProjectRelUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDUrl) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionDebuggingAssistantVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionDebuggingAssistant * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionDebuggingAssistant * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionDebuggingAssistant * This);
        
        HRESULT ( STDMETHODCALLTYPE *MapDeployedURLToProjectItem )( 
            __RPC__in IVsSolutionDebuggingAssistant * This,
            /* [in] */ __RPC__in LPCOLESTR pszDUrl,
            /* [out] */ __RPC__deref_out_opt IVsHierarchy **pphier,
            /* [out] */ __RPC__out VSITEMID *pitemid);
        
        HRESULT ( STDMETHODCALLTYPE *MapOutputToDeployedURL )( 
            __RPC__in IVsSolutionDebuggingAssistant * This,
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszOutputCanonicalName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDUrl);
        
        HRESULT ( STDMETHODCALLTYPE *MapProjectRelUrlToDeployedURL )( 
            __RPC__in IVsSolutionDebuggingAssistant * This,
            /* [in] */ __RPC__in_opt IVsProjectCfg *pProjectCfg,
            /* [in] */ __RPC__in LPCOLESTR pszProjectRelUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDUrl);
        
        END_INTERFACE
    } IVsSolutionDebuggingAssistantVtbl;

    interface IVsSolutionDebuggingAssistant
    {
        CONST_VTBL struct IVsSolutionDebuggingAssistantVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionDebuggingAssistant_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionDebuggingAssistant_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionDebuggingAssistant_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionDebuggingAssistant_MapDeployedURLToProjectItem(This,pszDUrl,pphier,pitemid)	\
    ( (This)->lpVtbl -> MapDeployedURLToProjectItem(This,pszDUrl,pphier,pitemid) ) 

#define IVsSolutionDebuggingAssistant_MapOutputToDeployedURL(This,pProjectCfg,pszOutputCanonicalName,pbstrDUrl)	\
    ( (This)->lpVtbl -> MapOutputToDeployedURL(This,pProjectCfg,pszOutputCanonicalName,pbstrDUrl) ) 

#define IVsSolutionDebuggingAssistant_MapProjectRelUrlToDeployedURL(This,pProjectCfg,pszProjectRelUrl,pbstrDUrl)	\
    ( (This)->lpVtbl -> MapProjectRelUrlToDeployedURL(This,pProjectCfg,pszProjectRelUrl,pbstrDUrl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionDebuggingAssistant_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0009 */
/* [local] */ 

#define SID_SVsSolutionBuilder IID_IVsSolutionBuilder


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0009_v0_0_s_ifspec;

#ifndef __IVsSolutionBuilder_INTERFACE_DEFINED__
#define __IVsSolutionBuilder_INTERFACE_DEFINED__

/* interface IVsSolutionBuilder */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsSolutionBuilder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027ce-8c1f-11d0-8a34-00a0c91e2acd")
    IVsSolutionBuilder : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseBuildStatusCallback( 
            /* [in] */ __RPC__in_opt IVsBuildStatusCallback *pIVsBuildStatusCallback,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseBuildStatusCallback( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryBuilderStatus( 
            /* [out] */ __RPC__out BOOL *pfBuilderBusy) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSolutionBuilderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSolutionBuilder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSolutionBuilder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSolutionBuilder * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseBuildStatusCallback )( 
            __RPC__in IVsSolutionBuilder * This,
            /* [in] */ __RPC__in_opt IVsBuildStatusCallback *pIVsBuildStatusCallback,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseBuildStatusCallback )( 
            __RPC__in IVsSolutionBuilder * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *QueryBuilderStatus )( 
            __RPC__in IVsSolutionBuilder * This,
            /* [out] */ __RPC__out BOOL *pfBuilderBusy);
        
        END_INTERFACE
    } IVsSolutionBuilderVtbl;

    interface IVsSolutionBuilder
    {
        CONST_VTBL struct IVsSolutionBuilderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSolutionBuilder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSolutionBuilder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSolutionBuilder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSolutionBuilder_AdviseBuildStatusCallback(This,pIVsBuildStatusCallback,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseBuildStatusCallback(This,pIVsBuildStatusCallback,pdwCookie) ) 

#define IVsSolutionBuilder_UnadviseBuildStatusCallback(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseBuildStatusCallback(This,dwCookie) ) 

#define IVsSolutionBuilder_QueryBuilderStatus(This,pfBuilderBusy)	\
    ( (This)->lpVtbl -> QueryBuilderStatus(This,pfBuilderBusy) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSolutionBuilder_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0010 */
/* [local] */ 


enum __VSSOLNBUILDERACTION
    {
        VSSBA_BUILD	= 0,
        VSSBA_REBUILD	= 1,
        VSSBA_CLEAN	= 2,
        VSSBA_DEPLOY	= 3
    } ;
typedef DWORD VSSOLNBUILDERACTION;



extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0010_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0010_v0_0_s_ifspec;

#ifndef __IVsGroupSelectionDialog_INTERFACE_DEFINED__
#define __IVsGroupSelectionDialog_INTERFACE_DEFINED__

/* interface IVsGroupSelectionDialog */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsGroupSelectionDialog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2B3B3A2D-4E79-11d3-9477-00C04F683646")
    IVsGroupSelectionDialog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DoModal( 
            /* [in] */ BOOL fMultipleSelectionEnabled,
            /* [in] */ __RPC__in_opt IVsProjectFilter *pIVsProjectFilter,
            /* [out] */ __RPC__out VARIANT *pvGroupCanonicalNames,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectGuid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrConfigurationCanonicalName,
            /* [out] */ __RPC__out BOOL *pfCancelled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsGroupSelectionDialogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsGroupSelectionDialog * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsGroupSelectionDialog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsGroupSelectionDialog * This);
        
        HRESULT ( STDMETHODCALLTYPE *DoModal )( 
            __RPC__in IVsGroupSelectionDialog * This,
            /* [in] */ BOOL fMultipleSelectionEnabled,
            /* [in] */ __RPC__in_opt IVsProjectFilter *pIVsProjectFilter,
            /* [out] */ __RPC__out VARIANT *pvGroupCanonicalNames,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrProjectGuid,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrConfigurationCanonicalName,
            /* [out] */ __RPC__out BOOL *pfCancelled);
        
        END_INTERFACE
    } IVsGroupSelectionDialogVtbl;

    interface IVsGroupSelectionDialog
    {
        CONST_VTBL struct IVsGroupSelectionDialogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsGroupSelectionDialog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsGroupSelectionDialog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsGroupSelectionDialog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsGroupSelectionDialog_DoModal(This,fMultipleSelectionEnabled,pIVsProjectFilter,pvGroupCanonicalNames,pbstrProjectGuid,pbstrConfigurationCanonicalName,pfCancelled)	\
    ( (This)->lpVtbl -> DoModal(This,fMultipleSelectionEnabled,pIVsProjectFilter,pvGroupCanonicalNames,pbstrProjectGuid,pbstrConfigurationCanonicalName,pfCancelled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsGroupSelectionDialog_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_vssolutn_0000_0011 */
/* [local] */ 

#define SID_SVsGroupSelectionDialog IID_IVsGroupSelectionDialog


extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssolutn_0000_0011_v0_0_s_ifspec;

#ifndef __IVsProjectFilter_INTERFACE_DEFINED__
#define __IVsProjectFilter_INTERFACE_DEFINED__

/* interface IVsProjectFilter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsProjectFilter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027d5-8c1f-11d0-8a34-00a0c91e2acd")
    IVsProjectFilter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DoFilter( 
            /* [in] */ ULONG cproj,
            /* [size_is][in] */ __RPC__in_ecount_full(cproj) IVsHierarchy *rgpIVsHierarchy[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cproj) BOOL rgfAllowProject[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsProjectFilterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsProjectFilter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsProjectFilter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsProjectFilter * This);
        
        HRESULT ( STDMETHODCALLTYPE *DoFilter )( 
            __RPC__in IVsProjectFilter * This,
            /* [in] */ ULONG cproj,
            /* [size_is][in] */ __RPC__in_ecount_full(cproj) IVsHierarchy *rgpIVsHierarchy[  ],
            /* [size_is][out][in] */ __RPC__inout_ecount_full(cproj) BOOL rgfAllowProject[  ]);
        
        END_INTERFACE
    } IVsProjectFilterVtbl;

    interface IVsProjectFilter
    {
        CONST_VTBL struct IVsProjectFilterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProjectFilter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProjectFilter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProjectFilter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProjectFilter_DoFilter(This,cproj,rgpIVsHierarchy,rgfAllowProject)	\
    ( (This)->lpVtbl -> DoFilter(This,cproj,rgpIVsHierarchy,rgfAllowProject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProjectFilter_INTERFACE_DEFINED__ */


#ifndef __IVsPropertyInterfaceBroker_INTERFACE_DEFINED__
#define __IVsPropertyInterfaceBroker_INTERFACE_DEFINED__

/* interface IVsPropertyInterfaceBroker */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsPropertyInterfaceBroker;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d0b027d8-8c1f-11d0-8a34-00a0c91e2acd")
    IVsPropertyInterfaceBroker : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryPropertyInterface( 
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsPropertyInterfaceBrokerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsPropertyInterfaceBroker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsPropertyInterfaceBroker * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsPropertyInterfaceBroker * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryPropertyInterface )( 
            __RPC__in IVsPropertyInterfaceBroker * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ __RPC__deref_out_opt void **ppvObject);
        
        END_INTERFACE
    } IVsPropertyInterfaceBrokerVtbl;

    interface IVsPropertyInterfaceBroker
    {
        CONST_VTBL struct IVsPropertyInterfaceBrokerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsPropertyInterfaceBroker_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsPropertyInterfaceBroker_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsPropertyInterfaceBroker_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsPropertyInterfaceBroker_QueryPropertyInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryPropertyInterface(This,riid,ppvObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsPropertyInterfaceBroker_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     __RPC__in unsigned long *, __RPC__in VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


