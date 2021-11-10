using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TileMapEditor.Models
{
    public class Tile
    {
        public int[,] TileId { get; set; }
        public int ImageId { get; set; }
        public BitmapSource CroppedTileSetImage { get; set; }
        public string ImagePath { get; set; }
        public ImageSource ImageSource { get; set; }
        public Image Image { get; set; }
        public bool IsCollidable { get; set; }
        public int LayerId { get; set; }
    }
}