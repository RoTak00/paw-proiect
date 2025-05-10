import cv2
import os
import argparse

folder_name = "output_contour_detection_v1"
os.makedirs(folder_name, exist_ok=True)


# Create the parser
parser = argparse.ArgumentParser(description="Contour detection tool")

# Add arguments
parser.add_argument('input_path', type=str, help="The path of the input image")
args = parser.parse_args()

image_path = args.input_path
output_path = folder_name + '/' + os.path.basename(image_path).split('.')[0] + ".png"

image = cv2.imread(image_path)

gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# Option 2: Canny (more dynamic)
edges = cv2.Canny(gray, 100, 200)

# Find contours
contours, _ = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Draw contours on the original image
cv2.drawContours(image, contours, -1, (0, 255, 0), 1)

# Save or display the result
cv2.imwrite(output_path, image)