using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Protos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamManagement.Server.Services.Implementation
{
    public class FacultyService : IFacultyService
    {

        public readonly ApplicationContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyService(ApplicationContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<AddFacultyResultMessage> AddFacultyAsync(AddFacultyMessage addFacultyMessage)
        {
            var result = await _dbContext.Faculty.AddAsync(new Faculty 
            {
                FacultyName = addFacultyMessage.FacultyName,
                CreatedDate = DateTime.UtcNow
            });

            var count = await _dbContext.SaveChangesAsync();

            if (count > 0)
            {
                return new AddFacultyResultMessage
                {
                    Success = true,
                    Message = "Facult was added."
                };
            }
            else 
            {
                return new AddFacultyResultMessage
                {
                    Success = false,
                    Message = "Couldn't be added due to internal reasons."
                };
            }

        }

        public async Task<DeleteFacultyResultMessage> DeleteFacultyAsync(IdMessage idMessage)
        {
            var find = await _dbContext.Faculty.FindAsync(idMessage.Id);
            if (find == null) 
            {
                return new DeleteFacultyResultMessage
                {
                    Success = false,
                    Message = "Such Record do not exists."
                };
            }
            var deleteResult = _dbContext.Faculty.Remove(find);
            var count = await _dbContext.SaveChangesAsync();
            if (count > 0)
            {
                return new DeleteFacultyResultMessage
                {
                    Success = true,
                    Message = "Record is successfully deleted."
                };
            }
            else 
            {
                return new DeleteFacultyResultMessage
                {
                    Success = false,
                    Message = "Couldn't be deleted due to internal reasons"
                };
            }

        }

        public async Task<List<Faculty>> GetAllFacultyAsync(SearchMessage searchMessage)
        {
            if (string.IsNullOrEmpty(searchMessage.Search))
            {
                var result = await _dbContext.Faculty.ToListAsync();
                return result;
            }
            else 
            {
                var result = await _dbContext.Faculty.Where(x => x.FacultyName.ToLower().Contains(searchMessage.Search.ToLower().Trim())).ToListAsync();
                return result;
            }
        }

        public async Task<FacultyMessage> GetFacultyByIdAsync(IdMessage idMessage)
        {
            var result = await _dbContext.Faculty.Where(x => x.Id == idMessage.Id).FirstOrDefaultAsync();
            return new FacultyMessage 
            {
                Id = result.Id,
                FacultyName = result.FacultyName,
                CreatedDate = result.CreatedDate.ToUniversalTime().ToTimestamp()
            };
        }

        public async Task<IdMessage> GetFacultyIdForDepartmentUserAsync(ClaimsPrincipal user)
        {
            var findUer = await _userManager.FindByNameAsync(user?.Identity?.Name);
            var facultyId = await _dbContext.UserDetail
                .Where(x => x.ApplicationUserId == findUer.Id)
                .Select(x => x.FacultyId)
                .FirstOrDefaultAsync();

            return new IdMessage
            {
                Id = facultyId
            };
        }

        public async Task<UpdateFacultyResultMessage> UpdateFacultyAsync(UpdateFacultyMessage updateFacultyMessage)
        {
            var getFaculty = await _dbContext.Faculty.FindAsync(updateFacultyMessage.Id);

            if (getFaculty == null) 
            {
                return new UpdateFacultyResultMessage 
                {
                    Success = false,
                    Message = "Such record do not exists."
                };
            }

            getFaculty.FacultyName = updateFacultyMessage.FacultyName;
            var result = _dbContext.Faculty.Update(getFaculty);
            var count = await _dbContext.SaveChangesAsync();

            if (count > 0)
            {
                return new UpdateFacultyResultMessage
                {
                    Success = true,
                    Message = "Record has been updated."
                };
            }
            else 
            {
                return new UpdateFacultyResultMessage
                {
                    Success = false,
                    Message = "Couldn't be deleted due to internal reasons"
                };
            }

        }
    }
}
