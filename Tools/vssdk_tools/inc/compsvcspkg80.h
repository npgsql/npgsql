

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for compsvcspkg80.idl:
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

#ifndef __compsvcspkg80_h__
#define __compsvcspkg80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsComponentEnumeratorFactory3_FWD_DEFINED__
#define __IVsComponentEnumeratorFactory3_FWD_DEFINED__
typedef interface IVsComponentEnumeratorFactory3 IVsComponentEnumeratorFactory3;
#endif 	/* __IVsComponentEnumeratorFactory3_FWD_DEFINED__ */


#ifndef __IVsSmartOpenScope_FWD_DEFINED__
#define __IVsSmartOpenScope_FWD_DEFINED__
typedef interface IVsSmartOpenScope IVsSmartOpenScope;
#endif 	/* __IVsSmartOpenScope_FWD_DEFINED__ */


#ifndef __SVsSmartOpenScope_FWD_DEFINED__
#define __SVsSmartOpenScope_FWD_DEFINED__
typedef interface SVsSmartOpenScope SVsSmartOpenScope;
#endif 	/* __SVsSmartOpenScope_FWD_DEFINED__ */


#ifndef __IVsAssemblyNameUnification_FWD_DEFINED__
#define __IVsAssemblyNameUnification_FWD_DEFINED__
typedef interface IVsAssemblyNameUnification IVsAssemblyNameUnification;
#endif 	/* __IVsAssemblyNameUnification_FWD_DEFINED__ */


#ifndef __SVsAssemblyNameUnification_FWD_DEFINED__
#define __SVsAssemblyNameUnification_FWD_DEFINED__
typedef interface SVsAssemblyNameUnification SVsAssemblyNameUnification;
#endif 	/* __SVsAssemblyNameUnification_FWD_DEFINED__ */


#ifndef __IVsSQLCLRReferencesUpdateCallback_FWD_DEFINED__
#define __IVsSQLCLRReferencesUpdateCallback_FWD_DEFINED__
typedef interface IVsSQLCLRReferencesUpdateCallback IVsSQLCLRReferencesUpdateCallback;
#endif 	/* __IVsSQLCLRReferencesUpdateCallback_FWD_DEFINED__ */


#ifndef __IVsSQLCLRReferences_FWD_DEFINED__
#define __IVsSQLCLRReferences_FWD_DEFINED__
typedef interface IVsSQLCLRReferences IVsSQLCLRReferences;
#endif 	/* __IVsSQLCLRReferences_FWD_DEFINED__ */


#ifndef __SVsSQLCLRReferences_FWD_DEFINED__
#define __SVsSQLCLRReferences_FWD_DEFINED__
typedef interface SVsSQLCLRReferences SVsSQLCLRReferences;
#endif 	/* __SVsSQLCLRReferences_FWD_DEFINED__ */


#ifndef __IVsEnumCryptoProviderContainers_FWD_DEFINED__
#define __IVsEnumCryptoProviderContainers_FWD_DEFINED__
typedef interface IVsEnumCryptoProviderContainers IVsEnumCryptoProviderContainers;
#endif 	/* __IVsEnumCryptoProviderContainers_FWD_DEFINED__ */


#ifndef __IVsEnumCryptoProviders_FWD_DEFINED__
#define __IVsEnumCryptoProviders_FWD_DEFINED__
typedef interface IVsEnumCryptoProviders IVsEnumCryptoProviders;
#endif 	/* __IVsEnumCryptoProviders_FWD_DEFINED__ */


#ifndef __IVsStrongNameKeys_FWD_DEFINED__
#define __IVsStrongNameKeys_FWD_DEFINED__
typedef interface IVsStrongNameKeys IVsStrongNameKeys;
#endif 	/* __IVsStrongNameKeys_FWD_DEFINED__ */


