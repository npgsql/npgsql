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
  using System.ComponentModel.Design;
  using System.Windows.Forms;
  using System.Drawing.Design;
  using System.Data;
  using System.Data.Common;

  public enum TriggerOccurs
  {
    Before = 0,
    After = 1,
  }

  public enum ViewTriggerOccurs
  {
    InsteadOf = 2
  }

  public enum TriggerType
  {
    Delete = 0,
    Insert = 1,
    Update = 2,
  }

  internal class ViewTrigger : Trigger, ICloneable
  {
    internal ViewTrigger(ViewTrigger source)
      : base(source)
    {
      _triggerOccurs = (int)ViewTriggerOccurs.InsteadOf;
    }

    internal ViewTrigger(ViewTableBase parent, DataRow row)
      : base(parent, row)
    {
      _triggerOccurs = (int)ViewTriggerOccurs.InsteadOf;
    }

    [Browsable(true)]
    [DefaultValue(ViewTriggerOccurs.InsteadOf)]
    [Category("Event")]
    [Description("Determines when the trigger fires.  For tables, Before or After are supported.  For views, Instead Of is the only option.")]
    public new ViewTriggerOccurs Occurs
    {
      get
      {
        return ViewTriggerOccurs.InsteadOf;
      }
    }

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return new ViewTrigger(this);
    }

    #endregion
  }

  [DefaultProperty("SQL")]
  internal class Trigger : IHaveConnection, ICloneable
  {
    protected int _triggerOccurs;
    private TriggerType _type = TriggerType.Update;
    //private bool _eachRow;
    private string _when;
    private ViewTableBase _table;
    private string _name;
    private string _columns;
    private string _action;
    private bool _calcname;
    private string _origsql;
    private bool _dirty = false;

    protected Trigger(Trigger source)
    {
      _triggerOccurs = source._triggerOccurs;
      _type = source._type;
      //_eachRow = source._eachRow;
      _when = source._when;
      _table = source._table;
      _name = source._name;
      _columns = source._columns;
      _action = source._action;
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

    internal virtual void WriteSql(StringBuilder builder)
    {
      WriteSql(builder, false);
    }

    private void WriteSql(StringBuilder builder, bool temp)
    {
      string name = Name;

      if (temp == true)
        name = String.Format("{0}_{1}", name, Guid.NewGuid().ToString("N"));

      builder.AppendFormat("CREATE TRIGGER [{0}].[{1}]", _table.Catalog, name);
      switch (_triggerOccurs)
      {
        case 0:
          builder.Append(" BEFORE");
          break;
        case 1:
          builder.Append(" AFTER");
          break;
        case 2:
          builder.Append(" INSTEAD OF");
          break;
      }

      builder.AppendFormat(" {0}", _type.ToString().ToUpperInvariant());
      if (_type == TriggerType.Update && String.IsNullOrEmpty(Columns) == false)
        builder.AppendFormat(" OF {0}", Columns);

      builder.AppendFormat(" ON [{0}].[{1}]", _table.Catalog, _table.Name);

      if (EachRow)
        builder.AppendFormat(" FOR EACH ROW");
      if (String.IsNullOrEmpty(When) == false)
        builder.AppendFormat(" WHEN {0}", When);

      builder.AppendFormat("\r\nBEGIN\r\n{0}", SQL);
      SimpleTokenizer.StringParts[] arr = SimpleTokenizer.BreakString(SQL);
      if (arr[arr.Length - 1].sepchar != ';')
        builder.Append(";");

      builder.Append("\r\nEND;");
    }

    [DefaultValue(TriggerOccurs.Before)]
    [Category("Event")]
    [Description("Determines when the trigger fires.  For tables, Before or After are supported.  For views, Instead Of is the only option.")]
    public virtual TriggerOccurs Occurs
    {
      get { return (TriggerOccurs)_triggerOccurs; }
      set
      {
        if (_triggerOccurs != (int)value)
        {
          _triggerOccurs = (int)value;
          MakeDirty();
        }
      }
    }

    [DefaultValue(TriggerType.Update)]
    [Category("Event")]
    [Description("Determines what type of operation the trigger applies to.  Can be either Insert, Update or Delete.")]
    public TriggerType Type
    {
      get { return _type; }
      set
      {
        if (_type != value)
        {
          _type = value;
          MakeDirty();
        }
      }
    }

    [DefaultValue(true)]
    [Category("Event")]
    [Description("When set to true, the trigger will fire for each row in an update, insert or delete operation.")]
    public bool EachRow
    {
      get { return true; }
      //get { return _eachRow; }
      //set { _eachRow = value; }
    }

    [Category("Constraint")]
    [Description("Limits the exection of the trigger so it only occurs when the expression evaluates to True.")]
    public string When
    {
      get { return _when; }
      set
      {
        if (_when != value)
        {
          _when = value;
          MakeDirty();
        }
      }
    }

    [Browsable(false)]
    protected virtual string NamePrefix
    {
      get { return "TRG"; }
    }

    [Browsable(false)]
    protected virtual string NewName
    {
      get { return "NewTrigger"; }
    }

    [Browsable(false)]
    internal ViewTableBase ViewTableBase
    {
      get { return _table; }
    }

    [ParenthesizePropertyName(true)]
    [Category("Identity")]
    [Description("The name of the trigger")]
    public string Name
    {
      get
      {
        if (String.IsNullOrEmpty(_name))
        {
          if (_calcname == true) return GetHashCode().ToString();

          string name = String.Format("{0}_{1}", NamePrefix, NewName);
          int count = 0;
          string proposed = name;

          _calcname = true;
          for (int n = 0; n < ((IList)_table.Triggers).Count; n++)
          {
            Trigger idx = ((IList)_table.Triggers)[n] as Trigger;
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
        if (_name != value)
        {
          _name = value;
          MakeDirty();
        }
      }
    }

    [Editor(typeof(ColumnsMultiSelectEditor), typeof(UITypeEditor))]
    [Category("Event")]
    [DisplayName("Update Columns")]
    [Description("Limit the trigger to only occur when updating these columns")]
    public string Columns
    {
      get { return _columns; }
      set
      {
        if (_columns != value)
        {
          _columns = value;
          MakeDirty();
        }
      }
    }

    [Category("Action")]
    [Description("The SQL to execute for this trigger")]
    public string SQL
    {
      get { return _action; }
      set
      {
        //using (DbTransaction trans = GetConnection().BeginTransaction())
        //using (DbCommand cmd = GetConnection().CreateCommand())
        //{
        //  StringBuilder builder = new StringBuilder();
        //  WriteSql(builder, true);
        //  cmd.CommandText = builder.ToString();

        //  cmd.ExecuteNonQuery();
        //  trans.Rollback();
        //}
        if (_action != value)
        {
          _action = value;
          MakeDirty();
        }
      }
    }

    [Browsable(false)]
    public string OriginalSql
    {
      get { return _origsql; }
    }

    internal Trigger(ViewTableBase parent, DataRow row)
    {
      _table = parent;
      if (row != null)
      {
        _name = row["TRIGGER_NAME"].ToString();

        string sql = row["TRIGGER_DEFINITION"].ToString();
        _origsql = sql;
        SimpleTokenizer.StringParts[] arr = SimpleTokenizer.BreakString(sql);

        int x = 3;
        switch (arr[x].keyword)
        {
          case "BEFORE":
            _triggerOccurs = 0;
            break;
          case "AFTER":
            _triggerOccurs = 1;
            break;
          case "INSTEAD":
            _triggerOccurs = 2;
            x++;
            break;
          default:
            x--;
            break;
        }
        x++;

        switch (arr[x].keyword)
        {
          case "UPDATE":
            _type = TriggerType.Update;
            if (arr[x + 1].keyword == "OF")
            {
              x++;
              StringBuilder builder = new StringBuilder();
              string separator = "";
              while (arr[x + 1].keyword != "ON")
              {
                builder.AppendFormat("{0}[{1}]", separator, arr[x + 1].value);
                separator = ", ";
                x++;
              }
              _columns = builder.ToString();
            }
            break;
          case "INSERT":
            _type = TriggerType.Insert;
            break;
          case "DELETE":
            _type = TriggerType.Delete;
            break;
        }
        x++;

        while (arr[x].keyword != "BEGIN")
        {
          if (arr[x].keyword == "FOR" && arr[x + 1].keyword == "EACH" && arr[x + 1].keyword == "ROW")
          {
            //_eachRow = true;
            x += 3;
            continue;
          }

          if (arr[x].keyword == "WHEN")
          {
            x++;
            int depth = 0;
            StringBuilder builder = new StringBuilder();
            while (arr[x].keyword != "BEGIN")
            {
              if (builder.Length > 0) builder.Append(" ");
              while (depth < arr[x].depth)
              {
                depth++;
                builder.Append("(");
              }
              while (depth > arr[x].depth)
              {
                depth--;
                builder.Append(")");
              }

              if (String.IsNullOrEmpty(arr[x].quote) == false)
                builder.Append(arr[x].quote[0]);
              builder.Append(arr[x].value);
              if (String.IsNullOrEmpty(arr[x].quote) == false)
                builder.Append(arr[x].quote[1]);
            }
            while (depth > 0)
            {
              depth--;
              builder.Append(")");
            }
            _when = builder.ToString();
            break;
          }
          x++;
        }

        int startpos = arr[x].position + arr[x].value.Length;

        x = arr.Length - 1;
        while (arr[x].keyword != "END")
        {
          x--;
        }
        int endpos = arr[x].position;

        _action = sql.Substring(startpos, endpos - startpos);
        _action = _action.TrimStart('\r').TrimStart('\n').TrimEnd('\n').TrimEnd('\r').Trim();
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

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return new Trigger(this);
    }

    #endregion
  }

  internal class TriggerEditor : CollectionEditor
  {
    ViewTableBase _table;
    object[] _items;
    object[] _orig;
    CollectionEditor.CollectionForm _form;
    int _count;

    internal TriggerEditor(ViewTableBase parent)
      : base((parent is View) ? typeof(List<ViewTrigger>) :  typeof(List<Trigger>))
    {
      _table = parent;
    }

    protected override CollectionEditor.CollectionForm CreateCollectionForm()
    {
      _form = base.CreateCollectionForm();
      _form.Text = "Trigger Editor";
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
      if (itemType == typeof(Trigger))
      {
        return new Trigger(_table, null);
      }
      else if (itemType == typeof(ViewTrigger))
      {
        return new ViewTrigger(_table, null);
      }
      throw new NotSupportedException();
    }

    protected override object[] GetItems(object editValue)
    {
      if (_items == null)
      {
        IList value = editValue as IList;
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
      bool dirty = false;
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
          Trigger trig = value[i] as Trigger;

          if (trig != null && String.IsNullOrEmpty(trig.SQL) == false)
          {
            if (trig.IsDirty) dirty = true;
            trig.Name = trig.Name;
            list.Add(trig);
          }
        }

        if ((dirty == true || list.Count != _count) && _form.DialogResult == DialogResult.OK)
          _table.MakeDirty();
      }

      return editValue;
    }
  }
}
