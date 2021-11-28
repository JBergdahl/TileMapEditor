using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TileMapEditor.ViewModels;

namespace TileMapEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindow = new MainWindowViewModel();
            DataContext = _mainWindow;
            _mainWindow.UnsubscribeToOldViewModels += OnUnsubscribeToOldViewModels;
            _mainWindow.SubscribeToNewViewModels += OnSubscribeToNewViewModels;

            if (GridEditorView is not null && _mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed += gridEditorViewModel.OnTileElementPressed;
                GridEditorView.TileElementRightPressed += gridEditorViewModel.OnTileElementRightPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged += GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged += GridEditorView.OnImageSourceTopLayerChanged;
                _mainWindow.IsShowingCollisionPressed += GridEditorView.OnIsShowingCollisionPressed;
            }

            if (TileEditorView is not null && _mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged += tileViewModel.OnSelectedCroppedImageChanged;
            }
        }

        private void OnSubscribeToNewViewModels(object? sender, int e)
        {
            if (GridEditorView is not null && _mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed += gridEditorViewModel.OnTileElementPressed;
                GridEditorView.TileElementRightPressed += gridEditorViewModel.OnTileElementRightPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged += GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged += GridEditorView.OnImageSourceTopLayerChanged;
            }

            if (TileEditorView is not null && _mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged += tileViewModel.OnSelectedCroppedImageChanged;
            }
        }

        private void OnUnsubscribeToOldViewModels(object? sender, int e)
        {
            if (GridEditorView is not null && _mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed -= gridEditorViewModel.OnTileElementPressed;
                GridEditorView.TileElementRightPressed -= gridEditorViewModel.OnTileElementRightPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged -= GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged -= GridEditorView.OnImageSourceTopLayerChanged;
            }

            if (TileEditorView is not null && _mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged -= tileViewModel.OnSelectedCroppedImageChanged;
            }
        }

        private void Tileset_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "^[0-9]+$"))
            {
                e.Handled = true;
            }
        }
    }
}