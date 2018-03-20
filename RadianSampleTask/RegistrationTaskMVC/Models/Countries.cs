using System.Collections.Generic;

namespace RegistrationTaskMVC.Controllers
{
	public class Countries
	{
		public string countryName { get; set; }
		public int countryId { get; set; }
		public virtual ICollection<State> States { get; set; }

	}
}