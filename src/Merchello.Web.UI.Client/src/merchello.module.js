// Bootstrap the Merchello angular module
(function () {

    angular.module('merchello', [
        'umbraco.filters',
        'umbraco.directives',
        'umbraco.services',
        'merchello.filters',
        'merchello.directives',
        'merchello.plugins',
        'merchello.resources',
        'merchello.services',
        'ngStorage'
    ]).config(['$sessionStorageProvider', '$localStorageProvider', '$routeProvider',
        function ($sessionStorageProvider, $localStorageProvider, $routeProvider) {
            $sessionStorageProvider.setKeyPrefix('merchello-');
            $localStorageProvider.setKeyPrefix('merchello-');

            var canRoute = function (isRequired) {

                return {
                    /** Checks that the user is authenticated, then ensures that are requires assets are loaded */
                    isAuthenticatedAndReady: function ($q, userService, $route, assetsService, appState) {
                        var deferred = $q.defer();

                        //don't need to check if we've redirected to login and we've already checked auth
                        if (!$route.current.params.section
                            && ($route.current.params.check === false || $route.current.params.check === "false")) {
                            deferred.resolve(true);
                            return deferred.promise;
                        }

                        userService.isAuthenticated()
                            .then(function () {

                                assetsService._loadInitAssets().then(function () {

                                    //This could be the first time has loaded after the user has logged in, in this case
                                    // we need to broadcast the authenticated event - this will be handled by the startup (init)
                                    // handler to set/broadcast the ready state
                                    var broadcast = appState.getGlobalState("isReady") !== true;

                                    userService.getCurrentUser({ broadcastEvent: broadcast }).then(function (user) {
                                        //is auth, check if we allow or reject
                                        if (isRequired) {

                                            //This checks the current section and will force a redirect to 'content' as the default
                                            if ($route.current.params.section.toLowerCase() === "default" || $route.current.params.section.toLowerCase() === "umbraco" || $route.current.params.section === "") {
                                                $route.current.params.section = "content";
                                            }

                                            // U4-5430, Benjamin Howarth
                                            // We need to change the current route params if the user only has access to a single section
                                            // To do this we need to grab the current user's allowed sections, then reject the promise with the correct path.
                                            if (user.allowedSections.indexOf($route.current.params.section) > -1) {
                                                //this will resolve successfully so the route will continue
                                                deferred.resolve(true);
                                            } else {
                                                deferred.reject({ path: "/" + user.allowedSections[0] });
                                            }
                                        }
                                        else {
                                            deferred.reject({ path: "/" });
                                        }
                                    });

                                });

                            }, function () {
                                //not auth, check if we allow or reject
                                if (isRequired) {
                                    //the check=false is checked above so that we don't have to make another http call to check
                                    //if they are logged in since we already know they are not.
                                    deferred.reject({ path: "/login/false" });
                                }
                                else {
                                    //this will resolve successfully so the route will continue
                                    deferred.resolve(true);
                                }
                            });
                        return deferred.promise;
                    }
                };
            };

            $routeProvider
                .when('/:section/:tree/:method/:id/store/:storeId', {
                    //This allows us to dynamically change the template for this route since you cannot inject services into the templateUrl method.
                    template: "<div ng-include='templateUrl'></div>",
                    //This controller will execute for this route, then we replace the template dynamnically based on the current tree.
                    controller: function ($scope, $route, $routeParams, treeService) {

                        if (!$routeParams.tree || !$routeParams.method) {
                            $scope.templateUrl = "views/common/dashboard.html";
                        }

                        // Here we need to figure out if this route is for a package tree and if so then we need
                        // to change it's convention view path to:
                        // /App_Plugins/{mypackage}/backoffice/{treetype}/{method}.html

                        // otherwise if it is a core tree we use the core paths:
                        // views/{treetype}/{method}.html

                        var packageTreeFolder = treeService.getTreePackageFolder($routeParams.tree);

                        if (packageTreeFolder) {
                            $scope.templateUrl = (Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath +
                                "/" + packageTreeFolder +
                                "/backoffice/" + $routeParams.tree + "/" + $routeParams.method + ".html");
                        }
                        else {
                            $scope.templateUrl = ('views/' + $routeParams.tree + '/' + $routeParams.method + '.html');
                        }

                    },
                    resolve: canRoute(true)
                });
        }]);

    angular.module('merchello.models', []);
    angular.module('merchello.filters', []);
    angular.module('merchello.directives', []);
    angular.module('merchello.resources', ['merchello.models']);
    angular.module('merchello.services', ['merchello.models']);
    angular.module('merchello.plugins', ['chart.js']);
    //// Inject our dependencies
    angular.module('umbraco.packages').requires.push('merchello');

}());


