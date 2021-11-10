using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileMapEditor.Models;

namespace TileMapEditor.Views
{
    /// <summary>
    /// Interaction logic for TileEditorView.xaml
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
