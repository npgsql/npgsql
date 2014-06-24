

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0361 */
/* Compiler settings for vslangproj2.idl:
    Oicf, W0, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __vslangproj2_h__
#define __vslangproj2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __VBPackageSettings_FWD_DEFINED__
#define __VBPackageSettings_FWD_DEFINED__
typedef interface VBPackageSettings VBPackageSettings;
#endif 	/* __VBPackageSettings_FWD_DEFINED__ */


#ifndef __ProjectConfigurationProperties2_FWD_DEFINED__
#define __ProjectConfigurationProperties2_FWD_DEFINED__
typedef interface ProjectConfigurationProperties2 ProjectConfigurationProperties2;
#endif 	/* __ProjectConfigurationProperties2_FWD_DEFINED__ */


#ifndef __ProjectProperties2_FWD_DEFINED__
#define __ProjectProperties2_FWD_DEFINED__
typedef interface ProjectProperties2 ProjectProperties2;
#endif 	/* __ProjectProperties2_FWD_DEFINED__ */


#ifndef __Reference2_FWD_DEFINED__
#define __Reference2_FWD_DEFINED__
typedef interface Reference2 Reference2;
#endif 	/* __Reference2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

/* interface __MIDL_itf_vslangproj2_0000 */
/* [local] */ 

#include "dte.h"
#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#define DTE VxDTE::DTE
#define Project VxDTE::Project
#define ProjectItem VxDTE::ProjectItem
#endif
#define VBProjectProperties2 ProjectProperties2
#define VBProjectConfigProperties2 ProjectConfigurationProperties2
#define IID_VBProjectProperties2 IID_ProjectProperties2
#define IID_VBProjectConfigProperties2 IID_ProjectConfigurationProperties2

enum __MIDL___MIDL_itf_vslangproj2_0000_0001
    {	VBPROJPROPID_NoStdLib	= 10066,
	VBPROJPROPID_PreBuildEvent	= 10076,
	VBPROJPROPID_PostBuildEvent	= VBPROJPROPID_PreBuildEvent + 1,
	VBPROJPROPID_RunPostBuildEvent	= VBPROJPROPID_PostBuildEvent + 1,
	VBPROJPROPID_NoWarn	= VBPROJPROPID_RunPostBuildEvent + 1,
	VBPROJPROPID_AspnetVersion	= VBPROJPROPID_NoWarn + 1
    } ;

enum __MIDL___MIDL_itf_vslangproj2_0000_0002
    {	DISPID_Reference_RuntimeVersion	= 100
    } ;
#define VBProjectProperties2 ProjectProperties2
#define VBProjectConfigProperties2 ProjectConfigurationProperties2
#define IID_VBProjectProperties2 IID_ProjectProperties2
#define IID_VBProjectConfigProperties2 IID_ProjectConfigurationProperties2
DEFINE_GUID(CATID_VJSharpFileProps, 0xe6fdf869, 0xf3d1, 0x11d4, 0x85, 0x76, 0x00, 0x02, 0xa5, 0x16, 0xec, 0xe8);
DEFINE_GUID(CATID_VJSharpFolderProps, 0xe6fdf86a, 0xf3d1, 0x11d4, 0x85, 0x76, 0x00, 0x02, 0xa5, 0x16, 0xec, 0xe8);
#define VSLANGPROJ2_VER_MAJ    7
#define VSLANGPROJ2_VER_MIN    1


extern RPC_IF_HANDLE __MIDL_itf_vslangproj2_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj2_0000_v0_0_s_ifspec;


#ifndef __VSLangProj2_LIBRARY_DEFINED__
#define __VSLangProj2_LIBRARY_DEFINED__

/* library VSLangProj2 */
/* [version][helpstring][uuid] */ 

// Enum values of project properties
typedef /* [uuid] */  DECLSPEC_UUID("48dec64c-7b34-4495-9c2d-2e4e7ca31d53") 
enum pkgCompare
    {	pkgCompareBinary	= 0,
	pkgCompareText	= pkgCompareBinary + 1
    } 	pkgCompare;

#define pkgCompareMin  pkgCompareBinary
#define pkgCompareMax  pkgCompareText
typedef /* [uuid] */  DECLSPEC_UUID("06954624-6a04-4edd-9254-b86fd55d56ef") 
enum pkgOptionExplicit
    {	pkgOptionExplicitOff	= 0,
	pkgOptionExplicitOn	= pkgOptionExplicitOff + 1
    } 	pkgOptionExplicit;

#define pkgOptionExplicitMin  pkgOptionExplicitOff
#define pkgOptionExplicitMax  pkgOptionExplicitOn
typedef /* [uuid] */  DECLSPEC_UUID("51a0b77a-9b73-487f-88a0-14b6892e3e19") 
enum pkgOptionStrict
    {	pkgOptionStrictOff	= 0,
	pkgOptionStrictOn	= pkgOptionStrictOff + 1
    } 	pkgOptionStrict;

#define pkgOptionStrictMin  pkgOptionStrictOff
#define pkgOptionStrictMax  pkgOptionStrictOn
typedef /* [uuid] */  DECLSPEC_UUID("A9DEC9CC-C687-49ca-9316-DB1B4FAE61BF") 
enum prjRunPostBuildEvent
    {	prjRunPostBuildEventAlways	= 0,
	prjRunPostBuildEventOnBuildSuccess	= prjRunPostBuildEventAlways + 1,
	prjRunPostBuildEventOnOutputUpdated	= prjRunPostBuildEventOnBuildSuccess + 1
    } 	prjRunPostBuildEvent;

#define prjRunPostBuildEventMin prjRunPostBuildEventAlways
#define prjRunPostBuildEventMax prjRunPostBuildEventOnOutputUpdated

EXTERN_C const IID LIBID_VSLangProj2;

#ifndef __VBPackageSettings_INTERFACE_DEFINED__
#define __VBPackageSettings_INTERFACE_DEFINED__

