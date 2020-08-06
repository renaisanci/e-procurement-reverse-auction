(function (app) {
    //'use strict';

    app.controller('confirmaCartaoCtrl', confirmaCartaoCtrl);

    confirmaCartaoCtrl.$inject = ['$scope', 'membershipService', '$sce', '$location', '$modalInstance', 'apiService', 'notificationService', '$rootScope', 'SweetAlert'];

    function confirmaCartaoCtrl($scope, membershipService, $sce, $location, $modalInstance, apiService, notificationService, $rootScope, SweetAlert) {

        $scope.close = close;
        $scope.salvarFormaPagamento = salvarFormaPagamento;
        $scope.bandeirasCartoes = [];
        $scope.exibirLoadCadastroCartao = false;
        var descBandeira = '';
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

        //---------Salvar---------------------

        function salvarFormaPagamento() {


            if ($scope.formaPagamento.Numero !== undefined && $scope.formaPagamento.Numero !== "") {
                var card;
                var num = ($scope.formaPagamento.Numero + '').replace(/\D/g, '');

                for (var i = 0; i < $scope.bandeirasCartoes.length; i++) {
                    card = $scope.bandeirasCartoes[i];
                    if (card.pattern.test(num)) {
                        $scope.formaPagamento.CartaoBandeiraId = card.id;
                        descBandeira = card.type;
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

                $scope.exibirLoadCadastroCartao = true;
                gravaPlanoPagCartaoCredito();
            }
        }

        function gravaPlanoPagCartaoCredito() {

            //#region [ Desenvolvimento ]
            //var s = document.createElement('script');
            //s.type = 'text/javascript';
            //var v = parseInt(Math.random() * 1000000);
            //console.log(v);
            //s.src = 'https://sandbox.gerencianet.com.br/v1/cdn/2498b7a8f0d593b9659acd3e12369878/' + v;
            //s.async = false;
            //s.id = '2498b7a8f0d593b9659acd3e12369878';
            //if (!document.getElementById('2498b7a8f0d593b9659acd3e12369878')) {
            //    document.getElementsByTagName('head')[0].appendChild(s);
            //}

            //$gn = {
            //    validForm: true,
            //    processed: false,
            //    done: {},
            //    ready: function (fn) {
            //        $gn.done = fn;
            //    }
            //};
            //#endregion

            //#region [ Produção ]
            var s = document.createElement('script');
            s.type = 'text/javascript';
            var v = parseInt(Math.random() * 1000000);
            s.src = 'https://api.gerencianet.com.br/v1/cdn/2498b7a8f0d593b9659acd3e12369878/' + v;
            s.async = false;
            s.id = '2498b7a8f0d593b9659acd3e12369878';
            if (!document.getElementById('2498b7a8f0d593b9659acd3e12369878')) {
                document.getElementsByTagName('head')[0].appendChild(s);
            }
            $gn = {
                validForm: true,
                processed: false,
                done: {},
                ready: function (fn) {
                    $gn.done = fn;
                }
            };
            //#endregion

            var mes = $scope.formaPagamento.DataVencimento.split('/')[0].trim();
            var ano = $scope.formaPagamento.DataVencimento.split('/')[1].trim();

            $gn.ready(function (checkout) {

                checkout.getPaymentToken({

                    brand: descBandeira, // bandeira do cartão
                    number: $scope.formaPagamento.Numero, // número do cartão
                    cvv: $scope.formaPagamento.Cvc, // código de segurança
                    expiration_month: mes, // mês de vencimento
                    expiration_year: ano // ano de vencimento

                }, function (error, response) {
                    if (error) {

                        // Trata o erro ocorrido
                        notificationService.displayError(error);
                        console.error(error);

                    } else {

                        // Trata a resposta
                        $gn.processed = true;

                        console.log(response);

                        $scope.formaPagamento.TokenCartaoGerenciaNet = response.data.payment_token;

                        $scope.exibirLoadCadastroCartao = false;

                        apiService.post('/api/pagamentos/atualizarPlano/' +
                            $scope.planoPagamento.IdPano +
                            '/' +
                            $scope.planoPagamento.TipoPagamentoId +
                            '/' +
                            $scope.trocarPlanoMembro,
                            $scope.formaPagamento,
                            inserirPlanoMensalidadeSucesso,
                            inserirPlanoMensalidadeFalha);
                    }
                });

            });
        }

        function inserirPlanoMensalidadeSucesso(response) {

            $modalInstance.dismiss();

            if ($scope.trocarPlanoMembro) {

                SweetAlert.swal({
                    title: "PLANO TROCADO!",
                    text: "Este plano ficará ativo em " + response.data.dataTrocaPlano,
                    type: "success",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK",
                    closeOnConfirm: true

                });

                $scope.trocarPlanoMembro = false;

            } else {

                SweetAlert.swal({
                    title: "PLANO CONFIRMADO!",
                    text: "Assim que seu pagamento for confirmado, você poderá fazer seus pedidos e começar a economizar!\n"
                        + "Será necessário fazer o Login novamente.\n "
                        + "Seja Bem-Vindo a EconomizaJá",
                    type: "success",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK",
                    closeOnConfirm: true
                }, function (isConfirm) {

                    if (isConfirm) {
                        membershipService.logout(function () {
                            $location.path("/login");
                            $scope.userData.displayUserInfo();
                        });
                    }

                });
            }

            reloadPlanos();
        }

        function inserirPlanoMensalidadeFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }


        function reloadPlanos() {

            $rootScope.$emit("reloadPlanos", {});

        }

        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //---------Fecha Modal---------------------

    }

})(angular.module('ECCCliente'));