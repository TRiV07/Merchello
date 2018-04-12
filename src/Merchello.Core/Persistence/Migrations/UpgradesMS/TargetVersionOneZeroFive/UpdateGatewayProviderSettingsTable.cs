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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroFive
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.0.5", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateGatewayProviderSettingsTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateGatewayProviderSettingsTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchGatewayProviderSettings"))
            {
                var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();
                var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();

                if (columns.Any(x => x.TableName.InvariantEquals("merchGatewayProviderSettings") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchGatewayProviderSettings").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                if (constraints.Any(x => x.Item1.InvariantEquals("merchGatewayProviderSettings")
                    && x.Item3.InvariantEquals("PK_merchGatewayProviderSettings")))
                    Delete.PrimaryKey("PK_merchGatewayProviderSettings").FromTable("merchGatewayProviderSettings");

                Create.PrimaryKey("PK_merchGatewayProviderSettings").OnTable("merchGatewayProviderSettings")
                    .Columns(new string[] { "pk", "storeId" });
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchGatewayProviderSettings"))
            {
                var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();
                var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();

                if (constraints.Any(x => x.Item1.InvariantEquals("merchGatewayProviderSettings")
                    && x.Item3.InvariantEquals("PK_merchGatewayProviderSettings")))
                    Delete.PrimaryKey("PK_merchGatewayProviderSettings").FromTable("merchGatewayProviderSettings");

                Create.PrimaryKey("PK_merchGatewayProviderSettings").OnTable("merchGatewayProviderSettings")
                    .Column("pk");

                if (columns.Any(x => x.TableName.InvariantEquals("merchGatewayProviderSettings") && x.ColumnName.InvariantEquals("storeId")) == true)
                    Delete.Column("storeId").FromTable("merchGatewayProviderSettings");
            }
        }
    }
}
