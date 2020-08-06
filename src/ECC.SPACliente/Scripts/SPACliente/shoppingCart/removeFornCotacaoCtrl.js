
(function (app) {
    'use strict';

    app.controller('removeFornCotacaoCtrl', removeFornCotacaoCtrl);

    removeFornCotacaoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'storeService', '$compile', 'uiCalendarConfig', '$filter', 'SweetAlert'];

    function removeFornCotacaoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, storeService, $compile, uiCalendarConfig, $filter, SweetAlert) {

        $scope.close = close;



        //1---------Fecha Modal---------------------
        function close() {
            $scope.$parent.carregaCarrinho();
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------

        //2-----Carrega fornecedores do membro------------
        function loadMembroFornecedor() {

            var config = {
                params: {
                    categoriaId: $scope.categoriaId
                }
            };


            apiService.get('/api/membro/membroFornecedoresProduto', config,
                    loadMembroFornecedorSucesso,
                    loadMembroFornecedorFalha);
        }

        function loadMembroFornecedorSucesso(response) {

            $scope.fornecedoresProd = response.data;
            for (var i = 0; i < $scope.fornecedoresProd.length; i++) {

                for (var j = 0; j < $scope.fornRemPedCot.length; j++) {
                    if ($scope.fornecedoresProd[i].FornecedorId === $scope.fornRemPedCot[j].forn && $scope.fornRemPedCot[j].prd === $scope.prdId) {
                        $scope.fornecedoresProd[i].checked = true;
                        break;
                    } else {
                        $scope.fornecedoresProd[i].checked = false;
                    }

                }


            }
        }

        function loadMembroFornecedorFalha(response) {
            notificationService.displayError(response.data);
        }
        //----Fim---------------------------------------


        //3-----Add fornecedor que serão removidos da cotação do pedido------------

        $scope.addFornecedorRem = function addFornecedorRem(idForn, checado, fornecedor) {

            var checkedForn = checado;
            var fornReceb = idForn;
            var prdReceb = $scope.prdId;
            var existeForPrd = false;
            var conotaPrdRemForn = 0;
            

            if ($scope.fornRemPedCot != null || $scope.fornRemPedCot != undefined) {
                for (var p = 0; p < $scope.fornRemPedCot.length; p++) {
                    if ($scope.fornRemPedCot[p].prd == prdReceb && checado) {

                        conotaPrdRemForn++;
                    }
                }
            }
            var qtdFora = $scope.fornecedoresProd.length - conotaPrdRemForn ;

            if (qtdFora < 3) {

                notificationService.displayWarning("Ao menos 2 fornecedores deve participar da cotação deste item.");
               
                return;
            } else {

                if ($scope.fornRemPedCot.length > 0) {
                    for (var i = 0; i < $scope.fornRemPedCot.length; i++) {
                        if (checkedForn) {
                            for (var k = 0; k < $scope.fornRemPedCot.length; k++) {
                                if (($scope.fornRemPedCot[k].forn == fornReceb && $scope.fornRemPedCot[k].prd == prdReceb)) {
                                    existeForPrd = true;
                                }
                            }
                            if (!($scope.fornRemPedCot[i].forn == fornReceb && $scope.fornRemPedCot[i].prd == prdReceb) && existeForPrd == false) {

                                $scope.fornRemPedCot.push({
                                    forn: fornReceb,
                                    prd: prdReceb
                                });
                                if (localStorage != null && JSON != null) {
                                    localStorage["fornRemPedCot"] = JSON.stringify($scope.fornRemPedCot);
                                }

                            }
                        } else {
                            if (($scope.fornRemPedCot[i].forn == idForn && $scope.fornRemPedCot[i].prd == prdReceb)) {

                                var fornIndex = $scope.fornRemPedCot[i].forn;

                                $scope.fornRemPedCot.splice(i, 1);

                                for (var j = 0; j < $scope.fornecedoresProd.length; j++) {

                                    if ($scope.fornecedoresProd[j].FornecedorId == fornIndex)
                                        $scope.fornecedoresProd[j].checked = false;
                                }

                                localStorage["fornRemPedCot"] = JSON.stringify($scope.fornRemPedCot);

                               
                            }
                        }
                    }
                } else {
                    if (checkedForn) {

                        $scope.fornRemPedCot.push(
                            {
                                forn: fornReceb,
                                prd: prdReceb
                            });

                        if (localStorage != null && JSON != null) {
                            localStorage["fornRemPedCot"] = JSON.stringify($scope.fornRemPedCot);
                        }



                    }
                }
            }
            //$scope.$parent.countPrdFornRem(prdReceb);
            $scope.$emit('countPrdFornRemCall', prdReceb);


        };

        //----Fim---------------------------------------

        loadMembroFornecedor();
    }

})(angular.module('ECCCliente'));