/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer.Design
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Windows.Forms;
  using System.Drawing.Design;
  using System.Data;
  using System.Data.Common;

  internal class Column : IHaveConnection
  {
    private bool _allowNulls = false;
    private string _dataType = "";
    private string _defaultValue = "";
    private string _columnName = "";
    private string _origName = "";
    private string _collate;
    private DataGridViewRow _parent;
    private Unique _unique;
    private Table _table;

    internal Column(Table table, DataGridViewRow row)
    {
      _parent = row;
      _table = table as Table;
      _unique = new Unique(this);
    }

    internal Column(DataRow row, Table source)
    {
      _table = source;
      _unique = new Unique(this, row);

      if (row.IsNull("AUTOINCREMENT") == false && (bool)row["AUTOINCREMENT"] == true)
        _table.PrimaryKey.AutoIncrement = true;

      _dataType = (row.IsNull("DATA_TYPE") == false) ? row["DATA_TYPE"].ToString() : String.Empty;
      _columnName = row["COLUMN_NAME"].ToString();
      _origName = _columnName;
      _allowNulls = (bool)row["IS_NULLABLE"];
      _defaultValue = (row.IsNull("COLUMN_DEFAULT") == false) ? row["COLUMN_DEFAULT"].ToString() : String.Empty;
      _collate = (row.IsNull("COLLATION_NAME") == false) ? row["COLLATION_NAME"].ToString() : String.Empty;

      string edmtype = (row.IsNull("EDM_TYPE") == false) ? row["EDM_TYPE"].ToString() : String.Empty;
      if (edmtype == "nvarchar" || edmtype == "varchar" || edmtype == "blob" || edmtype == "nchar" || edmtype == "char")
      {
        int size = (row.IsNull("CHARACTER_MAXIMUM_LENGTH") == false) ? Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]) : int.MaxValue;
        if (size != int.MaxValue)
          _dataType = string.Format("{0}({1})", _dataType, size);
      }
      else if (edmtype == "decimal")
      {
        int size = (row.IsNull("NUMERIC_PRECISION") == false) ? Convert.ToInt32(row["NUMERIC_PRECISION"]) : 53;
        int scale = (row.IsNull("NUMERIC_SCALE") == false) ? Convert.ToInt32(row["NUMERIC_SCALE"]) : int.MaxValue;

        if (size != 53)
        {
          string scalestr = (scale == int.MaxValue) ? "" : String.Format(",{0}", scale);
          _dataType = string.Format("{0}({1}{2})", _dataType, size, scalestr);
        }
      }
    }

    #region IHaveConnection Members

    [Browsable(false)]
    public ViewTableBase DesignTable
    {
      get { return _table; }
    }

    public DbConnection GetConnection()
    {
      return ((IHaveConnection)_table).GetConnection();
    }

    #endregion

    [Browsable(false)]
    internal Table Table
    {
      get { return _table; }
    }

    internal void Committed()
    {
      _origName = ColumnName;
    }

    internal DataGridViewRow Parent
    {
      get { return _parent; }
      set
      {
        _parent = value;
        _parent.Cells[0].Value = ColumnName;
        _parent.Cells[1].Value = DataType;
        _parent.Cells[2].Value = AllowNulls;
      }
    }

    internal void RefreshGrid()
    {
      if (_parent == null) return;
      _parent.DataGridView.Refresh();
    }

    internal void CellValueChanged(int rowIndex, int cellIndex)
    {
      if (_parent == null) return;
      if (rowIndex != _parent.Index) return;

      object value;

      if (_parent.Cells[cellIndex].IsInEditMode == true)
      {
        if (_parent.DataGridView.EditingControl != null)
          value = ((IDataGridViewEditingControl)_parent.DataGridView.EditingControl).EditingControlFormattedValue;
        else
          value = _parent.Cells[cellIndex].EditedFormattedValue;
      }
      else
        value = _parent.Cells[cellIndex].EditedFormattedValue;

      switch (cellIndex)
      {
        case 0:
          ColumnName = value.ToString();
          break;
        case 1:
          DataType = value.ToString();
          break;
        case 2:
          try
          {
            AllowNulls = Convert.ToBoolean(value);
          }
          catch
          {
          }
          break;
      }
    }

    [DefaultValue("BINARY")]
    [Category("Constraints")]
    [Editor(typeof(CollationTypeEditor), typeof(UITypeEditor))]
    [Description("The collation sequence to use for the column.  This will affect comparison and equality tests against the column.")]
    public virtual string Collate
    {
      get { return _collate; }
      set
      {
        if (String.IsNullOrEmpty(value)) value = "BINARY";
        if (_collate != value)
        {
          _collate = value;

          if (_table.PrimaryKey.Columns.Count == 1 && String.Compare(ColumnName, _table.PrimaryKey.Columns[0].Column, StringComparison.OrdinalIgnoreCase) == 0)
            _table.PrimaryKey.Columns[0].Collate = value;

          _table._owner.MakeDirty();
        }
      }
    }

    [Category("Constraints")]
    [Description("The unique constraints of the column")]
    public virtual Unique Unique
    {
      get { return _unique; }
    }

    [Browsable(false)]
    public virtual string ColumnName
    {
      get { return _columnName; }
      set
      {
        value = value.Trim();
        if (value != _columnName)
        {
          _columnName = value;
          _table.MakeDirty();
        }
      }
    }

    [Browsable(false)]
    public virtual string OriginalName
    {
      get { return _origName; }
    }

    [DefaultValue(false)]
    [DisplayName("Allow Nulls")]
    [Category("Constraints")]
    [Description("Specifies whether or not the column will allow NULL values.")]
    public virtual bool AllowNulls
    {
      get { return _allowNulls; }
      set
      {
        if (value != _allowNulls)
        {
          _allowNulls = value;
          if (_parent == null) return;
          _parent.Cells[2].Value = _allowNulls;
          _table.MakeDirty();
        }
      }
    }

    [Browsable(false)]
    public virtual string DataType
    {
      get { return _dataType; }
      set
      {
        value = value.Trim();
        if (value != _dataType)
        {
          _dataType = value;
          _table.MakeDirty();
        }
      }
    }

    [DisplayName("Default Value")]
    [Category("Constraints")]
    [Description("The default value to populate in the column when an explicit value is not specified.")]
    public virtual string DefaultValue
    {
      get { return _defaultValue; }
      set
      {
        value = value.Trim();

        if (value != _defaultValue)
        {
          _defaultValue = value;
          _table.MakeDirty();
        }
      }
    }

    internal void WriteSql(StringBuilder builder)
    {
      builder.AppendFormat("[{0}]", ColumnName);
      if (String.IsNullOrEmpty(DataType) == false)
        builder.AppendFormat(" {0}", DataType);

      bool isprimary = false;
      if (_table.PrimaryKey.Columns.Count == 1 && String.Compare(_table.PrimaryKey.Columns[0].Column, ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
      {
        isprimary = true;
        builder.Append(" PRIMARY KEY");

        if (_table.PrimaryKey.Columns[0].SortMode != ColumnSortMode.Ascending)
          builder.Append(" DESC");

        if (_table.PrimaryKey.Conflict != ConflictEnum.Abort)
          builder.AppendFormat(" ON CONFLICT {0}", _table.PrimaryKey.Conflict.ToString().ToUpperInvariant());

        if (_table.PrimaryKey.AutoIncrement == true)
          builder.Append(" AUTOINCREMENT");
      }

      if (AllowNulls == false)
        builder.Append(" NOT NULL");

      if (String.IsNullOrEmpty(Collate) == false && String.Compare(Collate, "BINARY", StringComparison.OrdinalIgnoreCase) != 0)
        builder.AppendFormat(" COLLATE {0}", Collate.ToUpperInvariant());

      if (Unique.Enabled == true && isprimary == false)
      {
        builder.Append(" UNIQUE");
        if (Unique.Conflict != ConflictEnum.Abort)
          builder.AppendFormat(" ON CONFLICT {0}", Unique.Conflict.ToString().ToUpperInvariant());
      }

      if (String.IsNullOrEmpty(DefaultValue) == false)
        builder.AppendFormat(" DEFAULT {0}", DefaultValue);
    }
  }
}
