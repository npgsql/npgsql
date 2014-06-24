

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsTrackProjectDocuments2.idl:
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

#ifndef __IVsTrackProjectDocuments2_h__
#define __IVsTrackProjectDocuments2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocuments2_FWD_DEFINED__
#define __IVsTrackProjectDocuments2_FWD_DEFINED__
typedef interface IVsTrackProjectDocuments2 IVsTrackProjectDocuments2;
#endif 	/* __IVsTrackProjectDocuments2_FWD_DEFINED__ */


#ifndef __SVsTrackProjectDocuments_FWD_DEFINED__
#define __SVsTrackProjectDocuments_FWD_DEFINED__
typedef interface SVsTrackProjectDocuments SVsTrackProjectDocuments;
#endif 	/* __SVsTrackProjectDocuments_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocumentsEvents2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocuments2_0000_0000 */
/* [local] */ 

#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocuments2_INTERFACE_DEFINED__
#define __IVsTrackProjectDocuments2_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocuments2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocuments2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-6639-11d3-a60d-005004775ab1")
    IVsTrackProjectDocuments2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeginBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Flush( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryAddFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddFilesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSADDFILEFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddDirectoriesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSADDDIRECTORYFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYRENAMEFILEFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYRENAMEFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSRENAMEFILEFLAGS rgflags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSQUERYRENAMEDIRECTORYFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirs) VSQUERYRENAMEDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSRENAMEDIRECTORYFLAGS rgflags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTrackProjectDocumentsEvents( 
            /* [in] */ __RPC__in_opt IVsTrackProjectDocumentsEvents2 *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTrackProjectDocumentsEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryAddDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYADDDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYADDDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveDirectories( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSccStatusChanged( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwSccStatus[  ]) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocuments2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTrackProjectDocuments2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTrackProjectDocuments2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginBatch )( 
            IVsTrackProjectDocuments2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBatch )( 
            IVsTrackProjectDocuments2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *Flush )( 
            IVsTrackProjectDocuments2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYADDFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYADDFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddFilesEx )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSADDFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddDirectoriesEx )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSADDDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYRENAMEFILEFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYRENAMEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameFile )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSRENAMEFILEFLAGS rgflags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameFile )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSQUERYRENAMEDIRECTORYFLAGS rgflags[  ],
            /* [out] */ __RPC__out VSQUERYRENAMEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirs) VSQUERYRENAMEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirs,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkOldNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const LPCOLESTR rgszMkNewNames[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirs) const VSRENAMEDIRECTORYFLAGS rgflags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTrackProjectDocumentsEvents )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsTrackProjectDocumentsEvents2 *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTrackProjectDocumentsEvents )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryAddDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYADDDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYADDDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYADDDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveFiles )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveDirectories )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSccStatusChanged )( 
            IVsTrackProjectDocuments2 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const DWORD rgdwSccStatus[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocuments2Vtbl;

    interface IVsTrackProjectDocuments2
    {
        CONST_VTBL struct IVsTrackProjectDocuments2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocuments2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocuments2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocuments2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocuments2_BeginBatch(This)	\
    ( (This)->lpVtbl -> BeginBatch(This) ) 

#define IVsTrackProjectDocuments2_EndBatch(This)	\
    ( (This)->lpVtbl -> EndBatch(This) ) 

#define IVsTrackProjectDocuments2_Flush(This)	\
    ( (This)->lpVtbl -> Flush(This) ) 

#define IVsTrackProjectDocuments2_OnQueryAddFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnAfterAddFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterAddFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments2_OnAfterAddFiles(This,pProject,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddFiles(This,pProject,cFiles,rgpszMkDocuments) ) 

#define IVsTrackProjectDocuments2_OnAfterAddDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterAddDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments2_OnAfterAddDirectories(This,pProject,cDirectories,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddDirectories(This,pProject,cDirectories,rgpszMkDocuments) ) 

#define IVsTrackProjectDocuments2_OnAfterRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments2_OnAfterRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments2_OnQueryRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue)	\
    ( (This)->lpVtbl -> OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue) ) 

#define IVsTrackProjectDocuments2_OnAfterRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags)	\
    ( (This)->lpVtbl -> OnAfterRenameFiles(This,pProject,cFiles,rgszMkOldNames,rgszMkNewNames,rgflags) ) 

#define IVsTrackProjectDocuments2_OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags)	\
    ( (This)->lpVtbl -> OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags) ) 

#define IVsTrackProjectDocuments2_OnQueryRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnAfterRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags)	\
    ( (This)->lpVtbl -> OnAfterRenameDirectories(This,pProject,cDirs,rgszMkOldNames,rgszMkNewNames,rgflags) ) 

#define IVsTrackProjectDocuments2_AdviseTrackProjectDocumentsEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTrackProjectDocumentsEvents(This,pEventSink,pdwCookie) ) 

#define IVsTrackProjectDocuments2_UnadviseTrackProjectDocumentsEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTrackProjectDocumentsEvents(This,dwCookie) ) 

#define IVsTrackProjectDocuments2_OnQueryAddDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryAddDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnQueryRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnQueryRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments2_OnAfterSccStatusChanged(This,pProject,cFiles,rgpszMkDocuments,rgdwSccStatus)	\
    ( (This)->lpVtbl -> OnAfterSccStatusChanged(This,pProject,cFiles,rgpszMkDocuments,rgdwSccStatus) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocuments2_INTERFACE_DEFINED__ */


#ifndef __SVsTrackProjectDocuments_INTERFACE_DEFINED__
#define __SVsTrackProjectDocuments_INTERFACE_DEFINED__

/* interface SVsTrackProjectDocuments */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsTrackProjectDocuments;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-1639-11d3-a60d-005004775ab1")
    SVsTrackProjectDocuments : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsTrackProjectDocumentsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsTrackProjectDocuments * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsTrackProjectDocuments * This);
        
        END_INTERFACE
    } SVsTrackProjectDocumentsVtbl;

    interface SVsTrackProjectDocuments
    {
        CONST_VTBL struct SVsTrackProjectDocumentsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsTrackProjectDocuments_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsTrackProjectDocuments_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsTrackProjectDocuments_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsTrackProjectDocuments_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsTrackProjectDocuments2_0000_0002 */
/* [local] */ 

#define SID_SVsTrackProjectDocuments IID_SVsTrackProjectDocuments


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments2_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments2_0000_0002_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


