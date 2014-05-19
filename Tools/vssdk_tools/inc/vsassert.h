//=--------------------------------------------------------------------------=
// VsAssert.H
//=--------------------------------------------------------------------------=
// Headers for common assert macros and debugging functionality
//=--------------------------------------------------------------------------=
// Copyright (c) 1997, Microsoft Corporation
//			All Rights Reserved
//=--------------------------------------------------------------------------=

/*
Throwing assertion support added.

Traditional code often does this
	VSASSERT(p!=NULL, "p was NULL");
	if(!p)
	{
		return E_FAIL;
	}
or something similar. 

Unfortunately, developers often forget to do the checking after an assertion. 
Additionally, the calling function is often not well prepared for this error return.

Exception-safe code has a better choice here. It's already ready to receive an exception
at any time. So we can use that in retail to avoid crashes in these kind of bad-internal
state situations.

The basic pattern here is that exception-safe code can do:

	VsVerifyThrow(p!=NULL, "p was NULL"	);

In debug, this is just a regular assertion. In retail, the code will throw an 
exception, avoiding the crash when p would have been dereferenced, and simplifying
the code because there's no need for the if clause shown above.

Different code bases use different kinds of exceptions, so vsassert is agnostic about
which kind of exception is being used. Its function looks like this

CLINKAGE BOOL ENTRYPOINT 
VsAssertAndThrow
(
	LPCSTR pszMsg, 
	LPCSTR pszAssert, 
	LPCSTR pszFile, 
	UINT line, 
	BOOL *pfThrow,				// Set to true if the use pressed the 'throw' button
	BOOL *pfDisableAssert,		// True if the user asked to disable this assert for ever
	BOOL *pfAlwaysThrow			// True if the user asked to throw this assert for ever
);

pfThrow will be true if the user wants to throw; the calling macro can then throw the
right kind of exception inline.

if pfAlwaysThrow is true, pfDisableAssert must be true too.


*/

#ifndef _INC_VSASSERT_H
#define _INC_VSASSERT_H

#include "windows.h"
#include "ole2.h"
#ifndef FEATURE_PAL
#include "commdlg.h"
#endif

#if _MSC_VER
#define	ENTRYPOINT  __declspec(dllexport) __stdcall
#else
#define ENTRYPOINT
#endif // _MSC_VER
    
#ifdef __cplusplus
#define CLINKAGE	extern "C"
#else
#define CLINKAGE
#endif

//=--------------------------------------------------------------------------=
// Turn off compiler warnings which are exacerbated by constructs in this
// file's definitions:

// Warning C4127: conditional expression is constant.  This is caused by all
//		of the macros with "do { ... } while (false)" syntax.  The syntax is
//		a good way to ensure that a statement-like macro can be used in all
//		contexts (specifically if statements), but the compiler warns about
//		the "while (false)" part. 

#if _MSC_VER
#pragma warning(disable: 4127)
#endif

#pragma push_macro("DEBUG")
#ifdef _DEBUG
#ifndef DEBUG
#define DEBUG
#endif
#endif

//=--------------------------------------------------------------------------=
// Debugging constants.  These flags are used to route debug messages to
// the appropriate places
//
#define DF_ENABLED               0x01 // This option is enabled
#define DF_OUTPUTDEBUGSTRING     0x02 // Use OutputDebugString
#define DF_OUTPUTDEBUGWINDOW     0x04 // Send to the host's immediate window
#define DF_OUTPUTFILE            0x08 // Output to a file
#define DF_OUTPUTDIALOG          0x10 // Use the assert dialog
#define DF_OUTOFPROCDIALOG       0x20 // Go out of proc to report the assert in a dialog (Must be combined with DF_OUTPUTDIALOG)

#define DF_DEFAULTASSERTOPTIONS  (DF_ENABLED | DF_OUTPUTDIALOG | DF_OUTOFPROCDIALOG)
#define DF_DEFAULTPRINTOPTIONS   (DF_ENABLED | DF_OUTPUTDEBUGSTRING)

//=--------------------------------------------------------------------------=
// If you want to route debug messages to an immediate/debug window of the
// hosting app, use this function to setup a callback with the same
// signature as below.  To revoke it, just call the fn again with a NULL
// parameter.
//
#ifdef DEBUG
typedef void (WINAPI *IMMEDIATECALLBACK)(LPCSTR pszMessage);
CLINKAGE BOOL ENTRYPOINT VsSetImmediateCallback(IMMEDIATECALLBACK pImmediateCallback);

//=--------------------------------------------------------------------------=
// Initialization / termination for debugging.  Yes, you can assert before
// and after these calls.  This just enables the fancy stuff.  Only the host
// EXE should call these.
//
CLINKAGE VOID ENTRYPOINT VsDebugInitialize();
CLINKAGE VOID ENTRYPOINT VsDebugTerminate();

//=--------------------------------------------------------------------------=
// Function prototypes.  You should use the macros below to keep debug/retail
// differences transparent.
//
// Support for managed VsTraceListener (note: ANSI strings)
CLINKAGE BOOL ENTRYPOINT VsManagedAssert(LPCSTR pszMsg, LPCSTR pszAssert, UINT line, BOOL *pfDisableAssert);
// Support for managed VsTraceListener (note: ANSI strings)
CLINKAGE VOID ENTRYPOINT VsDebugPrint(LPCSTR pszMsg);
CLINKAGE BOOL ENTRYPOINT VsAssert(LPCSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, BOOL *pfDisableAssert);
CLINKAGE BOOL ENTRYPOINT VsAssertAndThrow(LPCSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, BOOL *pfThrow, BOOL *pfDisableAssert, BOOL *pfAlwaysThrow);
#if defined(__cplusplus) && !defined(VSASSERT_MIXED_LINKAGE)
         BOOL ENTRYPOINT VsAssert(LPCWSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, BOOL *pfDisableAssert);
         BOOL ENTRYPOINT VsAssertAndThrow(LPCWSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, BOOL *pfThrow, BOOL *pfDisableAssert, BOOL *pfAlwaysThrow);
