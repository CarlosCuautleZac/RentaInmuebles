using System;
using System.Collections.Generic;

namespace RentaInmuebles.Models
{
    public partial class Ciudad
    {
        public Ciudad()
        {
            Propiedad = new HashSet<Propiedad>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string Pais { get; set; } = null!;

        public virtual ICollection<Propiedad> Propiedad { get; set; }
    }
}
