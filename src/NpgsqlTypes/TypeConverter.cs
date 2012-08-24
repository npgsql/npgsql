using System;
using Npgsql;

namespace NpgsqlTypes
{
	/// <summary>
	///	Exposed type converter for outside usage.
	/// </summary>
	public static class TypeConverter
	{
		/// <summary>
		///	Check if Npgsql support .NET type conversion to Postgres type
		/// </summary>
		public static bool CanConvert(Type type)
		{
			NpgsqlNativeTypeInfo info;
			return NpgsqlTypesHelper.TryGetNativeTypeInfo(type, out info);
		}
		/// <summary>
		///	Convert .NET type to Postgres string representation
		/// </summary>
		public static string Convert(Type type, object value)
		{
			NpgsqlNativeTypeInfo info;
			if (!NpgsqlTypesHelper.TryGetNativeTypeInfo(type, out info))
				throw new NpgsqlException("Can't convert " + type.FullName + " to native value");
			return info.ConvertToBackend(value, false);
		}
	}
}
