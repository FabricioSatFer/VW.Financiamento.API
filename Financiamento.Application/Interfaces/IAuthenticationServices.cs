using Financiamento.Application.DTOs;

namespace Financiamento.Application.Interfaces
{
    public interface IAuthenticationServices
    {
        Task<LoginResponseDto> Autenticar(LoginRequest req);
    }
}
