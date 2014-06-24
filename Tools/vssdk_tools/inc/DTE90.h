

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for dte90.idl:
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


#ifndef __dte90_h__
#define __dte90_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ToolBoxTab3_FWD_DEFINED__
#define __ToolBoxTab3_FWD_DEFINED__
typedef interface ToolBoxTab3 ToolBoxTab3;
#endif 	/* __ToolBoxTab3_FWD_DEFINED__ */


#ifndef __HTMLWindow3_FWD_DEFINED__
#define __HTMLWindow3_FWD_DEFINED__
typedef interface HTMLWindow3 HTMLWindow3;
#endif 	/* __HTMLWindow3_FWD_DEFINED__ */


#ifndef __Debugger3_FWD_DEFINED__
#define __Debugger3_FWD_DEFINED__
typedef interface Debugger3 Debugger3;
#endif 	/* __Debugger3_FWD_DEFINED__ */


#ifndef __Thread2_FWD_DEFINED__
#define __Thread2_FWD_DEFINED__
typedef interface Thread2 Thread2;
#endif 	/* __Thread2_FWD_DEFINED__ */


#ifndef __Process3_FWD_DEFINED__
#define __Process3_FWD_DEFINED__
typedef interface Process3 Process3;
#endif 	/* __Process3_FWD_DEFINED__ */


#ifndef __Modules_FWD_DEFINED__
#define __Modules_FWD_DEFINED__
typedef interface Modules Modules;
#endif 	/* __Modules_FWD_DEFINED__ */


#ifndef __Module_FWD_DEFINED__
#define __Module_FWD_DEFINED__
typedef interface Module Module;
#endif 	/* __Module_FWD_DEFINED__ */


#ifndef __ExceptionGroups_FWD_DEFINED__
#define __ExceptionGroups_FWD_DEFINED__
typedef interface ExceptionGroups ExceptionGroups;
#endif 	/* __ExceptionGroups_FWD_DEFINED__ */


#ifndef __ExceptionSettings_FWD_DEFINED__
#define __ExceptionSettings_FWD_DEFINED__
typedef interface ExceptionSettings ExceptionSettings;
#endif 	/* __ExceptionSettings_FWD_DEFINED__ */


#ifndef __ExceptionSetting_FWD_DEFINED__
#define __ExceptionSetting_FWD_DEFINED__
typedef interface ExceptionSetting ExceptionSetting;
#endif 	/* __ExceptionSetting_FWD_DEFINED__ */


#ifndef __Template_FWD_DEFINED__
#define __Template_FWD_DEFINED__
typedef interface Template Template;
#endif 	/* __Template_FWD_DEFINED__ */


#ifndef __Templates_FWD_DEFINED__
#define __Templates_FWD_DEFINED__
typedef interface Templates Templates;
#endif 	/* __Templates_FWD_DEFINED__ */


#ifndef __Solution3_FWD_DEFINED__
#define __Solution3_FWD_DEFINED__
typedef interface Solution3 Solution3;
#endif 	/* __Solution3_FWD_DEFINED__ */


#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_dte90_0000_0000 */
/* [local] */ 

#pragma once
#ifndef __INDENTSTYLE__
#define __INDENTSTYLE__
typedef /* [uuid] */  DECLSPEC_UUID("BCCEBE05-D29C-11D2-AABD-00C04F688DDE") 
enum _vsIndentStyle
    {	vsIndentStyleNone	= 0,
	vsIndentStyleDefault	= ( vsIndentStyleNone + 1 ) ,
	vsIndentStyleSmart	= ( vsIndentStyleDefault + 1 ) 
    } 	vsIndentStyle;

#endif // __INDENTSTYLE__


extern RPC_IF_HANDLE __MIDL_itf_dte90_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_dte90_0000_0000_v0_0_s_ifspec;


#ifndef __EnvDTE90_LIBRARY_DEFINED__
#define __EnvDTE90_LIBRARY_DEFINED__

/* library EnvDTE90 */
/* [version][helpstring][uuid] */ 

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("FCDF1B34-0781-43c6-BD46-062CAEA1FB05") 
enum vsHTMLViews
    {	vsHTMLViewSource	= 0,
	vsHTMLViewDesign	= 1
    } 	vsHTMLViews;

typedef /* [helpstringcontext][helpstring][helpcontext][uuid] */  DECLSPEC_UUID("7F27C244-AFD2-4bba-8193-DAA837CC03DA") 
enum vsHTMLPanes
    {	vsHTMLPaneSource	= 0,
	vsHTMLPaneDesign	= 1,
	vsHTMLPaneSplit	= 2
    } 	vsHTMLPanes;

typedef 
enum vsHTMLBackgroundTasks
    {	vsHTMLBackgroundCompilation	= 0,
	vsHTMLBackgroundToolboxPopulation	= ( vsHTMLBackgroundCompilation + 1 ) 
    } 	vsHTMLBackgroundTasks;









typedef 
enum enum_THREADCATEGORY
    {	THREADCATEGORY_Worker	= 0,
	THREADCATEGORY_UI	= ( THREADCATEGORY_Worker + 1 ) ,
	THREADCATEGORY_Main	= ( THREADCATEGORY_UI + 1 ) ,
	THREADCATEGORY_RPC	= ( THREADCATEGORY_Main + 1 ) ,
	THREADCATEGORY_Unknown	= ( THREADCATEGORY_RPC + 1 ) 
    } 	THREADCATEGORY;


EXTERN_C const IID LIBID_EnvDTE90;

#ifndef __ToolBoxTab3_INTERFACE_DEFINED__
#define __ToolBoxTab3_INTERFACE_DEFINED__

