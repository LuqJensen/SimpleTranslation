var app = angular.module("umbraco");

app.controller("SimpleTranslation.UserSettings.Controller", function($scope, $http, $routeParams, $timeout) {
    getUser();
    getLanguages();
    getRole();

    function getUser() {
        $http.get('/umbraco/backoffice/api/UserSettings/GetUser?id=' + $routeParams.id).success(function(response) {
            $scope.user = response;
        });
    }

    function getLanguages() {
        $http.get('/umbraco/backoffice/api/UserSettings/GetLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    function getRole() {
        $http.get('/umbraco/backoffice/api/UserSettings/GetRole?id=' + $routeParams.id).success(function(response) {
            $scope.currentRole = response;
            $scope.newRole = response;
        });
    }

    $scope.save = function() {
        event.preventDefault();

        angular.forEach($scope.addSelection, function(langId) {
            $.post("/umbraco/backoffice/api/UserSettings/AddLanguage?userId=" + $scope.user.id + "&langId=" + langId).success(function() {});
        });
        angular.forEach($scope.removeSelection, function(langId) {
            $.post("/umbraco/backoffice/api/UserSettings/RemoveLanguage?userId=" + $scope.user.id + "&langId=" + langId).success(function() {});
        });
        saveMessage("Saved");
        getUser();
        $scope.addSelection = [];
        $scope.removeSelection = [];

        if ($scope.currentRole !== $scope.newRole) {
            $.post("/umbraco/backoffice/api/UserSettings/SetRole?userId=" + $scope.user.id + "&roleId=" + $scope.newRole).success(function() {});
        }

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