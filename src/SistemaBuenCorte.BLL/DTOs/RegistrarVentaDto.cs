namespace SistemaBuenCorte.BLL.DTOs;

public class RegistrarVentaDto
{
    public int UsuarioId { get; set; }

    public string MetodoPago { get; set; } = "Efectivo";

    public decimal PorcentajeDescuento { get; set; }

    public string? NombreCliente { get; set; }

    public List<DetalleVentaRequestDto> Detalles { get; set; } = new();
}
