using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.DAL.Data;
using SistemaBuenCorte.DAL.Entities;

namespace SistemaBuenCorte.BLL.Services;

public class DescuentoService : IDescuentoService
{
    private const string TipoPorcentaje = "Porcentaje";
    private const string TipoMontoFijo = "MontoFijo";

    private readonly AppDbContext _context;

    public DescuentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DescuentoDto>> ObtenerTodosAsync(string? nombre = null, bool? soloActivos = null)
    {
        var query = _context.Descuentos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(d => d.Nombre.Contains(nombre));

        if (soloActivos == true)
            query = query.Where(d => d.Activo);

        var descuentos = await query
            .OrderBy(d => d.Nombre)
            .ToListAsync();

        return descuentos.Select(MapToDto);
    }

    public async Task<DescuentoDto> ObtenerPorIdAsync(int id)
    {
        var descuento = await _context.Descuentos.FindAsync(id);

        if (descuento is null)
            throw new NotFoundException($"No se encontró un descuento con Id {id}.");

        return MapToDto(descuento);
    }

    public async Task<DescuentoDto> CrearAsync(DescuentoCreateDto dto)
    {
        var tipoNormalizado = NormalizarTipo(dto.Tipo);
        var errores = ValidarDatosBasicos(dto.Nombre, tipoNormalizado, dto.Valor);

        var nombreDuplicado = await _context.Descuentos
            .AnyAsync(d => d.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

        if (nombreDuplicado)
            errores.Add("Ya existe un descuento registrado con ese nombre.");

        if (errores.Count > 0)
            throw new ValidationException(errores);

        var descuento = new Descuento
        {
            Nombre = dto.Nombre.Trim(),
            Tipo = tipoNormalizado,
            Valor = dto.Valor,
            Activo = true
        };

        _context.Descuentos.Add(descuento);
        await _context.SaveChangesAsync();

        return MapToDto(descuento);
    }

    public async Task<DescuentoDto> ActualizarAsync(int id, DescuentoUpdateDto dto)
    {
        var descuento = await _context.Descuentos.FindAsync(id);

        if (descuento is null)
            throw new NotFoundException($"No se encontró un descuento con Id {id}.");

        var tipoNormalizado = NormalizarTipo(dto.Tipo);
        var errores = ValidarDatosBasicos(dto.Nombre, tipoNormalizado, dto.Valor);

        var nombreDuplicado = await _context.Descuentos
            .AnyAsync(d => d.Id != id && d.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

        if (nombreDuplicado)
            errores.Add("Ya existe otro descuento registrado con ese nombre.");

        if (errores.Count > 0)
            throw new ValidationException(errores);

        descuento.Nombre = dto.Nombre.Trim();
        descuento.Tipo = tipoNormalizado;
        descuento.Valor = dto.Valor;
        descuento.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return MapToDto(descuento);
    }

    public async Task EliminarAsync(int id)
    {
        var descuento = await _context.Descuentos.FindAsync(id);

        if (descuento is null)
            throw new NotFoundException($"No se encontró un descuento con Id {id}.");

        // Borrado lógico: no se elimina físicamente para no perder el historial
        // (ventas pasadas que pudieran referenciar este descuento mediante DescuentoId).
        descuento.Activo = false;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Normaliza el tipo a la forma canónica ("Porcentaje" o "MontoFijo"),
    /// aceptando cualquier combinación de mayúsculas/minúsculas. Devuelve el
    /// valor recortado tal cual si no coincide, para que la validación lo rechace.
    /// </summary>
    private static string NormalizarTipo(string? tipo)
    {
        var valor = (tipo ?? string.Empty).Trim();

        if (string.Equals(valor, TipoPorcentaje, StringComparison.OrdinalIgnoreCase))
            return TipoPorcentaje;

        if (string.Equals(valor, TipoMontoFijo, StringComparison.OrdinalIgnoreCase))
            return TipoMontoFijo;

        return valor;
    }

    private static List<string> ValidarDatosBasicos(string nombre, string tipo, decimal valor)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
            errores.Add("El nombre del descuento es obligatorio.");
        else if (nombre.Trim().Length > 80)
            errores.Add("El nombre no puede superar los 80 caracteres.");

        if (tipo != TipoPorcentaje && tipo != TipoMontoFijo)
        {
            errores.Add("El tipo de descuento debe ser 'Porcentaje' o 'MontoFijo'.");
        }
        else
        {
            // Validaciones que dependen del tipo (solo si el tipo es válido).
            if (valor <= 0)
                errores.Add("El valor del descuento debe ser mayor a cero.");
            else if (tipo == TipoPorcentaje && valor > 100)
                errores.Add("Un descuento por porcentaje no puede superar el 100%.");
        }

        return errores;
    }

    private static DescuentoDto MapToDto(Descuento descuento) => new()
    {
        Id = descuento.Id,
        Nombre = descuento.Nombre,
        Tipo = descuento.Tipo,
        Valor = descuento.Valor,
        Activo = descuento.Activo
    };
}
