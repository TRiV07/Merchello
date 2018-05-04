namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the shipiment repository
    /// </summary>
    internal interface IShipmentRepository : IRepositoryQueryable<Guid, IShipment>, IAssertsMaxDocumentNumber
    {
        IEnumerable<IShipment> GetShipmentsByCustomer(Guid customer);
        IEnumerable<IShipment> GetShipmentsByCarrierKeyAndStatus(Guid carrierKey, params Guid[] statusKeys);
    }
}
