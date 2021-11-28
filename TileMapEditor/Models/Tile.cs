using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TileMapEditor.Models
{
    public class Tile
    {
        public int ImageId { get; set; }
        public BitmapSource CroppedTileSetImage { get; set; }
        public ImageSource ImageSource { get; set; }
        public bool IsCollidable { get; set; }
        public int LayerId { get; set; }
    }
}