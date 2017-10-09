// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VSToDoList.Controls
{
    /// <summary>
    ///     EditBox is a custom cotrol that can switch between two modes:
    ///     editing and normal. When it is in editing mode, the content is
    ///     displayed in a TextBox that provides editing capbability. When
    ///     the EditBox is in normal, its content is displayed in a TextBlock
    ///     that is not editable.
    /// </summary>
    public class EditBox : Control
    {
        #region Static Constructor

        /// <summary>
        ///     Static constructor
        /// </summary>
        static EditBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditBox),
                new FrameworkPropertyMetadata(typeof(EditBox)));
        }

        #endregion Static Constructor

        #region Public Methods

        /// <summary>
        ///     Called when the tree for the EditBox has been generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var textBlock = GetTemplateChild("PART_TextBlockPart") as TextBlock;
            Debug.Assert(textBlock != null, "No TextBlock!");

            // Adorn the TextBlock with a TextBox
            _textBox = new TextBox();
            _adorner = new EditBoxAdorner(textBlock, _textBox);
            var layer = AdornerLayer.GetAdornerLayer(textBlock);
            layer.Add(_adorner);

            _textBox.KeyDown += OnTextBoxKeyDown;
            _textBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;

            //Receive notification of the event to handle the column resize.
            HookTemplateParentResizeEvent();

            //Capture the resize event to  handle TreeView resize cases.
            HookItemsControlEvents();
            _treeViewItem = GetDependencyObjectFromVisualTree(this, typeof(TreeViewItem)) as TreeViewItem;
            if (_treeViewItem != null) _treeViewItem.LostKeyboardFocus += OnTreeViewItemLostKeyboardFocus;

            Debug.Assert(_treeViewItem != null, "No ListViewItem found");
        }

        public void SetEditMode(bool isInEditMode)
        {
            IsInEditMode = isInEditMode;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        ///     If the ListViewItem that contains the EditBox is selected,
        ///     when the mouse pointer moves over the EditBox, the corresponding
        ///     MouseEnter event is the first of two events (MouseUp is the second)
        ///     that allow the EditBox to change to editing mode.
        /// </summary>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsInEditMode && IsItemSelected)
            {
                _canEdit = true;
            }
        }

        /// <summary>
        ///     If the MouseLeave event occurs for an EditBox control that
        ///     is in normal mode, the mode cannot be changed to editing mode
        ///     until a MouseEnter event followed by a MouseUp event occurs.
        /// </summary>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseWithinScope = false;
            _canEdit = false;
        }

        /// <summary>
        ///     An EditBox switches to editing mode when the MouseUp event occurs
        ///     for that EditBox and the following conditions are satisfied:
        ///     1. A MouseEnter event for the EditBox occurred before the
        ///     MouseUp event.
        ///     2. The mouse did not leave the EditBox between the
        ///     MouseEnter and MouseUp events.
        ///     3. The ListViewItem that contains the EditBox was selected
        ///     when the MouseEnter event occurred.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Right ||
                e.ChangedButton == MouseButton.Middle)
                return;

            if (!IsInEditMode)
            {
                if (!e.Handled && (_canEdit || _isMouseWithinScope))
                {
                    // Enable the EditMode only on DoubeClick or
                    // on Click after the item has been selected
                    if (_treeViewItem.IsKeyboardFocusWithin && _isSelectionActive)
                    {
                        IsInEditMode = true;
                        _isSelectionActive = false;
                    }
                }

                // The first MouseUp event selects the parent TreeViewItem,
                // then the second MouseUp event puts the EditBox in editing
                // mode
                if (IsItemSelected)
                {
                    _isMouseWithinScope = true;
                    _isSelectionActive = true;
                }
            }
        }

        #endregion Protected Methods

        #region Public Properties

        #region Value

        /// <summary>
        ///     ValueProperty DependencyProperty.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(EditBox),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the value of the EditBox
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion Value

        #region IsInEditMode

        /// <summary>
        ///     IsEditingProperty DependencyProperty
        /// </summary>
        public static DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
                "IsInEditMode",
                typeof(bool),
                typeof(EditBox),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        ///     Returns true if the EditBox control is in editing mode.
        /// </summary>
        public bool IsInEditMode
        {
            get { return (bool)GetValue(IsInEditModeProperty); }
            private set
            {
                SetValue(IsInEditModeProperty, value);
                _adorner.UpdateVisibilty(value);
            }
        }

        #endregion IsInEditMode

        #region IsParentSelected

        /// <summary>
        ///     Gets whether the ListViewItem that contains the
        ///     EditBox is selected.
        /// </summary>
        private bool IsItemSelected
        {
            get
            {
                if (_treeViewItem == null)
                    return false;
                return _treeViewItem.IsSelectionActive;
            }
        }

        #endregion IsParentSelected

        #region TextDecoration

        /// <summary>
        /// Dependency property for the <see cref="TextDecoration"/>
        /// Currently it is based on the status of the task.
        /// If the task is Done then <seealso cref="TextDecorationCollection"/> will be <seealso cref="TextDecorations.Strikethrough"/>
        /// Otherwise, the TextDecoration will be removed by setting it to null.
        /// </summary>
        public static DependencyProperty TextDecorationProperty =
                 DependencyProperty.Register(
                            "TextDecoration",
                            typeof(TextDecorationCollection),
                            typeof(EditBox),
                            new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsRender,
                                OnTextDecorationPropertyChanged,
                                OnCoerceTextDecorationValue
                                )
                     );

        /// <summary>
        /// Decoration for the Task name Textblock text. Currently it is based on the status of the task.
        /// If the task is Done then <seealso cref="TextDecorationCollection"/> will be <seealso cref="TextDecorations.Strikethrough"/>
        /// Otherwise, the TextDecoration will be removed by setting it to null.
        /// </summary>
        public TextDecorationCollection TextDecoration
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationProperty); }
            set { SetValue(TextDecorationProperty, value); }
        }

        /// <summary>
        /// Handler for the CoerceValue event. This is called whenever the TextDecoration is set.
        /// Here will be the logic to handle the TextDecoration value.
        /// </summary>
        /// <param name="sender">The sender of the coerce. Should be <see cref="EditBox"/>.</param>
        /// <param name="baseValue">The value set to coerce. This will fire whenever the value is set, not just when it changes.</param>
        /// <returns></returns>
        private static object OnCoerceTextDecorationValue(DependencyObject sender, object baseValue)
        {
            if (baseValue == null || !(baseValue is TextDecorationCollection)) return null;
            return baseValue as TextDecorationCollection;
        }

        /// <summary>
        /// Handle TextDecoration accordingly after coercing the value.
        /// </summary>
        /// <param name="sender">Should be the EditBox</param>
        /// <param name="eventArgs">Arguments of the event, namely Old an New values.</param>
        private static void OnTextDecorationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var editBox = sender as EditBox;
            if (editBox == null) return;
            if (editBox._adorner == null) return; //The adorner is null when the tasks are loaded from disk

            var textBlock = editBox._adorner.AdornedElement as TextBlock;
            if (textBlock == null) return;

            if (eventArgs.NewValue == null)
            {
                textBlock.TextDecorations = null;
            }
            else
            {
                textBlock.TextDecorations = (TextDecorationCollection)eventArgs.NewValue;
            }
        }

        #endregion TextDecoration

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        ///     When an EditBox is in editing mode, pressing the ENTER or F2
        ///     keys switches the EditBox to normal mode.
        /// </summary>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode && (e.Key == Key.Enter || e.Key == Key.F2))
            {
                IsInEditMode = false;
                _isSelectionActive = false;
                _canEdit = false;
            }
        }

        /// <summary>
        ///     If an EditBox loses focus while it is in editing mode,
        ///     the EditBox mode switches to normal mode.
        /// </summary>
        private void OnTextBoxLostKeyboardFocus(object sender,
            KeyboardFocusChangedEventArgs e)
        {
            IsInEditMode = false;
            _isSelectionActive = false;
        }

        private void OnTreeViewItemLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _isSelectionActive = false;
        }

        /// <summary>
        ///     Sets IsEditing to false when the ListViewItem that contains an
        ///     EditBox changes its size
        /// </summary>
        private void OnCouldSwitchToNormalMode(object sender,
            RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        /// <summary>
        ///     Walk the visual tree to find the ItemsControl and
        ///     hook its some events on it.
        /// </summary>
        private void HookItemsControlEvents()
        {
            _itemsControl = GetDependencyObjectFromVisualTree(this,
                typeof(ItemsControl)) as ItemsControl;
            if (_itemsControl != null)
            {
                //Handle the Resize/ScrollChange/MouseWheel
                //events to determine whether to switch to Normal mode
                _itemsControl.SizeChanged +=
                    OnCouldSwitchToNormalMode;
                _itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent,
                    new RoutedEventHandler(OnScrollViewerChanged));
                _itemsControl.AddHandler(MouseWheelEvent,
                    new RoutedEventHandler(OnCouldSwitchToNormalMode), true);
            }
        }

        /// <summary>
        ///     If an EditBox is in editing mode and the content of a TreeView is
        ///     scrolled, then the EditBox switches to normal mode.
        /// </summary>
        private void OnScrollViewerChanged(object sender, RoutedEventArgs args)
        {
            if (IsInEditMode && Mouse.PrimaryDevice.LeftButton ==
                MouseButtonState.Pressed)
            {
                IsInEditMode = false;
            }
        }

        /// <summary>
        ///     Walk visual tree to find the first DependencyObject
        ///     of the specific type.
        /// </summary>
        private DependencyObject
            GetDependencyObjectFromVisualTree(DependencyObject startObject,
                Type type)
        {
            //Walk the visual tree to get the parent(ItemsControl)
            //of this control
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        /// <summary>
        ///     When the size of the column containing the EditBox changes
        ///     and the EditBox is in editing mode, switch the mode to normal mode
        /// </summary>
        private void HookTemplateParentResizeEvent()
        {
            var parent = TemplatedParent as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged +=
                    OnCouldSwitchToNormalMode;
            }
        }

        #endregion Private Methods

        #region Private variables

        private EditBoxAdorner _adorner;

        //A TextBox in the visual tree
        private FrameworkElement _textBox;

        //Specifies whether an EditBox can switch to editing mode.
        //Set to true if the ListViewItem that contains the EditBox is
        //selected, when the mouse pointer moves over the EditBox
        private bool _canEdit;

        //Specifies whether an EditBox can switch to editing mode.
        //Set to true when the ListViewItem that contains the EditBox is
        //selected when the mouse pointer moves over the EditBox.
        private bool _isMouseWithinScope;

        //The TreeView control that contains the EditBox
        private ItemsControl _itemsControl;

        //The TreeViewItem control that contains the EditBox
        private TreeViewItem _treeViewItem;

        //A flag for correctly handling the TreeViewItem selection
        private bool _isSelectionActive;

        #endregion Private variables
    }
}