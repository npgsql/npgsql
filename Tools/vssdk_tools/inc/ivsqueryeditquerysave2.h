

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsQueryEditQuerySave2.idl:
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

#ifndef __IVsQueryEditQuerySave2_h__
#define __IVsQueryEditQuerySave2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsQueryEditQuerySave2_FWD_DEFINED__
#define __IVsQueryEditQuerySave2_FWD_DEFINED__
typedef interface IVsQueryEditQuerySave2 IVsQueryEditQuerySave2;
#endif 	/* __IVsQueryEditQuerySave2_FWD_DEFINED__ */


#ifndef __SVsQueryEditQuerySave_FWD_DEFINED__
#define __SVsQueryEditQuerySave_FWD_DEFINED__
typedef interface SVsQueryEditQuerySave SVsQueryEditQuerySave;
#endif 	/* __SVsQueryEditQuerySave_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsQueryEditQuerySave2_0000_0000 */
/* [local] */ 

#pragma once
#pragma once

enum tagVSQueryEditFlags
    {	QEF_AllowInMemoryEdits	= 0,
	QEF_ForceInMemoryEdits	= 1,
	QEF_DisallowInMemoryEdits	= 2,
	QEF_SilentMode	= 4,
	QEF_ImplicitEdit	= 8,
	QEF_ReportOnly	= 16,
	QEF_NoReload	= 32,
	QEF_ForceEdit_NoPrompting	= 64
    } ;
typedef DWORD VSQueryEditFlags;


enum tagVSQueryEditResult
    {	QER_EditOK	= 0,
	QER_NoEdit_UserCanceled	= 1,
	QER_EditNotOK	= 1
    } ;
typedef DWORD VSQueryEditResult;


enum tagVSQueryEditResultFlags
    {	QER_MaybeCheckedout	= 1,
	QER_MaybeChanged	= 2,
	QER_InMemoryEdit	= 4,
	QER_InMemoryEditNotAllowed	= 8,
	QER_NoisyCheckoutRequired	= 16,
	QER_NoisyPromptRequired	= 16,
	QER_CheckoutCanceledOrFailed	= 32,
	QER_EditNotPossible	= 64,
	QER_ReadOnlyNotUnderScc	= 128,
	QER_ReadOnlyUnderScc	= 256
    } ;
typedef DWORD VSQueryEditResultFlags;


enum tagVSQuerySaveFlags
    {	QSF_DefaultOperation	= 0,
	QSF_SilentMode	= 1
    } ;
typedef DWORD VSQuerySaveFlags;


enum tagVSQuerySaveResult
    {	QSR_SaveOK	= 0,
	QSR_NoSave_UserCanceled	= 1,
	QSR_NoSave_Cancel	= 1,
	QSR_ForceSaveAs	= 2,
	QSR_NoSave_Continue	= 3,
	QSR_NoSave_NoisyPromptRequired	= 4
    } ;
typedef DWORD VSQuerySaveResult;


enum tagVSQEQSFlags
    {	VSQEQS_FileInfo	= 0x1,
	VSQEQS_AllowCheckout	= 0x2,
	VSQEQS_NoSaveAs	= 0x4
    } ;
typedef DWORD VSQEQSFlags;

typedef /* [public][public][public][public][public][public][public] */ struct __MIDL___MIDL_itf_IVsQueryEditQuerySave2_0000_0000_0001
    {
    DWORD dwFileAttributes;
    FILETIME ftLastWriteTime;
    DWORD nFileSizeHigh;
    DWORD nFileSizeLow;
    } 	VSQEQS_FILE_ATTRIBUTE_DATA;



extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsQueryEditQuerySave2_INTERFACE_DEFINED__
#define __IVsQueryEditQuerySave2_INTERFACE_DEFINED__

