using System.Web.Routing;
using MvcContrib.TestHelper;
using NUnit.Framework;
//using RoutingSample.Controllers;

namespace OnlineBankingForManager.NUnitTests
{
    using System.Web.Routing;
    using NUnit.Framework;
    using OnlineBankingForManager.WebUI;
    using OnlineBankingForManager.WebUI.Controllers;
    [TestFixture]
    class RoutesNUnitTestFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
           // RouteTable.Routes.Clear();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        [Test]
        public void root_maps_to_manager_list()
        {
            "~/".ShouldMapTo<ManagerController>(x => x.List(1, 10, null, null));
        }

        [Test]
        public void List_Manager_url()
        {
            OutBoundUrl.Of<ManagerController>(x => x.List(1, 10, null, null)).ShouldMapToUrl("/");
        }
    }
}