/* interface ToolBoxTab3 */
/* [helpstringcontext][helpstring][helpcontext][uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_ToolBoxTab3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("09D8476F-E6BF-46fb-A0A9-61C331B90F06")
    ToolBoxTab3 : public ToolBoxTab2
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_Expanded( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfExpanded) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_Expanded( 
            /* [in] */ VARIANT_BOOL fExpanded) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ToolBoxTab3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ToolBoxTab3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolBoxTabs **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Activate )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Delete )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ToolBoxItems )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__deref_out_opt ToolBoxItems **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ListView )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ListView )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UniqueID )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_UniqueID )( 
            ToolBoxTab3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Expanded )( 
            ToolBoxTab3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfExpanded);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Expanded )( 
            ToolBoxTab3 * This,
            /* [in] */ VARIANT_BOOL fExpanded);
        
        END_INTERFACE
    } ToolBoxTab3Vtbl;

    interface ToolBoxTab3
    {
        CONST_VTBL struct ToolBoxTab3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ToolBoxTab3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define ToolBoxTab3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define ToolBoxTab3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define ToolBoxTab3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define ToolBoxTab3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define ToolBoxTab3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define ToolBoxTab3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define ToolBoxTab3_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define ToolBoxTab3_put_Name(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Name(This,noname,retval) ) 

#define ToolBoxTab3_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define ToolBoxTab3_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define ToolBoxTab3_Activate(This,retval)	\
    ( (This)->lpVtbl -> Activate(This,retval) ) 

#define ToolBoxTab3_Delete(This,retval)	\
    ( (This)->lpVtbl -> Delete(This,retval) ) 

#define ToolBoxTab3_get_ToolBoxItems(This,retval)	\
    ( (This)->lpVtbl -> get_ToolBoxItems(This,retval) ) 

#define ToolBoxTab3_get_ListView(This,retval)	\
    ( (This)->lpVtbl -> get_ListView(This,retval) ) 

#define ToolBoxTab3_put_ListView(This,noname,retval)	\
    ( (This)->lpVtbl -> put_ListView(This,noname,retval) ) 

#define ToolBoxTab3_get_UniqueID(This,retval)	\
    ( (This)->lpVtbl -> get_UniqueID(This,retval) ) 

#define ToolBoxTab3_put_UniqueID(This,noname,retval)	\
    ( (This)->lpVtbl -> put_UniqueID(This,noname,retval) ) 


#define ToolBoxTab3_get_Expanded(This,pfExpanded)	\
    ( (This)->lpVtbl -> get_Expanded(This,pfExpanded) ) 

#define ToolBoxTab3_put_Expanded(This,fExpanded)	\
    ( (This)->lpVtbl -> put_Expanded(This,fExpanded) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ToolBoxTab3_INTERFACE_DEFINED__ */


#ifndef __HTMLWindow3_INTERFACE_DEFINED__
#define __HTMLWindow3_INTERFACE_DEFINED__

/* interface HTMLWindow3 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_HTMLWindow3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BAD0A3DD-8109-4684-B806-A5282267BFE4")
    HTMLWindow3 : public IDispatch
    {
    public:
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_CurrentView( 
            /* [retval][out] */ __RPC__out vsHTMLViews *pView) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_CurrentView( 
            /* [in] */ vsHTMLViews View) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT STDMETHODCALLTYPE get_CurrentPane( 
            /* [retval][out] */ __RPC__out vsHTMLPanes *pPane) = 0;
        
        virtual /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT STDMETHODCALLTYPE put_CurrentPane( 
            /* [in] */ vsHTMLPanes Pane) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE WaitForBackgroundProcessingComplete( 
            /* [in] */ vsHTMLBackgroundTasks Task) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct HTMLWindow3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            HTMLWindow3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            HTMLWindow3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            HTMLWindow3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            HTMLWindow3 * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            HTMLWindow3 * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            HTMLWindow3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            HTMLWindow3 * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentView )( 
            HTMLWindow3 * This,
            /* [retval][out] */ __RPC__out vsHTMLViews *pView);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentView )( 
            HTMLWindow3 * This,
            /* [in] */ vsHTMLViews View);
        
        /* [helpstringcontext][helpstring][helpcontext][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentPane )( 
            HTMLWindow3 * This,
            /* [retval][out] */ __RPC__out vsHTMLPanes *pPane);
        
        /* [helpstringcontext][helpstring][helpcontext][propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentPane )( 
            HTMLWindow3 * This,
            /* [in] */ vsHTMLPanes Pane);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *WaitForBackgroundProcessingComplete )( 
            HTMLWindow3 * This,
            /* [in] */ vsHTMLBackgroundTasks Task);
        
        END_INTERFACE
    } HTMLWindow3Vtbl;

    interface HTMLWindow3
    {
        CONST_VTBL struct HTMLWindow3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define HTMLWindow3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define HTMLWindow3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define HTMLWindow3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define HTMLWindow3_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define HTMLWindow3_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define HTMLWindow3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define HTMLWindow3_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define HTMLWindow3_get_CurrentView(This,pView)	\
    ( (This)->lpVtbl -> get_CurrentView(This,pView) ) 

#define HTMLWindow3_put_CurrentView(This,View)	\
    ( (This)->lpVtbl -> put_CurrentView(This,View) ) 

#define HTMLWindow3_get_CurrentPane(This,pPane)	\
    ( (This)->lpVtbl -> get_CurrentPane(This,pPane) ) 

#define HTMLWindow3_put_CurrentPane(This,Pane)	\
    ( (This)->lpVtbl -> put_CurrentPane(This,Pane) ) 

#define HTMLWindow3_WaitForBackgroundProcessingComplete(This,Task)	\
    ( (This)->lpVtbl -> WaitForBackgroundProcessingComplete(This,Task) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __HTMLWindow3_INTERFACE_DEFINED__ */


#ifndef __Debugger3_INTERFACE_DEFINED__
#define __Debugger3_INTERFACE_DEFINED__

/* interface Debugger3 */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_Debugger3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("87DFC8DA-67B4-4954-BB89-6A277A50BAFC")
    Debugger3 : public Debugger2
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_ForceContinue( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *ForceContinue) = 0;
        
        virtual /* [propput][id] */ HRESULT STDMETHODCALLTYPE put_ForceContinue( 
            /* [in] */ VARIANT_BOOL ForceContinue) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_ExceptionGroups( 
            /* [retval][out] */ __RPC__deref_out_opt ExceptionGroups **ExceptionGroups) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_SymbolPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolPath) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_SymbolPathState( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolPathState) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_SymbolCachePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolCachePath) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_OnlyLoadSymbolsManually( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *OnlyLoadSymbolsManually) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetSymbolSettings( 
            /* [in] */ __RPC__in BSTR SymbolPath,
            /* [in] */ __RPC__in BSTR SymbolPathState,
            /* [in] */ __RPC__in BSTR SymbolCachePath,
            /* [in] */ VARIANT_BOOL OnlyLoadSymbolsManually,
            /* [in] */ VARIANT_BOOL LoadSymbolsNow) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Debugger3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Debugger3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Debugger3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Debugger3 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetExpression )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExpressionText,
            /* [in][idldescattr] */ BOOLEAN UseAutoExpandRules,
            /* [in][idldescattr] */ signed long Timeout,
            /* [retval][out] */ __RPC__deref_out_opt Expression **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *DetachAll )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepInto )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepOver )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *StepOut )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Go )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Break )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Stop )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForDesignMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SetNextStatement )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *RunToCursor )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ExecuteStatement )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR Statement,
            /* [in][idldescattr] */ signed long Timeout,
            /* [in][idldescattr] */ BOOLEAN TreatAsExpression,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Breakpoints )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Languages )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Languages **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentMode )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out enum dbgDebugMode *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentProcess )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Process **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentProcess )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in_opt Process *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentProgram )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Program **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentProgram )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in_opt Program *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentThread )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Thread **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentThread )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in_opt Thread *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CurrentStackFrame )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt StackFrame **retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CurrentStackFrame )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in_opt StackFrame *noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HexDisplayMode )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_HexDisplayMode )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_HexInputMode )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_HexInputMode )( 
            Debugger3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LastBreakReason )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out enum dbgEventReason *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BreakpointLastHit )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoint **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllBreakpointsLastHit )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Breakpoints **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebuggedProcesses )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalProcesses )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *TerminateAll )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *WriteMinidump )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [in][idldescattr] */ enum dbgMinidumpOption Option,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetProcesses )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in_opt Transport *pTransport,
            /* [in][idldescattr] */ __RPC__in BSTR TransportQualifier,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetExpression2 )( 
            Debugger3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExpressionText,
            /* [in][idldescattr] */ BOOLEAN UseAutoExpandRules,
            /* [in][idldescattr] */ BOOLEAN TreatAsStatement,
            /* [in][idldescattr] */ signed long Timeout,
            /* [retval][out] */ __RPC__deref_out_opt Expression **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Transports )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Transports **retval);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ForceContinue )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *ForceContinue);
        
        /* [propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_ForceContinue )( 
            Debugger3 * This,
            /* [in] */ VARIANT_BOOL ForceContinue);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ExceptionGroups )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionGroups **ExceptionGroups);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SymbolPath )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolPath);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SymbolPathState )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolPathState);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SymbolCachePath )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolCachePath);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_OnlyLoadSymbolsManually )( 
            Debugger3 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *OnlyLoadSymbolsManually);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetSymbolSettings )( 
            Debugger3 * This,
            /* [in] */ __RPC__in BSTR SymbolPath,
            /* [in] */ __RPC__in BSTR SymbolPathState,
            /* [in] */ __RPC__in BSTR SymbolCachePath,
            /* [in] */ VARIANT_BOOL OnlyLoadSymbolsManually,
            /* [in] */ VARIANT_BOOL LoadSymbolsNow);
        
        END_INTERFACE
    } Debugger3Vtbl;

    interface Debugger3
    {
        CONST_VTBL struct Debugger3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Debugger3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Debugger3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Debugger3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Debugger3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Debugger3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Debugger3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Debugger3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Debugger3_GetExpression(This,ExpressionText,UseAutoExpandRules,Timeout,retval)	\
    ( (This)->lpVtbl -> GetExpression(This,ExpressionText,UseAutoExpandRules,Timeout,retval) ) 

#define Debugger3_DetachAll(This,retval)	\
    ( (This)->lpVtbl -> DetachAll(This,retval) ) 

#define Debugger3_StepInto(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepInto(This,WaitForBreakOrEnd,retval) ) 

#define Debugger3_StepOver(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepOver(This,WaitForBreakOrEnd,retval) ) 

#define Debugger3_StepOut(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> StepOut(This,WaitForBreakOrEnd,retval) ) 

#define Debugger3_Go(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Go(This,WaitForBreakOrEnd,retval) ) 

#define Debugger3_Break(This,WaitForBreakMode,retval)	\
    ( (This)->lpVtbl -> Break(This,WaitForBreakMode,retval) ) 

#define Debugger3_Stop(This,WaitForDesignMode,retval)	\
    ( (This)->lpVtbl -> Stop(This,WaitForDesignMode,retval) ) 

#define Debugger3_SetNextStatement(This,retval)	\
    ( (This)->lpVtbl -> SetNextStatement(This,retval) ) 

#define Debugger3_RunToCursor(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> RunToCursor(This,WaitForBreakOrEnd,retval) ) 

#define Debugger3_ExecuteStatement(This,Statement,Timeout,TreatAsExpression,retval)	\
    ( (This)->lpVtbl -> ExecuteStatement(This,Statement,Timeout,TreatAsExpression,retval) ) 

#define Debugger3_get_Breakpoints(This,retval)	\
    ( (This)->lpVtbl -> get_Breakpoints(This,retval) ) 

#define Debugger3_get_Languages(This,retval)	\
    ( (This)->lpVtbl -> get_Languages(This,retval) ) 

#define Debugger3_get_CurrentMode(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentMode(This,retval) ) 

#define Debugger3_get_CurrentProcess(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentProcess(This,retval) ) 

#define Debugger3_put_CurrentProcess(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentProcess(This,noname,retval) ) 

#define Debugger3_get_CurrentProgram(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentProgram(This,retval) ) 

#define Debugger3_put_CurrentProgram(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentProgram(This,noname,retval) ) 

#define Debugger3_get_CurrentThread(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentThread(This,retval) ) 

#define Debugger3_put_CurrentThread(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentThread(This,noname,retval) ) 

#define Debugger3_get_CurrentStackFrame(This,retval)	\
    ( (This)->lpVtbl -> get_CurrentStackFrame(This,retval) ) 

#define Debugger3_put_CurrentStackFrame(This,noname,retval)	\
    ( (This)->lpVtbl -> put_CurrentStackFrame(This,noname,retval) ) 

#define Debugger3_get_HexDisplayMode(This,retval)	\
    ( (This)->lpVtbl -> get_HexDisplayMode(This,retval) ) 

#define Debugger3_put_HexDisplayMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_HexDisplayMode(This,noname,retval) ) 

#define Debugger3_get_HexInputMode(This,retval)	\
    ( (This)->lpVtbl -> get_HexInputMode(This,retval) ) 

#define Debugger3_put_HexInputMode(This,noname,retval)	\
    ( (This)->lpVtbl -> put_HexInputMode(This,noname,retval) ) 

#define Debugger3_get_LastBreakReason(This,retval)	\
    ( (This)->lpVtbl -> get_LastBreakReason(This,retval) ) 

#define Debugger3_get_BreakpointLastHit(This,retval)	\
    ( (This)->lpVtbl -> get_BreakpointLastHit(This,retval) ) 

#define Debugger3_get_AllBreakpointsLastHit(This,retval)	\
    ( (This)->lpVtbl -> get_AllBreakpointsLastHit(This,retval) ) 

#define Debugger3_get_DebuggedProcesses(This,retval)	\
    ( (This)->lpVtbl -> get_DebuggedProcesses(This,retval) ) 

#define Debugger3_get_LocalProcesses(This,retval)	\
    ( (This)->lpVtbl -> get_LocalProcesses(This,retval) ) 

#define Debugger3_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Debugger3_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Debugger3_TerminateAll(This,retval)	\
    ( (This)->lpVtbl -> TerminateAll(This,retval) ) 

#define Debugger3_WriteMinidump(This,FileName,Option,retval)	\
    ( (This)->lpVtbl -> WriteMinidump(This,FileName,Option,retval) ) 

#define Debugger3_GetProcesses(This,pTransport,TransportQualifier,retval)	\
    ( (This)->lpVtbl -> GetProcesses(This,pTransport,TransportQualifier,retval) ) 

#define Debugger3_GetExpression2(This,ExpressionText,UseAutoExpandRules,TreatAsStatement,Timeout,retval)	\
    ( (This)->lpVtbl -> GetExpression2(This,ExpressionText,UseAutoExpandRules,TreatAsStatement,Timeout,retval) ) 

#define Debugger3_get_Transports(This,retval)	\
    ( (This)->lpVtbl -> get_Transports(This,retval) ) 


#define Debugger3_get_ForceContinue(This,ForceContinue)	\
    ( (This)->lpVtbl -> get_ForceContinue(This,ForceContinue) ) 

#define Debugger3_put_ForceContinue(This,ForceContinue)	\
    ( (This)->lpVtbl -> put_ForceContinue(This,ForceContinue) ) 

#define Debugger3_get_ExceptionGroups(This,ExceptionGroups)	\
    ( (This)->lpVtbl -> get_ExceptionGroups(This,ExceptionGroups) ) 

#define Debugger3_get_SymbolPath(This,SymbolPath)	\
    ( (This)->lpVtbl -> get_SymbolPath(This,SymbolPath) ) 

#define Debugger3_get_SymbolPathState(This,SymbolPathState)	\
    ( (This)->lpVtbl -> get_SymbolPathState(This,SymbolPathState) ) 

#define Debugger3_get_SymbolCachePath(This,SymbolCachePath)	\
    ( (This)->lpVtbl -> get_SymbolCachePath(This,SymbolCachePath) ) 

#define Debugger3_get_OnlyLoadSymbolsManually(This,OnlyLoadSymbolsManually)	\
    ( (This)->lpVtbl -> get_OnlyLoadSymbolsManually(This,OnlyLoadSymbolsManually) ) 

#define Debugger3_SetSymbolSettings(This,SymbolPath,SymbolPathState,SymbolCachePath,OnlyLoadSymbolsManually,LoadSymbolsNow)	\
    ( (This)->lpVtbl -> SetSymbolSettings(This,SymbolPath,SymbolPathState,SymbolCachePath,OnlyLoadSymbolsManually,LoadSymbolsNow) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Debugger3_INTERFACE_DEFINED__ */


#ifndef __Thread2_INTERFACE_DEFINED__
#define __Thread2_INTERFACE_DEFINED__

/* interface Thread2 */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_Thread2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("86FD0779-FBBE-41cc-B444-6EE8676F4F2C")
    Thread2 : public Thread
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Flag( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Flag) = 0;
        
        virtual /* [propput][id] */ HRESULT STDMETHODCALLTYPE put_Flag( 
            /* [in] */ VARIANT_BOOL Flag) = 0;
        
        virtual /* [propput][id] */ HRESULT STDMETHODCALLTYPE put_DisplayName( 
            /* [in] */ __RPC__in BSTR bstrName) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DisplayName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Category( 
            /* [retval][out] */ __RPC__out THREADCATEGORY *pCategory) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Thread2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Thread2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Thread2 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Thread2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Thread2 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Thread2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Freeze )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Thaw )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SuspendCount )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StackFrames )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt StackFrames **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Program )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Program **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsAlive )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Priority )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Location )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsFrozen )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Debugger **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt Threads **retval);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Flag )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Flag);
        
        /* [propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_Flag )( 
            Thread2 * This,
            /* [in] */ VARIANT_BOOL Flag);
        
        /* [propput][id] */ HRESULT ( STDMETHODCALLTYPE *put_DisplayName )( 
            Thread2 * This,
            /* [in] */ __RPC__in BSTR bstrName);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DisplayName )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrName);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Category )( 
            Thread2 * This,
            /* [retval][out] */ __RPC__out THREADCATEGORY *pCategory);
        
        END_INTERFACE
    } Thread2Vtbl;

    interface Thread2
    {
        CONST_VTBL struct Thread2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Thread2_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Thread2_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Thread2_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Thread2_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Thread2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Thread2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Thread2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Thread2_Freeze(This,retval)	\
    ( (This)->lpVtbl -> Freeze(This,retval) ) 

#define Thread2_Thaw(This,retval)	\
    ( (This)->lpVtbl -> Thaw(This,retval) ) 

#define Thread2_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Thread2_get_SuspendCount(This,retval)	\
    ( (This)->lpVtbl -> get_SuspendCount(This,retval) ) 

#define Thread2_get_ID(This,retval)	\
    ( (This)->lpVtbl -> get_ID(This,retval) ) 

#define Thread2_get_StackFrames(This,retval)	\
    ( (This)->lpVtbl -> get_StackFrames(This,retval) ) 

#define Thread2_get_Program(This,retval)	\
    ( (This)->lpVtbl -> get_Program(This,retval) ) 

#define Thread2_get_IsAlive(This,retval)	\
    ( (This)->lpVtbl -> get_IsAlive(This,retval) ) 

#define Thread2_get_Priority(This,retval)	\
    ( (This)->lpVtbl -> get_Priority(This,retval) ) 

#define Thread2_get_Location(This,retval)	\
    ( (This)->lpVtbl -> get_Location(This,retval) ) 

#define Thread2_get_IsFrozen(This,retval)	\
    ( (This)->lpVtbl -> get_IsFrozen(This,retval) ) 

#define Thread2_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Thread2_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Thread2_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 


#define Thread2_get_Flag(This,Flag)	\
    ( (This)->lpVtbl -> get_Flag(This,Flag) ) 

#define Thread2_put_Flag(This,Flag)	\
    ( (This)->lpVtbl -> put_Flag(This,Flag) ) 

#define Thread2_put_DisplayName(This,bstrName)	\
    ( (This)->lpVtbl -> put_DisplayName(This,bstrName) ) 

#define Thread2_get_DisplayName(This,pbstrName)	\
    ( (This)->lpVtbl -> get_DisplayName(This,pbstrName) ) 

#define Thread2_get_Category(This,pCategory)	\
    ( (This)->lpVtbl -> get_Category(This,pCategory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Thread2_INTERFACE_DEFINED__ */


#ifndef __Process3_INTERFACE_DEFINED__
#define __Process3_INTERFACE_DEFINED__

/* interface Process3 */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_Process3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D401C665-4EC7-452b-AA91-985D16772D84")
    Process3 : public Process2
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Modules( 
            /* [retval][out] */ __RPC__deref_out_opt Modules **Modules) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Process3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Process3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Process3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Process3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Process3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Process3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Process3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Process3 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Attach )( 
            Process3 * This,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Detach )( 
            Process3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Break )( 
            Process3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakMode,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Terminate )( 
            Process3 * This,
            /* [in][idldescattr] */ BOOLEAN WaitForBreakOrEnd,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProcessID )( 
            Process3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Programs )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Programs **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Debugger **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Processes **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Attach2 )( 
            Process3 * This,
            /* [in][idldescattr] */ VARIANT Engines,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Transport )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Transport **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TransportQualifier )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Threads )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Threads **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsBeingDebugged )( 
            Process3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_UserName )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Modules )( 
            Process3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Modules **Modules);
        
        END_INTERFACE
    } Process3Vtbl;

    interface Process3
    {
        CONST_VTBL struct Process3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Process3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Process3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Process3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Process3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Process3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Process3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Process3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Process3_Attach(This,retval)	\
    ( (This)->lpVtbl -> Attach(This,retval) ) 

#define Process3_Detach(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Detach(This,WaitForBreakOrEnd,retval) ) 

#define Process3_Break(This,WaitForBreakMode,retval)	\
    ( (This)->lpVtbl -> Break(This,WaitForBreakMode,retval) ) 

#define Process3_Terminate(This,WaitForBreakOrEnd,retval)	\
    ( (This)->lpVtbl -> Terminate(This,WaitForBreakOrEnd,retval) ) 

#define Process3_get_Name(This,retval)	\
    ( (This)->lpVtbl -> get_Name(This,retval) ) 

#define Process3_get_ProcessID(This,retval)	\
    ( (This)->lpVtbl -> get_ProcessID(This,retval) ) 

#define Process3_get_Programs(This,retval)	\
    ( (This)->lpVtbl -> get_Programs(This,retval) ) 

#define Process3_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Process3_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Process3_get_Collection(This,retval)	\
    ( (This)->lpVtbl -> get_Collection(This,retval) ) 

#define Process3_Attach2(This,Engines,retval)	\
    ( (This)->lpVtbl -> Attach2(This,Engines,retval) ) 

#define Process3_get_Transport(This,retval)	\
    ( (This)->lpVtbl -> get_Transport(This,retval) ) 

#define Process3_get_TransportQualifier(This,retval)	\
    ( (This)->lpVtbl -> get_TransportQualifier(This,retval) ) 

#define Process3_get_Threads(This,retval)	\
    ( (This)->lpVtbl -> get_Threads(This,retval) ) 

#define Process3_get_IsBeingDebugged(This,retval)	\
    ( (This)->lpVtbl -> get_IsBeingDebugged(This,retval) ) 

#define Process3_get_UserName(This,retval)	\
    ( (This)->lpVtbl -> get_UserName(This,retval) ) 


#define Process3_get_Modules(This,Modules)	\
    ( (This)->lpVtbl -> get_Modules(This,Modules) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Process3_INTERFACE_DEFINED__ */


#ifndef __Modules_INTERFACE_DEFINED__
#define __Modules_INTERFACE_DEFINED__

/* interface Modules */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_Modules;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B0F3B256-D962-4319-B7C0-A52486C16CB9")
    Modules : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Module **Module) = 0;
        
        virtual /* [restricted][id] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ModulesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Modules * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Modules * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Modules * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Modules * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Modules * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Modules * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Modules * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Modules * This,
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt Module **Module);
        
        /* [restricted][id] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Modules * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Modules * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Modules * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Modules * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        END_INTERFACE
    } ModulesVtbl;

    interface Modules
    {
        CONST_VTBL struct ModulesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Modules_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Modules_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Modules_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Modules_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Modules_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Modules_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Modules_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Modules_Item(This,Index,Module)	\
    ( (This)->lpVtbl -> Item(This,Index,Module) ) 

#define Modules__NewEnum(This,Enumerator)	\
    ( (This)->lpVtbl -> _NewEnum(This,Enumerator) ) 

#define Modules_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define Modules_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define Modules_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Modules_INTERFACE_DEFINED__ */


#ifndef __Module_INTERFACE_DEFINED__
#define __Module_INTERFACE_DEFINED__

/* interface Module */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_Module;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("72832EE4-5808-433d-83A7-B8F149A79DB4")
    Module : public IDispatch
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Path( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Path) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Optimized( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Optimized) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_UserCode( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *UserCode) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_SymbolFile( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolFile) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Order( 
            /* [retval][out] */ __RPC__out DWORD *Order) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Version( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Version) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_LoadAddress( 
            /* [retval][out] */ __RPC__out UINT64 *LoadAddress) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_EndAddress( 
            /* [retval][out] */ __RPC__out UINT64 *EndAddress) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Rebased( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Rebased) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt Modules **Modules) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Process( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Process **Process) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Is64bit( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Is64bit) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE LoadSymbols( 
            /* [in] */ __RPC__in BSTR SymbolPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ModuleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Module * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Module * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Module * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Module * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Module * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Module * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Module * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Path);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Optimized )( 
            Module * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Optimized);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_UserCode )( 
            Module * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *UserCode);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SymbolFile )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *SymbolFile);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Order )( 
            Module * This,
            /* [retval][out] */ __RPC__out DWORD *Order);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Version);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_LoadAddress )( 
            Module * This,
            /* [retval][out] */ __RPC__out UINT64 *LoadAddress);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_EndAddress )( 
            Module * This,
            /* [retval][out] */ __RPC__out UINT64 *EndAddress);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Rebased )( 
            Module * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Rebased);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt Modules **Modules);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Process )( 
            Module * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Process **Process);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Is64bit )( 
            Module * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *Is64bit);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *LoadSymbols )( 
            Module * This,
            /* [in] */ __RPC__in BSTR SymbolPath);
        
        END_INTERFACE
    } ModuleVtbl;

    interface Module
    {
        CONST_VTBL struct ModuleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Module_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Module_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Module_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Module_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Module_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Module_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Module_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Module_get_Name(This,Name)	\
    ( (This)->lpVtbl -> get_Name(This,Name) ) 

#define Module_get_Path(This,Path)	\
    ( (This)->lpVtbl -> get_Path(This,Path) ) 

#define Module_get_Optimized(This,Optimized)	\
    ( (This)->lpVtbl -> get_Optimized(This,Optimized) ) 

#define Module_get_UserCode(This,UserCode)	\
    ( (This)->lpVtbl -> get_UserCode(This,UserCode) ) 

#define Module_get_SymbolFile(This,SymbolFile)	\
    ( (This)->lpVtbl -> get_SymbolFile(This,SymbolFile) ) 

#define Module_get_Order(This,Order)	\
    ( (This)->lpVtbl -> get_Order(This,Order) ) 

#define Module_get_Version(This,Version)	\
    ( (This)->lpVtbl -> get_Version(This,Version) ) 

#define Module_get_LoadAddress(This,LoadAddress)	\
    ( (This)->lpVtbl -> get_LoadAddress(This,LoadAddress) ) 

#define Module_get_EndAddress(This,EndAddress)	\
    ( (This)->lpVtbl -> get_EndAddress(This,EndAddress) ) 

#define Module_get_Rebased(This,Rebased)	\
    ( (This)->lpVtbl -> get_Rebased(This,Rebased) ) 

#define Module_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define Module_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define Module_get_Collection(This,Modules)	\
    ( (This)->lpVtbl -> get_Collection(This,Modules) ) 

#define Module_get_Process(This,Process)	\
    ( (This)->lpVtbl -> get_Process(This,Process) ) 

#define Module_get_Is64bit(This,Is64bit)	\
    ( (This)->lpVtbl -> get_Is64bit(This,Is64bit) ) 

#define Module_LoadSymbols(This,SymbolPath)	\
    ( (This)->lpVtbl -> LoadSymbols(This,SymbolPath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Module_INTERFACE_DEFINED__ */


#ifndef __ExceptionGroups_INTERFACE_DEFINED__
#define __ExceptionGroups_INTERFACE_DEFINED__

/* interface ExceptionGroups */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_ExceptionGroups;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("21BDC491-F828-4846-9FD8-75C9148AEA24")
    ExceptionGroups : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSettings **ExceptionSettings) = 0;
        
        virtual /* [restricted][id] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE ResetAll( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ExceptionGroupsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ExceptionGroups * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ExceptionGroups * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ExceptionGroups * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ExceptionGroups * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ExceptionGroups * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ExceptionGroups * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ExceptionGroups * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            ExceptionGroups * This,
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSettings **ExceptionSettings);
        
        /* [restricted][id] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            ExceptionGroups * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ExceptionGroups * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            ExceptionGroups * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            ExceptionGroups * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *ResetAll )( 
            ExceptionGroups * This);
        
        END_INTERFACE
    } ExceptionGroupsVtbl;

    interface ExceptionGroups
    {
        CONST_VTBL struct ExceptionGroupsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ExceptionGroups_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ExceptionGroups_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ExceptionGroups_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ExceptionGroups_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ExceptionGroups_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ExceptionGroups_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ExceptionGroups_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ExceptionGroups_Item(This,Index,ExceptionSettings)	\
    ( (This)->lpVtbl -> Item(This,Index,ExceptionSettings) ) 

#define ExceptionGroups__NewEnum(This,Enumerator)	\
    ( (This)->lpVtbl -> _NewEnum(This,Enumerator) ) 

#define ExceptionGroups_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ExceptionGroups_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define ExceptionGroups_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#define ExceptionGroups_ResetAll(This)	\
    ( (This)->lpVtbl -> ResetAll(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ExceptionGroups_INTERFACE_DEFINED__ */


#ifndef __ExceptionSettings_INTERFACE_DEFINED__
#define __ExceptionSettings_INTERFACE_DEFINED__

/* interface ExceptionSettings */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_ExceptionSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0E1AB53B-4065-4884-A39F-02E16EB57F7D")
    ExceptionSettings : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting) = 0;
        
        virtual /* [restricted][id] */ HRESULT STDMETHODCALLTYPE _NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *Count) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_SupportsExceptionCodes( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *SupportsExceptionCodes) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE ItemFromCode( 
            /* [in] */ DWORD Code,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE NewException( 
            /* [in] */ __RPC__in BSTR Name,
            /* [in] */ DWORD Code,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ VARIANT Index) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RemoveByCode( 
            /* [in] */ DWORD Code) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetBreakWhenThrown( 
            /* [in] */ VARIANT_BOOL BreakWhenThrown,
            /* [in] */ __RPC__in_opt ExceptionSetting *ExceptionSetting) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetBreakWhenUserUnhandled( 
            /* [in] */ VARIANT_BOOL BreakWhenUserUnhandled,
            /* [in] */ __RPC__in_opt ExceptionSetting *ExceptionSetting) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ExceptionSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ExceptionSettings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ExceptionSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ExceptionSettings * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ExceptionSettings * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ExceptionSettings * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ExceptionSettings * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ExceptionSettings * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            ExceptionSettings * This,
            /* [in] */ VARIANT Index,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting);
        
        /* [restricted][id] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **Enumerator);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__out long *Count);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_SupportsExceptionCodes )( 
            ExceptionSettings * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *SupportsExceptionCodes);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *ItemFromCode )( 
            ExceptionSettings * This,
            /* [in] */ DWORD Code,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *NewException )( 
            ExceptionSettings * This,
            /* [in] */ __RPC__in BSTR Name,
            /* [in] */ DWORD Code,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSetting **ExceptionSetting);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            ExceptionSettings * This,
            /* [in] */ VARIANT Index);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RemoveByCode )( 
            ExceptionSettings * This,
            /* [in] */ DWORD Code);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetBreakWhenThrown )( 
            ExceptionSettings * This,
            /* [in] */ VARIANT_BOOL BreakWhenThrown,
            /* [in] */ __RPC__in_opt ExceptionSetting *ExceptionSetting);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetBreakWhenUserUnhandled )( 
            ExceptionSettings * This,
            /* [in] */ VARIANT_BOOL BreakWhenUserUnhandled,
            /* [in] */ __RPC__in_opt ExceptionSetting *ExceptionSetting);
        
        END_INTERFACE
    } ExceptionSettingsVtbl;

    interface ExceptionSettings
    {
        CONST_VTBL struct ExceptionSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ExceptionSettings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ExceptionSettings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ExceptionSettings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ExceptionSettings_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ExceptionSettings_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ExceptionSettings_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ExceptionSettings_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ExceptionSettings_Item(This,Index,ExceptionSetting)	\
    ( (This)->lpVtbl -> Item(This,Index,ExceptionSetting) ) 

#define ExceptionSettings__NewEnum(This,Enumerator)	\
    ( (This)->lpVtbl -> _NewEnum(This,Enumerator) ) 

#define ExceptionSettings_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ExceptionSettings_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define ExceptionSettings_get_Count(This,Count)	\
    ( (This)->lpVtbl -> get_Count(This,Count) ) 

#define ExceptionSettings_get_Name(This,Name)	\
    ( (This)->lpVtbl -> get_Name(This,Name) ) 

#define ExceptionSettings_get_SupportsExceptionCodes(This,SupportsExceptionCodes)	\
    ( (This)->lpVtbl -> get_SupportsExceptionCodes(This,SupportsExceptionCodes) ) 

#define ExceptionSettings_ItemFromCode(This,Code,ExceptionSetting)	\
    ( (This)->lpVtbl -> ItemFromCode(This,Code,ExceptionSetting) ) 

#define ExceptionSettings_NewException(This,Name,Code,ExceptionSetting)	\
    ( (This)->lpVtbl -> NewException(This,Name,Code,ExceptionSetting) ) 

#define ExceptionSettings_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define ExceptionSettings_RemoveByCode(This,Code)	\
    ( (This)->lpVtbl -> RemoveByCode(This,Code) ) 

#define ExceptionSettings_SetBreakWhenThrown(This,BreakWhenThrown,ExceptionSetting)	\
    ( (This)->lpVtbl -> SetBreakWhenThrown(This,BreakWhenThrown,ExceptionSetting) ) 

#define ExceptionSettings_SetBreakWhenUserUnhandled(This,BreakWhenUserUnhandled,ExceptionSetting)	\
    ( (This)->lpVtbl -> SetBreakWhenUserUnhandled(This,BreakWhenUserUnhandled,ExceptionSetting) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ExceptionSettings_INTERFACE_DEFINED__ */


#ifndef __ExceptionSetting_INTERFACE_DEFINED__
#define __ExceptionSetting_INTERFACE_DEFINED__

/* interface ExceptionSetting */
/* [object][version][dual][uuid] */ 


EXTERN_C const IID IID_ExceptionSetting;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("115998BF-A603-4848-B5DE-3B250A13D109")
    ExceptionSetting : public IDispatch
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_DTE( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Parent( 
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Collection( 
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSettings **ExceptionSettings) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_BreakWhenThrown( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenThrown) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_BreakWhenUserUnhandled( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenUserUnhandled) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_UserDefined( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *UserDefined) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Code( 
            /* [retval][out] */ __RPC__out DWORD *Code) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ExceptionSettingVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ExceptionSetting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ExceptionSetting * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ExceptionSetting * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ExceptionSetting * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ExceptionSetting * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ExceptionSetting * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ExceptionSetting * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ DTE **DTEObject);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__deref_out_opt /* external definition not present */ Debugger **Debugger);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__deref_out_opt ExceptionSettings **ExceptionSettings);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BreakWhenThrown )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenThrown);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BreakWhenUserUnhandled )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *BreakWhenUserUnhandled);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_UserDefined )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *UserDefined);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Code )( 
            ExceptionSetting * This,
            /* [retval][out] */ __RPC__out DWORD *Code);
        
        END_INTERFACE
    } ExceptionSettingVtbl;

    interface ExceptionSetting
    {
        CONST_VTBL struct ExceptionSettingVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ExceptionSetting_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ExceptionSetting_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ExceptionSetting_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ExceptionSetting_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ExceptionSetting_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ExceptionSetting_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ExceptionSetting_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ExceptionSetting_get_DTE(This,DTEObject)	\
    ( (This)->lpVtbl -> get_DTE(This,DTEObject) ) 

#define ExceptionSetting_get_Parent(This,Debugger)	\
    ( (This)->lpVtbl -> get_Parent(This,Debugger) ) 

#define ExceptionSetting_get_Collection(This,ExceptionSettings)	\
    ( (This)->lpVtbl -> get_Collection(This,ExceptionSettings) ) 

#define ExceptionSetting_get_Name(This,Name)	\
    ( (This)->lpVtbl -> get_Name(This,Name) ) 

#define ExceptionSetting_get_BreakWhenThrown(This,BreakWhenThrown)	\
    ( (This)->lpVtbl -> get_BreakWhenThrown(This,BreakWhenThrown) ) 

#define ExceptionSetting_get_BreakWhenUserUnhandled(This,BreakWhenUserUnhandled)	\
    ( (This)->lpVtbl -> get_BreakWhenUserUnhandled(This,BreakWhenUserUnhandled) ) 

#define ExceptionSetting_get_UserDefined(This,UserDefined)	\
    ( (This)->lpVtbl -> get_UserDefined(This,UserDefined) ) 

#define ExceptionSetting_get_Code(This,Code)	\
    ( (This)->lpVtbl -> get_Code(This,Code) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ExceptionSetting_INTERFACE_DEFINED__ */


#ifndef __Template_INTERFACE_DEFINED__
#define __Template_INTERFACE_DEFINED__

/* interface Template */
/* [helpstringcontext][helpstring][helpcontext][object][oleautomation][dual][uuid] */ 


EXTERN_C const IID IID_Template;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("76A0263C-083C-49f1-B312-9DB360FCC9F1")
    Template : public IDispatch
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_ID( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Description( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_FilePath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_BaseName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_CustomDataSignature( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_CustomData( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TemplateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Template * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Template * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Template * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Template * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Template * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Template * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Template * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_FilePath )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_BaseName )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CustomDataSignature )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_CustomData )( 
            Template * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrReturn);
        
        END_INTERFACE
    } TemplateVtbl;

    interface Template
    {
        CONST_VTBL struct TemplateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Template_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Template_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Template_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Template_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Template_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Template_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Template_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Template_get_ID(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_ID(This,pbstrReturn) ) 

#define Template_get_Name(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_Name(This,pbstrReturn) ) 

#define Template_get_Description(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_Description(This,pbstrReturn) ) 

#define Template_get_FilePath(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_FilePath(This,pbstrReturn) ) 

#define Template_get_BaseName(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_BaseName(This,pbstrReturn) ) 

#define Template_get_CustomDataSignature(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_CustomDataSignature(This,pbstrReturn) ) 

#define Template_get_CustomData(This,pbstrReturn)	\
    ( (This)->lpVtbl -> get_CustomData(This,pbstrReturn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Template_INTERFACE_DEFINED__ */


#ifndef __Templates_INTERFACE_DEFINED__
#define __Templates_INTERFACE_DEFINED__

/* interface Templates */
/* [helpstringcontext][helpstring][helpcontext][object][oleautomation][dual][uuid] */ 


EXTERN_C const IID IID_Templates;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30C96324-A117-4618-A9A9-0B06EC455121")
    Templates : public IDispatch
    {
    public:
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Item( 
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt Template **lppcReturn) = 0;
        
        virtual /* [propget][id] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ __RPC__out long *lplReturn) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct TemplatesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Templates * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            Templates * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            Templates * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Templates * This,
            /* [out] */ __RPC__out UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Templates * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Templates * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Templates * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            Templates * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Item )( 
            Templates * This,
            /* [in] */ long index,
            /* [retval][out] */ __RPC__deref_out_opt Template **lppcReturn);
        
        /* [propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Templates * This,
            /* [retval][out] */ __RPC__out long *lplReturn);
        
        END_INTERFACE
    } TemplatesVtbl;

    interface Templates
    {
        CONST_VTBL struct TemplatesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Templates_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define Templates_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define Templates_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define Templates_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define Templates_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define Templates_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define Templates_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define Templates_get__NewEnum(This,ppUnk)	\
    ( (This)->lpVtbl -> get__NewEnum(This,ppUnk) ) 

#define Templates_get_Item(This,index,lppcReturn)	\
    ( (This)->lpVtbl -> get_Item(This,index,lppcReturn) ) 

#define Templates_get_Count(This,lplReturn)	\
    ( (This)->lpVtbl -> get_Count(This,lplReturn) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Templates_INTERFACE_DEFINED__ */


#ifndef __Solution3_INTERFACE_DEFINED__
#define __Solution3_INTERFACE_DEFINED__

/* interface Solution3 */
/* [helpstringcontext][helpstring][helpcontext][object][oleautomation][dual][uuid] */ 


EXTERN_C const IID IID_Solution3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DF23915F-FDA3-4dd5-9CAA-2E1372C2BB16")
    Solution3 : public Solution2
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetProjectItemTemplates( 
            __RPC__in BSTR Language,
            __RPC__in BSTR CustomDataSignature,
            /* [retval][out] */ __RPC__deref_out_opt Templates **ppTemplates) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Solution3Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **ppvObj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Solution3 * This,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *pctinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Solution3 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__deref_out_opt void **pptinfo,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ __RPC__deref_in_opt signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ __RPC__out signed long *rgdispid,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Solution3 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ __RPC__in struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ __RPC__in struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ __RPC__out VARIANT *pvarResult,
            /* [out][idldescattr] */ __RPC__out struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ __RPC__out unsigned UINT *puArgErr,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            Solution3 * This,
            /* [in][idldescattr] */ VARIANT index,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *_NewEnum )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Parent )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out signed long *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *SaveAs )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFromTemplate )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [in][idldescattr] */ __RPC__in BSTR Destination,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectName,
            /* [in][idldescattr] */ BOOLEAN Exclusive,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddFromFile )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [in][idldescattr] */ BOOLEAN Exclusive,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Open )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            Solution3 * This,
            /* [in][idldescattr] */ BOOLEAN SaveFirst,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Properties )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Properties **retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsDirty )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IsDirty )( 
            Solution3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in_opt Project *proj,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TemplatePath )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ProjectType,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullName )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Saved )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Saved )( 
            Solution3 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Globals )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Globals **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AddIns )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt AddIns **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            Solution3 * This,
            /* [in][idldescattr] */ __RPC__in BSTR ExtenderName,
            /* [retval][out] */ __RPC__deref_out_opt IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IsOpen )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__out BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SolutionBuild )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt SolutionBuild **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Create )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR Destination,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__out void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Projects )( 
            Solution3 * This,
            /* [retval][out] */ __RPC__deref_out_opt Projects **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *FindProjectItem )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR FileName,
            /* [retval][out] */ __RPC__deref_out_opt ProjectItem **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *ProjectItemsTemplatePath )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR ProjectKind,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddSolutionFolder )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR Name,
            /* [retval][out] */ __RPC__deref_out_opt Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetProjectTemplate )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR TemplateName,
            /* [idldescattr] */ __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetProjectItemTemplate )( 
            Solution3 * This,
            /* [idldescattr] */ __RPC__in BSTR TemplateName,
            /* [idldescattr] */ __RPC__in BSTR Language,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *retval);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetProjectItemTemplates )( 
            Solution3 * This,
            __RPC__in BSTR Language,
            __RPC__in BSTR CustomDataSignature,
            /* [retval][out] */ __RPC__deref_out_opt Templates **ppTemplates);
        
        END_INTERFACE
    } Solution3Vtbl;

    interface Solution3
    {
        CONST_VTBL struct Solution3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Solution3_QueryInterface(This,riid,ppvObj,retval)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval) ) 

