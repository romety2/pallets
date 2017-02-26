﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palety.Models
{
    [Serializable]
    public class Wydarzenie
    {
        public Wydarzenie()
        {

        }

        public Wydarzenie(ulong id, string data, BindingList<MojePalety> palety, Firma firma, string uwagi)
        {
            Id = id;
            Data = data;
            MPalety = palety;
            Firma = firma;
            Uwagi = uwagi;
        }

        [Serializable]
        public struct MojePalety
        {
            private Paleta paleta;
            private int plus;
            private int minus;

            public Paleta Paleta { get; set; }
            public int Plus { get; set; }
            public int Minus { get; set; }

            public MojePalety(Paleta paleta, int plus, int minus) : this()
            {
                Paleta = paleta;
                Plus = plus;
                Minus = minus;
            }
        }

        ulong id;
        Firma firma;
        BindingList<MojePalety> palety;
        string data;
        string uwagi;

        public ulong Id { get; set; }
        public Firma Firma { get; set; }
        public string Data { get; set; }
        public string WyswietlNazweFirme
        {
            get {
                    return this.Firma.Nazwa;
                }
        }
        public string WyswietlPalety
        {
              get   {
                        string p = "";
                        int r, ile = 0;
                        List<MojePalety> mp = palety.Where(o => o.Plus != 0 || o.Minus != 0).ToList();
                        foreach(MojePalety pm in mp)
                        {
                            ile++;
                            r = pm.Plus - pm.Minus;
                            p += pm.Paleta.Nazwa + ": ";
                            if (r > 0)
                                p += "+";
                            p += r.ToString();
                            if (mp.Count != ile)
                                p += ", ";
                        }
                        return p;
                    }
        }
        public string Uwagi { get; set; }

        public BindingList<MojePalety> MPalety
        {
            get { return palety; }
            set { palety = value; }
        }
    }
}
