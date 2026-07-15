using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface IVentaService
{
    Task<IEnumerable<VentaResultadoDto>> ObtenerTodasAsync();

    Task<VentaResultadoDto> ObtenerPorIdAsync(int id);

    Task<VentaResultadoDto> RegistrarAsync(RegistrarVentaDto dto);
}
