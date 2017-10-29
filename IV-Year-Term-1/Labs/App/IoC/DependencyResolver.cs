using System;
using Autofac;

namespace App.IoC
{
    public class DependencyResolver : IDisposable
    {
        private readonly ILifetimeScope lifetimeScope;

        private bool disposedValue = false;

        public DependencyResolver(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public T Resolve<T>()
        {
            return this.lifetimeScope.Resolve<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.lifetimeScope.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}