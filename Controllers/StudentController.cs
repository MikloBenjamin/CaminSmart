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
using AplicatieCamine.Models;

namespace AplicatieCamine
{
    public class StudentController : Controller
    {
        private readonly DBSistemContext _context;
        int id_student = 1;
        public StudentController(DBSistemContext context)
        {
            _context = context;
            id_student = context.Student.ToList().Last().IdStudent + 1;
        }

        public IActionResult Home()
		{
            string user = User.Identity.Name;
            var model = _context.Student.Where(st => st.Email == user).Select(st => st).AsEnumerable();
            if(model.Count() == 0)
			{
                model = null;
			}
            return View("/Views/Home/Index.cshtml", model);
		}
        
        // GET: Student
        public async Task<IActionResult> Index()
        {
            var dBSistemContext = _context.Student.Include(s => s.IdCameraNavigation);
            return View(await dBSistemContext.ToListAsync());
        }

        public IActionResult Status()
		{
            var id = _context.Student.Where(st => st.Email == User.Identity.Name).Select(st => st);
            if(id.Count() > 0)
			{
                return View(id.First());
			}
            return View(null);
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

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStudent,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,IdCamera,DataCazare")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                id_student++;
                string controller = "Student", action = "Index";
                return RedirectToAction("UpdateNrStudentCazati", "Camere", new { id = (int)student.IdCamera, raction = action, rcontroller = controller });
            }
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera", student.IdCamera);
            return View(student);
        }

        private async Task<bool> DeleteApplicant(string bname, int id)
		{
            var aplc = _context.Applicant.Where(apl => apl.IdApplicant == id);
            if (aplc.Count() > 0)
            {
                _context.Remove(aplc.First());
            }
            await _context.SaveChangesAsync();
            BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
            if(containerClient.GetBlobClient(bname).Exists())
			{
                containerClient.DeleteBlob(bname);
			}
            return true;
		}

        public async Task<IActionResult> AcceptApplicant(
            int id, string nume, string prenume, string facultate, 
            string email, string adresa, int an, int varsta
           )
        {
            System.Diagnostics.Debug.WriteLine("Id student = " + id_student.ToString() + "\n");
            Student student = new Student
			{
				IdStudent = id_student++,
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
            foreach(var camin in camine)
			{
                bool found = false;
                var camere = _context.Camere.Where(cam => cam.IdCamin == camin.IdCamin).ToList();
                foreach(var camera in camere)
				{
                    if(camera.NrStudentiCazati < camera.LimitaNrStudenti)
					{
                        found = true;
                        student.IdCamera = camera.IdCamera;
                        break;
					}
				}
                if(found == true)
				{
                    break;
				}
			}


            if(student.IdCamera != -1)
			{
                _context.Add(student);
                await _context.SaveChangesAsync();
                await DeleteApplicant(nume + "_" + prenume + "_" + id, id);
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
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.IdStudent == id);
        }

        public IActionResult CleanServer()
		{
            BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
            var files = System.IO.Directory.GetFiles(@"wwwroot/UploadFiles");
            var blobs = containerClient.GetBlobs().Select(bl => bl.Name);
            foreach(string file in files)
			{
				if (!blobs.Contains(file))
				{
                    System.IO.File.Delete(file);
				}
			}

            containerClient = serviceClient.GetBlobContainerClient("tichete");
            files = System.IO.Directory.GetFiles(@"wwwroot/TichetImages");
            blobs = containerClient.GetBlobs().Select(bl => bl.Name);
            foreach (string file in files)
            {
                if (!blobs.Contains(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            return RedirectToAction("Home");
		}
    }
}
