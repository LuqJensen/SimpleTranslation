var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http, $timeout) {
    getTranslatableKeys();
    getRole();
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

    function getRole() {
        $http.get('/umbraco/backoffice/api/Pairs/GetRole').success(function(response) {
            $scope.role = response;
            getLanguages();
        });
    }

    function getLanguages() {
        if ($scope.role == 1) {
            $http.get('/umbraco/backoffice/api/Pairs/GetAllLanguages').success(function(response) {
                $scope.languages = response;
            });
        }
        else {
            $http.get('/umbraco/backoffice/api/Pairs/GetTranslatorLanguages').success(function(response) {
                $scope.languages = response;
            });
        }
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
        sendMessage("Selected keys has been send to translation");
    }

    $scope.sendToTranslationWholeLanguage = function(langId) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?langId=" + langId).success(function() {});
        getTranslationTasks();
        sendMessage("Keys for the language has been send to translation");
    }

    function sendMessage(message) {
        $scope.sendMessage = message;
        $timeout(function() { $scope.sendMessage = ""; }, 3000);
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

    $scope.getNewProposalForm = function(key, language) {
        event.preventDefault();

        UmbClientMgr.openAngularModalWindow({
            template: '/App_Plugins/SimpleTranslation/BackOffice/SimpleTranslation/partialViews/pairsNewProposal.html',
            dialogData: {
                key: key,
                language: language
            }
        });
    }

    $scope.createProposal = function() {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/CreateProposal?langId=" + $scope.dialogData.language.id + "&uniqueId=" + $scope.dialogData.key.id + "&value=" + $scope.proposedText).success(function() {
            UmbClientMgr.closeModalWindow();
        });
    };
});