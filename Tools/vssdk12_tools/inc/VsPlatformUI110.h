

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

#ifndef __VsPlatformUI110_h__
#define __VsPlatformUI110_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsUIBoundPropertyBase_FWD_DEFINED__
#define __IVsUIBoundPropertyBase_FWD_DEFINED__
typedef interface IVsUIBoundPropertyBase IVsUIBoundPropertyBase;

#endif 	/* __IVsUIBoundPropertyBase_FWD_DEFINED__ */


#ifndef __IVsUIBoundBooleanProperty_FWD_DEFINED__
#define __IVsUIBoundBooleanProperty_FWD_DEFINED__
typedef interface IVsUIBoundBooleanProperty IVsUIBoundBooleanProperty;

#endif 	/* __IVsUIBoundBooleanProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundStringProperty_FWD_DEFINED__
#define __IVsUIBoundStringProperty_FWD_DEFINED__
typedef interface IVsUIBoundStringProperty IVsUIBoundStringProperty;

#endif 	/* __IVsUIBoundStringProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundInt32Property_FWD_DEFINED__
#define __IVsUIBoundInt32Property_FWD_DEFINED__
typedef interface IVsUIBoundInt32Property IVsUIBoundInt32Property;

#endif 	/* __IVsUIBoundInt32Property_FWD_DEFINED__ */


#ifndef __IVsUIBoundDWordProperty_FWD_DEFINED__
#define __IVsUIBoundDWordProperty_FWD_DEFINED__
typedef interface IVsUIBoundDWordProperty IVsUIBoundDWordProperty;

#endif 	/* __IVsUIBoundDWordProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundQWordProperty_FWD_DEFINED__
#define __IVsUIBoundQWordProperty_FWD_DEFINED__
typedef interface IVsUIBoundQWordProperty IVsUIBoundQWordProperty;

#endif 	/* __IVsUIBoundQWordProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundDoubleProperty_FWD_DEFINED__
#define __IVsUIBoundDoubleProperty_FWD_DEFINED__
typedef interface IVsUIBoundDoubleProperty IVsUIBoundDoubleProperty;

#endif 	/* __IVsUIBoundDoubleProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundGuidProperty_FWD_DEFINED__
#define __IVsUIBoundGuidProperty_FWD_DEFINED__
typedef interface IVsUIBoundGuidProperty IVsUIBoundGuidProperty;

#endif 	/* __IVsUIBoundGuidProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundDataSourceProperty_FWD_DEFINED__
#define __IVsUIBoundDataSourceProperty_FWD_DEFINED__
typedef interface IVsUIBoundDataSourceProperty IVsUIBoundDataSourceProperty;

#endif 	/* __IVsUIBoundDataSourceProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundCollectionProperty_FWD_DEFINED__
#define __IVsUIBoundCollectionProperty_FWD_DEFINED__
typedef interface IVsUIBoundCollectionProperty IVsUIBoundCollectionProperty;

#endif 	/* __IVsUIBoundCollectionProperty_FWD_DEFINED__ */


#ifndef __IVsUIBoundFormattedProperty_FWD_DEFINED__
#define __IVsUIBoundFormattedProperty_FWD_DEFINED__
typedef interface IVsUIBoundFormattedProperty IVsUIBoundFormattedProperty;

#endif 	/* __IVsUIBoundFormattedProperty_FWD_DEFINED__ */


#ifndef __IVsUIDataSourceShape_FWD_DEFINED__
#define __IVsUIDataSourceShape_FWD_DEFINED__
typedef interface IVsUIDataSourceShape IVsUIDataSourceShape;

#endif 	/* __IVsUIDataSourceShape_FWD_DEFINED__ */


#ifndef __IVsUIDataSourceShapeProvider_FWD_DEFINED__
#define __IVsUIDataSourceShapeProvider_FWD_DEFINED__
typedef interface IVsUIDataSourceShapeProvider IVsUIDataSourceShapeProvider;

