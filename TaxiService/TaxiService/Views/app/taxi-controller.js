(function () {
    'use strict';

    angular
        .module('app')
        .controller('taxi_controller', taxi_controller);

    taxi_controller.$inject = ['$location'];

    function taxi_controller($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'taxi_controller';

        activate();

        function activate() { }
    }
})();
