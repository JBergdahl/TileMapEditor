﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using TileMapEditor.Models;
using TileMapEditor.Views;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TileMapEditor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private GridEditorViewModel _gridEditorViewModel;
        private TileEditorViewModel _tileEditorViewModel;
        private TileViewModel _tileViewModel;
        private bool _isShowingCollision;

        public MainWindowViewModel()
        {
            CreateStandardTileset();
            GridEditorViewModel = new GridEditorViewModel(15, 20);
            TileEditorViewModel = new TileEditorViewModel(10, 14, 16, 16, StandardTilesetPath);
            TileViewModel = new TileViewModel();
            ExportGridCommand = new RelayCommand(OnExportGridCommand);
            GridCollisionCommand = new RelayCommand(OnGridCollisionCommand);
            ImportGridCommand = new RelayCommand(OnImportGridCommand);
            NewTilesetCommand = new RelayCommand(OnNewTilesetCommand);
            NewGridCommand = new RelayCommand(OnNewGridCommand);
            IsShowingCollisionPressed += TileViewModel.OnIsShowingCollisionPressed;
            CollisionButtonBackground = new SolidColorBrush(Colors.DarkSeaGreen);
        }

        public RelayCommand ExportGridCommand { get; set; }
        public RelayCommand GridCollisionCommand { get; set; }
        public RelayCommand ImportGridCommand { get; set; }
        public RelayCommand NewTilesetCommand { get; set; }
        public RelayCommand NewGridCommand { get; set; }

        private void CreateStandardTileset()
        {
            var localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(localAppFolder + "\\TileMapEditor");
            Directory.CreateDirectory(localAppFolder + "\\TileMapEditor\\Tilesets");
            var path = localAppFolder + "\\TileMapEditor\\Tilesets\\tilemap.png";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File is in use! Use another image or try again");
                    return;
                }
            }
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/tilemap.png", path);
            StandardTilesetPath = path;
        }

        public string StandardTilesetPath
        {
            get => _standardTilesetPath;
            set => SetProperty(ref _standardTilesetPath, value);
        }

        public bool IsShowingCollision
        {
            get => _isShowingCollision;
            set => SetProperty(ref _isShowingCollision, value);
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

        public int GridRows
        {
            get => _gridRows;
            set => SetProperty(ref _gridRows, value);
        }

        public int GridColumns
        {
            get => _gridColumns;
            set => SetProperty(ref _gridColumns, value);
        }

        public SolidColorBrush CollisionButtonBackground
        {
            get => _collisionButtonBackground;
            set => SetProperty(ref _collisionButtonBackground, value);
        }

        private void OnNewTilesetCommand()
        {
            if (TileHeight is 0 || TileWidth is 0) return;
            var result = MessageBox.Show("Do you want to save current map?",
                $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel) return;

            if(result == MessageBoxResult.Yes) OnExportGridCommand();

            var op = new OpenFileDialog
            {
                Title = "Select a tileset",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                 "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                 "Portable Network Graphic (*.png)|*.png"
            };

            var localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (op.ShowDialog() != true) return;
            Directory.CreateDirectory(localAppFolder + "\\TileMapEditor");
            Directory.CreateDirectory(localAppFolder + "\\TileMapEditor\\Tilesets");
            var source = op.FileName;
            var path = localAppFolder + "\\TileMapEditor\\Tilesets\\" + op.SafeFileName;
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File is in use! Use another image or try again");
                    return;
                }
            }
            File.Copy(source, path);
            var image = new BitmapImage(new Uri(path));

            UnsubscribeToOldViewModels?.Invoke(this, 0);
            TileEditorViewModel = new TileEditorViewModel(
                (int)image.Height / TileHeight,
                (int)image.Width / TileWidth,
                TileWidth, TileHeight, path);
            GridEditorViewModel = new GridEditorViewModel(GridEditorViewModel.Rows, GridEditorViewModel.Columns);
            TileViewModel = new TileViewModel();
            SubscribeToNewViewModels?.Invoke(this, 0);
        }

        private void OnNewGridCommand()
        {
            if (GridRows is 0 || GridColumns is 0) return;
            var result = MessageBox.Show("Do you want to save current map?",
                $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel) return;

            if (result == MessageBoxResult.Yes) OnExportGridCommand();

            var image = new BitmapImage(new Uri(TileEditorViewModel.ImagePath));
            UnsubscribeToOldViewModels?.Invoke(this, 0);
            TileEditorViewModel = new TileEditorViewModel(
                (int)image.Height / TileEditorViewModel.TileHeight,
                (int)image.Width / TileEditorViewModel.TileWidth,
                TileEditorViewModel.TileWidth, TileEditorViewModel.TileHeight, TileEditorViewModel.ImagePath);
            GridEditorViewModel = new GridEditorViewModel(GridRows, GridColumns);
            TileViewModel = new TileViewModel();
            SubscribeToNewViewModels?.Invoke(this, 0);
        }

        public event EventHandler<int> UnsubscribeToOldViewModels;
        public event EventHandler<int> SubscribeToNewViewModels;
        private void OnImportGridCommand()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "json files (*.json)|*.json",
                Title = "Open tilemap"
            };

            openFileDialog.ShowDialog();

            if (openFileDialog.FileName == "") return;
            var text = File.ReadAllText(openFileDialog.FileName);
            var mainWindowModel = JsonConvert.DeserializeObject<MainWindowModel>(text);

            if (mainWindowModel is not { } mWm) return;
            UnsubscribeToOldViewModels?.Invoke(this, 0);
            GridEditorViewModel =
                new GridEditorViewModel(mWm.GridHeight, mWm.GridWidth);

            TileEditorViewModel = new TileEditorViewModel(mWm.TilemapHeight / mWm.TileHeight, mWm.TilemapWidth / mWm.TileWidth, mWm.TileWidth,
                mWm.TileHeight, mWm.TileMapPath);

            TileViewModel = new TileViewModel();

            for (var i = 0; i < GridEditorViewModel.GridTiles.Count; i++)
            {
                GridEditorViewModel.GridTiles[i].ImageIdBottom = mWm.GridLayers[0, i];
                if (GridEditorViewModel.GridTiles[i].ImageIdBottom != -1)
                {
                    GridEditorViewModel.GridTiles[i].ImageSourceBottomLayer = TileEditorViewModel.Tiles
                        .Find(x => x.ImageId == GridEditorViewModel.GridTiles[i].ImageIdBottom)
                        ?.CroppedTileSetImage;
                }

                GridEditorViewModel.GridTiles[i].ImageIdTop = mWm.GridLayers[1, i];
                if (GridEditorViewModel.GridTiles[i].ImageIdTop != -1)
                {
                    GridEditorViewModel.GridTiles[i].ImageSourceTopLayer = TileEditorViewModel.Tiles
                        .Find(x => x.ImageId == GridEditorViewModel.GridTiles[i].ImageIdTop)
                        ?.CroppedTileSetImage;
                }

                GridEditorViewModel.GridTiles[i].IsCollidable = mWm.Collidable[i] == 1;
            }

            SubscribeToNewViewModels?.Invoke(this, 0);
        }

        private List<ImageSource> _tempImages = new();
        private int _tileWidth;
        private int _tileHeight;
        private string _standardTilesetPath;
        private SolidColorBrush _collisionButtonBackground;
        private int _gridRows;
        private int _gridColumns;

        public event EventHandler<bool> IsShowingCollisionPressed;
        private void OnGridCollisionCommand()
        {
            IsShowingCollision = !IsShowingCollision;
            IsShowingCollisionPressed?.Invoke(this, !IsShowingCollision);

            if (IsShowingCollision)
            {
                var old = GridEditorViewModel;
                foreach (var tile in old.GridTiles.Where(tile => tile.IsCollidable))
                {
                    _tempImages.Add(tile.ImageSourceTopLayer);
                    _tempImages.Add(tile.ImageSourceBottomLayer);

                    tile.ImageSourceTopLayer =
                        new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory +
                                                "../../../Images/Collision.png"));
                    tile.ImageSourceBottomLayer =
                        new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory +
                                                "../../../Images/Collision.png"));
                }

                CollisionButtonBackground = new SolidColorBrush(Colors.Red);
                GridEditorViewModel = new GridEditorViewModel(old.Rows, old.Columns);
                GridEditorViewModel = old;
            }
            else
            {
                var old = GridEditorViewModel;
                foreach (var tile in old.GridTiles.Where(tile => tile.IsCollidable))
                {
                    tile.ImageSourceTopLayer = _tempImages[0];
                    _tempImages.RemoveAt(0);

                    tile.ImageSourceBottomLayer = _tempImages[0];
                    _tempImages.RemoveAt(0);
                }

                CollisionButtonBackground = new SolidColorBrush(Colors.DarkSeaGreen);
                GridEditorViewModel = new GridEditorViewModel(old.Rows, old.Columns);
                GridEditorViewModel = old;
            }
        }

        private void OnExportGridCommand()
        {
            if (GridEditorViewModel.GridTiles.Count != 0)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    Filter = "json files (*.json)|*.json",
                    Title = "Save tilemap",
                    FileName = $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_"
                };
                var ok = saveFileDialog.ShowDialog();

                var gridTileIsCollidableArray = new int[GridEditorViewModel.GridTiles.Count];
                for (var i = 0; i < GridEditorViewModel.GridTiles.Count; i++)
                {
                    gridTileIsCollidableArray[i] = GridEditorViewModel.GridTiles[i].IsCollidable ? 1 : 0;
                }

                var gridLayers = new int[2, GridEditorViewModel.GridTiles.Count];
                for (var i = 0; i < GridEditorViewModel.GridTiles.Count; i++)
                {
                    gridLayers[0, i] = GridEditorViewModel.GridTiles[i].ImageIdBottom;
                }
                for (var i = 0; i < GridEditorViewModel.GridTiles.Count; i++)
                {
                    gridLayers[1, i] = GridEditorViewModel.GridTiles[i].ImageIdTop;
                }

                if (saveFileDialog.FileName != "" && ok is true)
                {
                    var fs = (FileStream)saveFileDialog.OpenFile();

                    var image = new BitmapImage(new Uri(TileEditorViewModel.ImagePath));

                    MainWindowModel test = new MainWindowModel
                    {
                        Name = saveFileDialog.SafeFileName,
                        TilemapHeight = (int)image.Height,
                        TilemapWidth = (int)image.Width,
                        Collidable = gridTileIsCollidableArray,
                        GridLayers = gridLayers,
                        TileMapPath = TileEditorViewModel.ImagePath,
                        TileHeight = TileEditorViewModel.TileHeight,
                        TileWidth = TileEditorViewModel.TileWidth,
                        GridHeight = GridEditorViewModel.Rows,
                        GridWidth = GridEditorViewModel.Columns
                    };

                    var gridTileListSerialized = JsonConvert.SerializeObject(test);
                    var path = fs.Name;

                    fs.Close();

                    File.WriteAllText(path, gridTileListSerialized);
                }
            }
        }

        public GridEditorViewModel GridEditorViewModel
        {
            get => _gridEditorViewModel;
            set
            {
                if (_gridEditorViewModel is not null)
                {
                    GridEditorViewModel.UpdateNeighbourTiles -= OnUpdateNeighbourTiles;
                }
                if (!SetProperty(ref _gridEditorViewModel, value)) return;
                GridEditorViewModel.UpdateNeighbourTiles += OnUpdateNeighbourTiles;
            }
        }

        private void OnUpdateNeighbourTiles(object? sender, List<GridEditorModel> tiles)
        {
            foreach (var tile in tiles)
            {
                if (tile is not null && tile.ImageIdBottom != -1)
                {
                    tile.ImageSourceBottomLayer = TileEditorViewModel.Tiles.Find(x => x.ImageId == tile.ImageIdBottom)
                        ?.CroppedTileSetImage;
                }
            }

            var old = GridEditorViewModel;
            GridEditorViewModel = new GridEditorViewModel(old.Rows, old.Columns);
            GridEditorViewModel = old;
        }

        public TileEditorViewModel TileEditorViewModel
        {
            get => _tileEditorViewModel;
            set => SetProperty(ref _tileEditorViewModel, value);
        }

        public TileViewModel TileViewModel
        {
            get => _tileViewModel;
            set
            {
                if (_tileViewModel is not null)
                {
                    _tileViewModel.SelectedTileChanged -= GridEditorViewModel.OnSelectedTileChanged;
                    _tileViewModel.SelectedTileCollidableChanged -= GridEditorViewModel.OnSelectedTileCollidableChanged;
                    _tileViewModel.SelectedTileLayerChanged -= GridEditorViewModel.OnSelectedTileLayerChanged;
                }

                if (!SetProperty(ref _tileViewModel, value)) return;
                if (_tileViewModel is null) return;

                _tileViewModel.SelectedTileChanged += GridEditorViewModel.OnSelectedTileChanged;

                _tileViewModel.SelectedTileCollidableChanged += GridEditorViewModel.OnSelectedTileCollidableChanged;

                _tileViewModel.SelectedTileLayerChanged += GridEditorViewModel.OnSelectedTileLayerChanged;

                _tileViewModel.FillEmptyGridSpaceWithSelectedTile +=
                    GridEditorViewModel.OnFillEmptyGridSpaceWithSelectedTile;
            }
        }
    }
}