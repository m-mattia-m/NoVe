using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoVe.Models;

namespace NoVe.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DatabaseHelper dbContext, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            CreateFirstAdmin();
            InitAllowdDomains();
            return View();
        }

        private void CreateFirstAdmin()
        {
            if (_dbContext.Users.Where(u => u.Email == "admin@admin.ch").Count() == 0)
            {
                User user = new User();
                user.Email = "admin@admin.ch";
                user.PasswordHash = AccountController.hashPassword("admin");
                user.VerificationKey = 123456;
                user.AdminVerification = 1;
                user.VerificationStatus = 1;
                user.Role = "admin";
                _dbContext.Add(user);
                _dbContext.SaveChanges();
            }
        }

        private void InitAllowdDomains()
        {
            if (_dbContext.Domains.Count() == 0)
            {
                Domains domain = new Domains();
                domain.AllowedDomains = "gbssg.ch";
                _dbContext.Add(domain);
                _dbContext.SaveChanges();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
