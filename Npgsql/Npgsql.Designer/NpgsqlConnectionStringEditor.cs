/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using System.Reflection;
  using System.Data;
  using System.Data.Common;
  using System.ComponentModel.Design;
  using System.ComponentModel;

  /// <summary>
  /// This class provides connectionstring editing support in the properties window when
  /// using a SQLiteConnection as a toolbox component on a form (for example).
  ///
  /// In order to provide the dropdown list, unless someone knows a better way, I have to use
  /// the internal VsConnectionManager class since it utilizes some interfaces in the designer
  /// that are internal to the VSDesigner object.  We instantiate it and utilize it through reflection.
  /// </summary>
  internal sealed class NpgsqlConnectionStringEditor : ObjectSelectorEditor
  {
    private ObjectSelectorEditor.Selector _selector = null;

    private static Type _managerType = null;

    static NpgsqlConnectionStringEditor()
    {
      Assembly assm = NpgsqlDataAdapterToolboxItem._vsdesigner;
      if (assm != null)
      {
        _managerType = assm.GetType("Microsoft.VSDesigner.Data.VS.VsConnectionManager");
      }
    }

    public NpgsqlConnectionStringEditor()
    {
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (provider == null || context == null) return value;
      if (context.Instance == null) return value;

      try
      {
        context.OnComponentChanging();
        object newConnection = base.EditValue(context, provider, value);
        string connectionString = newConnection as string;
        int index = -1;

        if (connectionString == null && newConnection != null)
        {
          if (_managerType != null)
          {
            object manager = Activator.CreateInstance(_managerType, new object[] { provider });
            if (manager != null)
            {
              index = (int)_managerType.InvokeMember("AddNewConnection", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { "Npgsql" });
              if (index > -1 && _selector != null)
              {
                connectionString = (string)_managerType.InvokeMember("GetConnectionString", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { index });
                _selector.SelectedNode = _selector.AddNode((string)_managerType.InvokeMember("GetConnectionName", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { index }), connectionString, null);
              }
            }
          }
        }

        if (String.IsNullOrEmpty(connectionString) == false)
        {
          value = connectionString;
        }
        context.OnComponentChanged();
      }
      catch
      {
      }
      return value;
    }

    protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
    {
      object manager = Activator.CreateInstance(_managerType, new object[] { provider });
      DbConnection connection = (DbConnection)context.Instance;
      ObjectSelectorEditor.SelectorNode node;

      _selector = selector;

      _selector.Clear();

      if (manager != null)
      {
        int items = (int)_managerType.InvokeMember("GetConnectionCount", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, null);
        string dataProvider;
        string connectionString;
        string connectionName;

        for (int n = 0; n < items; n++)
        {
          connectionString = (string)_managerType.InvokeMember("GetConnectionString", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { n });
          connectionName = (string)_managerType.InvokeMember("GetConnectionName", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { n });
          dataProvider = (string)_managerType.InvokeMember("GetProvider", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, manager, new object[] { n });
          if (String.Compare(dataProvider, "Npgsql", true) == 0)
          {
            node = selector.AddNode(connectionName, connectionString, null);

            if (String.Compare(connectionString, connection.ConnectionString, true) == 0)
              selector.SelectedNode = node;
          }
        }
        selector.AddNode("<New Connection...>", this, null);
      }
    }
  }
}
