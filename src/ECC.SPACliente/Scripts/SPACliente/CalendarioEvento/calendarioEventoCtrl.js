
(function (app) {
    'use strict';

    app.controller('calendarioEventoCtrl', calendarioEventoCtrl);

    calendarioEventoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'storeService', '$compile', 'uiCalendarConfig', '$filter', 'SweetAlert'];

    function calendarioEventoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, storeService, $compile, uiCalendarConfig, $filter, SweetAlert) {

        $scope.close = close;
        $scope.atualizaPedidosCalendario = atualizaPedidosCalendario;

        //=================================================
        $scope.events = [];


        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();


        /* event source that pulls from google.com */
        //$scope.eventSource = {
        //    url: "http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic",
        //    className: 'gcal-event',           // an option!
        //    currentTimezone: 'America/Chicago' // an option!
        //};
        /* event source that contains custom events on the scope */

        //$scope.events = [
        //  { title: '56 Pedidos', start: new Date(y, m, 17), allDay: false },

        //  { title: '46 Pedidos', start: new Date(y, m, 20), allDay: false },
        //  { title: '33 Pedidos', start: new Date(y, m, 21), allDay: false },
        //  { title: 'Long Event', start: new Date(y, m, d - 5), end: new Date(y, m, d - 2) },
        //  { id: 999, title: 'Repeating Event', start: new Date(y, m, d - 3, 16, 0), allDay: false },
        //  { id: 999, title: 'Repeating Event', start: new Date(y, m, d + 4, 16, 0), allDay: false },
        //  { title: 'Birthday Party', start: new Date(y, m, d + 1, 19, 0), end: new Date(y, m, d + 1, 22, 30), allDay: false },
        //  { title: 'Click for Google', start: new Date(y, m, 28), end: new Date(y, m, 29), url: 'http://google.com/' }
        //];


        /* event source that calls a function on every view switch */
        $scope.eventsF = function (start, end, timezone, callback) {
            var s = new Date(start).getTime() / 1000;
            var e = new Date(end).getTime() / 1000;
            var m = new Date(start).getMonth();
            var events = [{ title: 'Feed Me ' + m, start: s + (50000), end: s + (100000), allDay: false, className: ['customFeed'] }];
            callback(events);
            $scope.events.splice(0);
            carregaTotalPedidosPorData();
        };

        $scope.calEventsExt = {
            color: '#f00',
            textColor: 'yellow',
            events: [
                { type: 'party', title: 'Lunch', start: new Date(y, m, d, 12, 0), end: new Date(y, m, d, 14, 0), allDay: false },
                { type: 'party', title: 'Lunch 2', start: new Date(y, m, d, 12, 0), end: new Date(y, m, d, 14, 0), allDay: false },
                { type: 'party', title: 'Click for Google', start: new Date(y, m, 28), end: new Date(y, m, 29), url: 'http://google.com/' }
            ]
        };

        /* alert on dayClick */
        $scope.alertOnDayClick = function (dateClick, jsEvent, view) {

            var dateC = new Date();
            var d1 = dateC.getDate();
            var m1 = dateC.getMonth();
            var y1 = dateC.getFullYear();

            var dt1 = new Date(y1, m1, d1, 0, 0);
            var dt2 = new Date(dateClick.toDate().setHours(0, 0, 0, 0));
            var dtSelect = new Date(dateClick.toDate().setHours(dateC.getHours(), dateC.getMinutes(), dateC.getSeconds()));

            //valores retornados no callback carregaDataCotacaoFranquiaCompleted.
            var dataBanco = new Date($scope.dataCotacao);
            var horaDataServidor = new Date($scope.dataServidor);
            var horaCotacao = $scope.horaCotacao;

            var d2 = dt2.getDate() + 1;
            var m2 = dt2.getMonth();
            var y2 = dt2.getFullYear();
            var dataCotacao = new Date(y2, m2, d2, 12, 0, 0);
            var dt22 = new Date(y2, m2, d2, 0, 0);

            var estado = $scope.EnderecoPadrao.Estado;

            var config = {
                params: {
                    data: $filter('date')(dt22, 'dd/MM/yyyy'),
                    estado: estado
                }
            };

            apiService.get('/api/pedido/verificaDataPedidoFeriado', config,
                verificaDataPedidoFeriadoCompleted,
                verificaDataPedidoFeriadoFailed);

            function verificaDataPedidoFeriadoCompleted(result) {

                //TipoFeriado = 0 => Não existe feriado
                //TipoFeriado = 1 => Nacional
                //TipoFeriado = 2 => Feriado para fornecedores que atendem região do membro
                //TipoFeriado = 3 => Feriado e não existe fornecedores fora do estado do membro para dar preço.

                var resultadoFeriado = result.data.calendarioFeriadoVM;
                var tipoFeriado = result.data.TipoRetornoFeriado;

                var diaSemana = dataCotacao.getDay();

                if (dt22 < dt1) {

                    notificationService.displayInfo('Selecione uma data maior ou igual a data de hoje!');

                } else if (diaSemana === 6 || diaSemana === 0) {

                    notificationService.displayInfo('Selecione um dia útil !');

                } else if (horaDataServidor.getHours() > 12 && dataCotacao.getDate() === horaDataServidor.getDate()) {

                    notificationService.displayInfo('Já passou o horário da cotação que é ' + horaCotacao + ', selecione outra data !');

                } else if (horaDataServidor.getHours() === 12 && horaDataServidor.getMinutes() > 0 && dataCotacao.getDate() === horaDataServidor.getDate()) {

                    notificationService.displayInfo('Já passou o horário da cotação que é ' + horaCotacao + ', selecione outra data !');

                } else if (tipoFeriado === 1 || tipoFeriado === 3) {

                    notificationService.displayInfo('Selecione outra data, feriado de "' + resultadoFeriado.NomeFeriado + '" !');

                } else if (tipoFeriado === 2) {

                    notificationService.displayInfo('Selecione outra data, feriado para fornecedores que atendem sua região!');

                }
                else {
                    //Propriedade copiada da controller "shoppingCartCtrl"
                    var dataCotacaoFixa = angular.copy($scope.dataCotacaoCadastrada);

                    var cont = 0;
                    var diaHoje = dt2.getDay() + 1;
                    var semana = new Array("Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado");
                    var stringDiasSemana = "";

                    for (var i = 0; i < $scope.diasSemanaCotacao.length; i++) {
                        var tamanho = $scope.diasSemanaCotacao.length;
                        stringDiasSemana += i !== tamanho - 1 ? semana[$scope.diasSemanaCotacao[i]] + ", " : semana[$scope.diasSemanaCotacao[i]] + "";
                        if ($scope.diasSemanaCotacao[i] === diaHoje) {
                            cont++;
                        }
                    }

                    if (cont === 0 && $scope.diasSemanaCotacao.length > 0 && $scope.dataCotacaoCadastrada) {
                        notificationService.displayInfo('Data configurada: ' + stringDiasSemana + " !");
                    } else {

                        //Verificando se Franquia bloqueou as escolhas de Datas de Cotação para Franqueados.
                        if (dataCotacaoFixa === false) {
                            if (verificaDataHora(dataBanco, dtSelect)) {
                                $modalInstance.close($filter('date')(dataCotacao, 'dd/MM/yyyy HH:mm'));
                                notificationService.displaySuccess('Seu pedido entrará na cotação de: ' + $filter('date')(dataCotacao, 'dd/MM/yyyy HH:mm'));
                            }
                        } else if (verificaDataHora(dataBanco, dtSelect)) {
                            if (cont === 0) {
                                notificationService.displayInfo('Próxima data para cotação ' + $filter('date')($scope.dataCotacao, 'dd/MM/yyyy'));
                            } else {
                                $modalInstance.close($filter('date')(dataCotacao, 'dd/MM/yyyy HH:mm'));
                                notificationService.displaySuccess('Seu pedido entrará na cotação de: ' + $filter('date')(dataCotacao, 'dd/MM/yyyy HH:mm'));
                            }                               
                        }
                    }
                }
            }

            function verificaDataPedidoFeriadoFailed(result) {

                notificationService.displayError(result.data);
            }

        };

        /* alert on eventClick */
        $scope.alertOnEventClick = function (date, jsEvent, view) {
            $scope.alertMessage = (date.title + ' was clicked ');
        };
        /* alert on Drop */
        $scope.alertOnDrop = function (event, delta, revertFunc, jsEvent, ui, view) {
            $scope.alertMessage = ('Event Droped to make dayDelta ' + delta);
        };
        /* alert on Resize */
        $scope.alertOnResize = function (event, delta, revertFunc, jsEvent, ui, view) {
            $scope.alertMessage = ('Event Resized to make dayDelta ' + delta);
        };
        /* add and removes an event source of choice */
        $scope.addRemoveEventSource = function (sources, source) {
            var canAdd = 0;
            angular.forEach(sources, function (value, key) {
                if (sources[key] === source) {
                    sources.splice(key, 1);
                    canAdd = 1;
                }
            });
            if (canAdd === 0) {
                sources.push(source);
            }
        };
        /* add custom event*/
        $scope.addEvent = function () {
            $scope.events.push({
                title: 'Open Sesame',
                start: new Date(y, m, 28),
                end: new Date(y, m, 29),
                className: ['openSesame']
            });
        };
        /* remove event */
        $scope.remove = function (index) {
            $scope.events.splice(index, 1);
        };
        /* Change View */
        $scope.changeView = function (view, calendar) {
            uiCalendarConfig.calendars[calendar].fullCalendar('changeView', view);
        };
        /* Change View */
        $scope.renderCalender = function (calendar) {
            if (uiCalendarConfig.calendars[calendar]) {
                uiCalendarConfig.calendars[calendar].fullCalendar('render');
            }
        };
        /* Render Tooltip */
        $scope.eventRender = function (event, element, view) {
            element.attr({
                'tooltip': event.title,
                'tooltip-append-to-body': true
            });
            $compile(element)($scope);
        };
        /* config object */
        $scope.uiConfig = {
            calendar: {
                height: 450,
                editable: true,
                header: {
                    left: 'title',
                    center: '',
                    right: 'today prev,next'
                },

                dayClick: $scope.alertOnDayClick,
                eventClick: $scope.alertOnEventClick,
                eventDrop: $scope.alertOnDrop,
                eventResize: $scope.alertOnResize,
                eventRender: $scope.eventRender
            }
        };

        $scope.uiConfig.calendar.dayNames = ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sabado"];
        $scope.uiConfig.calendar.dayNamesShort = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"];

        //$scope.changeLang = function () {
        //    if ($scope.changeTo === 'Hungarian') {
        //        $scope.uiConfig.calendar.dayNames = ["Vasárnap", "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek", "Szombat"];
        //        $scope.uiConfig.calendar.dayNamesShort = ["Vas", "Hét", "Kedd", "Sze", "Csüt", "Pén", "Szo"];
        //        $scope.changeTo = 'English';
        //    } else {
        //        $scope.uiConfig.calendar.dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        //        $scope.uiConfig.calendar.dayNamesShort = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        //        $scope.changeTo = 'Hungarian';
        //    }
        //};
        /* event sources array*/
        $scope.eventSources = [$scope.events, $scope.eventSource, $scope.eventsF];
        $scope.eventSources2 = [$scope.calEventsExt, $scope.eventsF, $scope.events];

        //==================================================

        //1---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------

        //02---------------Valida Data e Hora-----------------------
        function verificaDataHora(dataServidor, dataSelecionada) {

            var premissa = true;

            var horaCotacao = $filter('date')(dataServidor, 'HH:mm');
            var dataHoje = new Date();
            var d = dataHoje.getDate();
            var m = dataHoje.getMonth() + 1;
            var y = dataHoje.getFullYear();

            var dc = dataSelecionada.getDate() + 1;
            var mc = dataSelecionada.getMonth() + 1;
            var yc = dataSelecionada.getFullYear();

            if (dc === d && mc === m && yc === y) {

                if (dataSelecionada.getHours() > dataServidor.getHours()) {
                    notificationService.displayInfo('Selecione outra data, pois já passou o horário de cotação que é ' + horaCotacao);
                    premissa = false;
                }

                if (dataServidor.getHours() === dataSelecionada.getHours() && dataSelecionada.getMinutes() > dataServidor.getMinutes()) {
                    notificationService.displayInfo('Selecione outra data, pois já passou o horário de cotação que é ' + horaCotacao);
                    premissa = false;
                }

            }

            return premissa;
        }
        //02------------------Valida Data e Hora--------------------

        //03-----------Carrega Total Pedidos por Data------------------
        function carregaTotalPedidosPorData() {
            apiService.get('/api/pedido/totalPedidosPorData', null,
                loadcarregaQtdPedidosPorDataCompleted,
                loadcarregaQtdPedidosPorDataFailed);
        }

        function loadcarregaQtdPedidosPorDataCompleted(result) {

            var totalPedidosPorData = result.data.listaQtdPedData;

            for (var i = 0; i < totalPedidosPorData.length; i++) {

                var data = totalPedidosPorData[i].data.split('/');
                var d = parseInt(data[0]);
                var m = parseInt(data[1]) - 1;
                var y = parseInt(data[2]);

                var objetoPedidosData = {
                    title: totalPedidosPorData[i].qtd > 1 ? totalPedidosPorData[i].qtd + ' Pedidos' : totalPedidosPorData[i].qtd + ' Pedido',
                    start: new Date(y, m, d),
                    allDay: false
                }

                $scope.events.push(objetoPedidosData);
            }

        }

        function loadcarregaQtdPedidosPorDataFailed(result) {

            notificationService.displayError(result.data);
        }
        //03-----------------------------------------------------------


        function atualizaPedidosCalendario() {
            $scope.events.splice(0);
            carregaTotalPedidosPorData();

        }

        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };




    }

})(angular.module('ECCCliente'));