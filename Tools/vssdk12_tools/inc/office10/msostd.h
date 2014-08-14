#pragma once

/*************************************************************************
	msostd.h

	Owner: rickp
	Copyright (c) 1994 Microsoft Corporation

	Standard common definitions shared by all office stuff
*************************************************************************/

#if !defined(MSOSTD_H)
#define MSOSTD_H

#ifdef PPCMAC
#error
#endif

#ifdef PPCLIB
#error
#endif

#ifdef M68K
#error
#endif

/*************************************************************************
	make sure we have our processor type set up right - note that we
	now have three - count 'em, three - different symbols defined for
	each processor we support (e.g., X86, _X86_, and _M_IX386)
*************************************************************************/

#if defined(X86)
	// intentionally blank...
#elif defined(ALPHA)			
	#undef ALPHA
	#ifndef _ALPHA_
		#define _ALPHA_ 1
	#endif
#elif defined(_IA64_)
	// intentionally blank...
#elif defined(_AXP64_)			
	// intentionally blank...
#elif defined(_ALPHA_)
	// intentionally blank...
#elif defined(_M_IX86)
	#define X86 1
#elif defined(_M_IA64)
	#define _IA64_ 1
#elif defined(_M_ALPHA)
	#ifndef _ALPHA_
		#define _ALPHA_ 1
	#endif
#else
	#error Must define a target architecture
#endif

/*************************************************************************
	Pull in standard Windows and C definitions.
*************************************************************************/

// Warning pragmas for /W4 /WX compatability
#include <msowarn.h>

/*	make sure the compiler generates intrinsic calls of all crt functions,
	or else we'll pull in a ton of crt stuff we probably don't want. */
#ifndef RC_INVOKED
	#include <string.h>
#if !defined(_CRT_SECURE_CPP_OVERLOAD_STANDARD_NAMES) || !_CRT_SECURE_CPP_OVERLOAD_STANDARD_NAMES
		#pragma intrinsic(strcpy,strcat)
#endif
	#pragma intrinsic(strlen, memcpy, memset, memcmp)
#endif

#define OEMRESOURCE

#include <windows.h>
#if !defined(RC_INVOKED)
	#include <ole2.h>
#endif

#include <stdarg.h>

#define MsoStrcpy strcpy
#define MsoStrcat strcat
#define MsoStrlen strlen
#define MsoMemcpy memcpy
#define MsoMemset memset
#define MsoMemcmp memcmp
#define MsoMemmove memmove

/*************************************************************************
	Pre-processor magic to simplify Mac vs. Windows expressions.
*************************************************************************/

#define Mac(mac)
#define MacElse(mac, win) win
#define NotMac(win) win
#define Win(win) win
#define WinMac(win, mac) win


/*************************************************************************
	Calling conventions 

	If you futz with these, check the cloned copies in inc\msosdm.h
	
*************************************************************************/

#if STATIC_LIB_DEF
	#define MSOPUB
	#define MSOPUBDATA
#else
#if !defined(OFFICE_BUILD)
	#define MSOPUB __declspec(dllimport)
	#define MSOPUBDATA __declspec(dllimport)
#else
	#define MSOPUB __declspec(dllexport)
	#define MSOPUBDATA __declspec(dllexport)
#endif
#endif

/* MSOPUBX are APIs that used to be public but no one currently uses,
	so we've unexported them.  If someone decides they want/need one of
	these APIs, we should feel free to re-export them */

#if GELTEST
	#define MSOPUBX MSOPUB
	#define MSOPUBDATAX MSOPUBDATA
#else
	#define MSOPUBX
	#define MSOPUBDATAX
#endif

/* used for interface that rely on using the OS (stdcall) convention */
#define MSOSTDAPICALLTYPE __stdcall

/* used for interfaces that don't depend on using the OS (stdcall) convention */
#define MSOAPICALLTYPE __stdcall

#if defined(__cplusplus)
	#define MSOEXTERN_C extern "C"
	#define MSOEXTERN_C_BEGIN extern "C" {
	#define MSOEXTERN_C_END }
#else
	#define MSOEXTERN_C 
	#define MSOEXTERN_C_BEGIN
	#define MSOEXTERN_C_END
