var app = angular.module('ECCFornecedor')
	.config(['$controllerProvider', '$compileProvider', '$filterProvider', '$provide','$httpProvider',
        function ($controllerProvider, $compileProvider, $filterProvider, $provide, $httpProvider) {
            app.controller = $controllerProvider.register;
            app.directive = $compileProvider.directive;
            app.filter = $filterProvider.register;
            app.factory = $provide.factory;
            app.service = $provide.service;
            app.constant = $provide.constant;
			app.value = $provide.value;

			//initialize get if not there
			if (!$httpProvider.defaults.headers.get) {
				$httpProvider.defaults.headers.get = {};
			}		
			// Answer edited to include suggestions from comments
			// because previous version of code introduced browser-related errors

			// extra
			$httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
			$httpProvider.defaults.headers.get['Pragma'] = 'no-cache';

        }
    ]);


app.config(function ($breadcrumbProvider) {

    $breadcrumbProvider.setOptions({
        template: '<ul class="breadcrumb"><li><i class="fa fa-home"></i><a href="#/"> Home </a></li><li ng-repeat="step in steps" ng-class="{active: $last}" ng-switch="$last || !!step.abstract"><a ng-switch-when="false" href="{{step.ncyBreadcrumbLink}}">{{step.ncyBreadcrumbLabel}}</a><span ng-switch-when="true">{{step.ncyBreadcrumbLabel}}</span></li></ul>'
    });

   

    //app.value("configApi", window.location.protocol + "//localhost:1310");
});