/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using Microsoft.VisualStudio.Data.AdoDotNet;
  using Microsoft.VisualStudio.Data;
  using System.Runtime.InteropServices;
  using Microsoft.Data.ConnectionUI;

  /// <summary>
  /// For a package-based provider, this factory creates instances of the main objects we support
  /// </summary>
  [Guid("DCBE6C8D-0E57-4099-A183-98FF74C64D9D")]
  internal sealed class NpgsqlProviderObjectFactory : AdoDotNetProviderObjectFactory
  {
    public NpgsqlProviderObjectFactory()
    {
    }

    public override object CreateObject(Type objType)
    {
      if (objType == typeof(DataConnectionSupport))
        return new NpgsqlDataConnectionSupport();

      if (objType == typeof(IDataConnectionProperties) || objType == typeof(DataConnectionProperties))
        return new NpgsqlConnectionProperties();

      if (objType == typeof(IDataConnectionUIControl) || objType == typeof(DataConnectionUIControl))
        return new NpgsqlConnectionUIControl();

      return base.CreateObject(objType);
    }
  }
}
