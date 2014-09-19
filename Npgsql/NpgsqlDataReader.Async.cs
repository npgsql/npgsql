using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public partial class NpgsqlDataReader
    {
        internal async Task<Boolean> NextResultInternalAsync(CancellationToken ct)
        {
            try
            {
                CurrentRow = null;
                _currentResultsetSchema = null;
                _hasRows = false; // set to false and let the reading code determine if the set has rows.
                return (CurrentDescription = await GetNextRowDescriptionAsync(ct)) != null;
            }
            catch (IOException)
            {
                _command.Connection.ClearPool();
                throw;
            }            
        }

        private async Task<NpgsqlRowDescription> GetNextRowDescriptionAsync(CancellationToken ct)
        {
            if ((_behavior & CommandBehavior.SingleResult) != 0 && CurrentDescription != null)
            {
                CleanUp(false);
                return null;
            }
            NpgsqlRowDescription rd = _pendingDescription;
            while (rd == null)
            {
                object objNext = await GetNextServerMessageAsync(ct);
                if (objNext == null)
                {
                    break;
                }
                if (objNext is NpgsqlRow)
                {
                    (objNext as NpgsqlRow).Dispose();
                }

                rd = objNext as NpgsqlRowDescription;
            }

            _pendingDescription = null;

            // If there were records affected before,  keep track of their values.
            if (_recordsAffected != null)
                _recordsAffected += (_nextRecordsAffected ?? 0);
            else
                _recordsAffected = _nextRecordsAffected;

            _nextRecordsAffected = null;
            LastInsertedOID = _nextInsertOID;
            _nextInsertOID = null;
            return rd;
        }

        private async Task<IServerMessage> GetNextServerMessageAsync(CancellationToken ct, bool cleanup = false)
        {
            if (_cleanedUp)
                return null;
            try
            {
                CurrentRow = null;
                if (_pendingRow != null)
                {
                    _pendingRow.Dispose();
                }
                _pendingRow = null;
                while (true)
                {
                    var msg = await _connector.ReadMessageAsync(ct);

                    if (msg is RowReader)
                    {
                        RowReader reader = msg as RowReader;

                        if (cleanup)
                        {
                            // V3 rows can dispose by simply reading MessageLength bytes.
                            reader.Dispose();

                            return reader;
                        }
                        else
                        {
                            return _pendingRow = BuildRow(new ForwardsOnlyRow(reader));
                        }
                    }
                    else if (msg is CompletedResponse)
                    {
                        CompletedResponse cr = msg as CompletedResponse;
                        if (cr.RowsAffected.HasValue)
                        {
                            _nextRecordsAffected = (_nextRecordsAffected ?? 0) + cr.RowsAffected.Value;
                        }
                        _nextInsertOID = cr.LastInsertedOID ?? _nextInsertOID;
                    }
                    else if (msg is ReadyForQueryMsg)
                    {
                        CleanUp(true);
                        return null;
                    }
                    else
                    {
                        return msg;
                    }
                }
            }
            catch
            {
                CleanUp(true);
                throw;
            }
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            try
            {
                //CurrentRow = null;
                return (CurrentRow = await GetNextRowAsync(cancellationToken, true)) != null;
            }
            catch (IOException)
            {
                _command.Connection.ClearPool();
                throw;
            }            
        }

        private async Task<NpgsqlRow> GetNextRowAsync(CancellationToken ct, bool clearPending)
        {
            if (_pendingDescription != null)
            {
                return null;
            }
            if (((_behavior & CommandBehavior.SingleRow) != 0 && CurrentRow != null && _pendingDescription == null) ||
                ((_behavior & CommandBehavior.SchemaOnly) != 0))
            {
                if (!clearPending)
                {
                    return null;
                }
                //We should only have one row, and we've already had it. Move to end
                //of recordset.
                CurrentRow = null;
                for (object skip = await GetNextServerMessageAsync(ct);
                     skip != null && (_pendingDescription = skip as NpgsqlRowDescription) == null;
                     skip = await GetNextServerMessageAsync(ct))
                {
                    if (skip is NpgsqlRow)
                    {
                        (skip as NpgsqlRow).Dispose();
                    }
                }

                return null;
            }
            if (_pendingRow != null)
            {
                NpgsqlRow ret = _pendingRow;
                if (clearPending)
                {
                    _pendingRow = null;
                }
                if (!_hasRows)
                {
                    // when rows are found, store that this result has rows.
                    _hasRows = (ret != null);
                }
                return ret;
            }
            CurrentRow = null;
            object objNext = await GetNextServerMessageAsync(ct);
            if (clearPending)
            {
                _pendingRow = null;
            }
            if (objNext is NpgsqlRowDescription)
            {
                _pendingDescription = objNext as NpgsqlRowDescription;
                return null;
            }
            if (!_hasRows)
            {
                // when rows are found, store that this result has rows.
                _hasRows = objNext is NpgsqlRow;
            }
            return objNext as NpgsqlRow;
        }
    }
}
