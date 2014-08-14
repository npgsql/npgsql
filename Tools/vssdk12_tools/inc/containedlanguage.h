

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

#ifndef __containedlanguage_h__
#define __containedlanguage_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsIntellisenseProjectManager_FWD_DEFINED__
#define __IVsIntellisenseProjectManager_FWD_DEFINED__
typedef interface IVsIntellisenseProjectManager IVsIntellisenseProjectManager;

#endif 	/* __IVsIntellisenseProjectManager_FWD_DEFINED__ */


#ifndef __SVsIntellisenseProjectManager_FWD_DEFINED__
#define __SVsIntellisenseProjectManager_FWD_DEFINED__
typedef interface SVsIntellisenseProjectManager SVsIntellisenseProjectManager;

#endif 	/* __SVsIntellisenseProjectManager_FWD_DEFINED__ */


#ifndef __IVsIntellisenseEngine_FWD_DEFINED__
#define __IVsIntellisenseEngine_FWD_DEFINED__
typedef interface IVsIntellisenseEngine IVsIntellisenseEngine;

#endif 	/* __IVsIntellisenseEngine_FWD_DEFINED__ */


#ifndef __SVsIntellisenseEngine_FWD_DEFINED__
#define __SVsIntellisenseEngine_FWD_DEFINED__
typedef interface SVsIntellisenseEngine SVsIntellisenseEngine;

#endif 	/* __SVsIntellisenseEngine_FWD_DEFINED__ */


#ifndef __IVsIntellisenseProjectEventSink_FWD_DEFINED__
#define __IVsIntellisenseProjectEventSink_FWD_DEFINED__
typedef interface IVsIntellisenseProjectEventSink IVsIntellisenseProjectEventSink;

#endif 	/* __IVsIntellisenseProjectEventSink_FWD_DEFINED__ */


#ifndef __IVsItemTypeResolutionService_FWD_DEFINED__
#define __IVsItemTypeResolutionService_FWD_DEFINED__
typedef interface IVsItemTypeResolutionService IVsItemTypeResolutionService;

#endif 	/* __IVsItemTypeResolutionService_FWD_DEFINED__ */


#ifndef __IVsItemTypeResolutionSite_FWD_DEFINED__
#define __IVsItemTypeResolutionSite_FWD_DEFINED__
typedef interface IVsItemTypeResolutionSite IVsItemTypeResolutionSite;

#endif 	/* __IVsItemTypeResolutionSite_FWD_DEFINED__ */


#ifndef __IVsIntellisenseProjectHost_FWD_DEFINED__
#define __IVsIntellisenseProjectHost_FWD_DEFINED__
typedef interface IVsIntellisenseProjectHost IVsIntellisenseProjectHost;

#endif 	/* __IVsIntellisenseProjectHost_FWD_DEFINED__ */


#ifndef __SVsIntellisenseProjectHost_FWD_DEFINED__
#define __SVsIntellisenseProjectHost_FWD_DEFINED__
typedef interface SVsIntellisenseProjectHost SVsIntellisenseProjectHost;

#endif 	/* __SVsIntellisenseProjectHost_FWD_DEFINED__ */


#ifndef __IVsIntellisenseProject_FWD_DEFINED__
#define __IVsIntellisenseProject_FWD_DEFINED__
typedef interface IVsIntellisenseProject IVsIntellisenseProject;

#endif 	/* __IVsIntellisenseProject_FWD_DEFINED__ */


#ifndef __IVsDataEnvironment_FWD_DEFINED__
#define __IVsDataEnvironment_FWD_DEFINED__
typedef interface IVsDataEnvironment IVsDataEnvironment;

#endif 	/* __IVsDataEnvironment_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "oleipc.h"
#include "vsshell.h"
#include "singlefileeditor.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_containedlanguage_0000_0000 */
/* [local] */ 

#pragma once







enum INTELLIPROJSTATUS
    {
        INTELLIPROJSTATUS_LOADING	= 1,
        INTELLIPROJSTATUS_LOADCOMPLETE	= 2,
        INTELLIPROJSTATUS_CLOSE	= 3,
        INTELLIPROJSTATUS_REFRESH	= 4
    } ;

