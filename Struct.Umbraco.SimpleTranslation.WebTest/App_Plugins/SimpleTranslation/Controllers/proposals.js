angular.module("umbraco").controller("SimpleTranslation.Proposals.Controller", function($scope, $http) {
    $http.get('/umbraco/backoffice/api/Proposals/GetTranslationProposals').success(function(response) {
        console.log(response);
    });
});