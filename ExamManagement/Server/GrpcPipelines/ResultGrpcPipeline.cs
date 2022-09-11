using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;

namespace ExamManagement.Server.GrpcPipelines;

public class ResultGrpcPipeline:ResultGrpcService.ResultGrpcServiceBase
{
    private readonly IResultService _resultService;

    public ResultGrpcPipeline(IResultService resultService) 
    {
        _resultService = resultService;
    }

    public override async Task<AddResultResultMessage> AddResult(AddResultMessage request, ServerCallContext context)
    {

        var result = await _resultService.AddResultAsync(request);
        return result;

    }

    public override async Task<DeleteResultResultMessage> DeleteResult(DeleteResultMessage request, ServerCallContext context)
    {
        var result = await _resultService.DeleteResultAsync(request);
        return result;
    }

    public override async Task<UpdateResultResultMessage> UpdateResult(UpdateResultMessage request, ServerCallContext context)
    {
        var result = await _resultService.UpdateResultAsync(request);
        return result;
    }


    public override async Task GetResultPerFaculty(GetResultPerFacultyMessage request, IServerStreamWriter<GetResultPerFacultyResultMessage> responseStream, ServerCallContext context)
    {
        var result = await _resultService.GetResultPerFacultyAsync(request);

        foreach (var items in result)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;


            var supportMessage = new RepeatedField<GetResultPerFacultyResultSupportMessage>();

            var message = new GetResultPerFacultyResultMessage();

            foreach (var item in items) 
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;


                message.SGPA = 3.23;
                message.Batch = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.Batch;
                message.RollNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.RollNumber;
                message.ExamNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.ExamNumber;
                message.RegistrationNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.RegistrationNumber;
                message.ExamYear = item.Result.UserDetailExtensionStudentTemporary.ExamYear;
                message.FirstName = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.FirstName;
                message.LastName = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.LastName;
                message.Batch = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.Batch;
                message.UserDetailExtensionStudentTemporaryId = item.Result.UserDetailExtensionStudentTemporaryId;

                supportMessage.Add(new GetResultPerFacultyResultSupportMessage 
                {
                    Grade = item.Grade,
                    CourseName = item.Course.CourseName,
                    Semester = item.Course.SemesterTypeId,
                    CourseId = item.CourseId
                });
            }


            message.GetResultPerFacultyResultSupportMessages.AddRange(supportMessage);

            await responseStream.WriteAsync(message);

        }
    }

    public override async Task<GetResultPerStudentResultMessage> GetResultPerStudent(GetResultPerStudentMessage request, ServerCallContext context)
    {
        var message = new GetResultPerStudentResultMessage();

        var StudentResult = await _resultService.GetResultPerStudentAsync(request);

        foreach (var item in StudentResult) 
        {
            message.RollNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.RollNumber;
            message.ExamNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.ExamNumber;
            message.RegistrationNumber = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.RegistrationNumber;
            message.Batch = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.Batch;
            message.FirstName = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.FirstName;
            message.LastName = item.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.LastName;

            message.GetResultPerStudentResultSupportMessages.Add(new GetResultPerStudentResultSupportMessage 
            {
                CourseName = item.Course.CourseName,
                Grade = item.Grade,
                Semester = item.Course.SemesterTypeId,
                CourseId = item.CourseId
            });
        }

        return message;



    }

}
