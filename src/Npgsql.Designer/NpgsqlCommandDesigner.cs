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
  using System.Data;

  /// <summary>
  /// This object provides a designer for a SQLiteCommand.  The reason we provide an additional
  /// CommandDesignTimeVisible property is because certain MS designer components will look for it and
  /// fail if its not there.
  /// </summary>
  [ProvideProperty("CommandDesignTimeVisible", typeof(IDbCommand))]
  internal sealed class NpgsqlCommandDesigner : ComponentDesigner, IExtenderProvider
  {
    public NpgsqlCommandDesigner()
    {
    }

    /// <summary>
    /// Initialize the instance with the given SQLiteCommand component
    /// </summary>
    /// <param name="component"></param>
    public override void Initialize(IComponent component)
    {
      base.Initialize(component);
    }

    /// <summary>
    /// Add our designtimevisible attribute to the attributes for the item
    /// </summary>
    /// <param name="attributes"></param>
    protected override void PreFilterAttributes(System.Collections.IDictionary attributes)
    {
      base.PreFilterAttributes(attributes);
      DesignTimeVisibleAttribute att = new DesignTimeVisibleAttribute(((DbCommand)Component).DesignTimeVisible);
      attributes[att.TypeId] = att;
    }

    /// <summary>
    /// Provide a get method for the CommandDesignTimeVisible provided property
    /// </summary>
    /// <param name="command">The SQLiteCommand we're designing for</param>
    /// <returns>True or false if the object is visible in design mode</returns>
    [Browsable(false), DesignOnly(true), DefaultValue(true)]
    public bool GetCommandDesignTimeVisible(IDbCommand command)
    {
      return ((DbCommand)command).DesignTimeVisible;
    }

    /// <summary>
    /// Provide a set method for our supplied CommandDesignTimeVisible property
    /// </summary>
    /// <param name="command">The SQLiteCommand to set</param>
    /// <param name="visible">The new designtime visible property to assign to the command</param>
    public void SetCommandDesignTimeVisible(IDbCommand command, bool visible)
    {
      ((DbCommand)command).DesignTimeVisible = visible;
    }

    #region IExtenderProvider Members

    /// <summary>
    /// We extend any DbCommand
    /// </summary>
    /// <param name="extendee">The object being tested</param>
    /// <returns>True if the object derives from DbCommand</returns>
    public bool CanExtend(object extendee)
    {
      return (extendee is DbCommand);
    }

    #endregion
  }
}
