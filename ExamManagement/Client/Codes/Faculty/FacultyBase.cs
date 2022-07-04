using ExamManagement.Client.Pages.Faculty;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExamManagement.Client.Codes.Faculty
{
    public class FacultyBase : ComponentBase
    {
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcClient { get; set; }

        public List<FacultyMessage> facultyMessage;

        public async Task FetchData(string s) 
        {
            var result = _grpcClient.GetAllFaculty(new SearchMessage
            {
                Search = s
            });

            facultyMessage = new List<FacultyMessage>();

            while (await result.ResponseStream.MoveNext())
            {
                facultyMessage.Add(result.ResponseStream.Current);
            }
        }

        protected override async Task OnInitializedAsync()
        {

            await FetchData("");
        }

        public async Task OnSearch(string s)
        {
            await FetchData(s);
        }
        
    }
}
