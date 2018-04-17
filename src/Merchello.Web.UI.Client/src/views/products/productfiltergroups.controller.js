angular.module('merchello').controller('Merchello.Backoffice.ProductFilterGroupsController',
    ['$scope', '$routeParams', 'entityCollectionResource', 'merchelloTabsFactory', 'userService',
        function ($scope, $routeParams, entityCollectionResource, merchelloTabsFactory, userService) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;

            $scope.entityType = 'Product';

            $scope.tabs = [];

            $scope.add = function (collection) {
                $scope.preValuesLoaded = false;
                entityCollectionResource.addEntityCollection(collection, $routeParams.storeId).then(function (result) {
                    $scope.preValuesLoaded = true;
                });
            }

            $scope.edit = function (collection) {
                $scope.preValuesLoaded = false;
                entityCollectionResource.putEntityFilterGroup(collection).then(function (result) {
                    $scope.preValuesLoaded = true;
                });
            }

            function init() {

                userService.getCurrentUser().then(function (user) {
                    var isMainAdmin = _.find(user.startContentIds, function (id) {
                        if (id == -1) return id;
                    }) == -1;

                    $scope.tabs = merchelloTabsFactory.createProductListTabs(isMainAdmin);
                    $scope.tabs.setActive('filtergroups');

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                });
            }

            init();
        }]);
