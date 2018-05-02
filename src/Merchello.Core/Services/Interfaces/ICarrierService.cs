namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the CarrierService, which provides access to operations involving <see cref="ICarrier"/>
    /// </summary>
    public interface ICarrierService : IPageCachedMSService<ICarrier>
    {
        /// <summary>
        /// Creates a Carrier without saving to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the Carrier
        /// </param>
        /// <param name="lastName">
        /// The last name of the Carrier
        /// </param>
        /// <param name="email">
        /// the email address of the Carrier
        /// </param>
        /// <returns>
        /// The new <see cref="ICarrier"/>
        /// </returns>
        ICarrier CreateCarrier(string loginName, int storeId, string firstName, string lastName, string email);

        /// <summary>
        /// Creates a Carrier and saves the record to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the Carrier
        /// </param>
        /// <param name="lastName">
        /// The last name of the Carrier
        /// </param>
        /// <param name="email">
        /// the email address of the Carrier
        /// </param>
        /// <returns>
        /// <see cref="ICarrier"/>
        /// </returns>
        ICarrier CreateCarrierWithKey(string loginName, int storeId, string firstName, string lastName, string email);

        /// <summary>
        /// Creates a Carrier with the Umbraco member id passed
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// <see cref="ICarrier"/>
        /// </returns>
        ICarrier CreateCarrierWithKey(string loginName, int storeId);

        /// <summary>
        /// Saves a single <see cref="ICarrier"/> object
        /// </summary>
        /// <param name="carrier">The <see cref="ICarrier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICarrier carrier, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICarrier"/> objects
        /// </summary>
        /// <param name="carriers">The collection of Carriers to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<ICarrier> carriers, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICarrier"/> object
        /// </summary>
        /// <param name="carrier"><see cref="ICarrier"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICarrier carrier, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="ICarrier"/> objects
        /// </summary>
        /// <param name="carriers">Collection of <see cref="ICarrier"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICarrier> carriers, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICarrier"/> object by its Umbraco login name
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// <see cref="ICarrier"/> object or null if not found
        /// </returns>
        ICarrier GetByLoginName(string loginName, int storeId);

        /// <summary>
        /// Gets list of <see cref="ICarrier"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of GUID keys for Carriers to retrieve</param>
        /// <returns>List of <see cref="ICarrier"/></returns>
        IEnumerable<ICarrier> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets the total Carrier count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CarrierCount(int storeId);
    }
}
