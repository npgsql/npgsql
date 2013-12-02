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

  [TypeConverter(typeof(ExpandableObjectConverter))]
  [DefaultProperty("Enabled")]
  internal class Unique : IHaveConnection
  {
    private bool _isUnique;
    private ConflictEnum _conflict = ConflictEnum.Abort;
    private Column _column;

    internal Unique(Column col)
      : this(col, null)
    {
    }

    internal Unique(Column col, DataRow row)
    {
      _column = col;
      if (row != null)
      {
        _isUnique = (row.IsNull("UNIQUE") == false) ? (bool)row["UNIQUE"] : false;
      }
    }

    #region IHaveConnection Members

    [Browsable(false)]
    public ViewTableBase DesignTable
    {
      get { return _column.DesignTable; }
    }

    public DbConnection GetConnection()
    {
      return ((IHaveConnection)_column).GetConnection();
    }

    #endregion

    [DefaultValue(false)]
    [DisplayName("Enabled")]
    [RefreshProperties(RefreshProperties.All)]
    [Description("When enabled, all values entered into this column must be unique.")]
    public bool Enabled
    {
      get { return _isUnique; }
      set
      {
        if (value != _isUnique)
        {
          _isUnique = value;
          _column.Table._owner.MakeDirty();
        }
      }
    }

    [DefaultValue(ConflictEnum.Abort)]
    [DisplayName("On Conflict")]
    [RefreshProperties(RefreshProperties.All)]
    [Description("Specifies what action to take when the unique constraint on the column is violated.")]
    public ConflictEnum Conflict
    {
      get { return _conflict; }
      set
      {
        if (_conflict != value)
        {
          _conflict = value;

          if (_conflict != ConflictEnum.Abort && _isUnique == false)
            _isUnique = true;

          _column.Table._owner.MakeDirty();
        }
      }
    }

    public override string ToString()
    {
      if (_isUnique == false)
        return Convert.ToString(false);
      else
        return String.Format("{0} ({1})", Convert.ToString(true), Convert.ToString(Conflict));
        //return Convert.ToString(true);
    }
  }

  public enum ConflictEnum
  {
    Abort = 2,
    Rollback = 0,
    Fail = 3,
    Ignore = 4,
    Replace = 5,
  }
}
