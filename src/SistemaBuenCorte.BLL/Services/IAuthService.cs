using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}