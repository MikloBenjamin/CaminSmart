using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class Tichet
    {
        public int IdTichet { get; set; }
        public int? IdStudent { get; set; }
        public DateTime DataEmitere { get; set; }
        public DateTime? DateRezolvare { get; set; }
        public bool StatusTichet { get; set; }
        public string Detalii { get; set; }
        public bool TipTichet { get; set; }
        public int IdCamera { get; set; }
        public string Feedback { get; set; }
#nullable enable
        public string? FileName { get; set; }
#nullable disable
        public virtual Student IdStudentNavigation { get; set; }
       // public virtual Camere IdCameraNavigation { get; set; }

    }
}
