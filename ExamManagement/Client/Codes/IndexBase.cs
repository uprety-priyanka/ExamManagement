using ExamManagement.Client.Data;
using ExamManagement.Shared.Constants;
using Google.Protobuf.WellKnownTypes;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace ExamManagement.Client.Codes
{
    public class IndexBase:ComponentBase
    {
        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _accountGrpcClient { get; set; }
        public string userRole = "";
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }

        public CurrentStudentMessage currentStudentMessage = new();
        public CurrentUserMessage currentUserMessage = new();
        public string mudAvatarText = "";
        protected override async Task OnInitializedAsync()
        {
            var auth = await _authProvider.GetAuthenticationStateAsync();

            try
            {
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                if (userRole == RoleConstant.STUDENT) 
                {
                    var result = await _accountGrpcClient.GetCurrentStudentAsync(new Empty());
                    currentStudentMessage = result;

                    mudAvatarText = currentStudentMessage.FirstName.ToArray().FirstOrDefault().ToString() + currentStudentMessage.LastName.ToArray().FirstOrDefault().ToString();
                }
                if (userRole == RoleConstant.SUPERADMIN || userRole == RoleConstant.DEPARTMENTADMIN) 
                {
                    var result = await _accountGrpcClient.GetCurrentUserAsync(new Empty());
                    currentUserMessage = result;
                    mudAvatarText = currentUserMessage.GivenName.ToArray().FirstOrDefault().ToString() + currentUserMessage.SurName.ToArray().FirstOrDefault().ToString();
                }
            }
            catch (Exception) { }
        }
    }
}
