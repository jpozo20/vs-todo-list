using System.Collections.Generic;
using System.Linq;
using VSToDoList.Models;

namespace VSToDoList.BL.Helpers
{
    public class TaskHelper
    {
        /// <summary>
        /// Recursively search for a  Task's parent in a Tasks collection
        /// </summary>
        /// <param name="taskList">The task collection in which to find the task</param>
        /// <param name="taskToFindParentOf">The task to find the parent of</param>
        /// <returns>The parent <see cref="Task"/> of the child if found,else null</returns>
        public static ITask FindParentTask(ICollection<ITask> taskList, ITask taskToFindParentOf)
        {
            var parentTask = taskList.FirstOrDefault(x => x.SubTasks.Contains(taskToFindParentOf));
            if (parentTask == null)
            {
                foreach (var child in taskList)
                {
                    parentTask = FindParentTaskInChild(child, taskToFindParentOf);
                    if (parentTask != null) return parentTask;
                }
            }

            return parentTask;
        }

        /// <summary>
        /// Search for the parent of the given Task in a Task collection child
        /// </summary>
        /// <param name="child">A Task Collection child in which to find the parent</param>
        /// <param name="taskToFindParentOf">The task to find the parent of</param>
        /// <returns></returns>
        public static ITask FindParentTaskInChild(ITask child, ITask taskToFindParentOf)
        {
            var parentTask = child.SubTasks.FirstOrDefault(x => x.SubTasks.Contains(taskToFindParentOf));
            if (parentTask == null)
            {
                foreach (var childTask in child.SubTasks)
                {
                    parentTask = FindParentTaskInChild(childTask, taskToFindParentOf);
                    if (parentTask != null) return parentTask;
                }
            }

            return parentTask;
        }
    }
}