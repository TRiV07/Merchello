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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroThree
{
    /// <summary>
    /// Updates Customer table
    /// </summary>
    [Migration("1.0.3", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateCustomerTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateCustomerTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchCustomer"))
            {
                Alter.Table("merchCustomer").AddColumn("storeId").AsInt32().NotNullable().WithDefaultValue(-1);

                var dbIndexes = SqlSyntax.GetDefinedIndexesDefinitions(Context.Database);
                if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchCustomerLoginName")) == true)
                    Delete.Index("IX_merchCustomerLoginName").OnTable("merchCustomer");

                Create.Index("IX_merchCustomerLoginName")
                    .OnTable("merchCustomer")
                    .OnColumn("loginName")
                    .Ascending()
                    .WithOptions()
                    .NonClustered();
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchCustomer"))
            {
                Delete.Column("storeId").FromTable("merchCustomer");
            }
        }
    }
}
