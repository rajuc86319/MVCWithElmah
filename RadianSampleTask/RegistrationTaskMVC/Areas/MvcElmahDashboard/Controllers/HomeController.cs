using RegistrationTaskMVC.Areas.MvcElmahDashboard.Code;
using RegistrationTaskMVC.Areas.MvcElmahDashboard.Models.Home;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace RegistrationTaskMVC.Areas.MvcElmahDashboard.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        private static ElmahErrorCounters EECounters = new ElmahErrorCounters(TimeSpan.FromDays(15));
        private static HeartBeatService HeartBeatService = new HeartBeatService();

        #region Infrastructure

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Apply culture:
            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MvcElmahDashboardCulture"]))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["MvcElmahDashboardCulture"]);
            }

            base.OnActionExecuting(filterContext);
        }

        #endregion
//local latest changes
        // GET: ElmahLog/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HourlyStats()
        {
            using (var context = new ElmahDashboardContext())
            {
                EECounters.UpdateCache(context);

                var now = DateTime.UtcNow;
                var model = new HourlyStatsModel();
                model.Timestamp = now;
                model.RangeEnd = now.TruncToHours().AddHours(1);
                model.RangeStart = model.RangeEnd.AddHours(-24);
                model.HourlyCounters = EECounters.GetHourlyCounters(model.RangeStart, model.RangeEnd);
                model.LastHourErrors = EECounters.GetErrors(now.AddHours(-1), now);
                model.SampleLogCount = LogsController.GetAllErrors().Where(i => i.TimeUtc >= DateTime.UtcNow.AddHours(-1) && i.TimeUtc <= DateTime.UtcNow).ToList().Count;
				foreach (var app in EECounters.GetErrors(now.AddHours(-4), now).Select(i => i.Application).Distinct())
                {
                    var appvar = app;
                    model.AppHourlyCounters[appvar] = EECounters.GetHourlyCounters(model.RangeEnd.AddHours(-4), model.RangeEnd, item => item.Application == appvar);
                }

                return View(model);
            }
        }
		public ActionResult GetErrorsByDate(DateTime startDate, DateTime endDate)
		{
			ViewBag.controller = "GetErrorsByDate";
			ViewBag.startDate = startDate;
			ViewBag.endDate = endDate;
			var model = HomeController.GetDailyStatsModel();

			return View("DailyStats", model);

		}

		[NonAction]
		public static DailyStatsModel GetDailyStatsModel()
		{
			using (var context = new ElmahDashboardContext())
			{
				EECounters.UpdateCache(context);

				var now = DateTime.UtcNow;
				var model = new DailyStatsModel();
				model.Timestamp = now;
				model.RangeEnd = now.TruncToDays().AddDays(1);
				model.RangeStart = model.RangeEnd.AddDays(-14);
				model.DailyCounters = EECounters.GetDailyCounters(model.RangeStart, model.RangeEnd);
				model.LastDayErrors = EECounters.GetErrors(now.AddDays(-1), now);
				model.SampleLogCount = LogsController.GetAllErrors().Where(i => i.TimeUtc >= DateTime.UtcNow.AddHours(-24) && i.TimeUtc <= DateTime.UtcNow).ToList().Count;
				foreach (var app in EECounters.GetErrors(now.AddDays(-4), now).Select(i => i.Application).Distinct())
				{
					var appvar = app;
					model.AppDailyCounters[appvar] = EECounters.GetDailyCounters(model.RangeEnd.AddDays(-4), model.RangeEnd, item => item.Application == appvar);
				}
				return model;
			}
		}
		public ActionResult StatsByDate(string textData)
		{
			return View();
		}

		public ActionResult DailyStats()
        {
			ViewBag.controller = "DailyStats";
				   var model = HomeController.GetDailyStatsModel();
				return View(model);
            }
        }
		public ActionResult StatsByDate(DateTime startDate,DateTime EndDate)
		{
			using (var context = new ElmahDashboardContext())
			{
				var model = new DateStatsModel();
				model.RangeEnd = startDate;
				model.RangeStart =EndDate;
				model.ErrorsInTheDateRange = EECounters.GetErrorsByDate(startDate,EndDate).ToList();
				return View(model);
			}

    
				
		}
        public ActionResult Heartbeat()
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache"); 
            Response.Expires = -1;
            if (HeartBeatService.Beat())
                return new HttpStatusCodeResult(HttpStatusCode.OK, "Heartbeat succeeded");
            else
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Heartbeat failed");
        }

        /// <summary>
        /// Used to handle scripts and styles requests.
        /// </summary>
        protected override void HandleUnknownAction(string actionName)
        {
            this.View(actionName).ExecuteResult(this.ControllerContext);
        }
    }
}
