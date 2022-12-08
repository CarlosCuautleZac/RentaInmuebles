using RentaInmuebles.Models;

namespace RentaInmuebles.Areas.Administrador.Models.ViewModels
{
    public class AgregarPropiedadViewModel
    {
        public Propiedad? Propiedad { get; set; } = new();
        public IEnumerable<Ciudad>? Ciudades { get; set; }
        public IFormFile? Imagen { get; set; }
    }
}
