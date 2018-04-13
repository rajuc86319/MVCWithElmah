using RegistrationTaskMVC.Models;
using RegistrationTaskMVC.WebApi_Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationTaskMVC.Controllers
{
	[HandleError]
    public class ValidationController : Controller
    {
		//object for users api call  
		APICallsForUsers apiCallForUsers = new APICallsForUsers();
		
		// GET: Validation
		public ActionResult Index()
        {
			//done some chnages for testing 
			return View();
        }
	
		/// <summary>
		/// Checks whether the user name already exist or not 
		/// </summary>
		/// <param name="UserName">string</param>
		/// <returns>username exist status as json result</returns>
		[HttpGet]
		public JsonResult IsUserNameExist(string UserName)
		{

			IEnumerable<User> allUsers = apiCallForUsers.GetAllUsers();
			bool isExist = allUsers.Where(u => u.UserName == UserName).Any();
			return Json(!isExist, JsonRequestBehavior.AllowGet);
		}
	}
}