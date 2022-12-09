using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Helpers;
using RentaInmuebles.Models;
using RentaInmuebles.Models.ViewModels;
using System.Security.Claims;

namespace RentaInmuebles.Controllers
{
    public class HomeController : Controller
    {
        private readonly sistem21_inmueblesContext context;

        public HomeController(sistem21_inmueblesContext context)
        {
            this.context = context;
        }

        [Route("/")]
        [Route("/Inicio")]
        public IActionResult Index()
        {
            ViewBag.Inicio = true;

            var propiedades = context.Propiedad.Where(x => x.Disponible == Dispinible.Si).OrderByDescending(x => x.Precio).Take(3);

            return View(propiedades);
        }

        [Route("/Propiedades")]
        public IActionResult Propiedades()
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

        [Route("/Propiedades")]
        [HttpPost]
        public IActionResult Propiedades(VerPropiedadesViewModel vm)
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
                vm.Propiedades = context.Propiedad.Where(x => x.Idciudad == vm.IdCiudad&&x.Disponible==Dispinible.Si).Include(x => x.IdciudadNavigation).OrderBy(x => x.Nombre).ToList();
            }

            vm.Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList();

            return View(vm);
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

        [Route("/{id}")]
        [HttpPost]
        public IActionResult VerPropiedad(PropiedadViewModel propiedadViewModel)
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
                string mensaje = EnviarCorreo(propiedadViewModel.Contacto, propiedadViewModel.Propiedad);
                ViewBag.Message = mensaje;
                propiedadViewModel.Contacto = new();
                return View(propiedadViewModel);
            }
            else
                return View(propiedadViewModel);
        }


        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(Login login)
        {
            if (login.Username == "admin" && login.Password == "admin")
            {
                //crear claims


                var listaclaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Role,"Administrador")

                };
                //crear identidad
                var identidad = new ClaimsIdentity(listaclaims, CookieAuthenticationDefaults.AuthenticationScheme);


                //autenticar

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identidad), new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTime.Now.AddDays(1), // Cuanto tiempo estamos con la sesion activa
                        IsPersistent = true
                    });


                return RedirectToAction("Index", "Home", new { Area = "Administrador" });
            }
            else
            {
                ModelState.AddModelError("", "El nombre de usuario o la contraseña no son correctos");

                return View(login);
            }
            ;
        }



        public IActionResult CerrarSesion()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }


        private string EnviarCorreo(Contacto c, Propiedad p)
        {
            logic objLogic = new logic();

            var mensaje = $"Hola, soy {c.Nombre} y quiero que me contacten";
            var contacto = c.Email;

            string body = $@"
                            <h1>{mensaje}</h1></br>
                            
                            <p>Mi e-mail es {contacto} y mi telefono es {c.Telefono}</p>

                            <p>Estoy interesado acerca de la casa {p.Nombre} en {p.Direccion}, {p.IdciudadNavigation.Nombre} {p.IdciudadNavigation.Pais}</p>
                            <p>{c.Mensaje}</p>";


            var mesagge = objLogic.sendMail("InmobiliariaLaPonderosa@outlook.com", "Cliente Inmuebles", body);

            return mesagge;
        }
    }
}
