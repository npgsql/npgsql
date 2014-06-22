

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for IVsSccManager2.idl:
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

#ifndef __IVsSccManager2_h__
#define __IVsSccManager2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsSccManager2_FWD_DEFINED__
#define __IVsSccManager2_FWD_DEFINED__
typedef interface IVsSccManager2 IVsSccManager2;
#endif 	/* __IVsSccManager2_FWD_DEFINED__ */


#ifndef __ISccManagerLoaded_FWD_DEFINED__
#define __ISccManagerLoaded_FWD_DEFINED__
typedef interface ISccManagerLoaded ISccManagerLoaded;
#endif 	/* __ISccManagerLoaded_FWD_DEFINED__ */


#ifndef __IEnlistingInProject_FWD_DEFINED__
#define __IEnlistingInProject_FWD_DEFINED__
typedef interface IEnlistingInProject IEnlistingInProject;
#endif 	/* __IEnlistingInProject_FWD_DEFINED__ */


#ifndef __SVsSccManager_FWD_DEFINED__
#define __SVsSccManager_FWD_DEFINED__
typedef interface SVsSccManager SVsSccManager;
#endif 	/* __SVsSccManager_FWD_DEFINED__ */


/* header files for imported files */
#include "objidl.h"
#include "IVsSccProject2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_IVsSccManager2_0000_0000 */
/* [local] */ 

#if     _MSC_VER > 1000
#pragma once
#endif
#pragma once


extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0000_v0_0_s_ifspec;

#ifndef __IVsSccManager2_INTERFACE_DEFINED__
#define __IVsSccManager2_INTERFACE_DEFINED__

