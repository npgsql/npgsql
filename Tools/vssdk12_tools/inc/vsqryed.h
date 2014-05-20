

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

#ifndef __vsqryed_h__
#define __vsqryed_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsQueryEditQuerySave_FWD_DEFINED__
#define __IVsQueryEditQuerySave_FWD_DEFINED__
typedef interface IVsQueryEditQuerySave IVsQueryEditQuerySave;

#endif 	/* __IVsQueryEditQuerySave_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsQueryEditQuerySave90.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vsqryed_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_vsqryed_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vsqryed_0000_0000_v0_0_s_ifspec;

#ifndef __IVsQueryEditQuerySave_INTERFACE_DEFINED__
#define __IVsQueryEditQuerySave_INTERFACE_DEFINED__

/* interface IVsQueryEditQuerySave */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsQueryEditQuerySave;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-7E28-4d0c-A00F-3446801350CE")
    IVsQueryEditQuerySave : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeginQuerySaveBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndQuerySaveBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryEditFiles( 
            /* [in] */ VSQueryEditFlags rgfQueryEdit,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQueryEditResult *pfEditCanceled,
            /* [out] */ __RPC__out VSQueryEditResultFlags *prgfMoreInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeclareUnreloadableFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSaveUnreloadableFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QuerySaveFile( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReloadable( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [retval][out] */ __RPC__out BOOL *pbResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsQueryEditQuerySaveVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsQueryEditQuerySave * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsQueryEditQuerySave * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginQuerySaveBatch )( 
            __RPC__in IVsQueryEditQuerySave * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndQuerySaveBatch )( 
            __RPC__in IVsQueryEditQuerySave * This);
        
        HRESULT ( STDMETHODCALLTYPE *QueryEditFiles )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ VSQueryEditFlags rgfQueryEdit,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQueryEditResult *pfEditCanceled,
            /* [out] */ __RPC__out VSQueryEditResultFlags *prgfMoreInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DeclareUnreloadableFile )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSaveUnreloadableFile )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySaveFile )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [retval][out] */ __RPC__out VSQuerySaveResult *pdwQSResult);
        
        HRESULT ( STDMETHODCALLTYPE *IsReloadable )( 
            __RPC__in IVsQueryEditQuerySave * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [retval][out] */ __RPC__out BOOL *pbResult);
        
        END_INTERFACE
    } IVsQueryEditQuerySaveVtbl;

    interface IVsQueryEditQuerySave
    {
        CONST_VTBL struct IVsQueryEditQuerySaveVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryEditQuerySave_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryEditQuerySave_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryEditQuerySave_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryEditQuerySave_BeginQuerySaveBatch(This)	\
    ( (This)->lpVtbl -> BeginQuerySaveBatch(This) ) 

#define IVsQueryEditQuerySave_EndQuerySaveBatch(This)	\
    ( (This)->lpVtbl -> EndQuerySaveBatch(This) ) 

#define IVsQueryEditQuerySave_QueryEditFiles(This,rgfQueryEdit,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pfEditCanceled,prgfMoreInfo)	\
    ( (This)->lpVtbl -> QueryEditFiles(This,rgfQueryEdit,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pfEditCanceled,prgfMoreInfo) ) 

#define IVsQueryEditQuerySave_DeclareUnreloadableFile(This,pszMkDocument,rgf,pFileInfo)	\
    ( (This)->lpVtbl -> DeclareUnreloadableFile(This,pszMkDocument,rgf,pFileInfo) ) 

#define IVsQueryEditQuerySave_OnAfterSaveUnreloadableFile(This,pszMkDocument,rgf,pFileInfo)	\
    ( (This)->lpVtbl -> OnAfterSaveUnreloadableFile(This,pszMkDocument,rgf,pFileInfo) ) 

#define IVsQueryEditQuerySave_QuerySaveFile(This,pszMkDocument,rgf,pFileInfo,pdwQSResult)	\
    ( (This)->lpVtbl -> QuerySaveFile(This,pszMkDocument,rgf,pFileInfo,pdwQSResult) ) 

#define IVsQueryEditQuerySave_IsReloadable(This,pszMkDocument,pbResult)	\
    ( (This)->lpVtbl -> IsReloadable(This,pszMkDocument,pbResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryEditQuerySave_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


