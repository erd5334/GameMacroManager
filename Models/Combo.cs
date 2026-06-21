using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Bir kombo hareketini temsil eder (birden fazla tuş aksiyonu)
    /// </summary>
    public partial class Combo : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _hotkey = string.Empty;

        [ObservableProperty]
        private ObservableCollection<KeyAction> _actions = new();

        [ObservableProperty]
        private string _category = "Saldırı";

        [ObservableProperty]
        private bool _isEnabled = true;

        [ObservableProperty]
        private bool _repeatWhileHeld = true;

        [ObservableProperty]
        private int _repeatDelayMs = 100;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Komboyu görsel olarak temsil eden string
        /// </summary>
        public string GetVisualRepresentation()
        {
            if (Actions == null || !Actions.Any())
                return "(Boş)";

            return string.Join(" → ", Actions.Select(a => a.ToString()));
        }

        /// <summary>
        /// Tahmini çalışma süresi
        /// </summary>
        public int GetEstimatedDurationMs()
        {
            if (Actions == null || !Actions.Any())
                return 0;

            return Actions.Sum(a => a.DelayAfterMs + a.HoldDurationMs);
        }

        public Combo Clone()
        {
            var clone = new Combo
            {
                Id = Guid.NewGuid(),
                Name = Name + " (Kopya)",
                Description = Description,
                Hotkey = string.Empty,
                Category = Category,
                IsEnabled = IsEnabled,
                RepeatWhileHeld = RepeatWhileHeld,
                RepeatDelayMs = RepeatDelayMs,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            foreach (var action in Actions)
            {
                clone.Actions.Add(action.Clone());
            }
            
            return clone;
        }
    }
}