/* interface VBPackageSettings */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_VBPackageSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4e42424e-d013-4716-a7d3-47141b70432c")
    VBPackageSettings : public IDispatch
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionExplicit( 
            /* [retval][out] */ pkgOptionExplicit *pOptionExplicit) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionExplicit( 
            /* [in] */ pkgOptionExplicit optionExplicit) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionCompare( 
            /* [retval][out] */ pkgCompare *pOptionCompare) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionCompare( 
            /* [in] */ pkgCompare optionCompare) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_OptionStrict( 
            /* [retval][out] */ pkgOptionStrict *pOptionStrict) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_OptionStrict( 
            /* [in] */ pkgOptionStrict optionStrict) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct VBPackageSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            VBPackageSettings * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            VBPackageSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            VBPackageSettings * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            VBPackageSettings * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            VBPackageSettings * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            VBPackageSettings * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            VBPackageSettings * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            VBPackageSettings * This,
            /* [retval][out] */ pkgOptionExplicit *pOptionExplicit);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            VBPackageSettings * This,
            /* [in] */ pkgOptionExplicit optionExplicit);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            VBPackageSettings * This,
            /* [retval][out] */ pkgCompare *pOptionCompare);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            VBPackageSettings * This,
            /* [in] */ pkgCompare optionCompare);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            VBPackageSettings * This,
            /* [retval][out] */ pkgOptionStrict *pOptionStrict);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            VBPackageSettings * This,
            /* [in] */ pkgOptionStrict optionStrict);
        
        END_INTERFACE
    } VBPackageSettingsVtbl;

    interface VBPackageSettings
    {
        CONST_VTBL struct VBPackageSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define VBPackageSettings_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define VBPackageSettings_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define VBPackageSettings_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define VBPackageSettings_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define VBPackageSettings_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define VBPackageSettings_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define VBPackageSettings_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define VBPackageSettings_get_OptionExplicit(This,pOptionExplicit)	\
    (This)->lpVtbl -> get_OptionExplicit(This,pOptionExplicit)

#define VBPackageSettings_put_OptionExplicit(This,optionExplicit)	\
    (This)->lpVtbl -> put_OptionExplicit(This,optionExplicit)

#define VBPackageSettings_get_OptionCompare(This,pOptionCompare)	\
    (This)->lpVtbl -> get_OptionCompare(This,pOptionCompare)

#define VBPackageSettings_put_OptionCompare(This,optionCompare)	\
    (This)->lpVtbl -> put_OptionCompare(This,optionCompare)

#define VBPackageSettings_get_OptionStrict(This,pOptionStrict)	\
    (This)->lpVtbl -> get_OptionStrict(This,pOptionStrict)

#define VBPackageSettings_put_OptionStrict(This,optionStrict)	\
    (This)->lpVtbl -> put_OptionStrict(This,optionStrict)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_get_OptionExplicit_Proxy( 
    VBPackageSettings * This,
    /* [retval][out] */ pkgOptionExplicit *pOptionExplicit);


void __RPC_STUB VBPackageSettings_get_OptionExplicit_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_put_OptionExplicit_Proxy( 
    VBPackageSettings * This,
    /* [in] */ pkgOptionExplicit optionExplicit);


void __RPC_STUB VBPackageSettings_put_OptionExplicit_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_get_OptionCompare_Proxy( 
    VBPackageSettings * This,
    /* [retval][out] */ pkgCompare *pOptionCompare);


void __RPC_STUB VBPackageSettings_get_OptionCompare_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_put_OptionCompare_Proxy( 
    VBPackageSettings * This,
    /* [in] */ pkgCompare optionCompare);


void __RPC_STUB VBPackageSettings_put_OptionCompare_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_get_OptionStrict_Proxy( 
    VBPackageSettings * This,
    /* [retval][out] */ pkgOptionStrict *pOptionStrict);


void __RPC_STUB VBPackageSettings_get_OptionStrict_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE VBPackageSettings_put_OptionStrict_Proxy( 
    VBPackageSettings * This,
    /* [in] */ pkgOptionStrict optionStrict);


void __RPC_STUB VBPackageSettings_put_OptionStrict_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __VBPackageSettings_INTERFACE_DEFINED__ */


#ifndef __ProjectConfigurationProperties2_INTERFACE_DEFINED__
#define __ProjectConfigurationProperties2_INTERFACE_DEFINED__

/* interface ProjectConfigurationProperties2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_ProjectConfigurationProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CDAA65D-1E9D-11d4-B203-00C04F79CACB")
    ProjectConfigurationProperties2 : public ProjectConfigurationProperties
    {
    public:
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_NoWarn( 
            /* [retval][out] */ BSTR *pbstrWarnings) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_NoWarn( 
            /* [in] */ BSTR bstrWarnings) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_NoStdLib( 
            /* [retval][out] */ VARIANT_BOOL *pbNoStdLib) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_NoStdLib( 
            /* [in] */ VARIANT_BOOL bNoStdLib) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectConfigurationProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [out][idldescattr] */ void **ppvObj,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectConfigurationProperties2 * This,
            /* [out][idldescattr] */ unsigned UINT *pctinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ void **pptinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ signed long *rgdispid,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ VARIANT *pvarResult,
            /* [out][idldescattr] */ struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ unsigned UINT *puArgErr,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DebugSymbols )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DebugSymbols )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineDebug )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineDebug )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineTrace )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineTrace )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputPath )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputPath )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IntermediatePath )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IntermediatePath )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefineConstants )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefineConstants )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoveIntegerChecks )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoveIntegerChecks )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BaseAddress )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_BaseAddress )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AllowUnsafeBlocks )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CheckForOverflowUnderflow )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DocumentationFile )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DocumentationFile )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Optimize )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_Optimize )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_IncrementalBuild )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_IncrementalBuild )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartProgram )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartProgram )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWorkingDirectory )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWorkingDirectory )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartURL )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartURL )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartPage )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartPage )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartArguments )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartArguments )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartWithIE )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartWithIE )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableASPXDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableASPXDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableUnmanagedDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartAction )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ enum prjStartAction *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartAction )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ enum prjStartAction noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WarningLevel )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ enum prjWarningLevel *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WarningLevel )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ enum prjWarningLevel noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_TreatWarningsAsErrors )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_EnableSQLServerDebugging )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileAlignment )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileAlignment )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ unsigned long noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RegisterForComInterop )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RegisterForComInterop )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ConfigurationOverrideFile )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugEnabled )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugEnabled )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RemoteDebugMachine )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RemoteDebugMachine )( 
            ProjectConfigurationProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NoWarn )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ BSTR *pbstrWarnings);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NoWarn )( 
            ProjectConfigurationProperties2 * This,
            /* [in] */ BSTR bstrWarnings);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NoStdLib )( 
            ProjectConfigurationProperties2 * This,
            /* [retval][out] */ VARIANT_BOOL *pbNoStdLib);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NoStdLib )( 
            ProjectConfigurationProperties2 * This,
            /* [in] */ VARIANT_BOOL bNoStdLib);
        
        END_INTERFACE
    } ProjectConfigurationProperties2Vtbl;

    interface ProjectConfigurationProperties2
    {
        CONST_VTBL struct ProjectConfigurationProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectConfigurationProperties2_QueryInterface(This,riid,ppvObj,retval)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval)

