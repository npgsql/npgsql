/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer.Design
{
  using System;
  using System.Data.Common;
  using System.ComponentModel.Design;
  using System.ComponentModel;
  using System.Drawing.Design;
  using System.Collections.Generic;
  using System.Data;
  using System.Text;
  using Npgsql.Designer.Editors;

  internal abstract class ViewTableBase: IHaveConnection
  {
    public abstract string OldName { get; }
    public abstract string Name { get; set; }
    public abstract string Catalog { get; }
    public abstract object Triggers { get; }
    public abstract void MakeDirty();
    public abstract DbConnection GetConnection();
    public abstract ViewTableBase DesignTable { get; }
  }

  internal class Table : ViewTableBase, ICustomTypeDescriptor
  {
    private string _name;
    private string _oldname;
    private string _catalog;
    private List<Column> _columns = new List<Column>();
    private bool _exists = false;
    private string _origSql = String.Empty;
    private List<Index> _indexes = new List<Index>();
    private List<Index> _oldindexes = new List<Index>();
    private List<ForeignKey> _fkeys = new List<ForeignKey>();
    private List<ForeignKey> _oldfkeys = new List<ForeignKey>();
    private List<string> _check = new List<string>();
    private List<Trigger> _triggers = new List<Trigger>();
    private List<Trigger> _oldtriggers = new List<Trigger>();
    private PrimaryKey _key;
    internal TableDesignerDoc _owner;
    internal DbConnection _connection;

    internal Table(string tableName, DbConnection connection, TableDesignerDoc owner)
    {
      _owner = owner;
      _oldname = tableName;
      _connection = connection;
      _name = tableName;
      _owner.Name = _name;
      _catalog = _connection.Database; // main

      ReloadDefinition();

      if (_key == null) _key = new PrimaryKey(_connection, this, null);

      if (_exists)
      {
        using (DataTable tbl = connection.GetSchema("ForeignKeys", new string[] { Catalog, null, Name }))
        {
          foreach (DataRow row in tbl.Rows)
          {
            _fkeys.Add(new ForeignKey(connection, this, row));
            _oldfkeys.Add(new ForeignKey(connection, this, row));
          }
        }
      }

      using (DataTable tbl = connection.GetSchema("Columns", new string[] { Catalog, null, Name }))
      {
        foreach (DataRow row in tbl.Rows)
        {
          _columns.Add(new Column(row, this));
        }
      }
    }

    public override void MakeDirty()
    {
      _owner.MakeDirty();
    }

    private void ReloadDefinition()
    {
      using (DataTable tbl = _connection.GetSchema("Tables", new string[] { Catalog, null, Name }))
      {
        if (tbl.Rows.Count > 0)
        {
          _exists = true;
          _origSql = tbl.Rows[0]["TABLE_DEFINITION"].ToString().Trim().TrimEnd(';');
          _oldname = Name;
        }
        else
        {
          _exists = false;
          return;
        }
      }

      _indexes.Clear();
      _oldindexes.Clear();

      using (DataTable tbl = _connection.GetSchema("Indexes", new string[] { Catalog, null, Name }))
      {
        foreach (DataRow row in tbl.Rows)
        {
          if ((bool)row["PRIMARY_KEY"] == false)
          {
            if (row["INDEX_NAME"].ToString().StartsWith("sqlite_", StringComparison.OrdinalIgnoreCase) == false)
            {
              _indexes.Add(new Index(_connection, this, row));
              _oldindexes.Add(new Index(_connection, this, row));
            }
          }
          else if (_key == null)
          {
            _key = new PrimaryKey(_connection, this, row);
          }
        }
      }

      _check.Clear();
      StringBuilder builder = new StringBuilder();
      SimpleTokenizer.StringParts[] arr = SimpleTokenizer.BreakString(_origSql);
      for (int n = 0; n < arr.Length - 3; n++)
      {
        if (arr[n].keyword == "CONSTRAINT")
        {
          builder.Length = 0;
          int x;
          for (x = 1; x < 3; x++)
          {
            if (arr[n + x].keyword == "CHECK")
              break;
          }
          if (x == 3)
          {
            n += 2;
            continue;
          }
          x += n + 1;
          int depth = arr[n].depth;
          int basedepth = arr[x].depth;
          for (; x < arr.Length; x++)
          {
            if (arr[x].depth < basedepth)
              break;

            if (builder.Length > 0)
              builder.Append(" ");

            while (depth < arr[x].depth)
            {
              builder.Append("(");
              depth++;
            }
            while (depth > arr[x].depth)
            {
              builder.Append(")");
              depth--;
            }

            if (String.IsNullOrEmpty(arr[x].quote) == false)
              builder.Append(arr[x].quote[0]);
            builder.Append(arr[x].value);
            if (String.IsNullOrEmpty(arr[x].quote) == false)
              builder.Append(arr[x].quote[1]);

            if (arr[x].sep == true) break;
          }
          while (depth > arr[n].depth)
          {
            builder.Append(")");
            depth--;
          }
          n = x;
          _check.Add(builder.ToString());
        }
      }

      builder.Length = 0;
      builder.AppendLine("-- Original table schema");
      builder.Append(_origSql);

      builder.AppendLine(";");
      foreach (Index idx in _oldindexes)
      {
        builder.AppendFormat("{0};\r\n", idx.OriginalSql);
      }

      _triggers.Clear();
      _oldtriggers.Clear();

      using (DataTable tbl = _connection.GetSchema("Triggers", new string[] { Catalog, null, Name }))
      {
        foreach (DataRow row in tbl.Rows)
        {
          Trigger t = new Trigger(this, row);
          _triggers.Add(t);
          _oldtriggers.Add(((ICloneable)t).Clone() as Trigger);

          builder.AppendFormat("{0};\r\n", t.OriginalSql);
        }
      }

      _origSql = builder.ToString();
    }

    internal void Committed()
    {
      _exists = true;
      ReloadDefinition();

      foreach (Column c in Columns)
        c.Committed();

      foreach (ForeignKey key in ForeignKeys)
        key.ClearDirty();

      foreach (Index idx in Indexes)
        idx.ClearDirty();

      if (PrimaryKey != null)
        PrimaryKey.ClearDirty();
    }

    [Browsable(false)]
    public List<Index> Indexes
    {
      get { return _indexes; }
    }

    [Browsable(false)]
    public PrimaryKey PrimaryKey
    {
      get { return _key; }
      set
      {
        _key = value;
        _owner.Invalidate();
      }
    }

    [Browsable(false)]
    public List<ForeignKey> ForeignKeys
    {
      get { return _fkeys; }
    }

    [Browsable(false)]
    public List<string> Check
    {
      get { return _check; }
    }

    [Browsable(false)]
    public override object Triggers
    {
      get { return _triggers; }
    }

    [Browsable(false)]
    public string OriginalSql
    {
      get { return _origSql; }
    }

    [Category("Storage")]
    [RefreshProperties(RefreshProperties.All)]
    [ParenthesizePropertyName(true)]
    [NotifyParentProperty(true)]
    public override string Name
    {
      get { return _name; }
      set
      {
        if (_name != value)
        {
          _name = value;
          _owner.Name = value;
          _owner.MakeDirty();
        }
      }
    }

    [Browsable(false)]
    public override string OldName
    {
      get { return _oldname; }
    }

    public override string ToString()
    {
      return String.Format("[{0}].[{1}]", Catalog, Name);
    }

    [Category("Storage")]
    [Editor(typeof(CatalogTypeEditor), typeof(UITypeEditor))]
    [DefaultValue("main")]
    [RefreshProperties(RefreshProperties.All)]
    public override string Catalog
    {
      get { return _catalog; }
    }

    [Category("Storage")]
    public string Database
    {
      get { return _connection.DataSource; }
    }

    [Browsable(false)]
    public List<Column> Columns
    {
      get { return _columns; }
    }

    public string GetSql()
    {
      StringBuilder builder = new StringBuilder();
      string altName = null;

      if (_exists)
      {
        Guid g = Guid.NewGuid();
        altName = String.Format("{0}_{1}", Name, g.ToString("N"));

        if (_oldindexes.Count > 0)
        {
          builder.Append("-- Drop previous indexes on the table\r\n");
          foreach (Index idx in _oldindexes)
          {
            builder.AppendFormat("DROP INDEX [{0}].[{1}];\r\n", _catalog, idx.Name);
          }
          builder.AppendLine();
        }

        if (_oldtriggers.Count > 0)
        {
          builder.Append("-- Drop previous triggers on the table\r\n");
          foreach (Trigger trig in _oldtriggers)
          {
            builder.AppendFormat("DROP TRIGGER [{0}].[{1}];\r\n", _catalog, trig.Name);
          }
          builder.AppendLine();
        }

        builder.Append("-- Rename the old table\r\n");
        builder.AppendFormat("ALTER TABLE [{0}].[{1}] RENAME TO [{2}];\r\n\r\n", _catalog, _oldname, altName);
      }

      builder.Append("-- Create the new table\r\n");
      builder.AppendFormat("CREATE TABLE [{0}].[{1}] (\r\n", _catalog, Name);
      string separator = "    ";

      foreach (Column c in Columns)
      {
        if (String.IsNullOrEmpty(c.ColumnName) == false)
        {
          builder.Append(separator);
          c.WriteSql(builder);
          separator = ",\r\n    ";
        }
      }

      if (_key.Columns.Count > 1)
      {
        string innersep = "";
        builder.AppendFormat("{0}CONSTRAINT [PK_{1}] PRIMARY KEY (", separator, Name);
        foreach (IndexColumn c in _key.Columns)
        {
          builder.AppendFormat("{0}[{1}]", innersep, c.Column);
          if (String.IsNullOrEmpty(c.Collate) == false && String.Compare(c.Collate, "BINARY", StringComparison.OrdinalIgnoreCase) != 0)
            builder.AppendFormat(" COLLATE {0}", c.Collate.ToUpperInvariant());

          if (c.SortMode != ColumnSortMode.Ascending)
            builder.AppendFormat(" DESC");

          innersep = ", ";
        }
        builder.Append(")");

        if (_key.Conflict != ConflictEnum.Abort)
          builder.AppendFormat(" ON CONFLICT {0}", _key.Conflict.ToString().ToUpperInvariant());
      }

      for (int n = 0; n < Check.Count; n++)
      {
        string check = Check[n];

        if (String.IsNullOrEmpty(check) == true) continue;
        SimpleTokenizer.StringParts[] arr = SimpleTokenizer.BreakString(check);
        for (int x = 0; x < arr.Length; x++)
        {
          if (arr[x].depth == 0)
          {
            check = String.Format("({0})", check);
            break;
          }
        }
        builder.Append(separator);
        builder.AppendFormat("CONSTRAINT [CK_{0}_{1}] CHECK {2}", Name, n + 1, check);
      }

      List<ForeignKey> keys = new List<ForeignKey>();

      for (int x = 0; x < ForeignKeys.Count; x++)
      {
        ForeignKey key = ForeignKeys[x];

        if (String.IsNullOrEmpty(key.From.Column) == true || String.IsNullOrEmpty(key.From.Catalog) == true ||
          String.IsNullOrEmpty(key.To.Table) == true || String.IsNullOrEmpty(key.To.Column) == true)
          continue;

        if (keys.Count > 0)
        {
          if (keys[0].Name == key.Name && keys[0].To.Catalog == key.To.Catalog && keys[0].To.Table == key.To.Table)
          {
            keys.Add(key);
            continue;
          }
          builder.Append(separator);
          WriteFKeys(keys, builder);
          keys.Clear();
        }
        keys.Add(key);
      }

      if (keys.Count > 0)
      {
        builder.Append(separator);
        WriteFKeys(keys, builder);
      }

      builder.Append("\r\n);\r\n");

      // Rebuilding an existing table
      if (altName != null)
      {
        separator = "";
        builder.Append("\r\n-- Copy the contents of the old table into the new table\r\n");
        builder.AppendFormat("INSERT INTO [{0}].[{1}] (", _catalog, Name);
        foreach (Column c in Columns)
        {
          if (String.IsNullOrEmpty(c.OriginalName) == false)
          {
            builder.AppendFormat("{1}[{0}]", c.ColumnName, separator);
            separator = ", ";
          }
        }
        builder.Append(")\r\n  SELECT ");
        separator = "";
        foreach (Column c in Columns)
        {
          if (String.IsNullOrEmpty(c.OriginalName) == false)
          {
            builder.AppendFormat("{1}[{0}]", c.OriginalName, separator);
            separator = ", ";
          }
        }
        builder.AppendFormat("\r\n  FROM [{0}].[{1}];\r\n\r\n", _catalog, altName);

        builder.Append("-- Drop the old table\r\n");
        builder.AppendFormat("DROP TABLE [{0}].[{1}];\r\n", _catalog, altName);
      }

      separator = "\r\n";
      if (_indexes.Count > 0)
      {
        builder.Append("\r\n-- Create the new indexes");
        foreach (Index idx in _indexes)
        {
          builder.Append(separator);
          idx.WriteSql(builder);
        }
        builder.AppendLine();
      }

      if (_triggers.Count > 0)
      {
        builder.Append("\r\n-- Create the new triggers");
        foreach (Trigger trig in _triggers)
        {
          builder.Append(separator);
          trig.WriteSql(builder);
          separator = "\r\n";
        }
        builder.AppendLine();
      }

      return builder.ToString();
    }

    private void WriteFKeys(List<ForeignKey> keys, StringBuilder builder)
    {
      builder.AppendFormat("CONSTRAINT [{0}] FOREIGN KEY (", keys[0].Name);
      string separator = "";

      foreach (ForeignKey key in keys)
      {
        builder.AppendFormat("{0}[{1}]", separator, key.From.Column);
        separator = ", ";
      }

      builder.AppendFormat(") REFERENCES [{0}] (", keys[0].To.Table);

      separator = "";
      foreach (ForeignKey key in keys)
      {
        builder.AppendFormat("{0}[{1}]", separator, key.To.Column);
        separator = ", ";
      }
      builder.Append(")");
    }

    [Browsable(false)]
    public override ViewTableBase DesignTable
    {
      get { return this; }
    }

    public override DbConnection GetConnection()
    {
      return _connection;
    }

    #region ICustomTypeDescriptor Members

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(GetType());
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return "Table Design";
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return ToString();
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter(GetType());
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(GetType());
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(GetType());
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(GetType(), editorBaseType);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(GetType(), attributes);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(GetType());
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      return TypeDescriptor.GetProperties(GetType(), attributes);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return TypeDescriptor.GetProperties(GetType());
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion
  }

  internal interface IHaveConnection
  {
    DbConnection GetConnection();
    [Browsable(false)]
    ViewTableBase DesignTable { get; }
  }

  internal interface IHaveConnectionScope : IHaveConnection
  {
    [Browsable(false)]
    string CatalogScope { get; }
    [Browsable(false)]
    string TableScope { get; }
  }

  internal class CollationTypeEditor : ObjectSelectorEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
    {
      base.FillTreeWithData(selector, context, provider);
      selector.AddNode("BINARY", "BINARY", null);
      selector.AddNode("NOCASE", "NOCASE", null);
    }
  }

  internal class CatalogTypeEditor : ObjectSelectorEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
    {
      base.FillTreeWithData(selector, context, provider);
      IHaveConnection source = context.Instance as IHaveConnection;

      if (source == null) return;

      using (DataTable table = source.GetConnection().GetSchema("Catalogs"))
      {
        foreach (DataRow row in table.Rows)
        {
          selector.AddNode(row[0].ToString(), row[0], null);
        }
      }
    }
  }
}
