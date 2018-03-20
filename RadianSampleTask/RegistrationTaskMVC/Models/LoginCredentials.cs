using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RegistrationTaskMVC.Models
{
	/// <summary>
	///Login details 
	/// </summary>
	[MetadataType(typeof(LoginMetadata))]
	public class LoginCredentials
	{
	    public string userName { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// defining meta data for LoginCredentials
	/// </summary>
	public class LoginMetadata
	{
		[Required(ErrorMessage = "Please Enter User Name")]
		[Display(Name = "User Name :")]
		public string userName { get; set; }

		[Required(ErrorMessage = "Please Enter Password")]
		[Display(Name = "Password :")]
		public string password { get; set; }
	}
}