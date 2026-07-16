using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    // GET /api/reportes/resumen
    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen()
    {
        var resumen = await _reporteService.ObtenerResumenAsync();
        return Ok(resumen);
    }

    // GET /api/reportes/ventas-por-dia
    [HttpGet("ventas-por-dia")]
    public async Task<IActionResult> GetVentasPorDia()
    {
        var ventas = await _reporteService.ObtenerVentasPorDiaAsync();
        return Ok(ventas);
    }

    // GET /api/reportes/mas-vendidos
    [HttpGet("mas-vendidos")]
    public async Task<IActionResult> GetMasVendidos()
    {
        var productos = await _reporteService.ObtenerMasVendidosAsync();
        return Ok(productos);
    }

    // GET /api/reportes/historial?cantidad=10
    [HttpGet("historial")]
    public async Task<IActionResult> GetHistorial([FromQuery] int cantidad = 10)
    {
        var historial = await _reporteService.ObtenerHistorialAsync(cantidad);
        return Ok(historial);
    }
}
