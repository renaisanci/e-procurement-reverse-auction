
(function (app) {
    'use strict';

    app.controller('modalDetProdPromocaoCtrl', modalDetProdPromocaoCtrl);

    modalDetProdPromocaoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService'];

    function modalDetProdPromocaoCtrl($scope, $modalInstance, $timeout, apiService, notificationService) {

        $scope.close = close;
        

        //1---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------



    }

})(angular.module('ECCFornecedor'));