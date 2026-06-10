using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DesktopPet.Services;

/// <summary>
/// 帧动画引擎
/// </summary>
public class SpriteAnimationEngine : INotifyPropertyChanged
{
    private readonly List<BitmapSource> _frames = new();
    private readonly DispatcherTimer _timer;
    private int _currentFrameIndex;
    private bool _loop = true;

    public bool IsPlaying => _timer.IsEnabled;

    private BitmapSource? _currentFrame;
    public BitmapSource? CurrentFrame
    {
        get => _currentFrame;
        set
        {
            if (_currentFrame != value)
            {
                _currentFrame = value;
                OnPropertyChanged();
            }
        }
    }

    public event EventHandler? AnimationCompleted;
    public event PropertyChangedEventHandler? PropertyChanged;

    public SpriteAnimationEngine()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / 12)
        };
        _timer.Tick += Timer_Tick;
    }

    /// <summary>
    /// 加载动画帧
    /// </summary>
    public void LoadAnimation(List<string> framePaths, int fps = 12, bool loop = true)
    {
        Stop();
        _frames.Clear();
        _currentFrameIndex = 0;
        _loop = loop;

        foreach (var path in framePaths)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                _frames.Add(bitmap);
            }
            catch
            {
                // 跳过加载失败的帧
            }
        }

        _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / Math.Max(1, fps));

        // 设置第一帧
        if (_frames.Count > 0)
        {
            CurrentFrame = _frames[0];
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void Play()
    {
        if (_frames.Count == 0) return;
        _currentFrameIndex = 0;
        CurrentFrame = _frames[0];
        _timer.Start();
    }

    /// <summary>
    /// 暂停动画
    /// </summary>
    public void Pause()
    {
        _timer.Stop();
    }

    /// <summary>
    /// 停止动画
    /// </summary>
    public void Stop()
    {
        _timer.Stop();
        _currentFrameIndex = 0;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_frames.Count == 0) return;

        _currentFrameIndex++;

        if (_currentFrameIndex >= _frames.Count)
        {
            if (_loop)
            {
                _currentFrameIndex = 0;
            }
            else
            {
                _timer.Stop();
                AnimationCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        CurrentFrame = _frames[_currentFrameIndex];
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
