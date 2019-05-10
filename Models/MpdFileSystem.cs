using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Orpheus.Models
{
    public class MpdFileSystem
    {
        public Dictionary<int,ITreeItem<MpdFile>> Items { get; set; }
        
    }

    public interface ITreeItem<T>
    {
        string Uri { get; set; }
        Dictionary<int,ITreeItem<T>> Children { get; set; }
        MpdFileType Type { get; set; }
        string Name { get; set; }
    }

    public class MpdFile : INotifyPropertyChanged, ITreeItem<MpdFile>
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public string Name { get; set; }
        public string Uri { get; set; }
        public MpdFileType Type { get; set; }
        public Dictionary<int, ITreeItem<MpdFile>> Children { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
    }

    public enum MpdFileType
    {
        Folder=1,
        File = 2
    }
}
