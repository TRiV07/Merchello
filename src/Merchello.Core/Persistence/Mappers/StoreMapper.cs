namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="Store"/> to DTO mapper used to translate the properties of the public API 
    /// implementation to that of the database's DTO as SQL: [tableName].[columnName].
    /// </summary>
    internal sealed class StoreMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreMapper"/> class.
        /// </summary>
        public StoreMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// Maps a store to the store DTO.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Store, StoreDto>(src => src.Key, dto => dto.Key);
            CacheMap<Store, StoreDto>(src => src.StoreId, dto => dto.StoreId);
            CacheMap<Store, StoreDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<Store, StoreDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}