import requests
import os

folder_name = "input_images"
os.makedirs(folder_name, exist_ok=True)

for i in range(50):
    url = "https://picsum.photos/600/400"  # 600x400 random image
    response = requests.get(url)
    path = folder_name + "/random_image" + str(i) + ".jpg"
    with open(path, "wb") as f:
        f.write(response.content)

for i in range(50):
    url = "https://picsum.photos/1200/1200"  # 1200x1200 random image
    response = requests.get(url)
    path = folder_name + "/random_image_big" + str(i) + ".jpg"
    with open(path, "wb") as f:
        f.write(response.content)