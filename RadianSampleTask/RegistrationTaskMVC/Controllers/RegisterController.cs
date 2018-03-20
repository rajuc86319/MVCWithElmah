using RegistrationTaskMVC.Models;
using System;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Linq;
using RegistrationTaskMVC.WebApi_Calls;

namespace RegistrationTaskMVC.Controllers
{
	/// <summary>
	/// Register controller 
	/// </summary>
	[HandleError]
	public class RegisterController : Controller
	{
		//api call objects
		APICallsForCountries apiCallForCountries = new APICallsForCountries();
		APICallsForUsers apiCallForUsers = new APICallsForUsers();
		
		// GET: Register
		public ActionResult RegisterUser()
		{
			IEnumerable<Countries> countries = apiCallForCountries.GetAllCountries();
			var query = countries.Select(c => new { c.countryId, c.countryName });
			ViewBag.Countries = new SelectList(query.AsEnumerable(), "countryId", "countryName");

			return View();
		}

		/// <summary>
		/// Action method for user registration.
		/// </summary>
		/// <param name="U">User</param>
		/// <returns>view based on registration status</returns>
		[HttpPost]
		public ActionResult RegisterUser(User U)
		{
			
			bool registerSuccess = apiCallForUsers.CreateUser(U);
			if (registerSuccess)
			{
				return View("UserDetails", U);
			}
			else
			{
				return View("Error");
			}
			
        }
	}
}