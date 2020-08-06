
(function (app) {
    'use strict';

    app.controller('indicadoresCtrl', indicadoresCtrl);

    indicadoresCtrl.$inject = ['$scope', 'apiService', 'notificationService', 'SweetAlert', '$modal'];

    function indicadoresCtrl($scope, apiService, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-indicadores';

        
        //02-------------Teste para exibir informações nos graficos---------
        function loadData() {


            apiService.get("/api/membro/membroTeste", null,
                genresLoadCompleted,
                genresLoadFailed);
        }

        function genresLoadFailed(response) {
            notificationService.displayError(response.data);
            
        }

        function genresLoadCompleted(result) {
            var genres = result.data;
            Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'myfirstchart',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: [
                  { year: 'Jan', value: 4400 },
                  { year: 'Fev', value: 7000 },
                  { year: 'Mar', value: 14000 },
                  { year: 'Abr', value: 13000 },
                  { year: 'Mai', value: 500 },
                  { year: 'Jun', value: 2000 },
                  { year: 'Jul', value: 19000 },
                  { year: 'Ago', value: 3000 },
                  { year: 'Set', value: 300 },
                  { year: 'Out', value: 13000 },
                  { year: 'Nov', value: 2330 },
                  { year: 'Dez', value: 5000 },
                  
                ],
                // The name of the data record attribute that contains x-values.
                xkey: 'year',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['value'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Gastou'],

                preUnits: 'R$',

                lineWidth:10,

                parseTime: false
            });

            Morris.Donut({
                // ID of the element in which to draw the chart.
                element: 'myfirstchart2',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: [
                  { label: '2014', value: 20 },
                  { label: '2015', value: 10 },
                  { label: '2016', value: 30 },
                  { label: '2017', value: 25 },
                  { label: '2018', value: 50 }
                ],
                // The name of the data record attribute that contains x-values.
                xkey: 'year',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['value'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Value'],

               

               
            });


            Morris.Bar({
                element: 'myfirstchart3',
                data: [
                  { y: 'Jan', a: 15},
                  { y: 'Fev', a: 30 },
                  { y: 'Mar', a: 50},
                  { y: 'Abr', a: 40 },
                  { y: 'Mai', a: 25 },
                  { y: 'Jun', a: 20 },
                  { y: 'Jul', a: 15 },
                  { y: 'Ago', a: 8 },
                  { y: 'Set', a: 5 },
                  { y: 'Out', a: 12 },
                  { y: 'Nov', a: 17 },
                  { y: 'Dez', a: 50 },
                ],

                xkey: 'y',

                ykeys: ['a'],

                labels: ['Percentual'],

                barColors: ["#B21516"],

                postUnits: '%'
            });




        }
        //02------------------------------------------------------------------
    

        loadData();


    }

})(angular.module('ECCCliente'));