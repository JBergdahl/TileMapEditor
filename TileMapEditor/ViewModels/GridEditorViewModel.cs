using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        public event EventHandler<List<GridEditorModel>> UpdateNeighbourTiles;
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
            if (tileOnGrid.ImageIdBottom == 15 && LayerId == 0)
            {
                UpdateTileNeighbours(tileOnGrid);
            }
        }

        private void UpdateTileNeighbours(GridEditorModel tileOnGrid)
        {
            var listOfNeighbourTiles = new List<GridEditorModel>();

            var tileOnGridAbovePressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));
            var tileOnGridBelowPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));

            var tileOnGridAboveLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1);
            var tileOnGridAboveRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1);

            var tileOnGridBelowLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1);
            var tileOnGridBelowRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1);

            var tileOnGridRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));
            var tileOnGridLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));



            if (tileOnGridAbovePressedTile is { ImageIdBottom: not 3 and not 4 and not 14 and not 15 and not 16 and not 29 }) tileOnGridAbovePressedTile.ImageIdBottom = 1;
            if (tileOnGridAbovePressedTile is { ImageIdBottom: 3 })
            {
                tileOnGridAbovePressedTile.ImageIdBottom = 15;
                if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: -1 or 28 or 30 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 16;
                if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: 29 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 3;
            }
            if (tileOnGridAbovePressedTile is { ImageIdBottom: 4 })
            {
                tileOnGridAbovePressedTile.ImageIdBottom = 15;
                if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: -1 or 28 or 30 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 14;
                if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: 29 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 4;
            }
            if (tileOnGridAbovePressedTile is { ImageIdBottom: 29 }) tileOnGridAbovePressedTile.ImageIdBottom = 15;
            if (tileOnGridAbovePressedTile is { ImageIdBottom: 14 }) tileOnGridAbovePressedTile.ImageIdBottom = 18;
            if (tileOnGridAbovePressedTile is { ImageIdBottom: 16 }) tileOnGridAbovePressedTile.ImageIdBottom = 17;
            listOfNeighbourTiles.Add(tileOnGridAbovePressedTile);



            if (tileOnGridBelowPressedTile is { ImageIdBottom: not 1 and not 18 and not 17 and not 14 and not 16 and not 15}) tileOnGridBelowPressedTile.ImageIdBottom = 29;
            if (tileOnGridBelowPressedTile is { ImageIdBottom: 18 })
            {
                tileOnGridBelowPressedTile.ImageIdBottom = 15;
                if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: -1 or 0 or 2 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 14;
                if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: 1 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 18;
            }
            if (tileOnGridBelowPressedTile is { ImageIdBottom: 17 })
            {
                tileOnGridBelowPressedTile.ImageIdBottom = 15;
                if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: -1 or 0 or 2 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 16;
                if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: 1 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 17;
            }
            if (tileOnGridBelowPressedTile is { ImageIdBottom: 1 }) tileOnGridBelowPressedTile.ImageIdBottom = 15;
            if (tileOnGridBelowPressedTile is { ImageIdBottom: 14 }) tileOnGridBelowPressedTile.ImageIdBottom = 4;
            if (tileOnGridBelowPressedTile is { ImageIdBottom: 16 }) tileOnGridBelowPressedTile.ImageIdBottom = 3;
            listOfNeighbourTiles.Add(tileOnGridBelowPressedTile);



            if (tileOnGridRightOfPressedTile is { ImageIdBottom: not 4 and not 18 and not 15 and not 14 and not 1 and not 29 }) tileOnGridRightOfPressedTile.ImageIdBottom = 16;
            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 4 })
            {
                tileOnGridRightOfPressedTile.ImageIdBottom = 15;
                if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: -1 or 28 or 31 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 29;
                if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: 14 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 4;
            }
            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 18 })
            {
                tileOnGridRightOfPressedTile.ImageIdBottom = 15;
                if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: -1 or 0 or 2 or 31 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 1;
                if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: 14 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 18;
            }
            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 14 }) tileOnGridRightOfPressedTile.ImageIdBottom = 15;
            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 1 }) tileOnGridRightOfPressedTile.ImageIdBottom = 17;
            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 29 }) tileOnGridRightOfPressedTile.ImageIdBottom = 3;
            listOfNeighbourTiles.Add(tileOnGridRightOfPressedTile);



            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: not 3 and not 15 and not 16 and not 17 and not 1 and not 29 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 14;
            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 3 })
            {
                tileOnGridLeftOfPressedTile.ImageIdBottom = 15;
                if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: -1 or 30 or 31 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 29;
                if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: 16 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 3;
            }
            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 17 })
            {
                tileOnGridLeftOfPressedTile.ImageIdBottom = 15;
                if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: -1 or 0 or 2 or 31 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 1;
                if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: 16 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 17;
            }
            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 16 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 15;
            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 1 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 18;
            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 29 }) tileOnGridLeftOfPressedTile.ImageIdBottom = 4;
            listOfNeighbourTiles.Add(tileOnGridLeftOfPressedTile);



            if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: -1 or 31 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = 0;
            listOfNeighbourTiles.Add(tileOnGridAboveLeftOfPressedTile);

            if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: -1 or 31 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = 2;
            listOfNeighbourTiles.Add(tileOnGridAboveRightOfPressedTile);

            if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: -1 or 31 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = 28;
            listOfNeighbourTiles.Add(tileOnGridBelowLeftOfPressedTile);

            if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: -1 or 31 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = 30;
            listOfNeighbourTiles.Add(tileOnGridBelowRightOfPressedTile);

            UpdateNeighbourTiles?.Invoke(this, listOfNeighbourTiles);
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

        public void OnTileElementRightPressed(object? sender, int[,] tileId)
        {
            var tileOnGrid = GridTiles.Find(
                x => x.TileId.GetLength(0) == tileId.GetLength(0) && x.TileId.GetLength(1) == tileId.GetLength(1));

            if (tileOnGrid is null) return;

            var originalIdBorrom = tileOnGrid.ImageIdBottom;
            var originalIdTop = tileOnGrid.ImageIdTop;

            if (tileOnGrid.ImageSourceTopLayer != null)
            {
                tileOnGrid.ImageSourceTopLayer = null;
                tileOnGrid.ImageIdTop = -1;
            }
            else if (tileOnGrid.ImageSourceBottomLayer != null)
            {
                tileOnGrid.ImageSourceBottomLayer = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png"));
                tileOnGrid.ImageIdBottom = -1;
            }

            tileOnGrid.IsCollidable = false;

            // Tile is plain grass
            if (originalIdBorrom == 15 && originalIdTop == -1)
            {
                UpdateRemoveTileNeighbours(tileOnGrid);
            }
        }

        private void UpdateRemoveTileNeighbours(GridEditorModel tileOnGrid)
        {
            var listOfNeighbourTiles = new List<GridEditorModel>();

            var tileOnGridAbovePressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));
            var tileOnGridBelowPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1));

            var tileOnGridAboveLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1);
            var tileOnGridAboveRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) - 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1);

            var tileOnGridBelowLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1);
            var tileOnGridBelowRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0) + 1 && x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1);

            var tileOnGridRightOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) + 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));
            var tileOnGridLeftOfPressedTile = GridTiles.Find(x => x.TileId.GetLength(1) == tileOnGrid.TileId.GetLength(1) - 1 && x.TileId.GetLength(0) == tileOnGrid.TileId.GetLength(0));

            if (tileOnGridAbovePressedTile is { ImageIdBottom: 1 }) tileOnGridAbovePressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridAbovePressedTile);

            if (tileOnGridBelowPressedTile is { ImageIdBottom: 29 }) tileOnGridBelowPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridBelowPressedTile);

            if (tileOnGridAboveLeftOfPressedTile is { ImageIdBottom: 0 }) tileOnGridAboveLeftOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridAboveLeftOfPressedTile);

            if (tileOnGridAboveRightOfPressedTile is { ImageIdBottom: 2 }) tileOnGridAboveRightOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridAboveRightOfPressedTile);

            if (tileOnGridBelowLeftOfPressedTile is { ImageIdBottom: 28 }) tileOnGridBelowLeftOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridBelowLeftOfPressedTile);

            if (tileOnGridBelowRightOfPressedTile is { ImageIdBottom: 30 }) tileOnGridBelowRightOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridBelowRightOfPressedTile);

            if (tileOnGridRightOfPressedTile is { ImageIdBottom: 16 }) tileOnGridRightOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridRightOfPressedTile);

            if (tileOnGridLeftOfPressedTile is { ImageIdBottom: 14 }) tileOnGridLeftOfPressedTile.ImageIdBottom = -1;
            listOfNeighbourTiles.Add(tileOnGridLeftOfPressedTile);

            foreach (var tile in listOfNeighbourTiles)
            {
                tile.ImageSourceBottomLayer = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png"));
            }

            UpdateNeighbourTiles?.Invoke(this, listOfNeighbourTiles);
        }
    }
}