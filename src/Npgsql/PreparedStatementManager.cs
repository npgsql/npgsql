using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using NpgsqlTypes;

namespace Npgsql
{
    class PreparedStatementManager
    {
        internal int MaxAutoPrepared { get; }
        internal int UsagesBeforePrepare { get; }

        internal Dictionary<string, PreparedStatement> BySql { get; } = new Dictionary<string, PreparedStatement>();
        readonly PreparedStatement[] _autoPrepared;
        int _numAutoPrepared;

        [CanBeNull, ItemCanBeNull]
        readonly PreparedStatement[] _candidates;

        /// <summary>
        /// Total number of current prepared statements (whether explicit or automatic).
        /// </summary>
        internal int NumPrepared;

        readonly NpgsqlConnector _connector;

        internal string NextPreparedStatementName() => "_p" + (++_preparedStatementIndex);
        ulong _preparedStatementIndex;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

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
                _autoPrepared = new PreparedStatement[MaxAutoPrepared];
                _candidates = new PreparedStatement[CandidateCount];
            }
        }

        [CanBeNull]
        internal PreparedStatement GetOrAddExplicit(NpgsqlStatement statement)
        {
            var sql = statement.SQL;

            PreparedStatement statementBeingReplaced=null;
            if (BySql.TryGetValue(sql, out var pStatement))
            {
                Debug.Assert(pStatement.State != PreparedState.Unprepared);
                if (pStatement.IsExplicit)
                {
                    // Great, we've found an explicit prepared statement.
                    // We just need to check that the parameter types correspond, since prepared statements are
                    // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                    return pStatement.DoParametersMatch(statement.InputParameters)
                        ? pStatement
                        : null;
                }

                // We've found an autoprepare statement (candidate or otherwise)
                switch (pStatement.State)
                {
                case PreparedState.NotYetPrepared:
                    // Found a candidate for autopreparation. Remove it and prepare explicitly.
                    RemoveCandidate(pStatement);
                    break;
                case PreparedState.Prepared:
                    // The statement has already been autoprepared. We need to "promote" it to explicit.
                    statementBeingReplaced = pStatement;
                    break;
                case PreparedState.BeingPrepared:
                    throw new InvalidOperationException($"Found autoprepare statement in state {nameof(PreparedState.BeingPrepared)}");
                case PreparedState.Unprepared:
                    throw new InvalidOperationException($"Found unprepared statement in {nameof(PreparedStatementManager)}");
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

            // Statement hasn't been prepared yet
            return BySql[sql] = PreparedStatement.CreateExplicit(this, sql, NextPreparedStatementName(), statement.InputParameters, statementBeingReplaced);
        }

        [CanBeNull]
        internal PreparedStatement TryGetAutoPrepared(NpgsqlStatement statement)
        {
            Debug.Assert(_candidates != null);

            var sql = statement.SQL;
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

            if (pStatement.IsPrepared)
            {
                // The statement has already been prepared (explicitly or automatically).
                // We just need to check that the parameter types correspond, since prepared statements are
                // only keyed by SQL (to prevent pointless allocations). If we have a mismatch, simply run unprepared.
                return pStatement.DoParametersMatch(statement.InputParameters)
                    ? pStatement
                    : null;
            }

            if (++pStatement.Usages < UsagesBeforePrepare)
            {
                // Statement still hasn't passed the usage threshold, no automatic preparation.
                // Return null for unprepared exection.
                pStatement.LastUsed = DateTime.UtcNow;
                return null;
            }

            // Bingo, we've just passed the usage threshold, statement should get prepared
            Log.Trace($"Automatically preparing statement: {sql}", _connector.Id);

            RemoveCandidate(pStatement);

            if (_numAutoPrepared < MaxAutoPrepared)
            {
                // We still have free slots
                _autoPrepared[_numAutoPrepared++] = pStatement;
                pStatement.Name = "_auto" + _numAutoPrepared;
            }
            else
            {
                // We already have the maximum number of prepared statements.
                // Find the least recently used prepared statement and replace it.
                var oldestTimestamp = DateTime.MaxValue;
                var oldestIndex = -1;
                for (var i = 0; i < _autoPrepared.Length; i++)
                {
                    if (_autoPrepared[i].LastUsed < oldestTimestamp)
                    {
                        oldestIndex = i;
                        oldestTimestamp = _autoPrepared[i].LastUsed;
                    }
                }
                var lru = _autoPrepared[oldestIndex];
                pStatement.Name = lru.Name;
                pStatement.StatementBeingReplaced = lru;
                _autoPrepared[oldestIndex] = pStatement;
            }

            // Note that the parameter types are only set at the moment of preparation - in the candidate phase
            // there's no differentiation between overloaded statements, which are a pretty rare case, saving
            // allocations.
            pStatement.SetParamTypes(statement.InputParameters);

            return pStatement;
        }

        void RemoveCandidate(PreparedStatement candidate)
        {
            Debug.Assert(_candidates != null);
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
            _numAutoPrepared = 0;
            if (_candidates != null)
                for (var i = 0; i < _candidates.Length; i++)
                    _candidates[i] = null;
        }
    }
}
