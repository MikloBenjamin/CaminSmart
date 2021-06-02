using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using AplicatieCamine.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace AplicatieCamine
{
    public class StudentController : Controller
    {
        private readonly DBSistemContext _context;
        int id_student = 1;
        public StudentController(DBSistemContext context, IWebHostEnvironment env)
        {
            _context = context;
            GlobalVariables.env = env.WebRootPath;
        }

        private void setup()
		{
            id_student = (_context.Student.Count() > 0 ? _context.Student.ToList().Last().IdStudent + 1 : 1);
            if(_context.Student.Count() > 0)
			{
                id_student = _context.Student.ToList().Last().IdStudent + 1;
                System.Diagnostics.Debug.WriteLine("ID_STUDENT = " + id_student);
                System.Diagnostics.Debug.WriteLine("CONTEXT STUDENT COUNT = " + _context.Student.Count());
			}

            GlobalVariables.IsAdmin = true;

            if (!char.IsDigit(User.Identity.Name.Split("@")[0][^1]))
            {
                GlobalVariables.IsAdmin = true;
            }
            //GlobalVariables.IsAdmin = (_context.Administratori.Where(entry => entry.Email == User.Identity.Name).Count() > 0 ? true : false);
            var model = _context.Student.Where(st => st.Email == User.Identity.Name).Select(st => st).FirstOrDefault();
            if (model != null)
            {
                GlobalVariables.Student = model;
            }
            GlobalVariables.isSetUp = true;
        }

        public IActionResult Home(int? id)
		{
            if(GlobalVariables.isSetUp == false)
			{
                setup();
			}
            return View("/Views/Home/Index.cshtml", GlobalVariables.Student);
		}
        
        // GET: Student
        public async Task<IActionResult> Index()
        {            
            return View(await _context.Student.Include(s => s.IdCameraNavigation).ToListAsync());
        }

        public IActionResult Status(int ?id)
		{
            if(GlobalVariables.Student != null || id != null)
			{
                return View(GlobalVariables.Student);
			}
            return RedirectToAction("Inscriere", "Applicant");
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.IdCameraNavigation)
                .FirstOrDefaultAsync(m => m.IdStudent == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera");
            return View();
        }

        // GET: Student/Statistica
        public IActionResult Statistica()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStudent,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,IdCamera,DataCazare")] Student student)
        {
            if (ModelState.IsValid)
            {
                var camera = _context.Camere.Where(entry => entry.IdCamera == student.IdCamera).Select(entry => new { limit = entry.LimitaNrStudenti, cazati = entry.NrStudentiCazati }).FirstOrDefault();
                if (camera.limit > camera.cazati)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    id_student++;
                    string controller = "Student", action = "Index";
                    return RedirectToAction("UpdateNrStudentCazati", "Camere", new { id = (int)student.IdCamera, raction = action, rcontroller = controller });
                }
            }
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera", student.IdCamera);
            return View(student);
        }


        private async Task<bool> DeleteApplicant(string bname, int id)
		{
            bname = bname + ".pdf";
            var aplc = _context.Applicant.Where(apl => apl.IdApplicant == id);
            if (aplc.Count() > 0)
            {
                _context.Remove(aplc.First());
                await _context.SaveChangesAsync();
            }
            BlobContainerClient containerClient = GlobalVariables.BlobClient.GetBlobContainerClient("inscrieri");
            System.Diagnostics.Debug.WriteLine(bname);
            await containerClient.DeleteBlobIfExistsAsync(bname);
            return true;
        }

        public async Task<IActionResult> AcceptApplicant(
            int id, string nume, string prenume, string facultate,
            string email, string adresa, int an, int varsta
           )
        {
            System.Diagnostics.Debug.WriteLine("Id student = " + id_student + "\n");
            Student student = new Student
            {
                IdStudent = _context.Student.ToList().Last().IdStudent + 1,
                Nume = nume,
                Prenume = prenume,
                Facultate = facultate,
                Adresa = adresa,
                Email = email,
                Varsta = varsta,
                An = an,
                DataCazare = DateTime.Now,
                IdCamera = -1
            };
            var camine = _context.Camine.ToList();
            int idCamin = -1, nrCamera = -1;
            foreach (var camin in camine)
            {
                bool found = false;
                var camere = await _context.Camere.Where(cam => cam.IdCamin == camin.IdCamin).ToListAsync();
                foreach (var camera in camere)
                {
                    if (camera.NrStudentiCazati < camera.LimitaNrStudenti)
                    {
                        found = true;
                        student.IdCamera = camera.IdCamera;
                        nrCamera = camera.NrCamera;
                        idCamin = camin.IdCamin;
                        break;
                    }
                }
                if (found == true)
                {
                    break;
                }
            }


            if (student.IdCamera != -1)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                await DeleteApplicant(nume + "_" + prenume + "_" + id, id);
                var client = new SendGridClient(GlobalVariables.SendGridApiKey);
                var from = new EmailAddress("florin.marut99@e-uvt.ro", "Florin");
                var to = new EmailAddress(student.Email, student.Prenume);
                var subject = "Cazare ACCEPTATA!";
                var plainTextContent = @"Buna, drag Student! 
								Am venit cu vesti bune, ai fost ACCEPTAT in caminul " + idCamin.ToString() + " si camera " + nrCamera.ToString() +  @"
								Te rog sa verifici statusul tau de cazare pe urmatorul link: " + "https://localhost:44383/Student/Status/" + student.IdStudent.ToString() + @"
                                Numai Bine!
								Asociatia Managementul in Camine UVT";
                var htmlContent = @"<b>Buna, drag Student!</b> <br>
								Am venit cu vesti bune, ai fost <b>ACCEPTAT</b> in caminul <b>" + idCamin.ToString() + "</b> si camera <b>" + nrCamera.ToString() + "</b><br>";
					htmlContent += "Te rog sa verifici statusul tau de cazare pe urmatorul link: <a href = \"https://localhost:44383/Student/Status/" + student.IdStudent.ToString() + "\" target = \"_blank\">https://localhost:44383/Student/Status/" + student.IdStudent.ToString() + @"</a><br>
                                Numai Bine!<br>
								Asociatia Managementul in Camine UVT";
                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent,
                    htmlContent
                    );
                await client.SendEmailAsync(msg);
                string controller = "Applicant", action = "Applicants";
                return RedirectToAction("UpdateNrStudentCazati", "Camere", new { id = (int)student.IdCamera, raction = action, rcontroller = controller });
            }
            return NotFound();
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera", student.IdCamera);
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStudent,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,IdCamera,DataCazare")] Student student)
        {
            if (id != student.IdStudent)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.IdStudent))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera", student.IdCamera);
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.IdCameraNavigation)
                .FirstOrDefaultAsync(m => m.IdStudent == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            var tichete = await _context.Tichet.Where(entry => entry.IdStudent == student.IdStudent).ToListAsync();
            foreach (var tichet in tichete)
            {
                System.Diagnostics.Debug.WriteLine(tichet.FileName);
                _context.Tichet.Remove(tichet);
                await GlobalVariables.BlobClient.GetBlobContainerClient("tichete").DeleteBlobIfExistsAsync(tichet.FileName);
            }
            var camera = _context.Camere.Where(entry => entry.IdCamera == student.IdCamera).First();
            camera.NrStudentiCazati -= 1;
            _context.Camere.Update(camera);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            CleanServer();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.IdStudent == id);
        }

        public IActionResult CleanServer()
		{
            BlobContainerClient containerClient = GlobalVariables.BlobClient.GetBlobContainerClient("inscrieri");
            var files = Directory.GetFiles(@"wwwroot/UploadFiles");
            var blobs = containerClient.GetBlobs().Select(bl => bl.Name);
            foreach (string file in files)
            {
                if (!blobs.Contains(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            containerClient = GlobalVariables.BlobClient.GetBlobContainerClient("tichete");
            files = Directory.GetFiles(@"wwwroot/TichetImages");
            blobs = containerClient.GetBlobs().Select(bl => bl.Name);
            foreach (string file in files)
            {
                System.Diagnostics.Debug.WriteLine(file);
                if (!blobs.Contains(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            return RedirectToAction("Home");
        }
    }
}