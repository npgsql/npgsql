

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for uilocale.idl:
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

#ifndef __uilocale_h__
#define __uilocale_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IUIHostLocale_FWD_DEFINED__
#define __IUIHostLocale_FWD_DEFINED__
typedef interface IUIHostLocale IUIHostLocale;
#endif 	/* __IUIHostLocale_FWD_DEFINED__ */


#ifndef __IUIHostLocale2_FWD_DEFINED__
#define __IUIHostLocale2_FWD_DEFINED__
typedef interface IUIHostLocale2 IUIHostLocale2;
#endif 	/* __IUIHostLocale2_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_uilocale_0000_0000 */
/* [local] */ 

#if 0
typedef struct _LOGFONTW
    {
    LONG lfHeight;
    LONG lfWidth;
    LONG lfEscapement;
    LONG lfOrientation;
    LONG lfWeight;
    BYTE lfItalic;
    BYTE lfUnderline;
    BYTE lfStrikeOut;
    BYTE lfCharSet;
    BYTE lfOutPrecision;
    BYTE lfClipPrecision;
    BYTE lfQuality;
    BYTE lfPitchAndFamily;
    WCHAR lfFaceName[ 32 ];
    } 	UIDLGLOGFONT;

#else
#define UIDLGLOGFONT LOGFONTW
#endif


extern RPC_IF_HANDLE __MIDL_itf_uilocale_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_uilocale_0000_0000_v0_0_s_ifspec;

#ifndef __IUIHostLocale_INTERFACE_DEFINED__
#define __IUIHostLocale_INTERFACE_DEFINED__

