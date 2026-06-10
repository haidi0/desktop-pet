namespace DesktopPet.Models;

/// <summary>
/// 宠物状态枚举
/// </summary>
public enum PetState
{
    /// <summary>待机</summary>
    Idle,

    /// <summary>向右行走</summary>
    WalkRight,

    /// <summary>向左行走</summary>
    WalkLeft,

    /// <summary>睡眠</summary>
    Sleep,

    /// <summary>被拖拽</summary>
    Drag,

    /// <summary>互动</summary>
    Interact
}
