

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for fpstfmt.idl:
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

#ifndef __fpstfmt_h__
#define __fpstfmt_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IPersistFileFormat_FWD_DEFINED__
#define __IPersistFileFormat_FWD_DEFINED__
typedef interface IPersistFileFormat IPersistFileFormat;
#endif 	/* __IPersistFileFormat_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_fpstfmt_0000_0000 */
/* [local] */ 

//=--------------------------------------------------------------------------=
// fpstfmt.h
//=--------------------------------------------------------------------------=
// (C) Copyright 1997 Microsoft Corporation.  All Rights Reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//=--------------------------------------------------------------------------=
//
// Declarations for Vegas Shell's IPersistFileFormat.
//

////////////////////////////////////////////////////////////////////////////
// CLSID for CLSID_PersistFileFormat  {3AFAE241-B530-11d0-8199-00A0C91BBEE3}
// DEFINE_GUID(CLSID_PersistFileFormat, 
// 0x3afae241, 0xb530, 0x11d0, 0x81, 0x99, 0x0, 0xa0, 0xc9, 0x1b, 0xbe, 0xe3);
 
////////////////////////////////////////////////////////////////////////////
// Interface ID for IPersistFileFormat  {3AFAE242-B530-11d0-8199-00A0C91BBEE3}
// DEFINE_GUID(IID_IPersistFileFormat, 
// 0x3afae242, 0xb530, 0x11d0, 0x81, 0x99, 0x0, 0xa0, 0xc9, 0x1b, 0xbe, 0xe3);
 
#ifndef ___DEF_FORMAT_INDEX_DECLARATION__
#define ___DEF_FORMAT_INDEX_DECLARATION__
#define DEF_FORMAT_INDEX  ((DWORD) 0)    // used when caller does not know a specific format to specify
#endif ___DEF_FORMAT_INDEX_DECLARATION__
typedef 
enum _PFF_RESULTS
    {	STG_E_INVALIDCODEPAGE	= ( ( 0x80000000 | ( 3 << 16 )  )  | 0x300 ) ,
	STG_E_NOTTEXT	= ( ( 0x80000000 | ( 3 << 16 )  )  | 0x302 ) ,
	STG_S_DATALOSS	= ( ( 0 | ( 3 << 16 )  )  | 0x301 ) 
    } 	PFF_RESULTS;



extern RPC_IF_HANDLE __MIDL_itf_fpstfmt_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_fpstfmt_0000_0000_v0_0_s_ifspec;

#ifndef __IPersistFileFormat_INTERFACE_DEFINED__
#define __IPersistFileFormat_INTERFACE_DEFINED__

/* interface IPersistFileFormat */
/* [unique][version][uuid][object] */ 


EXTERN_C const IID IID_IPersistFileFormat;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3AFAE242-B530-11d0-8199-00A0C91BBEE3")
    IPersistFileFormat : public IPersist
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsDirty( 
            /* [out] */ __RPC__out BOOL *pfIsDirty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitNew( 
            /* [in] */ DWORD nFormatIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ DWORD grfMode,
            /* [in] */ BOOL fReadOnly) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ BOOL fRemember,
            /* [in] */ DWORD nFormatIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveCompleted( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurFile( 
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszFilename,
            /* [out] */ __RPC__out DWORD *pnFormatIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFormatList( 
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszFormatList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPersistFileFormatVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPersistFileFormat * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPersistFileFormat * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPersistFileFormat * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetClassID )( 
            IPersistFileFormat * This,
            /* [out] */ __RPC__out CLSID *pClassID);
        
        HRESULT ( STDMETHODCALLTYPE *IsDirty )( 
            IPersistFileFormat * This,
            /* [out] */ __RPC__out BOOL *pfIsDirty);
        
        HRESULT ( STDMETHODCALLTYPE *InitNew )( 
            IPersistFileFormat * This,
            /* [in] */ DWORD nFormatIndex);
        
        HRESULT ( STDMETHODCALLTYPE *Load )( 
            IPersistFileFormat * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ DWORD grfMode,
            /* [in] */ BOOL fReadOnly);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IPersistFileFormat * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ BOOL fRemember,
            /* [in] */ DWORD nFormatIndex);
        
        HRESULT ( STDMETHODCALLTYPE *SaveCompleted )( 
            IPersistFileFormat * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurFile )( 
            IPersistFileFormat * This,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszFilename,
            /* [out] */ __RPC__out DWORD *pnFormatIndex);
        
        HRESULT ( STDMETHODCALLTYPE *GetFormatList )( 
            IPersistFileFormat * This,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszFormatList);
        
        END_INTERFACE
    } IPersistFileFormatVtbl;

    interface IPersistFileFormat
    {
        CONST_VTBL struct IPersistFileFormatVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPersistFileFormat_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPersistFileFormat_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPersistFileFormat_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPersistFileFormat_GetClassID(This,pClassID)	\
    ( (This)->lpVtbl -> GetClassID(This,pClassID) ) 


#define IPersistFileFormat_IsDirty(This,pfIsDirty)	\
    ( (This)->lpVtbl -> IsDirty(This,pfIsDirty) ) 

#define IPersistFileFormat_InitNew(This,nFormatIndex)	\
    ( (This)->lpVtbl -> InitNew(This,nFormatIndex) ) 

#define IPersistFileFormat_Load(This,pszFilename,grfMode,fReadOnly)	\
    ( (This)->lpVtbl -> Load(This,pszFilename,grfMode,fReadOnly) ) 

#define IPersistFileFormat_Save(This,pszFilename,fRemember,nFormatIndex)	\
    ( (This)->lpVtbl -> Save(This,pszFilename,fRemember,nFormatIndex) ) 

#define IPersistFileFormat_SaveCompleted(This,pszFilename)	\
    ( (This)->lpVtbl -> SaveCompleted(This,pszFilename) ) 

#define IPersistFileFormat_GetCurFile(This,ppszFilename,pnFormatIndex)	\
    ( (This)->lpVtbl -> GetCurFile(This,ppszFilename,pnFormatIndex) ) 

#define IPersistFileFormat_GetFormatList(This,ppszFormatList)	\
    ( (This)->lpVtbl -> GetFormatList(This,ppszFormatList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPersistFileFormat_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


