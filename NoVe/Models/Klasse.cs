using System;
namespace NoVe.Models
{
    public class Klasse
    {
        public int Id { get; set; }
        public string KlasseName { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime EndDatum { get; set; }
        public int BerufId { get; set; }
    }
}