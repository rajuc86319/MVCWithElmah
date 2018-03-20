using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadianSampleTask;
using RadianSampleTask.Controllers;


namespace UnitTestsForMVCAPI
{
	[TestClass]
	public class UnitTestsOnWebAPIController
	{
		[TestMethod]
		public void TestMethodsOnGetStates()
		{
			StatesController sc = new StatesController();
			IQueryable<State> states = sc.GetStates(12);
			Assert.AreEqual(states.Count(),1);
		}
		[TestMethod]
		public void TestMethodsOnGetUsers()
		{
			UsersController sc = new UsersController();
			IQueryable<User> users = sc.GetUsers();
			Assert.AreEqual(users.Any(), true);
		}
		[TestMethod]
		public void TestMethodsOnGetCountries()
		{
			countriesController sc = new countriesController();
			IQueryable<country> countries = sc.Getcountries();
			Assert.AreEqual(countries.Any(), true);

		}
		

	}
}
