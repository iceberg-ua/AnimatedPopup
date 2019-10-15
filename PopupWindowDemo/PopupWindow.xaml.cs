using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PopupWindowDemo
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow(Window owner)
            : this()
        {
            Owner = owner;
            Owner.SizeChanged += OwnerSizeChanged;
            Owner.LocationChanged += OwnerLocationChanged;
        }

        public PopupWindow()
        {
            InitializeComponent();

            _story.Children.Add(_anime);
            _story.FillBehavior = FillBehavior.Stop;
            _story.Completed += StoryCompleted;

            Storyboard.SetTargetName(_anime, Holder.Name);
            Storyboard.SetTargetProperty(_anime, new PropertyPath(OpacityProperty));

            IsVisibleChanged += PopupIsVisibleChanged;
        }

        #region Instance members

        Point _location;

        #endregion

        #region Animation members

        readonly Storyboard _story = new Storyboard();
        readonly DoubleAnimation _anime = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = new Duration(new TimeSpan(0, 0, 1)),
            BeginTime = new TimeSpan(0, 0, 1),
        };

        #endregion

        #region Properties

        public string Text
        {
            get => (string)ContentLabel.Content;
            set
            {
                ResetAnimation();
                ContentLabel.Content = value;
                StartAnimation();
            }
        }

        public Brush BackgroundColor
        {
            get => Holder.Background;
            set => Holder.Background = value;
        }

        public Brush ForegroundColor
        {
            get => ContentLabel.Foreground;
            set => ContentLabel.Foreground = value;
        }

        #endregion

        #region Disabling focusing

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr shadingHandle = helper.Handle;

            uint style = (uint)GetWindowLong(shadingHandle, GWL_STYLE);
            style |= WS_CHILD;
            SetWindowLong(shadingHandle, GWL_STYLE, (int)style);
        }

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion

        #region Animation events

        private void StoryCompleted(object sender, EventArgs e)
        {
            Hide();
            ResetAnimation();
        }

        #endregion

        #region Window events

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            UpdateLocation();
        }

        private void PopupIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            StartAnimation();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ResetAnimation();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            StartAnimation();
        }

        #endregion

        #region OwnerEvents

        private void OwnerLocationChanged(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        private void OwnerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLocation();
        }

        #endregion

        #region Helpers

        private void StartAnimation()
        {
            if (IsVisible)
                _story.Begin(this, true);
        }

        private void ResetAnimation()
        {
            _story.Stop(this);
            Opacity = 1d;
        }

        private void UpdateLocation()
        {
            var screenLocation = ((Grid)Owner.Content).PointToScreen(_location);

            var width = Width > 0 ? Width : 0;
            var height = Height > 0 ? Height : 0;

            Left = Math.Min(screenLocation.X, Owner.Left + ((Grid)Owner.Content).ActualWidth - width);
            Top = Math.Min(screenLocation.Y, Owner.Top + ((Grid)Owner.Content).ActualHeight - height);
        }

        #endregion

        #region Public methods

        public void Show(Point location)
        {
            _location = location;

            ResetAnimation();
            UpdateLocation();
            Show();
        }

        #endregion
    }
}