#endif 	/* __IVsUIDataSourceShapeProvider_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "VsPlatformUI.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_VsPlatformUI110_0000_0000 */
/* [local] */ 

#define VSUI_TYPE_GUID  L"VsUI.Guid"


extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI110_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI110_0000_0000_v0_0_s_ifspec;

#ifndef __IVsUIBoundPropertyBase_INTERFACE_DEFINED__
#define __IVsUIBoundPropertyBase_INTERFACE_DEFINED__

/* interface IVsUIBoundPropertyBase */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundPropertyBase;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("42261aac-332f-4360-b925-0f532f9944be")
    IVsUIBoundPropertyBase : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ResetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CanReset( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CanWrite( 
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName) = 0;
        
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundPropertyBaseVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundPropertyBase * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundPropertyBase * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundPropertyBase * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        END_INTERFACE
    } IVsUIBoundPropertyBaseVtbl;

    interface IVsUIBoundPropertyBase
    {
        CONST_VTBL struct IVsUIBoundPropertyBaseVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundPropertyBase_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundPropertyBase_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundPropertyBase_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundPropertyBase_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundPropertyBase_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundPropertyBase_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundPropertyBase_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundPropertyBase_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundPropertyBase_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundBooleanProperty_INTERFACE_DEFINED__
#define __IVsUIBoundBooleanProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundBooleanProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundBooleanProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1406a251-7ebd-49d9-ae68-454f1906e6ff")
    IVsUIBoundBooleanProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ VARIANT_BOOL boolVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundBooleanPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundBooleanProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundBooleanProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *boolVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundBooleanProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ VARIANT_BOOL boolVal);
        
        END_INTERFACE
    } IVsUIBoundBooleanPropertyVtbl;

    interface IVsUIBoundBooleanProperty
    {
        CONST_VTBL struct IVsUIBoundBooleanPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundBooleanProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundBooleanProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundBooleanProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundBooleanProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundBooleanProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundBooleanProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundBooleanProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundBooleanProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundBooleanProperty_GetValue(This,owner,boolVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,boolVal) ) 

#define IVsUIBoundBooleanProperty_SetValue(This,owner,boolVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,boolVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundBooleanProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundStringProperty_INTERFACE_DEFINED__
#define __IVsUIBoundStringProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundStringProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundStringProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d86e711d-1ca3-4541-87ce-ffc8f604f454")
    IVsUIBoundStringProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *strVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in LPCOLESTR strVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundStringPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundStringProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundStringProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *strVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundStringProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in LPCOLESTR strVal);
        
        END_INTERFACE
    } IVsUIBoundStringPropertyVtbl;

    interface IVsUIBoundStringProperty
    {
        CONST_VTBL struct IVsUIBoundStringPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundStringProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundStringProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundStringProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundStringProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundStringProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundStringProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundStringProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundStringProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundStringProperty_GetValue(This,owner,strVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,strVal) ) 

#define IVsUIBoundStringProperty_SetValue(This,owner,strVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,strVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundStringProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundInt32Property_INTERFACE_DEFINED__
#define __IVsUIBoundInt32Property_INTERFACE_DEFINED__

/* interface IVsUIBoundInt32Property */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundInt32Property;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e5d6702c-cec1-4f24-b74e-abebccd65bf0")
    IVsUIBoundInt32Property : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out INT *intVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ INT intVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundInt32PropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundInt32Property * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundInt32Property * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out INT *intVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundInt32Property * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ INT intVal);
        
        END_INTERFACE
    } IVsUIBoundInt32PropertyVtbl;

    interface IVsUIBoundInt32Property
    {
        CONST_VTBL struct IVsUIBoundInt32PropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundInt32Property_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundInt32Property_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundInt32Property_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundInt32Property_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundInt32Property_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundInt32Property_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundInt32Property_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundInt32Property_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundInt32Property_GetValue(This,owner,intVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,intVal) ) 

