﻿namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;
    using Models.Payments;
    using WebApi;
    using Umbraco.Web.Mvc;
    using Merchello.Web.WebApi.Filters;

    /// <summary>
    /// Represents the PaymentGatewayApiController
    /// </summary>
    [PluginController("Merchello")]
    public class PaymentGatewayApiController : MerchelloApiController 
    {
        /// <summary>
        /// The payment context.
        /// </summary>
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayApiController"/> class.
        /// </summary>
        public PaymentGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        public PaymentGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _paymentContext = MerchelloContext.Gateways.Payment;
        }

        /// <summary>
        /// 
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the PaymentGatewayProvider
        /// </param>
        /// <returns>
        /// A collection of <see cref="GatewayResourceDisplay"/>.
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<GatewayResourceDisplay> GetGatewayResources(Guid id, int storeId)
        {
            try
            {
                var provider = _paymentContext.GetProviderByKey(id, storeId);

                var resources = provider.ListResourcesOffered();

                return resources.Select(resource => resource.ToGatewayResourceDisplay());
            }
            catch (Exception)
            {

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }            
        }

        /// <summary>
        /// Returns a list of all of GatewayProviders of GatewayProviderType Payment
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetAllGatewayProviders/
        /// </summary>
        /// <returns>
        /// A collection of all payment <see cref="GatewayProviderDisplay"/>
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<GatewayProviderDisplay> GetAllGatewayProviders(int storeId)
        {
            var providers = _paymentContext.GetAllActivatedProviders(storeId);

            if (providers == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return providers.Select(provider => provider.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// Get all <see cref="IPaymentMethod"/> for a payment provider
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetPaymentProviderPaymentMethods/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the PaymentGatewayProvider
        /// </param>
        /// <returns>
        /// A collection of <see cref="PaymentMethodDisplay"/>
        /// </returns>
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<PaymentMethodDisplay> GetPaymentProviderPaymentMethods(Guid id, int storeId)
        {
            var provider = _paymentContext.GetProviderByKey(id, storeId);
            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            foreach (var method in provider.PaymentMethods)
            {
                // we need the actual PaymentGatewayProvider so we can grab the if present
                yield return provider.GetPaymentGatewayMethodByKey(method.Key).ToPaymentMethodDisplay();
            }
        }

        /// <summary>
        /// The get available payment methods.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentMethodDisplay}"/>.
        /// </returns>
        [HttpGet]
        [EnsureUserPermissionForStore("storeId")]
        public IEnumerable<PaymentMethodDisplay> GetAvailablePaymentMethods(int storeId)
        {
            var methods = _paymentContext.GetPaymentGatewayMethods(storeId).Select(x => x.ToPaymentMethodDisplay());
            return methods.Where(x => x.IncludeInPaymentSelection).OrderBy(x => x.Name);
        }

            /// <summary>
        /// Gets a <see cref="PaymentMethodDisplay"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethodDisplay"/>.
        /// </returns>
        [HttpGet]
        [EnsureUserPermissionForStore("storeId")]
        public PaymentMethodDisplay GetPaymentMethodByKey(Guid? key, int storeId)
        {
            if (key != null)
            {
                var method = _paymentContext.GetPaymentGatewayMethods(storeId).FirstOrDefault(x => x.PaymentMethod.Key == key.Value);
                return method != null ? method.ToPaymentMethodDisplay() : null;
            }
            return null;
        }

        /// <summary>
        /// Adds a <see cref="IPaymentMethod"/>
        /// 
        /// POST /umbraco/Merchello/PaymentGatewayApi/AddPaymentMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="PaymentMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST")]
        [EnsureUserPermissionForStore("method.StoreId")]
        public HttpResponseMessage AddPaymentMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.GetProviderByKey(method.ProviderKey, method.StoreId);

                var gatewayResource =
                    provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == method.PaymentCode);

                var paymentGatewayMethod = provider.CreatePaymentMethod(gatewayResource, method.Name, method.Description);

                provider.SavePaymentMethod(paymentGatewayMethod);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a <see cref="IPaymentMethod"/>
        /// 
        /// PUT /umbraco/Merchello/PaymentGatewayApi/PutPaymentMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="PaymentMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        [EnsureUserPermissionForStore("method.StoreId")]
        public HttpResponseMessage PutPaymentMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.GetProviderByKey(method.ProviderKey, method.StoreId);

                var paymentMethod = provider.PaymentMethods.FirstOrDefault(x => x.Key == method.Key);

                paymentMethod = method.ToPaymentMethod(paymentMethod);

                provider.GatewayProviderService.Save(paymentMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="IPaymentMethod"/>
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/DeletePaymentMethod
        /// </summary>
        /// <param name="id">
        /// <see cref="PaymentMethodDisplay"/> key to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET", "DELETE")]
        [EnsureUserPermissionForStore("storeId")]
        public HttpResponseMessage DeletePaymentMethod(Guid id, int storeId)
        {
            var paymentProvider = MerchelloContext.Gateways.Payment.GetProviderByMethodKey(id, storeId);
            if (paymentProvider == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            var methodToDelete = paymentProvider.GetPaymentGatewayMethodByKey(id);

            paymentProvider.DeletePaymentMethod(methodToDelete);
           
            return Request.CreateResponse(HttpStatusCode.OK);
        }
         
    }
}