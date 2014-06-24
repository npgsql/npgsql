

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for textfind.idl:
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


#ifndef __textfind_h__
#define __textfind_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsTextSpanSet_FWD_DEFINED__
#define __IVsTextSpanSet_FWD_DEFINED__
typedef interface IVsTextSpanSet IVsTextSpanSet;
#endif 	/* __IVsTextSpanSet_FWD_DEFINED__ */


#ifndef __IVsFinder_FWD_DEFINED__
#define __IVsFinder_FWD_DEFINED__
typedef interface IVsFinder IVsFinder;
#endif 	/* __IVsFinder_FWD_DEFINED__ */


#ifndef __IVsFindHelper_FWD_DEFINED__
#define __IVsFindHelper_FWD_DEFINED__
typedef interface IVsFindHelper IVsFindHelper;
#endif 	/* __IVsFindHelper_FWD_DEFINED__ */


#ifndef __IVsFindTarget_FWD_DEFINED__
#define __IVsFindTarget_FWD_DEFINED__
typedef interface IVsFindTarget IVsFindTarget;
#endif 	/* __IVsFindTarget_FWD_DEFINED__ */


#ifndef __IVsFindScope_FWD_DEFINED__
#define __IVsFindScope_FWD_DEFINED__
typedef interface IVsFindScope IVsFindScope;
#endif 	/* __IVsFindScope_FWD_DEFINED__ */


#ifndef __IVsRegisterFindScope_FWD_DEFINED__
#define __IVsRegisterFindScope_FWD_DEFINED__
typedef interface IVsRegisterFindScope IVsRegisterFindScope;
#endif 	/* __IVsRegisterFindScope_FWD_DEFINED__ */


#ifndef __IVsHighlight_FWD_DEFINED__
#define __IVsHighlight_FWD_DEFINED__
typedef interface IVsHighlight IVsHighlight;
#endif 	/* __IVsHighlight_FWD_DEFINED__ */


#ifndef __IVsTextImageUtilities_FWD_DEFINED__
#define __IVsTextImageUtilities_FWD_DEFINED__
typedef interface IVsTextImageUtilities IVsTextImageUtilities;
#endif 	/* __IVsTextImageUtilities_FWD_DEFINED__ */


#ifndef __VsFinder_FWD_DEFINED__
#define __VsFinder_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsFinder VsFinder;
#else
typedef struct VsFinder VsFinder;
#endif /* __cplusplus */

#endif 	/* __VsFinder_FWD_DEFINED__ */


#ifndef __VsTextSpanSet_FWD_DEFINED__
#define __VsTextSpanSet_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextSpanSet VsTextSpanSet;
#else
typedef struct VsTextSpanSet VsTextSpanSet;
#endif /* __cplusplus */

#endif 	/* __VsTextSpanSet_FWD_DEFINED__ */


#ifndef __VsTextImage_FWD_DEFINED__
#define __VsTextImage_FWD_DEFINED__

#ifdef __cplusplus
typedef class VsTextImage VsTextImage;
#else
typedef struct VsTextImage VsTextImage;
#endif /* __cplusplus */

#endif 	/* __VsTextImage_FWD_DEFINED__ */


/* header files for imported files */
#include "vsshell.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textfind_0000_0000 */
/* [local] */ 









enum __VSFINDOPTIONS
    {	FR_OptionMask	= 0xfff,
	FR_MatchCase	= 0x1,
	FR_WholeWord	= 0x2,
	FR_Hidden	= 0x4,
	FR_Backwards	= 0x8,
	FR_Selection	= 0x10,
	FR_Block	= 0x20,
	FR_KeepCase	= 0x40,
	FR_SubFolders	= 0x100,
	FR_KeepOpen	= 0x200,
	FR_NameOnly	= 0x400,
	FR_SyntaxMask	= 0x3000,
	FR_Plain	= 0,
	FR_Wildcard	= 0x1000,
	FR_RegExpr	= 0x2000,
	FR_TargetMask	= 0x70000,
	FR_Document	= 0x10000,
	FR_OpenDocuments	= 0x20000,
	FR_Files	= 0x30000,
	FR_Project	= 0x40000,
	FR_Solution	= 0x50000,
	FR_ActionMask	= 0xf80000,
	FR_MarkAll	= 0x80000,
	FR_Find	= 0x100000,
	FR_FindAll	= 0x200000,
	FR_Replace	= 0x400000,
	FR_ReplaceAll	= 0x800000,
	FR_InternalMask	= 0x7f000000,
	FR_ResetPosition	= 0x1000000,
	FR_FromStart	= 0x2000000,
	FR_OneMatchPerLine	= 0x4000000,
	FR_Report	= 0x8000000,
	FR_SelectionDefault	= 0x10000000,
	FR_NoFind	= 0x80000000,
	FR_CommonOptions	= ( ( ( FR_MatchCase | FR_WholeWord )  | FR_Hidden )  | FR_KeepCase ) ,
	FR_DocOptionSet	= ( ( ( FR_CommonOptions | FR_Backwards )  | FR_Selection )  | FR_Block ) ,
	FR_FIFOptionSet	= ( ( ( FR_CommonOptions | FR_SubFolders )  | FR_KeepOpen )  | FR_NameOnly ) ,
	FR_OptionSet	= ( FR_DocOptionSet | FR_FIFOptionSet ) ,
	FR_SyntaxSet	= ( ( FR_Plain | FR_Wildcard )  | FR_RegExpr ) ,
	FR_Capabilities	= ( ( ( FR_OptionSet | FR_SyntaxSet )  | FR_ActionMask )  | FR_SelectionDefault ) ,
	FR_All	= 0x7fffffff,
	FR_None	= 0,
	FR_Procedure	= FR_Block,
	FR_Pattern	= FR_Wildcard
    } ;
