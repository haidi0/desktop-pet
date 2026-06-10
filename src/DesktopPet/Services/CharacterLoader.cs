using System.IO;
using System.Text.Json;
using DesktopPet.Models;

namespace DesktopPet.Services;

/// <summary>
/// 角色资源加载器
/// </summary>
public class CharacterLoader
{
    private static readonly string AssetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "characters");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 获取所有可用角色ID
    /// </summary>
    public List<string> GetAvailableCharacters()
    {
        var characters = new List<string>();

        if (Directory.Exists(AssetsPath))
        {
            foreach (var dir in Directory.GetDirectories(AssetsPath))
            {
                var configFile = Path.Combine(dir, "character.json");
                if (File.Exists(configFile))
                {
                    characters.Add(Path.GetFileName(dir));
                }
            }
        }

        return characters;
    }

    /// <summary>
    /// 加载指定角色的配置
    /// </summary>
    public CharacterConfig? LoadCharacter(string characterId)
    {
        var configPath = Path.Combine(AssetsPath, characterId, "character.json");

        if (!File.Exists(configPath))
            return null;

        try
        {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<CharacterConfig>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取角色某个状态的帧图片路径列表
    /// </summary>
    public List<string> GetStateFrames(string characterId, string stateFolder)
    {
        var folderPath = Path.Combine(AssetsPath, characterId, stateFolder);

        if (!Directory.Exists(folderPath))
            return new List<string>();

        return Directory.GetFiles(folderPath, "*.png")
            .OrderBy(f => f)
            .ToList();
    }

    /// <summary>
    /// 获取角色图标路径
    /// </summary>
    public string GetCharacterIconPath(string characterId)
    {
        return Path.Combine(AssetsPath, characterId, "icon.png");
    }
}
