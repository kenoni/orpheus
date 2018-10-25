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
        public readonly SettingsWindowDataContext _settingsDataContext = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer1 = null;
        private AppDataManager _appDataManager;

        public MainWindow()
        {
            
            InitializeComponent();
            VolumeSlider.Value = Settings.Default.Player_Volume;
            
            InitializeDataContext();
            InitializeAppDataManager();
            LoadSettings();
            StartTimers();
        }

        private void InitializeDataContext()
        {
            DataContext = MainContext.Instance;
            MpdServer.CreateInstance(MainContext.Instance.MainWindow.DisplayMessage, MainContext.Instance.MainWindow.UpdateUI);
        }

        public void InitializeAppDataManager()
        {
            var _appData = new AppData.AppDataContent(MainContext.Instance.MainWindow);
            _appDataManager = new AppDataManager(_appData);
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
            if (MainContext.Instance.MainWindow.ElapsedTime != 0 && MainContext.Instance.MainWindow.Duration != 0)
            {
                MainContext.Instance.MainWindow.ElapsedTime++;
            }
        }

        private void dispatcherTimer_Tick1(object sender, EventArgs e)
        {

            MainContext.Instance.MainWindow.UpdateStatus();
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (!MainContext.Instance.MainWindow.IsPlayerPlaying && MainContext.Instance.MainWindow.PlayerStreams.Count > 0)
            {
                var streamToPlay = MainContext.Instance.MainWindow.PlayerStreams.FirstOrDefault(s => s.IsPlaying) ?? MainContext.Instance.MainWindow.PlayerStreams.FirstOrDefault();
                MainContext.Instance.MainWindow.IsPlayerPlaying = true;

                Player.Instance.Play(streamToPlay.Url);
                Player.Instance.Volume = (int)VolumeSlider.Value;

                streamToPlay.IsPlaying = true;
            }
            else
            {
                MainContext.Instance.MainWindow.IsPlayerPlaying = false;
                Player.Instance.Stop();
            }
        }

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            Player.Instance.Stop();
        }

        private void OnStopClicked(object sender, RoutedEventArgs e)
        {
            Player.Instance.Pause();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Player.Instance.Stop();
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volumeValue = (int)VolumeSlider.Value;
            Player.Instance.Volume = volumeValue;
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
                    MpdServer.Instance?.PlayId(song.Id);
                    MainContext.Instance?.MainWindow?.UpdateStatus();
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
            MpdServer.Instance?.SeekCur(sliderValue.ToString(CultureInfo.InvariantCulture));
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
                MpdServer.Instance?.AddId(song);
            }

            MainContext.Instance.MainWindow.UpdatePlayList(); //TO DO : Query only added records
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

                        MainContext.Instance.MainWindow.CurrentPlaylist.Remove(playlistEntry);
                        MpdServer.Instance.DeleteId(playlistEntry.Id);
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
            MainContext.Instance.MainWindow.MpdFileSystem = null;
            MpdServer.Instance.Update();
        }

        private void PlayerStreamsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            ListViewItem item = ListViewRowBeingClicked(PlayerStreamsListView, e);
            if (item != null)
            {
                var stream = item.DataContext as PlayerStream;

                if(stream != null)
                {
                    MainContext.Instance.MainWindow.PlayerStreams.ToList().ForEach(x => x.IsPlaying = false);

                    if (!MainContext.Instance.MainWindow.IsPlayerPlaying)
                    {
                        MainContext.Instance.MainWindow.IsPlayerPlaying = true;
                        stream.IsPlaying = true;
                    }
                    else
                    {
                        Player.Instance.Stop();
                    }

                    Player.Instance.Play(stream.Url);
                    Player.Instance.Volume = (int)VolumeSlider.Value;
                }
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddStreamName.Text = string.Empty;
            AddStreamUrl.Text = string.Empty;
            AddStreamPopup.IsOpen = true;
        }

        private void AddStreamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainContext.Instance.MainWindow.PlayerStreams == null) MainContext.Instance.MainWindow.PlayerStreams = new List<PlayerStream>();
            MainContext.Instance.MainWindow.PlayerStreams.Add(new PlayerStream { Name = AddStreamName.Text, Url = AddStreamUrl.Text });
            _appDataManager.SaveData();
            AddStreamPopup.IsOpen = false;
            PlayerStreamsListView.Items.Refresh();
        }

        private void PlayerStreamsListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //listview.Items.Remove(listview.Items.CurrentItem);
                object currentItem = PlayerStreamsListView.Items.CurrentItem;
                if (currentItem != null)
                {
                    foreach (var streamToDelete in PlayerStreamsListView.SelectedItems)
                    {
                        var streamEntry = (PlayerStream)streamToDelete;

                        MainContext.Instance.MainWindow.PlayerStreams.Remove(streamEntry);
                    }

                    PlayerStreamsListView.Items.Refresh(); // TO DO : Refresh too soon must be done when all delete commands ended.
                }
            }
            _appDataManager.SaveData();

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                string tabName = ((sender as TabControl).SelectedItem as TabItem).Header as string;

                switch (tabName)
                {
                    case "Player": if (MainContext.Instance.MainWindow.PlayerStreams == null) _appDataManager.GetData(); break;
                    default: break;
                }
                e.Handled = true;
            }
           
        }

        public void LoadSettings()
        {
            TbMpdAddress.Text = Settings.Default.Mpd_Address;
            TbMpdRefreshInterval.Text = Settings.Default.Mpd_RefreshInterval.ToString();
            CbOutputDevices.SelectedValue = Settings.Default.OutputDeviceId;
        }

        public void SaveSettings()
        {
            Settings.Default.Mpd_Address = TbMpdAddress.Text;
            Settings.Default.Mpd_RefreshInterval = TbMpdRefreshInterval.Text.ToInt();
            var outputDevice = CbOutputDevices.SelectedValue;
            Settings.Default.OutputDeviceId = outputDevice.ToString();
            Settings.Default.Save();
        }

        private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MpdServer.CreateInstance(MainContext.Instance.MainWindow.DisplayMessage, MainContext.Instance.MainWindow.UpdateUI);
        }

        private void GridSplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(TabControlHolder.Width.Value != 30)
            {
                PreviousGridSplitterWIdth = TabControlHolder.Width.Value;
                TabControlHolder.Width = new GridLength(30, GridUnitType.Pixel);
            }
            else
            {
                TabControlHolder.Width = new GridLength(PreviousGridSplitterWIdth, GridUnitType.Pixel);
            }
        }

        private double PreviousGridSplitterWIdth = 0;

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(e.Source is CheckBox)
            {
                var checkbox = (CheckBox)e.Source;
                var output = (MpdOutput)checkbox.DataContext;

                if (checkbox.IsChecked ?? false)
                {
                    MpdServer.Instance.EnableOutput(output.Id);
                }
                else
                {
                    MpdServer.Instance.DisableOutput(output.Id);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var firstSongInCurrentPlaylist = MainContext.Instance.MainWindow.CurrentPlaylist.FirstOrDefault();
            if(firstSongInCurrentPlaylist != null)
            {
                MpdServer.Instance.PlayId(firstSongInCurrentPlaylist.Id);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Next();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Stop();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Previous();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Random("1");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Repeat("1");
        }
    }
}
