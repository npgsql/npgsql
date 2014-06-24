

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for VSEditor.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
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

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __VSEditor_h__
#define __VSEditor_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IEnumeratorBSTR_FWD_DEFINED__
#define __IEnumeratorBSTR_FWD_DEFINED__
typedef interface IEnumeratorBSTR IEnumeratorBSTR;
#endif 	/* __IEnumeratorBSTR_FWD_DEFINED__ */


#ifndef __IEnumerableBSTR_FWD_DEFINED__
#define __IEnumerableBSTR_FWD_DEFINED__
typedef interface IEnumerableBSTR IEnumerableBSTR;
#endif 	/* __IEnumerableBSTR_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxContentType_FWD_DEFINED__
#define __IEnumeratorIVxContentType_FWD_DEFINED__
typedef interface IEnumeratorIVxContentType IEnumeratorIVxContentType;
#endif 	/* __IEnumeratorIVxContentType_FWD_DEFINED__ */


#ifndef __IEnumerableIVxContentType_FWD_DEFINED__
#define __IEnumerableIVxContentType_FWD_DEFINED__
typedef interface IEnumerableIVxContentType IEnumerableIVxContentType;
#endif 	/* __IEnumerableIVxContentType_FWD_DEFINED__ */


#ifndef __IVxTextDocumentFileActionEvent_FWD_DEFINED__
#define __IVxTextDocumentFileActionEvent_FWD_DEFINED__
typedef interface IVxTextDocumentFileActionEvent IVxTextDocumentFileActionEvent;
#endif 	/* __IVxTextDocumentFileActionEvent_FWD_DEFINED__ */


#ifndef __IVxEventArgsEvent_FWD_DEFINED__
#define __IVxEventArgsEvent_FWD_DEFINED__
typedef interface IVxEventArgsEvent IVxEventArgsEvent;
#endif 	/* __IVxEventArgsEvent_FWD_DEFINED__ */


#ifndef __IVxTextDocumentEvent_FWD_DEFINED__
#define __IVxTextDocumentEvent_FWD_DEFINED__
typedef interface IVxTextDocumentEvent IVxTextDocumentEvent;
#endif 	/* __IVxTextDocumentEvent_FWD_DEFINED__ */


#ifndef __IVxSnapshotSpanEvent_FWD_DEFINED__
#define __IVxSnapshotSpanEvent_FWD_DEFINED__
typedef interface IVxSnapshotSpanEvent IVxSnapshotSpanEvent;
#endif 	/* __IVxSnapshotSpanEvent_FWD_DEFINED__ */


#ifndef __IVxTextContentChangedEvent_FWD_DEFINED__
#define __IVxTextContentChangedEvent_FWD_DEFINED__
typedef interface IVxTextContentChangedEvent IVxTextContentChangedEvent;
#endif 	/* __IVxTextContentChangedEvent_FWD_DEFINED__ */


#ifndef __IVxTextContentChangingEvent_FWD_DEFINED__
#define __IVxTextContentChangingEvent_FWD_DEFINED__
typedef interface IVxTextContentChangingEvent IVxTextContentChangingEvent;
#endif 	/* __IVxTextContentChangingEvent_FWD_DEFINED__ */


#ifndef __IVxContentTypeChangedEvent_FWD_DEFINED__
#define __IVxContentTypeChangedEvent_FWD_DEFINED__
typedef interface IVxContentTypeChangedEvent IVxContentTypeChangedEvent;
#endif 	/* __IVxContentTypeChangedEvent_FWD_DEFINED__ */


#ifndef __IVxTextBufferCreatedEvent_FWD_DEFINED__
#define __IVxTextBufferCreatedEvent_FWD_DEFINED__
typedef interface IVxTextBufferCreatedEvent IVxTextBufferCreatedEvent;
#endif 	/* __IVxTextBufferCreatedEvent_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxTextSnapshotLine_FWD_DEFINED__
#define __IEnumeratorIVxTextSnapshotLine_FWD_DEFINED__
typedef interface IEnumeratorIVxTextSnapshotLine IEnumeratorIVxTextSnapshotLine;
#endif 	/* __IEnumeratorIVxTextSnapshotLine_FWD_DEFINED__ */


#ifndef __IEnumerableIVxTextSnapshotLine_FWD_DEFINED__
#define __IEnumerableIVxTextSnapshotLine_FWD_DEFINED__
typedef interface IEnumerableIVxTextSnapshotLine IEnumerableIVxTextSnapshotLine;
#endif 	/* __IEnumerableIVxTextSnapshotLine_FWD_DEFINED__ */


#ifndef __IEnumeratorVxSpan_FWD_DEFINED__
#define __IEnumeratorVxSpan_FWD_DEFINED__
typedef interface IEnumeratorVxSpan IEnumeratorVxSpan;
#endif 	/* __IEnumeratorVxSpan_FWD_DEFINED__ */


#ifndef __IListVxSpan_FWD_DEFINED__
#define __IListVxSpan_FWD_DEFINED__
typedef interface IListVxSpan IListVxSpan;
#endif 	/* __IListVxSpan_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxTextBuffer_FWD_DEFINED__
#define __IEnumeratorIVxTextBuffer_FWD_DEFINED__
typedef interface IEnumeratorIVxTextBuffer IEnumeratorIVxTextBuffer;
#endif 	/* __IEnumeratorIVxTextBuffer_FWD_DEFINED__ */


#ifndef __IListIVxTextBuffer_FWD_DEFINED__
#define __IListIVxTextBuffer_FWD_DEFINED__
typedef interface IListIVxTextBuffer IListIVxTextBuffer;
#endif 	/* __IListIVxTextBuffer_FWD_DEFINED__ */


#ifndef __IVxGraphBuffersChangedEvent_FWD_DEFINED__
#define __IVxGraphBuffersChangedEvent_FWD_DEFINED__
typedef interface IVxGraphBuffersChangedEvent IVxGraphBuffersChangedEvent;
#endif 	/* __IVxGraphBuffersChangedEvent_FWD_DEFINED__ */


#ifndef __IVxGraphBufferContentTypeChangedEvent_FWD_DEFINED__
#define __IVxGraphBufferContentTypeChangedEvent_FWD_DEFINED__
typedef interface IVxGraphBufferContentTypeChangedEvent IVxGraphBufferContentTypeChangedEvent;
#endif 	/* __IVxGraphBufferContentTypeChangedEvent_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxTextSnapshot_FWD_DEFINED__
#define __IEnumeratorIVxTextSnapshot_FWD_DEFINED__
typedef interface IEnumeratorIVxTextSnapshot IEnumeratorIVxTextSnapshot;
#endif 	/* __IEnumeratorIVxTextSnapshot_FWD_DEFINED__ */


#ifndef __IListIVxTextSnapshot_FWD_DEFINED__
#define __IListIVxTextSnapshot_FWD_DEFINED__
typedef interface IListIVxTextSnapshot IListIVxTextSnapshot;
#endif 	/* __IListIVxTextSnapshot_FWD_DEFINED__ */


#ifndef __IEnumeratorVxSnapshotSpan_FWD_DEFINED__
#define __IEnumeratorVxSnapshotSpan_FWD_DEFINED__
typedef interface IEnumeratorVxSnapshotSpan IEnumeratorVxSnapshotSpan;
#endif 	/* __IEnumeratorVxSnapshotSpan_FWD_DEFINED__ */


#ifndef __IListVxSnapshotSpan_FWD_DEFINED__
#define __IListVxSnapshotSpan_FWD_DEFINED__
typedef interface IListVxSnapshotSpan IListVxSnapshotSpan;
#endif 	/* __IListVxSnapshotSpan_FWD_DEFINED__ */


#ifndef __IEnumeratorVxSnapshotPoint_FWD_DEFINED__
#define __IEnumeratorVxSnapshotPoint_FWD_DEFINED__
typedef interface IEnumeratorVxSnapshotPoint IEnumeratorVxSnapshotPoint;
#endif 	/* __IEnumeratorVxSnapshotPoint_FWD_DEFINED__ */


#ifndef __IListVxSnapshotPoint_FWD_DEFINED__
#define __IListVxSnapshotPoint_FWD_DEFINED__
typedef interface IListVxSnapshotPoint IListVxSnapshotPoint;
#endif 	/* __IListVxSnapshotPoint_FWD_DEFINED__ */


#ifndef __IEnumeratorIUnknown_FWD_DEFINED__
#define __IEnumeratorIUnknown_FWD_DEFINED__
typedef interface IEnumeratorIUnknown IEnumeratorIUnknown;
#endif 	/* __IEnumeratorIUnknown_FWD_DEFINED__ */


#ifndef __IListIUnknown_FWD_DEFINED__
#define __IListIUnknown_FWD_DEFINED__
typedef interface IListIUnknown IListIUnknown;
#endif 	/* __IListIUnknown_FWD_DEFINED__ */


#ifndef __IVxProjectionSourceSpansChangedEvent_FWD_DEFINED__
#define __IVxProjectionSourceSpansChangedEvent_FWD_DEFINED__
typedef interface IVxProjectionSourceSpansChangedEvent IVxProjectionSourceSpansChangedEvent;
#endif 	/* __IVxProjectionSourceSpansChangedEvent_FWD_DEFINED__ */


#ifndef __IVxProjectionSourceBuffersChangedEvent_FWD_DEFINED__
#define __IVxProjectionSourceBuffersChangedEvent_FWD_DEFINED__
typedef interface IVxProjectionSourceBuffersChangedEvent IVxProjectionSourceBuffersChangedEvent;
#endif 	/* __IVxProjectionSourceBuffersChangedEvent_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxTrackingSpan_FWD_DEFINED__
#define __IEnumeratorIVxTrackingSpan_FWD_DEFINED__
typedef interface IEnumeratorIVxTrackingSpan IEnumeratorIVxTrackingSpan;
#endif 	/* __IEnumeratorIVxTrackingSpan_FWD_DEFINED__ */


#ifndef __IListIVxTrackingSpan_FWD_DEFINED__
#define __IListIVxTrackingSpan_FWD_DEFINED__
typedef interface IListIVxTrackingSpan IListIVxTrackingSpan;
#endif 	/* __IListIVxTrackingSpan_FWD_DEFINED__ */


#ifndef __IEnumeratorIVxTextChange_FWD_DEFINED__
#define __IEnumeratorIVxTextChange_FWD_DEFINED__
typedef interface IEnumeratorIVxTextChange IEnumeratorIVxTextChange;
#endif 	/* __IEnumeratorIVxTextChange_FWD_DEFINED__ */


#ifndef __IListIVxTextChange_FWD_DEFINED__
#define __IListIVxTextChange_FWD_DEFINED__
typedef interface IListIVxTextChange IListIVxTextChange;
#endif 	/* __IListIVxTextChange_FWD_DEFINED__ */


#ifndef __IVxDisposable_FWD_DEFINED__
#define __IVxDisposable_FWD_DEFINED__
typedef interface IVxDisposable IVxDisposable;
#endif 	/* __IVxDisposable_FWD_DEFINED__ */


#ifndef __IVxPropertyOwner_FWD_DEFINED__
#define __IVxPropertyOwner_FWD_DEFINED__
typedef interface IVxPropertyOwner IVxPropertyOwner;
#endif 	/* __IVxPropertyOwner_FWD_DEFINED__ */


#ifndef __IVxPropertyCollection_FWD_DEFINED__
#define __IVxPropertyCollection_FWD_DEFINED__
typedef interface IVxPropertyCollection IVxPropertyCollection;
#endif 	/* __IVxPropertyCollection_FWD_DEFINED__ */


#ifndef __IVxContentTypeRegistryService_FWD_DEFINED__
#define __IVxContentTypeRegistryService_FWD_DEFINED__
typedef interface IVxContentTypeRegistryService IVxContentTypeRegistryService;
#endif 	/* __IVxContentTypeRegistryService_FWD_DEFINED__ */


#ifndef __IVxContentType_FWD_DEFINED__
#define __IVxContentType_FWD_DEFINED__
typedef interface IVxContentType IVxContentType;
#endif 	/* __IVxContentType_FWD_DEFINED__ */


#ifndef __IVxTextDocument_FWD_DEFINED__
#define __IVxTextDocument_FWD_DEFINED__
typedef interface IVxTextDocument IVxTextDocument;
#endif 	/* __IVxTextDocument_FWD_DEFINED__ */


#ifndef __IVxTextDocumentFactoryService_FWD_DEFINED__
#define __IVxTextDocumentFactoryService_FWD_DEFINED__
typedef interface IVxTextDocumentFactoryService IVxTextDocumentFactoryService;
#endif 	/* __IVxTextDocumentFactoryService_FWD_DEFINED__ */


#ifndef __IVxTextDocumentEventArgs_FWD_DEFINED__
#define __IVxTextDocumentEventArgs_FWD_DEFINED__
typedef interface IVxTextDocumentEventArgs IVxTextDocumentEventArgs;
#endif 	/* __IVxTextDocumentEventArgs_FWD_DEFINED__ */


#ifndef __IVxTextDocumentFileActionEventArgs_FWD_DEFINED__
#define __IVxTextDocumentFileActionEventArgs_FWD_DEFINED__
typedef interface IVxTextDocumentFileActionEventArgs IVxTextDocumentFileActionEventArgs;
#endif 	/* __IVxTextDocumentFileActionEventArgs_FWD_DEFINED__ */


#ifndef __IVxTextSnapshotChangedEventArgs_FWD_DEFINED__
#define __IVxTextSnapshotChangedEventArgs_FWD_DEFINED__
typedef interface IVxTextSnapshotChangedEventArgs IVxTextSnapshotChangedEventArgs;
#endif 	/* __IVxTextSnapshotChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxContentTypeChangedEventArgs_FWD_DEFINED__
#define __IVxContentTypeChangedEventArgs_FWD_DEFINED__
typedef interface IVxContentTypeChangedEventArgs IVxContentTypeChangedEventArgs;
#endif 	/* __IVxContentTypeChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxMappingPoint_FWD_DEFINED__
#define __IVxMappingPoint_FWD_DEFINED__
typedef interface IVxMappingPoint IVxMappingPoint;
#endif 	/* __IVxMappingPoint_FWD_DEFINED__ */


#ifndef __IVxMappingSpan_FWD_DEFINED__
#define __IVxMappingSpan_FWD_DEFINED__
typedef interface IVxMappingSpan IVxMappingSpan;
#endif 	/* __IVxMappingSpan_FWD_DEFINED__ */


#ifndef __IVxNormalizedTextChangeCollection_FWD_DEFINED__
#define __IVxNormalizedTextChangeCollection_FWD_DEFINED__
typedef interface IVxNormalizedTextChangeCollection IVxNormalizedTextChangeCollection;
#endif 	/* __IVxNormalizedTextChangeCollection_FWD_DEFINED__ */


#ifndef __IVxReadOnlyRegion_FWD_DEFINED__
#define __IVxReadOnlyRegion_FWD_DEFINED__
typedef interface IVxReadOnlyRegion IVxReadOnlyRegion;
#endif 	/* __IVxReadOnlyRegion_FWD_DEFINED__ */


#ifndef __IVxTextBufferEdit_FWD_DEFINED__
#define __IVxTextBufferEdit_FWD_DEFINED__
typedef interface IVxTextBufferEdit IVxTextBufferEdit;
#endif 	/* __IVxTextBufferEdit_FWD_DEFINED__ */


#ifndef __IVxReadOnlyRegionEdit_FWD_DEFINED__
#define __IVxReadOnlyRegionEdit_FWD_DEFINED__
typedef interface IVxReadOnlyRegionEdit IVxReadOnlyRegionEdit;
#endif 	/* __IVxReadOnlyRegionEdit_FWD_DEFINED__ */


#ifndef __IVxTextBuffer_FWD_DEFINED__
#define __IVxTextBuffer_FWD_DEFINED__
typedef interface IVxTextBuffer IVxTextBuffer;
#endif 	/* __IVxTextBuffer_FWD_DEFINED__ */


#ifndef __IVxTextBufferFactoryService_FWD_DEFINED__
#define __IVxTextBufferFactoryService_FWD_DEFINED__
typedef interface IVxTextBufferFactoryService IVxTextBufferFactoryService;
#endif 	/* __IVxTextBufferFactoryService_FWD_DEFINED__ */


#ifndef __IVxTextChange_FWD_DEFINED__
#define __IVxTextChange_FWD_DEFINED__
typedef interface IVxTextChange IVxTextChange;
#endif 	/* __IVxTextChange_FWD_DEFINED__ */


#ifndef __IVxTextEdit_FWD_DEFINED__
#define __IVxTextEdit_FWD_DEFINED__
typedef interface IVxTextEdit IVxTextEdit;
#endif 	/* __IVxTextEdit_FWD_DEFINED__ */


#ifndef __IVxTextSnapshot_FWD_DEFINED__
#define __IVxTextSnapshot_FWD_DEFINED__
typedef interface IVxTextSnapshot IVxTextSnapshot;
#endif 	/* __IVxTextSnapshot_FWD_DEFINED__ */


#ifndef __IVxTextSnapshotLine_FWD_DEFINED__
#define __IVxTextSnapshotLine_FWD_DEFINED__
typedef interface IVxTextSnapshotLine IVxTextSnapshotLine;
#endif 	/* __IVxTextSnapshotLine_FWD_DEFINED__ */


#ifndef __IVxTextVersion_FWD_DEFINED__
#define __IVxTextVersion_FWD_DEFINED__
typedef interface IVxTextVersion IVxTextVersion;
#endif 	/* __IVxTextVersion_FWD_DEFINED__ */


#ifndef __IVxTrackingPoint_FWD_DEFINED__
#define __IVxTrackingPoint_FWD_DEFINED__
typedef interface IVxTrackingPoint IVxTrackingPoint;
#endif 	/* __IVxTrackingPoint_FWD_DEFINED__ */


#ifndef __IVxTrackingSpan_FWD_DEFINED__
#define __IVxTrackingSpan_FWD_DEFINED__
typedef interface IVxTrackingSpan IVxTrackingSpan;
#endif 	/* __IVxTrackingSpan_FWD_DEFINED__ */


#ifndef __IVxNormalizedSnapshotSpanCollection_FWD_DEFINED__
#define __IVxNormalizedSnapshotSpanCollection_FWD_DEFINED__
typedef interface IVxNormalizedSnapshotSpanCollection IVxNormalizedSnapshotSpanCollection;
#endif 	/* __IVxNormalizedSnapshotSpanCollection_FWD_DEFINED__ */


#ifndef __IVxNormalizedSpanCollection_FWD_DEFINED__
#define __IVxNormalizedSpanCollection_FWD_DEFINED__
typedef interface IVxNormalizedSpanCollection IVxNormalizedSpanCollection;
#endif 	/* __IVxNormalizedSpanCollection_FWD_DEFINED__ */


#ifndef __IVxTextContentChangedEventArgs_FWD_DEFINED__
#define __IVxTextContentChangedEventArgs_FWD_DEFINED__
typedef interface IVxTextContentChangedEventArgs IVxTextContentChangedEventArgs;
#endif 	/* __IVxTextContentChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxGraphBufferContentTypeChangedEventArgs_FWD_DEFINED__
#define __IVxGraphBufferContentTypeChangedEventArgs_FWD_DEFINED__
typedef interface IVxGraphBufferContentTypeChangedEventArgs IVxGraphBufferContentTypeChangedEventArgs;
#endif 	/* __IVxGraphBufferContentTypeChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxGraphBuffersChangedEventArgs_FWD_DEFINED__
#define __IVxGraphBuffersChangedEventArgs_FWD_DEFINED__
typedef interface IVxGraphBuffersChangedEventArgs IVxGraphBuffersChangedEventArgs;
#endif 	/* __IVxGraphBuffersChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxBufferGraph_FWD_DEFINED__
#define __IVxBufferGraph_FWD_DEFINED__
typedef interface IVxBufferGraph IVxBufferGraph;
#endif 	/* __IVxBufferGraph_FWD_DEFINED__ */


#ifndef __IVxProjectionBufferBase_FWD_DEFINED__
#define __IVxProjectionBufferBase_FWD_DEFINED__
typedef interface IVxProjectionBufferBase IVxProjectionBufferBase;
#endif 	/* __IVxProjectionBufferBase_FWD_DEFINED__ */


#ifndef __IVxProjectionSnapshot_FWD_DEFINED__
#define __IVxProjectionSnapshot_FWD_DEFINED__
typedef interface IVxProjectionSnapshot IVxProjectionSnapshot;
#endif 	/* __IVxProjectionSnapshot_FWD_DEFINED__ */


#ifndef __IVxProjectionBuffer_FWD_DEFINED__
#define __IVxProjectionBuffer_FWD_DEFINED__
typedef interface IVxProjectionBuffer IVxProjectionBuffer;
#endif 	/* __IVxProjectionBuffer_FWD_DEFINED__ */


#ifndef __IVxProjectionSourceSpansChangedEventArgs_FWD_DEFINED__
#define __IVxProjectionSourceSpansChangedEventArgs_FWD_DEFINED__
typedef interface IVxProjectionSourceSpansChangedEventArgs IVxProjectionSourceSpansChangedEventArgs;
#endif 	/* __IVxProjectionSourceSpansChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxProjectionSourceBuffersChangedEventArgs_FWD_DEFINED__
#define __IVxProjectionSourceBuffersChangedEventArgs_FWD_DEFINED__
typedef interface IVxProjectionSourceBuffersChangedEventArgs IVxProjectionSourceBuffersChangedEventArgs;
#endif 	/* __IVxProjectionSourceBuffersChangedEventArgs_FWD_DEFINED__ */


#ifndef __IVxSnapshotSpanEventArgs_FWD_DEFINED__
#define __IVxSnapshotSpanEventArgs_FWD_DEFINED__
typedef interface IVxSnapshotSpanEventArgs IVxSnapshotSpanEventArgs;
#endif 	/* __IVxSnapshotSpanEventArgs_FWD_DEFINED__ */


#ifndef __IVxTextBufferCreatedEventArgs_FWD_DEFINED__
#define __IVxTextBufferCreatedEventArgs_FWD_DEFINED__
typedef interface IVxTextBufferCreatedEventArgs IVxTextBufferCreatedEventArgs;
#endif 	/* __IVxTextBufferCreatedEventArgs_FWD_DEFINED__ */


#ifndef __IVxTextContentChangingEventArgs_FWD_DEFINED__
#define __IVxTextContentChangingEventArgs_FWD_DEFINED__
typedef interface IVxTextContentChangingEventArgs IVxTextContentChangingEventArgs;
#endif 	/* __IVxTextContentChangingEventArgs_FWD_DEFINED__ */


#ifndef __IVxTextSearchService_FWD_DEFINED__
#define __IVxTextSearchService_FWD_DEFINED__
typedef interface IVxTextSearchService IVxTextSearchService;
#endif 	/* __IVxTextSearchService_FWD_DEFINED__ */


#ifndef __IVxTextStructureNavigator_FWD_DEFINED__
#define __IVxTextStructureNavigator_FWD_DEFINED__
typedef interface IVxTextStructureNavigator IVxTextStructureNavigator;
#endif 	/* __IVxTextStructureNavigator_FWD_DEFINED__ */


#ifndef __IVxPlatformFactory_FWD_DEFINED__
#define __IVxPlatformFactory_FWD_DEFINED__
typedef interface IVxPlatformFactory IVxPlatformFactory;
#endif 	/* __IVxPlatformFactory_FWD_DEFINED__ */


#ifndef __IVxThumbnailSupport_FWD_DEFINED__
#define __IVxThumbnailSupport_FWD_DEFINED__
typedef interface IVxThumbnailSupport IVxThumbnailSupport;
#endif 	/* __IVxThumbnailSupport_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __VSEditorLibrary_LIBRARY_DEFINED__
#define __VSEditorLibrary_LIBRARY_DEFINED__

/* library VSEditorLibrary */
/* [uuid] */ 















































































typedef 
enum _VxEnumerableCardinality
    {	VxEnumerableCardinalityZero	= 0,
	VxEnumerableCardinalityOne	= 1,
	VxEnumerableCardinalityTwoOrMore	= 2
    } 	VxEnumerableCardinality;

typedef 
enum _VxCompositionTraceId
    {	VxCompositionTraceIdRejection_DefinitionRejected	= 1,
	VxCompositionTraceIdRejection_DefinitionResurrected	= 2,
	VxCompositionTraceIdDiscovery_AssemblyLoadFailed	= 3,
	VxCompositionTraceIdDiscovery_DefinitionMarkedWithPartNotDiscoverableAttribute	= 4,
	VxCompositionTraceIdDiscovery_DefinitionContainsGenericParameters	= 5,
	VxCompositionTraceIdDiscovery_DefinitionContainsNoExports	= 6,
	VxCompositionTraceIdDiscovery_MemberMarkedWithMultipleImportAndImportMany	= 7
    } 	VxCompositionTraceId;

typedef 
enum _VxAtomicCompositionQueryState
    {	VxAtomicCompositionQueryStateUnknown	= 0,
	VxAtomicCompositionQueryStateTreatAsRejected	= 1,
	VxAtomicCompositionQueryStateTreatAsValidated	= 2,
	VxAtomicCompositionQueryStateNeedsTesting	= 3
    } 	VxAtomicCompositionQueryState;

typedef 
enum _VxExportCardinalityCheckResult
    {	VxExportCardinalityCheckResultMatch	= 0,
	VxExportCardinalityCheckResultNoExports	= 1,
	VxExportCardinalityCheckResultTooManyExports	= 2
    } 	VxExportCardinalityCheckResult;

typedef 
enum _VxImportState
    {	VxImportStateNoImportsSatisfied	= 0,
	VxImportStateImportsPreviewing	= 1,
	VxImportStateImportsPreviewed	= 2,
	VxImportStatePreExportImportsSatisfying	= 3,
	VxImportStatePreExportImportsSatisfied	= 4,
	VxImportStatePostExportImportsSatisfying	= 5,
	VxImportStatePostExportImportsSatisfied	= 6,
	VxImportStateComposedNotifying	= 7,
	VxImportStateComposed	= 8
    } 	VxImportState;

typedef 
enum _VxImportCardinality
    {	VxImportCardinalityZeroOrOne	= 0,
	VxImportCardinalityExactlyOne	= 1,
	VxImportCardinalityZeroOrMore	= 2
    } 	VxImportCardinality;

typedef 
enum _VxReflectionItemType
    {	VxReflectionItemTypeParameter	= 0,
	VxReflectionItemTypeField	= 1,
	VxReflectionItemTypeProperty	= 2,
	VxReflectionItemTypeMethod	= 3,
	VxReflectionItemTypeType	= 4
    } 	VxReflectionItemType;

