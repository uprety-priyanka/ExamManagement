using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastageFoodManagementAndDonation.Shared.AuthenticationViewModel
{
	public class ResetPasswordViewModel
	{
		public string UserId { get; set; }
		public string ResetPasswordToken { get; set; }
		[Display(Name ="New Password"), DataType(DataType.EmailAddress), Required(ErrorMessage ="New Password field cannot be empty.")]
		public string NewPassword { get; set; }
		[Display(Name = "Confirm New Password"), Compare(nameof(NewPassword), ErrorMessage ="Confirm New Password doesn't match with new password.")]
		public string ConfirmNewPassword { get; set; }
	}
}
