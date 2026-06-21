using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Combo playlist - Sıralı combo çalıştırma
    /// </summary>
    public partial class ComboPlaylist : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _name = "Yeni Playlist";

        [ObservableProperty]
        private ObservableCollection<PlaylistItem> _items = new();

        [ObservableProperty]
        private bool _isLooping = false;

        [ObservableProperty]
        private bool _isEnabled = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Playlist içindeki tek bir item
    /// </summary>
    public partial class PlaylistItem : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private Combo? _combo;

        // JSON serialization için Combo ID
        public Guid? ComboId { get; set; }

        [ObservableProperty]
        private int _delayAfterMs = 2000; // Combo'dan sonra bekleme süresi

        [ObservableProperty]
        private bool _isEnabled = true;

        public string DisplayName => Combo?.Name ?? "Seçilmedi";
    }
}
