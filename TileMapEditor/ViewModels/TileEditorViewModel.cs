using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class TileEditorViewModel : ObservableObject
    {
        public TileEditorViewModel()
        {
            InitTileset(8, 8);
        }

        private string _tileSetPath = AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Tileset_1_64x64.png";

        private int _rows;
        private int _columns;

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

        public List<Models.Tile> Tiles { get; set; } = new();

        private void InitTileset(int rows, int cols)
        {
            var tileSetBitmapImage = new BitmapImage(new Uri(_tileSetPath));
            var img = new Image{Width = 64, Height = 64};
            //img.MouseDown += new MouseButtonEventHandler()
            var cb = new CroppedBitmap(tileSetBitmapImage, new Int32Rect(0, 0, 64, 64));
            
            var tile = new Models.Tile { Background = tileSetBitmapImage, CroppedBitmap = cb, Data = "hej"};

            int xPositionCropped = 0;
            int yPositionCropped = 0;

            Rows = rows;
            Columns = cols;
            int imageIdCounter = 0;

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    Tiles.Add(new Models.Tile
                    {
                        TileId = new int[r, c],
                        ImageId = imageIdCounter,
                        CroppedBitmap = new CroppedBitmap(tileSetBitmapImage, 
                            new Int32Rect(xPositionCropped * 64, yPositionCropped * 64, 64,64))
                    });

                    imageIdCounter++;
                    yPositionCropped++;
                    if (xPositionCropped == 1 && yPositionCropped is >= 2 and <= 4) xPositionCropped++;
                    if (xPositionCropped != 5 && yPositionCropped == 2) yPositionCropped++;

                    while (xPositionCropped == 11 && yPositionCropped is 2 or 3 or 4) yPositionCropped++;
                    while (xPositionCropped == 13 && yPositionCropped is 1 or 2) yPositionCropped++;

                    if (yPositionCropped > 4)
                    {
                        xPositionCropped++;
                        if (xPositionCropped is 4 or 6 or 17) xPositionCropped++;
                        yPositionCropped = 0;
                    }

                    while (xPositionCropped == 10 && yPositionCropped is 0 or 1 or 2) yPositionCropped++;
                    while (xPositionCropped == 12 && yPositionCropped is 0 or 1 or 2) yPositionCropped++;
                    while (xPositionCropped == 18 && yPositionCropped is 0 or 1 or 2) yPositionCropped++;
                    while (xPositionCropped == 19 && yPositionCropped is 0 or 1 or 2) yPositionCropped++;

                    if (xPositionCropped >= 20)
                    {
                        c = 8;
                        r = 8;
                    }
                }
            }

            Swap(Tiles, 6, 12);
            Swap(Tiles, 7, 13);
            Swap(Tiles, 16, 23);
            Swap(Tiles, 19, 25);
            Swap(Tiles, 20, 26);
            Swap(Tiles, 33, 42);
            Swap(Tiles, 34, 43);
            Swap(Tiles, 36, 46);
            Swap(Tiles, 37, 47);
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }
    }
}