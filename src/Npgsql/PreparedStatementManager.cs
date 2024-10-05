using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;

namespace Npgsql;

sealed class PreparedStatementManager
{
    internal int MaxAutoPrepared { get; }
    internal int UsagesBeforePrepare { get; }

    internal Dictionary<string, PreparedStatement> BySql { get; } = new();
    internal PreparedStatement?[] AutoPrepared { get; }

    readonly PreparedStatement?[] _candidates;

    /// <summary>
    /// Total number of current prepared statements (whether explicit or automatic).
    /// </summary>
    internal int NumPrepared;

    readonly NpgsqlConnector _connector;

    internal string NextPreparedStatementName() => "_p" + (++_preparedStatementIndex);
    ulong _preparedStatementIndex;

    readonly ILogger _commandLogger;

    internal const int CandidateCount = 100;

    internal PreparedStatementManager(NpgsqlConnector connector)
    {
        _connector = connector;
        _commandLogger = connector.LoggingConfiguration.CommandLogger;

        MaxAutoPrepared = connector.Settings.MaxAutoPrepare;
        UsagesBeforePrepare = connector.Settings.AutoPrepareMinUsages;

        if (MaxAutoPrepared > 0)
        {
            if (MaxAutoPrepared > 256)
                _commandLogger.LogWarning($"{nameof(MaxAutoPrepared)} is over 256, performance degradation may occur. Please report via an issue.");
            AutoPrepared = new PreparedStatement[MaxAutoPrepared];
            _candidates = new PreparedStatement[CandidateCount];
        }
        else
        {
            AutoPrepared = null!;
            _candidates = null!;
        }
    }

