(function () {
    'use strict';

   angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('NewsController', NewsController);

    NewsController.$inject = ['$scope', 'moment', 'NewsService']; 

    function NewsController($scope, moment, NewsService)
    {
        $scope.title = 'NewsController';
        $scope.news = [];
        NewsService.get()
            .then(function (response) {
                $scope.news = response.data;
                angular.forEach($scope.news, function (news) {
                    news.tooltip = "Minimise";
                });
            });

        $scope.preparePostageTime = function (newsPost) {
            var timestamp = moment(newsPost.Timestamp);
            var timestampDateFormatted = timestamp.format('LL'); // eg. March 17, 2016 
            var timestampTimeFormatted = timestamp.format('LT'); // eg. 1:37 PM
            return timestampDateFormatted + " at " + timestampTimeFormatted;
        };

        $scope.preparePostedBy = function (newsPost) {
            return "By: " + newsPost.PostedBy;
        };

       $scope.determineCollapseBoxTooltipText = function (newsPost) {
            if (newsPost.collapsed == undefined || newsPost.collapsed == null || newsPost.collapsed == true)
            {
                newsPost.collapsed = false;
                newsPost.tooltip = "Minimise";
            }
            else
            {
                newsPost.collapsed = true;
                newsPost.tooltip = "Expand";
            }
        };
    }
})();