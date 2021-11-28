using System.Windows;
using System.Windows.Media;

namespace TileMapEditor.Models
{
    public class MapTile
    {
        public Point TilePositionOnGrid { get; set; }
        public int ImageIdBottom { get; set; }
        public int ImageIdTop { get; set; }
        public ImageSource ImageSourceBottomLayer { get; set; }
        public ImageSource ImageSourceTopLayer { get; set; }
        public bool IsCollidable { get; set; }
    }
}