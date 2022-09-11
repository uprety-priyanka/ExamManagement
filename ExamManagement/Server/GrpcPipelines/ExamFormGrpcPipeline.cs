using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;

namespace ExamManagement.Server.GrpcPipelines;

public class ExamFormGrpcPipeline:ExamFormGrpcService.ExamFormGrpcServiceBase
{
    private readonly IExamFormService _examFormService;

    public ExamFormGrpcPipeline(IExamFormService examFormService) 
    {
        _examFormService = examFormService;
    }

    public override async Task<PreFillFormResultMessage> PreFillForm(ApplicationUserIdMessage request, ServerCallContext context)
    {
        var result = await _examFormService.PreFillFormAsync(request.ApplicationUserId);
        return result;
    }

    public override async Task GetFormPerStudent(Empty request, IServerStreamWriter<FormResultMessage> responseStream, ServerCallContext context)
    {
        var formPerStudent = await _examFormService.GetFormPerStudentAsync();

        foreach (var item in formPerStudent) 
        {
            await responseStream.WriteAsync(item);
        }

    }

    public override async Task GetFormPerFaculty(Empty request, IServerStreamWriter<FormResultMessage> responseStream, ServerCallContext context)
    {
        var formPerStudent = await _examFormService.GetFormPerFacultyAsync();

        foreach (var item in formPerStudent)
        {
            await responseStream.WriteAsync(item);
        }
    }

    public override async Task<FillFormResultMessage> FillForm(FillFormMessage request, ServerCallContext context)
    {
        var result = await _examFormService.FillFormAsync(request);
        return result;
    }
}
