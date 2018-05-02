namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    internal sealed class CarrierMapper : MerchelloBaseMapper
    {
        ////NOTE: its an internal class but the ctor must be public since we're using Activator.CreateInstance to create it
        //// otherwise that would fail because there is no public constructor.
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerMapper"/> class.
        /// </summary>
        public CarrierMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper

        /// <summary>
        /// Maps the entities
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Carrier, CarrierDto>(src => src.Key, dto => dto.Key);
            CacheMap<Carrier, CarrierDto>(src => src.StoreId, dto => dto.StoreId);
            CacheMap<Carrier, CarrierDto>(src => src.FirstName, dto => dto.FirstName);
            CacheMap<Carrier, CarrierDto>(src => src.LastName, dto => dto.LastName);
            CacheMap<Carrier, CarrierDto>(src => src.Email, dto => dto.Email);
            CacheMap<Carrier, CarrierDto>(src => src.LoginName, dto => dto.LoginName);
            CacheMap<Carrier, CarrierDto>(src => src.IsDisabled, dto => dto.IsDisabled);
            CacheMap<Carrier, CarrierDto>(src => src.IsOnDuty, dto => dto.IsOnDuty);
            CacheMap<Carrier, CarrierDto>(src => src.ActiveOrders, dto => dto.ActiveOrders);
            CacheMap<Carrier, CarrierDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Carrier, CarrierDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }

        #endregion
    }
}
