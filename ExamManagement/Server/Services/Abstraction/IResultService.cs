using ExamManagement.Server.Entities;
using Grpc.Protos;
using System.Security.Claims;

namespace ExamManagement.Server.Services.Abstraction
{
    public interface IResultService
    {
        Task<AddResultResultMessage> AddResultAsync(AddResultMessage model);
        Task<DeleteResultResultMessage> DeleteResultAsync(DeleteResultMessage model);
        Task<UpdateResultResultMessage> UpdateResultAsync(UpdateResultMessage model);
        Task<List<List<ResultExtension>>> GetResultPerFacultyAsync(GetResultPerFacultyMessage model);
        Task<List<ResultExtension>> GetResultPerStudentAsync(GetResultPerStudentMessage model);
    }
}
