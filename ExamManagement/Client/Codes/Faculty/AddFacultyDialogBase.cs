using ExamManagement.Shared.Faculty;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Faculty
{
    public class AddFacultyDialogBase:ComponentBase
    {
        [CascadingParameter] 
        public MudDialogInstance MudDialog { get; set; }
        public bool submitButton = false;
        public AddFacultyViewModel model = new AddFacultyViewModel();

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcCient { get; set; }


        public async Task AddFacultyRecord(EditContext context)
        {
            submitButton = true;

            var result = await _grpcCient.AddFacultyAsync(new AddFacultyMessage 
            {
                FacultyName = model.FacultyName
            });
            submitButton = false;

            MudDialog.Close(DialogResult.Ok(true));

            if (result.Success)
            {
                Snackbar.Add("New record has been added.", Severity.Success);
            }
            else 
            {
                Snackbar.Add(result.Message, Severity.Error);
            }

        }

    }
}
