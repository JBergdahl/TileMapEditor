using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace TileMapEditor.ViewModels
{
    public class GridEditorViewModel : ObservableRecipient
    {
        private int _rows;
        private int _columns;
        private Models.Tile _selectedImage;

        public GridEditorViewModel()
        {
            InitTiles(8,11);
            PrintInfo = new RelayCommand(gogo);
        }

        public RelayCommand PrintInfo { get; set; }

        private void gogo()
        {
            for (var i = 0; i < Tiles.Count; i++)
            {
                Debug.WriteLine("Row: " + Tiles[i].TileId.GetLength(0) + ", Col: " + Tiles[i].TileId.GetLength(1) + ", ID: " + Tiles[i].ImageId);
            }
        }

        public List<Models.Tile> Tiles { get; set; } = new();

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

        public Models.Tile SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value);
        }

        public void InitTiles(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    Tiles.Add(new Models.Tile
                    {
                        TileId = new int[r, c],
                        ImageId = -1,
                        Image = " ",
                        Data = string.Format("Row {0}\nColumn {1}", r, c),
                        Background = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Bild.png")),
                        BackgroundSource = new CroppedBitmap(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png")), Int32Rect.Empty)
                    });
                }
            }
        }
        private string _tileSetPath = AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Tileset_1_64x64.png";
        public void OnTileElementPressed(object? sender, int[,] tileId)
        {
            Debug.WriteLine("Row: " + tileId.GetLength(0) + ", Col: " + tileId.GetLength(1));

            var tile = Tiles.Find(
                x => x.TileId.GetLength(0) == tileId.GetLength(0) && x.TileId.GetLength(1) == tileId.GetLength(1));

            if (tile is not null && SelectedImage is not null)
            {
                tile.ImageId = SelectedImage.ImageId;
                tile.BackgroundSource = SelectedImage.CroppedBitmap;

                tile.CroppedBitmap = SelectedImage.CroppedBitmap;
                tile.CroppedBitmap.Source = SelectedImage.CroppedBitmap.Source;
                tile.CroppedBitmap.SourceRect = SelectedImage.CroppedBitmap.SourceRect;

                tile.ImageTest = SelectedImage.ImageTest;
                tile.Image = SelectedImage.Image;
                Debug.WriteLine(tile.ImageId);
            }
        }

        public void OnSelectedCroppedImageChanged(object? sender, Models.Tile tile)
        {
            SelectedImage = tile;
        }
    }
}