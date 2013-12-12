using System;
using System.Data.Common;
#if ENTITIES6
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif

namespace Npgsql.SqlGenerators {
    internal class SqlFunctionGenerator : SqlBaseGenerator {
        private DbFunctionCommandTree _commandTree;

        public SqlFunctionGenerator(DbFunctionCommandTree commandTree) {
            _commandTree = commandTree;
        }

        public override void BuildCommand(DbCommand command) {
            System.Diagnostics.Debug.Assert(command is NpgsqlCommand);

            var edmFunc = _commandTree.EdmFunction;
            if (String.IsNullOrEmpty(edmFunc.CommandTextAttribute)) {
                String text = "";
                if (false) { }
#if ENTITIES6
                else if (!String.IsNullOrEmpty(edmFunc.Schema)) text += QuoteIdentifier(edmFunc.Schema);
#endif
                else if (!String.IsNullOrEmpty(edmFunc.NamespaceName)) text += QuoteIdentifier(edmFunc.NamespaceName);

                if (!String.IsNullOrEmpty(text))
                    text += ".";

                if (false) { }
#if ENTITIES6
                else if (!String.IsNullOrEmpty(edmFunc.StoreFunctionNameAttribute)) text += QuoteIdentifier(edmFunc.StoreFunctionNameAttribute);
#endif
                else if (!String.IsNullOrEmpty(edmFunc.Name)) text += QuoteIdentifier(edmFunc.Name);

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = text;
            }
            else {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = _commandTree.EdmFunction.CommandTextAttribute;
            }

            command.Parameters.Clear();

            foreach (var kv in _commandTree.Parameters) {
                FunctionParameter funcParm;
                if (edmFunc != null && edmFunc.Parameters.TryGetValue(kv.Key, true, out funcParm)) {
                    //fp.Name; fp.TypeUsage; fp.Mode;
                    var pgParm = new NpgsqlParameter(funcParm.Name, DBNull.Value);
                    if (false) { }
                    else if (funcParm.Mode == ParameterMode.In) pgParm.Direction = System.Data.ParameterDirection.Input;
                    else if (funcParm.Mode == ParameterMode.Out) pgParm.Direction = System.Data.ParameterDirection.Output;
                    else if (funcParm.Mode == ParameterMode.InOut) pgParm.Direction = System.Data.ParameterDirection.InputOutput;
                    else if (funcParm.Mode == ParameterMode.ReturnValue) pgParm.Direction = System.Data.ParameterDirection.ReturnValue;
                    else { }

                    if (false) { }
                    else if (funcParm.TypeUsage.EdmType is PrimitiveType) {
                        var pt = (PrimitiveType)funcParm.TypeUsage.EdmType;
                        if (false) { }
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Binary) pgParm.DbType = System.Data.DbType.Binary;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Boolean) pgParm.DbType = System.Data.DbType.Boolean;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Byte) pgParm.DbType = System.Data.DbType.Byte;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.DateTime) pgParm.DbType = System.Data.DbType.DateTime;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.DateTimeOffset) pgParm.DbType = System.Data.DbType.DateTimeOffset;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Decimal) pgParm.DbType = System.Data.DbType.Decimal;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Double) pgParm.DbType = System.Data.DbType.Double;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Guid) pgParm.DbType = System.Data.DbType.Guid;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Int16) pgParm.DbType = System.Data.DbType.Int16;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Int32) pgParm.DbType = System.Data.DbType.Int32;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Int64) pgParm.DbType = System.Data.DbType.Int64;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.SByte) pgParm.DbType = System.Data.DbType.SByte;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Single) pgParm.DbType = System.Data.DbType.Single;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.String) pgParm.DbType = System.Data.DbType.String;
                        else if (pt.PrimitiveTypeKind == PrimitiveTypeKind.Time) pgParm.DbType = System.Data.DbType.Time;
                        else throw new NotSupportedException("Unknown PrimitiveTypeKind: " + pt.PrimitiveTypeKind);
                    }
                    else throw new NotSupportedException("EdmType has to be PrimitiveType");

                    command.Parameters.Add(pgParm);
                }
            }
        }

        public override VisitedExpression Visit(DbPropertyExpression expression) {
            throw new NotImplementedException();
        }
    }
}
