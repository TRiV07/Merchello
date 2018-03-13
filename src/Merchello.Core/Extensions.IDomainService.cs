using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Merchello.Core
{
    public static partial class Extensions
    {
        public static IDomain CurrentDomain(this IDomainService domainService)
        {
            IEnumerable<IDomain> umDomainsList = UmbracoContext.Current.HttpContext.Cache["_umDomainsList"] as IEnumerable<IDomain>;
            if (umDomainsList == null)
            {
                UmbracoContext.Current.HttpContext.Cache["_umDomainsList"] =
                    umDomainsList =
                    domainService.GetAll(true);
            }
            return umDomainsList.FirstOrDefault(x => string.Compare(x.DomainName, UmbracoContext.Current.HttpContext.Request.Url.Host, true) == 0);
        }
    }
}