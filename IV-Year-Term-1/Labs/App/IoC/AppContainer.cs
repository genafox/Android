using App.Domain.Database;
using App.Domain.Interfaces;
using App.Domain.Repositories;
using Autofac;
using SQLite;

namespace App.IoC
{
    public class AppContainer
    {
        private static IContainer Container;

        static AppContainer()
        {
            InitDatabase();
            InitContainer();
        }

        public static DependencyResolver GetDependencyResolver()
        {
            ILifetimeScope scope = Container.BeginLifetimeScope();

            return new DependencyResolver(scope);
        }

        private static void InitDatabase()
        {
            Database.Init();
        }

        private static void InitContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register((ctx, p) => Database.EstablishConnection()).As<SQLiteConnection>();
            builder.RegisterType<SQLiteNoteRepository>().As<INoteRepository>();

            Container = builder.Build();
        }
    }
}