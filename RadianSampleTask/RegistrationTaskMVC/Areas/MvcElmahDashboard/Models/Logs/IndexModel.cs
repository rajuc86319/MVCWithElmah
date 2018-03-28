using System;

namespace RegistrationTaskMVC.Areas.MvcElmahDashboard.Models.Logs
{
    public class IndexModel
    {
        public string[] Applications { get; set; }

        public string[] Hosts { get; set; }
        
        public string[] Types { get; set; }
        
        public string[] Sources { get; set; }

		public string[] ErrorCategories { get; set; }
	}
}