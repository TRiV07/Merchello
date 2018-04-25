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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneOneOne
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.1.1", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class CreateStoreForeignKeys : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public CreateStoreForeignKeys(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToList();
            var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();
            var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();

            if (tables.InvariantContains("merchStore"))
            {
                if (columns.Any(x => x.TableName.InvariantEquals("merchGatewayProviderSettings")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchGatewayProviderSettings")
                        && x.Item3.InvariantEquals("FK_merchGatewayProviderSettings_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchGatewayProviderSettings_merchStore")
                        .FromTable("merchGatewayProviderSettings")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchWarehouse")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchWarehouse")
                        && x.Item3.InvariantEquals("FK_merchWarehouse_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchWarehouse_merchStore")
                        .FromTable("merchWarehouse")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchTaxMethod")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchTaxMethod")
                        && x.Item3.InvariantEquals("FK_merchTaxMethod_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchTaxMethod_merchStore")
                        .FromTable("merchTaxMethod")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchShipMethod")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchShipMethod")
                        && x.Item3.InvariantEquals("FK_merchShipMethod_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchShipMethod_merchStore")
                        .FromTable("merchShipMethod")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchInvoice")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchInvoice")
                        && x.Item3.InvariantEquals("FK_merchInvoice_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchInvoice_merchStore")
                        .FromTable("merchInvoice")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchStoreSetting")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchStoreSetting")
                        && x.Item3.InvariantEquals("FK_merchStoreSetting_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchStoreSetting_merchStore")
                        .FromTable("merchStoreSetting")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchNotificationMethod")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchNotificationMethod")
                        && x.Item3.InvariantEquals("FK_merchNotificationMethod_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchNotificationMethod_merchStore")
                        .FromTable("merchNotificationMethod")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchOfferSettings")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchOfferSettings")
                        && x.Item3.InvariantEquals("FK_merchOfferSettings_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchOfferSettings_merchStore")
                        .FromTable("merchOfferSettings")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchShipment")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchShipment")
                        && x.Item3.InvariantEquals("FK_merchShipment_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchShipment_merchStore")
                        .FromTable("merchShipment")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchEntityCollection")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchEntityCollection")
                        && x.Item3.InvariantEquals("FK_merchEntityCollection_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchEntityCollection_merchStore")
                        .FromTable("merchEntityCollection")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchPaymentMethod")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchPaymentMethod")
                        && x.Item3.InvariantEquals("FK_merchPaymentMethod_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchPaymentMethod_merchStore")
                        .FromTable("merchPaymentMethod")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchAnonymousCustomer")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchAnonymousCustomer")
                        && x.Item3.InvariantEquals("FK_merchAnonymousCustomer_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchAnonymousCustomer_merchStore")
                        .FromTable("merchAnonymousCustomer")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchCustomer")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchCustomer")
                        && x.Item3.InvariantEquals("FK_merchCustomer_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchCustomer_merchStore")
                        .FromTable("merchCustomer")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }


                if (columns.Any(x => x.TableName.InvariantEquals("merchProduct")
                        && x.ColumnName.InvariantEquals("storeId")) == true
                    && constraints.Any(x => x.Item1.InvariantEquals("merchProduct")
                        && x.Item3.InvariantEquals("FK_merchProduct_merchStore")) == false)
                {
                    Create.ForeignKey("FK_merchProduct_merchStore")
                        .FromTable("merchProduct")
                        .ForeignColumn("storeId")
                        .ToTable("merchStore")
                        .PrimaryColumn("storeId");
                }
            }
        }

        public override void Down()
        {

        }
    }
}