#define IVsUIBoundInt32Property_SetValue(This,owner,intVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,intVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundInt32Property_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundDWordProperty_INTERFACE_DEFINED__
#define __IVsUIBoundDWordProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundDWordProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundDWordProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ae72464d-2866-46c1-b088-21838df1c1ed")
    IVsUIBoundDWordProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out DWORD *dwordVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ DWORD dwordVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundDWordPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundDWordProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundDWordProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out DWORD *dwordVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundDWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ DWORD dwordVal);
        
        END_INTERFACE
    } IVsUIBoundDWordPropertyVtbl;

    interface IVsUIBoundDWordProperty
    {
        CONST_VTBL struct IVsUIBoundDWordPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundDWordProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundDWordProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundDWordProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundDWordProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundDWordProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundDWordProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundDWordProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundDWordProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundDWordProperty_GetValue(This,owner,dwordVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,dwordVal) ) 

#define IVsUIBoundDWordProperty_SetValue(This,owner,dwordVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,dwordVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundDWordProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundQWordProperty_INTERFACE_DEFINED__
#define __IVsUIBoundQWordProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundQWordProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundQWordProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2ca8b41c-091c-4d98-ad2e-23d075551e1a")
    IVsUIBoundQWordProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out ULONGLONG *qwordVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ ULONGLONG qwordVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundQWordPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundQWordProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundQWordProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out ULONGLONG *qwordVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundQWordProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ ULONGLONG qwordVal);
        
        END_INTERFACE
    } IVsUIBoundQWordPropertyVtbl;

    interface IVsUIBoundQWordProperty
    {
        CONST_VTBL struct IVsUIBoundQWordPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundQWordProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundQWordProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundQWordProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundQWordProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundQWordProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundQWordProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundQWordProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundQWordProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundQWordProperty_GetValue(This,owner,qwordVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,qwordVal) ) 

#define IVsUIBoundQWordProperty_SetValue(This,owner,qwordVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,qwordVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundQWordProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundDoubleProperty_INTERFACE_DEFINED__
#define __IVsUIBoundDoubleProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundDoubleProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundDoubleProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00705016-e9ef-4caf-bab1-8e4484ed7b69")
    IVsUIBoundDoubleProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out double *dblVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ double dblVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundDoublePropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundDoubleProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundDoubleProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out double *dblVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundDoubleProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ double dblVal);
        
        END_INTERFACE
    } IVsUIBoundDoublePropertyVtbl;

    interface IVsUIBoundDoubleProperty
    {
        CONST_VTBL struct IVsUIBoundDoublePropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundDoubleProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundDoubleProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundDoubleProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundDoubleProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundDoubleProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundDoubleProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundDoubleProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundDoubleProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundDoubleProperty_GetValue(This,owner,dblVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,dblVal) ) 

#define IVsUIBoundDoubleProperty_SetValue(This,owner,dblVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,dblVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundDoubleProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundGuidProperty_INTERFACE_DEFINED__
#define __IVsUIBoundGuidProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundGuidProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundGuidProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6d0f82bc-c086-44f0-9ab0-92f67db1c896")
    IVsUIBoundGuidProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out GUID *guidVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in REFGUID guidVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundGuidPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundGuidProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundGuidProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__out GUID *guidVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundGuidProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in REFGUID guidVal);
        
        END_INTERFACE
    } IVsUIBoundGuidPropertyVtbl;

    interface IVsUIBoundGuidProperty
    {
        CONST_VTBL struct IVsUIBoundGuidPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundGuidProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundGuidProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundGuidProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundGuidProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundGuidProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundGuidProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundGuidProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundGuidProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundGuidProperty_GetValue(This,owner,guidVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,guidVal) ) 

