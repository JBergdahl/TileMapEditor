using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public event EventHandler<int[,]> TileElementPressed;

        private void TileElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!CanEditTileGrid) return;
            var gridTile = (Grid)sender;
            var gridTileImages = gridTile.Children.OfType<Image>();
            var tileImages = gridTileImages.ToList();
            var bottomImage = tileImages.Find(x => x.Name == "BottomImage");
            var topImage = tileImages.Find(x => x.Name == "TopImage");

            if (bottomImage != null && SelectedImageBottom != null) bottomImage.Source = SelectedImageBottom;
            if (topImage != null && SelectedImageTop != null) topImage.Source = SelectedImageTop;

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

        public void OnIsShowingCollisionPressed(object? sender, bool e)
        {
            CanEditTileGrid = e;
        }

        public event EventHandler<int[,]> TileElementRightPressed;

        private void TileElement_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!CanEditTileGrid) return;
            var gridTile = (Grid)sender;
            var gridTileImages = gridTile.Children.OfType<Image>();
            var tileImages = gridTileImages.ToList();
            var bottomImage = tileImages.Find(x => x.Name == "BottomImage");
            var topImage = tileImages.Find(x => x.Name == "TopImage");

            if (topImage is { Source: { } })
                topImage.Source = null;
            else if (bottomImage is { Source: { } })
                bottomImage.Source =
                    new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png"));

            TileElementRightPressed?.Invoke(this, (int[,])gridTile.Tag);
        }
    }
}