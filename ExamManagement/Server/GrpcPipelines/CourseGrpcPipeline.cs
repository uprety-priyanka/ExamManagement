using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;

namespace ExamManagement.Server.GrpcPipelines
{
    public class CourseGrpcPipeline:CourseGrpcService.CourseGrpcServiceBase
    {
        public readonly ICourseService _courseService;

        public CourseGrpcPipeline(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task GetCourseByFacultyId(FacultyIdMessage request, IServerStreamWriter<CourseMessage> responseStream, ServerCallContext context)
        {
            var result = await _courseService.GetCourseByFacultyIdAsync(request);

            foreach (var item in result) 
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(new CourseMessage 
                {
                    Id = item.Id,
                    FacultyId = item.FacultyId,
                    CreatedDate = item.CreatedDate.ToUniversalTime().ToTimestamp(),
                    ConcurrentRegistrationCourse = item.ConcurrentRegistrationCourse,
                    Lecture = item.Lecture,
                    SemesterId = item.SemesterTypeId,
                    CourseCode = item.CourseCode,
                    SemesterName = item.SemesterType.Name,
                    CourseName = item.CourseName,
                    Credit = item.Credit,
                    FacultyName = item.Faculty.FacultyName,
                    Practical = item.Practical,
                    PreRequisiteCourse = item.PreRequisiteCourse,
                    Tutorial = item.Tutorial
                });

            }
        }

        public override async Task<AddCourseResultMessage> AddCourse(AddCourseMessage request, ServerCallContext context)
        {
            var result = await _courseService.AddCourseAsync(request);
            return result;
        }

        public override async Task<DeleteCourseResultMessage> DeleteCourse(DeleteCourseIdMessage request, ServerCallContext context)
        {
            var result = await _courseService.DeleteCourseAsync(request);
            return result;
        }

        public override async Task<UpdateCourseResultMessage> UpadteCourse(UpdateCourseMessage request, ServerCallContext context)
        {
            var result = await _courseService.UpdateCourseAsync(request);
            return result;
        }

    }
}
