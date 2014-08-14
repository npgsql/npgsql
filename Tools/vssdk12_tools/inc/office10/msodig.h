/******************************************************************************
	File: msodig.h

	Owner: HAILIU
	Copyright (c): 1999 Microsoft Corp.

	Digitial Signature for Office10
*****************************************************************************/
#pragma once

#ifndef __MSODIG_H__
#define __MSODIG_H__

#include <wincrypt.h>


// flag to pass to IMsoSignatureSetClient::PistgGet to get different storages
// of the document
enum
	{
	msofdsstgLoad = 0,  // the document storage to load any existing signature
						// this is called when the signature tab is initialized
	msofdsstgAdd,       // the document storage to add signatures to, client should
					  	// app should take this opportunity to prompt user for saving
					  	// never saved document (or save dirty document)
	msofdsstgCommit,    // the document storage to write the signatures to
	msofdsstgRemove,    // we are about to delete a signature, ask for an stg that
	                    // is writable.
	};

	
interface IMsoSignature;
interface IMsoSignatureSet;
interface IMsoSignatureSetClient;


#ifndef MSO_NO_INTERFACES
#undef  INTERFACE
#define INTERFACE  IMsoSignature

DECLARE_INTERFACE(IMsoSignature)
{
	// returns TRUE iff the signature is valid wrt the storage passed in
	// MsoFCreateSignatureSet
	MSOMETHOD_(BOOL, FValid)(THIS) PURE;

	// use the method to get the name of the singer
	// [out] pwz --> *pwz is the name of the signer. *pwz could point to an internal
	//               buffer of the signature object, so the caller should 
	//               NOT change, release, realloc the pointer.
	// the method returns the number of chars (excluding NULL) of *pwz.
	MSOMETHOD_(UINT, CchGetWzSigner)(THIS_ LPCWSTR *pwz) PURE;

	// use the method to get the name of the issuer
	// [out] pwz --> *pwz is the name of the issuer. *pwz could point to an internal
	//               buffer of the signature object, so the caller should 
	//               NOT change, release, realloc the pointer.
	// the method returns the number of chars (excluding NULL) of *pwz.
	MSOMETHOD_(UINT, CchGetWzIssuer)(THIS_ LPCWSTR *pwz) PURE;

	// returns the expiration date of the signature certificate in a FILETIME.
	// The caller should not modify the returned FILETIME, since this may come
	// from the signature's internal data structure
	MSOMETHOD_(const FILETIME*, PftExpire)(THIS) PURE;

	// the user can choose to attach the signature certificate to the signed doc
	// Use this method to find out whether user has choose to do so.
	MSOMETHOD_(BOOL, FAttachCertGet)(THIS) PURE;

	// change the attach signature certificate properpty.
	// [in] fAttach --> TRUE to attach certificate
	// returns the old property value.
	MSOMETHOD_(BOOL, FAttachCertSet)(THIS_ BOOL fAttach) PURE;

	// The client app can attach a DWORD value to be saved with a signature.
	// This value can be used to keep versioning info. For example, Word 11 may
	// decide to hash plain-text view in addition to the disk file image. Word 11
	// still needs to verify signature created by word 10, so word 11 must be
	// able to tell v10 signature and v11 signature apart. Word 10 can set
	// dwVersion to 1, and word11 can set the Version to 2.
	//
	// The default value of the Version is 0
	//
	// DwVersionSet returns the old Version value
	MSOMETHOD_(DWORD, DwVersionGet)(THIS) PURE;
	MSOMETHOD_(DWORD, DwVersionSet)(THIS_ DWORD dwVersion) PURE;
};
#endif // MSO_NO_INTERFACES


#ifndef MSO_NO_INTERFACES
#undef  INTERFACE
#define INTERFACE  IMsoSignatureSet