#define Solution3_AddRef(This,retval)	\
    ( (This)->lpVtbl -> AddRef(This,retval) ) 

#define Solution3_Release(This,retval)	\
    ( (This)->lpVtbl -> Release(This,retval) ) 

#define Solution3_GetTypeInfoCount(This,pctinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval) ) 

#define Solution3_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval) ) 

#define Solution3_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval) ) 

#define Solution3_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    ( (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval) ) 

#define Solution3_Item(This,index,retval)	\
    ( (This)->lpVtbl -> Item(This,index,retval) ) 

#define Solution3__NewEnum(This,retval)	\
    ( (This)->lpVtbl -> _NewEnum(This,retval) ) 

#define Solution3_get_DTE(This,retval)	\
    ( (This)->lpVtbl -> get_DTE(This,retval) ) 

#define Solution3_get_Parent(This,retval)	\
    ( (This)->lpVtbl -> get_Parent(This,retval) ) 

#define Solution3_get_Count(This,retval)	\
    ( (This)->lpVtbl -> get_Count(This,retval) ) 

#define Solution3_get_FileName(This,retval)	\
    ( (This)->lpVtbl -> get_FileName(This,retval) ) 

#define Solution3_SaveAs(This,FileName,retval)	\
    ( (This)->lpVtbl -> SaveAs(This,FileName,retval) ) 

