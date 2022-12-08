﻿using System;
using System.Collections.Generic;

public enum Dispinible
{
    Si, No
}

namespace RentaInmuebles.Models
{
    public partial class Propiedad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public decimal Precio { get; set; }
        public int? CantBaños { get; set; }
        public int? CantCochera { get; set; }
        public int? CantCuartos { get; set; }
        public int Idciudad { get; set; }
        public string? Descripcion { get; set; }
        public Dispinible Disponible { get; set; }

        public virtual Ciudad? IdciudadNavigation { get; set; }
    }
}
