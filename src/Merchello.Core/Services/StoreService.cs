namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a <see cref="IStoreService"/>.
    /// </summary>
    public class StoreService : MerchelloRepositoryService, IStoreService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreService"/> class.
        /// </summary>
        public StoreService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public StoreService(ILogger logger)
            : this(new RepositoryFactory(), logger, new StoreSettingService(logger))
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public StoreService(RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, storeSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public StoreService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), storeSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public StoreService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory, IStoreSettingService storeSettingService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            _storeSettingService = storeSettingService;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IStoreService, Events.NewEventArgs<IStore>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IStoreService, Events.NewEventArgs<IStore>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IStoreService, SaveEventArgs<IStore>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IStoreService, SaveEventArgs<IStore>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IStoreService, DeleteEventArgs<IStore>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IStoreService, DeleteEventArgs<IStore>> Deleted;

        #endregion

        public IStore CreateStoreWithKey(int storeId, bool raiseEvents = true)
        {
            var store = new Store(storeId);

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IStore>(store), this))
                {
                    store.WasCancelled = true;
                    return store;
                }


            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreRepository(uow))
                {
                    repository.AddOrUpdate(store);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IStore>(store), this);

            (_storeSettingService as StoreSettingService)?.CreateStoreSettings(storeId, raiseEvents);
            var warehouse = (MerchelloContext.Current.Services.WarehouseService as WarehouseService)?.CreateWarehouse($"Warehouse {storeId}", storeId);
            if (warehouse != null)
            {
                warehouse.IsDefault = true;
                MerchelloContext.Current.Services.WarehouseService.Save(warehouse);
                MerchelloContext.Current.Services.WarehouseService.CreateWarehouseCatalogWithKey(warehouse.Key, $"Catalog {storeId}");
            }

            EntityCollectionProviderResolver.Current.ReInitialize();

            return store;
        }

        /// <summary>
        /// Saves a single <see cref="IStore"/>
        /// </summary>
        /// <param name="store">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IStore store, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IStore>(store), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreRepository(uow))
                {
                    repository.AddOrUpdate(store);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IStore>(store), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IStore"/>.
        /// </summary>
        /// <param name="stores">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IEnumerable<IStore> stores, bool raiseEvents = true)
        {
            var storeArray = stores as IStore[] ?? stores.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IStore>(storeArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();

                using (var repository = RepositoryFactory.CreateStoreRepository(uow))
                {
                    foreach (var store in storeArray)
                    {
                        repository.AddOrUpdate(store);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IStore>(storeArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IStore"/> from the database.
        /// </summary>
        /// <param name="store">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IStore store, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IStore>(store), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreRepository(uow))
                {
                    repository.Delete(store);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IStore>(store), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IStore"/> from the database.
        /// </summary>
        /// <param name="stores">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<IStore> stores, bool raiseEvents = true)
        {
            var storeArray = stores as IStore[] ?? stores.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IStore>(storeArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreRepository(uow))
                {
                    foreach (var store in storeArray)
                    {
                        repository.Delete(store);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IStore>(storeArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IStore"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IStore"/>.
        /// </returns>
        public IStore GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateStoreRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        public IStore GetById(int storeId)
        {
            using (var repository = RepositoryFactory.CreateStoreRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IStore>.Builder.Where(x => x.StoreId == storeId);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IStore"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IStore}"/>.
        /// </returns>
        public IEnumerable<IStore> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateStoreRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IStore"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IStore}"/>.
        /// </returns>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        public IEnumerable<IStore> GetAll()
        {
            using (var repository = RepositoryFactory.CreateStoreRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        //public IEnumerable<int> GetAllStoresIds()
        //{
        //    using (var repository = RepositoryFactory.CreateStoreRepository(UowProvider.GetUnitOfWork()))
        //    {
        //        return repository.GetAll().Select(x => x.StoreId);
        //    }
        //}
    }
}
