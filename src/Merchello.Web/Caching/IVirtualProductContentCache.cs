namespace Merchello.Web.Caching
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Web.Models.VirtualContent;

    internal interface IVirtualProductContentCache : IVirtualContentCache<IProductContent, IProduct>
    {
        IProductContent GetBySlug(string slug, int domainRootStructureID, Func<string, int, IProductContent> get);

        IProductContent GetBySku(string sku, int domainRootStructureID, Func<string, int, IProductContent> get);
    }
}