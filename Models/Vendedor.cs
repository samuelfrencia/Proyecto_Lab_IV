namespace Proyecto_Lab_IV.Models
{
    public class Vendedor
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int nroTelf { get; set; }
        public string correo { get; set; }
        public int concesionariaId { get; set; }
        public Concesionaria? concesionaria { get; set; }
    }
}
