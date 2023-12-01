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
    public class VendedorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VendedorController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Vendedor
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.vendedor.Include(v => v.concesionaria);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vendedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.vendedor == null)
            {
                return NotFound();
            }

            var vendedor = await _context.vendedor
                .Include(v => v.concesionaria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendedor == null)
            {
                return NotFound();
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre");
            return View(vendedor);
        }

        // GET: Vendedor/Create
        public IActionResult Create()
        {
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre");
            return View();
        }

        // POST: Vendedor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,apellido,nroTelf,correo,concesionariaId")] Vendedor vendedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre", vendedor.concesionariaId);
            return View(vendedor);
        }

        // GET: Vendedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.vendedor == null)
            {
                return NotFound();
            }

            var vendedor = await _context.vendedor.FindAsync(id);
            if (vendedor == null)
            {
                return NotFound();
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre", vendedor.concesionariaId);
            return View(vendedor);
        }

        // POST: Vendedor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,apellido,nroTelf,correo,concesionariaId")] Vendedor vendedor)
        {
            if (id != vendedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendedorExists(vendedor.Id))
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
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre", vendedor.concesionariaId);
            return View(vendedor);
        }

        // GET: Vendedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vendedor == null)
            {
                return NotFound();
            }

            var vendedor = await _context.vendedor
                .Include(v => v.concesionaria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(vendedor);
        }

        // POST: Vendedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vendedor == null)
            {
                return Problem("Entity set 'ApplicationDbContext.vendedor'  is null.");
            }
            var vendedor = await _context.vendedor.FindAsync(id);
            if (vendedor != null)
            {
                _context.vendedor.Remove(vendedor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendedorExists(int id)
        {
          return (_context.vendedor?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
