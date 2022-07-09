using ExamManagement.Shared.Course;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ExamManagement.Client.Codes.Course
{
    public class UpdateCourseDialogBase:ComponentBase
    {
        public bool submitButton = false;

        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        public List<FacultyMessage> list;
        [Inject]

        public FacultyGrpcService.FacultyGrpcServiceClient _grpcFacultyClient { get; set; }
        [Inject]
        public CourseGrpcService.CourseGrpcServiceClient _grpcCourseClient { get; set; }

        public UpdateCourseViewModel model = new UpdateCourseViewModel();

        [Parameter]
        public CourseMessage info { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        public async Task UpdateCourseRecord(EditContext context)
        {
            submitButton = true;

            if (model.PreRequisiteCourse == null)
            {
                model.PreRequisiteCourse = "";
            }
            if (model.ConcurrentRegistrationCourse == null)
            {
                model.ConcurrentRegistrationCourse = "";
            }

            var result = await _grpcCourseClient.UpadteCourseAsync(new UpdateCourseMessage 
            {
                Id = model.Id,
                CourseName = model.CourseName,
                CourseCode = model.CourseCode,
                Tutorial = model.Tutorial,
                PreRequisiteCourse = model.PreRequisiteCourse,
                Credit = model.Credit,
                FacultyId = model.FacultyId,
                Lecture = model.Lecture,
                Practical = model.Practical,
                SemesterId = model.SemesterTypeId,
                ConcurrentRegistrationCourse = model.ConcurrentRegistrationCourse,
            });
            submitButton = false;

            if (result.Success)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add("Record has been updated.", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            list = new List<FacultyMessage>();
            var facultyStream = _grpcFacultyClient.GetAllFaculty(new SearchMessage { Search = "" });

            while (await facultyStream.ResponseStream.MoveNext())
            {
                list.Add(facultyStream.ResponseStream.Current);
            }

            model.Id = info.Id;
            model.CourseName = info.CourseName;
            model.CourseCode = info.CourseCode;
            model.Tutorial = info.Tutorial;
            model.PreRequisiteCourse = info.PreRequisiteCourse;
            model.ConcurrentRegistrationCourse = info.ConcurrentRegistrationCourse;
            model.Credit = info.Credit;
            model.FacultyId = info.FacultyId;
            model.Lecture = info.Lecture;
            model.Practical = info.Practical;
            model.SemesterTypeId = info.SemesterId;
        }

    }
}
