(function() {
  'use strict';

  angular.module('clunch', ['clunchServices', 'ui.bootstrap']).config([
    '$routeProvider', function($routeProvider) {
      return $routeProvider.when('/contacts', {
        templateUrl: 'Templates/contactList.html',
        controller: 'ContactList'
      }).when('/contacts/create', {
        templateUrl: 'Templates/contactCreate.html',
        controller: 'ContactCreate'
      }).when('/contacts/edit/:id', {
        templateUrl: 'Templates/contactEdit.html',
        controller: 'ContactEdit'
      }).when('/chat', {
        template: '<chat></chat>'
      }).otherwise({
        redirectTo: '/contacts'
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
