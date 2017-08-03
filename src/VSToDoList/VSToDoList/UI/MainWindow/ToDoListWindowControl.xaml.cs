//------------------------------------------------------------------------------
// <copyright file="ToDoListWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSToDoList.UI.MainWindow
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToDoListWindowControl.
    /// </summary>
    public partial class ToDoListWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoListWindowControl"/> class.
        /// </summary>
        public ToDoListWindowControl()
        {
            this.InitializeComponent();
        }

        public ToDoListWindowViewModel ViewModel => (ToDoListWindowViewModel)Resources["ViewModel"];
    }
}