#define ProjectConfigurationProperties2_AddRef(This,retval)	\
    (This)->lpVtbl -> AddRef(This,retval)

#define ProjectConfigurationProperties2_Release(This,retval)	\
    (This)->lpVtbl -> Release(This,retval)

#define ProjectConfigurationProperties2_GetTypeInfoCount(This,pctinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval)

#define ProjectConfigurationProperties2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval)

#define ProjectConfigurationProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)

#define ProjectConfigurationProperties2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)

#define ProjectConfigurationProperties2_get___id(This,retval)	\
    (This)->lpVtbl -> get___id(This,retval)

#define ProjectConfigurationProperties2_get_DebugSymbols(This,retval)	\
    (This)->lpVtbl -> get_DebugSymbols(This,retval)

#define ProjectConfigurationProperties2_put_DebugSymbols(This,noname,retval)	\
    (This)->lpVtbl -> put_DebugSymbols(This,noname,retval)

#define ProjectConfigurationProperties2_get_DefineDebug(This,retval)	\
    (This)->lpVtbl -> get_DefineDebug(This,retval)

#define ProjectConfigurationProperties2_put_DefineDebug(This,noname,retval)	\
    (This)->lpVtbl -> put_DefineDebug(This,noname,retval)

#define ProjectConfigurationProperties2_get_DefineTrace(This,retval)	\
    (This)->lpVtbl -> get_DefineTrace(This,retval)

#define ProjectConfigurationProperties2_put_DefineTrace(This,noname,retval)	\
    (This)->lpVtbl -> put_DefineTrace(This,noname,retval)

#define ProjectConfigurationProperties2_get_OutputPath(This,retval)	\
    (This)->lpVtbl -> get_OutputPath(This,retval)

#define ProjectConfigurationProperties2_put_OutputPath(This,noname,retval)	\
    (This)->lpVtbl -> put_OutputPath(This,noname,retval)

#define ProjectConfigurationProperties2_get_IntermediatePath(This,retval)	\
    (This)->lpVtbl -> get_IntermediatePath(This,retval)

#define ProjectConfigurationProperties2_put_IntermediatePath(This,noname,retval)	\
    (This)->lpVtbl -> put_IntermediatePath(This,noname,retval)

#define ProjectConfigurationProperties2_get_DefineConstants(This,retval)	\
    (This)->lpVtbl -> get_DefineConstants(This,retval)

#define ProjectConfigurationProperties2_put_DefineConstants(This,noname,retval)	\
    (This)->lpVtbl -> put_DefineConstants(This,noname,retval)

#define ProjectConfigurationProperties2_get_RemoveIntegerChecks(This,retval)	\
    (This)->lpVtbl -> get_RemoveIntegerChecks(This,retval)

#define ProjectConfigurationProperties2_put_RemoveIntegerChecks(This,noname,retval)	\
    (This)->lpVtbl -> put_RemoveIntegerChecks(This,noname,retval)

#define ProjectConfigurationProperties2_get_BaseAddress(This,retval)	\
    (This)->lpVtbl -> get_BaseAddress(This,retval)

#define ProjectConfigurationProperties2_put_BaseAddress(This,noname,retval)	\
    (This)->lpVtbl -> put_BaseAddress(This,noname,retval)

#define ProjectConfigurationProperties2_get_AllowUnsafeBlocks(This,retval)	\
    (This)->lpVtbl -> get_AllowUnsafeBlocks(This,retval)

#define ProjectConfigurationProperties2_put_AllowUnsafeBlocks(This,noname,retval)	\
    (This)->lpVtbl -> put_AllowUnsafeBlocks(This,noname,retval)

#define ProjectConfigurationProperties2_get_CheckForOverflowUnderflow(This,retval)	\
    (This)->lpVtbl -> get_CheckForOverflowUnderflow(This,retval)

#define ProjectConfigurationProperties2_put_CheckForOverflowUnderflow(This,noname,retval)	\
    (This)->lpVtbl -> put_CheckForOverflowUnderflow(This,noname,retval)

#define ProjectConfigurationProperties2_get_DocumentationFile(This,retval)	\
    (This)->lpVtbl -> get_DocumentationFile(This,retval)

#define ProjectConfigurationProperties2_put_DocumentationFile(This,noname,retval)	\
    (This)->lpVtbl -> put_DocumentationFile(This,noname,retval)

#define ProjectConfigurationProperties2_get_Optimize(This,retval)	\
    (This)->lpVtbl -> get_Optimize(This,retval)

#define ProjectConfigurationProperties2_put_Optimize(This,noname,retval)	\
    (This)->lpVtbl -> put_Optimize(This,noname,retval)

#define ProjectConfigurationProperties2_get_IncrementalBuild(This,retval)	\
    (This)->lpVtbl -> get_IncrementalBuild(This,retval)

