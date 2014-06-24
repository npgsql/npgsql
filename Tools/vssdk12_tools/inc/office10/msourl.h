/****************************************************************************
	msourl.h

	Owner: SeanMc
	Copyright (c) 1999 Microsoft Corporation

	MSO's url handling interfaces.
****************************************************************************/

#pragma once

#ifndef MSOURL_H
#define MSOURL_H

#ifndef CONST_METHOD
// to declare const methods in C++
#if defined(__cplusplus) && !defined(CINTERFACE)
	#define CONST_METHOD const
#else
	#define CONST_METHOD
#endif
#endif

#include "wininet.h"
#include "msobp.h"

// list of schemes recognized via MsoUrl
typedef enum
	{
	urlsNil = -1,
	urlsHttp,             // "http"
	urlsHttps,            // "https"
	urlsFtp,              // "ftp"
	urlsFile,             // "file"
	urlsCid,              // "cid"
	urlsMailto,           // "mailto"
	urlsJavascript,       // "javascript"
	urlsOutlook,          // "outlook"
	urlsODMA,             // "::ODMA\"
	urlsMhtmlCompound,    // "mhtml:X!Y"
	urlsThismessage,      // "thismessage"
	urlsRes,              // "res"
	urlsMk,               // "mk"
	urlsUnknown
	} MSOURLSCHEME;


// list of relativities understood via MsoUrl
typedef enum
	{
	urlrNil = -1,
	urlrOpaque,           // EXAMPLE: "mailto:someone@somewhere.com"
	urlrAbsolute,         // EXAMPLE: "http://mumble.com/woo.hoo"
	urlrServer,           // EXAMPLE: "/foo/bar/woo.hoo"
	urlrPage              // EXAMPLE: "../foo/bar/woo.hoo"
	} MSOURLRELATIVITY;


/* URLC (Url Component) flags, passed to IMsoUrl::HrGetCustomForm & IMsoUrl::FComponentsAreEqual. */
#define msofurlcScheme   0x0001
#define msofurlcUserName 0x0002
#define msofurlcPassword 0x0004
#define msofurlcServer   0x0008
#define msofurlcPort     0x0010
#define msofurlcDir      0x0020
#define msofurlcFileName 0x0040
#define msofurlcFileExt  0x0080
#define msofurlcQuery    0x0100
#define msofurlcFragment 0x0200
#define msofurlcMhtml	 0x0400

#define msofurlcAuthority (msofurlcUserName | msofurlcPassword | msofurlcServer | msofurlcPort)
#define msofurlcFileLeaf  (msofurlcFileName | msofurlcFileExt)
#define msofurlcPath      (msofurlcDir | msofurlcFileLeaf)
#define msofurlcComplete  (msofurlcScheme | msofurlcAuthority | msofurlcPath | msofurlcQuery | msofurlcFragment | msofurlcMhtml)

/* URL flags, passed to IMsoUrl::HrSetFromUser[Rgwch]. */
#define msofurlNoFragment               0x00000001  // don't treat the '#' character as a fragment delimiter
#define msofurlStripEncapsulatingDelims 0x00000002  // strip off quotes or angle brackets encapsulating the url (if present)
#define msofurlConvertDriveMappedUNC    0x00000004  // convert drive mapped UNCs to pure UNC form
#define msofurlConvertToShortFileName   0x00000008  // convert any local paths to 8.3 form
#define msofurlConvertToLongFileName    0x00000010  // convert any local paths to long filename form
#define msofurlEscapeSingleQuotes       0x00000020  // escape any single quotes found in the url
#define msofurlEscapeAllPercents        0x00000040  // escape all percents (normally percents in %HH sequences are not escaped)
#define msofurlStripLeadingWhitespace   0x00000080  // strip off leading whitespace (if present)
#define msofurlStripTrailingWhitespace  0x00000100  // strip off trailing whitespace (if present)
                                                    // if present with msofurlStripEncapsulatingDelims, strips both inside and outside delims
#define msofurlApplyGuessScheme         0x00000200  // apply best guess scheme if none exists on the url
#define msofurlApplyDefaultScheme       0x00000400  // apply the default scheme if none exists on the url
#define msofurlCaseInsensitivePath      0x00000800  // use case-insensitive comparisons on the path component
#define msofurlEnsureTrailingPathSep    0x00001000  // ensure the path component includes a trailing slash (indicating it is a directory)
#define msofurlRemoveTrailingPathSep    0x00002000  // ensure the path component does not have a trailing slash (indicating a file leaf or empty path)
#define msofurlAllowInvertedLocalSep    0x00004000  // let "x:/" map to { scheme:"file", path:"x:\" }, rather than { scheme:"x", path:"/" }
                                                    // also applies the same logic to "//server/share"
