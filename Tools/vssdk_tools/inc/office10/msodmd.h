#pragma once

///////////////////////////////////////////////////////////////////////////
// Mso Doc Management debug-only declarations.
//

#ifndef __INDEX_DUMP__STRUCT__
#define __INDEX_DUMP__STRUCT__

// Structure and index dump type for custom index specification
// Index dump types. In a Custom index dump, the user has selected the properties
// to be dumped out and their localized string names are stored in the structIndexDump
// structure.
typedef enum _MsoIndexDumpType
{
	Custom	=	0,
	Complete,
	OfficeProps_Only,
	NonOfficeProps_Only,
	TextProps_Only,
	DateProps_Only,
	NumericProps_Only,
	No_Contents,
	Contents_Only
} MsoIndexDumpType;

// Structure that holds criteria for limiting index dumps
typedef struct _MsoIndexDump {

    BOOL     fConvertToASCII;           // if true, output in ASCII rather than in Unicode

	BOOL	 fIncludeDocLists;			// whether or not include document lists for each prop
	BOOL	 fIncludeDocNames;			// whether or not to include document names
	BOOL	 fShowFullPath;				// whether or not show full path for the indexed files

	BOOL	 fIncludeCounts;			// whether or not print out occurence counts

	BOOL	 fIncludeEmptyVals;			//  .....  property names & counts for empty values


	MsoIndexDumpType idType;

	int		wcSelectedProps;			// number of properties selected for customized index dump

	WCHAR	**wzLocPropNames;			// array of pointers to localized property
										// names for properties to be dumped out (if
										// dump type is custom)

	WCHAR	*wzLowerLimit;				// lower range limit for text values
	WCHAR	*wzUpperLimit;				// upper range limit; if NULL, only the values
										// that are equal or extensions of m_wzLowerLimit are dumped out

    BOOL	fMatchWholeWordOnly;		// if this flag is set, AND m_wzUpperLimit is NULL, then
										// only values that are equal to m_wzLowerLimit are dumped out
} MsoIndexDump;



#endif // __INDEX_DUMP__STRUCT__