#endif
CLINKAGE BOOL ENTRYPOINT VsDisplayDebugMessage(LPCSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, int flags, BOOL *pfDisableAssert, ...);
CLINKAGE BOOL ENTRYPOINT VsDisplayDebugMessageThrow(LPCSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, int flags, BOOL *pfThrow, BOOL *pfDisableAssert, BOOL *pfAlwaysThrow, ...);
CLINKAGE BOOL ENTRYPOINT VsDisplayDebugMessageThrowVa(LPCSTR pszMsg, LPCSTR pszAssert, LPCSTR pszFile, UINT line, int flags, BOOL *pfThrow, BOOL *pfDisableAssert, BOOL *pfAlwaysThrow, va_list pArgs);
CLINKAGE VOID ENTRYPOINT VsDebugOutput(int dfOutput, LPCSTR pszOutputString, BOOL *pfDoInt3, BOOL *pfDisableAssert);
CLINKAGE VOID ENTRYPOINT VsDebugOutputThrow(int dfOutput, LPCSTR pszOutputString, BOOL *pfDoInt3, BOOL *pfThrow, BOOL *pfDisableAssert, BOOL *pfAlwaysThrow);
CLINKAGE VOID ENTRYPOINT VsDebugPrintf(LPCSTR pszMsg, ...);
CLINKAGE VOID ENTRYPOINT VsDebugPrintIf(BOOL fPrint, LPCSTR pszMsg, ...);
CLINKAGE VOID ENTRYPOINT VsEnableAsserts (BOOL fEnable);
CLINKAGE VOID ENTRYPOINT VsPrintCallstack(int nLines);
CLINKAGE BOOL ENTRYPOINT VsEnsureDebuggerPresent();
         BOOL ENTRYPOINT VsAssertWriteMiniDump();
         
//=--------------------------------------------------------------------------=
// Stack walk and resolve symbols functions
//
CLINKAGE UINT ENTRYPOINT VsGetStackAddresses(UINT ifrStart, UINT cfrTotal, DWORD_PTR * pdwAddr);
CLINKAGE BOOL ENTRYPOINT VsResolveSymbols(DWORD_PTR dwAddress, _Out_cap_(uicBuf) char * pszBuf, UINT uicBuf);

#if defined(_X86_) && !defined(PLATFORM_UNIX) && !defined(_M_CEE)
// Avoid inline assembler in macros because it breaks lambdas; can be changed back when dev10 bug 658310 is fixed
// #define Int3 _asm { int 3 }
#define Int3 DebugBreak();
#else
#define Int3 DebugBreak();
#endif // _X86_
#define VsDebugBreak() do { if (VsEnsureDebuggerPresent()) {Int3; } } while(0)

//=--------------------------------------------------------------------------=
// Debugging macros
//
static BOOL g_fStopOnVsAssert = FALSE;

// function which uses g_fStopOnVsAssert so we don't get a compiler warning
__forceinline
void
VSASSERT_H_UsefStopOnVsAssert()
{
    g_fStopOnVsAssert = g_fStopOnVsAssert;
}

