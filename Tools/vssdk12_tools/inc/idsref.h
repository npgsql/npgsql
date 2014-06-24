//-----------------------------------------------------------------------------
//
// Microsoft Visual Database Tools
//
// Copyright 1994 - 2000 Microsoft Corporation. All Rights Reserved.
//
// IDSRefProvider and IDSRefConsumer COM interface definitions
//
//-----------------------------------------------------------------------------

#ifndef _IDSREF_H_a1da37ec_0fb1_4450_ab38_7dc92534142d
#define _IDSREF_H_a1da37ec_0fb1_4450_ab38_7dc92534142d


// Types and Constants --------------------------------------------------------

// DSRef tree and node type information flags
enum DSREFTYPETag
{
	// Type information only available on DSRef tree root node
	DSREFTYPE_NULL				= 0x00,			// Tree has no type information
	DSREFTYPE_COLLECTION		= 0x01,			// Tree has multiple root nodes
	DSREFTYPE_MULTIPLE			= 0x02,			// Tree has multiple leaf nodes
	DSREFTYPE_MIXED				= 0x04,			// Multiple parents have leaf nodes

	// Type information only available on true DSRef tree nodes (i.e. database, table, etc.)
	DSREFTYPE_DATASOURCEROOT	= 0x10,			// Data source root node

	// Type information available on both DSRef tree root node and true nodes
	DSREFTYPE_FIELD				= 0x00000100,	// Field node
	DSREFTYPE_TABLE				= 0x00000200,	// Table node
	DSREFTYPE_QUERY				= 0x00000400,	// Query node (not a db object)
	DSREFTYPE_DATABASE			= 0x00000800,	// Database node
	DSREFTYPE_TRIGGER			= 0x00001000,	// Trigger node
	DSREFTYPE_STOREDPROCEDURE	= 0x00002000,	// Stored Procedure node
	DSREFTYPE_EXTENDED			= 0x00004000,	// Extended node
	DSREFTYPE_SCHEMADIAGRAM		= 0x00008000,	// Schema diagram
	DSREFTYPE_VIEW				= 0x00100000,	// View node
	DSREFTYPE_SYNONYM			= 0x00800000,   // Synonym node
	DSREFTYPE_FUNCTION			= 0x01000000,   // Function node
	DSREFTYPE_PACKAGE			= 0x02000000,   // Package node
	DSREFTYPE_PACKAGEBODY		= 0x04000000,   // Package body node
	DSREFTYPE_RELATIONSHIP		= 0x08000000,	// Relationship node
	DSREFTYPE_INDEX				= 0x10000000,	// Index node
	DSREFTYPE_USERDEFINEDTYPE	= 0x20000000,	// User Defined Type node
	DSREFTYPE_VIEWTRIGGER		= 0x40000000,	// ViewTrigger node
	DSREFTYPE_VIEWINDEX  		= 0x80000000,	// ViewIndex node


	// Type information used for persistence - internal use only
	// Clients of DSRef should not use these bit flags for determining
	// DSRef structure and properties.
	DSREFTYPE_HASFIRSTCHILD		= 0x00010000,	// Node has first child
	DSREFTYPE_HASNEXTSIBLING	= 0x00020000,	// Node has next sibling
	DSREFTYPE_HASNAME			= 0x00040000,	// Node has name
	DSREFTYPE_HASMONIKER		= 0x00080000,	// Node has moniker
	// DSREFTYPE_VIEW above uses  0x00100000
	DSREFTYPE_HASOWNER			= 0x00200000,	// Node has owner property
	DSREFTYPE_HASPROP			= 0x00400000,	// Node has additional properties
	// DSREFTYPE_SYNONYM - DSREFTYPE_PACKAGEBODY above use 0x00800000 - 0x04000000

};
typedef UINT	DSREFTYPE;	// Hungarian: grf

// Bit mask for node data source element type
#define DSREFTYPE_NODE	(DSREFTYPE_FIELD | \
						 DSREFTYPE_TABLE | \
						 DSREFTYPE_QUERY | \
						 DSREFTYPE_VIEW | \
						 DSREFTYPE_DATABASE | \
						 DSREFTYPE_TRIGGER | \
						 DSREFTYPE_STOREDPROCEDURE | \
						 DSREFTYPE_EXTENDED | \
						 DSREFTYPE_SCHEMADIAGRAM | \
						 DSREFTYPE_SYNONYM | \
						 DSREFTYPE_FUNCTION | \
						 DSREFTYPE_PACKAGE | \
						 DSREFTYPE_PACKAGEBODY | \
						 DSREFTYPE_RELATIONSHIP | \
						 DSREFTYPE_INDEX | \
						 DSREFTYPE_USERDEFINEDTYPE | \
						 DSREFTYPE_VIEWTRIGGER | \
						 DSREFTYPE_VIEWINDEX)

