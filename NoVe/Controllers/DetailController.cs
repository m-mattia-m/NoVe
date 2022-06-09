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
            return View("Detail", _dbContext.Users.Include(x => x.Klasse).FirstOrDefault(x => x.Email == Email));
        }

        public IActionResult Index(int ID)
        {
            return View("Detail", _dbContext.Users.Include(x => x.Klasse).FirstOrDefault(u => u.Id == ID));
        }

        public IActionResult DetailKlasse(int ID)
        {
            return View("DetailKlasse", _dbContext.Klasses.Include(x => x.Users).FirstOrDefault(u => u.Id == ID));
        }
    }
}
