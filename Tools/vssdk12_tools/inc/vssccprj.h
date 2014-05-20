

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

#ifndef __vssccprj_h__
#define __vssccprj_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProject_FWD_DEFINED__
#define __IVsSccProject_FWD_DEFINED__
typedef interface IVsSccProject IVsSccProject;

#endif 	/* __IVsSccProject_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "IVsSccProject2.h"
#include "vsscceng.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vssccprj_0000_0000 */
/* [local] */ 

#pragma once
#if defined(_MSC_VER) || defined(__cplusplus) || defined(__STDC__) /* C or C++ */
#include "scc.h"
#else
#endif
typedef DWORD VSSCCSTATUS;

#define	VSSCCSTATUS_INVALID	( ( VSSCCSTATUS  )-1 )



extern RPC_IF_HANDLE __MIDL_itf_vssccprj_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vssccprj_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProject_INTERFACE_DEFINED__
#define __IVsSccProject_INTERFACE_DEFINED__

/* interface IVsSccProject */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-CD19-4928-A834-AFCD8A966C36")
    IVsSccProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SccStatusChanged( 
            /* [in] */ int size,
            /* [size_is][in] */ __RPC__in_ecount_full(size) const VSITEMID rgVsitemids[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(size) const DWORD rgdwSccStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSccLocation( 
            /* [in] */ DWORD dwCookie,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccFiles( 
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__out CALPOLESTR *pCaStringsOut,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccSpecialFiles( 
            /* [in] */ VSITEMID itemid,
            /* [in] */ DWORD dwFlag,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCustomSccEngine( 
            /* [out] */ __RPC__deref_out_opt IVsSccEngine **piVsSccEngine) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SccPollFileChangeCause( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pbResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SccCheckoutForEdit( 
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [in] */ BOOL bReloadOK,
            /* [out] */ __RPC__out DWORD *pdwReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SccFileStatus( 
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out VSSCCSTATUS *pStatus,
            /* [out] */ __RPC__out CADWORD *prgStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE HasCustomSccEngine( 
            /* [out] */ __RPC__out BOOL *pbResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSccProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSccProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSccProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *SccStatusChanged )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ int size,
            /* [size_is][in] */ __RPC__in_ecount_full(size) const VSITEMID rgVsitemids[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(size) const DWORD rgdwSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetSccLocation )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ DWORD dwCookie,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccProvider);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccFiles )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__out CALPOLESTR *pCaStringsOut,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccSpecialFiles )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ DWORD dwFlag,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetCustomSccEngine )( 
            __RPC__in IVsSccProject * This,
            /* [out] */ __RPC__deref_out_opt IVsSccEngine **piVsSccEngine);
        
        HRESULT ( STDMETHODCALLTYPE *SccPollFileChangeCause )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [out] */ __RPC__out BOOL *pbResult);
        
        HRESULT ( STDMETHODCALLTYPE *SccCheckoutForEdit )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [in] */ BOOL bReloadOK,
            /* [out] */ __RPC__out DWORD *pdwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *SccFileStatus )( 
            __RPC__in IVsSccProject * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullpaths[  ],
            /* [out] */ __RPC__out VSSCCSTATUS *pStatus,
            /* [out] */ __RPC__out CADWORD *prgStatus);
        
        HRESULT ( STDMETHODCALLTYPE *HasCustomSccEngine )( 
            __RPC__in IVsSccProject * This,
            /* [out] */ __RPC__out BOOL *pbResult);
        
        END_INTERFACE
    } IVsSccProjectVtbl;

    interface IVsSccProject
    {
        CONST_VTBL struct IVsSccProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProject_SccStatusChanged(This,size,rgVsitemids,rgdwSccStatus)	\
    ( (This)->lpVtbl -> SccStatusChanged(This,size,rgVsitemids,rgdwSccStatus) ) 

#define IVsSccProject_SetSccLocation(This,dwCookie,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszSccProvider)	\
    ( (This)->lpVtbl -> SetSccLocation(This,dwCookie,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszSccProvider) ) 

#define IVsSccProject_GetSccFiles(This,itemid,pCaStringsOut,pCaFlagsOut)	\
    ( (This)->lpVtbl -> GetSccFiles(This,itemid,pCaStringsOut,pCaFlagsOut) ) 

#define IVsSccProject_GetSccSpecialFiles(This,itemid,dwFlag,pCaFlagsOut)	\
    ( (This)->lpVtbl -> GetSccSpecialFiles(This,itemid,dwFlag,pCaFlagsOut) ) 

#define IVsSccProject_GetCustomSccEngine(This,piVsSccEngine)	\
    ( (This)->lpVtbl -> GetCustomSccEngine(This,piVsSccEngine) ) 

#define IVsSccProject_SccPollFileChangeCause(This,pszMkDocument,pbResult)	\
    ( (This)->lpVtbl -> SccPollFileChangeCause(This,pszMkDocument,pbResult) ) 

#define IVsSccProject_SccCheckoutForEdit(This,cFiles,rgpszMkDocuments,bReloadOK,pdwReserved)	\
    ( (This)->lpVtbl -> SccCheckoutForEdit(This,cFiles,rgpszMkDocuments,bReloadOK,pdwReserved) ) 

#define IVsSccProject_SccFileStatus(This,cFiles,rgpszFullpaths,pStatus,prgStatus)	\
    ( (This)->lpVtbl -> SccFileStatus(This,cFiles,rgpszFullpaths,pStatus,prgStatus) ) 

#define IVsSccProject_HasCustomSccEngine(This,pbResult)	\
    ( (This)->lpVtbl -> HasCustomSccEngine(This,pbResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProject_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


