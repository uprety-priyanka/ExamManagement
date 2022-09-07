using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
using ExamManagement.Server.Services.Abstraction;
using Grpc.Protos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamManagement.Server.Services.Implementation;

public class ResultService : IResultService
{
    private readonly ApplicationContext _dbcontext;

    public ResultService(ApplicationContext dbcontext) 
    {
        _dbcontext = dbcontext;
    }

    public async Task<AddResultResultMessage> AddResultAsync(AddResultMessage model)
    {

        var result = await _dbcontext.Result.AddAsync(new Result
        {
            UserDetailExtensionStudentTemporaryId = model.UserDetailExtensionStudentTemporaryId,
        });
        await _dbcontext.SaveChangesAsync();

        var resultextension = await _dbcontext.ResultExtension.AddAsync(new ResultExtension
        {
            ResultId = result.Entity.Id,
            CourseId = model.CourseId,
            Grade = model.Grade,
        });

        var count = await _dbcontext.SaveChangesAsync();

        if (count > 0)
        {
            return new AddResultResultMessage
            {
                Success = true,
                Message = $"Result was successfully added."
            };
        }
        else
        {
            return new AddResultResultMessage
            {
                Success = false,
                Message = $"Result could not be added due to internal reasons."
            };
        }
    }

    public async Task<DeleteResultResultMessage> DeleteResultAsync(DeleteResultMessage model)
    {
        var result = await _dbcontext.Result.Where(x=>x.UserDetailExtensionStudentTemporaryId == model.UserDetailExtensionStudentTemporaryId).ToListAsync();

        var count = 0;

        foreach (var item in result) 
        {
            var delete = await _dbcontext.Result
                .Where(x => x.UserDetailExtensionStudentTemporaryId == item.UserDetailExtensionStudentTemporaryId)
                .FirstOrDefaultAsync();

            var deleteResult = _dbcontext.Result.Remove(delete);
            await _dbcontext.SaveChangesAsync();

            count++;
        }

        if (count > 0)
        {
            return new DeleteResultResultMessage
            {
                Success = true,
                Message = "Result has been deleted."
            };
        }
        else 
        {
            return new DeleteResultResultMessage 
            {
                Success = false,
                Message = "Could not delete record due to internal reasons."
            };
        }
    }

    public async Task<List<List<ResultExtension>>> GetResultPerFacultyAsync(GetResultPerFacultyMessage model)
    {

        var getUserDetailExtensionIds = await _dbcontext.UserDetailExtensionStudentTemporary
            .Include(x => x.UserDetailExtension)

            .Include(x => x.UserDetailExtension.UserDetail)
            .Include(x => x.UserDetailExtension.UserDetail.ApplicationUser)
            .Where(x => x.UserDetailExtension.UserDetail.FacultyId == model.FacultyId)
            .Where(x => x.UserDetailExtension.UserDetail.ApplicationUser.FirstName.ToUpper().Contains(model.Search.ToUpper()) ||
                        x.UserDetailExtension.UserDetail.ApplicationUser.LastName.ToUpper().Contains(model.Search.ToUpper()) ||
                        x.UserDetailExtension.RollNumber.ToString().ToUpper().Contains(model.Search.ToUpper())||
                        x.UserDetailExtension.RegistrationNumber.ToUpper().Contains(model.Search.ToUpper()) ||
                        x.UserDetailExtension.Batch.ToString().ToUpper().Contains(model.Search.ToUpper()) ||
                        x.UserDetailExtension.ExamNumber.ToUpper().Contains(model.Search.ToUpper()))
            .Select(x => x.UserDetailExtensionId)
            .Distinct()
            .ToListAsync();

        var list = new List<List<ResultExtension>>();

        foreach (var userDetailExtensionId in getUserDetailExtensionIds)
        {
            var getFacultyStudents = await _dbcontext.UserDetailExtensionStudentTemporary
            .Include(x => x.UserDetailExtension)
            .Include(x => x.UserDetailExtension.UserDetail)
            .Where(x => x.UserDetailExtension.UserDetail.FacultyId == model.FacultyId)
            .Where(x=>x.UserDetailExtensionId == userDetailExtensionId)
            .Select(x => x.Id)
            .ToListAsync();
            var sList = new List<ResultExtension>();

            foreach (var item in getFacultyStudents)
            {
                var result = await _dbcontext.ResultExtension
                    .Include(x => x.Result)
                    .Include(x => x.Result.UserDetailExtensionStudentTemporary)
                    .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension)
                    .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail)
                    .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser)
                    .Include(x => x.Course)
                    .Where(x => x.Result.UserDetailExtensionStudentTemporaryId == item)
                    .OrderBy(x => x.Result.UserDetailExtensionStudentTemporaryId)
                    .OrderBy(x => x.Result.UserDetailExtensionStudentTemporary.Semester)
                    .ToListAsync();
                sList.AddRange(result);
            }
            list.Add(sList);
        }
        
        return list;

    }

    public async Task<List<ResultExtension>> GetResultPerStudentAsync(GetResultPerStudentMessage model)
    {
            var result = await _dbcontext.ResultExtension
                .Include(x => x.Result)
                .Include(x => x.Result.UserDetailExtensionStudentTemporary)
                .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension)
                .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail)
                .Include(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser)
                .Include(x => x.Course)
                .Where(x => x.Result.UserDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUserId == model.ApplicationUserId)
                .OrderBy(x => x.Result.UserDetailExtensionStudentTemporaryId)
                .OrderBy(x => x.Result.UserDetailExtensionStudentTemporary.Semester)
                .ToListAsync();

        return result;

        }

    public async Task<UpdateResultResultMessage> UpdateResultAsync(UpdateResultMessage model)
    {
        var checkResult = await _dbcontext.Result.FindAsync(model.ResultId);
        if (checkResult is null)
        {
            return new UpdateResultResultMessage
            {
                Success = false,
                Message = "No such record exists."
            };
        }
        else 
        {
            var updateResult = _dbcontext.ResultExtension.Update(new ResultExtension 
            {
                Id = model.ResultExtensionId,
                CourseId = model.CourseId,
                ResultId = model.ResultId
            });

            var count = await _dbcontext.SaveChangesAsync();

            if (count > 0)
            {
                return new UpdateResultResultMessage
                {
                    Success = true,
                    Message = "Result was successfully updated."
                };
            }
            else 
            {
                return new UpdateResultResultMessage
                {
                    Success = false,
                    Message = "Could not update record due to internal reasons."
                };
            }
        }
    }
}
