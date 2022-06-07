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
        public Kompetenzbereich Kompetenzbereich { get; set; }
        public List<NoteView> NoteView { get; set; }
    }
}