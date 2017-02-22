

var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http) {

    getTranslatableKeys();
    getLanguages();

    function getTranslatableKeys() {
        $http.get('/umbraco/backoffice/api/Pairs/GetTranslatableKeys').success(function(response) {
            $scope.data = response;

            $scope.keys = [];
            for (var object in $scope.data) {
                loop($scope.data[object]);
            }
        });
    }

    function getLanguages() {
        $http.get('/umbraco/backoffice/api/Pairs/GetLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    function loop(object) {
        $scope.keys.push(object);
        if (object.children) {
            angular.forEach(object.children, function(values) {
                loop(values);
            });
        }
    }
});