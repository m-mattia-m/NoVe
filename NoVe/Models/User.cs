﻿using System;
namespace NoVe.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public Klasse Klasse { get; set; }
        public int VerificationKey { get; set; }
        public int VerificationStatus { get; set; }
        public int AdminVerification { get; set; }
        public string Role { get; set; }
        public string LehrmeisterEmail { get; set; }
        public string Firma { get; set; }
    }
}
