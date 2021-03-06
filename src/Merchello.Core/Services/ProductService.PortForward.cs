﻿namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Services.Interfaces;

    using Umbraco.Core.Persistence;

    using UConstants = Umbraco.Core.Constants;

    /// <summary>
    /// Product service queries for used in the product query.
    /// </summary>
    public partial class ProductService : IProductServicePortForward
    {
        /// <inheritdoc/>
        public IEnumerable<string> GetAllManufacturers(int storeId)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.GetAllManufacturers();
            }
        }

        /// <inheritdoc/>
        public PagedCollection<IProduct> GetRecentlyUpdatedProducts(int storeId, long page, long itemsPerPage = 10)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.GetRecentlyUpdatedProducts(page, itemsPerPage);
            }
        }


        /// <summary>
        /// Gets the entity keys for distinct entities in multiple collections
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        internal Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), UConstants.System.Root))
            {
                return repository.GetKeysThatExistInAllCollections(collectionKeys, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }
        }

        /// <summary>
        /// Gets a paged list of distinct keys for entities in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
        /// A filter term
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        internal Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), UConstants.System.Root))
            {
                return repository.GetKeysThatExistInAllCollections(collectionKeys, term, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }   
        }

        /// <summary>
        /// The get keys not in multiple collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            int storeId,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.GetKeysNotInAnyCollections(collectionKeys, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }
        }

        /// <summary>
        /// The get keys not in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The filter term
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            int storeId,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.GetKeysNotInAnyCollections(collectionKeys, term, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), UConstants.System.Root))
            {
                return repository.GetKeysThatExistInAnyCollections(collectionKeys, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
        /// The search term.
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), UConstants.System.Root))
            {
                return repository.GetKeysThatExistInAnyCollections(collectionKeys, term, min, max, page, itemsPerPage, orderExpression, sortDirection);
            }
        }

        /// <inheritdoc/>
        internal PagedCollection<IProduct> GetByAdvancedSearch(
            Guid collectionKey,
            string[] includeFields,
            string term,
            string manufacturer,
            long page,
            long itemsPerPage,
            int storeId,
            string orderExpression,
            SortDirection direction = SortDirection.Ascending)
        {
            using (var repository = RepositoryFactory.CreateProductRepository(UowProvider.GetUnitOfWork(), storeId))
            {
                return repository.GetByAdvancedSearch(collectionKey, includeFields, term, manufacturer, page, itemsPerPage, orderExpression, direction);
            }
        }
    }
}
