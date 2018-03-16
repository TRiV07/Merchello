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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroOne
{
    /// <summary>
    /// Updates product table
    /// </summary>
    [Migration("1.0.1", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateProductVariantTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateProductVariantTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchProductVariant"))
            {
                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchProductVariantSku")) == true)
                    Delete.Index("IX_merchProductVariantSku");

                Create.Index("IX_merchProductVariantSku")
                    .OnTable("merchProductVariant")
                    .OnColumn("sku")
                    .Ascending()
                    .WithOptions()
                    .NonClustered();
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchProduct"))
            {
                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchProductVariantSku")) == true)
                    Delete.Index("IX_merchProductVariantSku");
            }
        }
    }
}
