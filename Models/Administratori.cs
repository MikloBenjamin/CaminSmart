using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class Administratori
    {
        public int IdAdmin { get; set; }
        public int? IdCamin { get; set; }
        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string NrTelefon { get; set; }
        public string Email { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Camine IdCaminNavigation { get; set; }
    }
}
