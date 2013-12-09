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

  internal class View : ViewTableBase, ICustomTypeDescriptor
  {
    private string _name;
    private string _oldname;
    private string _sql;
    private string _oldsql;
    private ViewDesignerDoc _owner;
    private string _catalog;
    private DbConnection _connection;
    List<ViewTrigger> _triggers = new List<ViewTrigger>();
    List<ViewTrigger> _oldtriggers = new List<ViewTrigger>();

    internal View(string viewName, DbConnection connection, ViewDesignerDoc parent)
    {
      _owner = parent;
      _name = viewName;
      _oldname = viewName;
      _catalog = connection.Database;
      _connection = connection;
      _owner.Name = _name;

      if (String.IsNullOrEmpty(viewName) == false)
      {
        using (DataTable tbl = connection.GetSchema("Views", new string[] { Catalog, null, Name }))
        {
          if (tbl.Rows.Count > 0)
          {
            _sql = tbl.Rows[0]["VIEW_DEFINITION"].ToString();

            StringBuilder builder = new StringBuilder();
            builder.Append(_sql);
            builder.AppendLine(";");

            _triggers.Clear();
            _oldtriggers.Clear();

            using (DataTable ttbl = _connection.GetSchema("Triggers", new string[] { Catalog, null, Name }))
            {
              foreach (DataRow row in ttbl.Rows)
              {
                ViewTrigger t = new ViewTrigger(this, row);
                _triggers.Add(t);
                _oldtriggers.Add(((ICloneable)t).Clone() as ViewTrigger);

                builder.AppendFormat("{0};\r\n", t.OriginalSql);
              }
            }
            _oldsql = builder.ToString();
          }
          else
          {
            _oldname = null;
          }
        }
      }
    }

    public override void MakeDirty()
    {
      _owner.MakeDirty();
    }

    [Browsable(false)]
    public override object Triggers
    {
      get { return _triggers; }
    }

    public void Committed()
    {
      _oldsql = _sql;
      _oldname = _name;

      _oldtriggers = _triggers;
      _triggers.Clear();
      foreach (ViewTrigger trig in _oldtriggers)
      {
        _triggers.Add(((ICloneable)trig).Clone() as ViewTrigger);
      }
    }

    public override string ToString()
    {
      return String.Format("[{0}].[{1}]", Catalog, Name);
    }

    [Category("Storage")]
    [RefreshProperties(RefreshProperties.All)]
    [ParenthesizePropertyName(true)]
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
    public string SqlText
    {
      get { return _sql; }
      set
      {
        if (String.Compare(_sql, value, StringComparison.OrdinalIgnoreCase) != 0)
        {
          _sql = value;
          _owner.MakeDirty();
        }
      }
    }

    [Browsable(false)]
    public string OriginalSql
    {
      get { return _oldsql; }
    }

    public string GetSqlText()
    {
      if (String.Compare(_sql, _oldsql, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(_name, _oldname, StringComparison.OrdinalIgnoreCase) == 0) return null;

      StringBuilder builder = new StringBuilder();

      if (String.IsNullOrEmpty(_oldname) == false)
      {
        foreach (ViewTrigger trig in _oldtriggers)
        {
          builder.AppendFormat("DROP TRIGGER [{0}].[{1}];\r\n", Catalog, trig.Name);
        }
        builder.AppendFormat("DROP VIEW [{0}].[{1}];\r\n", Catalog, _oldname);
      }

      builder.AppendFormat("CREATE VIEW [{0}].[{1}] AS {2};\r\n", Catalog, Name, SqlText);

      string sep = "";
      foreach (ViewTrigger trig in _triggers)
      {
        builder.Append(sep);
        trig.WriteSql(builder);
        sep = "\r\n";
      }

      return builder.ToString();
    }

    #region ICustomTypeDescriptor Members

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(GetType());
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return "View Design";
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

    [Browsable(false)]
    public override ViewTableBase DesignTable
    {
      get { return this; }
    }

    public override DbConnection GetConnection()
    {
      return _connection;
    }
  }
}
