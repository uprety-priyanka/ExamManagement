using ExamManagement.Client.Data;
using ExamManagement.Client.Pages.Faculty;
using ExamManagement.Shared.Constants;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using static MudBlazor.CategoryTypes;

namespace ExamManagement.Client.Codes.Result
{
    public class ResultBase: Microsoft.AspNetCore.Components.ComponentBase
    {
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }
        [Inject]
        public ResultGrpcService.ResultGrpcServiceClient _resultClient { get; set; }
        [Inject]
        public FacultyGrpcService.FacultyGrpcServiceClient _facultyClient { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }
        public List<GetResultPerFacultyResultMessage> facultyResultList = new List<GetResultPerFacultyResultMessage>();
        public List<FacultyMessage> facultyList = new List<FacultyMessage>();
        public GetResultPerStudentResultMessage student = new GetResultPerStudentResultMessage();
        public GetResultPerStudentResultSupportMessage[] supportStudentList;
        public string userRole { get; set; }
        public string search = "";
        public int FacultyId;
        public string UserId = "";


        public TableGroupDefinition<GetResultPerFacultyResultSupportMessage> _groupDefinition = new()
        {
            GroupName = "Semester",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.Semester
        };

        public TableGroupDefinition<GetResultPerStudentResultSupportMessage> _groupDefinition2 = new()
        {
            GroupName = "Semester",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.Semester
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var auth = await _authProvider.GetAuthenticationStateAsync();
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                UserId = auth.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                if (userRole == RoleConstant.SUPERADMIN || userRole == RoleConstant.DEPARTMENTADMIN)
                {
                    var fId = await _facultyClient.GetFacultyIdForCurrentlyLoggedInDepartmentUserAsync(new Empty());
                    await FetchData("", fId.Id);

                    facultyList = new List<FacultyMessage>();

                    var faculties = _facultyClient.GetAllFaculty(new SearchMessage
                    {
                        Search = ""
                    });

                    while (await faculties.ResponseStream.MoveNext())
                    {
                        facultyList.Add(faculties.ResponseStream.Current);
                    }
                }
                else if (userRole == RoleConstant.STUDENT) 
                {
                    await FetchStudentData();
                }

            }
            catch (Exception) { }

        }


        public async Task FetchStudentData() 
        {
            student = new GetResultPerStudentResultMessage();
            var result = await _resultClient.GetResultPerStudentAsync(new GetResultPerStudentMessage 
            {
                ApplicationUserId = UserId
            });

            student = result;

            supportStudentList = student.GetResultPerStudentResultSupportMessages.ToArray();
        }


        public async Task UponValueChanged() 
        {
            var facultyId = await _facultyClient.GetFacultyIdForCurrentlyLoggedInDepartmentUserAsync(new Empty());
            await FetchData(search, facultyId.Id);
        }

        public async Task UponValueChangedSuperAdmin() 
        {
            await FetchData(search, FacultyId);
        }

        public async Task FetchData(string search, int FacId) 
        {
            facultyResultList = null;
            facultyResultList = new List<GetResultPerFacultyResultMessage>();
 
            var resultStream = _resultClient.GetResultPerFaculty(new GetResultPerFacultyMessage
            {
                FacultyId = FacId,
                Search = search
            });

            while (await resultStream.ResponseStream.MoveNext())
            {
                facultyResultList.Add(resultStream.ResponseStream.Current);
            }

        }

        public async Task DoStuff(int facultyId) 
        {
            FacultyId = facultyId;
            await FetchData(search, facultyId);
            StateHasChanged();
        }

        public async Task DeleteResult(int userDetailExtensionStudentTemporaryId) 
        {
            var result = await _resultClient.DeleteResultAsync(new DeleteResultMessage 
            {
                UserDetailExtensionStudentTemporaryId = userDetailExtensionStudentTemporaryId
            });

            if (result.Success)
            {
                Snackbar.Add(result.Message, Severity.Success);
            }
            else 
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

    }
}