DECLARE_INTERFACE(IMsoSignatureSet)
{
	// Standard FDebugMessage method, with ppv passed in lParam.
	MSODEBUGMETHOD

	// The client app call this method to release the signature set.
	// The client's IMsoSignatureSetClient::Release will be called.
	// Any IMsoSignature created thru this signature set is no longer
	// usable after this method returns.
	MSOMETHOD_(VOID, Release)(THIS) PURE;

	// Return the number of signatures in this signature set. This
	// incluldes all previously exsiting (last save) signatures (
	// valid and invalid), and all the new signatures added in the
	// current session.
	MSOMETHOD_(UINT, CSignature)(THIS) PURE;

	// Use this method get any signature in this signature set. 
	// [in] dwIndex --> 0 <= dwIndex < CSignature
	// [out] pps --> if successful, *pps is the signature object specified
	//               by dwIndex
	// The client app can use this method to enumerate signatures in a set
	// For example:
	//     dwIndex = 0;
	//     while (piSignatureSet->FGetSignature(dwIndex, &pis))
	//        {
	//        if (!pis->FValid())
	//           Alert("Signaure not valid");
	//        dwIndex++;
	//        }
	// NOTE: the client should not call FAddSignature during the enumeration
	//       loop. The meaning of the dwIndex is getting changed when signature
	//       is added. This may cause infinite loop.
	MSOMETHOD_(BOOL, FGetSignature)(THIS_ DWORD dwIndex,
		interface IMsoSignature **pps) PURE;

	// Use this method to let the user choose a signature to add. This method
	// brings up a modal dialog with a list of signatrue certificate the user can
	// choose to add the document. The user can bring up sub dialogs from this dialog.
	// For example, the "view" button brings up the certficate view dialog; the "wizard"
	// button brings up the certificate wizard (so user can manage his certificate store)
	//
	// [in] hwnd --> The parent window of the dialog
	// [out] pps --> If user chose a signature certificate and click ok, *pps is the
	//               the signature object of that the user chosed. Otherwize *pps == NULL
	// If user ok the dialog, FAddSignature returns TURE, and *pps is the signature chosen.
	// If user cancel the dialog, FAddSignature returns TRUE, but *pps is NULL.
	// If there is an error, FAddSignature return FALSE.
	MSOMETHOD_(BOOL, FAddSignature)(THIS_ HWND hwnd,
		interface IMsoSignature **pps) PURE;

	// Call this method to commit all the signatures to the storage. The signatures are
	// serialized into a stream named "_signatures" (not localized) under the root of the
	// storage. Only valid signatures are serialized, all invalid signatures are discarded.
	// [in] pistg --> the root storage that the signature stream will live. If there is a
	//                signatrue stream under the stg already, it is overwritten. This stg
	//                can be different from the one used to create the signature set, this
	//                so because on Save, the client app may save the doc to a tmp file and then
	//                copy the tmp file over the original. In this case, pistg here should point
	//                to the temp file
	// return value --> S_OK the signatures were commited
	//              --> S_FALSE the document is dirty and HrCommit did nothing so we don't lose
	//                  any previously existing signatures
	MSOMETHOD(HrCommit)(THIS_ LPSTORAGE pistg) PURE;

	// The client should only call this method in its implementation of FProcessExtraData.
	// See comments below for IMsoSignatureSetClient::FProcessExtraData for reaons why this
	// is so.
	// [in] pb --> poniter to the extra data to hash
	// [in] cb --> number of bytes to hash
	// return TRUE if successful.
	MSOMETHOD_(BOOL, FHashExtraData)(THIS_ PBYTE pb, UINT cb) PURE;

	// choose whether newly added signatures for this set will have their certificate
	// chain attached when the HrCommit is called. This is used by the signature dialog.
	// It only effects signatures that has been added to this point. It doesn't effect
	// signatures added after AttachCertForNewItems is called
	MSOMETHOD_(VOID, AttachCertForNewItems)(THIS_ BOOL fAttach) PURE;

	// delete the signature from the set
	// [in] pis --> the signature to delete, after this function returns the
	// IMsoSignature pointed by pis is undefined and you should not call it
	// any more.
	MSOMETHOD(HrDelete)(THIS_ IMsoSignature* pis) PURE;

	// only called by IMsoSignatureSetClient::HrFilterStream,when the client
	// wishes to filter out some bytes in the stream
	MSOMETHOD_(BOOL, FHashBytes)(THIS_ HANDLE hash, LPBYTE pb, DWORD cb) PURE;

	// Called when user wants to sign a dirty doc. In this case, the client
	// need to re-save the doc can call this method to gives us the new doc
	// storage. If the office code decides that the stg doesn't change, it returns
	// S_FALSE, if the stg does change, returns S_OK. If failed to reset, returns
	// FAILURE code (<0)
	MSOMETHOD(HrResetStgForSigning)(THIS_ IStorage *pistg) PURE;

	// Called by the dlg proc to remove all the signatures added/removed during the
	// current dlg session.
	MSOMETHOD(HrRevert)(THIS) PURE;

	// For ppt read-only scenario. ppt needs to close the document stg after the open.
	// To use this method, ppt must returns FALSE in its
	// IMsoSignatureSetClient::FReleaseStorageRef so we will know to create a copy of
	// the storage ourselves during the creation of the signature set. Ppt can then
	// cal HrHandsOffStorage and we will release our reference on the storage.
	MSOMETHOD(HrHandsOffStorage)(THIS) PURE;
};
#endif // MSO_NO_INTERFACES


