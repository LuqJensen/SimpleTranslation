var app = angular.module("umbraco");

app.controller("SimpleTranslation.UserSettings.Controller", function($scope, $http, $routeParams, $timeout) {
    getUser();

    function getUser() {
        $http.get('/umbraco/backoffice/api/UserSettings/GetUser?id=' + $routeParams.id).success(function(response) {
            $scope.user = response.user;
            $scope.currentRole = response.role;
            $scope.newRole = response.role;
            $scope.languages = response.languages;
        });
    }

    $scope.save = function() {
        event.preventDefault();

        var payload = {
            userId: $scope.user.id,
            AddLanguages: $scope.addSelection,
            RemoveLanguages: $scope.removeSelection
        }

        $http.post("/umbraco/backoffice/api/UserSettings/ChangeLanguages", payload).success(function() {
            saveMessage("Saved");

            getUser();
        });

        if ($scope.currentRole !== $scope.newRole) {
            $.post("/umbraco/backoffice/api/UserSettings/SetRole?userId=" + $scope.user.id + "&roleId=" + $scope.newRole).success(function() {
                saveMessage("Saved");
            });
        }

        $scope.addSelection = [];
        $scope.removeSelection = [];
    }

    function saveMessage(message) {
        $scope.saveMessage = message;
        $timeout(function() { $scope.saveMessage = ""; }, 3000);
    }

    $scope.addSelection = [];
    $scope.removeSelection = [];

    $scope.toggleSelectionAdd = function(langId) {
        var pos = $scope.addSelection.indexOf(langId);
        if (pos > -1) {
            $scope.addSelection.splice(pos, 1);
        }
        else {
            $scope.addSelection.push(langId);
        }
    };

    $scope.toggleSelectionRemove = function(langId) {
        var pos = $scope.removeSelection.indexOf(langId);
        if (pos > -1) {
            $scope.removeSelection.splice(pos, 1);
        }
        else {
            $scope.removeSelection.push(langId);
        }
    };
});