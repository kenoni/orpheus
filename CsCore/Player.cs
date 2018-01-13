using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using static Orpheus.CsCore.Helper;
using Orpheus.Properties;

namespace Orpheus.CsCore
{
    class Player
    {
        private readonly MusicPlayer _musicPlayer = new MusicPlayer();
        private List<MMDevice> _devices = new List<MMDevice>();
        private MMDevice _usedOutputDevice = null;

        public Player()
        {
            SetOutputDevice();
        }

        private void SetOutputDevice()
        {
            _devices = GetOutputDevice().ToList();
            var usedOutputDevice = _devices.FirstOrDefault(d => d.DeviceID == Settings.Default.OutputDeviceId);
            _usedOutputDevice = (_devices.Count == 1) ? _devices[0] : usedOutputDevice;
        }

        public void Play(string url)
        {
            SetOutputDevice();

            if (_usedOutputDevice != null)
            {
                _musicPlayer.Open(url, _usedOutputDevice);
                _musicPlayer.Play();
            }
        }

        public void Stop()
        {
            _musicPlayer.Stop();
        }

        public void Pause()
        {
            _musicPlayer.Pause();
        }

        public int Volume
        {
            get => _musicPlayer.Volume;
            set => _musicPlayer.Volume = value;
        }
    }
}