#define msofurlDontChangeSlashTypes     0x00008000  // don't change /'s to \'s or vice-versa
#define msofurlNoEscape                 0x00010000  // don't escape this url internally
#define msofurlEnsureValidAuthority     0x00020000  // ensure the authority component is valid if present (non-empty)
#define msofurlNoQuery                  0x00040000  // don't treat the '?' character as a query delimiter
#define msofurlEnsureBalancedQuotes     0x00080000  // ensure no unbalanced quotes at the beginning/end of the URL
                                                    // use with caution: these may be valid in some cases
#define msofurlEscapeAllReservedChars   0x00100000  // escape all reserved characters (any in ";/:@&=+$,") use with care -- creates very odd urls!
#define msofurlDetectASPLinks           0x00200000  // client wants ASP links detected

/* URLDF flags, passed to IMsoUrl::HrGetDisplayForm. */
#define msofurldfUnescapeHighAnsi       0x0001  // unescape high-ANSI entities (use with extreme caution!)
#define msofurldfUseLocalPathForm       0x0002  // use the local path form for local urls (return what HrGetLocalPath would return if the url is local)
#define msofurldfDoNotUnescapeHash      0x0004  // do not unescape the hash character (leave it "%23" instead)
#define msofurldfDoNotUnescapeLowAnsi   0x0008  // do not unescape low-ANSI entities (use with extreme caution!)

/* URLCF flags, passed to IMsoUrl::HrGetCustomForm. */
#define msofurlcfDisplay                0x0001  // get the display form
#define msofurlcf3SlashFileSyntax       0x0002  // use the 3 slash file: syntax
#define msofurlcfUseHideExtShellSetting 0x0004  // use the OS shell setting to hide extensions to include ext in leaf
#define msofurlcfUseLocalPathForm       0x0008  // use the local path form for local urls (return what HrGetLocalPath would return if the url is local)

/* URLE flags, passed to IMsoUrl::FExists. */
#define msofurleCheckServers      0x0001  // validate against the server (if not set then server based paths return FALSE)
#define msofurleFolder            0x0002  // verify that path points to a folder
#define msofurleBrowsable         0x0004  // verify that path points to a browsable folder (requires msofurleFolder)
#define msofurleAllowUI           0x0008  // allow UI to be raised (such as the password authentication dialog)

/* URLU flags, passed to IMsoUrl::HrUpload. */
#define msofurluAlertIfExist       0x0001
#define msofurluFailIfExist        0x0002

/* URLD flags, passed to IMsoUrl::HrDownload. */
#define msofurldUseCachedVersion  0x0001  // if it already exists in the cache, use it (otherwise download it)


/////////////////////////////////////////////////////////////////////////////////
//
// Basic url definitions:
//
// hierarchichal url := <scheme>://<authority>[/<path>][?<query>][#<fragment>]
// authority := [<username>[:<password>]@]<server>[:<port>]
// path := <dir>[/[fileleaf]]
// fileleaf := <filename>[.<fileext>]
//
// opaque url := <scheme>:<path>[?<query>][#<fragment>]
//
// NOTE: every url always has a path (& dir) component (but it may be the empty string)
//
/////////////////////////////////////////////////////////////////////////////////
//
// Mhtml url definitions:
// 
// Only-File form: mhtml:X - where X is an absolute hierarchichal url.
// Compound form: mhtml:X!Y - where X is like above, Y is a CID or CLOC (Content-Location)
//
// NOTE: We do not return Only-File forms for these types of url's, since this can
//       expose a bug in IE which causes these files to be loaded with the wrong codepage.
// NOTE: We only use the mhtml: prefix when we have a compound mhtml url (restating above).
//       Therefore Only-File url's become normal url's when accessed via url methods.
// 
// We fixup REL url's with mhtml url's by doing the following:
// mhtml:X + REL ==> mhtml:X!REL <- Only-File becomes a compound mhtml url
// mhtml:X!Y + REL ==> mhtml:X!(Y+REL)  <- here, Y+REL is the proper combination of these url's as single entities
//
/////////////////////////////////////////////////////////////////////////////////

