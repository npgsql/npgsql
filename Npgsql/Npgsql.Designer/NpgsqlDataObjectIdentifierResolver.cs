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

  /// <summary>
  /// This class is used to build identifier arrays and contract them.  Typically they are
  /// passed to SQLiteConnection.GetSchema() or are contracted for display on the screen or in the
  /// properties window.
  /// </summary>
  internal sealed class NpgsqlDataObjectIdentifierResolver : DataObjectIdentifierResolver, IObjectWithSite
  {
    private DataConnection _connection;

    public NpgsqlDataObjectIdentifierResolver()
    {
    }

    internal NpgsqlDataObjectIdentifierResolver(object site)
    {
      _connection = site as DataConnection;
    }

    protected override object[] QuickExpandIdentifier(string typeName, object[] partialIdentifier)
    {
      if (typeName == null)
      {
        throw new ArgumentNullException("typeName");
      }

      // Create an identifier array of the correct full length based on
      // the object type
      object[] identifier = null;
      int length = 0;

      switch (typeName.ToLowerInvariant())
      {
        case "":
          length = 0;
          break;
        case "table":
        case "view":
          length = 3;
          break;
        case "column":
        case "index":
        case "foreignkey":
        case "viewcolumn":
        case "triggers":
          length = 4;
          break;
        case "indexcolumn":
        case "foreignkeycolumn":
          length = 5;
          break;
        default:
          throw new NotSupportedException();
      }
      identifier = new object[length];

      // If the input identifier is not null, copy it to the full
      // identifier array.  If the input identifier's length is less
      // than the full length we assume the more specific parts are
      // specified and thus copy into the rightmost portion of the
      // full identifier array.
      if (partialIdentifier != null)
      {
        if (partialIdentifier.Length > length)
        {
          throw new InvalidOperationException();
        }
        partialIdentifier.CopyTo(identifier, length - partialIdentifier.Length);
      }

      if (length > 0)
      {
        // Fill in the current database if not specified
        if (!(identifier[0] is string))
        {
          identifier[0] = _connection.SourceInformation[DataSourceInformation.DefaultCatalog] as string;
        }
      }

      if (length > 1)
      {
        identifier[1] = null;
      }

      return identifier;
    }

    /// <summary>
    /// Strips out the schema, which we don't really support but has to be there for certain operations internal
    /// to MS's designer implementation.
    /// </summary>
    /// <param name="typeName">The type of identifier to contract</param>
    /// <param name="fullIdentifier">The full identifier array</param>
    /// <returns>A contracted identifier array</returns>
    protected override object[] QuickContractIdentifier(string typeName, object[] fullIdentifier)
    {
      if (fullIdentifier.Length < 2) return fullIdentifier;

      object[] identifier = new object[fullIdentifier.Length - 1];

      for (int n = 1; n < fullIdentifier.Length; n++)
      {
        identifier[n - 1] = fullIdentifier[n];
      }

      identifier[0] = fullIdentifier[0];

      return identifier;
    }

    /// <summary>
    /// GetSite does not need to be implemented since
    /// DDEX only calls SetSite to site the object.
    /// </summary>
    void IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
    {
      ppvSite = IntPtr.Zero;
      throw new NotImplementedException();
    }

    void IObjectWithSite.SetSite(object pUnkSite)
    {
      _connection = (DataConnection)pUnkSite;
    }
  }
}
