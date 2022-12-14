using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Areas.Administrador.Models.ViewModels;
using RentaInmuebles.Models;
using RentaInmuebles.Models.ViewModels;

namespace RentaInmuebles.Areas.Administrador.Controllers
{
    [Authorize]
    [Area("Administrador")]
    public class PropiedadesController : Controller
    {
        private readonly sistem21_inmueblesContext context;
        private readonly IWebHostEnvironment env;

        public PropiedadesController(sistem21_inmueblesContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [Route("/Administrador/Propiedades")]
        public IActionResult Index()
        {
            var propiedades = context.Propiedad.OrderByDescending(x => x.Precio).ToList();
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
                vm.Propiedades = context.Propiedad.OrderByDescending(x => x.Precio).ToList();
            }
            else
            {
                vm.Propiedades = context.Propiedad.Where(x => x.Idciudad == vm.IdCiudad).Include(x => x.IdciudadNavigation).OrderBy(x => x.Nombre).ToList();
            }

            vm.Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList();

            return View(vm);
        }

        public IActionResult Agregar()
        {
            AgregarPropiedadViewModel vm = new()
            {
                Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Agregar(AgregarPropiedadViewModel vm)
        {
            var propiedad = vm.Propiedad;
            var imagen = vm.Imagen;



            if (propiedad == null)
                ModelState.AddModelError("", "Por favor escriba los datos correspondientes para continuar");

            if (propiedad.Nombre != null)
                if (propiedad.Nombre.Length > 100)
                ModelState.AddModelError("", "El campo nombre no debe contener más de 100 caracteres");

            if (propiedad.Direccion.Length > 45)
                ModelState.AddModelError("", "El campo dirección no debe contener más de 100 caracteres");

            if (propiedad.Descripcion != null)
                if (propiedad.Descripcion.Length > 65000)
                    ModelState.AddModelError("", "El campo descripción no debe contener más de 65,000 caracteres");

            if (string.IsNullOrWhiteSpace(propiedad.Nombre))
                ModelState.AddModelError("", "Por favor escriba el nombre para agregar una propiedad");

            if (string.IsNullOrWhiteSpace(propiedad.Direccion))
                ModelState.AddModelError("", "Por favor escriba la direccion para agregar una propiedad");

            if (propiedad.Precio <= 0)
                ModelState.AddModelError("", "Monto invalido. Para agregar una propiedad debe escribir una monto mayor a $0.00");

            if (propiedad.CantCuartos <= 0)
                ModelState.AddModelError("", "NÚmero de cuartos invalido. Para agregar una propiedad debe escribir un número de cuartos mayor a 0");

            if (propiedad.Idciudad == 0)
                ModelState.AddModelError("", "Ciudad invalida. Debe seleccionar la ciudad de la propiedad");

            if (context.Propiedad.Any(x => x.Direccion == propiedad.Direccion && x.Idciudad == propiedad.Idciudad))
                ModelState.AddModelError("", "Ya existe una propiedad con esa dirección.");

            if (context.Propiedad.Any(x => x.Nombre == propiedad.Nombre))
                ModelState.AddModelError("", "Ya existe una propiedad con ese nombre.");

            if (ModelState.IsValid)
            {
                Propiedad p = new();
                p = propiedad;
                context.Add(p);
                int cambios = context.SaveChanges();

                if (cambios > 0)
                {
                    if (vm.Imagen == null)
                    {
                        string nodisp = env.WebRootPath + "/img/no-disponible.jpg";
                        string nuevaruta = env.WebRootPath + $"/img/{p.Id}.jpg";

                        System.IO.File.Copy(nodisp, nuevaruta);
                    }
                    else
                    {
                        string nuevaruta = env.WebRootPath + $"/img/{p.Id}.jpg";
                        var archivo = System.IO.File.Create(nuevaruta);
                        vm.Imagen.CopyTo(archivo);
                        archivo.Close();
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                vm.Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList();
                return View(vm);
            }

        }


        public IActionResult Editar(int id)
        {
            var p = context.Propiedad.Find(id);

            if (p == null)
                RedirectToAction("Index");

            AgregarPropiedadViewModel vm = new()
            {
                Ciudades = context.Ciudad.OrderBy(x => x.Nombre).ToList(),
                Propiedad = p

            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Editar(AgregarPropiedadViewModel vm)
        {
            var propiedad = vm.Propiedad;
            var imagen = vm.Imagen;

            if (vm.Propiedad == null)
                return RedirectToAction("Index");

            var p = context.Propiedad.Find(propiedad.Id);

            if (p == null)
                return RedirectToAction("Index");

            if (propiedad == null)
                ModelState.AddModelError("", "Por favor escriba los datos correspondientes para continuar");

            if (string.IsNullOrWhiteSpace(propiedad.Nombre))
                ModelState.AddModelError("", "Por favor escriba el nombre para agregar una propiedad");

            if (string.IsNullOrWhiteSpace(propiedad.Direccion))
                ModelState.AddModelError("", "Por favor escriba la direccion para agregar una propiedad");

            if (propiedad.Precio <= 0)
                ModelState.AddModelError("", "Monto invalido. Para agregar una propiedad debe escribir una monto mayor a $0.00");

            if (propiedad.CantCuartos <= 0)
                ModelState.AddModelError("", "NÚmero de cuartos invalido. Para agregar una propiedad debe escribir un número de cuartos mayor a 0");

            if (propiedad.Idciudad == 0)
                ModelState.AddModelError("", "Ciudad invalida. Debe seleccionar la ciudad de la propiedad");

            if (context.Propiedad.Any(x => x.Direccion == propiedad.Direccion && x.Idciudad == propiedad.Idciudad && x.Id != p.Id))
                ModelState.AddModelError("", "Ya existe una propiedad con esa dirección.");

            if (ModelState.IsValid)
            {

                p.Nombre = propiedad.Nombre;
                p.Direccion = propiedad.Direccion;
                p.Precio = propiedad.Precio;
                p.CantBaños = propiedad.CantBaños;
                p.CantCochera = propiedad.CantCochera;
                p.CantCuartos = propiedad.CantCuartos;
                p.Idciudad = propiedad.Idciudad;
                p.Descripcion = propiedad.Descripcion;
                p.Disponible = propiedad.Disponible;
                context.SaveChanges();

                if (vm.Imagen != null)
                {
                    string nuevaruta = env.WebRootPath + $"/img/{p.Id}.jpg";
                    var archivo = System.IO.File.Create(nuevaruta);
                    vm.Imagen.CopyTo(archivo);
                    archivo.Close();
                }
                return RedirectToAction("Index");

            }
            else
            {
                vm.Ciudades = context.Ciudad.OrderBy(x => x.Nombre);
                return View(vm);
            }
        }


        public IActionResult Eliminar(int id)
        {
            var propiedad = context.Propiedad.Find(id);

            if (propiedad == null)
                return RedirectToAction("Index");



            return View(propiedad);
        }

        [HttpPost]
        public IActionResult Eliminar(Propiedad p)
        {
            var propiedad = context.Propiedad.FirstOrDefault(x => x.Id == p.Id);

            if (propiedad == null)
                ModelState.AddModelError("", "La propiedad no se encuentra o ya ha sido eliminada");
            else
            {
                context.Remove(propiedad);
                string nuevaruta = env.WebRootPath + $"/img/{p.Id}.jpg";

                if (context.SaveChanges() > 0)
                {
                    System.IO.File.Delete(nuevaruta);
                }

                return RedirectToAction("Index");
            }

            return View(propiedad);
        }

    }
}
