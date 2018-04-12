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
    [Migration("1.0.5", 0, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class DeleteGatewayProviderSettingsForeignKeys : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public DeleteGatewayProviderSettingsForeignKeys(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            //var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToList();
            var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();

            if (tables.InvariantContains("merchNotificationMethod"))
            {
                if (constraints.Any(x => x.Item1.InvariantEquals("merchNotificationMethod")
                    && x.Item3.InvariantEquals("FK_merchNotificationMethod_merchGatewayProvider")))
                    Delete.ForeignKey("FK_merchNotificationMethod_merchGatewayProvider").OnTable("merchNotificationMethod");
            }

            if (tables.InvariantContains("merchPaymentMethod"))
            {
                if (constraints.Any(x => x.Item1.InvariantEquals("merchPaymentMethod")
                    && x.Item3.InvariantEquals("FK_merchPaymentMethod_merchGatewayProviderSettings")))
                    Delete.ForeignKey("FK_merchPaymentMethod_merchGatewayProviderSettings").OnTable("merchPaymentMethod");
            }

            if (tables.InvariantContains("merchShipMethod"))
            {
                if (constraints.Any(x => x.Item1.InvariantEquals("merchShipMethod")
                    && x.Item3.InvariantEquals("FK_merchShipMethod_merchGatewayProviderSettings")))
                    Delete.ForeignKey("FK_merchShipMethod_merchGatewayProviderSettings").OnTable("merchShipMethod");
            }

            if (tables.InvariantContains("merchTaxMethod"))
            {
                if (constraints.Any(x => x.Item1.InvariantEquals("merchTaxMethod")
                    && x.Item3.InvariantEquals("FK_merchTaxMethod_merchGatewayProviderSettings")))
                    Delete.ForeignKey("FK_merchTaxMethod_merchGatewayProviderSettings").OnTable("merchTaxMethod");
            }
        }

        public override void Down()
        {

        }
    }
}
