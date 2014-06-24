

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


#ifndef __vslangproj110_h__
#define __vslangproj110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __References2_FWD_DEFINED__
#define __References2_FWD_DEFINED__
typedef interface References2 References2;

#endif 	/* __References2_FWD_DEFINED__ */


#ifndef __VBProjectProperties7_FWD_DEFINED__
#define __VBProjectProperties7_FWD_DEFINED__
typedef interface VBProjectProperties7 VBProjectProperties7;

#endif 	/* __VBProjectProperties7_FWD_DEFINED__ */


#ifndef __VBProjectConfigurationProperties6_FWD_DEFINED__
#define __VBProjectConfigurationProperties6_FWD_DEFINED__
typedef interface VBProjectConfigurationProperties6 VBProjectConfigurationProperties6;

#endif 	/* __VBProjectConfigurationProperties6_FWD_DEFINED__ */


#ifndef __CSharpProjectProperties7_FWD_DEFINED__
#define __CSharpProjectProperties7_FWD_DEFINED__
typedef interface CSharpProjectProperties7 CSharpProjectProperties7;

#endif 	/* __CSharpProjectProperties7_FWD_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties6_FWD_DEFINED__
#define __CSharpProjectConfigurationProperties6_FWD_DEFINED__
typedef interface CSharpProjectConfigurationProperties6 CSharpProjectConfigurationProperties6;

#endif 	/* __CSharpProjectConfigurationProperties6_FWD_DEFINED__ */


#ifndef __Reference5_FWD_DEFINED__
#define __Reference5_FWD_DEFINED__
typedef interface Reference5 Reference5;

#endif 	/* __Reference5_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_vslangproj110_0000_0000 */
/* [local] */ 

#define VSLANGPROJ110_VER_MAJ   11
#define VSLANGPROJ110_VER_MIN    0


extern RPC_IF_HANDLE __MIDL_itf_vslangproj110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj110_0000_0000_v0_0_s_ifspec;


#ifndef __VSLangProj110_LIBRARY_DEFINED__
#define __VSLangProj110_LIBRARY_DEFINED__

/* library VSLangProj110 */
/* [version][helpstring][uuid] */ 

#pragma once

enum __PROJECTREFERENCETYPE2
    {
        PROJREFTYPE_SDK	= 4
    } ;

enum VsProjPropId110
    {
        VBPROJPROPID_OutputTypeEx	= 17000,
        VBPROJPROPID_Prefer32Bit	= 17001
    } ;

enum VsProjReferencePropId110
    {
        DISPID_Reference_ExpandedSdkReferences	= 128,
        DISPID_Reference_Group	= 129
    } ;

enum prjOutputTypeEx
    {
        prjOutputTypeEx_WinExe	= 0,
        prjOutputTypeEx_Exe	= ( prjOutputTypeEx_WinExe + 1 ) ,
        prjOutputTypeEx_Library	= ( prjOutputTypeEx_Exe + 1 ) ,
        prjOutputTypeEx_WinMDObj	= ( prjOutputTypeEx_Library + 1 ) ,
        prjOutputTypeEx_AppContainerExe	= ( prjOutputTypeEx_WinMDObj + 1 ) 
    } ;
#define prjOutputTypeEx_Min prjOutputTypeEx_WinExe
#define prjOutputTypeEx_Max prjOutputTypeEx_AppContainerExe

EXTERN_C const IID LIBID_VSLangProj110;

#ifndef __References2_INTERFACE_DEFINED__
#define __References2_INTERFACE_DEFINED__

/* interface References2 */
/* [custom][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_References2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8BF64AF0-AD98-46D1-8E3E-A02AF56F80AE")
    References2 : public References
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE AddSDK( 
            /* [in] */ __RPC__in BSTR wszSDKDisplayName,
            /* [in] */ __RPC__in BSTR wszSDKidentifier,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Reference **ppProjectReference) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct References2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in References2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt Reference **retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            __RPC__in References2 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Find )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrIdentity,
            /* [retval][out] */ __RPC__deref_out_opt Reference **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrPath,
            /* [retval][out] */ __RPC__deref_out_opt Reference **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddActiveX )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR bstrTypeLibGuid,
            /* [in][idldescattr] */ signed long lMajorVer,
            /* [in][idldescattr] */ signed long lMinorVer,
            /* [in][idldescattr] */ signed long lLocaleId,
            /* [in][idldescattr] */ __RPC__in BSTR bstrWrapperTool,
            /* [retval][out] */ __RPC__deref_out_opt Reference **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddProject )( 
            __RPC__in References2 * This,
            /* [in][idldescattr] */ __RPC__in_opt Project *pProject,
            /* [retval][out] */ __RPC__deref_out_opt Reference **retval);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *AddSDK )( 
            __RPC__in References2 * This,
            /* [in] */ __RPC__in BSTR wszSDKDisplayName,
            /* [in] */ __RPC__in BSTR wszSDKidentifier,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Reference **ppProjectReference);
        
        END_INTERFACE
    } References2Vtbl;

    interface References2
    {
        CONST_VTBL struct References2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define References2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define References2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define References2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define References2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define References2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define References2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define References2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define References2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define References2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define References2_get_ContainingProject(This,retval)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,retval) ) 

#define References2_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define References2_Item(This,index,retval)	\
    ( (This)->lpVtbl -> Item(This,index,retval) ) 

#define References2__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define References2_Find(This,bstrIdentity,retval)	\
    ( (This)->lpVtbl -> Find(This,bstrIdentity,retval) ) 

#define References2_Add(This,bstrPath,retval)	\
    ( (This)->lpVtbl -> Add(This,bstrPath,retval) ) 

#define References2_AddActiveX(This,bstrTypeLibGuid,lMajorVer,lMinorVer,lLocaleId,bstrWrapperTool,retval)	\
    ( (This)->lpVtbl -> AddActiveX(This,bstrTypeLibGuid,lMajorVer,lMinorVer,lLocaleId,bstrWrapperTool,retval) ) 

#define References2_AddProject(This,pProject,retval)	\
    ( (This)->lpVtbl -> AddProject(This,pProject,retval) ) 