#define IVsUIBoundGuidProperty_SetValue(This,owner,guidVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,guidVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundGuidProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundDataSourceProperty_INTERFACE_DEFINED__
#define __IVsUIBoundDataSourceProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundDataSourceProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundDataSourceProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b47682c3-726d-48f0-bcc9-247c8d1c4e1f")
    IVsUIBoundDataSourceProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSource **dataSourceVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [unique][in] */ __RPC__in_opt IVsUIDataSource *dataSourceVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundDataSourcePropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundDataSourceProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundDataSourceProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSource **dataSourceVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundDataSourceProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [unique][in] */ __RPC__in_opt IVsUIDataSource *dataSourceVal);
        
        END_INTERFACE
    } IVsUIBoundDataSourcePropertyVtbl;

    interface IVsUIBoundDataSourceProperty
    {
        CONST_VTBL struct IVsUIBoundDataSourcePropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundDataSourceProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundDataSourceProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundDataSourceProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundDataSourceProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundDataSourceProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundDataSourceProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundDataSourceProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundDataSourceProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundDataSourceProperty_GetValue(This,owner,dataSourceVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,dataSourceVal) ) 

#define IVsUIBoundDataSourceProperty_SetValue(This,owner,dataSourceVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,dataSourceVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundDataSourceProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundCollectionProperty_INTERFACE_DEFINED__
#define __IVsUIBoundCollectionProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundCollectionProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundCollectionProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b957e62c-a5e4-4c0a-b33f-51edeabff367")
    IVsUIBoundCollectionProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUICollection **collectionVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [unique][in] */ __RPC__in_opt IVsUICollection *collectionVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundCollectionPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundCollectionProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundCollectionProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUICollection **collectionVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundCollectionProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [unique][in] */ __RPC__in_opt IVsUICollection *collectionVal);
        
        END_INTERFACE
    } IVsUIBoundCollectionPropertyVtbl;

    interface IVsUIBoundCollectionProperty
    {
        CONST_VTBL struct IVsUIBoundCollectionPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundCollectionProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundCollectionProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundCollectionProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundCollectionProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundCollectionProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundCollectionProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundCollectionProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundCollectionProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundCollectionProperty_GetValue(This,owner,collectionVal)	\
    ( (This)->lpVtbl -> GetValue(This,owner,collectionVal) ) 

#define IVsUIBoundCollectionProperty_SetValue(This,owner,collectionVal)	\
    ( (This)->lpVtbl -> SetValue(This,owner,collectionVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundCollectionProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIBoundFormattedProperty_INTERFACE_DEFINED__
#define __IVsUIBoundFormattedProperty_INTERFACE_DEFINED__

/* interface IVsUIBoundFormattedProperty */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIBoundFormattedProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6e4a81aa-6694-4261-940d-837d8991dba4")
    IVsUIBoundFormattedProperty : public IVsUIBoundPropertyBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in_opt IVsUIObject *pValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIBoundFormattedPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIBoundFormattedProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIBoundFormattedProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanReset )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanReset);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_CanWrite )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [retval][out] */ __RPC__out VARIANT_BOOL *pCanWrite);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyName);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *pPropertyType);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIObject **ppValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            __RPC__in IVsUIBoundFormattedProperty * This,
            /* [in] */ __RPC__in_opt IUnknown *owner,
            /* [in] */ __RPC__in_opt IVsUIObject *pValue);
        
        END_INTERFACE
    } IVsUIBoundFormattedPropertyVtbl;

    interface IVsUIBoundFormattedProperty
    {
        CONST_VTBL struct IVsUIBoundFormattedPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIBoundFormattedProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIBoundFormattedProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIBoundFormattedProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIBoundFormattedProperty_ResetValue(This,owner)	\
    ( (This)->lpVtbl -> ResetValue(This,owner) ) 

#define IVsUIBoundFormattedProperty_get_CanReset(This,pCanReset)	\
    ( (This)->lpVtbl -> get_CanReset(This,pCanReset) ) 

#define IVsUIBoundFormattedProperty_get_CanWrite(This,pCanWrite)	\
    ( (This)->lpVtbl -> get_CanWrite(This,pCanWrite) ) 

#define IVsUIBoundFormattedProperty_get_Name(This,pPropertyName)	\
    ( (This)->lpVtbl -> get_Name(This,pPropertyName) ) 

#define IVsUIBoundFormattedProperty_get_Type(This,pPropertyType)	\
    ( (This)->lpVtbl -> get_Type(This,pPropertyType) ) 


#define IVsUIBoundFormattedProperty_GetValue(This,owner,ppValue)	\
    ( (This)->lpVtbl -> GetValue(This,owner,ppValue) ) 

#define IVsUIBoundFormattedProperty_SetValue(This,owner,pValue)	\
    ( (This)->lpVtbl -> SetValue(This,owner,pValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIBoundFormattedProperty_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataSourceShape_INTERFACE_DEFINED__
#define __IVsUIDataSourceShape_INTERFACE_DEFINED__

/* interface IVsUIDataSourceShape */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataSourceShape;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ddd2cb35-dc38-46a9-8c9e-f8bc0649cf80")
    IVsUIDataSourceShape : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumProperties( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBoundProperty( 
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIBoundPropertyBase **boundProperty) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIDataSourceShapeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIDataSourceShape * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIDataSourceShape * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIDataSourceShape * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProperties )( 
            __RPC__in IVsUIDataSourceShape * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetBoundProperty )( 
            __RPC__in IVsUIDataSourceShape * This,
            /* [in] */ __RPC__in LPCOLESTR name,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIBoundPropertyBase **boundProperty);
        
        END_INTERFACE
    } IVsUIDataSourceShapeVtbl;

    interface IVsUIDataSourceShape
    {
        CONST_VTBL struct IVsUIDataSourceShapeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataSourceShape_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataSourceShape_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataSourceShape_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataSourceShape_EnumProperties(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProperties(This,ppEnum) ) 

#define IVsUIDataSourceShape_GetBoundProperty(This,name,boundProperty)	\
    ( (This)->lpVtbl -> GetBoundProperty(This,name,boundProperty) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataSourceShape_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataSourceShapeProvider_INTERFACE_DEFINED__
#define __IVsUIDataSourceShapeProvider_INTERFACE_DEFINED__

/* interface IVsUIDataSourceShapeProvider */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataSourceShapeProvider;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7eef6797-7698-42a9-9157-dd8c90f5854a")
    IVsUIDataSourceShapeProvider : public IUnknown
    {
    public:
        virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Shape( 
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSourceShape **ppShape) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVsUIDataSourceShapeProviderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IVsUIDataSourceShapeProvider * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IVsUIDataSourceShapeProvider * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IVsUIDataSourceShapeProvider * This);
        
        /* [propget] */ HRESULT ( STDMETHODCALLTYPE *get_Shape )( 
            __RPC__in IVsUIDataSourceShapeProvider * This,
            /* [retval][out] */ __RPC__deref_out_opt IVsUIDataSourceShape **ppShape);
        
        END_INTERFACE
    } IVsUIDataSourceShapeProviderVtbl;

    interface IVsUIDataSourceShapeProvider
    {
        CONST_VTBL struct IVsUIDataSourceShapeProviderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataSourceShapeProvider_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataSourceShapeProvider_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataSourceShapeProvider_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataSourceShapeProvider_get_Shape(This,ppShape)	\
    ( (This)->lpVtbl -> get_Shape(This,ppShape) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataSourceShapeProvider_INTERFACE_DEFINED__ */


/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     __RPC__in unsigned long *, unsigned long            , __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  __RPC__in unsigned long *, __RPC__inout_xcount(0) unsigned char *, __RPC__in BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(__RPC__in unsigned long *, __RPC__in_xcount(0) unsigned char *, __RPC__out BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     __RPC__in unsigned long *, __RPC__in BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


