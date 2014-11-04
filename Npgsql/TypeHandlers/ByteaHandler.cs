using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class ByteaHandler : TypeHandler
    {
        static readonly string[] _pgNames = { "bytea" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override bool SupportsBinaryRead { get { return true; } }

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            throw new NotImplementedException();

            Int32 byteAPosition = 0;
#if GO
            if (len >= 2 && BackendData[0] == (byte)ASCIIBytes.BackSlash && BackendData[1] == (byte)ASCIIBytes.x)
            {
                // PostgreSQL 8.5+'s bytea_output=hex format
                byte[] result = new byte[(len - 2) / 2];
#if UNSAFE
                unsafe
                {
                    fixed (byte* pBackendData = &BackendData[2])
                    {
                        fixed (byte* pResult = &result[0])
                        {
                            byte* pBackendData2 = pBackendData;
                            byte* pResult2 = pResult;
                            
                            for (byteAPosition = 2 ; byteAPosition < byteALength ; byteAPosition += 2)
                            {
                                *pResult2 = FastConverter.ToByteHexFormat(pBackendData2);
                                pBackendData2 += 2;
                                pResult2++;
                            }
                        }
                    }
                }
#else
                Int32 k = 0;

                for (byteAPosition = 2; byteAPosition < len; byteAPosition += 2)
                {
                    result[k] = FastConverter.ToByteHexFormat(BackendData, byteAPosition);
                    k++;
                }
#endif

                return result;
            }
            else
            {
                byte octalValue = 0;
                MemoryStream ms = new MemoryStream();

                while (byteAPosition < len)
                {
                    // The IsDigit is necessary in case we receive a \ as the octal value and not
                    // as the indicator of a following octal value in decimal format.
                    // i.e.: \201\301P\A
                    if (BackendData[byteAPosition] == (byte)ASCIIBytes.BackSlash)
                    {
                        if (byteAPosition + 1 == len)
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition++;
                        }
                        else if (BackendData[byteAPosition + 1] >= (byte)ASCIIBytes.b0 && BackendData[byteAPosition + 1] <= (byte)ASCIIBytes.b7)
                        {
                            octalValue = FastConverter.ToByteEscapeFormat(BackendData, byteAPosition + 1);
                            byteAPosition += 4;
                        }
                        else
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition += 2;
                        }
                    }
                    else
                    {
                        octalValue = BackendData[byteAPosition];
                        byteAPosition++;
                    }

                    ms.WriteByte((Byte)octalValue);
                }

                return ms.ToArray();
            }
#endif
        }

        internal override void ReadBinary(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            // We're here only if a non-efficient access method was used (e.g. GetValue())
        }
    }
}
