(function() {
  'use strict';

  angular.module('clunch', ['clunchServices', 'ui.bootstrap', 'ui.bootstrap.dialog']).config([
    '$routeProvider', '$dialogProvider', function($routeProvider, $dialogProvider) {
      $routeProvider.when('/contacts', {
        templateUrl: 'Templates/contactList.html',
        controller: 'ContactList'
      }).when('/contacts/create', {
        templateUrl: 'Templates/contactEdit.html',
        controller: 'ContactCreate'
      }).when('/contacts/edit/:id', {
        templateUrl: 'Templates/contactEdit.html',
        controller: 'ContactEdit'
      }).when('/chat', {
        templateUrl: 'Templates/console.html',
        controller: 'Console'
      }).otherwise({
        redirectTo: '/contacts'
      });
      $dialogProvider.options({
        backdrop: true,
        backdropFade: true
      });
    }
  ]).run([
    '$rootScope', '$location', function($rootScope, $location) {
      return $rootScope.$on('$routeChangeSuccess', function(event, next, current) {
        var path, targetLink;
        path = '#' + $location.path();
        targetLink = $(".navbar a[href='" + path + "']");
        if (targetLink.length > 0) {
          $('.navbar li.active').removeClass('active');
          return targetLink.closest('li').addClass('active');
        }
      });
    }
  ]);

}).call(this);
