using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class TileViewModel : ObservableObject
    {
        private Image _image;
        private int _imageId;
        private ImageSource _imageSource;
        private bool _isCollidable;
        private bool _layerId;

        public TileViewModel()
        {
            FillEmptySpaceCommand = new RelayCommand(OnFillEmptySpaceCommand);
            CanEditTileGrid = true;
        }

        public event EventHandler<Tile> FillEmptyGridSpaceWithSelectedTile;

        public event EventHandler<bool> SelectedTileCollidableChanged;

        public event EventHandler<int> SelectedTileLayerChanged;

        public event EventHandler<Tile> SelectedTileChanged;

        public RelayCommand FillEmptySpaceCommand { get; set; }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public bool IsCollidable
        {
            get => _isCollidable;
            set
            {
                if (SetProperty(ref _isCollidable, value)) SelectedTileCollidableChanged?.Invoke(this, _isCollidable);
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

        public bool LayerId
        {
            get => _layerId;
            set
            {
                if (SetProperty(ref _layerId, value)) SelectedTileLayerChanged?.Invoke(this, LayerId ? 0 : 1);
            }
        }

        private bool CanEditTileGrid { get; set; }

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

        public void OnIsShowingCollisionPressed(object? sender, bool e)
        {
            CanEditTileGrid = e;
        }
    }
}