

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for VsPlatformUI.idl:
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

#ifndef __VsPlatformUI_h__
#define __VsPlatformUI_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IVsUIObject_FWD_DEFINED__
#define __IVsUIObject_FWD_DEFINED__
typedef interface IVsUIObject IVsUIObject;
#endif 	/* __IVsUIObject_FWD_DEFINED__ */


#ifndef __IVsUIDataConverter_FWD_DEFINED__
#define __IVsUIDataConverter_FWD_DEFINED__
typedef interface IVsUIDataConverter IVsUIDataConverter;
#endif 	/* __IVsUIDataConverter_FWD_DEFINED__ */


#ifndef __IVsUIDataConverterManager_FWD_DEFINED__
#define __IVsUIDataConverterManager_FWD_DEFINED__
typedef interface IVsUIDataConverterManager IVsUIDataConverterManager;
#endif 	/* __IVsUIDataConverterManager_FWD_DEFINED__ */


#ifndef __SVsUIDataConverters_FWD_DEFINED__
#define __SVsUIDataConverters_FWD_DEFINED__
typedef interface SVsUIDataConverters SVsUIDataConverters;
#endif 	/* __SVsUIDataConverters_FWD_DEFINED__ */


#ifndef __IVsUIEnumDataSourceProperties_FWD_DEFINED__
#define __IVsUIEnumDataSourceProperties_FWD_DEFINED__
typedef interface IVsUIEnumDataSourceProperties IVsUIEnumDataSourceProperties;
#endif 	/* __IVsUIEnumDataSourceProperties_FWD_DEFINED__ */


#ifndef __IVsUIEnumDataSourceVerbs_FWD_DEFINED__
#define __IVsUIEnumDataSourceVerbs_FWD_DEFINED__
typedef interface IVsUIEnumDataSourceVerbs IVsUIEnumDataSourceVerbs;
#endif 	/* __IVsUIEnumDataSourceVerbs_FWD_DEFINED__ */


#ifndef __IVsUIEventSink_FWD_DEFINED__
#define __IVsUIEventSink_FWD_DEFINED__
typedef interface IVsUIEventSink IVsUIEventSink;
#endif 	/* __IVsUIEventSink_FWD_DEFINED__ */


#ifndef __IVsUIDataSourcePropertyChangeEvents_FWD_DEFINED__
#define __IVsUIDataSourcePropertyChangeEvents_FWD_DEFINED__
typedef interface IVsUIDataSourcePropertyChangeEvents IVsUIDataSourcePropertyChangeEvents;
#endif 	/* __IVsUIDataSourcePropertyChangeEvents_FWD_DEFINED__ */


#ifndef __IVsUIDispatch_FWD_DEFINED__
#define __IVsUIDispatch_FWD_DEFINED__
typedef interface IVsUIDispatch IVsUIDispatch;
#endif 	/* __IVsUIDispatch_FWD_DEFINED__ */


#ifndef __IVsUISimpleDataSource_FWD_DEFINED__
#define __IVsUISimpleDataSource_FWD_DEFINED__
typedef interface IVsUISimpleDataSource IVsUISimpleDataSource;
#endif 	/* __IVsUISimpleDataSource_FWD_DEFINED__ */


#ifndef __IVsUIDataSource_FWD_DEFINED__
#define __IVsUIDataSource_FWD_DEFINED__
typedef interface IVsUIDataSource IVsUIDataSource;
#endif 	/* __IVsUIDataSource_FWD_DEFINED__ */


#ifndef __IVsUICollectionChangeEvents_FWD_DEFINED__
#define __IVsUICollectionChangeEvents_FWD_DEFINED__
typedef interface IVsUICollectionChangeEvents IVsUICollectionChangeEvents;
#endif 	/* __IVsUICollectionChangeEvents_FWD_DEFINED__ */


#ifndef __IVsUICollection_FWD_DEFINED__
#define __IVsUICollection_FWD_DEFINED__
typedef interface IVsUICollection IVsUICollection;
#endif 	/* __IVsUICollection_FWD_DEFINED__ */


#ifndef __IVsUIDynamicCollection_FWD_DEFINED__
#define __IVsUIDynamicCollection_FWD_DEFINED__
typedef interface IVsUIDynamicCollection IVsUIDynamicCollection;
#endif 	/* __IVsUIDynamicCollection_FWD_DEFINED__ */


#ifndef __IVsUIAccelerator_FWD_DEFINED__
#define __IVsUIAccelerator_FWD_DEFINED__
typedef interface IVsUIAccelerator IVsUIAccelerator;
#endif 	/* __IVsUIAccelerator_FWD_DEFINED__ */


#ifndef __IVsUIElement_FWD_DEFINED__
#define __IVsUIElement_FWD_DEFINED__
typedef interface IVsUIElement IVsUIElement;
#endif 	/* __IVsUIElement_FWD_DEFINED__ */


#ifndef __IVsUIWpfElement_FWD_DEFINED__
#define __IVsUIWpfElement_FWD_DEFINED__
typedef interface IVsUIWpfElement IVsUIWpfElement;
#endif 	/* __IVsUIWpfElement_FWD_DEFINED__ */


#ifndef __IVsUIWin32Element_FWD_DEFINED__
#define __IVsUIWin32Element_FWD_DEFINED__
typedef interface IVsUIWin32Element IVsUIWin32Element;
#endif 	/* __IVsUIWin32Element_FWD_DEFINED__ */


#ifndef __IVsUIWpfLoader_FWD_DEFINED__
#define __IVsUIWpfLoader_FWD_DEFINED__
typedef interface IVsUIWpfLoader IVsUIWpfLoader;
#endif 	/* __IVsUIWpfLoader_FWD_DEFINED__ */


#ifndef __IVsUIFactory_FWD_DEFINED__
#define __IVsUIFactory_FWD_DEFINED__
typedef interface IVsUIFactory IVsUIFactory;
#endif 	/* __IVsUIFactory_FWD_DEFINED__ */


#ifndef __IVsRegisterUIFactories_FWD_DEFINED__
#define __IVsRegisterUIFactories_FWD_DEFINED__
typedef interface IVsRegisterUIFactories IVsRegisterUIFactories;
#endif 	/* __IVsRegisterUIFactories_FWD_DEFINED__ */


#ifndef __SVsUIFactory_FWD_DEFINED__
#define __SVsUIFactory_FWD_DEFINED__
typedef interface SVsUIFactory SVsUIFactory;
#endif 	/* __SVsUIFactory_FWD_DEFINED__ */


#ifndef __IVsDataSourceFactory_FWD_DEFINED__
#define __IVsDataSourceFactory_FWD_DEFINED__
typedef interface IVsDataSourceFactory IVsDataSourceFactory;
#endif 	/* __IVsDataSourceFactory_FWD_DEFINED__ */


#ifndef __IVsRegisterDataSourceFactories_FWD_DEFINED__
#define __IVsRegisterDataSourceFactories_FWD_DEFINED__
typedef interface IVsRegisterDataSourceFactories IVsRegisterDataSourceFactories;
#endif 	/* __IVsRegisterDataSourceFactories_FWD_DEFINED__ */


#ifndef __SVsDataSourceFactory_FWD_DEFINED__
#define __SVsDataSourceFactory_FWD_DEFINED__
typedef interface SVsDataSourceFactory SVsDataSourceFactory;
#endif 	/* __SVsDataSourceFactory_FWD_DEFINED__ */


#ifndef __IVsUIWin32Icon_FWD_DEFINED__
#define __IVsUIWin32Icon_FWD_DEFINED__
typedef interface IVsUIWin32Icon IVsUIWin32Icon;
#endif 	/* __IVsUIWin32Icon_FWD_DEFINED__ */


#ifndef __IVsUIWin32ImageList_FWD_DEFINED__
#define __IVsUIWin32ImageList_FWD_DEFINED__
typedef interface IVsUIWin32ImageList IVsUIWin32ImageList;
#endif 	/* __IVsUIWin32ImageList_FWD_DEFINED__ */


