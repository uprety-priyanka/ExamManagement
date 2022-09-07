using ExamManagement.Server.Data;
using ExamManagement.Server.Services.Abstraction;
using ExamManagement.Shared.ItemTypeConstant;
using Google.Protobuf.Collections;
using Grpc.Protos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Server.Services.Implementation;

public class ExamFormService:IExamFormService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ApplicationContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IResultService _resultService;

    public ExamFormService(IHttpContextAccessor accessor,
                            ApplicationContext dbContext,
                            UserManager<ApplicationUser> userManager,
                            IResultService resultService,
                            ICourseService courseService) 
    {
        _accessor = accessor;
        _dbContext = dbContext;
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<PreFillFormResultMessage> PreFillFormAsync(string appUserId)
    {


        var getCurrentSemesterandFaculty = await _dbContext.UserDetailExtensionStudentTemporary
                                        .Include(x => x.UserDetailExtension)
                                        .Include(x => x.UserDetailExtension.UserDetail)
                                        .Include(x => x.UserDetailExtension.UserDetail.ApplicationUser)
                                        .Where(x => x.UserDetailExtension.UserDetail.ApplicationUser.Id == appUserId)
                                        .OrderByDescending(x => x.Semester)
                                        .Select(x => new { x.Semester, x.UserDetailExtension.UserDetail.FacultyId })
                                        .FirstOrDefaultAsync();

        var preFillFormResultMessage = new PreFillFormResultMessage();

        var preFillRegularFormResultSupportMessageRepeatedField = new RepeatedField<PreFillRegularFormResultSupportMessage>();
        var preFillConcurrentFormResultSupportMessageRepeatedField = new RepeatedField<PreFillConcurrentFormResultSupportMessage>();
        var preFillPrerequisiteFormResultSupportMessageRepeatedField = new RepeatedField<PreFillPrerequisiteFormResultSupportMessage>();
        var preFillBackFormResultSupportMessageRepeatedField = new RepeatedField<PreFillBackFormResultSupportMessage>();

        switch (getCurrentSemesterandFaculty.Semester) 
        {
            case 1:

                var courses = await _dbContext.Course
                                .Where(x => x.FacultyId == getCurrentSemesterandFaculty.FacultyId)
                                .Where(x => x.SemesterTypeId == SemesterTypeConstant.SEMESTERONE)
                                .ToListAsync();

                foreach (var course in courses) 
                {
                    preFillRegularFormResultSupportMessageRepeatedField.Add(new PreFillRegularFormResultSupportMessage 
                    {
                        CourseId = course.Id,
                        CourseName = course.CourseName
                    });
                }

                preFillFormResultMessage.PreFillRegularFormResultSupportMessages.AddRange(preFillRegularFormResultSupportMessageRepeatedField);
                preFillFormResultMessage.PreFillConcurrentFormResultSupportMessages.AddRange(preFillConcurrentFormResultSupportMessageRepeatedField);
                preFillFormResultMessage.PreFillPrerequisiteFormResultSupportMessages.AddRange(preFillPrerequisiteFormResultSupportMessageRepeatedField);
                preFillFormResultMessage.PreFillBackFormResultSupportMessages.AddRange(preFillBackFormResultSupportMessageRepeatedField);

                return preFillFormResultMessage;

            case 2:

                var coursesI = await _dbContext.Course
                                .Where(x => x.FacultyId == getCurrentSemesterandFaculty.FacultyId)
                                .Where(x => x.SemesterTypeId == SemesterTypeConstant.SEMESTERTWO)
                                .ToListAsync();

                foreach (var course in coursesI)
                {
                    preFillRegularFormResultSupportMessageRepeatedField.Add(new PreFillRegularFormResultSupportMessage
                    {
                        CourseId = course.Id,
                        CourseName = course.CourseName
                    });
                }

                preFillFormResultMessage.PreFillRegularFormResultSupportMessages.AddRange(preFillRegularFormResultSupportMessageRepeatedField);

                var result = await _resultService.GetResultPerStudentAsync(new GetResultPerStudentMessage 
                {
                    ApplicationUserId = appUserId
                });

                foreach (var item in result) 
                {
                    if (item.Grade.ToUpper() == "F") 
                    {

                        var checkCourses = await _dbContext.Course
                               .Where(x => x.FacultyId == getCurrentSemesterandFaculty.FacultyId)
                               .ToListAsync();

                        var isConcurrent = checkCourses.Where(x => x.ConcurrentRegistrationCourse == item.Course.CourseCode).Any();

                        if (isConcurrent)
                        {
                            preFillConcurrentFormResultSupportMessageRepeatedField.Add(new PreFillConcurrentFormResultSupportMessage
                            {
                                CourseId = item.CourseId,
                                CourseName = item.Course.CourseName
                            });
                        }
                        else 
                        {
                            preFillBackFormResultSupportMessageRepeatedField.Add(new PreFillBackFormResultSupportMessage
                            {
                                CourseId = item.CourseId,
                                CourseName = item.Course.CourseName
                            });
                        }
                    }
                }

                preFillFormResultMessage.PreFillConcurrentFormResultSupportMessages.AddRange(preFillConcurrentFormResultSupportMessageRepeatedField);
                preFillFormResultMessage.PreFillPrerequisiteFormResultSupportMessages.AddRange(preFillPrerequisiteFormResultSupportMessageRepeatedField);
                preFillFormResultMessage.PreFillBackFormResultSupportMessages.AddRange(preFillBackFormResultSupportMessageRepeatedField);

                return preFillFormResultMessage;

            case 3:

                var case3Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 3, getCurrentSemesterandFaculty.FacultyId);
                return case3Result;

            case 4:

                var case4Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 4, getCurrentSemesterandFaculty.FacultyId);
                return case4Result;

            case 5:

                var case5Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 5, getCurrentSemesterandFaculty.FacultyId);
                return case5Result;
                ;

            case 6:

                var case6Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 6, getCurrentSemesterandFaculty.FacultyId);
                return case6Result;

            case 7:
                var case7Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 7, getCurrentSemesterandFaculty.FacultyId);
                return case7Result;

            case 8:
                var case8Result = await ValidateRulesForConcurrentAndPrerequisite(appUserId, 8, getCurrentSemesterandFaculty.FacultyId);
                return case8Result;

            default:
                return null;
        }
    }

    public async Task<PreFillFormResultMessage> ValidateRulesForConcurrentAndPrerequisite(string appUserId, int semester, int facultyId) 
    {


        var preFillFormResultMessage = new PreFillFormResultMessage();

        var preFillRegularFormResultSupportMessageRepeatedField = new RepeatedField<PreFillRegularFormResultSupportMessage>();
        var preFillConcurrentFormResultSupportMessageRepeatedField = new RepeatedField<PreFillConcurrentFormResultSupportMessage>();
        var preFillPrerequisiteFormResultSupportMessageRepeatedField = new RepeatedField<PreFillPrerequisiteFormResultSupportMessage>();
        var preFillBackFormResultSupportMessageRepeatedField = new RepeatedField<PreFillBackFormResultSupportMessage>();

        var coursesII = await _dbContext.Course
                                .Where(x => x.FacultyId == facultyId)
                                .Where(x => x.SemesterTypeId == semester)
                                .ToListAsync();

        foreach (var course in coursesII)
        {
            preFillRegularFormResultSupportMessageRepeatedField.Add(new PreFillRegularFormResultSupportMessage
            {
                CourseId = course.Id,
                CourseName = course.CourseName
            });
        }

        preFillFormResultMessage.PreFillRegularFormResultSupportMessages.AddRange(preFillRegularFormResultSupportMessageRepeatedField);

        var resultI = await _resultService.GetResultPerStudentAsync(new GetResultPerStudentMessage
        {
            ApplicationUserId = appUserId
        });

        foreach (var item in resultI)
        {
            if (item.Grade.ToUpper() == "F")
            {

                var checkCourses = await _dbContext.Course
                       .Where(x => x.FacultyId == facultyId)
                       .ToListAsync();

                var isConcurrent = checkCourses.Where(x => x.ConcurrentRegistrationCourse == item.Course.CourseCode).Any();

                if (isConcurrent)
                {
                    preFillConcurrentFormResultSupportMessageRepeatedField.Add(new PreFillConcurrentFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }
                else
                {
                    preFillBackFormResultSupportMessageRepeatedField.Add(new PreFillBackFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }
            }
        }

        preFillFormResultMessage.PreFillConcurrentFormResultSupportMessages.AddRange(preFillConcurrentFormResultSupportMessageRepeatedField);

        foreach (var item in resultI)
        {
            if (item.Grade.ToUpper() == "F")
            {

                var checkCourses = await _dbContext.Course
                       .Where(x => x.FacultyId == facultyId)
                       .ToListAsync();

                var isPrerequisite = checkCourses.Where(x => x.PreRequisiteCourse == item.Course.CourseCode).Any();

                if (isPrerequisite)
                {
                    preFillPrerequisiteFormResultSupportMessageRepeatedField.Add(new PreFillPrerequisiteFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }
                else
                {
                    preFillBackFormResultSupportMessageRepeatedField.Add(new PreFillBackFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }

            }
        }

        preFillFormResultMessage.PreFillPrerequisiteFormResultSupportMessages.AddRange(preFillPrerequisiteFormResultSupportMessageRepeatedField);
        preFillFormResultMessage.PreFillBackFormResultSupportMessages.AddRange(preFillBackFormResultSupportMessageRepeatedField);

        return preFillFormResultMessage;
    }
}
