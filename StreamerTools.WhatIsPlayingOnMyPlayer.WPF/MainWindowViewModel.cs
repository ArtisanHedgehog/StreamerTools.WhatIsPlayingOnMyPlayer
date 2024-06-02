using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace StreamerTools.WhatIsPlayingOnMyPlayer.WPF;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private const string _foobarProcessName = "foobar2000";

    private string _lastTitle = string.Empty;
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(2);
    private Process? _foobarProcess = null;
    private readonly Timer _timer;

    private bool _transparent;

    [ObservableProperty]
    private Visibility _configurationVisibility;

    [ObservableProperty]
    private string _trackTitle = "Track Title";

    [ObservableProperty]
    private double _fontSize;

    [ObservableProperty]
    private double _windowHeight = DefaultWindowHeight;

    [ObservableProperty]
    private double _windowWidth = DefaultWindowWidth;

    public MainWindowViewModel()
    {
        _timer = new Timer(_delay);
        _timer.Elapsed += TimerTicked;
        _timer.AutoReset = true;
        _timer.Start();
        _transparent = true;
        _configurationVisibility = Visibility.Hidden;
        _fontSize = 18;
    }

    public const double DefaultWindowHeight = 400;

    public const double DefaultWindowWidth = 800;

    public Brush Background => Transparent
        ? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
        : new SolidColorBrush(Color.FromArgb(byte.MaxValue, 0, 0, 0));

    public bool Transparent
    {
        get => _transparent;
        set
        {
            if (SetProperty(ref _transparent, value))
            {
                OnPropertyChanged(nameof(Background));
            }
        }
    }

    public string FontSizeText
    {
        get => FontSize.ToString();
        set
        {
            if(!double.TryParse(value, out double fontSize))
            {
                return;
            }

            FontSize = fontSize;
        }
    }

    public string WindowHeightText
    {
        get => WindowHeight.ToString();
        set
        {
            if (!double.TryParse(value, out double windowHeight))
            {
                return;
            }

            WindowHeight = windowHeight;
        }
    }

    public string WindowWidthText
    {
        get => WindowWidth.ToString();
        set
        {
            if (!double.TryParse(value, out double windowWidth))
            {
                return;
            }

            WindowWidth = windowWidth;
        }
    }

    [RelayCommand]
    private void SwitchConfiugurationMode()
    {
        Transparent = !Transparent;
        ConfigurationVisibility = Transparent ? Visibility.Hidden : Visibility.Visible;
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    private void TimerTicked(object? _, ElapsedEventArgs _2)
    {
        if (_foobarProcess is not null && _foobarProcess.HasExited)
        {
            _foobarProcess = null;
            TrackTitle = "Foobar 2000 is closed";
        }

        if (_foobarProcess is null)
        {
            _foobarProcess = Process.GetProcessesByName(_foobarProcessName).FirstOrDefault();
        }

        if (_foobarProcess is null)
        {
            return;
        }

        _foobarProcess.Refresh();

        if (_foobarProcess is not null && _lastTitle != _foobarProcess.MainWindowTitle)
        {
            _lastTitle = _foobarProcess.MainWindowTitle;
            TrackTitle = _lastTitle.Replace($"[{_foobarProcessName}]", string.Empty).Trim(' ');
        }
    }

    public void Dispose()
    {
        _timer.Stop();
    }
}
