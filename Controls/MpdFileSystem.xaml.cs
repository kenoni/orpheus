using Orpheus.DataContext;
using Orpheus.Models;
using Orpheus.Mpd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Orpheus.Controls
{
    /// <summary>
    /// Interaction logic for MpdFileSystem.xaml
    /// </summary>
    public partial class MpdFileSystem : UserControl
    {
        public MpdFileSystem()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckBox)
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

        private void FileTreeview_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = TreeViewItemBeingClicked(FileTreeview, e);

            if (item != null && item.IsSelected)
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

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
    }
}