#endif
#define MSOAPI_(t)      MSOEXTERN_C MSOPUB t MSOAPICALLTYPE 
#define MSOSTDAPI_(t)   MSOEXTERN_C MSOPUB t MSOSTDAPICALLTYPE 
#define MSOAPIX_(t)     MSOEXTERN_C MSOPUBX t MSOAPICALLTYPE 
#define MSOSTDAPIX_(t)  MSOEXTERN_C MSOPUBX t MSOSTDAPICALLTYPE 
#define MSOCDECLAPI_(t) MSOEXTERN_C MSOPUB t MSOCDECLCALLTYPE 
#define MSOAPIMX_(t) MSOAPIX_(t)
#define MSOAPIXX_(t) MSOAPI_(t)
#if VSMSODEBUG
#define MSOAPIDBG_(t)   MSOAPI_(t)
#else
#define MSOAPIDBG_(t)   MSOAPIX_(t)
#endif

#define MSOMETHOD(m)      STDMETHOD(m)
#define MSOMETHOD_(t,m)   STDMETHOD_(t,m)
#define MSOMETHODIMP      STDMETHODIMP
#define MSOMETHODIMP_(t)  STDMETHODIMP_(t)

/* Interfaces derived from IUnknown behave in funny ways on the Mac */
#define BEGIN_MSOINTERFACE

#define MSOMACPUB 
#define MSOMACPUBDATA
#define MSOMACAPI_(t) t
#define MSOMACAPIX_(t) t 
	
#if X86 && !VSMSODEBUG
	#define MSOPRIVCALLTYPE __fastcall
#else
	#define MSOPRIVCALLTYPE __cdecl
#endif

#define MSOCDECLCALLTYPE __cdecl

#define MSOCONSTFIXUP(t) const t 

/*************************************************************************
	Extensions to systems headers from the MSAA (Accessibility) SDK
***************************************************************** DAVEPA */

// Extensions to winuser.h from \\ole\access\inc\winuser.h
#define WM_GETOBJECT	   0x003D
#define WMOBJ_ID        0x0000
#define WMOBJ_POINT     0x0001
#define WMOBJID_SELF    0x00000000

// Extensions to winable.h
#ifndef OBJID_NATIVEOM
#define OBJID_NATIVEOM	0xFFFFFFF0
#endif


/*************************************************************************
	Common #define section
*************************************************************************/

/* All Microsoft Office specific windows messages should use WM_MSO.
   Submessages passed through wParam should be defined in offpch.h.     */

// Note: This value needs to be free in all apps that use Mso.  
// It has been validated by all the 97 apps.  It would be safer to use
// RegisterWindowMessage, but this is more efficient and convenient.
#define WM_MSO (WM_USER + 0x0900)

#define MSOCHILDACTIVATE 23

// NA means not applicable. Use NA to help document parameters to functions.
#undef  NA
#define NA 0L

#ifndef MKCSM
// Section added to allow communication between Office 9 (pluggable UI) apps
// and the UI switching applet, Setlang
#define PUI_OFFICE_COMMAND (WM_USER + 0x0901)

#define PLUGUI_CMD_SHUTDOWN		0 // wParam value
#define PLUGUI_CMD_QUERY		1 // wParam value
#define OFFICE_VERSION_9		9 // standardized value to return for Office 9 apps
#define OFFICE_MAJOR_VERSION	10 // Current version of Office; used for communication with Setlang only

typedef struct _PLUGUI_INFO
{
	unsigned uMajorVersion : 8;	// Used to indicate App;s major version number
	unsigned uNoAppReboot : 1;	// BOOL, if FALSE, allows reboot by Setlang. TRUE means we don't want to be rebooted now
	unsigned uUnused : 23;		// not used
} PLUGUI_INFO;

typedef union _PLUGUI_QUERY
{
	UINT uQueryVal;
	PLUGUI_INFO PlugUIInfo;
} PLUGUI_QUERY;
// End of Pluggable UI section
#endif

/* End of common #define section */


/*************************************************************************
	Common segmentation definitions 
*************************************************************************/

/*	Used with #pragma to swap-tune global variables into the boot section
	of the data segment.  Should link with -merge:.bootdata=.data when
	using these pragmas */
	
#if VSMSODEBUG
	#define MSO_BOOTDATA
	#define MSO_ENDBOOTDATA
#else
	#define MSO_BOOTDATA data_seg(".bootdata")
	#define MSO_ENDBOOTDATA data_seg()
#endif


