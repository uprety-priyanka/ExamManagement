using ExamManagement.Server.Data;
using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;

namespace ExamManagement.Server.GrpcPipelines
{
    public class FacultyGrpcPipeline: FacultyGrpcService.FacultyGrpcServiceBase
    {

        public readonly IFacultyService _facultyService;

        public FacultyGrpcPipeline(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        public override async Task GetAllFaculty(SearchMessage request, IServerStreamWriter<FacultyMessage> responseStream, ServerCallContext context)
        {
            var faculties = await _facultyService.GetAllFacultyAsync(request);

            foreach (var item in faculties) 
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(new FacultyMessage 
                {
                    Id = item.Id,
                    FacultyName = item.FacultyName,
                    CreatedDate = item.CreatedDate.ToUniversalTime().ToTimestamp()
                });
            }
        }

        public override async Task<FacultyMessage> GetFacultyById(IdMessage request, ServerCallContext context)
        {
            var result = await _facultyService.GetFacultyByIdAsync(request);
            return result;
        }

        public override async Task<AddFacultyResultMessage> AddFaculty(AddFacultyMessage request, ServerCallContext context)
        {
            var result = await _facultyService.AddFacultyAsync(request);
            return result;
        }

        public override async Task<UpdateFacultyResultMessage> UpdateFaculty(UpdateFacultyMessage request, ServerCallContext context)
        {
            var result = await _facultyService.UpdateFacultyAsync(request);
            return result;
        }

        public override async Task<DeleteFacultyResultMessage> DeleteFaculty(IdMessage request, ServerCallContext context)
        {
            var result = await _facultyService.DeleteFacultyAsync(request);
            return result;
        }

        public override async Task<IdMessage> GetFacultyIdForCurrentlyLoggedInDepartmentUser(Empty request, ServerCallContext context)
        {
            var result = await _facultyService.GetFacultyIdForDepartmentUserAsync(context.GetHttpContext().User);
            return result;
        }

    }
}