#ifndef __SVsStrongNameKeys_FWD_DEFINED__
#define __SVsStrongNameKeys_FWD_DEFINED__
typedef interface SVsStrongNameKeys SVsStrongNameKeys;
#endif 	/* __SVsStrongNameKeys_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell.h"
#include "compsvcspkg.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_compsvcspkg80_0000_0000 */
/* [local] */ 


enum __VSCOMPENUMEXFLAGS
    {	VSCOMPENUM_IncludeRuntimeInfo	= 0x1
    } ;
typedef DWORD VSCOMPENUMEXFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0000_v0_0_s_ifspec;

#ifndef __IVsComponentEnumeratorFactory3_INTERFACE_DEFINED__
#define __IVsComponentEnumeratorFactory3_INTERFACE_DEFINED__

/* interface IVsComponentEnumeratorFactory3 */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsComponentEnumeratorFactory3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0C2B582A-DDB2-440c-A777-3D46C3502337")
    IVsComponentEnumeratorFactory3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetComponentsOfPathEx( 
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [in] */ VSCOMPENUMEXFLAGS grfFlags,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsComponentEnumeratorFactory3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsComponentEnumeratorFactory3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsComponentEnumeratorFactory3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsComponentEnumeratorFactory3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetComponentsOfPathEx )( 
            IVsComponentEnumeratorFactory3 * This,
            /* [in] */ __RPC__in BSTR bstrMachineName,
            /* [in] */ LONG lEnumType,
            /* [in] */ BOOL bForceRefresh,
            /* [in] */ VSCOMPENUMEXFLAGS grfFlags,
            /* [in] */ __RPC__in BSTR bstrPath,
            /* [out] */ __RPC__deref_out_opt IEnumComponents **pEnumerator);
        
        END_INTERFACE
    } IVsComponentEnumeratorFactory3Vtbl;

    interface IVsComponentEnumeratorFactory3
    {
        CONST_VTBL struct IVsComponentEnumeratorFactory3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsComponentEnumeratorFactory3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsComponentEnumeratorFactory3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsComponentEnumeratorFactory3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsComponentEnumeratorFactory3_GetComponentsOfPathEx(This,bstrMachineName,lEnumType,bForceRefresh,grfFlags,bstrPath,pEnumerator)	\
    ( (This)->lpVtbl -> GetComponentsOfPathEx(This,bstrMachineName,lEnumType,bForceRefresh,grfFlags,bstrPath,pEnumerator) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsComponentEnumeratorFactory3_INTERFACE_DEFINED__ */


#ifndef __IVsSmartOpenScope_INTERFACE_DEFINED__
#define __IVsSmartOpenScope_INTERFACE_DEFINED__

/* interface IVsSmartOpenScope */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_IVsSmartOpenScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("677D16F2-982E-49e6-ACDB-797991764AD8")
    IVsSmartOpenScope : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenScope( 
            /* [in] */ __RPC__in LPCOLESTR wszScope,
            /* [in] */ DWORD dwOpenFlags,
            /* [in] */ __RPC__in REFIID riid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnk) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSmartOpenScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSmartOpenScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSmartOpenScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSmartOpenScope * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenScope )( 
            IVsSmartOpenScope * This,
            /* [in] */ __RPC__in LPCOLESTR wszScope,
            /* [in] */ DWORD dwOpenFlags,
            /* [in] */ __RPC__in REFIID riid,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppIUnk);
        
        END_INTERFACE
    } IVsSmartOpenScopeVtbl;

    interface IVsSmartOpenScope
    {
        CONST_VTBL struct IVsSmartOpenScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSmartOpenScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSmartOpenScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSmartOpenScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSmartOpenScope_OpenScope(This,wszScope,dwOpenFlags,riid,ppIUnk)	\
    ( (This)->lpVtbl -> OpenScope(This,wszScope,dwOpenFlags,riid,ppIUnk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSmartOpenScope_INTERFACE_DEFINED__ */


#ifndef __SVsSmartOpenScope_INTERFACE_DEFINED__
#define __SVsSmartOpenScope_INTERFACE_DEFINED__

/* interface SVsSmartOpenScope */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_SVsSmartOpenScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("52F0DAB8-99A9-4aea-A037-42581B7009D2")
    SVsSmartOpenScope : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSmartOpenScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSmartOpenScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSmartOpenScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSmartOpenScope * This);
        
        END_INTERFACE
    } SVsSmartOpenScopeVtbl;

    interface SVsSmartOpenScope
    {
        CONST_VTBL struct SVsSmartOpenScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSmartOpenScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSmartOpenScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSmartOpenScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSmartOpenScope_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg80_0000_0003 */
/* [local] */ 

#define SID_SVsSmartOpenScope IID_SVsSmartOpenScope


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0003_v0_0_s_ifspec;

#ifndef __IVsAssemblyNameUnification_INTERFACE_DEFINED__
#define __IVsAssemblyNameUnification_INTERFACE_DEFINED__

/* interface IVsAssemblyNameUnification */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_IVsAssemblyNameUnification;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F33D7D20-49CE-47a7-9E3B-DCC9DD19AF00")
    IVsAssemblyNameUnification : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUnifiedAssemblyName( 
            /* [in] */ __RPC__in LPCOLESTR wszFrameworkDirectory,
            /* [in] */ __RPC__in LPCOLESTR wszSimpleAssemblyName,
            /* [in] */ __RPC__in LPCOLESTR wszFullAssemblyName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUnifiedAssemblyName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsAssemblyNameUnificationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsAssemblyNameUnification * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsAssemblyNameUnification * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsAssemblyNameUnification * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnifiedAssemblyName )( 
            IVsAssemblyNameUnification * This,
            /* [in] */ __RPC__in LPCOLESTR wszFrameworkDirectory,
            /* [in] */ __RPC__in LPCOLESTR wszSimpleAssemblyName,
            /* [in] */ __RPC__in LPCOLESTR wszFullAssemblyName,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrUnifiedAssemblyName);
        
        END_INTERFACE
    } IVsAssemblyNameUnificationVtbl;

    interface IVsAssemblyNameUnification
    {
        CONST_VTBL struct IVsAssemblyNameUnificationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsAssemblyNameUnification_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsAssemblyNameUnification_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsAssemblyNameUnification_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsAssemblyNameUnification_GetUnifiedAssemblyName(This,wszFrameworkDirectory,wszSimpleAssemblyName,wszFullAssemblyName,pbstrUnifiedAssemblyName)	\
    ( (This)->lpVtbl -> GetUnifiedAssemblyName(This,wszFrameworkDirectory,wszSimpleAssemblyName,wszFullAssemblyName,pbstrUnifiedAssemblyName) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsAssemblyNameUnification_INTERFACE_DEFINED__ */


#ifndef __SVsAssemblyNameUnification_INTERFACE_DEFINED__
#define __SVsAssemblyNameUnification_INTERFACE_DEFINED__

/* interface SVsAssemblyNameUnification */
/* [object][unique][uuid] */ 


EXTERN_C const IID IID_SVsAssemblyNameUnification;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B54B4062-117D-46fc-BE19-CD20F2867D52")
    SVsAssemblyNameUnification : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsAssemblyNameUnificationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsAssemblyNameUnification * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsAssemblyNameUnification * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsAssemblyNameUnification * This);
        
        END_INTERFACE
    } SVsAssemblyNameUnificationVtbl;

    interface SVsAssemblyNameUnification
    {
        CONST_VTBL struct SVsAssemblyNameUnificationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsAssemblyNameUnification_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsAssemblyNameUnification_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsAssemblyNameUnification_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsAssemblyNameUnification_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg80_0000_0005 */
/* [local] */ 

#define SID_SVsAssemblyNameUnification IID_SVsAssemblyNameUnification

enum _SqlReferenceUpdateFlags
    {	SqlReferenceUpdateFlags_ForceUpdate	= 1
    } ;
typedef DWORD SqlReferenceUpdateFlags;


enum _SqlAddNewReferenceFlags
    {	SqlAddNewReferenceFlags_DownloadSelected	= 1
    } ;
typedef DWORD SqlAddNewReferenceFlags;


enum _SqlReferenceUpdateAction
    {	SqlReferenceUpdateAction_None	= 1,
	SqlReferenceUpdateAction_Delete	= 2,
	SqlReferenceUpdateAction_Update	= 3
    } ;
typedef DWORD SqlReferenceUpdateAction;



extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0005_v0_0_s_ifspec;

#ifndef __IVsSQLCLRReferencesUpdateCallback_INTERFACE_DEFINED__
#define __IVsSQLCLRReferencesUpdateCallback_INTERFACE_DEFINED__

/* interface IVsSQLCLRReferencesUpdateCallback */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsSQLCLRReferencesUpdateCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E6F344E-52C0-4edc-814B-16C6DE4AE9B4")
    IVsSQLCLRReferencesUpdateCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE UpdateResult( 
            /* [in] */ __RPC__in LPCWSTR szAssembly,
            /* [in] */ DWORD dwPermisionLevel,
            /* [in] */ HRESULT hrUpdateResult,
            /* [in] */ __RPC__in_opt IErrorInfo *pErrorInfo,
            /* [in] */ SqlReferenceUpdateAction updateAction) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSQLCLRReferencesUpdateCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSQLCLRReferencesUpdateCallback * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSQLCLRReferencesUpdateCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSQLCLRReferencesUpdateCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateResult )( 
            IVsSQLCLRReferencesUpdateCallback * This,
            /* [in] */ __RPC__in LPCWSTR szAssembly,
            /* [in] */ DWORD dwPermisionLevel,
            /* [in] */ HRESULT hrUpdateResult,
            /* [in] */ __RPC__in_opt IErrorInfo *pErrorInfo,
            /* [in] */ SqlReferenceUpdateAction updateAction);
        
        END_INTERFACE
    } IVsSQLCLRReferencesUpdateCallbackVtbl;

    interface IVsSQLCLRReferencesUpdateCallback
    {
        CONST_VTBL struct IVsSQLCLRReferencesUpdateCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSQLCLRReferencesUpdateCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSQLCLRReferencesUpdateCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSQLCLRReferencesUpdateCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSQLCLRReferencesUpdateCallback_UpdateResult(This,szAssembly,dwPermisionLevel,hrUpdateResult,pErrorInfo,updateAction)	\
    ( (This)->lpVtbl -> UpdateResult(This,szAssembly,dwPermisionLevel,hrUpdateResult,pErrorInfo,updateAction) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSQLCLRReferencesUpdateCallback_INTERFACE_DEFINED__ */


#ifndef __IVsSQLCLRReferences_INTERFACE_DEFINED__
#define __IVsSQLCLRReferences_INTERFACE_DEFINED__

/* interface IVsSQLCLRReferences */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsSQLCLRReferences;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7E6F344D-52C0-4edc-814B-16C6DE4AE9B4")
    IVsSQLCLRReferences : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InvokeNewReferencesDlg( 
            /* [in] */ __RPC__in_opt IUnknown *pConnection,
            /* [in] */ __RPC__in_opt IUnknown *pAssemblySupport,
            /* [in] */ SqlAddNewReferenceFlags dwAddNewReferenceFlags,
            /* [in] */ __RPC__in LPCWSTR szLocalCache,
            /* [in] */ DWORD dwProjectPermisionLevel,
            /* [in] */ __RPC__in_opt IVsComponentUser *pComponentUserCallback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateReferences( 
            /* [in] */ __RPC__in_opt IUnknown *pConnection,
            /* [in] */ __RPC__in_opt IUnknown *pAssemblySupport,
            /* [in] */ SqlReferenceUpdateFlags dwReferenceUpdateFlags,
            /* [in] */ ULONG cAssemblyCount,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssemblyCount) LPCWSTR rgszAssemblies[  ],
            /* [in] */ __RPC__in LPCWSTR szLocalCache,
            /* [in] */ DWORD dwProjectPermisionLevel,
            /* [in] */ __RPC__in_opt IVsSQLCLRReferencesUpdateCallback *pCallBack) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSQLCLRReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSQLCLRReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSQLCLRReferences * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSQLCLRReferences * This);
        
        HRESULT ( STDMETHODCALLTYPE *InvokeNewReferencesDlg )( 
            IVsSQLCLRReferences * This,
            /* [in] */ __RPC__in_opt IUnknown *pConnection,
            /* [in] */ __RPC__in_opt IUnknown *pAssemblySupport,
            /* [in] */ SqlAddNewReferenceFlags dwAddNewReferenceFlags,
            /* [in] */ __RPC__in LPCWSTR szLocalCache,
            /* [in] */ DWORD dwProjectPermisionLevel,
            /* [in] */ __RPC__in_opt IVsComponentUser *pComponentUserCallback);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateReferences )( 
            IVsSQLCLRReferences * This,
            /* [in] */ __RPC__in_opt IUnknown *pConnection,
            /* [in] */ __RPC__in_opt IUnknown *pAssemblySupport,
            /* [in] */ SqlReferenceUpdateFlags dwReferenceUpdateFlags,
            /* [in] */ ULONG cAssemblyCount,
            /* [size_is][in] */ __RPC__in_ecount_full(cAssemblyCount) LPCWSTR rgszAssemblies[  ],
            /* [in] */ __RPC__in LPCWSTR szLocalCache,
            /* [in] */ DWORD dwProjectPermisionLevel,
            /* [in] */ __RPC__in_opt IVsSQLCLRReferencesUpdateCallback *pCallBack);
        
        END_INTERFACE
    } IVsSQLCLRReferencesVtbl;

    interface IVsSQLCLRReferences
    {
        CONST_VTBL struct IVsSQLCLRReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSQLCLRReferences_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSQLCLRReferences_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSQLCLRReferences_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSQLCLRReferences_InvokeNewReferencesDlg(This,pConnection,pAssemblySupport,dwAddNewReferenceFlags,szLocalCache,dwProjectPermisionLevel,pComponentUserCallback)	\
    ( (This)->lpVtbl -> InvokeNewReferencesDlg(This,pConnection,pAssemblySupport,dwAddNewReferenceFlags,szLocalCache,dwProjectPermisionLevel,pComponentUserCallback) ) 

#define IVsSQLCLRReferences_UpdateReferences(This,pConnection,pAssemblySupport,dwReferenceUpdateFlags,cAssemblyCount,rgszAssemblies,szLocalCache,dwProjectPermisionLevel,pCallBack)	\
    ( (This)->lpVtbl -> UpdateReferences(This,pConnection,pAssemblySupport,dwReferenceUpdateFlags,cAssemblyCount,rgszAssemblies,szLocalCache,dwProjectPermisionLevel,pCallBack) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSQLCLRReferences_INTERFACE_DEFINED__ */


#ifndef __SVsSQLCLRReferences_INTERFACE_DEFINED__
#define __SVsSQLCLRReferences_INTERFACE_DEFINED__

/* interface SVsSQLCLRReferences */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSQLCLRReferences;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8ED9966C-102F-45b0-8293-BB9F2463A6F2")
    SVsSQLCLRReferences : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSQLCLRReferencesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSQLCLRReferences * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSQLCLRReferences * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSQLCLRReferences * This);
        
        END_INTERFACE
    } SVsSQLCLRReferencesVtbl;

    interface SVsSQLCLRReferences
    {
        CONST_VTBL struct SVsSQLCLRReferencesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSQLCLRReferences_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSQLCLRReferences_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSQLCLRReferences_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSQLCLRReferences_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg80_0000_0008 */
/* [local] */ 

#define SID_SVsSQLCLRReferences IID_SVsSQLCLRReferences


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0008_v0_0_s_ifspec;

#ifndef __IVsEnumCryptoProviderContainers_INTERFACE_DEFINED__
#define __IVsEnumCryptoProviderContainers_INTERFACE_DEFINED__

/* interface IVsEnumCryptoProviderContainers */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumCryptoProviderContainers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("af855397-c4dc-478b-abd4-c3dbb3759e72")
    IVsEnumCryptoProviderContainers : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *pbstrContainers,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumCryptoProviderContainersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumCryptoProviderContainers * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumCryptoProviderContainers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumCryptoProviderContainers * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumCryptoProviderContainers * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *pbstrContainers,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumCryptoProviderContainers * This);
        
        END_INTERFACE
    } IVsEnumCryptoProviderContainersVtbl;

    interface IVsEnumCryptoProviderContainers
    {
        CONST_VTBL struct IVsEnumCryptoProviderContainersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumCryptoProviderContainers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumCryptoProviderContainers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumCryptoProviderContainers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumCryptoProviderContainers_Next(This,celt,pbstrContainers,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,pbstrContainers,pceltFetched) ) 

