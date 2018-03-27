using Merchello.Core.Models.EntityBase;
using Merchello.Core.Persistence.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Repositories
{
    internal abstract class MerchelloMSPetaPocoRepositoryBase<TEntity> : MerchelloPetaPocoRepositoryBase<TEntity>
        where TEntity : class, IEntity
    {

        public MerchelloMSPetaPocoRepositoryBase(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
        }

        public TEntity Get(Guid key, int storeId)
        {
            // TODO - This is hit a lot, we need to find out why and where
            var entity = PerformGet(key, storeId);
            if (entity != null)
            {
                entity.ResetDirtyProperties();
            }

            return entity;
        }

        public IEnumerable<TEntity> GetAll(int storeId, params Guid[] keys)
        {
            return PerformGetAll(storeId, keys);
        }

        protected abstract TEntity PerformGet(Guid key, int storeId);
       
        protected abstract IEnumerable<TEntity> PerformGetAll(int storeId, params Guid[] keys);

        protected abstract string GetBaseMSWhereClause();
    }
}
