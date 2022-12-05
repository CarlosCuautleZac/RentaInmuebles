using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Models;

namespace RentaInmuebles.Controllers
{
    public class HomeController : Controller
    {
        private readonly sistem21_inmueblesContext context;

        public HomeController(sistem21_inmueblesContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Inicio = true;

            var propiedades = context.Propiedad.Where(x=>x.Disponible==Dispinible.Si).OrderByDescending(x=>x.Precio).Take(3);
            
            return View(propiedades);
        }


        public IActionResult Propiedades()
        {
            var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio);
            
            if(propiedades==null)
                return RedirectToAction("Index");

            return View(propiedades);
        }

        [Route("/{id}")]
        public IActionResult VerPropiedad(string id)
        {
            var lugar = id.Replace("-", " ");
            var propiedad = context.Propiedad.Where(x => x.Nombre == lugar).Include(x=>x.IdciudadNavigation).FirstOrDefault();

            if (propiedad == null)
                return RedirectToAction("Index");

            return View(propiedad);
        }
    }
}
