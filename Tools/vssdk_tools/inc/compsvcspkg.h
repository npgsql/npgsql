

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for compsvcspkg.idl:
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

#ifndef __compsvcspkg_h__
#define __compsvcspkg_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsAddWebReferenceDlg_FWD_DEFINED__
#define __IVsAddWebReferenceDlg_FWD_DEFINED__
typedef interface IVsAddWebReferenceDlg IVsAddWebReferenceDlg;
#endif 	/* __IVsAddWebReferenceDlg_FWD_DEFINED__ */


#ifndef __IVsAddWebReferenceDlg2_FWD_DEFINED__
#define __IVsAddWebReferenceDlg2_FWD_DEFINED__
typedef interface IVsAddWebReferenceDlg2 IVsAddWebReferenceDlg2;
#endif 	/* __IVsAddWebReferenceDlg2_FWD_DEFINED__ */


#ifndef __IEnumComponents_FWD_DEFINED__
#define __IEnumComponents_FWD_DEFINED__
typedef interface IEnumComponents IEnumComponents;
#endif 	/* __IEnumComponents_FWD_DEFINED__ */


#ifndef __IVsComponentSelectorData_FWD_DEFINED__
#define __IVsComponentSelectorData_FWD_DEFINED__
typedef interface IVsComponentSelectorData IVsComponentSelectorData;
#endif 	/* __IVsComponentSelectorData_FWD_DEFINED__ */


#ifndef __IVsComponentEnumeratorFactory_FWD_DEFINED__
#define __IVsComponentEnumeratorFactory_FWD_DEFINED__
typedef interface IVsComponentEnumeratorFactory IVsComponentEnumeratorFactory;
#endif 	/* __IVsComponentEnumeratorFactory_FWD_DEFINED__ */


#ifndef __IVsComponentEnumeratorFactory2_FWD_DEFINED__
#define __IVsComponentEnumeratorFactory2_FWD_DEFINED__
typedef interface IVsComponentEnumeratorFactory2 IVsComponentEnumeratorFactory2;
#endif 	/* __IVsComponentEnumeratorFactory2_FWD_DEFINED__ */


#ifndef __IVsProvideComponentEnumeration_FWD_DEFINED__
#define __IVsProvideComponentEnumeration_FWD_DEFINED__
typedef interface IVsProvideComponentEnumeration IVsProvideComponentEnumeration;
#endif 	/* __IVsProvideComponentEnumeration_FWD_DEFINED__ */


#ifndef __CCompServicesPackage_FWD_DEFINED__
#define __CCompServicesPackage_FWD_DEFINED__

#ifdef __cplusplus
typedef class CCompServicesPackage CCompServicesPackage;
#else
typedef struct CCompServicesPackage CCompServicesPackage;
#endif /* __cplusplus */

#endif 	/* __CCompServicesPackage_FWD_DEFINED__ */


#ifndef __CCom2Enumerator_FWD_DEFINED__
#define __CCom2Enumerator_FWD_DEFINED__

#ifdef __cplusplus
typedef class CCom2Enumerator CCom2Enumerator;
#else
typedef struct CCom2Enumerator CCom2Enumerator;
#endif /* __cplusplus */

#endif 	/* __CCom2Enumerator_FWD_DEFINED__ */


#ifndef __CComPlusEnumerator_FWD_DEFINED__
#define __CComPlusEnumerator_FWD_DEFINED__

#ifdef __cplusplus
typedef class CComPlusEnumerator CComPlusEnumerator;
#else
typedef struct CComPlusEnumerator CComPlusEnumerator;
#endif /* __cplusplus */

#endif 	/* __CComPlusEnumerator_FWD_DEFINED__ */


#ifndef __IVsTypeLibraryWrapperCallback_FWD_DEFINED__
#define __IVsTypeLibraryWrapperCallback_FWD_DEFINED__
typedef interface IVsTypeLibraryWrapperCallback IVsTypeLibraryWrapperCallback;
#endif 	/* __IVsTypeLibraryWrapperCallback_FWD_DEFINED__ */


#ifndef __IVsTypeLibraryWrapper_FWD_DEFINED__
#define __IVsTypeLibraryWrapper_FWD_DEFINED__
typedef interface IVsTypeLibraryWrapper IVsTypeLibraryWrapper;
#endif 	/* __IVsTypeLibraryWrapper_FWD_DEFINED__ */


