namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Models.EntityBase;
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The Carrier dto.
    /// </summary>
    [TableName("merchCarrier")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class CarrierDto : IPageableDto, IHasDomainRoot
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
        [ForeignKey(typeof(StoreDto), Name = "FK_merchCarrier_merchStore", Column = "storeId")]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        [Column("loginName")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchCarrierLoginName", ForColumns = "loginName, storeId")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Column("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Column("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        [Column("isDisabled")]
        public bool IsDisabled { get; set; }

        [Column("isOnDuty")]
        public bool IsOnDuty { get; set; }

        [Column("activeOrders")]
        public int ActiveOrders { get; set; }

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