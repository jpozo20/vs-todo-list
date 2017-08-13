//------------------------------------------------------------------------------
// <copyright file="ToDoListWindowControl.xaml.cs"
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSToDoList.UI.MainWindow
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using VSToDoList.BL.Helpers;

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

        public void OnToolbarAddTaskButtonClicked()
        {
            ViewModel.AddNewTaskCommand.Execute(null);

            var newTask = TasksTreeView.Items.GetItemAt(TasksTreeView.Items.Count - 1);
            var treeViewItem = (TreeViewItem)TasksTreeView.ItemContainerGenerator.ContainerFromItem(newTask);
            treeViewItem.IsSelected = true;
            treeViewItem.Loaded += OnTreeViewItemLoaded;
        }

        /// <summary>
        /// Called when the new TreeViewItem has been rendered to the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var treeViewItem = e.Source as TreeViewItem;
                TreeViewHelper.SetTaskItemInEditMode(treeViewItem);
                treeViewItem.Loaded -= OnTreeViewItemLoaded;
            }
        }

        private void OnTreeViewItemAddTaskButtonClicked(object sender, RoutedEventArgs e)
        {
            var senderControl = (Control)sender;

            //First, add the new Task to the View
            var task = TreeViewHelper.GetTaskFromTreeViewItemTaskButton(senderControl);
            ViewModel.AddNewTaskCommand.Execute(task);

            //Once the Task is added, get its TreeViewItem and add a Loaded EventHandler
            //So when the  TreeViewItem is rendered, focus it and set it in EditMode
            var parentTreeViewItem = TreeViewHelper.GetTreeViewItemFromTreeViewItemTaskButton(senderControl);
            var newTask = parentTreeViewItem.Items.GetItemAt(parentTreeViewItem.Items.Count - 1);

            var newTreeViewItem = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(newTask);
            if (newTreeViewItem == null)
            {
                //When VirtualizingStackPanel.IsVirtualizing is True,
                //items are not rendered until they are visible
                //We need to expand the parent TreeViewItem and update its layout
                //so the ItemContainerGenerator can return the TreeViewItem of the new task
                parentTreeViewItem.IsExpanded = true;
                parentTreeViewItem.UpdateLayout();
                newTreeViewItem = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(newTask);
            }
            newTreeViewItem.Loaded += OnTreeViewItemLoaded;
        }

        private void OnRemoveItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var task = TreeViewHelper.GetTaskFromTreeViewItemTaskButton(sender as Control);
            ViewModel.RemoveTaskCommand.Execute(task);
        }

        /// <summary>
        /// Remove the ActiveItemSelection from the TreeView when clicking outside it
        /// </summary>
        private void OnGridMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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