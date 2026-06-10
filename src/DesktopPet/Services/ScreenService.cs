using System;
using System.Windows;

namespace DesktopPet.Services;

/// <summary>
/// 屏幕边界检测服务
/// </summary>
public class ScreenService
{
    /// <summary>
    /// 获取主屏幕工作区边界
    /// </summary>
    public Rect GetWorkingArea()
    {
        return SystemParameters.WorkArea;
    }

    /// <summary>
    /// 将窗口位置限制在屏幕工作区内
    /// </summary>
    public Point ClampToScreen(double left, double top, double width, double height)
    {
        var workArea = GetWorkingArea();

        var x = Math.Max(workArea.Left, Math.Min(left, workArea.Right - width));
        var y = Math.Max(workArea.Top, Math.Min(top, workArea.Bottom - height));

        return new Point(x, y);
    }

    /// <summary>
    /// 获取最近的屏幕边缘位置（用于拖拽吸附）
    /// </summary>
    public Point GetSnapPosition(double left, double top, double width, double height)
    {
        var workArea = GetWorkingArea();
        var centerX = left + width / 2;
        var centerY = top + height / 2;

        // 计算到各边缘的距离
        double distLeft = Math.Abs(centerX - workArea.Left);
        double distRight = Math.Abs(centerX - workArea.Right);
        double distTop = Math.Abs(centerY - workArea.Top);
        double distBottom = Math.Abs(centerY - workArea.Bottom);

        double snapX, snapY;

        // 找到最近的水平边缘
        if (distLeft < distRight)
            snapX = workArea.Left;
        else
            snapX = workArea.Right - width;

        // 找到最近的垂直边缘
        if (distTop < distBottom)
            snapY = workArea.Top;
        else
            snapY = workArea.Bottom - height;

        // 选择距离更近的那个轴进行吸附
        if (distLeft < distRight && distLeft < distTop && distLeft < distBottom)
            return new Point(workArea.Left, top);
        else if (distRight < distLeft && distRight < distTop && distRight < distBottom)
            return new Point(workArea.Right - width, top);
        else if (distTop < distBottom)
            return new Point(left, workArea.Top);
        else
            return new Point(left, workArea.Bottom - height);
    }

    /// <summary>
    /// 获取屏幕中心位置
    /// </summary>
    public Point GetScreenCenter(double width, double height)
    {
        var workArea = GetWorkingArea();
        return new Point(
            workArea.Left + (workArea.Width - width) / 2,
            workArea.Top + (workArea.Height - height) / 2
        );
    }

    /// <summary>
    /// 获取屏幕右下角位置
    /// </summary>
    public Point GetBottomRight(double width, double height)
    {
        var workArea = GetWorkingArea();
        return new Point(
            workArea.Right - width - 10,
            workArea.Bottom - height - 10
        );
    }
}
