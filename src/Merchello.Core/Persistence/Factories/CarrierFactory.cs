namespace Merchello.Core.Persistence.Factories
{
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The Carrier factory.
    /// </summary>
    internal class CarrierFactory : IEntityFactory<ICarrier, CarrierDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ICarrier"/>.
        /// </returns>
        public ICarrier BuildEntity(CarrierDto dto)
        {
            var carrier = new Carrier(dto.LoginName, dto.StoreId)
            {
                Key = dto.Key,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IsDisabled = dto.IsDisabled,
                IsOnDuty = dto.IsOnDuty,
                ActiveOrders = dto.ActiveOrders,
                ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate
            };

            carrier.ResetDirtyProperties();

            return carrier;
        }

        /// <summary>
        /// Build the dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="CarrierDto"/>.
        /// </returns>
        public CarrierDto BuildDto(ICarrier entity)
        {
            var dto = new CarrierDto()
            {
                Key = entity.Key,
                StoreId = entity.StoreId,
                LoginName = entity.LoginName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                IsDisabled = entity.IsDisabled,
                IsOnDuty = entity.IsOnDuty,
                ActiveOrders = entity.ActiveOrders,
                LastActivityDate = entity.LastActivityDate,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