#define ProjectConfigurationProperties2_put_IncrementalBuild(This,noname,retval)	\
    (This)->lpVtbl -> put_IncrementalBuild(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartProgram(This,retval)	\
    (This)->lpVtbl -> get_StartProgram(This,retval)

#define ProjectConfigurationProperties2_put_StartProgram(This,noname,retval)	\
    (This)->lpVtbl -> put_StartProgram(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartWorkingDirectory(This,retval)	\
    (This)->lpVtbl -> get_StartWorkingDirectory(This,retval)

#define ProjectConfigurationProperties2_put_StartWorkingDirectory(This,noname,retval)	\
    (This)->lpVtbl -> put_StartWorkingDirectory(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartURL(This,retval)	\
    (This)->lpVtbl -> get_StartURL(This,retval)

#define ProjectConfigurationProperties2_put_StartURL(This,noname,retval)	\
    (This)->lpVtbl -> put_StartURL(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartPage(This,retval)	\
    (This)->lpVtbl -> get_StartPage(This,retval)

#define ProjectConfigurationProperties2_put_StartPage(This,noname,retval)	\
    (This)->lpVtbl -> put_StartPage(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartArguments(This,retval)	\
    (This)->lpVtbl -> get_StartArguments(This,retval)

#define ProjectConfigurationProperties2_put_StartArguments(This,noname,retval)	\
    (This)->lpVtbl -> put_StartArguments(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartWithIE(This,retval)	\
    (This)->lpVtbl -> get_StartWithIE(This,retval)

#define ProjectConfigurationProperties2_put_StartWithIE(This,noname,retval)	\
    (This)->lpVtbl -> put_StartWithIE(This,noname,retval)

#define ProjectConfigurationProperties2_get_EnableASPDebugging(This,retval)	\
    (This)->lpVtbl -> get_EnableASPDebugging(This,retval)

#define ProjectConfigurationProperties2_put_EnableASPDebugging(This,noname,retval)	\
    (This)->lpVtbl -> put_EnableASPDebugging(This,noname,retval)

#define ProjectConfigurationProperties2_get_EnableASPXDebugging(This,retval)	\
    (This)->lpVtbl -> get_EnableASPXDebugging(This,retval)

#define ProjectConfigurationProperties2_put_EnableASPXDebugging(This,noname,retval)	\
    (This)->lpVtbl -> put_EnableASPXDebugging(This,noname,retval)

#define ProjectConfigurationProperties2_get_EnableUnmanagedDebugging(This,retval)	\
    (This)->lpVtbl -> get_EnableUnmanagedDebugging(This,retval)

#define ProjectConfigurationProperties2_put_EnableUnmanagedDebugging(This,noname,retval)	\
    (This)->lpVtbl -> put_EnableUnmanagedDebugging(This,noname,retval)

#define ProjectConfigurationProperties2_get_StartAction(This,retval)	\
    (This)->lpVtbl -> get_StartAction(This,retval)

#define ProjectConfigurationProperties2_put_StartAction(This,noname,retval)	\
    (This)->lpVtbl -> put_StartAction(This,noname,retval)

#define ProjectConfigurationProperties2_get_Extender(This,ExtenderName,retval)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,retval)

#define ProjectConfigurationProperties2_get_ExtenderNames(This,retval)	\
    (This)->lpVtbl -> get_ExtenderNames(This,retval)

#define ProjectConfigurationProperties2_get_ExtenderCATID(This,retval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,retval)

#define ProjectConfigurationProperties2_get_WarningLevel(This,retval)	\
    (This)->lpVtbl -> get_WarningLevel(This,retval)

#define ProjectConfigurationProperties2_put_WarningLevel(This,noname,retval)	\
    (This)->lpVtbl -> put_WarningLevel(This,noname,retval)

#define ProjectConfigurationProperties2_get_TreatWarningsAsErrors(This,retval)	\
    (This)->lpVtbl -> get_TreatWarningsAsErrors(This,retval)

#define ProjectConfigurationProperties2_put_TreatWarningsAsErrors(This,noname,retval)	\
    (This)->lpVtbl -> put_TreatWarningsAsErrors(This,noname,retval)

#define ProjectConfigurationProperties2_get_EnableSQLServerDebugging(This,retval)	\
    (This)->lpVtbl -> get_EnableSQLServerDebugging(This,retval)

#define ProjectConfigurationProperties2_put_EnableSQLServerDebugging(This,noname,retval)	\
    (This)->lpVtbl -> put_EnableSQLServerDebugging(This,noname,retval)

#define ProjectConfigurationProperties2_get_FileAlignment(This,retval)	\
    (This)->lpVtbl -> get_FileAlignment(This,retval)

#define ProjectConfigurationProperties2_put_FileAlignment(This,noname,retval)	\
    (This)->lpVtbl -> put_FileAlignment(This,noname,retval)

#define ProjectConfigurationProperties2_get_RegisterForComInterop(This,retval)	\
    (This)->lpVtbl -> get_RegisterForComInterop(This,retval)

#define ProjectConfigurationProperties2_put_RegisterForComInterop(This,noname,retval)	\
    (This)->lpVtbl -> put_RegisterForComInterop(This,noname,retval)

#define ProjectConfigurationProperties2_get_ConfigurationOverrideFile(This,retval)	\
    (This)->lpVtbl -> get_ConfigurationOverrideFile(This,retval)

#define ProjectConfigurationProperties2_put_ConfigurationOverrideFile(This,noname,retval)	\
    (This)->lpVtbl -> put_ConfigurationOverrideFile(This,noname,retval)

#define ProjectConfigurationProperties2_get_RemoteDebugEnabled(This,retval)	\
    (This)->lpVtbl -> get_RemoteDebugEnabled(This,retval)

#define ProjectConfigurationProperties2_put_RemoteDebugEnabled(This,noname,retval)	\
    (This)->lpVtbl -> put_RemoteDebugEnabled(This,noname,retval)

#define ProjectConfigurationProperties2_get_RemoteDebugMachine(This,retval)	\
    (This)->lpVtbl -> get_RemoteDebugMachine(This,retval)

#define ProjectConfigurationProperties2_put_RemoteDebugMachine(This,noname,retval)	\
    (This)->lpVtbl -> put_RemoteDebugMachine(This,noname,retval)


#define ProjectConfigurationProperties2_get_NoWarn(This,pbstrWarnings)	\
    (This)->lpVtbl -> get_NoWarn(This,pbstrWarnings)

#define ProjectConfigurationProperties2_put_NoWarn(This,bstrWarnings)	\
    (This)->lpVtbl -> put_NoWarn(This,bstrWarnings)

#define ProjectConfigurationProperties2_get_NoStdLib(This,pbNoStdLib)	\
    (This)->lpVtbl -> get_NoStdLib(This,pbNoStdLib)

#define ProjectConfigurationProperties2_put_NoStdLib(This,bNoStdLib)	\
    (This)->lpVtbl -> put_NoStdLib(This,bNoStdLib)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties2_get_NoWarn_Proxy( 
    ProjectConfigurationProperties2 * This,
    /* [retval][out] */ BSTR *pbstrWarnings);


void __RPC_STUB ProjectConfigurationProperties2_get_NoWarn_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties2_put_NoWarn_Proxy( 
    ProjectConfigurationProperties2 * This,
    /* [in] */ BSTR bstrWarnings);


void __RPC_STUB ProjectConfigurationProperties2_put_NoWarn_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [id][propget] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties2_get_NoStdLib_Proxy( 
    ProjectConfigurationProperties2 * This,
    /* [retval][out] */ VARIANT_BOOL *pbNoStdLib);


void __RPC_STUB ProjectConfigurationProperties2_get_NoStdLib_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [id][propput] */ HRESULT STDMETHODCALLTYPE ProjectConfigurationProperties2_put_NoStdLib_Proxy( 
    ProjectConfigurationProperties2 * This,
    /* [in] */ VARIANT_BOOL bNoStdLib);


void __RPC_STUB ProjectConfigurationProperties2_put_NoStdLib_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ProjectConfigurationProperties2_INTERFACE_DEFINED__ */


#ifndef __ProjectProperties2_INTERFACE_DEFINED__
#define __ProjectProperties2_INTERFACE_DEFINED__

/* interface ProjectProperties2 */
/* [object][dual][unique][helpstring][uuid] */ 


EXTERN_C const IID IID_ProjectProperties2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3CDAA65E-1E9D-11d4-B212-00C04F79CACB")
    ProjectProperties2 : public ProjectProperties
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PreBuildEvent( 
            /* [retval][out] */ BSTR *pbstrOut) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PreBuildEvent( 
            /* [in] */ BSTR bstrIn) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PostBuildEvent( 
            /* [retval][out] */ BSTR *pbstrOut) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PostBuildEvent( 
            /* [in] */ BSTR bstrIn) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_RunPostBuildEvent( 
            /* [retval][out] */ prjRunPostBuildEvent *pOut) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_RunPostBuildEvent( 
            /* [in] */ prjRunPostBuildEvent run) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AspnetVersion( 
            /* [retval][out] */ BSTR *pOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ProjectProperties2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [out][idldescattr] */ void **ppvObj,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            ProjectProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            ProjectProperties2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ProjectProperties2 * This,
            /* [out][idldescattr] */ unsigned UINT *pctinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ void **pptinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ signed long *rgdispid,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ VARIANT *pvarResult,
            /* [out][idldescattr] */ struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ unsigned UINT *puArgErr,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___id )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get___project )( 
            ProjectProperties2 * This,
            /* [retval][out] */ IUnknown **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StartupObject )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_StartupObject )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputType )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjOutputType *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OutputType )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjOutputType noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RootNamespace )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_RootNamespace )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyName )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyName )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyFile )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyFile )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyKeyContainerName )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyKeyContainerName )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AssemblyOriginatorKeyMode )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjOriginatorKeyMode *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_AssemblyOriginatorKeyMode )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjOriginatorKeyMode noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DelaySign )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DelaySign )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServer )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebServerVersion )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ServerExtensionsVersion )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LinkRepair )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_LinkRepair )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OfflineURL )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileSharePath )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileSharePath )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveFileSharePath )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_WebAccessMethod )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjWebAccessMethod *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_WebAccessMethod )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjWebAccessMethod noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveWebAccessMethod )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjWebAccessMethod *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultClientScript )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjScriptLanguage *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultClientScript )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjScriptLanguage noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultTargetSchema )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjTargetSchema *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultTargetSchema )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjTargetSchema noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultHTMLPageLayout )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjHTMLPageLayout *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultHTMLPageLayout )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjHTMLPageLayout noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_FullPath )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_LocalPath )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_URL )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ActiveConfigurationSettings )( 
            ProjectProperties2 * This,
            /* [retval][out] */ ProjectConfigurationProperties **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            ProjectProperties2 * This,
            /* [retval][out] */ VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ApplicationIcon )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ApplicationIcon )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionStrict )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjOptionStrict *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionStrict )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjOptionStrict noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ReferencePath )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_ReferencePath )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OutputFileName )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][hidden][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_AbsoluteProjectDirectory )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionExplicit )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjOptionExplicit *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionExplicit )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjOptionExplicit noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_OptionCompare )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjCompare *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_OptionCompare )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ enum prjCompare noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ProjectType )( 
            ProjectProperties2 * This,
            /* [retval][out] */ enum prjProjectType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DefaultNamespace )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_DefaultNamespace )( 
            ProjectProperties2 * This,
            /* [in][idldescattr] */ BSTR noname,
            /* [retval][out] */ void *retval);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PreBuildEvent )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *pbstrOut);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PreBuildEvent )( 
            ProjectProperties2 * This,
            /* [in] */ BSTR bstrIn);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PostBuildEvent )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *pbstrOut);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PostBuildEvent )( 
            ProjectProperties2 * This,
            /* [in] */ BSTR bstrIn);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RunPostBuildEvent )( 
            ProjectProperties2 * This,
            /* [retval][out] */ prjRunPostBuildEvent *pOut);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_RunPostBuildEvent )( 
            ProjectProperties2 * This,
            /* [in] */ prjRunPostBuildEvent run);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AspnetVersion )( 
            ProjectProperties2 * This,
            /* [retval][out] */ BSTR *pOut);
        
        END_INTERFACE
    } ProjectProperties2Vtbl;

    interface ProjectProperties2
    {
        CONST_VTBL struct ProjectProperties2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ProjectProperties2_QueryInterface(This,riid,ppvObj,retval)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval)

