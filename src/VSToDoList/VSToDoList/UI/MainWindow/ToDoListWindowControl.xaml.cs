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

        private void OnRemoveItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = (Control)sender;
            var buttonParent = (Grid)(button.Parent);
            var treeViewItem = (ContentPresenter)buttonParent.TemplatedParent;
            var task = treeViewItem.Content;

            ViewModel.RemoveTaskCommand.Execute(task);

        }
    }
}