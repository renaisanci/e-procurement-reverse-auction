(function (app) {
    'use strict';

    app.factory('apiService', apiService);

	apiService.$inject = ['$http', '$location', 'notificationService', '$rootScope', '$cookieStore'];

	function apiService($http, $location, notificationService, $rootScope, $cookieStore) {

        var service = {
            get: get,
            post: post,
            identificaAmbiente: identificaAmbiente
        };

        function get(url, config, success, failure) {

            return $http.get(identificaAmbiente() + url, config)
                .then(function (result) {
                    success(result);
                }, function (error) {
                    if (error.status == 401) {
                        notificationService.displayError('Autenticação requerida.');
                        $rootScope.previousState = $location.path();
                        $location.path('/login');
                    }
                    else if (failure != null) {
                        if (error.status == '0') {
							notificationService.displayError('Ops! Nosso Servidor se comportou mal.');							 
							 $rootScope.$emit("logoutExternoError");
                        }
                        failure(error);
                    }
                });
        }

        function post(url, data, success, failure) {

            return $http.post(identificaAmbiente() + url, data)
                .then(function (result) {
                    success(result);
                }, function (error) {
                    console.error(error);
                    if (error.status === 401) {
                        notificationService.displayError('Favor efetuar o login.');
                        $rootScope.previousState = $location.path();
                        $location.path('/login');
                    }
                    else if (failure != null) {
                        if (error.status === 0) {
							notificationService.displayError('Ops! Nosso Servidor se comportou mal.');
							$rootScope.$emit("logoutExternoError");
						 
                        }
                        failure(error);
                    }
                });
        }

        function identificaAmbiente() {
            if ($location.absUrl().indexOf("localhost") > 0) {

                return window.location.protocol + "//localhost:1310";

            } else if ($location.absUrl().indexOf("/dev") > 0) {

                return window.location.protocol + "//devadm.economizaja.com.br";

            } else if ($location.absUrl().indexOf("/hom") > 0) {

                return window.location.protocol + "//homadm.economizaja.com.br";

            } else {

                return  "https://adm.economizaja.com.br";

            }
		}


		function logoutError( ) {

			$cookieStore.remove('repositoryCliRefresh');
			localStorage.removeItem('repositoryCli');
			$http.defaults.headers.common.Authorization = '';
		}

 
        
        return service;
    }

})(angular.module('common.core'));