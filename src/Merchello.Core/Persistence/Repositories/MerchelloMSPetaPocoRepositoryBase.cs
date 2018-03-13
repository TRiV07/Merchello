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

        public TEntity Get(Guid key, int domainRootStructureID)
        {
            // TODO - This is hit a lot, we need to find out why and where
            var entity = PerformGet(key, domainRootStructureID);
            if (entity != null)
            {
                entity.ResetDirtyProperties();
            }

            return entity;
        }

        public IEnumerable<TEntity> GetAll(int domainRootStructureID, params Guid[] keys)
        {
            return PerformGetAll(domainRootStructureID, keys);
        }

        protected abstract TEntity PerformGet(Guid key, int domainRootStructureID);
       
        protected abstract IEnumerable<TEntity> PerformGetAll(int domainRootStructureID, params Guid[] keys);

        protected abstract string GetBaseMSWhereClause();
    }
}