#define VSASSERT(fTest, szMsg)                                      \
  do {                                                              \
    static BOOL fDisableThisAssert = FALSE;                         \
    if (!(fTest) && !fDisableThisAssert)                            \
      {                                                             \
      if(g_fStopOnVsAssert ||                                       \
        VsAssert(szMsg, #fTest, __FILE__, __LINE__, &fDisableThisAssert))\
        VsDebugBreak();                                             \
      }                                                             \
  } while (false)								    \

#define VSVERIFY(fTest, szMsg) VSASSERT((fTest), (szMsg))

#define VSVERIFYTHROW(fTest, szMsg, exception) VSVERIFYFUNC(fTest, szMsg, throw exception)

#define VSVERIFYFUNC(fTest, szMsg, function)                        \
  do                                                                \
    {                                                               \
    static BOOL fDisableThisAssert = FALSE;                         \
    static BOOL fThrowThisAssert = TRUE;                            \
    if (!(fTest))                                                   \
      {                                                             \
      BOOL fThrow=true;                                             \
      if(!fDisableThisAssert)                                       \
        {                                                           \
        if(g_fStopOnVsAssert ||                                     \
          VsAssertAndThrow( szMsg,                                  \
                            #fTest,                                 \
                            __FILE__,                               \
                            __LINE__,                               \
                            &fThrow,                                \
                            &fDisableThisAssert,                    \
                            &fThrowThisAssert))                     \
          {                                                         \
          VsDebugBreak();                                           \
          }                                                         \
        }                                                           \
      else                                                          \
        {                                                           \
          fThrow=fThrowThisAssert;                                  \
        }                                                           \
      if(fThrow)                                                    \
        {                                                           \
          function;                                                 \
        }                                                           \
      }                                                             \
    }                                                               \
  while (false)                                                     \

#define VSFAIL(szMsg) VSASSERT(0, szMsg)
#define VSIMPLIES(fHypothesis, fConclusion, szMsg) VSASSERT(!(fHypothesis) || (fConclusion), szMsg)
#define VSDEBUGPRINTF  VsDebugPrintf
#define VSDEBUGPRINTIF VsDebugPrintIf
#define VSPRINTCALLSTACK(nLines) VsPrintCallstack(nLines)

// "CAssert".  This is a compile time assert that will fire if fTest is
// zero.  Here, "msg" must be a non-quoted single word string that can be
// used as an identifier.
//
#define VSCASSERT(fTest, msg)  { struct foo { int Compile_Assert_##msg:((fTest)!=0); }; }

#else // DEBUG

//=--------------------------------------------------------------------------=
// Retail no-op implementations of debugging macros
//
#ifdef __cplusplus
inline void __cdecl _DebugNop(...) {}
#endif

#define VSASSERT(fTest, szMsg) do {} while (0)
#define VSVERIFY(fTest, szMsg) (void)(fTest);

#define VSVERIFYTHROW(fTest, szMsg, exception) VSVERIFYFUNC(fTest, szMsg, throw exception)

// Note: It is deliberate that this exists in retail too.
#define VSVERIFYFUNC(fTest, szMsg, function)                        \
  do                                                                \
    {                                                               \
    if (!(fTest))                                                   \
      {                                                             \
        function;                                                   \
      }                                                             \
    }                                                               \
  while (false)                                                     \

#define VSFAIL(szMsg) do {} while (0)
#define VSIMPLIES(fHypothesis, fConclusion, szMsg) do {} while (0)
#define VSDEBUGPRINTF 1 ? (void)0 : _DebugNop
#define VSDEBUGPRINTIF VSDEBUGPRINTF
#define VSPRINTCALLSTACK(nLines)
#define VSCASSERT(fTest, msg)

#define VsSetImmediateCallback(pfn)
#define VsDebugInitialize()
#define VsDebugTerminate()

#define VsGetStackAddresses(ifrStart, cfrTotal, pdwAddr) 0
#define VsResolveSymbols(dwAddress, pszBuf, uicBuf)      0
#define VsDebugBreak()  do {} while (0)
#endif // DEBUG

// usability improvement (accepting variable number of arguments and with formatting
// functionality built-in) (bug: 258645 - move the following definitions from
// env\msenv\inc\vbmacro.h to vscommon\vsassert\vsassert.h)

#ifdef DEBUG
    #define VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5) \
    if (pwszFormat && *pwszFormat)                                          \
    {                                                                       \
        WCHAR szFailString[MAX_PATH*4] = L"";                               \
                                                                            \
        VSBufPrint(szFailString, sizeof(szFailString)/sizeof(*szFailString), pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5);   \
                                                                            \
        VSFAIL(szFailString);                                               \
    }
    #define VSFAIL_FORMATWITHARGS_4(pwszFormat, ARG1, ARG2, ARG3, ARG4) \
        VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, ARG3, ARG4, NULL);
    #define VSFAIL_FORMATWITHARGS_3(pwszFormat, ARG1, ARG2, ARG3) \
        VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, ARG3, NULL, NULL);
    #define VSFAIL_FORMATWITHARGS_2(pwszFormat, ARG1, ARG2) \
        VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, NULL, NULL, NULL);
    #define VSFAIL_FORMATWITHARGS_1(pwszFormat, ARG1) \
        VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, NULL, NULL, NULL, NULL);

    #define VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5)    \
    {                                                                           \
        if (!fTest)                                                             \
        {                                                                       \
            VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5);    \
        }                                                                       \
    }
    #define VSASSERT_FORMATWITHARGS_4(fTest, pwszFormat, ARG1, ARG2, ARG3, ARG4)  \
        VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, ARG2, ARG3, ARG4, NULL);
    #define VSASSERT_FORMATWITHARGS_3(fTest, pwszFormat, ARG1, ARG2, ARG3)  \
        VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, ARG2, ARG3, NULL, NULL);
    #define VSASSERT_FORMATWITHARGS_2(fTest, pwszFormat, ARG1, ARG2)  \
        VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, ARG2, NULL, NULL, NULL);
    #define VSASSERT_FORMATWITHARGS_1(fTest, pwszFormat, ARG1)  \
        VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, NULL, NULL, NULL, NULL);

#else
    #define VSFAIL_FORMATWITHARGS_5(pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5)
    #define VSFAIL_FORMATWITHARGS_4(pwszFormat, ARG1, ARG2, ARG3, ARG4)
    #define VSFAIL_FORMATWITHARGS_3(pwszFormat, ARG1, ARG2, ARG3)
    #define VSFAIL_FORMATWITHARGS_2(pwszFormat, ARG1, ARG2)
    #define VSFAIL_FORMATWITHARGS_1(pwszFormat, ARG1)

    #define VSASSERT_FORMATWITHARGS_5(fTest, pwszFormat, ARG1, ARG2, ARG3, ARG4, ARG5)
    #define VSASSERT_FORMATWITHARGS_4(fTest, pwszFormat, ARG1, ARG2, ARG3, ARG4)
    #define VSASSERT_FORMATWITHARGS_3(fTest, pwszFormat, ARG1, ARG2, ARG3)
    #define VSASSERT_FORMATWITHARGS_2(fTest, pwszFormat, ARG1, ARG2)
    #define VSASSERT_FORMATWITHARGS_1(fTest, pwszFormat, ARG1)
