(function (app) {
    'use strict';

    app.controller('indexCtrl', indexCtrl);

    indexCtrl.$inject = ['$scope', 'apiService', 'notificationService', 'Hub', '$rootScope'];

    function indexCtrl($scope, apiService, notificationService, Hub, $rootScope) {
        $scope.pageClass = 'page-home';
        $scope.sms = { Creditos: 0, Validade: new Date(Date.now()) };



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
                    { label: '2008', value: 20 },
                    { label: '2009', value: 10 },
                    { label: '2010', value: 5 },
                    { label: '2011', value: 5 },
                    { label: '2012', value: 20 }
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

        function relatorioSMS() {
            apiService.get("/api/relatorio/sms", null,
                function (response) {
                    $scope.sms = response.data;
                }, function (response) { });


          
        }


        loadData();
        relatorioSMS();
    }

})(angular.module('ECCAdm'));