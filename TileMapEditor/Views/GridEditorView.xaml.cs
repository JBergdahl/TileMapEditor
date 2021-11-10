using System;
using System.Collections.Generic;
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
using TileMapEditor.Models;

namespace TileMapEditor.Views
{
    /// <summary>
    /// Interaction logic for GridEditor.xaml
    /// </summary>
    public partial class GridEditorView : UserControl
    {
        public GridEditorView()
        {
            InitializeComponent();
        }

        public int[,] TileId { get; set; }

        public Image SelectedImage { get; set; }

        public event EventHandler<int[,]> TileElementPressed;

        private void TileElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;

            if (SelectedImage is null) return;

            image.Source = SelectedImage.Source;
            TileId = (int[,])image.Tag;
            TileElementPressed?.Invoke(this, TileId);
        }

        public void OnSelectedCroppedImageChanged(object? sender, Tile tile)
        {
            SelectedImage = tile.Image;
        }
    }
}
