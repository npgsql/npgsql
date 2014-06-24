//////////////////////////////////////////////////////////////////////////////
//
//Copyright Microsoft Corporation.  All Rights Reserved.
//
//File: WCFReferencesIds.h
//
//////////////////////////////////////////////////////////////////////////////

#ifndef _WCFREFERENCEIDS_H_
#define _WCFREFERENCEIDS_H_

// Guids for the Add Service Reference dialog 
//    
//    This provides an extensibility point for packages to add new menu items to the dropdown
//    "discover" button in the Add Service Reference dialog.  The drop-down menu for this
//    button is a standard visual studio context menu with the ID 
//    "IDM_DiscoverServiceReferencesContextMenu".  To add a new entry in this context menu,
//    packages can add new menu groups and menu items to this context menu.  If the user clicks
//    the menu item, the package can display UI, if needed, to obtain input from the user, then
//    perform calculations necessary to obtain a list of "discovered" service urls.  The package
//    should then get the SVsAddWebReferenceDlg3 service from Visual Studio and call the
//    function ShowDiscoveredServicesInCurrentDialog() with that list of urls (see WCFReferences.idl).  
//    They will be displayed in the currently-shown add service reference dialog.  The user can 
//    then select one and click OK to have a reference to the selected service added to the project.


#ifndef _CTC_GUIDS_

// {df6db1a3-d973-4316-bdaa-7e21e9677f09}
DEFINE_GUID(guidAddServiceReferenceDialog,
		0xdf6db1a3, 0xd973, 0x4316, 0xbd, 0xaa, 0x7e, 0x21, 0xe9, 0x67, 0x7f, 0x09);

#else //_CTC_GUIDS_

#define guidAddServiceReferenceDialog { 0xdf6db1a3, 0xd973, 0x4316, { 0xbd, 0xaa, 0x7e, 0x21, 0xe9, 0x67, 0x7f, 0x09 } }

#endif //_CTC_GUIDS_


// To add a new entry to the "Discover" button, add a new menu group with your menu
//    items to this context menu.
#define IDM_DiscoverServiceReferencesContextMenu         0x1000

// Pre-defined menu group and "Services in Solution" menu ID.
#define IDG_DiscoverServiceReferences_ServicesInSolution 0x1001
#define cmdidDiscoverServicesInSolution                  0x1010

#endif //_WCFREFERENCEIDS_H_
