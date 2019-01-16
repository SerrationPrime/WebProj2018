

app.controller('VozController', function ($scope, $http) {
    $scope.selfState = {};
    $scope.selfState.lokacija = {};
    $scope.filteri = [
        { model: "komentar.ocena" },
        { model: "datum" },
        { model: "iznos" },
        { model: "status" }
    ];
    $scope.komentarForma = false;
    $scope.tipFiltera = "komentar.ocena";
    $http({
        url: "api/Vozac",
        method: "GET"
    }).then(function (response) {
        selfState = response.data;
        });
    position = { lat: $scope.selfState.lokacija.latitude, lng: $scope.selfState.lokacija.longitude };

    $scope.refresh = function(){
        $http({
            url: "api/Voznja",
            method: "GET"
        }).then(function (response) {
            $scope.lista = response.data;
            $scope.sortType = $scope.tipFiltera;
            $scope.sortReverse = false;
            $scope.search = '';
        });

    };

    $scope.stanjePlus = function (voznja) {
        $scope.errorName = "";
        $scope.novoStanje = 0;
        $scope.radnaVoznja = voznja;
        if ($scope.radnaVoznja.status === 0 || $scope.radnaVoznja.status === 1 || $scope.radnaVoznja.status === 2) {
            $scope.novoStanje = 3;
        }
        else if ($scope.radnaVoznja.status === 3) {
            $scope.novoStanje = 6;
        }
        if ($scope.novoStanje !== 0) {
            $http({
                url: "api/VoznjaEdit",
                method: "POST",
                params: { stanje: $scope.novoStanje, datum: $scope.radnaVoznja.vreme }
            }).then(function successCallback(response) {
                if ($scope.novoStanje === 3)
                    $scope.errorName = "Voznja prihvaćena.";

                else
                {
                    $scope.errorName = "Voznja zavrsena.";
                    $scope.radnaVoznja.stanje = $scope.novoStanje;
                    $scope.komentarForma = true;
                } 
            }, function errorCallback(response) {
                $scope.errorName = "Postoji problem na serveru. Probajte ponovo kasnije";
            });
        }
        $scope.refresh();
    };

    $scope.stanjeMinus = function (voznja) {
        $scope.errorName = "";
        $scope.novoStanje = 0;
        $scope.radnaVoznja = voznja;
        if ($scope.radnaVoznja.status === 1 || $scope.radnaVoznja.status === 2 || $scope.radnaVoznja.status === 3) {
            $scope.novoStanje = 5;
            $http({
                url: "api/VoznjaEdit",
                method: "POST",
                params: { stanje: $scope.novoStanje, datum: $scope.radnaVoznja.vreme}
            }).then(function successCallback(response) {
                $scope.errorName = "Voznja poništena.";
                $scope.radnaVoznja.stanje = $scope.novoStanje;
                $scope.komentarForma = true;
            }, function errorCallback(response) {
                $scope.errorName = "Postoji problem na serveru. Probajte ponovo kasnije";
            });
        }
        $scope.refresh();
    };

    $scope.osveziPoziciju = function () {
        $scope.selfState.lokacija.latitude = latitude;
        $scope.selfState.lokacija.longitude = longitude;

        $http({
            url: "api/VozacEdit",
            method: "POST",
            params: { lat: $scope.selfState.lokacija.latitude, lng: $scope.selfState.lokacija.longitude }
        });
    };

    $scope.komentarisi = function () {
        $scope.errorName = "";
        $scope.radnaVoznja.komentar = {};
        $scope.radnaVoznja.komentar.opis = $scope.komentar.opis;
        $scope.radnaVoznja.komentar.ocena = $scope.komentar.ocena;
        $http({
            method: "POST",
            url: "api/Komentar",
            data: $.param($scope.radnaVoznja),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function successCallback(response) {
            $scope.errorName = "Hvala vam!.";
            $scope.komentarForma = true;
            $scope.formData.komentar.opis = "";
            $scope.formData.komentar.ocena = 0;
        }, function errorCallback(response) {
            $scope.errorName = "Postoji problem na serveru. Probajte ponovo kasnije";
        });
        $scope.refresh();
    };
});