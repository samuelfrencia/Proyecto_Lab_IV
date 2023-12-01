namespace Proyecto_Lab_IV.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string patente { get; set; }
        public int marcaId { get; set; }
        public Marca? marca { get; set; }
        public string modelo { get; set; }
        public int anio { get; set; }
        public int precio { get; set; }
        public int tipoVehiculoId { get; set; }
        public TipoVehiculo? tipoVehiculo { get; set; }
        public string? fotografia { get; set; }
        public string color { get; set; }
        public string estado { get; set; }
        public int concesionariaId { get; set; }
        public Concesionaria? concesionaria { get; set; }
    }
}
