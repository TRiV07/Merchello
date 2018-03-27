using Merchello.Core.Configuration;
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
    [Migration("1.0.1", 0, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateProductTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateProductTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchProduct"))
            {
                Alter.Table("merchProduct").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(-1);
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchProduct"))
            {
                Delete.Column("storeId").FromTable("merchProduct");
            }
        }
    }
}
