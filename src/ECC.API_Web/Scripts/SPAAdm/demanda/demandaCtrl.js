(function (app) {
    'use strict';

    app.controller('demandaCtrl', demandaCtrl);

    demandaCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService'];

    function demandaCtrl($scope, $modalInstance, $timeout, apiService, notificationService) {

        $scope.periodicidadesModal = [];
        $scope.unidadesMedidaModal = [];
        $scope.categoriasModal = $scope.categorias;
        $scope.subcategoriasModal = [];
        $scope.membroCategoriaModal = {};
        $scope.loadSubCategoriaModal = loadSubCategoriaModal;
        $scope.loadPeriodicidadeModal = loadPeriodicidadeModal;
        $scope.loadUnidadesMedidaModal = loadUnidadesMedidaModal;
        $scope.subCategoriaModalSelecionada = {};
        $scope.cancel = cancel;
        $scope.inserirDemanda = inserirDemanda;
        $scope.carregarSubCategoriaSelecionada = carregarSubCategoriaSelecionada;
        $scope.subCategoriaModalSelecionada = subCategoriaModalSelecionada;

        //1---------Fecha Modal---------------------
        function cancel() {
            $modalInstance.dismiss();
        }
        //------------------------------------------

        function subCategoriaModalSelecionada(data) {
            $scope.novoMembroDemanda.SubCategoriaId = data.originalObject.Id;
        }

        //2---------Insere ou Atualiza Demanda------
        function inserirDemanda() {
            $scope.novoMembroDemanda.MembroId = $scope.novoMembro.Id;
            if ($scope.novoMembroDemanda.Observacao == undefined)
                $scope.novoMembroDemanda.Observacao = "";
            if ($scope.novoMembroDemanda.Id > 0) {
                atualizarModelDemanda();
            } else {
                inserirModelDemanda();
            }
        }

        //------------------------------------------

        //2---------Insere  Demanda----------------
        function inserirModelDemanda() {

            apiService.post('/api/membroDemanda/inserir', $scope.novoMembroDemanda,
                inserirModelDemandaSucesso,
                inserirModelDemandaFalha);
        }

        function inserirModelDemandaSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoMembroDemanda = response.data;
            $modalInstance.dismiss();
        }

        function inserirModelDemandaFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //------------------------------------------


        //3-----Atualiza Demanda-------------------------------
        function atualizarModelDemanda() {
            apiService.post('/api/membroDemanda/atualizar', $scope.novoMembroDemanda,
                atualizarDemandaSucesso,
                atualizarDemandaFalha);
        }

        function atualizarDemandaSucesso(response) {
            notificationService.displaySuccess(' Atualizado com Sucesso.');
            $scope.novoMembroDemanda = response.data;

        }

        function atualizarDemandaFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //3 ----------------------------Fim---------------------------------------//

        function loadSubCategoriaModal(categoriaModalId) {
            $scope.loadingSubCategoria = true;
            var config = {
                params: {
                    categoriaId: categoriaModalId
                }
            };

            apiService.get('/api/subcategoria/consultar', config,
                    loadSubCategoriaSucessoModal,
                    loadSubCategoriaFailedModal);
        }

        function loadSubCategoriaSucessoModal(response) {
            $scope.subcategoriasModal = response.data;
            if ($scope.novoMembroDemanda.SubCategoriaId > 0) {
                for (var i = 0; i < $scope.subcategoriasModal.length; i++) {
                    if ($scope.subcategoriasModal[i].Id == $scope.novoMembroDemanda.SubCategoriaId) {
                        $scope.subCategoriaModalInitial = $scope.subcategoriasModal[i];
                    }
                }
            }
        }

        function loadSubCategoriaFailedModal(response) {
            notificationService.displayError(response.data);
        }

        ///----- INÍCIO PERIODICIDADE  ---- ////
        function loadPeriodicidadeModal() {

            apiService.get('/api/periodicidade/consultar', null,
                    loadPeriodicidadeSucessoModal,
                    loadPeriodicidadeFailedModal);
        }

        function loadPeriodicidadeSucessoModal(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.DescPeriodicidade = "Periodicidade ...";

            };
            response.data.push(newItem);

            $scope.periodicidadesModal = response.data;
        }

        function loadPeriodicidadeFailedModal(response) {
            notificationService.displayError(response.data);
        }
        ///----- FIM PERIODICIDADE  ---- ////

        ///----- INÍCIO UNIDADE MEDIDA  ---- ////
        function loadUnidadesMedidaModal() {

            $scope.loadingUnidadeMedida = true;

            apiService.get('/api/unidadeMedida/consultar', null,
                    loadUnidadesMedidaSucessoModal,
                    loadUnidadesMedidaFailedModal);
        }

        function loadUnidadesMedidaSucessoModal(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.DescUnidadeMedida = "Unidade Medida ...";

            };
            response.data.push(newItem);

            $scope.unidadesMedidaModal = response.data;
        }

        function loadUnidadesMedidaFailedModal(response) {
            notificationService.displayError(response.data);
        }

        function carregarSubCategoriaSelecionada() {
            if ($scope.novoMembroDemanda.CategoriaId > 0) {
                loadSubCategoriaModal($scope.novoMembroDemanda.CategoriaId);
            }
        }

        ///----- FIM PERIODICIDADE  ---- ////
        $scope.loadPeriodicidadeModal();
        $scope.loadUnidadesMedidaModal();
        $scope.carregarSubCategoriaSelecionada();
    }

})(angular.module('ECCAdm'));