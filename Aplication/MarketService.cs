using Microsoft.EntityFrameworkCore;
using SuperMarket.Infrastructure;
using SuperMarket.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace SuperMarket.Aplication
{
    public class MarketService
    {
        private readonly SuperMarketContext _context;

        public MarketService(SuperMarketContext context)
        {
            _context = context;
        }

        public async Task<EntityResponse<VentasUltimosDias>> ObtenerTotalVentasUltimosDiasAsync(int? dias = 1)
        {
            try
            {
                DateTime fechaLimite = DateTime.Now.AddDays((double)-dias);

                var ventas = _context.Venta.Where(v => v.Fecha >= fechaLimite);

                var objeto = new VentasUltimosDias
                {
                    CantidadVentas = await ventas.CountAsync(),
                    MontoTotal = await ventas.SumAsync(v => v.Total)
                };

                return new EntityResponse<VentasUltimosDias>(objeto);
            }
            catch (Exception e)
            {
                return new EntityResponse<VentasUltimosDias>($"Error al obtener el total de ventas: {e.Message}", true);
            }
        }

        public async Task<EntityResponse<Ventum>> ObtenerVentaConMontoMasAltoAsync()
        {
            try
            {
                var venta = await _context.Venta
                    .OrderByDescending(v => v.Total)
                    .FirstOrDefaultAsync();

                return new EntityResponse<Ventum>(venta);
            }
            catch (Exception e)
            {
                return new EntityResponse<Ventum>($"Error al obtener la venta con el monto más alto: {e.Message}", true);
            }
        }

        public async Task<EntityResponse<Producto>> ObtenerProductoConMayorMontoVentasAsync()
        {
            try
            {
                var producto = await _context.Productos
                    .OrderByDescending(p => p.VentaDetalles.Sum(vd => vd.PrecioUnitario * vd.Cantidad)).Include(x => x.VentaDetalles)
                    .FirstOrDefaultAsync();

                return new EntityResponse<Producto>(producto);
            }
            catch (Exception e)
            {
                return new EntityResponse<Producto>($"Error al obtener el producto con mayor monto de ventas: {e.Message}", true);
            }
        }

        public async Task<EntityResponse<Local>> ObtenerLocalConMayorMontoVentasAsync()
        {
            try
            {
                var local = await _context.Locals.Include(x=>x.Venta)
                    .OrderByDescending(l => l.Venta.Sum(v => v.Total))
                    .FirstOrDefaultAsync();

                return new EntityResponse<Local>(local);
            }
            catch (Exception e)
            {
                return new EntityResponse<Local>($"Error al obtener el local con mayor monto de ventas: {e.Message}", true);
            }
        }

        public async Task<EntityResponse<Marca>> ObtenerMarcaConMayorMargenGananciasAsync()
        {
            try
            {
                var marcas = await _context.Marcas
                    .Include(x => x.Productos)
                        .ThenInclude(p => p.VentaDetalles)
                    .ToListAsync();
                var marca = marcas.OrderByDescending
                    (m => GetMargenGanancias(m))
                    .FirstOrDefault();

                return new EntityResponse<Marca>(marca);
            }
            catch (Exception e)
            {
                return new EntityResponse<Marca>($"Error al obtener la marca con mayor margen de ganancias: {e.Message}", true);
            }
        }

        private decimal GetMargenGanancias(Marca marca)
        {
            var productos = marca.Productos.ToList();

            decimal margenGanancias = 0;

            foreach (var producto in productos)
            {
                var ventas = producto.VentaDetalles.Sum(vd => vd.PrecioUnitario * vd.Cantidad);
                var costos = producto.CostoUnitario * producto.VentaDetalles.Sum(vd => vd.Cantidad);

                margenGanancias += ventas - costos;
            }

            return margenGanancias;
        }

        public async Task<EntityResponse<LocalProductoVentas>> ObtenerProductoMasVendidoPorLocalAsync()
        {
            try
            {
              
                List<LocalProductoVentas> list = new List<LocalProductoVentas>();

                var query = (from local in _context.Locals
                             join venta in _context.Venta on local.IdLocal equals venta.IdLocal
                             join ventaDetalle in _context.VentaDetalles on venta.IdVenta equals ventaDetalle.IdVenta
                             join producto in _context.Productos on ventaDetalle.IdProducto equals producto.IdProducto
                             group new { local, producto, ventaDetalle } by new { local.IdLocal, local.Nombre } into g
                             select new
                             {
                                 NombreLocal = g.Key.Nombre,
                                 CantidadVentas = g.Sum(x => x.ventaDetalle.Cantidad),
                                 Productos = g.Select(x => new { x.producto.Nombre, x.ventaDetalle.Cantidad })
                             }).ToList();

                var result = query.Select(q => new
                {
                    q.NombreLocal,
                    ProductoMasVendido = q.Productos.OrderByDescending(p => p.Cantidad).FirstOrDefault()?.Nombre,
                    q.CantidadVentas
                }).ToList();

                foreach (var item in result)
                {
                    list.Add(new LocalProductoVentas
                    {
                        NombreLocal = item.NombreLocal,
                        NombreProducto = item.ProductoMasVendido,
                        CantidadVentas = item.CantidadVentas
                    });
                }

                return new EntityResponse<LocalProductoVentas>(list);
            }
            catch (Exception e)
            {
                return new EntityResponse<LocalProductoVentas>($"Error al obtener el producto más vendido por local: {e.Message}", true);
            }
        }
    }
}