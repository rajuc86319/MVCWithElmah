using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using RadianSampleTask;

namespace RadianSampleTask.Controllers
{
    /// <summary>
	/// This api controller is for getting states 
	/// </summary>
	public class StatesController : ApiController
	{
		private UsersRegistrationEntities db = new UsersRegistrationEntities();

		// GET: api/States
		public IQueryable<State> GetStates(int countryId)
		{
			var states = db.States.Where(c => c.CountryID == countryId);
			return states;

		}

		// GET: api/States/5
		[ResponseType(typeof(State))]
		public IHttpActionResult GetState(int id)
		{
			State state = db.States.Find(id);
			if (state == null)
			{
				return NotFound();
			}

			return Ok(state);
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool StateExists(int id)
		{
			return db.States.Count(e => e.StateID == id) > 0;
		}
	}
}