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
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return NpgsqlTypesHelper.TryGetNativeTypeInfo(type.GetGenericArguments()[0], out info);
			return NpgsqlTypesHelper.TryGetNativeTypeInfo(type, out info);
		}
		/// <summary>
		///	Convert .NET type to Postgres string representation
		/// </summary>
		public static string Convert(Type type, object value)
		{
			NpgsqlNativeTypeInfo info;
			bool canConvert;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				canConvert = NpgsqlTypesHelper.TryGetNativeTypeInfo(type.GetGenericArguments()[0], out info);
			else
			{
				if (type == typeof(string) && value != null)
					return "'" + (value as string).Replace("'", "''") + "'";
				canConvert = NpgsqlTypesHelper.TryGetNativeTypeInfo(type, out info);
			}
			if (!canConvert)
				throw new NpgsqlException("Can't convert " + type.FullName + " to native value");
			return info.ConvertToBackend(value, false);
		}
		/// <summary>
		///	Convert Postgres type name for .NET type
		/// </summary>
		public static string GetTypeName(Type type)
		{
			NpgsqlNativeTypeInfo info;
			bool canConvert;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				canConvert = NpgsqlTypesHelper.TryGetNativeTypeInfo(type.GetGenericArguments()[0], out info);
			else
				canConvert = NpgsqlTypesHelper.TryGetNativeTypeInfo(type, out info);
			if (!canConvert)
				throw new NpgsqlException("Can't convert " + type.FullName + " to native value");
			return info.Name;
		}
	}
}
