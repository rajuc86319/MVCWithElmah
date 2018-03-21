using RegistrationTaskMVC.Models;
using RegistrationTaskMVC.WebApi_Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RegistrationTaskMVC.Controllers
{
	/// <summary>
	/// Login Controller 
	/// </summary>
	[HandleError]
	public class LoginController : Controller
    {
		//object for users api call  
		APICallsForUsers apiCallForUsers = new APICallsForUsers();
		
		// GET: Login
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// validating login details 
		/// </summary>
		/// <param name="ld"></param>
		/// <returns>returns a view based on the user validation</returns>
		[HttpPost]
		public ActionResult ValidateLoginDetails(LoginCredentials loginDetails)
		{
			bool validUser = apiCallForUsers.UserExist(loginDetails.userName, loginDetails.password);
			if (validUser)
			{
				return View("SuccessfullLogin", loginDetails);
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid Credentials");
				return View("Index");
			}
		}
	}
}