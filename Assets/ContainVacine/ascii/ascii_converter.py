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

def main(image_path, new_width=100):
    image = Image.open(image_path)
    image = resize_image(image, new_width)
    image = grayify(image)

    ascii_str = pixels_to_ascii(image)
    pixel_count = len(ascii_str)
    ascii_img = "\n".join([ascii_str[index:(index+new_width)] for index in range(0, pixel_count, new_width)])

    print(ascii_img)

    with open("ascii_image.txt", "w") as f:
        f.write(ascii_img)

if __name__ == "__main__":
    import sys
    if len(sys.argv) > 1:
        main(sys.argv[1])
    else:
        print("사용법: python ascii_converter.py <이미지파일경로>")