#ifndef __VSPIAImporter_FWD_DEFINED__
#define __VSPIAImporter_FWD_DEFINED__

#ifdef __cplusplus
typedef class VSPIAImporter VSPIAImporter;
#else
typedef struct VSPIAImporter VSPIAImporter;
#endif /* __cplusplus */

#endif 	/* __VSPIAImporter_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"
#include "discoveryservice.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_compsvcspkg_0000_0000 */
/* [local] */ 


enum CompEnum
    {	CompEnumType_COM2	= 100,
	CompEnumType_COMPlus	= ( CompEnumType_COM2 + 1 ) ,
	CompEnumType_AssemblyPaths	= ( CompEnumType_COMPlus + 1 ) 
    } ;


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0000_v0_0_s_ifspec;

#ifndef __IVsAddWebReferenceDlg_INTERFACE_DEFINED__
#define __IVsAddWebReferenceDlg_INTERFACE_DEFINED__

/* interface IVsAddWebReferenceDlg */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsAddWebReferenceDlg;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BD71396F-39C6-4e3f-BBA2-79CE33A8B302")
    IVsAddWebReferenceDlg : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddWebReferenceDlg( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl,
            /* [out] */ __RPC__out BOOL *pfCancelled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAddWebReferenceDlgVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAddWebReferenceDlg * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAddWebReferenceDlg * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAddWebReferenceDlg * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebReferenceDlg )( 
            IVsAddWebReferenceDlg * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl,
            /* [out] */ __RPC__out BOOL *pfCancelled);
        
        END_INTERFACE
    } IVsAddWebReferenceDlgVtbl;

    interface IVsAddWebReferenceDlg
    {
        CONST_VTBL struct IVsAddWebReferenceDlgVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddWebReferenceDlg_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddWebReferenceDlg_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddWebReferenceDlg_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddWebReferenceDlg_AddWebReferenceDlg(This,pbstrWebReferenceUrl,pfCancelled)	\
    ( (This)->lpVtbl -> AddWebReferenceDlg(This,pbstrWebReferenceUrl,pfCancelled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddWebReferenceDlg_INTERFACE_DEFINED__ */


#ifndef __IVsAddWebReferenceDlg2_INTERFACE_DEFINED__
#define __IVsAddWebReferenceDlg2_INTERFACE_DEFINED__

/* interface IVsAddWebReferenceDlg2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsAddWebReferenceDlg2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FE037B96-A8D1-4961-A3F3-E969094BA978")
    IVsAddWebReferenceDlg2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddWebReferenceDlg( 
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceName,
            /* [out] */ __RPC__deref_out_opt IDiscoveryResult **ppIDiscoveryResult,
            /* [out] */ __RPC__out BOOL *pfCancelled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAddWebReferenceDlg2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAddWebReferenceDlg2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAddWebReferenceDlg2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAddWebReferenceDlg2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddWebReferenceDlg )( 
            IVsAddWebReferenceDlg2 * This,
            /* [in] */ __RPC__in_opt IDiscoverySession *pDiscoverySession,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceUrl,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWebReferenceName,
            /* [out] */ __RPC__deref_out_opt IDiscoveryResult **ppIDiscoveryResult,
            /* [out] */ __RPC__out BOOL *pfCancelled);
        
        END_INTERFACE
    } IVsAddWebReferenceDlg2Vtbl;

    interface IVsAddWebReferenceDlg2
    {
        CONST_VTBL struct IVsAddWebReferenceDlg2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAddWebReferenceDlg2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAddWebReferenceDlg2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAddWebReferenceDlg2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAddWebReferenceDlg2_AddWebReferenceDlg(This,pDiscoverySession,pbstrWebReferenceUrl,pbstrWebReferenceName,ppIDiscoveryResult,pfCancelled)	\
    ( (This)->lpVtbl -> AddWebReferenceDlg(This,pDiscoverySession,pbstrWebReferenceUrl,pbstrWebReferenceName,ppIDiscoveryResult,pfCancelled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAddWebReferenceDlg2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg_0000_0002 */
/* [local] */ 

#define SID_SVsAddWebReferenceDlg IID_IVsAddWebReferenceDlg


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0002_v0_0_s_ifspec;

#ifndef __IEnumComponents_INTERFACE_DEFINED__
#define __IEnumComponents_INTERFACE_DEFINED__

/* interface IEnumComponents */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IEnumComponents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9a04b730-656c-11d3-85fc-00c04f6123b3")
    IEnumComponents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) VSCOMPONENTSELECTORDATA *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumComponents **ppIEnumComponents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumComponentsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumComponents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumComponents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumComponents * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumComponents * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) VSCOMPONENTSELECTORDATA *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumComponents * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumComponents * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumComponents * This,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **ppIEnumComponents);
        
        END_INTERFACE
    } IEnumComponentsVtbl;

    interface IEnumComponents
    {
        CONST_VTBL struct IEnumComponentsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumComponents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumComponents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumComponents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumComponents_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumComponents_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumComponents_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumComponents_Clone(This,ppIEnumComponents)	\
    ( (This)->lpVtbl -> Clone(This,ppIEnumComponents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumComponents_INTERFACE_DEFINED__ */


#ifndef __IVsComponentSelectorData_INTERFACE_DEFINED__
#define __IVsComponentSelectorData_INTERFACE_DEFINED__

/* interface IVsComponentSelectorData */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsComponentSelectorData;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("45bd8e74-6727-11d3-8600-00c04f6123b3")
    IVsComponentSelectorData : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetData( 
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pData) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentSelectorDataVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentSelectorData * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentSelectorData * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentSelectorData * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetData )( 
            IVsComponentSelectorData * This,
            /* [out] */ __RPC__out VSCOMPONENTSELECTORDATA *pData);
        
        END_INTERFACE
    } IVsComponentSelectorDataVtbl;

    interface IVsComponentSelectorData
    {
        CONST_VTBL struct IVsComponentSelectorDataVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentSelectorData_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentSelectorData_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentSelectorData_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentSelectorData_GetData(This,pData)	\
    ( (This)->lpVtbl -> GetData(This,pData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentSelectorData_INTERFACE_DEFINED__ */


#ifndef __IVsComponentEnumeratorFactory_INTERFACE_DEFINED__
#define __IVsComponentEnumeratorFactory_INTERFACE_DEFINED__

/* interface IVsComponentEnumeratorFactory */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsComponentEnumeratorFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("332cedee-6610-11d3-85fd-00c04f6123b3")
    IVsComponentEnumeratorFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComponents( 
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentEnumeratorFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentEnumeratorFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentEnumeratorFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentEnumeratorFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComponents )( 
            IVsComponentEnumeratorFactory * This,
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator);
        
        END_INTERFACE
    } IVsComponentEnumeratorFactoryVtbl;

    interface IVsComponentEnumeratorFactory
    {
        CONST_VTBL struct IVsComponentEnumeratorFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentEnumeratorFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentEnumeratorFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentEnumeratorFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentEnumeratorFactory_GetComponents(This,bstrMachineName,lEnumType,bForceRefresh,pEnumerator)	\
    ( (This)->lpVtbl -> GetComponents(This,bstrMachineName,lEnumType,bForceRefresh,pEnumerator) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentEnumeratorFactory_INTERFACE_DEFINED__ */


#ifndef __IVsComponentEnumeratorFactory2_INTERFACE_DEFINED__
#define __IVsComponentEnumeratorFactory2_INTERFACE_DEFINED__

/* interface IVsComponentEnumeratorFactory2 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsComponentEnumeratorFactory2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("64F6B8C1-3DEC-4606-8C8C-651A7E26A3DE")
    IVsComponentEnumeratorFactory2 : public IVsComponentEnumeratorFactory
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComponentsOfPath( 
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentEnumeratorFactory2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentEnumeratorFactory2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentEnumeratorFactory2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentEnumeratorFactory2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComponents )( 
            IVsComponentEnumeratorFactory2 * This,
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator);
        
        HRESULT ( STDMETHODCALLTYPE *GetComponentsOfPath )( 
            IVsComponentEnumeratorFactory2 * This,
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator);
        
        END_INTERFACE
    } IVsComponentEnumeratorFactory2Vtbl;

    interface IVsComponentEnumeratorFactory2
    {
        CONST_VTBL struct IVsComponentEnumeratorFactory2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentEnumeratorFactory2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentEnumeratorFactory2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentEnumeratorFactory2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentEnumeratorFactory2_GetComponents(This,bstrMachineName,lEnumType,bForceRefresh,pEnumerator)	\
    ( (This)->lpVtbl -> GetComponents(This,bstrMachineName,lEnumType,bForceRefresh,pEnumerator) ) 


#define IVsComponentEnumeratorFactory2_GetComponentsOfPath(This,bstrMachineName,lEnumType,bForceRefresh,bstrPath,pEnumerator)	\
    ( (This)->lpVtbl -> GetComponentsOfPath(This,bstrMachineName,lEnumType,bForceRefresh,bstrPath,pEnumerator) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentEnumeratorFactory2_INTERFACE_DEFINED__ */


#ifndef __IVsProvideComponentEnumeration_INTERFACE_DEFINED__
#define __IVsProvideComponentEnumeration_INTERFACE_DEFINED__

/* interface IVsProvideComponentEnumeration */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsProvideComponentEnumeration;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4df7bba0-660f-11d3-85fd-00c04f6123b3")
    IVsProvideComponentEnumeration : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WriteXMLToFile( 
            /* [in] */ __RPC__in BSTR bstrDesiredFile,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrActualFile) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsProvideComponentEnumerationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsProvideComponentEnumeration * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsProvideComponentEnumeration * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsProvideComponentEnumeration * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVsProvideComponentEnumeration * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVsProvideComponentEnumeration * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVsProvideComponentEnumeration * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsProvideComponentEnumeration * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *WriteXMLToFile )( 
            IVsProvideComponentEnumeration * This,
            /* [in] */ __RPC__in BSTR bstrDesiredFile,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrActualFile);
        
        END_INTERFACE
    } IVsProvideComponentEnumerationVtbl;

    interface IVsProvideComponentEnumeration
    {
        CONST_VTBL struct IVsProvideComponentEnumerationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsProvideComponentEnumeration_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsProvideComponentEnumeration_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsProvideComponentEnumeration_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsProvideComponentEnumeration_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVsProvideComponentEnumeration_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVsProvideComponentEnumeration_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVsProvideComponentEnumeration_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVsProvideComponentEnumeration_WriteXMLToFile(This,bstrDesiredFile,pbstrActualFile)	\
    ( (This)->lpVtbl -> WriteXMLToFile(This,bstrDesiredFile,pbstrActualFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsProvideComponentEnumeration_INTERFACE_DEFINED__ */



#ifndef __CompServicesLib_LIBRARY_DEFINED__
#define __CompServicesLib_LIBRARY_DEFINED__

/* library CompServicesLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_CompServicesLib;

EXTERN_C const CLSID CLSID_CCompServicesPackage;

#ifdef __cplusplus

class DECLSPEC_UUID("588205e0-66e0-11d3-8600-00c04f6123b3")
CCompServicesPackage;
#endif

EXTERN_C const CLSID CLSID_CCom2Enumerator;

#ifdef __cplusplus

class DECLSPEC_UUID("3129723e-660f-11d3-85fd-00c04f6123b3")
CCom2Enumerator;
#endif

EXTERN_C const CLSID CLSID_CComPlusEnumerator;

#ifdef __cplusplus

class DECLSPEC_UUID("f5bd4a64-67a2-11d3-8600-00c04f6123b3")
CComPlusEnumerator;
#endif
#endif /* __CompServicesLib_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_compsvcspkg_0000_0007 */
/* [local] */ 

const IID SID_SCompEnumService = { 0x33a24090, 0x6565, 0x11d3, 0x85, 0xfc, 0x00, 0xc0, 0x4f, 0x61, 0x23, 0xb3 }; 


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg_0000_0007_v0_0_s_ifspec;

#ifndef __IVsTypeLibraryWrapperCallback_INTERFACE_DEFINED__
#define __IVsTypeLibraryWrapperCallback_INTERFACE_DEFINED__

/* interface IVsTypeLibraryWrapperCallback */
/* [object][restricted][hidden][uuid] */ 


EXTERN_C const IID IID_IVsTypeLibraryWrapperCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AFF2B95E-229B-4A14-A422-E99452AD2F8C")
    IVsTypeLibraryWrapperCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAssembly( 
            /* [in] */ __RPC__in LPCOLESTR wszFusionName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComClassic( 
            /* [in] */ __RPC__in TLIBATTR *pTypeLibAttr,
            /* [in] */ __RPC__in LPCOLESTR wszWrapperTool,
            /* [out] */ __RPC__out BOOL *pDelaySigned,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWrapperTool,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetComClassicByTypeLibName( 
            /* [in] */ __RPC__in LPCOLESTR wszTypeLibName,
            /* [out] */ __RPC__out TLIBATTR *pTypeLibAttr,
            /* [out] */ __RPC__out BOOL *pDelaySigned,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWrapperTool,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTypeLibraryWrapperCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTypeLibraryWrapperCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTypeLibraryWrapperCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTypeLibraryWrapperCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAssembly )( 
            IVsTypeLibraryWrapperCallback * This,
            /* [in] */ __RPC__in LPCOLESTR wszFusionName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        HRESULT ( STDMETHODCALLTYPE *GetComClassic )( 
            IVsTypeLibraryWrapperCallback * This,
            /* [in] */ __RPC__in TLIBATTR *pTypeLibAttr,
            /* [in] */ __RPC__in LPCOLESTR wszWrapperTool,
            /* [out] */ __RPC__out BOOL *pDelaySigned,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWrapperTool,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        HRESULT ( STDMETHODCALLTYPE *GetComClassicByTypeLibName )( 
            IVsTypeLibraryWrapperCallback * This,
            /* [in] */ __RPC__in LPCOLESTR wszTypeLibName,
            /* [out] */ __RPC__out TLIBATTR *pTypeLibAttr,
            /* [out] */ __RPC__out BOOL *pDelaySigned,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrWrapperTool,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrPath);
        
        END_INTERFACE
    } IVsTypeLibraryWrapperCallbackVtbl;

    interface IVsTypeLibraryWrapperCallback
    {
        CONST_VTBL struct IVsTypeLibraryWrapperCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTypeLibraryWrapperCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTypeLibraryWrapperCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTypeLibraryWrapperCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTypeLibraryWrapperCallback_GetAssembly(This,wszFusionName,pbstrPath)	\
    ( (This)->lpVtbl -> GetAssembly(This,wszFusionName,pbstrPath) ) 

#define IVsTypeLibraryWrapperCallback_GetComClassic(This,pTypeLibAttr,wszWrapperTool,pDelaySigned,pbstrWrapperTool,pbstrPath)	\
    ( (This)->lpVtbl -> GetComClassic(This,pTypeLibAttr,wszWrapperTool,pDelaySigned,pbstrWrapperTool,pbstrPath) ) 

#define IVsTypeLibraryWrapperCallback_GetComClassicByTypeLibName(This,wszTypeLibName,pTypeLibAttr,pDelaySigned,pbstrWrapperTool,pbstrPath)	\
    ( (This)->lpVtbl -> GetComClassicByTypeLibName(This,wszTypeLibName,pTypeLibAttr,pDelaySigned,pbstrWrapperTool,pbstrPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTypeLibraryWrapperCallback_INTERFACE_DEFINED__ */


#ifndef __IVsTypeLibraryWrapper_INTERFACE_DEFINED__
#define __IVsTypeLibraryWrapper_INTERFACE_DEFINED__

/* interface IVsTypeLibraryWrapper */
/* [object][restricted][hidden][uuid] */ 


EXTERN_C const IID IID_IVsTypeLibraryWrapper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E6065B70-C9B6-4636-80F5-1CF92D7ECE5B")
    IVsTypeLibraryWrapper : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE WrapTypeLibrary( 
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [in] */ __RPC__in LPCOLESTR wszDestinationDirectory,
            /* [in] */ __RPC__in LPCOLESTR wszKeyFile,
            /* [in] */ __RPC__in LPCOLESTR wszKeyContainer,
            /* [in] */ BOOL bDelaySign,
            /* [in] */ __RPC__in_opt IVsTypeLibraryWrapperCallback *pCallback,
            /* [out] */ __RPC__deref_out_opt BSTR **rgbstrWrapperPaths,
            /* [out] */ __RPC__deref_out_opt TLIBATTR **rgWrappedTypeLibs,
            /* [out] */ __RPC__deref_out_opt BOOL **rgbGenerated,
            /* [out] */ __RPC__deref_out_opt BSTR **rgbstrWrapperTools,
            /* [out] */ __RPC__out ULONG *pcWrappedTypeLibs) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMainWrapperFilename( 
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NeedsRegeneration( 
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [in] */ __RPC__in LPCOLESTR wszKeyFile,
            /* [in] */ __RPC__in LPCOLESTR wszKeyContainerName,
            /* [in] */ BOOL bDelaySign,
            /* [in] */ BOOL bCurrentlyDelaySigned,
            /* [in] */ __RPC__in LPCOLESTR wszExistingWrapperFilename,
            /* [retval][out] */ __RPC__out BOOL *pbNeedsRegeneration) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMainWrapperFriendlyName( 
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTypeLibraryWrapperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTypeLibraryWrapper * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTypeLibraryWrapper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTypeLibraryWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *WrapTypeLibrary )( 
            IVsTypeLibraryWrapper * This,
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [in] */ __RPC__in LPCOLESTR wszDestinationDirectory,
            /* [in] */ __RPC__in LPCOLESTR wszKeyFile,
            /* [in] */ __RPC__in LPCOLESTR wszKeyContainer,
            /* [in] */ BOOL bDelaySign,
            /* [in] */ __RPC__in_opt IVsTypeLibraryWrapperCallback *pCallback,
            /* [out] */ __RPC__deref_out_opt BSTR **rgbstrWrapperPaths,
            /* [out] */ __RPC__deref_out_opt TLIBATTR **rgWrappedTypeLibs,
            /* [out] */ __RPC__deref_out_opt BOOL **rgbGenerated,
            /* [out] */ __RPC__deref_out_opt BSTR **rgbstrWrapperTools,
            /* [out] */ __RPC__out ULONG *pcWrappedTypeLibs);
        
        HRESULT ( STDMETHODCALLTYPE *GetMainWrapperFilename )( 
            IVsTypeLibraryWrapper * This,
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *NeedsRegeneration )( 
            IVsTypeLibraryWrapper * This,
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [in] */ __RPC__in LPCOLESTR wszKeyFile,
            /* [in] */ __RPC__in LPCOLESTR wszKeyContainerName,
            /* [in] */ BOOL bDelaySign,
            /* [in] */ BOOL bCurrentlyDelaySigned,
            /* [in] */ __RPC__in LPCOLESTR wszExistingWrapperFilename,
            /* [retval][out] */ __RPC__out BOOL *pbNeedsRegeneration);
        
        HRESULT ( STDMETHODCALLTYPE *GetMainWrapperFriendlyName )( 
            IVsTypeLibraryWrapper * This,
            /* [in] */ __RPC__in TLIBATTR *pTypeLibToWrap,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        END_INTERFACE
    } IVsTypeLibraryWrapperVtbl;

    interface IVsTypeLibraryWrapper
    {
        CONST_VTBL struct IVsTypeLibraryWrapperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTypeLibraryWrapper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTypeLibraryWrapper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTypeLibraryWrapper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTypeLibraryWrapper_WrapTypeLibrary(This,pTypeLibToWrap,wszDestinationDirectory,wszKeyFile,wszKeyContainer,bDelaySign,pCallback,rgbstrWrapperPaths,rgWrappedTypeLibs,rgbGenerated,rgbstrWrapperTools,pcWrappedTypeLibs)	\
    ( (This)->lpVtbl -> WrapTypeLibrary(This,pTypeLibToWrap,wszDestinationDirectory,wszKeyFile,wszKeyContainer,bDelaySign,pCallback,rgbstrWrapperPaths,rgWrappedTypeLibs,rgbGenerated,rgbstrWrapperTools,pcWrappedTypeLibs) ) 

#define IVsTypeLibraryWrapper_GetMainWrapperFilename(This,pTypeLibToWrap,pbstrFileName)	\
    ( (This)->lpVtbl -> GetMainWrapperFilename(This,pTypeLibToWrap,pbstrFileName) ) 

#define IVsTypeLibraryWrapper_NeedsRegeneration(This,pTypeLibToWrap,wszKeyFile,wszKeyContainerName,bDelaySign,bCurrentlyDelaySigned,wszExistingWrapperFilename,pbNeedsRegeneration)	\
    ( (This)->lpVtbl -> NeedsRegeneration(This,pTypeLibToWrap,wszKeyFile,wszKeyContainerName,bDelaySign,bCurrentlyDelaySigned,wszExistingWrapperFilename,pbNeedsRegeneration) ) 

#define IVsTypeLibraryWrapper_GetMainWrapperFriendlyName(This,pTypeLibToWrap,pbstrFileName)	\
    ( (This)->lpVtbl -> GetMainWrapperFriendlyName(This,pTypeLibToWrap,pbstrFileName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTypeLibraryWrapper_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


