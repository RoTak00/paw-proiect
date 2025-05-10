import cv2
import numpy as np
from detectron2.engine import DefaultPredictor
from detectron2.config import get_cfg
from detectron2 import model_zoo
import os
import argparse

folder_name = "output_contour_detection_v3"
os.makedirs(folder_name, exist_ok=True)

# Create the parser
parser = argparse.ArgumentParser(description="Contour detection tool")

# Add arguments
parser.add_argument('input_path', type=str, help="The path of the input image")
args = parser.parse_args()

image_path = args.input_path
output_path = folder_name + '/' + os.path.basename(image_path).split('.')[0] + ".png"

# Initialize the Mask R-CNN model (from Detectron2)
cfg = get_cfg()
cfg.MODEL.DEVICE = "cpu"
cfg.merge_from_file(model_zoo.get_config_file("COCO-InstanceSegmentation/mask_rcnn_R_50_FPN_3x.yaml"))
cfg.MODEL.WEIGHTS = model_zoo.get_checkpoint_url("COCO-InstanceSegmentation/mask_rcnn_R_50_FPN_3x.yaml")
cfg.MODEL.ROI_HEADS.SCORE_THRESH_TEST = 0.3  # Confidence threshold
predictor = DefaultPredictor(cfg)

# Load the image
image = cv2.imread(image_path)

# Make predictions
outputs = predictor(image)

# Get the detected instances (masks and contours)
instances = outputs["instances"]
masks = instances.pred_masks.to("cpu").numpy()

# Draw contours for each mask
for mask in masks:
    contours, _ = cv2.findContours(mask.astype(np.uint8), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    cv2.drawContours(image, contours, -1, (0, 255, 0), 2)

# Save or display the result
cv2.imwrite(output_path, image)
