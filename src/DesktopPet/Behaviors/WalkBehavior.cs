using System.Windows.Threading;

namespace DesktopPet.Behaviors;

/// <summary>
/// 行走行为 - 宠物随机方向移动
/// </summary>
public class WalkBehavior
{
    private readonly DispatcherTimer _walkTimer;
    private readonly Action<double, double> _onMove;
    private readonly Action _onWalkComplete;
    private readonly Random _random = new();

    private double _stepX;
    private double _stepY;
    private int _stepsRemaining;

    public WalkBehavior(Action<double, double> onMove, Action onWalkComplete)
    {
        _onMove = onMove;
        _onWalkComplete = onWalkComplete;

        _walkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _walkTimer.Tick += WalkTimer_Tick;
    }

    public void Start()
    {
        // 随机选择方向和步数
        var angle = _random.NextDouble() * Math.PI * 2;
        var speed = _random.Next(2, 6); // 每步像素
        var steps = _random.Next(20, 60); // 总步数

        _stepX = Math.Cos(angle) * speed;
        _stepY = Math.Sin(angle) * speed;
        _stepsRemaining = steps;

        _walkTimer.Start();
    }

    public void Stop()
    {
        _walkTimer.Stop();
    }

    private void WalkTimer_Tick(object? sender, EventArgs e)
    {
        if (_stepsRemaining <= 0)
        {
            _walkTimer.Stop();
            _onWalkComplete?.Invoke();
            return;
        }

        _onMove?.Invoke(_stepX, _stepY);
        _stepsRemaining--;
    }
}
