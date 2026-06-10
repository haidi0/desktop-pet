using System.Text.Json.Serialization;

namespace DesktopPet.Models;

/// <summary>
/// 全局设置模型
/// </summary>
public class AppSettings
{
    [JsonPropertyName("selectedCharacter")]
    public string SelectedCharacter { get; set; } = "zexiaoan";

    [JsonPropertyName("windowPosition")]
    public WindowPosition WindowPosition { get; set; } = new();

    [JsonPropertyName("isTopmost")]
    public bool IsTopmost { get; set; } = true;

    [JsonPropertyName("startWithWindows")]
    public bool StartWithWindows { get; set; } = false;

    [JsonPropertyName("petScale")]
    public double PetScale { get; set; } = 1.0;
}

/// <summary>
/// 窗口位置
/// </summary>
public class WindowPosition
{
    [JsonPropertyName("x")]
    public double X { get; set; } = -1;

    [JsonPropertyName("y")]
    public double Y { get; set; } = -1;
}
