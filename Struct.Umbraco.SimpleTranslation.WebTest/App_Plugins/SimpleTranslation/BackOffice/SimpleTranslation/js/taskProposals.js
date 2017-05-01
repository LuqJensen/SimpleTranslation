var app = angular.module("umbraco");

app.controller("SimpleTranslation.TaskProposals.Controller", function($scope, $http) {
    // http://stackoverflow.com/a/2655976/5552144 jQuery.css() doesnt support !important, so must use attr() and take into account not to override all other css in the process.
    $(".umb-modal.fade.in").attr("style", function(i, s) { return s + "width: 50% !important; margin-left: -50% !important;" });

    $http.get("/umbraco/backoffice/api/Tasks/GetProposalsForTask?id=" + $scope.dialogData.id + "&languageId=" + $scope.dialogData.languageId).success(function(response) {
        $scope.data = response;
    });
});