#ifndef __IVsUIWin32Bitmap_FWD_DEFINED__
#define __IVsUIWin32Bitmap_FWD_DEFINED__
typedef interface IVsUIWin32Bitmap IVsUIWin32Bitmap;
#endif 	/* __IVsUIWin32Bitmap_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_VsPlatformUI_0000_0000 */
/* [local] */ 

#pragma once



typedef 
enum __tagVSUIDATAFORMAT
    {	VSDF_RESERVED	= -2,
	VSDF_INVALID	= -1,
	VSDF_BUILTIN	= 0,
	VSDF_WIN32	= 1,
	VSDF_WINFORMS	= 2,
	VSDF_WPF	= 3
    } 	__VSUIDATAFORMAT;

typedef DWORD VSUIDATAFORMAT;

#define VSUI_TYPE_CHAR        L"VsUI.Char"
#define VSUI_TYPE_INT16       L"VsUI.Int16"
#define VSUI_TYPE_INT32       L"VsUI.Int32"
#define VSUI_TYPE_INT64       L"VsUI.Int64"
#define VSUI_TYPE_BYTE        L"VsUI.Byte"
#define VSUI_TYPE_WORD        L"VsUI.Word"
#define VSUI_TYPE_DWORD       L"VsUI.DWord"
#define VSUI_TYPE_QWORD       L"VsUI.QWord"
#define VSUI_TYPE_BOOL        L"VsUI.Boolean"
#define VSUI_TYPE_STRING      L"VsUI.String"
#define VSUI_TYPE_DATETIME    L"VsUI.DateTime"
#define VSUI_TYPE_SINGLE      L"VsUI.Single"
#define VSUI_TYPE_DOUBLE      L"VsUI.Double"
#define VSUI_TYPE_DECIMAL     L"VsUI.Decimal"
#define VSUI_TYPE_UNKNOWN     L"VsUI.Unknown"
#define VSUI_TYPE_DISPATCH    L"VsUI.Dispatch"
#define VSUI_TYPE_DATASOURCE  L"VsUI.DataSource"
#define VSUI_TYPE_COLLECTION  L"VsUI.Collection"
#define VSUI_TYPE_BITMAP      L"VsUI.Bitmap"
#define VSUI_TYPE_ICON        L"VsUI.Icon"
#define VSUI_TYPE_IMAGELIST   L"VsUI.ImageList"
#define VSUI_TYPE_COLOR       L"VsUI.Color"
typedef DWORD VSUICOOKIE;

#define VSUICOOKIE_NIL (0)


extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0000_v0_0_s_ifspec;

#ifndef __IVsUIObject_INTERFACE_DEFINED__
#define __IVsUIObject_INTERFACE_DEFINED__

