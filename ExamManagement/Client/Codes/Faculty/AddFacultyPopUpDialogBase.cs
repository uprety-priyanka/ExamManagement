using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Faculty
{
    public class AddFacultyPopUpDialogBase:ComponentBase
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public void RedirectToFaculty() 
        {
            NavigationManager.NavigateTo("/faculty");
        }
    }
}
