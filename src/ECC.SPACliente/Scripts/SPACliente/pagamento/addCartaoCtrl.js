(function (app) {
    'use strict';

    app.controller('addCartaoCtrl', addCartaoCtrl);

    addCartaoCtrl.$inject = ['$scope', '$sce', '$location', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$rootScope'];

    function addCartaoCtrl($scope, $sce, $location, $modalInstance, $timeout, apiService, notificationService, $rootScope) {

        $scope.close = close;
        $scope.salvarFormaPagamento = salvarFormaPagamento;
        $scope.bandeirasCartoes = [];
        var defaultFormat = /(\d{1,4})/g;

        $scope.bandeirasCartoes = [
            {
                id: 4,
                type: 'amex',
                pattern: /^3[47]/,
                format: /(\d{1,4})(\d{1,6})?(\d{1,5})?/,
                length: [15],
                cvcLength: [4],
                luhn: true
            }, {
                id: 0,
                type: 'dankort',
                pattern: /^5019/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 3,
                type: 'dinersclub',
                pattern: /^(36|38|30[0-5])/,
                format: /(\d{1,4})(\d{1,6})?(\d{1,4})?/,
                length: [14],
                cvcLength: [3],
                luhn: true
            }, {
                id: 0,
                type: 'discover',
                pattern: /^(6011|65|64[4-9]|622)/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 0,
                type: 'jcb',
                pattern: /^35/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 0,
                type: 'laser',
                pattern: /^(6706|6771|6709)/,
                format: defaultFormat,
                length: [16, 17, 18, 19],
                cvcLength: [3],
                luhn: true
            }, {
                id: 0,
                type: 'maestro',
                pattern: /^(5018|5020|5038|6304|6703|6708|6759|676[1-3])/,
                format: defaultFormat,
                length: [12, 13, 14, 15, 16, 17, 18, 19],
                cvcLength: [3],
                luhn: true
            }, {
                id: 2,
                type: 'mastercard',
                pattern: /^(5[1-5]|677189)|^(222[1-9]|2[3-6]\d{2}|27[0-1]\d|2720)/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 0,
                type: 'unionpay',
                pattern: /^62/,
                format: defaultFormat,
                length: [16, 17, 18, 19],
                cvcLength: [3],
                luhn: false
            }, {
                id: 1,
                type: 'visaelectron',
                pattern: /^4(026|17500|405|508|844|91[37])/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 6,
                type: 'elo',
                pattern: /^(4011|438935|45(1416|76|7393)|50(4175|6699|67|90[4-7])|63(6297|6368))/,
                format: defaultFormat,
                length: [16],
                cvcLength: [3],
                luhn: true
            }, {
                id: 1,
                type: 'visa',
                pattern: /^4/,
                format: defaultFormat,
                length: [13, 16, 19],
                cvcLength: [3],
                luhn: true
            }, {
                id: 7,
                type: 'hipercard',
                pattern: /^(38|60)\d{11}(?:\d{3})?(?:\d{3})?$/,
                format: defaultFormat,
                //length: [13, 16, 19],
                //cvcLength: [3],
                luhn: true
            }
        ];

        //#region [1 - Salvar formas de pagamentos]

        function salvarFormaPagamento() {


            if ($scope.formaPagamento.Numero !== undefined && $scope.formaPagamento.Numero !== "") {
                var card;
                var num = ($scope.formaPagamento.Numero + '').replace(/\D/g, '');

                for (var i = 0; i < $scope.bandeirasCartoes.length; i++) {
                    card = $scope.bandeirasCartoes[i];
                    if (card.pattern.test(num)) {
                        $scope.formaPagamento.CartaoBandeiraId = card.id;
                        continue;
                    }
                }

            }


            if ($scope.formaPagamento.Numero == undefined) {

                notificationService.displayInfo("Número do cartão é inválido!");
                return;

            } else if ($scope.formaPagamento.Cvc == undefined || $scope.formaPagamento.Cvc.length < 3) {

                notificationService.displayInfo("Número de segurança inválido!");
                return;

            } else if ($scope.formaPagamento.Nome == undefined || $scope.formaPagamento.Nome == "") {

                notificationService.displayInfo("Nome do cartão inválido!");
                return;

            } else if (($scope.formaPagamento.DataVencimento == undefined || $scope.formaPagamento.DataVencimento == "") ||
                        $scope.formaPagamento.DataVencimento.trim().replace(" ", "").length < 7) {

                notificationService.displayInfo("Data de validade inválida!");
                return;

            } else {

                apiService.post('/api/cartaocredito/inserirAlterarCartaoCredito', $scope.formaPagamento,
                    atualizarFormaPagamentoSucesso,
                    atualizarFormaPagamentoFalha);
            }
        }

        function atualizarFormaPagamentoSucesso(response) {

            notificationService.displaySuccess("Cartão salvo com sucesso!");
            $modalInstance.dismiss();

            if ($rootScope.repository.loggedUser.faturaMensalidade)
                $location.path("/perfil/planos");
            else
                $rootScope.$emit("reloadCartoesCredito", {});
        }

        function atualizarFormaPagamentoFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //#endregion [1 - Fim salvar formas de pagamentos]

        //#region [2 - Carrega Bandeiras Cartões]

        function loadCartoesBandeiras(param) {

            var cartaoId = param === undefined ? 0 : param.Id;

            apiService.get('/api/cartaocredito/getCartaoBandeira', null,
                cartoesBandeirasCompleted,
                cartoesBandeirasFailed);
        }

        function cartoesBandeirasCompleted(response) {

            $scope.bandeirasCartoes = response.data;


        }

        function cartoesBandeirasFailed(response) {

            notificationService.displayError(response.data);

        }

        //#endregion 2 - Fim Carrega Bandeiras Cartões
        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //---------Fecha Modal---------------------

        //loadCartoesBandeiras();
    }

})(angular.module('ECCCliente'));