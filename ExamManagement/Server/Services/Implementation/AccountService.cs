using ExamManagement.Server.Data;
using ExamManagement.Server.Entities;
using ExamManagement.Server.Services.Abstraction;
using ExamManagement.Shared.Constants;
using Grpc.Protos;
using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Server.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext _dbContext;

        public AccountService(IHttpContextAccessor accessor,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationContext dbContext) 
        {
            _accessor = accessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<CheckEmailResultMessage> CheckEmailAsync(CheckEmailMessage checkEmailMessage)
        {
            return new CheckEmailResultMessage 
            {
                Exists = _userManager.Users.Where(x => x.Email == checkEmailMessage.Email.Trim()).Any()
            };
        }

        public async Task<CheckUserNameResultMessage> CheckUserNameAsync(CheckUserNameMessage checkUserNameMessage)
        {
            return new CheckUserNameResultMessage
            {
                Exists = _userManager.Users.Where(x => x.UserName == checkUserNameMessage.UserName.Trim()).Any()
            };
        }

        public async Task<CurrentUserMessage> GetCurrentUserAsync()
        {
            if (_accessor.HttpContext.User.Identity.IsAuthenticated) 
            {
                var findUser = await _userManager.FindByNameAsync(_accessor.HttpContext.User.Identity.Name);
                var findRole = await _userManager.GetRolesAsync(findUser);

                return new CurrentUserMessage 
                {
                    GivenName = findUser.FirstName,
                    SurName = findUser.LastName,
                    EmailAddress = findUser.Email,
                    UserName = findUser.UserName,
                    Id = findUser.Id,
                    Role = findRole.First(),
                    IsSuccess = true,
                };
            }

            return new CurrentUserMessage
            {
                IsSuccess = false
            };
        }

        public async Task<LoginUserResultMessage> LoginUserAsync(LoginUserMessage loginUserMessage)
        {
            var applicationUser = new ApplicationUser();

            var findByEmail = await _userManager.FindByEmailAsync(loginUserMessage.Info);
            var findByUserName = await _userManager.FindByNameAsync(loginUserMessage.Info);

            if (findByEmail != null)
                applicationUser = findByEmail;
            if (findByUserName != null)
                applicationUser = findByUserName;
            if (findByUserName == null && findByEmail == null)
                return new LoginUserResultMessage
                {
                    Success = false,
                    LoginErrorType = LoginUserResultMessage.Types.LoginErrorTypes.NoSuchUser
                };


            var loginResult = await _signInManager.PasswordSignInAsync(applicationUser, loginUserMessage.Password, true, false);
            if (loginResult.Succeeded)
            {

                return new LoginUserResultMessage
                {
                    Success = true,
                    LoginErrorType = LoginUserResultMessage.Types.LoginErrorTypes.NoAnyError
                };
            }
            else
            {
                return new LoginUserResultMessage
                {
                    Success = false,
                    LoginErrorType = LoginUserResultMessage.Types.LoginErrorTypes.IncorrectPassword
                };
            }
        }

        public async Task LogOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterDepartmentAdminResultMessage> RegisterDepartmentAdminAsync(RegisterDepartmentAdminMessage registerDepartmentAdminMessage)
        {
            var checkIfRoleExists = await _roleManager.RoleExistsAsync(RoleConstant.DEPARTMENTADMIN);
            if (!checkIfRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = RoleConstant.DEPARTMENTADMIN
                });
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerDepartmentAdminMessage.GivenName,
                LastName = registerDepartmentAdminMessage.SurName,
                Email = registerDepartmentAdminMessage.Email,
                UserName = registerDepartmentAdminMessage.UserName,
            };

            var registerUser = await _userManager.CreateAsync(applicationUser, registerDepartmentAdminMessage.Password);

            if (registerUser.Succeeded)
            {

                await _userManager.AddToRoleAsync(applicationUser, RoleConstant.DEPARTMENTADMIN);

                var findUserByUsername = await _userManager.FindByNameAsync(registerDepartmentAdminMessage.UserName);

                var result = await _dbContext.UserDetail.AddAsync(new UserDetail
                {
                    ApplicationUserId = findUserByUsername.Id,
                    FacultyId = registerDepartmentAdminMessage.FacultyId,
                    CreatedDate = DateTime.UtcNow
                });

                var count = await _dbContext.SaveChangesAsync();
                return new RegisterDepartmentAdminResultMessage
                {
                    Success = true,
                    Message = ""
                };
            }
            else
            {
                return new RegisterDepartmentAdminResultMessage
                {
                    Success = registerUser.Succeeded,
                    Message = registerUser.Errors.First().ToString()
                };
            }

        }

        public async Task<RegisterStudentResultMessage> RegisterStudentAsync(RegisterStudentMessage registerStudentMessage)
        {
            var checkIfRoleExists = await _roleManager.RoleExistsAsync(RoleConstant.SUPERADMIN);
            if (!checkIfRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = RoleConstant.STUDENT
                });
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerStudentMessage.GivenName,
                LastName = registerStudentMessage.SurName,
                Email = registerStudentMessage.Email,
                UserName = registerStudentMessage.UserName,
            };

            var registerUser = await _userManager.CreateAsync(applicationUser, registerStudentMessage.Password);
            if (registerUser.Succeeded)
            {

                await _userManager.AddToRoleAsync(applicationUser, RoleConstant.STUDENT);

                var findUserByUsername = await _userManager.FindByNameAsync(registerStudentMessage.UserName);

                var addFacultyResult = await _dbContext.UserDetail.AddAsync(new UserDetail
                {
                    ApplicationUserId = findUserByUsername.Id,
                    FacultyId = registerStudentMessage.FacultyId,
                    CreatedDate = DateTime.UtcNow
                });

                await _dbContext.SaveChangesAsync();

                var addRegistrationResult = await _dbContext.UserDetailExtension.AddAsync(new UserDetailExtension
                {
                    RegistrationNumber = registerStudentMessage.RegistrationNumber,
                    ExamNumber = registerStudentMessage.ExamNumber,
                    UserDetailId = addFacultyResult.Entity.Id,
                    CreatedDate = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();

                return new RegisterStudentResultMessage
                {
                    Success = true,
                    Message = ""
                };

            }
            else
            {
                return new RegisterStudentResultMessage
                {
                    Success = false,
                    Message = registerUser.Errors.First().ToString()
                };
            }
        }

        public async Task<RegisterSuperAdminResultMessage> RegisterSuperAdminAsync(RegisterSuperAdminMessage registerSuperAdminMessage)
        {
            var checkIfRoleExists = await _roleManager.RoleExistsAsync(RoleConstant.SUPERADMIN);
            if (!checkIfRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = RoleConstant.SUPERADMIN
                });
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerSuperAdminMessage.GivenName.Trim(),
                LastName = registerSuperAdminMessage.SurName.Trim(),
                Email = registerSuperAdminMessage.Email.Trim(),
                UserName = registerSuperAdminMessage.UserName.Trim(),
            };

            var registerUser = await _userManager.CreateAsync(applicationUser, registerSuperAdminMessage.Password);

            if (registerUser.Succeeded)
            {

                await _userManager.AddToRoleAsync(applicationUser, RoleConstant.SUPERADMIN);

                return new RegisterSuperAdminResultMessage
                {
                    Success = registerUser.Succeeded,
                    Message = ""
                };
            }
            else
            {
                return new RegisterSuperAdminResultMessage
                {
                    Success = registerUser.Succeeded,
                    Message = registerUser.Errors.First().ToString()
                };
            }
        }
    }
}
