using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface IReporteService
{
    Task<ReporteResumenDto> ObtenerResumenAsync();
    Task<IEnumerable<VentaPorDiaDto>> ObtenerVentasPorDiaAsync();
    Task<IEnumerable<ProductoMasVendidoDto>> ObtenerMasVendidosAsync();
    Task<IEnumerable<HistorialVentaDto>> ObtenerHistorialAsync(int cantidad = 10);
}
