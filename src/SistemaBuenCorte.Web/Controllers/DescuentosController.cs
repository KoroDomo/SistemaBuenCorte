using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DescuentosController : ControllerBase
{
    private readonly IDescuentoService _descuentoService;

    public DescuentosController(IDescuentoService descuentoService)
    {
        _descuentoService = descuentoService;
    }

    // GET /api/descuentos?nombre=&soloActivos=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nombre, [FromQuery] bool? soloActivos)
    {
        var descuentos = await _descuentoService.ObtenerTodosAsync(nombre, soloActivos);
        return Ok(descuentos);
    }

    // GET /api/descuentos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var descuento = await _descuentoService.ObtenerPorIdAsync(id);
            return Ok(descuento);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    // POST /api/descuentos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DescuentoCreateDto dto)
    {
        try
        {
            var creado = await _descuentoService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errores = ex.Errores });
        }
    }

    // PUT /api/descuentos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DescuentoUpdateDto dto)
    {
        try
        {
            var actualizado = await _descuentoService.ActualizarAsync(id, dto);
            return Ok(actualizado);
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

    // DELETE /api/descuentos/{id}  — borrado lógico en el backend
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _descuentoService.EliminarAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}
