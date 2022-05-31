using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoVe.Models;

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
    }
}