#endif

//=--------------------------------------------------------------------------=
// Debug heap memory support routines.  These support leak tracking on a per
// DLL basis.  For us to be robust, you should ALWAYS use these routines
// in your debug code, unless there is some really well thought-out
// compelling reason not to.
//

#ifdef DEBUG

// This will get you the process heap
//
#define DEFAULT_HEAP (HANDLE)-1

// Debug allocators have the concept of an instance, which is a unique ID that
// is usually the instance handle of the DLL making the allocation.  This takes
// special support from the DLL, but gives you the benefit of being able to
// track DLL leaks as they exit your process.  You can also identify allocations
// that may occur in static constructors by presetting the instance to UNDEFINED,
// then updating the instances when the DLL's ProcessAttach gets called.
//
#define INSTANCE_GLOBAL    ((DWORD)0)
#define INSTANCE_UNDEFINED ((DWORD)-1)

// you should avoid calling these functions; use the macros defined below
//
CLINKAGE PVOID    ENTRYPOINT VsDebugAllocInternal           (HANDLE hheap, DWORD flags, SIZE_T cb, LPCSTR pszFile,  UINT  uLine, UINT_PTR dwInst, LPCSTR pszExtra);
CLINKAGE PVOID    ENTRYPOINT VsDebugReallocInternal         (HANDLE hheap, PVOID pv, DWORD flags, SIZE_T  cb, LPCSTR pszFile, UINT uLine, UINT_PTR dwInst, LPCSTR pszExtra);
CLINKAGE PVOID    ENTRYPOINT VsDebugSafeReallocInternal     (HANDLE hheap, PVOID pv, DWORD flags, SIZE_T  cb, LPCSTR pszFile, UINT uLine, UINT_PTR dwInst, LPCSTR pszExtra);
CLINKAGE VOID     ENTRYPOINT VsDebugFreeInternal            (HANDLE hheap, PVOID pv);
CLINKAGE SIZE_T   ENTRYPOINT VsDebugSizeInternal            (HANDLE hheap, PVOID pv);
CLINKAGE HANDLE   ENTRYPOINT VsDebugHeapCreateInternal      (DWORD flags, LPCSTR pszName, LPCSTR pszFile, UINT uLine);
CLINKAGE VOID     ENTRYPOINT VsDebugHeapDestroyInternal     (HANDLE hheap, BOOL fLeakCheck);

// debug wrappers for heap allocations.  You should call these to do all of your
// debug memory allocations.
//
#define VsDebAlloc(flags, cb)                 VsDebugAllocInternal(DEFAULT_HEAP, (flags), (cb), __FILE__, __LINE__, INSTANCE_GLOBAL, NULL)
#define VsDebRealloc(pv, flags, cb)           VsDebugReallocInternal(DEFAULT_HEAP, (pv), (flags), (cb), __FILE__, __LINE__, INSTANCE_GLOBAL, NULL)
#define VsDebSafeRealloc(pv, flags, cb)       VsDebugSafeReallocInternal(DEFAULT_HEAP, (pv), (flags), (cb), __FILE__, __LINE__, INSTANCE_GLOBAL, NULL)
#define VsDebFree(pv)                         VsDebugFreeInternal(DEFAULT_HEAP, (pv))
#define VsDebSize(pv)                         VsDebugSizeInternal(DEFAULT_HEAP, (pv))

#define VsDebHeapAlloc(heap, flags, cb)       VsDebugAllocInternal(heap, (flags), (cb), __FILE__, __LINE__, INSTANCE_GLOBAL, NULL)
#define VsDebHeapRealloc(heap, pv, flags, cb) VsDebugReallocInternal(heap, (pv), (flags), (cb), __FILE__, __LINE__, INSTANCE_GLOBAL, NULL)
#define VsDebHeapFree(heap, pv)               VsDebugFreeInternal(heap, (pv))
#define VsDebHeapSize(heap, pv)               VsDebugSizeInternal(heap, (pv))

#define VsDebHeapCreate(flags, name)          VsDebugHeapCreateInternal(flags, name, __FILE__, __LINE__)
#define VsDebHeapDestroy(heap, fLeakCheck)    VsDebugHeapDestroyInternal(heap, fLeakCheck)

// Heap diagnostic functions that you can freely call
//
CLINKAGE VOID     ENTRYPOINT VsDebValidateHeaps     ();
CLINKAGE BOOL     ENTRYPOINT VsDebIsValidHeap       (HANDLE hHeap);
CLINKAGE BOOL     ENTRYPOINT VsDebIsValidHeapPtr    (HANDLE hHeap, PVOID pv);
CLINKAGE VOID     ENTRYPOINT VsDebDumpMemStats      ();

//VsDebGetNextHeap       will return the next CHeapSpy instance. If passed in 0, then will get first instance
CLINKAGE HANDLE    ENTRYPOINT VsDebGetNextHeap       (
    __in HANDLE hHeap, 
    __out LPCSTR *pszHeapName, 
    __out LPCSTR *pszFile, 
    __out ULONG * pnLineNo  
);



#ifdef __cplusplus
CLINKAGE VOID     ENTRYPOINT VsDebCheckLeaks        (HANDLE hHeap, UINT_PTR dwInst = INSTANCE_GLOBAL);
#endif

