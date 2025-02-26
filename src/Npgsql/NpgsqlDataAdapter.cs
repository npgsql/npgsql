using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql;

/// <summary>
/// Represents the method that handles the <see cref="NpgsqlDataAdapter.RowUpdated"/> events.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">An <see cref="NpgsqlRowUpdatedEventArgs"/> that contains the event data.</param>
public delegate void NpgsqlRowUpdatedEventHandler(object sender, NpgsqlRowUpdatedEventArgs e);

/// <summary>
/// Represents the method that handles the <see cref="NpgsqlDataAdapter.RowUpdating"/> events.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">An <see cref="NpgsqlRowUpdatingEventArgs"/> that contains the event data.</param>
public delegate void NpgsqlRowUpdatingEventHandler(object sender, NpgsqlRowUpdatingEventArgs e);

/// <summary>
/// This class represents an adapter from many commands: select, update, insert and delete to fill a <see cref="System.Data.DataSet"/>.
/// </summary>
[System.ComponentModel.DesignerCategory("")]
public sealed class NpgsqlDataAdapter : DbDataAdapter
{
    /// <summary>
    /// Row updated event.
    /// </summary>
    public event NpgsqlRowUpdatedEventHandler? RowUpdated;

    /// <summary>
    /// Row updating event.
    /// </summary>
    public event NpgsqlRowUpdatingEventHandler? RowUpdating;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public NpgsqlDataAdapter() {}

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="selectCommand"></param>
    public NpgsqlDataAdapter(NpgsqlCommand selectCommand)
        => SelectCommand = selectCommand;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="selectCommandText"></param>
    /// <param name="selectConnection"></param>
    public NpgsqlDataAdapter(string selectCommandText, NpgsqlConnection selectConnection)
        : this(new NpgsqlCommand(selectCommandText, selectConnection)) {}

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="selectCommandText"></param>
    /// <param name="selectConnectionString"></param>
    public NpgsqlDataAdapter(string selectCommandText, string selectConnectionString)
        : this(selectCommandText, new NpgsqlConnection(selectConnectionString)) {}

    /// <summary>
    /// Create row updated event.
    /// </summary>
    protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand? command,
        System.Data.StatementType statementType,
        DataTableMapping tableMapping)
        => new NpgsqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);

    /// <summary>
    /// Create row updating event.
    /// </summary>
    protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand? command,
        System.Data.StatementType statementType,
        DataTableMapping tableMapping)
        => new NpgsqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);

    /// <summary>
    /// Raise the RowUpdated event.
    /// </summary>
    /// <param name="value"></param>
    protected override void OnRowUpdated(RowUpdatedEventArgs value)
    {
        //base.OnRowUpdated(value);
        if (value is NpgsqlRowUpdatedEventArgs args)
            RowUpdated?.Invoke(this, args);
        //if (RowUpdated != null && value is NpgsqlRowUpdatedEventArgs args)
        //    RowUpdated(this, args);
    }

    /// <summary>
    /// Raise the RowUpdating event.
    /// </summary>
    /// <param name="value"></param>
    protected override void OnRowUpdating(RowUpdatingEventArgs value)
    {
        if (value is NpgsqlRowUpdatingEventArgs args)
            RowUpdating?.Invoke(this, args);
    }

    /// <summary>
    /// Delete command.
    /// </summary>
    public new NpgsqlCommand? DeleteCommand
    {
        get => (NpgsqlCommand?)base.DeleteCommand;
        set => base.DeleteCommand = value;
    }

    /// <summary>
    /// Select command.
    /// </summary>
    public new NpgsqlCommand? SelectCommand
    {
        get => (NpgsqlCommand?)base.SelectCommand;
        set => base.SelectCommand = value;
    }

    /// <summary>
    /// Update command.
    /// </summary>
    public new NpgsqlCommand? UpdateCommand
    {
        get => (NpgsqlCommand?)base.UpdateCommand;
        set => base.UpdateCommand = value;
    }

    /// <summary>
    /// Insert command.
    /// </summary>
    public new NpgsqlCommand? InsertCommand
    {
        get => (NpgsqlCommand?)base.InsertCommand;
        set => base.InsertCommand = value;
    }

    // Temporary implementation, waiting for official support in System.Data via https://github.com/dotnet/runtime/issues/22109
    [RequiresUnreferencedCode("Members from serialized types or types used in expressions may be trimmed if not referenced directly.")]
    internal async Task<int> Fill(DataTable dataTable, bool async, CancellationToken cancellationToken = default)
    {
        var command = SelectCommand;
        var activeConnection = command?.Connection ?? throw new InvalidOperationException("Connection required");
        var originalState = ConnectionState.Closed;

        try
        {
            originalState = activeConnection.State;
            if (ConnectionState.Closed == originalState)
                await activeConnection.Open(async, cancellationToken).ConfigureAwait(false);

            var dataReader = await command.ExecuteReader(async, CommandBehavior.Default, cancellationToken).ConfigureAwait(false);
            try
            {
                return await Fill(dataTable, dataReader, async, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (async)
                    await dataReader.DisposeAsync().ConfigureAwait(false);
                else
                    dataReader.Dispose();
            }
        }
        finally
        {
            if (ConnectionState.Closed == originalState)
                activeConnection.Close();
        }
    }

    [RequiresUnreferencedCode("Members from serialized types or types used in expressions may be trimmed if not referenced directly.")]
    async Task<int> Fill(DataTable dataTable, NpgsqlDataReader dataReader, bool async, CancellationToken cancellationToken = default)
    {
        dataTable.BeginLoadData();
        try
        {
            var rowsAdded = 0;
            var count = dataReader.FieldCount;
            var columnCollection = dataTable.Columns;
            for (var i = 0; i < count; ++i)
            {
                var fieldName = dataReader.GetName(i);
                if (!columnCollection.Contains(fieldName))
                {
                    var fieldType = dataReader.GetFieldType(i);
                    var dataColumn = new DataColumn(fieldName, fieldType);
                    columnCollection.Add(dataColumn);
                }
            }

            var values = new object[count];

            while (async ? await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false) : dataReader.Read())
            {
                dataReader.GetValues(values);
                dataTable.LoadDataRow(values, true);
                rowsAdded++;
            }
            return rowsAdded;
        }
        finally
        {
            dataTable.EndLoadData();
        }
    }
}

#pragma warning disable 1591

public class NpgsqlRowUpdatingEventArgs(
    DataRow dataRow,
    IDbCommand? command,
    System.Data.StatementType statementType,
    DataTableMapping tableMapping)
    : RowUpdatingEventArgs(dataRow, command, statementType, tableMapping);

public class NpgsqlRowUpdatedEventArgs(
    DataRow dataRow,
    IDbCommand? command,
    System.Data.StatementType statementType,
    DataTableMapping tableMapping)
    : RowUpdatedEventArgs(dataRow, command, statementType, tableMapping);

#pragma warning restore 1591
