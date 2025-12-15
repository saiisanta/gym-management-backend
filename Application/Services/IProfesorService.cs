using Contract.Requests;
using Contract.Responses;
using Domain.Entities;

namespace Application.Services
{
    public interface IProfesorService
    {
        List<ProfesorResponse> GetAll();
        List<ProfesorResponse> GetBySucursalId(int sucursalId);
        ProfesorResponse? GetById(int id);
        public ProfesorResponse? Create(CreateProfesorRequest request);
        bool Update(int id, UpdateProfesorRequest request);
        bool Delete(int id);
    }
}