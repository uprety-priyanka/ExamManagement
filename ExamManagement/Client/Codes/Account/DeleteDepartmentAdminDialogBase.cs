using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Account
{
    public class DeleteDepartmentAdminDialogBase:ComponentBase
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _grpcClient { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Parameter]
        public DepartmentUserMessage info { get; set; }
        public bool DeleteButton = false;

        public async Task DeleteDepartmentAdmin() 
        {
            var result = await _grpcClient.DeleteDepartmentAdminUserAsync(new DepartmentAdminIdMessage 
            {
                UserId = info.Id
            });

            if (result.Success)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add("Department user has been deleted.", Severity.Success);
            }
            else 
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

    }

}
