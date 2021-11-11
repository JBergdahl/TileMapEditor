using System.Configuration;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileMapEditor.Views;

namespace TileMapEditor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private GridEditorViewModel _gridEditorViewModel;
        private TileEditorViewModel _tileEditorViewModel;
        private TileViewModel _tileViewModel;

        public MainWindowViewModel()
        {
            GridEditorViewModel = new GridEditorViewModel();
            TileEditorViewModel = new TileEditorViewModel();
            TileViewModel = new TileViewModel();
        }

        public GridEditorViewModel GridEditorViewModel
        {
            get => _gridEditorViewModel;
            set => SetProperty(ref _gridEditorViewModel, value);
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