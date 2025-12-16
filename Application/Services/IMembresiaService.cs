using Contract.Requests;
using Contract.Responses;

namespace Application.Services
{
    public interface IMembresiaService
    {
        string? AsociarMembresia(CreateMembresiaRequest request);
        bool Create(CreateMembresiaRequest request);
        bool Update(int id, UpdateMembresiaRequest request);
        List<MembresiaResponse> GetByAlumnoId(int alumnoId);
    }
}