/* IMsoUrl (iurl) */
#undef  INTERFACE
#define INTERFACE IMsoUrl
DECLARE_INTERFACE_(IMsoUrl, IUnknown)
{
	// ----- IUnknown methods

	MSOMETHOD (QueryInterface) (THIS_ REFIID riid, VOID **ppvObj) PURE;
	MSOMETHOD_(ULONG, AddRef) (THIS) PURE;
	MSOMETHOD_(ULONG, Release) (THIS) PURE;

	// ----- IMsoUrl methods

	/* FDebugMessage method */
	MSODEBUGMETHOD

	// is the URL valid? (was it set properly?)
	MSOMETHOD_(BOOL, FValid) (THIS) CONST_METHOD PURE;

	// set methods
	//
	// HrSetFromUser - use when you have a url from an external source other than another
	//    MsoUrl (for instance from user typing or pasting in from some external source
	//    or a url read in from an html file).  Param grfurl allows the user to specify
	//    special behavior (such as msofurlNoFragment specifying to interpreting the url
	//    as having no fragment).  Default grfurl is 0.
	// HrSetFromUserRgwch - same as HrSetFromUser but takes a rgwch & cch instead of wz.
	// HrSetFromCanonicalUrl - use only when you have a canonical url retrieved from
	//    another MsoUrl.  Primarily only used internally, or maybe when writing out a
	//    url to a cache in the registry for example.  When in doubt, use HrSetFromUser
	//    (which does more robustifying and canonicalizing on the input url).
	//
	// NOTE: each set method also has a param cp which is the codepage context of the
	//    url (i.e. the codepage of the html source file or CP_ACP for something pasted
	//    or the document codepage for something typed in by the user).
	// NOTE: each set method also has a param piurlBase which is a url which represent the
	//    base for resolving this url.  Urls which are absolute or opaque may safely pass
	//    in NULL for the base.  Relative url's should however always have a valid base.  If
	//    a relative url is created without a base, the HrResolve method will assert.
	MSOMETHOD (HrSetFromUser) (THIS_ const WCHAR *wzUrl, DWORD cp, const IMsoUrl *piurlBase, DWORD grfurl) PURE;
	MSOMETHOD (HrSetFromUserRgwch) (THIS_ const WCHAR *rgwchUrl, int cchUrl, DWORD cp, const IMsoUrl *piurlBase, DWORD grfurl) PURE;
	MSOMETHOD (HrSetFromCanonicalUrl) (THIS_ const WCHAR *wzUrl, DWORD cp, const IMsoUrl *piurlBase) PURE;

	// lock/unlock methods
	//
	// Use these when you use methods returning LPCWSTR's to ensure the data is not changed
	//    while you are accessing it.  These methods ensure that no updates are done to the
	//    url while you are using it's data.  If you don't want to use this, use the methods
	//    which copy the url data to a buffer instead (which don't require buffer locking).
	MSOMETHOD_(void, Lock) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(void, Unlock) (THIS) CONST_METHOD PURE;

	// get url methods
	//
	// HrGetDisplayForm - use to get the unescaped "friendly" form of a url for displaying to
	//    the user (i.e. spaces in the url are actually spaces instead of %20s).
	// HrGetCanonicalForm - use to get the canonical form of the url.  This is the standard
	//    form of the url.
	// HrGetCustomForm - use to get a url with or without certain components.  For instance,
	//    use this when you want to get a url without a fragment (if it has one) -- you would
	//    pass in grfurlc as (msofurlcComplete & ~msofurlcFragment).  Note, this returns the
	//    components with their associated delimiters (unlike the component access methods below).
	//
	// NOTE: the HrGet methods all take in a WCHAR buffer wzBuf, whose size is passed in as
	//    the input value of pcchBuf.  The length of the url put in wzBuf is returned as the
	//    output value of pcchBuf.  If NULL is passed in for wzBuf, then the requisite size
	//    of wzBuf is returned in pcchBuf.  In all cases (input/output) pcchBuf is the actual
	//    count of characters in wzBuf not including the NULL termination character.  So if
	//    you query with NULL, you should alloc (*pcchBuf + 1) characters for wzBuf.  If wzBuf
	//    is NULL and the function succeeds, S_FALSE is returned.  If wzBuf is non-NULL and
	//    the function succeeds, S_OK is returned.
	//
	// CpGetCodePage - use to retrieve the codepage context of this url.
	// WzCanonicalForm - returns the same value as HrGetCanonicalForm but returns an LPCWSTR
	//    pointer to the internal buffer rather than putting it in a user specified buffer.
	//    Useful in certain scenarios.  Url must be locked while this function is called and
	//    while the return value is in use.  Returns NULL on error.
	// CchCanonicalForm - returns the count of characters of the url returned by
	//    WzCanonicalForm.
	MSOMETHOD (HrGetDisplayForm) (THIS_ WCHAR *wzBuf, int *pcchBuf, DWORD grfurldf) CONST_METHOD PURE;
	MSOMETHOD (HrGetCanonicalForm) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetCustomForm) (THIS_ DWORD grfurlc, WCHAR *wzBuf, int *pcchBuf, DWORD grfurlcf) CONST_METHOD PURE;
	MSOMETHOD_(DWORD, CpGetCodePage) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, WzCanonicalForm) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(int, CchCanonicalForm) (THIS) CONST_METHOD PURE;

	// component accessor methods
	//
	// Note: see "Basic url definition" comment above for component definitions.
	//
	// UrlsGetScheme - returns the scheme specified in this url as an MSOURLSCHEME.
	// HrGetScheme - returns the scheme string (without ':' delimiter).
	// HrGetAuthority - returns the authority string
	//    (with all subcomponent delimiters but without initial '/' delimiters).
	// HrGetUserName - returns the username string (without '@' delimiter).
	// HrGetPassword - returns the password string (without ':' delimiter).
	// HrGetServer - returns the server string.
	// HrGetPort - returns the port string (without ':' delimiter).
	// HrGetPath - returns the path string (with the initial '/' delimiter if this
	//    is a hierarchical url).  This may validly return the empty string.
	//    (Path contains Dir & FileLeaf).
	// HrGetDir - returns the dir string (with the initial '/' delimiter if this
	//    is a hierarchical url).  This may validly return the empty string.
	// HrGetFileLeaf - returns the file leaf string (without '/' delimeter).
	//    (FileLeaf contains FileName & FileExt).
	// HrGetFileName - returns the file name string (without '/' delimeter).
	// HrGetFileExt - returns the file ext string (without '.' delimiter).
	// HrGetQuery - returns the query string (without '?' delimiter).
	// HrGetFragment - returns the fragment string (without '#' delimiter).
	//    (Fragment is also known as Bookmark).
	//
	// NOTE: see HrGet url note above for usage of the HrGet component methods.
	//
	// RgwchScheme - same as HrGetScheme but returns a pointer to the scheme component.
	// RgwchAuthority - same
	// RgwchUserName - same
	// RgwchPassword - same
	// RgwchServer - same
	// RgwchPort - same
	// RgwchPath - same
	// RgwchDir - same
	// RgwchFileLeaf - same
	// RgwchFileName - same
	// RgwchFileExt - same
	// RgwchQuery - same
	// RgwchFragment - same
	//
	// NOTE: the Rgwch returning functions all return NULL if the component does not
	//    exist.  Param pcch is an output parameter which will contain the length of
	//    the returned rgwch pointer.  These are not NULL terminated!
	MSOMETHOD_(MSOURLSCHEME, UrlsGetScheme) (THIS) CONST_METHOD PURE;
	MSOMETHOD (HrGetScheme) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetAuthority) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetUserName) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetPassword) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetServer) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetPort) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetPath) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetDir) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetFileLeaf) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetFileName) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetFileExt) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetQuery) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD (HrGetFragment) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchScheme) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchAuthority) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchUserName) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchPassword) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchServer) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchPort) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchPath) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchDir) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchFileLeaf) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchFileName) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchFileExt) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchQuery) (THIS_ int *pcch) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, RgwchFragment) (THIS_ int *pcch) CONST_METHOD PURE;

	// helper functions for file management
	//
	// FIsHttp - does the url point to a http or https address?
	// FIsFtp - does the url point to a ftp address?
	// FIsLocal - does the url point to a dos addressable file address
	//            (local drive, mapped drive or UNC drive)?
	// FIsUNC - does the url point to a UNC file address?
	// FIsODMA - does the url point to an ODMA path identifier?
	MSOMETHOD_(BOOL, FIsHttp) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(BOOL, FIsFtp) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(BOOL, FIsLocal) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(BOOL, FIsUNC) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(BOOL, FIsODMA) (THIS) CONST_METHOD PURE;

	// local path accessor methods (only work for file: url's)
	//
	// HrGetLocalPath - returns the dos path form of the file: url.  Returns error
	//    if the url is not a file: url.  Behaves the same as HrGet url methods.
	// WzLocalPath - returns a pointer to the dos path form of the file: url.  Returns NULL
	//    if the url is not a file: url.  Behaves the same as WzCanonicalForm.
	// CchLocalPath - returns the count of characters of the dos path returned by
	//    WzLocalPath.
	//
	// EXAMPLE 1: "file://c:/mumble/woo.hoo" yields "c:\mumble\woo.hoo".
	// EXAMPLE 2: "file://server/share/mumble/woo.hoo" yields "\\server\share\mumble\woo.hoo".
	MSOMETHOD (HrGetLocalPath) (THIS_ WCHAR *wzBuf, int *pcchBuf) CONST_METHOD PURE;
	MSOMETHOD_(LPCWSTR, WzLocalPath) (THIS) CONST_METHOD PURE;
	MSOMETHOD_(int, CchLocalPath) (THIS) CONST_METHOD PURE;

	// FIsEqual - returns TRUE iff the given url is equivalent to this url.
	//    (Equivalent to calling FComponentsAreEqual with grfurlc as msofurlcComplete).
	MSOMETHOD_(BOOL, FIsEqual) (THIS_ const IMsoUrl *piurl) CONST_METHOD PURE;

	// FComponentsAreEqual - returns TRUE iff the components specifed in grfurlc are equivalent
	//    in this url and the given url.
	MSOMETHOD_(BOOL, FComponentsAreEqual) (THIS_ DWORD grfurlc, const IMsoUrl *piurl) CONST_METHOD PURE;

	// FSubsumes - returns TRUE iff this url's folder location is a direct
	//    hierarchical ancestor of piurl.  If two url's subsume each other, they point to
	//    the same folder location!
	//
	// EXAMPLE 1: "http://mumble/woo/blah.htm" subsumes "http://mumble/woo/hoo/wunder.bar".
	// EXAMPLE 2: "http://mumble/woo/hoo/wunder.bar" does not subsume "http://mumble/woo/blah.htm".
	// EXAMPLE 3: "http://mumble/woo/blah.htm" does not subsume "ftp://foobarbaz/woo/hoo/wunder.bar".
	// EXAMPLE 4: "http://mumble/woo/hoo/blah.htm" does not subsume "http://mumble/woo/zoo/foo.gif".
	MSOMETHOD_(BOOL, FSubsumes) (THIS_ const IMsoUrl *piurl) CONST_METHOD PURE;

	// UrlrGetRelativity - returns the relativity of this url as an MSOURLRELATIVITY.
	MSOMETHOD_(MSOURLRELATIVITY, UrlrGetRelativity) (THIS) CONST_METHOD PURE;

	// HrSetRelativity - updates the relativity of this url.  Returns S_OK when the
	//    url is successfully updated or S_FALSE if the requested url cannot be set
	//    because the base url's server or scheme is different.  Returns error codes
	//    for other failures.
	MSOMETHOD (HrSetRelativity) (THIS_ MSOURLRELATIVITY urlr) PURE;

	// HrResolve - resolves this relative url to a full path (by combining it with its base),
	//    creates a new url and returns it in ppiurlAbsolute.  On success,
	//    *ppiurlAbsolute must be Released by the client when done.  Calling this on an
	//    absolute or opaque url is valid.
	//
	// EXAMPLE 1: this url "foo.bar" combined with base "http://woohoo/sub/hub/spiffy.htm"
	//    yields resolved path "http://woohoo/sub/hub/foo.bar".
	// EXAMPLE 2: this url "/foo.bar" combined with base "http://woohoo/sub/hub/spiffy.htm"
	//    yields resolved path "http://woohoo/foo.bar".
	// EXAMPLE 3: this url "../foo.bar" combined with base "http://woohoo/sub/hub/spiffy.htm"
	//    yields resolved path "http://woohoo/sub/foo.bar".
	MSOMETHOD (HrResolve) (THIS_ IMsoUrl **ppiurlAbsolute) CONST_METHOD PURE;

	// HrRebase - rebase this url with a different base url (i.e. link fixup).
	//    If the new base url is on a different server this just makes the url absolute.
	//    Otherwise it maintains the current relativity state and constructs a new
	//    relative url pointing at the same target location.  If the relativity state
	//    was not maintainable, it returns S_FALSE on success.
	//
	// EXAMPLE 1: this url "foo.bar" with old base "http://woohoo/sub/hub/spiffy.htm"
	//    rebased with new base "http://woohoo/sub/spiffy.htm"
	//    yields this url "hub/foo.bar".
	// EXAMPLE 2: this url "../foo.bar" with old base "http://woohoo/sub/hub/spiffy.htm"
	//    rebased with new base "http://woohoo/sub/spiffy.htm"
	//    yields this url "foo.bar".
	// EXAMPLE 3: this url "/foo.bar" with old base "http://woohoo/sub/hub/spiffy.htm"
	//    rebased with new base "http://woohoo/sub/spiffy.htm"
	//    yields this url "/foo.bar".
	// EXAMPLE 4: this url "foo.bar" with old base "http://woohoo/sub/hub/spiffy.htm"
	//    rebased with new base "http://mumble/spiffy.htm"
	//    yields this url "http://woohoo/sub/hub/foo.bar".
	MSOMETHOD (HrRebase) (THIS_ const IMsoUrl *piurlBase) PURE;

	// HrDownload - downloads this url for local data access.  On success, returns the
	//    local path to the downloaded file in wzBuf.  The size of wzBuf is the
	//    input value of pcchBuf, the length of the path put into wzBuf is the
	//    output value of pcchBuf.  The grfurld param is a set of flags used to
	//    control the download (default is 0).
	MSOMETHOD (HrDownload) (THIS_ WCHAR *wzBuf, int *pcchBuf, DWORD grfurld) CONST_METHOD PURE;

	// HrUpload - uploads the file specified in wzFile to the location specified
	//    by this url.  The grfurlu param is a set of flags used to control the
	//    upload (default is 0).
	//    Returns:
	//      S_OK    if the file was uploaded successfuly
	//      S_FALSE if the user cancel the operation (for example, he/she doesn't 
	//                      want to overwrite an existing file)
	//      E_FAIL  generic failure
	MSOMETHOD (HrUpload) (THIS_ const WCHAR *wzFile, DWORD grfurlu) CONST_METHOD PURE;

	// HrDelete - deletes the file specified by this url.
	MSOMETHOD (HrDelete) (THIS) CONST_METHOD PURE;

	// FExists - returns TRUE iff the file exists.  Param grfurle determines
	//    which kind of validation to perform (default is 0 which means do not
	//    check web servers (i.e. http/ftp) & returns FALSE if this url points
	//    to a web server location - to validate against web servers pass the
	//    msofurleCheckServers flag).
	MSOMETHOD_(BOOL, FExists) (THIS_ DWORD grfurle) CONST_METHOD PURE;

	// HrGetBase - returns the base url of this url in ppiurlBase.
	//    Caller must release the returned url when done.
	MSOMETHOD (HrGetBase) (THIS_ const IMsoUrl **ppiurlBase) CONST_METHOD PURE;

 	// FIsMhtml - does the url point to an mhtml file, and in the form "mhtml:X[!Y]" ?
	MSOMETHOD_(BOOL, FIsMhtml) (THIS) CONST_METHOD PURE;

	// Returns the reference to the bodypart this mhtml url is referring to
	MSOMETHOD_(LPCWSTR, WzMhtmlBodypart) (THIS) CONST_METHOD PURE;

	// Returns the cch of the reference to the bodypart this mhtml url is referring to
	MSOMETHOD_(int, CchMhtmlBodypart) (THIS) CONST_METHOD PURE;

	MSOMETHOD_(BPSC, BpscBulletProof) (THIS_ MSOBPCB *pmsobpcb) PURE;

	MSOMETHOD_(ULONG, Free) (THIS) PURE;
};