enum HOSTPROPID
    {
        HOSTPROPID_PROJECTNAME	= 1,
        HOSTPROPID_HIERARCHY	= 2,
        HOSTPROPID_RELURL	= 3,
        HOSTPROPID_INTELLISENSECACHE_FILENAME	= 4
    } ;

enum REFERENCECHANGETYPE
    {
        REFERENCE_Added	= 1,
        REFERENCE_Removed	= 2,
        REFERENCE_Changed	= 3
    } ;


extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0000_v0_0_s_ifspec;

#ifndef __IVsIntellisenseProjectManager_INTERFACE_DEFINED__
#define __IVsIntellisenseProjectManager_INTERFACE_DEFINED__

/* interface IVsIntellisenseProjectManager */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVsIntellisenseProjectManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B299765F-1FC2-41a7-BEC1-64721D86E658")
    IVsIntellisenseProjectManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AdviseIntellisenseProjectEvents( 
            /* [in] */ __RPC__in_opt IVsIntellisenseProjectEventSink *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseIntellisenseProjectEvents( 
            /* [in] */ VSCOOKIE dwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContainedLanguageFactory( 
            /* [in] */ __RPC__in BSTR bstrLanguage,
            /* [out] */ __RPC__deref_out_opt IVsContainedLanguageFactory **ppContainedLanguageFactory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseIntellisenseProject( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnEditorReady( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CompleteIntellisenseProjectLoad( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsIntellisenseProjectManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsIntellisenseProjectManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsIntellisenseProjectManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsIntellisenseProjectManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseIntellisenseProjectEvents )( 
            __RPC__in IVsIntellisenseProjectManager * This,
            /* [in] */ __RPC__in_opt IVsIntellisenseProjectEventSink *pSink,
            /* [out] */ __RPC__out VSCOOKIE *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseIntellisenseProjectEvents )( 
            __RPC__in IVsIntellisenseProjectManager * This,
            /* [in] */ VSCOOKIE dwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *GetContainedLanguageFactory )( 
            __RPC__in IVsIntellisenseProjectManager * This,
            /* [in] */ __RPC__in BSTR bstrLanguage,
            /* [out] */ __RPC__deref_out_opt IVsContainedLanguageFactory **ppContainedLanguageFactory);
        
        HRESULT ( STDMETHODCALLTYPE *CloseIntellisenseProject )( 
            __RPC__in IVsIntellisenseProjectManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnEditorReady )( 
            __RPC__in IVsIntellisenseProjectManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *CompleteIntellisenseProjectLoad )( 
            __RPC__in IVsIntellisenseProjectManager * This);
        
        END_INTERFACE
    } IVsIntellisenseProjectManagerVtbl;

    interface IVsIntellisenseProjectManager
    {
        CONST_VTBL struct IVsIntellisenseProjectManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsIntellisenseProjectManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsIntellisenseProjectManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsIntellisenseProjectManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsIntellisenseProjectManager_AdviseIntellisenseProjectEvents(This,pSink,pdwCookie)	\
    ( (This)->lpVtbl -> AdviseIntellisenseProjectEvents(This,pSink,pdwCookie) ) 

#define IVsIntellisenseProjectManager_UnadviseIntellisenseProjectEvents(This,dwCookie)	\
    ( (This)->lpVtbl -> UnadviseIntellisenseProjectEvents(This,dwCookie) ) 

#define IVsIntellisenseProjectManager_GetContainedLanguageFactory(This,bstrLanguage,ppContainedLanguageFactory)	\
    ( (This)->lpVtbl -> GetContainedLanguageFactory(This,bstrLanguage,ppContainedLanguageFactory) ) 

#define IVsIntellisenseProjectManager_CloseIntellisenseProject(This)	\
    ( (This)->lpVtbl -> CloseIntellisenseProject(This) ) 

#define IVsIntellisenseProjectManager_OnEditorReady(This)	\
    ( (This)->lpVtbl -> OnEditorReady(This) ) 

#define IVsIntellisenseProjectManager_CompleteIntellisenseProjectLoad(This)	\
    ( (This)->lpVtbl -> CompleteIntellisenseProjectLoad(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsIntellisenseProjectManager_INTERFACE_DEFINED__ */


#ifndef __SVsIntellisenseProjectManager_INTERFACE_DEFINED__
#define __SVsIntellisenseProjectManager_INTERFACE_DEFINED__

/* interface SVsIntellisenseProjectManager */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsIntellisenseProjectManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5CB6B09C-64F5-4579-8593-7720DAB2EF8D")
    SVsIntellisenseProjectManager : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsIntellisenseProjectManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsIntellisenseProjectManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsIntellisenseProjectManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsIntellisenseProjectManager * This);
        
        END_INTERFACE
    } SVsIntellisenseProjectManagerVtbl;

    interface SVsIntellisenseProjectManager
    {
        CONST_VTBL struct SVsIntellisenseProjectManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsIntellisenseProjectManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsIntellisenseProjectManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsIntellisenseProjectManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsIntellisenseProjectManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_containedlanguage_0000_0002 */
/* [local] */ 

#define SID_SVsIntellisenseProjectManager IID_SVsIntellisenseProjectManager


extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0002_v0_0_s_ifspec;

#ifndef __IVsIntellisenseEngine_INTERFACE_DEFINED__
#define __IVsIntellisenseEngine_INTERFACE_DEFINED__

/* interface IVsIntellisenseEngine */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVsIntellisenseEngine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4371079A-47C3-4b7a-93AE-BFB90FEDB8F0")
    IVsIntellisenseEngine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Load( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Unload( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SupportsLoad( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsIntellisenseEngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsIntellisenseEngine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsIntellisenseEngine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsIntellisenseEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *Load )( 
            __RPC__in IVsIntellisenseEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *Unload )( 
            __RPC__in IVsIntellisenseEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *SupportsLoad )( 
            __RPC__in IVsIntellisenseEngine * This);
        
        END_INTERFACE
    } IVsIntellisenseEngineVtbl;

    interface IVsIntellisenseEngine
    {
        CONST_VTBL struct IVsIntellisenseEngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsIntellisenseEngine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsIntellisenseEngine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsIntellisenseEngine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsIntellisenseEngine_Load(This)	\
    ( (This)->lpVtbl -> Load(This) ) 

#define IVsIntellisenseEngine_Unload(This)	\
    ( (This)->lpVtbl -> Unload(This) ) 

#define IVsIntellisenseEngine_SupportsLoad(This)	\
    ( (This)->lpVtbl -> SupportsLoad(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsIntellisenseEngine_INTERFACE_DEFINED__ */


#ifndef __SVsIntellisenseEngine_INTERFACE_DEFINED__
#define __SVsIntellisenseEngine_INTERFACE_DEFINED__

/* interface SVsIntellisenseEngine */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsIntellisenseEngine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D380C630-E46F-40e1-B71D-F3D1682C5AA8")
    SVsIntellisenseEngine : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsIntellisenseEngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsIntellisenseEngine * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsIntellisenseEngine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsIntellisenseEngine * This);
        
        END_INTERFACE
    } SVsIntellisenseEngineVtbl;

    interface SVsIntellisenseEngine
    {
        CONST_VTBL struct SVsIntellisenseEngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsIntellisenseEngine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsIntellisenseEngine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsIntellisenseEngine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsIntellisenseEngine_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_containedlanguage_0000_0004 */
/* [local] */ 

#define SID_SVsIntellisenseEngine IID_SVsIntellisenseEngine


extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0004_v0_0_s_ifspec;

#ifndef __IVsIntellisenseProjectEventSink_INTERFACE_DEFINED__
#define __IVsIntellisenseProjectEventSink_INTERFACE_DEFINED__

/* interface IVsIntellisenseProjectEventSink */
/* [object][version][uuid] */ 


EXTERN_C const IID IID_IVsIntellisenseProjectEventSink;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DFB5C0C2-817E-4a19-8C6D-E387FD68B50B")
    IVsIntellisenseProjectEventSink : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnStatusChange( 
            /* [in] */ DWORD dwStatus) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnReferenceChange( 
            /* [in] */ DWORD dwChangeType,
            __RPC__in LPCWSTR pszAssemblyPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnConfigChange( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnCodeFileChange( 
            /* [in] */ __RPC__in LPCWSTR pszOldCodeFile,
            /* [in] */ __RPC__in LPCWSTR pszNewCodeFile) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsIntellisenseProjectEventSinkVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsIntellisenseProjectEventSink * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsIntellisenseProjectEventSink * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsIntellisenseProjectEventSink * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnStatusChange )( 
            __RPC__in IVsIntellisenseProjectEventSink * This,
            /* [in] */ DWORD dwStatus);
        
        HRESULT ( STDMETHODCALLTYPE *OnReferenceChange )( 
            __RPC__in IVsIntellisenseProjectEventSink * This,
            /* [in] */ DWORD dwChangeType,
            __RPC__in LPCWSTR pszAssemblyPath);
        
        HRESULT ( STDMETHODCALLTYPE *OnConfigChange )( 
            __RPC__in IVsIntellisenseProjectEventSink * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnCodeFileChange )( 
            __RPC__in IVsIntellisenseProjectEventSink * This,
            /* [in] */ __RPC__in LPCWSTR pszOldCodeFile,
            /* [in] */ __RPC__in LPCWSTR pszNewCodeFile);
        
        END_INTERFACE
    } IVsIntellisenseProjectEventSinkVtbl;

    interface IVsIntellisenseProjectEventSink
    {
        CONST_VTBL struct IVsIntellisenseProjectEventSinkVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsIntellisenseProjectEventSink_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsIntellisenseProjectEventSink_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsIntellisenseProjectEventSink_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsIntellisenseProjectEventSink_OnStatusChange(This,dwStatus)	\
    ( (This)->lpVtbl -> OnStatusChange(This,dwStatus) ) 

#define IVsIntellisenseProjectEventSink_OnReferenceChange(This,dwChangeType,pszAssemblyPath)	\
    ( (This)->lpVtbl -> OnReferenceChange(This,dwChangeType,pszAssemblyPath) ) 

#define IVsIntellisenseProjectEventSink_OnConfigChange(This)	\
    ( (This)->lpVtbl -> OnConfigChange(This) ) 

#define IVsIntellisenseProjectEventSink_OnCodeFileChange(This,pszOldCodeFile,pszNewCodeFile)	\
    ( (This)->lpVtbl -> OnCodeFileChange(This,pszOldCodeFile,pszNewCodeFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsIntellisenseProjectEventSink_INTERFACE_DEFINED__ */


#ifndef __IVsItemTypeResolutionService_INTERFACE_DEFINED__
#define __IVsItemTypeResolutionService_INTERFACE_DEFINED__

/* interface IVsItemTypeResolutionService */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVsItemTypeResolutionService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C6CF64F7-8863-4e50-9DF1-892AA83D70D7")
    IVsItemTypeResolutionService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetSite( 
            /* [in] */ __RPC__in_opt IUnknown *punkVsItemTypeResolutionSite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeReferences( 
            /* [in] */ __RPC__in_opt IUnknown *punkCompilerParameters) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReferenceAdded( 
            /* [in] */ __RPC__in BSTR pszReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReferenceRemoved( 
            /* [in] */ __RPC__in BSTR pszReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReferenceChanged( 
            /* [in] */ __RPC__in BSTR pszReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsItemTypeResolutionServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsItemTypeResolutionService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsItemTypeResolutionService * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetSite )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in_opt IUnknown *punkVsItemTypeResolutionSite);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeReferences )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in_opt IUnknown *punkCompilerParameters);
        
        HRESULT ( STDMETHODCALLTYPE *ReferenceAdded )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in BSTR pszReference);
        
        HRESULT ( STDMETHODCALLTYPE *ReferenceRemoved )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in BSTR pszReference);
        
        HRESULT ( STDMETHODCALLTYPE *ReferenceChanged )( 
            __RPC__in IVsItemTypeResolutionService * This,
            /* [in] */ __RPC__in BSTR pszReference);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            __RPC__in IVsItemTypeResolutionService * This);
        
        END_INTERFACE
    } IVsItemTypeResolutionServiceVtbl;

    interface IVsItemTypeResolutionService
    {
        CONST_VTBL struct IVsItemTypeResolutionServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsItemTypeResolutionService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsItemTypeResolutionService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsItemTypeResolutionService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsItemTypeResolutionService_SetSite(This,punkVsItemTypeResolutionSite)	\
    ( (This)->lpVtbl -> SetSite(This,punkVsItemTypeResolutionSite) ) 

#define IVsItemTypeResolutionService_InitializeReferences(This,punkCompilerParameters)	\
    ( (This)->lpVtbl -> InitializeReferences(This,punkCompilerParameters) ) 

#define IVsItemTypeResolutionService_ReferenceAdded(This,pszReference)	\
    ( (This)->lpVtbl -> ReferenceAdded(This,pszReference) ) 

#define IVsItemTypeResolutionService_ReferenceRemoved(This,pszReference)	\
    ( (This)->lpVtbl -> ReferenceRemoved(This,pszReference) ) 

#define IVsItemTypeResolutionService_ReferenceChanged(This,pszReference)	\
    ( (This)->lpVtbl -> ReferenceChanged(This,pszReference) ) 

#define IVsItemTypeResolutionService_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsItemTypeResolutionService_INTERFACE_DEFINED__ */


#ifndef __IVsItemTypeResolutionSite_INTERFACE_DEFINED__
#define __IVsItemTypeResolutionSite_INTERFACE_DEFINED__

/* interface IVsItemTypeResolutionSite */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVsItemTypeResolutionSite;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("45ABED49-8D6E-47b8-A0D6-C9F2405817C6")
    IVsItemTypeResolutionSite : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddReference( 
            /* [in] */ __RPC__in BSTR bstrReferencePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitForReferencesReady( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsCodeDirectoryAssembly( 
            /* [in] */ __RPC__in BSTR bstrAssembly,
            /* [out] */ __RPC__out BOOL *pfIsCodeAssembly) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsItemTypeResolutionSiteVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsItemTypeResolutionSite * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsItemTypeResolutionSite * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsItemTypeResolutionSite * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddReference )( 
            __RPC__in IVsItemTypeResolutionSite * This,
            /* [in] */ __RPC__in BSTR bstrReferencePath);
        
        HRESULT ( STDMETHODCALLTYPE *WaitForReferencesReady )( 
            __RPC__in IVsItemTypeResolutionSite * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsCodeDirectoryAssembly )( 
            __RPC__in IVsItemTypeResolutionSite * This,
            /* [in] */ __RPC__in BSTR bstrAssembly,
            /* [out] */ __RPC__out BOOL *pfIsCodeAssembly);
        
        END_INTERFACE
    } IVsItemTypeResolutionSiteVtbl;

    interface IVsItemTypeResolutionSite
    {
        CONST_VTBL struct IVsItemTypeResolutionSiteVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsItemTypeResolutionSite_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsItemTypeResolutionSite_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsItemTypeResolutionSite_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsItemTypeResolutionSite_AddReference(This,bstrReferencePath)	\
    ( (This)->lpVtbl -> AddReference(This,bstrReferencePath) ) 

#define IVsItemTypeResolutionSite_WaitForReferencesReady(This)	\
    ( (This)->lpVtbl -> WaitForReferencesReady(This) ) 

#define IVsItemTypeResolutionSite_IsCodeDirectoryAssembly(This,bstrAssembly,pfIsCodeAssembly)	\
    ( (This)->lpVtbl -> IsCodeDirectoryAssembly(This,bstrAssembly,pfIsCodeAssembly) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsItemTypeResolutionSite_INTERFACE_DEFINED__ */


#ifndef __IVsIntellisenseProjectHost_INTERFACE_DEFINED__
#define __IVsIntellisenseProjectHost_INTERFACE_DEFINED__

/* interface IVsIntellisenseProjectHost */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVsIntellisenseProjectHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6C62F743-D2D3-46B3-BFBC-F04B54EE3f79")
    IVsIntellisenseProjectHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHostProperty( 
            /* [in] */ DWORD dwPropID,
            /* [retval][out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCompilerOptions( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOutputAssembly( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOutputAssembly) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateFileCodeModel( 
            /* [in] */ __RPC__in LPCWSTR pszFilename,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsIntellisenseProjectHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsIntellisenseProjectHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsIntellisenseProjectHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsIntellisenseProjectHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHostProperty )( 
            __RPC__in IVsIntellisenseProjectHost * This,
            /* [in] */ DWORD dwPropID,
            /* [retval][out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompilerOptions )( 
            __RPC__in IVsIntellisenseProjectHost * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOptions);
        
        HRESULT ( STDMETHODCALLTYPE *GetOutputAssembly )( 
            __RPC__in IVsIntellisenseProjectHost * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrOutputAssembly);
        
        HRESULT ( STDMETHODCALLTYPE *CreateFileCodeModel )( 
            __RPC__in IVsIntellisenseProjectHost * This,
            /* [in] */ __RPC__in LPCWSTR pszFilename,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel);
        
        END_INTERFACE
    } IVsIntellisenseProjectHostVtbl;

    interface IVsIntellisenseProjectHost
    {
        CONST_VTBL struct IVsIntellisenseProjectHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsIntellisenseProjectHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsIntellisenseProjectHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsIntellisenseProjectHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsIntellisenseProjectHost_GetHostProperty(This,dwPropID,pvar)	\
    ( (This)->lpVtbl -> GetHostProperty(This,dwPropID,pvar) ) 

#define IVsIntellisenseProjectHost_GetCompilerOptions(This,pbstrOptions)	\
    ( (This)->lpVtbl -> GetCompilerOptions(This,pbstrOptions) ) 

#define IVsIntellisenseProjectHost_GetOutputAssembly(This,pbstrOutputAssembly)	\
    ( (This)->lpVtbl -> GetOutputAssembly(This,pbstrOutputAssembly) ) 

#define IVsIntellisenseProjectHost_CreateFileCodeModel(This,pszFilename,ppCodeModel)	\
    ( (This)->lpVtbl -> CreateFileCodeModel(This,pszFilename,ppCodeModel) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsIntellisenseProjectHost_INTERFACE_DEFINED__ */


#ifndef __SVsIntellisenseProjectHost_INTERFACE_DEFINED__
#define __SVsIntellisenseProjectHost_INTERFACE_DEFINED__

/* interface SVsIntellisenseProjectHost */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsIntellisenseProjectHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5D13ECE7-AA1F-400f-B65D-FA2ABD4F1CD4")
    SVsIntellisenseProjectHost : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct SVsIntellisenseProjectHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in SVsIntellisenseProjectHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in SVsIntellisenseProjectHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in SVsIntellisenseProjectHost * This);
        
        END_INTERFACE
    } SVsIntellisenseProjectHostVtbl;

    interface SVsIntellisenseProjectHost
    {
        CONST_VTBL struct SVsIntellisenseProjectHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsIntellisenseProjectHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsIntellisenseProjectHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsIntellisenseProjectHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsIntellisenseProjectHost_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_containedlanguage_0000_0009 */
/* [local] */ 

#define SID_SVsIntellisenseProjectHost IID_SVsIntellisenseProjectHost


extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_containedlanguage_0000_0009_v0_0_s_ifspec;

#ifndef __IVsIntellisenseProject_INTERFACE_DEFINED__
#define __IVsIntellisenseProject_INTERFACE_DEFINED__

/* interface IVsIntellisenseProject */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVsIntellisenseProject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3B83B579-4969-4E12-A964-11EC19CC1503")
    IVsIntellisenseProject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Init( 
            /* [in] */ __RPC__in_opt IVsIntellisenseProjectHost *pHost) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddFile( 
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ VSITEMID itemid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveFile( 
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ VSITEMID itemid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RenameFile( 
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ __RPC__in BSTR bstrNewAbsPath,
            /* [in] */ VSITEMID itemid) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsCompilableFile( 
            /* [in] */ __RPC__in BSTR bstrFileName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContainedLanguageFactory( 
            /* [retval][out] */ __RPC__deref_out_opt IVsContainedLanguageFactory **ppContainedLanguageFactory) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCompilerReference( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCompilerReference) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileCodeModel( 
            /* [in] */ __RPC__in_opt IUnknown *pProj,
            /* [in] */ __RPC__in_opt IUnknown *pProjectItem,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProjectCodeModel( 
            /* [in] */ __RPC__in_opt IUnknown *pProj,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RefreshCompilerOptions( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCodeDomProviderName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsWebFileRequiredByProject( 
            /* [retval][out] */ __RPC__out BOOL *pbReq) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddAssemblyReference( 
            /* [in] */ __RPC__in BSTR bstrAbsPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAssemblyReference( 
            /* [in] */ __RPC__in BSTR bstrAbsPath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddP2PReference( 
            /* [in] */ __RPC__in_opt IUnknown *pUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveP2PReference( 
            /* [in] */ __RPC__in_opt IUnknown *pUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopIntellisenseEngine( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartIntellisenseEngine( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsSupportedP2PReference( 
            /* [in] */ __RPC__in_opt IUnknown *pUnk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WaitForIntellisenseReady( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExternalErrorReporter( 
            /* [retval][out] */ __RPC__deref_out_opt IVsReportExternalErrors **ppErrorReporter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SuspendPostedNotifications( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResumePostedNotifications( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsIntellisenseProjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsIntellisenseProject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *Init )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IVsIntellisenseProjectHost *pHost);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddFile )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ VSITEMID itemid);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveFile )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ VSITEMID itemid);
        
        HRESULT ( STDMETHODCALLTYPE *RenameFile )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrAbsPath,
            /* [in] */ __RPC__in BSTR bstrNewAbsPath,
            /* [in] */ VSITEMID itemid);
        
        HRESULT ( STDMETHODCALLTYPE *IsCompilableFile )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrFileName);
        
        HRESULT ( STDMETHODCALLTYPE *GetContainedLanguageFactory )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsContainedLanguageFactory **ppContainedLanguageFactory);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompilerReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCompilerReference);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileCodeModel )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IUnknown *pProj,
            /* [in] */ __RPC__in_opt IUnknown *pProjectItem,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel);
        
        HRESULT ( STDMETHODCALLTYPE *GetProjectCodeModel )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IUnknown *pProj,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppCodeModel);
        
        HRESULT ( STDMETHODCALLTYPE *RefreshCompilerOptions )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCodeDomProviderName )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrProvider);
        
        HRESULT ( STDMETHODCALLTYPE *IsWebFileRequiredByProject )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [retval][out] */ __RPC__out BOOL *pbReq);
        
        HRESULT ( STDMETHODCALLTYPE *AddAssemblyReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrAbsPath);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAssemblyReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in BSTR bstrAbsPath);
        
        HRESULT ( STDMETHODCALLTYPE *AddP2PReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnk);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveP2PReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnk);
        
        HRESULT ( STDMETHODCALLTYPE *StopIntellisenseEngine )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartIntellisenseEngine )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *IsSupportedP2PReference )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [in] */ __RPC__in_opt IUnknown *pUnk);
        
        HRESULT ( STDMETHODCALLTYPE *WaitForIntellisenseReady )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExternalErrorReporter )( 
            __RPC__in IVsIntellisenseProject * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsReportExternalErrors **ppErrorReporter);
        
        HRESULT ( STDMETHODCALLTYPE *SuspendPostedNotifications )( 
            __RPC__in IVsIntellisenseProject * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResumePostedNotifications )( 
            __RPC__in IVsIntellisenseProject * This);
        
        END_INTERFACE
    } IVsIntellisenseProjectVtbl;

    interface IVsIntellisenseProject
    {
        CONST_VTBL struct IVsIntellisenseProjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsIntellisenseProject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsIntellisenseProject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsIntellisenseProject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsIntellisenseProject_Init(This,pHost)	\
    ( (This)->lpVtbl -> Init(This,pHost) ) 

#define IVsIntellisenseProject_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#define IVsIntellisenseProject_AddFile(This,bstrAbsPath,itemid)	\
    ( (This)->lpVtbl -> AddFile(This,bstrAbsPath,itemid) ) 

#define IVsIntellisenseProject_RemoveFile(This,bstrAbsPath,itemid)	\
    ( (This)->lpVtbl -> RemoveFile(This,bstrAbsPath,itemid) ) 

#define IVsIntellisenseProject_RenameFile(This,bstrAbsPath,bstrNewAbsPath,itemid)	\
    ( (This)->lpVtbl -> RenameFile(This,bstrAbsPath,bstrNewAbsPath,itemid) ) 

#define IVsIntellisenseProject_IsCompilableFile(This,bstrFileName)	\
    ( (This)->lpVtbl -> IsCompilableFile(This,bstrFileName) ) 

#define IVsIntellisenseProject_GetContainedLanguageFactory(This,ppContainedLanguageFactory)	\
    ( (This)->lpVtbl -> GetContainedLanguageFactory(This,ppContainedLanguageFactory) ) 

#define IVsIntellisenseProject_GetCompilerReference(This,ppCompilerReference)	\
    ( (This)->lpVtbl -> GetCompilerReference(This,ppCompilerReference) ) 

#define IVsIntellisenseProject_GetFileCodeModel(This,pProj,pProjectItem,ppCodeModel)	\
    ( (This)->lpVtbl -> GetFileCodeModel(This,pProj,pProjectItem,ppCodeModel) ) 

#define IVsIntellisenseProject_GetProjectCodeModel(This,pProj,ppCodeModel)	\
    ( (This)->lpVtbl -> GetProjectCodeModel(This,pProj,ppCodeModel) ) 

#define IVsIntellisenseProject_RefreshCompilerOptions(This)	\
    ( (This)->lpVtbl -> RefreshCompilerOptions(This) ) 

#define IVsIntellisenseProject_GetCodeDomProviderName(This,pbstrProvider)	\
    ( (This)->lpVtbl -> GetCodeDomProviderName(This,pbstrProvider) ) 

#define IVsIntellisenseProject_IsWebFileRequiredByProject(This,pbReq)	\
    ( (This)->lpVtbl -> IsWebFileRequiredByProject(This,pbReq) ) 

#define IVsIntellisenseProject_AddAssemblyReference(This,bstrAbsPath)	\
    ( (This)->lpVtbl -> AddAssemblyReference(This,bstrAbsPath) ) 

#define IVsIntellisenseProject_RemoveAssemblyReference(This,bstrAbsPath)	\
    ( (This)->lpVtbl -> RemoveAssemblyReference(This,bstrAbsPath) ) 

#define IVsIntellisenseProject_AddP2PReference(This,pUnk)	\
    ( (This)->lpVtbl -> AddP2PReference(This,pUnk) ) 

#define IVsIntellisenseProject_RemoveP2PReference(This,pUnk)	\
    ( (This)->lpVtbl -> RemoveP2PReference(This,pUnk) ) 

#define IVsIntellisenseProject_StopIntellisenseEngine(This)	\
    ( (This)->lpVtbl -> StopIntellisenseEngine(This) ) 

#define IVsIntellisenseProject_StartIntellisenseEngine(This)	\
    ( (This)->lpVtbl -> StartIntellisenseEngine(This) ) 

#define IVsIntellisenseProject_IsSupportedP2PReference(This,pUnk)	\
    ( (This)->lpVtbl -> IsSupportedP2PReference(This,pUnk) ) 

#define IVsIntellisenseProject_WaitForIntellisenseReady(This)	\
    ( (This)->lpVtbl -> WaitForIntellisenseReady(This) ) 

#define IVsIntellisenseProject_GetExternalErrorReporter(This,ppErrorReporter)	\
    ( (This)->lpVtbl -> GetExternalErrorReporter(This,ppErrorReporter) ) 

#define IVsIntellisenseProject_SuspendPostedNotifications(This)	\
    ( (This)->lpVtbl -> SuspendPostedNotifications(This) ) 

#define IVsIntellisenseProject_ResumePostedNotifications(This)	\
    ( (This)->lpVtbl -> ResumePostedNotifications(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsIntellisenseProject_INTERFACE_DEFINED__ */


#ifndef __IVsDataEnvironment_INTERFACE_DEFINED__
#define __IVsDataEnvironment_INTERFACE_DEFINED__

/* interface IVsDataEnvironment */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IVsDataEnvironment;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CD138AD4-A0BF-4681-8FA7-B6D57D55C4DB")
    IVsDataEnvironment : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ __RPC__in_opt IServiceProvider *pServiceProvider) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsDataEnvironmentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsDataEnvironment * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsDataEnvironment * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsDataEnvironment * This);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            __RPC__in IVsDataEnvironment * This,
            /* [in] */ __RPC__in_opt IServiceProvider *pServiceProvider);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            __RPC__in IVsDataEnvironment * This);
        
        END_INTERFACE
    } IVsDataEnvironmentVtbl;

    interface IVsDataEnvironment
    {
        CONST_VTBL struct IVsDataEnvironmentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDataEnvironment_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDataEnvironment_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDataEnvironment_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDataEnvironment_Initialize(This,pServiceProvider)	\
    ( (This)->lpVtbl -> Initialize(This,pServiceProvider) ) 

#define IVsDataEnvironment_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDataEnvironment_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     __RPC__in unsigned long *, __RPC__in VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


