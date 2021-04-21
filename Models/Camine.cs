using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class Camine
    {
        public Camine()
        {
            Administratori = new HashSet<Administratori>();
            Camere = new HashSet<Camere>();
        }

        public int IdCamin { get; set; }
        public string Adresa { get; set; }
        public int NrCamere { get; set; }
        public int NrLocuriNormale { get; set; }
        public int NrLocuriErasmus { get; set; }
        public int NrLocuriSocial { get; set; }
        public string Facultate { get; set; }
        public string Descriere { get; set; }
        //public string Images { get; set; }
        public virtual ICollection<Administratori> Administratori { get; set; }
        public virtual ICollection<Camere> Camere { get; set; }
    }
}
