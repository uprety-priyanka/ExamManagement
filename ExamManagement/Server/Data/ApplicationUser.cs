using Microsoft.AspNetCore.Identity;

namespace ExamManagement.Server.Data
{
    public class ApplicationUser:IdentityUser 
    {
        public override string Id { get => base.Id; set => base.Id = value; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
