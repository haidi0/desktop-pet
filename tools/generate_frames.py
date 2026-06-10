"""
泽小安动画帧生成器 v3
- 使用 floodfill 算法去除背景
- 更精确的背景检测
"""

from PIL import Image, ImageDraw
import os
import math

# 配置
INPUT_IMAGE = r"D:\微信\xwechat_files\wxid_yaf5ozpl0zpp12_0596\temp\RWTemp\2026-06\9e20f478899dc29eb19741386f9343c8\24f251f82ba722be309bfbeba4fa0741.png"
OUTPUT_DIR = r"D:\海底的海底\vibe coding项目\桌面宠物\assets\characters\zexiaoan"

TARGET_HEIGHT = 300

def floodfill_transparent(img, x, y, threshold=40):
    """从指定点开始 floodfill，将相似颜色变为透明"""
    img = img.convert("RGBA")
    pixels = img.load()
    width, height = img.size

    # 获取起始像素颜色
    start_color = pixels[x, y]
    visited = set()
    queue = [(x, y)]

    while queue:
        cx, cy = queue.pop(0)

        if (cx, cy) in visited:
            continue
        if cx < 0 or cx >= width or cy < 0 or cy >= height:
            continue

        visited.add((cx, cy))
        current = pixels[cx, cy]

        # 计算颜色差异
        diff = math.sqrt(
            (current[0] - start_color[0])**2 +
            (current[1] - start_color[1])**2 +
            (current[2] - start_color[2])**2
        )

        if diff < threshold:
            pixels[cx, cy] = (255, 255, 255, 0)  # 设为透明
            # 添加相邻像素
            queue.extend([
                (cx+1, cy), (cx-1, cy),
                (cx, cy+1), (cx, cy-1)
            ])

    return img

def remove_background_floodfill(img):
    """使用 floodfill 从四角去除背景"""
    width, height = img.size

    # 从四个角开始 floodfill
    corners = [
        (0, 0),           # 左上
        (width-1, 0),     # 右上
        (0, height-1),    # 左下
        (width-1, height-1)  # 右下
    ]

    for x, y in corners:
        print(f"  处理角点 ({x}, {y})...")
        img = floodfill_transparent(img, x, y, threshold=45)

    # 也从边缘中点开始
    edge_points = [
        (width//2, 0),      # 上中
        (width//2, height-1), # 下中
        (0, height//2),      # 左中
        (width-1, height//2)  # 右中
    ]

    for x, y in edge_points:
        print(f"  处理边缘点 ({x}, {y})...")
        img = floodfill_transparent(img, x, y, threshold=45)

    return img

def crop_to_content(img):
    """裁剪到内容区域"""
    img_rgb = img.convert("RGBA")
    bbox = img_rgb.getbbox()

    if bbox:
        # 添加一点边距
        padding = 5
        bbox = (
            max(0, bbox[0] - padding),
            max(0, bbox[1] - padding),
            min(img.width, bbox[2] + padding),
            min(img.height, bbox[3] + padding)
        )
        return img.crop(bbox)
    return img

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
    print("泽小安动画帧生成器 v3")
    print("=" * 50)

    # 读取图片
    print(f"\n1. 读取图片...")
    base_img = Image.open(INPUT_IMAGE)
    print(f"   原始尺寸: {base_img.size}")

    # 去除背景
    print("\n2. Floodfill 去除背景...")
    base_img = remove_background_floodfill(base_img)

    # 裁剪到内容
    print("\n3. 裁剪到内容区域...")
    base_img = crop_to_content(base_img)
    print(f"   裁剪后尺寸: {base_img.size}")

    # 缩小
    print(f"\n4. 缩放到 {TARGET_HEIGHT}px...")
    base_img = resize_image(base_img, TARGET_HEIGHT)
    print(f"   最终尺寸: {base_img.size}")

    # 保存
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    icon_path = os.path.join(OUTPUT_DIR, "icon.png")
    base_img.save(icon_path, "PNG")
    print(f"   保存: {icon_path}")

    # 生成动画
    print("\n5. 生成待机动画 (idle)...")
    idle_frames = create_idle_frames(base_img, num_frames=10, amplitude=5)
    save_frames(idle_frames, OUTPUT_DIR, "idle")

    # 拖拽状态
    print("\n6. 生成拖拽状态 (drag)...")
    drag_dir = os.path.join(OUTPUT_DIR, "drag")
    os.makedirs(drag_dir, exist_ok=True)
    base_img.save(os.path.join(drag_dir, "frame_001.png"), "PNG")
    print("  保存 1 帧到 drag/")

    print("\n" + "=" * 50)
    print("完成！")
    print("=" * 50)

if __name__ == "__main__":
    main()
