using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyecto_Lab_IV.Models;

namespace Proyecto_Lab_IV.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehiculo> vehiculo { get; set; }
        public DbSet<Concesionaria> concesionaria { get; set; }
        public DbSet<Marca> marca { get; set; }
        public DbSet<TipoVehiculo> tipoVehiculo { get; set; }
        public DbSet<Vendedor> vendedor { get; set; }
    }
}