using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VSToDoList.Controls;
using VSToDoList.Models;

namespace VSToDoList.BL.Helpers
{
    public static class TreeViewHelper
    {
        /// <summary>
        /// Get the <see cref="Task"/> from the TreeViewItem where
        /// the AddTask or RemoveTask button has been clicked
        /// </summary>
        /// <param name="taskButton">The TreeViewItem button that has been clicked</param>
        /// <returns>The Task associated to the button</returns>
        public static ITask GetTaskFromTreeViewItemTaskButton(Control taskButton)
        {
            Grid parentGrid = (Grid)(taskButton.Parent);
            ContentPresenter itemContentPresenter = (ContentPresenter)parentGrid.TemplatedParent;
            object task = itemContentPresenter.Content;

            return task as Task;
        }

        /// <summary>
        /// Get the <see cref="TreeViewItem"/> object from the TreeViewItem where
        /// the AddTask or RemoveTask button has been clicked
        /// </summary>
        /// <param name="taskButton">The TreeViewItem button that has been clicked</param>
        /// <returns>The TreeViewItem associated to the button</returns>
        public static TreeViewItem GetTreeViewItemFromTreeViewItemTaskButton(Control taskButton)
        {
            Grid parentGrid = (Grid)(taskButton.Parent);
            ContentPresenter itemContentPresenter = (ContentPresenter)parentGrid.TemplatedParent;
            TreeViewItem treeViewItem = itemContentPresenter.TemplatedParent as TreeViewItem;

            return treeViewItem;
        }

        /// <summary>
        /// Set a newly added Task in EditMode
        /// </summary>
        /// <param name="treeViewItem">The newly added TreeViewItem</param>
        public static void SetTaskItemInEditMode(TreeViewItem treeViewItem)
        {
            FrameworkElement contentPresenter = (FrameworkElement)treeViewItem.Template.FindName("PART_Header", treeViewItem);
            if (contentPresenter == null) return;

            Grid grid = (Grid)VisualTreeHelper.GetChild(contentPresenter, 0);
            EditBox editBox = (EditBox)grid.Children[1];
            editBox.SetEditMode(true);
        }

        /// <summary>
        /// Finds whether a <see cref="TreeViewItem"/> is a child element of another. Outputs the parent if true.
        /// </summary>
        /// <param name="inputElement">The element to find if is a child element or not</param>
        /// <param name="parentTreeViewItem">Outputs the parent element if the element if a child</param>
        /// <returns></returns>
        public static bool IsTreeViewItemAChild(IInputElement inputElement, out TreeViewItem parentTreeViewItem)
        {
            parentTreeViewItem = null;

            Control inputControl = inputElement as Control;
            if (inputControl == null) return false;

            FrameworkElement parentControl = VisualTreeHelper.GetParent(inputControl) as FrameworkElement;
            if (parentControl == null) return false;

            FrameworkElement grandParentControl = VisualTreeHelper.GetParent(parentControl) as FrameworkElement;
            if (grandParentControl == null) return false;

            FrameworkElement grandGrandParentControl = VisualTreeHelper.GetParent(grandParentControl) as FrameworkElement;
            if (grandGrandParentControl == null) return false;

            if (grandGrandParentControl is Grid)
            {
                parentTreeViewItem = VisualTreeHelper.GetParent(grandGrandParentControl) as TreeViewItem;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the ancestor of type T of the given <see cref="FrameworkElement"/>
        /// </summary>
        /// <typeparam name="T">The type of the ancestor to search for</typeparam>
        /// <param name="item">THe framework element whose ancestor you want to find</param>
        /// <returns>Returns an instance of the ancestor if found, null otherwise</returns>
        public static T FindAncestor<T>(FrameworkElement item) where T : class
        {
            if (item == null) return default(T);

            DependencyObject parent = VisualTreeHelper.GetParent(item);
            if (parent == null) return default(T);
            if (!(parent is T))
                parent = FindAncestor<T>((FrameworkElement)parent) as DependencyObject;

            return parent as T;
        }
    }
}