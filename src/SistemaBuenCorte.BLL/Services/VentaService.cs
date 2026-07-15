using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.DAL.Data;
using SistemaBuenCorte.DAL.Entities;

namespace SistemaBuenCorte.BLL.Services;

public class VentaService : IVentaService
{
    private readonly AppDbContext _context;

    public VentaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VentaResultadoDto>> ObtenerTodasAsync()
    {
        var ventas = await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Factura)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return ventas.Select(MapToDto);
    }

    public async Task<VentaResultadoDto> ObtenerPorIdAsync(int id)
    {
        var venta = await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Factura)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venta is null)
            throw new NotFoundException($"No se encontró una venta con Id {id}.");

        return MapToDto(venta);
    }

    public async Task<VentaResultadoDto> RegistrarAsync(RegistrarVentaDto dto)
    {
        var errores = new List<string>();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId && u.Activo);

        if (usuario is null)
            errores.Add("El usuario especificado no existe o está inactivo.");

        var caja = await _context.Cajas
            .FirstOrDefaultAsync(c =>
                c.UsuarioId == dto.UsuarioId &&
                c.Estado == "Abierta");

        if (caja is null)
            errores.Add("El usuario debe tener una caja abierta para registrar ventas.");

        var metodoPago = (dto.MetodoPago ?? string.Empty).Trim();

        if (!string.Equals(metodoPago, "Efectivo", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(metodoPago, "Tarjeta", StringComparison.OrdinalIgnoreCase))
        {
            errores.Add("El método de pago debe ser 'Efectivo' o 'Tarjeta'.");
        }
        else
        {
            metodoPago = string.Equals(
                metodoPago,
                "Tarjeta",
                StringComparison.OrdinalIgnoreCase)
                ? "Tarjeta"
                : "Efectivo";
        }

        if (dto.PorcentajeDescuento < 0 || dto.PorcentajeDescuento > 100)
            errores.Add("El porcentaje de descuento debe estar entre 0 y 100.");

        var detallesEntrada = dto.Detalles ?? new List<DetalleVentaRequestDto>();

        if (detallesEntrada.Count == 0)
            errores.Add("La venta debe contener al menos un producto.");

        foreach (var detalle in detallesEntrada)
        {
            if (detalle.ProductoId <= 0)
                errores.Add("Todos los productos deben tener un identificador válido.");

            if (detalle.Cantidad <= 0)
                errores.Add($"La cantidad del producto {detalle.ProductoId} debe ser mayor a cero.");
        }

        if (errores.Count > 0)
            throw new ValidationException(errores);

        // Sumar cantidades cuando el mismo producto aparezca varias veces.
        var cantidadesPorProducto = detallesEntrada
            .GroupBy(d => d.ProductoId)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Sum(d => d.Cantidad));

        var productosIds = cantidadesPorProducto.Keys.ToList();

        var productos = await _context.Productos
            .Where(p => productosIds.Contains(p.Id))
            .ToListAsync();

        var productosEncontrados = productos
            .Select(p => p.Id)
            .ToHashSet();

        foreach (var productoId in productosIds)
        {
            if (!productosEncontrados.Contains(productoId))
                errores.Add($"No existe el producto con Id {productoId}.");
        }

        foreach (var producto in productos)
        {
            var cantidadSolicitada = cantidadesPorProducto[producto.Id];

            if (!producto.Activo)
                errores.Add($"El producto '{producto.Nombre}' está inactivo.");

            if (producto.TipoVenta == "Unidad" &&
                cantidadSolicitada != decimal.Truncate(cantidadSolicitada))
            {
                errores.Add(
                    $"El producto '{producto.Nombre}' se vende por unidad y no acepta cantidades decimales.");
            }

            if (producto.Stock < cantidadSolicitada)
            {
                errores.Add(
                    $"Stock insuficiente para '{producto.Nombre}'. " +
                    $"Disponible: {producto.Stock}; solicitado: {cantidadSolicitada}.");
            }
        }

        if (errores.Count > 0)
            throw new ValidationException(errores);

        var detallesVenta = productos.Select(producto =>
        {
            var cantidad = cantidadesPorProducto[producto.Id];
            var subtotalLinea = Math.Round(
                cantidad * producto.Precio,
                2,
                MidpointRounding.AwayFromZero);

            return new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = subtotalLinea
            };
        }).ToList();

        var subtotal = detallesVenta.Sum(d => d.Subtotal);

        var montoDescuento = Math.Round(
            subtotal * (dto.PorcentajeDescuento / 100m),
            2,
            MidpointRounding.AwayFromZero);

        var total = subtotal - montoDescuento;

        await using var transaccion =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                UsuarioId = dto.UsuarioId,
                CajaId = caja!.Id,
                DescuentoId = null,
                Subtotal = subtotal,
                MontoDescuento = montoDescuento,
                Total = total,
                MetodoPago = metodoPago,
                Detalles = detallesVenta
            };

            foreach (var producto in productos)
            {
                producto.Stock -= cantidadesPorProducto[producto.Id];
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            var factura = new Factura
            {
                NumeroFactura = $"FAC-{venta.Id:D6}",
                VentaId = venta.Id,
                FechaEmision = DateTime.Now,
                Subtotal = subtotal,
                Impuesto = 0,
                Total = total,
                NombreCliente = string.IsNullOrWhiteSpace(dto.NombreCliente)
                    ? null
                    : dto.NombreCliente.Trim()
            };

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            await transaccion.CommitAsync();

            venta.Factura = factura;

            return MapToDto(venta);
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }

    private static VentaResultadoDto MapToDto(Venta venta) => new()
    {
        Id = venta.Id,
        Fecha = venta.Fecha,
        UsuarioId = venta.UsuarioId,
        CajaId = venta.CajaId,
        Subtotal = venta.Subtotal,
        MontoDescuento = venta.MontoDescuento,
        Total = venta.Total,
        MetodoPago = venta.MetodoPago,
        NumeroFactura = venta.Factura?.NumeroFactura ?? string.Empty,
        NombreCliente = venta.Factura?.NombreCliente
    };
}
