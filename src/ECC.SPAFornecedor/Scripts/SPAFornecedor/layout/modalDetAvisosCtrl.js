
(function (app) {
    'use strict';

    app.controller('modalDetAvisosCtrl', modalDetAvisosCtrl);

    modalDetAvisosCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert', '$rootScope', '$location', '$state'];

    function modalDetAvisosCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert, $rootScope, $location, $state) {


        $scope.close = close;
        $scope.loadAvisoClick = loadAvisoClick;



        // 01--------Carrega as notificações do usuário e da empresa do mesmo-----------

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
        // 01---------------------------------------------------------------------------
 
        carregaAvisos();
        //2---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss();
        }
        //2------------------------------------------


        //03---------------Aviso Click ----------------
        function loadAvisoClick(pAviso) {

            var xPath = pAviso.URLPaginaDestino.replace("/#", "");
            var url = $location.url();

            if (xPath.toUpperCase() == "/MEUSPEDIDOS") {
                $rootScope.Referencia = {
                    Id: pAviso.IdReferencia,
                    TipoAvisosId: pAviso.TipoAvisosId
                };
            }

            if (url.toUpperCase() != xPath.toUpperCase()) {
                $location.path(xPath);
            } else {
                $state.reload();
            }

            $rootScope.$emit("reloadAvisosForn", {});
        }
        //03------------------------------------------

    }

})(angular.module('ECCFornecedor'));