using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using TileMapEditor.Models;

namespace TileMapEditor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly List<ImageSource> _tempImages = new();
        private SolidColorBrush _collisionButtonBackground;
        private int _gridColumns;
        private GridEditorViewModel _gridEditorViewModel;
        private int _gridRows;
        private bool _isShowingCollision;
        private string _standardTilesetPath;
        private int _tileHeight;
        private TilePickerViewModel _tilePickerViewModel;
        private TileViewModel _tileViewModel;
        private int _tileWidth;

        public MainWindowViewModel()
        {
            CreateStandardTileset();
            GridEditorViewModel = new GridEditorViewModel(15, 20);
            TilePickerViewModel = new TilePickerViewModel(10, 14, 16, 16, StandardTilesetPath);
            TileViewModel = new TileViewModel();
            ExportGridCommand = new AsyncRelayCommand(OnExportGridCommand);
            GridCollisionCommand = new RelayCommand(OnGridCollisionCommand);
            ImportGridCommand = new AsyncRelayCommand(OnImportGridCommand);
            NewTilesetCommand = new AsyncRelayCommand(OnNewTilesetCommand);
            NewGridCommand = new AsyncRelayCommand(OnNewGridCommand);
            IsShowingCollisionPressed += TileViewModel.OnIsShowingCollisionPressed;
            CollisionButtonBackground = new SolidColorBrush(Colors.DarkSeaGreen);
            GridRows = 15;
            GridColumns = 20;
            TileWidth = 16;
            TileHeight = 16;
        }

        public AsyncRelayCommand ExportGridCommand { get; set; }
        public RelayCommand GridCollisionCommand { get; set; }
        public AsyncRelayCommand ImportGridCommand { get; set; }
        public AsyncRelayCommand NewTilesetCommand { get; set; }
        public AsyncRelayCommand NewGridCommand { get; set; }

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

        public GridEditorViewModel GridEditorViewModel
        {
            get => _gridEditorViewModel;
            set
            {
                if (_gridEditorViewModel is not null)
                {
                    GridEditorViewModel.UpdateNeighbourTiles -= OnUpdateNeighbourTiles;
                }

                if (!SetProperty(ref _gridEditorViewModel, value))
                {
                    return;
                }

                GridEditorViewModel.UpdateNeighbourTiles += OnUpdateNeighbourTiles;
            }
        }

        public TilePickerViewModel TilePickerViewModel
        {
            get => _tilePickerViewModel;
            set => SetProperty(ref _tilePickerViewModel, value);
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

                if (!SetProperty(ref _tileViewModel, value))
                {
                    return;
                }

                if (_tileViewModel is null)
                {
                    return;
                }

                _tileViewModel.SelectedTileChanged += GridEditorViewModel.OnSelectedTileChanged;

                _tileViewModel.SelectedTileCollidableChanged += GridEditorViewModel.OnSelectedTileCollidableChanged;

                _tileViewModel.SelectedTileLayerChanged += GridEditorViewModel.OnSelectedTileLayerChanged;

                _tileViewModel.FillEmptyGridSpaceWithSelectedTile +=
                    GridEditorViewModel.OnFillEmptyGridSpaceWithSelectedTile;
            }
        }

        public event EventHandler<int> UnsubscribeToOldViewModels;
        public event EventHandler<int> SubscribeToNewViewModels;
        public event EventHandler<bool> IsShowingCollisionPressed;

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

            File.Copy(AssetStructure.TilesetPath, path);
            StandardTilesetPath = path;
        }

        private async Task OnNewTilesetCommand()
        {
            if (TileHeight is 0 || TileWidth is 0)
            {
                return;
            }

            var result = MessageBox.Show("Do you want to save current map?",
                $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                await OnExportGridCommand();
            }

            var op = new OpenFileDialog
            {
                Title = "Select a tileset",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png"
            };

            var localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (op.ShowDialog() != true)
            {
                return;
            }

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
                    TilePickerViewModel = new TilePickerViewModel(10, 14, 16, 16, StandardTilesetPath);
                    return;
                }
            }

            File.Copy(source, path);
            var image = new BitmapImage(new Uri(path));

            var newTileSetRows = (int)image.Height / TileHeight;
            var newTileSetColumns = (int)image.Width / TileWidth;
            var maxSizeOnTileSet = 400;

            if (newTileSetRows * newTileSetColumns >= maxSizeOnTileSet)
            {
                MessageBox.Show("Tile set is to big, try again with a smaller tile width and height",
                    $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                UnsubscribeToOldViewModels?.Invoke(this, 0);
                TilePickerViewModel = new TilePickerViewModel(
                    newTileSetRows,
                    newTileSetColumns,
                    TileWidth, TileHeight, path);
                GridEditorViewModel = new GridEditorViewModel(GridEditorViewModel.Rows, GridEditorViewModel.Columns);
                TileViewModel = new TileViewModel();
                SubscribeToNewViewModels?.Invoke(this, 0);
            }
        }

        private async Task OnNewGridCommand()
        {
            if (GridRows is 0 || GridColumns is 0)
            {
                return;
            }

            var result = MessageBox.Show("Do you want to save current map?",
                $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                await OnExportGridCommand();
            }

            var image = new BitmapImage(new Uri(TilePickerViewModel.ImagePath));
            UnsubscribeToOldViewModels?.Invoke(this, 0);
            TilePickerViewModel = new TilePickerViewModel(
                (int)image.Height / TilePickerViewModel.TileHeight,
                (int)image.Width / TilePickerViewModel.TileWidth,
                TilePickerViewModel.TileWidth, TilePickerViewModel.TileHeight, TilePickerViewModel.ImagePath);
            GridEditorViewModel = new GridEditorViewModel(GridRows, GridColumns);
            TileViewModel = new TileViewModel();
            SubscribeToNewViewModels?.Invoke(this, 0);
        }

        private async Task OnImportGridCommand()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "json files (*.json)|*.json",
                Title = "Open tilemap"
            };

            openFileDialog.ShowDialog();

            if (openFileDialog.FileName == "")
            {
                return;
            }

            var text = await File.ReadAllTextAsync(openFileDialog.FileName);
            var mainWindowModel = JsonConvert.DeserializeObject<MainWindowModel>(text);

            if (mainWindowModel is not { } mWm)
            {
                return;
            }

            UnsubscribeToOldViewModels?.Invoke(this, 0);
            GridEditorViewModel =
                new GridEditorViewModel(mWm.GridHeight, mWm.GridWidth);

            TilePickerViewModel = new TilePickerViewModel(mWm.TilemapHeight / mWm.TileHeight,
                mWm.TilemapWidth / mWm.TileWidth, mWm.TileWidth,
                mWm.TileHeight, mWm.TileMapPath);

            TileViewModel = new TileViewModel();

            for (var i = 0; i < GridEditorViewModel.MapTiles.Count; i++)
            {
                GridEditorViewModel.MapTiles[i].ImageIdBottom = mWm.GridLayers[0, i];
                if (GridEditorViewModel.MapTiles[i].ImageIdBottom != -1)
                {
                    GridEditorViewModel.MapTiles[i].ImageSourceBottomLayer = TilePickerViewModel.Tiles
                        .Find(x => x.ImageId == GridEditorViewModel.MapTiles[i].ImageIdBottom)
                        ?.CroppedTileSetImage;
                }

                GridEditorViewModel.MapTiles[i].ImageIdTop = mWm.GridLayers[1, i];
                if (GridEditorViewModel.MapTiles[i].ImageIdTop != -1)
                {
                    GridEditorViewModel.MapTiles[i].ImageSourceTopLayer = TilePickerViewModel.Tiles
                        .Find(x => x.ImageId == GridEditorViewModel.MapTiles[i].ImageIdTop)
                        ?.CroppedTileSetImage;
                }

                GridEditorViewModel.MapTiles[i].IsCollidable = mWm.Collidable[i] == 1;
            }

            SubscribeToNewViewModels?.Invoke(this, 0);
        }


        private void OnGridCollisionCommand()
        {
            IsShowingCollision = !IsShowingCollision;
            IsShowingCollisionPressed?.Invoke(this, !IsShowingCollision);

            if (IsShowingCollision)
            {
                foreach (var tile in GridEditorViewModel.MapTiles.Where(tile => tile.IsCollidable))
                {
                    _tempImages.Add(tile.ImageSourceTopLayer);
                    _tempImages.Add(tile.ImageSourceBottomLayer);

                    tile.ImageSourceTopLayer =
                        new BitmapImage(new Uri(AssetStructure.CollisionPngPath));
                    tile.ImageSourceBottomLayer =
                        new BitmapImage(new Uri(AssetStructure.CollisionPngPath));
                }

                CollisionButtonBackground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                foreach (var tile in GridEditorViewModel.MapTiles.Where(tile => tile.IsCollidable))
                {
                    tile.ImageSourceTopLayer = _tempImages[0];
                    _tempImages.RemoveAt(0);

                    tile.ImageSourceBottomLayer = _tempImages[0];
                    _tempImages.RemoveAt(0);
                }

                CollisionButtonBackground = new SolidColorBrush(Colors.DarkSeaGreen);
            }

            CollectionViewSource.GetDefaultView(GridEditorViewModel.MapTiles).Refresh();
        }

        private async Task OnExportGridCommand()
        {
            if (GridEditorViewModel.MapTiles.Count != 0)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    Filter = "json files (*.json)|*.json",
                    Title = "Save tilemap",
                    FileName = $"tilemap_{GridEditorViewModel.Rows}x{GridEditorViewModel.Columns}_"
                };
                var ok = saveFileDialog.ShowDialog();

                var gridTileIsCollidableArray = new int[GridEditorViewModel.MapTiles.Count];
                for (var i = 0; i < GridEditorViewModel.MapTiles.Count; i++)
                {
                    gridTileIsCollidableArray[i] = GridEditorViewModel.MapTiles[i].IsCollidable ? 1 : 0;
                }

                var gridLayers = new int[2, GridEditorViewModel.MapTiles.Count];
                for (var i = 0; i < GridEditorViewModel.MapTiles.Count; i++)
                {
                    gridLayers[0, i] = GridEditorViewModel.MapTiles[i].ImageIdBottom;
                }

                for (var i = 0; i < GridEditorViewModel.MapTiles.Count; i++)
                {
                    gridLayers[1, i] = GridEditorViewModel.MapTiles[i].ImageIdTop;
                }

                if (saveFileDialog.FileName != "" && ok is true)
                {
                    var fs = (FileStream)saveFileDialog.OpenFile();

                    var image = new BitmapImage(new Uri(TilePickerViewModel.ImagePath));

                    var test = new MainWindowModel
                    {
                        Name = saveFileDialog.SafeFileName,
                        TilemapHeight = (int)image.Height,
                        TilemapWidth = (int)image.Width,
                        Collidable = gridTileIsCollidableArray,
                        GridLayers = gridLayers,
                        TileMapPath = TilePickerViewModel.ImagePath,
                        TileHeight = TilePickerViewModel.TileHeight,
                        TileWidth = TilePickerViewModel.TileWidth,
                        GridHeight = GridEditorViewModel.Rows,
                        GridWidth = GridEditorViewModel.Columns
                    };

                    var gridTileListSerialized = JsonConvert.SerializeObject(test);
                    var path = fs.Name;

                    fs.Close();

                    await File.WriteAllTextAsync(path, gridTileListSerialized);
                }
            }
        }

        private void OnUpdateNeighbourTiles(object? sender, List<MapTile> tiles)
        {
            foreach (var tile in tiles.Where(tile => tile is not null && tile.ImageIdBottom != -1))
            {
                tile.ImageSourceBottomLayer = TilePickerViewModel.Tiles.Find(x => x.ImageId == tile.ImageIdBottom)
                    ?.CroppedTileSetImage;
            }

            CollectionViewSource.GetDefaultView(GridEditorViewModel.MapTiles).Refresh();
        }
    }
}