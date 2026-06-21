using System;
using System.Windows;
using System.Windows.Input;

namespace GameMacroManager.Views
{
    public partial class CoordinateCaptureOverlay : Window
    {
        public Point? CapturedPoint { get; private set; }
        public string? ClickType { get; private set; }

        public CoordinateCaptureOverlay()
        {
            InitializeComponent();
            this.PreviewMouseDown += OnPreviewMouseDown;
            this.PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Yakalanan koordinatlar ekran koordinatları olmalı (absolute)
            CapturedPoint = this.PointToScreen(e.GetPosition(this));
            
            if (e.ChangedButton == MouseButton.Left)
                ClickType = "MouseLeft";
            else if (e.ChangedButton == MouseButton.Right)
                ClickType = "MouseRight";
            else if (e.ChangedButton == MouseButton.Middle)
                ClickType = "MouseMiddle";
            
            e.Handled = true;
            this.DialogResult = true;
            this.Close();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
