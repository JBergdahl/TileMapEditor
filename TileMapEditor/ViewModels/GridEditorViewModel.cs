using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class GridEditorViewModel : ObservableRecipient
    {
        private const int Empty = -1;
        private const int TopLeftCoast = 0;
        private const int TopCoast = 1;
        private const int TopRightCoast = 2;
        private const int LeftCoast = 14;
        private const int PlainGrass = 15;
        private const int RightCoast = 16;
        private const int BottomLeftCoast = 28;
        private const int BottomCoast = 29;
        private const int BottomRightCoast = 30;
        private const int Water = 31;
        private const int BottomRightCornerCoast = 3;
        private const int BottomLeftCornerCoast = 4;
        private const int TopRightCornerCoast = 17;
        private const int TopLeftCornerCoast = 18;

        private int _columns;
        private int _imageId;
        private ImageSource _imageSourceBottomLayer;
        private ImageSource _imageSourceTopLayer;
        private bool _isCollidable;
        private int _layerId;
        private int _rows;

        public GridEditorViewModel(int rows, int cols)
        {
            InitTiles(rows, cols);
        }

        public ObservableCollection<MapTile> MapTiles { get; set; } = new();

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

        public event EventHandler<ImageSource> ImageSourceBottomLayerChanged;
        public event EventHandler<ImageSource> ImageSourceTopLayerChanged;
        public event EventHandler<List<MapTile>> UpdateNeighbourTiles;

        public void InitTiles(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            for (var r = 0; r < rows; r++)
            for (var c = 0; c < columns; c++)
            {
                MapTiles.Add(new MapTile
                {
                    TilePositionOnGrid = new Point(r, c),
                    ImageIdTop = -1,
                    ImageIdBottom = -1,
                    IsCollidable = false,
                    ImageSourceBottomLayer =
                        new BitmapImage(new Uri(AssetStructure.EmptyPngPath))
                });
            }
        }

        public void OnTileElementPressed(object? sender, Point tilePositionOnGrid)
        {
            Debug.WriteLine("Position: " + tilePositionOnGrid);

            var tileOnGrid = MapTiles.FirstOrDefault(
                x => x.TilePositionOnGrid.X.Equals(tilePositionOnGrid.X) &&
                     x.TilePositionOnGrid.Y.Equals(tilePositionOnGrid.Y));

            CollectionViewSource.GetDefaultView(MapTiles).Refresh();

            if (tileOnGrid == null)
            {
                return;
            }

            if (!tileOnGrid.IsCollidable)
            {
                tileOnGrid.IsCollidable = IsCollidable;
            }

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

            // Tile is plain grass + using standard Tile set
            if (tileOnGrid.ImageIdBottom == 15 && LayerId == 0)
            {
                UpdateTileNeighbours(tileOnGrid);
            }
        }

        private void UpdateTileNeighbours(MapTile tileOnGrid)
        {
            var listOfNeighbourTiles = new List<MapTile>();

            var tileOnGridAbovePressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y));

            var tileOnGridBelowPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y));

            var tileOnGridAboveLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            var tileOnGridAboveRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridBelowLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            var tileOnGridBelowRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            UpdateTilesAbovePressedTile(tileOnGridAbovePressedTile);
            listOfNeighbourTiles.Add(tileOnGridAbovePressedTile);

            UpdateTilesBelowPressedTile(tileOnGridBelowPressedTile);
            listOfNeighbourTiles.Add(tileOnGridBelowPressedTile);

            UpdateTilesRightOfPressedTile(tileOnGridRightOfPressedTile);
            listOfNeighbourTiles.Add(tileOnGridRightOfPressedTile);

            UpdateTilesLeftOfPressedTile(tileOnGridLeftOfPressedTile);
            listOfNeighbourTiles.Add(tileOnGridLeftOfPressedTile);

            UpdateTileAboveLeftOfPressedTile(tileOnGridAboveLeftOfPressedTile);
            listOfNeighbourTiles.Add(tileOnGridAboveLeftOfPressedTile);

            UpdateTileAboveRightOfPressedTile(tileOnGridAboveRightOfPressedTile);
            listOfNeighbourTiles.Add(tileOnGridAboveRightOfPressedTile);

            UpdateTileBelowLeftOfPressedTile(tileOnGridBelowLeftOfPressedTile);
            listOfNeighbourTiles.Add(tileOnGridBelowLeftOfPressedTile);

            UpdateTileBelowRightOfPressedTile(tileOnGridBelowRightOfPressedTile);
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
            foreach (var tile in MapTiles)
            {
                if (tileFromTileViewModel.LayerId == 0)
                {
                    if (tile.ImageIdBottom != -1)
                    {
                        continue;
                    }

                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdBottom = tileFromTileViewModel.ImageId;
                    tile.ImageSourceBottomLayer = tileFromTileViewModel.ImageSource;
                }
                else
                {
                    if (tile.ImageIdTop != -1)
                    {
                        continue;
                    }

                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdTop = tileFromTileViewModel.ImageId;
                    tile.ImageSourceTopLayer = tileFromTileViewModel.ImageSource;
                }
            }

            CollectionViewSource.GetDefaultView(MapTiles).Refresh();
        }

        public void OnTileElementRightPressed(object? sender, Point tilePositionOnGrid)
        {
            var tileOnGrid = MapTiles.FirstOrDefault(
                x => x.TilePositionOnGrid.X.Equals(tilePositionOnGrid.X) &&
                     x.TilePositionOnGrid.Y.Equals(tilePositionOnGrid.Y));


            CollectionViewSource.GetDefaultView(MapTiles).Refresh();

            if (tileOnGrid is null)
            {
                return;
            }

            var originalIdBottom = tileOnGrid.ImageIdBottom;
            var originalIdTop = tileOnGrid.ImageIdTop;

            if (tileOnGrid.ImageSourceTopLayer != null)
            {
                tileOnGrid.ImageSourceTopLayer = null;
                tileOnGrid.ImageIdTop = -1;
            }
            else if (tileOnGrid.ImageSourceBottomLayer != null)
            {
                tileOnGrid.ImageSourceBottomLayer = new BitmapImage(new Uri(AssetStructure.EmptyPngPath));
                tileOnGrid.ImageIdBottom = -1;
            }

            tileOnGrid.IsCollidable = false;

            // Tile is plain grass
            if (originalIdBottom == 15 && originalIdTop == -1)
            {
                UpdateRemoveTileNeighbours(tileOnGrid);
            }
        }

        private void UpdateRemoveTileNeighbours(MapTile tileOnGrid)
        {
            var listOfNeighbourTiles = new List<MapTile>();

            var tileOnGridAbovePressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y));

            var tileOnGridBelowPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y));

            var tileOnGridAboveLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            var tileOnGridAboveRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X - 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridBelowLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            var tileOnGridBelowRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X + 1) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridRightOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y + 1));

            var tileOnGridLeftOfPressedTile = MapTiles.FirstOrDefault(x =>
                x.TilePositionOnGrid.X.Equals(tileOnGrid.TilePositionOnGrid.X) &&
                x.TilePositionOnGrid.Y.Equals(tileOnGrid.TilePositionOnGrid.Y - 1));

            if (tileOnGridAbovePressedTile is { })
            {
                tileOnGridAbovePressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridAbovePressedTile);

            if (tileOnGridBelowPressedTile is { })
            {
                tileOnGridBelowPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridBelowPressedTile);

            if (tileOnGridAboveLeftOfPressedTile is { })
            {
                tileOnGridAboveLeftOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridAboveLeftOfPressedTile);

            if (tileOnGridAboveRightOfPressedTile is { })
            {
                tileOnGridAboveRightOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridAboveRightOfPressedTile);

            if (tileOnGridBelowLeftOfPressedTile is { })
            {
                tileOnGridBelowLeftOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridBelowLeftOfPressedTile);

            if (tileOnGridBelowRightOfPressedTile is { })
            {
                tileOnGridBelowRightOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridBelowRightOfPressedTile);

            if (tileOnGridRightOfPressedTile is { })
            {
                tileOnGridRightOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridRightOfPressedTile);

            if (tileOnGridLeftOfPressedTile is { })
            {
                tileOnGridLeftOfPressedTile.ImageIdBottom = Empty;
            }

            listOfNeighbourTiles.Add(tileOnGridLeftOfPressedTile);

            foreach (var tile in listOfNeighbourTiles.Where(tile => tile is { }))
            {
                tile.ImageSourceBottomLayer =
                    new BitmapImage(new Uri(AssetStructure.EmptyPngPath));
            }

            UpdateNeighbourTiles?.Invoke(this, listOfNeighbourTiles);
        }

        private static void UpdateTilesAbovePressedTile(MapTile above)
        {
            switch (above)
            {
                case { ImageIdBottom: Water or Empty or TopRightCoast or TopLeftCoast }:
                    above.ImageIdBottom = TopCoast;
                    break;
                case { ImageIdBottom: BottomRightCornerCoast }:
                    above.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: BottomLeftCornerCoast }:
                    above.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: BottomLeftCoast }:
                    above.ImageIdBottom = TopLeftCornerCoast;
                    break;
                case { ImageIdBottom: BottomRightCoast }:
                    above.ImageIdBottom = TopRightCornerCoast;
                    break;
                case { ImageIdBottom: BottomCoast }:
                    above.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: LeftCoast }:
                    above.ImageIdBottom = TopLeftCornerCoast;
                    break;
                case { ImageIdBottom: RightCoast }:
                    above.ImageIdBottom = TopRightCornerCoast;
                    break;
            }
        }

        private static void UpdateTilesBelowPressedTile(MapTile below)
        {
            switch (below)
            {
                case { ImageIdBottom: Water or Empty or BottomRightCoast or BottomLeftCoast }:
                    below.ImageIdBottom = BottomCoast;
                    break;
                case { ImageIdBottom: TopLeftCornerCoast }:
                    below.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopRightCornerCoast }:
                    below.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopCoast }:
                    below.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: LeftCoast }:
                    below.ImageIdBottom = BottomLeftCornerCoast;
                    break;
                case { ImageIdBottom: RightCoast }:
                    below.ImageIdBottom = BottomRightCornerCoast;
                    break;
                case { ImageIdBottom: TopLeftCoast }:
                    below.ImageIdBottom = BottomLeftCornerCoast;
                    break;
                case { ImageIdBottom: TopRightCoast }:
                    below.ImageIdBottom = BottomRightCornerCoast;
                    break;
            }
        }

        private static void UpdateTilesRightOfPressedTile(MapTile right)
        {
            switch (right)
            {
                case { ImageIdBottom: Water or Empty or TopRightCoast or BottomRightCoast }:
                    right.ImageIdBottom = RightCoast;
                    break;
                case { ImageIdBottom: BottomLeftCornerCoast }:
                    right.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopLeftCornerCoast }:
                    right.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: LeftCoast }:
                    right.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopCoast }:
                    right.ImageIdBottom = TopRightCornerCoast;
                    break;
                case { ImageIdBottom: BottomCoast }:
                    right.ImageIdBottom = BottomRightCornerCoast;
                    break;
                case { ImageIdBottom: BottomLeftCoast }:
                    right.ImageIdBottom = BottomRightCornerCoast;
                    break;
                case { ImageIdBottom: TopLeftCoast }:
                    right.ImageIdBottom = TopRightCornerCoast;
                    break;
            }
        }

        private static void UpdateTilesLeftOfPressedTile(MapTile left)
        {
            switch (left)
            {
                case { ImageIdBottom: Water or Empty or TopLeftCoast or BottomLeftCoast }:
                    left.ImageIdBottom = LeftCoast;
                    break;
                case { ImageIdBottom: BottomRightCornerCoast }:
                    left.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopRightCornerCoast }:
                    left.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: RightCoast }:
                    left.ImageIdBottom = PlainGrass;
                    break;
                case { ImageIdBottom: TopCoast }:
                    left.ImageIdBottom = TopLeftCornerCoast;
                    break;
                case { ImageIdBottom: BottomCoast }:
                    left.ImageIdBottom = BottomLeftCornerCoast;
                    break;
                case { ImageIdBottom: TopRightCoast }:
                    left.ImageIdBottom = TopLeftCornerCoast;
                    break;
                case { ImageIdBottom: BottomRightCoast }:
                    left.ImageIdBottom = BottomLeftCornerCoast;
                    break;
            }
        }

        private static void UpdateTileAboveLeftOfPressedTile(MapTile aboveLeft)
        {
            switch (aboveLeft)
            {
                case { ImageIdBottom: Empty or Water }:
                    aboveLeft.ImageIdBottom = TopLeftCoast;
                    break;
                case { ImageIdBottom: BottomCoast }:
                    aboveLeft.ImageIdBottom = BottomLeftCornerCoast;
                    break;
                case { ImageIdBottom: BottomLeftCoast }:
                    aboveLeft.ImageIdBottom = LeftCoast;
                    break;
                case { ImageIdBottom: TopRightCoast }:
                    aboveLeft.ImageIdBottom = TopCoast;
                    break;
                case { ImageIdBottom: RightCoast }:
                    aboveLeft.ImageIdBottom = TopRightCornerCoast;
                    break;
            }
        }

        private static void UpdateTileAboveRightOfPressedTile(MapTile aboveRight)
        {
            switch (aboveRight)
            {
                case { ImageIdBottom: Empty or Water }:
                    aboveRight.ImageIdBottom = TopRightCoast;
                    break;
                case { ImageIdBottom: BottomCoast }:
                    aboveRight.ImageIdBottom = BottomRightCornerCoast;
                    break;
                case { ImageIdBottom: BottomRightCoast }:
                    aboveRight.ImageIdBottom = RightCoast;
                    break;
                case { ImageIdBottom: TopLeftCoast }:
                    aboveRight.ImageIdBottom = TopCoast;
                    break;
                case { ImageIdBottom: LeftCoast }:
                    aboveRight.ImageIdBottom = TopLeftCornerCoast;
                    break;
            }
        }

        private static void UpdateTileBelowLeftOfPressedTile(MapTile belowLeft)
        {
            switch (belowLeft)
            {
                case { ImageIdBottom: Empty or Water }:
                    belowLeft.ImageIdBottom = BottomLeftCoast;
                    break;
                case { ImageIdBottom: TopCoast }:
                    belowLeft.ImageIdBottom = TopLeftCornerCoast;
                    break;
                case { ImageIdBottom: TopLeftCoast }:
                    belowLeft.ImageIdBottom = LeftCoast;
                    break;
                case { ImageIdBottom: BottomRightCoast }:
                    belowLeft.ImageIdBottom = BottomCoast;
                    break;
                case { ImageIdBottom: RightCoast }:
                    belowLeft.ImageIdBottom = BottomRightCornerCoast;
                    break;
            }
        }

        private static void UpdateTileBelowRightOfPressedTile(MapTile belowRight)
        {
            switch (belowRight)
            {
                case { ImageIdBottom: Empty or Water }:
                    belowRight.ImageIdBottom = BottomRightCoast;
                    break;
                case { ImageIdBottom: TopCoast }:
                    belowRight.ImageIdBottom = TopRightCornerCoast;
                    break;
                case { ImageIdBottom: TopRightCoast }:
                    belowRight.ImageIdBottom = RightCoast;
                    break;
                case { ImageIdBottom: BottomLeftCoast }:
                    belowRight.ImageIdBottom = BottomCoast;
                    break;
                case { ImageIdBottom: LeftCoast }:
                    belowRight.ImageIdBottom = BottomLeftCornerCoast;
                    break;
            }
        }
    }
}