#ifndef __cplusplus
// IMsoUrl wrapper/convenience functions for C clients
#define IMsoUrl_QueryInterface(this, riid, ppvObj) \
	( (this)->lpVtbl->QueryInterface(this, riid, ppvObj) )
#define IMsoUrl_AddRef(this) \
	( (this)->lpVtbl->AddRef(this) )
#define IMsoUrl_Release(this) \
	( (this)->lpVtbl->Free(this) )
#define IMsoUrl_FValid(this) \
	( (this)->lpVtbl->FValid(this) )
#define IMsoUrl_HrSetFromUser(this, wzUrl, cp, piurlBase, grfurl) \
	( (this)->lpVtbl->HrSetFromUser(this, wzUrl, cp, piurlBase, grfurl) )
#define IMsoUrl_HrSetFromUserRgwch(this, rgwchUrl, cchUrl, cp, piurlBase, grfurl) \
	( (this)->lpVtbl->HrSetFromUserRgwch(this, rgwchUrl, cchUrl, cp, piurlBase, grfurl) )
#define IMsoUrl_HrSetFromCanonicalUrl(this, wzUrl, cp, piurlBase) \
	( (this)->lpVtbl->HrSetFromCanonicalUrl(this, wzUrl, cp, piurlBase) )
#define IMsoUrl_Lock(this) \
	( (this)->lpVtbl->Lock(this) )
