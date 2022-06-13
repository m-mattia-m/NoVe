using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            InitData();
            return View();
        }

        private void InitData()
        {
            InitAllowdDomains();
            InitBerufe();
            InitKlasse();
            InitUsers();
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

        private void InitBerufe()
        {
            if (_dbContext.Berufs.Count() == 0 && _dbContext.Berufs.Where(u => u.Name == "Informatiker Applikationsentwicklung").Count() == 0)
            {
                Beruf beruf = new Beruf();
                beruf.Name = "Informatiker Applikationsentwicklung";

                _dbContext.Berufs.Add(beruf);
                _dbContext.SaveChanges();
                Beruf dbBeruf = _dbContext.Berufs.Where(u => u.Name == "Informatiker Applikationsentwicklung").FirstOrDefault();

                // Kompetenzbereiche

                Kompetenzbereich igk = new Kompetenzbereich();
                igk.BerufId = dbBeruf.Id;
                igk.Name = "Informatik Grundkompetenzen";
                igk.Rundung = 0.5;
                igk.Gewichtung = 30;

                Kompetenzbereich gk = new Kompetenzbereich();
                gk.BerufId = dbBeruf.Id;
                gk.Name = "Grundkompetenzen";
                gk.Rundung = 0.5;
                gk.Gewichtung = 20;

                Kompetenzbereich abu = new Kompetenzbereich();
                abu.BerufId = dbBeruf.Id;
                abu.Name = "Allgemeinbildnender Unterricht";
                abu.Rundung = 0.1;
                abu.Gewichtung = 20;

                Kompetenzbereich ipa = new Kompetenzbereich();
                ipa.BerufId = dbBeruf.Id;
                ipa.Name = "Informatik Projektarbeit";
                ipa.Rundung = 0.1;
                ipa.Gewichtung = 30;

                _dbContext.Kompetenzbereichs.Add(igk);
                _dbContext.Kompetenzbereichs.Add(gk);
                _dbContext.Kompetenzbereichs.Add(abu);
                _dbContext.Kompetenzbereichs.Add(ipa);
                _dbContext.SaveChanges();
                Kompetenzbereich dbIgk = _dbContext.Kompetenzbereichs.Where(u => u.Name == "Informatik Grundkompetenzen").FirstOrDefault();
                Kompetenzbereich dbGk = _dbContext.Kompetenzbereichs.Where(u => u.Name == "Grundkompetenzen").FirstOrDefault();
                Kompetenzbereich dbAbu = _dbContext.Kompetenzbereichs.Where(u => u.Name == "Allgemeinbildnender Unterricht").FirstOrDefault();
                Kompetenzbereich dbIpa = _dbContext.Kompetenzbereichs.Where(u => u.Name == "Informatik Projektarbeit").FirstOrDefault();

                // Fächer

                Fach m100 = new Fach();
                m100.KompetenzbereichId = dbIgk.Id;
                m100.Name = "M100";
                m100.Rundung = 0.5;
                m100.Gewichtung = 25;
                Fach m104 = new Fach();
                m104.KompetenzbereichId = dbIgk.Id;
                m104.Name = "M104";
                m104.Rundung = 0.5;
                m104.Gewichtung = 25;
                Fach m114 = new Fach();
                m114.KompetenzbereichId = dbIgk.Id;
                m114.Name = "M114";
                m114.Rundung = 0.5;
                m114.Gewichtung = 25;
                Fach m117 = new Fach();
                m117.KompetenzbereichId = dbIgk.Id;
                m117.Name = "M117";
                m117.Rundung = 0.5;
                m117.Gewichtung = 25;

                Fach en1 = new Fach();
                en1.KompetenzbereichId = dbGk.Id;
                en1.Name = "Englisch S1";
                en1.Rundung = 0.1;
                en1.Gewichtung = 16;
                Fach en2 = new Fach();
                en2.KompetenzbereichId = dbGk.Id;
                en2.Name = "Englisch S2";
                en2.Rundung = 0.1;
                en2.Gewichtung = 16;
                Fach wir3 = new Fach();
                wir3.KompetenzbereichId = dbGk.Id;
                wir3.Name = "Wirtschaft S3";
                wir3.Rundung = 0.5;
                wir3.Gewichtung = 17;
                Fach wir4 = new Fach();
                wir4.KompetenzbereichId = dbGk.Id;
                wir4.Name = "Wirtschaft S4";
                wir4.Rundung = 0.5;
                wir4.Gewichtung = 17;
                Fach mt1 = new Fach();
                mt1.KompetenzbereichId = dbGk.Id;
                mt1.Name = "Mathematik S1";
                mt1.Rundung = 0.1;
                mt1.Gewichtung = 17;
                Fach nw2 = new Fach();
                nw2.KompetenzbereichId = dbGk.Id;
                nw2.Name = "Naturwissenschaften S2";
                nw2.Rundung = 0.1;
                nw2.Gewichtung = 17;

                Fach suk1 = new Fach();
                suk1.KompetenzbereichId = dbAbu.Id;
                suk1.Name = "Sprache und Kommunikation S1";
                suk1.Rundung = 0.5;
                suk1.Gewichtung = 16;
                Fach suk2 = new Fach();
                suk2.KompetenzbereichId = dbAbu.Id;
                suk2.Name = "Sprache und Kommunikation S2";
                suk2.Rundung = 0.5;
                suk2.Gewichtung = 16;
                Fach ges1 = new Fach();
                ges1.KompetenzbereichId = dbAbu.Id;
                ges1.Name = "Gesellschaft S1";
                ges1.Rundung = 0.5;
                ges1.Gewichtung = 17;
                Fach ges2 = new Fach();
                ges2.KompetenzbereichId = dbAbu.Id;
                ges2.Name = "Gesellschaft S2";
                ges2.Rundung = 0.5;
                ges2.Gewichtung = 17;
                Fach va = new Fach();
                va.KompetenzbereichId = dbAbu.Id;
                va.Name = "Vertiefungsarbeit";
                va.Rundung = 0.5;
                va.Gewichtung = 17;
                Fach sp = new Fach();
                sp.KompetenzbereichId = dbAbu.Id;
                sp.Name = "Schlussprüfung";
                sp.Rundung = 0.5;
                sp.Gewichtung = 17;

                Fach ipafach = new Fach();
                ipafach.KompetenzbereichId = dbIpa.Id;
                ipafach.Name = "IPA";
                ipafach.Rundung = 0.5;
                ipafach.Gewichtung = 100;

                _dbContext.Fachs.Add(m100);
                _dbContext.Fachs.Add(m104);
                _dbContext.Fachs.Add(m114);
                _dbContext.Fachs.Add(m117);
                _dbContext.Fachs.Add(en1);
                _dbContext.Fachs.Add(en2);
                _dbContext.Fachs.Add(wir3);
                _dbContext.Fachs.Add(wir4);
                _dbContext.Fachs.Add(mt1);
                _dbContext.Fachs.Add(nw2);
                _dbContext.Fachs.Add(suk1);
                _dbContext.Fachs.Add(suk2);
                _dbContext.Fachs.Add(ges1);
                _dbContext.Fachs.Add(ges2);
                _dbContext.Fachs.Add(va);
                _dbContext.Fachs.Add(sp);
                _dbContext.Fachs.Add(ipafach);
                _dbContext.SaveChanges();
            }
        }

        private void InitKlasse()
        {

            if (_dbContext.Klasses.Count() == 0 && _dbContext.Klasses.Where(u => u.KlasseName == "INA19a").Count() == 0)
            {
                Beruf beruf = _dbContext.Berufs.Where(u => u.Name == "Informatiker Applikationsentwicklung").FirstOrDefault();

                Klasse klasse = new Klasse();
                klasse.KlasseName = "INA19a";
                klasse.BerufId = beruf.Id;
                klasse.KlassenInviteCode = 123456;
                klasse.Startdatum = DateTime.ParseExact("01/01/2022 12:00", "dd/MM/yyyy HH:mm", null);
                klasse.EndDatum = DateTime.ParseExact("31/07/2022 12:00", "dd/MM/yyyy HH:mm", null);
                _dbContext.Klasses.Add(klasse);
                _dbContext.SaveChanges();
            }
        }

        private void InitUsers()
        {
            if (_dbContext.Users.Count() <= 4 && _dbContext.Users.Where(u => u.Email == "admin@admin.ch").Count() == 0)
            {
                User user = new User();
                user.Email = "admin@admin.ch";
                user.PasswordHash = AccountController.hashPassword("admin");
                user.Vorname = "Admin";
                user.Nachname = "Admin";
                user.VerificationKey = 123456;
                user.AdminVerification = 1;
                user.VerificationStatus = 1;
                user.Role = "admin";
                _dbContext.Add(user);
                _dbContext.SaveChanges();
            }
            if (_dbContext.Users.Count() <= 4 && _dbContext.Users.Where(u => u.Email == "max.lehrer@gbssg.ch").Count() == 0)
            {
                User user = new User();
                user.Email = "max.lehrer@gbssg.ch";
                user.PasswordHash = AccountController.hashPassword("Nove1234!");
                user.Vorname = "Max";
                user.Nachname = "Lehrer";
                user.VerificationKey = 123456;
                user.AdminVerification = 1;
                user.VerificationStatus = 1;
                user.Role = "lehrer";
                user.Klasse = _dbContext.Klasses.Where(u => u.KlassenInviteCode == 123456).FirstOrDefault();
                _dbContext.Add(user);
                _dbContext.SaveChanges();
            }
            if (_dbContext.Users.Count() <= 4 && _dbContext.Users.Where(u => u.Email == "max.schueler@edu.gbssg.ch").Count() == 0)
            {
                User user = new User();
                user.Email = "max.schueler@edu.gbssg.ch";
                user.PasswordHash = AccountController.hashPassword("Nove1234!");
                user.Vorname = "Max";
                user.Nachname = "Schueler";
                user.VerificationKey = 123456;
                user.AdminVerification = 1;
                user.VerificationStatus = 1;
                user.LehrmeisterEmail = "max.berufsbildner@gbssg.ch";
                user.Klasse = _dbContext.Klasses.Where(u => u.KlassenInviteCode == 123456).FirstOrDefault();
                user.Role = "schueler";
                _dbContext.Add(user);
                _dbContext.SaveChanges();
            }
            if (_dbContext.Users.Count() <= 4 && _dbContext.Users.Where(u => u.Email == "max.berufsbildner@firma.ch").Count() == 0)
            {
                User user = new User();
                user.Email = "max.berufsbildner@firma.ch";
                user.PasswordHash = AccountController.hashPassword("Nove1234!");
                user.Vorname = "Max";
                user.Nachname = "Berufsbildner";
                user.VerificationKey = 123456;
                user.AdminVerification = 1;
                user.VerificationStatus = 1;
                user.Role = "berufsbildner";
                _dbContext.Add(user);
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
