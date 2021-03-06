﻿using CSCore.CoreAudioAPI;
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
    public class SettingsWindowDataContext : INotifyPropertyChanged
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

        private IList<MMDevice> _outputDevices;
        public IList<MMDevice> OutputDevices
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

