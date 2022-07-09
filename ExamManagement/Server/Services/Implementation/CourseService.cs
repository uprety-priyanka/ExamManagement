using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
using ExamManagement.Server.Services.Abstraction;
using Grpc.Protos;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Server.Services.Implementation
{
    public class CourseService : ICourseService
    {

        public readonly ApplicationContext _dbContext;

        public CourseService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AddCourseResultMessage> AddCourseAsync(AddCourseMessage addCourseMessage)
        {
            var result = await _dbContext.Course.AddAsync(new Course 
            {
                FacultyId = addCourseMessage.FacultyId,
                SemesterTypeId = addCourseMessage.SemesterId,
                CourseCode = addCourseMessage.CourseCode,
                CourseName = addCourseMessage.CourseName,
                ConcurrentRegistrationCourse = addCourseMessage.ConcurrentRegistrationCourse,
                PreRequisiteCourse = addCourseMessage.PreRequisiteCourse,
                Credit = addCourseMessage.Credit,
                Lecture = addCourseMessage.Lecture,
                Practical = addCourseMessage.Practical,
                Tutorial = addCourseMessage.Tutorial,
                CreatedDate = DateTime.UtcNow
            });

            var count = await _dbContext.SaveChangesAsync();
            if (count > 0) 
            {
                return new AddCourseResultMessage 
                {
                    Success = true,
                    Message = "Course has been added."
                };
            }
            else
            {
                return new AddCourseResultMessage
                {
                    Success = false,
                    Message = "Course could not be added due to internal reasons."
                };
            }
        }

        public async Task<DeleteCourseResultMessage> DeleteCourseAsync(DeleteCourseIdMessage deleteCourseIdMessage)
        {
            var course = await _dbContext.Course.FindAsync(deleteCourseIdMessage.Id);
            if (course == null) 
            {
                return new DeleteCourseResultMessage 
                {
                    Success = false,
                    Message = "No such record exists."
                };
            }
            else
            {
                var result = _dbContext.Course.Remove(course);
                var count = await _dbContext.SaveChangesAsync();
                if (count > 0)
                {
                    return new DeleteCourseResultMessage
                    {
                        Success = true,
                        Message = "Course has been deleted."
                    };
                }
                else
                {
                    return new DeleteCourseResultMessage
                    {
                        Success = false,
                        Message = "Course could not br deleted due to internal reasons."
                    };
                }
            }
        }

        public async Task<List<Course>> GetCourseByFacultyIdAsync(FacultyIdMessage facultyId)
        {
            var course = await _dbContext.Course
                .Include(x => x.SemesterType)
                .Include(x => x.Faculty)
                .Where(x => x.FacultyId == facultyId.FacultyId)
                .ToListAsync();

            return course;

        }

        public async Task<UpdateCourseResultMessage> UpdateCourseAsync(UpdateCourseMessage updateCourseMessage)
        {
            var findCourse = await _dbContext.Course.AsNoTracking().Where(x=>x.Id == updateCourseMessage.Id).FirstOrDefaultAsync();
            if (findCourse == null)
            {
                return new UpdateCourseResultMessage
                {
                    Success = false,
                    Message = "No such record exists."
                };
            }
            else 
            {
                var result = _dbContext.Course.Update(new Course 
                {
                    Id = updateCourseMessage.Id,
                    SemesterTypeId = updateCourseMessage.SemesterId,
                    ConcurrentRegistrationCourse = updateCourseMessage.ConcurrentRegistrationCourse,
                    CourseCode = updateCourseMessage.CourseCode,
                    CourseName = updateCourseMessage.CourseName,
                    Credit = updateCourseMessage.Credit,
                    Tutorial = updateCourseMessage.Tutorial,
                    FacultyId = updateCourseMessage.FacultyId,
                    Lecture = updateCourseMessage.Lecture,
                    Practical = updateCourseMessage.Practical,
                    PreRequisiteCourse = updateCourseMessage.PreRequisiteCourse,
                });
                var count = await _dbContext.SaveChangesAsync();

                if (count > 0)
                {
                    return new UpdateCourseResultMessage
                    {
                        Success = true,
                        Message = "Course has been updated."
                    };
                }
                else 
                {
                    return new UpdateCourseResultMessage
                    {
                        Success = false,
                        Message = "Could not update course due to internal reasons."
                    };
                }
            }
        }
    }
}
