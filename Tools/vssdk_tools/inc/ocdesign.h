

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for ocdesign.idl:
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

#ifndef __ocdesign_h__
#define __ocdesign_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IBuilderWizardManager_FWD_DEFINED__
#define __IBuilderWizardManager_FWD_DEFINED__
typedef interface IBuilderWizardManager IBuilderWizardManager;
#endif 	/* __IBuilderWizardManager_FWD_DEFINED__ */


#ifndef __IProvidePropertyBuilder_FWD_DEFINED__
#define __IProvidePropertyBuilder_FWD_DEFINED__
typedef interface IProvidePropertyBuilder IProvidePropertyBuilder;
#endif 	/* __IProvidePropertyBuilder_FWD_DEFINED__ */


#ifndef __IPerPropertyBrowsing2_FWD_DEFINED__
#define __IPerPropertyBrowsing2_FWD_DEFINED__
typedef interface IPerPropertyBrowsing2 IPerPropertyBrowsing2;
#endif 	/* __IPerPropertyBrowsing2_FWD_DEFINED__ */


#ifndef __IPropertyPageUndoString_FWD_DEFINED__
#define __IPropertyPageUndoString_FWD_DEFINED__
typedef interface IPropertyPageUndoString IPropertyPageUndoString;
#endif 	/* __IPropertyPageUndoString_FWD_DEFINED__ */


#ifndef __IProvideRuntimeHTML_FWD_DEFINED__
#define __IProvideRuntimeHTML_FWD_DEFINED__
typedef interface IProvideRuntimeHTML IProvideRuntimeHTML;
#endif 	/* __IProvideRuntimeHTML_FWD_DEFINED__ */


#ifndef __IProvideAltHTML_FWD_DEFINED__
#define __IProvideAltHTML_FWD_DEFINED__
typedef interface IProvideAltHTML IProvideAltHTML;
#endif 	/* __IProvideAltHTML_FWD_DEFINED__ */


#ifndef __IBuilderWizardManager_FWD_DEFINED__
#define __IBuilderWizardManager_FWD_DEFINED__
typedef interface IBuilderWizardManager IBuilderWizardManager;
#endif 	/* __IBuilderWizardManager_FWD_DEFINED__ */


#ifndef __IProvidePropertyBuilder_FWD_DEFINED__
#define __IProvidePropertyBuilder_FWD_DEFINED__
typedef interface IProvidePropertyBuilder IProvidePropertyBuilder;
#endif 	/* __IProvidePropertyBuilder_FWD_DEFINED__ */


#ifndef __IPerPropertyBrowsing2_FWD_DEFINED__
#define __IPerPropertyBrowsing2_FWD_DEFINED__
typedef interface IPerPropertyBrowsing2 IPerPropertyBrowsing2;
#endif 	/* __IPerPropertyBrowsing2_FWD_DEFINED__ */


#ifndef __IPropertyPageUndoString_FWD_DEFINED__
#define __IPropertyPageUndoString_FWD_DEFINED__
typedef interface IPropertyPageUndoString IPropertyPageUndoString;
#endif 	/* __IPropertyPageUndoString_FWD_DEFINED__ */


#ifndef __IProvideRuntimeHTML_FWD_DEFINED__
#define __IProvideRuntimeHTML_FWD_DEFINED__
typedef interface IProvideRuntimeHTML IProvideRuntimeHTML;
#endif 	/* __IProvideRuntimeHTML_FWD_DEFINED__ */


#ifndef __IProvideAltHTML_FWD_DEFINED__
#define __IProvideAltHTML_FWD_DEFINED__
typedef interface IProvideAltHTML IProvideAltHTML;
#endif 	/* __IProvideAltHTML_FWD_DEFINED__ */


/* header files for imported files */
#include "oleidl.h"
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ocdesign_0000_0000 */
/* [local] */ 








