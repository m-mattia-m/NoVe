using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoVe.Models
{
    [NotMapped]
    public class FachKompetenzbereich
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; }
        public string UserName { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime EndDatum { get; set; }
        public int InEditTime { get; set; }
        public Kompetenzbereich Kompetenzbereich { get; set; }
        public List<NoteView> NoteView { get; set; }
        public float NotenwertKompetenzbereich { get; set; }
        public float NotenwertGesamt { get; set; }
    }
}