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
        public static IEnumerable<StoreDisplay> GetAllStores(this IContentService contentService, IEnumerable<IDomain> umDomainsList)
        {
            var domainsList = umDomainsList
                .Where(x => x.RootContentId.HasValue)
                .GroupBy(x => x.RootContentId)
                .Select(x => x.First());

            var stores = domainsList.Select(x => new StoreDisplay { Id = x.RootContentId.Value, Domain = x.DomainName, Name = contentService.GetById(x.RootContentId.Value)?.Name });

            return stores;
        }
    }
}