

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


#ifndef __WebProperties90_h__
#define __WebProperties90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __VSWebProjectItem2_FWD_DEFINED__
#define __VSWebProjectItem2_FWD_DEFINED__
typedef interface VSWebProjectItem2 VSWebProjectItem2;

#endif 	/* __VSWebProjectItem2_FWD_DEFINED__ */


#ifndef __WebSiteProperties3_FWD_DEFINED__
#define __WebSiteProperties3_FWD_DEFINED__
typedef interface WebSiteProperties3 WebSiteProperties3;

#endif 	/* __WebSiteProperties3_FWD_DEFINED__ */


#ifndef __WebFileProperties2_FWD_DEFINED__
#define __WebFileProperties2_FWD_DEFINED__
typedef interface WebFileProperties2 WebFileProperties2;

#endif 	/* __WebFileProperties2_FWD_DEFINED__ */


#ifndef __WebReference2_FWD_DEFINED__
#define __WebReference2_FWD_DEFINED__
typedef interface WebReference2 WebReference2;

#endif 	/* __WebReference2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_WebProperties90_0000_0000 */
/* [local] */ 

#pragma once
#define VSWEBSITE90_VER_MAJ    9
#define VSWEBSITE90_VER_MIN    0

enum __WebReference_MapType
    {
        WebReference2_MapType_Unknown	= 0,
        WebReference2_MapType_Discomap	= ( WebReference2_MapType_Unknown + 1 ) ,
        WebReference2_MapType_Svcmap	= ( WebReference2_MapType_Discomap + 1 ) ,
        WebReference2_MapType_Max	= WebReference2_MapType_Svcmap
    } ;
typedef DWORD WebReference_MapType;



extern RPC_IF_HANDLE __MIDL_itf_WebProperties90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WebProperties90_0000_0000_v0_0_s_ifspec;


#ifndef __VSWebSite90_LIBRARY_DEFINED__
#define __VSWebSite90_LIBRARY_DEFINED__

/* library VSWebSite90 */
/* [version][helpstring][uuid] */ 


enum __MIDL___MIDL_itf_WebProperties90_0000_0000_0001
    {
        DISPID_WebProjectItem_RunCustomTool	= 2000
    } ;

enum __MIDL___MIDL_itf_WebProperties90_0000_0000_0002
    {
        WEBSITEPROPID_TargetFramework	= 2000,
        WEBSITEPROPID_StartOnDebug	= 2001
    } ;

enum __MIDL___MIDL_itf_WebProperties90_0000_0000_0003
    {
        WEBFILEPROPID_CustomTool	= 100,
        WEBFILEPROPID_CustomToolNamespace	= ( WEBFILEPROPID_CustomTool + 1 ) ,
        WEBFILEPROPID_IsDependent	= ( WEBFILEPROPID_CustomToolNamespace + 1 ) ,
        WEBFILEPROPID_IsCustomToolOutput	= ( WEBFILEPROPID_IsDependent + 1 ) ,
        WEBFILEPROPID_CustomToolOutput	= ( WEBFILEPROPID_IsCustomToolOutput + 1 ) 
    } ;

enum __MIDL___MIDL_itf_WebProperties90_0000_0000_0004
    {
        DISPID_WebReference2_MapType	= 2000
    } ;

EXTERN_C const IID LIBID_VSWebSite90;

#ifndef __VSWebProjectItem2_INTERFACE_DEFINED__
#define __VSWebProjectItem2_INTERFACE_DEFINED__

/* interface VSWebProjectItem2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_VSWebProjectItem2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AC170AC6-D938-4796-BADA-BB4DECE4C2C5")
    VSWebProjectItem2 : public VSWebProjectItem
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE RunCustomTool( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct VSWebProjectItem2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in VSWebProjectItem2 * This,
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
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *UpdateLocalCopy )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *UpdateRemoteCopy )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *WaitUntilReady )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Unload )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RelatedFiles )( 
            __RPC__in VSWebProjectItem2 * This,
            /* [retval][out] */ __RPC__deref_out_opt RelatedFiles **retval);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *RunCustomTool )( 
            __RPC__in VSWebProjectItem2 * This);
        
        END_INTERFACE
    } VSWebProjectItem2Vtbl;

    interface VSWebProjectItem2
    {
        CONST_VTBL struct VSWebProjectItem2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VSWebProjectItem2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define VSWebProjectItem2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define VSWebProjectItem2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define VSWebProjectItem2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define VSWebProjectItem2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define VSWebProjectItem2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define VSWebProjectItem2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define VSWebProjectItem2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define VSWebProjectItem2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define VSWebProjectItem2_get_ContainingProject(This,retval)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,retval) ) 

