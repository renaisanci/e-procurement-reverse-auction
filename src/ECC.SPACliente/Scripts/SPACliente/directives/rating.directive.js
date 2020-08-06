(function (app) {
    'use strict';

    app.directive('componentRating', componentRating);



    app.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce'];

    function componentRating(apiService) {
        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs) {

                //$scope.produtoPesq.Ranking = 0;

                $element.raty({
                    score: $attrs.componentRating,
                    halfShow: false,
                    readOnly: $scope.isReadOnly,
                    noRatedMsg: "Não Avaliado",
                    starHalf: "../Content/images/raty/star-half.png",
                    starOff: "../Content/images/raty/star-off.png",
                    starOn: "../Content/images/raty/star-on.png",
                    hints: ["Ruim", "Mais ou Menos", "Bom", "Muito Bom", "Excelente"],
                    click: function (score, event) {
                        //Set the model value
                        $scope.produtoPesq.Ranking = score;
                        apiService.post('/api/produto/inserirNota', $scope.produtoPesq,
                      sucesso,
                      falha);

                        $scope.$apply();
                    }
                });
            }
        }
    }


    function sucesso(response) {


    }


    function falha(response, notificationService) {
        notificationService.displayError(response.data);
    }

})(angular.module('common.ui'));