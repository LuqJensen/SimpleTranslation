angular.module("umbraco").controller("SimpleTranslation.Pairs.Controller", function ($scope, $http) {
    $http.get('/umbraco/backoffice/api/Pairs/GetTranslatableKeys').success(function (response) {
        $scope.keys = [];
        for (var prop in response) {
            loop(response[prop]);
        }

        function loop(data) {
            $scope.keys.push(data);
            if (data.children) {
                angular.forEach(data.children, function (values) {
                    loop(values);
                });
            }
        }
    });
});