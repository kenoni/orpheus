using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Orpheus.AppData;
using Orpheus.DataContext;
using Orpheus.Models;
using Orpheus.Mpd;
using Orpheus.Properties;
using Orpheus.CsCore;

namespace Orpheus
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player _player;
        private SettingsWindow _settingsWindow = null;
        readonly MainWindowDataContext _mainWindowDataContext = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer1 = null;
        private MpdServer _mpd = null;
        private AppDataManager _appDataManager;

        public MainWindow()
        {
            InitializeComponent();
            _player = new Player();
            VolumeSlider.Value = Settings.Default.Player_Volume;

            _mpd = new MpdServer();
            _mainWindowDataContext = new MainWindowDataContext(_mpd);
            _mainWindowDataContext.IsPlayerPlaying = false;
            DataContext = _mainWindowDataContext;

            var _appData = new AppData.AppDataContent(_mainWindowDataContext); 
             _appDataManager = new AppDataManager(_appData);
            

            StartTimers();
        }
        public MainWindowDataContext MainWindowDataContext
        {
            get;
            private set;
        }

        private void StartTimers()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += dispatcherTimer_Tick1;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer1.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _mainWindowDataContext.ElapsedTime++;
        }

        private void dispatcherTimer_Tick1(object sender, EventArgs e)
        {
            
            _mainWindowDataContext.UpdateStatus();
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow(this);
            }
            else
            {
                _settingsWindow.Visibility = Visibility.Visible;
            }

            _settingsWindow.Show();
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void OnChildWindowClosing(Window window)
        {
            if (window == _settingsWindow)
            {
                _settingsWindow = null;
            }
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (!_mainWindowDataContext.IsPlayerPlaying)
            {
                _mainWindowDataContext.IsPlayerPlaying = true;
                _player.Play(@"http://rasp-pi.tk:8000/");
                _player.Volume = (int)VolumeSlider.Value;
            }
            else
            {
                _mainWindowDataContext.IsPlayerPlaying = false;
                _player.Stop();
            }
        }

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            _player.Stop();
        }

        private void OnStopClicked(object sender, RoutedEventArgs e)
        {
            _player.Pause();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _player.Stop();
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volumeValue = (int)VolumeSlider.Value;
            _player.Volume = volumeValue;
            Settings.Default.Player_Volume = volumeValue;
            Settings.Default.Save();
        }

        private void OnPlaylistViewDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = ListViewRowBeingClicked(PlayListView, e);
            if (item != null)
            {
                dispatcherTimer.Stop();
                var song = item.DataContext as MpdPlaylistEntry;

                if (song != null)
                {
                    _mpd?.PlayId(song.Id);
                    _mainWindowDataContext?.UpdateStatus();
                    dispatcherTimer.Start();
                }
            }
        }

        private ListViewItem ListViewRowBeingClicked(ListView listView, MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));

            if (hit != null)
            {
                DependencyObject component = (DependencyObject)hit.VisualHit;

                while (component != null)
                {
                    if (component is ListViewItem)
                    {
                        return (ListViewItem)component;
                    }
                    else
                    {
                        component = VisualTreeHelper.GetParent(component);
                    }
                }
            }

            return null;
        }

        private void PlaylisItemSlider_OnValueChanged(object sender, MouseButtonEventArgs e)
        {
            var sliderValue = PlaylisItemSlider.Value;
            _mpd?.SeekCur(sliderValue.ToString(CultureInfo.InvariantCulture));
        }

        private void FileSystemBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (FileSystemDockPanel.Visibility == Visibility.Hidden)
            {
                PlayerDockPanel.Visibility = Visibility.Hidden;
                FileSystemDockPanel.Visibility = Visibility.Visible;
                _mainWindowDataContext.GetMpdFiles();
                SetSplitterVisible(true);
            }
            else
            {
               FileSystemDockPanel.Visibility = Visibility.Hidden;
                SetSplitterVisible(false);
            }
        }

        private void SetSplitterVisible(bool show)
        {
            MainGrid.ColumnDefinitions[2].Width = (show) ? new GridLength(50, GridUnitType.Star) : new GridLength(0);
            GridSplitter1.Visibility = (show) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void TextBlock_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) && !_isDragging)
            {
                var position = e.GetPosition(null);
                Trace.WriteLine(Math.Abs(position.X - _startPoint.X));

                if (Math.Abs(position.X - _startPoint.X) > 2 || Math.Abs(position.Y - _startPoint.Y) > 2)
                {
                    StartDrag(e);
                }
            }
        }

        private Point _startPoint;
        private bool _isDragging;


        private List<MpdFile> GetSelectedFiles()
        {
            return FileTreeview.SelectedItems.Cast<MpdFile>().ToList();
        }

        private void StartDrag(MouseEventArgs e)
        {
            _isDragging = true;
            object temp = GetSelectedFiles().Select(s => s.Uri).ToArray();

            var data = new DataObject("AddToPlaylist", temp);

            var dde = DragDropEffects.Move;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                dde = DragDropEffects.All;
            }
            var de = DragDrop.DoDragDrop(FileTreeview, data, dde);
            
            _isDragging = false;
        }

        private void PlayListView_PreviewDrop(object sender, DragEventArgs e)
        {
            int targetRow = DropTargetRowIndex(e);
            //string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] songsToBeAdded = (string[])e.Data.GetData("AddToPlaylist");

            foreach (var song in songsToBeAdded)
            {
                _mpd?.AddId(song);
            }

            _mainWindowDataContext.UpdatePlayList(); //TO DO : Query only added records
        }

        private int DropTargetRowIndex(DragEventArgs e)
        {
            for (int i = 0; i < PlayListView.Items.Count; ++i)
            {
                DataGridRow row = PlayListView.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;

                if (row != null)
                {
                    Point pt = e.GetPosition(row);
                    double yCoord = row.TranslatePoint(pt, row).Y;
                    double halfHeight = row.ActualHeight / 2;

                    if (yCoord < halfHeight)
                    {
                        return i;
                    }
                }
            }

            return PlayListView.Items.Count;
        }


        private void PlayListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //listview.Items.Remove(listview.Items.CurrentItem);
                object currentItem = PlayListView.Items.CurrentItem;
                if (currentItem != null)
                {
                    foreach(var songToDelete in PlayListView.SelectedItems)
                    {
                        var playlistEntry = (MpdPlaylistEntry)songToDelete;
                        
                        _mainWindowDataContext.CurrentPlaylist.Remove(playlistEntry);
                        _mpd.DeleteId(playlistEntry.Id);
                    }

                    PlayListView.Items.Refresh(); // TO DO : Refresh too soon must be done when all delete commands ended.
                }
            }
        }

        private void FileTreeview_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = TreeViewItemBeingClicked(FileTreeview,e);
            
            if(item != null && item.IsSelected)
                e.Handled = true;
        }

        private MultiSelectTreeViewItem TreeViewItemBeingClicked(MultiSelectTreeView tree, MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(tree, e.GetPosition(tree));

            if (hit != null)
            {
                DependencyObject component = (DependencyObject)hit.VisualHit;

                if (component is TextBlock) // Don't return hits to the expander arrow.
                {
                    while (component != null)
                    {
                        if (component is MultiSelectTreeViewItem)
                        {
                            return (MultiSelectTreeViewItem)component;
                        }
                        else
                        {
                            component = VisualTreeHelper.GetParent(component);
                        }
                    }
                }
            }

            return null;
        }

        private void RescanFileSystem_Click(object sender, RoutedEventArgs e)
        {
            _mpd.Update();
            _mainWindowDataContext.GetMpdFiles();
        }

        private void PlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerDockPanel.Visibility == Visibility.Hidden)
            {
                FileSystemDockPanel.Visibility = Visibility.Hidden;
                PlayerDockPanel.Visibility = Visibility.Visible;
                _appDataManager.GetData();
                SetSplitterVisible(true);
            }
            else
            {
                PlayerDockPanel.Visibility = Visibility.Hidden;
                SetSplitterVisible(false);
            }
        }

        private void PlayerStreamsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            ListViewItem item = ListViewRowBeingClicked(PlayerStreamsListView, e);
            if (item != null)
            {
                var stream = item.DataContext as PlayerStream;

                if(stream != null)
                {
                    _mainWindowDataContext.PlayerStreams.ToList().ForEach(x => x.IsPlaying = false);

                    if (!_mainWindowDataContext.IsPlayerPlaying)
                    {
                        _mainWindowDataContext.IsPlayerPlaying = true;
                        stream.IsPlaying = true;
                    }
                    else
                    {
                        _player.Stop();
                    }

                    _player.Play(stream.Url);
                    _player.Volume = (int)VolumeSlider.Value;
                }
            }

        }
    }
}
