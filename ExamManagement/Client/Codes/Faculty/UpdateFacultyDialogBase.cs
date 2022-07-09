using ExamManagement.Shared.Faculty;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Faculty
{
    public class UpdateFacultyDialogBase:ComponentBase
    {
        public UpdateFacultyViewModel model = new();

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        public bool submitButton = false;
        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcCient { get; set; }


        [Parameter]
        public FacultyMessage info { get; set; }

        protected override void OnInitialized()
        {
            model.Id = info.Id;
            model.FacultyName = info.FacultyName;
        }

        public async Task UpadteFacultyRecord(EditContext context)
        {
            submitButton = true;

            var result = await _grpcCient.UpdateFacultyAsync(new UpdateFacultyMessage 
            {
                FacultyName = model.FacultyName,
                Id = model.Id
            });
            submitButton = false;

            MudDialog.Close(DialogResult.Ok(true));

            if (result.Success)
            {
                Snackbar.Add("Record has been updated.", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

    }

}
