(function (app) {
    'use strict';

    app.factory('fileUploadService', fileUploadService);

    fileUploadService.$inject = ['$rootScope', '$http', '$timeout', '$upload', 'notificationService'];

    function fileUploadService($rootScope, $http, $timeout, $upload, notificationService) {

        $rootScope.upload = [];


        var service = {
            uploadImage: uploadImage
        }

        function uploadImage($files, produtoId) {
            //$files: an array of files selected
            for (var i = 0; i < $files.length; i++) {
                var $file = $files[i];
                (function (index) {
                    $rootScope.upload[index] = $upload.upload({

                        url: "/api/produto/images/upload?produtoId=" + produtoId, // webapi url

                        method: "POST",

                        file: $file

                    }).progress(function (evt) {

                    }).success(function (data, status, headers, config) {

                        notificationService.displaySuccess(data.CaminhoImagem + ' inserida com sucesso !');

                        $scope.novaImagem = data;


                    }).error(function (data, status, headers, config) {


                        notificationService.displayError(data.Message);

                    });
                })(i);
            }
        }

        return service;
    }

})(angular.module('common.core'));