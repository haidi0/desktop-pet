using System.Windows.Threading;

namespace DesktopPet.Behaviors;

/// <summary>
/// 待机行为 - 随机触发眨眼、摇摆等小动作
/// </summary>
public class IdleBehavior
{
    private readonly DispatcherTimer _randomActionTimer;
    private readonly Random _random = new();
    private readonly Action _onRandomAction;

    public IdleBehavior(Action onRandomAction)
    {
        _onRandomAction = onRandomAction;
        _randomActionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(_random.Next(30, 120))
        };
        _randomActionTimer.Tick += RandomActionTimer_Tick;
    }

    public void Start()
    {
        _randomActionTimer.Interval = TimeSpan.FromSeconds(_random.Next(30, 120));
        _randomActionTimer.Start();
    }

    public void Stop()
    {
        _randomActionTimer.Stop();
    }

    private void RandomActionTimer_Tick(object? sender, EventArgs e)
    {
        _onRandomAction?.Invoke();

        // 重置计时器为随机间隔
        _randomActionTimer.Stop();
        _randomActionTimer.Interval = TimeSpan.FromSeconds(_random.Next(30, 120));
        _randomActionTimer.Start();
    }
}
