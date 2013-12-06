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
  using System.Collections;
  using System.Text;
  using System.ComponentModel;
  using System.Data;
  using System.Data.Common;
  using System.ComponentModel.Design;
  using System.Drawing.Design;
  using System.Windows.Forms;

  internal class ForeignKeyEditor : CollectionEditor
  {
    Table _table;
    CollectionEditor.CollectionForm _form;
    object[] _items;
    object[] _orig;
    int _count;

    internal ForeignKeyEditor(Table parent)
      : base(typeof(List<ForeignKey>))
    {
      _table = parent;
      _count = _table.ForeignKeys.Count;
    }

    protected override CollectionEditor.CollectionForm CreateCollectionForm()
    {
      _form = base.CreateCollectionForm();
      _form.Text = "Foreign Key Editor";
      foreach (Control c in _form.Controls[0].Controls)
      {
        PropertyGrid grid = c as PropertyGrid;
        if (grid != null)
        {
          grid.HelpVisible = true;
          break;
        }
      }
      _form.Width = (int)(_form.Width * 1.25);
      _form.Height = (int)(_form.Height * 1.25);

      return _form;
    }

    protected override object CreateInstance(Type itemType)
    {
      if (itemType == typeof(ForeignKey))
      {
        return new ForeignKey(null, _table, null);
      }
      throw new NotSupportedException();
    }

    protected override object[] GetItems(object editValue)
    {
      if (_items == null)
      {
        List<ForeignKey> items = editValue as List<ForeignKey>;
        _items = new object[items.Count];
        _orig = new object[items.Count];
        for (int n = 0; n < _items.Length; n++)
        {
          _items[n] = ((ICloneable)items[n]).Clone();
          _orig[n] = items[n];
        }
      }
      return _items;
    }

    protected override object SetItems(object editValue, object[] value)
    {
      bool dirty = false;
      if (_form.DialogResult == DialogResult.Cancel) value = _orig;

      if (editValue != null)
      {
        int length = this.GetItems(editValue).Length;
        int num2 = value.Length;
        if (!(editValue is IList))
        {
          return editValue;
        }
        IList list = (IList)editValue;
        list.Clear();
        for (int i = 0; i < value.Length; i++)
        {
          ForeignKey fkey = value[i] as ForeignKey;

          if (fkey != null && String.IsNullOrEmpty(fkey.From.Column) == false && String.IsNullOrEmpty(fkey.To.Catalog) == false &&
            String.IsNullOrEmpty(fkey.To.Table) == false && String.IsNullOrEmpty(fkey.To.Column) == false)
          {
            if (fkey.IsDirty) dirty = true;

            list.Add(value[i]);
          }
        }
        if ((dirty == true || list.Count != _count) && _form.DialogResult == DialogResult.OK)
        _table.MakeDirty();
      }
      return editValue;
    }
  }

  internal class ForeignKeyItem : IHaveConnectionScope
  {
    private string _catalog;
    private string _table;
    private string _column;
    private ForeignKey _fkey;

    internal ForeignKeyItem(ForeignKey fkey, string catalog, string table, string column)
    {
      _catalog = catalog;
      _table = table;
      _column = column;
      _fkey = fkey;
    }

    public override string ToString()
    {
      return String.Format("[{0}].[{1}].[{2}]", _catalog, _table, _column);
    }

    #region IHaveConnection Members

    [Browsable(false)]
    public ViewTableBase DesignTable
    {
      get { return _fkey.DesignTable; }
    }

    public DbConnection GetConnection()
    {
      return ((IHaveConnection)_fkey).GetConnection();
    }

    [Browsable(false)]
    public string TableScope
    {
      get { return _table; }
    }

    [Browsable(false)]
    public string CatalogScope
    {
      get { return _catalog; }
    }

    #endregion

    [Browsable(false)]
    [Editor(typeof(CatalogTypeEditor), typeof(UITypeEditor))]
    public virtual string Catalog
    {
      get { return _catalog; }
    }

    [Editor(typeof(TablesTypeEditor), typeof(UITypeEditor))]
    public virtual string Table
    {
      get { return _table; }
    }

    [Editor(typeof(ColumnsTypeEditor), typeof(UITypeEditor))]
    public virtual string Column
    {
      get { return _column; }
    }

    protected void SetCatalog(string value)
    {
      if (_catalog != value)
      {
        _catalog = value;
        _fkey.MakeDirty();
      }
    }

    protected void SetTable(string value)
    {
      if (_table != value)
      {
        _table = value;
        _fkey.MakeDirty();
      }
    }

    protected void SetColumn(string value)
    {
      if (_column != value)
      {
        _column = value;
        _fkey.MakeDirty();
      }
    }
  }

  [TypeConverter(typeof(ExpandableObjectConverter))]
  internal class ForeignKeyFromItem : ForeignKeyItem
  {
    internal ForeignKeyFromItem(ForeignKey fkey, string column)
      : base(fkey, fkey._table.Catalog, fkey._table.Name, column)
    {
    }

    [Editor(typeof(ColumnsTypeEditor), typeof(UITypeEditor))]
    [Description("The column of the current table that refers to the foreign key relationship")]
    public new string Column
    {
      get { return base.Column; }
      set { SetColumn(value); }
    }

    [Browsable(false)]
    public override string Catalog
    {
      get
      {
        return base.Catalog;
      }
    }

    [Browsable(false)]
    public override string Table
    {
      get
      {
        return base.DesignTable.Name;
      }
    }
  }

  [TypeConverter(typeof(ExpandableObjectConverter))]
  internal class ForeignKeyToItem : ForeignKeyItem
  {
    internal ForeignKeyToItem(ForeignKey fkey, string catalog, string table, string column)
      : base(fkey, catalog, table, column)
    {
    }

    [Browsable(false)]
    [Editor(typeof(CatalogTypeEditor), typeof(UITypeEditor))]
    [Description("The database catalog (main, temp, or the name of an attached database) to which the foreign key refers.")]
    public new string Catalog
    {
      get
      {
        return base.Catalog;
      }
      set
      {
        SetCatalog(value);
      }
    }

    [DisplayName("Base Table")]
    [Editor(typeof(TablesTypeEditor), typeof(UITypeEditor))]
    [Description("The table to which the foreign key refers.")]
    public new string Table
    {
      get { return base.Table; }
      set { SetTable(value); }
    }

    [Editor(typeof(ColumnsTypeEditor), typeof(UITypeEditor))]
    [Description("The column to which the foreign key refers.")]
    public new string Column
    {
      get { return base.Column; }
      set { SetColumn(value); }
    }
  }

  [DefaultProperty("From")]
  internal class ForeignKey : IHaveConnection, ICloneable
  {
    internal Table _table;
    internal ForeignKeyFromItem _from;
    internal ForeignKeyToItem _to;
    internal string _name;
    private bool _dirty = false;

    private ForeignKey(ForeignKey source)
    {
      _table = source._table;
      _from = new ForeignKeyFromItem(this, source._from.Column);
      _to = new ForeignKeyToItem(this, source._to.Catalog, source._to.Table, source._to.Column);
      _name = source._name;
      _dirty = source._dirty;
    }

    internal void MakeDirty()
    {
      _dirty = true;
    }

    [Browsable(false)]
    internal bool IsDirty
    {
      get { return _dirty; }
    }

    internal void ClearDirty()
    {
      _dirty = false;
    }

    internal ForeignKey(DbConnection cnn, Table table, DataRow row)
    {
      _table = table;
      if (row != null)
      {
        _from = new ForeignKeyFromItem(this, row["FKEY_FROM_COLUMN"].ToString());
        _to = new ForeignKeyToItem(this, row["FKEY_TO_CATALOG"].ToString(), row["FKEY_TO_TABLE"].ToString(), row["FKEY_TO_COLUMN"].ToString());
        _name = row["CONSTRAINT_NAME"].ToString();
      }
      else
      {
        _name = null;
        _from = new ForeignKeyFromItem(this, "");
        _to = new ForeignKeyToItem(this, _table.Catalog, "", "");
      }
    }

    //internal void WriteSql(StringBuilder builder)
    //{
    //  if (String.IsNullOrEmpty(_from.Column) == false && String.IsNullOrEmpty(_to.Catalog) == false &&
    //    String.IsNullOrEmpty(_to.Table) == false && String.IsNullOrEmpty(_to.Column) == false)
    //  {
    //    builder.AppendFormat("CONSTRAINT [{0}] FOREIGN KEY ([{1}]) REFERENCES [{3}] ([{4}])", Name, _from.Column, _to.Catalog, _to.Table, _to.Column);
    //  }
    //}

    [ParenthesizePropertyName(true)]
    [Category("Identity")]
    [Description("The name of the foreign key.")]
    public string Name
    {
      get
      {
        if (String.IsNullOrEmpty(_name) == false) return _name;

        return String.Format("FK_{0}_{1}_{2}_{3}", _from.Table, _from.Column, _to.Table, _to.Column);
      }
      set
      {
        if (_name != value)
        {
          _name = value;
          MakeDirty();
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

    [DisplayName("From Key")]
    [Category("From")]
    [Description("The source column in the current table that refers to the foreign key.")]
    public ForeignKeyFromItem From
    {
      get { return _from; }
    }

    [DisplayName("To Key")]
    [Category("To")]
    [Description("The table and column to which the specified from column is related.")]
    public ForeignKeyToItem To
    {
      get { return _to; }
    }

    #region ICloneable Members

    public object Clone()
    {
      return new ForeignKey(this);
    }

    #endregion
  }
}
