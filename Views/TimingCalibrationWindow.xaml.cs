using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GameMacroManager.ViewModels;

namespace GameMacroManager.Views
{
    public partial class TimingCalibrationWindow : Window
    {
        private ObservableCollection<KeyPressRecord> _keyPressRecords = new ObservableCollection<KeyPressRecord>();
        private Stopwatch _stopwatch = new Stopwatch();
        private long _lastKeyUpTime = 0;
        private long _currentKeyDownTime = 0;
        private Key? _currentKey = null;
        private bool _isRecording = false;
        private MainViewModel? _viewModel;

        public TimingCalibrationWindow()
        {
            InitializeComponent();
            KeyPressListBox.ItemsSource = _keyPressRecords;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        public TimingCalibrationWindow(MainViewModel viewModel) : this()
        {
            _viewModel = viewModel;
        }

        private void StartRecordButton_Click(object sender, RoutedEventArgs e)
        {
            StartRecording();
        }

        private void StartRecording()
        {
            _isRecording = true;
            _stopwatch.Restart();
            _lastKeyUpTime = 0;
            _currentKeyDownTime = 0;
            _currentKey = null;

            StartRecordButton.IsEnabled = false;
            StopRecordButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
            CopyButton.IsEnabled = false;

            this.Focus();
        }

        private void StopRecordButton_Click(object sender, RoutedEventArgs e)
        {
            StopRecording();
        }

        private void StopRecording()
        {
            _isRecording = false;
            _stopwatch.Stop();

            StartRecordButton.IsEnabled = true;
            StopRecordButton.IsEnabled = false;
            
            if (_keyPressRecords.Count > 0)
            {
                SaveButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
            }

            UpdateStatistics();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isRecording)
                return;

            // Ignore modifier keys and repeat key presses
            if (IsModifierKey(e.Key) || e.IsRepeat)
                return;

            // Record key down time
            _currentKeyDownTime = _stopwatch.ElapsedMilliseconds;
            _currentKey = e.Key;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!_isRecording || _currentKey == null)
                return;

            // Ignore modifier keys
            if (IsModifierKey(e.Key))
                return;

            // Only process if this is the same key that was pressed
            if (e.Key != _currentKey)
                return;

            long currentTime = _stopwatch.ElapsedMilliseconds;

            // Calculate hold duration
            long holdDuration = currentTime - _currentKeyDownTime;

            // Calculate delay (time since last key release)
            long delay = _lastKeyUpTime > 0 ? _currentKeyDownTime - _lastKeyUpTime : 0;

            // Add record
            var record = new KeyPressRecord
            {
                Key = e.Key,
                HoldDurationMs = holdDuration,
                DelayMs = delay,
                Timestamp = currentTime
            };

            _keyPressRecords.Add(record);

            // Update last key up time
            _lastKeyUpTime = currentTime;
            _currentKey = null;

            // Update statistics
            UpdateStatistics();

            // Auto-scroll to bottom
            if (KeyPressListBox.Items.Count > 0)
            {
                var border = System.Windows.Media.VisualTreeHelper.GetParent(KeyPressListBox) as System.Windows.Controls.Border;
                var scrollViewer = border?.Parent as System.Windows.Controls.ScrollViewer;
                scrollViewer?.ScrollToEnd();
            }
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LWin || key == Key.RWin ||
                   key == Key.System;
        }

