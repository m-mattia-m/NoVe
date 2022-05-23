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
        public int KlassenId { get; set; }
        public int VerificationKey { get; set; }
        public int VerificationStatus { get; set; }
        public int AdminVerification { get; set; }
    }
}
