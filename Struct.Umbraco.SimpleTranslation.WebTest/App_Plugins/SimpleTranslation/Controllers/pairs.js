var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http, $timeout) {

    getTranslatableKeys();
    getTranslationTasks();
    getMyLanguages();

    function getTranslatableKeys() {
        $http.get('/umbraco/backoffice/api/Pairs/GetTranslatableKeys').success(function(response) {
            $scope.role = response.role;
            $scope.keys = response.pairs;
        });
    }

    function getMyLanguages() {
        $http.get('/umbraco/backoffice/api/Pairs/GetMyLanguages').success(function(response) {
            $scope.languages = response;
        });
    }

    function getTranslationTasks() {
        $http.get('/umbraco/backoffice/api/Pairs/GetTranslationTasks').success(function(response) {
            var tasks = [];

            angular.forEach(response, function(task) {
                tasks.push({
                    id: task.id,
                    languageId: task.languageId
                });
            });
            $scope.tasks = tasks;
        });
    }

    $scope.isTask = function(key, langId) {
        return $scope.tasks.some(function(e) {
            return e.id === key.id && e.languageId === langId;
        });
    };

    $scope.getTranslation = function(object, langId) {
        if (object.translationTexts[langId]) {
            $scope.translation = object.translationTexts[langId];
        }
        else {
            $scope.translation = null;
        }
    }

    $scope.sendToTranslation = function() {
        event.preventDefault();

        $http.post("/umbraco/backoffice/api/Pairs/SendToTranslation", $scope.selection).success(function() {
            angular.forEach($scope.selection, function(task) {
                $scope.tasks.push({
                    id: task.id,
                    languageId: task.languageId
                });
            });
            $scope.selection = [];
        });
        sendMessage("Selected keys have been send to translation");
    }

    $scope.sendToTranslationWholeLanguage = function(langId) {
        event.preventDefault();

        $.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?langId=" + langId).success(function() {});
        getTranslationTasks();
        sendMessage("Keys for the language have been send to translation");
    }

    function sendMessage(message) {
        $scope.sendMessage = message;
        $timeout(function() { $scope.sendMessage = ""; }, 3000);
    }

    $scope.selection = [];

    $scope.toggleSelection = function(key, languageId) {
        var pos = (function() {
            var pos;

            var found = $scope.selection.some(function(e, i) {
                pos = i;
                return e.id === key.id && e.languageId === languageId;
            });

            return found ? pos : -1;
        }());

        if (pos > -1) {
            $scope.selection.splice(pos, 1);
        }
        else {
            $scope.selection.push({
                id: key.id,
                languageId: languageId
            });
        }
    };

    $scope.toggleSelectionKey = function(key, checked) {
        if (checked) {
            angular.forEach($scope.languages, function(lang) {
                if (!$scope.selection.some(function(e) {
                    return e.id === key.id && e.languageId === lang.id;
                }) && !$scope.tasks.some(function(e) {
                    return e.id === key.id && e.languageId === lang.id;
                })) {
                    $scope.selection.push({
                        id: key.id,
                        languageId: lang.id
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
                        return e.id === key.id && e.languageId === lang.id;
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