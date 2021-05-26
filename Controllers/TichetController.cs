using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AplicatieCamine.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace AplicatieCamine
{
    public class TichetController : Controller
    {
        private readonly DBSistemContext _context;
        int id_tichet = 1;
        int stud_id = 1;

        public TichetController(DBSistemContext context)
        {
            _context = context;
            var tichets = _context.Tichet.Select(a => a).AsEnumerable();
            id_tichet = 1;
            if(tichets.Count() > 0)
			{
                id_tichet = tichets.Last().IdTichet + 1;
			}
        }

        private void AddTichet(Tichet tichet)
		{
            _context.Add(tichet);
        }

        // GET: Tichets
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tichet.Include(t => t.IdStudentNavigation).ToListAsync());
        }

        [HttpPost]
        public IActionResult Index(string x = "")
        {
            return RedirectToAction("Tichete");
        }
        public IActionResult Tichete()
        {
            var stid = _context.Student.Where(a => a.Email == User.Identity.Name).Select(a => a.IdStudent).AsEnumerable();
            if(stid.Count() > 0)
			{
                var model = _context.Tichet.AsEnumerable();
                dynamic modell = new ExpandoObject();
                modell.Tichet = model;
                modell.Camera = -1;
                stud_id = stid.First();
                var cid = _context.Student.Where(a => a.IdStudent == stud_id).Select(a => a.IdCamera).First();
                var nrc = _context.Camere.Where(a => a.IdCamera == cid).Select(a => a.NrCamera).First();
                modell.Tichet = _context.Tichet.Where(a => a.IdStudent == stud_id).Select(a => a).OrderBy(a => !a.StatusTichet).AsEnumerable();
                modell.Camera = nrc;
                return View("Tichete", modell);
            }
            return RedirectToAction("Index", "Applicant");
        }

        // GET: Tichets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tichet = await _context.Tichet
                .Include(t => t.IdStudentNavigation)
                .FirstOrDefaultAsync(m => m.IdTichet == id);
            if (tichet == null)
            {
                return NotFound();
            }

            return View(tichet);
        }

        public async Task<IActionResult> StudentTichetDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tichet = await _context.Tichet
                .Include(t => t.IdStudentNavigation)
                .FirstOrDefaultAsync(m => m.IdTichet == id);
            if (tichet == null)
            {
                return NotFound();
            }

            return View(tichet);
        }

        private async Task<bool> Add_Image(IFormFile file, string file_name)
		{
            string path = Url.Content("wwwroot/TichetImages/") + file_name;
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await GlobalVariables.BlobClient.GetBlobContainerClient("tichete").UploadBlobAsync(file_name, new FileStream(path, FileMode.Open));
            return true;
        }

        // GET: Tichets/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent");
            return View();
        }

        [HttpGet]
        public IActionResult CreateTichetST()
        {
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent");
            return View("CreateTichetST");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTichetST([Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet,IdCamera,Feedback,FileName")] Tichet tichet, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                tichet.IdTichet = id_tichet;
                tichet.DataEmitere = DateTime.Now;
                tichet.DateRezolvare = null;
                tichet.StatusTichet = false;
                tichet.Detalii = Request.Form["Detalii"];
                var stid = _context.Student.Where(a => a.Email == User.Identity.Name).Select(a => a).AsEnumerable();
                tichet.IdStudent = stid.First().IdStudent;
                tichet.IdCamera = (int)stid.First().IdCamera;
                if(tichet.Detalii != null && tichet.IdCamera != -1)
				{
                    if(file != null)
					{
                        string[] user = User.Identity.Name.Split("@")[0].Split(".");
                        user[1] = user[1][0..^2];
                        string idt = id_tichet.ToString();
                        string file_name = user[1] + "_" + user[0] + "_" + idt + "." + file.FileName.Split(".")[1];
                        tichet.FileName = file_name;
                        await Add_Image(file, file_name);
                    }
                    _context.Add(tichet);
                    await _context.SaveChangesAsync();
                    id_tichet++;
				}
                return RedirectToAction("Tichete");
            }
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent", tichet.IdStudent);
            return View(tichet);
        }

        // POST: Tichets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet,IdCamera,Feedback,FileName")] Tichet tichet, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string[] user = User.Identity.Name.Split("@")[0].Split(".");
                    string idt = id_tichet.ToString();
                    string file_name = user[1] + "_" + user[0] + "_" + idt + "." + file.Name.Split(".")[1];
                    tichet.FileName = file_name;
                    await Add_Image(file, file_name);
                }
                tichet.IdTichet = id_tichet++;
                AddTichet(tichet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent", tichet.IdStudent);
            return View(tichet);
        }

        // GET: Tichets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tichet = await _context.Tichet.FindAsync(id);
            if (tichet == null)
            {
                return NotFound();
            }
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent", tichet.IdStudent);
            return View(tichet);
        }

        // POST: Tichets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet,IdCamera,Feedback")] Tichet tichet)
        {
            tichet.IdTichet = id;
            if (id != tichet.IdTichet)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tc = _context.Tichet.First(entry => entry.IdTichet == id);
                    tichet.IdCamera = tc.IdCamera;
                    _context.Entry(tc).CurrentValues.SetValues(tichet);
                    await _context.SaveChangesAsync();
                    var email_student = _context.Student.Where(a => a.IdStudent == tichet.IdStudent).Select(a => a.Email).First();
                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("florin.marut99@e-uvt.ro", "Florin");
                    var to = new EmailAddress(email_student, "Florin");
                    var subject = "Administratorul a modificat tichetul tau!";
                    var plainTextContent = "Tichetul cu ID-ul " + tichet.IdTichet + " a fost actualizat!";
                    var htmlContent = "<strong>Tichetul cu ID-ul " + tichet.IdTichet + " a fost actualizat!</strong>";
                    var msg = MailHelper.CreateSingleEmail(
                        from,
                        to,
                        subject,
                        plainTextContent,
                        htmlContent
                        );
                    await client.SendEmailAsync(msg);
                    System.Diagnostics.Debug.WriteLine("Email successfully sent!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TichetExists(tichet.IdTichet))
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
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent", tichet.IdStudent);
            return View(tichet);
        }

        // GET: Tichets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tichet = await _context.Tichet
                .Include(t => t.IdStudentNavigation)
                .FirstOrDefaultAsync(m => m.IdTichet == id);
            if (tichet == null)
            {
                return NotFound();
            }

            return View(tichet);
        }

        // POST: Tichets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tichet = await _context.Tichet.FindAsync(id);
            _context.Tichet.Remove(tichet);
            await _context.SaveChangesAsync();
            var tichets = _context.Tichet.Select(a => a).AsEnumerable();
            id_tichet = (tichets.Count() > 0 ? tichets.Last().IdTichet : 0) + 1;
            return RedirectToAction(nameof(Index));
        }

        private bool TichetExists(int id)
        {
            return _context.Tichet.Any(e => e.IdTichet == id);
        }

        [HttpPost]
        public IActionResult UpdateView(bool isChecked)
		{
            System.Diagnostics.Debug.WriteLine("Button is " + (isChecked ? "checked" : "not checked") + "\n");
            return View("Index");
		}
    }
}
