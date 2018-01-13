using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orpheus.Models;
using Orpheus.Mpd;

namespace Orpheus.DataContext
{
    public class MainWindowDataContext : INotifyPropertyChanged
    {
        private MpdServer _mpd = null;

        public MainWindowDataContext(MpdServer mpd)
        {
            _mpd = mpd;
            _mpd.DisplayStatus += (message) => { CommandsStatus = message; };

            CurrentPlaylist = new ObservableCollection<MpdPlaylistEntry>();

            UpdatePlayList();
            UpdateStatus();
        }

        private void FillCurrentPlaylist(MpdPlaylist mpdPlaylist)
        {
            if (mpdPlaylist == null) return;
            CurrentPlaylist = mpdPlaylist.Items;
        }

        public void  UpdatePlayList()
        {
            _mpd.PlaylistInfo(FillCurrentPlaylist);
        }

        private void FillStatusFields(MpdStatus status)
        {
            if (status == null) return;

            State = $"{status.State}@{_mpd.ConnectionAsString}";
            Duration = status.Duration;
            ElapsedTime = status.Elapsed;

            SetPlayingItem(status.SongId);
        }

        private void SetPlayingItem(string songId)
        {
            if(PlayingSongId != songId)
            {
                PlayingSongId = songId;

                var playingItem = CurrentPlaylist?.First(s => s.Id == PlayingSongId);
                if (playingItem != null)
                {
                    CurrentPlaylist?.ToList().ForEach(x => { x.IsCurrentlyPlaying = false; });
                    playingItem.IsCurrentlyPlaying = true;
                    PlayingSong = playingItem.Name;
                }
            }
        }

        public void UpdateStatus()
        {
            _mpd.Status(FillStatusFields);
        }

        private void FillFileSystem(MpdFileSystem files)
        {
            MpdFileSystem = files.Items;
        }

        public void GetMpdFiles()
        {
            _mpd.FilelistInfo(FillFileSystem);
        }

        private IList<MpdPlaylistEntry> _currentPlaylist;

        public IList<MpdPlaylistEntry> CurrentPlaylist
        {
            get => _currentPlaylist;
            set
            {
                _currentPlaylist = value;
                NotifyPropertyChanged("CurrentPlaylist");
                foreach (var item in _currentPlaylist)
                {
                    item.PropertyChanged += PropertyChanged;
                }
            }
        }

        private IList<MpdFile> _mpdFileSystem;

        public IList<MpdFile> MpdFileSystem
        {
            get => _mpdFileSystem;
            set
            {
                if (_mpdFileSystem == value) return;

                _mpdFileSystem = value;
                NotifyPropertyChanged("MpdFileSystem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private string _state;

        public string State {
            get => _state;
            set
            {
                if (_state == value) return;

                _state = value;
                NotifyPropertyChanged("State");
            }
        }

        private double _duration;
        public double Duration
        {
            get => _duration;
            set
            {
                if (!(Math.Abs(_duration - value) > 0.1)) return;

                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        private double _elapsedTime;

        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if (Math.Abs(_elapsedTime - value) < 0.1) return;

                _elapsedTime = value;
                NotifyPropertyChanged("ElapsedTime");
            }
        }
        private string _playingSong;
        public string PlayingSong
        {
            get => _playingSong;
            set
            {
                if (_playingSong == value) return;

                _playingSong = value;
                NotifyPropertyChanged("PlayingSong");
            }
        }

        public string PlayingSongId { get; set; }

        

        private string _commandsStatus;
        public string CommandsStatus
        {
            get => _commandsStatus;
            set
            {
                if (_commandsStatus == value) return;

                _commandsStatus = value;
                NotifyPropertyChanged("CommandsStatus");
            }
        }

        private bool _isPlayerPlaying;
        public bool IsPlayerPlaying
        {
            get => _isPlayerPlaying;
            set
            {
                _isPlayerPlaying = value;
                NotifyPropertyChanged("IsPlayerPlaying");
            }
        }

    }
}