// This allows you go get at the allocation blocks for a given heap.  Most
// people will never have to call these -- you can use them to mimic the CRT
// debug leak detection scheme.
//
// Note:  You should avoid performing allocations on a heap that you're
//        enumerating.
//
typedef struct _ALLOCATION
  {
  PVOID         m_pv;         //address of block
  SIZE_T        m_cb;         //size of allocation in BYTES
  ULONG         m_cAlloc;     //allocation pass count.  relative to all heaps
  LPCSTR        m_pszFile;    //source file where the allocation was made
  ULONG         m_uLine;      //source line number where the allocation was made
  UINT_PTR      m_dwInst;     //instance ID of this allocation
  LPCSTR        m_pszExtra;   //additional info macros may like to add (eg. class name)
  DWORD         m_dwTid;      //thread ID that caused this allocation
  BOOL          m_fIgnorable; //can we ignore this allocation?
  } ALLOCATION;

// Work around a more strict VC6 compiler. When this file is included in components
// other than vsassert, these should be dllimport anyway. Also, the __declspec(dll*)
// should come *before* the return type.
typedef ALLOCATION* PALLOCATION;

CLINKAGE PALLOCATION ENTRYPOINT VsDebGetFirstBlock(HANDLE hHeap);
CLINKAGE PALLOCATION ENTRYPOINT VsDebGetNextBlock(HANDLE hHeap);

//=--------------------------------------------------------------------------=
// Debug IMallocSpy implementation.  Only an EXE should call these functions
// as there is only one MallocSpy per process.
//
// rgIgnoreList may specify an optional list of sizes of external allocations to
// ignore.  This is usefull if your leaks beyond your control happen that you
// would like to filter out.  rgIgnoreList must be static as it is not copied,
// and it must end in a 0 byte value.  You may pass in NULL here if you do not
// have any special case external allocations to ignore.
//
CLINKAGE HRESULT ENTRYPOINT VsStartMallocSpy(DWORD_PTR *rgIgnoreList); // obsolette, use VsStartMallocSpyEx
#ifdef __cplusplus
CLINKAGE HRESULT ENTRYPOINT VsStartMallocSpyEx(LPCOLESTR pszKnownLeaksFileName = NULL, LPCOLESTR pszGdiKnownLeaksFileName = NULL); 
   // NULL parameters mean default file names: <exename>_leaks.xml amd <exename>_gdileaks.xml
#endif
CLINKAGE HRESULT ENTRYPOINT VsStopMallocSpy();

//=--------------------------------------------------------------------------=
// Displays the standard debugging options dialog.
//
CLINKAGE VOID ENTRYPOINT VsShowDebugOptions(HWND hwndParent);

// you should avoid calling these functions; use the macros defined below
//
CLINKAGE PVOID ENTRYPOINT VsDebOleAllocInternal   (LPCSTR pszFile, ULONG ulLine, SIZE_T cb);
CLINKAGE VOID  ENTRYPOINT VsDebOleFreeInternal    (LPCSTR pszFile, ULONG ulLine, PVOID pv);
CLINKAGE PVOID ENTRYPOINT VsDebOleReallocInternal (LPCSTR pszFile, ULONG ulLine, PVOID pv, SIZE_T cb);

CLINKAGE BSTR  ENTRYPOINT VsSysAllocStringInternal        (LPCSTR pszFile, ULONG ulLine, const OLECHAR *pszString);
CLINKAGE BSTR  ENTRYPOINT VsSysAllocStringByteLenInternal (LPCSTR pszFile, ULONG ulLine, LPCSTR pszString, UINT cb);
CLINKAGE BSTR  ENTRYPOINT VsSysAllocStringLenInternal     (LPCSTR pszFile, ULONG ulLine, const OLECHAR *pszString, UINT cch );

CLINKAGE VOID  ENTRYPOINT VsIgnoreAllocsInternal (BOOL fIgnore);

//utility to help you add leak checking functionality in case you have to wrap
// allocation routines (in which case you loose file/line info)
CLINKAGE VOID  ENTRYPOINT VsDebOleSetAllocInfo (LPCSTR pszFile, ULONG ulLine);

//wrapper for CoCreateInstance, which calls VsIgnoreAllocs
CLINKAGE HRESULT ENTRYPOINT VsCoCreateInstance(REFCLSID rclsid, LPUNKNOWN pUnkOuter,
				    DWORD dwClsContext, REFIID riid, LPVOID FAR* ppv);

// debug wrappers for IMalloc allocations.  You should call these to do all of your debug
// IMalloc / SysAlloc* allocations
//
#define VsDebOleAlloc(cb)        VsDebOleAllocInternal(__FILE__, __LINE__, cb)
#define VsDebOleFree(pv)         VsDebOleFreeInternal(__FILE__, __LINE__, pv)
#define VsDebOleRealloc(pv, cb)  VsDebOleReallocInternal(__FILE__, __LINE__, pv, cb)

#define VsDebSysAllocString(str)            VsSysAllocStringInternal(__FILE__, __LINE__, str)
#define VsDebSysAllocStringByteLen(str, cb) VsSysAllocStringByteLenInternal(__FILE__, __LINE__, str, cb)
#define VsDebSysAllocStringLen(str, cch)    VsSysAllocStringLenInternal(__FILE__, __LINE__, str, cch)

// It may sometimes be necessary to ignore IMalloc allocations.  Common dialogs are
// an example, as they cleanup after our process has left the building.  Call this
// with TRUE for f to disable leak checking for EXTERNAL leaks, and FALSE to re-enable
// it.  Note that this (1) only works for external leaks and (2) is refcounted so
// you must balance it.
//
#define VsIgnoreAllocs(f)      VsIgnoreAllocsInternal(f);