typedef 
enum _VxCompositionErrorId
    {	VxCompositionErrorIdUnknown	= 0,
	VxCompositionErrorIdInvalidExportMetadata	= 1,
	VxCompositionErrorIdRequiredMetadataNotFound	= 2,
	VxCompositionErrorIdUnsupportedExportType	= 3,
	VxCompositionErrorIdImportNotSetOnPart	= 4,
	VxCompositionErrorIdImportEngine_ComposeTookTooManyIterations	= 5,
	VxCompositionErrorIdImportEngine_ImportCardinalityMismatch	= 6,
	VxCompositionErrorIdImportEngine_PartCycle	= 7,
	VxCompositionErrorIdImportEngine_PartCannotSetImport	= 8,
	VxCompositionErrorIdImportEngine_PartCannotGetExportedValue	= 9,
	VxCompositionErrorIdImportEngine_PartCannotActivate	= 10,
	VxCompositionErrorIdImportEngine_PreventedByExistingImport	= 11,
	VxCompositionErrorIdImportEngine_InvalidStateForRecomposition	= 12,
	VxCompositionErrorIdReflectionModel_PartConstructorMissing	= 13,
	VxCompositionErrorIdReflectionModel_PartConstructorThrewException	= 14,
	VxCompositionErrorIdReflectionModel_PartOnImportsSatisfiedThrewException	= 15,
	VxCompositionErrorIdReflectionModel_ExportNotReadable	= 16,
	VxCompositionErrorIdReflectionModel_ExportThrewException	= 17,
	VxCompositionErrorIdReflectionModel_ExportMethodTooManyParameters	= 18,
	VxCompositionErrorIdReflectionModel_ImportNotWritable	= 19,
	VxCompositionErrorIdReflectionModel_ImportThrewException	= 20,
	VxCompositionErrorIdReflectionModel_ImportNotAssignableFromExport	= 21,
	VxCompositionErrorIdReflectionModel_ImportCollectionNull	= 22,
	VxCompositionErrorIdReflectionModel_ImportCollectionNotWritable	= 23,
	VxCompositionErrorIdReflectionModel_ImportCollectionConstructionThrewException	= 24,
	VxCompositionErrorIdReflectionModel_ImportCollectionGetThrewException	= 25,
	VxCompositionErrorIdReflectionModel_ImportCollectionIsReadOnlyThrewException	= 26,
	VxCompositionErrorIdReflectionModel_ImportCollectionClearThrewException	= 27,
	VxCompositionErrorIdReflectionModel_ImportCollectionAddThrewException	= 28,
	VxCompositionErrorIdReflectionModel_ImportManyOnParameterCanOnlyBeAssigned	= 29
    } 	VxCompositionErrorId;

typedef 
enum _VxCreationPolicy
    {	VxCreationPolicyAny	= 0,
	VxCreationPolicyShared	= 1,
	VxCreationPolicyNonShared	= 2
    } 	VxCreationPolicy;

typedef 
enum _VxDifferenceType
    {	VxDifferenceTypeAdd	= 0,
	VxDifferenceTypeRemove	= 1,
	VxDifferenceTypeChange	= 2
    } 	VxDifferenceType;

typedef 
enum _VxStringDifferenceTypes
    {	VxStringDifferenceTypesLine	= 1,
	VxStringDifferenceTypesWord	= 2,
	VxStringDifferenceTypesCharacter	= 4
    } 	VxStringDifferenceTypes;

typedef 
enum _VxWordSplitBehavior
    {	VxWordSplitBehaviorDefault	= 0,
	VxWordSplitBehaviorCharacterClass	= 0,
	VxWordSplitBehaviorWhiteSpace	= 1,
	VxWordSplitBehaviorWhiteSpaceAndPunctuation	= 2
    } 	VxWordSplitBehavior;

typedef 
enum _VxReloadResult
    {	VxReloadResultAborted	= 0,
	VxReloadResultSucceeded	= 1,
	VxReloadResultSucceededWithCharacterSubstitutions	= 2
    } 	VxReloadResult;

typedef 
enum _VxFileActionTypes
    {	VxFileActionTypesContentSavedToDisk	= 1,
	VxFileActionTypesContentLoadedFromDisk	= 2,
	VxFileActionTypesDocumentRenamed	= 4
    } 	VxFileActionTypes;

typedef 
enum _VxEdgeInsertionMode
    {	VxEdgeInsertionModeAllow	= 0,
	VxEdgeInsertionModeDeny	= 1
    } 	VxEdgeInsertionMode;

typedef 
enum _VxPointTrackingMode
    {	VxPointTrackingModePositive	= 0,
	VxPointTrackingModeNegative	= 1
    } 	VxPointTrackingMode;

typedef 
enum _VxPositionAffinity
    {	VxPositionAffinityPredecessor	= 0,
	VxPositionAffinitySuccessor	= 1
    } 	VxPositionAffinity;

typedef 
enum _VxElisionBufferOptions
    {	VxElisionBufferOptionsNone	= 0,
	VxElisionBufferOptionsFillInMappingMode	= 1
    } 	VxElisionBufferOptions;

typedef 
enum _VxProjectionBufferOptions
    {	VxProjectionBufferOptionsNone	= 0,
	VxProjectionBufferOptionsPermissiveEdgeInclusiveSourceSpans	= 1,
	VxProjectionBufferOptionsWritableLiteralSpans	= 2
    } 	VxProjectionBufferOptions;

typedef 
enum _VxSpanTrackingMode
    {	VxSpanTrackingModeEdgeExclusive	= 0,
	VxSpanTrackingModeEdgeInclusive	= 1,
	VxSpanTrackingModeEdgePositive	= 2,
	VxSpanTrackingModeEdgeNegative	= 3,
	VxSpanTrackingModeCustom	= 4
    } 	VxSpanTrackingMode;

typedef 
enum _VxTrackingFidelityMode
    {	VxTrackingFidelityModeForward	= 0,
	VxTrackingFidelityModeBackward	= 1,
	VxTrackingFidelityModeUndoRedo	= 2
    } 	VxTrackingFidelityMode;

typedef 
enum _VxTagAggregatorOptions
    {	VxTagAggregatorOptionsNone	= 0,
	VxTagAggregatorOptionsMapByContentType	= 1
    } 	VxTagAggregatorOptions;

typedef 
enum _VxChangeTypes
    {	VxChangeTypesNone	= 0,
	VxChangeTypesChangedSinceOpened	= 1,
	VxChangeTypesChangedSinceSaved	= 2
    } 	VxChangeTypes;

typedef 
enum _VxFindOptions
    {	VxFindOptionsNone	= 0,
	VxFindOptionsMatchCase	= 1,
	VxFindOptionsUseRegularExpressions	= 2,
	VxFindOptionsWholeWord	= 4,
	VxFindOptionsSearchReverse	= 8
    } 	VxFindOptions;

typedef 
enum _VxTextUndoHistoryState
    {	VxTextUndoHistoryStateIdle	= 0,
	VxTextUndoHistoryStateUndoing	= 1,
	VxTextUndoHistoryStateRedoing	= 2
    } 	VxTextUndoHistoryState;

typedef 
enum _VxTextUndoTransactionCompletionResult
    {	VxTextUndoTransactionCompletionResultTransactionAdded	= 0,
	VxTextUndoTransactionCompletionResultTransactionMerged	= 1
    } 	VxTextUndoTransactionCompletionResult;

typedef 
enum _VxUndoTransactionState
    {	VxUndoTransactionStateOpen	= 0,
	VxUndoTransactionStateCompleted	= 1,
	VxUndoTransactionStateCanceled	= 2,
	VxUndoTransactionStateRedoing	= 3,
	VxUndoTransactionStateUndoing	= 4,
	VxUndoTransactionStateUndone	= 5,
	VxUndoTransactionStateInvalid	= 6
    } 	VxUndoTransactionState;

typedef 
enum _VxPopupStyles
    {	VxPopupStylesNone	= 0,
	VxPopupStylesDismissOnMouseLeaveText	= 4,
	VxPopupStylesDismissOnMouseLeaveTextOrContent	= 8,
	VxPopupStylesPositionLeftOrRight	= 16,
	VxPopupStylesPreferLeftOrTopPosition	= 32,
	VxPopupStylesRightOrBottomJustify	= 64,
	VxPopupStylesPositionClosest	= 128
    } 	VxPopupStyles;

typedef 
enum _VxEnsureSpanVisibleOptions
    {	VxEnsureSpanVisibleOptionsShowStart	= 1,
	VxEnsureSpanVisibleOptionsMinimumScroll	= 2,
	VxEnsureSpanVisibleOptionsAlwaysCenter	= 4,
	VxEnsureSpanVisibleOptionsNone	= 0
    } 	VxEnsureSpanVisibleOptions;

typedef 
enum _VxScrollDirection
    {	VxScrollDirectionUp	= 0,
	VxScrollDirectionDown	= 1
    } 	VxScrollDirection;

typedef 
enum _VxTextSelectionMode
    {	VxTextSelectionModeStream	= 0,
	VxTextSelectionModeBox	= 1
    } 	VxTextSelectionMode;

typedef 
enum _VxViewRelativePosition
    {	VxViewRelativePositionTop	= 0,
	VxViewRelativePositionBottom	= 1
    } 	VxViewRelativePosition;

typedef 
enum _VxWordWrapStyles
    {	VxWordWrapStylesNone	= 0,
	VxWordWrapStylesWordWrap	= 1,
	VxWordWrapStylesVisibleGlyphs	= 2,
	VxWordWrapStylesAutoIndent	= 4
    } 	VxWordWrapStyles;

typedef 
enum _VxTextViewLineChange
    {	VxTextViewLineChangeNone	= 0,
	VxTextViewLineChangeNewOrReformatted	= 1,
	VxTextViewLineChangeTranslated	= 2
    } 	VxTextViewLineChange;

typedef 
enum _VxVisibilityState
    {	VxVisibilityStateUnattached	= 0,
	VxVisibilityStateHidden	= 1,
	VxVisibilityStatePartiallyVisible	= 2,
	VxVisibilityStateFullyVisible	= 3
    } 	VxVisibilityState;

typedef 
enum _VxIncrementalSearchDirection
    {	VxIncrementalSearchDirectionForward	= 0,
	VxIncrementalSearchDirectionBackward	= 1
    } 	VxIncrementalSearchDirection;

typedef 
enum _VxAdornmentPositioningBehavior
    {	VxAdornmentPositioningBehaviorOwnerControlled	= 0,
	VxAdornmentPositioningBehaviorViewportRelative	= 1,
	VxAdornmentPositioningBehaviorTextRelative	= 2
    } 	VxAdornmentPositioningBehavior;

typedef 
enum _VxDragDropPointerEffects
    {	VxDragDropPointerEffectsNone	= 0,
	VxDragDropPointerEffectsCopy	= 1,
	VxDragDropPointerEffectsLink	= 2,
	VxDragDropPointerEffectsMove	= 4,
	VxDragDropPointerEffectsScroll	= 8,
	VxDragDropPointerEffectsTrack	= 16,
	VxDragDropPointerEffectsAll	= 31
    } 	VxDragDropPointerEffects;

typedef 
enum _VxConnectionReason
    {	VxConnectionReasonTextViewLifetime	= 0,
	VxConnectionReasonContentTypeChange	= 1,
	VxConnectionReasonBufferGraphChange	= 2
    } 	VxConnectionReason;

typedef 
enum _VxCompletionMatchType
    {	VxCompletionMatchTypeMatchDisplayText	= 0,
	VxCompletionMatchTypeMatchInsertionText	= 1
    } 	VxCompletionMatchType;

typedef 
enum _VxIntellisenseKeyboardCommand
    {	VxIntellisenseKeyboardCommandUp	= 0,
	VxIntellisenseKeyboardCommandDown	= 1,
	VxIntellisenseKeyboardCommandPageUp	= 2,
	VxIntellisenseKeyboardCommandPageDown	= 3,
	VxIntellisenseKeyboardCommandTopLine	= 4,
	VxIntellisenseKeyboardCommandBottomLine	= 5,
	VxIntellisenseKeyboardCommandHome	= 6,
	VxIntellisenseKeyboardCommandEnd	= 7,
	VxIntellisenseKeyboardCommandEnter	= 8,
	VxIntellisenseKeyboardCommandEscape	= 9,
	VxIntellisenseKeyboardCommandIncreaseFilterLevel	= 10,
	VxIntellisenseKeyboardCommandDecreaseFilterLevel	= 11
    } 	VxIntellisenseKeyboardCommand;

typedef 
enum _VxSmartTagState
    {	VxSmartTagStateCollapsed	= 0,
	VxSmartTagStateIntermediate	= 1,
	VxSmartTagStateExpanded	= 2
    } 	VxSmartTagState;

typedef 
enum _VxSmartTagType
    {	VxSmartTagTypeFactoid	= 0,
	VxSmartTagTypeEphemeral	= 1
    } 	VxSmartTagType;

typedef 
enum _VxStandardGlyphGroup
    {	VxStandardGlyphGroupGlyphGroupClass	= 0,
	VxStandardGlyphGroupGlyphGroupConstant	= 6,
	VxStandardGlyphGroupGlyphGroupDelegate	= 12,
	VxStandardGlyphGroupGlyphGroupEnum	= 18,
	VxStandardGlyphGroupGlyphGroupEnumMember	= 24,
	VxStandardGlyphGroupGlyphGroupEvent	= 30,
	VxStandardGlyphGroupGlyphGroupException	= 36,
	VxStandardGlyphGroupGlyphGroupField	= 42,
	VxStandardGlyphGroupGlyphGroupInterface	= 48,
	VxStandardGlyphGroupGlyphGroupMacro	= 54,
	VxStandardGlyphGroupGlyphGroupMap	= 60,
	VxStandardGlyphGroupGlyphGroupMapItem	= 66,
	VxStandardGlyphGroupGlyphGroupMethod	= 72,
	VxStandardGlyphGroupGlyphGroupOverload	= 78,
	VxStandardGlyphGroupGlyphGroupModule	= 84,
	VxStandardGlyphGroupGlyphGroupNamespace	= 90,
	VxStandardGlyphGroupGlyphGroupOperator	= 96,
	VxStandardGlyphGroupGlyphGroupProperty	= 102,
	VxStandardGlyphGroupGlyphGroupStruct	= 108,
	VxStandardGlyphGroupGlyphGroupTemplate	= 114,
	VxStandardGlyphGroupGlyphGroupTypedef	= 120,
	VxStandardGlyphGroupGlyphGroupType	= 126,
	VxStandardGlyphGroupGlyphGroupUnion	= 132,
	VxStandardGlyphGroupGlyphGroupVariable	= 138,
	VxStandardGlyphGroupGlyphGroupValueType	= 144,
	VxStandardGlyphGroupGlyphGroupIntrinsic	= 150,
	VxStandardGlyphGroupGlyphGroupJSharpMethod	= 156,
	VxStandardGlyphGroupGlyphGroupJSharpField	= 162,
	VxStandardGlyphGroupGlyphGroupJSharpClass	= 168,
	VxStandardGlyphGroupGlyphGroupJSharpNamespace	= 174,
	VxStandardGlyphGroupGlyphGroupJSharpInterface	= 180,
	VxStandardGlyphGroupGlyphGroupError	= 186,
	VxStandardGlyphGroupGlyphBscFile	= 191,
	VxStandardGlyphGroupGlyphAssembly	= 192,
	VxStandardGlyphGroupGlyphLibrary	= 193,
	VxStandardGlyphGroupGlyphVBProject	= 194,
	VxStandardGlyphGroupGlyphCoolProject	= 196,
	VxStandardGlyphGroupGlyphCppProject	= 199,
	VxStandardGlyphGroupGlyphDialogId	= 200,
	VxStandardGlyphGroupGlyphOpenFolder	= 201,
	VxStandardGlyphGroupGlyphClosedFolder	= 202,
	VxStandardGlyphGroupGlyphArrow	= 203,
	VxStandardGlyphGroupGlyphCSharpFile	= 204,
	VxStandardGlyphGroupGlyphCSharpExpansion	= 205,
	VxStandardGlyphGroupGlyphKeyword	= 206,
	VxStandardGlyphGroupGlyphInformation	= 207,
	VxStandardGlyphGroupGlyphReference	= 208,
	VxStandardGlyphGroupGlyphRecursion	= 209,
	VxStandardGlyphGroupGlyphXmlItem	= 210,
	VxStandardGlyphGroupGlyphJSharpProject	= 211,
	VxStandardGlyphGroupGlyphJSharpDocument	= 212,
	VxStandardGlyphGroupGlyphForwardType	= 213,
	VxStandardGlyphGroupGlyphCallersGraph	= 214,
	VxStandardGlyphGroupGlyphCallGraph	= 215,
	VxStandardGlyphGroupGlyphWarning	= 216,
	VxStandardGlyphGroupGlyphMaybeReference	= 217,
	VxStandardGlyphGroupGlyphMaybeCaller	= 218,
	VxStandardGlyphGroupGlyphMaybeCall	= 219,
	VxStandardGlyphGroupGlyphExtensionMethod	= 220,
	VxStandardGlyphGroupGlyphExtensionMethodInternal	= 221,
	VxStandardGlyphGroupGlyphExtensionMethodFriend	= 222,
	VxStandardGlyphGroupGlyphExtensionMethodProtected	= 223,
	VxStandardGlyphGroupGlyphExtensionMethodPrivate	= 224,
	VxStandardGlyphGroupGlyphExtensionMethodShortcut	= 225,
	VxStandardGlyphGroupGlyphXmlAttribute	= 226,
	VxStandardGlyphGroupGlyphXmlChild	= 227,
	VxStandardGlyphGroupGlyphXmlDescendant	= 228,
	VxStandardGlyphGroupGlyphXmlNamespace	= 229,
	VxStandardGlyphGroupGlyphXmlAttributeQuestion	= 230,
	VxStandardGlyphGroupGlyphXmlAttributeCheck	= 231,
	VxStandardGlyphGroupGlyphXmlChildQuestion	= 232,
	VxStandardGlyphGroupGlyphXmlChildCheck	= 233,
	VxStandardGlyphGroupGlyphXmlDescendantQuestion	= 234,
	VxStandardGlyphGroupGlyphXmlDescendantCheck	= 235,
	VxStandardGlyphGroupGlyphGroupUnknown	= 236
    } 	VxStandardGlyphGroup;

typedef 
enum _VxStandardGlyphItem
    {	VxStandardGlyphItemGlyphItemPublic	= 0,
	VxStandardGlyphItemGlyphItemInternal	= 1,
	VxStandardGlyphItemGlyphItemFriend	= 2,
	VxStandardGlyphItemGlyphItemProtected	= 3,
	VxStandardGlyphItemGlyphItemPrivate	= 4,
	VxStandardGlyphItemGlyphItemShortcut	= 5,
	VxStandardGlyphItemTotalGlyphItems	= 6
    } 	VxStandardGlyphItem;

typedef 
enum _VxUIElementType
    {	VxUIElementTypeSmall	= 0,
	VxUIElementTypeLarge	= 1,
	VxUIElementTypeTooltip	= 2
    } 	VxUIElementType;

typedef 
enum _VxVisitState
    {	VxVisitStateNotVisited	= 0,
	VxVisitStateVisiting	= 1,
	VxVisitStateVisited	= 2
    } 	VxVisitState;

typedef 
enum _VxQuickInfoState
    {	VxQuickInfoStateInitialized	= 1,
	VxQuickInfoStateCalculating	= 2,
	VxQuickInfoStateStarted	= 4,
	VxQuickInfoStateDismissed	= 8
    } 	VxQuickInfoState;

typedef 
enum _VxDefaultSmartTagPresenterStates
    {	VxDefaultSmartTagPresenterStatesInvisible	= 0,
	VxDefaultSmartTagPresenterStatesTickler	= 1,
	VxDefaultSmartTagPresenterStatesBareButton	= 2,
	VxDefaultSmartTagPresenterStatesGlowingButton	= 3,
	VxDefaultSmartTagPresenterStatesListMenu	= 4,
	VxDefaultSmartTagPresenterStatesOwnerDrawingTag	= 5
    } 	VxDefaultSmartTagPresenterStates;

typedef 
enum _VxInternalState
    {	VxInternalStateInitialized	= 1,
	VxInternalStateCalculating	= 2,
	VxInternalStateStartedAndCalculating	= 4,
	VxInternalStateStarted	= 8,
	VxInternalStateDismissed	= 16
    } 	VxInternalState;

typedef 
enum _VxDragDropState
    {	VxDragDropStateStart	= 0,
	VxDragDropStateMouseDown	= 1,
	VxDragDropStateDragging	= 2,
	VxDragDropStateCanceled	= 3,
	VxDragDropStateDropped	= 4
    } 	VxDragDropState;

typedef 
enum _VxCaretMovementDirection
    {	VxCaretMovementDirectionPrevious	= 0,
	VxCaretMovementDirectionNext	= 1
    } 	VxCaretMovementDirection;

typedef 
enum _VxLetterCase
    {	VxLetterCaseUppercase	= 0,
	VxLetterCaseLowercase	= 1
    } 	VxLetterCase;

typedef 
enum _VxSelectionUpdate
    {	VxSelectionUpdatePreserve	= 0,
	VxSelectionUpdateReset	= 1,
	VxSelectionUpdateResetUnlessEmptyBox	= 2,
	VxSelectionUpdateIgnore	= 3,
	VxSelectionUpdateClearVirtualSpace	= 4
    } 	VxSelectionUpdate;

typedef 
enum _VxTextEditAction
    {	VxTextEditActionNone	= 0,
	VxTextEditActionType	= 1,
	VxTextEditActionDelete	= 2,
	VxTextEditActionBackspace	= 3,
	VxTextEditActionPaste	= 4,
	VxTextEditActionEnter	= 5,
	VxTextEditActionAutoIndent	= 6,
	VxTextEditActionReplace	= 7,
	VxTextEditActionProvisionalOverwrite	= 8
    } 	VxTextEditAction;

typedef 
enum _VxTextTransactionMergeDirections
    {	VxTextTransactionMergeDirectionsForward	= 1,
	VxTextTransactionMergeDirectionsBackward	= 2
    } 	VxTextTransactionMergeDirections;

typedef 
enum _VxCharacterType
    {	VxCharacterTypeAlphaNumeric	= 0,
	VxCharacterTypeWhiteSpace	= 1,
	VxCharacterTypeSymbols	= 2
    } 	VxCharacterType;

typedef 
enum _VxSpanType
    {	VxSpanTypeEmpty	= 0,
	VxSpanTypeMultipleCharacters	= 1,
	VxSpanTypeWord	= 2,
	VxSpanTypeMultipleWords	= 3,
	VxSpanTypeSentence	= 4,
	VxSpanTypeMultipleSentences	= 5,
	VxSpanTypeParagraph	= 6,
	VxSpanTypeMultipleParagraphs	= 7,
	VxSpanTypeDocument	= 8
    } 	VxSpanType;

typedef 
enum _VxLineCalculationState
    {	VxLineCalculationStatePrimary	= 0,
	VxLineCalculationStateAppend	= 1,
	VxLineCalculationStatePrepend	= 2,
	VxLineCalculationStateBipend	= 3
    } 	VxLineCalculationState;

typedef 
enum _VxLineBreakBoundaryConditions
    {	VxLineBreakBoundaryConditionsNone	= 0,
	VxLineBreakBoundaryConditionsPrecedingReturn	= 1,
	VxLineBreakBoundaryConditionsSucceedingNewline	= 2
    } 	VxLineBreakBoundaryConditions;

typedef 
enum _VxValidProtocolFound
    {	VxValidProtocolFoundValidProtocol	= 0,
	VxValidProtocolFoundValidProtocolNoSlash	= 1,
	VxValidProtocolFoundInvalidProtocol	= 2
    } 	VxValidProtocolFound;

typedef struct _VxStringDifferenceOptions
    {
    VxStringDifferenceTypes differenceType;
    int locality;
    BOOL ignoreTrimWhiteSpace;
    } 	VxStringDifferenceOptions;

typedef struct _VxEditOptions
    {
    BOOL computeMinimalChange;
    VxStringDifferenceOptions differenceOptions;
    } 	VxEditOptions;

typedef struct _VxSnapshotPoint
    {
    IVxTextSnapshot *snapshot;
    int position;
    } 	VxSnapshotPoint;

typedef struct _VxSnapshotSpan
    {
    VxSnapshotPoint start;
    int length;
    } 	VxSnapshotSpan;

typedef struct _VxSpan
    {
    int start;
    int length;
    } 	VxSpan;

typedef struct _VxFindData
    {
    BSTR _searchString;
    IVxTextSnapshot *_textSnapshotToSearch;
    VxFindOptions _findOptions;
    IVxTextStructureNavigator *_textStructureNavigator;
    } 	VxFindData;

typedef struct _VxTextExtent
    {
    VxSnapshotSpan _span;
    BOOL _isSignificant;
    } 	VxTextExtent;


EXTERN_C const IID LIBID_VSEditorLibrary;

#ifndef __IEnumeratorBSTR_INTERFACE_DEFINED__
#define __IEnumeratorBSTR_INTERFACE_DEFINED__

/* interface IEnumeratorBSTR */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorBSTR;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("18a2071c-67fb-4543-bd62-d11e68a6785b")
    IEnumeratorBSTR : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorBSTRVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorBSTR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorBSTR * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorBSTR * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorBSTR * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorBSTR * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorBSTR * This);
        
        END_INTERFACE
    } IEnumeratorBSTRVtbl;

    interface IEnumeratorBSTR
    {
        CONST_VTBL struct IEnumeratorBSTRVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorBSTR_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorBSTR_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorBSTR_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorBSTR_GetCurrent(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,pRetVal) ) 

#define IEnumeratorBSTR_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorBSTR_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorBSTR_INTERFACE_DEFINED__ */


#ifndef __IEnumerableBSTR_INTERFACE_DEFINED__
#define __IEnumerableBSTR_INTERFACE_DEFINED__

