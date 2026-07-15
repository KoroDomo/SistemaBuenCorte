using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;

    public VentasController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    // GET /api/ventas
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ventas = await _ventaService.ObtenerTodasAsync();
        return Ok(ventas);
    }

    // GET /api/ventas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var venta = await _ventaService.ObtenerPorIdAsync(id);
            return Ok(venta);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    // POST /api/ventas
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarVentaDto dto)
    {
        try
        {
            var venta = await _ventaService.RegistrarAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = venta.Id },
                venta);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errores = ex.Errores });
        }
    }
}
