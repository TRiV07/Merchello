angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerNewCustomerController',
    ['$scope', '$location', 'dialogDataFactory', 'customerResource', 'notificationsService', 'navigationService', 'customerDisplayBuilder',
        function($scope, $location, dialogDataFactory, customerResource, notificationsService, navigationService, customerDisplayBuilder) {
            $scope.wasFormSubmitted = false;

            $scope.firstName = '';
            $scope.lastName = '';
            $scope.email = '';

            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
            }

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.editInfoForm.email.$valid) {
                    var customer = customerDisplayBuilder.createDefault();
                    customer.loginName = $scope.email;
                    customer.email = $scope.email;
                    customer.firstName = $scope.firstName;
                    customer.lastName = $scope.lastName;

                    if (customer.extendedData.items.length <= 0) {
                        customer.extendedData.items = null;
                    }

                    var promiseSaveCustomer = customerResource.AddCustomer(customer, $scope.dialogData.storeId);
                    promiseSaveCustomer.then(function (customerResponse) {
                        notificationsService.success("Customer Saved", "");
                        navigationService.hideNavigation();
                        $location.url("/merchello/merchello/customeroverview/" + customerResponse.key + '/store/' + $scope.dialogData.storeId, true);
                    }, function (reason) {
                        notificationsService.error("Customer Save Failed", reason.message);
                    });
                }
            }

            init();
        }]);
