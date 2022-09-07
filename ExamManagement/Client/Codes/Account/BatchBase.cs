using ExamManagement.Client.Data;
using ExamManagement.Client.Pages.Result;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Account
{
    public class BatchBase:ComponentBase
    {
        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }
        [Inject]
        
        public EMSAuthenticationStateProvider _authProvider { get; set; }
        public string userRole = "";
        public int FacultyId = 0;

        public StudentMessage[] _courses;
        public List<FacultyMessage> list = new List<FacultyMessage>();

        public HashSet<StudentMessage> studentMessages = new HashSet<StudentMessage>();

        public TableGroupDefinition<StudentMessage> _groupDefinition = new()
        {
            GroupName = "Batch",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.Batch
        };

        protected override async Task OnInitializedAsync()
        {

            var auth = await _authProvider.GetAuthenticationStateAsync();

            try
            {
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                list = new List<FacultyMessage>();
                var facultyStream = _grpcFacultyClient.GetAllFaculty(new SearchMessage { Search = "" });

                while (await facultyStream.ResponseStream.MoveNext())
                {
                    list.Add(facultyStream.ResponseStream.Current);
                }
            }
            catch (Exception) { }
        }

        public async void DoStuff(int newValue)
        {
            FacultyId = newValue;
            StateHasChanged();
            await FetchData(FacultyId, "");
            StateHasChanged();
        }

        public async Task OnSearch(string s) 
        {
            await FetchData(FacultyId, s);
        }


        public async Task FetchData(int facultyId, string s)
        {
            var list = new List<StudentMessage>();
            var result = _grpcClient.GetStudentByBatch(new FacultySearchMessage 
            {
                FacultyId = FacultyId,
                Search = s
            });

            while (await result.ResponseStream.MoveNext())
            {
                list.Add(result.ResponseStream.Current);

            }
            _courses = list.ToArray();
        }


        public async Task UpgradeSemester() 
        {
            var call = _grpcClient.UpgradeStudentSemester();

            foreach (var item in studentMessages) 
            {
                await call.RequestStream.WriteAsync(new UpgradeStudentmessage 
                {
                    Semester = item.Semester,
                    UserDetailExtensionTemporayId = item.Id
                });
            }

            await call.RequestStream.CompleteAsync();

            var response = await call;

            if (response.Success)
            {
                Snackbar.Add("Record has been updated.", Severity.Success);
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
            }

            await FetchData(FacultyId, "");

        }

    }
}
