using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GameMacroManager.Models;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;

namespace GameMacroManager.Services
{
    /// <summary>
    /// Makro çalıştırma servisi
    /// </summary>
    public class MacroService : IDisposable
    {
        private readonly InputSimulator _inputSimulator;
        private readonly Dictionary<string, CancellationTokenSource> _runningMacros;
        private bool _isEnabled = true;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        public event EventHandler<MacroEventArgs>? MacroStarted;
        public event EventHandler<MacroEventArgs>? MacroStopped;
        public event EventHandler<bool>? EnabledChanged;

        public MacroService()
        {
            _inputSimulator = new InputSimulator();
            _runningMacros = new Dictionary<string, CancellationTokenSource>();
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    if (!_isEnabled)
                    {
                        StopAllMacros();
                    }
                    EnabledChanged?.Invoke(this, _isEnabled);
                }
            }
        }

        /// <summary>
        /// Bir combo'yu çalıştır
        /// </summary>
        public async Task ExecuteComboAsync(Combo combo, CancellationToken cancellationToken = default)
        {
            if (!_isEnabled || combo == null || !combo.IsEnabled)
                return;

            MacroStarted?.Invoke(this, new MacroEventArgs(combo));

            try
            {
                do
                {
                    foreach (var action in combo.Actions)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        await ExecuteKeyActionAsync(action);

                        if (action.DelayAfterMs > 0)
                        {
                            await Task.Delay(action.DelayAfterMs, cancellationToken);
                        }
                    }

                    if (combo.RepeatWhileHeld && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(combo.RepeatDelayMs, cancellationToken);
                    }

                } while (combo.RepeatWhileHeld && !cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {
                // Normal iptal
            }
            finally
            {
                MacroStopped?.Invoke(this, new MacroEventArgs(combo));
            }
        }

        /// <summary>
        /// Tek bir tuş aksiyonunu çalıştır
        /// </summary>
        private async Task ExecuteKeyActionAsync(KeyAction action)
        {
            if (action.Key.Contains(":"))
            {
                var parts = action.Key.Split(':');
                if (parts.Length == 2 && parts[0].StartsWith("Mouse", StringComparison.OrdinalIgnoreCase))
                {
                    var coords = parts[1].Split(',');
                    if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                    {
                        await ExecuteCoordinateClickAsync(parts[0], x, y, action.ActionType, action.HoldDurationMs);
                        return;
                    }
                }
            }

            if (action.Key.StartsWith("Mouse", StringComparison.OrdinalIgnoreCase))
            {
                await ExecuteMouseActionAsync(action);
                return;
            }

            var virtualKey = GetVirtualKeyCode(action.Key);

            switch (action.ActionType)
            {
                case KeyActionType.Press:
                    _inputSimulator.Keyboard.KeyDown(virtualKey);
                    break;

                case KeyActionType.Release:
                    _inputSimulator.Keyboard.KeyUp(virtualKey);
                    break;

                case KeyActionType.Tap:
                    _inputSimulator.Keyboard.KeyDown(virtualKey);
                    if (action.HoldDurationMs > 0)
                    {
                        await Task.Delay(action.HoldDurationMs);
                    }
                    _inputSimulator.Keyboard.KeyUp(virtualKey);
                    break;
            }
        }

        /// <summary>
        /// Belirli koordinata taşıyarak fare tıklaması simüle et
        /// </summary>
        private async Task ExecuteCoordinateClickAsync(string clickType, int x, int y, KeyActionType actionType, int holdDurationMs)
        {
            string mouseButton = clickType.ToLower();
            
            // Fareyi hedef koordinata taşı
            SetCursorPos(x, y);

            // Tıklamayı gerçekleştir
            switch (actionType)
            {
                case KeyActionType.Press:
                    if (mouseButton == "mouseleft") _inputSimulator.Mouse.LeftButtonDown();
                    else if (mouseButton == "mouseright") _inputSimulator.Mouse.RightButtonDown();
                    else if (mouseButton == "mousemiddle") _inputSimulator.Mouse.MiddleButtonDown();
                    break;

                case KeyActionType.Release:
                    if (mouseButton == "mouseleft") _inputSimulator.Mouse.LeftButtonUp();
                    else if (mouseButton == "mouseright") _inputSimulator.Mouse.RightButtonUp();
                    else if (mouseButton == "mousemiddle") _inputSimulator.Mouse.MiddleButtonUp();
                    break;

                case KeyActionType.Tap:
                    if (mouseButton == "mouseleft")
                    {
                        _inputSimulator.Mouse.LeftButtonDown();
                        if (holdDurationMs > 0) await Task.Delay(holdDurationMs);
                        _inputSimulator.Mouse.LeftButtonUp();
                    }
                    else if (mouseButton == "mouseright")
                    {
                        _inputSimulator.Mouse.RightButtonDown();
                        if (holdDurationMs > 0) await Task.Delay(holdDurationMs);
                        _inputSimulator.Mouse.RightButtonUp();
                    }
                    else if (mouseButton == "mousemiddle")
                    {
                        _inputSimulator.Mouse.MiddleButtonDown();
                        if (holdDurationMs > 0) await Task.Delay(holdDurationMs);
                        _inputSimulator.Mouse.MiddleButtonUp();
                    }
                    break;
            }
        }

        /// <summary>
        /// Fare aksiyonunu çalıştır
        /// </summary>
        private async Task ExecuteMouseActionAsync(KeyAction action)
        {
            string mouseButton = action.Key.ToLower();
            switch (action.ActionType)
            {
                case KeyActionType.Press:
                    if (mouseButton == "mouseleft") _inputSimulator.Mouse.LeftButtonDown();
                    else if (mouseButton == "mouseright") _inputSimulator.Mouse.RightButtonDown();
                    else if (mouseButton == "mousemiddle") _inputSimulator.Mouse.MiddleButtonDown();
                    break;

                case KeyActionType.Release:
                    if (mouseButton == "mouseleft") _inputSimulator.Mouse.LeftButtonUp();
                    else if (mouseButton == "mouseright") _inputSimulator.Mouse.RightButtonUp();
                    else if (mouseButton == "mousemiddle") _inputSimulator.Mouse.MiddleButtonUp();
                    break;

                case KeyActionType.Tap:
                    if (mouseButton == "mouseleft")
                    {
                        _inputSimulator.Mouse.LeftButtonDown();
                        if (action.HoldDurationMs > 0)
                        {
                            await Task.Delay(action.HoldDurationMs);
                        }
                        _inputSimulator.Mouse.LeftButtonUp();
                    }
                    else if (mouseButton == "mouseright")
                    {
                        _inputSimulator.Mouse.RightButtonDown();
                        if (action.HoldDurationMs > 0)
                        {
                            await Task.Delay(action.HoldDurationMs);
                        }
                        _inputSimulator.Mouse.RightButtonUp();
                    }
                    else if (mouseButton == "mousemiddle")
                    {
                        _inputSimulator.Mouse.MiddleButtonDown();
                        if (action.HoldDurationMs > 0)
                        {
                            await Task.Delay(action.HoldDurationMs);
                        }
                        _inputSimulator.Mouse.MiddleButtonUp();
                    }
                    break;
            }
        }

        /// <summary>
        /// Bir hotkey için makro başlat
        /// </summary>
        public void StartMacro(string hotkey, Combo combo)
        {
            if (_runningMacros.ContainsKey(hotkey))
                return;

            var cts = new CancellationTokenSource();
            _runningMacros[hotkey] = cts;

            Task.Run(() => ExecuteComboAsync(combo, cts.Token));
        }

        /// <summary>
        /// Bir hotkey için makroyu durdur
        /// </summary>
        public void StopMacro(string hotkey)
        {
            if (_runningMacros.TryGetValue(hotkey, out var cts))
            {
                cts.Cancel();
                _runningMacros.Remove(hotkey);
            }
        }

        /// <summary>
        /// Tüm makroları durdur
        /// </summary>
        public void StopAllMacros()
        {
            foreach (var cts in _runningMacros.Values)
            {
                cts.Cancel();
            }
            _runningMacros.Clear();
        }

        /// <summary>
        /// String tuş adından VirtualKeyCode'a çevir
        /// </summary>
        private VirtualKeyCode GetVirtualKeyCode(string key)
        {
            // Özel tuşlar
            var keyMappings = new Dictionary<string, VirtualKeyCode>(StringComparer.OrdinalIgnoreCase)
            {
                // Harfler
                { "A", VirtualKeyCode.VK_A },
                { "B", VirtualKeyCode.VK_B },
                { "C", VirtualKeyCode.VK_C },
                { "D", VirtualKeyCode.VK_D },
                { "E", VirtualKeyCode.VK_E },
                { "F", VirtualKeyCode.VK_F },
                { "G", VirtualKeyCode.VK_G },
                { "H", VirtualKeyCode.VK_H },
                { "I", VirtualKeyCode.VK_I },
                { "J", VirtualKeyCode.VK_J },
                { "K", VirtualKeyCode.VK_K },
                { "L", VirtualKeyCode.VK_L },
                { "M", VirtualKeyCode.VK_M },
                { "N", VirtualKeyCode.VK_N },
                { "O", VirtualKeyCode.VK_O },
                { "P", VirtualKeyCode.VK_P },
                { "Q", VirtualKeyCode.VK_Q },
                { "R", VirtualKeyCode.VK_R },
                { "S", VirtualKeyCode.VK_S },
                { "T", VirtualKeyCode.VK_T },
                { "U", VirtualKeyCode.VK_U },
                { "V", VirtualKeyCode.VK_V },
                { "W", VirtualKeyCode.VK_W },
                { "X", VirtualKeyCode.VK_X },
                { "Y", VirtualKeyCode.VK_Y },
                { "Z", VirtualKeyCode.VK_Z },

                // Sayılar
                { "D0", VirtualKeyCode.VK_0 },
                { "D1", VirtualKeyCode.VK_1 },
                { "D2", VirtualKeyCode.VK_2 },
                { "D3", VirtualKeyCode.VK_3 },
                { "D4", VirtualKeyCode.VK_4 },
                { "D5", VirtualKeyCode.VK_5 },
                { "D6", VirtualKeyCode.VK_6 },
                { "D7", VirtualKeyCode.VK_7 },
                { "D8", VirtualKeyCode.VK_8 },
                { "D9", VirtualKeyCode.VK_9 },
                { "0", VirtualKeyCode.VK_0 },
                { "1", VirtualKeyCode.VK_1 },
                { "2", VirtualKeyCode.VK_2 },
                { "3", VirtualKeyCode.VK_3 },
                { "4", VirtualKeyCode.VK_4 },
                { "5", VirtualKeyCode.VK_5 },
                { "6", VirtualKeyCode.VK_6 },
                { "7", VirtualKeyCode.VK_7 },
                { "8", VirtualKeyCode.VK_8 },
                { "9", VirtualKeyCode.VK_9 },

                // Yön tuşları
                { "Up", VirtualKeyCode.UP },
                { "Down", VirtualKeyCode.DOWN },
                { "Left", VirtualKeyCode.LEFT },
                { "Right", VirtualKeyCode.RIGHT },

                // Fonksiyon tuşları
                { "F1", VirtualKeyCode.F1 },
                { "F2", VirtualKeyCode.F2 },
                { "F3", VirtualKeyCode.F3 },
                { "F4", VirtualKeyCode.F4 },
                { "F5", VirtualKeyCode.F5 },
                { "F6", VirtualKeyCode.F6 },
                { "F7", VirtualKeyCode.F7 },
                { "F8", VirtualKeyCode.F8 },
                { "F9", VirtualKeyCode.F9 },
                { "F10", VirtualKeyCode.F10 },
                { "F11", VirtualKeyCode.F11 },
                { "F12", VirtualKeyCode.F12 },

                // Özel tuşlar
                { "Space", VirtualKeyCode.SPACE },
                { "Enter", VirtualKeyCode.RETURN },
                { "Return", VirtualKeyCode.RETURN },
                { "Tab", VirtualKeyCode.TAB },
                { "Escape", VirtualKeyCode.ESCAPE },
                { "Esc", VirtualKeyCode.ESCAPE },
                { "Back", VirtualKeyCode.BACK },
                { "Backspace", VirtualKeyCode.BACK },
                { "Delete", VirtualKeyCode.DELETE },
                { "Insert", VirtualKeyCode.INSERT },
                { "Home", VirtualKeyCode.HOME },
                { "End", VirtualKeyCode.END },
                { "PageUp", VirtualKeyCode.PRIOR },
                { "PageDown", VirtualKeyCode.NEXT },
                { "Prior", VirtualKeyCode.PRIOR },
                { "Next", VirtualKeyCode.NEXT },
                { "Capital", VirtualKeyCode.CAPITAL },
                { "CapsLock", VirtualKeyCode.CAPITAL },

                // Modifier tuşlar
                { "Shift", VirtualKeyCode.SHIFT },
                { "LeftShift", VirtualKeyCode.LSHIFT },
                { "RightShift", VirtualKeyCode.RSHIFT },
                { "LShift", VirtualKeyCode.LSHIFT },
                { "RShift", VirtualKeyCode.RSHIFT },
                { "Control", VirtualKeyCode.CONTROL },
                { "Ctrl", VirtualKeyCode.CONTROL },
                { "LeftCtrl", VirtualKeyCode.LCONTROL },
                { "RightCtrl", VirtualKeyCode.RCONTROL },
                { "LControl", VirtualKeyCode.LCONTROL },
                { "RControl", VirtualKeyCode.RCONTROL },
                { "Alt", VirtualKeyCode.MENU },
                { "LeftAlt", VirtualKeyCode.LMENU },
                { "RightAlt", VirtualKeyCode.RMENU },
                { "LAlt", VirtualKeyCode.LMENU },
                { "RAlt", VirtualKeyCode.RMENU },

                // Numpad
                { "NumPad0", VirtualKeyCode.NUMPAD0 },
                { "NumPad1", VirtualKeyCode.NUMPAD1 },
                { "NumPad2", VirtualKeyCode.NUMPAD2 },
                { "NumPad3", VirtualKeyCode.NUMPAD3 },
                { "NumPad4", VirtualKeyCode.NUMPAD4 },
                { "NumPad5", VirtualKeyCode.NUMPAD5 },
                { "NumPad6", VirtualKeyCode.NUMPAD6 },
                { "NumPad7", VirtualKeyCode.NUMPAD7 },
                { "NumPad8", VirtualKeyCode.NUMPAD8 },
                { "NumPad9", VirtualKeyCode.NUMPAD9 },
                { "Multiply", VirtualKeyCode.MULTIPLY },
                { "Add", VirtualKeyCode.ADD },
                { "Subtract", VirtualKeyCode.SUBTRACT },
                { "Decimal", VirtualKeyCode.DECIMAL },
                { "Divide", VirtualKeyCode.DIVIDE },
            };

            if (keyMappings.TryGetValue(key, out var vk))
            {
                return vk;
            }

            // Tek karakter ise, doğrudan çevir
            if (key.Length == 1)
            {
                char c = char.ToUpper(key[0]);
                if (c >= 'A' && c <= 'Z')
                {
                    return (VirtualKeyCode)(0x41 + (c - 'A'));
                }
            }

            return VirtualKeyCode.VK_A; // Varsayılan
        }

        public void Dispose()
        {
            StopAllMacros();
        }
    }

    public class MacroEventArgs : EventArgs
    {
        public Combo Combo { get; }

        public MacroEventArgs(Combo combo)
        {
            Combo = combo;
        }
    }
}