#define IMsoUrl_Unlock(this) \
	( (this)->lpVtbl->Unlock(this) )
#define IMsoUrl_HrGetDisplayForm(this, wzBuf, pcchBuf, grfurldf) \
	( (this)->lpVtbl->HrGetDisplayForm(this, wzBuf, pcchBuf, grfurldf) )
#define IMsoUrl_HrGetCanonicalForm(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetCanonicalForm(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetCustomForm(this, grfurlc, wzBuf, pcchBuf, grfurlcf) \
	( (this)->lpVtbl->HrGetCustomForm(this, grfurlc, wzBuf, pcchBuf, grfurlcf) )
#define IMsoUrl_CpGetCodePage(this) \
	( (this)->lpVtbl->CpGetCodePage(this) )
#define IMsoUrl_WzCanonicalForm(this) \
	( (this)->lpVtbl->WzCanonicalForm(this) )
#define IMsoUrl_CchCanonicalForm(this) \
	( (this)->lpVtbl->CchCanonicalForm(this) )
#define IMsoUrl_UrlsGetScheme(this) \
	( (this)->lpVtbl->UrlsGetScheme(this) )
#define IMsoUrl_HrGetScheme(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetScheme(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetAuthority(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetAuthority(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetUserName(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetUserName(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetPassword(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetPassword(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetServer(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetServer(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetPort(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetPort(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetPath(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetPath(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetDir(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetDir(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetFileLeaf(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetFileLeaf(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetFileName(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetFileName(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetFileExt(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetFileExt(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetQuery(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetQuery(this, wzBuf, pcchBuf) )
#define IMsoUrl_HrGetFragment(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetFragment(this, wzBuf, pcchBuf) )
#define IMsoUrl_RgwchScheme(this, pcch) \
	( (this)->lpVtbl->RgwchScheme(this, pcch) )
