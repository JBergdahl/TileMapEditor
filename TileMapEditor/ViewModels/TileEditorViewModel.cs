using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class TileEditorViewModel : ObservableObject
    {
        private int _columns;
        private string _imagePath;
        private int _rows;
        private int _tileHeight;
        private int _tileWidth;

        public TileEditorViewModel(int rows, int cols, int tileWidth, int tileHeight, string tileMap)
        {
            InitTileSet(rows, cols, tileWidth, tileHeight, tileMap);
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

        public int TileWidth
        {
            get => _tileWidth;
            set => SetProperty(ref _tileWidth, value);
        }

        public int TileHeight
        {
            get => _tileHeight;
            set => SetProperty(ref _tileHeight, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public List<Tile> Tiles { get; set; } = new();

        private void InitTileSet(int rows, int cols, int width, int height, string imagePath)
        {
            var tileSetBitmapImage = new BitmapImage(new Uri(imagePath));
            var xPositionCropped = 0;
            var yPositionCropped = 0;
            var imageIdCounter = 0;

            Rows = rows;
            Columns = cols;
            TileWidth = width;
            TileHeight = height;
            ImagePath = imagePath;

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    Tiles.Add(new Tile
                    {
                        TileId = new int[r, c],
                        ImageId = imageIdCounter,
                        ImagePath = imagePath,
                        CroppedTileSetImage = new CroppedBitmap(tileSetBitmapImage,
                            new Int32Rect(xPositionCropped * width, yPositionCropped * height, width, height))
                    });
                    imageIdCounter++;
                    xPositionCropped++;
                    if (xPositionCropped > tileSetBitmapImage.Width / width - 1)
                    {
                        yPositionCropped++;
                        xPositionCropped = 0;
                    }
                }

                if (yPositionCropped > tileSetBitmapImage.Height / height - 1) break;
            }
        }
    }
}