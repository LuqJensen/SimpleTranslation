var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http, $route) {
    getTranslatableKeys();
    getLanguages();

    function getTranslatableKeys() {
        $http.get('/umbraco/backoffice/api/Pairs/GetTranslatableKeys').success(function(response) {
            var keys = [];
            for (var object in response) {
                if (response.hasOwnProperty(object)) {
                    loop(response[object], keys);
                }
            }
            $scope.keys = keys;
        });
    }

    function getLanguages() {
        $http.get('/umbraco/backoffice/api/Pairs/GetLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    function loop(object, keys) {
        keys.push(object);
        if (object.children) {
            angular.forEach(object.children, function(values) {
                loop(values, keys);
            });
        }
    }

    $scope.getTranslation = function(object, langId) {
        if (object.translationTexts[langId]) {
            $scope.translation = object.translationTexts[langId].value;
        }
        else {
            $scope.translation = null;
        }
    }

    $scope.sendToTranslation = function(key, lang) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslation?id=" + key.id + "&langId=" + lang.id).success(function() {});
    }

    $scope.sendToTranslationAllLanguages = function(key) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationAllLanguages?id=" + key.id).success(function() {});
    }

    $scope.sendToTranslationWholeLanguage = function(langId) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?langId=" + langId).success(function() {});
    }
});