var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http, $route) {

    $scope.taskView = false;
    $scope.switchBtnText = "Translation View: Off";
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

    $scope.switchView = function() {
        $scope.taskView = !$scope.taskView;
        if ($scope.taskView) {
            $scope.switchBtnText = "Translation View: On";
        }
        else {
            $scope.switchBtnText = "Translation View: Off";
        }
        getTranslatableKeys();
    }

    $scope.getTranslation = function(object, langId) {
        if (object.translationTexts[langId]) {
            $scope.translation = object.translationTexts[langId].value;
        }
        else {
            $scope.translation = null;
        }
    }

    $scope.sendToTranslation = function(key, langId) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslation?id=" + key.primaryKey + "&langId=" + langId).success(function() {});
    }

    $scope.sendToTranslationAllLanguages = function(key) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationAllLanguages?id=" + key.primaryKey).success(function() {});
    }

    $scope.sendToTranslationWholeLanguage = function (langId) {
        event.preventDefault();

        console.log("send to translation");
        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?id=" + langId).success(function() {

            console.log("send to translatiosdsdsn");
        });
    }
});