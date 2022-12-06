using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Helpers;
using RentaInmuebles.Models;
using RentaInmuebles.Models.ViewModels;

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

            var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio).Take(3);

            return View(propiedades);
        }


        public IActionResult Propiedades()
        {
            var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio);

            if (propiedades == null)
                return RedirectToAction("Index");

            return View(propiedades);
        }

        [Route("/{id}")]
        public IActionResult VerPropiedad(string id)
        {
            var lugar = id.Replace("-", " ");
            var propiedad = context.Propiedad.Where(x => x.Nombre == lugar).Include(x => x.IdciudadNavigation).FirstOrDefault();

            if (propiedad == null)
                return RedirectToAction("Index");

            if (propiedad.Disponible == Dispinible.No)
                return RedirectToAction("Index");

            PropiedadViewModel propiedadViewModel = new()
            {
                Propiedad = propiedad,
                Contacto = new Contacto()
            };


            return View(propiedadViewModel);
        }


        [HttpPost]
        public IActionResult VerPropiedad( PropiedadViewModel propiedadViewModel)
        {

            var propiedad = context.Propiedad.Include(x => x.IdciudadNavigation).
                Where(x => x.Id == propiedadViewModel.Propiedad.Id).FirstOrDefault();

            propiedadViewModel.Propiedad = propiedad;

            if (propiedad == null)
                return RedirectToAction("Index");
            if (propiedad.Disponible == Dispinible.No)
                ModelState.AddModelError("", "La casa ya no se encuentra disponible para su renta. Eliga otra casa para contactarnos con usted");

            if (string.IsNullOrWhiteSpace(propiedadViewModel.Contacto.Nombre))
            {
                ModelState.AddModelError("", "El nombre no debe ir vacio. Escriba su nombre para ponernos en contacto con usted.");
            }

            if (string.IsNullOrWhiteSpace(propiedadViewModel.Contacto.Email) && string.IsNullOrWhiteSpace(propiedadViewModel.Contacto.Telefono))
            {
                ModelState.AddModelError("", "Debe escribir por lo menos un metodo de contacto, ya sea telefono o correo electronico");
            }

            if (ModelState.IsValid)
            {
                //Falta validar
                string mensaje = EnviarCorreo(propiedadViewModel.Contacto);
                ViewBag.Message = mensaje;
                propiedadViewModel.Contacto = new();
                return View(propiedadViewModel);
            }
            else
                return View(propiedadViewModel);
        }





        private string EnviarCorreo(Contacto c)
        {
            logic objLogic = new logic();

            var mensaje = $"Hola, soy {c.Nombre} y quiero que me contacten";
            var contacto = c.Email;

            string body = $@"
                            <h1>{mensaje}</h1></br>
                            
                            <p>Mi e-mail es {contacto} y mi telefono es {c.Telefono}</p>

                            <p>{c.Mensaje}</p>";


            var mesagge = objLogic.sendMail("InmobiliariaLaPonderosa@outlook.com", "Cliente Inmuebles", body);

            return mesagge;
        }
    }
}
