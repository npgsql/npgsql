/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Data.Common;
  using System.Collections;
  using System.Reflection;

  /// <summary>
  /// The purpose of this class is to provide context menus and event support when designing a
  /// SQLite DataSet.  Most of the functionality is implemented by MS's VSDesigner object which we
  /// instantiate through reflection since I don't really have a design-time reference to the object
  /// and many of the objects in VSDesigner are internal.
  /// </summary>
  internal sealed class NpgsqlAdapterDesigner : ComponentDesigner, IExtenderProvider
  {
    private ComponentDesigner _designer = null;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public NpgsqlAdapterDesigner()
    {
    }

    /// <summary>
    /// Initialize the designer by creating a SqlDataAdapterDesigner and delegating most of our
    /// functionality to it.
    /// </summary>
    /// <param name="component"></param>
    public override void Initialize(IComponent component)
    {
      base.Initialize(component);

      // Initialize a SqlDataAdapterDesigner through reflection and set it up to work on our behalf
      if (NpgsqlDataAdapterToolboxItem._vsdesigner != null)
      {
        Type type = NpgsqlDataAdapterToolboxItem._vsdesigner.GetType("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner");
        if (type != null)
        {
          _designer = (ComponentDesigner)Activator.CreateInstance(type);
          _designer.Initialize(component);
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (_designer != null && disposing)
        ((IDisposable)_designer).Dispose();

      base.Dispose(disposing);
    }

    /// <summary>
    /// Forwards to the SqlDataAdapterDesigner object
    /// </summary>
    public override DesignerVerbCollection Verbs
    {
      get
      {
        return (_designer != null) ? _designer.Verbs : null;
      }
    }

    /// <summary>
    /// Forwards to the SqlDataAdapterDesigner object
    /// </summary>
    public override ICollection AssociatedComponents
    {
      get
      {
        return (_designer != null) ? _designer.AssociatedComponents : null;
      }
    }

    #region IExtenderProvider Members
    /// <summary>
    /// We extend support for DbDataAdapter-derived objects
    /// </summary>
    /// <param name="extendee">The object wanting to be extended</param>
    /// <returns>Whether or not we extend that object</returns>
    public bool CanExtend(object extendee)
    {
      return (extendee is DbDataAdapter);
    }

    #endregion
  }
}
