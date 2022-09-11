using ExamManagement.Server.Data;
using ExamManagement.Server.Services.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;

namespace ExamManagement.Server.GrpcPipelines;

public class AccountGrpcPipeline:AccountGrpcService.AccountGrpcServiceBase
{
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountGrpcPipeline(IAccountService accountService, UserManager<ApplicationUser> userManager) 
    {
        _accountService = accountService;
        _userManager = userManager;
    }


    public override async Task<CurrentUserMessage> GetCurrentUser(Empty request, ServerCallContext context)
    {
        var currentUserResult = await _accountService.GetCurrentUserAsync();
        return currentUserResult;
    }

    public override async Task<LoginUserResultMessage> LoginUser(LoginUserMessage request, ServerCallContext context)
    {
        var loginResult = await _accountService.LoginUserAsync(request);
        return loginResult;
    }

    public override async Task<RegisterSuperAdminResultMessage> RegisterSuperAdmin(RegisterSuperAdminMessage request, ServerCallContext context)
    {
        var registerSuperAdminResult = await _accountService.RegisterSuperAdminAsync(request);
        return registerSuperAdminResult;
    }

    public override async Task<RegisterDepartmentAdminResultMessage> RegisterDepartmentAdmin(RegisterDepartmentAdminMessage request, ServerCallContext context)
    {
        var registerDepartmentAdminResult = await _accountService.RegisterDepartmentAdminAsync(request);
        return registerDepartmentAdminResult;
    }

    public override async Task<CheckEmailResultMessage> CheckEmail(CheckEmailMessage request, ServerCallContext context)
    {
        var checkEmailResult = await _accountService.CheckEmailAsync(request);
        return checkEmailResult;
    }

    public override async Task<CheckUserNameResultMessage> CheckUserName(CheckUserNameMessage request, ServerCallContext context)
    {
        var checkUserNameResult = await _accountService.CheckUserNameAsync(request);
        return checkUserNameResult;
    }

    public override async Task<RegisterStudentResultMessage> RegisterStudent(RegisterStudentMessage request, ServerCallContext context)
    {
        var registerStudentResult = await _accountService.RegisterStudentAsync(request);
        return registerStudentResult;
    }

    public override async Task<Empty> LogOutUser(Empty request, ServerCallContext context)
    {
        await _accountService.LogOutUserAsync();
        return new Empty();
    }

