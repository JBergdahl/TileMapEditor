using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TileMapEditor.Models
{
    public class Tile
    {
        public int[,] TileId { get; set; }
        public int ImageId { get; set; }
        public BitmapSource CroppedTileSetImage { get; set; }
        public string ImagePath { get; set; }
        public Image Image { get; set; }
        public bool IsCollidable { get; set; } 
    }
}