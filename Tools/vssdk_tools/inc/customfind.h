

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for customfind.idl:
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

#ifndef __customfind_h__
#define __customfind_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsRegisterCustomFindScope_FWD_DEFINED__
#define __IVsRegisterCustomFindScope_FWD_DEFINED__
typedef interface IVsRegisterCustomFindScope IVsRegisterCustomFindScope;
#endif 	/* __IVsRegisterCustomFindScope_FWD_DEFINED__ */


#ifndef __IVsCustomFindScope_FWD_DEFINED__
#define __IVsCustomFindScope_FWD_DEFINED__
typedef interface IVsCustomFindScope IVsCustomFindScope;
#endif 	/* __IVsCustomFindScope_FWD_DEFINED__ */


#ifndef __IVsCustomFindScopeLookIn_FWD_DEFINED__
#define __IVsCustomFindScopeLookIn_FWD_DEFINED__
typedef interface IVsCustomFindScopeLookIn IVsCustomFindScopeLookIn;
#endif 	/* __IVsCustomFindScopeLookIn_FWD_DEFINED__ */


#ifndef __IVsCustomFindScopeSearch_FWD_DEFINED__
#define __IVsCustomFindScopeSearch_FWD_DEFINED__
typedef interface IVsCustomFindScopeSearch IVsCustomFindScopeSearch;
#endif 	/* __IVsCustomFindScopeSearch_FWD_DEFINED__ */


#ifndef __IVsCustomFindScopeNotify_FWD_DEFINED__
#define __IVsCustomFindScopeNotify_FWD_DEFINED__
typedef interface IVsCustomFindScopeNotify IVsCustomFindScopeNotify;
#endif 	/* __IVsCustomFindScopeNotify_FWD_DEFINED__ */


/* header files for imported files */
#include "textfind.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_customfind_0000_0000 */
/* [local] */ 







enum __VSCUSTOMFINDSTATUS
    {	CFR_IDLE	= 0,
	CFR_RUNNING	= ( CFR_IDLE + 1 ) ,
	CFR_COMPLETE	= ( CFR_RUNNING + 1 ) ,
	CFR_CANCEL	= ( CFR_COMPLETE + 1 ) ,
	CFR_ERROR	= ( CFR_CANCEL + 1 ) 
    } ;
typedef DWORD VSCUSTOMFINDSTATUS;

typedef struct _VSBROWSESCOPE
    {
    BSTR bstrDisplay;
    BSTR bstrTooltip;
    VARIANT varCanonicalScope;
    } 	VSBROWSESCOPEW;

typedef struct _VSBROWSESCOPE *PVSBROWSESCOPEW;



extern RPC_IF_HANDLE __MIDL_itf_customfind_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_customfind_0000_0000_v0_0_s_ifspec;

#ifndef __IVsRegisterCustomFindScope_INTERFACE_DEFINED__
#define __IVsRegisterCustomFindScope_INTERFACE_DEFINED__

