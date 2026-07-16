using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface IDescuentoService
{
    Task<IEnumerable<DescuentoDto>> ObtenerTodosAsync(string? nombre = null, bool? soloActivos = null);
    Task<DescuentoDto> ObtenerPorIdAsync(int id);
    Task<DescuentoDto> CrearAsync(DescuentoCreateDto dto);
    Task<DescuentoDto> ActualizarAsync(int id, DescuentoUpdateDto dto);
    Task EliminarAsync(int id);
}
