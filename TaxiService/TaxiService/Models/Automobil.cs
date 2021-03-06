﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public enum AutoTip { Neodredjen, Putnicki, Kombi}

    //Tekst zadatka ne nalaze ko je zaduzen za kreaciju automobila
    //Stoga, tretiram ih kao dispecere
    public class Automobil
    {
        public string VozacUsername { get; set; }
        public int Godiste { get; set; }
        public string Registracija { get; set; }
        public string TaksiBroj { get; set; }
        public AutoTip Tip { get; set; }
    }
}