/* interface IVsRegisterCustomFindScope */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterCustomFindScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("72F08BD4-412B-4f54-8B7C-02997C483981")
    IVsRegisterCustomFindScope : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterCustomFindScope( 
            /* [in] */ __RPC__in REFGUID guidSID,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSFINDOPTIONS grfEnabledOptions,
            /* [out] */ __RPC__out DWORD_PTR *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnRegisterCustomFindScope( 
            /* [in] */ DWORD_PTR dwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterCustomFindScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterCustomFindScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterCustomFindScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterCustomFindScope * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterCustomFindScope )( 
            IVsRegisterCustomFindScope * This,
            /* [in] */ __RPC__in REFGUID guidSID,
            /* [in] */ __RPC__in LPCOLESTR pszName,
            /* [in] */ VSFINDOPTIONS grfEnabledOptions,
            /* [out] */ __RPC__out DWORD_PTR *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnRegisterCustomFindScope )( 
            IVsRegisterCustomFindScope * This,
            /* [in] */ DWORD_PTR dwCookie);
        
        END_INTERFACE
    } IVsRegisterCustomFindScopeVtbl;

    interface IVsRegisterCustomFindScope
    {
        CONST_VTBL struct IVsRegisterCustomFindScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterCustomFindScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterCustomFindScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterCustomFindScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterCustomFindScope_RegisterCustomFindScope(This,guidSID,pszName,grfEnabledOptions,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterCustomFindScope(This,guidSID,pszName,grfEnabledOptions,pdwCookie) ) 

#define IVsRegisterCustomFindScope_UnRegisterCustomFindScope(This,dwCookie)	\
    ( (This)->lpVtbl -> UnRegisterCustomFindScope(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterCustomFindScope_INTERFACE_DEFINED__ */


#ifndef __IVsCustomFindScope_INTERFACE_DEFINED__
#define __IVsCustomFindScope_INTERFACE_DEFINED__

/* interface IVsCustomFindScope */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCustomFindScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51066B51-499A-419b-A7B3-18C1DB46EB95")
    IVsCustomFindScope : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCustomFindScopeLookIn( 
            /* [retval][out] */ __RPC__deref_out_opt IVsCustomFindScopeLookIn **ppFindLookIn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomFindScopeSearch( 
            /* [retval][out] */ __RPC__deref_out_opt IVsCustomFindScopeSearch **ppBatchFind) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCustomFindScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomFindScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomFindScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomFindScope * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomFindScopeLookIn )( 
            IVsCustomFindScope * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsCustomFindScopeLookIn **ppFindLookIn);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomFindScopeSearch )( 
            IVsCustomFindScope * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsCustomFindScopeSearch **ppBatchFind);
        
        END_INTERFACE
    } IVsCustomFindScopeVtbl;

    interface IVsCustomFindScope
    {
        CONST_VTBL struct IVsCustomFindScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomFindScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomFindScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomFindScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomFindScope_GetCustomFindScopeLookIn(This,ppFindLookIn)	\
    ( (This)->lpVtbl -> GetCustomFindScopeLookIn(This,ppFindLookIn) ) 

#define IVsCustomFindScope_GetCustomFindScopeSearch(This,ppBatchFind)	\
    ( (This)->lpVtbl -> GetCustomFindScopeSearch(This,ppBatchFind) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomFindScope_INTERFACE_DEFINED__ */


#ifndef __IVsCustomFindScopeLookIn_INTERFACE_DEFINED__
#define __IVsCustomFindScopeLookIn_INTERFACE_DEFINED__

/* interface IVsCustomFindScopeLookIn */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCustomFindScopeLookIn;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B82E262A-A6A4-407f-B562-42785C2C61FC")
    IVsCustomFindScopeLookIn : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Browse( 
            /* [out][in] */ __RPC__inout PVSBROWSESCOPEW pBrowseScope) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCustomFindScopeLookInVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomFindScopeLookIn * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomFindScopeLookIn * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomFindScopeLookIn * This);
        
        HRESULT ( STDMETHODCALLTYPE *Browse )( 
            IVsCustomFindScopeLookIn * This,
            /* [out][in] */ __RPC__inout PVSBROWSESCOPEW pBrowseScope);
        
        END_INTERFACE
    } IVsCustomFindScopeLookInVtbl;

    interface IVsCustomFindScopeLookIn
    {
        CONST_VTBL struct IVsCustomFindScopeLookInVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomFindScopeLookIn_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomFindScopeLookIn_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomFindScopeLookIn_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomFindScopeLookIn_Browse(This,pBrowseScope)	\
    ( (This)->lpVtbl -> Browse(This,pBrowseScope) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomFindScopeLookIn_INTERFACE_DEFINED__ */


#ifndef __IVsCustomFindScopeSearch_INTERFACE_DEFINED__
#define __IVsCustomFindScopeSearch_INTERFACE_DEFINED__

/* interface IVsCustomFindScopeSearch */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCustomFindScopeSearch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BC70E6DE-4B40-438f-8F83-AFE1CC6AB101")
    IVsCustomFindScopeSearch : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Find( 
            /* [in] */ VSBROWSESCOPEW VsBrowseScope,
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszFilter,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsCustomFindScopeNotify *pBatchFindNotify) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace( 
            /* [in] */ VSBROWSESCOPEW VsBrowseScope,
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ __RPC__in LPCOLESTR pszFilter,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsCustomFindScopeNotify *pBatchFindNotify) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStatus( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrStatus,
            /* [retval][out] */ __RPC__out VSCUSTOMFINDSTATUS *pdwStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Cancel( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCustomFindScopeSearchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomFindScopeSearch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomFindScopeSearch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomFindScopeSearch * This);
        
        HRESULT ( STDMETHODCALLTYPE *Find )( 
            IVsCustomFindScopeSearch * This,
            /* [in] */ VSBROWSESCOPEW VsBrowseScope,
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszFilter,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsCustomFindScopeNotify *pBatchFindNotify);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVsCustomFindScopeSearch * This,
            /* [in] */ VSBROWSESCOPEW VsBrowseScope,
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ __RPC__in LPCOLESTR pszFilter,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in_opt IVsCustomFindScopeNotify *pBatchFindNotify);
        
        HRESULT ( STDMETHODCALLTYPE *GetStatus )( 
            IVsCustomFindScopeSearch * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrStatus,
            /* [retval][out] */ __RPC__out VSCUSTOMFINDSTATUS *pdwStatus);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVsCustomFindScopeSearch * This);
        
        END_INTERFACE
    } IVsCustomFindScopeSearchVtbl;

    interface IVsCustomFindScopeSearch
    {
        CONST_VTBL struct IVsCustomFindScopeSearchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomFindScopeSearch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomFindScopeSearch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomFindScopeSearch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomFindScopeSearch_Find(This,VsBrowseScope,pszFind,pszFilter,grfOptions,pBatchFindNotify)	\
    ( (This)->lpVtbl -> Find(This,VsBrowseScope,pszFind,pszFilter,grfOptions,pBatchFindNotify) ) 

#define IVsCustomFindScopeSearch_Replace(This,VsBrowseScope,pszFind,pszReplace,pszFilter,grfOptions,pBatchFindNotify)	\
    ( (This)->lpVtbl -> Replace(This,VsBrowseScope,pszFind,pszReplace,pszFilter,grfOptions,pBatchFindNotify) ) 

#define IVsCustomFindScopeSearch_GetStatus(This,pbstrStatus,pdwStatus)	\
    ( (This)->lpVtbl -> GetStatus(This,pbstrStatus,pdwStatus) ) 

#define IVsCustomFindScopeSearch_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomFindScopeSearch_INTERFACE_DEFINED__ */


#ifndef __IVsCustomFindScopeNotify_INTERFACE_DEFINED__
#define __IVsCustomFindScopeNotify_INTERFACE_DEFINED__

/* interface IVsCustomFindScopeNotify */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsCustomFindScopeNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("169B2F24-BB17-40d8-AE2A-13B651B610CA")
    IVsCustomFindScopeNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Notify( 
            /* [in] */ VSCUSTOMFINDSTATUS grfStatus) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsCustomFindScopeNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsCustomFindScopeNotify * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsCustomFindScopeNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsCustomFindScopeNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *Notify )( 
            IVsCustomFindScopeNotify * This,
            /* [in] */ VSCUSTOMFINDSTATUS grfStatus);
        
        END_INTERFACE
    } IVsCustomFindScopeNotifyVtbl;

    interface IVsCustomFindScopeNotify
    {
        CONST_VTBL struct IVsCustomFindScopeNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsCustomFindScopeNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsCustomFindScopeNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsCustomFindScopeNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsCustomFindScopeNotify_Notify(This,grfStatus)	\
    ( (This)->lpVtbl -> Notify(This,grfStatus) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsCustomFindScopeNotify_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


