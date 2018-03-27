namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Models.EntityBase;
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The store setting dto.
    /// </summary>
    [TableName("merchStoreSetting")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class StoreSettingDto : IHasDomainRoot
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false, Clustered = true, Name = "PK_merchStoreSetting", OnColumns = "pk, storeId")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets domain root structure ID
        /// </summary>
        [Column("storeId")]
        [Constraint(Default = "-1")]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Column("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        [Column("typeName")]
        public string TypeName { get; set; }

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