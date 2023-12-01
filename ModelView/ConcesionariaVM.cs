using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_Lab_IV.Models;

namespace Proyecto_Lab_IV.ModelView
{
    public class ConcesionariaVM
    {
        public List<Concesionaria> concesionarias { get; set; }
        public Paginador paginador { get; set; }
    }
}