#define Solution3_AddFromTemplate(This,FileName,Destination,ProjectName,Exclusive,retval)	\
    ( (This)->lpVtbl -> AddFromTemplate(This,FileName,Destination,ProjectName,Exclusive,retval) ) 

#define Solution3_AddFromFile(This,FileName,Exclusive,retval)	\
    ( (This)->lpVtbl -> AddFromFile(This,FileName,Exclusive,retval) ) 

#define Solution3_Open(This,FileName,retval)	\
    ( (This)->lpVtbl -> Open(This,FileName,retval) ) 

#define Solution3_Close(This,SaveFirst,retval)	\
    ( (This)->lpVtbl -> Close(This,SaveFirst,retval) ) 

#define Solution3_get_Properties(This,retval)	\
    ( (This)->lpVtbl -> get_Properties(This,retval) ) 

#define Solution3_get_IsDirty(This,retval)	\
    ( (This)->lpVtbl -> get_IsDirty(This,retval) ) 

#define Solution3_put_IsDirty(This,noname,retval)	\
    ( (This)->lpVtbl -> put_IsDirty(This,noname,retval) ) 

#define Solution3_Remove(This,proj,retval)	\
    ( (This)->lpVtbl -> Remove(This,proj,retval) ) 

#define Solution3_get_TemplatePath(This,ProjectType,retval)	\
    ( (This)->lpVtbl -> get_TemplatePath(This,ProjectType,retval) ) 