#define ProjectProperties2_AddRef(This,retval)	\
    (This)->lpVtbl -> AddRef(This,retval)

#define ProjectProperties2_Release(This,retval)	\
    (This)->lpVtbl -> Release(This,retval)

#define ProjectProperties2_GetTypeInfoCount(This,pctinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval)

#define ProjectProperties2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval)

#define ProjectProperties2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)

#define ProjectProperties2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)

#define ProjectProperties2_get___id(This,retval)	\
    (This)->lpVtbl -> get___id(This,retval)

#define ProjectProperties2_get___project(This,retval)	\
    (This)->lpVtbl -> get___project(This,retval)

#define ProjectProperties2_get_StartupObject(This,retval)	\
    (This)->lpVtbl -> get_StartupObject(This,retval)

#define ProjectProperties2_put_StartupObject(This,noname,retval)	\
    (This)->lpVtbl -> put_StartupObject(This,noname,retval)

#define ProjectProperties2_get_OutputType(This,retval)	\
    (This)->lpVtbl -> get_OutputType(This,retval)

#define ProjectProperties2_put_OutputType(This,noname,retval)	\
    (This)->lpVtbl -> put_OutputType(This,noname,retval)

#define ProjectProperties2_get_RootNamespace(This,retval)	\
    (This)->lpVtbl -> get_RootNamespace(This,retval)

#define ProjectProperties2_put_RootNamespace(This,noname,retval)	\
    (This)->lpVtbl -> put_RootNamespace(This,noname,retval)

#define ProjectProperties2_get_AssemblyName(This,retval)	\
    (This)->lpVtbl -> get_AssemblyName(This,retval)

#define ProjectProperties2_put_AssemblyName(This,noname,retval)	\
    (This)->lpVtbl -> put_AssemblyName(This,noname,retval)

#define ProjectProperties2_get_AssemblyOriginatorKeyFile(This,retval)	\
    (This)->lpVtbl -> get_AssemblyOriginatorKeyFile(This,retval)

#define ProjectProperties2_put_AssemblyOriginatorKeyFile(This,noname,retval)	\
    (This)->lpVtbl -> put_AssemblyOriginatorKeyFile(This,noname,retval)

