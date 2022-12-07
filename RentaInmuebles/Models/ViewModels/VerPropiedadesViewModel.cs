namespace RentaInmuebles.Models.ViewModels
{
    public class VerPropiedadesViewModel
    {
        public List<Propiedad> Propiedades { get; set; } = new();
        public List<Ciudad> Ciudades { get; set; } = new();
        public int IdCiudad { get; set; }
    }
}