#define Solution3_get_FullName(This,retval)	\
    ( (This)->lpVtbl -> get_FullName(This,retval) ) 

#define Solution3_get_Saved(This,retval)	\
    ( (This)->lpVtbl -> get_Saved(This,retval) ) 

#define Solution3_put_Saved(This,noname,retval)	\
    ( (This)->lpVtbl -> put_Saved(This,noname,retval) ) 

#define Solution3_get_Globals(This,retval)	\
    ( (This)->lpVtbl -> get_Globals(This,retval) ) 

#define Solution3_get_AddIns(This,retval)	\
    ( (This)->lpVtbl -> get_AddIns(This,retval) ) 

#define Solution3_get_Extender(This,ExtenderName,retval)	\
    ( (This)->lpVtbl -> get_Extender(This,ExtenderName,retval) ) 

#define Solution3_get_ExtenderNames(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderNames(This,retval) ) 

#define Solution3_get_ExtenderCATID(This,retval)	\
    ( (This)->lpVtbl -> get_ExtenderCATID(This,retval) ) 

#define Solution3_get_IsOpen(This,retval)	\
    ( (This)->lpVtbl -> get_IsOpen(This,retval) ) 

#define Solution3_get_SolutionBuild(This,retval)	\
    ( (This)->lpVtbl -> get_SolutionBuild(This,retval) ) 

