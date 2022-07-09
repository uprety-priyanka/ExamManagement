using ExamManagement.Server.Entities;
using Grpc.Protos;

namespace ExamManagement.Server.Services.Abstraction
{
    public interface ICourseService
    {
        Task<List<Course>> GetCourseByFacultyIdAsync(FacultyIdMessage facultyId);
        Task<AddCourseResultMessage> AddCourseAsync(AddCourseMessage addCourseMessage);
        Task<DeleteCourseResultMessage> DeleteCourseAsync(DeleteCourseIdMessage deleteCourseIdMessage);
        Task<UpdateCourseResultMessage> UpdateCourseAsync(UpdateCourseMessage updateCourseMessage);
    }
}
