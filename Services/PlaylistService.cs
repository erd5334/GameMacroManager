using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameMacroManager.Models;

namespace GameMacroManager.Services
{
    /// <summary>
    /// Playlist çalıştırma servisi
    /// </summary>
    public class PlaylistService : IDisposable
    {
        private readonly MacroService _macroService;
        private CancellationTokenSource? _playlistCts;
        private bool _isPlaying = false;

        public event EventHandler<PlaylistEventArgs>? PlaylistStarted;
        public event EventHandler<PlaylistEventArgs>? PlaylistStopped;
        public event EventHandler<PlaylistItemEventArgs>? ItemStarted;
        public event EventHandler<PlaylistItemEventArgs>? ItemCompleted;

        public bool IsPlaying => _isPlaying;

        public PlaylistService(MacroService macroService)
        {
            _macroService = macroService;
        }

        /// <summary>
        /// Playlist'i çalıştır
        /// </summary>
        public async Task PlayAsync(ComboPlaylist playlist, CancellationToken cancellationToken = default)
        {
            if (_isPlaying || playlist == null || !playlist.IsEnabled)
                return;

            _isPlaying = true;
            _playlistCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            PlaylistStarted?.Invoke(this, new PlaylistEventArgs(playlist));

            try
            {
                do
                {
                    foreach (var item in playlist.Items.Where(i => i.IsEnabled && i.Combo != null))
                    {
                        if (_playlistCts.Token.IsCancellationRequested)
                            break;

                        // Item başladı
                        ItemStarted?.Invoke(this, new PlaylistItemEventArgs(item));

                        // Combo'yu çalıştır
                        await _macroService.ExecuteComboAsync(item.Combo!, _playlistCts.Token);

                        // Item tamamlandı
                        ItemCompleted?.Invoke(this, new PlaylistItemEventArgs(item));

                        // Gecikme
                        if (item.DelayAfterMs > 0 && !_playlistCts.Token.IsCancellationRequested)
                        {
                            await Task.Delay(item.DelayAfterMs, _playlistCts.Token);
                        }
                    }

                    // Döngü modunda değilse çık
                    if (!playlist.IsLooping)
                        break;

                } while (!_playlistCts.Token.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {
                // Normal iptal
            }
            finally
            {
                _isPlaying = false;
                PlaylistStopped?.Invoke(this, new PlaylistEventArgs(playlist));
            }
        }

        /// <summary>
        /// Playlist'i durdur
        /// </summary>
        public void Stop()
        {
            _playlistCts?.Cancel();
            _macroService.StopAllMacros();
        }

        public void Dispose()
        {
            Stop();
            _playlistCts?.Dispose();
        }
    }

    public class PlaylistEventArgs : EventArgs
    {
        public ComboPlaylist Playlist { get; }

        public PlaylistEventArgs(ComboPlaylist playlist)
        {
            Playlist = playlist;
        }
    }

    public class PlaylistItemEventArgs : EventArgs
    {
        public PlaylistItem Item { get; }

        public PlaylistItemEventArgs(PlaylistItem item)
        {
            Item = item;
        }
    }
}
