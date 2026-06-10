using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DesktopPet.Models;

namespace DesktopPet.Services;

/// <summary>
/// 设置持久化服务
/// </summary>
public class SettingsService
{
    private static readonly string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DesktopPet");

    private static readonly string SettingsFilePath = Path.Combine(AppDataPath, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// 加载设置，若不存在则返回默认值
    /// </summary>
    public AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            }
        }
        catch
        {
            // 读取失败时返回默认设置
        }
        return new AppSettings();
    }

    /// <summary>
    /// 保存设置到文件
    /// </summary>
    public async Task SaveAsync(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(AppDataPath);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
        catch
        {
            // 保存失败时静默忽略
        }
    }

    /// <summary>
    /// 同步保存设置
    /// </summary>
    public void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(AppDataPath);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // 保存失败时静默忽略
        }
    }
}
