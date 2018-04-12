/**
 * @ngdoc resource
 * @name paymentGatewayProviderResource
 * @description Loads in data for payment providers
 **/
angular.module('merchello.resources').factory('paymentGatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function ($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'];

            return {
                getGatewayResources: function (paymentGatewayProviderKey, storeId) {
                    var url = baseUrl + 'GetGatewayResources';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: paymentGatewayProviderKey, storeId: storeId }
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function (storeId) {
                    var url = baseUrl + 'GetAllGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { storeId: storeId }
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey, storeId) {
                    var url = baseUrl + 'GetPaymentProviderPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: paymentGatewayProviderKey, storeId: storeId }
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                getAvailablePaymentMethods: function (storeId) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAvailablePaymentMethods',
                            method: "GET",
                            params: { storeId: storeId }
                        }),
                        'Failed to load payment methods');
                },

                getPaymentMethodByKey: function (paymentMethodKey, storeId) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetPaymentMethodByKey',
                            method: "GET",
                            params: { key: paymentMethodKey, storeId: storeId }
                        }),
                        'Failed to payment method: ' + paymentMethodKey);
                },

                addPaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'AddPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'PutPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey, storeId) {
                    var url = baseUrl + 'DeletePaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: paymentMethodKey, storeId: storeId }
                        }),
                        'Failed to delete paymentMethod');
                }

            };

        }]);
