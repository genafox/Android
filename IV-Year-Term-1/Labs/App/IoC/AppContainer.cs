using App.Domain.Database;
using App.Domain.Interfaces;
using App.Domain.Repositories;
using App.Domain.Services;
using Autofac;
using SQLite;

namespace App.IoC
{
    public class AppContainer
    {
        static AppContainer()
        {
            InitDatabase();
            InitContainer();
        }

        public static IContainer Container { get; set; }

        private static void InitDatabase()
        {
            Database.Init();
        }

        private static void InitContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register(c => Database.EstablishConnection())
                   .As<SQLiteConnection>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<SQLiteNoteRepository>()
                   .As<INoteRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<PreferenceServcie>()
                   .As<ISettingsService>()
                   .SingleInstance();

            Container = builder.Build();
        }
    }
}