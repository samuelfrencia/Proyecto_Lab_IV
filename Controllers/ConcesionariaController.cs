using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Lab_IV.Data;
using Proyecto_Lab_IV.Models;

namespace Proyecto_Lab_IV.Controllers
{
    public class ConcesionariaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConcesionariaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Concesionaria
        public async Task<IActionResult> Index()
        {
              return _context.concesionaria != null ? 
                          View(await _context.concesionaria.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.concesionaria'  is null.");
        }

        // GET: Concesionaria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.concesionaria == null)
            {
                return NotFound();
            }

            var concesionaria = await _context.concesionaria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concesionaria == null)
            {
                return NotFound();
            }

            return View(concesionaria);
        }

        // GET: Concesionaria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Concesionaria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,direccion,nroTel,correo")] Concesionaria concesionaria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concesionaria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(concesionaria);
        }

        // GET: Concesionaria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.concesionaria == null)
            {
                return NotFound();
            }

            var concesionaria = await _context.concesionaria.FindAsync(id);
            if (concesionaria == null)
            {
                return NotFound();
            }
            return View(concesionaria);
        }

        // POST: Concesionaria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,direccion,nroTel,correo")] Concesionaria concesionaria)
        {
            if (id != concesionaria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concesionaria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcesionariaExists(concesionaria.Id))
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
            return View(concesionaria);
        }

        // GET: Concesionaria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.concesionaria == null)
            {
                return NotFound();
            }

            var concesionaria = await _context.concesionaria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concesionaria == null)
            {
                return NotFound();
            }

            return View(concesionaria);
        }

        // POST: Concesionaria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.concesionaria == null)
            {
                return Problem("Entity set 'ApplicationDbContext.concesionaria'  is null.");
            }
            var concesionaria = await _context.concesionaria.FindAsync(id);
            if (concesionaria != null)
            {
                _context.concesionaria.Remove(concesionaria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcesionariaExists(int id)
        {
          return (_context.concesionaria?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
