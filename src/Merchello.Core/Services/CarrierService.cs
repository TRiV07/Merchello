namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    using MS = Merchello.Core.Constants.MultiStore;

    /// <summary>
    /// Represents the Carrier Service, 
    /// </summary>
    public class CarrierService : PageCachedMSServiceBase<ICarrier>, ICarrierService
    {
        #region fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// The valid sort fields.
        /// </summary>
        private static readonly string[] ValidSortFields = { "firstname", "lastname", "loginname", "email", "lastactivitydate" };

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The payment service.
        /// </summary>
        private readonly IPaymentService _paymentService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
        /// </summary>
        public CarrierService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CarrierService(ILogger logger)
            : this(new RepositoryFactory(), logger, new InvoiceService(logger), new PaymentService(logger)) 
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public CarrierService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new RepositoryFactory(logger, sqlSyntax), logger, new InvoiceService(logger, sqlSyntax), new PaymentService(logger, sqlSyntax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="invoiceService">
        /// The invoice Service.
        /// </param>
        /// <param name="paymentService">
        /// The payment Service.
        /// </param>
        public CarrierService(
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : this(
            new PetaPocoUnitOfWorkProvider(logger),
            repositoryFactory,
            logger,
            invoiceService,
            paymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
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
        /// <param name="invoiceService">
        /// The invoice Service.
        /// </param>
        /// <param name="paymentService">
        /// The payment Service.
        /// </param>
        public CarrierService(
            IDatabaseUnitOfWorkProvider provider,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), invoiceService, paymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierService"/> class.
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
        /// <param name="invoiceService">
        /// The invoice service.
        /// </param>
        /// <param name="paymentService">
        /// The payment service.
        /// </param>
        public CarrierService(
            IDatabaseUnitOfWorkProvider provider,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Mandate.ParameterNotNull(invoiceService, "invoiceServie");
            Mandate.ParameterNotNull(paymentService, "paymentService");
            _invoiceService = invoiceService;
            _paymentService = paymentService;
        }

        #region Event Handlers



        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<ICarrierService, Events.NewEventArgs<ICarrier>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICarrierService, Events.NewEventArgs<ICarrier>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICarrierService, SaveEventArgs<ICarrier>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICarrierService, SaveEventArgs<ICarrier>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICarrierService, DeleteEventArgs<ICarrier>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICarrierService, DeleteEventArgs<ICarrier>> Deleted;



        #endregion


        #region ICarrierService Members

        /// <summary>
        /// Creates a Carrier without saving to the database
        /// </summary>
        /// <param name="loginName">The login name of the carrier.</param>
        /// <param name="firstName">The first name of the carrier</param>
        /// <param name="lastName">The last name of the carrier</param>
        /// <param name="email">the email address of the carrier</param>
        /// <returns>The <see cref="ICarrier"/></returns>
        public ICarrier CreateCarrier(string loginName, int storeId, string firstName, string lastName, string email)
        {
            Mandate.ParameterNotNullOrEmpty(loginName, "loginName");
            var carrier = new Carrier(loginName, storeId)
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

            if (!Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICarrier>(carrier), this))
            {
                return carrier;
            }

            carrier.WasCancelled = true;
            
            return carrier;
        }

        /// <summary>
        /// Creates a carrier and saves the record to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the carrier
        /// </param>
        /// <param name="lastName">
        /// The last name of the carrier
        /// </param>
        /// <param name="email">
        /// the email address of the carrier
        /// </param>
        /// <returns>
        /// <see cref="ICarrier"/>
        /// </returns>
        public ICarrier CreateCarrierWithKey(string loginName, int storeId, string firstName, string lastName, string email)
        {
            Mandate.ParameterNotNullOrEmpty(loginName, "loginName");

            var carrier = new Carrier(loginName, storeId)
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICarrier>(carrier), this))
            {
                carrier.WasCancelled = true;
                return carrier;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCarrierRepository(uow, storeId))
                {
                    repository.AddOrUpdate(carrier);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<ICarrier>(carrier), this);

            return carrier;
        }

        /// <summary>
        /// Creates a carrier with the Umbraco login name
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// The <see cref="ICarrier"/>
        /// </returns>
        public ICarrier CreateCarrierWithKey(string loginName, int storeId)
        {
            return CreateCarrierWithKey(loginName, storeId, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Saves a single <see cref="ICarrier"/> object
        /// </summary>
        /// <param name="carrier">The <see cref="ICarrier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICarrier carrier, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICarrier>(carrier), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCarrierRepository(uow, carrier.StoreId))
                {
                    repository.AddOrUpdate(carrier);
                    uow.Commit();
                }                
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICarrier>(carrier), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ICarrier"/> objects.
        /// </summary>
        /// <param name="carriers">Collection of <see cref="ICarrier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICarrier> carriers, bool raiseEvents = true)
        {
            var carrierArray = carriers as ICarrier[] ?? carriers.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICarrier>(carrierArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                
                using (var repository = RepositoryFactory.CreateCarrierRepository(uow, MS.DefaultId))
                {
                    foreach (var carrier in carrierArray)
                    {
                        repository.AddOrUpdate(carrier);
                    }
          
                    uow.Commit();
                }               
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICarrier>(carrierArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICarrier"/> object
        /// </summary>
        /// <param name="carrier">The <see cref="ICarrier"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICarrier carrier, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICarrier>(carrier), this);
          
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCarrierRepository(uow, MS.DefaultId))
                {
                    repository.Delete(carrier);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICarrier>(carrier), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICarrier"/> objects
        /// </summary>
        /// <param name="carriers">Collection of <see cref="ICarrier"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICarrier> carriers, bool raiseEvents = true)
        {
            var carrierArray = carriers as ICarrier[] ?? carriers.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICarrier>(carrierArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCarrierRepository(uow, MS.DefaultId))
                {
                    foreach (var carrier in carrierArray)
                    {
                        repository.Delete(carrier);
                    }

                    uow.Commit();                    
                }                
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICarrier>(carrierArray), this);
        }

        /// <summary>
        /// Gets a carrier by its unique id
        /// </summary>
        /// <param name="key">GUID key for the carrier</param>
        /// <returns><see cref="ICarrier"/></returns>
        public override ICarrier GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), MS.DefaultId))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a page of <see cref="ICarrier"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{ICarrier}"/>.
        /// </returns>
        public override Page<ICarrier> GetPage(long page, long itemsPerPage, int storeId, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                var query = Persistence.Querying.Query<ICarrier>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// The get by login name.
        /// </summary>
        /// <param name="loginName">
        /// The login name.
        /// </param>
        /// <returns>
        /// The <see cref="ICarrier"/>.
        /// </returns>
        public ICarrier GetByLoginName(string loginName, int storeId)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                var query = Persistence.Querying.Query<ICarrier>.Builder.Where(x => x.LoginName == loginName);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the total carrier count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CarrierCount(int storeId)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                var query = Persistence.Querying.Query<ICarrier>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.Count(query);
            }
        }

        /// <summary>
        /// Gets a list of carrier give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns>A collection of <see cref="ICarrier"/></returns>
        public IEnumerable<ICarrier> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), MS.DefaultId))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        #endregion


        /// <summary>
        /// For testing.
        /// </summary>
        /// <returns>
        /// The collection of all carriers.
        /// </returns>
        internal IEnumerable<ICarrier> GetAll()
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), MS.DefaultId))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets a count of items returned by a query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal override int Count(IQuery<ICarrier> query, int storeId)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.Count(query);
            }
        }

        /// <summary>
        /// Gets a <see cref="Page{Guid}"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> GetPagedKeys(long page, long itemsPerPage, int storeId, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repositoy = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                var query = Persistence.Querying.Query<ICarrier>.Builder.Where(x => x.Key != Guid.Empty);

                return repositoy.GetPagedKeys(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets a page by search term
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        /// <remarks>
        /// The search is prefabricated in the repository
        /// </remarks>
        internal Page<Guid> GetPagedKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            int storeId,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy));
            }
        }    

        /// <summary>
        /// Gets a page by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetPagedKeys(
            IQuery<ICarrier> query,
            long page,
            long itemsPerPage,
            int storeId,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetPagedKeys(
                RepositoryFactory.CreateCarrierRepository(UowProvider.GetUnitOfWork(), storeId),
                query,
                page,
                itemsPerPage,
                ValidateSortByField(sortBy),
                sortDirection);
        }

        /// <summary>
        /// Validates the sort by field
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string ValidateSortByField(string sortBy)
        {
            return ValidSortFields.Contains(sortBy.ToLower()) ? sortBy : "loginName";
        }
    }
}