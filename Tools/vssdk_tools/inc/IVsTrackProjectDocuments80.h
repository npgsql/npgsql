

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectDocuments80.idl:
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

#ifndef __IVsTrackProjectDocuments80_h__
#define __IVsTrackProjectDocuments80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocuments3_FWD_DEFINED__
#define __IVsTrackProjectDocuments3_FWD_DEFINED__
typedef interface IVsTrackProjectDocuments3 IVsTrackProjectDocuments3;
#endif 	/* __IVsTrackProjectDocuments3_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocuments2.h"
#include "IVsTrackProjectDocumentsEvents80.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocuments80_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments80_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocuments3_INTERFACE_DEFINED__
#define __IVsTrackProjectDocuments3_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocuments3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocuments3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-9097-4325-9270-754EB85A6351")
    IVsTrackProjectDocuments3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeginQueryBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndQueryBatch( 
            /* [retval][out] */ __RPC__out BOOL *pfActionOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelQueryBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryAddFilesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszNewMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszSrcMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HandsOffFiles( 
            /* [in] */ HANDSOFFMODE grfRequiredAccess,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HandsOnFiles( 
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocuments3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectDocuments3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectDocuments3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectDocuments3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginQueryBatch )( 
            IVsTrackProjectDocuments3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndQueryBatch )( 
            IVsTrackProjectDocuments3 * This,
            /* [retval][out] */ __RPC__out BOOL *pfActionOK);
        
        HRESULT ( STDMETHODCALLTYPE *CancelQueryBatch )( 
            IVsTrackProjectDocuments3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddFilesEx )( 
            IVsTrackProjectDocuments3 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszNewMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszSrcMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOffFiles )( 
            IVsTrackProjectDocuments3 * This,
            /* [in] */ HANDSOFFMODE grfRequiredAccess,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOnFiles )( 
            IVsTrackProjectDocuments3 * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocuments3Vtbl;

    interface IVsTrackProjectDocuments3
    {
        CONST_VTBL struct IVsTrackProjectDocuments3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocuments3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocuments3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocuments3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocuments3_BeginQueryBatch(This)	\
    ( (This)->lpVtbl -> BeginQueryBatch(This) ) 

#define IVsTrackProjectDocuments3_EndQueryBatch(This,pfActionOK)	\
    ( (This)->lpVtbl -> EndQueryBatch(This,pfActionOK) ) 

#define IVsTrackProjectDocuments3_CancelQueryBatch(This)	\
    ( (This)->lpVtbl -> CancelQueryBatch(This) ) 

#define IVsTrackProjectDocuments3_OnQueryAddFilesEx(This,pProject,cFiles,rgpszNewMkDocuments,rgpszSrcMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddFilesEx(This,pProject,cFiles,rgpszNewMkDocuments,rgpszSrcMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments3_HandsOffFiles(This,grfRequiredAccess,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> HandsOffFiles(This,grfRequiredAccess,cFiles,rgpszMkDocuments) ) 

#define IVsTrackProjectDocuments3_HandsOnFiles(This,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> HandsOnFiles(This,cFiles,rgpszMkDocuments) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocuments3_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


