using ExamManagement.Client.Data;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Course
{
    public class CourseBase:ComponentBase
    {
        [Inject]
        public CourseGrpcService.CourseGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }

        public int FacultyId = 0;

        public CourseMessage[] _courses;
        public List<FacultyMessage> list = new List<FacultyMessage>();

        public string userRole;

        public TableGroupDefinition<CourseMessage> _groupDefinition = new()
        {
            GroupName = "Semester",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.SemesterId
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
            await FetchData(FacultyId);
            StateHasChanged();
        }

        public async Task FetchData(int facultyId) 
        {
            var list = new List<CourseMessage>();
            var result = _grpcClient.GetCourseByFacultyId(new FacultyIdMessage { FacultyId = facultyId });

            while (await result.ResponseStream.MoveNext()) 
            {
                list.Add(result.ResponseStream.Current);
                Console.WriteLine(result.ResponseStream.Current.CourseName);
                
            }
            _courses = list.ToArray();
        }
    }
}
