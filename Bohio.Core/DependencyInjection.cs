using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bohio.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bohio.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services;

        }
    }
}