using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Faculty
{
    public class DeleteFacultyDialogBase : ComponentBase
    {
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcClient { get; set; }

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Parameter]
        public FacultyMessage info { get; set; }
        public bool DeleteButton = false;

        public async Task DeleteFaculty() 
        {
            var delete = await _grpcClient.DeleteFacultyAsync(new IdMessage 
            {
                Id = info.Id
            });

            MudDialog.Close(DialogResult.Ok(true));

            if (delete.Success)
            {
                Snackbar.Add(delete.Message, Severity.Success);
            }
            else 
            {
                Snackbar.Add(delete.Message, Severity.Error);
                MudDialog.Close(DialogResult.Cancel());
            }
        }
    }
}