#define VSWebProjectItem2_UpdateLocalCopy(This,retval)	\
    ( (This)->lpVtbl -> UpdateLocalCopy(This,retval) ) 

#define VSWebProjectItem2_UpdateRemoteCopy(This,retval)	\
    ( (This)->lpVtbl -> UpdateRemoteCopy(This,retval) ) 

#define VSWebProjectItem2_WaitUntilReady(This,retval)	\
    ( (This)->lpVtbl -> WaitUntilReady(This,retval) ) 

#define VSWebProjectItem2_Load(This,retval)	\
    ( (This)->lpVtbl -> Load(This,retval) ) 

#define VSWebProjectItem2_Unload(This,retval)	\
    ( (This)->lpVtbl -> Unload(This,retval) ) 

#define VSWebProjectItem2_get_RelatedFiles(This,retval)	\
    ( (This)->lpVtbl -> get_RelatedFiles(This,retval) ) 


#define VSWebProjectItem2_RunCustomTool(This)	\
    ( (This)->lpVtbl -> RunCustomTool(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __VSWebProjectItem2_INTERFACE_DEFINED__ */


#ifndef __WebSiteProperties3_INTERFACE_DEFINED__
#define __WebSiteProperties3_INTERFACE_DEFINED__

/* interface WebSiteProperties3 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebSiteProperties3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("42B54C4B-2A9C-4ebc-A9A2-344A8F96107F")
    WebSiteProperties3 : public WebSiteProperties2
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_TargetFramework( 
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework) = 0;
        
        virtual /* [helpstring][id][nonbrowsable][propput] */ HRESULT STDMETHODCALLTYPE put_TargetFramework( 
            /* [in] */ DWORD dwTargetFramework) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StartWebServerOnDebug( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbAlwaysStart) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StartWebServerOnDebug( 
            /* [in] */ VARIANT_BOOL bAlwaysStart) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebSiteProperties3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebSiteProperties3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in WebSiteProperties3 * This,
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
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OpenedURL )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BrowseURL )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BrowseURL )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServer )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServer )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebSiteType )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out enum webType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentWebsiteLanguage )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out enum webStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ enum webStartAction noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableFxCop )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableFxCop )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRuleAssemblies )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FxCopRules )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FxCopRules )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableVsWebServerDynamicPort )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerPort )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out unsigned short *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerPort )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ unsigned short noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableNTLMAuthentication )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectDirty )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_VsWebServerVPath )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_VsWebServerVPath )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_TargetFramework )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwTargetFramework);
        
        /* [helpstring][id][nonbrowsable][propput] */ HRESULT ( STDMETHODCALLTYPE *put_TargetFramework )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in] */ DWORD dwTargetFramework);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbAlwaysStart);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StartWebServerOnDebug )( 
            __RPC__in WebSiteProperties3 * This,
            /* [in] */ VARIANT_BOOL bAlwaysStart);
        
        END_INTERFACE
    } WebSiteProperties3Vtbl;

    interface WebSiteProperties3
    {
        CONST_VTBL struct WebSiteProperties3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebSiteProperties3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define WebSiteProperties3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define WebSiteProperties3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define WebSiteProperties3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define WebSiteProperties3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define WebSiteProperties3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define WebSiteProperties3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define WebSiteProperties3_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define WebSiteProperties3_get_OpenedURL(This,retval)	\
    ( (This)->lpVtbl -> get_OpenedURL(This,retval) ) 

#define WebSiteProperties3_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define WebSiteProperties3_get_BrowseURL(This,retval)	\
    ( (This)->lpVtbl -> get_BrowseURL(This,retval) ) 

#define WebSiteProperties3_put_BrowseURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_BrowseURL(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableVsWebServer(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServer(This,retval) ) 

#define WebSiteProperties3_put_EnableVsWebServer(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServer(This,noname,retval) ) 

#define WebSiteProperties3_get_WebSiteType(This,retval)	\
    ( (This)->lpVtbl -> get_WebSiteType(This,retval) ) 

#define WebSiteProperties3_get_CurrentWebsiteLanguage(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentWebsiteLanguage(This,retval) ) 

#define WebSiteProperties3_put_CurrentWebsiteLanguage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentWebsiteLanguage(This,noname,retval) ) 

#define WebSiteProperties3_get_StartProgram(This,retval)	\
    ( (This)->lpVtbl -> get_StartProgram(This,retval) ) 

#define WebSiteProperties3_put_StartProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartProgram(This,noname,retval) ) 

#define WebSiteProperties3_get_StartWorkingDirectory(This,retval)	\
    ( (This)->lpVtbl -> get_StartWorkingDirectory(This,retval) ) 

#define WebSiteProperties3_put_StartWorkingDirectory(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval) ) 

#define WebSiteProperties3_get_StartURL(This,retval)	\
    ( (This)->lpVtbl -> get_StartURL(This,retval) ) 

#define WebSiteProperties3_put_StartURL(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartURL(This,noname,retval) ) 

#define WebSiteProperties3_get_StartPage(This,retval)	\
    ( (This)->lpVtbl -> get_StartPage(This,retval) ) 

#define WebSiteProperties3_put_StartPage(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartPage(This,noname,retval) ) 

#define WebSiteProperties3_get_StartArguments(This,retval)	\
    ( (This)->lpVtbl -> get_StartArguments(This,retval) ) 

#define WebSiteProperties3_put_StartArguments(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartArguments(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableASPXDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableASPXDebugging(This,retval) ) 

#define WebSiteProperties3_put_EnableASPXDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableUnmanagedDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval) ) 

#define WebSiteProperties3_put_EnableUnmanagedDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval) ) 

#define WebSiteProperties3_get_StartAction(This,retval)	\
    ( (This)->lpVtbl -> get_StartAction(This,retval) ) 

#define WebSiteProperties3_put_StartAction(This,noname,retval)	\
    ( (This)->lpVtbl -> put_StartAction(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableSQLServerDebugging(This,retval)	\
    ( (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval) ) 

#define WebSiteProperties3_put_EnableSQLServerDebugging(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval) ) 

#define WebSiteProperties3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define WebSiteProperties3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define WebSiteProperties3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define WebSiteProperties3_get_EnableFxCop(This,retval)	\
    ( (This)->lpVtbl -> get_EnableFxCop(This,retval) ) 

#define WebSiteProperties3_put_EnableFxCop(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableFxCop(This,noname,retval) ) 

#define WebSiteProperties3_get_FxCopRuleAssemblies(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRuleAssemblies(This,retval) ) 

#define WebSiteProperties3_put_FxCopRuleAssemblies(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRuleAssemblies(This,noname,retval) ) 

#define WebSiteProperties3_get_FxCopRules(This,retval)	\
    ( (This)->lpVtbl -> get_FxCopRules(This,retval) ) 

#define WebSiteProperties3_put_FxCopRules(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FxCopRules(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableVsWebServerDynamicPort(This,retval)	\
    ( (This)->lpVtbl -> get_EnableVsWebServerDynamicPort(This,retval) ) 

#define WebSiteProperties3_put_EnableVsWebServerDynamicPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableVsWebServerDynamicPort(This,noname,retval) ) 

#define WebSiteProperties3_get_VsWebServerPort(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerPort(This,retval) ) 

#define WebSiteProperties3_put_VsWebServerPort(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerPort(This,noname,retval) ) 

#define WebSiteProperties3_get_EnableNTLMAuthentication(This,retval)	\
    ( (This)->lpVtbl -> get_EnableNTLMAuthentication(This,retval) ) 

#define WebSiteProperties3_put_EnableNTLMAuthentication(This,noname,retval)	\
    ( (This)->lpVtbl -> put_EnableNTLMAuthentication(This,noname,retval) ) 

#define WebSiteProperties3_get_ProjectDirty(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectDirty(This,retval) ) 

#define WebSiteProperties3_get_VsWebServerVPath(This,retval)	\
    ( (This)->lpVtbl -> get_VsWebServerVPath(This,retval) ) 

#define WebSiteProperties3_put_VsWebServerVPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_VsWebServerVPath(This,noname,retval) ) 


#define WebSiteProperties3_get_TargetFramework(This,pdwTargetFramework)	\
    ( (This)->lpVtbl -> get_TargetFramework(This,pdwTargetFramework) ) 

#define WebSiteProperties3_put_TargetFramework(This,dwTargetFramework)	\
    ( (This)->lpVtbl -> put_TargetFramework(This,dwTargetFramework) ) 

#define WebSiteProperties3_get_StartWebServerOnDebug(This,pbAlwaysStart)	\
    ( (This)->lpVtbl -> get_StartWebServerOnDebug(This,pbAlwaysStart) ) 

#define WebSiteProperties3_put_StartWebServerOnDebug(This,bAlwaysStart)	\
    ( (This)->lpVtbl -> put_StartWebServerOnDebug(This,bAlwaysStart) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebSiteProperties3_INTERFACE_DEFINED__ */


#ifndef __WebFileProperties2_INTERFACE_DEFINED__
#define __WebFileProperties2_INTERFACE_DEFINED__

/* interface WebFileProperties2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_WebFileProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("899456B4-E5AD-49ef-AB04-1CED913A6E44")
    WebFileProperties2 : public WebFileProperties
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomTool( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomTool) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CustomTool( 
            /* [in] */ __RPC__in BSTR bstrCustomTool) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomToolNamespace( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomToolNamespace) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_CustomToolNamespace( 
            /* [in] */ __RPC__in BSTR bstrCustomToolNamespace) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_CustomToolOutput( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomToolOutput) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsCustomToolOutput( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsCustomToolOutput) = 0;
        
        virtual /* [helpstring][nonbrowsable][id][propget] */ HRESULT STDMETHODCALLTYPE get_IsDependentFile( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsDependentFile) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebFileProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebFileProperties2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in WebFileProperties2 * This,
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
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extension )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RelativeURL )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AutoRefreshPath )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AutoRefreshPath )( 
            __RPC__in WebFileProperties2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomTool )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomTool);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CustomTool )( 
            __RPC__in WebFileProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrCustomTool);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolNamespace )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomToolNamespace);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_CustomToolNamespace )( 
            __RPC__in WebFileProperties2 * This,
            /* [in] */ __RPC__in BSTR bstrCustomToolNamespace);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_CustomToolOutput )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrCustomToolOutput);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsCustomToolOutput )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsCustomToolOutput);
        
        /* [helpstring][nonbrowsable][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IsDependentFile )( 
            __RPC__in WebFileProperties2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pbIsDependentFile);
        
        END_INTERFACE
    } WebFileProperties2Vtbl;

    interface WebFileProperties2
    {
        CONST_VTBL struct WebFileProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebFileProperties2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define WebFileProperties2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define WebFileProperties2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define WebFileProperties2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define WebFileProperties2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define WebFileProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define WebFileProperties2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define WebFileProperties2_get___id(This,retval)	\
    ( (This)->lpVtbl -> get___id(This,retval) ) 

#define WebFileProperties2_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define WebFileProperties2_put_FileName(This,noname,retval)	\
    ( (This)->lpVtbl -> put_FileName(This,noname,retval) ) 

#define WebFileProperties2_get_Extension(This,retval)	\
    ( (This)->lpVtbl -> get_Extension(This,retval) ) 

#define WebFileProperties2_get_FullPath(This,retval)	\
    ( (This)->lpVtbl -> get_FullPath(This,retval) ) 

#define WebFileProperties2_get_URL(This,retval)	\
    ( (This)->lpVtbl -> get_URL(This,retval) ) 

#define WebFileProperties2_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define WebFileProperties2_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define WebFileProperties2_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define WebFileProperties2_get_RelativeURL(This,retval)	\
    ( (This)->lpVtbl -> get_RelativeURL(This,retval) ) 

#define WebFileProperties2_get_AutoRefreshPath(This,retval)	\
    ( (This)->lpVtbl -> get_AutoRefreshPath(This,retval) ) 

#define WebFileProperties2_put_AutoRefreshPath(This,noname,retval)	\
    ( (This)->lpVtbl -> put_AutoRefreshPath(This,noname,retval) ) 


#define WebFileProperties2_get_CustomTool(This,pbstrCustomTool)	\
    ( (This)->lpVtbl -> get_CustomTool(This,pbstrCustomTool) ) 

#define WebFileProperties2_put_CustomTool(This,bstrCustomTool)	\
    ( (This)->lpVtbl -> put_CustomTool(This,bstrCustomTool) ) 

#define WebFileProperties2_get_CustomToolNamespace(This,pbstrCustomToolNamespace)	\
    ( (This)->lpVtbl -> get_CustomToolNamespace(This,pbstrCustomToolNamespace) ) 

#define WebFileProperties2_put_CustomToolNamespace(This,bstrCustomToolNamespace)	\
    ( (This)->lpVtbl -> put_CustomToolNamespace(This,bstrCustomToolNamespace) ) 

#define WebFileProperties2_get_CustomToolOutput(This,pbstrCustomToolOutput)	\
    ( (This)->lpVtbl -> get_CustomToolOutput(This,pbstrCustomToolOutput) ) 

#define WebFileProperties2_get_IsCustomToolOutput(This,pbIsCustomToolOutput)	\
    ( (This)->lpVtbl -> get_IsCustomToolOutput(This,pbIsCustomToolOutput) ) 

#define WebFileProperties2_get_IsDependentFile(This,pbIsDependentFile)	\
    ( (This)->lpVtbl -> get_IsDependentFile(This,pbIsDependentFile) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebFileProperties2_INTERFACE_DEFINED__ */


