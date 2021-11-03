using System;
using System.Collections.Generic;
using System.Diagnostics;
using Npgsql.Internal;
using Npgsql.Logging;

namespace Npgsql
{
    class PreparedStatementManager
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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(PreparedStatementManager));

        internal const int CandidateCount = 100;

        internal PreparedStatementManager(NpgsqlConnector connector)
        {
            _connector = connector;
            MaxAutoPrepared = connector.Settings.MaxAutoPrepare;
            UsagesBeforePrepare = connector.Settings.AutoPrepareMinUsages;
            if (MaxAutoPrepared > 0)
            {
                if (MaxAutoPrepared > 256)
                    Log.Warn($"{nameof(MaxAutoPrepared)} is over 256, performance degradation may occur. Please report via an issue.", connector.Id);
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
                if (pStatement.IsExplicit)
                {
                    // Great, we've found an explicit prepared statement.
                    // We just need to check that the parameter types correspond, since prepared statements are
                    // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                    return pStatement.DoParametersMatch(batchCommand.PositionalParameters!)
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
                case PreparedState.Prepared:
                    // The statement has already been autoprepared. We need to "promote" it to explicit.
                    statementBeingReplaced = pStatement;
                    break;
                case PreparedState.Unprepared:
                    throw new InvalidOperationException($"Found unprepared statement in {nameof(PreparedStatementManager)}");
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

            // Statement hasn't been prepared yet
            return BySql[sql] = PreparedStatement.CreateExplicit(this, sql, NextPreparedStatementName(), batchCommand.PositionalParameters, statementBeingReplaced);
        }

        internal PreparedStatement? TryGetAutoPrepared(NpgsqlBatchCommand batchCommand)
        {
            var sql = batchCommand.FinalCommandText!;
            if (!BySql.TryGetValue(sql, out var pStatement))
            {
                // New candidate. Find an empty candidate slot or eject a least-used one.
                int slotIndex = -1, leastUsages = int.MaxValue;
                var lastUsed = DateTime.MaxValue;
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
                break;

            case PreparedState.Prepared:
            case PreparedState.BeingPrepared:
                // The statement has already been prepared (explicitly or automatically), or has been selected
                // for preparation (earlier identical statement in the same command).
                // We just need to check that the parameter types correspond, since prepared statements are
                // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                if (!pStatement.DoParametersMatch(batchCommand.PositionalParameters))
                    return null;
                // Prevent this statement from being replaced within this batch
                pStatement.LastUsed = DateTime.MaxValue;
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
                pStatement.LastUsed = DateTime.UtcNow;
                return null;
            }

            // Bingo, we've just passed the usage threshold, statement should get prepared
            Log.Trace($"Automatically preparing statement: {sql}", _connector.Id);

            // Look for either an empty autoprepare slot, or the least recently used prepared statement which we'll replace it.
            var oldestTimestamp = DateTime.MaxValue;
            var selectedIndex = -1;
            for (var i = 0; i < AutoPrepared.Length; i++)
            {
                var slot = AutoPrepared[i];

                if (slot is null)
                {
                    // We found a free slot, exit the loop immediately
                    selectedIndex = i;
                    break;
                }

                switch (slot.State)
                {
                case PreparedState.Prepared:
                    if (slot.LastUsed < oldestTimestamp)
                    {
                        selectedIndex = i;
                        oldestTimestamp = slot.LastUsed;
                    }
                    break;

                case PreparedState.BeingPrepared:
                    // Slot has already been selected for preparation by an earlier statement in this batch. Skip it.
                    continue;

                default:
                    throw new Exception(
                        $"Invalid {nameof(PreparedState)} state {slot.State} encountered when scanning prepared statement slots");
                }
            }

            if (selectedIndex == -1)
            {
                // We're here if we couldn't find a free slot or a prepared statement to replace - this means all slots are taken by
                // statements being prepared in this batch.
                return null;
            }

            var oldPreparedStatement = AutoPrepared[selectedIndex];

            if (oldPreparedStatement is null)
            {
                pStatement.Name = "_auto" + selectedIndex;
            }
            else
            {
                pStatement.Name = oldPreparedStatement.Name;
                pStatement.StatementBeingReplaced = oldPreparedStatement;
                oldPreparedStatement.State = PreparedState.BeingUnprepared;
            }

            pStatement.AutoPreparedSlotIndex = selectedIndex;
            AutoPrepared[selectedIndex] = pStatement;

            RemoveCandidate(pStatement);

            // Make sure this statement isn't replaced by a later statement in the same batch.
            pStatement.LastUsed = DateTime.MaxValue;

            // Note that the parameter types are only set at the moment of preparation - in the candidate phase
            // there's no differentiation between overloaded statements, which are a pretty rare case, saving
            // allocations.
            pStatement.SetParamTypes(batchCommand.PositionalParameters);

            return pStatement;
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
}
