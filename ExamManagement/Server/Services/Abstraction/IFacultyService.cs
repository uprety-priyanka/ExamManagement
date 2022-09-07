using ExamManagement.Server.Entities;
using Grpc.Protos;
using System.Security.Claims;

namespace ExamManagement.Server.Services.Abstraction
{
    public interface IFacultyService
    {
        Task<AddFacultyResultMessage> AddFacultyAsync(AddFacultyMessage addFacultyMessage);
        Task<List<Faculty>> GetAllFacultyAsync(SearchMessage searchMessage);
        Task<FacultyMessage> GetFacultyByIdAsync(IdMessage idMessage);
        Task<UpdateFacultyResultMessage> UpdateFacultyAsync(UpdateFacultyMessage updateFacultyMessage);
        Task<DeleteFacultyResultMessage> DeleteFacultyAsync(IdMessage idMessage);
        Task<IdMessage> GetFacultyIdForDepartmentUserAsync(ClaimsPrincipal user);
    }
}
