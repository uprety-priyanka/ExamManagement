using ExamManagement.Shared.Result;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Result;

public class AddResultBase:ComponentBase
{
    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }
    public bool submitButton = false;
    public AddResultViewModel model = new AddResultViewModel();
    [Inject]
    public ISnackbar Snackbar { get; set; }
    [Inject]
    public AccountGrpcService.AccountGrpcServiceClient _accountServiceClient { get; set; }
    [Inject]
    public FacultyGrpcService.FacultyGrpcServiceClient _facultyGrpcCient { get; set; }
    [Inject]
    public CourseGrpcService.CourseGrpcServiceClient _courseGrpcClient { get; set; }
    [Inject]
    public ResultGrpcService.ResultGrpcServiceClient _resultGrpcClient { get; set; }
    public List<CourseMessage> courseMessageList = new();
    public List<userDetailExtensionTemporayIdAndNameMessage> userDetailExtensionTemporayIdAndNameMessageList = new();
    

    protected override async Task OnInitializedAsync()
    {
        var departmentUserFacultyId = await _facultyGrpcCient.GetFacultyIdForCurrentlyLoggedInDepartmentUserAsync(new Empty());
        var courses = _courseGrpcClient.GetCourseByFacultyId(new FacultyIdMessage 
        {
            FacultyId = departmentUserFacultyId.Id
        });

        while (await courses.ResponseStream.MoveNext()) 
        {
            courseMessageList.Add(courses.ResponseStream.Current);
        }

        var students = _accountServiceClient.GetStudentInFaculty(new AccFacultyIdMessage 
        {
            FacultyId = departmentUserFacultyId.Id
        });

        while (await students.ResponseStream.MoveNext()) 
        {
            userDetailExtensionTemporayIdAndNameMessageList.Add(students.ResponseStream.Current);
        }

    }

    public async Task AddGradeRecord(EditContext context) 
    {
        submitButton = true;

        var result = await _resultGrpcClient.AddResultAsync(new AddResultMessage 
        {
            CourseId = model.CourseId,
            Grade = model.Grade,
            UserDetailExtensionStudentTemporaryId = model.UserDetailExxtensionTemporartStudentId
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