typedef DWORD VSFINDOPTIONS;

#ifndef __RE_E_DEFINED__
#define __RE_E_DEFINED__

enum RE_ERRORS
    {	RE_E_INTERNALERROR	= ( ( 0x80000000 | 0x40000 )  | 0x1 ) ,
	RE_E_SYNTAXERROR	= ( ( 0x80000000 | 0x40000 )  | 0x2 ) ,
	RE_E_STACKOVERFLOW	= ( ( 0x80000000 | 0x40000 )  | 0x3 ) ,
	RE_E_MISSINGARG	= ( ( 0x80000000 | 0x40000 )  | 0x4 ) ,
	RE_E_POWERARGOUTOFRANGE	= ( ( 0x80000000 | 0x40000 )  | 0x5 ) ,
	RE_E_ESCAPEMISSINGARG	= ( ( 0x80000000 | 0x40000 )  | 0x6 ) ,
	RE_E_SPECIALUNKNOWN	= ( ( 0x80000000 | 0x40000 )  | 0x7 ) ,
	RE_E_TAGOUTOFRANGE	= ( ( 0x80000000 | 0x40000 )  | 0x8 ) ,
	RE_E_SETMISSINGCLOSE	= ( ( 0x80000000 | 0x40000 )  | 0x9 ) ,
	RE_E_TAGMISSINGCLOSE	= ( ( 0x80000000 | 0x40000 )  | 0xa ) ,
	RE_E_TOOMANYTAGS	= ( ( 0x80000000 | 0x40000 )  | 0xb ) ,
	RE_E_EMPTYSET	= ( ( 0x80000000 | 0x40000 )  | 0xc ) ,
	RE_E_GROUPMISSINGCLOSE	= ( ( 0x80000000 | 0x40000 )  | 0xd ) ,
	RE_E_REPLACETEXT	= ( ( 0x80000000 | 0x40000 )  | 0xe ) 
    } ;
#endif // __RE_E_DEFINED__

enum __VSFINDBUFFERFLAGS
    {	VSFB_StartOfLine	= 0x1,
	VSFB_EndOfLine	= 0x2,
	FINDBUFFER_StartOfLine	= 0x1,
	FINDBUFFER_EndOfLine	= 0x2
    } ;
typedef DWORD VSFINDBUFFERFLAGS;


enum __VSFINDRESULTLOC
    {	VSFRL_FindResultList1	= 1,
	VSFRL_FindResultList2	= 2,
	FINDREPLACE_RESULTLOC_ResultsList1	= 1,
	FINDREPLACE_RESULTLOC_ResultsList2	= 2
    } ;
typedef DWORD VSFINDRESULTLOC;


enum __VSFINDRESULT
    {	VSFR_NotFound	= 0,
	VSFR_Found	= 1,
	VSFR_Replaced	= 2,
	VSFR_EndOfDoc	= 3,
	VSFR_EndOfSearch	= 4,
	VSFR_FindInFiles	= 5,
	VSFR_NoTarget	= 6,
	VSFR_Error	= 7,
	VSFR_AndInterrupt	= 0x40000000,
	VSFR_AndReplace	= 0x80000000,
	VSFR_FLAGMASK	= 0xff000000,
	FINDREPLACE_RESULT_Failed	= 0,
	FINDREPLACE_RESULT_Found	= 1,
	FINDREPLACE_RESULT_Replaced	= 2,
	FINDREPLACE_RESULT_EndOfDoc	= 3,
	FINDREPLACE_RESULT_EndOfSearch	= 4,
	FINDREPLACE_RESULT_FindInFiles	= 5
    } ;
typedef DWORD VSFINDRESULT;


enum __VSFINDPOS
    {	VSFP_Start	= 0x1,
	VSFP_Current	= 0x2,
	VSFP_Wrapped	= 0x10000,
	VSFP_ResetCounts	= 0x20000
    } ;
typedef DWORD VSFINDPOS;


enum __VSFINDSTATE
    {	VSFS_Error	= 0x10000000,
	VSFS_Empty	= 0,
	VSFS_Start	= 0x1,
	VSFS_Found	= 0x2,
	VSFS_NotFound	= 0x3,
	VSFS_Wrapped	= 0x10000
    } ;
typedef DWORD VSFINDSTATE;


// Helper macros for looking at find state
//
#define VSFS_IsFound(fs) (VSFS_Found == (fs & 0xf))
#define VSFS_IsWrapped(fs) (VSFS_Wrapped & fs)