#define IMsoUrl_RgwchAuthority(this, pcch) \
	( (this)->lpVtbl->RgwchAuthority(this, pcch) )
#define IMsoUrl_RgwchUserName(this, pcch) \
	( (this)->lpVtbl->RgwchUserName(this, pcch) )
#define IMsoUrl_RgwchPassword(this, pcch) \
	( (this)->lpVtbl->RgwchPassword(this, pcch) )
#define IMsoUrl_RgwchServer(this, pcch) \
	( (this)->lpVtbl->RgwchServer(this, pcch) )
#define IMsoUrl_RgwchPort(this, pcch) \
	( (this)->lpVtbl->RgwchPort(this, pcch) )
#define IMsoUrl_RgwchPath(this, pcch) \
	( (this)->lpVtbl->RgwchPath(this, pcch) )
#define IMsoUrl_RgwchFileLeaf(this, pcch) \
	( (this)->lpVtbl->RgwchFileLeaf(this, pcch) )
#define IMsoUrl_RgwchFileExt(this, pcch) \
	( (this)->lpVtbl->RgwchFileExt(this, pcch) )
#define IMsoUrl_RgwchQuery(this, pcch) \
	( (this)->lpVtbl->RgwchQuery(this, pcch) )
#define IMsoUrl_RgwchFragment(this, pcch) \
	( (this)->lpVtbl->RgwchFragment(this, pcch) )
