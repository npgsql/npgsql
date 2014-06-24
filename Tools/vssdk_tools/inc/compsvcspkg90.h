

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for compsvcspkg90.idl:
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

#ifndef __compsvcspkg90_h__
#define __compsvcspkg90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IEnumTargetFrameworks_FWD_DEFINED__
#define __IEnumTargetFrameworks_FWD_DEFINED__
typedef interface IEnumTargetFrameworks IEnumTargetFrameworks;
#endif 	/* __IEnumTargetFrameworks_FWD_DEFINED__ */


#ifndef __IEnumSystemAssemblies_FWD_DEFINED__
#define __IEnumSystemAssemblies_FWD_DEFINED__
typedef interface IEnumSystemAssemblies IEnumSystemAssemblies;
#endif 	/* __IEnumSystemAssemblies_FWD_DEFINED__ */


#ifndef __IVsTargetFrameworkAssemblies_FWD_DEFINED__
#define __IVsTargetFrameworkAssemblies_FWD_DEFINED__
typedef interface IVsTargetFrameworkAssemblies IVsTargetFrameworkAssemblies;
#endif 	/* __IVsTargetFrameworkAssemblies_FWD_DEFINED__ */


#ifndef __SVsTargetFrameworkAssemblies_FWD_DEFINED__
#define __SVsTargetFrameworkAssemblies_FWD_DEFINED__
typedef interface SVsTargetFrameworkAssemblies SVsTargetFrameworkAssemblies;
#endif 	/* __SVsTargetFrameworkAssemblies_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "vsshell90.h"
#include "compsvcspkg80.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IEnumTargetFrameworks_INTERFACE_DEFINED__
#define __IEnumTargetFrameworks_INTERFACE_DEFINED__

/* interface IEnumTargetFrameworks */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IEnumTargetFrameworks;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D92C0B96-E08D-4268-A941-6D7D670F1820")
    IEnumTargetFrameworks : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) TARGETFRAMEWORKVERSION *rgFrameworks,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Count( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumTargetFrameworks **ppIEnumComponents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumTargetFrameworksVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumTargetFrameworks * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumTargetFrameworks * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumTargetFrameworks * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumTargetFrameworks * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) TARGETFRAMEWORKVERSION *rgFrameworks,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Count )( 
            IEnumTargetFrameworks * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumTargetFrameworks * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumTargetFrameworks * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumTargetFrameworks * This,
            /* [out] */ __RPC__deref_out_opt IEnumTargetFrameworks **ppIEnumComponents);
        
        END_INTERFACE
    } IEnumTargetFrameworksVtbl;

    interface IEnumTargetFrameworks
    {
        CONST_VTBL struct IEnumTargetFrameworksVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumTargetFrameworks_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumTargetFrameworks_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumTargetFrameworks_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumTargetFrameworks_Next(This,celt,rgFrameworks,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgFrameworks,pceltFetched) ) 

#define IEnumTargetFrameworks_Count(This,pCount)	\
    ( (This)->lpVtbl -> Count(This,pCount) ) 

#define IEnumTargetFrameworks_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumTargetFrameworks_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumTargetFrameworks_Clone(This,ppIEnumComponents)	\
    ( (This)->lpVtbl -> Clone(This,ppIEnumComponents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumTargetFrameworks_INTERFACE_DEFINED__ */


#ifndef __IEnumSystemAssemblies_INTERFACE_DEFINED__
#define __IEnumSystemAssemblies_INTERFACE_DEFINED__

/* interface IEnumSystemAssemblies */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IEnumSystemAssemblies;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7431FCE8-8E4F-49b6-BB50-E295636CBA6B")
    IEnumSystemAssemblies : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgAssemblies,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Count( 
            /* [out] */ __RPC__out ULONG *pCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IEnumSystemAssemblies **ppIEnumComponents) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumSystemAssembliesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumSystemAssemblies * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumSystemAssemblies * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumSystemAssemblies * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumSystemAssemblies * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgAssemblies,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Count )( 
            IEnumSystemAssemblies * This,
            /* [out] */ __RPC__out ULONG *pCount);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumSystemAssemblies * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumSystemAssemblies * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumSystemAssemblies * This,
            /* [out] */ __RPC__deref_out_opt IEnumSystemAssemblies **ppIEnumComponents);
        
        END_INTERFACE
    } IEnumSystemAssembliesVtbl;

    interface IEnumSystemAssemblies
    {
        CONST_VTBL struct IEnumSystemAssembliesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumSystemAssemblies_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumSystemAssemblies_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumSystemAssemblies_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumSystemAssemblies_Next(This,celt,rgAssemblies,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgAssemblies,pceltFetched) ) 

#define IEnumSystemAssemblies_Count(This,pCount)	\
    ( (This)->lpVtbl -> Count(This,pCount) ) 