enum
	{
	msodigevtSSDispFreed = 0, // let the client know that the dispatch object
							  // on the signature set is going to be freed.
							  // the client  should invalidate any cached pointer
							  // to this object
	msodigevtSSCommited,      // fired when the signature set is commited
	msodigevtMax
	};
	
#ifndef MSO_NO_INTERFACES
#undef  INTERFACE
#define INTERFACE  IMsoSignatureSetClient

DECLARE_INTERFACE(IMsoSignatureSetClient)
{
	// office no longer needs the client interface
	// [in] pvClient --> pvClient passed in MsoFCreateSignatureSet
	MSOMETHOD_(VOID, Release)(THIS_ LPVOID pvClient) PURE;

	// the client app should return FALSE iff the doc in question has not changed
	// (excluding changes to the signature stream, which the client should have
	// no knowledge of) since MsoFCreateSignatureSet. If the client returns TURE,
	// all signatures (newly added or existing) on the doc are invalidated. If the client
	// then commits the signature set, all signature data will be discarded. To add
	// signature back to the doc, the client must create a new signature set object.
	//
	// Note to implementer:
	// This should be fast (preferrably a bit check) since it will get called
	// a lot.
	MSOMETHOD_(BOOL, FContentChanged)(THIS_ LPVOID pvClient) PURE;

	// the signature set will call this method to get the doc's storage 
	// [in] fdsstg --> one of msofdsstgLoad, msofdsstgAdd, msofdsstgCommit
	// [in] pvClient --> client specific data
	MSOMETHOD_(LPSTORAGE, PistgGet)(THIS_ DWORD fdsstg, LPVOID pvClient) PURE;

	// before office starts enumerate the storage, it calls FReleaseStorageRef with
	// fRelease to TRUE. The client should release all the exclusive references it has
	// on that storage (except the root stg) so the office code can open the children
	// elements of the storage. Once office code is done, it calls FReleaseStorageRef
	// with fRelease set to FALSE
	MSOMETHOD_(BOOL, FReleaseStorageRef)(THIS_ BOOL fRelease, LPVOID pvClient) PURE;

	// In addition to hashing the bits in the storage. Office code will calls this
	// method to ask the client app for other data to hash. For example, word can
	// hash the plain text of the document for added security. The client should call
	// IMsoSignatureSet::FHashExtraData to send the data back to the signature set.
	// This method is called once for each different hash algorithm for existing 
	// signatures, and once per newly added signature.
	//
	// [in] pis --> signature in question, the client app may want to call
	//              pis->DwVersionGet to check version info and call
	//              IMsoSignatureSet::FHashExtraData accordingly
	MSOMETHOD_(BOOL, FProcessExtraData)(THIS_ IMsoSignature *pis, LPVOID pvClient) PURE;

	// during file properties dialog, the office code will create a signature set
	// on-behalf of the client app. The following 2 methods, let office keep track
	// of the signature created.
	MSOMETHOD_(VOID, SetSignatureSet)(THIS_ IMsoSignatureSet* piss, LPVOID pvClient) PURE;
	MSOMETHOD_(IMsoSignatureSet*, GetSignatureSet)(THIS_ LPVOID pvClient) PURE;

	// get the msoinst of from the client
	MSOMETHOD_(HMSOINST, Hmsoinst)(THIS_ LPVOID pvClient) PURE;

	// get the parent window from which we will put up the signature dialog
	MSOMETHOD_(HWND, HwndParentForDlg)(THIS_ LPVOID pvClient) PURE;

	// give the client the opportunity to filter the stream we are about to hash
	// client should return E_NOTIMPL if it is not interested in filter the stream.
	// otherwise, client should read the stream, filtering out unnecessary bytes 
	// and send the bytes to IMsoSignatureSet::HashBytes with the hash handle
	// XL will filter out the current user info in the workbook stream.
	// PPT will skip the pptuser stream altogether
	MSOMETHOD(HrFilterStream)(THIS_ LPSTREAM pistm, LPCWSTR cwzStmName,
		HANDLE hash, LPVOID pvClient) PURE;

	// event notification
	MSOMETHOD_(VOID, NotifyEvent)(THIS_ DWORD dwEvt, LPVOID pvEvent, LPVOID pvClient) PURE;
};
#endif // MSO_NO_INTERFACES


