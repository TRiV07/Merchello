using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroTwo
{
    /// <summary>
    /// Updates EntityCollection table
    /// </summary>
    [Migration("1.0.2", 0, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateEntityCollectionTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateEntityCollectionTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchEntityCollection"))
            {
                Alter.Table("merchEntityCollection").AddColumn("domainRootStructureID").AsInt32().NotNullable().WithDefaultValue(-1);
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchEntityCollection"))
            {
                Delete.Column("domainRootStructureID").FromTable("merchEntityCollection");
            }
        }
    }
}
