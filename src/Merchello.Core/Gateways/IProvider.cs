namespace Merchello.Core.Gateways
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Marker interface for Providers 
    /// </summary>
    public interface IProvider : IHasDomainRoot, IHasExtendedData
    {
        /// <summary>
        /// Gets the unique key for the gateway.  
        /// Used by Merchello in the GatewayProvider's installation/configuration
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IGatewayResource"/>
        /// </returns>
        IEnumerable<IGatewayResource> ListResourcesOffered();
    }
}