#define Solution3_Create(This,Destination,Name,retval)	\
    ( (This)->lpVtbl -> Create(This,Destination,Name,retval) ) 

#define Solution3_get_Projects(This,retval)	\
    ( (This)->lpVtbl -> get_Projects(This,retval) ) 

#define Solution3_FindProjectItem(This,FileName,retval)	\
    ( (This)->lpVtbl -> FindProjectItem(This,FileName,retval) ) 

#define Solution3_ProjectItemsTemplatePath(This,ProjectKind,retval)	\
    ( (This)->lpVtbl -> ProjectItemsTemplatePath(This,ProjectKind,retval) ) 

#define Solution3_AddSolutionFolder(This,Name,retval)	\
    ( (This)->lpVtbl -> AddSolutionFolder(This,Name,retval) ) 

#define Solution3_GetProjectTemplate(This,TemplateName,Language,retval)	\
    ( (This)->lpVtbl -> GetProjectTemplate(This,TemplateName,Language,retval) ) 

#define Solution3_GetProjectItemTemplate(This,TemplateName,Language,retval)	\
    ( (This)->lpVtbl -> GetProjectItemTemplate(This,TemplateName,Language,retval) ) 


#define Solution3_GetProjectItemTemplates(This,Language,CustomDataSignature,ppTemplates)	\
    ( (This)->lpVtbl -> GetProjectItemTemplates(This,Language,CustomDataSignature,ppTemplates) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __Solution3_INTERFACE_DEFINED__ */

#endif /* __EnvDTE90_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


