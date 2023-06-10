--Total de ventas de los últimos 30 días
SELECT 
    SUM(Total) AS MontoTotal,
    COUNT(*) AS CantidadVentas
FROM Venta
WHERE Fecha >= DATEADD(DAY, -30, GETDATE());

--Venta con el monto más alto
SELECT TOP 1
	ID_Venta,  Fecha,  Total, ID_Local
FROM Venta
ORDER BY Total DESC;


--Producto con mayor monto de ventas
SELECT TOP 1
    p.Codigo,
    p.Modelo,
    --DISTINCT p.Nombre AS NombreProducto,
    SUM(vd.Cantidad) AS Cantidad,
    SUM(vd.TotalLinea) AS MontoTotalVentas
FROM Producto p
LEFT JOIN VentaDetalle vd ON p.ID_Producto = vd.ID_Producto
GROUP BY p.Codigo, p.Modelo, p.Nombre
ORDER BY MontoTotalVentas DESC;
--SELECT * FROM [Prueba].[dbo].Producto where Producto.Nombre = 'Café Modelo LNBCTD'


--Local con mayor monto de ventas:
SELECT TOP 1
	l.ID_Local,
    l.Nombre AS NombreLocal,
    l.Direccion AS DireccionLocal,
    SUM(v.Total) AS MontoVentas
FROM Local l
INNER JOIN Venta v ON l.ID_Local = v.ID_Local
GROUP BY l.Nombre, l.Direccion, l.ID_Local
ORDER BY MontoVentas DESC;

--Marca con mayor margen de ganancias

SELECT TOP 1
    m.Nombre AS NombreMarca,
    SUM((vd.Precio_Unitario * vd.Cantidad) - (p.Costo_Unitario * vd.Cantidad)) AS MargenGanancias
FROM Marca m
INNER JOIN Producto p ON m.ID_Marca = p.ID_Marca
INNER JOIN VentaDetalle vd ON p.ID_Producto = vd.ID_Producto
GROUP BY m.Nombre
ORDER BY MargenGanancias DESC;

--Obtener el producto más vendido por local

SELECT l.Nombre AS NombreLocal, MAX(p.Nombre) AS ProductoMasVendido, SUM(vd.Cantidad) AS CantidadVentas
FROM Local l
JOIN Venta v ON l.ID_Local = v.ID_Local
JOIN VentaDetalle vd ON v.ID_Venta = vd.ID_Venta
JOIN Producto p ON vd.ID_Producto = p.ID_Producto
GROUP BY l.Nombre, l.ID_Local;

