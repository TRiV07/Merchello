using Merchello.Core.Configuration;
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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroSix
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.0.6", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateOfferSettingsTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateOfferSettingsTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchOfferSettings"))
            {
                var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();

                if (columns.Any(x => x.TableName.InvariantEquals("merchOfferSettings") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchOfferSettings").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchOfferSettingsOfferCode")) == true)
                    Delete.Index("IX_merchOfferSettingsOfferCode").OnTable("merchOfferSettings");

                Create.Index("IX_merchOfferSettingsOfferCode")
                    .OnTable("merchOfferSettings")
                    .OnColumn("offerCode")
                    .Ascending()
                    .OnColumn("storeId")
                    .Ascending()
                    .WithOptions()
                    .Unique();
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchOfferSettings"))
            {
                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchOfferSettingsOfferCode")) == true)
                    Delete.Index("IX_merchOfferSettingsOfferCode").OnTable("merchOfferSettings");

                Delete.Column("storeId").FromTable("merchOfferSettings");
            }
        }
    }
}
