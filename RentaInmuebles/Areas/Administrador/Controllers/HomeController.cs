using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RentaInmuebles.Areas.Administrador.Controllers
{
    [Authorize]
    [Area("Administrador")]
    public class HomeController : Controller
    {
        [Route("Administrador")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