#ifndef FEATURE_PAL
// Wrappers for comdlg.dll 'A' functions which call VsIgnoreAllocsInternal...
CLINKAGE BOOL ENTRYPOINT VsGetOpenFileNameA(LPOPENFILENAMEA pofn);
CLINKAGE BOOL ENTRYPOINT VsGetSaveFileNameA(LPOPENFILENAMEA pofn);
CLINKAGE short ENTRYPOINT VsGetFileTitleA(LPCSTR lpstr1, _Out_z_cap_(w) LPSTR lpstr2, WORD w);
CLINKAGE BOOL ENTRYPOINT VsChooseColorA(LPCHOOSECOLORA pcc);
CLINKAGE HWND ENTRYPOINT VsFindTextA(LPFINDREPLACEA pfr);
CLINKAGE HWND ENTRYPOINT VsReplaceTextA(LPFINDREPLACEA pfr);
CLINKAGE BOOL ENTRYPOINT VsChooseFontA(LPCHOOSEFONTA pcf);
CLINKAGE BOOL ENTRYPOINT VsPrintDlgA(LPPRINTDLGA ppd);
CLINKAGE DWORD ENTRYPOINT VsCommDlgExtendedError(VOID);
CLINKAGE BOOL ENTRYPOINT VsPageSetupDlgA(LPPAGESETUPDLGA pps);
#endif

//=--------------------------------------------------------------------------=
//=--------------------------------------------------------------------------=
// Recommended defaults
//
// This header / dll combination is not designed to be used all by itself.
// You should add appropriate macro definitions to your own global headers
// to hide the fact that you're calling into this DLL.  This allows your team
// to code the way they know how, using team-established standards instead
// of imposing arbitrary rules on them.
//
// The macros below define some guidelines for setting up your own macros.
// If these macros suit all of your needs you may just #define
// VSASSERT_SET_DEFAULTS before including this header.
//
//=--------------------------------------------------------------------------=
//=--------------------------------------------------------------------------=
#if VSASSERT_SET_DEFAULTS

// Assertion / tracing macro definitions
//
#define ASSERT(fTest, szMsg)  VSASSERT((fTest), (szMsg))
#define FAIL(szMsg)           VSFAIL((szMsg))
#define DEBUGPRINTF           VSDEBUGPRINTF
#define DEBUGPRINTIF          VSDEBUGPRINTIF
#define CASSERT(fTest, msg)   VSCASSERT((fTest), (msg))

// Debug switches
//
#define DEFINE_SWITCH(NAME, PACKAGE, DESC)  VSDEFINE_SWITCH(NAME, PACKAGE, DESC)
#define EXTERN_SWITCH(NAME)                 VSEXTERN_SWITCH(NAME)
#define FSWITCH(NAME)                       VSFSWITCH(NAME)

#ifdef DEBUG

// Memory allocation
//
#if 0
// Note:  You should copy these to your own codebase as we cannot link them in here
//
PVOID operator new(size_t size)
      { return VsDebAlloc(0, size); }
PVOID operator new(size_t size, LPCSTR pszFile, UINT uLine)
      { return VsDebugAllocInternal(DEFAULT_HEAP, 0, size, pszFile, uLine, INSTANCE_GLOBAL, NULL); }
void  operator delete(PVOID pv)
      { VsDebFree(pv); }
#endif // 0

PVOID __cdecl operator new(size_t size);
PVOID __cdecl operator new(size_t size, LPCSTR pszFile, UINT uLine);
void  __cdecl operator delete(PVOID pv);
#define new new(__FILE__, __LINE__)

#define calloc(num, size)     VsDebAlloc(0, (num) * (size))
#define malloc(size)          VsDebAlloc(0, (size))
#define realloc(pv, size)     VsDebSafeRealloc((pv), 0, (size))
#define _recalloc(pv, num, size) VsDebSafeRealloc((pv), 0, (num) * (size))
#define free(pv)              VsDebFree((pv))

#define HeapAlloc(heap, flags, size)        VsDebHeapAlloc((heap), (flags), (size))
#define HeapReAlloc(heap, flags, pv, size)  VsDebHeapRealloc((heap), (flags), (pv), (size))
#define HeapFree(heap, flags, pv)           VsDebHeapFree((heap), (pv))
#define HeapSize(heap, flags, pv)           VsDebHeapSize((heap), (pv))

#define CoTaskMemAlloc(cb)        VsDebOleAlloc(cb)
#define CoTaskMemRealloc(pv, cb)  VsDebOleRealloc(pv, cb)
#define CoTaskMemFree(pv)         VsDebOleFree(pv)

#define SysAllocString(str)             VsDebSysAllocString((str))
#define SysAllocStringByteLen(str, cb)  VsDebSysAllocStringByteLen((str), (cb))
#define SysAllocStringLen(str, cch)     VsDebSysAllocStringLen((str), (cch))

#endif // DEBUG

//=--------------------------------------------------------------------------=
//=--------------------------------------------------------------------------=
#endif // VSASSERT_SET_DEFAULTS

