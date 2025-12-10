using Contract.Requests;
using Contract.Responses;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResponse?> Register(RegisterRequest request);
        Task<AuthResponse?> Login(LoginRequest request);
    }
}
