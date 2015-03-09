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
                    switch (funcParm.Mode) {
                        case ParameterMode.In:
                            pgParm.Direction = System.Data.ParameterDirection.Input;
                            break;
                        case ParameterMode.Out:
                            pgParm.Direction = System.Data.ParameterDirection.Output;
                            break;
                        case ParameterMode.InOut:
                            pgParm.Direction = System.Data.ParameterDirection.InputOutput;
                            break;
                        case ParameterMode.ReturnValue:
                            pgParm.Direction = System.Data.ParameterDirection.ReturnValue;
                            break;
                    }

                    pgParm.DbType = NpgsqlProviderManifest.GetDbType((funcParm.TypeUsage.EdmType as PrimitiveType).PrimitiveTypeKind);

                    command.Parameters.Add(pgParm);
                }
            }
        }

        public override VisitedExpression Visit(DbPropertyExpression expression) {
            throw new NotImplementedException();
        }
    }
}
