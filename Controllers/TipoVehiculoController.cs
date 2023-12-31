﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Lab_IV.Data;
using Proyecto_Lab_IV.Models;
using Proyecto_Lab_IV.ModelView;

namespace Proyecto_Lab_IV.Controllers
{
    [Authorize]
    public class TipoVehiculoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoVehiculoController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        // GET: TipoVehiculo
        public async Task<IActionResult> Index(int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.tipoVehiculo;

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
            .Skip((pagina - 1) * paginas.RegistrosPorPagina)
            .Take(paginas.RegistrosPorPagina);

            TipoVehiculoVM datos = new TipoVehiculoVM()
            {
                tipoVehiculos = registrosMostrar.ToList(),
                paginador = paginas
            };

            return View(datos);
        }

        // GET: TipoVehiculo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tipoVehiculo == null)
            {
                return NotFound();
            }

            var tipoVehiculo = await _context.tipoVehiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoVehiculo == null)
            {
                return NotFound();
            }

            return View(tipoVehiculo);
        }

        // GET: TipoVehiculo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoVehiculo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre")] TipoVehiculo tipoVehiculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoVehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoVehiculo);
        }

        // GET: TipoVehiculo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tipoVehiculo == null)
            {
                return NotFound();
            }

            var tipoVehiculo = await _context.tipoVehiculo.FindAsync(id);
            if (tipoVehiculo == null)
            {
                return NotFound();
            }
            return View(tipoVehiculo);
        }

        // POST: TipoVehiculo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre")] TipoVehiculo tipoVehiculo)
        {
            if (id != tipoVehiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoVehiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoVehiculoExists(tipoVehiculo.Id))
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
            return View(tipoVehiculo);
        }

        // GET: TipoVehiculo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tipoVehiculo == null)
            {
                return NotFound();
            }

            var tipoVehiculo = await _context.tipoVehiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoVehiculo == null)
            {
                return NotFound();
            }

            return View(tipoVehiculo);
        }

        // POST: TipoVehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tipoVehiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.tipoVehiculo'  is null.");
            }
            var tipoVehiculo = await _context.tipoVehiculo.FindAsync(id);
            if (tipoVehiculo != null)
            {
                _context.tipoVehiculo.Remove(tipoVehiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoVehiculoExists(int id)
        {
          return (_context.tipoVehiculo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
