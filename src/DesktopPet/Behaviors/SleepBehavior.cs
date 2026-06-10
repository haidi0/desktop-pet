using System.Windows.Threading;

namespace DesktopPet.Behaviors;

/// <summary>
/// 睡眠行为 - 长时间无交互后进入睡眠
/// </summary>
public class SleepBehavior
{
    private readonly DispatcherTimer _sleepCheckTimer;
    private readonly Action _onSleep;

    private DateTime _lastInteractionTime;

    public SleepBehavior(Action onSleep, int timeoutSeconds = 300)
    {
        _onSleep = onSleep;
        _lastInteractionTime = DateTime.Now;

        _sleepCheckTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _sleepCheckTimer.Tick += SleepCheckTimer_Tick;
    }

    /// <summary>
    /// 记录用户交互（重置睡眠计时）
    /// </summary>
    public void RecordInteraction()
    {
        _lastInteractionTime = DateTime.Now;
    }

    public void Start()
    {
        _lastInteractionTime = DateTime.Now;
        _sleepCheckTimer.Start();
    }

    public void Stop()
    {
        _sleepCheckTimer.Stop();
    }

    private void SleepCheckTimer_Tick(object? sender, EventArgs e)
    {
        var elapsed = (DateTime.Now - _lastInteractionTime).TotalSeconds;

        if (elapsed >= 300) // 5分钟无交互
        {
            _sleepCheckTimer.Stop();
            _onSleep?.Invoke();
        }
    }
}
