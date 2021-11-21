using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using TileMapEditor.Models;
using TileMapEditor.ViewModels;
using TileMapEditor.Views;

namespace TileMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = new MainWindowViewModel();
            DataContext = mainWindow;
            mainWindow.UnsubscribeToOldViewModels += OnUnsubscribeToOldViewModels;
            mainWindow.SubscribeToNewViewModels += OnSubscribeToNewViewModels;

            if (GridEditorView is not null && mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed += gridEditorViewModel.OnTileElementPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged += GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged += GridEditorView.OnImageSourceTopLayerChanged;

                mainWindow.IsShowingCollisionPressed += GridEditorView.OnIsShowingCollisionPressed;
            }

            if (TileEditorView is not null && mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged += tileViewModel.OnSelectedCroppedImageChanged;

                tileViewModel.FillEmptyGridSpaceWithSelectedTile += OnFillEmptyGridSpaceWithSelectedTile;
            }
        }

        private void OnSubscribeToNewViewModels(object? sender, int e)
        {
            if(GridEditorView is not null && mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed += gridEditorViewModel.OnTileElementPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged += GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged += GridEditorView.OnImageSourceTopLayerChanged;
            }

            if (TileEditorView is not null && mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged += tileViewModel.OnSelectedCroppedImageChanged;
                tileViewModel.FillEmptyGridSpaceWithSelectedTile += OnFillEmptyGridSpaceWithSelectedTile;
            }
        }

        private void OnUnsubscribeToOldViewModels(object? sender, int e)
        {
            if (GridEditorView is not null && mainWindow.GridEditorViewModel is { } gridEditorViewModel)
            {
                GridEditorView.TileElementPressed -= gridEditorViewModel.OnTileElementPressed;
                gridEditorViewModel.ImageSourceBottomLayerChanged -= GridEditorView.OnImageSourceBottomLayerChanged;
                gridEditorViewModel.ImageSourceTopLayerChanged -= GridEditorView.OnImageSourceTopLayerChanged;
            }

            if (TileEditorView is not null && mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged -= tileViewModel.OnSelectedCroppedImageChanged;
                tileViewModel.FillEmptyGridSpaceWithSelectedTile -= OnFillEmptyGridSpaceWithSelectedTile;
            }
        }

        private void OnFillEmptyGridSpaceWithSelectedTile(object? sender, Tile e)
        {
            // Needed for real time update of grid
            var oldVM = mainWindow.GridEditorViewModel;
            mainWindow.GridEditorViewModel = new GridEditorViewModel(oldVM.Rows, oldVM.Columns);
            mainWindow.GridEditorViewModel = oldVM;
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
