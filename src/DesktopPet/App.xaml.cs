using System.Windows;

namespace DesktopPet;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 单实例检查
        var mutex = new System.Threading.Mutex(true, "DesktopPet_SingleInstance", out bool isNewInstance);
        if (!isNewInstance)
        {
            MessageBox.Show("桌面宠物已在运行中！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }
    }
}
