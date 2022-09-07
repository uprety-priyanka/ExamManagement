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

    public override Task<FillFormResultMessage> FillForm(FillFormMessage request, ServerCallContext context)
    {
        return base.FillForm(request, context);

    }
}
