namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Models.EntityBase;
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The invoice dto.
    /// </summary>
    [TableName("merchStore")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class StoreDto : IHasDomainRoot
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
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchStoreId", ForColumns = "storeId")]
        public int StoreId { get; set; }

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