﻿using System;
namespace NoVe.Models
{
    public class Fach
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int KompetenzbereichId { set; get; }
        public int Gewichtung { get; set; }
        public double Rundung { get; set; }
    }
}

