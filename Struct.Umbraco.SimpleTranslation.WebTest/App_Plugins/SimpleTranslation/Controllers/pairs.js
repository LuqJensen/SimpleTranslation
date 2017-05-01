var app = angular.module("umbraco");

app.controller("SimpleTranslation.Pairs.Controller", function($scope, $http, $timeout) {

    getViewModel();

    function getViewModel() {
        $http.get('/umbraco/backoffice/api/Pairs/GetViewModel').success(function(response) {
            $scope.isEditor = response.isEditor;
            $scope.keys = response.pairs;
            $scope.languages = response.languages;
        });
    }

    $scope.checkKey = function(key) {
        angular.forEach($scope.languages, function(lang) {
            key[lang.id] = key.master;
        });
    }

    $scope.sendToTranslation = function() {
        event.preventDefault();

        var tasks = [];
        angular.forEach($scope.keys, function(pair) {
            pair.master = false;

            angular.forEach($scope.languages, function(lang) {
                if (pair[lang.id]) {
                    tasks.push({
                        id: pair.id,
                        languageId: lang.id
                    });
                    pair.translationTasks[lang.id] = true;
                }

            });

        });

        $http.post("/umbraco/backoffice/api/Pairs/SendToTranslation", tasks).success(function() {
            sendMessage("Selected keys have been send to translation");
        });
    }

    $scope.sendToTranslationWholeLanguage = function(langId) {
        event.preventDefault();

        $http.post("/umbraco/backoffice/api/Pairs/SendToTranslationWholeLanguage?langId=" + langId).success(function() {
            angular.forEach($scope.keys, function(pair) {
                pair.master = false;
                pair.translationTasks[langId] = true;

            });
            sendMessage("Keys for the language have been send to translation");
        });
    }

    function sendMessage(message) {
        $scope.sendMessage = message;
        $timeout(function() { $scope.sendMessage = ""; }, 3000);
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

        $http.post("/umbraco/backoffice/api/Proposals/CreateProposal",
        {
            languageId: $scope.dialogData.language.id,
            id: $scope.dialogData.key.id,
            value: $scope.proposedText
        }).success(function() {
            UmbClientMgr.closeModalWindow();
        });
    };
});