#ifdef __cplusplus
//=--------------------------------------------------------------------------=
// This provides users with a way to create switches that only appear in
// debug.  Users can change these switches through a debug dialog that is
// automatically updated with the latest switches as package DLL's are loaded.
//
// These switches are persisted to a vsdebug.ini file in the windows directory.
//
// ...at the top of your code somewhere...
//
// DEFINE_SWITCH(FDisplayZonkerStatus, "Java", "Display Zonker Status");
//
//      FDisplayZonkerStatus    - the name of your switch
//      Java                    - some logical package grouping of your desire
//      Display Zonker Status   - text to display in the dialog
//
// ...then, wherever you would actually write the zonker status...
//
// if(FSWITCH(FDisplayZonkerStatus))
//    OutputDebugString(szZonkerStatus);
//
// Finally, if you need the switch in another file, use this construct:
//
// EXTERN_SWITCH(FDisplayZonkerStatus);
//
// Switches can be shared anywhere within the same DLL, but cannot be
// shared across DLLs.
//
//

class DebSwitch;
CLINKAGE PVOID ENTRYPOINT VsGetDebSwitchHead(void);
CLINKAGE VOID  ENTRYPOINT VsLoadSwitchState(DebSwitch *pSwitch);

class DebSwitch
  {
  public:
    DebSwitch(LPCSTR pszName, LPCSTR pszPackage, LPCSTR pszDesc)
      {
      // set fields
      m_pszName = pszName;
      m_pszPackage = pszPackage;
      m_pszDesc = pszDesc;
      m_fSet = FALSE;

      // link into global list of switches
      DebSwitch **ppHead = (DebSwitch **)VsGetDebSwitchHead();
      this->m_pdebswNext = *ppHead;
      *ppHead = this;

      // now load the switch state.  We must do this piecemeal
      // because DLL's may come and go at random times
      //
      VsLoadSwitchState(this);
      }

    ~DebSwitch()
      {
      // find our link and remove it.
      DebSwitch **ppHead = (DebSwitch **)VsGetDebSwitchHead();
      while (*ppHead) 
        {
        if (*ppHead == this) 
          {
          *ppHead = (*ppHead)->m_pdebswNext;
          break;
          }

          ppHead = &((*ppHead)->m_pdebswNext);
        }
      }

    BOOL m_fSet;              // TRUE if switch is enabled
    LPCSTR m_pszPackage;       // the package name
    LPCSTR m_pszName;          // name of the switch
    LPCSTR m_pszDesc;          // description string
    DebSwitch* m_pdebswNext;  // next switch in global list
  };

#define VSDEFINE_SWITCH(NAME, PACKAGE, DESC)  DebSwitch g_Switch_ ## NAME(#NAME, PACKAGE, DESC)
#define VSEXTERN_SWITCH(NAME)                 extern DebSwitch g_Switch_ ## NAME
#define VSFSWITCH(NAME)                       (g_Switch_ ## NAME . m_fSet)

//****************************************************************************
// Externally Created Switches
//
//****************************************************************************

// These entry points provide a way for packages created in non C languages
// to create and use VS debug switches.  If you wish to run DEBUG binaries
// using these entry points in non-DEBUG drops you should use dynamic binding
// and handle binding failures by providing default switch values.

STDAPI_(PVOID) VsExternalAddSwitch(LPCSTR pszName, LPCSTR pszPackage, LPCSTR pszDesc);
STDAPI_(BOOL) VsExternalQuerySwitch(PVOID pSwitch);
STDAPI_(VOID) VsExternalRemoveSwitch(PVOID pSwitch);


// DebSwitchExt is a version of DebugSwitch that is instantiated via dll
// entry point instead of by macro. The only difference is that this version
// must copy the provided strings since the pointers are not guaranteed to
// point to non-volatile memory.

class DebSwitchExt : public DebSwitch
  {
  public:
    DebSwitchExt(LPCSTR pszName, LPCSTR pszPackage, LPCSTR pszDesc);

    CHAR m_rgszName[100];
    CHAR m_rgszPackage[100];
    CHAR m_rgszDesc[300];
  };

//****************************************************************************
#endif


#else // DEBUG


//=--------------------------------------------------------------------------=
// Retail defines
//
#define VSDEFINE_SWITCH(NAME, PACKAGE, DESC)
#define VSEXTERN_SWITCH(NAME)
#define VSFSWITCH(NAME) FALSE

#define VsIgnoreAllocs(f)

#endif // DEBUG

/////
// This will show up in Debug.Threads in the VC debugger
/////
CLINKAGE VOID ENTRYPOINT VsNameThisThread (LPCSTR szThreadName);


