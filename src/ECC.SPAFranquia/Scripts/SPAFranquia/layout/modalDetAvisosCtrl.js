
(function (app) {
    'use strict';

    app.controller('modalDetAvisosCtrl', modalDetAvisosCtrl);

    modalDetAvisosCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert'];

    function modalDetAvisosCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert) {


        $scope.close = close;



        // 04--------Carrega as notificações do usuário e da empresa do mesmo-----------

        function carregaAvisos() {


            var config = {
                params: {

                    perfilModulo: 4,
                    TipoAvisosId: $scope.aviso.Id


                }
            };
        
                apiService.get('/api/avisos/avisosUsuarioEmpresaTp', config,
                        loadAvisosSucesso,
                        loadAvisosFailed);
        }

        function loadAvisosSucesso(response) {
            $scope.Avisos = response.data.avisos;

          
        }

        function loadAvisosFailed(response) {
            notificationService.displayError(response.data);
        }
        // 04---------------------------------------------------------------------------
 
        carregaAvisos();
        //4---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss();
        }
        //4------------------------------------------



    }

})(angular.module('ECCFranquia'));