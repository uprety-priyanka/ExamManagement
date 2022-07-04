using ExamManagement.Shared.Account;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;

namespace ExamManagement.Client.Codes.Account
{
    public class RegisterSuperAdminBase:ComponentBase
    {
		[Inject]
		public AccountGrpcService.AccountGrpcServiceClient _grpcClient{ get; set; }
		[Inject]
		public NavigationManager _navigate { get; set; }
		[Inject]
		ISnackbar Snackbar { get; set; }

		public bool RegisterButton = false;
		public string passError = "";
        public RegisterSuperAdminViewModel registerSuperAdminViewModel = new RegisterSuperAdminViewModel();

		public async Task<string> checkUserName(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return null;
			}
			var result = await _grpcClient.CheckUserNameAsync(new CheckUserNameMessage 
			{
				UserName = userName
			});

			if (result.Exists)
			{
				return "Username is already taken.";
			}
			else
			{
				return null;
			}
		}

		public async Task<string> checkEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return null;
			}
			var result = await _grpcClient.CheckEmailAsync(new CheckEmailMessage 
			{
				Email = email
			});
			if (result.Exists)
			{
				return "Email address already exists.";
			}
			else
			{
				return null;
			}
		}

		public IEnumerable<string> PasswordStrength(string pw)
		{
			if (string.IsNullOrWhiteSpace(pw))
			{
				passError = "Password is required!";
				yield return "Password is required!";
				yield break;
			}
			if (pw.Length < 8)
			{
				passError = "Password must be at least of length 8";
				yield return "Password must be at least of length 8";
			}
			if (!Regex.IsMatch(pw, @"[A-Z]"))
			{
				passError = "Password must contain at least one capital letter";
				yield return "Password must contain at least one capital letter";
			}
			if (!Regex.IsMatch(pw, @"[a-z]"))
			{
				passError = "Password must contain at least one lowercase letter";
				yield return "Password must contain at least one lowercase letter";
			}
			if (!Regex.IsMatch(pw, @"[0-9]"))
			{
				passError = "Password must contain at least one digit";
				yield return "Password must contain at least one digit";
			}
		}


		public async void RegisterSuperAdmin(EditContext editContext) 
		{
			RegisterButton = true;
			var result = await _grpcClient.RegisterSuperAdminAsync(new RegisterSuperAdminMessage 
			{
				GivenName = registerSuperAdminViewModel.FirstName,
				SurName = registerSuperAdminViewModel.LastName,
				Email = registerSuperAdminViewModel.Email,
				UserName = registerSuperAdminViewModel.UserName,
				Password = registerSuperAdminViewModel.Password
			});

			if (result.Success)
			{
				_navigate.NavigateTo("/account/login");
				Snackbar.Add("You have been registered as a super admin.", Severity.Success);
			}
			else 
			{
				Snackbar.Add(result.Message, Severity.Error);
			}
			
		}


	}
}
