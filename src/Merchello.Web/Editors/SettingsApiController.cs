﻿namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Core.Models;
    using Core.Models.TypeFields;
    using Core.Services;

    using Merchello.Core.Configuration;
    using Merchello.Core.Logging;
    using Merchello.Core.MultiStore;
    using Merchello.Core.Persistence.Migrations.Analytics;
    using Merchello.Web.Reporting;
    using Merchello.Web.Trees;
    using Merchello.Web.WebApi.Filters;
    using Models.ContentEditing;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using WebApi;

    using UConstants = Umbraco.Core.Constants;

    /// <summary>
    /// The settings api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class SettingsApiController : MerchelloApiController
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly StoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsApiController"/> class. 
        /// Constructor
        /// </summary>
        public SettingsApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsApiController"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context
        /// </param>
        public SettingsApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService as StoreSettingService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsApiController"/> class. 
        /// This is a helper contructor for unit testing
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        internal SettingsApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService as StoreSettingService;
        }

        /// <summary>
        /// Returns Country for the countryCode passed in
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetCountry/{countryCode}
        /// </summary>
        /// <param name="id">
        /// Country code to get
        /// </param>
        /// <returns>
        /// The <see cref="CountryDisplay"/>.
        /// </returns>
        public CountryDisplay GetCountry(string id)
        {
            ICountry country = _storeSettingService.GetCountryByCode(id);
            if (country == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return country.ToCountryDisplay();
        }

        /// <summary>
        /// Returns All Countries
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetAllCountries
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="CountryDisplay"/>.
        /// </returns>
        public IEnumerable<CountryDisplay> GetAllCountries()
        {
            var countries = _storeSettingService.GetAllCountries().OrderBy(x => x.Name);
            if (countries == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return countries.Select(x => x.ToCountryDisplay());

        }


        /// <summary>
        /// The get type fields.
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="TypeField"/>.
        /// </returns>
        public IEnumerable<TypeField> GetTypeFields()
        {
            var typeFields = _storeSettingService.GetTypeFields();

            return typeFields.Select(x => x as TypeField);
        }

        /// <summary>
        /// Returns All Countries with a list of country codes to exclude
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetAllCountriesExcludeCodes?codes={string}&codes={string}
        /// </summary>
        /// <param name="codes">Country codes to exclude</param>
        public IEnumerable<CountryDisplay> GetAllCountriesExcludeCodes([FromUri]string[] codes)
        {
            var countries = _storeSettingService.GetAllCountries(codes);
            if (countries == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return countries.Select(x => x.ToCountryDisplay());           
        }

		/// <summary>
		/// Returns All Tax Provinces
		/// 
		/// GET /umbraco/Merchello/SettingsApi/GetAllTaxProvinces
		/// </summary>
        public IEnumerable<ICurrency> GetAllCurrencies()
		{
			// TODO: replace with call to service
            var currencyList = _storeSettingService.GetAllCurrencies();

		    if (currencyList == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
		    }

            return currencyList;
		}

        /// <summary>
        /// Returns settings for domain id
        /// GET /umbraco/Merchello/SettingsApi/GetAllSettings
        /// </summary>
        [EnsureUserPermissionForStore("id", true)]
        public SettingDisplay GetAllSettings(int id)
		{
            // TODO - why is this done this way?												   
            var settings = _storeSettingService.GetAll(id);
			var settingDisplay = new SettingDisplay();

			if (settings == null)
			{
				throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
			}

			return settingDisplay.ToStoreSettingDisplay(settings);
		}

        /// <summary>
        /// Gets the nextInvoiceNumber and nextOrderNumber
        /// </summary>
        /// <returns>Next Invoice Number and Next Order Number</returns>
        [EnsureUserPermissionForStore("id", true)]
        public SettingDisplay GetInvoiceAndOrderNumbers(int id)
        {
            var settingDisplay = new SettingDisplay
            {
                NextInvoiceNumber = _storeSettingService.GetNextInvoiceNumber(id),
                NextOrderNumber = _storeSettingService.GetNextOrderNumber(id)
            };
            
            return settingDisplay;
        }

        /// <summary>
        /// The get merchello version.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpGet]
        public string GetMerchelloVersion()
        {
            return MerchelloVersion.Current.ToString();
        }

        /// <summary>
        /// Gets the report back office trees.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{BackOfficeTreeDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<BackOfficeTreeDisplay> GetReportBackofficeTrees()
        {
            var atts =
                ReportApiControllerResolver.Current.ResolvedTypesWithAttribute.Select(
                    x => x.GetCustomAttribute<BackOfficeTreeAttribute>(true)).Where(x => x != null).ToArray();
        
            return atts.Select(att => new BackOfficeTreeDisplay
                {
                    RouteId = att.RouteId,
                    ParentRouteId = att.ParentRouteId,
                    Icon = att.Icon,
                    RoutePath = att.RoutePath,
                    Title = att.Title,
                    SortOrder = att.SortOrder
                });
        }

        /// <summary>
        /// Updates existing global settings
        ///
        /// PUT /umbraco/Merchello/SettingsApi/PutSettings
        /// </summary>
        /// <param name="id">Store id</param>
        /// <param name="setting">SettingDisplay object serialized from WebApi</param>
        [AcceptVerbs("POST", "PUT")]
        [EnsureUserPermissionForStore("id")]
        public HttpResponseMessage PutSettings(int id, SettingDisplay setting)
		{
			var response = Request.CreateResponse(HttpStatusCode.OK);

			try
			{
				IEnumerable<IStoreSetting> merchSetting = setting.ToStoreSetting(_storeSettingService.GetAll(id).ToList());
				foreach(var s in merchSetting)
				{
					_storeSettingService.Save(s);
				}
			}
			catch (Exception ex)
			{
				response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
			}

			return response;
		}

        [HttpGet]
        [EnsureUserPermissionForStore("id")]
        public HttpResponseMessage GetDomainRootIdByContentId(int id)
        {
            var domainService = ApplicationContext.Services.DomainService;
            var domains = domainService.GetAllFromCache();

            var content = UmbracoContext.Current.ContentCache.GetById(id);
            if (content == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            var rootContent = content.AncestorOrSelf(1);
            return Request.CreateResponse(HttpStatusCode.OK, domains
                .FirstOrDefault(x => x.RootContentId.HasValue && x.RootContentId.Value == rootContent.Id)?.RootContentId);
        }

        /// <summary>
        /// Records the domain used by Merchello.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<HttpResponseMessage> RecordDomain(MigrationDomain record)
        {
            //var setting = _storeSettingService.GetByKey(Constants.StoreSetting.HasDomainRecordKey);

            //if (setting != null && setting.Value == "False")
            //{
            //    try
            //    {
            //        var migrationManager = new WebMigrationManager();
            //        var response = await migrationManager.PostDomainRecord(record);

            //        if (response.StatusCode != HttpStatusCode.OK)
            //        {
            //            var ex = new Exception(response.ReasonPhrase);
            //            MultiLogHelper.Error<SettingsApiController>("Failed to record domain analytic", ex);
            //        }

            //        setting.Value = true.ToString();
            //        _storeSettingService.Save(setting);

            //        return response;
            //    }
            //    catch (Exception ex)
            //    {
            //        // this is for analytics only and we don't want to throw
            //        MultiLogHelper.Error<SettingsApiController>("Failed to record analytics (Domain)", ex);
            //    }

            //}

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
