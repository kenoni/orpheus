using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus.Models
{
    public class MpdPlaylistEntry : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Path { get; set; }
        public MpdPlaylistEntryType Type { get; set; }
        public string Id { get; set; }

        private bool _isCurrentlyPlaying;
        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set
            {
                _isCurrentlyPlaying = value;
                NotifyPropertyChanged("IsCurrentlyPlaying");
            }
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }

    public enum MpdPlaylistEntryType
    {
        Folder = 0,
        File = 1
    }
}
