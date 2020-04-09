using System.Collections.Generic;
using System.Linq;
using CSCore.CoreAudioAPI;
using static Orpheus.CsCore.Helper;
using Orpheus.Properties;

namespace Orpheus.CsCore
{
    class Player
    {
        private static Player _instance;
        private readonly MusicPlayer _musicPlayer;
        private List<MMDevice> _devices = new List<MMDevice>();
        private MMDevice _usedOutputDevice = null;

        private Player()
        {
            SetOutputDevice();
            _musicPlayer = new MusicPlayer();
        }

        public static Player Instance => _instance ?? (_instance = new Player());

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
