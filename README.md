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
- Build and Run the project __(Shift + F10)__ 

### Visual Studio 

- Open the solution file
- Open _appsettings.json_ and edit the connection settings __ConnectionStrings:DefaultConnection__ to match your MySQL connection
- Run the migrations running this in the __Package Manager Console__: ```Update-Database```
- Run the project __(F5)__

(This was not tested)

### CLI
Run the following: 
````
dotnet ef database update
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
- [ ] ~~FileOwners~~ Files table (which links file to user) 

### Pages
- [x] login
- [x] register
- [ ] ~~log out~~
- [ ] ~~upload image~~ 
- [ ] convert image (with all the actions - includes upload)
- [ ] see my files 

### Functions 
- [ ] background removal
- [ ] contour detection
- [ ] smoothing
- [ ] object detection -> if possible in time limit
- [ ] face detection -> if possible in time limit
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