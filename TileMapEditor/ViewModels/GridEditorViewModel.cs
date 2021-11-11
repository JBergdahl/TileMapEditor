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

        public GridEditorViewModel()
        {
            InitTiles(8,11);
            PrintInfo = new RelayCommand(PrintTileInfo);
        }

        public RelayCommand PrintInfo { get; set; }

        private void PrintTileInfo()
        {
            foreach (var tile in GridTiles)
            {
                Debug.WriteLine("Row: " + tile.TileId.GetLength(0) + ", Col: " + tile.TileId.GetLength(1) + ", ID: " + tile.ImageId);
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

        public ImageSource ImageSourceFromTileViewModel
        {
            get => _imageSourceFromTileViewModel;
            set => SetProperty(ref _imageSourceFromTileViewModel, value);
        }

        public Tile TileToBePlaced { get; set; } = new();

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
                        ImageId = -1,
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

            if (tileOnGrid is null || TileToBePlaced is null) return;

            //tileOnGrid.ImageId = TileToBePlaced.ImageId;
            tileOnGrid.ImageId = ImageId;
            //tileOnGrid.ImageSourceBottomLayer = ImageSourceBottomLayer;
            //tileOnGrid.IsCollidable = TileToBePlaced.IsCollidable;
            tileOnGrid.IsCollidable = IsCollidable;
            //tileOnGrid.LayerId = TileToBePlaced.LayerId;
            tileOnGrid.LayerId = LayerId;

            if (LayerId == 0)
            {
                tileOnGrid.ImageSourceBottomLayer = ImageSourceBottomLayer;
            }
            else
            {
                tileOnGrid.ImageSourceTopLayer = ImageSourceTopLayer;
            }
        }

        public void OnSelectedTileChanged(object? sender, Tile tile)
        {
            //TileToBePlaced = tile;
            ImageId = tile.ImageId;
            ImageSourceFromTileViewModel = tile.ImageSource;
            ImageSourceBottomLayer = tile.ImageSource;
        }

        public void OnSelectedTileCollidableChanged(object? sender, bool collidable)
        {
            //TileToBePlaced.IsCollidable = collidable;
            IsCollidable = collidable;
        }

        public void OnSelectedTileLayerChanged(object? sender, int layerId)
        {
            //TileToBePlaced.LayerId = layerId;
            LayerId = layerId;
            if (layerId == 1)
            {
                ImageSourceTopLayer = ImageSourceFromTileViewModel;
            }
        }

        public void OnFillEmptyGridSpaceWithSelectedTile(object? sender, Tile tileFromTileViewModel)
        {
            foreach (var tile in GridTiles)
            {
                if (tile.ImageId == -1)
                {
                    tile.LayerId = tileFromTileViewModel.LayerId;
                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageId = tileFromTileViewModel.ImageId;
                    if (LayerId == 0)
                    {
                        tile.ImageSourceBottomLayer = tileFromTileViewModel.ImageSource;
                    }
                    else
                    {
                        tile.ImageSourceTopLayer = tileFromTileViewModel.ImageSource;
                    }
                }
            }
        }
    }
}