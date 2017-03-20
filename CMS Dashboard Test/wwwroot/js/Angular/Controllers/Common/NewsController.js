(function () {
    'use strict';

    angular
        .module(angularAppName)
        .constant('moment', moment)
        .controller('NewsController', NewsController);

    NewsController.$inject = ['$scope', 'moment', 'SidebarService', 'AccountService', 'NewsService']; 
    function NewsController($scope, moment, SidebarService, AccountService, NewsService)
    {
        $scope.title = 'NewsController';
        $scope.htmlData = '';
        $scope.postTitle = '';
        $scope.loading = true;
        $scope.validTitle = true;
        $scope.minTitleLength = 5;
        $scope.validHtmlData = true;
        $scope.minHtmlDataLength = 20;
        $scope.newNewsPostLoading = false;
        $scope.news = [];
        $scope.sidebar = SidebarService;

        AccountService.getLoggedInUser()
            .then(function (response)
            {
                $scope.currentUser = response.data;
            })

        NewsService.get()
            .then(function (response) {
                $scope.news = response.data;

                angular.forEach($scope.news, function (news) {
                    news.tooltip = "Minimise";
                });

                $scope.loading = false;
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

        $scope.isFormValid = function()
        {
            if ($scope.postTitle.length < $scope.minTitleLength)
            {
                $scope.validTitle = false;
            }
            else
            {
                $scope.validTitle = true;
            }

            if ($scope.htmlData.length < $scope.minHtmlDataLength)
            {
                $scope.validHtmlData = false;
            }
            else
            {
                $scope.validHtmlData = true;
            }

            return $scope.validTitle && $scope.validHtmlData;
        };

        $scope.deleteNewsPost = function (newsPost) {
            $scope.loading = true;

            NewsService.delete(newsPost.Id)
                .then(function (response) {

                    var index = $scope.news.indexOf(newsPost);
                    $scope.news.splice(index, 1);

                    $scope.loading = false;

                }, function (error) {
                    window.alert(error);
                    $scope.loading = false;
                });
        };

        $scope.createNewsPost = function (actionForm) {

            if (!$scope.isFormValid()) {
                return;
            }

            $scope.newNewsPostLoading = true;

            var post = {};
            post.Title = $scope.postTitle;
            post.Body = $scope.htmlData;
            post.PostedBy = $scope.currentUser.UserName;
            post.Timestamp = new Date();
            post.ApplicationUserId = $scope.currentUser.Id;

            NewsService.post(post)
                .then(function (response) {
                    $scope.news.push(response.data);
                    $('#newsModal').modal('hide');

                    $scope.postTitle = '';
                    $scope.htmlData = '';

                    $scope.newNewsPostLoading = false;
                    actionForm.autoValidateFormOptions.resetForm(); // reset the state of the form

                }, function (error) {
                    window.alert(error);
                    $scope.newNewsPostLoading = false;
                });
        };

        $scope.prepareModal = function () {

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