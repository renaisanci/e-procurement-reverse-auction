(function (app) {
    'use strict';

    app.controller('modalMotivoRecusaMembroCtrl', modalMotivoRecusaMembroCtrl);

    modalMotivoRecusaMembroCtrl.$inject = ['$scope', 'notificationService', 'apiService', '$modalInstance', 'SweetAlert'];

    function modalMotivoRecusaMembroCtrl($scope, notificationService, apiService, $uibModalInstance, SweetAlert) {



        $scope.membro = $scope.novoAceitaMembro;
        $scope.fornecedorNaoAceitaMembro = fornecedorNaoAceitaMembro;
        $scope.recusaMembro = {};

        $scope.cancel = cancel;



        function fornecedorNaoAceitaMembro() {
            var mbr = {};
            var membro = $scope.novoAceitaMembro;

            mbr = {
                MembroId: membro.Id,
                Observacao: $scope.recusaMembro.DescRecusa

            };

            apiService.post('/api/fornecedor/fornecedorRecusaMembro', mbr,
            fornecedorRecusaMembroSucesso,
            fornecedorRecusaMembroFalha);

        }

        function fornecedorRecusaMembroSucesso(result) {

            cancel();

            SweetAlert.swal({
                title: "SUA RESPOSTA FOI ENCAMINHADA PARA O MEMBRO!",
                text: "Agora ele só poderá te encaminhar uma nova solicitação assim que o mesmo regularizar " +
                    "a situação junto a você!",
                type: "success",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            },
                  function () {

                      $scope.habilitaDesabilitaAbas();
                      $scope.pesquisarNovoMembroSolicitaFornecedor();

                  });
        }

        function fornecedorRecusaMembroFalha(result) {

            notificationService.displayError('Erro ao salvar sua recusa para este membro, tente novamente!');

        }



        //----------------Fechar Modal------------------------------
        function cancel() {

            $uibModalInstance.dismiss();
        };
        //----------------Fim Modal----------------------------------



    }

})(angular.module('ECCFornecedor'));