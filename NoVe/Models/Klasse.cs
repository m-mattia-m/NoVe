using System;
using System.Collections.Generic;

namespace NoVe.Models
{
    public class Klasse
    {
        public int Id { get; set; }
        public string KlasseName { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime EndDatum { get; set; }
        public int BerufId { get; set; }
        public int KlassenInviteCode { get; set; }
        public List<User> Users { get; set; }
        public Boolean archived { get; set; }
    }
}
