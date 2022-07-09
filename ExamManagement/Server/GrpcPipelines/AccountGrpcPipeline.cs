using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Authorization;

namespace ExamManagement.Server.GrpcPipelines
{
    public class AccountGrpcPipeline:AccountGrpcService.AccountGrpcServiceBase
    {
        private readonly IAccountService _accountService;

        public AccountGrpcPipeline(IAccountService accountService) 
        {
            _accountService = accountService;
        }


        public override async Task<CurrentUserMessage> GetCurrentUser(Empty request, ServerCallContext context)
        {
            var currentUserResult = await _accountService.GetCurrentUserAsync();
            return currentUserResult;
        }

        public override async Task<LoginUserResultMessage> LoginUser(LoginUserMessage request, ServerCallContext context)
        {
            var loginResult = await _accountService.LoginUserAsync(request);
            return loginResult;
        }

        public override async Task<RegisterSuperAdminResultMessage> RegisterSuperAdmin(RegisterSuperAdminMessage request, ServerCallContext context)
        {
            var registerSuperAdminResult = await _accountService.RegisterSuperAdminAsync(request);
            return registerSuperAdminResult;
        }

        public override async Task<RegisterDepartmentAdminResultMessage> RegisterDepartmentAdmin(RegisterDepartmentAdminMessage request, ServerCallContext context)
        {
            var registerDepartmentAdminResult = await _accountService.RegisterDepartmentAdminAsync(request);
            return registerDepartmentAdminResult;
        }

        public override async Task<CheckEmailResultMessage> CheckEmail(CheckEmailMessage request, ServerCallContext context)
        {
            var checkEmailResult = await _accountService.CheckEmailAsync(request);
            return checkEmailResult;
        }

        public override async Task<CheckUserNameResultMessage> CheckUserName(CheckUserNameMessage request, ServerCallContext context)
        {
            var checkUserNameResult = await _accountService.CheckUserNameAsync(request);
            return checkUserNameResult;
        }

        public override async Task<RegisterStudentResultMessage> RegisterStudent(RegisterStudentMessage request, ServerCallContext context)
        {
            var registerStudentResult = await _accountService.RegisterStudentAsync(request);
            return registerStudentResult;
        }

        public override async Task<Empty> LogOutUser(Empty request, ServerCallContext context)
        {
            await _accountService.LogOutUserAsync();
            return new Empty();
        }

        public override async Task GetDepartmentUser(UserSearchMessage request, IServerStreamWriter<DepartmentUserMessage> responseStream, ServerCallContext context)
        {
            var list = await _accountService.GetAllDepartmentAdminAsync(request);

            foreach (var item in list) 
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(new DepartmentUserMessage 
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    GivenName = item.GivenName,
                    SurName = item.SurName,
                    EmailAddress = item.Email,
                    Department = item.Department
                });

            }
        }

        public override async Task<UserDeleteResultMessage> DeleteDepartmentAdminUser(DepartmentAdminIdMessage request, ServerCallContext context)
        {
            var result = await _accountService.DeleteUserAsync(request);
            return result;
        }


        public override async Task GetStudentByBatch(FacultySearchMessage request, IServerStreamWriter<StudentMessage> responseStream, ServerCallContext context)
        {
            var list = await _accountService.GetStudentByBatchAsync(request);

            foreach (var item in list)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(new StudentMessage 
                {
                    Id=item.Id,
                    Semester = item.Semester,
                    ApplicationUserId = item.UserDetailExtension.UserDetail.ApplicationUserId,
                    EmailAddress = item.UserDetailExtension.UserDetail.ApplicationUser.Email,
                    GivenName = item.UserDetailExtension.UserDetail.ApplicationUser.FirstName,
                    SurName = item.UserDetailExtension.UserDetail.ApplicationUser.LastName,
                    Batch = item.UserDetailExtension.Batch,
                    ExamNumber = item.UserDetailExtension.ExamNumber,
                    ExamYear = item.ExamYear,
                    RegistrationNumber = item.UserDetailExtension.RegistrationNumber,
                    RollNumber = item.UserDetailExtension.RollNumber
                });

            }

        }

    }
}
