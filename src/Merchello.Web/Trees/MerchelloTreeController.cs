namespace Merchello.Web.Trees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http.Formatting;
    using Core.Configuration;
    using Core.Configuration.Outline;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models.MultiStore;
    using Merchello.Core.MultiStore;
    using Merchello.Web.Models.ContentEditing.Collections;
    using Merchello.Web.Reporting;
    using Merchello.Web.Trees.Actions;

    using umbraco.BusinessLogic.Actions;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Models.Trees;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Trees;

    using UConstants = Umbraco.Core.Constants;

    /// <summary>
    /// The merchello tree controller.
    /// </summary>
    [Tree("merchello", "merchello", "Merchello")]
    [PluginController("Merchello")]
    public sealed class MerchelloTreeController : TreeController
    {
        /// <summary>
        /// The dialogs path.
        /// </summary>
        private const string DialogsPath = "/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/";

        /// <summary>
        /// The text service.
        /// </summary>
        private readonly ILocalizedTextService _textService;

        private readonly IDomainService _domainService;
        private readonly IContentService _contentService;

        /// <summary>
        /// The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The root trees.
        /// </summary>
        private readonly IEnumerable<TreeElement> _rootTrees;

        /// <summary>
        /// The collection trees.
        /// </summary>
        private readonly string[] _collectiontrees = { "products", "sales", "customers" };

        /// <summary>
        /// Trees that can be filled via attributes.
        /// </summary>
        private readonly string[] _attributetrees = { "reports" };

        /// <summary>
        /// The <see cref="EntityCollectionProviderResolver"/>.
        /// </summary>
        private readonly EntityCollectionProviderResolver _entityCollectionProviderResolver = EntityCollectionProviderResolver.Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloTreeController"/> class.
        /// </summary>
        public MerchelloTreeController()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloTreeController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the Umbraco ApplicationContent is null
        /// </exception>
        public MerchelloTreeController(UmbracoContext context)
        {
            if (ApplicationContext == null) throw new NullReferenceException("Umbraco ApplicationContent is null");
            Mandate.ParameterNotNull(context, "context");

            //// http://issues.merchello.com/youtrack/issue/M-732
            _textService = ApplicationContext.Services.TextService;
            _domainService = ApplicationContext.Services.DomainService;
            _contentService = ApplicationContext.Services.ContentService;

            _culture = LocalizationHelper.GetCultureFromUser(context.Security.CurrentUser);

            _rootTrees = MerchelloConfiguration.Current.BackOffice.GetTrees().Where(x => x.Visible).ToArray();
        }

        /// <summary>
        /// The get tree nodes.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNodeCollection"/>.
        /// </returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var collection = new TreeNodeCollection();

            if (id == "-1")
            {
                //UmbracoContext.Current.ContentCache.GetById(display.StoreId)
                //var storesTree = _contentService.GetAllStores(_domainService.GetAllFromCache());

                var storesTree = _contentService
                    .GetByIds(Services.StoreService().CachedAllStoresIds())
                    .Select(x => new StoreTreeDisplay()
                    {
                        Id = x.Id,
                        Name = x.Name
                    });

                var startNodeIds = Security.CurrentUser.CalculateContentStartNodeIds(Services.EntityService);
                var hasAccessToRoot = startNodeIds.Contains(Constants.System.Root);

                var treeNodes = storesTree
                    .Where(x => hasAccessToRoot || startNodeIds.Contains(x.Id))
                    .Select(x =>
                        CreateTreeNode($"store_{x.Id}",
                        null,
                        queryStrings,
                        //$"{x.Name} ({x.Domain})",
                        x.Name,
                        "icon-shopping-basket-alt",
                        true,
                        $"merchello/merchello/settings/manage/store/{x.Id}"));

                collection.AddRange(treeNodes);
            }
            else
            {
                //if (id.StartsWith("store-"))
                //{
                //    var qs = queryStrings.ReadAsNameValueCollection();
                //    qs.Add("storeId", id.Replace("store-", ""));
                //    new FormDataCollection(qs.ToDictionary());
                //}
                var currentTree = _rootTrees.FirstOrDefault(x => x.Id == id && x.Visible);
                var splitId = new SplitRoutePath(id);

                collection.AddRange(
                    currentTree != null
                        ? InitializeTree(currentTree, splitId, queryStrings)
                        : InitializeTree(splitId, queryStrings));
            }

            return collection;
        }

        /// <summary>
        /// The get menu for node.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="MenuItemCollection"/>.
        /// </returns>
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            var splitId = new SplitRoutePath(id);

            var startNodeIds = UmbracoContext.Current.Security.CurrentUser.CalculateContentStartNodeIds(ApplicationContext.Current.Services.EntityService);
            var hasAccessToRoot = startNodeIds.Contains(UConstants.System.Root);

            // Products
            if (splitId.CollectionId == "products")
            {
                menu.Items.Add<NewCollectionAction>(
                        _textService.Localize("merchelloVariant/newProduct", _culture),
                        hasSeparator: false,
                        additionalData: new Dictionary<string, object>() { { "dialogData", new { storeId = splitId.StoreId } } })
                    .LaunchDialogView(
                        DialogsPath + "product.add.html",
                        _textService.Localize("merchelloVariant/newProduct", _culture));
                //.NavigateToRoute("merchello/merchello/productedit/create");

                if (hasAccessToRoot)
                {
                    menu.Items.Add<NewProductContentTypeAction>(
                    _textService.Localize("merchelloDetachedContent/associateContentType", _culture),
                    false)
                    .LaunchDialogView(DialogsPath + "productcontenttype.add.html", _textService.Localize("merchelloDetachedContent/associateContentType", _culture));
                }
            }

            if (splitId.CollectionId == "customers")
            {
                menu.Items.Add<NewCollectionAction>(
                        _textService.Localize("merchelloCustomers/newCustomer", _culture),
                        hasSeparator: false,
                        additionalData: new Dictionary<string, object>() { { "dialogData", new { storeId = splitId.StoreId } } })
                    .LaunchDialogView(DialogsPath + "customer.newcustomer.html", _textService.Localize("merchelloCustomers/newCustomer", _culture));
            }

            if (splitId.CollectionId == "marketing")
            {
                menu.Items.Add<NewOfferSettingsAction>(
                    _textService.Localize("merchelloMarketing/newOffer", _culture),
                        hasSeparator: false,
                        additionalData: new Dictionary<string, object>() { { "dialogData", new { storeId = splitId.StoreId } } })
                    .LaunchDialogView(
                        DialogsPath + "marketing.newofferproviderselection.html",
                        _textService.Localize("merchelloMarketing/newOffer", _culture));
            }

            //// child nodes will have an id separated with a hypen and key
            //// e.g.  products_[GUID]
            //var splitId = new SplitRoutePath(id);

            if (_collectiontrees.Contains(splitId.CollectionId) && !id.EndsWith("_resolved"))
            {
                menu.Items.Add<NewCollectionAction>(
                    _textService.Localize(string.Format("merchelloCollections/{0}", NewCollectionAction.Instance.Alias), _culture),
                    _collectiontrees.Contains(splitId.CollectionId),
                    new Dictionary<string, object>()
                        {
                            { "dialogData", new { entityType = splitId.CollectionId, parentKey = splitId.CollectionKey, storeId = splitId.StoreId } }
                        }).LaunchDialogView(DialogsPath + "create.staticcollection.html", _textService.Localize(string.Format("merchelloCollections/{0}", NewCollectionAction.Instance.Alias), _culture));

                if (!_collectiontrees.Contains(splitId.CollectionId)) // don't show this on root nodes
                    menu.Items.Add<ManageEntitiesAction>(
                        _textService.Localize(string.Format("merchelloCollections/{0}", ManageEntitiesAction.Instance.Alias), _culture),
                        false,
                        new Dictionary<string, object>()
                            {
                            { "dialogData", new { entityType = splitId.CollectionId, collectionKey = splitId.CollectionKey, storeId = splitId.StoreId } }
                            }).LaunchDialogView(DialogsPath + "manage.staticcollection.html", _textService.Localize(string.Format("merchelloCollections/{0}", ManageEntitiesAction.Instance.Alias), _culture));

                menu.Items.Add<SortCollectionAction>(
                    _textService.Localize("actions/sort", _culture),
                    false,
                    new Dictionary<string, object>()
                        {
                             { "dialogData", new { entityType = splitId.CollectionId, parentKey = splitId.CollectionKey, storeId = splitId.StoreId } }
                        }).LaunchDialogView(DialogsPath + "sort.staticcollection.html", _textService.Localize(string.Format("merchelloCollections/{0}", SortCollectionAction.Instance.Alias), _culture));

                if (splitId.IsChildCollection)
                {
                    // add the delete button
                    menu.Items.Add<DeleteCollectionAction>(
                        _textService.Localize("actions/delete", _culture),
                        false,
                        new Dictionary<string, object>()
                            {
                                { "dialogData", new { entityType = splitId.CollectionId, collectionKey = splitId.CollectionKey, storeId = splitId.StoreId } }
                            })
                        .LaunchDialogView(DialogsPath + "delete.staticcollection.html", _textService.Localize("actions/delete", _culture));
                }
            }

            menu.Items.Add<RefreshNode, ActionRefresh>(_textService.Localize(string.Format("actions/{0}", ActionRefresh.Instance.Alias), _culture), splitId.CollectionId != "gateways" && !id.EndsWith("_resolved"));

            return menu;
        }


        /// <summary>
        /// Makes a route path id
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="storeId">
        /// The store Id.
        /// </param>
        /// <param name="collectionKey">
        /// Constructs the route path id for collection nodes.
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeCollectionRoutePathId(string collectionId, string storeId, string collectionKey)
        {
            return collectionKey.IsNullOrWhiteSpace()
                       ? $"{collectionId}_{storeId}"
                       : $"{collectionId}_{storeId}_{collectionKey}";
        }

        /// <summary>
        /// Gets tree nodes for collections.
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="parentRouteId">
        /// The parent route id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="collectionRoots">
        /// Indicates this is a collection root node
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetTreeNodesForCollections(string collectionId, string parentRouteId, FormDataCollection queryStrings, bool collectionRoots = true)
        {
            var splitId = new SplitRoutePath(parentRouteId);
            var info = this.GetCollectionProviderInfo(collectionId, splitId);

            // add any configured dynamic collections
            var currentTree = _rootTrees.FirstOrDefault(x => x.Id == splitId.CollectionId && x.Visible);

            var treeNodes = new List<TreeNode>();
            if (currentTree == null) return treeNodes;

            var managedFirst = true;
            if (currentTree.ChildSettings.Count > 0)
            {
                var setting =
                    currentTree.ChildSettings.AllSettings()
                        .First(x => x.Alias == "selfManagedProvidersBeforeStaticProviders");
                if (setting != null) managedFirst = bool.Parse(setting.Value);
            }

            if (managedFirst)
            {
                var sm = GetTreeNodesForSelfManagedProviders(currentTree, info, splitId, collectionId, parentRouteId, queryStrings).ToArray();
                if (sm.Any()) treeNodes.AddRange(sm);

                var sc = GetTreeNodesFromCollection(info, splitId, collectionId, parentRouteId, queryStrings, collectionRoots).ToArray();
                if (sc.Any()) treeNodes.AddRange(sc);
            }
            else
            {
                var sc = GetTreeNodesFromCollection(info, splitId, collectionId, parentRouteId, queryStrings, collectionRoots).ToArray();
                if (sc.Any()) treeNodes.AddRange(sc);

                var sm = GetTreeNodesForSelfManagedProviders(currentTree, info, splitId, collectionId, parentRouteId, queryStrings).ToArray();
                if (sm.Any()) treeNodes.AddRange(sm);
            }

            return treeNodes;
        }

        /// <summary>
        /// Gets tree nodes for static collections.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="splitId">
        /// The split id.
        /// </param>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="parentRouteId">
        /// The parent route id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="collectionRoots">
        /// The collection roots.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetTreeNodesFromCollection(CollectionProviderInfo info, SplitRoutePath splitId, string collectionId, string parentRouteId, FormDataCollection queryStrings, bool collectionRoots = true)
        {

            var collections = collectionRoots
                                  ? info.ManagedCollections.Where(x => x.ParentKey == null).OrderBy(x => x.SortOrder)
                                  : info.ManagedCollections.Where(x => x.ParentKey == splitId.CollectionKeyAsGuid())
                                        .OrderBy(x => x.SortOrder);

            var treeNodes = collections.Any() ?

                collections.Select(
                        collection =>
                        CreateTreeNode(
                            MakeCollectionRoutePathId(collectionId, splitId.StoreId, collection.Key.ToString()),
                            parentRouteId,
                            queryStrings,
                            collection.Name,
                            "icon-list",
                            info.ManagedCollections.Any(x => x.ParentKey == collection.Key),
                            string.Format("/merchello/merchello/{0}/{1}/store/{2}", info.ViewName, collection.Key, splitId.StoreId))).ToArray() :

                new TreeNode[] { };

            if (!treeNodes.Any()) return treeNodes;


            //// need to tag these nodes so that they can be filtered by the directive to select which 
            //// collections entities can be assigned to via the back office
            foreach (var tn in treeNodes)
            {
                tn.CssClasses.Add("static-collection");
            }

            return treeNodes;
        }

        /// <summary>
        /// Gets tree nodes for self managed collection providers.
        /// </summary>
        /// <param name="currentTree">
        /// The current tree.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="splitId">
        /// The split id.
        /// </param>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <param name="parentRouteId">
        /// The parent route id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetTreeNodesForSelfManagedProviders(
            TreeElement currentTree,
            CollectionProviderInfo info,
            SplitRoutePath splitId,
            string collectionId,
            string parentRouteId,
            FormDataCollection queryStrings)
        {
            var treeNodes = new List<TreeNode>();
            if (splitId.IsChildCollection) return treeNodes;

            // if there are no self managed providers - return 
            if (currentTree.SelfManagedEntityCollectionProviderCollections == null
                ||
                !currentTree.SelfManagedEntityCollectionProviderCollections.EntityCollectionProviders().Any(x => x.Visible))
                return treeNodes;

            return this.GetTreeNodeForConfigurationEntityCollectionProviders(currentTree, collectionId, info, splitId, queryStrings, parentRouteId);
        }

        /// <summary>
        /// The get tree node from configuration element.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="collectionId">
        /// The root collection type (e.g. sales, product, customer)
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="parentRouteId">The parent route id</param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetTreeNodeForConfigurationEntityCollectionProviders(TreeElement tree, string collectionId, CollectionProviderInfo info, SplitRoutePath splitId, FormDataCollection queryStrings, string parentRouteId)
        {
            // get the self managed providers
            var grouping = new List<Tuple<EntityCollectionProviderElement, EntityCollectionProviderDisplay>>();
            foreach (var element in
                tree.SelfManagedEntityCollectionProviderCollections.EntityCollectionProviders().Where(x => x.Visible))
            {
                Guid elementKey;
                if (!Guid.TryParse(element.Key, out elementKey))
                {
                    continue;
                }

                var providerDisplay =
                    this._entityCollectionProviderResolver.GetProviderAttributes()
                        .First(x => x.Key == elementKey)
                        .ToEntityCollectionProviderDisplay(splitId.StoreIdInt);
                if (providerDisplay != null)
                {
                    grouping.Add(new Tuple<EntityCollectionProviderElement, EntityCollectionProviderDisplay>(element, providerDisplay));
                }
            }

            if (!grouping.Any()) return Enumerable.Empty<TreeNode>();

            var treeNodes = new List<TreeNode>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var g in grouping)
            {
                if (!g.Item2.ManagedCollections.Any()) continue;

                var element = g.Item1;
                var provider = g.Item2;
                var collection = g.Item2.ManagedCollections.First();

                treeNodes.Add(
                    this.CreateTreeNode(
                        MakeCollectionRoutePathId(collectionId, splitId.StoreId, collection.Key.ToString()) + "_resolved",
                        parentRouteId,
                        queryStrings,
                        provider.LocalizedNameKey.IsNullOrWhiteSpace() ? provider.Name : this._textService.Localize(provider.LocalizedNameKey, this._culture),
                        element.Icon,
                        false,
                        string.Format("/merchello/merchello/{0}/{1}/store/{2}", info.ViewName, collection.Key, splitId.StoreId)));
            }

            return treeNodes;
        }

        /// <summary>
        /// The get tree node from configuration element.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <param name="parentTree">
        /// The parent tree.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode"/>.
        /// </returns>
        private TreeNode GetTreeNodeFromConfigurationElement(TreeElement tree, SplitRoutePath splitId, FormDataCollection queryStrings, TreeElement parentTree = null)
        {
            var hasSubs = tree.SubTree != null && tree.SubTree.GetTrees().Any();

            if (_collectiontrees.Contains(tree.Id))
                hasSubs = this.GetCollectionProviderInfo(tree.Id, splitId).ManagedCollections.Any()
                          || tree.SelfManagedEntityCollectionProviderCollections.EntityCollectionProviders().Any();

            if (_attributetrees.Contains(tree.Id))
            {
                hasSubs = GetAttributeDefinedTrees(queryStrings, splitId).Any();
            }

            return CreateTreeNode(
                MakeCollectionRoutePathId(tree.Id, splitId.StoreId, null),
                parentTree == null ? string.Empty : parentTree.Id,
                queryStrings,
                this.LocalizeTitle(tree),
                tree.Icon,
                hasSubs,
                $"{tree.RoutePath}/store/{splitId.StoreId}");
        }

        /// <summary>
        /// The localize title.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string LocalizeTitle(TreeElement tree)
        {
            var name = tree.LocalizeName.IsNullOrWhiteSpace() ? tree.Id : tree.LocalizeName;

            var localized = _textService.Localize(string.Format("{0}/{1}", tree.LocalizeArea, name), _culture);

            return localized.IsNullOrWhiteSpace() ? tree.Title : localized;
        }

        /// <summary>
        /// Adds attribute defined trees.
        /// </summary>
        /// <param name="queryStrings">
        /// The query Strings.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> GetAttributeDefinedTrees(FormDataCollection queryStrings, SplitRoutePath splitId)
        {
            var types = ReportApiControllerResolver.Current.ResolvedTypesWithAttribute.ToArray();
            if (!types.Any()) return new TreeNode[] { };

            var atts = types.Select(x => x.GetCustomAttribute<BackOfficeTreeAttribute>(true)).Where(x => x != null).OrderBy(x => x.SortOrder);

            // TODO RSS refactor
            return
                atts.Select(
                    att =>
                    CreateTreeNode(
                        att.RouteId,
                        att.ParentRouteId,
                        queryStrings,
                        att.Title,
                        att.Icon,
                        false,
                        string.Format("{0}{1}/store/{2}", "/merchello/merchello/reports.viewreport/", att.RouteId, splitId.StoreId)));
        }

        /// <summary>
        /// Resolves a provider and view for collection node.
        /// </summary>
        /// <param name="collectionId">
        /// The collection id.
        /// </param>
        /// <returns>
        /// The <see cref="CollectionProviderInfo"/>.
        /// </returns>
        private CollectionProviderInfo GetCollectionProviderInfo(string collectionId, SplitRoutePath splitId)
        {
            collectionId = collectionId.ToLowerInvariant();
            var info = new CollectionProviderInfo();

            switch (collectionId)
            {
                case "sales":
                    info.ManagedCollections =
                        this._entityCollectionProviderResolver.GetProviderAttribute<StaticInvoiceCollectionProvider>()
                            .ToEntityCollectionProviderDisplay(splitId.StoreIdInt).ManagedCollections;
                    info.ViewName = "saleslist";
                    break;
                case "customers":
                    info.ManagedCollections =
                        this._entityCollectionProviderResolver.GetProviderAttribute<StaticCustomerCollectionProvider>()
                            .ToEntityCollectionProviderDisplay(splitId.StoreIdInt).ManagedCollections;
                    info.ViewName = "customerlist";
                    break;
                default:
                    info.ManagedCollections =
                        this._entityCollectionProviderResolver.GetProviderAttribute<StaticProductCollectionProvider>()
                            .ToEntityCollectionProviderDisplay(splitId.StoreIdInt).ManagedCollections;
                    info.ViewName = "productlist";
                    break;
            }

            return info;
        }


        /// <summary>
        /// Initializes the tree with a current starting node.
        /// </summary>
        /// <param name="currentTree">
        /// The current Tree.
        /// </param>
        /// <param name="splitId">
        /// The split id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> InitializeTree(TreeElement currentTree, SplitRoutePath splitId, FormDataCollection queryStrings)
        {
            // collection tree
            if (_collectiontrees.Contains(splitId.CollectionId))
            {
                return this.GetTreeNodesForCollections(
                    splitId.CollectionId,
                    MakeCollectionRoutePathId(splitId.CollectionId, splitId.StoreId, splitId.CollectionKey),
                    queryStrings);
            }

            if (_attributetrees.Contains(splitId.CollectionId))
            {
                return GetAttributeDefinedTrees(queryStrings, splitId);
            }

            return currentTree.SubTree.GetTrees()
                    .Where(x => x.Visible)
                    .Select(tree => GetTreeNodeFromConfigurationElement(tree, splitId, queryStrings, currentTree));
        }

        /// <summary>
        /// Initializes the tree without a current starting node.
        /// </summary>
        /// <param name="splitId">
        /// The split id.
        /// </param>
        /// <param name="queryStrings">
        /// The query strings.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        private IEnumerable<TreeNode> InitializeTree(SplitRoutePath splitId, FormDataCollection queryStrings)
        {
            var backoffice = MerchelloConfiguration.Current.BackOffice;

            if (_collectiontrees.Contains(splitId.CollectionId))
            {
                return this.GetTreeNodesForCollections(
                    splitId.CollectionId,
                    MakeCollectionRoutePathId(splitId.CollectionId, splitId.StoreId, splitId.CollectionKey),
                    queryStrings,
                    false);
            }

            if (_attributetrees.Contains(splitId.CollectionId))
            {
                return GetAttributeDefinedTrees(queryStrings, splitId);
            }

            return backoffice.GetTrees()
                        .Where(x => x.Visible)
                        .Select(tree => GetTreeNodeFromConfigurationElement(tree, splitId, queryStrings));
        }


        /// <summary>
        /// The split route path.
        /// </summary>
        private class SplitRoutePath
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SplitRoutePath"/> class.
            /// </summary>
            /// <param name="routePath">
            /// The route path.
            /// </param>
            public SplitRoutePath(string routePath)
            {
                var tokens = routePath.Split('_');
                IsChildCollection = tokens.Length > 2;

                CollectionId = tokens.Length > 1 ? tokens[0] : routePath;
                StoreId = tokens.Length > 1 ? tokens[1] : string.Empty;
                CollectionKey = IsChildCollection ? tokens[2] : string.Empty;
            }

            /// <summary>
            /// Gets a value indicating whether is child collection.
            /// </summary>
            public bool IsChildCollection { get; private set; }

            /// <summary>
            /// Gets the collection id.
            /// </summary>
            public string CollectionId { get; private set; }

            /// <summary>
            /// Gets the store id.
            /// </summary>
            public string StoreId { get; private set; }

            /// <summary>
            /// Gets the store id as int.
            /// </summary>
            public int StoreIdInt { get { if (int.TryParse(StoreId, out int result)) return result; else return -1; } }

            /// <summary>
            /// Gets the collection key.
            /// </summary>
            public string CollectionKey { get; private set; }

            /// <summary>
            /// The collection key as guid.
            /// </summary>
            /// <returns>
            /// The <see cref="Guid"/>.
            /// </returns>
            public Guid? CollectionKeyAsGuid()
            {
                return !CollectionKey.IsNullOrWhiteSpace() ? new Guid(CollectionKey) as Guid? : null;
            }
        }

        /// <summary>
        /// The collection provider info.
        /// </summary>
        private class CollectionProviderInfo
        {
            /// <summary>
            /// Gets or sets the view name.
            /// </summary>
            public string ViewName { get; set; }

            /// <summary>
            /// Gets or sets the managed collections.
            /// </summary>
            public IEnumerable<EntityCollectionDisplay> ManagedCollections { get; set; }
        }
    }
}