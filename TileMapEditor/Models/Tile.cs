using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TileMapEditor.Models
{
    public class Tile
    {
        public int[,] TileId { get; set; }
        public int ImageId { get; set; }
        public string Data { get; set; }
        public BitmapImage Background { get; set; }
        public BitmapSource BackgroundSource { get; set; }
        public CroppedBitmap CroppedBitmap { get; set; }
        public string Image { get; set; }
        public Image ImageTest { get; set; }
    }
}