        private void UpdateStatistics()
        {
            TotalKeysText.Text = _keyPressRecords.Count.ToString();

            if (_keyPressRecords.Count > 0)
            {
                double avgHold = _keyPressRecords.Average(r => r.HoldDurationMs);
                AvgHoldText.Text = $"{avgHold:F0} ms";

                var recordsWithDelay = _keyPressRecords.Where(r => r.DelayMs > 0).ToList();
                if (recordsWithDelay.Count > 0)
                {
                    double avgDelay = recordsWithDelay.Average(r => r.DelayMs);
                    AvgDelayText.Text = $"{avgDelay:F0} ms";
                }
                else
                {
                    AvgDelayText.Text = "0 ms";
                }
            }
            else
            {
                AvgHoldText.Text = "0 ms";
                AvgDelayText.Text = "0 ms";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _keyPressRecords.Clear();
            _lastKeyUpTime = 0;
            _currentKeyDownTime = 0;
            _currentKey = null;
            UpdateStatistics();
            SaveButton.IsEnabled = false;
            CopyButton.IsEnabled = false;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                MessageBox.Show("Uygulama modeli yüklenemedi.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_viewModel.SelectedCharacter == null)
            {
                MessageBox.Show("Önce ana ekrandan bir karakter seçmelisiniz!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string comboName = NameTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(comboName))
            {
                MessageBox.Show("Lütfen komut için bir ad girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCombo = new Models.Combo
            {
                Name = comboName,
                Description = "Zamanlama kalibrasyonundan kaydedildi."
            };

            foreach (var record in _keyPressRecords)
            {
                newCombo.Actions.Add(new Models.KeyAction
                {
                    Key = ConvertKeyToGameKey(record.Key),
                    ActionType = Models.KeyActionType.Tap,
                    DelayAfterMs = (int)record.DelayMs,
                    HoldDurationMs = (int)record.HoldDurationMs
                });
            }

            await _viewModel.AddCalibratedCombo(newCombo);
            MessageBox.Show($"'{newCombo.Name}' başarıyla kaydedildi! ✅", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_keyPressRecords.Count == 0)
                return;

            var json = GenerateJSON();
            Clipboard.SetText(json);
            MessageBox.Show("JSON panoya kopyalandı! ✅", "Başarılı", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string GenerateJSON()
        {
            var actions = new System.Text.StringBuilder();
            actions.AppendLine("[");

            for (int i = 0; i < _keyPressRecords.Count; i++)
            {
                var record = _keyPressRecords[i];
                var keyName = ConvertKeyToGameKey(record.Key);

                actions.AppendLine("    {");
                actions.AppendLine($"        \"Id\": \"{Guid.NewGuid()}\",");
                actions.AppendLine($"        \"Key\": \"{keyName}\",");
                actions.AppendLine($"        \"ActionType\": \"Tap\",");
                actions.AppendLine($"        \"DelayAfterMs\": {record.DelayMs},");
                actions.AppendLine($"        \"HoldDurationMs\": {record.HoldDurationMs}");
                actions.Append("    }");
                
                if (i < _keyPressRecords.Count - 1)
                    actions.AppendLine(",");
                else
                    actions.AppendLine();
            }

            actions.AppendLine("]");
            return actions.ToString();
        }

        private string ConvertKeyToGameKey(Key key)
        {
            // Convert WPF Key to game key name
            return key switch
            {
                Key.Up => "Up",
                Key.Down => "Down",
                Key.Left => "Left",
                Key.Right => "Right",
                Key.Space => "Space",
                _ => key.ToString()
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                StopRecording();
            }
            this.Close();
        }
    }

    public class KeyPressRecord
    {
        public Key Key { get; set; }
        public long HoldDurationMs { get; set; }
        public long DelayMs { get; set; }
        public long Timestamp { get; set; }

        public string Icon
        {
            get
            {
                return Key switch
                {
                    Key.Up => "⬆️",
                    Key.Down => "⬇️",
                    Key.Left => "⬅️",
                    Key.Right => "➡️",
                    Key.Space => "␣",
                    _ => "⌨️"
                };
            }
        }

        public string DisplayText
        {
            get
            {
                string keyName = Key switch
                {
                    Key.Up => "Up",
                    Key.Down => "Down",
                    Key.Left => "Left",
                    Key.Right => "Right",
                    Key.Space => "Space",
                    _ => Key.ToString()
                };
                return keyName;
            }
        }

        public string TimingText
        {
            get
            {
                if (DelayMs > 0)
                    return $"⏱️ {HoldDurationMs} ms basılı | ⏳ {DelayMs} ms gecikme";
                else
                    return $"⏱️ {HoldDurationMs} ms basılı";
            }
        }
    }
}
