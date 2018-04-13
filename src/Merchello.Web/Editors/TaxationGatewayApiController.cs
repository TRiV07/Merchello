﻿namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;
    using Merchello.Web.WebApi.Filters;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The taxation gateway api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class TaxationGatewayApiController : MerchelloApiController
    {
        /// <summary>
        /// The taxation context.
        /// </summary>
        private readonly ITaxationContext _taxationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayApiController"/> class. 
        /// </summary>
        public TaxationGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        public TaxationGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _taxationContext = MerchelloContext.Gateways.Taxation;
        }

        /// <summary>
        /// 
        /// 
        /// GET /umbraco/Merchello/TaxationGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the TaxationGatewayProvider
        /// </param>
        /// <returns>
        /// The collection of <see cref="GatewayResourceDisplay"/>.
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<GatewayResourceDisplay> GetGatewayResources(Guid id, int storeId)
        {
            try
            {
                var provider = _taxationContext.GetProviderByKey(id, storeId);

                var resources = provider.ListResourcesOffered();

                return resources.Select(resource => resource.ToGatewayResourceDisplay());
            }
            catch (Exception)
            {

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }

        /// <summary>
        /// Returns a list of all of GatewayProviders of GatewayProviderType Taxation
        /// 
        /// GET /umbraco/Merchello/TaxationGatewayApi/GetAllGatewayProviders/
        /// </summary>
        /// <returns>
        /// The collection of <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<TaxationGatewayProviderDisplay> GetAllGatewayProviders(int storeId)
        {
            var providers = _taxationContext.GetAllActivatedProviders(storeId);
            if (providers == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            
            return providers.Select(provider => provider.ToTaxationGatewayProviderDisplay());
        }

        /// <summary>
        /// 
        /// 
        /// GET /umbraco/Merchello/TaxationGatewayApi/GetTaxationProviderTaxMethods/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the TaxationGatewayProvider
        /// </param>
        /// <returns>
        /// The collection of <see cref="TaxMethodDisplay"/>.
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<TaxMethodDisplay> GetTaxationProviderTaxMethods(Guid id, int storeId)
        {
            var provider = _taxationContext.GetProviderByKey(id, storeId);
            if (provider != null)
            {
                var methods = provider.GetAllGatewayTaxMethods().Select(x => x.ToTaxMethodDisplay());
                return methods;
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Adds a TaxMethod
        /// 
        /// POST /umbraco/Merchello/TaxationGatewayApi/AddTaxMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="TaxMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST")]
        [EnsureUserPermissionForStore("method.StoreId")]
        public HttpResponseMessage AddTaxMethod(TaxMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var deleteMethod = _taxationContext.GetTaxMethodForCountryCode(method.StoreId, method.CountryCode);
                if (deleteMethod != null) this.DeleteTaxMethod(deleteMethod.Key);

                var provider = _taxationContext.GetProviderByKey(method.ProviderKey, method.StoreId);

                var taxationGatewayMethod = provider.CreateTaxMethod(method.CountryCode, method.PercentageTaxRate);

                method.ToTaxMethod(taxationGatewayMethod.TaxMethod);

                provider.SaveTaxMethod(taxationGatewayMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a TaxMethod
        /// 
        /// PUT /umbraco/Merchello/TaxationGatewayApi/PutTaxMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="TaxMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        [EnsureUserPermissionForStore("method.StoreId")]
        public HttpResponseMessage PutTaxMethod(TaxMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _taxationContext.GetProviderByKey(method.ProviderKey, method.StoreId);

                var taxMethod = provider.TaxMethods.FirstOrDefault(x => x.Key == method.Key);

                if (taxMethod == null)
                {
                    var deleteMethod = _taxationContext.GetTaxMethodForCountryCode(method.StoreId, method.CountryCode);
                    this.DeleteTaxMethod(deleteMethod.Key);
                    return this.AddTaxMethod(method);
                }

                taxMethod = method.ToTaxMethod(taxMethod);

                provider.GatewayProviderService.Save(taxMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="ITaxMethod"/>
        /// 
        /// GET /umbraco/Merchello/TaxationGatewayApi/DeleteTaxMethod
        /// </summary>
        /// <param name="id">
        /// TaxMethod Key
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeleteTaxMethod(Guid id)
        {
            var taxMethodService = ((ServiceContext)MerchelloContext.Services).TaxMethodService;
            var methodToDelete = taxMethodService.GetByKey(id);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            ValidateStoreAccess(methodToDelete.StoreId);

            taxMethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }

}