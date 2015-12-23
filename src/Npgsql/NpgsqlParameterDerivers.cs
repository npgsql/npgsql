using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    internal interface INpgsqlParameterDeriver
    {
        void DeriveParameters(NpgsqlCommand command);
    }

    internal static class NpgsqlParameterDeriverFactory
    {
        private static readonly Version cutOffVersion1 = new Version(8, 1, 0);
        private static Lazy<NpgsqlParameterDeriver> deriver = new Lazy<NpgsqlParameterDeriver>(
            () => { return new NpgsqlParameterDeriver(); });
        private static Lazy<NpgsqlParameterDeriver2> deriver2 = new Lazy<NpgsqlParameterDeriver2>(
            () => { return new NpgsqlParameterDeriver2(); });

        public static INpgsqlParameterDeriver GetDeriver(Version serverVersion)
        {
            if (serverVersion < cutOffVersion1)
            {
                return deriver.Value;
            }
            else
            {
                return deriver2.Value;
            }
        }
    }

    internal class NpgsqlParameterDeriver : INpgsqlParameterDeriver
    {
        private const string pg_proc_query = @"
SELECT proretset, proargnames, proargtypes
  FROM pg_proc
 WHERE oid = :proname::regproc::oid";

        protected virtual string PgProcQuery
        {
            get
            {
                return pg_proc_query;
            }
        }

        public void DeriveParameters(NpgsqlCommand command)
        {
            command.Parameters.Clear();
            command.UnknownResultTypeList = null;
            using (var c = new NpgsqlCommand(PgProcQuery, command.Connection))
            {
                c.Parameters.AddWithValue("proname", NpgsqlDbType.Text, command.CommandText);
                using (var rdr = c.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                    {
                        bool[] unknownOutParameters = DeriveParametersFromPgProcRow(rdr, command.Parameters,
                            command.Connection.Connector.TypeHandlerRegistry);
                        command.UnknownResultTypeList = ((unknownOutParameters.Length > 0) ? unknownOutParameters : null);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Function {0} could not be found.", command.CommandText));
                    }

                }
            }
        }

        protected virtual bool[] DeriveParametersFromPgProcRow(NpgsqlDataReader row,
            NpgsqlParameterCollection parameters, TypeHandlerRegistry typeInfo)
        {
            uint[] argTypes = GetArgTypeOIDs(row);
            string[] argNames = GetArgNames(row);
            char[] argmodes = GetArgModes(row);
            List<bool> unknownResultTypes = new List<bool>();

            bool isSetReturning = IsSetReturningFunction(row);

            for (int i = 0; i < argTypes.Length; i++)
            {
                ParameterDirection direction;
                if (i < argmodes.Length)
                {
                    char argMode = argmodes[i];
                    direction = GetParameterDirectionFromArgMode(argMode);
                }
                else
                {
                    direction = ParameterDirection.Input;
                }


                NpgsqlParameter param = new NpgsqlParameter();
                param.Direction = direction;

                // TODO: Fix composite types
                uint argOID = argTypes[i];

                var typeHandler = typeInfo[argOID];
                if (typeHandler.NpgsqlDbType == (NpgsqlDbType)0)
                {
                    param.NpgsqlDbType = NpgsqlDbType.Unknown;
                }
                else
                {
                    param.NpgsqlDbType = typeHandler.NpgsqlDbType;
                }

                if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
                {
                    unknownResultTypes.Add(param.NpgsqlDbType == NpgsqlDbType.Unknown ? true : false);
                    if (direction == ParameterDirection.Output && isSetReturning)
                    {
                        continue;
                    }
                }

                if (typeHandler.EnumType != null)
                {
                    param.EnumType = typeHandler.EnumType;
                }

                if (i < argNames.Length)
                {
                    param.ParameterName = ":" + argNames[i];
                }
                else
                {
                    param.ParameterName = "parameter" + (i + 1);
                }

                parameters.Add(param);
            }
            return unknownResultTypes.ToArray();
        }

        protected virtual ParameterDirection GetParameterDirectionFromArgMode(char argMode)
        {
            return ParameterDirection.Input;
        }

        protected virtual NpgsqlDbType GetNpgsqlDbTypeFromOID(TypeHandlerRegistry typeInfo, uint proArgType)
        {
            NpgsqlDbType npgsqlDbType = typeInfo[proArgType].NpgsqlDbType;
            if (npgsqlDbType == NpgsqlDbType.Unknown)
                throw new InvalidOperationException(string.Format("Invalid parameter type: {0}", proArgType));
            return npgsqlDbType;
        }

        protected virtual uint[] GetArgTypeOIDs(NpgsqlDataReader row)
        {
            uint[] retVal;
            int ordinal = row.GetOrdinal("proargtypes");

            if (row.IsDBNull(ordinal))
            {
                retVal = new uint[0];
            }
            else
            {
                retVal = row.GetFieldValue<uint[]>(ordinal);
            }
            return retVal;
        }

        protected virtual bool IsSetReturningFunction(NpgsqlDataReader row)
        {
            int ordinal = row.GetOrdinal("proretset");
            return row.GetFieldValue<bool>(ordinal);
        }

        protected virtual string[] GetArgNames(NpgsqlDataReader row)
        {
            string[] retVal;
            int ordinal = row.GetOrdinal("proargnames");

            if (row.IsDBNull(ordinal))
            {
                retVal = new string[0];
            }
            else
            {
                retVal = (string[])row.GetValue(ordinal);
            }
            return retVal;
        }

        protected virtual char[] GetArgModes(NpgsqlDataReader row)
        {
            return new char[0];
        }

    }

    internal class NpgsqlParameterDeriver2 : NpgsqlParameterDeriver
    {

        private const string pg_proc_query = @"
SELECT proretset, proargnames, proargtypes, proallargtypes, proargmodes
  FROM pg_proc
 WHERE oid = :proname::regproc::oid";

        protected override string PgProcQuery
        {
            get
            {
                return pg_proc_query;
            }
        }

        protected override uint[] GetArgTypeOIDs(NpgsqlDataReader row)
        {
            uint[] retVal;
            int ordinal = row.GetOrdinal("proallargtypes");

            if (row.IsDBNull(ordinal))
            {
                retVal = base.GetArgTypeOIDs(row);
            }
            else
            {
                retVal = (uint[])row.GetValue(ordinal);
            }
            return retVal;
        }

        protected override char[] GetArgModes(NpgsqlDataReader row)
        {
            char[] retVal;
            int ordinal = row.GetOrdinal("proargmodes");

            if (row.IsDBNull(ordinal))
            {
                retVal = new char[0];
            }
            else
            {
                retVal = (char[])row.GetValue(ordinal);
            }
            return retVal;
        }

        protected override ParameterDirection GetParameterDirectionFromArgMode(char argMode)
        {
            switch (argMode)
            {
                case 'i':
                    return ParameterDirection.Input;
                case 'o':
                case 't':
                    return ParameterDirection.Output;
                case 'b':
                    return ParameterDirection.InputOutput;
                case 'v':
                    throw new NotImplementedException("Cannot derive function parameter of type VARIADIC");
                default:
                    throw new ArgumentOutOfRangeException("proargmode", argMode,
                        "Unknown code in proargmodes while deriving: " + argMode);
            }
        }
    }
}
