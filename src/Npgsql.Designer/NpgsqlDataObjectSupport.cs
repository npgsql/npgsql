/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using Microsoft.VisualStudio.Data;
  using Microsoft.VisualStudio.OLE.Interop;
  using Microsoft.VisualStudio.Data.AdoDotNet;

  /// <summary>
  /// Doesn't do much other than provide the DataObjectSupport base object with a location
  /// where the XML resource can be found.
  /// </summary>
  internal sealed class NpgsqlDataObjectSupport : DataObjectSupport
  {
    public NpgsqlDataObjectSupport()
      : base("Npgsql.Designer.SQLiteDataObjectSupport", typeof(NpgsqlDataObjectSupport).Assembly)
    {
    }
  }
}
