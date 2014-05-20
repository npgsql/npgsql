// warnings that are disabled for the Visual Studio builds
#pragma warning(disable:4290) // C++ Exception Specification ignored
#pragma warning(disable:4100) // Unreferenced formal parameter
#pragma warning(disable:4702) // unreachable code
#pragma warning(disable:4800) // forcing value to bool 'true' or 'false' (performance warning)
#pragma warning(disable:4141) // novtable more than once--broken in 8603 compiler
#pragma warning(disable:4296) // bool expression is always true
#pragma warning(disable:4242) // possible loss of data on conversion
#pragma warning(disable:4703) // Local pointer potentially used used w/o being initialized

// warnings disabled temporarily until fixed in VS sources (1/20/00 KarlSi)
#pragma warning(disable:4584) // bass-class 'x' is already a base-class of 'y'
#pragma warning(disable:4995) // using deprecated functionality (eg. iostream)

// warnings disabled temporarily until fixed in VS sources (2004/07/20 DJCarter)
#pragma warning(disable:4334) // '<<' : result of 32-bit shift implicitly converted to 64 bits
#pragma warning(disable:4293) // '<<' : shift count negative or too big, undefined behavior
#pragma warning(disable:4430) // missing type specifier - int assumed. Note: C++ does not support default-int

// warnings disabled temporarily until fixed in VS sources (2012/11/05 MarkLe)
// In the 17.1 compiler, C4312 is now enabled by default and VS sources are not clean
#pragma warning(disable:4312) // 'type cast' : conversion from 'int' to 'void *' of greater size

// warning disabled for IJW
#pragma warning(disable:4793) // 'vararg' : causes native code generation

