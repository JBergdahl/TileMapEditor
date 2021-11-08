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

        //public Image SelectedCroppedImage { get; set; }
        public ObservableCollection<Models.Tile> SelectedCroppedImage { get; set; } = new();

        public event EventHandler<Models.Tile> SelectedCroppedImageChanged;
        private void TileImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pressedTile = (Image)sender;
            SelectedTileSetImage.Source = pressedTile.Source;
            Debug.WriteLine(pressedTile.Source);
            SelectedCroppedImageChanged?.Invoke(this, new Models.Tile{CroppedBitmap = (CroppedBitmap)pressedTile.Source, ImageId = (int)pressedTile.Tag, ImageTest = pressedTile});
            
            //SelectedCroppedImageChanged?.Invoke(this, new Models.Tile{Background = (BitmapImage)pressedTile.Source, ImageId = (int)pressedTile.Tag, ImageTest = pressedTile});
        }
    }
}
