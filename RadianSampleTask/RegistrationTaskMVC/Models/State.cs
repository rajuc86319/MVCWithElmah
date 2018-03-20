using System;

namespace RegistrationTaskMVC.Controllers
{
	public class State
	{
		public int StateID { get; set; }
		public string StateNames { get; set; }
		public Nullable<int> CountryID { get; set; }

		public virtual Countries country { get; set; }
	}
}