#define References2_AddSDK(This,wszSDKDisplayName,wszSDKidentifier,ppProjectReference)	\
    ( (This)->lpVtbl -> AddSDK(This,wszSDKDisplayName,wszSDKidentifier,ppProjectReference) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __References2_INTERFACE_DEFINED__ */


#ifndef __VBProjectProperties7_INTERFACE_DEFINED__
#define __VBProjectProperties7_INTERFACE_DEFINED__

/* interface VBProjectProperties7 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBProjectProperties7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ecb0a5b7-1c4e-4400-a6c4-4b83614d9cc0")
    VBProjectProperties7 : public VBProjectProperties6
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OutputTypeEx( 
            /* [retval][out] */ __RPC__out DWORD *pOutputType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OutputTypeEx( 
            /* [in] */ DWORD outputType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VBProjectProperties7Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VBProjectProperties7 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjAssemblyType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjAssemblyType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MyApplication )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MyType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_MyType )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationManifest )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationManifest )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionInfer )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOptionInfer *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionInfer )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOptionInfer noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LanguageVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LanguageVersion )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OutputTypeEx )( 
            __RPC__in VBProjectProperties7 * This,
            /* [retval][out] */ __RPC__out DWORD *pOutputType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OutputTypeEx )( 
            __RPC__in VBProjectProperties7 * This,
            /* [in] */ DWORD outputType);
        
        END_INTERFACE
    } VBProjectProperties7Vtbl;

    interface VBProjectProperties7
    {
        CONST_VTBL struct VBProjectProperties7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBProjectProperties7_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBProjectProperties7_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBProjectProperties7_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBProjectProperties7_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBProjectProperties7_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBProjectProperties7_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBProjectProperties7_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBProjectProperties7_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define VBProjectProperties7_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define VBProjectProperties7_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define VBProjectProperties7_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define VBProjectProperties7_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define VBProjectProperties7_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define VBProjectProperties7_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define VBProjectProperties7_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define VBProjectProperties7_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define VBProjectProperties7_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define VBProjectProperties7_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define VBProjectProperties7_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define VBProjectProperties7_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define VBProjectProperties7_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define VBProjectProperties7_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define VBProjectProperties7_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define VBProjectProperties7_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define VBProjectProperties7_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define VBProjectProperties7_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define VBProjectProperties7_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define VBProjectProperties7_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define VBProjectProperties7_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define VBProjectProperties7_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define VBProjectProperties7_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define VBProjectProperties7_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define VBProjectProperties7_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define VBProjectProperties7_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define VBProjectProperties7_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define VBProjectProperties7_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define VBProjectProperties7_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define VBProjectProperties7_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define VBProjectProperties7_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define VBProjectProperties7_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define VBProjectProperties7_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define VBProjectProperties7_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define VBProjectProperties7_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define VBProjectProperties7_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define VBProjectProperties7_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define VBProjectProperties7_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define VBProjectProperties7_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define VBProjectProperties7_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define VBProjectProperties7_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define VBProjectProperties7_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define VBProjectProperties7_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define VBProjectProperties7_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define VBProjectProperties7_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define VBProjectProperties7_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define VBProjectProperties7_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define VBProjectProperties7_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define VBProjectProperties7_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define VBProjectProperties7_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define VBProjectProperties7_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define VBProjectProperties7_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define VBProjectProperties7_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define VBProjectProperties7_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define VBProjectProperties7_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define VBProjectProperties7_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define VBProjectProperties7_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define VBProjectProperties7_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define VBProjectProperties7_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties7_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define VBProjectProperties7_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define VBProjectProperties7_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 

#define VBProjectProperties7_get_Title(This,retval)	\
    ( (This)->lpVtbl -> get_Title(This,retval) ) 

#define VBProjectProperties7_put_Title(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Title(This,noname,retval) ) 

#define VBProjectProperties7_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define VBProjectProperties7_put_Description(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Description(This,noname,retval) ) 

#define VBProjectProperties7_get_Company(This,retval)	\
    ( (This)->lpVtbl -> get_Company(This,retval) ) 

#define VBProjectProperties7_put_Company(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Company(This,noname,retval) ) 

#define VBProjectProperties7_get_Product(This,retval)	\
    ( (This)->lpVtbl -> get_Product(This,retval) ) 

#define VBProjectProperties7_put_Product(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Product(This,noname,retval) ) 

#define VBProjectProperties7_get_Copyright(This,retval)	\
    ( (This)->lpVtbl -> get_Copyright(This,retval) ) 

#define VBProjectProperties7_put_Copyright(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Copyright(This,noname,retval) ) 

#define VBProjectProperties7_get_Trademark(This,retval)	\
    ( (This)->lpVtbl -> get_Trademark(This,retval) ) 

#define VBProjectProperties7_put_Trademark(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Trademark(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,retval) ) 

#define VBProjectProperties7_put_AssemblyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,noname,retval) ) 

#define VBProjectProperties7_get_TypeComplianceDiagnostics(This,retval)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,retval) ) 

#define VBProjectProperties7_put_TypeComplianceDiagnostics(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,noname,retval) ) 

#define VBProjectProperties7_get_Win32ResourceFile(This,retval)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,retval) ) 

#define VBProjectProperties7_put_Win32ResourceFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyKeyProviderName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,retval) ) 

#define VBProjectProperties7_put_AssemblyKeyProviderName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyOriginatorKeyFileType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,retval) ) 

#define VBProjectProperties7_put_AssemblyOriginatorKeyFileType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,retval) ) 

#define VBProjectProperties7_put_AssemblyVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyFileVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,retval) ) 

#define VBProjectProperties7_put_AssemblyFileVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,noname,retval) ) 

#define VBProjectProperties7_get_GenerateManifests(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,retval) ) 

#define VBProjectProperties7_put_GenerateManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,noname,retval) ) 

#define VBProjectProperties7_get_EnableSecurityDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,retval) ) 

#define VBProjectProperties7_put_EnableSecurityDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,noname,retval) ) 

#define VBProjectProperties7_get_DebugSecurityZoneURL(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,retval) ) 

#define VBProjectProperties7_put_DebugSecurityZoneURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,noname,retval) ) 

#define VBProjectProperties7_get_Publish(This,retval)	\
    ( (This)->lpVtbl -> get_Publish(This,retval) ) 

#define VBProjectProperties7_get_ComVisible(This,retval)	\
    ( (This)->lpVtbl -> get_ComVisible(This,retval) ) 

#define VBProjectProperties7_put_ComVisible(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ComVisible(This,noname,retval) ) 

#define VBProjectProperties7_get_AssemblyGuid(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,retval) ) 

#define VBProjectProperties7_put_AssemblyGuid(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,noname,retval) ) 

#define VBProjectProperties7_get_NeutralResourcesLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,retval) ) 

#define VBProjectProperties7_put_NeutralResourcesLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,noname,retval) ) 

#define VBProjectProperties7_get_SignAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,retval) ) 

#define VBProjectProperties7_put_SignAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,noname,retval) ) 

#define VBProjectProperties7_get_SignManifests(This,retval)	\
    ( (This)->lpVtbl -> get_SignManifests(This,retval) ) 

#define VBProjectProperties7_put_SignManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignManifests(This,noname,retval) ) 

#define VBProjectProperties7_get_TargetZone(This,retval)	\
    ( (This)->lpVtbl -> get_TargetZone(This,retval) ) 

#define VBProjectProperties7_put_TargetZone(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetZone(This,noname,retval) ) 

#define VBProjectProperties7_get_ExcludedPermissions(This,retval)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,retval) ) 

#define VBProjectProperties7_put_ExcludedPermissions(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,noname,retval) ) 

#define VBProjectProperties7_get_ManifestCertificateThumbprint(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,retval) ) 

#define VBProjectProperties7_put_ManifestCertificateThumbprint(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,noname,retval) ) 

#define VBProjectProperties7_get_ManifestKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,retval) ) 

#define VBProjectProperties7_put_ManifestKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,noname,retval) ) 

#define VBProjectProperties7_get_ManifestTimestampUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,retval) ) 

#define VBProjectProperties7_put_ManifestTimestampUrl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,noname,retval) ) 

#define VBProjectProperties7_get_MyApplication(This,retval)	\
    ( (This)->lpVtbl -> get_MyApplication(This,retval) ) 

#define VBProjectProperties7_get_MyType(This,retval)	\
    ( (This)->lpVtbl -> get_MyType(This,retval) ) 

#define VBProjectProperties7_put_MyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_MyType(This,noname,retval) ) 

#define VBProjectProperties7_get_TargetFramework(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,retval) ) 

#define VBProjectProperties7_put_TargetFramework(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,noname,retval) ) 

#define VBProjectProperties7_get_ApplicationManifest(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationManifest(This,retval) ) 

#define VBProjectProperties7_put_ApplicationManifest(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationManifest(This,noname,retval) ) 

#define VBProjectProperties7_get_OptionInfer(This,retval)	\
    ( (This)->lpVtbl -> get_OptionInfer(This,retval) ) 

#define VBProjectProperties7_put_OptionInfer(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionInfer(This,noname,retval) ) 

#define VBProjectProperties7_get_TargetFrameworkMoniker(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,retval) ) 

#define VBProjectProperties7_put_TargetFrameworkMoniker(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,noname,retval) ) 

#define VBProjectProperties7_get_LanguageVersion(This,retval)	\
    ( (This)->lpVtbl -> get_LanguageVersion(This,retval) ) 

#define VBProjectProperties7_put_LanguageVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LanguageVersion(This,noname,retval) ) 


#define VBProjectProperties7_get_OutputTypeEx(This,pOutputType)	\
    ( (This)->lpVtbl -> get_OutputTypeEx(This,pOutputType) ) 

#define VBProjectProperties7_put_OutputTypeEx(This,outputType)	\
    ( (This)->lpVtbl -> put_OutputTypeEx(This,outputType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBProjectProperties7_get_OutputTypeEx_Proxy( 
    __RPC__in VBProjectProperties7 * This,
    /* [retval][out] */ __RPC__out DWORD *pOutputType);


void __RPC_STUB VBProjectProperties7_get_OutputTypeEx_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBProjectProperties7_put_OutputTypeEx_Proxy( 
    __RPC__in VBProjectProperties7 * This,
    /* [in] */ DWORD outputType);


void __RPC_STUB VBProjectProperties7_put_OutputTypeEx_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VBProjectProperties7_INTERFACE_DEFINED__ */


#ifndef __VBProjectConfigurationProperties6_INTERFACE_DEFINED__
#define __VBProjectConfigurationProperties6_INTERFACE_DEFINED__

/* interface VBProjectConfigurationProperties6 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VBProjectConfigurationProperties6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3519671B-B697-4721-BAD5-3BC4BFC76AB4")
    VBProjectConfigurationProperties6 : public VBProjectConfigurationProperties5
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Prefer32Bit( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pValue) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Prefer32Bit( 
            /* [in] */ VARIANT_BOOL value) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VBProjectConfigurationProperties6Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum sgenGenerationOption *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum sgenGenerationOption noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisDictionaries )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisDictionaries )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisCulture )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisCulture )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSet )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSet )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSetDirectories )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSetDirectories )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleDirectories )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleDirectories )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisFailOnMissingRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisFailOnMissingRules )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Prefer32Bit )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pValue);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Prefer32Bit )( 
            __RPC__in VBProjectConfigurationProperties6 * This,
            /* [in] */ VARIANT_BOOL value);
        
        END_INTERFACE
    } VBProjectConfigurationProperties6Vtbl;

    interface VBProjectConfigurationProperties6
    {
        CONST_VTBL struct VBProjectConfigurationProperties6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBProjectConfigurationProperties6_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VBProjectConfigurationProperties6_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VBProjectConfigurationProperties6_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VBProjectConfigurationProperties6_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VBProjectConfigurationProperties6_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VBProjectConfigurationProperties6_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VBProjectConfigurationProperties6_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VBProjectConfigurationProperties6_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define VBProjectConfigurationProperties6_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define VBProjectConfigurationProperties6_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define VBProjectConfigurationProperties6_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define VBProjectConfigurationProperties6_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define VBProjectConfigurationProperties6_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define VBProjectConfigurationProperties6_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define VBProjectConfigurationProperties6_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define VBProjectConfigurationProperties6_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define VBProjectConfigurationProperties6_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define VBProjectConfigurationProperties6_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define VBProjectConfigurationProperties6_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define VBProjectConfigurationProperties6_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define VBProjectConfigurationProperties6_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define VBProjectConfigurationProperties6_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define VBProjectConfigurationProperties6_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define VBProjectConfigurationProperties6_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define VBProjectConfigurationProperties6_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define VBProjectConfigurationProperties6_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define VBProjectConfigurationProperties6_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define VBProjectConfigurationProperties6_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define VBProjectConfigurationProperties6_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define VBProjectConfigurationProperties6_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define VBProjectConfigurationProperties6_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define VBProjectConfigurationProperties6_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define VBProjectConfigurationProperties6_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_DebugInfo(This,retval)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,retval) ) 

