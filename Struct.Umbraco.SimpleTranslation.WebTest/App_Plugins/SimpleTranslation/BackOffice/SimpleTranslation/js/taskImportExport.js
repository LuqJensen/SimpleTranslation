var app = angular.module("umbraco");

app.controller("SimpleTranslation.ImportExport.Controller", function($scope, $http) {
    $scope.selectedFromLanguage = $scope.dialogData.languages[0].id;
    $scope.selectedToLanguage = $scope.dialogData.languages[1].id;

    $scope.uploadXml = function () {
        event.preventDefault();
        showMessage("Importing...", "waiting");
        if ($("#file")[0].files.length === 0) {
            showMessage("Please upload a file", "error");
            return;
        }

        var r = new FileReader();
        // Listen for read done event.
        r.onloadend = function (e) {
            if ($scope.importAs === "Proposals") {
                $http.post("/umbraco/backoffice/api/Tasks/ImportProposalsFromXml", { value: e.target.result }).success(function(response) {
                    showMessage("Xml file succesfully imported.", "succes");
                }).error(function (response) {
                    showMessage(response.ExceptionMessage, "error");
                });
            } else if ($scope.importAs === "Translations"){
                $http.post("/umbraco/backoffice/api/Tasks/ImportTranslationsFromXml", { value: e.target.result }).success(function (response) {
                    showMessage("Xml file succesfully imported.", "succes");
                }).error(function(response) {
                    showMessage(response.ExceptionMessage, "error");
                });
            } else {
                showMessage("Choose to import as Proposals or Translations", "error");
            }
        };

        var f = $("#file")[0].files[0];
        // Begin reading
        r.readAsText(f);
    }

    function showMessage(message, type) {
        $scope.message = message;
        $scope.messageStyle = type;
    }
});