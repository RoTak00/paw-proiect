using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PAW_Project.Controllers;


[Authorize]
public class UserController : Controller
{
    public IActionResult Profile()
    {
        return View();
    }
}