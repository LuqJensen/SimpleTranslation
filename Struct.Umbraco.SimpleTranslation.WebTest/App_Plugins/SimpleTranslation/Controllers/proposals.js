var app = angular.module("umbraco");

app.controller("SimpleTranslation.Proposals.Controller", function($scope, $http) {
    function getTranslationProposals() {
        $http.get('/umbraco/backoffice/api/Proposals/GetTranslationProposals').success(function(response) {
            $scope.data = response;
        });
    }

    getTranslationProposals();

    $scope.acceptProposal = function(id) {
        event.preventDefault();
        $http.post("/umbraco/backoffice/api/Proposals/AcceptProposal?id=" + id).success(function (response)
        {
            console.log("response is void atm. TODO: error handling and refresh on success?");
            getTranslationProposals();
        });
    }
});

app.filter('utc', function() {
    return function(val) {
        var date = new Date(val);
        return new Date(date.getUTCFullYear(),
            date.getUTCMonth(),
            date.getUTCDate(),
            date.getUTCHours(),
            date.getUTCMinutes(),
            date.getUTCSeconds());
    };
});