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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroZero
{
    /// <summary>
    /// Updates store settings table
    /// </summary>
    [Migration("1.0.0", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateStoreSetting : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateStoreSetting(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchStoreSetting"))
            {
                Alter.Table("merchStoreSetting").AddColumn("domainRootStructureID").AsInt32().NotNullable().WithDefaultValue(0);

                //Delete.Index("PK_merchStoreSetting").OnTable("merchStoreSetting");
                Delete.PrimaryKey("PK_merchStoreSetting").FromTable("merchStoreSetting");


                Create.PrimaryKey("PK_merchStoreSetting").OnTable("merchStoreSetting")
                    .Columns(new string[] { "pk", "domainRootStructureID" });

                //Create.Index("IX_merchStoreSetting").OnTable("merchStoreSetting")
                //    .OnColumn("pk").Ascending()
                //    .OnColumn("domainRootStructureID").Ascending()
                //    .WithOptions()
                //    .Unique();

                //this.Alter.Table("domainRootStructureID")..AlterColumn("domainRootStructureID").
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchStoreSetting"))
                this.Delete.Column("domainRootStructureID").FromTable("merchStoreSetting");
        }
    }
}
