using Merchello.Core.Models;
using Merchello.Core.Models.MultiStore;
using Merchello.Core.Services;
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
        public static IEnumerable<IStore> CachedAll(this IStoreService storeService)
        {
            IEnumerable<IStore> umStoresList = UmbracoContext.Current.HttpContext.Cache[Constants.Cache.StoresList] as IEnumerable<IStore>;
            if (umStoresList == null)
            {
                UmbracoContext.Current.HttpContext.Cache[Constants.Cache.StoresList] =
                    umStoresList =
                    storeService.GetAll().OrderBy(x => x.CreateDate).ToArray();
            }
            return umStoresList;
        }

        public static IEnumerable<int> CachedAllStoresIds(this IStoreService storeService)
        {
            return storeService.CachedAll()
                .Select(x => x.StoreId);
        }
    }
}