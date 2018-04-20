using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the store repository
    /// </summary>
    internal interface IStoreRepository : IRepositoryQueryable<Guid, IStore>
    {
        
    }
}