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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneOneZero
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.1.0", 1, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class CreateStoreTable : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public CreateStoreTable(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            if (!_databaseSchemaHelper.TableExist("merchStore"))
            {
                _databaseSchemaHelper.CreateTable<StoreDto>(false);

                this.Execute.Code(database =>
                {
                    var domainRootStructureIDs = database.Query<int>("SELECT DISTINCT [domainRootStructureID] FROM umbracoDomains").ToList();

                    foreach (var id in domainRootStructureIDs)
                    {
                        database.Insert("merchStore", "Key", new StoreDto() { Key = Guid.NewGuid(), StoreId = id, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
                    }

                    return string.Empty;
                });
            }
        }

        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchStore"))
            {
                _databaseSchemaHelper.DropTable<StoreDto>();
            }
        }
    }
}