/* interface IEnumerableBSTR */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumerableBSTR;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b887db4f-490c-415e-9ff4-f4657b757772")
    IEnumerableBSTR : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorBSTR **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumerableBSTRVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumerableBSTR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumerableBSTR * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumerableBSTR * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IEnumerableBSTR * This,
            /* [retval][out] */ IEnumeratorBSTR **ppRetVal);
        
        END_INTERFACE
    } IEnumerableBSTRVtbl;

    interface IEnumerableBSTR
    {
        CONST_VTBL struct IEnumerableBSTRVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumerableBSTR_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumerableBSTR_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumerableBSTR_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumerableBSTR_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumerableBSTR_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxContentType_INTERFACE_DEFINED__
#define __IEnumeratorIVxContentType_INTERFACE_DEFINED__

/* interface IEnumeratorIVxContentType */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxContentType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d92cce42-6e23-4ce3-a633-8b76ac0826f2")
    IEnumeratorIVxContentType : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxContentTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxContentType * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxContentType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxContentType * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxContentType * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxContentType * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxContentType * This);
        
        END_INTERFACE
    } IEnumeratorIVxContentTypeVtbl;

    interface IEnumeratorIVxContentType
    {
        CONST_VTBL struct IEnumeratorIVxContentTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxContentType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxContentType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxContentType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxContentType_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxContentType_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxContentType_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxContentType_INTERFACE_DEFINED__ */


#ifndef __IEnumerableIVxContentType_INTERFACE_DEFINED__
#define __IEnumerableIVxContentType_INTERFACE_DEFINED__

/* interface IEnumerableIVxContentType */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumerableIVxContentType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("acc50312-81c5-4da1-92b0-3f37acfa1106")
    IEnumerableIVxContentType : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumerableIVxContentTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumerableIVxContentType * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumerableIVxContentType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumerableIVxContentType * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IEnumerableIVxContentType * This,
            /* [retval][out] */ IEnumeratorIVxContentType **ppRetVal);
        
        END_INTERFACE
    } IEnumerableIVxContentTypeVtbl;

    interface IEnumerableIVxContentType
    {
        CONST_VTBL struct IEnumerableIVxContentTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumerableIVxContentType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumerableIVxContentType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumerableIVxContentType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumerableIVxContentType_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumerableIVxContentType_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocumentFileActionEvent_INTERFACE_DEFINED__
#define __IVxTextDocumentFileActionEvent_INTERFACE_DEFINED__

/* interface IVxTextDocumentFileActionEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocumentFileActionEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7598112a-a45e-4ec3-9b15-2536d26b1cab")
    IVxTextDocumentFileActionEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnTextDocumentFileAction( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextDocumentFileActionEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentFileActionEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocumentFileActionEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocumentFileActionEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocumentFileActionEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnTextDocumentFileAction )( 
            IVxTextDocumentFileActionEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextDocumentFileActionEventArgs *pE);
        
        END_INTERFACE
    } IVxTextDocumentFileActionEventVtbl;

    interface IVxTextDocumentFileActionEvent
    {
        CONST_VTBL struct IVxTextDocumentFileActionEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocumentFileActionEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocumentFileActionEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocumentFileActionEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocumentFileActionEvent_OnTextDocumentFileAction(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnTextDocumentFileAction(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocumentFileActionEvent_INTERFACE_DEFINED__ */


#ifndef __IVxEventArgsEvent_INTERFACE_DEFINED__
#define __IVxEventArgsEvent_INTERFACE_DEFINED__

/* interface IVxEventArgsEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxEventArgsEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("765b3465-12be-4cfe-8bd1-85b27f2cb081")
    IVxEventArgsEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnEventArgs( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IUnknown *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxEventArgsEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxEventArgsEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxEventArgsEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxEventArgsEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnEventArgs )( 
            IVxEventArgsEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IUnknown *pE);
        
        END_INTERFACE
    } IVxEventArgsEventVtbl;

    interface IVxEventArgsEvent
    {
        CONST_VTBL struct IVxEventArgsEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxEventArgsEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxEventArgsEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxEventArgsEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxEventArgsEvent_OnEventArgs(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnEventArgs(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxEventArgsEvent_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocumentEvent_INTERFACE_DEFINED__
#define __IVxTextDocumentEvent_INTERFACE_DEFINED__

/* interface IVxTextDocumentEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocumentEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("36811be1-5281-4eca-9a64-dacb86fabf66")
    IVxTextDocumentEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnTextDocument( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextDocumentEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocumentEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocumentEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocumentEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnTextDocument )( 
            IVxTextDocumentEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextDocumentEventArgs *pE);
        
        END_INTERFACE
    } IVxTextDocumentEventVtbl;

    interface IVxTextDocumentEvent
    {
        CONST_VTBL struct IVxTextDocumentEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocumentEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocumentEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocumentEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocumentEvent_OnTextDocument(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnTextDocument(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocumentEvent_INTERFACE_DEFINED__ */


#ifndef __IVxSnapshotSpanEvent_INTERFACE_DEFINED__
#define __IVxSnapshotSpanEvent_INTERFACE_DEFINED__

/* interface IVxSnapshotSpanEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxSnapshotSpanEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8b881e32-b5f3-47a1-b62d-1dd1f2ff7803")
    IVxSnapshotSpanEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnSnapshotSpan( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxSnapshotSpanEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxSnapshotSpanEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxSnapshotSpanEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxSnapshotSpanEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxSnapshotSpanEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnSnapshotSpan )( 
            IVxSnapshotSpanEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxSnapshotSpanEventArgs *pE);
        
        END_INTERFACE
    } IVxSnapshotSpanEventVtbl;

    interface IVxSnapshotSpanEvent
    {
        CONST_VTBL struct IVxSnapshotSpanEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxSnapshotSpanEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxSnapshotSpanEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxSnapshotSpanEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxSnapshotSpanEvent_OnSnapshotSpan(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnSnapshotSpan(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxSnapshotSpanEvent_INTERFACE_DEFINED__ */


#ifndef __IVxTextContentChangedEvent_INTERFACE_DEFINED__
#define __IVxTextContentChangedEvent_INTERFACE_DEFINED__

/* interface IVxTextContentChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextContentChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5be64365-ae6e-4d88-b265-b22c5e743450")
    IVxTextContentChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnTextContentChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextContentChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextContentChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextContentChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextContentChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextContentChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnTextContentChanged )( 
            IVxTextContentChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextContentChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxTextContentChangedEventVtbl;

    interface IVxTextContentChangedEvent
    {
        CONST_VTBL struct IVxTextContentChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextContentChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextContentChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextContentChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextContentChangedEvent_OnTextContentChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnTextContentChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextContentChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IVxTextContentChangingEvent_INTERFACE_DEFINED__
#define __IVxTextContentChangingEvent_INTERFACE_DEFINED__

/* interface IVxTextContentChangingEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextContentChangingEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5e12035a-0f79-408d-8d48-6896ff85bf7d")
    IVxTextContentChangingEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnTextContentChanging( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextContentChangingEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextContentChangingEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextContentChangingEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextContentChangingEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextContentChangingEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnTextContentChanging )( 
            IVxTextContentChangingEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextContentChangingEventArgs *pE);
        
        END_INTERFACE
    } IVxTextContentChangingEventVtbl;

    interface IVxTextContentChangingEvent
    {
        CONST_VTBL struct IVxTextContentChangingEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextContentChangingEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextContentChangingEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextContentChangingEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextContentChangingEvent_OnTextContentChanging(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnTextContentChanging(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextContentChangingEvent_INTERFACE_DEFINED__ */


#ifndef __IVxContentTypeChangedEvent_INTERFACE_DEFINED__
#define __IVxContentTypeChangedEvent_INTERFACE_DEFINED__

/* interface IVxContentTypeChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxContentTypeChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b58ad157-69a4-4225-aa41-0e5547b6cda3")
    IVxContentTypeChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnContentTypeChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxContentTypeChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxContentTypeChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxContentTypeChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxContentTypeChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxContentTypeChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnContentTypeChanged )( 
            IVxContentTypeChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxContentTypeChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxContentTypeChangedEventVtbl;

    interface IVxContentTypeChangedEvent
    {
        CONST_VTBL struct IVxContentTypeChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxContentTypeChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxContentTypeChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxContentTypeChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxContentTypeChangedEvent_OnContentTypeChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnContentTypeChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxContentTypeChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IVxTextBufferCreatedEvent_INTERFACE_DEFINED__
#define __IVxTextBufferCreatedEvent_INTERFACE_DEFINED__

/* interface IVxTextBufferCreatedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextBufferCreatedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7a45771d-7a36-4d18-9b81-c5daa88985b8")
    IVxTextBufferCreatedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnTextBufferCreated( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextBufferCreatedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextBufferCreatedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextBufferCreatedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextBufferCreatedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextBufferCreatedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnTextBufferCreated )( 
            IVxTextBufferCreatedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxTextBufferCreatedEventArgs *pE);
        
        END_INTERFACE
    } IVxTextBufferCreatedEventVtbl;

    interface IVxTextBufferCreatedEvent
    {
        CONST_VTBL struct IVxTextBufferCreatedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextBufferCreatedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextBufferCreatedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextBufferCreatedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextBufferCreatedEvent_OnTextBufferCreated(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnTextBufferCreated(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextBufferCreatedEvent_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxTextSnapshotLine_INTERFACE_DEFINED__
#define __IEnumeratorIVxTextSnapshotLine_INTERFACE_DEFINED__

/* interface IEnumeratorIVxTextSnapshotLine */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxTextSnapshotLine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5ba4ea24-9310-4284-9e86-6ebec3e1818e")
    IEnumeratorIVxTextSnapshotLine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxTextSnapshotLineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxTextSnapshotLine * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxTextSnapshotLine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxTextSnapshotLine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxTextSnapshotLine * This,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxTextSnapshotLine * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxTextSnapshotLine * This);
        
        END_INTERFACE
    } IEnumeratorIVxTextSnapshotLineVtbl;

    interface IEnumeratorIVxTextSnapshotLine
    {
        CONST_VTBL struct IEnumeratorIVxTextSnapshotLineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxTextSnapshotLine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxTextSnapshotLine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxTextSnapshotLine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxTextSnapshotLine_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxTextSnapshotLine_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxTextSnapshotLine_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxTextSnapshotLine_INTERFACE_DEFINED__ */


#ifndef __IEnumerableIVxTextSnapshotLine_INTERFACE_DEFINED__
#define __IEnumerableIVxTextSnapshotLine_INTERFACE_DEFINED__

/* interface IEnumerableIVxTextSnapshotLine */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumerableIVxTextSnapshotLine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("021b71e1-59bb-481a-9ac6-c9b4ad7b7446")
    IEnumerableIVxTextSnapshotLine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxTextSnapshotLine **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumerableIVxTextSnapshotLineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumerableIVxTextSnapshotLine * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumerableIVxTextSnapshotLine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumerableIVxTextSnapshotLine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IEnumerableIVxTextSnapshotLine * This,
            /* [retval][out] */ IEnumeratorIVxTextSnapshotLine **ppRetVal);
        
        END_INTERFACE
    } IEnumerableIVxTextSnapshotLineVtbl;

    interface IEnumerableIVxTextSnapshotLine
    {
        CONST_VTBL struct IEnumerableIVxTextSnapshotLineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumerableIVxTextSnapshotLine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumerableIVxTextSnapshotLine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumerableIVxTextSnapshotLine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumerableIVxTextSnapshotLine_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumerableIVxTextSnapshotLine_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorVxSpan_INTERFACE_DEFINED__
#define __IEnumeratorVxSpan_INTERFACE_DEFINED__

/* interface IEnumeratorVxSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorVxSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f5c3a3cb-442d-4abc-84b7-687611079c21")
    IEnumeratorVxSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ VxSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorVxSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorVxSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorVxSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorVxSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorVxSpan * This,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorVxSpan * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorVxSpan * This);
        
        END_INTERFACE
    } IEnumeratorVxSpanVtbl;

    interface IEnumeratorVxSpan
    {
        CONST_VTBL struct IEnumeratorVxSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorVxSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorVxSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorVxSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorVxSpan_GetCurrent(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,pRetVal) ) 

#define IEnumeratorVxSpan_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorVxSpan_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorVxSpan_INTERFACE_DEFINED__ */


#ifndef __IListVxSpan_INTERFACE_DEFINED__
#define __IListVxSpan_INTERFACE_DEFINED__

/* interface IListVxSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListVxSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f265752d-e60c-4c02-a3d4-9200a936b1b4")
    IListVxSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ VxSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorVxSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListVxSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListVxSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListVxSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListVxSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListVxSpan * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListVxSpan * This,
            /* [in] */ int index,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListVxSpan * This,
            /* [retval][out] */ IEnumeratorVxSpan **ppRetVal);
        
        END_INTERFACE
    } IListVxSpanVtbl;

    interface IListVxSpan
    {
        CONST_VTBL struct IListVxSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListVxSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListVxSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListVxSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListVxSpan_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListVxSpan_GetElement(This,index,pRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,pRetVal) ) 

#define IListVxSpan_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListVxSpan_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxTextBuffer_INTERFACE_DEFINED__
#define __IEnumeratorIVxTextBuffer_INTERFACE_DEFINED__

/* interface IEnumeratorIVxTextBuffer */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxTextBuffer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7a1e5f7c-6f1d-4e1e-b13d-8656b71f2f97")
    IEnumeratorIVxTextBuffer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxTextBufferVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxTextBuffer * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxTextBuffer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxTextBuffer * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxTextBuffer * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxTextBuffer * This);
        
        END_INTERFACE
    } IEnumeratorIVxTextBufferVtbl;

    interface IEnumeratorIVxTextBuffer
    {
        CONST_VTBL struct IEnumeratorIVxTextBufferVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxTextBuffer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxTextBuffer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxTextBuffer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxTextBuffer_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxTextBuffer_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxTextBuffer_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxTextBuffer_INTERFACE_DEFINED__ */


#ifndef __IListIVxTextBuffer_INTERFACE_DEFINED__
#define __IListIVxTextBuffer_INTERFACE_DEFINED__

/* interface IListIVxTextBuffer */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListIVxTextBuffer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("121811be-de48-4320-b7c6-1237c0d2ff3b")
    IListIVxTextBuffer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxTextBuffer **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListIVxTextBufferVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListIVxTextBuffer * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListIVxTextBuffer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListIVxTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListIVxTextBuffer * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListIVxTextBuffer * This,
            /* [in] */ int index,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListIVxTextBuffer * This,
            /* [retval][out] */ IEnumeratorIVxTextBuffer **ppRetVal);
        
        END_INTERFACE
    } IListIVxTextBufferVtbl;

    interface IListIVxTextBuffer
    {
        CONST_VTBL struct IListIVxTextBufferVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListIVxTextBuffer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListIVxTextBuffer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListIVxTextBuffer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListIVxTextBuffer_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListIVxTextBuffer_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IListIVxTextBuffer_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListIVxTextBuffer_INTERFACE_DEFINED__ */


#ifndef __IVxGraphBuffersChangedEvent_INTERFACE_DEFINED__
#define __IVxGraphBuffersChangedEvent_INTERFACE_DEFINED__

/* interface IVxGraphBuffersChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxGraphBuffersChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d1b54e5e-2782-4cd7-b6a0-2eff76c07dfb")
    IVxGraphBuffersChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnGraphBuffersChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxGraphBuffersChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxGraphBuffersChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxGraphBuffersChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxGraphBuffersChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxGraphBuffersChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnGraphBuffersChanged )( 
            IVxGraphBuffersChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxGraphBuffersChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxGraphBuffersChangedEventVtbl;

    interface IVxGraphBuffersChangedEvent
    {
        CONST_VTBL struct IVxGraphBuffersChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxGraphBuffersChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxGraphBuffersChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxGraphBuffersChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxGraphBuffersChangedEvent_OnGraphBuffersChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnGraphBuffersChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxGraphBuffersChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IVxGraphBufferContentTypeChangedEvent_INTERFACE_DEFINED__
#define __IVxGraphBufferContentTypeChangedEvent_INTERFACE_DEFINED__

/* interface IVxGraphBufferContentTypeChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxGraphBufferContentTypeChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("566861da-0ce4-4f47-896f-133cfb73a62f")
    IVxGraphBufferContentTypeChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnGraphBufferContentTypeChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxGraphBufferContentTypeChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxGraphBufferContentTypeChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxGraphBufferContentTypeChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxGraphBufferContentTypeChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxGraphBufferContentTypeChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnGraphBufferContentTypeChanged )( 
            IVxGraphBufferContentTypeChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxGraphBufferContentTypeChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxGraphBufferContentTypeChangedEventVtbl;

    interface IVxGraphBufferContentTypeChangedEvent
    {
        CONST_VTBL struct IVxGraphBufferContentTypeChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxGraphBufferContentTypeChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxGraphBufferContentTypeChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxGraphBufferContentTypeChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxGraphBufferContentTypeChangedEvent_OnGraphBufferContentTypeChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnGraphBufferContentTypeChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxGraphBufferContentTypeChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxTextSnapshot_INTERFACE_DEFINED__
#define __IEnumeratorIVxTextSnapshot_INTERFACE_DEFINED__

/* interface IEnumeratorIVxTextSnapshot */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxTextSnapshot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("13dc5d4f-7e53-4180-8779-1364b6ae8322")
    IEnumeratorIVxTextSnapshot : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxTextSnapshotVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxTextSnapshot * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxTextSnapshot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxTextSnapshot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxTextSnapshot * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxTextSnapshot * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxTextSnapshot * This);
        
        END_INTERFACE
    } IEnumeratorIVxTextSnapshotVtbl;

    interface IEnumeratorIVxTextSnapshot
    {
        CONST_VTBL struct IEnumeratorIVxTextSnapshotVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxTextSnapshot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxTextSnapshot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxTextSnapshot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxTextSnapshot_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxTextSnapshot_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxTextSnapshot_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxTextSnapshot_INTERFACE_DEFINED__ */


#ifndef __IListIVxTextSnapshot_INTERFACE_DEFINED__
#define __IListIVxTextSnapshot_INTERFACE_DEFINED__

/* interface IListIVxTextSnapshot */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListIVxTextSnapshot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b886572e-8f74-43c8-b8c1-f8bc0bdb5d1c")
    IListIVxTextSnapshot : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxTextSnapshot **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListIVxTextSnapshotVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListIVxTextSnapshot * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListIVxTextSnapshot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListIVxTextSnapshot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListIVxTextSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListIVxTextSnapshot * This,
            /* [in] */ int index,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListIVxTextSnapshot * This,
            /* [retval][out] */ IEnumeratorIVxTextSnapshot **ppRetVal);
        
        END_INTERFACE
    } IListIVxTextSnapshotVtbl;

    interface IListIVxTextSnapshot
    {
        CONST_VTBL struct IListIVxTextSnapshotVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListIVxTextSnapshot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListIVxTextSnapshot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListIVxTextSnapshot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListIVxTextSnapshot_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListIVxTextSnapshot_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IListIVxTextSnapshot_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListIVxTextSnapshot_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorVxSnapshotSpan_INTERFACE_DEFINED__
#define __IEnumeratorVxSnapshotSpan_INTERFACE_DEFINED__

/* interface IEnumeratorVxSnapshotSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorVxSnapshotSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fd6cb9a7-f2c2-450d-b14e-756b5a013591")
    IEnumeratorVxSnapshotSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorVxSnapshotSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorVxSnapshotSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorVxSnapshotSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorVxSnapshotSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorVxSnapshotSpan * This,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorVxSnapshotSpan * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorVxSnapshotSpan * This);
        
        END_INTERFACE
    } IEnumeratorVxSnapshotSpanVtbl;

    interface IEnumeratorVxSnapshotSpan
    {
        CONST_VTBL struct IEnumeratorVxSnapshotSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorVxSnapshotSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorVxSnapshotSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorVxSnapshotSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorVxSnapshotSpan_GetCurrent(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,pRetVal) ) 

#define IEnumeratorVxSnapshotSpan_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorVxSnapshotSpan_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorVxSnapshotSpan_INTERFACE_DEFINED__ */


#ifndef __IListVxSnapshotSpan_INTERFACE_DEFINED__
#define __IListVxSnapshotSpan_INTERFACE_DEFINED__

/* interface IListVxSnapshotSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListVxSnapshotSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("75970b1d-36b9-419b-bf43-ec2a66776955")
    IListVxSnapshotSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorVxSnapshotSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListVxSnapshotSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListVxSnapshotSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListVxSnapshotSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListVxSnapshotSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListVxSnapshotSpan * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListVxSnapshotSpan * This,
            /* [in] */ int index,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListVxSnapshotSpan * This,
            /* [retval][out] */ IEnumeratorVxSnapshotSpan **ppRetVal);
        
        END_INTERFACE
    } IListVxSnapshotSpanVtbl;

    interface IListVxSnapshotSpan
    {
        CONST_VTBL struct IListVxSnapshotSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListVxSnapshotSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListVxSnapshotSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListVxSnapshotSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListVxSnapshotSpan_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListVxSnapshotSpan_GetElement(This,index,pRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,pRetVal) ) 

#define IListVxSnapshotSpan_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListVxSnapshotSpan_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorVxSnapshotPoint_INTERFACE_DEFINED__
#define __IEnumeratorVxSnapshotPoint_INTERFACE_DEFINED__

/* interface IEnumeratorVxSnapshotPoint */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorVxSnapshotPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30ddec83-f0b5-4bd7-a4d3-2d82d51d6d70")
    IEnumeratorVxSnapshotPoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorVxSnapshotPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorVxSnapshotPoint * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorVxSnapshotPoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorVxSnapshotPoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorVxSnapshotPoint * This,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorVxSnapshotPoint * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorVxSnapshotPoint * This);
        
        END_INTERFACE
    } IEnumeratorVxSnapshotPointVtbl;

    interface IEnumeratorVxSnapshotPoint
    {
        CONST_VTBL struct IEnumeratorVxSnapshotPointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorVxSnapshotPoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorVxSnapshotPoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorVxSnapshotPoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorVxSnapshotPoint_GetCurrent(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,pRetVal) ) 

#define IEnumeratorVxSnapshotPoint_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorVxSnapshotPoint_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorVxSnapshotPoint_INTERFACE_DEFINED__ */


#ifndef __IListVxSnapshotPoint_INTERFACE_DEFINED__
#define __IListVxSnapshotPoint_INTERFACE_DEFINED__

/* interface IListVxSnapshotPoint */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListVxSnapshotPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("587a5a1a-4b4e-43f8-896f-7d61716a33cb")
    IListVxSnapshotPoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorVxSnapshotPoint **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListVxSnapshotPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListVxSnapshotPoint * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListVxSnapshotPoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListVxSnapshotPoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListVxSnapshotPoint * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListVxSnapshotPoint * This,
            /* [in] */ int index,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListVxSnapshotPoint * This,
            /* [retval][out] */ IEnumeratorVxSnapshotPoint **ppRetVal);
        
        END_INTERFACE
    } IListVxSnapshotPointVtbl;

    interface IListVxSnapshotPoint
    {
        CONST_VTBL struct IListVxSnapshotPointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListVxSnapshotPoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListVxSnapshotPoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListVxSnapshotPoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListVxSnapshotPoint_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListVxSnapshotPoint_GetElement(This,index,pRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,pRetVal) ) 

#define IListVxSnapshotPoint_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListVxSnapshotPoint_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIUnknown_INTERFACE_DEFINED__
#define __IEnumeratorIUnknown_INTERFACE_DEFINED__

