﻿namespace Merchello.Core.Persistence.Repositories
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

    using MS = Merchello.Core.Constants.MultiStore;

    /// <summary>
    /// Represents the NotificationMethodRepository
    /// </summary>
    internal class NotificationMethodRepository : MerchelloPetaPocoRepositoryBase<INotificationMethod>, INotificationMethodRepository
    {
        /// <summary>
        /// The domain root structure ID.
        /// </summary>
        private readonly int _storeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMethodRepository"/> class.
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
        public NotificationMethodRepository(IDatabaseUnitOfWork work, int storeId, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
            _storeId = storeId;
        }

        /// <summary>
        /// Gets a <see cref="INotificationMethod"/> by query.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="INotificationMethod"/>.
        /// </returns>
        protected override INotificationMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
             .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<NotificationMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new NotificationMethodFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INotificationMethod}"/>.
        /// </returns>
        protected override IEnumerable<INotificationMethod> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<NotificationMethodDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<NotificationMethodDto>(GetBaseQuery(false).WhereIn<NotificationMethodDto>(x => x.Key, keyList, SqlSyntax)));
                }

                dtos = keys.Select(k => dtos.FirstOrDefault(x => x.Key == k)).ToList();
            }
            else
            {
                dtos = Database.Fetch<NotificationMethodDto>(GetBaseQuery(false));
            }

            var factory = new NotificationMethodFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }

        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMethod"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INotificationMethod}"/>.
        /// </returns>
        protected override IEnumerable<INotificationMethod> PerformGetByQuery(IQuery<INotificationMethod> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<INotificationMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<NotificationMethodDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key)).ToArray();
        }

        /// <summary>
        /// Gets the base query.
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
                .From<NotificationMethodDto>(SqlSyntax);

            if (_storeId != MS.DefaultId)
            {
                sql.Where<NotificationMethodDto>(x => x.StoreId == _storeId, SqlSyntax);
            }

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
            return "merchNotificationMethod.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchNotificationMessage WHERE methodKey = @Key",
                "DELETE FROM merchNotificationMethod WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(INotificationMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new NotificationMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(INotificationMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new NotificationMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}