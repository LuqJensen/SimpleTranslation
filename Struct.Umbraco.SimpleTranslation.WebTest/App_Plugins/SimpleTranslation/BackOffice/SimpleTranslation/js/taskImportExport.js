var app = angular.module("umbraco");

app.controller("SimpleTranslation.ImportExport.Controller", function($scope, $http) {
    $scope.selectedFromLanguage = $scope.dialogData.languages[0].id;
    $scope.selectedToLanguage = $scope.dialogData.languages[1].id;

    $scope.uploadXml = function () {
        event.preventDefault();

        var r = new FileReader();
        // Listen for read done event.
        r.onloadend = function (e) {
            $http.post("/umbraco/backoffice/api/Tasks/ImportTranslationsFromXml", { value: e.target.result }).success(function (response) {
                $scope.uploadError = "Xml file succesfully uploaded.";
            }).error(function (response) { $scope.uploadError = response.ExceptionMessage; });
        };

        var f = $("#file")[0].files[0];
        // Begin reading
        r.readAsText(f);
    }
});