/*-----------------------------------------------------------------------------
	MsoFCreateSignatureSet

	We only sign storage. All the functionality is accessed by the client app 
	through a signature set object. This is the API the client app will call to
	create a signature set object.

	[in] pistg --> the storage that the client wants to sign and or verify exiting
				 	signatures.
	[in] pissc --> the signature client interface implemented by the client app
	[in] pvClient --> client specific data structure, this is passed back to each
					call to the signature set client interface.
	[out] ppss --> if successful, *ppss is the signature set created, otherwise
					*ppss is not defined.
	return TRUE if successful, FALSE otherwise. If return FALSE, *ppiss is
	undefined.

	The caller is responsible to call IMsoSignatureSet::Release when
	it is done with the signature set object.

	The caller is responsible to call IMsoSignatureSet::FDebugMessage during mem leak
	checking.
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFCreateSignatureSet(LPSTORAGE pistg, IMsoSignatureSetClient *pissc,
	LPVOID pvClient, IMsoSignatureSet **ppiss);

	
/*-----------------------------------------------------------------------------
	MsoHrCreateSignatureSetDisp

	Similar to MsoFCreateSignatureSet except that this API creates an IDispatch
	object that is used to implement the object model. 
	TODO (hailiu): The exact object model is still TBD.
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(HRESULT) MsoHrCreateSignatureSetDisp(LPSTORAGE pistg,
	IMsoSignatureSetClient *pissc, LPVOID pvClient, IUnknown *punkParent,
	IDispatch **ppdisp);


/*-----------------------------------------------------------------------------
	MsoFStgHasSignature

	return TRUE if the passed pistg has an immediate child stream named
	"_signatures". This means that the doc based on this stg has
	digital signature info with it. This doesn't indicate the validness of those
	info.

	NOTE: The API has to open the sub stream in order to check its existence.
	It is suggested that the client app cache the return result and avoid calling
	it many times for performance reasons
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFStgHasSignature(LPSTORAGE pistg);


/*-----------------------------------------------------------------------------
	MsoPissFromPdispSigSet

	Given a IDispatch returned from MsoHrCreateSignatureSetDisp, return the
	IMsoSignatureSet interface pointer contained in the IDipatch object.

	Note: no addref is called on the IDispatch object, and the client should
	not call IMsoSignatureSet::Release on the returned pointer, because this
	will cause the IDispatch object to point to freed memory. Instead, use
	AddRef/Release on the IDispatch interface pointer
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(IMsoSignatureSet*) MsoPissFromPdispSigSet(IDispatch *pdisp);



/*-----------------------------------------------------------------------------
	MsoGetSignatrueStmName

	wz must be at least long enough to fit vcwzSignatureStm in fundig.cpp
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(void) MsoGetSignatrueStmName(LPWSTR wz);



#if VSMSODEBUG
/*-----------------------------------------------------------------------------
	MsoFDebugSignatureSetDisp

	for mem leak detection
	[in] pidisp --> must be returned from previous call to 
					MsoHrCreateSignatureSetDisp
-------------------------------------------------------------------- HAILIU -*/
MSOAPI_(BOOL) MsoFDebugSignatureSetDisp(IDispatch *pidisp,
	HMSOINST hinst, UINT uMsg, WPARAM wparam, LPARAM lparam);
#else
#define MsoFDebugSignatureSetDisp(pdisp, hinst, uMsg, wparam, lparam) 
#endif // VSMSODEBUG

#endif // __MSODIG_H__
