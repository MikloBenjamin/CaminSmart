using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AplicatieCamine.Models
{
	public partial class Applicant
	{
        public int IdApplicant { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Facultate { get; set; }
        public int Varsta { get; set; }
        public string Adresa { get; set; }
        public string Email { get; set; }
        public int An { get; set; }
    }
}
