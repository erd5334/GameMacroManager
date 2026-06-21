using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Oyun karakterini temsil eder
    /// </summary>
    public partial class Character : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _iconPath = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Combo> _combos = new();

        [ObservableProperty]
        private ObservableCollection<ComboPlaylist> _playlists = new();

        [ObservableProperty]
        private bool _isEnabled = true;

        [ObservableProperty]
        private string _accentColor = "#4CAF50";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
