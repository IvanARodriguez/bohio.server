using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homespirations.Core.Entities;

namespace Homespirations.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<HomeSpace> HomeSpaces { get; }
        Task<int> SaveChangesAsync();
    }
}