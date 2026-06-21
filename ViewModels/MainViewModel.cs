using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameMacroManager.Models;
using GameMacroManager.Services;

namespace GameMacroManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly MacroService _macroService;
        private readonly HotkeyService _hotkeyService;
        private readonly PlaylistService _playlistService;

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<Game> _games = new();

        [ObservableProperty]
        private Game? _selectedGame;

        [ObservableProperty]
        private Character? _selectedCharacter;

        [ObservableProperty]
        private Combo? _selectedCombo;

        [ObservableProperty]
        private bool _isEnabled = true;

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private string _statusMessage = "Hazır";

        [ObservableProperty]
        private string _activeComboName = "";

        [ObservableProperty]
        private AppSettings _settings = new();

        [ObservableProperty]
        private ComboPlaylist? _currentPlaylist;

        [ObservableProperty]
        private bool _isPlaylistPlaying = false;

        [ObservableProperty]
        private string _playlistStatus = "Durduruldu";

        [ObservableProperty]
        private ObservableCollection<string> _runningProcesses = new();

        [ObservableProperty]
        private bool _isSidebarExpanded = true;

        #endregion

        public MainViewModel()
        {
            _dataService = new DataService();
            _macroService = new MacroService();
            _hotkeyService = new HotkeyService(_macroService);
            _playlistService = new PlaylistService(_macroService);

            // Event handlers
            _macroService.MacroStarted += OnMacroStarted;
            _macroService.MacroStopped += OnMacroStopped;
            _macroService.EnabledChanged += OnMacroEnabledChanged;
            
            _playlistService.PlaylistStarted += OnPlaylistStarted;
            _playlistService.PlaylistStopped += OnPlaylistStopped;
            _playlistService.ItemStarted += OnPlaylistItemStarted;
            
            _hotkeyService.PlaylistToggleRequested += OnPlaylistToggleRequested;
            _hotkeyService.GlobalToggleRequested += OnGlobalToggleRequested;

            _hotkeyService.CheckIsGameActive = IsTargetGameActive;
        }

        #region Initialization

        public async Task InitializeAsync()
        {
            IsLoading = true;
            StatusMessage = "Veriler yükleniyor...";

            try
            {
                Settings = await _dataService.LoadSettingsAsync();
                var games = await _dataService.LoadGamesAsync();

                Games = new ObservableCollection<Game>(games);

                // Son seçilen oyun ve karakteri yükle
                if (Settings.LastSelectedGameId.HasValue)
                {
                    SelectedGame = Games.FirstOrDefault(g => g.Id == Settings.LastSelectedGameId.Value);
                }

                if (SelectedGame == null && Games.Any())
                {
                    SelectedGame = Games.First();
                }

                if (Settings.LastSelectedCharacterId.HasValue && SelectedGame != null)
                {
                    SelectedCharacter = SelectedGame.Characters.FirstOrDefault(c => c.Id == Settings.LastSelectedCharacterId.Value);
                }

                if (SelectedCharacter == null && SelectedGame?.Characters.Any() == true)
                {
                    SelectedCharacter = SelectedGame.Characters.First();
                }

                // Son playlist'i yükle
                if (Settings.LastPlaylistId.HasValue && SelectedCharacter != null)
                {
                    CurrentPlaylist = SelectedCharacter.Playlists.FirstOrDefault(p => p.Id == Settings.LastPlaylistId.Value);
                    if (CurrentPlaylist != null)
                    {
                        // Combo referanslarını düzelt (JSON'dan yüklenirken kaybolabilir)
                        foreach (var item in CurrentPlaylist.Items)
                        {
                            if (item.Combo == null && item.ComboId.HasValue)
                            {
                                item.Combo = SelectedCharacter.Combos.FirstOrDefault(c => c.Id == item.ComboId.Value);
                            }
                        }
                        PlaylistStatus = $"'{CurrentPlaylist.Name}' yüklendi ({CurrentPlaylist.Items.Count} combo)";
                    }
                }

                // Hotkey service başlat
                _hotkeyService.Start();
                if (SelectedCharacter != null)
                {
                    _hotkeyService.RegisterCharacterCombos(SelectedCharacter);
                }

                StatusMessage = "Hazır";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Hata: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Property Changed Handlers

        partial void OnSelectedGameChanged(Game? value)
        {
            if (value != null)
            {
                Settings.LastSelectedGameId = value.Id;
                _ = _dataService.SaveSettingsAsync(Settings);

                if (value.Characters.Any())
                {
                    SelectedCharacter = value.Characters.First();
                }
                else
                {
                    SelectedCharacter = null;
                }
            }
        }

        partial void OnSelectedCharacterChanged(Character? value)
        {
            if (value != null)
            {
                Settings.LastSelectedCharacterId = value.Id;
                _ = _dataService.SaveSettingsAsync(Settings);

                _hotkeyService.RegisterCharacterCombos(value);
                StatusMessage = $"{value.Name} - {value.Combos.Count} combo aktif";
            }
            else
            {
                _hotkeyService.ClearAllBindings();
            }

            SelectedCombo = value?.Combos.FirstOrDefault();
        }

        partial void OnIsEnabledChanged(bool value)
        {
            _macroService.IsEnabled = value;
            _hotkeyService.IsEnabled = value;
            StatusMessage = value ? "Makrolar aktif" : "Makrolar devre dışı";
        }

        #endregion

        #region Game Commands

        [RelayCommand]
        private async Task AddGame()
        {
            var newGame = new Game
            {
                Name = "Yeni Oyun",
                Description = "Oyun açıklaması",
                ThemeColor = "#2196F3"
            };

            Games.Add(newGame);
            SelectedGame = newGame;
            await SaveDataAsync();
        }

        [RelayCommand]
        private async Task DeleteGame()
        {
            if (SelectedGame == null) return;

            var result = MessageBox.Show(
                $"'{SelectedGame.Name}' oyununu silmek istediğinize emin misiniz?",
                "Oyun Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Games.Remove(SelectedGame);
                SelectedGame = Games.FirstOrDefault();
                await SaveDataAsync();
            }
        }

        #endregion

        #region Character Commands

        [RelayCommand]
        private async Task AddCharacter()
        {
            if (SelectedGame == null) return;

            var newCharacter = new Character
            {
                Name = "Yeni Karakter",
                Description = "Karakter açıklaması",
                AccentColor = "#4CAF50"
            };

            SelectedGame.Characters.Add(newCharacter);
            SelectedCharacter = newCharacter;
            await SaveDataAsync();

            // UI güncellemesi için
            OnPropertyChanged(nameof(SelectedGame));
        }

        [RelayCommand]
        private async Task DeleteCharacter()
        {
            if (SelectedGame == null || SelectedCharacter == null) return;

            var result = MessageBox.Show(
                $"'{SelectedCharacter.Name}' karakterini silmek istediğinize emin misiniz?",
                "Karakter Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SelectedGame.Characters.Remove(SelectedCharacter);
                SelectedCharacter = SelectedGame.Characters.FirstOrDefault();
                await SaveDataAsync();
            }
        }

        #endregion

        #region Combo Commands

        [RelayCommand]
        private async Task AddCombo()
        {
            if (SelectedCharacter == null) return;

            var newCombo = new Combo
            {
                Name = "Yeni Combo",
                Description = "Combo açıklaması",
                Category = "Saldırı"
            };

            SelectedCharacter.Combos.Add(newCombo);
            SelectedCombo = newCombo;
            await SaveDataAsync();

            // Hotkey'leri güncelle
            _hotkeyService.RegisterCharacterCombos(SelectedCharacter);
            OnPropertyChanged(nameof(SelectedCharacter));
        }

        [RelayCommand]
        private async Task DeleteCombo()
        {
            if (SelectedCharacter == null || SelectedCombo == null) return;

            var result = MessageBox.Show(
                $"'{SelectedCombo.Name}' combosunu silmek istediğinize emin misiniz?",
                "Combo Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SelectedCharacter.Combos.Remove(SelectedCombo);
                SelectedCombo = SelectedCharacter.Combos.FirstOrDefault();
                await SaveDataAsync();

                _hotkeyService.RegisterCharacterCombos(SelectedCharacter);
            }
        }

        [RelayCommand]
        private async Task DuplicateCombo()
        {
            if (SelectedCharacter == null || SelectedCombo == null) return;

            var newCombo = SelectedCombo.Clone();
            SelectedCharacter.Combos.Add(newCombo);
            SelectedCombo = newCombo;
            await SaveDataAsync();

            OnPropertyChanged(nameof(SelectedCharacter));
        }

        [RelayCommand]
        private async Task AddKeyAction()
        {
            if (SelectedCombo == null) return;

            SelectedCombo.Actions.Add(new KeyAction
            {
                Key = "A",
                ActionType = KeyActionType.Tap,
                DelayAfterMs = Settings.DefaultKeyDelay,
                HoldDurationMs = Settings.DefaultHoldDuration
            });

            SelectedCombo.UpdatedAt = DateTime.Now;
            await SaveDataAsync();

            OnPropertyChanged(nameof(SelectedCombo));
        }

        [RelayCommand]
        private async Task RemoveKeyAction(KeyAction? action)
        {
            if (SelectedCombo == null || action == null) return;

            SelectedCombo.Actions.Remove(action);
            SelectedCombo.UpdatedAt = DateTime.Now;
            await SaveDataAsync();

            OnPropertyChanged(nameof(SelectedCombo));
        }

        public async Task AddCalibratedCombo(Combo newCombo)
        {
            if (SelectedCharacter == null) return;

            SelectedCharacter.Combos.Add(newCombo);
            SelectedCombo = newCombo;
            await SaveDataAsync();

            // Hotkey'leri güncelle
            _hotkeyService.RegisterCharacterCombos(SelectedCharacter);
            OnPropertyChanged(nameof(SelectedCharacter));
        }

        #endregion

        #region General Commands

        [RelayCommand]
        private void ToggleEnabled()
        {
            IsEnabled = !IsEnabled;
        }

        [RelayCommand]
        private void ToggleSidebar()
        {
            IsSidebarExpanded = !IsSidebarExpanded;
        }

        [RelayCommand]
        private async Task SaveData()
        {
            await SaveDataAsync();
        }

        [RelayCommand]
        private void OpenDataFolder()
        {
            _dataService.OpenDataFolder();
        }

        [RelayCommand]
        private async Task ExportProfile()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Dosyası (*.json)|*.json",
                FileName = "GameMacroManager_Backup.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _dataService.ExportDataAsync(saveFileDialog.FileName, Games.ToList());
                    StatusMessage = "Profil dışa aktarıldı.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task ImportProfile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Dosyası (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var result = MessageBox.Show(
                    "İçe aktarma işlemi mevcut tüm oyun ve karakterlerinizi silecek ve yerine aktarılan dosyayı yükleyecektir. Devam etmek istiyor musunuz?",
                    "İçe Aktarmayı Onayla",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var importedGames = await _dataService.ImportDataAsync(openFileDialog.FileName);
                        if (importedGames != null && importedGames.Any())
                        {
                            Games.Clear();
                            foreach (var game in importedGames)
                            {
                                Games.Add(game);
                            }
                            SelectedGame = Games.FirstOrDefault();
                            await SaveDataAsync();
                            StatusMessage = "Profil başarıyla içe aktarıldı.";
                        }
                        else
                        {
                            MessageBox.Show("Dosya boş veya geçersiz formatta.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"İçe aktarma hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        [RelayCommand]
        private void CaptureCoordinate(KeyAction? action)
        {
            if (action == null) return;

            var mainWindow = Application.Current.MainWindow;
            WindowState? previousState = mainWindow?.WindowState;

            if (mainWindow != null)
            {
                mainWindow.Visibility = Visibility.Collapsed;
            }

            // Ekranın temizlenmesi için kısa bir bekleme
            System.Threading.Thread.Sleep(200);

            var overlay = new Views.CoordinateCaptureOverlay();
            bool? result = overlay.ShowDialog();

            if (mainWindow != null)
            {
                mainWindow.Visibility = Visibility.Visible;
                if (previousState.HasValue)
                {
                    mainWindow.WindowState = previousState.Value;
                }
                mainWindow.Activate();
            }

            if (result == true && overlay.CapturedPoint.HasValue)
            {
                var point = overlay.CapturedPoint.Value;
                string clickType = overlay.ClickType ?? "MouseLeft";
                action.Key = $"{clickType}:{(int)point.X},{(int)point.Y}";
            }
        }

        [RelayCommand]
        private async Task TestCombo()
        {
            if (SelectedCombo == null) return;

            StatusMessage = $"Test: {SelectedCombo.Name}";

            // 2 saniye bekle ve combo'yu çalıştır
            await Task.Delay(2000);
            await _macroService.ExecuteComboAsync(SelectedCombo);

            StatusMessage = "Test tamamlandı";
        }

        #endregion

        #region Private Methods

        private async Task SaveDataAsync()
        {
            try
            {
                await _dataService.SaveGamesAsync(Games.ToList());
                StatusMessage = "Kaydedildi";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Kaydetme hatası: {ex.Message}";
            }
        }

        private void OnMacroStarted(object? sender, MacroEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ActiveComboName = e.Combo.Name;
                StatusMessage = $"▶ {e.Combo.Name}";
            });
        }

        private void OnMacroStopped(object? sender, MacroEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ActiveComboName = "";
                StatusMessage = "Hazır";
            });
        }

        private void OnMacroEnabledChanged(object? sender, bool enabled)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsEnabled = enabled;
            });
        }

        #endregion

        #region Playlist Commands

        [RelayCommand]
        private void CreatePlaylist()
        {
            if (SelectedCharacter == null)
            {
                MessageBox.Show("Önce bir karakter seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentPlaylist = new ComboPlaylist
            {
                Name = $"{SelectedCharacter.Name} Playlist"
            };
            
            // Karakterin playlist koleksiyonuna ekle
            SelectedCharacter.Playlists.Add(CurrentPlaylist);
            
            // Ayarlara kaydet
            Settings.LastPlaylistId = CurrentPlaylist.Id;
            _ = _dataService.SaveSettingsAsync(Settings);
            _ = _dataService.SaveGamesAsync(Games.ToList());
            
            PlaylistStatus = "Yeni playlist oluşturuldu";
        }

        [RelayCommand]
        private void AddToPlaylist(Combo? combo)
        {
            if (CurrentPlaylist == null)
            {
                CreatePlaylist();
            }


            if (combo != null && CurrentPlaylist != null)
            {
                CurrentPlaylist.Items.Add(new PlaylistItem
                {
                    Combo = combo,
                    ComboId = combo.Id, // JSON serialization için
                    DelayAfterMs = 2000 // Varsayılan 2 saniye
                });
                PlaylistStatus = $"'{combo.Name}' eklendi ({CurrentPlaylist.Items.Count} combo)";
                
                // Kaydet
                _ = _dataService.SaveGamesAsync(Games.ToList());
            }
        }

        [RelayCommand]
        private void RemoveFromPlaylist(PlaylistItem? item)
        {
            if (item != null && CurrentPlaylist != null)
            {
                CurrentPlaylist.Items.Remove(item);
                PlaylistStatus = $"Silindi ({CurrentPlaylist.Items.Count} combo kaldı)";
                
                // Kaydet
                _ = _dataService.SaveGamesAsync(Games.ToList());
            }
        }

        [RelayCommand]
        private async Task PlayPlaylistAsync()
        {
            if (CurrentPlaylist == null || !CurrentPlaylist.Items.Any())
            {
                MessageBox.Show("Playlist boş! Önce combo ekleyin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsPlaylistPlaying = true;
            PlaylistStatus = "Oynatılıyor...";

            try
            {
                await _playlistService.PlayAsync(CurrentPlaylist);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Playlist hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsPlaylistPlaying = false;
                PlaylistStatus = "Durduruldu";
            }
        }

        [RelayCommand]
        private void StopPlaylist()
        {
            _playlistService.Stop();
            IsPlaylistPlaying = false;
            PlaylistStatus = "Durduruldu";
        }

        [RelayCommand]
        private void ClearPlaylist()
        {
            if (CurrentPlaylist != null)
            {
                CurrentPlaylist.Items.Clear();
                PlaylistStatus = "Playlist temizlendi";
                
                // Kaydet
                _ = _dataService.SaveGamesAsync(Games.ToList());
            }
        }

        private void OnPlaylistStarted(object? sender, PlaylistEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsPlaylistPlaying = true;
                PlaylistStatus = $"▶ {e.Playlist.Name} başladı";
            });
        }

        private void OnPlaylistStopped(object? sender, PlaylistEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsPlaylistPlaying = false;
                PlaylistStatus = $"■ {e.Playlist.Name} durdu";
            });
        }

        private void OnPlaylistItemStarted(object? sender, PlaylistItemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaylistStatus = $"▶ {e.Item.DisplayName}";
            });
        }

        private void OnPlaylistToggleRequested(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (IsPlaylistPlaying)
                {
                    StopPlaylist();
                }
                else
                {
                    // Playlist boşsa veya içinde tanımlı ve etkin bir combo yoksa işlem yapma
                    if (CurrentPlaylist == null || !CurrentPlaylist.Items.Any(i => i.IsEnabled && i.Combo != null))
                    {
                        return;
                    }
                    await PlayPlaylistAsync();
                }
            });
        }

        private void OnGlobalToggleRequested(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsEnabled = !IsEnabled;
                StatusMessage = IsEnabled ? "Makrolar Aktif" : "Makrolar Devre Dışı";
            });
        }

        #endregion

        #region Win32 API for Active Window Filter

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetActiveProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return string.Empty;

            GetWindowThreadProcessId(hwnd, out uint pid);
            if (pid == 0) return string.Empty;

            try
            {
                using var process = System.Diagnostics.Process.GetProcessById((int)pid);
                return process.ProcessName;
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool IsTargetGameActive()
        {
            if (SelectedGame == null || string.IsNullOrWhiteSpace(SelectedGame.ExecutableName))
            {
                // Eğer tanımlı bir exe yoksa, tüm oyunlarda aktif varsayalım (eski davranış)
                return true;
            }

            string activeProcess = GetActiveProcessName();
            string target = SelectedGame.ExecutableName.Trim();

            // ".exe" uzantısını kaldırıp karşılaştır
            if (target.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(0, target.Length - 4);
            }

            return string.Equals(activeProcess, target, StringComparison.OrdinalIgnoreCase);
        }

        public void RefreshRunningProcesses()
        {
            try
            {
                var currentSelection = SelectedGame?.ExecutableName;

                RunningProcesses.Clear();
                RunningProcesses.Add(string.Empty); // Filtre yok (boş) seçeneği

                var processes = System.Diagnostics.Process.GetProcesses()
                    .Where(p => {
                        try { return !string.IsNullOrEmpty(p.MainWindowTitle); }
                        catch { return false; }
                    })
                    .Select(p => p.ProcessName + ".exe")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                foreach (var proc in processes)
                {
                    RunningProcesses.Add(proc);
                }

                // Eğer mevcut seçilen exe listede yoksa listede kaybolmasın diye ekleyelim
                if (!string.IsNullOrEmpty(currentSelection) && !RunningProcesses.Contains(currentSelection))
                {
                    RunningProcesses.Add(currentSelection);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Çalışan uygulamalar listelenirken hata: {ex.Message}");
            }
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            _hotkeyService.Stop();
            _hotkeyService.Dispose();
            _macroService.Dispose();
            _playlistService.Dispose();
        }

        #endregion
    }
}
