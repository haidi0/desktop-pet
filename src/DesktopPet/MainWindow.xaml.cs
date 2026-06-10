using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DesktopPet.Models;
using DesktopPet.Services;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private readonly SettingsService _settingsService;
    private readonly CharacterLoader _characterLoader;
    private readonly ScreenService _screenService;
    private readonly SpriteAnimationEngine _animationEngine;

    private AppSettings _settings;
    private bool _isDragging;
    private Point _dragOffset;

    public MainWindow()
    {
        InitializeComponent();

        _settingsService = new SettingsService();
        _characterLoader = new CharacterLoader();
        _screenService = new ScreenService();
        _animationEngine = new SpriteAnimationEngine();

        _settings = _settingsService.Load();
        PetScale.ScaleX = _settings.PetScale;
        PetScale.ScaleY = _settings.PetScale;

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 设置初始位置
        if (_settings.WindowPosition.X >= 0 && _settings.WindowPosition.Y >= 0)
        {
            var pos = _screenService.ClampToScreen(
                _settings.WindowPosition.X, _settings.WindowPosition.Y,
                Width, Height);
            Left = pos.X;
            Top = pos.Y;
        }
        else
        {
            var pos = _screenService.GetBottomRight(Width, Height);
            Left = pos.X;
            Top = pos.Y;
        }

        // 加载默认角色并播放待机动画
        LoadCharacter(_settings.SelectedCharacter);
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _settings.WindowPosition = new WindowPosition { X = Left, Y = Top };
        await _settingsService.SaveAsync(_settings);
    }

    /// <summary>
    /// 加载角色动画
    /// </summary>
    private void LoadCharacter(string characterId)
    {
        var config = _characterLoader.LoadCharacter(characterId);
        if (config == null) return;

        var frames = _characterLoader.GetStateFrames(characterId, "idle");

        // 如果有动画帧，播放动画
        if (frames.Count > 0)
        {
            int fps = config.DefaultFps.GetValueOrDefault("idle", 8);
            _animationEngine.LoadAnimation(frames, fps, loop: true);
            _animationEngine.Play();

            // 绑定当前帧到 Image
            _animationEngine.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SpriteAnimationEngine.CurrentFrame))
                {
                    PetImage.Source = _animationEngine.CurrentFrame;
                }
            };

            PetImage.Source = _animationEngine.CurrentFrame;
        }
        else
        {
            // 没有动画帧时，显示静态图片 (icon.png)
            var iconPath = _characterLoader.GetCharacterIconPath(characterId);
            if (File.Exists(iconPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                PetImage.Source = bitmap;
            }
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    private void PetImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _dragOffset = e.GetPosition(this);
        CaptureMouse();

        // 切换到拖拽动画
        LoadStateAnimation("drag");
    }

    /// <summary>
    /// 拖拽移动
    /// </summary>
    private void PetImage_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || e.LeftButton != MouseButtonState.Pressed)
            return;

        var currentPos = e.GetPosition(this);
        var screenPos = PointToScreen(currentPos);

        Left = screenPos.X - _dragOffset.X;
        Top = screenPos.Y - _dragOffset.Y;
    }

    /// <summary>
    /// 结束拖拽，执行边缘吸附
    /// </summary>
    private void PetImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging) return;

        _isDragging = false;
        ReleaseMouseCapture();

        // 边缘吸附
        var snapPos = _screenService.GetSnapPosition(Left, Top, Width, Height);
        AnimateToPosition(snapPos.X, snapPos.Y);

        // 切换回待机动画
        LoadStateAnimation("idle");
    }

    /// <summary>
    /// 右键点击 - 打开菜单
    /// </summary>
    private void PetImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 右键菜单会自动弹出
    }

    /// <summary>
    /// 退出应用
    /// </summary>
    private async void Exit_Click(object sender, RoutedEventArgs e)
    {
        _settings.WindowPosition = new WindowPosition { X = Left, Y = Top };
        await _settingsService.SaveAsync(_settings);
        Application.Current.Shutdown();
    }

    /// <summary>
    /// 加载指定状态的动画
    /// </summary>
    private void LoadStateAnimation(string stateName)
    {
        var config = _characterLoader.LoadCharacter(_settings.SelectedCharacter);
        if (config == null) return;

        var frames = _characterLoader.GetStateFrames(_settings.SelectedCharacter, stateName);
        if (frames.Count == 0) return;

        int fps = config.DefaultFps.GetValueOrDefault(stateName, 12);
        bool loop = config.States.GetValueOrDefault(stateName, new StateConfig { Loop = true }).Loop;

        _animationEngine.LoadAnimation(frames, fps, loop);
        _animationEngine.Play();
        PetImage.Source = _animationEngine.CurrentFrame;
    }

    /// <summary>
    /// 平滑移动到目标位置
    /// </summary>
    private void AnimateToPosition(double targetX, double targetY)
    {
        var storyboard = new System.Windows.Media.Animation.Storyboard();

        var animationX = new System.Windows.Media.Animation.DoubleAnimation
        {
            To = targetX,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new System.Windows.Media.Animation.QuadraticEase()
        };

        var animationY = new System.Windows.Media.Animation.DoubleAnimation
        {
            To = targetY,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new System.Windows.Media.Animation.QuadraticEase()
        };

        System.Windows.Media.Animation.Storyboard.SetTarget(animationX, this);
        System.Windows.Media.Animation.Storyboard.SetTargetProperty(animationX,
            new PropertyPath(Window.LeftProperty));

        System.Windows.Media.Animation.Storyboard.SetTarget(animationY, this);
        System.Windows.Media.Animation.Storyboard.SetTargetProperty(animationY,
            new PropertyPath(Window.TopProperty));

        storyboard.Children.Add(animationX);
        storyboard.Children.Add(animationY);
        storyboard.Begin();
    }
}
