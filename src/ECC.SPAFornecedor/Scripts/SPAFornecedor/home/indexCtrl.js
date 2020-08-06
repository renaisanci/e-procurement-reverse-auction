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
           // genresLoadCompleted();
            apiService.get("/api/relatorio/fornCotacaoEveolucao", null,
                genresLoadCompleted,
                genresLoadFailed);
        }

        function genresLoadCompleted(response) {



            var relatCe = response.data.relatCe;
            var relatCp = response.data.relatCp;
            var relatCped = response.data.relatCped;



            

            Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'myfirstchart',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                //data: [
                //    { year: 'Jan', value: 44 },
                //    { year: 'Fev', value: 77 },
                //    { year: 'Mar', value: 1234 },
                //    { year: 'Abr', value: 66 },
                //    { year: 'Mai', value: 779 },
                //    { year: 'Jun', value: 67 },
                //    { year: 'Jul', value: 11 },
                //    { year: 'Ago', value: 23 },
                //    { year: 'Set', value: 45 },
                //    { year: 'Out', value: 54 },
                //    { year: 'Nov', value: 89 },
                //    { year: 'Dez', value: 11 },
                //],

                data: relatCe,
                // The name of the data record attribute that contains x-values.
                xkey: 'mes',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['quantidade'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Quantidade'],
                preUnits: '',

                parseTime: false,
                yLabelFormat: function (y) { return y != Math.round(y) ? '' : y; },
            });


 

            //Morris.Donut({
            //    // ID of the element in which to draw the chart.
            //    element: 'myfirstchart2',
            //    // Chart data records -- each entry in this array corresponds to a point on
            //    // the chart.
            //    //data: [
            //    //    { label: 'Fechadas', value: 20 },
            //    //    { label: 'Participou', value: 10 },
            //    //    { label: 'Não Participou', value: 5 },

            //    //],

            //    data: relatCp,

            //    // The name of the data record attribute that contains x-values.
            //    xkey: 'descricao',
            //    // A list of names of data record attributes that contain y-values.
            //    ykeys: ['qtdcotacao'],
            //    // Labels for the ykeys -- will be displayed when you hover over the
            //    // chart.
            //    labels: ['Quantidade']
            //});


            //Morris.Bar({
            //    element: 'myfirstchart3',
            //    data: [
            //        { year: 'Jan', value: 44 },
            //        { year: 'Fev', a: 77, b: 77 },
            //        { year: 'Mar', a: 1234, b: 56 },
            //        { year: 'Abr', a: 66, b: 66 },
            //        { year: 'Mai', a: 779, b: 779 },
            //        { year: 'Jun', a: 67, b: 67 },
            //        { year: 'Jul', a: 11, b: 44 },
            //        { year: 'Ago', a: 23, b: 23 },
            //        { year: 'Set', a: 45, b: 167 },
            //        { year: 'Out', a: 54, b: 67 },
            //        { year: 'Nov', a: 89, b: 11 },
            //        { year: 'Dez', a: 11, b: 11 },
            //    ],

            //    xkey: 'year',
            //    ykeys: ['a', 'b'],

            //    labels: ['Pedidos Cotação', 'Pedidos Promoção'],
            //    barColors: ["#B21516", "#1531B2"],
            //    parseTime: false,
            //    yLabelFormat: function (y) { return y != Math.round(y) ? '' : y; },
            //});




        }

        function genresLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //02------------------------------------------------------------------






        //02-------------Teste para exibir informações nos graficos---------
        function loadDataFornCotPed() {
           
            apiService.get("/api/relatorio/fornCotacaoPedido", null,
                FornCotPedLoadCompleted,
                FornCotPedLoadFailed);
        }

        function FornCotPedLoadCompleted(response) {

 
            var relatCped = response.data.listaIndcador;

 
            Morris.Bar({
                element: 'myfirstchart3',
                //data: [
                //    { year: 'Jan', a: 73,     b: 79 },
                //    { year: 'Fev', a: 77,     b: 77 },
                //    { year: 'Mar', a: 1234,   b: 56 },
                //    { year: 'Abr', a: 66,     b: 66 },
                //    { year: 'Mai', a: 779,    b: 779 },
                //    { year: 'Jun', a: 67,     b: 67 },
                //    { year: 'Jul', a: 11,     b: 44 },
                //    { year: 'Ago', a: 23,     b: 23 },
                //    { year: 'Set', a: 45,     b: 167 },
                //    { year: 'Out', a: 54,     b: 67 },
                //    { year: 'Nov', a: 89,     b: 11 },
                //    { year: 'Dez', a: 11,     b: 11 },
                //],

                data: relatCped,

                xkey: 'mesUnic',
                ykeys: ['a', 'b'],

                labels: ['Pedidos Cotação', 'Pedidos Promoção'],
                barColors: ["#B21516", "#1531B2"],
                parseTime: false,
                yLabelFormat: function (y) { return y != Math.round(y) ? '' : y; },
            });




        }

        function FornCotPedLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //02------------------------------------------------------------------
         








		// carregas os grafico temporario até pegar no banco mesmo
		function graficoFakeLoadCompleted() {

			Morris.Line({
				// ID of the element in which to draw the chart.
				element: 'myfirstchart',
				// Chart data records -- each entry in this array corresponds to a point on
				// the chart.
				data: [

					{ year: '2017', value: 180 },
					{ year: '2018', value: 155 },
					{ year: '2019', value: 171 }
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

					{ y: '2017', a: 50, b: 40 },
					{ y: '2018', a: 75, b: 65 },
					{ y: '2019', a: 100, b: 90 }
				],

				xkey: 'y',
				ykeys: ['a', 'b'],

				labels: ['Series A', 'Series B'],
				barColors: ["#B21516", "#1531B2"]
			});




		}









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
                templateUrl: 'scripts/SPAFornecedor/termoUso/aceiteTermoUso.html',
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



        loadDataFornCotPed();
		loadData();

		//graficoFakeLoadCompleted();
    }

})(angular.module('ECCFornecedor'));