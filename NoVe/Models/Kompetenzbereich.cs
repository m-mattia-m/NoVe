using System;
namespace NoVe.Models
{
    public class Kompetenzbereich
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gewichtung { get; set; }
        public float Rundung { get; set; }
        public int BerufId { get; set; }
    }
}