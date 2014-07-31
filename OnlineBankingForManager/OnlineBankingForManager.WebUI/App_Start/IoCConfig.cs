// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfig.cs" company="">
//   
// </copyright>
// <summary>
//   The io c config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI
{
    using System.Reflection;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using OnlineBankingForManager.Domain.Concrete;

    /// <summary>
    /// The io c config.
    /// </summary>
    public class IoCConfig
    {
        /// <summary>
        /// The register dependencies.
        /// </summary>
        public static void RegisterDependencies()
        {
            

            var builder = new ContainerBuilder();

            

            #region Setup a common pattern - placed here before RegisterControllers as last one wins

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Provider"))
                .AsImplementedInterfaces()
                .InstancePerRequest();
            builder.Register(c => new EFClientRepository()).AsImplementedInterfaces().InstancePerRequest();

            #endregion

            #region Register all controllers for the assembly

            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerRequest();

            #endregion

            #region Set the MVC dependency resolver to use Autofac

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion
        }
    }
}