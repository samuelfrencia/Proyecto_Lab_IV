using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Lab_IV.Data;
using Proyecto_Lab_IV.Models;
using Proyecto_Lab_IV.ModelView;

namespace Proyecto_Lab_IV.Controllers
{
    public class VehiculoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VehiculoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Vehiculo
        public async Task<IActionResult> Index(string? busqMarca, string? busqModelo, string? busqConcesionaria, int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.vehiculo.Include(m => m.marca).Include(t => t.tipoVehiculo).Include(c => c.concesionaria);
            #region Busqueda de datos
            //if (!string.IsNullOrEmpty(busqNombre))
            //{
            //    applicationDbContext = applicationDbContext.Where(e => e.nombre.Contains(busqNombre));
            //    paginas.ValoresQueryString.Add("busqNombre", busqNombre);
            //}

            //if (busqLegajo != null && busqLegajo > 0)
            //{
            //    applicationDbContext = applicationDbContext.Where(e => e.legajo.Equals(busqLegajo));
            //    paginas.ValoresQueryString.Add("busqLegajo", busqLegajo.ToString());
            //}

            //if (carreraId != null && carreraId > 0)
            //{
            //    applicationDbContext = applicationDbContext.Where(e => e.CarreraId.Equals(carreraId));
            //    paginas.ValoresQueryString.Add("carreraId", carreraId.ToString());
            //}
            #endregion 
            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
                        .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                        .Take(paginas.RegistrosPorPagina);


            VehiculoVM datos = new VehiculoVM()
            {
                vehiculos = registrosMostrar.ToList(),
                ListMarca = new SelectList(_context.marca, "Id", "nombre"),
                ListTipoVehiculo = new SelectList(_context.tipoVehiculo, "Id", "nombre"),
                ListConcesionaria = new SelectList(_context.concesionaria, "Id", "nombre"),
                paginador = paginas
            };

            return View(datos);
        }

        // GET: Vehiculo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.vehiculo == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.vehiculo
                .Include(v => v.concesionaria)
                .Include(v => v.marca)
                .Include(v => v.tipoVehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre");
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "nombre");
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "nombre");
            return View(vehiculo);
        }

        // GET: Vehiculo/Create
        public IActionResult Create()
        {
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre");
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "nombre");
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "nombre");
            return View();
        }

        // POST: Vehiculo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,patente,marcaId,modelo,anio,precio,tipoVehiculoId,fotografia,color,estado,concesionariaId")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                vehiculo.fotografia = cargarFoto("");
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "Id", vehiculo.concesionariaId);
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "Id", vehiculo.marcaId);
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "Id", vehiculo.tipoVehiculoId);
            return View(vehiculo);
        }

        // GET: Vehiculo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.vehiculo == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.vehiculo.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre", vehiculo.concesionariaId);
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "nombre", vehiculo.marcaId);
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "nombre", vehiculo.tipoVehiculoId);
            return View(vehiculo);
        }

        // POST: Vehiculo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,patente,marcaId,modelo,anio,precio,tipoVehiculoId,fotografia,color,estado,concesionariaId")] Vehiculo vehiculo)
        {
            if (id != vehiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string nuevaFoto = cargarFoto(string.IsNullOrEmpty(vehiculo.fotografia) ? "" : vehiculo.fotografia);

                    if (!string.IsNullOrEmpty(nuevaFoto))
                    {
                        vehiculo.fotografia = nuevaFoto;
                    }
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.Id))
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
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre", vehiculo.concesionariaId);
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "nombre", vehiculo.marcaId);
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "nombre", vehiculo.tipoVehiculoId);
            return View(vehiculo);
        }

        // GET: Vehiculo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vehiculo == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.vehiculo
                .Include(v => v.concesionaria)
                .Include(v => v.marca)
                .Include(v => v.tipoVehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            ViewData["concesionariaId"] = new SelectList(_context.concesionaria, "Id", "nombre");
            ViewData["marcaId"] = new SelectList(_context.marca, "Id", "nombre");
            ViewData["tipoVehiculoId"] = new SelectList(_context.tipoVehiculo, "Id", "nombre");
            return View(vehiculo);
        }

        // POST: Vehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vehiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.vehiculo'  is null.");
            }
            var vehiculo = await _context.vehiculo.FindAsync(id);
            if (vehiculo != null)
            {
                _context.vehiculo.Remove(vehiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(int id)
        {
          return (_context.vehiculo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private string cargarFoto(string fotoAnterior)
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoFoto = archivos[0];
                if (archivoFoto.Length > 0)
                {
                    var pathDestino = Path.Combine(_env.WebRootPath, "fotos");

                    fotoAnterior = Path.Combine(pathDestino, fotoAnterior);
                    if (System.IO.File.Exists(fotoAnterior))
                        System.IO.File.Delete(fotoAnterior);

                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                    archivoDestino += Path.GetExtension(archivoFoto.FileName);

                    using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                    {
                        archivoFoto.CopyTo(filestream);
                        return archivoDestino;
                    };
                }
            }
            return "";
        }
    }
}
