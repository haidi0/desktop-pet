using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopPet.Models;
using DesktopPet.Services;

namespace DesktopPet.ViewModels;

/// <summary>
/// 主窗口 ViewModel
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly CharacterLoader _characterLoader;
    private readonly ScreenService _screenService;
    private readonly SpriteAnimationEngine _animationEngine;

    [ObservableProperty]
    private BitmapSource? _currentFrame;

    [ObservableProperty]
    private string _currentStateText = "待机";

    [ObservableProperty]
    private string _currentCharacterName = "泽小安";

    [ObservableProperty]
    private PetState _currentState = PetState.Idle;

    [ObservableProperty]
    private double _petScale = 1.0;

    public AppSettings Settings { get; private set; }

    public List<string> AvailableCharacters { get; private set; } = new();

    public MainViewModel()
    {
        _settingsService = new SettingsService();
        _characterLoader = new CharacterLoader();
        _screenService = new ScreenService();
        _animationEngine = new SpriteAnimationEngine();

        Settings = _settingsService.Load();
        PetScale = Settings.PetScale;

        AvailableCharacters = _characterLoader.GetAvailableCharacters();

        _animationEngine.AnimationCompleted += OnAnimationCompleted;

        LoadCharacter(Settings.SelectedCharacter);
    }

    /// <summary>
    /// 加载角色并播放待机动画
    /// </summary>
    public void LoadCharacter(string characterId)
    {
        var config = _characterLoader.LoadCharacter(characterId);
        if (config == null) return;

        CurrentCharacterName = config.Name;
        Settings.SelectedCharacter = characterId;

        ChangeState(PetState.Idle);
    }

    /// <summary>
    /// 切换宠物状态
    /// </summary>
    public void ChangeState(PetState newState)
    {
        CurrentState = newState;

        var stateName = newState switch
        {
            PetState.Idle => "idle",
            PetState.WalkRight => "walk_right",
            PetState.WalkLeft => "walk_left",
            PetState.Sleep => "sleep",
            PetState.Drag => "drag",
            PetState.Interact => "interact",
            _ => "idle"
        };

        CurrentStateText = newState switch
        {
            PetState.Idle => "待机",
            PetState.WalkRight => "行走",
            PetState.WalkLeft => "行走",
            PetState.Sleep => "睡眠",
            PetState.Drag => "拖拽",
            PetState.Interact => "互动",
            _ => "待机"
        };

        var config = _characterLoader.LoadCharacter(Settings.SelectedCharacter);
        if (config == null) return;

        var frames = _characterLoader.GetStateFrames(Settings.SelectedCharacter, stateName);
        if (frames.Count == 0) return;

        int fps = config.DefaultFps.GetValueOrDefault(stateName, 12);
        bool loop = config.States.GetValueOrDefault(stateName, new StateConfig { Loop = true }).Loop;

        _animationEngine.LoadAnimation(frames, fps, loop);
        _animationEngine.Play();
    }

    /// <summary>
    /// 获取屏幕右下角位置
    /// </summary>
    public Point GetStartPosition(double width, double height)
    {
        if (Settings.WindowPosition.X >= 0 && Settings.WindowPosition.Y >= 0)
        {
            return _screenService.ClampToScreen(
                Settings.WindowPosition.X, Settings.WindowPosition.Y,
                width, height);
        }

        return _screenService.GetBottomRight(width, height);
    }

    /// <summary>
    /// 获取拖拽吸附位置
    /// </summary>
    public Point GetSnapPosition(double left, double top, double width, double height)
    {
        return _screenService.GetSnapPosition(left, top, width, height);
    }

    /// <summary>
    /// 保存窗口位置
    /// </summary>
    public async Task SaveWindowPositionAsync(double left, double top)
    {
        Settings.WindowPosition = new WindowPosition { X = left, Y = top };
        await _settingsService.SaveAsync(Settings);
    }

    /// <summary>
    /// 保存所有设置
    /// </summary>
    public async Task SaveSettingsAsync()
    {
        await _settingsService.SaveAsync(Settings);
    }

    [RelayCommand]
    private void OnPetClicked()
    {
        if (CurrentState != PetState.Drag)
        {
            ChangeState(PetState.Interact);
        }
    }

    [RelayCommand]
    private void SwitchCharacter(string characterId)
    {
        LoadCharacter(characterId);
    }

    private void OnAnimationCompleted(object? sender, EventArgs e)
    {
        if (CurrentState == PetState.Interact)
        {
            ChangeState(PetState.Idle);
        }
    }
}
