var app = angular.module("umbraco");

app.controller("SimpleTranslation.UserSettings.Controller", function($scope, $http, $routeParams, $timeout) {
    getViewModel();

    function getViewModel() {
        $http.get('/umbraco/backoffice/api/UserSettings/GetViewModel?id=' + $routeParams.id).success(function(response) {
            $scope.user = response.user;
            $scope.languages = response.languages;
            $scope.selectedLanguages = response.userLanguages;
            $scope.roles = response.roles;
            $scope.selectedRole = response.userRole;
        });
    }

    $scope.save = function() {
        event.preventDefault();

        var payload = {
            userId: $scope.user.id,
            userLanguages: $scope.languages.filter(function(language) {
                return language.checked;
            }).map(x => x.id),
            userRole: $scope.selectedRole
        }

        $http.post("/umbraco/backoffice/api/UserSettings/SaveSettings", payload).success(function() {
            saveMessage("Saved");

            getViewModel();
        });
    }

    function saveMessage(message) {
        $scope.saveMessage = message;
        $timeout(function() { $scope.saveMessage = ""; }, 3000);
    }
});