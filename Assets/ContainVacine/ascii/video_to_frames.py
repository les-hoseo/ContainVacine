import cv2
import os

def video_to_frames(video_path, output_folder):
    # 출력 폴더 없으면 생성
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    # 비디오 캡처 객체 생성
    cap = cv2.VideoCapture(video_path)
    
    frame_count = 0
    success, frame = cap.read()

    while success:
        # 파일 이름 형식: frame_00001.png
        filename = os.path.join(output_folder, f"frame_{frame_count:05d}.png")
        cv2.imwrite(filename, frame)

        frame_count += 1
        success, frame = cap.read()

    cap.release()
    print(f"총 {frame_count}개의 프레임이 '{output_folder}'에 저장되었습니다.")

if __name__ == "__main__":
    video_path = "sample.mp4"   # 변환할 영상 파일명
    output_folder = "frames"        # 저장할 폴더명
    video_to_frames(video_path, output_folder)
