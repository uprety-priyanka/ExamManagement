using ExamManagement.Server.Entities;
using ExamManagement.Shared.Account;
using Grpc.Protos;

namespace ExamManagement.Server.Services.Abstraction
{
    public interface IAccountService
    {
        Task<CurrentUserMessage> GetCurrentUserAsync();
        Task<LoginUserResultMessage> LoginUserAsync(LoginUserMessage loginUserMessage);
        Task<RegisterSuperAdminResultMessage> RegisterSuperAdminAsync(RegisterSuperAdminMessage registerSuperAdminMessage);
        Task<CheckUserNameResultMessage> CheckUserNameAsync(CheckUserNameMessage checkUserNameMessage);
        Task<CheckEmailResultMessage> CheckEmailAsync(CheckEmailMessage checkEmailMessage);
        Task<RegisterDepartmentAdminResultMessage> RegisterDepartmentAdminAsync(RegisterDepartmentAdminMessage registerDepartmentAdminMessage);
        Task<RegisterStudentResultMessage> RegisterStudentAsync(RegisterStudentMessage registerStudentMessage);
        Task LogOutUserAsync();
        Task<List<DepartmentUserViewModel>> GetAllDepartmentAdminAsync(UserSearchMessage userSearchMessage);
        Task<UserDeleteResultMessage> DeleteUserAsync(DepartmentAdminIdMessage departmentAdminIdMessage);
        Task<List<UserDetailExtensionStudentTemporary>> GetStudentByBatchAsync(FacultySearchMessage message);
        Task<DeleteStudentResultMessage> DeleteStudentAsync(StudentIdMessage studentIdMessage);
        Task<UpgradeStudentResultmessage> UpgradeSemesterAsync(UpgradeStudentmessage studentmessage);
        Task<List<UserDetailExtensionStudentTemporary>> GetStudentInFacultyAsync(AccFacultyIdMessage facultyId);
    }
}
