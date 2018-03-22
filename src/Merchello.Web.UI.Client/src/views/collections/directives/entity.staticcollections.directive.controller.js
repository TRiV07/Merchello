angular.module('merchello').controller('Merchello.Directives.EntityStaticCollectionsDirectiveController',
    ['$scope', '$routeParams', 'notificationsService', 'dialogService', 'entityCollectionHelper', 'entityCollectionResource', 'dialogDataFactory', 'entityCollectionDisplayBuilder',
        function ($scope, $routeParams, notificationsService, dialogService, entityCollectionHelper, entityCollectionResource, dialogDataFactory, entityCollectionDisplayBuilder) {

            $scope.collections = [];
            $scope.remove = remove;

            // exposed methods
            $scope.openStaticEntityCollectionPicker = openStaticEntityCollectionPicker;

            function init() {
                $scope.$watch('preValuesLoaded', function (pvl) {
                    if (pvl) {
                        loadCollections();
                    }
                });
            }

            function loadCollections() {
                entityCollectionResource.getEntityCollectionsByEntity($scope.entity, $scope.entityType).then(function (collections) {
                    $scope.collections = entityCollectionDisplayBuilder.transform(collections);
                }, function (reason) {
                    notificationsService.error('Failed to get entity collections for entity ' + reason);
                });
            }

            function openStaticEntityCollectionPicker() {
                var dialogData = dialogDataFactory.createAddEditEntityStaticCollectionDialog();
                dialogData.entityType = $scope.entityType.toLocaleLowerCase();
                dialogData.collectionKeys = [];
                dialogData.storeId = $routeParams.storeId;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/pick.staticcollection.html',
                    show: true,
                    callback: processAddEditStaticCollection,
                    dialogData: dialogData
                });
            }

            function processAddEditStaticCollection(dialogData) {
                var key = $scope.entity.key;
                entityCollectionResource.addEntityToCollections(key, dialogData.collectionKeys).then(function () {
                    loadCollections();
                }, function (reason) {
                    notificationsService.error('Failed to add entity to collections ' + reason);
                });
            }

            function remove(collection) {
                if (confirm('Are you sure want to delete this product from "' + collection.name + '" collection?')) {
                    entityCollectionResource.removeEntityFromCollection($scope.entity.key, collection.key).then(function () {
                        loadCollections();
                    });
                }
            }

            // initialize the controller
            init();
        }]);
