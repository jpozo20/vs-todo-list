namespace VSToDoList.UI.MainWindow
{
    using GalaSoft.MvvmLight.Command;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using VSToDoList.BL.Helpers;
    using VSToDoList.Models;
    using VSToDoList.UI.MainWindow.ViewModels;

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

        #region Properties

        public ToDoListWindowViewModel ViewModel => (ToDoListWindowViewModel)Resources["ViewModel"];

        private RelayCommand _addTaskBelowItemCommand;

        public RelayCommand AddTaskBelowCommand
        {
            get
            {
                if (_addTaskBelowItemCommand == null)
                {
                    _addTaskBelowItemCommand = new RelayCommand(AddTaskBelowItem);
                }
                return _addTaskBelowItemCommand;
            }
        }

        private RelayCommand _addSubTaskBelowItemCommand;

        public RelayCommand AddSubTaskBelowCommand
        {
            get
            {
                if (_addSubTaskBelowItemCommand == null)
                {
                    _addSubTaskBelowItemCommand = new RelayCommand(AddSubTaskBelowItem);
                }
                return _addSubTaskBelowItemCommand;
            }
        }

        private RelayCommand _deleteTaskCommand;

        public RelayCommand DeleteTaskCommand
        {
            get
            {
                if (_deleteTaskCommand == null)
                {
                    _deleteTaskCommand = new RelayCommand(DeleteFocusedTask);
                }
                return _deleteTaskCommand;
            }
        }

        private RelayCommand _editTaskCommand;
        public RelayCommand EditTaskCommand
        {
            get
            {
                if (_editTaskCommand == null)
                {
                    _editTaskCommand = new RelayCommand(SetTaskInEditMode);
                }
                return _editTaskCommand;
            }
        }

        #endregion Properties

        #region Task Methods

        /// <summary>
        /// Adds a new task to the tasks list
        /// </summary>
        private void AddNewTaskToMainTaskList()
        {
            ViewModel.AddNewTaskCommand.Execute(null);

            var newTask = TasksTreeView.Items.GetItemAt(TasksTreeView.Items.Count - 1);
            var treeViewItem = (TreeViewItem)TasksTreeView.ItemContainerGenerator.ContainerFromItem(newTask);
            treeViewItem.IsSelected = true;
            treeViewItem.Loaded += OnTreeViewItemLoaded;
        }

        /// <summary>
        /// Deletes the currently selected task and all of it childrens
        /// </summary>
        private void DeleteFocusedTask()
        {
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is TextBox) return; //If the TextBox has the focus, do nothing

            TreeViewItem parentTreeViewItem = null;
            var treeViewItem = focusedElement as TreeViewItem;
            var isChild = TreeViewHelper.IsTreeViewItemAChild(treeViewItem, out parentTreeViewItem);
            var task = treeViewItem.Header as ITask;

            if (string.IsNullOrEmpty(task.Name))
            {
                ViewModel.RemoveTaskCommand.Execute(task);
                FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
            }
            else
            {
                var messageResult = MessageBox.Show("Are you sure you want to delete the task?", "To-Do List",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageResult == MessageBoxResult.Yes)
                {
                    ViewModel.RemoveTaskCommand.Execute(task);
                    FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
                }
            }
        }

        void FocusNextItemAfterDeletion(TreeViewItem parentTreeViewItem, bool isChild)
        {
            if (isChild)
            {
                if (!parentTreeViewItem.HasItems) return;
                var lastIndex = parentTreeViewItem.Items.Count - 1;
                var lastChild = parentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(lastIndex);
                if (lastChild == null) return;
                Keyboard.Focus((TreeViewItem)lastChild);
            }
            else
            {
                if (!TasksTreeView.HasItems) return;
                var lastIndex = TasksTreeView.Items.Count - 1;
                var lastChild = TasksTreeView.ItemContainerGenerator.ContainerFromIndex(lastIndex);
                if (lastChild == null) return;
                Keyboard.Focus((TreeViewItem)lastChild);
            }
        }

        #endregion Task Methods

        #region TreeView and TaskItem Events

        /// <summary>
        /// Event fired when the Add Task button is clicked, or the shorcut is used
        /// </summary>
        public void OnToolbarAddTaskButtonClicked()
        {
            AddNewTaskToMainTaskList();
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
                Keyboard.Focus(treeViewItem);
                TreeViewHelper.SetTaskItemInEditMode(treeViewItem);
                treeViewItem.Loaded -= OnTreeViewItemLoaded;
            }
        }

        /// <summary>
        /// Event raised when the Add Task button of a task is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemAddTaskButtonClicked(object sender, RoutedEventArgs e)
        {
            var senderControl = (Control)sender;

            //First, add the new Task to the View
            var task = TreeViewHelper.GetTaskFromTreeViewItemTaskButton(senderControl);
            ViewModel.AddNewTaskCommand.Execute(task);

            //Once the Task is added, get its TreeViewItem and add a Loaded EventHandler
            //So when the  TreeViewItem is rendered, focus it and set it in EditMode
            var parentTreeViewItem = TreeViewHelper.GetTreeViewItemFromTreeViewItemTaskButton(senderControl);
            FocusAndSetTreeViewItemInEditMode(parentTreeViewItem);
        }

        private void OnTreeViewItemDeleteTaskButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem parentTreeViewItem = null;
            var senderControl = sender as Control;
            var treeViewItem = TreeViewHelper.GetTreeViewItemFromTreeViewItemTaskButton(senderControl);
            var isChild = TreeViewHelper.IsTreeViewItemAChild(treeViewItem, out parentTreeViewItem);
            var task = treeViewItem.Header as ITask;

            if (string.IsNullOrEmpty(task.Name))
            {
                ViewModel.RemoveTaskCommand.Execute(task);
                FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
            }
            else
            {
                var messageResult = MessageBox.Show("Are you sure you want to delete the task?", "To-Do List",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageResult == MessageBoxResult.Yes)
                {
                    ViewModel.RemoveTaskCommand.Execute(task);
                    FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
                }
            }
        }

        private void FocusAndSetTreeViewItemInEditMode(TreeViewItem parentTreeViewItem)
        {
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

        /// <summary>
        /// Gets the current focused task and adds a task below it
        /// </summary>
        private void AddTaskBelowItem()
        {
            // First we find the AncestorLevel=3, if it's a Grid then the current item
            // is a subtask and need to find the parent task. Otherwise, add the task to
            // the main tasks list.

            TreeViewItem parentTreeViewItem;

            var focusedItem = Keyboard.FocusedElement;
            if (focusedItem is TextBox) return; //If the TextBox has the focus, do nothing

            var isChild = TreeViewHelper.IsTreeViewItemAChild(focusedItem, out parentTreeViewItem);
            if (!isChild)
            {
                AddNewTaskToMainTaskList();
            }
            else
            {
                var task = parentTreeViewItem.Header as ITask;
                if (task == null) return;

                ViewModel.AddNewTaskCommand.Execute(task);
                FocusAndSetTreeViewItemInEditMode(parentTreeViewItem);
            }
        }

        /// <summary>
        /// Gets the current focused task and adds a subtask below it
        /// </summary>
        private void AddSubTaskBelowItem()
        {
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is TextBox) return; //If the TextBox has the focus, do nothing

            var treeViewItem = focusedElement as TreeViewItem;
            var task = treeViewItem.Header as ITask;
            if (task == null) return;

            ViewModel.AddNewTaskCommand.Execute(task);
            FocusAndSetTreeViewItemInEditMode(treeViewItem);
        }

        private void SetTaskInEditMode()
        {
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is TextBox) return;

            TreeViewHelper.SetTaskItemInEditMode((TreeViewItem)focusedElement);
        }
        #endregion TreeView and TaskItem Events

        #region Window Events

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

        #endregion Window Events
    }
}