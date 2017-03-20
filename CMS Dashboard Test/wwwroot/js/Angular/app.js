(function () {
    'use strict';

    angular.module(angularAppName, ['angular.filter', 'angular-ladda', 'jcs-autoValidate', 'ui.calendar',
        '720kb.datepicker', 'ngAutocomplete', 'isteven-multi-select', 'textAngular', 'ngSanitize', 'ngBootbox'])
        .directive('repeatDone', function ($timeout) {
            return function (scope, element, attrs) {
                if (scope.$last){ // all are rendered
                    setTimeout(function() { // setting a timeout allows angular to process the expression in the DOM... this is useful in my case when i initialize external events
                        scope.$eval(attrs.repeatDone);
                    }, 2)
                }
            }
        })
        .directive('ngReallyClick', function ($timeout) {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    element.bind('click', function () {
                        var message = attrs.ngReallyMessage;
                        if (message && confirm(message)) {
                            scope.$apply(attrs.ngReallyClick);
                        }
                    });
                }
            }
        });
})();