import cv2
import os
from PIL import Image

ASCII_CHARS = " .,-~:=$#@"

def resize_image(image, new_width=100):
    width, height = image.size
    aspect_ratio = height / width
    new_height = int(aspect_ratio * new_width * 0.5)
    return image.resize((new_width, new_height))

def grayify(image):
    return image.convert("L")

def pixels_to_ascii(image):
    pixels = image.getdata()
    chars = "".join([ASCII_CHARS[pixel * (len(ASCII_CHARS) - 1) // 255] for pixel in pixels])
    return chars

def image_to_ascii(image_path, new_width=100):
    image = Image.open(image_path)
    image = resize_image(image, new_width)
    image = grayify(image)
    ascii_str = pixels_to_ascii(image)
    pixel_count = len(ascii_str)
    ascii_img = "\n".join([ascii_str[index:(index+new_width)] for index in range(0, pixel_count, new_width)])
    return ascii_img

def video_to_ascii_interactive(video_path, frames_folder="frames", ascii_folder="ascii_txt", new_width=100):
    if not os.path.exists(frames_folder):
        os.makedirs(frames_folder)
    if not os.path.exists(ascii_folder):
        os.makedirs(ascii_folder)

    cap = cv2.VideoCapture(video_path)
    frame_count = 0
    success, frame = cap.read()

    # 한번만 물어보기
    save_all = None
    while save_all not in ('y', 'n'):
        save_all = input("모든 프레임을 텍스트 파일로 저장할까요? (y/n): ").strip().lower()

    while success:
        frame_filename = os.path.join(frames_folder, f"frame_{frame_count:05d}.png")
        cv2.imwrite(frame_filename, frame)

        ascii_art = image_to_ascii(frame_filename, new_width)

        print(f"\nFrame {frame_count} ASCII Art:\n")
        print(ascii_art)

        if save_all == 'y':
            txt_filename = os.path.join(ascii_folder, f"frame_{frame_count:05d}.txt")
            with open(txt_filename, "w", encoding="utf-8") as f:
                f.write(ascii_art)
            print(f"저장 완료: {txt_filename}")

        frame_count += 1
        success, frame = cap.read()

    cap.release()
    print(f"\n총 {frame_count}개의 프레임을 처리했습니다.")

if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("사용법: python video_to_ascii_interactive.py <영상파일> [가로문자수]")
    else:
        video_file = sys.argv[1]
        width = int(sys.argv[2]) if len(sys.argv) > 2 else 100
        video_to_ascii_interactive(video_file, new_width=width)
