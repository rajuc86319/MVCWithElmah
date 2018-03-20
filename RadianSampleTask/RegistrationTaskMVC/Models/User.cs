using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace RegistrationTaskMVC.Models
{
	/// <summary>
	/// User Details.
	/// </summary>
	[MetadataType(typeof(UserMetadata))]
	public class User
	{
		
		public string LastName { get; set; }
		public string MiddleName { get; set; }
		public string FirstName { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string EmailAddress { get; set; }

		[Remote("IsUserNameExist", "Validation", ErrorMessage = "User Name already Exist")]
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
    }
	
	/// <summary>
	/// defining Meta data for User.
	/// </summary>
	public class UserMetadata
	{

		[Required(ErrorMessage = "Please Enter Last Name")]
		[Display(Name = "Last Name :")]
		public string LastName { get; set; }

		[Display(Name = "Middle Name")]
		public string MiddleName { get; set; }

		[Display(Name = "First Name :")]
		[Required(ErrorMessage = "Please Enter First Name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Please Enter Country")]
		[Display(Name = "Country :")]
		public string Country { get; set; }

		[Required(ErrorMessage = "Please Enter state")]
		[Display(Name = "State :")]
		public string State { get; set; }

		[Required(ErrorMessage = "Please Enter Email")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[Display(Name = "Email :")]
		public string EmailAddress { get; set; }

		[Required(ErrorMessage = "Please Enter password")]
		[Display(Name = "Password :")]
		[MaxLength(8, ErrorMessage = "Name cannot be longer than 8 characters.")]
		[RegularExpression(@"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$",ErrorMessage = "one special character, one uppercase, one lowercase(in any order)")]
		public string Password { get; set; }

		[NotMapped] // Does not effect with your database
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords does not match")]
		[Display(Name = "Confirm Password :")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Please Enter User Name")]
		[Display(Name = "User Name :")]
		[MaxLength(8, ErrorMessage = "Name cannot be longer than 8 characters.")]

		[RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,40}$", ErrorMessage = "special characters are not allowed.")]
		public string UserName { get; set; }
	}
}