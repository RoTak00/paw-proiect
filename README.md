# Image helper

## TODO list

### Visuals
- [ ] color theme
- [ ] logo
- [ ] page style

### Database
- [ ] Users table
- [ ] AuthenticationToken table (if using auth token style auth - simplest)
- [ ] ~~FileOwners~~ Files table (which links file to user) 

### Pages
- [ ] login
- [ ] register
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

# Changelog

_02.05.2025 - Robert Takacs_

- Created MySQL database connection setup for local database connection.
    - Host: __localhost:3306__
    - Database: __paw_project__
    - User: __paw_project_user__
    - Password: __pawpa55!__
    - These values can be found and changed in _appsettings.json_
- Created connection message on index page to mark whether connection to MySQL database was successful or not.

---
