using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationTaskMVC.Areas.MvcElmahDashboard.Models.Home
{
	public class DateStatsModel
	{
		public DateTime Timestamp { get; set; }

		public DateTime RangeStart { get; set; }

		public DateTime RangeEnd { get; set; }

		public int[] DailyCounters { get; set; }

		public List<Code.ElmahErrorItem> ErrorsInTheDateRange { get; set; }
	}
}