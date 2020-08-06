(function (app) {
    'use strict';

    app.directive('componentRatingFornecedor', componentRatingFornecedor);

    app.$inject = ['$scope', 'apiService', '$sce'];

    function componentRatingFornecedor(apiService) {
        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs) {


                $element.raty({
                    score: $attrs.componentRatingFornecedor,
                    halfShow: false,
                    readOnly: true,
                    noRatedMsg: "Não Avaliado",
                    starHalf: "../Content/images/raty/star-half.png",
                    starOff: "../Content/images/raty/star-off.png",
                    starOn: "../Content/images/raty/star-on.png",
                    hints: ["Ruim", "Mais ou Menos", "Bom", "Muito Bom", "Excelente"],
                    click: function (score, event) {
                        //  //Set the model value
                        //  $scope.produtoPesq.Ranking = score;


                        //  apiService.post('/api/produto/inserirNota', $scope.produtoPesq,
                        //sucesso,
                        //falha);

                        $scope.$apply();
                    }
                });
            }
        }
    }

})(angular.module('common.ui'));