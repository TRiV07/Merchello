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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroFour
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.0.4", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateInvoiceTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateInvoiceTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchInvoice"))
            {
                Alter.Table("merchInvoice").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(Constants.MultiStore.DefaultId);

                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchInvoiceNumber")) == true)
                    Delete.Index("IX_merchInvoiceNumber").OnTable("merchInvoice");

                Create.Index("IX_merchInvoiceNumber")
                    .OnTable("merchInvoice")
                    .OnColumn("invoiceNumber")
                    .Ascending()
                    .OnColumn("storeId")
                    .Ascending()
                    .WithOptions()
                    .Unique();
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchInvoice"))
            {
                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchInvoiceNumber")) == true)
                    Delete.Index("IX_merchInvoiceNumber").OnTable("merchInvoice");

                Delete.Column("storeId").FromTable("merchInvoice");
            }
        }
    }
}
