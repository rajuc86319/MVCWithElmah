using RegistrationTaskMVC.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace RegistrationTaskMVC.WebApi_Calls
{
    /// <summary>
	/// This Class is for calling Web API methods with various Http verbs(Get,Post,Put,Delete)
	/// </summary>
	public class APICallsForCountries
	{
		/// <summary>
		/// This Method calls the web api method for Getting all the countries  
		/// </summary>
		/// <returns>returns countries list</returns>
		public IEnumerable<Countries> GetAllCountries()
		{
			IEnumerable<Countries> countries = new List<Countries>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:53295/api/");
				//HTTP GET
				var responseTask = client.GetAsync("countries");
				responseTask.Wait();

				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsAsync<IList<Countries>>();
					readTask.Wait();
					countries = readTask.Result;
				}
				else //web api sent error response 
				{
					//log response status here..
					countries = Enumerable.Empty<Countries>();
				}
				return countries;
			}
		}

	}
}