enum __VSFINDERROR
    {	VSFE_Syntax	= -4,
	VSFE_NotSupported	= -3,
	VSFE_NotInitialized	= -2,
	VSFE_Failure	= -1,
	VSFE_NoError	= 0
    } ;
typedef DWORD VSFINDERROR;


enum __VSFINDHOW
    {	VSFH_Ambient	= 0,
	VSFH_Wrap	= 0x1,
	VSFH_TryMatch	= 0x10000000,
	VSFH_SetMatch	= 0x20000000
    } ;
typedef DWORD VSFINDHOW;


enum __VSFTPROPID
    {	VSFTPROPID_DocName	= 0,
	VSFTPROPID_IsDiskFile	= 1,
	VSFTPROPID_BlockName	= 2,
	VSFTPROPID_InitialPattern	= 3,
	VSFTPROPID_InitialPatternAggressive	= 4,
	VSFTPROPID_WindowFrame	= 5
    } ;
typedef DWORD VSFTPROPID;


enum __VSFTNOTIFY
    {	VSFTNOTIFY_CURRENT	= 0,
	VSFTNOTIFY_NOTCURRENT	= 1,
	VSFTNOTIFY_STARTFIND	= 2,
	VSFTNOTIFY_ENDFIND	= 3
    } ;
typedef DWORD VSFTNOTIFY;



extern RPC_IF_HANDLE __MIDL_itf_textfind_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textfind_0000_0000_v0_0_s_ifspec;


#ifndef __TextFind_LIBRARY_DEFINED__
#define __TextFind_LIBRARY_DEFINED__

/* library TextFind */
/* [uuid] */ 

#define SID_SVsTextSpanSet IID_IVsTextSpanSet
#define SID_SVsRegisterFindScope IID_IVsRegisterFindScope
#define SID_SVsTextImageUtilities IID_IVsTextImageUtilities

EXTERN_C const IID LIBID_TextFind;

#ifndef __IVsTextSpanSet_INTERFACE_DEFINED__
#define __IVsTextSpanSet_INTERFACE_DEFINED__

