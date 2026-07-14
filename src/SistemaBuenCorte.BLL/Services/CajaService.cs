using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.DAL.Data;
using SistemaBuenCorte.DAL.Entities;

namespace SistemaBuenCorte.BLL.Services;

public class CajaService : ICajaService
{
    private readonly AppDbContext _context;

    public CajaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CajaDto>> ObtenerTodasAsync()
    {
        var cajas = await _context.Cajas
            .Include(c => c.Usuario)
            .Include(c => c.Ventas)
            .OrderByDescending(c => c.FechaApertura)
            .ToListAsync();

        return cajas.Select(MapToDto);
    }

    public async Task<CajaDto> ObtenerPorIdAsync(int id)
    {
        var caja = await _context.Cajas
            .Include(c => c.Usuario)
            .Include(c => c.Ventas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (caja is null)
            throw new NotFoundException($"No se encontró una caja con Id {id}.");

        return MapToDto(caja);
    }

    public async Task<CajaDto?> ObtenerCajaAbiertaAsync(int usuarioId)
    {
        var caja = await _context.Cajas
            .Include(c => c.Usuario)
            .Include(c => c.Ventas)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Estado == "Abierta");

        return caja is null ? null : MapToDto(caja);
    }

    public async Task<CajaDto> AbrirCajaAsync(AbrirCajaDto dto)
    {
        var errores = new List<string>();

        // Validar que el usuario existe
        var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
        if (usuario is null)
            errores.Add("El usuario especificado no existe.");

        // Validar que el usuario no tenga ya una caja abierta
        var cajaAbierta = await _context.Cajas
            .AnyAsync(c => c.UsuarioId == dto.UsuarioId && c.Estado == "Abierta");
        if (cajaAbierta)
            errores.Add("El usuario ya tiene una caja abierta. Debe cerrarla antes de abrir una nueva.");

        // Validar monto de apertura
        if (dto.MontoApertura < 0)
            errores.Add("El monto de apertura no puede ser negativo.");

        if (errores.Count > 0)
            throw new ValidationException(errores);

        var caja = new Caja
        {
            UsuarioId = dto.UsuarioId,
            MontoApertura = dto.MontoApertura,
            FechaApertura = DateTime.Now,
            Estado = "Abierta"
        };

        _context.Cajas.Add(caja);
        await _context.SaveChangesAsync();

        // Recargar con el usuario incluido para el DTO
        await _context.Entry(caja).Reference(c => c.Usuario).LoadAsync();

        return MapToDto(caja);
    }

    public async Task<CajaDto> CerrarCajaAsync(int cajaId, CerrarCajaDto dto)
    {
        var caja = await _context.Cajas
            .Include(c => c.Usuario)
            .Include(c => c.Ventas)
            .FirstOrDefaultAsync(c => c.Id == cajaId);

        if (caja is null)
            throw new NotFoundException($"No se encontró una caja con Id {cajaId}.");

        var errores = new List<string>();

        if (caja.Estado == "Cerrada")
            errores.Add("La caja ya está cerrada.");

        if (dto.MontoCierreContado < 0)
            errores.Add("El monto de cierre contado no puede ser negativo.");

        if (errores.Count > 0)
            throw new ValidationException(errores);

        caja.MontoCierreContado = dto.MontoCierreContado;
        caja.FechaCierre = DateTime.Now;
        caja.Estado = "Cerrada";

        await _context.SaveChangesAsync();

        return MapToDto(caja);
    }

    private static CajaDto MapToDto(Caja caja) => new()
    {
        Id = caja.Id,
        UsuarioId = caja.UsuarioId,
        NombreUsuario = caja.Usuario?.NombreCompleto ?? string.Empty,
        MontoApertura = caja.MontoApertura,
        MontoCierreContado = caja.MontoCierreContado,
        FechaApertura = caja.FechaApertura,
        FechaCierre = caja.FechaCierre,
        Estado = caja.Estado,
        TotalVentas = caja.Ventas.Sum(v => v.Total)
    };
}