

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for undoredo.idl:
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

#ifndef __undoredo_h__
#define __undoredo_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IAction_FWD_DEFINED__
#define __IAction_FWD_DEFINED__
typedef interface IAction IAction;
#endif 	/* __IAction_FWD_DEFINED__ */


#ifndef __IEnumActions_FWD_DEFINED__
#define __IEnumActions_FWD_DEFINED__
typedef interface IEnumActions IEnumActions;
#endif 	/* __IEnumActions_FWD_DEFINED__ */


#ifndef __IActionHistory_FWD_DEFINED__
#define __IActionHistory_FWD_DEFINED__
typedef interface IActionHistory IActionHistory;
#endif 	/* __IActionHistory_FWD_DEFINED__ */


#ifndef __ActionHistory_FWD_DEFINED__
#define __ActionHistory_FWD_DEFINED__

#ifdef __cplusplus
typedef class ActionHistory ActionHistory;
#else
typedef struct ActionHistory ActionHistory;
#endif /* __cplusplus */

#endif 	/* __ActionHistory_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IAction_INTERFACE_DEFINED__
#define __IAction_INTERFACE_DEFINED__

/* interface IAction */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D8C8AB11-7D4A-11d0-A8AA-00A0C921A4D2")
    IAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            __RPC__deref_in_opt BSTR *bstrName,
            BOOL fLongName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSize( 
            __RPC__in long *piSize) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Undo( 
            __RPC__in_opt IUnknown *pObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Redo( 
            __RPC__in_opt IUnknown *pObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Abort( 
            __RPC__in_opt IUnknown *pObject) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddSibling( 
            __RPC__in_opt IAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNextSibling( 
            __RPC__deref_in_opt IAction **ppAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddChild( 
            __RPC__in_opt IAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFirstChild( 
            __RPC__deref_in_opt IAction **ppAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanMerge( 
            __RPC__in_opt IAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Merge( 
            __RPC__in_opt IAction *pAction) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            IAction * This,
            __RPC__deref_in_opt BSTR *bstrName,
            BOOL fLongName);
        
        HRESULT ( STDMETHODCALLTYPE *GetSize )( 
            IAction * This,
            __RPC__in long *piSize);
        
        HRESULT ( STDMETHODCALLTYPE *Undo )( 
            IAction * This,
            __RPC__in_opt IUnknown *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *Redo )( 
            IAction * This,
            __RPC__in_opt IUnknown *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *Abort )( 
            IAction * This,
            __RPC__in_opt IUnknown *pObject);
        
        HRESULT ( STDMETHODCALLTYPE *AddSibling )( 
            IAction * This,
            __RPC__in_opt IAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *GetNextSibling )( 
            IAction * This,
            __RPC__deref_in_opt IAction **ppAction);
        
        HRESULT ( STDMETHODCALLTYPE *AddChild )( 
            IAction * This,
            __RPC__in_opt IAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *GetFirstChild )( 
            IAction * This,
            __RPC__deref_in_opt IAction **ppAction);
        
        HRESULT ( STDMETHODCALLTYPE *CanMerge )( 
            IAction * This,
            __RPC__in_opt IAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *Merge )( 
            IAction * This,
            __RPC__in_opt IAction *pAction);
        
        END_INTERFACE
    } IActionVtbl;

    interface IAction
    {
        CONST_VTBL struct IActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAction_GetName(This,bstrName,fLongName)	\
    ( (This)->lpVtbl -> GetName(This,bstrName,fLongName) ) 

#define IAction_GetSize(This,piSize)	\
    ( (This)->lpVtbl -> GetSize(This,piSize) ) 

#define IAction_Undo(This,pObject)	\
    ( (This)->lpVtbl -> Undo(This,pObject) ) 

#define IAction_Redo(This,pObject)	\
    ( (This)->lpVtbl -> Redo(This,pObject) ) 

#define IAction_Abort(This,pObject)	\
    ( (This)->lpVtbl -> Abort(This,pObject) ) 

#define IAction_AddSibling(This,pAction)	\
    ( (This)->lpVtbl -> AddSibling(This,pAction) ) 

#define IAction_GetNextSibling(This,ppAction)	\
    ( (This)->lpVtbl -> GetNextSibling(This,ppAction) ) 

#define IAction_AddChild(This,pAction)	\
    ( (This)->lpVtbl -> AddChild(This,pAction) ) 

#define IAction_GetFirstChild(This,ppAction)	\
    ( (This)->lpVtbl -> GetFirstChild(This,ppAction) ) 

#define IAction_CanMerge(This,pAction)	\
    ( (This)->lpVtbl -> CanMerge(This,pAction) ) 

#define IAction_Merge(This,pAction)	\
    ( (This)->lpVtbl -> Merge(This,pAction) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAction_INTERFACE_DEFINED__ */


#ifndef __IEnumActions_INTERFACE_DEFINED__
#define __IEnumActions_INTERFACE_DEFINED__

/* interface IEnumActions */
/* [object][uuid] */ 


EXTERN_C const IID IID_IEnumActions;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7A6335C8-7884-11d0-A8A9-00A0C921A4D2")
    IEnumActions : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            ULONG celt,
            __RPC__deref_in_opt IAction **rgelt,
            __RPC__in ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            __RPC__deref_in_opt IEnumActions **ppenum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumActionsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumActions * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumActions * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumActions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IEnumActions * This,
            ULONG celt,
            __RPC__deref_in_opt IAction **rgelt,
            __RPC__in ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IEnumActions * This,
            ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumActions * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IEnumActions * This,
            __RPC__deref_in_opt IEnumActions **ppenum);
        
        END_INTERFACE
    } IEnumActionsVtbl;

    interface IEnumActions
    {
        CONST_VTBL struct IEnumActionsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumActions_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumActions_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumActions_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumActions_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IEnumActions_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IEnumActions_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IEnumActions_Clone(This,ppenum)	\
    ( (This)->lpVtbl -> Clone(This,ppenum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumActions_INTERFACE_DEFINED__ */


#ifndef __IActionHistory_INTERFACE_DEFINED__
#define __IActionHistory_INTERFACE_DEFINED__

/* interface IActionHistory */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IActionHistory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8D5DE85B-7D42-11D0-A8AA-00A0C921A4D2")
    IActionHistory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RecordAction( 
            /* [in] */ __RPC__in_opt IAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenAction( 
            /* [in] */ __RPC__in_opt IAction *pAction) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AbortAction( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanUndo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CanRedo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Undo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Redo( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumUndoActions( 
            __RPC__deref_in_opt IEnumActions **ppEnumUndoActions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumRedoActions( 
            __RPC__deref_in_opt IEnumActions **ppEnumRedoActions) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IActionHistoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IActionHistory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IActionHistory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *RecordAction )( 
            IActionHistory * This,
            /* [in] */ __RPC__in_opt IAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *OpenAction )( 
            IActionHistory * This,
            /* [in] */ __RPC__in_opt IAction *pAction);
        
        HRESULT ( STDMETHODCALLTYPE *CloseAction )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *AbortAction )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanUndo )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CanRedo )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *Undo )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *Redo )( 
            IActionHistory * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumUndoActions )( 
            IActionHistory * This,
            __RPC__deref_in_opt IEnumActions **ppEnumUndoActions);
        
        HRESULT ( STDMETHODCALLTYPE *EnumRedoActions )( 
            IActionHistory * This,
            __RPC__deref_in_opt IEnumActions **ppEnumRedoActions);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IActionHistory * This);
        
        END_INTERFACE
    } IActionHistoryVtbl;

    interface IActionHistory
    {
        CONST_VTBL struct IActionHistoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IActionHistory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IActionHistory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IActionHistory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IActionHistory_RecordAction(This,pAction)	\
    ( (This)->lpVtbl -> RecordAction(This,pAction) ) 

#define IActionHistory_OpenAction(This,pAction)	\
    ( (This)->lpVtbl -> OpenAction(This,pAction) ) 

#define IActionHistory_CloseAction(This)	\
    ( (This)->lpVtbl -> CloseAction(This) ) 

#define IActionHistory_AbortAction(This)	\
    ( (This)->lpVtbl -> AbortAction(This) ) 

#define IActionHistory_CanUndo(This)	\
    ( (This)->lpVtbl -> CanUndo(This) ) 

#define IActionHistory_CanRedo(This)	\
    ( (This)->lpVtbl -> CanRedo(This) ) 

#define IActionHistory_Undo(This)	\
    ( (This)->lpVtbl -> Undo(This) ) 

#define IActionHistory_Redo(This)	\
    ( (This)->lpVtbl -> Redo(This) ) 

#define IActionHistory_EnumUndoActions(This,ppEnumUndoActions)	\
    ( (This)->lpVtbl -> EnumUndoActions(This,ppEnumUndoActions) ) 

#define IActionHistory_EnumRedoActions(This,ppEnumRedoActions)	\
    ( (This)->lpVtbl -> EnumRedoActions(This,ppEnumRedoActions) ) 

#define IActionHistory_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IActionHistory_INTERFACE_DEFINED__ */



#ifndef __UNDOREDOLib_LIBRARY_DEFINED__
#define __UNDOREDOLib_LIBRARY_DEFINED__

/* library UNDOREDOLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_UNDOREDOLib;

EXTERN_C const CLSID CLSID_ActionHistory;

#ifdef __cplusplus

class DECLSPEC_UUID("F5E7E71F-1401-11d1-883B-0000F87579D2")
ActionHistory;
#endif
#endif /* __UNDOREDOLib_LIBRARY_DEFINED__ */

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


