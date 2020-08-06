
(function (app) {
    'use strict';

    app.controller('perfilCtrl', perfilCtrl);

    perfilCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', '$rootScope', 'membershipService', 'SweetAlert', '$location'];

    function perfilCtrl($scope, $timeout, apiService, notificationService, $rootScope, membershipService, SweetAlert, $location) {

        $scope.novoUsuario = {};
        $scope.atualizarModelUsuario = atualizarModelUsuario;
        $scope.cancelarAssinatura = cancelarAssinatura;

        //1-----Retorna Dados-------------------------------
        function carregarUsuario() {
            apiService.get('/api/usuario/perfil', $scope.novoUsuario,
                carregarUsuarioSucesso,
                carregarUsuarioFalha);

        }

        function carregarUsuarioSucesso(response) {
            $scope.novoUsuario = response.data;

            $scope.emailAnt = response.data.UsuarioEmail;
        }

        function carregarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //1 ----------------------------Fim---------------------------------------//


        //2-----Atualiza Usuario-------------------------------
        function atualizarModelUsuario() {

            if ($scope.emailAnt != $scope.novoUsuario.UsuarioEmail)
                membershipService.logout();

            apiService.post('/api/usuario/atualizar', $scope.novoUsuario,
                atualizarUsuarioSucesso,
                atualizarUsuarioFalha);

        }

        function atualizarUsuarioSucesso(response) {
            notificationService.displaySuccess($scope.novoUsuario.UsuarioNome + ' Atualizado com Sucesso.');
            $scope.novoUsuario = response.data;

            if ($scope.emailAnt != response.data.UsuarioEmail)
                $rootScope.$emit("logoutExterno");

        }

        function atualizarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //2 ----------------------------Fim---------------------------------------//

        //#region[ 3 Cancelar Assinatura ]

        function cancelarAssinatura() {

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Tem certeza que deseja cancelar sua assinatura?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "SIM",
                cancelButtonText: "NÃO",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        apiService.post('/api/usuario/cancelarAssinatura', null,
                            cancelarAssinaturaSucesso,
                            cancelarAssinaturaFalha);
                    }
                });
        }

        function cancelarAssinaturaSucesso(response) {

            if (response.status === 204) {

                SweetAlert.swal({
                    title: "Atenção!",
                    text: "Já existe uma solicitação de cancelamento de assinatura!",
                    type: "warning",
                    confirmButtonText: "OK"
                });

            } else {

                SweetAlert.swal({
                    title: "ATENÇÃO",
                    text: response.data,
                    type: "warning",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK",
                    closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {

                        if (response.status === 201) {
                            membershipService.logout(function () {
                                $location.path("/login");
                            });
                        }
                    }
                });
            }
        }

        function cancelarAssinaturaFalha(response) {

            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }


        //#endregion[ 3 Fim Cancelar Assinatura ]

        carregarUsuario();
    }

})(angular.module('ECCCliente'));