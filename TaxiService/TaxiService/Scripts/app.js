var app = angular.module('TaxiService', ['ngRoute']);

app.config(function ($locationProvider) {
    $locationProvider.html5Mode(true);
});

app.config(function ($routeProvider) {
    $routeProvider

        .when('/', {
            templateUrl: 'Home/HomeTemplate',
            controller: 'HomeController'
        })

        .otherwise({ redirectTo: '/' });
});

app.controller('HomeController', function ($scope) {
    $scope.message = 'Hello from HomeController';
});

app.controller('RegFormController', function ($scope, $http) {
    $scope.formData = {};

    $scope.processForm = function () {
        $scope.errorName = "";
        if ($scope.formData.password !== $scope.formData.passwordConfirm) {
            $scope.errorName = "Lozinke se ne poklapaju";
        }
        else {
            delete $scope.formData.passwordConfirm;
        }

        if ($scope.errorName === "") {
            $http({
                method: 'POST',
                url: 'api/Taxi/',
                data: $.param($scope.formData),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            })
                .success(function (data) {
                    console.log(data);

                    if (!data.success) {
                        $scope.errorName = data.errors.name;
                    } else {
                        $scope.message = data.message;
                    }
                });
        }
    };
});