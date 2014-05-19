/*************************************************************************
	msowarn.h
	
	Owner: michmarc
	Copyright (c) 1999 Microsoft Corporation
	
	File that contains all of the pragmas necessary to make
	/W4 /WX work with Office builds
*************************************************************************/

#ifndef _MSO_WARN
#define _MSO_WARN
#pragma once

// Warnings that need fixing to make things /W4 clean

// Always useless
#pragma warning(disable:4049)   // Compiler limit -- no more line number info
#pragma warning(disable:4054)   // Casting function pointer to data pointer
#pragma warning(disable:4055)   // Casting data pointer to function pointer
#pragma warning(disable:4100)   // Unreferenced formal parameter
#pragma warning(disable:4102)   // Unreferenced label
#pragma warning(disable:4115)   // Named type definition in parenthesis
#pragma warning(disable:4121)   // structure sensitive to packing
#pragma warning(disable:4152)   // Microsoft extension -- fn/data pointer conv
#pragma warning(disable:4168)   // Compiler limit -- out of debug types
#pragma warning(disable:4200)   // Microsoft extension -- Zero sized array
#pragma warning(disable:4201)   // Microsoft extension -- Nameless struct/union
#pragma warning(disable:4204)   // Microsoft extension -- Nonconst agg initializer
#pragma warning(disable:4206)   // Microsoft extension -- Source file is empty
#pragma warning(disable:4207)   // Microsoft extension -- Extended initializer form
#pragma warning(disable:4211)   // Microsoft extension -- Extern to static
#pragma warning(disable:4213)   // Microsoft extension -- Cast on LValue
#pragma warning(disable:4214)   // Microsoft extension -- Bitfield not int
#pragma warning(disable:4221)   // Microsoft extension -- Init with addr of local
#pragma warning(disable:4239)   // Microsoft extension -- nonconst reference to nonlvalue
#pragma warning(disable:4238)   // Microsoft extension -- class rvalue as lvalue
#pragma warning(disable:4305)   // Casting causes truncation
#pragma warning(disable:4509)   // Microsoft extension -- SEH and destructors
#pragma warning(disable:4510)   // Default constructor could not be generated
#pragma warning(disable:4511)   // Copy constructor could not be generated
#pragma warning(disable:4512)   // Assignment operator could not be generated
#pragma warning(disable:4513)   // Destructor could not be generated
#pragma warning(disable:4514)   // Unreferenced inline function removed
#pragma warning(disable:4527)   // User defined destructor required
#pragma warning(disable:4610)   // User defined constructor required
#pragma warning(disable:4611)   // Setjmp/C++ destruction interaction unportable
#pragma warning(disable:4710)   // Inline function not inlined
#pragma warning(disable:4798)   // Native code instead of PCode generated

#if VSMSODEBUG
#pragma warning(disable:4124)	  // Stack checking and __fastcall mixed
#endif

// Currently useless, but could be made useful
#pragma warning(disable:4018)   // Signed/unsigned comparison mismatch.  Might be useful, except that all
                                // arguments smaller than int are promoted to signed int, so byte==(byte+byte) generates this.
#pragma warning(disable:4127)   // Conditional is constant.  Might be useful, but many asserts are constantly true
                                //    and "while (1) {}" and "for(A;;B)" constructs generate this warning as well.
#pragma warning(disable:4245)   // Signed/unsigned asignment mismatch.  Might be useful, except that all
                                // arguments smaller than int are promoted to signed int, so byte=byte+byte generates this.
#pragma warning(disable:4268)   // const static/global initilzed with compiler generated default constructor
                                //    seems that "extern "C" const ClassName cn;" can generate this, even though this is
                                //    a declaration, not a definition
#pragma warning(disable:4310)   // Cast truncates constant value (problem because LOBYTE(0x113) generates this)
                                // and there is often no way to work around the warning
#pragma warning(disable:4414)   // __asm short jump converted to near jump.  Currently a compiler bug causes
								// the 'near' and 'far' keywords to be thrown out, making it impossible to declare
								// a jump in inline assembly that is anything except short.
#pragma warning(disable:4702)   // Unreachable code.  Can't be eliminated because this warning
                                // can be generated against compiler created code at the end of a block

// REVIEW -- should these be re-enabled?
#pragma warning(disable:4211)   // Redefined extern to static 
#pragma warning(disable:4505)   // Unreferenced static function removed (happens in ATL code)

#endif /* _MSO_WARN */
