using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameMacroManager.Models;
using Newtonsoft.Json;

namespace GameMacroManager.Services
{
    /// <summary>
    /// Veri kaydetme ve yükleme servisi
    /// </summary>
    public class DataService
    {
        private readonly string _dataDirectory;
        private readonly string _gamesFile;
        private readonly string _settingsFile;

        public DataService()
        {
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GameMacroManager"
            );

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            _gamesFile = Path.Combine(_dataDirectory, "games.json");
            _settingsFile = Path.Combine(_dataDirectory, "settings.json");
        }

        #region Games

        public async Task<List<Game>> LoadGamesAsync()
        {
            try
            {
                if (!File.Exists(_gamesFile))
                {
                    // İlk çalıştırmada örnek veri oluştur
                    var sampleGames = CreateSampleData();
                    await SaveGamesAsync(sampleGames);
                    return sampleGames;
                }

                var json = await File.ReadAllTextAsync(_gamesFile);
                
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                
                var games = JsonConvert.DeserializeObject<List<Game>>(json, settings) ?? new List<Game>();
                
                System.Diagnostics.Debug.WriteLine($"Yüklenen oyun sayısı: {games.Count}");
                foreach (var game in games)
                {
                    System.Diagnostics.Debug.WriteLine($"  Oyun: {game.Name}, Karakter: {game.Characters?.Count ?? 0}");
                }
                
                return games;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Oyunlar yüklenirken hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                System.Windows.MessageBox.Show($"Oyunlar yüklenirken hata oluştu:\n{ex.Message}", "Hata", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return new List<Game>();
            }
        }

        public async Task SaveGamesAsync(List<Game> games)
        {
            try
            {
                var json = JsonConvert.SerializeObject(games, Formatting.Indented);
                await File.WriteAllTextAsync(_gamesFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oyunlar kaydedilirken hata: {ex.Message}");
            }
        }

        #endregion

        #region Settings

        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                if (!File.Exists(_settingsFile))
                {
                    var defaultSettings = new AppSettings();
                    await SaveSettingsAsync(defaultSettings);
                    return defaultSettings;
                }

                var json = await File.ReadAllTextAsync(_settingsFile);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ayarlar yüklenirken hata: {ex.Message}");
                return new AppSettings();
            }
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                await File.WriteAllTextAsync(_settingsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ayarlar kaydedilirken hata: {ex.Message}");
            }
        }

        #endregion

        #region Sample Data

        private List<Game> CreateSampleData()
        {
            var blankaActions1 = new ObservableCollection<KeyAction>
            {
                new KeyAction { Key = "Z", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "D", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "D", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "D", ActionType = KeyActionType.Tap, DelayAfterMs = 50 }
            };

            var blankaActions2 = new ObservableCollection<KeyAction>
            {
                new KeyAction { Key = "Z", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "S", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "S", ActionType = KeyActionType.Tap, DelayAfterMs = 50 },
                new KeyAction { Key = "S", ActionType = KeyActionType.Tap, DelayAfterMs = 50 }
            };

            var blankaCombo1 = new Combo
            {
                Name = "Blitz Yumruk",
                Description = "Hızlı yumruk kombosu",
                Hotkey = "N",
                Category = "Saldırı",
                RepeatWhileHeld = true,
                Actions = blankaActions1
            };

            var blankaCombo2 = new Combo
            {
                Name = "Elektrik Tekme",
                Description = "Elektrikli tekme serisi",
                Hotkey = "B",
                Category = "Saldırı",
                RepeatWhileHeld = true,
                Actions = blankaActions2
            };

            var blanka = new Character
            {
                Name = "Blanka",
                Description = "Elektrik saldırılarıyla tanınan Brezilyalı savaşçı",
                AccentColor = "#4CAF50",
                Combos = new ObservableCollection<Combo> { blankaCombo1, blankaCombo2 }
            };

            var ryuActions = new ObservableCollection<KeyAction>
            {
                new KeyAction { Key = "Down", ActionType = KeyActionType.Tap, DelayAfterMs = 30 },
                new KeyAction { Key = "Right", ActionType = KeyActionType.Tap, DelayAfterMs = 30 },
                new KeyAction { Key = "A", ActionType = KeyActionType.Tap, DelayAfterMs = 50 }
            };

            var ryuCombo = new Combo
            {
                Name = "Hadouken",
                Description = "Enerji topuyla saldırı",
                Hotkey = "H",
                Category = "Özel",
                RepeatWhileHeld = false,
                Actions = ryuActions
            };

            var ryu = new Character
            {
                Name = "Ryu",
                Description = "Hadouken ustası Japon dövüşçü",
                AccentColor = "#2196F3",
                Combos = new ObservableCollection<Combo> { ryuCombo }
            };

            var streetFighter = new Game
            {
                Name = "Street Fighter",
                Description = "Capcom'un efsanevi dövüş oyunu",
                ThemeColor = "#FF5722",
                Characters = new ObservableCollection<Character> { blanka, ryu }
            };

            var jin = new Character
            {
                Name = "Jin Kazama",
                Description = "Mishima ailesinin varisi",
                AccentColor = "#F44336",
                Combos = new ObservableCollection<Combo>()
            };

            var tekken = new Game
            {
                Name = "Tekken",
                Description = "Bandai Namco'nun 3D dövüş oyunu",
                ThemeColor = "#9C27B0",
                Characters = new ObservableCollection<Character> { jin }
            };

            return new List<Game> { streetFighter, tekken };
        }

        #endregion

        /// <summary>
        /// Veri klasörünü aç
        /// </summary>
        public void OpenDataFolder()
        {
            System.Diagnostics.Process.Start("explorer.exe", _dataDirectory);
        }

        /// <summary>
        /// Verileri dışa aktar
        /// </summary>
        public async Task ExportDataAsync(string filePath, List<Game> games)
        {
            var json = JsonConvert.SerializeObject(games, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Verileri içe aktar
        /// </summary>
        public async Task<List<Game>> ImportDataAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
        }
    }
}
