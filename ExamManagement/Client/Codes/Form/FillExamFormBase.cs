using ExamManagement.Client.Data;
using ExamManagement.Shared.Form;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Form
{
    public class FillExamFormBase:ComponentBase
    {
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }

        public string appUserId = "";
        public string userRole = "";
        public string ef = "/examform";

        public FillFormViewModel model = new FillFormViewModel();

        public bool submitButton = false;

        [Inject]
        public ISnackbar Snackbar { get; set; }
        [Inject]
        public ExamFormGrpcService.ExamFormGrpcServiceClient _examFormGrpcClient { get; set; }

        public PreFillFormResultMessage preFillFormResultMessage = new();

        public List<PreFillRegularFormResultSupportMessage> preFillRegularFormResultSupportMessages = new();
        public List<PreFillConcurrentFormResultSupportMessage> preFillConcurrentFormResultSupportMessages = new();
        public List<PreFillPrerequisiteFormResultSupportMessage> preFillPrerequisiteFormResultSupportMessages = new();
        public List<PreFillBackFormResultSupportMessage> preFillBackFormResultSupportMessages = new();

        public IEnumerable<int> selectedRegularSubjects { get; set; } = new HashSet<int>();
        public IEnumerable<int> selectedConcurrentSubjects { get; set; } = new HashSet<int>();
        public IEnumerable<int> selectedPrerequisiteSubjects { get; set; } = new HashSet<int>();
        public IEnumerable<int> selectedBackSubjects { get; set; } = new HashSet<int>();

        protected override async Task OnInitializedAsync()
        {
          
            try
            {
                model = new FillFormViewModel();

                preFillRegularFormResultSupportMessages = new List<PreFillRegularFormResultSupportMessage>();

                var auth = await _authProvider.GetAuthenticationStateAsync();

                appUserId = auth.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                var subjects = await _examFormGrpcClient.PreFillFormAsync(new ApplicationUserIdMessage 
                {
                    ApplicationUserId = appUserId,
                });

                preFillRegularFormResultSupportMessages = subjects.PreFillRegularFormResultSupportMessages.ToList();
                preFillConcurrentFormResultSupportMessages = subjects.PreFillConcurrentFormResultSupportMessages.ToList();
                preFillPrerequisiteFormResultSupportMessages = subjects.PreFillPrerequisiteFormResultSupportMessages.ToList();
                preFillBackFormResultSupportMessages = subjects.PreFillBackFormResultSupportMessages.ToList();

            }
            catch (Exception) { }
        }

        public async Task FillForm(EditContext context) 
        {

        }
    }
}