/* interface IVsSccManager2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSccManager2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-B927-4320-B9DA-13D2CB3EA93B")
    IVsSccManager2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterSccProject( 
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterSccProject( 
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccGlyph( 
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullPaths[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VsStateIcon rgsiGlyphs[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgdwSccStatus[  ]) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSccGlyphFromStatus( 
            /* [in] */ DWORD dwSccStatus,
            /* [retval][out] */ __RPC__out VsStateIcon *psiGlyph) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInstalled( 
            /* [retval][out] */ __RPC__out BOOL *pbInstalled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BrowseForProject( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDirectory,
            /* [out] */ __RPC__out BOOL *pfOK) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CancelAfterBrowseForProject( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsSccManager2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsSccManager2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsSccManager2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsSccManager2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterSccProject )( 
            IVsSccManager2 * This,
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project,
            /* [in] */ __RPC__in LPCOLESTR pszSccProjectName,
            /* [in] */ __RPC__in LPCOLESTR pszSccAuxPath,
            /* [in] */ __RPC__in LPCOLESTR pszSccLocalPath,
            /* [in] */ __RPC__in LPCOLESTR pszProvider);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterSccProject )( 
            IVsSccManager2 * This,
            /* [in] */ __RPC__in_opt IVsSccProject2 *pscp2Project);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccGlyph )( 
            IVsSccManager2 * This,
            /* [in] */ int cFiles,
            /* [size_is][in] */ __RPC__in_ecount_full(cFiles) const LPCOLESTR rgpszFullPaths[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) VsStateIcon rgsiGlyphs[  ],
            /* [size_is][out] */ __RPC__out_ecount_full(cFiles) DWORD rgdwSccStatus[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GetSccGlyphFromStatus )( 
            IVsSccManager2 * This,
            /* [in] */ DWORD dwSccStatus,
            /* [retval][out] */ __RPC__out VsStateIcon *psiGlyph);
        
        HRESULT ( STDMETHODCALLTYPE *IsInstalled )( 
            IVsSccManager2 * This,
            /* [retval][out] */ __RPC__out BOOL *pbInstalled);
        
        HRESULT ( STDMETHODCALLTYPE *BrowseForProject )( 
            IVsSccManager2 * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrDirectory,
            /* [out] */ __RPC__out BOOL *pfOK);
        
        HRESULT ( STDMETHODCALLTYPE *CancelAfterBrowseForProject )( 
            IVsSccManager2 * This);
        
        END_INTERFACE
    } IVsSccManager2Vtbl;

    interface IVsSccManager2
    {
        CONST_VTBL struct IVsSccManager2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSccManager2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSccManager2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSccManager2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSccManager2_RegisterSccProject(This,pscp2Project,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider)	\
    ( (This)->lpVtbl -> RegisterSccProject(This,pscp2Project,pszSccProjectName,pszSccAuxPath,pszSccLocalPath,pszProvider) ) 

#define IVsSccManager2_UnregisterSccProject(This,pscp2Project)	\
    ( (This)->lpVtbl -> UnregisterSccProject(This,pscp2Project) ) 

#define IVsSccManager2_GetSccGlyph(This,cFiles,rgpszFullPaths,rgsiGlyphs,rgdwSccStatus)	\
    ( (This)->lpVtbl -> GetSccGlyph(This,cFiles,rgpszFullPaths,rgsiGlyphs,rgdwSccStatus) ) 

#define IVsSccManager2_GetSccGlyphFromStatus(This,dwSccStatus,psiGlyph)	\
    ( (This)->lpVtbl -> GetSccGlyphFromStatus(This,dwSccStatus,psiGlyph) ) 

#define IVsSccManager2_IsInstalled(This,pbInstalled)	\
    ( (This)->lpVtbl -> IsInstalled(This,pbInstalled) ) 

#define IVsSccManager2_BrowseForProject(This,pbstrDirectory,pfOK)	\
    ( (This)->lpVtbl -> BrowseForProject(This,pbstrDirectory,pfOK) ) 

#define IVsSccManager2_CancelAfterBrowseForProject(This)	\
    ( (This)->lpVtbl -> CancelAfterBrowseForProject(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSccManager2_INTERFACE_DEFINED__ */


#ifndef __ISccManagerLoaded_INTERFACE_DEFINED__
#define __ISccManagerLoaded_INTERFACE_DEFINED__

/* interface ISccManagerLoaded */
/* [object][uuid] */ 


EXTERN_C const IID IID_ISccManagerLoaded;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("795635A0-4522-11d1-8DCE-00AA00A3F593")
    ISccManagerLoaded : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct ISccManagerLoadedVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISccManagerLoaded * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISccManagerLoaded * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISccManagerLoaded * This);
        
        END_INTERFACE
    } ISccManagerLoadedVtbl;

    interface ISccManagerLoaded
    {
        CONST_VTBL struct ISccManagerLoadedVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISccManagerLoaded_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISccManagerLoaded_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISccManagerLoaded_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISccManagerLoaded_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsSccManager2_0000_0002 */
/* [local] */ 

#define UICONTEXT_SccManagerLoaded IID_ISccManagerLoaded


extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0002_v0_0_s_ifspec;

#ifndef __IEnlistingInProject_INTERFACE_DEFINED__
#define __IEnlistingInProject_INTERFACE_DEFINED__

/* interface IEnlistingInProject */
/* [object][uuid] */ 


EXTERN_C const IID IID_IEnlistingInProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F59FD070-0063-4256-8212-E5DB6FC22253")
    IEnlistingInProject : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IEnlistingInProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnlistingInProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnlistingInProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnlistingInProject * This);
        
        END_INTERFACE
    } IEnlistingInProjectVtbl;

    interface IEnlistingInProject
    {
        CONST_VTBL struct IEnlistingInProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnlistingInProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnlistingInProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnlistingInProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnlistingInProject_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsSccManager2_0000_0003 */
/* [local] */ 

#define UICONTEXT_EnlistingInProject IID_IEnlistingInProject


extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0003_v0_0_s_ifspec;

#ifndef __SVsSccManager_INTERFACE_DEFINED__
#define __SVsSccManager_INTERFACE_DEFINED__

/* interface SVsSccManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsSccManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("53544C4D-1927-4320-B9DA-13D2CB3EA93B")
    SVsSccManager : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsSccManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsSccManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsSccManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsSccManager * This);
        
        END_INTERFACE
    } SVsSccManagerVtbl;

    interface SVsSccManager
    {
        CONST_VTBL struct SVsSccManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsSccManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsSccManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsSccManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsSccManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_IVsSccManager2_0000_0004 */
/* [local] */ 

#define SID_SVsSccManager IID_SVsSccManager


extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_IVsSccManager2_0000_0004_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