#define ProjectProperties2_get_AssemblyKeyContainerName(This,retval)	\
    (This)->lpVtbl -> get_AssemblyKeyContainerName(This,retval)

#define ProjectProperties2_put_AssemblyKeyContainerName(This,noname,retval)	\
    (This)->lpVtbl -> put_AssemblyKeyContainerName(This,noname,retval)

#define ProjectProperties2_get_AssemblyOriginatorKeyMode(This,retval)	\
    (This)->lpVtbl -> get_AssemblyOriginatorKeyMode(This,retval)

#define ProjectProperties2_put_AssemblyOriginatorKeyMode(This,noname,retval)	\
    (This)->lpVtbl -> put_AssemblyOriginatorKeyMode(This,noname,retval)

#define ProjectProperties2_get_DelaySign(This,retval)	\
    (This)->lpVtbl -> get_DelaySign(This,retval)

#define ProjectProperties2_put_DelaySign(This,noname,retval)	\
    (This)->lpVtbl -> put_DelaySign(This,noname,retval)

#define ProjectProperties2_get_WebServer(This,retval)	\
    (This)->lpVtbl -> get_WebServer(This,retval)

#define ProjectProperties2_get_WebServerVersion(This,retval)	\
    (This)->lpVtbl -> get_WebServerVersion(This,retval)

#define ProjectProperties2_get_ServerExtensionsVersion(This,retval)	\
    (This)->lpVtbl -> get_ServerExtensionsVersion(This,retval)

#define ProjectProperties2_get_LinkRepair(This,retval)	\
    (This)->lpVtbl -> get_LinkRepair(This,retval)

#define ProjectProperties2_put_LinkRepair(This,noname,retval)	\
    (This)->lpVtbl -> put_LinkRepair(This,noname,retval)

#define ProjectProperties2_get_OfflineURL(This,retval)	\
    (This)->lpVtbl -> get_OfflineURL(This,retval)

#define ProjectProperties2_get_FileSharePath(This,retval)	\
    (This)->lpVtbl -> get_FileSharePath(This,retval)

#define ProjectProperties2_put_FileSharePath(This,noname,retval)	\
    (This)->lpVtbl -> put_FileSharePath(This,noname,retval)

#define ProjectProperties2_get_ActiveFileSharePath(This,retval)	\
    (This)->lpVtbl -> get_ActiveFileSharePath(This,retval)

#define ProjectProperties2_get_WebAccessMethod(This,retval)	\
    (This)->lpVtbl -> get_WebAccessMethod(This,retval)

#define ProjectProperties2_put_WebAccessMethod(This,noname,retval)	\
    (This)->lpVtbl -> put_WebAccessMethod(This,noname,retval)

#define ProjectProperties2_get_ActiveWebAccessMethod(This,retval)	\
    (This)->lpVtbl -> get_ActiveWebAccessMethod(This,retval)

#define ProjectProperties2_get_DefaultClientScript(This,retval)	\
    (This)->lpVtbl -> get_DefaultClientScript(This,retval)

#define ProjectProperties2_put_DefaultClientScript(This,noname,retval)	\
    (This)->lpVtbl -> put_DefaultClientScript(This,noname,retval)

#define ProjectProperties2_get_DefaultTargetSchema(This,retval)	\
    (This)->lpVtbl -> get_DefaultTargetSchema(This,retval)

#define ProjectProperties2_put_DefaultTargetSchema(This,noname,retval)	\
    (This)->lpVtbl -> put_DefaultTargetSchema(This,noname,retval)

#define ProjectProperties2_get_DefaultHTMLPageLayout(This,retval)	\
    (This)->lpVtbl -> get_DefaultHTMLPageLayout(This,retval)

#define ProjectProperties2_put_DefaultHTMLPageLayout(This,noname,retval)	\
    (This)->lpVtbl -> put_DefaultHTMLPageLayout(This,noname,retval)

#define ProjectProperties2_get_FileName(This,retval)	\
    (This)->lpVtbl -> get_FileName(This,retval)

#define ProjectProperties2_put_FileName(This,noname,retval)	\
    (This)->lpVtbl -> put_FileName(This,noname,retval)

#define ProjectProperties2_get_FullPath(This,retval)	\
    (This)->lpVtbl -> get_FullPath(This,retval)

#define ProjectProperties2_get_LocalPath(This,retval)	\
    (This)->lpVtbl -> get_LocalPath(This,retval)

#define ProjectProperties2_get_URL(This,retval)	\
    (This)->lpVtbl -> get_URL(This,retval)

#define ProjectProperties2_get_ActiveConfigurationSettings(This,retval)	\
    (This)->lpVtbl -> get_ActiveConfigurationSettings(This,retval)

#define ProjectProperties2_get_Extender(This,ExtenderName,retval)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,retval)

#define ProjectProperties2_get_ExtenderNames(This,retval)	\
    (This)->lpVtbl -> get_ExtenderNames(This,retval)

#define ProjectProperties2_get_ExtenderCATID(This,retval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,retval)

#define ProjectProperties2_get_ApplicationIcon(This,retval)	\
    (This)->lpVtbl -> get_ApplicationIcon(This,retval)

#define ProjectProperties2_put_ApplicationIcon(This,noname,retval)	\
    (This)->lpVtbl -> put_ApplicationIcon(This,noname,retval)

#define ProjectProperties2_get_OptionStrict(This,retval)	\
    (This)->lpVtbl -> get_OptionStrict(This,retval)

#define ProjectProperties2_put_OptionStrict(This,noname,retval)	\
    (This)->lpVtbl -> put_OptionStrict(This,noname,retval)

#define ProjectProperties2_get_ReferencePath(This,retval)	\
    (This)->lpVtbl -> get_ReferencePath(This,retval)

#define ProjectProperties2_put_ReferencePath(This,noname,retval)	\
    (This)->lpVtbl -> put_ReferencePath(This,noname,retval)

#define ProjectProperties2_get_OutputFileName(This,retval)	\
    (This)->lpVtbl -> get_OutputFileName(This,retval)

#define ProjectProperties2_get_AbsoluteProjectDirectory(This,retval)	\
    (This)->lpVtbl -> get_AbsoluteProjectDirectory(This,retval)

#define ProjectProperties2_get_OptionExplicit(This,retval)	\
    (This)->lpVtbl -> get_OptionExplicit(This,retval)

#define ProjectProperties2_put_OptionExplicit(This,noname,retval)	\
    (This)->lpVtbl -> put_OptionExplicit(This,noname,retval)

#define ProjectProperties2_get_OptionCompare(This,retval)	\
    (This)->lpVtbl -> get_OptionCompare(This,retval)

