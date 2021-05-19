using System;
using System.Collections.Generic;
using System.Text.Json;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class Student
    {
        public Student()
        {
            Tichet = new HashSet<Tichet>();
        }

        public int IdStudent { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Facultate { get; set; }
        public int Varsta { get; set; }
        public string Adresa { get; set; }
        public string Email { get; set; }
        public int An { get; set; }
        public int? IdCamera { get; set; }
        public DateTime? DataCazare { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Camere IdCameraNavigation { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Tichet> Tichet { get; set; }
    }
}