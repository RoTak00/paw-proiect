# Image helper

This project is a web-based platform for uploading and processing images using a variety of server-side tools.
Users can upload image files, select from different processing options (such as background removal, object detection, resizing, etc.), 
and receive the processed results directly in the browser. 

Some tools also provide additional output, like detected text. 
The system supports temporary and persistent file storage, user authentication, and seamless integration with Python-based image processing scripts.

This project was built as the lab project for the Web Application Development course of the Faculty of Mathematics and Informatics, University of Bucharest, 3rd year BA, 2025,
by [Norina Alexandru](https://github.com/norryna07) and [Takacs Robert](https://www.roberttakacs.ro) 


## How to run

This project was built in JetBrains Rider on Ubuntu.

### Requirements 
- .NET 8 SDK or later
- MySQL (running locally)
- EF Core CLI tools (dotnet ef)

### JetBrains Rider 

- Open the solution file
- Open _appsettings.json_ and edit the connection settings __ConnectionStrings:DefaultConnection__ to match your MySQL connection
- Run the migrations running this in the terminal: ````dotnet ef database update````
- In the command line, run ````python3 ./PythonScripts/activate_venv.py````
  - This requires that a python 3.10 version is installed and usable on the device.
- Build and Run the project __(Shift + F10)__ 

### Visual Studio 

- Open the solution file
- Open _appsettings.json_ and edit the connection settings __ConnectionStrings:DefaultConnection__ to match your MySQL connection
- Run the migrations running this in the __Package Manager Console__: ```Update-Database```
- In the command line, run ````python3 ./PythonScripts/activate_venv.py````
  - This requires that a python 3.10 version is installed and usable on the device.
- Run the project __(F5)__

(This was not tested)

### CLI
Run the following: 
````
dotnet ef database update
python3 ./PythonScripts/activate_venv.py
dotnet run
````

(This was not tested)

### For testing purposes:

The development mailservice used for the application is [smtp4dev](https://github.com/rnwood/smtp4dev), which provides
mock mail sending utility without actually launching mails outside of the development environment.

It can be run using this command (SMTP port 2525)

```
docker run -d -p 3000:80 -p 2525:25 rnwood/smtp4dev
```

After which the Web UI can be accessed at the following link [http://localhost:3000](http://localhost:3000)

## Changelog

_02.05.2025 - Robert Takacs_

- Created MySQL database connection setup for local database connection.
    - Host: __localhost:3306__
    - Database: __paw_project__
    - User: __paw_project_user__
    - Password: __pawpa55!__
    - These values can be found and changed in _appsettings.json_
- Created connection message on index page to mark whether connection to MySQL database was successful or not.

---

_03.05.2025 - Robert Takacs_

- Configured __ASP.NET Identity__ with MySQL for secure authentication and role-based authorization
  - Seeded Account Roles in the database on startup - __User__ and __Admin__
  - Seeded test accounts in the database on startup - __Admin__ and __User__
  - Created MVC pages for __Register__ and __Login__
  - Created MVC pages to test authorization based on authentication and role __User/Profile__ and __Admin/Panel__
  - Created AccessDenied page for unauthorized requests

---

_06.05.2025 - Robert Takacs_

- Configured simple e-mail service using SmtpClient
- Configured application to connect to local smtp4dev container, where mails can be seen

---

_07.05.2025 - Robert Takacs_

- Implemented e-mail template functionality
- Created "Forgot your password" functionality

---

_12.05.2025 - Robert Takacs_

- Added file upload with drag & drop and preview thumbnail
- Added tool selection grid (currently mock tools)
- Showing result display with loading spinner and download button
- Save uploads in uploads folder

_13.05.2025 - Norina Alexandru_

- Implemented venv that allows Python Scripts to be run on the local server
- Implemented various image processing scripts in Python
- Created database models for Uploaded Files, Image Tools and Image Tasks
- Connected tool selection to present the real existent tools and connected them to the website
- Implemented database saving in the context of logged-in users.
- Created .venv setup guide as an .ipynb file 

_14.05.2025 - Robert Takacs_

- Created DBSeeder function that loads Tools into the database if not existent
- Created error message on tool AJAX fail
- Updated the .venv setup guide to function as a one-run .py file
- Updated .gitignore

_18.05.2025 - Norina Alexandru_

- Created My Images page
- Added Delete option for all the files

_18.05.2025 - Norina Alexandru_

- Integrated new color theme (light and dark) on bootstrap using SCSS
- Added theme toggle 
- Saved theme in cookies
- Saved theme in the database

_19.05.2025 - Norina Alexandru_

- Added logo and brand name
- Updated color theme for buttons
- Added profile settings:
  - change name and email
  - change password
  - delete account
- Rearranged menu bar

_19.05.2025 - Norina Alexandru_

- Modified delete image 
  - verified the user to be the same
  - delete the file from the server also
_18.05.2025 - Norina Alexandru_

- Created My Images page
- Added Delete option for all the files

_18.05.2025 - Norina Alexandru_

- Integrated new color theme (light and dark) on bootstrap using SCSS
- Added theme toggle 
- Saved theme in cookies
- Saved theme in the database

_19.05.2025 - Norina Alexandru_

- Added logo and brand name
- Updated color theme for buttons
- Added profile settings:
  - change name and email
  - change password
  - delete acount
- Rearranged menu bar

_20.05.2025 - Robert Takacs_

- Modified File Upload and Tool processes
  - File gets saved to the database exactly on upload, as temp file
  - URL gets updated with file token
  - Whenever a tool is clicked, the token is used to load the file and modify it
  - If the user reloads the page, the file is persistent
  - Once a tool is used on a file, it stops being marked as "temp"
  - Files are always saved to the database now, even if not logged
- Implemented file edit
  - Simply loads file token on the index page to load file from database
- Files now have a OriginalFileName which is used for file download
- Files marked as Temp are deleted periodically if they're older than 6 hours (possibly 8 because timezones)

_24.05.2025 - Norina Alexandru_

- Moved Logout link from nav
- Added download buttons for each image in My Images page
  - needed to update the way we save the output path for more clear code when using Path.Combine
- Added no images uploaded message

_24.05.2026 - Takacs Robert_

- Implement functionality for tools that need extra input (Color Space Transformation and Resize), or offer extra output (Text Detection)
- Better error handling if tool fails
- Removed aspect ratio requirement of resize
- Extra checks on file upload (file type and MIME type in the backend)
- Auto-scrolling to the results once a file finishes 

_26.05.2025 - Norina Alexandru_

- Created a draft for Privacy page
- Created admin panel with:
  - storage per user
  - storage per task
  - usage per task
  - usage per user
  - average task per uploaded file
- Created user manager with:
  - option for modifying: username, email, password, role and preferred theme
  - option for deleting user