DEFINE_GUID(IID_IUseImmediateCommitPropertyPages, 0xfd6feba0, 0x24ac, 0x11d1, 0xab, 0x1b, 0x0, 0xa0, 0xc9, 0x5, 0x5a, 0x90);
DEFINE_GUID(CATID_HTMLBuilder, 0x73cef3d4, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_ObjectBuilder, 0x73cef3d5, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_PropertyBuilder, 0x73cef3d6, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_URLBuilder, 0x73cef3d9, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_ColorBuilder, 0x73cef3da, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_FontBuilder, 0x73cef3db, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_ActiveXControlBuilder, 0x73cef3de, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_ScriptBuilder, 0x73cef3df, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(CATID_PictureBuilder, 0x73cef3e0, 0xae85, 0x11cf, 0xa4, 0x6, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(SID_SBuilderWizardManager, 0x95fc88c2, 0x9fcb, 0x11cf, 0xa4, 0x5, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(SID_IBuilderWizardManager, 0x95fc88c2, 0x9fcb, 0x11cf, 0xa4, 0x5, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
typedef 
enum tagBLDPROMPTOPT
    {	BLDPROMPTOPT_PROMPTIFMULTIPLE	= 0,
	BLDPROMPTOPT_PROMPTALWAYS	= 1,
	BLDPROMPTOPT_PROMPTNEVER	= 2
    } 	BLDPROMPTOPT;

typedef 
enum tagBLDGETOPT
    {	BLDGETOPT_FAUTOMAPGUID	= 0x1,
	BLDGETOPT_FAUTOMAPENABLEPROMPT	= 0x2,
	BLDGETOPT_FAUTOMAPPROMPTALWAYS	= 0x4,
	BLDGETOPT_FOBJECTBUILDER	= 0x8,
	BLDGETOPT_FNOINTRINSICS	= 0x80000000
    } 	BLDGETFLAGS;



extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0000_v0_0_s_ifspec;

#ifndef __IBuilderWizardManager_INTERFACE_DEFINED__
#define __IBuilderWizardManager_INTERFACE_DEFINED__

/* interface IBuilderWizardManager */
/* [unique][version][uuid][object] */ 


EXTERN_C const IID IID_IBuilderWizardManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("95FC88C3-9FCB-11cf-A405-00AA00C00940")
    IBuilderWizardManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DoesBuilderExist( 
            /* [in] */ __RPC__in REFGUID rguidBuilder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapObjectToBuilderCLSID( 
            /* [in] */ __RPC__in REFCLSID rclsidObject,
            /* [in] */ DWORD dwPromptOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__out CLSID *pclsidBuilder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapBuilderCATIDToCLSID( 
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ DWORD dwPromptOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__out CLSID *pclsidBuilder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBuilder( 
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ DWORD grfGetOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispApp,
            /* [out] */ __RPC__deref_out_opt HWND *pwndBuilderOwner,
            /* [in] */ __RPC__in REFIID riidBuilder,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkBuilder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnableModeless( 
            /* [in] */ BOOL fEnable) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IBuilderWizardManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IBuilderWizardManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IBuilderWizardManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IBuilderWizardManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *DoesBuilderExist )( 
            IBuilderWizardManager * This,
            /* [in] */ __RPC__in REFGUID rguidBuilder);
        
        HRESULT ( STDMETHODCALLTYPE *MapObjectToBuilderCLSID )( 
            IBuilderWizardManager * This,
            /* [in] */ __RPC__in REFCLSID rclsidObject,
            /* [in] */ DWORD dwPromptOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__out CLSID *pclsidBuilder);
        
        HRESULT ( STDMETHODCALLTYPE *MapBuilderCATIDToCLSID )( 
            IBuilderWizardManager * This,
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ DWORD dwPromptOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__out CLSID *pclsidBuilder);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuilder )( 
            IBuilderWizardManager * This,
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ DWORD grfGetOpt,
            /* [in] */ __RPC__in HWND hwndPromptOwner,
            /* [out] */ __RPC__deref_out_opt IDispatch **ppdispApp,
            /* [out] */ __RPC__deref_out_opt HWND *pwndBuilderOwner,
            /* [in] */ __RPC__in REFIID riidBuilder,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppunkBuilder);
        
        HRESULT ( STDMETHODCALLTYPE *EnableModeless )( 
            IBuilderWizardManager * This,
            /* [in] */ BOOL fEnable);
        
        END_INTERFACE
    } IBuilderWizardManagerVtbl;

    interface IBuilderWizardManager
    {
        CONST_VTBL struct IBuilderWizardManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IBuilderWizardManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IBuilderWizardManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IBuilderWizardManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IBuilderWizardManager_DoesBuilderExist(This,rguidBuilder)	\
    ( (This)->lpVtbl -> DoesBuilderExist(This,rguidBuilder) ) 

#define IBuilderWizardManager_MapObjectToBuilderCLSID(This,rclsidObject,dwPromptOpt,hwndPromptOwner,pclsidBuilder)	\
    ( (This)->lpVtbl -> MapObjectToBuilderCLSID(This,rclsidObject,dwPromptOpt,hwndPromptOwner,pclsidBuilder) ) 

#define IBuilderWizardManager_MapBuilderCATIDToCLSID(This,rguidBuilder,dwPromptOpt,hwndPromptOwner,pclsidBuilder)	\
    ( (This)->lpVtbl -> MapBuilderCATIDToCLSID(This,rguidBuilder,dwPromptOpt,hwndPromptOwner,pclsidBuilder) ) 

#define IBuilderWizardManager_GetBuilder(This,rguidBuilder,grfGetOpt,hwndPromptOwner,ppdispApp,pwndBuilderOwner,riidBuilder,ppunkBuilder)	\
    ( (This)->lpVtbl -> GetBuilder(This,rguidBuilder,grfGetOpt,hwndPromptOwner,ppdispApp,pwndBuilderOwner,riidBuilder,ppunkBuilder) ) 

#define IBuilderWizardManager_EnableModeless(This,fEnable)	\
    ( (This)->lpVtbl -> EnableModeless(This,fEnable) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IBuilderWizardManager_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ocdesign_0000_0001 */
/* [local] */ 

typedef 
enum tagCTLBLDTYPE
    {	CTLBLDTYPE_FSTDPROPBUILDER	= 0x1,
	CTLBLDTYPE_FINTERNALBUILDER	= 0x2,
	CTLBLDTYPE_FEDITSOBJDIRECTLY	= 0x4
    } 	CTLBLDTYPE;



extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0001_v0_0_s_ifspec;

#ifndef __IProvidePropertyBuilder_INTERFACE_DEFINED__
#define __IProvidePropertyBuilder_INTERFACE_DEFINED__

/* interface IProvidePropertyBuilder */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IProvidePropertyBuilder;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33C0C1D8-33CF-11d3-BFF2-00C04F990235")
    IProvidePropertyBuilder : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MapPropertyToBuilder( 
            /* [in] */ LONG dispid,
            /* [out][in] */ __RPC__inout LONG *pdwCtlBldType,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrGuidBldr,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecuteBuilder( 
            /* [in] */ LONG dispid,
            /* [in] */ __RPC__in BSTR bstrGuidBldr,
            /* [in] */ __RPC__in_opt IDispatch *pdispApp,
            /* [in] */ LONG_PTR hwndBldrOwner,
            /* [out][in] */ __RPC__inout VARIANT *pvarValue,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IProvidePropertyBuilderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IProvidePropertyBuilder * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IProvidePropertyBuilder * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IProvidePropertyBuilder * This);
        
        HRESULT ( STDMETHODCALLTYPE *MapPropertyToBuilder )( 
            IProvidePropertyBuilder * This,
            /* [in] */ LONG dispid,
            /* [out][in] */ __RPC__inout LONG *pdwCtlBldType,
            /* [out][in] */ __RPC__deref_inout_opt BSTR *pbstrGuidBldr,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *ExecuteBuilder )( 
            IProvidePropertyBuilder * This,
            /* [in] */ LONG dispid,
            /* [in] */ __RPC__in BSTR bstrGuidBldr,
            /* [in] */ __RPC__in_opt IDispatch *pdispApp,
            /* [in] */ LONG_PTR hwndBldrOwner,
            /* [out][in] */ __RPC__inout VARIANT *pvarValue,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pfRetVal);
        
        END_INTERFACE
    } IProvidePropertyBuilderVtbl;

    interface IProvidePropertyBuilder
    {
        CONST_VTBL struct IProvidePropertyBuilderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IProvidePropertyBuilder_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IProvidePropertyBuilder_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IProvidePropertyBuilder_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IProvidePropertyBuilder_MapPropertyToBuilder(This,dispid,pdwCtlBldType,pbstrGuidBldr,pfRetVal)	\
    ( (This)->lpVtbl -> MapPropertyToBuilder(This,dispid,pdwCtlBldType,pbstrGuidBldr,pfRetVal) ) 

#define IProvidePropertyBuilder_ExecuteBuilder(This,dispid,bstrGuidBldr,pdispApp,hwndBldrOwner,pvarValue,pfRetVal)	\
    ( (This)->lpVtbl -> ExecuteBuilder(This,dispid,bstrGuidBldr,pdispApp,hwndBldrOwner,pvarValue,pfRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IProvidePropertyBuilder_INTERFACE_DEFINED__ */


#ifndef __IPerPropertyBrowsing2_INTERFACE_DEFINED__
#define __IPerPropertyBrowsing2_INTERFACE_DEFINED__

/* interface IPerPropertyBrowsing2 */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IPerPropertyBrowsing2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33C0C1DA-33CF-11d3-BFF2-00C04F990235")
    IPerPropertyBrowsing2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MapPropertyToBuilder( 
            /* [in] */ DISPID dispid,
            /* [out] */ __RPC__out GUID *pguidBuilder,
            /* [out] */ __RPC__out DWORD *pdwType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ExecuteBuilder( 
            /* [in] */ DISPID dispid,
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ __RPC__in_opt IDispatch *pdispApp,
            /* [in] */ __RPC__in HWND hwndBuilderOwner,
            /* [out][in] */ __RPC__inout VARIANT *pvarValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPerPropertyBrowsing2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPerPropertyBrowsing2 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPerPropertyBrowsing2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPerPropertyBrowsing2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *MapPropertyToBuilder )( 
            IPerPropertyBrowsing2 * This,
            /* [in] */ DISPID dispid,
            /* [out] */ __RPC__out GUID *pguidBuilder,
            /* [out] */ __RPC__out DWORD *pdwType);
        
        HRESULT ( STDMETHODCALLTYPE *ExecuteBuilder )( 
            IPerPropertyBrowsing2 * This,
            /* [in] */ DISPID dispid,
            /* [in] */ __RPC__in REFGUID rguidBuilder,
            /* [in] */ __RPC__in_opt IDispatch *pdispApp,
            /* [in] */ __RPC__in HWND hwndBuilderOwner,
            /* [out][in] */ __RPC__inout VARIANT *pvarValue);
        
        END_INTERFACE
    } IPerPropertyBrowsing2Vtbl;

    interface IPerPropertyBrowsing2
    {
        CONST_VTBL struct IPerPropertyBrowsing2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPerPropertyBrowsing2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPerPropertyBrowsing2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPerPropertyBrowsing2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPerPropertyBrowsing2_MapPropertyToBuilder(This,dispid,pguidBuilder,pdwType)	\
    ( (This)->lpVtbl -> MapPropertyToBuilder(This,dispid,pguidBuilder,pdwType) ) 

#define IPerPropertyBrowsing2_ExecuteBuilder(This,dispid,rguidBuilder,pdispApp,hwndBuilderOwner,pvarValue)	\
    ( (This)->lpVtbl -> ExecuteBuilder(This,dispid,rguidBuilder,pdispApp,hwndBuilderOwner,pvarValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPerPropertyBrowsing2_INTERFACE_DEFINED__ */


#ifndef __IPropertyPageUndoString_INTERFACE_DEFINED__
#define __IPropertyPageUndoString_INTERFACE_DEFINED__

/* interface IPropertyPageUndoString */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IPropertyPageUndoString;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33C0C1DB-33CF-11d3-BFF2-00C04F990235")
    IPropertyPageUndoString : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetUndoString( 
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszUndo) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IPropertyPageUndoStringVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPropertyPageUndoString * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPropertyPageUndoString * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPropertyPageUndoString * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetUndoString )( 
            IPropertyPageUndoString * This,
            /* [out] */ __RPC__deref_out_opt LPOLESTR *ppszUndo);
        
        END_INTERFACE
    } IPropertyPageUndoStringVtbl;

    interface IPropertyPageUndoString
    {
        CONST_VTBL struct IPropertyPageUndoStringVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPropertyPageUndoString_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPropertyPageUndoString_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPropertyPageUndoString_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPropertyPageUndoString_GetUndoString(This,ppszUndo)	\
    ( (This)->lpVtbl -> GetUndoString(This,ppszUndo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPropertyPageUndoString_INTERFACE_DEFINED__ */


#ifndef __IProvideRuntimeHTML_INTERFACE_DEFINED__
#define __IProvideRuntimeHTML_INTERFACE_DEFINED__

/* interface IProvideRuntimeHTML */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IProvideRuntimeHTML;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33C0C1DC-33CF-11d3-BFF2-00C04F990235")
    IProvideRuntimeHTML : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRuntimeHTML( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrRuntimeHTML) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IProvideRuntimeHTMLVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IProvideRuntimeHTML * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IProvideRuntimeHTML * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IProvideRuntimeHTML * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRuntimeHTML )( 
            IProvideRuntimeHTML * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrRuntimeHTML);
        
        END_INTERFACE
    } IProvideRuntimeHTMLVtbl;

    interface IProvideRuntimeHTML
    {
        CONST_VTBL struct IProvideRuntimeHTMLVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IProvideRuntimeHTML_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IProvideRuntimeHTML_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IProvideRuntimeHTML_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IProvideRuntimeHTML_GetRuntimeHTML(This,pstrRuntimeHTML)	\
    ( (This)->lpVtbl -> GetRuntimeHTML(This,pstrRuntimeHTML) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IProvideRuntimeHTML_INTERFACE_DEFINED__ */


#ifndef __IProvideAltHTML_INTERFACE_DEFINED__
#define __IProvideAltHTML_INTERFACE_DEFINED__

/* interface IProvideAltHTML */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IProvideAltHTML;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("33C0C1DD-33CF-11d3-BFF2-00C04F990235")
    IProvideAltHTML : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAltHTML( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrAltHTML) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsAltHTMLEditable( 
            /* [retval][out] */ __RPC__out boolean *pfIsEditable) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IProvideAltHTMLVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IProvideAltHTML * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IProvideAltHTML * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IProvideAltHTML * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAltHTML )( 
            IProvideAltHTML * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pstrAltHTML);
        
        HRESULT ( STDMETHODCALLTYPE *IsAltHTMLEditable )( 
            IProvideAltHTML * This,
            /* [retval][out] */ __RPC__out boolean *pfIsEditable);
        
        END_INTERFACE
    } IProvideAltHTMLVtbl;

    interface IProvideAltHTML
    {
        CONST_VTBL struct IProvideAltHTMLVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IProvideAltHTML_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IProvideAltHTML_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IProvideAltHTML_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IProvideAltHTML_GetAltHTML(This,pstrAltHTML)	\
    ( (This)->lpVtbl -> GetAltHTML(This,pstrAltHTML) ) 

#define IProvideAltHTML_IsAltHTMLEditable(This,pfIsEditable)	\
    ( (This)->lpVtbl -> IsAltHTMLEditable(This,pfIsEditable) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IProvideAltHTML_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ocdesign_0000_0006 */
/* [local] */ 

DEFINE_GUID(IID_IPersistHTMLStream, 0x56223fe3, 0xd397, 0x11cf, 0xa4, 0x2e, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);
DEFINE_GUID(IID_IPersistAltHTMLStream, 0x56223fe4, 0xd397, 0x11cf, 0xa4, 0x2e, 0x0, 0xaa, 0x0, 0xc0, 0x9, 0x40);


extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ocdesign_0000_0006_v0_0_s_ifspec;


#ifndef __OCDesign_LIBRARY_DEFINED__
#define __OCDesign_LIBRARY_DEFINED__

/* library OCDesign */
/* [helpstring][version][uuid] */ 












EXTERN_C const IID LIBID_OCDesign;
#endif /* __OCDesign_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  HWND_UserSize(     unsigned long *, unsigned long            , HWND * ); 
unsigned char * __RPC_USER  HWND_UserMarshal(  unsigned long *, unsigned char *, HWND * ); 
unsigned char * __RPC_USER  HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * ); 
void                      __RPC_USER  HWND_UserFree(     unsigned long *, HWND * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


