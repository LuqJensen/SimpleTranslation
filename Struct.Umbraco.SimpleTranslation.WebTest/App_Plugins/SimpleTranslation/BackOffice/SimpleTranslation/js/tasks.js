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

    $scope.uploadXml = function() {
        event.preventDefault();

        var r = new FileReader();
        // Listen for read done event.
        r.onloadend = function(e) {
            $http.post("/umbraco/backoffice/api/Tasks/ImportTranslationsFromXml", { value: e.target.result }).success(function(response) {
                $scope.uploadError = "Xml file succesfully uploaded.";
            }).error(function(response) { $scope.uploadError = response.ExceptionMessage; });
        };

        var f = $("#file")[0].files[0];
        // Begin reading
        r.readAsText(f);
    }
});