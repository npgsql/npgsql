

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

#ifndef __IVsTrackProjectDocuments110_h__
#define __IVsTrackProjectDocuments110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTrackProjectDocuments4_FWD_DEFINED__
#define __IVsTrackProjectDocuments4_FWD_DEFINED__
typedef interface IVsTrackProjectDocuments4 IVsTrackProjectDocuments4;

#endif 	/* __IVsTrackProjectDocuments4_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "IVsTrackProjectDocuments80.h"
#include "IVsTrackProjectDocumentsEvents110.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsTrackProjectDocuments110_0000_0000 */
/* [local] */ 

#pragma once
#pragma once
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsTrackProjectDocuments110_0000_0000_v0_0_s_ifspec;

#ifndef __IVsTrackProjectDocuments4_INTERFACE_DEFINED__
#define __IVsTrackProjectDocuments4_INTERFACE_DEFINED__

/* interface IVsTrackProjectDocuments4 */
/* [object][custom][uuid] */ 


EXTERN_C const IID IID_IVsTrackProjectDocuments4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544c4d-19F7-4351-9168-240478819500")
    IVsTrackProjectDocuments4 : public IUnknown
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
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS2 rgFlags[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterRemoveDirectoriesEx( 
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS2 rgFlags[  ]) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTrackProjectDocuments4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTrackProjectDocuments4 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTrackProjectDocuments4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTrackProjectDocuments4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveFilesEx )( 
            __RPC__in IVsTrackProjectDocuments4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQUERYREMOVEFILEFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEFILERESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VSQUERYREMOVEFILERESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnQueryRemoveDirectoriesEx )( 
            __RPC__in IVsTrackProjectDocuments4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSQUERYREMOVEDIRECTORYFLAGS2 rgFlags[  ],
            /* [out] */ __RPC__out VSQUERYREMOVEDIRECTORYRESULTS *pSummaryResult,
            /* [size_is][out] */ __RPC__out_ecount_full(cDirectories) VSQUERYREMOVEDIRECTORYRESULTS rgResults[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveFilesEx )( 
            __RPC__in IVsTrackProjectDocuments4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSREMOVEFILEFLAGS2 rgFlags[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterRemoveDirectoriesEx )( 
            __RPC__in IVsTrackProjectDocuments4 * This,
            /* [in] */ __RPC__in_opt IVsProject *pProject,
            /* [in] */ int cDirectories,
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cDirectories) const VSREMOVEDIRECTORYFLAGS2 rgFlags[  ]);
        
        END_INTERFACE
    } IVsTrackProjectDocuments4Vtbl;

    interface IVsTrackProjectDocuments4
    {
        CONST_VTBL struct IVsTrackProjectDocuments4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTrackProjectDocuments4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTrackProjectDocuments4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTrackProjectDocuments4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTrackProjectDocuments4_OnQueryRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments4_OnQueryRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults)	\
    ( (This)->lpVtbl -> OnQueryRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags,pSummaryResult,rgResults) ) 

#define IVsTrackProjectDocuments4_OnAfterRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveFilesEx(This,pProject,cFiles,rgpszMkDocuments,rgFlags) ) 

#define IVsTrackProjectDocuments4_OnAfterRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags)	\
    ( (This)->lpVtbl -> OnAfterRemoveDirectoriesEx(This,pProject,cDirectories,rgpszMkDocuments,rgFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTrackProjectDocuments4_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


