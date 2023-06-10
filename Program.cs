using SuperMarket.Aplication;
using SuperMarket.Infrastructure;

using (var context = new SuperMarketContext())
{

    Console.WriteLine("Bienvenidos!");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Importante! Para poder seguir visualizando las consultas pulsa una tecla." +
        "\nLa respuesta inicial dependera de la conexion a internet");


    var service = new MarketService(context);

    await ObtenerTotalVentasUltimosDiasAsync(service, 30); Console.ReadKey();
    await ObtenerVentaConMontoMasAltoAsync(service); Console.ReadKey();
    await ObtenerProductoConMayorMontoVentasAsync(service); Console.ReadKey();
    await ObtenerLocalConMayorMontoVentasAsync(service); Console.ReadKey();
    await ObtenerMarcaConMayorMargenGananciasAsync(service); Console.ReadKey();
    await ObtenerProductoMasVendidoPorLocalAsync(service); Console.ReadKey();

    Console.ReadKey();
}

async Task ObtenerTotalVentasUltimosDiasAsync(MarketService service, int dias)
{
    var ventasXdias = await service.ObtenerTotalVentasUltimosDiasAsync(dias);
    if (!ventasXdias.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nTotal de ventas de los últimos {dias} días:\n");
        ventasXdias.ConsolePrint(ventasXdias.Entity);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{ventasXdias.Message}");
    }
}

async Task ObtenerVentaConMontoMasAltoAsync(MarketService service)
{
    var ventaMasAlta = await service.ObtenerVentaConMontoMasAltoAsync();
    if (!ventaMasAlta.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Venta con el monto más alto:\n");
        ventaMasAlta.ConsolePrint(ventaMasAlta.Entity);
        var sum = ventaMasAlta.Entity.VentaDetalles.Sum(x => x.TotalLinea);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{ventaMasAlta.Message}");
    }
}

static async Task ObtenerProductoConMayorMontoVentasAsync(MarketService service)
{
    var productoMasVendido = await service.ObtenerProductoConMayorMontoVentasAsync();
    if (!productoMasVendido.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Producto con mayor monto de ventas:\n");
        productoMasVendido.ConsolePrint(productoMasVendido.Entity);
        var montoTotal = productoMasVendido.Entity.VentaDetalles.Sum(x => x.TotalLinea);
        Console.WriteLine($"MontoTotal : {montoTotal}");
        Console.WriteLine($"{string.Join("", Enumerable.Repeat("-", 5 + "MontoTotal".Length + montoTotal.ToString().Length))}\n");
        var cantidadTotal = productoMasVendido.Entity.VentaDetalles.Sum(x => x.Cantidad);
        Console.WriteLine($"cantidadTotal : {cantidadTotal}");
        Console.WriteLine($"{string.Join("", Enumerable.Repeat("-", 5 + "cantidadTotal".Length + cantidadTotal.ToString().Length))}\n");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{productoMasVendido.Message}");
    }
}

static async Task ObtenerLocalConMayorMontoVentasAsync(MarketService service)
{
    var localMasVentas = await service.ObtenerLocalConMayorMontoVentasAsync();
    if (!localMasVentas.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Local con mayor monto de ventas:\n");
        localMasVentas.ConsolePrint(localMasVentas.Entity);
        var montoVenta = localMasVentas.Entity.Venta.Sum(x => x.Total);
        Console.WriteLine($"cantidadTotal : {montoVenta}");
        Console.WriteLine($"{string.Join("", Enumerable.Repeat("-", 5 + "Total".Length + montoVenta.ToString().Length))}\n");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{localMasVentas.Message}");
    }
}

async Task ObtenerMarcaConMayorMargenGananciasAsync(MarketService service)
{
    var marcaMayorMargen = await service.ObtenerMarcaConMayorMargenGananciasAsync();
    if (!marcaMayorMargen.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Marca con mayor margen de ganancias:\n");
        marcaMayorMargen.ConsolePrint(marcaMayorMargen.Entity);

        var margenGanancias = marcaMayorMargen.Entity.Productos
            .SelectMany(p => p.VentaDetalles)
            .Sum(vd => (vd.PrecioUnitario * vd.Cantidad) - (vd.IdProductoNavigation.CostoUnitario * vd.Cantidad));

        Console.WriteLine($"margenGanancias : {margenGanancias}");
        Console.WriteLine($"{string.Join("", Enumerable.Repeat("-", 5 + "Total".Length + margenGanancias.ToString().Length))}\n");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{marcaMayorMargen.Message}");
    }
}

async Task ObtenerProductoMasVendidoPorLocalAsync(MarketService service)
{
    var productoMasVendidoPorLocal = await service.ObtenerProductoMasVendidoPorLocalAsync();
    if (!productoMasVendidoPorLocal.Error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Obtener el producto más vendido por local:\n");

        foreach (var local in productoMasVendidoPorLocal.Entities)
        {
            productoMasVendidoPorLocal.ConsolePrint(local);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{string.Join("", Enumerable.Repeat("-", 80))}\n");
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
    else
    {
        Console.WriteLine($"{productoMasVendidoPorLocal.Message}");
    }
}
