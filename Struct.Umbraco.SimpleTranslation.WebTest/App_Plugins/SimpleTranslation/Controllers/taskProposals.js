var app = angular.module("umbraco");

app.controller("SimpleTranslation.TaskProposals.Controller", function($scope, $http) {
    $http.get("/umbraco/backoffice/api/Tasks/GetProposalsForTask?id=" + $scope.dialogData.id + "&languageId=" + $scope.dialogData.languageId).success(function(response) {
        $scope.data = response;
    });
});

// Umbraco 7.5 uses Angular 1.1.5. Better built-in filters were introduced later http://stackoverflow.com/a/29675847/5552144
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