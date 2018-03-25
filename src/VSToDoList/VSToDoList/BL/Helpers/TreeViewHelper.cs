using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VSToDoList.Controls;
using VSToDoList.Models;

namespace VSToDoList.BL.Helpers
{
    public class TreeViewHelper
    {
        /// <summary>
        /// Get the <see cref="Task"/> from the TreeViewItem where
        /// the AddTask or RemoveTask button has been clicked
        /// </summary>
        /// <param name="taskButton">The TreeViewItem button that has been clicked</param>
        /// <returns>The Task associated to the button</returns>
        public static ITask GetTaskFromTreeViewItemTaskButton(Control taskButton)
        {
            var parentGrid = (Grid)(taskButton.Parent);
            var itemContentPresenter = (ContentPresenter)parentGrid.TemplatedParent;
            var task = itemContentPresenter.Content;

            return task as Task;
        }

        // <summary>
        /// Get the <see cref="TreeViewItem"/> object from the TreeViewItem where
        /// the AddTask or RemoveTask button has been clicked
        /// </summary>
        /// <param name="taskButton">The TreeViewItem button that has been clicked</param>
        /// <returns>The TreeViewItem associated to the button</returns>
        public static TreeViewItem GetTreeViewItemFromTreeViewItemTaskButton(Control taskButton)
        {
            var parentGrid = (Grid)(taskButton.Parent);
            var itemContentPresenter = (ContentPresenter)parentGrid.TemplatedParent;
            var treeViewItem = itemContentPresenter.TemplatedParent as TreeViewItem;

            return treeViewItem;
        }

        /// <summary>
        /// Set a newly added Task in EditMode
        /// </summary>
        /// <param name="treeViewItem">The newly added TreeViewItem</param>
        public static void SetTaskItemInEditMode(TreeViewItem treeViewItem)
        {
            var contentPresenter = (FrameworkElement)treeViewItem.Template.FindName("PART_Header", treeViewItem);
            if (contentPresenter == null) return;

            var grid = (Grid)VisualTreeHelper.GetChild(contentPresenter, 0);
            var editBox = (EditBox)grid.Children[1];
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

            var inputControl = inputElement as Control;
            if (inputControl == null) return false;

            var parentControl = VisualTreeHelper.GetParent(inputControl) as FrameworkElement;
            if (parentControl == null) return false;

            var grandParentControl = VisualTreeHelper.GetParent(parentControl) as FrameworkElement;
            if (grandParentControl == null) return false;

            var grandGrandParentControl = VisualTreeHelper.GetParent(grandParentControl) as FrameworkElement;
            if (grandGrandParentControl == null) return false;

            if (grandGrandParentControl is Grid)
            {
                parentTreeViewItem = VisualTreeHelper.GetParent(grandGrandParentControl) as TreeViewItem;
                return true;
            }

            return false;
        }
    }
}