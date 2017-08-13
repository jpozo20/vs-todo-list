// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

// EventArgs
// Debug
// UIElement, Size
// TextBox, Canvas
// Binding, BindingMode, UpdateSourceTrigger
// Adorner

// Visual, VisualCollection

namespace VSToDoList.Controls
{
    /// <summary>
    ///     An adorner class that contains a TextBox to provide editing capability
    ///     for an EditBox control. The editable TextBox resides in the
    ///     AdornerLayer. When the EditBox is in editing mode, the TextBox is given a size
    ///     it with desired size; otherwise, arrange it with size(0,0,0,0).
    /// </summary>
    internal sealed class EditBoxAdorner : Adorner
    {
        /// <summary>
        ///     Inialize the EditBoxAdorner.
        /// </summary>
        public EditBoxAdorner(UIElement adornedElement, UIElement adorningElement) : base(adornedElement)
        {
            var textBox = adorningElement as TextBox;
            if (textBox == null) throw new ArgumentException("adorningElement is not a TextBox.");
            _textBox = textBox;

            _visualChildren = new VisualCollection(this);

            BuildTextBox();
        }

        #region Public Methods

        /// <summary>
        ///     Specifies whether a TextBox is visible
        ///     when the IsEditing property changes.
        /// </summary>
        /// <param name="isVisible"></param>
        public void UpdateVisibilty(bool isVisible)
        {
            _isVisible = isVisible;
            InvalidateMeasure();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        ///     Override to measure elements.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            _textBox.IsEnabled = _isVisible;
            //if in editing mode, measure the space the adorner element
            //should cover.
            if (_isVisible)
            {
                AdornedElement.Measure(constraint);
                _textBox.Measure(constraint);

                // Gets the ActualWidth of the EditBox so the TextBox
                // doesn't cover further than the TextBlock and the buttons
                // remain visible
                var adornedElement = this.AdornedElement as TextBlock;
                var editBox = adornedElement.TemplatedParent as Control;
                var actualWdith = editBox.ActualWidth;

                return new Size(actualWdith, _textBox.DesiredSize.Height);
            }
            return new Size(0, 0);
        }

        /// <summary>
        ///     override function to arrange elements.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isVisible)
            {
                _textBox.Arrange(new Rect(0, 0, finalSize.Width,
                    finalSize.Height));
            }
            else // if is not is editable mode, no need to show elements.
            {
                _textBox.Arrange(new Rect(0, 0, 0, 0));
            }
            return finalSize;
        }

        /// <summary>
        ///     override property to return infomation about visual tree.
        /// </summary>
        protected override int VisualChildrenCount => _visualChildren.Count;

        /// <summary>
        ///     override function to return infomation about visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        ///     Inialize necessary properties and hook necessary events on TextBox,
        ///     then add it into tree.
        /// </summary>
        private void BuildTextBox()
        {
            //_textBox.Background = EnvironmentColors
            _canvas = new Canvas();
            _canvas.Children.Add(_textBox);
            _visualChildren.Add(_canvas);

            //Bind Text onto AdornedElement.
            var binding = new Binding("Text")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = AdornedElement
            };

            _textBox.SetBinding(TextBox.TextProperty, binding);

            // when layout finishes.
            _textBox.LayoutUpdated += OnTextBoxLayoutUpdated;
        }

        /// <summary>
        ///     When Layout finish, if in editable mode, update focus status
        ///     on TextBox.
        /// </summary>
        private void OnTextBoxLayoutUpdated(object sender, EventArgs e)
        {
            if (_isVisible) _textBox.Focus();
        }

        #endregion Private Methods

        #region Private Variables

        // Visual children
        private readonly VisualCollection _visualChildren;

        // The TextBox that this adorner covers.
        private readonly TextBox _textBox;

        // Whether the EditBox is in editing mode which means the Adorner is visible.
        private bool _isVisible;

        // Canvas that contains the TextBox that provides the ability for it to
        // display larger than the current size of the cell so that the entire
        // contents of the cell can be edited
        private Canvas _canvas;

        // Extra padding for the content when it is displayed in the TextBox
        private const double ExtraWidth = 15;

        private const double ExcessWidth = 45;

        #endregion Private Variables
    }
}