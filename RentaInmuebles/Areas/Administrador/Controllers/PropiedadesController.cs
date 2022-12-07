using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Models;
using RentaInmuebles.Models.ViewModels;

namespace RentaInmuebles.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class PropiedadesController : Controller
    {
        private readonly sistem21_inmueblesContext context;

        public PropiedadesController(sistem21_inmueblesContext context)
        {
            this.context = context;
        }

        [Route("/Administrador/Propiedades")]
        public IActionResult Index()
        {
            var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio).ToList();
            var ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList();

            if (propiedades == null)
                return RedirectToAction("Index");

            VerPropiedadesViewModel vm = new()
            {
                Ciudades = ciudades,
                Propiedades = propiedades,
                IdCiudad = 0
            };

            return View(vm);
        }



        [Route("/Administrador/Propiedades")]
        [HttpPost]
        public IActionResult Index(VerPropiedadesViewModel vm)
        {
            //var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio);

            //if (propiedades == null)
            //    return RedirectToAction("Index");

            //return View(propiedades);


            if (vm.IdCiudad == 0)//No eligieron categoria, selecciono todo
            {
                vm.Propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio).ToList();
            }
            else
            {
                vm.Propiedades = context.Propiedad.Where(x => x.Idciudad == vm.IdCiudad && x.Disponible == Dispinible.Si).Include(x => x.IdciudadNavigation).OrderBy(x => x.Nombre).ToList();
            }

            vm.Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList();

            return View(vm);
        }



    }
}
