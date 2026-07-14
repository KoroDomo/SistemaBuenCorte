using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface ICajaService
{
    Task<IEnumerable<CajaDto>> ObtenerTodasAsync();
    Task<CajaDto> ObtenerPorIdAsync(int id);
    Task<CajaDto?> ObtenerCajaAbiertaAsync(int usuarioId);
    Task<CajaDto> AbrirCajaAsync(AbrirCajaDto dto);
    Task<CajaDto> CerrarCajaAsync(int cajaId, CerrarCajaDto dto);
}