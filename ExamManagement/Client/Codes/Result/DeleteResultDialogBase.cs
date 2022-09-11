using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Result
{
    public class DeleteResultDialogBase:ComponentBase
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        public ResultGrpcService.ResultGrpcServiceClient _grpcClient { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Parameter]
        public GetResultPerFacultyResultMessage info { get; set; }
        public bool DeleteButton = false;

        public async Task DeleteResult() 
        {
            var result = await _grpcClient.DeleteResultAsync(new DeleteResultMessage 
            {
                UserDetailExtensionStudentTemporaryId = info.UserDetailExtensionStudentTemporaryId
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