//****************************************************************************
//	    REFERENCE TRACKING
//****************************************************************************
/*
  Tracker Macros
  ==============

    These macros can be used to record AddRef and Release on COM objects so
    reference counting errors can be pinpointed.

    How these macros work:
    =====================

    Whenever a reference is added by calling AddRef on a COM object, there is
    usually a variable somewhere that holds a pointer to the object.  This
    variable "owns" the reference.  When this variable is cleared (or goes out
    of scope) the reference should be released.  If there is a reference leak, it
    means that one of these variables didn't make its call to Release.

    What these macros do in the debug build is that whenever anybody calls
    AddRef, and entry is added to a table.  The entry stores the COM IUnkown
    pointer, the address of the variable that holds this pointer, and a pass count.
    Whenever anybody calls Release, we search the table for an entry where the COM
    IUnknown pointer matches and the address of the variable matches, and remove it.
    (If we don't find it we assert.  This means somebody is over-releasing.)

    Then, at shutdown, we make sure the table is empty.  If it is not empty, then
    we assert.  With the PassCount number, you can restart and break at the
    offending AddRef.  I.e., you can break at exactly the point where somebody
    AddRefs an object but never calls Release.


    How to use:
    ===========

    Here is some sample code which uses these macros.

    IFoo *g_pFoo;

    void PlayWithObjects()
    {
      IFoo *pFoo = NULL;
      IFoo *pFoo2 = NULL;

      GetFooFromSomewhere(&pFoo);
      TRACKER_RECEIVE(pFoo);

      pFoo2 = pFoo;
      TRACKER_ADDREF(pFoo2);

      // assign it to our global variable.  This global
      // is going to hold it for a long time, so we don't
      // release it at the end of this function.  Somebody somewhere
      // else is going to release it later.
      g_pFoo = pFoo;
      TRACKER_ADDREF(g_pFoo);

      // use pFoo and pFoo2 for a while

      TRACKER_RELEASE(pFoo);
      TRACKER_RELEASE(pFoo2);
    }

    Whenever you are given an object (because you called some function that
    returns it to you) and you now hold a reference to it, call TRACKER_RECEIVE.
    (This does nothing in the non-debug build.)  This will make sure you
    remember to call release later.

    Whenever you need to addref (because you copy the pointer into another
    variable, for example) use TRACKER_ADDREF.

    Whenever you need to release (when your variables are going out of scope),
    call TRACKER_RELEASE.

    Other notes: if you're giving out an object to your caller, you will need
    to use TRACKER_GIVE_AWAY.  If you're transferring ownership from one variable
    to another, use TRACKER_ASSIGN.

*/

//****************************************************************************

//---------------------------------------------------------------------------

#ifdef DEBUG
#define TRACKER_ASSERT_EVERYTHING_RELEASED() TrackerAssertEverythingReleased()
#else
#define TRACKER_ASSERT_EVERYTHING_RELEASED()
#define TrackerTransfer() do {} while (0)
#endif	// DEBUG

#ifdef DEBUG

#define TRACKER_ADDREF(punk1) do {	  \
    VSASSERT ((punk1), "TRACKER can't addref NULL");  \
    (punk1)->AddRef();			  	  \
    TrackerTransfer ((punk1), NULL, &(punk1));	  \
  } while (0)					  \

#define TRACKER_RELEASE(punk) do {	    	\
    VSASSERT (punk, "TRACKER can't release NULL");	\
    (punk)->Release();			    	\
    TrackerTransfer ((punk), &(punk), NULL);   	\
    (punk) = NULL;			    	\
  } while (0)					\

#define TRACKER_RECEIVE(punk) do {	       		\
    VSASSERT (punk, "TRACKER can't receive NULL");	\
    TrackerTransfer ((punk), NULL, &(punk)); 		\
  } while (0)						\


#define TRACKER_GIVE_AWAY(punk) do {			\
    VSASSERT (punk, "TRACKER can't give away NULL");	\
    TrackerTransfer ((punk), &(punk), NULL); 		\
  } while (0)						\


#define TRACKER_ASSIGN(punkDest, punkSrc) do {		  \
    VSASSERT (punkSrc, "TRACKER can't assign NULL");	  \
    VSASSERT (!punkDest, "TRACKER must assign to NULL");\
    (punkDest) = (punkSrc);				  \
    TrackerTransfer ((punkSrc), &(punkSrc), &(punkDest));  \
    punkSrc = NULL;					  \
  } while (0)


#define TRACKER_COPY(punkDest, punkSrc) do {		\
    VSASSERT ((punkSrc), "TRACKER can't assign NULL");	\
    VSASSERT ((!punkDest), "TRACKER must assign to NULL");\
    (punkDest) = (punkSrc);				\
    TRACKER_ADDREF ((punkDest));			\
  } while (0)

CLINKAGE void ENTRYPOINT  TrackerTransfer(IUnknown *punk, void *pvFrom, void *pvTo);
CLINKAGE void ENTRYPOINT  TrackerAssertEverythingReleased();

#else // DEBUG

#define TRACKER_ADDREF(punk)	    ((punk)->AddRef())
#define TRACKER_RELEASE(punk)	    ((punk) -> Release(), (punk) = NULL)
#define TRACKER_RECEIVE(punk)	    do {} while (0)
#define TRACKER_GIVE_AWAY(punk)	    do {} while (0)
#define TRACKER_ASSIGN(punkD, punkS) ((punkD) = (punkS), (punkS) = NULL)
#define TRACKER_COPY(punkD, punkS)  ((punkD) = (punkS), (punkD) -> AddRef())

#endif // DEBUG

#define TRACKER_CHECK_RELEASE(punk) do {	\
    if (punk) {					\
      TRACKER_RELEASE(punk);			\
    }						\
  } while (0)					


//******************** Dynamic casting

// The new C++ standard defines dynamic_cast<type> (expr), which appears
// to be particulary usefull for verifying that a pointer to one class
// can safely be casted to a pointer to another.  However, this safety
// requires the use of RTTI, which currently imposes a significant overhead.
// We could almost use static_cast when RTTI is not available, but it has
// slightly different semantics in some circumstances

// The defined(DEBUG) is uneccesary since VSASSERT will go away for retail

#if defined (_CPPRTTI) && defined (DEBUG)

#define ASSERT_TYPE(type, expr) do { \
  VSASSERT ((dynamic_cast <type> (expr)), "Wrong type"); \
  }  while (0)

#else

#define ASSERT_TYPE(type, expr) do { } while (0)

#endif


#pragma pop_macro("DEBUG")

#endif // _INC_VSASSERT_H




