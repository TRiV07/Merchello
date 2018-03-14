using Merchello.Core.Models.MultiStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Merchello.Core.MultiStore
{
    public static partial class Extensions
    {
        public static IDomain CurrentDomain(this IDomainService domainService)
        {
            IEnumerable<IDomain> umDomainsList = domainService.GetAllFromCache();
            return umDomainsList.FirstOrDefault(x => string.Compare(x.DomainName, UmbracoContext.Current.HttpContext.Request.Url.Host, true) == 0);
        }

        public static IEnumerable<IDomain> GetAllFromCache(this IDomainService domainService)
        {
            IEnumerable<IDomain> umDomainsList = UmbracoContext.Current.HttpContext.Cache[Constants.Cache.DomainsList] as IEnumerable<IDomain>;
            if (umDomainsList == null)
            {
                UmbracoContext.Current.HttpContext.Cache[Constants.Cache.DomainsList] =
                    umDomainsList =
                    domainService.GetAll(true).ToList();
            }
            return umDomainsList;
        }
    }
}