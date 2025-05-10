# Python scripts for image transformations

## Libraries
- run the script from [the notebook](./install.ipynb) in the environment to install the needed libraries

## Adding images
- to add some random images run [adding_images script](./adding_images.py)
- will create a `input_images` directory with 100 images:
  - 50 with size 400x600 (`random_image?.jpg`)
  - 50 with size 1200x1200 (`random_image_big?.jpg`)

## Background removal
- [script](./background_removal.py)
- usage: `python background_removal.py input_path`
- works fine

## Contour detection
- 3 versions (I do not know that works better)
- usage: `python contour_detection_v?.py input_path`
- [script v1](./contour_detection_v1.py)
- [script v2](./contour_detection_v2.py)
- [script v3](./contour_detection_v3.py)

## Object detection
- Detectron2 model from facebook (https://github.com/facebookresearch/detectron2)
- usage: `python object_detection.py input_path` 
- [script](./object_detection.py)

## Face detection
- 2 versions (the second one works better)
- usage:  `python face_detection.py input_path`
- [script v1](./face_detection.py)
- [script v2](./face_detection_v2.py)

## Resizing
- usage: `python resize.py input_path -w WIDTH -H HEIGHT`
- [script](./resize.py)

## Text recognition
- work really good, but the words are not in order 
- usage: `python text_recognition.py input_path`
- [script](./text_recognition.py)

## Color space transformation
- usage: `python color_space_transformation.py input_path GRAY|BGR|HSV`
- [script](./color_space_transformation.py)