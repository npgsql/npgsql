

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsQueryEditQuerySave90.idl:
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

#ifndef __IVsQueryEditQuerySave90_h__
#define __IVsQueryEditQuerySave90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsQueryEditQuerySave3_FWD_DEFINED__
#define __IVsQueryEditQuerySave3_FWD_DEFINED__
typedef interface IVsQueryEditQuerySave3 IVsQueryEditQuerySave3;
#endif 	/* __IVsQueryEditQuerySave3_FWD_DEFINED__ */


/* header files for imported files */
#include "IVsQueryEditQuerySave2.h"
#include "IVsQueryEditQuerySave80.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsQueryEditQuerySave90_0000_0000 */
/* [local] */ 

#pragma once
#pragma once

enum __VSQuerySaveFlags2
    {	QSF_DetectAnyChangedFile	= 0x2
    } ;
typedef DWORD VSQuerySaveFlags2;


enum tagVSQuerySaveResultFlags
    {	QSR_DefaultFlag	= 0,
	QSR_Reloaded	= 0x1,
	QSR_Changed	= 0x2
    } ;
typedef DWORD VSQuerySaveResultFlags;



extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsQueryEditQuerySave90_0000_0000_v0_0_s_ifspec;

#ifndef __IVsQueryEditQuerySave3_INTERFACE_DEFINED__
#define __IVsQueryEditQuerySave3_INTERFACE_DEFINED__

/* interface IVsQueryEditQuerySave3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsQueryEditQuerySave3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53474C4D-6279-30c2-b4b4-005004775ab4")
    IVsQueryEditQuerySave3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE QuerySaveFiles2( 
            /* [in] */ VSQuerySaveFlags rgfQuerySave,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQuerySaveResult *pdwQSResult,
            /* [out] */ __RPC__out VSQuerySaveResultFlags *prgfMoreInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QuerySaveFile2( 
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [out] */ __RPC__out VSQuerySaveResult *pdwQSResult,
            /* [out] */ __RPC__out VSQuerySaveResultFlags *prgfMoreInfo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsQueryEditQuerySave3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsQueryEditQuerySave3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsQueryEditQuerySave3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsQueryEditQuerySave3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySaveFiles2 )( 
            IVsQueryEditQuerySave3 * This,
            /* [in] */ VSQuerySaveFlags rgfQuerySave,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszMkDocuments[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQSFlags rgrgf[  ],
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const VSQEQS_FILE_ATTRIBUTE_DATA rgFileInfo[  ],
            /* [out] */ __RPC__out VSQuerySaveResult *pdwQSResult,
            /* [out] */ __RPC__out VSQuerySaveResultFlags *prgfMoreInfo);
        
        HRESULT ( STDMETHODCALLTYPE *QuerySaveFile2 )( 
            IVsQueryEditQuerySave3 * This,
            /* [in] */ __RPC__in LPCOLESTR pszMkDocument,
            /* [in] */ VSQEQSFlags rgf,
            /* [in] */ __RPC__in const VSQEQS_FILE_ATTRIBUTE_DATA *pFileInfo,
            /* [out] */ __RPC__out VSQuerySaveResult *pdwQSResult,
            /* [out] */ __RPC__out VSQuerySaveResultFlags *prgfMoreInfo);
        
        END_INTERFACE
    } IVsQueryEditQuerySave3Vtbl;

    interface IVsQueryEditQuerySave3
    {
        CONST_VTBL struct IVsQueryEditQuerySave3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsQueryEditQuerySave3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsQueryEditQuerySave3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsQueryEditQuerySave3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsQueryEditQuerySave3_QuerySaveFiles2(This,rgfQuerySave,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pdwQSResult,prgfMoreInfo)	\
    ( (This)->lpVtbl -> QuerySaveFiles2(This,rgfQuerySave,cFiles,rgpszMkDocuments,rgrgf,rgFileInfo,pdwQSResult,prgfMoreInfo) ) 

#define IVsQueryEditQuerySave3_QuerySaveFile2(This,pszMkDocument,rgf,pFileInfo,pdwQSResult,prgfMoreInfo)	\
    ( (This)->lpVtbl -> QuerySaveFile2(This,pszMkDocument,rgf,pFileInfo,pdwQSResult,prgfMoreInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsQueryEditQuerySave3_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


