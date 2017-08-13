//------------------------------------------------------------------------------
// <copyright file="ToDoListWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSToDoList.UI.MainWindow
{
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using VSToDoList.Controls;

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
            Assembly.Load("VSToDoList.Controls");
            this.InitializeComponent();
        }

        public ToDoListWindowViewModel ViewModel => (ToDoListWindowViewModel)Resources["ViewModel"];

        private void OnRemoveItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = (Control)sender;
            var buttonParent = (Grid)(button.Parent);
            var treeViewItem = (ContentPresenter)buttonParent.TemplatedParent;
            var task = treeViewItem.Content;

            ViewModel.RemoveTaskCommand.Execute(task);

        }

        /// <summary>
        /// Remove the ActiveItemSelection from the TreeView when clicking outside it
        /// </summary>
        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid)
            {
                Keyboard.ClearFocus();
            }
        }

        /// <summary>
        /// Disable the Expand/Collapse on double click
        /// </summary>
        private void TreeView_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}