namespace SistemaBuenCorte.BLL.DTOs;

public class VentaResultadoDto
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public int UsuarioId { get; set; }

    public int CajaId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal MontoDescuento { get; set; }

    public decimal Total { get; set; }

    public string MetodoPago { get; set; } = string.Empty;

    public string NumeroFactura { get; set; } = string.Empty;

    public string? NombreCliente { get; set; }
}