/* interface IVsUIObject */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIObject;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("86FD1A37-A8C2-41DF-98FA-086D79BFD33E")
    IVsUIObject : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Type( 
            /* [out] */ __RPC__deref_out_opt BSTR *pTypeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Format( 
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormat) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Data( 
            /* [out] */ __RPC__out VARIANT *pVar) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Equals( 
            /* [in] */ __RPC__in_opt IVsUIObject *pOtherObject,
            /* [out] */ __RPC__out VARIANT_BOOL *pfAreEqual) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIObjectVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIObject * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIObject * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIObject * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IVsUIObject * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pTypeName);
        
        HRESULT ( STDMETHODCALLTYPE *get_Format )( 
            IVsUIObject * This,
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormat);
        
        HRESULT ( STDMETHODCALLTYPE *get_Data )( 
            IVsUIObject * This,
            /* [out] */ __RPC__out VARIANT *pVar);
        
        HRESULT ( STDMETHODCALLTYPE *Equals )( 
            IVsUIObject * This,
            /* [in] */ __RPC__in_opt IVsUIObject *pOtherObject,
            /* [out] */ __RPC__out VARIANT_BOOL *pfAreEqual);
        
        END_INTERFACE
    } IVsUIObjectVtbl;

    interface IVsUIObject
    {
        CONST_VTBL struct IVsUIObjectVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIObject_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIObject_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIObject_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIObject_get_Type(This,pTypeName)	\
    ( (This)->lpVtbl -> get_Type(This,pTypeName) ) 

#define IVsUIObject_get_Format(This,pdwDataFormat)	\
    ( (This)->lpVtbl -> get_Format(This,pdwDataFormat) ) 

#define IVsUIObject_get_Data(This,pVar)	\
    ( (This)->lpVtbl -> get_Data(This,pVar) ) 

#define IVsUIObject_Equals(This,pOtherObject,pfAreEqual)	\
    ( (This)->lpVtbl -> Equals(This,pOtherObject,pfAreEqual) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIObject_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataConverter_INTERFACE_DEFINED__
#define __IVsUIDataConverter_INTERFACE_DEFINED__

/* interface IVsUIDataConverter */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataConverter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6E48EB81-ADD0-4F9F-AF78-C02F053250B3")
    IVsUIDataConverter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Type( 
            /* [out] */ __RPC__deref_out_opt BSTR *pTypeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_ConvertibleFormats( 
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormatFrom,
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormatTo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Convert( 
            /* [in] */ __RPC__in_opt IVsUIObject *pObject,
            /* [out] */ __RPC__deref_out_opt IVsUIObject **ppConvertedObject) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDataConverterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDataConverter * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDataConverter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDataConverter * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IVsUIDataConverter * This,
            /* [out] */ __RPC__deref_out_opt BSTR *pTypeName);
        
        HRESULT ( STDMETHODCALLTYPE *get_ConvertibleFormats )( 
            IVsUIDataConverter * This,
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormatFrom,
            /* [out] */ __RPC__out VSUIDATAFORMAT *pdwDataFormatTo);
        
        HRESULT ( STDMETHODCALLTYPE *Convert )( 
            IVsUIDataConverter * This,
            /* [in] */ __RPC__in_opt IVsUIObject *pObject,
            /* [out] */ __RPC__deref_out_opt IVsUIObject **ppConvertedObject);
        
        END_INTERFACE
    } IVsUIDataConverterVtbl;

    interface IVsUIDataConverter
    {
        CONST_VTBL struct IVsUIDataConverterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataConverter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataConverter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataConverter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataConverter_get_Type(This,pTypeName)	\
    ( (This)->lpVtbl -> get_Type(This,pTypeName) ) 

#define IVsUIDataConverter_get_ConvertibleFormats(This,pdwDataFormatFrom,pdwDataFormatTo)	\
    ( (This)->lpVtbl -> get_ConvertibleFormats(This,pdwDataFormatFrom,pdwDataFormatTo) ) 

#define IVsUIDataConverter_Convert(This,pObject,ppConvertedObject)	\
    ( (This)->lpVtbl -> Convert(This,pObject,ppConvertedObject) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataConverter_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataConverterManager_INTERFACE_DEFINED__
#define __IVsUIDataConverterManager_INTERFACE_DEFINED__

/* interface IVsUIDataConverterManager */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataConverterManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("806BA229-8188-4663-A918-65B0E0CC0503")
    IVsUIDataConverterManager : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterConverter( 
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [in] */ __RPC__in_opt IVsUIDataConverter *pConverter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterConverter( 
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetConverter( 
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [out] */ __RPC__deref_out_opt IVsUIDataConverter **ppConverter) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetObjectConverter( 
            /* [in] */ __RPC__in_opt IVsUIObject *pObject,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [out] */ __RPC__deref_out_opt IVsUIDataConverter **ppConverter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDataConverterManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDataConverterManager * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDataConverterManager * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDataConverterManager * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterConverter )( 
            IVsUIDataConverterManager * This,
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [in] */ __RPC__in_opt IVsUIDataConverter *pConverter);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterConverter )( 
            IVsUIDataConverterManager * This,
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo);
        
        HRESULT ( STDMETHODCALLTYPE *GetConverter )( 
            IVsUIDataConverterManager * This,
            /* [in] */ __RPC__in LPCOLESTR typeName,
            /* [in] */ VSUIDATAFORMAT dwDataFormatFrom,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [out] */ __RPC__deref_out_opt IVsUIDataConverter **ppConverter);
        
        HRESULT ( STDMETHODCALLTYPE *GetObjectConverter )( 
            IVsUIDataConverterManager * This,
            /* [in] */ __RPC__in_opt IVsUIObject *pObject,
            /* [in] */ VSUIDATAFORMAT dwDataFormatTo,
            /* [out] */ __RPC__deref_out_opt IVsUIDataConverter **ppConverter);
        
        END_INTERFACE
    } IVsUIDataConverterManagerVtbl;

    interface IVsUIDataConverterManager
    {
        CONST_VTBL struct IVsUIDataConverterManagerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataConverterManager_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataConverterManager_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataConverterManager_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataConverterManager_RegisterConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo,pConverter)	\
    ( (This)->lpVtbl -> RegisterConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo,pConverter) ) 

#define IVsUIDataConverterManager_UnregisterConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo)	\
    ( (This)->lpVtbl -> UnregisterConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo) ) 

#define IVsUIDataConverterManager_GetConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo,ppConverter)	\
    ( (This)->lpVtbl -> GetConverter(This,typeName,dwDataFormatFrom,dwDataFormatTo,ppConverter) ) 

#define IVsUIDataConverterManager_GetObjectConverter(This,pObject,dwDataFormatTo,ppConverter)	\
    ( (This)->lpVtbl -> GetObjectConverter(This,pObject,dwDataFormatTo,ppConverter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataConverterManager_INTERFACE_DEFINED__ */


#ifndef __SVsUIDataConverters_INTERFACE_DEFINED__
#define __SVsUIDataConverters_INTERFACE_DEFINED__

/* interface SVsUIDataConverters */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsUIDataConverters;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3A4AEC59-CF2A-42c9-8DA8-66E19D7C547D")
    SVsUIDataConverters : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsUIDataConvertersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsUIDataConverters * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsUIDataConverters * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsUIDataConverters * This);
        
        END_INTERFACE
    } SVsUIDataConvertersVtbl;

    interface SVsUIDataConverters
    {
        CONST_VTBL struct SVsUIDataConvertersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsUIDataConverters_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsUIDataConverters_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsUIDataConverters_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsUIDataConverters_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0004 */
/* [local] */ 

#define SID_SVsUIDataConverters __uuidof(SVsUIDataConverters)
typedef struct tagVsUIPropertyDescriptor
    {
    BSTR name;
    BSTR type;
    } 	VsUIPropertyDescriptor;



extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0004_v0_0_s_ifspec;

#ifndef __IVsUIEnumDataSourceProperties_INTERFACE_DEFINED__
#define __IVsUIEnumDataSourceProperties_INTERFACE_DEFINED__

/* interface IVsUIEnumDataSourceProperties */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIEnumDataSourceProperties;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("327922B7-0B7F-4123-8446-0E614B337673")
    IVsUIEnumDataSourceProperties : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) VsUIPropertyDescriptor *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIEnumDataSourcePropertiesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIEnumDataSourceProperties * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIEnumDataSourceProperties * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIEnumDataSourceProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsUIEnumDataSourceProperties * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) VsUIPropertyDescriptor *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsUIEnumDataSourceProperties * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsUIEnumDataSourceProperties * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsUIEnumDataSourceProperties * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum);
        
        END_INTERFACE
    } IVsUIEnumDataSourcePropertiesVtbl;

    interface IVsUIEnumDataSourceProperties
    {
        CONST_VTBL struct IVsUIEnumDataSourcePropertiesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIEnumDataSourceProperties_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIEnumDataSourceProperties_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIEnumDataSourceProperties_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIEnumDataSourceProperties_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsUIEnumDataSourceProperties_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsUIEnumDataSourceProperties_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsUIEnumDataSourceProperties_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIEnumDataSourceProperties_INTERFACE_DEFINED__ */


#ifndef __IVsUIEnumDataSourceVerbs_INTERFACE_DEFINED__
#define __IVsUIEnumDataSourceVerbs_INTERFACE_DEFINED__

/* interface IVsUIEnumDataSourceVerbs */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIEnumDataSourceVerbs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51C2FFFB-35FA-4ad2-81B1-11816C482AAA")
    IVsUIEnumDataSourceVerbs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Next( 
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Skip( 
            /* [in] */ ULONG celt) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Clone( 
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIEnumDataSourceVerbsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIEnumDataSourceVerbs * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIEnumDataSourceVerbs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIEnumDataSourceVerbs * This);
        
        HRESULT ( STDMETHODCALLTYPE *Next )( 
            IVsUIEnumDataSourceVerbs * This,
            /* [in] */ ULONG celt,
            /* [length_is][size_is][out] */ __RPC__out_ecount_part(celt, *pceltFetched) BSTR *rgelt,
            /* [out] */ __RPC__out ULONG *pceltFetched);
        
        HRESULT ( STDMETHODCALLTYPE *Skip )( 
            IVsUIEnumDataSourceVerbs * This,
            /* [in] */ ULONG celt);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IVsUIEnumDataSourceVerbs * This);
        
        HRESULT ( STDMETHODCALLTYPE *Clone )( 
            IVsUIEnumDataSourceVerbs * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        END_INTERFACE
    } IVsUIEnumDataSourceVerbsVtbl;

    interface IVsUIEnumDataSourceVerbs
    {
        CONST_VTBL struct IVsUIEnumDataSourceVerbsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIEnumDataSourceVerbs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIEnumDataSourceVerbs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIEnumDataSourceVerbs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIEnumDataSourceVerbs_Next(This,celt,rgelt,pceltFetched)	\
    ( (This)->lpVtbl -> Next(This,celt,rgelt,pceltFetched) ) 

#define IVsUIEnumDataSourceVerbs_Skip(This,celt)	\
    ( (This)->lpVtbl -> Skip(This,celt) ) 

#define IVsUIEnumDataSourceVerbs_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IVsUIEnumDataSourceVerbs_Clone(This,ppEnum)	\
    ( (This)->lpVtbl -> Clone(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIEnumDataSourceVerbs_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0006 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0006_v0_0_s_ifspec;

#ifndef __IVsUIEventSink_INTERFACE_DEFINED__
#define __IVsUIEventSink_INTERFACE_DEFINED__

/* interface IVsUIEventSink */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIEventSink;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("515953AC-99C6-4F1B-8645-636A57E4B1E2")
    IVsUIEventSink : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Disconnect( 
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pSource) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIEventSinkVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIEventSink * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIEventSink * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIEventSink * This);
        
        HRESULT ( STDMETHODCALLTYPE *Disconnect )( 
            IVsUIEventSink * This,
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pSource);
        
        END_INTERFACE
    } IVsUIEventSinkVtbl;

    interface IVsUIEventSink
    {
        CONST_VTBL struct IVsUIEventSinkVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIEventSink_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIEventSink_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIEventSink_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIEventSink_Disconnect(This,pSource)	\
    ( (This)->lpVtbl -> Disconnect(This,pSource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIEventSink_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataSourcePropertyChangeEvents_INTERFACE_DEFINED__
#define __IVsUIDataSourcePropertyChangeEvents_INTERFACE_DEFINED__

/* interface IVsUIDataSourcePropertyChangeEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataSourcePropertyChangeEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EC495559-B090-435e-8D7E-3D95286A9BE8")
    IVsUIDataSourcePropertyChangeEvents : public IVsUIEventSink
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnPropertyChanged( 
            /* [in] */ __RPC__in_opt IVsUIDataSource *pDataSource,
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [in] */ __RPC__in_opt IVsUIObject *pVarOld,
            /* [in] */ __RPC__in_opt IVsUIObject *pVarNew) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDataSourcePropertyChangeEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDataSourcePropertyChangeEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDataSourcePropertyChangeEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDataSourcePropertyChangeEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *Disconnect )( 
            IVsUIDataSourcePropertyChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pSource);
        
        HRESULT ( STDMETHODCALLTYPE *OnPropertyChanged )( 
            IVsUIDataSourcePropertyChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pDataSource,
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [in] */ __RPC__in_opt IVsUIObject *pVarOld,
            /* [in] */ __RPC__in_opt IVsUIObject *pVarNew);
        
        END_INTERFACE
    } IVsUIDataSourcePropertyChangeEventsVtbl;

    interface IVsUIDataSourcePropertyChangeEvents
    {
        CONST_VTBL struct IVsUIDataSourcePropertyChangeEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataSourcePropertyChangeEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataSourcePropertyChangeEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataSourcePropertyChangeEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataSourcePropertyChangeEvents_Disconnect(This,pSource)	\
    ( (This)->lpVtbl -> Disconnect(This,pSource) ) 


#define IVsUIDataSourcePropertyChangeEvents_OnPropertyChanged(This,pDataSource,prop,pVarOld,pVarNew)	\
    ( (This)->lpVtbl -> OnPropertyChanged(This,pDataSource,prop,pVarOld,pVarNew) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataSourcePropertyChangeEvents_INTERFACE_DEFINED__ */


#ifndef __IVsUIDispatch_INTERFACE_DEFINED__
#define __IVsUIDispatch_INTERFACE_DEFINED__

/* interface IVsUIDispatch */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDispatch;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0DF3E43A-5356-4A33-8AC1-3BE6E3337C37")
    IVsUIDispatch : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Invoke( 
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumVerbs( 
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDispatchVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDispatch * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDispatch * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDispatch * This);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsUIDispatch * This,
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut);
        
        HRESULT ( STDMETHODCALLTYPE *EnumVerbs )( 
            IVsUIDispatch * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        END_INTERFACE
    } IVsUIDispatchVtbl;

    interface IVsUIDispatch
    {
        CONST_VTBL struct IVsUIDispatchVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDispatch_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDispatch_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDispatch_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDispatch_Invoke(This,verb,pvaIn,pvaOut)	\
    ( (This)->lpVtbl -> Invoke(This,verb,pvaIn,pvaOut) ) 

#define IVsUIDispatch_EnumVerbs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumVerbs(This,ppEnum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDispatch_INTERFACE_DEFINED__ */


#ifndef __IVsUISimpleDataSource_INTERFACE_DEFINED__
#define __IVsUISimpleDataSource_INTERFACE_DEFINED__

/* interface IVsUISimpleDataSource */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUISimpleDataSource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("110596DC-7A19-4E04-9106-1DB0580F77E9")
    IVsUISimpleDataSource : public IVsUIDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUISimpleDataSourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUISimpleDataSource * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUISimpleDataSource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUISimpleDataSource * This);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsUISimpleDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut);
        
        HRESULT ( STDMETHODCALLTYPE *EnumVerbs )( 
            IVsUISimpleDataSource * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsUISimpleDataSource * This);
        
        END_INTERFACE
    } IVsUISimpleDataSourceVtbl;

    interface IVsUISimpleDataSource
    {
        CONST_VTBL struct IVsUISimpleDataSourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUISimpleDataSource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUISimpleDataSource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUISimpleDataSource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUISimpleDataSource_Invoke(This,verb,pvaIn,pvaOut)	\
    ( (This)->lpVtbl -> Invoke(This,verb,pvaIn,pvaOut) ) 

#define IVsUISimpleDataSource_EnumVerbs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumVerbs(This,ppEnum) ) 


#define IVsUISimpleDataSource_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUISimpleDataSource_INTERFACE_DEFINED__ */


#ifndef __IVsUIDataSource_INTERFACE_DEFINED__
#define __IVsUIDataSource_INTERFACE_DEFINED__

/* interface IVsUIDataSource */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDataSource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8D11DD44-7EF2-4c7a-B188-7DA136657F68")
    IVsUIDataSource : public IVsUISimpleDataSource
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetValue( 
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [out] */ __RPC__deref_out_opt IVsUIObject **ppValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetValue( 
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [in] */ __RPC__in_opt IVsUIObject *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdvisePropertyChangeEvents( 
            /* [in] */ __RPC__in_opt IVsUIDataSourcePropertyChangeEvents *pAdvise,
            /* [out] */ __RPC__out VSUICOOKIE *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadvisePropertyChangeEvents( 
            /* [in] */ VSUICOOKIE cookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnumProperties( 
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetShapeIdentifier( 
            /* [out] */ __RPC__out GUID *pGuid,
            /* [out] */ __RPC__out DWORD *pdw) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE QueryValue( 
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pTypeName,
            /* [optional][out] */ __RPC__out VSUIDATAFORMAT *pDataFormat,
            /* [optional][out] */ __RPC__out VARIANT *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetValue( 
            /* [in] */ __RPC__in LPCOLESTR prop) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDataSourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDataSource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDataSource * This);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut);
        
        HRESULT ( STDMETHODCALLTYPE *EnumVerbs )( 
            IVsUIDataSource * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsUIDataSource * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetValue )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [out] */ __RPC__deref_out_opt IVsUIObject **ppValue);
        
        HRESULT ( STDMETHODCALLTYPE *SetValue )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [in] */ __RPC__in_opt IVsUIObject *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdvisePropertyChangeEvents )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in_opt IVsUIDataSourcePropertyChangeEvents *pAdvise,
            /* [out] */ __RPC__out VSUICOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadvisePropertyChangeEvents )( 
            IVsUIDataSource * This,
            /* [in] */ VSUICOOKIE cookie);
        
        HRESULT ( STDMETHODCALLTYPE *EnumProperties )( 
            IVsUIDataSource * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceProperties **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *GetShapeIdentifier )( 
            IVsUIDataSource * This,
            /* [out] */ __RPC__out GUID *pGuid,
            /* [out] */ __RPC__out DWORD *pdw);
        
        HRESULT ( STDMETHODCALLTYPE *QueryValue )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR prop,
            /* [optional][out] */ __RPC__deref_out_opt BSTR *pTypeName,
            /* [optional][out] */ __RPC__out VSUIDATAFORMAT *pDataFormat,
            /* [optional][out] */ __RPC__out VARIANT *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *ResetValue )( 
            IVsUIDataSource * This,
            /* [in] */ __RPC__in LPCOLESTR prop);
        
        END_INTERFACE
    } IVsUIDataSourceVtbl;

    interface IVsUIDataSource
    {
        CONST_VTBL struct IVsUIDataSourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDataSource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDataSource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDataSource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDataSource_Invoke(This,verb,pvaIn,pvaOut)	\
    ( (This)->lpVtbl -> Invoke(This,verb,pvaIn,pvaOut) ) 

#define IVsUIDataSource_EnumVerbs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumVerbs(This,ppEnum) ) 


#define IVsUIDataSource_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 


#define IVsUIDataSource_GetValue(This,prop,ppValue)	\
    ( (This)->lpVtbl -> GetValue(This,prop,ppValue) ) 

#define IVsUIDataSource_SetValue(This,prop,pValue)	\
    ( (This)->lpVtbl -> SetValue(This,prop,pValue) ) 

#define IVsUIDataSource_AdvisePropertyChangeEvents(This,pAdvise,pCookie)	\
    ( (This)->lpVtbl -> AdvisePropertyChangeEvents(This,pAdvise,pCookie) ) 

#define IVsUIDataSource_UnadvisePropertyChangeEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadvisePropertyChangeEvents(This,cookie) ) 

#define IVsUIDataSource_EnumProperties(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumProperties(This,ppEnum) ) 

#define IVsUIDataSource_GetShapeIdentifier(This,pGuid,pdw)	\
    ( (This)->lpVtbl -> GetShapeIdentifier(This,pGuid,pdw) ) 

#define IVsUIDataSource_QueryValue(This,prop,pTypeName,pDataFormat,pValue)	\
    ( (This)->lpVtbl -> QueryValue(This,prop,pTypeName,pDataFormat,pValue) ) 

#define IVsUIDataSource_ResetValue(This,prop)	\
    ( (This)->lpVtbl -> ResetValue(This,prop) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDataSource_INTERFACE_DEFINED__ */


#ifndef __IVsUICollectionChangeEvents_INTERFACE_DEFINED__
#define __IVsUICollectionChangeEvents_INTERFACE_DEFINED__

/* interface IVsUICollectionChangeEvents */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUICollectionChangeEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D47ABBE0-4E31-424d-8DC9-31DE024E75E7")
    IVsUICollectionChangeEvents : public IVsUIEventSink
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnAfterItemAdded( 
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ UINT nItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterItemRemoved( 
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pRemovedItem,
            /* [in] */ UINT nItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnAfterItemReplaced( 
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pNewItem,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pOldItem,
            /* [in] */ UINT nItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnInvalidateAllItems( 
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUICollectionChangeEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUICollectionChangeEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUICollectionChangeEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *Disconnect )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pSource);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterItemAdded )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ UINT nItem);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterItemRemoved )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pRemovedItem,
            /* [in] */ UINT nItem);
        
        HRESULT ( STDMETHODCALLTYPE *OnAfterItemReplaced )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pNewItem,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pOldItem,
            /* [in] */ UINT nItem);
        
        HRESULT ( STDMETHODCALLTYPE *OnInvalidateAllItems )( 
            IVsUICollectionChangeEvents * This,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection);
        
        END_INTERFACE
    } IVsUICollectionChangeEventsVtbl;

    interface IVsUICollectionChangeEvents
    {
        CONST_VTBL struct IVsUICollectionChangeEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUICollectionChangeEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUICollectionChangeEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUICollectionChangeEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUICollectionChangeEvents_Disconnect(This,pSource)	\
    ( (This)->lpVtbl -> Disconnect(This,pSource) ) 


#define IVsUICollectionChangeEvents_OnAfterItemAdded(This,pCollection,nItem)	\
    ( (This)->lpVtbl -> OnAfterItemAdded(This,pCollection,nItem) ) 

#define IVsUICollectionChangeEvents_OnAfterItemRemoved(This,pCollection,pRemovedItem,nItem)	\
    ( (This)->lpVtbl -> OnAfterItemRemoved(This,pCollection,pRemovedItem,nItem) ) 

#define IVsUICollectionChangeEvents_OnAfterItemReplaced(This,pCollection,pNewItem,pOldItem,nItem)	\
    ( (This)->lpVtbl -> OnAfterItemReplaced(This,pCollection,pNewItem,pOldItem,nItem) ) 

#define IVsUICollectionChangeEvents_OnInvalidateAllItems(This,pCollection)	\
    ( (This)->lpVtbl -> OnInvalidateAllItems(This,pCollection) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUICollectionChangeEvents_INTERFACE_DEFINED__ */


#ifndef __IVsUICollection_INTERFACE_DEFINED__
#define __IVsUICollection_INTERFACE_DEFINED__

/* interface IVsUICollection */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUICollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F9362B93-C6FD-4c51-8AF9-B4BC13953E6C")
    IVsUICollection : public IVsUISimpleDataSource
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Count( 
            /* [out] */ __RPC__out UINT *pnCount) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetItem( 
            /* [in] */ UINT nItem,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **pVsUIDataSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseCollectionChangeEvents( 
            /* [in] */ __RPC__in_opt IVsUICollectionChangeEvents *pAdvise,
            /* [out] */ __RPC__out VSUICOOKIE *pCookie) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseCollectionChangeEvents( 
            /* [in] */ VSUICOOKIE cookie) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUICollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUICollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUICollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUICollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsUICollection * This,
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut);
        
        HRESULT ( STDMETHODCALLTYPE *EnumVerbs )( 
            IVsUICollection * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsUICollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IVsUICollection * This,
            /* [out] */ __RPC__out UINT *pnCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetItem )( 
            IVsUICollection * This,
            /* [in] */ UINT nItem,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **pVsUIDataSource);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseCollectionChangeEvents )( 
            IVsUICollection * This,
            /* [in] */ __RPC__in_opt IVsUICollectionChangeEvents *pAdvise,
            /* [out] */ __RPC__out VSUICOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseCollectionChangeEvents )( 
            IVsUICollection * This,
            /* [in] */ VSUICOOKIE cookie);
        
        END_INTERFACE
    } IVsUICollectionVtbl;

    interface IVsUICollection
    {
        CONST_VTBL struct IVsUICollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUICollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUICollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUICollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUICollection_Invoke(This,verb,pvaIn,pvaOut)	\
    ( (This)->lpVtbl -> Invoke(This,verb,pvaIn,pvaOut) ) 

#define IVsUICollection_EnumVerbs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumVerbs(This,ppEnum) ) 


#define IVsUICollection_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 


#define IVsUICollection_get_Count(This,pnCount)	\
    ( (This)->lpVtbl -> get_Count(This,pnCount) ) 

#define IVsUICollection_GetItem(This,nItem,pVsUIDataSource)	\
    ( (This)->lpVtbl -> GetItem(This,nItem,pVsUIDataSource) ) 

#define IVsUICollection_AdviseCollectionChangeEvents(This,pAdvise,pCookie)	\
    ( (This)->lpVtbl -> AdviseCollectionChangeEvents(This,pAdvise,pCookie) ) 

#define IVsUICollection_UnadviseCollectionChangeEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadviseCollectionChangeEvents(This,cookie) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUICollection_INTERFACE_DEFINED__ */


#ifndef __IVsUIDynamicCollection_INTERFACE_DEFINED__
#define __IVsUIDynamicCollection_INTERFACE_DEFINED__

/* interface IVsUIDynamicCollection */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIDynamicCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6D155041-B4B8-4121-8D74-841E5DA4373E")
    IVsUIDynamicCollection : public IVsUICollection
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddItem( 
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem,
            /* [out] */ __RPC__out UINT *pIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InsertItem( 
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveItem( 
            /* [in] */ UINT nIndex) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceItem( 
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ClearItems( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InsertCollection( 
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIDynamicCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIDynamicCollection * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIDynamicCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIDynamicCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVsUIDynamicCollection * This,
            /* [in] */ __RPC__in LPCOLESTR verb,
            /* [in] */ VARIANT pvaIn,
            /* [out] */ __RPC__out VARIANT *pvaOut);
        
        HRESULT ( STDMETHODCALLTYPE *EnumVerbs )( 
            IVsUIDynamicCollection * This,
            /* [out] */ __RPC__deref_out_opt IVsUIEnumDataSourceVerbs **ppEnum);
        
        HRESULT ( STDMETHODCALLTYPE *Close )( 
            IVsUIDynamicCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IVsUIDynamicCollection * This,
            /* [out] */ __RPC__out UINT *pnCount);
        
        HRESULT ( STDMETHODCALLTYPE *GetItem )( 
            IVsUIDynamicCollection * This,
            /* [in] */ UINT nItem,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **pVsUIDataSource);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseCollectionChangeEvents )( 
            IVsUIDynamicCollection * This,
            /* [in] */ __RPC__in_opt IVsUICollectionChangeEvents *pAdvise,
            /* [out] */ __RPC__out VSUICOOKIE *pCookie);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseCollectionChangeEvents )( 
            IVsUIDynamicCollection * This,
            /* [in] */ VSUICOOKIE cookie);
        
        HRESULT ( STDMETHODCALLTYPE *AddItem )( 
            IVsUIDynamicCollection * This,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem,
            /* [out] */ __RPC__out UINT *pIndex);
        
        HRESULT ( STDMETHODCALLTYPE *InsertItem )( 
            IVsUIDynamicCollection * This,
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveItem )( 
            IVsUIDynamicCollection * This,
            /* [in] */ UINT nIndex);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceItem )( 
            IVsUIDynamicCollection * This,
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUIDataSource *pItem);
        
        HRESULT ( STDMETHODCALLTYPE *ClearItems )( 
            IVsUIDynamicCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *InsertCollection )( 
            IVsUIDynamicCollection * This,
            /* [in] */ UINT nIndex,
            /* [in] */ __RPC__in_opt IVsUICollection *pCollection);
        
        END_INTERFACE
    } IVsUIDynamicCollectionVtbl;

    interface IVsUIDynamicCollection
    {
        CONST_VTBL struct IVsUIDynamicCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIDynamicCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIDynamicCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIDynamicCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIDynamicCollection_Invoke(This,verb,pvaIn,pvaOut)	\
    ( (This)->lpVtbl -> Invoke(This,verb,pvaIn,pvaOut) ) 

#define IVsUIDynamicCollection_EnumVerbs(This,ppEnum)	\
    ( (This)->lpVtbl -> EnumVerbs(This,ppEnum) ) 


#define IVsUIDynamicCollection_Close(This)	\
    ( (This)->lpVtbl -> Close(This) ) 


#define IVsUIDynamicCollection_get_Count(This,pnCount)	\
    ( (This)->lpVtbl -> get_Count(This,pnCount) ) 

#define IVsUIDynamicCollection_GetItem(This,nItem,pVsUIDataSource)	\
    ( (This)->lpVtbl -> GetItem(This,nItem,pVsUIDataSource) ) 

#define IVsUIDynamicCollection_AdviseCollectionChangeEvents(This,pAdvise,pCookie)	\
    ( (This)->lpVtbl -> AdviseCollectionChangeEvents(This,pAdvise,pCookie) ) 

#define IVsUIDynamicCollection_UnadviseCollectionChangeEvents(This,cookie)	\
    ( (This)->lpVtbl -> UnadviseCollectionChangeEvents(This,cookie) ) 


#define IVsUIDynamicCollection_AddItem(This,pItem,pIndex)	\
    ( (This)->lpVtbl -> AddItem(This,pItem,pIndex) ) 

#define IVsUIDynamicCollection_InsertItem(This,nIndex,pItem)	\
    ( (This)->lpVtbl -> InsertItem(This,nIndex,pItem) ) 

#define IVsUIDynamicCollection_RemoveItem(This,nIndex)	\
    ( (This)->lpVtbl -> RemoveItem(This,nIndex) ) 

#define IVsUIDynamicCollection_ReplaceItem(This,nIndex,pItem)	\
    ( (This)->lpVtbl -> ReplaceItem(This,nIndex,pItem) ) 

#define IVsUIDynamicCollection_ClearItems(This)	\
    ( (This)->lpVtbl -> ClearItems(This) ) 

#define IVsUIDynamicCollection_InsertCollection(This,nIndex,pCollection)	\
    ( (This)->lpVtbl -> InsertCollection(This,nIndex,pCollection) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIDynamicCollection_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0014 */
/* [local] */ 

typedef 
enum __tagVSUIACCELMODIFIERS
    {	VSAM_NONE	= 0,
	VSAM_SHIFT	= 0x1,
	VSAM_CONTROL	= 0x2,
	VSAM_ALT	= 0x4,
	VSAM_WINDOWS	= 0x8
    } 	__VSUIACCELMODIFIERS;

typedef DWORD VSUIACCELMODIFIERS;



extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0014_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0014_v0_0_s_ifspec;

#ifndef __IVsUIAccelerator_INTERFACE_DEFINED__
#define __IVsUIAccelerator_INTERFACE_DEFINED__

/* interface IVsUIAccelerator */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIAccelerator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4E25556D-941D-4c29-A171-384EA84F6705")
    IVsUIAccelerator : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_Message( 
            /* [out] */ __RPC__out MSG *pMsg) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE get_Modifiers( 
            /* [out] */ __RPC__out VSUIACCELMODIFIERS *pdwModifiers) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIAcceleratorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIAccelerator * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIAccelerator * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIAccelerator * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_Message )( 
            IVsUIAccelerator * This,
            /* [out] */ __RPC__out MSG *pMsg);
        
        HRESULT ( STDMETHODCALLTYPE *get_Modifiers )( 
            IVsUIAccelerator * This,
            /* [out] */ __RPC__out VSUIACCELMODIFIERS *pdwModifiers);
        
        END_INTERFACE
    } IVsUIAcceleratorVtbl;

    interface IVsUIAccelerator
    {
        CONST_VTBL struct IVsUIAcceleratorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIAccelerator_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIAccelerator_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIAccelerator_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIAccelerator_get_Message(This,pMsg)	\
    ( (This)->lpVtbl -> get_Message(This,pMsg) ) 

#define IVsUIAccelerator_get_Modifiers(This,pdwModifiers)	\
    ( (This)->lpVtbl -> get_Modifiers(This,pdwModifiers) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIAccelerator_INTERFACE_DEFINED__ */


#ifndef __IVsUIElement_INTERFACE_DEFINED__
#define __IVsUIElement_INTERFACE_DEFINED__

/* interface IVsUIElement */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIElement;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("62C0A03E-4979-4b4e-90F0-56DF90521F79")
    IVsUIElement : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE get_DataSource( 
            /* [out] */ __RPC__deref_out_opt IVsUISimpleDataSource **ppDataSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE put_DataSource( 
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pDataSource) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TranslateAccelerator( 
            /* [in] */ __RPC__in_opt IVsUIAccelerator *pAccel) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUIObject( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIElementVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIElement * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIElement * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIElement * This);
        
        HRESULT ( STDMETHODCALLTYPE *get_DataSource )( 
            IVsUIElement * This,
            /* [out] */ __RPC__deref_out_opt IVsUISimpleDataSource **ppDataSource);
        
        HRESULT ( STDMETHODCALLTYPE *put_DataSource )( 
            IVsUIElement * This,
            /* [in] */ __RPC__in_opt IVsUISimpleDataSource *pDataSource);
        
        HRESULT ( STDMETHODCALLTYPE *TranslateAccelerator )( 
            IVsUIElement * This,
            /* [in] */ __RPC__in_opt IVsUIAccelerator *pAccel);
        
        HRESULT ( STDMETHODCALLTYPE *GetUIObject )( 
            IVsUIElement * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnk);
        
        END_INTERFACE
    } IVsUIElementVtbl;

    interface IVsUIElement
    {
        CONST_VTBL struct IVsUIElementVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIElement_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIElement_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIElement_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIElement_get_DataSource(This,ppDataSource)	\
    ( (This)->lpVtbl -> get_DataSource(This,ppDataSource) ) 

#define IVsUIElement_put_DataSource(This,pDataSource)	\
    ( (This)->lpVtbl -> put_DataSource(This,pDataSource) ) 

#define IVsUIElement_TranslateAccelerator(This,pAccel)	\
    ( (This)->lpVtbl -> TranslateAccelerator(This,pAccel) ) 

#define IVsUIElement_GetUIObject(This,ppUnk)	\
    ( (This)->lpVtbl -> GetUIObject(This,ppUnk) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIElement_INTERFACE_DEFINED__ */


#ifndef __IVsUIWpfElement_INTERFACE_DEFINED__
#define __IVsUIWpfElement_INTERFACE_DEFINED__

/* interface IVsUIWpfElement */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWpfElement;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CA87E95D-5AEE-4A16-BDCA-94A1F7F769A9")
    IVsUIWpfElement : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateFrameworkElement( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkElement) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFrameworkElement( 
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkElement) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWpfElementVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWpfElement * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWpfElement * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWpfElement * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateFrameworkElement )( 
            IVsUIWpfElement * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkElement);
        
        HRESULT ( STDMETHODCALLTYPE *GetFrameworkElement )( 
            IVsUIWpfElement * This,
            /* [out] */ __RPC__deref_out_opt IUnknown **ppUnkElement);
        
        END_INTERFACE
    } IVsUIWpfElementVtbl;

    interface IVsUIWpfElement
    {
        CONST_VTBL struct IVsUIWpfElementVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWpfElement_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWpfElement_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWpfElement_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWpfElement_CreateFrameworkElement(This,ppUnkElement)	\
    ( (This)->lpVtbl -> CreateFrameworkElement(This,ppUnkElement) ) 

#define IVsUIWpfElement_GetFrameworkElement(This,ppUnkElement)	\
    ( (This)->lpVtbl -> GetFrameworkElement(This,ppUnkElement) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWpfElement_INTERFACE_DEFINED__ */


#ifndef __IVsUIWin32Element_INTERFACE_DEFINED__
#define __IVsUIWin32Element_INTERFACE_DEFINED__

/* interface IVsUIWin32Element */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWin32Element;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AD9A00F2-AC5B-4A49-94B7-17CC3CE1A46A")
    IVsUIWin32Element : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Create( 
            HWND parent,
            /* [out] */ HWND *pHandle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Destroy( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHandle( 
            /* [out] */ HWND *pHandle) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowModal( 
            HWND parent,
            /* [out] */ int *pDlgResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWin32ElementVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWin32Element * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWin32Element * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWin32Element * This);
        
        HRESULT ( STDMETHODCALLTYPE *Create )( 
            IVsUIWin32Element * This,
            HWND parent,
            /* [out] */ HWND *pHandle);
        
        HRESULT ( STDMETHODCALLTYPE *Destroy )( 
            IVsUIWin32Element * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHandle )( 
            IVsUIWin32Element * This,
            /* [out] */ HWND *pHandle);
        
        HRESULT ( STDMETHODCALLTYPE *ShowModal )( 
            IVsUIWin32Element * This,
            HWND parent,
            /* [out] */ int *pDlgResult);
        
        END_INTERFACE
    } IVsUIWin32ElementVtbl;

    interface IVsUIWin32Element
    {
        CONST_VTBL struct IVsUIWin32ElementVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWin32Element_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWin32Element_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWin32Element_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWin32Element_Create(This,parent,pHandle)	\
    ( (This)->lpVtbl -> Create(This,parent,pHandle) ) 

#define IVsUIWin32Element_Destroy(This)	\
    ( (This)->lpVtbl -> Destroy(This) ) 

#define IVsUIWin32Element_GetHandle(This,pHandle)	\
    ( (This)->lpVtbl -> GetHandle(This,pHandle) ) 

#define IVsUIWin32Element_ShowModal(This,parent,pDlgResult)	\
    ( (This)->lpVtbl -> ShowModal(This,parent,pDlgResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWin32Element_INTERFACE_DEFINED__ */


#ifndef __IVsUIWpfLoader_INTERFACE_DEFINED__
#define __IVsUIWpfLoader_INTERFACE_DEFINED__

/* interface IVsUIWpfLoader */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWpfLoader;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("89DB8AB3-9035-4016-AA8A-76F7AE09B65F")
    IVsUIWpfLoader : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateUIElement( 
            /* [in] */ LPCWSTR elementFQN,
            /* [in] */ LPCWSTR codeBase,
            /* [out] */ IVsUIElement **ppUIElement) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateUIElementOfType( 
            /* [in] */ IUnknown *pUnkElementType,
            /* [out] */ IVsUIElement **ppUIElement) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ShowModalElement( 
            /* [in] */ IVsUIElement *pUIElement,
            /* [in] */ HWND hWndParent,
            /* [out] */ int *pResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWpfLoaderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWpfLoader * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWpfLoader * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWpfLoader * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateUIElement )( 
            IVsUIWpfLoader * This,
            /* [in] */ LPCWSTR elementFQN,
            /* [in] */ LPCWSTR codeBase,
            /* [out] */ IVsUIElement **ppUIElement);
        
        HRESULT ( STDMETHODCALLTYPE *CreateUIElementOfType )( 
            IVsUIWpfLoader * This,
            /* [in] */ IUnknown *pUnkElementType,
            /* [out] */ IVsUIElement **ppUIElement);
        
        HRESULT ( STDMETHODCALLTYPE *ShowModalElement )( 
            IVsUIWpfLoader * This,
            /* [in] */ IVsUIElement *pUIElement,
            /* [in] */ HWND hWndParent,
            /* [out] */ int *pResult);
        
        END_INTERFACE
    } IVsUIWpfLoaderVtbl;

    interface IVsUIWpfLoader
    {
        CONST_VTBL struct IVsUIWpfLoaderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWpfLoader_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWpfLoader_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWpfLoader_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWpfLoader_CreateUIElement(This,elementFQN,codeBase,ppUIElement)	\
    ( (This)->lpVtbl -> CreateUIElement(This,elementFQN,codeBase,ppUIElement) ) 

#define IVsUIWpfLoader_CreateUIElementOfType(This,pUnkElementType,ppUIElement)	\
    ( (This)->lpVtbl -> CreateUIElementOfType(This,pUnkElementType,ppUIElement) ) 

#define IVsUIWpfLoader_ShowModalElement(This,pUIElement,hWndParent,pResult)	\
    ( (This)->lpVtbl -> ShowModalElement(This,pUIElement,hWndParent,pResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWpfLoader_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0019 */
/* [local] */ 

extern const __declspec(selectany) CLSID CLSID_VsUIWpfLoader = { 0x0B127700, 0x143C, 0x4AB5, { 0x9D, 0x39,  0xBF, 0xF4, 0x71, 0x51, 0xB5, 0x63 } };


extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0019_v0_0_s_ifspec;

#ifndef __IVsUIFactory_INTERFACE_DEFINED__
#define __IVsUIFactory_INTERFACE_DEFINED__

/* interface IVsUIFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D416BA0D-25C6-463b-B2BD-F06142F0D4B7")
    IVsUIFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateUIElement( 
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIElement **ppUIElement) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateUIElement )( 
            IVsUIFactory * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIElement **ppUIElement);
        
        END_INTERFACE
    } IVsUIFactoryVtbl;

    interface IVsUIFactory
    {
        CONST_VTBL struct IVsUIFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIFactory_CreateUIElement(This,guid,dw,ppUIElement)	\
    ( (This)->lpVtbl -> CreateUIElement(This,guid,dw,ppUIElement) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIFactory_INTERFACE_DEFINED__ */


#ifndef __IVsRegisterUIFactories_INTERFACE_DEFINED__
#define __IVsRegisterUIFactories_INTERFACE_DEFINED__

/* interface IVsRegisterUIFactories */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterUIFactories;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A146BAC8-4337-4d8f-8C51-9B5147DBCB8A")
    IVsRegisterUIFactories : public IVsUIFactory
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterUIFactory( 
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ __RPC__in_opt IVsUIFactory *pUIFactory) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterUIFactoriesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterUIFactories * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterUIFactories * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterUIFactories * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateUIElement )( 
            IVsRegisterUIFactories * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIElement **ppUIElement);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterUIFactory )( 
            IVsRegisterUIFactories * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ __RPC__in_opt IVsUIFactory *pUIFactory);
        
        END_INTERFACE
    } IVsRegisterUIFactoriesVtbl;

    interface IVsRegisterUIFactories
    {
        CONST_VTBL struct IVsRegisterUIFactoriesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterUIFactories_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterUIFactories_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterUIFactories_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterUIFactories_CreateUIElement(This,guid,dw,ppUIElement)	\
    ( (This)->lpVtbl -> CreateUIElement(This,guid,dw,ppUIElement) ) 


#define IVsRegisterUIFactories_RegisterUIFactory(This,guid,pUIFactory)	\
    ( (This)->lpVtbl -> RegisterUIFactory(This,guid,pUIFactory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterUIFactories_INTERFACE_DEFINED__ */


#ifndef __SVsUIFactory_INTERFACE_DEFINED__
#define __SVsUIFactory_INTERFACE_DEFINED__

/* interface SVsUIFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsUIFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("570352BF-9B42-4c1f-B0C4-A8323D8F7BD3")
    SVsUIFactory : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsUIFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsUIFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsUIFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsUIFactory * This);
        
        END_INTERFACE
    } SVsUIFactoryVtbl;

    interface SVsUIFactory
    {
        CONST_VTBL struct SVsUIFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsUIFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsUIFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsUIFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsUIFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0022 */
/* [local] */ 

#define SID_SVsUIFactory __uuidof(SVsUIFactory)


extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0022_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0022_v0_0_s_ifspec;

#ifndef __IVsDataSourceFactory_INTERFACE_DEFINED__
#define __IVsDataSourceFactory_INTERFACE_DEFINED__

/* interface IVsDataSourceFactory */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsDataSourceFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("24034437-CB2E-47dd-AE2B-14D56481A2F0")
    IVsDataSourceFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDataSource( 
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **ppUIDataSource) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsDataSourceFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsDataSourceFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsDataSourceFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsDataSourceFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDataSource )( 
            IVsDataSourceFactory * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **ppUIDataSource);
        
        END_INTERFACE
    } IVsDataSourceFactoryVtbl;

    interface IVsDataSourceFactory
    {
        CONST_VTBL struct IVsDataSourceFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsDataSourceFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsDataSourceFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsDataSourceFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsDataSourceFactory_GetDataSource(This,guid,dw,ppUIDataSource)	\
    ( (This)->lpVtbl -> GetDataSource(This,guid,dw,ppUIDataSource) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsDataSourceFactory_INTERFACE_DEFINED__ */


#ifndef __IVsRegisterDataSourceFactories_INTERFACE_DEFINED__
#define __IVsRegisterDataSourceFactories_INTERFACE_DEFINED__

/* interface IVsRegisterDataSourceFactories */
/* [object][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsRegisterDataSourceFactories;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("113592FE-DEE5-48ad-9D66-5B26794A4003")
    IVsRegisterDataSourceFactories : public IVsDataSourceFactory
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterDataSourceFactory( 
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ __RPC__in_opt IVsDataSourceFactory *pDataSourceFactory) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsRegisterDataSourceFactoriesVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsRegisterDataSourceFactories * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsRegisterDataSourceFactories * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsRegisterDataSourceFactories * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDataSource )( 
            IVsRegisterDataSourceFactories * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ DWORD dw,
            /* [out] */ __RPC__deref_out_opt IVsUIDataSource **ppUIDataSource);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterDataSourceFactory )( 
            IVsRegisterDataSourceFactories * This,
            /* [in] */ __RPC__in REFGUID guid,
            /* [in] */ __RPC__in_opt IVsDataSourceFactory *pDataSourceFactory);
        
        END_INTERFACE
    } IVsRegisterDataSourceFactoriesVtbl;

    interface IVsRegisterDataSourceFactories
    {
        CONST_VTBL struct IVsRegisterDataSourceFactoriesVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsRegisterDataSourceFactories_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsRegisterDataSourceFactories_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsRegisterDataSourceFactories_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsRegisterDataSourceFactories_GetDataSource(This,guid,dw,ppUIDataSource)	\
    ( (This)->lpVtbl -> GetDataSource(This,guid,dw,ppUIDataSource) ) 


#define IVsRegisterDataSourceFactories_RegisterDataSourceFactory(This,guid,pDataSourceFactory)	\
    ( (This)->lpVtbl -> RegisterDataSourceFactory(This,guid,pDataSourceFactory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsRegisterDataSourceFactories_INTERFACE_DEFINED__ */


#ifndef __SVsDataSourceFactory_INTERFACE_DEFINED__
#define __SVsDataSourceFactory_INTERFACE_DEFINED__

/* interface SVsDataSourceFactory */
/* [object][uuid] */ 


EXTERN_C const IID IID_SVsDataSourceFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BA361A60-1C0E-40db-BAD6-503413FC3AD2")
    SVsDataSourceFactory : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct SVsDataSourceFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            SVsDataSourceFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            SVsDataSourceFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            SVsDataSourceFactory * This);
        
        END_INTERFACE
    } SVsDataSourceFactoryVtbl;

    interface SVsDataSourceFactory
    {
        CONST_VTBL struct SVsDataSourceFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define SVsDataSourceFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define SVsDataSourceFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define SVsDataSourceFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __SVsDataSourceFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_VsPlatformUI_0000_0025 */
/* [local] */ 

#define SID_SVsDataSourceFactory __uuidof(SVsDataSourceFactory)


extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0025_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VsPlatformUI_0000_0025_v0_0_s_ifspec;

#ifndef __IVsUIWin32Icon_INTERFACE_DEFINED__
#define __IVsUIWin32Icon_INTERFACE_DEFINED__

/* interface IVsUIWin32Icon */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWin32Icon;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("55445E7E-DAD5-4C41-9F38-03511D922D1E")
    IVsUIWin32Icon : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHICON( 
            /* [out] */ INT_PTR *phIcon) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWin32IconVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWin32Icon * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWin32Icon * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWin32Icon * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHICON )( 
            IVsUIWin32Icon * This,
            /* [out] */ INT_PTR *phIcon);
        
        END_INTERFACE
    } IVsUIWin32IconVtbl;

    interface IVsUIWin32Icon
    {
        CONST_VTBL struct IVsUIWin32IconVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWin32Icon_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWin32Icon_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWin32Icon_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWin32Icon_GetHICON(This,phIcon)	\
    ( (This)->lpVtbl -> GetHICON(This,phIcon) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWin32Icon_INTERFACE_DEFINED__ */


#ifndef __IVsUIWin32ImageList_INTERFACE_DEFINED__
#define __IVsUIWin32ImageList_INTERFACE_DEFINED__

/* interface IVsUIWin32ImageList */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWin32ImageList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5EA86615-2308-4da4-8A7A-E442FE40C44F")
    IVsUIWin32ImageList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHIMAGELIST( 
            /* [out] */ INT_PTR *phImageList) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWin32ImageListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWin32ImageList * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWin32ImageList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWin32ImageList * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHIMAGELIST )( 
            IVsUIWin32ImageList * This,
            /* [out] */ INT_PTR *phImageList);
        
        END_INTERFACE
    } IVsUIWin32ImageListVtbl;

    interface IVsUIWin32ImageList
    {
        CONST_VTBL struct IVsUIWin32ImageListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWin32ImageList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWin32ImageList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWin32ImageList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWin32ImageList_GetHIMAGELIST(This,phImageList)	\
    ( (This)->lpVtbl -> GetHIMAGELIST(This,phImageList) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWin32ImageList_INTERFACE_DEFINED__ */


#ifndef __IVsUIWin32Bitmap_INTERFACE_DEFINED__
#define __IVsUIWin32Bitmap_INTERFACE_DEFINED__

/* interface IVsUIWin32Bitmap */
/* [object][local][unique][version][uuid] */ 


EXTERN_C const IID IID_IVsUIWin32Bitmap;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0DD7CF3A-6203-466F-B1C1-7653809CB73A")
    IVsUIWin32Bitmap : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHBITMAP( 
            /* [out] */ INT_PTR *phBitmap) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE BitmapContainsAlphaInfo( 
            /* [out] */ VARIANT_BOOL *pfHasAlpha) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVsUIWin32BitmapVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVsUIWin32Bitmap * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVsUIWin32Bitmap * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVsUIWin32Bitmap * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHBITMAP )( 
            IVsUIWin32Bitmap * This,
            /* [out] */ INT_PTR *phBitmap);
        
        HRESULT ( STDMETHODCALLTYPE *BitmapContainsAlphaInfo )( 
            IVsUIWin32Bitmap * This,
            /* [out] */ VARIANT_BOOL *pfHasAlpha);
        
        END_INTERFACE
    } IVsUIWin32BitmapVtbl;

    interface IVsUIWin32Bitmap
    {
        CONST_VTBL struct IVsUIWin32BitmapVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVsUIWin32Bitmap_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVsUIWin32Bitmap_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVsUIWin32Bitmap_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVsUIWin32Bitmap_GetHBITMAP(This,phBitmap)	\
    ( (This)->lpVtbl -> GetHBITMAP(This,phBitmap) ) 

#define IVsUIWin32Bitmap_BitmapContainsAlphaInfo(This,pfHasAlpha)	\
    ( (This)->lpVtbl -> BitmapContainsAlphaInfo(This,pfHasAlpha) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVsUIWin32Bitmap_INTERFACE_DEFINED__ */


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