    public override async Task GetDepartmentUser(UserSearchMessage request, IServerStreamWriter<DepartmentUserMessage> responseStream, ServerCallContext context)
    {
        var list = await _accountService.GetAllDepartmentAdminAsync(request);

        foreach (var item in list) 
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            await responseStream.WriteAsync(new DepartmentUserMessage 
            {
                Id = item.Id,
                UserName = item.UserName,
                GivenName = item.GivenName,
                SurName = item.SurName,
                EmailAddress = item.Email,
                Department = item.Department
            });

        }
    }

    public override async Task<UserDeleteResultMessage> DeleteDepartmentAdminUser(DepartmentAdminIdMessage request, ServerCallContext context)
    {
        var result = await _accountService.DeleteUserAsync(request);
        return result;
    }


    public override async Task GetStudentByBatch(FacultySearchMessage request, IServerStreamWriter<StudentMessage> responseStream, ServerCallContext context)
    {
        var list = await _accountService.GetStudentByBatchAsync(request);

        foreach (var item in list)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            await responseStream.WriteAsync(new StudentMessage 
            {
                Id=item.Id,
                Semester = item.Semester,
                ApplicationUserId = item.UserDetailExtension.UserDetail.ApplicationUserId,
                EmailAddress = item.UserDetailExtension.UserDetail.ApplicationUser.Email,
                GivenName = item.UserDetailExtension.UserDetail.ApplicationUser.FirstName,
                SurName = item.UserDetailExtension.UserDetail.ApplicationUser.LastName,
                Batch = item.UserDetailExtension.Batch,
                ExamNumber = item.UserDetailExtension.ExamNumber,
                ExamYear = item.ExamYear,
                RegistrationNumber = item.UserDetailExtension.RegistrationNumber,
                RollNumber = item.UserDetailExtension.RollNumber
            });

        }

    }

    public override async Task<DeleteStudentResultMessage> DeleteStudent(StudentIdMessage request, ServerCallContext context)
    {
        var result = await _accountService.DeleteStudentAsync(request);
        return result;
    }

    public override async Task<UpgradeStudentResultmessage> UpgradeStudentSemester(IAsyncStreamReader<UpgradeStudentmessage> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext()) 
        {
            var result = await _accountService.UpgradeSemesterAsync(new UpgradeStudentmessage 
            {
                Semester = requestStream.Current.Semester,
                UserDetailExtensionTemporayId = requestStream.Current.UserDetailExtensionTemporayId
            });
        }

        return new UpgradeStudentResultmessage
        {
            Success = true,
            Message = "Student has been upgraded."
        };
    }

    public override async Task GetStudentInFaculty(AccFacultyIdMessage request, IServerStreamWriter<userDetailExtensionTemporayIdAndNameMessage> responseStream, ServerCallContext context)
    {
        var list = await _accountService.GetStudentInFacultyAsync(request);

        foreach (var item in list)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            await responseStream.WriteAsync(new userDetailExtensionTemporayIdAndNameMessage
            {
                UserDetailExtensionSudentTemporaryId = item.Id,
                FirstName = item.UserDetailExtension.UserDetail.ApplicationUser.FirstName,
                LastName = item.UserDetailExtension.UserDetail.ApplicationUser.LastName
            });
        }
    }

    public override async Task<CurrentStudentMessage> GetCurrentStudent(Empty request, ServerCallContext context)
    {
        var result = await _accountService.GetCurrentStudentAsync();
        return result;
    }

    public override async Task<Empty> SendResetPasswordEmail(CheckEmailAddressMessage request, ServerCallContext context)
    {
        try
        {

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (resetPasswordToken is not null)
                {
                    var queryString = new Dictionary<string, string>
                        {
                            { "uid", user.Id },
                            { "token", resetPasswordToken }
                        };

                    var resetPasswordLink = QueryHelpers.AddQueryString($"{context.GetHttpContext().Request.Scheme}://{context.Host}/resetpassword", queryString);

                    using SmtpClient client = new();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("uprety_priyanka@hotmail.com", "tricky32");
                    client.Port = 587; // 25 587
                    client.Host = "smtp.live.com";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    using MailMessage mail = new();
                    mail.From = new MailAddress("uprety_priyanka@hotmail.com", "Examination Form Management System");
                    mail.To.Add(new MailAddress(request.Email));
                    mail.Subject = "Reset your password";
                    mail.Body = resetPasswordLink;

                    await client.SendMailAsync(mail);

                }
                else
                {
                    return new Empty();
                }
            }
            else
            {
                return new Empty();
            }
        }

        catch (Exception ex)
        {
            throw new Exception("", ex);
        }
        return new Empty();
    }

    public override async Task<ResetPasswordResultMessage> ResetUserPassword(ResetPasswordMessage request, ServerCallContext context)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return new ResetPasswordResultMessage
            {
                Success = false,
                PasswordResetErrorType = ResetPasswordResultMessage.Types.ResetPasswordErrorType.NoSuchUserFound
            };
        }
        var validate = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", request.ResetPasswordToken);
        if (!validate)
        {
            return new ResetPasswordResultMessage
            {
                Success = false,
                PasswordResetErrorType = ResetPasswordResultMessage.Types.ResetPasswordErrorType.InvalidToken
            };
        }
        var passwordResetResult = await _userManager.ResetPasswordAsync(user, request.ResetPasswordToken, request.Password);
        if (passwordResetResult.Succeeded)
        {
            return new ResetPasswordResultMessage
            {
                Success = true,
                PasswordResetErrorType = ResetPasswordResultMessage.Types.ResetPasswordErrorType.NoAnyError
            };
        }
        else
        {
            return new ResetPasswordResultMessage
            {
                Success = false,
                PasswordResetErrorType = ResetPasswordResultMessage.Types.ResetPasswordErrorType.CouldNotResetPassword
            };
        }
    }

} 