#define IEnumSystemAssemblies_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumSystemAssemblies_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumSystemAssemblies_Clone(This,ppIEnumComponents)	\
    ( (This)->lpVtbl -> Clone(This,ppIEnumComponents) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumSystemAssemblies_INTERFACE_DEFINED__ */


#ifndef __IVsTargetFrameworkAssemblies_INTERFACE_DEFINED__
#define __IVsTargetFrameworkAssemblies_INTERFACE_DEFINED__

/* interface IVsTargetFrameworkAssemblies */
/* [object][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_IVsTargetFrameworkAssemblies;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("399DC6D4-84D6-4208-AFA6-362098E7972F")
    IVsTargetFrameworkAssemblies : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSupportedFrameworks( 
            /* [out] */ __RPC__deref_out_opt IEnumTargetFrameworks **pTargetFrameworks) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTargetFrameworkDescription( 
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion,
            /* [out] */ __RPC__deref_out_opt BSTR *pszDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSystemAssemblies( 
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion,
            /* [out] */ __RPC__deref_out_opt IEnumSystemAssemblies **pAssemblies) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsSystemAssembly( 
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out BOOL *pIsSystem,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRequiredTargetFrameworkVersion( 
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRequiredTargetFrameworkVersionFromDependency( 
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTargetFrameworkAssembliesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTargetFrameworkAssemblies * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTargetFrameworkAssemblies * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedFrameworks )( 
            IVsTargetFrameworkAssemblies * This,
            /* [out] */ __RPC__deref_out_opt IEnumTargetFrameworks **pTargetFrameworks);
        
        HRESULT ( STDMETHODCALLTYPE *GetTargetFrameworkDescription )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion,
            /* [out] */ __RPC__deref_out_opt BSTR *pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *GetSystemAssemblies )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ TARGETFRAMEWORKVERSION targetVersion,
            /* [out] */ __RPC__deref_out_opt IEnumSystemAssemblies **pAssemblies);
        
        HRESULT ( STDMETHODCALLTYPE *IsSystemAssembly )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out BOOL *pIsSystem,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequiredTargetFrameworkVersion )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion);
        
        HRESULT ( STDMETHODCALLTYPE *GetRequiredTargetFrameworkVersionFromDependency )( 
            IVsTargetFrameworkAssemblies * This,
            /* [in] */ __RPC__in LPCOLESTR szAssemblyFile,
            /* [out] */ __RPC__out TARGETFRAMEWORKVERSION *pTargetFrameworkVersion);
        
        END_INTERFACE
    } IVsTargetFrameworkAssembliesVtbl;

    interface IVsTargetFrameworkAssemblies
    {
        CONST_VTBL struct IVsTargetFrameworkAssembliesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTargetFrameworkAssemblies_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTargetFrameworkAssemblies_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTargetFrameworkAssemblies_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTargetFrameworkAssemblies_GetSupportedFrameworks(This,pTargetFrameworks)	\
    ( (This)->lpVtbl -> GetSupportedFrameworks(This,pTargetFrameworks) ) 

#define IVsTargetFrameworkAssemblies_GetTargetFrameworkDescription(This,targetVersion,pszDescription)	\
    ( (This)->lpVtbl -> GetTargetFrameworkDescription(This,targetVersion,pszDescription) ) 

#define IVsTargetFrameworkAssemblies_GetSystemAssemblies(This,targetVersion,pAssemblies)	\
    ( (This)->lpVtbl -> GetSystemAssemblies(This,targetVersion,pAssemblies) ) 

#define IVsTargetFrameworkAssemblies_IsSystemAssembly(This,szAssemblyFile,pIsSystem,pTargetFrameworkVersion)	\
    ( (This)->lpVtbl -> IsSystemAssembly(This,szAssemblyFile,pIsSystem,pTargetFrameworkVersion) ) 

#define IVsTargetFrameworkAssemblies_GetRequiredTargetFrameworkVersion(This,szAssemblyFile,pTargetFrameworkVersion)	\
    ( (This)->lpVtbl -> GetRequiredTargetFrameworkVersion(This,szAssemblyFile,pTargetFrameworkVersion) ) 

#define IVsTargetFrameworkAssemblies_GetRequiredTargetFrameworkVersionFromDependency(This,szAssemblyFile,pTargetFrameworkVersion)	\
    ( (This)->lpVtbl -> GetRequiredTargetFrameworkVersionFromDependency(This,szAssemblyFile,pTargetFrameworkVersion) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTargetFrameworkAssemblies_INTERFACE_DEFINED__ */


#ifndef __SVsTargetFrameworkAssemblies_INTERFACE_DEFINED__
#define __SVsTargetFrameworkAssemblies_INTERFACE_DEFINED__

/* interface SVsTargetFrameworkAssemblies */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsTargetFrameworkAssemblies;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C269ADA8-95F9-4987-A247-151FB2DDFB34")
    SVsTargetFrameworkAssemblies : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsTargetFrameworkAssembliesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsTargetFrameworkAssemblies * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsTargetFrameworkAssemblies * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsTargetFrameworkAssemblies * This);
        
        END_INTERFACE
    } SVsTargetFrameworkAssembliesVtbl;

    interface SVsTargetFrameworkAssemblies
    {
        CONST_VTBL struct SVsTargetFrameworkAssembliesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsTargetFrameworkAssemblies_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsTargetFrameworkAssemblies_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsTargetFrameworkAssemblies_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsTargetFrameworkAssemblies_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_compsvcspkg90_0000_0004 */
/* [local] */ 

#define SID_SVsTargetFrameworkAssemblies IID_SVsTargetFrameworkAssemblies


extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg90_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_compsvcspkg90_0000_0004_v0_0_s_ifspec;

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


