using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public interface IUsuarioService
    {
        Usuario? GetByEmail(string email);
        Usuario? GetById(int id);
        bool ExistsByEmail(string email);
        bool ExistsByDni(string dni);
        Usuario? GetWithPasswordByEmail(string email);
        bool Desactivar(int id);
        UsuarioResponse? GetDtoByEmail(string email);
        UsuarioResponse? GetDtoById(int id);
        List<UsuarioResponse> GetAllDtos();
        List<UsuarioResponse> GetAllDtos(int? roleId, int? sucursalId);
        (List<UsuarioResponse> Items, int Total) GetPagedDtos(
            int page,
            int pageSize,
            string? q = null
        );
        bool Create(RegisterRequest request);
        UsuarioResponse? Update(int id, UpdateUsuarioRequest request);

        bool Delete(int id);
    }
}
