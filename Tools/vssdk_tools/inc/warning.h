// warnings that are promoted for the Visual Studio builds
#pragma warning(3:4092) // sizeof returns 'unsigned long'
#pragma warning(3:4121) // structure is sensitive to alignment
#pragma warning(3:4125) // decimal digit in octal sequence
#pragma warning(3:4130) // logical operation on address of string constant
#pragma warning(3:4132) // const object should be initialized
#pragma warning(3:4212) // function declaration used ellipsis
#pragma warning(3:4259) // pure virtual function was not defined
#pragma warning(3:4700) // local used without being initialized
#pragma warning(3:4702) // unreachable code
#pragma warning(3:4705) // statement has no effect
#pragma warning(3:4709) // comma operator within array index expression
#if 0
#pragma warning(3:4100) // Unreferenced formal parameter
#pragma warning(3:4701) // local may be used without being initialized
#pragma warning(3:4706) // assignment within conditional expression
#endif

// warnings that are demoted for the Visual Studio builds
#pragma warning(4:4061) // enumerate in switch is not explicitly handled by case label

// warnings that are disabled for the Visual Studio builds
#pragma warning(disable:4290) // C++ Exception Specification ignored
#pragma warning(disable:4100) // Unreferenced formal parameter
#pragma warning(disable:4702) // unreachable code
#pragma warning(disable:4710) // function not inlined
#pragma warning(disable:4800) // forcing value to bool 'true' or 'false' (performance warning)
#pragma warning(disable:4263) // member fnctn does not override base class fnctn
#pragma warning(disable:4264) // no override available for virtual hidden fnctn
#pragma warning(disable:4141) // novtable more than once--broken in 8603 compiler

// warnings disabled temporarily until fixed in VS sources (1/20/00 KarlSi)
#pragma warning(disable:4584) // bass-class 'x' is already a base-class of 'y'
#pragma warning(disable:4920) // enum 'x' member <expr> already seen in enum 'y'
#pragma warning(disable:4995) // using deprecated functionality (eg. iostream)

// warnings disabled temporarily until fixed in VS sources (2004/07/20 DJCarter)
#pragma warning(disable:4334) // '<<' : result of 32-bit shift implicitly converted to 64 bits
#pragma warning(disable:4293) // '<<' : shift count negative or too big, undefined behavior
#pragma warning(disable:4430) // missing type specifier - int assumed. Note: C++ does not support default-int
#pragma warning(disable:4812) // obsolete declaration style: use '%$S::%s%$I' instead
#pragma warning(disable:4743) // '%S' has different size in '$s1' and '$s2': $N1 and $N2 bytes

// warning disabled for IJW
#pragma warning(disable:4562) // fully prototyped functions are required with the '/clr' option: converting '()' to '(void)'
#pragma warning(disable:4793) // 'vararg' : causes native code generation

// warnings that Win64 IDL builds generate
#ifdef _WIN64
#pragma warning(disable:4206) // nonstandard extension used: translation unit is empty
#endif

#ifndef __cplusplus
#undef try
#undef except
#undef finally
#undef leave
#define try                         __try
#define except                      __except
#define finally                     __finally
#define leave                       __leave
#endif

#ifdef _PREFIX_
#include <pfx_vc7.h>
#endif
