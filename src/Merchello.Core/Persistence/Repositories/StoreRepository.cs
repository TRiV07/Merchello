namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using MS = Merchello.Core.Constants.MultiStore;

    /// <summary>
    /// The ship method repository.
    /// </summary>
    internal class StoreRepository : MerchelloPetaPocoRepositoryBase<IStore>, IStoreRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public StoreRepository(
            IDatabaseUnitOfWork work,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets a <see cref="IStore"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IStore"/>.
        /// </returns>
        protected override IStore PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<StoreDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new StoreFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="IStore"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStore}"/>.
        /// </returns>
        protected override IEnumerable<IStore> PerformGetAll(params Guid[] keys)
        {
            var dtos = new List<StoreDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<StoreDto>(GetBaseQuery(false).WhereIn<StoreDto>(x => x.Key, keyList, SqlSyntax)));
                }

                dtos = keys.Select(k => dtos.FirstOrDefault(x => x.Key == k)).ToList();
            }
            else
            {
                dtos = Database.Fetch<StoreDto>(GetBaseQuery(false));
            }

            var factory = new StoreFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IStore"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStore}"/>.
        /// </returns>
        protected override IEnumerable<IStore> PerformGetByQuery(IQuery<IStore> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IStore>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<StoreDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key)).ToArray();
        }

        /// <summary>
        /// Gets the base SQL clause.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<StoreDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchStore.pk = @Key";
        }

        /// <summary>
        /// Gets the list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchStore WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IStore entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new StoreFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Saves an existing item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IStore entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new StoreFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}