/*************************************************************************
	Stuff for Performance marker support - Don't mess with these in 
	your code unless you really know what you are doing.
*************************************************************************/
#define MSO_PERFMARKON  warning(disable: 4102)
#define MSO_PERFMARKOFF warning(default: 4102)
#define MsoPerformanceMarkerLabel()

/*---------------------------------------------------------------------------
	RISC alignment defines
--------------------------------------------------------------- BrianWen ---*/
#ifndef UNALIGNED
#if defined(_M_ALPHA)
#define UNALIGNED __unaligned
#else
#define UNALIGNED
#endif	/* (_M_ALPHA) */
#endif	/* UNALIGNED */

/*----------------------------------------------------------------------------
	COM IUnknown methods utilities
------------------------------------------------------------------- HAILIU -*/
#ifdef __cplusplus
#define MsoComAddRef(punk)   ((punk)->AddRef())
#define MsoComRelease(punk)  ((punk)->Release())
#define MsoComQueryInterface(punk, riid, ppv)   \
	((punk)->QueryInterface((riid), (LPVOID*)(ppv)))
#else
#define MsoComAddRef(punk)   ((punk)->lpVtbl->AddRef((punk)))
#define MsoComRelease(punk)  ((punk)->lpVtbl->Release((punk)))
#define MsoComQueryInterface(punk, riid, ppv)   \
	((punk)->lpVtbl->QueryInterface((punk), (riid), (LPVOID*)(ppv)))
#endif //__cplusplus


/// WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING 
/// WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING 
/// WARNING WARNING WARNING WARNING WARNING WARNING WARNING WARNING 


// for set {a,b[,c...]} in range 0..31
#define InSetOf9(v, a,b,c,d,e,f,g,h,i)	(!((v)&(~0x1fL)) && ((1<<(v)) & ((1<<(a))|(1<<(b))|(1<<(c))|(1<<(d))|(1<<(e))|(1<<(f))|(1<<(g))|(1<<(h))|(1<<(i)))))
#define InSetOf2(v, a,b)				InSetOf9(v, a,b,-1,-1,-1,-1,-1,-1,-1)
#define InSetOf3(v, a,b,c)				InSetOf9(v, a,b,c,-1,-1,-1,-1,-1,-1)
#define InSetOf4(v, a,b,c,d)			InSetOf9(v, a,b,c,d,-1,-1,-1,-1,-1)
#define InSetOf5(v, a,b,c,d,e)			InSetOf9(v, a,b,c,d,e,-1,-1,-1,-1)
#define InSetOf6(v, a,b,c,d,e,f)		InSetOf9(v, a,b,c,d,e,f,-1,-1,-1)
#define InSetOf7(v, a,b,c,d,e,f,g)		InSetOf9(v, a,b,c,d,e,f,g,-1,-1)
#define InSetOf8(v, a,b,c,d,e,f,g,h)	InSetOf9(v, a,b,c,d,e,f,g,h,-1)

// for set {a,b[,c...]} within 31 of each other, but not 0..31.  'a' must be smallest
#define InBiasSetOf2(v, a,b)				InSetOf2(v-(a), 0,b-(a))
#define InBiasSetOf3(v, a,b,c)				InSetOf3(v-(a), 0,b-(a),c-(a))
#define InBiasSetOf4(v, a,b,c,d)			InSetOf4(v-(a), 0,b-(a),c-(a),d-(a))
#define InBiasSetOf5(v, a,b,c,d,e)			InSetOf5(v-(a), 0,b-(a),c-(a),d-(a),e-(a))
#define InBiasSetOf6(v, a,b,c,d,e,f)		InSetOf6(v-(a), 0,b-(a),c-(a),d-(a),e-(a),f-(a))
#define InBiasSetOf7(v, a,b,c,d,e,f,g)		InSetOf7(v-(a), 0,b-(a),c-(a),d-(a),e-(a),f-(a),g-(a))
#define InBiasSetOf8(v, a,b,c,d,e,f,g,h)	InSetOf8(v-(a), 0,b-(a),c-(a),d-(a),e-(a),f-(a),g-(a),h-(a))
#define InBiasSetOf9(v, a,b,c,d,e,f,g,h,i)	InSetOf9(v-(a), 0,b-(a),c-(a),d-(a),e-(a),f-(a),g-(a),h-(a),i-(a))


#if VSMSODEBUG
#define MSOFORCECONST const
#else
#define MSOFORCECONST
#endif // VSMSODEBUG

#endif // MSOSTD_H
