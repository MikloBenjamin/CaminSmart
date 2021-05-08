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

        public async Task<IActionResult> Status()
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
        public async Task<IActionResult> Create([Bind("IdStudent,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,StatusCazare,IdCamera,DataCazare,DataDecazare")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                id_student++;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCamera"] = new SelectList(_context.Camere, "IdCamera", "IdCamera", student.IdCamera);
            return View(student);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdStudent,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,StatusCazare,IdCamera,DataCazare,DataDecazare")] Student student)
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
    }
}
