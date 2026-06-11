"""
泽小安动画帧生成器 v4
- 使用已抠好的透明图片
- 生成呼吸动画
"""

from PIL import Image
import os
import math

# 配置
INPUT_IMAGE = r"D:\海底的海底\vibe coding项目\桌面宠物\抠图版形象.png"
OUTPUT_DIR = r"D:\海底的海底\vibe coding项目\桌面宠物\assets\characters\zexiaoan"

TARGET_HEIGHT = 300  # 目标高度

def resize_image(img, target_height):
    """等比缩放"""
    width, height = img.size
    ratio = target_height / height
    new_width = int(width * ratio)
    return img.resize((new_width, target_height), Image.Resampling.LANCZOS)

def create_idle_frames(base_img, num_frames=10, amplitude=5):
    """创建待机动画（呼吸+轻微摇摆）"""
    frames = []
    width, height = base_img.size

    canvas_height = height + amplitude * 2 + 10
    canvas_width = width + 20

    for i in range(num_frames):
        canvas = Image.new("RGBA", (canvas_width, canvas_height), (0, 0, 0, 0))

        # 呼吸：上下移动
        offset_y = amplitude * math.sin(2 * math.pi * i / num_frames)
        # 摇摆：左右微动
        offset_x = 2 * math.sin(2 * math.pi * i / num_frames * 0.5)

        paste_x = int(10 + offset_x)
        paste_y = int(amplitude + 5 - offset_y)

        canvas.paste(base_img, (paste_x, paste_y), base_img)
        frames.append(canvas)

    return frames

def save_frames(frames, output_dir, state_name):
    """保存帧"""
    state_dir = os.path.join(output_dir, state_name)
    os.makedirs(state_dir, exist_ok=True)

    for i, frame in enumerate(frames):
        filename = f"frame_{i+1:03d}.png"
        filepath = os.path.join(state_dir, filename)
        frame.save(filepath, "PNG")

    print(f"  保存 {len(frames)} 帧到 {state_name}/")

def main():
    print("=" * 50)
    print("泽小安动画帧生成器 v4")
    print("=" * 50)

    # 读取抠好的图片
    print(f"\n1. 读取抠图...")
    base_img = Image.open(INPUT_IMAGE)
    print(f"   原始尺寸: {base_img.size}")
    print(f"   模式: {base_img.mode}")

    # 缩小到合适尺寸
    print(f"\n2. 缩放到 {TARGET_HEIGHT}px...")
    base_img = resize_image(base_img, TARGET_HEIGHT)
    print(f"   新尺寸: {base_img.size}")

    # 保存透明图标
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    icon_path = os.path.join(OUTPUT_DIR, "icon.png")
    base_img.save(icon_path, "PNG")
    print(f"   保存: {icon_path}")

    # 生成待机动画（呼吸+摇摆）
    print("\n3. 生成待机动画 (idle)...")
    idle_frames = create_idle_frames(base_img, num_frames=10, amplitude=5)
    save_frames(idle_frames, OUTPUT_DIR, "idle")

    # 生成拖拽状态
    print("\n4. 生成拖拽状态 (drag)...")
    drag_dir = os.path.join(OUTPUT_DIR, "drag")
    os.makedirs(drag_dir, exist_ok=True)
    base_img.save(os.path.join(drag_dir, "frame_001.png"), "PNG")
    print("  保存 1 帧到 drag/")

    print("\n" + "=" * 50)
    print("完成！")
    print(f"角色目录: {OUTPUT_DIR}")
    print("=" * 50)

if __name__ == "__main__":
    main()
