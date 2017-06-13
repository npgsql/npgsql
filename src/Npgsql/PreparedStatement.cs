using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Internally represents a statement has been prepared, is in the process of being prepared, or is a
    /// candidate for preparation (i.e. awaiting further usages).
    /// </summary>
    class PreparedStatement
    {
        readonly PreparedStatementManager _manager;

        internal string Sql { get; }

        internal string Name;

        [CanBeNull]
        internal RowDescriptionMessage Description;

        internal int Usages;

        internal PreparedState State { get; set; }

        internal bool IsPrepared => State == PreparedState.Prepared;

        /// <summary>
        /// If true, the user explicitly requested this statement be prepared. It does not get closed as part of
        /// the automatic preparation LRU mechanism.
        /// </summary>
        internal bool IsExplicit { get; private set; }

        /// <summary>
        /// If this statement is about to be prepared, but replaces a previous statement which needs to be closed,
        /// this holds the name of the previous statement. Otherwise null.
        /// </summary>
        [CanBeNull]
        internal PreparedStatement StatementBeingReplaced;

        internal DateTime LastUsed { get; set; }

        /// <summary>
        /// Contains the parameter types for a prepared statement, for overloaded cases (same SQL, different param types)
        /// Only populated after the statement has been prepared (i.e. null for candidates).
        /// </summary>
        [CanBeNull]
        internal NpgsqlDbType[] ParamTypes { get; private set; }

        static readonly NpgsqlDbType[] EmptyParamTypes = new NpgsqlDbType[0];

        internal static PreparedStatement CreateExplicit(
            PreparedStatementManager manager,
            string sql,
            string name,
            List<NpgsqlParameter> parameters,
            [CanBeNull] PreparedStatement statementBeingReplaced)
        {
            var pStatement = new PreparedStatement(manager, sql, true)
            {
                Name = name,
                StatementBeingReplaced = statementBeingReplaced
            };
            pStatement.SetParamTypes(parameters);
            return pStatement;
        }

        internal static PreparedStatement CreateAutoPrepareCandidate(PreparedStatementManager manager, string sql)
            => new PreparedStatement(manager, sql, false);

        PreparedStatement(PreparedStatementManager manager, string sql, bool isExplicit)
        {
            _manager = manager;
            Sql = sql;
            IsExplicit = isExplicit;
            State = PreparedState.NotPrepared;
        }

        internal void SetParamTypes(List<NpgsqlParameter> parameters)
        {
            Debug.Assert(ParamTypes == null);
            if (parameters.Count == 0)
                ParamTypes = EmptyParamTypes;
            ParamTypes = new NpgsqlDbType[parameters.Count];
            for (var i = 0; i < parameters.Count; i++)
                ParamTypes[i] = parameters[i].NpgsqlDbType;
        }

        internal bool DoParametersMatch(List<NpgsqlParameter> parameters)
        {
            Debug.Assert(ParamTypes != null);
            if (ParamTypes.Length != parameters.Count)
                return false;
            for (var i = 0; i < ParamTypes.Length; i++)
                if (ParamTypes[i] != parameters[i].NpgsqlDbType)
                    return false;
            return true;
        }

        internal void CompletePrepare()
        {
            Debug.Assert(ParamTypes != null);
            _manager.BySql[Sql] = this;
            _manager.NumPrepared++;
            State = PreparedState.Prepared;
        }

        internal void CompleteUnprepare()
        {
            _manager.BySql.Remove(Sql);
            _manager.NumPrepared--;
            State = PreparedState.Unprepared;
        }

        public override string ToString() => Sql;
    }

    /// <summary>
    /// The state of a <see cref="PreparedStatement"/>.
    /// </summary>
    enum PreparedState
    {
        /// <summary>
        /// The statement hasn't been prepared yet, nor is it in the process of being prepared.
        /// This is the value for autoprepare candidates which haven't been prepared yet, and is also
        /// a temporary state during preparation.
        /// </summary>
        NotPrepared,

        /// <summary>
        /// The statement has been selected for preparation, but the preparation hasn't started yet.
        /// This is a temporary state that only occurs during preparation.
        /// Specifically, no protocol message (Parse) has been sent yet. Specifically, it means that
        /// a Parse message for the statement has already been written to the write buffer.
        /// </summary>
        ToBePrepared,

        /// <summary>
        /// The statement is in the process of being prepared. This is a temporary state that only occurs during
        /// preparation. Specifically, it means that a Parse message for the statement has already been written
        /// to the write buffer.
        /// </summary>
        BeingPrepared,

        /// <summary>
        /// The statement has been fully prepared and can be executed.
        /// </summary>
        Prepared,

        /// <summary>
        /// The statement is in the process of being unprepared. This is a temporary state that only occurs during
        /// unpreparation. Specifically, it means that a Close message for the statement has already been written
        /// to the write buffer.
        /// </summary>
        BeingUnprepared,

        /// <summary>
        /// The statement has been unprepared and is no longer usable.
        /// </summary>
        Unprepared
    }
}
