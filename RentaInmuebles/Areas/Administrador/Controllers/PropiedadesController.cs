using Microsoft.AspNetCore.Mvc;

namespace RentaInmuebles.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class PropiedadesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