#define VBProjectConfigurationProperties6_put_DebugInfo(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_PlatformTarget(This,retval)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,retval) ) 

#define VBProjectConfigurationProperties6_put_PlatformTarget(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_TreatSpecificWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,retval) ) 

#define VBProjectConfigurationProperties6_put_TreatSpecificWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_RunCodeAnalysis(This,retval)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,retval) ) 

#define VBProjectConfigurationProperties6_put_RunCodeAnalysis(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisLogFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisLogFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisInputAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisInputAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisSpellCheckLanguages(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisSpellCheckLanguages(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisUseTypeNameInSuppression(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisModuleSuppressionsFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisModuleSuppressionsFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_UseVSHostingProcess(This,retval)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,retval) ) 

#define VBProjectConfigurationProperties6_put_UseVSHostingProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_GenerateSerializationAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,retval) ) 

#define VBProjectConfigurationProperties6_put_GenerateSerializationAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisIgnoreGeneratedCode(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreGeneratedCode(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisIgnoreGeneratedCode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreGeneratedCode(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisOverrideRuleVisibilities(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisOverrideRuleVisibilities(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisOverrideRuleVisibilities(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisOverrideRuleVisibilities(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisDictionaries(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisDictionaries(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisDictionaries(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisDictionaries(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisCulture(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisCulture(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisCulture(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisCulture(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisRuleSet(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSet(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisRuleSet(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSet(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisRuleSetDirectories(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSetDirectories(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisRuleSetDirectories(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSetDirectories(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisIgnoreBuiltInRuleSets(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRuleSets(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisIgnoreBuiltInRuleSets(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRuleSets(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisRuleDirectories(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleDirectories(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisRuleDirectories(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleDirectories(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisIgnoreBuiltInRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRules(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisIgnoreBuiltInRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRules(This,noname,retval) ) 

