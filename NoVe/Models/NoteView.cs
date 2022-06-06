using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoVe.Models
{
    [NotMapped]
    public class NoteView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FachId { get; set; }
        public string Fachname { get; set; }
        public float Noteid { get; set; }
        public float Notenwert { get; set; }
        public int Semester { get; set; }
        public int StudentAlreadyChanged { get; set; }
    }
}
