using System.ComponentModel;
using System.Windows;

namespace StreamerTools.WhatIsPlayingOnMyPlayer.WPF;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        DataContext = _viewModel;
        InitializeComponent();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (
            e.PropertyName == nameof(_viewModel.Transparent)
            || e.PropertyName == nameof(_viewModel.WindowWidth)
            || e.PropertyName == nameof(_viewModel.WindowHeight))
        {
            if (_viewModel.Transparent)
            {
                Width = _viewModel.WindowWidth;
                Height = _viewModel.WindowHeight;
            }
            else
            {
                Width = MainWindowViewModel.DefaultWindowWidth;
                Height = MainWindowViewModel.DefaultWindowHeight;
            }
        }
    }
}