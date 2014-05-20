

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


#ifndef __textmgr100_h__
#define __textmgr100_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsMethodTipWindow3_FWD_DEFINED__
#define __IVsMethodTipWindow3_FWD_DEFINED__
typedef interface IVsMethodTipWindow3 IVsMethodTipWindow3;

#endif 	/* __IVsMethodTipWindow3_FWD_DEFINED__ */


#ifndef __IVsTextTipWindow2_FWD_DEFINED__
#define __IVsTextTipWindow2_FWD_DEFINED__
typedef interface IVsTextTipWindow2 IVsTextTipWindow2;

#endif 	/* __IVsTextTipWindow2_FWD_DEFINED__ */


#ifndef __IVsSmartTagTipWindow2_FWD_DEFINED__
#define __IVsSmartTagTipWindow2_FWD_DEFINED__
typedef interface IVsSmartTagTipWindow2 IVsSmartTagTipWindow2;

#endif 	/* __IVsSmartTagTipWindow2_FWD_DEFINED__ */


#ifndef __IVsHiddenTextSessionEx2_FWD_DEFINED__
#define __IVsHiddenTextSessionEx2_FWD_DEFINED__
typedef interface IVsHiddenTextSessionEx2 IVsHiddenTextSessionEx2;

#endif 	/* __IVsHiddenTextSessionEx2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "textmgr.h"
#include "textmgr2.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_textmgr100_0000_0000 */
/* [local] */ 

#pragma once


extern RPC_IF_HANDLE __MIDL_itf_textmgr100_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_textmgr100_0000_0000_v0_0_s_ifspec;


#ifndef __TextMgr100_LIBRARY_DEFINED__
#define __TextMgr100_LIBRARY_DEFINED__

/* library TextMgr100 */
/* [version][uuid] */ 

typedef 
enum _TextViewInitFlags3
    {
        VIF_NO_HWND_SUPPORT	= 0x8000
    } 	TextViewInitFlags3;

extern const __declspec(selectany) GUID GUID_VsBufferContentType = { 0x1beb4195, 0x98f4, 0x4589, { 0x80, 0xe0, 0x48, 0xc, 0xe3, 0x2f, 0xf0, 0x59 } };
extern const __declspec(selectany) GUID GUID_VsTextViewRoles = { 0x297078ff, 0x81a2, 0x43d8, { 0x9c, 0xa3, 0x44, 0x89, 0xc5, 0x3c, 0x99, 0xba } };
extern const __declspec(selectany) GUID GUID_UseLazyInitialization = { 0xfea19c13, 0x32ce, 0x447b, { 0x8c, 0xc3, 0x72, 0x0d, 0xdf, 0x13, 0x8b, 0xb8 } };

EXTERN_C const IID LIBID_TextMgr100;

#ifndef __IVsMethodTipWindow3_INTERFACE_DEFINED__
#define __IVsMethodTipWindow3_INTERFACE_DEFINED__

