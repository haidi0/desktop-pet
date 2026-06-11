using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private bool _isDragging;
    private Point _dragOffset;
    private readonly DispatcherTimer _animationTimer;
    private BitmapSource[]? _frames;
    private int _currentFrame;
    private readonly string _basePath;

    public MainWindow()
    {
        InitializeComponent();

        _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "characters", "zexiaoan");
        _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _animationTimer.Tick += AnimationTimer_Tick;

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 直接设置到屏幕右下角
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Right - Width - 10;
        Top = workArea.Bottom - Height - 10;

        // 加载动画帧
        LoadAnimation();
    }

    private void LoadAnimation()
    {
        var idlePath = Path.Combine(_basePath, "idle");

        if (!Directory.Exists(idlePath))
        {
            // 尝试加载 icon.png
            var iconPath = Path.Combine(_basePath, "icon.png");
            if (File.Exists(iconPath))
            {
                PetImage.Source = new BitmapImage(new Uri(iconPath));
            }
            return;
        }

        var files = Directory.GetFiles(idlePath, "*.png");
        if (files.Length == 0) return;

        _frames = new BitmapSource[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(files[i]);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            _frames[i] = bitmap;
        }

        _currentFrame = 0;
        PetImage.Source = _frames[0];
        _animationTimer.Start();
    }

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        if (_frames == null || _frames.Length == 0) return;

        _currentFrame = (_currentFrame + 1) % _frames.Length;
        PetImage.Source = _frames[_currentFrame];
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _dragOffset = e.GetPosition(this);
        CaptureMouse();
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || e.LeftButton != MouseButtonState.Pressed) return;

        var screenPos = PointToScreen(e.GetPosition(this));
        Left = screenPos.X - _dragOffset.X;
        Top = screenPos.Y - _dragOffset.Y;
    }

    private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;
        ReleaseMouseCapture();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
