var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http) {
    getTranslatableKeys();
    getLanguages();
    getTranslationTasks();

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

    function loop(object, keys) {
        keys.push(object);
        if (object.children) {
            angular.forEach(object.children, function(values) {
                loop(values, keys);
            });
        }
    }

    function getLanguages() {
        $http.get('/umbraco/backoffice/api/Pairs/GetLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    function getTranslationTasks() {
        $http.get('/umbraco/backoffice/api/Pairs/GetTranslationTasks').success(function(response) {
            var tasks = [];

            angular.forEach(response, function(task) {
                tasks.push({
                    keyId: task.id,
                    langId: task.languageId
                });
            });
            $scope.tasks = tasks;
        });
    }

    $scope.isTask = function(key, langId) {
        return $scope.tasks.some(function(e) {
            return e.keyId === key.id && e.langId === langId;
        });
    };

    $scope.getTranslation = function(object, langId) {
        if (object.translationTexts[langId]) {
            $scope.translation = object.translationTexts[langId].value;
        }
        else {
            $scope.translation = null;
        }
    }

    $scope.sendToTranslation = function() {
        event.preventDefault();
        angular.forEach($scope.selection, function(task) {
            $.post("/umbraco/backoffice/api/Pairs/SendToTranslation?id=" + task.keyId + "&langId=" + task.langId).success(function() {});
            $scope.tasks.push({
                keyId: task.keyId,
                langId: task.langId
            });
        });
    }

    $scope.sendToTranslationWholeLanguage = function(langId) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?langId=" + langId).success(function() {});
    }

    $scope.selection = [];

    $scope.toggleSelection = function(key, langId) {
        var pos = (function() {
            var pos;

            var found = $scope.selection.some(function(e, i) {
                pos = i;
                return e.keyId === key.id && e.langId === langId;
            });

            return found ? pos : -1;
        }());

        if (pos > -1) {
            $scope.selection.splice(pos, 1);
        }
        else {
            $scope.selection.push({
                keyId: key.id,
                langId: langId
            });
        }
    };

    $scope.toggleSelectionKey = function(key, checked) {
        if (checked) {
            angular.forEach($scope.languages, function(lang) {
                if (!$scope.selection.some(function(e) {
                    return e.keyId === key.id && e.langId === lang.id;
                }) && !$scope.tasks.some(function(e) {
                    return e.keyId === key.id && e.langId === lang.id;
                })) {
                    $scope.selection.push({
                        keyId: key.id,
                        langId: lang.id
                    });
                }
            });
        }
        else {
            angular.forEach($scope.languages, function(lang) {
                var pos = (function() {
                    var pos;

                    var found = $scope.selection.some(function(e, i) {
                        pos = i;
                        return e.keyId === key.id && e.langId === lang.id;
                    });

                    return found ? pos : -1;
                }());
                if (pos > -1) {
                    $scope.selection.splice(pos, 1);
                }
            });
        }
    }
});