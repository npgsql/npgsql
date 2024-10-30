using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.Internal.Postgres;

namespace Npgsql;

/// <summary>
/// Internally represents a statement has been prepared, is in the process of being prepared, or is a
/// candidate for preparation (i.e. awaiting further usages).
/// </summary>
[DebuggerDisplay("{Name} ({State}): {Sql}")]
sealed class PreparedStatement
{
    readonly PreparedStatementManager _manager;

    internal string Sql { get; }

    internal byte[]? Name;

    internal RowDescriptionMessage? Description;

    internal int Usages;

    internal PreparedState State { get; set; }

    // Invalidated statement is still prepared and allocated on PG's side
    internal bool IsPrepared => State is PreparedState.Prepared or PreparedState.Invalidated;

    /// <summary>
    /// If true, the user explicitly requested this statement be prepared. It does not get closed as part of
    /// the automatic preparation LRU mechanism.
    /// </summary>
    internal bool IsExplicit { get; }

    /// <summary>
    /// If this statement is about to be prepared, but replaces a previous statement which needs to be closed,
    /// this holds the name of the previous statement. Otherwise null.
    /// </summary>
    internal PreparedStatement? StatementBeingReplaced;

    internal int AutoPreparedSlotIndex { get; set; }

    internal long LastUsed { get; set; }

    internal void RefreshLastUsed() => LastUsed = Stopwatch.GetTimestamp();

    /// <summary>
    /// Contains the handler types for a prepared statement's parameters, for overloaded cases (same SQL, different param types)
    /// Only populated after the statement has been prepared (i.e. null for candidates).
    /// </summary>
    PgTypeId[]? ConverterParamTypes { get; set; }

    internal static PreparedStatement CreateExplicit(
        PreparedStatementManager manager,
        string sql,
        string name,
        List<NpgsqlParameter> parameters,
        PreparedStatement? statementBeingReplaced)
    {
        var pStatement = new PreparedStatement(manager, sql, true)
        {
            Name = Encoding.ASCII.GetBytes(name),
            StatementBeingReplaced = statementBeingReplaced
        };
        pStatement.SetParamTypes(parameters);
        return pStatement;
    }

    internal static PreparedStatement CreateAutoPrepareCandidate(PreparedStatementManager manager, string sql)
        => new(manager, sql, false);

    internal PreparedStatement(PreparedStatementManager manager, string sql, bool isExplicit)
    {
        _manager = manager;
        Sql = sql;
        IsExplicit = isExplicit;
        State = PreparedState.NotPrepared;
    }

    internal void SetParamTypes(List<NpgsqlParameter> parameters)
    {
        if (parameters.Count == 0)
        {
            ConverterParamTypes = [];
            return;
        }

        ConverterParamTypes = new PgTypeId[parameters.Count];
        for (var i = 0; i < parameters.Count; i++)
            ConverterParamTypes[i] = parameters[i].PgTypeId;
    }

    internal bool DoParametersMatch(List<NpgsqlParameter> parameters)
    {
        var paramTypes = ConverterParamTypes!;
        if (paramTypes.Length != parameters.Count)
            return false;

        for (var i = 0; i < paramTypes.Length; i++)
            if (paramTypes[i] != parameters[i].PgTypeId)
                return false;

        return true;
    }

    internal void AbortPrepare()
    {
        Debug.Assert(State == PreparedState.BeingPrepared);

        // We were planned for preparation, but a failure occurred and we did not carry that out.
        // Remove it from the BySql dictionary, and place back the statement we were planned to replace (possibly null), setting
        // its state back to prepared.
        _manager.BySql.Remove(Sql);

        if (!IsExplicit)
        {
            _manager.AutoPrepared[AutoPreparedSlotIndex] = StatementBeingReplaced;
            if (StatementBeingReplaced is not null)
                StatementBeingReplaced.State = PreparedState.Prepared;
        }

        State = PreparedState.Unprepared;
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
    /// The statement is in the process of being prepared.
    /// </summary>
    BeingPrepared,

    /// <summary>
    /// The statement has been fully prepared and can be executed.
    /// </summary>
    Prepared,

    /// <summary>
    /// The statement is in the process of being unprepared. This is a temporary state that only occurs during
    /// unprepare. Specifically, it means that a Close message for the statement has already been written
    /// to the write buffer.
    /// </summary>
    BeingUnprepared,

    /// <summary>
    /// The statement has been unprepared and is no longer usable.
    /// </summary>
    Unprepared,

    /// <summary>
    /// The statement was invalidated because e.g. table schema has changed since preparation.
    /// </summary>
    Invalidated
}
