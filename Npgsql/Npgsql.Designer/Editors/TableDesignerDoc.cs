/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer.Editors
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Data.Common;
  using System.Drawing;
  using System.Text;
  using System.Windows.Forms;
  using Microsoft.VisualStudio.Shell.Interop;
  using Microsoft.VisualStudio.OLE.Interop;
  using Microsoft.VisualStudio;
  using Microsoft.VisualStudio.Data;
  using Npgsql.Designer.Design;
  using System.ComponentModel.Design;

  public partial class TableDesignerDoc : DesignerDocBase,
    IVsPersistDocData,
    IVsWindowPane,
    IOleCommandTarget,
    ISelectionContainer,
    IVsWindowPaneCommit,
    IVsWindowFrameNotify
  {
    private static Dictionary<int, string> _editingTables = new Dictionary<int, string>();

    internal DataConnection _connection;
    internal Microsoft.VisualStudio.Data.ServiceProvider _serviceProvider;
    internal Table _table;
    internal bool _dirty;
    internal bool _init = false;
    internal DataViewHierarchyAccessor _accessor;
    internal int _itemId;
    static private bool _warned = false;

    public TableDesignerDoc(int itemId, DataViewHierarchyAccessor accessor, string tableName)
    {
      _accessor = accessor;
      _connection = accessor.Connection;
      _itemId = itemId;
      _init = true;

      InitializeComponent();

      StringBuilder tables = new StringBuilder();

      using (DataReader reader = _connection.Command.Execute("SELECT * FROM sqlite_master", 1, null, 30))
      {
        while (reader.Read())
        {
          tables.Append(reader.GetItem(2).ToString());
          tables.Append(",");
        }
      }

      int n = 1;

      if (String.IsNullOrEmpty(tableName))
      {
        string alltables = tables.ToString();

        do
        {
          tableName = String.Format("Table{0}", n);
          n++;
        } while (alltables.IndexOf(tableName + ",", StringComparison.OrdinalIgnoreCase) > -1 || _editingTables.ContainsValue(tableName));

        _editingTables.Add(GetHashCode(), tableName);
      }
      _table = new Table(tableName, _connection.ConnectionSupport.ProviderObject as DbConnection, this);
      foreach(Column c in _table.Columns)
      {
        n = _dataGrid.Rows.Add();
        _dataGrid.Rows[n].Tag = c;
        c.Parent = _dataGrid.Rows[n];
      }
      _init = false;

      if (_dataGrid.Rows.Count > 0)
      {
        _dataGrid.EndEdit();
        _sqlText.Text = _table.OriginalSql;
      }
    }

    public override string CanonicalName
    {
      get
      {
        return _table.Name;
      }
    }

    void SetPropertyWindow()
    {
      IVsTrackSelectionEx track = _serviceProvider.GetService(typeof(SVsTrackSelectionEx)) as IVsTrackSelectionEx;
      if (track != null)
      {
        track.OnSelectChange(this);
      }
    }

    public string Caption
    {
      get
      {
        string catalog = "main";
        if (_table != null) catalog = _table.Catalog;

        return String.Format("{0}.{1} Table (SQLite [{2}])", catalog, base.Name, ((DbConnection)_connection.ConnectionSupport.ProviderObject).DataSource);
      }
    }

    public new string Name
    {
      get
      {
        if (_table != null)
          return _table.Name;
        else return base.Name;
      }
      set
      {
        base.Name = value;

        if (_serviceProvider != null)
        {
          IVsWindowFrame frame = _serviceProvider.GetService(typeof(IVsWindowFrame)) as IVsWindowFrame;
          if (frame != null)
          {
            frame.SetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, Caption);
          }
        }
      }
    }

    public void NotifyChanges()
    {
      if (_serviceProvider == null) return;

      _sqlText.Text = _table.GetSql();

      // Get a reference to the Running Document Table
      IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable)_serviceProvider.GetService(typeof(SVsRunningDocumentTable));

      // Lock the document
      uint docCookie;
      IVsHierarchy hierarchy;
      uint itemID;
      IntPtr docData;
      int hr = runningDocTable.FindAndLockDocument(
          (uint)_VSRDTFLAGS.RDT_ReadLock,
          base.Name,
          out hierarchy,
          out itemID,
          out docData,
          out docCookie
      );
      ErrorHandler.ThrowOnFailure(hr);

      IVsUIShell shell = _serviceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;
      if (shell != null)
      {
        shell.UpdateDocDataIsDirtyFeedback(docCookie, (_dirty == true) ? 1 : 0);
      }

      // Unlock the document.
      // Note that we have to unlock the document even if the previous call failed.
      runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);

      // Check ff the call to NotifyDocChanged failed.
      //ErrorHandler.ThrowOnFailure(hr);
    }

    #region IVsPersistDocData Members

    int IVsPersistDocData.Close()
    {
      return VSConstants.S_OK;
    }

    public int GetGuidEditorType(out Guid pClassID)
    {
      return ((IPersistFileFormat)this).GetClassID(out pClassID);
    }

    public int IsDocDataDirty(out int pfDirty)
    {
      pfDirty = _dirty == true ? 1 : 0;
      return VSConstants.S_OK;
    }

    public int IsDocDataReloadable(out int pfReloadable)
    {
      pfReloadable = 0;
      return VSConstants.S_OK;
    }

    public int LoadDocData(string pszMkDocument)
    {
      return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
    }

    public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
    {
      return VSConstants.S_OK;
    }

    public int ReloadDocData(uint grfFlags)
    {
      return VSConstants.E_NOTIMPL;
    }

    public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
    {
      return VSConstants.E_NOTIMPL;
    }

    private string GetChangeScript()
    {
      _dataGrid.EndEdit();
      _init = true;
      for (int n = 0; n < _table.Columns.Count; n++)
      {
        Column c = _table.Columns[n];
        if (String.IsNullOrEmpty(c.ColumnName) == true)
        {
          try
          {
            _dataGrid.Rows.Remove(c.Parent);
          }
          catch
          {
            c.Parent.Tag = null;
          }
          _table.Columns.Remove(c);
          n--;
          continue;
        }
      }

      for (int n = 0; n < _dataGrid.Rows.Count; n++)
      {
        if ((_dataGrid.Rows[n].Tag is Column) == false)
        {
          try
          {
            _dataGrid.Rows.RemoveAt(n);
          }
          catch
          {
            if (n == _dataGrid.Rows.Count - 1) break;
          }
          n--;
        }
      }
      _init = false;

      return _table.GetSql();
    }

    public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = _table.Name;
      pfSaveCanceled = 0;

      if (String.IsNullOrEmpty(_table.OriginalSql) == true)
      {
        using (TableNameDialog dlg = new TableNameDialog("Table", _table.Name))
        {
          if (dlg.ShowDialog(this) == DialogResult.Cancel)
          {
            pfSaveCanceled = 1;
            return VSConstants.S_OK;
          }
          _table.Name = dlg.TableName;
        }
      }

      string sql = GetChangeScript();

      using (DbTransaction trans = _table.GetConnection().BeginTransaction())
      {
        try
        {
          using (DbCommand cmd = _table.GetConnection().CreateCommand())
          {
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
          }
          trans.Commit();
        }
        catch (Exception)
        {
          trans.Rollback();
          throw;
        }
      }

      _dirty = false;
      _table.Committed();
      NotifyChanges();
      _sqlText.Text = _table.OriginalSql;

      NpgsqlCommandHandler.Refresh(_accessor, _itemId);

      _dataGrid_SelectionChanged(this, EventArgs.Empty);

      RefreshToolbars();

      return VSConstants.S_OK;
    }

    public int SetUntitledDocPath(string pszDocDataPath)
    {
      return ((IPersistFileFormat)this).InitNew(0);
    }

    #endregion

    #region IVsWindowPane Members

    public int ClosePane()
    {
      this.Dispose(true);
      return VSConstants.S_OK;
    }

    public int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
    {
      Win32Methods.SetParent(Handle, hwndParent);
      hwnd = Handle;

      Size = new System.Drawing.Size(cx - x, cy - y);
      return VSConstants.S_OK;
    }

    public int GetDefaultSize(Microsoft.VisualStudio.OLE.Interop.SIZE[] size)
    {
      if (size.Length >= 1)
      {
        size[0].cx = Size.Width;
        size[0].cy = Size.Height;
      }

      return VSConstants.S_OK;
    }

    public int LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
    {
      return VSConstants.S_OK;
    }

    public int SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
    {
      return VSConstants.S_OK;
    }

    public void RefreshToolbars()
    {
      if (_serviceProvider == null) return;

      IVsUIShell shell = _serviceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;

      if (shell != null)
      {
        shell.UpdateCommandUI(1);
      }
    }

    public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      _serviceProvider = new ServiceProvider(psp);
      return VSConstants.S_OK;
    }

    public int TranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
    {
      return VSConstants.S_FALSE;
    }

    #endregion

    public void MakeDirty()
    {
      _dirty = true;
      NotifyChanges();
    }

    private bool IsPkSelected()
    {
      bool newVal = false;
      foreach (Column c in _propertyGrid.SelectedObjects)
      {
        foreach (IndexColumn ic in _table.PrimaryKey.Columns)
        {
          if (String.Compare(c.ColumnName, ic.Column, StringComparison.OrdinalIgnoreCase) == 0)
          {
            newVal = true;
            break;
          }
        }
      }
      return newVal;
    }

    #region IOleCommandTarget Members

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
      {
        switch ((VSConstants.VSStd97CmdID)nCmdID)
        {
          case VSConstants.VSStd97CmdID.GenerateChangeScript:
            {
              using (ChangeScriptDialog dlg = new ChangeScriptDialog(_table.Name, (_dirty == true) ? GetChangeScript() : String.Empty, _table.OriginalSql))
              {
                dlg.ShowDialog(this);
              }
            }
            return VSConstants.S_OK;
          case VSConstants.VSStd97CmdID.PrimaryKey:
            bool newVal = IsPkSelected();

            if (newVal == false)
            {
              _table.PrimaryKey.Columns.Clear();
              foreach (Column c in _propertyGrid.SelectedObjects)
              {
                IndexColumn newcol = new IndexColumn(_table.PrimaryKey, null);
                newcol.Column = c.ColumnName;
                _table.PrimaryKey.Columns.Add(newcol);
              }
            }
            else
            {
              foreach (Column c in _propertyGrid.SelectedObjects)
              {
                foreach (IndexColumn ic in _table.PrimaryKey.Columns)
                {
                  if (String.Compare(c.ColumnName, ic.Column, StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    _table.PrimaryKey.Columns.Remove(ic);
                    break;
                  }
                }
              }
            }

            _dataGrid_SelectionChanged(this, EventArgs.Empty);
            _dataGrid.Invalidate();
            MakeDirty();
            return VSConstants.S_OK;

          case VSConstants.VSStd97CmdID.Cut:
            _clipRows = SelectedRows;
            foreach (DataGridViewRow row in _clipRows)
            {
              _dataGrid.Rows.Remove(row);
              _table.Columns.Remove(row.Tag as Column);
              MakeDirty();
            }
            Clipboard.SetData("SQLiteTableDesigner", Caption);
            RefreshToolbars();
            return VSConstants.S_OK;

          case VSConstants.VSStd97CmdID.Copy:
            _clipRows = SelectedRows;
            Clipboard.SetData("SQLiteTableDesigner", Caption);
            RefreshToolbars();
            return VSConstants.S_OK;

          case VSConstants.VSStd97CmdID.Paste:
            DataGridViewRow[] arr = GetClipboardRows();
            int rowIndex = _dataGrid.CurrentRow.Index;
            _dataGrid.EndEdit();

            for (int n = arr.Length - 1; n > -1; n--)
            {
              DataGridViewRow row = arr[n];
              Column c = row.Tag as Column;
              bool hasCol = false;
              foreach (Column oc in _table.Columns)
              {
                if (String.Compare(c.ColumnName, oc.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                  hasCol = true;
                  break;
                }
              }
              _dataGrid.Rows.Insert(rowIndex, 1);
              DataGridViewRow newrow = _dataGrid.Rows[rowIndex];

              if (hasCol == true)
              {
                Column oc = c;
                c = new Column(_table, newrow);
                int num = 1;
                while (String.IsNullOrEmpty(c.ColumnName) == true)
                {
                  bool found = false;
                  string proposed = String.Format("{0}{1}", oc.ColumnName, num);
                  foreach (Column cc in _table.Columns)
                  {
                    if (String.Compare(cc.ColumnName, proposed, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                      found = true;
                      break;
                    }
                  }
                  if (found == true)
                  {
                    num++;
                  }
                  else
                    c.ColumnName = proposed;
                }
                c.AllowNulls = oc.AllowNulls;
                c.Collate = oc.Collate;
                c.DataType = oc.DataType;
                c.DefaultValue = oc.DefaultValue;
                c.Unique.Enabled = oc.Unique.Enabled;
                c.Unique.Conflict = oc.Unique.Conflict;
              }

              c.Parent = newrow;
              newrow.Tag = c;
              newrow.SetValues(c.ColumnName, c.DataType, c.AllowNulls);
              _table.Columns.Insert(rowIndex, c);
            }

            MakeDirty();
            _dataGrid.Invalidate();

            return VSConstants.S_OK;
        }
      }
      else if (pguidCmdGroup == NpgsqlCommandHandler.guidDavinci)
      {
        switch ((VSConstants.VSStd97CmdID)nCmdID)
        {
          case VSConstants.VSStd97CmdID.ManageIndexes:
            EditorHolder holder = new EditorHolder(_table);
            _pg.SelectedObject = holder;
            _pg.SelectedGridItem = _pg.SelectedGridItem.Parent.GridItems[0];
            IndexEditor ed = new IndexEditor(_table);
            ed.EditValue((ITypeDescriptorContext)_pg.SelectedGridItem, (System.IServiceProvider)_pg.SelectedGridItem, _pg.SelectedGridItem.Value);

            _dataGrid_SelectionChanged(this, EventArgs.Empty);
            _dataGrid.Invalidate();

            return VSConstants.S_OK;
          case VSConstants.VSStd97CmdID.ManageRelationships:
            holder = new EditorHolder(_table);
            _pg.SelectedObject = holder;
            _pg.SelectedGridItem = _pg.SelectedGridItem.Parent.GridItems[1];
            ForeignKeyEditor fed = new ForeignKeyEditor(_table);
            fed.EditValue((ITypeDescriptorContext)_pg.SelectedGridItem, (System.IServiceProvider)_pg.SelectedGridItem, _pg.SelectedGridItem.Value);
            return VSConstants.S_OK;

          case VSConstants.VSStd97CmdID.ManageConstraints:
            holder = new EditorHolder(_table);
            _pg.SelectedObject = holder;
            _pg.SelectedGridItem = _pg.SelectedGridItem.Parent.GridItems[2];
            CheckEditor ced = new CheckEditor(_table);
            ced.EditValue((ITypeDescriptorContext)_pg.SelectedGridItem, (System.IServiceProvider)_pg.SelectedGridItem, _pg.SelectedGridItem.Value);
            return VSConstants.S_OK;

          case VSConstants.VSStd97CmdID.AlignRight: // Insert Column
            _dataGrid.EndEdit();
            _dataGrid.Rows.Insert(_dataGrid.SelectedRows[0].Index, 1);
            return VSConstants.S_OK;
          case VSConstants.VSStd97CmdID.AlignToGrid: // Delete Column
            _dataGrid.EndEdit();
            int deleted = 0;
            while (_dataGrid.SelectedRows.Count > 0)
            {
              try
              {
                DataGridViewRow row = _dataGrid.SelectedRows[0];
                Column c = row.Tag as Column;
                row.Selected = false;
                if (c != null)
                {
                  _table.Columns.Remove(c);
                  deleted++;
                }
                _dataGrid.Rows.Remove(row);
              }
              catch
              {
              }
            }
            if (deleted > 0) MakeDirty();
            return VSConstants.S_OK;
        }
      }
      else if (pguidCmdGroup == NpgsqlCommandHandler.guidSQLiteCmdSet)
      {
        switch ((cmdid)nCmdID)
        {
          case cmdid.Triggers:
            EditorHolder holder = new EditorHolder(_table);
            _pg.SelectedObject = holder;
            _pg.SelectedGridItem = _pg.SelectedGridItem.Parent.GridItems[3];
            TriggerEditor ted = new TriggerEditor(_table);
            ted.EditValue((ITypeDescriptorContext)_pg.SelectedGridItem, (System.IServiceProvider)_pg.SelectedGridItem, _pg.SelectedGridItem.Value);
            return VSConstants.S_OK;
        }
      }

      return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
    }

    private DataGridViewRow[] _clipRows;

    private DataGridViewRow[] GetClipboardRows()
    {
      if (Clipboard.ContainsData("SQLiteTableDesigner") && Clipboard.GetData("SQLiteTableDesigner").ToString() == Caption)
      {
        DataGridViewRow[] arr = _clipRows;
        if (arr != null)
        {
          for (int n = 0; n < arr.Length; n++)
          {
            Column c = arr[n].Tag as Column;
            if (c != null && c.Table == _table) return arr;
          }
        }
      }
      return null;
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
      {
        switch ((VSConstants.VSStd97CmdID)prgCmds[0].cmdID)
        {
          case VSConstants.VSStd97CmdID.PrimaryKey:
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
            if (IsPkSelected() == true)
              prgCmds[0].cmdf |= (uint)(OLECMDF.OLECMDF_LATCHED);

            break;
          case VSConstants.VSStd97CmdID.GenerateChangeScript:
            if (_dirty == true || String.IsNullOrEmpty(_table.OriginalSql) == false)
            {
              prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
              break;
            }
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
          case VSConstants.VSStd97CmdID.Cut:
            if (SelectedRows.Length > 0)
            {
              prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
              break;
            }
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
          case VSConstants.VSStd97CmdID.Copy:
            if (SelectedRows.Length > 0)
            {
              prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
              break;
            }
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
          case VSConstants.VSStd97CmdID.Paste:
            DataGridViewRow[] rows = GetClipboardRows();
            if (rows != null)
            {
              prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
              break;
            }
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
          default:
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
        }
        return VSConstants.S_OK;
      }

      if (pguidCmdGroup == NpgsqlCommandHandler.guidSQLiteCmdSet)
      {
        switch (prgCmds[0].cmdID)
        {
          case (uint)cmdid.Triggers:
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
            return VSConstants.S_OK;
        }
      }

      if (pguidCmdGroup == NpgsqlCommandHandler.guidDavinci)
      {
        switch (prgCmds[0].cmdID)
        {
          case (uint)VSConstants.VSStd97CmdID.ManageRelationships:
          case (uint)VSConstants.VSStd97CmdID.ManageIndexes:
          case (uint)VSConstants.VSStd97CmdID.ManageConstraints:
          //case 10: // Table View -> Custom
          //case 14: // Table View -> Modify Custom
          //case 33: // Database Diagram -> Add Table
          //case 1: // Database Diagram -> Add Related Tables
          //case 12: // Database Diagram -> Delete From Database
          //case 51: // Database Diagram -> Remove From Diagram
          //case 13: // Database Diagram -> Autosize Selected Tables
          //case 3: // Database Diagram -> Arrange Selection
          //case 2: // Database Diagram -> Arrange Tables
          //case 16: // Database Diagram -> Zoom -> 200%
          //case 17: // Database Diagram -> Zoom -> 150%
          //case 18: // Database Diagram -> Zoom -> 100%
          //case 19: // Database Diagram -> Zoom -> 75%
          //case 20: // Database Diagram -> Zoom -> 50%
          //case 21: // Database Diagram -> Zoom -> 25%
          //case 22: // Database Diagram -> Zoom -> 10%
          //case 24: // Database Diagram -> Zoom -> To Fit
          //case 6: // Database Diagram -> New Text Annotation
          //case 15: // Database Diagram -> Set Text Annotation Font
          //case 7: // Database Diagram -> Show Relationship Labels
          //case 8: // Database Diagram -> View Page Breaks
          //case 9: // Database Diagram -> Recalculate Page Breaks
          //case 43: // Database Diagram -> Copy Diagram to Clipboard
          //case 41: // Query Designer -> Table Display -> Column Names
          //case 42: // Query Designer -> Table Display -> Name Only
          //case 39: // Query Designer -> Add Table
          case 4: // Insert Column
          case 5: // Delete Column
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
            break;
          default:
            return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
        }
        return VSConstants.S_OK;
      }

      return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
    }

    #endregion

    #region ISelectionContainer Members

    int ISelectionContainer.CountObjects(uint dwFlags, out uint pc)
    {
      pc = 1;
      return VSConstants.S_OK;
    }

    int ISelectionContainer.GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
    {
      apUnkObjects[0] = _table;
      return VSConstants.S_OK;
    }

    int ISelectionContainer.SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
    {
      return VSConstants.S_OK;
    }

    #endregion

    #region IVsWindowPaneCommit Members

    int IVsWindowPaneCommit.CommitPendingEdit(out int pfCommitFailed)
    {
      pfCommitFailed = 0;
      return VSConstants.S_OK;
    }

    #endregion

    #region IVsWindowFrameNotify Members

    int IVsWindowFrameNotify.OnDockableChange(int fDockable)
    {
      return VSConstants.S_OK;
    }

    int IVsWindowFrameNotify.OnMove()
    {
      return VSConstants.S_OK;
    }

    int IVsWindowFrameNotify.OnShow(int fShow)
    {
      switch ((__FRAMESHOW)fShow)
      {
        case __FRAMESHOW.FRAMESHOW_WinShown:
        case __FRAMESHOW.FRAMESHOW_WinRestored:
          SetPropertyWindow();
          if (_warned == false)
          {
            _warned = true;
            MessageBox.Show(this, "The table designer is still in development.  Please report bugs to robert@blackcastlesoft.com", "Feature Under Review", MessageBoxButtons.OK);
          }
          break;
      }
      return VSConstants.S_OK;
    }

    int IVsWindowFrameNotify.OnSize()
    {
      return VSConstants.S_OK;
    }

    #endregion

    private void _dataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (e.ColumnIndex > -1)
        {
          _dataGrid.BeginEdit(true);
          _dataGrid_SelectionChanged(sender, e);
        }
      }
      catch
      {
      }
    }

    private void _dataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      _dataGrid.EndEdit();
      if (e.Button == MouseButtons.Right)
      {
        if (_dataGrid.Rows[e.RowIndex].Selected == false)
        {
          switch (Control.ModifierKeys)
          {
            case Keys.Control:
              _dataGrid.Rows[e.RowIndex].Selected = true;
              break;
            case Keys.Shift:
              int min = Math.Min(_dataGrid.CurrentRow.Index, e.RowIndex);
              int max = Math.Max(_dataGrid.CurrentRow.Index, e.RowIndex);
              for (int n = 0; n < _dataGrid.Rows.Count; n++)
              {
                _dataGrid.Rows[n].Selected = (_dataGrid.Rows[n].Index <= min || _dataGrid.Rows[n].Index <= max);
              }
              break;
            default:
              for (int n = 0; n < _dataGrid.Rows.Count; n++)
              {
                _dataGrid.Rows[n].Selected = (e.RowIndex == n);
              }
              break;
          }
        }

        IVsUIShell shell = _serviceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;
        if (shell != null)
        {
          Guid guid;
          POINTS[] p = new POINTS[1];
          int ret;

          p[0].x = (short)Control.MousePosition.X;
          p[0].y = (short)Control.MousePosition.Y;

          guid = new Guid("732abe74-cd80-11d0-a2db-00aa00a3efff");

          ret = shell.ShowContextMenu(0, ref guid, 259, p, this);
        }
      }
    }

    private void _dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (_init == true) return;
      if (e.ColumnIndex == -1 && e.RowIndex == -1)
      {
        _dataGrid.EndEdit();
      }
      _dataGrid_SelectionChanged(sender, e);
    }

    private void _dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (_init == true) return;
      if (e.ColumnIndex > -1 || e.RowIndex < 0) return;

      Column col = _dataGrid.Rows[e.RowIndex].Tag as Column;

      if (col == null) return;

      bool ispk = false;
      foreach (IndexColumn ic in _table.PrimaryKey.Columns)
      {
        if (String.Compare(ic.Column, col.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
        {
          ispk = true;
          break;
        }
      }
      if (ispk == true)
      {
        e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
        _imageList.Draw(e.Graphics, e.CellBounds.Left, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) - _imageList.ImageSize.Height) / 2, 0);
        e.Handled = true;
      }
    }

    private DataGridViewRow[] SelectedRows
    {
      get
      {
        List<DataGridViewRow> items = new List<DataGridViewRow>();

        for (int n = 0; n < _dataGrid.Rows.Count; n++)
        {
          if (_dataGrid.Rows[n].Selected || (_dataGrid.CurrentCell.RowIndex == n && _dataGrid.CurrentCell.Selected == true))
          {
            if (_dataGrid.Rows[n].Tag is Column)
              items.Add(_dataGrid.Rows[n]);
          }
        }

        DataGridViewRow[] objs = new DataGridViewRow[items.Count];
        items.CopyTo(objs);

        return objs;
      }
    }

    private void _dataGrid_SelectionChanged(object sender, EventArgs e)
    {
      if (_init == true) return;
      DataGridViewRow[] arr = SelectedRows;

      object[] objs = new object[arr.Length];

      for (int n = 0; n < objs.Length; n++)
        objs[n] = arr[n].Tag;

      _propertyGrid.SelectedObjects = objs;

      RefreshToolbars();
    }

    private void _dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (_init == true) return;
      if (e.RowIndex > -1)
      {
        _propertyGrid.SelectedObjects = _propertyGrid.SelectedObjects;
      }
    }

    private void _dataGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
    {
      if (_init == true) return;

      if (e.Row.Tag is Column)
        MakeDirty();

      _dataGrid_SelectionChanged(sender, e);
    }

    private void _dataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
      if (_init == true) return;
      if (e.Row.Tag is Column)
      {
        _table.Columns.Remove(e.Row.Tag as Column);
      }
    }

    private void _dataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {

    }

    private void _dataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {

    }

    private Rectangle _dragBoxFromMouseDown;
    private int _rowIndexFromMouseDown;
    private int _rowIndexOfItemUnderMouseToDrop;

    private void _dataGrid_DragDrop(object sender, DragEventArgs e)
    {
      // The mouse locations are relative to the screen, so they must be
      // converted to client coordinates.
      Point clientPoint = _dataGrid.PointToClient(new Point(e.X, e.Y));

      if (_rowIndexOfItemUnderMouseToDrop != -1)
        _dataGrid.Rows[_rowIndexOfItemUnderMouseToDrop].DividerHeight = 0;

      _rowIndexOfItemUnderMouseToDrop = _dataGrid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

      if (_rowIndexOfItemUnderMouseToDrop != -1)
        _dataGrid.Rows[_rowIndexOfItemUnderMouseToDrop].DividerHeight = 0;

      // If the drag operation was a move then remove and insert the row.
      if (e.Effect == DragDropEffects.Move)
      {
        DataGridViewRow[] rowsToMove = e.Data.GetData(typeof(DataGridViewRow[])) as DataGridViewRow[];
        for (int n = 0; n < rowsToMove.Length; n++)
        {
          _dataGrid.Rows.Remove(rowsToMove[n]);
          _table.Columns.Remove(rowsToMove[n].Tag as Column);
          _dataGrid.Rows.Insert(_rowIndexOfItemUnderMouseToDrop, rowsToMove[n]);
          _table.Columns.Insert(_rowIndexOfItemUnderMouseToDrop, rowsToMove[n].Tag as Column);
        }
        MakeDirty();
        RefreshToolbars();
      }
    }

    private void _dataGrid_DragOver(object sender, DragEventArgs e)
    {
      // The mouse locations are relative to the screen, so they must be
      // converted to client coordinates.
      Point clientPoint = _dataGrid.PointToClient(new Point(e.X, e.Y));

      // Get the row index of the item the mouse is below.
      if (_rowIndexOfItemUnderMouseToDrop != -1)
        _dataGrid.Rows[_rowIndexOfItemUnderMouseToDrop].DividerHeight = 0;

      _rowIndexOfItemUnderMouseToDrop = _dataGrid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

      if (_dataGrid.Rows[_rowIndexOfItemUnderMouseToDrop].IsNewRow == false && _rowIndexOfItemUnderMouseToDrop != _rowIndexFromMouseDown)
      {
        e.Effect = DragDropEffects.Move;

        if (_rowIndexOfItemUnderMouseToDrop != -1)
          _dataGrid.Rows[_rowIndexOfItemUnderMouseToDrop].DividerHeight = 3;
      }
      else
        e.Effect = DragDropEffects.None;
    }

    private void _dataGrid_MouseDown(object sender, MouseEventArgs e)
    {
      // Get the index of the item the mouse is below.
      _rowIndexFromMouseDown = _dataGrid.HitTest(e.X, e.Y).RowIndex;

      if (_rowIndexFromMouseDown != -1)
      {
        // Remember the point where the mouse down occurred.
        // The DragSize indicates the size that the mouse can move
        // before a drag event should be started.
        Size dragSize = SystemInformation.DragSize;

        // Create a rectangle using the DragSize, with the mouse position being
        // at the center of the rectangle.
        _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
      }
      else
        // Reset the rectangle if the mouse is not over an item in the ListBox.
        _dragBoxFromMouseDown = Rectangle.Empty;
    }

    private void _dataGrid_MouseMove(object sender, MouseEventArgs e)
    {
      if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
      {
        // If the mouse moves outside the rectangle, start the drag.
        if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
        {
          _rowIndexOfItemUnderMouseToDrop = -1;
          _dataGrid.EndEdit();
          // Proceed with the drag and drop, passing in the list item.
          DragDropEffects dropEffect = _dataGrid.DoDragDrop(SelectedRows, DragDropEffects.Move);
        }
      }
    }

    private void _dataGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
      DataGridViewRow row = _dataGrid.Rows[e.RowIndex];
      Column col = row.Tag as Column;

      if (col == null && row.IsNewRow == false)
      {
        col = new Column(_table, row);
        row.Tag = col;
        _table.Columns.Insert(row.Index, col);
        _dataGrid_SelectionChanged(this, new EventArgs());
      }
      if (col != null)
        col.CellValueChanged(e.RowIndex, e.ColumnIndex);

      RefreshToolbars();
    }
  }

  internal class EditorHolder
  {
    private List<Index> _indexes;
    private List<ForeignKey> _fkeys;
    private List<string> _check;
    private List<Trigger> _triggers;

    internal EditorHolder(Table tbl)
    {
      _indexes = tbl.Indexes;
      _fkeys = tbl.ForeignKeys;
      _check = tbl.Check;
      _triggers = tbl.Triggers as List<Trigger>;
    }

    public List<Index> Indexes
    {
      get { return _indexes; }
    }

    public List<ForeignKey> ForeignKeys
    {
      get { return _fkeys; }
    }

    public List<string> Check
    {
      get { return _check; }
    }

    public List<Trigger> Triggers
    {
      get { return _triggers; }
    }
  }

  public class DesignerDocBase : UserControl
  {
    public virtual string CanonicalName
    {
      get
      {
        return null;
      }
    }
  }

  internal class FakeHierarchy : IVsUIHierarchy, IVsPersistHierarchyItem2
  {
    DesignerDocBase _control;
    IVsUIHierarchy _owner;
    Dictionary<uint, IVsHierarchyEvents> _events = new Dictionary<uint, IVsHierarchyEvents>();

    internal FakeHierarchy(DesignerDocBase control, IVsUIHierarchy owner)
    {
      _control = control;
      _owner = owner;
    }

    #region IVsUIHierarchy Members

    int IVsUIHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      pdwCookie = 100;
      while (_events.ContainsKey(pdwCookie))
        pdwCookie++;

      _events[pdwCookie] = pEventSink;

      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.Close()
    {
      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      pbstrName = _control.CanonicalName;
      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      return _owner.GetGuidProperty(itemid, propid, out pguid);
    }

    int IVsUIHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      ppHierarchyNested = IntPtr.Zero;
      pitemidNested = 0;

      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      pvar = null;

      switch ((__VSHPROPID)propid)
      {
        case __VSHPROPID.VSHPROPID_AllowEditInRunMode:
          pvar = true;
          break;
        case __VSHPROPID.VSHPROPID_CanBuildFromMemory:
          pvar = true;
          break;
        case __VSHPROPID.VSHPROPID_Caption:
        case __VSHPROPID.VSHPROPID_SaveName:
          pvar = _control.CanonicalName;
          break;
        case __VSHPROPID.VSHPROPID_IsHiddenItem:
          pvar = true;
          break;
        case __VSHPROPID.VSHPROPID_IsNewUnsavedItem:
          pvar = true;
          break;
        case __VSHPROPID.VSHPROPID_ShowOnlyItemCaption:
          pvar = true;
          break;
        case __VSHPROPID.VSHPROPID_IconImgList:
          pvar = 0;
          break;
        case __VSHPROPID.VSHPROPID_IconHandle:
          pvar = null;
          return VSConstants.S_OK;
      }

      if (pvar == null)
        return _owner.GetProperty(itemid, propid, out pvar);
      else
        return VSConstants.S_OK;
    }

    int IVsUIHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
    {
      ppSP = null;
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      pitemid = 0;
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.QueryClose(out int pfCanClose)
    {
      pfCanClose = 1;
      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      foreach(IVsHierarchyEvents listener in _events.Values)
      {
        listener.OnPropertyChanged(itemid, propid, 0);
      }
      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      _events.Remove(dwCookie);
      return VSConstants.S_OK;
    }

    int IVsUIHierarchy.Unused0()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused1()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused2()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused3()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused4()
    {
      return VSConstants.E_NOTIMPL;
    }

    #endregion

    #region IVsHierarchy Members

    int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      return ((IVsUIHierarchy)this).AdviseHierarchyEvents(pEventSink, out pdwCookie);
    }

    int IVsHierarchy.Close()
    {
      return ((IVsUIHierarchy)this).Close();
    }

    int IVsHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      return ((IVsUIHierarchy)this).GetCanonicalName(itemid, out pbstrName);
    }

    int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      return ((IVsUIHierarchy)this).GetGuidProperty(itemid, propid, out pguid);
    }

    int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      return ((IVsUIHierarchy)this).GetNestedHierarchy(itemid, ref iidHierarchyNested, out ppHierarchyNested, out pitemidNested);
    }

    int IVsHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      return ((IVsUIHierarchy)this).GetProperty(itemid, propid, out pvar);
    }

    int IVsHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
    {
      ppSP = null;
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      pitemid = 0;
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.QueryClose(out int pfCanClose)
    {
      return ((IVsUIHierarchy)this).QueryClose(out pfCanClose);
    }

    int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return ((IVsUIHierarchy)this).SetGuidProperty(itemid, propid, ref rguid);
    }

    int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      return ((IVsUIHierarchy)this).SetProperty(itemid, propid, var);
    }

    int IVsHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      return ((IVsUIHierarchy)this).SetSite(psp);
    }

    int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      return ((IVsUIHierarchy)this).UnadviseHierarchyEvents(dwCookie);
    }

    int IVsHierarchy.Unused0()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused1()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused2()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused3()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused4()
    {
      return VSConstants.E_NOTIMPL;
    }

    #endregion

    #region IVsPersistHierarchyItem Members

    int IVsPersistHierarchyItem.IsItemDirty(uint itemid, IntPtr punkDocData, out int pfDirty)
    {
      return ((IVsPersistDocData)_control).IsDocDataDirty(out pfDirty);
    }

    int IVsPersistHierarchyItem.SaveItem(VSSAVEFLAGS dwSave, string pszSilentSaveAsName, uint itemid, IntPtr punkDocData, out int pfCanceled)
    {
      return ((IVsPersistDocData)_control).SaveDocData(dwSave, out pszSilentSaveAsName, out pfCanceled);
    }

    #endregion

    #region IVsPersistHierarchyItem2 Members

    int IVsPersistHierarchyItem2.IgnoreItemFileChanges(uint itemid, int fIgnore)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsPersistHierarchyItem2.IsItemDirty(uint itemid, IntPtr punkDocData, out int pfDirty)
    {
      return ((IVsPersistDocData)_control).IsDocDataDirty(out pfDirty);
    }

    int IVsPersistHierarchyItem2.IsItemReloadable(uint itemid, out int pfReloadable)
    {
      return ((IVsPersistDocData)_control).IsDocDataReloadable(out pfReloadable);
    }

    int IVsPersistHierarchyItem2.ReloadItem(uint itemid, uint dwReserved)
    {
      return ((IVsPersistDocData)_control).ReloadDocData(dwReserved);
    }

    int IVsPersistHierarchyItem2.SaveItem(VSSAVEFLAGS dwSave, string pszSilentSaveAsName, uint itemid, IntPtr punkDocData, out int pfCanceled)
    {
      return ((IVsPersistDocData)_control).SaveDocData(dwSave, out pszSilentSaveAsName, out pfCanceled);
    }

    #endregion
  }
}
