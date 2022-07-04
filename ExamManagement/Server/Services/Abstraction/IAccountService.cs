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
    }
}
