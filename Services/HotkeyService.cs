using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GameMacroManager.Models;

namespace GameMacroManager.Services
{
    /// <summary>
    /// Global hotkey dinleme servisi (Low-level keyboard hook)
    /// </summary>
    public class HotkeyService : IDisposable
    {
        #region Win32 API

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        #endregion

        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;
        private readonly MacroService _macroService;
        private readonly Dictionary<string, Combo> _hotkeyBindings;
        private readonly HashSet<string> _pressedKeys;

        public event EventHandler<KeyEventArgs>? KeyPressed;
        public event EventHandler<KeyEventArgs>? KeyReleased;
        public event EventHandler? PlaylistToggleRequested;
        public event EventHandler? GlobalToggleRequested;

        public bool IsEnabled { get; set; } = true;
        public Func<bool>? CheckIsGameActive { get; set; }

        public HotkeyService(MacroService macroService)
        {
            _macroService = macroService;
            _hotkeyBindings = new Dictionary<string, Combo>(StringComparer.OrdinalIgnoreCase);
            _pressedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _proc = HookCallback;
        }

        /// <summary>
        /// Hook'u başlat
        /// </summary>
        public void Start()
        {
            if (_hookId != IntPtr.Zero)
                return;

            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule!;
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);

            if (_hookId == IntPtr.Zero)
            {
                throw new InvalidOperationException("Keyboard hook kurulamadı!");
            }
        }

        /// <summary>
        /// Hook'u durdur
        /// </summary>
        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Hotkey binding ekle
        /// </summary>
        public void RegisterHotkey(string hotkey, Combo combo)
        {
            _hotkeyBindings[hotkey] = combo;
        }

        /// <summary>
        /// Hotkey binding kaldır
        /// </summary>
        public void UnregisterHotkey(string hotkey)
        {
            _hotkeyBindings.Remove(hotkey);
        }

        /// <summary>
        /// Tüm binding'leri temizle
        /// </summary>
        public void ClearAllBindings()
        {
            _hotkeyBindings.Clear();
        }

        /// <summary>
        /// Karakterin tüm combo'larını kaydet
        /// </summary>
        public void RegisterCharacterCombos(Character character)
        {
            ClearAllBindings();

            if (character?.Combos == null)
                return;

            foreach (var combo in character.Combos)
            {
                if (!string.IsNullOrEmpty(combo.Hotkey) && combo.IsEnabled)
                {
                    RegisterHotkey(combo.Hotkey, combo);
                }
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string keyName = KeyInterop.KeyFromVirtualKey(vkCode).ToString();

                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    // İlk basış mı kontrol et (tekrar önleme)
                    if (!_pressedKeys.Contains(keyName))
                    {
                        _pressedKeys.Add(keyName);
                        OnKeyPressed(keyName);
                    }
                }
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    _pressedKeys.Remove(keyName);
                    OnKeyReleased(keyName);
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private void OnKeyPressed(string key)
        {
            KeyPressed?.Invoke(this, new KeyEventArgs(key, true));

            // F9: Global Toggle - her zaman çalışır (makrolar kapalıyken de)
            if (key == "F9")
            {
                GlobalToggleRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Makrolar devre dışıysa Z dahil hiçbir tuş çalışmaz
            if (!IsEnabled) return;

            // Eğer hedef oyun odakta değilse kısayolları ve komboları çalıştırma
            if (CheckIsGameActive != null && !CheckIsGameActive())
            {
                return;
            }

            // Z: Playlist Toggle - sadece makrolar aktifken çalışır
            if (key == "Z")
            {
                PlaylistToggleRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_hotkeyBindings.TryGetValue(key, out var combo))
            {
                _macroService.StartMacro(key, combo);
            }
        }

        private void OnKeyReleased(string key)
        {
            KeyReleased?.Invoke(this, new KeyEventArgs(key, false));

            if (_hotkeyBindings.ContainsKey(key))
            {
                _macroService.StopMacro(key);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public string Key { get; }
        public bool IsPressed { get; }

        public KeyEventArgs(string key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }
    }
}
