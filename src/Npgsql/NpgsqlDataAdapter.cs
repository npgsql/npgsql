using System.Data;
using System.Data.Common;
using JetBrains.Annotations;

namespace Npgsql
{
    /// <summary>
    /// Represents the method that handles the <see cref="NpgsqlDataAdapter.RowUpdated">RowUpdated</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlRowUpdatedEventArgs">NpgsqlRowUpdatedEventArgs</see> that contains the event data.</param>
    public delegate void NpgsqlRowUpdatedEventHandler(object sender, NpgsqlRowUpdatedEventArgs e);

    /// <summary>
    /// Represents the method that handles the <see cref="NpgsqlDataAdapter.RowUpdating">RowUpdating</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NpgsqlRowUpdatingEventArgs">NpgsqlRowUpdatingEventArgs</see> that contains the event data.</param>
    public delegate void NpgsqlRowUpdatingEventHandler(object sender, NpgsqlRowUpdatingEventArgs e);

    /// <summary>
    /// This class represents an adapter from many commands: select, update, insert and delete to fill <see cref="System.Data.DataSet">Datasets.</see>
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public sealed class NpgsqlDataAdapter : DbDataAdapter
    {
        /// <summary>
        /// Row updated event.
        /// </summary>
        [PublicAPI]
        public event NpgsqlRowUpdatedEventHandler RowUpdated;

        /// <summary>
        /// Row updating event.
        /// </summary>
        public event NpgsqlRowUpdatingEventHandler RowUpdating;

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
        protected override RowUpdatedEventArgs CreateRowUpdatedEvent([NotNull] DataRow dataRow, [NotNull] IDbCommand command,
                                                                     System.Data.StatementType statementType,
                                                                     [NotNull] DataTableMapping tableMapping)
            => new NpgsqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);

        /// <summary>
        /// Create row updating event.
        /// </summary>
        protected override RowUpdatingEventArgs CreateRowUpdatingEvent([NotNull] DataRow dataRow, [NotNull] IDbCommand command,
                                                                       System.Data.StatementType statementType,
                                                                       [NotNull] DataTableMapping tableMapping)
            => new NpgsqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);

        /// <summary>
        /// Raise the RowUpdated event.
        /// </summary>
        /// <param name="value"></param>
        protected override void OnRowUpdated([NotNull] RowUpdatedEventArgs value)
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
        protected override void OnRowUpdating([NotNull] RowUpdatingEventArgs value)
        {
            if (value is NpgsqlRowUpdatingEventArgs args)
                RowUpdating?.Invoke(this, args);
        }

        /// <summary>
        /// Delete command.
        /// </summary>
        public new NpgsqlCommand DeleteCommand
        {
            get => (NpgsqlCommand)base.DeleteCommand;
            set => base.DeleteCommand = value;
        }

        /// <summary>
        /// Select command.
        /// </summary>
        public new NpgsqlCommand SelectCommand
        {
            get => (NpgsqlCommand)base.SelectCommand;
            set => base.SelectCommand = value;
        }

        /// <summary>
        /// Update command.
        /// </summary>
        public new NpgsqlCommand UpdateCommand
        {
            get => (NpgsqlCommand)base.UpdateCommand;
            set => base.UpdateCommand = value;
        }

        /// <summary>
        /// Insert command.
        /// </summary>
        public new NpgsqlCommand InsertCommand
        {
            get => (NpgsqlCommand)base.InsertCommand;
            set => base.InsertCommand = value;
        }
    }

#pragma warning disable 1591

    public class NpgsqlRowUpdatingEventArgs : RowUpdatingEventArgs
    {
        public NpgsqlRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, System.Data.StatementType statementType,
                                          DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) {}
    }

    public class NpgsqlRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public NpgsqlRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, System.Data.StatementType statementType,
                                         DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) {}
    }

#pragma warning restore 1591
}
