using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public MainWindow()
        {
            InitializeComponent();
            var mainWindow = new MainWindowViewModel();
            DataContext = mainWindow;

            if (GridEditorView is not null && mainWindow.GridEditorViewModel is not null)
            {
                GridEditorView.TileElementPressed += mainWindow.GridEditorViewModel.OnTileElementPressed;
            }

            if (TileEditorView is not null && GridEditorView is not null)
            {
                TileEditorView.SelectedCroppedImageChanged += GridEditorView.OnSelectedCroppedImageChanged;
            }

            
            if (TileEditorView is not null && mainWindow.TileViewModel is { } tileViewModel)
            {
                TileEditorView.SelectedCroppedImageChanged += tileViewModel.OnSelectedCroppedImageChanged;
            }
        }
    }
}
