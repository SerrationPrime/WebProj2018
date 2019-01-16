//ovaj api kljuc ne podrzava rad sa geokodiranjem, pa je potreban drugaciji pristup
var latitude;
var longitude;
var markersArray = [];
var map;


function initMap() {
    var latlng = new google.maps.LatLng(45.255097, 19.844539);
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 15,
        center: latlng
    });
    var geocoder = new google.maps.Geocoder();

    google.maps.event.addListener(map, "click", function (event) {
        // place a marker
        placeMarker(event.latLng);

        // display the lat/lng in your form's lat/lng fields
        latitude = event.latLng.lat();
        longitude = event.latLng.lng();
    });

}

function placeMarker(location) {
    // first remove all markers if there are any
    deleteOverlays();

    var marker = new google.maps.Marker({
        position: location,
        map: map
    });

    // add marker in markers array
    markersArray.push(marker);

    map.setCenter(location);
}

function deleteOverlays() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
        markersArray.length = 0;
    }
}

var app = angular.module('TaxiService');

app.controller('DispController', function ($scope) {
    $scope.vozacKreacija = false;
    $scope.voznjaKreacija = false;
    $scope.neobradjeneVoznje = false;
});

app.controller('VozFormController', function ($scope, $http) {
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
                url: 'api/Vozac/',
                data: $.param($scope.formData),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function successCallback(response) {
                $scope.errorName = "Registracija vozaca uspešna!";
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

app.controller('PutFormController', function ($scope, $http) {
    $scope.formData = {};
    $scope.listaVozacaVidljiva = false;
    $scope.lista = [];
    $scope.formData.lokacija = {};
    $scope.formData.odrediste = {};
    $scope.formData.lokacija.latitude = 360;
    $scope.formData.lokacija.longitude = 360;
    $scope.formData.lokacija.adresa = "";
    $scope.formData.odrediste.latitude = 360;
    $scope.formData.odrediste.longitude = 360;
    $scope.formData.odrediste.adresa = "";

    $scope.zeljenaLokacija = function () {
        $scope.formData.odrediste.latitude = latitude;
        $scope.formData.odrediste.longitude = longitude;
        $scope.formData.odrediste.adresa = $scope.boxLoc;
    }

    $scope.traziVozace = function () {
        $scope.formData.lokacija.latitude = latitude;
        $scope.formData.lokacija.longitude = longitude;
        $scope.formData.lokacija.adresa = $scope.boxLoc;

        $http({
            url: "api/Vozac",
            method: "GET",
            params: { lat: latitude, lng: longitude }
        }).then(function (response) {
            $scope.lista = response.data;
            $scope.listaVozacaVidljiva = true;
            $scope.sortType = 'username';
            $scope.sortReverse = false;
            $scope.search = '';
            if ($scope.lista === []) {
                $scope.stanjeVozaca = "Nema slobodnih vozaca!";
            }
            else {
                $scope.stanjeVozaca = "";
            }
            });


    };

    $scope.processForm = function () {
        $scope.errorName = "";
        if (!($scope.formData.hasOwnProperty("vozacUsername"))) {
            $scope.errorName = "Niste odabrali vozaca.";
        }
        if ($scope.formData.lokacija.adresa === "" || $scope.formData.odrediste.adresa === "") {
            $scope.errorName = "Niste uneli jednu od adresa.";
        }
        if ($scope.formData.lokacija.latitude === 360 || $scope.formData.odrediste.latitude === 360) {
            $scope.errorName = "Niste oznacili jednu od adresa na mapi";
        }
        if ($scope.errorName === "") {
            $http({
                method: 'POST',
                url: 'api/Voznja/',
                data: $.param($scope.formData),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function successCallback(response) {
                $scope.errorName = "Voznja uspesno dodata!";
            }, function errorCallback(response) {
                $scope.errorName = "Neuspesna registracija, greska " + status;
                });
        }
    };
});

app.controller('BlockFormController', function ($scope, $http) {
    $scope.flipBlock = function () {
        $scope.errorName = "";
        if ($scope.username === "") {
            $scope.errorName = "Niste uneli korisnicko ime.";
        }
        else {
            $http({
                method: 'POST',
                url: 'api/Control/',
                params: { username: $scope.username },
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function successCallback(response) {
                $scope.errorName = "Korisnik uspesno blokiran/odblokiran!";
            }, function errorCallback(response) {
                $scope.errorName = "Korisnik ne postoji, ili postoji problem na serveru.";
            });
        }
    };
});

app.controller('NeobradjeneController', function ($scope, $http) {
    $scope.lista = [];
    $scope.filteri = [
        { model: "komentar.ocena" },
        { model: "datum" },
        { model: "iznos" },
        { model: "status" }
    ];
    $scope.tipFiltera = "komentar.ocena";

    $scope.refresh = function () {
        $http({
            url: "api/Voznja",
            method: "GET",
        }).then(function (response) {
            $scope.lista = response.data;
            $scope.sortType = $scope.tipFiltera;
            $scope.sortReverse = false;
            $scope.search = '';
            if ($scope.lista === []) {
                $scope.stanjeVoznji = "Nema slobodnih voznji!";
            }
            else {
                $scope.stanjeVoznji = "";
            }
        });

    };
});