#define VBProjectConfigurationProperties6_get_CodeAnalysisFailOnMissingRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisFailOnMissingRules(This,retval) ) 

#define VBProjectConfigurationProperties6_put_CodeAnalysisFailOnMissingRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisFailOnMissingRules(This,noname,retval) ) 


#define VBProjectConfigurationProperties6_get_Prefer32Bit(This,pValue)	\
    ( (This)->lpVtbl -> get_Prefer32Bit(This,pValue) ) 

#define VBProjectConfigurationProperties6_put_Prefer32Bit(This,value)	\
    ( (This)->lpVtbl -> put_Prefer32Bit(This,value) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VBProjectConfigurationProperties6_INTERFACE_DEFINED__ */


#ifndef __CSharpProjectProperties7_INTERFACE_DEFINED__
#define __CSharpProjectProperties7_INTERFACE_DEFINED__

/* interface CSharpProjectProperties7 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_CSharpProjectProperties7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ad327414-a8f4-4e96-9a2a-3008592fd6a9")
    CSharpProjectProperties7 : public CSharpProjectProperties6
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OutputTypeEx( 
            /* [retval][out] */ __RPC__out DWORD *pOutputType) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OutputTypeEx( 
            /* [in] */ DWORD outputType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CSharpProjectProperties7Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjRunPostBuildEvent *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjRunPostBuildEvent noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Description )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Company )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Company )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Product )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Product )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Copyright )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Copyright )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Trademark )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Trademark )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out enum prjAssemblyType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ enum prjAssemblyType noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TypeComplianceDiagnostics )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TypeComplianceDiagnostics )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Win32ResourceFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Win32ResourceFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyProviderName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyProviderName )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFileType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFileType )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyFileVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyFileVersion )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateManifests )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateManifests )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSecurityDebugging )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSecurityDebugging )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSecurityZoneURL )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSecurityZoneURL )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Publish )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ComVisible )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ComVisible )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyGuid )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyGuid )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NeutralResourcesLanguage )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NeutralResourcesLanguage )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignAssembly )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignAssembly )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SignManifests )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SignManifests )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetZone )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetZone )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExcludedPermissions )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ExcludedPermissions )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestCertificateThumbprint )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestCertificateThumbprint )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestKeyFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestKeyFile )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ManifestTimestampUrl )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ManifestTimestampUrl )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationManifest )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationManifest )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFrameworkMoniker )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFrameworkMoniker )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OutputTypeEx )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [retval][out] */ __RPC__out DWORD *pOutputType);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OutputTypeEx )( 
            __RPC__in CSharpProjectProperties7 * This,
            /* [in] */ DWORD outputType);
        
        END_INTERFACE
    } CSharpProjectProperties7Vtbl;

    interface CSharpProjectProperties7
    {
        CONST_VTBL struct CSharpProjectProperties7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CSharpProjectProperties7_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CSharpProjectProperties7_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CSharpProjectProperties7_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CSharpProjectProperties7_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CSharpProjectProperties7_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CSharpProjectProperties7_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CSharpProjectProperties7_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CSharpProjectProperties7_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define CSharpProjectProperties7_get___project(This,retval)	\
    ( (This)->lpVtbl -> get___project(This,retval) ) 

#define CSharpProjectProperties7_get_StartupObject(This,retval)	\
    ( (This)->lpVtbl -> get_StartupObject(This,retval) ) 

#define CSharpProjectProperties7_put_StartupObject(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartupObject(This,noname,retval) ) 

#define CSharpProjectProperties7_get_OutputType(This,retval)	\
    ( (This)->lpVtbl -> get_OutputType(This,retval) ) 

#define CSharpProjectProperties7_put_OutputType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputType(This,noname,retval) ) 

#define CSharpProjectProperties7_get_RootNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_RootNamespace(This,retval) ) 

#define CSharpProjectProperties7_put_RootNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RootNamespace(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyName(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyName(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyOriginatorKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyKeyContainerName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyKeyContainerName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyOriginatorKeyMode(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval) ) 

#define CSharpProjectProperties7_get_DelaySign(This,retval)	\
    ( (This)->lpVtbl -> get_DelaySign(This,retval) ) 

#define CSharpProjectProperties7_put_DelaySign(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DelaySign(This,noname,retval) ) 

#define CSharpProjectProperties7_get_WebServer(This,retval)	\
    ( (This)->lpVtbl -> get_WebServer(This,retval) ) 

#define CSharpProjectProperties7_get_WebServerVersion(This,retval)	\
    ( (This)->lpVtbl -> get_WebServerVersion(This,retval) ) 

#define CSharpProjectProperties7_get_ServerExtensionsVersion(This,retval)	\
    ( (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval) ) 

#define CSharpProjectProperties7_get_LinkRepair(This,retval)	\
    ( (This)->lpVtbl -> get_LinkRepair(This,retval) ) 

#define CSharpProjectProperties7_put_LinkRepair(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LinkRepair(This,noname,retval) ) 

#define CSharpProjectProperties7_get_OfflineURL(This,retval)	\
    ( (This)->lpVtbl -> get_OfflineURL(This,retval) ) 

#define CSharpProjectProperties7_get_FileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_FileSharePath(This,retval) ) 

#define CSharpProjectProperties7_put_FileSharePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileSharePath(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ActiveFileSharePath(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveFileSharePath(This,retval) ) 

#define CSharpProjectProperties7_get_WebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_WebAccessMethod(This,retval) ) 

#define CSharpProjectProperties7_put_WebAccessMethod(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ActiveWebAccessMethod(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval) ) 

#define CSharpProjectProperties7_get_DefaultClientScript(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultClientScript(This,retval) ) 

#define CSharpProjectProperties7_put_DefaultClientScript(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval) ) 

#define CSharpProjectProperties7_get_DefaultTargetSchema(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultTargetSchema(This,retval) ) 

#define CSharpProjectProperties7_put_DefaultTargetSchema(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval) ) 

#define CSharpProjectProperties7_get_DefaultHTMLPageLayout(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval) ) 

#define CSharpProjectProperties7_put_DefaultHTMLPageLayout(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval) ) 

#define CSharpProjectProperties7_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define CSharpProjectProperties7_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define CSharpProjectProperties7_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define CSharpProjectProperties7_get_LocalPath(This,retval)	\
    ( (This)->lpVtbl -> get_LocalPath(This,retval) ) 

#define CSharpProjectProperties7_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define CSharpProjectProperties7_get_ActiveConfigurationSettings(This,retval)	\
    ( (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval) ) 

#define CSharpProjectProperties7_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CSharpProjectProperties7_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CSharpProjectProperties7_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CSharpProjectProperties7_get_ApplicationIcon(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationIcon(This,retval) ) 

#define CSharpProjectProperties7_put_ApplicationIcon(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval) ) 

#define CSharpProjectProperties7_get_OptionStrict(This,retval)	\
    ( (This)->lpVtbl -> get_OptionStrict(This,retval) ) 

#define CSharpProjectProperties7_put_OptionStrict(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionStrict(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ReferencePath(This,retval)	\
    ( (This)->lpVtbl -> get_ReferencePath(This,retval) ) 

#define CSharpProjectProperties7_put_ReferencePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ReferencePath(This,noname,retval) ) 

#define CSharpProjectProperties7_get_OutputFileName(This,retval)	\
    ( (This)->lpVtbl -> get_OutputFileName(This,retval) ) 

#define CSharpProjectProperties7_get_AbsoluteProjectDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval) ) 

#define CSharpProjectProperties7_get_OptionExplicit(This,retval)	\
    ( (This)->lpVtbl -> get_OptionExplicit(This,retval) ) 

#define CSharpProjectProperties7_put_OptionExplicit(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionExplicit(This,noname,retval) ) 

#define CSharpProjectProperties7_get_OptionCompare(This,retval)	\
    ( (This)->lpVtbl -> get_OptionCompare(This,retval) ) 

#define CSharpProjectProperties7_put_OptionCompare(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OptionCompare(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ProjectType(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectType(This,retval) ) 

#define CSharpProjectProperties7_get_DefaultNamespace(This,retval)	\
    ( (This)->lpVtbl -> get_DefaultNamespace(This,retval) ) 

#define CSharpProjectProperties7_put_DefaultNamespace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval) ) 

#define CSharpProjectProperties7_get_PreBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PreBuildEvent(This,retval) ) 

#define CSharpProjectProperties7_put_PreBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PreBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties7_get_PostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_PostBuildEvent(This,retval) ) 

#define CSharpProjectProperties7_put_PostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PostBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties7_get_RunPostBuildEvent(This,retval)	\
    ( (This)->lpVtbl -> get_RunPostBuildEvent(This,retval) ) 

#define CSharpProjectProperties7_put_RunPostBuildEvent(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunPostBuildEvent(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AspnetVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AspnetVersion(This,retval) ) 

#define CSharpProjectProperties7_get_Title(This,retval)	\
    ( (This)->lpVtbl -> get_Title(This,retval) ) 

#define CSharpProjectProperties7_put_Title(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Title(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define CSharpProjectProperties7_put_Description(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Description(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Company(This,retval)	\
    ( (This)->lpVtbl -> get_Company(This,retval) ) 

#define CSharpProjectProperties7_put_Company(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Company(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Product(This,retval)	\
    ( (This)->lpVtbl -> get_Product(This,retval) ) 

#define CSharpProjectProperties7_put_Product(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Product(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Copyright(This,retval)	\
    ( (This)->lpVtbl -> get_Copyright(This,retval) ) 

#define CSharpProjectProperties7_put_Copyright(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Copyright(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Trademark(This,retval)	\
    ( (This)->lpVtbl -> get_Trademark(This,retval) ) 

#define CSharpProjectProperties7_put_Trademark(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Trademark(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyType(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyType(This,noname,retval) ) 

#define CSharpProjectProperties7_get_TypeComplianceDiagnostics(This,retval)	\
    ( (This)->lpVtbl -> get_TypeComplianceDiagnostics(This,retval) ) 

#define CSharpProjectProperties7_put_TypeComplianceDiagnostics(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TypeComplianceDiagnostics(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Win32ResourceFile(This,retval)	\
    ( (This)->lpVtbl -> get_Win32ResourceFile(This,retval) ) 

#define CSharpProjectProperties7_put_Win32ResourceFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Win32ResourceFile(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyKeyProviderName(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyKeyProviderName(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyKeyProviderName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyKeyProviderName(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyOriginatorKeyFileType(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyOriginatorKeyFileType(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyOriginatorKeyFileType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyOriginatorKeyFileType(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyVersion(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyVersion(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyFileVersion(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyFileVersion(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyFileVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyFileVersion(This,noname,retval) ) 

#define CSharpProjectProperties7_get_GenerateManifests(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateManifests(This,retval) ) 

#define CSharpProjectProperties7_put_GenerateManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateManifests(This,noname,retval) ) 

#define CSharpProjectProperties7_get_EnableSecurityDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSecurityDebugging(This,retval) ) 

#define CSharpProjectProperties7_put_EnableSecurityDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSecurityDebugging(This,noname,retval) ) 

#define CSharpProjectProperties7_get_DebugSecurityZoneURL(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSecurityZoneURL(This,retval) ) 

#define CSharpProjectProperties7_put_DebugSecurityZoneURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSecurityZoneURL(This,noname,retval) ) 

#define CSharpProjectProperties7_get_Publish(This,retval)	\
    ( (This)->lpVtbl -> get_Publish(This,retval) ) 

#define CSharpProjectProperties7_get_ComVisible(This,retval)	\
    ( (This)->lpVtbl -> get_ComVisible(This,retval) ) 

#define CSharpProjectProperties7_put_ComVisible(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ComVisible(This,noname,retval) ) 

#define CSharpProjectProperties7_get_AssemblyGuid(This,retval)	\
    ( (This)->lpVtbl -> get_AssemblyGuid(This,retval) ) 

#define CSharpProjectProperties7_put_AssemblyGuid(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AssemblyGuid(This,noname,retval) ) 

#define CSharpProjectProperties7_get_NeutralResourcesLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_NeutralResourcesLanguage(This,retval) ) 

#define CSharpProjectProperties7_put_NeutralResourcesLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NeutralResourcesLanguage(This,noname,retval) ) 

#define CSharpProjectProperties7_get_SignAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_SignAssembly(This,retval) ) 

#define CSharpProjectProperties7_put_SignAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignAssembly(This,noname,retval) ) 

#define CSharpProjectProperties7_get_SignManifests(This,retval)	\
    ( (This)->lpVtbl -> get_SignManifests(This,retval) ) 

#define CSharpProjectProperties7_put_SignManifests(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SignManifests(This,noname,retval) ) 

#define CSharpProjectProperties7_get_TargetZone(This,retval)	\
    ( (This)->lpVtbl -> get_TargetZone(This,retval) ) 

#define CSharpProjectProperties7_put_TargetZone(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetZone(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ExcludedPermissions(This,retval)	\
    ( (This)->lpVtbl -> get_ExcludedPermissions(This,retval) ) 

#define CSharpProjectProperties7_put_ExcludedPermissions(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ExcludedPermissions(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ManifestCertificateThumbprint(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestCertificateThumbprint(This,retval) ) 

#define CSharpProjectProperties7_put_ManifestCertificateThumbprint(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestCertificateThumbprint(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ManifestKeyFile(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestKeyFile(This,retval) ) 

#define CSharpProjectProperties7_put_ManifestKeyFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestKeyFile(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ManifestTimestampUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ManifestTimestampUrl(This,retval) ) 

#define CSharpProjectProperties7_put_ManifestTimestampUrl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ManifestTimestampUrl(This,noname,retval) ) 

#define CSharpProjectProperties7_get_TargetFramework(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,retval) ) 

#define CSharpProjectProperties7_put_TargetFramework(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,noname,retval) ) 

#define CSharpProjectProperties7_get_ApplicationManifest(This,retval)	\
    ( (This)->lpVtbl -> get_ApplicationManifest(This,retval) ) 

#define CSharpProjectProperties7_put_ApplicationManifest(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ApplicationManifest(This,noname,retval) ) 

#define CSharpProjectProperties7_get_TargetFrameworkMoniker(This,retval)	\
    ( (This)->lpVtbl -> get_TargetFrameworkMoniker(This,retval) ) 

#define CSharpProjectProperties7_put_TargetFrameworkMoniker(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TargetFrameworkMoniker(This,noname,retval) ) 


#define CSharpProjectProperties7_get_OutputTypeEx(This,pOutputType)	\
    ( (This)->lpVtbl -> get_OutputTypeEx(This,pOutputType) ) 

#define CSharpProjectProperties7_put_OutputTypeEx(This,outputType)	\
    ( (This)->lpVtbl -> put_OutputTypeEx(This,outputType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE CSharpProjectProperties7_get_OutputTypeEx_Proxy( 
    __RPC__in CSharpProjectProperties7 * This,
    /* [retval][out] */ __RPC__out DWORD *pOutputType);


void __RPC_STUB CSharpProjectProperties7_get_OutputTypeEx_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE CSharpProjectProperties7_put_OutputTypeEx_Proxy( 
    __RPC__in CSharpProjectProperties7 * This,
    /* [in] */ DWORD outputType);


void __RPC_STUB CSharpProjectProperties7_put_OutputTypeEx_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __CSharpProjectProperties7_INTERFACE_DEFINED__ */


#ifndef __CSharpProjectConfigurationProperties6_INTERFACE_DEFINED__
#define __CSharpProjectConfigurationProperties6_INTERFACE_DEFINED__

/* interface CSharpProjectConfigurationProperties6 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_CSharpProjectConfigurationProperties6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("97C0FC65-E652-44E1-8367-907FDFCF84FD")
    CSharpProjectConfigurationProperties6 : public CSharpProjectConfigurationProperties5
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Prefer32Bit( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pValue) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Prefer32Bit( 
            /* [in] */ VARIANT_BOOL value) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct CSharpProjectConfigurationProperties6Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugInfo )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugInfo )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PlatformTarget )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_PlatformTarget )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatSpecificWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatSpecificWarningsAsErrors )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RunCodeAnalysis )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RunCodeAnalysis )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisLogFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisLogFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisInputAssembly )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisInputAssembly )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisSpellCheckLanguages )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisUseTypeNameInSuppression )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisModuleSuppressionsFile )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UseVSHostingProcess )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UseVSHostingProcess )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_GenerateSerializationAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out enum sgenGenerationOption *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_GenerateSerializationAssemblies )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ enum sgenGenerationOption noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LanguageVersion )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LanguageVersion )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ErrorReport )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ErrorReport )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreGeneratedCode )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisOverrideRuleVisibilities )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisDictionaries )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisDictionaries )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisCulture )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisCulture )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSet )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSet )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleSetDirectories )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleSetDirectories )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRuleSets )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisRuleDirectories )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisRuleDirectories )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisIgnoreBuiltInRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CodeAnalysisFailOnMissingRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CodeAnalysisFailOnMissingRules )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Prefer32Bit )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pValue);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Prefer32Bit )( 
            __RPC__in CSharpProjectConfigurationProperties6 * This,
            /* [in] */ VARIANT_BOOL value);
        
        END_INTERFACE
    } CSharpProjectConfigurationProperties6Vtbl;

    interface CSharpProjectConfigurationProperties6
    {
        CONST_VTBL struct CSharpProjectConfigurationProperties6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define CSharpProjectConfigurationProperties6_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define CSharpProjectConfigurationProperties6_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define CSharpProjectConfigurationProperties6_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define CSharpProjectConfigurationProperties6_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define CSharpProjectConfigurationProperties6_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define CSharpProjectConfigurationProperties6_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define CSharpProjectConfigurationProperties6_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define CSharpProjectConfigurationProperties6_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DebugSymbols(This,retval)	\
    ( (This)->lpVtbl -> get_DebugSymbols(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DebugSymbols(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugSymbols(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DefineDebug(This,retval)	\
    ( (This)->lpVtbl -> get_DefineDebug(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DefineDebug(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineDebug(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DefineTrace(This,retval)	\
    ( (This)->lpVtbl -> get_DefineTrace(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DefineTrace(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineTrace(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_OutputPath(This,retval)	\
    ( (This)->lpVtbl -> get_OutputPath(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_OutputPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_OutputPath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_IntermediatePath(This,retval)	\
    ( (This)->lpVtbl -> get_IntermediatePath(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_IntermediatePath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IntermediatePath(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DefineConstants(This,retval)	\
    ( (This)->lpVtbl -> get_DefineConstants(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DefineConstants(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DefineConstants(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_RemoveIntegerChecks(This,retval)	\
    ( (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_RemoveIntegerChecks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_BaseAddress(This,retval)	\
    ( (This)->lpVtbl -> get_BaseAddress(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_BaseAddress(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BaseAddress(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_AllowUnsafeBlocks(This,retval)	\
    ( (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_AllowUnsafeBlocks(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CheckForOverflowUnderflow(This,retval)	\
    ( (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CheckForOverflowUnderflow(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DocumentationFile(This,retval)	\
    ( (This)->lpVtbl -> get_DocumentationFile(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DocumentationFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DocumentationFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_Optimize(This,retval)	\
    ( (This)->lpVtbl -> get_Optimize(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_Optimize(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Optimize(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_IncrementalBuild(This,retval)	\
    ( (This)->lpVtbl -> get_IncrementalBuild(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_IncrementalBuild(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartWithIE(This,retval)	\
    ( (This)->lpVtbl -> get_StartWithIE(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartWithIE(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWithIE(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_EnableASPDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_EnableASPDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define CSharpProjectConfigurationProperties6_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define CSharpProjectConfigurationProperties6_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define CSharpProjectConfigurationProperties6_get_WarningLevel(This,retval)	\
    ( (This)->lpVtbl -> get_WarningLevel(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_WarningLevel(This,noname,retval)	\
    ( (This)->lpVtbl -> put_WarningLevel(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_TreatWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_TreatWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_FileAlignment(This,retval)	\
    ( (This)->lpVtbl -> get_FileAlignment(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_FileAlignment(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileAlignment(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_RegisterForComInterop(This,retval)	\
    ( (This)->lpVtbl -> get_RegisterForComInterop(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_RegisterForComInterop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_ConfigurationOverrideFile(This,retval)	\
    ( (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_ConfigurationOverrideFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_RemoteDebugEnabled(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_RemoteDebugEnabled(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_RemoteDebugMachine(This,retval)	\
    ( (This)->lpVtbl -> get_RemoteDebugMachine(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_RemoteDebugMachine(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_NoWarn(This,retval)	\
    ( (This)->lpVtbl -> get_NoWarn(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_NoWarn(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoWarn(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_NoStdLib(This,retval)	\
    ( (This)->lpVtbl -> get_NoStdLib(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_NoStdLib(This,noname,retval)	\
    ( (This)->lpVtbl -> put_NoStdLib(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_DebugInfo(This,retval)	\
    ( (This)->lpVtbl -> get_DebugInfo(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_DebugInfo(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DebugInfo(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_PlatformTarget(This,retval)	\
    ( (This)->lpVtbl -> get_PlatformTarget(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_PlatformTarget(This,noname,retval)	\
    ( (This)->lpVtbl -> put_PlatformTarget(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_TreatSpecificWarningsAsErrors(This,retval)	\
    ( (This)->lpVtbl -> get_TreatSpecificWarningsAsErrors(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_TreatSpecificWarningsAsErrors(This,noname,retval)	\
    ( (This)->lpVtbl -> put_TreatSpecificWarningsAsErrors(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_RunCodeAnalysis(This,retval)	\
    ( (This)->lpVtbl -> get_RunCodeAnalysis(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_RunCodeAnalysis(This,noname,retval)	\
    ( (This)->lpVtbl -> put_RunCodeAnalysis(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisLogFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisLogFile(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisLogFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisLogFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleAssemblies(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleAssemblies(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisInputAssembly(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisInputAssembly(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisInputAssembly(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisInputAssembly(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRules(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRules(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisSpellCheckLanguages(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisSpellCheckLanguages(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisSpellCheckLanguages(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisSpellCheckLanguages(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisUseTypeNameInSuppression(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisUseTypeNameInSuppression(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisUseTypeNameInSuppression(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisModuleSuppressionsFile(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisModuleSuppressionsFile(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisModuleSuppressionsFile(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisModuleSuppressionsFile(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_UseVSHostingProcess(This,retval)	\
    ( (This)->lpVtbl -> get_UseVSHostingProcess(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_UseVSHostingProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UseVSHostingProcess(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_GenerateSerializationAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_GenerateSerializationAssemblies(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_GenerateSerializationAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_GenerateSerializationAssemblies(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_LanguageVersion(This,retval)	\
    ( (This)->lpVtbl -> get_LanguageVersion(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_LanguageVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_LanguageVersion(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_ErrorReport(This,retval)	\
    ( (This)->lpVtbl -> get_ErrorReport(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_ErrorReport(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ErrorReport(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisIgnoreGeneratedCode(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreGeneratedCode(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisIgnoreGeneratedCode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreGeneratedCode(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisOverrideRuleVisibilities(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisOverrideRuleVisibilities(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisOverrideRuleVisibilities(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisOverrideRuleVisibilities(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisDictionaries(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisDictionaries(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisDictionaries(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisDictionaries(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisCulture(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisCulture(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisCulture(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisCulture(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisRuleSet(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSet(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisRuleSet(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSet(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisRuleSetDirectories(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleSetDirectories(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisRuleSetDirectories(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleSetDirectories(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisIgnoreBuiltInRuleSets(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRuleSets(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisIgnoreBuiltInRuleSets(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRuleSets(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisRuleDirectories(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisRuleDirectories(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisRuleDirectories(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisRuleDirectories(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisIgnoreBuiltInRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisIgnoreBuiltInRules(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisIgnoreBuiltInRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisIgnoreBuiltInRules(This,noname,retval) ) 

#define CSharpProjectConfigurationProperties6_get_CodeAnalysisFailOnMissingRules(This,retval)	\
    ( (This)->lpVtbl -> get_CodeAnalysisFailOnMissingRules(This,retval) ) 

#define CSharpProjectConfigurationProperties6_put_CodeAnalysisFailOnMissingRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CodeAnalysisFailOnMissingRules(This,noname,retval) ) 


#define CSharpProjectConfigurationProperties6_get_Prefer32Bit(This,pValue)	\
    ( (This)->lpVtbl -> get_Prefer32Bit(This,pValue) ) 

#define CSharpProjectConfigurationProperties6_put_Prefer32Bit(This,value)	\
    ( (This)->lpVtbl -> put_Prefer32Bit(This,value) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE CSharpProjectConfigurationProperties6_put_Prefer32Bit_Proxy( 
    __RPC__in CSharpProjectConfigurationProperties6 * This,
    /* [in] */ VARIANT_BOOL value);


void __RPC_STUB CSharpProjectConfigurationProperties6_put_Prefer32Bit_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __CSharpProjectConfigurationProperties6_INTERFACE_DEFINED__ */


#ifndef __Reference5_INTERFACE_DEFINED__
#define __Reference5_INTERFACE_DEFINED__

/* interface Reference5 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Reference5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("676A4C6F-F436-40DE-88E9-DB12BB864598")
    Reference5 : public Reference4
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ExpandedSdkReferences( 
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pRetVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Group( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Reference **ppRefGroup) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct Reference5Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in Reference5 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt References **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out enum prjReferenceType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Culture )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MajorVersion )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MinorVersion )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RevisionNumber )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildNumber )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StrongName )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SourceProject )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CopyLocal )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CopyLocal )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PublicKeyToken )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RuntimeVersion )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SpecificVersion )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SpecificVersion )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SubType )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_SubType )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Isolated )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Isolated )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Aliases )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Aliases )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RefType )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AutoReferenced )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Resolved )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EmbedInteropTypes )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EmbedInteropTypes )( 
            __RPC__in Reference5 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExpandedSdkReferences )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *pRetVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Group )( 
            __RPC__in Reference5 * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Reference **ppRefGroup);
        
        END_INTERFACE
    } Reference5Vtbl;

    interface Reference5
    {
        CONST_VTBL struct Reference5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Reference5_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Reference5_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Reference5_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Reference5_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Reference5_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Reference5_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Reference5_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Reference5_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Reference5_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Reference5_get_ContainingProject(This,retval)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,retval) ) 

#define Reference5_Remove(This,retval)	\
    ( (This)->lpVtbl -> Remove(This,retval) ) 

#define Reference5_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Reference5_get_Type(This,retval)	\
    ( (This)->lpVtbl -> get_Type(This,retval) ) 

#define Reference5_get_Identity(This,retval)	\
    ( (This)->lpVtbl -> get_Identity(This,retval) ) 

#define Reference5_get_Path(This,retval)	\
    ( (This)->lpVtbl -> get_Path(This,retval) ) 

#define Reference5_get_Description(This,retval)	\
    ( (This)->lpVtbl -> get_Description(This,retval) ) 

#define Reference5_get_Culture(This,retval)	\
    ( (This)->lpVtbl -> get_Culture(This,retval) ) 

#define Reference5_get_MajorVersion(This,retval)	\
    ( (This)->lpVtbl -> get_MajorVersion(This,retval) ) 

#define Reference5_get_MinorVersion(This,retval)	\
    ( (This)->lpVtbl -> get_MinorVersion(This,retval) ) 

#define Reference5_get_RevisionNumber(This,retval)	\
    ( (This)->lpVtbl -> get_RevisionNumber(This,retval) ) 

#define Reference5_get_BuildNumber(This,retval)	\
    ( (This)->lpVtbl -> get_BuildNumber(This,retval) ) 

#define Reference5_get_StrongName(This,retval)	\
    ( (This)->lpVtbl -> get_StrongName(This,retval) ) 

#define Reference5_get_SourceProject(This,retval)	\
    ( (This)->lpVtbl -> get_SourceProject(This,retval) ) 

#define Reference5_get_CopyLocal(This,retval)	\
    ( (This)->lpVtbl -> get_CopyLocal(This,retval) ) 

#define Reference5_put_CopyLocal(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CopyLocal(This,noname,retval) ) 

#define Reference5_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define Reference5_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define Reference5_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define Reference5_get_PublicKeyToken(This,retval)	\
    ( (This)->lpVtbl -> get_PublicKeyToken(This,retval) ) 

#define Reference5_get_Version(This,retval)	\
    ( (This)->lpVtbl -> get_Version(This,retval) ) 

#define Reference5_get_RuntimeVersion(This,retval)	\
    ( (This)->lpVtbl -> get_RuntimeVersion(This,retval) ) 

#define Reference5_get_SpecificVersion(This,retval)	\
    ( (This)->lpVtbl -> get_SpecificVersion(This,retval) ) 

#define Reference5_put_SpecificVersion(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SpecificVersion(This,noname,retval) ) 

#define Reference5_get_SubType(This,retval)	\
    ( (This)->lpVtbl -> get_SubType(This,retval) ) 

#define Reference5_put_SubType(This,noname,retval)	\
    ( (This)->lpVtbl -> put_SubType(This,noname,retval) ) 

#define Reference5_get_Isolated(This,retval)	\
    ( (This)->lpVtbl -> get_Isolated(This,retval) ) 

#define Reference5_put_Isolated(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Isolated(This,noname,retval) ) 

#define Reference5_get_Aliases(This,retval)	\
    ( (This)->lpVtbl -> get_Aliases(This,retval) ) 

#define Reference5_put_Aliases(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Aliases(This,noname,retval) ) 

#define Reference5_get_RefType(This,retval)	\
    ( (This)->lpVtbl -> get_RefType(This,retval) ) 

#define Reference5_get_AutoReferenced(This,retval)	\
    ( (This)->lpVtbl -> get_AutoReferenced(This,retval) ) 

#define Reference5_get_Resolved(This,retval)	\
    ( (This)->lpVtbl -> get_Resolved(This,retval) ) 

#define Reference5_get_EmbedInteropTypes(This,retval)	\
    ( (This)->lpVtbl -> get_EmbedInteropTypes(This,retval) ) 

#define Reference5_put_EmbedInteropTypes(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EmbedInteropTypes(This,noname,retval) ) 


#define Reference5_get_ExpandedSdkReferences(This,pRetVal)	\
    ( (This)->lpVtbl -> get_ExpandedSdkReferences(This,pRetVal) ) 

#define Reference5_get_Group(This,ppRefGroup)	\
    ( (This)->lpVtbl -> get_Group(This,ppRefGroup) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Reference5_INTERFACE_DEFINED__ */

#endif /* __VSLangProj110_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


