using Autofac;
using VMS.Data;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace VMS.Infrastructure
{
    public class DefaultInfrastructureModule : Module
    {
        private readonly bool _isDevelopment = false;
        private readonly List<Assembly> _assemblies = new();

        public DefaultInfrastructureModule(bool isDevelopment, Assembly callingAssembly = null)
        {
            _isDevelopment = isDevelopment;
            var coreAssembly = Assembly.GetAssembly(typeof(DatabasePopulator));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(Repository));
            _assemblies.Add(coreAssembly);
            _assemblies.Add(infrastructureAssembly);
            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }
            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>()
                .InstancePerLifetimeScope();

            builder.RegisterType<Repository>().As<IRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .AsClosedTypesOf(typeof(IHandle<>));

        }

        private static void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            // Add development only services
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess) // find all types in the assembly
                   .Where(t => t.Name.EndsWith("RepositoryExt")) // filter the types
                   .AsImplementedInterfaces()  // register the service with all its public interfaces
                   .SingleInstance(); // register the services as singletons

            builder.GetHashCode();
        }

        private static void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess) // find all types in the assembly
                   .Where(t => t.Name.EndsWith("RepositoryExt")) // filter the types
                   .AsImplementedInterfaces()  // register the service with all its public interfaces
                   .SingleInstance(); // register the services as singletons

            // Add production only services
            builder.GetHashCode();
        }
    }
}
