using RegistrationTaskMVC.Areas.MvcElmahDashboard.Code;
using RegistrationTaskMVC.Areas.MvcElmahDashboard.Models.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RegistrationTaskMVC.Areas.MvcElmahDashboard.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class LogsController : Controller
    {
        private static RequeueCache<string, ActionResult> ExternalRequestsCache = new RequeueCache<string, ActionResult>(256);

        private static dynamic UserAgentInfoProvider = new ExternalServiceConfig(ConfigurationManager.AppSettings["MvcElmahDashboardUserAgentInfoProvider"]);

        private static dynamic RemoteAddressInfoProvider = new ExternalServiceConfig(ConfigurationManager.AppSettings["MvcElmahDashboardRemoteAddressInfoProvider"]);

        
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


		public ActionResult Index(int? offset, int? length)
        {
            using (var context = new ElmahDashboardContext())
            {
                var model = new IndexModel();
                model.Applications = context.ListApplications().ToArray();
                model.Hosts = context.ListHosts().ToArray();
                model.Sources = context.ListSource().ToArray();
                model.Types = context.ListTypes().ToArray();
			//	model.ErrorCategories = context.ListCategories().ToArray();

				return View(model);
            }
        }

        public ActionResult Items(int? offset, int? length, string application, string host, string source, string type, string search, int? truncValueLength)
        {
            using (var context = new ElmahDashboardContext())
            {
                var where = "([Application] LIKE @p0) AND ([Host] LIKE @p1) AND ([Source] LIKE @p2) AND ([Type] LIKE @p3) AND ([AllXml] LIKE @p4)";
                var parameters = new object[] { application.IfNullOrWhiteSpace("%"), host.IfNullOrWhiteSpace("%"), source.IfNullOrWhiteSpace("%"), type.IfNullOrWhiteSpace("%"), "%" + search + "%" };
                var orderBy = (String)null;
                var model = new ItemsModel();

				IEnumerable<Categories> allCategories = context.ListCategories();
				var query = allCategories.Select(c => new { c.catogeryID, c.categoryName });
				ViewBag.categories = new SelectList(query.AsEnumerable(), "catogeryID", "categoryName");

				model.Items = context.ListErrors(offset ?? 0, length ?? 10, where, orderBy, false, parameters).ToArray();

				if (truncValueLength.HasValue)
                {
                    foreach (var item in model.Items)
                    {
                        if (item.Type != null && item.Type.Length > truncValueLength)
                            item.Type = item.Type.Substring(0, truncValueLength.Value) + "...";
                        if (item.Source != null && item.Source.Length > truncValueLength)
                            item.Source = item.Source.Substring(0, truncValueLength.Value) + "...";
                        if (item.Message != null && item.Message.Length > truncValueLength)
                            item.Message = item.Message.Substring(0, truncValueLength.Value) + "...";
                    }
                }
                return View("ItemsPart", model);
            }
        }

        public ActionResult Details(DetailsModel model)
        {
            model.UserAgentInfoProvider = UserAgentInfoProvider;
            model.RemoteAddressInfoProvider = RemoteAddressInfoProvider;
            return View(model);
        }

        public ActionResult ItemDetails(Guid id)
        {
            using (var context = new ElmahDashboardContext())
            {
                var model = new ItemDetailsModel();
                model.Item = context.GetError(id);
                model.UserAgentInfoProvider = UserAgentInfoProvider;
                model.RemoteAddressInfoProvider = RemoteAddressInfoProvider;

                return View("ItemDetailsPart", model);
            }
        }

        public ActionResult NextItemDetails(PrevNextRequest input)
        {
            using (var context = new ElmahDashboardContext())
            {
                var where = "([Application] LIKE @p0) AND ([Host] LIKE @p1) AND ([Source] LIKE @p2) AND ([Type] LIKE @p3) AND ([AllXml] LIKE @p4) AND ([Sequence] > @p5)";
                var parameters = new object[] { input.Application.IfNullOrWhiteSpace("%"), input.Host.IfNullOrWhiteSpace("%"), input.Source.IfNullOrWhiteSpace("%"), input.Type.IfNullOrWhiteSpace("%"), "%" + input.Search + "%", input.Sequence };
                var orderBy = "[Sequence] ASC";

                var model = new ItemDetailsModel();
                model.Item = context.ListErrors(0, 1, where, orderBy, true, parameters).SingleOrDefault();
                model.UserAgentInfoProvider = UserAgentInfoProvider;
                model.RemoteAddressInfoProvider = RemoteAddressInfoProvider;

                if (model.Item != null)
                    return View("ItemDetailsPart", model);
                else
                    return this.HttpNotFound();
            }
        }

        public ActionResult PreviousItemDetails(PrevNextRequest input)
        {
            using (var context = new ElmahDashboardContext())
            {
                var where = "([Application] LIKE @p0) AND ([Host] LIKE @p1) AND ([Source] LIKE @p2) AND ([Type] LIKE @p3) AND ([AllXml] LIKE @p4) AND ([Sequence] < @p5)";
                var parameters = new object[] { input.Application.IfNullOrWhiteSpace("%"), input.Host.IfNullOrWhiteSpace("%"), input.Source.IfNullOrWhiteSpace("%"), input.Type.IfNullOrWhiteSpace("%"), "%" + input.Search + "%", input.Sequence };
                var orderBy = "[Sequence] DESC";

                var model = new ItemDetailsModel();
                model.Item = context.ListErrors(0, 1, where, orderBy, true, parameters).SingleOrDefault();
                model.UserAgentInfoProvider = UserAgentInfoProvider;
                model.RemoteAddressInfoProvider = RemoteAddressInfoProvider;

                if (model.Item != null)
                    return View("ItemDetailsPart", model);
                else
                    return this.HttpNotFound();
            }
        }

        public ActionResult AllXml(Guid id)
        {
            using (var context = new ElmahDashboardContext())
            {
                var item = context.GetError(id);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                return Content(item.AllXml.ToString());
            }
        }

        //[HttpGet]
        public ActionResult UserAgentInfo(string id)
        {
            var url = (string)UserAgentInfoProvider.ServiceUrl;
            if (!String.IsNullOrWhiteSpace(url))
            {
                url = url.Replace("{value}", Server.UrlEncode(id));
                return DelegateJsonRequest(url, true);
            }
            else
            {
                return HttpNotFound();
            }
        }
		[HttpPost]
		public bool UpdateCategory(int sequenceId, int categoryId)
		{
			ElmahDashboardContext ec = new ElmahDashboardContext();
			ec.CategoryUpdation(sequenceId, categoryId);
			return true;
		}



		[HttpPost]
		public bool UpdateAnalysis(int Sequence,string textAreaData)
		{
			ElmahDashboardContext ec = new ElmahDashboardContext();
			ec.AnalysisUpdation(Sequence, textAreaData);
			return true;
		}


		//[HttpGet]
		public ActionResult RemoteHostInfo(string id)
        {
            var url = (string)RemoteAddressInfoProvider.ServiceUrl;
            if (!String.IsNullOrWhiteSpace(url))
            {
                url = url.Replace("{value}", Server.UrlEncode(id));
                return DelegateJsonRequest(url, true);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [NonAction]
        private ActionResult DelegateJsonRequest(string url, bool allowCaching)
        {
            if (allowCaching)
            {
                var result = (ActionResult)null;
                if (ExternalRequestsCache.TryGet(url, out result))
                {
                    return result;
                }
            }

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 10000;
            using (var resp = req.GetResponse())
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                var result = Content(data, "application/json", Encoding.UTF8);
                if (allowCaching) ExternalRequestsCache.Set(url, result);
                return result;
            }
        }
		/// <summary>
		/// to get all the errors
		/// </summary>

		private static List<ElmahError> GetAllErrors()
		{
			List<ElmahError> allErrors = new ElmahDashboardContext().GetAllErrors();
			return allErrors;
		}
		private static List<ElmahError> GetHourlyErrors()
		{
			List<ElmahError> allHourlyErrors = LogsController.GetAllErrors().Where(i => i.TimeUtc >= DateTime.UtcNow.AddHours(-1) && i.TimeUtc <= DateTime.UtcNow).ToList();
			return allHourlyErrors;
		}
		private static List<ElmahError> GetDailyErrors()
		{
			List<ElmahError> allDailyErrors = LogsController.GetAllErrors().Where(i => i.TimeUtc >= DateTime.UtcNow.AddHours(-24) && i.TimeUtc <= DateTime.UtcNow).ToList();
			return allDailyErrors;
		}
		/// <summary>
		/// action for Bar chart of hourly errors
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateBarHourly()
		{
			ViewBag.ChartType = "CreateBarHourly";
			//Create bar chart
			ViewBag.errorTypes = GetDistinctErrorTypes(LogsController.GetHourlyErrors());
			ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, LogsController.GetHourlyErrors());
			return View("CreateBar");
		}
		/// <summary>
		/// action for line chart of hourly errors
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateLineHourly()
		{
			var now = DateTime.UtcNow;
			ViewBag.ChartType = "CreateLineHourly";
			ViewBag.errorTypes = GetDistinctErrorTypes(LogsController.GetHourlyErrors());
			ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, LogsController.GetHourlyErrors());
			return View("CreateLine");
		}
		/// <summary>
		/// /// <summary>
		/// action for Bar chart of daily errors
		/// </summary>
		/// <returns></returns>
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateBarDaily()
		{

			//Create bar chart
			ViewBag.errorTypes = GetDistinctErrorTypes(LogsController.GetDailyErrors());
			ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, LogsController.GetDailyErrors());
			return View("CreateBar");
		}
		/// <summary>
		/// action for line chart of daily errors
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateLineDaily()
		{
			var now = DateTime.UtcNow;
			//Create bar chart
			ViewBag.errorTypes = GetDistinctErrorTypes(LogsController.GetDailyErrors());
			ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, LogsController.GetDailyErrors());
			return View("CreateLine");
		}
		///// <summary>
		///// action for bar chart
		///// </summary>
		///// <returns></returns>
		//[HttpGet]
		//public ActionResult CreateBar()
		//{
		//	//Create bar chart
			
		//	ViewBag.errorTypes = GetDistinctErrorTypes(allErrors);
		//	ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, allErrors);
		//	return View();
		//}
		///// <summary>
		///// action for line chart
		///// </summary>
		///// <returns></returns>
		//[HttpGet]
		//public ActionResult CreateLine()
		//{
		//	//Create bar chart
		//	ViewBag.errorTypes = GetDistinctErrorTypes(allErrors);
		//	ViewBag.ErrorCount = GetErrorCount(ViewBag.errorTypes, allErrors);
		//	return View();
		//}

		/// <summary>
		/// gets all the Distinct errors logged in the DB
		/// </summary>
		/// <param name="allErrors"></param>
		/// <returns></returns>
		[NonAction]
		private string[] GetDistinctErrorTypes(List<ElmahError> allErrors)
		{
			
			string[] errorTypes = allErrors.Select(x => x.Type).Distinct().ToArray();
			return errorTypes;
		}

		/// <summary>
		/// Get the Error count of specific error
		/// </summary>
		/// <param name="errorTypes"></param>
		/// <param name="allErrors"></param>
		/// <returns></returns>
		[NonAction]
	   private int[] GetErrorCount(string [] errorTypes, List<ElmahError> allErrors)
		{
			
			int[] errorCount = new int[errorTypes.Length];
			int count, i = 0;
			foreach (string errorType in errorTypes)
			{
				count = allErrors.Where(x => x.Type == errorType).Count();
				errorCount[i] = count;
				i++;
			}
			return errorCount;
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