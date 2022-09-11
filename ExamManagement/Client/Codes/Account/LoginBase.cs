using ExamManagement.Client.Data;
using ExamManagement.Client.Pages.Account;
using ExamManagement.Shared.Account;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Account
{
    public class LoginBase:ComponentBase
    {

        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public EMSAuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject]
        public NavigationManager _navigate { get; set; }

        public string InfoDisplayer = "";
        public bool loginButton = false;

        public LoginViewModel loginViewModel = new LoginViewModel();

        public async Task LogUserIn(EditContext editContext) 
        {
            loginButton = true;
            InfoDisplayer = "";
            var result = await _grpcClient.LoginUserAsync(new LoginUserMessage 
            {
                Info = loginViewModel.Info,
                Password = loginViewModel.Password
            });
            if (result.Success)
            {
                await _authenticationStateProvider.UpdateAuthenticationState();
                _navigate.NavigateTo("/");
            }
            else 
            {
                loginButton = false;
                if (result.LoginErrorType == LoginUserResultMessage.Types.LoginErrorTypes.IncorrectPassword) 
                {
                    InfoDisplayer = "Your password is incorrect";
                }
                if (result.LoginErrorType == LoginUserResultMessage.Types.LoginErrorTypes.NoSuchUser) 
                {
                    InfoDisplayer = "We couldn't find you.";
                }
            }
        }
    }
}
