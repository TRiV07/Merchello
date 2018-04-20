namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a DigitalMediaService.
    /// </summary>
    public interface IStoreService : IService
    {
        IStore CreateStoreWithKey(int storeId, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IStore"/>
        /// </summary>
        /// <param name="store">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IStore store, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IStore"/>.
        /// </summary>
        /// <param name="stores">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IEnumerable<IStore> stores, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IStore"/> from the database.
        /// </summary>
        /// <param name="store">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Delete(IStore store, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IStore"/> from the database.
        /// </summary>
        /// <param name="stores">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Delete(IEnumerable<IStore> stores, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IStore"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IStore"/>.
        /// </returns>
        IStore GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="IStore"/> by it's unique storeId.
        /// </summary>
        /// <param name="storeId">
        /// The store Id.
        /// </param>
        /// <returns>
        /// The <see cref="IStore"/>.
        /// </returns>
        IStore GetById(int storeId);

        /// <summary>
        /// Gets a collection of <see cref="IStore"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStore}"/>.
        /// </returns>
        IEnumerable<IStore> GetByKeys(IEnumerable<Guid> keys);

        IEnumerable<IStore> GetAll();

        //IEnumerable<int> GetAllStoresIds();
    }
}