using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Orpheus.CsCore.Helper;

namespace Orpheus.DataContext
{
    class SettingsWindowDataContext : INotifyPropertyChanged
    {
        public SettingsWindowDataContext()
        {
            _outputDevices = GetOutputDevice().ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private List<MMDevice> _outputDevices = null;
        public List<MMDevice> OutputDevices
        {
            get => _outputDevices;
            set
            {
                if (_outputDevices == value) return;

                _outputDevices = value;
                NotifyPropertyChanged("OutputDevices");
            }
        }

    }
}

