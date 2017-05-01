var app = angular.module("umbraco");

app.controller("SimpleTranslation.Proposals.Controller", function($scope, $http) {
    function getViewModel() {
        $http.get('/umbraco/backoffice/api/Proposals/GetViewModel').success(function(response) {
            $scope.data = response.proposals;
        });
    }

    getViewModel();

    $scope.acceptProposal = function(selectedProposal, keyProposals) {
        event.preventDefault();

        var relatedProposalsCount = keyProposals.filter(function(proposal) {
            return proposal.languageId === selectedProposal.languageId;
        }).length - 1;

        var text = "Are you sure you want to accept this proposal?\n";
        if (relatedProposalsCount > 0) {
            text += "Accepting this proposal will automatically delete " + relatedProposalsCount + " other suggestion(s) for this language.";
        }

        UmbClientMgr.openAngularModalWindow({
            template: '/App_Plugins/SimpleTranslation/BackOffice/SimpleTranslation/partialViews/dialog.html',
            dialogData: {
                text: text,
                title: "Accept proposal",
                okText: "Accept",
                okCallback: function() {
                    $http.post("/umbraco/backoffice/api/Proposals/AcceptProposal?id=" + selectedProposal.primaryKey).success(function() {
                        getViewModel();
                        UmbClientMgr.closeModalWindow();
                    });
                }
            }
        });
    }

    $scope.rejectProposal = function(pk) {
        event.preventDefault();

        UmbClientMgr.openAngularModalWindow({
            template: '/App_Plugins/SimpleTranslation/BackOffice/SimpleTranslation/partialViews/dialog.html',
            dialogData: {
                text: "Are you sure you want to reject this proposal?",
                title: "Reject proposal",
                okText: "Confirm",
                okCallback: function() {
                    $http.post("/umbraco/backoffice/api/Proposals/RejectProposal?id=" + pk).success(function() {
                        getViewModel();
                        UmbClientMgr.closeModalWindow();
                    });
                }
            }
        });
    }
});