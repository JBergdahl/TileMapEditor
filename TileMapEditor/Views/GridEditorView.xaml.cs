using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TileMapEditor.Models;

namespace TileMapEditor.Views
{
    /// <summary>
    ///     Interaction logic for GridEditor.xaml
    /// </summary>
    public partial class GridEditorView : UserControl
    {
        public GridEditorView()
        {
            InitializeComponent();
            CanEditTileGrid = true;
        }

        public ImageSource SelectedImageBottom { get; set; }
        public ImageSource SelectedImageTop { get; set; }
        private bool CanEditTileGrid { get; set; }

        public event EventHandler<Point> TileElementPressed;

        private void TileElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!CanEditTileGrid)
            {
                return;
            }

            var gridTile = (Grid)sender;
            var mapTile = (MapTile)gridTile.DataContext;

            TileElementPressed?.Invoke(this, mapTile.TilePositionOnGrid);
        }

        public event EventHandler<Point> TileElementRightPressed;

        private void TileElement_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!CanEditTileGrid)
            {
                return;
            }

            var gridTile = (Grid)sender;
            var mapTile = (MapTile)gridTile.DataContext;

            TileElementRightPressed?.Invoke(this, mapTile.TilePositionOnGrid);
        }

        public void OnImageSourceBottomLayerChanged(object? sender, ImageSource imageSource)
        {
            SelectedImageBottom = imageSource;
            SelectedImageTop = null;
        }

        public void OnImageSourceTopLayerChanged(object? sender, ImageSource imageSource)
        {
            SelectedImageTop = imageSource;
            SelectedImageBottom = null;
        }

        public void OnIsShowingCollisionPressed(object? sender, bool e)
        {
            CanEditTileGrid = e;
        }
    }
}