
(function (app) {
    'use strict';

    app.controller('aceiteTermoUsoCtrl', aceiteTermoUsoCtrl);

    aceiteTermoUsoCtrl.$inject = ['$scope', '$sce', '$location', '$modalInstance', '$timeout', 'apiService', 'SweetAlert', 'membershipService', '$modal'];

    function aceiteTermoUsoCtrl($scope, $sce, $location, $modalInstance, $timeout, apiService, SweetAlert, membershipService, $modal) {

        $scope.close = close;
        $scope.recusar = recusar;
        $scope.aceitar = aceitar;
        $scope.conteudo = $sce.trustAsHtml($scope.TermoUso.Documento);
        $scope.openTermprivacidade = openTermprivacidade;

        //---------Recusar---------------------

        function recusar() {


            SweetAlert.swal({
                title: "Para usar a plataforma, é necessário aceitar o Termo de Uso e a Politica de Privacidade. Caso não aceite você será deslogado do sistema. Tem certeza que deseja sair do sistema?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok",
                cancelButtonText: "Cancelar",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {

                        logout();

                    } else {

                    }
                });

        }


        //---------Recusar---------------------

        //---------Aceitar---------------------

        function aceitar() {
            var config = {
                params: {
                    termoUsoId: $scope.TermoUso.Id
                }
            };

            apiService.get("/api/usuario/aceitoTermoUso", config,
                aceitoCompleted,
                aceitoUsoFailed);
        }

        function aceitoCompleted(response) {
            close();
        }

        function aceitoUsoFailed(response) {

        }

        //---------Aceitar---------------------

        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //---------Fecha Modal---------------------

        //---------Sair do sistema---------------------
        function logout() {

            membershipService.removeCredentials();
            $location.path("/login");
            $scope.userData.displayUserInfo();
        }
        //---------Sair do sistema---------------------




        //2-----Abre popup termo de privacidade----------------------------------

        function openTermprivacidade() {

            $scope.btFechaModal = true;

            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/termoUso/privacidade.html',
                controller: 'privacidadeCtrl',
                scope: $scope
            }).result.then(function ($scope) {

                //console.log("Modal Closed!!!");

            }, function () {

                //console.log("Modal Dismissed!!!");
            });


        }


        //---------------------------------------------------------------------------------------
    }

})(angular.module('ECCFornecedor'));