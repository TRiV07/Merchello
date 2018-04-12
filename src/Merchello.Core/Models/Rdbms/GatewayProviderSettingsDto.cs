using Merchello.Core.Models.EntityBase;
using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchGatewayProviderSettings")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class GatewayProviderSettingsDto : IHasDomainRoot
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false, Clustered = true, Name = "PK_merchGatewayProviderSettings", OnColumns = "pk, storeId")]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("storeId")]
        [Constraint(Default = Constants.MultiStore.DefaultId)]
        public int StoreId { get; set; }

        [Column("providerTfKey")]
        public Guid ProviderTfKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        [Column("encryptExtendedData")]
        [Constraint(Default = "0")]
        public bool EncryptExtendedData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}