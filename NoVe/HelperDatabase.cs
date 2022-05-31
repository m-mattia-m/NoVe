using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NoVe.Models;

public class DatabaseHelper : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Beruf> Berufs { get; set; }
    public DbSet<Fach> Fachs { get; set; }
    public DbSet<Klasse> Klasses { get; set; }
    public DbSet<Kompetenzbereich> Kompetenzbereichs { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Domains> Domains { get; set; }

    /*public DatabaseHelper(DbContextOptions<DatabaseHelper> options) : base(options) { }*/
    /*public DatabaseHelper() { }*/
    public DatabaseHelper(DbContextOptions<DatabaseHelper> options) : base(options) {

        Database.Migrate();
    }

    ////Migration erstellen
    ///
    //1. Package Manager Console öffnen (View -> Other Windows -> Package Manager Console)
    //2. Befehl ausführen um Migration zu erstellen (nur wenn Models angepasst und dann auch Description in Befehl anpassen): Add-Migration Description
    //3. Befehl ausführen um Datenbank zu erstellen/updaten: Update-Database
}
