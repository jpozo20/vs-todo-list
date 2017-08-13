//------------------------------------------------------------------------------
// <copyright file="ToDoListWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSToDoList.UI.MainWindow
{
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("24a31f03-f6e5-4c62-ae53-687fb56d2c46")]
    public class ToDoListWindow : ToolWindowPane
    {
        //private ToDoListWindowControl _windowControl;
        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoListWindow"/> class.
        /// </summary>
        public ToDoListWindow() : base(null)
        {
            this.Caption = "ToDoListWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ToDoListWindowControl();
            this.ToolBar = new System.ComponentModel.Design.CommandID((ToDoListWindowCommand.CommandSet), ToDoListWindowCommand.ToolbarID);
            //this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }
}