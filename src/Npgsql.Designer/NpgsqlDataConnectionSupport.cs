/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using Microsoft.VisualStudio.Data;
  using Microsoft.VisualStudio.OLE.Interop;
  using Microsoft.VisualStudio.Data.AdoDotNet;
  using Microsoft.Win32;

  /// <summary>
  /// This class creates many of the DDEX components when asked for by the server explorer.
  /// </summary>
  internal sealed class NpgsqlDataConnectionSupport : AdoDotNetConnectionSupport
  {
    private NpgsqlDataViewSupport _dataViewSupport;
    private NpgsqlDataObjectSupport _dataObjectSupport;
    private NpgsqlDataObjectIdentifierResolver _dataObjectIdentifierResolver;

    public NpgsqlDataConnectionSupport()
      : base("Npgsql")
    {
    }

    protected override DataSourceInformation CreateDataSourceInformation()
    {
      return new NpgsqlDataSourceInformation(Site as DataConnection);
    }

    protected override object GetServiceImpl(Type serviceType)
    {
      if (serviceType == typeof(DataViewSupport))
      {
        if (_dataViewSupport == null) _dataViewSupport = new NpgsqlDataViewSupport();
        return _dataViewSupport;
      }

      if (serviceType == typeof(DataObjectSupport))
      {
        if (_dataObjectSupport == null) _dataObjectSupport = new NpgsqlDataObjectSupport();
        return _dataObjectSupport;
      }

      if (serviceType == typeof(DataObjectIdentifierResolver))
      {
        if (_dataObjectIdentifierResolver == null) _dataObjectIdentifierResolver = new NpgsqlDataObjectIdentifierResolver(Site);
        return _dataObjectIdentifierResolver;
      }

      if (serviceType == typeof(DataConnectionSupport))
        return this;

      return base.GetServiceImpl(serviceType);
    }
  }
}
