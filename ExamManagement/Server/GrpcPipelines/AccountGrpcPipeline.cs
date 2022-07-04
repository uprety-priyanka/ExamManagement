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

    }
}
