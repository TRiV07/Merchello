namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Models.EntityBase;
    using System;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The key dto.
    /// </summary>
    public class KeyStoreDto : IHasDomainRoot
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets domain root structure ID
        /// </summary>
        [Column("storeId")]
        public int StoreId { get; set; }
    }
}
