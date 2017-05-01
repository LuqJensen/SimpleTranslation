var app = angular.module("umbraco");

app.controller("SimpleTranslation.NewProposal.Controller", function($scope, $http) {
    $scope.createProposal = function() {
        event.preventDefault();

        $http.post("/umbraco/backoffice/api/Proposals/CreateProposal",
        {
            languageId: $scope.dialogData.language.id,
            id: $scope.dialogData.key.id,
            value: $scope.proposedText
        }).success(function() {
            $scope.close();
        });
    };
});