/* interface IVsMethodTipWindow3 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsMethodTipWindow3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5C17E526-0F7B-40FC-8E94-E12ADC618A02")
    IVsMethodTipWindow3 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetMethodData( 
            /* [out] */ __RPC__deref_out_opt IVsMethodData **pMethodData) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsMethodTipWindow3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsMethodTipWindow3 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsMethodTipWindow3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsMethodTipWindow3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetMethodData )( 
            __RPC__in IVsMethodTipWindow3 * This,
            /* [out] */ __RPC__deref_out_opt IVsMethodData **pMethodData);
        
        END_INTERFACE
    } IVsMethodTipWindow3Vtbl;

    interface IVsMethodTipWindow3
    {
        CONST_VTBL struct IVsMethodTipWindow3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsMethodTipWindow3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsMethodTipWindow3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsMethodTipWindow3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsMethodTipWindow3_GetMethodData(This,pMethodData)	\
    ( (This)->lpVtbl -> GetMethodData(This,pMethodData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsMethodTipWindow3_INTERFACE_DEFINED__ */


#ifndef __IVsTextTipWindow2_INTERFACE_DEFINED__
#define __IVsTextTipWindow2_INTERFACE_DEFINED__

/* interface IVsTextTipWindow2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsTextTipWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("085F0AB4-5518-4898-8D4A-48E4D3E177DD")
    IVsTextTipWindow2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextTipData( 
            /* [out] */ __RPC__deref_out_opt IVsTextTipData **pMethodData) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsTextTipWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsTextTipWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsTextTipWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsTextTipWindow2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextTipData )( 
            __RPC__in IVsTextTipWindow2 * This,
            /* [out] */ __RPC__deref_out_opt IVsTextTipData **pMethodData);
        
        END_INTERFACE
    } IVsTextTipWindow2Vtbl;

    interface IVsTextTipWindow2
    {
        CONST_VTBL struct IVsTextTipWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsTextTipWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsTextTipWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsTextTipWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsTextTipWindow2_GetTextTipData(This,pMethodData)	\
    ( (This)->lpVtbl -> GetTextTipData(This,pMethodData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsTextTipWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsSmartTagTipWindow2_INTERFACE_DEFINED__
#define __IVsSmartTagTipWindow2_INTERFACE_DEFINED__

/* interface IVsSmartTagTipWindow2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsSmartTagTipWindow2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4C5B474B-B539-4A60-9819-82A5BED76C60")
    IVsSmartTagTipWindow2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSmartTagData( 
            /* [out] */ __RPC__deref_out_opt IVsSmartTagData **pSmartTagData) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsSmartTagTipWindow2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsSmartTagTipWindow2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsSmartTagTipWindow2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsSmartTagTipWindow2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSmartTagData )( 
            __RPC__in IVsSmartTagTipWindow2 * This,
            /* [out] */ __RPC__deref_out_opt IVsSmartTagData **pSmartTagData);
        
        END_INTERFACE
    } IVsSmartTagTipWindow2Vtbl;

    interface IVsSmartTagTipWindow2
    {
        CONST_VTBL struct IVsSmartTagTipWindow2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsSmartTagTipWindow2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsSmartTagTipWindow2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsSmartTagTipWindow2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsSmartTagTipWindow2_GetSmartTagData(This,pSmartTagData)	\
    ( (This)->lpVtbl -> GetSmartTagData(This,pSmartTagData) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsSmartTagTipWindow2_INTERFACE_DEFINED__ */


#ifndef __IVsHiddenTextSessionEx2_INTERFACE_DEFINED__
#define __IVsHiddenTextSessionEx2_INTERFACE_DEFINED__

/* interface IVsHiddenTextSessionEx2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_IVsHiddenTextSessionEx2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A40043F7-0865-4322-9308-32133314AD6C")
    IVsHiddenTextSessionEx2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StopOutlining( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartOutlining( 
            /* [in] */ BOOL fRemoveAdhoc) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartBatch( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EndBatch( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsHiddenTextSessionEx2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsHiddenTextSessionEx2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsHiddenTextSessionEx2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsHiddenTextSessionEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StopOutlining )( 
            __RPC__in IVsHiddenTextSessionEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *StartOutlining )( 
            __RPC__in IVsHiddenTextSessionEx2 * This,
            /* [in] */ BOOL fRemoveAdhoc);
        
        HRESULT ( STDMETHODCALLTYPE *StartBatch )( 
            __RPC__in IVsHiddenTextSessionEx2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *EndBatch )( 
            __RPC__in IVsHiddenTextSessionEx2 * This);
        
        END_INTERFACE
    } IVsHiddenTextSessionEx2Vtbl;

    interface IVsHiddenTextSessionEx2
    {
        CONST_VTBL struct IVsHiddenTextSessionEx2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsHiddenTextSessionEx2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsHiddenTextSessionEx2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsHiddenTextSessionEx2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsHiddenTextSessionEx2_StopOutlining(This)	\
    ( (This)->lpVtbl -> StopOutlining(This) ) 

#define IVsHiddenTextSessionEx2_StartOutlining(This,fRemoveAdhoc)	\
    ( (This)->lpVtbl -> StartOutlining(This,fRemoveAdhoc) ) 

#define IVsHiddenTextSessionEx2_StartBatch(This)	\
    ( (This)->lpVtbl -> StartBatch(This) ) 

#define IVsHiddenTextSessionEx2_EndBatch(This)	\
    ( (This)->lpVtbl -> EndBatch(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsHiddenTextSessionEx2_INTERFACE_DEFINED__ */

#endif /* __TextMgr100_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


