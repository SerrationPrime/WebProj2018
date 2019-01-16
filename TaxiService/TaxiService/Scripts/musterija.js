app.controller('MusController', function ($scope, $http) {
    $scope.komentarForma = false;
    latitude = 360;
    longitude = 360;
    $scope.formData = {};
    $scope.formData.zeljeniTip = 0;
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
            method: "GET"
        }).then(function (response) {
            $scope.lista = response.data;
            $scope.sortType = $scope.tipFiltera;
            $scope.sortReverse = false;
            $scope.search = '';
        });

    };

    $scope.novaVoznja = function () {
        $scope.errorName = "";
        if ($scope.formData.lokacija.adresa === "") {
            $scope.errorName = "Niste uneli adresu.";
        }
        if (latitude === 360 || longitude === 360) {
            $scope.errorName = "Niste oznacili adresu na mapi";
        }
        if ($scope.errorName === "") {
            $scope.formData.lokacija.latitude = latitude;
            $scope.formData.lokacija.longitude = longitude;
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
        $scope.refresh();
    };

    $scope.ponisti = function (voznja) {
        $scope.errorName = "";
        $scope.novoStanje = 0;
        $scope.radnaVoznja = voznja;
        if ($scope.radnaVoznja.status === 1 || $scope.radnaVoznja.status === 2 || $scope.radnaVoznja.status === 3) {
            $scope.novoStanje = 4;
            $http({
                url: "api/VoznjaEdit",
                method: "POST",
                params: { stanje: $scope.novoStanje, datum: $scope.radnaVoznja.vreme }
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