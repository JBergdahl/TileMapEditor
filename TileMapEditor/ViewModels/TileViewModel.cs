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
            FillEmptySpaceCommand = new AsyncRelayCommand(OnFillEmptySpaceCommand);
        }

        public AsyncRelayCommand FillEmptySpaceCommand { get; set; }

        private async Task OnFillEmptySpaceCommand()
        {
            // Fill empty space
        }

        private bool _isCollidable;
        private bool _layerId;
        private ImageSource _imageSource;

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

            var tileToSend = new Tile
            {
                ImageSource = tile.ImageSource,
                ImageId = tile.ImageId,
                Image = tile.Image
            };

            SelectedTileChanged?.Invoke(this, tileToSend);
        }
    }
}