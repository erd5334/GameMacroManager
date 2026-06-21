using System;

namespace GameMacroManager.Models
{
    /// <summary>
    /// Uygulama ayarları
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Son seçilen oyun ID'si
        /// </summary>
        public Guid? LastSelectedGameId { get; set; }

        /// <summary>
        /// Son seçilen karakter ID'si
        /// </summary>
        public Guid? LastSelectedCharacterId { get; set; }

        /// <summary>
        /// Son seçilen playlist ID'si
        /// </summary>
        public Guid? LastPlaylistId { get; set; }

        /// <summary>
        /// Uygulama başlangıçta minimize başlasın mı?
        /// </summary>
        public bool StartMinimized { get; set; } = false;

        /// <summary>
        /// Windows ile birlikte başlasın mı?
        /// </summary>
        public bool StartWithWindows { get; set; } = false;

        /// <summary>
        /// Küçültüldüğünde sistem tepsisine git
        /// </summary>
        public bool MinimizeToTray { get; set; } = true;

        /// <summary>
        /// Global hotkey'leri etkinleştir
        /// </summary>
        public bool EnableGlobalHotkeys { get; set; } = true;

        /// <summary>
        /// Karanlık tema
        /// </summary>
        public bool DarkMode { get; set; } = true;

        /// <summary>
        /// Bildirim sesleri
        /// </summary>
        public bool EnableSounds { get; set; } = true;

        /// <summary>
        /// Aktivasyon/deaktivasyon hotkey'i
        /// </summary>
        public string ToggleHotkey { get; set; } = "F9";

        /// <summary>
        /// Tüm makroları durdurma hotkey'i
        /// </summary>
        public string PanicHotkey { get; set; } = "F10";

        /// <summary>
        /// Varsayılan tuş gecikmesi (ms)
        /// </summary>
        public int DefaultKeyDelay { get; set; } = 50;

        /// <summary>
        /// Varsayılan tuş basılı tutma süresi (ms)
        /// </summary>
        public int DefaultHoldDuration { get; set; } = 30;
    }
}
