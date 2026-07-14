namespace SistemaBuenCorte.BLL.DTOs;

public class CajaDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public decimal MontoApertura { get; set; }
    public decimal? MontoCierreContado { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal TotalVentas { get; set; }
}