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
    public class CamereController : Controller
    {
        private readonly DBSistemContext _context;

        public CamereController(DBSistemContext context)
        {
            _context = context;
        }

        // GET: Cameres
        public async Task<IActionResult> Index()
        {
            var dBSistemContext = _context.Camere.Include(c => c.IdCaminNavigation);
            return View(await dBSistemContext.ToListAsync());
        }


        public async Task<IActionResult> CamereCamin(int id)
        {
            var camere = _context.Camere.Where(c => c.IdCamin == id).Select(a => a).AsEnumerable();
            return View("CamereCamin", camere);
        }

        // GET: Cameres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camere = await _context.Camere
                .Include(c => c.IdCaminNavigation)
                .FirstOrDefaultAsync(m => m.IdCamera == id);
            if (camere == null)
            {
                return NotFound();
            }

            return View(camere);
        }

        // GET: Cameres/Create
        public IActionResult Create()
        {
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "IdCamin");
            return View();
        }

        // POST: Cameres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCamera,IdCamin,LimitaNrStudenti,NrStudentiCazati,Descriere,TipCamera")] Camere camere)
        {
            if (ModelState.IsValid)
            {
                _context.Add(camere);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "IdCamin", camere.IdCamin);
            return View(camere);
        }

        // GET: Cameres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camere = await _context.Camere.FindAsync(id);
            if (camere == null)
            {
                return NotFound();
            }
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "IdCamin", camere.IdCamin);
            return View(camere);
        }

        // POST: Cameres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCamera,IdCamin,LimitaNrStudenti,NrStudentiCazati,Descriere,TipCamera")] Camere camere)
        {
            if (id != camere.IdCamera)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camere);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CamereExists(camere.IdCamera))
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
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "IdCamin", camere.IdCamin);
            return View(camere);
        }

        // GET: Cameres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camere = await _context.Camere
                .Include(c => c.IdCaminNavigation)
                .FirstOrDefaultAsync(m => m.IdCamera == id);
            if (camere == null)
            {
                return NotFound();
            }

            return View(camere);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var camere = await _context.Camere.FindAsync(id);
            _context.Camere.Remove(camere);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CamereExists(int id)
        {
            return _context.Camere.Any(e => e.IdCamera == id);
        }
    }
}
