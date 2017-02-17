angular.module("umbraco").controller("SimpleTranslation.Pairs.Controller", function($scope, $http) {
    $http.get('/umbraco/backoffice/api/Pairs/GetTranslatableKeys').success(function(response) {
        console.log(response);
    });

});