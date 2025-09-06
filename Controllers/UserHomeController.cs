using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using GroceryMvc.Models;

namespace GroceryMvc.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "User")
                return RedirectToAction("AccessDenied", "Login");
            return View();
        }
    }
}
