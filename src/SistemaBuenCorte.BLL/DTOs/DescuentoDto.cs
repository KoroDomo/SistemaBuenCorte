namespace SistemaBuenCorte.BLL.DTOs;

public class DescuentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool Activo { get; set; }
}
