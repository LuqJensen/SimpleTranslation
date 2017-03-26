var app = angular.module("umbraco");

app.controller("SimpleTranslation.UserLanguages.Controller", function($scope, $http, $routeParams, $timeout) {
    getUser();
    getLanguages();

    function getUser() {
        $http.get('/umbraco/backoffice/api/UserLanguages/GetUser?id=' + $routeParams.id).success(function(response) {
            $scope.user = response;
        });
    }

    function getLanguages() {
        $http.get('/umbraco/backoffice/api/UserLanguages/GetLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    $scope.save = function() {
        event.preventDefault();
        angular.forEach($scope.addSelection, function(langId) {
            $.post("/umbraco/backoffice/api/UserLanguages/AddLanguage?userId=" + $scope.user.id + "&langId=" + langId).success(function () { });
        });
        angular.forEach($scope.removeSelection, function(langId) {
            $.post("/umbraco/backoffice/api/UserLanguages/RemoveLanguage?userId=" + $scope.user.id + "&langId=" + langId).success(function () { });
        });
        saveMessage("Saved");
        getUser();
        $scope.addSelection = [];
        $scope.removeSelection = [];
    }

    function saveMessage(message) {
        $scope.saveMessage = message;
        $timeout(function () { $scope.saveMessage = ""; }, 3000);
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