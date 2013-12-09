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

  internal class IndexEditor : CollectionEditor
  {
    Table _table;
    CollectionEditor.CollectionForm _form;
    object[] _items = null;
    object[] _orig;
    int _count;

    internal IndexEditor(Table parent)
      : base(typeof(List<Index>))
    {
      _table = parent;
    }

    protected override object[] GetItems(object editValue)
    {
      if (_items == null)
      {
        List<Index> value = editValue as List<Index>;

        int extra = (_table.PrimaryKey.Columns.Count > 0) ? 1 : 0;

        _items = new object[value.Count + extra];
        _orig = new object[_items.Length];
        for (int n = extra; n < _items.Length; n++)
        {
          _items[n] = ((ICloneable)value[n - extra]).Clone();
          _orig[n] = value[n - extra];
        }

        if (extra > 0)
        {
          _items[0] = ((ICloneable)_table.PrimaryKey).Clone();
          _orig[0] = _table.PrimaryKey;
        }

        _count = _items.Length;
      }
      return _items;
    }

    protected override CollectionEditor.CollectionForm CreateCollectionForm()
    {
      _form = base.CreateCollectionForm();
      _form.Text = "Index Editor";

      /* Doing this because I can't figure out how to get the Columns collection editor to notify this editor when a column of an index is updated.
         This forces the collection editor form to be "dirty" which calls SetItems() when you hit OK or cancel.  Otherwise, if you
         change a column around and hit OK, then hit OK on this editor, it won't be dirty and won't update. */
      try
      {
        _form.GetType().InvokeMember("dirty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetField, null, _form, new object[] { true });
      }
      catch
      {
      }

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
      if (itemType == typeof(Index))
      {
        return new Index(null, _table, null);
      }
      throw new NotSupportedException();
    }

    protected override bool CanRemoveInstance(object value)
    {
      return !(value is PrimaryKey);
    }

    protected override object SetItems(object editValue, object[] value)
    {
      bool dirty = false;
      int count = 0;

      if (_form.DialogResult == DialogResult.Cancel)
        value = _orig;

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
          Index idx = value[i] as Index;

          if (idx is PrimaryKey)
          {
            _table.PrimaryKey = (PrimaryKey)idx;
            if (idx.IsDirty) dirty = true;
            count++;
          }
          else
          {
            if (idx != null && idx.Columns.Count > 0)
            {
              idx.Name = idx.Name;
              list.Add(idx);
              if (idx.IsDirty) dirty = true;
              count++;
            }
          }
        }
      }

      if ((dirty == true || count != _count) && _form.DialogResult == DialogResult.OK)
        _table._owner.MakeDirty();

      return editValue;
    }
  }

  internal class IndexColumnEditor : CollectionEditor
  {
    Index _index;
    object[] _items;
    object[] _orig;
    int _count;
    CollectionEditor.CollectionForm _form;

    public IndexColumnEditor() : base(typeof(List<IndexColumn>))
    {
    }

    protected override CollectionEditor.CollectionForm CreateCollectionForm()
    {
      _form = base.CreateCollectionForm();
      _form.Text = "Index Columns Editor";
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

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      _index = context.Instance as Index;
      _items = null;
      _count = 0;
      return base.EditValue(context, provider, value);
    }

    protected override object CreateInstance(Type itemType)
    {
      if (itemType == typeof(IndexColumn))
      {
        return new IndexColumn(_index, null);
      }
      throw new NotSupportedException();
    }

    protected override object[] GetItems(object editValue)
    {
      if (_items == null)
      {
        List<IndexColumn> value = editValue as List<IndexColumn>;
        _items = new object[value.Count];
        _orig = new object[value.Count];
        for (int n = 0; n < _items.Length; n++)
        {
          _items[n] = ((ICloneable)value[n]).Clone();
          _orig[n] = value[n];
        }

        _count = _items.Length;
      }
      return _items;
    }

    protected override object SetItems(object editValue, object[] value)
    {
      if (_form.DialogResult == DialogResult.Cancel)
        value = _orig;

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
          IndexColumn idx = value[i] as IndexColumn;

          if (idx != null && String.IsNullOrEmpty(idx.Column) == false)
          {
            list.Add(value[i]);
          }
        }
      }

      if ((_index.IsDirty || _index.Columns.Count != _count) && _form.DialogResult == DialogResult.OK)
      {
        if (_index.Columns.Count > 0 && String.IsNullOrEmpty(_index._name) == true)
          _index.Name = _index.Name;
      }
      return editValue;
    }
  }

  internal class IndexTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        StringBuilder builder = new StringBuilder();
        string separator = "";
        foreach (IndexColumn c in (List<IndexColumn>)value)
        {
          builder.AppendFormat("{0}[{1}]", separator, c.Column);
          if (c.SortMode != ColumnSortMode.Ascending)
            builder.Append(" DESC");
          if (c.Collate != "BINARY")
            builder.AppendFormat(" COLLATE {0}", c.Collate.ToUpperInvariant());

          separator = ", ";
        }
        return builder.ToString();
      }
      else
        return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  internal enum ColumnSortMode
  {
    Ascending = 0,
    Descending = 1
  }

  [DefaultProperty("Column")]
  internal class IndexColumn : IHaveConnectionScope, ICloneable
  {
    internal Index _parent;
    private string _column;
    private ColumnSortMode _mode = ColumnSortMode.Ascending;
    private string _collate = "BINARY";

    [Editor(typeof(ColumnsTypeEditor), typeof(UITypeEditor))]
    [DisplayName("Base Column")]
    [Category("Source")]
    [Description("The column name to be included in the index.")]
    [NotifyParentProperty(true)]
    [RefreshProperties(RefreshProperties.All)]
    public string Column
    {
      get { return _column; }
      set
      {
        if (_column != value)
        {
          _column = value;
          _parent.MakeDirty();
        }
      }
    }

    [DefaultValue(ColumnSortMode.Ascending)]
    [Category("Constraints")]
    [Description("Specifies what order to sort the column in.  Descending indexes are not supported when using the SQLite legacy file format.")]
    [NotifyParentProperty(true)]
    [RefreshProperties(RefreshProperties.All)]
    public ColumnSortMode SortMode
    {
      get { return _mode; }
      set
      {
        if (value != _mode)
        {
          _mode = value;
          _parent.MakeDirty();
        }
      }
    }

    [DefaultValue("BINARY")]
    [Category("Constraints")]
    [Editor(typeof(CollationTypeEditor), typeof(UITypeEditor))]
    [Description("The collation sequence to use to generate the index for the specified column.")]
    [NotifyParentProperty(true)]
    [RefreshProperties(RefreshProperties.All)]
    public string Collate
    {
      get { return _collate; }
      set
      {
        if (String.IsNullOrEmpty(value)) value = "BINARY";

        if (value != _collate)
        {
          _collate = value;
          if (_parent is PrimaryKey)
          {
            PrimaryKey pk = _parent as PrimaryKey;
            if (pk.Columns.Count == 1)
            {
              foreach (Column c in pk.Table.Columns)
              {
                if (string.Compare(c.ColumnName, Column, StringComparison.OrdinalIgnoreCase) == 0)
                {
                  c.Collate = value;
                  break;
                }
              }
            }
          }
          _parent.MakeDirty();
        }
      }
    }

    public override string ToString()
    {
      if (String.IsNullOrEmpty(_column) == true) return "(none)";
      return _column;
    }

    private IndexColumn(IndexColumn source)
    {
      _parent = source._parent;
      _column = source._column;
      _mode = source._mode;
      _collate = source._collate;
    }

    internal IndexColumn(Index parent, DataRow row)
    {
      _parent = parent;
      if (row != null)
      {
        _column = row["COLUMN_NAME"].ToString();
        if (row.IsNull("SORT_MODE") == false && (string)row["SORT_MODE"] != "ASC")
          _mode = ColumnSortMode.Descending;

        if (row.IsNull("COLLATION_NAME") == false)
          _collate = row["COLLATION_NAME"].ToString().ToUpperInvariant();
      }
    }

    public object Clone()
    {
      return new IndexColumn(this);
    }

    #region IHaveConnectionScope Members

    [Browsable(false)]
    public string CatalogScope
    {
      get { return _parent.Table.Catalog; }
    }

    [Browsable(false)]
    public string TableScope
    {
      get { return _parent.Table.Name; }
    }

    #endregion

    #region IHaveConnection Members

    [Browsable(false)]
    public ViewTableBase DesignTable
    {
      get { return _parent.DesignTable; }
    }

    public DbConnection GetConnection()
    {
      return ((IHaveConnection)_parent).GetConnection();
    }

    #endregion
  }

  public enum IndexTypeEnum
  {
    Index = 0,
    PrimaryKey = 1,
  }

  [DefaultProperty("Columns")]
  internal class Index : IHaveConnection, ICloneable
  {
    private Table _table;
    internal string _name = null;
    private bool _unique;
    private List<IndexColumn> _columns = new List<IndexColumn>();
    private string _definition;
    private bool _calcname;
    internal ConflictEnum _conflict = ConflictEnum.Abort;
    bool _dirty = false;

    protected Index(Index source)
    {
      _table = source._table;
      _name = source._name;
      _unique = source._unique;
      _definition = source._definition;
      _conflict = source._conflict;
      _dirty = source._dirty;

      foreach (IndexColumn c in source._columns)
      {
        IndexColumn copy = ((ICloneable)c).Clone() as IndexColumn;
        copy._parent = this;
        _columns.Add(copy);
      }
    }

    internal Index(DbConnection cnn, Table table, DataRow index)
    {
      _table = table;
      if (index != null)
      {
        _name = index["INDEX_NAME"].ToString();
        _unique = (bool)index["UNIQUE"];
        _definition = index["INDEX_DEFINITION"].ToString();

        using (DataTable tbl = cnn.GetSchema("IndexColumns", new string[] { table.Catalog, null, table.Name, Name }))
        {
          foreach (DataRow row in tbl.Rows)
          {
            _columns.Add(new IndexColumn(this, row));
          }
        }
      }
    }

    [DisplayName("Index Type")]
    [Category("Storage")]
    [Description("Specifies whether this is an index or a primary key.")]
    public virtual IndexTypeEnum IndexType
    {
      get { return IndexTypeEnum.Index; }
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

    internal virtual void WriteSql(StringBuilder builder)
    {
      string separator = "";
      builder.AppendFormat("CREATE {0}INDEX [{1}].[{2}] ON [{3}] (", (_unique == true) ? "UNIQUE " : "", _table.Catalog, Name, _table.Name);
      foreach (IndexColumn c in Columns)
      {
        builder.AppendFormat("{0}[{1}]", separator, c.Column);

        if (c.SortMode != ColumnSortMode.Ascending)
          builder.Append(" DESC");

        if (String.IsNullOrEmpty(c.Collate) && String.Compare(c.Collate,"BINARY", StringComparison.OrdinalIgnoreCase) != 0)
          builder.AppendFormat(" COLLATE {0}", c.Collate.ToUpperInvariant());

        separator = ", ";
      }
      builder.AppendFormat(");");
    }

    [Browsable(false)]
    internal Table Table
    {
      get { return _table; }
    }

    [Browsable(false)]
    public string OriginalSql
    {
      get { return _definition; }
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

    [DefaultValue(false)]
    [Description("When set to true, the combination of column(s) of the index must be a unique value.")]
    public virtual bool Unique
    {
      get { return _unique; }
      set
      {
        if (value != _unique)
        {
          _unique = value;
          MakeDirty();
        }
      }
    }

    [Browsable(false)]
    protected virtual string NamePrefix
    {
      get { return "IX"; }
    }

    [Browsable(false)]
    protected virtual string NewName
    {
      get { return "NewIndex"; }
    }

    [ParenthesizePropertyName(true)]
    [RefreshProperties(RefreshProperties.All)]
    [Category("Identity")]
    [Description("The name of the index.")]
    public virtual string Name
    {
      get
      {
        if (String.IsNullOrEmpty(_name))
        {
          if (_calcname == true) return GetHashCode().ToString();

          string name = String.Format("{0}_{1}", NamePrefix, NewName);
          if (Columns.Count > 0 && NewName != Table.Name)
          {
            name = String.Format("{0}_", NamePrefix);
            for (int n = 0; n < Columns.Count; n++)
            {
              if (n > 0) name += "_";
              name += Columns[n].Column;
            }
          }
          int count = 0;
          string proposed = name;

          _calcname = true;
          for (int n = 0; n < _table.Indexes.Count; n++)
          {
            Index idx = _table.Indexes[n];
            proposed = string.Format("{0}{1}", name, (count > 0) ? count.ToString() : "");
            if (idx.Name == proposed)
            {
              count++;
              n = -1;
            }
          }
          _calcname = false;
          return proposed;
        }
        return _name;
      }
      set
      {
        if (value != _name)
        {
          _name = value;
          MakeDirty();
        }
      }
    }

    [TypeConverter(typeof(IndexTypeConverter))]
    [Editor(typeof(IndexColumnEditor), typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.All)]
    [Category("Source")]
    [Description("The column(s) to be indexed.")]
    [NotifyParentProperty(true)]
    public List<IndexColumn> Columns
    {
      get { return _columns; }
    }

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return new Index(this);
    }

    #endregion
  }

  public class ColumnsMultiSelectEditor : UITypeEditor
  {
    private System.Windows.Forms.Design.IWindowsFormsEditorService _edSvc;
    private CheckedListBox _list;
    private bool _cancel;

    public ColumnsMultiSelectEditor()
    {
      // build selector list
      _list = new CheckedListBox();
      _list.BorderStyle = BorderStyle.FixedSingle;
      _list.CheckOnClick = true;
      _list.ThreeDCheckBoxes = false;
      _list.KeyPress += new KeyPressEventHandler(_list_KeyPress);
    }

    override public UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext ctx)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    override public object EditValue(ITypeDescriptorContext ctx, IServiceProvider provider, object value)
    {
      Index idx = ctx.Instance as Index;
      Trigger trig = ctx.Instance as Trigger;
      ViewTableBase parent = null;

      if (idx != null) parent = idx.Table;
      else if (trig != null) parent = trig.ViewTableBase;

      // initialize editor service
      _edSvc = (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));
      if (_edSvc == null)
        return value;

      if (value == null) value = String.Empty;
      if (String.IsNullOrEmpty(value.ToString()) == true) value = String.Empty;

      string[] values = value.ToString().Split(',');

      // populate the list
      _list.Items.Clear();

      if (parent is Table)
      {
        foreach (Column c in ((Table)parent).Columns)
        {
          CheckState check = CheckState.Unchecked;
          for (int n = 0; n < values.Length; n++)
          {
            if (values[n].Trim() == String.Format("[{0}]", c.ColumnName))
            {
              check = CheckState.Checked;
              break;
            }
          }
          _list.Items.Add(c.ColumnName, check);
        }
      }
      else
      {
        try
        {
          using (DbCommand cmd = trig.GetConnection().CreateCommand())
          {
            cmd.CommandText = ((View)parent).SqlText;
            using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
            using (DataTable tbl = reader.GetSchemaTable())
            {
              foreach (DataRow row in tbl.Rows)
              {
                CheckState check = CheckState.Unchecked;
                for (int n = 0; n < values.Length; n++)
                {
                  if (values[n].Trim() == String.Format("[{0}]", row[SchemaTableColumn.ColumnName]))
                  {
                    check = CheckState.Checked;
                    break;
                  }
                }
                _list.Items.Add(row[SchemaTableColumn.ColumnName].ToString(), check);
              }
            }
          }
        }
        catch
        {
        }
      }
      _list.Height = Math.Min(300, (_list.Items.Count + 1) * _list.Font.Height);

      // show the list
      _cancel = false;
      _edSvc.DropDownControl(_list);

      // build return value from checked items on the list
      if (!_cancel)
      {
        // build a comma-delimited string with the checked items
        StringBuilder sb = new StringBuilder();
        foreach (object item in _list.CheckedItems)
        {
          if (sb.Length > 0) sb.Append(", ");
          sb.AppendFormat("[{0}]", item.ToString());
        }

        return sb.ToString();
      }

      // done
      return value;
    }

    // ** event handlers

    // close editor if the user presses enter or escape
    private void _list_KeyPress(object sender, KeyPressEventArgs e)
    {
      switch (e.KeyChar)
      {
        case (char)27:
          _cancel = true;
          _edSvc.CloseDropDown();
          break;
        case (char)13:
          _edSvc.CloseDropDown();
          break;
      }
    }
  }

  internal class ColumnsTypeEditor : ObjectSelectorEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
    {
      base.FillTreeWithData(selector, context, provider);
      IHaveConnectionScope source = context.Instance as IHaveConnectionScope;
      ViewTableBase design;

      if (source == null) return;

      design = source.DesignTable;

      if (design.Name != source.TableScope)
      {
        using (DataTable table = source.GetConnection().GetSchema("Columns", new string[] { source.CatalogScope, null, source.TableScope }))
        {
          foreach (DataRow row in table.Rows)
          {
            selector.AddNode(row[3].ToString(), row[3], null);
          }
        }
      }
      else
      {
        Table tbl = design as Table;
        if (tbl != null)
        {
          foreach (Column c in tbl.Columns)
          {
            selector.AddNode(c.ColumnName, c.ColumnName, null);
          }
        }
      }
    }
  }

  internal class TablesTypeEditor : ObjectSelectorEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
    {
      base.FillTreeWithData(selector, context, provider);
      IHaveConnectionScope source = context.Instance as IHaveConnectionScope;
      Table design;

      if (source == null) return;

      design = source.DesignTable as Table;

      using (DataTable table = source.GetConnection().GetSchema("Tables", new string[] { source.CatalogScope }))
      {
        foreach (DataRow row in table.Rows)
        {
          bool add = true;
          if (design != null && (row[2].ToString() == design.OldName || row[2].ToString() == design.Name))
            add = false;

          if (add)
            selector.AddNode(row[2].ToString(), row[2], null);
        }
      }
      if (design != null)
        selector.AddNode(design.Name, design.Name, null);
    }
  }
}
