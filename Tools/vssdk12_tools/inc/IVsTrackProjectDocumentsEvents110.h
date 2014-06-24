

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

#ifndef __IVsTrackProjectDocumentsEvents110_h__
#define __IVsTrackProjectDocumentsEvents110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocumentsEvents4_FWD_DEFINED__
#define __IVsTrackProjectDocumentsEvents4_FWD_DEFINED__
typedef interface IVsTrackProjectDocumentsEvents4 IVsTrackProjectDocumentsEvents4;

#endif 	/* __IVsTrackProjectDocumentsEvents4_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocumentsEvents80.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocumentsEvents110_0000_0000 */
/* [local] */ 

#pragma once
#pragma once

enum __VSQUERYREMOVEFILEFLAGS2
    {
        VSQUERYREMOVEFILEFLAGS_IsRemovedFromProjectOnly	= 4
    } ;
typedef DWORD VSQUERYREMOVEFILEFLAGS2;


enum __VSQUERYREMOVEDIRECTORYFLAGS2
    {
        VSQUERYREMOVEDIRECTORYFLAGS_IsRemovedFromProjectOnly	= 1
    } ;
typedef DWORD VSQUERYREMOVEDIRECTORYFLAGS2;


enum __VSREMOVEFILEFLAGS2
    {
        VSREMOVEFILEFLAGS_IsRemovedFromProjectOnly	= 16
    } ;
typedef DWORD VSREMOVEFILEFLAGS2;


enum __VSREMOVEDIRECTORYFLAGS2
    {
        VSREMOVEDIRECTORYFLAGS_IsRemovedFromProjectOnly	= 4
    } ;
typedef DWORD VSREMOVEDIRECTORYFLAGS2;



extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocumentsEvents110_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocumentsEvents4_INTERFACE_DEFINED__
#define __IVsTrackProjectDocumentsEvents4_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocumentsEvents4 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocumentsEvents4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-DB74-4078-9642-5D1BC0BB5B26")
    IVsTrackProjectDocumentsEvents4 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveFilesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnQueryRemoveDirectoriesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveFilesEx( 
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS2 rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveDirectoriesEx( 
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS2 rgFlags[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocumentsEvents4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveFilesEx )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveDirectoriesEx )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFilesEx )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS2 rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectoriesEx )( 
            __RPC__in IVsTrackProjectDocumentsEvents4 * This,
            /* [in] */ int cProjects,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) IVsProject *rgpProjects[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cProjects) const int rgFirstIndices[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS2 rgFlags[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocumentsEvents4Vtbl;

    interface IVsTrackProjectDocumentsEvents4
    {
        CONST_VTBL struct IVsTrackProjectDocumentsEvents4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocumentsEvents4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocumentsEvents4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocumentsEvents4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocumentsEvents4_OnQueryRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents4_OnQueryRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocumentsEvents4_OnAfterRemoveFilesEx(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFilesEx(This,cProjects,cFiles,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocumentsEvents4_OnAfterRemoveDirectoriesEx(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectoriesEx(This,cProjects,cDirectories,rgpProjects,rgFirstIndices,rgpszMkDocuments,rgFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocumentsEvents4_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


