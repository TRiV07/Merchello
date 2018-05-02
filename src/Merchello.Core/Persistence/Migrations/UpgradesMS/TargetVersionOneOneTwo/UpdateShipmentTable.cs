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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneOneTwo
{
    [Migration("1.1.2", 2, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateShipmentTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateShipmentTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchShipment"))
            {
                var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();
                var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();

                if (columns.Any(x => x.TableName.InvariantEquals("merchShipment") && x.ColumnName.InvariantEquals("carrierKey")) == false)
                    Alter.Table("merchShipment").AddColumn("carrierKey").AsGuid().Nullable();

                if (constraints.Any(x => x.Item1.InvariantEquals("merchShipment")
                        && x.Item3.InvariantEquals("FK_merchShipment_merchCarrier")) == false)
                {
                    Create.ForeignKey("FK_merchShipment_merchCarrier")
                        .FromTable("merchShipment")
                        .ForeignColumn("carrierKey")
                        .ToTable("merchCarrier")
                        .PrimaryColumn("pk");
                }
            }
        }

        public override void Down()
        {

        }
    }
}
