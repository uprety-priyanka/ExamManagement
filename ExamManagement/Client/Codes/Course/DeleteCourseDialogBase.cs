using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Course
{
    public class DeleteCourseDialogBase:ComponentBase
    {

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        public CourseGrpcService.CourseGrpcServiceClient _grpcClient { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Parameter]
        public CourseMessage info { get; set; }
        public bool DeleteButton = false;

        public async Task DeleteCourse() 
        {
            var result = await _grpcClient.DeleteCourseAsync(new DeleteCourseIdMessage { Id = info.Id});

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
