using System.Collections.Generic;

namespace GameMacroManager.Helpers
{
    /// <summary>
    /// Kullanılabilir tuş listesi
    /// </summary>
    public static class KeyList
    {
        /// <summary>
        /// Tüm desteklenen tuşlar
        /// </summary>
        public static List<string> AllKeys { get; } = new List<string>
        {
            // Harfler
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            
            // Sayılar
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9",
            
            // Numpad
            "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4",
            "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9",
            "Multiply", "Add", "Subtract", "Decimal", "Divide",
            
            // Fonksiyon tuşları
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
            
            // Yön tuşları
            "Left", "Right", "Up", "Down",
            
            // Kontrol tuşları
            "Space", "Enter", "Tab", "Escape", "Back", "Delete", "Insert",
            "Home", "End", "PageUp", "PageDown",
            
            // Modifier tuşları
            "LeftShift", "RightShift", "LeftCtrl", "RightCtrl",
            "LeftAlt", "RightAlt", "LWin", "RWin",
            
            // Özel tuşlar
            "CapsLock", "NumLock", "Scroll", "PrintScreen", "Pause",
            
            // Noktalama
            "OemComma", "OemPeriod", "OemMinus", "OemPlus",
            "OemOpenBrackets", "OemCloseBrackets", "OemPipe", "OemQuotes",
            "OemSemicolon", "OemQuestion", "OemTilde",

            // Fare Tuşları
            "MouseLeft", "MouseRight", "MouseMiddle"
        };

        /// <summary>
        /// Yaygın kullanılan tuşlar (kısa liste)
        /// </summary>
        public static List<string> CommonKeys { get; } = new List<string>
        {
            // Harfler
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            
            // Sayılar
            "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D0",
            
            // Yön tuşları
            "Left", "Right", "Up", "Down",
            
            // Sık kullanılan
            "Space", "Enter", "Tab", "Escape", "Back",
            "LeftShift", "LeftCtrl", "LeftAlt",

            // Fare Tuşları
            "MouseLeft", "MouseRight", "MouseMiddle"
        };

        /// <summary>
        /// Hotkey için uygun tuşlar (Genişletilmiş liste)
        /// </summary>
        public static List<string> HotkeyKeys { get; } = new List<string>
        {
            // Harfler (Tam liste)
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            
            // Sayılar
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9",
            
            // Fonksiyon tuşları
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
            
            // Numpad
            "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4",
            "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9",
            
            // Yön tuşları
            "Left", "Right", "Up", "Down",

            // Kontrol tuşları
            "Space", "Enter", "Tab", "Escape", "CapsLock",

            // Modifier tuşları
            "LeftShift", "RightShift", "LeftCtrl", "RightCtrl",
            "LeftAlt", "RightAlt"
        };
    }
}
