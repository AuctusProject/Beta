using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Test
{
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider ServiceProvider;

        internal ServiceScopeFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }
    }
}
