namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// The Carrier repository.
    /// </summary>
    internal class CarrierRepository : PagedMSRepositoryBase<ICarrier, CarrierDto>, ICarrierRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The database unit of work
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        /// <param name="storeId">
        /// The domain root structure ID.
        /// </param>
        public CarrierRepository(
            IDatabaseUnitOfWork work,
            int storeId,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntax)
            : base(work, storeId, logger, sqlSyntax)
        {

        }

        /// <summary>
        /// Searches Carriers
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildCarrierSearchSql(searchTerm);

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Performs the Get by key operation.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICarrier"/>.
        /// </returns>
        protected override ICarrier PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });


            var dto = Database.Fetch<CarrierDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CarrierFactory();
            var carrier = factory.BuildEntity(dto);

            return carrier;
        }

        /// <summary>
        /// The perform get all operation.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of all <see cref="ICarrier"/>.
        /// </returns>
        protected override IEnumerable<ICarrier> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<CarrierDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<CarrierDto>(GetBaseQuery(false).WhereIn<CarrierDto>(x => x.Key, keyList, SqlSyntax)));
                }

                dtos = keys.Select(k => dtos.FirstOrDefault(x => x.Key == k)).ToList();
            }
            else
            {
                dtos = Database.Fetch<CarrierDto>(GetBaseQuery(false));
            }

            var factory = new CarrierFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// The get base query.
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
                .From<CarrierDto>(SqlSyntax);

            if (_storeId != Core.Constants.MultiStore.DefaultId)
            {
                sql.Where<CarrierDto>(x => x.StoreId == _storeId, SqlSyntax);
            }

            return sql;
        }

        /// <summary>
        /// The get base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchCarrier.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "UPDATE merchShipment SET carrierKey = NULL WHERE carrierKey = @Key",
                    "DELETE FROM merchCarrier WHERE pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(ICarrier entity)
        {
            ((Carrier)entity).AddingEntity();

            var factory = new CarrierFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(ICarrier entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CarrierFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(ICarrier entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// The perform get by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ICarrier"/>
        /// </returns>
        protected override IEnumerable<ICarrier> PerformGetByQuery(IQuery<ICarrier> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICarrier>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CarrierDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key)).ToArray();
        }

        /// <summary>
        /// Builds Carrier search SQL.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql BuildCarrierSearchSql(string searchTerm)
        {
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var sql = new Sql();
            sql.Select("*").From<CarrierDto>(SqlSyntax);
            if (_storeId != Core.Constants.MultiStore.DefaultId)
            {
                sql.Where<CarrierDto>(x => x.StoreId == _storeId, SqlSyntax);
            }

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("% ", terms)).Trim();

                sql.Where("lastName LIKE @ln OR firstName LIKE @fn OR email LIKE @email", new { @ln = preparedTerms, @fn = preparedTerms, @email = preparedTerms });
            }

            return sql;
        }
    }
}
