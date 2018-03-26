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
    /// Updates Anonymous Customer table
    /// </summary>
    [Migration("1.0.3", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateAnonymousCustomerTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateAnonymousCustomerTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchAnonymousCustomer"))
            {
                Alter.Table("merchAnonymousCustomer").AddColumn("domainRootStructureID").AsInt32().NotNullable().WithDefaultValue(-1);
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchAnonymousCustomer"))
            {
                Delete.Column("domainRootStructureID").FromTable("merchAnonymousCustomer");
            }
        }
    }
}