/* interface IUIHostLocale */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IUIHostLocale;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C2EA031-02BE-11d1-8C85-00C04FC2AA89")
    IUIHostLocale : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUILocale( 
            /* [retval][out] */ __RPC__out LCID *plcid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDialogFont( 
            /* [out] */ __RPC__out UIDLGLOGFONT *plogfont) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IUIHostLocaleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IUIHostLocale * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IUIHostLocale * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IUIHostLocale * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUILocale )( 
            IUIHostLocale * This,
            /* [retval][out] */ __RPC__out LCID *plcid);
        
        HRESULT ( STDMETHODCALLTYPE *GetDialogFont )( 
            IUIHostLocale * This,
            /* [out] */ __RPC__out UIDLGLOGFONT *plogfont);
        
        END_INTERFACE
    } IUIHostLocaleVtbl;

    interface IUIHostLocale
    {
        CONST_VTBL struct IUIHostLocaleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IUIHostLocale_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IUIHostLocale_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IUIHostLocale_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IUIHostLocale_GetUILocale(This,plcid)	\
    ( (This)->lpVtbl -> GetUILocale(This,plcid) ) 

#define IUIHostLocale_GetDialogFont(This,plogfont)	\
    ( (This)->lpVtbl -> GetDialogFont(This,plogfont) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IUIHostLocale_INTERFACE_DEFINED__ */


#ifndef __IUIHostLocale2_INTERFACE_DEFINED__
#define __IUIHostLocale2_INTERFACE_DEFINED__

/* interface IUIHostLocale2 */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IUIHostLocale2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C2EA032-02BE-11d1-8C85-00C04FC2AA89")
    IUIHostLocale2 : public IUIHostLocale
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadUILibrary( 
            /* [in] */ __RPC__in LPCOLESTR lpstrPath,
            /* [in] */ __RPC__in LPCOLESTR lpstrDllName,
            /* [in] */ DWORD dwExFlags,
            /* [retval][out] */ __RPC__out DWORD_PTR *phinstOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MungeDialogFont( 
            /* [in] */ DWORD dwSize,
            /* [size_is][in] */ __RPC__in_ecount_full(dwSize) const BYTE *pDlgTemplate,
            /* [out] */ __RPC__deref_out_opt BYTE **ppDlgTemplateOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadDialog( 
            /* [in] */ DWORD_PTR hMod,
            /* [in] */ DWORD dwDlgResId,
            /* [out] */ __RPC__deref_out_opt BYTE **ppDlgTemplate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUILibraryFileName( 
            /* [in] */ __RPC__in LPCOLESTR lpstrPath,
            /* [in] */ __RPC__in LPCOLESTR lpstrDllName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IUIHostLocale2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IUIHostLocale2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IUIHostLocale2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IUIHostLocale2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUILocale )( 
            IUIHostLocale2 * This,
            /* [retval][out] */ __RPC__out LCID *plcid);
        
        HRESULT ( STDMETHODCALLTYPE *GetDialogFont )( 
            IUIHostLocale2 * This,
            /* [out] */ __RPC__out UIDLGLOGFONT *plogfont);
        
        HRESULT ( STDMETHODCALLTYPE *LoadUILibrary )( 
            IUIHostLocale2 * This,
            /* [in] */ __RPC__in LPCOLESTR lpstrPath,
            /* [in] */ __RPC__in LPCOLESTR lpstrDllName,
            /* [in] */ DWORD dwExFlags,
            /* [retval][out] */ __RPC__out DWORD_PTR *phinstOut);
        
        HRESULT ( STDMETHODCALLTYPE *MungeDialogFont )( 
            IUIHostLocale2 * This,
            /* [in] */ DWORD dwSize,
            /* [size_is][in] */ __RPC__in_ecount_full(dwSize) const BYTE *pDlgTemplate,
            /* [out] */ __RPC__deref_out_opt BYTE **ppDlgTemplateOut);
        
        HRESULT ( STDMETHODCALLTYPE *LoadDialog )( 
            IUIHostLocale2 * This,
            /* [in] */ DWORD_PTR hMod,
            /* [in] */ DWORD dwDlgResId,
            /* [out] */ __RPC__deref_out_opt BYTE **ppDlgTemplate);
        
        HRESULT ( STDMETHODCALLTYPE *GetUILibraryFileName )( 
            IUIHostLocale2 * This,
            /* [in] */ __RPC__in LPCOLESTR lpstrPath,
            /* [in] */ __RPC__in LPCOLESTR lpstrDllName,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOut);
        
        END_INTERFACE
    } IUIHostLocale2Vtbl;

    interface IUIHostLocale2
    {
        CONST_VTBL struct IUIHostLocale2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IUIHostLocale2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IUIHostLocale2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IUIHostLocale2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IUIHostLocale2_GetUILocale(This,plcid)	\
    ( (This)->lpVtbl -> GetUILocale(This,plcid) ) 

#define IUIHostLocale2_GetDialogFont(This,plogfont)	\
    ( (This)->lpVtbl -> GetDialogFont(This,plogfont) ) 


#define IUIHostLocale2_LoadUILibrary(This,lpstrPath,lpstrDllName,dwExFlags,phinstOut)	\
    ( (This)->lpVtbl -> LoadUILibrary(This,lpstrPath,lpstrDllName,dwExFlags,phinstOut) ) 

#define IUIHostLocale2_MungeDialogFont(This,dwSize,pDlgTemplate,ppDlgTemplateOut)	\
    ( (This)->lpVtbl -> MungeDialogFont(This,dwSize,pDlgTemplate,ppDlgTemplateOut) ) 

#define IUIHostLocale2_LoadDialog(This,hMod,dwDlgResId,ppDlgTemplate)	\
    ( (This)->lpVtbl -> LoadDialog(This,hMod,dwDlgResId,ppDlgTemplate) ) 

#define IUIHostLocale2_GetUILibraryFileName(This,lpstrPath,lpstrDllName,pbstrOut)	\
    ( (This)->lpVtbl -> GetUILibraryFileName(This,lpstrPath,lpstrDllName,pbstrOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IUIHostLocale2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_uilocale_0000_0002 */
/* [local] */ 

#define SID_SUIHostLocale IID_IUIHostLocale


extern RPC_IF_HANDLE __MIDL_itf_uilocale_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_uilocale_0000_0002_v0_0_s_ifspec;

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


