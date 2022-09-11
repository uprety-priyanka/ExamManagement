using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
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

    public async Task<FillFormResultMessage> FillFormAsync(FillFormMessage model)
    {

        var name = _accessor?.HttpContext?.User?.Identity?.Name;

        var applicationUser = await _userManager.FindByNameAsync(name);

        var userDetailExtensionStudentTemporaryId = await _dbContext.UserDetailExtensionStudentTemporary
                                                                    .Include(x => x.UserDetailExtension)
                                                                    .Include(x => x.UserDetailExtension.UserDetail)
                                                                    .Include(x => x.UserDetailExtension.UserDetail.ApplicationUser)
                                                                    .Where(x => x.UserDetailExtension.UserDetail.ApplicationUserId == applicationUser.Id)
                                                                    .Select(x => x.Id)
                                                                    .FirstOrDefaultAsync();

        var formResult = await _dbContext.ExamForm.AddAsync(new ExamForm 
        {
            UserDetailExtensionStudentTemporaryId = userDetailExtensionStudentTemporaryId,
            FormFilledDate = DateTime.UtcNow,
        });

        var count = await _dbContext.SaveChangesAsync();

        foreach (var item in model.FillRegularFormSupportMessages) 
        {
            var regularResult = await _dbContext.ExamFormRegular.AddAsync(new ExamFormRegular
            {
                CourseId = item.CourseId,
                ExamFormId = formResult.Entity.Id
            });

            await _dbContext.SaveChangesAsync();
        }

        foreach (var item in model.FillConcurrentFormSupportMessages) 
        {
            var concurrentResult = await _dbContext.ExamFormConcurrent.AddAsync(new ExamFormConcurrent 
            {
                CourseId = item.CourseId,
                ExamFormId = formResult.Entity.Id
            });

            await _dbContext.SaveChangesAsync();
        }

        foreach (var item in model.FillPrerequisiteFormSupportMessages)
        {
            var regularResult = await _dbContext.ExamFormPrerequisite.AddAsync(new ExamFormPrerequisite
            {
                CourseId = item.CourseId,
                ExamFormId = formResult.Entity.Id
            });

            await _dbContext.SaveChangesAsync();
        }

        foreach (var item in model.FillBackFormSupportMessages)
        {
            var regularResult = await _dbContext.ExamFormBack.AddAsync(new ExamFormBack
            {
                CourseId = item.CourseId,
                ExamFormId = formResult.Entity.Id
            });

            await _dbContext.SaveChangesAsync();
        }

        if (count > 0)
        {
            return new FillFormResultMessage
            {
                Success = true,
                Message = $"Form has been filled. Form Id: {formResult.Entity.Id}."
            };
        }
        else 
        {
            return new FillFormResultMessage
            {
                Success = true,
                Message = "Record cannot be added due to internal reasons."
            };
        }
    }

    public async Task<RepeatedField<FormResultMessage>> GetFormPerFacultyAsync()
    {

        var name = _accessor?.HttpContext?.User?.Identity?.Name;

        var applicationUser = await _userManager.FindByNameAsync(name);

        var getFaculltyId = await _dbContext.UserDetail
                                                .Include(x => x.ApplicationUser)
                                                .Where(x => applicationUser.Id == applicationUser.Id)
                                                .Select(x => x.FacultyId)
                                                .FirstOrDefaultAsync();

        var userDetailExtensionStudentTemporary = await _dbContext.UserDetailExtensionStudentTemporary
                                                                    .Include(x => x.UserDetailExtension)
                                                                    .Include(x => x.UserDetailExtension.UserDetail)
                                                                    .Include(x => x.UserDetailExtension.UserDetail.Faculty)
                                                                    .Include(x => x.UserDetailExtension.UserDetail.ApplicationUser)
                                                                    .Where(x => x.UserDetailExtension.UserDetail.FacultyId == getFaculltyId)
                                                                    .FirstOrDefaultAsync();

        var getForms = await _dbContext.ExamForm.Where(x => x.UserDetailExtensionStudentTemporaryId == userDetailExtensionStudentTemporary.Id).ToListAsync();


        var formList = new RepeatedField<FormResultMessage>();

        foreach (var item in getForms)
        {
            var regularForms = new RepeatedField<RegularFormResultSupportMessage>();
            var concurrentForms = new RepeatedField<ConcurrentFormResultSupportMessage>();
            var prerequisiteForms = new RepeatedField<PrerequisiteFormResultSupportMessage>();
            var backForms = new RepeatedField<BackFormResultSupportMessage>();

            var Regulars = await _dbContext.ExamFormRegular
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Concurrents = await _dbContext.ExamFormConcurrent
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Prerequisites = await _dbContext.ExamFormPrerequisite
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Backs = await _dbContext.ExamFormBack
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            foreach (var regular in Regulars)
            {
                regularForms.Add(new RegularFormResultSupportMessage
                {
                    CourseName = regular.Course.CourseName,
                    CourseCode = regular.Course.CourseCode,
                    Credit = regular.Course.Credit
                });
            }

            foreach (var concurrent in Concurrents)
            {
                concurrentForms.Add(new ConcurrentFormResultSupportMessage
                {
                    CourseName = concurrent.Course.CourseName,
                    CourseCode = concurrent.Course.CourseCode,
                    Credit = concurrent.Course.Credit
                });
            }

            foreach (var prerequisite in Prerequisites)
            {
                prerequisiteForms.Add(new PrerequisiteFormResultSupportMessage
                {
                    CourseName = prerequisite.Course.CourseName,
                    CourseCode = prerequisite.Course.CourseCode,
                    Credit = prerequisite.Course.Credit
                });
            }

            foreach (var back in Backs)
            {
                backForms.Add(new BackFormResultSupportMessage
                {
                    CourseName = back.Course.CourseName,
                    CourseCode = back.Course.CourseCode,
                    Credit = back.Course.Credit
                });
            }

            var frm = new FormResultMessage();

            frm.RegularFormResultSupportMessages.AddRange(regularForms);
            frm.BackFormResultSupportMessages.AddRange(backForms);
            frm.PrerequisiteFormResultSupportMessage.AddRange(prerequisiteForms);
            frm.ConcurrentFormResultSupportMessages.AddRange(concurrentForms);

            frm.FirstName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.FirstName;
            frm.LastName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.LastName;
            frm.ExamNumber = userDetailExtensionStudentTemporary.UserDetailExtension.ExamNumber;
            frm.RegistrationNumber = userDetailExtensionStudentTemporary.UserDetailExtension.RegistrationNumber;
            frm.FacultyName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.Faculty.FacultyName;
            frm.ExamYear = userDetailExtensionStudentTemporary.ExamYear;
            frm.FormId = item.Id;

            formList.Add(frm);

        }

        return formList;
    }

    public async Task<RepeatedField<FormResultMessage>> GetFormPerStudentAsync()
    {
        var name = _accessor?.HttpContext?.User?.Identity?.Name;

        var applicationUser = await _userManager.FindByNameAsync(name);

        var userDetailExtensionStudentTemporary = await _dbContext.UserDetailExtensionStudentTemporary
                                                                    .Include(x => x.UserDetailExtension)
                                                                    .Include(x => x.UserDetailExtension.UserDetail)
                                                                    .Include(x => x.UserDetailExtension.UserDetail.Faculty)
                                                                    .Include(x => x.UserDetailExtension.UserDetail.ApplicationUser)
                                                                    .Where(x => x.UserDetailExtension.UserDetail.ApplicationUserId == applicationUser.Id)
                                                                    .FirstOrDefaultAsync();
        var getForms = await _dbContext.ExamForm.Where(x => x.UserDetailExtensionStudentTemporaryId == userDetailExtensionStudentTemporary.Id).ToListAsync();


        var formList = new RepeatedField<FormResultMessage>();

        foreach (var item in getForms) 
        {
            var regularForms = new RepeatedField<RegularFormResultSupportMessage>();
            var concurrentForms = new RepeatedField<ConcurrentFormResultSupportMessage>();
            var prerequisiteForms = new RepeatedField<PrerequisiteFormResultSupportMessage>();
            var backForms = new RepeatedField<BackFormResultSupportMessage>();

            var Regulars = await _dbContext.ExamFormRegular
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Concurrents = await _dbContext.ExamFormConcurrent
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Prerequisites = await _dbContext.ExamFormPrerequisite
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            var Backs = await _dbContext.ExamFormBack
                .Include(x => x.Course)
                .Where(x => x.ExamFormId == item.Id).ToListAsync();

            foreach (var regular in Regulars) 
            {
                regularForms.Add(new RegularFormResultSupportMessage 
                {
                    CourseName = regular.Course.CourseName,
                    CourseCode = regular.Course.CourseCode,
                    Credit = regular.Course.Credit
                });
            }

            foreach (var concurrent in Concurrents) 
            {
                concurrentForms.Add(new ConcurrentFormResultSupportMessage 
                {
                    CourseName = concurrent.Course.CourseName,
                    CourseCode = concurrent.Course.CourseCode,
                    Credit = concurrent.Course.Credit
                });
            }

            foreach (var prerequisite in Prerequisites) 
            {
                prerequisiteForms.Add(new PrerequisiteFormResultSupportMessage 
                {
                    CourseName = prerequisite.Course.CourseName,
                    CourseCode = prerequisite.Course.CourseCode,
                    Credit = prerequisite.Course.Credit
                });
            }

            foreach (var back in Backs) 
            {
                backForms.Add(new BackFormResultSupportMessage 
                {
                    CourseName = back.Course.CourseName,
                    CourseCode = back.Course.CourseCode,
                    Credit = back.Course.Credit
                });
            }

            var frm = new FormResultMessage();

            frm.RegularFormResultSupportMessages.AddRange(regularForms);
            frm.BackFormResultSupportMessages.AddRange(backForms);
            frm.PrerequisiteFormResultSupportMessage.AddRange(prerequisiteForms);
            frm.ConcurrentFormResultSupportMessages.AddRange(concurrentForms);

            frm.FirstName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.FirstName;
            frm.LastName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.ApplicationUser.LastName;
            frm.ExamNumber = userDetailExtensionStudentTemporary.UserDetailExtension.ExamNumber;
            frm.RegistrationNumber = userDetailExtensionStudentTemporary.UserDetailExtension.RegistrationNumber;
            frm.FacultyName = userDetailExtensionStudentTemporary.UserDetailExtension.UserDetail.Faculty.FacultyName;
            frm.ExamYear = userDetailExtensionStudentTemporary.ExamYear;
            frm.FormId = item.Id;

            formList.Add(frm);

        }

        return formList;

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


        //Concurrent Courses
        var resultI = await _resultService.GetResultPerStudentAsync(new GetResultPerStudentMessage
        {
            ApplicationUserId = appUserId
        });

        foreach (var item in resultI)
        {
            if (item.Grade.ToUpper().Trim() == "F")
            {

                var checkCourses = await _dbContext.Course
                       .Where(x => x.FacultyId == facultyId)
                       .ToListAsync();

                var isConcurrent = checkCourses.Where(x => x.CourseCode == item.Course.ConcurrentRegistrationCourse).Any();

                if (isConcurrent)
                {
                    preFillConcurrentFormResultSupportMessageRepeatedField.Add(new PreFillConcurrentFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }
            }
        }

        preFillFormResultMessage.PreFillConcurrentFormResultSupportMessages.AddRange(preFillConcurrentFormResultSupportMessageRepeatedField);


        //Prerequisite

        foreach (var item in resultI)
        {
            if (item.Grade.ToUpper() == "F")
            {
                var checkCourses = await _dbContext.Course
                       .Where(x => x.FacultyId == facultyId)
                       .ToListAsync();

                var isPrerequisite = checkCourses.Where(x => x.ConcurrentRegistrationCourse == item.Course.CourseCode).Any();

                if (isPrerequisite)
                {
                    preFillPrerequisiteFormResultSupportMessageRepeatedField.Add(new PreFillPrerequisiteFormResultSupportMessage
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }
            }
        }

        preFillFormResultMessage.PreFillPrerequisiteFormResultSupportMessages.AddRange(preFillPrerequisiteFormResultSupportMessageRepeatedField);


        //Back

        foreach (var item in resultI) 
        {
            if (item.Grade.Trim().ToUpper() == "F") 
            {
                var checkCourses = await _dbContext.Course
                       .Where(x => x.FacultyId == facultyId)
                       .ToListAsync();

                var checkInConcurrent = preFillConcurrentFormResultSupportMessageRepeatedField.Where(x => x.CourseId == item.CourseId).Any();
                var checkInPrerequisite = preFillPrerequisiteFormResultSupportMessageRepeatedField.Where(x => x.CourseId == item.CourseId).Any();

                if (!checkInConcurrent && !checkInPrerequisite) 
                {
                    preFillBackFormResultSupportMessageRepeatedField.Add(new PreFillBackFormResultSupportMessage 
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.CourseName
                    });
                }

            }
        }
        preFillFormResultMessage.PreFillBackFormResultSupportMessages.AddRange(preFillBackFormResultSupportMessageRepeatedField);

        return preFillFormResultMessage;
    }
}
