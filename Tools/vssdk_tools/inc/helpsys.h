

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for helpsys.idl:
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

#ifndef __helpsys_h__
#define __helpsys_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsHelpSystem_FWD_DEFINED__
#define __IVsHelpSystem_FWD_DEFINED__
typedef interface IVsHelpSystem IVsHelpSystem;
#endif 	/* __IVsHelpSystem_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_helpsys_0000_0000 */
/* [local] */ 

#ifndef INT_TYPE
#define INT_TYPE DWORD_PTR
#define LCID_TYPE DWORD
#endif
typedef /* [public][v1_enum] */ 
enum _VsHelpErrors
    {	VSHELP_E_HTMLHELP_UNKNOWN	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1000 ) ,
	VSHELP_E_COLLECTIONDOESNOTEXIST	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1001 ) ,
	VSHELP_E_COLLECTIONNOTREGISTERED	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1002 ) ,
	VSHELP_E_REGISTRATION	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1003 ) ,
	VSHELP_E_PREFERREDCOLLECTION	= ( ( 0x80000000 | ( 4 << 16 )  )  | 0x1004 ) 
    } 	VsHelpErrors;



extern RPC_IF_HANDLE __MIDL_itf_helpsys_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_helpsys_0000_0000_v0_0_s_ifspec;

#ifndef __IVsHelpSystem_INTERFACE_DEFINED__
#define __IVsHelpSystem_INTERFACE_DEFINED__

/* interface IVsHelpSystem */
/* [unique][helpstring][uuid][object] */ 

typedef /* [public][v1_enum] */ 
enum _VHS_COMMANDS
    {	VHS_Default	= 0,
	VHS_NoErrorMessages	= 0x8,
	VHS_UseBrowser	= 0x1,
	VHS_UseHelp	= 0x2,
	VHS_Localize	= 0x4,
	VHS_OutsideCollectionSpace	= 0x10
    } 	VHS_COMMAND;


EXTERN_C const IID IID_IVsHelpSystem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("854d7ac0-bc3d-11d0-b421-00a0c90f9dc4")
    IVsHelpSystem : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE KeywordSearch( 
            /* [in] */ __RPC__in LPCOLESTR pszKeyword,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE ALinkSearch( 
            /* [in] */ __RPC__in LPCOLESTR pszALink,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE KeywordSearchDlg( 
            /* [in] */ __RPC__in LPCOLESTR pszKeyword,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE FullTextSearchDlg( 
            /* [in] */ __RPC__in LPCOLESTR pszQuery,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetCurrentURL( 
            /* [out] */ __RPC__deref_out_opt BSTR *ppszURL) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromURL( 
            /* [in] */ __RPC__in LPCOLESTR pszURL,
            /* [in] */ const DWORD_PTR Command) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE DisplayTopicFromIdentifier( 
            /* [in] */ __RPC__in LPCOLESTR pszFile,
            /* [in] */ const DWORD_PTR Id,
            /* [in] */ const DWORD_PTR Command) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE ActivateHelpSystem( 
            /* [in] */ const DWORD_PTR dwFlags) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Reserved( 
            /* [in] */ VHS_COMMAND command,
            /* [in] */ VsHelpErrors error) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHelpSystemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHelpSystem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHelpSystem * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *KeywordSearch )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszKeyword,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *ALinkSearch )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszALink,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *KeywordSearchDlg )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszKeyword,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *FullTextSearchDlg )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszQuery,
            /* [in] */ const DWORD_PTR dwFlags,
            /* [in] */ DWORD_PTR dwReserved);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *GetCurrentURL )( 
            IVsHelpSystem * This,
            /* [out] */ __RPC__deref_out_opt BSTR *ppszURL);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromURL )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszURL,
            /* [in] */ const DWORD_PTR Command);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *DisplayTopicFromIdentifier )( 
            IVsHelpSystem * This,
            /* [in] */ __RPC__in LPCOLESTR pszFile,
            /* [in] */ const DWORD_PTR Id,
            /* [in] */ const DWORD_PTR Command);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *ActivateHelpSystem )( 
            IVsHelpSystem * This,
            /* [in] */ const DWORD_PTR dwFlags);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *Reserved )( 
            IVsHelpSystem * This,
            /* [in] */ VHS_COMMAND command,
            /* [in] */ VsHelpErrors error);
        
        END_INTERFACE
    } IVsHelpSystemVtbl;

    interface IVsHelpSystem
    {
        CONST_VTBL struct IVsHelpSystemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHelpSystem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHelpSystem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHelpSystem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHelpSystem_KeywordSearch(This,pszKeyword,dwFlags,dwReserved)	\
    ( (This)->lpVtbl -> KeywordSearch(This,pszKeyword,dwFlags,dwReserved) ) 

#define IVsHelpSystem_ALinkSearch(This,pszALink,dwFlags,dwReserved)	\
    ( (This)->lpVtbl -> ALinkSearch(This,pszALink,dwFlags,dwReserved) ) 

#define IVsHelpSystem_KeywordSearchDlg(This,pszKeyword,dwFlags,dwReserved)	\
    ( (This)->lpVtbl -> KeywordSearchDlg(This,pszKeyword,dwFlags,dwReserved) ) 

#define IVsHelpSystem_FullTextSearchDlg(This,pszQuery,dwFlags,dwReserved)	\
    ( (This)->lpVtbl -> FullTextSearchDlg(This,pszQuery,dwFlags,dwReserved) ) 

#define IVsHelpSystem_GetCurrentURL(This,ppszURL)	\
    ( (This)->lpVtbl -> GetCurrentURL(This,ppszURL) ) 

#define IVsHelpSystem_DisplayTopicFromURL(This,pszURL,Command)	\
    ( (This)->lpVtbl -> DisplayTopicFromURL(This,pszURL,Command) ) 

#define IVsHelpSystem_DisplayTopicFromIdentifier(This,pszFile,Id,Command)	\
    ( (This)->lpVtbl -> DisplayTopicFromIdentifier(This,pszFile,Id,Command) ) 

#define IVsHelpSystem_ActivateHelpSystem(This,dwFlags)	\
    ( (This)->lpVtbl -> ActivateHelpSystem(This,dwFlags) ) 

#define IVsHelpSystem_Reserved(This,command,error)	\
    ( (This)->lpVtbl -> Reserved(This,command,error) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHelpSystem_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_helpsys_0000_0001 */
/* [local] */ 

#define SID_SVsHelpService IID_IVsHelpSystem
#define SID_SHelpService IID_IVsHelpSystem


extern RPC_IF_HANDLE __MIDL_itf_helpsys_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_helpsys_0000_0001_v0_0_s_ifspec;

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


