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

        public async Task<IActionResult> Index()
        {
            var dBSistemContext = _context.Camere.Include(c => c.IdCaminNavigation);
            return View(await dBSistemContext.ToListAsync());
        }

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
        public IActionResult Create()
        {
            ViewData["IdCamin"] = new SelectList(_context.Camine, "IdCamin", "IdCamin");
            return View();
        }

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
