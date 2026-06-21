using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Tek bir tuş aksiyonunu temsil eder
    /// </summary>
    public partial class KeyAction : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _key = string.Empty;

        [ObservableProperty]
        [property: JsonConverter(typeof(StringEnumConverter))]
        private KeyActionType _actionType = KeyActionType.Tap;

        [ObservableProperty]
        private int _delayAfterMs = 50;

        [ObservableProperty]
        private int _holdDurationMs = 30;

        public KeyAction Clone()
        {
            return new KeyAction
            {
                Id = Guid.NewGuid(),
                Key = Key,
                ActionType = ActionType,
                DelayAfterMs = DelayAfterMs,
                HoldDurationMs = HoldDurationMs
            };
        }

        public override string ToString()
        {
            return ActionType switch
            {
                KeyActionType.Press => $"↓{Key}",
                KeyActionType.Release => $"↑{Key}",
                KeyActionType.Tap => $"[{Key}]",
                _ => Key
            };
        }
    }

    public enum KeyActionType
    {
        /// <summary>
        /// Tuşa basılı tut
        /// </summary>
        Press,

        /// <summary>
        /// Tuşu bırak
        /// </summary>
        Release,

        /// <summary>
        /// Bas ve bırak
        /// </summary>
        Tap
    }
}
