

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectDocumentsEvents80.idl:
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

#ifndef __IVsTrackProjectDocumentsEvents80_h__
#define __IVsTrackProjectDocumentsEvents80_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocumentsEvents3_FWD_DEFINED__
#define __IVsTrackProjectDocumentsEvents3_FWD_DEFINED__
typedef interface IVsTrackProjectDocumentsEvents3 IVsTrackProjectDocumentsEvents3;
#endif 	/* __IVsTrackProjectDocumentsEvents3_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocumentsEvents2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocumentsEvents80_0000_0000 */
/* [local] */ 

#pragma once
#pragma once

enum __HANDSOFFMODE
    {	HANDSOFFMODE_ReadAccess	= 0x1,
	HANDSOFFMODE_WriteAccess	= 0x2,
	HANDSOFFMODE_DeleteAccess	= 0x4,
	HANDSOFFMODE_AsyncOperation	= 0x80000000,
	HANDSOFFMODE_FullAccess	= ( ( HANDSOFFMODE_DeleteAccess | HANDSOFFMODE_ReadAccess )  | HANDSOFFMODE_WriteAccess ) ,
	HANDSOFFMODE_ReadWriteAccess	= ( HANDSOFFMODE_ReadAccess | HANDSOFFMODE_WriteAccess ) 
    } ;
typedef DWORD HANDSOFFMODE;



extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents80_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents80_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocumentsEvents3_INTERFACE_DEFINED__
#define __IVsTrackProjectDocumentsEvents3_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocumentsEvents3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocumentsEvents3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-BD74-4D21-A79F-2C190E38AB6F")
    IVsTrackProjectDocumentsEvents3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnBeginQueryBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnEndQueryBatch( 
            /* [retval][out] */ __RPC__out BOOL *pfActionOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnCancelQueryBatch( void) = 0;
        
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

    typedef struct IVsTrackProjectDocumentsEvents3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectDocumentsEvents3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectDocumentsEvents3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectDocumentsEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnBeginQueryBatch )( 
            IVsTrackProjectDocumentsEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnEndQueryBatch )( 
            IVsTrackProjectDocumentsEvents3 * This,
            /* [retval][out] */ __RPC__out BOOL *pfActionOK);
        
        HRESULT ( STDMETHODCALLTYPE *OnCancelQueryBatch )( 
            IVsTrackProjectDocumentsEvents3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddFilesEx )( 
            IVsTrackProjectDocumentsEvents3 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszNewMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszSrcMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOffFiles )( 
            IVsTrackProjectDocumentsEvents3 * This,
            /* [in] */ HANDSOFFMODE grfRequiredAccess,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *HandsOnFiles )( 
            IVsTrackProjectDocumentsEvents3 * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocumentsEvents3Vtbl;

    interface IVsTrackProjectDocumentsEvents3
    {
        CONST_VTBL struct IVsTrackProjectDocumentsEvents3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocumentsEvents3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocumentsEvents3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocumentsEvents3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocumentsEvents3_OnBeginQueryBatch(This)	\
    ( (This)->lpVtbl -> OnBeginQueryBatch(This) ) 

#define IVsTrackProjectDocumentsEvents3_OnEndQueryBatch(This,pfActionOK)	\
    ( (This)->lpVtbl -> OnEndQueryBatch(This,pfActionOK) ) 

#define IVsTrackProjectDocumentsEvents3_OnCancelQueryBatch(This)	\
    ( (This)->lpVtbl -> OnCancelQueryBatch(This) ) 

#define IVsTrackProjectDocumentsEvents3_OnQueryAddFilesEx(This,pProject,cFiles,rgpszNewMkDocuments,rgpszSrcMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddFilesEx(This,pProject,cFiles,rgpszNewMkDocuments,rgpszSrcMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents3_HandsOffFiles(This,grfRequiredAccess,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> HandsOffFiles(This,grfRequiredAccess,cFiles,rgpszMkDocuments) ) 

#define IVsTrackProjectDocumentsEvents3_HandsOnFiles(This,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> HandsOnFiles(This,cFiles,rgpszMkDocuments) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocumentsEvents3_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