#define IVsEnumCryptoProviderContainers_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumCryptoProviderContainers_INTERFACE_DEFINED__ */


#ifndef __IVsEnumCryptoProviders_INTERFACE_DEFINED__
#define __IVsEnumCryptoProviders_INTERFACE_DEFINED__

/* interface IVsEnumCryptoProviders */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsEnumCryptoProviders;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f7fc33a9-10da-42be-9f88-9700e583ec33")
    IVsEnumCryptoProviders : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *pbstrProviders,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsEnumCryptoProvidersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsEnumCryptoProviders * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsEnumCryptoProviders * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsEnumCryptoProviders * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsEnumCryptoProviders * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *pbstrProviders,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsEnumCryptoProviders * This);
        
        END_INTERFACE
    } IVsEnumCryptoProvidersVtbl;

    interface IVsEnumCryptoProviders
    {
        CONST_VTBL struct IVsEnumCryptoProvidersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsEnumCryptoProviders_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsEnumCryptoProviders_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsEnumCryptoProviders_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsEnumCryptoProviders_Next(This,celt,pbstrProviders,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,pbstrProviders,pceltFetched) ) 

#define IVsEnumCryptoProviders_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsEnumCryptoProviders_INTERFACE_DEFINED__ */


#ifndef __IVsStrongNameKeys_INTERFACE_DEFINED__
#define __IVsStrongNameKeys_INTERFACE_DEFINED__

