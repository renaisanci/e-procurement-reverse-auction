(function (app) {
    'use strict';

    app.directive('componentRatingFornecedorAvaliacao', componentRatingFornecedorAvaliacao);

    app.$inject = ['$scope', 'apiService', '$sce', 'notificationService'];

    function componentRatingFornecedorAvaliacao(apiService, notificationService) {

        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs) {
                
                $element.raty({
                    score: $attrs.componentRatingFornecedorAvaliacao,
                    halfShow: false,
                    readOnly: false,
                    scope: $scope,
                    noRatedMsg: "Não Avaliado",
                    starHalf: "../Content/images/raty/star-half-big.png",
                    starOff: "../Content/images/raty/ranking-estelar-cinza.jpg",
                    starOn: "../Content/images/raty/ranking-estelar-amarela.jpg",
                    hints: ["Ruim", "Mais ou Menos", "Bom", "Muito Bom", "Excelente"],
                    click: function (score, event) {

                        var target = $(this);
                        var contentid = target.attr('id');

                        if (contentid === "qualidadeProduto") {

                            $scope.novaAvaliacaoFornecedor.QualidadeProdutos = score;

                            if (score === 1) {
                                $scope.painelObsQualidade = true;
                            } else {
                                notificationService.displaySuccess('Qualidade do produto avaliado com sucesso!');
                                $scope.novaAvaliacaoFornecedor.ObsQualidade = '';
                                $scope.painelObsQualidade = false;
                            }

                        }

                        if (contentid === "tempoEntrega") {

                            $scope.novaAvaliacaoFornecedor.TempoEntrega = score;

                            if (score === 1) {

                                $scope.painelObsEntrega = true;
                            } else {
                                notificationService.displaySuccess('Tempo de entrega do pedido avaliado com sucesso!');
                                $scope.novaAvaliacaoFornecedor.ObsEntrega = '';
                                $scope.painelObsEntrega = false;

                            }

                        }

                        if (contentid === "atendimentoFornecedor") {

                            $scope.novaAvaliacaoFornecedor.Atendimento = score;

                            if (score === 1) {

                                $scope.painelObsAtendimento = true;
                            } else {
                                notificationService.displaySuccess('Atendimento pelo fornecedor avaliado com sucesso!');
                                $scope.novaAvaliacaoFornecedor.ObsAtendimento = '';
                                $scope.painelObsAtendimento = false;

                            }
                        }

                        if ($scope.novaAvaliacaoFornecedor.QualidadeProdutos != undefined &&
             $scope.novaAvaliacaoFornecedor.TempoEntrega != undefined && $scope.novaAvaliacaoFornecedor.Atendimento != undefined) {

                            //pega os dados quando vai pela tela de pedido aguardando entrega
                            var ped = angular.copy($scope.ped);

                            //pega os dados quando vai pela tela de aprovação de itens do pedido
                            var pedido = angular.copy($scope.ItensPedido);

                            if (ped.Itens != undefined) {

                                $scope.novaAvaliacaoFornecedor.PedidoId = ped.Itens[0].PedidoId;
                                $scope.novaAvaliacaoFornecedor.FornecedorId = ped.Itens[0].FornecedorId;
                                ped = {};
                            } else {

                                $scope.novaAvaliacaoFornecedor.PedidoId = pedido.PedidoId;
                                $scope.novaAvaliacaoFornecedor.FornecedorId = pedido.FornecedorId;
                            }
                        }
                        
                        $scope.$apply();
                    }
                });
            }
        }
    }
    
})(angular.module('common.ui'));