#ifndef __WebReference2_INTERFACE_DEFINED__
#define __WebReference2_INTERFACE_DEFINED__

/* interface WebReference2 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_WebReference2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9D6065EC-93A1-4caa-8985-9B7783330BE7")
    WebReference2 : public WebReference
    {
    public:
        virtual /* [helpstring][id][nonbrowsable][propget] */ HRESULT STDMETHODCALLTYPE get_MapType( 
            /* [retval][out] */ __RPC__out DWORD *pdwMapType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct WebReference2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in WebReference2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            __RPC__in WebReference2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            __RPC__in WebReference2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            __RPC__in WebReference2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            __RPC__in WebReference2 * This,
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
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DynamicUrl )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DynamicUrl )( 
            __RPC__in WebReference2 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DynamicPropName )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Namespace )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceName )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceLocationUrl )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Update )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileCodeModel )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt FileCodeModel **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectItem )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServiceDefinitionUrl )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Discomap )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WsdlAppRelativeUrl )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [helpstring][id][nonbrowsable][propget] */ HRESULT ( STDMETHODCALLTYPE *get_MapType )( 
            __RPC__in WebReference2 * This,
            /* [retval][out] */ __RPC__out DWORD *pdwMapType);
        
        END_INTERFACE
    } WebReference2Vtbl;

    interface WebReference2
    {
        CONST_VTBL struct WebReference2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define WebReference2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define WebReference2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define WebReference2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define WebReference2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define WebReference2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define WebReference2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define WebReference2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define WebReference2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define WebReference2_get_ContainingProject(This,retval)	\
    ( (This)->lpVtbl -> get_ContainingProject(This,retval) ) 

#define WebReference2_Remove(This,retval)	\
    ( (This)->lpVtbl -> Remove(This,retval) ) 

#define WebReference2_get_DynamicUrl(This,retval)	\
    ( (This)->lpVtbl -> get_DynamicUrl(This,retval) ) 

#define WebReference2_put_DynamicUrl(This,noname,retval)	\
    ( (This)->lpVtbl -> put_DynamicUrl(This,noname,retval) ) 

#define WebReference2_get_DynamicPropName(This,retval)	\
    ( (This)->lpVtbl -> get_DynamicPropName(This,retval) ) 

#define WebReference2_get_Namespace(This,retval)	\
    ( (This)->lpVtbl -> get_Namespace(This,retval) ) 

#define WebReference2_get_ServiceName(This,retval)	\
    ( (This)->lpVtbl -> get_ServiceName(This,retval) ) 

#define WebReference2_get_ServiceLocationUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ServiceLocationUrl(This,retval) ) 

#define WebReference2_Update(This,retval)	\
    ( (This)->lpVtbl -> Update(This,retval) ) 

#define WebReference2_get_FileCodeModel(This,retval)	\
    ( (This)->lpVtbl -> get_FileCodeModel(This,retval) ) 

#define WebReference2_get_ProjectItem(This,retval)	\
    ( (This)->lpVtbl -> get_ProjectItem(This,retval) ) 

#define WebReference2_get_ServiceDefinitionUrl(This,retval)	\
    ( (This)->lpVtbl -> get_ServiceDefinitionUrl(This,retval) ) 

#define WebReference2_get_Discomap(This,retval)	\
    ( (This)->lpVtbl -> get_Discomap(This,retval) ) 

#define WebReference2_get_WsdlAppRelativeUrl(This,retval)	\
    ( (This)->lpVtbl -> get_WsdlAppRelativeUrl(This,retval) ) 


#define WebReference2_get_MapType(This,pdwMapType)	\
    ( (This)->lpVtbl -> get_MapType(This,pdwMapType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __WebReference2_INTERFACE_DEFINED__ */

#endif /* __VSWebSite90_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


