using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.DatabaseModelDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroZero
{
    /// <summary>
    /// Updates store settings table
    /// </summary>
    [Migration("1.0.0", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateStoreSetting : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateStoreSetting(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchStoreSetting"))
            {
                Alter.Table("merchStoreSetting").AddColumn("domainRootStructureID").AsInt32().NotNullable().WithDefaultValue(-1);

                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("PK_merchStoreSetting")) == true)
                    Delete.PrimaryKey("PK_merchStoreSetting").FromTable("merchStoreSetting");

                Create.PrimaryKey("PK_merchStoreSetting").OnTable("merchStoreSetting")
                    .Columns(new string[] { "pk", "domainRootStructureID" });

                this.Execute.Code(database =>
                {
                    var domainRootStructureIDs = database.Query<int>("SELECT DISTINCT [domainRootStructureID] FROM umbracoDomains").ToList();

                    foreach (var id in domainRootStructureIDs)
                    {
                        try
                        {
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.CurrencyCodeKey, Name = "currencyCode", Value = "USD", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.NextOrderNumberKey, Name = "nextOrderNumber", Value = "1", TypeName = "System.Int32", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.NextInvoiceNumberKey, Name = "nextInvoiceNumber", Value = "1", TypeName = "System.Int32", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.NextShipmentNumberKey, Name = "nextShipmentNumber", Value = "1", TypeName = "System.Int32", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.DateFormatKey, Name = "dateFormat", Value = "dd-MM-yyyy", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.TimeFormatKey, Name = "timeFormat", Value = "am-pm", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.UnitSystemKey, Name = "unitSystem", Value = "Imperial", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.GlobalShippableKey, Name = "globalShippable", Value = "true", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.GlobalTaxableKey, Name = "globalTaxable", Value = "true", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.GlobalTrackInventoryKey, Name = "globalTrackInventory", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.GlobalShippingIsTaxableKey, Name = "globalShippingIsTaxable", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.MigrationKey, Name = "migration", Value = Guid.NewGuid().ToString(), TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSetting.GlobalTaxationApplicationKey, Name = "globalTaxationApplication", Value = "Invoice", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSetting.DefaultExtendedContentCulture, Name = "defaultExtendedContentCulture", Value = "en-US", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSetting.HasDomainRecordKey, Name = "hasDomainRecord", Value = false.ToString(), TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, DomainRootStructureID = id });
                        }
                        catch { }
                    }

                    return string.Empty;
                });

                
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchStoreSetting"))
            {
                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("PK_merchStoreSetting")) == true)
                    Delete.PrimaryKey("PK_merchStoreSetting").FromTable("merchStoreSetting");

                Create.PrimaryKey("PK_merchStoreSetting").OnTable("merchStoreSetting")
                    .Column("pk");

                Delete.Column("domainRootStructureID").FromTable("merchStoreSetting");
            }
        }
    }
}
