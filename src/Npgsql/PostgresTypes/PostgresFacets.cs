using System;
using System.Text;

namespace Npgsql.PostgresTypes
{
    readonly struct PostgresFacets : IEquatable<PostgresFacets>
    {
        internal static readonly PostgresFacets None = new PostgresFacets(null, null, null);

        internal PostgresFacets(int? size, int? precision, int? scale)
        {
            Size = size;
            Precision = precision;
            Scale = scale;
        }

        public readonly int? Size;
        public readonly int? Precision;
        public readonly int? Scale;

        public override bool Equals(object o)
            => o is PostgresFacets otherFacets && Equals(otherFacets);

        public bool Equals(PostgresFacets o)
            => Size == o.Size && Precision == o.Precision && Scale == o.Scale;

        public static bool operator ==(PostgresFacets x, PostgresFacets y) => x.Equals(y);

        public static bool operator !=(PostgresFacets x, PostgresFacets y) => !(x == y);

        public override int GetHashCode()
        {
            var hashcode = Size?.GetHashCode() ?? 0;
            hashcode = (hashcode * 397) ^ (Precision?.GetHashCode() ?? 0);
            hashcode = (hashcode * 397) ^ (Scale?.GetHashCode() ?? 0);
            return hashcode;
        }

        public override string ToString()
        {
            if (Size == null && Precision == null && Scale == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append('(');
            var needComma = false;
            if (Size != null)
            {
                sb.Append(Size);
                needComma = true;
            }

            if (Precision != null)
            {
                if (needComma)
                    sb.Append(", ");
                sb.Append(Precision);
                needComma = true;
            }

            if (Scale != null)
            {
                if (needComma)
                    sb.Append(", ");
                sb.Append(Scale);
            }

            sb.Append(')');
            return sb.ToString();
        }
    }
}
