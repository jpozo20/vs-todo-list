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
    }
}