/* interface IVsTextSpanSet */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTextSpanSet;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E6BBDFD0-C06A-11d2-936C-D714766E8B50")
    IVsTextSpanSet : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AttachTextImage( 
            /* [in] */ __RPC__in_opt IUnknown *pText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SuspendTracking( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResumeTracking( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ LONG cel,
            /* [size_is][in] */ __RPC__in_ecount_full(cel) const TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ __RPC__out LONG *pcel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAt( 
            /* [in] */ LONG iEl,
            /* [retval][out] */ __RPC__out TextSpan *pSpan) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveAll( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Sort( 
            /* [in] */ DWORD SortOptions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddFromEnum( 
            /* [in] */ __RPC__in_opt IVsEnumTextSpans *pEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextSpanSetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextSpanSet * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextSpanSet * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextSpanSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *AttachTextImage )( 
            IVsTextSpanSet * This,
            /* [in] */ __RPC__in_opt IUnknown *pText);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IVsTextSpanSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *SuspendTracking )( 
            IVsTextSpanSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResumeTracking )( 
            IVsTextSpanSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *Add )( 
            IVsTextSpanSet * This,
            /* [in] */ LONG cel,
            /* [size_is][in] */ __RPC__in_ecount_full(cel) const TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVsTextSpanSet * This,
            /* [retval][out] */ __RPC__out LONG *pcel);
        
        HRESULT ( STDMETHODCALLTYPE *GetAt )( 
            IVsTextSpanSet * This,
            /* [in] */ LONG iEl,
            /* [retval][out] */ __RPC__out TextSpan *pSpan);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveAll )( 
            IVsTextSpanSet * This);
        
        HRESULT ( STDMETHODCALLTYPE *Sort )( 
            IVsTextSpanSet * This,
            /* [in] */ DWORD SortOptions);
        
        HRESULT ( STDMETHODCALLTYPE *AddFromEnum )( 
            IVsTextSpanSet * This,
            /* [in] */ __RPC__in_opt IVsEnumTextSpans *pEnum);
        
        END_INTERFACE
    } IVsTextSpanSetVtbl;

    interface IVsTextSpanSet
    {
        CONST_VTBL struct IVsTextSpanSetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextSpanSet_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextSpanSet_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextSpanSet_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextSpanSet_AttachTextImage(This,pText)	\
    ( (This)->lpVtbl -> AttachTextImage(This,pText) ) 

#define IVsTextSpanSet_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IVsTextSpanSet_SuspendTracking(This)	\
    ( (This)->lpVtbl -> SuspendTracking(This) ) 

#define IVsTextSpanSet_ResumeTracking(This)	\
    ( (This)->lpVtbl -> ResumeTracking(This) ) 

#define IVsTextSpanSet_Add(This,cel,pSpan)	\
    ( (This)->lpVtbl -> Add(This,cel,pSpan) ) 

#define IVsTextSpanSet_GetCount(This,pcel)	\
    ( (This)->lpVtbl -> GetCount(This,pcel) ) 

#define IVsTextSpanSet_GetAt(This,iEl,pSpan)	\
    ( (This)->lpVtbl -> GetAt(This,iEl,pSpan) ) 

#define IVsTextSpanSet_RemoveAll(This)	\
    ( (This)->lpVtbl -> RemoveAll(This) ) 

#define IVsTextSpanSet_Sort(This,SortOptions)	\
    ( (This)->lpVtbl -> Sort(This,SortOptions) ) 

#define IVsTextSpanSet_AddFromEnum(This,pEnum)	\
    ( (This)->lpVtbl -> AddFromEnum(This,pEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextSpanSet_INTERFACE_DEFINED__ */


#ifndef __IVsFinder_INTERFACE_DEFINED__
#define __IVsFinder_INTERFACE_DEFINED__

/* interface IVsFinder */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFinder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D0C79C30-C06A-11d2-936C-D714766E8B50")
    IVsFinder : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AttachTextImage( 
            /* [in] */ __RPC__in_opt IUnknown *pTextImage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Detach( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetScope( 
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pSpanScope) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Init( 
            /* [in] */ DWORD grfOptions,
            /* [in] */ __RPC__in LPCOLESTR pszFindPattern,
            /* [in] */ BOOL fReinit,
            /* [retval][out] */ __RPC__out VSFINDERROR *pResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPosition( 
            /* [in] */ VSFINDPOS fp,
            /* [in] */ TextAddress ta) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Find( 
            /* [in] */ VSFINDHOW grfFindHow,
            /* [out][in] */ __RPC__inout TextSpan *ptsMatch,
            /* [retval][out] */ __RPC__out VSFINDSTATE *pgrfResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMatch( 
            /* [retval][out] */ __RPC__out TextSpan *pts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMatchedSpans( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppSpans) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTaggedSpans( 
            /* [retval][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppTags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetState( 
            /* [out] */ __RPC__out LONG *pcFound,
            /* [out] */ __RPC__out VSFINDERROR *pResult,
            /* [retval][out] */ __RPC__out VSFINDSTATE *pState) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFinderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFinder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFinder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFinder * This);
        
        HRESULT ( STDMETHODCALLTYPE *AttachTextImage )( 
            IVsFinder * This,
            /* [in] */ __RPC__in_opt IUnknown *pTextImage);
        
        HRESULT ( STDMETHODCALLTYPE *Detach )( 
            IVsFinder * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetScope )( 
            IVsFinder * This,
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pSpanScope);
        
        HRESULT ( STDMETHODCALLTYPE *Init )( 
            IVsFinder * This,
            /* [in] */ DWORD grfOptions,
            /* [in] */ __RPC__in LPCOLESTR pszFindPattern,
            /* [in] */ BOOL fReinit,
            /* [retval][out] */ __RPC__out VSFINDERROR *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *SetPosition )( 
            IVsFinder * This,
            /* [in] */ VSFINDPOS fp,
            /* [in] */ TextAddress ta);
        
        HRESULT ( STDMETHODCALLTYPE *Find )( 
            IVsFinder * This,
            /* [in] */ VSFINDHOW grfFindHow,
            /* [out][in] */ __RPC__inout TextSpan *ptsMatch,
            /* [retval][out] */ __RPC__out VSFINDSTATE *pgrfResult);
        
        HRESULT ( STDMETHODCALLTYPE *GetMatch )( 
            IVsFinder * This,
            /* [retval][out] */ __RPC__out TextSpan *pts);
        
        HRESULT ( STDMETHODCALLTYPE *GetMatchedSpans )( 
            IVsFinder * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppSpans);
        
        HRESULT ( STDMETHODCALLTYPE *GetTaggedSpans )( 
            IVsFinder * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppTags);
        
        HRESULT ( STDMETHODCALLTYPE *GetState )( 
            IVsFinder * This,
            /* [out] */ __RPC__out LONG *pcFound,
            /* [out] */ __RPC__out VSFINDERROR *pResult,
            /* [retval][out] */ __RPC__out VSFINDSTATE *pState);
        
        END_INTERFACE
    } IVsFinderVtbl;

    interface IVsFinder
    {
        CONST_VTBL struct IVsFinderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFinder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFinder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFinder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFinder_AttachTextImage(This,pTextImage)	\
    ( (This)->lpVtbl -> AttachTextImage(This,pTextImage) ) 

#define IVsFinder_Detach(This)	\
    ( (This)->lpVtbl -> Detach(This) ) 

#define IVsFinder_SetScope(This,pSpanScope)	\
    ( (This)->lpVtbl -> SetScope(This,pSpanScope) ) 

#define IVsFinder_Init(This,grfOptions,pszFindPattern,fReinit,pResult)	\
    ( (This)->lpVtbl -> Init(This,grfOptions,pszFindPattern,fReinit,pResult) ) 

#define IVsFinder_SetPosition(This,fp,ta)	\
    ( (This)->lpVtbl -> SetPosition(This,fp,ta) ) 

#define IVsFinder_Find(This,grfFindHow,ptsMatch,pgrfResult)	\
    ( (This)->lpVtbl -> Find(This,grfFindHow,ptsMatch,pgrfResult) ) 

#define IVsFinder_GetMatch(This,pts)	\
    ( (This)->lpVtbl -> GetMatch(This,pts) ) 

#define IVsFinder_GetMatchedSpans(This,ppSpans)	\
    ( (This)->lpVtbl -> GetMatchedSpans(This,ppSpans) ) 

#define IVsFinder_GetTaggedSpans(This,ppTags)	\
    ( (This)->lpVtbl -> GetTaggedSpans(This,ppTags) ) 

#define IVsFinder_GetState(This,pcFound,pResult,pState)	\
    ( (This)->lpVtbl -> GetState(This,pcFound,pResult,pState) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFinder_INTERFACE_DEFINED__ */


#ifndef __IVsFindHelper_INTERFACE_DEFINED__
#define __IVsFindHelper_INTERFACE_DEFINED__

/* interface IVsFindHelper */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindHelper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BB02E777-EC3E-479f-980E-8E36B9617DCA")
    IVsFindHelper : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE FindInText( 
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ VSFINDOPTIONS grfFindOptions,
            /* [in] */ VSFINDBUFFERFLAGS grfBufferFlags,
            /* [in] */ ULONG cchText,
            /* [size_is][in] */ __RPC__in_ecount_full(cchText) LPCOLESTR pchText,
            /* [out] */ __RPC__out ULONG *piFound,
            /* [out] */ __RPC__out ULONG *pcchFound,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrReplaceText,
            /* [retval][out] */ __RPC__out BOOL *pfFound) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindHelperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindHelper * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindHelper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *FindInText )( 
            IVsFindHelper * This,
            /* [in] */ __RPC__in LPCOLESTR pszFind,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ VSFINDOPTIONS grfFindOptions,
            /* [in] */ VSFINDBUFFERFLAGS grfBufferFlags,
            /* [in] */ ULONG cchText,
            /* [size_is][in] */ __RPC__in_ecount_full(cchText) LPCOLESTR pchText,
            /* [out] */ __RPC__out ULONG *piFound,
            /* [out] */ __RPC__out ULONG *pcchFound,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrReplaceText,
            /* [retval][out] */ __RPC__out BOOL *pfFound);
        
        END_INTERFACE
    } IVsFindHelperVtbl;

    interface IVsFindHelper
    {
        CONST_VTBL struct IVsFindHelperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindHelper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindHelper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindHelper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindHelper_FindInText(This,pszFind,pszReplace,grfFindOptions,grfBufferFlags,cchText,pchText,piFound,pcchFound,pbstrReplaceText,pfFound)	\
    ( (This)->lpVtbl -> FindInText(This,pszFind,pszReplace,grfFindOptions,grfBufferFlags,cchText,pchText,piFound,pcchFound,pbstrReplaceText,pfFound) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindHelper_INTERFACE_DEFINED__ */


#ifndef __IVsFindTarget_INTERFACE_DEFINED__
#define __IVsFindTarget_INTERFACE_DEFINED__

/* interface IVsFindTarget */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindTarget;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DE89D360-C06A-11d2-936C-D714766E8B50")
    IVsFindTarget : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCapabilities( 
            /* [custom][out] */ __RPC__out BOOL *pfImage,
            /* [out] */ __RPC__out VSFINDOPTIONS *pgrfOptions) = 0;
        
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE GetProperty( 
            /* [in] */ VSFTPROPID propid,
            /* [retval][out] */ __RPC__out VARIANT *pvar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSearchImage( 
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [custom][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppSpans,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextImage **ppTextImage) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Find( 
            /* [in] */ __RPC__in LPCOLESTR pszSearch,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ BOOL fResetStartPoint,
            /* [in] */ __RPC__in_opt IVsFindHelper *pHelper,
            /* [retval][out] */ __RPC__out VSFINDRESULT *pResult) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace( 
            /* [in] */ __RPC__in LPCOLESTR pszSearch,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ BOOL fResetStartPoint,
            /* [in] */ __RPC__in_opt IVsFindHelper *pHelper,
            /* [retval][out] */ __RPC__out BOOL *pfReplaced) = 0;
        
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE GetMatchRect( 
            /* [retval][out] */ __RPC__out PRECT prc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateTo( 
            /* [in] */ __RPC__in const TextSpan *pts) = 0;
        
        virtual /* [custom] */ HRESULT STDMETHODCALLTYPE GetCurrentSpan( 
            /* [retval][out] */ __RPC__out TextSpan *pts) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetFindState( 
            /* [in] */ __RPC__in_opt IUnknown *punk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFindState( 
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NotifyFindTarget( 
            /* [in] */ VSFTNOTIFY notification) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MarkSpan( 
            /* [in] */ __RPC__in const TextSpan *pts) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindTargetVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindTarget * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindTarget * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCapabilities )( 
            IVsFindTarget * This,
            /* [custom][out] */ __RPC__out BOOL *pfImage,
            /* [out] */ __RPC__out VSFINDOPTIONS *pgrfOptions);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *GetProperty )( 
            IVsFindTarget * This,
            /* [in] */ VSFTPROPID propid,
            /* [retval][out] */ __RPC__out VARIANT *pvar);
        
        HRESULT ( STDMETHODCALLTYPE *GetSearchImage )( 
            IVsFindTarget * This,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [custom][out] */ __RPC__deref_out_opt IVsTextSpanSet **ppSpans,
            /* [retval][out] */ __RPC__deref_out_opt IVsTextImage **ppTextImage);
        
        HRESULT ( STDMETHODCALLTYPE *Find )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in LPCOLESTR pszSearch,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ BOOL fResetStartPoint,
            /* [in] */ __RPC__in_opt IVsFindHelper *pHelper,
            /* [retval][out] */ __RPC__out VSFINDRESULT *pResult);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in LPCOLESTR pszSearch,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ BOOL fResetStartPoint,
            /* [in] */ __RPC__in_opt IVsFindHelper *pHelper,
            /* [retval][out] */ __RPC__out BOOL *pfReplaced);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *GetMatchRect )( 
            IVsFindTarget * This,
            /* [retval][out] */ __RPC__out PRECT prc);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateTo )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in const TextSpan *pts);
        
        /* [custom] */ HRESULT ( STDMETHODCALLTYPE *GetCurrentSpan )( 
            IVsFindTarget * This,
            /* [retval][out] */ __RPC__out TextSpan *pts);
        
        HRESULT ( STDMETHODCALLTYPE *SetFindState )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in_opt IUnknown *punk);
        
        HRESULT ( STDMETHODCALLTYPE *GetFindState )( 
            IVsFindTarget * This,
            /* [retval][out] */ __RPC__deref_out_opt IUnknown **ppunk);
        
        HRESULT ( STDMETHODCALLTYPE *NotifyFindTarget )( 
            IVsFindTarget * This,
            /* [in] */ VSFTNOTIFY notification);
        
        HRESULT ( STDMETHODCALLTYPE *MarkSpan )( 
            IVsFindTarget * This,
            /* [in] */ __RPC__in const TextSpan *pts);
        
        END_INTERFACE
    } IVsFindTargetVtbl;

    interface IVsFindTarget
    {
        CONST_VTBL struct IVsFindTargetVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindTarget_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindTarget_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindTarget_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindTarget_GetCapabilities(This,pfImage,pgrfOptions)	\
    ( (This)->lpVtbl -> GetCapabilities(This,pfImage,pgrfOptions) ) 

#define IVsFindTarget_GetProperty(This,propid,pvar)	\
    ( (This)->lpVtbl -> GetProperty(This,propid,pvar) ) 

#define IVsFindTarget_GetSearchImage(This,grfOptions,ppSpans,ppTextImage)	\
    ( (This)->lpVtbl -> GetSearchImage(This,grfOptions,ppSpans,ppTextImage) ) 

#define IVsFindTarget_Find(This,pszSearch,grfOptions,fResetStartPoint,pHelper,pResult)	\
    ( (This)->lpVtbl -> Find(This,pszSearch,grfOptions,fResetStartPoint,pHelper,pResult) ) 

#define IVsFindTarget_Replace(This,pszSearch,pszReplace,grfOptions,fResetStartPoint,pHelper,pfReplaced)	\
    ( (This)->lpVtbl -> Replace(This,pszSearch,pszReplace,grfOptions,fResetStartPoint,pHelper,pfReplaced) ) 

#define IVsFindTarget_GetMatchRect(This,prc)	\
    ( (This)->lpVtbl -> GetMatchRect(This,prc) ) 

#define IVsFindTarget_NavigateTo(This,pts)	\
    ( (This)->lpVtbl -> NavigateTo(This,pts) ) 

#define IVsFindTarget_GetCurrentSpan(This,pts)	\
    ( (This)->lpVtbl -> GetCurrentSpan(This,pts) ) 

#define IVsFindTarget_SetFindState(This,punk)	\
    ( (This)->lpVtbl -> SetFindState(This,punk) ) 

#define IVsFindTarget_GetFindState(This,ppunk)	\
    ( (This)->lpVtbl -> GetFindState(This,ppunk) ) 

#define IVsFindTarget_NotifyFindTarget(This,notification)	\
    ( (This)->lpVtbl -> NotifyFindTarget(This,notification) ) 

#define IVsFindTarget_MarkSpan(This,pts)	\
    ( (This)->lpVtbl -> MarkSpan(This,pts) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindTarget_INTERFACE_DEFINED__ */


#ifndef __IVsFindScope_INTERFACE_DEFINED__
#define __IVsFindScope_INTERFACE_DEFINED__

/* interface IVsFindScope */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsFindScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0FE9496A-129C-40ee-99D8-7705A6283518")
    IVsFindScope : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUIName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbsName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetQuery( 
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBaseDirectory,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrQuery) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumFilenames( 
            /* [retval][out] */ __RPC__deref_out_opt IEnumString **ppEnumString) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsFindScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsFindScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsFindScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsFindScope * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUIName )( 
            IVsFindScope * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbsName);
        
        HRESULT ( STDMETHODCALLTYPE *GetQuery )( 
            IVsFindScope * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pbstrBaseDirectory,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrQuery);
        
        HRESULT ( STDMETHODCALLTYPE *EnumFilenames )( 
            IVsFindScope * This,
            /* [retval][out] */ __RPC__deref_out_opt IEnumString **ppEnumString);
        
        END_INTERFACE
    } IVsFindScopeVtbl;

    interface IVsFindScope
    {
        CONST_VTBL struct IVsFindScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsFindScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsFindScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsFindScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsFindScope_GetUIName(This,pbsName)	\
    ( (This)->lpVtbl -> GetUIName(This,pbsName) ) 

#define IVsFindScope_GetQuery(This,pbstrBaseDirectory,pbstrQuery)	\
    ( (This)->lpVtbl -> GetQuery(This,pbstrBaseDirectory,pbstrQuery) ) 

#define IVsFindScope_EnumFilenames(This,ppEnumString)	\
    ( (This)->lpVtbl -> EnumFilenames(This,ppEnumString) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsFindScope_INTERFACE_DEFINED__ */


#ifndef __IVsRegisterFindScope_INTERFACE_DEFINED__
#define __IVsRegisterFindScope_INTERFACE_DEFINED__

/* interface IVsRegisterFindScope */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterFindScope;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("08363D65-C178-4484-A293-D148173DD750")
    IVsRegisterFindScope : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterFindScope( 
            /* [in] */ __RPC__in_opt IVsFindScope *pScope,
            /* [out] */ __RPC__out DWORD_PTR *pdwCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnRegisterFindScope( 
            /* [in] */ DWORD_PTR dwCookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterFindScopeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterFindScope * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterFindScope * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterFindScope * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterFindScope )( 
            IVsRegisterFindScope * This,
            /* [in] */ __RPC__in_opt IVsFindScope *pScope,
            /* [out] */ __RPC__out DWORD_PTR *pdwCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnRegisterFindScope )( 
            IVsRegisterFindScope * This,
            /* [in] */ DWORD_PTR dwCookie);
        
        END_INTERFACE
    } IVsRegisterFindScopeVtbl;

    interface IVsRegisterFindScope
    {
        CONST_VTBL struct IVsRegisterFindScopeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterFindScope_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterFindScope_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterFindScope_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterFindScope_RegisterFindScope(This,pScope,pdwCookie)	\
    ( (This)->lpVtbl -> RegisterFindScope(This,pScope,pdwCookie) ) 

#define IVsRegisterFindScope_UnRegisterFindScope(This,dwCookie)	\
    ( (This)->lpVtbl -> UnRegisterFindScope(This,dwCookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterFindScope_INTERFACE_DEFINED__ */


#ifndef __IVsHighlight_INTERFACE_DEFINED__
#define __IVsHighlight_INTERFACE_DEFINED__

/* interface IVsHighlight */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsHighlight;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("22212CD2-83FF-11d2-92E6-005345000000")
    IVsHighlight : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHighlightRect( 
            /* [out] */ __RPC__out RECT *prc) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsHighlightVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsHighlight * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsHighlight * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsHighlight * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHighlightRect )( 
            IVsHighlight * This,
            /* [out] */ __RPC__out RECT *prc);
        
        END_INTERFACE
    } IVsHighlightVtbl;

    interface IVsHighlight
    {
        CONST_VTBL struct IVsHighlightVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHighlight_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHighlight_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHighlight_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHighlight_GetHighlightRect(This,prc)	\
    ( (This)->lpVtbl -> GetHighlightRect(This,prc) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHighlight_INTERFACE_DEFINED__ */


#ifndef __IVsTextImageUtilities_INTERFACE_DEFINED__
#define __IVsTextImageUtilities_INTERFACE_DEFINED__

/* interface IVsTextImageUtilities */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsTextImageUtilities;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C5557BAA-B685-46dc-AF46-C16BFDE8FAB7")
    IVsTextImageUtilities : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetReplaceText( 
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ __RPC__in_opt IVsTextImage *pText,
            /* [in] */ __RPC__in const TextSpan *pMatch,
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pTags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrComputedText) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadTextFile( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ VSTFF vstffIn,
            /* [out] */ __RPC__out VSTFF *pvstffOut,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadTextImageFromFile( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveTextImageToFile( 
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextFormat( 
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [size_is][in] */ __RPC__in_ecount_full(cbData) const BYTE *pData,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadTextImageFromMemory( 
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [size_is][in] */ __RPC__in_ecount_full(cbData) const BYTE *pData,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveTextImageToMemory( 
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cbData, *pcbWritten) BYTE *pData,
            /* [out] */ __RPC__out DWORD *pcbWritten,
            /* [out] */ __RPC__out VSTFF *pvstffOut) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsTextImageUtilitiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsTextImageUtilities * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsTextImageUtilities * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetReplaceText )( 
            IVsTextImageUtilities * This,
            /* [in] */ VSFINDOPTIONS grfOptions,
            /* [in] */ __RPC__in LPCOLESTR pszReplace,
            /* [in] */ __RPC__in_opt IVsTextImage *pText,
            /* [in] */ __RPC__in const TextSpan *pMatch,
            /* [in] */ __RPC__in_opt IVsTextSpanSet *pTags,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstrComputedText);
        
        HRESULT ( STDMETHODCALLTYPE *LoadTextFile )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ VSTFF vstffIn,
            /* [out] */ __RPC__out VSTFF *pvstffOut,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pbstr);
        
        HRESULT ( STDMETHODCALLTYPE *LoadTextImageFromFile )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut);
        
        HRESULT ( STDMETHODCALLTYPE *SaveTextImageToFile )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in LPCOLESTR pszFilename,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextFormat )( 
            IVsTextImageUtilities * This,
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [size_is][in] */ __RPC__in_ecount_full(cbData) const BYTE *pData,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut);
        
        HRESULT ( STDMETHODCALLTYPE *LoadTextImageFromMemory )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [size_is][in] */ __RPC__in_ecount_full(cbData) const BYTE *pData,
            /* [retval][out] */ __RPC__out VSTFF *pvstffOut);
        
        HRESULT ( STDMETHODCALLTYPE *SaveTextImageToMemory )( 
            IVsTextImageUtilities * This,
            /* [in] */ __RPC__in_opt IVsTextImage *pImage,
            /* [in] */ VSTFF vstffIn,
            /* [in] */ DWORD cbData,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(cbData, *pcbWritten) BYTE *pData,
            /* [out] */ __RPC__out DWORD *pcbWritten,
            /* [out] */ __RPC__out VSTFF *pvstffOut);
        
        END_INTERFACE
    } IVsTextImageUtilitiesVtbl;

    interface IVsTextImageUtilities
    {
        CONST_VTBL struct IVsTextImageUtilitiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextImageUtilities_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextImageUtilities_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextImageUtilities_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextImageUtilities_GetReplaceText(This,grfOptions,pszReplace,pText,pMatch,pTags,pbstrComputedText)	\
    ( (This)->lpVtbl -> GetReplaceText(This,grfOptions,pszReplace,pText,pMatch,pTags,pbstrComputedText) ) 

#define IVsTextImageUtilities_LoadTextFile(This,pszFilename,vstffIn,pvstffOut,pbstr)	\
    ( (This)->lpVtbl -> LoadTextFile(This,pszFilename,vstffIn,pvstffOut,pbstr) ) 

#define IVsTextImageUtilities_LoadTextImageFromFile(This,pszFilename,pImage,vstffIn,pvstffOut)	\
    ( (This)->lpVtbl -> LoadTextImageFromFile(This,pszFilename,pImage,vstffIn,pvstffOut) ) 

#define IVsTextImageUtilities_SaveTextImageToFile(This,pszFilename,pImage,vstffIn,pvstffOut)	\
    ( (This)->lpVtbl -> SaveTextImageToFile(This,pszFilename,pImage,vstffIn,pvstffOut) ) 

#define IVsTextImageUtilities_GetTextFormat(This,vstffIn,cbData,pData,pvstffOut)	\
    ( (This)->lpVtbl -> GetTextFormat(This,vstffIn,cbData,pData,pvstffOut) ) 

#define IVsTextImageUtilities_LoadTextImageFromMemory(This,pImage,vstffIn,cbData,pData,pvstffOut)	\
    ( (This)->lpVtbl -> LoadTextImageFromMemory(This,pImage,vstffIn,cbData,pData,pvstffOut) ) 

#define IVsTextImageUtilities_SaveTextImageToMemory(This,pImage,vstffIn,cbData,pData,pcbWritten,pvstffOut)	\
    ( (This)->lpVtbl -> SaveTextImageToMemory(This,pImage,vstffIn,cbData,pData,pcbWritten,pvstffOut) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextImageUtilities_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_VsFinder;

#ifdef __cplusplus

class DECLSPEC_UUID("9F842E53-E27B-426f-AE69-8F075770C6F3")
VsFinder;
#endif

EXTERN_C const CLSID CLSID_VsTextSpanSet;

#ifdef __cplusplus

class DECLSPEC_UUID("31A81214-5A9F-4532-ACD0-3204A9AB5D08")
VsTextSpanSet;
#endif

EXTERN_C const CLSID CLSID_VsTextImage;

#ifdef __cplusplus

class DECLSPEC_UUID("66B88230-2363-4992-B740-B0450A6F51CA")
VsTextImage;
#endif
#endif /* __TextFind_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_textfind_0001_0223 */
/* [local] */ 

const GUID GUID_FindReplace   = { 0xcf2ddc32, 0x8cad, 0x11d2, { 0x93, 0x2, 0x0, 0x53, 0x45, 0x0, 0x0, 0x0 } };
const GUID GUID_FindAdvanced = { 0xe830ec50, 0xc2b5, 0x11d2, { 0x93, 0x75, 0x0, 0x80, 0xc7, 0x47, 0xd9, 0xa0 } };
const GUID GUID_FindResults1  = { 0xf887920, 0xc2b6, 0x11d2, { 0x93, 0x75, 0x0, 0x80, 0xc7, 0x47, 0xd9, 0xa0 } };
const GUID GUID_FindResults2  = { 0xf887921, 0xc2b6, 0x11d2, { 0x93, 0x75, 0x0, 0x80, 0xc7, 0x47, 0xd9, 0xa0 } };


extern RPC_IF_HANDLE __MIDL_itf_textfind_0001_0223_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textfind_0001_0223_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


