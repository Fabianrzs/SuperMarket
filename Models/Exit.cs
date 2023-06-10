using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket.Models
{
    public class VentasUltimosDias
    {
        public long CantidadVentas { get; set; }
        public long MontoTotal { get; set; }
    }
    public class LocalProductoVentas
    {
        public string NombreLocal { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadVentas { get; set; }
    }
}
