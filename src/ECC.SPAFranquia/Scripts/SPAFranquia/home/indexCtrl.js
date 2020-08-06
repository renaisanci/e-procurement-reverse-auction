(function (app) {
    'use strict';

    app.controller('indexCtrl', indexCtrl);

    indexCtrl.$inject = ['$scope', 'apiService', 'notificationService', '$modal'];

    function indexCtrl($scope, apiService, notificationService, $modal) {
        $scope.pageClass = 'page-home';
        $scope.isReadOnly = true;
        $scope.alterarSenhaLogado = alterarSenhaLogado;
        $scope.loadData = loadData;
    
        function alterarSenhaLogado() { }

        //function loadData() {
        //    apiService.get('/api/painel/fornecedor', null,
        //                indexLoadCompleted,
        //                indexLoadFailed);

     
        //}

        //function indexLoadCompleted(result) {

        
        //}

        //function indexLoadFailed(response) {
        //    //console.log(response);
        //    if (response.status == '400')
        //        for (var i = 0; i < response.data.length; i++) {
        //            notificationService.displayInfo(response.data[i]);
        //        }
        //    else
        //        notificationService.displayError(response.statusText);
        //}

        //02-------------Teste para exibir informações nos graficos---------
        function loadData() {
            genresLoadCompleted();

            //apiService.get("/api/fornecedor/forTeste", null,
            //    genresLoadCompleted,
            //    genresLoadFailed);
        }


        function genresLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function genresLoadCompleted( ) {
           
            Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'myfirstchart',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: [
                  { year: '2008', value: 20 },
                  { year: '2009', value: 10 },
                  { year: '2010', value: 5 },
                  { year: '2011', value: 5 },
                  { year: '2012', value: 20 }
                ],
                // The name of the data record attribute that contains x-values.
                xkey: 'year',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['value'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Value']
            });

            Morris.Donut({
                // ID of the element in which to draw the chart.
                element: 'myfirstchart2',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: [
                  { label: 'Fechadas', value: 20 },
                  { label: 'Participou', value: 10 },
                  { label: 'Não Participou', value: 5 },
            
                ],
                // The name of the data record attribute that contains x-values.
                xkey: 'year',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['value'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Value']
            });


            Morris.Bar({
                element: 'myfirstchart3',
                data: [
                  { y: '2006', a: 100, b: 90 },
                  { y: '2007', a: 75, b: 65 },
                  { y: '2008', a: 50, b: 40 },
                  { y: '2009', a: 75, b: 65 },
                  { y: '2010', a: 50, b: 40 },
                  { y: '2011', a: 75, b: 65 },
                  { y: '2012', a: 100, b: 90 }
                ],

                xkey: 'y',
                ykeys: ['a', 'b'],

                labels: ['Series A', 'Series B'],
                barColors: ["#B21516", "#1531B2"]
            });




        }
        //02------------------------------------------------------------------

        //-------------Aceite do Termo de Uso---------

        function aceiteTermoUso() {
            apiService.get("/api/usuario/verificaTermoUso", null,
                aceiteTermoUsoCompleted,
                aceiteTermoUsoFailed);
        }

        function aceiteTermoUsoCompleted(response) {
            //console.log(response.data);

            if (!response.data.ExibirAceite) return;

            $scope.TermoUso = response.data.TermoUso;

            $modal.open({
                backdrop: 'static',
                keyboard: false,
                templateUrl: 'scripts/SPAFranquia/termoUso/aceiteTermoUso.html',
                controller: 'aceiteTermoUsoCtrl',
                scope: $scope,
                size: 'lg-ecc'
            }).result.then(function ($scope) {
            }, function () {
            });
        }

        function aceiteTermoUsoFailed(response) {
        }

        aceiteTermoUso();

        //-------------Aceite do Termo de Uso---------



       
        loadData();
    }

})(angular.module('ECCFranquia'));