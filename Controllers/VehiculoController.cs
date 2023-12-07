using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Proyecto_Lab_IV.Data;
using Proyecto_Lab_IV.Models;
using Proyecto_Lab_IV.ModelView;

namespace Proyecto_Lab_IV.Controllers
{
    [Authorize]
    public class VehiculoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VehiculoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> ImportarVehiculos(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Mensaje = "Error: No se ha proporcionado un archivo de Excel";
                return View();
            }

            try
            {
                using (var package = new ExcelPackage(archivo.OpenReadStream()))
                {

                        var worksheet = package.Workbook.Worksheets[0];

                        var vehiculos = new List<Vehiculo>();

                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var patente = worksheet.Cells[row, 1].Value?.ToString();
                        var marcaIdValue = worksheet.Cells[row, 2].Value;

                        if (!string.IsNullOrEmpty(patente) && marcaIdValue != null && int.TryParse(marcaIdValue.ToString(), out int marcaId))
                        {
                            var vehiculo = new Vehiculo
                            {
                                patente = patente,
                                marcaId = marcaId,
                                modelo = worksheet.Cells[row, 3].Value?.ToString(),
                                anio = Convert.ToInt32(worksheet.Cells[row, 4].Value),
                                precio = Convert.ToInt32(worksheet.Cells[row, 5].Value),
                                tipoVehiculoId = Convert.ToInt32(worksheet.Cells[row, 6].Value),
                                fotografia = worksheet.Cells[row, 7].Value?.ToString(),
                                color = worksheet.Cells[row, 8].Value?.ToString(),
                                estado = worksheet.Cells[row, 9].Value?.ToString(),
                                concesionariaId = Convert.ToInt32(worksheet.Cells[row, 10].Value)
                            };

                            vehiculos.Add(vehiculo);
                        }
                        else
                        {
                            // Manejar el caso en el que una celda es null o no tiene el formato esperado
                            Console.WriteLine($"Error en la fila {row}: La patente o marcaId no tiene el formato esperado.");
                        }
                    }
                    _context.vehiculo.AddRange(vehiculos);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: La importación ha fallado. Verifica el formato del archivo o consulta los registros del servidor para obtener más detalles.";
                Console.WriteLine("Error en la importación: " + ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Excepción interna: " + ex.InnerException.Message);
                }
            }

            var applicationDbContext = _context.vehiculo
                .Include(m => m.marca)
                .Include(t => t.tipoVehiculo)
                .Include(c => c.concesionaria);
            return RedirectToAction("Index", await applicationDbContext.ToListAsync());
        }
        [AllowAnonymous]
        // GET: Vehiculo
        public async Task<IActionResult> Index(int? busqMarcaId, int? busqTipoVehiculoId, int? busqConcesionariaId, int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.vehiculo
                .Include(m => m.marca)
                .Include(t => t.tipoVehiculo)
                .Include(c => c.concesionaria)
                .Select(e => e);

            if (busqMarcaId != null && busqMarcaId > 0)
            {
                applicationDbContext = applicationDbContext.Where(e => e.marcaId.Equals(busqMarcaId));
                paginas.ValoresQueryString.Add("marcaId", busqMarcaId.ToString());
            }
            if (busqTipoVehiculoId != null && busqTipoVehiculoId > 0)
            {
                applicationDbContext = applicationDbContext.Where(e => e.tipoVehiculoId.Equals(busqTipoVehiculoId));
                paginas.ValoresQueryString.Add("tipoVehiculoId", busqTipoVehiculoId.ToString());
            }
            if (busqConcesionariaId != null && busqConcesionariaId > 0)
            {
                applicationDbContext = applicationDbContext.Where(e => e.concesionariaId.Equals(busqConcesionariaId));
                paginas.ValoresQueryString.Add("concesionariaId", busqConcesionariaId.ToString());
            }

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
                        .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                        .Take(paginas.RegistrosPorPagina);


            VehiculoVM datos = new VehiculoVM()
            {
                vehiculos = registrosMostrar.ToList(),
                busqMarcaId = busqMarcaId,
                busqTipoVehiculoId = busqTipoVehiculoId,
                busqConcesionariaId = busqConcesionariaId,
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