#define ProjectProperties2_put_OptionCompare(This,noname,retval)	\
    (This)->lpVtbl -> put_OptionCompare(This,noname,retval)

#define ProjectProperties2_get_ProjectType(This,retval)	\
    (This)->lpVtbl -> get_ProjectType(This,retval)

#define ProjectProperties2_get_DefaultNamespace(This,retval)	\
    (This)->lpVtbl -> get_DefaultNamespace(This,retval)

#define ProjectProperties2_put_DefaultNamespace(This,noname,retval)	\
    (This)->lpVtbl -> put_DefaultNamespace(This,noname,retval)


#define ProjectProperties2_get_PreBuildEvent(This,pbstrOut)	\
    (This)->lpVtbl -> get_PreBuildEvent(This,pbstrOut)

#define ProjectProperties2_put_PreBuildEvent(This,bstrIn)	\
    (This)->lpVtbl -> put_PreBuildEvent(This,bstrIn)

#define ProjectProperties2_get_PostBuildEvent(This,pbstrOut)	\
    (This)->lpVtbl -> get_PostBuildEvent(This,pbstrOut)

#define ProjectProperties2_put_PostBuildEvent(This,bstrIn)	\
    (This)->lpVtbl -> put_PostBuildEvent(This,bstrIn)

#define ProjectProperties2_get_RunPostBuildEvent(This,pOut)	\
    (This)->lpVtbl -> get_RunPostBuildEvent(This,pOut)

#define ProjectProperties2_put_RunPostBuildEvent(This,run)	\
    (This)->lpVtbl -> put_RunPostBuildEvent(This,run)

#define ProjectProperties2_get_AspnetVersion(This,pOut)	\
    (This)->lpVtbl -> get_AspnetVersion(This,pOut)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_get_PreBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [retval][out] */ BSTR *pbstrOut);


void __RPC_STUB ProjectProperties2_get_PreBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_put_PreBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [in] */ BSTR bstrIn);


void __RPC_STUB ProjectProperties2_put_PreBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_get_PostBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [retval][out] */ BSTR *pbstrOut);


void __RPC_STUB ProjectProperties2_get_PostBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_put_PostBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [in] */ BSTR bstrIn);


void __RPC_STUB ProjectProperties2_put_PostBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_get_RunPostBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [retval][out] */ prjRunPostBuildEvent *pOut);


void __RPC_STUB ProjectProperties2_get_RunPostBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_put_RunPostBuildEvent_Proxy( 
    ProjectProperties2 * This,
    /* [in] */ prjRunPostBuildEvent run);


void __RPC_STUB ProjectProperties2_put_RunPostBuildEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE ProjectProperties2_get_AspnetVersion_Proxy( 
    ProjectProperties2 * This,
    /* [retval][out] */ BSTR *pOut);


void __RPC_STUB ProjectProperties2_get_AspnetVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ProjectProperties2_INTERFACE_DEFINED__ */



#ifndef __PrjKind2_MODULE_DEFINED__
#define __PrjKind2_MODULE_DEFINED__


/* module PrjKind2 */
/* [uuid] */ 

/* [helpstring] */ const LPSTR prjKindSDEVBProject	=	"{CB4CE8C6-1BDB-4dc7-A4D3-65A1999772F8}";

/* [helpstring] */ const LPSTR prjKindSDECSharpProject	=	"{20D4826A-C6FA-45db-90F4-C717570B9F32}";

/* [helpstring] */ const LPSTR prjKindVJSharpProject	=	"{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";

#endif /* __PrjKind2_MODULE_DEFINED__ */


#ifndef __PrjBrowseObjectCATID2_MODULE_DEFINED__
#define __PrjBrowseObjectCATID2_MODULE_DEFINED__


/* module PrjBrowseObjectCATID2 */
/* [uuid] */ 

/* [helpstring] */ const LPSTR prjCATIDVJSharpProjectBrowseObject	=	"{E6FDF86C-F3D1-11D4-8576-0002A516ECE8}";

/* [helpstring] */ const LPSTR prjCATIDVJSharpProjectConfigBrowseObject	=	"{E6FDF86D-F3D1-11D4-8576-0002A516ECE8}";

/* [helpstring] */ const LPSTR prjCATIDVJSharpFileBrowseObject	=	"{E6FDF869-F3D1-11D4-8576-0002A516ECE8}";

/* [helpstring] */ const LPSTR prjCATIDVJSharpFolderBrowseObject	=	"{E6FDF86A-F3D1-11D4-8576-0002A516ECE8}";

/* [helpstring] */ const LPSTR prjCATIDVJSharpReferenceBrowseObject	=	"{E6FDF86E-F3D1-11D4-8576-0002A516ECE8}";

/* [helpstring] */ const LPSTR prjCATIDVJSharpConfig	=	"{E6FDF8C8-F3D1-11D4-8576-0002A516ECE8}";

#endif /* __PrjBrowseObjectCATID2_MODULE_DEFINED__ */

#ifndef __Reference2_INTERFACE_DEFINED__
#define __Reference2_INTERFACE_DEFINED__

/* interface Reference2 */
/* [uuid][object][oleautomation][dual] */ 


