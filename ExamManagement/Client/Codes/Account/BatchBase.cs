using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Account
{
    public class BatchBase:ComponentBase
    {
        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }
        public int FacultyId = 0;

        public StudentMessage[] _courses;
        public List<FacultyMessage> list;

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
            list = new List<FacultyMessage>();
            var facultyStream = _grpcFacultyClient.GetAllFaculty(new SearchMessage { Search = "" });

            while (await facultyStream.ResponseStream.MoveNext())
            {
                list.Add(facultyStream.ResponseStream.Current);
            }
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
    }
}
