using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace TileMapEditor.Models
{
    public class GridEditorModel
    {
        public int[,] TileId { get; set; }
        public int ImageId { get; set; }
        public ImageSource ImageSourceBottomLayer { get; set; }
        public ImageSource ImageSourceTopLayer { get; set; }
        public bool IsCollidable { get; set; }
        public int LayerId { get; set; }
    }
}