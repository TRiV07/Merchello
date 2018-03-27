using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories.Interfaces
{
    interface IMSRepository<in TId, TEntity> : IRepository<TId, TEntity>
    {
        TEntity Get(Guid key, int storeId);
        IEnumerable<TEntity> GetAll(int storeId, params Guid[] keys);
    }
}
