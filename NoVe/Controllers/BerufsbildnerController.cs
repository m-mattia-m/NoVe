using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

using NoVe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Globalization;

namespace NoVe.Controllers
{
    public class BerufsbildnerController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        public BerufsbildnerController(DatabaseHelper dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View("Lernende", getLernende());
        }

        public IActionResult Lernende()
        {
            return View(getLernende());
        }

        public IActionResult notenEinsehen(int ID)
        {
            HttpContext.Session.SetInt32("_StudentID", ID);
            int studentID = (int)HttpContext.Session.GetInt32("_StudentID");
            Console.WriteLine("Noten einsehen von: " + studentID);

            return RedirectToAction("Index", "Noten");
        }

        private List<User> getLernende()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserID");
            User currentUser = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            //Set Student into Class - only for testing
            //Klasse klasse2 = _dbContext.Klasses.Where(k => k.Id == 1).FirstOrDefault();
            //User user2 = _dbContext.Users.Where(u => u.Id == 5).FirstOrDefault();
            //user2.Klasse = klasse2;
            //_dbContext.SaveChanges();

            List<User> lernendelist = _dbContext.Users.Include(x => x.Klasse).Where(u => u.LehrmeisterEmail == currentUser.Email).ToList();

            List<User> users = new List<User>(lernendelist.Count);

            foreach (User user in lernendelist)
            {
                if (user.Role == "schueler")
                {
                    users.Add(user);
                }
            }

            return users;
        }
    }
}
