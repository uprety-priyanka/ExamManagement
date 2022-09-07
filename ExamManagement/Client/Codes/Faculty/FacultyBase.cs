using ExamManagement.Client.Data;
using ExamManagement.Client.Pages.Faculty;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Faculty
{
    public class FacultyBase : ComponentBase
    {
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }
        public string userRole = "";

        public List<FacultyMessage> facultyMessage = new List<FacultyMessage>();
        public string search;

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
            var auth = await _authProvider.GetAuthenticationStateAsync();

            try
            {
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                await FetchData("");
            }
            catch (Exception) { }
        }

        public async Task OnSearch(string s)
        {
            await FetchData(s);
        }
        
    }
}
