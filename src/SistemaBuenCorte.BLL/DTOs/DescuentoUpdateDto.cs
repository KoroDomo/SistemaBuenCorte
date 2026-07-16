namespace SistemaBuenCorte.BLL.DTOs;

public class DescuentoUpdateDto
{
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Tipo de descuento: "Porcentaje" o "MontoFijo".</summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Valor del descuento. Si Tipo es "Porcentaje" es un % (0-100);
    /// si es "MontoFijo" es una cantidad en moneda mayor a 0.
    /// </summary>
    public decimal Valor { get; set; }

    public bool Activo { get; set; } = true;
}
