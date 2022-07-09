using ExamManagement.Shared.Account;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;

namespace ExamManagement.Client.Codes.Account
{
    public class AddDepartmentAdminBase:ComponentBase
    {
        public bool submitButton = false;

        public List<FacultyMessage> list;

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }

        [Inject]

        public AccountGrpcService.AccountGrpcServiceClient _grpcAccountClient { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }

        public IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 8)
            {
                yield return "Password must be at least of length 8";
            }
            if (!Regex.IsMatch(pw, @"[A-Z]"))
            {
                yield return "Password must contain at least one capital letter";
            }
            if (!Regex.IsMatch(pw, @"[a-z]"))
            {
                yield return "Password must contain at least one lowercase letter";
            }
            if (!Regex.IsMatch(pw, @"[0-9]"))
            {
                yield return "Password must contain at least one digit";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            list = new List<FacultyMessage>();
            var facultyStream = _grpcFacultyClient.GetAllFaculty(new SearchMessage { Search = ""});

            while (await facultyStream.ResponseStream.MoveNext()) 
            {
                list.Add(facultyStream.ResponseStream.Current);
            }
        }

        public AddDepartmentAdminViewModel model = new AddDepartmentAdminViewModel();

        public async Task AddDepartmentAdminRecord(EditContext context) 
        {
            var result = await _grpcAccountClient.RegisterDepartmentAdminAsync(new RegisterDepartmentAdminMessage 
            {
                FacultyId = model.FacultyId,
                Email = model.Email,
                GivenName = model.GivenName,
                Password = model.Password,
                SurName = model.SurName,
                UserName = model.UserName,
            });

            if (result.Success)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(result.Message, Severity.Success);
            }
            else 
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

    }
}
