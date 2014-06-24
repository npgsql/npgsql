

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

#ifndef __vstrkdoc_h__
#define __vstrkdoc_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocuments_FWD_DEFINED__
#define __IVsTrackProjectDocuments_FWD_DEFINED__
typedef interface IVsTrackProjectDocuments IVsTrackProjectDocuments;

#endif 	/* __IVsTrackProjectDocuments_FWD_DEFINED__ */


#ifndef __IVsTrackProjectDocumentsEvents_FWD_DEFINED__
#define __IVsTrackProjectDocumentsEvents_FWD_DEFINED__
typedef interface IVsTrackProjectDocumentsEvents IVsTrackProjectDocumentsEvents;

#endif 	/* __IVsTrackProjectDocumentsEvents_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocuments110.h"
#include "vssccmgr.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vstrkdoc_0000_0000 */
/* [local] */ 

#pragma once
#pragma once




extern RPC_IF_HANDLE __MIDL_itf_vstrkdoc_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vstrkdoc_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocuments_INTERFACE_DEFINED__
#define __IVsTrackProjectDocuments_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocuments */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocuments;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-449A-4487-A56F-740CF8130032")
    IVsTrackProjectDocuments : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE BeginBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Flush( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddFiles( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
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
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSccStatusChanged( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSSCCSTATUS rgSccStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTrackProjectDocumentsEvents( 
            /* [in] */ __RPC__in_opt IVsTrackProjectDocumentsEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTrackProjectDocumentsEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocumentsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTrackProjectDocuments * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTrackProjectDocuments * This);
        
        HRESULT ( STDMETHODCALLTYPE *BeginBatch )( 
            __RPC__in IVsTrackProjectDocuments * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBatch )( 
            __RPC__in IVsTrackProjectDocuments * This);
        
        HRESULT ( STDMETHODCALLTYPE *Flush )( 
            __RPC__in IVsTrackProjectDocuments * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddFiles )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddDirectories )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFiles )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectories )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSccStatusChanged )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSSCCSTATUS rgSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameFile )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameFile )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTrackProjectDocumentsEvents )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ __RPC__in_opt IVsTrackProjectDocumentsEvents *pEventSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTrackProjectDocumentsEvents )( 
            __RPC__in IVsTrackProjectDocuments * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        END_INTERFACE
    } IVsTrackProjectDocumentsVtbl;

    interface IVsTrackProjectDocuments
    {
        CONST_VTBL struct IVsTrackProjectDocumentsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocuments_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocuments_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocuments_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocuments_BeginBatch(This)	\
    ( (This)->lpVtbl -> BeginBatch(This) ) 

#define IVsTrackProjectDocuments_EndBatch(This)	\
    ( (This)->lpVtbl -> EndBatch(This) ) 

#define IVsTrackProjectDocuments_Flush(This)	\
    ( (This)->lpVtbl -> Flush(This) ) 

#define IVsTrackProjectDocuments_OnAfterAddFiles(This,pProject,cFiles,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddFiles(This,pProject,cFiles,rgpszMkDocuments) ) 

#define IVsTrackProjectDocuments_OnAfterAddDirectories(This,pProject,cDirectories,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddDirectories(This,pProject,cDirectories,rgpszMkDocuments) ) 

#define IVsTrackProjectDocuments_OnAfterRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFiles(This,pProject,cFiles,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments_OnAfterRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectories(This,pProject,cDirectories,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments_OnAfterSccStatusChanged(This,pProject,cFiles,rgpszMkDocuments,rgSccStatus)	\
    ( (This)->lpVtbl -> OnAfterSccStatusChanged(This,pProject,cFiles,rgpszMkDocuments,rgSccStatus) ) 

#define IVsTrackProjectDocuments_OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue)	\
    ( (This)->lpVtbl -> OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue) ) 

#define IVsTrackProjectDocuments_OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags)	\
    ( (This)->lpVtbl -> OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags) ) 

#define IVsTrackProjectDocuments_AdviseTrackProjectDocumentsEvents(This,pEventSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseTrackProjectDocumentsEvents(This,pEventSink,pdwCookie) ) 

#define IVsTrackProjectDocuments_UnadviseTrackProjectDocumentsEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseTrackProjectDocumentsEvents(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocuments_INTERFACE_DEFINED__ */


#ifndef __IVsTrackProjectDocumentsEvents_INTERFACE_DEFINED__
#define __IVsTrackProjectDocumentsEvents_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocumentsEvents */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocumentsEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-A98B-4fd3-AA79-B182EE26185B")
    IVsTrackProjectDocumentsEvents : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddFiles( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterAddDirectories( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveFiles( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveDirectories( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterSccStatusChanged( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSSCCSTATUS rgSccStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRenameFile( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocumentsEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddFiles )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterAddDirectories )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFiles )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectories )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterSccStatusChanged )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSSCCSTATUS rgSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRenameFile )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags,
            /* [out] */ __RPC__out BOOL *pfRenameCanContinue);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRenameFile )( 
            __RPC__in IVsTrackProjectDocumentsEvents * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ __RPC__in LPCOLESTR pszMkOldName,
            /* [in] */ __RPC__in LPCOLESTR pszMkNewName,
            /* [in] */ VSRENAMEFILEFLAGS flags);
        
        END_INTERFACE
    } IVsTrackProjectDocumentsEventsVtbl;

    interface IVsTrackProjectDocumentsEvents
    {
        CONST_VTBL struct IVsTrackProjectDocumentsEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocumentsEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocumentsEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocumentsEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocumentsEvents_OnAfterAddFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments) ) 

#define IVsTrackProjectDocumentsEvents_OnAfterAddDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments)	\
    ( (This)->lpVtbl -> OnAfterAddDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments) ) 

#define IVsTrackProjectDocumentsEvents_OnAfterRemoveFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFiles(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents_OnAfterRemoveDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectories(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents_OnAfterSccStatusChanged(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgSccStatus)	\
    ( (This)->lpVtbl -> OnAfterSccStatusChanged(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgSccStatus) ) 

#define IVsTrackProjectDocumentsEvents_OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue)	\
    ( (This)->lpVtbl -> OnQueryRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags,pfRenameCanContinue) ) 

#define IVsTrackProjectDocumentsEvents_OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags)	\
    ( (This)->lpVtbl -> OnAfterRenameFile(This,pProject,pszMkOldName,pszMkNewName,flags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocumentsEvents_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


