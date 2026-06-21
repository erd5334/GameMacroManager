using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Oyunu temsil eder
    /// </summary>
    public partial class Game : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _iconPath = string.Empty;

        [ObservableProperty]
        private string _bannerPath = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Character> _characters = new();

        [ObservableProperty]
        private bool _isEnabled = true;

        [ObservableProperty]
        private string _themeColor = "#2196F3";

        [ObservableProperty]
        private string _executableName = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
