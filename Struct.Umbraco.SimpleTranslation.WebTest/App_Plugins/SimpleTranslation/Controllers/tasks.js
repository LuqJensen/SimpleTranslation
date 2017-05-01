var app = angular.module("umbraco");

app.controller("SimpleTranslation.Tasks.Controller", function($scope, $http) {
    function getViewModel() {
        $http.get('/umbraco/backoffice/api/Tasks/GetViewModel').success(function(response) {
            $scope.tasks = response.tasks;
            $scope.isEditor = response.isEditor;
            $scope.languages = response.languages;
            $scope.selectedLanguage = $scope.languages[0].id;
        });
    }

    getViewModel();

    $scope.deleteTask = function(pk) {
        event.preventDefault();

        UmbClientMgr.openAngularModalWindow({
            template: '/App_Plugins/SimpleTranslation/BackOffice/SimpleTranslation/partialViews/dialog.html',
            dialogData: {
                text: "Are you sure you want to delete this task?",
                title: "Delete task",
                okText: "Confirm",
                okCallback: function() {
                    $http.post("/umbraco/backoffice/api/Tasks/DeleteTask?id=" + pk).success(function() {
                        getViewModel();
                        UmbClientMgr.closeModalWindow();
                    });
                }
            }
        });
    }

    $scope.createProposal = function(task) {
        event.preventDefault();

        $http.post("/umbraco/backoffice/api/Proposals/CreateProposal", task).success(function() {
            getViewModel();
        });
    }

    $scope.getProposalsForTask = function(id, languageId) {
        event.preventDefault();

        UmbClientMgr.openAngularModalWindow({
            template: '/App_Plugins/SimpleTranslation/BackOffice/SimpleTranslation/partialViews/taskProposals.html',
            dialogData: {
                id: id,
                languageId: languageId
            }
        });
    }
});