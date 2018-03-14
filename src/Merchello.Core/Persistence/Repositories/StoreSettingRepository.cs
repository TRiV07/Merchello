﻿namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;
    using Models.TypeFields;
    using Querying;
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using UnitOfWork;
    using static Umbraco.Core.Persistence.Database;

    /// <summary>
    /// Represents the Store Settings Repository
    /// </summary>
    internal class StoreSettingRepository : MerchelloMSPetaPocoRepositoryBase<IStoreSetting>, IStoreSettingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public StoreSettingRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets the next invoice number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextInvoiceNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="invoicesCount">The number of invoices needing invoice numbers.  Useful when saving multiple new invoices.</param>
        /// <returns>The next invoice number</returns>
        public int GetNextInvoiceNumber(Guid storeSettingKey, int domainRootStructureID, Func<int> validate, int invoicesCount = 1)
        {
            Mandate.ParameterCondition(1 <= invoicesCount, "invoicesCount");

            var setting = Get(storeSettingKey, domainRootStructureID);
            if (string.IsNullOrEmpty(setting.Value)) setting.Value = "1";
            var nextInvoiceNumber = int.Parse(setting.Value);
            var max = validate();
            if (max == 0) max++;
            nextInvoiceNumber = nextInvoiceNumber >= max ? nextInvoiceNumber : max + 5;
            var invoiceNumber = nextInvoiceNumber + invoicesCount;

            setting.Value = invoiceNumber.ToString(CultureInfo.InvariantCulture);

            PersistUpdatedItem(setting); // this will deal with the cache as well

            return invoiceNumber;
        }

        /// <summary>
        /// Gets the next order number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextOrderNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="ordersCount">The number of orders needing invoice orders.  Useful when saving multiple new orders.</param>
        /// <returns>The next order number</returns>
        public int GetNextOrderNumber(Guid storeSettingKey, int domainRootStructureID, Func<int> validate, int ordersCount = 1)
        {
            Mandate.ParameterCondition(1 >= ordersCount, "ordersCount");

            var setting = Get(storeSettingKey, domainRootStructureID);
            if (string.IsNullOrEmpty(setting.Value)) setting.Value = "1";
            var max = validate();
            if (max == 0) max++;
            var nextOrderNumber = int.Parse(setting.Value);
            nextOrderNumber = nextOrderNumber >= max ? nextOrderNumber : max + 5;
            var orderNumber = nextOrderNumber + ordersCount;

            setting.Value = orderNumber.ToString(CultureInfo.InvariantCulture);

            PersistUpdatedItem(setting);

            return orderNumber;
        }

        /// <summary>
        /// Gets the next shipment number
        /// </summary>
        /// <param name="storeSettingKey">Constant GUID Key of the NextShipmentNumber store setting</param>
        /// <param name="validate">Function to execute to validate the next number</param>
        /// <param name="shipmentsCount">The number of shipments needing invoice orders.  Useful when saving multiple new shipments.</param>
        /// <returns>The next shipment number</returns>
        public int GetNextShipmentNumber(Guid storeSettingKey, int domainRootStructureID, Func<int> validate, int shipmentsCount = 1)
        {
            Mandate.ParameterCondition(1 >= shipmentsCount, "shipmentsCount");

            var setting = Get(storeSettingKey, domainRootStructureID);
            if (string.IsNullOrEmpty(setting.Value)) setting.Value = "1";
            var max = validate();
            if (max == 0) max++;
            var nextShipmentNumber = int.Parse(setting.Value);
            nextShipmentNumber = nextShipmentNumber >= max ? nextShipmentNumber : max + 5;
            var shipmentNumber = nextShipmentNumber + shipmentsCount;

            setting.Value = shipmentNumber.ToString(CultureInfo.InvariantCulture);

            PersistUpdatedItem(setting);

            return shipmentNumber;
        }

        /// <summary>
        /// The get type fields.
        /// </summary>
        /// <returns>
        /// The collection of all type fields.
        /// </returns>
        public IEnumerable<ITypeField> GetTypeFields()
        {
            var dtos = Database.Fetch<TypeFieldDto>("SELECT * FROM merchTypeField");
            return dtos.Select(dto => new TypeField(dto.Alias, dto.Name, dto.Key));
        }

        protected override IStoreSetting PerformGet(Guid key, int domainRootStructureID)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseMSWhereClause(), new { Key = key, domainRootStructureID });

            var dto = Database.Fetch<StoreSettingDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new StoreSettingFactory();

            var setting = factory.BuildEntity(dto);

            return setting;
        }

        protected override IEnumerable<IStoreSetting> PerformGetAll(int domainRootStructureID, params Guid[] keys)
        {
            var dtos = new List<StoreSettingDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<StoreSettingDto>(GetBaseQuery(false)
                        .Where("domainRootStructureID = @domainRootStructureID", new { domainRootStructureID })
                        .WhereIn<StoreSettingDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<StoreSettingDto>(GetBaseQuery(false)
                    .Where("domainRootStructureID = @domainRootStructureID", new { domainRootStructureID }));
            }

            var factory = new StoreSettingFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// Gets a <see cref="IStoreSetting"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IStoreSetting"/>.
        /// </returns>
        protected override IStoreSetting PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<StoreSettingDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new StoreSettingFactory();

            var setting = factory.BuildEntity(dto);

            return setting;
        }

        /// <summary>
        /// Gets a collection of all <see cref="IStoreSetting"/>
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStoreSetting}"/>.
        /// </returns>
        protected override IEnumerable<IStoreSetting> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<StoreSettingDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<StoreSettingDto>(GetBaseQuery(false).WhereIn<StoreSettingDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<StoreSettingDto>(GetBaseQuery(false));
            }

            var factory = new StoreSettingFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// A collection of <see cref="IStoreSetting"/> by query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStoreSetting}"/>.
        /// </returns>
        protected override IEnumerable<IStoreSetting> PerformGetByQuery(IQuery<IStoreSetting> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IStoreSetting>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<StoreSettingDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL query
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
                .From<StoreSettingDto>(SqlSyntax);

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
            return "merchStoreSetting.pk = @Key";
        }

        protected override string GetBaseMSWhereClause()
        {
            return "merchStoreSetting.pk = @Key AND domainRootStructureID = @domainRootStructureID";
        }

        /// <summary>
        /// Gets the collection of delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchStoreSetting WHERE pk = @Key AND domainRootStructureID = @DomainRootStructureID"
            };

            return list;
        }

        /// <summary>
        /// Persists a new <see cref="IStoreSetting"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IStoreSetting entity)
        {

            ((Entity)entity).AddingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Persist an updated <see cref="IStoreSetting"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IStoreSetting entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            //Database.Update(dto);
            Database.Delete<StoreSettingDto>("WHERE pk = @Key AND domainRootStructureID = @DomainRootStructureID", new { dto.Key, dto.DomainRootStructureID });
            Database.Insert(dto);

            //We need to do a special InsertOrUpdate here because we know that the PreviewXmlDto table has a composite key and thus
            // a unique constraint which can be violated if 2+ threads try to execute the same insert sql at the same time.
            //Database.Update(
            //    //Since the table has a composite key, we need to specify an explit update statement
            //    "SET name = @Name, value = @Value, typeName = @TypeName, updateDate = @UpdateDate WHERE pk=@Key AND domainRootStructureID=@DomainRootStructureID",
            //    new
            //    {
            //        dto.Name,
            //        dto.Value,
            //        dto.TypeName,
            //        dto.UpdateDate,
            //        dto.Key,
            //        dto.DomainRootStructureID
            //    });

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IStoreSetting entity)
        {
            Database.Delete<StoreSettingDto>("WHERE pk = @Key AND domainRootStructureID = @DomainRootStructureID", new { entity.Key, entity.DomainRootStructureID });
            entity.ResetDirtyProperties();
        }
    }
}