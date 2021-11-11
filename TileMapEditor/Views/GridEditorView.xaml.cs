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

        public ImageSource SelectedImageBottom { get; set; }
        public ImageSource SelectedImageTop { get; set; }

        public event EventHandler<int[,]> TileElementPressed;
        private void TileElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var gridTile = (Grid)sender;
            var gridTileImages = gridTile.Children.OfType<Image>();
            var tileImages = gridTileImages.ToList();
            var bottomImage = tileImages.Find(x => x.Name == "BottomImage");
            var topImage = tileImages.Find(x => x.Name == "TopImage");

            if (bottomImage != null && SelectedImageBottom != null)
            {
                bottomImage.Source = SelectedImageBottom;
            }
            if (topImage != null && SelectedImageTop != null)
            {
                topImage.Source = SelectedImageTop;
            }
            TileElementPressed?.Invoke(this, (int[,])gridTile.Tag);
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
    }
}
