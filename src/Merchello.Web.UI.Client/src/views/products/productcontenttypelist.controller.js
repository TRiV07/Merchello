angular.module('merchello').controller('Merchello.Backoffice.ProductContentTypeListController',
    ['$scope', 'merchelloTabsFactory', 'userService',
        function ($scope, merchelloTabsFactory, userService) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.tabs = {};

            function init() {
                userService.getCurrentUser().then(function (user) {
                    var isMainAdmin = _.find(user.startContentIds, function (id) {
                        if (id == -1) return id;
                    }) == -1;
                    $scope.tabs = merchelloTabsFactory.createProductListTabs(isMainAdmin);
                    $scope.tabs.setActive('contentTypeList');
                });
            }


            // Initializes the controller
            init();
        }]);
