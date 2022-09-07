using Grpc.Protos;

namespace ExamManagement.Server.Services.Abstraction;

public interface IExamFormService
{
    Task<PreFillFormResultMessage> PreFillFormAsync(string appUserId);
}
