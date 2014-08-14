

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccProject2.idl:
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

#ifndef __IVsSccProject2_h__
#define __IVsSccProject2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccProject2_FWD_DEFINED__
#define __IVsSccProject2_FWD_DEFINED__
typedef interface IVsSccProject2 IVsSccProject2;
#endif 	/* __IVsSccProject2_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccProject2_0000_0000 */
/* [local] */ 

#pragma once

enum tagVsSccFilesFlags
    {	SFF_NoFlags	= 0,
	SFF_HasSpecialFiles	= 1
    } ;
typedef DWORD VsSccFilesFlags;


enum tagVsSccSpecialFilesFlags
    {	SSFF_NoFlags	= 0
    } ;
typedef DWORD VsSccSpecialFilesFlags;



extern RPC_IF_HANDLE __MIDL_itf_IVsSccProject2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccProject2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccProject2_INTERFACE_DEFINED__
#define __IVsSccProject2_INTERFACE_DEFINED__

/* interface IVsSccProject2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccProject2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-AC92-49AC-9172-603E01FA483A")
    IVsSccProject2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SccGlyphChanged( 
            /* [in] */ int cAffectedNodes,
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const VSITEMID rgitemidAffectedNodes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const VsStateIcon rgsiNewGlyphs[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const DWORD rgdwNewSccStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSccLocation( 
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
            /* [in] */ __RPC__in LPCOLESTR pszSccFile,
            /* [out] */ __RPC__out CALPOLESTR *pCaStringsOut,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccProject2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccProject2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccProject2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccProject2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *SccGlyphChanged )( 
            IVsSccProject2 * This,
            /* [in] */ int cAffectedNodes,
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const VSITEMID rgitemidAffectedNodes[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const VsStateIcon rgsiNewGlyphs[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cAffectedNodes) const DWORD rgdwNewSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *SetSccLocation )( 
            IVsSccProject2 * This,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccProvider);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccFiles )( 
            IVsSccProject2 * This,
            /* [in] */ VSITEMID itemid,
            /* [out] */ __RPC__out CALPOLESTR *pCaStringsOut,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccSpecialFiles )( 
            IVsSccProject2 * This,
            /* [in] */ VSITEMID itemid,
            /* [in] */ __RPC__in LPCOLESTR pszSccFile,
            /* [out] */ __RPC__out CALPOLESTR *pCaStringsOut,
            /* [out] */ __RPC__out CADWORD *pCaFlagsOut);
        
        END_INTERFACE
    } IVsSccProject2Vtbl;

    interface IVsSccProject2
    {
        CONST_VTBL struct IVsSccProject2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccProject2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccProject2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccProject2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccProject2_SccGlyphChanged(This,cAffectedNodes,rgitemidAffectedNodes,rgsiNewGlyphs,rgdwNewSccStatus)	\
    ( (This)->lpVtbl -> SccGlyphChanged(This,cAffectedNodes,rgitemidAffectedNodes,rgsiNewGlyphs,rgdwNewSccStatus) ) 

#define IVsSccProject2_SetSccLocation(This,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszSccProvider)	\
    ( (This)->lpVtbl -> SetSccLocation(This,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszSccProvider) ) 

#define IVsSccProject2_GetSccFiles(This,itemid,pCaStringsOut,pCaFlagsOut)	\
    ( (This)->lpVtbl -> GetSccFiles(This,itemid,pCaStringsOut,pCaFlagsOut) ) 

#define IVsSccProject2_GetSccSpecialFiles(This,itemid,pszSccFile,pCaStringsOut,pCaFlagsOut)	\
    ( (This)->lpVtbl -> GetSccSpecialFiles(This,itemid,pszSccFile,pCaStringsOut,pCaFlagsOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccProject2_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


