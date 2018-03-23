﻿namespace Merchello.Web.Search
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Trees;
    using Merchello.Web.Models;

    // REFACTOR - This is currently using Request caching.  In V3, this will change to the RuntimeCache in V3 
    //  after we do the repository refactoring and we can ensure the values are cleared appropriately on relevant 
    //  collection tree.

    /// <summary>
    /// Provides <see cref="IProductCollection"/> in a <see cref="TreeNode{IProductCollection}"/> structure.
    /// </summary>
    internal class ProductCollectionTreeQuery : ProductCollectionQuery, IProductCollectionTreeQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionTreeQuery"/> class.
        /// </summary>
        public ProductCollectionTreeQuery()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionTreeQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ProductCollectionTreeQuery(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <inheritdoc/>
        public TreeNode<IProductCollection> GetTreeByValue(IProductCollection value)
        {
            var cacheKey = GetCacheKey("GetTreeByValue");
            var tree = (TreeNode<IProductCollection>)Cache.GetCacheItem(cacheKey);

            if (tree != null) return tree;

            var root = GetTreeContaining(value);

            tree = root.FirstByValue(value);
            return tree != null ? (TreeNode<IProductCollection>)Cache.GetCacheItem(cacheKey, () => tree) : null;
        }

        /// <inheritdoc />
        public TreeNode<IProductCollection> GetTreeContaining(IProductCollection collection)
        {
            var col = Service.GetByKey(collection.Key);
            var trees = GetRootTrees(col.DomainRootStructureID);
            return trees.FirstOrDefault(tree => tree.Flatten().Any(node => node.Key == collection.Key));
        }

        /// <inheritdoc />
        public TreeNode<IProductCollection> GetTreeWithRoot(IProductCollection collection, int domainRootStructureID)
        {
            return new TreeNode<IProductCollection>(collection).Populate(GetAll(domainRootStructureID));
        }

        /// <inheritdoc />
        public IEnumerable<TreeNode<IProductCollection>> GetRootTrees(int domainRootStructureID)
        {
            var cacheKey = GetCacheKey("GetRootTrees", domainRootStructureID);
            var trees = (IEnumerable<TreeNode<IProductCollection>>)Cache.GetCacheItem(cacheKey);

            if (trees != null) return trees;

            var factory = new TreeNodeFactory<IProductCollection>();
            return (IEnumerable<TreeNode<IProductCollection>>)
                        Cache.GetCacheItem(cacheKey, () => factory.BuildTrees(GetAll(domainRootStructureID)));
        }
    }
}