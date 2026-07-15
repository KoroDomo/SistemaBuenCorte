using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CajaController : ControllerBase
{
    private readonly ICajaService _cajaService;

    public CajaController(ICajaService cajaService)
    {
        _cajaService = cajaService;
    }

    // GET /api/caja
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cajas = await _cajaService.ObtenerTodasAsync();
        return Ok(cajas);
    }

    // GET /api/caja/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var caja = await _cajaService.ObtenerPorIdAsync(id);
            return Ok(caja);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    // GET /api/caja/abierta/{usuarioId}
    [HttpGet("abierta/{usuarioId}")]
    public async Task<IActionResult> GetCajaAbierta(int usuarioId)
    {
        var caja = await _cajaService.ObtenerCajaAbiertaAsync(usuarioId);
        if (caja is null)
            return NotFound(new { mensaje = "El usuario no tiene una caja abierta actualmente." });
        return Ok(caja);
    }

    // POST /api/caja/abrir
    [HttpPost("abrir")]
    public async Task<IActionResult> Abrir([FromBody] AbrirCajaDto dto)
    {
        try
        {
            var caja = await _cajaService.AbrirCajaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = caja.Id }, caja);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errores = ex.Errores });
        }
    }

    // PUT /api/caja/{id}/cerrar
    [HttpPut("{id}/cerrar")]
    public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarCajaDto dto)
    {
        try
        {
            var caja = await _cajaService.CerrarCajaAsync(id, dto);
            return Ok(caja);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errores = ex.Errores });
        }
    }
}