#define IMsoUrl_FIsHttp(this) \
	( (this)->lpVtbl->FIsHttp(this) )
#define IMsoUrl_FIsFtp(this) \
	( (this)->lpVtbl->FIsFtp(this) )
#define IMsoUrl_FIsLocal(this) \
	( (this)->lpVtbl->FIsLocal(this) )
#define IMsoUrl_FIsUNC(this) \
	( (this)->lpVtbl->FIsUNC(this) )
#define IMsoUrl_FIsODMA(this) \
	( (this)->lpVtbl->FIsODMA(this) )
#define IMsoUrl_HrGetLocalPath(this, wzBuf, pcchBuf) \
	( (this)->lpVtbl->HrGetLocalPath(this, wzBuf, pcchBuf) )
#define IMsoUrl_WzLocalPath(this) \
	( (this)->lpVtbl->WzLocalPath(this) )
#define IMsoUrl_CchLocalPath(this) \
	( (this)->lpVtbl->CchLocalPath(this) )
#define IMsoUrl_FIsEqual(this, piurl) \
	( (this)->lpVtbl->FIsEqual(this, piurl) )
#define IMsoUrl_FComponentsAreEqual(this, grfurlc, piurl) \
	( (this)->lpVtbl->FComponentsAreEqual(this, grfurlc, piurl) )
#define IMsoUrl_FSubsumes(this, piurl) \
	( (this)->lpVtbl->FSubsumes(this, piurl) )
#define IMsoUrl_UrlrGetRelativity(this) \
	( (this)->lpVtbl->UrlrGetRelativity(this) )
#define IMsoUrl_HrSetRelativity(this, urlr) \
	( (this)->lpVtbl->HrSetRelativity(this, urlr) )
#define IMsoUrl_HrResolve(this, ppiurlAbsolute) \
	( (this)->lpVtbl->HrResolve(this, ppiurlAbsolute) )
