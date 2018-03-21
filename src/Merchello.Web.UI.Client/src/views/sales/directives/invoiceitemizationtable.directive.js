angular.module('merchello.directives').directive('invoiceItemizationTable',
    ['$q', '$routeParams', '$timeout', 'localizationService', 'invoiceResource', 'invoiceHelper', 'dialogService', 'productResource', 'notificationsService',
        function ($q, $routeParams, $timeout, localizationService, invoiceResource, invoiceHelper, dialogService, productResource, notificationsService) {
            return {
                restrict: 'E',
                replace: true,
                scope: {
                    invoice: '=',
                    payments: '=',
                    allPayments: '=',
                    paymentMethods: '=',
                    preValuesLoaded: '=',
                    currencySymbol: '=',
                    canEditLineItems:'=',
                    save: '&',
                    reload:'&'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/invoiceitemizationtable.tpl.html',
                link: function (scope, elm, attr) {

                    scope.loaded = false;
                    scope.authorizedCapturedLabel = '';
                    scope.taxTotal = 0;
                    scope.shippingTotal = 0;
                    scope.customLineItems = [];
                    scope.discountLineItems = [];
                    scope.adjustmentLineItems = [];
                    scope.remainingBalance = 0;

                    scope.itemization = {};

                    function init() {

                        // ensure that the parent scope promises have been resolved
                        scope.$watch('preValuesLoaded', function (pvl) {
                            if (pvl === true) {
                                loadInvoice();
                            }
                        });
                    }


                    // Previews a line item on invoice in a dialog
                    scope.lineItemPreview = function(sku) {

                        // Setup the dialog data
                        var dialogData = {
                            product: {},
                            sku: sku
                        };

                        // Get the product if it exists! We call the vairant service as this seems
                        // to return the base product too
                        productResource.getVariantBySku(sku, $routeParams.storeId).then(function (result) {
                            // If we get something back then add it to the diaglogData
                            if (result) {
                                dialogData.product = result;
                            }
                        });

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.previewlineitem.html',
                            show: true,
                            dialogData: dialogData
                        });
                    };


                    // The dialog that deals with lineitem quantity changes and deletions
                    scope.editLineItem = function (lineItem, lineItemType) {

                        var dialogData = {
                            quantity: lineItem.quantity,
                            lineItem: lineItem,
                            deleteLineItem: false,
                            canDelete: scope.invoice.items.length > 1,
                            lineItemType: lineItemType
                        };

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.edit.lineitem.html',
                            show: true,
                            dialogData: dialogData,
                            callback: updateLineItem
                        });

                    };

                    // Update a product line item on the invoice (Edit or Delete)
                    function updateLineItem(lineItemDialogData) {

                        var keepFindingProduct = true;
                        // Post the model back to the controller
                        var invoiceAddItems = {};

                        if (lineItemDialogData.deleteLineItem) {

                            // Loop through items                           
                            angular.forEach(scope.invoice.items, function (item) {
                                if (keepFindingProduct) {
                                    if (lineItemDialogData.lineItem.sku === item.sku) {

                                        // Make an invoice AddItemsModel
                                        invoiceAddItems = {
                                            InvoiceKey: scope.invoice.key,
                                            LineItemType: lineItemDialogData.lineItemType,
                                            Items: [
                                                {
                                                    Sku: item.sku,
                                                    Quantity: 0
                                                }
                                            ]
                                        }

                                        // Stop finding and break (As no break in angular loop, this is best way)
                                        keepFindingProduct = false;
                                    }
                                }
                            });

                        } else {

                            // See if the quantity has changed and then        
                            angular.forEach(scope.invoice.items, function (item) {
                                if (keepFindingProduct) {
                                    if (lineItemDialogData.lineItem.sku === item.sku
                                        && lineItemDialogData.quantity !== item.quantity) {

                                        // Make an invoice AddItemsModel
                                        invoiceAddItems = {
                                            InvoiceKey: scope.invoice.key,
                                            LineItemType: lineItemDialogData.lineItemType,
                                            Items: [
                                                {
                                                    Sku: item.sku,
                                                    Quantity: lineItemDialogData.quantity,
                                                    OriginalQuantity: item.quantity
                                                }
                                            ]
                                        }

                                        keepFindingProduct = false;
                                    }   
                                }
                            });                           
                        }

                        // Put the new items
                        var invoiceSavePromise = invoiceResource.putInvoiceNewProducts(invoiceAddItems);
                        invoiceSavePromise.then(function () {
                            $timeout(function () {
                                scope.reload();
                                loadInvoice();
                                notificationsService.success('Invoice updated.');
                            }, 1500);
                        }, function (reason) {
                            notificationsService.error("Failed to update invoice", reason.message);
                        });


                    };

                    function loadInvoice() {
                        var taxLineItem = scope.invoice.getTaxLineItem();
                        scope.taxTotal = taxLineItem !== undefined ? taxLineItem.price : 0;
                        scope.shippingTotal = scope.invoice.shippingTotal();

                        scope.customLineItems = scope.invoice.getCustomLineItems();
                        scope.discountLineItems = scope.invoice.getDiscountLineItems();
                        scope.adjustmentLineItems = scope.invoice.getAdjustmentLineItems();

                        angular.forEach(scope.adjustmentLineItems, function(item) {
                            item.userName = item.extendedData.getValue("userName");
                            item.email = item.extendedData.getValue("email");
                        });

                        scope.remainingBalance = invoiceHelper.round(scope.invoice.remainingBalance(scope.allPayments), 2);

                        var label  = scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';

                        $q.all([
                            localizationService.localize(label),
                            invoiceResource.getItemItemization(scope.invoice.key)
                        ]).then(function(data) {
                            scope.authorizedCapturedLabel = data[0];
                            scope.itemization = data[1];
                            scope.loaded = true;
                        });

                    }

                    // initialize the directive
                    init();
                }
            }
        }]);
