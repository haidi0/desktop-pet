using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DesktopPet.Models;

/// <summary>
/// 角色配置文件 character.json 的数据模型
/// </summary>
public class CharacterConfig
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "icon.png";

    [JsonPropertyName("defaultFps")]
    public Dictionary<string, int> DefaultFps { get; set; } = new()
    {
        ["idle"] = 8,
        ["walk_right"] = 12,
        ["walk_left"] = 12,
        ["sleep"] = 4,
        ["drag"] = 10,
        ["interact"] = 15
    };

    [JsonPropertyName("states")]
    public Dictionary<string, StateConfig> States { get; set; } = new();
}

/// <summary>
/// 单个状态的配置
/// </summary>
public class StateConfig
{
    [JsonPropertyName("folder")]
    public string Folder { get; set; } = string.Empty;

    [JsonPropertyName("loop")]
    public bool Loop { get; set; } = true;
}
