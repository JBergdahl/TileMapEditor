using System;
using System.Windows.Controls;
using System.Windows.Input;
using TileMapEditor.Models;

namespace TileMapEditor.Views
{
    /// <summary>
    ///     Interaction logic for TileEditorView.xaml
    /// </summary>
    public partial class TileEditorView : UserControl
    {
        public TileEditorView()
        {
            InitializeComponent();
        }

        public event EventHandler<Tile> SelectedCroppedImageChanged;

        private void TileSetImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pressedTileSetImage = (Image)sender;

            var tileInfoToSend = new Tile
            {
                ImageId = (int)pressedTileSetImage.Tag,
                Image = pressedTileSetImage,
                ImageSource = pressedTileSetImage.Source
            };

            SelectedCroppedImageChanged?.Invoke(this, tileInfoToSend);
        }
    }
}