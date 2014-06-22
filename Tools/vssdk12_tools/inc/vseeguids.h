/*-----------------------------------------------------------------------------
Microsoft VSEE

Copyright 1995-2000 Microsoft Corporation. All Rights Reserved.

@doc
@module VseeGuids.h - Guids for VSEE services/interfaces |
This is included by .idl files to define guids for VSEE services/interfaces.
These are the non-shell-SDK interfaces, in idl/vsee

@owner Source Control Integration Team
-----------------------------------------------------------------------------*/
#pragma once

#ifdef _WIN64

#define uuid_IVsSccManager							53474C4D-0F05-4735-8AAC-264109CF68AC
#define uuid_IVsSccProject							53474C4D-CD19-4928-A834-AFCD8A966C36
#define uuid_IVsQueryEditQuerySave					53474C4D-7E28-4d0c-A00F-3446801350CE
#define uuid_IVsTrackProjectDocuments				53474C4D-449A-4487-A56F-740CF8130032
#define uuid_IVsTrackProjectDocumentsEvents			53474C4D-A98B-4fd3-AA79-B182EE26185B
#define uuid_IVsSccEngine							53474C4D-F82C-11d0-8D84-00AA00A3F593
#define uuid_IVsSccPopulateList						53474C4D-F8CF-11d0-8D84-00AA00A3F593
#define uuid_IVsSccToolsOptions						53474C4D-304B-4D82-AD93-074816C1A0E5
#define uuid_IVsSccManagerTooltip					53474C4D-DF28-406D-81DA-96DEEB800B64


#else

#define uuid_IVsSccManager							53544C4D-0F05-4735-8AAC-264109CF68AC
#define uuid_IVsSccProject							53544C4D-CD19-4928-A834-AFCD8A966C36
#define uuid_IVsQueryEditQuerySave					53544C4D-7E28-4d0c-A00F-3446801350CE
#define uuid_IVsTrackProjectDocuments				53544C4D-449A-4487-A56F-740CF8130032
#define uuid_IVsTrackProjectDocumentsEvents			53544C4D-A98B-4fd3-AA79-B182EE26185B
#define uuid_IVsSccEngine							53544C4D-F82C-11d0-8D84-00AA00A3F593
#define uuid_IVsSccPopulateList						53544C4D-F8CF-11d0-8D84-00AA00A3F593
#define uuid_IVsSccToolsOptions						53544C4D-304B-4D82-AD93-074816C1A0E5
#define uuid_IVsSccManagerTooltip					53544C4D-DF28-406D-81DA-96DEEB800B64

#endif

#define uuid_SVsSccToolsOptions						53544C4D-104B-4D82-AD93-074816C1A0E5

