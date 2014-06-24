/*-----------------------------------------------------------------------------
Microsoft VSEE

Copyright 1995-2000 Microsoft Corporation. All Rights Reserved.

@doc
@module VseeGuids.h - Guids for VSEE services/interfaces |
This is included by .idl files to define guids for VSEE services/interfaces.
These are the interfaces related to the shell SDK, i.e. in idl/vs

@owner Source Control Integration Team
-----------------------------------------------------------------------------*/
#pragma once

#ifdef _WIN64

#define uuid_IVsSccManager2							53474C4D-B927-4320-B9DA-13D2CB3EA93B
#define uuid_IVsSccManager3							53474C4D-A152-4757-868A-2A7D0F2E5580
#define uuid_IVsSccProject2							53474C4D-AC92-49AC-9172-603E01FA483A
#define uuid_IVsQueryEditQuerySave2					53474C4D-5984-11d3-a606-005004775ab1
#define uuid_IVsQueryEditQuerySave3					53474C4D-6279-30c2-b4b4-005004775ab4
#define uuid_IVsTrackProjectDocuments2				53474C4D-6639-11d3-a60d-005004775ab1
#define	uuid_IVsTrackProjectDocuments3				53474C4D-9097-4325-9270-754EB85A6351
#define uuid_IVsTrackProjectDocumentsEvents2		53474C4D-663d-11d3-a60d-005004775ab1
#define	uuid_IVsTrackProjectDocumentsEvents3		53474C4D-BD74-4D21-A79F-2C190E38AB6F
#define uuid_IVsSccProviderFactory					53474C4D-03f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectProviderBinding			53474C4D-02f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectEnlistmentFactory			53474C4D-00f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectEnlistmentChoice			53474C4D-06f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccEnlistmentPathTranslation		53474C4D-01f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectFactoryUpgradeChoice      53544c4d-E3A4-4938-A5EC-6593A79CF27C

#else

#define uuid_IVsSccManager2							53544C4D-B927-4320-B9DA-13D2CB3EA93B
#define uuid_IVsSccManager3							53544C4D-A152-4757-868A-2A7D0F2E5580
#define uuid_IVsSccProject2							53544C4D-AC92-49AC-9172-603E01FA483A
#define uuid_IVsQueryEditQuerySave2					53544C4D-5984-11d3-a606-005004775ab1
#define uuid_IVsQueryEditQuerySave3					53474C4D-6279-30c2-b4b4-005004775ab4
#define uuid_IVsTrackProjectDocuments2				53544C4D-6639-11d3-a60d-005004775ab1
#define	uuid_IVsTrackProjectDocuments3				53544c4d-9097-4325-9270-754EB85A6351
#define uuid_IVsTrackProjectDocumentsEvents2		53544c4d-663d-11d3-a60d-005004775ab1
#define	uuid_IVsTrackProjectDocumentsEvents3		53544c4d-BD74-4D21-A79F-2C190E38AB6F
#define uuid_IVsSccProviderFactory					53544c4d-03f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectProviderBinding			53544c4d-02f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectEnlistmentFactory			53544c4d-00f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectEnlistmentChoice			53544c4d-06f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccEnlistmentPathTranslation		53544c4d-01f8-11d0-8e5e-00a0c911005a
#define uuid_IVsSccProjectFactoryUpgradeChoice      53544c4d-E3A4-4938-A5EC-6593A79CF27C

#endif

#define uuid_SVsSccManager							53544C4D-1927-4320-B9DA-13D2CB3EA93B
#define uuid_SVsQueryEditQuerySave					53544C4D-1984-11d3-a606-005004775ab1
#define uuid_SVsTrackProjectDocuments				53544C4D-1639-11d3-a60d-005004775ab1
