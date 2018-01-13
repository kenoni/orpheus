using System;
using System.ComponentModel;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace Orpheus.CsCore
{
    public class MusicPlayer : Component
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                    return _soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
            }

            set => _waveSource?.SetPosition(value);
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                    return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                }
            }
        }

        public void Open(string uri, MMDevice device)
        {
            try
            {
                CleanupPlayback();

                _waveSource = CodecFactory.Instance.GetCodec(new Uri(uri));
                _soundOut = new WasapiOut() {Latency = 100, Device = device};
                _soundOut.Initialize(_waveSource);
                if (PlaybackStopped != null) _soundOut.Stopped += PlaybackStopped;
            }
            catch (Exception ex)
            {
                
            }
        }

        public void Play()
        {
            if(_soundOut.WaveSource != null)
            _soundOut?.Play();
        }

        public void Pause()
        {
            _soundOut?.Pause();
        }

        public void Stop()
        {
            _soundOut?.Stop();
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}