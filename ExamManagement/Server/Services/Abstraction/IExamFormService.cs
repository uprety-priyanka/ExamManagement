using Google.Protobuf.Collections;
using Grpc.Protos;

namespace ExamManagement.Server.Services.Abstraction;

public interface IExamFormService
{
    Task<PreFillFormResultMessage> PreFillFormAsync(string appUserId);
    Task<FillFormResultMessage> FillFormAsync(FillFormMessage model);
    Task<RepeatedField<FormResultMessage>> GetFormPerStudentAsync();
    Task<RepeatedField<FormResultMessage>> GetFormPerFacultyAsync();
}