/* interface IVsStrongNameKeys */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsStrongNameKeys;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b0de8148-dabc-4ab9-b418-413e63df6e6c")
    IVsStrongNameKeys : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumProviders( 
            /* [out] */ __RPC__deref_out_opt IVsEnumCryptoProviders **ppEnumProviders) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumContainers( 
            /* [in] */ __RPC__in LPCOLESTR szProvider,
            /* [out] */ __RPC__deref_out_opt IVsEnumCryptoProviderContainers **ppEnumContainers) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNewKey( 
            /* [in] */ __RPC__in LPCOLESTR szFileLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNewKeyNoUI( 
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNewKeyWithName( 
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword,
            /* [in] */ __RPC__in LPCOLESTR szSubjectName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ImportKeyFromPFX( 
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrContainerName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ChangePassword( 
            /* [in] */ __RPC__in LPCOLESTR szPfxFile,
            /* [in] */ __RPC__in LPCOLESTR szOldPassword,
            /* [in] */ __RPC__in LPCOLESTR szNewPassword) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsStrongNameKeysVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsStrongNameKeys * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsStrongNameKeys * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProviders )( 
            IVsStrongNameKeys * This,
            /* [out] */ __RPC__deref_out_opt IVsEnumCryptoProviders **ppEnumProviders);
        
        HRESULT ( STDMETHODCALLTYPE *EnumContainers )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szProvider,
            /* [out] */ __RPC__deref_out_opt IVsEnumCryptoProviderContainers **ppEnumContainers);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewKey )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szFileLocation,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewKeyNoUI )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewKeyWithName )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [in] */ __RPC__in LPCOLESTR szPassword,
            /* [in] */ __RPC__in LPCOLESTR szSubjectName);
        
        HRESULT ( STDMETHODCALLTYPE *ImportKeyFromPFX )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szFile,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrContainerName);
        
        HRESULT ( STDMETHODCALLTYPE *ChangePassword )( 
            IVsStrongNameKeys * This,
            /* [in] */ __RPC__in LPCOLESTR szPfxFile,
            /* [in] */ __RPC__in LPCOLESTR szOldPassword,
            /* [in] */ __RPC__in LPCOLESTR szNewPassword);
        
        END_INTERFACE
    } IVsStrongNameKeysVtbl;

    interface IVsStrongNameKeys
    {
        CONST_VTBL struct IVsStrongNameKeysVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsStrongNameKeys_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsStrongNameKeys_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsStrongNameKeys_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsStrongNameKeys_EnumProviders(This,ppEnumProviders)	\
    ( (This)->lpVtbl -> EnumProviders(This,ppEnumProviders) ) 

#define IVsStrongNameKeys_EnumContainers(This,szProvider,ppEnumContainers)	\
    ( (This)->lpVtbl -> EnumContainers(This,szProvider,ppEnumContainers) ) 

#define IVsStrongNameKeys_CreateNewKey(This,szFileLocation,pbstrFileName)	\
    ( (This)->lpVtbl -> CreateNewKey(This,szFileLocation,pbstrFileName) ) 

#define IVsStrongNameKeys_CreateNewKeyNoUI(This,szFile,szPassword)	\
    ( (This)->lpVtbl -> CreateNewKeyNoUI(This,szFile,szPassword) ) 

#define IVsStrongNameKeys_CreateNewKeyWithName(This,szFile,szPassword,szSubjectName)	\
    ( (This)->lpVtbl -> CreateNewKeyWithName(This,szFile,szPassword,szSubjectName) ) 

#define IVsStrongNameKeys_ImportKeyFromPFX(This,szFile,pbstrContainerName)	\
    ( (This)->lpVtbl -> ImportKeyFromPFX(This,szFile,pbstrContainerName) ) 

#define IVsStrongNameKeys_ChangePassword(This,szPfxFile,szOldPassword,szNewPassword)	\
    ( (This)->lpVtbl -> ChangePassword(This,szPfxFile,szOldPassword,szNewPassword) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsStrongNameKeys_INTERFACE_DEFINED__ */


#ifndef __SVsStrongNameKeys_INTERFACE_DEFINED__
#define __SVsStrongNameKeys_INTERFACE_DEFINED__

/* interface SVsStrongNameKeys */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsStrongNameKeys;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("97BAE668-2CF2-4ed8-A019-D279E3D74C80")
    SVsStrongNameKeys : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsStrongNameKeysVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsStrongNameKeys * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsStrongNameKeys * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsStrongNameKeys * This);
        
        END_INTERFACE
    } SVsStrongNameKeysVtbl;

    interface SVsStrongNameKeys
    {
        CONST_VTBL struct SVsStrongNameKeysVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsStrongNameKeys_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsStrongNameKeys_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsStrongNameKeys_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsStrongNameKeys_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg80_0000_0012 */
/* [local] */ 

#define SID_SVsStrongNameKeys IID_SVsStrongNameKeys


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0012_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg80_0000_0012_v0_0_s_ifspec;

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


