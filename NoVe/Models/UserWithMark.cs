using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoVe.Models
{
    [NotMapped]
    public class UserWithMark
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public Klasse Klasse { get; set; }
        public string Role { get; set; }
        public string LehrmeisterEmail { get; set; }
        public string Firma { get; set; }
        public Boolean archived { get; set; }
        public double NotenWert { get; set; }
    }
}
