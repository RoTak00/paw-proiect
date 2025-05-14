# Image helper

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

## TODO list

### Visuals
- [ ] color theme
- [ ] logo
- [ ] page style

### Database
- [x] Users table
- [x] ~~AuthenticationToken table (if using auth token style auth - simplest)~~ Using ASP.NET Identity instead
- [x] ~~FileOwners~~ Files table (which links file to user) 

### Pages
- [x] login
- [x] register
- [ ] ~~log out~~
- [ ] ~~upload image~~ 
- [x] convert image (with all the actions - includes upload)
- [ ] see my files 

### Functions 
- [x] background removal
- [x] contour detection
- [ ] smoothing
- [x] object detection -> if possible in time limit
- [x] face detection -> if possible in time limit
- [ ] resizing (downscaling and upscaling)
- [ ] text recognition -> if possible in time limit
- [ ] color space transformation

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

