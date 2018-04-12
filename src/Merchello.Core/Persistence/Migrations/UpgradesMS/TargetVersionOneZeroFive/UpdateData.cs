using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
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

namespace Merchello.Core.Persistence.Migrations.UpgradesMS.TargetVersionOneZeroFive
{
    /// <summary>
    /// Updates Invoice table
    /// </summary>
    [Migration("1.0.5", 3, MerchelloConfiguration.MerchelloMSMigrationName)]
    public class UpdateData : MigrationBase
    {
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public UpdateData(ISqlSyntaxProvider sqlSyntax, Umbraco.Core.Logging.ILogger logger) : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        public override void Up()
        {
            this.Execute.Code(database =>
            {
                var domainRootStructureIDs = database.Query<int>("SELECT DISTINCT [domainRootStructureID] FROM umbracoDomains").ToList();

                foreach (var id in domainRootStructureIDs)
                {
                    database.Insert("merchGatewayProviderSettings", "Key", new GatewayProviderSettingsDto() { Key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey, StoreId = id, Name = "Fixed Rate Shipping Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Shipping).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

                    database.Insert("merchGatewayProviderSettings", "Key", new GatewayProviderSettingsDto() { Key = Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey, StoreId = id, Name = "Fixed Rate Tax Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Taxation).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });


                    database.Insert("merchGatewayProviderSettings", "Key", new GatewayProviderSettingsDto() { Key = Constants.ProviderKeys.Payment.CashPaymentProviderKey, StoreId = id, Name = "Cash Payment Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Payment).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

                    database.Insert(
                        "merchPaymentMethod",
                        "Key",
                        new PaymentMethodDto()
                        {
                            Key = Guid.NewGuid(),
                            StoreId = id,
                            Name = "Cash",
                            PaymentCode = "Cash",
                            Description = "Cash Payment",
                            ProviderKey = Constants.ProviderKeys.Payment.CashPaymentProviderKey,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });
                }

                return string.Empty;
            });
        }

        public override void Down()
        {

        }
    }
}
