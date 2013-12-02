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

  internal class CheckEditor : CollectionEditor
  {
    Table _table;
    CollectionEditor.CollectionForm _form;
    object[] _items;
    object[] _orig;
    int _count;

    internal CheckEditor(Table parent)
      : base(typeof(List<string>))
    {
      _table = parent;
    }

    protected override CollectionEditor.CollectionForm CreateCollectionForm()
    {
      _form = base.CreateCollectionForm();
      _form.Text = "CHECK Constraint Editor";
      _form.Width = (int)(_form.Width * 1.25);
      _form.Height = (int)(_form.Height * 1.25);
      return _form;
    }

    protected override object CreateInstance(Type itemType)
    {
      if (itemType == typeof(string)) return String.Empty;
      return base.CreateInstance(itemType);
    }

    protected override object[] GetItems(object editValue)
    {
      if (_items == null)
      {
        List<string> items = editValue as List<string>;
        _items = new object[items.Count];
        _orig = new object[items.Count];

        for (int n = 0; n < _items.Length; n++)
        {
          _items[n] = items[n];
          _orig[n] = items[n];
        }
        _count = _items.Length;
      }
      return _items;
    }

    protected override object SetItems(object editValue, object[] value)
    {
      bool dirty = false;
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
          string val = value[i] as string;

          if (String.IsNullOrEmpty(val) == false)
          {
            list.Add(val);
            if (i < _orig.Length && val != (string)_orig[i])
              dirty = true;
          }
        }
        if (list.Count != _count)
          dirty = true;

        if (dirty == true && _form.DialogResult == DialogResult.OK)
          _table._owner.MakeDirty();
      }
      return editValue;
    }
  }
}
