using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AplicatieCamine.Models;

namespace AplicatieCamine
{
    public class TichetController : Controller
    {
        private readonly DBSistemContext _context;

        public TichetController(DBSistemContext context)
        {
            _context = context;
        }

        // GET: Tichets
        public async Task<IActionResult> Index()
        {
            var dBSistemContext = _context.Tichet.Include(t => t.IdStudentNavigation);
            return View(await dBSistemContext.ToListAsync());
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

        // GET: Tichets/Create
        public IActionResult Create()
        {
            ViewData["IdStudent"] = new SelectList(_context.Student, "IdStudent", "IdStudent");
            return View();
        }

        // POST: Tichets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet")] Tichet tichet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tichet);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdTichet,IdStudent,DataEmitere,DateRezolvare,StatusTichet,Detalii,TipTichet")] Tichet tichet)
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
            return RedirectToAction(nameof(Index));
        }

        private bool TichetExists(int id)
        {
            return _context.Tichet.Any(e => e.IdTichet == id);
        }
    }
}
