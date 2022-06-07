using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index(string Email)
        {
            return View("Detail", _dbContext.Users.FirstOrDefault(x => x.Email == Email));
        }

        public IActionResult Index(int ID)
        {
            return View("Detail", _dbContext.Users.FirstOrDefault(u => u.Id == ID));
        }

        public IActionResult DetailKlasse(int ID)
        {
            return View("DetailKlasse", _dbContext.Klasses.FirstOrDefault(u => u.Id == ID));
        }
    }
}
