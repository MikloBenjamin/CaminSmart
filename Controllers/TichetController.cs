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
            var dBSistemContext = _context.Tichet.Include(t => t.IdStudentNavigation);
            System.Diagnostics.Debug.WriteLine(dBSistemContext);
            return View(await dBSistemContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string x = "")
        {
            return await Tichete();
        }
        public async Task<IActionResult> Tichete()
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
            return RedirectToAction("Inscriere", "Student");
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
        public async Task<IActionResult> CreateTichetST([Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet,IdCamera,Feedback")] Tichet tichet)
        {
            if (ModelState.IsValid)
            {
                tichet.IdTichet = id_tichet++;
                tichet.IdStudent = stud_id;
                tichet.DataEmitere = DateTime.Now;
                tichet.DateRezolvare = null;
                tichet.StatusTichet = false;
                tichet.Detalii = Request.Form["Detalii"];
                tichet.TipTichet = (Request.Form["TipTichet"].Count() != 0);
                var stid = _context.Student.Where(a => a.Email == User.Identity.Name).Select(a => a.IdStudent).AsEnumerable();
                if (stid.Count() > 0)
                {
                    var x = _context.Tichet.Where(a => a.IdStudent == stid.First()).Select(a => a.IdCamera).AsEnumerable();
                    tichet.IdCamera = x.First();
                }
                else
				{
                    tichet.IdCamera = -1;
				}
                if(tichet.Detalii != null && tichet.IdCamera != -1)
				{
                    AddTichet(tichet);
                    await _context.SaveChangesAsync();
				}
                return await Tichete();
            }
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent", tichet.IdStudent);
            return View(tichet);
        }

        // POST: Tichets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet,IdCamera,Feedback")] Tichet tichet)
        {
            if (ModelState.IsValid)
            {
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
            if (id != tichet.IdTichet)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tichet);
                    await _context.SaveChangesAsync();
                    //Aici trimitem Email la student
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
    }
}
