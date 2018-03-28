﻿namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using Core;
    using Core.Models;
    using Core.Services;
    using Providers;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The invoice data service.
    /// </summary>
    internal class InvoiceDataService : DataServiceBase, IInvoiceDataService
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDataService"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public InvoiceDataService()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDataService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public InvoiceDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }


        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IInvoice"/>.
        /// </returns>
        public IEnumerable<IInvoice> GetAll()
        {
            //TODOMS
            return MerchelloContext.HasCurrent
                       ? ((InvoiceService)MerchelloContext.Current.Services.InvoiceService).GetAll()
                       : new InvoiceService(Logger.CreateWithDefaultLog4NetConfiguration()).GetAll();
        }


        /// <summary>
        /// The get index field names.
        /// </summary>
        /// <returns>
        /// The collection of index field names.
        /// </returns>
        public IEnumerable<string> GetIndexFieldNames()
        {
            return InvoiceIndexer.IndexFieldPolicies.Select(x => x.Name);
        }
    }
}