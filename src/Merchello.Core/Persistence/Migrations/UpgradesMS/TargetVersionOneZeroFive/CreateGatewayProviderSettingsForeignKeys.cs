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
    [Migration("1.0.5", 2, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class CreateGatewayProviderSettingsForeignKeys : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public CreateGatewayProviderSettingsForeignKeys(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToList();
            var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();

            if (tables.InvariantContains("merchNotificationMethod"))
            {
                if (columns.Any(x => x.TableName.InvariantEquals("merchNotificationMethod") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchNotificationMethod").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                this.Execute.Code(database =>
                {
                    database.Execute("ALTER TABLE [merchNotificationMethod] ADD CONSTRAINT [FK_merchNotificationMethod_merchGatewayProvider] FOREIGN KEY ([providerKey], [storeId]) REFERENCES [merchGatewayProviderSettings] ([pk], [storeId])");
                    return string.Empty;
                });
            }

            if (tables.InvariantContains("merchPaymentMethod"))
            {
                if (columns.Any(x => x.TableName.InvariantEquals("merchPaymentMethod") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchPaymentMethod").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                this.Execute.Code(database =>
                {
                    database.Execute("ALTER TABLE [merchPaymentMethod] ADD CONSTRAINT [FK_merchPaymentMethod_merchGatewayProviderSettings] FOREIGN KEY ([providerKey], [storeId]) REFERENCES [merchGatewayProviderSettings] ([pk], [storeId])");
                    return string.Empty;
                });
            }

            if (tables.InvariantContains("merchShipMethod"))
            {
                if (columns.Any(x => x.TableName.InvariantEquals("merchShipMethod") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchShipMethod").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                this.Execute.Code(database =>
                {
                    database.Execute("ALTER TABLE [merchShipMethod] ADD CONSTRAINT [FK_merchShipMethod_merchGatewayProviderSettings] FOREIGN KEY ([providerKey], [storeId]) REFERENCES [merchGatewayProviderSettings] ([pk], [storeId])");
                    return string.Empty;
                });
            }

            if (tables.InvariantContains("merchTaxMethod"))
            {
                if (columns.Any(x => x.TableName.InvariantEquals("merchTaxMethod") && x.ColumnName.InvariantEquals("storeId")) == false)
                    Alter.Table("merchTaxMethod").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                this.Execute.Code(database =>
                {
                    database.Execute("ALTER TABLE [merchTaxMethod] ADD CONSTRAINT [FK_merchTaxMethod_merchGatewayProviderSettings] FOREIGN KEY ([providerKey], [storeId]) REFERENCES [merchGatewayProviderSettings] ([pk], [storeId])");
                    return string.Empty;
                });
            }
        }

        public override void Down()
        {
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
    }
}