#define IMsoUrl_HrRebase(this, piurlBase) \
	( (this)->lpVtbl->HrRebase(this, piurlBase) )
#define IMsoUrl_HrDownload(this, wzBuf, pcchBuf, grfurld) \
	( (this)->lpVtbl->HrDownload(this, wzBuf, pcchBuf, grfurld) )
#define IMsoUrl_HrUpload(this, wzFile, grfurlu) \
	( (this)->lpVtbl->HrUpload(this, wzFile, grfurlu) )
#define IMsoUrl_HrDelete(this) \
	( (this)->lpVtbl->HrDelete(this) )
#define IMsoUrl_FExists(this, grfurle) \
	( (this)->lpVtbl->FExists(this, grfurle) )
#define IMsoUrl_HrGetBase(this, ppiurlBase) \
	( (this)->lpVtbl->HrGetBase(this, ppiurlBase) )
#define IMsoUrl_FIsMhtml(this) \
 	( (this)->lpVtbl->FIsMhtml(this) )
#define IMsoUrl_WzMhtmlBodypart(this) \
 	( (this)->lpVtbl->WzMhtmlBodypart(this) )
#define IMsoUrl_CchMhtmlBodypart(this) \
 	( (this)->lpVtbl->CchMhtmlBodypart(this) )
#endif // __cplusplus


// Url Creation APIs
//
// MsoHrCreateUrl - creates an unset url in ppiurl.
// MsoHrCreateUrlFromUser - same as calling MsoHrCreateUrl
//    followed by IMsoUrl::HrSetFromUser.
// MsoHrCreateUrlFromUserRgwch - same as calling MsoHrCreateUrl
//    followed by IMsoUrl::HrSetFromUserRgwch.
// MsoHrCreateUrlFromCanonicalUrl - same as calling MsoHrCreateUrl
//    followed by IMsoUrl::HrSetFromCanonicalUrl.
// MsoHrCloneUrl - creates a new copy of the given url
MSOAPI_(HRESULT) MsoHrCreateUrl(interface IMsoUrl **ppiurl);
MSOAPI_(HRESULT) MsoHrCreateUrlFromUser(interface IMsoUrl **ppiurl,
	const WCHAR *wzUrl, DWORD cp, const IMsoUrl *piurlBase, DWORD grfurl);
MSOAPI_(HRESULT) MsoHrCreateUrlFromUserRgwch(interface IMsoUrl **ppiurl,
	const WCHAR *wzUrl, int cchUrl, DWORD cp, const IMsoUrl *piurlBase, DWORD grfurl);
MSOAPIX_(HRESULT) MsoHrCreateUrlFromCanonicalUrl(interface IMsoUrl **ppiurl,
	const WCHAR *wzUrl, DWORD cp, const IMsoUrl *piurlBase);
MSOAPI_(HRESULT) MsoHrCloneUrl(interface IMsoUrl **ppiurl, const IMsoUrl *piurl);


// Some useful path manipulation APIs
//
// MsoFIsPathSep - returns TRUE iff the character is a recognized path separator
// MsoRgwchPathSepIndex - returns a ptr to the first path sep character
// MsoRgwchPathSepIndexRight - returns a ptr to the last path sep character
__inline MsoFIsPathSep(WCHAR wch) { return wch == L'/' || wch == L'\\'; };
MSOAPIX_(WCHAR*) MsoRgwchPathSepIndex(const WCHAR *rgwch, int cch);
MSOAPI_(WCHAR*) MsoRgwchPathSepIndexRight(const WCHAR *rgwch, int cch);

MSOAPIX_(BOOL) MsoFWzIsResUrl(const WCHAR *wzURL);

// Path buffer constants
#define MSO_MAX_PATH            INTERNET_MAX_URL_LENGTH
#define MSO_CB_MAX_PATH         (MSO_MAX_PATH*sizeof(WCHAR))
#define MSO_MAX_URL_PATH        INTERNET_MAX_URL_LENGTH
#define MSO_CB_MAX_URL_PATH     (MSO_MAX_URL_PATH*sizeof(WCHAR))
#define MSO_MAX_LOCAL_PATH      MAX_PATH
#define MSO_CB_MAX_LOCAL_PATH   (MSO_MAX_LOCAL_PATH*sizeof(WCHAR))


//////////////////////////////////////////////////////////////////////////
//
// OPEN ISSUES:
// 
// 1.  We don't handle default ports in the equivalence comparison.
// 2.  We don't handle "http://server.com" & "http://www.server.com"
//     equivalence correctly.
//
//////////////////////////////////////////////////////////////////////////

#endif // MSOURL_H











