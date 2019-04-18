namespace VSToDoList.UI.MainWindow
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
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
            _dragTargetBackground = new SolidColorBrush(Color.FromArgb(180, 253, 153, 20));
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

        private Point _startPosition;
        private bool _isDragging;
        private object _currentTask;
        private SolidColorBrush _dragTargetBackground;

        #endregion Properties

        #region Task Methods

        /// <summary>
        /// Adds a new task to the tasks list
        /// </summary>
        private void AddNewTaskToMainTaskList()
        {
            ViewModel.AddNewTaskCommand.Execute(null);

            object newTask = TasksTreeView.Items.GetItemAt(TasksTreeView.Items.Count - 1);
            TreeViewItem treeViewItem = (TreeViewItem)TasksTreeView.ItemContainerGenerator.ContainerFromItem(newTask);
            treeViewItem.IsSelected = true;
            treeViewItem.Loaded += OnTreeViewItemLoaded;
        }

        /// <summary>
        /// Deletes the currently selected task and all of it childrens
        /// </summary>
        private void DeleteFocusedTask()
        {
            TreeViewItem treeViewItem = GetFocusedItem();
            if (treeViewItem == null) return;

            bool isChild = TreeViewHelper.IsTreeViewItemAChild(treeViewItem, out TreeViewItem parentTreeViewItem);
            ITask task = treeViewItem.Header as ITask;

            if (string.IsNullOrEmpty(task.Name))
            {
                ViewModel.RemoveTaskCommand.Execute(task);
                FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
            }
            else
            {
                MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the task?", "To-Do List",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageResult == MessageBoxResult.Yes)
                {
                    ViewModel.RemoveTaskCommand.Execute(task);
                    FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
                }
            }
        }

        private void FocusNextItemAfterDeletion(TreeViewItem parentTreeViewItem, bool isChild)
        {
            if (isChild)
            {
                if (!parentTreeViewItem.HasItems) return;
                int lastIndex = parentTreeViewItem.Items.Count - 1;
                DependencyObject lastChild = parentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(lastIndex);
                if (lastChild == null) return;
                Keyboard.Focus((TreeViewItem)lastChild);
            }
            else
            {
                if (!TasksTreeView.HasItems) return;
                int lastIndex = TasksTreeView.Items.Count - 1;
                DependencyObject lastChild = TasksTreeView.ItemContainerGenerator.ContainerFromIndex(lastIndex);
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
                TreeViewItem treeViewItem = e.Source as TreeViewItem;
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
            Control senderControl = (Control)sender;

            //First, add the new Task to the View
            ITask task = TreeViewHelper.GetTaskFromTreeViewItemTaskButton(senderControl);
            ViewModel.AddNewTaskCommand.Execute(task);

            //Once the Task is added, get its TreeViewItem and add a Loaded EventHandler
            //So when the  TreeViewItem is rendered, focus it and set it in EditMode
            TreeViewItem parentTreeViewItem = TreeViewHelper.GetTreeViewItemFromTreeViewItemTaskButton(senderControl);
            FocusAndSetTreeViewItemInEditMode(parentTreeViewItem);
        }

        private void OnTreeViewItemDeleteTaskButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Control senderControl = sender as Control;
            TreeViewItem treeViewItem = TreeViewHelper.GetTreeViewItemFromTreeViewItemTaskButton(senderControl);
            bool isChild = TreeViewHelper.IsTreeViewItemAChild(treeViewItem, out TreeViewItem parentTreeViewItem);
            ITask task = treeViewItem.Header as ITask;

            if (string.IsNullOrEmpty(task.Name))
            {
                ViewModel.RemoveTaskCommand.Execute(task);
                FocusNextItemAfterDeletion(parentTreeViewItem, isChild);
            }
            else
            {
                MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the task?", "To-Do List",
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
            object newTask = parentTreeViewItem.Items.GetItemAt(parentTreeViewItem.Items.Count - 1);

            TreeViewItem newTreeViewItem = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(newTask);
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

            IInputElement focusedItem = Keyboard.FocusedElement;
            if (focusedItem is TextBox) return; //If the TextBox has the focus, do nothing

            bool isChild = TreeViewHelper.IsTreeViewItemAChild(focusedItem, out TreeViewItem parentTreeViewItem);
            if (!isChild)
            {
                AddNewTaskToMainTaskList();
            }
            else
            {
                ITask task = parentTreeViewItem.Header as ITask;
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
            TreeViewItem treeViewItem = GetFocusedItem();
            if (treeViewItem == null) return;

            ITask task = treeViewItem.Header as ITask;
            if (task == null) return;

            ViewModel.AddNewTaskCommand.Execute(task);
            FocusAndSetTreeViewItemInEditMode(treeViewItem);
        }

        private void SetTaskInEditMode()
        {
            IInputElement focusedElement = Keyboard.FocusedElement;
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

        private void TaskBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPosition = e.GetPosition(null);
        }

        private void TaskBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                Point currentPosition = e.GetPosition(null);
                if (Math.Abs(currentPosition.X - _startPosition.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(currentPosition.Y - _startPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag();
                }
            }
        }

        private void StartDrag()
        {
            FrameworkElement dragScope = (TasksTreeView.Parent as FrameworkElement);
            dragScope.AllowDrop = true;
            dragScope.PreviewDragOver += DragScope_PreviewDragOver;
            dragScope.DragLeave += DragScope_PreviewDragLeave;
            dragScope.Drop += DragScope_Drop;
            _isDragging = true;

            _currentTask = TasksTreeView.SelectedItem;
            DataObject dataObject = new DataObject(DataFormats.Serializable, _currentTask);
            DragDropEffects dropEffects = DragDrop.DoDragDrop(this.TasksTreeView, dataObject, DragDropEffects.Move);

            dragScope.PreviewDragOver -= DragScope_PreviewDragOver;
            dragScope.PreviewDragLeave -= DragScope_PreviewDragLeave;
            dragScope.Drop -= DragScope_Drop;
            _isDragging = false;
        }

        private void DragScope_Drop(object sender, DragEventArgs e)
        {
            // If the Task item is present, then move the item in tasklist
            // otherwise cancel the drag
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                object targetTask = (e.OriginalSource as FrameworkElement).DataContext;
                object data = e.Data.GetData(DataFormats.Serializable);
                ViewModel.MoveTask((ITask)_currentTask, (ITask)targetTask);
                e.Effects = DragDropEffects.Move;

                UpdateTargetBackground(e.OriginalSource, Brushes.Transparent);
                TreeViewItem treeViewItem = TreeViewHelper.FindAncestor<TreeViewItem>(e.OriginalSource as FrameworkElement);
                if (treeViewItem != null) treeViewItem.IsSelected = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void DragScope_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource is Grid)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                UpdateTargetBackground(e.OriginalSource, Brushes.Transparent);
            }

            e.Handled = true;
        }

        private void DragScope_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Only allow the drag if it's into a different Task item
            if (e.OriginalSource is TextBlock)
            {
                if (e.OriginalSource is FrameworkElement fe)
                {
                    if (fe.DataContext is Task && (fe.DataContext != _currentTask))
                    {
                        UpdateTargetBackground(e.OriginalSource, _dragTargetBackground);
                        e.Effects = DragDropEffects.Move;
                        e.Handled = true;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the target Task item background during a drag n drop operation.
        /// </summary>
        /// <param name="control">The target Task item</param>
        /// <param name="color">The color to apply</param>
        private void UpdateTargetBackground(object control, Brush color)
        {
            FrameworkElement element = control as FrameworkElement;
            FrameworkElement ancestor = TreeViewHelper.FindAncestor<Grid>(element);
            if (ancestor == null) return;
            ((Grid)ancestor).Background = color;
        }

        /// <summary>
        /// Gets the currently Keyboard focused element
        /// </summary>
        /// <returns></returns>
        private TreeViewItem GetFocusedItem()
        {
            IInputElement focusedElement = Keyboard.FocusedElement;
            if (focusedElement is TextBox) return null; //If the TextBox has the focus, do nothing

            return focusedElement as TreeViewItem;
        }
    }
}