/* interface IEnumeratorIUnknown */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIUnknown;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("379d6c69-25b8-413d-b58c-015f2c7a942d")
    IEnumeratorIUnknown : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IUnknown **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIUnknownVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIUnknown * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIUnknown * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIUnknown * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIUnknown * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIUnknown * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIUnknown * This);
        
        END_INTERFACE
    } IEnumeratorIUnknownVtbl;

    interface IEnumeratorIUnknown
    {
        CONST_VTBL struct IEnumeratorIUnknownVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIUnknown_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIUnknown_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIUnknown_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIUnknown_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIUnknown_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIUnknown_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIUnknown_INTERFACE_DEFINED__ */


#ifndef __IListIUnknown_INTERFACE_DEFINED__
#define __IListIUnknown_INTERFACE_DEFINED__

/* interface IListIUnknown */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListIUnknown;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fdc0603f-f733-49c9-aad2-ac3e0ae513bb")
    IListIUnknown : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ IUnknown **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIUnknown **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListIUnknownVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListIUnknown * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListIUnknown * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListIUnknown * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListIUnknown * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListIUnknown * This,
            /* [in] */ int index,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListIUnknown * This,
            /* [retval][out] */ IEnumeratorIUnknown **ppRetVal);
        
        END_INTERFACE
    } IListIUnknownVtbl;

    interface IListIUnknown
    {
        CONST_VTBL struct IListIUnknownVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListIUnknown_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListIUnknown_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListIUnknown_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListIUnknown_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListIUnknown_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IListIUnknown_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListIUnknown_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionSourceSpansChangedEvent_INTERFACE_DEFINED__
#define __IVxProjectionSourceSpansChangedEvent_INTERFACE_DEFINED__

/* interface IVxProjectionSourceSpansChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionSourceSpansChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6c6c11a3-ec8a-426d-9c73-180431dcf306")
    IVxProjectionSourceSpansChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnProjectionSourceSpansChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxProjectionSourceSpansChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionSourceSpansChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionSourceSpansChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionSourceSpansChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionSourceSpansChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnProjectionSourceSpansChanged )( 
            IVxProjectionSourceSpansChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxProjectionSourceSpansChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxProjectionSourceSpansChangedEventVtbl;

    interface IVxProjectionSourceSpansChangedEvent
    {
        CONST_VTBL struct IVxProjectionSourceSpansChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionSourceSpansChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionSourceSpansChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionSourceSpansChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionSourceSpansChangedEvent_OnProjectionSourceSpansChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnProjectionSourceSpansChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionSourceSpansChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionSourceBuffersChangedEvent_INTERFACE_DEFINED__
#define __IVxProjectionSourceBuffersChangedEvent_INTERFACE_DEFINED__

/* interface IVxProjectionSourceBuffersChangedEvent */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionSourceBuffersChangedEvent;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5fd8dc1e-0c10-4026-8fde-a3f198815998")
    IVxProjectionSourceBuffersChangedEvent : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnProjectionSourceBuffersChanged( 
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxProjectionSourceBuffersChangedEventArgs *pE) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionSourceBuffersChangedEventVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionSourceBuffersChangedEvent * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionSourceBuffersChangedEvent * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionSourceBuffersChangedEvent * This);
        
        HRESULT ( STDMETHODCALLTYPE *OnProjectionSourceBuffersChanged )( 
            IVxProjectionSourceBuffersChangedEvent * This,
            /* [in] */ IUnknown *pSender,
            /* [in] */ IVxProjectionSourceBuffersChangedEventArgs *pE);
        
        END_INTERFACE
    } IVxProjectionSourceBuffersChangedEventVtbl;

    interface IVxProjectionSourceBuffersChangedEvent
    {
        CONST_VTBL struct IVxProjectionSourceBuffersChangedEventVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionSourceBuffersChangedEvent_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionSourceBuffersChangedEvent_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionSourceBuffersChangedEvent_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionSourceBuffersChangedEvent_OnProjectionSourceBuffersChanged(This,pSender,pE)	\
    ( (This)->lpVtbl -> OnProjectionSourceBuffersChanged(This,pSender,pE) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionSourceBuffersChangedEvent_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxTrackingSpan_INTERFACE_DEFINED__
#define __IEnumeratorIVxTrackingSpan_INTERFACE_DEFINED__

/* interface IEnumeratorIVxTrackingSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxTrackingSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("496727c4-5622-4889-94ae-ddd857bacea7")
    IEnumeratorIVxTrackingSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxTrackingSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxTrackingSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxTrackingSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxTrackingSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxTrackingSpan * This,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxTrackingSpan * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxTrackingSpan * This);
        
        END_INTERFACE
    } IEnumeratorIVxTrackingSpanVtbl;

    interface IEnumeratorIVxTrackingSpan
    {
        CONST_VTBL struct IEnumeratorIVxTrackingSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxTrackingSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxTrackingSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxTrackingSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxTrackingSpan_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxTrackingSpan_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxTrackingSpan_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxTrackingSpan_INTERFACE_DEFINED__ */


#ifndef __IListIVxTrackingSpan_INTERFACE_DEFINED__
#define __IListIVxTrackingSpan_INTERFACE_DEFINED__

/* interface IListIVxTrackingSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListIVxTrackingSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("fb17674f-3e6c-4ad2-9d17-e605c3f45262")
    IListIVxTrackingSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxTrackingSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListIVxTrackingSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListIVxTrackingSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListIVxTrackingSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListIVxTrackingSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListIVxTrackingSpan * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListIVxTrackingSpan * This,
            /* [in] */ int index,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListIVxTrackingSpan * This,
            /* [retval][out] */ IEnumeratorIVxTrackingSpan **ppRetVal);
        
        END_INTERFACE
    } IListIVxTrackingSpanVtbl;

    interface IListIVxTrackingSpan
    {
        CONST_VTBL struct IListIVxTrackingSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListIVxTrackingSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListIVxTrackingSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListIVxTrackingSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListIVxTrackingSpan_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListIVxTrackingSpan_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IListIVxTrackingSpan_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListIVxTrackingSpan_INTERFACE_DEFINED__ */


#ifndef __IEnumeratorIVxTextChange_INTERFACE_DEFINED__
#define __IEnumeratorIVxTextChange_INTERFACE_DEFINED__

/* interface IEnumeratorIVxTextChange */
/* [uuid][object] */ 


EXTERN_C const IID IID_IEnumeratorIVxTextChange;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a4728f3b-90ee-4783-a6af-0caeeea73147")
    IEnumeratorIVxTextChange : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrent( 
            /* [retval][out] */ IVxTextChange **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MoveNext( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IEnumeratorIVxTextChangeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEnumeratorIVxTextChange * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEnumeratorIVxTextChange * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEnumeratorIVxTextChange * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrent )( 
            IEnumeratorIVxTextChange * This,
            /* [retval][out] */ IVxTextChange **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MoveNext )( 
            IEnumeratorIVxTextChange * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IEnumeratorIVxTextChange * This);
        
        END_INTERFACE
    } IEnumeratorIVxTextChangeVtbl;

    interface IEnumeratorIVxTextChange
    {
        CONST_VTBL struct IEnumeratorIVxTextChangeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEnumeratorIVxTextChange_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEnumeratorIVxTextChange_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEnumeratorIVxTextChange_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEnumeratorIVxTextChange_GetCurrent(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrent(This,ppRetVal) ) 

#define IEnumeratorIVxTextChange_MoveNext(This,pRetVal)	\
    ( (This)->lpVtbl -> MoveNext(This,pRetVal) ) 

#define IEnumeratorIVxTextChange_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEnumeratorIVxTextChange_INTERFACE_DEFINED__ */


#ifndef __IListIVxTextChange_INTERFACE_DEFINED__
#define __IListIVxTextChange_INTERFACE_DEFINED__

/* interface IListIVxTextChange */
/* [uuid][object] */ 


EXTERN_C const IID IID_IListIVxTextChange;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7627a97f-187f-428e-bba9-07183f73a088")
    IListIVxTextChange : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetElement( 
            /* [in] */ int index,
            /* [retval][out] */ IVxTextChange **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnumerator( 
            /* [retval][out] */ IEnumeratorIVxTextChange **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IListIVxTextChangeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IListIVxTextChange * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IListIVxTextChange * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IListIVxTextChange * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IListIVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IListIVxTextChange * This,
            /* [in] */ int index,
            /* [retval][out] */ IVxTextChange **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IListIVxTextChange * This,
            /* [retval][out] */ IEnumeratorIVxTextChange **ppRetVal);
        
        END_INTERFACE
    } IListIVxTextChangeVtbl;

    interface IListIVxTextChange
    {
        CONST_VTBL struct IListIVxTextChangeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IListIVxTextChange_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IListIVxTextChange_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IListIVxTextChange_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IListIVxTextChange_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IListIVxTextChange_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IListIVxTextChange_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IListIVxTextChange_INTERFACE_DEFINED__ */


#ifndef __IVxDisposable_INTERFACE_DEFINED__
#define __IVxDisposable_INTERFACE_DEFINED__

/* interface IVxDisposable */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxDisposable;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00c8c1e7-de26-42cf-a058-5ccda2b42beb")
    IVxDisposable : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Dispose( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxDisposableVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxDisposable * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxDisposable * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxDisposable * This);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IVxDisposable * This);
        
        END_INTERFACE
    } IVxDisposableVtbl;

    interface IVxDisposable
    {
        CONST_VTBL struct IVxDisposableVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxDisposable_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxDisposable_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxDisposable_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxDisposable_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxDisposable_INTERFACE_DEFINED__ */


#ifndef __IVxPropertyOwner_INTERFACE_DEFINED__
#define __IVxPropertyOwner_INTERFACE_DEFINED__

/* interface IVxPropertyOwner */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxPropertyOwner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("b7b3e0d6-f9f3-415e-a977-a06cc723aff1")
    IVxPropertyOwner : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetProperties( 
            /* [retval][out] */ IVxPropertyCollection **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxPropertyOwnerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxPropertyOwner * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxPropertyOwner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxPropertyOwner * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperties )( 
            IVxPropertyOwner * This,
            /* [retval][out] */ IVxPropertyCollection **ppRetVal);
        
        END_INTERFACE
    } IVxPropertyOwnerVtbl;

    interface IVxPropertyOwner
    {
        CONST_VTBL struct IVxPropertyOwnerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxPropertyOwner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxPropertyOwner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxPropertyOwner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxPropertyOwner_GetProperties(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetProperties(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxPropertyOwner_INTERFACE_DEFINED__ */


#ifndef __IVxPropertyCollection_INTERFACE_DEFINED__
#define __IVxPropertyCollection_INTERFACE_DEFINED__

/* interface IVxPropertyCollection */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxPropertyCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("275f2096-2f87-4886-bff7-a216f6decaab")
    IVxPropertyCollection : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE AddProperty( 
            /* [in] */ IUnknown *pKey,
            /* [in] */ IUnknown *pProperty) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveProperty( 
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetProperty_2( 
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ IUnknown **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ContainsProperty( 
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxPropertyCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxPropertyCollection * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxPropertyCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxPropertyCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddProperty )( 
            IVxPropertyCollection * This,
            /* [in] */ IUnknown *pKey,
            /* [in] */ IUnknown *pProperty);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveProperty )( 
            IVxPropertyCollection * This,
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperty_2 )( 
            IVxPropertyCollection * This,
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *ContainsProperty )( 
            IVxPropertyCollection * This,
            /* [in] */ IUnknown *pKey,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxPropertyCollectionVtbl;

    interface IVxPropertyCollection
    {
        CONST_VTBL struct IVxPropertyCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxPropertyCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxPropertyCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxPropertyCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxPropertyCollection_AddProperty(This,pKey,pProperty)	\
    ( (This)->lpVtbl -> AddProperty(This,pKey,pProperty) ) 

#define IVxPropertyCollection_RemoveProperty(This,pKey,pRetVal)	\
    ( (This)->lpVtbl -> RemoveProperty(This,pKey,pRetVal) ) 

#define IVxPropertyCollection_GetProperty_2(This,pKey,ppRetVal)	\
    ( (This)->lpVtbl -> GetProperty_2(This,pKey,ppRetVal) ) 

#define IVxPropertyCollection_ContainsProperty(This,pKey,pRetVal)	\
    ( (This)->lpVtbl -> ContainsProperty(This,pKey,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxPropertyCollection_INTERFACE_DEFINED__ */


#ifndef __IVxContentTypeRegistryService_INTERFACE_DEFINED__
#define __IVxContentTypeRegistryService_INTERFACE_DEFINED__

/* interface IVxContentTypeRegistryService */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxContentTypeRegistryService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("aeefbb53-ff73-45d0-a826-4f978725d21e")
    IVxContentTypeRegistryService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetContentType( 
            /* [in] */ BSTR typeName,
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddContentType( 
            /* [in] */ BSTR typeName,
            /* [in] */ IEnumerableBSTR *pBaseTypeNames,
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveContentType( 
            /* [in] */ BSTR typeName) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUnknownContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContentTypes( 
            /* [retval][out] */ IEnumerableIVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxContentTypeRegistryServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxContentTypeRegistryService * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxContentTypeRegistryService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxContentTypeRegistryService * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxContentTypeRegistryService * This,
            /* [in] */ BSTR typeName,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AddContentType )( 
            IVxContentTypeRegistryService * This,
            /* [in] */ BSTR typeName,
            /* [in] */ IEnumerableBSTR *pBaseTypeNames,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveContentType )( 
            IVxContentTypeRegistryService * This,
            /* [in] */ BSTR typeName);
        
        HRESULT ( STDMETHODCALLTYPE *GetUnknownContentType )( 
            IVxContentTypeRegistryService * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentTypes )( 
            IVxContentTypeRegistryService * This,
            /* [retval][out] */ IEnumerableIVxContentType **ppRetVal);
        
        END_INTERFACE
    } IVxContentTypeRegistryServiceVtbl;

    interface IVxContentTypeRegistryService
    {
        CONST_VTBL struct IVxContentTypeRegistryServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxContentTypeRegistryService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxContentTypeRegistryService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxContentTypeRegistryService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxContentTypeRegistryService_GetContentType(This,typeName,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,typeName,ppRetVal) ) 

#define IVxContentTypeRegistryService_AddContentType(This,typeName,pBaseTypeNames,ppRetVal)	\
    ( (This)->lpVtbl -> AddContentType(This,typeName,pBaseTypeNames,ppRetVal) ) 

#define IVxContentTypeRegistryService_RemoveContentType(This,typeName)	\
    ( (This)->lpVtbl -> RemoveContentType(This,typeName) ) 

#define IVxContentTypeRegistryService_GetUnknownContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetUnknownContentType(This,ppRetVal) ) 

#define IVxContentTypeRegistryService_GetContentTypes(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentTypes(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxContentTypeRegistryService_INTERFACE_DEFINED__ */


#ifndef __IVxContentType_INTERFACE_DEFINED__
#define __IVxContentType_INTERFACE_DEFINED__

/* interface IVxContentType */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxContentType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("6faa3727-ed8f-4fc0-8032-82ad17394029")
    IVxContentType : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTypeName( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDisplayName( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsOfType( 
            /* [in] */ BSTR type,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBaseTypes( 
            /* [retval][out] */ IEnumerableIVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxContentTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxContentType * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxContentType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxContentType * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeName )( 
            IVxContentType * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetDisplayName )( 
            IVxContentType * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsOfType )( 
            IVxContentType * This,
            /* [in] */ BSTR type,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBaseTypes )( 
            IVxContentType * This,
            /* [retval][out] */ IEnumerableIVxContentType **ppRetVal);
        
        END_INTERFACE
    } IVxContentTypeVtbl;

    interface IVxContentType
    {
        CONST_VTBL struct IVxContentTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxContentType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxContentType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxContentType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxContentType_GetTypeName(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTypeName(This,pRetVal) ) 

#define IVxContentType_GetDisplayName(This,pRetVal)	\
    ( (This)->lpVtbl -> GetDisplayName(This,pRetVal) ) 

#define IVxContentType_IsOfType(This,type,pRetVal)	\
    ( (This)->lpVtbl -> IsOfType(This,type,pRetVal) ) 

#define IVxContentType_GetBaseTypes(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBaseTypes(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxContentType_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocument_INTERFACE_DEFINED__
#define __IVxTextDocument_INTERFACE_DEFINED__

/* interface IVxTextDocument */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocument;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5498337a-84b2-41bf-ab74-e5b88419079f")
    IVxTextDocument : public IVxDisposable
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFilePath( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIsDirty( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseFileActionOccurred( 
            /* [in] */ IVxTextDocumentFileActionEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseFileActionOccurred( 
            /* [in] */ IVxTextDocumentFileActionEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseDirtyStateChanged( 
            /* [in] */ IVxEventArgsEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseDirtyStateChanged( 
            /* [in] */ IVxEventArgsEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Rename( 
            /* [in] */ BSTR newFilePath) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reload( 
            /* [retval][out] */ VxReloadResult *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Reload_2( 
            /* [in] */ VxEditOptions options,
            /* [retval][out] */ VxReloadResult *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetIsReloading( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Save( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveAs( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveAs_2( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveAs_3( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ IVxContentType *pNewContentType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveAs_4( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder,
            /* [in] */ IVxContentType *pNewContentType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveCopy( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SaveCopy_2( 
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocument * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocument * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *Dispose )( 
            IVxTextDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilePath )( 
            IVxTextDocument * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTextDocument * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetIsDirty )( 
            IVxTextDocument * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseFileActionOccurred )( 
            IVxTextDocument * This,
            /* [in] */ IVxTextDocumentFileActionEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseFileActionOccurred )( 
            IVxTextDocument * This,
            /* [in] */ IVxTextDocumentFileActionEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseDirtyStateChanged )( 
            IVxTextDocument * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseDirtyStateChanged )( 
            IVxTextDocument * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *Rename )( 
            IVxTextDocument * This,
            /* [in] */ BSTR newFilePath);
        
        HRESULT ( STDMETHODCALLTYPE *Reload )( 
            IVxTextDocument * This,
            /* [retval][out] */ VxReloadResult *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Reload_2 )( 
            IVxTextDocument * This,
            /* [in] */ VxEditOptions options,
            /* [retval][out] */ VxReloadResult *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetIsReloading )( 
            IVxTextDocument * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Save )( 
            IVxTextDocument * This);
        
        HRESULT ( STDMETHODCALLTYPE *SaveAs )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite);
        
        HRESULT ( STDMETHODCALLTYPE *SaveAs_2 )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder);
        
        HRESULT ( STDMETHODCALLTYPE *SaveAs_3 )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ IVxContentType *pNewContentType);
        
        HRESULT ( STDMETHODCALLTYPE *SaveAs_4 )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder,
            /* [in] */ IVxContentType *pNewContentType);
        
        HRESULT ( STDMETHODCALLTYPE *SaveCopy )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite);
        
        HRESULT ( STDMETHODCALLTYPE *SaveCopy_2 )( 
            IVxTextDocument * This,
            /* [in] */ BSTR filePath,
            /* [in] */ BOOL overwrite,
            /* [in] */ BOOL createFolder);
        
        END_INTERFACE
    } IVxTextDocumentVtbl;

    interface IVxTextDocument
    {
        CONST_VTBL struct IVxTextDocumentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocument_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocument_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocument_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocument_Dispose(This)	\
    ( (This)->lpVtbl -> Dispose(This) ) 


#define IVxTextDocument_GetFilePath(This,pRetVal)	\
    ( (This)->lpVtbl -> GetFilePath(This,pRetVal) ) 

#define IVxTextDocument_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxTextDocument_GetIsDirty(This,pRetVal)	\
    ( (This)->lpVtbl -> GetIsDirty(This,pRetVal) ) 

#define IVxTextDocument_AdviseFileActionOccurred(This,pValue)	\
    ( (This)->lpVtbl -> AdviseFileActionOccurred(This,pValue) ) 

#define IVxTextDocument_UnadviseFileActionOccurred(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseFileActionOccurred(This,pValue) ) 

#define IVxTextDocument_AdviseDirtyStateChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseDirtyStateChanged(This,pValue) ) 

#define IVxTextDocument_UnadviseDirtyStateChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseDirtyStateChanged(This,pValue) ) 

#define IVxTextDocument_Rename(This,newFilePath)	\
    ( (This)->lpVtbl -> Rename(This,newFilePath) ) 

#define IVxTextDocument_Reload(This,pRetVal)	\
    ( (This)->lpVtbl -> Reload(This,pRetVal) ) 

#define IVxTextDocument_Reload_2(This,options,pRetVal)	\
    ( (This)->lpVtbl -> Reload_2(This,options,pRetVal) ) 

#define IVxTextDocument_GetIsReloading(This,pRetVal)	\
    ( (This)->lpVtbl -> GetIsReloading(This,pRetVal) ) 

#define IVxTextDocument_Save(This)	\
    ( (This)->lpVtbl -> Save(This) ) 

#define IVxTextDocument_SaveAs(This,filePath,overwrite)	\
    ( (This)->lpVtbl -> SaveAs(This,filePath,overwrite) ) 

#define IVxTextDocument_SaveAs_2(This,filePath,overwrite,createFolder)	\
    ( (This)->lpVtbl -> SaveAs_2(This,filePath,overwrite,createFolder) ) 

#define IVxTextDocument_SaveAs_3(This,filePath,overwrite,pNewContentType)	\
    ( (This)->lpVtbl -> SaveAs_3(This,filePath,overwrite,pNewContentType) ) 

#define IVxTextDocument_SaveAs_4(This,filePath,overwrite,createFolder,pNewContentType)	\
    ( (This)->lpVtbl -> SaveAs_4(This,filePath,overwrite,createFolder,pNewContentType) ) 

#define IVxTextDocument_SaveCopy(This,filePath,overwrite)	\
    ( (This)->lpVtbl -> SaveCopy(This,filePath,overwrite) ) 

#define IVxTextDocument_SaveCopy_2(This,filePath,overwrite,createFolder)	\
    ( (This)->lpVtbl -> SaveCopy_2(This,filePath,overwrite,createFolder) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocument_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocumentFactoryService_INTERFACE_DEFINED__
#define __IVxTextDocumentFactoryService_INTERFACE_DEFINED__

/* interface IVxTextDocumentFactoryService */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocumentFactoryService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f4d19c93-7267-4509-9284-88e8c08be615")
    IVxTextDocumentFactoryService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateAndLoadTextDocument( 
            /* [in] */ BSTR filePath,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextDocument **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextDocument( 
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [in] */ BSTR filePath,
            /* [retval][out] */ IVxTextDocument **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TryGetTextDocument( 
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [out] */ IVxTextDocument **ppTextDocument,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextDocumentCreated( 
            /* [in] */ IVxTextDocumentEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextDocumentCreated( 
            /* [in] */ IVxTextDocumentEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextDocumentDisposed( 
            /* [in] */ IVxTextDocumentEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextDocumentDisposed( 
            /* [in] */ IVxTextDocumentEvent *pValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentFactoryServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocumentFactoryService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocumentFactoryService * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateAndLoadTextDocument )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ BSTR filePath,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextDocument **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextDocument )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [in] */ BSTR filePath,
            /* [retval][out] */ IVxTextDocument **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *TryGetTextDocument )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [out] */ IVxTextDocument **ppTextDocument,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextDocumentCreated )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextDocumentEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextDocumentCreated )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextDocumentEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextDocumentDisposed )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextDocumentEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextDocumentDisposed )( 
            IVxTextDocumentFactoryService * This,
            /* [in] */ IVxTextDocumentEvent *pValue);
        
        END_INTERFACE
    } IVxTextDocumentFactoryServiceVtbl;

    interface IVxTextDocumentFactoryService
    {
        CONST_VTBL struct IVxTextDocumentFactoryServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocumentFactoryService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocumentFactoryService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocumentFactoryService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocumentFactoryService_CreateAndLoadTextDocument(This,filePath,pContentType,ppRetVal)	\
    ( (This)->lpVtbl -> CreateAndLoadTextDocument(This,filePath,pContentType,ppRetVal) ) 

#define IVxTextDocumentFactoryService_CreateTextDocument(This,pTextBuffer,filePath,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextDocument(This,pTextBuffer,filePath,ppRetVal) ) 

#define IVxTextDocumentFactoryService_TryGetTextDocument(This,pTextBuffer,ppTextDocument,pRetVal)	\
    ( (This)->lpVtbl -> TryGetTextDocument(This,pTextBuffer,ppTextDocument,pRetVal) ) 

#define IVxTextDocumentFactoryService_AdviseTextDocumentCreated(This,pValue)	\
    ( (This)->lpVtbl -> AdviseTextDocumentCreated(This,pValue) ) 

#define IVxTextDocumentFactoryService_UnadviseTextDocumentCreated(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseTextDocumentCreated(This,pValue) ) 

#define IVxTextDocumentFactoryService_AdviseTextDocumentDisposed(This,pValue)	\
    ( (This)->lpVtbl -> AdviseTextDocumentDisposed(This,pValue) ) 

#define IVxTextDocumentFactoryService_UnadviseTextDocumentDisposed(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseTextDocumentDisposed(This,pValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocumentFactoryService_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocumentEventArgs_INTERFACE_DEFINED__
#define __IVxTextDocumentEventArgs_INTERFACE_DEFINED__

/* interface IVxTextDocumentEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocumentEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d85f080d-428e-4ac7-b478-dac2f0cb3a4e")
    IVxTextDocumentEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextDocument( 
            /* [retval][out] */ IVxTextDocument **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocumentEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocumentEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocumentEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextDocument )( 
            IVxTextDocumentEventArgs * This,
            /* [retval][out] */ IVxTextDocument **ppRetVal);
        
        END_INTERFACE
    } IVxTextDocumentEventArgsVtbl;

    interface IVxTextDocumentEventArgs
    {
        CONST_VTBL struct IVxTextDocumentEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocumentEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocumentEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocumentEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocumentEventArgs_GetTextDocument(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextDocument(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocumentEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxTextDocumentFileActionEventArgs_INTERFACE_DEFINED__
#define __IVxTextDocumentFileActionEventArgs_INTERFACE_DEFINED__

/* interface IVxTextDocumentFileActionEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextDocumentFileActionEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("dc0c964d-81ae-4d24-8d4f-9340cc78d6ae")
    IVxTextDocumentFileActionEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFilePath( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFileActionType( 
            /* [retval][out] */ VxFileActionTypes *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextDocumentFileActionEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextDocumentFileActionEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextDocumentFileActionEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextDocumentFileActionEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFilePath )( 
            IVxTextDocumentFileActionEventArgs * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetFileActionType )( 
            IVxTextDocumentFileActionEventArgs * This,
            /* [retval][out] */ VxFileActionTypes *pRetVal);
        
        END_INTERFACE
    } IVxTextDocumentFileActionEventArgsVtbl;

    interface IVxTextDocumentFileActionEventArgs
    {
        CONST_VTBL struct IVxTextDocumentFileActionEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextDocumentFileActionEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextDocumentFileActionEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextDocumentFileActionEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextDocumentFileActionEventArgs_GetFilePath(This,pRetVal)	\
    ( (This)->lpVtbl -> GetFilePath(This,pRetVal) ) 

#define IVxTextDocumentFileActionEventArgs_GetFileActionType(This,pRetVal)	\
    ( (This)->lpVtbl -> GetFileActionType(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextDocumentFileActionEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxTextSnapshotChangedEventArgs_INTERFACE_DEFINED__
#define __IVxTextSnapshotChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxTextSnapshotChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextSnapshotChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ab1c7415-f8da-41f3-8262-221a2cf59cc4")
    IVxTextSnapshotChangedEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBefore( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAfter( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBeforeVersion( 
            /* [retval][out] */ IVxTextVersion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAfterVersion( 
            /* [retval][out] */ IVxTextVersion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditTag( 
            /* [retval][out] */ IUnknown **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextSnapshotChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextSnapshotChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextSnapshotChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterVersion )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxTextSnapshotChangedEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        END_INTERFACE
    } IVxTextSnapshotChangedEventArgsVtbl;

    interface IVxTextSnapshotChangedEventArgs
    {
        CONST_VTBL struct IVxTextSnapshotChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextSnapshotChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextSnapshotChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextSnapshotChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextSnapshotChangedEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxTextSnapshotChangedEventArgs_GetAfter(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter(This,ppRetVal) ) 

#define IVxTextSnapshotChangedEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#define IVxTextSnapshotChangedEventArgs_GetAfterVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterVersion(This,ppRetVal) ) 

#define IVxTextSnapshotChangedEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextSnapshotChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxContentTypeChangedEventArgs_INTERFACE_DEFINED__
#define __IVxContentTypeChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxContentTypeChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxContentTypeChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("e9ed1827-223d-4cf1-aa25-09c3a0ff6f67")
    IVxContentTypeChangedEventArgs : public IVxTextSnapshotChangedEventArgs
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetBeforeContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAfterContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxContentTypeChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxContentTypeChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxContentTypeChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxContentTypeChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterVersion )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeContentType )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterContentType )( 
            IVxContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        END_INTERFACE
    } IVxContentTypeChangedEventArgsVtbl;

    interface IVxContentTypeChangedEventArgs
    {
        CONST_VTBL struct IVxContentTypeChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxContentTypeChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxContentTypeChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxContentTypeChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxContentTypeChangedEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxContentTypeChangedEventArgs_GetAfter(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter(This,ppRetVal) ) 

#define IVxContentTypeChangedEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#define IVxContentTypeChangedEventArgs_GetAfterVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterVersion(This,ppRetVal) ) 

#define IVxContentTypeChangedEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 


#define IVxContentTypeChangedEventArgs_GetBeforeContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeContentType(This,ppRetVal) ) 

#define IVxContentTypeChangedEventArgs_GetAfterContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterContentType(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxContentTypeChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxMappingPoint_INTERFACE_DEFINED__
#define __IVxMappingPoint_INTERFACE_DEFINED__

/* interface IVxMappingPoint */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxMappingPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("446d7a04-48a5-4721-91a7-bf28c6653192")
    IVxMappingPoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPoint( 
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPoint_2( 
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAnchorBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBufferGraph( 
            /* [retval][out] */ IVxBufferGraph **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxMappingPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxMappingPoint * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxMappingPoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxMappingPoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetPoint )( 
            IVxMappingPoint * This,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetPoint_2 )( 
            IVxMappingPoint * This,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAnchorBuffer )( 
            IVxMappingPoint * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBufferGraph )( 
            IVxMappingPoint * This,
            /* [retval][out] */ IVxBufferGraph **ppRetVal);
        
        END_INTERFACE
    } IVxMappingPointVtbl;

    interface IVxMappingPoint
    {
        CONST_VTBL struct IVxMappingPointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxMappingPoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxMappingPoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxMappingPoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxMappingPoint_GetPoint(This,pTargetBuffer,affinity,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> GetPoint(This,pTargetBuffer,affinity,pRetValHasValue,pRetVal) ) 

#define IVxMappingPoint_GetPoint_2(This,pTargetSnapshot,affinity,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> GetPoint_2(This,pTargetSnapshot,affinity,pRetValHasValue,pRetVal) ) 

#define IVxMappingPoint_GetAnchorBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAnchorBuffer(This,ppRetVal) ) 

#define IVxMappingPoint_GetBufferGraph(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBufferGraph(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxMappingPoint_INTERFACE_DEFINED__ */


#ifndef __IVxMappingSpan_INTERFACE_DEFINED__
#define __IVxMappingSpan_INTERFACE_DEFINED__

/* interface IVxMappingSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxMappingSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("db0e4bfb-1747-494b-a4c6-1eb544fb9df7")
    IVxMappingSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSpans( 
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpans_2( 
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStart( 
            /* [retval][out] */ IVxMappingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnd( 
            /* [retval][out] */ IVxMappingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAnchorBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBufferGraph( 
            /* [retval][out] */ IVxBufferGraph **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxMappingSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxMappingSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxMappingSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxMappingSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpans )( 
            IVxMappingSpan * This,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpans_2 )( 
            IVxMappingSpan * This,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetStart )( 
            IVxMappingSpan * This,
            /* [retval][out] */ IVxMappingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnd )( 
            IVxMappingSpan * This,
            /* [retval][out] */ IVxMappingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAnchorBuffer )( 
            IVxMappingSpan * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBufferGraph )( 
            IVxMappingSpan * This,
            /* [retval][out] */ IVxBufferGraph **ppRetVal);
        
        END_INTERFACE
    } IVxMappingSpanVtbl;

    interface IVxMappingSpan
    {
        CONST_VTBL struct IVxMappingSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxMappingSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxMappingSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxMappingSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxMappingSpan_GetSpans(This,pTargetBuffer,ppRetVal)	\
    ( (This)->lpVtbl -> GetSpans(This,pTargetBuffer,ppRetVal) ) 

#define IVxMappingSpan_GetSpans_2(This,pTargetSnapshot,ppRetVal)	\
    ( (This)->lpVtbl -> GetSpans_2(This,pTargetSnapshot,ppRetVal) ) 

#define IVxMappingSpan_GetStart(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetStart(This,ppRetVal) ) 

#define IVxMappingSpan_GetEnd(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnd(This,ppRetVal) ) 

#define IVxMappingSpan_GetAnchorBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAnchorBuffer(This,ppRetVal) ) 

#define IVxMappingSpan_GetBufferGraph(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBufferGraph(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxMappingSpan_INTERFACE_DEFINED__ */


#ifndef __IVxNormalizedTextChangeCollection_INTERFACE_DEFINED__
#define __IVxNormalizedTextChangeCollection_INTERFACE_DEFINED__

/* interface IVxNormalizedTextChangeCollection */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxNormalizedTextChangeCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("cab6d7ae-e6a1-411d-ac94-bdc1629bb621")
    IVxNormalizedTextChangeCollection : public IListIVxTextChange
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetIncludesLineChanges( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxNormalizedTextChangeCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxNormalizedTextChangeCollection * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxNormalizedTextChangeCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxNormalizedTextChangeCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVxNormalizedTextChangeCollection * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IVxNormalizedTextChangeCollection * This,
            /* [in] */ int index,
            /* [retval][out] */ IVxTextChange **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IVxNormalizedTextChangeCollection * This,
            /* [retval][out] */ IEnumeratorIVxTextChange **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetIncludesLineChanges )( 
            IVxNormalizedTextChangeCollection * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxNormalizedTextChangeCollectionVtbl;

    interface IVxNormalizedTextChangeCollection
    {
        CONST_VTBL struct IVxNormalizedTextChangeCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxNormalizedTextChangeCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxNormalizedTextChangeCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxNormalizedTextChangeCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxNormalizedTextChangeCollection_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IVxNormalizedTextChangeCollection_GetElement(This,index,ppRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,ppRetVal) ) 

#define IVxNormalizedTextChangeCollection_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 


#define IVxNormalizedTextChangeCollection_GetIncludesLineChanges(This,pRetVal)	\
    ( (This)->lpVtbl -> GetIncludesLineChanges(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxNormalizedTextChangeCollection_INTERFACE_DEFINED__ */


#ifndef __IVxReadOnlyRegion_INTERFACE_DEFINED__
#define __IVxReadOnlyRegion_INTERFACE_DEFINED__

/* interface IVxReadOnlyRegion */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxReadOnlyRegion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ece504c9-946e-4dd0-b929-0315736f86e4")
    IVxReadOnlyRegion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEdgeInsertionMode( 
            /* [retval][out] */ VxEdgeInsertionMode *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpan( 
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxReadOnlyRegionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxReadOnlyRegion * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxReadOnlyRegion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxReadOnlyRegion * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEdgeInsertionMode )( 
            IVxReadOnlyRegion * This,
            /* [retval][out] */ VxEdgeInsertionMode *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpan )( 
            IVxReadOnlyRegion * This,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        END_INTERFACE
    } IVxReadOnlyRegionVtbl;

    interface IVxReadOnlyRegion
    {
        CONST_VTBL struct IVxReadOnlyRegionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxReadOnlyRegion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxReadOnlyRegion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxReadOnlyRegion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxReadOnlyRegion_GetEdgeInsertionMode(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEdgeInsertionMode(This,pRetVal) ) 

#define IVxReadOnlyRegion_GetSpan(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSpan(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxReadOnlyRegion_INTERFACE_DEFINED__ */


#ifndef __IVxTextBufferEdit_INTERFACE_DEFINED__
#define __IVxTextBufferEdit_INTERFACE_DEFINED__

/* interface IVxTextBufferEdit */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextBufferEdit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ff1f08f7-7447-49e1-af8f-0a0929a21ac9")
    IVxTextBufferEdit : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSnapshot( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Apply( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Cancel( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCanceled( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextBufferEditVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextBufferEdit * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextBufferEdit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextBufferEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSnapshot )( 
            IVxTextBufferEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Apply )( 
            IVxTextBufferEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVxTextBufferEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanceled )( 
            IVxTextBufferEdit * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxTextBufferEditVtbl;

    interface IVxTextBufferEdit
    {
        CONST_VTBL struct IVxTextBufferEditVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextBufferEdit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextBufferEdit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextBufferEdit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextBufferEdit_GetSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSnapshot(This,ppRetVal) ) 

#define IVxTextBufferEdit_Apply(This,ppRetVal)	\
    ( (This)->lpVtbl -> Apply(This,ppRetVal) ) 

#define IVxTextBufferEdit_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#define IVxTextBufferEdit_GetCanceled(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCanceled(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextBufferEdit_INTERFACE_DEFINED__ */


#ifndef __IVxReadOnlyRegionEdit_INTERFACE_DEFINED__
#define __IVxReadOnlyRegionEdit_INTERFACE_DEFINED__

/* interface IVxReadOnlyRegionEdit */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxReadOnlyRegionEdit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("93a8521c-26ec-44fc-9a8b-3f7af2974eaa")
    IVxReadOnlyRegionEdit : public IVxTextBufferEdit
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateReadOnlyRegion( 
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxReadOnlyRegion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateReadOnlyRegion_2( 
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxEdgeInsertionMode edgeInsertionMode,
            /* [retval][out] */ IVxReadOnlyRegion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RemoveReadOnlyRegion( 
            /* [in] */ IVxReadOnlyRegion *pReadOnlyRegion) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxReadOnlyRegionEditVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxReadOnlyRegionEdit * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxReadOnlyRegionEdit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxReadOnlyRegionEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSnapshot )( 
            IVxReadOnlyRegionEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Apply )( 
            IVxReadOnlyRegionEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVxReadOnlyRegionEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanceled )( 
            IVxReadOnlyRegionEdit * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReadOnlyRegion )( 
            IVxReadOnlyRegionEdit * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxReadOnlyRegion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReadOnlyRegion_2 )( 
            IVxReadOnlyRegionEdit * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxEdgeInsertionMode edgeInsertionMode,
            /* [retval][out] */ IVxReadOnlyRegion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *RemoveReadOnlyRegion )( 
            IVxReadOnlyRegionEdit * This,
            /* [in] */ IVxReadOnlyRegion *pReadOnlyRegion);
        
        END_INTERFACE
    } IVxReadOnlyRegionEditVtbl;

    interface IVxReadOnlyRegionEdit
    {
        CONST_VTBL struct IVxReadOnlyRegionEditVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxReadOnlyRegionEdit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxReadOnlyRegionEdit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxReadOnlyRegionEdit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxReadOnlyRegionEdit_GetSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSnapshot(This,ppRetVal) ) 

#define IVxReadOnlyRegionEdit_Apply(This,ppRetVal)	\
    ( (This)->lpVtbl -> Apply(This,ppRetVal) ) 

#define IVxReadOnlyRegionEdit_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#define IVxReadOnlyRegionEdit_GetCanceled(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCanceled(This,pRetVal) ) 


#define IVxReadOnlyRegionEdit_CreateReadOnlyRegion(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> CreateReadOnlyRegion(This,span,ppRetVal) ) 

#define IVxReadOnlyRegionEdit_CreateReadOnlyRegion_2(This,span,trackingMode,edgeInsertionMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateReadOnlyRegion_2(This,span,trackingMode,edgeInsertionMode,ppRetVal) ) 

#define IVxReadOnlyRegionEdit_RemoveReadOnlyRegion(This,pReadOnlyRegion)	\
    ( (This)->lpVtbl -> RemoveReadOnlyRegion(This,pReadOnlyRegion) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxReadOnlyRegionEdit_INTERFACE_DEFINED__ */


#ifndef __IVxTextBuffer_INTERFACE_DEFINED__
#define __IVxTextBuffer_INTERFACE_DEFINED__

/* interface IVxTextBuffer */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextBuffer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("df9ab9af-f07e-43c5-9106-eaf5849760d2")
    IVxTextBuffer : public IVxPropertyOwner
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE IsReadOnly( 
            /* [in] */ VxSpan span,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReadOnly_2( 
            /* [in] */ VxSpan span,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReadOnlyExtents( 
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxNormalizedSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSnapshot( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateEdit( 
            /* [in] */ VxEditOptions options,
            /* [in] */ BOOL reiteratedVersionNumberHasValue,
            /* [in] */ int reiteratedVersionNumber,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxTextEdit **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateEdit_2( 
            /* [retval][out] */ IVxTextEdit **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateReadOnlyRegionEdit( 
            /* [retval][out] */ IVxReadOnlyRegionEdit **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditInProgress( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TakeThreadOwnership( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CheckEditAccess( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseReadOnlyRegionsChanged( 
            /* [in] */ IVxSnapshotSpanEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseReadOnlyRegionsChanged( 
            /* [in] */ IVxSnapshotSpanEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseChanged( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseChanged( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseChangedLowPriority( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseChangedLowPriority( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseChangedHighPriority( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseChangedHighPriority( 
            /* [in] */ IVxTextContentChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseChanging( 
            /* [in] */ IVxTextContentChangingEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseChanging( 
            /* [in] */ IVxTextContentChangingEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdvisePostChanged( 
            /* [in] */ IVxEventArgsEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadvisePostChanged( 
            /* [in] */ IVxEventArgsEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseContentTypeChanged( 
            /* [in] */ IVxContentTypeChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseContentTypeChanged( 
            /* [in] */ IVxContentTypeChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ChangeContentType( 
            /* [in] */ IVxContentType *pNewContentType,
            /* [in] */ IUnknown *pEditTag) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Insert( 
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete( 
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace( 
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReadOnly_3( 
            /* [in] */ int position,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsReadOnly_4( 
            /* [in] */ int position,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextBufferVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextBuffer * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextBuffer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperties )( 
            IVxTextBuffer * This,
            /* [retval][out] */ IVxPropertyCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IVxTextBuffer * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_2 )( 
            IVxTextBuffer * This,
            /* [in] */ VxSpan span,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetReadOnlyExtents )( 
            IVxTextBuffer * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxNormalizedSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxTextBuffer * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSnapshot )( 
            IVxTextBuffer * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit )( 
            IVxTextBuffer * This,
            /* [in] */ VxEditOptions options,
            /* [in] */ BOOL reiteratedVersionNumberHasValue,
            /* [in] */ int reiteratedVersionNumber,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit_2 )( 
            IVxTextBuffer * This,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReadOnlyRegionEdit )( 
            IVxTextBuffer * This,
            /* [retval][out] */ IVxReadOnlyRegionEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditInProgress )( 
            IVxTextBuffer * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *TakeThreadOwnership )( 
            IVxTextBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckEditAccess )( 
            IVxTextBuffer * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseReadOnlyRegionsChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseReadOnlyRegionsChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedLowPriority )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedLowPriority )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedHighPriority )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedHighPriority )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanging )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanging )( 
            IVxTextBuffer * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdvisePostChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadvisePostChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseContentTypeChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseContentTypeChanged )( 
            IVxTextBuffer * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *ChangeContentType )( 
            IVxTextBuffer * This,
            /* [in] */ IVxContentType *pNewContentType,
            /* [in] */ IUnknown *pEditTag);
        
        HRESULT ( STDMETHODCALLTYPE *Insert )( 
            IVxTextBuffer * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IVxTextBuffer * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVxTextBuffer * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_3 )( 
            IVxTextBuffer * This,
            /* [in] */ int position,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_4 )( 
            IVxTextBuffer * This,
            /* [in] */ int position,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxTextBufferVtbl;

    interface IVxTextBuffer
    {
        CONST_VTBL struct IVxTextBufferVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextBuffer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextBuffer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextBuffer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextBuffer_GetProperties(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetProperties(This,ppRetVal) ) 


#define IVxTextBuffer_IsReadOnly(This,span,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly(This,span,pRetVal) ) 

#define IVxTextBuffer_IsReadOnly_2(This,span,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_2(This,span,isEdit,pRetVal) ) 

#define IVxTextBuffer_GetReadOnlyExtents(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> GetReadOnlyExtents(This,span,ppRetVal) ) 

#define IVxTextBuffer_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#define IVxTextBuffer_GetCurrentSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrentSnapshot(This,ppRetVal) ) 

#define IVxTextBuffer_CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal) ) 

#define IVxTextBuffer_CreateEdit_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit_2(This,ppRetVal) ) 

#define IVxTextBuffer_CreateReadOnlyRegionEdit(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateReadOnlyRegionEdit(This,ppRetVal) ) 

#define IVxTextBuffer_GetEditInProgress(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEditInProgress(This,pRetVal) ) 

#define IVxTextBuffer_TakeThreadOwnership(This)	\
    ( (This)->lpVtbl -> TakeThreadOwnership(This) ) 

#define IVxTextBuffer_CheckEditAccess(This,pRetVal)	\
    ( (This)->lpVtbl -> CheckEditAccess(This,pRetVal) ) 

#define IVxTextBuffer_AdviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxTextBuffer_UnadviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxTextBuffer_AdviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanged(This,pValue) ) 

#define IVxTextBuffer_UnadviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanged(This,pValue) ) 

#define IVxTextBuffer_AdviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedLowPriority(This,pValue) ) 

#define IVxTextBuffer_UnadviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedLowPriority(This,pValue) ) 

#define IVxTextBuffer_AdviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedHighPriority(This,pValue) ) 

#define IVxTextBuffer_UnadviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedHighPriority(This,pValue) ) 

#define IVxTextBuffer_AdviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanging(This,pValue) ) 

#define IVxTextBuffer_UnadviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanging(This,pValue) ) 

#define IVxTextBuffer_AdvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdvisePostChanged(This,pValue) ) 

#define IVxTextBuffer_UnadvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadvisePostChanged(This,pValue) ) 

#define IVxTextBuffer_AdviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseContentTypeChanged(This,pValue) ) 

#define IVxTextBuffer_UnadviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseContentTypeChanged(This,pValue) ) 

#define IVxTextBuffer_ChangeContentType(This,pNewContentType,pEditTag)	\
    ( (This)->lpVtbl -> ChangeContentType(This,pNewContentType,pEditTag) ) 

#define IVxTextBuffer_Insert(This,position,text,ppRetVal)	\
    ( (This)->lpVtbl -> Insert(This,position,text,ppRetVal) ) 

#define IVxTextBuffer_Delete(This,deleteSpan,ppRetVal)	\
    ( (This)->lpVtbl -> Delete(This,deleteSpan,ppRetVal) ) 

#define IVxTextBuffer_Replace(This,replaceSpan,replaceWith,ppRetVal)	\
    ( (This)->lpVtbl -> Replace(This,replaceSpan,replaceWith,ppRetVal) ) 

#define IVxTextBuffer_IsReadOnly_3(This,position,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_3(This,position,pRetVal) ) 

#define IVxTextBuffer_IsReadOnly_4(This,position,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_4(This,position,isEdit,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextBuffer_INTERFACE_DEFINED__ */


#ifndef __IVxTextBufferFactoryService_INTERFACE_DEFINED__
#define __IVxTextBufferFactoryService_INTERFACE_DEFINED__

/* interface IVxTextBufferFactoryService */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextBufferFactoryService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a8f4abb4-783f-462b-b5ee-384e63e63f5b")
    IVxTextBufferFactoryService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPlaintextContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInertContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextBuffer_2( 
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextBuffer_3( 
            /* [in] */ BSTR text,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextBuffer_4( 
            /* [in] */ IUnknown *pReader,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseTextBufferCreated( 
            /* [in] */ IVxTextBufferCreatedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseTextBufferCreated( 
            /* [in] */ IVxTextBufferCreatedEvent *pValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextBufferFactoryServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextBufferFactoryService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextBufferFactoryService * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextContentType )( 
            IVxTextBufferFactoryService * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetPlaintextContentType )( 
            IVxTextBufferFactoryService * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetInertContentType )( 
            IVxTextBufferFactoryService * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextBuffer )( 
            IVxTextBufferFactoryService * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextBuffer_2 )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextBuffer_3 )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ BSTR text,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextBuffer_4 )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ IUnknown *pReader,
            /* [in] */ IVxContentType *pContentType,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseTextBufferCreated )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ IVxTextBufferCreatedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseTextBufferCreated )( 
            IVxTextBufferFactoryService * This,
            /* [in] */ IVxTextBufferCreatedEvent *pValue);
        
        END_INTERFACE
    } IVxTextBufferFactoryServiceVtbl;

    interface IVxTextBufferFactoryService
    {
        CONST_VTBL struct IVxTextBufferFactoryServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextBufferFactoryService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextBufferFactoryService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextBufferFactoryService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextBufferFactoryService_GetTextContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextContentType(This,ppRetVal) ) 

#define IVxTextBufferFactoryService_GetPlaintextContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetPlaintextContentType(This,ppRetVal) ) 

#define IVxTextBufferFactoryService_GetInertContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetInertContentType(This,ppRetVal) ) 

#define IVxTextBufferFactoryService_CreateTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextBuffer(This,ppRetVal) ) 

#define IVxTextBufferFactoryService_CreateTextBuffer_2(This,pContentType,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextBuffer_2(This,pContentType,ppRetVal) ) 

#define IVxTextBufferFactoryService_CreateTextBuffer_3(This,text,pContentType,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextBuffer_3(This,text,pContentType,ppRetVal) ) 

#define IVxTextBufferFactoryService_CreateTextBuffer_4(This,pReader,pContentType,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextBuffer_4(This,pReader,pContentType,ppRetVal) ) 

#define IVxTextBufferFactoryService_AdviseTextBufferCreated(This,pValue)	\
    ( (This)->lpVtbl -> AdviseTextBufferCreated(This,pValue) ) 

#define IVxTextBufferFactoryService_UnadviseTextBufferCreated(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseTextBufferCreated(This,pValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextBufferFactoryService_INTERFACE_DEFINED__ */


#ifndef __IVxTextChange_INTERFACE_DEFINED__
#define __IVxTextChange_INTERFACE_DEFINED__

/* interface IVxTextChange */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextChange;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9b3001b6-bc6d-4dfc-9b7e-0e6f2c776d8e")
    IVxTextChange : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetOldSpan( 
            /* [retval][out] */ VxSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNewSpan( 
            /* [retval][out] */ VxSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOldPosition( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNewPosition( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDelta( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOldEnd( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNewEnd( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOldText( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNewText( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOldLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNewLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineCountDelta( 
            /* [retval][out] */ int *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextChangeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextChange * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextChange * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextChange * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetOldSpan )( 
            IVxTextChange * This,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewSpan )( 
            IVxTextChange * This,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOldPosition )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewPosition )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetDelta )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOldEnd )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewEnd )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOldText )( 
            IVxTextChange * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewText )( 
            IVxTextChange * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOldLength )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetNewLength )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCountDelta )( 
            IVxTextChange * This,
            /* [retval][out] */ int *pRetVal);
        
        END_INTERFACE
    } IVxTextChangeVtbl;

    interface IVxTextChange
    {
        CONST_VTBL struct IVxTextChangeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextChange_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextChange_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextChange_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextChange_GetOldSpan(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOldSpan(This,pRetVal) ) 

#define IVxTextChange_GetNewSpan(This,pRetVal)	\
    ( (This)->lpVtbl -> GetNewSpan(This,pRetVal) ) 

#define IVxTextChange_GetOldPosition(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOldPosition(This,pRetVal) ) 

#define IVxTextChange_GetNewPosition(This,pRetVal)	\
    ( (This)->lpVtbl -> GetNewPosition(This,pRetVal) ) 

#define IVxTextChange_GetDelta(This,pRetVal)	\
    ( (This)->lpVtbl -> GetDelta(This,pRetVal) ) 

#define IVxTextChange_GetOldEnd(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOldEnd(This,pRetVal) ) 

#define IVxTextChange_GetNewEnd(This,pRetVal)	\
    ( (This)->lpVtbl -> GetNewEnd(This,pRetVal) ) 

#define IVxTextChange_GetOldText(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOldText(This,pRetVal) ) 

#define IVxTextChange_GetNewText(This,pRetVal)	\
    ( (This)->lpVtbl -> GetNewText(This,pRetVal) ) 

#define IVxTextChange_GetOldLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOldLength(This,pRetVal) ) 

#define IVxTextChange_GetNewLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetNewLength(This,pRetVal) ) 

#define IVxTextChange_GetLineCountDelta(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineCountDelta(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextChange_INTERFACE_DEFINED__ */


#ifndef __IVxTextEdit_INTERFACE_DEFINED__
#define __IVxTextEdit_INTERFACE_DEFINED__

/* interface IVxTextEdit */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextEdit;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("aaaa0268-3374-4d81-8c09-c130688ab1e8")
    IVxTextEdit : public IVxTextBufferEdit
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Insert( 
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete( 
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete_2( 
            /* [in] */ int startPosition,
            /* [in] */ int charsToDelete,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace( 
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace_2( 
            /* [in] */ int startPosition,
            /* [in] */ int charsToReplace,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHasEffectiveChanges( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHasFailedChanges( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextEditVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextEdit * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextEdit * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSnapshot )( 
            IVxTextEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Apply )( 
            IVxTextEdit * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVxTextEdit * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanceled )( 
            IVxTextEdit * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Insert )( 
            IVxTextEdit * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IVxTextEdit * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete_2 )( 
            IVxTextEdit * This,
            /* [in] */ int startPosition,
            /* [in] */ int charsToDelete,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVxTextEdit * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace_2 )( 
            IVxTextEdit * This,
            /* [in] */ int startPosition,
            /* [in] */ int charsToReplace,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetHasEffectiveChanges )( 
            IVxTextEdit * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetHasFailedChanges )( 
            IVxTextEdit * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxTextEditVtbl;

    interface IVxTextEdit
    {
        CONST_VTBL struct IVxTextEditVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextEdit_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextEdit_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextEdit_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextEdit_GetSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSnapshot(This,ppRetVal) ) 

#define IVxTextEdit_Apply(This,ppRetVal)	\
    ( (This)->lpVtbl -> Apply(This,ppRetVal) ) 

#define IVxTextEdit_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#define IVxTextEdit_GetCanceled(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCanceled(This,pRetVal) ) 


#define IVxTextEdit_Insert(This,position,text,pRetVal)	\
    ( (This)->lpVtbl -> Insert(This,position,text,pRetVal) ) 

#define IVxTextEdit_Delete(This,deleteSpan,pRetVal)	\
    ( (This)->lpVtbl -> Delete(This,deleteSpan,pRetVal) ) 

#define IVxTextEdit_Delete_2(This,startPosition,charsToDelete,pRetVal)	\
    ( (This)->lpVtbl -> Delete_2(This,startPosition,charsToDelete,pRetVal) ) 

#define IVxTextEdit_Replace(This,replaceSpan,replaceWith,pRetVal)	\
    ( (This)->lpVtbl -> Replace(This,replaceSpan,replaceWith,pRetVal) ) 

#define IVxTextEdit_Replace_2(This,startPosition,charsToReplace,replaceWith,pRetVal)	\
    ( (This)->lpVtbl -> Replace_2(This,startPosition,charsToReplace,replaceWith,pRetVal) ) 

#define IVxTextEdit_GetHasEffectiveChanges(This,pRetVal)	\
    ( (This)->lpVtbl -> GetHasEffectiveChanges(This,pRetVal) ) 

#define IVxTextEdit_GetHasFailedChanges(This,pRetVal)	\
    ( (This)->lpVtbl -> GetHasFailedChanges(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextEdit_INTERFACE_DEFINED__ */


#ifndef __IVxTextSnapshot_INTERFACE_DEFINED__
#define __IVxTextSnapshot_INTERFACE_DEFINED__

/* interface IVxTextSnapshot */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextSnapshot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f989d547-f569-495c-819b-9810e2bb6dd3")
    IVxTextSnapshot : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVersion( 
            /* [retval][out] */ IVxTextVersion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ VxSpan span,
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText_2( 
            /* [in] */ int startIndex,
            /* [in] */ int length,
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText_3( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingPoint( 
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingPoint_2( 
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan( 
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_2( 
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_3( 
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_4( 
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineFromLineNumber( 
            /* [in] */ int lineNumber,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineFromPosition( 
            /* [in] */ int position,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineNumberFromPosition( 
            /* [in] */ int position,
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLines( 
            /* [retval][out] */ IEnumerableIVxTextSnapshotLine **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Write( 
            /* [in] */ IUnknown *pWriter,
            /* [in] */ VxSpan span) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Write_2( 
            /* [in] */ IUnknown *pWriter) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextSnapshotVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextSnapshot * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextSnapshot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextSnapshot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetVersion )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLength )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVxTextSnapshot * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText_2 )( 
            IVxTextSnapshot * This,
            /* [in] */ int startIndex,
            /* [in] */ int length,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText_3 )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint )( 
            IVxTextSnapshot * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint_2 )( 
            IVxTextSnapshot * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan )( 
            IVxTextSnapshot * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_2 )( 
            IVxTextSnapshot * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_3 )( 
            IVxTextSnapshot * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_4 )( 
            IVxTextSnapshot * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineFromLineNumber )( 
            IVxTextSnapshot * This,
            /* [in] */ int lineNumber,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineFromPosition )( 
            IVxTextSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineNumberFromPosition )( 
            IVxTextSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLines )( 
            IVxTextSnapshot * This,
            /* [retval][out] */ IEnumerableIVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Write )( 
            IVxTextSnapshot * This,
            /* [in] */ IUnknown *pWriter,
            /* [in] */ VxSpan span);
        
        HRESULT ( STDMETHODCALLTYPE *Write_2 )( 
            IVxTextSnapshot * This,
            /* [in] */ IUnknown *pWriter);
        
        END_INTERFACE
    } IVxTextSnapshotVtbl;

    interface IVxTextSnapshot
    {
        CONST_VTBL struct IVxTextSnapshotVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextSnapshot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextSnapshot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextSnapshot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextSnapshot_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxTextSnapshot_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#define IVxTextSnapshot_GetVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetVersion(This,ppRetVal) ) 

#define IVxTextSnapshot_GetLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLength(This,pRetVal) ) 

#define IVxTextSnapshot_GetLineCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineCount(This,pRetVal) ) 

#define IVxTextSnapshot_GetText(This,span,pRetVal)	\
    ( (This)->lpVtbl -> GetText(This,span,pRetVal) ) 

#define IVxTextSnapshot_GetText_2(This,startIndex,length,pRetVal)	\
    ( (This)->lpVtbl -> GetText_2(This,startIndex,length,pRetVal) ) 

#define IVxTextSnapshot_GetText_3(This,pRetVal)	\
    ( (This)->lpVtbl -> GetText_3(This,pRetVal) ) 

#define IVxTextSnapshot_CreateTrackingPoint(This,position,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint(This,position,trackingMode,ppRetVal) ) 

#define IVxTextSnapshot_CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextSnapshot_CreateTrackingSpan(This,span,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan(This,span,trackingMode,ppRetVal) ) 

#define IVxTextSnapshot_CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextSnapshot_CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal) ) 

#define IVxTextSnapshot_CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextSnapshot_GetLineFromLineNumber(This,lineNumber,ppRetVal)	\
    ( (This)->lpVtbl -> GetLineFromLineNumber(This,lineNumber,ppRetVal) ) 

#define IVxTextSnapshot_GetLineFromPosition(This,position,ppRetVal)	\
    ( (This)->lpVtbl -> GetLineFromPosition(This,position,ppRetVal) ) 

#define IVxTextSnapshot_GetLineNumberFromPosition(This,position,pRetVal)	\
    ( (This)->lpVtbl -> GetLineNumberFromPosition(This,position,pRetVal) ) 

#define IVxTextSnapshot_GetLines(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetLines(This,ppRetVal) ) 

#define IVxTextSnapshot_Write(This,pWriter,span)	\
    ( (This)->lpVtbl -> Write(This,pWriter,span) ) 

#define IVxTextSnapshot_Write_2(This,pWriter)	\
    ( (This)->lpVtbl -> Write_2(This,pWriter) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextSnapshot_INTERFACE_DEFINED__ */


#ifndef __IVxTextSnapshotLine_INTERFACE_DEFINED__
#define __IVxTextSnapshotLine_INTERFACE_DEFINED__

/* interface IVxTextSnapshotLine */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextSnapshotLine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7b15e2bd-277b-42e8-869f-71b9fb79f962")
    IVxTextSnapshotLine : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSnapshot( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtent( 
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetExtentIncludingLineBreak( 
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineNumber( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStart( 
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLengthIncludingLineBreak( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEnd( 
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEndIncludingLineBreak( 
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineBreakLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextIncludingLineBreak( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLineBreakText( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextSnapshotLineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextSnapshotLine * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextSnapshotLine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextSnapshotLine * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSnapshot )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtent )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtentIncludingLineBreak )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineNumber )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetStart )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLength )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLengthIncludingLineBreak )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnd )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEndIncludingLineBreak )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineBreakLength )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextIncludingLineBreak )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineBreakText )( 
            IVxTextSnapshotLine * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        END_INTERFACE
    } IVxTextSnapshotLineVtbl;

    interface IVxTextSnapshotLine
    {
        CONST_VTBL struct IVxTextSnapshotLineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextSnapshotLine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextSnapshotLine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextSnapshotLine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextSnapshotLine_GetSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSnapshot(This,ppRetVal) ) 

#define IVxTextSnapshotLine_GetExtent(This,pRetVal)	\
    ( (This)->lpVtbl -> GetExtent(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetExtentIncludingLineBreak(This,pRetVal)	\
    ( (This)->lpVtbl -> GetExtentIncludingLineBreak(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetLineNumber(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineNumber(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetStart(This,pRetVal)	\
    ( (This)->lpVtbl -> GetStart(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLength(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetLengthIncludingLineBreak(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLengthIncludingLineBreak(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetEnd(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEnd(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetEndIncludingLineBreak(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEndIncludingLineBreak(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetLineBreakLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineBreakLength(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetText(This,pRetVal)	\
    ( (This)->lpVtbl -> GetText(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetTextIncludingLineBreak(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTextIncludingLineBreak(This,pRetVal) ) 

#define IVxTextSnapshotLine_GetLineBreakText(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineBreakText(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextSnapshotLine_INTERFACE_DEFINED__ */


#ifndef __IVxTextVersion_INTERFACE_DEFINED__
#define __IVxTextVersion_INTERFACE_DEFINED__

/* interface IVxTextVersion */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextVersion;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("dc22e98b-dee1-4836-8fcc-ae137a47b89e")
    IVxTextVersion : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNext( 
            /* [retval][out] */ IVxTextVersion **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLength( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetChanges( 
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingPoint( 
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingPoint_2( 
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan( 
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_2( 
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_3( 
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTrackingSpan_4( 
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVersionNumber( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetReiteratedVersionNumber( 
            /* [retval][out] */ int *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextVersionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextVersion * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextVersion * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextVersion * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNext )( 
            IVxTextVersion * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLength )( 
            IVxTextVersion * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetChanges )( 
            IVxTextVersion * This,
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint )( 
            IVxTextVersion * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint_2 )( 
            IVxTextVersion * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan )( 
            IVxTextVersion * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_2 )( 
            IVxTextVersion * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_3 )( 
            IVxTextVersion * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_4 )( 
            IVxTextVersion * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTextVersion * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetVersionNumber )( 
            IVxTextVersion * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetReiteratedVersionNumber )( 
            IVxTextVersion * This,
            /* [retval][out] */ int *pRetVal);
        
        END_INTERFACE
    } IVxTextVersionVtbl;

    interface IVxTextVersion
    {
        CONST_VTBL struct IVxTextVersionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextVersion_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextVersion_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextVersion_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextVersion_GetNext(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetNext(This,ppRetVal) ) 

#define IVxTextVersion_GetLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLength(This,pRetVal) ) 

#define IVxTextVersion_GetChanges(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetChanges(This,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingPoint(This,position,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint(This,position,trackingMode,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingSpan(This,span,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan(This,span,trackingMode,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal) ) 

#define IVxTextVersion_CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxTextVersion_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxTextVersion_GetVersionNumber(This,pRetVal)	\
    ( (This)->lpVtbl -> GetVersionNumber(This,pRetVal) ) 

#define IVxTextVersion_GetReiteratedVersionNumber(This,pRetVal)	\
    ( (This)->lpVtbl -> GetReiteratedVersionNumber(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextVersion_INTERFACE_DEFINED__ */


#ifndef __IVxTrackingPoint_INTERFACE_DEFINED__
#define __IVxTrackingPoint_INTERFACE_DEFINED__

/* interface IVxTrackingPoint */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTrackingPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d59ac777-8bb7-4634-a566-2d812970f5f6")
    IVxTrackingPoint : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTrackingMode( 
            /* [retval][out] */ VxPointTrackingMode *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTrackingFidelity( 
            /* [retval][out] */ VxTrackingFidelityMode *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPoint( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPosition( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPosition_2( 
            /* [in] */ IVxTextVersion *pVersion,
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCharacter( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ USHORT *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTrackingPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTrackingPoint * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTrackingPoint * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTrackingPoint * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTrackingPoint * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrackingMode )( 
            IVxTrackingPoint * This,
            /* [retval][out] */ VxPointTrackingMode *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrackingFidelity )( 
            IVxTrackingPoint * This,
            /* [retval][out] */ VxTrackingFidelityMode *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetPoint )( 
            IVxTrackingPoint * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetPosition )( 
            IVxTrackingPoint * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetPosition_2 )( 
            IVxTrackingPoint * This,
            /* [in] */ IVxTextVersion *pVersion,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCharacter )( 
            IVxTrackingPoint * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ USHORT *pRetVal);
        
        END_INTERFACE
    } IVxTrackingPointVtbl;

    interface IVxTrackingPoint
    {
        CONST_VTBL struct IVxTrackingPointVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTrackingPoint_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTrackingPoint_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTrackingPoint_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTrackingPoint_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxTrackingPoint_GetTrackingMode(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTrackingMode(This,pRetVal) ) 

#define IVxTrackingPoint_GetTrackingFidelity(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTrackingFidelity(This,pRetVal) ) 

#define IVxTrackingPoint_GetPoint(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetPoint(This,pSnapshot,pRetVal) ) 

#define IVxTrackingPoint_GetPosition(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetPosition(This,pSnapshot,pRetVal) ) 

#define IVxTrackingPoint_GetPosition_2(This,pVersion,pRetVal)	\
    ( (This)->lpVtbl -> GetPosition_2(This,pVersion,pRetVal) ) 

#define IVxTrackingPoint_GetCharacter(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetCharacter(This,pSnapshot,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTrackingPoint_INTERFACE_DEFINED__ */


#ifndef __IVxTrackingSpan_INTERFACE_DEFINED__
#define __IVxTrackingSpan_INTERFACE_DEFINED__

/* interface IVxTrackingSpan */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTrackingSpan;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ed17a9a-cc3f-4c10-aefb-92a19fee5abc")
    IVxTrackingSpan : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTrackingMode( 
            /* [retval][out] */ VxSpanTrackingMode *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTrackingFidelity( 
            /* [retval][out] */ VxTrackingFidelityMode *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpan( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpan_2( 
            /* [in] */ IVxTextVersion *pVersion,
            /* [retval][out] */ VxSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetText( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStartPoint( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEndPoint( 
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTrackingSpanVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTrackingSpan * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTrackingSpan * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTrackingSpan * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTrackingSpan * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrackingMode )( 
            IVxTrackingSpan * This,
            /* [retval][out] */ VxSpanTrackingMode *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetTrackingFidelity )( 
            IVxTrackingSpan * This,
            /* [retval][out] */ VxTrackingFidelityMode *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpan )( 
            IVxTrackingSpan * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpan_2 )( 
            IVxTrackingSpan * This,
            /* [in] */ IVxTextVersion *pVersion,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVxTrackingSpan * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetStartPoint )( 
            IVxTrackingSpan * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEndPoint )( 
            IVxTrackingSpan * This,
            /* [in] */ IVxTextSnapshot *pSnapshot,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        END_INTERFACE
    } IVxTrackingSpanVtbl;

    interface IVxTrackingSpan
    {
        CONST_VTBL struct IVxTrackingSpanVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTrackingSpan_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTrackingSpan_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTrackingSpan_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTrackingSpan_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxTrackingSpan_GetTrackingMode(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTrackingMode(This,pRetVal) ) 

#define IVxTrackingSpan_GetTrackingFidelity(This,pRetVal)	\
    ( (This)->lpVtbl -> GetTrackingFidelity(This,pRetVal) ) 

#define IVxTrackingSpan_GetSpan(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetSpan(This,pSnapshot,pRetVal) ) 

#define IVxTrackingSpan_GetSpan_2(This,pVersion,pRetVal)	\
    ( (This)->lpVtbl -> GetSpan_2(This,pVersion,pRetVal) ) 

#define IVxTrackingSpan_GetText(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetText(This,pSnapshot,pRetVal) ) 

#define IVxTrackingSpan_GetStartPoint(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetStartPoint(This,pSnapshot,pRetVal) ) 

#define IVxTrackingSpan_GetEndPoint(This,pSnapshot,pRetVal)	\
    ( (This)->lpVtbl -> GetEndPoint(This,pSnapshot,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTrackingSpan_INTERFACE_DEFINED__ */


#ifndef __IVxNormalizedSnapshotSpanCollection_INTERFACE_DEFINED__
#define __IVxNormalizedSnapshotSpanCollection_INTERFACE_DEFINED__

/* interface IVxNormalizedSnapshotSpanCollection */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxNormalizedSnapshotSpanCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("df81e2df-b253-4859-9429-bb9712cf7f2b")
    IVxNormalizedSnapshotSpanCollection : public IListVxSnapshotSpan
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OverlapsWith( 
            /* [in] */ IVxNormalizedSnapshotSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IntersectsWith( 
            /* [in] */ IVxNormalizedSnapshotSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxNormalizedSnapshotSpanCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxNormalizedSnapshotSpanCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxNormalizedSnapshotSpanCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [in] */ int index,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [retval][out] */ IEnumeratorVxSnapshotSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *OverlapsWith )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [in] */ IVxNormalizedSnapshotSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IntersectsWith )( 
            IVxNormalizedSnapshotSpanCollection * This,
            /* [in] */ IVxNormalizedSnapshotSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal);
        
        END_INTERFACE
    } IVxNormalizedSnapshotSpanCollectionVtbl;

    interface IVxNormalizedSnapshotSpanCollection
    {
        CONST_VTBL struct IVxNormalizedSnapshotSpanCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxNormalizedSnapshotSpanCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxNormalizedSnapshotSpanCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxNormalizedSnapshotSpanCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxNormalizedSnapshotSpanCollection_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IVxNormalizedSnapshotSpanCollection_GetElement(This,index,pRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,pRetVal) ) 

#define IVxNormalizedSnapshotSpanCollection_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 


#define IVxNormalizedSnapshotSpanCollection_OverlapsWith(This,pSet,pRetVal)	\
    ( (This)->lpVtbl -> OverlapsWith(This,pSet,pRetVal) ) 

#define IVxNormalizedSnapshotSpanCollection_IntersectsWith(This,pSet,pRetVal)	\
    ( (This)->lpVtbl -> IntersectsWith(This,pSet,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxNormalizedSnapshotSpanCollection_INTERFACE_DEFINED__ */


#ifndef __IVxNormalizedSpanCollection_INTERFACE_DEFINED__
#define __IVxNormalizedSpanCollection_INTERFACE_DEFINED__

/* interface IVxNormalizedSpanCollection */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxNormalizedSpanCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f12c3b51-b3c2-42ea-86b0-6185c6f712a8")
    IVxNormalizedSpanCollection : public IListVxSpan
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OverlapsWith( 
            /* [in] */ IVxNormalizedSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IntersectsWith( 
            /* [in] */ IVxNormalizedSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHashCode( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Equals( 
            /* [in] */ IUnknown *pObj,
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ToString( 
            /* [retval][out] */ BSTR *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxNormalizedSpanCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxNormalizedSpanCollection * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxNormalizedSpanCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxNormalizedSpanCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCount )( 
            IVxNormalizedSpanCollection * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetElement )( 
            IVxNormalizedSpanCollection * This,
            /* [in] */ int index,
            /* [retval][out] */ VxSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEnumerator )( 
            IVxNormalizedSpanCollection * This,
            /* [retval][out] */ IEnumeratorVxSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *OverlapsWith )( 
            IVxNormalizedSpanCollection * This,
            /* [in] */ IVxNormalizedSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IntersectsWith )( 
            IVxNormalizedSpanCollection * This,
            /* [in] */ IVxNormalizedSpanCollection *pSet,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetHashCode )( 
            IVxNormalizedSpanCollection * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Equals )( 
            IVxNormalizedSpanCollection * This,
            /* [in] */ IUnknown *pObj,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *ToString )( 
            IVxNormalizedSpanCollection * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        END_INTERFACE
    } IVxNormalizedSpanCollectionVtbl;

    interface IVxNormalizedSpanCollection
    {
        CONST_VTBL struct IVxNormalizedSpanCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxNormalizedSpanCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxNormalizedSpanCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxNormalizedSpanCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxNormalizedSpanCollection_GetCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCount(This,pRetVal) ) 

#define IVxNormalizedSpanCollection_GetElement(This,index,pRetVal)	\
    ( (This)->lpVtbl -> GetElement(This,index,pRetVal) ) 

#define IVxNormalizedSpanCollection_GetEnumerator(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEnumerator(This,ppRetVal) ) 


#define IVxNormalizedSpanCollection_OverlapsWith(This,pSet,pRetVal)	\
    ( (This)->lpVtbl -> OverlapsWith(This,pSet,pRetVal) ) 

#define IVxNormalizedSpanCollection_IntersectsWith(This,pSet,pRetVal)	\
    ( (This)->lpVtbl -> IntersectsWith(This,pSet,pRetVal) ) 

#define IVxNormalizedSpanCollection_GetHashCode(This,pRetVal)	\
    ( (This)->lpVtbl -> GetHashCode(This,pRetVal) ) 

#define IVxNormalizedSpanCollection_Equals(This,pObj,pRetVal)	\
    ( (This)->lpVtbl -> Equals(This,pObj,pRetVal) ) 

#define IVxNormalizedSpanCollection_ToString(This,pRetVal)	\
    ( (This)->lpVtbl -> ToString(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxNormalizedSpanCollection_INTERFACE_DEFINED__ */


#ifndef __IVxTextContentChangedEventArgs_INTERFACE_DEFINED__
#define __IVxTextContentChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxTextContentChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextContentChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0fe2e653-3d0b-43db-b76f-12c5e04db756")
    IVxTextContentChangedEventArgs : public IVxTextSnapshotChangedEventArgs
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetChanges( 
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetOptions( 
            /* [retval][out] */ VxEditOptions *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextContentChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextContentChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextContentChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextContentChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterVersion )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetChanges )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOptions )( 
            IVxTextContentChangedEventArgs * This,
            /* [retval][out] */ VxEditOptions *pRetVal);
        
        END_INTERFACE
    } IVxTextContentChangedEventArgsVtbl;

    interface IVxTextContentChangedEventArgs
    {
        CONST_VTBL struct IVxTextContentChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextContentChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextContentChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextContentChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextContentChangedEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxTextContentChangedEventArgs_GetAfter(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter(This,ppRetVal) ) 

#define IVxTextContentChangedEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#define IVxTextContentChangedEventArgs_GetAfterVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterVersion(This,ppRetVal) ) 

#define IVxTextContentChangedEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 


#define IVxTextContentChangedEventArgs_GetChanges(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetChanges(This,ppRetVal) ) 

#define IVxTextContentChangedEventArgs_GetOptions(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOptions(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextContentChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxGraphBufferContentTypeChangedEventArgs_INTERFACE_DEFINED__
#define __IVxGraphBufferContentTypeChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxGraphBufferContentTypeChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxGraphBufferContentTypeChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("020c7be6-00fe-43e1-ba03-e62fb81eb8f9")
    IVxGraphBufferContentTypeChangedEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBeforeContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAfterContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxGraphBufferContentTypeChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxGraphBufferContentTypeChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxGraphBufferContentTypeChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxGraphBufferContentTypeChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxGraphBufferContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeContentType )( 
            IVxGraphBufferContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterContentType )( 
            IVxGraphBufferContentTypeChangedEventArgs * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        END_INTERFACE
    } IVxGraphBufferContentTypeChangedEventArgsVtbl;

    interface IVxGraphBufferContentTypeChangedEventArgs
    {
        CONST_VTBL struct IVxGraphBufferContentTypeChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxGraphBufferContentTypeChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxGraphBufferContentTypeChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxGraphBufferContentTypeChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxGraphBufferContentTypeChangedEventArgs_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxGraphBufferContentTypeChangedEventArgs_GetBeforeContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeContentType(This,ppRetVal) ) 

#define IVxGraphBufferContentTypeChangedEventArgs_GetAfterContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterContentType(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxGraphBufferContentTypeChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxGraphBuffersChangedEventArgs_INTERFACE_DEFINED__
#define __IVxGraphBuffersChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxGraphBuffersChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxGraphBuffersChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("395bd869-ffa5-4eea-97bf-8ce42a7b3fcd")
    IVxGraphBuffersChangedEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAddedBuffers( 
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemovedBuffers( 
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxGraphBuffersChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxGraphBuffersChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxGraphBuffersChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxGraphBuffersChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetAddedBuffers )( 
            IVxGraphBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemovedBuffers )( 
            IVxGraphBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        END_INTERFACE
    } IVxGraphBuffersChangedEventArgsVtbl;

    interface IVxGraphBuffersChangedEventArgs
    {
        CONST_VTBL struct IVxGraphBuffersChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxGraphBuffersChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxGraphBuffersChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxGraphBuffersChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxGraphBuffersChangedEventArgs_GetAddedBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAddedBuffers(This,ppRetVal) ) 

#define IVxGraphBuffersChangedEventArgs_GetRemovedBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetRemovedBuffers(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxGraphBuffersChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxBufferGraph_INTERFACE_DEFINED__
#define __IVxBufferGraph_INTERFACE_DEFINED__

/* interface IVxBufferGraph */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxBufferGraph;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("125ae4f6-7cca-4756-a175-a2ca5e7f60da")
    IVxBufferGraph : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTopBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateMappingPoint( 
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxMappingPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateMappingSpan( 
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxMappingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapDownToBuffer( 
            /* [in] */ VxSnapshotPoint position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapDownToSnapshot( 
            /* [in] */ VxSnapshotPoint position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapDownToBuffer_2( 
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapDownToSnapshot_2( 
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapUpToBuffer( 
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxPositionAffinity affinity,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapUpToSnapshot( 
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxPositionAffinity affinity,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapUpToBuffer_2( 
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapUpToSnapshot_2( 
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseGraphBuffersChanged( 
            /* [in] */ IVxGraphBuffersChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseGraphBuffersChanged( 
            /* [in] */ IVxGraphBuffersChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseGraphBufferContentTypeChanged( 
            /* [in] */ IVxGraphBufferContentTypeChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseGraphBufferContentTypeChanged( 
            /* [in] */ IVxGraphBufferContentTypeChangedEvent *pValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxBufferGraphVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxBufferGraph * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxBufferGraph * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxBufferGraph * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTopBuffer )( 
            IVxBufferGraph * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateMappingPoint )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxMappingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateMappingSpan )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxMappingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapDownToBuffer )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotPoint position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapDownToSnapshot )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotPoint position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapDownToBuffer_2 )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapDownToSnapshot_2 )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapUpToBuffer )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxPositionAffinity affinity,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapUpToSnapshot )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxPositionAffinity affinity,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapUpToBuffer_2 )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextBuffer *pTargetBuffer,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapUpToSnapshot_2 )( 
            IVxBufferGraph * This,
            /* [in] */ VxSnapshotSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ IVxTextSnapshot *pTargetSnapshot,
            /* [retval][out] */ IVxNormalizedSnapshotSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseGraphBuffersChanged )( 
            IVxBufferGraph * This,
            /* [in] */ IVxGraphBuffersChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseGraphBuffersChanged )( 
            IVxBufferGraph * This,
            /* [in] */ IVxGraphBuffersChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseGraphBufferContentTypeChanged )( 
            IVxBufferGraph * This,
            /* [in] */ IVxGraphBufferContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseGraphBufferContentTypeChanged )( 
            IVxBufferGraph * This,
            /* [in] */ IVxGraphBufferContentTypeChangedEvent *pValue);
        
        END_INTERFACE
    } IVxBufferGraphVtbl;

    interface IVxBufferGraph
    {
        CONST_VTBL struct IVxBufferGraphVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxBufferGraph_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxBufferGraph_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxBufferGraph_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxBufferGraph_GetTopBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTopBuffer(This,ppRetVal) ) 

#define IVxBufferGraph_CreateMappingPoint(This,point,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateMappingPoint(This,point,trackingMode,ppRetVal) ) 

#define IVxBufferGraph_CreateMappingSpan(This,span,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateMappingSpan(This,span,trackingMode,ppRetVal) ) 

#define IVxBufferGraph_MapDownToBuffer(This,position,trackingMode,pTargetBuffer,affinity,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> MapDownToBuffer(This,position,trackingMode,pTargetBuffer,affinity,pRetValHasValue,pRetVal) ) 

#define IVxBufferGraph_MapDownToSnapshot(This,position,trackingMode,pTargetSnapshot,affinity,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> MapDownToSnapshot(This,position,trackingMode,pTargetSnapshot,affinity,pRetValHasValue,pRetVal) ) 

#define IVxBufferGraph_MapDownToBuffer_2(This,span,trackingMode,pTargetBuffer,ppRetVal)	\
    ( (This)->lpVtbl -> MapDownToBuffer_2(This,span,trackingMode,pTargetBuffer,ppRetVal) ) 

#define IVxBufferGraph_MapDownToSnapshot_2(This,span,trackingMode,pTargetSnapshot,ppRetVal)	\
    ( (This)->lpVtbl -> MapDownToSnapshot_2(This,span,trackingMode,pTargetSnapshot,ppRetVal) ) 

#define IVxBufferGraph_MapUpToBuffer(This,point,trackingMode,affinity,pTargetBuffer,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> MapUpToBuffer(This,point,trackingMode,affinity,pTargetBuffer,pRetValHasValue,pRetVal) ) 

#define IVxBufferGraph_MapUpToSnapshot(This,point,trackingMode,affinity,pTargetSnapshot,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> MapUpToSnapshot(This,point,trackingMode,affinity,pTargetSnapshot,pRetValHasValue,pRetVal) ) 

#define IVxBufferGraph_MapUpToBuffer_2(This,span,trackingMode,pTargetBuffer,ppRetVal)	\
    ( (This)->lpVtbl -> MapUpToBuffer_2(This,span,trackingMode,pTargetBuffer,ppRetVal) ) 

#define IVxBufferGraph_MapUpToSnapshot_2(This,span,trackingMode,pTargetSnapshot,ppRetVal)	\
    ( (This)->lpVtbl -> MapUpToSnapshot_2(This,span,trackingMode,pTargetSnapshot,ppRetVal) ) 

#define IVxBufferGraph_AdviseGraphBuffersChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseGraphBuffersChanged(This,pValue) ) 

#define IVxBufferGraph_UnadviseGraphBuffersChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseGraphBuffersChanged(This,pValue) ) 

#define IVxBufferGraph_AdviseGraphBufferContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseGraphBufferContentTypeChanged(This,pValue) ) 

#define IVxBufferGraph_UnadviseGraphBufferContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseGraphBufferContentTypeChanged(This,pValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxBufferGraph_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionBufferBase_INTERFACE_DEFINED__
#define __IVxProjectionBufferBase_INTERFACE_DEFINED__

/* interface IVxProjectionBufferBase */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionBufferBase;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("dc35621d-2318-46d3-88cd-ca026f95b9c1")
    IVxProjectionBufferBase : public IVxTextBuffer
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSnapshot_2( 
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceBuffers( 
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Insert_2( 
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Delete_2( 
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Replace_2( 
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionBufferBaseVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionBufferBase * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionBufferBase * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionBufferBase * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperties )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxPropertyCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_2 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan span,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetReadOnlyExtents )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxNormalizedSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSnapshot )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxEditOptions options,
            /* [in] */ BOOL reiteratedVersionNumberHasValue,
            /* [in] */ int reiteratedVersionNumber,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit_2 )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReadOnlyRegionEdit )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxReadOnlyRegionEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditInProgress )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *TakeThreadOwnership )( 
            IVxProjectionBufferBase * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckEditAccess )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseReadOnlyRegionsChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseReadOnlyRegionsChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedLowPriority )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedLowPriority )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedHighPriority )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedHighPriority )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanging )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanging )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdvisePostChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadvisePostChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseContentTypeChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseContentTypeChanged )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *ChangeContentType )( 
            IVxProjectionBufferBase * This,
            /* [in] */ IVxContentType *pNewContentType,
            /* [in] */ IUnknown *pEditTag);
        
        HRESULT ( STDMETHODCALLTYPE *Insert )( 
            IVxProjectionBufferBase * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_3 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ int position,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_4 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ int position,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSnapshot_2 )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceBuffers )( 
            IVxProjectionBufferBase * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Insert_2 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete_2 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace_2 )( 
            IVxProjectionBufferBase * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        END_INTERFACE
    } IVxProjectionBufferBaseVtbl;

    interface IVxProjectionBufferBase
    {
        CONST_VTBL struct IVxProjectionBufferBaseVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionBufferBase_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionBufferBase_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionBufferBase_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionBufferBase_GetProperties(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetProperties(This,ppRetVal) ) 


#define IVxProjectionBufferBase_IsReadOnly(This,span,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly(This,span,pRetVal) ) 

#define IVxProjectionBufferBase_IsReadOnly_2(This,span,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_2(This,span,isEdit,pRetVal) ) 

#define IVxProjectionBufferBase_GetReadOnlyExtents(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> GetReadOnlyExtents(This,span,ppRetVal) ) 

#define IVxProjectionBufferBase_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#define IVxProjectionBufferBase_GetCurrentSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrentSnapshot(This,ppRetVal) ) 

#define IVxProjectionBufferBase_CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal) ) 

#define IVxProjectionBufferBase_CreateEdit_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit_2(This,ppRetVal) ) 

#define IVxProjectionBufferBase_CreateReadOnlyRegionEdit(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateReadOnlyRegionEdit(This,ppRetVal) ) 

#define IVxProjectionBufferBase_GetEditInProgress(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEditInProgress(This,pRetVal) ) 

#define IVxProjectionBufferBase_TakeThreadOwnership(This)	\
    ( (This)->lpVtbl -> TakeThreadOwnership(This) ) 

#define IVxProjectionBufferBase_CheckEditAccess(This,pRetVal)	\
    ( (This)->lpVtbl -> CheckEditAccess(This,pRetVal) ) 

#define IVxProjectionBufferBase_AdviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxProjectionBufferBase_AdviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanged(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanged(This,pValue) ) 

#define IVxProjectionBufferBase_AdviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedLowPriority(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedLowPriority(This,pValue) ) 

#define IVxProjectionBufferBase_AdviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedHighPriority(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedHighPriority(This,pValue) ) 

#define IVxProjectionBufferBase_AdviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanging(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanging(This,pValue) ) 

#define IVxProjectionBufferBase_AdvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdvisePostChanged(This,pValue) ) 

#define IVxProjectionBufferBase_UnadvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadvisePostChanged(This,pValue) ) 

#define IVxProjectionBufferBase_AdviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseContentTypeChanged(This,pValue) ) 

#define IVxProjectionBufferBase_UnadviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseContentTypeChanged(This,pValue) ) 

#define IVxProjectionBufferBase_ChangeContentType(This,pNewContentType,pEditTag)	\
    ( (This)->lpVtbl -> ChangeContentType(This,pNewContentType,pEditTag) ) 

#define IVxProjectionBufferBase_Insert(This,position,text,ppRetVal)	\
    ( (This)->lpVtbl -> Insert(This,position,text,ppRetVal) ) 

#define IVxProjectionBufferBase_Delete(This,deleteSpan,ppRetVal)	\
    ( (This)->lpVtbl -> Delete(This,deleteSpan,ppRetVal) ) 

#define IVxProjectionBufferBase_Replace(This,replaceSpan,replaceWith,ppRetVal)	\
    ( (This)->lpVtbl -> Replace(This,replaceSpan,replaceWith,ppRetVal) ) 

#define IVxProjectionBufferBase_IsReadOnly_3(This,position,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_3(This,position,pRetVal) ) 

#define IVxProjectionBufferBase_IsReadOnly_4(This,position,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_4(This,position,isEdit,pRetVal) ) 


#define IVxProjectionBufferBase_GetCurrentSnapshot_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrentSnapshot_2(This,ppRetVal) ) 

#define IVxProjectionBufferBase_GetSourceBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSourceBuffers(This,ppRetVal) ) 

#define IVxProjectionBufferBase_Insert_2(This,position,text,ppRetVal)	\
    ( (This)->lpVtbl -> Insert_2(This,position,text,ppRetVal) ) 

#define IVxProjectionBufferBase_Delete_2(This,deleteSpan,ppRetVal)	\
    ( (This)->lpVtbl -> Delete_2(This,deleteSpan,ppRetVal) ) 

#define IVxProjectionBufferBase_Replace_2(This,replaceSpan,replaceWith,ppRetVal)	\
    ( (This)->lpVtbl -> Replace_2(This,replaceSpan,replaceWith,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionBufferBase_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionSnapshot_INTERFACE_DEFINED__
#define __IVxProjectionSnapshot_INTERFACE_DEFINED__

/* interface IVxProjectionSnapshot */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionSnapshot;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ea74bbf4-8acb-4c36-a3ef-f05d9e14ca9e")
    IVxProjectionSnapshot : public IVxTextSnapshot
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer_2( 
            /* [retval][out] */ IVxProjectionBufferBase **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanCount( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceSnapshots( 
            /* [retval][out] */ IListIVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMatchingSnapshot( 
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceSpans( 
            /* [in] */ int startSpanIndex,
            /* [in] */ int count,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSourceSpans_2( 
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToSourceSnapshot( 
            /* [in] */ int position,
            /* [in] */ VxPositionAffinity affinity,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToSourceSnapshots( 
            /* [in] */ int position,
            /* [retval][out] */ IListVxSnapshotPoint **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToSourceSnapshot_2( 
            /* [in] */ int position,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapFromSourceSnapshot( 
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapToSourceSnapshots_2( 
            /* [in] */ VxSpan span,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE MapFromSourceSnapshot_2( 
            /* [in] */ VxSnapshotSpan span,
            /* [retval][out] */ IListVxSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionSnapshotVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionSnapshot * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionSnapshot * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionSnapshot * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetVersion )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLength )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineCount )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int startIndex,
            /* [in] */ int length,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetText_3 )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ BSTR *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingPoint_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [in] */ VxPointTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSpan span,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_3 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTrackingSpan_4 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int start,
            /* [in] */ int length,
            /* [in] */ VxSpanTrackingMode trackingMode,
            /* [in] */ VxTrackingFidelityMode trackingFidelity,
            /* [retval][out] */ IVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineFromLineNumber )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int lineNumber,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineFromPosition )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ IVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLineNumberFromPosition )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetLines )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IEnumerableIVxTextSnapshotLine **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Write )( 
            IVxProjectionSnapshot * This,
            /* [in] */ IUnknown *pWriter,
            /* [in] */ VxSpan span);
        
        HRESULT ( STDMETHODCALLTYPE *Write_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ IUnknown *pWriter);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer_2 )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IVxProjectionBufferBase **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanCount )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceSnapshots )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IListIVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetMatchingSnapshot )( 
            IVxProjectionSnapshot * This,
            /* [in] */ IVxTextBuffer *pTextBuffer,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceSpans )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int startSpanIndex,
            /* [in] */ int count,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceSpans_2 )( 
            IVxProjectionSnapshot * This,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSourceSnapshot )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [in] */ VxPositionAffinity affinity,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSourceSnapshots )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ IListVxSnapshotPoint **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSourceSnapshot_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ int position,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapFromSourceSnapshot )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSnapshotPoint point,
            /* [in] */ VxPositionAffinity affinity,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotPoint *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapToSourceSnapshots_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *MapFromSourceSnapshot_2 )( 
            IVxProjectionSnapshot * This,
            /* [in] */ VxSnapshotSpan span,
            /* [retval][out] */ IListVxSpan **ppRetVal);
        
        END_INTERFACE
    } IVxProjectionSnapshotVtbl;

    interface IVxProjectionSnapshot
    {
        CONST_VTBL struct IVxProjectionSnapshotVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionSnapshot_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionSnapshot_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionSnapshot_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionSnapshot_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#define IVxProjectionSnapshot_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#define IVxProjectionSnapshot_GetVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetVersion(This,ppRetVal) ) 

#define IVxProjectionSnapshot_GetLength(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLength(This,pRetVal) ) 

#define IVxProjectionSnapshot_GetLineCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetLineCount(This,pRetVal) ) 

#define IVxProjectionSnapshot_GetText(This,span,pRetVal)	\
    ( (This)->lpVtbl -> GetText(This,span,pRetVal) ) 

#define IVxProjectionSnapshot_GetText_2(This,startIndex,length,pRetVal)	\
    ( (This)->lpVtbl -> GetText_2(This,startIndex,length,pRetVal) ) 

#define IVxProjectionSnapshot_GetText_3(This,pRetVal)	\
    ( (This)->lpVtbl -> GetText_3(This,pRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingPoint(This,position,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint(This,position,trackingMode,ppRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingPoint_2(This,position,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingSpan(This,span,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan(This,span,trackingMode,ppRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_2(This,span,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_3(This,start,length,trackingMode,ppRetVal) ) 

#define IVxProjectionSnapshot_CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTrackingSpan_4(This,start,length,trackingMode,trackingFidelity,ppRetVal) ) 

#define IVxProjectionSnapshot_GetLineFromLineNumber(This,lineNumber,ppRetVal)	\
    ( (This)->lpVtbl -> GetLineFromLineNumber(This,lineNumber,ppRetVal) ) 

#define IVxProjectionSnapshot_GetLineFromPosition(This,position,ppRetVal)	\
    ( (This)->lpVtbl -> GetLineFromPosition(This,position,ppRetVal) ) 

#define IVxProjectionSnapshot_GetLineNumberFromPosition(This,position,pRetVal)	\
    ( (This)->lpVtbl -> GetLineNumberFromPosition(This,position,pRetVal) ) 

#define IVxProjectionSnapshot_GetLines(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetLines(This,ppRetVal) ) 

#define IVxProjectionSnapshot_Write(This,pWriter,span)	\
    ( (This)->lpVtbl -> Write(This,pWriter,span) ) 

#define IVxProjectionSnapshot_Write_2(This,pWriter)	\
    ( (This)->lpVtbl -> Write_2(This,pWriter) ) 


#define IVxProjectionSnapshot_GetTextBuffer_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer_2(This,ppRetVal) ) 

#define IVxProjectionSnapshot_GetSpanCount(This,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanCount(This,pRetVal) ) 

#define IVxProjectionSnapshot_GetSourceSnapshots(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSourceSnapshots(This,ppRetVal) ) 

#define IVxProjectionSnapshot_GetMatchingSnapshot(This,pTextBuffer,ppRetVal)	\
    ( (This)->lpVtbl -> GetMatchingSnapshot(This,pTextBuffer,ppRetVal) ) 

#define IVxProjectionSnapshot_GetSourceSpans(This,startSpanIndex,count,ppRetVal)	\
    ( (This)->lpVtbl -> GetSourceSpans(This,startSpanIndex,count,ppRetVal) ) 

#define IVxProjectionSnapshot_GetSourceSpans_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSourceSpans_2(This,ppRetVal) ) 

#define IVxProjectionSnapshot_MapToSourceSnapshot(This,position,affinity,pRetVal)	\
    ( (This)->lpVtbl -> MapToSourceSnapshot(This,position,affinity,pRetVal) ) 

#define IVxProjectionSnapshot_MapToSourceSnapshots(This,position,ppRetVal)	\
    ( (This)->lpVtbl -> MapToSourceSnapshots(This,position,ppRetVal) ) 

#define IVxProjectionSnapshot_MapToSourceSnapshot_2(This,position,pRetVal)	\
    ( (This)->lpVtbl -> MapToSourceSnapshot_2(This,position,pRetVal) ) 

#define IVxProjectionSnapshot_MapFromSourceSnapshot(This,point,affinity,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> MapFromSourceSnapshot(This,point,affinity,pRetValHasValue,pRetVal) ) 

#define IVxProjectionSnapshot_MapToSourceSnapshots_2(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> MapToSourceSnapshots_2(This,span,ppRetVal) ) 

#define IVxProjectionSnapshot_MapFromSourceSnapshot_2(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> MapFromSourceSnapshot_2(This,span,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionSnapshot_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionBuffer_INTERFACE_DEFINED__
#define __IVxProjectionBuffer_INTERFACE_DEFINED__

/* interface IVxProjectionBuffer */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionBuffer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d8a38847-94af-4919-aa4e-be760e66905d")
    IVxProjectionBuffer : public IVxProjectionBufferBase
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE InsertSpan( 
            /* [in] */ int position,
            /* [in] */ IVxTrackingSpan *pSpanToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InsertSpan_2( 
            /* [in] */ int position,
            /* [in] */ BSTR literalSpanToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InsertSpans( 
            /* [in] */ int position,
            /* [in] */ IListIUnknown *pSpansToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteSpans( 
            /* [in] */ int position,
            /* [in] */ int spansToDelete,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReplaceSpans( 
            /* [in] */ int position,
            /* [in] */ int spansToReplace,
            /* [in] */ IListIUnknown *pSpansToInsert,
            /* [in] */ VxEditOptions options,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseSourceSpansChanged( 
            /* [in] */ IVxProjectionSourceSpansChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseSourceSpansChanged( 
            /* [in] */ IVxProjectionSourceSpansChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AdviseSourceBuffersChanged( 
            /* [in] */ IVxProjectionSourceBuffersChangedEvent *pValue) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnadviseSourceBuffersChanged( 
            /* [in] */ IVxProjectionSourceBuffersChangedEvent *pValue) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionBufferVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionBuffer * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionBuffer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetProperties )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxPropertyCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_2 )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan span,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetReadOnlyExtents )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan span,
            /* [retval][out] */ IVxNormalizedSpanCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSnapshot )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxEditOptions options,
            /* [in] */ BOOL reiteratedVersionNumberHasValue,
            /* [in] */ int reiteratedVersionNumber,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateEdit_2 )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxTextEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateReadOnlyRegionEdit )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxReadOnlyRegionEdit **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditInProgress )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *TakeThreadOwnership )( 
            IVxProjectionBuffer * This);
        
        HRESULT ( STDMETHODCALLTYPE *CheckEditAccess )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseReadOnlyRegionsChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseReadOnlyRegionsChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxSnapshotSpanEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedLowPriority )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedLowPriority )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChangedHighPriority )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChangedHighPriority )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseChanging )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseChanging )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxTextContentChangingEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdvisePostChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadvisePostChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxEventArgsEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseContentTypeChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseContentTypeChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxContentTypeChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *ChangeContentType )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxContentType *pNewContentType,
            /* [in] */ IUnknown *pEditTag);
        
        HRESULT ( STDMETHODCALLTYPE *Insert )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_3 )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *IsReadOnly_4 )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ BOOL isEdit,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSnapshot_2 )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSourceBuffers )( 
            IVxProjectionBuffer * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Insert_2 )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ BSTR text,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Delete_2 )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan deleteSpan,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Replace_2 )( 
            IVxProjectionBuffer * This,
            /* [in] */ VxSpan replaceSpan,
            /* [in] */ BSTR replaceWith,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *InsertSpan )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ IVxTrackingSpan *pSpanToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *InsertSpan_2 )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ BSTR literalSpanToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *InsertSpans )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ IListIUnknown *pSpansToInsert,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteSpans )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ int spansToDelete,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *ReplaceSpans )( 
            IVxProjectionBuffer * This,
            /* [in] */ int position,
            /* [in] */ int spansToReplace,
            /* [in] */ IListIUnknown *pSpansToInsert,
            /* [in] */ VxEditOptions options,
            /* [in] */ IUnknown *pEditTag,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseSourceSpansChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxProjectionSourceSpansChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseSourceSpansChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxProjectionSourceSpansChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *AdviseSourceBuffersChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxProjectionSourceBuffersChangedEvent *pValue);
        
        HRESULT ( STDMETHODCALLTYPE *UnadviseSourceBuffersChanged )( 
            IVxProjectionBuffer * This,
            /* [in] */ IVxProjectionSourceBuffersChangedEvent *pValue);
        
        END_INTERFACE
    } IVxProjectionBufferVtbl;

    interface IVxProjectionBuffer
    {
        CONST_VTBL struct IVxProjectionBufferVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionBuffer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionBuffer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionBuffer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionBuffer_GetProperties(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetProperties(This,ppRetVal) ) 


#define IVxProjectionBuffer_IsReadOnly(This,span,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly(This,span,pRetVal) ) 

#define IVxProjectionBuffer_IsReadOnly_2(This,span,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_2(This,span,isEdit,pRetVal) ) 

#define IVxProjectionBuffer_GetReadOnlyExtents(This,span,ppRetVal)	\
    ( (This)->lpVtbl -> GetReadOnlyExtents(This,span,ppRetVal) ) 

#define IVxProjectionBuffer_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#define IVxProjectionBuffer_GetCurrentSnapshot(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrentSnapshot(This,ppRetVal) ) 

#define IVxProjectionBuffer_CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit(This,options,reiteratedVersionNumberHasValue,reiteratedVersionNumber,pEditTag,ppRetVal) ) 

#define IVxProjectionBuffer_CreateEdit_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateEdit_2(This,ppRetVal) ) 

#define IVxProjectionBuffer_CreateReadOnlyRegionEdit(This,ppRetVal)	\
    ( (This)->lpVtbl -> CreateReadOnlyRegionEdit(This,ppRetVal) ) 

#define IVxProjectionBuffer_GetEditInProgress(This,pRetVal)	\
    ( (This)->lpVtbl -> GetEditInProgress(This,pRetVal) ) 

#define IVxProjectionBuffer_TakeThreadOwnership(This)	\
    ( (This)->lpVtbl -> TakeThreadOwnership(This) ) 

#define IVxProjectionBuffer_CheckEditAccess(This,pRetVal)	\
    ( (This)->lpVtbl -> CheckEditAccess(This,pRetVal) ) 

#define IVxProjectionBuffer_AdviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseReadOnlyRegionsChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseReadOnlyRegionsChanged(This,pValue) ) 

#define IVxProjectionBuffer_AdviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanged(This,pValue) ) 

#define IVxProjectionBuffer_AdviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedLowPriority(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseChangedLowPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedLowPriority(This,pValue) ) 

#define IVxProjectionBuffer_AdviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChangedHighPriority(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseChangedHighPriority(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChangedHighPriority(This,pValue) ) 

#define IVxProjectionBuffer_AdviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> AdviseChanging(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseChanging(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseChanging(This,pValue) ) 

#define IVxProjectionBuffer_AdvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdvisePostChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadvisePostChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadvisePostChanged(This,pValue) ) 

#define IVxProjectionBuffer_AdviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseContentTypeChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseContentTypeChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseContentTypeChanged(This,pValue) ) 

#define IVxProjectionBuffer_ChangeContentType(This,pNewContentType,pEditTag)	\
    ( (This)->lpVtbl -> ChangeContentType(This,pNewContentType,pEditTag) ) 

#define IVxProjectionBuffer_Insert(This,position,text,ppRetVal)	\
    ( (This)->lpVtbl -> Insert(This,position,text,ppRetVal) ) 

#define IVxProjectionBuffer_Delete(This,deleteSpan,ppRetVal)	\
    ( (This)->lpVtbl -> Delete(This,deleteSpan,ppRetVal) ) 

#define IVxProjectionBuffer_Replace(This,replaceSpan,replaceWith,ppRetVal)	\
    ( (This)->lpVtbl -> Replace(This,replaceSpan,replaceWith,ppRetVal) ) 

#define IVxProjectionBuffer_IsReadOnly_3(This,position,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_3(This,position,pRetVal) ) 

#define IVxProjectionBuffer_IsReadOnly_4(This,position,isEdit,pRetVal)	\
    ( (This)->lpVtbl -> IsReadOnly_4(This,position,isEdit,pRetVal) ) 


#define IVxProjectionBuffer_GetCurrentSnapshot_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetCurrentSnapshot_2(This,ppRetVal) ) 

#define IVxProjectionBuffer_GetSourceBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetSourceBuffers(This,ppRetVal) ) 

#define IVxProjectionBuffer_Insert_2(This,position,text,ppRetVal)	\
    ( (This)->lpVtbl -> Insert_2(This,position,text,ppRetVal) ) 

#define IVxProjectionBuffer_Delete_2(This,deleteSpan,ppRetVal)	\
    ( (This)->lpVtbl -> Delete_2(This,deleteSpan,ppRetVal) ) 

#define IVxProjectionBuffer_Replace_2(This,replaceSpan,replaceWith,ppRetVal)	\
    ( (This)->lpVtbl -> Replace_2(This,replaceSpan,replaceWith,ppRetVal) ) 


#define IVxProjectionBuffer_InsertSpan(This,position,pSpanToInsert,ppRetVal)	\
    ( (This)->lpVtbl -> InsertSpan(This,position,pSpanToInsert,ppRetVal) ) 

#define IVxProjectionBuffer_InsertSpan_2(This,position,literalSpanToInsert,ppRetVal)	\
    ( (This)->lpVtbl -> InsertSpan_2(This,position,literalSpanToInsert,ppRetVal) ) 

#define IVxProjectionBuffer_InsertSpans(This,position,pSpansToInsert,ppRetVal)	\
    ( (This)->lpVtbl -> InsertSpans(This,position,pSpansToInsert,ppRetVal) ) 

#define IVxProjectionBuffer_DeleteSpans(This,position,spansToDelete,ppRetVal)	\
    ( (This)->lpVtbl -> DeleteSpans(This,position,spansToDelete,ppRetVal) ) 

#define IVxProjectionBuffer_ReplaceSpans(This,position,spansToReplace,pSpansToInsert,options,pEditTag,ppRetVal)	\
    ( (This)->lpVtbl -> ReplaceSpans(This,position,spansToReplace,pSpansToInsert,options,pEditTag,ppRetVal) ) 

#define IVxProjectionBuffer_AdviseSourceSpansChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseSourceSpansChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseSourceSpansChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseSourceSpansChanged(This,pValue) ) 

#define IVxProjectionBuffer_AdviseSourceBuffersChanged(This,pValue)	\
    ( (This)->lpVtbl -> AdviseSourceBuffersChanged(This,pValue) ) 

#define IVxProjectionBuffer_UnadviseSourceBuffersChanged(This,pValue)	\
    ( (This)->lpVtbl -> UnadviseSourceBuffersChanged(This,pValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionBuffer_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionSourceSpansChangedEventArgs_INTERFACE_DEFINED__
#define __IVxProjectionSourceSpansChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxProjectionSourceSpansChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionSourceSpansChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9ef0e53c-b564-46d9-8616-d28034ac48c9")
    IVxProjectionSourceSpansChangedEventArgs : public IVxTextContentChangedEventArgs
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSpanPosition( 
            /* [retval][out] */ int *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetInsertedSpans( 
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDeletedSpans( 
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBefore_2( 
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetAfter_2( 
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionSourceSpansChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionSourceSpansChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionSourceSpansChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterVersion )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetChanges )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOptions )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ VxEditOptions *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanPosition )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetInsertedSpans )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeletedSpans )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore_2 )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter_2 )( 
            IVxProjectionSourceSpansChangedEventArgs * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        END_INTERFACE
    } IVxProjectionSourceSpansChangedEventArgsVtbl;

    interface IVxProjectionSourceSpansChangedEventArgs
    {
        CONST_VTBL struct IVxProjectionSourceSpansChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionSourceSpansChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionSourceSpansChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionSourceSpansChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionSourceSpansChangedEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetAfter(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetAfterVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterVersion(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 


#define IVxProjectionSourceSpansChangedEventArgs_GetChanges(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetChanges(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetOptions(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOptions(This,pRetVal) ) 


#define IVxProjectionSourceSpansChangedEventArgs_GetSpanPosition(This,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanPosition(This,pRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetInsertedSpans(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetInsertedSpans(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetDeletedSpans(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetDeletedSpans(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetBefore_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore_2(This,ppRetVal) ) 

#define IVxProjectionSourceSpansChangedEventArgs_GetAfter_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter_2(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionSourceSpansChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxProjectionSourceBuffersChangedEventArgs_INTERFACE_DEFINED__
#define __IVxProjectionSourceBuffersChangedEventArgs_INTERFACE_DEFINED__

/* interface IVxProjectionSourceBuffersChangedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxProjectionSourceBuffersChangedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d058cf17-7aa6-41da-8816-90b93250e998")
    IVxProjectionSourceBuffersChangedEventArgs : public IVxProjectionSourceSpansChangedEventArgs
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetAddedBuffers( 
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRemovedBuffers( 
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxProjectionSourceBuffersChangedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxProjectionSourceBuffersChangedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxProjectionSourceBuffersChangedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfterVersion )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetChanges )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxNormalizedTextChangeCollection **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetOptions )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ VxEditOptions *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanPosition )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ int *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetInsertedSpans )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeletedSpans )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTrackingSpan **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore_2 )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAfter_2 )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IVxProjectionSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetAddedBuffers )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemovedBuffers )( 
            IVxProjectionSourceBuffersChangedEventArgs * This,
            /* [retval][out] */ IListIVxTextBuffer **ppRetVal);
        
        END_INTERFACE
    } IVxProjectionSourceBuffersChangedEventArgsVtbl;

    interface IVxProjectionSourceBuffersChangedEventArgs
    {
        CONST_VTBL struct IVxProjectionSourceBuffersChangedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxProjectionSourceBuffersChangedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxProjectionSourceBuffersChangedEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetAfter(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetAfterVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfterVersion(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 


#define IVxProjectionSourceBuffersChangedEventArgs_GetChanges(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetChanges(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetOptions(This,pRetVal)	\
    ( (This)->lpVtbl -> GetOptions(This,pRetVal) ) 


#define IVxProjectionSourceBuffersChangedEventArgs_GetSpanPosition(This,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanPosition(This,pRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetInsertedSpans(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetInsertedSpans(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetDeletedSpans(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetDeletedSpans(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetBefore_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore_2(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetAfter_2(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAfter_2(This,ppRetVal) ) 


#define IVxProjectionSourceBuffersChangedEventArgs_GetAddedBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetAddedBuffers(This,ppRetVal) ) 

#define IVxProjectionSourceBuffersChangedEventArgs_GetRemovedBuffers(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetRemovedBuffers(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxProjectionSourceBuffersChangedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxSnapshotSpanEventArgs_INTERFACE_DEFINED__
#define __IVxSnapshotSpanEventArgs_INTERFACE_DEFINED__

/* interface IVxSnapshotSpanEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxSnapshotSpanEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("521a6cf3-58c3-4f05-be86-64871c7e7787")
    IVxSnapshotSpanEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSpan( 
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxSnapshotSpanEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxSnapshotSpanEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxSnapshotSpanEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxSnapshotSpanEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpan )( 
            IVxSnapshotSpanEventArgs * This,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        END_INTERFACE
    } IVxSnapshotSpanEventArgsVtbl;

    interface IVxSnapshotSpanEventArgs
    {
        CONST_VTBL struct IVxSnapshotSpanEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxSnapshotSpanEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxSnapshotSpanEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxSnapshotSpanEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxSnapshotSpanEventArgs_GetSpan(This,pRetVal)	\
    ( (This)->lpVtbl -> GetSpan(This,pRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxSnapshotSpanEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxTextBufferCreatedEventArgs_INTERFACE_DEFINED__
#define __IVxTextBufferCreatedEventArgs_INTERFACE_DEFINED__

/* interface IVxTextBufferCreatedEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextBufferCreatedEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("318855de-0da5-4d1d-9cce-481d99b75f16")
    IVxTextBufferCreatedEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetTextBuffer( 
            /* [retval][out] */ IVxTextBuffer **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextBufferCreatedEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextBufferCreatedEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextBufferCreatedEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextBufferCreatedEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTextBuffer )( 
            IVxTextBufferCreatedEventArgs * This,
            /* [retval][out] */ IVxTextBuffer **ppRetVal);
        
        END_INTERFACE
    } IVxTextBufferCreatedEventArgsVtbl;

    interface IVxTextBufferCreatedEventArgs
    {
        CONST_VTBL struct IVxTextBufferCreatedEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextBufferCreatedEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextBufferCreatedEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextBufferCreatedEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextBufferCreatedEventArgs_GetTextBuffer(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetTextBuffer(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextBufferCreatedEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxTextContentChangingEventArgs_INTERFACE_DEFINED__
#define __IVxTextContentChangingEventArgs_INTERFACE_DEFINED__

/* interface IVxTextContentChangingEventArgs */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextContentChangingEventArgs;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("f26e15b3-89f8-4d5e-9bff-3d95c0101499")
    IVxTextContentChangingEventArgs : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCanceled( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBefore( 
            /* [retval][out] */ IVxTextSnapshot **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetEditTag( 
            /* [retval][out] */ IUnknown **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Cancel( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBeforeVersion( 
            /* [retval][out] */ IVxTextVersion **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextContentChangingEventArgsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextContentChangingEventArgs * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextContentChangingEventArgs * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextContentChangingEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCanceled )( 
            IVxTextContentChangingEventArgs * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetBefore )( 
            IVxTextContentChangingEventArgs * This,
            /* [retval][out] */ IVxTextSnapshot **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetEditTag )( 
            IVxTextContentChangingEventArgs * This,
            /* [retval][out] */ IUnknown **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *Cancel )( 
            IVxTextContentChangingEventArgs * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetBeforeVersion )( 
            IVxTextContentChangingEventArgs * This,
            /* [retval][out] */ IVxTextVersion **ppRetVal);
        
        END_INTERFACE
    } IVxTextContentChangingEventArgsVtbl;

    interface IVxTextContentChangingEventArgs
    {
        CONST_VTBL struct IVxTextContentChangingEventArgsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextContentChangingEventArgs_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextContentChangingEventArgs_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextContentChangingEventArgs_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextContentChangingEventArgs_GetCanceled(This,pRetVal)	\
    ( (This)->lpVtbl -> GetCanceled(This,pRetVal) ) 

#define IVxTextContentChangingEventArgs_GetBefore(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBefore(This,ppRetVal) ) 

#define IVxTextContentChangingEventArgs_GetEditTag(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetEditTag(This,ppRetVal) ) 

#define IVxTextContentChangingEventArgs_Cancel(This)	\
    ( (This)->lpVtbl -> Cancel(This) ) 

#define IVxTextContentChangingEventArgs_GetBeforeVersion(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetBeforeVersion(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextContentChangingEventArgs_INTERFACE_DEFINED__ */


#ifndef __IVxTextSearchService_INTERFACE_DEFINED__
#define __IVxTextSearchService_INTERFACE_DEFINED__

/* interface IVxTextSearchService */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextSearchService;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("30d6bd3f-c415-42a6-b6e2-fad40ba8ab35")
    IVxTextSearchService : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE FindNext( 
            /* [in] */ int startIndex,
            /* [in] */ BOOL wraparound,
            /* [in] */ VxFindData findData,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindAll( 
            /* [in] */ VxFindData findData,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextSearchServiceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextSearchService * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextSearchService * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextSearchService * This);
        
        HRESULT ( STDMETHODCALLTYPE *FindNext )( 
            IVxTextSearchService * This,
            /* [in] */ int startIndex,
            /* [in] */ BOOL wraparound,
            /* [in] */ VxFindData findData,
            /* [out] */ BOOL *pRetValHasValue,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *FindAll )( 
            IVxTextSearchService * This,
            /* [in] */ VxFindData findData,
            /* [retval][out] */ IListVxSnapshotSpan **ppRetVal);
        
        END_INTERFACE
    } IVxTextSearchServiceVtbl;

    interface IVxTextSearchService
    {
        CONST_VTBL struct IVxTextSearchServiceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextSearchService_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextSearchService_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextSearchService_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextSearchService_FindNext(This,startIndex,wraparound,findData,pRetValHasValue,pRetVal)	\
    ( (This)->lpVtbl -> FindNext(This,startIndex,wraparound,findData,pRetValHasValue,pRetVal) ) 

#define IVxTextSearchService_FindAll(This,findData,ppRetVal)	\
    ( (This)->lpVtbl -> FindAll(This,findData,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextSearchService_INTERFACE_DEFINED__ */


#ifndef __IVxTextStructureNavigator_INTERFACE_DEFINED__
#define __IVxTextStructureNavigator_INTERFACE_DEFINED__

/* interface IVxTextStructureNavigator */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxTextStructureNavigator;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("112990cb-72c0-43a9-ab89-bca935c4806f")
    IVxTextStructureNavigator : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExtentOfWord( 
            /* [in] */ VxSnapshotPoint currentPosition,
            /* [retval][out] */ VxTextExtent *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanOfEnclosing( 
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanOfFirstChild( 
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanOfNextSibling( 
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpanOfPreviousSibling( 
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContentType( 
            /* [retval][out] */ IVxContentType **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxTextStructureNavigatorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxTextStructureNavigator * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxTextStructureNavigator * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxTextStructureNavigator * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExtentOfWord )( 
            IVxTextStructureNavigator * This,
            /* [in] */ VxSnapshotPoint currentPosition,
            /* [retval][out] */ VxTextExtent *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanOfEnclosing )( 
            IVxTextStructureNavigator * This,
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanOfFirstChild )( 
            IVxTextStructureNavigator * This,
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanOfNextSibling )( 
            IVxTextStructureNavigator * This,
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpanOfPreviousSibling )( 
            IVxTextStructureNavigator * This,
            /* [in] */ VxSnapshotSpan activeSpan,
            /* [retval][out] */ VxSnapshotSpan *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *GetContentType )( 
            IVxTextStructureNavigator * This,
            /* [retval][out] */ IVxContentType **ppRetVal);
        
        END_INTERFACE
    } IVxTextStructureNavigatorVtbl;

    interface IVxTextStructureNavigator
    {
        CONST_VTBL struct IVxTextStructureNavigatorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxTextStructureNavigator_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxTextStructureNavigator_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxTextStructureNavigator_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxTextStructureNavigator_GetExtentOfWord(This,currentPosition,pRetVal)	\
    ( (This)->lpVtbl -> GetExtentOfWord(This,currentPosition,pRetVal) ) 

#define IVxTextStructureNavigator_GetSpanOfEnclosing(This,activeSpan,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanOfEnclosing(This,activeSpan,pRetVal) ) 

#define IVxTextStructureNavigator_GetSpanOfFirstChild(This,activeSpan,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanOfFirstChild(This,activeSpan,pRetVal) ) 

#define IVxTextStructureNavigator_GetSpanOfNextSibling(This,activeSpan,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanOfNextSibling(This,activeSpan,pRetVal) ) 

#define IVxTextStructureNavigator_GetSpanOfPreviousSibling(This,activeSpan,pRetVal)	\
    ( (This)->lpVtbl -> GetSpanOfPreviousSibling(This,activeSpan,pRetVal) ) 

#define IVxTextStructureNavigator_GetContentType(This,ppRetVal)	\
    ( (This)->lpVtbl -> GetContentType(This,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxTextStructureNavigator_INTERFACE_DEFINED__ */


#ifndef __IVxPlatformFactory_INTERFACE_DEFINED__
#define __IVxPlatformFactory_INTERFACE_DEFINED__

/* interface IVxPlatformFactory */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxPlatformFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("a5596e43-5f0e-4dec-83e9-45f8fe64c64f")
    IVxPlatformFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateTextBufferFactoryService( 
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextBufferFactoryService **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextDocumentFactoryService( 
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextDocumentFactoryService **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateContentTypeRegistryService( 
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxContentTypeRegistryService **ppRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateTextSearchService( 
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextSearchService **ppRetVal) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxPlatformFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxPlatformFactory * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxPlatformFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxPlatformFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextBufferFactoryService )( 
            IVxPlatformFactory * This,
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextBufferFactoryService **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextDocumentFactoryService )( 
            IVxPlatformFactory * This,
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextDocumentFactoryService **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateContentTypeRegistryService )( 
            IVxPlatformFactory * This,
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxContentTypeRegistryService **ppRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *CreateTextSearchService )( 
            IVxPlatformFactory * This,
            /* [in] */ IUnknown *pServiceProvider,
            /* [retval][out] */ IVxTextSearchService **ppRetVal);
        
        END_INTERFACE
    } IVxPlatformFactoryVtbl;

    interface IVxPlatformFactory
    {
        CONST_VTBL struct IVxPlatformFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxPlatformFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxPlatformFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxPlatformFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxPlatformFactory_CreateTextBufferFactoryService(This,pServiceProvider,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextBufferFactoryService(This,pServiceProvider,ppRetVal) ) 

#define IVxPlatformFactory_CreateTextDocumentFactoryService(This,pServiceProvider,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextDocumentFactoryService(This,pServiceProvider,ppRetVal) ) 

#define IVxPlatformFactory_CreateContentTypeRegistryService(This,pServiceProvider,ppRetVal)	\
    ( (This)->lpVtbl -> CreateContentTypeRegistryService(This,pServiceProvider,ppRetVal) ) 

#define IVxPlatformFactory_CreateTextSearchService(This,pServiceProvider,ppRetVal)	\
    ( (This)->lpVtbl -> CreateTextSearchService(This,pServiceProvider,ppRetVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxPlatformFactory_INTERFACE_DEFINED__ */


#ifndef __IVxThumbnailSupport_INTERFACE_DEFINED__
#define __IVxThumbnailSupport_INTERFACE_DEFINED__

/* interface IVxThumbnailSupport */
/* [uuid][object] */ 


EXTERN_C const IID IID_IVxThumbnailSupport;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("d83723ad-977d-4bf0-8534-1f1f72f5ac00")
    IVxThumbnailSupport : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRemoveVisualsWhenHidden( 
            /* [retval][out] */ BOOL *pRetVal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRemoveVisualsWhenHidden( 
            /* [in] */ BOOL value) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IVxThumbnailSupportVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVxThumbnailSupport * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVxThumbnailSupport * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVxThumbnailSupport * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRemoveVisualsWhenHidden )( 
            IVxThumbnailSupport * This,
            /* [retval][out] */ BOOL *pRetVal);
        
        HRESULT ( STDMETHODCALLTYPE *SetRemoveVisualsWhenHidden )( 
            IVxThumbnailSupport * This,
            /* [in] */ BOOL value);
        
        END_INTERFACE
    } IVxThumbnailSupportVtbl;

    interface IVxThumbnailSupport
    {
        CONST_VTBL struct IVxThumbnailSupportVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVxThumbnailSupport_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVxThumbnailSupport_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVxThumbnailSupport_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVxThumbnailSupport_GetRemoveVisualsWhenHidden(This,pRetVal)	\
    ( (This)->lpVtbl -> GetRemoveVisualsWhenHidden(This,pRetVal) ) 

#define IVxThumbnailSupport_SetRemoveVisualsWhenHidden(This,value)	\
    ( (This)->lpVtbl -> SetRemoveVisualsWhenHidden(This,value) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVxThumbnailSupport_INTERFACE_DEFINED__ */

#endif /* __VSEditorLibrary_LIBRARY_DEFINED__ */

/* interface __MIDL_itf_VSEditor_0001_0141 */
/* [local] */ 

struct CVxStringDifferenceOptions : public VxStringDifferenceOptions
{
    CVxStringDifferenceOptions()
    {
        Init();
    }
    CVxStringDifferenceOptions(const VxStringDifferenceOptions &source)
    {
        *this = source;
    }
    CVxStringDifferenceOptions(VxStringDifferenceTypes _differenceType, int _locality, BOOL _ignoreTrimWhiteSpace)
    {
        differenceType = _differenceType;
        locality = _locality;
        ignoreTrimWhiteSpace = _ignoreTrimWhiteSpace;
    }
    void Init()
    {
        CVxStringDifferenceOptions::Init(*this);
    }
    static void Init(VxStringDifferenceOptions &obj)
    {
        obj.differenceType = static_cast<VxStringDifferenceTypes>(0);
        obj.locality = 0;
        obj.ignoreTrimWhiteSpace = FALSE;
    }
};
struct CVxEditOptions : public VxEditOptions
{
    CVxEditOptions()
    {
        Init();
    }
    CVxEditOptions(const VxEditOptions &source)
    {
        *this = source;
    }
    CVxEditOptions(BOOL _computeMinimalChange, const CVxStringDifferenceOptions &_differenceOptions)
    {
        computeMinimalChange = _computeMinimalChange;
        differenceOptions = _differenceOptions;
    }
    void Init()
    {
        CVxEditOptions::Init(*this);
    }
    static void Init(VxEditOptions &obj)
    {
        obj.computeMinimalChange = FALSE;
        CVxStringDifferenceOptions::Init(obj.differenceOptions);
    }
};
struct CVxSnapshotPoint : public VxSnapshotPoint
{
    CVxSnapshotPoint()
    {
        Init();
    }
    CVxSnapshotPoint(const VxSnapshotPoint &source)
    {
        InitFrom(source);
    }
    CVxSnapshotPoint(IVxTextSnapshot* _snapshot, int _position)
    {
        if (_snapshot != NULL)
            _snapshot->AddRef();
        snapshot = _snapshot;
        position = _position;
    }
    void Init()
    {
        CVxSnapshotPoint::Init(*this);
    }
    static void Init(VxSnapshotPoint &obj)
    {
        obj.snapshot = 0;
        obj.position = 0;
    }
    void InitFrom(const VxSnapshotPoint &dest)
    {
        CVxSnapshotPoint::InitFrom(*this, dest);
    }
    static void InitFrom(VxSnapshotPoint &dest, const CVxSnapshotPoint &source)
    {
        if (source.snapshot != NULL)
            source.snapshot->AddRef();
        dest.snapshot = source.snapshot;
        dest.position = source.position;
    }
    CVxSnapshotPoint* operator&()
    {
#ifdef _DEBUG
        AssertIsReleased();
#endif
        return this;
    }
    ~CVxSnapshotPoint()
    {
        Release();
    }
    CVxSnapshotPoint& operator=(const VxSnapshotPoint &source)
    {
        CVxSnapshotPoint::CopyTo(source, *this);
        return *this;
    }
    void CopyTo(VxSnapshotPoint &dest) const
    {
        CVxSnapshotPoint::CopyTo(*this, dest);
    }
    static void CopyTo(const CVxSnapshotPoint &source, VxSnapshotPoint &dest)
    {
        if (&source == &dest)
            return;
        if (source.snapshot != NULL)
            source.snapshot->AddRef();
        if (dest.snapshot != NULL)
            dest.snapshot->Release();
        dest.snapshot = source.snapshot;
        dest.position = source.position;
    }
#ifdef _DEBUG
    void AssertIsReleased() const
    {
        CVxSnapshotPoint::AssertIsReleased(*this);
    }
    static void AssertIsReleased(const VxSnapshotPoint &obj)
    {
        VSASSERT(obj.snapshot == NULL, L"'obj.snapshot' is not NULL; potential memory leak.");
    }
#endif
    void Release()
    {
        CVxSnapshotPoint::Release(*this);
    }
    static void Release(VxSnapshotPoint &obj)
    {
        if (obj.snapshot != NULL)
        {
            obj.snapshot->Release();
            obj.snapshot = NULL;
        }
    }
};
struct CVxSnapshotSpan : public VxSnapshotSpan
{
    CVxSnapshotSpan()
    {
        Init();
    }
    CVxSnapshotSpan(const VxSnapshotSpan &source)
    {
        InitFrom(source);
    }
    CVxSnapshotSpan(const CVxSnapshotPoint &_start, int _length)
    {
        CVxSnapshotPoint::InitFrom(start, _start);
        length = _length;
    }
    void Init()
    {
        CVxSnapshotSpan::Init(*this);
    }
    static void Init(VxSnapshotSpan &obj)
    {
        CVxSnapshotPoint::Init(obj.start);
        obj.length = 0;
    }
    void InitFrom(const VxSnapshotSpan &dest)
    {
        CVxSnapshotSpan::InitFrom(*this, dest);
    }
    static void InitFrom(VxSnapshotSpan &dest, const CVxSnapshotSpan &source)
    {
        CVxSnapshotPoint::InitFrom(dest.start, source.start);
        dest.length = source.length;
    }
    CVxSnapshotSpan* operator&()
    {
#ifdef _DEBUG
        AssertIsReleased();
#endif
        return this;
    }
    ~CVxSnapshotSpan()
    {
        Release();
    }
    CVxSnapshotSpan& operator=(const VxSnapshotSpan &source)
    {
        CVxSnapshotSpan::CopyTo(source, *this);
        return *this;
    }
    void CopyTo(VxSnapshotSpan &dest) const
    {
        CVxSnapshotSpan::CopyTo(*this, dest);
    }
    static void CopyTo(const CVxSnapshotSpan &source, VxSnapshotSpan &dest)
    {
        if (&source == &dest)
            return;
        CVxSnapshotPoint::CopyTo(source.start, dest.start);
        dest.length = source.length;
    }
#ifdef _DEBUG
    void AssertIsReleased() const
    {
        CVxSnapshotSpan::AssertIsReleased(*this);
    }
    static void AssertIsReleased(const VxSnapshotSpan &obj)
    {
        CVxSnapshotPoint::AssertIsReleased(obj.start);
    }
#endif
    void Release()
    {
        CVxSnapshotSpan::Release(*this);
    }
    static void Release(VxSnapshotSpan &obj)
    {
        CVxSnapshotPoint::Release(obj.start);
    }
};
struct CVxSpan : public VxSpan
{
    CVxSpan()
    {
        Init();
    }
    CVxSpan(const VxSpan &source)
    {
        *this = source;
    }
    CVxSpan(int _start, int _length)
    {
        start = _start;
        length = _length;
    }
    void Init()
    {
        CVxSpan::Init(*this);
    }
    static void Init(VxSpan &obj)
    {
        obj.start = 0;
        obj.length = 0;
    }
};
struct CVxFindData : public VxFindData
{
    CVxFindData()
    {
        Init();
    }
    CVxFindData(const VxFindData &source)
    {
        InitFrom(source);
    }
    CVxFindData(LPCWSTR __searchString, IVxTextSnapshot* __textSnapshotToSearch, VxFindOptions __findOptions, IVxTextStructureNavigator* __textStructureNavigator)
    {
        if (_searchString != __searchString)
        {
            _searchString = (__searchString == NULL) ? NULL : SysAllocString(__searchString);
        }
        if (__textSnapshotToSearch != NULL)
            __textSnapshotToSearch->AddRef();
        _textSnapshotToSearch = __textSnapshotToSearch;
        _findOptions = __findOptions;
        if (__textStructureNavigator != NULL)
            __textStructureNavigator->AddRef();
        _textStructureNavigator = __textStructureNavigator;
    }
    void Init()
    {
        CVxFindData::Init(*this);
    }
    static void Init(VxFindData &obj)
    {
        obj._searchString = NULL;
        obj._textSnapshotToSearch = 0;
        obj._findOptions = static_cast<VxFindOptions>(0);
        obj._textStructureNavigator = 0;
    }
    void InitFrom(const VxFindData &dest)
    {
        CVxFindData::InitFrom(*this, dest);
    }
    static void InitFrom(VxFindData &dest, const CVxFindData &source)
    {
        if (dest._searchString != source._searchString)
        {
            dest._searchString = (source._searchString == NULL) ? NULL : SysAllocString(source._searchString);
        }
        if (source._textSnapshotToSearch != NULL)
            source._textSnapshotToSearch->AddRef();
        dest._textSnapshotToSearch = source._textSnapshotToSearch;
        dest._findOptions = source._findOptions;
        if (source._textStructureNavigator != NULL)
            source._textStructureNavigator->AddRef();
        dest._textStructureNavigator = source._textStructureNavigator;
    }
    CVxFindData* operator&()
    {
#ifdef _DEBUG
        AssertIsReleased();
#endif
        return this;
    }
    ~CVxFindData()
    {
        Release();
    }
    CVxFindData& operator=(const VxFindData &source)
    {
        CVxFindData::CopyTo(source, *this);
        return *this;
    }
    void CopyTo(VxFindData &dest) const
    {
        CVxFindData::CopyTo(*this, dest);
    }
    static void CopyTo(const CVxFindData &source, VxFindData &dest)
    {
        if (&source == &dest)
            return;
        if (dest._searchString != source._searchString)
        {
            SysFreeString(dest._searchString);
            dest._searchString = (source._searchString == NULL) ? NULL : SysAllocString(source._searchString);
        }
        if (source._textSnapshotToSearch != NULL)
            source._textSnapshotToSearch->AddRef();
        if (dest._textSnapshotToSearch != NULL)
            dest._textSnapshotToSearch->Release();
        dest._textSnapshotToSearch = source._textSnapshotToSearch;
        dest._findOptions = source._findOptions;
        if (source._textStructureNavigator != NULL)
            source._textStructureNavigator->AddRef();
        if (dest._textStructureNavigator != NULL)
            dest._textStructureNavigator->Release();
        dest._textStructureNavigator = source._textStructureNavigator;
    }
#ifdef _DEBUG
    void AssertIsReleased() const
    {
        CVxFindData::AssertIsReleased(*this);
    }
    static void AssertIsReleased(const VxFindData &obj)
    {
        VSASSERT(obj._searchString == NULL, L"'obj._searchString' is not NULL; potential memory leak.");
        VSASSERT(obj._textSnapshotToSearch == NULL, L"'obj._textSnapshotToSearch' is not NULL; potential memory leak.");
        VSASSERT(obj._textStructureNavigator == NULL, L"'obj._textStructureNavigator' is not NULL; potential memory leak.");
    }
#endif
    void Release()
    {
        CVxFindData::Release(*this);
    }
    static void Release(VxFindData &obj)
    {
        if (obj._searchString != NULL)
        {
            SysFreeString(obj._searchString);
            obj._searchString = NULL;
        }
        if (obj._textSnapshotToSearch != NULL)
        {
            obj._textSnapshotToSearch->Release();
            obj._textSnapshotToSearch = NULL;
        }
        if (obj._textStructureNavigator != NULL)
        {
            obj._textStructureNavigator->Release();
            obj._textStructureNavigator = NULL;
        }
    }
};
struct CVxTextExtent : public VxTextExtent
{
    CVxTextExtent()
    {
        Init();
    }
    CVxTextExtent(const VxTextExtent &source)
    {
        InitFrom(source);
    }
    CVxTextExtent(const CVxSnapshotSpan &__span, BOOL __isSignificant)
    {
        CVxSnapshotSpan::InitFrom(_span, __span);
        _isSignificant = __isSignificant;
    }
    void Init()
    {
        CVxTextExtent::Init(*this);
    }
    static void Init(VxTextExtent &obj)
    {
        CVxSnapshotSpan::Init(obj._span);
        obj._isSignificant = FALSE;
    }
    void InitFrom(const VxTextExtent &dest)
    {
        CVxTextExtent::InitFrom(*this, dest);
    }
    static void InitFrom(VxTextExtent &dest, const CVxTextExtent &source)
    {
        CVxSnapshotSpan::InitFrom(dest._span, source._span);
        dest._isSignificant = source._isSignificant;
    }
    CVxTextExtent* operator&()
    {
#ifdef _DEBUG
        AssertIsReleased();
#endif
        return this;
    }
    ~CVxTextExtent()
    {
        Release();
    }
    CVxTextExtent& operator=(const VxTextExtent &source)
    {
        CVxTextExtent::CopyTo(source, *this);
        return *this;
    }
    void CopyTo(VxTextExtent &dest) const
    {
        CVxTextExtent::CopyTo(*this, dest);
    }
    static void CopyTo(const CVxTextExtent &source, VxTextExtent &dest)
    {
        if (&source == &dest)
            return;
        CVxSnapshotSpan::CopyTo(source._span, dest._span);
        dest._isSignificant = source._isSignificant;
    }
#ifdef _DEBUG
    void AssertIsReleased() const
    {
        CVxTextExtent::AssertIsReleased(*this);
    }
    static void AssertIsReleased(const VxTextExtent &obj)
    {
        CVxSnapshotSpan::AssertIsReleased(obj._span);
    }
#endif
    void Release()
    {
        CVxTextExtent::Release(*this);
    }
    static void Release(VxTextExtent &obj)
    {
        CVxSnapshotSpan::Release(obj._span);
    }
};


extern RPC_IF_HANDLE __MIDL_itf_VSEditor_0001_0141_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VSEditor_0001_0141_v0_0_s_ifspec;

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


