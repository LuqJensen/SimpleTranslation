var app = angular.module("umbraco");

app.controller("SimpleTranslation.Dialog.Controller", function($scope, $http) {
    // http://stackoverflow.com/a/2655976/5552144 jQuery.css() doesnt support !important, so must use attr() and take into account not to override all other css in the process.
    $(".umb-modal.fade.in").attr("style", function(i, s) { return s + "width: 25% !important; margin-left: -25% !important;" });
});