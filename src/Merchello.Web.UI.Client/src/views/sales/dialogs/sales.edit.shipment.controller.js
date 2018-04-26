'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.EditShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        $scope.checkboxDisabled = checkboxDisabled;

        function init() {
            _.each($scope.dialogData.shipment.items, function(item) {
                item.selected = true;
            });
            $scope.loaded = true;
        }

        function checkboxDisabled() {
            return $scope.dialogData.shipment.shipmentStatus.key === 'b37be101-cec9-4608-9330-54e56fa0537a' || $scope.dialogData.shipment.shipmentStatus.key === '3a279633-4919-485d-8c3b-479848a053d9' || $scope.dialogData.shipment.shipmentStatus.key === '5f22d354-81a4-4356-bee4-aa8c0f717438'
        }

        function save() {
            $scope.dialogData.shipment.items = _.filter($scope.dialogData.shipment.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);

