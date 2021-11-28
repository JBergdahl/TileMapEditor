using System;
using System.Windows.Controls;
using System.Windows.Input;
using TileMapEditor.Models;

namespace TileMapEditor.Views
{
    /// <summary>
    ///     Interaction logic for TileEditorView.xaml
    /// </summary>
    public partial class TilePickerView : UserControl
    {
        public TilePickerView()
        {
            InitializeComponent();
        }

        public event EventHandler<Tile> SelectedCroppedImageChanged;

        private void TileSetImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pressedTileSetImage = (Image)sender;
            var tile = (Tile)pressedTileSetImage.DataContext;

            var tileInfoToSend = new Tile
            {
                ImageId = tile.ImageId,
                ImageSource = tile.CroppedTileSetImage
            };

            SelectedCroppedImageChanged?.Invoke(this, tileInfoToSend);
        }
    }
}