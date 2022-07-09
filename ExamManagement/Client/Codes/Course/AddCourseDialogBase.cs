using ExamManagement.Shared.Course;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Course
{
    public class AddCourseDialogBase : ComponentBase
    {
        public bool submitButton = false;

        public List<FacultyMessage> list;

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        [Inject]

        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }
        [Inject]
        public CourseGrpcService.CourseGrpcServiceClient _grpcCourseClient { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }

        public AddCourseViewModel model = new();

        protected override async Task OnInitializedAsync()
        {
            list = new List<FacultyMessage>();
            var facultyStream = _grpcFacultyClient.GetAllFaculty(new SearchMessage { Search = "" });

            while (await facultyStream.ResponseStream.MoveNext())
            {
                list.Add(facultyStream.ResponseStream.Current);
            }
        }

        public async Task AddCourseRecord(EditContext context) 
        {

            if (model.PreRequisiteCourse == null) 
            {
                model.PreRequisiteCourse = "";
            }
            if (model.ConcurrentRegistrationCourse == null) 
            {
                model.ConcurrentRegistrationCourse = "";
            }

            var result = await _grpcCourseClient.AddCourseAsync(new AddCourseMessage 
            {
                FacultyId = model.FacultyId,
                SemesterId = model.SemesterTypeId,
                CourseCode = model.CourseCode,
                CourseName = model.CourseName,
                ConcurrentRegistrationCourse = model.ConcurrentRegistrationCourse,
                Credit =  model.Credit,
                Lecture = model.Lecture,
                Practical = model.Practical,
                PreRequisiteCourse = model.PreRequisiteCourse,
                Tutorial = model.Tutorial,
            });

            if (result.Success)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(result.Message, Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

    }
}