/* interface IVsQueryEditQuerySave2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsQueryEditQuerySave2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-5984-11d3-a606-005004775ab1")
    IVsQueryEditQuerySave2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QueryEditFiles( 
            /* [in] */ VSQueryEditFlags rgfQueryEdit,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQueryEditResult *pfEditVerdict,
            /* [out] */ __RPC__out VSQueryEditResultFlags *prgfMoreInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QuerySaveFiles( 
            /* [in] */ VSQuerySaveFlags rgfQuerySave,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QuerySaveFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeclareReloadableFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeclareUnreloadableFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSaveUnreloadableFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReloadable( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [retval][out] */ __RPC__out BOOL *pbResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BeginQuerySaveBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndQuerySaveBatch( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsQueryEditQuerySave2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsQueryEditQuerySave2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsQueryEditQuerySave2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryEditFiles )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ VSQueryEditFlags rgfQueryEdit,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQueryEditResult *pfEditVerdict,
            /* [out] */ __RPC__out VSQueryEditResultFlags *prgfMoreInfo);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySaveFiles )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ VSQuerySaveFlags rgfQuerySave,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySaveFile )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult);
        
        HRESULT ( STDMETHODCALLTYPE *DeclareReloadableFile )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DeclareUnreloadableFile )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSaveUnreloadableFile )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *IsReloadable )( 
            IVsQueryEditQuerySave2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [retval][out] */ __RPC__out BOOL *pbResult);
        
        HRESULT ( STDMETHODCALLTYPE *BeginQuerySaveBatch )( 
            IVsQueryEditQuerySave2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndQuerySaveBatch )( 
            IVsQueryEditQuerySave2 * This);
        
        END_INTERFACE
    } IVsQueryEditQuerySave2Vtbl;

    interface IVsQueryEditQuerySave2
    {
        CONST_VTBL struct IVsQueryEditQuerySave2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryEditQuerySave2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryEditQuerySave2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryEditQuerySave2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryEditQuerySave2_QueryEditFiles(This,rgfQueryEdit,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pfEditVerdict,prgfMoreInfo)	\
    ( (This)->lpVtbl -> QueryEditFiles(This,rgfQueryEdit,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pfEditVerdict,prgfMoreInfo) ) 

#define IVsQueryEditQuerySave2_QuerySaveFiles(This,rgfQuerySave,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pdwQSResult)	\
    ( (This)->lpVtbl -> QuerySaveFiles(This,rgfQuerySave,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pdwQSResult) ) 

#define IVsQueryEditQuerySave2_QuerySaveFile(This,pszMkDocument,rgf,pFileInfo,pdwQSResult)	\
    ( (This)->lpVtbl -> QuerySaveFile(This,pszMkDocument,rgf,pFileInfo,pdwQSResult) ) 

#define IVsQueryEditQuerySave2_DeclareReloadableFile(This,pszMkDocument,rgf,pFileInfo)	\
    ( (This)->lpVtbl -> DeclareReloadableFile(This,pszMkDocument,rgf,pFileInfo) ) 

#define IVsQueryEditQuerySave2_DeclareUnreloadableFile(This,pszMkDocument,rgf,pFileInfo)	\
    ( (This)->lpVtbl -> DeclareUnreloadableFile(This,pszMkDocument,rgf,pFileInfo) ) 

#define IVsQueryEditQuerySave2_OnAfterSaveUnreloadableFile(This,pszMkDocument,rgf,pFileInfo)	\
    ( (This)->lpVtbl -> OnAfterSaveUnreloadableFile(This,pszMkDocument,rgf,pFileInfo) ) 

#define IVsQueryEditQuerySave2_IsReloadable(This,pszMkDocument,pbResult)	\
    ( (This)->lpVtbl -> IsReloadable(This,pszMkDocument,pbResult) ) 

#define IVsQueryEditQuerySave2_BeginQuerySaveBatch(This)	\
    ( (This)->lpVtbl -> BeginQuerySaveBatch(This) ) 

#define IVsQueryEditQuerySave2_EndQuerySaveBatch(This)	\
    ( (This)->lpVtbl -> EndQuerySaveBatch(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryEditQuerySave2_INTERFACE_DEFINED__ */


#ifndef __SVsQueryEditQuerySave_INTERFACE_DEFINED__
#define __SVsQueryEditQuerySave_INTERFACE_DEFINED__

/* interface SVsQueryEditQuerySave */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsQueryEditQuerySave;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-1984-11d3-a606-005004775ab1")
    SVsQueryEditQuerySave : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsQueryEditQuerySaveVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsQueryEditQuerySave * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsQueryEditQuerySave * This);
        
        END_INTERFACE
    } SVsQueryEditQuerySaveVtbl;

    interface SVsQueryEditQuerySave
    {
        CONST_VTBL struct SVsQueryEditQuerySaveVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsQueryEditQuerySave_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsQueryEditQuerySave_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsQueryEditQuerySave_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsQueryEditQuerySave_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsQueryEditQuerySave2_0000_0002 */
/* [local] */ 

#define SID_SVsQueryEditQuerySave IID_SVsQueryEditQuerySave


extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave2_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave2_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


