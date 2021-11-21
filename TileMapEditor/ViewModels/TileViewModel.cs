using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class TileViewModel : ObservableObject
    {
        public TileViewModel()
        {
            FillEmptySpaceCommand = new RelayCommand(OnFillEmptySpaceCommand);
            CanEditTileGrid = true;
        }

        public RelayCommand FillEmptySpaceCommand { get; set; }

        public event EventHandler<Tile> FillEmptyGridSpaceWithSelectedTile;
        private void OnFillEmptySpaceCommand()
        {
            if (CanEditTileGrid)
            {
                var tileToSend = new Tile
                {
                    ImageId = ImageId,
                    IsCollidable = IsCollidable,
                    LayerId = LayerId ? 0 : 1,
                    Image = Image,
                    ImageSource = ImageSource
                };

                FillEmptyGridSpaceWithSelectedTile?.Invoke(this, tileToSend);
            }
        }

        private bool _isCollidable;
        private bool _layerId;
        private ImageSource _imageSource;
        private int _imageId;
        private Image _image;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public event EventHandler<bool> SelectedTileCollidableChanged;
        public bool IsCollidable
        {
            get => _isCollidable;
            set
            {
                if (SetProperty(ref _isCollidable, value))
                {
                    SelectedTileCollidableChanged?.Invoke(this, _isCollidable);
                }
            }
        }

        public int ImageId
        {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        public Image Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public event EventHandler<int> SelectedTileLayerChanged;
        public bool LayerId
        {
            get => _layerId;
            set
            {
                if (SetProperty(ref _layerId, value))
                {
                    SelectedTileLayerChanged?.Invoke(this, LayerId ? 0 : 1);
                }
            }
        }

        public event EventHandler<Tile> SelectedTileChanged;
        public void OnSelectedCroppedImageChanged(object? sender, Tile tile)
        {
            ImageSource = tile.ImageSource;
            IsCollidable = false;
            LayerId = true;
            ImageId = tile.ImageId;
            Image = tile.Image;

            var tileToSend = new Tile
            {
                ImageSource = tile.ImageSource,
                ImageId = tile.ImageId,
                Image = tile.Image
            };

            SelectedTileChanged?.Invoke(this, tileToSend);
        }

        private bool CanEditTileGrid { get; set; }
        public void OnIsShowingCollisionPressed(object? sender, bool e)
        {
            CanEditTileGrid = e;
        }
    }
}