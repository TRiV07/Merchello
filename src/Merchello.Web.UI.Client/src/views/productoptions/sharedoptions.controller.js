angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope', '$log', '$q', 'merchelloTabsFactory', 'localizationService', 'productOptionResource', 'dialogDataFactory', 'dialogService', 'userService',
        'merchelloListViewHelper',
        function ($scope, $log, $q, merchelloTabsFactory, localizationService, productOptionResource, dialogDataFactory, dialogService, userService, merchelloListViewHelper) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;

            $scope.tabs = [];

            // In the initial release of this feature we are only going to allow sharedOnly params
            // to be managed here.  We may open this up at a later date depending on feedback.
            $scope.sharedOnly = true;

            function init() {
                userService.getCurrentUser().then(function (user) {
                    var isMainAdmin = _.find(user.startContentIds, function (id) {
                        if (id == -1) return id;
                    }) == -1;
                    $scope.tabs = merchelloTabsFactory.createProductListTabs(isMainAdmin);
                    $scope.tabs.setActive('sharedoptions');
                });
            }


            $scope.load = function (query) {
                query.addSharedOptionOnlyParam($scope.sharedOnly);
                return productOptionResource.searchOptions(query);
            }

            // adds an option
            $scope.add = function (option) {
                // this is the toggle to relead in the directive
                $scope.preValuesLoaded = false;

                productOptionResource.addProductOption(option).then(function (o) {
                    $scope.preValuesLoaded = true;
                });
            }

            $scope.edit = function (option) {
                // this is the toggle to relead in the directive
                $scope.preValuesLoaded = false;
                productOptionResource.saveProductOption(option).then(function (o) {
                    $scope.preValuesLoaded = true;
                });
            }

            $scope.delete = function (option) {
                if (option.canBeDeleted()) {
                    $scope.preValuesLoaded = false;

                    productOptionResource.deleteProductOption(option).then(function () {
                        $scope.preValuesLoaded = true;
                    });
                }
            }

            init();
        }]);
