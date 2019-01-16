var app = angular.module('TaxiService', ['ngRoute']);

app.config(function ($locationProvider) {
    $locationProvider.html5Mode(true);
});

app.config(function ($routeProvider) {
    $routeProvider

        .when('/', {
            templateUrl: 'Home/HomeTemplate',
        })

        .when('/Kontakt', {
             templateUrl: 'Kontakt/KontaktTemplate',
        })

        .when('/Musterija', {
            templateUrl: 'Home/MusterijaTemplate',
        })

        .when('/Dispecer', {
            templateUrl: 'Home/DispecerTemplate',
        })

        .when('/Vozac', {
            templateUrl: 'Home/VozacTemplate',
        })

        .otherwise({ redirectTo: '/' });
});

app.controller('RegFormController', function ($scope, $http) {
    $scope.formData = {};

    $scope.processForm = function () {
        $scope.errorName = "";
        if ($scope.formData.jmbg.length !== 13 || isNaN($scope.formData.jmbg)) {
            $scope.errorName += "\nJMBG nije pravilan";
        }
        if ($scope.formData.password !== $scope.formData.passwordConfirm) {
            $scope.errorName += "\nLozinke se ne poklapaju";
        }

        if ($scope.errorName === "") {
            delete $scope.formData.passwordConfirm;
            $scope.formData.idvoznje = [];
            $scope.formData.uloga = "Musterija";

            $http({
                method: 'POST',
                url: 'api/Taxi/',
                data: $.param($scope.formData),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function successCallback(response) {
                $scope.errorName = "Registracija uspešna!";
            }, function errorCallback(response) {
                if (response.status === 409) {
                    $scope.errorName = "Vec postoji korisnik sa tim korisnickim imenom";
                }
                else {
                    $scope.errorName = "Neuspesna registracija, greska " + status;
                }
            });
        }
    };
});

app.controller('LoginFormController', function ($scope, $http) {
    $scope.loginData = {};

    $scope.login = function () {
        $http({
            method: 'POST',
            url: 'api/Login/',
            data: $.param($scope.loginData),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function successCallback(response) {
            $scope.errorName = "Login uspešan!";
            location.reload(true);
        }, function errorCallback(response) {
            if (response.status === 401) {
                $scope.errorName = "Nepravilni podaci, probajte ponovo";
            }
            else if (response.status === 403) {
                $scope.errorName = "Blokirani ste, i ne mozete korisiti ovaj web sajt.";
            }
            else {
                $scope.errorName = "Neuspesna prijava, greska " + status;
            }
        });
    };
});