// DSRef node NodeID
#define DSREFNODEID_ROOT	0
#define DSREFNODEID_NIL		0
typedef VOID	*DSREFNODEID;	// Hungarian: drnid




//-----------------------------------------------------------------------------
// Name: IDSRefProvider
//
// Description:
// IDSRefProvider COM interface definition
//
// Thread-Safety: None
//-----------------------------------------------------------------------------
DECLARE_INTERFACE_(IDSRefProvider, IUnknown)
{
	// IUnknown methods
	STDMETHOD(QueryInterface) (REFIID, LPVOID *) PURE;
	STDMETHOD_(ULONG, AddRef) (VOID) PURE;
	STDMETHOD_(ULONG, Release) (VOID) PURE;

	// IDSRefProvider methods
	STDMETHOD(Clear) (VOID) PURE;
	STDMETHOD(CreateFirstChildNode) (
		DSREFNODEID drnidCurr,
		DSREFNODEID *pdrnidChild) PURE;
	STDMETHOD(CreateNextSiblingNode) (
		DSREFNODEID drnidCurr,
		DSREFNODEID *pdrnidSibling) PURE;
	STDMETHOD(SetType) (
		DSREFNODEID drnidCurr,
		DSREFTYPE grfType) PURE;
	STDMETHOD(SetExtendedType) (
		DSREFNODEID drnidCurr,
		const GUID *pguidType) PURE;
	STDMETHOD(SetName) (
		DSREFNODEID drnidCurr,
		BSTR bstrName) PURE;
	STDMETHOD(SetMoniker) (
		DSREFNODEID drnidCurr,
		IMoniker *pIMoniker) PURE;
	STDMETHOD(SetOwner) (
		DSREFNODEID drnidCurr,
		BSTR bstrOwnerName) PURE;
	STDMETHOD(SetProperty) (
		DSREFNODEID drnidCurr,
		REFGUID guidProp,
		VARIANT varPropValue) PURE;

};
typedef IDSRefProvider *PDSREFPROVIDER;




//-----------------------------------------------------------------------------
// Name: IDSRefConsumer
//
// Description:
// IDSRefConsumer COM interface definition
//
// Thread-Safety: None
//-----------------------------------------------------------------------------
DECLARE_INTERFACE_(IDSRefConsumer, IUnknown)
{
	// IUnknown methods
	STDMETHOD(QueryInterface) (REFIID, LPVOID *) PURE;
	STDMETHOD_(ULONG, AddRef) (VOID) PURE;
	STDMETHOD_(ULONG, Release) (VOID) PURE;

	// IDSRefConsumer methods
	STDMETHOD(GetVersion) (DWORD *pdwVersion) PURE;
	STDMETHOD(GetTimestamp) (FILETIME *pftTimestamp) PURE;
	STDMETHOD(GetFirstChildNode) (
		DSREFNODEID drnidCurr,
		DSREFNODEID *pdrnidChild) PURE;
	STDMETHOD(GetNextSiblingNode) (
		DSREFNODEID drnidCurr,
		DSREFNODEID *pdrnidSibling) PURE;
	STDMETHOD(GetType) (
		DSREFNODEID drnidCurr,
		DSREFTYPE *pgrfType) PURE;
	STDMETHOD(GetExtendedType) (
		DSREFNODEID drnidCurr,
		GUID *pguidType) PURE;
	STDMETHOD(GetName) (
		DSREFNODEID drnidCurr,
		BSTR *pbstrName) PURE;
	STDMETHOD(GetMoniker) (
		DSREFNODEID drnidCurr,
		IMoniker **ppIMoniker) PURE;
	STDMETHOD(GetOwner) (
		DSREFNODEID drnidCurr,
		BSTR *pbstrOwnerName) PURE;
	STDMETHOD(GetProperty) (
		DSREFNODEID drnidCurr,
		REFGUID guidProp,
		VARIANT *varPropValue) PURE;

};
typedef IDSRefConsumer *PDSREFCONSUMER;

// DSRef clipboard format text name
const TCHAR			szcfDSRef[] = TEXT("CF_DSREF");

#endif	// _IDSREF_H_a1da37ec_0fb1_4450_ab38_7dc92534142d
