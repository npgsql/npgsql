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
  using System.Data;
  using System.Data.Common;

  internal class PrimaryKey : Index, ICloneable
  {
    private bool _autoincrement;

    internal PrimaryKey(DbConnection cnn, Table table, DataRow row)
      : base(cnn, table, row)
    {
      if (String.IsNullOrEmpty(_name) == false && _name.StartsWith("sqlite_", StringComparison.OrdinalIgnoreCase))
        _name = null;
    }

    protected PrimaryKey(PrimaryKey source)
      : base(source)
    {
      _autoincrement = source._autoincrement;
    }

    public override IndexTypeEnum IndexType
    {
      get
      {
        return IndexTypeEnum.PrimaryKey;
      }
    }

    protected override string NamePrefix
    {
      get
      {
        return "PK";
      }
    }

    protected override string NewName
    {
      get
      {
        return Table.Name;
      }
    }

    [Browsable(false)]
    public override bool Unique
    {
      get
      {
        return true;
      }
      set
      {
        base.Unique = true;
      }
    }

    [DefaultValue(ConflictEnum.Abort)]
    [DisplayName("On Conflict")]
    [Category("Constraints")]
    [Description("Specifies what action to take when the primary key constraint is violated.")]
    public ConflictEnum Conflict
    {
      get { return _conflict; }
      set
      {
        if (value != _conflict)
        {
          _conflict = value;
          MakeDirty();
        }
      }
    }

    [DefaultValue(false)]
    [Category("Constraints")]
    [Description("Can only be enabled for a single column primary key of type INTEGER.  When set, the primary key is guaranteed to increment in sequence, and no previously deleted or uncommitted values will ever be used.")]
    public bool AutoIncrement
    {
      get
      {
        if (Columns.Count > 1) return false;
        if (Columns.Count == 1 && Columns[0].SortMode != ColumnSortMode.Ascending) return false;
        return _autoincrement;
      }
      set
      {
        if (value != _autoincrement)
        {
          _autoincrement = value;
          MakeDirty();
        }
      }
    }

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return new PrimaryKey(this);
    }

    #endregion
  }
}
