using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace NoVe.Controllers
{
    public class DetailController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public DetailController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult IndexMail(string Email)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View("Detail", _dbContext.Users.Include(x => x.Klasse).FirstOrDefault(x => x.Email == Email));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult Index(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View("Detail", _dbContext.Users.Include(x => x.Klasse).FirstOrDefault(u => u.Id == ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }

        public IActionResult DetailKlasse(int ID)
        {
            if (HttpContext.Session.GetString("_UserRole") == "admin")
            {
                return View("DetailKlasse", _dbContext.Klasses.Include(x => x.Users).FirstOrDefault(u => u.Id == ID));
            }
            else
            {
                ViewBag.Message = string.Format("Du hast keinen Zugriff auf die Seite.");
                return View("~/Views/Admin/message.cshtml");
            }
        }
    }
}
