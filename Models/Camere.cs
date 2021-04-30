using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class Camere
    {
        public Camere()
        {
            Student = new HashSet<Student>();
        }

        public int IdCamera { get; set; }
        public int? IdCamin { get; set; }
        public int LimitaNrStudenti { get; set; }
        public int NrStudentiCazati { get; set; }
        public string Descriere { get; set; }
        public int NrCamera { get; set; }
        public virtual Camine IdCaminNavigation { get; set; }
        public virtual ICollection<Student> Student { get; set; }
    }
}
