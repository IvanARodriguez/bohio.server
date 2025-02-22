using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homespirations.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Homespirations.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services;

        }
    }
}