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
  using System.Runtime.InteropServices;

  public partial class ViewDesignerDoc : DesignerDocBase,
    IVsPersistDocData,
    IVsWindowPane,
    IOleCommandTarget,
    ISelectionContainer,
    IVsWindowPaneCommit,
    IVsWindowFrameNotify
  {
    private static Dictionary<int, string> _editingTables = new Dictionary<int, string>();

    private bool _qdinit = false;
    private bool _init = false;
    internal DataConnection _connection;
    internal Microsoft.VisualStudio.Data.ServiceProvider _serviceProvider;
    internal bool _dirty = false;
    internal UserControl _queryDesigner;
    internal Type _typeQB;
    internal Npgsql.Designer.Design.View _view;
    internal IOleCommandTarget _qbole;
    internal IOleInPlaceActiveObject _qbbase;
    private IntPtr _qbsql;
    internal DataViewHierarchyAccessor _accessor;
    internal int _itemId;
    static private bool _warned = false;

    public delegate bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam);
    [DllImport("user32.Dll")]
    public static extern bool EnumChildWindows(IntPtr parentHandle, EnumWindowsCallback callback, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowTextLength(IntPtr hWnd);

    public ViewDesignerDoc(int itemId, DataViewHierarchyAccessor accessor, string viewName)
    {
      _accessor = accessor;
      _connection = accessor.Connection;
      _itemId = itemId;
      InitializeComponent();

      _init = true;
      try
      {
        _typeQB = NpgsqlDataAdapterToolboxItem._vsdesigner.GetType("Microsoft.VSDesigner.Data.Design.QueryBuilderControl");

        if (_typeQB != null)
        {
          _queryDesigner = Activator.CreateInstance(_typeQB) as UserControl;
          _typeQB.InvokeMember("Provider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, _queryDesigner, new object[] { "Npgsql" });
          _typeQB.InvokeMember("ConnectionString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, _queryDesigner, new object[] { _connection.ConnectionSupport.ConnectionString });
          _typeQB.InvokeMember("EnableMorphing", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, _queryDesigner, new object[] { false });
          Controls.Add(_queryDesigner);
          _queryDesigner.Dock = DockStyle.Fill;
          _queryDesigner.Visible = true;
        }

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

        if (String.IsNullOrEmpty(viewName))
        {
          string alltables = tables.ToString();

          do
          {
            viewName = String.Format("View{0}", n);
            n++;
          } while (alltables.IndexOf(viewName + ",", StringComparison.OrdinalIgnoreCase) > -1 || _editingTables.ContainsValue(viewName));

          _editingTables.Add(GetHashCode(), viewName);
        }
        _view = new Npgsql.Designer.Design.View(viewName, _connection.ConnectionSupport.ProviderObject as DbConnection, this);
      }
      finally
      {
        _init = false;
      }
    }

    private string GetRichText()
    {
      if (_qbsql != IntPtr.Zero)
      {
        int len = GetWindowTextLength(_qbsql);
        StringBuilder builder = new StringBuilder(len + 1);
        GetWindowText(_qbsql, builder, builder.Capacity);
        return builder.ToString();
      }
      return String.Empty;
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
        if (_view != null) catalog = _view.Catalog;

        return String.Format("{0}.{1} View (SQLite [{2}])", catalog, base.Name, ((DbConnection)_connection.ConnectionSupport.ProviderObject).DataSource);
      }
    }

    public override string CanonicalName
    {
      get
      {
        return _view.Name;
      }
    }
    public new string Name
    {
      get
      {
        if (_view != null)
          return _view.Name;
        else
        return base.Name;
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

    private void CommitQueryBuilder()
    {
      string query = _typeQB.InvokeMember("SqlText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty, null, _queryDesigner, null) as string;
      _view.SqlText = query;
    }

    public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = null; // _view.Name;
      pfSaveCanceled = 0;

      if (String.IsNullOrEmpty(_view.OriginalSql) == true)
      {
        using (TableNameDialog dlg = new TableNameDialog("View", _view.Name))
        {
          if (dlg.ShowDialog(this) == DialogResult.Cancel)
          {
            pfSaveCanceled = 1;
            return VSConstants.S_OK;
          }
          _view.Name = dlg.TableName;
        }
      }

      CommitQueryBuilder();

      string query = _view.GetSqlText();
      if (String.IsNullOrEmpty(query) == false)
      {
        using (DbTransaction trans = _view.GetConnection().BeginTransaction())
        {
          try
          {
            using (DbCommand cmd = _view.GetConnection().CreateCommand())
            {
              cmd.CommandText = query;
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
      }

      _dirty = false;
      _view.Committed();
      NotifyChanges();

      NpgsqlCommandHandler.Refresh(_accessor, _itemId);

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
      _serviceProvider = new MyServiceProvider(psp);
      System.Reflection.MethodInfo mi = _typeQB.GetMethod("Init", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[] { typeof(System.IServiceProvider), typeof(string) }, null);
      mi.Invoke(_queryDesigner, new object[] { _serviceProvider, _view.SqlText });

      _qbole = _typeQB.InvokeMember("OleCommandTarget", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty, null, _queryDesigner, null) as IOleCommandTarget;
      _qbbase = _typeQB.InvokeMember("OleInPlaceActiveObject", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty, null, _queryDesigner, null) as IOleInPlaceActiveObject;

      return VSConstants.S_OK;
    }

    private bool EnumWindows(IntPtr hwnd, IntPtr lparam)
    {
      if (_qbsql != IntPtr.Zero) return false;

      StringBuilder builder = new StringBuilder(256);
      GetClassName(hwnd, builder, builder.Capacity);
      if (String.Compare(builder.ToString(), "RichEdit20W", StringComparison.OrdinalIgnoreCase) == 0)
      {
        _qbsql = hwnd;
        return false;
      }

      EnumChildWindows(hwnd, new EnumWindowsCallback(EnumWindows), IntPtr.Zero);

      return true;
    }

    public int TranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
    {
      return VSConstants.S_FALSE;
    }

    #endregion

    public void MakeDirty()
    {
      if (_init == true) return;
      _dirty = true;
      NotifyChanges();
    }

    #region IOleCommandTarget Members

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (pguidCmdGroup == NpgsqlCommandHandler.guidSQLiteCmdSet)
      {
        switch ((cmdid)nCmdID)
        {
          case cmdid.Triggers:
            ViewHolder holder = new ViewHolder(_view);
            _pg.SelectedObject = holder;
            _pg.SelectedGridItem = _pg.SelectedGridItem.Parent.GridItems[0];
            CommitQueryBuilder();
            TriggerEditor ted = new TriggerEditor(_view);
            ted.EditValue((ITypeDescriptorContext)_pg.SelectedGridItem, (System.IServiceProvider)_pg.SelectedGridItem, _pg.SelectedGridItem.Value);
            return VSConstants.S_OK;
        }
      }

      if (_qbole != null) return _qbole.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

      return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      if (pguidCmdGroup == NpgsqlCommandHandler.guidSQLiteCmdSet)
      {
        switch (prgCmds[0].cmdID)
        {
          case (uint)cmdid.Triggers:
            prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
            return VSConstants.S_OK;
        }
      }

      int retval = (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);

      if (_qbole != null)
        retval = _qbole.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

      //if (pguidCmdGroup == SQLiteCommandHandler.guidQueryGroup)
      //{
        // 0x83 -- show SQL pane
        // 0x84 -- show diagram pane
        // 0x0f --
        // 0x10 --
        // 0x1a --
        // 0xc9 -- execute SQL
        // 0x86 -- show criteria pane
        // 0x85 -- show results pane
        // 0x76 --
        // 0x6b -- verify sql syntax
        // 0x79 -- add group by
      //}

      return retval;
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
      apUnkObjects[0] = _view;
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

    private void _timer_Tick(object sender, EventArgs e)
    {
      _timer.Enabled = false;
      if (_serviceProvider != null)
      {
        EnvDTE.DTE dte = (EnvDTE.DTE)_serviceProvider.GetService(typeof(EnvDTE.DTE));

        if (_qdinit == false)
        {
          _qdinit = true;
          _init = true;
          try
          {
            _typeQB.InvokeMember("PostInit", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, null, _queryDesigner, null);

            if (_qbbase != null)
            {
              EnumChildWindows(Handle, new EnumWindowsCallback(EnumWindows), IntPtr.Zero);
            }

            _check_Tick(this, EventArgs.Empty);

            _typeQB.InvokeMember("ShowAddTableDialogIfNeeded", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, null, _queryDesigner, null);
          }
          finally
          {
            _init = false;
            _check.Enabled = true;
          }
        }
      }
    }

    int IVsWindowFrameNotify.OnShow(int fShow)
    {
      switch ((__FRAMESHOW)fShow)
      {
        case __FRAMESHOW.FRAMESHOW_WinShown:
        case __FRAMESHOW.FRAMESHOW_WinRestored:
          _timer.Enabled = true;
          SetPropertyWindow();
          if (_warned == false)
          {
            _warned = true;
            MessageBox.Show(this, "The view designer is still in development.  Please report bugs to robert@blackcastlesoft.com", "Feature Under Review", MessageBoxButtons.OK);
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

    private void _check_Tick(object sender, EventArgs e)
    {
      string str = GetRichText();

      _view.SqlText = str;
    }
  }

  internal class MyServiceProvider : ServiceProvider
  {
    Microsoft.VisualStudio.OLE.Interop.IServiceProvider _psp;

    internal MyServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      _psp = psp;
    }

    protected override object GetServiceImpl(Guid serviceGuid)
    {
      IntPtr zero = IntPtr.Zero;
      Guid unk = new Guid("00000000-0000-0000-c000-000000000046");
      Guid self = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");

      if (serviceGuid == self) return this;

      int status = _psp.QueryService(ref serviceGuid, ref unk, out zero);
      if (status < 0)
      {
        if ((status == -2147467259) || (status == -2147467262))
        {
          return null;
        }
        Marshal.ThrowExceptionForHR(status);
      }
      object objectForIUnknown = Marshal.GetObjectForIUnknown(zero);
      Marshal.Release(zero);
      return objectForIUnknown;
    }

    protected override object GetServiceImpl(Type serviceType)
    {
      return this.GetService(serviceType.GUID);
    }
  }

  internal class ViewHolder
  {
    private List<ViewTrigger> _triggers;

    internal ViewHolder(Npgsql.Designer.Design.View parent)
    {
      _triggers = parent.Triggers as List<ViewTrigger>;
    }

    public List<ViewTrigger> Triggers
    {
      get { return _triggers; }
    }
  }
}
