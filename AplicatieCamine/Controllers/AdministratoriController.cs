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
    public class AdministratoriController : Controller
    {
        private readonly DBSistemContext _context;

        public AdministratoriController(DBSistemContext context)
        {
            _context = context;
        }

        // GET: Administratori
        public async Task<IActionResult> Index()
        {
            var dBSistemContext = _context.Administratori.Include(a => a.IdCaminNavigation);
            return View(await dBSistemContext.ToListAsync());
        }

        // GET: Administratori/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratori = await _context.Administratori
                .Include(a => a.IdCaminNavigation)
                .FirstOrDefaultAsync(m => m.IdAdmin == id);
            if (administratori == null)
            {
                return NotFound();
            }

            return View(administratori);
        }

        // GET: Administratori/Create
        public IActionResult Create()
        {
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "Adresa");
            return View();
        }

        // POST: Administratori/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAdmin,IdCamin,Nume,Adresa,NrTelefon,Email,TipAdmin")] Administratori administratori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(administratori);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "Adresa", administratori.IdCamin);
            return View(administratori);
        }

        // GET: Administratori/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratori = await _context.Administratori.FindAsync(id);
            if (administratori == null)
            {
                return NotFound();
            }
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "Adresa", administratori.IdCamin);
            return View(administratori);
        }

        // POST: Administratori/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAdmin,IdCamin,Nume,Adresa,NrTelefon,Email,TipAdmin")] Administratori administratori)
        {
            if (id != administratori.IdAdmin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administratori);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministratoriExists(administratori.IdAdmin))
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
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "Adresa", administratori.IdCamin);
            return View(administratori);
        }

        // GET: Administratori/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratori = await _context.Administratori
                .Include(a => a.IdCaminNavigation)
                .FirstOrDefaultAsync(m => m.IdAdmin == id);
            if (administratori == null)
            {
                return NotFound();
            }

            return View(administratori);
        }

        // POST: Administratori/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administratori = await _context.Administratori.FindAsync(id);
            _context.Administratori.Remove(administratori);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratoriExists(int id)
        {
            return _context.Administratori.Any(e => e.IdAdmin == id);
        }
    }
}
