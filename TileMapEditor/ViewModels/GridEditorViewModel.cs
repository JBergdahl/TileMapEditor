using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class GridEditorViewModel : ObservableRecipient
    {
        private int _rows;
        private int _columns;
        private ImageSource _imageSourceBottomLayer;
        private int[,] _tileId;
        private ImageSource _imageSourceTopLayer;
        private int _imageId;
        private bool _isCollidable;
        private int _layerId;
        private ImageSource _imageSourceFromTileViewModel;

        public GridEditorViewModel(int rows, int cols)
        {
            InitTiles(rows, cols);
            PrintInfo = new RelayCommand(PrintTileInfo);
        }

        public RelayCommand PrintInfo { get; set; }

        private void PrintTileInfo()
        {
            foreach (var tile in GridTiles)
            {
                Debug.WriteLine("Row: " + tile.TileId.GetLength(0) + ", Col: " + tile.TileId.GetLength(1) + ", ID: " + tile.ImageIdBottom);
                Debug.WriteLine("Is collidable: " + tile.IsCollidable + ", Layer ID: " + tile.LayerId);
                Debug.WriteLine("Image source: " + tile.ImageSourceBottomLayer);
                Debug.WriteLine("Image source: " + tile.ImageSourceTopLayer);
            }
        }

        public List<GridEditorModel> GridTiles { get; set; } = new();

        public int[,] TileId
        {
            get => _tileId;
            set => SetProperty(ref _tileId, value);
        }

        public int Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        public int Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public int ImageId
        {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        public event EventHandler<ImageSource> ImageSourceBottomLayerChanged;
        public ImageSource ImageSourceBottomLayer
        {
            get => _imageSourceBottomLayer;
            set
            {
                if (SetProperty(ref _imageSourceBottomLayer, value))
                {
                    ImageSourceBottomLayerChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<ImageSource> ImageSourceTopLayerChanged;
        public ImageSource ImageSourceTopLayer
        {
            get => _imageSourceTopLayer;
            set
            {
                if (SetProperty(ref _imageSourceTopLayer, value))
                {
                    ImageSourceTopLayerChanged?.Invoke(this, value);
                }
            }
        }

        public bool IsCollidable
        {
            get => _isCollidable;
            set => SetProperty(ref _isCollidable, value);
        }

        public int LayerId
        {
            get => _layerId;
            set => SetProperty(ref _layerId, value);
        }

        public void InitTiles(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    GridTiles.Add(new GridEditorModel
                    {
                        TileId = new int[r, c],
                        ImageIdTop = -1,
                        ImageIdBottom = -1,
                        LayerId = -1,
                        IsCollidable = false,
                        ImageSourceBottomLayer = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png"))
                    });
                }
            }
        }

        public void OnTileElementPressed(object? sender, int[,] tileId)
        {
            Debug.WriteLine("Row: " + tileId.GetLength(0) + ", Col: " + tileId.GetLength(1));

            var tileOnGrid = GridTiles.Find(
                x => x.TileId.GetLength(0) == tileId.GetLength(0) && x.TileId.GetLength(1) == tileId.GetLength(1));

            if (tileOnGrid is null) return;

            tileOnGrid.IsCollidable = IsCollidable;
            tileOnGrid.LayerId = LayerId;

            if (LayerId == 0)
            {
                tileOnGrid.ImageSourceBottomLayer = ImageSourceBottomLayer;
                tileOnGrid.ImageIdBottom = ImageId;
            }
            else
            {
                tileOnGrid.ImageSourceTopLayer = ImageSourceTopLayer;
                tileOnGrid.ImageIdTop = ImageId;
            }

            // Tile is plain grass
            if (tileOnGrid.ImageIdBottom == 15)
            {
                var tileOnGridAbovePressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));
                var tileOnGridBelowPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));

                var tileOnGridAboveLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) -1);
                var tileOnGridAboveRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) +1);

                var tileOnGridBelowLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1);
                var tileOnGridBelowRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1);

                var tileOnGridRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));
                var tileOnGridLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));

                if (tileOnGridAbovePressedTile is { ImageIdBottom: not 15 and not 29 }) tileOnGridAbovePressedTile.ImageIdBottom = 1;
                if (tileOnGridAbovePressedTile is { ImageIdBottom: 29 }) tileOnGridAbovePressedTile.ImageIdBottom = 15;

                if (tileOnGridBelowPressedTile is { ImageIdBottom: not 15 and not 1}) tileOnGridBelowPressedTile.ImageIdBottom = 29;
                if (tileOnGridBelowPressedTile is { ImageIdBottom: 1 }) tileOnGridBelowPressedTile.ImageIdBottom = 15;

                if (tileOnGridRightOfPressedTile is { ImageIdBottom: not 15 and not 16 }) tileOnGridRightOfPressedTile.ImageIdBottom = 14;
                if (tileOnGridRightOfPressedTile is { ImageIdBottom: 16 }) tileOnGridRightOfPressedTile.ImageIdBottom = 15;

                if (tileOnGridLeftOfPressedTile is { ImageIdBottom: not 15 and not 14 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 16;
                if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 14 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 15;

                if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: -1 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 0;
                if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: -1 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 2;

                if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: -1 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 28;
                if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: -1 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 30;
            }
        }

        public void OnSelectedTileChanged(object? sender, Tile tile)
        {
            ImageId = tile.ImageId;
            ImageSourceBottomLayer = tile.ImageSource;
        }

        public void OnSelectedTileCollidableChanged(object? sender, bool collidable)
        {
            IsCollidable = collidable;
        }

        public void OnSelectedTileLayerChanged(object? sender, int layerId)
        {
            LayerId = layerId;
            if (layerId == 1)
            {
                ImageSourceTopLayer = null;
                ImageSourceTopLayer = ImageSourceBottomLayer;
            }
        }


        public void OnFillEmptyGridSpaceWithSelectedTile(object? sender, Tile tileFromTileViewModel)
        {
            foreach (var tile in GridTiles)
            {
                if (tileFromTileViewModel.LayerId == 0)
                {
                    if (tile.ImageIdBottom != -1) continue;
                    //tile.LayerId = tileFromTileViewModel.LayerId; ------------->useless?
                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdBottom = tileFromTileViewModel.ImageId;
                    tile.ImageSourceBottomLayer = tileFromTileViewModel.ImageSource;
                }
                else
                {
                    if (tile.ImageIdTop != -1) continue;
                    //tile.LayerId = tileFromTileViewModel.LayerId;  ------------->useless?
                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdTop = tileFromTileViewModel.ImageId;
                    tile.ImageSourceTopLayer = tileFromTileViewModel.ImageSource;
                }
            }
        }
    }
}