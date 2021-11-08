using System.Configuration;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileMapEditor.Views;

namespace TileMapEditor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private GridEditorViewModel _gridEditorViewModel;
        private TileEditorViewModel _tileEditorViewModel;

        public MainWindowViewModel()
        {
            GridEditorViewModel = new GridEditorViewModel();
            TileEditorViewModel = new TileEditorViewModel();
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
    }
}