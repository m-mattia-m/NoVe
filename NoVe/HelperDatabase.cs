using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NoVe.Models;

public class DatabaseHelper : DbContext
{

    public DbSet<User> User { get; set; }
    public DbSet<Beruf> Beruf { get; set; }
    public DbSet<Fach> Fach { get; set; }
    public DbSet<Klasse> Klasse { get; set; }
    public DbSet<Kompetenzbereich> Kompetenzbereich { get; set; }
    public DbSet<Note> Note { get; set; }

    //public DatabaseHelper(DbContextOptions<DatabaseHelper> options) : base(options) { }
    public DatabaseHelper(DbContextOptions<DatabaseHelper> options) : base(options) {
        Database.Migrate();
    }
}
