
(function (app) {
    'use strict';

    app.controller('promocaoRecusadaCtrl', promocaoRecusadaCtrl);

    promocaoRecusadaCtrl.$inject = ['$scope', '$modalInstance'];

    function promocaoRecusadaCtrl($scope, $modalInstance) {

      
        $scope.close = close;

        //15---------Fecha Modal-------------------------------------
        function close() {
            $modalInstance.dismiss();
        }
        //15---------Fecha Modal-------------------------------------
     


    }

})(angular.module('ECCFornecedor'));