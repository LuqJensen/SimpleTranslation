var app = angular.module("umbraco");

app.controller("SimpleTranslation.Proposals.Controller", function($scope, $http, $timeout) {
    function getTranslationProposals() {
        // Use jquery.post and jquery.get over Angular's http.post and http.get as these will wait for Angular to receive focus after dialogs.
        $.get('/umbraco/backoffice/api/Proposals/GetTranslationProposals').success(function(response) {
            // Instruct Angular to execute this on next digest. Workaround for Angular not updating regularly when not having focus due to dialogs.
            $timeout(function() {
                $scope.data = response;
            });
        });
    }

    getTranslationProposals();

    $scope.acceptProposal = function(selectedProposal, keyProposals) {
        event.preventDefault();

        var relatedProposalsCount = keyProposals.filter(function(proposal) {
            return proposal.languageId === selectedProposal.languageId;
        }).length - 1;

        var text = "Are you sure you want to accept this proposal?\n";
        if (relatedProposalsCount > 0) {
            text += "Accepting this proposal will automatically delete " + relatedProposalsCount + " other suggestion(s) for this language.";
        }

        bootbox.confirm(text, function(result) {
            if (result) {
                // Use jquery.post and jquery.get over Angular's http.post and http.get as these will wait for Angular to receive focus after dialogs.
                $.post("/umbraco/backoffice/api/Proposals/AcceptProposal?id=" + selectedProposal.primaryKey).success(function() {
                    getTranslationProposals();
                }) /*.error(function(response) {
                    console.log("response is void atm. TODO: error handling?");
                })*/;
            }
        });
    }

    $scope.rejectProposal = function(pk) {
        event.preventDefault();

        bootbox.confirm("Are you sure you want to reject this proposal? Doing so will notify the proposer per email.", function(result) {
            if (result) {
                // Use jquery.post and jquery.get over Angular's http.post and http.get as these will wait for Angular to receive focus after dialogs.
                $.post("/umbraco/backoffice/api/Proposals/RejectProposal?id=" + pk).success(function() {
                    getTranslationProposals();
                }) /*.error(function(response) {
                    console.log("response is void atm. TODO: error handling?");
                })*/;
            }
        });
    }
});

// Umbraco 7.5 uses Angular 1.1.5. Better built-in filters were introduced later http://stackoverflow.com/a/29675847/5552144
app.filter('utc', function() {
    return function(val) {
        var date = new Date(val);
        return new Date(date.getUTCFullYear(),
            date.getUTCMonth(),
            date.getUTCDate(),
            date.getUTCHours(),
            date.getUTCMinutes(),
            date.getUTCSeconds());
    };
});