using RegistrationTaskMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.ModelBinding;

namespace RegistrationTaskMVC.WebApi_Calls
{
	/// <summary>
	/// Calling Web Api methods for Users 
	/// </summary>
	public class APICallsForUsers
	{
		/// <summary>
		/// Register the User 
		/// </summary>
		/// <param name="U"></param>
		/// <returns>boolean value</returns>
		public  bool CreateUser(User U)
		{
		   using (var client = new HttpClient())
			 {
				client.BaseAddress = new Uri("http://localhost:53295/api/users");

				//HTTP POST
				var postTask = client.PostAsJsonAsync<User>("Users", U);
				postTask.Wait();
				var result = postTask.Result;
				if (result.IsSuccessStatusCode)
				{
					return true;
				}
				else
					return false;
		    }
	    }
		
		/// <summary>
		/// checks the user authentication 
		/// </summary>
		/// <param name="uname">Username</param>
		/// <param name="password">password</param>
		/// <returns>returns the boolean value based on the details exist</returns>
		public bool UserExist(string uname, string password)
		{
			bool userValidity = true;
			IEnumerable<User> users = GetAllUsers();

			userValidity = users.Where(u => u.UserName == uname && u.Password == password).Any();
			return userValidity;
		}
		
		/// <summary>
		/// Gets all the users 
		/// </summary>
		/// <returns>users list</returns>
		public IEnumerable<User> GetAllUsers()
		{
			IEnumerable<User> users = new List<User>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:53295/api/");
				//HTTP GET
				var responseTask = client.GetAsync("Users");
				responseTask.Wait();

				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsAsync<IList<User>>();
					readTask.Wait();

					users = readTask.Result;
				}
				else //web api sent error response 
				{
					//log response status here.
					users = Enumerable.Empty<User>();		
				}

			}
			return users;
		}
	}
}