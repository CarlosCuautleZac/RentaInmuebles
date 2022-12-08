using Microsoft.AspNetCore.Mvc;
using RentaInmuebles.Models;

namespace RentaInmuebles.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class CiudadesController : Controller
    {
        private readonly sistem21_inmueblesContext context;

        public CiudadesController(sistem21_inmueblesContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var ciudades = context.Ciudad.OrderBy(x => x.Nombre);

            return View(ciudades);
        }

        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Ciudad ciudad)
        {
            if (context.Ciudad.Any(x => x.Nombre == ciudad.Nombre && x.Estado == ciudad.Estado && x.Pais == ciudad.Pais))
                ModelState.AddModelError("", "Ya existe esa ciudad");

            if (string.IsNullOrWhiteSpace(ciudad.Nombre))
                ModelState.AddModelError("", "El campo nombre no debe ir vacío. Complete el campo para agregar una ciudad");

            if (string.IsNullOrWhiteSpace(ciudad.Estado))
                ModelState.AddModelError("", "El campo estado no debe ir vacío. Complete el campo para agregar una ciudad");

            if (string.IsNullOrWhiteSpace(ciudad.Pais))
                ModelState.AddModelError("", "El país no debe ir vacío. Complete el campo para agregar una ciudad");

            if (ModelState.IsValid)
            {
                context.Add(ciudad);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ciudad);
        }

        public IActionResult Editar(int id)
        {
            var ciudad = context.Ciudad.Find(id);
            if (ciudad == null)
                return RedirectToAction("Index");

            return View(ciudad);
        }

        [HttpPost]
        public IActionResult Editar(Ciudad c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre))
                ModelState.AddModelError("", "El campo nombre no debe ir vacío. Complete el campo para agregar una ciudad");

            if (string.IsNullOrWhiteSpace(c.Estado))
                ModelState.AddModelError("", "El campo estado no debe ir vacío. Complete el campo para agregar una ciudad");

            if (string.IsNullOrWhiteSpace(c.Pais))
                ModelState.AddModelError("", "El país no debe ir vacío. Complete el campo para agregar una ciudad");

            if (context.Ciudad.Any(x => x.Nombre == c.Nombre && x.Id != c.Id && x.Estado == c.Estado && x.Pais == c.Pais))
            {
                ModelState.AddModelError("", "Ya existe esa ciudad. Escriba alguna otra para continuar");
            }

            if (ModelState.IsValid)
            {
                var ciudad = context.Ciudad.Find(c.Id);

                if (ciudad == null)
                    return RedirectToAction("Index");

                ciudad.Nombre = c.Nombre;
                ciudad.Estado = c.Estado;
                ciudad.Pais = c.Pais;

                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                return View(c);
        }



        public IActionResult Eliminar(int id)
        {
            var ciudad = context.Ciudad.Find(id);
            if (ciudad == null)
                return RedirectToAction("Index");

            return View(ciudad);
        }



        [HttpPost]
        public IActionResult Eliminar(Ciudad c)
        {
            var ciudad = context.Ciudad.Find(c.Id);
            if (ciudad == null)
            {
                ModelState.AddModelError("", "La ciudad no existe o ya ha sido eliminada");
            }

            else
            {

                if (context.Propiedad.Any(x => x.Idciudad == ciudad.Id))
                {
                    ModelState.AddModelError("", "No se puede eliminar una ciudad que tenga productos");
                }

                if (ModelState.IsValid)
                {
                    context.Remove(ciudad);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(c);


        }

    }
}
