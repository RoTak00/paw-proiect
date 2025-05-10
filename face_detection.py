from mtcnn import MTCNN
import cv2
import os
import argparse

folder_name = "output_face_detection"
os.makedirs(folder_name, exist_ok=True)

# Create the parser
parser = argparse.ArgumentParser(description="Face detection tool")

# Add arguments
parser.add_argument('input_path', type=str, help="The path of the input image")
args = parser.parse_args()

image_path = args.input_path
output_path = folder_name + '/' + os.path.basename(image_path).split('.')[0] + ".png"

image = cv2.imread(image_path)

# Initialize MTCNN face detector
detector = MTCNN()

# Detect faces
faces = detector.detect_faces(image)

# Draw rectangles around faces
for face in faces:
    x, y, w, h = face['box']
    cv2.rectangle(image, (x, y), (x+w, y+h), (0, 255, 0), 2)

cv2.imwrite(output_path, image)