    internal PreparedStatement? GetOrAddExplicit(NpgsqlBatchCommand batchCommand)
    {
        var sql = batchCommand.FinalCommandText!;

        PreparedStatement? statementBeingReplaced = null;
        if (BySql.TryGetValue(sql, out var pStatement))
        {
            Debug.Assert(pStatement.State != PreparedState.Unprepared);
            // If statement is invalidated, fall through below where we replace it with another
            if (pStatement.IsExplicit && pStatement.State != PreparedState.Invalidated)
            {
                // Great, we've found an explicit prepared statement.
                // We just need to check that the parameter types correspond, since prepared statements are
                // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                return pStatement.DoParametersMatch(batchCommand.CurrentParametersReadOnly)
                    ? pStatement
                    : null;
            }

            // We've found an autoprepare statement (candidate or otherwise)
            switch (pStatement.State)
            {
            case PreparedState.NotPrepared:
                // Found a candidate for autopreparation. Remove it and prepare explicitly.
                RemoveCandidate(pStatement);
                break;
            // The statement is invalidated. Just replace it with a new one.
            case PreparedState.Invalidated:
            // The statement has already been autoprepared. We need to "promote" it to explicit.
            case PreparedState.Prepared:
                statementBeingReplaced = pStatement;
                break;
            case PreparedState.Unprepared:
                throw new InvalidOperationException($"Found unprepared statement in {nameof(PreparedStatementManager)}");
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        // Statement hasn't been prepared yet
        return BySql[sql] = PreparedStatement.CreateExplicit(this, sql, NextPreparedStatementName(), batchCommand.CurrentParametersReadOnly, statementBeingReplaced);
    }

    internal PreparedStatement? TryGetAutoPrepared(NpgsqlBatchCommand batchCommand)
    {
        var sql = batchCommand.FinalCommandText!;
        // We could also test for PreparedState.BeingPrepared as it's handled the exact same way as PreparedState.Prepared
        // But since it's so rare we'll just go through the slow path
        if (!BySql.TryGetValue(sql, out var pStatement) || pStatement.State != PreparedState.Prepared)
            return TryGetAutoPreparedSlow(batchCommand, pStatement);

        // The statement has already been prepared (explicitly or automatically)
        // We just need to check that the parameter types correspond, since prepared statements are
        // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
        if (!pStatement.DoParametersMatch(batchCommand.CurrentParametersReadOnly))
            return null;
        // Prevent this statement from being replaced within this batch
        pStatement.LastUsed = long.MaxValue;
        return pStatement;

        PreparedStatement? TryGetAutoPreparedSlow(NpgsqlBatchCommand batchCommand, PreparedStatement? pStatement)
        {
            var sql = batchCommand.FinalCommandText!;
            if (pStatement is null)
            {
                // New candidate. Find an empty candidate slot or eject a least-used one.
                int slotIndex = -1, leastUsages = int.MaxValue;
                var lastUsed = long.MaxValue;
                for (var i = 0; i < _candidates.Length; i++)
                {
                    var candidate = _candidates[i];
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    // ReSharper disable HeuristicUnreachableCode
                    if (candidate == null)  // Found an unused candidate slot, return immediately
                    {
                        slotIndex = i;
                        break;
                    }
                    // ReSharper restore HeuristicUnreachableCode
                    if (candidate.Usages < leastUsages)
                    {
                        leastUsages = candidate.Usages;
                        slotIndex = i;
                        lastUsed = candidate.LastUsed;
                    }
                    else if (candidate.Usages == leastUsages && candidate.LastUsed < lastUsed)
                    {
                        slotIndex = i;
                        lastUsed = candidate.LastUsed;
                    }
                }

                var leastUsed = _candidates[slotIndex];
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (leastUsed != null)
                    BySql.Remove(leastUsed.Sql);
                pStatement = BySql[sql] = _candidates[slotIndex] = PreparedStatement.CreateAutoPrepareCandidate(this, sql);
            }

            switch (pStatement.State)
            {
            case PreparedState.NotPrepared:
            case PreparedState.Invalidated:
                break;

            // We shouldn't ever get PreparedState.Prepared since it's handled above but handle it here just in case
            case PreparedState.Prepared:
            case PreparedState.BeingPrepared:
                // The statement has already been prepared (explicitly or automatically), or has been selected
                // for preparation (earlier identical statement in the same command).
                // We just need to check that the parameter types correspond, since prepared statements are
                // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                if (!pStatement.DoParametersMatch(batchCommand.CurrentParametersReadOnly))
                    return null;
                // Prevent this statement from being replaced within this batch
                pStatement.LastUsed = long.MaxValue;
                return pStatement;

            case PreparedState.BeingUnprepared:
                // The statement is being replaced by an earlier statement in this same batch.
                return null;

            default:
                Debug.Fail($"Unexpected {nameof(PreparedState)} in auto-preparation: {pStatement.State}");
                break;
            }

            if (++pStatement.Usages < UsagesBeforePrepare)
            {
                // Statement still hasn't passed the usage threshold, no automatic preparation.
                // Return null for unprepared execution.
                pStatement.RefreshLastUsed();
                return null;
            }

            // Bingo, we've just passed the usage threshold, statement should get prepared
            LogMessages.AutoPreparingStatement(_commandLogger, sql, _connector.Id);

            // Look for either an empty autoprepare slot, or the least recently used prepared statement which we'll replace it.
            var oldestLastUsed = long.MaxValue;
            var selectedIndex = -1;
            for (var i = 0; i < AutoPrepared.Length; i++)
            {
                var slot = AutoPrepared[i];

                if (slot is null or { State: PreparedState.Invalidated })
                {
                    // We found a free or invalidated slot, exit the loop immediately
                    selectedIndex = i;
                    break;
                }

                switch (slot.State)
                {
                case PreparedState.Prepared:
                    if (slot.LastUsed < oldestLastUsed)
                    {
                        selectedIndex = i;
                        oldestLastUsed = slot.LastUsed;
                    }
                    break;

                case PreparedState.BeingPrepared:
                    // Slot has already been selected for preparation by an earlier statement in this batch. Skip it.
                    continue;

                default:
                    ThrowHelper.ThrowInvalidOperationException($"Invalid {nameof(PreparedState)} state {slot.State} encountered when scanning prepared statement slots");
                    return null;
                }
            }

            if (selectedIndex < 0)
            {
                // We're here if we couldn't find a free slot or a prepared statement to replace - this means all slots are taken by
                // statements being prepared in this batch.
                return null;
            }

            if (pStatement.State != PreparedState.Invalidated)
                RemoveCandidate(pStatement);

            var oldPreparedStatement = AutoPrepared[selectedIndex];

            if (oldPreparedStatement is null)
            {
                pStatement.Name = Encoding.ASCII.GetBytes("_auto" + selectedIndex);
            }
            else
            {
                // When executing an invalidated prepared statement, the old and the new statements are the same instance.
                // Create a copy so that we have two distinct instances with their own states.
                if (oldPreparedStatement == pStatement)
                {
                    oldPreparedStatement = new PreparedStatement(this, oldPreparedStatement.Sql, isExplicit: false)
                    {
                        Name = oldPreparedStatement.Name
                    };
                }

                pStatement.Name = oldPreparedStatement.Name;
                pStatement.State = PreparedState.NotPrepared;
                pStatement.StatementBeingReplaced = oldPreparedStatement;
                oldPreparedStatement.State = PreparedState.BeingUnprepared;
            }

            pStatement.AutoPreparedSlotIndex = selectedIndex;
            AutoPrepared[selectedIndex] = pStatement;


            // Make sure this statement isn't replaced by a later statement in the same batch.
            pStatement.LastUsed = long.MaxValue;

            // Note that the parameter types are only set at the moment of preparation - in the candidate phase
            // there's no differentiation between overloaded statements, which are a pretty rare case, saving
            // allocations.
            pStatement.SetParamTypes(batchCommand.CurrentParametersReadOnly);

            return pStatement;
        }
    }

    void RemoveCandidate(PreparedStatement candidate)
    {
        var i = 0;
        for (; i < _candidates.Length; i++)
        {
            if (_candidates[i] == candidate)
            {
                _candidates[i] = null;
                return;
            }
        }
        Debug.Assert(i < _candidates.Length);
    }

    internal void ClearAll()
    {
        BySql.Clear();
        NumPrepared = 0;
        _preparedStatementIndex = 0;
        if (AutoPrepared is not null)
            for (var i = 0; i < AutoPrepared.Length; i++)
                AutoPrepared[i] = null;
        if (_candidates != null)
            for (var i = 0; i < _candidates.Length; i++)
                _candidates[i] = null;
    }
}
