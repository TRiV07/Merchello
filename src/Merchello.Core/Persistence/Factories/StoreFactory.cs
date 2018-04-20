namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The store factory.
    /// </summary>
    internal class StoreFactory : IEntityFactory<IStore, StoreDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IStore"/>.
        /// </returns>
        public IStore BuildEntity(StoreDto dto)
        {
            var entity = new Store(dto.StoreId);
            entity.ResetDirtyProperties();
            return entity;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="StoreDto"/>.
        /// </returns>
        public StoreDto BuildDto(IStore entity)
        {           
            return new StoreDto()
            {
                Key = entity.Key,
                StoreId = entity.StoreId,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}