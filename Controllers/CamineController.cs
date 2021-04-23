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
    public class CamineController : Controller
    {
        private readonly DBSistemContext _context;

        public CamineController(DBSistemContext context)
        {
            _context = context;
        }

        // GET: Camine
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Camine.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string x = "")
        {
            return await Camine();
        }
        public async Task<IActionResult> Camine()
        {
            var model = _context.Camine.AsEnumerable();
            return View("Camine", model);
        }
        // GET: Camine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camine = await _context.Camine
                .FirstOrDefaultAsync(m => m.IdCamin == id);
            if (camine == null)
            {
                return NotFound();
            }

            return View(camine);
        }

        // GET: Camine/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Camine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCamin,Adresa,NrCamere,NrLocuri,Facultate,Descriere")] Camine camine)
        {
            if (ModelState.IsValid)
            {
                //camine.Images = "This should be a path";
                _context.Add(camine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camine);
        }

        // GET: Camine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camine = await _context.Camine.FindAsync(id);
            if (camine == null)
            {
                return NotFound();
            }
            return View(camine);
        }

        // POST: Camine/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCamin,Adresa,NrCamere,NrLocuri,Facultate,Descriere")] Camine camine)
        {
            if (id != camine.IdCamin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CamineExists(camine.IdCamin))
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
            return View(camine);
        }

        // GET: Camine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camine = await _context.Camine
                .FirstOrDefaultAsync(m => m.IdCamin == id);
            if (camine == null)
            {
                return NotFound();
            }

            return View(camine);
        }

        // POST: Camine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var camine = await _context.Camine.FindAsync(id);
            _context.Camine.Remove(camine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CamineExists(int id)
        {
            return _context.Camine.Any(e => e.IdCamin == id);
        }
    }
}