EXTERN_C const IID IID_Reference2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4FFF24C5-5644-4A47-A48A-B74C3F1F8FC8")
    Reference2 : public Reference
    {
    public:
        virtual /* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE get_RuntimeVersion( 
            /* [retval][out] */ BSTR *pbstrVersion) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct Reference2Vtbl
    {
        BEGIN_INTERFACE
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            Reference2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [out][idldescattr] */ void **ppvObj,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *AddRef )( 
            Reference2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Release )( 
            Reference2 * This,
            /* [retval][out] */ unsigned long *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            Reference2 * This,
            /* [out][idldescattr] */ unsigned UINT *pctinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            Reference2 * This,
            /* [in][idldescattr] */ unsigned UINT itinfo,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ void **pptinfo,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            Reference2 * This,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ signed char **rgszNames,
            /* [in][idldescattr] */ unsigned UINT cNames,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [out][idldescattr] */ signed long *rgdispid,
            /* [retval][out] */ void *retval);
        
        /* [id][restricted][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            Reference2 * This,
            /* [in][idldescattr] */ signed long dispidMember,
            /* [in][idldescattr] */ struct GUID *riid,
            /* [in][idldescattr] */ unsigned long lcid,
            /* [in][idldescattr] */ unsigned short wFlags,
            /* [in][idldescattr] */ struct DISPPARAMS *pdispparams,
            /* [out][idldescattr] */ VARIANT *pvarResult,
            /* [out][idldescattr] */ struct EXCEPINFO *pexcepinfo,
            /* [out][idldescattr] */ unsigned UINT *puArgErr,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_DTE )( 
            Reference2 * This,
            /* [retval][out] */ **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Collection )( 
            Reference2 * This,
            /* [retval][out] */ References **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ContainingProject )( 
            Reference2 * This,
            /* [retval][out] */ Project **retval);
        
        /* [id][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            Reference2 * This,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            Reference2 * This,
            /* [retval][out] */ enum prjReferenceType *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Identity )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Path )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Description )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Culture )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MajorVersion )( 
            Reference2 * This,
            /* [retval][out] */ signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_MinorVersion )( 
            Reference2 * This,
            /* [retval][out] */ signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_RevisionNumber )( 
            Reference2 * This,
            /* [retval][out] */ signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_BuildNumber )( 
            Reference2 * This,
            /* [retval][out] */ signed long *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_StrongName )( 
            Reference2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_SourceProject )( 
            Reference2 * This,
            /* [retval][out] */ Project **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_CopyLocal )( 
            Reference2 * This,
            /* [retval][out] */ BOOLEAN *retval);
        
        /* [id][propput][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *put_CopyLocal )( 
            Reference2 * This,
            /* [in][idldescattr] */ BOOLEAN noname,
            /* [retval][out] */ void *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Extender )( 
            Reference2 * This,
            /* [in][idldescattr] */ BSTR ExtenderName,
            /* [retval][out] */ IDispatch **retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderNames )( 
            Reference2 * This,
            /* [retval][out] */ VARIANT *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_ExtenderCATID )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_PublicKeyToken )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [id][propget][funcdescattr] */ HRESULT ( STDMETHODCALLTYPE *get_Version )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *retval);
        
        /* [helpstring][propget][id] */ HRESULT ( STDMETHODCALLTYPE *get_RuntimeVersion )( 
            Reference2 * This,
            /* [retval][out] */ BSTR *pbstrVersion);
        
        END_INTERFACE
    } Reference2Vtbl;

    interface Reference2
    {
        CONST_VTBL struct Reference2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define Reference2_QueryInterface(This,riid,ppvObj,retval)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObj,retval)

#define Reference2_AddRef(This,retval)	\
    (This)->lpVtbl -> AddRef(This,retval)

#define Reference2_Release(This,retval)	\
    (This)->lpVtbl -> Release(This,retval)

#define Reference2_GetTypeInfoCount(This,pctinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo,retval)

#define Reference2_GetTypeInfo(This,itinfo,lcid,pptinfo,retval)	\
    (This)->lpVtbl -> GetTypeInfo(This,itinfo,lcid,pptinfo,retval)

#define Reference2_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgdispid,retval)

#define Reference2_Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)	\
    (This)->lpVtbl -> Invoke(This,dispidMember,riid,lcid,wFlags,pdispparams,pvarResult,pexcepinfo,puArgErr,retval)

#define Reference2_get_DTE(This,retval)	\
    (This)->lpVtbl -> get_DTE(This,retval)

#define Reference2_get_Collection(This,retval)	\
    (This)->lpVtbl -> get_Collection(This,retval)

#define Reference2_get_ContainingProject(This,retval)	\
    (This)->lpVtbl -> get_ContainingProject(This,retval)

#define Reference2_Remove(This,retval)	\
    (This)->lpVtbl -> Remove(This,retval)

#define Reference2_get_Name(This,retval)	\
    (This)->lpVtbl -> get_Name(This,retval)

#define Reference2_get_Type(This,retval)	\
    (This)->lpVtbl -> get_Type(This,retval)

#define Reference2_get_Identity(This,retval)	\
    (This)->lpVtbl -> get_Identity(This,retval)

#define Reference2_get_Path(This,retval)	\
    (This)->lpVtbl -> get_Path(This,retval)

#define Reference2_get_Description(This,retval)	\
    (This)->lpVtbl -> get_Description(This,retval)

#define Reference2_get_Culture(This,retval)	\
    (This)->lpVtbl -> get_Culture(This,retval)

#define Reference2_get_MajorVersion(This,retval)	\
    (This)->lpVtbl -> get_MajorVersion(This,retval)

#define Reference2_get_MinorVersion(This,retval)	\
    (This)->lpVtbl -> get_MinorVersion(This,retval)

#define Reference2_get_RevisionNumber(This,retval)	\
    (This)->lpVtbl -> get_RevisionNumber(This,retval)

#define Reference2_get_BuildNumber(This,retval)	\
    (This)->lpVtbl -> get_BuildNumber(This,retval)

#define Reference2_get_StrongName(This,retval)	\
    (This)->lpVtbl -> get_StrongName(This,retval)

#define Reference2_get_SourceProject(This,retval)	\
    (This)->lpVtbl -> get_SourceProject(This,retval)

#define Reference2_get_CopyLocal(This,retval)	\
    (This)->lpVtbl -> get_CopyLocal(This,retval)

#define Reference2_put_CopyLocal(This,noname,retval)	\
    (This)->lpVtbl -> put_CopyLocal(This,noname,retval)

#define Reference2_get_Extender(This,ExtenderName,retval)	\
    (This)->lpVtbl -> get_Extender(This,ExtenderName,retval)

#define Reference2_get_ExtenderNames(This,retval)	\
    (This)->lpVtbl -> get_ExtenderNames(This,retval)

#define Reference2_get_ExtenderCATID(This,retval)	\
    (This)->lpVtbl -> get_ExtenderCATID(This,retval)

#define Reference2_get_PublicKeyToken(This,retval)	\
    (This)->lpVtbl -> get_PublicKeyToken(This,retval)

#define Reference2_get_Version(This,retval)	\
    (This)->lpVtbl -> get_Version(This,retval)


#define Reference2_get_RuntimeVersion(This,pbstrVersion)	\
    (This)->lpVtbl -> get_RuntimeVersion(This,pbstrVersion)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][propget][id] */ HRESULT STDMETHODCALLTYPE Reference2_get_RuntimeVersion_Proxy( 
    Reference2 * This,
    /* [retval][out] */ BSTR *pbstrVersion);


void __RPC_STUB Reference2_get_RuntimeVersion_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __Reference2_INTERFACE_DEFINED__ */

#endif /* __VSLangProj2_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_vslangproj2_0121 */
/* [local] */ 

#ifdef FORCE_EXPLICIT_DTE_NAMESPACE
#undef DTE
#undef Project
#undef ProjectItem
#endif


extern RPC_IF_HANDLE __MIDL_itf_vslangproj2_0121_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_vslangproj2_0121_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


