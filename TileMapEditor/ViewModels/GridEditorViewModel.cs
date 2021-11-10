using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
        private Tile _tileToBePlaced;

        public GridEditorViewModel()
        {
            InitTiles(8,11);
            PrintInfo = new RelayCommand(PrintTileInfo);
        }

        public RelayCommand PrintInfo { get; set; }

        private void PrintTileInfo()
        {
            foreach (var tile in Tiles)
            {
                Debug.WriteLine("Row: " + tile.TileId.GetLength(0) + ", Col: " + tile.TileId.GetLength(1) + ", ID: " + tile.ImageId);
                Debug.WriteLine("Is collidable: " + tile.IsCollidable + ", Layer ID: " + tile.LayerId);
            }
        }

        public List<Tile> Tiles { get; set; } = new();

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

        public Tile TileToBePlaced { get; set; } = new();

        public void InitTiles(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    Tiles.Add(new Tile
                    {
                        TileId = new int[r, c],
                        ImageId = -1,
                        ImagePath = AppDomain.CurrentDomain.BaseDirectory + "../../../Images/Empty.png"
                    });
                }
            }
        }

        public void OnTileElementPressed(object? sender, int[,] tileId)
        {
            Debug.WriteLine("Row: " + tileId.GetLength(0) + ", Col: " + tileId.GetLength(1));

            var tileOnGrid = Tiles.Find(
                x => x.TileId.GetLength(0) == tileId.GetLength(0) && x.TileId.GetLength(1) == tileId.GetLength(1));

            if (tileOnGrid is null || TileToBePlaced is null) return;

            tileOnGrid.ImageId = TileToBePlaced.ImageId;
            tileOnGrid.ImagePath = TileToBePlaced.ImagePath;
            tileOnGrid.IsCollidable = TileToBePlaced.IsCollidable;
            tileOnGrid.LayerId = TileToBePlaced.LayerId;
        }

        public void OnSelectedTileChanged(object? sender, Tile tile)
        {
            TileToBePlaced = tile;
        }

        public void OnSelectedTileCollidableChanged(object? sender, bool collidable)
        {
            TileToBePlaced.IsCollidable = collidable;
        }

        public void OnSelectedTileLayerChanged(object? sender, int layerId)
        {
            TileToBePlaced.LayerId = layerId;
        }
    }
}