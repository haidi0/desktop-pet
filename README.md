# 桌面宠物 - DesktopPet

一个基于 C# WPF 的 Windows 桌面宠物应用。

## 功能特性

- 透明窗口显示角色
- 拖拽移动 + 边缘吸附
- 帧动画引擎（PNG序列）
- 系统托盘图标
- 多角色切换支持
- 设置持久化

## 环境要求

- Windows 10/11
- .NET 8.0 SDK

## 安装

1. 安装 [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. 克隆仓库
3. 运行项目：

```bash
cd src/DesktopPet
dotnet restore
dotnet run
```

## 项目结构

```
DesktopPet/
├── src/DesktopPet/          # 主项目
│   ├── Models/              # 数据模型
│   ├── ViewModels/          # MVVM视图模型
│   ├── Services/            # 核心服务
│   ├── Helpers/             # 工具类
│   └── Behaviors/           # 行为逻辑
├── assets/characters/       # 角色资源
│   └── zexiaoan/            # 泽小安角色
└── tests/                   # 测试项目
```

## 角色资源

角色资源存放在 `assets/characters/` 目录下，每个角色包含：
- `character.json` - 角色配置
- `idle/` - 待机动画帧
- `walk_right/` - 向右行走帧
- `walk_left/` - 向左行走帧
- `sleep/` - 睡眠动画帧
- `drag/` - 拖拽动画帧
- `interact/` - 互动动画帧

## 添加新角色

1. 在 `assets/characters/` 下创建新文件夹
2. 添加 `character.json` 配置文件
3. 按状态放入对应的 PNG 帧图片

## License

MIT
