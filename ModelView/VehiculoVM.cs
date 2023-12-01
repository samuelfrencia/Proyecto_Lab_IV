using Proyecto_Lab_IV.Models;
using Proyecto_Lab_IV.ModelView;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_Lab_IV.ModelView
{
    public class VehiculoVM
    {
        public List<Vehiculo> vehiculos { get; set; }
        public SelectList ListMarca { get; set; }
        public SelectList ListTipoVehiculo { get; set; }
        public SelectList ListConcesionaria { get; set; }
        public int? busqMarcaId { get; set; }
        public int? busqTipoVehiculoId { get; set; }
        public int? busqConcesionariaId { get; set; }

        public Paginador paginador { get; set; }
    }
}
