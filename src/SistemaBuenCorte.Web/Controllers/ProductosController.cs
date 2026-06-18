using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    // GET /api/productos
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        var productos = await _productoService.ObtenerTodosAsync(nombre, categoria);
        return Ok(productos);
    }

    // GET /api/productos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        return Ok(producto);
    }

    // POST /api/productos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
    {
        var creado = await _productoService.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    // PUT /api/productos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
    {
        var actualizado = await _productoService.ActualizarAsync(id, dto);
        return Ok(actualizado);
    }

    // DELETE /api/productos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productoService.EliminarAsync(id);
        return NoContent();
    }
}