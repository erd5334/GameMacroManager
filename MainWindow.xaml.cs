using System;
using System.Windows;
using GameMacroManager.ViewModels;

namespace GameMacroManager
{
    public partial class MainWindow : Window
    {
        private MainViewModel? _viewModel;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _viewModel = new MainViewModel();
                DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Pencere başlatılırken hata:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "Başlangıç Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel != null)
                {
                    await _viewModel.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken hata oluştu:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _viewModel?.Cleanup();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        private void TimingCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null) return;
                var calibrationWindow = new Views.TimingCalibrationWindow(_viewModel);
                calibrationWindow.Owner = this;
                calibrationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Timing Calibration penceresi açılırken hata oluştu:\n{ex.Message}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GameProcessComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RefreshRunningProcesses();
            }
        }
    }
}