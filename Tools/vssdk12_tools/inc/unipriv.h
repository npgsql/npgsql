// unipriv.h -- UniLib private internal header

#ifdef _MSC_VER
#pragma once
#endif

#ifndef __UNIPRIV_H__
#define __UNIPRIV_H__

#undef UASSERT

#ifdef FEATURE_PAL
#define __UBREAK__ DebugBreak();
#define UASSERT(exp) _ASSERTE(exp)
#else // FEATURE_PAL

#ifdef _DEBUG

#ifdef _X86_
// Avoid inline assembler in macros because it breaks lambdas; can be changed back when dev10 bug 658310 is fixed
// #define __UBREAK__ _asm { int 3 }
#define __UBREAK__ DebugBreak();
#else
#define __UBREAK__ DebugBreak();
#endif // _X86_

#define UASSERT(exp) do { if (!(exp)) __UBREAK__; } while (FALSE)

#else // _DEBUG

#define UASSERT(exp) do {} while (false)

#endif // _DEBUG

#endif // FEATURE_PAL

#endif // __UNIPRIV_H__
