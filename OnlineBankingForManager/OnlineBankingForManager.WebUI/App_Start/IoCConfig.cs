using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using OnlineBankingForManager.Domain.Abstract;
using OnlineBankingForManager.Domain.Concrete;

namespace OnlineBankingForManager.WebUI
{
    public class IoCConfig
    {

        public static void RegisterDependencies()
        {
            #region Create the builder
            var builder = new ContainerBuilder();
            #endregion

            #region Setup a common pattern - placed here before RegisterControllers as last one wins
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Provider")).AsImplementedInterfaces().InstancePerRequest();
            builder.Register(c => new EFClientRepository()).AsImplementedInterfaces().InstancePerRequest();
            #endregion

            #region Register all controllers for the assembly
            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();
            #endregion

            #region Set the MVC dependency resolver to use Autofac
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            #endregion

        }

    }
}