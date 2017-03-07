angular.module("umbraco").controller("SimpleTranslation.Tasks.Controller", function($scope, $http, $timeout) {
    function getTranslationTasks() {
        // Use jquery.post and jquery.get over Angular's http.post and http.get as these will wait for Angular to receive focus after dialogs.
        $.get('/umbraco/backoffice/api/Tasks/GetTranslationTasks').success(function(response) {
            // Instruct Angular to execute this on next digest. Workaround for Angular not updating regularly when not having focus due to dialogs.
            $timeout(function() {
                $scope.data = response.tasks;
                $scope.canDiscard = response.canDiscard;
            });
        });
    }

    getTranslationTasks();

    $scope.deleteTask = function(pk) {
        event.preventDefault();

        bootbox.confirm("Are you sure you want to delete this task?", function(result) {
            if (result) {
                // Use jquery.post and jquery.get over Angular's http.post and http.get as these will wait for Angular to receive focus after dialogs.
                $.post("/umbraco/backoffice/api/Tasks/DeleteTask?id=" + pk).success(function() {
                    getTranslationTasks();
                }) /*.error(function(response) {
                    console.log("response is void atm. TODO: error handling?");
                })*/;
            }
        });
    }

    $scope.createProposal = function(task) {
        event.preventDefault();

        // jquery.post drops the value of the task's id (Guid) causing it to default to 0000-000... angulars http.post works correctly here...
        $http.post("/umbraco/backoffice/api/Tasks/CreateProposal", task).success(function() {
            getTranslationTasks();
        });
    }
});