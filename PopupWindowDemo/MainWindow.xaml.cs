using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PopupWindowDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        PopupWindow _popup;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _popup = new PopupWindow(this);
            _popup.Text = "Content text";
            _popup.BackgroundColor = Brushes.DimGray;
            _popup.ForegroundColor = Brushes.LightGray;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _popup.Show(Mouse.GetPosition(this));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _popup.Close();
        }
    }
}
