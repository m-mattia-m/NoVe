using System;
namespace NoVe.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FachId { get; set; }
        public int FachbereichId { get; set; }
        public float Notenwert { get; set; }
        public int Semester { get; set; }
    }
}
