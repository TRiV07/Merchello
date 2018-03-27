using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroTwo
{
    /// <summary>
    /// Updates Warehouse table
    /// </summary>
    [Migration("1.0.2", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateWarehouseTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateWarehouseTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchWarehouse"))
            {
                Alter.Table("merchWarehouse").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(-1);

                this.Execute.Code(database =>
                {
                    var domainRootStructureIDs = database.Query<int>("SELECT DISTINCT [domainRootStructureID] FROM umbracoDomains").ToList();

                    foreach (var id in domainRootStructureIDs)
                    {
                        try
                        {
                            Guid warehouseKey = Guid.NewGuid();
                            database.Insert("merchWarehouse", "Key", new WarehouseDto() { Key = warehouseKey, StoreId = id, Name = "Default Warehouse", CountryCode = string.Empty, IsDefault = true, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

                            database.Insert("merchWarehouseCatalog", "Key", new WarehouseCatalogDto() { Key = Guid.NewGuid(), WarehouseKey = warehouseKey, Name = "Default Catalog", Description = null, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
                        }
                        catch { }
                    }

                    return string.Empty;
                });
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchWarehouse"))
            {
                Delete.Column("storeId").FromTable("merchWarehouse");
            }
        }
    }
}
