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
using RadianSampleTask;

namespace RadianSampleTask.Controllers
{
	/// <summary>
	///it is for getting countries from the database 
	/// </summary>
	public class countriesController : ApiController
    {
        private UsersRegistrationEntities db = new UsersRegistrationEntities();

        // GET: api/countries
        public IQueryable<country> Getcountries()
        {
            return db.countries;
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}