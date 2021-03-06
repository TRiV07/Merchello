﻿namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Models.EntityBase;
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The anonymous customer dto.
    /// </summary>
    [TableName("merchAnonymousCustomer")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AnonymousCustomerDto : IHasDomainRoot
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets domain root structure ID
        /// </summary>
        [Column("storeId")]
        [Constraint(Default = Constants.MultiStore.DefaultId)]
        [ForeignKey(typeof(StoreDto), Name = "FK_merchAnonymousCustomer_merchStore", Column = "storeId")]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        [Column("lastActivityDate")]
        [Constraint(Default = "getdate()")]
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
